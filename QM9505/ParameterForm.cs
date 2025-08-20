using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public partial class ParameterForm : Form
    {
        private object threadLock;
        INIHelper inidata = new INIHelper();
        Thread refreshThread;
        public string group;
        public GroupBox[] groupBox = new GroupBox[7];
        public string txtIP;
        TextBox[] txtboxIP = new TextBox[40];
        public string txtPoint;
        TextBox[] txtboxPoint = new TextBox[40];
        public int formNum = 0;
        UDPServer udpServer = new UDPServer();
        TXT myTXT = new TXT();
        Temperature temperature = new Temperature();
        Temperature tem = new Temperature();
        mc mc = new mc(); // 添加mc对象引用
        // 温度历史数据
        private List<double> tempHistory1 = new List<double>(); // 通道1
        private List<double> tempHistory2 = new List<double>(); // 通道2
        private List<DateTime> tempTimeHistory = new List<DateTime>(); // 采集时间
        private int tempSampleCounter = 0;


        public ParameterForm()
        {
            InitializeComponent();
            threadLock = new object();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            Variable.formOpenFlag = false;
            formNum += 1;
            if (formNum > 1)
            {
                if (refreshThread != null)
                {
                    refreshThread.Abort();
                    refreshThread = null;
                }
            }

            base.OnVisibleChanged(e);
            if (!IsHandleCreated)
            {
                this.Close();
            }
        }

        #region 打开窗体
        private void ParameterForm_Load(object sender, EventArgs e)
        {
            //IP
            for (int i = 0; i < 40; i++)
            {
                txtIP = "IP" + (i + 1).ToString().PadLeft(2, '0');
                txtboxIP[i] = (TextBox)(this.Controls.Find(txtIP, true)[0]);
            }

            //Point
            for (int i = 0; i < 40; i++)
            {
                txtPoint = "Point" + (i + 1).ToString().PadLeft(2, '0');
                txtboxPoint[i] = (TextBox)(this.Controls.Find(txtPoint, true)[0]);
            }

            //权限
            for (int i = 0; i < 7; i++)
            {
                group = "groupBox" + (i + 100).ToString();//.PadLeft(2, '0');
                groupBox[i] = (GroupBox)(this.Controls.Find(group, true)[0]);

            }

            Variable.formOpenFlag = true;

            //加载参数
            LoadParameter(Application.StartupPath + "\\Parameter.ini");

            refreshThread = new Thread(Rsh);//开始后，开新线程执行此方法
            refreshThread.IsBackground = true;
            refreshThread.Start();

            //与板卡通信
            if (Variable.Serverflag)
            {
                ServerPort.BackColor = Color.Green;
            }
            else
            {
                ServerPort.BackColor = Color.Red;
            }

            if (Variable.VCQCheck)
            {
                VCQCheck.Checked = true;
            }
            else
            {
                VCQCheck.Checked = false;
            }


            if (Variable.testTimeReadCheck)
            {
                testTimeReadCheck.Checked = true;
            }
            else
            {
                testTimeReadCheck.Checked = false;
            }

        }

        #endregion

        #region 加载参数

        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
            OpenFileDialog1.InitialDirectory = "D:/";
            OpenFileDialog1.Filter = "加载参数(*.bin)|*.bin";
            OpenFileDialog1.FilterIndex = 1;
            OpenFileDialog1.RestoreDirectory = true;
            OpenFileDialog1.FileName = "";
            OpenFileDialog1.ShowDialog();

            try
            {
                if (OpenFileDialog1.FileName == "")
                {
                    MessageBox.Show("加载文件名不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    string path = OpenFileDialog1.FileName;//保存文件地址名称
                    SetPath.Text = path;
                    string[] split = path.Split(new Char[] { '\\' });
                    Variable.RecordName = Variable.FileName = split[split.Length - 1];//参数名称

                    LoadParameter(path);

                    MessageBox.Show("参数加载成功！");
                }
            }
            catch (Exception EX)
            {
                MessageBox.Show("载入参数失败，异常信息如下：" + EX.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }



        #endregion

        #region 加载参数方法

        public void LoadParameter(string path)
        {
            //客服端IP,端口号
            for (int i = 0; i < 40; i++)
            {
                if (Variable.formOpenFlag)
                {
                    txtboxIP[i].Text = inidata.getIni("PGM", "IP" + (i + 1).ToString(), "", path);
                    txtboxPoint[i].Text = inidata.getIni("PGM", "Point" + (i + 1).ToString(), "", path);
                }

                Variable.txtboxIP[i] = inidata.getIni("PGM", "IP" + (i + 1).ToString(), "", path);
                Variable.txtboxPoint[i] = inidata.getIni("PGM", "Point" + (i + 1).ToString(), "", path);
            }

            //服务器端口
            ServerIP.Text = inidata.getIni("PGM", "ServerIP", "", path);
            ServerPoint.Text = inidata.getIni("PGM", "ServerPoint", "", path);
            Variable.ServerIP = ServerIP.Text;
            Variable.ServerPoint = ServerPoint.Text;

            time_alarm.Text = inidata.getIni("PGM", "time_alarm", "", path);//报警延迟
            if (time_alarm.Text == "")
            { time_alarm.Text = "5000"; }
            Variable.time_Alarm = time_alarm.Text;

            time_Cylinder.Text = inidata.getIni("PGM", "time_Cylinder", "", path);//报警延迟
            if (time_Cylinder.Text == "")
            { time_Cylinder.Text = "300"; }
            Variable.time_cylinder = time_Cylinder.Text;

            //其他参数

            string hotModcheckBox = inidata.getIni("GN", "HotModcheckBox", "", path);
            if (hotModcheckBox == "1")
            {
                HotModcheckBox.Checked = true;
                Variable.HotModel = true;
            }
            else
            {
                HotModcheckBox.Checked = false;
                Variable.HotModel = false;
            }

            string testWaitTime1 = inidata.getIni("GN", "testWaitTimeCheck", "", path);
            if (testWaitTime1 == "1")
            {
                testWaitTimeCheck.Checked = true;
                Variable.testWaitTimeCheck = true;
            }
            else
            {
                testWaitTimeCheck.Checked = false;
                Variable.testWaitTimeCheck = false;
            }

            string testTime1 = inidata.getIni("GN", "testTimeCheck", "", path);
            if (testTime1 == "1")
            {
                testTimeCheck.Checked = true;
                Variable.testTimeCheck = true;
            }
            else
            {
                testTimeCheck.Checked = false;
                Variable.testTimeCheck = false;
            }

            string testTimeOut1 = inidata.getIni("GN", "testTimeOutCheck", "", path);
            if (testTimeOut1 == "1")
            {
                testTimeOutCheck.Checked = true;
                Variable.testTimeOutCheck = true;
            }
            else
            {
                testTimeOutCheck.Checked = false;
                Variable.testTimeOutCheck = false;
            }

            string vcqCheck = inidata.getIni("GN", "VCQCheck", "", path);
            if (vcqCheck == "1")
            {
                VCQCheck.Checked = true;
                Variable.VCQCheck = true;
            }
            else
            {
                VCQCheck.Checked = false;
                Variable.VCQCheck = false;
            }


            string Up1checks = inidata.getIni("GN", "Up1check", "", path);
            if (Up1checks == "1")
            {
                Up1check.Checked = true;
                Variable.Up1check = true;
            }
            else
            {
                Up1check.Checked = false;
                Variable.Up1check = false;
            }
            string Up2checks = inidata.getIni("GN", "Up2check", "", path);
            if (Up2checks == "1")
            {
                Up2check.Checked = true;
                Variable.Up2check = true;
            }
            else
            {
                Up2check.Checked = false;
                Variable.Up2check = false;
            }


            string Down1checks = inidata.getIni("GN", "Down1check", "", path);
            if (Down1checks == "1")
            {
                Down1check.Checked = true;
                Variable.Down1check = true;
            }
            else
            {
                Down1check.Checked = false;
                Variable.Down1check = false;
            }
            string Down2checks = inidata.getIni("GN", "Down2check", "", path);
            if (Down2checks == "1")
            {
                Down2check.Checked = true;
                Variable.Down2check = true;
            }
            else
            {
                Down2check.Checked = false;
                Variable.Down2check = false;
            }
            string che_Door1 = inidata.getIni("GN", "che_Door1", "", path);
            if (che_Door1 == "1")
            {
                che_Door.Checked = true;
                Variable.che_door = true;
            }
            else
            {
                che_Door.Checked = false;
                Variable.che_door = false;
            }



            testWaitTime.Text = inidata.getIni("PGM", "testWaitTime", "", path);
            testTime.Text = inidata.getIni("PGM", "testTime", "", path);
            testTimeOut.Text = inidata.getIni("PGM", "testTimeOut", "", path);
            VCCVolSet.Text = inidata.getIni("PGM", "VCCVolSet", "", path);
            VCQVolSet.Text = inidata.getIni("PGM", "VCQVolSet", "", path);
            Powertime.Text = inidata.getIni("PGM", "Powertime", "", path);
            Variable.testWaitTime = testWaitTime.Text;
            Variable.testTime = testTime.Text;
            Variable.testTimeOut = testTimeOut.Text;
            Variable.VCCVolSet = VCCVolSet.Text;
            Variable.VCQVolSet = VCQVolSet.Text;
            if (Powertime.Text == "")
            { Powertime.Text = "5"; }
            Variable.Powertime = Powertime.Text;
            //串口数据
            Variable.portName = inidata.getIni("PGM", "portName", "", path);
            Variable.baudRate = Convert.ToInt32(inidata.getIni("PGM", "baudRate", "", path));
            Variable.dataBit = Convert.ToInt32(inidata.getIni("PGM", "dataBit", "", path));
            Variable.parity = inidata.getIni("PGM", "parity", "", path);
            Variable.stop = Convert.ToInt32(inidata.getIni("PGM", "stop", "", path));

            TempUpLimit.Text = inidata.getIni("PGM", "TempUpLimit", "", path);
            TempDownLimit.Text = inidata.getIni("PGM", "TempDownLimit", "", path);
            temper.Text = inidata.getIni("PGM", "temper", "", path);
            offsets.Text = inidata.getIni("PGM", "offsets", "", path);
            blowTime.Text = inidata.getIni("PGM", "blowTime", "", path);
            Uptemper.Text = inidata.getIni("PGM", "Uptemper", "", path);

            // 安全的字符串到double转换
            Variable.TempUpLimit = SafeConvertToDouble(TempUpLimit.Text, 0.0);
            Variable.TempDownLimit = SafeConvertToDouble(TempDownLimit.Text, 0.0);
            Variable.temper = SafeConvertToDouble(temper.Text, 0.0);
            Variable.offsets = SafeConvertToDouble(offsets.Text, 0.0);
            Variable.blowTime = SafeConvertToDouble(blowTime.Text, 0.0);
            Variable.upTemper = SafeConvertToDouble(Uptemper.Text, 0.0);

            texttempSetDelay.Text = inidata.getIni("PGM", "texttempSetDelay", "", path);
            Variable.tempSetDelay = SafeConvertToDouble(texttempSetDelay.Text, 0.0);

        }

        #endregion

        #region 安全的字符串转换方法
        /// <summary>
        /// 安全地将字符串转换为double类型
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>转换后的double值</returns>
        private double SafeConvertToDouble(string value, double defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            double result;
            if (double.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }
        #endregion

        #region 保存参数

        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = "D:/";
            SaveFileDialog1.Filter = "保存参数(*.bin)|*.bin";
            SaveFileDialog1.FilterIndex = 1;
            SaveFileDialog1.RestoreDirectory = true;
            SaveFileDialog1.FileName = "";
            SaveFileDialog1.ShowDialog();
            try
            {
                if (SaveFileDialog1.FileName == "")
                {
                    MessageBox.Show("保存文件名不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    bool b = TextEmpty();
                    if (!b)
                    {
                        string path = SaveFileDialog1.FileName;//保存文件地址名称
                        SetPath.Text = path;
                        string[] split = path.Split(new Char[] { '\\' });
                        Variable.FileName = split[split.Length - 1];//档案名称   
                        SaveParameter(path);

                        //保存参数
                        path = Application.StartupPath + "\\parameter.ini";
                        inidata.writeIni("GN", "parameter", Variable.FileName, path);
                        SaveParameter(path);
                        //加载参数
                        LoadParameter(path);

                        MessageBox.Show("参数保存成功！");
                    }
                    else
                    {
                        MessageBox.Show("文本框不能为空,请输入0");
                    }
                }
            }
            catch (Exception EX)
            {
                MessageBox.Show("保存参数失败，异常信息如下：" + EX.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        #endregion

        #region 保存参数方法
        public void SaveParameter(string path)
        {
            //客户端IP,端口号
            for (int i = 0; i < 40; i++)
            {
                inidata.writeIni("PGM", "IP" + (i + 1).ToString(), txtboxIP[i].Text, path);
                inidata.writeIni("PGM", "Point" + (i + 1).ToString(), txtboxPoint[i].Text, path);
            }


            //服务器端口
            inidata.writeIni("PGM", "ServerIP", ServerIP.Text, path);
            inidata.writeIni("PGM", "ServerPoint", ServerPoint.Text, path);


            inidata.writeIni("PGM", "time_alarm", time_alarm.Text, path);//报警延迟
            inidata.writeIni("PGM", "time_Cylinder", time_Cylinder.Text, path);//其他气缸报警延迟

            //其他参数
            if (HotModcheckBox.Checked == true)
            {
                inidata.writeIni("GN", "HotModcheckBox", "1", path);
            }
            else
            {
                inidata.writeIni("GN", "HotModcheckBox", "0", path);
            }
            if (testWaitTimeCheck.Checked == true)
            {
                string EN_testWaitTime = "1";
                inidata.writeIni("GN", "testWaitTimeCheck", EN_testWaitTime, path);
            }
            else
            {
                string EN_testWaitTime = "0";
                inidata.writeIni("GN", "testWaitTimeCheck", EN_testWaitTime, path);
            }

            if (testTimeCheck.Checked == true)
            {
                string EN_testTimeCheck = "1";
                inidata.writeIni("GN", "testTimeCheck", EN_testTimeCheck, path);
            }
            else
            {
                string EN_testTimeCheck = "0";
                inidata.writeIni("GN", "testTimeCheck", EN_testTimeCheck, path);
            }

            if (testTimeOutCheck.Checked == true)
            {
                string EN_testTimeOutCheck = "1";
                inidata.writeIni("GN", "testTimeOutCheck", EN_testTimeOutCheck, path);
            }
            else
            {
                string EN_testTimeOutCheck = "0";
                inidata.writeIni("GN", "testTimeOutCheck", EN_testTimeOutCheck, path);
            }

            if (VCQCheck.Checked == true)
            {
                string vcqCheck = "1";
                inidata.writeIni("GN", "VCQCheck", vcqCheck, path);
            }
            else
            {
                string vcqCheck = "0";
                inidata.writeIni("GN", "VCQCheck", vcqCheck, path);
            }


            if (Up1check.Checked == true)
            {
                string Up1checkds = "1";
                inidata.writeIni("GN", "Up1check", Up1checkds, path);
            }
            else
            {
                string Up1checkds = "0";
                inidata.writeIni("GN", "Up1check", Up1checkds, path);
            }

            if (Up2check.Checked == true)
            {
                string Up2checkds = "1";
                inidata.writeIni("GN", "Up2check", Up2checkds, path);
            }
            else
            {
                string Up2checkds = "0";
                inidata.writeIni("GN", "Up2check", Up2checkds, path);
            }

            if (Down1check.Checked == true)
            {
                string Down1checkds = "1";
                inidata.writeIni("GN", "Down1check", Down1checkds, path);
            }
            else
            {
                string Down1checkds = "0";
                inidata.writeIni("GN", "Down1check", Down1checkds, path);
            }

            if (Down2check.Checked == true)
            {
                string Down2checkds = "1";
                inidata.writeIni("GN", "Down2check", Down2checkds, path);
            }
            else
            {
                string Down2checkds = "0";
                inidata.writeIni("GN", "Down2check", Down2checkds, path);
            }

            if (che_Door.Checked == true)
            {
                string che_Door1 = "1";
                inidata.writeIni("GN", "che_Door", che_Door1, path);
            }
            else
            {
                string che_Door1 = "0";
                inidata.writeIni("GN", "che_Door", che_Door1, path);
            }



            inidata.writeIni("PGM", "VCCVolSet", VCCVolSet.Text, path);
            inidata.writeIni("PGM", "VCQVolSet", VCQVolSet.Text, path);
            inidata.writeIni("PGM", "testWaitTime", testWaitTime.Text, path);
            inidata.writeIni("PGM", "testTime", testTime.Text, path);
            inidata.writeIni("PGM", "testTimeOut", testTimeOut.Text, path);
            inidata.writeIni("PGM", "Powertime", Powertime.Text, path);

            //串口数据
            inidata.writeIni("PGM", "portName", Variable.portName, path);
            inidata.writeIni("PGM", "baudRate", Variable.baudRate.ToString(), path);
            inidata.writeIni("PGM", "dataBit", Variable.dataBit.ToString(), path);
            inidata.writeIni("PGM", "parity", Variable.parity, path);
            inidata.writeIni("PGM", "stop", Variable.stop.ToString(), path);

            //温度设定
            inidata.writeIni("PGM", "TempUpLimit", TempUpLimit.Text, path);
            inidata.writeIni("PGM", "TempDownLimit", TempDownLimit.Text, path);
            inidata.writeIni("PGM", "temper", temper.Text, path);
            inidata.writeIni("PGM", "offsets", offsets.Text, path);
            inidata.writeIni("PGM", "blowTime", blowTime.Text, path);
            inidata.writeIni("PGM", "Uptemper", Uptemper.Text, path);

            inidata.writeIni("PGM", "texttempSetDelay", texttempSetDelay.Text, path);
        }

        #endregion

        #region 文本框不能为空
        public bool TextEmpty()
        {
            bool flag = false;
            foreach (Control c in this.Controls)
            {
                foreach (Control item1 in this.Controls)
                {
                    foreach (Control item2 in item1.Controls)
                    {
                        foreach (Control item3 in item2.Controls)
                        {
                            foreach (Control item4 in item3.Controls)
                            {
                                if (item4 is TextBox)
                                {
                                    if (item4.Text.Length == 0)
                                    {
                                        //MessageBox.Show("文本框不能为空,请输入0");
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return flag;
        }

        #endregion

        #region 文本框去除符号
        public void SetTextBoxOnlyInt()
        {
            foreach (Control item1 in this.Controls)
            {
                foreach (Control item2 in item1.Controls)
                {
                    foreach (Control item3 in item2.Controls)
                    {
                        foreach (Control item4 in item3.Controls)
                        {
                            if (item4 is TextBox)
                            {
                                ((TextBox)item4).KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBox_KeyPress);
                            }
                        }
                    }
                }
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8 && e.KeyChar != (char)45 && e.KeyChar != (char)46)
            {
                e.Handled = true;
            }
        }


        #endregion

        #region 刷新
        public void Rsh()
        {
            while (true)
            {
                if (this.IsDisposed)
                {
                    return;
                }

                //权限
                if (Variable.userEnter == Variable.UserEnter.User)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        groupBox[i].Enabled = false;
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        groupBox[i].Enabled = true;
                    }
                }

                if (testTimeReadCheck.Checked)
                {
                    Variable.testTimeReadCheck = true;
                }
                else
                {
                    Variable.testTimeReadCheck = false;
                }
                tempSampleCounter++;
                // 实时显示最新温度到timingtemper控件
                if (timingtemper.InvokeRequired)
                    timingtemper.Invoke(new Action(() => timingtemper.Text = Variable.TemperData[0].ToString("F1")));
                else
                    timingtemper.Text = Variable.TemperData[0].ToString("F1");
                if (tempSampleCounter >= 10) // 每10秒采集一次
                {
                    tempSampleCounter = 0;
                    tempHistory1.Add(Variable.TemperData[0]);
                    //tempHistory2.Add(Variable.TemperData[1]);
                    tempTimeHistory.Add(DateTime.Now);
                    
                    if (chart3.InvokeRequired)
                        chart3.Invoke(new Action(UpdateTempChart));
                    else
                        UpdateTempChart();
                }
                Thread.Sleep(1000);
            }
        }

        private void UpdateTempChart()
        {
            chart3.Series.Clear();

            // 设置Y轴（垂直轴）的坐标范围
            chart3.ChartAreas[0].AxisY.Minimum = 0;
            chart3.ChartAreas[0].AxisY.Maximum = 100;
            chart3.ChartAreas[0].AxisY.Interval = 10;
            chart3.ChartAreas[0].AxisY.MajorGrid.Interval = 10;

            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series("通道1");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Red;
            //var series2 = new System.Windows.Forms.DataVisualization.Charting.Series("通道2");
            //series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            //series2.Color = System.Drawing.Color.Blue;
            for (int i = 0; i < tempHistory1.Count; i++)
            {
                string timeLabel = tempTimeHistory[i].ToString("HH:mm:ss");
                series1.Points.AddXY(timeLabel, tempHistory1[i]);
               // series2.Points.AddXY(timeLabel, tempHistory2[i]);
            }
            chart3.Series.Add(series1);
           // chart3.Series.Add(series2);
        }

        #endregion

        #region 与板卡通信
        private void udpOpen_Click(object sender, EventArgs e)
        {
            bool Socket = udpServer.SocketStart(ServerIP.Text, ServerPoint.Text);
            if (!Socket)
            {
                ServerPort.BackColor = Color.Red;
                MessageBox.Show("测试板1链接失败");
            }
            else
            {
                ServerPort.BackColor = Color.Green;
            }
        }

        private void udpClose_Click(object sender, EventArgs e)
        {
            if (ServerPort.BackColor == Color.Green)
            {
                ServerPort.BackColor = Color.Red;
                udpServer.SocketStop();
            }
        }
        #endregion

        #region 串口打开
        public void btnOpen_Click(object sender, EventArgs e)
        {
            //Temperature.ComPort(ConfigurationManager.ConnectionStrings["TemperatureCOM1"].ConnectionString);
            Temperature.ComPort(portName.Text.Trim(), Convert.ToInt32(baudRate.Text), Convert.ToInt16(dataBit.Text.Trim()), parity.Text.Trim(), Convert.ToInt16(stop.Text.Trim()));
            bool flog = Temperature.Open();
            if (flog == false)
            {
                MessageBox.Show("QR链接失败，请确认串口线是否连接好！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                EllipsePort.BackColor = Color.Red;
            }
            else
            {
                //    连接成功
                EllipsePort.BackColor = Color.Green;
                Variable.portName = portName.Text;
            }
        }

        #endregion

        #region 串口关闭
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                Temperature.Close();
                temperature.Dispose();
                EllipsePort.BackColor = Color.Red;
            }
            catch
            {

            }
        }

        #endregion

        #region 温度设定
        private void btnTemper_Click(object sender, EventArgs e)
        {
            Variable.Tempertest = 0;
            Variable.temWriteFlag = true;
            try
            {
                Thread.Sleep(200);
                //地址:01,02,03,04,05,06,07,08,09,0A,0B,0C,0D,0E,0F,10,11,12,13.14
                //TempWrite("01", Convert.ToInt16(temper.Value * 10));
                for (int i = 0; i < 2; i++)
                {
                    TempWrite((i + 1).ToString(), Convert.ToInt16(temper.Value * 10));
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
            }
            
            Variable.temWriteFlag = false;
            Variable.Tempertest = 1;
        }
        #endregion

        #region 最高温度设定
        private void btnUpTemper_Click(object sender, EventArgs e)
        {
            Variable.Tempertest = 0;
            Variable.temWriteFlag = true;
            Thread.Sleep(200);
            //地址:01,02,03,04,05,06,07,08,09,0A,0B,0C,0D,0E,0F,10,11,12,13.14
            //UpTempWrite("01", Convert.ToInt16(Uptemper.Value * 10));
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    UpTempWrite((i + 1).ToString("X2"), Convert.ToInt16(Uptemper.Value * 10));
                    Thread.Sleep(200);
                }
            }
            catch { }
            
            Variable.temWriteFlag = false;
            Variable.Tempertest = 1;
        }
        #endregion

        #region 补偿值设定
        private void btnoffsets_Click(object sender, EventArgs e)
        {
            Variable.Tempertest = 0;
            // 获取补偿值
            int offsetValue = 0;
            if (!int.TryParse(offsets.Text, out offsetValue))
            {
                MessageBox.Show("请输入有效的补偿值！");
                return;
            }
            try
            {
                // 只为一个温控器设置补偿值（地址01）          
                    Thread.Sleep(500);
                    string channelAddress = (8).ToString("X2"); // 确保是两位十六进制
                    ConTempWrite("01", channelAddress, (short)offsetValue);
                    Variable.Tempertest = 1;
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region 温度写入方法

        #region 温度写入
        public void TempWrite(string num, int data)//TempWrite("01", 110);
        {
            //地址:01,02,03,04,05,06,07,08,09,0A,0B,0C,0D,0E,0F,10,11,12,13.14
            //string s = data.ToString("X4");
            ////string str = num + "064701" + s;//"0106470103E8"
            //string str = num + "061001" + s;//"0106100103E8"
            int channel = int.Parse(num);
            string SvAddness = channel.ToString("X4");
            string deviceAddr = "01";
            string functioncode = "06";
            string dataHex = data.ToString("X4");
            string str = deviceAddr + functioncode + SvAddness + dataHex;
            char[] startch = { ':' };
            char[] endch = { '\0', '\0', '\r', '\n' };
            char[] lrcch = tem.LRC(str).ToCharArray();//校验码生成字符数组
            lrcch.CopyTo(endch, 0);//把校验码数组嵌入到endch中
            char[] datach = str.ToCharArray();//发送内容生成字符数组
            char[] ch3 = new char[datach.Length + 5];
            datach.CopyTo(ch3, 1);//组成命令字符数组
            endch.CopyTo(ch3, datach.Length + 1);//组成命令字符数组
            startch.CopyTo(ch3, 0);//组成命令字符数组
            byte[] byt = new byte[ch3.Length];//把命令字符数组转换为字节数组
            for (int i = 0; i < ch3.Length; i++)
            {
                byt[i] = Convert.ToByte(ch3[i]);
            }
            tem.SendData(byt);
        }

        #endregion

        #region 补偿值写入
        /// <summary>
        /// 补偿值写入
        /// </summary>
        /// <param name="num"></param>
        /// <param name="cnt"></param>
        /// <param name="data"></param>
        public void ConTempWrite(string num, string n, Int16 data)
        {
            
            //TempWrite(b.ToString(), a.ToString(), Convert.ToInt32(Convert.ToDouble(Temp_Write.Text.Trim()) * 10));
            //地址:01,02,03,04,05,06,07,08,09,0A,0B,0C,0D,0E,0F,10,11,12,13.14
            string s = String.Format("{0:X}", Convert.ToInt64(data));
            if (s.Length > 4)
            {
                s = s.Substring(s.Length - 4);
            }
            else
            {
                s = s.PadLeft(4, '0');
            }
            var aa = data.ToString("X4");
            // int x = Convert.ToInt16("FFE2", 16);
            //string s = data.ToString("X");//转换为16进制
            string str = "0106" + num + "01" + n + aa;//"0106470103E8"
            //Console.WriteLine("补偿：" + str);
            char[] startch = { ':' };
            char[] endch = { '\0', '\0', '\r', '\n' };
            char[] lrcch = tem.LRC(str).ToCharArray();//校验码生成字符数组
                                                      //for (int i = 0; i < 2; i++) {
                                                      //lrcch.CopyTo(endch, i);//把校验码数组嵌入到endch中
                                                      // }
            lrcch.CopyTo(endch, 0);
            char[] datach = str.ToCharArray();//发送内容生成字符数组
            char[] ch3 = new char[datach.Length + 5];
            datach.CopyTo(ch3, 1);//组成命令字符数组
            endch.CopyTo(ch3, datach.Length + 1);//组成命令字符数组
            startch.CopyTo(ch3, 0);//组成命令字符数组
            byte[] byt = new byte[ch3.Length];//把命令字符数组转换为字节数组
            for (int i = 0; i < ch3.Length; i++)
            {
                byt[i] = Convert.ToByte(ch3[i]);
            }
            tem.SendData(byt);
            Thread.Sleep(200);
            
        }
        #endregion

        #region 最高温度写入
        private void UpTempWrite(string num, int data)//TempWrite("01", 110);
        {
            //地址:01,02,03,04,05,06,07,08,09,0A,0B,0C,0D,0E,0F,10,11,12,13.14
            string s = data.ToString("X4");
            //string str = num + "064706" + s;//"0106470603E8"
            string str = num + "061002" + s;//"0106100203E8"
            char[] startch = { ':' };
            char[] endch = { '\0', '\0', '\r', '\n' };
            char[] lrcch = tem.LRC(str).ToCharArray();//校验码生成字符数组
            lrcch.CopyTo(endch, 0);//把校验码数组嵌入到endch中
            char[] datach = str.ToCharArray();//发送内容生成字符数组
            char[] ch3 = new char[datach.Length + 5];
            datach.CopyTo(ch3, 1);//组成命令字符数组
            endch.CopyTo(ch3, datach.Length + 1);//组成命令字符数组
            startch.CopyTo(ch3, 0);//组成命令字符数组
            byte[] byt = new byte[ch3.Length];//把命令字符数组转换为字节数组
            for (int i = 0; i < ch3.Length; i++)
            {
                byt[i] = Convert.ToByte(ch3[i]);
            }
            tem.SendData(byt);
        }

        #endregion

        private void HotModcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //if (HotModcheckBox.Checked == true)
            //{
            //    mc.GTN_SetExtDoBit(1, (short)(26), 1);//扩展模块
            //    mc.GTN_SetExtDoBit(1, (short)(27), 1);
            //    mc.GTN_SetExtDoBit(1, (short)(42), 1);//扩展模块
            //    mc.GTN_SetExtDoBit(1, (short)(43), 1);
            //}
            //else
            //{
            //    // temp = false;//关闭温度读取
            //    mc.GTN_SetExtDoBit(1, (short)(26), 0);//扩展模块
            //    mc.GTN_SetExtDoBit(1, (short)(27), 0);
            //    mc.GTN_SetExtDoBit(1, (short)(42), 0);//扩展模块
            //    mc.GTN_SetExtDoBit(1, (short)(43), 0);
            //    //Temp_Step = 0;
            //    //Temperature.Close();//关闭串口


            //}
        }

        #endregion

        private void label94_Click(object sender, EventArgs e)
        {

        }

        private void label92_Click(object sender, EventArgs e)
        {

        }

        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// /吹气按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            int blowTimeValue = 0;
            if (!int.TryParse(blowTime.Text, out blowTimeValue) || blowTimeValue <= 0)
            {
                MessageBox.Show("请输入有效的吹气时间！");
                return;
            }
            // 启动吹气
            mc.GTN_SetExtDoBit(1, (short)(28), 1);
            // 延时blowTime分钟（转换为秒）
            for (int i = 0; i < blowTimeValue * 60; i++)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(1000);
            }
            // 关闭吹气
            mc.GTN_SetExtDoBit(1, (short)(28), 0);
        }

        // 导出Excel按钮事件
        private void btnExport_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV文件|*.csv";
            sfd.FileName = "温度数据.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("No,Date,Time,CH1");
                    for (int i = 0; i < tempHistory1.Count; i++)
                    {
                        string date = tempTimeHistory[i].ToString("yyyy/M/d");
                        string time = tempTimeHistory[i].ToString("HH:mm:ss");
                        //sw.WriteLine($"{i},{date},{time},{tempHistory1[i]},{tempHistory2[i]}");
                        sw.WriteLine(string.Format("{0},{1},{2},{3}", i, date, time, tempHistory1[i]));
                    }
                }
                MessageBox.Show("导出完成！");
            }
        }
    }
}
