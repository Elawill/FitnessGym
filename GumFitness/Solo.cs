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
using Word = Microsoft.Office.Interop.Word;

namespace GumFitness
{
    public partial class Solo : Form
    {
        DataSet bd,bd1,bd2,bd3,bd4,bd5,bd6,bd7;
        DataSet s,up,k;
        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True";

        public Solo()
        {
            InitializeComponent();
        }
        public int id_s,id_cl,price,id_service, count, cdacha, sum = 0;

        //****************ЧЕК***********************
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
            Repwo("{ysluga}", "Персональная тренировка", Worddoc);
            Repwo("{count}", count.ToString(), Worddoc);
            Repwo("{price}", price.ToString(), Worddoc);
            Repwo("{sum}", sum.ToString(), Worddoc);
            Repwo("{itog}", price.ToString(), Worddoc);
            Repwo("{sdacha}", cdacha.ToString(), Worddoc);
            Worddoc.SaveAs2(Application.StartupPath + $"\\Чек {"персональные тренировки "+DateTime.Now.ToLongDateString()}" + ".docx");
            WordApp.Visible = true;
        }
        private void Repwo(string subToReplace, string text, Word.Document worddoc)
        {
            var range = worddoc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: subToReplace, ReplaceWith: text);
        }

        public int id_soloFitness, count_s;
        public string name_cl;

        private void button4_Click(object sender, EventArgs e)
        {
            Start admin = new Start();
            admin.Show();
            this.Hide();
        }
       
        //Запрос на вывод в Combobox
        private void fillBy2ToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.jobsTableAdapter.FillBy2(this.gymDataSet.Jobs);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void button5_Click(object sender, EventArgs e)//Выбрать тренера
        {
            button3.Visible = true;
            panel2.Visible = true;
            label2.Text = "" + price;

            string ok = "select id_service from Service inner join Jobs on Service.id_job=Jobs.id_job " +
                "where Jobs.FIO='"+comboBox1.Text+"' and Service.name='Персольные тренировки'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd5 = new DataSet();
                sql = new SqlDataAdapter(ok, podkl);
                sql.Fill(bd5);
            }
            id_service = Convert.ToInt32(bd5.Tables[0].Rows[0][0]);//ID Услуги по выбору тренера

        }
        //************************ПОКУПКА******************************
        private void button3_Click(object sender, EventArgs e)//Оформить покупку
        {
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
                    label5.Text = "" + cdacha;
                    insert();                    
                    MessageBox.Show("Покупка завершена!", "Покупка Абонемента", MessageBoxButtons.OK);
                    panel4.Visible = true;
                }
            }
        } 

        public void insert()
        {
            string vi = "select * from SoloFitness where id_client="+id_cl;
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                s = new DataSet();
                sql = new SqlDataAdapter(vi, podkl);
                sql.Fill(s);
            }
            if (s.Tables[0].Rows.Count == 0)//ЕСЛИ У КЛИЕНТА ЕЩЕ НЕ БЫЛО Покупки
            {
                string max = "select MAX(id_solofitness) from SoloFitness";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    bd6 = new DataSet();
                    sql = new SqlDataAdapter(max, podkl);
                    sql.Fill(bd6);
                }
                id_soloFitness = Convert.ToInt32(bd6.Tables[0].Rows[0][0]) + 1;

                string ins = "insert into SoloFitness(id_solofitness,id_service,id_client,id_solo,count) " +
                    "values(" + id_soloFitness.ToString() + ", " + id_service.ToString() + ", " + id_cl.ToString() + "," + id_s.ToString() + "," + count.ToString() + ")";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    bd7 = new DataSet();
                    sql = new SqlDataAdapter(ins, podkl);
                    sql.Fill(bd7);
                }
            }
            else
            {
                string upd = "update SoloFitness set count=" + count + " where id_client=" + id_cl + "and id_service=" + id_service;
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    up = new DataSet();
                    sql = new SqlDataAdapter(upd, podkl);
                    sql.Fill(up);
                }                
            }
        }
        //******************************************************


        //********************ВЫВОД КЛИЕНТОВ**************************
        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel3.Visible = true;

            string vivod = "select Client.FIO from Client inner join Status on Client.id_status=Status.id_status";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd2 = new DataSet();
                sql = new SqlDataAdapter(vivod, podkl);
                sql.Fill(bd2);
                dataGridView2.DataSource = bd2.Tables[0];
                dataGridView2.Columns[0].HeaderCell.Value = "ФИО";
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string search = "select Client.FIO from Client inner join Status " +
                "on Client.id_status=Status.id_status where FIO like '" + textBox1.Text + "%'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd3 = new DataSet();
                sql = new SqlDataAdapter(search, podkl);
                sql.Fill(bd3);
                dataGridView2.DataSource = bd3.Tables[0];
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            name_cl = dataGridView2.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();//ИМЯ Выбранного клиента
            string click = "select id_client from Client where FIO='" + name_cl + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd4 = new DataSet();
                sql = new SqlDataAdapter(click, podkl);
                sql.Fill(bd4);
            }
            id_cl = Convert.ToInt32(bd4.Tables[0].Rows[0][0]);//ID Выбранного клиента        
        }

        //********************************************************


        //********************ВЫВОД АБОНЕМЕНТОВ**************************
        private void Solo_Load(object sender, EventArgs e)
        {
            string vivod = "select Solo.id_solo, Ogranich.visit, Solo.price " +
                "from Solo inner join Ogranich on Solo.id_ogranch = Ogranich.id_ogranch ";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(vivod, podkl);
                sql.Fill(bd);
                dataGridView1.DataSource = bd.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "ID";
                dataGridView1.Columns[1].HeaderCell.Value = "Кол-во посещений";
                dataGridView1.Columns[2].HeaderCell.Value = "Цена";
            }

        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            id_s = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue.ToString());//ID Выбранной персональной тренировки

            string click = "select Ogranich.visit, Solo.price from Ogranich inner join Solo on " +
                "Ogranich.id_ogranch=Solo.id_ogranch  where Solo.id_solo=" + id_s;
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd1 = new DataSet();
                sql = new SqlDataAdapter(click, podkl);
                sql.Fill(bd1);
            }           
            price = Convert.ToInt32(bd1.Tables[0].Rows[0]["price"]);//ЦЕНА персональной тренировки
            count = Convert.ToInt32(bd1.Tables[0].Rows[0]["visit"]);//Кол-во посещений персональной тренировки
        }
        //********************************************************
    }
}
