using WT_Core;

namespace WT_Strategy.Indicators
{
    internal class ADL:Indicator
    {
        public ADL() : base(null, 0)
        {
        }
 
        public override void OnBarUpdate()
        {
            Value[0] = (CurrentBarIndex == 0 ? 0 : Value[1]) + (Highs[0] != Lows[0] ? (((Closes[0] - Lows[0]) - (Highs[0] - Closes[0])) / (Highs[0] - Lows[0])) * Volumes[0] : 0);
        }
    }
}
