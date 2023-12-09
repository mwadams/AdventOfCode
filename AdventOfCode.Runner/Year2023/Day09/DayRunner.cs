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

        private bool HasNonZeroes(List<long> currentNumbers)
        {
            return currentNumbers.Any(v => v != 0);
        }

        private long AccumulateResult(Func<long, List<List<long>>, long> diffMunger)
        {
            long result = 0;

            foreach (ReadOnlySpan<char> line in lines)
            {
                List<long> currentNumbers = ParseLine(line);
                List<List<long>> diffs = new(currentNumbers.Count) { currentNumbers };
                while (HasNonZeroes(currentNumbers))
                {
                    var lastDiff = currentNumbers;
                    currentNumbers = [];
                    for (int i = 0; i < lastDiff.Count - 1; i++)
                    {
                        currentNumbers.Add(lastDiff[i + 1] - lastDiff[i]);
                    }

                    diffs.Add(currentNumbers);
                }

                result = diffMunger(result, diffs);
            }

            return result;
        }

        private List<long> ParseLine(ReadOnlySpan<char> line)
        {
            List<long> result = [];

            int start = 0;
            int end = 1;
            while (end < line.Length)
            {
                if (line[end] == ' ')
                {
                    result.Add(long.Parse(line[start..end]));
                    start = end + 1;
                    end = start + 1;
                }
                else
                {
                    end++;
                }
            }

            // And add the one at the end
            result.Add(long.Parse(line[start..]));

            return result;
        }
    }
}
