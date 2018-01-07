using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WindTunnel
{
    //显示行号
    public class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (ListViewItem)value;
            var listview = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listview.ItemContainerGenerator.IndexFromContainer(item) + 1;
            return index.ToString()+".";
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //负数显示为绿色，正数显示为正数
    public class DataColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double tempValue = 0;
            double.TryParse((string)value, out tempValue);
            if (tempValue <= 0)
            {
                return "Green";
            }
            else
            {
                return "Red";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (string)value;
        }
    }

}
