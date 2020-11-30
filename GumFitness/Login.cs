using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace GumFitness
{
    public partial class Login : Form
    {
        DataSet bd;
        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True";

        int i=0;
        int t = 0;
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string exit = "select Exits.login, Exits.password, Jobs.job from Exits inner join Jobs ON Exits.id_job = Jobs.id_job where login ='" + textBox1.Text + "' and password='" + textBox2.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(exit, podkl);
                sql.Fill(bd);
            }

            if (bd.Tables[0].Rows.Count == 0)
            {
                if (i < 1)
                {
                    MessageBox.Show("Не верный логин или пароль", "Ошибка", MessageBoxButtons.OK);
                    textBox1.Clear();
                    textBox2.Clear();
                    i++;
                }

                else
                {
                    t = 0;
                    timer1.Enabled = true;
                    Timer timer = new Timer();
                    timer.Show();
                    i = 0;
                }
                
            }
            else
            {
                if (bd.Tables[0].Rows[0]["job"].ToString() == "Администратор")
                {
                    Start admin = new Start();
                    admin.Show();
                    this.Hide();
                }
                if (bd.Tables[0].Rows[0]["job"].ToString() == "Директор")
                {
                    Director director = new Director();
                    director.Show();
                    this.Hide();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (t < 59)
            {
                t++;
                panel1.Enabled = false;

            }
            else
            {
                timer1.Enabled = true;
                panel1.Enabled = true;
            }
        }

    }
}
