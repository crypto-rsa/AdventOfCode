using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day23 : IAdventDay
    {
        private class Cup
        {
            public Cup(int value)
            {
                Value = value;
            }

            public int Value { get; }

            public Cup Next { get; set; }

            public Cup Prev { get; set; }

            public void AttachAfter(Cup other)
            {
                other.Next = this;
                Prev = other;
            }
        }

        public string Name => "23. 12. 2020";

        private static string Input => "167248359";

        public string Solve()
        {
            var cache = new Dictionary<int, Cup>();
            var head = Initialize(Input.Select(c => int.Parse(c.ToString())), cache);
            int highest = Input.Length;

            for (int i = 0; i < 100; i++)
            {
                head = Rearrange(head, highest, cache);
            }

            head = cache[1];

            var output = new System.Text.StringBuilder();
            var current = head.Next;

            while (current != head)
            {
                output.Append(current.Value);
                current = current.Next;
            }

            return output.ToString();
        }

        public string SolveAdvanced()
        {
            const int totalCount = 1_000_000;
            var values = Input.Select(c => int.Parse(c.ToString())).ToList();
            int highest = totalCount;

            values.AddRange(Enumerable.Range(Input.Length + 1, totalCount - Input.Length));

            var cache = new Dictionary<int, Cup>();
            var head = Initialize(values, cache);

            for (int i = 0; i < 10_000_000; i++)
            {
                head = Rearrange(head, highest, cache);
            }

            head = cache[1];

            return (((long)head.Next.Value) * head.Next.Next.Value).ToString();
        }

        private static Cup Initialize(IEnumerable<int> values, Dictionary<int, Cup> cache)
        {
            Cup head = null;
            Cup prev = null;

            foreach (int value in values)
            {
                var current = new Cup(value);

                if (head == null)
                {
                    head = current;
                }

                if (prev != null)
                {
                    current.AttachAfter(prev);
                }

                prev = current;

                if (cache != null)
                {
                    cache.Add(value, current);
                }
            }

            if (prev != null)
            {
                head.AttachAfter(prev);
            }

            return head;
        }

        private static Cup Rearrange(Cup head, int highest, Dictionary<int, Cup> cache)
        {
            var removed = new Cup[] { head.Next, head.Next.Next, head.Next.Next.Next };
            int destinationValue = head.Value - 1;

            if (destinationValue == 0)
            {
                destinationValue = highest;
            }

            while (destinationValue == removed[0].Value || destinationValue == removed[1].Value || destinationValue == removed[2].Value)
            {
                destinationValue--;

                if (destinationValue == 0)
                {
                    destinationValue = highest;
                }
            }

            var destination = cache[destinationValue];
            var afterDestination = destination.Next;
            var afterRemoved = removed[2].Next;

            afterRemoved.AttachAfter(head);
            removed[0].AttachAfter(destination);
            afterDestination.AttachAfter(removed[2]);

            return head.Next;
        }
    }
}
