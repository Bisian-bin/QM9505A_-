using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM9505
{
    public partial class LoginForm : Form
    {
        public static long timer;
        Access access = new Access();
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (Variable.userEnter == Variable.UserEnter.User)
            {
                txtuser.Text = "操作员";
            }
            else if (Variable.userEnter == Variable.UserEnter.Engineer)
            {
                txtuser.Text = "工程师";
            }
            else if (Variable.userEnter == Variable.UserEnter.Manufacturer)
            {
                txtuser.Text = "厂商";
            }
            else if (Variable.userEnter == Variable.UserEnter.Administrator)
            {
                txtuser.Text = "Admin";
            }
        }


        //确认
        private void btnSure_Click(object sender, EventArgs e)
        {
            string CONN = Access.GetSqlConnectionString();
            using (OleDbConnection conn = new OleDbConnection(CONN))
            {
                conn.Open();
                using (OleDbCommand Rcmd = conn.CreateCommand())
                {
                    Rcmd.CommandText = "SELECT username FROM UserPass WHERE username=@user";//判断用户是否存在
                    Rcmd.Parameters.AddWithValue("user", txtuser.Text.Trim());
                    OleDbDataReader Rreader = Rcmd.ExecuteReader();
                    bool b = Rreader.Read();
                    if (!b)
                    {
                        MessageBox.Show("该用户不存在!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txtuser.Focus();
                        return;
                    }
                }
                conn.Close();
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from UserPass where username=@user";
                        cmd.Parameters.AddWithValue("user", txtuser.Text.Trim());
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string pass = reader.GetString(reader.GetOrdinal("pass"));//密码核对
                                if (pass == txtPass.Text.Trim())
                                {
                                    string power1 = reader.GetString(reader.GetOrdinal("power1"));//操作员权限1#
                                    if (power1 == "1")
                                    {
                                        Variable.userEnter = Variable.UserEnter.User;
                                        //写入数据库
                                        access.RecordAccess(LogType.Operate, "用户登录:操作员");
                                    }
                                    string power2 = reader.GetString(reader.GetOrdinal("power2"));//工程师权限2#
                                    if (power2 == "1")
                                    {
                                        Variable.userEnter = Variable.UserEnter.Engineer;
                                        timer = 0;
                                        Variable.loginFlag = true;
                                        //写入数据库
                                        access.RecordAccess(LogType.Operate, "用户登录:工程师");
                                    }
                                    string power3 = reader.GetString(reader.GetOrdinal("power3"));//厂商权限3#    
                                    if (power3 == "1")
                                    {
                                        Variable.userEnter = Variable.UserEnter.Manufacturer;
                                        timer = 0;
                                        Variable.loginFlag = true;
                                        //写入数据库
                                        access.RecordAccess(LogType.Operate, "用户登录:厂商");
                                    }
                                    string power4 = reader.GetString(reader.GetOrdinal("power4"));//Admin权限3#    
                                    if (power4 == "1")
                                    {
                                        Variable.userEnter = Variable.UserEnter.Administrator;
                                        timer = 0;
                                        Variable.loginFlag = true;
                                        //写入数据库
                                        access.RecordAccess(LogType.Operate, "用户登录:Admin");
                                    }

                                    this.Close();//退出软件
                                }
                                else
                                {
                                    MessageBox.Show("密码错误,请重新输入!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtPass.Clear();
                                }
                            }
                            else
                            {
                                MessageBox.Show("无此用户!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                    }
                    conn.Close();
                }
            }
            catch
            {

            }
        }

        //修改
        private void btnChange_Click(object sender, EventArgs e)
        {
            if (Variable.userEnter == Variable.UserEnter.Administrator)
            {
                try
                {
                    string username = txtuser.SelectedItem.ToString();//用户名                    
                    string pass = txtPass.Text;//密码

                    if (username == "Admin")
                    {
                        return;
                    }
                    string CONN = Access.GetSqlConnectionString();
                    using (OleDbConnection conn = new OleDbConnection(CONN))
                    {
                        conn.Open();
                        using (OleDbCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "update UserPass set pass=@pass where username=@username";

                            cmd.Parameters.AddWithValue("pass", pass);
                            cmd.Parameters.AddWithValue("username", username);
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //大于10分钟自动退出
            if (Variable.loginFlag)
            {
                timer += 1;
            }
            if (timer > 600)
            {
                Variable.loginFlag = false;
                Variable.userEnter = Variable.UserEnter.User;
            }

            if (Variable.userEnter == Variable.UserEnter.Administrator)
            {
                btnChange.Enabled = true;
            }
            else
            {
                btnChange.Enabled = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string[] svValue = new string[5];
            if (txtuser.Text == string.Empty || txtPass.Text == "")
            {
                MessageBox.Show("输入内容不能为空！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            else
            {
                try
                {
                    svValue[0] = txtuser.Text.Trim();//用户名称
                    svValue[1] = txtPass.Text.Trim();//密码
                    svValue[2] = "1"; //权限1
                    svValue[3] = "1"; //权限2   
                    svValue[4] = "1"; //权限3   
                    svValue[5] = "1"; //权限4   
                }
                catch
                {
                    MessageBox.Show("请输入正确的数字!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                string CONN = Access.GetSqlConnectionString();
                using (OleDbConnection conn = new OleDbConnection(CONN))
                {
                    conn.Open();
                    using (OleDbCommand Rcmd = conn.CreateCommand())
                    {
                        Rcmd.CommandText = "SELECT * FROM UserPass WHERE username=@user";//判斷是否有相同名稱存在!
                        Rcmd.Parameters.AddWithValue("user", txtuser.Text.Trim());
                        OleDbDataReader Rreader = Rcmd.ExecuteReader();
                        bool b = Rreader.Read();
                        if (b)
                        {
                            MessageBox.Show("已有相同名称存在!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO UserPass(username,pass,power1,power2,power3)" +
                            "values(@s1,@s2,@s3,@s4,@s5)";
                        string sr = "s";
                        for (int i = 0; i < svValue.Length; i++)
                        {
                            sr = "s" + (i + 1).ToString();
                            cmd.Parameters.AddWithValue(sr, svValue[i]);
                        }
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("权限创建完成!", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    }
                }
            }
        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            if (txtuser.Text.Trim().Length < 1)
            {
                MessageBox.Show("请选择要删除的用户名!", "提示:", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;

            }
            if (DialogResult.OK != MessageBox.Show("确定要删除此用户名?", txtuser.Text.Trim(), MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
            {
                return;
            }
            string CONN = Access.GetSqlConnectionString();
            using (OleDbConnection conn = new OleDbConnection(CONN))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "delete from UserPass where username = @user";
                    cmd.Parameters.AddWithValue("user", txtuser.Text.Trim());
                    cmd.ExecuteNonQuery();
                    txtPass.Clear();
                    MessageBox.Show("已刪除!", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }
            }
        }


    }
}
