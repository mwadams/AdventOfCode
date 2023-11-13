namespace AdventOfCode.Common
{
    /// <summary>
    /// Interface for the day challenges.
    /// </summary>
    /// <typeparam name="T">The type implementing the day.</typeparam>
    public interface IDay<T>
        where T : notnull, IDay<T>
    {
        static abstract ValueTask<T> Initialize(bool test);

        void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter;

        void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter;
    }
}