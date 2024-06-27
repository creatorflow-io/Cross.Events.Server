
using Cross.Identity.Api.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class IdentityAuthorizationServiceCollectionExtensions
	{

		/// <summary>
		/// Register default authorization for identity
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddIdentityAuthorizationDefault(this IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.AddPolicy(Policies.IdentityAdmin, policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireRole("admin");
				});

				options.AddPolicy(Policies.IdentityReader, policy =>
				{
					policy.RequireAuthenticatedUser();
				});

			});
			return services;
		}

	}
}
