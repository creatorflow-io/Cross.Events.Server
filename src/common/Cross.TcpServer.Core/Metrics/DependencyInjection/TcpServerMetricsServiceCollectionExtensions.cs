using Cross.TcpServer.Core.Metrics;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class TcpServerMetricsServiceCollectionExtensions
	{
		public static IServiceCollection AddTcpServerMetrics(this IServiceCollection services)
		{
			services.AddSingleton<TcpServerMetrics>();
			return services;
		}
	}
}
