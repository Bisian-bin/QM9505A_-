namespace QM9505
{
    partial class TCPForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.components = new System.ComponentModel.Container();
            this.timerScan = new System.Windows.Forms.Timer(this.components);
            this.CommRecord = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // CommRecord
            // 
            this.CommRecord.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CommRecord.FormattingEnabled = true;
            this.CommRecord.ItemHeight = 21;
            this.CommRecord.Location = new System.Drawing.Point(3, 3);
            this.CommRecord.Name = "CommRecord";
            this.CommRecord.Size = new System.Drawing.Size(1248, 886);
            this.CommRecord.TabIndex = 2;
            // 
            // TCPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 908);
            this.Controls.Add(this.CommRecord);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TCPForm";
            this.Text = "TCPForm";
            this.Load += new System.EventHandler(this.TCPForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerScan;
        private System.Windows.Forms.ListBox CommRecord;
    }
}