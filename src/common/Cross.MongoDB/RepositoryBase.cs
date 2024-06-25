using Cross.Attibutes;
using MongoDB.Driver;

namespace Cross.MongoDB
{
	public abstract class RepositoryBase<T> where T : class
	{
		protected IMongoDatabase _database;

		protected RepositoryBase(IMongoDatabase database) { _database = database;}

		public IMongoCollection<T> GetCollection(ReadPreference? readPreference = default)
		{
			return _database
			  .WithReadPreference(readPreference ?? ReadPreference.Primary)
			  .GetCollection<T>(GetCollectionName<T>());
		}

		public static string GetCollectionName<TT>() where TT : class
		{
			return (typeof(TT).GetCustomAttributes(typeof(MongoCollectionAttribute), true).FirstOrDefault() as MongoCollectionAttribute)?.CollectionName ?? typeof(T).Name;
		}

	}
}
