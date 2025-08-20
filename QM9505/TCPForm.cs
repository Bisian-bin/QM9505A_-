using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public partial class TCPForm : Form
    {
        Thread refreshThread;
        public int formNum;

        public TCPForm()
        {
            InitializeComponent();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            formNum += 1;
            if (formNum > 1)
            {
                if (refreshThread != null)
                {
                    refreshThread.Abort();
                    refreshThread = null;
                }
            }

            base.OnVisibleChanged(e);
            if (!IsHandleCreated)
            {
                this.Close();
            }
        }

        #region 窗体加载
        private void TCPForm_Load(object sender, EventArgs e)
        {
            refreshThread = new Thread(Rsh);//开始后，开新线程执行此方法
            refreshThread.IsBackground = true;
            refreshThread.Start();
        }

        #endregion

        #region 刷新
        public void Rsh()
        {
            while (true)
            {
                if (this.IsDisposed)
                {
                    return;
                }

                //通讯记录
                if (Variable.receiveRecord != null && Variable.receiveRecord != "")
                {
                    CommRecord.Items.Insert(0, Variable.receiveMessage);
                    Variable.receiveRecord = "";
                }

                Thread.Sleep(200);
            }
        }

        #endregion

    }
}
