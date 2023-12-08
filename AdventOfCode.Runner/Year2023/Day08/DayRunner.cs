namespace AdventOfCode.Runner.Year2023.Day08
{
    using AdventOfCode.Common;
    using System.Diagnostics;
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
            var lines = await InputReader.ReadLines(2023, 8, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 1;

            ReadOnlySpan<char> instructionBuffer = lines[0];
            int instructionIndex = 0;

            TreeNode current = BuildTree(lines[2..]);

            while (true)
            {
                TreeNode next;
                if (instructionBuffer[instructionIndex] == 'L')
                {
                    next = current.Left!;
                }
                else
                {
                    next = current.Right!;
                }

                current = next;

                if (current.Name == "ZZZ")
                {
                    break;
                }

                // Wrap around the instruction buffer.
                instructionIndex++;
                result++;
                if (instructionIndex == instructionBuffer.Length)
                {
                    instructionIndex = 0;
                }
            }

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            long result = 1;

            ReadOnlySpan<char> instructionBuffer = lines[0];
            int instructionIndex = 0;

            TreeNode[] nodeBuffer = BuildGhostTree(lines[2..]);
            Span<long> counts = stackalloc long[nodeBuffer.Length];

            while (true)
            {
                bool allZ = true;

                for (int i = 0; i < nodeBuffer.Length; ++i)
                {
                    TreeNode current = nodeBuffer[i];

                    if (current.Name.EndsWith('Z'))
                    {
                        continue;
                    }

                    TreeNode next;
                    if (instructionBuffer[instructionIndex] == 'L')
                    {
                        next = current.Left!;
                    }
                    else
                    {
                        next = current.Right!;
                    }

                    // Add one to the count for this one
                    counts[i]++;
                    nodeBuffer[i] = next;
                    if (!next.Name.EndsWith('Z'))
                    {
                        allZ = false;
                    }
                }

                if (allZ)
                {
                    break;
                }

                // Wrap around the instruction buffer.
                instructionIndex++;
                if (instructionIndex == instructionBuffer.Length)
                {
                    instructionIndex = 0;
                }
            }

            result = LowestCommonMultiple(counts);

            formatter.Format(result);
        }

        private static long LowestCommonMultiple(Span<long> counts)
        {
            return LowestCommonMultiple(counts[0], counts[1..]);
        }

        private static long LowestCommonMultiple(long a, Span<long> counts)
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

        private TreeNode[] BuildGhostTree(string[] nodes)
        {
            Dictionary<string, TreeNode> nodeMap = [];
            List<TreeNode> aNodes = [];

            foreach (ReadOnlySpan<char> node in nodes)
            {
                string nodeName = node[..3].ToString();
                TreeNode result = BuildNode(nodeMap, nodeName, node.Slice(7, 3).ToString(), node.Slice(12, 3).ToString());
                if (result.Name.EndsWith('A'))
                {
                    aNodes.Add(result);
                }
            }

            return aNodes.ToArray();
        }

        private TreeNode BuildTree(string[] nodes)
        {
            Dictionary<string, TreeNode> nodeMap = [];
            foreach (ReadOnlySpan<char> node in nodes)
            {
                BuildNode(nodeMap, node[..3].ToString(), node.Slice(7, 3).ToString(), node.Slice(12, 3).ToString());
            }

            return nodeMap["AAA"];
        }

        private TreeNode BuildNode(Dictionary<string, TreeNode> nodeMap, string name, string leftName, string rightName)
        {
            if (nodeMap.TryGetValue(name, out TreeNode? node))
            {
                // Already got the node
                if (node.Left != null && node.Right != null)
                {
                    return node;
                }
            }
            else
            {
                // We have to add it before we look it up.
                node = new TreeNode(name);
                nodeMap.Add(name, node);
            }

            if (!nodeMap.TryGetValue(leftName, out TreeNode? leftNode))
            {
                leftNode = new TreeNode(leftName);
                nodeMap.Add(leftName, leftNode);
            }

            if (!nodeMap.TryGetValue(rightName, out TreeNode? rightNode))
            {
                rightNode = new TreeNode(rightName);
                nodeMap.Add(rightName, rightNode);
            }

            node.Left = leftNode;
            node.Right = rightNode;
            return node;
        }
    }
}
