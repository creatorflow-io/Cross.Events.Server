using Cross.Events.Api.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class TcpEventAuthorizationServiceCollectionExtensions
	{

		/// <summary>
		/// Register default authorization for events
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddEventsAuthorizationDefault(this IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.AddPolicy(Policies.EventContributePolicy, policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireRole("contributor", "admin");
				});

				options.AddPolicy(Policies.EventReadPolicy, policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireRole("reader", "contributor", "admin");
				});

				options.AddPolicy(Policies.EventAdminPolicy, policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireRole("admin");
				});

				options.AddPolicy(Policies.ClientContributePolicy, policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireRole("contributor", "admin");
				});
			});
			return services;
		}

		/// <summary>
		/// Register testing authorization for events to work wihout authentication
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddEventsAuthorizationTest(this IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.AddPolicy(Policies.EventContributePolicy, policy =>
				{
					policy.RequireAssertion(context => true);
				});

				options.AddPolicy(Policies.EventReadPolicy, policy =>
				{
					policy.RequireAssertion(context => true);
				});

				options.AddPolicy(Policies.EventAdminPolicy, policy =>
				{
					policy.RequireAssertion(context => true);
				});

				options.AddPolicy(Policies.ClientContributePolicy, policy =>
				{
					policy.RequireAssertion(context => true);
				});
			});
			return services;
		}
	}
}
