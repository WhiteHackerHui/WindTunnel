using System;
using System.ComponentModel;
using Numeric = System.Double;

namespace WT_Core
{
    //K线类型
    public class Bar
    {
        /// <summary>
		/// 	更新时间
		/// </summary>
		[Description("更新时间") Category("字段"), ReadOnly(true)]
        public DateTime UpdateDateTime { get; set; }

        /// <summary>
        /// 	开盘价
        /// </summary>
        [Description("开盘价"), Category("字段"), ReadOnly(true)]
        public Numeric Open { get; set; }

        /// <summary>
        /// 	最高价
        /// </summary>
        [Description("最高价"), Category("字段"), ReadOnly(true)]
        public Numeric High { get; set; }

        /// <summary>
        /// 	最低价
        /// </summary>
        [Description("最低价"), Category("字段"), ReadOnly(true)]
        public Numeric Low { get; set; }

        /// <summary>
        /// 	收盘价
        /// </summary>
        [Description("收盘价"), Category("字段"), ReadOnly(true)]
        public Numeric Close { get; set; }

        /// <summary>
        /// 	成交量
        /// </summary>
        [Description("成交量"), Category("字段"), ReadOnly(true)]
        public Numeric Volume { get; set; }

        /// <summary>
        /// 	持仓量
        /// </summary>
        [Description("持仓量"), Category("字段"), ReadOnly(true)]
        public Numeric OpenInt { get; set; }

    }
}
