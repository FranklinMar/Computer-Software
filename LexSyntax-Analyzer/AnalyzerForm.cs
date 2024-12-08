namespace LexSyntax_Analyzer
{
    public partial class AnalyzerForm : Form
    {
        public AnalyzerForm()
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
            SyntaxAnalyzer Lex = new("a-(t*5.818 - x1)/x2");
            Lex.Errors.Add(new Exception());
            Console.WriteLine(Lex);
        }
    }
}
