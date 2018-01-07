using System;
using WT_Core;
using Numeric = System.Double;

namespace WT_Strategy.Indicators
{
    internal class DEM : Indicator
    {
        private NumericSeries _FZ, _FM;

        public DEM() : base(null)
        {
        }

        public override void OnBarUpdate()
        {
            _FZ[0] = Math.Max(Highs[0] - Closes[1], 0) + Closes[0] - Lows[0];
            _FM[0] = Math.Max(Closes[1] - Lows[0], 0) + Highs[0] - Closes[0] + _FZ[0];
            Value[0] = Functions.Summation(_FZ, 8) / Functions.Summation(_FM, 8) * 100;         
        }
    }
}
