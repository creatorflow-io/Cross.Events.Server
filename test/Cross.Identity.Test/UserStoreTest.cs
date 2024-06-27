using FluentAssertions;
using Juice.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Xunit.Abstractions;

namespace Cross.Identity.Test
{
	public class UserStoreTest
	{
		private ITestOutputHelper _output;

		public UserStoreTest(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact(DisplayName = "Get user roles")]
		public async Task User_should_in_roleAsync()
		{
			var resolver = new DependencyResolver();

			resolver.ConfigureServices(services =>
			{
				var configService = services.BuildServiceProvider().GetRequiredService<IConfigurationService>();
				var configuration = configService.GetConfiguration();
				services.AddSingleton(provider => _output);
				services.AddLogging(builder =>
				{
					builder.ClearProviders()
					.AddTestOutputLogger()
					.AddConfiguration(configuration.GetSection("Logging"));
				});

				services.AddIdentity<ApplicationUser, ApplicationRole>()
					.AddMongoStores(options => configuration.GetSection("Cross:MongoDb").Bind(options));
			});

			using var scope = resolver.ServiceProvider.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
			var role = await roleManager.FindByNameAsync("test");
			if(role == null)
			{
                role = new ApplicationRole("test");
                await roleManager.CreateAsync(role);
            }

			var user = await userManager.FindByNameAsync("alice");
			if (user != null)
			{
				var roles = await userManager.GetRolesAsync(user);
				if (!roles.Contains("test"))
				{
					await userManager.AddToRolesAsync(user, new string[] { "test"});
					var isInRole = await userManager.IsInRoleAsync(user, "test");
					Assert.True(isInRole);
					roles = await userManager.GetRolesAsync(user);
				}
				Assert.Contains("test", roles);

				await userManager.RemoveFromRoleAsync(user, "test");
				
				var isInRole1 = await userManager.IsInRoleAsync(user, "test");
				Assert.False(isInRole1);
			}
		}

		[Fact(DisplayName = "Get users in role")]
		public async Task Users_should_in_roleAsync()
		{
			var resolver = new DependencyResolver();

			resolver.ConfigureServices(services =>
			{
				var configService = services.BuildServiceProvider().GetRequiredService<IConfigurationService>();
				var configuration = configService.GetConfiguration();
				services.AddSingleton(provider => _output);
				services.AddLogging(builder =>
				{
					builder.ClearProviders()
					.AddTestOutputLogger()
					.AddConfiguration(configuration.GetSection("Logging"));
				});

				services.AddIdentity<ApplicationUser, ApplicationRole>()
					.AddMongoStores(options => configuration.GetSection("Cross:MongoDb").Bind(options));
			});

			using var scope = resolver.ServiceProvider.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			var users = await userManager.GetUsersInRoleAsync("admin");
			Assert.NotEmpty(users);
		}

        [Fact(DisplayName = "Get users has claim")]
        public async Task Users_should_has_claimAsync()
        {
            var resolver = new DependencyResolver();

            resolver.ConfigureServices(services =>
            {
                var configService = services.BuildServiceProvider().GetRequiredService<IConfigurationService>();
                var configuration = configService.GetConfiguration();
                services.AddSingleton(provider => _output);
                services.AddLogging(builder =>
                {
                    builder.ClearProviders()
                    .AddTestOutputLogger()
                    .AddConfiguration(configuration.GetSection("Logging"));
                });

                services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddMongoStores(options => configuration.GetSection("Cross:MongoDb").Bind(options));
            });

            using var scope = resolver.ServiceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var users = await userManager.GetUsersForClaimAsync(new Claim("name", "Alice Smith"));
            Assert.NotEmpty(users);
        }

        [Fact(DisplayName = "Replace user claim")]
		public async Task User_claim_should_be_replaceAsync()
		{
            var resolver = new DependencyResolver();

            resolver.ConfigureServices(services =>
            {
                var configService = services.BuildServiceProvider().GetRequiredService<IConfigurationService>();
                var configuration = configService.GetConfiguration();
                services.AddSingleton(provider => _output);
                services.AddLogging(builder =>
                {
                    builder.ClearProviders()
                    .AddTestOutputLogger()
                    .AddConfiguration(configuration.GetSection("Logging"));
                });

                services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddMongoStores(options => configuration.GetSection("Cross:MongoDb").Bind(options));
            });

            using var scope = resolver.ServiceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			var claim = new Claim("test", "originvalue");
			var user = await userManager.FindByNameAsync("alice");
			if (user != null)
			{
				if(!user.Claims.Any(c => c.ClaimType == "test" && c.ClaimValue == "originalvalue"))
				{
					var rs = await userManager.AddClaimAsync(user, claim);
					rs.Succeeded.Should().BeTrue();
				}
				var rs1 = await userManager.ReplaceClaimAsync(user, claim, new Claim("test", "newvalue"));
				rs1.Succeeded.Should().BeTrue();

				var claims = await userManager.GetClaimsAsync(user);
				claims.Should().ContainSingle(c => c.Type == "test" && c.Value == "newvalue");
			}
        }
    }
}