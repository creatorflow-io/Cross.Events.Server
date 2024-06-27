using Cross;
using Cross.Events.MongoDB;
using Cross.MongoDB;
using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class MongoRepoServiceCollectionExtensions
	{
		/// <summary>
		/// Try add mongo client and database
		/// </summary>
		/// <param name="services"></param>
		/// <param name="connectionString"></param>
		/// <param name="databaseName"></param>
		/// <param name="dbKey">Use to get <see cref="IMongoDatabase"/> as keyed service</param>
		/// <returns></returns>
		public static IServiceCollection TryAddMongoDatabase(this IServiceCollection services, string connectionString, string databaseName, string? dbKey)
		{
			services.TryAddSingleton<IMongoClient>(new MongoClient(connectionString));

			if (!string.IsNullOrEmpty(dbKey))
			{
				services.TryAddKeyedSingleton(dbKey,
					(sp, key) => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
			}
			return services;
		}

		/// <summary>
		/// Add default mongo repository
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="services"></param>
		/// <param name="connectionString"></param>
		/// <param name="databaseName"></param>
		/// <param name="dbKey">Use to get <see cref="IMongoDatabase"/> as keyed service</param>
		/// <returns></returns>
		public static IServiceCollection AddMongoRepository<T, TKey>(this IServiceCollection services, string connectionString, string databaseName, string dbKey)
			where TKey : IEquatable<TKey>
			where T : class, IIdentifiable<TKey>
			=> services.AddMongoRepository<T, TKey>(options =>
			{
				options.ConnectionString = connectionString;
				options.DatabaseName = databaseName;
			}, dbKey);

		/// <summary>
		/// Add default mongo repository
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="services"></param>
		/// <param name="configure"></param>
		/// <param name="dbKey">Use to get <see cref="IMongoDatabase"/> as keyed service</param>
		/// <returns></returns>
		public static IServiceCollection AddMongoRepository<T, TKey>(this IServiceCollection services, Action<RepositoryOptions> configure, string? dbKey)
			where TKey : IEquatable<TKey>
			where T : class, IIdentifiable<TKey>
		{
			var options = new RepositoryOptions();
			configure(options);

			services.TryAddMongoDatabase(options.ConnectionString, options.DatabaseName, dbKey);

			services.AddScoped(sp => { 
				var db = !string.IsNullOrEmpty(dbKey) 
					? sp.GetRequiredKeyedService<IMongoDatabase>(dbKey)
					: sp.GetRequiredService<IMongoClient>().GetDatabase(options.DatabaseName);
				var mediator = sp.GetService<IMediator>();
				return new MongoRepository<T, TKey>(db, mediator);
			});
			services.AddScoped<IRepository<T, TKey>>(sp => sp.GetRequiredService<MongoRepository<T, TKey>>());
			return services;
		}
	}
}
