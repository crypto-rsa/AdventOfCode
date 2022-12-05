using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day5 : IAdventDay
{
    public string Name => "5. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day5.txt");

    private class Cargo
    {
        private readonly Stack<char>[] _stacks;

        public Cargo(IReadOnlyList<string> lines)
        {
            var stackCount = lines[^1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .Last();

            _stacks = Enumerable.Range(0, stackCount).Select(_ => new Stack<char>()).ToArray();

            foreach (string line in lines.Reverse().Skip(1))
            {
                for (int stack = 0; stack < stackCount; stack++)
                {
                    int position = stack * 4 + 1;

                    if (line.Length <= position)
                        break;

                    if (line[position] == ' ')
                        continue;

                    _stacks[stack].Push(line[position]);
                }
            }
        }

        public void PerformOperation(string instruction, Action<int, int, int> moveAction)
        {
            var match = System.Text.RegularExpressions.Regex.Match(instruction, @"move (\d+) from (\d+) to (\d+)");

            int count = int.Parse(match.Groups[1].Value);
            int from = int.Parse(match.Groups[2].Value) - 1;
            int to = int.Parse(match.Groups[3].Value) - 1;

            moveAction(count, from, to);
        }

        public void MoveOneAtATime(int count, int from, int to)
        {
            for (int i = 0; i < count; i++)
            {
                _stacks[to].Push(_stacks[from].Pop());
            }
        }

        public void MoveMultiple(int count, int from, int to)
        {
            var tempStack = new Stack<char>();

            for (int i = 0; i < count; i++)
            {
                tempStack.Push(_stacks[from].Pop());
            }

            for (int i = 0; i < count; i++)
            {
                _stacks[to].Push(tempStack.Pop());
            }
        }

        public string GetResult() => new(_stacks.Select(s => s.Peek()).ToArray());
    }

    public string Solve()
    {
        var blocks = GetInput().ParseByBlankLines().ToArray();
        var cargo = new Cargo(blocks[0].ToArray());

        foreach (string operation in blocks[1])
        {
            cargo.PerformOperation(operation, cargo.MoveOneAtATime);
        }

        return cargo.GetResult();
    }

    public string SolveAdvanced()
    {
        var blocks = GetInput().ParseByBlankLines().ToArray();
        var cargo = new Cargo(blocks[0].ToArray());

        foreach (string operation in blocks[1])
        {
            cargo.PerformOperation(operation, cargo.MoveMultiple);
        }

        return cargo.GetResult();
    }
}
