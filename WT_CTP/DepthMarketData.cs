using System.ComponentModel;
using System.Text.RegularExpressions;

namespace WT_CTP
{
    /// <summary>
    /// 对CTP行情数据进行封装后的类型
    /// </summary>
    public sealed class DepthMarketData : INotifyPropertyChanged
    {
        private CThostFtdcDepthMarketDataField _dmdf;
        //private bool _isNotified;

        [DisplayName("卖一价")]
        public double AskPrice1 { get { return _dmdf.AskPrice1; } }
        [DisplayName("卖二价")]
        public double AskPrice2 { get { return _dmdf.AskPrice2; } }
        [DisplayName("卖三价")]
        public double AskPrice3 { get { return _dmdf.AskPrice3; } }
        [DisplayName("卖四价")]
        public double AskPrice4 { get { return _dmdf.AskPrice4; } }
        [DisplayName("卖五价")]
        public double AskPrice5 { get { return _dmdf.AskPrice5; } }
        [DisplayName("卖一量")]
        public int AskVolume1 { get { return _dmdf.AskVolume1; } }
        [DisplayName("卖二量")]
        public int AskVolume2 { get { return _dmdf.AskVolume2; } }
        [DisplayName("卖三量")]
        public int AskVolume3 { get { return _dmdf.AskVolume3; } }
        [DisplayName("卖四量")]
        public int AskVolume4 { get { return _dmdf.AskVolume4; } }
        [DisplayName("卖五量")]
        public int AskVolume5 { get { return _dmdf.AskVolume5; } }
        [DisplayName("均价")]
        public double AveragePrice { get { return _dmdf.AveragePrice; } }
        [DisplayName("卖一价")]
        public double BidPrice1 { get { return _dmdf.BidPrice1; } }
        [DisplayName("卖二价")]
        public double BidPrice2 { get { return _dmdf.BidPrice2; } }
        [DisplayName("卖三价")]
        public double BidPrice3 { get { return _dmdf.BidPrice3; } }
        [DisplayName("卖四价")]
        public double BidPrice4 { get { return _dmdf.BidPrice4; } }
        [DisplayName("卖五价")]
        public double BidPrice5 { get { return _dmdf.BidPrice5; } }
        [DisplayName("卖一量")]
        public int BidVolume1 { get { return _dmdf.BidVolume1; } }
        [DisplayName("卖二量")]
        public int BidVolume2 { get { return _dmdf.BidVolume2; } }
        [DisplayName("卖三量")]
        public int BidVolume3 { get { return _dmdf.BidVolume3; } }
        [DisplayName("卖四量")]
        public int BidVolume4 { get { return _dmdf.BidVolume4; } }
        [DisplayName("卖五量")]
        public int BidVolume5 { get { return _dmdf.BidVolume5; } }
        [DisplayName("今收盘价")]
        public double ClosePrice { get { return _dmdf.ClosePrice; } }
        [DisplayName("今虚实度")]
        public double CurrDelta { get { return _dmdf.CurrDelta; } }
        [DisplayName("交易所代码")]
        public string ExchangeID { get { return _dmdf.ExchangeID; } }
        [DisplayName("合约在交易所的代码")]
        public string ExchangeInstID { get { return _dmdf.ExchangeInstID; } }
        [DisplayName("最高价")]
        public double HighestPrice { get { return _dmdf.HighestPrice; } }
        [DisplayName("合约代码")]
        public string InstrumentID { get { return _dmdf.InstrumentID; } }
        [DisplayName("最新价")]
        public double LastPrice { get { return _dmdf.LastPrice; } }
        [DisplayName("跌停板价")]
        public double LowerLimitPrice { get { return _dmdf.LowerLimitPrice; } }
        [DisplayName("最低价")]
        public double LowestPrice { get { return _dmdf.LowestPrice; } }
        [DisplayName("持仓量")]
        public double OpenInterest { get { return _dmdf.OpenInterest; } }
        [DisplayName("今开盘价")]
        public double OpenPrice { get { return _dmdf.OpenPrice; } }
        [DisplayName("昨收盘价")]
        public double PreClosePrice { get { return _dmdf.PreClosePrice; } }
        [DisplayName("昨虚实度")]
        public double PreDelta { get { return _dmdf.PreDelta; } }
        [DisplayName("昨持仓量")]
        public double PreOpenInterest { get { return _dmdf.PreOpenInterest; } }
        [DisplayName("上次结算价")]
        public double PreSettlementPrice { get { return _dmdf.PreSettlementPrice; } }
        [DisplayName("本次结算价")]
        public double SettlementPrice { get { return _dmdf.SettlementPrice; } }
        [DisplayName("交易日")]
        public string TradingDay { get { return _dmdf.TradingDay; } }
        [DisplayName("成交金额")]
        public double Turnover { get { return _dmdf.Turnover; } }
        [DisplayName("最后修改毫秒")]
        public int UpdateMillisec { get { return _dmdf.UpdateMillisec; } }
        [DisplayName("最后修改时间")]
        public string UpdateTime { get { return _dmdf.UpdateTime; } }
        [DisplayName("涨停板价")]
        public double UpperLimitPrice { get { return _dmdf.UpperLimitPrice; } }
        [DisplayName("成交数量")]
        public int Volume { get { return _dmdf.Volume; } }
        [DisplayName("期货、期权种类代码")]
        public string ProductID { get; set; }
        [DisplayName("CTP行情数据实例")]
        public CThostFtdcDepthMarketDataField CThostFtdcMarketDepthDataFieldInstance
        {
            get { return _dmdf; }
            set
            {
                this._dmdf = value;
                //Notify("");
                Notify("AskPrice1"); Notify("AskVolume1");
                Notify("BidPrice1"); Notify("BidVolume1");
                Notify("LastPrice"); Notify("BidVolume1");
                Notify("Volume"); Notify("OpenInterest");
                Notify("HighestPrice"); Notify("LowestPrice");
                Notify("AveragePrice"); Notify("UpdateTime");
                Notify("Turnover"); Notify("UpdateMillisec");
                //this._isNotified = true;
            }
        }
        //[DisplayName("是否被Notify过？")]
        //public bool IsNotified { get { return this._isNotified; } }

        ///// <summary>
        ///// 构造函数1
        ///// </summary>
        //public DepthMarketData(CThostFtdcDepthMarketDataField dmdf)
        //{
        //    this.CThostFtdcMarketDepthDataFieldInstance = dmdf;
        //    Notify("");
            
        //}
        /// <summary>
        /// 构造函数
        /// </summary>
        public DepthMarketData()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string PropertyName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName)); }

    }

}
