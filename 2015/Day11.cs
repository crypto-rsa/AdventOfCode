using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day11 : IAdventDay
    {
        public string Name => "11. 12. 2015";

        private const string Input = "hxbxwxba";

        private const string Characters = "abcdefghjkmnpqrstuvwxyz";

        private static readonly char[] _charactersArray = Characters.ToCharArray();

        private static readonly List<string> _allowedTriples = Enumerable.Range(0, Characters.Length - 2).Select(i => Characters[i..(i + 3)]).ToList();

        private static readonly List<string> _allowedPairs = Characters.Select(c => new string(c, 2)).ToList();

        public string Solve() => FindNextPassword(Input);

        public string SolveAdvanced() => FindNextPassword(FindNextPassword(Input));

        private string FindNextPassword(string current)
        {
            var password = current.ToCharArray();

            do
            {
                Increment(password);
            }
            while (!IsValidPassword(password));

            return new string(password);
        }

        private void Increment(char[] password)
        {
            for (int i = password.Length - 1; i >= 0; i--)
            {
                bool increment = password[i] != 'z';

                password[i] = increment ? (char)(password[i] + 1) : 'a';

                if (increment)
                    break;
            }
        }

        private bool IsValidPassword(char[] password)
        {
            var s = new string(password);

            return password.All(c => System.Array.BinarySearch(_charactersArray, c) >= 0)
                && _allowedTriples.Any(s.Contains)
                && _allowedPairs.Count(s.Contains) > 1;
        }
    }
}
