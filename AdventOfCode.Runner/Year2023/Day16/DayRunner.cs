namespace AdventOfCode.Runner.Year2023.Day16
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
            var lines = await InputReader.ReadLines(2023, 16, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            int width = lines[0].Length;
            int height = lines.Length;
            Span<Tile> map = stackalloc Tile[width * height];

            BuildMap(lines, map);

            result = ProcessBeam(BeamDirection.GoingRight, -1, 0, map, width, height);

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            int width = lines[0].Length;
            int height = lines.Length;
            Span<Tile> map = stackalloc Tile[width * height];
            Span<Tile> mapBackup = stackalloc Tile[width * height];

            BuildMap(lines, map);

            map.CopyTo(mapBackup);

            for (int x = 0; x < width; x++)
            {
                result = Math.Max(result, ProcessBeam(BeamDirection.GoingDown, x, -1, map, width, height));

                mapBackup.CopyTo(map);

                result = Math.Max(result, ProcessBeam(BeamDirection.GoingUp, x, height, map, width, height));

                // We *do* copy this time to prep for the first y
                mapBackup.CopyTo(map);
            }

            for (int y = 0; y < height; y++)
            {
                result = Math.Max(result, ProcessBeam(BeamDirection.GoingRight, -1, y, map, width, height));

                mapBackup.CopyTo(map);

                result = Math.Max(result, ProcessBeam(BeamDirection.GoingLeft, width, y, map, width, height));

                if (y != height - 1)
                {
                    // We don't copy the last time
                    mapBackup.CopyTo(map);
                }
            }

            formatter.Format(result);
        }

        private static int ProcessBeam(BeamDirection direction, int x, int y, Span<Tile> map, int width, int height)
        {
            int energizedCount = 0;

            while (true)
            {
                if (direction == BeamDirection.GoingLeft)
                {
                    x--;
                    if (x < 0)
                    {
                        // We've gone off the edge
                        return energizedCount;
                    }

                    ref Tile tile = ref map[y * width + x];

                    if ((tile & Tile.BeamFromRight) != 0)
                    {
                        // We've hit a beam entering this tile from the right, so we can stop
                        return energizedCount;
                    }

                    if ((tile & Tile.IsEnergized) == 0)
                    {
                        // The tile was not already energized
                        energizedCount++;
                    }

                    // Now add ourselves to the tile
                    tile |= Tile.BeamFromRight;


                    if ((tile & Tile.MirrorBottomLeftTopRight) != 0)
                    {
                        direction = BeamDirection.GoingDown;
                    }
                    else if ((tile & Tile.MirrorTopLeftBottomRight) != 0)
                    {
                        direction = BeamDirection.GoingUp;
                    }
                    else if ((tile & Tile.MirrorTopBottom) != 0)
                    {
                        // We change to a top beam
                        direction = BeamDirection.GoingUp;
                        // And we add a bottom beam
                        energizedCount += ProcessBeam(BeamDirection.GoingDown, x, y, map, width, height);
                    }
                }
                else if (direction == BeamDirection.GoingRight)
                {
                    x++;
                    if (x >= width)
                    {
                        // We've gone off the edge
                        return energizedCount;
                    }

                    ref Tile tile = ref map[y * width + x];

                    if ((tile & Tile.BeamFromLeft) != 0)
                    {
                        // We've hit a beam entering this tile from the left, so we can stop
                        return energizedCount;
                    }

                    if ((tile & Tile.IsEnergized) == 0)
                    {
                        // The tile was not already energized
                        energizedCount++;
                    }

                    // Now add ourselves to the tile
                    tile |= Tile.BeamFromLeft;


                    if ((tile & Tile.MirrorBottomLeftTopRight) != 0)
                    {
                        direction = BeamDirection.GoingUp;
                    }
                    else if ((tile & Tile.MirrorTopLeftBottomRight) != 0)
                    {
                        direction = BeamDirection.GoingDown;
                    }
                    else if ((tile & Tile.MirrorTopBottom) != 0)
                    {
                        // We change to a top beam
                        direction = BeamDirection.GoingUp;
                        // And we add a bottom beam
                        energizedCount += ProcessBeam(BeamDirection.GoingDown, x, y, map, width, height);
                    }
                }
                else if (direction == BeamDirection.GoingUp)
                {
                    y--;
                    if (y < 0)
                    {
                        // We've gone off the edge
                        return energizedCount;
                    }

                    ref Tile tile = ref map[y * width + x];

                    if ((tile & Tile.BeamFromBottom) != 0)
                    {
                        // We've hit a beam entering this tile from the bottom, so we can stop
                        return energizedCount;
                    }

                    if ((tile & Tile.IsEnergized) == 0)
                    {
                        // The tile was not already energized
                        energizedCount++;
                    }

                    // Now add ourselves to the tile
                    tile |= Tile.BeamFromBottom;


                    if ((tile & Tile.MirrorBottomLeftTopRight) != 0)
                    {
                        direction = BeamDirection.GoingRight;
                    }
                    else if ((tile & Tile.MirrorTopLeftBottomRight) != 0)
                    {
                        direction = BeamDirection.GoingLeft;
                    }
                    else if ((tile & Tile.MirrorLeftRight) != 0)
                    {
                        // We change to a right beam
                        direction = BeamDirection.GoingRight;
                        // And we add a left beam
                        energizedCount += ProcessBeam(BeamDirection.GoingLeft, x, y, map, width, height);
                    }
                }
                else if (direction == BeamDirection.GoingDown)
                {
                    y++;
                    if (y >= height)
                    {
                        // We've gone off the edge
                        return energizedCount;
                    }

                    ref Tile tile = ref map[y * width + x];

                    if ((tile & Tile.BeamFromTop) != 0)
                    {
                        // We've hit a beam entering this tile from the bottom, so we can stop
                        return energizedCount;
                    }

                    if ((tile & Tile.IsEnergized) == 0)
                    {
                        // The tile was not already energized
                        energizedCount++;
                    }

                    // Now add ourselves to the tile
                    tile |= Tile.BeamFromTop;


                    if ((tile & Tile.MirrorBottomLeftTopRight) != 0)
                    {
                        direction = BeamDirection.GoingLeft;
                    }
                    else if ((tile & Tile.MirrorTopLeftBottomRight) != 0)
                    {
                        direction = BeamDirection.GoingRight;
                    }
                    else if ((tile & Tile.MirrorLeftRight) != 0)
                    {
                        // We change to a right beam
                        direction = BeamDirection.GoingRight;
                        // And we add a left beam
                        energizedCount += ProcessBeam(BeamDirection.GoingLeft, x, y, map, width, height);
                    }
                }
            }
        }

        private static void BuildMap(ReadOnlySpan<string> lines, Span<Tile> map)
        {
            int offset = 0;

            foreach (ReadOnlySpan<char> line in lines)
            {
                foreach (char tile in line)
                {
                    map[offset++] = tile switch
                    {
                        '.' => Tile.Empty,
                        '/' => Tile.MirrorBottomLeftTopRight,
                        '\\' => Tile.MirrorTopLeftBottomRight,
                        '-' => Tile.MirrorLeftRight,
                        '|' => Tile.MirrorTopBottom,
                        _ => throw new InvalidOperationException("Unknown tile"),
                    };
                }
            }
        }

        private enum BeamDirection
        {
            GoingLeft,
            GoingRight,
            GoingDown,
            GoingUp,
        }

        [Flags]
        private enum Tile
        {
            Empty = 0b0000_0000,
            MirrorBottomLeftTopRight = 0b0000_0001,
            MirrorTopLeftBottomRight = 0b0000_0010,
            MirrorLeftRight = 0b0000_0100,
            MirrorTopBottom = 0b0000_1000,
            BeamFromLeft = 0b0001_0000,
            BeamFromRight = 0b0010_0000,
            BeamFromBottom = 0b0100_0000,
            BeamFromTop = 0b1000_0000,
            IsEnergized = BeamFromLeft | BeamFromRight | BeamFromTop | BeamFromBottom
        }
    }
}
