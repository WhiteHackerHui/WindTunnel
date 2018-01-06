using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numeric = System.Double;


namespace WT_CTP
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public enum ConnectionStatus 
    {
        /// <summary>
        /// 未连接
        /// </summary>
        Disconnected = 0,
        /// <summary>
        /// 连接中
        /// </summary>
        Connecting = 1,
        /// <summary>
        /// 已连接
        /// </summary>
        Connected = 2,
        /// <summary>
        /// 登录中
        /// </summary>
        Logining = 3,
        /// <summary>
        /// 已登录
        /// </summary>
        Logined = 4,
        /// <summary>
        /// 未登录
        /// </summary>
        Logout = 5
    }

    /// <summary>
    /// 命令类型选项
    /// </summary>
    public enum ReqCmdType
    {
        /// <summary>
        /// 查询合约
        /// </summary>
        QryInstrument,
        /// <summary>
        /// 查询持仓细节
        /// </summary>
        QryInvestorPositionDetail,
        /// <summary>
        /// 查询持仓
        /// </summary>
        QryInvestorPosition,
        /// <summary>
        /// 查询结算信息
        /// </summary>
        QrySettlementInfo,
        /// <summary>
        /// 确认结算信息
        /// </summary>
        QrySettlementInfoConfirm,
        /// <summary>
        /// 查询交易账户
        /// </summary>
        QryTradingAccount,
    }

    /// <summary>
    /// 投机套保标志
    /// </summary>
    public enum HedgeFlag
    {
        /// <summary>
        /// 投机
        /// </summary>
        Speculation = 1,
        /// <summary>
        /// 套利
        /// </summary>
        Arbitrage = 2,
        /// <summary>
        /// 套保
        /// </summary>
        Hedge = 3,
        /// <summary>
        /// 做市商
        /// </summary>
        MarketMaker = 5
    }

    /// <summary>
    /// 触发条件类型
    ///</summary>
    public enum ContingentCondition
    {
        /// <summary>
        /// 立即
        ///</summary>
        Immediately,
        /// <summary>
        /// 止损
        ///</summary>
        Touch,
        /// <summary>
        /// 止赢
        ///</summary>
        TouchProfit,
        /// <summary>
        /// 预埋单
        ///</summary>
        ParkedOrder,
        /// <summary>
        /// 最新价大于条件价
        ///</summary>
        LastPriceGreaterThanStopPrice,
        /// <summary>
        /// 最新价大于等于条件价
        ///</summary>
        LastPriceGreaterEqualStopPrice,
        /// <summary>
        /// 最新价小于条件价
        ///</summary>
        LastPriceLesserThanStopPrice,
        /// <summary>
        /// 最新价小于等于条件价
        ///</summary>
        LastPriceLesserEqualStopPrice,
        /// <summary>
        /// 卖一价大于条件价
        ///</summary>
        AskPriceGreaterThanStopPrice,
        /// <summary>
        /// 卖一价大于等于条件价
        ///</summary>
        AskPriceGreaterEqualStopPrice,
        /// <summary>
        /// 卖一价小于条件价
        ///</summary>
        AskPriceLesserThanStopPrice,
        /// <summary>
        /// 卖一价小于等于条件价
        ///</summary>
        AskPriceLesserEqualStopPrice,
        /// <summary>
        /// 买一价大于条件价
        ///</summary>
        BidPriceGreaterThanStopPrice,
        /// <summary>
        /// 买一价大于等于条件价
        ///</summary>
        BidPriceGreaterEqualStopPrice,
        /// <summary>
        /// 买一价小于条件价
        ///</summary>
        BidPriceLesserThanStopPrice,
        /// <summary>
        /// 买一价小于等于条件价
        ///</summary>
        BidPriceLesserEqualStopPrice
    }

    /// <summary>
    /// 强平理由
    /// </summary>
    public enum ForceCloseReason
    {
        /// <summary>
        /// 非强平
        ///</summary>
        NotForceClose,
        /// <summary>
        /// 资金不足
        ///</summary>
        LackDeposit,
        /// <summary>
        /// 客户超仓
        ///</summary>
        ClientOverPositionLimit,
        /// <summary>
        /// 会员超仓
        ///</summary>
        MemberOverPositionLimit,
        /// <summary>
        /// 持仓非整数倍
        ///</summary>
        NotMultiple,
        /// <summary>
        /// 违规
        ///</summary>
        Violation,
        /// <summary>
        /// 其它
        ///</summary>
        Other,
        /// <summary>
        /// 自然人临近交割
        ///</summary>
        PersonDeliv
    }

    /// <summary>
    /// 报单价格条件类型
    ///</summary>
    public enum OrderPriceType
    {
        /// <summary>
        /// 任意价
        ///</summary>
        AnyPrice,
        /// <summary>
        /// 限价
        ///</summary>
        LimitPrice,
        /// <summary>
        /// 最优价
        ///</summary>
        BestPrice,
        /// <summary>
        /// 最新价
        ///</summary>
        LastPrice,
        /// <summary>
        /// 最新价浮动上浮1个ticks
        ///</summary>
        LastPricePlusOneTicks,
        /// <summary>
        /// 最新价浮动上浮2个ticks
        ///</summary>
        LastPricePlusTwoTicks,
        /// <summary>
        /// 最新价浮动上浮3个ticks
        ///</summary>
        LastPricePlusThreeTicks,
        /// <summary>
        /// 卖一价
        ///</summary>
        AskPrice1,
        /// <summary>
        /// 卖一价浮动上浮1个ticks
        ///</summary>
        AskPrice1PlusOneTicks,
        /// <summary>
        /// 卖一价浮动上浮2个ticks
        ///</summary>
        AskPrice1PlusTwoTicks,
        /// <summary>
        /// 卖一价浮动上浮3个ticks
        ///</summary>
        AskPrice1PlusThreeTicks,
        /// <summary>
        /// 买一价
        ///</summary>
        BidPrice1,
        /// <summary>
        /// 买一价浮动上浮1个ticks
        ///</summary>
        BidPrice1PlusOneTicks,
        /// <summary>
        /// 买一价浮动上浮2个ticks
        ///</summary>
        BidPrice1PlusTwoTicks,
        /// <summary>
        /// 买一价浮动上浮3个ticks
        ///</summary>
        BidPrice1PlusThreeTicks,
        /// <summary>
        /// 五档价
        ///</summary>
        FiveLevelPrice
    }

    /// <summary>
    /// 报单来源类型
    ///</summary>
    public enum OrderSourceType
    {
        /// <summary>
        /// 来自参与者
        ///</summary>
        Participant,
        /// <summary>
        /// 来自管理员
        ///</summary>
        Administrator
    }

    /// <summary>
    /// 报单状态类型
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 全部成交
        ///</summary>
        AllTraded,
        /// <summary>
        /// 部分成交还在队列中
        ///</summary>
        PartTradedQueueing,
        /// <summary>
        /// 部分成交不在队列中
        ///</summary>
        PartTradedNotQueueing,
        /// <summary>
        /// 未成交还在队列中
        ///</summary>
        NoTradeQueueing,
        /// <summary>
        /// 未成交不在队列中
        ///</summary>
        NoTradeNotQueueing,
        /// <summary>
        /// 撤单
        ///</summary>
        Canceled,
        /// <summary>
        /// 未知
        ///</summary>
        Unknown,
        /// <summary>
        /// 尚未触发
        ///</summary>
        NotTouched,
        /// <summary>
        /// 已触发
        ///</summary>
        Touched
    }

    /// <summary>
    /// 报单提交状态类型
    ///</summary>
    public enum OrderSubmitStatus
    {
        /// <summary>
        /// 已经提交
        ///</summary>
        InsertSubmitted,
        /// <summary>
        /// 撤单已经提交
        ///</summary>
        CancelSubmitted,
        /// <summary>
        /// 修改已经提交
        ///</summary>
        ModifySubmitted,
        /// <summary>
        /// 已经接受
        ///</summary>
        Accepted,
        /// <summary>
        /// 报单已经被拒绝
        ///</summary>
        InsertRejected,
        /// <summary>
        /// 撤单已经被拒绝
        ///</summary>
        CancelRejected,
        /// <summary>
        /// 改单已经被拒绝
        ///</summary>
        ModifyRejected
    }

    /// <summary>
    /// 报单类型类型
    ///</summary>
    public enum OrderType
    {
        /// <summary>
        /// 正常
        ///</summary>
        Normal,
        /// <summary>
        /// 报价衍生
        ///</summary>
        DeriveFromQuote,
        /// <summary>
        /// 组合衍生
        ///</summary>
        DeriveFromCombination,
        /// <summary>
        /// 组合报单
        ///</summary>
        Combination,
        /// <summary>
        /// 条件单
        ///</summary>
        ConditionalOrder,
        /// <summary>
        /// 互换单
        ///</summary>
        Swap,
    }

    /// <summary>
    /// 有效期类型类型
    /// </summary>
    public enum TimeCondition
    {
        /// <summary>
        /// 立即完成，否则撤销
        ///</summary>
        IOC,
        /// <summary>
        /// 本节有效
        ///</summary>
        GFS,
        /// <summary>
        /// 当日有效
        ///</summary>
        GFD,
        /// <summary>
        /// 指定日期前有效
        ///</summary>
        GTD,
        /// <summary>
        /// 撤销前有效
        ///</summary>
        GTC,
        /// <summary>
        /// 集合竞价有效
        ///</summary>
        GFA 

    }

    /// <summary>
    /// 成交量类型类型
    ///</summary>
    public enum VolumeCondition
    {
        /// <summary>
        /// 任何数量
        ///</summary>
        AV,
        /// <summary>
        /// 最小数量
        ///</summary>
        MV,
        /// <summary>
        /// 全部数量
        ///</summary>
        CV
    }

    /// <summary>
    /// 持仓日期类型
    ///</summary>
    public enum PositionDateType
    {
        /// <summary>
        /// 今日持仓
        ///</summary>
        Today,
        /// <summary>
        /// 历史持仓
        ///</summary>
        History,
    }

    /// <summary>
    /// 成交类型类型
    ///</summary>
    public enum TradeType
    {
        /// <summary>
        /// 组合持仓拆分为单一持仓,初始化不应包含该类型的持仓
        ///</summary>
        SplitCombination,
        /// <summary>
        /// 普通成交
        ///</summary>
        Common,
        /// <summary>
        /// 期权执行
        ///</summary>
        OptionsExecution,
        /// <summary>
        /// OTC成交
        ///</summary>
        OTC,
        /// <summary>
        /// 期转现衍生成交
        ///</summary>
        EFPDerived,
        /// <summary>
        /// 组合衍生成交
        ///</summary>
        CombinationDerived
    }

    /// <summary>
    /// 条件单类型
    /// </summary>
    public enum ConditionOrderType
    {
        PriceCondition,
        TimeCondition,
        PositionCondition
    }

    /// <summary>
    /// 条件单执行状态
    /// </summary>
    public enum ConditionOrderExecutionStatus
    {
        NotRunning,
        Running,
        Complete
    }

}
