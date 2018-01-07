using System;
using WT_Core;
using Numeric = System.Double;

namespace WT_Strategy.Indicators
{
    internal class ADX : Indicator
    {
        private NumericSeries DMPs,DMMs;
        private NumericSeries avgDMP,avgDMM;
        public NumericSeries PDIs, MDIs;

        public NumericSeries TRs,ATRs,HDs,LDs;

        public ADX(int period) : base(null, period)
        {
        }

        public override void OnBarUpdate()
        {
            Numeric trueRange = Highs[0] - Lows[0];
            int N = (int)Periods[0];
            Numeric sFactor = 2.0 / (N + 1);

            if (CurrentBarIndex == 0)
            {
                TRs[0] = Highs[0] - Lows[0];
                ATRs[0] = Functions.Average(TRs, Period);
                HDs[0] = Highs[1] - Highs[2];
                LDs[0] = Lows[2] - Lows[1];
                DMPs[0] = (HDs[0] > 0 && HDs[0] > LDs[0]) ? HDs[0] : 0;
                this.avgDMP[0] = DMPs[0];
                DMMs[0] = (LDs[0] > 0 && LDs[0] > HDs[0]) ? LDs[0] : 0;
                this.avgDMM[0] = DMMs[0];
            }
            else
            {
                TRs[0] = Math.Max(Math.Max(Highs[0] - Lows[0], Math.Abs(Closes[1] - Highs[0])), Math.Abs(Closes[1] - Lows[0]));
                ATRs[0] = Functions.Average(TRs, Period);
                HDs[0] = Highs[1] - Highs[2];
                LDs[0] = Lows[2] - Lows[1];
                DMPs[0] = (HDs[0] > 0 && HDs[0] > LDs[0]) ? HDs[0] : 0;
                this.avgDMP[0] = this.avgDMP[1] + sFactor * (this.DMPs[0] - this.avgDMP[1]);
                DMMs[0] = (LDs[0] > 0 && LDs[0] > HDs[0]) ? LDs[0] : 0;
                this.avgDMM[0] = this.avgDMM[1] + sFactor * (this.DMMs[0] - this.avgDMM[1]);
            }

            PDIs[0] = avgDMP[0] * 100 / ATRs[1];
            MDIs[0] = avgDMM[0] * 100 / ATRs[1];
            Value[0] = Math.Abs(MDIs[0] - PDIs[0]) / (MDIs[0] + PDIs[0]) * 100;
        }
    }

}
