using Cross.Identity.Api.Services;
using FluentAssertions;
using Juice.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Cross.Identity.Test
{
    public class ViewServicesTest
    {
        private ITestOutputHelper _output;

        public ViewServicesTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "User records >= 2 with query 'a'")]

        public async Task Users_count_should_gte2Async()
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

                services.AddMediatR(options => {
                    options.RegisterServicesFromAssembly(typeof(UserViewService).Assembly);
                });

                services.AddIdentityRepos(options => configuration.GetSection("Cross:MongoDb").Bind(options));

                services.AddScoped<UserViewService>();
            });

            using var scope = resolver.ServiceProvider.CreateScope();
            var userViewService = scope.ServiceProvider.GetRequiredService<UserViewService>();

            var userDatasource = await userViewService.GetDatasourceResultAsync(new Juice.AspNetCore.Models.DatasourceRequest
            {
                Query = "a"
            });

            userDatasource.Count.Should().BeGreaterThanOrEqualTo(2);
            userDatasource.Data.Count.Should().Be((int)userDatasource.Count);
        }

        [Fact(DisplayName = "User records == 3 without query")]

        public async Task Users_count_should_e3Async()
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

                services.AddIdentityRepos(options => configuration.GetSection("Cross:MongoDb").Bind(options));

                services.AddScoped<UserViewService>();
            });

            using var scope = resolver.ServiceProvider.CreateScope();
            var userViewService = scope.ServiceProvider.GetRequiredService<UserViewService>();

            var userDatasource = await userViewService.GetDatasourceResultAsync(new Juice.AspNetCore.Models.DatasourceRequest
            {
            });

            userDatasource.Count.Should().Be(3);

            userDatasource.Data.Count.Should().Be((int)userDatasource.Count);
        }

        [Fact(DisplayName = "User records should be in descending order")]

        public async Task Users_should_be_orderedAsync()
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

                services.AddIdentityRepos(options => configuration.GetSection("Cross:MongoDb").Bind(options));

                services.AddScoped<UserViewService>();
            });

            using var scope = resolver.ServiceProvider.CreateScope();
            var userViewService = scope.ServiceProvider.GetRequiredService<UserViewService>();

            var userDatasource = await userViewService.GetDatasourceResultAsync(new Juice.AspNetCore.Models.DatasourceRequest
            {
                Sorts = new []
                {
                    new Juice.AspNetCore.Models.SortDescriptor
                    {
                        Property = "UserName",
                        Direction = Juice.AspNetCore.Models.SortDirection.Desc
                    }
                }
            });

            userDatasource.Data.Count.Should().Be((int)userDatasource.Count);

            var usernames = userDatasource.Data.Select(u => u.UserName);

            usernames.Should().BeInDescendingOrder();
        }

        [Fact(DisplayName = "Roles count should be 2 with query 'a'")]
        public async Task Roles_count_should_be2Async()
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

                services.AddIdentityRepos(options => configuration.GetSection("Cross:MongoDb").Bind(options));

                services.AddScoped<RoleViewService>();
            });

            using var scope = resolver.ServiceProvider.CreateScope();
            var userViewService = scope.ServiceProvider.GetRequiredService<RoleViewService>();

            var userDatasource = await userViewService.GetDatasourceResultAsync(new Juice.AspNetCore.Models.DatasourceRequest
            {
                Query = "a"
            });

            userDatasource.Count.Should().Be(2);
            userDatasource.Data.Count.Should().Be((int)userDatasource.Count);
        }
    }
}
