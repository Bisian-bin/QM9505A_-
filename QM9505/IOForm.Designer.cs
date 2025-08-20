
namespace QM9505
{
    partial class IOForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridIN = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridOut = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.labAutoStep4 = new System.Windows.Forms.Label();
            this.labAutoStep0 = new System.Windows.Forms.Label();
            this.label827 = new System.Windows.Forms.Label();
            this.labAutoStep1 = new System.Windows.Forms.Label();
            this.label809 = new System.Windows.Forms.Label();
            this.labAutoStep2 = new System.Windows.Forms.Label();
            this.label807 = new System.Windows.Forms.Label();
            this.labAutoStep3 = new System.Windows.Forms.Label();
            this.label805 = new System.Windows.Forms.Label();
            this.labAutoStep5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridIN)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOut)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(6, 7);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1248, 898);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridIN);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1240, 865);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "输入";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridIN
            // 
            this.dataGridIN.AllowUserToAddRows = false;
            this.dataGridIN.AllowUserToDeleteRows = false;
            this.dataGridIN.AllowUserToResizeColumns = false;
            this.dataGridIN.AllowUserToResizeRows = false;
            this.dataGridIN.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dataGridIN.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridIN.Location = new System.Drawing.Point(6, 6);
            this.dataGridIN.Name = "dataGridIN";
            this.dataGridIN.RowTemplate.Height = 23;
            this.dataGridIN.Size = new System.Drawing.Size(1700, 853);
            this.dataGridIN.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridOut);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1240, 865);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "输出";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridOut
            // 
            this.dataGridOut.AllowUserToAddRows = false;
            this.dataGridOut.AllowUserToDeleteRows = false;
            this.dataGridOut.AllowUserToResizeColumns = false;
            this.dataGridOut.AllowUserToResizeRows = false;
            this.dataGridOut.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dataGridOut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridOut.Location = new System.Drawing.Point(6, 6);
            this.dataGridOut.Name = "dataGridOut";
            this.dataGridOut.RowTemplate.Height = 23;
            this.dataGridOut.Size = new System.Drawing.Size(1700, 853);
            this.dataGridOut.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1240, 865);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "步序监控";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labAutoStep5);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.labAutoStep4);
            this.groupBox2.Controls.Add(this.labAutoStep0);
            this.groupBox2.Controls.Add(this.label827);
            this.groupBox2.Controls.Add(this.labAutoStep1);
            this.groupBox2.Controls.Add(this.label809);
            this.groupBox2.Controls.Add(this.labAutoStep2);
            this.groupBox2.Controls.Add(this.label807);
            this.groupBox2.Controls.Add(this.labAutoStep3);
            this.groupBox2.Controls.Add(this.label805);
            this.groupBox2.Location = new System.Drawing.Point(2, 2);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(675, 419);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "自动流程监控";
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label15.Location = new System.Drawing.Point(22, 153);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(200, 40);
            this.label15.TabIndex = 50;
            this.label15.Text = "上层外自动";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labAutoStep4
            // 
            this.labAutoStep4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.labAutoStep4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labAutoStep4.Location = new System.Drawing.Point(231, 351);
            this.labAutoStep4.Name = "labAutoStep4";
            this.labAutoStep4.Size = new System.Drawing.Size(420, 40);
            this.labAutoStep4.TabIndex = 39;
            this.labAutoStep4.Text = "0";
            this.labAutoStep4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labAutoStep0
            // 
            this.labAutoStep0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.labAutoStep0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labAutoStep0.Location = new System.Drawing.Point(229, 21);
            this.labAutoStep0.Name = "labAutoStep0";
            this.labAutoStep0.Size = new System.Drawing.Size(420, 40);
            this.labAutoStep0.TabIndex = 31;
            this.labAutoStep0.Text = "0";
            this.labAutoStep0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label827
            // 
            this.label827.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label827.Location = new System.Drawing.Point(22, 21);
            this.label827.Name = "label827";
            this.label827.Size = new System.Drawing.Size(200, 40);
            this.label827.TabIndex = 32;
            this.label827.Text = "上层自动流程";
            this.label827.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labAutoStep1
            // 
            this.labAutoStep1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.labAutoStep1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labAutoStep1.Location = new System.Drawing.Point(229, 87);
            this.labAutoStep1.Name = "labAutoStep1";
            this.labAutoStep1.Size = new System.Drawing.Size(420, 40);
            this.labAutoStep1.TabIndex = 33;
            this.labAutoStep1.Text = "0";
            this.labAutoStep1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label809
            // 
            this.label809.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label809.Location = new System.Drawing.Point(22, 87);
            this.label809.Name = "label809";
            this.label809.Size = new System.Drawing.Size(200, 40);
            this.label809.TabIndex = 34;
            this.label809.Text = "上层内自动";
            this.label809.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labAutoStep2
            // 
            this.labAutoStep2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.labAutoStep2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labAutoStep2.Location = new System.Drawing.Point(229, 153);
            this.labAutoStep2.Name = "labAutoStep2";
            this.labAutoStep2.Size = new System.Drawing.Size(420, 40);
            this.labAutoStep2.TabIndex = 35;
            this.labAutoStep2.Text = "0";
            this.labAutoStep2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label807
            // 
            this.label807.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label807.Location = new System.Drawing.Point(22, 285);
            this.label807.Name = "label807";
            this.label807.Size = new System.Drawing.Size(200, 40);
            this.label807.TabIndex = 36;
            this.label807.Text = "下层内自动";
            this.label807.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labAutoStep3
            // 
            this.labAutoStep3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.labAutoStep3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labAutoStep3.Location = new System.Drawing.Point(229, 285);
            this.labAutoStep3.Name = "labAutoStep3";
            this.labAutoStep3.Size = new System.Drawing.Size(420, 40);
            this.labAutoStep3.TabIndex = 37;
            this.labAutoStep3.Text = "0";
            this.labAutoStep3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label805
            // 
            this.label805.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label805.Location = new System.Drawing.Point(22, 351);
            this.label805.Name = "label805";
            this.label805.Size = new System.Drawing.Size(200, 40);
            this.label805.TabIndex = 38;
            this.label805.Text = "下层外自动";
            this.label805.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labAutoStep5
            // 
            this.labAutoStep5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.labAutoStep5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labAutoStep5.Location = new System.Drawing.Point(229, 219);
            this.labAutoStep5.Name = "labAutoStep5";
            this.labAutoStep5.Size = new System.Drawing.Size(420, 40);
            this.labAutoStep5.TabIndex = 51;
            this.labAutoStep5.Text = "0";
            this.labAutoStep5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label2.Location = new System.Drawing.Point(22, 219);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 40);
            this.label2.TabIndex = 52;
            this.label2.Text = "下层自动流程";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // IOForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 908);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "IOForm";
            this.Text = "IOForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IOForm_FormClosing);
            this.Load += new System.EventHandler(this.IOForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridIN)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOut)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridIN;
        private System.Windows.Forms.DataGridView dataGridOut;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox2;
        internal System.Windows.Forms.Label label15;
        internal System.Windows.Forms.Label labAutoStep0;
        internal System.Windows.Forms.Label label827;
        internal System.Windows.Forms.Label labAutoStep1;
        internal System.Windows.Forms.Label label809;
        internal System.Windows.Forms.Label labAutoStep2;
        internal System.Windows.Forms.Label labAutoStep4;
        internal System.Windows.Forms.Label label807;
        internal System.Windows.Forms.Label labAutoStep3;
        internal System.Windows.Forms.Label label805;
        internal System.Windows.Forms.Label labAutoStep5;
        internal System.Windows.Forms.Label label2;
    }
}