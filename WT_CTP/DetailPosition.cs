using System;
using System.ComponentModel;
using WT_Core;

namespace WT_CTP
{
    /// <summary>
    /// 封装持仓细节的类型
    /// </summary>
    public sealed class DetailPosition : INotifyPropertyChanged, IEquatable<DetailPosition>
    {
        #region ==========内部变量==========

        private CThostFtdcInvestorPositionDetailField _ipdf;

        #endregion ==========内部变量==========

        #region ==========属性==========
        
        [DisplayName("经纪公司代码")] 
        public string BrokerID { get { return _ipdf.BrokerID; } }
        [DisplayName("平仓金额")]
        public double CloseAmount { get { return _ipdf.CloseAmount; } }
        [DisplayName("逐日盯市平仓盈亏")]
        public double CloseProfitByDate { get { return _ipdf.CloseProfitByDate; } }
        [DisplayName("逐笔对冲平仓盈亏")]
        public double CloseProfitByTrade { get { return _ipdf.CloseProfitByTrade; } }
        [DisplayName("平仓量")]
        public int CloseVolume { get { return _ipdf.CloseVolume; } }
        [DisplayName("组合合约代码")]
        public string CombInstrumentID { get { return _ipdf.CombInstrumentID; } }
        [DisplayName("买卖方向")]
        public Direction Direction { get { return ConvertFunctions.TThostFtdcDirectionType_To_Direction(_ipdf.Direction); } }
        [DisplayName("交易所代码")]
        public string ExchangeID { get { return _ipdf.ExchangeID; } }
        [DisplayName("交易所保证金")]
        public double ExchMargin { get { return _ipdf.ExchMargin; } }
        [DisplayName("投机套保标志")]
        public HedgeFlag HedgeFlag { get { return ConvertFunctions.TThostFtdcHedgeFlagType_To_HedgeFlag(_ipdf.HedgeFlag); } }
        [DisplayName("合约代码")]
        public string InstrumentID { get { return _ipdf.InstrumentID; } }
        [DisplayName("投资者代码")]
        public string InvestorID { get { return _ipdf.InvestorID; } }
        [DisplayName("昨结算价")]
        public double LastSettlementPrice { get { return _ipdf.LastSettlementPrice; } }
        [DisplayName("投资者保证金")]
        public double Margin { get { return _ipdf.Margin; } }
        [DisplayName("保证金率(按金额)")]
        public double MarginRateByMoney { get { return _ipdf.MarginRateByMoney; } }
        [DisplayName("保证金率(按手数)")]
        public double MarginRateByVolume { get { return _ipdf.MarginRateByVolume; } }
        [DisplayName("开仓日期")]
        public string OpenDate { get { return _ipdf.OpenDate; } }
        [DisplayName("开仓价")]
        public double OpenPrice { get { return _ipdf.OpenPrice; } }
        [DisplayName("逐日盯市持仓盈亏")]
        public double PositionProfitByDate { get { return _ipdf.PositionProfitByDate; } }
        [DisplayName("逐笔对冲持仓盈亏")]
        public double PositionProfitByTrade { get { return _ipdf.PositionProfitByTrade; } }
        [DisplayName("结算编号")]
        public int SettlementID { get { return _ipdf.SettlementID; } }
        [DisplayName("结算价")]
        public double SettlementPrice { get { return _ipdf.SettlementPrice; } }
        [DisplayName("成交编号")]
        public string TradeID { get { return _ipdf.TradeID; } }
        [DisplayName("成交类型")]
        public TThostFtdcTradeTypeType TradeType { get { return _ipdf.TradeType; } }
        [DisplayName("交易日")]
        public string TradingDay { get { return _ipdf.TradingDay; } }
        [DisplayName("数量")]
        public int Volume { get { return _ipdf.Volume; } }
        [DisplayName("CThostFtdcInvestorPositionDetailField实例")]
        public CThostFtdcInvestorPositionDetailField InvestorPositionDetailFieldInstance
        {
            get { return _ipdf; }
            set { _ipdf = value; Notify(""); }
        }

        #endregion ==========属性==========

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ipdf"></param>
        public DetailPosition(CThostFtdcInvestorPositionDetailField ipdf)
        {
            this._ipdf = ipdf;
            this.Notify("");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        /// <summary>
        /// DetailPosition相等性比较：OpenDate 和 TradeID 都相同
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DetailPosition other)
        {
            return this.OpenDate == other.OpenDate && this.TradeID == other.TradeID;
        }

    }
}
