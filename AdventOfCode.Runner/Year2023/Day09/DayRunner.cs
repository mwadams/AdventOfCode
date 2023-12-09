namespace AdventOfCode.Runner.Year2023.Day09
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
            var lines = await InputReader.ReadLines(2023, 9, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result =
                AccumulateResult(
                    static (result, diffs, lineCount, diffCount) =>
                    {
                        int currentLength = lineCount - diffCount + 1;
                        int diffOffset = diffs.Length - 1;
                        long current = 0;

                        while (diffOffset >= 0)
                        {
                            current = diffs[diffOffset] + current;
                            diffOffset -= currentLength;
                            currentLength += 1;
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
                    static (result, diffs, lineCount, diffCount) =>
                    {
                        int currentLength = lineCount - diffCount + 1;
                        int diffOffset = diffs.Length - currentLength;

                        long current = 0;

                        while (diffOffset >= 0)
                        {
                            current = diffs[diffOffset] - current;
                            currentLength += 1;
                            diffOffset -= currentLength;
                        }

                        return result + current;
                    });

            formatter.Format(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AreNotSame(ReadOnlySpan<long> currentNumbers)
        {
            long firstNum = currentNumbers[0];
            for (int i = 1; i < currentNumbers.Length; ++i)
            {
                if (currentNumbers[i] != firstNum)
                {
                    return true;
                }
            }

            return false;
        }

        delegate long DiffMunger(long result, ReadOnlySpan<long> diffs, int numberCount, int diffCount);

        private long AccumulateResult(DiffMunger diffMunger)
        {
            long result = 0;

            foreach (ReadOnlySpan<char> line in lines)
            {
                result = ProcessLine(diffMunger, result, line);
            }

            return result;
        }

        private static long ProcessLine(DiffMunger diffMunger, long result, ReadOnlySpan<char> line)
        {
            Span<long> numberBuffer = stackalloc long[21];
            int numberCount = ParseLine(line, numberBuffer);

            Span<long> currentNumbers = numberBuffer[..numberCount];

            int totalLength = ((currentNumbers.Length + 1) * (currentNumbers.Length + 1) / 2) + 1;
            Span<long> diffs = stackalloc long[totalLength];
            int diffOffset = 0;
            int diffCount = 1;
            currentNumbers.CopyTo(diffs[diffOffset..]);
            int currentLength = currentNumbers.Length;
            while (AreNotSame(currentNumbers))
            {
                Span<long> lastDiff = currentNumbers;
                diffOffset += currentLength;
                currentLength--;
                currentNumbers = diffs.Slice(diffOffset, currentLength);
                for (int i = 0; i < lastDiff.Length - 1; i++)
                {
                    currentNumbers[i] = lastDiff[i + 1] - lastDiff[i];
                }

                diffCount++;
            }

            result = diffMunger(result, diffs[..(diffOffset + currentLength)], numberCount, diffCount);
            return result;
        }

        private static int ParseLine(ReadOnlySpan<char> line, Span<long> result)
        {
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

            return index;
        }
    }
}
