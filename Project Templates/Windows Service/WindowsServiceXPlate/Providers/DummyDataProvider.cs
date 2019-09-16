using System;
using System.Collections.Generic;
using $ext_safeprojectname$.Data;
using NLog;

namespace $ext_safeprojectname$.Providers
{
	public class DummyDataProvider
	{
		private readonly Logger logger = LogManager.GetLogger("ConsoleLogger");

		private readonly Guid correlationid;
		private readonly Configuration configuration;

		public DummyDataProvider(Guid correlationid, Configuration configuration)
		{
			this.correlationid = correlationid;
			this.configuration = configuration;
		}

		public string GetDummyData()
		{
			logger.Debug($"{correlationid} - Dummy Provider is doing Something, config string value: {configuration.StringValue}");

			return DateTime.Now.ToString("F");
		}
	}
}