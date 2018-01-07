using System;
using System.ComponentModel;
using Numeric = System.Double;

namespace WT_Core
{
    /// <summary>
	/// 交易报单
	/// </summary>
	public class OrderItem
    {
        //在策略中被赋值
        internal int IndexEntryLong;
        internal int IndexLastEntryLong;
        internal int IndexExitShort;
        internal int IndexEntryShort;
        internal int IndexExitLong;
        internal int IndexLastEntryShort;

        /// <summary>
        /// 	时间
        /// </summary>
        [Description("时间:yyyyMMdd.HHmmss"), Category("字段"), ReadOnly(true)]
        public DateTime Date { get; set; }

        /// <summary>
        /// 合约代码
        /// </summary>
        [Description("合约"), Category("字段"), ReadOnly(true)]
        public string InstrumentID { get; set; }

        /// <summary>
        /// 	买卖
        /// </summary>
        [Description("买卖"), Category("字段"), ReadOnly(true)]
        public Direction Dir { get; set; }

        /// <summary>
        /// 	开平
        /// </summary>
        [Description("开平"), Category("字段"), ReadOnly(true)]
        public Offset Offset { get; set; }

        /// <summary>
        /// 	价格
        /// </summary>
        [Description("价格"), Category("字段"), ReadOnly(true)]
        public Numeric Price { get; set; }

        /// <summary>
        /// 	手数
        /// </summary>
        [Description("手数"), Category("字段"), ReadOnly(true)]
        public int Lots { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        [Description("说明"), Category("字段"), ReadOnly(true)]
        public string Remark { get; set; }

        /// <summary>
        /// 平均开空价格
        /// </summary>
        internal Numeric AvgEntryPriceShort { get; set; }

        /// <summary>
        /// 平均开多价格
        /// </summary>
        internal Numeric AvgEntryPriceLong { get; set; }

        /// <summary>
        /// 多头手数
        /// </summary>
        internal int PositionLong { get; set; }

        /// <summary>
        /// 空头手数
        /// </summary>
        internal int PositionShort { get; set; }

        /// <summary>
        /// 开多日期
        /// </summary>
        internal Numeric EntryDateLong { get; set; }

        /// <summary>
        /// 开多价格
        /// </summary>
        internal Numeric EntryPriceLong { get; set; }

        /// <summary>
        /// 平空日期
        /// </summary>
        internal Numeric ExitDateShort { get; set; }

        /// <summary>
        /// 平空价格
        /// </summary>
        internal Numeric ExitPriceShort { get; set; }

        /// <summary>
        /// 开空日期
        /// </summary>
        internal Numeric EntryDateShort { get; set; }

        /// <summary>
        /// 开空价格
        /// </summary>
        internal Numeric EntryPriceShort { get; set; }

        /// <summary>
        /// 平多日期
        /// </summary>
        internal Numeric ExitDateLong { get; set; }

        /// <summary>
        /// 平多价格
        /// </summary>
        internal Numeric ExitPriceLong { get; set; }

        /// <summary>
        /// 上一次开空日期
        /// </summary>
        internal Numeric LastEntryDateShort { get; set; }
        
        /// <summary>
        /// 上一次开空价格
        /// </summary>
        internal Numeric LastEntryPriceShort { get; set; }

        /// <summary>
        /// 上一次开多日期
        /// </summary>
        internal Numeric LastEntryDateLong { get; set; }

        /// <summary>
        /// 上一次开多价格
        /// </summary>
        internal Numeric LastEntryPriceLong { get; set; }

        /// <summary>
        /// OrderItem ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ( string.Join(",", new string[] { this.Date.ToString() ,this.Dir.ToString(), this.Offset.ToString(),this.Price.ToString(),this.Lots.ToString() }));
        }

    }
}
