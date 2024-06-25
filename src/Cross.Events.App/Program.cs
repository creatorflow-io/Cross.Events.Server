
using Cross.AspNetCore.WebPush;
using Cross.AspNetCore.WebPush.Models;
using Juice.Extensions.Logging;
using Juice.Modular;
using Lib.Net.Http.WebPush;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFileLogger(builder.Configuration.GetSection("Logging:File"));

builder.AddDiscoveredModules();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowKnownOrigins");
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


app.ConfigureDiscoverdModules(app.Environment);

await SendPushAsync(app);

await app.RunAsync();


async Task SendPushAsync(WebApplication app)
{
	var scope = app.Services.CreateScope();
	var subStore = scope.ServiceProvider.GetRequiredService<IPushSubscriptionStore>();
	var pushService = scope.ServiceProvider.GetRequiredService<IPushNotificationService>();
	var subscriptions = await subStore.GetSubscriptionsAsync("admin");

	if (subscriptions.Any())
	{
		var message = AngularHelper.CreateAngularServiceWorkerMessage("Notice from Cross", 
			"New event added", "Go to site", "https://localhost:4200/events");
		foreach (var sub in subscriptions)
		{
			await pushService.SendNotificationAsync(sub, new PushMessage(message));
		}
	}
}