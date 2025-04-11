using ParallelTree_Builder;
using static MathNet.Symbolics.VisualExpression;


namespace SoftwareComputerSystem
{
    public class ComputerSystem
    {
        public class MemoryBlock
        {
            public int Address { get; }
            public ParallelTree_Builder.TreeNode Node { get; private set; }
            public bool IsOccupied => Node != null;
            public int CycleOccupied { get; private set; } = -1;
            public int CycleReleased { get; private set; } = -1;

            public MemoryBlock(int address)
            {
                Address = address;
            }

            public void Allocate()
        }

        public class OperationsDictionary
        {
            private int Add { get; set;  }
            private int Subtract { get; set;  }
            private int Multiply { get; set;  }
            private int Divide { get; set;  }

            public OperationsDictionary(int add, int subtract, int multiply, int divide)
            {
                if (add <= 0 || subtract <= 0 || multiply <= 0 || divide <= 0)
                {
                    throw new ArgumentException("All operation values must be greater than 0.");
                }

                Add = add;
                Subtract = subtract;
                Multiply = multiply;
                Divide = divide;
            }

            public int this[string operatorSymbol]
            {
                get
                {
                    return operatorSymbol switch
                    {
                        "+" => Add,
                        "-" => Subtract,
                        "*" => Multiply,
                        "/" => Divide,
                        _ => throw new ArgumentException("Invalid operator."),
                    };
                }
                set
                {
                    if (value <= 0)
                    {
                        throw new ArgumentException("All operation values must be greater than 0.");
                    }
                }
            }
        }

        private OperationsDictionary Counts { get; set; }
        private OperationsDictionary Durations { get; set; }

        private Dictionary<string, List<List<string>>> Blocks;

        public ComputerSystem(OperationsDictionary counts, OperationsDictionary durations)
        {
            Counts = counts;
            Durations = durations;
        }

        public void CalculateExpression(ParallelTree_Builder.TreeNode expression)
        {
            /*for (int Level = expression.GetMaxLevel(); Level >= 0; --Level)
            {

            }*/
            string[] Operators = {"+", "-", "*", "/"}; 
            List<List<ParallelTree_Builder.TreeNode>> Result = new List<List<ParallelTree_Builder.TreeNode>>();

            if (expression == null)
            {
                return;
            }
            Queue<ParallelTree_Builder.TreeNode> BreadthFirstQueue = new Queue<ParallelTree_Builder.TreeNode>();
            BreadthFirstQueue.Enqueue(expression);

            while (BreadthFirstQueue.Count > 0)
            {
                int levelSize = BreadthFirstQueue.Count;
                List<ParallelTree_Builder.TreeNode> currentLevel = new List<ParallelTree_Builder.TreeNode>();

                for (int i = 0; i < levelSize; i++)
                {
                    ParallelTree_Builder.TreeNode node = BreadthFirstQueue.Dequeue();
                    currentLevel.Add(node);

                    if (node.Left is ParallelTree_Builder.TreeNode)
                    {
                        BreadthFirstQueue.Enqueue((ParallelTree_Builder.TreeNode) node.Left);
                    }

                    if (node.Right is ParallelTree_Builder.TreeNode)
                    {
                        BreadthFirstQueue.Enqueue((ParallelTree_Builder.TreeNode) node.Right);
                    }
                }

                Result.Insert(0, currentLevel); // Insert at the beginning to reverse order
            }
            foreach (List<ParallelTree_Builder.TreeNode> allOperations in Result)
            {
                foreach (var Operator in Operators) {
                    var operations = allOperations.FindAll(Op => Op.Value == Operator);
                    while (operations.Count > 0)
                    {
                        SetExpressions(Operator, operations[..Counts[Operator]]);
                        operations = operations[Counts[Operator]..];
                    }
                }
            }
        }

        public void SetExpressions(string Operator, List<ParallelTree_Builder.TreeNode> Expressions)
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
                } else
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
        }
    }
}
