using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day21 : IAdventDay
    {
        private class Recipe
        {
            public Recipe(string input)
            {
                var match = System.Text.RegularExpressions.Regex.Match(input, @"(.*) \(contains (.*)\)");

                Ingredients = match.Groups[1].Value.Split(' ').ToHashSet();
                Allergens = match.Groups[2].Value.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            }

            public HashSet<string> Ingredients { get; }

            public HashSet<string> Allergens { get; }
        }
        public string Name => "21. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day21.txt");

        public string Solve()
        {
            var recipes = GetInput().Select(s => new Recipe(s)).ToList();
            var possibleSources = GetPossibleSources(recipes);

            var allSources = possibleSources.Values.SelectMany(s => s).ToHashSet();

            return recipes.Sum(r => r.Ingredients.Count(i => !allSources.Contains(i))).ToString();
        }

        public string SolveAdvanced()
        {
            var recipes = GetInput().Select(s => new Recipe(s)).ToList();
            var possibleSources = GetPossibleSources(recipes);
            var sources = new Dictionary<string, string>();

            int allergenCount = possibleSources.Count;

            while (sources.Count < allergenCount)
            {
                var candidate = possibleSources.First(i => i.Value.Count == 1);
                var ingredient = candidate.Value.Single();

                sources[candidate.Key] = ingredient;
                possibleSources.Remove(candidate.Key);

                foreach (var item in possibleSources.Values)
                {
                    item.Remove(ingredient);
                }
            }

            return string.Join(',', sources.OrderBy(i => i.Key).Select(i => i.Value));
        }

        private static Dictionary<string, HashSet<string>> GetPossibleSources(List<Recipe> recipes)
        {
            var possibleSources = new Dictionary<string, HashSet<string>>();

            foreach (var recipe in recipes)
            {
                foreach (var allergen in recipe.Allergens)
                {
                    if (!possibleSources.TryGetValue(allergen, out var ingredients))
                    {
                        ingredients = new HashSet<string>(recipe.Ingredients);
                        possibleSources[allergen] = ingredients;
                    }
                    else
                    {
                        possibleSources[allergen].IntersectWith(recipe.Ingredients);
                    }
                }
            }

            return possibleSources;
        }
    }
}
