namespace AdventOfCode.Benchmarks
{
    using AdventOfCode.Common;
    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public abstract class BenchmarkBase<TRunner>
        where TRunner : notnull, IDay<TRunner>
    {
        private TRunner? runner;

        /// <summary>
        /// Global setup.
        /// </summary>
        /// <returns>A <see cref="Task"/> which completes once cleanup is complete.</returns>
        [GlobalSetup]
        public async Task GlobalSetup()
        {
            this.runner = await TRunner.Initialize(false);
        }

        [Benchmark]
        public void RunPart1()
        {
            runner!.SolvePart1(NullFormatter.Instance);
        }

        [Benchmark]
        public void RunPart2()
        {
            runner!.SolvePart2(NullFormatter.Instance);
        }
    }
}
