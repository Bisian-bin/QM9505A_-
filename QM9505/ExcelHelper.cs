using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    class ExcelHelper
    {

        #region 数据导出到Excel
        public void SaveAs(DataGridView dgvAgeWeekSex)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl files (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "Export Excel File To";
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            Stream myStream;
            myStream = saveFileDialog.OpenFile();

            //StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            // StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
            StreamWriter sw = new StreamWriter(myStream, System.Text.ASCIIEncoding.Unicode);//这样不会出现乱码

            string str = "";
            try
            {
                //写标题
                for (int i = 0; i < dgvAgeWeekSex.ColumnCount; i++)
                {
                    if (i > 0)
                    {
                        str += "\t";
                    }
                    str += dgvAgeWeekSex.Columns[i].HeaderText;
                }
                sw.WriteLine(str);
                //写内容
                for (int j = 0; j < dgvAgeWeekSex.Rows.Count; j++)
                {
                    string tempStr = "";
                    for (int k = 0; k < dgvAgeWeekSex.Columns.Count; k++)
                    {
                        if (k > 0)
                        {
                            tempStr += "\t";
                        }
                        if (dgvAgeWeekSex.Rows[j].Cells[k].Value != null)
                        {
                            tempStr += dgvAgeWeekSex.Rows[j].Cells[k].Value.ToString();
                        }
                    }
                    sw.WriteLine(tempStr);
                }
                MessageBox.Show("导出成功!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Information);
                sw.Close();
                myStream.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
        }

        #endregion



    }
}
