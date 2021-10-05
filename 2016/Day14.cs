using System;
using System.Linq;
using System.Text;

namespace Advent_of_Code.Year2016
{
    public class Day14 : IAdventDay
    {
        public string Name => "14. 12. 2016";

        private const string Salt = "cuanljph";

        private static int GetLastKeyIndex(int iterations = 0)
        {
            const int window = 1000;

            int found = 0;
            var cache = new byte[window + 1][];
            var md5 = System.Security.Cryptography.MD5.Create();

            var indices = Enumerable.Range(0, 256).Select(GetIndices).ToArray();

            for (int i = 0, currentIndex = 0;; i++, currentIndex = (currentIndex + 1) % (window + 1))
            {
                cache[currentIndex] = GetBytes(i);
                cache[(currentIndex + window) % (window + 1)] = GetBytes(i + window);

                byte? triplet = GetFirstTriplet(cache[currentIndex]);

                if(!triplet.HasValue)
                    continue;

                for (int j = 1; j <= window; j++)
                {
                    var index = (currentIndex + j) % (window + 1);

                    cache[index] ??= GetBytes(i + j);

                    if (HasQuintuplet(cache[index], triplet.Value))
                    {
                        found++;
                        break;
                    }
                }

                if (found == 64)
                    return i;
            }

            byte[] GetBytes(int i)
            {
                var current = md5.ComputeHash(Encoding.ASCII.GetBytes(Salt + i));

                for (int it = 0; it < iterations; it++)
                {
                    current = md5.ComputeHash(Encoding.ASCII.GetBytes(Convert.ToHexString(current).ToLower()));
                }

                return current;
            }

            bool HasQuintuplet(byte[] array, byte value)
            {
                for (int i = 0; i < array.Length - 2; i++)
                {
                    (byte? first, _, byte firstLower) = indices[array[i + 0]];
                    (byte? mid, _, _) = indices[array[i + 1]];
                    (byte? last, byte lastUpper, _) = indices[array[i + 2]];

                    if (mid == value && ((first == mid && lastUpper == mid) || (firstLower == mid && last == mid)))
                        return true;
                }

                return false;
            }

            byte? GetFirstTriplet(byte[] array)
            {
                for (int i = 0; i < array.Length - 1; i++)
                {
                    (byte? first, _, byte firstLower) = indices[array[i + 0]];
                    (byte? second, byte secondUpper, _) = indices[array[i + 1]];

                    if (first == secondUpper)
                        return first;

                    if (firstLower == second)
                        return second;
                }

                return null;
            }

            static (byte?, byte, byte) GetIndices(int value)
            {
                byte upper = (byte)(value >> 4);
                byte lower = (byte)(value & 0xF);
                byte? both = upper == lower ? upper : null;

                return (both, upper, lower);
            }
        }

        public string Solve() => GetLastKeyIndex().ToString();

        public string SolveAdvanced() => GetLastKeyIndex(2016).ToString();
    }
}
