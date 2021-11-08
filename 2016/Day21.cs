using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2016
{
    public class Day21 : IAdventDay
    {
        public string Name => "21. 12. 2016";

        private static IEnumerable<string> GetInput() => File.ReadAllLines("2016/Resources/day21.txt");

        private static string Scramble(string password)
        {
            var builder = new System.Text.StringBuilder(password);

            foreach (var instruction in GetInput())
            {
                if (instruction.StartsWith("swap position"))
                {
                    SwapPositions(instruction);
                }
                else if (instruction.StartsWith("swap letter"))
                {
                    SwapLetters(instruction);
                }
                else if (instruction.StartsWith("rotate based"))
                {
                    RotateBasedOnPosition(instruction);
                }
                else if (instruction.StartsWith("rotate"))
                {
                    SimpleRotate(instruction);
                }
                else if (instruction.StartsWith("reverse"))
                {
                    ReversePositions(instruction);
                }
                else if (instruction.StartsWith("move"))
                {
                    Move(instruction);
                }
                else
                {
                    throw new InvalidOperationException("Invalid instruction!");
                }
            }

            return builder.ToString();

            void SwapPositions(string instruction)
            {
                var match = Regex.Match(instruction, @"swap position (\d+) with position (\d+)");
                (int pos1, int pos2) = (Parse(match, 1), Parse(match, 2));

                (builder[pos1], builder[pos2]) = (builder[pos2], builder[pos1]);
            }

            void SwapLetters(string instruction)
            {
                var match = Regex.Match(instruction, @"swap letter (.) with letter (.)");
                (int pos1, int pos2) = (FindIndex(GetChar(match, 1)), FindIndex(GetChar(match, 2)));

                (builder[pos1], builder[pos2]) = (builder[pos2], builder[pos1]);
            }

            void SimpleRotate(string instruction)
            {
                var match = Regex.Match(instruction, @"rotate (left|right) (\d+) steps?");
                int offset = (match.Groups[1].Value == "left" ? +1 : -1) * Parse(match, 2);

                Rotate(offset);
            }

            void RotateBasedOnPosition(string instruction)
            {
                var match = Regex.Match(instruction, @"rotate based on position of letter (.)");
                int index = FindIndex(GetChar(match, 1));

                if (index >= 4)
                {
                    index++;
                }

                Rotate(-(index + 1));
            }

            void Rotate(int offset)
            {
                var original = builder.ToString();
                int start = (2 * original.Length + offset) % original.Length;

                for (int i = 0; i < original.Length; i++)
                {
                    builder[i] = original[(start + i) % original.Length];
                }
            }

            void ReversePositions(string instruction)
            {
                var match = Regex.Match(instruction, @"reverse positions (\d+) through (\d+)");
                (int pos1, int pos2) = (Parse(match, 1), Parse(match, 2));

                var original = builder.ToString();

                for (int i = pos1, j = pos2; i <= pos2; i++, j--)
                {
                    builder[i] = original[j];
                }
            }

            void Move(string instruction)
            {
                var match = Regex.Match(instruction, @"move position (\d+) to position (\d+)");
                (int pos1, int pos2) = (Parse(match, 1), Parse(match, 2));

                char c = builder[pos1];
                builder.Remove(pos1, 1);
                builder.Insert(pos2, c);
            }

            static int Parse(Match match, int index) => int.Parse(match.Groups[index].Value);

            static char GetChar(Match match, int index) => match.Groups[index].Value[0];

            int FindIndex(char c) => Enumerable.Range(0, builder.Length).First(i => builder[i] == c);
        }

        public string Solve() => Scramble("abcdefgh");

        public string SolveAdvanced()
        {
            const string pattern = "abcdefgh";
            const string target = "fbgdceah";

            foreach (int[] permutation in Tools.Combinatorics.GetPermutations(target.Length))
            {
                var input = new string(permutation.Select(i => pattern[i]).ToArray());

                if (Scramble(input) == target)
                    return input;
            }

            return "No solution found!";
        }
    }
}
