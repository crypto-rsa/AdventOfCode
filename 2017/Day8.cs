using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day8 : IAdventDay
    {
        public string Name => "8. 12. 2017";

        private static string[] GetInput() => System.IO.File.ReadAllLines(@"2017/Resources/day8.txt");

        public string Solve()
        {
            return ProcessRegisters(out _).Values.Max().ToString();
        }

        private static Dictionary<string, int> ProcessRegisters( out int highestValue )
        {
            var registers = new Dictionary<string, int>();

            var testMethods = new Dictionary<string, Func<int, int, bool>>()
            {
                [">"] = ( curValue, testValue ) => curValue > testValue,
                [">="] = ( curValue, testValue ) => curValue >= testValue,
                ["<="] = ( curValue, testValue ) => curValue <= testValue,
                ["=="] = ( curValue, testValue ) => curValue == testValue,
                ["!="] = ( curValue, testValue ) => curValue != testValue,
                ["<"] = ( curValue, testValue ) => curValue < testValue,
            };

            highestValue = 0;
            foreach( var line in GetInput() )
            {
                var parts = line.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

                // check the condition
                registers.TryGetValue( parts[4], out int curValue );

                if( testMethods[parts[5]]( curValue, int.Parse( parts[6] ) ) )
                {
                    registers.TryGetValue( parts[0], out curValue );
                    curValue += (parts[1] == "dec" ? -1 : +1) * int.Parse( parts[2] );

                    registers[parts[0]] = curValue;

                    highestValue = Math.Max( highestValue, curValue );
                }
            }

            return registers;
        }

        public string SolveAdvanced()
        {
            ProcessRegisters( out int highestValue );

            return highestValue.ToString();
        }
    }
}
