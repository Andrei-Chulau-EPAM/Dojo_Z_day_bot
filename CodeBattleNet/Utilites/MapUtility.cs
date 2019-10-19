using System;
using System.Collections.Generic;
using CodeBattleNet.Core;
using CodeBattleNetLibrary;

namespace CodeBattleNet.Utilites
{
    static class MapUtility
    {
        public static readonly Dictionary<Direction, Func<Point, Point>> IterationMap = new Dictionary<Direction, Func<Point, Point>>
        {
            {Direction.Down, point => new Point(point.X, point.Y + 1) },
            {Direction.Left, point => new Point(point.X - 1, point.Y) },
            {Direction.Right, point => new Point(point.X + 1, point.Y) },
            {Direction.Up, point => new Point(point.X, point.Y - 1) },
        };
    }
}
