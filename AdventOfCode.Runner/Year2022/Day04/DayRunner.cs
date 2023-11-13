namespace AdventOfCode.Runner.Year2022.Day04
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
            var lines = await InputReader.ReadLines(2022, 4, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            ElfAccumulator accumulator = new();

            foreach (var line in this.lines)
            {
                accumulator.ProcessLine(line);
            }

            formatter.Format(accumulator.OverlapCount);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            ElfAccumulatorPartialOverlap accumulator = new();

            foreach (var line in this.lines)
            {
                accumulator.ProcessLine(line);
            }

            formatter.Format(accumulator.OverlapCount);
        }


        private ref struct ElfAccumulatorPartialOverlap
        {
            public ElfAccumulatorPartialOverlap()
            {
                this.OverlapCount = 0;
            }

            public int OverlapCount { get; private set; }

            public void ProcessLine(string line)
            {
                if (IsFullyContainedOverlap(line))
                {
                    this.OverlapCount++;
                }
            }

            private static bool IsFullyContainedOverlap(string line)
            {
                ReadOnlySpan<char> lineSpan = line.AsSpan();
                // There must be at least 1 digit, so we don't need to look in the first char
                int firstSeparator = lineSpan[1..].IndexOf('-') + 1;
                // There must be at least 1 digit, so we don't need to look in the first char after the separator
                int rangeSeparator = lineSpan[(firstSeparator + 2)..].IndexOf(',') + (firstSeparator + 2);
                // There must be at least 1 digit, so we don't need to look in the first char after the separator
                int secondSeparator = lineSpan[(rangeSeparator + 1)..].IndexOf('-') + (rangeSeparator + 1);

                int start1 = int.Parse(lineSpan[0..firstSeparator]);
                int end1 = int.Parse(lineSpan[(firstSeparator + 1)..rangeSeparator]);
                int start2 = int.Parse(lineSpan[(rangeSeparator + 1)..secondSeparator]);
                int end2 = int.Parse(lineSpan[(secondSeparator + 1)..]);

                // The constraint for "not overlapping", inverted
                return !(start1 > end2 || start2 > end1);
            }
        }

        private ref struct ElfAccumulator
        {
            public ElfAccumulator()
            {
                this.OverlapCount = 0;
            }

            public int OverlapCount { get; private set; }

            public void ProcessLine(string line)
            {
                if (IsFullyContainedOverlap(line))
                {
                    this.OverlapCount++;
                }
            }

            private static bool IsFullyContainedOverlap(string line)
            {
                ReadOnlySpan<char> lineSpan = line.AsSpan();
                // There must be at least 1 digit, so we don't need to look in the first char
                int firstSeparator = lineSpan[1..].IndexOf('-') + 1;
                // There must be at least 1 digit, so we don't need to look in the first char after the separator
                int rangeSeparator = lineSpan[(firstSeparator + 2)..].IndexOf(',') + (firstSeparator + 2);
                // There must be at least 1 digit, so we don't need to look in the first char after the separator
                int secondSeparator = lineSpan[(rangeSeparator + 1)..].IndexOf('-') + (rangeSeparator + 1);

                int start1 = int.Parse(lineSpan[0..firstSeparator]);
                int end1 = int.Parse(lineSpan[(firstSeparator + 1)..rangeSeparator]);
                int start2 = int.Parse(lineSpan[(rangeSeparator + 1)..secondSeparator]);
                int end2 = int.Parse(lineSpan[(secondSeparator + 1)..]);

                return
                    (start1 <= start2 && end1 >= end2) ||
                    (start2 <= start1 && end2 >= end1);
            }
        }
    }
}
