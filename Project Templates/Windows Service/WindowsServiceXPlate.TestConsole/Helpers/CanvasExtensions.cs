using System;
using System.Collections.Generic;
using System.Threading;
using $ext_safeprojectname$.TestConsole.Data.Base;
using $ext_safeprojectname$.TestConsole.Data.Constants;
using $ext_safeprojectname$.TestConsole.Data.Exceptions;

namespace $ext_safeprojectname$.TestConsole.Helpers
{
	public static class CanvasExtensions
	{
		/// <summary>
		/// Sets the Cursor position relatively to canvas
		/// </summary>
		/// <param name="canvas">Map used as relative reference</param>
		/// <param name="left">X Coord</param>
		/// <param name="top">Y Coord</param>
		/// <param name="validateInput"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static void SetCursorPosition(this Canvas canvas, int left, int top, bool validateInput = true)
		{
			if (validateInput)
			{
				// Substract walls
				if (left >= canvas.ContentWidth || left < 0)
					return;// throw new ArgumentOutOfRangeException(nameof(left), $"Parameter is out of Map; width: {canvas.Width}, left: {left}");
				if (top >= canvas.ContentHeight || top < 0)
					return; // throw new ArgumentOutOfRangeException(nameof(top), $"Parameter is out of Map; content height: {canvas.ContentHeight}, top: {top}");
			}

			// + 1 because top-left border is at StartX and StartY
			Console.SetCursorPosition(
				left + canvas.StartX + 1,
				top + canvas.StartY + 1);
		}

		/// <summary>
		/// Useful when using PathFinder
		/// </summary>
		/// <param name="canvas"></param>
		/// <param name="obstacles">All X,Y based coords where obstacles are. X and Y are meant to be <b>ralative</b> to canvas (not absolute to Console)</param>
		/// <exception cref="ArgumentOutOfRangeException">If </exception>
		/// <returns>2 dimensional array of binary values (true means obstacle, false symbolises empty block)</returns>
		public static bool[,] To2DBinaryArray(this Canvas canvas, IEnumerable<Position> obstacles)
		{
			/**
			 * Trim 2 becouse of Width and Height contains walls as well
			 * Implicit value for bool is False -> No need to set it manually
			 */
			bool[,] array = new bool[canvas.ContentHeight, canvas.ContentWidth];

			foreach (Position obstacle in obstacles)
			{
				array[obstacle.Y, obstacle.X] = true;
			}

			return array;
		}

		/// <summary>
		/// A Generic method allowing to convert Array of positions(even classes deriving from position) to 2D
		/// </summary>
		/// <typeparam name="TX"></typeparam>
		/// <typeparam name="TY"></typeparam>
		/// <param name="canvas"></param>
		/// <param name="positions"></param>
		/// <param name="transformation"></param>
		/// <returns></returns>
		public static TX[,] To2DArray<TX, TY>(this Canvas canvas, IEnumerable<TY> positions, Func<TY, TX> transformation)
			where TY : class
		{
			// Method with no current usage, just playing with some generics...

			// Check if TY inherits from Position class
			Type baseType = typeof(TY).BaseType;
			if (baseType == null || baseType != typeof(Position))
				throw new InvalidTypeException();

			TX[,] toReturn = new TX[canvas.ContentHeight, canvas.ContentWidth];

			foreach (TY position in positions)
			{
				Position converted = (Position)(object)position;
				toReturn[converted.Y, converted.X] = transformation(position);
			}

			return toReturn;
		}
	}
}