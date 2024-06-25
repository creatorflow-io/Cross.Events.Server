using Juice.Domain;
using MediatR;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Cross.Events.Domain.AggregateModels
{
	public abstract class AggregateRoot : IAggregateRoot<INotification>, IValidatable, IIdentifiable<string>
	{
		#region inherited properties
		[BsonIgnore]
		[JsonIgnore]
		public IList<INotification> DomainEvents { get; protected set; } = new List<INotification>();

		[BsonIgnore]
		[JsonIgnore]
		public IList<string> ValidationErrors { get; protected set; } = new List<string>();
		#endregion

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public AggregateRoot()
		{
			DomainEvents = new List<INotification>();
			ValidationErrors = new List<string>();
		}
	}
}
