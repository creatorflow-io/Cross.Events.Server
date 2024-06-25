using Cross.Events.Domain.AggregateModels.EventAggregate;
using MediatR;

namespace Cross.Events.Api.Domain.EventHandlers
{
	internal class TcpEventInsertedWebPushHandler : INotificationHandler<DataInserted<TcpEvent>>
	{
		/// <summary>
		/// I temporarily don't send push notification to the client because
		/// it requires a specific user to send to instead of sending to all clients.
		/// In the real world, we should implement a notification feature to send notification to a specific user (via webpush, firebase, signalR...)
		/// based on the user's subscription.
 		/// </summary>
		/// <param name="notification"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task Handle(DataInserted<TcpEvent> notification, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
