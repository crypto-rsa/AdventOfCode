using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2016
{
    public class Day16 : IAdventDay
    {
        public string Name => "16. 12. 2016";

        private const string Input = "01111001100111011";

        private static string CalculateChecksum(int targetLength)
        {
            var data = Input.Select(c => c == '1' ? (byte)1 : (byte)0).ToList();

            while (data.Count < targetLength)
            {
                data.Add(0);

                for (int i = data.Count - 2; i >= 0; i--)
                {
                    data.Add((byte)(1 - data[i]));

                    if (data.Count >= targetLength)
                        break;
                }
            }

            while (data.Count % 2 == 0)
            {
                var sum = new List<byte>();

                for (int i = 0; i < data.Count - 1; i += 2)
                {
                    byte a = data[i];
                    byte b = data[i + 1];

                    sum.Add(a == b ? (byte)1 : (byte)0);
                }

                data = sum;
            }

            return new string(data.Select(b => b == 1 ? '1' : '0').ToArray());
        }

        public string Solve() => CalculateChecksum(272);

        public string SolveAdvanced() => CalculateChecksum(35651584);
    }
}
