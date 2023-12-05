namespace AdventOfCode.Runner.Year2023.Day05;


public readonly struct Mapper
{
    public Mapper(long destination, long source, long range)
    {
        Destination = destination;
        Source = source;
        Range = range;
    }

    public long Destination { get; }
    public long Source { get; }
    public long Range { get; }

    public static long GetDestination(ReadOnlySpan<Mapper> map, long input)
    {
        for (int i = 0; i < map.Length; i++)
        {
            Mapper mapper = map[i];
            var destination = mapper.GetDestination(input);
            if (destination >= 0)
            {
                return destination;
            }
        }

        return input;
    }

    public static int GetDestinationRange(ReadOnlySpan<Mapper> map, in (long Lower, long Range) input, Span<(long Lower, long Range)> output)
    {
        int count = 0;
        long lowest = input.Lower + input.Range;
        long highest = input.Lower - 1;

        for (int i = 0; i < map.Length; ++i)
        {
            Mapper mapper = map[i];
            (long lowerOverlap, long upperOverlap) = mapper.GetOverlap(input);
            if (lowerOverlap >= 0)
            {
                var lower = mapper.GetDestination(lowerOverlap);
                var upper = mapper.GetDestination(upperOverlap);

                output[count++] = (lower, (upper - lower) + 1);
                if (lowerOverlap < lowest)
                {
                    lowest = lowerOverlap;
                }
                if (upperOverlap > highest)
                {
                    highest = upperOverlap;
                }
            }
        }

        // Add the "outside the range" items that were not found anywhere

        if (lowest > input.Lower)
        {
            (long Lower, long Range) range = (input.Lower, lowest - input.Lower);
            output[count++] = range;
            if (highest < range.Lower + range.Range - 1)
            {
                highest = range.Lower + range.Range - 1;
            }
        }

        if (highest < input.Lower + input.Range - 1)
        {
            output[count++] = (highest + 1, (input.Lower + input.Range) - (highest + 1));
        }

        return count;
    }

    private (long lowerOverlap, long upperOverlap) GetOverlap((long Lower, long Range) input)
    {

        if (input.Lower < (this.Source + this.Range) && (input.Lower + input.Range -1) > this.Source)
        {
            long lowerOverlap = Math.Max(input.Lower, this.Source);
            long upperOverlap = Math.Min(input.Lower + input.Range - 1, this.Source + this.Range - 1);
            return (lowerOverlap, upperOverlap);
        }

        return (-1, -1);
    }

    public override string ToString()
    {
        return $"<{Destination}, {Source}, {Range}>";
    }

    private long GetDestination(long input)
    {
        if (input >= Source && input < Source + Range)
        {
            return (input - Source) + Destination;
        }

        return -1;
    }
}
