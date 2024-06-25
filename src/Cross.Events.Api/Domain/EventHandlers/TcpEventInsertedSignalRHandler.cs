using Cross.Events.Api.Contracts;
using Cross.Events.Api.Hubs;
using Cross.Events.Domain.AggregateModels.EventAggregate;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Cross.Events.Api.Domain.EventHandlers
{
	internal class TcpEventInsertedSignalRHandler : INotificationHandler<DataInserted<TcpEvent>>
	{
		private readonly IHubContext<EventHub, IEventClient> _hubContext;
		private readonly ILogger _logger;

		public TcpEventInsertedSignalRHandler(IHubContext<EventHub, IEventClient> hubContext, 
			ILogger<TcpEventInsertedSignalRHandler> logger)
		{
			_hubContext = hubContext;
			_logger = logger;
		}
		public async Task Handle(DataInserted<TcpEvent> notification, CancellationToken cancellationToken)
		{
			try
			{
				await _hubContext.Clients.All.EventAddedAsync(notification.Entity.Id);
				_logger.LogInformation($"Send SignalR EventAddedAsync: {notification.Entity.Id}");
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, ex.Message);
			}
		}
	}
}
