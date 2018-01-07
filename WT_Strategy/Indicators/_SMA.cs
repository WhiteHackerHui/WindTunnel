using System;
using WT_Core;
using Numeric = System.Double;

namespace WT_Strategy.Indicators
{
    public class SMA : Indicator
    {
        public SMA(int period)
            : base(null, period)
        {
        }

        public SMA(NumericSeries input, int period)
            : base(input, period)
        {
        }

        public override void OnBarUpdate()
        {
            if (CurrentBarIndex == 0)
            {
                Value[0] = Input[0];
            }
            else
            {
                Numeric last = Value[1] * Math.Min(CurrentBarIndex, Period);
                if (CurrentBarIndex >= Period)
                {
                    Value[0] = (last + Input[0] - Input[Period]) / Math.Min(CurrentBarIndex, Period);
                }
                else
                {
                    Value[0] = (last + Input[0]) / (Math.Min(CurrentBarIndex, Period) + 1);
                }
            }
        }
    }
}
