using Cross.Events.Domain.AggregateModels.ClientAggregate;
using Cross.Events.MongoDB;
using Cross.TcpServer.Core;
using MongoDB.Driver;
using Juice;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using Cross.Events.Api.Mertics;
using Cross.Events.Api.Contracts;

namespace Cross.Events.Api.TcpServer.Behaviors
{
	internal class TcpMessageRequestAuthorityBehavior : IPipelineBehavior<TcpMessageRequest, IOperationResult>
	{
		private ILogger _logger;
		private MongoRepository<TcpClient, string> _repository;
		private EventMetrics _metrics;

		public TcpMessageRequestAuthorityBehavior(ILogger<TcpMessageRequestAuthorityBehavior> logger,
			MongoRepository<TcpClient, string> repository,
			EventMetrics eventMetrics)
		{
			_logger = logger;
			_repository = repository;
			_metrics = eventMetrics;
		}
		public async Task<IOperationResult> Handle(TcpMessageRequest request, RequestHandlerDelegate<IOperationResult> next, CancellationToken cancellationToken)
		{
			if(!await IsAuthorizedClientAsync(request))
			{
				_metrics.IncrementEventsLimited(request.Data?.Split(TcpMessage.MessagesSeparator)?.Length??0, request.ClientEndpoint);
				return OperationResult.Failed("Unauthorized client");
			}
			return await next();
		}

		private async Task<bool> IsAuthorizedClientAsync(TcpMessageRequest request)
		{
			if (request.ClientEndpoint == null)
			{
				_logger.LogDebug("Unauthorized client: null endpoint");
				return false;
			}
			if (IPAddress.IsLoopback(request.ClientEndpoint.Address))
			{
				return true;
			}

			var ipaddress = request.ClientEndpoint.Address.ToString();
			var client = await _repository.GetCollection().Find(x => x.IpAddress == ipaddress).FirstOrDefaultAsync();
			if(client!=null && client.Status == TcpClientStatus.Accepted)
			{
				return true;
			}else if (client == null)
			{
				var newClient = new TcpClient(ipaddress);
				await _repository.InsertAsync(newClient);
			}
			_logger.LogDebug("Unauthorized client: {ipaddress}", ipaddress);
			return false;
		}
	}
}
