using Cross.MongoDB.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class HealthChecksBuilderExtensions
	{
		public static IHealthChecksBuilder AddMongoDbCheck(this IHealthChecksBuilder builder)
		{
			builder.AddCheck<DbHealthCheck>("MongoDbHealthCheck");

			return builder;
		}
	}
}
