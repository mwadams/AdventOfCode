namespace AdventOfCode.Runner.Year2023.Day01
{
    using AdventOfCode.Common;
    using System.Buffers;
    using System.Threading.Tasks;

    public class DayRunner : IDay<DayRunner>
    {
        private static readonly SearchValues<char> Digits = SearchValues.Create("1234567989");
        private static readonly (string Text, long Value)[] TextAndDigits =
            [
                ("0", 0),
                ("1", 1),
                ("2", 2),
                ("3", 3),
                ("4", 4),
                ("5", 5),
                ("6", 6),
                ("7", 7),
                ("8", 8),
                ("9", 9),
                ("one", 1),
                ("two", 2),
                ("six", 6),
                ("four", 4),
                ("five", 5),
                ("nine", 9),
                ("three", 3),
                ("seven", 7),
                ("eight", 8),
            ];
        private readonly string[] lines;

        public DayRunner(string[] lines)
        {
            this.lines = lines;
        }

        public static async ValueTask<DayRunner> Initialize(bool test = false)
        {
            var lines = await InputReader.ReadLines(2023, 1, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            Span<char> characters = stackalloc char[2];

            foreach (var line in lines)
            {
                ReadOnlySpan<char> lineSpan = line.AsSpan();
                int firstIndex = lineSpan.IndexOfAny(Digits);
                int lastIndex = lineSpan.LastIndexOfAny(Digits);

                if (firstIndex <= lastIndex)
                {
                    characters[0] = line[firstIndex];
                    characters[1] = line[lastIndex];
                    result += long.Parse(characters);
                }
            }

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            int lineIndex = 0;
            foreach (var line in lines)
            {
                lineIndex++;
                ReadOnlySpan<char> lineAsSpan = line.AsSpan();
                (long firstValue, int firstOffset) = First(lineAsSpan);
                if(firstValue >= 0 && firstOffset < lineAsSpan.Length)
                {
                    long lastValue = Last(lineAsSpan[firstOffset..]);
                    if (lastValue >= 0)
                    {
                        result += (firstValue * 10) + lastValue;
                    }
                }
            }

            formatter.Format(result);
        }

        private (long Value, int FirstOffset) First(ReadOnlySpan<char> line)
        {
            int index = line.Length - 1;
            long value = 0;
            bool found = false;

            foreach(var (text, digit) in TextAndDigits)
            {
                int currentIndex = line[..(index + 1)].IndexOf(text);
                if (currentIndex >= 0)
                {
                    found = true;
                    index = currentIndex;
                    value = digit;
                }

                if (index == 0)
                {
                    break;
                }
            }

            return (found ? value : -1, index);
        }

        private long Last(ReadOnlySpan<char> line)
        {
            int index = 0;
            long value = 0;
            bool found = false;

            foreach (var (text, digit) in TextAndDigits)
            {
                if (line.Length < index + text.Length)
                {
                    // We are sorted by text length so there's
                    // no point in looking further
                    break;
                }

                int currentIndex = line[index..].LastIndexOf(text);
                if (currentIndex >= 0)
                {
                    // To ensure overlaps work, we can only advance 1
                    index += currentIndex + 1;
                    value = digit;
                    found = true;
                }

                if (currentIndex == line.Length)
                {
                    break;
                }
            }

            return found ? value : -1;
        }
    }
}
