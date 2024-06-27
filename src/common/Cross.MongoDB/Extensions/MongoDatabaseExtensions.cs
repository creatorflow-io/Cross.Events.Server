using Cross.Attibutes;
using MongoDB.Driver;

namespace Cross.MongoDB.Extensions
{
	public static class MongoDatabaseExtensions
	{
		public static string GetCollectionName<T>() 
			where T : class
		{
			return (typeof(T).GetCustomAttributes(typeof(MongoCollectionAttribute), true).FirstOrDefault() as MongoCollectionAttribute)?.CollectionName ?? typeof(T).Name;
		}

		public static IMongoCollection<T> GetCollection<T>(this IMongoDatabase database, ReadPreference? readPreference = default)
			where T : class
		{
			return database
			  .WithReadPreference(readPreference ?? ReadPreference.Primary)
			  .GetCollection<T>(GetCollectionName<T>());
		}
	}
}
