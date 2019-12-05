using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day13 : IAdventDay
    {
        public string Name => "13. 12. 2017";

        private static string[] GetInput() => System.IO.File.ReadAllLines(@"2017/Resources/day13.txt");

        public string Solve()
        {
            var ranges = new Dictionary<int, int>();

            foreach( var line in GetInput() )
            {
                var parts = line.Split( ':' );
                ranges.Add( int.Parse( parts[0].Trim() ), int.Parse( parts[1].Trim() ) );
            }

            var positions = ranges.Keys.ToDictionary( i => i, i => 0 );
            var directions = ranges.Keys.ToDictionary( i => i, i => +1 );
            int end = ranges.Keys.Max();

            int depth = -1;
            int severity = 0;
            for( int step = 0; step <= end; step++ )
            {
                depth++;

                if( positions.TryGetValue( depth, out int position ) && position == 0  )
                {
                    severity += depth * ranges[depth];
                }

                foreach( var scanner in ranges.Keys )
                {
                    positions[scanner] += directions[scanner];

                    if( positions[scanner] == 0 || positions[scanner] == ranges[scanner] - 1 )
                    {
                        directions[scanner] *= -1;
                    }
                }
            }

            return severity.ToString();
        }

        public string SolveAdvanced()
        {
            var ranges = new Dictionary<int, int>();

            foreach( var line in GetInput() )
            {
                var parts = line.Split( ':' );
                ranges.Add( int.Parse( parts[0].Trim() ), int.Parse( parts[1].Trim() ) );
            }

            for( int delay = 0; ; delay++ )
            {
                bool passes = true;
                foreach( var layer in ranges.Keys )
                {
                    int stepsBefore = layer + delay;
                    int c = ranges[layer] - 1;
                    var mod = stepsBefore % (2 * c);
                    int position = mod > c ? c - (mod - c) : mod;

                    if( position == 0 )
                    {
                        passes = false;
                        break;
                    }
                }

                if( passes )
                    return delay.ToString();
            }
        }
    }
}
