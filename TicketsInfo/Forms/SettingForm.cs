using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace TicketsInfo.Forms
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
            textBox1.Text = AppConfig.GetValue("listpath");
            textBox2.Text = AppConfig.GetValue("savepath");
            textBox3.Text = AppConfig.GetValue("pricepath");
            textBox4.Text = AppConfig.GetValue("diagrampath");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AppConfig.SetValue("listpath",textBox1.Text);
            AppConfig.SetValue("savepath",textBox2.Text);
            AppConfig.SetValue("pricepath", textBox3.Text);
            AppConfig.SetValue("diagrampath", textBox4.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    
}
