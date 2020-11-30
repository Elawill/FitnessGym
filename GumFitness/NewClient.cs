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
    public partial class NewClient : Form
    {
        DataSet max1,max2,max3,bd,bd1,bd2,bd3;
        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True";
        public NewClient()
        {
            InitializeComponent();
        }

        public string start, finish, status="1";
        public int id_client;     

        private void button4_Click(object sender, EventArgs e)//Гостевое посещение
        {
            guest();
            MessageBox.Show("Успешно!", "Гостевое посещение", MessageBoxButtons.OK);
        }
        private void button6_Click(object sender, EventArgs e)//ДОБАВИТЬ ЛЬГОТУ
        {
            panel1.Visible = true;            
        }

        private void button5_Click(object sender, EventArgs e)//CОХРАНИТЬ
        {
            //Проверка заполнения полей
            if (String.IsNullOrWhiteSpace(textBox1.Text)|| String.IsNullOrWhiteSpace(textBox2.Text)|| 
                String.IsNullOrWhiteSpace(textBox4.Text)|| String.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Заполните поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //Нахождение возраста
                string date = "select datediff(YEAR, '"+ dateTimePicker1.Value.Date.ToString()+ "', GETDATE()) from Client";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    bd1 = new DataSet();
                    sql = new SqlDataAdapter(date, podkl);
                    sql.Fill(bd1);
                }
                int old = Convert.ToInt32(bd1.Tables[0].Rows[0][0]);//Кол-во полных лет

                if (old < 18)//Проверка на возраст клиента 
                    MessageBox.Show("Недопустимый возраст клиента \n Запрещено посещение людям младше 18 лет!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    try
                    {
                        insert();
                        MessageBox.Show("Добавление прошло успешно!", "Новый Клиент", MessageBoxButtons.OK);
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка!", "Новый Клиент", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            button1.Visible = true;
            button4.Visible = true;
        }

        public void insert() 
        {
            //Поиск и добавдение нового ID клиента
            string max_cl = "select MAX(id_client) from Client";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                max1 = new DataSet();
                sql = new SqlDataAdapter(max_cl, podkl);
                sql.Fill(max1);
            }
            id_client = Convert.ToInt32(max1.Tables[0].Rows[0][0]) + 1;

            //Поиск и добавдение нового ID паспорта
            string max_pas = "select MAX(id_passport) from Passport";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                max2 = new DataSet();
                sql = new SqlDataAdapter(max_pas, podkl);
                sql.Fill(max2);
            }
            int id_pas = Convert.ToInt32(max2.Tables[0].Rows[0][0]) + 1;

            //Добавдение нового Клиента
            string insert_cl = "insert into Client (id_client,id_passport,FIO,DR,id_status,job_stady,activity,id_abonem,start,finish) " +
                "values("+id_client.ToString()+", "+id_pas.ToString()+", '"+textBox1.Text +" "+textBox2.Text+" "+textBox3.Text +"', '"
                + dateTimePicker1.Value.Date.ToString()+"', "+ status+", '"+textBox6.Text+"', 'Не активный', 21, '"+start+"', '"+finish+"')";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(insert_cl, podkl);
                sql.Fill(bd);
            }

            //Добавдение нового Паспорта
            string insert_pas = "insert into Passport(id_passport,serias,number) " +
                "values("+id_pas.ToString() + ", "+textBox4.Text+", "+textBox5.Text+")";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd2 = new DataSet();
                sql = new SqlDataAdapter(insert_pas, podkl);
                sql.Fill(bd2);
            }
        }

        private void button2_Click(object sender, EventArgs e)//Форма Администратора
        {
            Start admin = new Start();
            admin.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Abonement abonement = new Abonement();
            abonement.Show();
            this.Hide();
        }

        public void guest()
        {
            //Поиск и добавдение новой ID Тренировки
            string max_tr = "select MAX(id_training) from Training";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                max3 = new DataSet();
                sql = new SqlDataAdapter(max_tr, podkl);
                sql.Fill(max3);
            }
            int id_tr = Convert.ToInt32(max3.Tables[0].Rows[0][0]) + 1;

            //Добавление Гостевого посещения у нового клиента
            string insert_tr = "insert into Training(id_training, id_service, id_client, date, count) " +
                "values("+id_tr.ToString()+", 2, "+id_client.ToString()+", '"+ DateTime.Today.ToString()+ "', 1)";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd3 = new DataSet();
                sql = new SqlDataAdapter(insert_tr, podkl);
                sql.Fill(bd3);
            }           
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            status = "3";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            panel2.Visible = true;
            start = Convert.ToString(dateTimePicker2.Value.Date);
            finish = Convert.ToString(dateTimePicker3.Value.Date);
            status = "2";
        }

        //ВВОД ТОЛЬКО ЦИФР В ПАСПОРТ
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }

        //ВВОД ТОЛЬКО БУКВ 
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) return;
            else
                e.Handled = true;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) return;
            else
                e.Handled = true;
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) return;
            else
                e.Handled = true;
        }
    }
}
