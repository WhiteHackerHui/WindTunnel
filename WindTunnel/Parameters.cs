using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindTunnel
{
    public static class Parameters
    {
        //行情和交易参数
        static public string[] InvestorIDs { get { return new string[] { ""}; } }
        static public string[] Passwords { get { return new string[] { "" }; } }
        static public string[] BrokerIDs { get { return new string[] { "" }; } }
        static public Dictionary<string,Tuple<string,string>> FrontAddress
        {
            get
            {
                Dictionary<string,Tuple<string,string>> providers = new Dictionary<string, Tuple<string,string>>();
                providers.Add("电信1", new Tuple<string, string>( "tcp://180.168.146.187:10000", "tcp://180.168.146.187:10010" ));
                providers.Add("电信2", new Tuple<string, string>("tcp://180.168.146.187:10001", "tcp://180.168.146.187:10011" ));
                providers.Add("移动", new Tuple<string, string>("tcp://218.202.237.33:10002", "tcp://218.202.237.33:10012" ));
                providers.Add("CTPMini1", new Tuple<string, string>("tcp://180.168.146.187:10003", "tcp://180.168.146.187:10013"));
                providers.Add("CTP 7*24", new Tuple<string, string>("tcp://180.168.146.187:10030", "tcp://180.168.146.187:10031" ));
                return providers;
            }
        }

    }
}
