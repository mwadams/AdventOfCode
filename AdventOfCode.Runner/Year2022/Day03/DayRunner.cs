namespace AdventOfCode.Runner.Year2022.Day03
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
            var lines = await InputReader.ReadLines(2022, 3, test);
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

            formatter.Format(accumulator.PrioritySum);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            Span<int> buffer1 = stackalloc int[52];
            Span<int> buffer2 = stackalloc int[52];

            ElfBadgeAccumulator accumulator = new(ref buffer1, ref buffer2);

            foreach (var line in this.lines)
            {
                accumulator.ProcessLine(line);
            }

            formatter.Format(accumulator.PrioritySum);
        }

        private ref struct ElfAccumulator
        {
            private int lineNumber;

            public ElfAccumulator()
            {
                this.PrioritySum = 0;
            }

            public int PrioritySum { get; private set; }

            public void ProcessLine(string line)
            {
                this.lineNumber++;
                this.PrioritySum += this.ScoreLine(line);
            }

            private readonly int ScoreLine(string line)
            {
                int halfLineLength = line.Length / 2;

                if (line.Length % 2 != 0)
                {
                    throw new InvalidOperationException($"Line {this.lineNumber} did not have the correct number of entries.");
                }

                for (int i = 0; i < halfLineLength; ++i)
                {
                    for (int j = halfLineLength; j < line.Length; ++j)
                    {
                        if (line[i] == line[j])
                        {
                            // We can leave immediately as there is only 1 error per line
                            return ScoreChar(line[i]);
                        }
                    }
                }

                throw new InvalidOperationException($"Line {this.lineNumber} had no mispacked items.");
            }

            private static int ScoreChar(char v)
            {
                if (v >= 'a' && v <= 'z')
                {
                    return ((int)v - (int)'a' + 1);
                }

                return ((int)v - (int)'A' + 27);
            }
        }


        private ref struct ElfBadgeAccumulator
        {
            private int lineNumber;
            private string previousLine;
            private readonly Span<int> candidateBuffer1;
            private readonly Span<int> candidateBuffer2;

            public ElfBadgeAccumulator(ref Span<int> candidateBuffer1, ref Span<int> candidateBuffer2)
            {
                if (candidateBuffer1.Length < 52)
                {
                    throw new ArgumentException("Buffer 1 must be at least 52 ints long.", nameof(candidateBuffer1));
                }

                if (candidateBuffer2.Length < 52)
                {
                    throw new ArgumentException("Buffer 1 must be at least 52 ints long.", nameof(candidateBuffer1));
                }

                this.PrioritySum = 0;
                this.lineNumber = 0;
                this.previousLine = string.Empty;
                this.candidateBuffer1 = candidateBuffer1[..52]; // Slice off 52 ints
                this.candidateBuffer2 = candidateBuffer2[..52]; // Slice off 52 ints
            }

            public int PrioritySum { get; private set; }

            public void ProcessLine(string line)
            {
                int lineIndex = this.lineNumber % 3;

                this.lineNumber++;

                switch (lineIndex)
                {
                    case 0:
                        // This is the first of a group of 3, so reset them all to zeros
                        this.candidateBuffer1.Clear();
                        this.candidateBuffer2.Clear();
                        this.previousLine = line;
                        break;
                    case 1:
                        // This is the second, so compare with the first
                        this.FindCandidateMatch(line, candidateBuffer1);
                        this.previousLine = line;
                        break;
                    case 2:
                        // This is the third, so compare with the second
                        this.FindCandidateMatch(line, candidateBuffer2);
                        this.PrioritySum += this.FindChar();
                        break;
                }
            }

            private static int ScoreChar(char v)
            {
                if (v >= 'a' && v <= 'z')
                {
                    return ((int)v - (int)'a' + 1);
                }

                return ((int)v - (int)'A' + 27);
            }

            private readonly void FindCandidateMatch(string line, Span<int> candidateBuffer)
            {
                for (int i = 0; i < line.Length; ++i)
                {
                    for (int j = 0; j < this.previousLine.Length; ++j)
                    {
                        if (line[i] == this.previousLine[j])
                        {
                            int value = ScoreChar(line[i]);

                            // Stash the value in our candidate buffer
                            // We avoid having to do any calculation later
                            // and we know the size of the whole output space.
                            candidateBuffer[value - 1] = value;
                        }
                    }
                }
            }

            private readonly int FindChar()
            {
                for (int i = 0; i < this.candidateBuffer1.Length; ++i)
                {
                    if (this.candidateBuffer1[i] != 0 && this.candidateBuffer2[i] != 0)
                    {
                        return this.candidateBuffer1[i];
                    }
                }

                throw new InvalidOperationException("No candidates found.");
            }
        }
    }
}
