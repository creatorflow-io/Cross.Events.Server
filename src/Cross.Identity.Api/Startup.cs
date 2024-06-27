using Juice.Modular;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cross.Identity.Api
{
	[Feature(Required = true)]
	public class Startup : ModuleStartup
	{
		public override void ConfigureServices(IServiceCollection services, IMvcBuilder mvc, IWebHostEnvironment env, IConfiguration configuration)
		{
			// IRepository
			services.AddIdentityRepos(options => configuration.GetSection("Cross:MongoDb").Bind(options));

			// Identity services
			services.AddIdentityCoreServices<ApplicationUser, ApplicationRole>();
			// Identity stores
			services.AddIdentityMongoStores(options => configuration.GetSection("Cross:MongoDb").Bind(options));

			services.AddIdentityApiViewServices();

            services.AddIdentityAuthorizationDefault();
        }

    }
}
