namespace AdventOfCode.Runner.Year2023.Day11
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
            var lines = await InputReader.ReadLines(2023, 11, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            Span<(long X, long Y)> galaxyBuffer = stackalloc (long X, long Y)[lines.Length * lines[0].Length];

            int galaxyCount = FindGalaxies(lines, galaxyBuffer, 2);
            
            ReadOnlySpan<(long X, long Y)> galaxies = galaxyBuffer[..galaxyCount];
            Span<long> distances = stackalloc long[galaxies.Length * (galaxies.Length - 1) / 2];
            GetWeights(galaxies, distances);


            foreach(var distance in distances)
            {
                result += distance;
            }

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            Span<(long X, long Y)> galaxyBuffer = stackalloc (long X, long Y)[lines.Length * lines[0].Length];

            int galaxyCount = FindGalaxies(lines, galaxyBuffer, 1_000_000);

            ReadOnlySpan<(long X, long Y)> galaxies = galaxyBuffer[..galaxyCount];
            Span<long> distances = stackalloc long[galaxies.Length * (galaxies.Length - 1) / 2];
            GetWeights(galaxies, distances);


            foreach (var distance in distances)
            {
                result += distance;
            }

            formatter.Format(result);
        }

        private static int FindGalaxies(ReadOnlySpan<string> lines, Span<(long X, long Y)> galaxies, long expansion)
        {
            int galaxyCount = 0;
            Span<long> rowOffsets = stackalloc long[lines.Length];
            Span<long> columnOffsets = stackalloc long[lines[0].Length];
            Span<(int X, int Y)> initialGalaxies = stackalloc (int X, int Y)[galaxies.Length];

            for(int y = 0; y < lines.Length; y++)
            {
                ReadOnlySpan<char> line = lines[y];
                int index = line.IndexOf('#');
                if (index >= 0)
                {
                    while (true)
                    {
                        initialGalaxies[galaxyCount++] = (index, y);
                        columnOffsets[index] = 1;
                        int nextIndex = line[(index + 1)..].IndexOf('#');
                        if (nextIndex < 0)
                        {
                            break;
                        }

                        index += nextIndex + 1;
                    }

                    rowOffsets[y] = (y > 0 ? rowOffsets[y - 1] : 0);
                }
                else
                {
                    rowOffsets[y] = (y > 0 ? rowOffsets[y - 1] : 0) + (expansion - 1);
                }
            }

            // Now, accumulate those column offsets
            long columnAccumulator = 0;

            for(int x = 0; x < columnOffsets.Length; ++x)
            {
                if (columnOffsets[x] == 0)
                {
                    columnOffsets[x] = columnAccumulator + (expansion - 1);
                    columnAccumulator += (expansion - 1);
                }
                else
                {
                    columnOffsets[x] = columnAccumulator;
                }
            }

            // So now rowOffsets contain the value to add to the Y and columnOffsets contains the value to add to the X
            for(int i = 0; i < galaxyCount; i++)
            {
                var galaxy = initialGalaxies[i];
                galaxies[i] = (galaxy.X + columnOffsets[galaxy.X], galaxy.Y + rowOffsets[galaxy.Y]);
            }

            return galaxyCount;
        }

        // Betting on a travelling salesman problem next :)
        private static void GetWeights(ReadOnlySpan<(long X, long Y)> points, Span<long> weights)
        {
            int offset = 0;
            for(int i = 0; i < points.Length - 1; i++)
            {
                var first = points[i];
                for(int j = i + 1; j < points.Length; j++)
                {
                    var second = points[j];
                    // We are just doing a left/right/up/down walk so this is the distance travelled
                    weights[offset++] = Math.Abs(second.X - first.X) + Math.Abs(second.Y - first.Y);
                }
            }
        }
    }
}
