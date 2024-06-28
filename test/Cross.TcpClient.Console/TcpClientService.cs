using Cross.Events.Api.Contracts;
using Juice.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.TcpClient.Console
{
	internal class TcpClientService : IHostedService
	{
		private Task? _background;
		private CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly TcpOptions _options;
		private readonly ILogger _logger;
		private IDisposable? _scope;

		private DefaultStringIdGenerator _idGenerator = new DefaultStringIdGenerator();

		public TcpClientService(IOptions<TcpOptions> options, ILogger<TcpClientService> logger)
		{
			_options = options.Value;
			_logger = logger;
			_scope = _logger.BeginScope(new Dictionary<string, object?>
			{
				["ServiceId"] = Guid.Empty,
				["ServiceDescription"] = "TcpClient"
			});
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			if (_background == null)
			{
				_background = Task.Run(ExecuteAsync);
			}
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_cts.Cancel();
			if (_background != null)
			{
				return Task.WhenAny(_background, Task.Delay(5000));
			}
			return Task.CompletedTask;
		}

		private async Task ExecuteAsync()
		{
			_logger.LogInformation("Starting client service");
			var random = new Random();

			while (!_cts.IsCancellationRequested)
			{
				if(DateTime.Now.Ticks%2 == 0)
					await SendParallelAsync(random.Next(1,5));
				else
					await SendMultipleAsync(random.Next(1, 5));

				await Task.Delay(5000);
			}
			_logger.LogInformation("Client service stopped");
		}

		private async Task SendMultipleAsync(int count = 5)
		{
			using var _ = _logger.BeginScope($"Send {count} messages");
			var messages = new List<TcpMessage>();
			for (int i = 0; i < count; i++)
			{
				messages.Add(new TcpMessage(DateTimeOffset.Now, $"{_idGenerator.GenerateRandomId(6)} Message {i} from client"));;
			}
			await SendAsync(messages.ToArray());
		}

		private Task SendParallelAsync(int count = 5)
		{
			using var _ = _logger.BeginScope($"Send {count} messages in parallel");
			Parallel.For(0, count, async i =>
			{
				await SendAsync(new TcpMessage(DateTimeOffset.Now, $"{_idGenerator.GenerateRandomId(6)} Message {i} from client"));
			});
			return Task.CompletedTask;
		}

		private async Task SendAsync(params TcpMessage[] messages)
		{
			try
			{
				using (var client = new System.Net.Sockets.TcpClient(_options.ServerAddress, _options.ServerPort))
				{
					var message = string.Join(TcpMessage.MessagesSeparator, messages.Select(m => m.ToString()));

					_logger.LogInformation("Sending message to server: {0}", message);
					var stream = client.GetStream();
					var buffer = Encoding.UTF8.GetBytes(message);
					await stream.WriteAsync(buffer, 0, buffer.Length);
					_logger.LogInformation("Message sent to server");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error sending message to server");
			}
		}
	}
}
