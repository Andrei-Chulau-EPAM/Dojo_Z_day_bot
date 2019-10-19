using System;
using System.Collections.Generic;
using System.Linq;
using CodeBattleNet.Analitics;
using CodeBattleNet.Core;
using CodeBattleNet.Extensions;
using CodeBattleNetLibrary;

namespace CodeBattleNet.Utilites
{
    class PlayerUtility
    {
        public static bool IsInClosureArea(BattleNetClient client, Region area) =>
            IsInClosureArea(client, client.GetPlayerTank(), area, new List<Point>());

        private static bool IsInClosureArea(BattleNetClient client, Point position, Region area, List<Point> checkedPoints)
        {
            if (checkedPoints.Exists(x => x.X == position.X && x.Y == position.Y))
            {
                return true;//skip checking. As far as we checking closure area by "&&" true equivalent to skipping
            }

            if (client.IsOutOf(position))
            {
                return true;
            }

            var iterationMap = MapUtility.IterationMap;

            var topPosition = iterationMap[Direction.Up](position);
            var bottomPosition = iterationMap[Direction.Down](position);
            var leftPosition = iterationMap[Direction.Left](position);
            var rightPosition = iterationMap[Direction.Right](position);

            checkedPoints.Add(position);

            if (client.IsObstacleAt(position))
            {
                return true;
            }

            if (position.IsOutOf(area))
            {
                return false;
            }

            return IsInClosureArea(client, topPosition, area, checkedPoints) &&
                   IsInClosureArea(client, bottomPosition, area, checkedPoints) &&
                   IsInClosureArea(client, leftPosition, area, checkedPoints) &&
                   IsInClosureArea(client, rightPosition, area, checkedPoints);
        }

        public static Point GetClosestConstructionPosition(BattleNetClient client)
        {
            var movements = new List<Movement>();

            return GetClosestConstructionPosition(client, client.GetPlayerTank(), new List<Point>(), movements);
        }

        public static Point GetClosestConstructionPosition(BattleNetClient client, out List<Movement> movements)
        {
            movements = new List<Movement>();

            return GetClosestConstructionPosition(client, client.GetPlayerTank(), new List<Point>(), movements);
        }

        private static Point GetClosestConstructionPosition(BattleNetClient client, Point position, List<Point> checkedPoints, 
            List<Movement> movements)
        {
            return GetClosestPosition(client, position, client.IsConstructionAt, checkedPoints, movements);
        }

        public static Point GetClosestEnemyPosition(BattleNetClient client, out List<Movement> movements, int maxTurnsToCalculate)
        {
            var enemyPosition = PointUtility.CreateNegativePoint();
            movements = new List<Movement>();

            List<List<Point>> checkedPoints = new List<List<Point>>(new []{new List<Point>(new []{client.GetPlayerTank()})});
            List<List<Movement>> movementsInProcess = new List<List<Movement>>(new []{new List<Movement>(new []{Movement.Stop})});
            List<Point> currentRangePoints = new List<Point>(new[] {client.GetPlayerTank()});

            for (int count = 0; count < maxTurnsToCalculate && enemyPosition.IsNegativePoint(); count++)
            {
                enemyPosition = GetClosestEnemyPosition(client,
                    checkedPoints, movementsInProcess,
                    currentRangePoints,
                    out movements);
            }

            if (movements.Any())
            {
                movements.RemoveAt(0);
                return enemyPosition;
            }
            else
            {
                return PointUtility.CreateNegativePoint();
            }
            
        }

        public static List<Movement> GetRoad(BattleNetClient client, Point targetPoint)
        {
            var road = new  List<Movement>();

            List<List<Point>> checkedPoints = new List<List<Point>>(new[] { new List<Point>(new[] { client.GetPlayerTank() }) });
            List<List<Movement>> movementsInProcess = new List<List<Movement>>(new[] { new List<Movement>(new[] { Movement.Stop }) });
            List<Point> currentRangePoints = new List<Point>(new[] { client.GetPlayerTank() });

            while (road.Count == 0)
            {
                road = GetRoad(client, targetPoint,
                    checkedPoints, movementsInProcess,
                    currentRangePoints);
            }

            road.RemoveAt(0);

            return road;
        }

        private static Point GetClosestEnemyPosition(BattleNetClient client, List<List<Point>> checkedPoints,
            List<List<Movement>> movements, List<Point> currentRangePoints, out List<Movement> road)
        {

            if (client.IsAnyOfEnemyAt(currentRangePoints))
            {
                for (int index = 0; index < currentRangePoints.Count; index++)
                {
                    if (client.IsEnemyAt(currentRangePoints[index]))
                    {
                        road = new List<Movement>(movements[index]);
                        return currentRangePoints[index];
                    }
                }
            }

            var iterationMap = MapUtility.IterationMap;
            var nextRangePoints = new List<Point>();
            var nextMovements = new List<List<Movement>>();
            var nextCheckedPoints = new List<List<Point>>();

            for (int index = 0; index < movements.Count; index++)
            {
                foreach (var iteration in iterationMap)
                {
                    var nextPosition = iteration.Value(currentRangePoints[index]);
                    var movement = iteration.Key.ToMovement();

                    if (checkedPoints[index].Exists(x => x.X == nextPosition.X && x.Y == nextPosition.Y))
                    {
                        continue;
                    }

                    if (client.IsOutOf(nextPosition) || client.IsObstacleAt(nextPosition))
                    {
                        continue;
                    }

                    nextRangePoints.Add(nextPosition);

                    var move = new List<Movement>(movements[index]);
                    move.Add(iteration.Key.ToMovement());
                    nextMovements.Add(move);

                    var nextChecked = new List<Point>(checkedPoints[index]);
                    nextChecked.Add(currentRangePoints[index]);
                    nextCheckedPoints.Add(nextChecked);
                }
            }

            checkedPoints.Clear();
            checkedPoints.AddRange(nextCheckedPoints);
            movements.Clear();
            movements.AddRange(nextMovements);
            currentRangePoints.Clear();
            currentRangePoints.AddRange(nextRangePoints);
            road = new List<Movement>();
            return PointUtility.CreateNegativePoint();
        }

        private static List<Movement> GetRoad(BattleNetClient client, Point targetPoint, List<List<Point>> checkedPoints,
            List<List<Movement>> movements, List<Point> currentRangePoints)
        {

            if (currentRangePoints.Exists(x=>x.X == targetPoint.X && x.Y == targetPoint.Y))
            {
                for (int index = 0; index < currentRangePoints.Count; index++)
                {
                    if (currentRangePoints[index].X == targetPoint.X && currentRangePoints[index].Y == targetPoint.Y)
                    {
                        return movements[index];
                    }
                }
            }

            var iterationMap = MapUtility.IterationMap;
            var nextRangePoints = new List<Point>();
            var nextMovements = new List<List<Movement>>();
            var nextCheckedPoints = new List<List<Point>>();

            for (int index = 0; index < movements.Count; index++)
            {
                foreach (var iteration in iterationMap)
                {
                    var nextPosition = iteration.Value(currentRangePoints[index]);
                    var movement = iteration.Key.ToMovement();

                    if (checkedPoints[index].Exists(x => x.X == nextPosition.X && x.Y == nextPosition.Y))
                    {
                        continue;
                    }

                    if (client.IsOutOf(nextPosition) || client.IsObstacleAt(nextPosition))
                    {
                        continue;
                    }

                    nextRangePoints.Add(nextPosition);

                    var move = new List<Movement>(movements[index]);
                    move.Add(iteration.Key.ToMovement());
                    nextMovements.Add(move);

                    var nextChecked = new List<Point>(checkedPoints[index]);
                    nextChecked.Add(currentRangePoints[index]);
                    nextCheckedPoints.Add(nextChecked);
                }
            }

            checkedPoints.Clear();
            checkedPoints.AddRange(nextCheckedPoints);
            movements.Clear();
            movements.AddRange(nextMovements);
            currentRangePoints.Clear();
            currentRangePoints.AddRange(nextRangePoints);

            return new List<Movement>();
        }

        private static Point GetClosestPosition(BattleNetClient client, Point position, Func<Point, bool> successCheck,
            List<Point> checkedPoints, List<Movement> movements)
        {
            if (checkedPoints.Exists(x => x.X == position.X && x.Y == position.Y))
            {
                return PointUtility.CreateNegativePoint();//skip checking.
            }

            if (client.IsOutOf(position) || client.IsObstacleAt(position))
            {
                return PointUtility.CreateNegativePoint();
            }

            var iterationMap = MapUtility.IterationMap;

            var topPosition = iterationMap[Direction.Up](position);
            var bottomPosition = iterationMap[Direction.Down](position);
            var leftPosition = iterationMap[Direction.Left](position);
            var rightPosition = iterationMap[Direction.Right](position);

            checkedPoints.Add(position);

            if (successCheck(position))
            {
                return position;
            }

            var topChildMovements = new List<Movement>(movements);
            var bottomChildMovements = new List<Movement>(movements);
            var rightChildMovements = new List<Movement>(movements);
            var leftChildMovements = new List<Movement>(movements);

            var topEnemyPosition = GetClosestPosition(client, topPosition, successCheck, new List<Point>(checkedPoints), topChildMovements);
            var bottomEnemyPosition = GetClosestPosition(client, bottomPosition, successCheck, new List<Point>(checkedPoints), bottomChildMovements);
            var leftEnemyPosition = GetClosestPosition(client, leftPosition, successCheck, new List<Point>(checkedPoints), leftChildMovements);
            var rightEnemyPosition = GetClosestPosition(client, rightPosition, successCheck, new List<Point>(checkedPoints), rightChildMovements);

            var topMovements = topChildMovements.Count;
            var bottomMovements =  bottomChildMovements.Count;
            var leftMovements = leftChildMovements.Count;
            var rightMovements = rightChildMovements.Count;

            if (topMovements < bottomMovements &&
                topMovements < leftMovements &&
                topMovements < rightMovements)
            {
                movements.Add(Movement.Up);
                movements.AddRange(topChildMovements);
                return topEnemyPosition;
            }

            if (bottomMovements < topMovements &&
                bottomMovements < leftMovements &&
                bottomMovements < rightMovements)
            {
                movements.Add(Movement.Down);
                movements.AddRange(bottomChildMovements);
                return bottomEnemyPosition;
            }

            if (leftMovements < topMovements &&
                leftMovements < bottomMovements &&
                leftMovements < rightMovements)
            {
                movements.Add(Movement.Left);
                movements.AddRange(leftChildMovements);
                return leftEnemyPosition;
            }

            if (rightMovements < topMovements &&
                rightMovements < bottomMovements &&
                rightMovements < leftMovements)
            {
                movements.Add(Movement.Right);
                movements.AddRange(rightChildMovements);
                return rightEnemyPosition;
            }

            return PointUtility.CreateNegativePoint();
        }

    }
}
