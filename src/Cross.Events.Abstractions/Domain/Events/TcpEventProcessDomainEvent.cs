using Cross.Events.Domain.AggregateModels.EventAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Events.Domain.Events
{
    public class TcpEventProcessDomainEvent : INotification
    {
        public string Id { get; private set; }
        public DateTimeOffset ProcessedTime { get; private set; }
        public TcpEventStatus Status { get; private set; }

        public TcpEventProcessDomainEvent(string id, DateTimeOffset processedTime, TcpEventStatus status)
        {
			Id = id;
			ProcessedTime = processedTime;
			Status = status;
		}
    }
}
