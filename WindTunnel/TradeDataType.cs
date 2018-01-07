using System;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Threading;
using HaiFeng;
using System.Collections.Generic;
using System.Linq;

namespace WindTunnel
{
    //命令
    public enum EnumReqCmdType
    {
        QryInstrument,
        QryInvestorPositionDetail,
        QryInvestorPosition,
        QrySettlementInfo,
        QrySettlementInfoConfirm,
        QryTradingAccount,
        SettlementInfoConfirm
    }

    #region InstrumentField
    public class InstrumentField : INotifyPropertyChanged
    {
        //内部变量
        private CThostFtdcInstrumentField _if;
        private string _futureCode;

        //属性
        public string CreateDate { get { return _if.CreateDate; } }
        public int DeliveryMonth { get { return _if.DeliveryMonth; } }
        public int DeliveryYear { get { return _if.DeliveryYear; } }
        public string EndDelivDate { get { return _if.EndDelivDate; } }
        public string ExchangeID { get { return _if.ExchangeID; } }
        public string ExchangeInstID { get { return _if.ExchangeInstID; } }
        public string ExpireDate { get { return _if.ExpireDate; } }
        public TThostFtdcInstLifePhaseType InstLifePhase { get { return _if.InstLifePhase; } }
        public string InstrumentID { get { return _if.InstrumentID; } }
        public string InstrumentName { get { return _if.InstrumentName; } }
        public int IsTrading { get { return _if.IsTrading; } }
        public double LongMarginRatio { get { return _if.LongMarginRatio; } }
        public int MaxLimitOrderVolume { get { return _if.MaxLimitOrderVolume; } }
        public int MaxMarketOrderVolume { get { return _if.MaxMarketOrderVolume; } }
        public int MinLimitOrderVolume { get { return _if.MinLimitOrderVolume; } }
        public int MinMarketOrderVolume { get { return _if.MinMarketOrderVolume; } }
        public string OpenDate { get { return _if.OpenDate; } }
        public TThostFtdcPositionDateTypeType PositionDateType { get { return _if.PositionDateType; } }
        public TThostFtdcPositionTypeType PositionType { get { return _if.PositionType; } }
        public double PriceTick { get { return _if.PriceTick; } }
        public TThostFtdcProductClassType ProductClass { get { return _if.ProductClass; } }
        public string ProductID { get { return _if.ProductID; } }
        public double ShortMarginRatio { get { return _if.ShortMarginRatio; } }
        public string StartDelivDate { get { return _if.StartDelivDate; } }
        public int VolumeMultiple { get { return _if.VolumeMultiple; } }
        public string FutureCode { get { return _futureCode; } }
        public CThostFtdcInstrumentField CThostFtdcInstrumentFieldInstance
        {
            get { return _if; }
            set { _if = value;Notify("");}
        }

        //构造函数
        public InstrumentField(CThostFtdcInstrumentField instrumentField)
        {
            this._if = instrumentField;
            Regex r = new Regex(@"\d");
            int index = r.Match(_if.InstrumentID).Index;
            _futureCode = _if.InstrumentID.Substring(0, index);
            Notify("");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

    }
    #endregion

    #region OrderField
    public class OrderField : INotifyPropertyChanged, IEquatable<OrderField>
    {
        //变量
        private CThostFtdcOrderField _of;

        //属性
        public string BrokerID { set { _of.BrokerID = value; } get { return _of.BrokerID; } }
        public string InvestorID { set { _of.InvestorID = value; } get { return _of.InvestorID; } }
        public string InstrumentID { set { _of.InstrumentID = value; } get { return _of.InstrumentID; } }
        public string OrderRef { set { _of.OrderRef = value; } get { return _of.OrderRef; } }
        public string UserID { set { _of.UserID = value; } get { return _of.UserID; } }
        public string ActiveTime { set { _of.ActiveTime = value; } get { return _of.ActiveTime; } }
        public string ActiveTraderID { set { _of.ActiveTraderID = value; } get { return _of.ActiveTraderID; } }
        public string ActiveUserID { set { _of.ActiveUserID = value; } get { return _of.ActiveUserID; } }
        public int BrokerOrderSeq { set { _of.BrokerOrderSeq = value; } get { return _of.BrokerOrderSeq; } }
        public string BusinessUnit { set { _of.BusinessUnit = value; } get { return _of.BusinessUnit; } }
        public string CancelTime { set { _of.CancelTime = value; } get { return _of.CancelTime; } }
        public string ClearingPartID { set { _of.ClearingPartID = value; } get { return _of.ClearingPartID; } }
        public string ClientID { set { _of.ClientID = value; } get { return _of.ClientID; } }
        public string CombHedgeFlag { set { _of.CombHedgeFlag = value; } get { return _of.CombHedgeFlag; } }
        public string StrCombHedgeFlag
        {
            get {
                string hedgeFlag = _of.CombHedgeFlag;
                string strValue = null;
                if (hedgeFlag == "1")
                {
                    strValue = "投机";
                }
                else if (hedgeFlag == "2")
                {
                    strValue = "套利";
                }
                else if (hedgeFlag == "3")
                {
                    strValue = "套保";
                }
                else if (hedgeFlag == "5")
                {
                    strValue = "做市商";
                }
                return strValue;
            }
        }

        public string CombOffsetFlag { set { _of.CombOffsetFlag = value; } get { return _of.CombOffsetFlag; } }
        public string StrCombOffsetFlag
        {
            get
            {
                string offsetFlag = _of.CombOffsetFlag;
                string strValue = null;
                if (offsetFlag=="0")
                {
                    strValue = "开仓";
                }
                else if(offsetFlag == "1")
                {
                    strValue = "平仓";
                }
                else if (offsetFlag == "2")
                {
                    strValue = "强平";
                }
                else if (offsetFlag == "3")
                {
                    strValue = "平今";
                }
                else if (offsetFlag == "4")
                {
                    strValue = "平昨";
                }
                else if (offsetFlag == "5")
                {
                    strValue = "强减";
                }
                else if (offsetFlag == "6")
                {
                    strValue = "本地强平";
                }
                return strValue;
            }
            set
            {
                if (value == "开仓" )
                {
                    _of.CombOffsetFlag = "0";
                }
                else if (value == "平仓")
                {
                    _of.CombOffsetFlag = "1";
                }
                else if (value == "强平")
                {
                    _of.CombOffsetFlag = "2";
                }
                else if (value == "平今")
                {
                    _of.CombOffsetFlag = "3";
                }
                else if (value == "平昨")
                {
                   _of.CombOffsetFlag = "4";
                }
                else if (value == "强减")
                {
                   _of.CombOffsetFlag = "5";
                }
                else if (value == "本地强平")
                {
                   _of.CombOffsetFlag = "6";
                }
            }
        }
        public TThostFtdcContingentConditionType ContingentCondition { set { _of.ContingentCondition = value; } get { return _of.ContingentCondition; } }
        public TThostFtdcDirectionType Direction { set { _of.Direction = value; } get { return _of.Direction; } }
        public string StrDirection
        {
            get
            {
                switch (_of.Direction)
                {
                    case TThostFtdcDirectionType.THOST_FTDC_D_Buy:
                        return "Buy";
                    case TThostFtdcDirectionType.THOST_FTDC_D_Sell:
                        return "Sell";
                    default:
                        return null;
                }
            }
            set
            {
                if (value == "Buy")
                {
                    this.Direction = TThostFtdcDirectionType.THOST_FTDC_D_Buy;
                }
                else if (value == "Sell")
                {
                    this.Direction = TThostFtdcDirectionType.THOST_FTDC_D_Sell;
                }
            }
        }
        public string ExchangeID { set { _of.ExchangeID = value; } get { return _of.ExchangeID; } }
        public string ExchangeInstID { set { _of.ExchangeInstID = value; } get { return _of.ExchangeInstID; } }
        public TThostFtdcForceCloseReasonType ForceCloseReason { set { _of.ForceCloseReason = value; } get { return _of.ForceCloseReason; } }
        public int FrontID { set { _of.FrontID = value; } get { return _of.FrontID; } }
        public string GTDDate { set { _of.GTDDate = value; } get { return _of.GTDDate; } }
        public string InsertDate { set { _of.InsertDate = value; } get { return _of.InsertDate; } }
        public string InsertTime { set { _of.InsertTime = value; } get { return _of.InsertTime; } }
        public int InstallID { set { _of.InstallID = value; } get { return _of.InstallID; } }
        public int IsAutoSuspend { set { _of.IsAutoSuspend = value; } get { return _of.IsAutoSuspend; } }
        public double LimitPrice { set { _of.LimitPrice = value; } get { return _of.LimitPrice; } }
        public int MinVolume { set { _of.MinVolume = value; } get { return _of.MinVolume; } }
        public int NotifySequence { set { _of.NotifySequence = value; } get { return _of.NotifySequence; } }
        public string OrderLocalID { set { _of.OrderLocalID = value; } get { return _of.OrderLocalID; } }
        public TThostFtdcOrderPriceTypeType OrderPriceType { set { _of.OrderPriceType = value; } get { return _of.OrderPriceType; } }
        public string StrOrderPriceType
        {
            set
            {
                if (value == "Limit")
                {
                    this._of.OrderPriceType = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice;
                }
                else if (value == "Market")
                {
                    this._of.OrderPriceType = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AnyPrice;
                }
            }
            get
            {
                string strValue = string.Empty;
                switch (this._of.OrderPriceType)
                {
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AnyPrice:
                        strValue = "Market";
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice:
                        strValue = "Limit";
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BestPrice:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPrice:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPricePlusOneTicks:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPricePlusTwoTicks:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPricePlusThreeTicks:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1PlusOneTicks:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1PlusTwoTicks:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1PlusThreeTicks:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1PlusOneTicks:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1PlusTwoTicks:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1PlusThreeTicks:
                        break;
                    case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_FiveLevelPrice:
                        break;
                    default:
                        break;
                }
                return strValue;
            }

        }
        public TThostFtdcOrderSourceType OrderSource { set { _of.OrderSource = value; } get { return _of.OrderSource; } }
        public TThostFtdcOrderStatusType OrderStatus { set { _of.OrderStatus = value; } get { return _of.OrderStatus; } }
        public string StrOrderStatus
        {
            get
            {
                switch (_of.OrderStatus)
                {
                    case TThostFtdcOrderStatusType.THOST_FTDC_OST_AllTraded:
                        return "全部成交";
                    case TThostFtdcOrderStatusType.THOST_FTDC_OST_PartTradedQueueing:
                        return "部分成交还在队列中";
                    case TThostFtdcOrderStatusType.THOST_FTDC_OST_PartTradedNotQueueing:
                        return "部分成交不在队列中";
                    case TThostFtdcOrderStatusType.THOST_FTDC_OST_NoTradeQueueing:
                        return "未成交还在队列中";
                    case TThostFtdcOrderStatusType.THOST_FTDC_OST_NoTradeNotQueueing:
                        return "未成交不在队列中";
                    case TThostFtdcOrderStatusType.THOST_FTDC_OST_Canceled:
                        return "撤单";
                    case TThostFtdcOrderStatusType.THOST_FTDC_OST_Unknown:
                        return "未知";
                    case TThostFtdcOrderStatusType.THOST_FTDC_OST_NotTouched:
                        return "尚未触发";
                    case TThostFtdcOrderStatusType.THOST_FTDC_OST_Touched:
                        return "已触发";
                    default:
                        return null;
                }
            }
        }
        public TThostFtdcOrderSubmitStatusType OrderSubmitStatus { set { _of.OrderSubmitStatus = value; } get { return _of.OrderSubmitStatus; } }
        public string StrOrderSubmitStatus
        {
            get
            {
                switch (_of.OrderSubmitStatus)
                {
                    case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_InsertSubmitted:
                        return "已经提交";
                    case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_CancelSubmitted:
                        return "撤单已经提交";
                    case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_ModifySubmitted:
                        return "修改已经提交";
                    case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_Accepted:
                        return "已经接受";
                    case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_InsertRejected:
                        return "报单已经被拒绝";
                    case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_CancelRejected:
                        return "撤单已经被拒绝";
                    case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_ModifyRejected:
                        return "改单已经被拒绝";
                    default:
                        return null;
                }
            }
        }
        public string OrderSysID { set { _of.OrderSysID = value; } get { return _of.OrderSysID; } }
        public TThostFtdcOrderTypeType OrderType { set { _of.OrderType = value; } get { return _of.OrderType; } }
        public string ParticipantID { set { _of.ParticipantID = value; } get { return _of.ParticipantID; } }
        public string RelativeOrderSysID { set { _of.RelativeOrderSysID = value; } get { return _of.RelativeOrderSysID; } }
        public int RequestID { set { _of.RequestID = value; } get { return _of.RequestID; } }
        public int SequenceNo { set { _of.SequenceNo = value; } get { return _of.SequenceNo; } }
        public int SessionID { set { _of.SessionID = value; } get { return _of.SessionID; } }
        public int SettlementID { set { _of.SettlementID = value; } get { return _of.SettlementID; } }
        public string StatusMsg { set { _of.StatusMsg = value; } get { return _of.StatusMsg; } }
        public double StopPrice { set { _of.StopPrice = value; } get { return _of.StopPrice; } }
        public string SuspendTime { set { _of.SuspendTime = value; } get { return _of.SuspendTime; } }
        public TThostFtdcTimeConditionType TimeCondition { set { _of.TimeCondition = value; } get { return _of.TimeCondition; } }
        public string TraderID { set { _of.TraderID = value; } get { return _of.TraderID; } }
        public string TradingDay { set { _of.TradingDay = value; } get { return _of.TradingDay; } }
        public string UpdateTime { set { _of.UpdateTime = value; } get { return _of.UpdateTime; } }
        public int UserForceClose { set { _of.UserForceClose = value; } get { return _of.UserForceClose; } }
        public string UserProductInfo { set { _of.UserProductInfo = value; } get { return _of.UserProductInfo; } }
        public TThostFtdcVolumeConditionType VolumeCondition { set { _of.VolumeCondition = value; } get { return _of.VolumeCondition; } }
        public int VolumeTotal { set { _of.VolumeTotal = value; } get { return _of.VolumeTotal; } }
        public int VolumeTotalOriginal { set { _of.VolumeTotalOriginal = value; } get { return _of.VolumeTotalOriginal; } }
        public int VolumeTraded { set { _of.VolumeTraded = value; } get { return _of.VolumeTraded; } }
        public CThostFtdcOrderField OrderFieldInstance
        {
            get { return _of; }
            set { _of = value; Notify(""); }
        }

        //构造函数
        public OrderField(CThostFtdcOrderField of)
        {
            OrderFieldInstance = of;
        }
        public OrderField()
        { 
        }
        public event PropertyChangedEventHandler PropertyChanged;

        //定义更新属性：propName的函数过程
        public void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

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
    }
    #endregion

    #region 条件单
    //条件单类型
    public enum ConditionOrderType
    {
        价格单,
        时间单,
        持仓单
    }
    public enum ConditionOrderExecutionStatus
    {
        未执行,
        执行中,
        执行完毕
    }
    //条件单类型
    public class ConditionOrderField : OrderField
    {
        private bool _isRunning = false;
        private ConditionOrderType _conditionOrderType;
        private string _expressionSymbol;//大、小于号等等
        private double _targetPrice;//指定的条件价格
        private string _conditionText;//条件文本
        private bool _orderSent = false;
        private ConditionOrderExecutionStatus _coes = ConditionOrderExecutionStatus.未执行;
        private static readonly object _lock = new object();

        //价格满足条件委托
        private delegate bool DeleIsPriceConditionSatisfied(DepthMarketData dmd);
        private DeleIsPriceConditionSatisfied _isPriceConditionSatisfied;
        //时间满足条件委托
        private delegate bool DeleIsTimeConditionSatisfied(DateTime time);
        private DeleIsTimeConditionSatisfied IsTimeConditionSatisfied;
        //持仓量满足条件委托
        private delegate bool DeleIsOpenInterestConditionSatisfied(DepthMarketData dmd);
        private DeleIsOpenInterestConditionSatisfied IsOpenInterestConditionSatisfied;

        


        //价格条件单运行
        public void Run_PriceConditionOrder(DepthMarketData dmd)
        {
            lock (_lock)
            {
                if (!this._orderSent && dmd.InstrumentID == this.InstrumentID && this._isPriceConditionSatisfied != null && this._isRunning)
                {
                    if (this._isPriceConditionSatisfied(dmd))
                    {
                        this.TradeApi.SendOrder(this);
                        this._orderSent = true;
                        this.ConditionStatus = ConditionOrderExecutionStatus.执行完毕;
                    }
                }
            }
        }

        //追加属性
        public bool IsRunning {
            get { return this._isRunning; }
            set
            {
                lock (_lock)
                {
                    this._isRunning = value;
                    if (this._isRunning)
                    {
                        this.ConditionStatus = ConditionOrderExecutionStatus.执行中;
                        switch (this._conditionOrderType)
                        {
                            case ConditionOrderType.价格单:
                                #region 价格条件单
                                this._conditionText = string.Format("市价 {0} {1}", this._expressionSymbol, this._targetPrice);
                                if (this._expressionSymbol == ">")
                                {
                                    this._isPriceConditionSatisfied = (dmd) => { return dmd.LastPrice > this.TargetPrice; };
                                }
                                else if (this._expressionSymbol == ">=")
                                {
                                    this._isPriceConditionSatisfied = (dmd) => { return dmd.LastPrice >= this.TargetPrice; };
                                }
                                else if (this._expressionSymbol == "=")
                                {
                                    this._isPriceConditionSatisfied = (dmd) => { return dmd.LastPrice == this.TargetPrice; };
                                }
                                else if (this._expressionSymbol == "<=")
                                {
                                    this._isPriceConditionSatisfied = (dmd) => { return dmd.LastPrice <= this.TargetPrice; };
                                }
                                else if (this._expressionSymbol == "<")
                                {
                                    this._isPriceConditionSatisfied = (dmd) => { return dmd.LastPrice < this.TargetPrice; };
                                }
                                #endregion 价格条件单
                                break;
                            case ConditionOrderType.时间单:
                                break;
                            case ConditionOrderType.持仓单:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        this.ConditionStatus = ConditionOrderExecutionStatus.未执行;
                        this._orderSent = false;
                    }
                    this.Notify("IsRunning");
                }
            }
        }

        public CTradeApi TradeApi { get; set; }
        public ConditionOrderExecutionStatus ConditionStatus { get { return this._coes; } private set { this._coes = value; Notify("ConditionStatus"); } }
        public string ConditionText { get { return this._conditionText; } }
        public ConditionOrderType ConOrderType { get { return this._conditionOrderType; } set { this._conditionOrderType = value; } }
        public string ExpressionSymbol { get { return _expressionSymbol; } set {this._expressionSymbol = value;}}
        public double TargetPrice {
            get { return this._targetPrice; }
            set { this._targetPrice = value; } }

        //构造函数
        public ConditionOrderField(CThostFtdcOrderField of):base(of)
        {
        }
        public ConditionOrderField():base()
        {
        }
        public ConditionOrderField(OrderField orderfield)
        {
            this.OrderFieldInstance = orderfield.OrderFieldInstance;
        }

    }
    #endregion

    #region Position
    public class Position: INotifyPropertyChanged,IEquatable<Position>, IEqualityComparer<Position>
    {
        public string InstrumentID { get; set; }
        public string BrokerID { get; set; }
        public string InvestorID { get; set; }
        public TThostFtdcPosiDirectionType PosiDirection { get; set; }
        public string SrtPosiDirection { get
            {
                switch (this.PosiDirection)
                {
                    case TThostFtdcPosiDirectionType.THOST_FTDC_PD_Net:
                        return "Net";
                    case TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long:
                        return "Buy";
                    case TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short:
                        return "Sell";
                    default:
                        return null;
                }
            } }
        public TThostFtdcHedgeFlagType HedgeFlag { get; set; }
        public string StrHedgeFlag
        {
            get
            {
                switch (this.HedgeFlag)
                {
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation:
                        return "投机";
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Arbitrage:
                        return "套利";
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Hedge:
                        return "套保";
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_MarketMaker:
                        return "做市商";
                    default:
                        return null;
                }
            }
        }
        public TThostFtdcPositionDateType PositionDate { get; set; }
        public int YdPosition { get; set; }
        public int TotalPosition { get { return this.YdPosition + this.TdPosition; } }
        public int LongFrozen { get; set; }
        public int ShortFrozen { get; set; }
        public double LongFrozenAmount { get; set; }
        public double ShortFrozenAmount { get; set; }
        public int OpenVolume { get; set; }
        public int CloseVolume { get; set; }
        public double OpenAmount { get; set; }
        public double CloseAmount { get; set; }
        public double PositionCost { get; set; }
        public double PreMargin { get; set; }
        public double UseMargin { get; set; }
        public double FrozenMargin { get; set; }
        public double FrozenCash { get; set; }
        public double FrozenCommission { get; set; }
        public double CashIn { get; set; }
        public double Commission { get; set; }
        public double CloseProfit { get; set; }
        public double PositionProfit { get; set; }
        public double OpenProfit { get; set; }
        public double PreSettlementPrice { get; set; }
        public double SettlementPrice { get; set; }
        public string TradingDay { get; set; }
        public int SettlementID { get; set; }
        public double OpenCost { get; set; }
        public double ExchangeMargin { get; set; }
        public int CombPosition { get; set; }
        public int CombLongFrozen { get; set; }
        public int CombShortFrozen { get; set; }
        public double CloseProfitByDate { get; set; }
        public double CloseProfitByTrade { get; set; }
        public int TdPosition { get; set; }
        public double MarginRateByMoney { get; set; }
        public double MarginRateByVolume { get; set; }
        public int StrikeFrozen { get; set; }
        public double StrikeFrozenAmount { get; set; }
        public int AbandonFrozen { get; set; }
        public double AvgOpenPrice { get; set; }
        public double LastPrice { get; set; }

        //构造函数
        public Position(string InstrumentID, TThostFtdcPosiDirectionType Direction, TThostFtdcHedgeFlagType Hedge)
        {
            this.InstrumentID = InstrumentID;
            this.PosiDirection = Direction;
            this.HedgeFlag = Hedge;
        }
        public Position()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        //定义更新属性：propName的函数过程
        public void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public bool Equals(Position other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return this.InstrumentID == other.InstrumentID && this.PosiDirection == other.PosiDirection && this.HedgeFlag == other.HedgeFlag;
            }
        }

        bool IEqualityComparer<Position>.Equals(Position x, Position y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<Position>.GetHashCode(Position obj)
        {
            return obj.InstrumentID.GetHashCode() ^ obj.PosiDirection.ToString().GetHashCode() ^ obj.HedgeFlag.ToString().GetHashCode();
        }
    }
    #endregion

    #region DetailPositionField
    public class DetailPositionField: INotifyPropertyChanged, IEquatable<DetailPositionField>
    {
        private CThostFtdcInvestorPositionDetailField _ipdf;

        public string BrokerID { get { return _ipdf.BrokerID; } }
        public double CloseAmount { get { return _ipdf.CloseAmount; } }
        public double CloseProfitByDate { get { return _ipdf.CloseProfitByDate; } }
        public double CloseProfitByTrade { get { return _ipdf.CloseProfitByTrade; } }
        public int CloseVolume { get { return _ipdf.CloseVolume; } }
        public string CombInstrumentID { get { return _ipdf.CombInstrumentID; } }
        public TThostFtdcDirectionType Direction { get { return _ipdf.Direction; } }
        public string StrDirection
        {
            get
            {
                switch (_ipdf.Direction)
                {
                    case TThostFtdcDirectionType.THOST_FTDC_D_Buy:
                        return "Buy";
                    case TThostFtdcDirectionType.THOST_FTDC_D_Sell:
                        return "Sell";
                    default:
                        return null;
                }
            }
        }
        public string ExchangeID { get { return _ipdf.ExchangeID; } }
        public double ExchMargin { get { return _ipdf.ExchMargin; } }
        public TThostFtdcHedgeFlagType HedgeFlag { get { return _ipdf.HedgeFlag; } }
        public string StrHedgeFlag
        {
            get
            {
                switch (_ipdf.HedgeFlag)
                {
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation:
                        return "投机";
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Arbitrage:
                        return "套利";
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Hedge:
                        return "套保";
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_MarketMaker:
                        return "做市商";
                    default:
                        return null;
                }
            }
        }
        public string InstrumentID { get { return _ipdf.InstrumentID; } }
        public string InvestorID { get { return _ipdf.InvestorID; } }
        public double LastSettlementPrice { get { return _ipdf.LastSettlementPrice; } }
        public double Margin { get { return _ipdf.Margin; } }
        public double MarginRateByMoney { get { return _ipdf.MarginRateByMoney; } }
        public double MarginRateByVolume { get { return _ipdf.MarginRateByVolume; } }
        public string OpenDate { get { return _ipdf.OpenDate; } }
        public double OpenPrice { get { return _ipdf.OpenPrice; } }
        public double PositionProfitByDate { get { return _ipdf.PositionProfitByDate; } }
        public double PositionProfitByTrade { get { return _ipdf.PositionProfitByTrade; } }
        public int SettlementID { get { return _ipdf.SettlementID; } }
        public double SettlementPrice { get { return _ipdf.SettlementPrice; } }
        public string TradeID { get { return _ipdf.TradeID; } }
        public TThostFtdcTradeTypeType TradeType { get { return _ipdf.TradeType; } }
        public string TradingDay { get { return _ipdf.TradingDay; } }
        public int Volume { get { return _ipdf.Volume; } }
        public CThostFtdcInvestorPositionDetailField InvestorPositionDetailFieldInstance
        {
            get { return _ipdf; }
            set { _ipdf = value; Notify(""); }
        }

        public DetailPositionField(CThostFtdcInvestorPositionDetailField ipdf)
        {
            this._ipdf = ipdf;
            this.Notify("");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        public bool Equals(DetailPositionField other)
        {
            return this.OpenDate == other.OpenDate && this.TradeID == other.OpenDate;
        }
    }
    #endregion

    #region TradeField
    public class TradeField: INotifyPropertyChanged, IEquatable<TradeField>
    {
        private CThostFtdcTradeField _tf;

        public string BrokerID { get { return _tf.BrokerID; }}
        public int BrokerOrderSeq { get { return _tf.BrokerOrderSeq; }}
        public string BusinessUnit { get { return _tf.BusinessUnit; }}
        public string ClearingPartID { get { return _tf.ClearingPartID; }}
        public string ClientID { get { return _tf.ClientID; } }
        public string StrDirection { get { return _tf.Direction == TThostFtdcDirectionType.THOST_FTDC_D_Buy? "Buy":"Sell"; }}
        public TThostFtdcDirectionType Direction { get { return _tf.Direction; } }
        public string ExchangeID { get { return _tf.ExchangeID; }}
        public string ExchangeInstID { get { return _tf.ExchangeInstID; } }
        public string StrHedgeFlag
        { get {
                switch (_tf.HedgeFlag)
                {
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation:
                        return "投机";
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Arbitrage:
                        return "套利";
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Hedge:
                        return "套保";
                    case TThostFtdcHedgeFlagType.THOST_FTDC_HF_MarketMaker:
                        return "做市商";
                    default:
                        return string.Empty;
                }
            } }
        public TThostFtdcHedgeFlagType HedgeFlag { get { return _tf.HedgeFlag; } }
        public string InstrumentID { get { return _tf.InstrumentID; } }
        public string InvestorID { get { return _tf.InvestorID; } }
        public string StrOffsetFlag { get {
                switch (_tf.OffsetFlag)
                {
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_Open:
                        return "开仓";
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_Close:
                        return "平仓";
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_ForceClose:
                        return "强平";
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseToday:
                        return "平今";
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseYesterday:
                        return "平昨";
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_ForceOff:
                        return "强减";
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_LocalForceClose:
                        return "本地强平";
                    default:
                        return string.Empty;
                }
            } }
        public TThostFtdcOffsetFlagType OffsetFlag { get { return _tf.OffsetFlag; } }
        public string OrderLocalID { get { return _tf.OrderLocalID; } }
        public string OrderRef { get { return _tf.OrderRef; } }
        public string OrderSysID { get { return _tf.OrderSysID; }}
        public string ParticipantID { get { return _tf.ParticipantID; } }
        public double Price { get { return _tf.Price; } }
        public TThostFtdcPriceSourceType PriceSource { get { return _tf.PriceSource; } }
        public int SequenceNo { get { return _tf.SequenceNo; } }
        public int SettlementID { get { return _tf.SettlementID; } }
        public string TradeDate { get { return _tf.TradeDate; } }
        public string TradeID { get { return _tf.TradeID; } }
        public string TraderID { get { return _tf.TraderID; } }
        public string TradeTime { get { return _tf.TradeTime; } }
        public TThostFtdcTradeTypeType TradeType { get { return _tf.TradeType; }}
        public string TradingDay { get { return _tf.TradingDay; } }
        public TThostFtdcTradingRoleType TradingRole { get { return _tf.TradingRole; } }
        public string UserID { get { return _tf.UserID; } }
        public int Volume { get { return _tf.Volume; } }
        public CThostFtdcTradeField CThostFtdcTradeFieldInstance
        {
            get { return _tf; }
            set { _tf = value; Notify(""); }
        }

        //构造函数
        public TradeField(CThostFtdcTradeField tf)
        {
            this._tf = tf;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        public bool Equals(TradeField other)
        {
            if (other == null) return false;
            return (this.TradeID == other.TradeID);
        }
    }
    #endregion TradeField

    #region TradingAccountField
    public class TradingAccountField: INotifyPropertyChanged, IEquatable<TradingAccountField>
    {
        private CThostFtdcTradingAccountField _taf;

        public string BrokerID { get { return _taf.BrokerID; } }
        public string AccountID { get { return _taf.AccountID; } }
        public double PreMortgage { get { return _taf.PreMortgage; } }
        public double PreCredit { get { return _taf.PreCredit; } }
        public double PreDeposit { get { return _taf.PreDeposit; } }
        public double PreBalance { get { return _taf.PreBalance; } }
        public double PreMargin { get { return _taf.PreMargin; } }
        public double InterestBase { get { return _taf.InterestBase; } }
        public double Interest { get { return _taf.Interest; } }
        public double Deposit { get { return _taf.Deposit; } }
        public double Withdraw { get { return _taf.Withdraw; } }
        public double FrozenMargin { get { return _taf.FrozenMargin; } }
        public double FrozenCash { get { return _taf.FrozenCash; } }
        public double FrozenCommission { get { return _taf.FrozenCommission; } }
        public double CurrMargin { get { return _taf.CurrMargin; } }
        public double CashIn { get { return _taf.CashIn; } }
        public double Commission { get { return _taf.Commission; } }
        public double CloseProfit { get { return _taf.CloseProfit; } }
        public double PositionProfit { get { return _taf.PositionProfit; } }
        public double Balance { get { return _taf.Balance; } }
        public double Available { get { return _taf.Available; } }
        public double WithdrawQuota { get { return _taf.WithdrawQuota; } }
        public double Reserve { get { return _taf.Reserve; } }
        public string TradingDay { get { return _taf.TradingDay; } }
        public int SettlementID { get { return _taf.SettlementID; } }
        public double Credit { get { return _taf.Credit; } }
        public double Mortgage { get { return _taf.Mortgage; } }
        public double ExchangeMargin { get { return _taf.ExchangeMargin; } }
        public double DeliveryMargin { get { return _taf.DeliveryMargin; } }
        public double ExchangeDeliveryMargin { get { return _taf.ExchangeDeliveryMargin; } }
        public double ReserveBalance { get { return _taf.ReserveBalance; } }
        public string CurrencyID { get { return _taf.CurrencyID; } }
        public double PreFundMortgageIn { get { return _taf.PreFundMortgageIn; } }
        public double PreFundMortgageOut { get { return _taf.PreFundMortgageOut; } }
        public double FundMortgageIn { get { return _taf.FundMortgageIn; } }
        public double FundMortgageOut { get { return _taf.FundMortgageOut; } }
        public double FundMortgageAvailable { get { return _taf.FundMortgageAvailable; } }
        public double MortgageableFund { get { return _taf.MortgageableFund; } }
        public double SpecProductMargin { get { return _taf.SpecProductMargin; } }
        public double SpecProductFrozenMargin { get { return _taf.SpecProductFrozenMargin; } }
        public double SpecProductCommission { get { return _taf.SpecProductCommission; } }
        public double SpecProductFrozenCommission { get { return _taf.SpecProductFrozenCommission; } }
        public double SpecProductPositionProfit { get { return _taf.SpecProductPositionProfit; } }
        public double SpecProductCloseProfit { get { return _taf.SpecProductCloseProfit; } }
        public double SpecProductPositionProfitByAlg { get { return _taf.SpecProductPositionProfitByAlg; } }
        public double SpecProductExchangeMargin { get { return _taf.SpecProductExchangeMargin; } }
        public CThostFtdcTradingAccountField CThostFtdcTradingAccountFieldInstance
        {
            get { return _taf; }
            set { this._taf = value; Notify(""); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        public bool Equals(TradingAccountField other)
        {
            if (other == null) return false;
            return (this.AccountID == other.AccountID);
        }
    }
    #endregion

    #region 快速生成Icomparer，作为比较参数
    public static class Equality<T>
    {
        public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector)
        {
            return new CommonEqualityComparer<V>(keySelector);
        }
        public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector, IEqualityComparer<V> comparer)
        {
            return new CommonEqualityComparer<V>(keySelector, comparer);
        }

        class CommonEqualityComparer<V> : IEqualityComparer<T>
        {
            private Func<T, V> keySelector;
            private IEqualityComparer<V> comparer;

            public CommonEqualityComparer(Func<T, V> keySelector, IEqualityComparer<V> comparer)
            {
                this.keySelector = keySelector;
                this.comparer = comparer;
            }
            public CommonEqualityComparer(Func<T, V> keySelector)
                : this(keySelector, EqualityComparer<V>.Default)
            { }

            public bool Equals(T x, T y)
            {
                return comparer.Equals(keySelector(x), keySelector(y));
            }
            public int GetHashCode(T obj)
            {
                return comparer.GetHashCode(keySelector(obj));
            }
        }
    }
    #endregion 

    //20170207，第三次换底层，希望这是最后一次。
    public class CTradeApi : ctp_trade
    {
        //内部变量
        private bool _isQrySettlementInfoCompleted, _isQryPositionDetailCompleted, _isQryPositionCompleted, _isQryInstrumentCompleted,
            _isQrySettlementInfoConfirmCompleted, _isQryTradingAccountCompleted, _isSettlementInfoConfirmCompleted;
        private StringBuilder _sbSettlementInfo, _sbSettlementInfoConfirm;
        private ConnectionStatus _connStatus;
        private readonly ConcurrentDictionary<string, InstrumentField> _dicInstrumentField; //InstrumentID
        private readonly ConcurrentDictionary<Tuple<string, string>, DetailPositionField> _dicDetailPositionField; //OpenDate、TradeID 
        private readonly ConcurrentDictionary<Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>, Position> _dicPosition; //合并的持仓：InstrumentID_买/卖_TThostFtdcHedgeFlagType
        private readonly ConcurrentDictionary<Tuple<int, int, string>, OrderField> _dicOrderField;   //FrontID、SessionID、OrderRef
        private readonly ConcurrentDictionary<Tuple<string, TThostFtdcDirectionType>, TradeField> _dicTradeField; //TradeID、Direction (区分自成交)
        private readonly ConcurrentStack<Tuple<EnumReqCmdType, object>> _stackQuery;
        private readonly List<Delegate> _listDele;
        private readonly List<CThostFtdcInvestorPositionField> _listPosition;
        private string _investor, _pwd, _broker, _addr;
        private int _frontID, _sessionID, _maxOrderRef;
        private TradingAccountField _taf;
        private DeleOnTrade _OnTrade;
        private DeleOnDetailPosition _OnDetailPosition;
        private DeleOnPosition _OnPosition;
        private DeleOnOrder _OnOrder;
        private DeleOnErrorOrder _OnErrorOrder;
        private DeleOnUpdate _OnUpdate;
        private DeleOnTradingAccount _OnTradingAccount;
        private bool _isFirstTimeLogin = true;
        private bool _canExecuteStackQuery = true;

        //构造函数
        public CTradeApi(string _investor, string _pwd, string _broker = "9999", string _addr = "tcp://180.168.146.187:10030", string pFile = "./ctp_dll/ctp_trade.dll") : base(pFile)
        {
            this._investor = _investor;
            this._pwd = _pwd;
            this._broker = _broker;
            this._addr = _addr;

            //内部变量初始化
            this._isQrySettlementInfoCompleted = true;
            this._isQryPositionCompleted = true;
            this._isQryPositionDetailCompleted = true;
            this._isQryInstrumentCompleted = true;
            this._isSettlementInfoConfirmCompleted = true;
            this._isQrySettlementInfoConfirmCompleted = true;
            this._sbSettlementInfo = new StringBuilder();
            this._sbSettlementInfoConfirm = new StringBuilder();
            this._connStatus = ConnectionStatus.Disconnected;
            this._dicInstrumentField = new ConcurrentDictionary<string, InstrumentField>();
            this._dicOrderField = new ConcurrentDictionary<Tuple<int, int, string>, OrderField>();
            this._dicTradeField = new ConcurrentDictionary<Tuple<string, TThostFtdcDirectionType>, TradeField>();
            this._dicDetailPositionField = new ConcurrentDictionary<Tuple<string, string>, DetailPositionField>();
            this._stackQuery = new ConcurrentStack<Tuple<EnumReqCmdType, object>>();
            this._listDele = new List<Delegate>();
            this._dicPosition = new ConcurrentDictionary<Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>, Position>();
            this._listPosition = new List<CThostFtdcInvestorPositionField>();
            this._taf = new TradingAccountField();

            //回调
            this.SetOnFrontConnected((DeleOnFrontConnected)AddDele(new DeleOnFrontConnected(CTradeApi_OnFrontConnected))); 
            this.SetOnFrontDisconnected((DeleOnFrontDisconnected)AddDele(new DeleOnFrontDisconnected(CTradeApi_OnFrontDisConnected)));
            this.SetOnRspUserLogout((DeleOnRspUserLogout)AddDele(new DeleOnRspUserLogout(CTradeApi_OnRspUserLogout)));
            this.SetOnRspError((DeleOnRspError)AddDele(new DeleOnRspError(CTradeApi_OnRspError)));
            this.SetOnRspUserLogin((DeleOnRspUserLogin)AddDele(new DeleOnRspUserLogin(CTradeApi_OnRspUserLogin))); 
            this.SetOnRspQryInstrument((DeleOnRspQryInstrument)AddDele(new DeleOnRspQryInstrument(CTradeApi_OnRspQryInstrument)));
            this.SetOnRspQrySettlementInfo((DeleOnRspQrySettlementInfo)AddDele(new DeleOnRspQrySettlementInfo(CTradeApi_OnRspQrySettlementInfo)));
            this.SetOnRspQrySettlementInfoConfirm((DeleOnRspQrySettlementInfoConfirm)AddDele(new DeleOnRspQrySettlementInfoConfirm(CTradeApi_OnRspQrySettlementInfoConfirm)));
            this.SetOnRspSettlementInfoConfirm((DeleOnRspSettlementInfoConfirm)AddDele(new DeleOnRspSettlementInfoConfirm(CTradeApi_OnRspSettlementInfoConfirm)));

            this.SetOnRspQryInvestorPositionDetail((DeleOnRspQryInvestorPositionDetail)AddDele(new DeleOnRspQryInvestorPositionDetail(CTradeApi_OnRspQryInvestorPositionDetail))); 
            this.SetOnRtnOrder((DeleOnRtnOrder)AddDele(new DeleOnRtnOrder(CTradeApi_OnRtnOrder)));  
            this.SetOnRspOrderInsert((DeleOnRspOrderInsert)AddDele(new DeleOnRspOrderInsert(CTradeApi_OnRspOrderInsert))); //Thost 收到报单指令，如果没有通过参数校验，拒绝接受报单指令。用户就会收到OnRspOrderInsert 消息，其中包含了错误编码和错误消息。
            this.SetOnErrRtnOrderInsert((DeleOnErrRtnOrderInsert)AddDele(new DeleOnErrRtnOrderInsert(CTradeApi_OnErrRtnOrderInsert))); 
            this.SetOnRtnTrade((DeleOnRtnTrade)AddDele(new DeleOnRtnTrade(CTradeApi_OnRtnTrade))); 
            this.SetOnRspQryInvestorPosition((DeleOnRspQryInvestorPosition)AddDele(new DeleOnRspQryInvestorPosition(CTradeApi_OnRspQryInvestorPosition))); 
            this.SetOnRspQryInvestorPositionCombineDetail((DeleOnRspQryInvestorPositionCombineDetail)AddDele(new DeleOnRspQryInvestorPositionCombineDetail(CTradeApi_OnRspQryInvestorPositionCombineDetail)));
            this.SetOnRspOrderAction((DeleOnRspOrderAction)AddDele(new DeleOnRspOrderAction(CTradeApi_OnRspOrderAction)));
            this.SetOnRspQryTradingAccount((DeleOnRspQryTradingAccount)AddDele(new DeleOnRspQryTradingAccount(CTradeApi_OnRspQryTradingAccount)));
        }

        //事件
        public delegate void DeleOnTrade(TradeField tf);
        public event DeleOnTrade OnTrade { add { this._OnTrade += value; } remove { this._OnTrade -= value; } }
        public delegate void DeleOnDetailPosition(DetailPositionField dpf);
        public event DeleOnDetailPosition OnDetailPosition { add { this._OnDetailPosition += value; } remove { this._OnDetailPosition -= value; } }
        public delegate void DeleOnPosition(Position pos);
        public event DeleOnPosition OnPosition { add { this._OnPosition += value; } remove { this._OnPosition -= value; } }
        public delegate void DeleOnOrder(OrderField of);
        public event DeleOnOrder OnOrder { add { this._OnOrder += value; } remove { this._OnOrder -= value; } }
        public delegate void DeleOnErrorOrder (OrderField orderField);
        public event DeleOnErrorOrder OnErrorOrder { add { this._OnErrorOrder += value; } remove { this._OnErrorOrder -= value; } }
        public delegate void DeleOnUpdate(object o);
        public event DeleOnUpdate OnUpdate { add { this._OnUpdate += value; } remove { this._OnUpdate -= value; } }
        public delegate void DeleOnTradingAccount(TradingAccountField taf);
        public event DeleOnTradingAccount OnTradingAccount { add { this._OnTradingAccount += value; } remove { this._OnTradingAccount -= value; } }

        //属性
        Delegate AddDele(Delegate d) { _listDele.Add(d); return d; }
        public string Investor { get { return this._investor; } }
        public string Broker { get { return this._broker; } }
        public string Address { get { return this._addr; } }
        public string Password { get { return this._pwd; } }
        public int FrontID { get { return this._frontID; } }
        public int SessionID { get { return this._sessionID; } }
        public ConcurrentDictionary<string, InstrumentField> DicInstrumentField { get { return _dicInstrumentField; } }
        public ConcurrentDictionary<Tuple<string, string>, DetailPositionField> DicDetailPositionField { get { return _dicDetailPositionField; } }
        public ConcurrentDictionary<Tuple<int, int, string>, OrderField> DicOrderField { get { return this._dicOrderField; } }
        public ConnectionStatus ConnStatus { get { return _connStatus; } }
        public string SettlementInfoContent { get { return _sbSettlementInfo.ToString(); } }
        public string SettlementInfoConfirmDateTime { get { return _sbSettlementInfoConfirm.ToString(); } }
        public ConcurrentStack<Tuple<EnumReqCmdType, object>> StackQuery { get { return _stackQuery; } }
        public ConcurrentDictionary<Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>, Position> DicPosition { get { return this._dicPosition; } }
        public TradingAccountField TradingAccountField { get { return this._taf; } }
        
        //流控查询
        public async Task ExecStackQuery()
        {
            if (!this._canExecuteStackQuery)
            {//防止上次查询还未完成就再次使用该函数！
                return;
            }
            this._canExecuteStackQuery = false;
            Tuple<EnumReqCmdType, object> query;

            while (this._stackQuery.TryPop(out query))
            {
                await Task.Delay(1000);
                switch (query.Item1)
                {
                    case EnumReqCmdType.QryInstrument:
                        await Task.Run(async () =>
                        {
                            this._isQryInstrumentCompleted = false;
                            this.ReqQryInstrument(InstrumentID: (string)query.Item2, ExchangeID: "", ExchangeInstID: "", ProductID: "");
                            while (!this._isQryInstrumentCompleted)
                                await Task.Delay(100);
                        });
                        break;
                    case EnumReqCmdType.QryInvestorPositionDetail:
                        await Task.Run(async () =>
                        {
                            this._isQryPositionDetailCompleted = false;
                            this.ReqQryInvestorPositionDetail(this._broker, this._investor, InstrumentID: (string)query.Item2);
                            while (!this._isQryPositionDetailCompleted)
                                await Task.Delay(100);
                        });
                        break;
                    case EnumReqCmdType.QrySettlementInfo:
                        await Task.Run(async () =>
                        {
                            this._isQrySettlementInfoCompleted = false;
                            _sbSettlementInfo.Clear();
                            this.ReqQrySettlementInfo(this._broker, this._investor, TradingDay: (string)query.Item2);
                            while (!this._isQrySettlementInfoCompleted)
                                await Task.Delay(100);
                        });
                        break;
                    case EnumReqCmdType.QrySettlementInfoConfirm:
                        await Task.Run(async () =>
                        {
                            this._isQrySettlementInfoConfirmCompleted = false;
                            this.ReqQrySettlementInfoConfirm(this._broker, this._investor);
                            while (!this._isQrySettlementInfoConfirmCompleted)
                                await Task.Delay(100);
                        });
                        break;
                    case EnumReqCmdType.SettlementInfoConfirm:
                        await Task.Run(async () =>
                        {
                            this._isSettlementInfoConfirmCompleted = false;
                            this.ReqSettlementInfoConfirm(this._broker, this._investor);
                            while (!this._isSettlementInfoConfirmCompleted)
                                await Task.Delay(100);
                        });
                        break;
                    case EnumReqCmdType.QryInvestorPosition:
                        await Task.Run(async () =>
                        {
                            this._isQryPositionCompleted = false;
                            this.ReqQryInvestorPosition(BrokerID: this._broker, InvestorID: this._investor, InstrumentID: (string)query.Item2);
                            while (!this._isQryPositionCompleted)
                                await Task.Delay(100);
                        });
                        break;
                    case EnumReqCmdType.QryTradingAccount:
                        await Task.Run(async () =>
                        {
                            this._isQryTradingAccountCompleted = false;
                            this.ReqQryTradingAccount(BrokerID: this._broker, InvestorID: this._investor);
                            while (!this._isQryTradingAccountCompleted)
                            {
                                await Task.Delay(100);
                            }
                        });
                        break;
                    default:
                        break;
                }
            }
            this._canExecuteStackQuery = true;
        }

        //查询函数
        public async Task AsyncConnect(CancellationToken CancelToken)
        {
            if (this._connStatus == ConnectionStatus.Connected || this._connStatus == ConnectionStatus.Connecting)
            {
                return;
            }
            this.RegisterFront(this._addr);
            this.SubscribePrivateTopic(THOST_TE_RESUME_TYPE.THOST_TERT_RESTART);
            this.SubscribePublicTopic(THOST_TE_RESUME_TYPE.THOST_TERT_RESTART);
            int result = (int)this.Init();
            if (result != 0)
            {
                throw new Exception(result.ToString());
            }
            while (this._connStatus != ConnectionStatus.Connected)
            {
                await Task.Delay(10);
                CancelToken.ThrowIfCancellationRequested();
            }
        }
        public async Task AsyncLogin(CancellationToken CancelToken)
        {
            int result = (int)base.ReqUserLogin(BrokerID: this._broker,UserID: this._investor, Password: this._pwd, UserProductInfo: "@HaiFeng_WHWING");
            if (result != 0)
            {
                throw new Exception(result.ToString());
            }
            this._connStatus = ConnectionStatus.Logining;
            while (this._connStatus != ConnectionStatus.Logined)
            {
                await Task.Delay(10);
                CancelToken.ThrowIfCancellationRequested();
            }
        }
        public void ReqUserLogout()
        {
            this._connStatus = ConnectionStatus.Logout;
            //取消连接响应,避免重连后的再登录.
            this.SetOnFrontDisconnected(null);
            this.SetOnFrontConnected(null);
            this.Release();
        }
        //下普通单
        public void SendOrder(OrderField of)
        {//封装下单函数
            //OrderRef如果没有填（null），则为""
            string _orderRef = (of.OrderRef != null ? of.OrderRef : string.Empty);
            //当日有效(默认)
            TThostFtdcTimeConditionType _timeCondition = (of.TimeCondition == 0 ? TThostFtdcTimeConditionType.THOST_FTDC_TC_GFD:of.TimeCondition);
            //任何数量
            TThostFtdcVolumeConditionType _volumeCondition = (of.VolumeCondition == 0 ? TThostFtdcVolumeConditionType.THOST_FTDC_VC_AV:of.VolumeCondition);
            //立即执行
            TThostFtdcContingentConditionType _contingentCondition = (of.ContingentCondition == 0 ? TThostFtdcContingentConditionType.THOST_FTDC_CC_Immediately:of.ContingentCondition);
            //是否强平
            TThostFtdcForceCloseReasonType _forceCloseReason = (of.ForceCloseReason == 0 ? TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_NotForceClose : of.ForceCloseReason);
            //默认是投机单
            string _combHedgeFlag = (of.CombHedgeFlag == null ? new string((char)TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation, 1):of.CombHedgeFlag);

            this.ReqOrderInsert(
                BrokerID: this._broker,
                InvestorID: this._investor, 
                InstrumentID: of.InstrumentID,
                OrderRef: _orderRef,
                CombHedgeFlag: _combHedgeFlag,
                CombOffsetFlag: of.CombOffsetFlag,
                Direction: of.Direction,
                VolumeTotalOriginal: of.VolumeTotalOriginal,
                ForceCloseReason: _forceCloseReason,
                ContingentCondition: _contingentCondition,
                VolumeCondition: _volumeCondition,
                LimitPrice: of.LimitPrice,
                IsSwapOrder:0,
                MinVolume: 1,
                UserForceClose: of.UserForceClose,
                TimeCondition: _timeCondition,
                OrderPriceType: of.OrderPriceType
                  );                
        }
        //撤销普通单
        public void CancelOrder(OrderField of)
        {//OrderRef和InstrumentID
            this.ReqOrderAction(this._broker, this._investor, InstrumentID: of.InstrumentID,
                OrderRef: of.OrderRef,
                FrontID: of.FrontID,
                SessionID: of.SessionID,
                ActionFlag: TThostFtdcActionFlagType.THOST_FTDC_AF_Delete);
        }
        //下预埋单
        public void SendParkedOrder(OrderField of)
        {
            string _orderRef = (of.OrderRef != null ? of.OrderRef : string.Empty);
            //当日有效(默认)
            TThostFtdcTimeConditionType _timeCondition = (of.TimeCondition == 0 ? TThostFtdcTimeConditionType.THOST_FTDC_TC_GFD : of.TimeCondition);
            //任何数量
            TThostFtdcVolumeConditionType _volumeCondition = (of.VolumeCondition == 0 ? TThostFtdcVolumeConditionType.THOST_FTDC_VC_AV : of.VolumeCondition);
            //立即执行
            TThostFtdcContingentConditionType _contingentCondition = (of.ContingentCondition == 0 ? TThostFtdcContingentConditionType.THOST_FTDC_CC_Immediately : of.ContingentCondition);
            //是否强平
            TThostFtdcForceCloseReasonType _forceCloseReason = (of.ForceCloseReason == 0 ? TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_NotForceClose : of.ForceCloseReason);

            this.ReqParkedOrderInsert(
                BrokerID: this._broker,
                InvestorID: this._investor,
                InstrumentID: of.InstrumentID,
                OrderRef: _orderRef,
                CombHedgeFlag: of.CombHedgeFlag,
                CombOffsetFlag: of.CombOffsetFlag,
                Direction: of.Direction,
                VolumeTotalOriginal: of.VolumeTotalOriginal,
                ForceCloseReason: _forceCloseReason,
                ContingentCondition: _contingentCondition,
                VolumeCondition: _volumeCondition,
                LimitPrice: of.LimitPrice,
                IsSwapOrder: 0,
                MinVolume: 1,
                UserForceClose: of.UserForceClose,
                TimeCondition: _timeCondition,
                OrderPriceType: of.OrderPriceType
                  );                

        }
        //预埋单撤单
        public void CancelParkedOrder(OrderField of)
        {
            //要根据ParkedOrderActionID来撤单

        }
        #region 封装回调函数
        private void CTradeApi_OnRspError(ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new Exception(string.Format("ErrorID:{0},ErrorMsg:{1}", pRspInfo.ErrorID, pRspInfo.ErrorMsg));
        }
        private void CTradeApi_OnFrontConnected()
        {
            this._connStatus = ConnectionStatus.Connected;
        }
        private void CTradeApi_OnFrontDisConnected(int reason)
        {
            this._connStatus = ConnectionStatus.Disconnected;
     
        }
        private void CTradeApi_OnRspUserLogin(ref CThostFtdcRspUserLoginField pRspUserLogin, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast && pRspInfo.ErrorID == 0)
            {
                this._connStatus = ConnectionStatus.Logined;
                this._frontID = pRspUserLogin.FrontID;
                this._sessionID = pRspUserLogin.SessionID;
                this._maxOrderRef = int.Parse(pRspUserLogin.MaxOrderRef);
            }
        }
        private void CTradeApi_OnRspUserLogout(ref CThostFtdcUserLogoutField pUserLogout, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast && pRspInfo.ErrorID == 0)
            {
                this._connStatus = ConnectionStatus.Logout;
            }

        }
        //查询合约回报
        private void CTradeApi_OnRspQryInstrument(ref CThostFtdcInstrumentField pInstrument, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (pRspInfo.ErrorID == 0)
            {
                CThostFtdcInstrumentField tmpInstrument = pInstrument;
                this._dicInstrumentField.AddOrUpdate(pInstrument.InstrumentID, new InstrumentField(pInstrument),
                    (k, v) => { v.CThostFtdcInstrumentFieldInstance = tmpInstrument; return v; });
            }
            _isQryInstrumentCompleted = bIsLast;
        }
        //查询结算信息回报
        private void CTradeApi_OnRspQrySettlementInfo(ref CThostFtdcSettlementInfoField pSettlementInfo, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (pRspInfo.ErrorID == 0)
            {
                _sbSettlementInfo.Append(pSettlementInfo.Content);
            }
            this._isQrySettlementInfoCompleted = bIsLast;
        }
        //查询结算确认回报
        private void CTradeApi_OnRspQrySettlementInfoConfirm(ref CThostFtdcSettlementInfoConfirmField pSettlementInfoConfirm, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (pRspInfo.ErrorID == 0)
            {
                _sbSettlementInfoConfirm.Append(string.Format("InvestorID:{0}; BrokerID:{1}; ConfirmDate:{2}; ConfrimTime:{3}",
                    pSettlementInfoConfirm.InvestorID, pSettlementInfoConfirm.BrokerID, pSettlementInfoConfirm.ConfirmDate, pSettlementInfoConfirm.ConfirmTime));
            }
            this._isQrySettlementInfoConfirmCompleted = bIsLast;
        }
        //确认结算回报
        private void CTradeApi_OnRspSettlementInfoConfirm(ref CThostFtdcSettlementInfoConfirmField pSettlementInfoConfirm, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            this._isSettlementInfoConfirmCompleted = bIsLast; 
        }
        //查询持仓细节回报
        private void CTradeApi_OnRspQryInvestorPositionDetail(ref CThostFtdcInvestorPositionDetailField pInvestorPositionDetail, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (pRspInfo.ErrorID == 0 && !string.IsNullOrEmpty(pInvestorPositionDetail.TradeID))
            {
                CThostFtdcInvestorPositionDetailField tmpPositionDetail = pInvestorPositionDetail;
                var value = this._dicDetailPositionField.AddOrUpdate(new Tuple<string, string>(pInvestorPositionDetail.OpenDate, pInvestorPositionDetail.TradeID),
                    new DetailPositionField(pInvestorPositionDetail), (k, v) => { v.InvestorPositionDetailFieldInstance = tmpPositionDetail; return v; });
                this._OnDetailPosition?.Invoke(value);
            }
            if (bIsLast)
            {
                this._OnUpdate?.Invoke(this._dicDetailPositionField);
            }
            this._isQryPositionDetailCompleted = bIsLast;
        }
        //普通单成功下单后回报
        private void CTradeApi_OnRtnOrder(ref CThostFtdcOrderField pOrder)
        {
            CThostFtdcOrderField tmpOrder = pOrder;
            var value = this._dicOrderField.AddOrUpdate(new Tuple<int, int, string>(pOrder.FrontID, pOrder.SessionID, pOrder.OrderRef), new OrderField(pOrder),
                (k, v) => { v.OrderFieldInstance = tmpOrder; return v; });
            this._OnOrder.Invoke(value);
        }
        //成交回报
        private void CTradeApi_OnRtnTrade(ref CThostFtdcTradeField pTrade)
        {
            var key = new Tuple<string, TThostFtdcDirectionType>(pTrade.TradeID, pTrade.Direction);
            CThostFtdcTradeField tmp = pTrade;
            var value = this._dicTradeField.AddOrUpdate(key, new TradeField(pTrade), (k, v) => { v.CThostFtdcTradeFieldInstance = tmp; return v; });
            if (this._isFirstTimeLogin)
            {
                this._OnTrade?.Invoke(value);
            }
            else
            { //对于第一次登陆，由于先返回的是trade，所以不能调用UpdatePositionFromRtnTrade；只有当Position查询过后，才可以调用！
                UpdatePositionFromRtnTrade(new TradeField(pTrade));
                this._OnTrade?.Invoke(value);
            }
        }
        //查询持仓回报
        private void CTradeApi_OnRspQryInvestorPosition(ref CThostFtdcInvestorPositionField pInvestorPosition, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (!string.IsNullOrEmpty(pInvestorPosition.InstrumentID))
            {
                var f = pInvestorPosition;
                if (f.Position > 0)    //Position字段即总仓位
                {
                    //if (f.PositionDate==TThostFtdcPositionDateType.THOST_FTDC_PSD_History)
                    //{
                    //    f.YdPosition -= f.CloseVolume;
                    //}
                    //else
                    //{
                    //    f.TodayPosition -= f.CloseVolume;
                    //}
                    _listPosition.Add(f);
                }
            }
            if (bIsLast)
            {
                this._isFirstTimeLogin = false; 
                foreach (var g in _listPosition.GroupBy(p => p.InstrumentID + "_" + p.PosiDirection +"_" + p.HedgeFlag))
                {
                    var id = g.First();
                    var instrumentID = id.InstrumentID;
                    var direction = id.PosiDirection;
                    var hedge = id.HedgeFlag;
                    var key = new Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>( instrumentID, direction,hedge);
                    var pos = this._dicPosition.GetOrAdd(key, new Position(instrumentID, direction, hedge));
                    #region 持仓赋值
                    pos.TdPosition = g.Sum(n => n.TodayPosition);
                    pos.YdPosition = g.Sum(n => n.YdPosition);
                    if (pos.TotalPosition == 0)
                    {
                        this._dicPosition.TryRemove(key, out pos);
                        continue;
                    }
                    pos.BrokerID = id.BrokerID;
                    pos.InvestorID = id.InvestorID;
                    pos.PositionDate = g.Max(n=>n.PositionDate);
                    pos.LongFrozen = g.Sum(n => n.LongFrozen);
                    pos.ShortFrozen = g.Sum(n => n.ShortFrozen);
                    pos.LongFrozenAmount = g.Sum(n => n.LongFrozenAmount);
                    pos.ShortFrozenAmount = g.Sum(n => n.ShortFrozenAmount);
                    pos.OpenVolume = g.Sum(n => n.OpenVolume);
                    pos.CloseVolume = g.Sum(n => n.CloseVolume);
                    pos.OpenAmount = g.Sum(n => n.OpenAmount);
                    pos.CloseAmount = g.Sum(n => n.CloseAmount);
                    pos.PositionCost = g.Sum(n => n.PositionCost);
                    pos.PreMargin = g.Sum(n => n.PreMargin);
                    pos.UseMargin = g.Sum(n => n.UseMargin);
                    pos.FrozenMargin = g.Sum(n => n.FrozenMargin);
                    pos.FrozenCash = g.Sum(n => n.FrozenCash);
                    pos.FrozenCommission = g.Sum(n => n.FrozenCommission);
                    pos.CashIn = g.Sum(n => n.CashIn);
                    pos.Commission = g.Sum(n => n.Commission);
                    pos.CloseProfit = g.Sum(n => n.CloseProfit);
                    pos.PositionProfit = g.Sum(n => n.PositionProfit);
                    pos.PreSettlementPrice = g.Sum(n => n.PreSettlementPrice);
                    pos.SettlementPrice = g.Sum(n => n.SettlementPrice);
                    pos.TradingDay = g.Max(n => n.TradingDay);
                    pos.OpenCost = g.Sum(n => n.OpenCost);
                    pos.ExchangeMargin = g.Sum(n => n.ExchangeMargin);
                    pos.CombPosition = g.Sum(n => n.CombPosition);
                    pos.CombLongFrozen = g.Sum(n => n.CombLongFrozen);
                    pos.CombShortFrozen = g.Sum(n => n.CombShortFrozen);
                    pos.CloseProfitByDate = g.Sum(n => n.CloseProfitByDate);
                    pos.CloseProfitByTrade = g.Sum(n => n.CloseProfitByTrade);
                    pos.MarginRateByMoney = id.MarginRateByMoney;
                    pos.MarginRateByVolume = id.MarginRateByVolume;
                    pos.StrikeFrozen = g.Sum(n=>n.StrikeFrozen);
                    pos.StrikeFrozenAmount = g.Sum(n => n.StrikeFrozenAmount);
                    pos.AbandonFrozen = g.Sum(n => n.AbandonFrozen);
                    InstrumentField instrument;
                    this._dicInstrumentField.TryGetValue(pos.InstrumentID, out instrument);
                    if (instrument != null)
                    {//OpenProfit是自设字段，表示开仓后到现在的总盈亏
                        pos.AvgOpenPrice = pos.OpenCost / pos.TotalPosition / instrument.VolumeMultiple;
                        if (pos.PosiDirection==TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long)
                        {
                            pos.OpenProfit = (pos.LastPrice - pos.AvgOpenPrice) * pos.TotalPosition * instrument.VolumeMultiple;
                        }
                        else if (pos.PosiDirection == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short)
                        {
                            pos.OpenProfit = - (pos.LastPrice - pos.AvgOpenPrice) * pos.TotalPosition * instrument.VolumeMultiple;
                        }

                    }
                    #endregion
                    pos.Notify("");
                    //外部函数引发调用
                    this._OnPosition?.Invoke(pos);   
                }
                this._OnUpdate?.Invoke(this._dicPosition);
                #region TradingAccount
                //TradingAccount.CloseProfit = _listPosi.Sum(n => n.CloseProfit);
                //TradingAccount.PositionProfit = _listPosi.Sum(n => n.PositionProfit);
                //TradingAccount.Commission = _listPosi.Sum(n => n.Commission);
                //TradingAccount.Fund = TradingAccount.PreBalance + TradingAccount.CloseProfit + TradingAccount.PositionProfit - TradingAccount.Commission;
                //TradingAccount.FrozenCash = _listPosi.Sum(n => n.FrozenCash);
                ////由查帐户资金函数处理,原因:保证金有单边收的情况无法用持仓统计
                ////TradingAccount.CurrMargin = _listPosi.Sum(n => n.UseMargin);
                ////TradingAccount.Available = TradingAccount.Fund - TradingAccount.CurrMargin - TradingAccount.FrozenCash;
                ////TradingAccount.Risk = TradingAccount.CurrMargin / TradingAccount.Fund;
                #endregion
                _listPosition.Clear();//清除,以便得到结果是重新添加
            }
            _isQryPositionCompleted = bIsLast;
        }
        //错单回报
        private void CTradeApi_OnErrRtnOrderInsert(ref CThostFtdcInputOrderField pInputOrder, ref CThostFtdcRspInfoField pRspInfo)
        {
            OrderField of = this.InputOrderFieldToOrderField(pInputOrder, pRspInfo);
            this._OnErrorOrder?.Invoke(of);
        }
        //下普通单错误时回报
        private void CTradeApi_OnRspOrderInsert(ref CThostFtdcInputOrderField pInputOrder, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            OrderField of = this.InputOrderFieldToOrderField(pInputOrder, pRspInfo);
            this._OnErrorOrder?.Invoke(of);
        }
        //撤普通单错误时回报
        private void CTradeApi_OnRspOrderAction(ref CThostFtdcInputOrderActionField pInputOrderAction, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            OrderField of = new OrderField();
            of.StatusMsg = string.Format("{0}：{1}", pRspInfo.ErrorID, pRspInfo.ErrorMsg);
            of.InstrumentID = pInputOrderAction.InstrumentID;
            of.OrderRef = pInputOrderAction.OrderRef;
            of.LimitPrice = pInputOrderAction.LimitPrice;
            of.OrderSysID = pInputOrderAction.OrderSysID;
            of.SessionID = pInputOrderAction.SessionID;
            of.FrontID = pInputOrderAction.FrontID;
            of.InvestorID = pInputOrderAction.InvestorID;
            of.BrokerID = pInputOrderAction.BrokerID;
            this._OnErrorOrder?.Invoke(of);
        }
        private void CTradeApi_OnRspQryInvestorPositionCombineDetail(ref CThostFtdcInvestorPositionCombineDetailField pInvestorPositionCombineDetail, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            //throw new NotImplementedException();
        }
        private void CTradeApi_OnRspQryTradingAccount(ref CThostFtdcTradingAccountField pTradingAccount, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            this._taf.CThostFtdcTradingAccountFieldInstance = pTradingAccount;
            if (bIsLast)
            {
                this._OnTradingAccount?.Invoke(this._taf);
            }
            this._isQryTradingAccountCompleted = bIsLast;
        }

        #endregion 封装回调函数


        //转换函数：InputOrderField转换为OrderField
        private OrderField InputOrderFieldToOrderField (CThostFtdcInputOrderField iof, CThostFtdcRspInfoField pRspInfo)
        {
            OrderField of = new OrderField();
            of.BrokerID = iof.BrokerID;
            of.InvestorID = iof.InvestorID;
            of.InstrumentID = iof.InstrumentID;
            of.OrderRef = iof.OrderRef;
            of.UserID = iof.UserID;
            of.CombHedgeFlag = iof.CombHedgeFlag;
            of.CombHedgeFlag = iof.CombHedgeFlag;
            of.ContingentCondition = iof.ContingentCondition;
            of.Direction = iof.Direction;
            of.ExchangeID = iof.ExchangeID;
            of.ForceCloseReason = iof.ForceCloseReason;
            of.GTDDate = iof.GTDDate;
            of.LimitPrice = iof.LimitPrice;
            of.StopPrice = iof.StopPrice;
            of.OrderPriceType = iof.OrderPriceType;
            of.TimeCondition = iof.TimeCondition;
            of.VolumeTotalOriginal = iof.VolumeTotalOriginal;
            of.MinVolume = iof.MinVolume;
            of.VolumeCondition = iof.VolumeCondition;
            of.StatusMsg = string.Format("{0}: {1}", pRspInfo.ErrorID, pRspInfo.ErrorMsg);
            return of;
        }
        //每当有报单成交时，则要更新持仓
        private void UpdatePositionFromRtnTrade(TradeField tf)
        {
            lock (this._dicPosition) //Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>
            {
                //更新_dicPosition
                InstrumentField instrument;
                if (!this._dicInstrumentField.TryGetValue(tf.InstrumentID, out instrument))
                {
                    return;
                }
                TThostFtdcPosiDirectionType direction = TThostFtdcPosiDirectionType.THOST_FTDC_PD_Net;
                TThostFtdcHedgeFlagType hedge = tf.HedgeFlag;
                switch (tf.Direction)
                {
                    case TThostFtdcDirectionType.THOST_FTDC_D_Buy:
                        direction = TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long;
                        break;
                    case TThostFtdcDirectionType.THOST_FTDC_D_Sell:
                        direction = TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short;
                        break;
                    default:
                        break;
                }

                switch (tf.OffsetFlag)
                {
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_Open:   //开仓成交
                        foreach (var kvp in this._dicPosition)
                        {//根据合约ID、方向、投保，找出是否有相同的键在字典内，如果有则更新
                            if (kvp.Key.Item1 == tf.InstrumentID && kvp.Key.Item2 == direction && kvp.Key.Item3 == hedge)
                            {
                                kvp.Value.TdPosition += tf.Volume;
                                kvp.Value.PositionCost += tf.Price * tf.Volume * instrument.VolumeMultiple;
                                kvp.Value.OpenCost += tf.Price * tf.Volume * instrument.VolumeMultiple;
                                kvp.Value.AvgOpenPrice = kvp.Value.OpenCost / kvp.Value.TotalPosition / instrument.VolumeMultiple;
                                this._OnUpdate?.Invoke(this._dicPosition);
                                return;
                            }
                        }
                        //没有持仓，则新建
                        Position pos = CreatePositionFromRtnTrade(tf);
                        this._dicPosition.TryAdd( new Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>(tf.InstrumentID,direction,hedge), pos);
                        this._OnUpdate?.Invoke(this._dicPosition);
                        break;

                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseToday: //平今
                        foreach (var kvp in this._dicPosition)
                        {//根据合约ID、方向、投保，找出是否有相同的键在字典内，如果有则更新
                            if (kvp.Key.Item1 == tf.InstrumentID && kvp.Key.Item3 == hedge &&
                                ((kvp.Key.Item2 == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long && direction == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short)
                                || (kvp.Key.Item2 == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short && direction == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long))
                                )
                            {
                                kvp.Value.TdPosition -= tf.Volume;
                                kvp.Value.PositionCost -= tf.Price * tf.Volume * instrument.VolumeMultiple;
                                if (kvp.Value.TotalPosition == 0)
                                {
                                    Position tmp;
                                    if (this._dicPosition.TryRemove(kvp.Key, out tmp))
                                    {
                                        this._OnUpdate?.Invoke(this._dicPosition);
                                        return;
                                    }
                                }
                                kvp.Value.OpenCost = kvp.Value.OpenCost * kvp.Value.TotalPosition / (kvp.Value.TotalPosition + tf.Volume);
                                kvp.Value.AvgOpenPrice = kvp.Value.OpenCost / kvp.Value.TotalPosition / instrument.VolumeMultiple;
                                this._OnUpdate?.Invoke(this._dicPosition);
                                return;
                            }
                        }
                        this._OnUpdate?.Invoke(this._dicPosition);
                        break;
                    
                    //平昨
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_Close:    
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseYesterday:
                        foreach(var kvp in this._dicPosition)
                        {//根据合约ID、方向、投保，找出是否有相同的键在字典内，如果有则更新
                            if (
                                kvp.Key.Item1 == tf.InstrumentID && kvp.Key.Item3 == hedge &&
                                ((kvp.Key.Item2 == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long && direction == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short)
                                || (kvp.Key.Item2 == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short && direction == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long))
                                )
                            {
                                kvp.Value.YdPosition -= tf.Volume;
                                kvp.Value.PositionCost -= tf.Price * tf.Volume * instrument.VolumeMultiple;
                                if (kvp.Value.TotalPosition == 0)
                                {
                                    Position tmp;
                                    if (this._dicPosition.TryRemove(kvp.Key, out tmp))
                                    {
                                        this._OnUpdate?.Invoke(this._dicPosition);
                                        return;
                                    }
                                }
                                kvp.Value.OpenCost = kvp.Value.OpenCost * kvp.Value.TotalPosition / (kvp.Value.TotalPosition + tf.Volume);
                                kvp.Value.AvgOpenPrice = kvp.Value.OpenCost / kvp.Value.TotalPosition / instrument.VolumeMultiple;
                                this._OnUpdate?.Invoke(this._dicPosition);
                                return;
                            }
                        }
                        this._OnUpdate?.Invoke(this._dicPosition);
                        break;
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_ForceOff:
                        break;
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_LocalForceClose:
                        break;
                    case TThostFtdcOffsetFlagType.THOST_FTDC_OF_ForceClose:
                        break;
                    default:
                        break;
                }
            }
        }
        //根据成交报单来构造持仓
        private Position CreatePositionFromRtnTrade(TradeField tf)
        {
            TThostFtdcPosiDirectionType direction = TThostFtdcPosiDirectionType.THOST_FTDC_PD_Net;
            TThostFtdcHedgeFlagType hedge = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation;
            switch (tf.Direction)
            {
                case TThostFtdcDirectionType.THOST_FTDC_D_Buy:
                    direction = TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long;
                    break;
                case TThostFtdcDirectionType.THOST_FTDC_D_Sell:
                    direction = TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short;
                    break;
                default:
                    break;
            }

            Position pos = new Position(tf.InstrumentID, direction, hedge);
            pos.AvgOpenPrice = tf.Price;
            pos.PositionDate = TThostFtdcPositionDateType.THOST_FTDC_PSD_Today;
            pos.TdPosition = tf.Volume;
            pos.InvestorID = tf.InvestorID;
            InstrumentField instrument;
            if (this._dicInstrumentField.TryGetValue(tf.InstrumentID, out instrument))
            {
                pos.OpenCost = tf.Price * tf.Volume * instrument.VolumeMultiple;
                pos.PositionCost = pos.OpenCost;
            }
            pos.Notify("");
            return pos;
        }
    }

}
