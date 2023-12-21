namespace AdventOfCode.Runner.Year2023.Day21
{
    using AdventOfCode.Common;
    using System.Buffers;

    public class DayRunner : IDay<DayRunner>
    {
        private readonly string[] lines;
        private bool test;

        public DayRunner(string[] lines, bool test)
        {
            this.lines = lines;
            this.test = test;
        }

        public static async ValueTask<DayRunner> Initialize(bool test = false)
        {
            var lines = await InputReader.ReadLines(2023, 21, test);
            return new(lines, test);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            int width = lines[0].Length;
            int height = lines.Length;
            Span<Tile> map = stackalloc Tile[width * height];
            (int X, int Y, int Steps) startPosition = BuildMap(lines, map, width, height);

            result = Calculate(map, width, height, this.test ? 6 : 64, startPosition);

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            int width = lines[0].Length;
            int height = lines.Length;
            Span<Tile> map = stackalloc Tile[width * height];
            (int X, int Y, int Steps) startPosition = BuildMap(lines, map, width, height);

            int steps = (int)(26501365L % width);

            // Run on the 1x1 map
            long result1 = Calculate(map, width, height, steps, startPosition);

            // Expand to a 3x3 map
            long result3 = ExpandAndCalculate(3, map, width, height, startPosition, steps);

            // Expand to a 5x5 map
            long result5 = ExpandAndCalculate(5, map, width, height, startPosition, steps);

            result = ExtrapolateQuadratic(26501365L / width, result1, result3, result5);

            formatter.Format(result);
        }

        private long ExtrapolateQuadratic(long target, long value1, long value2, long value3)
        {
            // Should have Googled this rather than worked it out by hand :-)
            long c = value1;

            long a = (value3 + c - 2 * value2) / 2;

            long b = value2 - c - a;

            return (a * target * target) + (b * target) + c;
        }

        private static long Calculate(
            ReadOnlySpan<Tile> map,
            int width,
            int height,
            int steps,
            in (int X, int Y, int Steps) startPosition)
        {
            long result = 0;
            HashSet<(int X, int Y, int Steps)> visited = [];
            Queue<(int X, int Y, int Steps)> locations = new();
            locations.Enqueue(startPosition);

            while (locations.TryDequeue(out (int X, int Y, int Steps) current))
            {
                int north = current.Y - 1;
                int west = current.X - 1;
                int south = current.Y + 1;
                int east = current.X + 1;

                if (north >= 0 && map[(north * width) + current.X] == Tile.Garden)
                {
                    var newOne = (current.X, north, current.Steps + 1);
                    result += TryEnqueue(steps, locations, visited, newOne);
                }

                if (west >= 0 && map[(current.Y * width) + west] == Tile.Garden)
                {
                    var newOne = (west, current.Y, current.Steps + 1);
                    result += TryEnqueue(steps, locations, visited, newOne);
                }

                if (south < height && map[(south * width) + current.X] == Tile.Garden)
                {
                    var newOne = (current.X, south, current.Steps + 1);
                    result += TryEnqueue(steps, locations, visited, newOne);
                }

                if (east < width && map[(current.Y * width) + east] == Tile.Garden)
                {
                    var newOne = (east, current.Y, current.Steps + 1);
                    result += TryEnqueue(steps, locations, visited, newOne);
                }
            }

            return result;
        }

        private static long TryEnqueue(int steps, Queue<(int X, int Y, int Steps)> locations, HashSet<(int X, int Y, int Steps)> visited, (int X, int Y, int Steps) newOne)
        {
            if (!visited.Contains(newOne))
            {
                // Don't revisit it if we've already seen it at this number of steps
                visited.Add(newOne);
                if (newOne.Steps < steps)
                {
                    locations.Enqueue(newOne);
                }
                else
                {
                    return 1;
                }
            }

            return 0;
        }

        private static (int X, int Y, int Steps) BuildMap(string[] lines, Span<Tile> map, int width, int height)
        {
            (int X, int Y, int Steps) result = default;

            int offset = 0;
            for (int y = 0; y < lines.Length; y++)
            {
                ReadOnlySpan<char> line = (ReadOnlySpan<char>)lines[y];
                for (int x = 0; x < line.Length; ++x)
                {

                    switch (line[x])
                    {
                        case '.':
                            map[offset++] = Tile.Garden;
                            break;
                        case '#':
                            map[offset++] = Tile.Rock;
                            break;
                        case 'S':
                            map[offset++] = Tile.Garden;
                            result = (x, y, 0);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            return result;
        }

        private static long ExpandAndCalculate(
            int multiplier,
            ReadOnlySpan<Tile> map,
            int width,
            int height,
            in (int X, int Y, int Steps) startPosition,
            int steps)
        {
            Tile[] expandedMapBuffer = ArrayPool<Tile>.Shared.Rent(width * height * multiplier * multiplier);
            Span<Tile> expandedMap = expandedMapBuffer.AsSpan()[..(width * height * multiplier * multiplier)];

            try
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        Tile tile = map[(y * width) + x];

                        for (int yPrime = 0; yPrime < multiplier; ++yPrime)
                        {
                            int offsetY = (y * width * multiplier) + (yPrime * width * multiplier * height);

                            for (int xPrime = 0; xPrime < multiplier; ++xPrime)
                            {
                                int offsetX = x + (width * xPrime);
                                expandedMap[offsetY + offsetX] = tile;
                            }
                        }
                    }
                }

                int offsetToCentre = width * (multiplier / 2);

                return Calculate(
                    expandedMap,
                    width * multiplier,
                    height * multiplier,
                    steps + offsetToCentre,
                    (startPosition.X + offsetToCentre, startPosition.Y + offsetToCentre, 0));
            }
            finally
            {
                ArrayPool<Tile>.Shared.Return(expandedMapBuffer);
            }
        }

        private static void Dump(Span<Tile> expandedMap, int width, int height)
        {
            for(int y = 0; y < height; ++y)
            {
                for(int x = 0; x < width; ++x)
                {
                    if (expandedMap[(y * width) + x] == Tile.Garden)
                    {
                        Console.Write('.');
                    }
                    else
                    {
                        Console.Write('#');
                    }
                }

                Console.WriteLine();
            }
        }

        private enum Tile
        {
            Garden,
            Rock
        }
    }
}
