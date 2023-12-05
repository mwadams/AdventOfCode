namespace AdventOfCode.Runner.Year2023.Day05
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
            var lines = await InputReader.ReadLines(2023, 5, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            ReadOnlySpan<string> lineSpan = lines.AsSpan();

            // Max of 20 seeds
            Span<long> seedsBuffer = stackalloc long[20];

            int offset = 0;

            int numberOfSeeds = ParseSeeds(lineSpan[offset], seedsBuffer);
            ReadOnlySpan<long> seeds = seedsBuffer[..numberOfSeeds];

            offset += 3;

            Span<Mapper> seedToSoilBuffer = stackalloc Mapper[45];
            int numberOfSeedToSoil = ParseBuffer(lineSpan[offset..], seedToSoilBuffer);
            ReadOnlySpan<Mapper> seedToSoil = seedToSoilBuffer[..numberOfSeedToSoil];

            offset += 2 + numberOfSeedToSoil;

            Span<Mapper> soilToFertilizerBuffer = stackalloc Mapper[45];
            int numberOfSoilToFertilizer = ParseBuffer(lineSpan[offset..], soilToFertilizerBuffer);
            ReadOnlySpan<Mapper> soilToFertilizer = soilToFertilizerBuffer[..numberOfSoilToFertilizer];

            offset += 2 + numberOfSoilToFertilizer;

            Span<Mapper> fertilizerToWaterBuffer = stackalloc Mapper[45];
            int numberOfFertilizerToWater = ParseBuffer(lineSpan[offset..], fertilizerToWaterBuffer);
            ReadOnlySpan<Mapper> fertilizerToWater = fertilizerToWaterBuffer[..numberOfFertilizerToWater];

            offset += 2 + numberOfFertilizerToWater;

            Span<Mapper> waterToLightBuffer = stackalloc Mapper[45];
            int numberOfWaterToLight = ParseBuffer(lineSpan[offset..], waterToLightBuffer);
            ReadOnlySpan<Mapper> waterToLight = waterToLightBuffer[..numberOfWaterToLight];

            offset += 2 + numberOfWaterToLight;

            Span<Mapper> lightToTemperatureBuffer = stackalloc Mapper[45];
            int numberOfLightToTemperature = ParseBuffer(lineSpan[offset..], lightToTemperatureBuffer);
            ReadOnlySpan<Mapper> lightToTemperature = lightToTemperatureBuffer[..numberOfLightToTemperature];

            offset += 2 + numberOfLightToTemperature;

            Span<Mapper> temperatureToHumidityBuffer = stackalloc Mapper[45];
            int numberOfTemperatureToHumidity = ParseBuffer(lineSpan[offset..], temperatureToHumidityBuffer);
            ReadOnlySpan<Mapper> temperatureToHumidity = temperatureToHumidityBuffer[..numberOfTemperatureToHumidity];

            offset += 2 + numberOfTemperatureToHumidity;

            Span<Mapper> humidityToLocationBuffer = stackalloc Mapper[45];
            int numberOfHumidityToLocation = ParseBuffer(lineSpan[offset..], humidityToLocationBuffer);
            ReadOnlySpan<Mapper> humidityToLocation = humidityToLocationBuffer[..numberOfHumidityToLocation];

            long result = long.MaxValue;

            foreach(var seed in seeds)
            {
                long soil = Mapper.GetDestination(seedToSoil, seed);
                long fertilizer = Mapper.GetDestination(soilToFertilizer, soil);
                long water = Mapper.GetDestination(fertilizerToWater, fertilizer);
                long light = Mapper.GetDestination(waterToLight, water);
                long temperature = Mapper.GetDestination(lightToTemperature, light);
                long humidity = Mapper.GetDestination(temperatureToHumidity, temperature);
                long location = Mapper.GetDestination(humidityToLocation, humidity);

                if (location < result)
                {
                    result = location;
                }
            }

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            formatter.Format(result);
        }

        private int ParseSeeds(ReadOnlySpan<char> line, Span<long> seedsBuffer)
        {
            int offset = 7;
            int index = line[offset..].IndexOf(' ');
            int outputIndex = 0;

            while (index != -1)
            {
                seedsBuffer[outputIndex++] = long.Parse(line[offset..(offset + index)]);
                offset += index + 1;
                index = line[offset..].IndexOf(' ');
            }

            // And the last one
            seedsBuffer[outputIndex++] = long.Parse(line[offset..]);

            return outputIndex;
        }

        private int ParseBuffer(ReadOnlySpan<string> lines, Span<Mapper> buffer)
        {
            int lineIndex = 0;
            ReadOnlySpan<char> line = lines[lineIndex];

            while (line.Length > 0)
            {
                buffer[lineIndex] = ParseMapper(line);

                lineIndex++;

                if (lineIndex < lines.Length)
                {
                    line = lines[lineIndex];
                }
                else
                {
                    break;
                }
            }

            return lineIndex;
        }

        private Mapper ParseMapper(ReadOnlySpan<char> line)
        {
            int firstSpace = line.IndexOf(' ');
            long destination = long.Parse(line[..firstSpace]);
            int secondSpace = line[(firstSpace + 1)..].IndexOf(' ');
            long source = long.Parse(line.Slice(firstSpace + 1, secondSpace));
            long range = long.Parse(line[(firstSpace + secondSpace + 2)..]);

            return new(destination, source, range);
        }
    }
}
