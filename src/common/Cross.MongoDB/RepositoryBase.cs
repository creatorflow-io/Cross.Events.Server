using Cross.Attibutes;
using Cross.MongoDB.Extensions;
using MongoDB.Driver;

namespace Cross.MongoDB
{
	public abstract class RepositoryBase<T> where T : class
	{
		protected IMongoDatabase _database;

		protected RepositoryBase(IMongoDatabase database) { _database = database;}

		public IMongoCollection<T> GetCollection(ReadPreference? readPreference = default)
		{
			return _database.GetCollection<T>(readPreference);
		}
	}
}
