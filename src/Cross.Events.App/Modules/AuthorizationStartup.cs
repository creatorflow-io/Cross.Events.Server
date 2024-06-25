using Juice.Modular;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Cross.Events.App.Modules
{
	[Feature(Name = "Authorization", Required = true)]
	public class AuthorizationStartup : ModuleStartup
	{
		public override int StartOrder => 0;
		public override void ConfigureServices(IServiceCollection services, IMvcBuilder mvc, IWebHostEnvironment env, IConfiguration configuration)
		{
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(
					options =>
					{
						options.Authority = configuration.GetSection("OpenIdConnect:Authority").Get<string>();
						options.Audience = configuration.GetSection("OpenIdConnect:Audience").Get<string?>();
						options.RequireHttpsMetadata = false;
					}
				);

			services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerPostConfigureOptions>());

			services.AddDefaultAuthorizationPolicies();
		}
	}
}
