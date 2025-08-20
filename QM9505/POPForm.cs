using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public partial class POPForm : Form
    {
        public POPForm()
        {
            InitializeComponent();
        }

        #region 确定
        private void btnSure_Click(object sender, EventArgs e)
        {
            if (Variable.step != "" && Variable.cancelStep != 0 && Variable.sureStep != 0)
            {
                switch (Variable.step)
                {
                    case "Variable.UpAutoStep":
                        {
                            Variable.UpAutoStep = Convert.ToInt32(Variable.sureStep);
                            break;
                        }
                    case "Variable.DownAutoStep":
                        {
                            Variable.DownAutoStep = Convert.ToInt32(Variable.sureStep);
                            break;
                        }
                }
            }

            Variable.POPFlag = false;
            Variable.step = "";
            Variable.cancelStep = 0;
            Variable.sureStep = 0;
            timerScan.Stop();
            Variable.MachineState = Variable.MachineStatus.Stop;
            Variable.AlarmFlag = false;
            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
            }
            this.Close();
        }

        #endregion

        #region 取消/跳过
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (Variable.step != "" && Variable.cancelStep != 0 && Variable.sureStep != 0)
            {
                switch (Variable.step)
                {
                    case "Variable.UpAutoStep":
                        {
                            Variable.UpAutoStep = Convert.ToInt32(Variable.cancelStep);
                            break;
                        }
                    case "Variable.DownAutoStep":
                        {
                            Variable.DownAutoStep = Convert.ToInt32(Variable.cancelStep);
                            break;
                        }
                }
            }

            Variable.POPFlag = false;
            Variable.step = "";
            Variable.cancelStep = 0;
            Variable.sureStep = 0;
            timerScan.Stop();
            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
            }
            this.Close();
        }

        #endregion

        private void timerScan_Tick(object sender, EventArgs e)
        {
            if (Variable.PauseButton == true)
            {
                Variable.POPFlag = false;
                Variable.AlarmFlag = false;
                timerScan.Stop();
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                }
                this.Close();
            }
        }
    }
}
