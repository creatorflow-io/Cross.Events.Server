using Cross.TcpServer.Core;
using Cross.TcpServer.Core.Dummy;
using Juice;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class DummnyServiceCollectionExtensions
	{
		/// <summary>
		/// Don't use this method in production.
		/// <para>It's unnecessary to call this method if you have already registered assembly with MediatR</para>
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddTcpMessageDummnyHandler(this IServiceCollection services)
		{
			services.AddTransient<IRequestHandler<TcpMessageRequest, IOperationResult>, TcpMessageRequestDummyHandler>();
			return services;
		}
	}
}
