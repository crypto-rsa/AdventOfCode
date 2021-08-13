using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2016
{
    public class Day2 : IAdventDay
    {
        public string Name => "2. 12. 2016";

        private static IEnumerable<string> GetInput() => File.ReadAllLines("2016/Resources/day2.txt");

        public string Solve()
        {
            var pad = new[,]
            {
                { '1', '2', '3' },
                { '4', '5', '6' },
                { '7', '8', '9' },
            };

            return new string(GetCode(pad).ToArray());
        }

        private static IEnumerable<char> GetCode(char[,] pad)
        {
            var current = FindStart();

            foreach (string line in GetInput())
            {
                current = line.Aggregate(current, GetNextPosition);

                yield return pad[current.Row, current.Col];
            }

            (int Row, int Col) FindStart()
            {
                for (int i = 0; i < pad.GetLength(0); i++)
                {
                    for (int j = 0; j < pad.GetLength(1); j++)
                    {
                        if (pad[i, j] == '5')
                            return (i, j);
                    }
                }

                throw new ArgumentException(nameof( pad ));
            }

            (int Row, int Col) GetNextPosition((int Row, int Col) cur, char next)
            {
                var (rowOffset, colOffset) = next switch
                {
                    'L' => (0, -1),
                    'U' => (-1, 0),
                    'R' => (0, +1),
                    'D' => (+1, 0),
                    _ => throw new ArgumentException(nameof( next )),
                };

                var candidate = (Row: GetPositionInArray(cur.Row, 0, rowOffset), Col: GetPositionInArray(cur.Col, 1, colOffset));

                return pad[candidate.Row, candidate.Col] == ' ' ? cur : candidate;
            }

            int GetPositionInArray(int cur, int dimension, int offset) => Math.Max(0, Math.Min(cur + offset, pad.GetLength(dimension) - 1));
        }

    public string SolveAdvanced()
        {
            var pad = new[,]
            {
                { ' ', ' ', '1', ' ', ' ' },
                { ' ', '2', '3', '4', ' ' },
                { '5', '6', '7', '8', '9' },
                { ' ', 'A', 'B', 'C', ' ' },
                { ' ', ' ', 'D', ' ', ' ' },
            };

            return new string(GetCode(pad).ToArray());
        }
    }
}