using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day17 : IAdventDay
    {
        private struct Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }

            public int Y { get; }
        }
        private class Ground
        {
            private char[,] _materials;

            private int _maxX;

            private int _minY = int.MaxValue;

            private int _maxY;

            private readonly HashSet<Point> _fallPoints = new HashSet<Point>();

            private readonly Dictionary<Point, Point> _parents = new Dictionary<Point, Point>();

            public Ground()
            {
                Initialize(System.IO.File.ReadAllLines(@"2018/Resources/day17.txt"));
            }

            public int Drops { get; private set; }

            private void Initialize(string[] lines)
            {
                var clayLocations = new List<(HashSet<int> X, HashSet<int> Y)>();

                foreach (var line in lines)
                {
                    var items = line.Split(", ");
                    HashSet<int> x = null;
                    HashSet<int> y = null;

                    foreach (var item in items)
                    {
                        var parts = item.Split('=');
                        if (parts[0] == "x")
                        {
                            x = GetRange(parts[1]);
                        }
                        else
                        {
                            y = GetRange(parts[1]);
                        }
                    }

                    clayLocations.Add((x, y));

                    _maxX = Math.Max(_maxX, x.Max());
                    _minY = Math.Min(_minY, y.Min());
                    _maxY = Math.Max(_maxY, y.Max());
                }

                _maxX++;
                _materials = new char[_maxX + 1, _maxY + 1];

                for (int x = 0; x <= _maxX; x++)
                {
                    for (int y = 0; y <= _maxY; y++)
                    {
                        _materials[x, y] = '.';
                    }
                }

                _materials[500, 0] = '+';

                foreach (var location in clayLocations)
                {
                    foreach (int x in location.X)
                    {
                        foreach (int y in location.Y)
                        {
                            _materials[x, y] = '#';
                        }
                    }
                }

                HashSet<int> GetRange(string part)
                {
                    var limits = part.Split("..");
                    int first = int.Parse(limits[0]);
                    int last = limits.Length == 1 ? first : int.Parse(limits[1]);

                    return new HashSet<int>(Enumerable.Range(first, last - first + 1));
                }

                _fallPoints.Add(new Point(500, 0));
            }

            public void Store()
            {
                var builder = new System.Text.StringBuilder(_maxX * _maxY);

                for (int y = 0; y <= _maxY; y++)
                {
                    for (int x = 0; x <= _maxX; x++)
                    {
                        var point = new Point(x, y);
                        builder.Append(_fallPoints.Contains(point) ? '*' : _materials[x, y]);
                    }

                    builder.AppendLine();
                }

                string path = $@"2018/Day17/{Drops}.txt";
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                System.IO.File.WriteAllText(path, builder.ToString());
            }

            private (bool Found, int Y) FindObstacleBelow(int startX, int startY)
            {
                var obstacle = Enumerable.Range(startY + 1, _maxY - startY).Select(i => (Y: i, Material: _materials[startX, i])).FirstOrDefault(i => IsObstacle(i.Material));
                if (obstacle.Material == '\0')
                    return (false, _maxY + 1);

                return (true, obstacle.Y);
            }

            private (bool Found, int X, char Type) FindObstacleHorizontal(int startX, int startY, int velocity)
            {
                var hole = (X: velocity < 0 ? startX + 1 : startX - 1, Material: '\0');
                var range = velocity < 0 ? Enumerable.Range(0, startX).Reverse().ToList() : Enumerable.Range(startX + 1, _maxX - startX).ToList();

                if (startY < _maxY)
                {
                    hole = range.Select(i => (X: i, Material: _materials[i, startY + 1])).FirstOrDefault(i => !IsObstacle(i.Material));
                }

                var obstacle = range.Select(i => (X: i, Material: _materials[i, startY])).FirstOrDefault(i => IsObstacle(i.Material));

                if (hole.Material == '\0' && obstacle.Material == '\0')
                    return (false, -1, '|');

                if (obstacle.Material == '\0')
                    return (true, hole.X, '|');

                if (hole.Material == '\0')
                    return (true, obstacle.X, '~');

                bool holeCloser = (velocity < 0 && hole.X > obstacle.X) || (velocity > 0 && hole.X < obstacle.X);

                return holeCloser ? (true, hole.X, '|') : (true, obstacle.X, '~');
            }

            private bool TraceDrop(Point currentPoint)
            {
                int startX = currentPoint.X;
                int startY = currentPoint.Y;

                var obstacleBelow = FindObstacleBelow(startX, startY);
                for (int y = startY + 1; y < obstacleBelow.Y; y++)
                {
                    _materials[startX, y] = '|';
                }

                if (obstacleBelow.Found)
                {
                    int y = obstacleBelow.Y - 1;

                    if (y == startY)
                    {
                        _fallPoints.Add(_parents[new Point(startX, startY)]);
                        return true;
                    }

                    var obstacleLeft = FindObstacleHorizontal(startX, y, -1);
                    var obstacleRight = FindObstacleHorizontal(startX, y, +1);

                    char fillType = obstacleLeft.Type == '~' && obstacleRight.Type == '~' ? '~' : '|';
                    for (int x = obstacleLeft.X + 1; x < obstacleRight.X; x++)
                    {
                        _materials[x, y] = fillType;
                    }

                    if (fillType == '~')
                    {
                        _fallPoints.Add(currentPoint);
                        return false;
                    }
                    else
                    {
                        if (obstacleLeft.Found && obstacleLeft.Type == '|')
                        {
                            _materials[obstacleLeft.X, y] = obstacleLeft.Type;
                            var pointLeft = new Point(obstacleLeft.X, y);
                            _fallPoints.Add(pointLeft);
                            _parents[pointLeft] = currentPoint;
                            // TraceDrop(pointLeft);
                        }

                        if (obstacleRight.Found && obstacleRight.Type == '|')
                        {
                            _materials[obstacleRight.X, y] = obstacleRight.Type;
                            var pointRight = new Point(obstacleRight.X, y);
                            _fallPoints.Add(pointRight);
                            _parents[pointRight] = currentPoint;
                            // TraceDrop(pointRight);
                        }
                    }
                }

                return false;
            }

            private bool IsObstacle(char c) => c == '#' || c == '~';

            public bool DropWater()
            {
                var oldFallPoints = new HashSet<Point>(_fallPoints);
                _fallPoints.Clear();

                foreach (var fallPoint in oldFallPoints)
                {
                    Drops++;
                    TraceDrop(fallPoint);
                }

                return _fallPoints.Any();
            }

            public int GetWetTileCount()
            {
                int total = 0;
                for (int y = _minY; y <= _maxY; y++)
                {
                    for (int x = 0; x <= _maxX; x++)
                    {
                        if (_materials[x, y] == '|' || _materials[x, y] == '~')
                        {
                            total++;
                        }
                    }
                }

                return total;
            }

            public int GetRetainedWaterTiles() => _materials.Cast<char>().Count(c => c == '~');

            public void RunSimulation()
            {
                while (true)
                {
                    if (!DropWater())
                        break;
                }
            }
        }
        public string Name => "17. 12. 2018";

        public string Solve()
        {
            var ground = new Ground();
            ground.RunSimulation();

            return ground.GetWetTileCount().ToString();
        }

        public string SolveAdvanced()
        {
            var ground = new Ground();
            ground.RunSimulation();

            return ground.GetRetainedWaterTiles().ToString();
        }
    }
}