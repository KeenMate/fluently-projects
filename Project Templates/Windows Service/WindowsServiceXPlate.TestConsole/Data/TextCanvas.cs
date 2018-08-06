using System;
using System.Collections.Generic;
using $ext_safeprojectname$.TestConsole.Data.Base;
using $ext_safeprojectname$.TestConsole.Helpers;

namespace $ext_safeprojectname$.TestConsole.Data
{
	// TODO: Consider adding optional scroll bar
	public class TextCanvas : Canvas
	{
		/// <summary>
		/// Whole Content buffer
		/// </summary>
		private List<string> Buffer { get; }

		/// <summary>
		/// Visible content (currentRow - 3 up to (currentRow - 3) + ContentHeigth)
		/// </summary>
		public List<string> ScreenBuffer {
			get
			{
				int start;
				if (currentRow <= 3)
					start = 0;
				else
					start = currentRow - 3;

				int end;
				if (start + ContentHeight > Buffer.Count - 1 - start)
					end = Buffer.Count - 1;
				else
					end = start + ContentHeight;

				return Buffer.GetRange(start, end);
			}
		}

		public int PosX { get; set; }

		public int PosY { get; set; }

		private int currentRow;

		/// <summary>
		/// Index of row currently selected row to write on
		/// </summary>
		public int CurrentRow
		{
			get => currentRow;
			set
			{
				if (value > Height || value < 0)
					throw new ArgumentOutOfRangeException(nameof(CurrentRow), "Selected row is out of canvas's height");

				currentRow = value;
			}
		}

		public TextCanvas()
		{
			 Buffer = new List<string>();
		}

		public TextCanvas(List<string> buffer)
		{
			this.Buffer = buffer;
		}

		public void Write(string s, int? row = null, int? col = null)
		{
			this.SetCursorPosition(col ?? PosX, row ?? PosY);
		}

		public override void Write(char c)
		{

		}
	}
}