using Cross.Events.Api.Contracts;
using Cross.Events.Domain.AggregateModels.EventAggregate;
using Microsoft.AspNetCore.SignalR;

namespace Cross.Events.Api.Hubs
{
	public class EventHub : Hub<IEventClient>
	{
		public async Task SendNewEventAsync(string id)
		{
			await Clients.All.EventAddedAsync(id);
		}

		public async Task SendChangedEventAsync(string id, TcpEventStatus status)
		{
			await Clients.All.StatusChangedAsync(id, status.ToString());
		}
	}
}
