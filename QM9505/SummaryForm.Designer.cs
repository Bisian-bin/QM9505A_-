
namespace QM9505
{
    partial class SummaryForm
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
            this.label566 = new System.Windows.Forms.Label();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.MessageCheck = new System.Windows.Forms.CheckBox();
            this.AlarmCheck = new System.Windows.Forms.CheckBox();
            this.OperateCheck = new System.Windows.Forms.CheckBox();
            this.DataCheck = new System.Windows.Forms.CheckBox();
            this.ErrorCheck = new System.Windows.Forms.CheckBox();
            this.CommCheck = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AlarmGridView = new System.Windows.Forms.DataGridView();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.dateTimeProductOverUp = new System.Windows.Forms.DateTimePicker();
            this.dateTimeProductBeginUp = new System.Windows.Forms.DateTimePicker();
            this.label895 = new System.Windows.Forms.Label();
            this.label896 = new System.Windows.Forms.Label();
            this.groupBox26.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AlarmGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label566
            // 
            this.label566.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.label566.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label566.Location = new System.Drawing.Point(7, 7);
            this.label566.Name = "label566";
            this.label566.Size = new System.Drawing.Size(1198, 34);
            this.label566.TabIndex = 33;
            this.label566.Text = "报警信息记录";
            this.label566.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.MessageCheck);
            this.groupBox26.Controls.Add(this.AlarmCheck);
            this.groupBox26.Controls.Add(this.OperateCheck);
            this.groupBox26.Controls.Add(this.DataCheck);
            this.groupBox26.Controls.Add(this.ErrorCheck);
            this.groupBox26.Controls.Add(this.CommCheck);
            this.groupBox26.Controls.Add(this.label1);
            this.groupBox26.Controls.Add(this.AlarmGridView);
            this.groupBox26.Controls.Add(this.btnSearch);
            this.groupBox26.Controls.Add(this.btnExcel);
            this.groupBox26.Controls.Add(this.dateTimeProductOverUp);
            this.groupBox26.Controls.Add(this.dateTimeProductBeginUp);
            this.groupBox26.Controls.Add(this.label895);
            this.groupBox26.Controls.Add(this.label896);
            this.groupBox26.Location = new System.Drawing.Point(5, 45);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(1200, 851);
            this.groupBox26.TabIndex = 32;
            this.groupBox26.TabStop = false;
            // 
            // MessageCheck
            // 
            this.MessageCheck.AutoSize = true;
            this.MessageCheck.Checked = true;
            this.MessageCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MessageCheck.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MessageCheck.Location = new System.Drawing.Point(232, 63);
            this.MessageCheck.Name = "MessageCheck";
            this.MessageCheck.Size = new System.Drawing.Size(104, 23);
            this.MessageCheck.TabIndex = 103;
            this.MessageCheck.Text = "提示信息";
            this.MessageCheck.UseVisualStyleBackColor = true;
            // 
            // AlarmCheck
            // 
            this.AlarmCheck.AutoSize = true;
            this.AlarmCheck.Checked = true;
            this.AlarmCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AlarmCheck.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AlarmCheck.Location = new System.Drawing.Point(97, 63);
            this.AlarmCheck.Name = "AlarmCheck";
            this.AlarmCheck.Size = new System.Drawing.Size(104, 23);
            this.AlarmCheck.TabIndex = 102;
            this.AlarmCheck.Text = "报警信息";
            this.AlarmCheck.UseVisualStyleBackColor = true;
            // 
            // OperateCheck
            // 
            this.OperateCheck.AutoSize = true;
            this.OperateCheck.Checked = true;
            this.OperateCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.OperateCheck.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.OperateCheck.Location = new System.Drawing.Point(367, 63);
            this.OperateCheck.Name = "OperateCheck";
            this.OperateCheck.Size = new System.Drawing.Size(104, 23);
            this.OperateCheck.TabIndex = 101;
            this.OperateCheck.Text = "操作信息";
            this.OperateCheck.UseVisualStyleBackColor = true;
            // 
            // DataCheck
            // 
            this.DataCheck.AutoSize = true;
            this.DataCheck.Checked = true;
            this.DataCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DataCheck.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DataCheck.Location = new System.Drawing.Point(772, 63);
            this.DataCheck.Name = "DataCheck";
            this.DataCheck.Size = new System.Drawing.Size(104, 23);
            this.DataCheck.TabIndex = 100;
            this.DataCheck.Text = "生产数据";
            this.DataCheck.UseVisualStyleBackColor = true;
            // 
            // ErrorCheck
            // 
            this.ErrorCheck.AutoSize = true;
            this.ErrorCheck.Checked = true;
            this.ErrorCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ErrorCheck.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ErrorCheck.Location = new System.Drawing.Point(637, 63);
            this.ErrorCheck.Name = "ErrorCheck";
            this.ErrorCheck.Size = new System.Drawing.Size(104, 23);
            this.ErrorCheck.TabIndex = 99;
            this.ErrorCheck.Text = "异常信息";
            this.ErrorCheck.UseVisualStyleBackColor = true;
            // 
            // CommCheck
            // 
            this.CommCheck.AutoSize = true;
            this.CommCheck.Checked = true;
            this.CommCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CommCheck.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CommCheck.Location = new System.Drawing.Point(502, 63);
            this.CommCheck.Name = "CommCheck";
            this.CommCheck.Size = new System.Drawing.Size(104, 23);
            this.CommCheck.TabIndex = 98;
            this.CommCheck.Text = "通讯信息";
            this.CommCheck.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(6, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 20);
            this.label1.TabIndex = 97;
            this.label1.Text = "日志类型:";
            // 
            // AlarmGridView
            // 
            this.AlarmGridView.AllowUserToAddRows = false;
            this.AlarmGridView.AllowUserToDeleteRows = false;
            this.AlarmGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AlarmGridView.Location = new System.Drawing.Point(6, 100);
            this.AlarmGridView.Name = "AlarmGridView";
            this.AlarmGridView.ReadOnly = true;
            this.AlarmGridView.RowHeadersWidth = 51;
            this.AlarmGridView.RowTemplate.Height = 23;
            this.AlarmGridView.Size = new System.Drawing.Size(1190, 745);
            this.AlarmGridView.TabIndex = 96;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.Control;
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(899, 16);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(116, 42);
            this.btnSearch.TabIndex = 95;
            this.btnSearch.Text = "查找";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.BackColor = System.Drawing.SystemColors.Control;
            this.btnExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExcel.Location = new System.Drawing.Point(1078, 16);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(116, 42);
            this.btnExcel.TabIndex = 94;
            this.btnExcel.Text = "导出";
            this.btnExcel.UseVisualStyleBackColor = false;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // dateTimeProductOverUp
            // 
            this.dateTimeProductOverUp.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimeProductOverUp.Location = new System.Drawing.Point(506, 20);
            this.dateTimeProductOverUp.Name = "dateTimeProductOverUp";
            this.dateTimeProductOverUp.Size = new System.Drawing.Size(202, 29);
            this.dateTimeProductOverUp.TabIndex = 93;
            // 
            // dateTimeProductBeginUp
            // 
            this.dateTimeProductBeginUp.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimeProductBeginUp.Location = new System.Drawing.Point(89, 20);
            this.dateTimeProductBeginUp.Name = "dateTimeProductBeginUp";
            this.dateTimeProductBeginUp.Size = new System.Drawing.Size(202, 29);
            this.dateTimeProductBeginUp.TabIndex = 92;
            // 
            // label895
            // 
            this.label895.AutoSize = true;
            this.label895.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label895.Location = new System.Drawing.Point(423, 24);
            this.label895.Name = "label895";
            this.label895.Size = new System.Drawing.Size(77, 20);
            this.label895.TabIndex = 91;
            this.label895.Text = "结束时间:";
            // 
            // label896
            // 
            this.label896.AutoSize = true;
            this.label896.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label896.Location = new System.Drawing.Point(6, 24);
            this.label896.Name = "label896";
            this.label896.Size = new System.Drawing.Size(77, 20);
            this.label896.TabIndex = 90;
            this.label896.Text = "开始时间:";
            // 
            // SummaryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 908);
            this.Controls.Add(this.label566);
            this.Controls.Add(this.groupBox26);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SummaryForm";
            this.Text = "SummaryForm";
            this.Load += new System.EventHandler(this.SummaryForm_Load);
            this.groupBox26.ResumeLayout(false);
            this.groupBox26.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AlarmGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label566;
        private System.Windows.Forms.GroupBox groupBox26;
        private System.Windows.Forms.CheckBox MessageCheck;
        private System.Windows.Forms.CheckBox AlarmCheck;
        private System.Windows.Forms.CheckBox OperateCheck;
        private System.Windows.Forms.CheckBox DataCheck;
        private System.Windows.Forms.CheckBox ErrorCheck;
        private System.Windows.Forms.CheckBox CommCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView AlarmGridView;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.DateTimePicker dateTimeProductOverUp;
        private System.Windows.Forms.DateTimePicker dateTimeProductBeginUp;
        private System.Windows.Forms.Label label895;
        private System.Windows.Forms.Label label896;
    }
}