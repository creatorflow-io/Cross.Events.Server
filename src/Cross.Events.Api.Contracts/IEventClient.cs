
namespace Cross.Events.Api.Contracts
{
	public interface IEventClient
	{
		Task EventAddedAsync(string eventId);
		Task StatusChangedAsync(string eventId, string status);
	}
}
