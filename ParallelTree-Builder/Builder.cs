using System.ComponentModel;
using LexSyntax_Analyzer;

namespace ParallelTree_Builder
{
    public partial class Builder : Form
    {
        private Color BackgroundColour;
        private Color ForeColour;
        private Font FontDefault;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SyntaxAnalyzer ExpressionAnalyzer { get; private set; }

        public Builder()
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
                TreeBuilder ParallelTree = new(ExpressionAnalyzer);
                ResultBox.Text = ParallelTree.Root != null ? ParallelTree.Root.Print() : "";
                ResultBox.Select(0, ResultBox.Text.Length - 1);
                ResultBox.SelectionFont = new Font(Font, FontStyle.Bold);
                //ResultBox.ForeColor = Color.LightGreen;
                Box.Select(0, Box.Text.Length);
                Box.ForeColor = Color.LightGreen;
                Box.BackColor = BackgroundColour;
                Box.SelectionBackColor = BackgroundColour;
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
                Box.ForeColor = Color.White;
                Box.SelectionColor = Color.White;
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

        /*public void PrintTree(RichTextBox Box, TreeNode Root, int topMargin = 2, int leftMargin = 2)
        {
            if (Root == null)
            {
                return;
            }

            int rootTop = Console.CursorTop + topMargin;
            List<VisualTreeNode> last = new List<VisualTreeNode>();
            TreeNode next = Root;

            for (int level = 0; next != null; level++)
            {
                VisualTreeNode item = new VisualTreeNode()
                {
                    Node = next
                };

                if (level < last.Count)
                {
                    item.StartPos = last[level].EndPos + 1;
                    last[level] = item;
                }
                else
                {
                    item.StartPos = leftMargin;
                    last.Add(item);
                }

                if (level > 0)
                {
                    item.Parent = last[level - 1];
                    if (next == item.Parent.Node.Left)
                    {
                        item.Parent.Left = item;
                        item.EndPos = Math.Max(item.EndPos, item.Parent.StartPos);
                    }
                    else
                    {
                        item.Parent.Right = item;
                        item.StartPos = Math.Max(item.StartPos, item.Parent.EndPos);
                    }
                }

                next = next.Left ?? next.Right;

                for (; next == null; item = item.Parent)
                {
                    Print(Box, item, rootTop + 2 * level);

                    if (--level < 0) break;
                    if (item == item.Parent.Left)
                    {
                        item.Parent.StartPos = item.EndPos;
                        next = item.Parent.Node.Right;
                    }
                    else
                    {
                        if (item.Parent.Left == null)
                        {
                            item.Parent.EndPos = item.StartPos;
                        }
                        else
                        {
                            item.Parent.StartPos += (item.StartPos - item.Parent.EndPos) / 2;
                        }
                    }
                }
                Console.SetCursorPosition(0, rootTop + 2 * last.Count - 1);
            }
        }

        private static void PrintTree(RichTextBox Box, VisualTreeNode item, int top)
        {
            SwapColors();
            Print(Box, item.Text, top, item.StartPos);
            SwapColors();

            if (item.Left != null)
            {
                PrintLink(Box, top + 1, "┌", "┘", item.Left.StartPos + item.Left.Size / 2, item.StartPos);
            }

            if (item.Right != null)
            {
                PrintLink(Box, top + 1, "└", "┐", item.EndPos - 1, item.Right.StartPos + item.Right.Size / 2);
            }
        }

        private static void Print(RichTextBox Box, string text, int top, int left, int right = -1)
        {
            Console.SetCursorPosition(left, top);
            if (right < 0)
            {
                right = left + text.Length;
            }

            while (Console.CursorLeft < right)
            {
                Console.Write(text);
            }
        }

        private static void PrintLink(RichTextBox Box, int top, string start, string end, int startPos, int endPos)
        {
            Print(Box, start, top, startPos);
            Print(Box, "-", top, startPos + 1, endPos);
            Print(Box, end, top, endPos);
        }

        private static void SwapColors()
        {
            Console.ForegroundColor = Color.White;
            Console.BackgroundColor = BackGroundColor;
        }*/
    }
}
