using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GumFitness
{
    public partial class Timer : Form
    {
        int s;
        public Timer()
        {
            InitializeComponent();
            s = 0;
        }

        private void Timer_Load(object sender, EventArgs e)
        {
            s = 0;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (s < 59)
            {
                s++;
                if (s < 10)
                    label2.Text = "0" + s.ToString();
                else label2.Text = s.ToString();
            }
            else
            {
                if (s > 59)
                {
                    label2.Text = "00";
                }
                label1.Text = "01";
            }

            if (label2.Text == "01")
            {
                timer1.Enabled = false;
                this.Hide();
            }
        }
    }
}
