namespace AdventOfCode.Runner.Year2023.Day14
{
    using AdventOfCode.Common;
    using System.Threading.Tasks;

    public class DayRunner : IDay<DayRunner>
    {
        private readonly string[] lines;

        public DayRunner(string[] lines)
        {
            this.lines = lines;
        }

        public static async ValueTask<DayRunner> Initialize(bool test = false)
        {
            var lines = await InputReader.ReadLines(2023, 14, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            Span<Tile> map = stackalloc Tile[lines.Length * lines[0].Length];

            BuildMap(lines, map);

            result = TiltNorthAndCalculateLoad(map, lines[0].Length, lines.Length);

            Dump(map, lines[0].Length, lines.Length);

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            formatter.Format(result);
        }

        private void Dump(Span<Tile> map, int width, int height)
        {
            for(int y = 0; y < height; ++y)
            {
                int yOffset = y * height;
                for(int x = 0; x < width; ++x)
                {
                    Console.Write(map[yOffset + x] switch
                    {
                        Tile.Empty => '.',
                        Tile.Cube => '#',
                        Tile.Sphere => 'O',
                        _ => throw new NotImplementedException("Unknown tile"),
                    });
                }

                Console.WriteLine();
            }
        }

        private long TiltNorthAndCalculateLoad(Span<Tile> map, int width, int height)
        {
            long load = 0;
            Span<int> targetYOffset = stackalloc int[width];
            for(int y = 0; y < height; y++)
            {
                int yOffset = y * width;
                for(int x = 0; x < width; x++)
                {
                    int offset = yOffset + x;
                    Tile tile = map[offset];
                    if (tile == Tile.Sphere)
                    {
                        int targetY = targetYOffset[x];
                        if (targetY != y)
                        {
                            map[offset] = Tile.Empty;
                            map[targetY * width + x] = Tile.Sphere;
                        }

                        targetYOffset[x] += 1;
                        load += height - targetY;
                    }
                    else if (tile == Tile.Cube)
                    {
                        targetYOffset[x] = y + 1;
                    }
                }
            }

            return load;
        }

        private static void BuildMap(ReadOnlySpan<string> lines, Span<Tile> map)
        {
            int offset = 0;

            foreach(ReadOnlySpan<char> line in lines)
            {
                foreach(char tile in line)
                {
                    map[offset++] = tile switch
                    {
                        '.' => Tile.Empty,
                        '#' => Tile.Cube,
                        'O' => Tile.Sphere,
                        _ => throw new NotImplementedException("Unknown tile"),
                    };
                }
            }
        }

        private enum Tile
        {
            Empty,
            Cube,
            Sphere,
        }

    }
}
