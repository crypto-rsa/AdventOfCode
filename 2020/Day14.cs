using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2020
{
    public class Day14 : IAdventDay
    {
        public string Name => "14. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day14.txt");

        public string Solve()
        {
            const int maskLength = 36;

            var maskBits = Enumerable.Range(0, maskLength).Select(i => 1L << i).ToArray();
            string mask = new string('X', maskLength);
            var memory = new Dictionary<long, long>();

            foreach (var line in GetInput())
            {
                var maskMatch = Regex.Match(line, "mask = (.*)");

                if (maskMatch.Success)
                {
                    var charArray = maskMatch.Groups[1].Value.ToCharArray();
                    Array.Reverse(charArray);
                    mask = new string( charArray );
                }
                else
                {
                    var memoryMatch = Regex.Match(line, @"mem\[(\d+)\] = (\d+)");

                    memory[long.Parse(memoryMatch.Groups[1].Value)] = ApplyMask(long.Parse(memoryMatch.Groups[2].Value));
                }
            }

            return memory.Values.Sum().ToString();

            long ApplyMask(long value)
            {
                for (int i = 0; i < maskLength; i++)
                {
                    switch(mask[i])
                    {
                        case '0':
                            value &= ~maskBits[i];
                            break;

                        case '1':
                            value |= maskBits[i];
                            break;
                    }
                }

                return value;
            }
        }

        public string SolveAdvanced()
        {
            const int maskLength = 36;

            var maskBits = Enumerable.Range(0, maskLength).Select(i => 1L << i).ToArray();
            string mask = new string('X', maskLength);
            var memory = new Dictionary<long, long>();

            foreach (var line in GetInput())
            {
                var maskMatch = Regex.Match(line, "mask = (.*)");

                if (maskMatch.Success)
                {
                    var charArray = maskMatch.Groups[1].Value.ToCharArray();
                    Array.Reverse(charArray);
                    mask = new string(charArray);
                }
                else
                {
                    var memoryMatch = Regex.Match(line, @"mem\[(\d+)\] = (\d+)");
                    long value = long.Parse(memoryMatch.Groups[2].Value);

                    foreach (var address in GetAddresses(long.Parse(memoryMatch.Groups[1].Value)))
                    {
                        memory[address] = value;
                    }
                }
            }

            return memory.Values.Sum().ToString();

            IEnumerable<long> GetAddresses(long baseAddress)
            {
                for (int i = 0; i < maskLength; i++)
                {
                    if (mask[i] == '1')
                    {
                        baseAddress |= maskBits[i];
                    }
                }

                var floatingNumber = (int)Math.Pow(2, mask.Count(c => c == 'X'));

                for (int floating = 0; floating < floatingNumber; floating++)
                {
                    long address = baseAddress;

                    for (int i = 0, f = 0; i < maskLength; i++)
                    {
                        if (mask[i] != 'X')
                            continue;

                        if((floating & maskBits[f]) != 0)
                        {
                            address |= maskBits[i];
                        }
                        else
                        {
                            address &= ~maskBits[i];
                        }

                        f++;
                    }

                    yield return address;
                }
            }
        }
    }
}
