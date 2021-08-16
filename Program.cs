using System;
using System.Linq;
using System.Reflection;

namespace Advent_of_Code
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Advent_of_Code <year> <day>");
                return;
            }

            var day = GetAdventDay();

            if (day == null)
            {
                Console.WriteLine($"Cannot find day {args[1]} for year {args[0]}!");
                return;
            }

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine($"{day.Name}: {day.Solve()}, {day.SolveAdvanced()}");
            Console.WriteLine($"Calculated in {stopWatch.ElapsedMilliseconds} ms");

            IAdventDay GetAdventDay()
            {
                string fullName = $"Advent_of_Code.Year{args[0]}.Day{args[1]}";

                var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.FullName == fullName);

                if (type == null)
                    return null;

                return Activator.CreateInstance(type) as IAdventDay;
            }
        }
    }
}
