using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.AspNetCore.WebPush.Models
{
	public static class AngularHelper
	{
		public static string CreateAngularServiceWorkerMessage(string title, string body, string actionTitle, string url)
		{
			var notificationPayload = new
			{
				notification = new
				{
					title,
					body,
					icon = "assets/images/logo-cross.svg",
					vibrate = new int[] { 100, 50, 100 },
					data = new
					{
						dateOfArrival = DateTime.Now,
						primaryKey = 1,
						onActionClick = new
						{
							gotoSite = new
							{
								operation = "openWindow",
								url
							}
						}
					},
					actions = new object[] {
					new{
						action= "gotoSite",
						title=  actionTitle
					}
				}
				}
			};
			return JsonConvert.SerializeObject(notificationPayload);
		}

	}
}
