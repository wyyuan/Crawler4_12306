using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TicketsInfo.Forms.Forms.Forms.Forms
{
    public partial class TicketPriceOnly : Form
    {
        public TicketPriceOnly()
        {
            InitializeComponent();
        }

        private void TicketPriceOnly_Load(object sender, EventArgs e)
        {
            string url = "https://kyfw.12306.cn/otn/leftTicketPrice/init";
            //Console.WriteLine("当前执行的查询:{0}", url);
            string result = HttpHelper.GetResponseString(HttpHelper.CreateGetHttpResponse(url, 20, null, null));
        }

    }
}
