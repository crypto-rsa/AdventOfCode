using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent_of_Code.Year2017
{
    public class Day19 : IAdventDay
    {
        private class Point
        {
            public Point( int row, int column )
            {
                Row = row;
                Column = column;
            }

            public string Name => $"[{Row}, {Column}]";

            public override int GetHashCode()
            {
                return Row.GetHashCode();
            }

            public override bool Equals( object obj )
            {
                var pt = obj as Point;
                return (pt.Row == Row && pt.Column == Column);
            }

            public int Row { get; }
            public int Column { get; }
        }

        private class Segment
        {
            public Segment( Point start, Point end )
            {
                Start = start;
                End = end;
            }

            public Point Start { get; }
            public Point End { get; }
            public string Name { get; set; } = string.Empty;
        }

        public string Name => "19. 12. 2017";

        private static string GetInput() => System.IO.File.ReadAllText("2017/Resources/day19.txt");

        public string Solve()
        {
            var lines = GetInput().Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries );

            var path = CreatePath( lines );

            var startPoint = path.Keys.Single( p => p.Row == 0 );
            var lastPoint = startPoint;
            var curSegment = path[startPoint][0];
            bool reversed = false;
            var sequence = new StringBuilder();

            while( curSegment != null )
            {
                if( curSegment.Name != null )
                {
                    sequence.Append( reversed ? new string( curSegment.Name.Reverse().ToArray() ) : curSegment.Name );
                }

                reversed = lastPoint.Equals( curSegment.End );
                var nextPoint = reversed ? curSegment.Start : curSegment.End;
                curSegment = path[nextPoint].FirstOrDefault( s => s != curSegment );
                lastPoint = nextPoint;
            }

            return sequence.ToString();
        }

        private Dictionary<Point, List<Segment>> CreatePath( string[] lines )
        {
            var path = new Dictionary<Point, List<Segment>>();

            FindHorizontalSegments( lines, path );
            FindVerticalSegments( lines, path );

            return path;
        }

        private static void FindVerticalSegments( string[] lines, Dictionary<Point, List<Segment>> path )
        {
            for( int column = 0; column < lines[0].Length; column++ )
            {
                Point startPoint = null;
                var name = new StringBuilder();

                for( int row = 0; row < lines.Length; row++ )
                {
                    char c = lines[row][column];
                    switch( c )
                    {
                        case ' ':
                            startPoint = null;
                            break;

                        case '-':
                            continue;

                        default:
                            if( char.IsUpper( c ) )
                            {
                                name.Append( c );
                            }

                            if( startPoint == null )
                            {
                                startPoint = new Point( row, column );
                                name = new StringBuilder( char.IsUpper( c ) ? c.ToString() : string.Empty );
                            }
                            else if( lines[row + 1][column] == ' ' )
                            {
                                AddSegment( path, new Segment( startPoint, new Point( row, column ) ) { Name = name.ToString() } );
                                startPoint = null;
                            }
                            break;
                    }
                }
            }
        }

        private static void FindHorizontalSegments( string[] lines, Dictionary<Point, List<Segment>> path )
        {
            for( int row = 0; row < lines.Length; row++ )
            {
                string line = lines[row];

                Point startPoint = null;
                var name = new StringBuilder();

                for( int column = 0; column < line.Length; column++ )
                {
                    char c = lines[row][column];

                    switch( c )
                    {
                        case ' ':
                            startPoint = null;
                            break;

                        case '|':
                            continue;

                        default:
                            if( char.IsUpper( c ) )
                            {
                                name.Append( c );
                            }

                            if( startPoint == null )
                            {
                                startPoint = new Point( row, column );
                                name = new StringBuilder( char.IsUpper( c ) ? c.ToString() : string.Empty );
                            }
                            else if( line[column + 1] == ' ' )
                            {
                                AddSegment( path, new Segment( startPoint, new Point( row, column ) ) { Name = name.ToString() } );
                                startPoint = null;
                            }
                            break;
                    }
                }
            }
        }

        private static void AddSegment( Dictionary<Point, List<Segment>> path, Segment segment )
        {
            if( !path.TryGetValue( segment.Start, out List<Segment> list ) )
            {
                list = new List<Segment>();
                path[segment.Start] = list;
            }

            list.Add( segment );

            if( !path.TryGetValue( segment.End, out list ) )
            {
                list = new List<Segment>();
                path[segment.End] = list;
            }

            list.Add( segment );
        }

        public string SolveAdvanced()
        {
            var lines = GetInput().Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries );
            var path = CreatePath( lines );

            var startPoint = path.Keys.Single( p => p.Row == 0 );
            var lastPoint = startPoint;
            var curSegment = path[startPoint][0];
            bool reversed = false;
            int length = 1;

            while( curSegment != null )
            {
                length += Math.Abs( curSegment.End.Row - curSegment.Start.Row ) + Math.Abs( curSegment.End.Column - curSegment.Start.Column );
                reversed = lastPoint.Equals( curSegment.End );
                var nextPoint = reversed ? curSegment.Start : curSegment.End;
                curSegment = path[nextPoint].FirstOrDefault( s => s != curSegment );
                lastPoint = nextPoint;
            }

            return length.ToString();
        }
    }
}
