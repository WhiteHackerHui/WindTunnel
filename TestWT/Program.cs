using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT_Core;
using WT_Strategy;
using WT_CTP;
using Numeric = System.Double;
using System.Diagnostics;
using WT_UI;
using WindTunnel;
using WindTunnel.Properties;

namespace TestWT
{
    class Program
    {
        static List<Tuple<DateTime, Numeric, Numeric, Numeric, Numeric, Numeric, Numeric>> CS_data = new List<Tuple<DateTime, Numeric, Numeric, Numeric, Numeric, Numeric, Numeric>>();
        static WT_CTP.CMdApi _mktApi = CTPFactory.CreateCMdApiInstance("062649", "12345", "tcp://180.168.146.187:10000", "tcp://180.168.146.187:10010");

        [STAThread]
        static void Main(string[] args)
        {
            //读取数据
            using (StreamReader r = new StreamReader("cs888(1分钟).csv"))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    string[] tmp = line.Split(',');
                    DateTime dt = DateTime.Parse(tmp[0]);
                    Numeric open = Numeric.Parse(tmp[1]);
                    Numeric high = Numeric.Parse(tmp[2]);
                    Numeric low = Numeric.Parse(tmp[3]);
                    Numeric close = Numeric.Parse(tmp[4]);
                    Numeric volume = Numeric.Parse(tmp[5]);
                    Numeric openInt = Numeric.Parse(tmp[6]);

                    CS_data.Add(new Tuple<DateTime, Numeric, Numeric, Numeric, Numeric, Numeric, Numeric>(dt, open, high, low, close, volume, openInt));
                }
            }

            //传入历史Bar数据
            BarSeries CS888_1min_Bars = new BarSeries();
            CS888_1min_Bars.IntervalType = IntervalType.Min;
            CS888_1min_Bars.Interval = 1;
            CS888_1min_Bars.InstrumentInfo = new InstrumentInfo { InstrumentID = "au888", PriceTick = 0.05, VolumeMultiple = 1000, ProductID = "" };
            


            //构造Strategy1实例
            AppADX stra = new AppADX();
            stra.LoadBarSerieses(CS888_1min_Bars);

            //播放Bar数据
            for (int i = 0; i < CS_data.Count; i++)
            {
                var p = CS_data[i];
                Bar curBar = new Bar { UpdateDateTime = p.Item1, Open = p.Item2, High = p.Item3, Low = p.Item4, Close = p.Item5,
                    Volume = p.Item6, OpenInt = p.Item7 };
                //增加Bar的同时，引发了Bar、Indicator和Strategy更新
                CS888_1min_Bars.Add(curBar);
                stra.UpdateStrategy();
                
            }

            CS888_1min_Bars.Instrument = "cs1706";
            CS888_1min_Bars.Interval = 1;
            CS888_1min_Bars.IntervalType = IntervalType.Min;

            //回调绑定
            _mktApi.OnRtnTick += CS888_1min_Bars.OnRtnTick;

            //登录
            _mktApi.Connect();
            _mktApi.Login();
            Debug.WriteLine(_mktApi.ConnStatus);
            _mktApi.ReqSubscribeMarketData("cs1706");

            //FormWorkSpace fws = new FormWorkSpace(stra);
            //fws.ShowDialog();

            //App app = new App();
            //app.InitializeComponent();
            //app.Run();

            for (int i = 0; i < stra.Operations.Count; i++)
            {
                OrderItem oi = stra.Operations[i];
                if (i % 2 == 0)
                {
                    stra.sb.Append(oi.ToString());
                }
                else
                {
                    stra.sb.AppendLine(","+ oi.ToString());
                }
            }

            System.IO.File.WriteAllText("straData.csv", stra.sb.ToString());



        }

   

    }
}
