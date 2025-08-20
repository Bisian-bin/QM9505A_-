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
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;

namespace QM9505
{
    public partial class UserForm : Form
    {
        #region 变量
        UDPServer udpServer = new UDPServer();
        INIHelper inidata = new INIHelper();
        ParameterForm parameterForm = new ParameterForm();
        TXT myTXT = new TXT();
        Access access = new Access();

        LoginForm login = new LoginForm();
        DataGrid dataGrid = new DataGrid();
        Function function = new Function();
        Temperature tem = new Temperature();
        public POPForm pop = null;

        Thread CycleThread;
        Thread AutoThread1;
        Thread AutoThread2;
        Thread UpAutoThread1;
        Thread DownAutoThread1;
        Thread UpAutoThread2;
        Thread DownAutoThread2;
        Thread DataThread;
        Thread TimerThread;
        Thread TempThread;
        Thread LampScanThread;
        Thread refresh;

        public static int upCount1;
        public static int downCount1;
        public static int upCounttime1;
        public static int downCounttime1;
        public static int upCount2;
        public static int downCount2;
        public static int upCounttime2;
        public static int downCounttime2;
        public static int upNum1;
        public static int upNum2;
        public static int downNum1;
        public static int downNum2;
        public static int lightFlag;
        public static int grating;
        public int ccds = 0;
        public int AlarmTime = 0;
        public bool above;
        public bool beloew;
        public bool come_up;//上内
        public bool upper_back;//上外
        public bool lower_front;//下内
        public bool lower_back;//下外

        public static string[] UpCellPassNum1 = new string[152];
        public static string[] UpCellFailNum1 = new string[152];
        public static string[] DownCellPassNum1 = new string[152];
        public static string[] DownCellFailNum1 = new string[152];

        public static string[] UpCellPassNum2 = new string[152];
        public static string[] UpCellFailNum2 = new string[152];
        public static string[] DownCellPassNum2 = new string[152];
        public static string[] DownCellFailNum2 = new string[152];

        #endregion

        #region 禁用窗体的关闭按钮
        //禁用窗体的关闭按钮
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        #endregion

        #region 程序主入口
        public UserForm()
        {
            InitializeComponent();
            //OrderNum.SelectAll();//选中文本框中的所有文本
            //OrderNum.Focus();//为控件设置输入焦点
        }

        #endregion

        #region 窗体加载
        private void UserForm_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;//检查线程间非法交
            this.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "V1.0";
            this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = true;

            //加载OK数和NG数
            string path1 = Application.StartupPath + "\\Map\\UpCellPassNum1";
            UpCellPassNum1 = myTXT.ReadTXT(path1);
            string path2 = Application.StartupPath + "\\Map\\UpCellFailNum1";
            UpCellFailNum1 = myTXT.ReadTXT(path2);
            string path3 = Application.StartupPath + "\\Map\\DownCellPassNum1";
            DownCellPassNum1 = myTXT.ReadTXT(path3);
            string path4 = Application.StartupPath + "\\Map\\DownCellFailNum1";
            DownCellFailNum1 = myTXT.ReadTXT(path4);

            string path5 = Application.StartupPath + "\\Map\\UpCellPassNum2";
            UpCellPassNum2 = myTXT.ReadTXT(path5);
            string path6 = Application.StartupPath + "\\Map\\UpCellFailNum2";
            UpCellFailNum2 = myTXT.ReadTXT(path6);
            string path7 = Application.StartupPath + "\\Map\\DownCellPassNum2";
            DownCellPassNum2 = myTXT.ReadTXT(path7);
            string path8 = Application.StartupPath + "\\Map\\DownCellFailNum2";
            DownCellFailNum2 = myTXT.ReadTXT(path8);

            string path9 = Application.StartupPath + "\\Map\\UpNum1";
            string[] str1 = myTXT.ReadTXT(path9);
            upNum1 = Convert.ToInt32(str1[0]);
            string path10 = Application.StartupPath + "\\Map\\DownNum1";
            string[] str2 = myTXT.ReadTXT(path10);
            downNum1 = Convert.ToInt32(str2[0]);

            string path11 = Application.StartupPath + "\\Map\\UpNum2";
            string[] str11 = myTXT.ReadTXT(path11);
            upNum2 = Convert.ToInt32(str1[0]);
            string path12 = Application.StartupPath + "\\Map\\DownNum2";
            string[] str12 = myTXT.ReadTXT(path12);
            downNum2 = Convert.ToInt32(str2[0]);

            for (int i = 0; i < 152; i++)
            {
                Variable.UpCellPassNum1[i] = Convert.ToDouble(UpCellPassNum1[i]);
                Variable.UpCellFailNum1[i] = Convert.ToDouble(UpCellFailNum1[i]);
                Variable.DownCellPassNum1[i] = Convert.ToDouble(DownCellPassNum1[i]);
                Variable.DownCellFailNum1[i] = Convert.ToDouble(DownCellFailNum1[i]);

                Variable.UpCellPassNum2[i] = Convert.ToDouble(UpCellPassNum2[i]);
                Variable.UpCellFailNum2[i] = Convert.ToDouble(UpCellFailNum2[i]);
                Variable.DownCellPassNum2[i] = Convert.ToDouble(DownCellPassNum2[i]);
                Variable.DownCellFailNum2[i] = Convert.ToDouble(DownCellFailNum2[i]);
            }
            button3_Click(null, null);
            Thread.Sleep(1000);
            button5_Click(null, null);
            Variable.MachineState = Variable.MachineStatus.Stop;
            //总数良率计算
            Count();

            #region dataGrid初始化
            dataGrid.IniLeftLoadTrayW(GridU1, 8, 19);
            dataGrid.IniLeftLoadTrayW(GridU2, 8, 19);
            dataGrid.IniLeftLoadTrayW(GridD1, 8, 19);
            dataGrid.IniLeftLoadTrayW(GridD2, 8, 19);

            dataGrid.IniLeftLoadTrayG(VolU_1, 1, 10);
            dataGrid.IniLeftLoadTrayG(VolU_2, 1, 10);
            dataGrid.IniLeftLoadTrayG(VolD_1, 1, 10);
            dataGrid.IniLeftLoadTrayG(VolD_2, 1, 10);

            #endregion

            #region 参数加载

            string path = (System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "//parameter.ini");
            RecordName.Text = inidata.getIni("GN", "parameter", "", path);
            parameterForm.LoadParameter(Application.StartupPath + "\\Parameter.ini");

            #endregion

            #region Test与板卡链接
            //Test与板卡链接  
            bool Socket = udpServer.SocketStart(Variable.ServerIP, Variable.ServerPoint);
            if (!Socket)
            {
                Variable.Serverflag = false;
                MessageBox.Show("测试板1链接失败");
            }
            else
            {
                Variable.Serverflag = true;
            }
            #endregion  

            #region 温控器链接在线

            bool sc1 = false;
            ////Temperature.ComPort(ConfigurationManager.ConnectionStrings["TemperatureCOM1"].ConnectionString);
            parameterForm.btnOpen_Click(null, null);
            sc1 = Temperature.Open();
            if (!sc1)
            {
                MessageBox.Show("温控器1链接失败");
            }


            #endregion

            ClearArry1(Variable.UpCellPassNum1);
            ClearArry1(Variable.UpCellYield1);
            ClearArry1(Variable.DownCellPassNum1);
            ClearArry1(Variable.DownCellYield1);

            ClearArry1(Variable.UpCellPassNum2);
            ClearArry1(Variable.UpCellYield2);
            ClearArry1(Variable.DownCellPassNum2);
            ClearArry1(Variable.DownCellYield2);

            #region 开启线程

            //开启刷新线程
            CycleThread = new Thread(CycleScan);
            CycleThread.IsBackground = true;
            CycleThread.Start();

            //开启自动线程
            AutoThread1 = new Thread(AutoThreadStep1);
            AutoThread1.IsBackground = true;
            AutoThread1.Start();

            //上层内自动线程
            UpAutoThread1 = new Thread(UpAutoThreadStep1);
            UpAutoThread1.IsBackground = true;
            UpAutoThread1.Start();

            //上层外自动线程
            UpAutoThread2 = new Thread(UpAutoThreadStep2);
            UpAutoThread2.IsBackground = true;
            UpAutoThread2.Start();

            //开启自动线程
            AutoThread2 = new Thread(AutoThreadStep2);
            AutoThread2.IsBackground = true;
            AutoThread2.Start();

            //下层内自动线程
            DownAutoThread1 = new Thread(DownAutoThreadStep1);
            DownAutoThread1.IsBackground = true;
            DownAutoThread1.Start();

            //下层外自动线程
            DownAutoThread2 = new Thread(DownAutoThreadStep2);
            DownAutoThread2.IsBackground = true;
            DownAutoThread2.Start();

            //数据处理线程
            DataThread = new Thread(DataThreadStep);
            DataThread.IsBackground = true;
            DataThread.Start();

            //温控器
            TempThread = new Thread(Temp);
            TempThread.IsBackground = true;
            TempThread.Start();

            //三色灯扫描
            LampScanThread = new Thread(LampScan);
            LampScanThread.IsBackground = true;
            LampScanThread.Start();

            //计时
            TimerThread = new Thread(Timer1);
            TimerThread.IsBackground = true;
            TimerThread.Start();

            //刷新
            refresh = new Thread(Refresh_);
            refresh.IsBackground = true;
            refresh.Start();


            #endregion

        }

        #endregion

        #region 刷新
        void Refresh_()
        {
            while (true)
            {

                if (Variable.XStatus[4])//停止
                {
                    Variable.MachineState = Variable.MachineStatus.Stop;
                    statelab.Text = "停止";
                    statelab.BackColor = Color.Yellow;
                    mc.GTN_SetExtDoBit(1, (short)(3), 0);//绿灯灭
                    mc.GTN_SetExtDoBit(1, (short)(2), 1);//黄灯亮
                    mc.GTN_SetExtDoBit(1, (short)(1), 0);//红灯灭
                    mc.GTN_SetExtDoBit(1, (short)(4), 0);//蜂鸣灭
                    mc.GTN_SetExtDoBit(1, (short)(5), 1);//停止亮        
                    above = false;//上层
                    come_up = false;//上前
                    upper_back = false;//上后
                    OutYOFF(23);//1#断电
                    OutYOFF(24);//2#断电
                    beloew = false;//下层
                    lower_front = false;//下前
                    lower_back = false;//下后
                    OutYOFF(39);//1#断电
                    OutYOFF(40);//2#断电
                }
                if ((Variable.XStatus[6] && Variable.XStatus[7]))
                {
                    button4_Click(null, null); //开始按钮
                    above = true;
                }

                if (Variable.XStatus[8] && Variable.XStatus[9])
                {
                    button4_Click(null, null); //开始按钮
                    beloew = true;
                }
                if (Variable.che_door)//门禁
                {
                    if (Variable.MachineState == Variable.MachineStatus.Running)
                    {
                        if (!Variable.XStatus[1])
                        {
                            Down("X0", LogType.Message, "门窗未关闭，请确认！", "", 0, 0);
                        }
                    }
                }
                if (Variable.MachineState == Variable.MachineStatus.Running)
                {
                    if (!Variable.Up1check && !Variable.Up2check && !Variable.Down1check && !Variable.Down2check)
                    {
                        Down("X0", LogType.Message, "(上层内，上层外，下层内，下层外)至少选择一个模组测试，请确认！", "", 0, 0);
                    }
                }
                if (Variable.XStatus[19]) { InPlace1.BackColor = Color.Green; } else { InPlace1.BackColor = Color.LightGray; }
                if (Variable.XStatus[23]) { InPlace2.BackColor = Color.Green; } else { InPlace2.BackColor = Color.LightGray; }
                if (Variable.XStatus[35]) { InPlace3.BackColor = Color.Green; } else { InPlace3.BackColor = Color.LightGray; }
                if (Variable.XStatus[39]) { InPlace4.BackColor = Color.Green; } else { InPlace4.BackColor = Color.LightGray; }
                if (Variable.XStatus[27]) { Location1.BackColor = Color.Green; } else { Location1.BackColor = Color.LightGray; }
                if (Variable.XStatus[43]) { Location2.BackColor = Color.Green; } else { Location2.BackColor = Color.LightGray; }
                Thread.Sleep(20);
            }
        }

        #endregion

        #region 上层自动流程
        void AutoThreadStep1()
        {
            while (true)
            {
                if (above)//上层
                {
                    switch (Variable.UpAutoStep)
                    {
                        case 10:
                            {
                                if (Variable.RunEnable == true)
                                {
                                    //Eif (Variable.XStatus[6] && Variable.XStatus[7])//双手启动
                                    // {
                                    if (!Variable.Up1check && !Variable.Up2check)
                                    {
                                        Variable.UpAutoStep = 10;
                                    }
                                    else { Variable.UpAutoStep = 20; }

                                    // }3q
                                }
                                break;
                            }
                        case 20://清参数
                            {
                                if (Variable.RunEnable == true)
                                {
                                    U1_StartTime.Text = "0";
                                    U1_EndTime.Text = "0";
                                    U1_TestTime.Text = "0";
                                    U2_StartTime.Text = "0";
                                    U2_EndTime.Text = "0";
                                    U2_TestTime.Text = "0";

                                    Variable.UpTotalNum1 = 0;
                                    Variable.UpTotalNum2 = 0;
                                    Variable.UpPassNum1 = 0;
                                    Variable.UpPassNum2 = 0;
                                    Variable.UpFailNum1 = 0;
                                    Variable.UpFailNum2 = 0;
                                    Variable.UpYield1 = 0;
                                    Variable.UpYield2 = 0;

                                    parameterForm.LoadParameter(Application.StartupPath + "\\Parameter.ini");
                                    Variable.UpAutoStep = 30;
                                }
                                break;
                            }
                        case 30://判断平台定位气缸
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.XStatus[16] && Variable.XStatus[17])
                                    {
                                        Variable.UpAutoStep = 40;
                                    }
                                    else
                                    {
                                        Down("X0", LogType.Message, "请检查X016或X017，" + "上层平台定位气缸被拉出，请确认!", "", 0, 0);
                                    }
                                }
                                break;
                            }
                        case 40://检测Tray盘有没有推到位
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Up1check && Variable.Up2check)//上层内外启动
                                    {
                                        /*if (Variable.XStatus[19])
                                        {*/
                                        if (Variable.XStatus[27])
                                        {
                                            if (Variable.XStatus[23])
                                            {
                                                Variable.UpAutoStep = 50;
                                            }
                                            else
                                            {
                                                Down("X0", LogType.Message, "请检查X023，" + "上层2#tray到位信号感应器异常，请确认！", "", 0, 0);
                                            }
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X027，" + "上层2#侧定位气缸感应器动出感应器异常，请确认！", "", 0, 0);
                                        }
                                        /*}1
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X019，" + "上层1#tray到位信号感应器异常，请确认！", "", 0, 0);
                                        }*/
                                    }
                                    else if (Variable.Up1check && !Variable.Up2check)//上层内启动
                                    {
                                        if (Variable.XStatus[19])
                                        {
                                            Variable.UpAutoStep = 50;
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X019，" + "上层1#tray到位信号感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                    else if (!Variable.Up1check && Variable.Up2check)//上层外启动
                                    {
                                        if (Variable.XStatus[27])
                                        {
                                            if (Variable.XStatus[23])
                                            {
                                                Variable.UpAutoStep = 50;
                                            }
                                            else
                                            {
                                                Down("X0", LogType.Message, "请检查X023，" + "上层2#tray到位信号感应器异常，请确认！", "", 0, 0);
                                            }
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X027，" + "上层2#侧定位气缸感应器动出感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                    else
                                    {
                                        Down("X0", LogType.Message, "请选择测试模组，请确认！", "", 0, 0);
                                    }
                                }
                                break;
                            }
                        case 50://内层动作
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Up1check && Variable.Up2check)//上层内外启动
                                    {
                                        OutYOFF(17);
                                        OutYON(18);//上层内气缸上升
                                        OutYOFF(19);
                                        OutYON(20);//上层外气缸上升
                                        //Thread.Sleep(Convert.ToInt32(Variable.time_Alarm));
                                        Thread.Sleep(1000);

                                        if (Variable.XStatus[21] && Variable.XStatus[25])
                                        {
                                            Variable.UpAutoStep = 60;
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X021或X025，" + "上层1#，2#上顶气缸感应器上升感应器异常，请确认！", "", 0, 0);
                                        }

                                    }
                                    else if (Variable.Up1check && !Variable.Up2check)//上层内启动
                                    {
                                        OutYOFF(17);
                                        OutYON(18);//上层内气缸上升
                                        Thread.Sleep(Convert.ToInt32(Variable.time_Alarm));
                                        if (Variable.XStatus[21])
                                        {
                                            Variable.UpAutoStep = 60;
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X021，" + "上层1#上顶气缸感应器上升感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                    else if (!Variable.Up1check && Variable.Up2check)//上层外启动
                                    {
                                        OutYOFF(19);
                                        OutYON(20);//上层外气缸上升
                                        Thread.Sleep(Convert.ToInt32(Variable.time_Alarm));
                                        if (Variable.XStatus[25])
                                        {
                                            Variable.UpAutoStep = 60;
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X025，" + "上层2#上顶气缸感应器上升感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                }
                                break;
                            }
                        case 60://通电
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Up1check && Variable.Up2check)//上层内外启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[18] && !Variable.XStatus[22]/* Convert.ToDouble(labUp1.Text) <= Variable.TempUpLimit && Convert.ToDouble(labUp1.Text) >= Variable.TempDownLimit && !Variable.XStatus[22] && Convert.ToDouble(labUp2.Text) <= Variable.TempUpLimit && Convert.ToDouble(labUp2.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(23);//上层内上电
                                        OutYON(24);//上层外上电
                                        //  Delay(Convert.ToInt32( Variable.Powertime));//延时
                                        Variable.UpAutoStep = 61;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "上层1#，2#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(23);//上层内上电
                                        //    OutYON(24);//上层外上电
                                        //    Delay(5);//延时
                                        //    Variable.UpAutoStep = 70;
                                        //}

                                    }
                                    else if (Variable.Up1check && !Variable.Up2check)//上层内启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[18] /*&& Convert.ToDouble(labUp1.Text) <= Variable.TempUpLimit && Convert.ToDouble(labUp1.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(23);//上层内上电
                                        //   Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.UpAutoStep = 61;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "上层1#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(23);//上层内上电
                                        //    Variable.UpAutoStep = 70;
                                        //}
                                    }
                                    else if (!Variable.Up1check && Variable.Up2check)//上层外启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[22] /*&& Convert.ToDouble(labUp2.Text) <= Variable.TempUpLimit && Convert.ToDouble(labUp2.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(24);//上层内上电
                                        //     Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.UpAutoStep = 61;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "上层2#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(24);//上层内上电
                                        //    Variable.UpAutoStep = 70;
                                        //}
                                    }
                                }
                                break;
                            }

                        case 61:
                            {
                                if (Variable.RunEnable == true)//断电
                                {
                                    if (Variable.Up1check && Variable.Up2check)//上层内外启动
                                    {
                                        OutYOFF(23);
                                        OutYOFF(24);
                                        //  Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.UpAutoStep = 62;
                                    }
                                    else if (Variable.Up1check && !Variable.Up2check)//上层内启动
                                    {

                                        OutYOFF(24);
                                        //  Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.UpAutoStep = 62;
                                    }
                                    else if (!Variable.Up1check && Variable.Up2check)//上层外启动
                                    {

                                        OutYOFF(24);
                                        //  Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.UpAutoStep = 62;
                                    }
                                }
                                break;
                            }

                        case 62:
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Up1check && Variable.Up2check)//上层内外启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[18] && !Variable.XStatus[22]/* Convert.ToDouble(labUp1.Text) <= Variable.TempUpLimit && Convert.ToDouble(labUp1.Text) >= Variable.TempDownLimit && !Variable.XStatus[22] && Convert.ToDouble(labUp2.Text) <= Variable.TempUpLimit && Convert.ToDouble(labUp2.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(23);//上层内上电
                                        OutYON(24);//上层外上电
                                        //      Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.UpAutoStep = 70;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "上层1#，2#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(23);//上层内上电
                                        //    OutYON(24);//上层外上电
                                        //    Delay(5);//延时
                                        //    Variable.UpAutoStep = 70;
                                        //}

                                    }
                                    else if (Variable.Up1check && !Variable.Up2check)//上层内启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[18] /*&& Convert.ToDouble(labUp1.Text) <= Variable.TempUpLimit && Convert.ToDouble(labUp1.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(23);//上层内上电
                                        //     Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.UpAutoStep = 70;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "上层1#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(23);//上层内上电
                                        //    Variable.UpAutoStep = 70;
                                        //}
                                    }
                                    else if (!Variable.Up1check && Variable.Up2check)//上层外启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[22] /*&& Convert.ToDouble(labUp2.Text) <= Variable.TempUpLimit && Convert.ToDouble(labUp2.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(24);//上层内上电
                                        //    Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.UpAutoStep = 70;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "上层2#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(24);//上层内上电
                                        //    Variable.UpAutoStep = 70;
                                        //}
                                    }
                                }
                                break;
                            }



                        case 70://判断1，2是否都测试
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Up1check && Variable.Up2check)//上层内外启动
                                    {
                                        Variable.UpAutoStep1 = 10;
                                        Variable.UpAutoStep2 = 10;
                                        come_up = true;//上内
                                        upper_back = true;//上外
                                        Variable.UpAutoStep = 100;
                                    }
                                    else if (Variable.Up1check && !Variable.Up2check)//上层内启动
                                    {
                                        Variable.UpAutoStep1 = 10;
                                        Variable.UpAutoStep = 100;
                                        come_up = true;//上内

                                    }
                                    else if (!Variable.Up1check && Variable.Up2check)//上层外启动
                                    {
                                        Variable.UpAutoStep2 = 10;
                                        upper_back = true;//上外
                                        Variable.UpAutoStep = 100;
                                    }
                                }
                                break;
                            }
                        case 100://判断1，2是否都测试完成
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Up1check && Variable.Up2check)//上层内外启动
                                    {
                                        if (Variable.UpAutoStep1 == 200 && Variable.UpAutoStep2 == 200)
                                        {
                                            Variable.UpAutoStep = 110;
                                        }
                                    }
                                    else if (Variable.Up1check && !Variable.Up2check)//上层内启动
                                    {
                                        if (Variable.UpAutoStep1 == 200)
                                        {
                                            Variable.UpAutoStep = 110;
                                        }
                                    }
                                    else if (!Variable.Up1check && Variable.Up2check)//上层外启动
                                    {
                                        if (Variable.UpAutoStep2 == 200)
                                        {
                                            Variable.UpAutoStep = 110;
                                        }
                                    }
                                }
                                break;
                            }
                        case 110://断电
                            {
                                if (Variable.RunEnable == true)
                                {
                                    OutYOFF(23);
                                    OutYOFF(24);
                                    //   Thread.Sleep(1000);
                                    Variable.UpAutoStep = 120;
                                }
                                break;
                            }
                        case 120://气缸下降
                            {
                                if (Variable.RunEnable == true)
                                {
                                    OutYOFF(18);
                                    OutYON(17);//上层内气缸下降
                                    OutYOFF(20);
                                    OutYON(19);//上层外气缸下降
                                    //   Thread.Sleep(Convert.ToInt32(Variable.time_Alarm));
                                    if (Variable.XStatus[20] && Variable.XStatus[24])
                                    {
                                        Thread.Sleep(5000);
                                        Variable.UpAutoStep = 130;
                                    }
                                    else { Down("X0", LogType.Message, "请检查X020或X024，" + "请检查，上层1#或2#下降感应器异常，请确认！", "", 0, 0); }
                                }
                                break;
                            }
                        case 130://侧定位气缸回
                            {
                                if (Variable.RunEnable == true)
                                {
                                    OutYOFF(21);//侧定位气缸回
                                    if (Variable.XStatus[26])
                                    {
                                        AlarmTime = 0;
                                        Variable.UpAutoStep = 131;
                                    }
                                    else
                                    {
                                        AlarmTime++;
                                        if (AlarmTime >= Convert.ToInt32(Variable.time_cylinder))
                                        {
                                            AlarmTime = 0;
                                            Down("X0", LogType.Message, "请检查X026，" + "请检查上层X026侧定位回感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                }
                                break;
                            }

                        case 131://侧定位气缸回
                            {
                                if (Variable.RunEnable == true)
                                {
                                    OutYON(22);//推tray出
                                    if (Variable.XStatus[29])
                                    {
                                        AlarmTime = 0;
                                        Variable.UpAutoStep = 132;
                                    }
                                    else
                                    {
                                        AlarmTime++;
                                        if (AlarmTime >= Convert.ToInt32(Variable.time_cylinder))
                                        {
                                            AlarmTime = 0;
                                            Down("X0", LogType.Message, "请检查X029，" + "请检查上层X029推tray出感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                }
                                break;
                            }

                        case 132://侧定位气缸回
                            {
                                if (Variable.RunEnable == true)
                                {
                                    OutYOFF(22);//推tray回
                                    if (Variable.XStatus[28])
                                    {
                                        AlarmTime = 0;
                                        Variable.UpAutoStep = 140;
                                    }
                                    else
                                    {
                                        AlarmTime++;
                                        if (AlarmTime >= Convert.ToInt32(Variable.time_cylinder))
                                        {
                                            AlarmTime = 0;
                                            Down("X0", LogType.Message, "请检查X028，" + "请检查上层X028推tray回感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                }
                                break;
                            }

                        case 140://测试完成
                            {
                                if (Variable.RunEnable == true)
                                {
                                    statelab.Text = "上层测试完成";
                                    //Down("X0", LogType.Message, "测试完成，请确认！", "", 0, 0);
                                    Variable.UpAutoStep = 10;
                                    above = false;
                                }
                                break;
                            }
                    }
                }
                //Thread.Sleep(10);

            }
        }
        #endregion

        #region 上层内自动流程
        void UpAutoThreadStep1()
        {
            while (true)
            {
                if (come_up)
                {
                    switch (Variable.UpAutoStep1)
                    {
                        case 10://等待读取Test,开始测试
                            {
                                if (Variable.Up1check)
                                {

                                    dataGrid.IniLeftLoadTrayW(GridU1, 8, 19);
                                    U1_StartTime.Text = "0";
                                    U1_EndTime.Text = "0";
                                    U1_TestTime.Text = "0";
                                    Variable.UpTotalNum1 = 0;
                                    Variable.UpPassNum1 = 0;
                                    Variable.UpFailNum1 = 0;
                                    Variable.UpYield1 = 0;

                                    //上层清空
                                    ClearArry(Variable.receiveUPState1);
                                    ClearArry(Variable.receiveUPVCCVolt1);
                                    ClearArry(Variable.receiveUPVCQVolt1);
                                    ClearArry(Variable.receiveUPVCCElec1);
                                    ClearArry(Variable.receiveUPVCQElec1);
                                    upCount1 = 0;
                                    upCounttime1 = 0;
                                    for (int i = 0; i < 10; i++)
                                    {
                                        Variable.receiveUPVCC1[i] = "";
                                        Variable.receiveUPVCQ1[i] = "";
                                    }

                                    U1_StartTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//获取当时时间
                                    //Thread.Sleep(Convert.ToInt16(testWaitTime.Text));//测试等待
                                    Variable.UpAutoStep1 = 20; //开始测试 
                                }

                                break;
                            }
                        case 20://VCQ写入判断
                            {
                                if (Variable.VCQCheck == true)
                                {
                                    U1_VC.Text = "VCQ";
                                    Variable.UpAutoStep1 = 30;//写入VCQ电压
                                }
                                else
                                {
                                    U1_VC.Text = "VCC";
                                    Variable.UpAutoStep1 = 50;//写入VCC电压
                                }
                                break;
                            }
                        case 30://写入VCQ电压
                            {

                                string str = "";
                                string strSend = "";
                                string model = "";

                                if ((Convert.ToDouble(Variable.VCQVolSet) * 10) > 36 || (Convert.ToDouble(Variable.VCQVolSet) * 10) < 18)
                                {
                                    MessageBox.Show("电压设定超出范围");
                                }
                                else
                                {
                                    string Vol = Convert.ToInt32((Convert.ToDouble(Variable.VCQVolSet) * 10)).ToString("X2");

                                    model = "31";
                                    str = "A55A0400FF" + model + Vol;
                                    string str1 = BCC(str);
                                    strSend = str + str1 + "FF";
                                }

                                //发送数据
                                UpSendDataVCQ1(strSend);
                                Variable.UpAutoStep1 = 40;
                                break;
                            }
                        case 40://写入VCQ电压判断
                            {
                                bool flag = false;
                                for (int i = 0; i < 10; i++)
                                {
                                    if (Variable.receiveUPVCQ1[i] != "" && Variable.receiveUPVCQ1[i] != null)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                        break;
                                    }
                                }

                                if (flag)
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        Variable.receiveUPVCQ1[i] = "";
                                    }
                                    U1_VC.Text = "VCC";
                                    upCount1 = 0;
                                    upCounttime1 = 0;
                                    Variable.UpAutoStep1 = 50;
                                }
                                else
                                {
                                    upCount1 += 1;
                                    upCounttime1 += 1;
                                    Variable.UpAutoStep1 = 30;
                                    if (upCount1 > 3)
                                    {
                                        //MessageBox.Show("上层电压设定写入失败");
                                        upCount1 = 0;
                                    }
                                    if (upCounttime1 > 1000)
                                    {
                                        string[] str = new string[1] { "1" };
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm1");
                                        /* if (MessageBox.Show("上层VCQ电压设定写入失败?选择'是'重新写入，选择'否'放弃写入！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                         {
                                             str[0] = "0";
                                             myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm1");
                                             upCounttime1 = 0;
                                         }
                                         else
                                         {
                                             str[0] = "0";
                                             myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm1");
                                             upCounttime1 = 0;
                                             Variable.UpAutoStep1 = 50;
                                         }
                                         */
                                        str[0] = "0";
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm1");
                                        upCounttime1 = 0;
                                        Variable.UpAutoStep1 = 50;
                                    }
                                }
                                break;
                            }
                        case 50://写入VCC电压
                            {
                                string str = "";
                                string model = "";
                                string strSend = "";

                                if ((Convert.ToDouble(Variable.VCCVolSet) * 10) > 36 || (Convert.ToDouble(Variable.VCCVolSet) * 10) < 18)
                                {
                                    MessageBox.Show("电压设定超出范围");
                                }
                                else
                                {
                                    string Vol = Convert.ToInt32((Convert.ToDouble(Variable.VCCVolSet) * 10)).ToString("X2");

                                    model = "30";//VCC
                                    str = "A55A0400FF" + model + Vol;
                                    string str1 = BCC(str);
                                    strSend = str + str1 + "FF";
                                }

                                //发送数据
                                UpSendDataVCC1(strSend);
                                Variable.UpAutoStep1 = 60;
                                break;
                            }
                        case 60://写入VCC电压判断
                            {
                                bool flag = false;
                                for (int i = 0; i < 10; i++)
                                {
                                    if (Variable.receiveUPVCC1[i] != "" && Variable.receiveUPVCC1[i] != null)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                        break;
                                    }
                                }

                                if (flag)
                                {
                                    upCount1 = 0;
                                    upCounttime1 = 0;
                                    Variable.UpAutoStep1 = 70;
                                }
                                else
                                {
                                    upCount1 += 1;
                                    upCounttime1 += 1;
                                    Variable.UpAutoStep1 = 50;
                                    if (upCount1 > 3)
                                    {
                                        //MessageBox.Show("上层电压设定写入失败");
                                        upCount1 = 0;
                                    }
                                    if (upCounttime1 > 1000)
                                    {
                                        string[] str = new string[1] { "2" };
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm1");
                                        /* if (MessageBox.Show("上层VCC电压设定写入失败?选择'是'重新写入，选择'否'放弃写入！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                         {
                                             str[0] = "0";
                                             myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm1");
                                             upCounttime1 = 0;
                                         }
                                         else
                                         {
                                             str[0] = "0";
                                             myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm1");
                                             upCounttime1 = 0;
                                             Variable.UpAutoStep1 = 70;
                                         }*/
                                        str[0] = "0";
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm1");
                                        upCounttime1 = 0;
                                        Variable.UpAutoStep1 = 70;

                                    }
                                }
                                break;
                            }
                        //UDP通信
                        case 70://读取状态
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    string str = "A5 5A 03 00 FF 22 23 FF";// 状态查询

                                    //发送数据
                                    UpSendData1(str);
                                    Variable.UpAutoStep1 = 80;
                                }
                                break;
                            }

                        case 80://读取电流
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    string str = "A5 5A 03 00 FF 21 22 FF ";// 电流查询

                                    //发送数据
                                    UpSendData1(str);
                                    Variable.UpAutoStep1 = 90;
                                }
                                break;
                            }
                        case 90://VCQ写入判断
                            {
                                if (Variable.VCQCheck == true)
                                {
                                    Variable.UpAutoStep1 = 100;
                                }
                                else
                                {
                                    Variable.UpAutoStep1 = 110;
                                }
                                break;
                            }
                        case 100://读取VCQ电压
                            {
                                string str = "";
                                str = "A5 5A 03 00 FF 20 21 FF";// VCQ电压查询
                                //发送数据
                                UpSendData1(str);

                                Variable.UpAutoStep1 = 110;

                                break;
                            }
                        case 110://读取VCC电压
                            {
                                string str = "";
                                str = "A5 5A 03 00 FF 20 21 FF";// VCQ电压查询
                                //发送数据
                                UpSendData1(str);
                                Variable.UpAutoStep1 = 120;
                                break;
                            }
                        case 120://测试时间判断
                            {
                                try
                                {
                                    if (Variable.testTimeReadCheck)//模拟测试时间
                                    {
                                        if (Convert.ToInt64(U1_TestTime.Text) >= 100)
                                        {
                                            //将其余没有测试好的赋超时值
                                            for (int i = 0; i < 19; i++)
                                            {
                                                for (int j = 0; j < 8; j++)
                                                {
                                                    if (Variable.receiveUPState1[i * 8 + j] != "00")
                                                    {
                                                        Variable.receiveUPState1[i * 8 + j] = "02";
                                                    }
                                                }
                                            }
                                            Variable.UpAutoStep1 = 140;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt64(U1_TestTime.Text) <= 100)
                                            {
                                                Variable.UpAutoStep1 = 70;
                                            }
                                            else
                                            {
                                                Variable.UpAutoStep1 = 130;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt64(U1_TestTime.Text) >= Convert.ToInt64(Variable.testTime))
                                        {
                                            //将其余没有测试好的赋超时值
                                            //for (int i = 0; i < 19; i++)
                                            //{
                                            //    for (int j = 0; j < 8; j++)
                                            //    {
                                            //        if (Variable.receiveUPState1[i * 8 + j] != "00")
                                            //        {
                                            //            Variable.receiveUPState1[i * 8 + j] = "02";
                                            //        }
                                            //    }
                                            //}
                                            Variable.UpAutoStep1 = 140;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt64(U1_TestTime.Text) <= Convert.ToInt64(Variable.testWaitTime))
                                            {
                                                Variable.UpAutoStep1 = 70;
                                            }
                                            else
                                            {
                                                Variable.UpAutoStep1 = 130;
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                break;
                            }

                        case 130://判断是否都测试完成
                            {
                                bool b = true;//测试完成
                                for (int i = 0; i < 19; i++)
                                {
                                    if (b)
                                    {
                                        for (int j = 0; j < 8; j++)
                                        {
                                            if (b)
                                            {
                                                if (Variable.receiveUPState1[i * 8 + j] != null)//不为空
                                                {
                                                    if (Variable.receiveUPState1[i * 8 + j] == "01")
                                                    {
                                                        b = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        b = true;//测试完成
                                                    }
                                                }
                                                else//为空
                                                {
                                                    b = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (b)
                                {
                                    Variable.UpAutoStep1 = 140;
                                }
                                else
                                {
                                    Variable.UpAutoStep1 = 70;
                                }
                                break;
                            }

                        case 140://测试完成
                            {
                                GridUpCount1(Variable.receiveUPState1);
                                //上层总数良率计算
                                Count();
                                U1_EndTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                Variable.UpAutoStep1 = 150;

                                break;
                            }
                        case 150://发送Tray数据
                            {
                                myTXT.WriteTxt(Variable.receiveUPState1, Application.StartupPath + "\\Map\\Uptray1");
                                Variable.UpAutoStep1 = 160;
                                break;
                            }

                        case 160://删除Test文件
                            {
                                Thread.Sleep(500);
                                upNum1 += 1;
                                UpTotalToRecord1(); //将上层内总数量保存到数据库
                                UpYieldToRecord1();//将上层内良率保存到数据库
                                string path1 = Application.StartupPath + "\\Map\\UpNum1";
                                string[] str = new string[1];
                                str[0] = upNum1.ToString();
                                myTXT.WriteTxtVariable(str, path1);

                                Variable.UpAutoStep1 = 200;

                                break;
                            }
                    }
                }
                Thread.Sleep(10);
            }
        }
        #endregion

        #region 上层外自动流程
        void UpAutoThreadStep2()
        {
            while (true)
            {
                if (upper_back)
                {
                    switch (Variable.UpAutoStep2)
                    {
                        case 10://等待读取Test,开始测试
                            {
                                if (Variable.Up2check)
                                {

                                    dataGrid.IniLeftLoadTrayW(GridU2, 8, 19);
                                    U2_StartTime.Text = "0";
                                    U2_EndTime.Text = "0";
                                    U2_TestTime.Text = "0";
                                    Variable.UpTotalNum2 = 0;
                                    Variable.UpPassNum2 = 0;
                                    Variable.UpFailNum2 = 0;
                                    Variable.UpYield2 = 0;

                                    //上层清空
                                    ClearArry(Variable.receiveUPState2);
                                    ClearArry(Variable.receiveUPVCCVolt2);
                                    ClearArry(Variable.receiveUPVCQVolt2);
                                    ClearArry(Variable.receiveUPVCCElec2);
                                    ClearArry(Variable.receiveUPVCQElec2);
                                    upCount2 = 0;
                                    upCounttime2 = 0;
                                    for (int i = 0; i < 10; i++)
                                    {
                                        Variable.receiveUPVCC2[i] = "";
                                        Variable.receiveUPVCQ2[i] = "";
                                    }

                                    U2_StartTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//获取当时时间
                                    //Thread.Sleep(Convert.ToInt16(testWaitTime.Text));//测试等待
                                    Variable.UpAutoStep2 = 20; //开始测试 

                                }
                                break;
                            }
                        case 20://VCQ写入判断
                            {
                                if (Variable.VCQCheck == true)
                                {
                                    U2_VC.Text = "VCQ";
                                    Variable.UpAutoStep2 = 30;//写入VCQ电压
                                }
                                else
                                {
                                    U2_VC.Text = "VCC";
                                    Variable.UpAutoStep2 = 50;//写入VCC电压
                                }
                                break;
                            }
                        case 30://写入VCQ电压
                            {
                                string str = "";
                                string model = "";
                                string strSend = "";

                                if ((Convert.ToDouble(Variable.VCQVolSet) * 10) > 36 || (Convert.ToDouble(Variable.VCQVolSet) * 10) < 18)
                                {
                                    MessageBox.Show("电压设定超出范围");
                                }
                                else
                                {
                                    string Vol = Convert.ToInt32((Convert.ToDouble(Variable.VCQVolSet) * 10)).ToString("X2");

                                    model = "31";//VCQ
                                    str = "A55A0400FF" + model + Vol;
                                    string str1 = BCC(str);
                                    strSend = str + str1 + "FF";
                                }

                                //发送数据
                                UpSendDataVCQ2(strSend);
                                Variable.UpAutoStep2 = 40;

                                break;
                            }
                        case 40://写入VCQ电压判断
                            {
                                bool flag = false;
                                for (int i = 0; i < 10; i++)
                                {
                                    if (Variable.receiveUPVCQ2[i] != "" && Variable.receiveUPVCQ2[i] != null)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                        break;
                                    }
                                }

                                if (flag)
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        Variable.receiveUPVCQ2[i] = "";
                                    }
                                    U2_VC.Text = "VCC";
                                    upCount2 = 0;
                                    upCounttime2 = 0;
                                    Variable.UpAutoStep2 = 50;
                                }
                                else
                                {
                                    upCount2 += 1;
                                    upCounttime2 += 1;
                                    Variable.UpAutoStep2 = 30;
                                    if (upCount2 > 3)
                                    {
                                        //MessageBox.Show("上层电压设定写入失败");
                                        upCount2 = 0;
                                    }
                                    if (upCounttime2 > 1000)
                                    {
                                        string[] str = new string[1] { "1" };
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm2");
                                        /* if (MessageBox.Show("上层VCQ电压设定写入失败?选择'是'重新写入，选择'否'放弃写入！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                         {
                                             str[0] = "0";
                                             myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm2");
                                             upCounttime2 = 0;
                                         }
                                         else
                                         {
                                             str[0] = "0";
                                             myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm2");
                                             upCounttime2 = 0;
                                             Variable.UpAutoStep2 = 50;
                                         }*/
                                        str[0] = "0";
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm2");
                                        upCounttime2 = 0;
                                        Variable.UpAutoStep2 = 50;
                                    }
                                }
                                break;
                            }
                        case 50://写入VCC电压
                            {
                                string str = "";
                                string model = "";
                                string strSend = "";

                                if ((Convert.ToDouble(Variable.VCCVolSet) * 10) > 36 || (Convert.ToDouble(Variable.VCCVolSet) * 10) < 18)
                                {
                                    MessageBox.Show("电压设定超出范围");
                                }
                                else
                                {
                                    string Vol = Convert.ToInt32((Convert.ToDouble(Variable.VCCVolSet) * 10)).ToString("X2");

                                    model = "30";//VCC
                                    str = "A55A0400FF" + model + Vol;
                                    string str1 = BCC(str);
                                    strSend = str + str1 + "FF";
                                }

                                //发送数据
                                UpSendDataVCC2(strSend);
                                Variable.UpAutoStep2 = 60;
                                break;
                            }
                        case 60://写入VCC电压判断
                            {
                                bool flag = false;
                                for (int i = 0; i < 10; i++)
                                {
                                    if (Variable.receiveUPVCC2[i] != "" && Variable.receiveUPVCC2[i] != null)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                        break;
                                    }
                                }

                                if (flag)
                                {
                                    upCount2 = 0;
                                    upCounttime2 = 0;
                                    Variable.UpAutoStep2 = 70;
                                }
                                else
                                {
                                    upCount2 += 1;
                                    upCounttime2 += 1;
                                    Variable.UpAutoStep2 = 50;
                                    if (upCount2 > 3)
                                    {
                                        //MessageBox.Show("上层电压设定写入失败");
                                        upCount2 = 0;
                                    }
                                    if (upCounttime2 > 1000)
                                    {
                                        string[] str = new string[1] { "2" };
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm2");
                                        /* if (MessageBox.Show("上层VCC电压设定写入失败?选择'是'重新写入，选择'否'放弃写入！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                         {
                                             str[0] = "0";
                                             myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm2");
                                             upCounttime2 = 0;
                                         }
                                         else
                                         {
                                             str[0] = "0";
                                             myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm2");
                                             upCounttime2 = 0;
                                             Variable.UpAutoStep2 = 70;
                                         }
                                         */
                                        str[0] = "0";
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\UpAlarm2");
                                        upCounttime2 = 0;
                                        Variable.UpAutoStep2 = 70;
                                    }
                                }
                                break;
                            }
                        //UDP通信
                        case 70://读取状态
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    string str = "A5 5A 03 00 FF 22 23 FF";// 状态查询

                                    //发送数据
                                    UpSendData2(str);
                                    Variable.UpAutoStep2 = 80;
                                }
                                break;
                            }

                        case 80://读取电流
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    string str = "A5 5A 03 00 FF 21 22 FF ";// 电流查询

                                    //发送数据
                                    UpSendData2(str);
                                    Variable.UpAutoStep2 = 90;
                                }
                                break;
                            }
                        case 90://VCQ写入判断
                            {
                                if (Variable.VCQCheck == true)
                                {
                                    Variable.UpAutoStep2 = 100;
                                }
                                else
                                {
                                    Variable.UpAutoStep2 = 110;
                                }
                                break;
                            }
                        case 100://读取VCQ电压
                            {
                                string str = "";
                                str = "A5 5A 03 00 FF 20 21 FF";// VCQ电压查询
                                //发送数据
                                UpSendData2(str);

                                Variable.UpAutoStep2 = 110;

                                break;
                            }
                        case 110://读取VCC电压
                            {
                                string str = "";
                                str = "A5 5A 03 00 FF 20 21 FF";// VCQ电压查询
                                //发送数据
                                UpSendData2(str);

                                Variable.UpAutoStep2 = 120;

                                break;
                            }
                        case 120://测试时间判断
                            {
                                try
                                {
                                    if (Variable.testTimeReadCheck)//模拟测试时间
                                    {
                                        if (Convert.ToInt64(U2_TestTime.Text) >= 100)
                                        {
                                            //将其余没有测试好的赋超时值
                                            for (int i = 0; i < 19; i++)
                                            {
                                                for (int j = 0; j < 8; j++)
                                                {
                                                    if (Variable.receiveUPState2[i * 8 + j] != "00")
                                                    {
                                                        Variable.receiveUPState2[i * 8 + j] = "02";
                                                    }
                                                }
                                            }
                                            Variable.UpAutoStep2 = 140;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt64(U2_TestTime.Text) <= 100)
                                            {
                                                Variable.UpAutoStep2 = 70;
                                            }
                                            else
                                            {
                                                Variable.UpAutoStep2 = 130;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt64(U2_TestTime.Text) >= Convert.ToInt64(Variable.testTime))
                                        {
                                            //将其余没有测试好的赋超时值
                                            //for (int i = 0; i < 19; i++)
                                            //{
                                            //    for (int j = 0; j < 8; j++)
                                            //    {
                                            //        if (Variable.receiveUPState2[i * 8 + j] != "00")
                                            //        {
                                            //            Variable.receiveUPState2[i * 8 + j] = "02";
                                            //        }
                                            //    }
                                            //}
                                            Variable.UpAutoStep2 = 140;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt64(U2_TestTime.Text) <= Convert.ToInt64(Variable.testWaitTime))
                                            {
                                                Variable.UpAutoStep2 = 70;
                                            }
                                            else
                                            {
                                                Variable.UpAutoStep2 = 130;
                                            }
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                break;
                            }

                        case 130://判断是否都测试完成
                            {
                                bool b = true;//测试完成
                                for (int i = 0; i < 19; i++)
                                {
                                    if (b)
                                    {
                                        for (int j = 0; j < 8; j++)
                                        {
                                            if (b)
                                            {
                                                if (Variable.receiveUPState2[i * 8 + j] != null)//不为空
                                                {
                                                    if (Variable.receiveUPState2[i * 8 + j] == "01")
                                                    {
                                                        b = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        b = true;//测试完成
                                                    }
                                                }
                                                else//为空
                                                {
                                                    b = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (b)
                                {
                                    Variable.UpAutoStep2 = 140;
                                }
                                else
                                {
                                    Variable.UpAutoStep2 = 70;
                                }
                                break;
                            }

                        case 140://测试完成
                            {
                                GridUpCount2(Variable.receiveUPState2);
                                //上层总数良率计算
                                Count();
                                U2_EndTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                Variable.UpAutoStep2 = 150;
                                break;
                            }
                        case 150://发送Tray数据
                            {
                                myTXT.WriteTxt(Variable.receiveUPState2, Application.StartupPath + "\\Map\\Uptray2");
                                Variable.UpAutoStep2 = 160;

                                break;
                            }

                        case 160://删除Test文件
                            {
                                Thread.Sleep(500);
                                upNum2 += 1;
                                UpTotalToRecord2();//将上层外总数量保存到数据库
                                UpYieldToRecord2();//将上层外良率保存到数据库
                                string path1 = Application.StartupPath + "\\Map\\UpNum2";
                                string[] str = new string[1];
                                str[0] = upNum2.ToString();
                                myTXT.WriteTxtVariable(str, path1);

                                Variable.UpAutoStep2 = 200;

                                break;
                            }
                    }
                }
                Thread.Sleep(10);
            }
        }
        #endregion

        #region 下层自动流程
        void AutoThreadStep2()
        {
            while (true)
            {
                if (beloew)//下层
                {
                    switch (Variable.DownAutoStep)
                    {
                        case 10:
                            {
                                if (Variable.RunEnable == true)
                                {
                                    //if (Variable.XStatus[8] && Variable.XStatus[9])//双手启动
                                    //{                        
                                    if (!Variable.Down1check && !Variable.Down2check)
                                    {
                                        Variable.DownAutoStep = 10;
                                    }
                                    else { Variable.DownAutoStep = 20; }

                                    //}
                                }
                                break;
                            }
                        case 20://清参数
                            {
                                if (Variable.RunEnable == true)
                                {
                                    D1_StartTime.Text = "0";
                                    D1_EndTime.Text = "0";
                                    D1_TestTime.Text = "0";
                                    D2_StartTime.Text = "0";
                                    D2_EndTime.Text = "0";
                                    D2_TestTime.Text = "0";

                                    Variable.DownTotalNum1 = 0;
                                    Variable.DownTotalNum2 = 0;
                                    Variable.DownPassNum1 = 0;
                                    Variable.DownPassNum2 = 0;
                                    Variable.DownFailNum1 = 0;
                                    Variable.DownFailNum2 = 0;
                                    Variable.DownYield1 = 0;
                                    Variable.DownYield2 = 0;

                                    parameterForm.LoadParameter(Application.StartupPath + "\\Parameter.ini");
                                    Variable.DownAutoStep = 30;
                                }
                                break;
                            }
                        case 30://判断平台定位气缸
                            {
                                if (Variable.RunEnable == true)
                                {
                                    //if (Variable.XStatus[32] && Variable.XStatus[33])
                                    if (true)
                                    {
                                        Variable.DownAutoStep = 40;
                                    }
                                    else
                                    {
                                        Down("X0", LogType.Message, "请检查X032或X038，" + "下层平台定位气缸被拉出，请确认!", "", 0, 0);
                                    }
                                }
                                break;
                            }
                        case 40://检测Tray盘有没有推到位
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Down1check && Variable.Down2check)//下层内外启动
                                    {
                                        if (Variable.XStatus[35])
                                        {
                                            if (Variable.XStatus[43])
                                            {
                                                if (!Variable.XStatus[39])
                                                {
                                                    Variable.DownAutoStep = 50;
                                                }
                                                else
                                                {
                                                    Down("X0", LogType.Message, "请检查X039，" + "下层2#tray到位信号感应器异常，请确认！", "", 0, 0);
                                                }
                                            }
                                            else
                                            {
                                                Down("X0", LogType.Message, "请检查X043，" + "下层2#侧定位气缸感应器动出感应器异常，请确认！", "", 0, 0);
                                            }
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X035，" + "下层1#tray到位信号感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                    else if (Variable.Down1check && !Variable.Down2check)//下层内启动
                                    {
                                        if (Variable.XStatus[35])
                                        {
                                            Variable.DownAutoStep = 50;
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X035，" + "下层1#tray到位信号感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                    else if (!Variable.Down1check && Variable.Down2check)//下层外启动
                                    {
                                        if (Variable.XStatus[43])
                                        {
                                            if (!Variable.XStatus[39])
                                            {
                                                Variable.DownAutoStep = 50;
                                            }
                                            else
                                            {
                                                Down("X0", LogType.Message, "请检查X039，" + "下层2#tray到位信号感应器异常，请确认！", "", 0, 0);
                                            }
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X043，" + "下层2#侧定位气缸感应器动出感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                    else
                                    {
                                        Down("X0", LogType.Message, "请选择测试模组，请确认！", "", 0, 0);
                                    }
                                }
                                break;
                            }
                        case 50://内层动作
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Down1check && Variable.Down2check)//下层内外启动
                                    {
                                        OutYOFF(33);
                                        OutYON(34);//下层内气缸上升
                                        OutYOFF(35);
                                        OutYON(36);//下层外气缸上升
                                        // Thread.Sleep(Convert.ToInt32(Variable.time_Alarm));
                                        Thread.Sleep(12000);
                                        if (Variable.XStatus[37] && Variable.XStatus[41])
                                        {
                                            Variable.DownAutoStep = 60;
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X037或X041，" + "下层1#，2#上顶气缸感应器上升感应器异常，请确认！", "", 0, 0);
                                        }

                                    }
                                    else if (Variable.Down1check && !Variable.Down2check)//下层内启动
                                    {
                                        OutYOFF(33);
                                        OutYON(34);//下层内气缸上升
                                        Thread.Sleep(Convert.ToInt32(Variable.time_Alarm));
                                        if (Variable.XStatus[37])
                                        {
                                            Variable.DownAutoStep = 60;
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X037，" + "下层1#上顶气缸感应器上升感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                    else if (!Variable.Down1check && Variable.Down2check)//下层外启动
                                    {
                                        OutYOFF(35);
                                        OutYON(36);//下层外气缸上升
                                        Thread.Sleep(Convert.ToInt32(Variable.time_Alarm));
                                        if (Variable.XStatus[41])
                                        {
                                            Variable.DownAutoStep = 60;
                                        }
                                        else
                                        {
                                            Down("X0", LogType.Message, "请检查X041，" + "下层2#上顶气缸感应器上升感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                }
                                break;
                            }
                        case 60://通电
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Down1check && Variable.Down2check)//下层内外启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[34] && !Variable.XStatus[38]/* Convert.ToDouble(labDown1.Text) <= Variable.TempUpLimit && Convert.ToDouble(labDown1.Text) >= Variable.TempDownLimit && !Variable.XStatus[38] && Convert.ToDouble(labDown2.Text) <= Variable.TempUpLimit && Convert.ToDouble(labDown2.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(39);//下层内上电
                                        OutYON(40);//下层外上电
                                        // Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.DownAutoStep = 61;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "下层1#，2#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(39);//下层内上电
                                        //    OutYON(40);//下层外上电
                                        //    Variable.DownAutoStep = 70;
                                        //}

                                    }
                                    else if (Variable.Down1check && !Variable.Down2check)//下层内启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[34] /*&& Convert.ToDouble(labDown1.Text) <= Variable.TempUpLimit && Convert.ToDouble(labDown1.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(39);//下层内上电
                                        // Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.DownAutoStep = 61;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "下层1#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(39);//下层内上电
                                        //    Variable.DownAutoStep = 70;
                                        //}
                                    }
                                    else if (!Variable.Down1check && Variable.Down2check)//下层外启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[38] /*&& Convert.ToDouble(labDown2.Text) <= Variable.TempUpLimit && Convert.ToDouble(labDown2.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        //    OutYON(40);//下层外上电
                                        //Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.DownAutoStep = 61;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "下层2#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(40);//下层内上电
                                        //    Variable.DownAutoStep = 70;
                                        //}
                                    }
                                }
                                break;
                            }

                        case 61:
                            {
                                if (Variable.RunEnable == true)//断电
                                {
                                    if (Variable.Down1check && Variable.Down2check)//下层内外启动
                                    {
                                        OutYOFF(39);
                                        OutYOFF(40);
                                        //Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.DownAutoStep = 62;
                                    }
                                    else if (Variable.Down1check && !Variable.Down2check)//下层内启动
                                    {

                                        OutYOFF(39);
                                        //Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.DownAutoStep = 62;
                                    }
                                    else if (!Variable.Down1check && Variable.Down2check)//下层外启动
                                    {

                                        OutYOFF(40);
                                        //Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.DownAutoStep = 62;
                                    }
                                }
                                break;
                            }


                        case 62://通电
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Down1check && Variable.Down2check)//下层内外启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[34] && !Variable.XStatus[38]/* Convert.ToDouble(labDown1.Text) <= Variable.TempUpLimit && Convert.ToDouble(labDown1.Text) >= Variable.TempDownLimit && !Variable.XStatus[38] && Convert.ToDouble(labDown2.Text) <= Variable.TempUpLimit && Convert.ToDouble(labDown2.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(39);//下层内上电
                                        OutYON(40);//下层外上电
                                        //Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.DownAutoStep = 70;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "下层1#，2#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(39);//下层内上电
                                        //    OutYON(40);//下层外上电
                                        //    Variable.DownAutoStep = 70;
                                        //}

                                    }
                                    else if (Variable.Down1check && !Variable.Down2check)//下层内启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[34] /*&& Convert.ToDouble(labDown1.Text) <= Variable.TempUpLimit && Convert.ToDouble(labDown1.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(39);//下层内上电
                                        //Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.DownAutoStep = 70;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "下层1#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(39);//下层内上电
                                        //    Variable.DownAutoStep = 70;
                                        //}
                                    }
                                    else if (!Variable.Down1check && Variable.Down2check)//下层外启动
                                    {
                                        //if (Variable.HotModel == true)
                                        //{
                                        //if (!Variable.XStatus[38] /*&& Convert.ToDouble(labDown2.Text) <= Variable.TempUpLimit && Convert.ToDouble(labDown2.Text) >= Variable.TempDownLimit*/)
                                        //{
                                        OutYON(40);//下层外上电
                                        // Delay(Convert.ToInt32(Variable.Powertime));//延时
                                        Variable.DownAutoStep = 70;
                                        //}
                                        //else
                                        //{
                                        //    Down("X0", LogType.Message, "下层2#温度未达到，请确认！", "", 0, 0);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    OutYON(40);//下层内上电
                                        //    Variable.DownAutoStep = 70;
                                        //}
                                    }
                                }
                                break;
                            }

                        case 70://判断1，2是否都测试
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Down1check && Variable.Down2check)//下层内外启动
                                    {
                                        Variable.DownAutoStep1 = 10;
                                        Variable.DownAutoStep2 = 10;
                                        lower_front = true;//下内
                                        lower_back = true;//下外
                                        Variable.DownAutoStep = 100;
                                    }
                                    else if (Variable.Down1check && !Variable.Down2check)//下层内启动
                                    {
                                        Variable.DownAutoStep1 = 10;

                                        Variable.DownAutoStep = 100;
                                        lower_front = true;//下内
                                    }
                                    else if (!Variable.Down1check && Variable.Down2check)//下层外启动
                                    {
                                        Variable.DownAutoStep2 = 10;

                                        Variable.DownAutoStep = 100;
                                        lower_back = true;//下外
                                    }
                                }
                                break;
                            }
                        case 100://判断1，2是否都测试完成
                            {
                                if (Variable.RunEnable == true)
                                {
                                    if (Variable.Down1check && Variable.Down2check)//下层内外启动
                                    {
                                        if (Variable.DownAutoStep1 == 200 && Variable.DownAutoStep2 == 200)
                                        {
                                            Variable.DownAutoStep = 110;
                                        }
                                    }
                                    else if (Variable.Down1check && !Variable.Down2check)//下层内启动
                                    {
                                        if (Variable.DownAutoStep1 == 200)
                                        {
                                            Variable.DownAutoStep = 110;
                                        }
                                    }
                                    else if (!Variable.Down1check && Variable.Down2check)//下层外启动
                                    {
                                        if (Variable.DownAutoStep2 == 200)
                                        {
                                            Variable.DownAutoStep = 110;
                                        }
                                    }
                                }
                                break;
                            }
                        case 110://断电
                            {
                                if (Variable.RunEnable == true)
                                {
                                    OutYOFF(39);
                                    OutYOFF(40);
                                    // Thread.Sleep(1000);
                                    Variable.DownAutoStep = 120;
                                }
                                break;
                            }
                        case 120://气缸下降
                            {
                                if (Variable.RunEnable == true)
                                {
                                    OutYOFF(34);
                                    OutYON(33);//下层内气缸下降
                                    OutYOFF(36);
                                    OutYON(35);//下层外气缸下降
                                    Thread.Sleep(6000);
                                    if (Variable.XStatus[36] && Variable.XStatus[40])
                                    {
                                        Thread.Sleep(5000);
                                        Variable.DownAutoStep = 130;
                                    }
                                    else
                                    {
                                        Down("X0", LogType.Message, "请检查X036或X040，" + "请检查下层1#或2#下降感应器异常，请确认！", "", 0, 0);
                                    }
                                }
                                break;
                            }
                        case 130:
                            {
                                if (Variable.RunEnable == true)
                                {
                                    OutYOFF(37);//侧定位气缸回
                                    Thread.Sleep(3000);
                                    if (Variable.XStatus[42])
                                    {
                                        Variable.DownAutoStep = 131; AlarmTime = 0;
                                    }
                                    else
                                    {
                                        AlarmTime++;
                                        if (AlarmTime >= Convert.ToInt32(Variable.time_cylinder))
                                        {
                                            AlarmTime = 0;
                                            Down("X0", LogType.Message, "请检查X042，" + "请检查下层X042侧定位回感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                }
                                break;
                            }

                        case 131:
                            {
                                if (Variable.RunEnable == true)
                                {
                                    OutYON(38);//推tray出
                                    Thread.Sleep(3000);
                                    if (Variable.XStatus[45])
                                    {
                                        AlarmTime = 0;
                                        Variable.DownAutoStep = 132;

                                    }
                                    else
                                    {
                                        AlarmTime++;
                                        if (AlarmTime >= Convert.ToInt32(Variable.time_cylinder))
                                        {
                                            AlarmTime = 0;
                                            Down("X0", LogType.Message, "请检查X045，" + "请检查下层X045推tray出感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                }
                                break;
                            }

                        case 132://侧定位气缸回
                            {
                                if (Variable.RunEnable == true)
                                {
                                    OutYOFF(38);//推tray回   
                                    Thread.Sleep(3000);
                                    if (Variable.XStatus[44])
                                    {
                                        Variable.DownAutoStep = 140; AlarmTime = 0;
                                    }
                                    else
                                    {
                                        AlarmTime++;
                                        if (AlarmTime >= Convert.ToInt32(Variable.time_cylinder))
                                        {
                                            AlarmTime = 0;
                                            Down("X0", LogType.Message, "请检查X044，" + "请检查下层X044推tray回感应器异常，请确认！", "", 0, 0);
                                        }
                                    }
                                }
                                break;
                            }

                        case 140://测试完成
                            {
                                if (Variable.RunEnable == true)
                                {
                                    statelab.Text = "下层测试完成";
                                    // Down("X0", LogType.Message, "测试完成，请确认！", "", 0, 0);
                                    Variable.DownAutoStep = 10;
                                    beloew = false;
                                }
                                break;
                            }
                    }
                }
                //Thread.Sleep(10);
            }
        }
        #endregion

        #region 下层内自动流程
        void DownAutoThreadStep1()
        {
            while (true)
            {
                if (lower_front)
                {
                    switch (Variable.DownAutoStep1)
                    {
                        case 10://等待读取Test,开始测试
                            {
                                if (Variable.Down1check)
                                {

                                    dataGrid.IniLeftLoadTrayW(GridD1, 8, 19);
                                    D1_StartTime.Text = "0";
                                    D1_EndTime.Text = "0";
                                    D1_TestTime.Text = "0";
                                    Variable.DownTotalNum1 = 0;
                                    Variable.DownPassNum1 = 0;
                                    Variable.DownFailNum1 = 0;
                                    Variable.DownYield1 = 0;

                                    //下层清空
                                    ClearArry(Variable.receiveDownState1);
                                    ClearArry(Variable.receiveDownVCCVolt1);
                                    ClearArry(Variable.receiveDownVCQVolt1);
                                    ClearArry(Variable.receiveDownVCCElec1);
                                    ClearArry(Variable.receiveDownVCQElec1);
                                    downCount1 = 0;
                                    downCounttime1 = 0;
                                    for (int i = 0; i < 10; i++)
                                    {
                                        Variable.receiveDownVCC1[i] = "";
                                        Variable.receiveDownVCQ1[i] = "";
                                    }

                                    D1_StartTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//获取当时时间
                                    //Thread.Sleep(Convert.ToInt16(testWaitTime.Text));//测试等待                                    
                                    Variable.DownAutoStep1 = 20; //开始测试 
                                }

                                break;
                            }
                        case 20://VCQ写入判断
                            {
                                if (Variable.VCQCheck == true)
                                {
                                    D1_VC.Text = "VCQ";
                                    Variable.DownAutoStep1 = 30;
                                }
                                else
                                {
                                    D1_VC.Text = "VCC";
                                    Variable.DownAutoStep1 = 50;
                                }
                                break;
                            }
                        case 30://写入VCQ电压
                            {
                                string str = "";
                                string strSend = "";
                                string model = "";
                                if ((Convert.ToDouble(Variable.VCQVolSet) * 10) > 36 || (Convert.ToDouble(Variable.VCQVolSet) * 10) < 18)
                                {
                                    MessageBox.Show("电压设定超出范围");
                                }
                                else
                                {
                                    string Vol = Convert.ToInt32((Convert.ToDouble(Variable.VCQVolSet) * 10)).ToString("X2");

                                    model = "31";
                                    str = "A55A0300AFF" + model + Vol;
                                    string str1 = BCC(str);
                                    strSend = str + str1 + "FF";
                                }

                                //发送数据
                                DownSendDataVCQ1(strSend);
                                Variable.DownAutoStep1 = 40;
                                break;
                            }
                        case 40://写入VCQ电压判断
                            {
                                bool flag = false;
                                for (int i = 0; i < 10; i++)
                                {
                                    if (Variable.receiveDownVCQ1[i] != "" && Variable.receiveDownVCQ1[i] != null)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                        break;
                                    }
                                }

                                if (flag)
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        Variable.receiveDownVCQ1[i] = "";
                                    }

                                    D1_VC.Text = "VCC";

                                    downCount1 = 0;
                                    downCounttime1 = 0;
                                    Variable.DownAutoStep1 = 50;
                                }
                                else
                                {
                                    downCount1 += 1;
                                    downCounttime1 += 1;
                                    Variable.DownAutoStep1 = 30;
                                    if (downCount1 > 3)
                                    {
                                        //MessageBox.Show("下层电压设定写入失败");
                                        downCount1 = 0;
                                    }
                                    if (downCounttime1 > 1000)
                                    {
                                        string[] str = new string[1] { "1" };
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm1");
                                        if (MessageBox.Show("下层VCQ电压设定写入失败?选择'是'重新写入，选择'否'放弃写入！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            str[0] = "0";
                                            myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm1");
                                            downCounttime1 = 0;
                                        }
                                        else
                                        {
                                            str[0] = "0";
                                            myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm1");
                                            downCounttime1 = 0;
                                            Variable.DownAutoStep1 = 50;
                                        }
                                    }
                                }
                                break;
                            }
                        case 50://写入VCC电压
                            {
                                string str = "";
                                string strSend = "";
                                string model = "";
                                if ((Convert.ToDouble(Variable.VCCVolSet) * 10) > 36 || (Convert.ToDouble(Variable.VCCVolSet) * 10) < 18)
                                {
                                    MessageBox.Show("电压设定超出范围");
                                }
                                else
                                {
                                    string Vol = Convert.ToInt32((Convert.ToDouble(Variable.VCCVolSet) * 10)).ToString("X2");

                                    model = "30";
                                    str = "A55A0400FF" + model + Vol;
                                    string str1 = BCC(str);
                                    strSend = str + str1 + "FF";
                                }

                                //发送数据
                                DownSendDataVCC1(strSend);
                                Variable.DownAutoStep1 = 60;
                                break;
                            }
                        case 60://写入VCC电压判断
                            {
                                bool flag = false;
                                for (int i = 0; i < 10; i++)
                                {
                                    //if(true)
                                    if (Variable.receiveDownVCC1[i] != "" && Variable.receiveDownVCC1[i] != null)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                        break;
                                    }
                                }

                                if (flag)
                                {
                                    downCount1 = 0;
                                    downCounttime1 = 0;
                                    Variable.DownAutoStep1 = 70;
                                }
                                else
                                {
                                    downCount1 += 1;
                                    downCounttime1 += 1;
                                    Variable.DownAutoStep1 = 50;
                                    if (downCount1 > 3)
                                    {
                                        //MessageBox.Show("下层电压设定写入失败");
                                        downCount1 = 0;
                                    }
                                    if (downCounttime1 > 1000)
                                    {
                                        string[] str = new string[1] { "2" };
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm1");
                                        if (MessageBox.Show("下层VCC电压设定写入失败?选择'是'重新写入，选择'否'放弃写入！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            str[0] = "0";
                                            myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm1");
                                            downCounttime1 = 0;
                                        }
                                        else
                                        {
                                            str[0] = "0";
                                            myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm1");
                                            downCounttime1 = 0;
                                            Variable.DownAutoStep1 = 70;
                                        }
                                    }
                                }
                                break;
                            }
                        //UDP通信
                        case 70://读取状态
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    string str = "A5 5A 03 00 FF 22 23 FF";// 状态查询

                                    //发送数据
                                    DownSendData1(str);
                                    Variable.DownAutoStep1 = 80;
                                }
                                break;
                            }

                        case 80://读取电流
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    string str = "A5 5A 03 00 FF 21 22 FF";// 电流查询

                                    //发送数据
                                    DownSendData1(str);
                                    Variable.DownAutoStep1 = 90;
                                }
                                break;
                            }
                        case 90://读取电压判断
                            {
                                if (Variable.VCQCheck == true)
                                {
                                    Variable.DownAutoStep1 = 100;
                                }
                                else
                                {
                                    Variable.DownAutoStep1 = 110;
                                }
                                break;
                            }
                        case 100://读取VCQ电压
                            {
                                string str = "";
                                str = "A5 5A 03 00 FF 20 21 FF";// VCQ电压查询
                                //发送数据
                                DownSendData1(str);

                                Variable.DownAutoStep1 = 110;
                                break;
                            }
                        case 110://读取VCC电压
                            {
                                string str = "";
                                str = "A5 5A 03 00 FF 20 21 FF";// VCC电压查询
                                //发送数据
                                DownSendData1(str);

                                Variable.DownAutoStep1 = 120;
                                break;
                            }
                        case 120://测试时间判断
                            {
                                try
                                {
                                    if (Variable.testTimeReadCheck)//模拟测试时间
                                    {
                                        if (Convert.ToInt64(D1_TestTime.Text) >= 100)
                                        {
                                            //将其余没有测试好的赋超时值
                                            for (int i = 0; i < 19; i++)
                                            {
                                                for (int j = 0; j < 8; j++)
                                                {
                                                    if (Variable.receiveDownState1[i * 8 + j] != "00")
                                                    {
                                                        Variable.receiveDownState1[i * 8 + j] = "02";
                                                    }
                                                }
                                            }
                                            Variable.DownAutoStep1 = 140;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt64(D1_TestTime.Text) <= 100)
                                            {
                                                Variable.DownAutoStep1 = 70;
                                            }
                                            else
                                            {
                                                Variable.DownAutoStep1 = 130;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt64(D1_TestTime.Text) >= Convert.ToInt64(Variable.testTime))
                                        {

                                            //将其余没有测试好的赋超时值
                                            //for (int i = 0; i < 19; i++)
                                            //{
                                            //    for (int j = 0; j < 8; j++)
                                            //    {
                                            //        if (Variable.receiveDownState1[i * 8 + j] != "00")
                                            //        {
                                            //            Variable.receiveDownState1[i * 8 + j] = "02";
                                            //        }
                                            //    }
                                            //}
                                            Variable.DownAutoStep1 = 140;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt64(D1_TestTime.Text) <= Convert.ToInt64(Variable.testWaitTime))
                                            {
                                                Variable.DownAutoStep1 = 70;
                                            }
                                            else
                                            {
                                                Variable.DownAutoStep1 = 130;
                                            }
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                break;
                            }

                        case 130://判断是否都测试完成
                            {
                                bool b = true;//测试完成
                                for (int i = 0; i < 19; i++)
                                {
                                    if (b)
                                    {
                                        for (int j = 0; j < 8; j++)
                                        {
                                            if (b)
                                            {
                                                if (Variable.receiveDownState1[i * 8 + j] != null)//不为空
                                                {
                                                    if (Variable.receiveDownState1[i * 8 + j] == "01")
                                                    {
                                                        b = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        b = true;//测试完成
                                                    }
                                                }
                                                else//为空
                                                {
                                                    b = false;
                                                    break;
                                                }

                                            }
                                        }
                                    }
                                }
                                if (b)
                                {
                                    Variable.DownAutoStep1 = 140;
                                }
                                else
                                {
                                    Variable.DownAutoStep1 = 70;
                                }

                                break;
                            }

                        case 140://测试完成
                            {
                                GridDownCount1(Variable.receiveDownState1);
                                //下层总数良率计算
                                Count();
                                D1_EndTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                Variable.DownAutoStep1 = 150;

                                break;
                            }

                        case 150://发送Tray数据
                            {

                                myTXT.WriteTxt(Variable.receiveDownState1, Application.StartupPath + "\\Map\\Downtray1");
                                Variable.DownAutoStep1 = 160;

                                break;
                            }

                        case 160://删除Test文件
                            {

                                Thread.Sleep(500);
                                downNum1 += 1;
                                DownTotalToRecord1();//将下层内总数量保存到数据库
                                DownYieldToRecord1();//将下层内良率保存到数据库
                                string path1 = Application.StartupPath + "\\Map\\DownNum1";
                                string[] str = new string[1];
                                str[0] = downNum1.ToString();
                                myTXT.WriteTxtVariable(str, path1);
                                Variable.DownAutoStep1 = 200;

                                break;
                            }
                    }
                }

                Thread.Sleep(10);
            }
        }
        #endregion

        #region 下层外自动流程
        void DownAutoThreadStep2()
        {
            while (true)
            {
                if (lower_back)
                {
                    switch (Variable.DownAutoStep2)
                    {
                        case 10://等待读取Test,开始测试
                            {
                                if (Variable.Down2check)
                                {
                                    dataGrid.IniLeftLoadTrayW(GridD2, 8, 19);
                                    D2_StartTime.Text = "0";
                                    D2_EndTime.Text = "0";
                                    D2_TestTime.Text = "0";
                                    Variable.DownTotalNum2 = 0;
                                    Variable.DownPassNum2 = 0;
                                    Variable.DownFailNum2 = 0;
                                    Variable.DownYield2 = 0;

                                    //下层清空
                                    ClearArry(Variable.receiveDownState2);
                                    ClearArry(Variable.receiveDownVCCVolt2);
                                    ClearArry(Variable.receiveDownVCQVolt2);
                                    ClearArry(Variable.receiveDownVCCElec2);
                                    ClearArry(Variable.receiveDownVCQElec2);
                                    downCount2 = 0;
                                    downCounttime2 = 0;
                                    for (int i = 0; i < 10; i++)
                                    {
                                        Variable.receiveDownVCC2[i] = "";
                                        Variable.receiveDownVCQ2[i] = "";
                                    }

                                    D2_StartTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//获取当时时间
                                    //Thread.Sleep(Convert.ToInt16(testWaitTime.Text));//测试等待                                    
                                    Variable.DownAutoStep2 = 20; //开始测试 
                                }


                                break;
                            }
                        case 20://VCQ写入判断
                            {
                                if (Variable.VCQCheck == true)
                                {
                                    D2_VC.Text = "VCQ";
                                    Variable.DownAutoStep2 = 30;
                                }
                                else
                                {
                                    D2_VC.Text = "VCC";
                                    Variable.DownAutoStep2 = 50;
                                }
                                break;
                            }
                        case 30://写入VCQ电压
                            {
                                string str = "";
                                string strSend = "";
                                string model = "";
                                if ((Convert.ToDouble(Variable.VCQVolSet) * 10) > 36 || (Convert.ToDouble(Variable.VCQVolSet) * 10) < 18)
                                {
                                    MessageBox.Show("电压设定超出范围");
                                }
                                else
                                {
                                    string Vol = Convert.ToInt32((Convert.ToDouble(Variable.VCQVolSet) * 10)).ToString("X2");

                                    model = "31";
                                    str = "A55A0300FF" + model + Vol;
                                    string str1 = BCC(str);
                                    strSend = str + str1 + "FF";
                                }
                                //发送数据
                                DownSendDataVCQ2(strSend);
                                Variable.DownAutoStep2 = 40;
                                break;
                            }
                        case 40://写入VCQ电压判断
                            {
                                bool flag = false;
                                for (int i = 0; i < 10; i++)
                                {
                                    if (Variable.receiveDownVCQ2[i] != "" && Variable.receiveDownVCQ2[i] != null)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                        break;
                                    }
                                }

                                if (flag)
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        Variable.receiveDownVCQ2[i] = "";
                                    }

                                    D2_VC.Text = "VCC";

                                    downCount2 = 0;
                                    downCounttime2 = 0;
                                    Variable.DownAutoStep2 = 50;
                                }
                                else
                                {
                                    downCount2 += 1;
                                    downCounttime2 += 1;
                                    Variable.DownAutoStep2 = 30;
                                    if (downCount2 > 3)
                                    {
                                        //MessageBox.Show("下层电压设定写入失败");
                                        downCount2 = 0;
                                    }
                                    if (downCounttime2 > 1000)
                                    {
                                        string[] str = new string[1] { "1" };
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm2");
                                        if (MessageBox.Show("下层VCQ电压设定写入失败?选择'是'重新写入，选择'否'放弃写入！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            str[0] = "0";
                                            myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm2");
                                            downCounttime2 = 0;
                                        }
                                        else
                                        {
                                            str[0] = "0";
                                            myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm2");
                                            downCounttime2 = 0;
                                            Variable.DownAutoStep2 = 50;
                                        }
                                    }
                                }

                                break;
                            }
                        case 50://写入VCC电压
                            {
                                string str = "";
                                string strSend = "";
                                string model = "";
                                if ((Convert.ToDouble(Variable.VCCVolSet) * 10) > 36 || (Convert.ToDouble(Variable.VCCVolSet) * 10) < 18)
                                {
                                    MessageBox.Show("电压设定超出范围");
                                }
                                else
                                {
                                    string Vol = Convert.ToInt32((Convert.ToDouble(Variable.VCCVolSet) * 10)).ToString("X2");

                                    model = "30";
                                    str = "A55A0400FF" + model + Vol;
                                    string str1 = BCC(str);
                                    strSend = str + str1 + "FF";
                                }

                                //发送数据
                                DownSendDataVCC2(strSend);
                                Variable.DownAutoStep2 = 60;
                                break;
                            }
                        case 60://写入VCC电压判断
                            {
                                bool flag = false;
                                for (int i = 0; i < 10; i++)
                                {
                                    if (Variable.receiveDownVCC2[i] != "" && Variable.receiveDownVCC2[i] != null)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    downCount2 = 0;
                                    downCounttime2 = 0;
                                    Variable.DownAutoStep2 = 70;
                                }
                                else
                                {
                                    downCount2 += 1;
                                    downCounttime2 += 1;
                                    Variable.DownAutoStep2 = 50;
                                    if (downCount2 > 3)
                                    {
                                        //MessageBox.Show("下层电压设定写入失败");
                                        downCount2 = 0;
                                    }
                                    if (downCounttime2 > 1000)
                                    {
                                        string[] str = new string[1] { "2" };
                                        myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm2");
                                        if (MessageBox.Show("下层VCC电压设定写入失败?选择'是'重新写入，选择'否'放弃写入！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            str[0] = "0";
                                            myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm2");
                                            downCounttime2 = 0;
                                        }
                                        else
                                        {
                                            str[0] = "0";
                                            myTXT.WriteTxt(str, Application.StartupPath + "\\Map\\DownAlarm2");
                                            downCounttime2 = 0;
                                            Variable.DownAutoStep2 = 70;
                                        }
                                    }
                                }

                                break;
                            }
                        //UDP通信
                        case 70://读取状态
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    string str = "A5 5A 03 00 FF 22 23 FF";// 状态查询

                                    //发送数据
                                    DownSendData2(str);
                                    Variable.DownAutoStep2 = 80;
                                }

                                break;
                            }

                        case 80://读取电流
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    string str = "A5 5A 03 00 FF 21 22 FF";// 电流查询

                                    //发送数据
                                    DownSendData2(str);
                                    Variable.DownAutoStep2 = 90;
                                }
                                break;
                            }
                        case 90://读取电压判断
                            {
                                if (Variable.VCQCheck == true)
                                {
                                    Variable.DownAutoStep2 = 100;
                                }
                                else
                                {
                                    Variable.DownAutoStep2 = 110;
                                }
                                break;
                            }
                        case 100://读取VCQ电压
                            {
                                string str = "";
                                str = "A5 5A 03 00 FF 20 21 FF";// VCQ电压查询
                                //发送数据
                                DownSendData2(str);

                                Variable.DownAutoStep2 = 110;

                                break;
                            }
                        case 110://读取VCC电压
                            {
                                string str = "";
                                str = "A5 5A 03 00 FF 20 21 FF";// VCC电压查询
                                //发送数据
                                DownSendData2(str);

                                Variable.DownAutoStep2 = 120;
                                break;
                            }
                        case 120://测试时间判断
                            {
                                try
                                {
                                    if (Variable.testTimeReadCheck)//模拟测试时间
                                    {
                                        if (Convert.ToInt64(D2_TestTime.Text) >= 100)
                                        {
                                            //将其余没有测试好的赋超时值
                                            for (int i = 0; i < 19; i++)
                                            {
                                                for (int j = 0; j < 8; j++)
                                                {
                                                    if (Variable.receiveDownState2[i * 8 + j] != "00")
                                                    {
                                                        Variable.receiveDownState2[i * 8 + j] = "02";
                                                    }
                                                }
                                            }
                                            Variable.DownAutoStep2 = 140;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt64(D2_TestTime.Text) <= 100)
                                            {
                                                Variable.DownAutoStep2 = 70;
                                            }
                                            else
                                            {
                                                Variable.DownAutoStep2 = 130;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt64(D2_TestTime.Text) >= Convert.ToInt64(Variable.testTime))
                                        {
                                            //将其余没有测试好的赋超时值
                                            //for (int i = 0; i < 19; i++)
                                            //{
                                            //    for (int j = 0; j < 8; j++)
                                            //    {
                                            //        if (Variable.receiveDownState2[i * 8 + j] != "00")
                                            //        {
                                            //            Variable.receiveDownState2[i * 8 + j] = "02";
                                            //        }
                                            //    }
                                            //}
                                            Variable.DownAutoStep2 = 140;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt64(D2_TestTime.Text) <= Convert.ToInt64(Variable.testWaitTime))
                                            {
                                                Variable.DownAutoStep2 = 70;
                                            }
                                            else
                                            {
                                                Variable.DownAutoStep2 = 130;
                                            }
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                break;
                            }

                        case 130://判断是否都测试完成
                            {
                                bool b = true;//测试完成
                                for (int i = 0; i < 19; i++)
                                {
                                    if (b)
                                    {
                                        for (int j = 0; j < 8; j++)
                                        {
                                            if (b)
                                            {
                                                if (Variable.receiveDownState2[i * 8 + j] != null)//不为空
                                                {
                                                    if (Variable.receiveDownState2[i * 8 + j] == "01")
                                                    {
                                                        b = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        b = true;//测试完成
                                                    }
                                                }
                                                else//为空
                                                {
                                                    b = false;
                                                    break;
                                                }

                                            }
                                        }
                                    }
                                }
                                if (b)
                                {
                                    Variable.DownAutoStep2 = 140;
                                }
                                else
                                {
                                    Variable.DownAutoStep2 = 70;
                                }
                                break;
                            }

                        case 140://测试完成
                            {
                                GridDownCount2(Variable.receiveDownState2);
                                //下层总数良率计算
                                Count();
                                D2_EndTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                Variable.DownAutoStep2 = 150;
                                break;
                            }
                        case 150://发送Tray数据
                            {
                                myTXT.WriteTxt(Variable.receiveDownState2, Application.StartupPath + "\\Map\\Downtray2");
                                Variable.DownAutoStep2 = 160;
                                break;
                            }

                        case 160://删除Test文件
                            {
                                Thread.Sleep(500);
                                downNum2 += 1;
                                DownTotalToRecord2();//将下层外总数量保存到数据库
                                DownYieldToRecord2();//将下层外良率保存到数据库
                                string path1 = Application.StartupPath + "\\Map\\DownNum2";
                                string[] str = new string[1];
                                str[0] = downNum2.ToString();
                                myTXT.WriteTxtVariable(str, path1);
                                Variable.DownAutoStep2 = 200;
                                break;
                            }
                    }
                }
                Thread.Sleep(10);
            }
        }
        #endregion

        #region 测试计时
        private void Timer1()
        {
            while (true)
            {
                if (U1_StartTime.Text != "0" && U1_StartTime.Text != "" && Variable.UpAutoStep1 > 10 && Variable.UpAutoStep1 < 140)
                {
                    UpTimeShow1();
                }

                if (U2_StartTime.Text != "0" && U2_StartTime.Text != "" && Variable.UpAutoStep2 > 10 && Variable.UpAutoStep2 < 140)
                {
                    UpTimeShow2();
                }

                if (D1_StartTime.Text != "0" && D1_StartTime.Text != "" && Variable.DownAutoStep1 > 10 && Variable.DownAutoStep1 < 140)
                {
                    DownTimeShow1();
                }

                if (D2_StartTime.Text != "0" && D2_StartTime.Text != "" && Variable.DownAutoStep2 > 10 && Variable.DownAutoStep2 < 140)
                {
                    DownTimeShow2();
                }

                Thread.Sleep(1000);
            }
        }

        public void UpTimeShow1()
        {
            try
            {
                //上层显示时间
                string upTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//现在时间
                if (upTime != "" && U1_StartTime.Text != "0")
                {
                    DateTime t1 = DateTime.Parse(upTime);//将现在时间转换为固定时间模式
                    DateTime t2 = DateTime.Parse(U1_StartTime.Text);//将开始时间转换为固定时间模式
                    TimeSpan t3 = t1 - t2;//时间差
                    U1_TestTime.Text = t3.TotalSeconds.ToString();
                }
            }
            catch
            {

            }
        }
        public void UpTimeShow2()
        {
            try
            {
                //上层显示时间
                string upTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//现在时间
                if (upTime != "" && U2_StartTime.Text != "0")
                {
                    DateTime t1 = DateTime.Parse(upTime);//将现在时间转换为固定时间模式
                    DateTime t2 = DateTime.Parse(U2_StartTime.Text);//将开始时间转换为固定时间模式
                    TimeSpan t3 = t1 - t2;//时间差
                    U2_TestTime.Text = t3.TotalSeconds.ToString();
                }
            }
            catch
            {

            }
        }
        public void DownTimeShow1()
        {
            try
            {
                //下层显示时间
                string downTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//现在时间
                if (downTime != "" && D1_StartTime.Text != "0")
                {
                    DateTime t1 = DateTime.Parse(downTime);//将现在时间转换为固定时间模式
                    DateTime t2 = DateTime.Parse(D1_StartTime.Text);//将开始时间转换为固定时间模式
                    TimeSpan t3 = t1 - t2;//时间差
                    D1_TestTime.Text = t3.TotalSeconds.ToString();
                }
            }
            catch
            {

            }
        }
        public void DownTimeShow2()
        {
            try
            {
                //下层显示时间
                string downTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//现在时间
                if (downTime != "" && D2_StartTime.Text != "0")
                {
                    DateTime t1 = DateTime.Parse(downTime);//将现在时间转换为固定时间模式
                    DateTime t2 = DateTime.Parse(D2_StartTime.Text);//将开始时间转换为固定时间模式
                    TimeSpan t3 = t1 - t2;//时间差
                    D2_TestTime.Text = t3.TotalSeconds.ToString();
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 循环扫描
        void CycleScan()
        {
            while (true)
            {
                #region 界面刷新
                try
                {
                    //上层状态更新
                    GridColorRefresh(GridU1, Variable.receiveUPState1);
                    GridColorRefresh(GridU2, Variable.receiveUPState2);

                    //下层状态更新
                    GridColorRefresh(GridD1, Variable.receiveDownState1);
                    GridColorRefresh(GridD2, Variable.receiveDownState2);
                }
                catch
                {
                }
                #endregion

                #region 测试板链接状态显示

                for (int i = 0; i < 10; i++)
                {
                    if ((Variable.receiveUPVCC1[i] != "" && Variable.receiveUPVCC1[i] != null) || (Variable.receiveUPVCQ1[i] != "" && Variable.receiveUPVCQ1[i] != null))
                    {
                        VolU_1.Rows[0].Cells[i].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        VolU_1.Rows[0].Cells[i].Style.BackColor = Color.Red;
                    }

                    if ((Variable.receiveUPVCC2[i] != "" && Variable.receiveUPVCC2[i] != null) || (Variable.receiveUPVCQ2[i] != "" && Variable.receiveUPVCQ2[i] != null))
                    {
                        VolU_2.Rows[0].Cells[i].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        VolU_2.Rows[0].Cells[i].Style.BackColor = Color.Red;
                    }

                    if ((Variable.receiveDownVCC1[i] != "" && Variable.receiveDownVCC1[i] != null) || (Variable.receiveDownVCQ1[i] != "" && Variable.receiveDownVCQ1[i] != null))
                    {
                        VolD_1.Rows[0].Cells[i].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        VolD_1.Rows[0].Cells[i].Style.BackColor = Color.Red;
                    }

                    if ((Variable.receiveDownVCC2[i] != "" && Variable.receiveDownVCC2[i] != null) || (Variable.receiveDownVCQ2[i] != "" && Variable.receiveDownVCQ2[i] != null))
                    {
                        VolD_2.Rows[0].Cells[i].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        VolD_2.Rows[0].Cells[i].Style.BackColor = Color.Red;
                    }
                }

                #endregion

                //BatchNum.Text = Variable.BatchNum;//批号
                //  RecordName.Text = Variable.RecordName;//档案
                Total.Text = Variable.TotalNum.ToString();//总数
                Pass.Text = Variable.PassNum.ToString();//良品数
                if (Variable.XStatus[4])
                {
                    button6_Click(null, null);
                }

                if (Variable.XStatus[5])
                {
                    Variable.MachineState = Variable.MachineStatus.Stop;
                    if (Variable.MachineState == Variable.MachineStatus.Stop || Variable.MachineState == Variable.MachineStatus.Pause || Variable.MachineState == Variable.MachineStatus.Emg)
                    {
                        Variable.RunEnable = false;
                        D1_StartTime.Text = "0";
                        D1_EndTime.Text = "0";
                        D1_TestTime.Text = "0";
                        D2_StartTime.Text = "0";
                        D2_EndTime.Text = "0";
                        D2_TestTime.Text = "0";

                        Variable.DownTotalNum1 = 0;
                        Variable.DownTotalNum2 = 0;
                        Variable.DownPassNum1 = 0;
                        Variable.DownPassNum2 = 0;
                        Variable.DownFailNum1 = 0;
                        Variable.DownFailNum2 = 0;
                        Variable.DownYield1 = 0;
                        Variable.DownYield2 = 0;
                        lower_front = false;//下前
                        lower_back = false;//下后
                        beloew = false;//下层
                        OutYOFF(39);//1#断电
                        OutYOFF(40);//2#断电
                        Delay(1);
                        OutYOFF(34);
                        OutYON(33);//1#压合气缸下降
                        Thread.Sleep(2000);
                        OutYOFF(33);
                        OutYOFF(36);
                        OutYON(35);//2#压合气缸下降
                        Thread.Sleep(2000);
                        OutYOFF(35);
                        OutYOFF(37);//侧定位回
                        OutYOFF(38);//推tray气缸回                

                        Variable.DownAutoStep = 10;
                        Variable.DownAutoStep1 = 10;
                        Variable.DownAutoStep2 = 10;
                        parameterForm.LoadParameter(Application.StartupPath + "\\Parameter.ini");
                        Variable.MachineState = Variable.MachineStatus.Pause;
                    }
                    if (Variable.MachineState == Variable.MachineStatus.Stop || Variable.MachineState == Variable.MachineStatus.Pause || Variable.MachineState == Variable.MachineStatus.Emg)
                    {
                        Variable.RunEnable = false;
                        U1_StartTime.Text = "0";
                        U1_EndTime.Text = "0";
                        U1_TestTime.Text = "0";
                        U2_StartTime.Text = "0";
                        U2_EndTime.Text = "0";
                        U2_TestTime.Text = "0";

                        Variable.UpTotalNum1 = 0;
                        Variable.UpTotalNum2 = 0;
                        Variable.UpPassNum1 = 0;
                        Variable.UpPassNum2 = 0;
                        Variable.UpFailNum1 = 0;
                        Variable.UpFailNum2 = 0;
                        Variable.UpYield1 = 0;
                        Variable.UpYield2 = 0;
                        come_up = false;//上前
                        upper_back = false;//上后
                        OutYOFF(23);//1#断电
                        OutYOFF(24);//2#断电
                        Delay(1);
                        OutYOFF(18);//
                        OutYON(17);//1#压合气缸下降
                        Thread.Sleep(2000);
                        OutYOFF(17);//

                        OutYOFF(20);//                
                        OutYON(19);//2#压合气缸下降
                        Thread.Sleep(2000);
                        OutYOFF(19);//
                        OutYOFF(21);//侧定位回
                        OutYOFF(22);//推tray气缸回                

                        Variable.UpAutoStep = 10;
                        Variable.UpAutoStep1 = 10;
                        Variable.UpAutoStep2 = 10;
                        above = false;//上层
                        parameterForm.LoadParameter(Application.StartupPath + "\\Parameter.ini");
                        Variable.MachineState = Variable.MachineStatus.Pause;
                    }
                }







                if (!Variable.XStatus[0])//急停
                {
                    Variable.MachineState = Variable.MachineStatus.Emg;
                }
                if (!Variable.XStatus[3])//急停
                {
                    ccds += 1;
                    if (ccds > 1000)
                    {
                        ccds = 0;
                        if (Variable.MachineState != Variable.MachineStatus.Alarm)
                        {
                            Down("X0", LogType.Message, "气压报警，请确认!", "", 0, 0);
                        }

                    }
                }
                if (Variable.HotModel)
                {
                    //if (Variable.Up1check)
                    //{
                    mc.GTN_SetExtDoBit(1, (short)(26), 1);//上层内
                    //}
                    //else { mc.GTN_SetExtDoBit(1, (short)(26), 0); }

                    //if (Variable.Up2check)
                    //{
                    mc.GTN_SetExtDoBit(1, (short)(27), 1);//上层外
                    //}
                    //else { mc.GTN_SetExtDoBit(1, (short)(27), 0); }

                    //if (Variable.Down1check)
                    //{
                    mc.GTN_SetExtDoBit(1, (short)(42), 1);//下层内
                    //}
                    //else { mc.GTN_SetExtDoBit(1, (short)(42), 0); }

                    //if (Variable.Down2check)
                    //{
                    mc.GTN_SetExtDoBit(1, (short)(43), 1);//下层外
                    //}
                    //else { mc.GTN_SetExtDoBit(1, (short)(43), 0); }
                }
                else
                {
                    mc.GTN_SetExtDoBit(1, (short)(26), 0);//扩展模块
                    mc.GTN_SetExtDoBit(1, (short)(27), 0);
                    mc.GTN_SetExtDoBit(1, (short)(42), 0);//扩展模块
                    mc.GTN_SetExtDoBit(1, (short)(43), 0);
                }


                Thread.Sleep(10);
            }
        }
        #endregion

        #region 数据处理线程
        void DataThreadStep()
        {
            while (true)
            {
                //UDP通信
                DataDispose(Variable.receiveMessage);

                Thread.Sleep(1);
            }
        }
        #endregion

        #region UDP数据处理
        public void DataDispose(string str)
        {
            string port = "";
            string mod = "";

            if (str != "" && str != null)
            {
                string[] portData = str.Split(':');
                string[] portNum = portData[0].Split('.');
                port = (Convert.ToInt16(portNum[3]) - 40).ToString();

                //string port = port1.TrimStart('0');//去掉前面的0
                mod = portData[2].Substring(10, 2);
                switch (mod)
                {
                    case "20"://读取电压参数192.168.1.10:1000:A55A23000A2021212121212121212121212121212122121212121212121212121212121212127DFF
                        {
                            //VCC
                            string s = portData[2].Substring(12, 32);
                            string[] array = StringSplitToArray(s);
                            //上层内数据
                            if (Convert.ToInt16(port) <= 10)
                            {
                                int i = Convert.ToInt16(port) - 1;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveUPVCCVolt1, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveUPVCCVolt1, (9 - i) * 16);
                                }

                            }


                            //上层外数据

                            if (Convert.ToInt16(port) > 10 && Convert.ToInt16(port) <= 20)
                            {
                                int i = Convert.ToInt16(port) - 11;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveUPVCCVolt2, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveUPVCCVolt2, (9 - i) * 16);
                                }
                            }


                            //下层内数据

                            if (Convert.ToInt16(port) > 20 && Convert.ToInt16(port) <= 30)
                            {
                                int i = Convert.ToInt16(port) - 21;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveDownVCCVolt1, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveDownVCCVolt1, (9 - i) * 16);
                                }
                            }

                            //下层外数据

                            if (Convert.ToInt16(port) > 30 && Convert.ToInt16(port) <= 40)
                            {
                                int i = Convert.ToInt16(port) - 31;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveDownVCCVolt2, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveDownVCCVolt2, (9 - i) * 16);
                                }
                            }


                            //VCQ
                            //if (VCQCheck.IsChecked == true)
                            //{
                            string s1 = portData[2].Substring(44, 32);
                            string[] array1 = StringSplitToArray(s1);
                            //上层内数据
                            if (Convert.ToInt16(port) <= 10)
                            {
                                int i = Convert.ToInt16(port) - 1;
                                if (i < 9)
                                {
                                    array1.CopyTo(Variable.receiveUPVCQVolt1, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array1, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveUPVCQVolt1, (9 - i) * 16);
                                }
                            }


                            //上层外数据
                            if (Convert.ToInt16(port) > 10 && Convert.ToInt16(port) <= 20)
                            {
                                int i = Convert.ToInt16(port) - 11;
                                if (i < 9)
                                {
                                    array1.CopyTo(Variable.receiveUPVCQVolt2, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array1, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveUPVCQVolt2, (9 - i) * 16);
                                }
                            }

                            //下层内数据
                            if (Convert.ToInt16(port) > 20 && Convert.ToInt16(port) <= 30)
                            {
                                int i = Convert.ToInt16(port) - 21;
                                if (i < 9)
                                {
                                    array1.CopyTo(Variable.receiveDownVCQVolt1, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array1, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveDownVCQVolt1, (9 - i) * 16);
                                }
                            }

                            //下层外数据
                            if (Convert.ToInt16(port) > 30 && Convert.ToInt16(port) <= 40)
                            {
                                int i = Convert.ToInt16(port) - 31;
                                if (i < 9)
                                {
                                    array1.CopyTo(Variable.receiveDownVCQVolt2, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array1, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveDownVCQVolt2, (9 - i) * 16);
                                }
                            }


                            break;
                        }
                    case "21"://读取电流参数192.168.1.10:1000:A55A23000A2100000000000000000000000000000000000000000000000000000000000000004DFF
                        {
                            //VCC 
                            string s = portData[2].Substring(12, 64);
                            string[] array = StringSplitToArray1(s);
                            //上层内数据

                            if (Convert.ToInt16(port) <= 10)
                            {
                                int i = Convert.ToInt16(port) - 1;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveUPVCCElec1, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveUPVCCElec1, (9 - i) * 16);
                                }
                            }

                            //上层外数据

                            if (Convert.ToInt16(port) > 10 && Convert.ToInt16(port) <= 20)
                            {
                                int i = Convert.ToInt16(port) - 11;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveUPVCCElec2, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveUPVCCElec2, (9 - i) * 16);
                                }
                            }

                            //下层内数据
                            if (Convert.ToInt16(port) > 20 && Convert.ToInt16(port) <= 30)
                            {
                                int i = Convert.ToInt16(port) - 21;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveDownVCCElec1, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveDownVCCElec1, (9 - i) * 16);
                                }
                            }

                            //下层外数据
                            if (Convert.ToInt16(port) > 30 && Convert.ToInt16(port) <= 40)
                            {
                                int i = Convert.ToInt16(port) - 31;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveDownVCCElec2, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveDownVCCElec2, (9 - i) * 16);
                                }
                            }


                            //VCQ
                            if (Variable.VCQCheck == true)
                            {
                                string s1 = portData[2].Substring(44, 32);
                                string[] array1 = StringSplitToArray(s1);
                                //上层内数据
                                if (Convert.ToInt16(port) <= 10)
                                {
                                    int i = Convert.ToInt16(port) - 1;
                                    if (i < 9)
                                    {
                                        array1.CopyTo(Variable.receiveUPVCQElec1, (8 - i) * 16 + 8);
                                    }
                                    else
                                    {
                                        string[] data = new string[8];
                                        Array.Copy(array1, 8, data, 0, 8);
                                        data.CopyTo(Variable.receiveUPVCQElec1, (9 - i) * 16);
                                    }
                                }

                                //上层外数据
                                if (Convert.ToInt16(port) > 10 && Convert.ToInt16(port) <= 20)
                                {
                                    int i = Convert.ToInt16(port) - 11;
                                    if (i < 9)
                                    {
                                        array1.CopyTo(Variable.receiveUPVCQElec2, (8 - i) * 16 + 8);
                                    }
                                    else
                                    {
                                        string[] data = new string[8];
                                        Array.Copy(array1, 8, data, 0, 8);
                                        data.CopyTo(Variable.receiveUPVCQElec2, (9 - i) * 16);
                                    }
                                }

                                //下层内数据

                                if (Convert.ToInt16(port) > 20 && Convert.ToInt16(port) <= 30)
                                {
                                    int i = Convert.ToInt16(port) - 21;
                                    if (i < 9)
                                    {
                                        array1.CopyTo(Variable.receiveDownVCQElec1, (8 - i) * 16 + 8);
                                    }
                                    else
                                    {
                                        string[] data = new string[8];
                                        Array.Copy(array1, 8, data, 0, 8);
                                        data.CopyTo(Variable.receiveDownVCQElec1, (9 - i) * 16);
                                    }
                                }

                                //下层外数据
                                if (Convert.ToInt16(port) > 30 && Convert.ToInt16(port) <= 40)
                                {
                                    int i = Convert.ToInt16(port) - 31;
                                    if (i < 9)
                                    {
                                        array1.CopyTo(Variable.receiveDownVCQElec2, (8 - i) * 16 + 8);
                                    }
                                    else
                                    {
                                        string[] data = new string[8];
                                        Array.Copy(array1, 8, data, 0, 8);
                                        data.CopyTo(Variable.receiveDownVCQElec2, (9 - i) * 16);
                                    }
                                }

                            }

                            break;
                        }
                    case "22"://读取状态192.168.1.100:1000:A55A13000A22020202020202020202020202020202025EFF
                        {
                            string s = portData[2].Substring(12, 32);
                            string[] array = StringSplitToArray(s);
                            //Array.Reverse(array);//反转数据
                            //上层内数据

                            if (Convert.ToInt16(port) <= 10)
                            {
                                int i = Convert.ToInt16(port) - 1;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveUPState1, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveUPState1, (9 - i) * 16);
                                }
                            }

                            //上层外数据
                            if (Convert.ToInt16(port) > 10 && Convert.ToInt16(port) <= 20)
                            {
                                int i = Convert.ToInt16(port) - 11;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveUPState2, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveUPState2, (9 - i) * 16);
                                }
                            }

                            //下层内数据

                            if (Convert.ToInt16(port) > 20 && Convert.ToInt16(port) <= 30)
                            {
                                int i = Convert.ToInt16(port) - 21;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveDownState1, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveDownState1, (9 - i) * 16);
                                }
                            }

                            //下层外数据
                            if (Convert.ToInt16(port) > 30 && Convert.ToInt16(port) <= 40)
                            {
                                int i = Convert.ToInt16(port) - 31;
                                if (i < 9)
                                {
                                    array.CopyTo(Variable.receiveDownState2, (8 - i) * 16 + 8);
                                }
                                else
                                {
                                    string[] data = new string[8];
                                    Array.Copy(array, 8, data, 0, 8);
                                    data.CopyTo(Variable.receiveDownState2, (9 - i) * 16);
                                }
                            }

                            break;
                        }
                    case "30"://设置VCC192.168.1.10:1000:A55A04000A30003DFF
                        {
                            string array = portData[2].Substring(12, 2);
                            //上层内数据
                            if (Convert.ToInt16(port) <= 10)
                            {
                                int i = Convert.ToInt16(port) - 1;
                                // Variable.receiveUPVCC1[i] = array;
                            }

                            //上层外数据
                            if (Convert.ToInt16(port) > 10 && Convert.ToInt16(port) <= 20)
                            {
                                int i = Convert.ToInt16(port) - 11;
                                // Variable.receiveUPVCC2[i] = array;
                            }

                            //下层内数据
                            if (Convert.ToInt16(port) > 20 && Convert.ToInt16(port) <= 30)
                            {
                                int i = Convert.ToInt16(port) - 21;
                                Variable.receiveDownVCC1[i] = array;
                            }

                            //下层外数据
                            if (Convert.ToInt16(port) > 30 && Convert.ToInt16(port) <= 40)
                            {
                                int i = Convert.ToInt16(port) - 31;
                                Variable.receiveDownVCC2[i] = array;
                            }

                            break;
                        }
                    case "31"://设置VCQ192.168.1.10:1000:A55A04000A31003EFF
                        {
                            string array = portData[2].Substring(12, 2);
                            //上层内数据
                            if (Convert.ToInt16(port) <= 10)
                            {
                                int i = Convert.ToInt16(port) - 1;
                                // Variable.receiveUPVCQ1[i] = array;
                            }

                            //上层外数据
                            if (Convert.ToInt16(port) > 10 && Convert.ToInt16(port) <= 20)
                            {
                                int i = Convert.ToInt16(port) - 11;
                                //  Variable.receiveUPVCQ2[i] = array;
                            }

                            //下层内数据
                            if (Convert.ToInt16(port) > 20 && Convert.ToInt16(port) <= 30)
                            {
                                int i = Convert.ToInt16(port) - 21;
                                Variable.receiveDownVCQ1[i] = array;
                            }

                            //下层外数据
                            if (Convert.ToInt16(port) > 30 && Convert.ToInt16(port) <= 40)
                            {
                                int i = Convert.ToInt16(port) - 31;
                                Variable.receiveDownVCQ2[i] = array;
                            }

                            break;
                        }
                }
            }

        }
        #endregion

        #region **********温度读取显示线程**********

        public void Temp()
        {
            while (true)
            {
                if (!Variable.formOpenFlag)
                {
                    if (!Variable.temWriteFlag)
                    {
                        Variable.temReadFlag = true;

                        #region 温度读取
                        bool sc1 = false;
                        sc1 = Temperature.Open();
                        if (sc1)
                        {
                            //温度读取
                            Variable.TemperData[0] = TempRead(0.ToString("X2"));
                            //Variable.TemperData[0] = TempRead(1.ToString("X2"));
                            //labUp1.Text = Variable.TemperData[0].ToString();
                            //Variable.TemperData[1] = TempRead(2.ToString("X2"));
                            //labUp2.Text = Variable.TemperData[1].ToString();
                            //Variable.TemperData[2] = TempRead(3.ToString("X2"));
                            //labDown1.Text = Variable.TemperData[2].ToString();
                            //Variable.TemperData[3] = TempRead(4.ToString("X2"));
                            //labDown2.Text = Variable.TemperData[3].ToString();
                            // 实时更新Uptemper和Downtemper控件显示
                            //UpdateTemperatureDisplay();
                            // 检查温度超限并控制上层吹气
                            //CheckTemperatureAndControlFan();

                        }

                        #endregion
                        Variable.temReadFlag = false;
                    }
                }
                Thread.Sleep(10);
            }
        }
        #endregion

        #region 三色灯按钮灯扫描
        public void LampScan()
        {
            while (true)
            {
                #region 三色灯按钮灯
                //待机
                if (Variable.MachineState == Variable.MachineStatus.Stop)
                {
                    // Variable.MachineState = Variable.MachineStatus.Stop;
                    statelab.Text = "待机";
                    statelab.BackColor = Color.Yellow;
                    mc.GTN_SetExtDoBit(1, (short)(3), 0);//绿灯灭
                    mc.GTN_SetExtDoBit(1, (short)(2), 1);//黄灯亮
                    mc.GTN_SetExtDoBit(1, (short)(1), 0);//红灯灭
                    mc.GTN_SetExtDoBit(1, (short)(4), 0);//蜂鸣灭
                    mc.GTN_SetExtDoBit(1, (short)(5), 1);//停止亮
                    mc.GTN_SetExtDoBit(1, (short)(6), 1);//复位亮                                       
                }
                //开始
                else if (Variable.MachineState == Variable.MachineStatus.Running)
                {
                    statelab.Text = "运行";
                    statelab.BackColor = Color.Green;
                    mc.GTN_SetExtDoBit(1, (short)(3), 1);//绿灯亮
                    mc.GTN_SetExtDoBit(1, (short)(2), 0);//黄灯灭
                    mc.GTN_SetExtDoBit(1, (short)(1), 0);//红灯灭
                    mc.GTN_SetExtDoBit(1, (short)(4), 0);//蜂鸣灭
                    mc.GTN_SetExtDoBit(1, (short)(5), 1);//停止亮
                    mc.GTN_SetExtDoBit(1, (short)(6), 0);//复位灭
                }
                //复位
                else if (Variable.MachineState == Variable.MachineStatus.Reset)
                {
                    mc.GTN_SetExtDoBit(1, (short)(3), 0);//绿灯灭
                    mc.GTN_SetExtDoBit(1, (short)(2), 1);//黄灯亮
                    mc.GTN_SetExtDoBit(1, (short)(1), 0);//红灯灭
                    mc.GTN_SetExtDoBit(1, (short)(4), 0);//蜂鸣灭
                    mc.GTN_SetExtDoBit(1, (short)(5), 1);//停止亮
                    mc.GTN_SetExtDoBit(1, (short)(6), 0);//复位灭
                }
                //停止
                else if (Variable.MachineState == Variable.MachineStatus.Pause)
                {
                    statelab.Text = "停止";
                    statelab.BackColor = Color.Yellow;
                    mc.GTN_SetExtDoBit(1, (short)(3), 0);//绿灯灭
                    mc.GTN_SetExtDoBit(1, (short)(2), 1);//黄灯亮
                    mc.GTN_SetExtDoBit(1, (short)(1), 0);//红灯灭
                    mc.GTN_SetExtDoBit(1, (short)(4), 0);//蜂鸣灭
                    mc.GTN_SetExtDoBit(1, (short)(5), 1);//停止亮        
                    above = false;//上层
                    beloew = false;//下层
                }
                //报警
                else if (Variable.MachineState == Variable.MachineStatus.Alarm)
                {
                    statelab.Text = "报警";
                    statelab.BackColor = Color.Red;
                    mc.GTN_SetExtDoBit(1, (short)(3), 0);//绿灯灭
                    mc.GTN_SetExtDoBit(1, (short)(2), 0);//黄灯亮                                     
                    mc.GTN_SetExtDoBit(1, (short)(5), 1);//停止亮    
                    if (lightFlag == 0)
                    {
                        mc.GTN_SetExtDoBit(1, (short)(4), 0);//蜂鸣灭
                        mc.GTN_SetExtDoBit(1, (short)(1), 0);//红灯亮 
                        lightFlag = 1;
                    }
                    else
                    {
                        mc.GTN_SetExtDoBit(1, (short)(4), 1);//蜂鸣灭
                        mc.GTN_SetExtDoBit(1, (short)(1), 1);//红灯亮 
                        lightFlag = 0;
                    }

                }
                //急停
                else if (Variable.MachineState == Variable.MachineStatus.Emg)
                {
                    statelab.Text = "急停中";
                    statelab.BackColor = Color.Red;
                    mc.GTN_SetExtDoBit(1, (short)(3), 0);//绿灯灭
                    mc.GTN_SetExtDoBit(1, (short)(2), 0);//黄灯亮                                     
                    mc.GTN_SetExtDoBit(1, (short)(5), 1);//停止亮    
                    above = false;//上层
                    beloew = false;//下层
                    if (lightFlag == 0)
                    {
                        mc.GTN_SetExtDoBit(1, (short)(1), 0);//红灯亮 
                        mc.GTN_SetExtDoBit(1, (short)(4), 0);//蜂鸣灭
                        lightFlag = 1;
                    }
                    else
                    {
                        mc.GTN_SetExtDoBit(1, (short)(1), 1);//红灯亮 
                        mc.GTN_SetExtDoBit(1, (short)(4), 1);//蜂鸣灭
                        lightFlag = 0;
                    }
                }
                //归零完成
                else if (Variable.MachineState == Variable.MachineStatus.StandBy)
                {
                    statelab.Text = "归零OK";
                    statelab.BackColor = Color.Yellow;
                }
                //归零
                else if (Variable.MachineState == Variable.MachineStatus.zero)
                {
                    statelab.Text = "归零中";
                    statelab.BackColor = Color.Green;
                }

                #endregion
                if (Variable.che_door)//门禁
                {
                    if (Variable.MachineState == Variable.MachineStatus.Running)
                    {
                        if (!Variable.XStatus[2])//光栅
                        {
                            if (grating == 1)
                            {
                                mc.GTN_SetExtDoBit(1, (short)(1), 0);//红灯亮 
                                mc.GTN_SetExtDoBit(1, (short)(4), 0);//蜂鸣灭
                                grating = 0;
                            }
                            else
                            {
                                grating = 1;
                                mc.GTN_SetExtDoBit(1, (short)(1), 1);//红灯亮 
                                mc.GTN_SetExtDoBit(1, (short)(4), 1);//蜂鸣灭
                            }
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region 报警弹出

        public void Down(string X, Enum logType, string content, string step, int cancelStep, int sureStep)
        {
            if (!Variable.AlarmFlag)
            {
                if (logType.ToString() != "Operate")
                {
                    Thread.Sleep(200);
                    //Form POPFormIsOpenOrNot = Application.OpenForms["POPForm"];
                    //if ((POPFormIsOpenOrNot == null) || (POPFormIsOpenOrNot.IsDisposed))//如果没有创建过或者窗体已经被释放
                    //{
                    Variable.MachineState = Variable.MachineStatus.Alarm;
                    Variable.RunEnable = false;
                    POPForm pop = new POPForm();
                    pop.StartPosition = FormStartPosition.CenterScreen;
                    //pop.Show();
                    pop.pictureBox.Image = Image.FromFile(Application.StartupPath + "\\ico\\" + X + ".jpg");
                    pop.LabelX1.Text = content;
                    Variable.AlarmFlag = true;

                    Variable.step = step;
                    Variable.cancelStep = cancelStep;
                    Variable.sureStep = sureStep;

                    Variable.POPFlag = true;
                    pop.timerScan.Enabled = true;
                    pop.ShowDialog(this);

                    //}
                    //else
                    //{
                    //    POPFormIsOpenOrNot.Activate();
                    //    POPFormIsOpenOrNot.WindowState = FormWindowState.Normal;
                    //}
                }

                //写入数据库
                access.RecordAccess(logType, content);
            }
        }

        #endregion

        #region 温度显示更新方法
        /// <summary>
        /// 更新温度显示文本
        /// </summary>
        private void UpdateTemperatureDisplay()
        {
            try
            {
                // 上层温度显示（取上层内外温度的最高值）
                double upTemp = Math.Max(Variable.TemperData[0], Variable.TemperData[1]);
                string upTempText = upTemp.ToString("F1") + "°C";

                // 更新控件文本
                if (Uptemper.InvokeRequired)
                {
                    Uptemper.Invoke(new Action(() =>
                    {
                        Uptemper.Text = upTempText;
                    }));
                }
                else
                {
                    Uptemper.Text = upTempText;
                }
            }
            catch
            {
                // 忽略显示更新错误
            }
        }
        #endregion

        #region 温度超限检测和控制吹气
        /// <summary>
        /// 检查温度超限并控制上层吹气
        /// </summary>
        private void CheckTemperatureAndControlFan()
        {
            try
            {
                // 获取上层温度
                double upTemp = Variable.TemperData[0];

                // 检查上层温度是否超过设定值
                if (upTemp > Variable.temper)
                {
                    // 温度超限，启动上层吹气
                    OutYON(27);

                }
                else
                {
                    // 温度正常，关闭上层吹气
                    OutYOFF(27);
                }
            }
            catch
            {
            }
        }
        #endregion

        #region 温控器温度读取
        public double TempRead(string num)//string s = TempRead("01");
        {
            string str = "";
            double d = 0;
            //Variable.Temp = "00";
            //地址:01,02,03,04,05,06,07,08,09,0A,0B,0C,0D,0E,0F,10,11,12,13.14
            //string str = num + "0347000002";//"010347000002"
            //string str = num + "0310000002";//"010310000002"
            str = "010302680001";
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
            Thread.Sleep(500);
            try
            {
                if (Variable.Temp != "0")
                {
                    d = Math.Round(Convert.ToDouble(Variable.Temp) / 10, 1);
                }
                else
                {
                    d = Convert.ToDouble(Variable.Temp);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(new StackTrace(new StackFrame(true)), new StackFrame(), ex);
            }

            return d;
        }
        #endregion

        #region 将字符串分割为两位数组

        public string[] StringSplitToArray(string str)
        {
            string[] b = new string[str.Length / 2];
            for (int i = 0; i < str.Length / 2; i++)
            {
                b[i] = str.Substring(i * 2, 2);
            }
            return b;
        }
        public string[] StringSplitToArray1(string str)
        {
            string[] b = new string[str.Length / 4];
            for (int i = 0; i < str.Length / 4; i++)
            {
                b[i] = str.Substring(i * 4, 4);
            }
            return b;
        }
        #endregion

        #region 总数良率计算
        public void Count()
        {
            //总数良率计算
            Variable.TotalNum = Convert.ToInt32(Variable.PassNum + Variable.FailNum);
            Variable.UpTotalNum1 = Convert.ToInt32(Variable.UpPassNum1 + Variable.UpFailNum1);
            Variable.UpTotalNum2 = Convert.ToInt32(Variable.UpPassNum2 + Variable.UpFailNum2);
            Variable.DownTotalNum1 = Convert.ToInt32(Variable.DownPassNum1 + Variable.DownFailNum1);
            Variable.DownTotalNum2 = Convert.ToInt32(Variable.DownPassNum2 + Variable.DownFailNum2);

            if (Variable.TotalNum != 0)
            {
                Variable.Yield = Convert.ToDouble(Variable.PassNum / Variable.TotalNum);
            }
            if (Variable.UpTotalNum1 != 0)
            {
                Variable.UpYield1 = Variable.UpPassNum1 / Variable.UpTotalNum1;
            }
            if (Variable.UpTotalNum2 != 0)
            {
                Variable.UpYield2 = Variable.UpPassNum2 / Variable.UpTotalNum2;
            }
            if (Variable.DownTotalNum1 != 0)
            {
                Variable.DownYield1 = Variable.DownPassNum1 / Variable.DownTotalNum1;
            }
            if (Variable.DownTotalNum2 != 0)
            {
                Variable.DownYield2 = Variable.DownPassNum2 / Variable.DownTotalNum2;
            }

            Total.Text = Convert.ToInt32(Variable.TotalNum).ToString();
            U1_Total.Text = Convert.ToInt32(Variable.UpTotalNum1).ToString();
            U2_Total.Text = Convert.ToInt32(Variable.UpTotalNum2).ToString();
            D1_Total.Text = Convert.ToInt32(Variable.DownTotalNum1).ToString();
            D2_Total.Text = Convert.ToInt32(Variable.DownTotalNum2).ToString();

            Pass.Text = Convert.ToInt32(Variable.PassNum).ToString();
            U1_Pass.Text = Convert.ToInt32(Variable.UpPassNum1).ToString();
            U2_Pass.Text = Convert.ToInt32(Variable.UpPassNum2).ToString();
            D1_Pass.Text = Convert.ToInt32(Variable.DownPassNum1).ToString();
            D2_Pass.Text = Convert.ToInt32(Variable.DownPassNum2).ToString();

            U1_Fail.Text = Convert.ToInt32(Variable.UpFailNum1).ToString();
            U2_Fail.Text = Convert.ToInt32(Variable.UpFailNum2).ToString();
            D1_Fail.Text = Convert.ToInt32(Variable.DownFailNum1).ToString();
            D2_Fail.Text = Convert.ToInt32(Variable.DownFailNum2).ToString();

            //TotalYield.Text = (Variable.Yield * 100).ToString("0.0") + "%";
            U1_Yield.Text = (Variable.UpYield1 * 100).ToString("0.0") + "%";
            U2_Yield.Text = (Variable.UpYield2 * 100).ToString("0.0") + "%";
            D1_Yield.Text = (Variable.DownYield1 * 100).ToString("0.0") + "%";
            D2_Yield.Text = (Variable.DownYield2 * 100).ToString("0.0") + "%";

            for (int i = 0; i < 152; i++)
            {
                Variable.UpCellTotalNum1[i] = Convert.ToInt32(Variable.UpCellPassNum1[i] + Variable.UpCellFailNum1[i]);
                Variable.UpCellTotalNum2[i] = Convert.ToInt32(Variable.UpCellPassNum2[i] + Variable.UpCellFailNum2[i]);
                Variable.DownCellTotalNum1[i] = Convert.ToInt32(Variable.DownCellPassNum1[i] + Variable.DownCellFailNum1[i]);
                Variable.DownCellTotalNum2[i] = Convert.ToInt32(Variable.DownCellPassNum2[i] + Variable.DownCellFailNum2[i]);

                if (Variable.UpCellTotalNum1[i] != 0)
                {
                    Variable.UpCellYield1[i] = Variable.UpCellPassNum1[i] / Variable.UpCellTotalNum1[i] * 100;
                }
                if (Variable.UpCellTotalNum2[i] != 0)
                {
                    Variable.UpCellYield2[i] = Variable.UpCellPassNum2[i] / Variable.UpCellTotalNum2[i] * 100;
                }
                if (Variable.DownCellTotalNum1[i] != 0)
                {
                    Variable.DownCellYield1[i] = Variable.DownCellPassNum1[i] / Variable.DownCellTotalNum1[i] * 100;
                }
                if (Variable.DownCellTotalNum2[i] != 0)
                {
                    Variable.DownCellYield2[i] = Variable.DownCellPassNum2[i] / Variable.DownCellTotalNum2[i] * 100;
                }
            }
        }
        #endregion

        #region DataGrid界面颜色刷新
        public void GridColorRefresh(DataGridView GridU, string[] data)
        {
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    //刷新值
                    if (data[i * 8 + j] == "02")
                    {
                        GridU.Rows[j].Cells[i].Style.ForeColor = Color.Red;
                        GridU.Rows[j].Cells[i].Value = "02";
                    }
                    else if (data[i * 8 + j] == "01")
                    {
                        GridU.Rows[j].Cells[i].Style.ForeColor = Color.Blue;
                        GridU.Rows[j].Cells[i].Value = "01";
                    }
                    else if (data[i * 8 + j] == "00")
                    {
                        GridU.Rows[j].Cells[i].Style.ForeColor = Color.Green;
                        GridU.Rows[j].Cells[i].Value = "00";
                    }
                }
            }
        }

        #endregion

        #region 程序等待延迟执行
        //[System.Runtime.InteropServices.DllImport("kernel32.dll")]
        //static extern uint GetTickCount();
        //static void MySleep(uint ms)
        //{
        //    uint start = GetTickCount();
        //    while (GetTickCount() - start <ms)
        //    {
        //        Application.DoEvents();
        //    }
        //}
        #endregion

        #region 上层发送数据
        public void UpSendData1(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                udpServer.SendMsg(Variable.txtboxIP[i], Variable.txtboxPoint[i], str);
                Thread.Sleep(5);
            }
        }

        public void UpSendData2(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                udpServer.SendMsg(Variable.txtboxIP[i + 10], Variable.txtboxPoint[i + 10], str);
                Thread.Sleep(5);
            }
        }

        public void UpSendDataVCC1(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Variable.receiveUPVCC1[i] == "" || Variable.receiveUPVCC1[i] == null)
                {
                    udpServer.SendMsg(Variable.txtboxIP[i], Variable.txtboxPoint[i], str);
                    Thread.Sleep(5);
                }
            }
        }

        public void UpSendDataVCC2(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Variable.receiveUPVCC2[i] == "" || Variable.receiveUPVCC2[i] == null)
                {
                    udpServer.SendMsg(Variable.txtboxIP[i + 10], Variable.txtboxPoint[i + 10], str);
                    Thread.Sleep(5);
                }
            }
        }

        public void UpSendDataVCQ1(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Variable.receiveUPVCQ1[i] == "" || Variable.receiveUPVCQ1[i] == null)
                {
                    udpServer.SendMsg(Variable.txtboxIP[i], Variable.txtboxPoint[i], str);
                    Thread.Sleep(5);
                }
            }
        }

        public void UpSendDataVCQ2(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Variable.receiveUPVCQ2[i] == "" || Variable.receiveUPVCQ2[i] == null)
                {
                    udpServer.SendMsg(Variable.txtboxIP[i + 10], Variable.txtboxPoint[i + 10], str);
                    Thread.Sleep(5);
                }
            }
        }

        #endregion

        #region 下层发送数据
        public void DownSendData1(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                udpServer.SendMsg(Variable.txtboxIP[i + 20], Variable.txtboxPoint[i + 20], str);
                Thread.Sleep(5);
            }
        }

        public void DownSendData2(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                udpServer.SendMsg(Variable.txtboxIP[i + 30], Variable.txtboxPoint[i + 30], str);
                Thread.Sleep(5);
            }
        }

        public void DownSendDataVCC1(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Variable.receiveDownVCC1[i] == "" || Variable.receiveDownVCC1[i] == null)
                {
                    udpServer.SendMsg(Variable.txtboxIP[i + 20], Variable.txtboxPoint[i + 20], str);
                    Thread.Sleep(5);
                }
            }
        }
        public void DownSendDataVCC2(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Variable.receiveDownVCC2[i] == "" || Variable.receiveDownVCC2[i] == null)
                {
                    udpServer.SendMsg(Variable.txtboxIP[i + 30], Variable.txtboxPoint[i + 30], str);
                    Thread.Sleep(5);
                }
            }
        }

        public void DownSendDataVCQ1(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Variable.receiveDownVCQ1[i] == "" || Variable.receiveDownVCQ1[i] == null)
                {
                    udpServer.SendMsg(Variable.txtboxIP[i + 20], Variable.txtboxPoint[i + 20], str);
                    Thread.Sleep(5);
                }
            }
        }

        public void DownSendDataVCQ2(string str)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Variable.receiveDownVCQ2[i] == "" || Variable.receiveDownVCQ2[i] == null)
                {
                    udpServer.SendMsg(Variable.txtboxIP[i + 30], Variable.txtboxPoint[i + 30], str);
                    Thread.Sleep(5);
                }
            }
        }

        #endregion

        #region 上层计数
        public void GridUpCount1(string[] data)
        {
            string path = @"D:\参数\Map\photo\UpTray1.txt";
            string[] str = myTXT.ReadTXT(path);
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (data[i * 8 + j] == "00")
                    {
                        Variable.PassNum += 1;
                        Variable.UpPassNum1 += 1;
                        Variable.UpCellPassNum1[i * 8 + j] += 1;
                        UpCellPassNum1[i * 8 + j] = Variable.UpCellPassNum1[i * 8 + j].ToString();
                    }
                    else if (data[i * 8 + j] == "02")
                    {
                        if (str.Length != 0)
                        {
                            if (str[i * 8 + j] != "10")
                            {
                                Variable.FailNum += 1;
                                Variable.UpFailNum1 += 1;
                                Variable.UpCellFailNum1[i * 8 + j] += 1;
                                UpCellFailNum1[i * 8 + j] = Variable.UpCellFailNum1[i * 8 + j].ToString();
                            }
                        }
                        else
                        {
                            Variable.FailNum += 1;
                            Variable.UpFailNum1 += 1;
                            Variable.UpCellFailNum1[i * 8 + j] += 1;
                            UpCellFailNum1[i * 8 + j] = Variable.UpCellFailNum1[i * 8 + j].ToString();
                        }
                    }
                }
            }
        }

        public void GridUpCount2(string[] data)
        {
            string path = @"D:\参数\Map\photo\UpTray2.txt";
            string[] str = myTXT.ReadTXT(path);
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (data[i * 8 + j] == "00")
                    {
                        Variable.PassNum += 1;
                        Variable.UpPassNum2 += 1;
                        Variable.UpCellPassNum2[i * 8 + j] += 1;
                        UpCellPassNum2[i * 8 + j] = Variable.UpCellPassNum2[i * 8 + j].ToString();
                    }
                    else if (data[i * 8 + j] == "02")
                    {
                        if (str.Length != 0)
                        {
                            if (str[i * 8 + j] != "10")
                            {
                                Variable.FailNum += 1;
                                Variable.UpFailNum2 += 1;
                                Variable.UpCellFailNum2[i * 8 + j] += 1;
                                UpCellFailNum2[i * 8 + j] = Variable.UpCellFailNum2[i * 8 + j].ToString();
                            }
                        }
                        else
                        {
                            Variable.FailNum += 1;
                            Variable.UpFailNum2 += 1;
                            Variable.UpCellFailNum2[i * 8 + j] += 1;
                            UpCellFailNum2[i * 8 + j] = Variable.UpCellFailNum2[i * 8 + j].ToString();
                        }
                    }
                }
            }
        }
        #endregion

        #region 下层计数
        public void GridDownCount1(string[] data)
        {
            string path = @"D:\参数\Map\photo\DownTray1.txt";
            string[] str = myTXT.ReadTXT(path);
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (data[i * 8 + j] == "00")
                    {
                        Variable.PassNum += 1;
                        Variable.DownPassNum1 += 1;
                        Variable.DownCellPassNum1[i * 8 + j] += 1;
                        DownCellPassNum1[i * 8 + j] = Variable.DownCellPassNum1[i * 8 + j].ToString();
                    }
                    else if (data[i * 8 + j] == "02")
                    {
                        if (str.Length != 0)
                        {
                            if (str[i * 8 + j] != "10")
                            {
                                Variable.FailNum += 1;
                                Variable.DownFailNum1 += 1;
                                Variable.DownCellFailNum1[i * 8 + j] += 1;
                                DownCellFailNum1[i * 8 + j] = Variable.DownCellFailNum1[i * 8 + j].ToString();
                            }
                        }
                        else
                        {
                            Variable.FailNum += 1;
                            Variable.DownFailNum1 += 1;
                            Variable.DownCellFailNum1[i * 8 + j] += 1;
                            DownCellFailNum1[i * 8 + j] = Variable.DownCellFailNum1[i * 8 + j].ToString();
                        }
                    }
                }
            }
        }
        public void GridDownCount2(string[] data)
        {
            string path = @"D:\参数\Map\photo\DownTray2.txt";
            string[] str = myTXT.ReadTXT(path);
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (data[i * 8 + j] == "00")
                    {
                        Variable.PassNum += 1;
                        Variable.DownPassNum2 += 1;
                        Variable.DownCellPassNum2[i * 8 + j] += 1;
                        DownCellPassNum2[i * 8 + j] = Variable.DownCellPassNum2[i * 8 + j].ToString();
                    }
                    else if (data[i * 8 + j] == "02")
                    {
                        if (str.Length != 0)
                        {
                            if (str[i * 8 + j] != "10")
                            {
                                Variable.FailNum += 1;
                                Variable.DownFailNum2 += 1;
                                Variable.DownCellFailNum2[i * 8 + j] += 1;
                                DownCellFailNum2[i * 8 + j] = Variable.DownCellFailNum2[i * 8 + j].ToString();
                            }
                        }
                        else
                        {
                            Variable.FailNum += 1;
                            Variable.DownFailNum2 += 1;
                            Variable.DownCellFailNum2[i * 8 + j] += 1;
                            DownCellFailNum2[i * 8 + j] = Variable.DownCellFailNum2[i * 8 + j].ToString();
                        }
                    }
                }
            }
        }
        #endregion

        #region 清空数组
        public void ClearArry(string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = null;
            }
        }

        public void ClearArry1(double[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
        }
        #endregion

        #region 存储异常

        public static void EX(string str)
        {
            try
            {
                FileStream fs = new FileStream(ConfigurationManager.ConnectionStrings["AbnormalCapture"].ConnectionString, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(DateTime.Now);
                sw.WriteLine(str);
                sw.Close();
                fs.Close();

            }
            catch
            {
            }
        }
        #endregion

        #region BCC校验计算

        public string BCC(string str)
        {
            int sum = 0;
            string hex = "";
            int len = str.Length;
            for (int i = 0; i < len; i = i + 2)//若参数len不包含发送符(:) 则i = 0,i < len
            {
                // 检查是否还有足够的字符进行截取
                if (i + 1 < len)
                {
                    string data = str.Substring(i, 2);//转换成1字节16进制形式数据
                    sum = sum + Convert.ToInt32(data, 16);//转换成10进制，然后叠加
                }
                else if (i < len)
                {
                    // 如果只剩下一个字符，补0处理
                    string data = str.Substring(i, 1) + "0";
                    sum = sum + Convert.ToInt32(data, 16);
                }
            }

            hex = Convert.ToInt32(sum).ToString("X");//转换为16进制显示
            if (hex.Length > 2)
            {
                hex = hex.Substring(hex.Length - 2, 2);//取最后两位
            }

            return hex;
        }

        #endregion

        //<<----------数据保存到数据库---------->>

        #region 将上层内总数量保存到数据库
        public void UpTotalToRecord1()
        {
            //将产品信息保存到数据库
            try
            {
                string[] svValue = new string[155];
                svValue[0] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//结束日期
                svValue[1] = BatchNum.Text.Trim();//批号
                svValue[2] = upNum1.ToString();//序号   

                for (int i = 0; i < Variable.UpCellTotalNum1.Length; i++)
                {
                    svValue[i + 3] = Variable.UpCellTotalNum1[i].ToString();
                }
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Insert into UpTotalRecord1(Aldate,PN,Num,sv1,sv2,sv3,sv4,sv5,sv6,sv7,sv8,sv9,sv10,sv11,sv12,sv13,sv14,sv15,sv16,sv17,sv18,sv19,sv20,sv21,sv22,sv23,sv24,sv25,sv26,sv27,sv28,sv29,sv30,sv31,sv32,sv33,sv34,sv35,sv36,sv37,sv38,sv39,sv40,sv41,sv42,sv43,sv44,sv45,sv46,sv47,sv48,sv49,sv50,sv51,sv52,sv53,sv54,sv55,sv56,sv57,sv58,sv59,sv60,sv61,sv62,sv63,sv64,sv65,sv66,sv67,sv68,sv69,sv70,sv71,sv72,sv73,sv74,sv75,sv76,sv77,sv78,sv79,sv80,sv81,sv82,sv83,sv84,sv85,sv86,sv87,sv88,sv89,sv90,sv91,sv92,sv93,sv94,sv95,sv96,sv97,sv98,sv99,sv100,sv101,sv102,sv103,sv104,sv105,sv106,sv107,sv108,sv109,sv110,sv111,sv112,sv113,sv114,sv115,sv116,sv117,sv118,sv119,sv120,sv121,sv122,sv123,sv124,sv125,sv126,sv127,sv128,sv129,sv130,sv131,sv132,sv133,sv134,sv135,sv136,sv137,sv138,sv139,sv140,sv141,sv142,sv143,sv144,sv145,sv146,sv147,sv148,sv149,sv150,sv151,sv152)" +
                            "values(@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11,@s12,@s13,@s14,@s15,@s16,@s17,@s18,@s19,@s20,@s21,@s22,@s23,@s24,@s25,@s26,@s27,@s28,@s29,@s30,@s31,@s32,@s33,@s34,@s35,@s36,@s37,@s38,@s39,@s40,@s41,@s42,@s43,@s44,@s45,@s46,@s47,@s48,@s49,@s50,@s51,@s52,@s53,@s54,@s55,@s56,@s57,@s58,@s59,@s60,@s61,@s62,@s63,@s64,@s65,@s66,@s67,@s68,@s69,@s70,@s71,@s72,@s73,@s74,@s75,@s76,@s77,@s78,@s79,@s80,@s81,@s82,@s83,@s84,@s85,@s86,@s87,@s88,@s89,@s90,@s91,@s92,@s93,@s94,@s95,@s96,@s97,@s98,@s99,@s100,@s101,@s102,@s103,@s104,@s105,@s106,@s107,@s108,@s109,@s110,@s111,@s112,@s113,@s114,@s115,@s116,@s117,@s118,@s119,@s120,@s121,@s122,@s123,@s124,@s125,@s126,@s127,@s128,@s129,@s130,@s131,@s132,@s133,@s134,@s135,@s136,@s137,@s138,@s139,@s140,@s141,@s142,@s143,@s144,@s145,@s146,@s147,@s148,@s149,@s150,@s151,@s152,@s153,@s154,@s155)";
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

        #region 将上层内良率保存到数据库
        public void UpYieldToRecord1()
        {
            //将产品信息保存到数据库
            try
            {
                string[] svValue = new string[155];
                svValue[0] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//结束日期
                svValue[1] = BatchNum.Text.Trim();//批号                
                svValue[2] = upNum1.ToString();//序号 

                for (int i = 0; i < Variable.UpCellYield1.Length; i++)
                {
                    svValue[i + 3] = Variable.UpCellYield1[i].ToString("0.00") + "%";
                }
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Insert into UpYieldRecord1(Aldate,PN,Num,sv1,sv2,sv3,sv4,sv5,sv6,sv7,sv8,sv9,sv10,sv11,sv12,sv13,sv14,sv15,sv16,sv17,sv18,sv19,sv20,sv21,sv22,sv23,sv24,sv25,sv26,sv27,sv28,sv29,sv30,sv31,sv32,sv33,sv34,sv35,sv36,sv37,sv38,sv39,sv40,sv41,sv42,sv43,sv44,sv45,sv46,sv47,sv48,sv49,sv50,sv51,sv52,sv53,sv54,sv55,sv56,sv57,sv58,sv59,sv60,sv61,sv62,sv63,sv64,sv65,sv66,sv67,sv68,sv69,sv70,sv71,sv72,sv73,sv74,sv75,sv76,sv77,sv78,sv79,sv80,sv81,sv82,sv83,sv84,sv85,sv86,sv87,sv88,sv89,sv90,sv91,sv92,sv93,sv94,sv95,sv96,sv97,sv98,sv99,sv100,sv101,sv102,sv103,sv104,sv105,sv106,sv107,sv108,sv109,sv110,sv111,sv112,sv113,sv114,sv115,sv116,sv117,sv118,sv119,sv120,sv121,sv122,sv123,sv124,sv125,sv126,sv127,sv128,sv129,sv130,sv131,sv132,sv133,sv134,sv135,sv136,sv137,sv138,sv139,sv140,sv141,sv142,sv143,sv144,sv145,sv146,sv147,sv148,sv149,sv150,sv151,sv152)" +
                            "values(@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11,@s12,@s13,@s14,@s15,@s16,@s17,@s18,@s19,@s20,@s21,@s22,@s23,@s24,@s25,@s26,@s27,@s28,@s29,@s30,@s31,@s32,@s33,@s34,@s35,@s36,@s37,@s38,@s39,@s40,@s41,@s42,@s43,@s44,@s45,@s46,@s47,@s48,@s49,@s50,@s51,@s52,@s53,@s54,@s55,@s56,@s57,@s58,@s59,@s60,@s61,@s62,@s63,@s64,@s65,@s66,@s67,@s68,@s69,@s70,@s71,@s72,@s73,@s74,@s75,@s76,@s77,@s78,@s79,@s80,@s81,@s82,@s83,@s84,@s85,@s86,@s87,@s88,@s89,@s90,@s91,@s92,@s93,@s94,@s95,@s96,@s97,@s98,@s99,@s100,@s101,@s102,@s103,@s104,@s105,@s106,@s107,@s108,@s109,@s110,@s111,@s112,@s113,@s114,@s115,@s116,@s117,@s118,@s119,@s120,@s121,@s122,@s123,@s124,@s125,@s126,@s127,@s128,@s129,@s130,@s131,@s132,@s133,@s134,@s135,@s136,@s137,@s138,@s139,@s140,@s141,@s142,@s143,@s144,@s145,@s146,@s147,@s148,@s149,@s150,@s151,@s152,@s153,@s154,@s155)";
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

        #region 将上层外总数量保存到数据库
        public void UpTotalToRecord2()
        {
            //将产品信息保存到数据库
            try
            {
                string[] svValue = new string[155];
                svValue[0] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//结束日期
                svValue[1] = BatchNum.Text.Trim();//批号
                svValue[2] = upNum2.ToString();//序号   

                for (int i = 0; i < Variable.UpCellTotalNum2.Length; i++)
                {
                    svValue[i + 3] = Variable.UpCellTotalNum2[i].ToString();
                }
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Insert into UpTotalRecord2(Aldate,PN,Num,sv1,sv2,sv3,sv4,sv5,sv6,sv7,sv8,sv9,sv10,sv11,sv12,sv13,sv14,sv15,sv16,sv17,sv18,sv19,sv20,sv21,sv22,sv23,sv24,sv25,sv26,sv27,sv28,sv29,sv30,sv31,sv32,sv33,sv34,sv35,sv36,sv37,sv38,sv39,sv40,sv41,sv42,sv43,sv44,sv45,sv46,sv47,sv48,sv49,sv50,sv51,sv52,sv53,sv54,sv55,sv56,sv57,sv58,sv59,sv60,sv61,sv62,sv63,sv64,sv65,sv66,sv67,sv68,sv69,sv70,sv71,sv72,sv73,sv74,sv75,sv76,sv77,sv78,sv79,sv80,sv81,sv82,sv83,sv84,sv85,sv86,sv87,sv88,sv89,sv90,sv91,sv92,sv93,sv94,sv95,sv96,sv97,sv98,sv99,sv100,sv101,sv102,sv103,sv104,sv105,sv106,sv107,sv108,sv109,sv110,sv111,sv112,sv113,sv114,sv115,sv116,sv117,sv118,sv119,sv120,sv121,sv122,sv123,sv124,sv125,sv126,sv127,sv128,sv129,sv130,sv131,sv132,sv133,sv134,sv135,sv136,sv137,sv138,sv139,sv140,sv141,sv142,sv143,sv144,sv145,sv146,sv147,sv148,sv149,sv150,sv151,sv152)" +
                            "values(@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11,@s12,@s13,@s14,@s15,@s16,@s17,@s18,@s19,@s20,@s21,@s22,@s23,@s24,@s25,@s26,@s27,@s28,@s29,@s30,@s31,@s32,@s33,@s34,@s35,@s36,@s37,@s38,@s39,@s40,@s41,@s42,@s43,@s44,@s45,@s46,@s47,@s48,@s49,@s50,@s51,@s52,@s53,@s54,@s55,@s56,@s57,@s58,@s59,@s60,@s61,@s62,@s63,@s64,@s65,@s66,@s67,@s68,@s69,@s70,@s71,@s72,@s73,@s74,@s75,@s76,@s77,@s78,@s79,@s80,@s81,@s82,@s83,@s84,@s85,@s86,@s87,@s88,@s89,@s90,@s91,@s92,@s93,@s94,@s95,@s96,@s97,@s98,@s99,@s100,@s101,@s102,@s103,@s104,@s105,@s106,@s107,@s108,@s109,@s110,@s111,@s112,@s113,@s114,@s115,@s116,@s117,@s118,@s119,@s120,@s121,@s122,@s123,@s124,@s125,@s126,@s127,@s128,@s129,@s130,@s131,@s132,@s133,@s134,@s135,@s136,@s137,@s138,@s139,@s140,@s141,@s142,@s143,@s144,@s145,@s146,@s147,@s148,@s149,@s150,@s151,@s152,@s153,@s154,@s155)";
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

        #region 将上层外良率保存到数据库
        public void UpYieldToRecord2()
        {
            //将产品信息保存到数据库
            try
            {
                string[] svValue = new string[155];
                svValue[0] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//结束日期
                svValue[1] = BatchNum.Text.Trim();//批号                
                svValue[2] = upNum2.ToString();//序号 

                for (int i = 0; i < Variable.UpCellYield2.Length; i++)
                {
                    svValue[i + 3] = Variable.UpCellYield2[i].ToString("0.00") + "%";
                }
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Insert into UpYieldRecord2(Aldate,PN,Num,sv1,sv2,sv3,sv4,sv5,sv6,sv7,sv8,sv9,sv10,sv11,sv12,sv13,sv14,sv15,sv16,sv17,sv18,sv19,sv20,sv21,sv22,sv23,sv24,sv25,sv26,sv27,sv28,sv29,sv30,sv31,sv32,sv33,sv34,sv35,sv36,sv37,sv38,sv39,sv40,sv41,sv42,sv43,sv44,sv45,sv46,sv47,sv48,sv49,sv50,sv51,sv52,sv53,sv54,sv55,sv56,sv57,sv58,sv59,sv60,sv61,sv62,sv63,sv64,sv65,sv66,sv67,sv68,sv69,sv70,sv71,sv72,sv73,sv74,sv75,sv76,sv77,sv78,sv79,sv80,sv81,sv82,sv83,sv84,sv85,sv86,sv87,sv88,sv89,sv90,sv91,sv92,sv93,sv94,sv95,sv96,sv97,sv98,sv99,sv100,sv101,sv102,sv103,sv104,sv105,sv106,sv107,sv108,sv109,sv110,sv111,sv112,sv113,sv114,sv115,sv116,sv117,sv118,sv119,sv120,sv121,sv122,sv123,sv124,sv125,sv126,sv127,sv128,sv129,sv130,sv131,sv132,sv133,sv134,sv135,sv136,sv137,sv138,sv139,sv140,sv141,sv142,sv143,sv144,sv145,sv146,sv147,sv148,sv149,sv150,sv151,sv152)" +
                            "values(@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11,@s12,@s13,@s14,@s15,@s16,@s17,@s18,@s19,@s20,@s21,@s22,@s23,@s24,@s25,@s26,@s27,@s28,@s29,@s30,@s31,@s32,@s33,@s34,@s35,@s36,@s37,@s38,@s39,@s40,@s41,@s42,@s43,@s44,@s45,@s46,@s47,@s48,@s49,@s50,@s51,@s52,@s53,@s54,@s55,@s56,@s57,@s58,@s59,@s60,@s61,@s62,@s63,@s64,@s65,@s66,@s67,@s68,@s69,@s70,@s71,@s72,@s73,@s74,@s75,@s76,@s77,@s78,@s79,@s80,@s81,@s82,@s83,@s84,@s85,@s86,@s87,@s88,@s89,@s90,@s91,@s92,@s93,@s94,@s95,@s96,@s97,@s98,@s99,@s100,@s101,@s102,@s103,@s104,@s105,@s106,@s107,@s108,@s109,@s110,@s111,@s112,@s113,@s114,@s115,@s116,@s117,@s118,@s119,@s120,@s121,@s122,@s123,@s124,@s125,@s126,@s127,@s128,@s129,@s130,@s131,@s132,@s133,@s134,@s135,@s136,@s137,@s138,@s139,@s140,@s141,@s142,@s143,@s144,@s145,@s146,@s147,@s148,@s149,@s150,@s151,@s152,@s153,@s154,@s155)";
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

        #region 将下层内总数量保存到数据库
        public void DownTotalToRecord1()
        {
            //将产品信息保存到数据库
            try
            {
                string[] svValue = new string[155];
                svValue[0] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//结束日期
                svValue[1] = BatchNum.Text.Trim();//批号                
                svValue[2] = downNum1.ToString();//序号  

                for (int i = 0; i < Variable.DownCellTotalNum1.Length; i++)
                {
                    svValue[i + 3] = Variable.DownCellTotalNum1[i].ToString();
                }
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Insert into DownTotalRecord1(Aldate,PN,Num,sv1,sv2,sv3,sv4,sv5,sv6,sv7,sv8,sv9,sv10,sv11,sv12,sv13,sv14,sv15,sv16,sv17,sv18,sv19,sv20,sv21,sv22,sv23,sv24,sv25,sv26,sv27,sv28,sv29,sv30,sv31,sv32,sv33,sv34,sv35,sv36,sv37,sv38,sv39,sv40,sv41,sv42,sv43,sv44,sv45,sv46,sv47,sv48,sv49,sv50,sv51,sv52,sv53,sv54,sv55,sv56,sv57,sv58,sv59,sv60,sv61,sv62,sv63,sv64,sv65,sv66,sv67,sv68,sv69,sv70,sv71,sv72,sv73,sv74,sv75,sv76,sv77,sv78,sv79,sv80,sv81,sv82,sv83,sv84,sv85,sv86,sv87,sv88,sv89,sv90,sv91,sv92,sv93,sv94,sv95,sv96,sv97,sv98,sv99,sv100,sv101,sv102,sv103,sv104,sv105,sv106,sv107,sv108,sv109,sv110,sv111,sv112,sv113,sv114,sv115,sv116,sv117,sv118,sv119,sv120,sv121,sv122,sv123,sv124,sv125,sv126,sv127,sv128,sv129,sv130,sv131,sv132,sv133,sv134,sv135,sv136,sv137,sv138,sv139,sv140,sv141,sv142,sv143,sv144,sv145,sv146,sv147,sv148,sv149,sv150,sv151,sv152)" +
                            "values(@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11,@s12,@s13,@s14,@s15,@s16,@s17,@s18,@s19,@s20,@s21,@s22,@s23,@s24,@s25,@s26,@s27,@s28,@s29,@s30,@s31,@s32,@s33,@s34,@s35,@s36,@s37,@s38,@s39,@s40,@s41,@s42,@s43,@s44,@s45,@s46,@s47,@s48,@s49,@s50,@s51,@s52,@s53,@s54,@s55,@s56,@s57,@s58,@s59,@s60,@s61,@s62,@s63,@s64,@s65,@s66,@s67,@s68,@s69,@s70,@s71,@s72,@s73,@s74,@s75,@s76,@s77,@s78,@s79,@s80,@s81,@s82,@s83,@s84,@s85,@s86,@s87,@s88,@s89,@s90,@s91,@s92,@s93,@s94,@s95,@s96,@s97,@s98,@s99,@s100,@s101,@s102,@s103,@s104,@s105,@s106,@s107,@s108,@s109,@s110,@s111,@s112,@s113,@s114,@s115,@s116,@s117,@s118,@s119,@s120,@s121,@s122,@s123,@s124,@s125,@s126,@s127,@s128,@s129,@s130,@s131,@s132,@s133,@s134,@s135,@s136,@s137,@s138,@s139,@s140,@s141,@s142,@s143,@s144,@s145,@s146,@s147,@s148,@s149,@s150,@s151,@s152,@s153,@s154,@s155)";
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

        #region 将下层内良率保存到数据库
        public void DownYieldToRecord1()
        {
            //将产品信息保存到数据库
            try
            {
                string[] svValue = new string[155];
                svValue[0] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//结束日期
                svValue[1] = BatchNum.Text.Trim();//批号                
                svValue[2] = downNum1.ToString();//序号 

                for (int i = 0; i < Variable.DownCellYield1.Length; i++)
                {
                    svValue[i + 3] = Variable.DownCellYield1[i].ToString("0.00") + "%";
                }
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Insert into DownYieldRecord1(Aldate,PN,Num,sv1,sv2,sv3,sv4,sv5,sv6,sv7,sv8,sv9,sv10,sv11,sv12,sv13,sv14,sv15,sv16,sv17,sv18,sv19,sv20,sv21,sv22,sv23,sv24,sv25,sv26,sv27,sv28,sv29,sv30,sv31,sv32,sv33,sv34,sv35,sv36,sv37,sv38,sv39,sv40,sv41,sv42,sv43,sv44,sv45,sv46,sv47,sv48,sv49,sv50,sv51,sv52,sv53,sv54,sv55,sv56,sv57,sv58,sv59,sv60,sv61,sv62,sv63,sv64,sv65,sv66,sv67,sv68,sv69,sv70,sv71,sv72,sv73,sv74,sv75,sv76,sv77,sv78,sv79,sv80,sv81,sv82,sv83,sv84,sv85,sv86,sv87,sv88,sv89,sv90,sv91,sv92,sv93,sv94,sv95,sv96,sv97,sv98,sv99,sv100,sv101,sv102,sv103,sv104,sv105,sv106,sv107,sv108,sv109,sv110,sv111,sv112,sv113,sv114,sv115,sv116,sv117,sv118,sv119,sv120,sv121,sv122,sv123,sv124,sv125,sv126,sv127,sv128,sv129,sv130,sv131,sv132,sv133,sv134,sv135,sv136,sv137,sv138,sv139,sv140,sv141,sv142,sv143,sv144,sv145,sv146,sv147,sv148,sv149,sv150,sv151,sv152)" +
                            "values(@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11,@s12,@s13,@s14,@s15,@s16,@s17,@s18,@s19,@s20,@s21,@s22,@s23,@s24,@s25,@s26,@s27,@s28,@s29,@s30,@s31,@s32,@s33,@s34,@s35,@s36,@s37,@s38,@s39,@s40,@s41,@s42,@s43,@s44,@s45,@s46,@s47,@s48,@s49,@s50,@s51,@s52,@s53,@s54,@s55,@s56,@s57,@s58,@s59,@s60,@s61,@s62,@s63,@s64,@s65,@s66,@s67,@s68,@s69,@s70,@s71,@s72,@s73,@s74,@s75,@s76,@s77,@s78,@s79,@s80,@s81,@s82,@s83,@s84,@s85,@s86,@s87,@s88,@s89,@s90,@s91,@s92,@s93,@s94,@s95,@s96,@s97,@s98,@s99,@s100,@s101,@s102,@s103,@s104,@s105,@s106,@s107,@s108,@s109,@s110,@s111,@s112,@s113,@s114,@s115,@s116,@s117,@s118,@s119,@s120,@s121,@s122,@s123,@s124,@s125,@s126,@s127,@s128,@s129,@s130,@s131,@s132,@s133,@s134,@s135,@s136,@s137,@s138,@s139,@s140,@s141,@s142,@s143,@s144,@s145,@s146,@s147,@s148,@s149,@s150,@s151,@s152,@s153,@s154,@s155)";
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

        #region 将下层外总数量保存到数据库
        public void DownTotalToRecord2()
        {
            //将产品信息保存到数据库
            try
            {
                string[] svValue = new string[155];
                svValue[0] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//结束日期
                svValue[1] = BatchNum.Text.Trim();//批号                
                svValue[2] = downNum2.ToString();//序号  

                for (int i = 0; i < Variable.DownCellTotalNum2.Length; i++)
                {
                    svValue[i + 3] = Variable.DownCellTotalNum2[i].ToString();
                }
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Insert into DownTotalRecord2(Aldate,PN,Num,sv1,sv2,sv3,sv4,sv5,sv6,sv7,sv8,sv9,sv10,sv11,sv12,sv13,sv14,sv15,sv16,sv17,sv18,sv19,sv20,sv21,sv22,sv23,sv24,sv25,sv26,sv27,sv28,sv29,sv30,sv31,sv32,sv33,sv34,sv35,sv36,sv37,sv38,sv39,sv40,sv41,sv42,sv43,sv44,sv45,sv46,sv47,sv48,sv49,sv50,sv51,sv52,sv53,sv54,sv55,sv56,sv57,sv58,sv59,sv60,sv61,sv62,sv63,sv64,sv65,sv66,sv67,sv68,sv69,sv70,sv71,sv72,sv73,sv74,sv75,sv76,sv77,sv78,sv79,sv80,sv81,sv82,sv83,sv84,sv85,sv86,sv87,sv88,sv89,sv90,sv91,sv92,sv93,sv94,sv95,sv96,sv97,sv98,sv99,sv100,sv101,sv102,sv103,sv104,sv105,sv106,sv107,sv108,sv109,sv110,sv111,sv112,sv113,sv114,sv115,sv116,sv117,sv118,sv119,sv120,sv121,sv122,sv123,sv124,sv125,sv126,sv127,sv128,sv129,sv130,sv131,sv132,sv133,sv134,sv135,sv136,sv137,sv138,sv139,sv140,sv141,sv142,sv143,sv144,sv145,sv146,sv147,sv148,sv149,sv150,sv151,sv152)" +
                            "values(@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11,@s12,@s13,@s14,@s15,@s16,@s17,@s18,@s19,@s20,@s21,@s22,@s23,@s24,@s25,@s26,@s27,@s28,@s29,@s30,@s31,@s32,@s33,@s34,@s35,@s36,@s37,@s38,@s39,@s40,@s41,@s42,@s43,@s44,@s45,@s46,@s47,@s48,@s49,@s50,@s51,@s52,@s53,@s54,@s55,@s56,@s57,@s58,@s59,@s60,@s61,@s62,@s63,@s64,@s65,@s66,@s67,@s68,@s69,@s70,@s71,@s72,@s73,@s74,@s75,@s76,@s77,@s78,@s79,@s80,@s81,@s82,@s83,@s84,@s85,@s86,@s87,@s88,@s89,@s90,@s91,@s92,@s93,@s94,@s95,@s96,@s97,@s98,@s99,@s100,@s101,@s102,@s103,@s104,@s105,@s106,@s107,@s108,@s109,@s110,@s111,@s112,@s113,@s114,@s115,@s116,@s117,@s118,@s119,@s120,@s121,@s122,@s123,@s124,@s125,@s126,@s127,@s128,@s129,@s130,@s131,@s132,@s133,@s134,@s135,@s136,@s137,@s138,@s139,@s140,@s141,@s142,@s143,@s144,@s145,@s146,@s147,@s148,@s149,@s150,@s151,@s152,@s153,@s154,@s155)";
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

        #region 将下层外良率保存到数据库
        public void DownYieldToRecord2()
        {
            //将产品信息保存到数据库
            try
            {
                string[] svValue = new string[155];
                svValue[0] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//结束日期
                svValue[1] = BatchNum.Text.Trim();//批号                
                svValue[2] = downNum2.ToString();//序号 

                for (int i = 0; i < Variable.DownCellYield2.Length; i++)
                {
                    svValue[i + 3] = Variable.DownCellYield2[i].ToString("0.00") + "%";
                }
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Insert into DownYieldRecord2(Aldate,PN,Num,sv1,sv2,sv3,sv4,sv5,sv6,sv7,sv8,sv9,sv10,sv11,sv12,sv13,sv14,sv15,sv16,sv17,sv18,sv19,sv20,sv21,sv22,sv23,sv24,sv25,sv26,sv27,sv28,sv29,sv30,sv31,sv32,sv33,sv34,sv35,sv36,sv37,sv38,sv39,sv40,sv41,sv42,sv43,sv44,sv45,sv46,sv47,sv48,sv49,sv50,sv51,sv52,sv53,sv54,sv55,sv56,sv57,sv58,sv59,sv60,sv61,sv62,sv63,sv64,sv65,sv66,sv67,sv68,sv69,sv70,sv71,sv72,sv73,sv74,sv75,sv76,sv77,sv78,sv79,sv80,sv81,sv82,sv83,sv84,sv85,sv86,sv87,sv88,sv89,sv90,sv91,sv92,sv93,sv94,sv95,sv96,sv97,sv98,sv99,sv100,sv101,sv102,sv103,sv104,sv105,sv106,sv107,sv108,sv109,sv110,sv111,sv112,sv113,sv114,sv115,sv116,sv117,sv118,sv119,sv120,sv121,sv122,sv123,sv124,sv125,sv126,sv127,sv128,sv129,sv130,sv131,sv132,sv133,sv134,sv135,sv136,sv137,sv138,sv139,sv140,sv141,sv142,sv143,sv144,sv145,sv146,sv147,sv148,sv149,sv150,sv151,sv152)" +
                            "values(@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11,@s12,@s13,@s14,@s15,@s16,@s17,@s18,@s19,@s20,@s21,@s22,@s23,@s24,@s25,@s26,@s27,@s28,@s29,@s30,@s31,@s32,@s33,@s34,@s35,@s36,@s37,@s38,@s39,@s40,@s41,@s42,@s43,@s44,@s45,@s46,@s47,@s48,@s49,@s50,@s51,@s52,@s53,@s54,@s55,@s56,@s57,@s58,@s59,@s60,@s61,@s62,@s63,@s64,@s65,@s66,@s67,@s68,@s69,@s70,@s71,@s72,@s73,@s74,@s75,@s76,@s77,@s78,@s79,@s80,@s81,@s82,@s83,@s84,@s85,@s86,@s87,@s88,@s89,@s90,@s91,@s92,@s93,@s94,@s95,@s96,@s97,@s98,@s99,@s100,@s101,@s102,@s103,@s104,@s105,@s106,@s107,@s108,@s109,@s110,@s111,@s112,@s113,@s114,@s115,@s116,@s117,@s118,@s119,@s120,@s121,@s122,@s123,@s124,@s125,@s126,@s127,@s128,@s129,@s130,@s131,@s132,@s133,@s134,@s135,@s136,@s137,@s138,@s139,@s140,@s141,@s142,@s143,@s144,@s145,@s146,@s147,@s148,@s149,@s150,@s151,@s152,@s153,@s154,@s155)";
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

        #region 按钮

        private void button4_Click(object sender, EventArgs e)//开始按钮
        {
            Variable.MachineState = Variable.MachineStatus.Running;
            button3.Enabled = false;
            button5.Enabled = false;
            Variable.RunEnable = true;
            //Variable.UpAutoStep = 10;
            //Variable.DownAutoStep = 10;

        }
        private void button3_Click(object sender, EventArgs e)//上层复位
        {
            // Variable.MachineState = Variable.MachineStatus.zero;
            if (Variable.MachineState == Variable.MachineStatus.Stop || Variable.MachineState == Variable.MachineStatus.Pause || Variable.MachineState == Variable.MachineStatus.Emg)
            {
                Variable.RunEnable = false;
                U1_StartTime.Text = "0";
                U1_EndTime.Text = "0";
                U1_TestTime.Text = "0";
                U2_StartTime.Text = "0";
                U2_EndTime.Text = "0";
                U2_TestTime.Text = "0";

                Variable.UpTotalNum1 = 0;
                Variable.UpTotalNum2 = 0;
                Variable.UpPassNum1 = 0;
                Variable.UpPassNum2 = 0;
                Variable.UpFailNum1 = 0;
                Variable.UpFailNum2 = 0;
                Variable.UpYield1 = 0;
                Variable.UpYield2 = 0;
                come_up = false;//上前
                upper_back = false;//上后
                OutYOFF(23);//1#断电
                OutYOFF(24);//2#断电
                Delay(1);
                OutYOFF(18);//
                OutYON(17);//1#压合气缸下降
                Thread.Sleep(2000);
                OutYOFF(17);//

                OutYOFF(20);//                
                OutYON(19);//2#压合气缸下降
                Thread.Sleep(2000);
                OutYOFF(19);//
                OutYOFF(21);//侧定位回
                OutYOFF(22);//推tray气缸回                

                Variable.UpAutoStep = 10;
                Variable.UpAutoStep1 = 10;
                Variable.UpAutoStep2 = 10;
                above = false;//上层
                parameterForm.LoadParameter(Application.StartupPath + "\\Parameter.ini");
                Variable.MachineState = Variable.MachineStatus.Pause;
            }
        }

        private void button5_Click(object sender, EventArgs e)//下层复位
        {
            if (Variable.MachineState == Variable.MachineStatus.Stop || Variable.MachineState == Variable.MachineStatus.Pause || Variable.MachineState == Variable.MachineStatus.Emg)
            {
                Variable.RunEnable = false;
                D1_StartTime.Text = "0";
                D1_EndTime.Text = "0";
                D1_TestTime.Text = "0";
                D2_StartTime.Text = "0";
                D2_EndTime.Text = "0";
                D2_TestTime.Text = "0";

                Variable.DownTotalNum1 = 0;
                Variable.DownTotalNum2 = 0;
                Variable.DownPassNum1 = 0;
                Variable.DownPassNum2 = 0;
                Variable.DownFailNum1 = 0;
                Variable.DownFailNum2 = 0;
                Variable.DownYield1 = 0;
                Variable.DownYield2 = 0;
                lower_front = false;//下前
                lower_back = false;//下后
                beloew = false;//下层
                OutYOFF(39);//1#断电
                OutYOFF(40);//2#断电
                Delay(1);
                OutYOFF(34);
                OutYON(33);//1#压合气缸下降
                Thread.Sleep(2000);
                OutYOFF(33);
                OutYOFF(36);
                OutYON(35);//2#压合气缸下降
                Thread.Sleep(2000);
                OutYOFF(35);
                OutYOFF(37);//侧定位回
                OutYOFF(38);//推tray气缸回                

                Variable.DownAutoStep = 10;
                Variable.DownAutoStep1 = 10;
                Variable.DownAutoStep2 = 10;
                parameterForm.LoadParameter(Application.StartupPath + "\\Parameter.ini");
                Variable.MachineState = Variable.MachineStatus.Pause;
            }
        }

        private void button1_Click(object sender, EventArgs e)//上层侧定位
        {
            //if (Variable.MachineState == Variable.MachineStatus.Stop || Variable.MachineState == Variable.MachineStatus.Pause || Variable.MachineState == Variable.MachineStatus.Reset)
            //{
            if (Variable.MachineState != Variable.MachineStatus.Emg)
            {
                if (Variable.XStatus[27] && !Variable.XStatus[26])
                {
                    OutYOFF(21);
                }
                else if (!Variable.XStatus[27] && Variable.XStatus[26])
                {
                    OutYON(21);
                }
            }
            //}
        }

        private void button2_Click(object sender, EventArgs e)//下层侧定位
        {
            //if (Variable.MachineState == Variable.MachineStatus.Stop || Variable.MachineState == Variable.MachineStatus.Pause || Variable.MachineState == Variable.MachineStatus.Reset)
            //{
            if (Variable.MachineState != Variable.MachineStatus.Emg)
            {
                if (Variable.XStatus[43] && !Variable.XStatus[42])
                {
                    OutYOFF(37);
                }
                else if (!Variable.XStatus[43] && Variable.XStatus[42])
                {
                    OutYON(37);
                }
            }
            //}
        }
        private void button6_Click(object sender, EventArgs e)//停止按扭
        {
            Variable.MachineState = Variable.MachineStatus.Stop;
            button3.Enabled = true;

            Variable.RunEnable = false;
            Variable.AlarmFlag = false;
            above = false;//上层

        }
        #endregion

        #region 输出动作

        public void OutYON(int data)
        {
            if (data >= 0 && data < 48)
            {
                mc.GTN_SetExtDoBit(1, (short)(data + 1), 1);
            }
            //Variable.OnEnable[data] = true;
            //Variable.OffEnable[data] = false;
            //Variable.OnTime[data] = 0;

        }

        public void OutYOFF(int data)
        {
            if (data >= 0 && data < 48)
            {
                mc.GTN_SetExtDoBit(1, (short)(data + 1), 0);
            }
            //Variable.OffEnable[data] = true;
            //Variable.OnEnable[data] = false;
            //Variable.OffTime[data] = 0;
        }
        #endregion

        #region 延时函数
        public static bool Delay(int delayTime)
        {
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.Seconds;
                Application.DoEvents();
            }
            while (s < delayTime);
            return true;
        }
        #endregion
    }
}
