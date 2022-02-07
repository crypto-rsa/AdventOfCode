using System.Collections.Generic;

namespace Tools;

/// <summary>
/// Contains extension methods for the <see cref="ISet{T}"/> interface
/// </summary>
public static class EnumerableExtensions
{
    #region Methods

    /// <summary>
    /// Adds all items to the given set
    /// </summary>
    /// <param name="set">The set to add to</param>
    /// <param name="items">The items to add</param>
    /// <typeparam name="T">The type of items in the set</typeparam>
    public static void AddRange<T>(this ISet<T> set, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            set.Add(item);
        }
    }

    #endregion
}
