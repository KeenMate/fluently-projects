using System;
using ConsoleAppTemplate.Options;
using ConsoleAppTemplate.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ConsoleAppTemplate
{
	class Program
	{
		private static IConfigurationRoot configuration;
		private static string[] args;
		private static ServiceProvider serviceProvider;

		static void Main(string[] args)
		{
			Program.args = args;

			InitializeConfiguration();
			InitializeLogger();

			try
			{
				CreateHostBuilder(args).RunConsoleAsync();
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
							.Configure<AppSettings>(configuration.GetSection("App"));
						// .AddHostedService<Bootstrapper>();
					}
				)
				.UseSerilog();
		}

		private static void InitializeConfiguration()
		{
			configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				//.AddEnvironmentVariables()
				.AddCommandLine(args)
				.Build();
		}

		private static void InitializeLogger()
		{
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)
				.CreateLogger();
		}
	}
}
