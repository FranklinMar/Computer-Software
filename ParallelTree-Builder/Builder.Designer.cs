namespace ParallelTree_Builder
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
            InputBox = new System.Windows.Forms.RichTextBox();
            ResultBox = new System.Windows.Forms.RichTextBox();
            AnalyzerLabel = new System.Windows.Forms.Label();
            ResultLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // InputBox
            // 
            InputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            InputBox.BackColor = System.Drawing.Color.FromArgb(((int)((byte)36)), ((int)((byte)14)), ((int)((byte)31)));
            InputBox.ForeColor = System.Drawing.Color.White;
            InputBox.Location = new System.Drawing.Point(30, 20);
            InputBox.Name = "InputBox";
            InputBox.Size = new System.Drawing.Size(450, 400);
            InputBox.TabIndex = 1;
            InputBox.Text = "";
            InputBox.TextChanged += NewInput;
            // 
            // ResultBox
            // 
            ResultBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            ResultBox.BackColor = System.Drawing.Color.FromArgb(((int)((byte)36)), ((int)((byte)14)), ((int)((byte)31)));
            ResultBox.ForeColor = System.Drawing.Color.Silver;
            ResultBox.Location = new System.Drawing.Point(500, 20);
            ResultBox.Name = "ResultBox";
            ResultBox.ReadOnly = true;
            ResultBox.Size = new System.Drawing.Size(450, 400);
            ResultBox.TabIndex = 2;
            ResultBox.Text = "";
            // 
            // AnalyzerLabel
            // 
            AnalyzerLabel.AutoSize = true;
            AnalyzerLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            AnalyzerLabel.ForeColor = System.Drawing.Color.White;
            AnalyzerLabel.Location = new System.Drawing.Point(398, 11);
            AnalyzerLabel.Name = "AnalyzerLabel";
            AnalyzerLabel.Size = new System.Drawing.Size(69, 17);
            AnalyzerLabel.TabIndex = 3;
            AnalyzerLabel.Text = "Expressions";
            // 
            // ResultLabel
            // 
            ResultLabel.AutoSize = true;
            ResultLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ResultLabel.ForeColor = System.Drawing.Color.White;
            ResultLabel.Location = new System.Drawing.Point(892, 11);
            ResultLabel.Name = "ResultLabel";
            ResultLabel.Size = new System.Drawing.Size(46, 17);
            ResultLabel.TabIndex = 4;
            ResultLabel.Text = "Results";
            // 
            // Builder
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(((int)((byte)48)), ((int)((byte)25)), ((int)((byte)52)));
            ClientSize = new System.Drawing.Size(984, 561);
            Controls.Add(ResultLabel);
            Controls.Add(AnalyzerLabel);
            Controls.Add(ResultBox);
            Controls.Add(InputBox);
            ForeColor = System.Drawing.Color.AliceBlue;
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
