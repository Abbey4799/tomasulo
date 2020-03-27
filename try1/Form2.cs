using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace try1
{
    public partial class Form2 : Form
    {
        public Form1 parent;
        public int Num_LOAD_RS =0 ;
        public int Num_ADD_RS = 0 ;
        public int Num_MULT_RS= 0;
        public int Num_LOAD_lat = 0;
        public int Num_ADD_lat = 0;
        public int Num_MULT_lat = 0;
        public int Num_DIV_lat = 0;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            parent.Show();

            Hide();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Num_LOAD_RS = int.Parse(textBox1.Text);
            Num_ADD_RS = int.Parse(textBox2.Text);
            Num_MULT_RS = int.Parse(textBox3.Text);
            Num_LOAD_lat = int.Parse(textBox4.Text);
            Num_ADD_lat = int.Parse(textBox5.Text);
            Num_MULT_lat = int.Parse(textBox6.Text);
            Num_DIV_lat = int.Parse(textBox7.Text);
            parent.SetResStation(Num_LOAD_RS, Num_ADD_RS, Num_MULT_RS, Num_LOAD_lat, Num_ADD_lat, Num_MULT_lat, Num_DIV_lat);
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }
}
