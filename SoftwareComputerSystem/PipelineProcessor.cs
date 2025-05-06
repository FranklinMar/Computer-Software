using ParallelTree;
using static SoftwareComputerSystem.PipelineProcessor.ExecutionEvent;
using TreeNode = ParallelTree.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoftwareComputerSystem.PipelineProcessor.ExecutionEvent;

namespace SoftwareComputerSystem
{


    public class PipelineProcessor
    {
        public class ExecutionEvent
        {
            public enum ExecutionEventType
            {
                Start, Complete
            }
            public int Tick { get; set; }
            public ExecutionEventType EventType { get; set; }
            public Tree Node { get; set; }
            public int MemoryBlock { get; set; }
            public int Duration { get; set; } = 0;
            //public Tree LeftMemoryBlock { get; set; }
            //public Tree RightMemoryBlock { get; set; }

            public override string ToString()
            {
                string Type = Node is TreeValue ? "value" : "operation";
                string Result = $"Tick {Tick}: {EventType} processing of {Type} in block {MemoryBlock} (Node Value: '{Node.Value}', ID: {Node.GetHashCode():X8})";
                if (EventType == ExecutionEventType.Complete && Node is not TreeValue)
                {
                    Result += $" LeftOperand: {((TreeNode)Node).Left.GetHashCode():X8} RightOperand: {((TreeNode)Node).Right.GetHashCode():X8} Duration: {Duration}";
                }
                return Result;
            }
        }
        private DistributedMemory Memory;
        private OperationsDictionary DurationsFactor;
        private Dictionary<Tree, int> NodeCompletionTick = new Dictionary<Tree, int>();
        private Dictionary<Tree, int> NodeToMemoryBlock = new Dictionary<Tree, int>();
        private Queue<Tree> MemoryQueue = new Queue<Tree>();
        public int CurrentTick { get; private set; } = 0;
        public List<ExecutionEvent> ExecutionHistory { get; private set; } = new List<ExecutionEvent>();

        public PipelineProcessor(DistributedMemory memory, OperationsDictionary durationsFactor)
        {
            Memory = memory;
            DurationsFactor = durationsFactor;
        }

        public void SimulateExecution(Tree node)
        {
            if (node == null)
                return;
            int StartingTick = CurrentTick;
            int MemoryBlockAddress = Memory.AllocateMemory(node, StartingTick/*, out int MemoryBlockAddress*/);
            /*try
            {
                 MemoryBlockAddress*//*CurrentTick*//* = Memory.AllocateMemory(node, StartingTick*//*, out int MemoryBlockAddress*//*);
            }catch (OutOfMemoryException Exception)
            {
                MemoryQueue.Enqueue(node);
            }*/
            NodeToMemoryBlock[node] = MemoryBlockAddress;
            CurrentTick = StartingTick + Memory.MemoryAccessTime;
            ExecutionHistory.Add(new ExecutionEvent
            {
                Tick = CurrentTick,
                EventType = ExecutionEventType.Start,
                Node = node,
                MemoryBlock = MemoryBlockAddress
            });

            if (node is TreeNode)
            {
                TreeNode Node = (TreeNode)node;
                SimulateExecution(Node.Left);
                int LeftCompleteTick = NodeCompletionTick[Node.Left];

                SimulateExecution(Node.Right);
                int RightCompleteTick = NodeCompletionTick[Node.Right];
                int OperationStartTick = Math.Max(LeftCompleteTick, RightCompleteTick);

                int OperationTime = DurationsFactor[Node.Value];
                CurrentTick = OperationStartTick + OperationTime;
                NodeCompletionTick[Node] = CurrentTick;

                ExecutionHistory.Add(new ExecutionEvent
                {
                    Tick = CurrentTick,
                    EventType = ExecutionEventType.Complete,
                    Node = Node,
                    MemoryBlock = MemoryBlockAddress
                });

                int LastReleaseTick = CurrentTick;
                if (Node.Left is TreeValue)
                {
                    LastReleaseTick = Math.Max(LastReleaseTick, Memory.ReleaseMemory(NodeToMemoryBlock[Node.Left], CurrentTick));
                }
                if (Node.Right is TreeValue)
                {
                    LastReleaseTick = Math.Max(LastReleaseTick, Memory.ReleaseMemory(NodeToMemoryBlock[Node.Right], CurrentTick));
                }
                CurrentTick = LastReleaseTick;
            }
            else
            {
                NodeCompletionTick[node] = CurrentTick;

                ExecutionHistory.Add(new ExecutionEvent
                {
                    Tick = CurrentTick,
                    EventType = ExecutionEventType.Complete,
                    Node = node,
                    MemoryBlock = MemoryBlockAddress,
                    Duration = 0
                });
            }
        }

        public void ReleaseRootMemory(Tree root)
        {
            if (root != null && NodeToMemoryBlock.ContainsKey(root))
            {
                int releaseCompleteTick = Memory.ReleaseMemory(NodeToMemoryBlock[root], CurrentTick);
                CurrentTick = releaseCompleteTick;
            }
        }
    }
}
