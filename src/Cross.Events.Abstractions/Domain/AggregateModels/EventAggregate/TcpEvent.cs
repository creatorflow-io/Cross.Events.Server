using Cross.Events.Domain.Events;
using Juice.Domain;
using MediatR;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Cross.Attibutes;
using Newtonsoft.Json;

namespace Cross.Events.Domain.AggregateModels.EventAggregate
{
    [MongoCollection("TcpEvents")]
    [BsonIgnoreExtraElements]
    public class TcpEvent : AggregateRoot
    {
        
        public DateTimeOffset Timestamp { get; init; }
        public string Message { get; init; }
        public TcpEventStatus Status { get; private set; }
        public DateTimeOffset? ProcessedTime { get; private set; }
        public string? ProcessedBy { get; private set; }

		public TcpEvent(DateTimeOffset timestamp, string message)
        {
			this.NotExceededLength(message, 50);
            Timestamp = timestamp;
            Message = message;
            Status = TcpEventStatus.New;
        }

        [BsonConstructor]
        public TcpEvent() : base()
        {
		}

        public void Process(string? user)
        {
            if(Status == TcpEventStatus.Abandoned)
            {
				this.AddValidationError("Event is already abandoned");
				return;
			}
            if (Status == TcpEventStatus.Processed)
            {
                this.AddValidationError("Event is already being processed");
                return;
            }

            Status = TcpEventStatus.Processed;
            ProcessedTime = DateTimeOffset.Now;
            ProcessedBy = user;
            this.AddDomainEvent(new TcpEventProcessDomainEvent(Id, ProcessedTime.Value, Status));
        }

        public void Abandon(string? user)
        {
            if (Status == TcpEventStatus.Abandoned)
            {
                this.AddValidationError("Event is already abandoned");
                return;
            }
            if(Status == TcpEventStatus.Processed)
            {
				this.AddValidationError("Event is already processed");
				return;
			}
            Status = TcpEventStatus.Abandoned;
            ProcessedTime = DateTimeOffset.Now;
            ProcessedBy = user;
            this.AddDomainEvent(new TcpEventProcessDomainEvent(Id, ProcessedTime.Value, Status));
        }
    }
}
