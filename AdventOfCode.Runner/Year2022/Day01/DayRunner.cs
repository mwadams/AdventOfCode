namespace AdventOfCode.Runner.Year2022.Day01
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
            var lines = await InputReader.ReadLines(2022, 1, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            int maxCount = 0;

            int currentCount = 0;

            foreach(string line in this.lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    maxCount = Math.Max(maxCount, currentCount);
                    currentCount = 0;
                }
                else
                {
                    currentCount += int.Parse(line);
                }
            }

            formatter.Format(maxCount);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            // We need the top 3.
            Span<Elf> elves = stackalloc Elf[3];

            ElfAccumulator accumulator = new(ref elves);

            foreach(var line in this.lines)
            {
                accumulator.ProcessLine(line);
            }

            accumulator.FinishProcessing();

            formatter.Format(TotalCalories(elves));

            /// <summary>
            /// Calculates the total calorie count carried by a span of elves.
            /// </summary>
            static int TotalCalories(in Span<Elf> elves)
            {
                int total = 0;
                foreach (var elf in elves)
                {
                    if (!elf.IsEmpty) // Belt and braces check; the calorie count will actually be zero for empty records
                    {
                        total += elf.CalorieCount;
                    }
                }

                return total;
            }
        }

        /// <summary>
        /// Accumulates the top N elves by calorie count.
        /// </summary>
        /// <remarks>
        /// You pass it each line of an elf calorie record file, using <see cref="ProcessLine(string)"/>.
        /// It builds a top-n set of <see cref="Elf"/> records, based on the number of calories each elf is carrying, in the
        /// <see cref="Span{Elf}"/> you provide. This will be ordered by count, from lowest to highest.
        /// <para>
        /// Note that if there are fewer elf records in the input file than the number of slots available in the
        /// output span, then the slots at at the lower indices will be <see cref="Elf.IsEmpty"/>
        /// </para>
        /// </remarks>
        private ref struct ElfAccumulator
        {
            private int currentLine;
            private int startLineOfCurrentElf;
            private int currentElf;
            private int currentCalorieCount;

            private readonly Span<Elf> elves;

            public ElfAccumulator(ref Span<Elf> elves)
            {
                this.currentLine = 1;
                this.startLineOfCurrentElf = 1;
                this.currentElf = 0;
                this.currentCalorieCount = 0;
                this.elves = elves;
            }

            public void ProcessLine(string line)
            {
                if (string.IsNullOrEmpty(line))
                {
                    this.TryInsertLastElf();

                    this.currentCalorieCount = 0;
                    this.currentElf++;


                    // The "next" elf starts on the current line + 1.
                    this.startLineOfCurrentElf = this.currentLine + 1;
                }
                else
                {
                    if (int.TryParse(line, out int result))
                    {
                        this.currentCalorieCount += result;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Error processing line: {this.currentLine}. Unable to parse '{line}'");
                    }
                }

                this.currentLine++;
            }

            /// <summary>
            /// Complete processing the file.
            /// </summary>
            /// <remarks>
            /// This will ensure the last record is processed.
            /// </remarks>
            public void FinishProcessing()
            {
                if (this.currentCalorieCount > 0)
                {
                    this.TryInsertLastElf();
                    this.currentCalorieCount = 0;
                    this.currentElf++;
                }
            }

            private void TryInsertLastElf()
            {
                // We store the elves in sorted order
                int i = 0;
                // We can do a brute force iteration as we are expecting a low number of "top n" values
                // and brute force will be the most efficient.
                // We could switch to e.g. a binary search if elves.Length got very large
                for (; i < elves.Length; ++i)
                {
                    if (elves[i].CalorieCount > this.currentCalorieCount)
                    {
                        // We need to insert immediately before this item
                        break;
                    }
                }

                int indexAtWhichToInsert = i - 1;
                if (indexAtWhichToInsert >= 0)
                {
                    // This should be a very processor friendly copy; we're just blitting down the contents
                    // of the array
                    for (int index = 0; index < indexAtWhichToInsert; index++)
                    {
                        elves[index] = elves[index + 1];
                    }

                    elves[indexAtWhichToInsert] = new(this.currentCalorieCount, this.currentElf, this.startLineOfCurrentElf);
                }
            }
        }

        /// <summary>
        /// Holds the information about an elf.
        /// </summary>
        /// <param name="CalorieCount">The calorie count for the elf.</param>
        /// <param name="Index">The index of the elf in the order in which it appears in the input file.</param>
        /// <param name="StartLine">The start line of the elf's record in the input file.</param>
        private readonly record struct Elf(int CalorieCount, int Index, int StartLine)
        {
            /// <summary>
            /// Gets a value indicating whether this elf record is empty.
            /// </summary>
            public bool IsEmpty => this.StartLine == 0;
        }
    }
}
