using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using System.Data.SqlClient;

namespace GumFitness
{
    public partial class Abonement : Form
    {
        DataSet bd, bd1, bd2, bd3,bd4,bd5,bd6;
        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True";

        public int id_ab,id_cl, price,status_cl,status_ab, cdacha, sum=0;
        public string name_cl, name_ab;

        public Abonement()
        {
            InitializeComponent();

        }
        private void button5_Click(object sender, EventArgs e)
        {
            Start admin = new Start();
            admin.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Solo solo = new Solo();
            solo.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Random r1 = new Random();
            var WordApp = new Word.Application();
            WordApp.Visible = false;
            //путь к шаблону
            var Worddoc = WordApp.Documents.Open(Application.StartupPath + @"\Чек.docx");
            //заполнение
            Repwo("{check}", (1000 + r1.Next(10000)).ToString(), Worddoc);
            Repwo("{date}", DateTime.Now.ToLongDateString(), Worddoc);
            Repwo("{ysluga}", name_ab, Worddoc);
            Repwo("{count}", "1", Worddoc);
            Repwo("{price}", price.ToString(), Worddoc);
            Repwo("{sum}", sum.ToString(), Worddoc);
            Repwo("{itog}", price.ToString(), Worddoc);
            Repwo("{sdacha}", cdacha.ToString(), Worddoc);
            Worddoc.SaveAs2(Application.StartupPath + $"\\Чек {"абонемент " + DateTime.Now.ToLongDateString()}" + ".docx");
            WordApp.Visible = true;
        }
        private void Repwo(string subToReplace, string text, Word.Document worddoc)
        {
            var range = worddoc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: subToReplace, ReplaceWith: text);
        }
    
        //********************ВЫВОД АБОНЕМЕНТОВ**************************
        private void Abonement_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "gymDataSet.Status". При необходимости она может быть перемещена или удалена.
            this.statusTableAdapter.Fill(this.gymDataSet.Status);

            string vivod = "select Abonement.id_abonem, Abonement.name, Ogranich.limited, Abonement.price,Ogranich.visit " +
                "from Abonement inner join Ogranich on Abonement.id_ogranch = Ogranich.id_ogranch join Status " +
                "on Abonement.id_status=Status.id_status where Status.status!='Пустой'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(vivod, podkl);
                sql.Fill(bd);
                dataGridView1.DataSource = bd.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "ID";
                dataGridView1.Columns[1].HeaderCell.Value = "Название";
                dataGridView1.Columns[2].HeaderCell.Value = "Месяц";
                dataGridView1.Columns[3].HeaderCell.Value = "Цена";
                dataGridView1.Columns[4].HeaderCell.Value = "Кол-во посещений";
            }
            name_ab = bd.Tables[0].Rows[0]["name"].ToString();

        }
        private void button1_Click(object sender, EventArgs e)//Поиск по категории
        {
            string st = "select status from  Status where status='" + comboBox1.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd1 = new DataSet();
                sql = new SqlDataAdapter(st, podkl);
                sql.Fill(bd1);
            }
            string status = Convert.ToString(bd1.Tables[0].Rows[0][0]);
            //Запрет на вывод Гостевого Абонемента
            if (status == "Пустой")
            {
                MessageBox.Show("Данный абонемент не является подлинным \n Только для гостевого посещения", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string vivod = "select Abonement.id_abonem, Abonement.name, Ogranich.limited, Abonement.price,Ogranich.visit " +
                "from Abonement inner join Ogranich on Abonement.id_ogranch = Ogranich.id_ogranch " +
                "join Status on Abonement.id_status=Status.id_status where Status.status='" + comboBox1.Text + "' and Status.status!='Пустой'";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    bd = new DataSet();
                    sql = new SqlDataAdapter(vivod, podkl);
                    sql.Fill(bd);
                    dataGridView1.DataSource = bd.Tables[0];
                }
            }

        }
        //********************************************************

        //********************ВЫВОД КЛИЕНТОВ**************************
        private void button2_Click(object sender, EventArgs e)//Вывод Клиентов
        {
            panel1.Visible = true;
            string vivod = "select Client.FIO, Status.status from Client inner join Status on Client.id_status=Status.id_status";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd2 = new DataSet();
                sql = new SqlDataAdapter(vivod, podkl);
                sql.Fill(bd2);
                dataGridView2.DataSource = bd2.Tables[0];
                dataGridView2.Columns[0].HeaderCell.Value = "ФИО";
                dataGridView2.Columns[1].HeaderCell.Value = "Статус клиента";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            proverka();            
        }

        private void button3_Click(object sender, EventArgs e)//Поиск Клиентов
        {
            string search = "select Client.FIO, Status.status from Client inner join Status " +
                "on Client.id_status=Status.id_status where FIO like '" + textBox1.Text+"%'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd3 = new DataSet();
                sql = new SqlDataAdapter(search, podkl);
                sql.Fill(bd3);
                dataGridView2.DataSource = bd3.Tables[0];
            }
            
        }
        //********************************************************

        //************************ПОКУПКА******************************
        private void button4_Click(object sender, EventArgs e)//ПОКУПКА
        {
            panel2.Visible = true;
            label5.Text = "" + price;
            
            if (String.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Укажите внесенную сумму", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                sum = Convert.ToInt32(textBox2.Text);
                if (sum < price)
                {
                    MessageBox.Show("Недостаточно средств", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    cdacha = sum - price;
                    label7.Text = "" + cdacha;
                    //Внесение абонемента в Таблицу Клиенты
                    string update = "update Client set id_abonem=" + id_ab + ", activity='Активный' where id_client=" + id_cl;
                    using (SqlConnection podkl = new SqlConnection(podkServer))
                    {
                        podkl.Open();
                        bd6 = new DataSet();
                        sql = new SqlDataAdapter(update, podkl);
                        sql.Fill(bd6);
                    }
                    MessageBox.Show("Покупка завершена!", "Покупка Абонемента", MessageBoxButtons.OK);
                    panel3.Visible = true;
                }
            }
        }
        //********************************************************

        public void proverka()//Проверка категории клиента и абонемента
        {
            
            if (status_cl==1 && status_ab == 2)//Взрослый не может приобрести Студенчиский абонемент
            {
                MessageBox.Show("Клиент не можете приобрести данный абонемент, потому что не входит в эту категорию!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (status_cl == 1 && status_ab == 3)//Взрослый не может приобрести Пенсионный абонемент
            {
                MessageBox.Show("Клиент не можете приобрести данный абонемент, потому что не входит в эту категорию!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (status_cl == 2 && status_ab == 3)//Студент не может приобрести Пенсионный абонемент
            {
                MessageBox.Show("Клиент не можете приобрести данный абонемент, потому что не входит в эту категорию!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (status_cl == 3 && status_ab == 2)//Пенсионер не может приобрести Студенчиский абонемент
            {
                MessageBox.Show("Клиент не можете приобрести данный абонемент, потому что не входит в эту категорию!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                panel2.Visible = true;
                label5.Text = "" + price;
            }
        }

        //********************************************************
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)//Выбор Абонемента
        {
            id_ab = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue.ToString());

            string click = "select price, id_status from Abonement where id_abonem=" + id_ab;
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd4 = new DataSet();
                sql = new SqlDataAdapter(click, podkl);
                sql.Fill(bd4);
            }
            price = Convert.ToInt32(bd4.Tables[0].Rows[0][0]);
            status_ab = Convert.ToInt32(bd4.Tables[0].Rows[0]["id_status"]);
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)//Выбор Клиента
        {
            name_cl = dataGridView2.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
            string click = "select id_client,id_status from Client where FIO='" + name_cl + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd5 = new DataSet();
                sql = new SqlDataAdapter(click, podkl);
                sql.Fill(bd5);
            }
            id_cl = Convert.ToInt32(bd5.Tables[0].Rows[0][0]);
            status_cl= Convert.ToInt32(bd5.Tables[0].Rows[0]["id_status"]);
        }
    }
}
