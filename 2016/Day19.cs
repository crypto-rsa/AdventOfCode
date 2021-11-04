using System.Collections.Generic;

namespace Advent_of_Code.Year2016
{
    public class Day19 : IAdventDay
    {
        public string Name => "19. 12. 2016";

        private const int Input = 3004953;

        private (LinkedList<int> List, LinkedListNode<int> Source, LinkedListNode<int> Target) GetList(int count, int targetIndex)
        {
            var list = new LinkedList<int>();
            var current = list.AddFirst(0);
            var source = current;
            var target = current;

            for (int i = 1; i < count; i++)
            {
                list.AddAfter(current!, i);

                if (i == targetIndex)
                {
                    target = list.Last;
                }

                current = list.Last;
            }

            return (list, source, target);
        }

        private static LinkedListNode<int> GetNext(LinkedListNode<int> source) => source.Next ?? source.List!.First;

        public string Solve()
        {
            var (list, source, target) = GetList(Input, 1);

            while (source != target)
            {
                list.Remove(target!);
                source = GetNext(source);
                target = GetNext(source);
            }

            return (source.Value + 1).ToString();
        }

        public string SolveAdvanced()
        {
            int count = Input;
            var (list, source, target) = GetList(count, count / 2);

            while (count > 1)
            {
                var nextTarget = count % 2 == 1 ? GetNext(GetNext(target)) : GetNext(target);

                list.Remove(target!);
                source = GetNext(source);
                target = nextTarget;
                count--;
            }

            return (source.Value + 1).ToString();
        }
    }
}
