using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2020
{
    public class Day4 : IAdventDay
    {
        public string Name => "4. 12. 2020";

        private static string GetInput() => System.IO.File.ReadAllText("2020/Resources/day4.txt");

        public string Solve() => GetValidPassportCount(validate: false).ToString();

        public string SolveAdvanced() => GetValidPassportCount(validate: true).ToString();

        private static readonly HashSet<string> _requiredFields = new HashSet<string>() { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };

        private static int GetValidPassportCount(bool validate)
        {
            return GetInput().Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries).Count(IsValid);

            bool IsValid(string passport)
            {
                var fields = new Dictionary<string, string>( passport.Split(new string[] { "\r\n", " " }, StringSplitOptions.RemoveEmptyEntries).Select(GetKeyValuePairs));

                if (!_requiredFields.All(fields.ContainsKey))
                    return false;

                if (!validate)
                    return true;

                if (!IsNumber(fields["byr"], 4, 1920, 2002))
                    return false;

                if (!IsNumber(fields["iyr"], 4, 2010, 2020))
                    return false;

                if (!IsNumber(fields["eyr"], 4, 2020, 2030))
                    return false;

                if (!IsHeight(fields["hgt"]))
                    return false;

                if (!Regex.IsMatch(fields["hcl"], "^#[0-9a-f]{6}$"))
                    return false;

                if (!Regex.IsMatch(fields["ecl"], "^(amb|blu|brn|gry|grn|hzl|oth)$"))
                    return false;

                if (!Regex.IsMatch(fields["pid"], @"^\d{9}$"))
                    return false;

                return true;
            }

            KeyValuePair<string, string> GetKeyValuePairs(string field)
            {
                var match = Regex.Match(field, @"(\w+):(.*)");

                return new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value);
            }

            bool IsNumber(string input, int length, int minValue, int maxValue)
                => input.Length == length && int.TryParse(input, out int value) && value >= minValue && value <= maxValue;

            bool IsHeight(string input)
            {
                var match = Regex.Match(input, @"^(\d+)(cm|in)$");

                if (!match.Success)
                    return false;

                var (minValue, maxValue) = match.Groups[2].Value switch
                {
                    "cm" => (150, 193),
                    "in" => (59, 76),
                    _ => (0, 0),
                };

                return IsNumber(match.Groups[1].Value, match.Groups[1].Value.Length, minValue, maxValue);
            }
        }
    }
}
