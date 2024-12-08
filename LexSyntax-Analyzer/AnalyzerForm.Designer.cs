namespace LexSyntax_Analyzer
{
    partial class Analyzer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            InputBox = new RichTextBox();
            ResultBox = new RichTextBox();
            AnalyzerLabel = new Label();
            ResultLabel = new Label();
            SuspendLayout();
            // 
            // InputBox
            // 
            InputBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            InputBox.BackColor = Color.FromArgb(36, 14, 31);
            InputBox.ForeColor = Color.White;
            InputBox.Location = new Point(30, 20);
            InputBox.Name = "InputBox";
            InputBox.Size = new Size(450, 400);
            InputBox.TabIndex = 1;
            InputBox.Text = "";
            InputBox.TextChanged += NewInput;
            // 
            // ResultBox
            // 
            ResultBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ResultBox.BackColor = Color.FromArgb(36, 14, 31);
            ResultBox.ForeColor = Color.Silver;
            ResultBox.Location = new Point(500, 20);
            ResultBox.Name = "ResultBox";
            ResultBox.ReadOnly = true;
            ResultBox.Size = new Size(450, 400);
            ResultBox.TabIndex = 2;
            ResultBox.Text = "";
            // 
            // AnalyzerLabel
            // 
            AnalyzerLabel.AutoSize = true;
            AnalyzerLabel.BorderStyle = BorderStyle.FixedSingle;
            AnalyzerLabel.ForeColor = Color.White;
            AnalyzerLabel.Location = new Point(410, 11);
            AnalyzerLabel.Name = "AnalyzerLabel";
            AnalyzerLabel.Size = new Size(58, 18);
            AnalyzerLabel.TabIndex = 3;
            AnalyzerLabel.Text = "Analyzer";
            // 
            // ResultLabel
            // 
            ResultLabel.AutoSize = true;
            ResultLabel.BorderStyle = BorderStyle.FixedSingle;
            ResultLabel.ForeColor = Color.White;
            ResultLabel.Location = new Point(892, 11);
            ResultLabel.Name = "ResultLabel";
            ResultLabel.Size = new Size(46, 18);
            ResultLabel.TabIndex = 4;
            ResultLabel.Text = "Results";
            // 
            // Analyzer
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(48, 25, 52);
            ClientSize = new Size(984, 561);
            Controls.Add(ResultLabel);
            Controls.Add(AnalyzerLabel);
            Controls.Add(ResultBox);
            Controls.Add(InputBox);
            Font = new Font("Futura Bk BT", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.AliceBlue;
            Name = "Analyzer";
            Load += AnalyzerForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox InputBox;
        private RichTextBox ResultBox;
        private Label AnalyzerLabel;
        private Label ResultLabel;
    }
}
