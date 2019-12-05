using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day22 : IAdventDay
    {
        public string Name => "22. 12. 2017";

        private Dictionary<(int X, int Y), char> _state;

        public string Solve() => GetInfectionCount(10_000, isEvolved: false).ToString();

        public string SolveAdvanced() => GetInfectionCount(10_000_000, isEvolved: true).ToString();

        private int GetInfectionCount(int iterations, bool isEvolved)
        {
            _state = GetInput();

            var curPos = (X: 0, Y: 0);
            var direction = (X: 0, Y: +1);
            int infections = 0;

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                switch (GetState(curPos))
                {
                    case '.':
                        direction = TurnLeft();
                        if (isEvolved)
                        {
                            _state[curPos] = 'W';
                        }
                        else
                        {
                            _state[curPos] = '#';
                            infections++;
                        }
                        break;

                    case 'W':
                        _state[curPos] = '#';
                        infections++;
                        break;

                    case '#':
                        direction = TurnRight();
                        if (isEvolved)
                        {
                            _state[curPos] = 'F';
                        }
                        else
                        {
                            _state.Remove(curPos);
                        }
                        break;

                    case 'F':
                        direction = TurnAround();
                        _state.Remove(curPos);
                        break;
                }

                curPos = (curPos.X + direction.X, curPos.Y + direction.Y);
            }

            return infections;

            char GetState((int, int) position)
            {
                if (!_state.TryGetValue(position, out var state))
                    return '.';

                return state;
            }

            (int X, int Y) TurnLeft() => direction switch
            {
                (0, +1) => (-1, 0),
                (-1, 0) => (0, -1),
                (0, -1) => (+1, 0),
                (+1, 0) => (0, +1),
                _ => throw new InvalidOperationException(),
            };

            (int X, int Y) TurnRight() => direction switch
            {
                (0, +1) => (+1, 0),
                (+1, 0) => (0, -1),
                (0, -1) => (-1, 0),
                (-1, 0) => (0, +1),
                _ => throw new InvalidOperationException(),
            };

            (int X, int Y) TurnAround() => direction switch
            {
                (0, +1) => (0, -1),
                (+1, 0) => (-1, 0),
                (0, -1) => (0, +1),
                (-1, 0) => (+1, 0),
                _ => throw new InvalidOperationException(),
            };
        }

        private static Dictionary<(int, int), char> GetInput()
        {
            var input = System.IO.File.ReadAllLines("2017/Resources/day22.txt").ToList();
            int width = input.Select(s => s.Length).Distinct().Single();
            int height = input.Count;

            int xOffset = -(width - 1) / 2;
            int yOffset = (height - 1) / 2;

            var infected = new Dictionary<(int, int), char>();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (input[i][j] != '#')
                        continue;

                    infected.Add((xOffset + j, yOffset - i), '#');
                }
            }

            return infected;
        }
    }
}
