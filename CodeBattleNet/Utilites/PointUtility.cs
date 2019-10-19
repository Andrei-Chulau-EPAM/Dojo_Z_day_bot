using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBattleNetLibrary;

namespace CodeBattleNet.Utilites
{
    static class PointUtility
    {
        public static Point CreateNegativePoint() => new Point(-1, -1);

        public static int GetDistance(this Point current, Point target) =>
            Math.Abs(current.X - target.X) + Math.Abs(current.Y - target.Y);

        public static int GetDistance(this Point current, int x, int y) =>
            Math.Abs(current.X - x) + Math.Abs(current.Y - y);
    }
}
