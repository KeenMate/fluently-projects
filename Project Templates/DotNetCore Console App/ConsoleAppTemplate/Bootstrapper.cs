using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleAppTemplate
{
	public class Bootstrapper : IHostedService
	{
		private readonly ILogger<Bootstrapper> logger;

		public Bootstrapper(ILogger<Bootstrapper> logger)
		{
			this.logger = logger;
		}
		
		public Task StartAsync(CancellationToken cancellationToken)
		{
			logger.LogCritical("Critical thing happening");
			logger.LogError("Error thing happening");
			logger.LogWarning("Warning thing happening");
			logger.LogInformation("Informational thing happening");
			logger.LogDebug("Debug thing happening");
			logger.LogTrace("Trace thing happening");

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