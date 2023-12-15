namespace AdventOfCode.Runner.Year2023.Day15
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
            var lines = await InputReader.ReadLines(2023, 15, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            long current = 0;
            foreach(ReadOnlySpan<char> line in lines)
            {
                foreach(char c in line)
                {
                    if (c == ',')
                    {
                        result += current;
                        current = 0;
                        continue;
                    }

                    checked
                    {
                        current += c;
                        current *= 17;
                        current %= 256;
                    }
                }
            }

            result += current;

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            formatter.Format(result);
        }
    }
}
