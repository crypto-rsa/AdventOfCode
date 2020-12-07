using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2020
{
    public class Day7 : IAdventDay
    {
        private class Bag
        {
            public Bag(string input)
            {
                var match = Regex.Match(input, @"(.*?) bags contain (.*)\.");

                Color = match.Groups[1].Value;

                if (match.Groups[2].Value != "no other bags")
                {
                    foreach( var bag in match.Groups[2].Value.Split(", ", System.StringSplitOptions.RemoveEmptyEntries) )
                    {
                        var bagMatch = Regex.Match(bag, @"(\d+) (.*) bags?");

                        Contents[bagMatch.Groups[2].Value] = int.Parse(bagMatch.Groups[1].Value);
                    }
                }
            }

            public string Color { get; }

            public Dictionary<string, int> Contents { get; } = new Dictionary<string, int>();
        }

        private const string ShinyGold = "shiny gold";

        public string Name => "7. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day7.txt");

        public string Solve()
        {
            var bags = GetInput().Select(s => new Bag(s)).ToHashSet();

            var innerBags = bags.Where(b => b.Color == ShinyGold).ToHashSet();
            var allOuterBags = new HashSet<Bag>();

            while(true)
            {
                var outerBags = bags.Where(b => innerBags.Any(b2 => b.Contents.ContainsKey(b2.Color))).ToHashSet();

                if (!outerBags.Any())
                    break;

                foreach (var bag in outerBags)
                {
                    allOuterBags.Add(bag);
                }

                bags.ExceptWith(innerBags);
                innerBags = outerBags;
            }

            return allOuterBags.Count.ToString();
        }

        public string SolveAdvanced()
        {
            var bags = GetInput().Select(s => new Bag(s)).ToHashSet();
            var bagCounts = new Dictionary<string, long>();

            while (true)
            {
                var outerBags = bags.Where(b => b.Contents.Keys.All(c => bagCounts.ContainsKey(c))).ToHashSet();

                foreach (var bag in outerBags)
                {
                    bagCounts[bag.Color] = bag.Contents.Sum(p => p.Value * (bagCounts[p.Key] + 1));

                    if (bag.Color == ShinyGold)
                        return bagCounts[bag.Color].ToString();
                }

                bags.ExceptWith(outerBags);
            }

            throw new System.InvalidOperationException();
        }
    }
}
