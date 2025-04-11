using System.ComponentModel;
using LexSyntax_Analyzer;
using ParallelTree_Builder;

namespace AlgebraicLaws
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
                    Tree ParallelTree = TreeBuilder.Parse(ExpressionAnalyzer);
                    ResultBox.Text = ParallelTree != null ? ParallelTree.Print() : "";
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
    }
}
