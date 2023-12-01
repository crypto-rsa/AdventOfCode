using System;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day1 : IAdventDay
{
    public string Name => "1. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day1.txt");

    private readonly char[] _numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    private readonly string[] _numberWords = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

    public string Solve()
    {
        return GetInput()
            .SplitToLines()
            .Select(GetNumber)
            .Sum()
            .ToString();

        int GetNumber(string s)
        {
            int first = s.IndexOfAny(_numbers);
            int last = s.LastIndexOfAny(_numbers);

            return int.Parse($"{s[first]}{s[last]}");
        }
    }

    public string SolveAdvanced()
    {
        return GetInput()
            .SplitToLines()
            .Select(GetNumber)
            .Sum()
            .ToString();

        int GetNumber(string s)
        {
            int first = s.IndexOfAny(_numbers);
            int last = s.LastIndexOfAny(_numbers);

            var firstWord = first != -1 ? (Index: first, Word: s[first].ToString()) : (Index: s.Length, Word: string.Empty);
            var lastWord = last != -1 ? (Index: last, Word: s[last].ToString()) : (Index: -1, Word: string.Empty);

            for(int i = 0; i < _numberWords.Length; i++)
            {
                var word = _numberWords[i];
                int firstIndex = s.IndexOf(word, StringComparison.Ordinal);

                if (firstIndex != -1 && firstIndex < firstWord.Index)
                {
                    firstWord = (firstIndex, i.ToString());
                }

                int lastIndex = s.LastIndexOf(word, StringComparison.Ordinal);

                if (lastIndex > lastWord.Index)
                {
                    lastWord = (lastIndex, i.ToString());
                }
            }

            return int.Parse($"{firstWord.Word}{lastWord.Word}");
        }
    }
}
