using System;
using WT_Core;
using Numeric = System.Double;

namespace WT_Strategy.Indicators
{
    internal class ATR : Indicator
    {
        //不要嵌套Indicator，会引发异常值出现
        public readonly NumericSeries TRs;

        public ATR(int period) : base(null, period)
        {
        }

        public override void OnBarUpdate()
        {
            if (CurrentBarIndex == 0)
            {
                TRs[0] = Highs[0] - Lows[0];
               
            }
            else
            {
                TRs[0] = Math.Max(Math.Max(Highs[0] - Lows[0], Math.Abs(Closes[1] - Highs[0])), Math.Abs(Closes[1] - Lows[0]));
            }
            Value[0] = Functions.Average(TRs,Period);
        }
    }
}
