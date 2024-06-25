using Cross.AspNetCore.WebPush;
using Cross.AspNetCore.WebPush.Services;
using Cross.MongoDB;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class WebPushServiceCollectionExtensions
	{
		public static IServiceCollection AddWebPushService(this IServiceCollection services, 
			IConfigurationSection configuration,
			Action<RepositoryOptions> configureDb)
		{
			services.Configure<PushNotificationServiceOptions>(configuration);

			var repoOptions = new RepositoryOptions();
			configureDb(repoOptions);

			services.AddScoped<IPushSubscriptionStore>(sp => { 
				var client = sp.GetRequiredService<IMongoClient>();
				return new PushSubscriptionMongoStore(client.GetDatabase(repoOptions.DatabaseName));
			});

			services.AddHttpClient<PushServiceClient>();
			services.AddSingleton<IVapidTokenCache, MemoryVapidTokenCache>();
			services.AddSingleton<IPushNotificationService, PushServicePushNotificationService>();
			return services;
		}

	}
}
