using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day16 : IAdventDay
    {
        public string Name => "16. 12. 2021";

        private static string GetInput() => File.ReadAllText("2021/Resources/day16.txt");

        private class Packet
        {
            private int Version { get; }

            private int Type { get; }

            private long Value { get; }

            private List<Packet> SubPackets { get; } = new();

            private Packet(string input, ref int position)
            {
                Version = Convert.ToInt32(ReadAndUpdate(3, ref position), 2);
                Type = Convert.ToInt32(ReadAndUpdate(3, ref position), 2);

                if (Type == 4)
                {
                    var continuingGroups = input[position..].Chunk(5).TakeWhile(a => a[0] == '1').ToList();
                    int tailStart = position + continuingGroups.Sum(a => a.Length);
                    var tail = input[tailStart..(tailStart + 5)];

                    var valueString = string.Concat(continuingGroups.Select(a => new string(a[1..])).Append(tail[1..]));

                    Value = Convert.ToInt64(valueString, 2);

                    position += (continuingGroups.Count + 1) * 5;
                }
                else
                {
                    char lengthTypeId = input[position++];

                    if (lengthTypeId == '0')
                    {
                        int totalLength = Convert.ToInt32(ReadAndUpdate(15, ref position), 2);
                        int end = position + totalLength;

                        while (position < end)
                        {
                            SubPackets.Add(new Packet(input, ref position));
                        }
                    }
                    else
                    {
                        int subPacketCount = Convert.ToInt32(ReadAndUpdate(11, ref position), 2);

                        for (int i = 0; i < subPacketCount; i++)
                        {
                            SubPackets.Add(new Packet(input, ref position));
                        }
                    }
                }

                string ReadAndUpdate(int length, ref int pos)
                {
                    string chunk = input[pos..(pos + length)];

                    pos += length;

                    return chunk;
                }
            }

            public static Packet Create()
            {
                var input = string.Concat(GetInput().Trim().Select(c => Convert.ToString(Convert.ToByte(c.ToString(), 16), 2).PadLeft(4, '0')));
                int position = 0;

                return new Packet(input, ref position);
            }

            public IEnumerable<int> GetAllVersionNumbers()
            {
                yield return Version;

                foreach (var subVersion in SubPackets.SelectMany(p => p.GetAllVersionNumbers()))
                {
                    yield return subVersion;
                }
            }

            public long GetTotalValue() => Type switch
            {
                0 => SubPackets.Sum(p => p.GetTotalValue()),
                1 => SubPackets.Aggregate(1L, (cur, next) => cur * next.GetTotalValue()),
                2 => SubPackets.Min(p => p.GetTotalValue()),
                3 => SubPackets.Max(p => p.GetTotalValue()),
                4 => Value,
                5 => SubPackets[0].GetTotalValue() > SubPackets[1].GetTotalValue() ? 1 : 0,
                6 => SubPackets[0].GetTotalValue() < SubPackets[1].GetTotalValue() ? 1 : 0,
                7 => SubPackets[0].GetTotalValue() == SubPackets[1].GetTotalValue() ? 1 : 0,
                _ => throw new InvalidOperationException(),
            };
        }

        public string Solve() => Packet.Create().GetAllVersionNumbers().Sum().ToString();

        public string SolveAdvanced() => Packet.Create().GetTotalValue().ToString();
    }
}
