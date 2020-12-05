using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2015
{
    public class Day15 : IAdventDay
    {
        private class Ingredient
        {
            public Ingredient(string input)
            {
                var match = Regex.Match(input, @"(\w+): capacity (-?\d+), durability (-?\d+), flavor (-?\d+), texture (-?\d+), calories (-?\d+)");

                Name = match.Groups[1].Value;
                Capacity = GetValue(2);
                Durability = GetValue(3);
                Flavor = GetValue(4);
                Texture = GetValue(5);
                Calories = GetValue(6);

                int GetValue(int group) => int.Parse(match.Groups[group].Value);
            }

            public string Name { get; }

            public int Capacity { get; }

            public int Durability { get; }

            public int Flavor { get; }

            public int Texture { get; }

            public int Calories { get; }

            public int[] GetVector(int multiplier) => new int[]
            {
                multiplier * Capacity,
                multiplier * Durability,
                multiplier * Flavor,
                multiplier * Texture
            };
        }

        private const int Spoons = 100;

        public string Name => "15. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day15.txt");

        public string Solve() => SolveInternal(checkCalories: false).ToString();

        public string SolveAdvanced() => SolveInternal(checkCalories: true).ToString();

        public int SolveInternal(bool checkCalories)
        {
            var ingredients = GetInput().Select(s => new Ingredient(s)).ToList();

            return GetPartitions(Spoons, ingredients.Count).Where(IsValid).Max(GetScore);

            int GetScore(int[] counts) => Product(ingredients.Zip(counts, (i, k) => i.GetVector(k)).Aggregate(new int[4], Sum));

            static int[] Sum(int[] prev, int[] next) => prev.Zip(next, (i, j) => i + j).ToArray();

            static int Product(int[] values) => values.Aggregate(1, (acc, next) => acc * Math.Max(0, next));

            bool IsValid(int[] counts) => !checkCalories || ingredients.Zip(counts, (i, k) => i.Calories * k).Sum() == 500;
        }

        private static IEnumerable<int[]> GetPartitions(int sum, int count)
        {
            var values = new int[count];
            values[0] = sum;

            while (true)
            {
                yield return values.ToArray();

                int moveIndex = count - 2;
                while (moveIndex >= 0 && values[moveIndex] == 0)
                    moveIndex--;

                if (moveIndex < 0)
                    break;

                values[moveIndex]--;
                values[moveIndex + 1] = sum - values.Take(moveIndex + 1).Sum();

                for(int i = moveIndex + 2; i < count; i++)
                {
                    values[i] = 0;
                }
            }
        }
    }
}
