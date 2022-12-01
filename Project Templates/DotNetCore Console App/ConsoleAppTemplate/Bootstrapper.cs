using System;
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
			Console.WriteLine("Happening for real");
			logger.LogInformation("Happening for real");

			return Task.CompletedTask;

		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("Everything needs to come to an end");

			return Task.CompletedTask;
		}
	}
}