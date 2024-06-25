// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Juice.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddFileLogger(builder.Configuration.GetSection("Logging:File"));

builder.Services.AddTcpClient(builder.Configuration.GetSection("TcpClient"));

var options = new Juice.Extensions.Logging.File.FileLoggerOptions();
builder.Configuration.GetSection("Logging:File").Bind(options);
var logger = IsolatedLoggerHelper.BuildLogger("ConsoleHost", options);
try
{
	var app = builder.Build();
	logger.LogInformation("Application starting...");

	await app.RunAsync();

	logger.LogInformation("Application stopped");
}
catch (Exception ex)
{
	logger.LogError(ex, ex.Message);
}