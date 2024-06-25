using Cross.Events.Api.Hubs;
using Cross.Events.Domain.AggregateModels.ClientAggregate;
using Cross.Events.Domain.AggregateModels.EventAggregate;
using Juice.Modular;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cross.Events.Api
{
	[Feature(Required = true)]
	public class Startup : ModuleStartup
	{
		public override void ConfigureServices(IServiceCollection services, IMvcBuilder mvc, IWebHostEnvironment env, IConfiguration configuration)
		{
			services.AddMongoRepository<TcpEvent, string>(options => configuration.GetSection("Cross:MongoDb").Bind(options));

			services.AddMongoRepository<TcpClient, string>(options => configuration.GetSection("Cross:MongoDb").Bind(options));

			services.AddTcpServerMediatorBehaviors();

			services.AddTcpServerMetrics();

			services.AddEventsAuthorizationDefault();

			services.AddHttpContextAccessor();
		}

		public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IWebHostEnvironment env)
		{
			routes.MapHub<EventHub>("/eventshub");
		}
	}
}
