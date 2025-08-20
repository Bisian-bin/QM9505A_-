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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public partial class MainForm : Form
    {
        Function function = new Function();
        ParameterForm parameterForm = new ParameterForm();
        TCPForm tcpForm = new TCPForm();
        RecordForm recordForm = new RecordForm();
        VoltageForm voltageForm = new VoltageForm();
        YieldForm yieldForm = new YieldForm();
        ElectricityForm electricityForm = new ElectricityForm();
        UserForm userForm = new UserForm();
        Motion motion = new Motion();
        Access access = new Access();
        TXT myTXT = new TXT();
        Thread IoThread;
        public static string[] UpCellPassNum1 = new string[152];
        public static string[] UpCellFailNum1 = new string[152];
        public static string[] DownCellPassNum1 = new string[152];
        public static string[] DownCellFailNum1 = new string[152];

        public static string[] UpCellPassNum2 = new string[152];
        public static string[] UpCellFailNum2 = new string[152];
        public static string[] DownCellPassNum2 = new string[152];
        public static string[] DownCellFailNum2 = new string[152];

        #region 禁用窗体的关闭按钮
        ////禁用窗体的关闭按钮
        //private const int CP_NOCLOSE_BUTTON = 0x200;
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams myCp = base.CreateParams;
        //        myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
        //        return myCp;
        //    }
        //}
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        #region 窗体加载
        private void MainForm_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;//检查线程间非法交
            //this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = true;

            //直接读取屏幕分辨率，
            this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            //这一条必须加，不加还是会把任务栏档住。具体原因我也不知道。
            this.MinimumSize = new Size(100, 100);

            //隐藏TabControl选项栏
            this.TabControl.Region = new Region(new RectangleF(this.MainTabPage.Left, this.MainTabPage.Top, this.MainTabPage.Width, this.MainTabPage.Height));

            #region 控制卡初始化

            short rtn = motion.OpenCardInit("gtn_core1.cfg", "gtn_core2.cfg");//初始化控制卡
            if (rtn != 0)
            {
                MessageBox.Show("初始化运动控制器失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                //写入数据库
                access.RecordAccess(LogType.Operate, "初始化运动控制器失败！");
            }

            #endregion
            IoThread = new Thread(function.ReadIO);
            IoThread.IsBackground = true;
            IoThread.Start();
            //加载主窗体
            LoadMainForm(new UserForm());
            
            Variable.userEnter = Variable.UserEnter.User;
        }

        #endregion

        #region 窗体关闭

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult.Yes != MessageBox.Show("程序正在使用中,确认退出?", "请确认", MessageBoxButtons.YesNo))
            {
                e.Cancel = true;
            }
            else
            {
                WriteMessageToLog("软件运行结束，即将关闭");
                //写入数据库
                access.RecordAccess(LogType.Operate, "软件运行结束，即将关闭");

                for (int i = 0; i < 152; i++)
                {
                    UpCellPassNum1[i] = Variable.UpCellPassNum1[i].ToString();
                    UpCellFailNum1[i] = Variable.UpCellFailNum1[i].ToString();
                    DownCellPassNum1[i] = Variable.DownCellPassNum1[i].ToString();
                    DownCellFailNum1[i] = Variable.DownCellFailNum1[i].ToString();

                    UpCellPassNum2[i] = Variable.UpCellPassNum2[i].ToString();
                    UpCellFailNum2[i] = Variable.UpCellFailNum2[i].ToString();
                    DownCellPassNum2[i] = Variable.DownCellPassNum2[i].ToString();
                    DownCellFailNum2[i] = Variable.DownCellFailNum2[i].ToString();
                }

                //将OK数和NG数纪录下来
                string path1 = Application.StartupPath + "\\Map\\UpCellPassNum1";
                myTXT.WriteTxtVariable(UpCellPassNum1, path1);
                string path2 = Application.StartupPath + "\\Map\\UpCellFailNum1";
                myTXT.WriteTxtVariable(UpCellFailNum1, path2);
                string path3 = Application.StartupPath + "\\Map\\DownCellPassNum1";
                myTXT.WriteTxtVariable(DownCellPassNum1, path3);
                string path4 = Application.StartupPath + "\\Map\\DownCellFailNum1";
                myTXT.WriteTxtVariable(DownCellFailNum1, path4);

                string path5 = Application.StartupPath + "\\Map\\UpCellPassNum1";
                myTXT.WriteTxtVariable(UpCellPassNum1, path5);
                string path6 = Application.StartupPath + "\\Map\\UpCellFailNum1";
                myTXT.WriteTxtVariable(UpCellFailNum1, path6);
                string path7 = Application.StartupPath + "\\Map\\DownCellPassNum1";
                myTXT.WriteTxtVariable(DownCellPassNum1, path7);
                string path8 = Application.StartupPath + "\\Map\\DownCellFailNum1";
                myTXT.WriteTxtVariable(DownCellFailNum1, path8);

                //释放内存
                ClearMemory();

                //程序结束干净
                System.Environment.Exit(0);

            }
        }

        #endregion

        #region 页面切换
        public void LoadMainForm(object form)
        {
            TabControl.SelectTab(0);
            if (this.MainPanel.Controls.Count > 0)
            {
                this.MainPanel.Controls.RemoveAt(0);
            }

            Form f = form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.MainPanel.Controls.Add(f);
            this.MainPanel.Tag = f;
            f.Show();
        }
        public void LoadSubForm(object form)
        {
            TabControl.SelectTab(1);
            if (this.SubPanel.Controls.Count > 0)
            {
                this.SubPanel.Controls.RemoveAt(0);
            }

            Form f = form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.SubPanel.Controls.Add(f);
            this.SubPanel.Tag = f;
            f.Show();
        }

        private void 主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            CloseForm();

            if (this.SubPanel.Controls.Count > 0)
            {
                this.SubPanel.Controls.RemoveAt(0);
            }

            TabControl.SelectTab(0);
        }
      
        private void 参数界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                CloseForm();

                foreach (Control c in SubPanel.Controls)
                {
                    SubPanel.Controls.Remove(c);
                    c.Dispose();
                }
                LoadSubForm(new ParameterForm());

        }
        private void IO界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseForm();

            foreach (Control c in SubPanel.Controls)
            {
                SubPanel.Controls.Remove(c);
                c.Dispose();
            }
            LoadSubForm(new IOForm());
        }
        private void 通信界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                CloseForm();

                foreach (Control c in SubPanel.Controls)
                {
                    SubPanel.Controls.Remove(c);
                    c.Dispose();
                }
                LoadSubForm(new TCPForm());
        }
        private void 电压监控ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                CloseForm();

                foreach (Control c in SubPanel.Controls)
                {
                    SubPanel.Controls.Remove(c);
                    c.Dispose();
                }
                LoadSubForm(new VoltageForm());
        }

        private void 电流监控ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
                CloseForm();

                foreach (Control c in SubPanel.Controls)
                {
                    SubPanel.Controls.Remove(c);
                    c.Dispose();
                }
                LoadSubForm(new ElectricityForm());
        }
        
        private void 良率监控ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                CloseForm();

                foreach (Control c in SubPanel.Controls)
                {
                    SubPanel.Controls.Remove(c);
                    c.Dispose();
                }
                LoadSubForm(new YieldForm());
        }

        private void 数据记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                CloseForm();

                foreach (Control c in SubPanel.Controls)
                {
                    SubPanel.Controls.Remove(c);
                    c.Dispose();
                }
                LoadSubForm(new RecordForm());
        }
        private void SummaryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CloseForm();

            foreach (Control c in SubPanel.Controls)
            {
                SubPanel.Controls.Remove(c);
                c.Dispose();
            }
            LoadSubForm(new SummaryForm());
        }
        private void 权限登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.StartPosition = FormStartPosition.CenterScreen;
            loginForm.ShowDialog();
        }

        #endregion

        #region 释放内存
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
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

        #region 记录异常日志

        public void WriteMessageToLog(string message)
        {
            try
            {
                string path = Application.StartupPath + "\\Trace.log";
                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Create(path).Close();
                }
                StreamWriter sw = System.IO.File.AppendText(path);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + Application.StartupPath + " " + message);
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                string path = Application.StartupPath + "\\Trace.log";
                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Create(path).Close();
                }
                StreamWriter sw = System.IO.File.AppendText(path);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + Application.StartupPath + e.Message);
                sw.Flush();
                sw.Close();
            }
        }













        #endregion

        #region 关闭窗体
        public void CloseForm()
        {
            parameterForm.Close();
            parameterForm.Dispose();
            tcpForm.Close();
            tcpForm.Dispose();
            recordForm.Close();
            recordForm.Dispose();
            electricityForm.Close();
            electricityForm.Dispose();
            voltageForm.Close();
            voltageForm.Dispose();
            yieldForm.Close();
            yieldForm.Dispose();

            while (SubPanel.Controls.Count > 0)
            {
                Control ct = SubPanel.Controls[0];
                SubPanel.Controls.Remove(ct);
                ct.Dispose();
                ct = null;
            }

            //foreach (Control item in this.SubPanel.Controls)
            //{
            //    if (item is Form)
            //    {
            //        ((Form)item).Close();
            //    }
            //}
        }















        #endregion


    }
}
