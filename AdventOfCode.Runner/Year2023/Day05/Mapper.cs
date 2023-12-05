namespace AdventOfCode.Runner.Year2023.Day05;


public readonly struct Mapper
{
    private readonly long destination;
    private readonly long source;
    private readonly long range;

    public Mapper(long destination, long source, long range)
    {
        this.destination = destination;
        this.source = source;
        this.range = range;
    }
    
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

    private long GetDestination(long input)
    {
        if (input >= source && input < source + range)
        {
            return (input - source) + destination;
        }

        return -1;
    }
}
