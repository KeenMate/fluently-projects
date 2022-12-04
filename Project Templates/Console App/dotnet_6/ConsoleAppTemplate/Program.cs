using System;
using System.Threading.Tasks;
using ConsoleAppTemplate.Options;
using ConsoleAppTemplate.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace ConsoleAppTemplate
{
	class Program
	{
		private static IConfigurationRoot configuration;
		private static string[] args;
		private static ServiceProvider serviceProvider;

		static async Task Main(string[] args)
		{
			Program.args = args;

			InitializeConfiguration();
			InitializeLogger();

			try
			{
				var hostBuilder = CreateHostBuilder(args);
				await hostBuilder.RunConsoleAsync();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "An unhandled exception occurred.");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private static IHostBuilder CreateHostBuilder(string[] args)
		{
			return new HostBuilder()
				.ConfigureServices((hostContext, services) =>
					{
						services
							.AddSingleton(configuration)
							.AddTransient<CalculationProvider>()
							.AddOptions()
							.Configure<AppSettings>(configuration.GetSection("App"))
						 .AddHostedService<Bootstrapper>();
					}
				)
				.UseSerilog();
		}

		private static void InitializeConfiguration()
		{
			configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.AddCommandLine(args)
				.Build();
		}

		private static void InitializeLogger()
		{
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)

				// Enabled for fast asynchronous logging to console, with standard appsettings.json configuration 250k messages took 30s to display, with this 23s
				// blockWhenFull ensures that all messages are printed out properly, without it only bufferSize (default 10000)
				// of messages is printed all out and the rest is a random pick
				.WriteTo.Async(wt => wt.Console(
					theme: AnsiConsoleTheme.Literate,
					restrictedToMinimumLevel: LogEventLevel.Verbose,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]\t {Message:lj} <s:{SourceContext}>{NewLine}{Exception}"), blockWhenFull: true)

				.CreateLogger();
		}
	}
}
