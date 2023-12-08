namespace AdventOfCode.Runner.Year2023.Day08
{
    using System.Diagnostics;

    [DebuggerDisplay("{Name} => {Left.Name}, {Right.Name}")]
    public class TreeNode
    {
        public TreeNode(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public TreeNode? Left { get; set; }

        public TreeNode? Right { get; set; }
    }
}
