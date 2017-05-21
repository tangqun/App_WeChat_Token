using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log4Net.config", Watch = true)]
namespace Helper_9H
{
    public class LogHelper
    {
        private static readonly ILog logError = LogManager.GetLogger("logError");
        private static readonly ILog logInfo = LogManager.GetLogger("logInfo");
        
        public static void Error(string authorName, Exception ex)
        {
            logError.Error("作者：" + authorName + "，日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "，堆栈信息：" + ex.StackTrace + "，异常信息：" + ex.Message);
        }

        public static void Info(string msg)
        {
            logInfo.Info(msg);
        }
    }
}
