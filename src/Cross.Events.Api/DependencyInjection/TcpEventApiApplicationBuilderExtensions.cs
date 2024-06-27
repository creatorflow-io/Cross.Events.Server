using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TcpEventApiApplicationBuilderExtensions
    {
		/// <summary>
		/// Mapping Swagger UI for Background Service with endpoint /tcpevents/swagger
		/// </summary>
		/// <param name="app"></param>
		public static void UseEventsSwaggerUI(this IApplicationBuilder app)
        {
            app.UseSwagger(options => options.RouteTemplate = "tcpevents/swagger/{documentName}/swagger.json");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("tcpevents/swagger.json", "Tcp Events API");
                c.RoutePrefix = "tcpevents/swagger";
            });
        }

    }
}
