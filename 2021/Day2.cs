using System.IO;

namespace Advent_of_Code.Year2021
{
    public class Day2 : IAdventDay
    {
        public string Name => "2. 12. 2021";

        private static string[] GetInput() => File.ReadAllLines("2021/Resources/day2.txt");

        public string Solve()
        {
            int distance = 0;
            int depth = 0;

            foreach (string instruction in GetInput())
            {
                var parts = instruction.Split(' ');
                int value = int.Parse(parts[1]);

                switch (parts[0])
                {
                    case "forward":
                        distance += value;

                        break;

                    case "down":
                        depth += value;

                        break;

                    case "up":
                        depth -= value;

                        break;
                }
            }

            return (distance * depth).ToString();
        }

        public string SolveAdvanced()
        {
            long distance = 0;
            long depth = 0;
            long aim = 0;

            foreach (string instruction in GetInput())
            {
                var parts = instruction.Split(' ');
                long value = long.Parse(parts[1]);

                switch (parts[0])
                {
                    case "forward":
                        distance += value;
                        depth += aim * value;

                        break;

                    case "down":
                        aim += value;

                        break;

                    case "up":
                        aim -= value;

                        break;
                }
            }

            return (distance * depth).ToString();
        }
    }
}
