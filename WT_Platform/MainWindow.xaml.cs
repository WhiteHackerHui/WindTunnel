using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WT_Core;
using WT_CTP;
using WT_Strategy;
using Numeric = System.Double;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using WT_UI;

namespace WT_Platform
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ==========内部变量==========

        //API
        private readonly CMdApi _quoteApi = CTPFactory.CreateCMdApiInstance();
        private readonly CTradeApi _tradeApi = CTPFactory.CreateCTradeApiInstance();
        //字典
        private readonly ConcurrentDictionary<string, Strategy> _DictStrategies = new ConcurrentDictionary<string, Strategy>();
        private readonly ConcurrentDictionary<string, List<Tuple<string, Direction, HedgeFlag>>> _DictInstrumentIDPositions = new ConcurrentDictionary<string, List<Tuple<string, Direction, HedgeFlag>>>();
        //OC
        private ObservableCollection<DepthMarketData> _OcAllDepthMarketData = new ObservableCollection<DepthMarketData>();
        private ObservableCollection<DepthMarketData> _OcMainDepthMarketData = new ObservableCollection<DepthMarketData>();
        private readonly ObservableCollection<TradeAccount> _OcAllTradeAccount = new ObservableCollection<TradeAccount>();
        private readonly ObservableCollection<Position> _OcAllPosition = new ObservableCollection<Position>();
        private readonly ObservableCollection<Trade> _OcAllTradeField = new ObservableCollection<Trade>();
        private readonly ObservableCollection<DetailPosition> _OcAllDetailPositionField = new ObservableCollection<DetailPosition>();
        private readonly ObservableCollection<OrderField> _OcAllOrderField = new ObservableCollection<OrderField>();
        private readonly ObservableCollection<OrderField> _OcNoTradeOrderField = new ObservableCollection<OrderField>();
        private readonly ObservableCollection<OrderField> _OcCanceledOrderField = new ObservableCollection<OrderField>();
        private readonly ObservableCollection<OrderField> _OcErrorOrderField = new ObservableCollection<OrderField>();
        internal readonly ObservableCollection<PriceConditionOrderField> _OcAllPriceConditionOrderField = new ObservableCollection<PriceConditionOrderField>();
        private readonly ObservableCollection<OrderItem> _OcAllOrderItem = new ObservableCollection<OrderItem>();
        private readonly ObservableCollection<Strategy> _OcAllStrategy = new ObservableCollection<Strategy>();

        //排序
        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirection;

        //锁
        private readonly object _lockAllDepthMarketData = new object();
        private readonly object _lockAllTradeAccount = new object();
        private readonly object _lockAllPosition = new object();
        private readonly object _lockAllTradeField = new object();
        private readonly object _lockAllDetailPositionField = new object();
        private readonly object _lockAllOrderField = new object();
        private readonly object _lockNoTradeOrderField = new object();
        private readonly object _lockCanceledOrderField = new object();
        private readonly object _lockErrorOrderField = new object();

        //Redis数据库
        private readonly string _dbHost = "localhost";
        private readonly int _dbPort = 27017;
        private readonly string _dbName = "CTPDB";
        private readonly MongoClient _dbClient;
        private readonly IMongoDatabase _dbDatabase;
        private readonly IMongoCollection<Tick> _dbCollectionTick;
        private int _dbRecords = 0; //写入到数据库的tick数量

        //计时器
        private readonly Stopwatch _stopwatch = new Stopwatch();

        #endregion ==========内部变量==========

        #region ==========属性==========

        public CTradeApi TradeAPI { get { return this._tradeApi; } }
        public CMdApi QuotedAPI { get { return this._quoteApi; } }

        #endregion ==========属性==========


        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //绑定LV源至OC
            this.lvAllMarketData.ItemsSource = this._OcAllDepthMarketData;
            BindingOperations.EnableCollectionSynchronization(this._OcAllDepthMarketData, new object());//更新线程用
            this.lvAllTradingAccount.ItemsSource = this._OcAllTradeAccount;
            BindingOperations.EnableCollectionSynchronization(this._OcAllTradeAccount, new object());
            this.lvAllPosition.ItemsSource = this._OcAllPosition;
            BindingOperations.EnableCollectionSynchronization(this._OcAllPosition, new object());
            this.lvAllTradeField.ItemsSource = this._OcAllTradeField;
            BindingOperations.EnableCollectionSynchronization(this._OcAllTradeField, new object());
            this.lvAllDetailPositionField.ItemsSource = this._OcAllDetailPositionField;
            BindingOperations.EnableCollectionSynchronization(this._OcAllDetailPositionField, new object());
            this.lvAllOrderField.ItemsSource = this._OcAllOrderField;
            BindingOperations.EnableCollectionSynchronization(this._OcAllOrderField, new object());
            this.lvNoTradeOrder.ItemsSource = this._OcNoTradeOrderField;
            BindingOperations.EnableCollectionSynchronization(this._OcNoTradeOrderField, new object());
            this.lvCanceledOrder.ItemsSource = this._OcCanceledOrderField;
            BindingOperations.EnableCollectionSynchronization(this._OcCanceledOrderField, new object());
            this.lvErrorOrder.ItemsSource = this._OcErrorOrderField;
            BindingOperations.EnableCollectionSynchronization(this._OcErrorOrderField, new object());
            this.lvAllPriceConditionOrderField.ItemsSource = this._OcAllPriceConditionOrderField;
            BindingOperations.EnableCollectionSynchronization(this._OcAllPriceConditionOrderField, new object());
            this.lvAllOrderItem.ItemsSource = this._OcAllOrderItem;
            BindingOperations.EnableCollectionSynchronization(this._OcAllOrderItem, new object());
            this.lvAllStrategy.ItemsSource = this._OcAllStrategy;
            BindingOperations.EnableCollectionSynchronization(this._OcAllStrategy, new object());

            //绑定回报函数
            this._quoteApi.OnRtnDMD += this.Platform_OnPositionProfitChanged;
            this._quoteApi.OnConnectionStatusChanged += this.Platform_OnQuoteApiConnectionStatusChanged;
            this._tradeApi.OnTrade += this.Platform_OnTrade;
            this._tradeApi.OnDetailPosition += this.Platform_OnDetailPosition;
            this._tradeApi.OnPosition += this.Platform_OnPosition;
            this._tradeApi.OnOrderField += this.Platform_OnOrderField;
            this._tradeApi.OnErrorOrder += this.Platform_OnErrorOrder;
            this._tradeApi.OnTradeAccount += this.Platform_OnRtnTradeAccount;
            this._tradeApi.OnPositionChanged += this.Platform_OnPositionChanged;
            this._tradeApi.OnConnectionStatusChanged += this.Platform_OnTradeApiConnectionStatusChanged;
            this._quoteApi.OnRtnTick += this.Platform_OnRtnTick;

            //this._trdApi.OnUpdate += Body.Body_OnUpdate;

            //数据库准备写入Tick数据
            this._dbClient = new MongoClient($"mongodb://{this._dbHost}:{this._dbPort}");
            this._dbDatabase = this._dbClient.GetDatabase(this._dbName); //获得指定的数据库实例
            this._dbCollectionTick = this._dbDatabase.GetCollection<Tick>("Tick");

            ////默认下载Tick
            //this.tgbDownloadTick.IsChecked = true;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~MainWindow()
        {
        }

        #region ==========平台绑定回报函数==========

        /// <summary>
        /// 深度行情数据回报，更新Position中的持仓损益
        /// </summary>
        /// <param name="dmd"></param>
        private void Platform_OnPositionProfitChanged(DepthMarketData dmd)
        {
            if (!this._DictInstrumentIDPositions.IsEmpty && _DictInstrumentIDPositions.ContainsKey(dmd.InstrumentID))
            {
                //字典中含有dmd相对应的InstrumentID
                List<Tuple<string, Direction, HedgeFlag>> value;
                this._DictInstrumentIDPositions.TryGetValue(dmd.InstrumentID, out value);
                foreach (var item in value)
                {
                    var key = item;
                    var instrumentID = key.Item1;
                    Position pos;
                    if (!this._tradeApi.DictInvestorPosition.TryGetValue(key, out pos))
                    {
                        continue;
                    }
                    var lastPrice = this._quoteApi.DictDepthMarketData[key.Item1].LastPrice;
                    var mult = this._tradeApi.DictInstrumentField[dmd.InstrumentID].VolumeMultiple;
                    pos.LastPrice = lastPrice;
                    if (key.Item2 == Direction.Buy)
                    {
                        pos.OpenProfit = (lastPrice - pos.AvgOpenPrice) * pos.TotalPosition * mult;
                        pos.PositionProfit = lastPrice * pos.TotalPosition * mult - pos.PositionCost;
                    }
                    else
                    {
                        pos.OpenProfit = -1 * (lastPrice - pos.AvgOpenPrice) * pos.TotalPosition * mult;
                        pos.PositionProfit = -1 * (lastPrice * pos.TotalPosition * mult - pos.PositionCost);
                    }
                    pos.Notify("");
                }
            }
        }

        /// <summary>
        /// 交易账户信息回报
        /// </summary>
        private void Platform_OnRtnTradeAccount(TradeAccount taf)
        {
            lock (this._lockAllTradeAccount)
            {
                int index = _OcAllTradeAccount.IndexOf(taf);
                if (index == -1)
                {
                    _OcAllTradeAccount.Insert(0, taf);
                }
                else
                {
                    _OcAllTradeAccount[index].CThostFtdcTradingAccountFieldInstance = taf.CThostFtdcTradingAccountFieldInstance;
                }
            }
        }

        /// <summary>
        /// 合并持仓回报
        /// </summary>
        private void Platform_OnPosition(Position pos)
        {
            lock (this._lockAllPosition)
            {
                int index = this._OcAllPosition.IndexOf(pos);
                if (index == -1)
                {
                    _OcAllPosition.Insert(0, pos);
                }
                else
                {
                    _OcAllPosition[index] = pos;
                }
                foreach (var key in this._tradeApi.DictInvestorPosition.Keys)
                {//更新Dic_InstrumentID_PositionKeys
                    var instrumentID = key.Item1;
                    var direction = key.Item2;
                    var hedge = key.Item3;
                    var value = this._DictInstrumentIDPositions.GetOrAdd(key.Item1, k => new List<Tuple<string, Direction, HedgeFlag>>());
                    value.Add(key);
                }
            }
        }

        /// <summary>
        /// 细节持仓回报
        /// </summary>
        private void Platform_OnDetailPosition(DetailPosition dpf)
        {
            lock (this._lockAllDetailPositionField)
            {
                int index = this._OcAllDetailPositionField.IndexOf(dpf);
                if (index == -1)
                {
                    this._OcAllDetailPositionField.Insert(0, dpf);
                }
                else
                {
                    this._OcAllDetailPositionField[index].InvestorPositionDetailFieldInstance = dpf.InvestorPositionDetailFieldInstance;
                }
            }
        }

        /// <summary>
        /// 交易回报
        /// </summary>
        private void Platform_OnTrade(Trade tf)
        {
            lock (this._lockAllTradeField)
            {
                int index = this._OcAllTradeField.IndexOf(tf);
                if (index == -1)
                {
                    this._OcAllTradeField.Insert(0, tf);
                }
                else
                {
                    this._OcAllTradeField[index].CThostFtdcTradeFieldInstance = tf.CThostFtdcTradeFieldInstance;
                }
            }

        }

        /// <summary>
        /// 报单回报
        /// </summary>
        private void Platform_OnOrderField(OrderField of)
        {
            //所有报单
            lock (this._lockAllOrderField)
            {
                int index = this._OcAllOrderField.IndexOf(of);
                if (index == -1)
                {
                    this._OcAllOrderField.Insert(0, of);
                }
                else
                {
                    this._OcAllOrderField[index].OrderFieldInstance = of.OrderFieldInstance;
                }
            }
            //未成交报单
            lock (this._lockNoTradeOrderField)
            {
                int index = this._OcNoTradeOrderField.IndexOf(of);
                if (index == -1 && (of.OrderStatus == OrderStatus.NoTradeNotQueueing || of.OrderStatus == OrderStatus.NoTradeQueueing))
                {
                    this._OcNoTradeOrderField.Insert(0, of);
                }
                else if (index >= 0 && (of.OrderStatus != OrderStatus.NoTradeNotQueueing && of.OrderStatus != OrderStatus.NoTradeQueueing))
                {
                    this._OcNoTradeOrderField.RemoveAt(index);
                }
            }
            //撤销的报单
            lock (this._lockCanceledOrderField)
            {
                int index = this._OcCanceledOrderField.IndexOf(of);
                if (index == -1 && (of.OrderStatus == OrderStatus.Canceled || of.OrderStatus == OrderStatus.Unknown))
                {
                    this._OcCanceledOrderField.Insert(0, of);
                }
                else if (index >= 0 && (of.OrderStatus != OrderStatus.Canceled && of.OrderStatus != OrderStatus.Unknown))
                {
                    this._OcCanceledOrderField.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// 错单回报
        /// </summary>
        private void Platform_OnErrorOrder(OrderField of)
        {
            lock (this._lockErrorOrderField)
            {
                this._OcErrorOrderField.Insert(0, of);
            }
        }

        /// <summary>
        /// TradeAPI OnUpdate回调：新增的情况 Position、DetialPosition
        /// </summary>
        private void Platform_OnPositionChanged(object obj)
        {
            lock (obj)
            {
                if (obj.GetType() == typeof(ConcurrentDictionary<Tuple<string, Direction, HedgeFlag>, Position>))
                {//更新持仓
                    this._OcAllPosition.Clear();
                    var dic = obj as ConcurrentDictionary<Tuple<string, Direction, HedgeFlag>, Position>;
                    foreach (var kvp in dic)
                    {
                        this._OcAllPosition.Add(kvp.Value);
                    }
                    foreach (var key in this._tradeApi.DictInvestorPosition.Keys)
                    {//更新Dic_InstrumentID_PositionKeys
                        var instrumentID = key.Item1;
                        var direction = key.Item2;
                        var hedge = key.Item3;
                        var value = this._DictInstrumentIDPositions.GetOrAdd(key.Item1, k => new List<Tuple<string, Direction, HedgeFlag>>());
                        value.Add(key);
                    }
                }
                else if (obj.GetType() == typeof(ConcurrentDictionary<Tuple<string, string>, DetailPosition>))
                {//更新持仓细节
                    this._OcAllDetailPositionField.Clear();
                    var dic = obj as ConcurrentDictionary<Tuple<string, string>, DetailPosition>;
                    foreach (var kvp in dic)
                    {
                        this._OcAllDetailPositionField.Add(kvp.Value);
                    }
                }

            }

        }

        /// <summary>
        /// Strategy策略下单
        /// </summary>
        /// <param name="pDir"></param>
        /// <param name="pOffset"></param>
        /// <param name="pLots"></param>
        /// <param name="pPrice"></param>
        /// <param name="pRemark"></param>
        private void Platform_OnOrderItem(object pStrategyData, OrderItemArgs pOrderItemArgs)
        {
            OrderItem oi = pOrderItemArgs.OrderItem;
            string pInstrumentID = oi.InstrumentID;
            Direction pDir = oi.Dir;
            Offset pOffset = oi.Offset;
            int pLots = oi.Lots;
            Numeric pPrice = oi.Price;

            OrderField of = new OrderField();
            of.InstrumentID = pInstrumentID;
            of.Direction = pDir;
            of.VolumeTotalOriginal = pLots;
            of.LimitPrice = pPrice;
            of.OrderPriceType = OrderPriceType.LimitPrice;  //限价单
            of.CombOffsetFlag = pOffset;
            //如果是平仓的话，默认是平昨，但是如果没有平昨仓位，则改为平今。
            if (pOffset == Offset.Close)
            {
                List<Tuple<string, Direction, HedgeFlag>> tpList;
                if(this._DictInstrumentIDPositions.TryGetValue(pInstrumentID, out tpList))
                {
                    foreach (var tp in tpList)
                    {
                        Direction tpDir = tp.Item2;
                        //找出与报单Direction相反的Position
                        if (pDir==Direction.Buy && tpDir==Direction.Sell || pDir==Direction.Sell && tpDir == Direction.Buy)
                        {
                            Position pos = this._tradeApi.DictInvestorPosition[tp];
                            //如果昨仓为0，则平今
                            if (0 == pos.YdPosition)
                            {
                                of.CombOffsetFlag = Offset.CloseToday;
                            }
                        }
                    }
                }
            }
            //策略下单
            this._tradeApi.SendOrder(of);
            //添加OrderItem
            this._OcAllOrderItem.Add(oi);
        }

        /// <summary>
        /// 交易端连接状态变化
        /// </summary>
        private void Platform_OnTradeApiConnectionStatusChanged(ConnectionStatus cs)
        {
            Dispatcher.Invoke(()=> this.lblTradeAPIConnectionStatus.Content = cs.ToString());
        }

        /// <summary>
        /// 行情端连接状态变化
        /// </summary>
        private void Platform_OnQuoteApiConnectionStatusChanged(ConnectionStatus cs)
        {
            Dispatcher.Invoke(()=> this.lblQuoteAPIConnectionStatus.Content = cs.ToString());
        }


        #endregion ==========平台绑定回报函数==========

        #region ==========策略回调更新函数==========

        /// <summary>
        /// 行情数据回调，更新各个载入的Strategy；同时将Tick数据存入数据库中。
        /// </summary>
        private void Platform_OnRtnTick(Tick curTick)
        {
            //通过更新StrategyData的Bar，从而更新策略
            foreach (Strategy strategy in _DictStrategies.Values)
            {
                if (curTick.InstrumentID == strategy.InstrumentID)
                {
                    foreach (BarSeries bars in strategy.BarSerieses)
                    {
                        bars.OnRtnTick(curTick);
                    }
                    //strategy.UpdateStrategy();
                }
            }
        }

        #endregion ==========策略回调更新函数==========

        
        #region ==========菜单函数==========

        /// <summary>
        /// 调用登录窗口，并绑定All DMD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void btnSystem_Login_Click(object sender, RoutedEventArgs e)
        {
            //启动登录窗口
            LoginWindow lw = new LoginWindow(this, this._quoteApi, this._tradeApi);
            lw.ShowDialog();
            //行情数据初始化
            if (this._quoteApi.ConnStatus == ConnectionStatus.Logined && this._tradeApi.ConnStatus == ConnectionStatus.Logined)
            {
                //行情OC
                this._OcAllDepthMarketData = await Task.Run(() => GetOcAllMarketDepthData());
                this.lvAllMarketData.ItemsSource = this._OcAllDepthMarketData;
            }
        }

        /// <summary>
        /// 调用下单板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutil_OrderPanel_Click(object sender, RoutedEventArgs e)
        {
            OrderPanelWindow opw = new OrderPanelWindow(this);
            opw.ShowDialog();
        }

        /// <summary>
        /// 下载Tick数据：True
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tgbDownloadTick_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)this.tgbDownloadTick.IsChecked && this._quoteApi != null)
            {
                this._quoteApi.OnRtnTick += this.InsertTick;
            }
        }

        /// <summary>
        /// 下载Tick数据：False
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tgbDownloadTick_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)this.tgbDownloadTick.IsChecked && this._quoteApi != null)
            {
                this._quoteApi.OnRtnTick -= this.InsertTick;
            }
        }

        #endregion ==========菜单函数==========

        /// <summary>
        /// 切换至主力合约视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void tcMarketDepthData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (e.Source is TabControl)
            {
                TabControl tabControl = (TabControl)e.Source;
                var selectedTabItem = (TabItem)tabControl.SelectedItem;
                //更新主力合约
                if (selectedTabItem.Name == "tiMainDepthMarketData")
                {
                    this._OcMainDepthMarketData = await Task.Run(() => GetOcMainMarketDepthData(this._OcAllDepthMarketData));
                    lvMainMarketData.ItemsSource = this._OcMainDepthMarketData;//更新数据源        
                }
            }
        }

        /// <summary>
        /// 点击列标题后排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                ListView lv = sender as ListView;
                var headerClicked = e.OriginalSource as GridViewColumnHeader;
                ListSortDirection direction = 0;
                if (lv == null || headerClicked == null || headerClicked.Role == GridViewColumnHeaderRole.Padding)
                    return;
                if (headerClicked != _lastHeaderClicked)
                {//如果选中的列不是上次选中的列，则direction设置为Asc
                    direction = ListSortDirection.Ascending;
                }
                else
                {//对于同一个选中列第二次排序
                    if (_lastDirection == ListSortDirection.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                    }
                }
                //排序
                string header = headerClicked.Column.Header as string;
                Sort(header, direction, lv);
                _lastHeaderClicked = headerClicked;
                _lastDirection = direction;
            }
        }

        /// <summary>
        /// 查询交易账户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void btnQryTradingAccount_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => this._tradeApi.ReqQry_TradeAccount());
        }

        /// <summary>
        /// 查询持仓
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void btnQryInvestorPosition_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => this._tradeApi.SyncReqQryInvestorPosition());
        }


        #region ==========功能函数==========

        /// <summary>
        /// 初始化 OcAllMarketDepthData
        /// </summary>
        private ObservableCollection<DepthMarketData> GetOcAllMarketDepthData()
        {
            lock (this._lockAllDepthMarketData)
            {
                ObservableCollection<DepthMarketData> oc = new ObservableCollection<DepthMarketData>();
                oc.Clear();

                foreach (var kvp in this._quoteApi.DictDepthMarketData)
                {
                    InstrumentField _instrumentField;
                    if (this._tradeApi.DictInstrumentField.TryGetValue(kvp.Key, out _instrumentField))
                    {
                        DepthMarketData dmd = kvp.Value;
                        dmd.ProductID = _instrumentField.ProductID;
                        //有些合约虽然有ProductID，但是DMD的InstrumentID却是null
                        oc.Add(dmd);
                    }
                }
                return oc;
            }
        }

        /// <summary>
        /// 初始化_OcMainMarketDepthData
        /// </summary>
        private ObservableCollection<DepthMarketData> GetOcMainMarketDepthData(ObservableCollection<DepthMarketData> OcAllDMD)
        {
            lock (this._lockAllDepthMarketData)
            {
                ObservableCollection<DepthMarketData> oc = new ObservableCollection<DepthMarketData>();
                var grp = OcAllDMD.GroupBy(r => r.ProductID);
                foreach (IGrouping<string, DepthMarketData> item in grp)
                {
                    DepthMarketData r = item.OrderByDescending(x => x.OpenInterest).FirstOrDefault();
                    oc.Add(r);
                }
                return oc;
            }
        }

        /// <summary>
        /// 属性排序函数Sort
        /// </summary>
        private void Sort(string sortBy, ListSortDirection direction, ListView lv)
        {
            ICollectionView dataview = CollectionViewSource.GetDefaultView(lv.ItemsSource); //mainContractMktDepData
            dataview.SortDescriptions.Clear();
            switch (sortBy)
            {
                case "账户号": sortBy = "AccountID"; break;
                case "合约": sortBy = "InstrumentID"; break;
                case "期望手数": sortBy = "ExpectedContracts"; break;
                case "未过滤信号": sortBy = "Signal"; break;
                case "过滤信号": sortBy = "FiltedSignal"; break;
                case "委托日期": sortBy = "InsertDate"; break;
                case "委托时间": sortBy = "InsertTime"; break;
                case "总仓": sortBy = "TotalPosition"; break;
                case "今仓": sortBy = "TdPosition"; break;
                case "昨仓": sortBy = "YdPosition"; break;
                case "投保": sortBy = "StrHedgeFlag"; break;
                case "买卖": sortBy = "StrDirection"; break;
                default: return;
            }
            var sd = new SortDescription(sortBy, direction);
            dataview.SortDescriptions.Add(sd);
            dataview.Refresh();
        }

        /// <summary>
        /// 双击撤销当前挂单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvNoTradeOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            OrderField of = lvNoTradeOrder.SelectedItem as OrderField;
            if (of == null || this._OcNoTradeOrderField.Count <= 0)
            {
                return;
            }
            this._tradeApi.CancelOrder(of);
        }

        #endregion ==========功能函数==========


        #region ==========策略函数==========
     
        /// <summary>
        /// 策略添加
        /// </summary>
        private void AddStrategy(Strategy strategy, string pName, string pInstrumentID, int pInterval, IntervalType pIntervalType)
        {
            //向策略字典中添加字典
            if (!this._DictStrategies.TryAdd(pName, strategy))
            {
                System.Windows.Forms.MessageBox.Show("名称不能重复", "错误", 
                    System.Windows.Forms.MessageBoxButtons.OK, 
                    System.Windows.Forms.MessageBoxIcon.Warning);
                return;
            }
            //策略名称赋值
            strategy.Name = pName;
            //策略标的
            BarSeries bars = new BarSeries();
            bars.Interval = pInterval;
            bars.IntervalType = pIntervalType;
            
            InstrumentField instrumentField;
            if (this._tradeApi.DictInstrumentField.TryGetValue(pInstrumentID, out instrumentField))
            {//之后要用数据库！
                bars.InstrumentInfo = new InstrumentInfo
                {
                    InstrumentID = instrumentField.InstrumentID,
                    PriceTick = instrumentField.PriceTick,
                    VolumeMultiple = instrumentField.VolumeMultiple,
                    ProductID = instrumentField.ProductID
                };
                //策略加载BarSeries实例
                strategy.LoadBarSerieses(bars);
                //视图添加
                this._OcAllStrategy.Add(strategy);
            }
        }

        /// <summary>
        /// 策略添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddStrategy_Click(object sender, RoutedEventArgs e)
        {
            Type strategyType = (Type)this.cbxStrategyName.SelectedItem;
            Strategy strategy = (Strategy)Activator.CreateInstance(strategyType);
            string instrumentID = this.cbxStrategyInstrumentID.Text.ToString();
            IntervalType intervalType = (IntervalType)Enum.Parse(typeof(IntervalType), this.cbxStrategyIntervalType.Text.ToString());
            int interval = int.Parse(this.cbxStrategyInterval.Text);

            //调用参数Form
            using (FormParams fp = new FormParams())
            {
                ////从文本中做参数赋值
                //string ss = "(MaxLoss:1)";
                //foreach (var v in ss.Trim('(', ')').Split(','))
                //{
                //    strategy.SetParameterValue(v.Split(':')[0], v.Split(':')[1]);
                //}
                //策略参数配置
                fp.propertyGrid1.SelectedObject = strategy;
                fp.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                if (fp.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                strategy.Notify("");
            }

            //将策略添加到DictStrategies
            string name = strategyType.Name + "_" + instrumentID + "_" + interval +
                "_" + intervalType.ToString() + strategy.GetParams();
            this.AddStrategy(strategy, name, instrumentID, interval, intervalType);
        }

        /// <summary>
        /// 策略删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveStrategy_Click(object sender, RoutedEventArgs e)
        {
            if (lvAllStrategy.SelectedItem == null)
            {
                return;
            }
            Strategy strategy = (Strategy)lvAllStrategy.SelectedItem;
            Strategy tmp;
            if(this._DictStrategies.TryRemove(strategy.Name,out tmp))
                this._OcAllStrategy.Remove(tmp);
        }

        /// <summary>
        /// 加载策略文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadStrategyFile_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo strategyDI = new DirectoryInfo("../../../WT_Strategy/bin/debug/");
            foreach (var file in strategyDI.GetFiles("*.dll", SearchOption.AllDirectories))
            {
                if (file.Name == "WT_Core.dll")
                {
                    continue;
                }
                LoadStrategyFile(file.FullName);
            }
            //每次加载时，将合约信息来源绑定一下；之后会改成数据库！
            this.cbxStrategyInstrumentID.ItemsSource = this._tradeApi.DictInstrumentField.Keys;
        }

        /// <summary>
        /// 加载不同的策略类型
        /// </summary>
        /// <param name="file"></param>
        private void LoadStrategyFile(string file)
        {
            try
            {
                Assembly ass = Assembly.LoadFile(file);
                foreach (var t in ass.GetTypes())
                {
                    if (t.BaseType == typeof(Strategy) && this.cbxStrategyName.Items.IndexOf(t)<0)
                    {
                        this.cbxStrategyName.Items.Add(t);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        /// <summary>
        /// 策略是否运行？
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbStrategyIsTrading_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            CheckBox chb = (CheckBox)sender;
            Strategy strategy = chb.DataContext as Strategy;
            strategy.IsTrading = (bool)chb.IsChecked;
            if (strategy.IsTrading)
            {
                //OrderItem更新回报绑定，可下单
                foreach (var strategyData in strategy.StrategyDatas)
                {
                    strategyData.OnOrderTrigger += this.Platform_OnOrderItem;
                }
            }
            else
            {
                //OrderItem更新回报绑定，可下单
                foreach (var strategyData in strategy.StrategyDatas)
                {
                    strategyData.OnOrderTrigger -= this.Platform_OnOrderItem;
                }
            }
        }

        /// <summary>
        /// 双击选中策略，修改参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvAllStrategy_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvAllStrategy.SelectedItem == null)
            {
                return;
            }
            Strategy strategy = (Strategy)lvAllStrategy.SelectedItem;
            using (FormParams fp = new FormParams())
            {
                //策略参数配置
                fp.propertyGrid1.SelectedObject = strategy ;
                fp.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                if (fp.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                string name = strategy.Name;
                int i = name.IndexOf('(');
                strategy.Name = name.Remove(i) + strategy.GetParams();
            }
        }

        #endregion ==========策略函数========== 


        #region ==========价格条件单板==========

        /// <summary>
        /// 条件单下单
        /// </summary>
        /// <param name="cof"></param>
        private void SendConditionOrder(PriceConditionOrderField cof)
        {
            this._tradeApi.SendOrder(cof);
        }

        /// <summary>
        /// 价格条件单是否处于运行状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbPCOIsRunning_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            CheckBox chb = (CheckBox)sender;
            PriceConditionOrderField cof = chb.DataContext as PriceConditionOrderField;
            cof.IsRunning = (bool)chb.IsChecked;
            if (cof.IsRunning)
            {
                cof.ExecutionTrigger += this.SendConditionOrder;
                this._quoteApi.OnRtnDMD += cof.Run;
            }
            else
            {
                cof.ExecutionTrigger -= this.SendConditionOrder;
                this._quoteApi.OnRtnDMD -= cof.Run;
            }
        }

        /// <summary>
        /// 改变价格条件单的条件价格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PCOChangeTargetPrice(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button btn = (Button)sender;
            PriceConditionOrderField cof = (PriceConditionOrderField)btn.DataContext;
            InstrumentField instrument;
            this._tradeApi.DictInstrumentField.TryGetValue(cof.InstrumentID, out instrument);
            double tick = instrument.PriceTick;
            double Price = 0;
            StackPanel panel = (StackPanel)btn.Parent;
            TextBox tbx = (TextBox)panel.FindName("tbxTargetPrice");
            if (tbx != null)
            {
                double.TryParse(tbx.Text, out Price);
                //Instrument instrument = btn.TemplatedParent.
                if (btn.Name == "btnIncTargetPrice")
                {
                    Price += tick;
                }
                else if (btn.Name == "btnDecTargetPrice")
                {
                    if (Price > 0)
                    {
                        Price -= tick;
                    }
                }
                cof.TargetPrice = Price;
                tbx.Text = Price.ToString();
            }
        }

        /// <summary>
        /// 改变价格条件单的限价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PCOChangeLimitPrice(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button btn = (Button)sender;
            PriceConditionOrderField cof = (PriceConditionOrderField)btn.DataContext;
            InstrumentField instrument;
            this._tradeApi.DictInstrumentField.TryGetValue(cof.InstrumentID, out instrument);
            double tick = instrument.PriceTick;
            double Price = 0;
            StackPanel panel = (StackPanel)btn.Parent;
            TextBox tbx = (TextBox)panel.FindName("tbxLimitPrice");
            if (tbx != null)
            {
                double.TryParse(tbx.Text, out Price);
                if (btn.Name == "btnIncLimitPrice")
                {
                    Price += tick;
                }
                else if (btn.Name == "btnDecLimitPrice")
                {
                    if (Price > 0)
                    {
                        Price -= tick;
                    }
                }
                cof.LimitPrice = Price;
                tbx.Text = Price.ToString();
            }
        }

        /// <summary>
        /// 改变条件单下单数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PCOChangeQty(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button btn = (Button)sender;
            PriceConditionOrderField cof = btn.DataContext as PriceConditionOrderField;
            StackPanel panel = (StackPanel)btn.Parent;
            TextBox tbx = (TextBox)panel.FindName("tbxQty");
            if (tbx != null)
            {
                int Qty = cof.VolumeTotalOriginal;
                if (btn.Name == "btnIncQty")
                {
                    Qty++;
                }
                else
                {
                    if (Qty > 0)
                    {
                        Qty--;
                    }
                }
                cof.VolumeTotalOriginal = Qty;
                tbx.Text = Qty.ToString();
            }
        }

        /// <summary>
        /// 删除价格条件单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemovePCO_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            PriceConditionOrderField cof = (PriceConditionOrderField)btn.DataContext;

            //先解除外部绑定
            this._quoteApi.OnRtnDMD -= cof.Run;
            //再将运行状态设置为否
            cof.IsRunning = false;

            //从OC中删除cof
            if (cof == null || this._OcAllPriceConditionOrderField.Count <= 0)
            {
                return;
            }
            this._OcAllPriceConditionOrderField.Remove(cof);

            //GC
            cof = null;
        }

        #endregion ==========条件单板==========

        #region ==========数据库函数==========

        /// <summary>
        /// 向数据库中写入Tick数据
        /// </summary>
        /// <param name="curTick"></param>
        private void InsertTick(Tick curTick)
        {
            //this._stopwatch.Reset();
            //this._stopwatch.Start();
            this._dbCollectionTick.InsertOne(curTick); //将currentTick存入数据库中
            this._dbRecords++;
            Dispatcher.Invoke(() => this.lblTickRecorded.Content = this._dbRecords); //显示写入数据库的tick数量
            //this._stopwatch.Stop();
            //Debug.WriteLine(this._stopwatch.ElapsedMilliseconds);
        }





        #endregion ==========数据库函数==========

        
    }
}
