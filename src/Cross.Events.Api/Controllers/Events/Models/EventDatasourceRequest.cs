using Cross.Events.Domain.AggregateModels.EventAggregate;
using Juice.AspNetCore.Models;

namespace Cross.Events.Api.Controllers.Events.Models
{
    public class EventDatasourceRequest : DatasourceRequest
    {
        public TcpEventStatus? Status { get; set; }
    }
}
