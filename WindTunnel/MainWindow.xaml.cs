using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindTunnel
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //参数初始化
            Body.Initialize();
            
            //绑定数据源
            this.lvAllMarketData.ItemsSource = Body.OcAllDepthMarketData;
            BindingOperations.EnableCollectionSynchronization(Body.OcAllDepthMarketData, new object());//更新线程用
            this.lvAllTradeField.ItemsSource = Body.OcAllTradeField;
            BindingOperations.EnableCollectionSynchronization(Body.OcAllTradeField, new object());
            this.lvAllDetailPositionField.ItemsSource = Body.OcAllDetailPositionField;
            BindingOperations.EnableCollectionSynchronization(Body.OcAllDetailPositionField, new object());
            this.lvAllPosition.ItemsSource = Body.OcAllPosition;
            BindingOperations.EnableCollectionSynchronization(Body.OcAllPosition, new object());
            this.lvAllOrderField.ItemsSource = Body.OcAllOrderField;
            BindingOperations.EnableCollectionSynchronization(Body.OcAllOrderField, new object());
            this.lvNoTradeOrder.ItemsSource = Body.OcNoTradeOrderField;
            BindingOperations.EnableCollectionSynchronization(Body.OcNoTradeOrderField, new object());
            this.lvCanceledOrder.ItemsSource = Body.OcCanceledOrderField;
            BindingOperations.EnableCollectionSynchronization(Body.OcCanceledOrderField, new object());
            this.lvErrorOrder.ItemsSource = Body.OcErrorOrderField;
            BindingOperations.EnableCollectionSynchronization(Body.OcErrorOrderField, new object());
            this.lvConditionOrderField.ItemsSource = Body.OcAllConditionOrderField;
            BindingOperations.EnableCollectionSynchronization(Body.OcAllConditionOrderField, new object());
            this.lvAllTradingAccount.ItemsSource = Body.OcAllTradingAccountField;
            BindingOperations.EnableCollectionSynchronization(Body.OcAllTradingAccountField, new object());
            
            //界面控件初始化
            this.cbxInstrumentID.ItemsSource = Body.trdApi.DicInstrumentField.Keys;
            this.rdoBuy.IsChecked = true;
            this.rdoOpen.IsChecked = true;
            Body.mktApi.OnTick += MainWindow_OnTick;
            this.cbxStrategies.ItemsSource = Body.ListStrategies;
        }

        //市场行情与主力合约切换
        private async void tcMarketDepthData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (e.Source is TabControl)
            {
                TabControl tabControl = (TabControl)e.Source;
                var selectedTabItem = (TabItem)tabControl.SelectedItem;
                //更新主力合约
                if (selectedTabItem.Name == "tiMainDepthMarketData")
                {
                    try
                    {
                        //btnCalculate.IsEnabled = false;
                        Body.OcMainDepthMarketData = await Task.Run(()=> Body.GenerateMainDepthData(Body.OcAllDepthMarketData));
                        lvMainMarketData.ItemsSource = Body.OcMainDepthMarketData;//更新数据源
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        //btnCalculate.IsEnabled = true;
                    }
                }
            }
        }
        //增加减少委托价格
        private void btn_ChangeOrderPrice(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (!this.tbxPrice.IsEnabled || this._instrumentID == null)
            {
                return;
            }
            Button btn = (Button)sender;
            InstrumentField instrument;
            
            if (Body.trdApi.DicInstrumentField.TryGetValue(this._instrumentID,out instrument))
            {
                double tick = instrument.PriceTick; //最小变动点数
                double price;
                if (double.TryParse(this.tbxPrice.Text,out price))
                {
                    if (btn.Name == "btnIncOrderPrice")
                    {
                        price += tick;
                    }
                    else
                    {
                        if (price > 0)
                        {
                            price -= tick;
                        }
                    }
                    this.tbxPrice.Text = price.ToString();
                }
            }
        }
        //下单板增加/减少委托数量
        private void btn_ChangeOrderQty(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button btn = (Button)sender;
            int volume;
            if(int.TryParse(this.tbxVolume.Text,out volume))
            {
                if (btn.Name == "btnAddOrderQty")
                {
                    volume++;
                }
                else
                {
                    if (volume > 0)
                    {
                        volume--;
                    }
                }
            }
            this.tbxVolume.Text = volume.ToString();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        #region 下单板

        private OrderField _mwOF = new OrderField();
        private CancellationTokenSource _mwCTS = new CancellationTokenSource();
        private string _instrumentID;
       
        //合约ID改变时自动绑定对应值
        private void cbxInstrumentID_PreviewKeyUp(object sender, RoutedEventArgs e)
        {
            //e.Handled = true;
            DepthMarketData dmd;
            if (Body.mktApi.DicDepthMarketData.TryGetValue(this.cbxInstrumentID.Text, out dmd))
            {
                _instrumentID = this.cbxInstrumentID.SelectedItem?.ToString();
                if (this._instrumentID == null)
                    return;
                this.tbxPrice.Text = dmd.LastPrice.ToString();
                this.btnAskPrice1.Content = dmd.AskPrice1.ToString();
                this.tbkAskVolume1.Text = dmd.AskVolume1.ToString();
                this.btnBidPrice1.Content = dmd.BidPrice1.ToString();
                this.tbkBidVolume1.Text = dmd.BidVolume1.ToString();
            }
            else
            {
                _instrumentID = null;
                this.tbxPrice.Text = string.Empty;
                this.btnAskPrice1.Content = string.Empty;
                this.tbkAskVolume1.Text = string.Empty;
                this.btnBidPrice1.Content = string.Empty;
                this.tbkBidVolume1.Text = string.Empty;
            }
        }
        //买
        private void rdoBtnBuy_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)this.rdoBuy.IsChecked)
            {
                this.btnOrderInsert.Background = new SolidColorBrush(Colors.DarkRed);
                this._mwOF.Direction = TThostFtdcDirectionType.THOST_FTDC_D_Buy;
            }
        }
        //卖
        private void rdoBtnSell_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)this.rdoSell.IsChecked)
            {
                this.btnOrderInsert.Background = new SolidColorBrush(Colors.Green);
                this._mwOF.Direction = TThostFtdcDirectionType.THOST_FTDC_D_Sell;
            }
        }
        //开仓
        private void rdoOpen_Checked(object sender, RoutedEventArgs e)
        {
            this._mwOF.CombOffsetFlag = new String((char)TThostFtdcOffsetFlagType.THOST_FTDC_OF_Open,1);
        }
        //平今
        private void rdoCloseToday_Checked(object sender, RoutedEventArgs e)
        {
            this._mwOF.CombOffsetFlag = new String((char)TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseToday, 1);
        }
        //平昨
        private void rdoCloseYesterday_Checked(object sender, RoutedEventArgs e)
        {
            this._mwOF.CombOffsetFlag = new String((char)TThostFtdcOffsetFlagType.THOST_FTDC_OF_Close, 1);
        }
        //是否采用市价单
        private void chbIsMarketOrder_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chbIsMarketOrder.IsChecked)
            {
                this.tbxPrice.IsEnabled = false;
            }
            else
            {
                this.tbxPrice.IsEnabled = true;
            }
        }

        //点击卖一价，则委托价自动变为买一价
        private void btnAskPrice1_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)this.chbIsMarketOrder.IsChecked)
            {
                this.tbxPrice.Text = this.btnAskPrice1.Content?.ToString();
            }
        }
        //点击买一价，则委托价自动变为买一价
        private void btnBidPrice1_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)this.chbIsMarketOrder.IsChecked)
            {
                this.tbxPrice.Text = this.btnBidPrice1.Content?.ToString();
            }
        }
        //下单按钮
        private void btnOrderInsert_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.CheckOrder();
            //投机
            //_mwOF.CombHedgeFlag = new string((char)TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation, 1);
            ////当日有效
            //_mwOF.TimeCondition = TThostFtdcTimeConditionType.THOST_FTDC_TC_GFD;
            ////任何数量
            //_mwOF.VolumeCondition = TThostFtdcVolumeConditionType.THOST_FTDC_VC_AV;
            ////立即执行
            //_mwOF.ContingentCondition = TThostFtdcContingentConditionType.THOST_FTDC_CC_Immediately;
            
            //_mwOF.OrderRef = string.Format("{0:000000}{1:000000}", Body.trdApi.DicOrderField.Count, Interlocked.Add(ref Body.nOrders, 1) % 1000000);
            
            //下单
            Body.trdApi.SendOrder(this._mwOF);
        }
        //条件单创建
        private void btnConditionOrderCreate_Click(object sender, RoutedEventArgs e)
        {
            this.CheckOrder();
            ConditionOrderField cof = new ConditionOrderField(this._mwOF);
            cof.TargetPrice = this._mwOF.LimitPrice;
            //cof.OrderRef = string.Format("{0:000000}{1:000000}", Body.trdApi.DicOrderField.Count, Interlocked.Add(ref Body.nOrders, 1) % 1000000);
            lock (Body.OcAllConditionOrderField)
            {
                Body.OcAllConditionOrderField.Add(cof);
            }
          
        }
        //检查报单是否参数正确
        private void CheckOrder()
        {
            //合约ID，方向，投保，价格（限价）
            if (this.cbxInstrumentID.SelectedItem == null)
            {
                MessageBox.Show("请选择合约代码！");
                return;
            }
            this._mwOF.InstrumentID = this.cbxInstrumentID.SelectedItem.ToString();

            double price;
            if (double.TryParse(this.tbxPrice.Text, out price))
            {
                this._mwOF.LimitPrice = price;
            }
            else
            {
                MessageBox.Show("报价错误！");
                return;
            }

            int volume;
            if (int.TryParse(this.tbxVolume.Text, out volume))
            {
                this._mwOF.VolumeTotalOriginal = volume;
            }
            else
            {
                MessageBox.Show("委托数量错误！");
                return;
            }

            if ((bool)this.chbIsMarketOrder.IsChecked)
            {
                _mwOF.OrderPriceType = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AnyPrice;
            }
            else
            {
                _mwOF.OrderPriceType = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice;
            }
        }

        #endregion 下单板


        //////////////////////////////////////////////////////////////////////////////////////////////
        #region 窗口函数

        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirection;

        //双击，则在New Order中撤单
        private void lvNoTradeOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            OrderField of = lvNoTradeOrder.SelectedItem as OrderField;
            if (of == null || Body.OcNoTradeOrderField.Count <= 0)
            {
                return;
            }
            Body.trdApi.CancelOrder(of);
        }

        //点击列标题后排序：暂时只对主力合约listview有效
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

        //属性排序函数Sort
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

        //窗口关闭时调用
        private void Window_Closed(object sender, EventArgs e)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////
        #region 回调绑定
        //行情回调
        private void MainWindow_OnTick(DepthMarketData dmd)
        {
            if (dmd.InstrumentID != this._instrumentID)
            {
                return;
            }
            Dispatcher.Invoke(new Action(()=>
            {
                this.btnAskPrice1.Content = dmd.AskPrice1.ToString();
                this.tbkAskVolume1.Text = dmd.AskVolume1.ToString();
                this.btnBidPrice1.Content = dmd.BidPrice1.ToString();
                this.tbkBidVolume1.Text = dmd.BidVolume1.ToString();
            }));
        }

        #endregion 回调绑定

        //计算CTA策略信号，并且显示结果
        async private void btnCTA1Calculate_Click(object sender, RoutedEventArgs e)
        {
            //CTAStrategy cta1 = Body.ListStrategies[0] as CTAStrategy;
            //var tu = await Task.Run(() => cta1.Calculate()) as Tuple;
            //this.lvCTADailyStrategyResult.ItemsSource = tu.Item1;
            //this.lvStrategyOrder.ItemsSource = tu.Item2;

            //this.lvCTADailyStrategyResult.ItemsSource = Body.cta1.Calculate(false);
        }

        //策略报单下单
        private void btnStrategySendOrders_Click(object sender, RoutedEventArgs e)
        {
            StrategyBase strategy = this.cbxStrategies.SelectedItem as StrategyBase;
            if (strategy != null)
            {
                strategy.SendOrders();
            }
        }

        //策略计算
        async private void btnStrategyCalculate_Click(object sender, RoutedEventArgs e)
        {
            StrategyBase strategy = this.cbxStrategies.SelectedItem as StrategyBase;
            if (strategy != null)
            {
                await Task.Run(() => strategy.Calculate());
                this.lvStrategyCalculationResult.ItemsSource = strategy.OcStrategyCalculationResults;
                this.lvStrategyOrder.ItemsSource = strategy.OcStrategyOrders;
            }
            
        }

        //选中策略变换
        private void cbxStrategies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbxStrategies.SelectedIndex == 0)
            {//选中CTA策略
                this.lvStrategyCalculationResult.View = App.Current.Resources["CTADailyStrategyCalcualtionResultGridView"] as GridView;
            }
            else
            {
                this.lvStrategyCalculationResult.View = null;
            }
        }

        //查询持仓
        async private void btnQryInvestorPosition_Click(object sender, RoutedEventArgs e)
        {
            Body.trdApi.StackQuery.Push(new Tuple<EnumReqCmdType, object>(EnumReqCmdType.QryInvestorPosition, ""));
            await Body.trdApi.ExecStackQuery();
        }

        //查询交易账户
        async private void btnQryTradingAccount_Click(object sender, RoutedEventArgs e)
        {
            Body.trdApi.StackQuery.Push(new Tuple<EnumReqCmdType, object>(EnumReqCmdType.QryTradingAccount, ""));
            await Body.trdApi.ExecStackQuery();
        }

    }
}
