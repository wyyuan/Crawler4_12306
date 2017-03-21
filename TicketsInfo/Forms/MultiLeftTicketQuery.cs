using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace TicketsInfo.Forms
{
    public partial class MultiLeftTicketQuery : Form
    {

        public MultiLeftTicketQuery()
        {
            InitializeComponent();
            //Control.CheckForIllegalCrossThreadCalls = false;
        }
        
        //当前进行的循环数
        int de = 1;
        int interval = 1000;
        //控制数据采集的线程  
        Thread controlThread;

        //采集一次数据线程  
        Thread workThread;

        //此委托允许异步的调用为Listbox添加Item  
        delegate void detailInfoUpdateCallback(string text);

        //这种方法演示如何在线程安全的模式下调用Windows窗体上的控件。  
        private void detailInfoUpdate(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                detailInfoUpdateCallback d = new detailInfoUpdateCallback(detailInfoUpdate);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.AppendText(text);
                textBox1.SelectionStart = textBox1.Text.Length;  //设定光标位置
                textBox1.ScrollToCaret();
                textBox1.Refresh();
            }
        }

        //此委托允许异步的调用为Listbox添加Item  
        delegate void cleanDetailInfoUpdateCallback();

        //这种方法演示如何在线程安全的模式下调用Windows窗体上的控件。  
        private void cleanDetailInfoUpdate()
        {
            if (this.textBox1.InvokeRequired)
            {
                cleanDetailInfoUpdateCallback d = new cleanDetailInfoUpdateCallback(cleanDetailInfoUpdate);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.textBox1.Text = "";
            }
        }


        //此委托允许异步的调用为Listbox添加Item  
        delegate void InfoUpdateCallback(string text);

        //这种方法演示如何在线程安全的模式下调用Windows窗体上的控件。  
        private void InfoUpdate(string text)
        {
            if (this.textBox2.InvokeRequired)
            {
                InfoUpdateCallback d = new InfoUpdateCallback(InfoUpdate);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox2.AppendText(text);
                textBox2.SelectionStart = textBox2.Text.Length;  //设定光标位置
                textBox2.ScrollToCaret();
                textBox2.Refresh();
            }
        }


        //一次数据采集方法  
        public void DataGet()
        {
            int i = 1;
            foreach (QueryItem q in LeftTicketsInfo.QueryList)
            {
                try
                {
                    detailInfoUpdate(LeftTicketsInfo.getSingleTrainTicketsLeft(q.date,q.fromStation,q.toStation, de) + ",当前第" + i++ + "行\r\n");
                    Thread.Sleep(interval);
                }
                catch
                {
                    //do nothing but continue
                    continue;
                }
            }
            //如果此方法退出了，那么这个线程也就退出了
        }

        public void Control()
        {
            DateTime lastQueryTime = DateTime.Now;
            int period = Convert.ToInt32(AppConfig.GetValue("period"));
            interval = Convert.ToInt32(AppConfig.GetValue("Interval"));
            while (true)
            {
                //判断当前时间距离上一次查询是否跨天
                if (lastQueryTime.Day != DateTime.Now.Day)
                {
                    LeftTicketsInfo.UpdateQueryDate();
                }
                
                InfoUpdate("当前第" + de + "次查询，当前时间:"+DateTime.Now.ToString()+"\r\n");
                cleanDetailInfoUpdate();
                de++;
                workThread = new Thread(new ThreadStart(DataGet));
                workThread.Name = "数据获取线程";
                workThread.Start();

                lastQueryTime = DateTime.Now;
                Thread.Sleep(period);
            }
        }

        #region 按钮事件
        private void button1_Start(object sender, EventArgs e)
        {
            controlThread = new Thread(new ThreadStart(Control));
            controlThread.Name = "流程控制线程";
            controlThread.Start();
        }

        private void button2_Setting(object sender, EventArgs e)
        {
            SettingForm set = new SettingForm();
            set.ShowDialog();
        }

        private void button1_Resume(object sender, EventArgs e)
        {
            if(workThread.ThreadState == ThreadState.Suspended)
            {
                controlThread.Resume();
            }
            controlThread.Abort();
            workThread.Abort();
        }

        private void button2_Suspend(object sender, EventArgs e)
        {
            if (workThread.ThreadState == ThreadState.Running)
            {
                controlThread.Suspend();
            }
            else if(workThread.ThreadState == ThreadState.Suspended)
            {
                controlThread.Resume();
            }
        }
        #endregion

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                //this.notifyIcon1.Visible = true;
            } 

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            //this.notifyIcon1.Visible = false; 

        }

        //初始化静态类
        private void Form2_Shown(object sender, EventArgs e)
        {
            InitStationDict();
            InitQueryList();
        }

        private  void InitQueryList()
        {
            LeftTicketsInfo.QueryList = new List<QueryItem>();
            //读取list
            string listpath = AppConfig.GetValue("listpath");
            StreamReader sr1 = new StreamReader(listpath, Encoding.Default);
            int i = 1;
            string line;
            while ((line = sr1.ReadLine()) != null)
            {
                try
                {
                    string[] ss = line.ToString().Split(',');
                    QueryItem q = new QueryItem();
                    q.id = i++;
                    q.fromStation = ss[0];
                    q.toStation = ss[1];
                    q.date = Convert.ToDateTime(ss[2]);
                    LeftTicketsInfo.QueryList.Add(q);
                }
                catch
                {
                    //do nothing but continue
                    continue;
                }

            }
        }

        private  void InitStationDict()
        {
            //读取dict
            StreamReader sr = new StreamReader(@"Data/dict.txt", Encoding.Default);
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                try
                {
                    string[] ss = s.Split(',');
                    LeftTicketsInfo.TelecodeDict.Add(ss[0], ss[1]);
                }
                catch
                {
                    //do nothing but continue
                    continue;
                }
            }
        }
    }
}
