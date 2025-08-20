using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QM9505
{
    class Helping
    {
        CancellationToken token;
        ManualResetEvent resetEvent = new ManualResetEvent(true);

        /// <summary>开始压缩内存</summary>
        /// <param name="sleepSpan">间隔，单位：秒</param>
        public void Cracker(int sleepSpan = 10)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)//这个是加法停止的判断
                    {
                        return;
                    }
                    resetEvent.WaitOne();

                    try
                    {
                        FlushMemory();
                        Thread.Sleep(TimeSpan.FromSeconds((double)sleepSpan));
                    }
                    catch (Exception ex)
                    {
                        //TODO Cracker->FlushMemory Error
                    }
                }
            });
        }

        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        private static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }

        public static void CrackerOnlyGC(int sleepSpan = 10)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        Thread.Sleep(TimeSpan.FromSeconds((double)sleepSpan));
                    }
                    catch (Exception ex)
                    {
                        //TODO Error
                    }
                }
            });
        }


    }
}
