using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBattleNet.Analitics;
using CodeBattleNet.Core;
using CodeBattleNet.Extensions;
using CodeBattleNet.Utilites;
using CodeBattleNetLibrary;

namespace CodeBattleNet.AI
{
    class WayOutProgram
    {
        private readonly BattleNetClient _client;

        public WayOutProgram(BattleNetClient client)
        {
            _client = client;
        }

        public bool CanBeUsed(int viewSize)
        {
            var playerX = _client.PlayerX;
            var playerY = _client.PlayerY;
            var area = new Region(new Region(playerY, playerY, playerX, playerX), viewSize);

            return PlayerUtility.IsInClosureArea(_client, area);
        }

        public bool TryPreAct(Region area)
        {
            var playerDirection = _client.GetPlayerDirection();
            var testPosition = new Point(_client.PlayerX, _client.PlayerY);

            return IsConstructionsOnLine(testPosition, playerDirection, area);
        }

        private bool IsConstructionsOnLine(Point zeroPosition, Direction requestedDirection, Region area)
        {
            var testPosition = new Point(zeroPosition.X, zeroPosition.Y);
            var iterator = MapUtility.IterationMap[requestedDirection];

            while (!_client.IsOutOf(testPosition) && testPosition.IsOutOf(area))
            {
                testPosition = iterator(testPosition);

                if (_client.IsConstructionAt(testPosition))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
