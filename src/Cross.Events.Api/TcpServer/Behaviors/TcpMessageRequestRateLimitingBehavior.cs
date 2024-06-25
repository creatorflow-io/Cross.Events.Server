using Cross.TcpServer.Core;
using Juice;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cross.Events.Api.TcpServer.Behaviors
{
	internal class TcpMessageRequestRateLimitingBehavior : IPipelineBehavior<TcpMessageRequest, IOperationResult>
	{
		private readonly ILogger _logger;
		public TcpMessageRequestRateLimitingBehavior(ILogger<TcpMessageRequestRateLimitingBehavior> logger)
		{
			_logger = logger;
		}
		
		public Task<IOperationResult> Handle(TcpMessageRequest request, RequestHandlerDelegate<IOperationResult> next, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Check client rate limiting: {0}", request.ClientEndpoint);
			return next();
		}
	}
}
