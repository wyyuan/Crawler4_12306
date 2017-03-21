using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TicketsInfo.Forms
{
    public partial class InitForm : Form
    {
        public InitForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Util.InitStationDict();
            if (radioButton1.Checked == true)
            {
                Util.InitQueryListbyTelecode();
                MessageBox.Show("加载成功");

            }
            else if (radioButton2.Checked == true)
            {
                Util.InitQueryListbyName();
                MessageBox.Show("加载成功");
            }
            this.Close();
        }
    }
}
