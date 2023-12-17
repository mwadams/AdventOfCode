namespace AdventOfCode.Runner.Year2023.Day17
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
            var lines = await InputReader.ReadLines(2023, 17, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            int width = lines[0].Length;
            int height = lines.Length;

            Span<int> map = stackalloc int[width * height];

            BuildMap(lines, map, width, height);

            Dictionary<Vertex, int> distances = [];
            PriorityQueue<Vertex, int> work = new();

            // You don't incur the first block's heat loss
            // But you can head off right, or head off down.
            distances[(0, 0, Direction.East, 0)] = 0;
            distances[(0, 0, Direction.South, 0)] = 0;
            work.Enqueue((0, 0, Direction.East, 0), 0);
            work.Enqueue((0, 0, Direction.South, 0), 0);

            result = ProcessWork(work, distances, map, width, height, 0, 3);

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            int result = 0;

            int width = lines[0].Length;
            int height = lines.Length;

            Span<int> map = stackalloc int[width * height];

            BuildMap(lines, map, width, height);

            Dictionary<Vertex, int> distances = [];
            PriorityQueue<Vertex, int> work = new();

            // You don't incur the first block's heat loss
            // But you can head off right, or head off down.
            distances[(0, 0, Direction.East, 0)] = 0;
            distances[(0, 0, Direction.South, 0)] = 0;
            work.Enqueue((0, 0, Direction.East, 0), 0);
            work.Enqueue((0, 0, Direction.South, 0), 0);

            result = ProcessWork(work, distances, map, width, height, 4, 10);

            formatter.Format(result);
        }

        private static int ProcessWork(PriorityQueue<Vertex, int> work, Dictionary<Vertex, int> distances, Span<int> map, int width, int height, int minBeforeTurn, int maxLength)
        {
            while (work.TryDequeue(out Vertex current, out int heatLossToGetThere))
            {
                // You need to have moved at least 4
                if (current.X == width - 1 && current.Y == height - 1 && current.RunLength >= 4)
                {
                    return heatLossToGetThere;
                }

                int currentHeatLoss = distances[current];

                (int x1, int y1, _, _) = current;
                // Reset and advance in the direction of the run
                if (current.RunLength < maxLength)
                {
                    ApplyStep(current.Direction, ref x1, ref y1);
                    EnqueueWork(currentHeatLoss, (x1, y1, current.Direction, current.RunLength + 1), map, width, height, distances, work);
                }

                if (current.RunLength >= minBeforeTurn)
                {
                    (x1, y1, Direction newDirection, _) = current;
                    // Now turn clockwise
                    ApplyClockwiseTurn(ref newDirection, ref x1, ref y1);
                    EnqueueWork(currentHeatLoss, (x1, y1, newDirection, 1), map, width, height, distances, work);

                    // Reset and turn anticlockwise
                    (x1, y1, newDirection, _) = current;
                    ApplyAnticlockwiseTurn(ref newDirection, ref x1, ref y1);
                    EnqueueWork(currentHeatLoss, (x1, y1, newDirection, 1), map, width, height, distances, work);
                }
            }

            return -1;
        }

        private static void EnqueueWork(int currentHeatLoss, in Vertex next, Span<int> map, int width, int height, Dictionary<Vertex, int> distances, PriorityQueue<Vertex, int> work)
        {
            if (next.X >= 0 && next.X < width && next.Y >= 0 && next.Y < height)
            {
                int nextHeatLoss = currentHeatLoss + map[next.Y * width + next.X];
                if (nextHeatLoss < distances.GetValueOrDefault(next, int.MaxValue))
                {
                    distances[next] = nextHeatLoss;
                    // Short-circuit by assuming that the minimum cost is when there is a 1
                    // in every tile in the shortest path to the terminus
                    work.Enqueue(next, nextHeatLoss + ManhattanDistance(next.X, next.Y, width - 1, height - 1));
                }
            }
        }

        private static void BuildMap(string[] lines, Span<int> map, int width, int height)
        {
            int offset = 0;
            foreach (ReadOnlySpan<char> line in lines)
            {
                foreach (char c in line)
                {
                    map[offset++] = c - '0';
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        private static void ApplyAnticlockwiseTurn(ref Direction direction, ref int x, ref int y)
        {
            (direction, x, y) =
                direction switch
                {
                    Direction.North => (Direction.West, x - 1, y),
                    Direction.West => (Direction.South, x, y + 1),
                    Direction.South => (Direction.East, x + 1, y),
                    Direction.East => (Direction.North, x, y - 1),
                    _ => throw new ArgumentException($"Unexpected direction {direction}", nameof(direction))
                };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        private static void ApplyClockwiseTurn(ref Direction direction, ref int x, ref int y)
        {
            (direction, x, y) =
                direction switch
                {
                    Direction.North => (Direction.East, x + 1, y),
                    Direction.West => (Direction.North, x, y - 1),
                    Direction.South => (Direction.West, x - 1, y),
                    Direction.East => (Direction.South, x, y + 1),
                    _ => throw new ArgumentException($"Unexpected direction {direction}", nameof(direction))
                };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        private static void ApplyStep(Direction direction, ref int x, ref int y)
        {
            (x, y) =
                direction switch
                {
                    Direction.North => (x, y - 1),
                    Direction.West => (x - 1, y),
                    Direction.South => (x, y + 1),
                    Direction.East => (x + 1, y),
                    _ => throw new ArgumentException($"Unexpected direction {direction}", nameof(direction))
                };
        }

        private static int ManhattanDistance(int x1, int y1, int x2, int y2)
        {
            int x = Math.Abs(x1 - x2);
            int y = Math.Abs(y1 - y2);
            return x + y;
        }

        private readonly record struct Vertex(int X, int Y, Direction Direction, int RunLength)
        {
            public static implicit operator (int X, int Y, Direction Direction, int RunLength)(Vertex value)
            {
                return (value.X, value.Y, value.Direction, value.RunLength);
            }

            public static implicit operator Vertex((int X, int Y, Direction Direction, int RunLength) value)
            {
                return new Vertex(value.X, value.Y, value.Direction, value.RunLength);
            }
        }

        private enum Direction
        {
            North,
            West,
            South,
            East,
        }
    }
}
