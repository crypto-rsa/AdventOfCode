using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day8 : IAdventDay
    {
        public string Name => "8. 12. 2019";

        public string Solve() => CheckConsistency(25, 6).ToString();

        public string SolveAdvanced() => GetFinalImage(25, 6);

        private static string GetInput() => File.ReadAllText("2019/Resources/day8.txt");

        private int CheckConsistency(int width, int height)
        {
            var resultLayer = GetLayers(width, height).OrderBy(s => s.Count(c => c == '0')).First();

            return resultLayer.Count(c => c == '1') * resultLayer.Count(c => c == '2');
        }

        private List<string> GetLayers(int width, int height)
        {
            var input = GetInput();
            var layers = new List<string>();
            int layerLength = width * height;

            for (int i = 0; i < input.Length / layerLength; i++)
            {
                layers.Add(input[(i * layerLength)..((i + 1) * layerLength)]);
            }

            return layers;
        }

        private string GetFinalImage(int width, int height)
        {
            var layers = GetLayers(width, height);

            var image = string.Join(System.Environment.NewLine, Enumerable.Range(0, height).Select(GetRow));

            System.Console.WriteLine(image.Replace('0', ' ').Replace('1', '*'));

            return "See above";

            string GetRow(int row) => new string(Enumerable.Range(0, width).Select(i => GetPixel(row, i)).ToArray());

            char GetPixel(int row, int position) => layers.Select(s => s[row * width + position]).First(c => c != '2');
        }
    }
}
