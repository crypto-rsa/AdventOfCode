using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day12 : IAdventDay
    {
        private record Instruction
        {
            public Instruction(string input)
                : this(input[0], int.Parse(input[1..]))
            {
            }

            public Instruction(char operation, int value)
            {
                Operation = operation;
                Value = value;
            }

            public char Operation { get; }

            public int Value { get; }
        }

        private record Position(int North, int East)
        {
            public int Distance => Math.Abs(North) + Math.Abs(East);

            public Position Rotate(char direction, int angle) => (direction, angle) switch
            {
                (_, 180) => new Position(-North, -East),
                ('R', 90) => new Position(-East, North),
                ('L', 90) => new Position(East, -North),
                ('R', 270) => Rotate('L', 90),
                ('L', 270) => Rotate('R', 90),
                _ => throw new InvalidOperationException(),
            };
        }

        public string Name => "12. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day12.txt");

        private static readonly List<char> _rightDirections = new List<char>() { 'E', 'S', 'W', 'N' };

        public string Solve()
        {
            var instructions = GetInput().Select(s => new Instruction(s));

            char direction = 'E';
            var position = new Position(0, 0);

            foreach (var instruction in instructions)
            {
                (direction, position) = Update(instruction);
            }

            return position.Distance.ToString();

            (char, Position) Update(Instruction instruction) => instruction.Operation switch
            {
                'N' => (direction, position with { North = position.North + instruction.Value }),
                'E' => (direction, position with { East = position.East + instruction.Value }),
                'S' => (direction, position with { North = position.North - instruction.Value }),
                'W' => (direction, position with { East = position.East - instruction.Value }),
                'L' => (GetDirection(instruction), position),
                'R' => (GetDirection(instruction), position),
                'F' => Update(new Instruction(direction, instruction.Value)),
                _ => throw new InvalidOperationException(),
            };

            char GetDirection(Instruction instruction)
            {
                var offset = instruction.Operation switch
                {
                    'R' => +instruction.Value / 90,
                    'L' => -instruction.Value / 90,
                    _ => throw new InvalidOperationException(),
                };

                var index = (_rightDirections.IndexOf(direction) + offset + 4) % 4;

                return _rightDirections[index];
            }
        }

        public string SolveAdvanced()
        {
            var instructions = GetInput().Select(s => new Instruction(s));

            var waypoint = new Position(1, 10);
            var ship = new Position(0, 0);

            foreach (var instruction in instructions)
            {
                (waypoint, ship) = Update(instruction);
            }

            return ship.Distance.ToString();

            (Position waypoint, Position ship) Update(Instruction instruction) => instruction.Operation switch
            {
                'N' => (waypoint with { North = waypoint.North + instruction.Value }, ship),
                'E' => (waypoint with { East = waypoint.East + instruction.Value }, ship),
                'S' => (waypoint with { North = waypoint.North - instruction.Value }, ship),
                'W' => (waypoint with { East = waypoint.East - instruction.Value }, ship),
                'L' => (waypoint.Rotate(instruction.Operation, instruction.Value), ship),
                'R' => (waypoint.Rotate(instruction.Operation, instruction.Value), ship),
                'F' => (waypoint, ship with { North = ship.North + instruction.Value * waypoint.North, East = ship.East + instruction.Value * waypoint.East }),
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
