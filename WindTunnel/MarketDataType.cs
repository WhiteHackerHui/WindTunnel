using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Threading;
using HaiFeng;
using WindTunnel;
using System.Runtime.InteropServices;

namespace WindTunnel
{
    //连接状态
    public enum ConnectionStatus : byte
    {
        Disconnected = 0,
        Connecting = 1,
        Connected = 2,
        Logining = 3,
        Logined = 4,
        Logout = 14
    }

    //行情数据类型
    public class DepthMarketData : INotifyPropertyChanged
    {
        private CThostFtdcDepthMarketDataField _dmdf;
        private string _futureCode;

        public double AskPrice1 { get { return _dmdf.AskPrice1; } }
        public double AskPrice2 { get { return _dmdf.AskPrice2; } }
        public double AskPrice3 { get { return _dmdf.AskPrice3; } }
        public double AskPrice4 { get { return _dmdf.AskPrice4; } }
        public double AskPrice5 { get { return _dmdf.AskPrice5; } }
        public int AskVolume1 { get { return _dmdf.AskVolume1; } }
        public int AskVolume2 { get { return _dmdf.AskVolume2; } }
        public int AskVolume3 { get { return _dmdf.AskVolume3; } }
        public int AskVolume4 { get { return _dmdf.AskVolume4; } }
        public int AskVolume5 { get { return _dmdf.AskVolume5; } }
        public double AveragePrice { get { return _dmdf.AveragePrice; } }
        public double BidPrice1 { get { return _dmdf.BidPrice1; } }
        public double BidPrice2 { get { return _dmdf.BidPrice2; } }
        public double BidPrice3 { get { return _dmdf.BidPrice3; } }
        public double BidPrice4 { get { return _dmdf.BidPrice4; } }
        public double BidPrice5 { get { return _dmdf.BidPrice5; } }
        public int BidVolume1 { get { return _dmdf.BidVolume1; } }
        public int BidVolume2 { get { return _dmdf.BidVolume2; } }
        public int BidVolume3 { get { return _dmdf.BidVolume3; } }
        public int BidVolume4 { get { return _dmdf.BidVolume4; } }
        public int BidVolume5 { get { return _dmdf.BidVolume5; } }
        public double ClosePrice { get { return _dmdf.ClosePrice; } }
        public double CurrDelta { get { return _dmdf.CurrDelta; } }
        public string ExchangeID { get { return _dmdf.ExchangeID; } }
        public string ExchangeInstID { get { return _dmdf.ExchangeID; } }
        public double HighestPrice { get { return _dmdf.HighestPrice; } }
        public string InstrumentID { get { return _dmdf.InstrumentID; } }
        public double LastPrice { get { return _dmdf.LastPrice; } }
        public double LowerLimitPrice { get { return _dmdf.LowerLimitPrice; } }
        public double LowestPrice { get { return _dmdf.LowestPrice; } }
        public double OpenInterest { get { return _dmdf.OpenInterest; } }
        public double OpenPrice { get { return _dmdf.OpenPrice; } }
        public double PreClosePrice { get { return _dmdf.PreClosePrice; } }
        public double PreDelta { get { return _dmdf.PreDelta; } }
        public double PreOpenInterest { get { return _dmdf.PreOpenInterest; } }
        public double PreSettlementPrice { get { return _dmdf.PreSettlementPrice; } }
        public double SettlementPrice { get { return _dmdf.SettlementPrice; } }
        public string TradingDay { get { return _dmdf.TradingDay; } }
        public double Turnover { get { return _dmdf.Turnover; } }
        public int UpdateMillisec { get { return _dmdf.UpdateMillisec; } }
        public string UpdateTime { get { return _dmdf.UpdateTime; } }
        public double UpperLimitPrice { get { return _dmdf.UpperLimitPrice; } }
        public int Volume { get { return _dmdf.Volume; } }
        public string FutureCode { get { return this._futureCode; } }
        public CThostFtdcDepthMarketDataField CThostFtdcMarketDepthDataFieldInstance
        {
            get { return _dmdf; }
            set {
                this._dmdf = value;
                if(this._futureCode == null)
                    this._futureCode = GetFutureCode(_dmdf.InstrumentID);
                Notify("");
            }
        }

        //构造函数
        public DepthMarketData(CThostFtdcDepthMarketDataField dmdf)
        {
            this.CThostFtdcMarketDepthDataFieldInstance = dmdf;
            this._futureCode = GetFutureCode(_dmdf.InstrumentID);
            Notify("");
        }
        public DepthMarketData()
        {
        }
        //属性改变通知事件
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string PropertyName){ PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName)); }
        private string GetFutureCode(string InstrumentID)
        {
            Regex r = new Regex(@"\d");
            int index = r.Match(InstrumentID).Index;
            return InstrumentID.Substring(0, index).ToLower();
        }

    }



////自定义的Trade，成交后自动返回的值！
//public class Trade : INotifyPropertyChanged, IEquatable<Trade>
//{
//    //内部变量
//    private TradeField _tradeField;

//    //封装属性
//    public string TradeID { get { return _tradeField.TradeID; } }   //成交编号！
//    public string InstrumentID { get { return _tradeField.InstrumentID; } } //成交合约
//    public double Price { get { return _tradeField.Price; } } //成交的价格！
//    public double Qty { get { return _tradeField.Qty; } } //成交数量
//    public OrderSide Side { get { return _tradeField.Side; } }   //方向
//    public double Commission { get { return _tradeField.Commission; } } //佣金
//    public OpenCloseType OpenClose { get { return _tradeField.OpenClose; } }
//    public string ExchangeID { get { return _tradeField.ExchangeID; } }
//    public HedgeFlagType HedgeFlag { get { return _tradeField.HedgeFlag; } }
//    public string AccountID { get { return _tradeField.AccountID; } }
//    public BusinessType Business { get { return _tradeField.Business; } }
//    public string ClientID { get { return _tradeField.ClientID; } }
//    public int Date { get { return _tradeField.Date; } }
//    public string ID { get { return _tradeField.ID; } }
//    public string InstrumentName { get { return Encoding.Default.GetString(_tradeField.InstrumentName).Replace("\0", ""); } }
//    public string PortfolioID1 { get { return _tradeField.PortfolioID1; } }
//    public string PortfolioID2 { get { return _tradeField.PortfolioID2; } }
//    public string PortfolioID3 { get { return _tradeField.PortfolioID3; } }
//    public int Time { get { return _tradeField.Time; } }
//    public string ReserveChar64 { get { return _tradeField.ReserveChar64; } }
//    public int ReserveInt32 { get { return _tradeField.ReserveInt32; } }
//    public string Symbol { get { return _tradeField.Symbol; } }
//    public string TradeDate { get { return DateTime.Now.ToString("yyyyMMdd"); } }

//    //构造函数
//    public Trade(TradeField tradeField) { this._tradeField = tradeField; }

//    //定义比较，以后ObservableCollection中就可以直接用Contains来判断是否包含TradeID的Trade！
//    bool IEquatable<Trade>.Equals(Trade other)
//    {
//        if (other == null) return false;
//        return (TradeID.Equals(other.TradeID));
//    }

//    //声明的是属性改变事件
//    public event PropertyChangedEventHandler PropertyChanged;
//    //定义更新属性：propName的函数过程
//    public void Notify(string propName)
//    {
//        if (PropertyChanged != null)
//        {
//            //引发事件
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
//        }
//    }

//    //当行情回调时，引发PropertyChanged事件
//    public TradeField TradeFieldInstance
//    {
//        get { return _tradeField; }
//        set
//        {
//            _tradeField = value;
//            //通知更新过的属性
//            Notify("");
//        }
//    }
//}


////自定义的Account，账户信息
//public class Account : INotifyPropertyChanged, IEquatable<Account>
//{
//    private AccountField _accountField;

//    public string AccountID { get { return _accountField.AccountID; } }
//    public double Available { get { return _accountField.Available; } }
//    public double Balance { get { return _accountField.Balance; } }
//    public double CashIn { get { return _accountField.CashIn; } }
//    public string ClientID { get { return _accountField.ClientID; } }
//    public double CloseProfit { get { return _accountField.CloseProfit; } }
//    public double Commission { get { return _accountField.Commission; } }
//    public string CurrencyID { get { return _accountField.CurrencyID; } }
//    public double CurrMargin { get { return _accountField.CurrMargin; } }
//    public double Deposit { get { return _accountField.Deposit; } }
//    public double FrozenCash { get { return _accountField.FrozenCash; } }
//    public double FrozenCommission { get { return _accountField.FrozenCommission; } }
//    public double FrozenStampTax { get { return _accountField.FrozenStampTax; } }
//    public double FrozenTransferFee { get { return _accountField.FrozenTransferFee; } }
//    public double PositionProfit { get { return _accountField.PositionProfit; } }
//    public double PreBalance { get { return _accountField.PreBalance; } }
//    public double StampTax { get { return _accountField.StampTax; } }
//    public double TransferFee { get { return _accountField.TransferFee; } }
//    public double Withdraw { get { return _accountField.Withdraw; } }
//    public double WithdrawQuota { get { return _accountField.WithdrawQuota; } }

//    public Account(AccountField accountField) { this._accountField = accountField; }

//    public event PropertyChangedEventHandler PropertyChanged;

//    public void Notify(string propName)
//    {
//        if (PropertyChanged != null)
//        {
//            //引发事件
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
//        }
//    }

//    public bool Equals(Account other)
//    {
//        if (other == null) return false;
//        return (AccountID.Equals(other.AccountID));
//    }

//    //当行情回调时，引发PropertyChanged事件
//    public AccountField AccountFieldInstance
//    {
//        get { return _accountField; }
//        set
//        {
//            _accountField = value;
//            //通知更新过的属性
//            Notify("");
//        }
//    }
//}

    public class CMdApi : ctp_quote
    {
        //内部变量
        private ConnectionStatus _connStatus;
        private readonly ConcurrentDictionary<string, DepthMarketData> _dicDepthMarketData;
        private readonly ConcurrentQueue<CThostFtdcDepthMarketDataField> _queueDepthMarketDataField;
        private CancellationTokenSource _cts;
        private int _processorNumber;
        private DeleOnTick _OnTick;
        private string _investor, _pwd, _broker, _addr;
        private int _frontID, _sessionID;
        private readonly List<Delegate> _listDele = new List<Delegate>();

        //构造函数
        public CMdApi(string _investor, string _pwd, string _broker = "9999", string _addr = "tcp://180.168.146.187:10031", string pFile = "./ctp_dll/ctp_quote.dll") : base(pFile)
        {
            //初始化
            this._investor = _investor;
            this._addr = _addr;
            this._pwd = _pwd;
            this._broker = _broker;
            this._connStatus = ConnectionStatus.Disconnected;
            this._dicDepthMarketData = new ConcurrentDictionary<string, DepthMarketData>();
            this._queueDepthMarketDataField = new ConcurrentQueue<CThostFtdcDepthMarketDataField>();
            this._cts = new CancellationTokenSource();
            this._processorNumber = 1000;

            //注册回调
            this.SetOnFrontConnected((DeleOnFrontConnected)AddDele(new DeleOnFrontConnected(CMdApi_OnFrontConnected)));
            this.SetOnFrontDisconnected((DeleOnFrontDisconnected)AddDele(new DeleOnFrontDisconnected(CMdApi_OnFrontDisconnected)));
            this.SetOnRspUserLogin((DeleOnRspUserLogin)AddDele(new DeleOnRspUserLogin(CMdApi_OnRspUserLogin)));
            this.SetOnRspUserLogout((DeleOnRspUserLogout)AddDele(new DeleOnRspUserLogout(CMdApi_OnRspUserLogout)));
            this.SetOnHeartBeatWarning((DeleOnHeartBeatWarning)AddDele(new DeleOnHeartBeatWarning(CMdApi_OnHeartBeatWarning)));
            this.SetOnRspError((DeleOnRspError)AddDele(new DeleOnRspError(CMdApi_OnRspError)));
            this.SetOnRspSubMarketData((DeleOnRspSubMarketData)AddDele(new DeleOnRspSubMarketData(CMdApi_OnRspSubMarketData))); 
            this.SetOnRtnDepthMarketData((DeleOnRtnDepthMarketData)AddDele(new DeleOnRtnDepthMarketData(CMdApi_OnRtnDepthMarketData)));

        }

        Delegate AddDele(Delegate d) { _listDele.Add(d); return d; }

        //事件
        public delegate void DeleOnTick(DepthMarketData dmd);
        public event DeleOnTick OnTick { add { this._OnTick += value; } remove { this._OnTick -= value; } }

        //属性
        public ConnectionStatus ConnStatus { get { return _connStatus; } }
        public int nTimeLapse { get; private set; }
        public string Error { get; private set; }
        public int ProcessorNumber { get { return _processorNumber; } set { _processorNumber = value; } }
        public CancellationTokenSource CTS { get { return _cts; } }
        public ConcurrentDictionary<string, DepthMarketData> DicDepthMarketData { get { return _dicDepthMarketData; } }
        public string Investor { get { return this._investor; } }
        public string Broker { get { return this._broker; } }
        public string Address { get { return this._addr; } }
        public string Password { get { return this._pwd; } }
        public int FrontID { get { return this._frontID; } }
        public int SessionID { get { return this._sessionID; } }

        //查询函数
        public async Task AsyncConnect(CancellationToken CancelToken)
        {
            if (this._connStatus == ConnectionStatus.Connected || this._connStatus == ConnectionStatus.Connecting)
            {
                return;
            }
            this.RegisterFront(this._addr);

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
            int result = (int)this.ReqUserLogin(this._broker, this._investor, this._pwd, "@HaiFeng_WHWING");
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
        public int ReqSubscribeMarketData(params string[] pInstrument)
        {
            foreach (string instrumentID in pInstrument)
            {
                this._dicDepthMarketData.TryAdd(instrumentID, new DepthMarketData());
            }
            int size = Marshal.SizeOf(typeof(IntPtr));
            IntPtr insts = Marshal.AllocHGlobal(size * pInstrument.Length);
            var tmp = insts;
            for (int i = 0; i < pInstrument.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(Marshal.StringToHGlobalAnsi(pInstrument[i]), tmp, false);
            }
            return (int)this.SubscribeMarketData(insts, pInstrument.Length);
        }
        public int ReqUnSubscribeMarketData(params string[] pInstrument)
        {
            int size = Marshal.SizeOf(typeof(IntPtr));
            IntPtr insts = Marshal.AllocHGlobal(size * pInstrument.Length);
            var tmp = insts;
            for (int i = 0; i < pInstrument.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(pInstrument[i], tmp, false);
            }
            return (int)this.UnSubscribeMarketData(insts, pInstrument.Length);
        }
        public void ReqUserLogout()
        {
            this._connStatus = ConnectionStatus.Logout;
            //取消连接响应,避免重连后的再登录.
            this.SetOnFrontDisconnected(null);
            this.SetOnFrontConnected(null);
            this.Release();
        }

        #region 封装回调函数
        private void CMdApi_OnFrontConnected()
        {
            this._connStatus = ConnectionStatus.Connected;
        }
        private void CMdApi_OnRspUserLogin(ref CThostFtdcRspUserLoginField pRspUserLogin, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast && pRspInfo.ErrorID == 0)
            {
                _connStatus = ConnectionStatus.Logined;
                this._sessionID = pRspUserLogin.SessionID;
                this._frontID = pRspUserLogin.FrontID;
            }
        }
        private void CMdApi_OnFrontDisconnected(int nReason)
        {
            this._connStatus = ConnectionStatus.Disconnected;
        }
        private void CMdApi_OnRspUserLogout(ref CThostFtdcUserLogoutField pUserLogout, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast && pRspInfo.ErrorID == 0)
            {
                this._connStatus = ConnectionStatus.Logout;
            }
        }
        private void CMdApi_OnHeartBeatWarning(int nTimeLapse)
        {
            this.nTimeLapse = nTimeLapse;
        }
        private void CMdApi_OnRspError(ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (pRspInfo.ErrorID != 0)
            {
                this.Error = string.Format("Error ID: {0}; Error Message: {1};", pRspInfo.ErrorID, pRspInfo.ErrorMsg);
                throw new Exception(this.Error);
            }
        }
        private void CMdApi_OnRspSubMarketData(ref CThostFtdcSpecificInstrumentField pSpecificInstrument, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast)
            {//【注】由于效率问题，我最后决定不使用固定数量的处理任务线程!
             //使用默认数量处理任务来处理行情数据队列。 
             //Task[] taskProcessors = new Task[_processorNumber];
             //for (int i = 0; i < _processorNumber; i++)
             //{
             //    string processorId = i.ToString();
             //    taskProcessors[i] = Task.Run(() => TaskProcessor(this._queueDepthMarketDataField, _cts.Token));
             //}
            }
        }
        private void CMdApi_OnRtnDepthMarketData(ref CThostFtdcDepthMarketDataField pDepthMarketData)
        {
            CThostFtdcDepthMarketDataField dmdf= pDepthMarketData;
            this._dicDepthMarketData[dmdf.InstrumentID].CThostFtdcMarketDepthDataFieldInstance = dmdf;
            Task.Run(() => _OnTick?.Invoke(this._dicDepthMarketData[dmdf.InstrumentID]));
             //引发更新事件
            //this._queueDepthMarketDataField.Enqueue(pDepthMarketData);
            //Task.Run(() => TaskProcessor(this._queueDepthMarketDataField, _cts.Token));
        }
        #endregion 封装回调函数

        //处理行情数据（Field）的任务
        private void TaskProcessor(ConcurrentQueue<CThostFtdcDepthMarketDataField> queue, CancellationToken token)
        {
            CThostFtdcDepthMarketDataField dmdf;
            while (!token.IsCancellationRequested)
            {
                if (queue.TryDequeue(out dmdf))
                {
                    this._dicDepthMarketData[dmdf.InstrumentID].CThostFtdcMarketDepthDataFieldInstance = dmdf;
                    _OnTick?.Invoke(this._dicDepthMarketData[dmdf.InstrumentID]); //引发更新事件
                }
                else
                    break;
            }
        }
    }
}

