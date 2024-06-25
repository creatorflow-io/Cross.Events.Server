using Cross.TcpServer.Core;
using Juice.BgService.Api;
using Juice.BgService.Management;
using Juice.Extensions.Swagger;
using Juice.Modular;

namespace Cross.Events.App.Modules
{
	/// <summary>
	/// Specify the feature name instead of the default name by the namespace.
	/// </summary>
	[Feature(Name = "BackgroundService", Required = true)]
	public class BackgroundServiceStartup : ModuleStartup
	{
		public override void ConfigureServices(IServiceCollection services, IMvcBuilder mvc, IWebHostEnvironment env, IConfiguration configuration)
		{
			#region Background service management
			services.AddBgService<TcpServiceModel>(configuration.GetSection("BackgroundService"))
				.UseFileStore(configuration.GetSection("BackgroundService:FileStore"));

			services.UseOptionsMutableFileStore("appsettings.Development.json");

			#endregion
		}

	}
}
