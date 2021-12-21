using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021;

public class Day21 : IAdventDay
{
    public string Name => "21. 12. 2021";

    private static string[] GetInput() => File.ReadAllLines("2021/Resources/day21.txt");

    public string Solve()
    {
        int rolls = 0;
        int player = 0;

        var position = GetInput().Select(s => int.Parse(System.Text.RegularExpressions.Regex.Match(s, @"Player \d starting position: (\d)").Groups[1].Value)).ToArray();
        var score = new int[2];

        while (true)
        {
            int totalRoll = GetRoll() + GetRoll() + GetRoll();
            position[player] = ((position[player] - 1) + totalRoll) % 10 + 1;
            score[player] += position[player];

            if (score[player] >= 1000)
                return ((long)rolls * score[1 - player]).ToString();

            player = 1 - player;
        }

        int GetRoll()
        {
            int roll = ++rolls;

            return roll > 100 ? (roll - 1) % 100 + 1 : roll;
        }
    }

    public string SolveAdvanced()
    {
        const int fields = 10;
        const int winningScore = 21;

        var rolls = GetRolls();
        var startPosition = GetInput().Select(s => int.Parse(System.Text.RegularExpressions.Regex.Match(s, @"Player \d starting position: (\d)").Groups[1].Value)).ToArray();

        var universes = new List<Dictionary<(int Position1, int Score1, int Position2, int Score2), long>>
        {
            new()
            {
                [(startPosition[0] - 1, 0, startPosition[1] - 1, 0)] = 1,
            },
        };

        for (int round = 1, player = 0;; round++, player = 1 - player)
        {
            var universe = new Dictionary<(int, int, int, int), long>();
            var nonWinningStates = universes[round - 1].Where(i => i.Key is { Score1: < winningScore, Score2: < winningScore }).ToList();

            if (!nonWinningStates.Any())
                return GetWinningStateCount().ToString();

            foreach (var roll in rolls)
            {
                foreach (var state in nonWinningStates)
                {
                    (int position, int score) = player == 0
                        ? (state.Key.Position1, state.Key.Score1)
                        : (state.Key.Position2, state.Key.Score2);

                    int newPosition = (position + roll.Key) % fields;
                    int newScore = score + newPosition + 1;

                    var newKey = player == 0
                        ? (newPosition, newScore, state.Key.Position2, state.Key.Score2)
                        : (state.Key.Position1, state.Key.Score1, newPosition, newScore);

                    universe.TryGetValue(newKey, out long currentCount);
                    universe[newKey] = currentCount + state.Value * roll.Value;
                }
            }

            universes.Add(universe);
        }

        static Dictionary<int, int> GetRolls()
        {
            var dictionary = new Dictionary<int, int>();

            for (int i = 1; i <= 3; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    for (int k = 1; k <= 3; k++)
                    {
                        int sum = i + j + k;

                        dictionary.TryGetValue(sum, out int count);
                        dictionary[sum] = count + 1;
                    }
                }
            }

            return dictionary;
        }

        long GetWinningStateCount()
        {
            var winning = new long[2];

            foreach (var universe in universes)
            {
                foreach (var state in universe)
                {
                    if (state.Key.Score1 >= winningScore)
                    {
                        winning[0] += state.Value;
                    }

                    if (state.Key.Score2 >= winningScore )
                    {
                        winning[1] += state.Value;
                    }
                }
            }

            return winning.Max();
        }
    }
}
