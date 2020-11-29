using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2015
{
    public class Day7 : IAdventDay
    {
        private class Instruction
        {
            private string[] _inputs;

            private string _operator;
            public Instruction(string input)
            {
                const string valuePattern = @"^(\w+) -> (\w+)$";
                const string notPattern = @"^NOT (\w+) -> (\w+)$";
                const string binaryPattern = @"^(\w+) (\w+) (\w+) -> (\w+)$";

                bool result = ParseValue() || ParseNot() || ParseBinary();

                if (!result)
                    throw new System.InvalidOperationException();

                bool ParseValue()
                {
                    var match = Regex.Match(input, valuePattern);

                    if (!match.Success)
                        return false;

                    _inputs = new string[] { match.Groups[1].Value };
                    _operator = string.Empty;
                    Output = match.Groups[2].Value;

                    return true;
                }

                bool ParseNot()
                {
                    var match = Regex.Match(input, notPattern);

                    if (!match.Success)
                        return false;

                    _inputs = new string[] { match.Groups[1].Value };
                    _operator = "NOT";
                    Output = match.Groups[2].Value;

                    return true;
                }

                bool ParseBinary()
                {
                    var match = Regex.Match(input, binaryPattern);

                    if (!match.Success)
                        return false;

                    _inputs = new string[] { match.Groups[1].Value, match.Groups[3].Value };
                    _operator = match.Groups[2].Value;
                    Output = match.Groups[4].Value;

                    return true;
                }
            }

            public void Evaluate(Dictionary<string, int> values)
            {
                values[Output] = _operator switch
                {
                    "" => GetValue(_inputs[0]),
                    "NOT" => ~GetValue(_inputs[0]),
                    "AND" => GetValue(_inputs[0]) & GetValue(_inputs[1]),
                    "OR" => GetValue(_inputs[0]) | GetValue(_inputs[1]),
                    "LSHIFT" => GetValue(_inputs[0]) << GetValue(_inputs[1]),
                    "RSHIFT" => GetValue(_inputs[0]) >> GetValue(_inputs[1]),
                    _ => throw new System.InvalidOperationException(),
                };

                int GetValue(string s) => int.TryParse(s, out int v) ? v : values[s];
            }

            public IEnumerable<string> GetWireInputs() => _inputs.Where(s => !Regex.IsMatch(s, @"\d+"));

            public string Output { get; private set; }
        }

        public string Name => "7. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day7.txt");

        public string Solve() => SolveInternal(GetInput().Select(s => new Instruction(s)).ToList());

        public string SolveAdvanced()
        {
            int b = int.Parse(Solve());
            var instructions = GetInput().Select(s => new Instruction(s)).Where(i => i.Output != "b").ToList();

            instructions.Add(new Instruction($"{b} -> b"));

            return SolveInternal(instructions);
        }

        private string SolveInternal(List<Instruction> instructions)
        {
            var values = new Dictionary<string, int>();

            while (true)
            {
                var index = instructions.FindIndex(i => i.GetWireInputs().All(values.ContainsKey));

                if (index < 0)
                    break;

                instructions[index].Evaluate(values);

                instructions.RemoveAt(index);
            }

            return values["a"].ToString();
        }
    }
}
