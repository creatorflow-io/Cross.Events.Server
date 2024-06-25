using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Events.Domain.AggregateModels.EventAggregate
{
    public enum TcpEventStatus
    {
        New,
        Processed,
        Abandoned
    }
}
