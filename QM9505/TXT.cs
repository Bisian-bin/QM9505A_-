using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public class TXT
    {
        private object threadLock;
        public TXT()
        {
            threadLock = new object();
        }

        #region 从TXT读取数据

        public string[] ReadTXT(string path1)
        {
            lock (threadLock)
            {
                string path = path1 + ".txt";

                // 创建泛型列表
                List<string> list = new List<string>();
                // 打开数据文件 E:\data.txt逐行读入
                if (File.Exists(path))
                {
                    StreamReader rd = File.OpenText(path);
                    string line;
                    while ((line = rd.ReadLine()) != null)
                    {
                        string[] data = line.Split(",".ToCharArray());//按逗号讲数据分割成一个数组
                        list.AddRange(data);
                    }
                    // 关闭文件
                    rd.Close();

                    // 将泛型列表转换成数组
                }
                else
                {
                    //list.AddRange(listintial());

                }
                return list.ToArray();
            }
        }

        public string[] ReadTXT1(string path)
        {
            lock (threadLock)
            {
                // 创建泛型列表
                List<string> list = new List<string>();
                // 打开数据文件 E:\data.txt逐行读入
                if (File.Exists(path))
                {
                    StreamReader rd = File.OpenText(path);
                    string line;
                    while ((line = rd.ReadLine()) != null)
                    {
                        string[] data = line.Split(",".ToCharArray());//按逗号讲数据分割成一个数组
                        list.AddRange(data);
                    }
                    // 关闭文件
                    rd.Close();

                    // 将泛型列表转换成数组
                }
                else
                {
                    //list.AddRange(listintial());

                }
                return list.ToArray();
            }
        }

        #endregion

        #region 向TXT写入数据
        public void WriteTxtVariable(string[] log, string path1)
        {
            lock (threadLock)
            {
                string str = null;
                string path = path1 + ".txt";
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamWriter wr = null;
                wr = new StreamWriter(fs);

                for (int i = 0; i < log.Length; i++)
                {
                    if (!(log[i] == null))
                    {
                        if (i < log.Length - 1)
                        {
                            str += log[i] + ",";
                        }
                        else
                        {
                            str += log[i];
                        }
                    }
                }
                wr.WriteLine(str);
                wr.Flush();
                wr.Close();
            }
        }
        #endregion

        #region 向TXT写入Tray数据

        public void WriteTxt(string[] log, string path1)
        {
            lock (threadLock)
            {
                string[] data = new string[log.Length];
                Array.Copy(log, 0, data, 0, log.Length);
                Array.Reverse(data);//反转数据
                string str = null;
                string path = path1 + ".txt";
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamWriter wr = null;
                wr = new StreamWriter(fs);

                for (int i = 0; i < data.Length; i++)
                {
                    if (Variable.testTimeOut != "998")
                    {
                        str += data[i] + ",";
                    }
                    else
                    {
                        str += "20" + ",";
                    }

                }

                wr.WriteLine(str);
                wr.Flush();
                wr.Close();
            }
        }
        #endregion

        #region 读取TXT文件名

        public string[] ReadFileName(string path)
        {
            lock (threadLock)
            {
                string[] name = Directory.GetFiles(path);
                return name;
            }
        }

        #endregion

        #region 写入TXT文件名

        public void WriteFileName(string path1, string path2, string path3)
        {
            lock (threadLock)
            {
                string path = @"D:\" + path1 + "\\";
                string resultData = path + path2 + @"\" + path3 + ".txt";
                foreach (string file in Directory.GetFileSystemEntries(path))
                {
                    if (!File.Exists(file))
                    {
                        FileStream fs = new FileStream(resultData, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                        fs.Flush();
                        fs.Close();
                    }
                }
            }
        }

        #endregion

        #region 删除txt文档
        public void DeleteTXT(string path1, string name)
        {
            lock (threadLock)
            {
                string path = @"D:\" + path1;
                foreach (string file in Directory.GetFileSystemEntries(path))
                {
                    if (File.Exists(file))
                    {
                        string mamefile = Path.GetFileNameWithoutExtension(file);
                        if (mamefile.StartsWith(name))
                        {
                            File.Delete(file);
                        }
                    }
                }
            }

        }

        #endregion



    }
}
