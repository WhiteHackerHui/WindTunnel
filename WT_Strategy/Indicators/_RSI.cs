using System;
using WT_Core;
using Numeric = System.Double;

namespace WT_Strategy.Indicators
{
    internal class RSI : Indicator
    {
        private NumericSeries NetChgAvg, TotChagAvg;
        private Numeric SF, Change, ChgRatio;

        public RSI(int period) : base(null, period)
        {
        }

        public override void OnBarUpdate()
        {
            if (CurrentBarIndex <= Period - 1 )
            {
                NetChgAvg[0] = (Closes[0] - Closes[Period]) / Period;
                TotChagAvg[0] = (Closes[0] - Closes[1]) / Period;
            }
            else
            {
                SF = 1.0 / Period;
                Change = Closes[0] - Closes[1];
                NetChgAvg[0] = NetChgAvg[1] + SF * (Change - NetChgAvg[1]);
                TotChagAvg[0] = TotChagAvg[1] + SF * (Math.Abs(Change) - TotChagAvg[1]);
            }

            if (TotChagAvg[0] != 0)
            {
                ChgRatio = NetChgAvg[0] / TotChagAvg[0];
            }
            else
            {
                ChgRatio = 0;
            }

            Value[0] = 50 * (ChgRatio + 1);
        }
    }
}
