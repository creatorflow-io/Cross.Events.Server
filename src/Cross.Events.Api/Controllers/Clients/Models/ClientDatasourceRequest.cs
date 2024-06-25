using Cross.Events.Domain.AggregateModels.ClientAggregate;
using Juice.AspNetCore.Models;

namespace Cross.Events.Api.Controllers.Clients.Models
{
    public class ClientDatasourceRequest : DatasourceRequest
    {
        public TcpClientStatus? Status { get; set; }
    }
}
