namespace AdventOfCode.Runner.Year2023.Day18
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
            var lines = await InputReader.ReadLines(2023, 18, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            Span<(int Distance, char Direction, int Colour)> instructions = stackalloc (int, char, int)[lines.Length];

            (int minX, int minY, int maxX, int maxY) = GetInstructions(lines, instructions);
            (int x, int y, int width, int height) = GetSizeAndOffset(minX, minY, maxX, maxY);

            Span<int> map = stackalloc int[width * height];

            result = PaintMap(instructions, map, x, y, width, height);

            (x, y) = FindSeed(map, width, height);

            Dump(map, width, height, x, y);

            Stack<(int X, int Y)> work = new(width * height * 3);
            work.Push((x, y));
            result += BoundaryFill(work, map, width, height);


            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            formatter.Format(result);
        }

        private static (int x, int y) FindSeed(Span<int> map, int width, int height)
        {
            bool foundNotEmpty = false;
            for(int y = 0; y < height; ++y)
            {
                for(int x = 0; x < width; ++x)
                {
                    if (!foundNotEmpty && map[y * width + x] != 0)
                    {
                        foundNotEmpty = true;
                        // Move down a row
                        y += 1;
                    }
                    else if (foundNotEmpty && map[y * width + x] == 0)
                    {
                        return (x, y);
                    }
                }
            }

            throw new InvalidOperationException("Did not find a seed.");
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (int x, int y, int width, int height) GetSizeAndOffset(int minX, int minY, int maxX, int maxY)
        {
            int absMinX = Math.Abs(minX);
            int absMinY = Math.Abs(minY);

            return (absMinX, absMinY, absMinX + Math.Abs(maxX) + 1, absMinY + Math.Abs(maxY) + 1);
        }

        private static (int MinX, int MinY, int MaxX, int MaxY) GetInstructions(ReadOnlySpan<string> lines, Span<(int Distance, char Direction, int Colour)> instructions)
        {
            int x = 0;
            int y = 0;
            int maxX = 0;
            int maxY = 0;
            int minX = 0;
            int minY = 0;
            int instructionPointer = 0;

            foreach (ReadOnlySpan<char> line in lines)
            {
                int distance = int.Parse(line[2..4]);
                int colourStart = 6;

                if (distance >= 10)
                {
                    colourStart++;
                }

                int colour = int.Parse(line[colourStart..^1], System.Globalization.NumberStyles.HexNumber);

                instructions[instructionPointer++] = (distance, line[0], colour);

                Move(line[0], distance, ref x, ref y);

                if (x > maxX)
                {
                    maxX = x;
                }

                if (y > maxY)
                {
                    maxY = y;
                }

                if (x < minX)
                {
                    minX = x;
                }

                if (y < minY)
                {
                    minY = y;
                }
            }

            return (minX, minY, maxX, maxY);
        }

        private static long PaintMap(ReadOnlySpan<(int Distance, char Direction, int Colour)> instructions, Span<int> map, int x, int y, int width, int height)
        {
            long setCount = 0;
            foreach ((int distance, char direction, int colour) in instructions)
            {
                for (int i = 0; i < distance; i++)
                {
                    Move(direction, 1, ref x, ref y);
                    map[(y * width) + x] = colour;
                    setCount++;
                }
            }

            return setCount;
        }


        private static long BoundaryFill(Stack<(int X, int Y)> work, Span<int> map, int width, int height)
        {
            long count = 0;

            while (work.TryPop(out (int x, int y) current))
            {
                if (current.y < 0 || current.y >= height | current.x < 0 || current.x >= width)
                {
                    continue;
                }

                ref int cell = ref map[current.y * width + current.x];
                if (cell == 0)
                {
                    cell = 1;
                    count++;
                    work.Push((current.x + 1, current.y));
                    work.Push((current.x, current.y + 1));
                    work.Push((current.x - 1, current.y));
                    work.Push((current.x, current.y - 1));
                    work.Push((current.x - 1, current.y - 1));
                    work.Push((current.x - 1, current.y + 1));
                    work.Push((current.x + 1, current.y - 1));
                    work.Push((current.x + 1, current.y + 1));
                }
            }

            return count;
        }

        private static void Dump(Span<int> map, int width, int height, int seedX, int seedY)
        {
            int offset = 0;
            for(int y = 0; y < height; ++y)
            {
                for(int x = 0; x < width; ++x)
                {
                    if (x == seedX && y == seedY)
                    {
                        Console.Write("X");
                    }
                    else
                    {
                        Console.Write(map[offset] == 0 ? '.' : map[offset] == 1 ? 'O' : '#');
                    }

                    offset++;
                }

                Console.WriteLine();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Move(char direction, int distance, ref int x, ref int y)
        {
            switch (direction)
            {
                case 'R':
                    x += distance;
                    break;
                case 'L':
                    x -= distance;
                    break;
                case 'U':
                    y -= distance;
                    break;
                case 'D':
                    y += distance;
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected direction: {direction}");
            }
        }
    }
}
