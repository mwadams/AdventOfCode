namespace AdventOfCode.Runner.Year2023.Day12
{
    using AdventOfCode.Common;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
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
                result += ProcessLinePt1(line);
            }

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            int index = 1;
            foreach (ReadOnlySpan<char> line in lines)
            {
                result += ProcessLinePt2(line);
                Console.WriteLine($"Processed line {index++}");
            }

            formatter.Format(result);
        }

        private static long ProcessLinePt2(ReadOnlySpan<char> line)
        {
            long result = 0;
            // By inspection, maximum size is <10
            Span<int> groupBuffer = stackalloc int[10];
            int groupCount = ReadLine(line, groupBuffer, out ReadOnlySpan<char> map);
            int extendedMapLength = map.Length + 1;
            Span<char> expandedMap = stackalloc char[(extendedMapLength * 5) - 1];
            Span<int> expandedGroups = stackalloc int[groupCount * 5];

            for (int i = 0; i < 5; ++i)
            {
                map.CopyTo(expandedMap[(extendedMapLength * i)..]);
                int offset = (extendedMapLength * (i + 1)) - 1;
                if (offset < expandedMap.Length)
                {
                    expandedMap[offset] = '?';
                }

                groupBuffer[..groupCount].CopyTo(expandedGroups[(groupCount * i)..]);
            }

            return CountCandidates(expandedGroups, MinimumLength(expandedGroups), expandedMap, []);
        }

        private static long ProcessLinePt1(ReadOnlySpan<char> line)
        {
            // By inspection, maximum size is <10
            Span<int> groupBuffer = stackalloc int[10];
            int groupCount = ReadLine(line, groupBuffer, out ReadOnlySpan<char> map);
            ReadOnlySpan<int> groups = groupBuffer[..groupCount];

            return CountCandidates(groups, MinimumLength(groups), map, []);
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

        private static long CountCandidates(ReadOnlySpan<int> groups, int minimumLength, ReadOnlySpan<char> map, Dictionary<string, long> seenPatterns)
        {
            int currentGroupLength = groups[0];

            StringBuilder sb = new(groups.Length * 2 + map.Length + 1);

            for(int i = 0; i < groups.Length; ++i)
            {
                sb.Append(groups[i]);
                sb.Append(',');
                sb.Append(map);
            }

            string matchKey = sb.ToString();

            if (seenPatterns.TryGetValue(matchKey, out long cachedMatches))
            {
                return cachedMatches;
            }

            int nextLength = minimumLength - currentGroupLength - 1;

            long matches = 0;
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
                        matches += CountCandidates(groups[1..], nextLength, map[(currentIndex + currentGroupLength + 1)..], seenPatterns);
                    }
                }
                else
                {
                    seenPatterns[matchKey] = 0;
                }


                if (map[currentIndex] == '#')
                {
                    // We can't advance past a #, because it anchors us to this start place.
                    break;
                }

                // Then add one more
                currentIndex++;
            }

            seenPatterns[matchKey] = matches;
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
