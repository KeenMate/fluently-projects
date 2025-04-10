using System.Collections.Generic;
using ConsoleAppTemplate.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConsoleAppTemplate.Providers
{
	public class DatabaseProvider : IDatabaseProvider
	{
		private readonly ILogger<DatabaseProvider> logger;

		public DatabaseProvider(ILogger<DatabaseProvider> logger)
		{
			this.logger = logger;

			logger.LogInformation("{class} created", nameof(DatabaseProvider));
		}

		public List<string> GetData()
		{
			logger.LogDebug("Fetching real data from DB");
			return new List<string>() { "Hijuju" };
		}
	}
}