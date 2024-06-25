using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class DefautPoliciesServiceCollectionExtensions
	{

		/// <summary>
		/// Add default authorization policies
		/// <para><c>Authenticated</c> policy</para>
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddDefaultAuthorizationPolicies(this IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.AddPolicy("Authenticated", policy =>
				{
					policy.RequireAuthenticatedUser();
				});
			});
			return services;
		}
	}
}
