using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2016
{
    public class Day10 : IAdventDay
    {
        public string Name => "10. 12. 2016";

        private static IEnumerable<string> GetInput() => File.ReadAllLines("2016/Resources/day10.txt");

        private sealed record Unit(string Type, int Index)
        {
            public List<int> Chips { get; } = new();

            public Unit LowTarget { get; private set; }

            public Unit HighTarget { get; private set; }

            public void SetInstruction(string instruction)
            {
                var match = Regex.Match(instruction, @"bot \d+ gives low to (bot|output) (\d+) and high to (bot|output) (\d+)");

                LowTarget = new Unit(match.Groups[1].Value, int.Parse(match.Groups[2].Value));
                HighTarget = new Unit(match.Groups[3].Value, int.Parse(match.Groups[4].Value));
            }

            public bool Equals(Unit unit) => Type == unit?.Type && Index == unit?.Index;

            public override int GetHashCode() => Index;
        }

        private static int Process(int lowValueChip, int highValueChip)
        {
            var instructions = GetInput();
            var units = new List<Unit>();
            var output0 = GetOrCreateUnit("output", 0);
            var output1 = GetOrCreateUnit("output", 1);
            var output2 = GetOrCreateUnit("output", 2);

            foreach (string instruction in instructions)
            {
                if (Regex.Match(instruction, @"value (\d+) goes to bot (\d+)") is { Success: true } matchValue)
                {
                    int value = int.Parse(matchValue.Groups[1].Value);
                    int index = int.Parse(matchValue.Groups[2].Value);

                    GetOrCreateUnit("bot", index).Chips.Add(value);
                }
                else if (Regex.Match(instruction, @"bot (\d+) gives") is { Success: true } matchRule)
                {
                    int index = int.Parse(matchRule.Groups[1].Value);

                    GetOrCreateUnit("bot", index).SetInstruction(instruction);
                }
            }

            while (true)
            {
                var source = units.FirstOrDefault(u => u.Type == "bot" && u.Chips.Count == 2 && u.LowTarget != null && u.HighTarget != null);

                if (source == null)
                    break;

                (int lowValue, int highValue) = (source.Chips.Min(), source.Chips.Max());

                if (lowValueChip == lowValue && highValueChip == highValue)
                    return source.Index;

                GetOrCreateUnit(source.LowTarget.Type, source.LowTarget.Index).Chips.Add(lowValue);
                GetOrCreateUnit(source.HighTarget.Type, source.HighTarget.Index).Chips.Add(highValue);

                source.Chips.Clear();
            }

            return output0.Chips.Single() * output1.Chips.Single() * output2.Chips.Single();

            Unit GetOrCreateUnit(string type, int index)
            {
                var key = new Unit(type, index);
                var unit = units.FirstOrDefault(u => key.Equals(u));

                if (unit == null)
                {
                    unit = key;
                    units.Add(unit);
                }

                return unit;
            }
        }

        public string Solve() => Process(17, 61).ToString();

        public string SolveAdvanced() => Process(0, 0).ToString();
    }
}
