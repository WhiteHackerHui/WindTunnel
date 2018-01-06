using System;
using System.ComponentModel;
using WT_Core;

namespace WT_CTP
{
    public class Trade: INotifyPropertyChanged, IEquatable<Trade>
    {
        #region ==========内部变量==========

        private CThostFtdcTradeField _tf;

        #endregion ==========内部变量==========

        #region ==========属性==========

        [DisplayName("经纪商代码")]
        public string BrokerID { get { return _tf.BrokerID; } }
        [DisplayName("经纪公司报单编号")]
        public int BrokerOrderSeq { get { return _tf.BrokerOrderSeq; } }
        [DisplayName("业务单元")]
        public string BusinessUnit { get { return _tf.BusinessUnit; } }
        [DisplayName("结算会员编号")]
        public string ClearingPartID { get { return _tf.ClearingPartID; } }
        [DisplayName("客户代码")]
        public string ClientID { get { return _tf.ClientID; } }
        [DisplayName("买卖方向")]
        public Direction Direction { get { return ConvertFunctions.TThostFtdcDirectionType_To_Direction(_tf.Direction); } }
        [DisplayName("交易所代码")]
        public string ExchangeID { get { return _tf.ExchangeID; } }
        [DisplayName("合约在交易所的代码")]
        public string ExchangeInstID { get { return _tf.ExchangeInstID; } }
        [DisplayName("投机套保标志")]
        public HedgeFlag HedgeFlag { get { return ConvertFunctions.TThostFtdcHedgeFlagType_To_HedgeFlag(_tf.HedgeFlag); } }
        [DisplayName("合约代码")]
        public string InstrumentID { get { return _tf.InstrumentID; } }
        [DisplayName("投资者代码")]
        public string InvestorID { get { return _tf.InvestorID; } }
        [DisplayName("开平标志")]
        public Offset OffsetFlag { get { return ConvertFunctions.TThostFtdcOffsetFlagType_To_Offset(_tf.OffsetFlag); } }
        [DisplayName("本地报单编号")]
        public string OrderLocalID { get { return _tf.OrderLocalID; } }
        [DisplayName("报单引用")]
        public string OrderRef { get { return _tf.OrderRef; } }
        [DisplayName("报单编号")]
        public string OrderSysID { get { return _tf.OrderSysID; } }
        [DisplayName("会员代码")]
        public string ParticipantID { get { return _tf.ParticipantID; } }
        [DisplayName("价格")]
        public double Price { get { return _tf.Price; } }
        [DisplayName("成交价来源")]
        public TThostFtdcPriceSourceType PriceSource { get { return _tf.PriceSource; } }
        [DisplayName("序号")]
        public int SequenceNo { get { return _tf.SequenceNo; } }
        [DisplayName("结算编号")]
        public int SettlementID { get { return _tf.SettlementID; } }
        [DisplayName("成交时期")]
        public string TradeDate { get { return _tf.TradeDate; } }
        [DisplayName("成交编号")]
        public string TradeID { get { return _tf.TradeID; } }
        [DisplayName("交易所交易员代码")]
        public string TraderID { get { return _tf.TraderID; } }
        [DisplayName("成交时间")]
        public string TradeTime { get { return _tf.TradeTime; } }
        [DisplayName("成交类型")]
        public TThostFtdcTradeTypeType TradeType { get { return _tf.TradeType; } }
        [DisplayName("交易日")]
        public string TradingDay { get { return _tf.TradingDay; } }
        [DisplayName("交易角色")]
        public TThostFtdcTradingRoleType TradingRole { get { return _tf.TradingRole; } }
        [DisplayName("用户代码")]
        public string UserID { get { return _tf.UserID; } }
        [DisplayName("数量")]
        public int Volume { get { return _tf.Volume; } }
        [DisplayName("CThostFtdcTradeField实例")]
        public CThostFtdcTradeField CThostFtdcTradeFieldInstance
        {
            get { return _tf; }
            set { _tf = value; Notify(""); }
        }

        #endregion ==========属性==========

        /// <summary>
        /// 构造函数
        /// </summary>
        public Trade(CThostFtdcTradeField tf)
        {
            this._tf = tf;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        /// <summary>
        /// Trade相等性判断：TradeID
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Trade other)
        {
            if (other == null) return false;
            return (this.TradeID == other.TradeID);
        }

    }
}
