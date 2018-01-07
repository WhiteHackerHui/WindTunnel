using System;
using System.ComponentModel;
using WT_Core;

namespace WT_CTP
{
    /// <summary>
    /// 封装的报单信息
    /// </summary>
    public class OrderField : INotifyPropertyChanged, IEquatable<OrderField>
    {
        #region =========变量=========

        private CThostFtdcOrderField _of;

        #endregion =========变量=========

        #region ==========属性==========

        [DisplayName("经纪公司代码")]
        public string BrokerID { set { _of.BrokerID = value; } get { return _of.BrokerID; } }
        [DisplayName("投资者代码")]
        public string InvestorID { set { _of.InvestorID = value; } get { return _of.InvestorID; } }
        [DisplayName("合约代码")]
        public string InstrumentID { set { _of.InstrumentID = value; } get { return _of.InstrumentID; } }
        [DisplayName("报单引用")]
        public string OrderRef { set { _of.OrderRef = value; } get { return _of.OrderRef; } }
        [DisplayName("用户代码")]
        public string UserID { set { _of.UserID = value; } get { return _of.UserID; } }
        [DisplayName("激活时间")]
        public string ActiveTime { get { return _of.ActiveTime; } }
        [DisplayName("最后修改交易所交易员代码")]
        public string ActiveTraderID { get { return _of.ActiveTraderID; } }
        [DisplayName("操作用户代码")]
        public string ActiveUserID {get { return _of.ActiveUserID; } }
        [DisplayName("经纪公司报单编号")]
        public int BrokerOrderSeq {get { return _of.BrokerOrderSeq; } }
        [DisplayName("业务单元")]
        public string BusinessUnit {get { return _of.BusinessUnit; } }
        [DisplayName("撤销时间")]
        public string CancelTime {get { return _of.CancelTime; } }
        [DisplayName("结算会员编号")]
        public string ClearingPartID { get { return _of.ClearingPartID; } }
        [DisplayName("客户代码")]
        public string ClientID { get { return _of.ClientID; } }
        [DisplayName("组合投机套保标志")]
        public HedgeFlag CombHedgeFlag
        {
            set
            {
                switch (value)
                {
                    case HedgeFlag.Speculation:
                        _of.CombHedgeFlag = "1";
                        break;
                    case HedgeFlag.Arbitrage:
                        _of.CombHedgeFlag = "2";
                        break;
                    case HedgeFlag.Hedge:
                        _of.CombHedgeFlag = "3";
                        break;
                    case HedgeFlag.MarketMaker:
                        _of.CombHedgeFlag = "5";
                        break;
                    default:
                        break;
                }
            }
            get
            {
                string hedgeFlag = _of.CombHedgeFlag;
                HedgeFlag hf = HedgeFlag.Speculation;
                if (hedgeFlag == "1")
                {
                    hf = HedgeFlag.Speculation;
                }
                else if (hedgeFlag == "2")
                {
                    hf = HedgeFlag.Arbitrage;
                }
                else if (hedgeFlag == "3")
                {
                    hf = HedgeFlag.Hedge;
                }
                else if (hedgeFlag == "5")
                {
                    hf = HedgeFlag.MarketMaker;
                }
                return hf;
            }
        }
        [DisplayName("组合开平标志")]
        public Offset CombOffsetFlag
        {
            get{return ConvertFunctions.CombOffsetFlag_To_Offset(_of.CombOffsetFlag);}
            set
            {
                switch (value)
                {
                    case Offset.Open:
                        this._of.CombOffsetFlag = "0";
                        break;
                    case Offset.Close:
                        this._of.CombOffsetFlag = "1";
                        break;
                    case Offset.ForceClose:
                        this._of.CombOffsetFlag = "2";
                        break;
                    case Offset.CloseToday:
                        this._of.CombOffsetFlag = "3";
                        break;
                    case Offset.CloseYesterday:
                        this._of.CombOffsetFlag = "4";
                        break;
                    case Offset.ForceOff:
                        this._of.CombOffsetFlag = "5";
                        break;
                    case Offset.LocalForceClose:
                        this._of.CombOffsetFlag = "6";
                        break;
                    default:
                        break;
                }
            }
        }
        [DisplayName("触发条件")]
        public ContingentCondition ContingentCondition {
            set { this._of.ContingentCondition = ConvertFunctions.ContingentCondition_To_TThostFtdcContingentConditionType(value); }
            get { return ConvertFunctions.TThostFtdcContingentConditionType_To_ContingentCondition(_of.ContingentCondition); }
        }
        [DisplayName("买卖方向")]
        public Direction Direction {
            set { this._of.Direction = ConvertFunctions.Direction_To_TThostFtdcDirectionType(value); }
            get { return ConvertFunctions.TThostFtdcDirectionType_To_Direction(_of.Direction);}
        }
        [DisplayName("交易所代码")]
        public string ExchangeID { set { _of.ExchangeID = value; } get { return _of.ExchangeID; } }
        [DisplayName("合约在交易所的代码")]
        public string ExchangeInstID { get { return _of.ExchangeInstID; } }
        [DisplayName("强平原因")]
        public ForceCloseReason ForceCloseReason {
            set { this._of.ForceCloseReason = ConvertFunctions.ForceCloseReason_To_TThostFtdcForceCloseReasonType(value); }
            get { return ConvertFunctions.TThostFtdcForceCloseReasonType_To_ForceCloseReason(_of.ForceCloseReason);}
        }
        [DisplayName("前置编号")]
        public int FrontID { set { _of.FrontID = value; } get { return _of.FrontID; } }
        [DisplayName("GTD日期")]
        public string GTDDate { set { _of.GTDDate = value; } get { return _of.GTDDate; } }
        [DisplayName("报单日期")]
        public string InsertDate { get { return _of.InsertDate; } }
        [DisplayName("委托时间")]
        public string InsertTime { get { return _of.InsertTime; } }
        [DisplayName("安装编号")]
        public int InstallID { get { return _of.InstallID; } }
        [DisplayName("自动挂起标志")]
        public int IsAutoSuspend { get { return _of.IsAutoSuspend; } }
        [DisplayName("价格")]
        public double LimitPrice { set { _of.LimitPrice = value; } get { return _of.LimitPrice; } }
        [DisplayName("最小成交量")]
        public int MinVolume { set { _of.MinVolume = value; } get { return _of.MinVolume; } }
        [DisplayName("报单提示序号")]
        public int NotifySequence { get { return _of.NotifySequence; } }
        [DisplayName("本地报单编号")]
        public string OrderLocalID { get { return _of.OrderLocalID; } }
        [DisplayName("报单价格条件")]
        public OrderPriceType OrderPriceType {
            set { this._of.OrderPriceType = ConvertFunctions.OrderPriceType_To_TThostFtdcOrderPriceTypeType(value); }
            get { return ConvertFunctions.TThostFtdcOrderPriceTypeType_To_OrderPriceType(_of.OrderPriceType); }
        }
        [DisplayName("报单来源类型")]
        public OrderSourceType OrderSource {
            get {
                return ConvertFunctions.TThostFtdcOrderSourceType_To_OrderSourceType(_of.OrderSource);
            }
        }
        [DisplayName("报单状态类型")]
        public OrderStatus OrderStatus { get { return ConvertFunctions.TThostFtdcOrderStatusType_To_OrderStatus(_of.OrderStatus); }}
        [DisplayName("报单提交状态类型")]
        public OrderSubmitStatus OrderSubmitStatus {
            get {
                return ConvertFunctions.TThostFtdcOrderSubmitStatusType_To_OrderSubmitStatus(_of.OrderSubmitStatus);
            }
        }
        [DisplayName("报单编号")]
        public string OrderSysID { set { _of.OrderSysID = value; } get { return _of.OrderSysID; } }
        [DisplayName("报单类型类型")]
        public OrderType OrderType {
            get {
                return ConvertFunctions.TThostFtdcOrderTypeType_To_OrderType(_of.OrderType);
            }
        }
        [DisplayName("会员代码")]
        public string ParticipantID { get { return _of.ParticipantID; } }
        [DisplayName("相关报单")]
        public string RelativeOrderSysID { get { return _of.RelativeOrderSysID; } }
        [DisplayName("请求编号")]
        public int RequestID { get { return _of.RequestID; } }
        [DisplayName("序号")]
        public int SequenceNo {get { return _of.SequenceNo; } }
        [DisplayName("会话编号")]
        public int SessionID { set { _of.SessionID = value; } get { return _of.SessionID; } }
        [DisplayName("结算编号")]
        public int SettlementID { get { return _of.SettlementID; } }
        [DisplayName("状态信息")]
        public string StatusMsg { set { _of.StatusMsg = value; } get { return _of.StatusMsg; } }
        [DisplayName("止损价")]
        public double StopPrice { set { _of.StopPrice = value; } get { return _of.StopPrice; } }
        [DisplayName("挂起时间")]
        public string SuspendTime { get { return _of.SuspendTime; } }
        [DisplayName("有效期类型类型")]
        public TimeCondition TimeCondition {
            set { this._of.TimeCondition = ConvertFunctions.TimeCondition_To_TThostFtdcTimeConditionType(value); }
            get { return ConvertFunctions.TThostFtdcTimeConditionType_To_TimeCondition(_of.TimeCondition); }
        }
        [DisplayName("交易所交易员代码")]
        public string TraderID { get { return _of.TraderID; } }
        [DisplayName("交易日")]
        public string TradingDay { get { return _of.TradingDay; } }
        [DisplayName("最后修改时间")]
        public string UpdateTime { get { return _of.UpdateTime; } }
        [DisplayName("用户强平标志")]
        public int UserForceClose { get { return _of.UserForceClose; } }
        [DisplayName("用户端产品信息")]
        public string UserProductInfo { get { return _of.UserProductInfo; } }
        [DisplayName("成交量类型类型")]
        public VolumeCondition VolumeCondition {
            set { this._of.VolumeCondition = ConvertFunctions.VolumeCondition_To_TThostFtdcVolumeConditionType(value); }
            get { return ConvertFunctions.TThostFtdcVolumeConditionType_To_VolumeCondition(_of.VolumeCondition); }
        }
        [DisplayName("剩余数量")]
        public int VolumeTotal { get { return _of.VolumeTotal; } }
        [DisplayName("数量")]
        public int VolumeTotalOriginal { set { _of.VolumeTotalOriginal = value; } get { return _of.VolumeTotalOriginal; } }
        [DisplayName("今成交数量")]
        public int VolumeTraded { get { return _of.VolumeTraded; } }
        [DisplayName("CThostFtdcOrderField实例")]
        public CThostFtdcOrderField OrderFieldInstance
        {
            get { return _of; }
            set { _of = value; Notify(""); }
        }

        #endregion ==========属性==========


        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderField(CThostFtdcOrderField of)
        {
            OrderFieldInstance = of;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderField()
        {
        }

        /// <summary>
        /// 报单相同性判断：FrontID、SessionID和OrderRef都要相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(OrderField other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return this.FrontID == other.FrontID && this.SessionID == other.SessionID && this.OrderRef == other.OrderRef;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
