using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace QM9505
{
    public class INIHelper
    {
        //String appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);//相对路径
        public static string IOFileName = Application.StartupPath + "\\IO.ini";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);//写入ini配置文件
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);//读取ini配置文件


        /// <summary>
        /// 将数据写入ini配置文件里面
        /// </summary>
        /// <param name="section">段落名</param>
        /// <param name="key">键名</param>
        /// <param name="val">写入值</param>
        /// <param name="filename">ini文件完整路径</param>
        /// <returns></returns>
        public void writeIni(string section, string key, string val, string filename)
        {
            WritePrivateProfileString(section, key, val, filename);
        }


        /// <summary>
        /// 读取ini配置文件里面数据
        /// </summary>
        /// <param name="section">段落名</param>
        /// <param name="key">键名</param>
        /// <param name="def">写入值</param>
        /// <param name="filename">ini文件完整路径</param>
        /// <returns></returns>
        public string getIni(string section, string key, string def, string filename)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, def, sb, 1024, filename);
            return sb.ToString();
        }


        #region 写入Parameter INI文件

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section">节点名称[如[TypeName]]</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, byte[] key, byte[] val, string filepath);

        /// <summary>
        /// 写入INI文件中的内容方法
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key">键</param>
        /// <param name="iValue">值</param>
        public bool IOWriteContentValue(string section, string key, string value)
        {
            try
            {
                WritePrivateProfileString(section, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(value), IOFileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 读取Parameter INI文件

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键</param>
        /// <param name="def">值</param>
        /// <param name="retval">stringbulider对象</param>
        /// <param name="size">字节大小</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

        /// <summary>
        /// 读取INI文件中的内容方法
        /// </summary>
        /// <param name="Section">节点名称</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public string IOReadContentValue(string Section, string key)
        {
            try
            {
                byte[] Buffer = new byte[1024];
                int bufLen = GetPrivateProfileString(Section, key, "", Buffer, Buffer.GetUpperBound(0), IOFileName);
                string s = Encoding.UTF8.GetString(Buffer, 0, bufLen);//使用utf-8编码读取 解决乱码问题             
                return s;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion


    }
}
