using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    class DataGrid
    {
        #region 禁止更改宽高
        public void NotChangeListRow(DataGridView dataGridView1)
        {
            // 禁止用户改变DataGridView的所有列的列宽   
            dataGridView1.AllowUserToResizeColumns = false;
            //禁止用户改变DataGridView所有行的行高   
            dataGridView1.AllowUserToResizeRows = false;
            // 禁止用户改变列头的高度   
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            // 禁止用户改变列头的宽度   
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
        }

        #endregion

        #region 自动适应列宽
        public void AutoSizeColumn(DataGridView dgViewFiles)//自动适应列宽
        {
            int width = 0;
            //对于DataGridView的每一个列都调整
            //将每一列都调整为自动适应模式
            for (int i = 0; i < dgViewFiles.Columns.Count; i++)
            {  //将每一列都调整为自动适应模式
                dgViewFiles.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);
                //记录整个DataGridView的宽度
                width += dgViewFiles.Columns[i].Width;
            }
            //判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，
            //则将DataGridView的列自动调整模式设置为显示的列即可，
            //如果是小于原来设定的宽度，将模式改为填充。
            if (width > dgViewFiles.Size.Width)
            {
                dgViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            else
            {
                dgViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            //冻结某列 从左开始 0，1，2
            dgViewFiles.Columns[1].Frozen = true;
        }

        #endregion

        #region 改变DataGridView 表头颜色
        public void DataGridViewchangeColor(DataGridView dataGridView1, Color backColor, Color fontColor)//改变DataGridView 表头颜色
        {
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = backColor;
            columnHeaderStyle.ForeColor = fontColor;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
        }

        #endregion

        #region 设置列的背景色
        public void setColorColum(DataGridView D1, int i)//•设置列的背景色
        {
            Color GridReadOnlyColor = Color.GreenYellow;
            D1.Columns[i].DefaultCellStyle.BackColor = GridReadOnlyColor;
        }

        #endregion

        #region LoadTray初始化白色
        public void IniLeftLoadTrayW(DataGridView dataGridView, double TrayRowNum, double TrayListNum)
        {
            try
            {    //先控件X方向数量；Y方向数量
                dataGridView.RowCount = Convert.ToInt16(TrayRowNum);
                dataGridView.ColumnCount = Convert.ToInt16(TrayListNum);
                for (int a = 0; a < Convert.ToInt16(TrayListNum); a++)//X方向
                {
                    for (int b = 0; b < Convert.ToInt16(TrayRowNum); b++)//Y方向
                    {
                        dataGridView.Rows[b].Cells[a].Style.BackColor = Color.White;
                        if (a == 0)
                        {
                            dataGridView.Rows[b].Cells[a].Value = b + 1;
                            dataGridView.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                            dataGridView.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        if (b == 0)
                        {
                            dataGridView.Rows[b].Cells[a].Value = a + 1;
                            dataGridView.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                            dataGridView.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                }
                //设置控件高度
                for (int a = 0; a < Convert.ToInt16(TrayRowNum); a++)
                {
                    dataGridView.Rows[a].Height = 18;//高度
                }
                //设置控件宽度
                for (int a = 0; a < Convert.ToInt16(TrayListNum); a++)
                {
                    dataGridView.Columns[a].Width = 45;
                }
                //取消取消标记
                dataGridView.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tray初始化，异常信息如下：" + ex.Message, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }

        #endregion

        #region LoadTray初始化绿色
        public void IniLeftLoadTrayG(DataGridView dataGridView, double TrayRowNum, double TrayListNum)
        {
            try
            {    //先控件X方向数量；Y方向数量
                dataGridView.RowCount = Convert.ToInt16(TrayRowNum);
                dataGridView.ColumnCount = Convert.ToInt16(TrayListNum);
                for (int a = 0; a < Convert.ToInt16(TrayListNum); a++)//X方向
                {
                    for (int b = 0; b < Convert.ToInt16(TrayRowNum); b++)//Y方向
                    {
                        dataGridView.Rows[b].Cells[a].Style.BackColor = Color.Green;
                        if (a == 0)
                        {
                            dataGridView.Rows[b].Cells[a].Value = b + 1;
                            dataGridView.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                            dataGridView.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        if (b == 0)
                        {
                            dataGridView.Rows[b].Cells[a].Value = a + 1;
                            dataGridView.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                            dataGridView.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                }
                //设置控件高度
                for (int a = 0; a < Convert.ToInt16(TrayRowNum); a++)
                {
                    dataGridView.Rows[a].Height = 20;//高度
                }
                //设置控件宽度
                for (int a = 0; a < Convert.ToInt16(TrayListNum); a++)
                {
                    dataGridView.Columns[a].Width = 24;
                }
                //取消取消标记
                dataGridView.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tray初始化，异常信息如下：" + ex.Message, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }

        #endregion

        #region ModelTray初始化白色
        public void IniLeftModelTrayW(DataGridView dataGridView, double TrayRowNum, double TrayListNum)
        {
            try
            {    //先控件X方向数量；Y方向数量
                dataGridView.RowCount = Convert.ToInt16(TrayRowNum);
                dataGridView.ColumnCount = Convert.ToInt16(TrayListNum);
                for (int a = 0; a < Convert.ToInt16(TrayListNum); a++)//X方向
                {
                    for (int b = 0; b < Convert.ToInt16(TrayRowNum); b++)//Y方向
                    {
                        dataGridView.Rows[b].Cells[a].Style.BackColor = Color.White;
                        if (a == 0)
                        {
                            dataGridView.Rows[b].Cells[a].Value = b + 1;
                            dataGridView.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                            dataGridView.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        if (b == 0)
                        {
                            dataGridView.Rows[b].Cells[a].Value = a + 1;
                            dataGridView.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                            dataGridView.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                }
                //设置控件高度
                for (int a = 0; a < Convert.ToInt16(TrayRowNum); a++)
                {
                        dataGridView.Rows[a].Height = 24;//高度
                }
                //设置控件宽度
                for (int a = 0; a < Convert.ToInt16(TrayListNum); a++)
                {
                        dataGridView.Columns[a].Width = 57;
                }
                //取消取消标记
                dataGridView.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tray初始化，异常信息如下：" + ex.Message, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }

        #endregion

        #region ModelTray初始化绿色
        public void IniLeftModelTrayG(DataGridView dataGridView, double TrayRowNum, double TrayListNum)
        {
            try
            {    //先控件X方向数量；Y方向数量
                dataGridView.RowCount = Convert.ToInt16(TrayRowNum);
                dataGridView.ColumnCount = Convert.ToInt16(TrayListNum);
                for (int a = 0; a < Convert.ToInt16(TrayListNum); a++)//X方向
                {
                    for (int b = 0; b < Convert.ToInt16(TrayRowNum); b++)//Y方向
                    {
                        dataGridView.Rows[b].Cells[a].Style.BackColor = Color.Green;
                        if (a == 0)
                        {
                            dataGridView.Rows[b].Cells[a].Value = b + 1;
                            dataGridView.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                            dataGridView.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        if (b == 0)
                        {
                            dataGridView.Rows[b].Cells[a].Value = a + 1;
                            dataGridView.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                            dataGridView.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                }
                //设置控件高度
                for (int a = 0; a < Convert.ToInt16(TrayRowNum); a++)
                {
                    if (Convert.ToInt16(TrayRowNum) > 21)
                    {
                        dataGridView.Rows[a].Height = 15;//高度
                    }
                    else
                    {
                        dataGridView.Rows[a].Height = 18;//高度
                    }
                }
                //设置控件宽度
                for (int a = 0; a < Convert.ToInt16(TrayListNum); a++)
                {
                    if (Convert.ToInt16(TrayListNum) > 11)
                    {
                        dataGridView.Columns[a].Width = 10;
                    }
                    else
                    {
                        dataGridView.Columns[a].Width = 26;
                    }
                }
                //取消取消标记
                dataGridView.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tray初始化，异常信息如下：" + ex.Message, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }

        #endregion

        #region Tray初始化白色
        public void IniLeftModelTrayY(DataGridView dataGridView, double TrayRowNum, double TrayListNum)
        {
            try
            {    //先控件X方向数量；Y方向数量
                dataGridView.RowCount = Convert.ToInt16(TrayRowNum);
                dataGridView.ColumnCount = Convert.ToInt16(TrayListNum);
                for (int a = 0; a < Convert.ToInt16(TrayListNum); a++)//X方向
                {
                    for (int b = 0; b < Convert.ToInt16(TrayRowNum); b++)//Y方向
                    {
                        dataGridView.Rows[b].Cells[a].Style.BackColor = Color.White;
                        if (a == 0)
                        {
                            dataGridView.Rows[b].Cells[a].Value = b + 1;
                            dataGridView.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                            dataGridView.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        if (b == 0)
                        {
                            dataGridView.Rows[b].Cells[a].Value = a + 1;
                            dataGridView.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                            dataGridView.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                }
                //设置控件高度
                for (int a = 0; a < Convert.ToInt16(TrayRowNum); a++)
                {
                    dataGridView.Rows[a].Height = 23;//高度
                }
                //设置控件宽度
                for (int a = 0; a < Convert.ToInt16(TrayListNum); a++)
                {
                    dataGridView.Columns[a].Width = 48;
                }
                //取消取消标记
                dataGridView.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tray初始化，异常信息如下：" + ex.Message, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }

        #endregion

        #region 初始化表格带序号

        public void InitializeData(DataGridView dataGridView1, double RowNum, double ListNum)
        {
            try
            {    //先控件X方向数量；Y方向数量
                dataGridView1.RowCount = Convert.ToInt16(RowNum);
                dataGridView1.ColumnCount = Convert.ToInt16(ListNum);
                for (int a = 0; a < Convert.ToInt16(ListNum); a++)//X方向
                {
                    for (int b = 0; b < Convert.ToInt16(RowNum); b++)//Y方向
                    {
                        dataGridView1.Rows[b].Cells[a].Style.BackColor = Color.White;

                        dataGridView1.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                        dataGridView1.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }
                //设置控件高度
                for (int a = 0; a < Convert.ToInt16(RowNum); a++)
                {
                    dataGridView1.Rows[a].Height = 30;//高度
                }
                //设置控件宽度
                for (int a = 0; a < Convert.ToInt16(ListNum); a++)
                {
                    if (a == 0)
                    {
                        dataGridView1.Columns[a].Width = 100;
                    }
                    else
                    {
                        dataGridView1.Columns[a].Width = 200;
                    }
                }

                for (int i = 0; i < RowNum; i++)
                {
                    dataGridView1.Rows[i].Cells[0].Value = "Temp" + (i + 1).ToString();
                }

                //取消取消标记
                dataGridView1.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tray初始化，异常信息如下：" + ex.Message, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }

        #endregion

        #region info信息
        public void Initialize(DataGridView dataGridView1, double RowNum, double ListNum)
        {
            try
            {    //先控件X方向数量；Y方向数量
                dataGridView1.RowCount = Convert.ToInt16(RowNum);
                dataGridView1.ColumnCount = Convert.ToInt16(ListNum);
                for (int a = 0; a < Convert.ToInt16(ListNum); a++)//X方向
                {
                    for (int b = 0; b < Convert.ToInt16(RowNum); b++)//Y方向
                    {
                        dataGridView1.Rows[b].Cells[a].Style.BackColor = Color.White;
                        dataGridView1.Rows[b].Cells[a].Value = "0";
                        dataGridView1.Columns[a].DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));/*New Font("宋体", 8.75)*/
                        dataGridView1.Columns[a].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }
                //设置控件高度
                for (int a = 0; a < Convert.ToInt16(RowNum); a++)
                {
                    dataGridView1.Rows[a].Height = 36;//高度
                }
                //设置控件宽度
                for (int a = 0; a < Convert.ToInt16(ListNum); a++)
                {
                    if (a == 0 || a == 2)
                    {
                        dataGridView1.Columns[a].Width = 85;
                    }
                    else
                    {
                        dataGridView1.Columns[a].Width = 130;
                    }
                }

                dataGridView1.Rows[0].Cells[0].Value = "操作员";
                dataGridView1.Rows[1].Cells[0].Value = "批号";
                dataGridView1.Rows[2].Cells[0].Value = "工单号";
                dataGridView1.Rows[3].Cells[0].Value = "PO号";
                dataGridView1.Rows[4].Cells[0].Value = "批开始时间";
                dataGridView1.Rows[5].Cells[0].Value = "运行时间";
                dataGridView1.Rows[6].Cells[0].Value = "停止时间";
                dataGridView1.Rows[7].Cells[0].Value = "加热时间";

                dataGridView1.Rows[0].Cells[2].Value = "投入总数";
                dataGridView1.Rows[1].Cells[2].Value = "产出总数";
                dataGridView1.Rows[2].Cells[2].Value = "良品数";
                dataGridView1.Rows[3].Cells[2].Value = "总良率";
                dataGridView1.Rows[4].Cells[2].Value = "UPH";
                dataGridView1.Rows[5].Cells[2].Value = "报警率(M/N)";
                dataGridView1.Rows[6].Cells[2].Value = "入盘数";
                dataGridView1.Rows[7].Cells[2].Value = "出盘数";

                //取消取消标记
                dataGridView1.CurrentCell = null;

                dataGridView1.Rows[0].Cells[1].ReadOnly = false;
                dataGridView1.Rows[1].Cells[1].ReadOnly = false;
                dataGridView1.Rows[2].Cells[1].ReadOnly = false;
                dataGridView1.Rows[3].Cells[1].ReadOnly = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Tray初始化，异常信息如下：" + ex.Message, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }
        #endregion

        #region DataGridView点击行或者列颜色发生变化

        public void SelectedCellcount(DataGridView dataGridView, string list, string row)
        {
            int MyCount = dataGridView.GetCellCount(DataGridViewElementStates.Selected);//被选中单元格数
            if (MyCount > 0)
            {
                if (dataGridView.AreAllCellsSelected(true))//判断是否选中多个
                {
                    for (int a = 0; a < Convert.ToInt16(row); a++)
                    {
                        for (int b = 0; b < Convert.ToInt16(list); b++)
                        {
                            if (dataGridView.Rows[a].Cells[b].Style.BackColor != Color.Green)
                            {
                                dataGridView.Rows[a].Cells[b].Style.BackColor = Color.Green;
                            }
                            else
                            {
                                dataGridView.Rows[a].Cells[b].Style.BackColor = Color.White;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < MyCount; i++)
                    {
                        if (dataGridView.SelectedCells[i].Style.BackColor != Color.Green)
                        {
                            dataGridView.SelectedCells[i].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            dataGridView.SelectedCells[i].Style.BackColor = Color.White;
                        }
                    }
                }
                dataGridView.CurrentCell = null;
            }
        }

        #endregion



    }
}
