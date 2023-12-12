namespace AdventOfCode.Runner.Year2023.Day12
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
            var lines = await InputReader.ReadLines(2023, 12, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            foreach (ReadOnlySpan<char> line in lines)
            {
                result += ProcessLine(line);
            }

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            formatter.Format(result);
        }

        private static long ProcessLine(ReadOnlySpan<char> line)
        {
            // By inspection, maximum size is <10
            Span<int> groupBuffer = stackalloc int[10];
            int groupCount = ReadLine(line, groupBuffer, out ReadOnlySpan<char> map);
            ReadOnlySpan<int> groups = groupBuffer[..groupCount];

            return CountCandidates(groups, MinimumLength(groups), map);
        }

        private static int MinimumLength(ReadOnlySpan<int> groups)
        {
            int minLength = 0;
            foreach (var group in groups)
            {
                minLength += group;
            }

            return minLength + groups.Length - 1;
        }

        private static int CountCandidates(ReadOnlySpan<int> groups, int minimumLength, ReadOnlySpan<char> map)
        {
            int currentGroupLength = groups[0];
            int nextLength = minimumLength - currentGroupLength - 1;

            int matches = 0;
            int currentIndex = 0;
            while (currentIndex <= map.Length - minimumLength)
            {
                if (map[currentIndex] == '.')
                {
                    currentIndex++;
                    continue;
                }

                if (IsMatch(currentGroupLength, map[currentIndex..]))
                {
                    if (groups.Length == 1)
                    {
                        // We only match if there are no more #s after the end of this group
                        if (map[(currentIndex + currentGroupLength)..].IndexOf('#') <= 0)
                        {
                            matches++;
                        }
                    }
                    else
                    {
                        matches += CountCandidates(groups[1..], nextLength, map[(currentIndex + currentGroupLength + 1)..]);
                    }
                }


                if (map[currentIndex] == '#')
                {
                    // We can't advance past a #, because it anchors us to this start place.
                    break;
                }

                // Then add one more
                currentIndex++;
            }

            return matches;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsMatch(int currentGroup, ReadOnlySpan<char> map)
        {
            int i = 0;

            while (i < currentGroup)
            {
                if (map[i] == '.')
                {
                    return false;
                }

                i++;
            }

            if (i < map.Length && map[i] == '#')
            {
                return false;
            }

            return true;
        }

        private static int ReadLine(ReadOnlySpan<char> line, Span<int> groups, out ReadOnlySpan<char> map)
        {
            int indexOfNumbers = line.IndexOf(' ');
            map = line[..indexOfNumbers];
            int current = indexOfNumbers + 1;
            int count = 0;
            while (current < line.Length)
            {
                int next = line[current..].IndexOf(',');
                if (next == -1)
                {
                    next = line.Length;
                }
                else
                {
                    next += current;
                }

                groups[count++] = int.Parse(line[current..next]);
                current = next + 1;
            }

            return count;
        }
    }
}
