using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{

    public class Temperature
    {
        public static List<byte> buffer = new List<byte>(4096);

        //串口通讯对象
        public static SerialPort tempPort;
        //端口号默认是COM1
        //public static string portName = "COM3";
        //波特率默认是9600
        public static int baudRate = 38400;
        //接收超时时间默认为1000
        public static double timeOut = 1000;
        //数据位
        public static int dataBit = 7;

        public static string[] temp_data = new string[8];

        #region 串口初始化

        //public static void ComPort(string portName, int baudRate, int dataBit, string parity, int stop)
        //{
        //    tempPort = new SerialPort(portName);
        //    tempPort.BaudRate = baudRate;
        //    tempPort.DataBits = 7;
        //    tempPort.Parity = System.IO.Ports.Parity.Even;
        //    tempPort.ReceivedBytesThreshold = 1;
        //    tempPort.DataReceived += new SerialDataReceivedEventHandler(ScrewPort_DataReceived);
        //}

        public static void ComPort(string portName, int baudRate, int dataBit, string parity, int stop)
        {
            tempPort = new SerialPort(portName);
            tempPort.BaudRate = baudRate;
            tempPort.DataBits = dataBit;
            tempPort.DtrEnable = true;
            tempPort.RtsEnable = true;
            tempPort.Encoding = Encoding.ASCII;
            if (parity == "EVEN") { tempPort.Parity = System.IO.Ports.Parity.Even; }
            else { tempPort.Parity = System.IO.Ports.Parity.Odd; }
            if (stop == 1) { tempPort.StopBits = StopBits.One; } else { tempPort.StopBits = StopBits.Two; }
            tempPort.ReadTimeout = 1000; tempPort.WriteTimeout = 1000;
            tempPort.ReceivedBytesThreshold = 1;
            tempPort.DataReceived += new SerialDataReceivedEventHandler(ScrewPort_DataReceived);
        }
        #endregion

        #region 打开串口

        public static bool Open()
        {
            try
            {
                if (tempPort.IsOpen != true)
                {
                    tempPort.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 关闭串口

        public static void Close()
        {
            if (tempPort.IsOpen == true)
            {
                tempPort.Close();
            }
        }
        #endregion

        #region 释放托管资源

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //释放托管资源
                tempPort.Dispose();
            }
            //释放非托管资源先关闭在释放
            Close();
            tempPort.Dispose();
            if (disposing)
            {
                // 对象会被 Dispose 方法释放. 
                // 调用GC.SupressFinalize将此对象从释放队列中清除 
                // 防止终结器对此对象重复释放。 
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region 发送数据
        public bool SendData(byte[] data)
        {
            try
            {
                if (Open() == false)
                {
                    MessageBox.Show("温度控制器1串口打开失败，请检查串口是否被占用！");
                    return false;
                }
                tempPort.Write(data, 0, data.Length);
                return true;
            }
            catch
            {
                Close();
                return false;
            }
        }
        #endregion

        #region 串口返回值读取

        public static void ScrewPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Variable.Tempertest != 1)
            {
                return;
            }
            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;//跨线程访问
                Variable.Temp = "";
                string temp = "";
                int n = tempPort.BytesToRead;
                byte[] buf = new byte[n];
                tempPort.Read(buf, 0, n);
                //缓存数据
                buffer.AddRange(buf);
                //完整性判断
                int s = buffer.Count;
                while (s >= 7) //至少包含控制器号（1字节）、功能码（1字节）、数据长度（1字节）、数据值（2字节）、CRC校验值（2字节）、结束符（2字节）
                {
                    //查找数据结束符
                    if (buffer[buffer.Count - 1].ToString() == "10")//判断结束符是否为10
                    {
                        for (int i = 0; i < buffer.Count; i++)
                        {
                            temp += buffer[i].ToString("X2");
                        }

                        string temp1 = temp.Substring(14, 8);
                        string temp2 = HexToStr(temp1);
                        int temp3 = Convert.ToInt32(temp2, 16);

                        Variable.Temp = temp3.ToString();
                        s = 0;
                        buffer.RemoveRange(0, buffer.Count);
                    }
                    else//帧头不正确，记得清除
                    {
                        break;
                    }
                }
            }
            catch
            {
                MessageBox.Show("温控器1串口数据读取错误！");
            }
        }
        /*public static void ScrewPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;//跨线程访问
                                                                //Variable.Temp = "";
                string temp = "";
                int n = tempPort.BytesToRead;
                byte[] buf = new byte[n];
                tempPort.Read(buf, 0, n);
                //缓存数据
                buffer.AddRange(buf);
                //完整性判断
                int s = buffer.Count;
                while (s >= 7) //至少包含控制器号（1字节）、功能码（1字节）、数据长度（1字节）、数据值（2字节）、CRC校验值（2字节）、结束符（2字节）
                {
                    //查找数据结束符
                    if (buffer[buffer.Count - 1].ToString() == "10")//判断结束符是否为10
                    {
                        for (int i = 0; i < buffer.Count; i++)
                        {
                            temp += buffer[i].ToString("X2");
                        }
                        if (temp.Length == 86)
                        {
                            for (int a = 0; a < 8; a++)
                            {
                                string temp1 = temp.Substring(14 + a * 8, 8);
                                string temp2 = HexToStr(temp1);
                                int temp3 = Convert.ToInt16(temp2, 16);
                                temp_data[a] = temp3.ToString();
                            }
                            s = 0;
                            buffer.RemoveRange(0, buffer.Count);
                        }
                        else
                        {
                            temp = "";
                            s = 0;
                            buffer.RemoveRange(0, buffer.Count);
                            break;
                        }
                    }
                    else//帧头不正确，记得清除
                    {

                    }
                }
            }
            catch
            {
                MessageBox.Show("温控器串口数据读取错误,请手动打开串口！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
        }*/

        #endregion

        #region 将字节数组转换为十六进制字符串

        public static string HexToStr(string hStr)
        {
            hStr = hStr.Replace(" ", "");
            if (hStr.Length <= 0) return "";
            byte[] vBytes = new byte[hStr.Length / 2];
            for (int i = 0; i < hStr.Length; i += 2)
                if (!byte.TryParse(hStr.Substring(i, 2), System.Globalization.NumberStyles.HexNumber, null, out vBytes[i / 2]))
                    vBytes[i / 2] = 0;
            return ASCIIEncoding.Default.GetString(vBytes);

        }
        #endregion

        #region 将十六进制字节数组转换为十六进制字符串

        public static string HexToStr2(string str)
        {
            string str1 = str;
            List<byte> buffer = new List<byte>();
            for (int i = 0; i < str1.Length; i += 2)
            {
                string temp = str1.Substring(i, 2);
                byte value = Convert.ToByte(temp, 16);
                buffer.Add(value);
            }
            str = System.Text.Encoding.ASCII.GetString(buffer.ToArray());
            return str;
        }
        #endregion

        #region 转换十六进制字符串到字节数组
        /// <summary>
        /// 转换十六进制字符串到字节数组
        /// </summary>
        /// <param name="str">传入的16进制参数</param>
        /// <returns>返回字节数组</returns>
        public static byte[] ToByteArray(string str)
        {
            str = str.Replace(" ", "");//移除空格
            byte[] byteArray = new byte[str.Length / 2];
            for (int i = 0; i < str.Length; i += 2)
            {
                byte n = Convert.ToByte(str.Substring(i, 2), 16);
                byteArray[i / 2] = n;
            }
            return byteArray;
        }

        #endregion

        #region LRC校验计算

        public string LRC(string str)
        {
            int sum = 0;
            string hex = "";
            int len = str.Length;
            for (int i = 0; i < len; i = i + 2)//若参数len不包含发送符(:) 则i = 0,i < len
            {
                string data = str.Substring(i, 2);//转换成1字节16进制形式数据
                sum = sum + Convert.ToInt32(data, 16);//转换成10进制，然后叠加
            }
            if (sum >= 256)
            {
                sum = sum % 256;//去除溢出部分
            }
            hex = Convert.ToInt32(~sum + 1).ToString("X");//取反在加1，转换为16进制显示
            if (hex.Length > 2)
            {
                hex = hex.Substring(hex.Length - 2, 2);//取最后两位
            }

            return hex;
        }

        #endregion





























    }
}
