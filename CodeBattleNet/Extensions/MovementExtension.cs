using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBattleNet.Core
{
    static class MovementExtension
    {
        private static Dictionary<Movement, Direction> _movementMap = new Dictionary<Movement, Direction>
        {
            {Movement.Up, Direction.Up },
            {Movement.Left, Direction.Left },
            {Movement.Right, Direction.Right },
            {Movement.Down, Direction.Down },
        };

        public static Direction ToDirection(this Movement movement, BattleNetClient client)
        {
            if (movement == Movement.Stop)
            {
                return client.GetPlayerDirection();
            }
            else
            {
                return _movementMap[movement];
            }
        }

    }
}
