namespace AdventOfCode.Benchmarks;

using AdventOfCode.Common;
using System;

internal class NullFormatter : IResultFormatter
{
    public static readonly NullFormatter Instance = new();

    private NullFormatter()
    {
    }

    public void Format<T>(T result)
    {
        // NOP
        return;
    }
}
