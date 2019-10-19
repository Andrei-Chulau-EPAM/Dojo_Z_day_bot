using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBattleNet.Core;
using CodeBattleNet.Extensions;
using CodeBattleNet.Utilites;
using CodeBattleNetLibrary;

namespace CodeBattleNet.AI
{
    class MoveProgram
    {
        private BattleNetClient _client;
        private Random _randomValueGenerator;
        private List<Movement> _movementRoad;
        private Point _movementTarget;
        private MoveType _moveType;

        private Movement[] _possibleMovements = new Movement[]
        {
            Movement.Up,
            Movement.Down,
            Movement.Left,
            Movement.Right,
            Movement.Stop
        };

        public MoveProgram(BattleNetClient client)
        {
            _client = client;
            _moveType = MoveType.Undefined;

            _randomValueGenerator = new Random();
        }

        public Movement GetRandomMovement()
        {
            _moveType = MoveType.Random;
            return _possibleMovements[_randomValueGenerator.Next(_possibleMovements.Length -1)];
        }

        private Movement GetMovementTo(Point point, int distance)
        {
            var player = _client.GetPlayerTank();

            if (point.IsNegativePoint() || point.GetDistance(player) <= distance)
            {
                return GetRandomMovement();
            }

            var horisontalMovement = player.GetHorisontalDirectionTo(point);
            var verticalMovement = player.GetVerticalDirectionTo(point);

            if (horisontalMovement == Movement.Stop)
            {
                return verticalMovement;
            }

            if (verticalMovement == Movement.Stop)
            {
                return horisontalMovement;
            }

            _moveType = MoveType.MovementTo;

            return _randomValueGenerator.Next(1) == 1 ? verticalMovement : horisontalMovement;

        }

        public Movement GetMovementToClosestEnemy()
        {
            var player = _client.GetPlayerTank();
            var closestEnemyPosition = PlayerUtility.GetClosestEnemyPosition(_client, out var movementRoad, 50);

            if (closestEnemyPosition.IsNegativePoint())
            {
                return GetLongTermRandomDirection();
            }

            if (_movementRoad?.Count > 0 && _movementTarget != null &&
                _client.IsEnemyAt(_movementTarget) &&
                player.GetDistance(closestEnemyPosition) == player.GetDistance(_movementTarget) &&
                _moveType == MoveType.MovementToClosestEnemy)
            {
                var movement = _movementRoad.First();
                _movementRoad.RemoveAt(0);
                return movement;
            }

            _movementRoad = movementRoad;
            _moveType = MoveType.MovementToClosestEnemy;

            {
                var movement = _movementRoad.First();
                _movementRoad.RemoveAt(0);
                _movementTarget = closestEnemyPosition;
                return movement;
            }
        }

        public Movement GetPartialMovementToClosestEnemy(int turns)
        {
            if (_moveType == MoveType.PartialMovementToClosestEnemy && _movementRoad?.Count > 0 &&
                _movementTarget != null)
            {
                var movement = _movementRoad.First();
                _movementRoad.RemoveAt(0);
                return movement;
            }

            var player = _client.GetPlayerTank();
            var enemies = new List<Point>();

            enemies.AddRange(_client.GetBotsTanks());
            enemies.AddRange(_client.GetOtherPlayersTanks());

            var closestEnemy = enemies.OrderBy(x => x.GetDistance(player)).First();
            var deltaX = player.X - closestEnemy.X;
            var deltaY = player.Y - closestEnemy.Y;
            var turnsToEnemy = deltaX + deltaY;
            var speedX = (float) deltaX / turnsToEnemy;
            var speedY = (float) deltaY / turnsToEnemy;
            var targetX = (int) (speedX * turns);
            var targetY = (int) (speedY * turns);
            var target = new Point(targetX, targetY);

            var horisontalCorrectionIteration = MapUtility.IterationMap[target.GetHorisontalDirectionTo(player).ToDirection(_client)];
            var verticalCorrectionIteration = MapUtility.IterationMap[target.GetVerticalDirectionTo(player).ToDirection(_client)];

            var horisontalCorrectedPoint = target;
            var verticalCorrectedPoint = target;

            while (_client.IsObstacleAt(horisontalCorrectedPoint) && _client.IsObstacleAt(verticalCorrectedPoint))
            {
                horisontalCorrectedPoint = horisontalCorrectionIteration(horisontalCorrectedPoint);
                verticalCorrectedPoint = verticalCorrectionIteration(verticalCorrectedPoint);
            }

            target = _client.IsObstacleAt(horisontalCorrectedPoint) ? verticalCorrectedPoint : horisontalCorrectedPoint;
            var road = PlayerUtility.GetRoad(_client, target);

            _moveType = MoveType.PartialMovementToClosestEnemy;
            _movementRoad = road;
            _movementTarget = target;

            {
                var movement = _movementRoad.First();
                _movementRoad.RemoveAt(0);
                return movement;
            }
        }

        public Movement GetMovementToClosestConstruction()
        {
            PlayerUtility.GetClosestConstructionPosition(_client, out _movementRoad);

            _moveType = MoveType.MovementToClosestConstruction;

            var movement = _movementRoad.First();
            _movementRoad.RemoveAt(0);
            return movement;
        }

        public Movement GetLongTermRandomDirection()
        {
            var player = _client.GetPlayerTank();

            if (_moveType == MoveType.LongTermDirection && _movementRoad?.Count > 0)
            {
                var movement = _movementRoad.First();
                var nextPoint = MapUtility.IterationMap[movement.ToDirection(_client)](player);

                if (!_client.IsObstacleAt(nextPoint))
                {
                    _movementRoad.RemoveAt(0);
                    return movement;
                }
            }

            {
                var randomMovement = GetRandomMovement();
                var nextPoint = MapUtility.IterationMap[randomMovement.ToDirection(_client)](player);

                while (_client.IsObstacleAt(nextPoint))
                {
                    randomMovement = GetRandomMovement();
                    nextPoint = MapUtility.IterationMap[randomMovement.ToDirection(_client)](player);
                }

                _moveType = MoveType.LongTermDirection;
                _movementRoad = new object[5].Select(x => randomMovement).ToList();

                var movement = _movementRoad.First();
                _movementRoad.RemoveAt(0);
                return movement;
            }
        }

        public Movement GetSafeMovementDirection(HashSet<Direction> dangerDirections)
        {
            _moveType = MoveType.SafeMovement;

            var possibleDirections = MapUtility.IterationMap.Select(x => x.Key).ToArray();
            var safeDirections = possibleDirections.Where(x => !dangerDirections.Contains(x)).ToArray();

            switch (safeDirections.Length)
            {
                case 1:
                    return safeDirections[0].ToMovement();
                case 2:
                    return safeDirections[_randomValueGenerator.Next(1)].ToMovement();
                case 3:
                    return dangerDirections.First().IsVerticalDirection()
                            ? DirectionUility.HorisontalDirections[
                                _randomValueGenerator.Next(DirectionUility.HorisontalDirections.Length - 1)]
                                .ToMovement()
                            : DirectionUility.VerticalDirections[
                                _randomValueGenerator.Next(DirectionUility.VerticalDirections.Length - 1)]
                                .ToMovement()
                        ;
                default:
                    return possibleDirections[_randomValueGenerator.Next(possibleDirections.Length - 1)].ToMovement();
            }

        }

    }
}
