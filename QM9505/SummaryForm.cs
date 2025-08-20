using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public partial class SummaryForm : Form
    {
        Access access = new Access();
        DataGrid dataGrid = new DataGrid();
        ExcelHelper excelHelper = new ExcelHelper();
        public int row;
        public int col;
        public SummaryForm()
        {
            InitializeComponent();
            dateTimeProductBeginUp.Value = DateTime.Now;//获取当时时间
            dateTimeProductOverUp.Value = DateTime.Now;//获取当时时间
        }

        #region 窗体加载
        private void SummaryForm_Load(object sender, EventArgs e)
        {
            btnSearch_Click(null, null);
        }
        #endregion

        #region 查询报警记录
        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            SearchAlarmMsg(AlarmGridView, dateTimeProductBeginUp, dateTimeProductOverUp);
            btnSearch.Enabled = true;
        }
        #endregion

        #region 将参数导出
        private void btnExcel_Click(object sender, EventArgs e)
        {
            btnExcel.Enabled = false;
            btnExcel.Text = "数据导出中";
            excelHelper.SaveAs(AlarmGridView);
            btnExcel.Enabled = true;
            btnExcel.Text = "导出";
        }
        #endregion

        #region 按照日期查询报警信息
        public void SearchAlarmMsg(DataGridView dataGridView1, DateTimePicker dtBeginSelect, DateTimePicker dtOverSelect)
        {
            string dtBegin = dtBeginSelect.Value.AddDays(-0).ToString("yyyy-MM-dd");
            string dtOver = dtOverSelect.Value.AddDays(+0).ToString("yyyy-MM-dd");
            try
            {
                string CONN = Access.GetSqlConnectionString();
                OleDbConnection conn = new OleDbConnection(CONN);
                string cmdText = "select Aldate as 日期,AlarmTime as 开始时间,logType as 报警类型,AlarmMsg as 报警信息 from AlarmMsg  where Aldate between #" + dtBegin + "# and #" + dtOver + "#";
                OleDbDataAdapter sda = new OleDbDataAdapter(cmdText, conn);
                DataSet ds = new DataSet();
                //manuDataTable(ds);
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!CommCheck.Checked)
                    {
                        if (ds.Tables[0].Rows[i][2].ToString() == "Comm")
                        {
                            ds.Tables[0].Rows.Remove(ds.Tables[0].Rows[i]);
                        }
                    }
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!ErrorCheck.Checked)
                    {
                        if (ds.Tables[0].Rows[i][2].ToString() == "Error")
                        {
                            ds.Tables[0].Rows.Remove(ds.Tables[0].Rows[i]);
                        }
                    }
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!AlarmCheck.Checked)
                    {
                        if (ds.Tables[0].Rows[i][2].ToString() == "Alarm")
                        {
                            ds.Tables[0].Rows.Remove(ds.Tables[0].Rows[i]);
                        }
                    }
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!OperateCheck.Checked)
                    {
                        if (ds.Tables[0].Rows[i][2].ToString() == "Operate")
                        {
                            ds.Tables[0].Rows.Remove(ds.Tables[0].Rows[i]);
                        }
                    }
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!MessageCheck.Checked)
                    {
                        if (ds.Tables[0].Rows[i][2].ToString() == "Message")
                        {
                            ds.Tables[0].Rows.Remove(ds.Tables[0].Rows[i]);
                        }
                    }
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!DataCheck.Checked)
                    {
                        if (ds.Tables[0].Rows[i][2].ToString() == "Data")
                        {
                            ds.Tables[0].Rows.Remove(ds.Tables[0].Rows[i]);
                        }
                    }
                }



                dataGrid.AutoSizeColumn(dataGridView1);
                //自适应后,再指定个别列的宽度
                dataGridView1.Columns[0].Width = 100;
                dataGridView1.Columns[1].Width = 180;
                dataGridView1.Columns[2].Width = 180;
                dataGridView1.RowHeadersVisible = false;//行头隐藏
                dataGridView1.AllowUserToResizeColumns = false;// 禁止改变所有列的列宽
                dataGridView1.AllowUserToResizeRows = false;//禁止改变所有行的行高
                dataGrid.DataGridViewchangeColor(dataGridView1, Color.White, Color.Blue);
                dataGrid.setColorColum(dataGridView1, 0);
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;//自动滚动到最后一行
            }
            catch
            {
                //MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion


        //获取行列坐标索引
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //获取行列坐标索引
            //方法一：
            row = e.RowIndex;
            col = e.ColumnIndex;
        }

       

        
    }
}
