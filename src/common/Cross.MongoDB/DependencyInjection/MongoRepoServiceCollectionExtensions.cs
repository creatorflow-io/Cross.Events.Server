using Cross;
using Cross.Events.MongoDB;
using Cross.MongoDB;
using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class MongoRepoServiceCollectionExtensions
	{
		public static IServiceCollection AddMongoRepository<T, TKey>(this IServiceCollection services, string connectionString, string databaseName)
			where TKey : IEquatable<TKey>
			where T : class, IIdentifiable<TKey>
			=> services.AddMongoRepository<T, TKey>(options =>
			{
				options.ConnectionString = connectionString;
				options.DatabaseName = databaseName;
			});

		public static IServiceCollection AddMongoRepository<T, TKey>(this IServiceCollection services, Action<RepositoryOptions> configure)
			where TKey : IEquatable<TKey>
			where T : class, IIdentifiable<TKey>
		{
			var options = new RepositoryOptions();
			configure(options);

			services.TryAddSingleton<IMongoClient>(new MongoClient(options.ConnectionString));
			services.AddScoped(sp => {
				var client = sp.GetRequiredService<IMongoClient>();
				var mediator = sp.GetRequiredService<IMediator>();
				return new MongoRepository<T, TKey>(client.GetDatabase(options.DatabaseName), mediator);
			});
			services.AddScoped<IRepository<T, TKey>>(sp => sp.GetRequiredService<MongoRepository<T, TKey>>());
			return services;
		}
	}
}
