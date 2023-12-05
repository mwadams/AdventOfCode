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
        for(int i = 0; i < map.Length; i++)
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
