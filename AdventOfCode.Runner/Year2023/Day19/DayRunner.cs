namespace AdventOfCode.Runner.Year2023.Day19
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
            var lines = await InputReader.ReadLines(2023, 19, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 0;
            Dictionary<string, Part1.WorkflowStep> steps = [];

            Part1.WorkflowStep initialStep = Part1.BuildWorkflowSteps(lines, steps);
            
            int startLine = steps.Count + 1;

            foreach(ReadOnlySpan<char> line in lines.AsSpan()[startLine..])
            {
                Part1.Part part = Part1.GetPart(line);
                Part1.State state = initialStep(Part1.State.For(part, steps));
                if (state.Result == Part1.Result.Accept)
                {
                    result += part.X + part.M + part.A + part.S;
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
    }
}
