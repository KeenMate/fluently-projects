using System.Collections;
using System.Diagnostics;
using CorrelationId;
using Template0.Helpers;
using Template0.Middlewares;
using Serilog;
using Template0.Extensions;

namespace Template0;

internal class Program
{
	public static void Main(string[] args)
	{
		string aspnetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

		var builder = WebApplication.CreateBuilder(args);

		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json")
			.AddJsonFile($"appsettings.{aspnetEnvironment}.json", true)
			.AddEnvironmentVariables()
			.Build();

		builder.AddCorrelationId();

		// Add services to the container.
		builder.Services.AddCors(options =>
		{
			options.AddDefaultPolicy(policy => policy
				.SetIsOriginAllowed(IsOriginAllowed)
				.WithMethods("POST")
				.WithHeaders("Content-Type"));
		});

		builder.Services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		builder.Services.AddRouting(options =>
		{
			options.LowercaseQueryStrings = true;
			options.LowercaseUrls = true;
		});
		
		// LOGGER SETTINGS
		Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg)); Serilog.Debugging.SelfLog.Enable(Console.Error);

		builder.Host.UseSerilog();

		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.WriteTo.Console()
			.CreateBootstrapLogger();

		var app = builder.Build();

		app.UseCorrelationId();

		// MIDDLEWARES
		app.UseMiddleware<OnlyOriginMiddleware>();
		app.UseMiddleware<SecurityHeadersMiddleware>();

		// RUNTIME CONFIGURATION

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseStaticFiles();


		app.UseAuthorization();

		app.MapControllers();
		app.UseCors();
		
		app.Run();
	}

	static bool IsOriginAllowed(string origin)
	{
		var allowedOrigins = ConfigExtensions.GetAllowedOrigins();

		return allowedOrigins.Any(x => allowedOrigins.Contains(origin.ToLower()));
	}
}