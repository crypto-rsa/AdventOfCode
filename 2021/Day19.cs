using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021;

public class Day19 : IAdventDay
{
    public string Name => "19. 12. 2021";

    private static string GetInput() => File.ReadAllText("2021/Resources/day19.txt");

    private static class Transformation
    {
        public const int Count = 24;

        private static Dictionary<(int, int), int> CompoundTransformMap { get; } = InitializeCompoundTransformMap();

        private static IEnumerable<Coordinates> GetCubeCorners()
        {
            yield return new Coordinates(+1, +1, +1);
            yield return new Coordinates(+1, +1, -1);
            yield return new Coordinates(+1, -1, +1);
            yield return new Coordinates(+1, -1, -1);
            yield return new Coordinates(-1, +1, +1);
            yield return new Coordinates(-1, +1, -1);
            yield return new Coordinates(-1, -1, +1);
            yield return new Coordinates(-1, -1, -1);
        }

        private static Dictionary<(int, int), int> InitializeCompoundTransformMap()
        {
            var corners = GetCubeCorners().ToList();
            var dictionary = new Dictionary<(int, int), int>();

            for (int inner = 0; inner < Count; inner++)
            {
                for (int outer = 0; outer < Count; outer++)
                {
                    dictionary[(inner, outer)] = Enumerable.Range(0, Count).First(t => corners.All(c => Matches(c, inner, outer, t)));
                }
            }

            return dictionary;

            bool Matches(Coordinates coordinates, int inner, int outer, int compound)
                => coordinates.Transform(inner).Transform(outer) == coordinates.Transform(compound);
        }

        public static int GetCompound(int inner, int outer) => CompoundTransformMap[(inner, outer)];
    }

    private record Coordinates(int X, int Y, int Z)
    {
        public static Coordinates Create(string input)
        {
            var parts = input.Split(',');

            return new Coordinates(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }

        public Coordinates Transform(int transformIndex) => transformIndex switch
        {
            0 => new Coordinates(+X, +Y, +Z),
            1 => new Coordinates(+X, +Z, -Y),
            2 => new Coordinates(+X, -Y, -Z),
            3 => new Coordinates(+X, -Z, +Y),

            4 => new Coordinates(-X, -Y, +Z),
            5 => new Coordinates(-X, +Z, +Y),
            6 => new Coordinates(-X, +Y, -Z),
            7 => new Coordinates(-X, -Z, -Y),

            8 => new Coordinates(+Y, -X, +Z),
            9 => new Coordinates(+Y, -Z, -X),
            10 => new Coordinates(+Y, +X, -Z),
            11 => new Coordinates(+Y, +Z, +X),

            12 => new Coordinates(-Y, +X, +Z),
            13 => new Coordinates(-Y, -Z, +X),
            14 => new Coordinates(-Y, -X, -Z),
            15 => new Coordinates(-Y, +Z, -X),

            16 => new Coordinates(+Z, +X, +Y),
            17 => new Coordinates(+Z, +Y, -X),
            18 => new Coordinates(+Z, -X, -Y),
            19 => new Coordinates(+Z, -Y, +X),

            20 => new Coordinates(-Z, +X, -Y),
            21 => new Coordinates(-Z, +Y, +X),
            22 => new Coordinates(-Z, -X, +Y),
            23 => new Coordinates(-Z, -Y, -X),
            _ => throw new ArgumentOutOfRangeException(nameof( transformIndex )),
        };

        public bool IsDistant() => Math.Abs(X) > 1000 || Math.Abs(Y) > 1000 || Math.Abs(Z) > 1000;

        public int GetMagnitude() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public static Coordinates operator -(Coordinates first, Coordinates second) => new(first.X - second.X, first.Y - second.Y, first.Z - second.Z);

        public static Coordinates operator +(Coordinates first, Coordinates second) => new(first.X + second.X, first.Y + second.Y, first.Z + second.Z);

        public static Coordinates operator -(Coordinates instance) => new(-instance.X, -instance.Y, -instance.Z);
    }

    private class Scanner : List<Coordinates>
    {
        public Scanner(string input)
            : base(input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(Coordinates.Create))
        {
        }
    }

    private class Submarine
    {
        private Dictionary<(int Source, int Target), (Coordinates Offset, int Transform)> _scannerOffsets;

        private List<Scanner> Scanners { get; }

        public int BeaconCount { get; private set; }

        public Submarine()
        {
            Scanners = GetInput().Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries).Select(s => new Scanner(s)).ToList();
            Calculate();
        }

        private void Calculate()
        {
            var connected = new HashSet<int> { 0 };
            var unconnected = Scanners.Select((_, i) => i).Except(connected).ToHashSet();
            var connectedBeacons = Scanners[0].ToHashSet();
            var incompatiblePairs = new HashSet<(int, int)>();

            _scannerOffsets = new Dictionary<(int Source, int Target), (Coordinates Offset, int Transform)>
            {
                [(0, 0)] = (new Coordinates(0, 0, 0), 0),
            };

            while (unconnected.Any())
            {
                bool isConnected = false;

                foreach (int toConnect in unconnected)
                {
                    foreach (int i in connected)
                    {
                        if (incompatiblePairs.Contains((i, toConnect)))
                            continue;

                        var candidate = GetOffsetCandidates(i, toConnect, connected, connectedBeacons).FirstOrDefault();

                        isConnected = candidate.Offset != default;

                        if (!isConnected)
                        {
                            incompatiblePairs.Add((i, toConnect));
                        }
                        else
                        {
                            Connect(i, candidate, toConnect);

                            break;
                        }
                    }

                    if (!isConnected)
                        continue;

                    unconnected.Remove(toConnect);
                    connected.Add(toConnect);

                    break;
                }

                if (!isConnected)
                    return;
            }

            BeaconCount = connectedBeacons.Count;

            void Connect(int sourceIndex, (Coordinates Offset, int Transform) target, int targetIndex)
            {
                var source = _scannerOffsets[(0, sourceIndex)];

                var transform = Transformation.GetCompound(target.Transform, source.Transform);
                var offset = source.Offset + target.Offset.Transform(source.Transform);

                var newOffsets = Scanners[targetIndex].Select(c => offset + c.Transform(transform)).ToList();

                foreach (var c in newOffsets)
                {
                    connectedBeacons.Add(c);
                }

                _scannerOffsets[(0, targetIndex)] = (offset, transform);
            }
        }

        private IEnumerable<(Coordinates Offset, int Transform)> GetOffsetCandidates(int source, int target, IReadOnlyCollection<int> connectedScanners, HashSet<Coordinates> connectedBeacons)
        {
            var sourceScanner = Scanners[source];
            var targetScanner = Scanners[target];

            foreach (var center1 in sourceScanner)
            {
                var offsets1 = sourceScanner.Select((c, i) => (Coord: c - center1, Index: i)).ToDictionary(i => i.Coord, i => i.Index);

                foreach (var center2 in targetScanner)
                {
                    var offsets2 = targetScanner.Select(c => c - center2).ToList();

                    for (int t = 0; t < Transformation.Count; t++)
                    {
                        var pairing = GetPairing(offsets1, offsets2, t);
                        var offset = center1 + (-center2).Transform(t);

                        if (pairing.Count < 12 || !IsValid(offset, t))
                            continue;

                        yield return (offset, t);
                    }
                }
            }

            List<(int Index1, int Index2)> GetPairing(IReadOnlyDictionary<Coordinates, int> points1, IReadOnlyList<Coordinates> points2, int transform)
            {
                var pairings = new List<(int, int)>(points1.Count);

                for (int i = 0; i < points2.Count; i++)
                {
                    if (!points1.TryGetValue(points2[i].Transform(transform), out var index1))
                        continue;

                    pairings.Add((index1, i));
                }

                return pairings;
            }

            bool IsValid(Coordinates targetOffset, int targetTransform)
            {
                if (targetOffset == default)
                    return false;

                (var sourceOffset, int sourceTransform) = _scannerOffsets[(0, source)];

                var transform = Transformation.GetCompound(targetTransform, sourceTransform);
                var offset = sourceOffset + targetOffset.Transform(sourceTransform);

                var newOffsets = Scanners[target].Select(c => offset + c.Transform(transform)).ToList();

                return newOffsets.All(c => connectedBeacons.Contains(c) || connectedScanners.All(i => (c - _scannerOffsets[(0, i)].Offset).IsDistant()));
            }
        }

        public int GetMaximumScannerDistance()
        {
            int maxDistance = 0;

            for (int i = 0; i < Scanners.Count; i++)
            {
                for (int j = i + 1; j < Scanners.Count; j++)
                {
                    maxDistance = Math.Max(maxDistance, (_scannerOffsets[(0, i)].Offset - _scannerOffsets[(0, j)].Offset).GetMagnitude());
                }
            }

            return maxDistance;
        }
    }

    public string Solve() => new Submarine().BeaconCount.ToString();

    public string SolveAdvanced() => new Submarine().GetMaximumScannerDistance().ToString();
}
