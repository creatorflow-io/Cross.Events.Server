using Cross.TcpServer.Core.Metrics;
using Juice;
using Juice.BgService;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cross.TcpServer.Core.Network
{
	/// <summary>
	/// Background service that listens for incoming TCP connections.
	/// <para>I'm using a managed service from my code base to make it accessible via API.</para>
	/// <para>We can consider to use <see cref="IHostedService"/> instead.</para>
	/// </summary>
	public class ServerListener : Juice.BgService.BackgroundService, IManagedService<TcpServiceModel>
	{
		private TcpListener? _server;
		private ServerOptions _options;
		private CancellationTokenSource _cts = new CancellationTokenSource();

		private IServiceScopeFactory _scopeFactory;
		private TcpServerMetrics? _metrics;

		private int BufferSize = 1024;

		private int _connectionsCount = 0;


		public ServerListener(ILogger<ServerListener> logger,
			IServiceProvider serviceProvider,
			IServiceScopeFactory scopeFactory) : base(logger)
		{
			_scopeFactory = scopeFactory;
			_metrics = serviceProvider.GetService<TcpServerMetrics>();
		}

		protected override async Task ExecuteAsync()
		{
			var tcpPort = _options.Port;
			if (tcpPort <= 0)
			{
				_logger.LogError("Invalid port number: {0}", tcpPort);
				return;
			}
			try
			{
				_server = new TcpListener(IPAddress.Any, tcpPort);
				_server.Start();
				_logger.LogInformation("Server started on port number: {0}", tcpPort);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error starting server on port number: {0}", tcpPort);
				return;
			}
			while (_server.Server.IsBound && !_cts.IsCancellationRequested)
			{
				try
				{
					TcpClient client = await _server.AcceptTcpClientAsync(_cts.Token);
					TcpServerLog.ClientConnected(_logger, client.Client.RemoteEndPoint?.ToString() ?? "unknown");

					Interlocked.Increment(ref _connectionsCount);
					if(_connectionsCount > _options.MaxConnections)
					{
						_logger.LogWarning("Max connections reached. Closing connection.");
						client.Close();
						Interlocked.Decrement(ref _connectionsCount);
						continue;
					}

					_ = Task.Run(() => HandleClientAsync(client, _cts.Token));
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error accepting client connection.");
				}
			}
			try
			{
				_server?.Stop();
				_logger.LogInformation("Server stopped.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error stopping server.");
			}
		}

		private async Task HandleClientAsync(TcpClient client, CancellationToken token)
		{
			var ipEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
			using var _ = _logger.BeginScope($"Processing request from {ipEndPoint?.ToString() ?? "unknown"}");
			try
			{
				_metrics?.IncrementTotalClients(ipEndPoint);
				_metrics?.IncrementHandlingRequests();

				while (client.Connected && !token.IsCancellationRequested)
				{
					using NetworkStream stream = client.GetStream();

					string data = await ReadAsync(stream, token);

					if (!string.IsNullOrWhiteSpace(data))
					{
						_metrics?.IncrementTotalMessages(ipEndPoint);
						TcpServerLog.MessageReceived(_logger, data);
						using var scope = _scopeFactory.CreateScope();
						var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
						TcpServerLog.TcpMessageRequestSending(_logger);
						IOperationResult rs = await mediator.Send(new TcpMessageRequest(ipEndPoint, data, DateTimeOffset.Now), token);
						TcpServerLog.TcpMessageRequestResult(_logger, rs.ToString()??"UnknownStatus");
						if(!rs.Succeeded)
						{
							_metrics?.IncrementTotalErrors();
						}
					}
				}
				if (client.Connected)
				{
					TcpServerLog.ClientDisconnected(_logger, ipEndPoint?.ToString() ?? "unknown");
				}
				else if (token.IsCancellationRequested)
				{
					_logger.LogInformation("Cancellation requested.");
				}
			}
			catch (Exception ex)
			{
				// Handle exceptions
				_logger.LogError($"Error handling client: {ex.Message}");
			}
			finally
			{
				Interlocked.Decrement(ref _connectionsCount);
				_metrics?.DecrementHandlingRequests();
				try
				{
					if (client.Connected)
					{
						client.Close();
					}
				}
				catch { }
			}
		}

		private async Task<string> ReadAsync(NetworkStream stream, CancellationToken token)
		{
			string data = "";
			byte[] buffer = new byte[BufferSize];
			int i;

			// Loop to receive all the data sent by the client.
			while ((i = await stream.ReadAsync(buffer, token)) != 0)
			{
				// Translate data bytes to a ASCII string.
				var bufferedData = Encoding.ASCII.GetString(buffer, 0, i);
				if (_logger.IsEnabled(LogLevel.Debug))
				{
					_logger.LogDebug("<<< Received: {0}", bufferedData);
				}
				data += bufferedData;
			}
			return data;
		}

		public override Task<(bool Healthy, string Message)> HealthCheckAsync()
		{
			if (_server == null || !_server.Server.IsBound)
			{
				return Task.FromResult((false, "ServerListener is not running."));
			}
			return Task.FromResult((true, "ServerListener is running."));
		}

		public void Configure(TcpServiceModel model)
		{
			_options = model.ServerOptions ?? new ServerOptions();
			if (_options.BufferSize > 0)
			{
				BufferSize = _options.BufferSize;
			}
		}
	}

	static partial class TcpServerLog
	{
		[LoggerMessage(1, LogLevel.Debug, "Received: {data}", EventName = "MessageReceived")]
		internal static partial void MessageReceived(ILogger logger, string data);

		[LoggerMessage(2, LogLevel.Debug, "Sending TcpMessageRequest")]
		internal static partial void TcpMessageRequestSending(ILogger logger);

		[LoggerMessage(3, LogLevel.Debug, "TcpMessageRequest {result}", EventName = "TcpMessageRequestProcessed")]
		internal static partial void TcpMessageRequestResult(ILogger logger, string result);

		[LoggerMessage(4, LogLevel.Debug, "Client {client} connected.")]
		internal static partial void ClientConnected(ILogger logger, string client);
		[LoggerMessage(5, LogLevel.Debug, "Client {client} disconnected.")]
		internal static partial void ClientDisconnected(ILogger logger, string client);
	}
}
