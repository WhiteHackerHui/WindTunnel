using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using WT_Core;

namespace WT_CTP
{
    /// <summary>
    /// 继承自海风ctp_quote的CMdApi类型
    /// </summary>
    public sealed class CMdApi : HaiFeng.ctp_quote, INotifyPropertyChanged
    {
        #region ==========内部变量==========

        private int _nTimeLapse;
        private int _frontID, _sessionID;
        private string _investor, _pwd, _broker, _addr;
        private string _error;
        private ConnectionStatus _connStatus = ConnectionStatus.Disconnected;
        private readonly List<string> _ListSubscribedInstrumentIDs = new List<string>();
        private readonly ConcurrentDictionary<string, DepthMarketData> _DictDepthMarketData  = new ConcurrentDictionary<string, DepthMarketData>();
        private readonly List<Delegate> _ListDele = new List<Delegate>();
        private DeleOnRtnTick _OnRtnTick;
        private DeleOnRtnDMD _OnRtnDMD;
        private DeleOnConnectionStatusChanged _OnConnectionStatusChanged;

        #endregion ==========内部变量==========

        #region ==========属性==========

        [DisplayName("连接状态")]
        public ConnectionStatus ConnStatus { get { return _connStatus; } }
        [DisplayName("心跳测试下的延迟时间")]
        public int nTimeLapse { get { return this._nTimeLapse; } }
        [DisplayName("错误信息")]
        public string Error { get { return this._error; } }
        [DisplayName("订阅行情数据的合约代码集合")]
        public List<string> ListSubscribedInstrumentIDs{ get { return this._ListSubscribedInstrumentIDs; } }
        [DisplayName("前端主机ID")]
        public int FrontID { get { return this._frontID; } }
        [DisplayName("会话编号")]
        public int SessionID { get { return this._sessionID; } }
        [DisplayName("投资者代码")]
        public string Investor { get { return this._investor; } set { this._investor = value; } }
        [DisplayName("经纪公司代码")]
        public string Broker { get { return this._broker; } set { this._broker = value; } }
        [DisplayName("行情端IP地址")]
        public string Address { get { return this._addr; } set { this._addr = value; } }
        [DisplayName("密码")]
        public string Password { get { return this._pwd; } set { this._pwd = value; } }
        [DisplayName("行情数据字典")]
        public ConcurrentDictionary<string, DepthMarketData> DictDepthMarketData { get { return this._DictDepthMarketData; } }

        #endregion ==========属性==========

        #region ==========封装请求函数==========

        /// <summary>
        /// 订阅市场深度信息
        /// </summary>
        /// <param name="pInstrument">合约代码</param>
        /// <returns></returns>
        public int ReqSubscribeMarketData(params string[] pInstrument)
        {
            foreach (string instrumentID in pInstrument)
            {
                this._DictDepthMarketData.TryAdd(instrumentID, new DepthMarketData());
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

        /// <summary>
        /// 取消订阅市场深度信息
        /// </summary>
        /// <param name="pInstrument">合约代码</param>
        /// <returns></returns>
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

        #endregion ==========封装请求函数==========

        #region ==========事件==========
        public delegate void DeleOnConnectionStatusChanged(ConnectionStatus cs);
        public event DeleOnConnectionStatusChanged OnConnectionStatusChanged { add { this._OnConnectionStatusChanged += value; } remove { this._OnConnectionStatusChanged -= value; } }
        #endregion ==========事件==========


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
        /// 前端连接后的回报
        /// </summary>
        private void CMdApi_OnFrontConnected()
        {
            this._connStatus = ConnectionStatus.Connected;
            this._OnConnectionStatusChanged?.Invoke(this._connStatus);
        }
        
        /// <summary>
        /// 登录成功后的回报
        /// </summary>
        /// <param name="pRspUserLogin"></param>
        /// <param name="pRspInfo"></param>
        /// <param name="nRequestID"></param>
        /// <param name="bIsLast"></param>
        private void CMdApi_OnRspUserLogin(ref CThostFtdcRspUserLoginField pRspUserLogin, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast && pRspInfo.ErrorID == 0)
            {
                this._connStatus = ConnectionStatus.Logined;
                this._sessionID = pRspUserLogin.SessionID;
                this._frontID = pRspUserLogin.FrontID;
                this._OnConnectionStatusChanged?.Invoke(this._connStatus);
            }
        }

        /// <summary>
        /// 前端断连后的回报
        /// </summary>
        /// <param name="nReason"></param>
        private void CMdApi_OnFrontDisconnected(int nReason)
        {
            this._connStatus = ConnectionStatus.Disconnected;
            this._OnConnectionStatusChanged?.Invoke(this._connStatus);
        }

        /// <summary>
        /// 登出后的回报
        /// </summary>
        /// <param name="pUserLogout"></param>
        /// <param name="pRspInfo"></param>
        /// <param name="nRequestID"></param>
        /// <param name="bIsLast"></param>
        private void CMdApi_OnRspUserLogout(ref CThostFtdcUserLogoutField pUserLogout, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast && pRspInfo.ErrorID == 0)
            {
                this._connStatus = ConnectionStatus.Logout;
                this.Notify("ConnStatus");
            }
        }

        /// <summary>
        /// 回报心跳测试：延迟时间
        /// </summary>
        /// <param name="nTimeLapse"></param>
        private void CMdApi_OnHeartBeatWarning(int nTimeLapse)
        {
            this._nTimeLapse = nTimeLapse;
        }

        /// <summary>
        /// 返回错误后的回报
        /// </summary>
        /// <param name="pRspInfo"></param>
        /// <param name="nRequestID"></param>
        /// <param name="bIsLast"></param>
        private void CMdApi_OnRspError(ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast)
            {
                this._error = string.Format("DateTime: {0}; Error ID: {1}; Error Message: {2};", DateTime.Now, pRspInfo.ErrorID, pRspInfo.ErrorMsg);
                //throw new Exception(this.Error);
            }
        }

        /// <summary>
        /// 对于订阅市场深度信息查询的回报
        /// </summary>
        /// <param name="pSpecificInstrument"></param>
        /// <param name="pRspInfo"></param>
        /// <param name="nRequestID"></param>
        /// <param name="bIsLast"></param>
        private void CMdApi_OnRspSubMarketData(ref CThostFtdcSpecificInstrumentField pSpecificInstrument, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            //添加未重复的订阅过的合约代码
            if (pRspInfo.ErrorID == 0 && !this._ListSubscribedInstrumentIDs.Contains(pSpecificInstrument.InstrumentID))
            {
                this._ListSubscribedInstrumentIDs.Add(pSpecificInstrument.InstrumentID);
            }
        }

        /// <summary>
        /// 行情数据回调函数，同时引发_OnTick多播事件。
        /// </summary>
        /// <param name="pDepthMarketData"></param>
        private void CMdApi_OnRtnDepthMarketData(ref CThostFtdcDepthMarketDataField pDepthMarketData)
        {
            //添加到字典中
            string instrumentIDKey = pDepthMarketData.InstrumentID;
            DepthMarketData dmd = this._DictDepthMarketData[instrumentIDKey];
            dmd.CThostFtdcMarketDepthDataFieldInstance = pDepthMarketData;
            this._OnRtnDMD?.Invoke(dmd);
            
            //后台线程调用OnTick多播委托
            //Task.Run(() =>
            //{
                Tick tick = new Tick();
                tick.AskPrice = dmd.AskPrice1;
                tick.BidPrice = dmd.BidPrice1;
                tick.AskVolume = dmd.AskVolume1;
                tick.BidVolume = dmd.BidVolume1;
                tick.InstrumentID = dmd.InstrumentID;
                tick.LastPrice = dmd.LastPrice;
                tick.OpenInt = dmd.OpenInterest;
                tick.UpdateTime =dmd.TradingDay+ " "+ dmd.UpdateTime;
                tick.UpdateMillisec = dmd.UpdateMillisec;
                tick.Volume = dmd.Volume;
                _OnRtnTick?.Invoke(tick);
            //}
            //);

            //引发更新事件
            //this._queueDepthMarketDataField.Enqueue(pDepthMarketData);
            //Task.Run(() => TaskProcessor(this._queueDepthMarketDataField, _cts.Token));
        }

        #endregion ==========封装回调函数==========

        
        /// <summary>
        /// Tick回报
        /// </summary>
        public event DeleOnRtnTick OnRtnTick { add { this._OnRtnTick += value; } remove { this._OnRtnTick -= value; } }
        public delegate void DeleOnRtnTick(Tick tick);
        
        /// <summary>
        /// DMD回报
        /// </summary>
        public event DeleOnRtnDMD OnRtnDMD { add { this._OnRtnDMD += value; } remove { this._OnRtnDMD -= value; } }
        public delegate void DeleOnRtnDMD(DepthMarketData dmd);

        /// <summary>
        /// 属性改变通知事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 添加委托
        /// </summary>
        /// <param name="d">委托实例</param>
        /// <returns></returns>
        private Delegate AddDele(Delegate d) { _ListDele.Add(d); return d; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_investor"></param>
        /// <param name="_pwd"></param>
        /// <param name="_broker"></param>
        /// <param name="_addr"></param>
        /// <param name="pFile"></param>
        internal CMdApi(string _investor, string _pwd, string _broker = "9999", string _addr = "tcp://180.168.146.187:10031", string pFile = "./ctp_dll/ctp_quote.dll") : base(pFile)
        {
            //初始化
            this._investor = _investor;
            this._addr = _addr;
            this._pwd = _pwd;
            this._broker = _broker;
            //this._queueDepthMarketDataField = new ConcurrentQueue<CThostFtdcDepthMarketDataField>();

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

        /// <summary>
        /// 通知属性变更
        /// </summary>
        /// <param name="PropertyName"></param>
        private void Notify(string PropertyName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName)); }
 
    }


}
