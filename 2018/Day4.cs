using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day4 : IAdventDay
    {
        private class Shift : IComparable<Shift>
        {
            public Shift(string line)
            {
                var items = line.Split(new char[] {'[', ']'}, StringSplitOptions.RemoveEmptyEntries);
                var dateTimeItems = items[0].Split('-', ' ', ':');

                Year = int.Parse(dateTimeItems[0]);
                Month = int.Parse(dateTimeItems[1]);
                Day = int.Parse(dateTimeItems[2]);
                Hour = int.Parse(dateTimeItems[3]);
                Minute = int.Parse(dateTimeItems[4]);
                Status = items[1].Trim();
                ID = -1;
            }

            public int ID { get; set; }

            public int Year { get; }

            public int Month { get; }

            public int Day { get; }

            public int Hour { get; }

            public int Minute { get; }

            public string Status { get; }

            public bool IsShiftStart => Status.StartsWith("Guard");

            public int CompareTo(Shift other)
            {
                var compareItems = new (int First, int Second)[]
                {
                    (Year, other.Year),
                    (Month, other.Month),
                    (Day, other.Day),
                    (Hour, other.Hour),
                    (Minute, other.Minute),
                };

                return compareItems.Select(item => item.First.CompareTo(item.Second)).FirstOrDefault(i => i != 0);
            }
        }

        public string Name => "4. 12. 20018";

        private static IEnumerable<Shift> GetInput() => System.IO.File.ReadAllLines(@"2018/Resources/day4.txt").Select(s => new Shift(s));

        public string Solve()
        {
            var shiftDurations = GetShiftDurations();

            var sleepyGuardID = shiftDurations.OrderByDescending(pair => pair.Value.Sum(i => i.End - i.Start)).First().Key;
            int sleepyMinute = Enumerable.Range(0, 60).OrderByDescending(m => shiftDurations[sleepyGuardID].Count(i => i.Start <= m && m < i.End)).First();

            return (sleepyGuardID * sleepyMinute).ToString();
        }

        private static Dictionary<int, List<(int Start, int End)>> GetShiftDurations()
        {
            var shifts = GetInput().ToList();
            shifts.Sort();

            int currentGuard = -1;
            foreach (var shift in shifts)
            {
                if (shift.IsShiftStart)
                {
                    currentGuard = int.Parse(shift.Status.Split(' ')[1].Trim('#'));
                }

                shift.ID = currentGuard;
            }

            var shiftsByID = shifts.Where(s => !s.IsShiftStart).GroupBy(s => s.ID);
            var shiftDurations = new Dictionary<int, List<(int Start, int End)>>();

            foreach (var shiftByID in shiftsByID)
            {
                shiftDurations[shiftByID.Key] = new List<(int, int)>();

                var list = shiftByID.ToList();
                for (int i = 0; i < list.Count; i += 2)
                {
                    shiftDurations[shiftByID.Key].Add((list[i].Minute, list[i + 1].Minute));
                }
            }

            return shiftDurations;
        }

        public string SolveAdvanced()
        {
            var shiftDurations = GetShiftDurations();

            var byFrequency = Enumerable.Range(0, 60).ToDictionary(m => m, m =>
                shiftDurations.Select(s => (ID: s.Key, Count: s.Value.Count(i => i.Start <= m && m < i.End))).OrderByDescending(i => i.Count).First());

            var mostFrequent = byFrequency.OrderByDescending(p => p.Value.Count).First();

            return (mostFrequent.Key * mostFrequent.Value.ID).ToString();
        }
    }
}