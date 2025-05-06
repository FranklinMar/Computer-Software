using System.ComponentModel;
using LexSyntax_Analyzer;
using ParallelTree;
using AlgebraicLaws;
using System.Diagnostics;
using TreeNode = ParallelTree.TreeNode;
using System.Text;
using System.Xml.Linq;

namespace SoftwareComputerSystem
{
    public partial class ResultForm : Form
    {
        Tree ParallelTree;
        private Color BackgroundColour;
        private Color ForeColour;
        private Font FontDefault;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SyntaxAnalyzer ExpressionAnalyzer { get; private set; }

        public ResultForm()
        {
            InitializeComponent();
            //AddCount.Text = "0";
            //SubCount.Text = "0";
            //MulCount.Text = "0";
            //DivCount.Text = "0";
            //AddTicks.Text = "0";
            //SubTicks.Text = "0";
            //MulTicks.Text = "0";
            //DivTicks.Text = "0";
        }

        private void LoadBuilder(object sender, EventArgs e)
        {
            BackgroundColour = InputBox.BackColor;
            ForeColour = InputBox.ForeColor;
            FontDefault = InputBox.Font;
        }

        private void NewInput(object sender, EventArgs e)
        {
            RichTextBox Box = (RichTextBox)sender;
            Box.ForeColor = ForeColour;
            Box.Font = FontDefault;
            ExpressionAnalyzer = new(Box.Text);
            if (ExpressionAnalyzer.Errors.Count == 0)
            {
                Color Color = Color.LightGreen;
                try
                {
                    ExpressionAnalyzer = new(LawOptimization.Simplify(Box.Text));
                    ParallelTree = TreeBuilder.Parse(ExpressionAnalyzer);
                    //Debug.WriteLine(ParallelTree != null ? ParallelTree.Print() : "");

                    ResultBox.Text = ParallelTree != null ? ParallelTree.Print() : "";
                    //ResultBox.Text = 
                    ResultBox.Select(0, ResultBox.Text.Length);
                    //ResultBox.SelectionFont = new Font(Font, FontStyle.Bold);
                    ResultBox.ForeColor = Color.Snow;
                }
                catch (DivideByZeroException Exception)
                {
                    ResultBox.Text = "Error: Division by zero. Impossible operation";
                    ResultBox.Select(0, ResultBox.Text.Length);
                    //ResultBox.SelectionFont = new Font(Font, FontStyle.Bold);
                    ResultBox.ForeColor = Color.Red;
                    Color = Color.Snow;
                }
                Box.Select(0, Box.Text.Length);
                Box.BackColor = BackgroundColour;
                Box.SelectionBackColor = BackgroundColour;
                Box.ForeColor = Color;
                Box.Select(Box.Text.Length, 0);
            }
            else
            {
                ResultBox.Text = "";
                var Errors = ExpressionAnalyzer.Errors;
                //int Index = 0;
                for (int i = 0; i < Errors.Count; i++)
                {
                    ResultBox.Text += $"#{i + 1}: {ExpressionAnalyzer.Errors[i].Message}\n";
                }
                // ResultBox.Text = string.Join("\n#1:\t", Errors.Select(Err => Err.Message));
                Box.Select(0, Box.Text.Length);
                Box.ForeColor = Color.Snow;
                Box.SelectionColor = Color.Snow;
                foreach (var Error in Errors)
                {
                    Box.Select(Error.Index, Error.Length);
                    Box.SelectionColor = Color.Red;
                }
                Box.Select(0, Box.Text.Length);
                Box.BackColor = BackgroundColour;
                Box.SelectionBackColor = BackgroundColour;
                Box.Select(Box.Text.Length, 0);
            }
        }

        private void ResultBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            TextBox Box = (TextBox)sender;
            if (Box.Text != "" && !int.TryParse(Box.Text, out _))
            {
                MessageBox.Show($"Value inside this field should be integer greater than zero", $"{Box.Name} Error");
                Box.Text = "";
            }
        }

        private void CalcButton_Click(object sender, EventArgs e)
        {
            List<TextBox> Fields = new(){ AddTicks, SubTicks, MulTicks, DivTicks};
            List<TextBox> ErrorFields = new();
            foreach (var Box in Fields)
            {
                if (Box.Text == "")
                {
                    ErrorFields.Add(Box);
                }
            }
            if (ErrorFields.Count > 0) {
                MessageBox.Show($"{string.Join(", ", ErrorFields.Select(Box => Box.Name))} TextBox should not have empty fields"
                    .Replace("Add", "'+' ").Replace("Sub", "'-' ").Replace("Mul", "'*' ").Replace("Div", "'/' "), "Error");
            }
            //OperationsDictionary Ticks;
            Dictionary<string, int> Ticks;
            //try
            //{
            /*Ticks = new(int.Parse(AddTicks.Text),
                int.Parse(SubTicks.Text), int.Parse(MulTicks.Text), int.Parse(DivTicks.Text));*/
            Ticks = new Dictionary<string, int>()
            {
                ["+"] = int.Parse(AddTicks.Text),
                ["-"] = int.Parse(SubTicks.Text),
                ["*"] = int.Parse(MulTicks.Text),
                ["/"] = int.Parse(DivTicks.Text),
            };
            if (ParallelTree is TreeNode)
            {
                  
                int Layers = 5;
                TreeNode Node = (TreeNode)ParallelTree;
                ComputerSystem System = new(Layers, Ticks);
                StringBuilder SB = new StringBuilder();
                var NormalSim = System.CalculateExpression(Node);
                var SequentialSim = System.CalculateExpression(Node, true);
                int TotalActions = NormalSim.Sum(Step => Step.ProcessorActions.Count);
                int NonIdleActions = NormalSim.Sum(Step => Step.ProcessorActions.Count(Action => !(Action is IdleProcessorAction)));
                double Acceleration = SequentialSim.Count / NormalSim.Count;
                double Efficiency = NonIdleActions / TotalActions;
                SB.AppendLine($"Час послідовного виконання = {SequentialSim.Count}");
                SB.AppendLine($"Час паралельного виконання = {NormalSim.Count}");
                SB.AppendLine($"Коефіцієнт прискорення = {Acceleration:F2}");
                SB.AppendLine($"Коефіцієнт ефективності = {Efficiency:F2}");
                Node.Traverse(Node => { SB.AppendLine($"{Node.GetHashCode():X8} : {Node.Value}"); });
                SB.Append("   T   R |");
                for (int i = 0; i < Layers; i++)
                {
                    SB.Append($"S{i + 1}\t\t");
                }
                SB.AppendLine("| W");
                for (int i = 0; i < NormalSim.Count; i++)
                {
                    SB.AppendLine($"{i + 1,4} {NormalSim[i]}");
                }
                ResultBox.Text = SB.ToString();
                //ComputerSystem System = new(16, Ticks);
                //ResultBox.Text = System.SimulateWork(Node);
            }
            //}
            //catch (ArgumentException Exception)
            //{
            //    MessageBox.Show(Exception.Message, "Error");
            //    return;
            //}
        }
    }
}
