using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    class UDPServer
    {
        Socket server;
        Thread th;
        private Boolean udpFlag;

        #region 链接

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="serverPoint"></param>
        /// <returns></returns>
        public bool SocketStart(string ip, string serverPoint)
        {
            try
            {
                ////-----UDP1-----
                ////1.创建服务器端udp
                //server1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                ////2.获取IP
                //IPAddress iP1 = IPAddress.Parse(ip.Text);//Parse()固定IP转换。
                //IPEndPoint Point1 = new IPEndPoint(iP1, int.Parse(serverPoint.Text));
                ////3.绑定端口号和IP
                //server1.Bind(Point1);


                //-----UDP1-----
                //1.创建服务器端udp
                server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //2.获取IP
                IPAddress iP1 = IPAddress.Parse(ip);//Parse()固定IP转换。
                IPEndPoint Point1 = new IPEndPoint(iP1, int.Parse(serverPoint));
                //3.绑定端口号和IP
                server.Bind(Point1);

                //开启接收消息线程
                th = new Thread(ReciveMsg);
                th.IsBackground = true;
                th.Start();

                return true;
            }
            catch
            {
                return false;
            }

        }

        #endregion

        #region 关闭连接
        public void SocketStop()
        {
            udpFlag = false;
            server.Close();

            try
            {
                th.Abort(); //强制关闭线程
            }
            catch
            {

            }
        }
        #endregion

        #region 发送数据

        //-----udp1-----
        /// <summary>
        /// 向特定客户端ip的主机的端口发送数据
        /// </summary>te
        public void SendMsg(string IP, string Point, string message)
        {
            if (udpFlag)
            {
                EndPoint point = new IPEndPoint(IPAddress.Parse(IP), int.Parse(Point));
                byte[] buffer = StrToHexByte(message);//将消息内容转换层字节数组
                List<byte> list = new List<byte>();
                //集合中添加数组
                list.AddRange(buffer);
                //集合转化为数组
                byte[] newBuffer = list.ToArray();
                //发送消息，格式为字节数组
                server.SendTo(newBuffer, point);
            }
        }

        #endregion

        #region 接收数据

        /// <summary>
        /// 接收发送给本机ip对应端口号的数据
        /// </summary>
        public void ReciveMsg()
        {
            while (true)
            {
                if (Variable.UpAutoStep1 >= 20 || Variable.UpAutoStep2 >= 20 || Variable.DownAutoStep1 >= 20 || Variable.DownAutoStep2 >= 20)
                {
                    try
                    {
                        udpFlag = true;
                        EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                        byte[] buffer = new byte[1024 * 1024];
                        int length = server.ReceiveFrom(buffer, ref point);//接收数据报
                        byte[] tt = buffer.Skip(0).Take(length).ToArray();
                        string message = ByteArrayToHexString(tt);
                        Variable.receiveMessage = "";
                        Variable.receiveMessage = Variable.receiveRecord = point.ToString() + ":" + message;
                    }
                    catch
                    {

                    }
                }
                Thread.Sleep(1);
            }
        }

        #endregion


        #region 将16进制的字符串转为byte[]
        /// <summary>
        /// 将16进制的字符串转为byte[]
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        #endregion

        #region 将byte[]转为16进制的字符串
        /// <summary>
        /// 将byte[]转为16进制的字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();
        }
        #endregion

        #region 去除后面多余的零
        /// <summary>
        /// 去除后面多余的零
        /// </summary>
        /// <param name="dValue"></param>
        /// <returns></returns>
        private string RemoveZero(string sResult)
        {
            if (sResult.IndexOf("FF") < 0)
                return sResult;
            int iIndex = sResult.Length - 1;
            for (int i = sResult.Length - 1; i >= 0; i--)
            {
                if (sResult.Substring(i, 1) != "0")
                {
                    iIndex = i;
                    break;
                }
            }
            sResult = sResult.Substring(0, iIndex + 1);
            if (sResult.EndsWith("."))
                sResult = sResult.Substring(0, sResult.Length - 1);
            return sResult;
        }
        #endregion



    }
}
