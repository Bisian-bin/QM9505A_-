using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public partial class RecordForm : Form
    {
        Access access = new Access();
        DataGrid dataGrid = new DataGrid();
        ExcelHelper excelHelper = new ExcelHelper();
        public RecordForm()
        {
            InitializeComponent();
        }

        #region 窗体加载
        private void RecordForm_Load(object sender, EventArgs e)
        {
            //dataGrid初始化
            dataGrid.IniLeftModelTrayY(U_1DataGrid, 16, 19);
            dataGrid.IniLeftModelTrayY(U_2DataGrid, 16, 19);
            dataGrid.IniLeftModelTrayY(D_1DataGrid, 16, 19);
            dataGrid.IniLeftModelTrayY(D_2DataGrid, 16, 19);
        }
        #endregion

        //<<----------上层内搜索数据库---------->>

        #region 上层搜索

        private void BtnSearchUp_1_Click(object sender, EventArgs e)
        {
            if (textBoxViewUp_1.Text == "")
            {
                SearchDataUp1(ListBoxViewUp_1);//将结果显示在界面
            }
            else
            {
                try
                {
                    string CONN = Access.GetSqlConnectionString();
                    ListBoxViewUp_1.Items.Clear();
                    OleDbConnection conn = new OleDbConnection(CONN);//连接
                    conn.Open();//打开
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select  * from UpTotalRecord1 where PN like '%" + textBoxViewUp_1.Text.Trim() + "%'";
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ListBoxViewUp_1.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
                    }
                    conn.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }
            }
        }

        #endregion

        #region 上层搜索料号
        public void SearchDataUp1(ListBox listBox)
        {
            string CONN = Access.GetSqlConnectionString();
            using (OleDbConnection conn = new OleDbConnection(CONN))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from UpTotalRecord1";

                    OleDbDataReader reader = cmd.ExecuteReader();
                    listBox.Items.Clear();

                    try
                    {
                        while (reader.Read())
                        {
                            listBox.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
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

        #region 上层ListBox单击

        private void ListBoxViewUp_1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string str = (sender as ListBox).SelectedItem.ToString();
            if (str.Length < 1)
            {
                return;
            }
            string[] data = str.Split('/');
            string PN = data[3];
            string Num = data[4];
            //上层总数
            try
            {
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from UpTotalRecord1 where PN = @PN and Num = @Num";
                        cmd.Parameters.AddWithValue("PN", PN);//料号
                        cmd.Parameters.AddWithValue("Num", Num);//序号

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string[] svValue = new string[152];
                                for (int a = 0; a < 152; a++)
                                {
                                    svValue[a] = reader["sv" + (a + 1).ToString()].ToString();
                                }

                                for (int i = 0; i < 19; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        U_1DataGrid.Rows[j * 2].Cells[i].Style.ForeColor = Color.Blue;
                                        U_1DataGrid.Rows[j * 2].Cells[i].Value = svValue[i * 8 + j];
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }

            //上层良率
            try
            {
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from UpYieldRecord1 where PN = @PN and Num = @Num";
                        cmd.Parameters.AddWithValue("PN", PN);//料号
                        cmd.Parameters.AddWithValue("Num", Num);//序号

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string[] svValue = new string[152];
                                for (int a = 0; a < 152; a++)
                                {
                                    svValue[a] = reader["sv" + (a + 1).ToString()].ToString();
                                }

                                for (int i = 0; i < 19; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        U_1DataGrid.Rows[j * 2 + 1].Cells[i].Style.ForeColor = Color.Gray;
                                        U_1DataGrid.Rows[j * 2 + 1].Cells[i].Value = svValue[i * 8 + j];
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }

        }

        #endregion

        #region 上层数据查找
        private void btnProductSearchUp_1_Click(object sender, EventArgs e)
        {
            btnProductSearchUp_1.Enabled = false;
            SearchAsDataUp1(dateTimeProductBeginUp_1, dateTimeProductOverUp_1);
            btnProductSearchUp_1.Enabled = true;
        }

        #endregion

        #region 上层按时间查找数据库

        /// <summary>
        /// 查找数据库ProductRecord
        /// </summary>
        /// <param name="dataGridView1"></param>
        /// <param name="dtBeginSelect"></param>
        /// <param name="dtOverSelect"></param>
        public void SearchAsDataUp1(DateTimePicker dtBeginSelect, DateTimePicker dtOverSelect)
        {
            if (dtBeginSelect.Text.Length != 0 && dtOverSelect.Text.Length != 0)
            {
                string dtBegin = dtBeginSelect.Value.AddDays(-0).ToString("yyyy-MM-dd");
                string dtOver = dtOverSelect.Value.AddDays(+0).ToString("yyyy-MM-dd");
                try
                {
                    string CONN = Access.GetSqlConnectionString();
                    ListBoxViewUp_1.Items.Clear();
                    OleDbConnection conn = new OleDbConnection(CONN);//连接
                    conn.Open();//打开
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select  * from UpTotalRecord1 where Aldate between #" + dtBegin + "# and #" + dtOver + "#";
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ListBoxViewUp_1.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
                    }
                    conn.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }
            }
            else
            {
                MessageBox.Show("日期未选择!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        //<<----------上层外搜索数据库---------->>

        #region 上层搜索

        private void BtnSearchUp_2_Click(object sender, EventArgs e)
        {
            if (textBoxViewUp_2.Text == "")
            {
                SearchDataUp2(ListBoxViewUp_2);//将结果显示在界面
            }
            else
            {
                try
                {
                    string CONN = Access.GetSqlConnectionString();
                    ListBoxViewUp_2.Items.Clear();
                    OleDbConnection conn = new OleDbConnection(CONN);//连接
                    conn.Open();//打开
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select  * from UpTotalRecord2 where PN like '%" + textBoxViewUp_2.Text.Trim() + "%'";
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ListBoxViewUp_2.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
                    }
                    conn.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }
            }
        }

        #endregion

        #region 上层搜索料号
        public void SearchDataUp2(ListBox listBox)
        {
            string CONN = Access.GetSqlConnectionString();
            using (OleDbConnection conn = new OleDbConnection(CONN))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from UpTotalRecord2";

                    OleDbDataReader reader = cmd.ExecuteReader();
                    listBox.Items.Clear();

                    try
                    {
                        while (reader.Read())
                        {
                            listBox.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
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

        #region 上层ListBox单击

        private void ListBoxViewUp_2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string str = (sender as ListBox).SelectedItem.ToString();
            if (str.Length < 1)
            {
                return;
            }
            string[] data = str.Split('/');
            string PN = data[3];
            string Num = data[4];
            //上层总数
            try
            {
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from UpTotalRecord2 where PN = @PN and Num = @Num";
                        cmd.Parameters.AddWithValue("PN", PN);//料号
                        cmd.Parameters.AddWithValue("Num", Num);//序号

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string[] svValue = new string[152];
                                for (int a = 0; a < 152; a++)
                                {
                                    svValue[a] = reader["sv" + (a + 1).ToString()].ToString();
                                }

                                for (int i = 0; i < 19; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        U_2DataGrid.Rows[j * 2].Cells[i].Style.ForeColor = Color.Blue;
                                        U_2DataGrid.Rows[j * 2].Cells[i].Value = svValue[i * 8 + j];
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }

            //上层良率
            try
            {
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from UpYieldRecord2 where PN = @PN and Num = @Num";
                        cmd.Parameters.AddWithValue("PN", PN);//料号
                        cmd.Parameters.AddWithValue("Num", Num);//序号

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string[] svValue = new string[152];
                                for (int a = 0; a < 152; a++)
                                {
                                    svValue[a] = reader["sv" + (a + 1).ToString()].ToString();
                                }

                                for (int i = 0; i < 19; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        U_2DataGrid.Rows[j * 2 + 1].Cells[i].Style.ForeColor = Color.Gray;
                                        U_2DataGrid.Rows[j * 2 + 1].Cells[i].Value = svValue[i * 8 + j];
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }

        }

        #endregion

        #region 上层数据查找
        private void btnProductSearchUp_2_Click(object sender, EventArgs e)
        {
            btnProductSearchUp_1.Enabled = false;
            SearchAsDataUp2(dateTimeProductBeginUp_2, dateTimeProductOverUp_2);
            btnProductSearchUp_1.Enabled = true;
        }

        #endregion

        #region 上层按时间查找数据库

        /// <summary>
        /// 查找数据库ProductRecord
        /// </summary>
        /// <param name="dataGridView1"></param>
        /// <param name="dtBeginSelect"></param>
        /// <param name="dtOverSelect"></param>
        public void SearchAsDataUp2(DateTimePicker dtBeginSelect, DateTimePicker dtOverSelect)
        {
            if (dtBeginSelect.Text.Length != 0 && dtOverSelect.Text.Length != 0)
            {
                string dtBegin = dtBeginSelect.Value.AddDays(-0).ToString("yyyy-MM-dd");
                string dtOver = dtOverSelect.Value.AddDays(+0).ToString("yyyy-MM-dd");
                try
                {
                    string CONN = Access.GetSqlConnectionString();
                    ListBoxViewUp_2.Items.Clear();
                    OleDbConnection conn = new OleDbConnection(CONN);//连接
                    conn.Open();//打开
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select  * from UpTotalRecord2 where Aldate between #" + dtBegin + "# and #" + dtOver + "#";
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ListBoxViewUp_2.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
                    }
                    conn.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }
            }
            else
            {
                MessageBox.Show("日期未选择!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        //<<----------下层内搜索数据库---------->>

        #region 下层搜索

        private void BtnSearchDown_1_Click(object sender, EventArgs e)
        {
            if (textBoxViewDown_1.Text == "")
            {
                SearchDataDown1(ListBoxViewDown_1);//将结果显示在界面
            }
            else
            {
                try
                {
                    string CONN = Access.GetSqlConnectionString();
                    ListBoxViewDown_1.Items.Clear();
                    OleDbConnection conn = new OleDbConnection(CONN);//连接
                    conn.Open();//打开
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select  * from DownTotalRecord1 where PN like '%" + textBoxViewDown_1.Text.Trim() + "%'";
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ListBoxViewDown_1.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
                    }
                    conn.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }
            }
        }

        #endregion

        #region 下层搜索料号

        public void SearchDataDown1(ListBox listBox)
        {
            string CONN = Access.GetSqlConnectionString();
            using (OleDbConnection conn = new OleDbConnection(CONN))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from DownTotalRecord1";

                    OleDbDataReader reader = cmd.ExecuteReader();
                    listBox.Items.Clear();

                    try
                    {
                        while (reader.Read())
                        {
                            listBox.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
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

        #region 下层ListBox单击

        private void ListBoxViewDown_1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string str = (sender as ListBox).SelectedItem.ToString();
            if (str.Length < 1)
            {
                return;
            }
            string[] data = str.Split('/');
            string PN = data[3];
            string Num = data[4];

            //下层总数
            try
            {
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from DownTotalRecord1 where PN = @PN and Num = @Num";
                        cmd.Parameters.AddWithValue("PN", PN);//料号
                        cmd.Parameters.AddWithValue("Num", Num);//序号

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string[] svValue = new string[152];
                                for (int a = 0; a < 152; a++)
                                {
                                    svValue[a] = reader["sv" + (a + 1).ToString()].ToString();
                                }

                                for (int i = 0; i < 19; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        D_1DataGrid.Rows[j * 2].Cells[i].Style.ForeColor = Color.Blue;
                                        D_1DataGrid.Rows[j * 2].Cells[i].Value = svValue[i * 8 + j];
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }

            //下层良率
            try
            {
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from UpYieldRecord1 where PN = @PN and Num = @Num";
                        cmd.Parameters.AddWithValue("PN", PN);//料号
                        cmd.Parameters.AddWithValue("Num", Num);//序号

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string[] svValue = new string[152];
                                for (int a = 0; a < 152; a++)
                                {
                                    svValue[a] = reader["sv" + (a + 1).ToString()].ToString();
                                }

                                for (int i = 0; i < 19; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        D_1DataGrid.Rows[j * 2 + 1].Cells[i].Style.ForeColor = Color.Gray;
                                        D_1DataGrid.Rows[j * 2 + 1].Cells[i].Value = svValue[i * 8 + j];
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }

        #endregion

        #region 下层数据查找

        private void btnProductSearchDown_1_Click(object sender, EventArgs e)
        {
            btnProductSearchDown_1.Enabled = false;
            SearchAsDataDown1(dateTimeProductBeginDown_1, dateTimeProductOverDown_1);
            btnProductSearchDown_1.Enabled = true;
        }

        #endregion

        #region 下层按时间查找数据库

        /// <summary>
        /// 查找数据库ProductRecord
        /// </summary>
        /// <param name="dataGridView1"></param>
        /// <param name="dtBeginSelect"></param>
        /// <param name="dtOverSelect"></param>
        public void SearchAsDataDown1(DateTimePicker dtBeginSelect, DateTimePicker dtOverSelect)
        {
            if (dtBeginSelect.Text.Length != 0 && dtOverSelect.Text.Length != 0)
            {
                string dtBegin = dtBeginSelect.Value.AddDays(-0).ToString("yyyy-MM-dd");
                string dtOver = dtOverSelect.Value.AddDays(+0).ToString("yyyy-MM-dd");
                try
                {
                    string CONN = Access.GetSqlConnectionString();
                    ListBoxViewDown_1.Items.Clear();
                    OleDbConnection conn = new OleDbConnection(CONN);//连接
                    conn.Open();//打开
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select  * from DownTotalRecord1 where Aldate between #" + dtBegin + "# and #" + dtOver + "#";
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ListBoxViewDown_1.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
                    }
                    conn.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }
            }
            else
            {
                MessageBox.Show("日期未选择!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        //<<----------下层外搜索数据库---------->>

        #region 下层搜索

        private void BtnSearchDown_2_Click(object sender, EventArgs e)
        {
            if (textBoxViewDown_2.Text == "")
            {
                SearchDataDown2(ListBoxViewDown_2);//将结果显示在界面
            }
            else
            {
                try
                {
                    string CONN = Access.GetSqlConnectionString();
                    ListBoxViewDown_2.Items.Clear();
                    OleDbConnection conn = new OleDbConnection(CONN);//连接
                    conn.Open();//打开
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select  * from DownTotalRecord2 where PN like '%" + textBoxViewDown_2.Text.Trim() + "%'";
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ListBoxViewDown_2.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
                    }
                    conn.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }
            }
        }

        #endregion

        #region 下层搜索料号

        public void SearchDataDown2(ListBox listBox)
        {
            string CONN = Access.GetSqlConnectionString();
            using (OleDbConnection conn = new OleDbConnection(CONN))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from DownTotalRecord2";

                    OleDbDataReader reader = cmd.ExecuteReader();
                    listBox.Items.Clear();

                    try
                    {
                        while (reader.Read())
                        {
                            listBox.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
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

        #region 下层ListBox单击

        private void ListBoxViewDown_2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string str = (sender as ListBox).SelectedItem.ToString();
            if (str.Length < 1)
            {
                return;
            }
            string[] data = str.Split('/');
            string PN = data[3];
            string Num = data[4];

            //下层总数
            try
            {
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from DownTotalRecord2 where PN = @PN and Num = @Num";
                        cmd.Parameters.AddWithValue("PN", PN);//料号
                        cmd.Parameters.AddWithValue("Num", Num);//序号

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string[] svValue = new string[152];
                                for (int a = 0; a < 152; a++)
                                {
                                    svValue[a] = reader["sv" + (a + 1).ToString()].ToString();
                                }

                                for (int i = 0; i < 19; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        D_2DataGrid.Rows[j * 2].Cells[i].Style.ForeColor = Color.Blue;
                                        D_2DataGrid.Rows[j * 2].Cells[i].Value = svValue[i * 8 + j];
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }

            //下层良率
            try
            {
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from UpYieldRecord2 where PN = @PN and Num = @Num";
                        cmd.Parameters.AddWithValue("PN", PN);//料号
                        cmd.Parameters.AddWithValue("Num", Num);//序号

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string[] svValue = new string[152];
                                for (int a = 0; a < 152; a++)
                                {
                                    svValue[a] = reader["sv" + (a + 1).ToString()].ToString();
                                }

                                for (int i = 0; i < 19; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        D_2DataGrid.Rows[j * 2 + 1].Cells[i].Style.ForeColor = Color.Gray;
                                        D_2DataGrid.Rows[j * 2 + 1].Cells[i].Value = svValue[i * 8 + j];
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("无资料查询!", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }

        #endregion

        #region 下层数据查找

        private void btnProductSearchDown_2_Click(object sender, EventArgs e)
        {
            btnProductSearchDown_2.Enabled = false;
            SearchAsDataDown2(dateTimeProductBeginDown_2, dateTimeProductOverDown_2);
            btnProductSearchDown_2.Enabled = true;
        }

        #endregion

        #region 下层按时间查找数据库

        /// <summary>
        /// 查找数据库ProductRecord
        /// </summary>
        /// <param name="dataGridView1"></param>
        /// <param name="dtBeginSelect"></param>
        /// <param name="dtOverSelect"></param>
        public void SearchAsDataDown2(DateTimePicker dtBeginSelect, DateTimePicker dtOverSelect)
        {
            if (dtBeginSelect.Text.Length != 0 && dtOverSelect.Text.Length != 0)
            {
                string dtBegin = dtBeginSelect.Value.AddDays(-0).ToString("yyyy-MM-dd");
                string dtOver = dtOverSelect.Value.AddDays(+0).ToString("yyyy-MM-dd");
                try
                {
                    string CONN = Access.GetSqlConnectionString();
                    ListBoxViewDown_2.Items.Clear();
                    OleDbConnection conn = new OleDbConnection(CONN);//连接
                    conn.Open();//打开
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select  * from DownTotalRecord2 where Aldate between #" + dtBegin + "# and #" + dtOver + "#";
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ListBoxViewDown_2.Items.Add(reader["Aldate"].ToString() + "/" + reader["PN"].ToString() + "/" + reader["Num"].ToString());
                    }
                    conn.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }
            }
            else
            {
                MessageBox.Show("日期未选择!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        //<<----------数据导出到Excel---------->>

        #region 数据导出

        private void btnProductSaveUp_1_Click(object sender, EventArgs e)
        {
            //DataGridView数据
            btnProductSaveUp_1.Enabled = false;
            ReportToExcel(U_1DataGrid, "上层内");
            btnProductSaveUp_1.Enabled = true;
        }
        private void btnProductSaveUp_2_Click(object sender, EventArgs e)
        {
            //DataGridView数据
            btnProductSaveUp_2.Enabled = false;
            ReportToExcel(U_2DataGrid, "上层内");
            btnProductSaveUp_2.Enabled = true;
        }
        private void btnProductSaveDown_1_Click(object sender, EventArgs e)
        {
            //DataGridView数据
            btnProductSaveDown_1.Enabled = false;
            ReportToExcel(D_1DataGrid, "下层");
            btnProductSaveDown_1.Enabled = true;
        }
        private void btnProductSaveDown_2_Click(object sender, EventArgs e)
        {
            //DataGridView数据
            btnProductSaveDown_2.Enabled = false;
            ReportToExcel(D_2DataGrid, "下层");
            btnProductSaveDown_2.Enabled = true;
        }
        #endregion

        #region 数据导出到Excel
        public void ReportToExcel(DataGridView dataGrid, string strs)
        {
            DateTime dt = DateTime.Now;
            string sj = dt.ToString("yyyyMMddHHmmss");
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl表格文件 (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "导出Excel文件到";
            saveFileDialog.FileName = this.Text + strs + sj;
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            Stream myStream;
            myStream = saveFileDialog.OpenFile();

            //StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            // StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
            StreamWriter sw = new StreamWriter(myStream, System.Text.ASCIIEncoding.Unicode);//这样不会出现乱码

            string str = "";
            try
            {
                //写标题
                for (int i = 0; i < dataGrid.ColumnCount; i++)
                {
                    if (i > 0)
                    {
                        str += "\t";
                    }
                    str += dataGrid.Columns[i].HeaderText;
                }
                sw.WriteLine(str);
                //写内容
                for (int j = 0; j < dataGrid.Rows.Count; j++)
                {
                    string tempStr = "";
                    for (int k = 0; k < dataGrid.Columns.Count; k++)
                    {
                        if (k > 0)
                        {
                            tempStr += "\t";
                        }
                        if (dataGrid.Rows[j].Cells[k].Value != null)
                        {
                            tempStr += dataGrid.Rows[j].Cells[k].Value.ToString();
                        }
                    }
                    sw.WriteLine(tempStr);
                }
                MessageBox.Show("导出成功!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Information);
                sw.Close();
                myStream.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
        }



























        #endregion


    }
}
