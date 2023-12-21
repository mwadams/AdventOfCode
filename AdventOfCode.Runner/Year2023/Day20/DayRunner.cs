namespace AdventOfCode.Runner.Year2023.Day20
{
    using AdventOfCode.Common;
    using System.Threading.Tasks;
    using static AdventOfCode.Runner.Year2023.Day20.DayRunner;

    public class DayRunner : IDay<DayRunner>
    {
        private readonly string[] lines;

        public DayRunner(string[] lines)
        {
            this.lines = lines;
        }

        public static async ValueTask<DayRunner> Initialize(bool test = false)
        {
            var lines = await InputReader.ReadLines(2023, 20, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            var system = new System(this.lines);

            for (int i = 0; i < 1000; ++i)
            {
                system.PushButton();
            }

            result = system.Low * system.High;

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;

            var system = new System(this.lines);

            IModule broadcaster = system.GetModule(System.BroadcasterKey)!;

            Span<long> counts = stackalloc long[broadcaster.Outputs.Length];

            Queue<int> keyQueue = new Queue<int>(12);

            for(int i = 0; i < broadcaster.Outputs.Length; ++i)
            {
                keyQueue.Enqueue(broadcaster.Outputs[i]);

                int index = 0;
                while(keyQueue.TryDequeue(out int key))
                {                 
                    IModule connected = system.GetModule(key)!;

                    if (connected is FlipFlop ff)
                    {
                        foreach(var output in ff.Outputs)
                        {
                            IModule next = system.GetModule(output)!;
                            if (next is FlipFlop)
                            {
                                keyQueue.Enqueue(output);
                            }
                            else
                            {
                                counts[i] |= 1L << index;
                            }
                        }
                    }

                    index++;
                }
            }

            result = LowestCommonMultiple(counts);

            formatter.Format(result);
        }

        private static long LowestCommonMultiple(ReadOnlySpan<long> counts)
        {
            return LowestCommonMultiple(counts[0], counts[1..]);
        }

        private static long LowestCommonMultiple(long a, ReadOnlySpan<long> counts)
        {
            if (counts.Length == 1)
            {
                return LowestCommonMultiple(a, counts[0]);
            }

            return LowestCommonMultiple(a, LowestCommonMultiple(counts[0], counts[1..]));
        }

        private static long GreatestCommonFactor(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private static long LowestCommonMultiple(long a, long b)
        {
            return (a / GreatestCommonFactor(a, b)) * b;
        }

        public class System
        {
            private Dictionary<int, IModule> modules;

            private Queue<(int Source, Pulse Pulse, int Output)> pulses = [];

            private long countHigh;
            private long countLow;

            public static readonly int BroadcasterKey = MakeKey("broadcaster");
            public static readonly int ButtonKey = MakeKey("button");


            public System(ReadOnlySpan<string> lines)
            {
                modules = new(lines.Length);
                foreach (ReadOnlySpan<char> line in lines)
                {
                    AddModule(line);
                }

                foreach(KeyValuePair<int, IModule> module in modules)
                {
                    module.Value.SetOutputs();
                }
            }

            public long Low => this.countLow;
            public long High => this.countHigh;

            public long Result => this.countHigh * this.countLow;

            private void AddModule(ReadOnlySpan<char> line)
            {
                if (line[0] == '%')
                {
                    AddFlipFlop(line[1..]);
                }
                else if (line[0] == '&')
                {
                    AddConjunction(line[1..]);
                }
                else
                {
                    AddBroadcaster(line);
                }
            }

            private void AddFlipFlop(ReadOnlySpan<char> line)
            {
                (int outputsIndex, int key) = MakeKeyForName(line);
                int[] outputs = ParseOutputs(line[outputsIndex..]);
                this.modules[key] = new FlipFlop(key, outputs, this);
            }

            private void AddConjunction(ReadOnlySpan<char> line)
            {
                (int outputsIndex, int key) = MakeKeyForName(line);
                int[] outputs = ParseOutputs(line[outputsIndex..]);
                this.modules[key] = new Conjunction(key, outputs, this);
            }

            private void AddBroadcaster(ReadOnlySpan<char> line)
            {
                (int outputsIndex, int key) = MakeKeyForName(line);
                int[] outputs = ParseOutputs(line[outputsIndex..]);
                this.modules[key] = new Broadcaster(key, outputs, this);
            }

            private static int[] ParseOutputs(ReadOnlySpan<char> line)
            {
                Span<int> outputs = stackalloc int[10];

                int count = 0;
                int index = 0;
                int currentIndex = 0;
                while (currentIndex < line.Length)
                {
                    if (line[currentIndex] == ',')
                    {
                        outputs[count++] = MakeKey(line[index..currentIndex]);
                        index = currentIndex + 2; // Skip the space.
                        currentIndex = index;
                    }
                    else
                    {
                        currentIndex++;
                    }
                }

                outputs[count++] = MakeKey(line[index..]);

                int[] result = new int[count];
                outputs[..count].CopyTo(result);
                return result;
            }

            private static (int OutputsIndex, int Key) MakeKeyForName(ReadOnlySpan<char> line)
            {
                int index = line.IndexOf('-');
                return (index + 3, MakeKey(line[..(index - 1)]));
            }

            public void PushButton(int Key = -1)
            {
                SendPulse(ButtonKey, Pulse.Low, [Key == -1 ? BroadcasterKey : Key]);

                while (this.pulses.TryDequeue(out (int Source, Pulse Pulse, int Output) pulse))
                {
                    // There may be no module of that name e.g. "output" which just terminates
                    // so TryGet() is used.
                    if (this.modules.TryGetValue(pulse.Output, out IModule? value))
                    {
                        var module = this.modules[pulse.Output];
                        module.SetInput(pulse.Source, pulse.Pulse);
                    }
                }
            }

            // Send a pulse to the given outputs
            public void SendPulse(int source, Pulse pulse, int[] outputs)
            {
                if (pulse == Pulse.High)
                {
                    this.countHigh += outputs.Length;
                }
                else
                {
                    this.countLow += outputs.Length;
                }

                foreach (var output in outputs)
                {
                    this.pulses.Enqueue((source, pulse, output));
                }
            }

            internal IModule? GetModule(int key)
            {
                return this.modules.TryGetValue(key, out IModule? value) ? value : null;
            }

            internal void Reset()
            {
                foreach(var module in this.modules.Values)
                {
                    module.Reset();
                }
            }
        }

        private static string GetName(int key)
        {
            char[] name = new char[4];

            for (int i = 0; i < 4; ++i)
            {
                name[i] = (char)(byte)(key >> (8 * i));
                if (name[i] == 0)
                {
                    return string.Create(i + 1, (name, i), (span, c) => c.name.AsSpan()[..c.i].CopyTo(span));
                }
            }

            return string.Create(4, name, (span, c) => name.AsSpan().CopyTo(span));
        }

        public enum Pulse
        {
            High,
            Low
        }

        public interface IModule
        {
            void SetInput(int source, Pulse pulse);

            void AddInput(int source);

            void SetOutputs();
            void Reset();

            public int[] Outputs { get; }
        }

        public class FlipFlop(int Key, int[] Outputs, System System) : IModule
        {
            public bool State { get; private set; }
            public int Key { get; private set; } = Key;
            public int[] Outputs { get; private set; } = Outputs;

            public System System { get; private set; } = System;

            public void AddInput(int source)
            {
                // NOP
            }

            public void Reset()
            {
                this.State = false;
            }

            public void SetOutputs()
            {
                foreach(var output in Outputs)
                {
                    this.System.GetModule(output)?.AddInput(this.Key);
                }
            }

            public void SetInput(int source, Pulse pulse)
            {
                if (pulse == Pulse.Low)
                {
                    if (State)
                    {
                        this.State = false;
                        this.System.SendPulse(this.Key, Pulse.Low, this.Outputs);
                    }
                    else
                    {
                        this.State = true;
                        this.System.SendPulse(this.Key, Pulse.High, this.Outputs);
                    }
                }
            }
        }

        public class Broadcaster(int Key, int[] Outputs, System System) : IModule
        {
            public int Key { get; private set; } = Key;
            public int[] Outputs { get; private set; } = Outputs;
            public System System { get; private set; } = System;

            public void AddInput(int source)
            {
                // NOP
            }

            public void Reset()
            {
            }

            public void SetOutputs()
            {
                foreach (var output in Outputs)
                {
                    this.System.GetModule(output)?.AddInput(this.Key);
                }
            }

            public void SetInput(int source, Pulse pulse)
            {
                this.System.SendPulse(this.Key, pulse, this.Outputs);
            }
        }

        public class Conjunction(int Key, int[] Outputs, System System) : IModule
        {
            private readonly int[] inputs = new int[8];
            private int inputCount = 0;
            private uint mask = 0;
            public uint Bits { get; private set; }
            public int Key { get; private set; } = Key;
            public string KeyName => GetName(Key);
            public int[] Outputs { get; private set; } = Outputs;
            public System System { get; private set; } = System;
            public ReadOnlySpan<int> Inputs => this.inputs.AsSpan()[..inputCount];

            public void AddInput(int source)
            {
                this.inputs[this.inputCount] = source;
                this.mask |= 1U << this.inputCount;
                this.inputCount++;
            }

            public void Reset()
            {
                this.Bits = 0;
            }

            public void SetOutputs()
            {
                foreach (var output in Outputs)
                {
                    this.System.GetModule(output)?.AddInput(this.Key);
                }
            }

            public void SetInput(int source, Pulse pulse)
            {
                int index = this.Inputs.IndexOf(source);

                if (pulse == Pulse.High)
                {
                    this.Bits |= 1U << index;
                }
                else
                {
                    this.Bits &= ~(1U << index);
                }

                if ((this.Bits ^ this.mask)  == 0)
                {
                    this.System.SendPulse(this.Key, Pulse.Low, this.Outputs);
                }
                else
                {
                    this.System.SendPulse(this.Key, Pulse.High, this.Outputs);
                }
            }
        }

        private static int MakeKey(ReadOnlySpan<char> name)
        {
            int result = 0;
            for (int i = 0; i < Math.Min(name.Length, 4); ++i)
            {
                result += name[i] << (i * 8);
            }

            return result;
        }
    }
}
