using Cross.Events.Domain.AggregateModels.EventAggregate;
using Cross.MongoDb.Test.Domain.EventHandlers;
using FluentAssertions;
using Juice.Extensions.DependencyInjection;
using Juice.XUnit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Cross.MongoDb.Test
{
	public class RepositotyTests
	{
		private ITestOutputHelper _output;

		public RepositotyTests(ITestOutputHelper output)
		{
			_output = output;
		}

		[IgnoreOnCIFact(DisplayName = "Insert one then delete")]
		public async Task Event_should_insert_then_deleteAsync()
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

				services.AddMongoRepository<TcpEvent, string>(options => configuration.GetSection("Cross:MongoDb").Bind(options), default);

				services.AddMediatR(options => {
					options.RegisterServicesFromAssembly(typeof(RepositotyTests).Assembly);
				});

				services.AddSingleton<SharedService>();
			});

			using var scope = resolver.ServiceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IRepository<TcpEvent, string>>();
			var sharedService = scope.ServiceProvider.GetRequiredService<SharedService>();

			var tcpEvent = new TcpEvent(DateTimeOffset.Now, "test message");
			await repository.InsertAsync(tcpEvent);

			_output.WriteLine($"TcpEvent Id: {tcpEvent.Id}");
			sharedService.EventHandlers.Should().Contain(nameof(TcpEventInsertedHandler));

			var result = await repository.GetByIdAsync(tcpEvent.Id);
			Assert.NotNull(result);

			tcpEvent.Process(default);
			await repository.UpdateAsync(tcpEvent);

			sharedService.EventHandlers.Should().Contain(nameof(TcpEventProcessDomainEventHandler));

			await repository.DeleteAsync(tcpEvent.Id);

			sharedService.EventHandlers.Should().Contain(nameof(TcpEventDeletedHandler));
			result = await repository.GetByIdAsync(tcpEvent.Id);
			Assert.Null(result);
		}
	}
}