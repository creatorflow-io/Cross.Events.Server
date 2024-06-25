using Cross.MongoDB;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cross.AspNetCore.WebPush.Services
{
	internal class PushSubscriptionMongoStore : RepositoryBase<UserPushSubscription>, IPushSubscriptionStore
	{

		public PushSubscriptionMongoStore(IMongoDatabase database)
			: base(database)
		{
			_database = database;
		}

		public async Task DiscardSubscriptionAsync(string endpoint, string user)
		{
			await GetCollection().FindOneAndDeleteAsync(s => s.User == user && s.Subscription.Endpoint == endpoint);
		}

		public async Task<bool> ExistsAsync(string endpoint, string user)
		{
			return await GetCollection().Find(s => s.User == user && s.Subscription.Endpoint == endpoint).AnyAsync();
		}

		public async Task<IEnumerable<PushSubscription>> GetSubscriptionsAsync(string user)
		{
			var usubs = await GetCollection().Find(s => s.User == user).ToListAsync();
			return usubs.Select(s => s.Subscription);
		}

		public async Task StoreSubscriptionAsync(PushSubscription subscription, string user)
		{
			if(await ExistsAsync(subscription.Endpoint, user))
			{
				return;
			}
			var usub = new UserPushSubscription
			{
				User = user,
				Subscription = subscription
			};
			await GetCollection().InsertOneAsync(usub);
		}
	}

	internal class UserPushSubscription
	{
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		public string User { get; set; }
		public PushSubscription Subscription { get; set; }
	}
}
