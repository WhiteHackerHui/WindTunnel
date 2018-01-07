using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Numeric = System.Double;

namespace WT_Core
{
    //Bar的集合，海风中称之为Data
    public class BarSeries:Collection<Bar>
    {
        //变量
        private string _instrument = string.Empty;
        private IntervalType _intervalType = IntervalType.Min;
        private int _interval = 15;
        private CollectionChange _onChange;
        private LastBarComplete _onLastBarComplete;

        /// <summary>
        /// 策略变化:加1;减-1;更新0
        /// </summary>
        public event CollectionChange OnChanged { add { _onChange += value; } remove { _onChange -= value; }}

        /// <summary>
        /// 上一个Bar已经完成更新
        /// </summary>
        public event LastBarComplete OnLastBarComplete { add { _onLastBarComplete += value; } remove { _onLastBarComplete -= value; }   }

        #region 数据序列：NumDateTimes、Opens、Highs、Lows、Closes、Volumes、OpenInts

        /// <summary>
        /// 时间(海风: yyyyMMdd.HHmmss)
        /// </summary>
        public NumericSeries NumDateTimes = new NumericSeries();

        /// <summary>
        /// 开盘价
        /// </summary>
        public NumericSeries Opens = new NumericSeries();

        /// <summary>
        /// 最高价
        /// </summary>
        public NumericSeries Highs = new NumericSeries();

        /// <summary>
        /// 最低价
        /// </summary>
        public NumericSeries Lows = new NumericSeries();

        /// <summary>
        /// 收盘价
        /// </summary>
        public NumericSeries Closes = new NumericSeries();

        /// <summary>
        /// 成交量
        /// </summary>
        public NumericSeries Volumes = new NumericSeries();

        /// <summary>
        /// 持仓量
        /// </summary>
        public NumericSeries OpenInts = new NumericSeries();

        #endregion

        /// <summary>
		/// 构造函数
		/// </summary>
		public BarSeries()
        {
            this.Tick = new Tick();
            //this.InstrumentInfo = new InstrumentField();
        }

        /// <summary>
		/// 	实际行情(无数据时为Instrument== null)
		/// </summary>
		[Description("分笔数据"), Category("数据"), Browsable(false)]
        public Tick Tick { get; set; }

        /// <summary>
		/// 合约
		/// </summary>
		[Description("合约"), Category("配置")]
        public string Instrument
        {
            get { return this._instrument; }
            set { this._instrument = value; }
        }

        /// <summary>
		/// 合约信息
		/// </summary>
		[Description("合约信息"), Category("数据"), Browsable(false)]
        public InstrumentInfo InstrumentInfo { get; set; }

        /// <summary>
		/// 周期数
		/// </summary>
		[Description("周期数"), Category("配置")]
        public int Interval { get { return this._interval; } set { this._interval = value; } }

        /// <summary>
		/// 周期类型
		/// </summary>
		[Description("周期类型"), Category("配置")]
        public IntervalType IntervalType { get { return this._intervalType; } set { this._intervalType = value; } }

        /// <summary>
		/// 当前K线索引(由左向右从0开始)，即当前K线在Bars中的顺序位置
		/// </summary>
		[Description("当前K线索引"), Category("设计"), Browsable(false)]
        public int CurrentBarIndex { get { return Count == 0 ? 0 : (Count - 1); } }

        /// <summary>
		/// Bar的名字：合约_周期数_周期类型
		/// </summary>
		public string Name { get { return this.Instrument + "_" + this.Interval + "_" + this.IntervalType; } }

        /// <summary>
        /// 通过Tick来更新Bar或者生成新Bar
        /// </summary>
        /// <param name="curTick"></param>
        /// <exception cref="Exception"></exception>
        public void OnRtnTick(Tick curTick)
        {
            //判断curTick是不是Bars中对应的合约
            if (this.InstrumentInfo.InstrumentID != curTick.InstrumentID)
                return;
            
            //Bar的UpdateDateTime就是dtBegin
            DateTime dt = DateTime.ParseExact(curTick.UpdateTime, "yyyyMMdd HH:mm:ss", null);
            DateTime dtBegin = dt.Date;
 
            #region 根据周期类型，得到每个时刻的开始时间
            switch (this.IntervalType)
            {
                case IntervalType.Sec:
                    dtBegin = dtBegin.Date.AddHours(dt.Hour).AddMinutes(dt.Minute).AddSeconds(dt.Second / Interval * Interval);
                    break;
                case IntervalType.Min:
                    dtBegin = dtBegin.Date.AddHours(dt.Hour).AddMinutes(dt.Minute / Interval * Interval);
                    break;
                case IntervalType.Hour:
                    dtBegin = dtBegin.Date.AddHours(dt.Hour / Interval * Interval);
                    break;
                case IntervalType.Day:
                    dtBegin = dtBegin.Date;
                    break;
                case IntervalType.Week:
                    dtBegin = dtBegin.Date.AddDays(1 - (byte)dtBegin.DayOfWeek);
                    break;
                case IntervalType.Month:
                    dtBegin = new DateTime(dtBegin.Year, dtBegin.Month, 1);
                    break;
                case IntervalType.Year:
                    dtBegin = new DateTime(dtBegin.Year, 1, 1);
                    break;
                default:
                    throw new Exception("参数错误");
            }
            #endregion 根据周期类型，得到每个时刻的开始时间

            #region 生成或者更新K线
            if (base.Count == 0)
            {
                Bar bar = new Bar
                {
                    UpdateDateTime = dtBegin,
                    OpenInt = curTick.OpenInt
                };
                bar.High = bar.Low = bar.Open = bar.Close = curTick.LastPrice;
                bar.Volume = curTick.Volume;
                Add(bar);
            }
            else
            {
                Bar bar = base[CurrentBarIndex]; 
                if (bar.UpdateDateTime == dtBegin) //在当前K线范围内
                {
                    bar.High = Math.Max(bar.High, curTick.LastPrice);
                    bar.Low = Math.Min(bar.Low, curTick.LastPrice);
                    bar.Close = curTick.LastPrice;
                    bar.OpenInt = curTick.OpenInt;
                }
                else if (dtBegin > bar.UpdateDateTime)   //新建K线
                {
                    //更新前Bar的Volume
                    base[CurrentBarIndex].Volume = curTick.Volume - base[CurrentBarIndex].Volume; //海风提示，此处可能有问题
                    //通知上一个Bar已经更新完毕了
                    this._onLastBarComplete?.Invoke();
                    //生成新的Bar
                    Bar newBar = new Bar
                    {
                        UpdateDateTime = dtBegin,
                        OpenInt = curTick.OpenInt,
                    };
                    newBar.High = newBar.Low = newBar.Open = newBar.Close = curTick.LastPrice;
                    newBar.Volume = curTick.Volume;
                    Add(newBar);
                }
            }
            #endregion 生成或者更新K线
            
            //更新最后的tick
            this.Tick = curTick; 
        }

        /// <summary>
		/// 当有新的Bar加入时，更新Bars中的序列：Closes
		/// </summary>
		/// <param name="index"></param>
		/// <param name="bar"></param>
		protected override void InsertItem(int index, Bar bar)
        {
            //序列更新
            this.NumDateTimes.Add(Numeric.Parse(bar.UpdateDateTime.ToString("yyyyMMdd.HHmmss")));
            this.Opens.Add(bar.Open);
            this.Highs.Add(bar.High);
            this.Lows.Add(bar.Low);
            this.Closes.Add(bar.Close);
            this.Volumes.Add(bar.Volume);
            this.OpenInts.Add(bar.OpenInt);
            //添加Bar
            base.InsertItem(index, bar);
            //通知增加Bar
            this._onChange?.Invoke(1, bar, null);
        }

        /// <summary>
		/// 根据逆序来设置SetItem
		/// </summary>
		/// <param name="reverseIndex">逆序</param>
		/// <param name="bar"></param>
        protected override void SetItem(int reverseIndex, Bar bar)
        {
            Bar old = this[reverseIndex];
            this.Highs[reverseIndex] = bar.High;
            this.Lows[reverseIndex] = bar.Low;
            this.Closes[reverseIndex] = bar.Close;
            this.Volumes[reverseIndex] = bar.Volume;
            this.OpenInts[reverseIndex] = bar.OpenInt;
            base.SetItem(CurrentBarIndex - reverseIndex, bar);
            //通知更新Bar
            this._onChange?.Invoke(0, bar, old);
        }

        /// <summary>
        /// 删除Bars中指定位置index（从后往前第index个）的Bar
        /// </summary>
        /// <param name="reverseIndex">逆序</param>
        protected override void RemoveItem(int reverseIndex)
        {
            Bar old = this[reverseIndex];
            if (this.NumDateTimes.Count == Count)
            {
                this.NumDateTimes.RemoveAt(CurrentBarIndex - reverseIndex);
                this.Opens.RemoveAt(CurrentBarIndex - reverseIndex);
                this.Highs.RemoveAt(CurrentBarIndex - reverseIndex);
                this.Lows.RemoveAt(CurrentBarIndex - reverseIndex);
                this.Closes.RemoveAt(CurrentBarIndex - reverseIndex);
                this.Volumes.RemoveAt(CurrentBarIndex - reverseIndex);
                this.OpenInts.RemoveAt(CurrentBarIndex - reverseIndex);
            }
            base.RemoveItem(CurrentBarIndex - reverseIndex);
            //通知删除Bar
            this._onChange?.Invoke(-1, null, old);
        }
        
        /// <summary>
        /// 清除Bars
        /// </summary>
        protected override void ClearItems()
        {
            this.NumDateTimes.Clear();
            this.Opens.Clear();
            this.Highs.Clear();
            this.Lows.Clear();
            this.Closes.Clear();
            this.Volumes.Clear();
            this.OpenInts.Clear();
            base.ClearItems();
        }

    }
}
