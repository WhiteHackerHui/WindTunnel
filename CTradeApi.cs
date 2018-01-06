using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WT_Core;

namespace WT_CTP
{
    public sealed class CTradeApi : HaiFeng.ctp_trade
    {
        #region ==========内部变量==========

        //查询同步信号
        private bool _isQrySettlementInfoCompleted          = true;
        private bool _isQryPositionDetailCompleted          = true;
        private bool _isQryPositionCompleted                = true;
        private bool _isQryInstrumentCompleted              = true;
        private bool _isQrySettlementInfoConfirmCompleted   = true;
        private bool _isQryTradingAccountCompleted          = true;
        private bool _isSettlementInfoConfirmCompleted      = true;
 
        //报单字典：FrontID、SessionID、OrderRef
        private readonly ConcurrentDictionary<Tuple<int, int, string>, OrderField> _DictOrderField = new ConcurrentDictionary<Tuple<int, int, string>, OrderField>();
        //合约字典：InstrumentID
        private readonly ConcurrentDictionary<string, InstrumentField> _DictInstrumentField = new ConcurrentDictionary<string, InstrumentField>();
        //合并持仓字典：InstrumentID、买/卖、投保
        private readonly ConcurrentDictionary<Tuple<string, Direction, HedgeFlag>, Position> _DictInvestorPosition = new ConcurrentDictionary<Tuple<string, Direction, HedgeFlag>, Position>();
        //持仓细节字典：OpenDate、TradeID 
        private readonly ConcurrentDictionary<Tuple<string, string>, DetailPosition> _DictDetailPosition = new ConcurrentDictionary<Tuple<string, string>, DetailPosition>();
        //交易单字典：TradeID、Direction (区分自成交)
        private readonly ConcurrentDictionary<Tuple<string, Direction>, Trade> _DictTrade = new ConcurrentDictionary<Tuple<string, Direction>, Trade>();

        //回报的持仓列表
        private readonly List<CThostFtdcInvestorPositionField> _ListPositionField = new List<CThostFtdcInvestorPositionField>();
        //委托列表
        private readonly List<Delegate> _ListDele = new List<Delegate>();
        //查询队列
        private string _investor, _pwd, _broker, _addr;
        private int _frontID, _sessionID, _maxOrderRef;
        private TradeAccount _taf = new TradeAccount();
        private bool _isFirstTimeLogin = true; //当日头一次登录
        private ConnectionStatus _connStatus = ConnectionStatus.Disconnected;
        private DateTime _LastReqTime = DateTime.MinValue;  //上次查询的时间
        private StringBuilder _sbSettlementInfo = new StringBuilder(), _sbSettlementInfoConfirm = new StringBuilder();
        private int _MinReqTimeInterval = 1000;  //最小查询间隔：1000毫秒
        private object _reqLock = new object();
        private string _errorMsg;
        //private readonly ConcurrentQueue<Tuple<ReqCmdType, object>> _QueryQueue = new ConcurrentQueue<Tuple<ReqCmdType, object>>();
        
        //内部委托  
        private DeleOnTrade _OnTrade;
        private DeleOnDetailPosition _OnDetailPosition;
        private DeleOnPosition _OnPosition;
        private DeleOnOrderField _OnOrderField;
        private DeleOnErrorOrder _OnErrorOrder;
        private DeleOnPositionChanged _OnPositionChanged;
        private DeleOnTradeAccount _OnTradingAccount;
        private DeleOnConnectionStatusChanged _OnConnectionStatusChanged;

        #endregion ==========内部变量==========

        #region ==========事件==========

        public delegate void DeleOnTrade(Trade tf);
        public event DeleOnTrade OnTrade { add { this._OnTrade += value; } remove { this._OnTrade -= value; } }
        public delegate void DeleOnDetailPosition(DetailPosition dpf);
        public event DeleOnDetailPosition OnDetailPosition { add { this._OnDetailPosition += value; } remove { this._OnDetailPosition -= value; } }
        public delegate void DeleOnPosition(Position pos);
        public event DeleOnPosition OnPosition { add { this._OnPosition += value; } remove { this._OnPosition -= value; } }
        public delegate void DeleOnOrderField(OrderField of);
        public event DeleOnOrderField OnOrderField { add { this._OnOrderField += value; } remove { this._OnOrderField -= value; } }
        public delegate void DeleOnErrorOrder(OrderField orderField);
        public event DeleOnErrorOrder OnErrorOrder { add { this._OnErrorOrder += value; } remove { this._OnErrorOrder -= value; } }
        public delegate void DeleOnPositionChanged(object position);
        public event DeleOnPositionChanged OnPositionChanged { add { this._OnPositionChanged += value; } remove { this._OnPositionChanged -= value; } }
        public delegate void DeleOnTradeAccount(TradeAccount taf);
        public event DeleOnTradeAccount OnTradeAccount { add { this._OnTradingAccount += value; } remove { this._OnTradingAccount -= value; } }
        public delegate void DeleOnConnectionStatusChanged(ConnectionStatus cs);
        public event DeleOnConnectionStatusChanged OnConnectionStatusChanged { add { this._OnConnectionStatusChanged += value; } remove { this._OnConnectionStatusChanged -= value; } }


        #endregion #region ==========事件==========

        #region ==========属性==========

        private Delegate AddDele(Delegate d) { _ListDele.Add(d); return d; }
        public string Investor { get { return this._investor; } set { this._investor = value; } }
        public string Broker { get { return this._broker; } set { this._broker = value; } }
        public string Address { get { return this._addr; } set { this._addr = value; } }
        public string Password { get { return this._pwd; } set { this._pwd = value; } }
        public int FrontID { get { return this._frontID; } }
        public int SessionID { get { return this._sessionID; } }
        [DisplayName("合约信息字典，主键为<InstrumentID>")]
        public ConcurrentDictionary<string, InstrumentField> DictInstrumentField { get { return _DictInstrumentField; } }
        [DisplayName("持仓细节字典，主键为：<OpenDate,TradeID> ")]
        public ConcurrentDictionary<Tuple<string, string>, DetailPosition> DictDetailPosition { get { return _DictDetailPosition; } }
        [DisplayName("报单字典，主键为：<FrontID, SessionID, OrderRef> ")]
        public ConcurrentDictionary<Tuple<int, int, string>, OrderField> DictOrderField { get { return this._DictOrderField; } }
        [DisplayName("连接状态")]
        public ConnectionStatus ConnStatus { get { return _connStatus; } }
        [DisplayName("结算信息内容")]
        public string SettlementInfoContent { get { return _sbSettlementInfo.ToString(); } }
        [DisplayName("结算信息确认日期")]
        public string SettlementInfoConfirmDateTime { get { return _sbSettlementInfoConfirm.ToString(); } }
        //[DisplayName("查询队列")]
        //public ConcurrentQueue<Tuple<ReqCmdType, object>> QueryQueue { get { return _QueryQueue; } }
        [DisplayName("合并持仓字典，主键为：<InstrumentID, 买/卖, 投保> ")]
        public ConcurrentDictionary<Tuple<string, Direction, HedgeFlag>, Position> DictInvestorPosition { get { return this._DictInvestorPosition; } }
        [DisplayName("交易账户")]
        public TradeAccount TradeAccount { get { return this._taf; } }
        [DisplayName("错误信息")]
        public string ErrorMsg { get { return this._errorMsg; } }

        #endregion ==========属性==========

        #region ==========下单撤单函数==========

        /// <summary>
        /// 下普通单
        /// </summary>
        public void SendOrder(OrderField of)
        {//封装下单函数
            //OrderRef如果没有填（null），则为""
            string _orderRef = (of.OrderRef != null ? of.OrderRef : string.Empty);
            //当日有效(默认)
            TThostFtdcTimeConditionType _timeCondition = (of.OrderFieldInstance.TimeCondition == 0 ? TThostFtdcTimeConditionType.THOST_FTDC_TC_GFD : of.OrderFieldInstance.TimeCondition);
            //立即执行
            TThostFtdcContingentConditionType _contingentCondition = (of.OrderFieldInstance.ContingentCondition == 0 ? TThostFtdcContingentConditionType.THOST_FTDC_CC_Immediately : of.OrderFieldInstance.ContingentCondition);
            //是否强平
            TThostFtdcForceCloseReasonType _forceCloseReason = (of.OrderFieldInstance.ForceCloseReason == 0 ? TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_NotForceClose : of.OrderFieldInstance.ForceCloseReason);
            //报单价格类型：默认是限价单
            TThostFtdcOrderPriceTypeType _orderPriceTypeType = (of.OrderFieldInstance.OrderPriceType == 0 ? TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice : of.OrderFieldInstance.OrderPriceType);
            //任何数量
            TThostFtdcVolumeConditionType _volumeCondition = (of.OrderFieldInstance.VolumeCondition == 0 ? TThostFtdcVolumeConditionType.THOST_FTDC_VC_AV : of.OrderFieldInstance.VolumeCondition);
            //方向
            TThostFtdcDirectionType _directionType = (of.Direction == 0 ? TThostFtdcDirectionType.THOST_FTDC_D_Buy : of.OrderFieldInstance.Direction);

            //默认是投机单：1
            string _combHedgeFlag = (of.CombHedgeFlag == 0 ? "1" : ((int)of.CombHedgeFlag).ToString());

            base.ReqOrderInsert(
                BrokerID: this._broker,
                InvestorID: this._investor,
                InstrumentID: of.InstrumentID,
                OrderRef: _orderRef,
                CombHedgeFlag: _combHedgeFlag,
                CombOffsetFlag: ((int)of.CombOffsetFlag).ToString(),
                Direction: _directionType,                      //方向：默认为买
                VolumeTotalOriginal: of.VolumeTotalOriginal,
                ForceCloseReason: _forceCloseReason,
                ContingentCondition: _contingentCondition,
                VolumeCondition: _volumeCondition,              //数量条件：默认为任何数量
                LimitPrice: of.LimitPrice,
                IsSwapOrder: 0,
                MinVolume: 1,
                UserForceClose: of.UserForceClose,
                TimeCondition: _timeCondition,
                OrderPriceType: _orderPriceTypeType
                  );
        }

        /// <summary>
        /// 撤销普通单
        /// </summary>
        public void CancelOrder(OrderField of)
        {//OrderRef和InstrumentID
            base.ReqOrderAction(this._broker, this._investor, InstrumentID: of.InstrumentID,
                OrderRef: of.OrderRef,
                FrontID: of.FrontID,
                SessionID: of.SessionID,
                ActionFlag: TThostFtdcActionFlagType.THOST_FTDC_AF_Delete);
        }

        /// <summary>
        /// 下预埋单
        /// </summary>
        public void SendParkedOrder(OrderField of)
        {
            //OrderRef如果没有填（null），则为""
            string _orderRef = (of.OrderRef != null ? of.OrderRef : string.Empty);
            //当日有效(默认)
            TThostFtdcTimeConditionType _timeCondition = (of.OrderFieldInstance.TimeCondition == 0 ? TThostFtdcTimeConditionType.THOST_FTDC_TC_GFD : of.OrderFieldInstance.TimeCondition);
            //立即执行
            TThostFtdcContingentConditionType _contingentCondition = (of.OrderFieldInstance.ContingentCondition == 0 ? TThostFtdcContingentConditionType.THOST_FTDC_CC_Immediately : of.OrderFieldInstance.ContingentCondition);
            //是否强平
            TThostFtdcForceCloseReasonType _forceCloseReason = (of.OrderFieldInstance.ForceCloseReason == 0 ? TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_NotForceClose : of.OrderFieldInstance.ForceCloseReason);
            //报单价格类型：默认是限价单
            TThostFtdcOrderPriceTypeType _orderPriceTypeType = (of.OrderFieldInstance.OrderPriceType == 0 ? TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice : of.OrderFieldInstance.OrderPriceType);
            //任何数量
            TThostFtdcVolumeConditionType _volumeCondition = (of.OrderFieldInstance.VolumeCondition == 0 ? TThostFtdcVolumeConditionType.THOST_FTDC_VC_AV : of.OrderFieldInstance.VolumeCondition);
            //方向
            TThostFtdcDirectionType _directionType = (of.Direction == 0 ? TThostFtdcDirectionType.THOST_FTDC_D_Buy : of.OrderFieldInstance.Direction);

            //默认是投机单：1
            string _combHedgeFlag = (of.CombHedgeFlag == 0 ? "1" : ((int)of.CombHedgeFlag).ToString());

            base.ReqParkedOrderInsert(
                BrokerID: this._broker,
                InvestorID: this._investor,
                InstrumentID: of.InstrumentID,
                OrderRef: _orderRef,
                CombHedgeFlag: _combHedgeFlag,
                CombOffsetFlag: ((int)of.CombOffsetFlag).ToString(),
                Direction: _directionType,
                VolumeTotalOriginal: of.VolumeTotalOriginal,
                ForceCloseReason: _forceCloseReason,
                ContingentCondition: _contingentCondition,
                VolumeCondition: _volumeCondition,
                LimitPrice: of.LimitPrice,
                IsSwapOrder: 0,
                MinVolume: 1,
                UserForceClose: of.UserForceClose,
                TimeCondition: _timeCondition,
                OrderPriceType: _orderPriceTypeType
                  );
        }

        /// <summary>
        /// 预埋单撤单
        /// </summary>
        public void CancelParkedOrder(OrderField of)
        {
            //要根据ParkedOrderActionID来撤单
            base.ReqParkedOrderAction(this._broker, this._investor, InstrumentID: of.InstrumentID,
                OrderRef: of.OrderRef,
                FrontID: of.FrontID,
                SessionID: of.SessionID,
                ActionFlag: TThostFtdcActionFlagType.THOST_FTDC_AF_Delete
                );
        }

        #endregion ==========下单撤单函数==========

        #region ==========同步查询==========

        /// <summary>
        /// 查询所有在交易的合约
        /// </summary>
        /// <returns>字典，主键为InstrumentID</returns>
        public void SyncReqQryInstrumentField()
        {
            lock (this._reqLock)
            {
                while ((DateTime.Now - this._LastReqTime).TotalMilliseconds <= this._MinReqTimeInterval)
                {
                    Thread.Sleep(100);
                }
                this._isQryInstrumentCompleted = false;
                int result = (int)this.ReqQryInstrument();
                this._LastReqTime = DateTime.Now;
                while (!this._isQryInstrumentCompleted)
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 查询持仓细节
        /// </summary>
        /// <returns>字典，主键为OpenDate、TradeID </returns>
        public void SyncReqQryDetailPosition()
        {
            lock (this._reqLock)
            {
                while ((DateTime.Now - this._LastReqTime).TotalMilliseconds <= this._MinReqTimeInterval)
                {
                    Thread.Sleep(100);
                }
                this._isQryPositionDetailCompleted = false;
                int result = (int)this.ReqQryInvestorPositionDetail();
                this._LastReqTime = DateTime.Now;
                while (!this._isQryPositionDetailCompleted)
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 查询结算信息
        /// </summary>
        /// <param name="TradingDay">交易日</param>
        /// <returns></returns>
        public string SyncReqQrySettlementInfo(string TradingDay)
        {
            lock (this._reqLock)
            {
                while ((DateTime.Now - this._LastReqTime).TotalMilliseconds <= this._MinReqTimeInterval)
                {
                    Thread.Sleep(100);
                }
                this._isQrySettlementInfoCompleted = false;
                _sbSettlementInfo.Clear();
                this.ReqQrySettlementInfo(TradingDay: TradingDay);
                this._LastReqTime = DateTime.Now;
                while (!this._isQrySettlementInfoCompleted)
                {
                    Thread.Sleep(100);
                }
                return _sbSettlementInfo.ToString();
            }
        }

        /// <summary>
        /// 查询确认结算的日期
        /// </summary>
        /// <returns></returns>
        public string ReqQry_SettlementInfoConfirm()
        {
            lock (this._reqLock)
            {
                while ((DateTime.Now - this._LastReqTime).TotalMilliseconds <= this._MinReqTimeInterval)
                {
                    Thread.Sleep(100);
                }
                this._isQrySettlementInfoConfirmCompleted = false;
                this._sbSettlementInfoConfirm.Clear();
                this.ReqQrySettlementInfoConfirm();
                this._LastReqTime = DateTime.Now;
                while (!this._isQrySettlementInfoConfirmCompleted)
                {
                    Thread.Sleep(100);
                }
                return _sbSettlementInfoConfirm.ToString();
            }
        }

        /// <summary>
        /// 确认结算请求
        /// </summary>
        /// <returns></returns>
        public void SyncReqSettlementInfoConfirm()
        {
            lock (this._reqLock)
            {
                while ((DateTime.Now - this._LastReqTime).TotalMilliseconds <= this._MinReqTimeInterval)
                {
                    Thread.Sleep(100);
                }
                this._isSettlementInfoConfirmCompleted = false;
                this.ReqSettlementInfoConfirm(BrokerID:this._broker,InvestorID:this._investor);
                this._LastReqTime = DateTime.Now;
                while (!this._isSettlementInfoConfirmCompleted)
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 查询持仓
        /// </summary>
        /// <returns>字典，主键为InstrumentID、买/卖、投保</returns>
        public void SyncReqQryInvestorPosition()
        {
            lock (this._reqLock)
            {
                while ((DateTime.Now - this._LastReqTime).TotalMilliseconds <= this._MinReqTimeInterval)
                {
                    Thread.Sleep(100);
                }
                this._isQryPositionCompleted = false;
                int result =(int)this.ReqQryInvestorPosition();
                this._LastReqTime = DateTime.Now;
                while (!this._isQryPositionCompleted)
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 查询交易账户信息
        /// </summary>
        /// <returns></returns>
        public TradeAccount ReqQry_TradeAccount()
        {
            lock (this._reqLock)
            {
                while ((DateTime.Now - this._LastReqTime).TotalMilliseconds <= this._MinReqTimeInterval)
                {
                    Thread.Sleep(100);
                }
                this._isQryTradingAccountCompleted = false;
                int result = (int)this.ReqQryTradingAccount();
                this._LastReqTime = DateTime.Now;
                while (!this._isQryTradingAccountCompleted)
                {
                    Thread.Sleep(100);
                }
                return this._taf;
            }
        }

        #endregion ==========同步查询==========

        #region ==========连接登录登出==========

        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            if (this._connStatus == ConnectionStatus.Connected || this._connStatus == ConnectionStatus.Connecting)
            {
                return;
            }
            base.RegisterFront(this._addr);
            base.SubscribePrivateTopic(THOST_TE_RESUME_TYPE.THOST_TERT_RESTART);
            base.SubscribePublicTopic(THOST_TE_RESUME_TYPE.THOST_TERT_RESTART);
            int result = (int)this.Init();
            if (result != 0)
            {
                throw new Exception(result.ToString());
            }
            while (this._connStatus != ConnectionStatus.Connected)
            {
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            int result = (int)base.ReqUserLogin(BrokerID: this._broker, UserID: this._investor, Password: this._pwd, UserProductInfo: "@HaiFeng_WHWING");
            if (result != 0)
            {
                throw new Exception(result.ToString());
            }
            this._connStatus = ConnectionStatus.Logining;
            while (this._connStatus != ConnectionStatus.Logined)
            {
                Thread.Sleep(100);
            }
            //记录最后查询时间
            this._LastReqTime = DateTime.Now;
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void Logout()
        {
            //取消连接响应,避免重连后的再登录.
            this.ReqUserLogout();
            this.SetOnFrontDisconnected(null);
            this.SetOnFrontConnected(null);
            this.Release();
        }

        #endregion ==========登录登出==========
 
        #region ==========封装回调函数==========

        /// <summary>
        /// 错误回报
        /// </summary>
        /// <param name="pRspInfo"></param>
        /// <param name="nRequestID"></param>
        /// <param name="bIsLast"></param>
        private void CTradeApi_OnRspError(ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            this._errorMsg = string.Format("ErrorID:{0},ErrorMsg:{1}", pRspInfo.ErrorID, pRspInfo.ErrorMsg);
        }
        
        /// <summary>
        /// 前端连接回报
        /// </summary>
        private void CTradeApi_OnFrontConnected()
        {
            this._connStatus = ConnectionStatus.Connected;
            this._OnConnectionStatusChanged?.Invoke(this._connStatus);
        }

        /// <summary>
        /// 前端断连回报
        /// </summary>
        /// <param name="reason"></param>
        private void CTradeApi_OnFrontDisConnected(int reason)
        {
            this._connStatus = ConnectionStatus.Disconnected;
            this._OnConnectionStatusChanged?.Invoke(this._connStatus);
        }

        /// <summary>
        /// 登录回报
        /// </summary>
        /// <param name="pRspUserLogin"></param>
        /// <param name="pRspInfo"></param>
        /// <param name="nRequestID"></param>
        /// <param name="bIsLast"></param>
        private void CTradeApi_OnRspUserLogin(ref CThostFtdcRspUserLoginField pRspUserLogin, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast && pRspInfo.ErrorID == 0)
            {
                this._connStatus = ConnectionStatus.Logined;
                this._frontID = pRspUserLogin.FrontID;
                this._sessionID = pRspUserLogin.SessionID;
                this._maxOrderRef = int.Parse(pRspUserLogin.MaxOrderRef);
                this._OnConnectionStatusChanged?.Invoke(this._connStatus);
            }
        }

        /// <summary>
        /// 登出回报
        /// </summary>
        /// <param name="pUserLogout"></param>
        /// <param name="pRspInfo"></param>
        /// <param name="nRequestID"></param>
        /// <param name="bIsLast"></param>
        private void CTradeApi_OnRspUserLogout(ref CThostFtdcUserLogoutField pUserLogout, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast && pRspInfo.ErrorID == 0)
            {
                this._connStatus = ConnectionStatus.Logout;
                this._OnConnectionStatusChanged?.Invoke(this._connStatus);
            }

        }

        /// <summary>
        /// 查询合约回报
        /// </summary>
        private void CTradeApi_OnRspQryInstrument(ref CThostFtdcInstrumentField pInstrument, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (pRspInfo.ErrorID == 0)
            {
                CThostFtdcInstrumentField tmpInstrument = pInstrument;
                this._DictInstrumentField.AddOrUpdate(pInstrument.InstrumentID, new InstrumentField(pInstrument),
                    (k, v) => { v.CThostFtdcInstrumentFieldInstance = tmpInstrument; return v; });
            }
            _isQryInstrumentCompleted = bIsLast;
        }
        
        /// <summary>
        /// 查询结算信息回报
        /// </summary>
        private void CTradeApi_OnRspQrySettlementInfo(ref CThostFtdcSettlementInfoField pSettlementInfo, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (pRspInfo.ErrorID == 0)
            {
                _sbSettlementInfo.Append(pSettlementInfo.Content);
            }
            this._isQrySettlementInfoCompleted = bIsLast;
        }

        /// <summary>
        /// 查询持仓细节回报
        /// </summary>
        private void CTradeApi_OnRspQryInvestorPositionDetail(ref CThostFtdcInvestorPositionDetailField pInvestorPositionDetail, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (pRspInfo.ErrorID == 0 && !string.IsNullOrEmpty(pInvestorPositionDetail.TradeID))
            {
                CThostFtdcInvestorPositionDetailField tmpPositionDetail = pInvestorPositionDetail;
                var value = this._DictDetailPosition.AddOrUpdate(new Tuple<string, string>(pInvestorPositionDetail.OpenDate, pInvestorPositionDetail.TradeID),
                    new DetailPosition(pInvestorPositionDetail), (k, v) => { v.InvestorPositionDetailFieldInstance = tmpPositionDetail; return v; });
                this._OnDetailPosition?.Invoke(value);
            }
            if (bIsLast)
            {
                this._OnPositionChanged?.Invoke(this._DictDetailPosition);
            }
            this._isQryPositionDetailCompleted = bIsLast;
        }

        /// <summary>
        /// 查询结算确认回报
        /// </summary>
        private void CTradeApi_OnRspQrySettlementInfoConfirm(ref CThostFtdcSettlementInfoConfirmField pSettlementInfoConfirm, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (pRspInfo.ErrorID == 0)
            {
                _sbSettlementInfoConfirm.Append(string.Format("InvestorID:{0}; BrokerID:{1}; ConfirmDate:{2}; ConfrimTime:{3}",
                    pSettlementInfoConfirm.InvestorID, pSettlementInfoConfirm.BrokerID, pSettlementInfoConfirm.ConfirmDate, pSettlementInfoConfirm.ConfirmTime));
            }
            this._isQrySettlementInfoConfirmCompleted = bIsLast;
        }
        
        /// <summary>
        /// 确认结算回报
        /// </summary> 
        private void CTradeApi_OnRspSettlementInfoConfirm(ref CThostFtdcSettlementInfoConfirmField pSettlementInfoConfirm, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            this._isSettlementInfoConfirmCompleted = bIsLast;
        }

        /// <summary>
        /// 查询持仓回报
        /// </summary>
        private void CTradeApi_OnRspQryInvestorPosition(ref CThostFtdcInvestorPositionField pInvestorPosition, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (!string.IsNullOrEmpty(pInvestorPosition.InstrumentID))
            {
                var f = pInvestorPosition;
                if (f.Position > 0)    //Position字段即总仓位
                {
                    this._ListPositionField.Add(f);
                }
            }
            if (bIsLast)
            {
                this._isFirstTimeLogin = false;
                foreach (var g in _ListPositionField.GroupBy(p => p.InstrumentID + "_" + p.PosiDirection + "_" + p.HedgeFlag))
                {
                    var id = g.First();
                    var instrumentID = id.InstrumentID;
                    var direction = ConvertFunctions.TThostFtdcPosiDirectionType_To_Direction(id.PosiDirection);
                    var hedge = ConvertFunctions.TThostFtdcHedgeFlagType_To_HedgeFlag(id.HedgeFlag);
                    var key = new Tuple<string, Direction, HedgeFlag>(instrumentID, direction, hedge);
                    var pos = this._DictInvestorPosition.GetOrAdd(key, new Position(instrumentID, direction, hedge));

                    #region 持仓赋值
                    pos.TdPosition = g.Sum(n => n.TodayPosition);
                    pos.YdPosition = g.Sum(n => n.YdPosition);
                    if (pos.TotalPosition == 0)
                    {
                        this._DictInvestorPosition.TryRemove(key, out pos);
                        continue;
                    }
                    pos.BrokerID = id.BrokerID;
                    pos.InvestorID = id.InvestorID;
                    pos.PositionDateType = g.Max(n => ConvertFunctions.TThostFtdcPositionDateType_To_PositionDateType(n.PositionDate));//枚举类型值：今 = 1；昨 = 2；
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
                    pos.StrikeFrozen = g.Sum(n => n.StrikeFrozen);
                    pos.StrikeFrozenAmount = g.Sum(n => n.StrikeFrozenAmount);
                    pos.AbandonFrozen = g.Sum(n => n.AbandonFrozen);
                    InstrumentField instrument;
                    this._DictInstrumentField.TryGetValue(pos.InstrumentID, out instrument);
                    if (instrument != null)
                    {//OpenProfit是自设字段，表示开仓后到现在的总盈亏
                        pos.AvgOpenPrice = pos.OpenCost / pos.TotalPosition / instrument.VolumeMultiple;
                        if (pos.PosiDirection == Direction.Buy)
                        {
                            pos.OpenProfit = (pos.LastPrice - pos.AvgOpenPrice) * pos.TotalPosition * instrument.VolumeMultiple;
                        }
                        else if (pos.PosiDirection == Direction.Sell)
                        {
                            pos.OpenProfit = -(pos.LastPrice - pos.AvgOpenPrice) * pos.TotalPosition * instrument.VolumeMultiple;
                        }

                    }
                    #endregion

                    pos.Notify("");
                    //外部函数引发调用
                    this._OnPosition?.Invoke(pos);
                }
                this._OnPositionChanged?.Invoke(this._DictInvestorPosition);
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
                _ListPositionField.Clear();//清除,以便得到结果是重新添加
            }
            _isQryPositionCompleted = bIsLast;
        }

        /// <summary>
        /// 普通单成功下单后回报
        /// </summary>
        private void CTradeApi_OnRtnOrder(ref CThostFtdcOrderField pOrder)
        {
            CThostFtdcOrderField tmpOrder = pOrder;
            var value = this._DictOrderField.AddOrUpdate(new Tuple<int, int, string>(pOrder.FrontID, pOrder.SessionID, pOrder.OrderRef), new OrderField(pOrder),
                (k, v) => { v.OrderFieldInstance = tmpOrder; return v; });
            this._OnOrderField?.Invoke(value);
        }

        /// <summary>
        /// 成交回报
        /// </summary>
        private void CTradeApi_OnRtnTrade(ref CThostFtdcTradeField pTrade)
        {
            Trade t = new Trade(pTrade);
            var key = new Tuple<string, Direction>(t.TradeID, t.Direction);
            var value = this._DictTrade.AddOrUpdate(key, t, (k, v) => { v = t; return v; });
            //对于第一次登陆，由于先返回的是trade，所以不能调用UpdatePositionFromRtnTrade；只有当Position查询过后，才可以调用！
            if (this._isFirstTimeLogin)
            {
                this._OnTrade?.Invoke(value);
            }
            else
            { 
                UpdatePositionFromRtnTrade(new Trade(pTrade));
                this._OnTrade?.Invoke(value);
            }
        }

        /// <summary>
        /// 错单回报
        /// </summary>
        private void CTradeApi_OnErrRtnOrderInsert(ref CThostFtdcInputOrderField pInputOrder, ref CThostFtdcRspInfoField pRspInfo)
        {
            OrderField of = ConvertFunctions.InputOrderField_To_OrderField(pInputOrder, pRspInfo);
            this._OnErrorOrder?.Invoke(of);
        }
        
        /// <summary>
        /// 下普通单错误时回报
        /// </summary>
        private void CTradeApi_OnRspOrderInsert(ref CThostFtdcInputOrderField pInputOrder, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            OrderField of = ConvertFunctions.InputOrderField_To_OrderField(pInputOrder, pRspInfo);
            this._OnErrorOrder?.Invoke(of);
        }
        
        /// <summary>
        /// 撤普通单错误时回报
        /// </summary>
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

        /// <summary>
        /// 查询交易账户的回报
        /// </summary>
        /// <param name="pTradingAccount"></param>
        /// <param name="pRspInfo"></param>
        /// <param name="nRequestID"></param>
        /// <param name="bIsLast"></param>
        private void CTradeApi_OnRspQryTradingAccount(ref CThostFtdcTradingAccountField pTradingAccount, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            this._taf.CThostFtdcTradingAccountFieldInstance = pTradingAccount;
            if (bIsLast)
            {
                this._OnTradingAccount?.Invoke(this._taf);
            }
            this._isQryTradingAccountCompleted = bIsLast;
        }

        #endregion ==========封装回调函数==========


        /// <summary>
        /// 构造函数
        /// </summary>
        internal CTradeApi(string _investor, string _pwd, string _broker = "9999", string _addr = "tcp://180.168.146.187:10030", string pFile = "./ctp_dll/ctp_trade.dll") : base(pFile)
        {
            this._investor = _investor;
            this._pwd = _pwd;
            this._broker = _broker;
            this._addr = _addr;

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
            this.SetOnRspOrderAction((DeleOnRspOrderAction)AddDele(new DeleOnRspOrderAction(CTradeApi_OnRspOrderAction)));
            this.SetOnRspQryTradingAccount((DeleOnRspQryTradingAccount)AddDele(new DeleOnRspQryTradingAccount(CTradeApi_OnRspQryTradingAccount)));
        }

        /// <summary>
        /// 每当有报单成交时，则要更新持仓
        /// </summary>
        private void UpdatePositionFromRtnTrade(Trade tf)
        {
            lock (this._DictInvestorPosition) //Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>
            {
                //更新_dicPosition
                InstrumentField instrument;
                if (!this._DictInstrumentField.TryGetValue(tf.InstrumentID, out instrument))
                {
                    return;
                }
                Direction direction =tf.Direction;
                HedgeFlag hedge = tf.HedgeFlag;

                switch (tf.OffsetFlag)
                {
                    case Offset.Open:   //开仓成交
                        foreach (var kvp in this._DictInvestorPosition)
                        {//根据合约ID、方向、投保，找出是否有相同的键在字典内，如果有则更新
                            if (kvp.Key.Item1 == tf.InstrumentID && kvp.Key.Item2 == direction && kvp.Key.Item3 == hedge)
                            {
                                kvp.Value.TdPosition += tf.Volume;
                                kvp.Value.PositionCost += tf.Price * tf.Volume * instrument.VolumeMultiple;
                                kvp.Value.OpenCost += tf.Price * tf.Volume * instrument.VolumeMultiple;
                                kvp.Value.AvgOpenPrice = kvp.Value.OpenCost / kvp.Value.TotalPosition / instrument.VolumeMultiple;
                                this._OnPositionChanged?.Invoke(this._DictInvestorPosition);
                                return;
                            }
                        }
                        //没有持仓，则新建
                        Position pos = CreatePositionFromRtnTrade(tf);
                        this._DictInvestorPosition.TryAdd(new Tuple<string, Direction, HedgeFlag>(tf.InstrumentID, direction, hedge), pos);
                        this._OnPositionChanged?.Invoke(this._DictInvestorPosition);
                        break;

                    case Offset.CloseToday: //平今
                        foreach (var kvp in this._DictInvestorPosition)
                        {//根据合约ID、方向、投保，找出是否有相同的键在字典内，如果有则更新
                            if (kvp.Key.Item1 == tf.InstrumentID && kvp.Key.Item3 == hedge &&
                                ((kvp.Key.Item2 == Direction.Buy && direction == Direction.Sell)
                                || (kvp.Key.Item2 == Direction.Sell && direction == Direction.Buy))
                                )
                            {
                                kvp.Value.TdPosition -= tf.Volume;
                                kvp.Value.PositionCost -= tf.Price * tf.Volume * instrument.VolumeMultiple;
                                if (kvp.Value.TotalPosition == 0)
                                {
                                    Position tmp;
                                    if (this._DictInvestorPosition.TryRemove(kvp.Key, out tmp))
                                    {
                                        this._OnPositionChanged?.Invoke(this._DictInvestorPosition);
                                        return;
                                    }
                                }
                                kvp.Value.OpenCost = kvp.Value.OpenCost * kvp.Value.TotalPosition / (kvp.Value.TotalPosition + tf.Volume);
                                kvp.Value.AvgOpenPrice = kvp.Value.OpenCost / kvp.Value.TotalPosition / instrument.VolumeMultiple;
                                this._OnPositionChanged?.Invoke(this._DictInvestorPosition);
                                return;
                            }
                        }
                        this._OnPositionChanged?.Invoke(this._DictInvestorPosition);
                        break;

                    //平昨
                    case Offset.Close:
                    case Offset.CloseYesterday:
                        foreach (var kvp in this._DictInvestorPosition)
                        {//根据合约ID、方向、投保，找出是否有相同的键在字典内，如果有则更新
                            if (
                                kvp.Key.Item1 == tf.InstrumentID && kvp.Key.Item3 == hedge &&
                                ((kvp.Key.Item2 == Direction.Buy && direction == Direction.Sell)
                                || (kvp.Key.Item2 == Direction.Sell && direction == Direction.Buy))
                                )
                            {
                                kvp.Value.YdPosition -= tf.Volume;
                                kvp.Value.PositionCost -= tf.Price * tf.Volume * instrument.VolumeMultiple;
                                if (kvp.Value.TotalPosition == 0)
                                {
                                    Position tmp;
                                    if (this._DictInvestorPosition.TryRemove(kvp.Key, out tmp))
                                    {
                                        this._OnPositionChanged?.Invoke(this._DictInvestorPosition);
                                        return;
                                    }
                                }
                                kvp.Value.OpenCost = kvp.Value.OpenCost * kvp.Value.TotalPosition / (kvp.Value.TotalPosition + tf.Volume);
                                kvp.Value.AvgOpenPrice = kvp.Value.OpenCost / kvp.Value.TotalPosition / instrument.VolumeMultiple;
                                this._OnPositionChanged?.Invoke(this._DictInvestorPosition);
                                return;
                            }
                        }
                        this._OnPositionChanged?.Invoke(this._DictInvestorPosition);
                        break;
                    case Offset.ForceOff:
                        break;
                    case Offset.LocalForceClose:
                        break;
                    case Offset.ForceClose:
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 根据成交报单来构造持仓
        /// </summary>
        private Position CreatePositionFromRtnTrade(Trade tf)
        {
            Direction direction= tf.Direction;
            HedgeFlag hedge =  tf.HedgeFlag == 0 ? HedgeFlag.Speculation : tf.HedgeFlag;
            Position pos = new Position(tf.InstrumentID, direction, hedge);
            pos.AvgOpenPrice = tf.Price;
            pos.PositionDateType = PositionDateType.Today;
            pos.TdPosition = tf.Volume;
            pos.InvestorID = tf.InvestorID;
            InstrumentField instrument;
            if (this._DictInstrumentField.TryGetValue(tf.InstrumentID, out instrument))
            {
                pos.OpenCost = tf.Price * tf.Volume * instrument.VolumeMultiple;
                pos.PositionCost = pos.OpenCost;
            }
            pos.Notify("");
            return pos;
        }

    }
}
