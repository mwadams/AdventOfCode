namespace AdventOfCode.Runner.Year2023.Day03
{
    using AdventOfCode.Common;
    using System.Buffers;
    using System.Threading.Tasks;

    public class DayRunner : IDay<DayRunner>
    {
        private readonly string[] lines;
        private static readonly SearchValues<char> NotSymbols = SearchValues.Create("0123456789.");
        private static readonly SearchValues<char> Numbers = SearchValues.Create("0123456789");
        public DayRunner(string[] lines)
        {
            this.lines = lines;
        }

        public static async ValueTask<DayRunner> Initialize(bool test = false)
        {
            var lines = await InputReader.ReadLines(2023, 3, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            int width = lines[0].Length;
            int height = lines.Length;

            // Allocate a grid
            char[] gridArray = ArrayPool<char>.Shared.Rent(width * height);
            Span<char> grid = gridArray.AsSpan();

            // Copy the first line of input into the grid.
            lines[0].CopyTo(grid);

            try
            {
                // Copy the input into the grid as we go
                for (int y = 0; y < height; ++y)
                {
                    // Copy the *next* line into the grid, if available
                    // This makes it ready for the lookahead
                    if (y < height - 1)
                    {
                        lines[y + 1].CopyTo(grid[((y + 1) * width)..]);
                    }

                    for (int x = 0; x < width; ++x)
                    {
                        char cell = grid[y * width + x];
                        if (!NotSymbols.Contains(cell))
                        {
                            result += FindAdjacentNumbers(grid, width, height, x, y);
                        }
                    }
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(gridArray);
            }

            formatter.Format(result);
        }

        private static void DumpGrid(int width, int height, Span<char> grid)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Console.Write(grid[y * width + x]);
                }

                Console.WriteLine();
            }
        }

        private long FindAdjacentNumbers(Span<char> grid, int width, int height, int x, int y)
        {
            long localResult = 0;
            localResult += FindLeft(grid, height, x, y);
            localResult += FindRight(grid, width, height, x, y);
            localResult += FindAbove(grid, width, height, x, y);
            localResult += FindBelow(grid, width, height, x, y);
            return localResult;
        }

        private long FindExactlyTwoAdjacentNumbers(Span<char> grid, int width, int height, int x, int y)
        {
            long localResult = 1;
            int found = 0;
            long current = FindLeft(grid, height, x, y);

            if (current != 0)
            {
                found++;
                localResult *= current;
            }

            current = FindRight(grid, width, height, x, y);

            if (current != 0)
            {
                found++;
                localResult *= current;
            }


            (current, int foundCurrent) = FindAboveWithCount(grid, width, height, x, y);
            if (foundCurrent > 0)
            {
                found += foundCurrent;
                if (found > 2)
                {
                    return 0;
                }

                localResult *= current;
            }

            (current, foundCurrent) = FindBelowWithCount(grid, width, height, x, y);
            if (foundCurrent > 0)
            {
                found += foundCurrent;
                if (found > 2)
                {
                    return 0;
                }

                localResult *= current;
            }

            if (found == 2)
            {
                return localResult;
            }
            else
            {
                return 0;
            }
        }


        private long FindAbove(Span<char> grid, int width, int height, int x, int y)
        {
            if (y == 0)
            {
                return 0;
            }

            int lineStart = (y - 1) * height;

            return FindLeftRight(grid, width, x, lineStart);
        }

        private long FindBelow(Span<char> grid, int width, int height, int x, int y)
        {
            if (y == height - 1)
            {
                return 0;
            }

            int lineStart = (y + 1) * height;

            return FindLeftRight(grid, width, x, lineStart);
        }

        private (long Result, int Found) FindAboveWithCount(Span<char> grid, int width, int height, int x, int y)
        {
            if (y == 0)
            {
                return (0,0);
            }

            int lineStart = (y - 1) * height;

            return FindLeftRightWithCount(grid, width, x, lineStart);
        }

        private (long Result, int Found) FindBelowWithCount(Span<char> grid, int width, int height, int x, int y)
        {
            if (y == height - 1)
            {
                return (0,0);
            }

            int lineStart = (y + 1) * height;

            return FindLeftRightWithCount(grid, width, x, lineStart);
        }

        private static long FindLeftRight(Span<char> grid, int width, int x, int lineStart)
        {
            int result = 0;

            if (Numbers.Contains(grid[lineStart + x]))
            {
                result += ScanLeftRight(grid, width, x, lineStart);

            }

            if (x > 0 && Numbers.Contains(grid[lineStart + x - 1]))
            {
                result += ScanLeftRight(grid, width, x - 1, lineStart);
            }

            if (x < width - 1 && Numbers.Contains(grid[lineStart + x + 1]))
            {
                result += ScanLeftRight(grid, width, x + 1, lineStart);
            }

            return result;
        }

        private static (long Result, int Found) FindLeftRightWithCount(Span<char> grid, int width, int x, int lineStart)
        {
            long result = 1;
            int found = 0;
            if (Numbers.Contains(grid[lineStart + x]))
            {
                long local = ScanLeftRight(grid, width, x, lineStart);
                if (local > 0)
                {
                    found++;
                    result *= local;
                }
            }

            if (x > 0 && Numbers.Contains(grid[lineStart + x - 1]))
            {
                long local = ScanLeftRight(grid, width, x - 1, lineStart);
                if (local > 0)
                {
                    found++;
                    result *= local;
                }
            }

            if (x < width - 1 && Numbers.Contains(grid[lineStart + x + 1]))
            {
                long local = ScanLeftRight(grid, width, x + 1, lineStart);
                if (local > 0)
                {
                    found++;
                    result *= local;
                }
            }

            return (result, found);
        }

        private static int ScanLeftRight(Span<char> grid, int width, int x, int lineStart)
        {
            int start = x;
            int end = x;
            while (start > 0 && Numbers.Contains(grid[lineStart + (start - 1)]))
            {
                start--;
            }

            while (end < width - 1 && Numbers.Contains(grid[lineStart + (end + 1)]))
            {
                end++;
            }

            int result = int.Parse(grid[(lineStart + start)..(lineStart + end + 1)]);

            grid[(lineStart + start)..(lineStart + end + 1)].Fill('.');
            return result;
        }

        private long FindLeft(Span<char> grid, int height, int x, int y)
        {
            if (x == 0)
            {
                return 0;
            }

            int lineStart = y * height;
            int end = x;
            int start = end;
            while(start > 0 && Numbers.Contains(grid[lineStart + (start - 1)]))
            {
                start--;
            }

            if (start != end)
            {
                int result = int.Parse(grid[(lineStart + start)..(lineStart + end)]);

                grid[(lineStart + start)..(lineStart + end)].Fill('.');
                return result;
            }

            return 0;
        }

        private long FindRight(Span<char> grid, int width, int height, int x, int y)
        {
            if (x == width - 1)
            {
                return 0;
            }

            int lineStart = y * height;
            int start = x;
            int end = start;
            while (end < width - 1 && Numbers.Contains(grid[lineStart + (end + 1)]))
            {
                end++;
            }

            if (start != end)
            {
                int result = int.Parse(grid[(lineStart + start + 1)..(lineStart + end + 1)]);

                grid[(lineStart + start + 1)..(lineStart + end + 1)].Fill('.');
                return result;
            }

            return 0;
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            int width = lines[0].Length;
            int height = lines.Length;

            // Allocate a grid
            char[] gridArray = ArrayPool<char>.Shared.Rent(width * height);
            Span<char> grid = gridArray.AsSpan();

            // Copy the first line of input into the grid.
            lines[0].CopyTo(grid);

            try
            {
                // Copy the input into the grid as we go
                for (int y = 0; y < height; ++y)
                {
                    // Copy the *next* line into the grid, if available
                    // This makes it ready for the lookahead
                    if (y < height - 1)
                    {
                        lines[y + 1].CopyTo(grid[((y + 1) * width)..]);
                    }

                    for (int x = 0; x < width; ++x)
                    {
                        char cell = grid[y * width + x];
                        if (cell == '*')
                        {
                            result += FindExactlyTwoAdjacentNumbers(grid, width, height, x, y);
                        }
                    }
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(gridArray);
            }
            formatter.Format(result);
        }
    }
}
