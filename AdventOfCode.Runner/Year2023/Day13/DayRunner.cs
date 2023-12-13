namespace AdventOfCode.Runner.Year2023.Day13
{
    using AdventOfCode.Common;
    using System.ComponentModel;
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
            var lines = await InputReader.ReadLines(2023, 13, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = ProcessLines(lines);
            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = ProcessLinesWithSmudge(lines);
            formatter.Format(result);
        }

        private static long ProcessLines(ReadOnlySpan<string> lines)
        {
            long result = 0;
            int prevIndex = 0;
            int index = 0;
            foreach (ReadOnlySpan<char> line in lines)
            {
                if (line.IsEmpty)
                {
                    result += FindReflections(lines[prevIndex..index]);
                    prevIndex = index + 1;
                }

                index++;
            }

            result += FindReflections(lines[prevIndex..]);

            return result;
        }

        private static long ProcessLinesWithSmudge(ReadOnlySpan<string> lines)
        {
            long result = 0;
            int prevIndex = 0;
            int index = 0;
            foreach (ReadOnlySpan<char> line in lines)
            {
                if (line.IsEmpty)
                {
                    result += FindReflectionsWithSmudge(lines[prevIndex..index]);
                    prevIndex = index + 1;
                }

                index++;
            }

            result += FindReflectionsWithSmudge(lines[prevIndex..]);

            return result;
        }


        private static long FindReflections(ReadOnlySpan<string> lines)
        {
            long result = 0;

            Span<char> map = stackalloc char[lines.Length * lines[0].Length];
            Span<char> transposedMap = stackalloc char[lines.Length * lines[0].Length];
            BuildMap(lines, map, transposedMap);

            long horizontal = FindReflections(map, lines[0].Length, lines.Length);
            long vertical = FindReflections(transposedMap, lines.Length, lines[0].Length);

            Dump(map, horizontal, vertical, lines[0].Length, lines.Length);

            result += (100 * horizontal) + vertical;

            Console.WriteLine();

            return result;
        }

        private static long FindReflectionsWithSmudge(ReadOnlySpan<string> lines)
        {
            long result = 0;

            Span<char> map = stackalloc char[lines.Length * lines[0].Length];
            Span<char> transposedMap = stackalloc char[lines.Length * lines[0].Length];
            BuildMap(lines, map, transposedMap);

            long horizontal = FindReflectionsWithSmudge(map, lines[0].Length, lines.Length);
            long vertical = FindReflectionsWithSmudge(transposedMap, lines.Length, lines[0].Length);

            Dump(map, horizontal, vertical, lines[0].Length, lines.Length);

            result += (100 * horizontal) + vertical;

            Console.WriteLine();

            return result;
        }

        private static void Dump(ReadOnlySpan<char> map, long horizontal, long vertical, int width, int height)
        {
            Console.WriteLine();

            if (horizontal != 0)
            {
                Console.WriteLine($"Horizontal: {horizontal}");
            }

            if (vertical != 0)
            {
                Console.WriteLine($"Vertical: {vertical}");
                for(int x = 0; x < width; ++x)
                {
                    if (x == vertical - 1)
                    {
                        Console.Write('>');
                    }
                    else if (x == vertical)
                    {
                        Console.Write('<');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }

                Console.WriteLine();
            }

            for (int y = 0; y < height; y++)
            {
                if(horizontal != 0)
                {
                    if(y == horizontal - 1)
                    {
                        Console.Write('v');
                    }
                    else if (y == horizontal)
                    {
                        Console.Write('^');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }

                Console.Write(map.Slice(y * width, width).ToString());

                if (horizontal != 0)
                {
                    if (y == horizontal - 1)
                    {
                        Console.Write('v');
                    }
                    else if (y == horizontal)
                    {
                        Console.Write('^');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }

                Console.WriteLine();
            }

            if (vertical != 0)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (x == vertical - 1)
                    {
                        Console.Write('>');
                    }
                    else if (x == vertical)
                    {
                        Console.Write('<');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }

                Console.WriteLine();
            }
        }

        private static long FindReflections(ReadOnlySpan<char> map, int width, int height)
        {
            ReadOnlySpan<char> lineZero = map[..width];

            for (int y = height - 1; y > 0; y--)
            {
                if (map.Slice(y * width, width).SequenceEqual(lineZero))
                {
                    bool found = CheckFrom(map, width, 1, y - 1);
                    if (found)
                    {
                        // Add one for the non-zero offset
                        return (y / 2) + 1;
                    }
                }
            }

            ReadOnlySpan<char> lineLast = map[^width..];
            for (int y = 0; y < height - 1; y++)
            {
                if (map.Slice(y * width, width).SequenceEqual(lineLast))
                {
                    bool found = CheckFrom(map, width, y + 1, height - 2);
                    if (found)
                    {
                        return y + (height - y) / 2;
                    }
                }
            }

            return 0;
        }

        private static long FindReflectionsWithSmudge(ReadOnlySpan<char> map, int width, int height)
        {
            ReadOnlySpan<char> lineZero = map[..width];

            for (int y = height - 1; y > 0; y--)
            {
                (bool match, bool usedSmudge) = CompareWithSmudge(map.Slice(y * width, width), lineZero);

                if (match)
                {
                    (bool found, bool additionalSmudge) = CheckWithSmudgeFrom(map, width, 1, y - 1);
                    if (found && (additionalSmudge ^ usedSmudge))
                    {
                        // Add one for the non-zero offset
                        return (y / 2) + 1;
                    }
                }
            }

            ReadOnlySpan<char> lineLast = map[^width..];
            for (int y = 0; y < height - 1; y++)
            {
                (bool match, bool usedSmudge) = CompareWithSmudge(map.Slice(y * width, width), lineLast);

                if (match)
                {
                    (bool found, bool additionalSmudge) = CheckWithSmudgeFrom(map, width, y + 1, height - 2);
                    if (found && (additionalSmudge ^ usedSmudge))
                    {
                        return y + (height - y) / 2;
                    }
                }
            }

            return 0;
        }


        private static bool CheckFrom(ReadOnlySpan<char> map, int width, int inv1, int inv2)
        {
            if (inv1 == inv2)
            {
                return false;
            }

            int v1 = inv1;
            int v2 = inv2;

            while (v1 < v2)
            {
                if (!map.Slice(v1 * width, width).SequenceEqual(map.Slice(v2 * width, width)))
                {
                    return false;
                }

                v1++;
                v2--;
            }

            return true;
        }

        private static (bool, bool) CheckWithSmudgeFrom(ReadOnlySpan<char> map, int width, int inv1, int inv2)
        {
            if (inv1 == inv2)
            {
                return (false, false);
            }

            int v1 = inv1;
            int v2 = inv2;

            bool smudged = false;
            while (v1 < v2)
            {
                (bool match, bool usedSmudge) = CompareWithSmudge(map.Slice(v1 * width, width), map.Slice(v2 * width, width));
                if (!match || (usedSmudge && smudged))
                {
                    return (false, false);
                }
                else
                {
                    smudged = smudged | usedSmudge;
                }

                v1++;
                v2--;
            }

            return (true, smudged);
        }

        private static (bool match, bool usedSmudge) CompareWithSmudge(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
        {
            int commonPrefix = left.CommonPrefixLength(right);
            if (commonPrefix == left.Length)
            {
                return (true, false);
            }

            // One character different at the end
            if (commonPrefix == left.Length - 1)
            {
                return (true, true);
            }

            // They have prefix, followed by a single distinct character, followed by a match
            if (left[(commonPrefix + 1)..].SequenceEqual(right[(commonPrefix + 1)..]))
            {
                return (true, true);
            }

            return (false, false);
        }

        private static void BuildMap(ReadOnlySpan<string> lines, Span<char> map, Span<char> transposedMap)
        {
            int offset = 0;
            int height = lines.Length;
            int y = 0;
            foreach (ReadOnlySpan<char> line in lines)
            {
                for (int x = 0; x < line.Length; ++x)
                {
                    map[offset + x] = line[x];
                    transposedMap[x * height + y] = line[x];
                }

                offset += line.Length;
                y++;
            }
        }
    }
}
