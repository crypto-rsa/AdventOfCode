using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day16 : IAdventDay
    {
        private const int Count = 16;

        public string Name => "16. 12. 2017";

        private static string[] GetInput() => System.IO.File.ReadAllText("2017/Resources/day16.txt").Split(',');

        private (List<int> Renames, List<int> Permutation) ProcessInput()
        {
            var renames = Enumerable.Range(0, Count).ToList();
            var permutation = Enumerable.Range(0, Count).ToList();

            foreach (var input in GetInput())
            {
                switch (input[0])
                {
                    case 'p':
                        var names = input.Substring(1).Split('/');
                        int firstIndex = renames.FindIndex(i => i == names[0][0] - 'a');
                        int secondIndex = renames.FindIndex(i => i == names[1][0] - 'a');
                        Swap(renames, firstIndex, secondIndex);
                        break;

                    case 's':
                        int shift = int.Parse(input.Substring(1));
                        permutation = Enumerable.Range(0, Count).Select(i => permutation[(i - shift + Count) % Count]).ToList();
                        break;

                    case 'x':
                        var indices = input.Substring(1).Split('/').Select(int.Parse).ToArray();
                        Swap(permutation, indices[0], indices[1]);
                        break;

                    default:
                        throw new System.NotSupportedException();
                }
            }

            return (renames, permutation);

            void Swap(List<int> list, int index1, int index2)
            {
                int temp = list[index1];
                list[index1] = list[index2];
                list[index2] = temp;
            }
        }

        public string Solve() => GetIteration(1);

        public string SolveAdvanced() => GetIteration(1_000_000_000);

        private string GetIteration( long iteration )
        {
            var (renames, permutation) = ProcessInput();

            return new string(Enumerable.Range(0, Count).Select(i => (char)('a' + Iterate(renames, Iterate(permutation, i)))).ToArray());

            int Iterate(List<int> list, int position)
            {
                long count = 0;
                int curPos = position;

                while (true)
                {
                    curPos = list[curPos];
                    count++;

                    if (curPos == position)
                    {
                        count = iteration - (iteration % count);
                    }

                    if (count == iteration)
                        return curPos;
                }
            }
        }
    }
}
