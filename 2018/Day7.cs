using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day7 : IAdventDay
    {
        public string Name => "7. 12. 2018";

        private static (char Prev, char Next)[] GetInput() => System.IO.File.ReadAllLines(@"2018/Resources/day7.txt").Select(s => (s[5], s[36])).ToArray();

        public string Solve()
        {
            var prerequisites = GetPrerequisites();
            var completed = new HashSet<char>();
            var output = new System.Text.StringBuilder();

            while (completed.Count < prerequisites.Count)
            {
                var open = prerequisites.Where(p => !completed.Contains(p.Key) && (!p.Value.Any() || p.Value.All(c => completed.Contains(c)))).OrderBy(p => p.Key);
                var current = open.First();
                output.Append(current.Key);
                completed.Add(current.Key);
            }

            return output.ToString();
        }

        private static Dictionary<char, HashSet<char>> GetPrerequisites()
        {
            var prerequisites = new Dictionary<char, HashSet<char>>();

            foreach (var input in GetInput())
            {
                if (!prerequisites.ContainsKey(input.Prev))
                {
                    prerequisites[input.Prev] = new HashSet<char>();
                }

                if (!prerequisites.ContainsKey(input.Next))
                {
                    prerequisites[input.Next] = new HashSet<char>();
                }

                prerequisites[input.Next].Add(input.Prev);
            }

            return prerequisites;
        }

        public string SolveAdvanced()
        {
            var prerequisites = GetPrerequisites();
            var completed = new HashSet<char>();
            var inProgress = new HashSet<char>();

            const int workerCount = 5;
            var workers = new char[workerCount];
            var jobFinish = new int[workerCount];
            int step = 0;

            for (int i= 0; i < workerCount;i++)
            {
                workers[i] = '\0';
                jobFinish[i] = int.MaxValue;
            }

                while (completed.Count < prerequisites.Count)
                {
                    var open = new HashSet<char>(prerequisites.Where(p => !completed.Contains(p.Key) && !inProgress.Contains(p.Key)
                         && (!p.Value.Any() || p.Value.All(c => completed.Contains(c)))).Select(p => p.Key));

                    // assign jobs
                    for (int i = 0; i < workerCount; i++)
                    {
                        if (!open.Any())
                            break;

                        if (workers[i] != 0)
                            continue;

                        var job = open.OrderBy(c => c).First();
                        workers[i] = job;
                        jobFinish[i] = step + GetDuration(job);
                        open.Remove(job);
                        inProgress.Add(job);
                    }

                    // move forward
                    step = jobFinish.Min();

                    // finish jobs
                    for (int i = 0; i < workerCount; i++)
                    {
                        if (jobFinish[i] != step)
                            continue;

                        inProgress.Remove(workers[i]);
                        completed.Add(workers[i]);
                        workers[i] = (char)0;
                        jobFinish[i] = int.MaxValue;
                    }
                }

            return step.ToString();

            int GetDuration(char c) => c - 'A' + 60 + 1;
        }
    }
}