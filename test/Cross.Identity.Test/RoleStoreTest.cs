using FluentAssertions;
using Juice.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Cross.Identity.Test
{
    public class RoleStoreTest
    {
        private ITestOutputHelper _output;

        public RoleStoreTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Role name should be change")]
        public async Task Role_name_should_be_ChangeAsync()
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

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var role = await roleManager.FindByNameAsync("test");
            if (role == null)
            {
                role = new ApplicationRole("test");
                await roleManager.CreateAsync(role);
            }

            var rs =  await roleManager.SetRoleNameAsync(role, "test1");
            rs.Succeeded.Should().BeTrue();

            role.Name.Should().Be("test1");
            role.NormalizedName.Should().Be("TEST1");

            rs = await roleManager.UpdateAsync(role);
            rs.Succeeded.Should().BeTrue();

            role = await roleManager.FindByNameAsync("test1");
            role.Should().NotBeNull();
        }

    }
}
