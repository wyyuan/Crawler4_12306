using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TicketsInfo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            //string url = @"https://kyfw.12306.cn/otn/leftTicketPrice/init";
            //string result = HttpHelper.GetResponseString(HttpHelper.CreateGetHttpResponse(url, 20, null, null));
            //url = @"https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module=other&rand=sjrand";
            //object obj = HttpHelper.CreateGetHttpResponse(url, 20, null, null).GetResponseStream();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TicketsInfo.Forms.MainForm());
            //Application.Run(new TicketsInfo.Forms.Forms.Forms.Forms.TicketPriceOnly());
        }
    }
}
