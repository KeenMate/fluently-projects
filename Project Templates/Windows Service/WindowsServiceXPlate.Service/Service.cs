using System;
using System.ServiceProcess;
using $ext_safeprojectname$.Data;

namespace $ext_safeprojectname$.Service
{
	public partial class $ext_safeprojectname$ : ServiceBase
	{
		private readonly Bootstrapper bootstrapper;

		public $ext_safeprojectname$()
		{
			bootstrapper = new Bootstrapper(Guid.NewGuid(), new Configuration());

			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			bootstrapper.Start();
		}

		protected override void OnStop()
		{
			bootstrapper.Stop();
		}

		protected override void OnContinue()
		{
			bootstrapper.Continue();

			base.OnContinue();
		}

		protected override void OnPause()
		{
			bootstrapper.Pause();

			base.OnPause();
		}

		protected override void OnShutdown()
		{
			bootstrapper.Shutdown();

			base.OnShutdown();
		}
	}
}
