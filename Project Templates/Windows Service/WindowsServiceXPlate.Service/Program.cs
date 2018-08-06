using System.ServiceProcess;

namespace $ext_safeprojectname$.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase.Run(new ServiceBase[]
			{
				new $ext_safeprojectname$()
			});
		}
	}
}
