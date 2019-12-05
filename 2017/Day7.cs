using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2017
{
    public class Day7 : IAdventDay
    {
        private class Node
        {
            public Node( string name )
            {
                Name = name;
            }

            public string Name { get; }

            public int Weight { get; set; }

            public int TotalWeight => Weight + Children.Sum( n => n.TotalWeight );

            public Node Parent { get; set; }

            public List<Node> Children = new List<Node>();

            public bool HasBalancedChildren => Children.Select( n => n.TotalWeight ).Distinct().Count() == 1;
        }

        public string Name => "7. 12. 2017";

        private static string[] GetInput() => System.IO.File.ReadAllLines(@"2017/Resources/day7.txt");

        public string Solve()
        {
            return BuildTree().Single( item => item.Value.Parent == null ).Key;
        }

        private Dictionary<string, Node> BuildTree()
        {
            var lines = GetInput();
            var nodes = new Dictionary<string, Node>( lines.Length );

            foreach( var line in lines )
            {
                var match = Regex.Match( line, @"^ ?(\w+) \((\d+)\)( -> (.*))?$" );

                string name = match.Groups[1].Value;
                int weight = int.Parse( match.Groups[2].Value );

                if( !nodes.TryGetValue( name, out Node node ) )
                {
                    node = new Node( name );
                    nodes[name] = node;
                }

                node.Weight = weight;

                if( match.Groups.Count > 4 )
                {
                    var children = match.Groups[4].Value.Split( new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries );
                    foreach( var child in children )
                    {
                        if( !nodes.TryGetValue( child, out Node childNode ) )
                        {
                            childNode = new Node( child );
                            nodes[child] = childNode;
                        }

                        childNode.Parent = node;
                        node.Children.Add( childNode );
                    }
                }
            }

            return nodes;
        }

        public string SolveAdvanced()
        {
            var root = BuildTree().Values.Single( n => n.Parent == null );
            Node node = root;

            while( !node.HasBalancedChildren )
            {
                // find the unbalanced child
                int majorityWeight = node.Children.GroupBy( n => n.TotalWeight ).OrderByDescending( g => g.Count() ).First().Key;
                node = node.Children.Find( n => n.TotalWeight != majorityWeight );
            }

            var sibling = node.Parent.Children.First( n => n != node );
            int difference = node.Weight - (node.TotalWeight - sibling.TotalWeight);

            return difference.ToString();
        }
    }
}
