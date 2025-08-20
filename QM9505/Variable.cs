using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM9505
{
    class Variable
    {

        #region 各IO口信号读取存储

        public static double[] AIMpos = new double[30];//目标坐标值数组
        public static double[] REApos = new double[30];//实际坐标值数组
        public static int Home1, Plimit1, Nlimit1, Alarm1;
        public static int Home2, Plimit2, Nlimit2, Alarm2;

        //输入
        public static int[] XValue_0 = new int[4];
        public static int[] XValue_1 = new int[4];
        public static int[] XValue = new int[6];

        //输出
        public static int[] YValue_0 = new int[4];
        public static int[] YValue_1 = new int[4];
        public static int[] YValue = new int[6];

        #endregion

        #region 输入输出信号定义

        //按钮
        public static bool EMG;
        public static bool StartButton;
        public static bool PauseButton;
        public static bool AlarmClrButton;
        public static bool OneCycleButton;
        public static bool CleanOutButton;
        public static bool ZeroButton;

        //输入信号
        public static bool[] XStatus = new bool[300];
        //输出信号
        public static bool[] YStatus = new bool[300];
        public static bool[] OnEnable = new bool[300];
        public static bool[] OffEnable = new bool[300];
        public static int[] OnTime = new int[300];
        public static int[] OffTime = new int[300];

        #endregion

        public static string receiveMessage, receiveRecord;
        public static string[] receiveUPVCCVolt1 = new string[152];
        public static string[] receiveUPVCQVolt1 = new string[152];
        public static string[] receiveUPVCCElec1 = new string[152];
        public static string[] receiveUPVCQElec1 = new string[152];
        public static string[] receiveUPState1 = new string[152];
        public static string[] receiveUPVCC1 = new string[10];
        public static string[] receiveUPVCQ1 = new string[10];
        public static string[] receiveDownVCCVolt1 = new string[152];
        public static string[] receiveDownVCQVolt1 = new string[152];
        public static string[] receiveDownVCCElec1 = new string[152];
        public static string[] receiveDownVCQElec1 = new string[152];
        public static string[] receiveDownState1 = new string[152];
        public static string[] receiveDownVCC1 = new string[10];
        public static string[] receiveDownVCQ1 = new string[10];

        public static string[] receiveUPVCCVolt2 = new string[152];
        public static string[] receiveUPVCQVolt2 = new string[152];
        public static string[] receiveUPVCCElec2 = new string[152];
        public static string[] receiveUPVCQElec2 = new string[152];
        public static string[] receiveUPState2 = new string[152];
        public static string[] receiveUPVCC2 = new string[10];
        public static string[] receiveUPVCQ2 = new string[10];
        public static string[] receiveDownVCCVolt2 = new string[152];
        public static string[] receiveDownVCQVolt2 = new string[152];
        public static string[] receiveDownVCCElec2 = new string[152];
        public static string[] receiveDownVCQElec2 = new string[152];
        public static string[] receiveDownState2 = new string[152];
        public static string[] receiveDownVCC2 = new string[10];
        public static string[] receiveDownVCQ2 = new string[10];


        public static int UpAutoStep = 0;//上层自动流程
        public static int DownAutoStep = 0;//下层自动流程
        public static int UpAutoStep1 = 0;//上层内测试自动流程
        public static int UpAutoStep2 = 0;//上层外测试自动流程
        public static int DownAutoStep1 = 0;//下层内测试自动流程
        public static int DownAutoStep2 = 0;//下层外测试自动流程
        public static bool RunEnable;

        public static double TotalNum;
        public static double PassNum;
        public static double FailNum;
        public static double Yield;

        public static double UpTotalNum1;
        public static double UpPassNum1;
        public static double UpFailNum1;
        public static double UpYield1;

        public static double UpTotalNum2;
        public static double UpPassNum2;
        public static double UpFailNum2;
        public static double UpYield2;

        public static double DownTotalNum1;
        public static double DownPassNum1;
        public static double DownFailNum1;
        public static double DownYield1;

        public static double DownTotalNum2;
        public static double DownPassNum2;
        public static double DownFailNum2;
        public static double DownYield2;

        public static double[] UpCellTotalNum1 = new double[152];
        public static double[] UpCellPassNum1 = new double[152];
        public static double[] UpCellFailNum1 = new double[152];
        public static double[] UpCellYield1 = new double[152];

        public static double[] UpCellTotalNum2 = new double[152];
        public static double[] UpCellPassNum2 = new double[152];
        public static double[] UpCellFailNum2 = new double[152];
        public static double[] UpCellYield2 = new double[152];

        public static double[] DownCellTotalNum1 = new double[152];
        public static double[] DownCellPassNum1 = new double[152];
        public static double[] DownCellFailNum1 = new double[152];
        public static double[] DownCellYield1 = new double[152];

        public static double[] DownCellTotalNum2 = new double[152];
        public static double[] DownCellPassNum2 = new double[152];
        public static double[] DownCellFailNum2 = new double[152];
        public static double[] DownCellYield2 = new double[152];


        public static string timeout;

        #region 用户密码
        public enum UserEnter
        {
            NoUser = 0,
            User = 1,
            Engineer = 2,
            Manufacturer = 3,
            Administrator = 4,
        }

        public static UserEnter userEnter = Variable.UserEnter.NoUser;

        #endregion

        /// <summary>
        /// 在线模式
        /// </summary>
        public static bool Online;

        /// <summary>
        /// 批号
        /// </summary>
        public static string BatchNum;

        /// <summary>
        /// 档案名称
        /// </summary>
        public static string FileName;

        /// <summary>
        /// 登录标志
        /// </summary>
        public static bool loginFlag;

        /// <summary>
        /// 参数窗体加载
        /// </summary>
        public static bool formOpenFlag = false;

        public static string[] txtboxIP = new string[40];
        public static string[] txtboxPoint = new string[40];

        public static string ServerIP;
        public static string ServerPoint;
        public static bool Serverflag;

        public static string HandlerIP;
        public static string HandlerPoint;
        public static bool Handlerflag;

        public static bool testWaitTimeCheck;
        public static bool testTimeCheck;
        public static bool testTimeOutCheck;
        public static bool VCQCheck;


        public static string VCCVolSet;
        public static string VCQVolSet;

        public static string UpModelNum1;
        public static string UpModelNum2;
        public static string DownModelNum1;
        public static string DownModelNum2;
        public static string testWaitTime;
        public static string testTime;
        public static string testTimeOut;
        public static string RecordName;
        public static string Powertime;//延迟上电时间
        public static double Total;
        public static double Pass;
        public static double TotalYield;
        public static double TesttimeShow;
        public static double VCCvoltage;
        public static double VCQvoltage;
        public static bool testTimeReadCheck;

        public static string portName;
        public static int baudRate;
        public static int dataBit;
        public static string parity;
        public static int stop;
        public static string strTemp;


        public static string time_Alarm;//报警延迟
        public static string time_cylinder;//其他气缸延迟

        /// <summary>
        /// 温控器温度
        /// </summary>
        public static string Temp="00";

        /// <summary>
        /// 温控器温度上限
        /// </summary>
        public static double TempUpLimit;

        /// <summary>
        /// 温控器温度下限
        /// </summary>
        public static double TempDownLimit;

        /// <summary>
        /// 温度设置
        /// </summary>
        public static double temper;

        /// <summary>
        /// 温度补偿值设置
        /// </summary>
        public static double offsets;

        /// <summary>
        /// 吹气时间设置
        /// </summary>
        public static double blowTime;

        /// <summary>
        /// 最高温度设置
        /// </summary>
        public static double upTemper;

        /// <summary>
        /// 温度到达延时设定;
        /// </summary>
        public static double tempSetDelay;

        /// <summary>
        /// 温度写入标志;
        /// </summary>
        public static bool temWriteFlag = false;

        /// <summary>
        /// 温度读取标志;
        /// </summary>
        public static bool temReadFlag = false;

        /// <summary>
        /// 机器状态
        /// </summary>
        /// 
        public enum MachineStatus
        {
            Emg = 0,// 急停
            Stop = 1,//停止
            Alarm = 5,//报警
            StandBy = 2,//准备就绪            
            Running = 3,//运行
            Pause = 4,//暂停
            zero = 6,//归零
            Reset = 7,//复位
        }
        public static MachineStatus MachineState;

        /// <summary>
        /// 温度
        /// </summary>
        public static double[] TemperData = new double[4];
        public static int Tempertest = 1;

        /// <summary>
        /// 高温选择
        /// </summary>
        public static bool HotModel = false;

        public static bool AlarmFlag;


        /// <summary>
        /// 报警弹窗步
        /// </summary>
        public static string step;

        /// <summary>
        /// 取消
        /// </summary>
        public static int cancelStep;

        /// <summary>
        /// 确定
        /// </summary>
        public static int sureStep;

        /// <summary>
        /// 弹窗标志
        /// </summary>
        public static bool POPFlag;

        public static bool Up1check;
        public static bool Up2check;
        public static bool Down1check;
        public static bool Down2check;
        public static bool che_door;



    }
}
