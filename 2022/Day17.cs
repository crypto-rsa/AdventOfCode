using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day17 : IAdventDay
{
    public string Name => "17. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day17.txt");

    private const int BoardWidth = 7;

    private class HistoryKey
    {
        private readonly byte[] _lines;

        private readonly int _shapeKind;

        private readonly int _jetIndex;

        public HistoryKey(int shapeKind, int jetIndex, byte[] lines)
        {
            _shapeKind = shapeKind;
            _jetIndex = jetIndex;
            _lines = lines;
        }

        public override int GetHashCode() => HashCode.Combine(_shapeKind, _jetIndex, _lines.Length);

        public override bool Equals(object obj) => obj is HistoryKey other
                                                   && _shapeKind == other._shapeKind
                                                   && _jetIndex == other._jetIndex
                                                   && _lines.SequenceEqual(other._lines);
    }

    private class Shape
    {
        private readonly byte[][] _masks;

        private Shape(string input)
        {
            var shape = input.SplitToLines().ToArray();

            Height = shape.Length;
            Width = shape.Max(s => s.Length);

            _masks = Enumerable.Range(1, BoardWidth - 1)
                .Aggregate(new List<byte[]> { CreateShapeMask() }, OffsetMask)
                .ToArray();

            byte CreateLineMask(string line) => Enumerable
                .Range(1, BoardWidth)
                .Aggregate(0, (cur, i) => cur + (i <= line.Length && line[i - 1] == '#' ? 1 << (BoardWidth - i) : 0), res => (byte)res);

            byte[] CreateShapeMask() => shape.Select(CreateLineMask).ToArray();

            List<byte[]> OffsetMask(List<byte[]> prevList, int i)
            {
                var previous = prevList.Last();

                var next = i <= BoardWidth - Width
                    ? previous.Select(b => (byte)(b >> 1)).ToArray()
                    : previous.ToArray();

                prevList.Add(next);

                return prevList;
            }
        }

        public static IEnumerable<Shape> GetShapes()
        {
            yield return new Shape("####");
            yield return new Shape(".#.\r\n###\r\n.#.");
            yield return new Shape("..#\r\n..#\r\n###");
            yield return new Shape("#\r\n#\r\n#\r\n#");
            yield return new Shape("##\r\n##");
        }

        public byte[] GetMask(int offset) => _masks[offset];

        public bool CollidesWith(List<byte> lines, int x, int y)
        {
            if (x < 0 || x + Width - 1 >= BoardWidth)
                return true;

            return _masks[x].Select(GetCollision).Any(b => b);

            bool GetCollision(byte value, int i)
            {
                int lineIndex = y - i;

                if (lineIndex >= lines.Count)
                    return false;

                if (lineIndex < 0)
                    return true;

                return (lines[lineIndex] & value) != 0;
            }
        }

        public int Height { get; }

        private int Width { get; }
    }

    private static long Simulate(long shapeCount)
    {
        var jets = GetInput().Trim();
        var shapes = Shape.GetShapes().ToList();
        var lines = new List<byte> { (1 << BoardWidth) - 1 };
        var peaks = new int[BoardWidth];
        int top = 0;
        int jetIndex = -1;

        var history = new Dictionary<HistoryKey, (int ShapeIndex, int Top)>();
        (long Index, long Top)? offset = null;

        for (int i = 0; i < shapeCount; i++)
        {
            if (offset is { Index: var indexOffset } && indexOffset + i >= shapeCount)
                break;

            var shape = shapes[i % shapes.Count];
            int x = 2;
            int y = top + shape.Height + 3;

            while (true)
            {
                int xMove = jets[++jetIndex % jets.Length] == '<' ? -1 : +1;

                if (!shape.CollidesWith(lines, x + xMove, y))
                {
                    x += xMove;
                }

                if (shape.CollidesWith(lines, x, y - 1))
                    break;

                y--;
            }

            while (lines.Count <= y)
            {
                lines.Add(0);
            }

            for (int r = 0; r < shape.Height; r++)
            {
                lines[y - r] |= shape.GetMask(x)[r];
            }

            top = Math.Max(y, top);

            for (int k = 0; k < BoardWidth; k++)
            {
                for (int j = y; j > peaks[k]; j--)
                {
                    if ((lines[j] & (1 << k)) == 0)
                        continue;

                    peaks[k] = j;

                    break;
                }
            }

            int bottom = peaks.Min();

            if (offset is not null)
                continue;

            var key = new HistoryKey(i % shapes.Count, jetIndex % jets.Length, lines.GetRange(bottom, top - bottom + 1).ToArray());

            if (history.TryGetValue(key, out var item))
            {
                long length = i - item.ShapeIndex;
                long repetitions = (shapeCount - i) / length;

                offset = (repetitions * length, repetitions * (top - item.Top));
            }

            history[key] = (i, top);
        }

        return top + (offset is { Top: var topOffset } ? topOffset : 0);
    }

    public string Solve() => Simulate(2022).ToString();

    public string SolveAdvanced() => Simulate(1_000_000_000_000).ToString();
}
