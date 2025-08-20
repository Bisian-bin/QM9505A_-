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
    public partial class ElectricityForm : Form
    {
        DataGrid dataGrid = new DataGrid();
        TXT myTXT = new TXT();
        public Thread RefreshThread;
        public int formNum = 0;

        public ElectricityForm()
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
        private void IOForm_Load(object sender, EventArgs e)
        {
            //dataGrid初始化
            dataGrid.IniLeftModelTrayW(GridViewUA_1, 16, 19);
            dataGrid.IniLeftModelTrayW(GridViewUA_2, 16, 19);
            dataGrid.IniLeftModelTrayW(GridViewDA_1, 16, 19);
            dataGrid.IniLeftModelTrayW(GridViewDA_2, 16, 19);

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
                    //上层VCC电流更新
                    GridViewVCCElecRefresh(GridViewUA_1, Variable.receiveUPVCCElec1);
                    //上层VCC电流更新
                    GridViewVCCElecRefresh(GridViewUA_2, Variable.receiveUPVCCElec2);
                    //上层VCQ电流更新
                    GridViewVCQElecRefresh(GridViewUA_1, Variable.receiveUPVCQElec1);
                    GridViewVCQElecRefresh(GridViewUA_2, Variable.receiveUPVCQElec2);

                    //下层VCC电流更新
                    GridViewVCCElecRefresh(GridViewDA_1, Variable.receiveDownVCCElec1);
                    //下层VCC电流更新
                    GridViewVCCElecRefresh(GridViewDA_2, Variable.receiveDownVCCElec2);
                    //下层VCQ电流更新
                    GridViewVCQElecRefresh(GridViewDA_1, Variable.receiveDownVCQElec1);
                    GridViewVCQElecRefresh(GridViewDA_2, Variable.receiveDownVCQElec2);
                }
                catch
                {
                }
                Thread.Sleep(1);
            }
        }
        #endregion

        #region GridView界面电流刷新
        public void GridViewVCCElecRefresh(DataGridView GridU, string[] data)
        {
            //Array.Reverse(data);//反转数据
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (data[i * 8 + j] != null && data[i * 8 + j] != "")
                    {
                        double currentValue = Convert.ToDouble(Convert.ToInt32(data[i * 8 + j], 16));

                        // 如果电流值为0.0，标记为红色，否则为蓝色
                        if (currentValue == 0.0)
                        {
                            GridU.Rows[j * 2].Cells[i].Style.ForeColor = Color.Red;
                        }
                        else
                        {
                            GridU.Rows[j * 2].Cells[i].Style.ForeColor = Color.Blue;
                        }

                        GridU.Rows[j * 2].Cells[i].Value = currentValue.ToString("0.0");
                    }
                }
            }
        }

        public void GridViewVCQElecRefresh(DataGridView GridU, string[] data)
        {
            //Array.Reverse(data);//反转数据
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (data[i * 8 + j] != null && data[i * 8 + j] != "")
                    {
                        double currentValue = Convert.ToDouble(Convert.ToInt32(data[i * 8 + j], 16));

                        // 如果电流值为0.0，标记为红色，否则为灰色
                        if (currentValue == 0.0)
                        {
                            GridU.Rows[j * 2 + 1].Cells[i].Style.ForeColor = Color.Red;
                        }
                        else
                        {
                            GridU.Rows[j * 2 + 1].Cells[i].Style.ForeColor = Color.Gray;
                        }

                        GridU.Rows[j * 2 + 1].Cells[i].Value = currentValue.ToString("0.0");
                    }
                    else
                    {
                        // 如果没有数据，显示0并标记为红色
                        GridU.Rows[j * 2 + 1].Cells[i].Style.ForeColor = Color.Red;
                        GridU.Rows[j * 2 + 1].Cells[i].Value = "0.0";
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
