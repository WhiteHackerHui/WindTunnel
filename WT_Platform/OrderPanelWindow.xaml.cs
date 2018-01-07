using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WT_Core;
using WT_CTP;

namespace WT_Platform
{
    /// <summary>
    /// OrderPanelWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OrderPanelWindow : Window
    {
        MainWindow _mw;
        string _instrumentID;
        OrderField _of = new OrderField();

        public OrderPanelWindow(MainWindow mw)
        {
            InitializeComponent();

            this._mw = mw;
            this._mw.QuotedAPI.OnRtnTick += this.OnRtnTick;
            this.cbxInstrumentID.ItemsSource = this._mw.TradeAPI.DictInstrumentField.Keys;
            this.rdoBuy.IsChecked = true;
            this.rdoOpen.IsChecked = true;
        }

        /// <summary>
        /// 实时显示一档买卖价格
        /// </summary>
        /// <param name="tick"></param>
        private void OnRtnTick(Tick tick)
        {
            if (tick.InstrumentID != this._instrumentID)
            {
                return;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                this.btnAskPrice1.Content = tick.AskPrice.ToString();
                this.tbkAskVolume1.Text = tick.AskVolume.ToString();
                this.btnBidPrice1.Content = tick.BidPrice.ToString();
                this.tbkBidVolume1.Text = tick.BidVolume.ToString();
            }));
        }

        /// <summary>
        /// 选中合约
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxInstrumentID_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            DepthMarketData dmd;
            if ( this._mw.QuotedAPI.DictDepthMarketData.TryGetValue(this.cbxInstrumentID.Text, out dmd))
            {
                _instrumentID = this.cbxInstrumentID.Text;
                this.tbxPrice.Text = dmd.LastPrice.ToString();
                this.btnAskPrice1.Content = dmd.AskPrice1.ToString();
                this.tbkAskVolume1.Text = dmd.AskVolume1.ToString();
                this.btnBidPrice1.Content = dmd.BidPrice1.ToString();
                this.tbkBidVolume1.Text = dmd.BidVolume1.ToString();
                //_instrumentID = dmd.InstrumentID;
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
        
        /// <summary>
        /// 选中买单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoBtnBuy_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)this.rdoBuy.IsChecked)
            {
                this.btnOrderInsert.Background = new SolidColorBrush(Colors.DarkRed);
                this._of.Direction = Direction.Buy;
            }
        }

        /// <summary>
        /// 选中卖单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoBtnSell_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)this.rdoSell.IsChecked)
            {
                this.btnOrderInsert.Background = new SolidColorBrush(Colors.Green);
                this._of.Direction = Direction.Sell;
            }
        }

        /// <summary>
        /// 选中开仓
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoOpen_Checked(object sender, RoutedEventArgs e)
        {
            this._of.CombOffsetFlag = Offset.Open;
        }

        /// <summary>
        /// 选中平今
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoCloseToday_Checked(object sender, RoutedEventArgs e)
        {
            this._of.CombOffsetFlag = Offset.CloseToday;
        }

        /// <summary>
        /// 选中平昨
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoCloseYesterday_Checked(object sender, RoutedEventArgs e)
        {
            this._of.CombOffsetFlag = Offset.Close;
        }

        /// <summary>
        /// 以卖一价作为委托价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAskPrice1_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)this.chbIsMarketOrder.IsChecked)
            {
                this.tbxPrice.Text = this.btnAskPrice1.Content?.ToString();
            }
        }

        /// <summary>
        /// 以买一价作为委托价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBidPrice1_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)this.chbIsMarketOrder.IsChecked)
            {
                this.tbxPrice.Text = this.btnBidPrice1.Content?.ToString();
            }
        }

        /// <summary>
        /// 上下调整委托价格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ChangeOrderPrice(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (!this.tbxPrice.IsEnabled || this._instrumentID == null)
            {
                return;
            }
            Button btn = (Button)sender;
            InstrumentField instrument;

            if (this._mw.TradeAPI.DictInstrumentField.TryGetValue(this._instrumentID, out instrument))
            {
                double tick = instrument.PriceTick; //最小变动点数
                double price;
                if (double.TryParse(this.tbxPrice.Text, out price))
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

        /// <summary>
        /// 是否使用市价单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 上下调整委托数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ChangeOrderQty(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button btn = (Button)sender;
            int volume;
            if (int.TryParse(this.tbxVolume.Text, out volume))
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

        /// <summary>
        /// 下单确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOrderInsert_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.CheckOrder();
            this._mw.TradeAPI.SendOrder(this._of);
        }

        /// <summary>
        /// 检查报单是否参数正确
        /// </summary>
        private void CheckOrder()
        {
            //合约ID，方向，投保，价格（限价）
            if (this.cbxInstrumentID.SelectedItem == null)
            {
                MessageBox.Show("请选择合约代码！");
                return;
            }
            this._of.InstrumentID = _instrumentID;

            double price;
            if (double.TryParse(this.tbxPrice.Text, out price))
            {
                this._of.LimitPrice = price;
            }
            else
            {
                MessageBox.Show("报价错误！");
                return;
            }

            int volume;
            if (int.TryParse(this.tbxVolume.Text, out volume))
            {
                this._of.VolumeTotalOriginal = volume;
            }
            else
            {
                MessageBox.Show("委托数量错误！");
                return;
            }

            if ((bool)this.chbIsMarketOrder.IsChecked)
            {
                this._of.OrderPriceType = OrderPriceType.AnyPrice;
            }
            else
            {
                this._of.OrderPriceType = OrderPriceType.LimitPrice;
            }
        }

        /// <summary>
        /// 新建条件价格单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreatePriceConditionOrder_Click(object sender, RoutedEventArgs e)
        {
            this.CheckOrder();
            PriceConditionOrderField cof = new PriceConditionOrderField();
            cof.OrderFieldInstance = this._of.OrderFieldInstance;
            cof.TargetPrice = this._of.LimitPrice;
            this._mw._OcAllPriceConditionOrderField.Add(cof);
        
        }
    }
}
