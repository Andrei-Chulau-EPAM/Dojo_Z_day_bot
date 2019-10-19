using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBattleNet.Analitics;
using CodeBattleNet.Core;
using CodeBattleNet.Utilites;

namespace CodeBattleNet.AI
{
    class DefenseProgram
    {
        private BattleNetClient _client;

        public HashSet<Direction> DangerDirections { get; private set; }

        public DefenseProgram(BattleNetClient client)
        {
            _client = client;
        }

        public bool CanBeUsed()
        {
            var player = _client.GetPlayerTank();
            var iterators = MapUtility.IterationMap;
            DangerDirections = new HashSet<Direction>();
            var bulets = _client.GetBullets();
            var iterationPoints = iterators.ToDictionary(x=>x.Key, x => _client.GetPlayerTank());

            for (int lengthCounter = 0; lengthCounter < 2; lengthCounter++)
            {
                foreach (var iterator in iterators)
                {
                    var key = iterator.Key;
                    iterationPoints[key] = iterator.Value(iterationPoints[key]);
                    if (bulets.Exists(x=>x.X == iterationPoints[key].X && x.Y == iterationPoints[key].Y))
                    {
                        DangerDirections.Add(key);
                    }
                }
            }

            return DangerDirections.Count > 0;
        }


    }
}