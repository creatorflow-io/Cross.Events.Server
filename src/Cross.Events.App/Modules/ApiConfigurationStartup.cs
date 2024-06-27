using Cross.AspNetCore;
using Juice.BgService.Api;
using Juice.Extensions.Swagger;
using Juice.Modular;
using Microsoft.OpenApi.Models;

namespace Cross.Events.App.Modules
{
	[Feature(Name = "ApiConfiguration", Required = true)]
	public class ApiConfigurationStartup : ModuleStartup
	{
		public override int StartOrder => 1;
		public override void ConfigureServices(IServiceCollection services, IMvcBuilder mvc, IWebHostEnvironment env, IConfiguration configuration)
		{
			var authority = configuration.GetSection("OpenIdConnect:Authority").Get<string>();
			services.AddSwaggerGen(c =>
			{
				c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

				c.IgnoreObsoleteActions();

				c.IgnoreObsoleteProperties();

				c.SchemaFilter<SwaggerIgnoreFilter>();

				c.UseInlineDefinitionsForEnums();

				c.DescribeAllParametersInCamelCase();

				c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.OAuth2,
					Flows = new OpenApiOAuthFlows
					{
						AuthorizationCode = new OpenApiOAuthFlow
						{
							AuthorizationUrl = new Uri(authority + "/connect/authorize"),
							TokenUrl = new Uri(authority + "/connect/token"),
							Scopes = new Dictionary<string, string>
							{
								{ "openid", "OpenId" },
								{ "profile", "Profile" },
								{ "events-api", "Events API" }
							}
						}
					},
					Scheme = "Bearer"
				});

				c.OperationFilter<AuthorizeCheckOperationFilter>();
				c.OperationFilter<ReApplyOptionalRouteParameterOperationFilter>();

				c.ParameterFilter<QueryArramParamFilter>();

			});

			services.AddSwaggerGenNewtonsoftSupport();

			services.ConfigureEventsSwaggerGen();

			services.ConfigureBgServiceSwaggerGen();

		}

		public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IWebHostEnvironment env)
		{
			app.UseSwagger(options => options.RouteTemplate = "swagger/{documentName}/swagger.json");
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("tcpevents/swagger.json", "Tcp Events API");
				c.SwaggerEndpoint("bgservice/swagger.json", "Background Service API");

				c.RoutePrefix = "swagger";

				c.OAuthClientId("events_api_swaggerui");
				c.OAuthAppName("Events API Swagger UI");
				c.OAuthUsePkce();
			});
		}
	}
}
