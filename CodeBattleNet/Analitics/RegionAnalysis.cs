using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBattleNet.Core;
using CodeBattleNet.Utilites;
using CodeBattleNetLibrary;

namespace CodeBattleNet.Analitics
{
    class RegionAnalysis
    {
        private readonly BattleNetClient _client;

        public RegionAnalysis(BattleNetClient client)
        {
            _client = client;
        }

        public Region GetFreeRegion()
        {
            return new Region(
                _client.GetClosestUpperObstacleY(),
                _client.GetClosestDownObstacleY(),
                _client.GetClosestLeftObstacleX(),
                _client.GetRightObstacleX()
                );
        }

        public MapPoint GetClosestEnemy(Region region)
        {
            var x = _client.PlayerX;
            var y = _client.PlayerY;

            return _client
                .GetEnemiesPoints(region)
                .Select(enemy => new {EnemyPoint = enemy, length = enemy.GetDistance(x, y)})
                .OrderBy(calculations => calculations.length)
                .First()
                .EnemyPoint;
        }

    }
}
