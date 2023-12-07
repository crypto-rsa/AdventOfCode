using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day7 : IAdventDay
{
    public string Name => "7. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day7.txt");

    private static readonly char[] _cards = ['J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'];

    private class HandComparer : IComparer<string>
    {
        public bool ConsiderJokers { get; init; }

        public int Compare(string x, string y)
        {
            if (x is null && y is null)
                return 0;

            if (x is null || y is null)
                return +1;

            var handComparison = GetHandOrder(x).CompareTo(GetHandOrder(y));

            if (handComparison != 0)
                return handComparison;

            for (int i = 0; i < 5; i++)
            {
                var cardComparison = Array.IndexOf(_cards, x[i]).CompareTo(Array.IndexOf(_cards, y[i]));

                if (cardComparison != 0)
                    return cardComparison;
            }

            return 0;

            int GetHandOrder(IEnumerable<char> chars)
            {
                var charArray = chars.ToArray();

                if (ConsiderJokers)
                {
                    var cards = charArray.ToLookup(c => c);

                    if (cards['J'].Count() is > 0 and < 5)
                    {
                        char mostCommonCard = cards.Where(g => g.Key != 'J').MaxBy(g => g.Count()).Key;

                        charArray = cards.SelectMany(g => g.Select(c => c == 'J' ? mostCommonCard : c)).ToArray();
                    }
                }

                var counts = charArray
                    .GroupBy(c => c)
                    .Select(g => g.Count())
                    .OrderByDescending(i => i)
                    .ToArray();

                return counts switch
                {
                    [5] => 6,
                    [4, 1] => 5,
                    [3, 2] => 4,
                    [3, 1, 1] => 3,
                    [2, 2, 1] => 2,
                    [2, 1, 1, 1] => 1,
                    _ => 0,
                };
            }
        }
    }

    private static string GetWinnings(bool considerJokers)
    {
        return GetInput()
            .SplitToLines()
            .Select(Parse)
            .OrderBy(i => i.Hand, new HandComparer { ConsiderJokers = considerJokers })
            .Aggregate((Rank: 0, Sum: 0L), (cur, next) => (cur.Rank + 1, cur.Sum + next.Bid * (cur.Rank + 1)))
            .Sum
            .ToString();

        (string Hand, long Bid) Parse(string input)
        {
            var parts = input.Split(' ');

            return (parts[0], long.Parse(parts[1]));
        }
    }

    public string Solve() => GetWinnings(considerJokers: false);

    public string SolveAdvanced() => GetWinnings(considerJokers: true);
}
