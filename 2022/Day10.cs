using System;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day10 : IAdventDay
{
    public string Name => "10. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day10.txt");

    public string Solve()
    {
        int x = 1;
        int cyclesComplete = 0;
        var checkCycles = new[] { 20, 60, 100, 140, 180, 220 }.ToQueue();

        int nextCheck = checkCycles.Dequeue();
        int total = 0;

        foreach (string line in GetInput().SplitToLines())
        {
            var parts = line.Split(' ');

            switch (parts[0])
            {
                case "addx":
                    if (cyclesComplete + 2 >= nextCheck)
                    {
                        total += nextCheck * x;

                        checkCycles.TryDequeue(out nextCheck);
                    }

                    cyclesComplete += 2;
                    x += int.Parse(parts[1]);

                    break;
                
                case "noop":
                    cyclesComplete++;

                    break;
            }
        }

        return total.ToString();
    }

    public string SolveAdvanced()
    {
        int spritePosition = 1;

        var instructions = GetInput().SplitToLines().Select( Parse ).ToQueue();
        (string Type, int AddX) currentInstruction = (null, 0);
        int currentInstructionEnd = -1;

        for (int cycle = 1;  ;cycle++)
        {
            int pixelPosition = (cycle - 1) % 40;
            
            if (currentInstruction.Type == null)
            {
                if (!instructions.TryDequeue(out currentInstruction))
                    return "^^^";

                currentInstructionEnd = cycle + (currentInstruction.Type == "addx" ? 1 : 0);
            }

            Console.Write(Math.Abs(pixelPosition - spritePosition) <= 1 ? '#' : ' ');

            if (cycle % 40 == 0)
            {
                Console.WriteLine();
            }

            if (cycle == currentInstructionEnd)
            {
                spritePosition += currentInstruction.AddX;
                currentInstruction = (null, 0);
            }
        }
        
        (string Type, int AddX) Parse(string line)
        {
            var parts = line.Split(' ');
            int addX = parts[0] == "addx" ? int.Parse(parts[1]) : 0;

            return (parts[0], addX);
        }
    }
}
