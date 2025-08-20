using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QM9505
{
    public class Function
    {
        private object threadLock;
        double pValue;     //返回值
        uint pClock;       //时钟信号       
        public Function()
        {
            threadLock = new object();
        }

        public void ReadIO()
        {
            while (true)
            {
                InSenser();
                OutSenser();

                #region 读取IO刷新

                //读取板卡上输入点
                //mc.GTN_GetDiEx(1, mc.MC_GPI, out Variable.XValue_0[0], 3);//主卡通用输入(核1)16,16,16
                //mc.GTN_GetDi(2, mc.MC_GPI, out Variable.XValue_1[0]);      //主卡通用输入(核2)    16
                mc.GTN_GetExtDi(1, 1, out Variable.XValue[0]);          //通用输入(扩展卡1-32)    32
                mc.GTN_GetExtDi(1, 33, out Variable.XValue[1]);         //通用输入(扩展卡33-64)   32
                mc.GTN_GetExtDi(1, 65, out Variable.XValue[2]);         //通用输入(扩展卡65-96)   32
                mc.GTN_GetExtDi(1, 97, out Variable.XValue[3]);         //通用输入(扩展卡97-128)  32

                //读取板卡上输出点
                //mc.GTN_GetDo(1, mc.MC_GPO, out Variable.YValue_0[0]);//主卡通用输出(核1)  10,10,10
                //mc.GTN_GetDo(2, mc.MC_GPO, out Variable.YValue_1[0]);//主卡通用输出(核2)       10
                mc.GTN_GetExtDo(1, 1, out Variable.YValue[0]);       //通用输出(扩展卡1-32)    32
                mc.GTN_GetExtDo(1, 33, out Variable.YValue[1]);      //通用输出(扩展卡33-64)   32
                mc.GTN_GetExtDo(1, 65, out Variable.YValue[2]);      //通用输出(扩展卡65-96)   32
                mc.GTN_GetExtDo(1, 97, out Variable.YValue[3]);      //通用输出(扩展卡97-128)  32
                mc.GTN_GetExtDo(1, 129, out Variable.YValue[4]);      //通用输出(扩展卡129-160)  32             
                #endregion

                Thread.Sleep(1);
            }
        }

        #region 输入信号
        public void InSenser()
        {
            for (int b = 0; b < 3; b++)//模块输入信息
            {
                for (int a = 0; a < 32; a++)
                {
                    if (b <=1)
                    {
                        Variable.XStatus[a] = GetBit1(IntToBin32(Variable.XValue[0], a));
                    }
                    //else if (b == 1)
                    //{
                    //    Variable.XStatus[16 + a] = GetBit1(IntToBin32(Variable.XValue[b], a));
                    //}
                    else //if (b == 2)
                    {
                        Variable.XStatus[32+ a] = GetBit1(IntToBin32(Variable.XValue[1], a));
                    }          
                    //if (b == 0)//X000--X031
                    //{
                    //    if (a >= 16 && a < 28)
                    //    {
                    //        Variable.XStatus[(b + 1) + a] = !GetBit1(IntToBin32(Variable.XValue[b], a));
                    //    }
                    //    else
                    //    {
                    //        Variable.XStatus[(b + 1) + a] = GetBit1(IntToBin32(Variable.XValue[b], a));
                    //    }
                    //}
                    //else if (b == 1)//X032--X063
                    //{
                    //    if (a < 16)
                    //    {
                    //        if ((a >= 4 && a < 8) || (a >= 22 && a < 27) || a == 31)
                    //        {
                    //            Variable.XStatus[(b + 2) + a] = !GetBit1(IntToBin32(Variable.XValue[b], a));
                    //        }
                    //        else
                    //        {
                    //            Variable.XStatus[(b + 2) + a] = GetBit1(IntToBin32(Variable.XValue[b], a));
                    //        }
                    //    }
                    //}                   
                }
            }
        }

        #endregion

        #region 输出信号
        public void OutSenser()
        {
            for (int b = 0; b < 3; b++)//模块输入信息
            {
                for (int a = 0; a < 32; a++)
                {
                    if (b <= 1)
                    {
                        Variable.YStatus[a] = GetBit1(IntToBin32(Variable.YValue[0], a));
                    }
                    else
                    {
                        Variable.YStatus[32 + a] = GetBit1(IntToBin32(Variable.YValue[1], a));
                    }
                    //if (b == 0)//Y000--Y031
                    //{
                    //    Variable.YStatus[32 * b + 10 + a] = GetBit1(IntToBin32(Variable.YValue[b], a));
                    //}
                    //else if (b == 1)//Y032--Y063
                    //{
                    //    Variable.YStatus[32 * b + 10 + a] = GetBit1(IntToBin32(Variable.YValue[b], a));
                    //}
                }
            }           
        }

        #endregion

        //#region 输出动作   

        //public void OutYON(int data)
        //{
        //    if (data >= 0 && data < 48)
        //    {
        //        //mc.GTN_SetExtDoBit(1, (short)(21), 1);
        //        mc.GTN_SetExtDoBit(1, (short)(data), 1);
        //    }

        //    Variable.OnEnable[data] = true;
        //    Variable.OffEnable[data] = false;
        //    Variable.OnTime[data] = 0;

        //}
        
        //public void OutYOFF(int data)
        //{
        //    if (data >= 0 && data < 48)
        //    {
              
        //        mc.GTN_SetExtDoBit(1, (short)(data), 0);
        //    }

        //    Variable.OffEnable[data] = true;
        //    Variable.OnEnable[data] = false;
        //    Variable.OffTime[data] = 0;
        //}
        //#endregion

        #region 读取IO值转Bool

        public static bool GetBit0(string data)
        {
            if (data == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GetBit1(string data)
        {
            if (data == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 将整数转换二进制
        /// <summary>
        /// 将整数转换二进制
        /// </summary>
        /// <param name="data">整数值</param>
        /// <param name="a">截取二进制位置</param>
        /// <returns></returns>
        public string IntToBin32(int data, int a)
        {
            lock (threadLock)
            {
                string cnt = "0";//记录转换过后二进制值
                try
                {
                    cnt = Convert.ToString(data, 2);
                    if (cnt.Length < 32)
                    {
                        int c = cnt.Length;
                        for (int b = 0; b < (32 - c); b++)
                        {
                            cnt = "0" + cnt;
                        }
                    }
                }
                catch
                {

                }
                return cnt.Substring(31 - a, 1);
            }
        }

        public string IntToBin16(int data, int a)
        {
            lock (threadLock)
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
        }

        #endregion




    }
}
