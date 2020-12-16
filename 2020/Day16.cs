using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day16 : IAdventDay
    {
        private record Interval(int Minimum, int Maximum)
        {
            public static Interval Create(string input)
            {
                var parts = input.Split('-');

                return new Interval(int.Parse(parts[0]), int.Parse(parts[1]));
            }
        }

        private record Rule(string Name, List<Interval> Intervals)
        {
            public static Rule Create(string input)
            {
                var parts = input.Split(": ");
                var intervalParts = parts[1].Split(" or ");

                return new Rule(parts[0], intervalParts.Select(Interval.Create).ToList());
            }

            public bool Contains(int number) => Intervals.Any(i => number >= i.Minimum && number <= i.Maximum);

            public bool MatchesAll(int[] values) => values.All(Contains);
        }

        private class Tickets
        {
            public Tickets(string input)
            {
                var parts = input
                    .Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList())
                    .ToList();

                Rules = parts[0].ConvertAll(Rule.Create);
                MyTicket = parts[1][1].Split(',').Select(int.Parse).ToArray();
                NearbyTickets = parts[2].Skip(1).Select(s => s.Split(',').Select(int.Parse).ToArray()).ToList();
                ValidTickets = NearbyTickets.Where(t => t.All(i => Rules.Any(r => r.Contains(i)))).Append( MyTicket ).ToList();
            }

            public List<Rule> Rules { get; }

            public int[] MyTicket { get; }

            public List<int[]> NearbyTickets { get; }

            public List<int[]> ValidTickets { get; }
        }

        private record Assignment(int Size, Dictionary<int, int> Values)
        {
            public Assignment(int size)
                : this(size, new Dictionary<int, int>())
            {
            }

            public static Assignment Extend( Assignment initial, int position, int rule)
            {
                var dictionary = new Dictionary<int, int>(initial.Values)
                {
                    [position] = rule,
                };

                return new Assignment(initial.Size, dictionary);
            }

            public bool IsComplete => Values.Count == Size;
        }

        public string Name => "16. 12. 2020";

        private static string GetInput() => System.IO.File.ReadAllText("2020/Resources/day16.txt");

        public string Solve()
        {
            var tickets = new Tickets(GetInput());

            return tickets.NearbyTickets.Sum(t => t.Where(i => !tickets.Rules.Any(r => r.Contains(i))).Sum()).ToString();
        }

        public string SolveAdvanced()
        {
            var tickets = new Tickets(GetInput());
            var solution = DoAssignment(new Assignment(tickets.Rules.Count), tickets);

            return solution.IsComplete
                ? solution.Values.Aggregate(1L, GetAssignedRuleValue).ToString()
                : string.Empty;

            long GetAssignedRuleValue(long accumulator, KeyValuePair<int, int> pair)
                => accumulator * (tickets.Rules[pair.Value].Name.StartsWith("departure") ? tickets.MyTicket[pair.Key] : 1);
        }

        private static Assignment DoAssignment(Assignment initial, Tickets tickets)
        {
            var unassigned = Enumerable.Range(0, initial.Size)
                .Where(i => !initial.Values.ContainsKey(i))
                .Select(i => (Index: i, Rules: GetMatchingRules(i)))
                .OrderBy(i => i.Rules.Length)
                .ToList();

            if (!unassigned.Any())
                return initial;

            var item = unassigned.First();

            foreach (int rule in item.Rules)
            {
                var assignment = DoAssignment( Assignment.Extend(initial, item.Index, rule), tickets );

                if (assignment.IsComplete)
                    return assignment;
            }

            return initial;

            int[] GetMatchingRules(int position)
                => Enumerable.Range(0, tickets.Rules.Count)
                .Where(i => !initial.Values.ContainsValue(i) && tickets.Rules[i].MatchesAll(tickets.ValidTickets.Select(t => t[position]).ToArray()))
                .ToArray();
        }
    }
}
