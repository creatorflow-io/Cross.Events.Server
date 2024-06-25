using Cross.Events.Domain.Commands.Events;
using Cross.Events.Api.Contracts;
using Cross.TcpServer.Core;
using Juice;
using MediatR;
using Microsoft.Extensions.Logging;
using Cross.Events.Api.Mertics;

namespace Cross.Events.Api.TcpServer.CommandHandlers
{
	/// <summary>
	/// Handle wrapped TcpMessageRequest to create TcpEvent
	/// </summary>
	internal class TcpMessageRequestHandler : IRequestHandler<TcpMessageRequest, IOperationResult>
	{
		private IMediator _mediator;
		private ILogger _logger;
		private EventMetrics _metrics;

		public TcpMessageRequestHandler(IMediator mediator, ILogger<TcpMessageRequestHandler> logger,
			EventMetrics metrics)
		{
			_mediator = mediator;
			_logger = logger;
			_metrics = metrics;
		}

		public async Task<IOperationResult> Handle(TcpMessageRequest request, CancellationToken cancellationToken)
		{
			try
			{
				var messages = request.Data.Split(TcpMessage.MessagesSeparator);
				if(messages.Length == 0)
				{
					_logger.LogWarning("No message to handle: {0}", request.Data);
				}
				var count = 0;
				foreach (var message in messages)
				{
					using var _ = _logger.BeginScope($"Handle {message}");
					try
					{
						var tcpMessage = TcpMessage.Parse(message);
						count++;
						var command = new CreateTcpEventCommand(tcpMessage.Timestamp, tcpMessage.Message);
						if (_logger.IsEnabled(LogLevel.Debug))
						{
							_logger.LogDebug("Sending create TcpEvent command: {0}", tcpMessage);
						}
						var rs = await _mediator.Send(command, cancellationToken);
						if (_logger.IsEnabled(LogLevel.Debug))
						{
							_logger.LogDebug("Command {0}", rs);
							if (rs.Succeeded)
							{
								_logger.LogDebug("TcpEvent created: {0}", rs.Data);
							}
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error during create TcpEvent: {0}", message);
					}
				}
				_metrics.IncrementEvents(count, request.ClientEndpoint);
				return OperationResult.Success;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error handling message: {0}", request.Data);
				return OperationResult.Failed(ex);
			}
		}
	}
}
