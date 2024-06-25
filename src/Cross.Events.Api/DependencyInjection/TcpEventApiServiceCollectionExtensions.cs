using Cross.Events.Api.Authorization;
using Juice.Extensions.Swagger;
using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TcpEventApiServiceCollectionExtensions
    {
		/// <summary>
		/// Configure SwaggerGen for Background Service
		/// <para>tcpevents-v1</para>
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection ConfigureEventsSwaggerGen(this IServiceCollection services)
        {
            services.ConfigureSwaggerGen(c =>
            {
                c.SwaggerDoc("tcpevents", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Tcp Events API",
                    Description = "Provide tcp events management API"
                });

				c.IncludeReferencedXmlComments();
			});
            return services;
        }


	}
}
