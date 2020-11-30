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
    public partial class Admin : Form
    {
        DataSet bd, bd1, bd2, bd3, bd4,k;
        DataSet up,og, og_data,up1,up2;
        DataSet gr,gr1,gr2,m_g;
        DataSet s,s1,s2,m_s;
        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True";

        public string name_cl, fio;
        public int id_cl,id_tr,max_tr,count,kol_visit,id_ab, limit;  
        public int id_s_gr, max_gr,count_gr, id_solo_serv, max_s,count_s,id_solo;
        public int visit_solo;
        DateTime date = DateTime.Today;

        public Admin()
        {
            InitializeComponent();
        }
                     
        //********************ПОСЕЩЕНИЕ*******************************
        private void button5_Click(object sender, EventArgs e)
        {
            visit();
            
        }

        public void vivod()
        {
            string search = "select Client.FIO, Service.name, Training.count, Training.date from Client" +
                " join Training on Client.id_client = Training.id_client join Service on Service.id_service" +
                " = Training.id_service where Training.count != 0";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                k = new DataSet();
                sql = new SqlDataAdapter(search, podkl);
                sql.Fill(k);
                dataGridView2.DataSource = k.Tables[0];
                dataGridView2.Columns[0].HeaderCell.Value = "Клиент";
                dataGridView2.Columns[1].HeaderCell.Value = "Посещение";
                dataGridView2.Columns[2].HeaderCell.Value = "Кол-во посещений";
                dataGridView2.Columns[3].HeaderCell.Value = "Дата";
            }
        }

        public void visit()
        {
            //ЗАПРОС НА ПОСЕЩЕНИЕ КЛИЕНТА ПО АБОНЕМЕНТУ
            string visit = "select Training.id_training, Training.count from Training inner join Client on Client.id_client " +
                "= Training.id_client join Service on Service.id_service = Training.id_service " +
                "where Client.id_client = "+id_cl.ToString();
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd2 = new DataSet();
                sql = new SqlDataAdapter(visit, podkl);
                sql.Fill(bd2);                
            }                        

            //Поиск ID абонемента, и граничений у клиента
            string org = "select Client.id_abonem, Ogranich.limited,Ogranich.visit from Client inner join Abonement " +
                    "on Abonement.id_abonem = Client.id_abonem join Ogranich on Ogranich.id_ogranch = Abonement.id_ogranch " +
                     "where Client.id_client = " + id_cl.ToString();
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                og = new DataSet();
                sql = new SqlDataAdapter(org, podkl);
                sql.Fill(og);
            }
            id_ab = Convert.ToInt32(og.Tables[0].Rows[0]["id_abonem"]);

            if (id_ab == 21)//Если у клиента гостевое посещение
            {
                if (bd2.Tables[0].Rows.Count == 0)//Если у клиента еще не было гостевых посещений
                {
                    string max_trein = "select MAX(id_training) from Training";//ПОИСК МАКС ID
                    using (SqlConnection podkl = new SqlConnection(podkServer))
                    {
                        podkl.Open();
                        bd4 = new DataSet();
                        sql = new SqlDataAdapter(max_trein, podkl);
                        sql.Fill(bd4);
                    }
                    max_tr = Convert.ToInt32(bd4.Tables[0].Rows[0][0]) + 1;

                    string ins = "insert into Training (id_training,id_service,id_client,date,count) " +
                     "values(" + max_tr.ToString() + ",2," + id_cl.ToString() + ",GETDATE(), 1)";
                    using (SqlConnection podkl = new SqlConnection(podkServer))
                    {
                        podkl.Open();
                        bd3 = new DataSet();
                        sql = new SqlDataAdapter(ins, podkl);
                        sql.Fill(bd3);
                    }
                    MessageBox.Show("У клиента зафиксированно гостевое занятие занятие", "Посещение", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("Закончилось кол-во посещений клиента", "Посещение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    string upd = "update Training set id_service=1 where id_client=" + id_cl;
                    using (SqlConnection podkl = new SqlConnection(podkServer))
                    {
                        podkl.Open();
                        up1 = new DataSet();
                        sql = new SqlDataAdapter(upd, podkl);
                        sql.Fill(up1);
                    }
                    update();
                }                   
            }
            else
            {
                if (bd2.Tables[0].Rows.Count == 0)//ЕСЛИ У КЛИЕНТА ЕЩЕ НЕ БЫЛО ЗАНЯТИЙ ПО АБОНЕМЕНТУ
                {
                    string max_trein = "select MAX(id_training) from Training";//ПОИСК МАКС ID
                    using (SqlConnection podkl = new SqlConnection(podkServer))
                    {
                        podkl.Open();
                        bd4 = new DataSet();
                        sql = new SqlDataAdapter(max_trein, podkl);
                        sql.Fill(bd4);
                    }
                    max_tr = Convert.ToInt32(bd4.Tables[0].Rows[0][0]) + 1;

                    //ДОБАВЛЕНИЕ ПЕРВОГО ПОСЕЩЕНИЯ КЛИЕНТА
                    string ins = "insert into Training (id_training,id_service,id_client,date,count) " +
                    "values(" + max_tr.ToString() + ",1," + id_cl.ToString() + ",GETDATE(), 1)";
                    using (SqlConnection podkl = new SqlConnection(podkServer))
                    {
                        podkl.Open();
                        bd3 = new DataSet();
                        sql = new SqlDataAdapter(ins, podkl);
                        sql.Fill(bd3);
                    }

                    MessageBox.Show("У клиента зафиксированно первое занятие", "Посещение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    vivod();
                }
                else
                {
                    
                    count = Convert.ToInt32(bd2.Tables[0].Rows[0]["count"]);//КОЛ-ВО ПОСЕЩЕНИЙ КЛИЕНТА ПО АБОНЕМЕНТУ
                    ogranichenie();                 
                }
            }               
        }

        //***************Ограничения ПОСЕЩЕНИЙ КЛИЕНТА********************
        public void ogranichenie()
        {            
            limit = Convert.ToInt32(og.Tables[0].Rows[0]["limited"]);//Допустимое кол-во месяцев

            limited();
            if (og.Tables[0].Rows[0]["visit"].ToString() == "")
            {
                insert_visit();                
            }
            else
            {
                kol_visit = Convert.ToInt32(og.Tables[0].Rows[0]["visit"]);//Допустимое кол-во посещений
                if (kol_visit <= count)
                {

                    MessageBox.Show("Закончилось кол-во посещений клиента \nПродлите абонемент", "Посещение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    update();
                }
                else
                {
                    insert_visit();                    
                }
            }

        }

        public void limited()//Ограничение посещений по кол-ву месяцев
        {
            string search = "select DATEADD(MONTH,"+limit.ToString()+", date) from Training where id_client="+id_cl.ToString();
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                og_data = new DataSet();
                sql = new SqlDataAdapter(search, podkl);
                sql.Fill(og_data);                
            }

            DateTime date_limit = Convert.ToDateTime(og_data.Tables[0].Rows[0][0]);//Конец даты работы абонемента           

            if (date > date_limit)
            {
                MessageBox.Show("Закончилось кол-во посещений клиента \nПродлите абонемент", "Посещение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                update();
            }

        }
        public void update()
        {
            string upd = "update Training set count=0 where id_client=" + id_cl;
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                up1 = new DataSet();
                sql = new SqlDataAdapter(upd, podkl);
                sql.Fill(up1);
            }
            string upd_act = "update Client set activity='Не активный' where id_client=3" + id_cl;
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                up2 = new DataSet();
                sql = new SqlDataAdapter(upd_act, podkl);
                sql.Fill(up2);
            }
        }
        //***************************************************

        //***************ДОБАВЛЕНИЕ ПОСЕЩЕНИЯ КЛИЕНТА********************
        public void insert_visit()
        {
            count += 1;
            string upd = "update Training set count=" + count.ToString() + " where id_client=" + id_cl;
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                up = new DataSet();
                sql = new SqlDataAdapter(upd, podkl);
                sql.Fill(up);
            }
            MessageBox.Show("Посещение добавленно", "Посещение", MessageBoxButtons.OK);
            vivod();
        }
        //*******************************************************************
        //*************************************************************************

        //***************ПЕРСОНАЛЬНЫЕ ТРЕНИРОВКИ КЛИЕНТА********************
        private void button3_Click(object sender, EventArgs e)
        {
            solo();
        }

        public void solo()
        {
            try
            {
                string group = "select * from SoloFitness where id_client=" + id_cl;
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    s = new DataSet();
                    sql = new SqlDataAdapter(group, podkl);
                    sql.Fill(s);
                }
                id_solo_serv = Convert.ToInt32(s.Tables[0].Rows[0]["id_service"]);//ID персональной тренировки клиента
                id_solo = Convert.ToInt32(s.Tables[0].Rows[0]["id_solo"]);//ID персональной тренировки
                visit_solo = Convert.ToInt32(s.Tables[0].Rows[0]["count"]);// Кол-во персональной тренировки

                //Запрос на информацию о посещениях клиента по выбранной тренировки
                string visit = "select Training.id_training, Training.id_service,Training.count from Service join Training" +
                    " on Service.id_service=Training.id_service join Client on Training.id_client = Client.id_client " +
                    "join SoloFitness on SoloFitness.id_client = Training.id_client where Service.id_service = " + id_solo_serv + " and Client.id_client = " + id_cl;
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    s1 = new DataSet();
                    sql = new SqlDataAdapter(visit, podkl);
                    sql.Fill(s1);
                }
                if (s1.Tables[0].Rows.Count == 0)//ЕСЛИ У КЛИЕНТА ЕЩЕ НЕ БЫЛО ЗАНЯТИЙ 
                {
                    string max_trein = "select MAX(id_training) from Training";//ПОИСК МАКС ID
                    using (SqlConnection podkl = new SqlConnection(podkServer))
                    {
                        podkl.Open();
                        m_s = new DataSet();
                        sql = new SqlDataAdapter(max_trein, podkl);
                        sql.Fill(m_s);
                    }
                    max_s = Convert.ToInt32(m_s.Tables[0].Rows[0][0]) + 1;
                    //ДОБАВЛЕНИЕ ПЕРВОГО ПОСЕЩЕНИЯ 
                    string ins = "insert into Training (id_training,id_service,id_client,date,count) " +
                    "values(" + max_s.ToString() + "," + id_solo_serv.ToString() + "," + id_cl.ToString() + ",GETDATE(), 1)";
                    using (SqlConnection podkl = new SqlConnection(podkServer))
                    {
                        podkl.Open();
                        s2 = new DataSet();
                        sql = new SqlDataAdapter(ins, podkl);
                        sql.Fill(s2);
                    }
                    MessageBox.Show("Добавлена первая первольная тренировка", "Посещение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    vivod();
                }
                else
                {
                    count_s = Convert.ToInt32(s1.Tables[0].Rows[0]["count"]);//КОЛ-ВО ПОСЕЩЕНИЙ КЛИЕНТА
                    ogranich_solo();

                }
            }
            catch
            {
                MessageBox.Show("У клиента нет персональных тренировок", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void ogranich_solo()
        {
            limited_solo();
            if (count_s >= visit_solo)
            {
                MessageBox.Show("Закончилось кол-во посещений персональных тренировок", "Посещение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                update();
            }
            else
            {
                count_s += 1;
                string upd = "update Training set count=" + count_s + " where id_client=" + id_cl + "and id_service=" + id_solo_serv;
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    up = new DataSet();
                    sql = new SqlDataAdapter(upd, podkl);
                    sql.Fill(up);
                }
                MessageBox.Show("Посещение персональной тренировки добавлено", "Посещение", MessageBoxButtons.OK);
                vivod();
            }

        }
        public void limited_solo()
        {
            string search = "select DATEADD(MONTH, 1 , date) from Training where id_client=" + id_cl.ToString();
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                og_data = new DataSet();
                sql = new SqlDataAdapter(search, podkl);
                sql.Fill(og_data);
            }

            DateTime date_limit = Convert.ToDateTime(og_data.Tables[0].Rows[0][0]);//Конец даты работы абонемента           
            if (date > date_limit)
            {
                MessageBox.Show("Закончилось кол-во персональных тренировок ", "Посещение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                update();
            }
        }
        //******************************************************************

        //***************ГРУППОВЫЕ ТРЕНИРОВКИ КЛИЕНТА********************
        private void button6_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;

        }
        private void button2_Click(object sender, EventArgs e)
        {
            group();
        }

        public void group()
        {
            //Запрос на вывод выбранной тренировки
            string group = "select * from Service where name='" + comboBox1.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                gr = new DataSet();
                sql = new SqlDataAdapter(group, podkl);
                sql.Fill(gr);
            }
            id_s_gr = Convert.ToInt32(gr.Tables[0].Rows[0]["id_service"]);//ID групповой тренировки

            //Запрос на информацию о посещениях клиента по выбранной тренировки
            string visit = "select Training.id_training, Training.id_service,Training.count " +
                "from Service join Training on Service.id_service=Training.id_service join Client " +
                "on Training.id_client = Client.id_client where Service.id_service =" + id_s_gr.ToString() +
                " and Client.id_client =" + id_cl.ToString();
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                gr1 = new DataSet();
                sql = new SqlDataAdapter(visit, podkl);
                sql.Fill(gr1);
            }

            if (gr1.Tables[0].Rows.Count == 0)//ЕСЛИ У КЛИЕНТА ЕЩЕ НЕ БЫЛО ЗАНЯТИЙ 
            {
                string max_trein = "select MAX(id_training) from Training";//ПОИСК МАКС ID
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    m_g = new DataSet();
                    sql = new SqlDataAdapter(max_trein, podkl);
                    sql.Fill(m_g);
                }
                max_gr = Convert.ToInt32(m_g.Tables[0].Rows[0][0]) + 1;
                //ДОБАВЛЕНИЕ ПЕРВОГО ПОСЕЩЕНИЯ 
                string ins = "insert into Training (id_training,id_service,id_client,date,count) " +
                "values(" + max_gr.ToString() + "," + id_s_gr.ToString() + "," + id_cl.ToString() + ",GETDATE(), 1)";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    gr2 = new DataSet();
                    sql = new SqlDataAdapter(ins, podkl);
                    sql.Fill(gr2);
                }

                MessageBox.Show("У клиента зафиксированно первое групповое занятие", "Посещение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                vivod();
            }
            else
            {

                count_gr = Convert.ToInt32(gr1.Tables[0].Rows[0]["count"]);//КОЛ-ВО ПОСЕЩЕНИЙ КЛИЕНТА ПО АБОНЕМЕНТУ
                count_gr += 1;
                string upd = "update Training set count=" + count_gr + " where id_client=" + id_cl + "and id_service=" + id_s_gr;
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    up = new DataSet();
                    sql = new SqlDataAdapter(upd, podkl);
                    sql.Fill(up);
                }
                MessageBox.Show("Посещение групповой тренировки добавленно", "Посещение", MessageBoxButtons.OK);
                vivod();
            }

        }
        //*******************************************************************
        //********************ПЕРЕХОД НА ДРУГИЕ ФОРМЫ*******************************
        private void button1_Click(object sender, EventArgs e)
        {
            Start start = new Start();
            start.Show();
            this.Hide();
        } 
        //*************************************************************************

        //**************************ВЫВОД КЛИЕНТА**********************************
        private void Admin_Load(object sender, EventArgs e)//Вывод клиентов
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "gymDataSet.Service". При необходимости она может быть перемещена или удалена.
            this.serviceTableAdapter.Fill(this.gymDataSet.Service);
            string vivod = "select Client.FIO, Status.status from Client inner join Status on Client.id_status=Status.id_status";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(vivod, podkl);
                sql.Fill(bd);
                dataGridView1.DataSource = bd.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "ФИО";
            }
        }

        private void button4_Click(object sender, EventArgs e)//Поиск клиентов
        {
            string search = "select Client.FIO, Status.status from Client inner join Status " +
               "on Client.id_status=Status.id_status where FIO like '" + textBox1.Text + "%'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(search, podkl);
                sql.Fill(bd);
                dataGridView1.DataSource = bd.Tables[0];
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            name_cl = dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();//ИМЯ Выбранного клиента
            string click = "select id_client,FIO from Client where FIO='" + name_cl + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd1 = new DataSet();
                sql = new SqlDataAdapter(click, podkl);
                sql.Fill(bd1);
            }
            id_cl = Convert.ToInt32(bd1.Tables[0].Rows[0][0]);//ID Выбранного клиента 
            
        }
        //*************************************************************************

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.serviceTableAdapter.FillBy(this.gymDataSet.Service);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }
    }
}
