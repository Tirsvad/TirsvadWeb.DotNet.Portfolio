namespace Portfolio.Infrastructure.Extensions;

/// <summary>
/// Extension methods for array manipulation, supporting removal of elements before or after a predicate match.
/// <para>
/// <b>Inheriting XML Documentation with &lt;inheritdoc/&gt;:</b>
/// <br/>
/// Use &lt;inheritdoc/&gt; in derived or implementing members to inherit documentation from base definitions. This ensures consistency and reduces duplication.
/// </para>
/// <para>
/// <b>How to use:</b>
/// <br/>
/// Place &lt;inheritdoc/&gt; in your method or property XML doc comment. The documentation from the base or interface will be automatically included.
/// </para>
/// <para>
/// <b>Why we have it:</b>
/// <br/>
/// It helps maintain DRY documentation, making code easier to maintain and understand, especially in large codebases with many overrides or implementations.
/// </para>
/// <example>
/// <code>
/// // Example usage of RemoveAfter and RemoveBefore
/// int[] numbers = { 1, 2, 3, 4, 5 };
/// int[] after = numbers.RemoveAfter(n => n == 3); // { 1, 2, 3 }
/// int[] before = numbers.RemoveBefore(n => n == 3); // { 4, 5 }
/// </code>
/// </example>
/// </summary>
public static class DbContextOptionsServicesExtensions
{
    /// <summary>
    /// Removes all elements after the first match of the predicate.
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T">Type of array element.</typeparam>
    /// <param name="source">Source array.</param>
    /// <param name="predicate">Predicate to match.</param>
    /// <param name="includeMatch">If true, includes the matched element; otherwise, excludes it.</param>
    /// <returns>New array with elements after the match removed.</returns>
    public static T[] RemoveAfter<T>(this T[] source, Func<T, bool> predicate, bool includeMatch = false)
    {
        // Validate input
        if (source is null || predicate is null)
            throw new ArgumentNullException(source is null ? nameof(source) : nameof(predicate));

        List<T> list = [.. source];
        int index = list.FindIndex(item => predicate(item));
        if (index < 0)
        {
            return source;
        }

        // Remove elements after the match
        if (includeMatch)
        {
            list.RemoveRange(index + 1, list.Count - index - 1);
        }
        else
        {
            list.RemoveRange(index, list.Count - (index));
        }

        return [.. list];
    }

    /// <summary>
    /// Removes all elements before the last match of the predicate.
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T">Type of array element.</typeparam>
    /// <param name="source">Source array.</param>
    /// <param name="predicate">Predicate to match.</param>
    /// <param name="includeMatch">If true, includes the matched element; otherwise, excludes it.</param>
    /// <returns>New array with elements before the match removed.</returns>
    public static T[] RemoveBefore<T>(this T[] source, Func<T, bool> predicate, bool includeMatch = false)
    {
        // Validate input
        if (source is null || predicate is null)
            throw new ArgumentNullException(source is null ? nameof(source) : nameof(predicate));

        List<T> list = [.. source];
        int index = list.FindLastIndex(item => predicate(item));
        if (index < 0)
        {
            return source;
        }

        // Remove elements before the match
        if (includeMatch)
        {
            list.RemoveRange(0, index);
        }
        else
        {
            list.RemoveRange(0, index + 1);
        }

        return list.ToArray();
    }
}
