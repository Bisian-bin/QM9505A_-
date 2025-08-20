using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public partial class YieldForm : Form
    {
        DataGrid dataGrid = new DataGrid();
        public Thread RefreshThread;
        public int formNum = 0;
        TXT myTXT = new TXT();
        public string[] UpCellPassNum1 = new string[152];
        public string[] UpCellFailNum1 = new string[152];
        public string[] UpCellPassNum2 = new string[152];
        public string[] UpCellFailNum2 = new string[152];

        public string[] DownCellPassNum1 = new string[152];
        public string[] DownCellFailNum1 = new string[152];
        public string[] DownCellPassNum2 = new string[152];
        public string[] DownCellFailNum2 = new string[152];

        public YieldForm()
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
        private void AlarmForm_Load(object sender, EventArgs e)
        {
            //dataGrid初始化
            dataGrid.IniLeftModelTrayW(GridViewU1, 16, 19);
            dataGrid.IniLeftModelTrayW(GridViewU2, 16, 19);
            dataGrid.IniLeftModelTrayW(GridViewD1, 16, 19);
            dataGrid.IniLeftModelTrayW(GridViewD2, 16, 19);

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
                    //上层总数更新
                    GridViewTotalRefresh(GridViewU1, Variable.UpCellTotalNum1);
                    //上层良率更新
                    GridViewYieldRefresh(GridViewU1, Variable.UpCellYield1);

                    //上层总数更新
                    GridViewTotalRefresh(GridViewU2, Variable.UpCellTotalNum2);
                    //上层良率更新
                    GridViewYieldRefresh(GridViewU2, Variable.UpCellYield2);

                    //下层总数更新
                    GridViewTotalRefresh(GridViewD1, Variable.DownCellTotalNum1);
                    //下层良率更新
                    GridViewYieldRefresh(GridViewD1, Variable.DownCellYield1);

                    //下层总数更新
                    GridViewTotalRefresh(GridViewD2, Variable.DownCellTotalNum2);
                    //下层良率更新
                    GridViewYieldRefresh(GridViewD2, Variable.DownCellYield2);

                }
                catch
                {
                }

                Thread.Sleep(1);
            }
        }
        #endregion

        #region GridView界面总数刷新

        public void GridViewTotalRefresh(DataGridView GridU, double[] data)
        {
            //Array.Reverse(data);//反转数据

            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    GridU.Rows[j * 2].Cells[i].Style.ForeColor = Color.Blue;
                    GridU.Rows[j * 2].Cells[i].Value = data[i * 8 + j];
                }
            }
        }

        #endregion

        #region GridView界面良率刷新

        public void GridViewYieldRefresh(DataGridView GridU, double[] data)
        {
            //Array.Reverse(data);//反转数据

            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    GridU.Rows[j * 2 + 1].Cells[i].Style.ForeColor = Color.Gray;
                    GridU.Rows[j * 2 + 1].Cells[i].Value = data[i * 8 + j] + "%";
                }
            }
        }

        #endregion

        #region 总数良率清空
        private void U_1btn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 152; i++)
            {
                Variable.UpCellTotalNum1[i] = 0;
                Variable.UpCellPassNum1[i] = 0;
                UpCellPassNum1[i] = "0";
                Variable.UpCellFailNum1[i] = 0;
                UpCellFailNum1[i] = "0";
            }

            //将OK数和NG数纪录下来
            string path1 = Application.StartupPath + "\\Map\\UpCellPassNum1";
            myTXT.WriteTxtVariable(UpCellPassNum1, path1);
            string path2 = Application.StartupPath + "\\Map\\UpCellFailNum1";
            myTXT.WriteTxtVariable(UpCellFailNum1, path2);
        }

        private void U_2btn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 152; i++)
            {
                Variable.UpCellTotalNum2[i] = 0;
                Variable.UpCellPassNum2[i] = 0;
                UpCellPassNum2[i] = "0";
                Variable.UpCellFailNum1[i] = 0;
                UpCellFailNum2[i] = "0";
            }

            //将OK数和NG数纪录下来
            string path1 = Application.StartupPath + "\\Map\\UpCellPassNum2";
            myTXT.WriteTxtVariable(UpCellPassNum2, path1);
            string path2 = Application.StartupPath + "\\Map\\UpCellFailNum2";
            myTXT.WriteTxtVariable(UpCellFailNum2, path2);
        }

        private void D_1btn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 152; i++)
            {
                Variable.DownCellTotalNum1[i] = 0;
                Variable.DownCellPassNum1[i] = 0;
                DownCellPassNum1[i] = "0";
                Variable.DownCellFailNum1[i] = 0;
                DownCellFailNum1[i] = "0";
            }

            //将OK数和NG数纪录下来
            string path3 = Application.StartupPath + "\\Map\\DownCellPassNum1";
            myTXT.WriteTxtVariable(DownCellPassNum1, path3);
            string path4 = Application.StartupPath + "\\Map\\DownCellFailNum1";
            myTXT.WriteTxtVariable(DownCellFailNum1, path4);

        }

        private void D_2btn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 152; i++)
            {
                Variable.DownCellTotalNum2[i] = 0;
                Variable.DownCellPassNum2[i] = 0;
                DownCellPassNum2[i] = "0";
                Variable.DownCellFailNum2[i] = 0;
                DownCellFailNum2[i] = "0";
            }

            //将OK数和NG数纪录下来
            string path3 = Application.StartupPath + "\\Map\\DownCellPassNum2";
            myTXT.WriteTxtVariable(DownCellPassNum2, path3);
            string path4 = Application.StartupPath + "\\Map\\DownCellFailNum2";
            myTXT.WriteTxtVariable(DownCellFailNum2, path4);
        }


        #endregion


    }
}
