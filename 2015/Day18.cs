using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day18 : IAdventDay
    {
        public string Name => "18. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day18.txt");

        private const int Size = 100;
        private const int Steps = 100;
        private const char On = '#';
        private const char Off = '.';

        public string Solve() => CountLightsOn(cornersAlwaysOn: false).ToString();

        public string SolveAdvanced() => CountLightsOn(cornersAlwaysOn: true).ToString();

        private int CountLightsOn(bool cornersAlwaysOn)
        {
            var arrays = new char[][][]
            {
                GetInput().Select(s => s.ToCharArray()).ToArray(),
                Enumerable.Range(0, Size).Select(_ => new char[Size]).ToArray(),
            };

            for (int i = 0; i < Steps; i++)
            {
                var input = arrays[i % 2];
                var output = arrays[1 - i % 2];

                Iterate(input, output);

                if (cornersAlwaysOn)
                {
                    output[0][0] = On;
                    output[0][Size - 1] = On;
                    output[Size - 1][0] = On;
                    output[Size - 1][Size - 1] = On;
                }
            }

            return arrays[0].Sum(a => a.Count(c => c == On));
        }

        private void Iterate(char[][] input, char[][] output)
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    output[row][col] = (input[row][col], GetLiveNeighbours(row, col)) switch
                    {
                        (On, 2) => On,
                        (On, 3) => On,
                        (Off, 3) => On,
                        _ => Off,
                    };
                }
            }

            int GetLiveNeighbours(int row, int col)
                => GetNeighbours().Where(i => IsValid(row, col, i)).Count(i => input[row + i.RowOffset][col + i.ColOffset] == On);

            IEnumerable<(int RowOffset, int ColOffset)> GetNeighbours()
            {
                yield return (-1, -1);
                yield return (0, -1);
                yield return (+1, -1);
                yield return (-1, 0);
                yield return (+1, 0);
                yield return (-1, +1);
                yield return (0, +1);
                yield return (+1, +1);
            }

            bool IsValid(int row, int col, (int RowOffset, int ColOffset) offset)
                => row + offset.RowOffset >= 0 && row + offset.RowOffset < Size
                && col + offset.ColOffset >= 0 && col + offset.ColOffset < Size;
        }
    }
}
