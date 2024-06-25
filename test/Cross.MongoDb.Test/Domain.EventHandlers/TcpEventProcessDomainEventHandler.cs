using Cross.Events.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.MongoDb.Test.Domain.EventHandlers
{
	internal class TcpEventProcessDomainEventHandler : INotificationHandler<TcpEventProcessDomainEvent>
	{
		private readonly ILogger _logger;
		private readonly SharedService _sharedService;

		public TcpEventProcessDomainEventHandler(ILogger<TcpEventProcessDomainEventHandler> logger, SharedService sharedService)
		{
			_logger = logger;
			_sharedService = sharedService;
		}
		public Task Handle(TcpEventProcessDomainEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"EventId: {notification.Id}");
			_sharedService.EventHandlers.Add(nameof(TcpEventProcessDomainEventHandler));
			return Task.CompletedTask;
		}
	}
}
