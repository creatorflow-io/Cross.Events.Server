using Duende.IdentityServer.Models;

namespace Cross.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("roles", new[] { "role" })
        };

    public static IEnumerable<ApiResource> ApiResources =>
		new ApiResource[]
        {
			new ApiResource("events", "Events API")
            {
				Scopes = { "events-api" }
			},
			new ApiResource("identity", "Identity API")
            {
				Scopes = { "identity-api" }
			}
		};

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("events-api", "Operate your events", new string[]{ "role" }),
            new ApiScope("identity-api", "Manage identity data", new string[]{ "role" }),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "identity_api_swaggerui",
                ClientName = "Swagger UI for Identity API",
                ClientSecrets = { },

                RequireClientSecret = false,

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:11001/swagger/oauth2-redirect.html" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "roles", "identity-api" },

                AllowedCorsOrigins = { "https://localhost:11001" }
            },
            new Client
            {
                ClientId = "events_api_swaggerui",
                ClientName = "Swagger UI for Events API",
                ClientSecrets = { },

                RequireClientSecret = false,

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:12001/swagger/oauth2-redirect.html" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "roles", "events-api" },
                
                AllowedCorsOrigins = { "https://localhost:12001" }
            },
			 new Client
			{
				ClientId = "events_client",
                ClientName = "Angular Client",
				ClientSecrets = { },

				RequireClientSecret = false,

				AllowedGrantTypes = GrantTypes.Code,

				RedirectUris = { "https://localhost:4200/auth/login-completed" },
				FrontChannelLogoutUri = "https://localhost:4200/auth/logout-completed",
				PostLogoutRedirectUris = { "https://localhost:4200/auth/logout-completed" },

				AllowOfflineAccess = true,
				AllowedScopes = { "openid", "profile", "roles", "events-api", "identity-api" },
                AllowedCorsOrigins = { "https://localhost:4200" }
			},
		};
}
