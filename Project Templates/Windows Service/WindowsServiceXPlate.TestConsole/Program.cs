using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using $ext_safeprojectname$.Data;
using $ext_safeprojectname$.Data.Enums;
using $ext_safeprojectname$.TestConsole.Data;
using $ext_safeprojectname$.TestConsole.Data.Enums;
using $ext_safeprojectname$.TestConsole.Helpers;
using $ext_safeprojectname$.TestConsole.Providers;

namespace $ext_safeprojectname$.TestConsole
{
	public class Program
	{
		private static MenuAction _action;

		public static object ActionBooker = new object();

		public static object ConsoleGuardian = new object();

		public static Bootstrapper Bootstrapper { get; set; }

		public static MenuAction Action
		{
			get
			{
				lock (ActionBooker)
				{
					return _action;
				}
			}
			set
			{
				lock (ActionBooker)
				{
					_action = value;
				}
			}
		}

		public static void Main(string[] args)
		{
			#region Declarations

			var dialogProvider = new DialogProvider(ConsoleGuardian);

			var menu = new Menu<MenuAction>
			{
				IsInline = true,
				Position = RenderPosition.Top,
				Margin = 0,
				Choices = new Dictionary<MenuAction, string>
				{
					{
						MenuAction.ServiceStart,
						"Q: Start service"
					},
					{
						MenuAction.ServicePause,
						"W: Pause service"
					},
					{
						MenuAction.ServiceContinue,
						"E: Continue service"
					},
					{
						MenuAction.ServiceStop,
						"R: Stop service"
					},
					{
						MenuAction.ServiceShutdown,
						"D: Shutdown service"
					},
					{
						MenuAction.Exit,
						"F: Exit simulator"
					}
				}
			};

			var brush = new PaintBrush(ConsoleGuardian);

			Configure();

			#endregion

			AutoResetEvent autoEvent = new AutoResetEvent(false);

			CancellationTokenSource cancelToken = dialogProvider
				.ShowNotification("You can press F1 for help. Press anything to close this message", brush);

			while (!Console.KeyAvailable) { }

			cancelToken.Cancel();

			ShowMenuCommandWatcher(autoEvent, dialogProvider, menu, brush);

			Bootstrapper = new Bootstrapper(Guid.NewGuid(), new Configuration
			{
				StringValue = System.Configuration.ConfigurationManager.AppSettings["stringvalue"],
				IntValue = int.Parse(System.Configuration.ConfigurationManager.AppSettings["intvalue"]),
				TimerInterval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["timerinterval"])
			});

			do
			{
				autoEvent.WaitOne();

				HandleAction(Bootstrapper, Action);
			} while (Action != MenuAction.Exit);

			if (Bootstrapper.CurrentState == ServiceState.Started)
				Bootstrapper.Stop();
		}

		private static void HandleAction(Bootstrapper bootstrapper, MenuAction action)
		{
			switch (action)
			{
				case MenuAction.ServiceStart:
					if (bootstrapper.CurrentState != ServiceState.Started)
						bootstrapper.Start();
					break;
				case MenuAction.ServicePause:
					if (bootstrapper.CurrentState != ServiceState.Paused)
						bootstrapper.Pause();
					break;
				case MenuAction.ServiceContinue:
					if (bootstrapper.CurrentState != ServiceState.Resumed)
						bootstrapper.Continue();
					break;
				case MenuAction.ServiceStop:
					if (bootstrapper.CurrentState != ServiceState.Stopped)
						bootstrapper.Stop();
					break;
				case MenuAction.ServiceShutdown:
					if (bootstrapper.CurrentState != ServiceState.Shutdowned)
						bootstrapper.Shutdown();
					break;
			}
		}

		public static Task ShowMenuCommandWatcher(
			AutoResetEvent autoResetEvent,
			DialogProvider dialogProvider,
			Menu<MenuAction> menu,
			PaintBrush brush)
		{
			return Task.Run(() =>
			{
				do
				{
					if (!Console.KeyAvailable)
					{
						Thread.Sleep(100);
						continue;
					}

					var key = Console.ReadKey(false).Key;

					switch ((int)key)
					{
						case (int)MenuAction.ServiceStart:
							Action = MenuAction.ServiceStart;
							break;
						case (int)MenuAction.ServiceStop:
							Action = MenuAction.ServiceStop;
							break;
						case (int)MenuAction.ServicePause:
							Action = MenuAction.ServicePause;
							break;
						case (int)MenuAction.ServiceContinue:
							Action = MenuAction.ServiceContinue;
							break;
						case (int)MenuAction.ServiceShutdown:
							Action = MenuAction.ServiceShutdown;
							break;
						case (int)MenuAction.Exit:
							Action = MenuAction.Exit;
							break;
						case (int)ConsoleKey.F1:
							lock (ConsoleGuardian)
							{
								Console.WriteLine("\n" + menu);
							}

							continue;
						default:
							continue;
					}

					autoResetEvent.Set();
				} while (Action != MenuAction.Exit);
			});
		}

		private static void Configure()
		{
			Console.CursorVisible = false;

			Console.WindowHeight = 30;

			Console.WindowWidth = 200;
		}
	}
}
