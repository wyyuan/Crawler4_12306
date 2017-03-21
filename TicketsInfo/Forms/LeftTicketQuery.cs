using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections.Generic;

namespace TicketsInfo.Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string date = textBox1.Text;
            string fromStation = textBox2.Text;
            string toStation = textBox3.Text;
            string url = "https://kyfw.12306.cn/otn/lcxxcx/query?purpose_codes=ADULT&queryDate=" + date + "&from_station=" + fromStation + "&to_station=" + toStation;
            string result = HttpHelper.GetResponseString(HttpHelper.CreateGetHttpResponse(url, 20, null, null));
            LeftTicketResult obj = (LeftTicketResult)JsonToObject(result, new LeftTicketResult());
            listView1.Items.Clear();
            foreach (LeftTicketItem item in obj.data.datas)
            {
                ListViewItem it = new ListViewItem();
                it.SubItems.Add(item.station_train_code);
                it.SubItems.Add(item.swz_num);
                it.SubItems.Add(item.tz_num);
                it.SubItems.Add(item.zy_num);
                it.SubItems.Add(item.ze_num);
                it.SubItems.Add(item.gr_num);
                it.SubItems.Add(item.rw_num);
                it.SubItems.Add(item.yw_num);
                it.SubItems.Add(item.rz_num);
                it.SubItems.Add(item.yz_num);
                it.SubItems.Add(item.wz_num);
                listView1.Items.Add(it);
            }
            
        }
        // 从一个Json串生成对象信息
        public static object JsonToObject(string jsonString, object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return serializer.ReadObject(mStream);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}
