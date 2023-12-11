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
            // Eyeballing the results, there aren't more than 1/3 galaxies in a line
            Span<(int X, int Y)> galaxyBuffer = stackalloc (int X, int Y)[lines.Length * lines[0].Length];

            int galaxyCount = FindGalaxies(lines, galaxyBuffer);
            
            ReadOnlySpan<(int X, int Y)> galaxies = galaxyBuffer[..galaxyCount];
            Span<int> distances = stackalloc int[galaxies.Length * (galaxies.Length - 1) / 2];
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
            formatter.Format(result);
        }

        private static int FindGalaxies(ReadOnlySpan<string> lines, Span<(int X, int Y)> galaxies)
        {
            int galaxyCount = 0;
            Span<int> rowOffsets = stackalloc int[lines.Length];
            Span<int> columnOffsets = stackalloc int[lines[0].Length];

            for(int y = 0; y < lines.Length; y++)
            {
                ReadOnlySpan<char> line = lines[y];
                int index = line.IndexOf('#');
                if (index >= 0)
                {
                    while (true)
                    {
                        galaxies[galaxyCount++] = (index, y);
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
                    rowOffsets[y] = (y > 0 ? rowOffsets[y - 1] : 0) + 1;
                }
            }

            // Now, accumulate those column offsets
            int columnAccumulator = 0;

            for(int x = 0; x < columnOffsets.Length; ++x)
            {
                if (columnOffsets[x] == 0)
                {
                    columnOffsets[x] = columnAccumulator++;
                }
                else
                {
                    columnOffsets[x] = columnAccumulator;
                }
            }

            // So now rowOffsets contain the value to add to the Y and columnOffsets contains the value to add to the X
            for(int i = 0; i < galaxyCount; i++)
            {
                var galaxy = galaxies[i];
                galaxies[i] = (galaxy.X + columnOffsets[galaxy.X], galaxy.Y + rowOffsets[galaxy.Y]);
            }

            return galaxyCount;
        }

        // Betting on a travelling salesman problem next :)
        private static void GetWeights(ReadOnlySpan<(int X, int Y)> points, Span<int> weights)
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
