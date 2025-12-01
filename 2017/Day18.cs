using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day18 : IAdventDay
    {
        private enum State
        {
            Normal,
            Waiting,
            Terminated,
        }

        private abstract class ProgramBase
        {
            protected readonly Dictionary<string, long> _values = new Dictionary<string, long>();
            protected readonly List<string[]> _lines;
            protected int _ip;

            public State State { get; protected set; }

            protected ProgramBase( int id, List<string[]> instructions )
            {
                _values["p"] = id;
                _lines = instructions;
                _ip = 0;
                State = State.Normal;
            }

            public void Step()
            {
                if( _ip >= _lines.Count )
                {
                    State = State.Terminated;
                    return;
                }

                var instruction = _lines[_ip];

                ProcessInstruction( instruction );
            }

            protected virtual void ProcessInstruction( string[] instruction )
            {
                switch( instruction[0] )
                {
                    case "set":
                        _values[instruction[1]] = GetValue( instruction[2] );
                        _ip++;
                        State = State.Normal;
                        break;

                    case "add":
                        _values[instruction[1]] = GetValue( instruction[1] ) + GetValue( instruction[2] );
                        _ip++;
                        State = State.Normal;
                        break;

                    case "mul":
                        _values[instruction[1]] = GetValue( instruction[1] ) * GetValue( instruction[2] );
                        _ip++;
                        State = State.Normal;
                        break;

                    case "mod":
                        _values[instruction[1]] = GetValue( instruction[1] ) % GetValue( instruction[2] );
                        _ip++;
                        State = State.Normal;
                        break;

                    case "jgz":
                        if( GetValue( instruction[1] ) > 0 )
                            _ip += (int) GetValue( instruction[2] );
                        else
                            _ip++;
                        State = State.Normal;
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            private long GetValue( string input )
            {
                if( long.TryParse( input, out long val ) )
                    return val;

                _values.TryGetValue( input, out long registerVal );

                return registerVal;
            }
        }

        private class SoundProgram : ProgramBase
        {
            private readonly Dictionary<string, long> _sounds = new Dictionary<string, long>();

            public string Answer { get; private set; }

            public SoundProgram( int id, List<string[]> instructions )
                : base( id, instructions )
            {
            }

            protected override void ProcessInstruction( string[] instruction )
            {
                switch( instruction[0] )
                {
                    case "snd":
                        _values.TryGetValue( instruction[1], out long val );
                        _sounds[instruction[1]] = val;
                        _ip++;
                        break;

                    case "rcv":
                        if( _sounds.TryGetValue( instruction[1], out long sound ) && sound > 0 )
                        {
                            Answer = sound.ToString();
                            State = State.Terminated;
                        }
                        _ip++;
                        break;

                    default:
                        base.ProcessInstruction( instruction );
                        break;
                }
            }
        }

        private class DuetProgram : ProgramBase
        {
            private readonly Queue<long> _queue = new Queue<long>();

            public int SendCount { get; private set; }

            public DuetProgram( int id, List<string[]> instructions )
                : base( id, instructions )
            {
            }

            public void Receive( long value )
            {
                _values[_lines[_ip][1]] = value;
                _ip++;
            }

            public bool CandSendValue => _queue.Any();

            public long GetSendValue() => _queue.Dequeue();

            protected override void ProcessInstruction( string[] instruction )
            {
                switch( instruction[0] )
                {
                    case "snd":
                        _values.TryGetValue( instruction[1], out long val );
                        _queue.Enqueue( val );
                        _ip++;
                        SendCount++;
                        break;

                    case "rcv":
                        State = State.Waiting;
                        break;

                    default:
                        base.ProcessInstruction( instruction );
                        break;
                }
            }
        }

        public string Name => "18. 12. 2017";

        private static string GetInput() => System.IO.File.ReadAllText("2017/Resources/day18.txt");

        public string Solve()
        {
            var lines = GetInput().Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ).Select( s => s.Trim().Split( ' ' ) ).ToList();

            var program = new SoundProgram( 0, lines );

            while( program.State != State.Terminated )
            {
                program.Step();
            }

            return program.Answer;
        }

        public string SolveAdvanced()
        {
            var lines = GetInput().Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ).Select( s => s.Trim().Split( ' ' ) ).ToList();
            var programs = Enumerable.Range( 0, 2 ).Select( i => new DuetProgram( i, lines ) ).ToArray();

            while( !(programs[0].State == State.Terminated && programs[1].State == State.Terminated) &&
                !(programs[0].State == State.Waiting && programs[1].State == State.Waiting && !programs[0].CandSendValue && !programs[1].CandSendValue) )
            {
                programs[0].Step();
                programs[1].Step();

                for( int i = 0; i < 2; i++ )
                {
                    if( programs[i].State == State.Waiting && programs[1 - i].CandSendValue )
                    {
                        programs[i].Receive( programs[1 - i].GetSendValue() );
                    }
                }
            }

            return programs[1].SendCount.ToString();
        }
    }
}
