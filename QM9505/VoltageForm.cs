using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public partial class VoltageForm : Form
    {
        DataGrid dataGrid = new DataGrid();
        public Thread RefreshThread;
        public int formNum = 0;

        public VoltageForm()
        {
            InitializeComponent();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            formNum += 1;
            if (formNum > 1)
            {
                if (RefreshThread != null)
                {
                    RefreshThread.Abort();
                    RefreshThread = null;
                }
            }

            base.OnVisibleChanged(e);
            if (!IsHandleCreated)
            {
                this.Close();
            }
        }

        #region 窗体加载
        private void ManualForm_Load(object sender, EventArgs e)
        {
            //dataGrid初始化
            dataGrid.IniLeftModelTrayW(GridViewUV_1, 16, 19);
            dataGrid.IniLeftModelTrayW(GridViewUV_2, 16, 19);
            dataGrid.IniLeftModelTrayW(GridViewDV_1, 16, 19);
            dataGrid.IniLeftModelTrayW(GridViewDV_2, 16, 19);

            RefreshThread = new Thread(Rsh);//开始后，开新线程执行此方法
            RefreshThread.IsBackground = true;
            RefreshThread.Start();
        }

        #endregion

        #region 刷新
        public void Rsh()
        {
            while (true)
            {
                try
                {

                    //上层VCC电压更新
                    GridViewVCCVoltRefresh(GridViewUV_1, Variable.receiveUPVCCVolt1);
                    //上层VCQ电压更新
                    GridViewVCQVoltRefresh(GridViewUV_1, Variable.receiveUPVCQVolt1);
                    //上层VCC电压更新
                    GridViewVCCVoltRefresh(GridViewUV_2, Variable.receiveUPVCCVolt2);
                    //上层VCQ电压更新
                    GridViewVCQVoltRefresh(GridViewUV_2, Variable.receiveUPVCQVolt2);


                    //下层VCC电压更新
                    GridViewVCCVoltRefresh(GridViewDV_1, Variable.receiveDownVCCVolt1);
                    //下层VCQ电压更新
                    GridViewVCQVoltRefresh(GridViewDV_1, Variable.receiveDownVCQVolt1);
                    //下层VCC电压更新
                    GridViewVCCVoltRefresh(GridViewDV_2, Variable.receiveDownVCCVolt2);
                    //下层VCQ电压更新
                    GridViewVCQVoltRefresh(GridViewDV_2, Variable.receiveDownVCQVolt2);

                }
                catch
                {
                }
                Thread.Sleep(1);
            }
        }
        #endregion

        #region GridView界面电压刷新

        public void GridViewVCCVoltRefresh(DataGridView GridU, string[] data)
        {
            //Array.Reverse(data);//反转数据
            int data1 = 0;
            string data2 = "0";
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (data[i * 8 + j] != null && data[i * 8 + j] != "")
                    {
                        data1 = Convert.ToInt16(data[i * 8 + j], 16);
                        data2 = data1.ToString();
                        GridU.Rows[j * 2].Cells[i].Style.ForeColor = Color.Blue;
                        GridU.Rows[j * 2].Cells[i].Value = Convert.ToDouble(Convert.ToDouble(data2) / 10).ToString("0.0");
                        //tb.Text = Convert.ToDouble(Convert.ToDouble(data[i * 8 + j]) / 10).ToString("0.0");
                    }
                }
            }
        }

        public void GridViewVCQVoltRefresh(DataGridView GridU, string[] data)
        {
            //Array.Reverse(data);//反转数据
            int data1 = 0;
            string data2 = "0";
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (data[i * 8 + j] != null && data[i * 8 + j] != "")
                    {
                        data1 = Convert.ToInt16(data[i * 8 + j], 16);
                        data2 = data1.ToString();
                        GridU.Rows[j * 2 + 1].Cells[i].Style.ForeColor = Color.Gray;
                        GridU.Rows[j * 2 + 1].Cells[i].Value = Convert.ToDouble(Convert.ToDouble(data2) / 10).ToString("0.0");
                    }
                }
            }
        }
        #endregion

        #region 数据清空
        private void U_1btn_Click(object sender, EventArgs e)
        {

        }

        private void U_2btn_Click(object sender, EventArgs e)
        {

        }

        private void D_1btn_Click(object sender, EventArgs e)
        {

        }

        private void D_2btn_Click(object sender, EventArgs e)
        {

        }
        #endregion


    }
}
