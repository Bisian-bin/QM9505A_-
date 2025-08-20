using QM9505;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public class Motion
    {
        //private object threadLock;

        //public Motion()
        //{
        //    threadLock = new object();
        //}

        public static uint pClock;

        #region 板卡初始化

        /// <summary>
        /// 初始化卡: 打开运动控制卡、复位、加载配置文件、清除报警
        /// </summary>
        /// <param name="str">配置文件名</param>
        /// <returns>指令是否执行成功</returns>
        public short OpenCardInit(string str1, string str2)
        {
            short sRtn = 0;                     //返回值
            sRtn += mc.GTN_Close();            //关闭运动控制器
            Thread.Sleep(300);
            sRtn += mc.GTN_Open(5, 1);           //打开控制卡，参数默认，多卡时会自动全部打开
            sRtn += mc.GTN_Reset(1);             //复位控制器，单卡为例复位核1、2，此处复位会刷新模块io状态
            //sRtn += mc.GTN_Reset(2);
            sRtn += mc.GTN_ExtModuleInit(1, 1);                 //复位扩展模块
            //sRtn += mc.GTN_LoadConfig(1, str1);  //初始化配置，配置文件按核划分，每个核对应一个配置文件
            //sRtn += mc.GTN_LoadConfig(2, str2);
          //  sRtn += mc.GTN_ClrSts(1, 1, 12);                //核1、2清除轴状态
            //sRtn += mc.GTN_ClrSts(2, 1, 12);

            //for (short i = 1; i < 13; i++)
            //{
            //    sRtn += mc.GTN_SetAxisBand(1, i, 20, 5);
            //    sRtn += mc.GTN_SetAxisBand(2, i, 20, 5);
            //}
            return sRtn;
        }

        #endregion

        #region 轴使能

        /// <summary>
        /// 轴使能
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="axisNum">使能轴数</param>
        public void MCAxisOnAll(short cardNo, short axisNum)
        {
            short sRtn;
            sRtn = mc.GTN_ClrSts(cardNo, 1, axisNum);
            for (short i = 0; i < axisNum; i++)
            {
                sRtn = mc.GTN_AxisOn(cardNo, (short)(i + 1));
            }
        }

        #endregion

        #region 轴使能关闭

        /// <summary>
        /// 轴使能关闭
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="axisNum">使能轴数</param>
        public void MCAxisOffAll(short cardNo, short axisNum)
        {
            short sRtn;
            for (short i = 0; i < axisNum; i++)
            {
                sRtn = mc.GTN_AxisOff(cardNo, (short)(i + 1));
            }
        }

        #endregion

        #region 读取核1当前轴坐标

        public double GetAxisPosition(short AxisNo)
        {
            double pos;
            short sRtn;
            uint pClock;                //时钟信号
            sRtn = mc.GTN_GetEncPos(1, AxisNo, out pos, 1, out pClock);
            return pos;

        }
        #endregion

        #region 读取核2当前轴坐标

        public double GetAxisPosition1(short AxisNo)
        {
            double pos;
            short sRtn;
            uint pClock;                //时钟信号
            sRtn = mc.GTN_GetEncPos(2, AxisNo, out pos, 1, out pClock);
            return pos;

        }
        #endregion

        #region 轴停止
        public void AxisStop(short carID, short axis)
        {
            mc.GTN_Stop(carID, 1 << (axis - 1), 0);
        }
        #endregion

        #region 停止所有轴

        public void AxisStopAll()
        {
            for (short i = 0; i < 13; i++)
            {
                AxisStop(1, i);//轴停止
                AxisStop(2, i);
            }
        }

        #endregion

        #region JOG运动

        /// <summary>
        /// JOG运动
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="axis">轴号</param>
        /// <param name="jogP">JOG运动参数</param>
        /// <param name="velJ">JOG运动速度</param>
        /// <returns></returns>
        public void JOG(short cardNo, short axis, mc.TJogPrm jogP, double velJ)
        {
            short sRtn;
            mc.TJogPrm pJog;

            sRtn = mc.GTN_PrfJog(cardNo, axis);
            pJog = jogP;

            sRtn = mc.GTN_SetJogPrm(cardNo, axis, ref pJog);//设置jog运动参数
            sRtn = mc.GTN_SetVel(cardNo, axis, velJ);//设置目标速度,velJd的符号决定JOG运动方向
            sRtn = mc.GTN_Update(cardNo, 1 << (axis - 1));//更新轴运动


        }

        #endregion

        #region 单轴绝对位置点位运动

        /// <summary>
        /// 单轴绝对位置点位运动
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="axis">轴号</param>
        /// <param name="velP">速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="posP">位置</param>
        /// <returns></returns>
        public void P2PAbs(short cardNo, short axis, double velP, double acc, double dec, double velStart, short smoothTime, int posP)
        {

            int pStatus;//轴状态判断
            short sRtn;//返回值
            mc.TTrapPrm trap;
            sRtn = mc.GTN_PrfTrap(cardNo, axis);     //设置为点位运动，模式切换需要停止轴运动。
            //若返回值为 1：若当前轴在规划运动，请调用GT_Stop停止运动再调用该指令。
            sRtn = mc.GTN_GetTrapPrm(cardNo, axis, out trap);       /*读取点位运动参数（不一定需要）。若返回值为 1：请检查当前轴是否为 Trap 模式
                                                                    若不是，请先调用 GT_PrfTrap 将当前轴设置为 Trap 模式。*/
            trap.acc = acc;              //单位pulse/ms2
            trap.dec = dec;              //单位pulse/ms2
            trap.velStart = velStart;           //起跳速度，默认为0。
            trap.smoothTime = smoothTime;       //平滑时间，使加减速更为平滑。范围[0,50]单位ms。

            sRtn = mc.GTN_SetTrapPrm(cardNo, axis, ref trap);//设置点位运动参数。

            sRtn = mc.GTN_SetVel(cardNo, axis, velP);        //设置目标速度
            sRtn = mc.GTN_SetPos(cardNo, axis, posP);        //设置目标位置
            sRtn = mc.GTN_Update(cardNo, 1 << (axis - 1));   //更新轴运动

            //do
            //{
            //    sRtn = mc.GT_GetSts(cardNo, axis, out pStatus, 1, out pClock);//等待到位完成
            //} while ((pStatus & 0x0800) == 0);  //等待规划运动停止位判断

        }

        #endregion

        #region 单轴增量位置点位运动

        /// <summary>
        /// 单轴增量位置点位运动
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="axis">轴号</param>
        /// <param name="velP">速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="posP">位置</param>
        public void P2PInc(short cardNo, short axis, double velP, double acc, double dec, short smoothTime, int posP)
        {
            int pStatus;//轴状态判断
            short sRtn;     //返回值
            double prfPos; //规划脉冲
            uint pClock;    //时钟信号
            mc.TTrapPrm trap;
            sRtn = mc.GTN_PrfTrap(cardNo, axis);     //设置为点位运动，模式切换需要停止轴运动。
            //若返回值为 1：若当前轴在规划运动，请调用GT_Stop停止运动再调用该指令。
            sRtn = mc.GTN_GetTrapPrm(cardNo, axis, out trap);       /*读取点位运动参数（不一定需要）。若返回值为 1：请检查当前轴是否为 Trap 模式
                                                                    若不是，请先调用 GT_PrfTrap 将当前轴设置为 Trap 模式。*/
            trap.acc = acc;              //单位pulse/ms2
            trap.dec = dec;              //单位pulse/ms2
            trap.velStart = 0;           //起跳速度，默认为0。
            trap.smoothTime = smoothTime;         //平滑时间，使加减速更为平滑。范围[0,50]单位ms。

            sRtn = mc.GTN_SetTrapPrm(cardNo, axis, ref trap);//设置点位运动参数。
            sRtn = mc.GTN_GetPrfPos(cardNo, axis, out prfPos, 1, out pClock);//读取规划位置
            sRtn = mc.GTN_SetVel(cardNo, axis, velP);        //设置目标速度
            sRtn = mc.GTN_SetPos(cardNo, axis, (int)(posP + prfPos));        //设置目标位置
            sRtn = mc.GTN_Update(cardNo, 1 << (axis - 1));   //更新轴运动

            do
            {
                sRtn = mc.GTN_GetSts(cardNo, axis, out pStatus, 1, out pClock);//等待到位完成
            } while ((pStatus & 0x0800) == 0);  //等待规划运动停止位判断
        }

        #endregion

        #region 电子齿轮模式
        /// <summary>
        /// 电子齿轮模式 需要开启主轴运动后从轴才会运动
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="axisM">主轴</param>
        /// <param name="axisS">从轴</param>
        /// <param name="gearDir">跟随方向 0双向,1正向,2负向</param>
        /// <param name="gearType">跟随类型 1编码器,2规划,3改变当量后轴规划或编码器</param>
        /// <param name="ratio">电子齿轮比 从/主</param>
        /// <param name="slope">离合区 不能小于0或=1</param>
        /// <returns></returns>      
        public int Gear(short core, short axisM, short axisS, short gearDir, short gearType, int ratio, int slope)
        {
            short sRtn = 0;
            sRtn = mc.GTN_PrfGear(core, axisS, gearDir);                  //从轴跟随模式
            sRtn = mc.GTN_SetGearMaster(core, axisS, axisM, gearType, 0); //后两个参数选择跟随类型
            sRtn = mc.GTN_SetGearRatio(core, axisS, 1, ratio, slope);     //slope不能小于0或=1
            sRtn = mc.GTN_GearStart(core, 1 << (axisS - 1));
            return sRtn;
        }
        #endregion

        #region 读取IO信号
        /// <summary>
        /// 读取IO信号
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="type">IO类型</param>
        /// <param name="iNo">IO号 0-15或0-7</param>
        /// <param name="iEverBit"></param>
        /// <returns></returns>
        public int ReadIO(short cardNo, short type, short iNo, out int iEverBit)
        {
            short sRtn;
            int iValue;
            if (iNo > 15) Console.WriteLine("IO在0-15位");
            sRtn = mc.GTN_GetDi(cardNo, type, out iValue);
            if ((1 << (iNo) & iValue) == 0)
            {
                iEverBit = 0;
            }
            else
            {
                iEverBit = 1;
            }
            return sRtn;
        }
        #endregion

        #region 核1回原点

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="axis"></param>
        /// <param name="mode"></param>
        /// <param name="moveDir"></param>
        /// <param name="edge"></param>
        /// <param name="velHigh"></param>
        /// <param name="velLow"></param>
        /// <param name="escapeStep"></param>
        public void SmartHome1(short cardNo, short axis, short mode, short moveDir, short edge, double velHigh, double velLow, int escapeStep)
        {
            short sRtn;
            int pStatus;
            uint pClock;
            mc.THomeStatus tHomeStatus;
            mc.THomePrm tHomePrm = new mc.THomePrm();
            mc.THomePrm homePrm;

            //回原点参数
            tHomePrm.mode = mode;//归零模式
            tHomePrm.moveDir = moveDir;//归零方向
            tHomePrm.edge = 0;// 设置捕获沿：0-下降沿，非0值-上升沿
            if (axis >= 7)
            {
                tHomePrm.edge = 1;// 设置捕获沿：0-下降沿，非0值-上升沿
            }

            tHomePrm.acc = 0.2;//加速度
            tHomePrm.dec = 0.2;//减速度
            tHomePrm.velHigh = velHigh;//最高速度
            tHomePrm.velLow = velLow;//最低速度
            tHomePrm.smoothTime = 0; // 回原点运动的平滑时间：取值[0,1)，具体含义与GTS系列
            tHomePrm.searchHomeDistance = 0;//home搜索距离 0 默认为 805306368
            tHomePrm.searchIndexDistance = 0;//index搜索距离 0 默认为 805306368
            tHomePrm.escapeStep = escapeStep;//限位回零时反向脱离距离

            //回零前先确认点位或jog运动规划和实际位置是否一致，包括方向和大小
            sRtn = mc.GTN_Stop(cardNo, 1 << (axis - 1), 0);
            sRtn = mc.GTN_ClrSts(cardNo, axis, 1);//清除状态，回零前若在原点开关触发位置需要先移开在启动回零
            sRtn = mc.GTN_ZeroPos(cardNo, axis, 1);//清除位置保证规划位置和实际位置一致            
            sRtn = mc.GTN_GetHomePrm(cardNo, axis, out homePrm);//可省略  
            homePrm = tHomePrm;
            sRtn = mc.GTN_GoHome(cardNo, axis, ref tHomePrm);//启动SmartHome回原点
            do
            {
                sRtn = mc.GTN_GetHomeStatus(cardNo, axis, out tHomeStatus);//获取回原点状态
            } while (tHomeStatus.run == 1); // 等待搜索原点停止
            // do
            // {
            //    sRtn = mc.GTN_GetSts(cardNo, axis, out pStatus, 1, out pClock);//等待到位完成
            //} while ((pStatus & 0x0800) == 1);  //等待搜索原点停止、规划运动停止位判断
            //while ((pStatus& 0x0800) == 0) 软件内部判断到位信号，需要使用GTN_SetAxisBand()指令（螺距误差补偿功能补偿值包含在到位误差带内）
            //也可以使用GTN_GetDi指令读取驱动器到位信号来判断
            int delay = 1000;
            Thread.Sleep(delay);    //延时确保完全到位

            sRtn = mc.GTN_ZeroPos(cardNo, axis, 1);//手动位置清零，若未完全到位可能会发现编码器位置回零后不为0，需要确保轴停止
        }

        #endregion

        #region 核2回原点

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="axis"></param>
        /// <param name="mode"></param>
        /// <param name="moveDir"></param>
        /// <param name="edge"></param>
        /// <param name="velHigh"></param>
        /// <param name="velLow"></param>
        /// <param name="escapeStep"></param>
        public void SmartHome2(short cardNo, short axis, short mode, short moveDir, short edge, double velHigh, double velLow, int escapeStep)
        {
            short sRtn;
            int pStatus;
            uint pClock;
            mc.THomeStatus tHomeStatus;
            mc.THomePrm tHomePrm;

            //回原点参数
            tHomePrm.mode = mode;//归零模式
            tHomePrm.moveDir = moveDir;//归零方向
            tHomePrm.edge = 0;// 设置捕获沿：0-下降沿，非0值-上升沿
            tHomePrm.acc = 0.2;//加速度
            tHomePrm.dec = 0.2;//减速度
            tHomePrm.velHigh = velHigh;//最高速度
            tHomePrm.velLow = velLow;//最低速度
            tHomePrm.smoothTime = 0; // 回原点运动的平滑时间：取值[0,1)，具体含义与GTS系列
            tHomePrm.searchHomeDistance = 0;//home搜索距离 0 默认为 805306368
            tHomePrm.searchIndexDistance = 0;//index搜索距离 0 默认为 805306368
            tHomePrm.escapeStep = escapeStep;//限位回零时反向脱离距离

            //回零前先确认点位或jog运动规划和实际位置是否一致，包括方向和大小
            sRtn = mc.GTN_Stop(cardNo, 1 << (axis - 1), 0);
            sRtn = mc.GTN_ClrSts(cardNo, axis, 1);//清除状态，回零前若在原点开关触发位置需要先移开在启动回零
            sRtn = mc.GTN_ZeroPos(cardNo, axis, 1);//清除位置保证规划位置和实际位置一致            
            sRtn = mc.GTN_GetHomePrm(cardNo, axis, out tHomePrm);//可省略  
            sRtn = mc.GTN_GoHome(cardNo, axis, ref tHomePrm);//启动SmartHome回原点
            do
            {
                sRtn = mc.GTN_GetHomeStatus(cardNo, axis, out tHomeStatus);//获取回原点状态
            } while (tHomeStatus.run == 1); // 等待搜索原点停止
            do
            {
                sRtn = mc.GTN_GetSts(cardNo, axis, out pStatus, 1, out pClock);//等待到位完成
            } while ((pStatus & 0x0800) == 1);  //等待搜索原点停止、规划运动停止位判断
            //while ((pStatus& 0x0800) == 0) 软件内部判断到位信号，需要使用GTN_SetAxisBand()指令（螺距误差补偿功能补偿值包含在到位误差带内）
            //也可以使用GTN_GetDi指令读取驱动器到位信号来判断
            int delay = 1000;
            Thread.Sleep(delay);    //延时确保完全到位

            sRtn = mc.GTN_ZeroPos(cardNo, axis, 1);//手动位置清零，若未完全到位可能会发现编码器位置回零后不为0，需要确保轴停止
        }

        #endregion

        #region 建立坐标系1

        /// <summary>
        /// 建立坐标系
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="crd">坐标系号1，2</param>
        /// <param name="axisNo">坐标系各维度对应轴号</param>
        /// <param name="maxVel">坐标系最大合成速度</param>
        /// <param name="maxAcc">坐标系最大合成加速度</param>
        public void SetCoordinate1(short cardNo, short crd, short[] axisNo)
        {
            short sRtn = 0;
            short[] demin = new short[8];//表示1-8轴的数组
            for (short i = 0; i < axisNo.Length; i++)
            {
                demin[axisNo[i]] = (short)(i + 1);//规划器与坐标系对应；
            }
            mc.TCrdPrm crdPrm = new mc.TCrdPrm();
            crdPrm.dimension = 2;                        // 建立二维的坐标系
            crdPrm.synVelMax = 3000;                      // 坐标系的最大合成速度是: 500 pulse/ms
            crdPrm.synAccMax = 10;                        // 坐标系的最大合成加速度是: 2 pulse/ms^2
            crdPrm.evenTime = 0;                         // 坐标系的最小匀速时间为0
            crdPrm.profile1 = demin[0];                         // 规划器1对应到X轴                       
            crdPrm.profile2 = demin[1];                         // 规划器2对应到Y轴
            crdPrm.profile3 = demin[2];
            crdPrm.profile4 = demin[3];
            crdPrm.profile5 = demin[4];
            crdPrm.profile6 = demin[5];
            crdPrm.profile7 = demin[6];                         // 若profile7为1，规划器7对应到X轴           
            crdPrm.profile8 = demin[7];
            crdPrm.setOriginFlag = 1;                    // 需要设置加工坐标系原点位置
            crdPrm.originPos1 = 0;                       // 加工坐标系原点位置在(0,0)，即与机床坐标系原点重合
            crdPrm.originPos2 = 0;
            crdPrm.originPos3 = 0;
            crdPrm.originPos4 = 0;
            crdPrm.originPos5 = 0;
            crdPrm.originPos6 = 0;
            crdPrm.originPos7 = 0;
            crdPrm.originPos8 = 0;
            sRtn += mc.GTN_SetCrdPrm(cardNo, crd, ref crdPrm);   //必须在轴调用停止状态下,重建坐标系会清除原有数据
        }

        #endregion

        #region 建立坐标系2

        /// <summary>
        /// 建立坐标系
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="crd">坐标系号1，2</param>
        /// <param name="axisNo">坐标系各维度对应轴号</param>
        /// <param name="maxVel">坐标系最大合成速度</param>
        /// <param name="maxAcc">坐标系最大合成加速度</param>
        public void SetCoordinate2(short cardNo, short crd, short[] axisNo)
        {
            short sRtn = 0;
            sRtn = mc.GTN_SetCrdMapBase(1, crd, 9); //第 9 个规划器作为插补坐标系映射基础轴
            mc.TCrdPrm crdPrm;
            sRtn = mc.GTN_GetCrdPrm(1, crd, out crdPrm);

            //crdPrm.dimension = 4;
            //crdPrm.profile[0] = 1; //规划器 9 作为插补坐标系的 X 轴
            //crdPrm.profile[1] = 2; //规划器 10 作为插补坐标系的 Y 轴
            //crdPrm.profile[2] = 3; //规划器 11 作为插补坐标系的 Z 轴
            //crdPrm.profile[3] = 4; //规划器 12 作为插补坐标系的 A 轴

            short[] demin = new short[4];//表示9-12轴的数组
            for (short i = 0; i < axisNo.Length; i++)
            {
                demin[axisNo[i]] = (short)(i + 1);//规划器与坐标系对应；
            }
            //mc.TCrdPrm crdPrm = new mc.TCrdPrm();
            crdPrm.dimension = 2;                        // 建立二维的坐标系
            crdPrm.synVelMax = 3000;                      // 坐标系的最大合成速度是: 500 pulse/ms
            crdPrm.synAccMax = 10;                        // 坐标系的最大合成加速度是: 2 pulse/ms^2
            crdPrm.evenTime = 0;                         // 坐标系的最小匀速时间为0
            crdPrm.profile1 = demin[0];                         // 规划器1对应到X轴                       
            crdPrm.profile2 = demin[1];                         // 规划器2对应到Y轴
            crdPrm.profile3 = demin[2];
            crdPrm.profile4 = demin[3];

            crdPrm.setOriginFlag = 1;                    // 需要设置加工坐标系原点位置
            crdPrm.originPos1 = 0;                       // 加工坐标系原点位置在(0,0)，即与机床坐标系原点重合
            crdPrm.originPos2 = 0;
            crdPrm.originPos3 = 0;

            sRtn += mc.GTN_SetCrdPrm(cardNo, crd, ref crdPrm);   //必须在轴调用停止状态下,重建坐标系会清除原有数据
        }

        #endregion

        #region 直线插补运动1
        public void Line1Move1(short[] axisNo, short axisNo1, short axisNo2, double Vel, double Acc, int XPos, int YPos)
        {
            short run;
            int segment;
            int space;
            int pStatus;//轴状态判断
            short sRtn;//返回值
            //short[] axisNo = new short[]{ 0, 5 };
            //SetCoordinate(short cardNo, short crd, short[] axisNo, double maxVel, double maxAcc)
            SetCoordinate1(1, 1, axisNo);
            mc.GTN_ClrSts(1, 1, 10);//qinchu zhuangtai
            sRtn = mc.GTN_CrdClear(1, 1, 0);
            sRtn = mc.GTN_LnXY(1, 1, XPos, YPos, Vel, Acc, 0, 0);
            sRtn = mc.GTN_CrdSpace(1, 1, out space, 0);
            sRtn = mc.GTN_CrdStart(1, 1, 0);

            // 等待运动完成
            sRtn = mc.GTN_CrdStatus(1, 1, out run, out segment, 0);
            //do
            //{
            //    // 查询坐标系1的FIFO的插补运动状态
            //    sRtn = mc.GTN_CrdStatus(
            //    1,
            //    1, // 坐标系是坐标系1
            //    out run, // 读取插补运动状态
            //    out segment, // 读取当前已经完成的插补段数
            //    0); // 查询坐标系1的FIFO0缓存区
            //    // 坐标系在运动, 查询到的run的值为1
            //} while (run == 1 || segment == 0);

            //do
            //{
            //    sRtn = mc.GTN_GetSts(1, (short)(axisNo1+1), out pStatus, 1, out pClock);//等待到位完成
            //} while ((pStatus & 0x0800) == 0 );  //等待规划运动停止位判断

            //do
            //{
            //    sRtn = mc.GTN_GetSts(1, (short)(axisNo2 + 1), out pStatus, 1, out pClock);//等待到位完成
            //} while ((pStatus & 0x0800) == 0);  //等待规划运动停止位判断

        }



        #endregion

        #region 直线插补运动2
        public void Line1Move2(short[] axisNo, short axisNo1, short axisNo2, double Vel, double Acc, int XPos, int YPos)
        {
            short run;
            int segment;
            int space;
            int pStatus;//轴状态判断
            short sRtn;//返回值
            //short[] axisNo = new short[] { 0, 2 };
            //SetCoordinate(short cardNo, short crd, short[] axisNo, double maxVel, double maxAcc)
            SetCoordinate2(1, 2, axisNo);

            sRtn = mc.GTN_CrdClear(1, 2, 0);
            sRtn = mc.GTN_LnXY(1, 2, XPos, YPos, Vel, Acc, 0, 0);
            sRtn = mc.GTN_CrdSpace(1, 2, out space, 0);
            sRtn = mc.GTN_CrdStart(1, 2, 0);

            //等待运动完成
            sRtn = mc.GTN_CrdStatus(1, 2, out run, out segment, 0);
            //do
            //{
            //    // 查询坐标系1的FIFO的插补运动状态
            //    sRtn = mc.GTN_CrdStatus(
            //    1,
            //    2, // 坐标系是坐标系1
            //    out run, // 读取插补运动状态
            //    out segment, // 读取当前已经完成的插补段数
            //    0); // 查询坐标系1的FIFO0缓存区
            //    // 坐标系在运动, 查询到的run的值为1
            //} while (run == 1 || segment == 0);

            //do
            //{
            //    sRtn = mc.GTN_GetSts(1, (short)(axisNo1 + 1), out pStatus, 1, out pClock);//等待到位完成
            //} while ((pStatus & 0x0800) == 0);  //等待规划运动停止位判断

            //do
            //{
            //    sRtn = mc.GTN_GetSts(1, (short)(axisNo2 + 1), out pStatus, 1, out pClock);//等待到位完成
            //} while ((pStatus & 0x0800) == 0);  //等待规划运动停止位判断

        }



        #endregion

        #region 两轴同时运动

        public void DoubleAxisMove(short cardNo, short axisNo1, short axisNo2, double acc, double dec, double velStart, short smoothTime, double Vel1, double Vel2, int Pos1, int Pos2)
        {
            int pStatus;//轴状态判断
            short sRtn;//返回值
            mc.TTrapPrm trap1;
            mc.TTrapPrm trap2;
            sRtn = mc.GTN_PrfTrap(cardNo, axisNo1);     //设置为点位运动，模式切换需要停止轴运动。          
            //若返回值为 1：若当前轴在规划运动，请调用GT_Stop停止运动再调用该指令。
            sRtn = mc.GTN_GetTrapPrm(cardNo, axisNo1, out trap1);
            /*读取点位运动参数（不一定需要）。若返回值为 1：请检查当前轴是否为 Trap 模式  
             * 若不是，请先调用 GT_PrfTrap 将当前轴设置为 Trap 模式。*/

            trap1.acc = acc;              //单位pulse/ms2
            trap1.dec = dec;              //单位pulse/ms2
            trap1.velStart = velStart;           //起跳速度，默认为0。
            trap1.smoothTime = smoothTime;       //平滑时间，使加减速更为平滑。范围[0,50]单位ms。

            sRtn = mc.GTN_SetTrapPrm(cardNo, axisNo1, ref trap1);//设置点位运动参数。
            sRtn = mc.GTN_SetVel(cardNo, axisNo1, Vel1);        //设置目标速度
            sRtn = mc.GTN_SetPos(cardNo, axisNo1, Pos1);       //设置目标位置
            sRtn = mc.GTN_Update(cardNo, 1 << (axisNo1 - 1));   //更新轴运动


            sRtn = mc.GTN_PrfTrap(cardNo, axisNo2);     //设置为点位运动，模式切换需要停止轴运动。
            //若返回值为 1：若当前轴在规划运动，请调用GT_Stop停止运动再调用该指令。
            sRtn = mc.GTN_GetTrapPrm(cardNo, axisNo2, out trap2);
            /*读取点位运动参数（不一定需要）。若返回值为 1：请检查当前轴是否为 Trap 模式  
             * 若不是，请先调用 GT_PrfTrap 将当前轴设置为 Trap 模式。*/

            trap2.acc = acc;              //单位pulse/ms2
            trap2.dec = dec;              //单位pulse/ms2
            trap2.velStart = velStart;           //起跳速度，默认为0。
            trap2.smoothTime = smoothTime;       //平滑时间，使加减速更为平滑。范围[0,50]单位ms。

            sRtn = mc.GTN_SetTrapPrm(cardNo, axisNo2, ref trap2);//设置点位运动参数。
            sRtn = mc.GTN_SetVel(cardNo, axisNo2, Vel2);        //设置目标速度
            sRtn = mc.GTN_SetPos(cardNo, axisNo2, Pos2);       //设置目标位置
            sRtn = mc.GTN_Update(cardNo, 1 << (axisNo2 - 1));   //更新轴运动

            do
            {
                sRtn = mc.GTN_GetSts(2, axisNo1, out pStatus, 1, out pClock);//等待到位完成
            } while ((pStatus & 0x0800) == 0);  //等待规划运动停止位判断

            do
            {
                sRtn = mc.GTN_GetSts(2, axisNo2, out pStatus, 1, out pClock);//等待到位完成
            } while ((pStatus & 0x0800) == 0);  //等待规划运动停止位判断

        }


        #endregion








    }
}
