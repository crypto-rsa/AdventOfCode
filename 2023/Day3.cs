using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day3 : IAdventDay
{
    public string Name => "3. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day3.txt");

    private class MapRow
    {
        private readonly int _index;
        private readonly string _source;

        private readonly HashSet<Range> _numberRanges = new();

        private readonly HashSet<int> _symbolIndices = new();

        private readonly HashSet<int> _gearIndices = new();

        public MapRow(int index, string input)
        {
            _index = index;
            _source = input;

            for (int pos = 0; pos < input.Length;)
            {
                char c = input[pos];

                if (c == '.')
                {
                    pos++;

                    continue;
                }

                if (c == '*')
                {
                    _gearIndices.Add(pos);
                }

                if (!char.IsDigit(c))
                {
                    _symbolIndices.Add(pos);
                    pos++;

                    continue;
                }

                Index start = pos;

                while (pos < input.Length && char.IsDigit(input[pos]))
                {
                    pos++;
                }

                _numberRanges.Add(new Range(start, pos));
            }
        }

        private bool HasAdjacentIndex(int numberIndex)
            => _symbolIndices.Contains(numberIndex - 1) || _symbolIndices.Contains(numberIndex) || _symbolIndices.Contains(numberIndex + 1);

        private bool HasAdjacentIndex(Range range) => HasAdjacentIndex(range.Start.Value) || HasAdjacentIndex(range.End.Value - 1);

        private IEnumerable<string> GetAdjacentNumbers(int index) => _numberRanges
            .Where(r => Math.Abs(r.Start.Value - index) <= 1 || Math.Abs(r.End.Value - 1 - index) <= 1)
            .Select(r => _source[r]);

        public int GetSumOfValidNumbers(MapRow[] rows)
        {
            var adjacentRows = GetAdjacentRows(rows).ToArray();

            return _numberRanges
                .Where(range => adjacentRows.Any(r => r.HasAdjacentIndex(range)))
                .Select(r => int.Parse(_source[r]))
                .Sum();
        }

        IEnumerable<MapRow> GetAdjacentRows(MapRow[] rows)
        {
            if (_index > 0)
                yield return rows[_index - 1];

            yield return rows[_index];

            if (_index < rows.Length - 1)
                yield return rows[_index + 1];
        }

        public int GetSumOfValidGearRatios(MapRow[] rows)
        {
            var adjacentRows = GetAdjacentRows(rows).ToArray();

            return _gearIndices
                .Select(i => adjacentRows.SelectMany(r => r.GetAdjacentNumbers(i)).ToArray())
                .Where(a => a.Length == 2)
                .Sum(a => int.Parse(a[0]) * int.Parse(a[1]));
        }
    }

    public string Solve()
    {
        var rows = GetInput()
            .SplitToLines()
            .Select((s, i) => new MapRow(i, s))
            .ToArray();

        return rows.Sum(r => r.GetSumOfValidNumbers(rows)).ToString();
    }

    public string SolveAdvanced()
    {
        var rows = GetInput()
            .SplitToLines()
            .Select((s, i) => new MapRow(i, s))
            .ToArray();

        return rows.Sum(r => r.GetSumOfValidGearRatios(rows)).ToString();
    }
}
