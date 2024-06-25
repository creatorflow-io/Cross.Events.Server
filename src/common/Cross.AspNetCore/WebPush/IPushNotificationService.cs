using Lib.Net.Http.WebPush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.AspNetCore.WebPush
{
	public interface IPushNotificationService
	{
		string PublicKey { get; }
		Task SendNotificationAsync(PushSubscription subscription, PushMessage message, CancellationToken cancellationToken = default);
	}
}
