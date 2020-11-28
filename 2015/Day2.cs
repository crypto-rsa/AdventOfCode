using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day2 : IAdventDay
    {
        private struct Box
        {
            public Box(string input)
            {
                var dimensions = input.Split('x').Select(int.Parse).ToArray();
                var indices = new (int Index1, int Index2)[] { (0, 1), (0, 2), (1, 2) };

                var areas = indices.Select(i => dimensions[i.Index1] * dimensions[i.Index2]).ToArray();
                var perimeters = indices.Select(i => 2 * (dimensions[i.Index1] + dimensions[i.Index2])).ToArray();

                Area = 2 * areas.Sum() + areas.Min();
                RibbonLength = perimeters.Min() + dimensions[0] * dimensions[1] * dimensions[2];
            }

            public int Area { get; }

            public int RibbonLength { get; }
        }

        public string Name => "2. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day2.txt");

        public string Solve()
        {
            var input = GetInput();

            return input.Select(s => new Box(s)).Sum(b => b.Area).ToString();
        }

        public string SolveAdvanced()
        {
            var input = GetInput();

            return input.Select(s => new Box(s)).Sum(b => b.RibbonLength).ToString();
        }
    }
}
