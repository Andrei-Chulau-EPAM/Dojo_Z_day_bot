using System;
using System.Collections.Generic;
using System.Linq;
using CodeBattleNet.Analitics;
using CodeBattleNet.Utilites;
using CodeBattleNetLibrary;

namespace CodeBattleNet.Core
{
    class BattleNetClient : GameClientBattlecity
    {
        private Elements[] _obstacles = new []
        {
            Elements.BATTLE_WALL,
            Elements.CONSTRUCTION,
            Elements.CONSTRUCTION_DESTROYED_DOWN,
            Elements.CONSTRUCTION_DESTROYED_DOWN_LEFT,
            Elements.CONSTRUCTION_DESTROYED_DOWN_RIGHT,
            Elements.CONSTRUCTION_DESTROYED_DOWN_TWICE,
            Elements.CONSTRUCTION_DESTROYED_LEFT,
            Elements.CONSTRUCTION_DESTROYED_LEFT_RIGHT,
            Elements.CONSTRUCTION_DESTROYED_LEFT_TWICE,
            Elements.CONSTRUCTION_DESTROYED_RIGHT,
            Elements.CONSTRUCTION_DESTROYED_RIGHT_TWICE,
            Elements.CONSTRUCTION_DESTROYED_RIGHT_UP,
            Elements.CONSTRUCTION_DESTROYED_UP,
            Elements.CONSTRUCTION_DESTROYED_UP_DOWN,
            Elements.CONSTRUCTION_DESTROYED_DOWN,
            Elements.CONSTRUCTION_DESTROYED_UP_LEFT,
            Elements.CONSTRUCTION_DESTROYED_UP_TWICE,
        };

        private Elements[] _constructions = new[]
        {
            Elements.CONSTRUCTION,
            Elements.CONSTRUCTION_DESTROYED_DOWN,
            Elements.CONSTRUCTION_DESTROYED_DOWN_LEFT,
            Elements.CONSTRUCTION_DESTROYED_DOWN_RIGHT,
            Elements.CONSTRUCTION_DESTROYED_DOWN_TWICE,
            Elements.CONSTRUCTION_DESTROYED_LEFT,
            Elements.CONSTRUCTION_DESTROYED_LEFT_RIGHT,
            Elements.CONSTRUCTION_DESTROYED_LEFT_TWICE,
            Elements.CONSTRUCTION_DESTROYED_RIGHT,
            Elements.CONSTRUCTION_DESTROYED_RIGHT_TWICE,
            Elements.CONSTRUCTION_DESTROYED_RIGHT_UP,
            Elements.CONSTRUCTION_DESTROYED_UP,
            Elements.CONSTRUCTION_DESTROYED_UP_DOWN,
            Elements.CONSTRUCTION_DESTROYED_DOWN,
            Elements.CONSTRUCTION_DESTROYED_UP_LEFT,
            Elements.CONSTRUCTION_DESTROYED_UP_TWICE,
        };

        private Elements[] _enemies = new[]
        {
            Elements.OTHER_TANK_DOWN,
            Elements.OTHER_TANK_LEFT,
            Elements.OTHER_TANK_RIGHT,
            Elements.OTHER_TANK_UP,
            Elements.AI_TANK_DOWN,
            Elements.AI_TANK_LEFT,
            Elements.AI_TANK_RIGHT,
            Elements.AI_TANK_UP
        };

        private Elements[] _player = new[]
        {
            Elements.TANK_DOWN,
            Elements.TANK_LEFT,
            Elements.TANK_RIGHT,
            Elements.TANK_UP
        };

        private Dictionary<Elements, Direction> _directionMap = new Dictionary<Elements, Direction>
        {
            {Elements.AI_TANK_DOWN, Direction.Down },
            {Elements.AI_TANK_LEFT, Direction.Left },
            {Elements.AI_TANK_RIGHT, Direction.Right },
            {Elements.AI_TANK_UP, Direction.Up },
            {Elements.OTHER_TANK_DOWN, Direction.Down },
            {Elements.OTHER_TANK_LEFT, Direction.Left },
            {Elements.OTHER_TANK_RIGHT, Direction.Right },
            {Elements.OTHER_TANK_UP, Direction.Up },
            {Elements.TANK_DOWN, Direction.Down },
            {Elements.TANK_LEFT, Direction.Left },
            {Elements.TANK_RIGHT, Direction.Right },
            {Elements.TANK_UP, Direction.Up },
        };

        public BattleNetClient(string url) : base(url)
        {
        }

        public bool IsPlayerAlive() => IsAnyOfAt(PlayerX, PlayerY, _player);
        public bool IsOutOf(Point point) => IsOutOf(point.X, point.Y);
        public bool IsObstacleAt(Point position) => IsAnyOfAt(position, _obstacles);
        public bool IsObstacleAt(int x, int y) => IsAnyOfAt(x, y, _obstacles);
        public bool IsConstructionAt(Point position) => IsAnyOfAt(position, _constructions);
        public bool IsConstructionAt(int x, int y) => IsAnyOfAt(x, y, _constructions);
        public bool IsAnyOfEnemyAt(List<Point> position) => position.Any(x => IsAnyOfAt(x, _enemies));
        public bool IsEnemyAt(Point position) => IsAnyOfAt(position, _enemies);
        public bool IsEnemyAt(int x, int y) => IsAnyOfAt(x, y, _enemies);
        public bool IsAnyOfAt(Point point, Elements[] elements) => IsAnyOfAt(point.X, point.Y, elements);
        public int GetClosestLeftObstacleX() => GetClosestObstacleCoordinate(Direction.Left, point => point.X, -1);
        public int GetRightObstacleX() => GetClosestObstacleCoordinate(Direction.Right, point => point.X, MapSize);
        public int GetClosestUpperObstacleY() => GetClosestObstacleCoordinate(Direction.Up, point => point.Y, -1);
        public int GetClosestDownObstacleY() => GetClosestObstacleCoordinate(Direction.Down, point => point.Y, MapSize);

        public Direction GetPlayerDirection()
        {
            Elements playerElements = Map[PlayerX, PlayerY];

            return _directionMap[playerElements];
        }

        public List<MapPoint> GetEnemiesPoints(Region areaToSearch)
        {
            var enemies = new List<MapPoint>();
            enemies.Clear();

            for (int x = areaToSearch.Left; x < areaToSearch.Right; x++)
            {
                for (int y = areaToSearch.Top; y < areaToSearch.Bottom; y++)
                {
                    if (IsEnemyAt(x, y))
                    {
                        enemies.Add(new MapPoint(x, y, Map[x, y]));
                    }
                }
            }

            return enemies;
        }

        public int GetClosestObstacleCoordinate(Direction direction, Func<Point, int> coordinateGetter, int defaultValue)
        {
            var iterator = MapUtility.IterationMap[direction];
            var testPosition = GetPlayerTank();

            while (!IsOutOf(testPosition))
            {
                testPosition = iterator(testPosition);

                if (IsObstacleAt(testPosition))
                {
                    return coordinateGetter(testPosition);
                }

            }

            return defaultValue;
        }
    }
}
