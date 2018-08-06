using System;
using System.Threading.Tasks;
using System.Timers;
using $ext_safeprojectname$.Data.Enums;
using $ext_safeprojectname$.Providers;
using $ext_safeprojectname$.Data;
using NLog;

namespace $ext_safeprojectname$
{
	/// <summary>
	/// User's code
	/// </summary>
	public class Bootstrapper
	{
		public ServiceState CurrentState { get; set; } = ServiceState.Stopped;

		private readonly ILogger logger = LogManager.GetLogger("ConsoleLogger");
		private readonly Guid correlationId;

		private readonly Configuration configuration;

		private readonly DummyDataProvider dummyProvider;

		private readonly Timer mainTimer;

		public Bootstrapper(Guid correlationId, Configuration configuration)
		{
			this.correlationId = correlationId;
			this.configuration = configuration;

			logger.Trace($"{correlationId} - config values: string - {configuration.StringValue}, int - {configuration.IntValue}");

			dummyProvider = new DummyDataProvider(correlationId, configuration);

			mainTimer = new Timer(configuration.TimerInterval);

			mainTimer.Elapsed += (sender, args) =>
			{
				logger.Trace($"{correlationId} - Got dummy data from Dummy provider: {dummyProvider.GetDummyData()}");
			};
		}

		public void Start()
		{
			logger.Debug($"{correlationId} - {nameof(Start)} method Called");

			CurrentState = ServiceState.Started;

			mainTimer.Start();
		}

		public void Stop()
		{
			logger.Debug($"{correlationId} - {nameof(Stop)} method Called");

			CurrentState = ServiceState.Stopped;

			mainTimer.Stop();
		}

		public void Continue()
		{
			logger.Debug($"{correlationId} - {nameof(Continue)} method Called");

			CurrentState = ServiceState.Resumed;

			mainTimer.Start();
		}

		public void Pause()
		{
			logger.Debug($"{correlationId} - {nameof(Pause)} method Called");

			CurrentState = ServiceState.Paused;

			mainTimer.Stop();
		}

		public void Shutdown()
		{
			logger.Debug($"{correlationId} - {nameof(Shutdown)} method Called");

			CurrentState = ServiceState.Shutdowned;

			mainTimer.Stop();
		}
	}
}
