using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day11 : IAdventDay
{
    public string Name => "11. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day11.txt");

    private class Monkey
    {
        public Monkey(string[] description)
        {
            var items = Regex.Match(description[1], @"Starting items: ((\d+)(, )?)*");

            foreach (Capture capture in items.Groups[2].Captures)
            {
                Items.Enqueue(int.Parse(capture.Value));
            }

            var operation = Regex.Match(description[2], @"Operation: new = old (.) (.+)");

            Operation = operation.Groups[1].Value switch
            {
                "+" when operation.Groups[2].Value == "old" => i => i + i,
                "*" when operation.Groups[2].Value == "old" => i => i * i,
                "+" => i => i + int.Parse(operation.Groups[2].Value),
                "*" => i => i * int.Parse(operation.Groups[2].Value),
                _ => throw new InvalidOperationException(),
            };

            var test = Regex.Match(description[3], @"Test: divisible by (\d+)");

            Modulus = int.Parse(test.Groups[1].Value);

            Target = description[^2..].Select(Parse).ToDictionary(i => i.Condition, i => i.Target);

            (bool Condition, long Target) Parse(string input)
            {
                var match = Regex.Match(input, @"If (.*): throw to monkey (\d+)");

                return (bool.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value));
            }
        }

        public (bool HasItem, long Item, long Target) Process(long modulus, bool divideByThree)
        {
            if (!Items.TryDequeue(out long item))
                return (false, -1, -1);

            long worry = Operation(item);

            if (divideByThree)
            {
                worry /= 3;
            }
            else
            {
                worry %= modulus;
            }

            return (true, worry, Target[Test(worry)]);
        }

        private bool Test(long item) => item % Modulus == 0;

        public Queue<long> Items { get; } = new();

        public long Modulus { get; }

        private Func<long, long> Operation { get; }

        private Dictionary<bool, long> Target { get; }
    }

    private static long ProcessMonkeys(bool divideByThree, int rounds)
    {
        var monkeys = GetInput()
            .ParseByBlankLines()
            .Select(i => new Monkey(i.ToArray()))
            .ToArray();

        var modulus = monkeys.Aggregate(1L, (i, m) => i * m.Modulus);
        var items = new long[monkeys.Length];

        for (int round = 0; round < rounds; round++)
        {
            for (int m = 0; m < monkeys.Length; m++)
            {
                var monkey = monkeys[m];

                while (true)
                {
                    (bool hasItem, long item, long target) = monkey.Process(modulus, divideByThree);

                    if (!hasItem)
                        break;

                    items[m]++;

                    monkeys[target].Items.Enqueue(item);
                }
            }
        }

        return items.OrderByDescending(i => i).Take(2).Aggregate(1L, (cur, next) => cur * next);
    }

    public string Solve() => ProcessMonkeys(divideByThree: true, rounds: 20).ToString();

    public string SolveAdvanced() => ProcessMonkeys(divideByThree: false, rounds: 10_000).ToString();
}
