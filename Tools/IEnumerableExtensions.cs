using System.Collections.Generic;

namespace Tools;

public static class IEnumerableExtensions
{
    #region Methods

    public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
    {
        var queue = new Queue<T>();

        foreach (var item in source)
        {
            queue.Enqueue(item);
        }

        return queue;
    }

    #endregion
}
