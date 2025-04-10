using System.Collections.Generic;
using ConsoleAppTemplate.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConsoleAppTemplate.Providers
{
	public class DummyDatabaseProvider : IDatabaseProvider
	{
		private readonly ILogger<DummyDatabaseProvider> logger;

		public DummyDatabaseProvider(ILogger<DummyDatabaseProvider> logger)
		{
			this.logger = logger;

			logger.LogInformation("{class} created", nameof(DummyDatabaseProvider));
		}

		public List<string> GetData()
		{
			logger.LogDebug("Fetching dummy data from DB");
			return new List<string>() { "Hijuju" };
		}
	}
}