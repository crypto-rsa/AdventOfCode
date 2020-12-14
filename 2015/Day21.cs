using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2015
{
    public class Day21 : IAdventDay
    {
        private record Person(int HitPoints, int Damage, int Armor)
        {
            public bool KillsBoss(Person boss) => RoundsToKill(boss) <= boss.RoundsToKill(this);

            public int RoundsToKill(Person other)
            {
                int trueDamage = Math.Max(1, Damage - other.Armor);
                var rounds = Math.DivRem(other.HitPoints, trueDamage, out var remainder);

                return rounds + (remainder > 0 ? 1 : 0);
            }
        }

        public string Name => "21. 12. 2015";

        private static string GetInput() => System.IO.File.ReadAllText("2015/Resources/day21.txt");

        public string Solve()
        {
            var match = Regex.Match(GetInput(), "Hit Points: (\\d+)\r\nDamage: (\\d+)\r\nArmor: (\\d+)");
            var boss = new Person(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));

            return GetPlayers().OrderBy(i => i.Cost).FirstOrDefault(i => i.person.KillsBoss(boss)).Cost.ToString();
        }

        public string SolveAdvanced()
        {
            var match = Regex.Match(GetInput(), "Hit Points: (\\d+)\r\nDamage: (\\d+)\r\nArmor: (\\d+)");
            var boss = new Person(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));

            return GetPlayers().OrderByDescending(i => i.Cost).FirstOrDefault(i => !i.person.KillsBoss(boss)).Cost.ToString();
        }

        private static IEnumerable<(Person person, int Cost)> GetPlayers()
        {
            var rings = GetRings().ToList();

            foreach (var weapon in GetWeapons())
            {
                foreach (var armor in GetArmor())
                {
                    for( int i = 0; i < rings.Count - 1; i++)
                    {
                        for(int j = i; j < rings.Count; j++ )
                        {
                            if (i != 0 && i == j)
                                continue;

                            var ring1 = rings[i];
                            var ring2 = rings[j];
                            var person = new Person(100, weapon.Damage + ring1.Damage + ring2.Damage, armor.Armor + ring1.Armor + ring2.Armor);

                            yield return (person, weapon.Cost + armor.Cost + ring1.Cost + ring2.Cost);
                        }
                    }
                }
            }
        }

        private static IEnumerable<(string Name, int Cost, int Damage)> GetWeapons()
        {
            yield return ("Dagger", 8, 4);
            yield return ("Shortsword", 10, 5);
            yield return ("Warhammer", 25, 6);
            yield return ("Longsword", 40, 7);
            yield return ("Greataxe", 74, 8);
        }

        private static IEnumerable<(string Name, int Cost, int Armor)> GetArmor()
        {
            yield return ("None", 0, 0);
            yield return ("Leather", 13, 1);
            yield return ("Chainmail", 31, 2);
            yield return ("Splintmail", 53, 3);
            yield return ("Bandedmail", 75, 4);
            yield return ("Platemail", 102, 5);
        }

        private static IEnumerable<(string Name, int Cost, int Damage, int Armor)> GetRings()
        {
            yield return ("None", 0, 0, 0);
            yield return ("Damage +1", 25, 1, 0);
            yield return ("Damage +2", 50, 2, 0);
            yield return ("Damage +3", 100, 3, 0);
            yield return ("Defense +1", 20, 0, 1);
            yield return ("Defense +2", 40, 0, 2);
            yield return ("Defense +3", 80, 0, 3);
        }
    }
}
