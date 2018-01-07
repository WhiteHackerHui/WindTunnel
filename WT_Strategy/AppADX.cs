using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT_Core;
using WT_Strategy.Indicators;
using Numeric = System.Double;

namespace WT_Strategy
{
    /// <summary>
    /// ADX策略
    /// </summary>
    public sealed class AppADX:Strategy
    {
        [Parameter("手数")]
        int lots = 1;
        [Parameter("ADX平滑数")]
        int M = 6;
        [Parameter("开仓阈值")]
        Numeric AdxValue = 45;
        [Parameter("止损百分比")]
        Numeric MaxLoss = 1;
        [Parameter("止盈点数")]
        Numeric SS = 45;
        [Parameter("滑点")]
        int i_offset = 1;   

        private ADX adx1 = new ADX(14);
        bool ADXCON = false;
        Numeric OffsetPoint = 0;
        Numeric SADXR;
        Numeric MyEntryPrice, MyExitPrice;
        NumericSeries HighestAfterEntry,LowestAfterEntry;
        //DateTime filtTime = new DateTime(2014, 12, 22,11,0,0);
        public StringBuilder sb = new StringBuilder();

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
        }

        /// <summary>
        /// 策略更新部分
        /// </summary>
        public override void OnBarUpdate()
        {
            //MarketPosition在之前就已经更新了，所以之后所有的操作都不再影响到MarketPosition的值
            this.SADXR = Functions.Average(adx1.Value, M);
            this.ADXCON = SADXR > AdxValue;
            this.OffsetPoint = i_offset * MinMove * PriceScale;

            //if (this.Date[0] + this.Time[0] >= Numeric.Parse(filtTime.ToString("yyyyMMdd.HHmmss")))
            //{
            
            //买开
            if (adx1.PDIs[0] > adx1.MDIs[0] && ADXCON && Highs[0] >= Highs[1] && PositionNet<=0)
            {
                MyEntryPrice = Closes[0] + OffsetPoint;
                Buy(lots, MyEntryPrice,Name);
            }

            //卖开
            if (adx1.PDIs[0] < adx1.MDIs[0] && ADXCON && Lows[0] <= Lows[1] && PositionNet>=0)
            {
                MyEntryPrice = Closes[0] - OffsetPoint;
                SellShort(lots, MyEntryPrice,Name);
            }

            //卖平止损
            if (MarketPosition == 1 && Lows[0] < EntryPrice * (1 - MaxLoss / 100) && BarsSinceEntry > 0)
            {
                MyExitPrice = Closes[0] - OffsetPoint; //Math.Min(Opens[0], EntryPrice * (1 - MaxLoss / 100));
                Sell(lots, MyExitPrice,Name);
            }

            //买平止损
            if (MarketPosition == -1 && Highs[0] > EntryPrice * (1 + MaxLoss / 100) && BarsSinceEntry > 0)
            {
                MyExitPrice = Closes[0] + OffsetPoint; //Math.Max(Opens[0], EntryPrice * (1 + MaxLoss / 100));
                BuyToCover(lots, MyExitPrice + OffsetPoint,Name);
            }

            //对于开仓的那个K线进行记录最高价和最低价
            if (BarsSinceEntry == 0 )
            {
                HighestAfterEntry[0] = Closes[0];
                LowestAfterEntry[0] = Closes[0];
                if (PositionNet != 0)
                {
                    HighestAfterEntry[0] = Math.Max(HighestAfterEntry[1], MyEntryPrice);
                    LowestAfterEntry[0] = Math.Min(LowestAfterEntry[1], MyEntryPrice);
                }
            }
            else
            {
                HighestAfterEntry[0] = Math.Max(HighestAfterEntry[1], Highs[0]);
                LowestAfterEntry[0] = Math.Min(LowestAfterEntry[1], Lows[0]);
            }

            //卖平止盈
            if (BarsSinceEntry > 0 && MarketPosition == 1 && Lows[0] <= HighestAfterEntry[1] * (1 - SS / 1000))
            {
                Numeric StopLine = HighestAfterEntry[1] * (1 - SS / 1000);
                if (Lows[0] <= StopLine)
                {
                    MyExitPrice = Closes[0] - OffsetPoint; //Math.Min(Opens[0], StopLine);
                    Sell(lots, MyExitPrice);
                }
            }

            //买平止盈
            if (BarsSinceEntry > 0 && MarketPosition == -1 && Highs[0] >= LowestAfterEntry[1] * (1 + SS / 1000))
            {
                Numeric StopLine = LowestAfterEntry[1] * (1 + SS / 1000);
                if (Highs[0] >= StopLine)
                {
                    MyExitPrice = Closes[0] + OffsetPoint; //Math.Max(Opens[0], StopLine);
                    BuyToCover(lots, MyExitPrice);
                }
            }

        }
    }
}
