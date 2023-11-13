namespace AdventOfCode.Common;

/// <summary>
/// Implemented by types that can format a result to the output.
/// </summary>
public interface IResultFormatter
{
    /// <summary>
    /// Format that result to the output.
    /// </summary>
    /// <typeparam name="T">The type of the output.</typeparam>
    /// <param name="result">The result.</param>
    void Format<T>(T result);
}
