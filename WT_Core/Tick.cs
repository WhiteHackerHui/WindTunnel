using System;
using System.Runtime.InteropServices;
using Numeric = System.Double;

namespace WT_Core
{
    //Tick数据，深度行情数据的子集
    [StructLayout(LayoutKind.Sequential)]
    public class Tick
    {
        ///// <summary>
        ///// 更新时间（日期型），暂时设想每次有深度行情数据返回时，构造下这个时间类型实例。By wh_wing
        ///// </summary>
        //public DateTime UpdateDateTime;

        /// <summary>
		/// 合约代码
		/// </summary>
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string InstrumentID { get; set; }
        
        /// <summary>
        /// 最新价
        /// </summary>
        public Numeric LastPrice { get; set; }
        /// <summary>
        ///申买价一
        /// </summary>
        public Numeric BidPrice { get; set; }
        /// <summary>
        ///申买量一
        /// </summary>
        public int BidVolume { get; set; }
        /// <summary>
        ///申卖价一
        /// </summary>
        public Numeric AskPrice { get; set; }
        /// <summary>
        ///申卖量一
        /// </summary>
        public int AskVolume { get; set; }
        /// <summary>
        ///成交量
        /// </summary>
        public int Volume { get; set; }
        /// <summary>
        ///持仓量
        /// </summary>
        public Numeric OpenInt { get; set; }
        /// <summary>
        ///最后修改时间:yyyyMMdd HH:mm:ss(20141114:日期由主程序处理,因大商所取到的actionday==tradingday)
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string UpdateTime { get; set; }
        /// <summary>
        ///最后修改毫秒
        /// </summary>
        public int UpdateMillisec { get; set; }
        ///// <summary>
        /////涨停板价
        ///// </summary>
        //public Numeric UpperLimitPrice;
        ///// <summary>
        /////跌停板价
        ///// </summary>
        //public Numeric LowerLimitPrice;
    }
}
