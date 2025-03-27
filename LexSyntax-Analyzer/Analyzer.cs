namespace LexSyntax_Analyzer
{
    public partial class Analyzer : Form
    {   
        private Color BackgroundColour;
        private Font FontDefault;
        public StateAnalyzer ExpressionAnalyzer { get; private set; }    

        public Analyzer()
        {
            InitializeComponent();
        }

        private void AnalyzerForm_Load(object sender, EventArgs e)
        {
            BackgroundColour = InputBox.BackColor;
            FontDefault = InputBox.Font;
        }

        private void NewInput(object sender, EventArgs e)
        {
            RichTextBox Box = (RichTextBox) sender;
            Box.Font = FontDefault;
            ExpressionAnalyzer = new(Box.Text);
            if (ExpressionAnalyzer.Errors.Count == 0)
            {
                ResultBox.Text = "No errors found";
                //ResultBox.Select(0, Box.Text.Length - 1);
                //ResultBox.SelectionFont = new Font(Font, FontStyle.Bold);
                //ResultBox.ForeColor = Color.LightGreen;
                Box.Select(0, Box.Text.Length);
                Box.ForeColor = Color.LightGreen;
                Box.BackColor = BackgroundColour;
                Box.SelectionBackColor = BackgroundColour;
                Box.Select(Box.Text.Length, 0);
            } else
            {
                ResultBox.Text = "";
                var Errors = ExpressionAnalyzer.Errors;
                for (int i = 0; i < Errors.Count; i++)
                {
                    ResultBox.Text += $"#{i + 1}: {ExpressionAnalyzer.Errors[i].Message}\n";
                }
                Box.Font = Font;
                Box.Select(0, Box.Text.Length);
                Box.ForeColor = Color.White;
                Box.SelectionColor = Color.White;
                for (int i = 0; i < Errors.Count; i++)
                {
                    Box.Select(Errors[i].Index, Errors[i].Length);
                    Box.SelectionColor = Color.Red;
                }
                Box.Select(0, Box.Text.Length);
                Box.BackColor = BackgroundColour;
                Box.SelectionBackColor = BackgroundColour;
                Box.Select(Box.Text.Length, 0);
            }
        }
    }
}
