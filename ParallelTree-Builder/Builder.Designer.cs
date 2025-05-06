namespace ParallelTree
{
    partial class Builder
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
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
            InputBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            InputBox.BackColor = Color.FromArgb(36, 14, 31);
            InputBox.Cursor = Cursors.IBeam;
            InputBox.Font = new Font("Roboto", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            InputBox.ForeColor = Color.White;
            InputBox.Location = new Point(30, 20);
            InputBox.Name = "InputBox";
            InputBox.Size = new Size(450, 500);
            InputBox.TabIndex = 1;
            InputBox.Text = "";
            InputBox.TextChanged += NewInput;
            // 
            // ResultBox
            // 
            ResultBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ResultBox.BackColor = Color.FromArgb(36, 14, 31);
            ResultBox.Font = new Font("Roboto", 12F);
            ResultBox.ForeColor = Color.White;
            ResultBox.Location = new Point(500, 20);
            ResultBox.Name = "ResultBox";
            ResultBox.ReadOnly = true;
            ResultBox.Size = new Size(450, 500);
            ResultBox.TabIndex = 2;
            ResultBox.Text = "";
            // 
            // AnalyzerLabel
            // 
            AnalyzerLabel.AutoSize = true;
            AnalyzerLabel.BorderStyle = BorderStyle.FixedSingle;
            AnalyzerLabel.Font = new Font("Roboto", 12F);
            AnalyzerLabel.ForeColor = Color.White;
            AnalyzerLabel.Location = new Point(362, 11);
            AnalyzerLabel.Name = "AnalyzerLabel";
            AnalyzerLabel.Size = new Size(96, 21);
            AnalyzerLabel.TabIndex = 3;
            AnalyzerLabel.Text = "Expressions";
            // 
            // ResultLabel
            // 
            ResultLabel.AutoSize = true;
            ResultLabel.BorderStyle = BorderStyle.FixedSingle;
            ResultLabel.Font = new Font("Roboto", 12F);
            ResultLabel.ForeColor = Color.White;
            ResultLabel.Location = new Point(865, 11);
            ResultLabel.Name = "ResultLabel";
            ResultLabel.Size = new Size(63, 21);
            ResultLabel.TabIndex = 4;
            ResultLabel.Text = "Results";
            // 
            // Builder
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(48, 25, 52);
            ClientSize = new Size(984, 561);
            Controls.Add(ResultLabel);
            Controls.Add(AnalyzerLabel);
            Controls.Add(ResultBox);
            Controls.Add(InputBox);
            ForeColor = Color.AliceBlue;
            Name = "Builder";
            Load += LoadBuilder;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RichTextBox InputBox;
        private RichTextBox ResultBox;
        private System.Windows.Forms.Label AnalyzerLabel;
        private Label ResultLabel;
    }
}
