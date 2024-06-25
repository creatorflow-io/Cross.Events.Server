using Cross.TcpClient.Console;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TcpClientServiceCollectionExtensions
    {
        public static IServiceCollection AddTcpClient(this IServiceCollection services, IConfiguration configuration)
        {
			services.Configure<TcpOptions>(configuration);
			services.AddHostedService<TcpClientService>();
			return services;
		}
    }
}
