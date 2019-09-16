using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using $ext_safeprojectname$.TestConsole.Data;
using $ext_safeprojectname$.TestConsole.Data.Base;
using $ext_safeprojectname$.TestConsole.Data.Enums;
using $ext_safeprojectname$.TestConsole.Helpers;
using NLog;

namespace $ext_safeprojectname$.TestConsole.Providers
{
	public class DialogProvider
	{
		private readonly object consoleGuardian;

		public DialogProvider(object consoleGuardian)
		{
			this.consoleGuardian = consoleGuardian;
		}

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Clears Cosole Screen and draws menu in fullscreen mode of provided options
		/// </summary>
		/// <typeparam name="TReturnType"></typeparam>
		/// <param name="menu"></param>
		/// <param name="brush"></param>
		/// <returns></returns>
		public async Task<TReturnType> AskUser<TReturnType>(Menu<TReturnType> menu, PaintBrush brush)
		{
			Logger.Trace($"Method {nameof(AskUser)} called");

			Console.Clear();

			lock (consoleGuardian)
			{
				brush.RenderMenu(menu);
			}

			ConsoleKey key;
			do
			{
				key = await Task.Run(() => Console.ReadKey(true).Key);

				switch (key)
				{
					case ConsoleKey.LeftArrow:
					{
						if (!menu.IsInline)
							break;

						int newIndex = menu.Choices.ToList().IndexOf(menu.SelectedChoice) - 1;
						// overflow validation
						// todo implement "rotating option for Menu model" (if selected q is 0 and direction is UP select last q and vise versa)
						if (newIndex < 0)
							continue;

						lock (consoleGuardian)
						{
							SelectNewOption(menu, brush, newIndex);
						}

						break;
					}
					case ConsoleKey.UpArrow:
					{
						int newIndex = menu.Choices.ToList().IndexOf(menu.SelectedChoice) - 1;
						// overflow validation
						// todo implement "rotating option for Menu model" (if selected q is 0 and direction is UP select last q and vise versa)
						if (newIndex < 0)
							continue;

						lock (consoleGuardian)
						{
							SelectNewOption(menu, brush, newIndex);
						}

						break;
					}
					case ConsoleKey.DownArrow:
					{
						int newIndex = menu.Choices.ToList().IndexOf(menu.SelectedChoice) + 1;

						// todo implement "rotating option for Menu model" (if selected q is 0 and direction is UP select last q and vise versa)
						if (newIndex >= menu.Choices.Count)
							continue;

						lock (consoleGuardian)
						{
							SelectNewOption(menu, brush, newIndex);
						}

						break;
					}
					case ConsoleKey.RightArrow:
					{
						if (!menu.IsInline)
							break;

						int newIndex = menu.Choices.ToList().IndexOf(menu.SelectedChoice) + 1;

						// todo implement "rotating option for Menu model" (if selected q is 0 and direction is UP select last q and vise versa)
						if (newIndex >= menu.Choices.Count)
							continue;

						lock (consoleGuardian)
						{
							SelectNewOption(menu, brush, newIndex);
						}

						break;
					}
				}
			} while (key != ConsoleKey.Enter);

			Console.Clear();

			Logger.Trace($"Method {nameof(AskUser)} ended");

			return menu.SelectedChoice.Key;
		}

		private void SelectNewOption<TReturnType>(Menu<TReturnType> menu, PaintBrush brush, int newIndex)
		{
			lock (consoleGuardian)
			{
				brush.DeselectChoice(menu);
			}

			menu.SelectedChoice = menu.Choices.ToList()[newIndex];

			lock (consoleGuardian)
			{
				brush.SelectChoice(menu);
			}
		}

		// todo Implement PromtUser method


		// todo Implement ShowNotification/overload for AskUser method
		// todo Add overload taking string[] param for multiple-row message
		// todo Add param for more display positions (center, top-left etc..)
		/// <summary>
		/// Shows one-row long message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="brush"></param>
		/// <param name="slowWrite"></param>
		/// <returns></returns>
		public CancellationTokenSource ShowNotification(string message, PaintBrush brush, bool slowWrite = false)
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			Task.Run(() =>
			{
				Canvas notificationCanvas = new Canvas
				{
					Width = message.Length + 5,
					Height = 5,
					RenderPosition = RenderPosition.TopLeft
				};
				// render
				lock (consoleGuardian)
				{
					brush.RenderCanvas(notificationCanvas);
				}

				lock (consoleGuardian)
				{
					if (slowWrite)
					{
						for (int i = 0; i < message.Length; i++)

							brush.Render(
								notificationCanvas,
								new Position(notificationCanvas.CenterXPosition - (int)Math.Ceiling(message.Length / 2.0) + i, 1),
								message[i]);
					}
					else
					{
						notificationCanvas.SetCursorPosition(notificationCanvas.CenterXPosition - (int)Math.Ceiling(message.Length / 2.0), 1);
						Console.Write(message);
					}
				}

				// wait for cancelation
				WaitHandle.WaitAll(new[] { cts.Token.WaitHandle });

				// derender
				lock (consoleGuardian)
				{
					brush.DerenderCanvas(notificationCanvas);
				}
			}, cts.Token);

			return cts;
		}

		//public CancellationToken ShowNotification(Func<string> animatedText, PaintBrush brush, TimeSpan interval)
		//{
		//	throw new NotImplementedException();
		//}
	}
}
