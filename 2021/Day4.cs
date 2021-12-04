using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day4 : IAdventDay
    {
        public string Name => "4. 12. 2021";

        private static string GetInput() => File.ReadAllText("2021/Resources/day4.txt");

        private class Bingo
        {
            private readonly List<int> _numbers;

            private readonly List<int[][]> _boards;

            public Bingo(string input)
            {
                var parts = input.Split($"{Environment.NewLine}{Environment.NewLine}");

                _numbers = parts[0].Split(',').Select(int.Parse).ToList();
                _boards = parts.Skip(1).Select(GetBoard).ToList();

                int[][] GetBoard(string s) => s.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()).ToArray();
            }

            public int GetFirstWinningScore()
            {
                var drawn = new HashSet<int>();

                foreach (int number in _numbers)
                {
                    drawn.Add(number);

                    var score = _boards.Max(b => GetBoardScore(drawn, b, number));

                    if (score >= 0)
                        return score;
                }

                return -1;
            }

            public int GetLastWinningScore()
            {
                var drawn = new HashSet<int>();
                var nonWinning = _boards.ToHashSet();

                foreach (int number in _numbers)
                {
                    drawn.Add(number);

                    var winning = nonWinning.Where(b => GetBoardScore(drawn, b, number) >= 0).ToHashSet();

                    foreach (int[][] board in winning)
                    {
                        nonWinning.Remove(board);
                    }

                    if (nonWinning.Count == 0 && winning.Count == 1)
                        return GetBoardScore(drawn, winning.Single(), number);
                }

                return -1;
            }

            private static int GetBoardScore(HashSet<int> drawn, int[][] board, int lastNumber)
            {
                if (!IsBingo())
                    return -1;

                return board.SelectMany(a => a).Where(n => !drawn.Contains(n)).Sum() * lastNumber;

                bool IsBingo() => board.Any(a => a.All(drawn.Contains)) || Enumerable.Range(0, board[0].Length).Any(col => board.All(a => drawn.Contains(a[col])));
            }
        }

        public string Solve() => new Bingo(GetInput()).GetFirstWinningScore().ToString();

        public string SolveAdvanced() => new Bingo(GetInput()).GetLastWinningScore().ToString();
    }
}
