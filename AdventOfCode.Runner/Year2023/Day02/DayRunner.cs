namespace AdventOfCode.Runner.Year2023.Day02
{
    using AdventOfCode.Common;
    using System;
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
            var lines = await InputReader.ReadLines(2023, 2, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            foreach(var line in lines)
            {
                result += GetIdOrZero(line, 12, 13, 14);
            }

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            foreach (var line in lines)
            {
                (long red, long green, long blue) = GetMinimumValuesFor(line);
                result += red * green * blue;
            }
            
            formatter.Format(result);
        }

        private static long GetIdOrZero(ReadOnlySpan<char> line, int maxRed, int maxGreen, int maxBlue)
        {
            // Skip "Game " and the first digit
            int indexOfColon = 6 + line[6..].IndexOf(':');
            // The span 6..indexOfColon is the id but we won't parse it til we need it

            // First digit is at indexOfColon + 2
            int currentIndex = indexOfColon + 2;

            while (true)
            {
                int indexOfSpace = currentIndex + line[currentIndex..].IndexOf(' ');

                // The span currentIndex..indexOfSpace is the number of the colour
                int numberOfCubes = int.Parse(line[currentIndex..indexOfSpace]);

                // The colour is the next character
                switch(line[indexOfSpace + 1])
                {
                    case 'r':
                        if (numberOfCubes > maxRed)
                        {
                            return 0;
                        }

                        currentIndex = indexOfSpace + 4;
                        break;

                    case 'g':
                        if (numberOfCubes > maxGreen)
                        {
                            return 0;
                        }

                        currentIndex = indexOfSpace + 6;
                        break;

                    case 'b':
                        if (numberOfCubes > maxBlue)
                        {
                            return 0;
                        }

                        currentIndex = indexOfSpace + 5;
                        break;

                    default:
                        throw new InvalidOperationException("Unknown colour.");
                }

                // We are now pointing at the next character after the colour name
                if (currentIndex == line.Length)
                {
                    // We reached the end of the line without baling out
                    return int.Parse(line[5..indexOfColon]);
                }

                // Advance past the comma or semi-colon
                currentIndex += 2;
            };
        }

        private static (long MinRed, long MinGreen, long MinBlue) GetMinimumValuesFor(ReadOnlySpan<char> line)
        {
            int maxRed = 0;
            int maxGreen = 0;
            int maxBlue = 0;

            // Skip "Game " and the first digit
            int indexOfColon = 6 + line[6..].IndexOf(':');
            // The span 6..indexOfColon is the id but we won't parse it til we need it

            // First digit is at indexOfColon + 2
            int currentIndex = indexOfColon + 2;

            while (true)
            {
                int indexOfSpace = currentIndex + line[currentIndex..].IndexOf(' ');

                // The span currentIndex..indexOfSpace is the number of the colour
                int numberOfCubes = int.Parse(line[currentIndex..indexOfSpace]);

                // The colour is the next character
                switch (line[indexOfSpace + 1])
                {
                    case 'r':
                        if (numberOfCubes > maxRed)
                        {
                            maxRed = numberOfCubes;
                        }

                        currentIndex = indexOfSpace + 4;
                        break;

                    case 'g':
                        if (numberOfCubes > maxGreen)
                        {
                            maxGreen = numberOfCubes;
                        }

                        currentIndex = indexOfSpace + 6;
                        break;

                    case 'b':
                        if (numberOfCubes > maxBlue)
                        {
                            maxBlue = numberOfCubes;
                        }

                        currentIndex = indexOfSpace + 5;
                        break;

                    default:
                        throw new InvalidOperationException("Unknown colour.");
                }

                // We are now pointing at the next character after the colour name
                if (currentIndex == line.Length)
                {
                    // We reached the end of the line without baling out
                    return (maxRed, maxGreen, maxBlue);
                }

                // Advance past the comma or semi-colon
                currentIndex += 2;
            };
        }
    }
}
