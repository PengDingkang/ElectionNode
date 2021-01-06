using System;
using System.Configuration;

namespace WebNode
{
    public class GlobalVars
    {
        public static readonly int NodeAmount = Convert.ToInt32(ConfigurationManager.AppSettings["NodeAmount"]);
        public static readonly int NodeNumber = Convert.ToInt32(ConfigurationManager.AppSettings["NodeNumber"]);
        public static readonly string listenPort = ConfigurationManager.AppSettings["ListenPort"];
        public static readonly string[] OtherNodes = ConfigurationManager.AppSettings["OtherNodes"].Split(",");
    }
}
