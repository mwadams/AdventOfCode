namespace AdventOfCode.Runner.Year2023.Day19
{
    using System;
    using System.Runtime.CompilerServices;

    public static class Part1
    {
        public enum Result
        {
            Continue,
            Accept,
            Reject,
        }

        public static Part GetPart(ReadOnlySpan<char> line)
        {
            int start = 1;
            int end = line[start..].IndexOf(',') + start;
            int x = int.Parse(line[(start + 2)..end]);
            start = end + 1;
            end = line[start..].IndexOf(',') + start;
            int m = int.Parse(line[(start + 2)..end]);
            start = end + 1;
            end = line[start..].IndexOf(',') + start;
            int a = int.Parse(line[(start + 2)..end]);
            start = end + 1;
            int s = int.Parse(line[(start + 2)..^1]);

            return new(x, m, a, s);
        }

        private static readonly int inLabel = MakeLabel("in");

        public static WorkflowStep BuildWorkflowSteps(ReadOnlySpan<string> lines, Dictionary<int, WorkflowStep> steps)
        {
            
            foreach (ReadOnlySpan<char> line in lines)
            {

                if (line.IsEmpty)
                {
                    return steps[inLabel];
                }

                int labelIndex = line.IndexOf('{');

                int label = MakeLabel(line[..labelIndex]);
                
                WorkflowStep step = Identity();

                int index = labelIndex + 1;

                Property property = Property.None;
                Operator @operator = Operator.None;
                int comparisonValue = 0;
                ReadOnlySpan<char> result = default;

                while (index < line.Length)
                {
                    switch (line[index])
                    {
                        case ',':
                            step = Bind(step, CreateStep(property, @operator, comparisonValue, result));
                            index++;
                            property = Property.None;
                            @operator = Operator.None;
                            comparisonValue = 0;
                            result = default;
                            break;
                        case '}':
                            step = Bind(step, CreateStep(property, @operator, comparisonValue, result));
                            steps.Add(label, step);
                            step = Identity();
                            index++;
                            property = Property.None;
                            @operator = Operator.None;
                            comparisonValue = 0;
                            result = default;
                            break;
                        case 'x':
                            index = SetResultOrProperty(line, index, steps, Property.X, out result, out property);
                            break;
                        case 'm':
                            index = SetResultOrProperty(line, index, steps, Property.M, out result, out property);
                            break;
                        case 'a':
                            index = SetResultOrProperty(line, index, steps, Property.A, out result, out property);
                            break;
                        case 's':
                            index = SetResultOrProperty(line, index, steps, Property.S, out result, out property);
                            break;
                        case '<':
                            @operator = Operator.LessThan;
                            index = GetComparison(line, index, out comparisonValue);
                            break;
                        case '>':
                            @operator = Operator.GreaterThan;
                            index = GetComparison(line, index, out comparisonValue);
                            break;
                        case ':':
                            int nextIndex = line[(index + 1)..].IndexOfAny(",}") + index + 1;
                            result = line[(index + 1)..nextIndex];
                            index = nextIndex;
                            break;
                        default:
                            index = SetResult(line, index, out result);
                            break;

                    }
                }
            }

            throw new InvalidOperationException("Expected to see a blank line");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MakeLabel(ReadOnlySpan<char> span)
        {
            int label = 0;
            
            for(int index =0; index < span.Length; ++index)
            {
                label += span[index] << (8 * index);
            }

            return label;
        }

        private static int SetResult(ReadOnlySpan<char> line, int index, out ReadOnlySpan<char> result)
        {
            int nextIndex = line[index..].IndexOfAny(",}") + index;
            result = line[index..nextIndex];
            return nextIndex;
        }

        private static int SetResultOrProperty(
            ReadOnlySpan<char> line,
            int index,
            Dictionary<int, WorkflowStep> steps,
            Property property,
            out ReadOnlySpan<char> result,
            out Property propertyResult)
        {
            char nextChar = line[index + 1];

            if (nextChar == '<' || nextChar == '>')
            {
                result = default;
                propertyResult = property;
                return index + 1;
            }

            int nextIndex = line[index..].IndexOfAny(",}") + index;
            result = line[index..nextIndex];
            propertyResult = Property.None;
            return nextIndex;

        }

        private static int GetComparison(ReadOnlySpan<char> line, int index, out int comparisonValue)
        {
            int startIndex = index + 1;
            int endIndex = line[startIndex..].IndexOf(":") + startIndex;
            comparisonValue = int.Parse(line[startIndex..endIndex]);
            return endIndex;
        }

        private static WorkflowStep CreateStep(
            Property building,
            Operator operation,
            int comparison,
            ReadOnlySpan<char> result)
        {
            char c = result.Length == 1 ? result[0] : '\0';
            return (building, operation, c) switch
            {
                (Property.X, Operator.LessThan, 'A') => AcceptXLessThan(comparison),
                (Property.X, Operator.GreaterThan, 'A') => AcceptXGreaterThan(comparison),
                (Property.M, Operator.LessThan, 'A') => AcceptMLessThan(comparison),
                (Property.M, Operator.GreaterThan, 'A') => AcceptMGreaterThan(comparison),
                (Property.A, Operator.LessThan, 'A') => AcceptALessThan(comparison),
                (Property.A, Operator.GreaterThan, 'A') => AcceptAGreaterThan(comparison),
                (Property.S, Operator.LessThan, 'A') => AcceptSLessThan(comparison),
                (Property.S, Operator.GreaterThan, 'A') => AcceptSGreaterThan(comparison),
                (Property.X, Operator.LessThan, 'R') => RejectXLessThan(comparison),
                (Property.X, Operator.GreaterThan, 'R') => RejectXGreaterThan(comparison),
                (Property.M, Operator.LessThan, 'R') => RejectMLessThan(comparison),
                (Property.M, Operator.GreaterThan, 'R') => RejectMGreaterThan(comparison),
                (Property.A, Operator.LessThan, 'R') => RejectALessThan(comparison),
                (Property.A, Operator.GreaterThan, 'R') => RejectAGreaterThan(comparison),
                (Property.S, Operator.LessThan, 'R') => RejectSLessThan(comparison),
                (Property.S, Operator.GreaterThan, 'R') => RejectSGreaterThan(comparison),
                (Property.X, Operator.LessThan, _) => ProcessXLessThan(comparison, MakeLabel(result)),
                (Property.X, Operator.GreaterThan, _) => ProcessXGreaterThan(comparison, MakeLabel(result)),
                (Property.M, Operator.LessThan, _) => ProcessMLessThan(comparison, MakeLabel(result)),
                (Property.M, Operator.GreaterThan, _) => ProcessMGreaterThan(comparison, MakeLabel(result)),
                (Property.A, Operator.LessThan, _) => ProcessALessThan(comparison, MakeLabel(result)),
                (Property.A, Operator.GreaterThan, _) => ProcessAGreaterThan(comparison, MakeLabel(result)),
                (Property.S, Operator.LessThan, _) => ProcessSLessThan(comparison, MakeLabel(result)),
                (Property.S, Operator.GreaterThan, _) => ProcessSGreaterThan(comparison, MakeLabel(result)),
                (_, _, 'A') => Accept(),
                (_, _, 'R') => Reject(),
                _ => Process(MakeLabel(result)),
            };
        }

        private enum Property
        {
            None,
            X,
            M,
            A,
            S,
        }

        private enum Operator
        {
            None,
            LessThan,
            GreaterThan,
        }

        private static WorkflowStep Bind(WorkflowStep first, WorkflowStep second) =>
            state =>
            {
                state = first(state);
                if (state.Result == Result.Continue)
                {
                    return second(state);
                }

                return state;
            };

        private static WorkflowStep Identity() =>
            state => state;

        private static WorkflowStep Accept() =>
            state => state with { Result = Result.Accept };

        private static WorkflowStep Reject() =>
            state => state with { Result = Result.Reject };

        private static WorkflowStep Process(int processor) =>
            state => state.Steps[processor](state);

        private static WorkflowStep AcceptXLessThan(int val) =>
            state => state.Part.X < val ? state with { Result = Result.Accept } : state;

        private static WorkflowStep AcceptXGreaterThan(int val) =>
            state => state.Part.X > val ? state with { Result = Result.Accept } : state;

        private static WorkflowStep AcceptMLessThan(int val) =>
            state => state.Part.M < val ? state with { Result = Result.Accept } : state;

        private static WorkflowStep AcceptMGreaterThan(int val) =>
            state => state.Part.M > val ? state with { Result = Result.Accept } : state;

        private static WorkflowStep AcceptALessThan(int val) =>
            state => state.Part.A < val ? state with { Result = Result.Accept } : state;

        private static WorkflowStep AcceptAGreaterThan(int val) =>
            state => state.Part.A > val ? state with { Result = Result.Accept } : state;

        private static WorkflowStep AcceptSLessThan(int val) =>
            state => state.Part.S < val ? state with { Result = Result.Accept } : state;

        private static WorkflowStep AcceptSGreaterThan(int val) =>
            state => state.Part.S > val ? state with { Result = Result.Accept } : state;

        private static WorkflowStep RejectXLessThan(int val) =>
            state => state.Part.X < val ? state with { Result = Result.Reject } : state;

        private static WorkflowStep RejectXGreaterThan(int val) =>
            state => state.Part.X > val ? state with { Result = Result.Reject } : state;

        private static WorkflowStep RejectMLessThan(int val) =>
            state => state.Part.M < val ? state with { Result = Result.Reject } : state;

        private static WorkflowStep RejectMGreaterThan(int val) =>
            state => state.Part.M > val ? state with { Result = Result.Reject } : state;

        private static WorkflowStep RejectALessThan(int val) =>
            state => state.Part.A < val ? state with { Result = Result.Reject } : state;

        private static WorkflowStep RejectAGreaterThan(int val) =>
            state => state.Part.A > val ? state with { Result = Result.Reject } : state;

        private static WorkflowStep RejectSLessThan(int val) =>
            state => state.Part.S < val ? state with { Result = Result.Reject } : state;

        private static WorkflowStep RejectSGreaterThan(int val) =>
            state => state.Part.S > val ? state with { Result = Result.Reject } : state;

        private static WorkflowStep ProcessXLessThan(int val, int processor) =>
            state => state.Part.X < val ? state.Steps[processor](state) : state;

        private static WorkflowStep ProcessXGreaterThan(int val, int processor) =>
            state => state.Part.X > val ? state.Steps[processor](state) : state;

        private static WorkflowStep ProcessMLessThan(int val, int processor) =>
            state => state.Part.M < val ? state.Steps[processor](state) : state;

        private static WorkflowStep ProcessMGreaterThan(int val, int processor) =>
            state => state.Part.M > val ? state.Steps[processor](state) : state;

        private static WorkflowStep ProcessALessThan(int val, int processor) =>
            state => state.Part.A < val ? state.Steps[processor](state) : state;

        private static WorkflowStep ProcessAGreaterThan(int val, int processor) =>
            state => state.Part.A > val ? state.Steps[processor](state) : state;

        private static WorkflowStep ProcessSLessThan(int val, int processor) =>
            state => state.Part.S < val ? state.Steps[processor](state) : state;

        private static WorkflowStep ProcessSGreaterThan(int val, int processor) =>
            state => state.Part.S > val ? state.Steps[processor](state) : state;

        public delegate State WorkflowStep(State state);

        public readonly struct State
        {
            private State(Part part, Result result, Dictionary<int, WorkflowStep> steps)
            {
                Part = part;
                Result = result;
                Steps = steps;
            }

            public Part Part { get; init; }

            public Result Result { get; init; }

            public Dictionary<int, WorkflowStep> Steps { get; init; }

            public static State For(Part part, Dictionary<int, WorkflowStep> steps)
            {
                return new(part, Result.Continue, steps);
            }
        }

        public readonly record struct Part(int X, int M, int A, int S);
    }
}
