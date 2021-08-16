using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent_of_Code.Year2016
{
    public class Day5 : IAdventDay
    {
        public string Name => "5. 12. 2016";

        private static string GetInput() => "ugkcyxxp";

        private static IEnumerable<(char Sixth, char Seventh)> GetPasswordChars()
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            string input = GetInput();

            for (int i = 0;; i++)
            {
                var bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(input + i));
                int number = bytes[0] << 16 | bytes[1] << 8 | bytes[2];

                if (number >= 16)
                    continue;

                yield return (GetHexChar(number), GetHexChar(bytes[3] >> 4));

                char GetHexChar(int num) => num switch
                {
                    >= 0 and < 10 => (char)('0' + num),
                    >= 10 and < 16 => (char)('a' + (num - 10)),
                    _ => throw new System.ArgumentException(),
                };
            }
        }

        public string Solve() => string.Join(string.Empty, GetPasswordChars().Select(i => i.Sixth).Take(8));

        public string SolveAdvanced()
        {
            int toFill = 8;
            var passwordChars = new char?[toFill];

            foreach (var (sixth, seventh) in GetPasswordChars())
            {
                TryFill(sixth, seventh);

                if (toFill == 0)
                    return string.Join(string.Empty, passwordChars);
            }

            return string.Empty;

            void TryFill(char sixth, char seventh)
            {
                int i = sixth switch
                {
                    >= '0' and <= '7' => sixth - '0',
                    _ => -1,
                };

                if (i is < 0 or >= 8)
                    return;

                if (passwordChars[i] != null)
                    return;

                passwordChars[i] = seventh;
                toFill--;
            }
        }
    }
}
