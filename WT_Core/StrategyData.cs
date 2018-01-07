using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Numeric = System.Double;

namespace WT_Core
{
    /// <summary>
    /// 策略中所用到的数据：Bars，Operations
    /// Strategy中有多个品种的数据Datas[]，对于每个Data都对应一个StrategyData（根据情况各自下单）
    /// </summary>
    public class StrategyData
    {
        private BarSeries _bars;

        /// <summary>
		/// 行情序列：Bars
		/// </summary>
		public BarSeries Bars { get {return this._bars; } }

        /// <summary>
		/// 报单操作列表：Operations
		/// </summary>
		[Description("报单操作列表"), Category("交易")]
        public List<OrderItem> Operations { get; private set; }

        /// <summary>
        /// 上一报单操作：如果Operations中有元素，则返回最后一个元素；反之，返回新实例。
        /// </summary>
        internal OrderItem lastOrder{get{ return Operations.Count > 0 ? Operations.Last() : new OrderItem();}}

        /// <summary>
        /// 构造器：将Bars赋值，并且初始化Operations
		/// </summary>
		/// <param name="bars">BarSeries指针</param>
		public StrategyData(BarSeries bars)
        {
            this._bars = bars;
            Operations = new List<OrderItem>();
        }

        #region 引用Bars的相关属性，在实际策略编写中引用的诸如Highs就是通过以下属性间接获得到Bars的Highs。

        /// <summary>
        /// 合约
        /// </summary>
        [Description("合约"), Category("设计")]
        public string InstrumentID { get { return this.Bars.Instrument; } }

        /// <summary>
        /// 周期数
        /// </summary>
        [Description("周期数"), Category("设计")]
        public int Interval { get { return this.Bars.Interval; } }

        /// <summary>
        /// 周期类型:0-tick(保留);1-秒(保留);2-分;3-时;4-天;5-周;6-月;7-年
        /// </summary>
        [Description("周期类型"), Category("设计")]
        public IntervalType IntervalType { get { return this.Bars.IntervalType; } }

        /// <summary>
        /// 当前K线索引(由左向右从0开始)
        /// </summary>
        [Description("当前K线索引"), Category("数据")]
        public int CurrentBarIndex { get { return this.Bars.CurrentBarIndex; } }

        /// <summary>
        /// 实际行情(无数据时为UpdateTime == null)
        /// </summary>
        [Description("分笔数据"), Category("数据")]
        public Tick Tick { get { return this.Bars.Tick; } }

        /// <summary>
        /// 合约信息
        /// </summary>
        public InstrumentInfo InstrumentInfo { get { return this.Bars.InstrumentInfo; } }

        /// <summary>
        /// 时间(yyyyMMdd.HHmmss)
        /// </summary>
        public NumericSeries NumDateTimes { get { return this.Bars.NumDateTimes; } }

        /// <summary>
        /// 最高价
        /// </summary>
        public NumericSeries Highs { get { return this.Bars.Highs; } }

        /// <summary>
        /// 最低价
        /// </summary>
        public NumericSeries Lows { get { return this.Bars.Lows; } }

        /// <summary>
        /// 开盘价
        /// </summary>
        public NumericSeries Opens { get { return this.Bars.Opens; } }

        /// <summary>
        /// 收盘价
        /// </summary>
        public NumericSeries Closes { get { return this.Bars.Closes; } }

        /// <summary>
        /// 成交量
        /// </summary>
        public NumericSeries Volumes { get { return this.Bars.Volumes; } }

        /// <summary>
        /// 持仓量
        /// </summary>
        public NumericSeries OpenInts { get { return this.Bars.OpenInts; } }

        #endregion

        #region 策略状态信息，受到this.Order影响

        /// <summary>
        /// 当前持仓手数:多
        /// </summary>
        [Description("当前持仓手数:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public int PositionLong { get { return this.lastOrder.PositionLong; } }

        /// <summary>
        /// 当前持仓手数:空
        /// </summary>
        [Description("当前持仓手数:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public int PositionShort { get { return this.lastOrder.PositionShort; } }

        /// <summary>
        /// 当前持仓手数:净
        /// </summary>
        [Description("当前持仓手数:净"), Category("状态"), ReadOnly(true), Browsable(false)]
        public int PositionNet { get { return this.lastOrder.PositionLong - this.lastOrder.PositionShort; } }

        /// <summary>
        /// 当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)
        /// </summary>
        [Description("当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric EntryDateLong { get { return this.lastOrder.EntryDateLong; } }

        /// <summary>
        /// 当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)
        /// </summary>
        [Description("当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric EntryDateShort { get { return this.lastOrder.EntryDateShort; } }

        /// <summary>
        /// 当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)
        /// </summary>
        [Description("当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric LastEntryDateLong { get { return this.lastOrder.LastEntryDateLong; } }

        /// <summary>
        /// 当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)
        /// </summary>
        [Description("当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric LastEntryDateShort { get { return this.lastOrder.LastEntryDateShort; } }

        /// <summary>
        /// 当前持仓首个建仓价格:多
        /// </summary>
        [Description("当前持仓首个建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric EntryPriceLong { get { return this.lastOrder.EntryPriceLong; } }

        /// <summary>
        /// 当前持仓首个建仓价格:空
        /// </summary>
        [Description("当前持仓首个建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric EntryPriceShort { get { return this.lastOrder.EntryPriceShort; } }

        /// <summary>
        /// 当前持仓最后建仓价格:多
        /// </summary>
        [Description("当前持仓最后建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric LastEntryPriceLong { get { return this.lastOrder.LastEntryPriceLong; } }

        /// <summary>
        /// 当前持仓最后建仓价格:空
        /// </summary>
        [Description("当前持仓最后建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric LastEntryPriceShort { get { return this.lastOrder.LastEntryPriceShort; } }

        /// <summary>
        /// 当前持仓平均建仓价格:多
        /// </summary>
        [Description("当前持仓平均建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric AvgEntryPriceLong { get { return this.lastOrder.AvgEntryPriceLong; } }

        /// <summary>
        /// 当前持仓平均建仓价格:空
        /// </summary>
        [Description("当前持仓平均建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric AvgEntryPriceShort { get { return this.lastOrder.AvgEntryPriceShort; } }

        /// <summary>
        /// 当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)
        /// </summary>
        [Description("当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceEntryLong { get { return this.Bars.CurrentBarIndex - this.lastOrder.IndexEntryLong; } }

        /// <summary>
        /// 当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)
        /// </summary>
        [Description("当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceEntryShort { get { return this.Bars.CurrentBarIndex - this.lastOrder.IndexEntryShort; } }

        /// <summary>
        /// 当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)
        /// </summary>
        [Description("当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceLastEntryLong { get { return this.Bars.CurrentBarIndex - this.lastOrder.IndexLastEntryLong; } }

        /// <summary>
        /// 当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)
        /// </summary>
        [Description("当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceLastEntryShort { get { return this.Bars.CurrentBarIndex - this.lastOrder.IndexLastEntryShort; } }

        /// <summary>
        /// 最近平仓位置到当前位置的Bar计数:多(从0开始计数)
        /// </summary>
        [Description("最近平仓位置到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceExitLong { get { return this.Bars.CurrentBarIndex - this.lastOrder.IndexExitLong; } }

        /// <summary>
        /// 最近平仓位置到当前位置的Bar计数:空(从0开始计数)
        /// </summary>
        [Description("最近平仓位置到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceExitShort { get { return this.Bars.CurrentBarIndex - this.lastOrder.IndexExitShort; } }

        /// <summary>
        /// 最近平仓时间:多(yyyyMMdd.HHmmss)
        /// </summary>
        [Description("平仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric ExitDateLong { get { return this.lastOrder.ExitDateLong; } }

        ///<summary>
        ///	最近平仓时间:空(yyyyMMdd.HHmmss)
        ///</summary>
        [Description("平仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric ExitDateShort { get { return this.lastOrder.ExitDateShort; } }

        /// <summary>
        /// 最近平仓价格:多
        /// </summary>
        [Description("平仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric ExitPriceLong { get { return this.lastOrder.ExitPriceLong; } }

        /// <summary>
        /// 最近平仓价格:空
        /// </summary>
        [Description("平仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric ExitPriceShort { get { return this.lastOrder.ExitPriceShort; } }

        /// <summary>
        /// 当前持仓浮动盈亏(点数):多
        /// </summary>
        [Description("浮动盈亏:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric PositionProfitLong { get { return this.Bars.Count == 0 ? 0 : ((this.Closes[0] - this.lastOrder.AvgEntryPriceLong) * this.lastOrder.PositionLong); } }

        /// <summary>
        /// 当前持仓浮动盈亏(点数):空
        /// </summary>
        [Description("浮动盈亏:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric PositionProfitShort { get { return this.Bars.Count == 0 ? 0 : ((this.lastOrder.AvgEntryPriceShort - this.Closes[0]) * this.lastOrder.PositionShort); } }

        /// <summary>
        /// 当前持仓浮动盈亏(点数):净
        /// </summary>
        [Description("浮动盈亏:净"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric PositionProfit { get { return this.PositionProfitLong + this.PositionProfitShort; } }

      
        #endregion

        /// <summary>
        /// 策略数据——报单：海风是在每次策略发出报单后，直接调用该函数；我个人认为应该在回调OnRtnTrade中使用更好，因为不可回滚。
        /// </summary>
        /// <param name="pDir"> </param>
        /// <param name="pOffset"> </param>
        /// <param name="pLots"> </param>
        /// <param name="pPrice"> </param>
        /// <param name="pRemark">注释</param>
        private void RecordOperation(Direction pDir, Offset pOffset, int pLots, Numeric pPrice, string pRemark)
        {
            OrderItem order;
            
            //Step 1：预处理
            if (this.Operations.Count == 0)
            {//若报单记录中元素数量为0，则新建OrderItem，但是没有last记录~
                order = new OrderItem
                {
                    Date = this.Bars[CurrentBarIndex].UpdateDateTime,
                    Dir = pDir,
                    Offset = pOffset,
                    Price = pPrice,
                    Lots = pLots,
                    Remark = pRemark
                };
            }
            else
            {//若报单记录中已经有元素，则新建OrderItem，其持仓等方面的属性沿用之前的orderitem的（预处理）
                order = new OrderItem
                {
                    Date = this.Bars[CurrentBarIndex].UpdateDateTime,
                    Dir = pDir,
                    Offset = pOffset,
                    Price = pPrice,
                    Lots = pLots,
                    Remark = pRemark,

                    IndexEntryLong = this.lastOrder.IndexEntryLong,
                    IndexExitLong = this.lastOrder.IndexExitLong,
                    IndexEntryShort = this.lastOrder.IndexEntryShort,
                    IndexExitShort = this.lastOrder.IndexExitShort,
                    IndexLastEntryLong = this.lastOrder.IndexLastEntryLong,
                    IndexLastEntryShort = this.lastOrder.IndexLastEntryShort,

                    AvgEntryPriceLong = this.lastOrder.AvgEntryPriceLong,
                    AvgEntryPriceShort = this.lastOrder.AvgEntryPriceShort,
                    PositionLong = this.lastOrder.PositionLong,
                    PositionShort = this.lastOrder.PositionShort,
                    EntryDateLong = this.lastOrder.EntryDateLong,
                    EntryDateShort = this.lastOrder.EntryDateShort,
                    EntryPriceLong = this.lastOrder.EntryPriceLong,
                    EntryPriceShort = this.lastOrder.EntryPriceShort,
                    ExitDateLong = this.lastOrder.ExitDateLong,
                    ExitDateShort = this.lastOrder.ExitDateShort,
                    ExitPriceLong = this.lastOrder.ExitPriceLong,
                    ExitPriceShort = this.lastOrder.ExitPriceShort,
                    LastEntryDateLong = this.lastOrder.LastEntryDateLong,
                    LastEntryDateShort = this.lastOrder.LastEntryDateShort,
                    LastEntryPriceLong = this.lastOrder.LastEntryPriceLong,
                    LastEntryPriceShort = this.lastOrder.LastEntryPriceShort
                };
            }

            //Step 2：根据上述修改，将新order的影响施加到OrderItem上
            switch (string.Format("{0}{1}", pDir, pOffset))
            {
                case "BuyOpen":
                    //开多或者追多
                    order.PositionLong += pLots;
                    //平均开多价格：点数
                    order.AvgEntryPriceLong = (this.lastOrder.PositionLong * this.lastOrder.AvgEntryPriceLong + pLots * pPrice) / order.PositionLong;
                    //如果之前没有多头仓位，则设置IndexEntryLong、EntryDateLong、EntryPriceLong为当前Bar的相关值
                    if (this.lastOrder.PositionLong == 0)
                    {
                        order.IndexEntryLong = this.CurrentBarIndex;
                        order.EntryDateLong = this.NumDateTimes[0];
                        order.EntryPriceLong = pPrice;
                    }
                    order.IndexLastEntryLong = this.CurrentBarIndex;
                    order.LastEntryDateLong = this.NumDateTimes[0];
                    order.LastEntryPriceLong = pPrice;
                    break;

                case "SellOpen":
                    order.PositionShort += pLots;
                    order.AvgEntryPriceShort = (this.lastOrder.PositionShort * this.lastOrder.AvgEntryPriceShort + pLots * pPrice) / order.PositionShort;
                    if (this.lastOrder.PositionShort == 0)
                    {
                        order.IndexEntryShort = this.CurrentBarIndex;
                        order.EntryDateShort = this.NumDateTimes[0];
                        order.EntryPriceShort = pPrice;
                    }
                    order.IndexLastEntryShort = this.CurrentBarIndex;   //最近一次开空的时间
                    order.LastEntryDateShort = this.NumDateTimes[0];
                    order.LastEntryPriceShort = pPrice;
                    break;

                case "BuyClose":
                    if (pLots > this.PositionShort) //买平数量大于空头持仓数量，则返回。
                        return;
                    order.PositionShort -= pLots;
                    order.IndexExitShort = this.CurrentBarIndex;
                    order.ExitDateShort = this.NumDateTimes[0];
                    order.ExitPriceShort = pPrice;
                    break;

                case "SellClose":
                    if (pLots > this.PositionLong) //卖平数量大于多头持仓数量，则返回。
                        return;
                    order.PositionLong -= pLots;
                    order.IndexExitLong = this.CurrentBarIndex;
                    order.ExitDateLong = this.NumDateTimes[0];
                    order.ExitPriceLong = pPrice;
                    break;
            }
            //添加到操作记录中
            this.Operations.Add(order);

            //if (_rtnOrder != null)
            //{
            //    _rtnOrder(order, this.Data);
            //}
        }

        /// <summary>
		/// 策略下单——开多仓：买开
		/// </summary>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		public void Buy(int pLots, Numeric pPrice, string pRemark = "")
        {
            OrderItemArgs oia = new OrderItemArgs();
            oia.OrderItem = new OrderItem { Date = Bars[CurrentBarIndex].UpdateDateTime,
                InstrumentID = this.InstrumentInfo.InstrumentID, Dir = Direction.Buy,
                Lots = pLots, Offset = Offset.Open, Price = pPrice, Remark = pRemark };
            
            //发出下单信号
            this._orderTrigger?.Invoke(this, oia);
            //记录
            this.RecordOperation(Direction.Buy, Offset.Open, pLots, pPrice, pRemark);
        }

        /// <summary>
		/// 平空仓：卖平
		/// </summary>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		public void Sell(int pLots, Numeric pPrice, string pRemark = "")
        {
            OrderItemArgs oia = new OrderItemArgs();
            oia.OrderItem = new OrderItem
            {
                Date = Bars[CurrentBarIndex].UpdateDateTime,
                InstrumentID = this.InstrumentInfo.InstrumentID,
                Dir = Direction.Sell,
                Lots = pLots,
                Offset = Offset.Close,
                Price = pPrice,
                Remark = pRemark
            };

            this._orderTrigger?.Invoke(this, oia);
            this.RecordOperation(Direction.Sell, Offset.Close, pLots, pPrice, pRemark);
        }

        /// <summary>
        /// 开空仓：卖开
        /// </summary>
        /// <param name="pLots"> </param>
        /// <param name="pPrice"> </param>
        /// <param name="pRemark">注释</param>
        public void SellShort(int pLots, Numeric pPrice, string pRemark = "")
        {
            OrderItemArgs oia = new OrderItemArgs();
            oia.OrderItem = new OrderItem
            {
                Date = Bars[CurrentBarIndex].UpdateDateTime,
                InstrumentID = this.InstrumentInfo.InstrumentID,
                Dir = Direction.Sell,
                Lots = pLots,
                Offset = Offset.Open,
                Price = pPrice,
                Remark = pRemark
            };

            this._orderTrigger?.Invoke(this,oia);
            this.RecordOperation(Direction.Sell, Offset.Open, pLots, pPrice, pRemark);
        }

        /// <summary>
        /// 平多仓：买平
        /// </summary>
        /// <param name="pLots"> </param>
        /// <param name="pPrice"> </param>
        /// <param name="pRemark">注释</param>
        public void BuyToCover(int pLots, Numeric pPrice, string pRemark = "")
        {
            OrderItemArgs oia = new OrderItemArgs();
            oia.OrderItem = new OrderItem
            {
                Date = Bars[CurrentBarIndex].UpdateDateTime,
                InstrumentID = this.InstrumentInfo.InstrumentID,
                Dir = Direction.Buy,
                Lots = pLots,
                Offset = Offset.Close,
                Price = pPrice,
                Remark = pRemark
            };
            this._orderTrigger?.Invoke(this, oia);
            this.RecordOperation(Direction.Buy, Offset.Close, pLots, pPrice, pRemark);
        }

        /// <summary>
        /// 发出下单信号
        /// </summary>
        /// <param name="pDir"></param>
        /// <param name="pOffset"></param>
        /// <param name="pLots"></param>
        /// <param name="pPrice"></param>
        /// <param name="pRemark"></param>
        public delegate void OrderTrigger(object sender, OrderItemArgs orderItemArgs);
        private OrderTrigger _orderTrigger;

        /// <summary>
        /// StrategyData中出现下单信号，由于Strategy中可能有多个StrategyData，所以在事件绑定时，
        /// 设计为为StrategyData层面上绑定Platform中SendOrder函数。
        /// </summary>
        public event OrderTrigger OnOrderTrigger
        {
            add
            {
                this._orderTrigger += value;
            }
            remove
            {
                this._orderTrigger -= value;
            }
        }

    }
}
