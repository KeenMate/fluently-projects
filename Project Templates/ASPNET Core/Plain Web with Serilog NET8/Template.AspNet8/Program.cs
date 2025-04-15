using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using System.Diagnostics;
using Serilog.Core;
using Template.AspNet8.Enrichers;
using Template.AspNet8.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Template.AspNet8
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string aspnetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

			var builder = WebApplication.CreateBuilder(args);

			var configuration = InitializeConfiguration(aspnetEnvironment);

			builder.Services.AddProblemDetails();
			builder.Services.AddHttpContextAccessor();
			
			// Add services to the container.
			builder.Services.AddSingleton<CorrelationIdLogEventEnricher>();
			
			AddLogging(builder, configuration, new LoggingOptions() { LogColor = true });

			builder.Services.AddRazorPages();

			//builder.Services.AddScoped<IProblemDetailsService>();
			builder.Services.AddScoped<CorrelationIdMiddleware>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseMiddleware<CorrelationIdMiddleware>();
			app.UseStaticFiles();
			app.UseRouting();

			app.UseAuthorization();

			app.MapRazorPages();

			app.Run();
		}

		private static IConfigurationRoot InitializeConfiguration(string aspnetEnvironment)
		{
			// create a full path to the .appsettings.json file
			string secretConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".appsettings.json");
			string secretEnvironmentConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), $".appsettings.{aspnetEnvironment}.json");

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{aspnetEnvironment}.json", true)
				.AddUserSecrets<Program>();

			if (File.Exists(secretConfigFilePath))
				builder.AddJsonStream(File.OpenRead(secretConfigFilePath)); // we have to open file stream because AddJsonFile cannot find files starting with dot
			if (File.Exists(secretEnvironmentConfigFilePath))
				builder.AddJsonStream(File.OpenRead(secretEnvironmentConfigFilePath)); // we have to open file stream because AddJsonFile cannot find files starting with dot

			return builder.AddEnvironmentVariables()
								.Build();
		}

		public static WebApplicationBuilder AddLogging(WebApplicationBuilder builder, IConfigurationRoot configuration,
			LoggingOptions loggingOptions)
		{
			//var loggingBuilder = builder.Logging.ClearProviders();
			//loggingBuilder.EnableEnrichment();
			//loggingBuilder.Services.AddLogEnricher<CorrelationIdLogEnricher>();

			// LOGGER SETTINGS
			Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
			Serilog.Debugging.SelfLog.Enable(Console.Error);

			var buildServiceProvider = builder.Services.BuildServiceProvider();
			var httpContextAccessor = buildServiceProvider.GetService<IHttpContextAccessor>();

			builder.Host.UseSerilog((context, serviceProvider, loggerConfiguration) =>
			{
				
				var enricher = buildServiceProvider.GetRequiredService<CorrelationIdLogEventEnricher>();
				loggerConfiguration
					.ReadFrom.Configuration(configuration)
					.Enrich.With(enricher)
					.WriteTo.Console();

			});

			return builder;
		}
	}

	public class LoggingOptions
	{
		public bool LogColor { get; set; }
	}
}
