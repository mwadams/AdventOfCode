namespace AdventOfCode.Runner.Year2023.Day04
{
    using AdventOfCode.Common;
    using System;
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
            var lines = await InputReader.ReadLines(2023, 4, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            foreach(var line in lines)
            {
                int count = ParseLine(line);
                if (count > 0)
                {
                    result += 1L << (count - 1);
                }
            }

            formatter.Format(result);
        }
        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            Span<long> scores = stackalloc long[lines.Length];

            
            for(int i =0; i < lines.Length; ++i)
            {
                ReadOnlySpan<char> line = lines[i];

                int count = ParseLine(line);

                // You always have 1 copy of the card
                scores[i] += 1;

                if (count > 0)
                {
                    for(int j = 1; j <= count; ++j)
                    {
                        scores[i + j] += scores[i];
                    }
                }
            }

            foreach(int score in scores)
            {
                result += score;
            }

            formatter.Format(result);
        }

        private int ParseLine(ReadOnlySpan<char> line)
        {
            // Looking at the input, they are all 1 or 2 digit numbers
            int firstOffset = line.IndexOf(':') + 2;
            int secondOffset = line[firstOffset..].IndexOf('|') + firstOffset;

            Span<int> winners = stackalloc int[(secondOffset - firstOffset) / 3];

            Build(line, firstOffset, winners);

            int thirdOffset = secondOffset + 2;
            
            return CalculateWinCount(line, thirdOffset, winners);
        }

        private int CalculateWinCount(ReadOnlySpan<char> line, int thirdOffset, Span<int> winners)
        {
            int count = 0;
            int length = ((line.Length + 1) - thirdOffset) / 3;

            for (int i = 0; i < length; ++i)
            {
                int currentValue = int.Parse(line.Slice(thirdOffset + (i * 3), 2));
                if (winners.IndexOf(currentValue) >= 0)
                {
                    count += 1;
                }
            }

            return count;
        }

        private static void Build(ReadOnlySpan<char> line, int firstOffset, Span<int> winners)
        {
            for (int i = 0; i < winners.Length; ++i)
            {
                winners[i] = int.Parse(line.Slice(firstOffset + (i * 3), 2));
            }
        }
    }
}
