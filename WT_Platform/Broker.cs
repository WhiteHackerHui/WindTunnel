﻿using System.ComponentModel;

namespace WT_Platform
{
    public class Broker
    {
        [DisplayName("服务器")]
        public string Name { get; set; }
        [DisplayName("经纪商代码")]
        public string BrokerID { get; set; }
        [DisplayName("交易地址")]
        public string TradeIP { get; set; }
        [DisplayName("交易端口")]
        public ushort TradePort { get; set; }
        [DisplayName("行情地址")]
        public string QuoteIP { get; set; }
        [DisplayName("行情端口")]
        public ushort QuotePort { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
