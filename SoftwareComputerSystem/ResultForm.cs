using System.ComponentModel;
using LexSyntax_Analyzer;
using ParallelTree_Builder;
using AlgebraicLaws;
using System.Diagnostics;

namespace SoftwareComputerSystem
{
    public partial class ResultForm : Form
    {
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
                    Tree ParallelTree = TreeBuilder.Parse(ExpressionAnalyzer);
                    Debug.WriteLine(ParallelTree != null ? ParallelTree.Print() : "");
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
            List<TextBox> Fields = new(){ AddCount, AddTicks, SubCount, SubTicks, MulCount, MulTicks, DivCount, DivTicks};
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
            VectorComputerSystem.OperationsDictionary Counts;
            VectorComputerSystem.OperationsDictionary Ticks;
            VectorComputerSystem VCS;
            try
            {
                Counts = new(int.Parse(AddCount.Text),
                    int.Parse(SubCount.Text), int.Parse(MulCount.Text), int.Parse(DivCount.Text));
                Ticks = new(int.Parse(AddTicks.Text),
                    int.Parse(SubTicks.Text), int.Parse(MulTicks.Text), int.Parse(DivTicks.Text));
                VCS = new(Counts, Ticks);
            }
            catch (ArgumentException Exception)
            {
                MessageBox.Show(Exception.Message, "Error");
                return;
            }

        }
    }
}
