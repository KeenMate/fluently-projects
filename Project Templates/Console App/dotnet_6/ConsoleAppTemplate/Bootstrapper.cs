using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ConsoleAppTemplate.Interfaces;
using ConsoleAppTemplate.Providers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleAppTemplate
{
	public class Bootstrapper : IHostedService
	{
		private readonly ILogger<Bootstrapper> logger;
		private readonly IDatabaseProvider databaseProvider;

		public Bootstrapper(ILogger<Bootstrapper> logger, IDatabaseProvider databaseProvider)
		{
			this.logger = logger;
			this.databaseProvider = databaseProvider;
		}
		
		public Task StartAsync(CancellationToken cancellationToken)
		{
			logger.LogCritical("Critical thing happening");
			logger.LogError("Error thing happening");
			logger.LogWarning("Warning thing happening");
			logger.LogInformation("Informational thing happening");
			logger.LogDebug("Debug thing happening");
			logger.LogTrace("Trace thing happening");

			var data = databaseProvider.GetData();

			var sw = Stopwatch.StartNew();

			for (int i = 0; i < 250000; i++)
			{
				logger.LogInformation("Iteration: {iteration}", i);
			}

			logger.LogWarning("Time taken: {elapsed}", sw.Elapsed);
			return Task.CompletedTask;

		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("Everything needs to come to an end");

			return Task.CompletedTask;
		}
	}
}