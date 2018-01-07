using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WindTunnel
{
    partial class AppResourceDictionary: ResourceDictionary
    {
        //增加或者减少Order的手数
        private void btn_ChangeOrderQty(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button btn = (Button)sender;
            ConditionOrderField cof = btn.DataContext as ConditionOrderField;
            StackPanel panel = (StackPanel)btn.Parent;
            TextBox tbx = (TextBox)panel.FindName("tbxQty");
            if (tbx != null)
            {
                int Qty = cof.VolumeTotalOriginal;
                if (btn.Name == "btnAddOrderQty")
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
        //增加或者减少LimitPrice
        private void btn_ChangeOrderLimitPrice(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button btn = (Button)sender;
            ConditionOrderField cof = (ConditionOrderField)btn.DataContext;
            InstrumentField instrument;
            Body.trdApi.DicInstrumentField.TryGetValue(cof.InstrumentID,out instrument);
            double tick =  instrument.PriceTick;
            double Price = 0;
            StackPanel panel = (StackPanel)btn.Parent;
            TextBox tbx = (TextBox)panel.FindName("tbxLimitPrice");
            if (tbx != null)
            {
                double.TryParse(tbx.Text, out Price);
                //Instrument instrument = btn.TemplatedParent.
                if (btn.Name == "btnIncOrderLimitPrice")
                {
                    Price += tick;
                }
                else if (btn.Name == "btnDecOrderLimitPrice") 
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
        //改变目标价
        private void btn_ChangeOrderTargetPrice(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button btn = (Button)sender;
            ConditionOrderField cof = (ConditionOrderField)btn.DataContext;
            InstrumentField instrument;
            Body.trdApi.DicInstrumentField.TryGetValue(cof.InstrumentID, out instrument);
            double tick = instrument.PriceTick;
            double Price = 0;
            StackPanel panel = (StackPanel)btn.Parent;
            TextBox tbx = (TextBox)panel.FindName("tbxTargetPrice");
            if (tbx != null)
            {
                double.TryParse(tbx.Text, out Price);
                //Instrument instrument = btn.TemplatedParent.
                if (btn.Name == "btnIncOrderTargetPrice")
                {
                    Price += tick;
                }
                else if (btn.Name == "btnDecOrderTargetPrice")
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

        //是否运行？
        private void chbIsRunning_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            CheckBox chb = (CheckBox)sender;
            ConditionOrderField cof = chb.DataContext as ConditionOrderField;
            cof.IsRunning = (bool)chb.IsChecked;

            switch (cof.ConOrderType)
            {
                case ConditionOrderType.价格单:
                    if (cof.IsRunning)
                    {
                        cof.TradeApi = Body.trdApi;
                        Body.mktApi.OnTick += cof.Run_PriceConditionOrder;
                    }
                    else
                    {
                        Body.mktApi.OnTick -= cof.Run_PriceConditionOrder;
                        cof.TradeApi = null;
                    }
                    break;
                case ConditionOrderType.时间单:
                    break;
                case ConditionOrderType.持仓单:
                    break;
                default:
                    break;
            }
        }

        //删除条件单板上显示的报单
        private void btnRemoveConditionOrder_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ConditionOrderField cof = (ConditionOrderField)btn.DataContext;
            
            //先解除外部绑定
            Body.mktApi.OnTick -= cof.Run_PriceConditionOrder;
            //再将运行状态设置为否
            cof.IsRunning = false;

            //从OC中删除cof
            if (cof == null || Body.OcAllConditionOrderField.Count <= 0)
            {
                return;
            }
            Body.OcAllConditionOrderField.Remove(cof);
            
            //GC
            cof = null;
        }
       
    }
}
