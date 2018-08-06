using System;
using System.Collections.Generic;
using System.Linq;
using $ext_safeprojectname$.TestConsole.Data.Base;
using NLog;

namespace $ext_safeprojectname$.TestConsole.Helpers
{
	public static class PositionExtensions
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		// TODO: Fisnish Converting absolute values to relative values based on canvas
		public static IEnumerable<Position> ToRelative(this IEnumerable<Position> positions, Canvas canvas)
		{
			Logger.Trace($"{nameof(ToRelative)} method called");

			throw new NotImplementedException($"{nameof(ToRelative)} is not Implemented yet.");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="obstacles"></param>
		/// <returns>True if obstaclePosition is equal to atleast one obstacle in obstacles</returns>
		public static bool PredictCollision(this Position position, IEnumerable<Position> obstacles)
		{
			Logger.Trace($"{nameof(PredictCollision)} method called");

			return obstacles.Any(obstacle => obstacle.X == position.X && obstacle.Y == position.Y);
		}

		public static bool Compare(this Position pos1, Position pos2) => pos1.X == pos2.X && pos1.Y == pos2.Y;
	}
}