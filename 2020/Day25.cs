using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day25 : IAdventDay
    {
        public string Name => "25. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day25.txt");

        public string Solve()
        {
            var keys = GetInput().Select(long.Parse).ToArray();
            var loopSize = new long[2];

            const long mod = 20201227;

            for(long current = 1, loop = 0; loopSize[0] == 0 || loopSize[1] == 0; loop++)
            {
                if (current == keys[0])
                {
                    loopSize[0] = loop;
                }

                if (current == keys[1])
                {
                    loopSize[1] = loop;
                }

                current = (current * 7) % mod;
            }

            var encryptionKeys = new long[] { 1, 1 };

            for(long i = 0; i < loopSize[0]; i++)
            {
                encryptionKeys[0] = (encryptionKeys[0] * keys[1]) % mod;
            }

            for(long i = 0; i < loopSize[1]; i++)
            {
                encryptionKeys[1] = (encryptionKeys[1] * keys[0]) % mod;
            }

            return encryptionKeys[0] == encryptionKeys[1] ? encryptionKeys[0].ToString() : "The codes do not match!";
        }

        public string SolveAdvanced() => string.Empty;
    }
}
