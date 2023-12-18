namespace AdventOfCode.Runner.Year2023.Day18
{
    using AdventOfCode.Common;
    using System.Runtime.CompilerServices;
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
            var lines = await InputReader.ReadLines(2023, 18, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            Span<long> xCoords = stackalloc long[lines.Length + 1];
            Span<long> yCoords = stackalloc long[lines.Length + 1];

            result = (BuildCoordinatesPart1(lines, xCoords, yCoords) / 2L) + 1;
            result += ShoelaceFormula(xCoords, yCoords);

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            Span<long> xCoords = stackalloc long[lines.Length + 1];
            Span<long> yCoords = stackalloc long[lines.Length + 1];

            result = (BuildCoordinatesPart2(lines, xCoords, yCoords) / 2L) + 1;
            result += ShoelaceFormula(xCoords, yCoords);

            formatter.Format(result);
        }

        private static long ShoelaceFormula(ReadOnlySpan<long> x, ReadOnlySpan<long> y)
        {
            long area = 0;
            int j = x.Length - 1;
            for (int i = 0; i < x.Length; i++)
            {
                area += (x[j] + x[i]) * (y[j] - y[i]);
                j = i;
            }
            return Math.Abs(area / 2L);
        }

        private static long BuildCoordinatesPart1(ReadOnlySpan<string> lines, Span<long> xCoords, Span<long> yCoords)
        {
            int offset = 1; // We need a (0,0) to start with
            long count = 0;
            long x = 0;
            long y = 0;

            foreach (ReadOnlySpan<char> line in lines)
            {
                long distance = long.Parse(line.Slice(2, 2));
                int direction = line[0];
                count += Math.Abs(distance);
                switch (direction)
                {
                    case 'R':
                        x += distance;
                        break;
                    case 'D':
                        y += distance;
                        break;
                    case 'L':
                        x -= distance;
                        break;
                    case 'U':
                        y -= distance;
                        break;
                }

                xCoords[offset] = x;
                yCoords[offset] = y;
                offset++;
            }

            return count;
        }

        private static long BuildCoordinatesPart2(ReadOnlySpan<string> lines, Span<long> xCoords, Span<long> yCoords)
        {
            int offset = 1; // We need a (0,0) to start with
            long count = 0;
            long x = 0;
            long y = 0;

            foreach (ReadOnlySpan<char> line in lines)
            {
                long distance = long.Parse(line[(line.IndexOf('#') + 1)..^2], System.Globalization.NumberStyles.HexNumber);
                int direction = line[^2];
                count += Math.Abs(distance);
                switch (direction)
                {
                    case '0':
                        x += distance;
                        break;
                    case '1':
                        y += distance;
                        break;
                    case '2':
                        x -= distance;
                        break;
                    case '3':
                        y -= distance;
                        break;
                }

                xCoords[offset] = x;
                yCoords[offset] = y;
                offset++;
            }

            return count;
        }
    }
}
