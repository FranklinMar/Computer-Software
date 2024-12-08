namespace LexSyntax_Analyzer
{
    public partial class Analyzer : Form
    {   
        private Color BackgroundColour;
        private Font Font;
        public SyntaxAnalyzer ExpressionAnalyzer { get; private set; }    

        public Analyzer()
        {
            InitializeComponent();
        }

        private void AnalyzerForm_Load(object sender, EventArgs e)
        {
            //LexicalAnalyzer Lex = new("12345 123124dasdf 1");
            //Console.WriteLine(Lex);
            // "12345 123124dasdf (1 + hello(123, 3123 ,gfg))"
            //SyntaxAnalyzer Lex = new("(1 + hello(123, 3123 ,gfg))");
            //SyntaxAnalyzer Lex = new("12345 123124dasdf * 123 + (1 + hello(123, 3123 ,gfg)) / (45");
            //SyntaxAnalyzer Lex = new("a-+(t*5.81.8 - ))/");
            //SyntaxAnalyzer Lex = new("a-(t*5.818 - x1)/x2");
            //Lex.Errors.Add(new Exception());
            //Console.WriteLine(Lex);
            BackgroundColour = InputBox.BackColor;
            Font = InputBox.Font;
        }

        private void NewInput(object sender, EventArgs e)
        {
            RichTextBox Box = (RichTextBox) sender;
            ExpressionAnalyzer = new(Box.Text);
            if (ExpressionAnalyzer.Errors.Count == 0)
            {
                ResultBox.Text = "No errors found";
                ResultBox.Select(0, Box.Text.Length - 1);
                ResultBox.SelectionFont = new Font(Font, FontStyle.Bold);
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
                //int Index = 0;
                ResultBox.Text = string.Join("\n", Errors.Select(Err => Err.Message));
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
