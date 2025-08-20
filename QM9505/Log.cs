using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM9505
{
    internal class Log
    {
        private static object objComm = new object();
        private static object objData = new object();
        private static object objOperate = new object();
        private static object objError = new object();
        private static object objAlarm = new object();
        private static object objMessage = new object();

        private static string dataPath = "D:\\QM Pro";


        internal static void SaveLog(Enum logType, string message)
        {
            string str = logType.ToString();
            try
            {
                switch (str)
                {
                    case "Comm":
                        lock (objComm)
                        {
                            DateTime now = DateTime.Now;
                            string filePath = string.Format("{0}\\Log\\Comm\\{1}\\", dataPath, now.ToString("yyyyMMdd"));
                            string fileName = now.ToString("HH时") + ".txt";         
                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);
                            if (!File.Exists(filePath + fileName))
                                File.Create(filePath + fileName).Close();
                            File.AppendAllText(filePath + fileName, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss    ") + message + Environment.NewLine);
                        }
                        break;
                    case "Operate":
                        lock (objOperate)
                        {
                            DateTime now1 = DateTime.Now;
                            string filePath1 = string.Format("{0}\\Log\\Operate\\", dataPath);
                            string fileName1 = now1.ToString("yy_MM_dd") + ".txt";
                            if (!Directory.Exists(filePath1))
                                Directory.CreateDirectory(filePath1);
                            if (!File.Exists(filePath1 + fileName1))
                                File.Create(filePath1 + fileName1).Close();
                            File.AppendAllText(filePath1 + fileName1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss    ") + Variable.userEnter.ToString() + "    " + message + Environment.NewLine);
                        }
                        break;
                    case "Error":
                        lock (objError)
                        {
                            DateTime now = DateTime.Now;
                            string filePath = string.Format("{0}\\Log\\Error\\{1}\\", dataPath, now.ToString("yyyyMMdd"));
                            string fileName = now.ToString("HH时") + ".txt";         
                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);
                            if (!File.Exists(filePath + fileName))
                                File.Create(filePath + fileName).Close();
                            File.AppendAllText(filePath + fileName, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss    ") + message + Environment.NewLine);
                        }
                        break;
                    case "Alarm":
                        lock (objAlarm)
                        {
                            DateTime now = DateTime.Now;
                            string filePath = string.Format("{0}\\Log\\Alarm\\{1}\\", dataPath, now.ToString("yyyyMMdd"));
                            string fileName = now.ToString("HH时") + ".txt";         
                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);
                            if (!File.Exists(filePath + fileName))
                                File.Create(filePath + fileName).Close();
                            File.AppendAllText(filePath + fileName, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss    ") + message + Environment.NewLine);
                        }
                        break;
                    case "Message":
                        lock (objMessage)
                        {
                            DateTime now = DateTime.Now;
                            string filePath = string.Format("{0}\\Log\\Message\\{1}\\", dataPath, now.ToString("yyyyMMdd"));
                            string fileName = now.ToString("HH时") + ".txt";         
                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);
                            if (!File.Exists(filePath + fileName))
                                File.Create(filePath + fileName).Close();
                            File.AppendAllText(filePath + fileName, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss    ") + message + Environment.NewLine);
                        }
                        break;
                    case "Data":
                        lock (objData)
                        {
                            //此处待添加
                        }
                        break;
                }
            }
            catch (Exception es)
            {
                //System.Windows.Forms.MessageBox.Show("存储日志文件异常\r\n" + es.ToString(), "提示：");
            }
        }


        internal static void SaveError(StackTrace tmpST, StackFrame tmpSF, Exception ex)
        {
            try
            {
                lock (objError)
                {
                    tmpSF = tmpST.GetFrame(0);
                    string rowNo = "";
                    if (ex.ToString().Contains("行号") || ex.ToString().Contains("line"))
                    {
                        string temp = ex.ToString();
                        if (ex.ToString().Contains("行号"))
                        {
                            int index = temp.LastIndexOf(" ");
                            rowNo = temp.Substring(index + 1, temp.Length - index - 1);
                        }
                        else
                        {
                            rowNo = ex.ToString().Split(new string[] { "line" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        }
                    }
                    else
                    {
                        rowNo = "未知";
                    }
                    string data = "----------     " + DateTime.Now.ToString() + "     -----------------------------------------------------" + Environment.NewLine +
                                  "原始数据：" + ex.ToString() + Environment.NewLine +
                                  "出错文件：" + tmpSF.GetFileName() + Environment.NewLine +
                                  "出错函数：" + tmpSF.GetMethod().Name + Environment.NewLine +
                                  "出错行号：" + rowNo + Environment.NewLine +
                                  "出错列号：" + tmpSF.GetFileColumnNumber() + Environment.NewLine +
                                  "出错信息：" + ex.Message.ToString() + Environment.NewLine + Environment.NewLine;

                    data += Environment.NewLine;
                    string ErrorPath = dataPath + "\\Log\\Error";
                    if (!Directory.Exists(ErrorPath))
                    {
                        Directory.CreateDirectory(ErrorPath);
                    }
                    string path = ErrorPath + "\\" + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
                    if (!File.Exists(path))
                    {
                        File.Create(path).Close();
                    }
                    File.AppendAllText(path, data);
                    //System.Windows.Forms.MessageBox.Show(ex.ToString(), "提示：");
                }
            }
            catch (Exception es)
            {
                //System.Windows.Forms.MessageBox.Show("存储日志文件异常\r\n" + es.ToString(), "提示：");
            }
        }
    }


    internal enum LogType
    {
        Comm,
        Error,
        Alarm,
        Operate,
        Message,
        Data,
    }


}
