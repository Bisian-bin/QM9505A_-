using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public partial class IOForm : Form
    {
        INIHelper inIHelper = new INIHelper();
        DataGrid dataGrid = new DataGrid();
        Function function = new Function();      
        TXT myTXT = new TXT();
        public Thread IORefresh;
        PictureBox[] picbox_X = new PictureBox[192];
        PictureBox[] picbox_Y = new PictureBox[192];
        PictureBox[] picbox_Axis = new PictureBox[64];
        PictureBox[] picbox_Axisx = new PictureBox[64];
        public string picStatus_X;
        public string picStatus_Y;
        public string picStatus_Axis;
        public string picStatus_Axisx;
        public int formNum = 0;
        public int ioStep = 0;

        public IOForm()
        {
            InitializeComponent();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            ioStep = 0;
            formNum += 1;
            if (formNum > 1)
            {
                if (IORefresh != null)
                {
                    IORefresh.Abort();
                    IORefresh = null;
                }
            }

            base.OnVisibleChanged(e);
            if (!IsHandleCreated)
            {
                this.Close();
            }
        }

        private void IOForm_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;//线程间操作

            //初始化DataGridView
            dataGrid.NotChangeListRow(dataGridIN);
            DataGridViewInit(dataGridIN, 0);
            AddControl_X(dataGridIN, 0);
            ReadIN(dataGridIN, "_In");

            DataGridViewInit(dataGridOut, 0);

            #region 初始化控件

            //for (int i = 0; i < 64; i++)
            //{
            //    picStatus_X = "Xlight" + (i).ToString();
            //    picbox_X[i] = (PictureBox)(this.Controls.Find(picStatus_X, true)[0]);
            //}

            //for (int i = 0; i < 64; i++)
            //{
            //    picStatus_Y = "Ylight" + (i).ToString();
            //    picbox_Y[i] = (PictureBox)(this.Controls.Find(picStatus_Y, true)[0]);
            //}

            #endregion

            #region 开启线程

            IORefresh = new Thread(IORsh);//开始后，开新线程执行此方法
            IORefresh.IsBackground = true;
            IORefresh.Start();


            #endregion
        }

        private void IOForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IORefresh != null)
            {
                IORefresh.Abort();
                IORefresh = null;
            }
        }

        #region tabpage切换
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    {
                        dataGrid.NotChangeListRow(dataGridIN);
                        AddControl_X(dataGridIN, 0);
                        ReadIN(dataGridIN, "_In");
                        break;
                    }
                case 1:
                    {
                        dataGrid.NotChangeListRow(dataGridOut);
                        AddControl_Y(dataGridOut, 0);
                        ReadOUT(dataGridOut, "_out");
                        break;
                    }
            }
        }
        #endregion

        #region 初始化DataGridView
        public void DataGridViewInit(DataGridView dataGridView, int num)
        {
            //添加列
            for (int i = 0; i < 12; i++)
            {
                if (i == 0 || i == 3 || i == 6 || i == 9)
                {
                    dataGridView.Columns.Add((i + num).ToString(), "状态");
                }
                else if (i == 1 || i == 4 || i == 7 || i == 10)
                {
                    dataGridView.Columns.Add((i + num).ToString(), "索引");
                }
                else if (i == 2 || i == 5 || i == 8 || i == 11)
                {
                    dataGridView.Columns.Add((i + num).ToString(), "内容");
                }
            }

            //添加行
            for (int i = 0; i < 16; i++)
            {
                dataGridView.Rows.Add();
            }
            //修改高度
            for (int i = 0; i < 16; i++)
            {
                dataGridView.Rows[i].Height = 50;
            }
            //修改宽度
            for (int i = 0; i < 12; i++)
            {
                if (i == 0 || i == 3 || i == 6 || i == 9)
                {
                    dataGridView.Columns[i].Width = 70;
                }
                else if (i == 1 || i == 4 || i == 7 || i == 10)
                {
                    dataGridView.Columns[i].Width = 70;
                }
                else if (i == 2 || i == 5 || i == 8 || i == 11)
                {
                    dataGridView.Columns[i].Width = 270;
                }
            }

            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;//列标题居中
            dataGridView.AllowUserToAddRows = false;//取消第一行
            dataGridView.RowHeadersVisible = false;//取消第一列
            dataGridView.ClearSelection(); //取消默认选中  
        }

        #endregion

        #region 添加X控件
        public void AddControl_X(DataGridView dataGridView, int num)
        {
            //状态
            PictureBox[] pic = new PictureBox[64];
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    pic[i] = new PictureBox();
                    pic[i].Name = "Xlight" + (j * 16 + i + num).ToString().PadLeft(3, '0');
                    dataGridView.Controls.Add(pic[i]);
                    Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3, i, false);
                    pic[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                    pic[i].Location = new Point(rect.Left + 3, rect.Top + 3);

                }
            }

            //索引
            Label[] lab = new Label[64];
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    lab[i] = new Label();
                    lab[i].Text = "X" + (j * 16 + i + num).ToString().PadLeft(3, '0');
                    lab[i].AutoSize = false;
                    lab[i].TextAlign = ContentAlignment.MiddleCenter;
                    lab[i].BackColor = Color.LightGoldenrodYellow;
                    dataGridView.Controls.Add(lab[i]);
                    Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 1, i, false);
                    lab[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                    lab[i].Location = new Point(rect.Left + 3, rect.Top + 3);

                }
            }

            //内容
            Label[] labcon = new Label[64];
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    labcon[i] = new Label();
                    labcon[i].Name = "X" + (j * 16 + i + num).ToString().PadLeft(3, '0') + "_In";
                    labcon[i].Text = "123";
                    labcon[i].AutoSize = false;
                    labcon[i].TextAlign = ContentAlignment.MiddleCenter;
                    labcon[i].BackColor = Color.LightGoldenrodYellow;
                    dataGridView.Controls.Add(labcon[i]);
                    Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 2, i, false);
                    labcon[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                    labcon[i].Location = new Point(rect.Left + 3, rect.Top + 3);

                }
            }
        }

        #endregion

        #region 添加Y控件
        public void AddControl_Y(DataGridView dataGridView, int num)
        {
            //状态
            PictureBox[] pic = new PictureBox[64];
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    pic[i] = new PictureBox();
                    pic[i].Name = "Ylight" + (j * 16 + i + num).ToString().PadLeft(3, '0');
                    dataGridView.Controls.Add(pic[i]);
                    Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3, i, false);
                    pic[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                    pic[i].Location = new Point(rect.Left + 3, rect.Top + 3);

                }
            }

            //索引
            Label[] lab = new Label[64];
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    lab[i] = new Label();
                    lab[i].Text = "Y" + (j * 16 + i + num).ToString().PadLeft(3, '0');
                    lab[i].AutoSize = false;
                    lab[i].TextAlign = ContentAlignment.MiddleCenter;
                    lab[i].BackColor = Color.LightGoldenrodYellow;
                    dataGridView.Controls.Add(lab[i]);
                    Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 1, i, false);
                    lab[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                    lab[i].Location = new Point(rect.Left + 3, rect.Top + 3);

                }
            }

            //输出按钮
            Button[] btn = new Button[64];
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    btn[i] = new Button();
                    btn[i].Name = "Y" + (j * 16 + i + num).ToString().PadLeft(3, '0') + "_out";
                    btn[i].Text = "123";
                    btn[i].AutoSize = false;
                    btn[i].TextAlign = ContentAlignment.MiddleCenter;
                    btn[i].BackColor = Color.LightGoldenrodYellow;
                    dataGridView.Controls.Add(btn[i]);
                    Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 2, i, false);
                    btn[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                    btn[i].Location = new Point(rect.Left + 3, rect.Top + 3);
                    btn[i].Click += new EventHandler(OutBtn_Click);

                }
            }
        }

        #endregion

        #region 添加轴控件
        public void AddControl_Axis(DataGridView dataGridView, string str, int statu, int num)
        {
            //状态
            PictureBox[] pic = new PictureBox[64];
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    pic[i] = new PictureBox();
                    pic[i].Name = str + (j * 16 + i + statu).ToString().PadLeft(3, '0');
                    dataGridView.Controls.Add(pic[i]);
                    Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3, i, false);
                    pic[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                    pic[i].Location = new Point(rect.Left + 3, rect.Top + 3);
                }
            }

            //索引
            Label[] lab = new Label[64];
            for (int j = 0; j < 4; j++)
            {
                switch (j)
                {
                    case 0:
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                lab[i] = new Label();
                                lab[i].Text = (i + 1 + num).ToString().PadLeft(2, '0') + "Home";
                                lab[i].AutoSize = false;
                                lab[i].TextAlign = ContentAlignment.MiddleCenter;
                                lab[i].BackColor = Color.LightGoldenrodYellow;
                                dataGridView.Controls.Add(lab[i]);
                                Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 1, i, false);
                                lab[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                                lab[i].Location = new Point(rect.Left + 3, rect.Top + 3);
                            }
                            break;
                        }
                    case 1:
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                lab[i] = new Label();
                                lab[i].Text = (i + 1 + num).ToString().PadLeft(2, '0') + "Plimt";
                                lab[i].AutoSize = false;
                                lab[i].TextAlign = ContentAlignment.MiddleCenter;
                                lab[i].BackColor = Color.LightGoldenrodYellow;
                                dataGridView.Controls.Add(lab[i]);
                                Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 1, i, false);
                                lab[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                                lab[i].Location = new Point(rect.Left + 3, rect.Top + 3);
                            }
                            break;
                        }
                    case 2:
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                lab[i] = new Label();
                                lab[i].Text = (i + 1 + num).ToString().PadLeft(2, '0') + "Nlimt";
                                lab[i].AutoSize = false;
                                lab[i].TextAlign = ContentAlignment.MiddleCenter;
                                lab[i].BackColor = Color.LightGoldenrodYellow;
                                dataGridView.Controls.Add(lab[i]);
                                Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 1, i, false);
                                lab[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                                lab[i].Location = new Point(rect.Left + 3, rect.Top + 3);
                            }
                            break;
                        }
                    case 3:
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                lab[i] = new Label();
                                lab[i].Text = (i + 1 + num).ToString().PadLeft(2, '0') + "Alarm";
                                lab[i].AutoSize = false;
                                lab[i].TextAlign = ContentAlignment.MiddleCenter;
                                lab[i].BackColor = Color.LightGoldenrodYellow;
                                dataGridView.Controls.Add(lab[i]);
                                Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 1, i, false);
                                lab[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                                lab[i].Location = new Point(rect.Left + 3, rect.Top + 3);
                            }
                            break;
                        }
                }
            }

            //内容
            Label[] labcon = new Label[64];
            for (int j = 0; j < 4; j++)
            {
                switch (j)
                {
                    case 0:
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                labcon[i] = new Label();
                                labcon[i].Name = (i + 1 + num).ToString().PadLeft(2, '0') + "Home";
                                labcon[i].Text = "123";
                                labcon[i].AutoSize = false;
                                labcon[i].TextAlign = ContentAlignment.MiddleCenter;
                                labcon[i].BackColor = Color.LightGoldenrodYellow;
                                dataGridView.Controls.Add(labcon[i]);
                                Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 2, i, false);
                                labcon[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                                labcon[i].Location = new Point(rect.Left + 3, rect.Top + 3);

                            }
                            break;
                        }
                    case 1:
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                labcon[i] = new Label();
                                labcon[i].Name = (i + 1 + num).ToString().PadLeft(2, '0') + "Plimit";
                                labcon[i].Text = "123";
                                labcon[i].AutoSize = false;
                                labcon[i].TextAlign = ContentAlignment.MiddleCenter;
                                labcon[i].BackColor = Color.LightGoldenrodYellow;
                                dataGridView.Controls.Add(labcon[i]);
                                Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 2, i, false);
                                labcon[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                                labcon[i].Location = new Point(rect.Left + 3, rect.Top + 3);

                            }
                            break;
                        }
                    case 2:
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                labcon[i] = new Label();
                                labcon[i].Name = (i + 1 + num).ToString().PadLeft(2, '0') + "Nlimit";
                                labcon[i].Text = "123";
                                labcon[i].AutoSize = false;
                                labcon[i].TextAlign = ContentAlignment.MiddleCenter;
                                labcon[i].BackColor = Color.LightGoldenrodYellow;
                                dataGridView.Controls.Add(labcon[i]);
                                Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 2, i, false);
                                labcon[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                                labcon[i].Location = new Point(rect.Left + 3, rect.Top + 3);

                            }
                            break;
                        }
                    case 3:
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                labcon[i] = new Label();
                                labcon[i].Name = (i + 1 + num).ToString().PadLeft(2, '0') + "Alarm";
                                labcon[i].Text = "123";
                                labcon[i].AutoSize = false;
                                labcon[i].TextAlign = ContentAlignment.MiddleCenter;
                                labcon[i].BackColor = Color.LightGoldenrodYellow;
                                dataGridView.Controls.Add(labcon[i]);
                                Rectangle rect = dataGridView.GetCellDisplayRectangle(j * 3 + 2, i, false);
                                labcon[i].Size = new Size(rect.Width - 6, rect.Height - 6);
                                labcon[i].Location = new Point(rect.Left + 3, rect.Top + 3);

                            }
                            break;
                        }
                }
            }
        }

        #endregion

        #region 输出按钮点击
        public void OutBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                int a = Convert.ToInt32(btn.Name.Substring(1, 3));//获取按钮值
                if (btn.BackColor == Color.LightGoldenrodYellow)//根据颜色控制输出
                {
                    if (a < 48)
                    { 
                        mc.GTN_SetExtDoBit(1, (short)(a+1), 1);//模块
                      btn.BackColor = Color.LightGreen;
                    
                    }
                    //if (a < 10)
                    //{
                    //    mc.GTN_SetDoBit(1, mc.MC_GPO, (short)(a + 1), 0);//主卡输出
                    //    btn.BackColor = Color.LightGreen;
                    //}
                    //else if (a >= 10 && a < 122)
                    //{
                    //    mc.GTN_SetExtDoBit(1, (short)(a - 10 + 1), 1);//模块
                    //    btn.BackColor = Color.LightGreen;
                    //}
                    //if (a >= 122 && a < 132)
                    //{
                    //    mc.GTN_SetDoBit(1, mc.MC_GPO, (short)(a - 101), 0);//主卡输出
                    //    btn.BackColor = Color.LightGreen;
                    //}
                    //if (a >= 132)
                    //{
                    //    mc.GTN_SetDoBit(1, mc.MC_GPO, (short)(a - 121), 0);//主卡输出
                    //    btn.BackColor = Color.LightGreen;
                    //}
                }
                else if (btn.BackColor == Color.LightGreen)//根据颜色控制输出
                {
                    if (a < 48)
                    {
                        mc.GTN_SetExtDoBit(1, (short)(a+1), 0);//模块
                         btn.BackColor = Color.LightGoldenrodYellow;
                    }


                    //if (a < 10)
                    //{
                    //    mc.GTN_SetDoBit(1, mc.MC_GPO, (short)(a + 1), 1);//主卡1#输出
                    //    btn.BackColor = Color.LightGoldenrodYellow;
                    //}
                    //else if (a >= 10 && a < 122)
                    //{
                    //    mc.GTN_SetExtDoBit(1, (short)(a - 10 + 1), 0);//模块
                    //    btn.BackColor = Color.LightGoldenrodYellow;
                    //}
                    //if (a >= 122 && a < 132)
                    //{
                    //    mc.GTN_SetDoBit(1, mc.MC_GPO, (short)(a - 101), 1);//主卡输出
                    //    btn.BackColor = Color.LightGoldenrodYellow;
                    //}
                    //if (a >= 132)
                    //{
                    //    mc.GTN_SetDoBit(1, mc.MC_GPO, (short)(a - 121), 1);//主卡输出
                    //    btn.BackColor = Color.LightGoldenrodYellow;
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("输出点击按钮事件异常" + ex.Message, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }
        #endregion

        #region IO文字显示

        public void ReadIN(DataGridView dataGridView, string name)
        {

            foreach (Control c in dataGridView.Controls)
            {
                if (c.Name.Contains(name))
                {
                    string index = c.Name.Substring(0, 4);

                    c.Text = inIHelper.IOReadContentValue("IO", index);
                }
            }
        }

        public void ReadOUT(DataGridView dataGridView, string name)
        {

            foreach (Control c in dataGridView.Controls)
            {
                if (c.Name.Contains(name))
                {
                    string index = c.Name.Substring(0, 4);

                    c.Text = inIHelper.IOReadContentValue("IO", index);
                }
            }
        }

        public void ReadAxis(DataGridView dataGridView, string name)
        {

            foreach (Control c in dataGridView.Controls)
            {
                if (c.Name.Contains(name))
                {
                    string index = c.Name;

                    c.Text = inIHelper.IOReadContentValue("IO", index);
                }
            }
        }


        #endregion

        #region 更新IO指示灯
        public void UpdateColor(DataGridView dataGridView, string name, int num, int state)
        {
            foreach (var c in dataGridView.Controls)
            {
                if (c is Label)
                {
                    Label lab = (Label)c;
                    if (lab.Name == name + num.ToString())
                    {
                        if (state == 1)
                        {
                            lab.BackColor = Color.Green;
                        }
                        else
                        {
                            lab.BackColor = Color.LightGray;
                        }
                    }
                }
            }
        }
        #endregion

        #region 刷新
        public void IORsh()
        {
            while (true)
            {
                if (this.IsDisposed)
                {
                    return;
                }

                #region IO界面刷新
                //try
                //{
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        {
                            //输入刷新
                            for (int i = 0; i < 48; i++)
                            {
                                picStatus_X = "Xlight" + (i).ToString().PadLeft(3, '0');
                                picbox_X[i] = (PictureBox)(this.Controls.Find(picStatus_X, true)[0]);
                            }
                            for (int i = 0; i < 48; i++)
                            {
                                if (Variable.XStatus[i])
                                {
                                    picbox_X[i].BackColor = Color.Green;                                    
                                }
                                else
                                {
                                    picbox_X[i].BackColor = Color.LightGray;                                  
                                }
                            }
                            break;
                        }
                    case 1:
                        {
                            //输出刷新
                            for (int i = 0; i < 48; i++)
                            {
                                picStatus_Y = "Ylight" + (i).ToString().PadLeft(3, '0');
                                picbox_Y[i] = (PictureBox)(this.Controls.Find(picStatus_Y, true)[0]);
                            }

                            for (int i = 0; i < 48; i++)
                            {
                                if (Variable.YStatus[i])
                                {
                                    picbox_Y[i].BackColor = Color.Green;
                                }
                                else
                                {
                                    picbox_Y[i].BackColor = Color.LightGray;
                                }
                            }
                            break;
                        }
                }
                //}
                //catch
                //{

                //}
                #endregion                               

                #region 步序监控
                try
                {                    
                    labAutoStep0.Text = Variable.UpAutoStep.ToString();
                    labAutoStep1.Text = Variable.UpAutoStep1.ToString();
                    labAutoStep2.Text = Variable.UpAutoStep2.ToString();
                    labAutoStep3.Text = Variable.DownAutoStep1.ToString();
                    labAutoStep4.Text = Variable.DownAutoStep2.ToString();
                    labAutoStep5.Text = Variable.DownAutoStep.ToString();
                }
                catch
                {
                }
                #endregion

                Thread.Sleep(10);
            }

        }
        #endregion

        #region 将整数转换二进制

        public string IntToBin32(int data, int a)
        {
            string cnt;//记录转换过后二进制值
            byte[] IO_input = new byte[32];

            for (int i = 0; i < 32; i++)
            {
                IO_input[i] = (byte)(((data & 1) + 1) % 2);
                data = data >> 1;//n变成n向右移一位的那个数
            }
            cnt = IO_input[a].ToString();
            return cnt;
        }

        public string IntToBin16(int data, int a)
        {
            string cnt;//记录转换过后二进制值
            byte[] IO_input = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                IO_input[i] = (byte)(((data & 1) + 1) % 2);
                data = data >> 1;//n变成n向右移一位的那个数
            }
            cnt = IO_input[a].ToString();
            return cnt;
        }


        #endregion




    }
}
