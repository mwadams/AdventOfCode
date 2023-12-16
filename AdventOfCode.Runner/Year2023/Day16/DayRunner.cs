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

            Stack<Beam> beams = new();

            beams.Push(new Beam(BeamDirection.Right, 0, -1, 0));

            result = Process(beams, map, width, height);

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            formatter.Format(result);
        }

        private static long Process(Stack<Beam> beams, Span<Tile> map, int width, int height)
        {
            long result = 0;

            while (beams.TryPop(out Beam current))
            {
                result += ProcessBeam(current, beams, map, width, height);
            }

            return result;
        }

        private static long ProcessBeam(Beam beam, Stack<Beam> beams, Span<Tile> map, int width, int height)
        {
            int x = beam.X;
            int y = beam.Y;
            int energizedCount = beam.EnergizedCount;
            BeamDirection direction = beam.Direction;

            while (true)
            {
                if (direction == BeamDirection.Left)
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
                        direction = BeamDirection.Bottom;                        
                    }
                    else if ((tile & Tile.MirrorTopLeftBottomRight) != 0)
                    {
                        direction = BeamDirection.Top;
                    }
                    else if ((tile & Tile.MirrorTopBottom) != 0)
                    {
                        // We change to a top beam
                        direction = BeamDirection.Top;
                        // And we add a bottom beam
                        beams.Push(new(BeamDirection.Bottom, 0,x, y));
                    }
                }
                else if (direction == BeamDirection.Right)
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
                        direction = BeamDirection.Top;
                    }
                    else if ((tile & Tile.MirrorTopLeftBottomRight) != 0)
                    {
                        direction = BeamDirection.Bottom;
                    }
                    else if ((tile & Tile.MirrorTopBottom) != 0)
                    {
                        // We change to a top beam
                        direction = BeamDirection.Top;
                        // And we add a bottom beam
                        beams.Push(new(BeamDirection.Bottom, 0, x, y));
                    }
                }
                else if (direction == BeamDirection.Top)
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
                        direction = BeamDirection.Right;
                    }
                    else if ((tile & Tile.MirrorTopLeftBottomRight) != 0)
                    {
                        direction = BeamDirection.Left;
                    }
                    else if ((tile & Tile.MirrorLeftRight) != 0)
                    {
                        // We change to a right beam
                        direction = BeamDirection.Right;
                        // And we add a left beam
                        beams.Push(new(BeamDirection.Left, 0, x, y));
                    }
                }
                else if (direction == BeamDirection.Bottom)
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
                        direction = BeamDirection.Left;
                    }
                    else if ((tile & Tile.MirrorTopLeftBottomRight) != 0)
                    {
                        direction = BeamDirection.Right;
                    }
                    else if ((tile & Tile.MirrorLeftRight) != 0)
                    {
                        // We change to a right beam
                        direction = BeamDirection.Right;
                        // And we add a left beam
                        beams.Push(new(BeamDirection.Left, 0, x, y));
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

        [Flags]
        private enum BeamDirection
        {
            Left = 0b0001_0000,
            Right = 0b0010_0000,
            Bottom = 0b0100_0000,
            Top = 0b1000_0000,
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

        private readonly record struct Beam(BeamDirection Direction, int EnergizedCount, int X, int Y);
    }
}
