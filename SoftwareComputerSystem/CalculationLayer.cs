using ParallelTree;
using TreeNode = ParallelTree.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareComputerSystem
{
    /*public interface CalculationLayer
    {
        public Tree Calculate(Tree Root);
    }*/
    [Serializable]
    public class CalculationLayer
    {
        /*public enum CalcState {
            Idle, Calculating
        }*/
        public int CalculatingTick = 0;
        public TreeNode? Node { get; set; }
        public bool IsIdle { get => Node == null; }
        public Dictionary<string, int> durations;
        public Dictionary<string, int> Durations
        {
            get => durations;
            set
            {
                if (value.Values.Any(Value => Value <= 0))
                {
                    throw new ArgumentException("Operation duration must be greater than 0");
                }
                durations = value;
            }
        }
        //public CalcState State { get => Node == null ? CalcState.Idle : CalcState.Calculating; }
        public ProcessorAction State { get => Node == null ? new IdleProcessorAction() : new CalculatingProcessorAction(Node); }
        public CalculationLayer(Dictionary<string, int> OpDurations, TreeNode? node = null)
        {
            Node = node;
            Durations = OpDurations;
        }

        public TreeNode? Calculate()
        {
            if (Node != null)
            {
                CalculatingTick += 1;
                if (CalculatingTick == Durations[Node.Value])
                {
                    CalculatingTick = 0;
                    TreeNode node = Node;
                    Node = null;
                    return node;
                }
            }
            return null;
        }

        public override string ToString()
        {
            if (Node == null)
            {
                return "";
            }
            return $"{GetHashCode():X8}";
        }

        public CalculationLayer Clone()
        {
            return new(Durations, Node);
        }
    }
}
