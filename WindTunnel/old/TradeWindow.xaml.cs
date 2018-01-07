using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
//using System.Deployment.Application;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;

//namespace WindTunnel
//{
//    /// <summary>
//    /// TrdWindow.xaml 的交互逻辑
//    /// </summary>
//    public partial class TradeWindow : Window
//    {
//        //变量
//        public ObservableCollection<DepthMarketData> allContractMktDepData,mainContractMktDepData;
//        public ObservableCollection<Order> allOrders, cancelledOrders, newOrders;
//        public ObservableCollection<Position> allPositions;
//        public ObservableCollection<Trade> allTrades;
//        public ObservableCollection<Order> ordersToInsert;
//        public ObservableCollection<Account> accounts;
//        public ObservableCollection<CTACalResult> calResults;
//        private GridViewColumnHeader _lastHeaderClicked;
//        private ListSortDirection _lastDirection;
//        public string accountID;
//        static public double leverageDefinedByUser = 0.029, lembdaDefinedByUser=0.94, riskfactorDefinedByUser = 1;
//        static DispatcherTimer timer = new DispatcherTimer();
//        public static DispatcherTimer saveCSVTimer,systemUpdateTimer,dailyCTATimer,connTimer,TdxTimer;
//        public Random rnd = new Random();


//        public TradeWindow()
//        {
//            InitializeComponent(); 

//            //初始化市场行情listview
//            Body.GenerateAllContractMarketDepthData();
//            allContractMktDepData = Body.AllContractMktDepData;
//            lvAllContractMktData.Items.Clear();
//            lvAllContractMktData.ItemsSource = allContractMktDepData;

//            //所有报单绑定
//            allOrders = Body.orderSeries;
//            lvAllOrders.ItemsSource = allOrders;
//            BindingOperations.EnableCollectionSynchronization(allOrders, new object());//更新线程用
//            //撤单绑定
//            cancelledOrders = Body.cancelledOrdersSeries;
//            lvCancelledOrders.ItemsSource = cancelledOrders;
//            BindingOperations.EnableCollectionSynchronization(cancelledOrders, new object());//更新线程用
//            //新单绑定
//            newOrders = Body.newOrdersSeries;
//            lvNewOrders.ItemsSource = newOrders;
//            BindingOperations.EnableCollectionSynchronization(newOrders, new object());//更新线程用

//            //持仓绑定
//            allPositions = Body.posSeries;
//            lstviewInvestorPosition.ItemsSource = allPositions;
//            BindingOperations.EnableCollectionSynchronization(allPositions, new object());//更新线程用

//            //成交报单绑定
//            allTrades = Body.tradeSeries;
//            lvTrade.ItemsSource = allTrades;
//            BindingOperations.EnableCollectionSynchronization(allTrades, new object());//更新线程用

//            //插入报单绑定
//            ordersToInsert = Body.OrdersToInsert;
//            lvOrdersToInsert.ItemsSource = ordersToInsert;
//            BindingOperations.EnableCollectionSynchronization(ordersToInsert, new object());//更新线程用

//            //主力合约
//            Body.GenerateMainContractMarketDepthData();
//            mainContractMktDepData = Body.MainContractMktDepData;

//            //账户
//            accounts = Body.Accounts;
//            lvTradingAccount.ItemsSource = accounts;
//            BindingOperations.EnableCollectionSynchronization(accounts, new object());//更新线程用

//            //计算结果
//            calResults = CTAStrategy.CalculationResults;
//            lvCTADailyStrategyResult.ItemsSource = calResults;
//            BindingOperations.EnableCollectionSynchronization(calResults, new object());//更新线程用

//            //绑定杠杆率
//            tbxLeverage.Text = leverageDefinedByUser.ToString();
//            tbxLembda.Text = lembdaDefinedByUser.ToString();
//            tbxRiskfactor.Text = riskfactorDefinedByUser.ToString();

//            //绑定定时调用
//            timer.Tick += new EventHandler(timeCycle);

//            //用来处理数据保存，每一分钟存储一次。
//            saveCSVTimer = new DispatcherTimer();
//            saveCSVTimer.Tick += new EventHandler(SaveCSVCycle);
//            saveCSVTimer.Interval = TimeSpan.FromSeconds(60);
//            saveCSVTimer.Start();

//            //系统更新Timer
//            systemUpdateTimer = new DispatcherTimer();
//            systemUpdateTimer.Tick += new EventHandler(UpdateSystem);
//            systemUpdateTimer.Interval = TimeSpan.FromSeconds(1);
//            systemUpdateTimer.Start();

//            //CTA日度策略Timer
//            dailyCTATimer = new DispatcherTimer();
//            dailyCTATimer.Interval = TimeSpan.FromSeconds(1);
//            dailyCTATimer.Tick += new EventHandler(
//                async (senderParam, eParam) => {
//                    if (DateTime.Now.Hour == 14 && DateTime.Now.Minute == 45)
//                    {
//                        dailyCTATimer.Stop();
//                        await CalculateSignals();
//                    }
//                });
//            tglAutoCalculation.IsChecked = true;    //下午2:45自动计算信号

//            //连接设置：在指定时间段断连或者连接
//            connTimer = new DispatcherTimer();
//            connTimer.Interval = TimeSpan.FromSeconds(10);
//            connTimer.Tick += CheckConnectionStatus;
//            connTimer.Start();

//            //定时更新TDX行情数据
//            TdxTimer = new DispatcherTimer();
//            TdxTimer.Interval = TimeSpan.FromSeconds(1);
//            TdxTimer.Start();

//            #if !DEBUG
//            //获得版本号
//            try
//            {
//                string version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(); //System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
//                this.Title += string.Format(" Version {0}", version);
//            }
//            catch(InvalidDeploymentException)
//            {
                
//            }
//            #endif
//        }

//        //在指定时间段断连或者连接
//        async private void CheckConnectionStatus(object sender, EventArgs e)
//        {
//            DateTime systemTime = DateTime.Now;
//            //断开连接时间段：16:10至20:30 
//            if (systemTime.IsInTimeRange(16, 10, 20, 30))
//            {
//                if (Body.trdAPI.IsConnected)
//                {//断开交易接口
//                    Body.trdAPI.Disconnect();
//                }
//                if (Body.mktAPI.IsConnected)
//                {//断开Market接口
//                    Body.mktAPI.Disconnect();
//                }
//            }

//            //连接时间段：08:50至16:00 以及 20:50以后
//            if (systemTime.IsInTimeRange(8, 50, 16, 0) || systemTime.IsInTimeRange(20, 50, 23, 59)
//                || systemTime.IsInTimeRange(0, 0, 2, 40))
//            {
//                if (!Body.trdAPI.IsConnected)
//                {//连接交易接口（在连接时，暂停计时器）
//                    await Body.trdAPI.AsyncConnect(Body.tokenSource.Token);
//                }
//                if (!Body.mktAPI.IsConnected)
//                {//连接Market接口
//                    await Body.mktAPI.AsyncConnect(Body.tokenSource.Token);
//                    //Body.StoreDataList.Clear();
//                    await Body.trdAPI.QueryAllInstruments(Body.tokenSource.Token);
//                    Body.mktAPI.Subscribe(string.Join(";", Body.mktdepdata.Keys), ""); //订阅行情数据

//                }
//            }

//            //自动关闭：03：00至08:30
//            if ((systemTime.IsInTimeRange(3, 0, 8, 30)))
//            {
//                this.Close();
//            }
//        }

//        //每一秒钟更新一次系统状态
//        async private void UpdateSystem(object sender, EventArgs e)
//        {
//            //连接状态监控
//            lblConnectionStatus.Content = string.Format("Market：{0} ； Trade：{1}",
//                Body.connMktAPIStatus.ToString(), Body.connTrdAPIStatus.ToString());
//            ////查询持仓和账户信息
//            //await Body.trdAPI.QueryInvestorPosition(Body.tokenSource.Token);
//            //await Body.trdAPI.QueryTradingAccount(Body.tokenSource.Token);
//        }

//        //双击lvAllContractMktData的Item后，自动添加OrdersToInsert
//        private void lvMktData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//        {
//            ListView lv = sender as ListView;
//            var item = lv.SelectedItem;
//            if (item==null)
//            {
//                return;
//            }
//            DepthMarketData dmd = item as DepthMarketData;
//            //初步构造报单
//            OrderField orderField = default(OrderField);
//            orderField.InstrumentID = dmd.InstrumentID;
//            orderField.Price = dmd.LastPrice;
//            Order order = new Order(orderField);
//            Body.OrdersToInsert.Add(order);
//        }

//        //确认下单
//        private void btnOrderInsert_Click(object sender, EventArgs e)
//        {
//            Body.SendOrders(Body.OrdersToInsert.ToArray());
//            Body.OrdersToInsert.Clear();
//        }

//        //清空所有OrdersToInsert
//        private void btnOrderClear_Click(object sender, RoutedEventArgs e)
//        {
//            Body.OrdersToInsert.Clear();
//        }

//        //打印成交单
//        private void PrintTradesOrdersPositions(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                btnPrint.IsEnabled = false;
//                allTrades.ToCSV(@"E:\C# Projects\TradeHelper\输出文件\allTrades" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
//                allOrders.ToCSV(@"E:\C# Projects\TradeHelper\输出文件\allOrders_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
//                allPositions.ToCSV(@"E:\C# Projects\TradeHelper\输出文件\allPositions_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
//            finally
//            {
//                btnPrint.IsEnabled = true;
//            }
//        }

//        //双击，则在下单版面删除报单
//        private void lvOrdersToInsert_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//        {
//            var item = lvOrdersToInsert.SelectedItem;
//            if (item == null || ordersToInsert.Count <= 0)
//            {
//                return;
//            }
//            Order orderToDelete = item as Order;
//            //找出是否已经有该合约代码在OrdersToInsert中
//            int index = -1;
//            for (int i = 0; i < ordersToInsert.Count; i++)
//            {
//                if (orderToDelete.InstrumentID == ordersToInsert[i].InstrumentID)
//                {
//                    index = i;
//                    break;
//                }
//            }
//            if (index >= 0)
//                ordersToInsert.RemoveAt(index);
//        }

//        //双击，则在New Order中撤单
//        private void lvNewOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//        {
//            var item = lvNewOrders.SelectedItem;
//            if (item == null || newOrders.Count <= 0)
//            {
//                return;
//            }
//            Order newOrderToCancel = item as Order;
//            Body.trdAPI.CancelOrder(newOrderToCancel.LocalID);
//        }

//        //点击列标题后排序：暂时只对主力合约listview有效
//        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
//        {
//            lock (this)
//            {
//                ListView lv = sender as ListView;
//                var headerClicked = e.OriginalSource as GridViewColumnHeader;
//                ListSortDirection direction = 0;
//                if (lv == null || headerClicked == null || headerClicked.Role == GridViewColumnHeaderRole.Padding)
//                    return;
//                if (headerClicked != _lastHeaderClicked)
//                {//如果选中的列不是上次选中的列，则direction设置为Asc
//                    direction = ListSortDirection.Ascending;
//                }
//                else
//                {//对于同一个选中列第二次排序
//                    if (_lastDirection == ListSortDirection.Ascending)
//                    {
//                        direction = ListSortDirection.Descending;
//                    }
//                    else
//                    {
//                        direction = ListSortDirection.Ascending;
//                    }
//                }
//                //排序
//                string header = headerClicked.Column.Header as string;
//                Sort(header, direction,lv);
//                _lastHeaderClicked = headerClicked;
//                _lastDirection = direction;
//            }
//        }

//        //手动查询持仓
//        async private void btnQryInvestorPosition_Click(object sender, RoutedEventArgs e)
//        {
//            await Body.trdAPI.QueryInvestorPosition(Body.tokenSource.Token);
//        }

//        //下载数据选项
//        //开始下载数据
//        private void checkDownloadData_Checked(object sender, RoutedEventArgs e)
//        {
//            Body.mktAPI.OnRtnDepthMarketData += Body.OnRtn_SaveDepthMarketData;
//        }
//        //停止下载数据
//        private void checkDownloadData_Unchecked(object sender, RoutedEventArgs e)
//        {
//            Body.mktAPI.OnRtnDepthMarketData -= Body.OnRtn_SaveDepthMarketData;
//        }

//        //隔段时间开始执行计算信号和下单
//        private async void timeCycle(object sender, EventArgs e)
//        {
//            tbxLeverage.Text = (rnd.Next(-1000, 1000)/20000.0).ToString();
//            await CalculateSignals();
//            Body.SendOrders(Body.OrdersToInsert.ToArray());
//        }

//        //隔段时间开始执行计算信号和下单
//        private void SaveCSVCycle(object sender, EventArgs e)
//        {
//            //存储CSV文件
//            Body.SaveCSVFile();
//            this.lblDataSaveCSVStatus.Content = string.Format( "已写入 {0} 条数据。",Body.lastRow.ToString());
//        }

//        //计算信号
//        async void CalulateSignalsAndOrders(object sender, RoutedEventArgs e)
//        {
//            await CalculateSignals();
//        }

//        //将数据转换为数据表DataTable
//        private void DataMaintenance(object sender, RoutedEventArgs e)
//        {
//            if (checkDownloadData.IsChecked == true)
//            {
//                MessageBox.Show("下载数据过程中不能做数据维护！");
//                return;
//            }
//            DatabaseWindow dbWindow = new DatabaseWindow();
//            dbWindow.Show();
//        }

//        //取消当前所有未成交挂单（New）
//        private void btnNewOrdersCancel_Click(object sender, RoutedEventArgs e)
//        {
//            Body.CancelAllNewOrders();
//        }

//        //查询结算单
//        async private void tbnQuerySettlementInfo_Click(object sender, RoutedEventArgs e)
//        {
//            MessageBox.Show(await Body.trdAPI.QuerySettlementInfo(Body.tokenSource.Token));  
//        }

//        //取消订阅
//        private void btnUnsubscribe_Click(object sender, RoutedEventArgs e)
//        {
//            Body.mktAPI.Unsubscribe(string.Join(";", Body.mktdepdata.Keys), "");
//        }

//        //通达信数据下载
//        async private void btnTDXData_Click(object sender, RoutedEventArgs e)
//        {
//            //参数设置
//            StringBuilder tdxResult = new StringBuilder(1024 * 1024);
//            StringBuilder tdxError = new StringBuilder(256);
            
//            TDXParameters.Initialize();
//            IpAddressAndPort ipp = TDXParameters.exConnparameters["扩展市场上海主站1"];
            
//            //拓展行情连接
//            TDX.TdxExHq_Connect(ipp.ip, ipp.port,tdxResult,tdxError);     
            
//            //获得所有Instrument的数量
//            int instrumentCount = 0;
//            TDX.TdxExHq_GetInstrumentCount(ref instrumentCount, tdxError);

//            ////获得所有合约的市场代码、合约代码
//            //string instrumentInfoData = null;
//            //bool gotInstrumentInfo = await Task.Run(()=> TDX.ExHq_GetInstrumentInfo(out instrumentInfoData));
//            //File.WriteAllText(@"E:\C# Projects\TradeHelper\TDXData\InstrumentInfo.csv",
//            //    instrumentInfoData.Replace("\t",","), Encoding.Default);

//            ////获得期权指定日的历史成交数据
//            DataTable instrumentInfos;
//            Functions.FromCSV<TdxInstrumentInfo>(out instrumentInfos, @"E:\C# Projects\TradeHelper\TDXData\InstrumentInfo.csv");
//            var q = from r in instrumentInfos.AsEnumerable()
//                    where r.Field<string>("Code").StartsWith("1000") && r.Field<string>("Market") == "8"
//                    select r.Field<string>("Code");
//            string[] optionCodes = q.ToArray();
//            //int date = 20170117;
//            //await Task.Run(()=> TDX.WriteHistoryTransactionDataToCSVs(8, q.ToArray(), date, 
//            //    string.Format(@"E:\TDX_CSV\{0}\HistTransactionData", date.ToString())));

//            //得到上海期权（市场代码为8）的五档信息
//            TDX.InstrumentQuotes = new Dictionary<string, StringBuilder>();
//            for (int i = 0; i < 3; i++)
//            {
//                TDX.InstrumentQuotes.Add(optionCodes[i], new StringBuilder());
//            }

//            //TdxTimer.Tick += new EventHandler((object o, EventArgs ea) => TDX.ExHq_InstrumentQuoteHandler(8,optionCodes));
//            //TDX.ExHq_InstrumentQuoteHandler(8, optionCodes);

//            var taskQueue = new ConcurrentQueue<string>();
//            var cts = new CancellationTokenSource();
//            var taskSource = Task.Run(()=>TDX.TaskProducer(taskQueue));

//            Task[] taskProcessors = new Task[4];
//            for (int i = 0; i < 4; i++)
//            {
//                taskProcessors[i] = Task.Run(() => TDX.TaskProcessor(taskQueue, cts.Token));
//            }
//            await taskSource;
            
//            cts.CancelAfter(10 * 1000);
//            await Task.WhenAll(taskProcessors);
//            //TdxTimer.Tick += new EventHandler(
//            //    (object o, EventArgs ea) =>
//            //    {
//            //        dt1 = DateTime.Now;
//            //        TDX.TdxExHq_GetInstrumentQuote(8, "10000671", tdxResult, tdxError);
//            //        string tmpText = tdxResult.ToString();
//            //        sb.Append(tmpText.Remove(0, tmpText.IndexOf("\n")));
//            //        double t = (dt1 - DateTime.Now).TotalMilliseconds;
//            //    }
//            //    );
//            //TdxTimer.Start();
//            //await Task.Run(()=> Thread.Sleep(60 * 1000 * 2));
//            //TdxTimer.Stop();
//            //File.WriteAllText(@"E:\C# Projects\TradeHelper\TDXData\Quote.csv",
//            //    sb.ToString().Replace("\t", ",").Insert(0, "市场,代码,昨收,开盘,最高,最低,现价,开仓,持仓,总量,现量,内盘,外盘,买一价,买二价,买三价,买四价,买五价,买一量,买二量,买三量,买四量,买五量,卖一价,卖二价,卖三价,卖四价,卖五价,卖一量,卖二量,卖三量,卖四量,卖五量,仓差,日期"), Encoding.Default);

//        }

//        private void DataFileWriteToCsv(object sender, RoutedEventArgs e)
//        {
//            if (checkDownloadData.IsChecked == true)
//            {
//                MessageBox.Show("下载数据过程中不能写入数据到CSV文件！");
//                return;
//            }
//            Body.SaveCSVFile();
//            this.lblDataSaveCSVStatus.Content = string.Format("已写入 {0} 条数据。", Body.lastRow.ToString());
//        }

//        //自动在每天14：45分交易CTA日度策略
//        private void StartAutoTrading(object sender, RoutedEventArgs e)
//        {
//            dailyCTATimer.Start();
//        }

//        //关闭CTA日度策略自动执行
//        private void EndAutoTrading(object sender, RoutedEventArgs e)
//        {
//            dailyCTATimer.Stop();
//        }

//        //计算CTA日度策略信号
//        async public Task CalculateSignals()
//        {
//            try
//            {
//                //如果还有挂单未成交，则先清空
//                //Body.CancelAllNewOrders();
//                //if (newOrders.Count > 0)
//                //{
//                //    MessageBox.Show("请先撤销未成交报单！");
//                //    return;
//                //}
//                btnCalculate.IsEnabled = false;
//                tabMarketData.IsEnabled = false;
//                leverageDefinedByUser = double.Parse(tbxLeverage.Text);
//                lembdaDefinedByUser = double.Parse(tbxLembda.Text);
//                riskfactorDefinedByUser = double.Parse(tbxRiskfactor.Text);

//                //计算信号和手数
//                await CTAStrategy.Calculate((bool)chkPrintIntermediateResult.IsChecked);
//                //根据计算结果做单
//                CTAStrategy.PortfolioAllocation();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
//            finally
//            {
//                btnCalculate.IsEnabled = true;
//                tabMarketData.IsEnabled = true;
//            }
//        }

//        //查询账户信息
//        async private void btnQryTradingAccount_Click(object sender, RoutedEventArgs e)
//        {
//            await Body.trdAPI.QueryTradingAccount(Body.tokenSource.Token);
//        }

//        //对于指定列排序，主要针对CTA Result视图
//        private void Sort(string sortBy, ListSortDirection direction, ListView lv)
//        {
//            ICollectionView dataview = CollectionViewSource.GetDefaultView(lv.ItemsSource); //mainContractMktDepData
//            dataview.SortDescriptions.Clear();
//            switch (sortBy)
//            {
//                case "账户号": sortBy = "AccountID"; break;
//                case "合约": sortBy = "InstrumentID"; break;
//                case "期望手数": sortBy = "ExpectedContracts";break;
//                case "未过滤信号": sortBy = "Signal"; break;
//                case "过滤信号": sortBy = "FiltedSignal"; break;
//                case "成交时间": sortBy = "Time"; break;
//                case "时间": sortBy = "Time"; break;
//                case "仓位": sortBy = "Positions"; break;
//                case "今仓": sortBy = "TodayPosition"; break;
//                case "昨仓": sortBy = "HistoryPosition"; break;
//                case "性质": sortBy = "HedgeFlag"; break;
//                case "多空": sortBy = "Side"; break;
//                case "买卖": sortBy = "Side"; break;
//                default: return;
//            }
//            var sd = new SortDescription(sortBy, direction);
//            dataview.SortDescriptions.Add(sd);
//            dataview.Refresh();
//        }

//        //关闭整个应用
//        private void Window_Closed(object sender, EventArgs e)
//        {
//            connTimer.Stop();
//            dailyCTATimer.Stop();
//            saveCSVTimer.Stop();
//            systemUpdateTimer.Stop();
//            Body.mktAPI.Disconnect();
//            Body.trdAPI.Disconnect();
//            Application.Current.Shutdown();
//            Environment.Exit(0);
//        }

//        //市场行情与主力合约切换
//        private void tabMarkets_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (e.Source is TabControl)
//            {
//                //mainContractMktDepData?.Clear();
//                TabControl tabControl =(TabControl)e.Source;
//                var selectedTab = (TabItem)tabControl.SelectedItem;
//                //更新主力合约
//                if (selectedTab.TabIndex == 1)
//                {
//                    try
//                    {
//                        btnCalculate.IsEnabled = false;
//                        Body.GenerateMainContractMarketDepthData();
//                        mainContractMktDepData = Body.MainContractMktDepData;
//                        lvMainContractMktData.ItemsSource = mainContractMktDepData;//更新数据源
//                    }
//                    catch (Exception)
//                    {
//                        throw;
//                    }
//                    finally
//                    {
//                        btnCalculate.IsEnabled = true;
//                    }
//                }
//            }
//        }


//    }
//}
