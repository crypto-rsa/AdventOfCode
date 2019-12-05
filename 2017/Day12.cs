using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day12 : IAdventDay
    {
        public string Name => "12. 12. 2017";

        private static string[] GetInput() => System.IO.File.ReadAllLines(@"2017/Resources/day12.txt");

        public string Solve()
        {
            var nodes = BuildGraph();

            return FindReachable( nodes, "0" ).Count.ToString();
        }

        private HashSet<string> FindReachable( Dictionary<string, HashSet<string>> nodes, string startNode )
        {
            var visited = new HashSet<string>();
            var open = new HashSet<string>() { startNode };

            while( open.Count > 0 )
            {
                string node = open.First();
                visited.Add( node );
                open.Remove( node );

                foreach( var neighbour in nodes[node] )
                {
                    if( !visited.Contains( neighbour ) )
                    {
                        open.Add( neighbour );
                    }
                }
            }

            return visited;
        }

        private Dictionary<string, HashSet<string>> BuildGraph()
        {
            var nodes = new Dictionary<string, HashSet<string>>();

            foreach( var line in GetInput() )
            {
                var parts = line.Trim().Split( new string[] { " <-> " }, StringSplitOptions.RemoveEmptyEntries );
                string node = parts[0];
                var neighbours = new HashSet<string>( parts[1].Trim().Split( new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries ) );

                AddNeighbours( nodes, node, neighbours.ToArray() );

                foreach( var neighbour in neighbours )
                {
                    AddNeighbours( nodes, neighbour, node );
                }
            }

            return nodes;
        }

        private void AddNeighbours( Dictionary<string, HashSet<string>> nodes, string node, params string[] neighbours )
        {
            if( !nodes.TryGetValue( node, out HashSet<string> existing ) )
            {
                existing = new HashSet<string>();
                nodes.Add( node, existing );
            }

            foreach( var neighbour in neighbours )
            {
                existing.Add( neighbour );
            }
        }

        public string SolveAdvanced()
        {
            var nodes = BuildGraph();

            int groups = 0;
            var allNodes = new HashSet<string>( nodes.Keys );

            while( allNodes.Any() )
            {
                foreach( var node in FindReachable( nodes, allNodes.First() ) )
                {
                    allNodes.Remove( node );
                }

                groups++;
            }

            return groups.ToString();
        }
    }
}
