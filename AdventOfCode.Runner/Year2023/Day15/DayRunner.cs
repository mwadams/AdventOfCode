namespace AdventOfCode.Runner.Year2023.Day15
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
            var lines = await InputReader.ReadLines(2023, 15, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            long current = 0;

            foreach (ReadOnlySpan<char> line in lines)
            {
                foreach (char c in line)
                {
                    if (c == ',')
                    {
                        result += current;
                        current = 0;
                        continue;
                    }

                    current += c;
                    current *= 17;
                    current %= 256;
                }
            }

            result += current;

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            Span<Bucket> lenses = new Bucket[256];

            foreach (ReadOnlySpan<char> line in lines)
            {
                long key = 0;

                int startFocalLengthIndex = 0;
                int startLabelIndex = 0;
                int afterLabelIndex = 0;
                int index = 0;
                bool add = false;

                foreach (char c in line)
                {
                    if (c == ',')
                    {
                        CommitChange(
                            lenses,
                            line,
                            key,
                            startFocalLengthIndex,
                            startLabelIndex,
                            afterLabelIndex,
                            index,
                            add);

                        // apply result
                        add = false;
                        key = 0;
                        startLabelIndex = index + 1;

                    }
                    else if (c == '=')
                    {
                        afterLabelIndex = index;
                        startFocalLengthIndex = index + 1;
                        add = true;
                    }
                    else if (c == '-')
                    {
                        // remove the key from the dictionary
                        afterLabelIndex = index;
                        add = false;
                    }
                    else if (!add)
                    {
                        key += c;
                        key *= 17;
                        key %= 256;
                    }

                    index++;

                }

                // Commit the last change
                CommitChange(
                    lenses,
                    line,
                    key,
                    startFocalLengthIndex,
                    startLabelIndex,
                    afterLabelIndex,
                    index,
                    add);
            }

            for (int i = 0; i < 256; ++i)
            {
                ref Bucket bucket = ref lenses[i];
                for (int j = 0; j < bucket.Count; ++j)
                {
                    result += (i + 1) * (j + 1) * bucket.Lenses[j].Power;
                }
            }

            formatter.Format(result);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void CommitChange(Span<Bucket> lenses, ReadOnlySpan<char> line, long key, int startFocalLengthIndex, int startLabelIndex, int afterLabelIndex, int index, bool add)
            {
                ref Bucket bucket = ref lenses[(int)key]; // It ends up as <256 but needs to be computed as a long

                if (add)
                {
                    int focalPower = int.Parse(line[startFocalLengthIndex..index]);
                    AddLens(line, startLabelIndex, afterLabelIndex, focalPower, ref bucket);
                }
                else
                {
                    RemoveLens(line, startLabelIndex, afterLabelIndex, ref bucket);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RemoveLens(ReadOnlySpan<char> line, int labelStart, int labelEnd, ref Bucket bucket)
        {
            bool removing = false;
            ReadOnlySpan<char> label = line[labelStart..labelEnd];

            for (int i = 0; i < bucket.Count; ++i)
            {
                ref Lens lens = ref bucket.Lenses[i];
                if (!removing)
                {
                    if (line[lens.LabelStart..lens.LabelEnd].SequenceEqual(label))
                    {
                        removing = true;
                    }
                }
                else
                {
                    bucket.Lenses[i - 1] = lens;
                }
            }

            if (removing)
            {
                // And we have one fewer item in the bucket.
                bucket.Count--;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddLens(ReadOnlySpan<char> line, int labelStart, int labelEnd, int focalPower, ref Bucket bucket)
        {
            if (bucket.Capacity == 0)
            {
                bucket = new Bucket(8, 0, new Lens[8]);
            }
            else
            {
                ReadOnlySpan<char> label = line[labelStart..labelEnd];
                for (int i = 0; i < bucket.Count; ++i)
                {
                    ref Lens lens = ref bucket.Lenses[i];
                    if (line[lens.LabelStart..lens.LabelEnd].SequenceEqual(label))
                    {
                        lens.Power = focalPower;
                        return;
                    }
                }

                if (bucket.Count == bucket.Capacity)
                {
                    // Need to resize the bucket
                    if (bucket.Capacity == 0)
                    {
                        bucket = new Bucket(8, 0, new Lens[8]);
                    }
                    else
                    {
                        int newCapacity = bucket.Capacity * 2;
                        Lens[] newLenses = new Lens[newCapacity];
                        bucket.Lenses.CopyTo(newLenses.AsSpan());
                        bucket.Lenses = newLenses;
                        bucket.Capacity = newCapacity;
                    }
                }
            }

            bucket.Lenses[bucket.Count] = new Lens(labelStart, labelEnd, focalPower);
            bucket.Count += 1;
        }

        private record struct Lens(int LabelStart, int LabelEnd, int Power);
        private record struct Bucket(int Capacity, int Count, Lens[] Lenses);
    }
}
