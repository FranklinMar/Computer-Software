namespace LexSyntax_Analyzer
{
    partial class AnalyzerForm
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
            SuspendLayout();
            // 
            // AnalyzerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(48, 25, 52);
            ClientSize = new Size(984, 561);
            Font = new Font("Futura Bk BT", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.AliceBlue;
            Name = "AnalyzerForm";
            Text = "Analyzer";
            Load += AnalyzerForm_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}
