using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
//using System.Windows.Forms;

namespace QM9505
{
    class Access
    {
        DataGrid dataGrid = new DataGrid();

        #region 链接字符串
        /// <summary>
        /// 链接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetSqlConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["access"].ConnectionString;
        }
        #endregion

        #region 报警信息写入
        public void RecordAccess(Enum logType, string content)
        {
            //写入ACCESS文件里面
            try
            {
                string[] svValue = new string[4];
                svValue[0] = DateTime.Now.ToString("yyyy/MM/dd");
                svValue[1] = DateTime.Now.ToString("HH:mm:ss");
                svValue[2] = logType.ToString();
                svValue[3] = content;
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO AlarmMsg(Aldate,alarmTime,logType,AlarmMsg)" +
                            "values(@s1,@s2,@s3,@s4)";
                        string sr = "s";
                        for (int i = 0; i < svValue.Length; i++)
                        {
                            sr = "s" + (i + 1).ToString();
                            cmd.Parameters.AddWithValue(sr, svValue[i]);
                        }
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region 查找数据库BIB板信息
        public bool SearchDataRecord(System.Windows.Forms.DataGridView dataGridView, string comboPos1, string BIBListNum, string BIBRowNum)
        {
            string[] svValue = new string[200];
            string qr = comboPos1;
            int list = Convert.ToInt32(BIBListNum);
            int row = Convert.ToInt32(BIBRowNum);
            bool b = false;
            try
            {
                string CONN = Access.GetSqlConnectionString();
                OleDbConnection conn = new OleDbConnection(CONN);
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from BIBSetRecord where QR=@qr";
                    cmd.Parameters.AddWithValue("QR", qr);//QR
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            b = true;
                            for (int i = 0; i < 200; i++)
                            {
                                svValue[i] = reader["sv" + (i + 1).ToString()].ToString();
                                string[] svValue1 = svValue[i].Split(',');
                            }
                            for (int i = 0; i < row; i++)
                            {
                                for (int j = 0; j < list; j++)
                                {
                                    if (svValue[i * list + j] == "1")
                                    {
                                        dataGridView.Rows[i].Cells[j].Style.BackColor = Color.LightGreen;
                                    }
                                    else
                                    {
                                        dataGridView.Rows[i].Cells[j].Style.BackColor = Color.White;
                                    }
                                }
                            }
                        }
                    }
                }
                conn.Close();
            }
            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return b;
        }

        #endregion

        #region 根据DataGridView获取数据写入
        public string[] GetBIBDataGridValue(System.Windows.Forms.DataGridView dataGridView, string BIBListNum, string BIBRowNum)
        {
            string[] svValue = new string[200];
            for (int i = 0; i < Convert.ToInt32(BIBRowNum); i++)
            {
                for (int j = 0; j < Convert.ToInt32(BIBListNum); j++)
                {
                    if (dataGridView.Rows[i].Cells[j].Style.BackColor == Color.LightGreen)
                    {
                        svValue[i * Convert.ToInt32(BIBListNum) + j] = "1";
                    }
                    else if (dataGridView.Rows[i].Cells[j].Style.BackColor == Color.White)
                    {
                        svValue[i * Convert.ToInt32(BIBListNum) + j] = "0";
                    }
                }
            }
            for (int i = 0; i < 200; i++)
            {
                if (svValue[i] == null)
                {
                    svValue[i] = "0";
                }
            }
            return svValue;
        }
        #endregion

        //**********发送**********
        #region 搜索料号
        public void SearchSendData(ListBox listBox)
        {
            string CONN = Access.GetSqlConnectionString();
            using (OleDbConnection conn = new OleDbConnection(CONN))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from BIBSendRecord";

                    OleDbDataReader reader = cmd.ExecuteReader();
                    listBox.Items.Clear();

                    try
                    {
                        while (reader.Read())
                        {
                            listBox.Items.Add(reader["DataTime"].ToString() + "-" + reader["QR"].ToString());
                        }
                    }
                    catch
                    {
                        MessageBox.Show("沒有需求的信息!", "提示");
                    }
                }
                conn.Close();
            }
        }

        #endregion

        #region List点击查找数据库
        public void SearchSendList(System.Windows.Forms.DataGridView dataGridView1, string listbox)
        {
            string liao = listbox;
            if (liao.Length < 1)
                return;

            //将料号名称和程序分开

            string name = listbox;
            char[] delimiterChars = { '-', '\t' };
            string[] msg = name.Split(delimiterChars);

            try
            {
                string CONN = Access.GetSqlConnectionString();
                OleDbConnection conn = new OleDbConnection(CONN);
                string cmdText = "select * from BIBSendRecord  where QR ='" + msg[1] + "'";
                OleDbDataAdapter sda = new OleDbDataAdapter(cmdText, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGrid.AutoSizeColumn(dataGridView1);
                dataGridView1.RowHeadersVisible = false;//行头隐藏
                dataGrid.DataGridViewchangeColor(dataGridView1, Color.White, Color.Blue);
                dataGrid.setColorColum(dataGridView1, 0);
            }

            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 按时间查找数据库
        public void SearchSendData(DataGridView dataGridView1, DateTimePicker dtBeginSelect, DateTimePicker dtOverSelect)
        {
            if (dtBeginSelect.Text.Length != 0 && dtOverSelect.Text.Length != 0)
            {
                string dtBegin = dtBeginSelect.Value.AddDays(-0).ToString("yyyy-MM-dd");
                string dtOver = dtOverSelect.Value.AddDays(+0).ToString("yyyy-MM-dd");
                try
                {
                    string CONN = Access.GetSqlConnectionString();
                    OleDbConnection conn = new OleDbConnection(CONN);
                    string cmdText = "select * from BIBSendRecord  where DataTime between #" + dtBegin + "# and #" + dtOver + "#";

                    OleDbDataAdapter sda = new OleDbDataAdapter(cmdText, conn);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0];
                    dataGrid.AutoSizeColumn(dataGridView1);
                    dataGridView1.RowHeadersVisible = false;//行头隐藏
                    dataGrid.DataGridViewchangeColor(dataGridView1, Color.White, Color.Blue);
                    dataGrid.setColorColum(dataGridView1, 0);
                }
                catch
                {
                    MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("日期未选择!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #endregion

        //**********接受**********
        #region 搜索料号
        public void SearchReceiveData(ListBox listBox)
        {
            string CONN = Access.GetSqlConnectionString();
            using (OleDbConnection conn = new OleDbConnection(CONN))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from BIBReceiveRecord";

                    OleDbDataReader reader = cmd.ExecuteReader();
                    listBox.Items.Clear();

                    try
                    {
                        while (reader.Read())
                        {
                            listBox.Items.Add(reader["DataTime"].ToString() + "-" + reader["QR"].ToString());
                        }
                    }
                    catch
                    {
                        MessageBox.Show("沒有需求的信息!", "提示");
                    }
                }
                conn.Close();
            }
        }

        #endregion

        #region List点击查找数据库
        public void SearchReceiveList(System.Windows.Forms.DataGridView dataGridView1, string listbox)
        {
            string liao = listbox;
            if (liao.Length < 1)
                return;

            //将料号名称和程序分开

            string name = listbox;
            char[] delimiterChars = { '-', '\t' };
            string[] msg = name.Split(delimiterChars);

            try
            {
                string CONN = Access.GetSqlConnectionString();
                OleDbConnection conn = new OleDbConnection(CONN);
                string cmdText = "select * from BIBReceiveRecord  where QR ='" + msg[1] + "'";
                OleDbDataAdapter sda = new OleDbDataAdapter(cmdText, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGrid.AutoSizeColumn(dataGridView1);
                dataGridView1.RowHeadersVisible = false;//行头隐藏
                dataGrid.DataGridViewchangeColor(dataGridView1, Color.White, Color.Blue);
                dataGrid.setColorColum(dataGridView1, 0);
            }

            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 按时间查找数据库
        public void SearchReceiveData(DataGridView dataGridView1, DateTimePicker dtBeginSelect, DateTimePicker dtOverSelect)
        {
            if (dtBeginSelect.Text.Length != 0 && dtOverSelect.Text.Length != 0)
            {
                string dtBegin = dtBeginSelect.Value.AddDays(-0).ToString("yyyy-MM-dd");
                string dtOver = dtOverSelect.Value.AddDays(+0).ToString("yyyy-MM-dd");
                try
                {
                    string CONN = Access.GetSqlConnectionString();
                    OleDbConnection conn = new OleDbConnection(CONN);
                    string cmdText = "select * from BIBReceiveRecord  where DataTime between #" + dtBegin + "# and #" + dtOver + "#";

                    OleDbDataAdapter sda = new OleDbDataAdapter(cmdText, conn);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0];
                    dataGrid.AutoSizeColumn(dataGridView1);
                    dataGridView1.RowHeadersVisible = false;//行头隐藏
                    dataGrid.DataGridViewchangeColor(dataGridView1, Color.White, Color.Blue);
                    dataGrid.setColorColum(dataGridView1, 0);
                }
                catch
                {
                    MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("日期未选择!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #endregion


        //**********查询二维码信息**********
        #region 查询二维码信息
        public void SearchQRData(DataGridView dataGridView1)
        {
            try
            {
                string CONN = Access.GetSqlConnectionString();
                OleDbConnection conn = new OleDbConnection(CONN);
                conn.Open();
                string cmdText = "select * from QRRecord";

                OleDbDataAdapter sda = new OleDbDataAdapter(cmdText, conn);
                DataSet ds = new DataSet();
                //manuDataTable(ds);
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGrid.AutoSizeColumn(dataGridView1);
                //自适应后,再指定个别列的宽度
                dataGridView1.Columns[0].Width = 50;
                dataGridView1.Columns[1].Width = 50;
                dataGridView1.Columns[2].Width = 500;
                dataGridView1.RowHeadersVisible = false;//行头隐藏
                dataGrid.DataGridViewchangeColor(dataGridView1, Color.White, Color.Blue);
                dataGrid.setColorColum(dataGridView1, 0);
                conn.Close();
  
            }

            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion



    }
}
