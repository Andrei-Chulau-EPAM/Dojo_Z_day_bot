using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBattleNet.Analitics;
using CodeBattleNet.Core;
using CodeBattleNetLibrary;

namespace CodeBattleNet.Extensions
{
    static class PointExtension
    {
        public static bool IsOutOf(this Point point, Region area) => 
            point.X < area.Left || point.X > area.Right || point.Y < area.Top || point.Y > area.Bottom;

        public static bool IsNegativePoint(this Point point) => point.X < 0 && point.Y < 0;

        public static Movement GetHorisontalDirectionTo(this Point current, Point target)
        {
            if (target.X < current.X)
            {
                return Movement.Left;
            }

            if (target.X > current.X)
            {
                return Movement.Right;
            }

            return Movement.Stop;
        }

        public static Movement GetVerticalDirectionTo(this Point current, Point target)
        {
            if (target.Y < current.Y)
            {
                return Movement.Up;
            }

            if (target.Y > current.Y)
            {
                return Movement.Down;
            }

            return Movement.Stop;
        }

    }
}
