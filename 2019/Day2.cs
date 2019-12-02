using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day2 : IAdventDay
    {
        public string Name => "2. 12. 2019";

        public string Solve()
        {
            var input = GetInput();
            input[1] = 12;
            input[2] = 2;

            Run(input);

            return input[0].ToString();
        }

        private bool Run(int[] input)
        {
            int position = 0;

            while (true)
            {
                switch (input[position])
                {
                    case 1:
                        if (input[position + 3] >= input.Length)
                            return false;

                        input[input[position + 3]] = input[input[position + 1]] + input[input[position + 2]];
                        position += 4;
                        break;

                    case 2:
                        if (input[position + 3] >= input.Length)
                            return false;

                        input[input[position + 3]] = input[input[position + 1]] * input[input[position + 2]];
                        position += 4;
                        break;

                    case 99:
                        return true;

                    default:
                        return false;
                }
            }
        }

        private static int[] GetInput() => File.ReadAllText(@"2019/Resources/day2.txt").Split(',').Select(int.Parse).ToArray();

        public string SolveAdvanced()
        {
            var originalInput = GetInput();

            for (int noun = 0; noun <= 99; noun++)
            {
                for (int verb = 0; verb <= 99; verb++)
                {
                    var input = originalInput.ToArray();
                    input[1] = noun;
                    input[2] = verb;

                    if (Run(input) && input[0] == 19690720)
                        return (100 * noun + verb).ToString();
                }
            }

            return string.Empty;
        }
    }
}