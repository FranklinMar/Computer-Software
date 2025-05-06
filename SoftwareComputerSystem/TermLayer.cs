using ParallelTree;
using TreeNode = ParallelTree.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareComputerSystem
{
    public class GenericLayer //: CalculationLayer
    {
        public string Operation { get; private set;}
        public GenericLayer(string Operation)
        {
            if (!TreeBuilder.OperationPriorities.ContainsKey(Operation))
            {
                throw new ArgumentException("Unallowed operation cannot be used here");
            }
            this.Operation = Operation;
        }
        public Tree Calculate(Tree Root)
        {
            if (Root is TreeValue || Root == null)
            {
                return Root;
            }
            TreeNode Node = (TreeNode) Root;
            if (Node.Left is not TreeValue)
            {
                Node.Left = Calculate(Node.Left);
            }
            if (Node.Right is not TreeValue)
            {
                Node.Right = Calculate(Node.Right);
            }

            if (Node.Value == Operation)
            {
                return new TreeValue(Node.Left.Value + Node.Value + Node.Right.Value);
            }

            return new TreeNode(Node.Value, Node.Left, Node.Right);
        }
    }
}
