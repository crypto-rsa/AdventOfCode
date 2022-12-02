using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools;

/// <summary>
/// Contains methods for parsing text input
/// </summary>
public static class InputParser
{
    #region Methods

    /// <summary>
    /// Splits the input string to individual lines
    /// </summary>
    /// <param name="input">The input to split</param>
    /// <returns>A collection of strings representing the individual lines</returns>
    public static IEnumerable<string> SplitToLines(this string input) => input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    /// Returns a collection of strings from the original input separated by a blank line
    /// </summary>
    /// <param name="input">The input to parse</param>
    /// <returns>A collection of collections of strings representing individual line blocks</returns>
    public static IEnumerable<IEnumerable<string>> ParseByBlankLines(this string input) => input
        .Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));

    /// <summary>
    /// Parses integers from individual lines
    /// </summary>
    /// <param name="lines">The lines to parse</param>
    /// <returns>A collection of integers parsed from the input lines</returns>
    public static IEnumerable<int> ParseIntegers(this IEnumerable<string> lines) => lines.Select(int.Parse);

    #endregion
}
