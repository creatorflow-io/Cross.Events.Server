using Cross.Events.Domain.AggregateModels.EventAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.MongoDb.Test.Domain.EventHandlers
{
	internal class TcpEventDeletedHandler : INotificationHandler<DataDeleted<TcpEvent>>
	{
		private readonly ILogger _logger;
		private readonly SharedService _sharedService;

		public TcpEventDeletedHandler(ILogger<TcpEventDeletedHandler> logger, SharedService sharedService)
		{
			_logger = logger;
			_sharedService = sharedService;
		}

		public Task Handle(DataDeleted<TcpEvent> notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"EventId: {notification.Entity.Id}");
			_sharedService.EventHandlers.Add(nameof(TcpEventDeletedHandler));
			return Task.CompletedTask;
		}
	}
}
