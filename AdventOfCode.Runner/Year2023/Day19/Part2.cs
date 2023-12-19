namespace AdventOfCode.Runner.Year2023.Day19
{
    using System.Runtime.CompilerServices;

    internal static class Part2
    {
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
                            step = Bind(step, CreateStep(label, property, @operator, comparisonValue, result));
                            index++;
                            property = Property.None;
                            @operator = Operator.None;
                            comparisonValue = 0;
                            result = default;
                            break;
                        case '}':
                            step = Bind(step, CreateStep(label, property, @operator, comparisonValue, result));
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MakeLabel(ReadOnlySpan<char> span)
        {
            int label = 0;

            for (int index = 0; index < span.Length; ++index)
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
            int label,
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

        public readonly struct State
        {
            private State(Range x, Range m, Range a, Range s, long total, Dictionary<int, WorkflowStep> steps)
            {
                X = x;
                M = m;
                A = a;
                S = s;
                Total = total;
                Steps = steps;
            }

            public Range X { get; init; }

            public Range M { get; init; }

            public Range A { get; init; }

            public Range S { get; init; }

            public long Total { get; init; }

            public Dictionary<int, WorkflowStep> Steps { get; init; }

            public static State For(Range range, Dictionary<int, WorkflowStep> steps)
            {
                return new(range, range, range, range, 0, steps);
            }
        }

        public delegate State WorkflowStep(State state);

        // The range has an inclusive Min and an exclusive Max
        public readonly record struct Range(int Min, int Max)
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Length() => Math.Max(0, Max - Min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long GetRangeCombinations(Range x, Range m, Range a, Range s)
        {
            return x.Length() * m.Length() * a.Length() * s.Length();
        }

        private static WorkflowStep Identity() =>
            state => state;

        private static WorkflowStep Accept() =>
            state => state with { Total = state.Total + GetRangeCombinations(state.X, state.M, state.A, state.S) };

        private static WorkflowStep Reject() =>
            state => state;

        private static WorkflowStep Process(int processor) =>
            state => state.Steps[processor](state);

        private static WorkflowStep AcceptXLessThan(int val) =>
            state =>
            {
                Range processRange = new(Math.Min(state.X.Min, val - 1), Math.Min(state.X.Max, val));
                Range newRange = new(Math.Max(state.X.Min, val), Math.Max(state.X.Max, val + 1));
                return state with { X = newRange, Total = state.Total + GetRangeCombinations(processRange, state.M, state.A, state.S) };
            };

        private static WorkflowStep AcceptXGreaterThan(int val) =>
            state =>
            {
                Range processRange = new(Math.Max(state.X.Min, val + 1), Math.Max(state.X.Max, val + 2));
                Range newRange = new(Math.Min(state.X.Min, val), Math.Min(state.X.Max, val + 1));
                return state with { X = newRange, Total = state.Total + GetRangeCombinations(processRange, state.M, state.A, state.S) };
            };

        private static WorkflowStep AcceptMLessThan(int val) =>
            state =>
            {
                Range processRange = new(Math.Min(state.M.Min, val - 1), Math.Min(state.M.Max, val));
                Range newRange = new(Math.Max(state.M.Min, val), Math.Max(state.M.Max, val + 1));
                return state with { M = newRange, Total = state.Total + GetRangeCombinations(state.X, processRange, state.A, state.S) };
            };

        private static WorkflowStep AcceptMGreaterThan(int val) =>
            state =>
            {
                Range processRange = new(Math.Max(state.M.Min, val + 1), Math.Max(state.M.Max, val + 2));
                Range newRange = new(Math.Min(state.M.Min, val), Math.Min(state.M.Max, val + 1));
                return state with { M = newRange, Total = state.Total + GetRangeCombinations(state.X, processRange, state.A, state.S) };
            };

        private static WorkflowStep AcceptALessThan(int val) =>
            state =>
            {
                Range processRange = new(Math.Min(state.A.Min, val - 1), Math.Min(state.A.Max, val));
                Range newRange = new(Math.Max(state.A.Min, val), Math.Max(state.A.Max, val + 1));
                return state with { A = newRange, Total = state.Total + GetRangeCombinations(state.X, state.M, processRange, state.S) };
            };

        private static WorkflowStep AcceptAGreaterThan(int val) =>
            state =>
            {
                Range processRange = new(Math.Max(state.A.Min, val + 1), Math.Max(state.A.Max, val + 2));
                Range newRange = new(Math.Min(state.A.Min, val), Math.Min(state.A.Max, val + 1));
                return state with { A = newRange, Total = state.Total + GetRangeCombinations(state.X, state.M, processRange, state.S) };
            };

        private static WorkflowStep AcceptSLessThan(int val) =>
            state =>
            {
                Range processRange = new(Math.Min(state.S.Min, val - 1), Math.Min(state.S.Max, val));
                Range newRange = new(Math.Max(state.S.Min, val), Math.Max(state.S.Max, val + 1));
                return state with { S = newRange, Total = state.Total + GetRangeCombinations(state.X, state.M, state.A, processRange) };
            };

        private static WorkflowStep AcceptSGreaterThan(int val) =>
            state =>
            {
                Range processRange = new(Math.Max(state.S.Min, val + 1), Math.Max(state.S.Max, val + 2));
                Range newRange = new(Math.Min(state.S.Min, val), Math.Min(state.S.Max, val + 1));
                return state with { S = newRange, Total = state.Total + GetRangeCombinations(state.X, state.M, state.A, processRange) };
            };

        private static WorkflowStep RejectXLessThan(int val) =>
            state =>
            {
                Range newRange = new(Math.Max(state.X.Min, val), Math.Max(state.X.Max, val + 1));
                return state with { X = newRange };
            };

        private static WorkflowStep RejectXGreaterThan(int val) =>
            state =>
            {
                Range newRange = new(Math.Min(state.X.Min, val), Math.Min(state.X.Max, val + 1));
                return state with { X = newRange };
            };

        private static WorkflowStep RejectMLessThan(int val) =>
            state =>
            {
                Range newRange = new(Math.Max(state.M.Min, val), Math.Max(state.M.Max, val + 1));
                return state with { M = newRange };
            };

        private static WorkflowStep RejectMGreaterThan(int val) =>
            state =>
            {
                Range newRange = new(Math.Min(state.M.Min, val), Math.Min(state.M.Max, val + 1));
                return state with { M = newRange };
            };

        private static WorkflowStep RejectALessThan(int val) =>
            state =>
            {
                Range newRange = new(Math.Max(state.A.Min, val), Math.Max(state.A.Max, val + 1));
                return state with { A = newRange };
            };

        private static WorkflowStep RejectAGreaterThan(int val) =>
            state =>
            {
                Range newRange = new(Math.Min(state.A.Min, val), Math.Min(state.A.Max, val + 1));
                return state with { A = newRange };
            };

        private static WorkflowStep RejectSLessThan(int val) =>
            state =>
            {
                Range newRange = new(Math.Max(state.S.Min, val), Math.Max(state.S.Max, val + 1));
                return state with { S = newRange };
            };

        private static WorkflowStep RejectSGreaterThan(int val) =>
            state =>
            {
                Range newRange = new(Math.Min(state.S.Min, val), Math.Min(state.S.Max, val + 1));
                return state with { S = newRange };
            };

        private static WorkflowStep ProcessXLessThan(int val, int processor) =>
            state =>
            {
                State currentState = state;
                Range processRange = new(Math.Min(state.X.Min, val - 1), Math.Min(state.X.Max, val));
                if (processRange.Length() > 0)
                {
                    currentState = state.Steps[processor](state with { X = processRange });
                }

                // This is the range that's greater than or equal to the value 
                Range newRange = new(Math.Max(state.X.Min, val), Math.Max(state.X.Max, val + 1));
                return state with { X = newRange, Total = currentState.Total };
            };

        private static WorkflowStep ProcessXGreaterThan(int val, int processor) =>
            state =>
            {
                State currentState = state;
                Range processRange = new(Math.Max(state.X.Min, val + 1), Math.Max(state.X.Max, val + 2));
                if (processRange.Length() > 0)
                {
                    currentState = state.Steps[processor](state with { X = processRange });
                }

                // This is the range that's less than or equal to the value 
                Range newRange = new(Math.Min(state.X.Min, val), Math.Min(state.X.Max, val + 1));
                return state with { X = newRange, Total = currentState.Total };
            };

        private static WorkflowStep ProcessMLessThan(int val, int processor) =>
            state =>
            {
                State currentState = state;
                Range processRange = new(Math.Min(state.M.Min, val - 1), Math.Min(state.M.Max, val));
                if (processRange.Length() > 0)
                {
                    currentState = state.Steps[processor](state with { M = processRange });
                }

                // This is the range that's greater than or equal to the value 
                Range newRange = new(Math.Max(state.M.Min, val), Math.Max(state.M.Max, val + 1));
                return state with { M = newRange, Total = currentState.Total };
            };

        private static WorkflowStep ProcessMGreaterThan(int val, int processor) =>
            state =>
            {
                State currentState = state;
                Range processRange = new(Math.Max(state.M.Min, val + 1), Math.Max(state.M.Max, val + 2));
                if (processRange.Length() > 0)
                {
                    currentState = state.Steps[processor](state with { M = processRange });
                }

                // This is the range that's less than or equal to the value 
                Range newRange = new(Math.Min(state.M.Min, val), Math.Min(state.M.Max, val + 1));
                return state with { M = newRange, Total = currentState.Total };
            };

        private static WorkflowStep ProcessALessThan(int val, int processor) =>
            state =>
            {
                State currentState = state;
                Range processRange = new(Math.Min(state.A.Min, val - 1), Math.Min(state.A.Max, val));
                if (processRange.Length() > 0)
                {
                    currentState = state.Steps[processor](state with { A = processRange });
                }

                // This is the range that's greater than or equal to the value 
                Range newRange = new(Math.Max(state.A.Min, val), Math.Max(state.A.Max, val + 1));
                return state with { A = newRange, Total = currentState.Total };
            };

        private static WorkflowStep ProcessAGreaterThan(int val, int processor) =>
            state =>
            {
                State currentState = state;
                Range processRange = new(Math.Max(state.A.Min, val + 1), Math.Max(state.A.Max, val + 2));
                if (processRange.Length() > 0)
                {
                    currentState = state.Steps[processor](state with { A = processRange });
                }

                // This is the range that's less than or equal to the value 
                Range newRange = new(Math.Min(state.A.Min, val), Math.Min(state.A.Max, val + 1));
                return state with { A = newRange, Total = currentState.Total };
            };

        private static WorkflowStep ProcessSLessThan(int val, int processor) =>
            state =>
            {
                State currentState = state;
                Range processRange = new(Math.Min(state.S.Min, val - 1), Math.Min(state.S.Max, val));
                if (processRange.Length() > 0)
                {
                    currentState = state.Steps[processor](state with { S = processRange });
                }

                // This is the range that's greater than or equal to the value 
                Range newRange = new(Math.Max(state.S.Min, val), Math.Max(state.S.Max, val + 1));
                return state with { S = newRange, Total = currentState.Total };
            };

        private static WorkflowStep ProcessSGreaterThan(int val, int processor) =>
            state =>
            {
                State currentState = state;
                Range processRange = new(Math.Max(state.S.Min, val + 1), Math.Max(state.S.Max, val + 2));
                if (processRange.Length() > 0)
                {
                    currentState = state.Steps[processor](state with { S = processRange });
                }

                // This is the range that's less than or equal to the value 
                Range newRange = new(Math.Min(state.S.Min, val), Math.Min(state.S.Max, val + 1));
                return state with { S = newRange, Total = currentState.Total };
            };


        private static WorkflowStep Bind(WorkflowStep first, WorkflowStep second) =>
            state =>
            {
                return second(first(state));
            };

    }
}
