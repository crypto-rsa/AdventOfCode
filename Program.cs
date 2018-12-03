using System;

namespace Advent_of_Code
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            IAdventDay day = new Advent_of_Code_2018.Day3();

            Console.WriteLine($"{day.Name}: {day.Solve()}, {day.SolveAdvanced()}");
            Console.WriteLine($"Calculated in {stopWatch.ElapsedMilliseconds} ms");
        }
    }
}
