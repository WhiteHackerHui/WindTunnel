using WT_Core;
using Numeric = System.Double;

namespace WT_Strategy.Indicators
{
    internal class MACD : Indicator
    {
        private readonly Numeric _fast, _slow, _smooth;
        //private readonly DataSeries fastEma = new DataSeries();
        //private readonly DataSeries slowEma = new DataSeries();

        //输出序列
        public readonly NumericSeries Fast, Slow, DIF, DEA;

        public MACD(NumericSeries input, int fast, int slow, int smooth) : base(input, fast, slow, smooth)
        {
            this._fast = fast;
            this._slow = slow;
            this._smooth = smooth;
        }

        public override void OnBarUpdate()
        {
            if (CurrentBarIndex == 0)
            {
                this.Fast[0] = Input[0];
                this.Slow[0] = Input[0];
                Value[0] = 0;
                this.Fast[0] = 0;
                this.Slow[0] = 0;
            }
            else
            {
                this.Fast[0] = (2 / (1 + this._fast)) * Input[0] + (1 - (2 / (1 + this._fast))) * this.Fast[1];
                this.Slow[0] = (2 / (1 + this._slow)) * Input[0] + (1 - (2 / (1 + this._slow))) * this.Slow[1];
                this.DIF[0] = this.Fast[0] - this.Slow[0];
                this.DEA[0] = (2 / (1 + this._smooth)) * this.DIF[0] + (1 - (2 / (1 + this._smooth))) * this.DEA[1];
                Value[0] = (DIF[0] - DEA[0]) * 2;
            }
        }
    }
}
