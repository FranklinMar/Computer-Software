    using ParallelTree;
    using TreeNode = ParallelTree.TreeNode;
using static MathNet.Symbolics.VisualExpression;
using System.Xml.Linq;
using static SoftwareComputerSystem.PipelineProcessor.ExecutionEvent;
using System.   Text;
using MathNet.Numerics.LinearRegression;
using System.Configuration;


namespace SoftwareComputerSystem
{
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
        public Dictionary<string, int> Durations {
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
        public CalculationLayer (Dictionary<string, int> OpDurations, TreeNode? node = null) 
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

    public class SimulationStep
    {
        //public KeyValuePair<TreeNode, string>? Reading { get; set; }
        public TreeNode? Reading { get; set; }
        public TreeNode? Writing { get; set; }
        //public KeyValuePair<TreeNode, string>? Writing { get; set; }
        public List<ProcessorAction> ProcessorActions { get; set;  }

        public SimulationStep(List<ProcessorAction>? Actions = null/*int Layers*/)
        {
            if (Actions == null)
            {
                Actions = new List<ProcessorAction>();
            }
            ProcessorActions = Actions;
            //if (Layers <= 0)
            //{
            //    throw new ArgumentException("Number of layers cannot be 0 or lower");
            //}
            //ProcessorActions = Enumerable.Repeat<ProcessorAction>(new IdleProcessorAction(), Layers).ToList();
        }

        /*public string? Operation
        {
            get
            {
                if (ProcessorActions.All(p => p is IdleProcessorAction))
                    return null;

                var calculatingAction = ProcessorActions.FirstOrDefault(p => p is CalculatingProcessorAction) as CalculatingProcessorAction;
                return calculatingAction?.Node.Value;
            }
        }*/

        public override string ToString()
        {
            //string readingStr = Reading.HasValue ? Reading.Value.Value : "";
            //string writingStr = Writing.HasValue ? Writing.Value.Value : "";
            string readingStr = Reading == null ? "\t" : Reading.Value;
            string writingStr = Writing == null ? "\t" : Writing.Value;
            return $"{readingStr,4} | {string.Join(" ", ProcessorActions)} | {writingStr}";
        }
    }

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
    public class ComputerSystem
    {
        private int CurrentStep = 0;
        private List<CalculationLayer> Layers;
        Dictionary<string, int> OperationDurations;

        public ComputerSystem(int Layers, Dictionary<string, int> OperationDurations)
        {
            if (Layers <= 0)
            {
                throw new ArgumentException("Number of layers cannot be 0 or lower");
            }
            /*if (!OperationDurations.All(Operation => Operation.Value > 0))
            {
                throw new ArgumentException("All operations durations must be greater than zero");
            }*/
            //this.Layers = Enumerable.Repeat<CalculationLayer>(new CalculationLayer(OperationDurations), Layers).ToList();
            this.Layers = new();
            for (int i = 0; i < Layers; i++)
            {
                this.Layers.Add(new CalculationLayer(OperationDurations));
            }
            this.OperationDurations = OperationDurations;
        }

        private static List<List<TreeNode>> GroupOpsByOperationTypes(List<List<TreeNode>> opsByLevels)
        {
            return opsByLevels.Select(Levels => Levels.Select(Nodes => Nodes)
                                            .OrderByDescending(Node => TreeBuilder.OperationPriorities[Node.Value]).ToList())
                .ToList();
        }
        /*void AddStep(List<SimulationStep> Steps, SimulationStep Step, int StepNum = -1)
        {
            if (StepNum < 0)
            {
                StepNum = CurrentStep;
            }
            if (StepNum < Steps.Count)
            {
                var existingStep = Steps[StepNum];

                if (existingStep.Reading == null)
                {
                    existingStep.Reading = Step.Reading;
                }

                if (existingStep.Writing == null)
                {
                    existingStep.Writing = Step.Writing;
                }

                for (int i = 0; i < existingStep.ProcessorActions.Count; i++)
                {
                    if (existingStep.ProcessorActions[i] is IdleProcessorAction *//*&& i < Step.ProcessorActions.Count && !(Step.ProcessorActions[i] is IdleProcessorAction)*//*)
                    {
                        existingStep.ProcessorActions[i] = Step.ProcessorActions[i];
                    }
                }
            } else
            {
                Steps.Add(Step);
            }

        }

        bool IsExecutionCompleted(List<SimulationStep> Steps, TreeNode node, int AtStep = -1)
        {
            if (AtStep == -1)
            {
                AtStep = CurrentStep;
            }

            if (TreeBuilder.OperationPriorities.Keys.Contains(node.Value))
            {
                // Find the first step where this node was written
                for (int i = 0; i < Steps.Count; i++)
                {
                    if (Steps[i].Writing == node)
                    {
                        return AtStep - i > 1;
                    }
                }
                return false;
            }
            return true;
        }

        bool IsStepSynchronized(List<SimulationStep> Steps, int AtStep = -1)
        {
            if (AtStep == -1)
            {
                AtStep = CurrentStep;
            }

            if (Steps[AtStep].ProcessorActions.All(action => action is IdleProcessorAction))
            {
                return true; // All layers are idle
            }
            else
            {
                if (AtStep > 0)
                {
                    for (int i = 0; i < Steps[AtStep].ProcessorActions.Count; i++)
                    {
                        // If at least one layer is idle now but was calculating in the previous step,
                        // the current step is synchronized
                        if (Steps[AtStep].ProcessorActions[i] is IdleProcessorAction &&
                            Steps[AtStep - 1].ProcessorActions[i] is CalculatingProcessorAction)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }*/
        public bool AreLayersClear() => Layers.All(Layer => Layer.Node == null);
        public bool ProcessorOperationMatch(string operation) => Layers.All(Layers => Layers.Node == null || Layers.Node.Value == operation);

        public List<SimulationStep> CalculateExpression(TreeNode expression, bool Sequential = false)
        {
            /*for (int Level = expression.GetMaxLevel(); Level >= 0; --Level)
            {

            }*/
            if (expression == null)
            {
                return new List<SimulationStep>();
            }

            //Dictionary<int, List<TreeNode>> Levels = new Dictionary<int, List<TreeNode>>();

            List<SimulationStep> Steps = new(); 
            List<List<TreeNode>> Levels = new();
            int RootDepth = expression.Depth;
            for(int i = 0; i <= RootDepth; i++)
            {
                Levels.Add(new List<TreeNode>());
            }
            Queue<TreeNode> Queue = new();

            Queue<TreeNode> BreadthFirstQueue = new Queue<TreeNode>();
            BreadthFirstQueue.Enqueue(expression);

            while (BreadthFirstQueue.Count > 0)
            {
                TreeNode Node = BreadthFirstQueue.Dequeue();

                if (Node.Left is TreeNode)
                {
                    BreadthFirstQueue.Enqueue((TreeNode)Node.Left);
                }

                if (Node.Right is TreeNode)
                {
                    BreadthFirstQueue.Enqueue((TreeNode)Node.Right);
                }
                Levels[Node.Depth].Add(Node);
            }
            List<List<TreeNode>> SortedLevels = GroupOpsByOperationTypes(Levels);
            List<TreeNode> Nodes = SortedLevels.SelectMany(_ => _).ToList();
            Dictionary<int, TreeNode?> Temp = new();
            for (int i = 0; i < Layers.Count; i++)
            {
                Temp[i] = null;
            }
            /*for (int Level = 0; Level < SortedLevels.Count; Level++)
            {
                for (int i = 0; i < SortedLevels[Level].Count; i++)
                {
                    while (true)
                    {
                        Temp =
                    } 
                }
            }*/
            int Index = 0;
            SimulationStep Step;
            while(Index < Nodes.Count || Layers.Any(Layer => !Layer.IsIdle))
            {
                //TreeNode Node = Nodes[j];
                Step = new();
                bool Condition = Index < Nodes.Count && (Sequential ? (!Layers.Any(Layer => !Layer.IsIdle))
                    : (Layers[0].IsIdle && Layers.All(Layer => Layer.IsIdle || Layer.Node.Value == Nodes[Index].Value)));
                if (Condition)
                {
                    Layers[0].Node = Nodes[Index];
                    Step.Reading = Nodes[Index];
                    Index++;
                    //CurrentStep++;
                }
                Step.ProcessorActions = (from Layer in Layers select Layer.State).ToList();
                for (int i = 0; i < Layers.Count; i++)
                {
                    Temp[i] = Layers[i].Calculate();
                }
                CurrentStep++;
                for (int i = 0; i < Layers.Count - 1; i++)
                {
                    if (Temp[i] != null && Layers[i + 1].IsIdle)
                    {
                        Layers[i + 1].Node = Temp[i];
                        Temp[i] = null;
                    }
                }
                if (Temp[Layers.Count - 1] != null)
                {
                    Step.Writing = Temp[Layers.Count - 1];
                    Temp[Layers.Count - 1] = null;
                }
                Steps.Add(Step);
            }
            return Steps;

            /*var Steps = new List<SimulationStep>();
            CurrentStep = 0;

            AddStep(Steps, new SimulationStep(Layers));

            List<TreeNode> NodesByLevels = new();

            for (int i = 0; i < SortedLevels.Count; i++)
            {
                List<TreeNode> LevelList = SortedLevels[i];
                for (int j = 0; j < LevelList.Count; j++)
                {
                    NodesByLevels.Add(LevelList[j]);
                }
            }

            for (int i = 0; i < NodesByLevels.Count; i++)
            {
                // Get the next operation from the sorted list
                TreeNode pendingOp = NodesByLevels[i];

                // Check if required previous operations are completed
                bool leftOpFinished = pendingOp.Left is TreeValue || IsExecutionCompleted(Steps, (TreeNode) pendingOp.Left);
                bool rightOpFinished = pendingOp.Right is TreeValue || IsExecutionCompleted(Steps, (TreeNode) pendingOp.Right);
                bool requiredOpsFinished = leftOpFinished && rightOpFinished;

                if (CurrentStep >= Steps.Count)
                {
                    AddStep(Steps, new SimulationStep(Layers));
                }

                // Check if operation type matches the currently executing one (if any)
                bool currentConveyorOpTypeIsSuitable = Sequential
                    ? Steps[CurrentStep].Operation == null
                    : Steps[CurrentStep].Operation == null || Steps[CurrentStep].Operation == pendingOp.Value;

                // Check if the first layer is ready
                bool firstLayerReady = Steps[CurrentStep].ProcessorActions[0] is IdleProcessorAction;

                // Check if the current step is synchronized
                bool stepSynchronized = IsStepSynchronized(Steps);

                if (requiredOpsFinished && currentConveyorOpTypeIsSuitable && firstLayerReady && stepSynchronized)
                {
                    // All conditions are met, add the operation to the graph

                    // Reading the operation from memory
                    if (CurrentStep > 0)
                    {
                        var readingStep = new SimulationStep(Layers);
                        readingStep.Reading = pendingOp;
                        AddStep(Steps, readingStep, CurrentStep - 1);
                    }
                    else
                    {
                        var readingStep = new SimulationStep(Layers);
                        readingStep.Reading = pendingOp;
                        AddStep(Steps, readingStep);
                        CurrentStep++;
                    }

                    // Operation execution time on a single layer
                    int opExecutionTime = OperationDurations[pendingOp.Value];
                    int totalExecutionTime = opExecutionTime * Layers;

                    for (int activeLayer = 0; activeLayer < Layers; activeLayer++)
                    {
                        for (int j = 0; j < opExecutionTime; j++)
                        {
                            var processorActions = new List<ProcessorAction>();

                            // Add idle actions before the active layer
                            for (int k = 0; k < activeLayer; k++)
                            {
                                processorActions.Add(new IdleProcessorAction());
                            }

                            // Add calculating action for the active layer
                            processorActions.Add(new CalculatingProcessorAction(pendingOp));

                            // Add idle actions after the active layer
                            for (int k = 0; k < Layers - (activeLayer + 1); k++)
                            {
                                processorActions.Add(new IdleProcessorAction());
                            }

                            var calculatingStep = new SimulationStep(Layers);
                            calculatingStep.ProcessorActions.Clear();
                            foreach (var action in processorActions)
                            {
                                calculatingStep.ProcessorActions.Add(action);
                            }

                            AddStep(Steps, calculatingStep, CurrentStep + activeLayer * opExecutionTime + j);
                        }
                    }

                    // Operation added to the graph, move to the next one in the queue
                    var writingStep = new SimulationStep(Layers);
                    writingStep.Writing = pendingOp;
                    AddStep(Steps, writingStep, CurrentStep + totalExecutionTime + 1);
                }

                CurrentStep++;
            }

            return Steps;*/






            //private Dictionary<string, int> Counts { get; set; } = new() { { "+", 0 }, { "-", 0 }, { "*", 0 }, { "/", 0 } };
            /*private DistributedMemory Memory { get; set; }
            //private DistributedMemory Memory { get; set; }
            private PipelineProcessor Processor { get; set; }

            public ComputerSystem(int TotalMemoryBlocks, OperationsDictionary durations)
            {
                Memory = new(TotalMemoryBlocks);
                Processor = new(Memory, durations);
            }

            public string SimulateWork(TreeNode Node)
            {
                StringBuilder SB = new();
                Processor.SimulateExecution(Node);
                SB.AppendLine($"ID of Root node: {Node.GetHashCode():X8}\nExecution summary\n");
                foreach(var Execution in Processor.ExecutionHistory)
                {
                    SB.Append(Execution + "\n");
                }
                SB.AppendLine($"Memory allocation history");
                foreach (var AllocationEvent in Memory.AllocationHistory)
                {
                    SB.Append(AllocationEvent + "\n");
                }
                return SB.ToString();
            }*/

            /*string[] Operators = { "+", "-", "*", "/" };
            List<List<TreeNode>> Result = new List<List<TreeNode>>();

            Queue<TreeNode> BreadthFirstQueue = new Queue<TreeNode>();
            BreadthFirstQueue.Enqueue(expression);

            while (BreadthFirstQueue.Count > 0)
            {
                int levelSize = BreadthFirstQueue.Count;
                List<TreeNode> currentLevel = new List<TreeNode>();

                for (int i = 0; i < levelSize; i++)
                {
                    TreeNode node = BreadthFirstQueue.Dequeue();
                    currentLevel.Add(node);

                    if (node.Left is TreeNode)
                    {
                        BreadthFirstQueue.Enqueue((TreeNode)node.Left);
                    }

                    if (node.Right is TreeNode)
                    {
                        BreadthFirstQueue.Enqueue((TreeNode)node.Right);
                    }
                }

                Result.Insert(0, currentLevel); // Insert at the beginning to reverse order
            }
            foreach (List<TreeNode> allOperations in Result)
            {
                foreach (var Operator in Operators)
                {
                    var operations = allOperations.FindAll(Op => Op.Value == Operator);
                    while (operations.Count > 0)
                    {
                        SetExpressions(Operator, operations[..Counts[Operator]]);
                        operations = operations[Counts[Operator]..];
                    }
                }
            }*/
        }

        /*public void SetExpressions(string Operator, List<TreeNode> Expressions)
        {
            foreach (var Entry in Blocks)
            {
                if (Entry.Key == Operator)
                {
                    for (int i = 0; i < Entry.Value.Count; i++)
                    {
                        string Value = i < Expressions.Count ? Expressions[i].GetHashCode().ToString() : "";
                        Entry.Value[i].Add(Value);
                    }
                    //continue;
                }
                else
                {
                    for (int i = 0; i < Entry.Value.Count; i++)
                    {
                        Entry.Value[i].Add("");
                    }
                }
            }
        }

        public double CalculateTime()
        {
            double Result = 0;
            foreach (var Entry in Blocks)
            {
                if (Entry.Value.Count > 0)
                {
                    Result += Entry.Value[0].Count(Op => Op != "") * Durations[Entry.Key];
                }
            }
            return Result;
        }*/
    }
}