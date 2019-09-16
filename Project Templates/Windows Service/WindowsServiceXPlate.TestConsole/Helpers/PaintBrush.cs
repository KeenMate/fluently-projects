using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using $ext_safeprojectname$.TestConsole.Data;
using $ext_safeprojectname$.TestConsole.Data.Base;
using $ext_safeprojectname$.TestConsole.Data.Constants;
using NLog;

namespace $ext_safeprojectname$.TestConsole.Helpers
{
	public class PaintBrush
	{
		private readonly Logger logger;

		private readonly bool leaveTrail;

		private readonly object consoleGuardian;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="consoleGuardian">Object used to Lock console writer to prevent multiple thread writing</param>
		/// <param name="leaveTrail"></param>
		public PaintBrush(object consoleGuardian, bool leaveTrail = false)
		{
			logger = LogManager.GetLogger("fileLogger");

			this.consoleGuardian = consoleGuardian;

			this.leaveTrail = leaveTrail;
		}

		/// <summary>
		/// Draws basic renderCanvas surrounded by walls
		/// Lets a space for socre board on right side of Console screen
		/// </summary>
		public void RenderCanvas(Canvas canvas)
		{
			logger.Trace($"Method {nameof(RenderCanvas)} called");
			logger.Info($"Rendering renderCanvas {canvas.Title}");

			BookConsole(() =>
			{
				DrawRectangle(canvas.StartX, canvas.StartY, canvas.Width, canvas.Height);

				// Render title
				if (!string.IsNullOrEmpty(canvas.Title))
				{
					canvas.SetCursorPosition(2, -1, false);
					Console.Write(CharacterMap.Space + canvas.Title + CharacterMap.Space);
				}

			});

			logger.Trace($"Method {nameof(RenderCanvas)} ended");
		}

		public void DerenderCanvas(Canvas canvas)
		{
			for (int y = 0; y < canvas.Height; y++)
			{
				for (int x = 0; x < canvas.Width; x++)
				{
					BookConsole(() =>
					{
						canvas.SetCursorPosition(x - 1, y - 1, false);
						Console.Write(CharacterMap.Space);
					});
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="canvas"></param>
		/// <param name="path"></param>
		/// <param name="visibleLength"></param>
		/// <param name="foregroundColor"></param>
		/// <param name="backgroundColor"></param>
		/// <param name="animated"></param>
		/// <param name="underlineTrail"></param>
		/// <param name="underlineChar"></param>
		/// <param name="animationDelay"></param>
		/// <param name="visibleFor"></param>
		/// <param name="renderChar"></param>
		public void ShowPath(
			Canvas canvas,
			List<Position> path,
			int visibleLength = 10,
			ConsoleColor? foregroundColor = null,
			ConsoleColor? backgroundColor = null,
			bool animated = false,
			bool underlineTrail = false,
			char? underlineChar = null,
			int animationDelay = 250,
			int visibleFor = 1000,
			char renderChar = CharacterMap.FullBlock)
		{
			logger.Trace($"Method {nameof(ShowPath)} called");
			logger.Info($"Rendering path of {path.Count} elements on {canvas.Title} renderCanvas");

			BookConsole(() =>
			{
				// No need of derendering => will derendered as visiblePath is rendering/derendering
				if (underlineTrail)
					Render(canvas, path, underlineChar ?? CharacterMap.LightTrail);

				for (int i = 0; i < path.Count; i++)
				{
					Render(canvas, path[i], renderChar,
						foregroundColor ?? Console.ForegroundColor,
						backgroundColor ?? Console.BackgroundColor);

					if (animated)
						Thread.Sleep(animationDelay);

					if (i >= visibleLength)
						Derender(canvas, path[i - visibleLength], underlineTrail, underlineChar);
				}

				for (int i = path.Count - 1 - visibleLength; i < path.Count; i++)
				{
					if (animated)
						Thread.Sleep(animationDelay);

					Derender(canvas, path[i], underlineTrail, underlineChar);
				}
			});

			Thread.Sleep(visibleFor);

			BookConsole(() =>
			{
				// Derender remaining positions
				foreach (var position in path)
					Derender(canvas, position);
			});

			logger.Trace($"Mathod call {nameof(ShowPath)} ended");
		}

		public void RenderInlineMenu<T>(Menu<T> menu)
		{
			// validation
			if (menu.Choices
					.Values
					.Select(x => x.Length + menu.Margin * 2)
					.Aggregate((baseX, x) => baseX + x)
						+ menu.LeftSelector.Length + menu.RightSelector.Length > Console.WindowWidth)
				throw new ArgumentOutOfRangeException(nameof(menu), "Menu is too long");

			// Title disposed

			int startX = menu.StartX;

			string prefix = menu.LeftSelector.PadRight(menu.LeftSelector.Length + menu.SelectorDistance),
				prefixSpacing = CharacterMap.Space.Repeat(prefix.Length),
				postfix = menu.RightSelector.PadLeft(menu.RightSelector.Length + menu.SelectorDistance),
				postfixSpacing = CharacterMap.Space.Repeat(postfix.Length);

			Console.SetCursorPosition(startX, menu.StartY);

			BookConsole(() =>
			{
				foreach (KeyValuePair<T, string> option in menu.Choices)
				{
					bool isSelected = Equals(menu.SelectedChoice, option);
					string optionText = string.Format(menu.ChoiceFormat, option.Value);
					string spacing = CharacterMap.Space.Repeat(menu.InlineSpacing);

					Console.Write(
						$"{(isSelected ? prefix : prefixSpacing)}{optionText}{(isSelected ? postfix : postfixSpacing)}{spacing}");

					startX += prefix.Length
					          + optionText.Length
					          + postfix.Length
					          + spacing.Length;
				}
			});
		}

		///  <summary>
		/// 		Renders Menu in "Full-screen" mode
		///  </summary>
		///  <param name="menu"></param>
		public void RenderMenu<T>(Menu<T> menu)
		{
			logger.Trace($"Method for {nameof(RenderMenu)} called");

			if (menu.IsInline)
			{
				RenderInlineMenu(menu);
				return;
			}

			int startY = menu.StartY;

			// Render title
			if (!string.IsNullOrEmpty(menu.Question))
			{
				BookConsole(() =>
				{
					Console.SetCursorPosition(
						menu.CenterXPosition - (menu.Question.Length / 2),
						startY
					);
					Console.Write(menu.Question);
				});
			}

			startY++;

			// RenderMenu Choices
			foreach (KeyValuePair<T, string> option in menu.Choices)
			{
				bool isSelected = Equals(menu.SelectedChoice, option);

				if (isSelected)
				{
					Console.SetCursorPosition(
						menu.CenterXPosition
						- (option.Value.Length / 2)
						- (menu.SelectorDistance + menu.LeftSelector.Length),
						startY);
					Console.Write(menu.LeftSelector);
				}

				BookConsole(() =>
				{
					Console.SetCursorPosition(menu.CenterXPosition - (option.Value.Length / 2), startY++);
					Console.Write(menu.ChoiceFormat, option.Value);
				});

				if (!isSelected) continue;

				BookConsole(() =>
				{
					Console.Write(menu.RightSelector.PadLeft(menu.RightSelector.Length + menu.SelectorDistance));
				});
			}

			logger.Trace($"Method {nameof(RenderMenu)} ended");
		}

		private int GetLeftSelectorXPos<T>(Menu<T> menu)
		{
			int xPos;

			if (menu.IsInline)
			{
				xPos = menu.StartX;

				List<KeyValuePair<T, string>> choicesList = menu.Choices.ToList();
				int acumulator = 0,
					index = choicesList.IndexOf(menu.SelectedChoice);

				int i = 0;
				while (i < index)
				{
					if (index == 0)
						break;

					acumulator += string.Format(menu.ChoiceFormat, choicesList[i].Value).Length
												+ (menu.SelectorDistance * 2)
												+ menu.LeftSelector.Length
												+ menu.RightSelector.Length
												+ menu.InlineSpacing;
					i++;
				}

				xPos += acumulator;
			}
			else
			{
				xPos = menu.CenterXPosition
					- (menu.SelectedChoice.Value.Length / 2)
					- (menu.SelectorDistance + menu.LeftSelector.Length);
			}

			return xPos;
		}

		private int GetRightSelectorXPos<T>(Menu<T> menu)
		{
			int xPos;

			if (menu.IsInline)
			{
				xPos = menu.StartX;

				List<KeyValuePair<T, string>> choicesList = menu.Choices.ToList();
				int acumulator = 0,
					index = choicesList.IndexOf(menu.SelectedChoice);

				int i = 0;
				do
				{
					acumulator += CharacterMap.Space.Repeat(menu.LeftSelector.Length + menu.SelectorDistance).Length;

					acumulator += string.Format(menu.ChoiceFormat, choicesList[i].Value).Length;
					//+ (menu.SelectorDistance * 2)
					//+ menu.LeftSelector.Length
					//+ menu.RightSelector.Length;

					acumulator += CharacterMap.Space.Repeat(menu.RightSelector.Length + menu.SelectorDistance).Length;

					if (i > 0)
						acumulator += menu.InlineSpacing;
				} while (i++ < index);

				xPos += acumulator - 1; // because of Index
			}
			else
			{
				xPos = menu.CenterXPosition
							 + ((int)Math.Ceiling(menu.SelectedChoice.Value.Length / 2.0) - 1)
							 + menu.SelectorDistance + menu.RightSelector.Length;
			}

			return xPos;
		}

		/// <summary>
		/// Erase selectors from selected choice,
		/// Uses SelectedChoice property of Menu
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="menu"></param>
		public void DeselectChoice<T>(Menu<T> menu)
		{
			logger.Trace($"Method {nameof(DeselectChoice)} called");

			//KeyValuePair<T, string> selectedOptionOld = menu.SelectedChoice ?? menu.Choices.First();

			// Last selected option's index
			int index = menu.Choices.ToList().IndexOf(menu.SelectedChoice);

			BookConsole(() =>
			{
				// Erase left selector
				if (!menu.IsInline)
					Console.SetCursorPosition(
						GetLeftSelectorXPos(menu),
						menu.StartY + 1 + index);
				else
					Console.SetCursorPosition(
						GetLeftSelectorXPos(menu),
						menu.StartY);
				Console.Write(CharacterMap.Space);

				// Erase right selector
				if (!menu.IsInline)
				{
					Console.SetCursorPosition(
						GetRightSelectorXPos(menu),
						menu.StartY + 1 + index);
				}
				else
				{
					Console.SetCursorPosition(
						GetRightSelectorXPos(menu),
						menu.StartY);
				}

				Console.Write(CharacterMap.Space);
			});

			logger.Trace($"Method {nameof(DeselectChoice)} ended");
		}

		/// <summary>
		///	Draws selectors around newly selected choice
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">newIdex param must be in range of menu's options</exception>
		/// <typeparam name="T"></typeparam>
		/// <param name="menu"></param>
		/// <param name="newIndex"></param>
		public void SelectChoice<T>(Menu<T> menu)
		{
			logger.Trace($"Method {nameof(SelectChoice)} called");

			int newIndex = menu.Choices.ToList().IndexOf(menu.SelectedChoice);

			BookConsole(() =>
			{
				if (!menu.IsInline)
					Console.SetCursorPosition(
						GetLeftSelectorXPos(menu),
						menu.StartY + 1 + newIndex);
				else
					Console.SetCursorPosition(
						GetLeftSelectorXPos(menu),
						menu.StartY);
				Console.Write(menu.LeftSelector);

				if (!menu.IsInline)
					Console.SetCursorPosition(
						GetRightSelectorXPos(menu),
						menu.StartY + 1 + newIndex);
				else
					Console.SetCursorPosition(
						GetRightSelectorXPos(menu),
						menu.StartY);
				Console.Write(menu.RightSelector);
			});

			logger.Trace($"Method {nameof(SelectChoice)} ended");
		}

		/// <summary>
		/// Removes character at the specified position relatively to renderCanvas
		/// </summary>
		/// <param name="canvas"></param>
		/// <param name="position"></param>
		/// <param name="trailChar"></param>
		public void Derender(Canvas canvas, Position position, char? trailChar = null)
		{
			Derender(canvas, position, leaveTrail, trailChar);
		}

		private void Derender(Canvas canvas, Position position, bool leaveTrail, char? trailChar = null)
		{
			logger.Trace("Derendering position");

			logger.Debug($"Derendering position {{{position.X}, {position.Y}}} in renderCanvas {canvas.Title}");

			char renderChar = leaveTrail ? (trailChar ?? CharacterMap.LightTrail) : CharacterMap.Space;

			BookConsole(() =>
			{
				canvas.SetCursorPosition(position.X, position.Y);

				Console.Write(renderChar);
			});
		}

		public void Derender(Canvas canvas, IEnumerable<Position> obstacles)
		{
			logger.Trace("Derendering positions");

			Position[] positions = obstacles as Position[] ?? obstacles.ToArray();

			logger.Debug($"Derendering {positions.Length} positions in renderCanvas {canvas.Title}");

			foreach (Position position in positions)
			{
				BookConsole(() =>
				{
					canvas.SetCursorPosition(position.X, position.Y);
					Console.Write(CharacterMap.Space);
				});
			}
		}

		public void Derender(Canvas canvas, bool[,] positions, char? trailChar = null)
		{
			for (int y = 0; y < positions.GetLength(0); y++)
			{
				for (int x = 0; x < positions.GetLength(1); x++)
				{
					Derender(canvas, new Position(x, y), trailChar);
				}
			}
		}

		public void EraseCanvas(Canvas canvas)
		{
			for (int y = 0; y < canvas.ContentHeight; y++)
			{
				for (int x = 0; x < canvas.ContentWidth; x++)
				{
					BookConsole(() =>
					{
						canvas.SetCursorPosition(x, y);
						Console.Write(CharacterMap.Space);
					});
				}
			}
		}

		public void Render(
			Canvas canvas,
			Position position,
			char character,
			ConsoleColor? foregroundColor = null,
			ConsoleColor? backgroundColor = null)
		{
			ConsoleColor baseForegroundColor = Console.ForegroundColor;
			ConsoleColor baseBackgroundColor = Console.BackgroundColor;

			logger.Trace("Rendering position");

			logger.Debug($"Rendering character at {{{position.X}, {position.Y}}} in renderCanvas: {canvas.Title}");

			BookConsole(() =>
			{
				Console.BackgroundColor = backgroundColor ?? baseBackgroundColor;
				Console.ForegroundColor = foregroundColor ?? baseForegroundColor;

				canvas.SetCursorPosition(position.X, position.Y);

				Console.Write(character);

				Console.ForegroundColor = baseForegroundColor;
				Console.BackgroundColor = baseBackgroundColor;
			});
		}

		public void Render(
			Canvas canvas,
			List<Position> obstacles,
			char character,
			ConsoleColor? foregroundColor = null,
			ConsoleColor? backgroundColor = null)
		{
			logger.Trace("Rendering popsitions");

			logger.Debug($"Rendering {obstacles.Count} characters in renderCanvas {canvas.Title}");

			foreach (Position position in obstacles)
				Render(canvas, position, character,
					foregroundColor,
					backgroundColor);
		}

		public void Render(
			Canvas canvas,
			bool[,] positions,
			char trueChar = CharacterMap.DarkTrail,
			char falseChar = CharacterMap.Space)
		{
			for (int y = 0; y < positions.GetLength(0); y++)
			{
				for (int x = 0; x < positions.GetLength(1); x++)
				{
					Render(canvas, new Position(x, y), positions[y, x]
					? trueChar
					: falseChar);
				}
			}
		}

		public void BookConsole(Action what)
		{
			lock (consoleGuardian)
			{
				//int previousXPos = Console.CursorLeft,
				//	previousYPos = Console.CursorTop;
				what();

				//Console.SetCursorPosition(previousXPos, previousYPos);
			}
		}

		public void DrawRectangle(int startX, int startY, int width, int height)
		{
			logger.Trace("Drawing rectangle");

			logger.Debug($"Drawing rectagle of {width} width and {height} height at {{{startX},{startY}}}");

			for (int y = startY; y < startY + height; y++)
			{
				for (int x = startX; x < startX + width; x++)
				{
					BookConsole(() =>
					{
						if (y == startY && x == startX)
						{
							Console.SetCursorPosition(x, y);
							Console.Write(CharacterMap.TopLeftCornerWall);
						}
						else if (y == startY && x == startX + width - 1)
						{
							Console.SetCursorPosition(x, y);
							Console.Write(CharacterMap.TopRightCornerWall);
						}
						else if (y == startY + height - 1 && x == startX)
						{
							Console.SetCursorPosition(x, y);
							Console.Write(CharacterMap.DownLeftCornerWall);
						}
						else if (y == startY + height - 1 && x == startX + width - 1)
						{
							Console.SetCursorPosition(x, y);
							Console.Write(CharacterMap.DownRightCornerWall);
						}
						else if (y == startY || y == startY + height - 1)
						{
							Console.SetCursorPosition(x, y);
							Console.Write(CharacterMap.HorizontalWall);
						}
						else if (x == startX || x == startX + width - 1)
						{
							Console.SetCursorPosition(x, y);
							Console.Write(CharacterMap.VerticalWall);
						}
					});
				}
			}
		}
	}
}