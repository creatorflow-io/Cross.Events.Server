using Cross.Events.Api.Contracts;
using Cross.Events.Api.Hubs;
using Cross.Events.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Events.Api.Domain.EventHandlers
{
	internal class TcpEventProccessedDomainEventHandler : INotificationHandler<TcpEventProcessDomainEvent>
	{
		private readonly IHubContext<EventHub, IEventClient> _hubContext;
		private readonly ILogger _logger;
		public TcpEventProccessedDomainEventHandler(IHubContext<EventHub, IEventClient> hubContext, ILogger<TcpEventProccessedDomainEventHandler> logger)
		{
			_hubContext = hubContext;
			_logger = logger;
		}

		public async Task Handle(TcpEventProcessDomainEvent notification, CancellationToken cancellationToken)
		{
			try
			{
				await _hubContext.Clients.All.StatusChangedAsync(notification.Id, notification.Status.ToString());
				_logger.LogInformation($"Send SignalR StatusChangedAsync: {notification.Id} - {notification.Status}");
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, ex.Message);
			}
		}
	}
}
