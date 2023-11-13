namespace AdventOfCode.Runner.Year2022.Day02
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
            var lines = await InputReader.ReadLines(2022, 2, test);
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

            formatter.Format(accumulator.TotalScore);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            ElfAccumulatorWithStrategy accumulator = new();

            foreach (var line in this.lines)
            {
                accumulator.ProcessLine(line);
            }

            formatter.Format(accumulator.TotalScore);
        }

        private ref struct ElfAccumulatorWithStrategy
        {
            private int lineNumber;

            public ElfAccumulatorWithStrategy()
            {
                this.TotalScore = 0;
                this.lineNumber = 0;
            }

            public int TotalScore { get; private set; }

            public void ProcessLine(string line)
            {
                this.lineNumber++;
                this.TotalScore += this.ScoreLine(line);
            }

            private readonly int ScoreLine(string line)
            {
                return line switch
                {
                    "A X" => 0 + 3, // Rock Lose => Scissors
                    "A Y" => 3 + 1, // Rock Draw => Rock
                    "A Z" => 6 + 2, // Rock Win => Paper
                    "B X" => 0 + 1, // Paper Lose => Rock
                    "B Y" => 3 + 2, // Paper Draw => Paper
                    "B Z" => 6 + 3, // Paper Win => Scissors
                    "C X" => 0 + 2, // Scissors Lose => Paper
                    "C Y" => 3 + 3, // Scissors Draw => Scissors
                    "C Z" => 6 + 1, // Scissors Win => Rock
                    _ => throw new InvalidOperationException($"Unexpected input on line {this.lineNumber}:{Environment.NewLine}{line}{Environment.NewLine}")
                };
            }
        }

        private ref struct ElfAccumulator
        {
            private int lineNumber;

            public ElfAccumulator()
            {
                this.TotalScore = 0;
            }

            public int TotalScore { get; private set; }

            public void ProcessLine(string line)
            {
                this.lineNumber++;
                this.TotalScore += this.ScoreLine(line);
            }

            private readonly int ScoreLine(string line)
            {
                return line switch
                {
                    "A X" => 1 + 3, // Rock Rock => tie
                    "A Y" => 2 + 6, // Rock Paper => win
                    "A Z" => 3 + 0, // Rock Scissors => loss
                    "B X" => 1 + 0, // Paper Rock => loss
                    "B Y" => 2 + 3, // Paper Paper => tie
                    "B Z" => 3 + 6, // Paper Scissors => win
                    "C X" => 1 + 6, // Scissors Rock => win
                    "C Y" => 2 + 0, // Scissors Paper => loss
                    "C Z" => 3 + 3, // Scissors Scissors => tie
                    _ => throw new InvalidOperationException($"Unexpected input on line {this.lineNumber}:{Environment.NewLine}{line}{Environment.NewLine}")
                };
            }
        }
    }
}
