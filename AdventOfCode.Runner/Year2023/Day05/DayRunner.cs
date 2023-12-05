namespace AdventOfCode.Runner.Year2023.Day05
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

            foreach (var seed in seeds)
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
            ReadOnlySpan<string> lineSpan = lines.AsSpan();

            // Max of 20 seeds
            Span<(long Lower, long Range)> seedPairsBuffer = stackalloc (long, long)[10];

            int offset = 0;

            int numberOfSeedPairs = ParseSeedPairs(lineSpan[offset], seedPairsBuffer);
            ReadOnlySpan<(long Lower, long Range)> seedPairs = seedPairsBuffer[..numberOfSeedPairs];

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

            Span<(long Lower, long Range)> accumulator1 = stackalloc (long, long)[45];
            Span<(long Lower, long Range)> accumulator2 = stackalloc (long, long)[45];
            Span<(long Lower, long Range)> accumulator3 = stackalloc (long, long)[45];
            Span<(long Lower, long Range)> accumulator4 = stackalloc (long, long)[45];
            Span<(long Lower, long Range)> accumulator5 = stackalloc (long, long)[45];
            Span<(long Lower, long Range)> accumulator6 = stackalloc (long, long)[45];
            Span<(long Lower, long Range)> accumulator7 = stackalloc (long, long)[45];

            foreach (var seedPair in seedPairs)
            {
                int soil = Mapper.GetDestinationRange(seedToSoil, seedPair, accumulator1);
                foreach (var soilPair in accumulator1[..soil])
                {
                    int fertilizer = Mapper.GetDestinationRange(soilToFertilizer, soilPair, accumulator2);
                    foreach (var fertilizerPair in accumulator2[..fertilizer])
                    {
                        int water = Mapper.GetDestinationRange(fertilizerToWater, fertilizerPair, accumulator3);
                        foreach (var waterPair in accumulator3[..water])
                        {
                            int light = Mapper.GetDestinationRange(waterToLight, waterPair, accumulator4);

                            foreach (var lightPair in accumulator4[..light])
                            {
                                int temperature = Mapper.GetDestinationRange(lightToTemperature, lightPair, accumulator5);

                                foreach (var temperaturePair in accumulator5[..temperature])
                                {
                                    int humidity = Mapper.GetDestinationRange(temperatureToHumidity, temperaturePair, accumulator6);

                                    foreach (var humidityPair in accumulator6[..humidity])
                                    {
                                        int location = Mapper.GetDestinationRange(humidityToLocation, humidityPair, accumulator7);
                                        foreach(var locationPair in accumulator7[..location])
                                        {
                                            if (locationPair.Lower < result)
                                            {
                                                result = locationPair.Lower;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            formatter.Format(result);
        }


        private int ParseSeedPairs(ReadOnlySpan<char> line, Span<(long Lower, long Range)> seedsBuffer)
        {
            int offset = 7;
            int index = line[offset..].IndexOf(' ');
            int outputIndex = 0;

            long lower = -1;
            while (index != -1)
            {
                if (lower == -1)
                {
                    lower = long.Parse(line[offset..(offset + index)]);
                }
                else
                {
                    seedsBuffer[outputIndex++] = (lower, long.Parse(line[offset..(offset + index)]));
                    lower = -1;
                }

                offset += index + 1;
                index = line[offset..].IndexOf(' ');
            }

            // And the last one
            seedsBuffer[outputIndex++] = (lower, long.Parse(line[offset..]));

            return outputIndex;
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
