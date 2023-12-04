using System;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day4 : IAdventDay
{
    public string Name => "4. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day4.txt");

    public string Solve()
    {
        return GetInput()
            .SplitToLines()
            .Sum(GetPoints)
            .ToString();

        int GetPoints(string card)
        {
            var match = System.Text.RegularExpressions.Regex.Match(card, @"Card +\d+: (.*) \| (.*)");
            var winning = match.Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToHashSet();
            var own = match.Groups[2].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToHashSet();

            var matching = winning.Intersect(own).Count();

            if (matching == 0)
                return 0;

            return 1 << (matching - 1);
        }
    }

    public string SolveAdvanced()
    {
        var cards = GetInput().SplitToLines().ToArray();
        var cardCounts = cards.Select(_ => 1).ToArray();

        for (int index = 0; index < cards.Length; index++)
        {
            string card = cards[index];
            var match = System.Text.RegularExpressions.Regex.Match(card, @"Card +\d+: (.*) \| (.*)");
            var winning = match.Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToHashSet();
            var own = match.Groups[2].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToHashSet();

            var matching = winning.Intersect(own).Count();

            for (int win = 0; win < matching; win++)
            {
                cardCounts[index + win + 1] += cardCounts[index];
            }
        }

        return cardCounts.Sum().ToString();
    }
}
