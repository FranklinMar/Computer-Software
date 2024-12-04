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
            SyntaxAnalyzer Lex = new("(1 + hello(123, 3123 ,gfg))");
            
            Console.WriteLine(Lex);
        }
    }
}
