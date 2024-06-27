using Juice.Modular;

namespace Cross.Events.App.Modules
{
	/// <summary>
	/// Specify the feature name instead of the default name by the namespace.
	/// </summary>
	[Feature(Name = "Common", Required = true)]
	public class CommonStartup : ModuleStartup
	{
		public override void ConfigureServices(IServiceCollection services, IMvcBuilder mvc, IWebHostEnvironment env, IConfiguration configuration)
		{
            services.AddMediatR(options => {
                options.RegisterServicesFromAssemblies(
                    typeof(CommonStartup).Assembly
                    );
            });

            services.AddHealthChecks()
				.AddMongoDbCheck();

			services.AddMemoryCache();

			var origins = configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();
			services.AddCors(options =>
			{
				options.AddPolicy("AllowKnownOrigins",
							   builder =>
							   {
								   builder.WithOrigins(origins)
								   .AllowAnyMethod()
								   .AllowAnyHeader()
								   .AllowCredentials();
							   });
			});

			services.AddSignalR();

			services.AddHttpContextAccessor();

		}

		public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IWebHostEnvironment env)
		{
			routes.MapControllers();
			routes.MapHealthChecks("/health");
		}
	}
}
