using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day18 : IAdventDay
    {
        private class CellularAutomaton
        {
            private const int FieldLength = 50;

            private int[,] _state;

            private readonly static (int Row, int Col)[] _cellOffsets = new (int, int)[]
            {
                (-1, -1),
                (-1, 0),
                (-1, +1),
                (0, -1),
                (0, +1),
                (+1, -1),
                (+1, 0),
                (+1, +1)
            };

            public CellularAutomaton()
            {
                Initialize(System.IO.File.ReadAllLines(@"2018/Resources/day18.txt"));
            }

            public int Steps { get; private set; }

            private void Initialize(string[] lines)
            {
                _state = new int[FieldLength, FieldLength];

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    for (int j = 0; j < line.Length; j++)
                    {
                        _state[i, j] = GetIndex(line[j]);
                    }
                }

                int GetIndex(char c)
                {
                    switch (c)
                    {
                        case '.': return 0;
                        case '|': return 1;
                        case '#': return 2;
                    }

                    throw new ArgumentException(nameof(c));
                }
            }

            private (int Open, int Trees, int Yards) GetCounts(int row, int col)
            {
                var counts = new int[3];
                foreach (var offset in _cellOffsets)
                {
                    int r = row + offset.Row;
                    int c = col + offset.Col;

                    if (r < 0 || r >= FieldLength || c < 0 || c >= FieldLength)
                        continue;

                    counts[_state[r, c]]++;
                }

                return (counts[0], counts[1], counts[2]);
            }

            private int GetNextState(int row, int col)
            {
                var counts = GetCounts(row, col);

                switch (_state[row, col])
                {
                    case 0: return counts.Trees >= 3 ? 1 : 0;
                    case 1: return counts.Yards >= 3 ? 2 : 1;
                    case 2: return counts.Yards >= 1 && counts.Trees >= 1 ? 2 : 0;
                }

                throw new InvalidOperationException();
            }

            private void Step()
            {
                var nextState = new int[FieldLength, FieldLength];

                for (int row = 0; row < FieldLength; row++)
                {
                    for (int col = 0; col < FieldLength; col++)
                    {
                        nextState[row, col] = GetNextState(row, col);
                    }
                }

                _state = nextState;
                Steps++;
            }

            private void Store()
            {
                System.IO.Directory.CreateDirectory(@"2018/Day 18");
                System.IO.File.WriteAllText($@"2018/Day 18/{Steps}.txt", GetSnapshot());
            }

            private string GetSnapshot()
            {
                var builder = new System.Text.StringBuilder(FieldLength * FieldLength);

                for (int row = 0; row < FieldLength; row++)
                {
                    for (int col = 0; col < FieldLength; col++)
                    {
                        builder.Append(GetChar(_state[row, col]));
                    }

                    builder.AppendLine();
                }

                return builder.ToString();

                char GetChar(int i)
                {
                    switch (i)
                    {
                        case 0: return '.';
                        case 1: return '|';
                        case 2: return '#';
                    }

                    throw new ArgumentException(nameof(i));
                }
            }

            public (int Open, int Trees, int Yards) GetTotals()
            {
                var lookup = _state.Cast<int>().ToLookup(i => i);

                return (lookup[0].Count(), lookup[1].Count(), lookup[2].Count());
            }

            public void RunSimulation(int steps)
            {
                var snapshots = new Dictionary<string, int>()
                {
                    [GetSnapshot()] = 0,
                };

                while (Steps < steps)
                {
                    Step();

                    var snapshot = GetSnapshot();
                    if (snapshots.TryGetValue(snapshot, out int prevSteps))
                    {
                        int cycleLength = Steps - prevSteps;
                        Steps += ((steps - Steps) / cycleLength) * cycleLength;
                    }
                    else
                    {
                        snapshots[snapshot] = Steps;
                    }

                    // Store();
                }
            }
        }

        public string Name => "18. 12. 2018";

        public string Solve()
        {
            var ca = new CellularAutomaton();
            ca.RunSimulation(10);
            var totals = ca.GetTotals();

            return (totals.Trees * totals.Yards).ToString();
        }

        public string SolveAdvanced()
        {
            var ca = new CellularAutomaton();
            ca.RunSimulation(1_000_000_000);
            var totals = ca.GetTotals();

            return (totals.Trees * totals.Yards).ToString();
        }
    }
}