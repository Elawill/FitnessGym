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
    public partial class Director : Form
    {
        DataSet max1, max2,max3, bd, bd1, bd2, bd3,bd4;
        DataSet show, show1,ins, click;
        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True";

        bool search = false;
        public string name_cl, name_service="", date = "";
        public int id_job;
        public Director()
        {
            InitializeComponent();
        }
      
       
        private void Director_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "gymDataSet.Service". При необходимости она может быть перемещена или удалена.
            this.serviceTableAdapter.Fill(this.gymDataSet.Service);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "gymDataSet.Jobs". При необходимости она может быть перемещена или удалена.
            this.jobsTableAdapter.Fill(this.gymDataSet.Jobs);

            //ВЫВОД ТРЕНИРОВОК
            string vivod = "select Client.FIO,Training.date, Training.count from Training inner join Client on Client.id_client " +
                "= Training.id_client join Service on Service.id_service = Training.id_service where Client.activity = 'Активный' " +
                "and Training.count != 0 ";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                show = new DataSet();
                sql = new SqlDataAdapter(vivod, podkl);
                sql.Fill(show);
                dataGridView1.DataSource = show.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "ФИО";
                dataGridView1.Columns[1].HeaderCell.Value = "Дата тренировки";
                dataGridView1.Columns[2].HeaderCell.Value = "Кол-во посещений";                
            }
            //ВЫВОД ТРЕНЕРОВ
            string vi = "select Jobs.FIO from Jobs where job='Тренер'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                show1 = new DataSet();
                sql = new SqlDataAdapter(vi, podkl);
                sql.Fill(show1);
                dataGridView2.DataSource = show1.Tables[0];
                dataGridView2.Columns[0].HeaderCell.Value = "ФИО";
                
            }

        }
        //*********************ПОИСК****************************
        private void button3_Click(object sender, EventArgs e)//ДАТА
        {
            search = true;
            date = dateTimePicker1.Value.Date.ToString();
            string delete = "select Client.FIO,Training.date, Training.count from Training inner join" +
                " Client on Client.id_client = Training.id_client join Service on Service.id_service = Training.id_service " +
                "where Client.activity='Активный' and Training.date='" + date + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                show = new DataSet();
                sql = new SqlDataAdapter(delete, podkl);
                sql.Fill(show);
                dataGridView1.DataSource = show.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "ФИО";
                dataGridView1.Columns[0].HeaderCell.Value = "Дата тренировки";
                dataGridView1.Columns[0].HeaderCell.Value = "Кол-во посещений";
                dataGridView1.Columns[0].HeaderCell.Value = "Абонемент";
            }
        }

        private void button4_Click(object sender, EventArgs e)//ПП
        {
            string pp = "select Client.FIO, Jobs.FIO, SoloFitness.count, Training.date from Client " +
                "inner join Training on Client.id_client = Training.id_client join Service on " +
                "Service.id_service = Training.id_service join SoloFitness on SoloFitness.id_service = Service.id_service " +
                "join Jobs on Jobs.id_job = Service.id_job ";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                show = new DataSet();
                sql = new SqlDataAdapter(pp, podkl);
                sql.Fill(show);
                dataGridView1.DataSource = show.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "ФИО";
                dataGridView1.Columns[1].HeaderCell.Value = "Тренер";
                dataGridView1.Columns[2].HeaderCell.Value = "Кол-во посещений";
                dataGridView1.Columns[3].HeaderCell.Value = "Дата";
            }
            if (search == true && date != "")
            {
                string pp1 = "select Client.FIO, Jobs.FIO, SoloFitness.count, Training.date from Client " +
                "inner join Training on Client.id_client = Training.id_client join Service on " +
                "Service.id_service = Training.id_service join SoloFitness on SoloFitness.id_service = Service.id_service " +
                "join Jobs on Jobs.id_job = Service.id_job where Training.date='" + date + "'";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    show = new DataSet();
                    sql = new SqlDataAdapter(pp1, podkl);
                    sql.Fill(show);
                    dataGridView1.DataSource = show.Tables[0];
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)//ГП
        {
            string gp = "select Client.FIO, Service.name, Jobs.FIO, Training.date " +
                "from Client inner join Training on Client.id_client = Training.id_client " +
                "join Service on Service.id_service = Training.id_service join Jobs on Jobs.id_job = Service.id_job " +
                "where Service.name != 'Абонемент' and Service.name != 'Гостевое посещение' and Service.name != 'Персольные тренировки'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                show = new DataSet();
                sql = new SqlDataAdapter(gp, podkl);
                sql.Fill(show);
                dataGridView1.DataSource = show.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "ФИО";
                dataGridView1.Columns[1].HeaderCell.Value = "Групповая тренировка";
                dataGridView1.Columns[2].HeaderCell.Value = "Тренер";
                dataGridView1.Columns[3].HeaderCell.Value = "Дата";

            }
            if (search == true && date != "")
            {
                string gp1 = "select Client.FIO, Service.name, Jobs.FIO, Training.date " +
                "from Client inner join Training on Client.id_client = Training.id_client " +
                "join Service on Service.id_service = Training.id_service join Jobs on Jobs.id_job = Service.id_job " +
                "where Service.name != 'Абонемент' and Service.name != 'Гостевое посещение' and Service.name != 'Персольные тренировки' " +
                "and Training.date='" + date + "'";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    show = new DataSet();
                    sql = new SqlDataAdapter(gp1, podkl);
                    sql.Fill(show);
                    dataGridView1.DataSource = show.Tables[0];

                }
            }
        }
        //**********************************************************************8

        //*********************Тренировки тренера****************************
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            name_cl = dataGridView2.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();//ИМЯ Выбранного клиента
            string select = "select id_job from Jobs where FIO='" + name_cl + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                click = new DataSet();
                sql = new SqlDataAdapter(select, podkl);
                sql.Fill(click);
            }
            id_job = Convert.ToInt32(click.Tables[0].Rows[0][0]);//ID Выбранного клиента 
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string pr = "select Service.name from Service inner join Jobs on Jobs.id_job=Service.id_job " +
                    "where Service.name = '" + comboBox3.Text + "' and Jobs.id_job =" + id_job;
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd4 = new DataSet();
                sql = new SqlDataAdapter(pr, podkl);
                sql.Fill(bd4);
            }
            
            if (bd4.Tables[0].Rows.Count == 0)
            {
                insert_service();
                if (MessageBox.Show("Присвоить еще тренировки?", "Тренировки", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (bd4.Tables[0].Rows.Count == 0)
                    {
                        insert_service();                        
                    }
                    else
                    {
                        MessageBox.Show("Тренер уже ведет выбранную тренировку!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }                
            }
            else
            {
                MessageBox.Show("Тренер уже ведет выбранную тренировку!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void insert_service()
        {
            string ma = "select Max(id_service) from Service";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                max3 = new DataSet();
                sql = new SqlDataAdapter(ma, podkl);
                sql.Fill(max3);

            }
            int id_service = Convert.ToInt32(max3.Tables[0].Rows[0][0]) + 1;

            string insert = "insert into Service(id_service,name,id_job) " +
                "values(" + id_service.ToString() + ", '" + comboBox3.Text + "', " + id_job.ToString() + " )";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                ins = new DataSet();
                sql = new SqlDataAdapter(insert, podkl);
                sql.Fill(ins);                
            }
            MessageBox.Show("Тренеру присвоена тренировка", "", MessageBoxButtons.OK);

            string vi = "select Jobs.FIO, Service.name from Jobs inner join " +
                "Service on Service.id_job=Jobs.id_job where job='Тренер'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                show1 = new DataSet();
                sql = new SqlDataAdapter(vi, podkl);
                sql.Fill(show1);
                dataGridView2.DataSource = show1.Tables[0];
                dataGridView2.Columns[0].HeaderCell.Value = "ФИО";
                dataGridView2.Columns[1].HeaderCell.Value = "Тренировка";
            }
        }

        //*******************************8****************************************


        //*********************УДАЛЕНИЕ СОТРУДНИКА****************************
        private void button2_Click(object sender, EventArgs e)
        {
            //Поиск ID выбранного сотрудника
            string search = "select id_job from Jobs where FIO='"+comboBox2.Text+"'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd2 = new DataSet();
                sql = new SqlDataAdapter(search, podkl);
                sql.Fill(bd2);
            }
            int id_job = Convert.ToInt32(bd2.Tables[0].Rows[0][0]);

            if (MessageBox.Show("Вы уверены, что ходите удалить сотрудника?", "Удаление", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //Удаление сотрудника
                string delete = "delete from Jobs where id_job=" + id_job.ToString();
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    bd3 = new DataSet();
                    sql = new SqlDataAdapter(delete, podkl);
                    sql.Fill(bd3);
                    MessageBox.Show("Сотрудник уволен", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            
        }
        //***************************************************************************


        //*********************ДОБАВЛЕНИЕ СОТРУДНИКА****************************
        private void button1_Click(object sender, EventArgs e)
        {
            //Проверка заполнения полей
            if (String.IsNullOrWhiteSpace(textBox1.Text) || String.IsNullOrWhiteSpace(textBox2.Text) ||
                String.IsNullOrWhiteSpace(textBox4.Text) || String.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Заполните поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        public void insert()
        {
            //Поиск и добавдение нового ID сотрудника
            string max_j = "select MAX(id_job) from Jobs"; 
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                max1 = new DataSet();
                sql = new SqlDataAdapter(max_j, podkl);
                sql.Fill(max1);
            }
            int max_job = Convert.ToInt32(max1.Tables[0].Rows[0][0]) + 1;

            //Поиск и добавдение нового ID паспорта
            string max_p = "select MAX(id_passport) from Passport";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                max2 = new DataSet();
                sql = new SqlDataAdapter(max_p, podkl);
                sql.Fill(max2);
            }
            int max_pas = Convert.ToInt32(max2.Tables[0].Rows[0][0]) + 1;

            //Добавдение нового сотрудника
            string insert_cl = "insert into Jobs (id_job,FIO,job,id_passport) values("+max_job.ToString()+", " +
                "'"+textBox1.Text+" "+textBox2.Text+" "+textBox3.Text+"', '"+comboBox1.Text+"', "+max_pas.ToString()+")";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(insert_cl, podkl);
                sql.Fill(bd);
            }

            //Добавдение нового Паспорта
            string insert_pas = "insert into Passport(id_passport,serias,number) " +
                "values(" + max_pas.ToString() + ", " + textBox4.Text + ", " + textBox5.Text + ")";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd1 = new DataSet();
                sql = new SqlDataAdapter(insert_pas, podkl);
                sql.Fill(bd1);
            }
        }
        //***************************************************************************


        private void fillBy1ToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.serviceTableAdapter.FillBy1(this.gymDataSet.Service);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void fillBy4ToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.jobsTableAdapter.FillBy4(this.gymDataSet.Jobs);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void fillBy5ToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.jobsTableAdapter.FillBy5(this.gymDataSet.Jobs);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }
    }
}
