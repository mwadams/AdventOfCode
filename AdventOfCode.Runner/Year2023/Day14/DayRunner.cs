namespace AdventOfCode.Runner.Year2023.Day14
{
    using AdventOfCode.Common;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
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
            int result = 0;

            Span<Tile> map = stackalloc Tile[lines.Length * lines[0].Length];

            BuildMap(lines, map);

            result = TiltNorthAndCalculateLoad(map, lines[0].Length, lines.Length);

            Dump(map, lines[0].Length, lines.Length);

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            int result = 0;

            Span<Tile> map = stackalloc Tile[lines.Length * lines[0].Length];

            BuildMap(lines, map);

            HashSet<int> seenConfiguration = [];

            bool cycled = false;

            for (int i = 0; i < 1000000000; ++i)
            {
                TiltNorthAndCalculateLoad(map, lines[0].Length, lines.Length);
                Dump(map, lines[0].Length, lines.Length);
                TiltWestAndCalculateLoad(map, lines[0].Length, lines.Length);
                Dump(map, lines[0].Length, lines.Length);
                TiltSouthAndCalculateLoad(map, lines[0].Length, lines.Length);
                Dump(map, lines[0].Length, lines.Length);
                result = TiltEastAndCalculateLoad(map, lines[0].Length, lines.Length);
                Dump(map, lines[0].Length, lines.Length);

                if (!seenConfiguration.Add(result))
                {
                    if (!cycled)
                    {
                        cycled = true;
                        Console.WriteLine($"Cycled after: {i} iterations.");
                    }
                }

                Console.WriteLine(result);
                if (cycled)
                {
                    Console.ReadKey();
                }
            }

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

            Console.WriteLine();
        }

        private int TiltNorthAndCalculateLoad(Span<Tile> map, int width, int height)
        {
            int load = 0;
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

        private int TiltSouthAndCalculateLoad(Span<Tile> map, int width, int height)
        {
            int load = 0;
            Span<int> targetYOffset = stackalloc int[width];
            targetYOffset.Fill(height - 1);

            for (int y = height - 1; y >= 0; y--)
            {
                int yOffset = y * width;
                for (int x = 0; x < width; x++)
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

                        targetYOffset[x] -= 1;
                        load += height - targetY;
                    }
                    else if (tile == Tile.Cube)
                    {
                        targetYOffset[x] = y - 1;
                    }
                }
            }

            return load;
        }

        private int TiltWestAndCalculateLoad(Span<Tile> map, int width, int height)
        {
            int load = 0;
            Span<int> targetXOffset = stackalloc int[width];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int offset = y * width + x;
                    Tile tile = map[offset];
                    if (tile == Tile.Sphere)
                    {
                        int targetX = targetXOffset[y];
                        if (targetX != x)
                        {
                            map[offset] = Tile.Empty;
                            map[y * width + targetX] = Tile.Sphere;
                        }

                        targetXOffset[y] += 1;
                        load += height - y;
                    }
                    else if (tile == Tile.Cube)
                    {
                        targetXOffset[y] = x + 1;
                    }
                }
            }

            return load;
        }

        private int TiltEastAndCalculateLoad(Span<Tile> map, int width, int height)
        {
            int load = 0;
            Span<int> targetXOffset = stackalloc int[width];
            targetXOffset.Fill(width - 1);

            for (int x = width - 1; x >= 0; x--)
            {
                for (int y = 0; y < height; y++)
                {
                    int offset = y * width + x;
                    Tile tile = map[offset];
                    if (tile == Tile.Sphere)
                    {
                        int targetX = targetXOffset[y];
                        if (targetX != x)
                        {
                            map[offset] = Tile.Empty;
                            map[y * width + targetX] = Tile.Sphere;
                        }

                        targetXOffset[y] -= 1;
                        load += height - y;
                    }
                    else if (tile == Tile.Cube)
                    {
                        targetXOffset[y] = x - 1;
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

        private enum Tile : byte
        {
            Empty,
            Cube,
            Sphere,
        }

    }
}
