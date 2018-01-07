using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WT_Core
{
    /// <summary>
    /// 时间类型
    /// </summary>
    public enum IntervalType
    {
        /// <summary>
        /// 秒
        /// </summary>
        Sec,

        /// <summary>
        /// 分
        /// </summary>
        Min,

        /// <summary>
        /// 时
        /// </summary>
        Hour,

        /// <summary>
        /// 日
        /// </summary>
        Day,

        /// <summary>
        /// 周
        /// </summary>
        Week,

        /// <summary>
        /// 月
        /// </summary>
        Month,

        /// <summary>
        /// 年
        /// </summary>
        Year,
    }

    /// <summary>
	/// 买卖方向
	/// </summary>
	public enum Direction
    {
        /// <summary>
        /// 买
        /// </summary>
        Buy,

        /// <summary>
        /// 卖
        /// </summary>
        Sell
    }

    /// <summary>
    /// 开平
    /// </summary>
    public enum Offset
    {
        /// <summary>
        /// 开仓
        /// </summary>
        Open = 0,
        /// <summary>
        /// 平仓
        /// </summary>
        Close = 1,
        /// <summary>
        /// 强平
        ///</summary>
        ForceClose = 2,
        /// <summary>
        /// 平今
        ///</summary>
        CloseToday = 3,
        /// <summary>
        /// 平昨
        ///</summary>
        CloseYesterday = 4,
        /// <summary>
        /// 强减
        ///</summary>
        ForceOff = 5,
        /// <summary>
        /// 本地强平
        ///</summary>
        LocalForceClose = 6
    }

    /// <summary>
    /// 价格类型：H，L，O，C，V，I
    /// </summary>
    public enum PriceType
    {
        /// <summary>
        /// 
        /// </summary>
        H,
        /// <summary>
        /// 
        /// </summary>
        L,
        /// <summary>
        /// 
        /// </summary>
        O,
        /// <summary>
        /// 
        /// </summary>
        C,
        /// <summary>
        /// 
        /// </summary>
        V,
        /// <summary>
        /// 
        /// </summary>
        I
    }

    /// <summary>
    /// 周期类型：（非Bar）Tick,Second,Minute,Hour,Day,Week,Month,Year
    /// </summary>
    public enum PeriodType
    {
        /// <summary>
        /// 笔
        /// </summary>
        Tick,
        /// <summary>
        /// 秒
        /// </summary>
        Second,
        /// <summary>
        /// 分
        /// </summary>
        Minute,
        /// <summary>
        /// 小时
        /// </summary>
        Hour,
        /// <summary>
        /// 天
        /// </summary>
        Day,
        /// <summary>
        /// 周
        /// </summary>
        Week,
        /// <summary>
        /// 月
        /// </summary>
        Month,
        /// <summary>
        /// 年
        /// </summary>
        Year
    }
}
