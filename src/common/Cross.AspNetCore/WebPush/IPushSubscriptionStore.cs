using Lib.Net.Http.WebPush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.AspNetCore.WebPush
{
	public interface IPushSubscriptionStore
	{
		Task StoreSubscriptionAsync(PushSubscription subscription, string user);

		Task DiscardSubscriptionAsync(string endpoint, string user);

		Task<bool> ExistsAsync(string endpoint, string user);

		Task<IEnumerable<PushSubscription>> GetSubscriptionsAsync(string user);
	}
}
