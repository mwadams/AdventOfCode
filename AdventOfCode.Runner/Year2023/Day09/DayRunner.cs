namespace AdventOfCode.Runner.Year2023.Day09
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
            var lines = await InputReader.ReadLines(2023, 9, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result =
                AccumulateResult(
                    static (result, diffs) =>
                    {
                        long current = diffs[^1][^1];

                        for (var i = diffs.Count - 2; i >= 0; i--)
                        {
                            // Diff the diffs
                            current = diffs[i][^1] + current;
                        }

                        return result + current;
                    });

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result =
                AccumulateResult(
                    static (result, diffs) =>
                    {
                        long current = diffs[^1][0];

                        for (var i = diffs.Count - 2; i >= 0; i--)
                        {
                            // Diff the diffs
                            current = diffs[i][0] - current;
                        }

                        return result + current;
                    });

            formatter.Format(result);
        }

        private bool HasNonZeroes(ReadOnlySpan<long> currentNumbers)
        {
            foreach(long num in currentNumbers)
            {
                if (num != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private long AccumulateResult(Func<long, List<long[]>, long> diffMunger)
        {
            long result = 0;

            foreach (ReadOnlySpan<char> line in lines)
            {
                long[] currentNumbers = ParseLine(line);
                List<long[]> diffs = new(currentNumbers.Length) { currentNumbers };
                while (HasNonZeroes(currentNumbers))
                {
                    var lastDiff = currentNumbers;
                    currentNumbers = new long[currentNumbers.Length - 1];
                    for (int i = 0; i < lastDiff.Length - 1; i++)
                    {
                        currentNumbers[i] = lastDiff[i + 1] - lastDiff[i];
                    }

                    diffs.Add(currentNumbers);
                }

                result = diffMunger(result, diffs);
            }

            return result;
        }

        private long[] ParseLine(ReadOnlySpan<char> line)
        {
            Span<long> result = stackalloc long[line.Length / 2];

            int start = 0;
            int end = 1;
            int index = 0;
            while (end < line.Length)
            {
                if (line[end] == ' ')
                {
                    result[index++] = long.Parse(line[start..end]);
                    start = end + 1;
                    end = start + 1;
                }
                else
                {
                    end++;
                }
            }

            // And add the one at the end
            result[index++] = long.Parse(line[start..]);

            return result[..index].ToArray();
        }
    }
}
