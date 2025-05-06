using ParallelTree;
using TreeNode = ParallelTree.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareComputerSystem
{
    public interface ProcessorAction
    {
        //public override string ToString();
    }

    public class IdleProcessorAction : ProcessorAction
    {
        public override string ToString() => new('\t', 2);
    }

    public class CalculatingProcessorAction : ProcessorAction
    {
        public TreeNode Node { get; }

        public CalculatingProcessorAction(TreeNode node)
        {
            Node = node;
        }

        public override string ToString() => $"{Node.GetHashCode():X8}";
    }
}
