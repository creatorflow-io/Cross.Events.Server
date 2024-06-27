using Cross.Identity;
using Cross.Identity.Stores;
using Cross.MongoDB;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityMongoDBBuilderExtensions
    {
        /// <summary>
        /// Add MongoDB stores for the Identity framework.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IdentityBuilder AddMongoStores(this IdentityBuilder builder, Action<RepositoryOptions> configure)
        {
            builder.Services.AddIdentityMongoStores(configure);
            return builder;
        }

        /// <summary>
        /// Add MongoDB stores for the Identity framework.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityMongoStores(this IServiceCollection services, Action<RepositoryOptions> configure)
        {
            var options = new RepositoryOptions();
            configure(options);

            services.TryAddMongoDatabase(options.ConnectionString, options.DatabaseName, Constants.DbKey);

            services.AddScoped<IUserStore<ApplicationUser>, UserStore>();
            services.AddScoped<IRoleStore<ApplicationRole>, RoleStore>();

            return services;
        }


        /// <summary>
        /// Add MongoDB repositories for the ApplicationRole, ApplicationUser, and UserRole.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityRepos(this IServiceCollection services, Action<RepositoryOptions> configure)
        {
            var options = new RepositoryOptions();
            configure(options);

            services.TryAddMongoDatabase(options.ConnectionString, options.DatabaseName, Constants.DbKey);

            services.AddMongoRepository<ApplicationUser, Guid>(options.ConnectionString, options.DatabaseName, Constants.DbKey);

            services.AddMongoRepository<ApplicationRole, Guid>(options.ConnectionString, options.DatabaseName, Constants.DbKey);

            return services;
        }
    }
}
