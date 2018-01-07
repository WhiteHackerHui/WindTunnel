using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WindTunnel
{
    partial class myResourceDictionary:ResourceDictionary
    {
        //增加或者减少Order的手数
        private void btn_ChangeOrderQty(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Order order = (Order)btn.DataContext;
            StackPanel panel = (StackPanel)btn.Parent;
            TextBox tbx = (TextBox)panel.FindName("tbxQty");
            if (tbx != null)
            {
                double Qty = order.Qty;
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
                order.Qty = Qty;
                tbx.Text = Qty.ToString();
            }
            e.Handled = true;
        }

        //增加或者减少Price
        private void btn_ChangeOrderPrice(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Order order = (Order)btn.DataContext;
            double tick = Body.instruments[order.InstrumentID].PriceTick;
            double Price = 0;
            StackPanel panel = (StackPanel)btn.Parent;
            TextBox tbx = (TextBox)panel.FindName("tbxPrice");
            if (tbx!=null)
            {
                double.TryParse(tbx.Text, out Price);
                //Instrument instrument = btn.TemplatedParent.
                if (btn.Name == "btnIncOrderPrice")
                {
                    Price += tick;
                }
                else
                {
                    if (Price > 0)
                    {
                        Price -= tick;
                    }
                }
                order.Price = Price;
                tbx.Text = Price.ToString();
            }
            e.Handled = true;
        }

        //删除下单板上显示的报单
        private void btnRemoveOrderToInsert_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Order orderToDelete = (Order)btn.DataContext;
            if (orderToDelete == null || Body.OrdersToInsert.Count <= 0)
            {
                return;
            }

            //找出是否已经有该合约代码在OrdersToInsert中
            int index = -1;
            for (int i = 0; i < Body.OrdersToInsert.Count; i++)
            {
                if (orderToDelete._orderField.Equals(Body.OrdersToInsert[i]._orderField))
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
                Body.OrdersToInsert.RemoveAt(index);
        }

        //将下单板上显示的当前报单Send出去
        private void btnAddOrderToInsert_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Order orderToSend = btn.DataContext as Order; // new Order( (btn.DataContext as Order)._orderField);
            Body.SendOrder(orderToSend);
        }
    }
}
