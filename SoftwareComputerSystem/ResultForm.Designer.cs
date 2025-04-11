namespace SoftwareComputerSystem
{
    partial class ResultForm
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
            AddTicks = new TextBox();
            AddCount = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            SubTicks = new TextBox();
            SubCount = new TextBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            MulTicks = new TextBox();
            MulCount = new TextBox();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            DivTicks = new TextBox();
            DivCount = new TextBox();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            CalcButton = new Button();
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
            InputBox.Size = new Size(450, 411);
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
            ResultBox.Size = new Size(450, 411);
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
            // AddTicks
            // 
            AddTicks.BackColor = Color.FromArgb(36, 14, 31);
            AddTicks.Cursor = Cursors.IBeam;
            AddTicks.Font = new Font("Roboto", 12F);
            AddTicks.ForeColor = Color.White;
            AddTicks.Location = new Point(107, 501);
            AddTicks.Name = "AddTicks";
            AddTicks.PlaceholderText = "0";
            AddTicks.Size = new Size(100, 27);
            AddTicks.TabIndex = 5;
            AddTicks.TextChanged += TextBoxTextChanged;
            // 
            // AddCount
            // 
            AddCount.BackColor = Color.FromArgb(36, 14, 31);
            AddCount.Cursor = Cursors.IBeam;
            AddCount.Font = new Font("Roboto", 12F);
            AddCount.ForeColor = Color.White;
            AddCount.Location = new Point(107, 468);
            AddCount.Name = "AddCount";
            AddCount.PlaceholderText = "0";
            AddCount.Size = new Size(100, 27);
            AddCount.TabIndex = 6;
            AddCount.TextChanged += TextBoxTextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BorderStyle = BorderStyle.FixedSingle;
            label1.Font = new Font("Roboto", 12F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(57, 444);
            label1.Name = "label1";
            label1.Size = new Size(150, 21);
            label1.TabIndex = 7;
            label1.Text = "'+' Operation Blocks";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Roboto", 12F);
            label2.ForeColor = Color.White;
            label2.Location = new Point(32, 476);
            label2.Name = "label2";
            label2.Size = new Size(69, 19);
            label2.TabIndex = 8;
            label2.Text = "Quantity";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Roboto", 12F);
            label3.ForeColor = Color.White;
            label3.Location = new Point(54, 509);
            label3.Name = "label3";
            label3.Size = new Size(47, 19);
            label3.TabIndex = 9;
            label3.Text = "Ticks";
            // 
            // SubTicks
            // 
            SubTicks.BackColor = Color.FromArgb(36, 14, 31);
            SubTicks.Cursor = Cursors.IBeam;
            SubTicks.Font = new Font("Roboto", 12F);
            SubTicks.ForeColor = Color.White;
            SubTicks.Location = new Point(295, 501);
            SubTicks.Name = "SubTicks";
            SubTicks.PlaceholderText = "0";
            SubTicks.Size = new Size(100, 27);
            SubTicks.TabIndex = 5;
            SubTicks.TextChanged += TextBoxTextChanged;
            // 
            // SubCount
            // 
            SubCount.BackColor = Color.FromArgb(36, 14, 31);
            SubCount.Cursor = Cursors.IBeam;
            SubCount.Font = new Font("Roboto", 12F);
            SubCount.ForeColor = Color.White;
            SubCount.Location = new Point(295, 468);
            SubCount.Name = "SubCount";
            SubCount.PlaceholderText = "0";
            SubCount.Size = new Size(100, 27);
            SubCount.TabIndex = 6;
            SubCount.TextChanged += TextBoxTextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BorderStyle = BorderStyle.FixedSingle;
            label4.Font = new Font("Roboto", 12F);
            label4.ForeColor = Color.White;
            label4.Location = new Point(250, 444);
            label4.Name = "label4";
            label4.Size = new Size(145, 21);
            label4.TabIndex = 7;
            label4.Text = "'-' Operation Blocks";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Roboto", 12F);
            label5.ForeColor = Color.White;
            label5.Location = new Point(220, 476);
            label5.Name = "label5";
            label5.Size = new Size(69, 19);
            label5.TabIndex = 8;
            label5.Text = "Quantity";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Roboto", 12F);
            label6.ForeColor = Color.White;
            label6.Location = new Point(242, 509);
            label6.Name = "label6";
            label6.Size = new Size(47, 19);
            label6.TabIndex = 9;
            label6.Text = "Ticks";
            // 
            // MulTicks
            // 
            MulTicks.BackColor = Color.FromArgb(36, 14, 31);
            MulTicks.Cursor = Cursors.IBeam;
            MulTicks.Font = new Font("Roboto", 12F);
            MulTicks.ForeColor = Color.White;
            MulTicks.Location = new Point(485, 501);
            MulTicks.Name = "MulTicks";
            MulTicks.PlaceholderText = "0";
            MulTicks.Size = new Size(100, 27);
            MulTicks.TabIndex = 5;
            MulTicks.TextChanged += TextBoxTextChanged;
            // 
            // MulCount
            // 
            MulCount.BackColor = Color.FromArgb(36, 14, 31);
            MulCount.Cursor = Cursors.IBeam;
            MulCount.Font = new Font("Roboto", 12F);
            MulCount.ForeColor = Color.White;
            MulCount.Location = new Point(485, 468);
            MulCount.Name = "MulCount";
            MulCount.PlaceholderText = "0";
            MulCount.Size = new Size(100, 27);
            MulCount.TabIndex = 6;
            MulCount.TextChanged += TextBoxTextChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BorderStyle = BorderStyle.FixedSingle;
            label7.Font = new Font("Roboto", 12F);
            label7.ForeColor = Color.White;
            label7.Location = new Point(437, 444);
            label7.Name = "label7";
            label7.Size = new Size(148, 21);
            label7.TabIndex = 7;
            label7.Text = "'*' Operation Blocks";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Roboto", 12F);
            label8.ForeColor = Color.White;
            label8.Location = new Point(410, 476);
            label8.Name = "label8";
            label8.Size = new Size(69, 19);
            label8.TabIndex = 8;
            label8.Text = "Quantity";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Roboto", 12F);
            label9.ForeColor = Color.White;
            label9.Location = new Point(432, 509);
            label9.Name = "label9";
            label9.Size = new Size(47, 19);
            label9.TabIndex = 9;
            label9.Text = "Ticks";
            // 
            // DivTicks
            // 
            DivTicks.BackColor = Color.FromArgb(36, 14, 31);
            DivTicks.Cursor = Cursors.IBeam;
            DivTicks.Font = new Font("Roboto", 12F);
            DivTicks.ForeColor = Color.White;
            DivTicks.Location = new Point(678, 501);
            DivTicks.Name = "DivTicks";
            DivTicks.PlaceholderText = "0";
            DivTicks.Size = new Size(100, 27);
            DivTicks.TabIndex = 5;
            DivTicks.TextChanged += TextBoxTextChanged;
            // 
            // DivCount
            // 
            DivCount.BackColor = Color.FromArgb(36, 14, 31);
            DivCount.Cursor = Cursors.IBeam;
            DivCount.Font = new Font("Roboto", 12F);
            DivCount.ForeColor = Color.White;
            DivCount.Location = new Point(678, 468);
            DivCount.Name = "DivCount";
            DivCount.PlaceholderText = "0";
            DivCount.Size = new Size(100, 27);
            DivCount.TabIndex = 6;
            DivCount.TextChanged += TextBoxTextChanged;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BorderStyle = BorderStyle.FixedSingle;
            label10.Font = new Font("Roboto", 12F);
            label10.ForeColor = Color.White;
            label10.Location = new Point(630, 444);
            label10.Name = "label10";
            label10.Size = new Size(148, 21);
            label10.TabIndex = 7;
            label10.Text = "'/' Operation Blocks";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Roboto", 12F);
            label11.ForeColor = Color.White;
            label11.Location = new Point(603, 476);
            label11.Name = "label11";
            label11.Size = new Size(69, 19);
            label11.TabIndex = 8;
            label11.Text = "Quantity";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Roboto", 12F);
            label12.ForeColor = Color.White;
            label12.Location = new Point(625, 509);
            label12.Name = "label12";
            label12.Size = new Size(47, 19);
            label12.TabIndex = 9;
            label12.Text = "Ticks";
            // 
            // CalcButton
            // 
            CalcButton.BackColor = Color.FromArgb(36, 14, 31);
            CalcButton.Location = new Point(784, 444);
            CalcButton.Name = "CalcButton";
            CalcButton.Size = new Size(166, 84);
            CalcButton.TabIndex = 10;
            CalcButton.Text = "Calculate";
            CalcButton.UseVisualStyleBackColor = false;
            CalcButton.Click += CalcButton_Click;
            // 
            // ResultForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(48, 25, 52);
            ClientSize = new Size(984, 561);
            Controls.Add(CalcButton);
            Controls.Add(label12);
            Controls.Add(label9);
            Controls.Add(label6);
            Controls.Add(label3);
            Controls.Add(label11);
            Controls.Add(label8);
            Controls.Add(label5);
            Controls.Add(label2);
            Controls.Add(label10);
            Controls.Add(label7);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(DivCount);
            Controls.Add(DivTicks);
            Controls.Add(MulCount);
            Controls.Add(MulTicks);
            Controls.Add(SubCount);
            Controls.Add(SubTicks);
            Controls.Add(AddCount);
            Controls.Add(AddTicks);
            Controls.Add(ResultLabel);
            Controls.Add(AnalyzerLabel);
            Controls.Add(ResultBox);
            Controls.Add(InputBox);
            ForeColor = Color.AliceBlue;
            Name = "ResultForm";
            Load += LoadBuilder;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RichTextBox InputBox;
        private RichTextBox ResultBox;
        private System.Windows.Forms.Label AnalyzerLabel;
        private Label ResultLabel;
        private TextBox AddTicks;
        private TextBox AddCount;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox SubTicks;
        private TextBox SubCount;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox MulTicks;
        private TextBox MulCount;
        private Label label7;
        private Label label8;
        private Label label9;
        private TextBox DivTicks;
        private TextBox DivCount;
        private Label label10;
        private Label label11;
        private Label label12;
        private Button CalcButton;
    }
}
