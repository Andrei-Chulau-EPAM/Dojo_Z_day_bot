using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBattleNet.Analitics;
using CodeBattleNet.Core;
using CodeBattleNet.Utilites;
using CodeBattleNetLibrary;

namespace CodeBattleNet.AI
{
    class FightProgram
    {
        private BattleNetClient _client;

        public FightProgram(BattleNetClient client)
        {
            _client = client;
        }

        public bool CanBeUsed(List<MapPoint> enemiesInArea)
        {
            var player = _client.GetPlayerTank();
            return enemiesInArea?.Count > 0;
        }

        public bool CanBeUsed(int viewSize)
        {
            var playerX = _client.PlayerX;
            var playerY = _client.PlayerY;
            var area = new Region(new Region(playerY, playerY, playerX, playerX), viewSize);

            return CanBeUsed(_client.GetEnemiesPoints(area));
        }

        public bool TryPreAct(List<MapPoint> enemiesInArea)
        {
            var playerDirection = _client.GetPlayerDirection();
            var testPosition = new Point(_client.PlayerX, _client.PlayerY);

            return IsEnemiesOnLine(testPosition, playerDirection, enemiesInArea);
        }

        public bool TryPostAct(Direction requestedDirection, List<MapPoint> enemiesInArea)
        {
            var testPosition = new Point(_client.PlayerX, _client.PlayerY);
            var iterator = MapUtility.IterationMap[requestedDirection];

            testPosition = iterator(testPosition);

            if (_client.IsEnemyAt(testPosition))
            {
                return true;
            }

            return IsEnemiesOnLine(testPosition, requestedDirection, enemiesInArea);
        }

        private bool IsEnemiesOnLine(Point zeroPosition, Direction requestedDirection, List<MapPoint> enemiesInArea)
        {
            var testPosition = new Point(zeroPosition.X, zeroPosition.Y);
            var iterator = MapUtility.IterationMap[requestedDirection];

            while (!_client.IsOutOf(testPosition.X, testPosition.Y))
            {
                testPosition = iterator(testPosition);

                if (enemiesInArea.Exists(enemy => enemy.X == testPosition.X && enemy.Y == testPosition.Y))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
