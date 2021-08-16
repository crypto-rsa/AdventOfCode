using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2016
{
    public class Day4 : IAdventDay
    {
        public string Name => "4. 12. 2016";

        private static IEnumerable<Room> GetInput() => File.ReadAllLines("2016/Resources/day4.txt").Select(s => new Room(s));

        private readonly struct Room
        {
            public Room(string s)
            {
                var match = Regex.Match(s, @"([a-z\-]*)(\d+)\[([a-z]+)\]");
                string encryptedName = match.Groups[1].Value;

                var parts = encryptedName
                    .Replace("-", string.Empty)
                    .GroupBy(c => c)
                    .OrderByDescending(g => g.Count())
                    .ThenBy(g => g.Key)
                    .Take(5)
                    .Select(g => g.Key)
                    .ToArray();

                IsReal = match.Groups[3].Value == new string(parts);
                SectorId = long.Parse(match.Groups[2].Value);

                var rotations = SectorId % 26;

                DecryptedName = new string(encryptedName.Select(Rotate).ToArray());

                char Rotate(char c)
                {
                    if (c == '-')
                        return ' ';

                    return (char)('a' + ((c - 'a') + rotations) % 26);
                }
            }

            public bool IsReal { get; }

            public long SectorId { get; }

            public string DecryptedName { get; }
        }

        public string Solve() => GetInput().Where(r => r.IsReal).Sum(r => r.SectorId).ToString();

        public string SolveAdvanced()
        {
            var room = GetInput().FirstOrDefault(r => r.DecryptedName.Contains("north"));

            return room.IsReal ? $"{room.DecryptedName}: {room.SectorId}" : "Not found";
        }
    }
}
