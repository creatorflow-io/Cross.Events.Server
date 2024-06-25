using Cross.Events.Api.Mertics;
using Cross.Events.Api.TcpServer.Behaviors;
using Cross.TcpServer.Core;
using Juice;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class TcpServerMediatorServiceCollectionExtensions
	{
		public static IServiceCollection AddTcpServerMediatorBehaviors(this IServiceCollection services)
		{

			services.AddSingleton<EventMetrics>();

			services.AddScoped(typeof(IPipelineBehavior<TcpMessageRequest,IOperationResult>), 
				typeof(TcpMessageRequestAuthorityBehavior));
			services.AddScoped(typeof(IPipelineBehavior<TcpMessageRequest,IOperationResult>),
				typeof(TcpMessageRequestRateLimitingBehavior));
			return services;
		}
	}
}
