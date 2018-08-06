using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using $ext_safeprojectname$.TestConsole.Data.Constants;
using $ext_safeprojectname$.TestConsole.Data.Enums;
using $ext_safeprojectname$.TestConsole.Helpers;

namespace $ext_safeprojectname$.TestConsole.Data
{
	public class Menu<TReturnType>
	{
		// TODO: Add cancelation option

		public string Question { get; set; } = "";

		public Dictionary<TReturnType, string> Choices { get; set; }

		private KeyValuePair<TReturnType, string>? selectedChoice;

		public KeyValuePair<TReturnType, string> SelectedChoice
		{
			get => selectedChoice ?? Choices.First();
			set => selectedChoice = value;
		}

		public string ChoiceFormat { get; set; } = "{0}";

		public bool IsInline { get; set; }

		public int InlineSpacing { get; set; } = 1;

		public RenderPosition Position { get; set; } = RenderPosition.Center;

		public int StartX
		{
			get
			{
				switch (Position)
				{
					case RenderPosition.Left:
					case RenderPosition.TopLeft:
					case RenderPosition.BottomLeft:
						return Margin;
					case RenderPosition.Right:
					case RenderPosition.TopRight:
					case RenderPosition.BottomRight:
						return Console.WindowWidth - MenuWidth - Margin;
					case RenderPosition.Top:
					case RenderPosition.Bottom:
					case RenderPosition.Center:
						return (int)Math.Ceiling((Console.WindowWidth - MenuWidth) / 2.0);
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// Y axis level where the whole menu starts (Question Row included)
		/// </summary>
		public int StartY
		{
			get
			{
				switch (Position)
				{
					case RenderPosition.Top:
					case RenderPosition.TopLeft:
					case RenderPosition.TopRight:
						return Margin;
					case RenderPosition.Left:
					case RenderPosition.Right:
					case RenderPosition.Center:
						return (Console.WindowHeight - (IsInline ? 0 : Rows)) / 2;
					case RenderPosition.Bottom:
					case RenderPosition.BottomLeft:
					case RenderPosition.BottomRight:
						return Console.WindowHeight - (IsInline ? 0 : Rows) - Margin;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// Margin around menu
		/// </summary>
		public int Margin { get; set; } = 1;

		public int CenterXPosition => StartX + (MenuWidth / 2);

		/// <summary>
		/// Returns the widht of the Menu dialog (selector characters included)
		/// </summary>
		public int MenuWidth
		{
			get
			{
				if (IsInline)
					return Choices
									 .Values
									 .Select(x => string.Format(ChoiceFormat, x).Length + LeftSelector.Length + RightSelector.Length + (SelectorDistance * 2))
									 .Aggregate((basex, x) => basex + x)
										 + LeftSelector.Length
										 + RightSelector.Length
										 + (2 * SelectorDistance);

				int maximalOption = Choices.Max(o => string.Format(ChoiceFormat, o.Value).Length);

				return (maximalOption > Question.Length
								 ? maximalOption
								 : Question.Length) + LeftSelector.Length + RightSelector.Length + (SelectorDistance * 2);

			}
		}

		public string LeftSelector { get; set; } = "[";

		public string RightSelector { get; set; } = "]";

		public int SelectorDistance { get; set; } = 1;

		/// <summary>
		/// Returns Total rows for current dialog (The Question/Title included)
		/// </summary>
		public int Rows => !IsInline
			? Choices.Count + (!string.IsNullOrEmpty(Question) ? 0 : 1)
			: 1;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			string margin = CharacterMap.Space.Repeat(Margin),
				inlineSpacing = CharacterMap.Space.Repeat(InlineSpacing),
				leftChoiceSpacing = CharacterMap.Space.Repeat(LeftSelector.Length + SelectorDistance + (IsInline ? InlineSpacing : 0)),
				rightChoiceSpacing = CharacterMap.Space.Repeat(RightSelector.Length + SelectorDistance + (IsInline ? InlineSpacing : 0));

			builder.Append(margin);

			List<string> choices = Choices.Values.ToList();

			choices.ForEach(choice =>
			{
				builder.Append($"{leftChoiceSpacing}" +
				               $"{string.Format(ChoiceFormat, choice)}" +
				               $"{rightChoiceSpacing}" +
				               $"{(!IsInline && choices.IndexOf(choice) != choices.Count - 1 ? "\n" : inlineSpacing)}");
			});

			builder.Append(margin);

			return builder.ToString();
		}
	}

	public class Menu
	{
		public enum OptionChangeDirection
		{
			Up,
			Down
		}
	}
}
