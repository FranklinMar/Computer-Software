using ParallelTree;
using TreeNode = ParallelTree.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareComputerSystem
{
    public class SimulationStep
    {
        public TreeNode? Reading { get; set; }
        public TreeNode? Writing { get; set; }
        public List<ProcessorAction> ProcessorActions { get; set; }

        public SimulationStep(List<ProcessorAction>? Actions = null)
        {
            if (Actions == null)
            {
                Actions = new List<ProcessorAction>();
            }
            ProcessorActions = Actions;
        }

        public override string ToString()
        {
            string readingStr = Reading == null ? "\t" : Reading.Value;
            string writingStr = Writing == null ? "\t" : Writing.Value;
            return $"{readingStr,4} | {string.Join(" ", ProcessorActions)} | {writingStr}";
        }
    }
}
