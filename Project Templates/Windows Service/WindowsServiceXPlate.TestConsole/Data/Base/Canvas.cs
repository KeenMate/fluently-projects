using System;
using System.IO;
using System.Text;
using $ext_safeprojectname$.TestConsole.Data.Enums;

namespace $ext_safeprojectname$.TestConsole.Data.Base
{
	public class Canvas : TextWriter
	{
		private string title;

		private int? startX = null;

		private int? startY = null;

		public int StartX
		{
			get
			{
				if (startX != null)
					return startX.Value;

				switch (RenderPosition)
				{
					case RenderPosition.TopLeft:
						return Margin;
					case RenderPosition.TopRight:
						return Console.WindowWidth - Width + Margin;
					case RenderPosition.Center:
						return (Console.WindowWidth - Width) / 2;
					case RenderPosition.BottomLeft:
						return Margin;
					case RenderPosition.BottomRight:
						return Console.WindowWidth - Width - Margin;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			set => startX = value;
		}

		public int StartY
		{
			get
			{
				if (startY != null)
					return startY.Value;

				switch (RenderPosition)
				{
					case RenderPosition.TopLeft:
						return Margin;
					case RenderPosition.TopRight:
						return Margin;
					case RenderPosition.Center:
						return (Console.WindowHeight - Height) / 2;
					case RenderPosition.BottomLeft:
						return Console.WindowHeight - Height - Margin;
					case RenderPosition.BottomRight:
						return Console.WindowHeight - Height - Margin;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			set => startY = value;
		}

		public RenderPosition RenderPosition { get; set; } = RenderPosition.Center;

		public int Margin { get; set; } = 1;

		public int Width { get; set; }

		public int ContentWidth => Width - 2;

		public int Height { get; set; }

		public int ContentHeight => Height - 2;

		public int CenterXPosition => ContentWidth / 2;

		//public char BorderChar { get; set; }

		public string Title
		{
			get => title;
			set
			{
				if (value.Length > Width)
					throw new ArgumentOutOfRangeException($"{nameof(Title)}: {value} is longer than width of Canvas");

				title = value;
			}
		}

		public override Encoding Encoding => Encoding.ASCII;
	}
}