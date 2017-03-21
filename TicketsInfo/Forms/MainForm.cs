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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void 票价数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PriceANDTimeDiagram FormPrice = new PriceANDTimeDiagram(Util.QueryList);
            FormPrice.MdiParent = this;
            FormPrice.Show();
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingForm FormSet = new SettingForm();
            FormSet.ShowDialog();
        }

        private void 输入查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitForm form = new InitForm();
            form.ShowDialog();
        }

        private void Form_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;

        }

        private void 时刻表数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeDiagram FormTT = new TimeDiagram(Util.QueryList);
            FormTT.MdiParent = this;
            FormTT.Show();
        }

        private void 余票数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MultiLeftTicketQuery Form = new MultiLeftTicketQuery();
            Form.MdiParent = this;
            Form.Show();
        }
    }
}
