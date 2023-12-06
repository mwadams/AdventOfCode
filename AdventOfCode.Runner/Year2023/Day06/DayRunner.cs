namespace AdventOfCode.Runner.Year2023.Day06
{
    using AdventOfCode.Common;
    using System.Buffers;
    using System.Threading.Tasks;

    public class DayRunner : IDay<DayRunner>
    {
        private static readonly SearchValues<char> integerRange = SearchValues.Create("0123456789");

        private readonly string[] lines;

        public DayRunner(string[] lines)
        {
            this.lines = lines;
        }

        public static async ValueTask<DayRunner> Initialize(bool test = false)
        {
            var lines = await InputReader.ReadLines(2023, 6, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 1;

            Span<int> times = stackalloc int[4];
            Span<int> distances = stackalloc int[4];
            int numberOfGames = ParseTimesAndDistances(times, distances);

            for(int i = 0; i < numberOfGames; ++i)
            {
                result *= CalculateRange(times[i], distances[i] + 1);
            }

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            (long duration, long distance) = ParseTimeAndDistance();
            long result = CalculateRange(duration, distance + 1);

            formatter.Format(result);
        }

        private (long duration, long distance) ParseTimeAndDistance()
        {
            return (ConcatenateToSpaceSeparatedLong(lines[0]), ConcatenateToSpaceSeparatedLong(lines[1]));
        }

        private long ConcatenateToSpaceSeparatedLong(ReadOnlySpan<char> line)
        {
            Span<char> accumulator = stackalloc char[line.Length];
            int currentIndex = line.IndexOfAny(integerRange);
            int count = 0;
            
            for(int i = currentIndex; i < line.Length; ++i)
            {
                char currentChar = line[currentIndex++];
                if (integerRange.Contains(currentChar))
                {
                    accumulator[count++] = currentChar;
                }
            }

            return long.Parse(accumulator[..count]);
        }

        private int ParseTimesAndDistances(Span<int> times, Span<int> distances)
        {
            int total = GetSpaceSeparatedInts(lines[0], times);
            GetSpaceSeparatedInts(lines[1], distances);
            return total;
        }

        private int GetSpaceSeparatedInts(string line, Span<int> integers)
        {
            int count = 0;
            ReadOnlySpan<char> currentLine = line;
            int currentIndex = currentLine.IndexOfAny(integerRange);
            while (true)
            {
                currentLine = currentLine[currentIndex..];
                int nextIndex = currentLine.IndexOf(' ');
                if (nextIndex >= 0)
                {
                    integers[count++] = int.Parse(currentLine[..nextIndex]);
                }
                else
                {
                    integers[count++] = int.Parse(currentLine);
                    break;
                }

                currentIndex = (nextIndex + 1) + currentLine[(nextIndex + 1)..].IndexOfAny(integerRange);
            }

            return count;
        }

        private long CalculateRange(long duration, long distance)
        {
            var resultB = QuadraticFormula(1, -duration, distance);

            long lower = (long)Math.Ceiling(resultB);
            long upper = duration - lower;

            return (upper - lower) + 1;
        }

        private double QuadraticFormula(double a, double b, double c)
        {
            var body = Math.Sqrt((b * b) - (4 * a * c));
            var twoa = (2 * a);

            // B is always negative, so we want the smaller of the two solutions
            // the other will always be greater than the maximum allowed time
            return (-b - body) / twoa;
        }
    }
}
