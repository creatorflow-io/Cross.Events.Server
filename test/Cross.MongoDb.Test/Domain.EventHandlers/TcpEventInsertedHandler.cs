using Cross.Events.Domain.AggregateModels.EventAggregate;
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
	internal class TcpEventInsertedHandler : INotificationHandler<DataInserted<TcpEvent>>
	{
		private readonly ILogger _logger;
		private readonly SharedService _sharedService;

		public TcpEventInsertedHandler(ILogger<TcpEventInsertedHandler> logger, SharedService sharedService)
		{
			_logger = logger;
			_sharedService = sharedService;
		}
		public Task Handle(DataInserted<TcpEvent> notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"EventId: {notification.Entity.Id}");
			_sharedService.EventHandlers.Add(nameof(TcpEventInsertedHandler));
			return Task.CompletedTask;
		}
	}
}
