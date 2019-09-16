using System;

namespace $ext_safeprojectname$.TestConsole.Data.Enums
{
	public enum MenuAction
	{
		None,
		ServiceStart = ConsoleKey.Q,
		ServicePause = ConsoleKey.W,
		ServiceContinue = ConsoleKey.E,
		ServiceStop = ConsoleKey.R,
		ServiceShutdown = ConsoleKey.D,
		Exit = ConsoleKey.F
	}
}