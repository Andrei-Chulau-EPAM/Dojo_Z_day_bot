using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBattleNet.Core;

namespace CodeBattleNet.Extensions
{
    static class DirectionExtension
    {

        private static Dictionary<Direction, Movement> _directionMap = new Dictionary<Direction, Movement>
        {
            {Direction.Up, Movement.Up },
            {Direction.Left, Movement.Left },
            {Direction.Right, Movement.Right },
            {Direction.Down, Movement.Down },
        };

        public static Movement ToMovement(this Direction direction)
        {
            return _directionMap[direction];
        }

        public static bool IsVerticalDirection(this Direction direction)
        {
            return direction == Direction.Up || direction == Direction.Down;
        }
        public static bool IsHorisontalDirection(this Direction direction)
        {
            return direction == Direction.Left || direction == Direction.Right;
        }
    }
}
