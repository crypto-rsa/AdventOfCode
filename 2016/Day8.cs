using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2016
{
    public class Day8 : IAdventDay
    {
        public string Name => "8. 12. 2016";

        private static IAction[] GetInput() => File.ReadAllLines("2016/Resources/day8.txt").Select(GetAction).ToArray();

        private interface IAction
        {
            char[,] Apply(char[,] array);
        }

        private record Rectangle(int Width, int Height) : IAction
        {
            public char[,] Apply(char[,] array)
            {
                var newArray = Clone(array);

                for (int row = 0; row < Height; row++)
                {
                    for (int col = 0; col < Width; col++)
                    {
                        newArray[row, col] = '#';
                    }
                }

                return newArray;
            }
        }

        private record RotateRow(int Index, int Offset) : IAction
        {
            public char[,] Apply(char[,] array)
            {
                var newArray = Clone(array);
                var width = array.GetLength(1);

                for (int col = 0; col < width; col++)
                {
                    newArray[Index, (col + Offset) % width] = array[Index, col];
                }

                return newArray;
            }
        }

        private record RotateColumn(int Index, int Offset) : IAction
        {
            public char[,] Apply(char[,] array)
            {
                var newArray = Clone(array);
                var height = array.GetLength(0);

                for (int row = 0; row < height; row++)
                {
                    newArray[(row + Offset) % height, Index] = array[row, Index];
                }

                return newArray;
            }
        }

        private static char[,] Clone(char[,] array)
        {
            var newArray = new char[array.GetLength(0), array.GetLength(1)];

            Array.Copy(array, newArray, array.GetLength(0) * array.GetLength(1));

            return newArray;
        }

        private static IAction GetAction(string line)
        {
            if (Regex.Match(line, @"rect (\d+)x(\d+)") is { Success: true } matchRectangle)
                return new Rectangle(int.Parse(matchRectangle.Groups[1].Value), int.Parse(matchRectangle.Groups[2].Value));

            if (Regex.Match(line, @"rotate row y=(\d+) by (\d+)") is { Success: true } matchRotateRow)
                return new RotateRow(int.Parse(matchRotateRow.Groups[1].Value), int.Parse(matchRotateRow.Groups[2].Value));

            if (Regex.Match(line, @"rotate column x=(\d+) by (\d+)") is { Success: true } matchRotateColumn)
                return new RotateColumn(int.Parse(matchRotateColumn.Groups[1].Value), int.Parse(matchRotateColumn.Groups[2].Value));

            throw new ArgumentException(null, nameof( line ));
        }

        public string Solve() => GetInput()
            .Aggregate(new char[6, 50], (current, action) => action.Apply(current))
            .Cast<char>()
            .Count(c => c == '#')
            .ToString();

        public string SolveAdvanced()
        {
            var array = GetInput().Aggregate(new char[6, 50], (current, action) => action.Apply(current));

            for (int row = 0; row < array.GetLength(0); row++)
            {
                for (int col = 0; col < array.GetLength(1); col++)
                {
                    Console.Write(array[row, col] == '#' ? array[row, col] : ' ');
                }

                Console.WriteLine();
            }

            return string.Empty;
        }
    }
}
