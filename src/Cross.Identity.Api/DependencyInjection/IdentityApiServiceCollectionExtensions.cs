using Cross.Identity.Api.Authorization;
using Cross.Identity.Api.Services;
using Juice.Extensions.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityApiServiceCollectionExtensions
    {
		/// <summary>
		/// Configure SwaggerGen for Identity API
		/// <para>identity</para>
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection ConfigureIdentitySwaggerGen(this IServiceCollection services)
        {
            services.ConfigureSwaggerGen(c =>
            {
                c.SwaggerDoc("identity", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Identity API",
                    Description = "Provide Identity management API"
                });

				c.IncludeReferencedXmlComments();
			});
            return services;
        }

        /// <summary>
        /// Add Microsoft default services for Identity without AddIdentity() call
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityCoreServices<TUser, TRole>(this IServiceCollection services)
            where TUser : class
            where TRole : class
        {
            services.TryAddScoped<IUserValidator<TUser>, UserValidator<TUser>>();
            services.TryAddScoped<IPasswordValidator<TUser>, PasswordValidator<TUser>>();
            services.TryAddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.TryAddScoped<IRoleValidator<TRole>, RoleValidator<TRole>>();
            // No interface for the error describer so we can add errors without rev'ing the interface
            services.TryAddScoped<IdentityErrorDescriber>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<TUser>>();
            services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<TUser>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<TUser>, UserClaimsPrincipalFactory<TUser, TRole>>();
            services.TryAddScoped<IUserConfirmation<TUser>, DefaultUserConfirmation<TUser>>();
            services.TryAddScoped<UserManager<TUser>>();
            services.TryAddScoped<SignInManager<TUser>>();
            services.TryAddScoped<RoleManager<TRole>>();
            return services;
        }

        /// <summary>
        /// Services to query data for view
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityApiViewServices(this IServiceCollection services)
        {
            services.AddScoped<RoleViewService>();
            services.AddScoped<UserViewService>();
            return services;
        }
	}
}
