namespace AdventOfCode.Runner.Year2023.Day10
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
            var lines = await InputReader.ReadLines(2023, 10, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            int width = lines[0].Length;
            int height = lines.Length;

            Span<Tile> map = stackalloc Tile[width * height];
            (int startX, int startY) = BuildMap(lines, map, width, height);
            Span<Tile> visited = stackalloc Tile[width * height];
            result = FindMaxPath(visited, startX, startY, map, width, height) / 2;
            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            int width = lines[0].Length;
            int height = lines.Length;

            Span<Tile> map = stackalloc Tile[width * height];
            (int startX, int startY) = BuildMap(lines, map, width, height);
            Span<Tile> visited = stackalloc Tile[width * height];

            FindMaxPath(visited, startX, startY, map, width, height);

            result = CountInside(visited, width, height);

            formatter.Format(result);
        }

        private static long CountInside(Span<Tile> map, int width, int height)
        {
            long count = 0;
            for (int y = 0; y < height; ++y)
            {
                int insideCount = 0;

                RemoveLoops(map.Slice(y * width, width));

                for (int x = 0; x < width; ++x)
                {
                    int offset = y * width + x;
                    Tile tile = map[offset];
                    if (tile == Tile.None)
                    {
                        if (insideCount % 2 == 1)
                        {
                            count++;
                        }
                        else
                        {
                            map[offset] = Tile.Filled;
                        }
                    }
                    else if ((tile & Tile.NorthAndSouth) != 0)
                    {
                        insideCount++;
                    }
                }
            }

            return count;
        }

        private enum LoopState
        {
            LookingForSouthAndEastOrNorthAndEast,
            FoundSouthAndEast,
            FoundNorthAndEast,
        }

        private static void RemoveLoops(Span<Tile> span)
        {
            LoopState state = LoopState.LookingForSouthAndEastOrNorthAndEast;

            int index = 0;
            int startOfRun = 0;

            foreach (Tile tile in span)
            {
                if (state == LoopState.LookingForSouthAndEastOrNorthAndEast)
                {
                    if (tile == Tile.SouthAndEast)
                    {
                        startOfRun = index;
                        state = LoopState.FoundSouthAndEast;
                    }
                    else if (tile == Tile.NorthAndEast)
                    {
                        startOfRun = index;
                        state = LoopState.FoundNorthAndEast;
                    }
                }
                else if (state == LoopState.FoundSouthAndEast)
                {
                    if (tile == Tile.SouthAndWest)
                    {
                        span[startOfRun..(index + 1)].Fill(Tile.Filled);
                        state = LoopState.LookingForSouthAndEastOrNorthAndEast;
                    }
                    else if (tile == Tile.NorthAndWest)
                    {
                        span[startOfRun..index].Fill(Tile.Filled);
                        span[index] = Tile.NorthAndSouth;
                        state = LoopState.LookingForSouthAndEastOrNorthAndEast;
                    }
                    else if (tile == Tile.EastAndWest)
                    {
                        // NOP
                    }
                    else
                    {
                        state = LoopState.LookingForSouthAndEastOrNorthAndEast;
                    }
                }
                else if (state == LoopState.FoundNorthAndEast)
                {
                    if (tile == Tile.NorthAndWest)
                    {
                        span[startOfRun..(index + 1)].Fill(Tile.Filled);
                        state = LoopState.LookingForSouthAndEastOrNorthAndEast;
                    }
                    else if (tile == Tile.SouthAndWest)
                    {
                        span[startOfRun..index].Fill(Tile.Filled);
                        span[index] = Tile.NorthAndSouth;
                        state = LoopState.LookingForSouthAndEastOrNorthAndEast;
                    }
                    else if (tile == Tile.EastAndWest)
                    {
                        // NOP
                    }
                    else
                    {
                        state = LoopState.LookingForSouthAndEastOrNorthAndEast;
                    }
                }

                ++index;
            }
        }

        private static void Dump(ReadOnlySpan<Tile> visited, int width, int height)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Console.Write(Reverse(visited[y * width + x]));
                }
                Console.WriteLine();
            }
        }

        private static (int StartX, int StartY) BuildMap(ReadOnlySpan<string> lines, Span<Tile> map, int width, int height)
        {
            int startX = -1;
            int startY = -1;

            int x = 0;
            int y = 0;

            foreach (ReadOnlySpan<char> line in lines)
            {
                foreach (char mapTile in line)
                {
                    Tile tile = GetTile(mapTile);
                    if ((tile & Tile.Start) != 0)
                    {
                        startX = x;
                        startY = y;
                    }

                    map[y * width + x] = tile;
                    x++;
                }

                x = 0;
                y++;
            }

            map[startY * width + startX] = GetActualStartTile(startX, startY, map, width, height);
            return (startX, startY);
        }

        private static long FindMaxPath(Span<Tile> visited, int startX, int startY, ReadOnlySpan<Tile> map, int width, int height)
        {
            Stack<(int x, int y, long count)> stillToVisit = new(width * height);

            stillToVisit.Push((startX, startY, 0));
            return FindPathsCore(stillToVisit, visited, map, width, height);
        }

        private static long FindPathsCore(Stack<(int x, int y, long count)> stillToVisit, Span<Tile> visited, ReadOnlySpan<Tile> map, int width, int height)
        {
            long maxCount = 0;

            while (stillToVisit.TryPop(out (int X, int Y, long MaxCount) current))
            {
                int offset = current.Y * width + current.X;

                if (visited[offset] != 0)
                {
                    // Do we have to continue, or do we have to add the loop count?
                    continue;
                }

                visited[offset] = map[offset];

                Directions availableDirections = AvailableDirections(current.X, current.Y, map, width, height);
                if (availableDirections == Directions.None)
                {
                    continue;
                }

                long currentMaxCount = current.MaxCount + 1;
                if ((availableDirections & Directions.North) != 0 
                    && visited[(current.Y - 1) * width + current.X] == Tile.None)
                {
                    stillToVisit.Push((current.X, current.Y - 1, currentMaxCount));
                }

                if ((availableDirections & Directions.South) != 0
                     && visited[(current.Y + 1) * width + current.X] == Tile.None)

                {
                    stillToVisit.Push((current.X, current.Y + 1, currentMaxCount));
                }

                if ((availableDirections & Directions.East) != 0
                    && visited[current.Y * width + current.X + 1] == Tile.None)
                {
                    stillToVisit.Push((current.X + 1, current.Y, currentMaxCount));
                }

                if ((availableDirections & Directions.West) != 0
                    && visited[current.Y * width + current.X - 1] == Tile.None)
                {
                    stillToVisit.Push((current.X - 1, current.Y, currentMaxCount));
                }

                // Increment the count by 1
                if (currentMaxCount > maxCount)
                {
                    maxCount = currentMaxCount;
                }
            }

            return maxCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Tile GetActualStartTile(int x, int y, ReadOnlySpan<Tile> map, int width, int height)
        {
            Tile north = y > 0 ? GetLocation(x, y - 1, map, width) : Tile.None;
            Tile south = y < height - 1 ? GetLocation(x, y + 1, map, width) : Tile.None;
            Tile east = x < width - 1 ? GetLocation(x + 1, y, map, width) : Tile.None;
            Tile west = x > 0 ? GetLocation(x - 1, y, map, width) : Tile.None;

            bool connectsEast = false;
            bool connectsWest = false;
            bool connectsNorth = false;
            bool connectsSouth = false;

            if ((east & Tile.ConnectsWest) != 0)
            {
                connectsEast = true;
            }

            if ((west & Tile.ConnectsEast) != 0)
            {
                connectsWest = true;
            }

            if ((north & Tile.ConnectsSouth) != 0)
            {
                connectsNorth = true;
            }

            if ((south & Tile.ConnectsNorth) != 0)
            {
                connectsSouth = true;
            }

            if (connectsEast)
            {
                if (connectsWest)
                {
                    return Tile.EastAndWest;
                }

                if (connectsNorth)
                {
                    return Tile.NorthAndEast;
                }

                if (connectsSouth)
                {
                    return Tile.SouthAndEast;
                }

                throw new InvalidOperationException("No valid path.");
            }

            if (connectsWest)
            {
                if (connectsNorth)
                {
                    return Tile.NorthAndWest;
                }

                if (connectsSouth)
                {
                    return Tile.SouthAndWest;
                }

                throw new InvalidOperationException("No valid path.");
            }

            if (connectsNorth)
            {
                if (connectsSouth)
                {
                    return Tile.NorthAndSouth;
                }

                throw new InvalidOperationException("No valid path.");
            }

            throw new InvalidOperationException("No valid path.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Directions AvailableDirections(int x, int y, ReadOnlySpan<Tile> map, int width, int height)
        {
            Tile currentLocation = GetLocation(x, y, map, width);

            Tile north = y > 0 ? GetLocation(x, y - 1, map, width) : Tile.None;
            Tile south = y < height - 1 ? GetLocation(x, y + 1, map, width) : Tile.None;
            Tile east = x < width - 1 ? GetLocation(x + 1, y, map, width) : Tile.None;
            Tile west = x > 0 ? GetLocation(x - 1, y, map, width) : Tile.None;

            Directions directions = Directions.None;
            if ((currentLocation & Tile.ConnectsEast) != 0 &&
                (east & Tile.ConnectsWest) != 0)
            {
                directions |= Directions.East;
            }

            if ((currentLocation & Tile.ConnectsWest) != 0 &&
                     (west & Tile.ConnectsEast) != 0)
            {
                directions |= Directions.West;
            }

            if ((currentLocation & Tile.ConnectsNorth) != 0 &&
                     (north & Tile.ConnectsSouth) != 0)
            {
                directions |= Directions.North;
            }

            if ((currentLocation & Tile.ConnectsSouth) != 0 &&
                     (south & Tile.ConnectsNorth) != 0)
            {
                directions |= Directions.South;
            }

            return directions;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Tile GetLocation(int x, int y, ReadOnlySpan<Tile> map, int width)
        {
            return map[y * width + x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Tile GetTile(char tile)
        {
            return tile switch
            {
                '.' => Tile.None,
                'S' => Tile.Start,
                '|' => Tile.NorthAndSouth,
                '-' => Tile.EastAndWest,
                'L' => Tile.NorthAndEast,
                'J' => Tile.NorthAndWest,
                '7' => Tile.SouthAndWest,
                'F' => Tile.SouthAndEast,
                _ => throw new InvalidOperationException($"Invalid tile: {tile}"),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char Reverse(Tile tile)
        {
            return tile switch
            {
                Tile.None => '.',
                Tile.Start => 'S',
                Tile.Filled => '*',
                Tile.NorthAndSouth => '|',
                Tile.EastAndWest => '-',
                Tile.NorthAndEast => 'L',
                Tile.NorthAndWest => 'J',
                Tile.SouthAndWest => '7',
                Tile.SouthAndEast => 'F',
                _ => throw new InvalidOperationException($"Invalid tile: {tile}"),
            };
        }
    }

    [Flags]
    enum Directions : byte
    {
        None = 0b0000,
        North = 0b0001,
        South = 0b0010,
        East = 0b0100,
        West = 0b1000,
    }


    [Flags]
    enum Tile : byte
    {
        None = 0b0000_0000,
        Start = 0b0000_0001,
        NorthAndSouth = 0b0000_0010,
        EastAndWest = 0b0000_0100,
        NorthAndEast = 0b0000_1000,
        NorthAndWest = 0b0001_0000,
        SouthAndWest = 0b0010_0000,
        SouthAndEast = 0b0100_0000,
        Filled = 0b1000_0000,

        ConnectsNorth = NorthAndSouth | NorthAndEast | NorthAndWest,
        ConnectsSouth = NorthAndSouth | SouthAndEast | SouthAndWest,
        ConnectsEast = EastAndWest | NorthAndEast | SouthAndEast,
        ConnectsWest = EastAndWest | NorthAndWest | SouthAndWest,
    }
}