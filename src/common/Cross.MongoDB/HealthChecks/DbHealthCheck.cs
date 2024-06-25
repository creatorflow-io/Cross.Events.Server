using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Cross.MongoDB.HealthChecks
{
	internal class DbHealthCheck : IHealthCheck
	{
		private IMongoClient _client;
		public DbHealthCheck(IMongoClient client)
		{
			_client = client;
		}
		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				_client.ListDatabasesAsync();
				return Task.FromResult(HealthCheckResult.Healthy());
			}catch (Exception ex)
			{
				return Task.FromResult(HealthCheckResult.Unhealthy(ex.Message));
			}
		}
	}
}
