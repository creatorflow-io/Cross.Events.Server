using Cross.Attibutes;
using Cross.MongoDB;
using Juice.Domain;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Cross.Events.MongoDB
{
	public class MongoRepository<T, TKey> : RepositoryBase<T>, IRepository<T, TKey>
		where TKey : IEquatable<TKey>
		where T : class, IIdentifiable<TKey>
	{
		private readonly IMediator _mediator;
		public MongoRepository(IMongoDatabase database, IMediator mediator): base(database)
		{
			_database = database;
			_mediator = mediator;
		}
		
		private async Task PublishDomainEventsAsync(T entity)
		{
			if (entity is IAggregateRoot<INotification> aggregate)
			{
				var domainEvents = aggregate.DomainEvents;
				foreach (var domainEvent in domainEvents)
				{
					await _mediator.Publish(domainEvent);
				}
				aggregate.ClearDomainEvents();
			}
		}

		private async Task PublishDataInsertedEventAsync(T entity)
		{
			await _mediator.Publish(new DataInserted<T>(entity));
		}
		private async Task PublishDataDeletedEventAsync(T entity)
		{
			await _mediator.Publish(new DataDeleted<T>(entity));
		}

		public async Task InsertAsync(T entity)
		{
			await GetCollection().InsertOneAsync(entity);
			await PublishDataInsertedEventAsync(entity);
		}

		public async Task<T> GetByIdAsync(TKey id) 
		{
			return await GetCollection().Find(Builders<T>.Filter.Eq(p => p.Id, id)).FirstOrDefaultAsync();
		}

		public async Task UpdateAsync(T aggregate)
		{
			await GetCollection().ReplaceOneAsync(Builders<T>.Filter.Eq(p => p.Id, aggregate.Id), aggregate);
			await PublishDomainEventsAsync(aggregate);
		}

		public async Task DeleteAsync(TKey id)
		{
			var entity = await GetByIdAsync(id);
			if (entity == null)
			{
				return;
			}
			await GetCollection().DeleteOneAsync(Builders<T>.Filter.Eq(p => p.Id, id));
			await PublishDataDeletedEventAsync(entity);
		}

	}
}
