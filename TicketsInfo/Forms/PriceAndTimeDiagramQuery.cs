using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace TicketsInfo.Forms
{
    public partial class PriceANDTimeDiagram : Form
    {
        //控制数据采集的线程  
        Thread controlThread;
        string diagrampath = AppConfig.GetValue("diagrampath");
        string pricepath = AppConfig.GetValue("pricepath");
        string logpath = AppConfig.GetValue("logpath");
        //查询信息引用
        List<QueryItem> QueryList;
        List<QueryItem> FailQueryList = new List<QueryItem>();
        public static List<StationDictItem> StaList = new List<StationDictItem>();

        //初始化窗体
        public PriceANDTimeDiagram(List<QueryItem> _queryList)
        {
            InitializeComponent();
            this.QueryList = _queryList;
            CheckForIllegalCrossThreadCalls = false;
        }

        //启动或暂停查询
        private void button3_Click(object sender, EventArgs e)
        {
            if (controlThread == null)
            {
                controlThread = new Thread(new ThreadStart(DataGet));
                controlThread.Start();

                PrintTextBox("==========查询已启动==========");
                button3.Text = "点我暂停";
            }
            else
            {

                if (controlThread.ThreadState == ThreadState.Running)
                {
                    controlThread.Suspend();
                    PrintTextBox("==========查询已暂停==========");
                    button3.Text = "点我继续";
                }
                else if (controlThread.ThreadState == ThreadState.Suspended)
                {
                    controlThread.Resume();
                    PrintTextBox("==========查询已继续==========");
                    button3.Text = "点我暂停";
                }
                else if (controlThread.ThreadState == ThreadState.Unstarted)
                {
                    controlThread.Name = "数据获取线程";
                    controlThread.Start();
                    button3.Text = "点我暂停";
                }
            }
        }

        //数据获取方法(主流程)
        private void DataGet()
        {
            int i = 0;
            //遍历OD
            i = ExecuteQuery(i,this.QueryList);
            //失败查询重试
            int j = 0;
            while (this.FailQueryList.Count > 0)
            {
                if (j > 10) break;
                try
                {
                    i = ExecuteQuery(i, this.QueryList);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    j++;
                    Log();
                    this.QueryList = this.FailQueryList;
                    this.FailQueryList = new List<QueryItem>();
                }
            }
            PrintTextBox("================查询完成================");
            foreach (QueryItem qi in FailQueryList)
            {
                PrintTextBox(string.Format("{0},{1}", qi.fromStation, qi.toStation));
            }
           
        }

        private void Log()
        {
            StreamWriter FileWriter = new StreamWriter(logpath, true); //写文件
            FileWriter.Write(this.textBox1.Text);//将字符串写入
            FileWriter.Close(); //关闭StreamWriter对象
        }

        //执行个querylist的查询
        private int ExecuteQuery(int i, List<QueryItem> tempQueryList)
        {
            //重画progressBar
            progressBar1.Maximum = QueryList.Count;
            DataSet ds = new DataSet();
            foreach (QueryItem q in tempQueryList)
            {
                UpdateProcessBar();
                //默认查询第二天的余票
                string date = DateTime.Now.AddDays(1).Year + "-" + string.Format("{0:D2}", DateTime.Now.AddDays(1).Month) + "-" + string.Format("{0:D2}", DateTime.Now.AddDays(1).Day);
                //查询OD之间所有车次
                List<LeftTicketItem> list = LeftTicketsInfo.getFromInternet(date, q.fromStation, q.toStation);
                //遍历车次，获取时刻表
                if (list == null) continue;
                DataTable dt = CreateDiagramTable();
                DataTable dtPrice = CreatePriceTable();
                foreach (LeftTicketItem lti in list)
                {
                    //查询每个车次的时刻表
                    List<TimeDialgramItem> tdiList;
                    try
                    {
                        tdiList = TimeDiagramInfo.getFromInternet(date, Util.StaDict[q.fromStation].station_telecode, Util.StaDict[q.toStation].station_telecode, lti.train_no);
                    }
                    catch
                    {
                        continue;
                    }
                    if (tdiList == null) continue;
                    foreach (TimeDialgramItem tdi in tdiList)
                    {
                        dt.Rows.Add(
                            lti.train_no,
                            lti.station_train_code,
                            tdi.arrive_time,
                            tdi.start_time,
                            tdi.station_no,
                            tdi.station_name);
                    }
                    //查询每个车次的票价信息
                    if (tdiList.Exists(k => k.station_name == Util.StaDict[q.fromStation].station_name) &&
                        tdiList.Exists(k => k.station_name == Util.StaDict[q.toStation].station_name))
                    {
                        PrintTextBox(string.Format("开始查询票价信息{0}，{1}，{2}",
                            Util.StaDict[q.fromStation].station_name,
                            Util.StaDict[q.toStation].station_name,
                            lti.station_train_code
                            ));

                        TicketPriceItem tpi = GetSinglePriceInfo(date,
                            tdiList.Find(k => k.station_name == Util.StaDict[q.fromStation].station_name).station_no,
                            tdiList.Find(k => k.station_name == Util.StaDict[q.toStation].station_name).station_no,
                            lti.train_no, lti.seat_types);
                        if (tpi == null || tpi.IsEmpty())
                        {
                            PrintTextBox(string.Format("查询票价信息{0}，{1}，{2}失败",
                                Util.StaDict[q.fromStation].station_name,
                                Util.StaDict[q.toStation].station_name,
                                lti.station_train_code
                                ));
                            FailQueryList.Add(q);
                            continue;
                        }
                        dtPrice.Rows.Add(
                            tpi.train_no,
                            lti.station_train_code,
                            Util.StaDict[q.fromStation].station_name,
                            Util.StaDict[q.toStation].station_name,
                            date,
                            tpi.swz_price,
                            tpi.tz_price,
                            tpi.zy_price,
                            tpi.ze_price,
                            tpi.gr_price,
                            tpi.rw_price,
                            tpi.yw_price,
                            tpi.rz_price,
                            tpi.yz_price,
                            tpi.wz_price
                            );
                    }
                    Thread.Sleep(5000);
                }
                i++;
                dt.TableName = "diag_" + Util.StaDict[q.fromStation].station_name + "_" + Util.StaDict[q.toStation].station_name + "_" + i.ToString();
                dtPrice.TableName = "price_" + Util.StaDict[q.fromStation].station_name + "_" + Util.StaDict[q.toStation].station_name + "_" + i.ToString();
                ds.Tables.Add(dt);
                DataSet2CSV.Export2CSV(ds, dt.TableName, true, diagrampath + dt.TableName + ".csv");
                ds.Tables.Add(dtPrice);
                DataSet2CSV.Export2CSV(ds, dtPrice.TableName, true, pricepath + dtPrice.TableName + ".csv");
                PrintTextBox(string.Format("查询{0}，{1}成功", Util.StaDict[q.fromStation].station_name, Util.StaDict[q.toStation].station_name));
            }
            return i;
        }

        //向textbox输出
        private void PrintTextBox(string q)
        {
            textBox1.AppendText(q+"\r\n");
            textBox1.SelectionStart = textBox1.Text.Length;  //设定光标位置
            textBox1.ScrollToCaret();
            textBox1.Refresh();
        }
        
        //更新processBar
        private void UpdateProcessBar()
        {
            progressBar1.Value++;
            progressBar1.Refresh();
            label1.Text = progressBar1.Value + "/" + progressBar1.Maximum;
            label1.Refresh();
            Console.WriteLine(label1.Text);
        }

        /// <summary>
        /// 获取票价信息
        /// </summary>
        /// <param name="date">上车日期</param>
        /// <param name="fromNo">上车站站序</param>
        /// <param name="toNo">下车站站序</</param>
        /// <param name="trainNo">列车编号</param>
        /// <param name="seatTypes">席位种类</param>
        /// <returns></returns>
        private TicketPriceItem GetSinglePriceInfo(string date, string fromNo, string toNo, string trainNo, string seatTypes)
        {
            for (int i=0; i < 5; i++)
            {

                TicketPriceItem tpi = TicketPriceInfo.getFromInternet(date, fromNo, toNo, trainNo, seatTypes);
                PrintTextBox("第" + (i + 1) + "次尝试获取票价");
                if (tpi != null)
                {
                    return tpi;
                }
                else
                {
                    Thread.Sleep(5000+i*1000);//5+i000秒后重试
                    continue;
                }
                
                
            }
            return null;
        }

        private static DataTable CreatePriceTable()
        {
            DataTable dtPrice = new DataTable();
            dtPrice.Columns.Add("列车编号");
            dtPrice.Columns.Add("列车车次");
            dtPrice.Columns.Add("始发站");
            dtPrice.Columns.Add("终到站");
            dtPrice.Columns.Add("乘车日期");
            dtPrice.Columns.Add("商务座");
            dtPrice.Columns.Add("特等座");
            dtPrice.Columns.Add("一等座");
            dtPrice.Columns.Add("二等座");
            dtPrice.Columns.Add("高级软卧");
            dtPrice.Columns.Add("软卧");
            dtPrice.Columns.Add("硬卧");
            dtPrice.Columns.Add("软座");
            dtPrice.Columns.Add("硬座");
            dtPrice.Columns.Add("无座");
            return dtPrice;
        }

        private static DataTable CreateDiagramTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("列车编号");
            dt.Columns.Add("列车车次");
            dt.Columns.Add("到站时间");
            dt.Columns.Add("出发时间");
            dt.Columns.Add("车站顺序");
            dt.Columns.Add("车站名称");
            return dt;
        }

        private void PriceANDTimeDiagram_Shown(object sender, EventArgs e)
        {

        }

    }
}
