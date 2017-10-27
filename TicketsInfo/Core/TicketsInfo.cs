using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Data;
using System.Configuration;

namespace TicketsInfo
{
    #region 基础类
    //HTTP基础类
    public class HttpHelper
    {
        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        public static HttpWebResponse CreateGetHttpResponse(string url, int timeout, string userAgent, CookieCollection cookies)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;    //http版本，默认是1.1,这里设置为1.0
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout;
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int timeout, string userAgent, CookieCollection cookies)
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.ASCII.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 获取请求的数据
        /// </summary>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();

            }
        }

        /// <summary>
        /// 验证证书
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return true;
        }

    }

    /// <summary>
    /// JSON解析类
    /// </summary>
    public static class JSONHelper
    {
        // 从一个Json串生成对象信息
        public static object JsonToObject(string jsonString, object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return serializer.ReadObject(mStream);
        }

    }

    /// <summary>  
    /// 配置信息维护类  
    /// </summary>  
    public class AppConfig
    {
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        /// <summary>  
        /// 获取配置值  
        /// </summary>  
        /// <param name="key">配置标识</param>  
        /// <returns></returns>  
        public static string GetValue(string key)
        {
            string result = string.Empty;
            if (config.AppSettings.Settings[key] != null)
                result = config.AppSettings.Settings[key].Value;
            return result;
        }

        /// <summary>  
        /// 修改或增加配置值  
        /// </summary>  
        /// <param name="key">配置标识</param>  
        /// <param name="value">配置值</param>  
        public static void SetValue(string key, string value)
        {
            if (config.AppSettings.Settings[key] != null)
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>  
        /// 删除配置值  
        /// </summary>  
        /// <param name="key">配置标识</param>  
        public static void DeleteValue(string key)
        {
            config.AppSettings.Settings.Remove(key);
        }
    }
    #endregion

    #region 余票查询模型
    [DataContract]
    public class LeftTicketResult
    {
        [DataMember(Order = 0)]
        public string validateMessagesShowId { get; set; }

        [DataMember(Order = 1)]
        public bool status { get; set; }

        [DataMember(Order = 2)]
        public int httpstatus { get; set; }

        [DataMember(Order = 3,IsRequired=false)]
        public LeftTicketData data { get; set; }

        [DataMember(Order = 4)]
        public string messages { get; set; }

        [DataMember(Order = 5)]
        public string validateMessages { get; set; }
    }

    [DataContract]
    public class LeftTicketData
    {
        [DataMember(Order = 0)]
        public List<LeftTicketItem> datas { get; set; }

        [DataMember(Order = 1)]
        public bool flag { get; set; }

        [DataMember(Order = 2)]
        public string searchDate { get; set; }

    }

    [DataContract]
    public class LeftTicketItem
    {

        [DataMember(Order = 0, IsRequired = true)]
        public string train_no { get; set; }

        [DataMember(Order = 1, IsRequired = true)]
        public string station_train_code { get; set; }

        [DataMember(Order = 12, IsRequired = true)]
        public string seat_types { get; set; }

        #region 席别余票
        [DataMember(Order = 2, IsRequired = true)]
        public string swz_num { get; set; }//商务座

        [DataMember(Order = 3, IsRequired = true)]
        public string tz_num { get; set; }//特等座
        
        [DataMember(Order = 4, IsRequired = true)]
        public string zy_num { get; set; }//一等座

        [DataMember(Order = 5, IsRequired = true)]
        public string ze_num { get; set; }//二等座

        [DataMember(Order = 6, IsRequired = true)]
        public string gr_num { get; set; }//高级软卧
        
        [DataMember(Order = 7, IsRequired = true)]
        public string rw_num { get; set; }//软卧

        [DataMember(Order = 8, IsRequired = true)]
        public string yw_num { get; set; }//硬卧
        
        [DataMember(Order = 9, IsRequired = true)]
        public string rz_num { get; set; }//软座

        [DataMember(Order = 10, IsRequired = true)]
        public string yz_num { get; set; }//硬座

        [DataMember(Order = 11, IsRequired = true)]
        public string wz_num { get; set; }//无座

        #endregion
    }
    #endregion

    #region 票价查询模型
    [DataContract]
    public class TicketPriceResult
    {
        [DataMember(Order = 0)]
        public string validateMessagesShowId { get; set; }

        [DataMember(Order = 1)]
        public bool status { get; set; }

        [DataMember(Order = 2)]
        public int httpstatus { get; set; }

        [DataMember(Order = 3, IsRequired = false)]
        public TicketPriceItem data { get; set; }

        [DataMember(Order = 4)]
        public string messages { get; set; }

        [DataMember(Order = 5)]
        public string validateMessages { get; set; }
    }

    [DataContract]
    public class TicketPriceItem
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string train_no { get; set; }

        #region 票价
        [DataMember(Order = 1, IsRequired = false, Name = "A9")]
        public string swz_price { get; set; }//商务座

        [DataMember(Order = 2, IsRequired = false, Name = "P")]
        public string tz_price { get; set; }//特等座

        [DataMember(Order = 3, IsRequired = false, Name = "M")]
        public string zy_price { get; set; }//一等座

        [DataMember(Order = 4, IsRequired = false, Name = "O")]
        public string ze_price { get; set; }//二等座

        [DataMember(Order = 5, IsRequired = false, Name = "A6")]
        public string gr_price { get; set; }//高级软卧

        [DataMember(Order = 6, IsRequired = false, Name = "A4")]
        public string rw_price { get; set; }//软卧

        [DataMember(Order = 7, IsRequired = false, Name = "A3")]
        public string yw_price { get; set; }//硬卧

        [DataMember(Order = 8, IsRequired = false, Name = "A2")]
        public string rz_price { get; set; }//软座

        [DataMember(Order = 9, IsRequired = false, Name = "A1")]
        public string yz_price { get; set; }//硬座

        [DataMember(Order = 10, IsRequired = false, Name = "WZ")]
        public string wz_price { get; set; }//无座
        #endregion

        public bool IsEmpty()
        {
            string s = this.swz_price + this.tz_price + this.zy_price + this.ze_price + this.gr_price + this.rw_price + this.yw_price + this.rz_price + this.yz_price + this.wz_price;
            if (s == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion

    #region 时刻表查询模型

    [DataContract]
    public class TimeDialgramResult
    {
        [DataMember(Order = 0)]
        public string validateMessagesShowId { get; set; }

        [DataMember(Order = 1)]
        public bool status { get; set; }

        [DataMember(Order = 2)]
        public int httpstatus { get; set; }

        [DataMember(Order = 3, IsRequired = false)]
        public TimeDialgramData data { get; set; }

        [DataMember(Order = 4)]
        public string messages { get; set; }

        [DataMember(Order = 5)]
        public string validateMessages { get; set; }
    }

    [DataContract]
    public class TimeDialgramData
    {
        [DataMember(Order = 0)]
        public List<TimeDialgramItem> data { get; set; }

    }
    [DataContract]
    public class TimeDialgramItem
    {
        [DataMember(Order = 0, IsRequired = true, Name = "arrive_time")]
        public string arrive_time { get; set; }//到站时间

        [DataMember(Order = 1, IsRequired = true, Name = "isEnabled")]
        public bool isEnabled { get; set; }

        [DataMember(Order = 2, IsRequired = true,Name="start_time")]
        public string start_time { get; set; }//出发时间

        [DataMember(Order = 3, IsRequired = true,Name="station_name")]
        public string station_name { get; set; }//车站名称

        [DataMember(Order = 4, IsRequired = true,Name="station_no")]
        public string station_no { get; set; }//车站顺序    

        [DataMember(Order = 5, IsRequired = true,Name="stopover_time")]
        public string stopover_time { get; set; }//停车时间
    }
    #endregion

    #region 基础业务类
    // 车站
    public class StationDictItem
    {

        [DataMember(Order = 1, IsRequired = true)]
        public string id { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string station_name { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string station_telecode { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public string pingyin { get; set; }

        [DataMember(Order = 5, IsRequired = true)]
        public string pingyin2 { get; set; }

        public StationDictItem(string _id, string _station_name, string _station_telecode, string _pingyin, string _pingyin2)
        {
            this.id = _id;
            this.station_name = _station_name;
            this.station_telecode = _station_telecode;
            this.pingyin = _pingyin;
            this.pingyin2 = _pingyin2;
        }
    }

    // 查询参数
    public class QueryItem
    {
        [DataMember(Order = 0, IsRequired = true)]
        public int id { get; set; }

        [DataMember(Order = 1, IsRequired = true)]
        public string fromStation { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string toStation { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public DateTime date { get; set; }
    }

    // 通用信息
    public static class Util
    {
        //查询列表
        public static List<QueryItem> QueryList;

        //失败查询列表
        //public static List<QueryItem> FailQueryList;

        //车站字典
        public static Dictionary<string, StationDictItem> StaDict;

        public static void InitQueryListbyTelecode()
        {
            QueryList = new List<QueryItem>();
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
                    QueryList.Add(q);
                }
                catch
                {
                    //do nothing but continue
                    continue;
                }

            }
        }

        public static void InitQueryListbyName()
        {
            if (StaDict == null || StaDict.Count == 0) return;//需要初始化车站信息            

            QueryList = new List<QueryItem>();
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
                    q.fromStation = translate(ss[0]);
                    q.toStation = translate(ss[1]);
                    q.date = Convert.ToDateTime(ss[2]);
                    QueryList.Add(q);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    continue;
                }

            }
        }

        public static void InitStationDict()
        {
            StaDict = new Dictionary<string, StationDictItem>();
            //读取dict
            StreamReader sr = new StreamReader(@"Data/sta_dict.txt", Encoding.Default);
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                try
                {
                    string[] ss = s.Split(',');
                    StaDict.Add(ss[2], new StationDictItem(ss[0], ss[1], ss[2], ss[3], ss[4]));
                }
                catch
                {
                    //do nothing but continue
                    continue;
                }
            }
        }

        private static string translate(string staname)
        {
            foreach (StationDictItem sdi in Util.StaDict.Values)
            {
                if (sdi.station_name == staname)
                {
                    return sdi.station_telecode;
                }
            }
            throw new Exception("车站名未出现在字典中");
        }
    }
    #endregion

    //余票信息查询
    public class LeftTicketsInfo
    {
        /// <summary>
        /// 车站电报码字典
        /// </summary>
        public static Dictionary<string, string> TelecodeDict = new Dictionary<string, string>();
        public static List<QueryItem> QueryList = new List<QueryItem>();
        /// <summary>
        /// 查询一次一对OD间的车次并输出至CSV文件中
        /// </summary>
        /// <param name="line">输入参数</param>
        /// <param name="de">查询次数</param>
        /// <returns></returns>
        public static string getSingleTrainTicketsLeft(string line,int de)
        {
            string savepath = AppConfig.GetValue("savepath");
            DataSet ds = new DataSet();
            string[] s = line.ToString().Split(',');
            DataTable dt = LeftTicketsInfo.GetSingleTicketsInfoByTelecode(s[2], s[0], s[1]);
            if (dt == null) 
            {
                return "获取" + s[2] +"_"+LeftTicketsInfo.TelecodeDict[s[0]] +"-"+ LeftTicketsInfo.TelecodeDict[s[1]] + "失败";
            }else{
                ds.Tables.Add(dt);
                //在目标文件夹下创建每日的采集数据文件夹
                string date = DateTime.Now.Date.Year + "-" + DateTime.Now.Date.Month+"-" + DateTime.Now.Date.Day;
                //DirectoryDateTime.Now.Date.Year + "-" + DateTime.Now.Date.Month+"-" + DateTime.Now.Date.Day
                if(!Directory.Exists(savepath+date))
                {
                    Directory.CreateDirectory(savepath+date);
                }
                DataSet2CSV.Export2CSV(ds, dt.TableName, true, savepath + date +@"/"+ dt.TableName + "_" + de + ".csv");
                return "获取" + s[2] + "_" + LeftTicketsInfo.TelecodeDict[s[0]] + "-" + LeftTicketsInfo.TelecodeDict[s[1]] + "成功";
            }
        }
        public static string getSingleTrainTicketsLeft(DateTime date,string fromStation,string toStation, int de)
        {
            return getSingleTrainTicketsLeft(fromStation + "," + toStation + "," + date.Year + "-" + string.Format("{0:D2}", date.Month) + "-" + string.Format("{0:D2}", date.Day), de);
        }
        /// <summary>
        /// 查询两站间所有车次余票信息，输出至DataTable中。
        /// </summary>
        /// <param name="date">乘车日期</param>
        /// <param name="fromStation">出发车站电报码</param>
        /// <param name="toStation">到达车站电报码</param>
        /// <returns></returns>
        public static DataTable GetSingleTicketsInfoByTelecode(string date, string fromStation, string toStation)
        {
           List<LeftTicketItem> items = getFromInternet(date, fromStation, toStation);
            if (items == null) return null;
            DataTable dt = new DataTable();
            
            dt.Columns.Add("列车车次");
            dt.Columns.Add("商务座");
            dt.Columns.Add("特等座");
            dt.Columns.Add("一等座");
            dt.Columns.Add("二等座");
            dt.Columns.Add("高级软卧");
            dt.Columns.Add("软卧");
            dt.Columns.Add("硬卧");
            dt.Columns.Add("软座");
            dt.Columns.Add("硬座");
            dt.Columns.Add("无座");
            dt.Columns.Add("乘车日期");
            dt.Columns.Add("始发站");
            dt.Columns.Add("终到站");
            dt.Columns.Add("查询时刻");


            foreach (LeftTicketItem item in items)
            {
                dt.Rows.Add(
                item.station_train_code,
                item.swz_num,
                item.tz_num,
                item.zy_num,
                item.ze_num,
                item.gr_num,
                item.rw_num,
                item.yw_num,
                item.rz_num,
                item.yz_num,
                item.wz_num,
                date,
                fromStation,
                toStation,
                DateTime.Now.ToString()
                );
            }
            dt.TableName = date + "_" +LeftTicketsInfo.TelecodeDict[fromStation] + "-" + LeftTicketsInfo.TelecodeDict[toStation] + "_" +
                DateTime.Now.Date.Year + "-" + DateTime.Now.Date.Month+"-" + DateTime.Now.Date.Day;
            return dt;
            
        }
        public static List<LeftTicketItem> getFromInternet(string date, string fromStation, string toStation)
        {
            try
            {
                string url = "https://kyfw.12306.cn/otn/lcxxcx/query?purpose_codes=ADULT&queryDate=" + date + "&from_station=" + fromStation + "&to_station=" + toStation;
                //Console.WriteLine("当前执行的查询:{0}",url);
                string result = HttpHelper.GetResponseString(HttpHelper.CreateGetHttpResponse(url, 20, null, null));
                LeftTicketResult obj = (LeftTicketResult)JSONHelper.JsonToObject(result, new LeftTicketResult());
                return obj.data.datas;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        public static void UpdateQueryDate()
        {
            foreach (QueryItem q in QueryList)
            {
                q.date = new DateTime(q.date.Year, q.date.Month, q.date.Day + 1);
            }
        }
    }

    //票价信息查询
    public class TicketPriceInfo
    {
        //获取单一车次票价信息
        public static TicketPriceItem getFromInternet(string Date, string FromStationNo, string ToStationNo, string Train_no,string SeatTypes)
        {
            try
            {
                string url = "https://kyfw.12306.cn/otn/leftTicket/queryTicketPrice?train_no="+Train_no+"&from_station_no=" + FromStationNo + "&to_station_no=" + ToStationNo+"&seat_types="+SeatTypes+"&train_date=" + Date;
                //Console.WriteLine("当前执行的查询:{0}", url);
                string result = HttpHelper.GetResponseString(HttpHelper.CreateGetHttpResponse(url, 20, null, null));
                TicketPriceResult obj = (TicketPriceResult)JSONHelper.JsonToObject(result, new TicketPriceResult());
                return obj.data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }      
        
    }

    //时刻表信息查询
    public class TimeDiagramInfo
    {
        //获取单一车次时刻表信息
        public static List<TimeDialgramItem> getFromInternet(string Date, string FromStationTelecode, string ToStationTelecode, string Train_no)
        {
            try
            {
                string url = "https://kyfw.12306.cn/otn/czxx/queryByTrainNo?train_no=" + Train_no + "&from_station_telecode=" + FromStationTelecode + "&to_station_telecode=" + ToStationTelecode + "&depart_date=" + Date;
                //Console.WriteLine("当前执行的查询:{0}", url);
                string result = HttpHelper.GetResponseString(HttpHelper.CreateGetHttpResponse(url, 20, null, null));
                TimeDialgramResult obj = (TimeDialgramResult)JSONHelper.JsonToObject(result, new TimeDialgramResult());
                return obj.data.data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}


