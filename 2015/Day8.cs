using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day8 : IAdventDay
    {
        public string Name => "8. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day8.txt");

        public string Solve()
        {
            return GetInput().Sum(GetEscapingLength).ToString();

            static int GetEscapingLength(string s)
            {
                const string hexadecimal = "0123456789abcdef";

                int length = 2;

                for (int i = 1; i < s.Length - 1;)
                {
                    if (s[i] == '\\')
                    {
                        if (i < s.Length - 2 && (s[i + 1] == '"' || s[i + 1] == '\\'))
                        {
                            length++;
                            i += 2;
                            continue;
                        }
                        else if (i < s.Length - 4 && s[i + 1] == 'x' && hexadecimal.Contains(s[i + 2]) && hexadecimal.Contains(s[i + 3]))
                        {
                            length += 3;
                            i += 4;
                            continue;
                        }
                    }

                    i++;
                }

                return length;
            }
        }

        public string SolveAdvanced()
        {
            return GetInput().Sum(GetEscapingLength).ToString();

            static int GetEscapingLength(string s)
            {
                int length = 2;

                for (int i = 0; i < s.Length; i++)
                {
                    length += s[i] switch
                    {
                        '"' => 2,
                        '\\' => 2,
                        _ => 1,
                    };
                }

                return length - s.Length;
            }
        }
    }
}
