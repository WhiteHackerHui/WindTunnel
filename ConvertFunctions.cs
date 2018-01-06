using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT_Core;

namespace WT_CTP
{
    internal static class ConvertFunctions
    {
        /// <summary>
        /// 将TThostFtdcContingentConditionType转换为ContingentCondition
        /// </summary>
        /// <param name="tfct">TThostFtdcContingentConditionType枚举型</param>
        /// <returns></returns>
        public static ContingentCondition TThostFtdcContingentConditionType_To_ContingentCondition(TThostFtdcContingentConditionType tfct)
        {
            ContingentCondition cct = ContingentCondition.Immediately;
            switch (tfct)
            {
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_Immediately:
                    //cct = ContingentConditionType.Immediately;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_Touch:
                    cct = ContingentCondition.Touch;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_TouchProfit:
                    cct = ContingentCondition.TouchProfit;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_ParkedOrder:
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_LastPriceGreaterThanStopPrice:
                    cct = ContingentCondition.LastPriceGreaterThanStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_LastPriceGreaterEqualStopPrice:
                    cct = ContingentCondition.LastPriceGreaterEqualStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_LastPriceLesserThanStopPrice:
                    cct = ContingentCondition.LastPriceLesserThanStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_LastPriceLesserEqualStopPrice:
                    cct = ContingentCondition.LastPriceLesserEqualStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_AskPriceGreaterThanStopPrice:
                    cct = ContingentCondition.AskPriceGreaterThanStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_AskPriceGreaterEqualStopPrice:
                    cct = ContingentCondition.AskPriceGreaterEqualStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_AskPriceLesserThanStopPrice:
                    cct = ContingentCondition.AskPriceLesserThanStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_AskPriceLesserEqualStopPrice:
                    cct = ContingentCondition.AskPriceLesserEqualStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_BidPriceGreaterThanStopPrice:
                    cct = ContingentCondition.BidPriceGreaterThanStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_BidPriceGreaterEqualStopPrice:
                    cct = ContingentCondition.BidPriceGreaterEqualStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_BidPriceLesserThanStopPrice:
                    cct = ContingentCondition.BidPriceLesserThanStopPrice;
                    break;
                case TThostFtdcContingentConditionType.THOST_FTDC_CC_BidPriceLesserEqualStopPrice:
                    cct = ContingentCondition.BidPriceLesserEqualStopPrice;
                    break;
                default:
                    break;
            }
            return cct;
        }

        /// <summary>
        /// 将字符串形式的CombOffsetFlag转换为Offset枚举型
        /// </summary>
        /// <param name="cof">CombOffsetFlag字符串</param>
        /// <returns></returns>
        public static Offset CombOffsetFlag_To_Offset(string cof)
        {
            Offset ofs = Offset.Open;
            if (cof == "0")
            {
                //ofs = Offset.Open;
            }
            else if (cof == "1")
            {
                ofs = Offset.Close;
            }
            else if (cof == "2")
            {
                ofs = Offset.ForceClose;
            }
            else if (cof == "3")
            {
                ofs = Offset.CloseToday;
            }
            else if (cof == "4")
            {
                ofs = Offset.CloseYesterday;
            }
            else if (cof == "5")
            {
                ofs = Offset.ForceOff;
            }
            else if (cof == "6")
            {
                ofs = Offset.LocalForceClose;
            }
            return ofs;
        }

        /// <summary>
        /// TThostFtdcOffsetFlagType枚举型转为Offset枚举型
        /// </summary>
        /// <param name="tfoft">TThostFtdcOffsetFlagType枚举型</param>
        /// <returns></returns>
        public static Offset TThostFtdcOffsetFlagType_To_Offset(TThostFtdcOffsetFlagType tfoft)
        {
            Offset offset = Offset.Open;
            switch (tfoft)
            {
                case TThostFtdcOffsetFlagType.THOST_FTDC_OF_Open:
                    break;
                case TThostFtdcOffsetFlagType.THOST_FTDC_OF_Close:
                    offset = Offset.Close;
                    break;
                case TThostFtdcOffsetFlagType.THOST_FTDC_OF_ForceClose:
                    offset = Offset.ForceClose;
                    break;
                case TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseToday:
                    offset = Offset.CloseToday;
                    break;
                case TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseYesterday:
                    offset = Offset.CloseYesterday;
                    break;
                case TThostFtdcOffsetFlagType.THOST_FTDC_OF_ForceOff:
                    offset = Offset.ForceOff;
                    break;
                case TThostFtdcOffsetFlagType.THOST_FTDC_OF_LocalForceClose:
                    offset = Offset.LocalForceClose;
                    break;
                default:
                    break;
            }
            return offset;
        }

        /// <summary>
        /// 将TThostFtdcDirectionType枚举型转为Direction枚举型
        /// </summary>
        /// <param name="tfdt">TThostFtdcDirectionType枚举型实例</param>
        /// <returns></returns>
        public static Direction TThostFtdcDirectionType_To_Direction(TThostFtdcDirectionType tfdt)
        {
            Direction dir = Direction.Buy;
            switch (tfdt)
            {
                case TThostFtdcDirectionType.THOST_FTDC_D_Buy:
                    break;
                case TThostFtdcDirectionType.THOST_FTDC_D_Sell:
                    dir = Direction.Sell;
                    break;
                default:
                    break;
            }
            return dir;
        }

        /// <summary>
        /// Direction枚举型转为TThostFtdcDirectionType枚举型
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static TThostFtdcDirectionType Direction_To_TThostFtdcDirectionType(Direction dir)
        {
            TThostFtdcDirectionType tfdt = TThostFtdcDirectionType.THOST_FTDC_D_Buy;
            switch (dir)
            {
                case Direction.Buy:
                    break;
                case Direction.Sell:
                    tfdt = TThostFtdcDirectionType.THOST_FTDC_D_Sell;
                    break;
                default:
                    break;
            }
            return tfdt;
        }

        /// <summary>
        /// 将TThostFtdcForceCloseReasonType枚举型转为ForceCloseReasonType枚举型
        /// </summary>
        /// <param name="tfcrt">TThostFtdcForceCloseReasonType枚举型</param>
        /// <returns></returns>
        public static ForceCloseReason TThostFtdcForceCloseReasonType_To_ForceCloseReason(TThostFtdcForceCloseReasonType tfcrt)
        {
            ForceCloseReason fcrt = ForceCloseReason.NotForceClose;
            switch (tfcrt)
            {
                case TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_NotForceClose:
                    //fcrt = ForceCloseReasonType.NotForceClose;
                    break;
                case TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_LackDeposit:
                    fcrt = ForceCloseReason.LackDeposit;
                    break;
                case TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_ClientOverPositionLimit:
                    fcrt = ForceCloseReason.ClientOverPositionLimit;
                    break;
                case TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_MemberOverPositionLimit:
                    fcrt = ForceCloseReason.MemberOverPositionLimit;
                    break;
                case TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_NotMultiple:
                    fcrt = ForceCloseReason.NotMultiple;
                    break;
                case TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_Violation:
                    fcrt = ForceCloseReason.Violation;
                    break;
                case TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_Other:
                    fcrt = ForceCloseReason.Other;
                    break;
                case TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_PersonDeliv:
                    fcrt = ForceCloseReason.PersonDeliv;
                    break;
                default:
                    break;
            }
            return fcrt;
        }

        /// <summary>
        /// 将TThostFtdcOrderPriceTypeType枚举型转为OrderPriceTypeType枚举型
        /// </summary>
        /// <param name="tfoptt">TThostFtdcOrderPriceTypeType枚举型</param>
        /// <returns></returns>
        public static OrderPriceType TThostFtdcOrderPriceTypeType_To_OrderPriceType(TThostFtdcOrderPriceTypeType tfoptt)
        {
            OrderPriceType optt = OrderPriceType.AnyPrice;
            switch (tfoptt)
            {
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AnyPrice:
                    //optt = OrderPriceTypeType.AnyPrice;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice:
                    optt = OrderPriceType.LimitPrice;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BestPrice:
                    optt = OrderPriceType.BestPrice;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPrice:
                    optt = OrderPriceType.LastPrice;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPricePlusOneTicks:
                    optt = OrderPriceType.LastPricePlusOneTicks;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPricePlusTwoTicks:
                    optt = OrderPriceType.LastPricePlusTwoTicks;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPricePlusThreeTicks:
                    optt = OrderPriceType.LastPricePlusThreeTicks;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1:
                    optt = OrderPriceType.AskPrice1;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1PlusOneTicks:
                    optt = OrderPriceType.AskPrice1PlusOneTicks;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1PlusTwoTicks:
                    optt = OrderPriceType.AskPrice1PlusTwoTicks;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1PlusThreeTicks:
                    optt = OrderPriceType.AskPrice1PlusThreeTicks;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1:
                    optt = OrderPriceType.BidPrice1;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1PlusOneTicks:
                    optt = OrderPriceType.BidPrice1PlusOneTicks;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1PlusTwoTicks:
                    optt = OrderPriceType.BidPrice1PlusTwoTicks;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1PlusThreeTicks:
                    optt = OrderPriceType.BidPrice1PlusThreeTicks;
                    break;
                case TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_FiveLevelPrice:
                    optt = OrderPriceType.FiveLevelPrice;
                    break;
                default:
                    break;
            }
            return optt;
        }

        /// <summary>
        /// OrderPriceType枚举型转为TThostFtdcOrderPriceTypeType枚举型
        /// </summary>
        /// <param name="opt">OrderPriceType枚举型</param>
        /// <returns></returns>
        public static TThostFtdcOrderPriceTypeType OrderPriceType_To_TThostFtdcOrderPriceTypeType(OrderPriceType opt)
        {
            TThostFtdcOrderPriceTypeType tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AnyPrice;
            switch (opt)
            {
                case OrderPriceType.AnyPrice:
                    break;
                case OrderPriceType.LimitPrice:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice;
                    break;
                case OrderPriceType.BestPrice:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BestPrice;
                    break;
                case OrderPriceType.LastPrice:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPrice;
                    break;
                case OrderPriceType.LastPricePlusOneTicks:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPricePlusOneTicks;
                    break;
                case OrderPriceType.LastPricePlusTwoTicks:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPricePlusTwoTicks;
                    break;
                case OrderPriceType.LastPricePlusThreeTicks:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LastPricePlusThreeTicks;
                    break;
                case OrderPriceType.AskPrice1:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1;
                    break;
                case OrderPriceType.AskPrice1PlusOneTicks:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1PlusOneTicks;
                    break;
                case OrderPriceType.AskPrice1PlusTwoTicks:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1PlusTwoTicks;
                    break;
                case OrderPriceType.AskPrice1PlusThreeTicks:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AskPrice1PlusThreeTicks;
                    break;
                case OrderPriceType.BidPrice1:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1;
                    break;
                case OrderPriceType.BidPrice1PlusOneTicks:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1PlusOneTicks;
                    break;
                case OrderPriceType.BidPrice1PlusTwoTicks:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1PlusTwoTicks;
                    break;
                case OrderPriceType.BidPrice1PlusThreeTicks:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_BidPrice1PlusThreeTicks;
                    break;
                case OrderPriceType.FiveLevelPrice:
                    tfoptt = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_FiveLevelPrice;
                    break;
                default:
                    break;
            }
            return tfoptt;
        }

        /// <summary>
        /// TThostFtdcOrderSourceType枚举型转为OrderSourceType枚举型
        /// </summary>
        /// <param name="tfost">TThostFtdcOrderSourceType枚举型</param>
        /// <returns></returns>
        public static OrderSourceType TThostFtdcOrderSourceType_To_OrderSourceType(TThostFtdcOrderSourceType tfost)
        {
            OrderSourceType ost = OrderSourceType.Participant;
            switch (tfost)
            {
                case TThostFtdcOrderSourceType.THOST_FTDC_OSRC_Participant:
                    break;
                case TThostFtdcOrderSourceType.THOST_FTDC_OSRC_Administrator:
                    ost = OrderSourceType.Administrator;
                    break;
                default:
                    break;
            }
            return ost;
        }

        /// <summary>
        /// TThostFtdcOrderStatusType枚举型转为OrderStatusType枚举型
        /// </summary>
        /// <param name="tfost">TThostFtdcOrderStatusType枚举型</param>
        /// <returns></returns>
        public static OrderStatus TThostFtdcOrderStatusType_To_OrderStatus(TThostFtdcOrderStatusType tfost)
        {
            OrderStatus ost = OrderStatus.AllTraded;
            switch (tfost)
            {
                case TThostFtdcOrderStatusType.THOST_FTDC_OST_AllTraded:
                    break;
                case TThostFtdcOrderStatusType.THOST_FTDC_OST_PartTradedQueueing:
                    ost = OrderStatus.PartTradedQueueing;
                    break;
                case TThostFtdcOrderStatusType.THOST_FTDC_OST_PartTradedNotQueueing:
                    ost = OrderStatus.PartTradedNotQueueing;
                    break;
                case TThostFtdcOrderStatusType.THOST_FTDC_OST_NoTradeQueueing:
                    ost = OrderStatus.NoTradeQueueing;
                    break;
                case TThostFtdcOrderStatusType.THOST_FTDC_OST_NoTradeNotQueueing:
                    ost = OrderStatus.NoTradeNotQueueing;
                    break;
                case TThostFtdcOrderStatusType.THOST_FTDC_OST_Canceled:
                    ost = OrderStatus.Canceled;
                    break;
                case TThostFtdcOrderStatusType.THOST_FTDC_OST_Unknown:
                    ost = OrderStatus.Unknown;
                    break;
                case TThostFtdcOrderStatusType.THOST_FTDC_OST_NotTouched:
                    ost = OrderStatus.NotTouched;
                    break;
                case TThostFtdcOrderStatusType.THOST_FTDC_OST_Touched:
                    ost = OrderStatus.Touched;
                    break;
                default:
                    break;
            }
            return ost;
        }

        /// <summary>
        /// TThostFtdcOrderSubmitStatusType枚举型转为OrderSubmitStatusType枚举型
        /// </summary>
        /// <param name="tfosst">TThostFtdcOrderSubmitStatusType枚举型</param>
        /// <returns></returns>
        public static OrderSubmitStatus TThostFtdcOrderSubmitStatusType_To_OrderSubmitStatus(TThostFtdcOrderSubmitStatusType tfosst)
        {
            OrderSubmitStatus osst = OrderSubmitStatus.InsertSubmitted;
            switch (tfosst)
            {
                case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_InsertSubmitted:
                    break;
                case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_CancelSubmitted:
                    osst = OrderSubmitStatus.CancelSubmitted;
                    break;
                case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_ModifySubmitted:
                    osst = OrderSubmitStatus.ModifySubmitted;
                    break;
                case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_Accepted:
                    osst = OrderSubmitStatus.Accepted;
                    break;
                case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_InsertRejected:
                    osst = OrderSubmitStatus.InsertRejected;
                    break;
                case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_CancelRejected:
                    osst = OrderSubmitStatus.CancelRejected;
                    break;
                case TThostFtdcOrderSubmitStatusType.THOST_FTDC_OSS_ModifyRejected:
                    osst = OrderSubmitStatus.ModifyRejected;
                    break;
                default:
                    break;
            }
            return osst;
        }

        /// <summary>
        /// TThostFtdcOrderTypeType枚举型转为OrderTypeType枚举型
        /// </summary>
        /// <param name="tfott">TThostFtdcOrderTypeType枚举型</param>
        /// <returns></returns>
        public static OrderType TThostFtdcOrderTypeType_To_OrderType(TThostFtdcOrderTypeType tfott)
        {
            OrderType ott = OrderType.Normal;
            switch (tfott)
            {
                case TThostFtdcOrderTypeType.THOST_FTDC_ORDT_Normal:
                    break;
                case TThostFtdcOrderTypeType.THOST_FTDC_ORDT_DeriveFromQuote:
                    ott = OrderType.DeriveFromQuote;
                    break;
                case TThostFtdcOrderTypeType.THOST_FTDC_ORDT_DeriveFromCombination:
                    ott = OrderType.DeriveFromCombination;
                    break;
                case TThostFtdcOrderTypeType.THOST_FTDC_ORDT_Combination:
                    ott = OrderType.Combination;
                    break;
                case TThostFtdcOrderTypeType.THOST_FTDC_ORDT_ConditionalOrder:
                    ott = OrderType.ConditionalOrder;
                    break;
                case TThostFtdcOrderTypeType.THOST_FTDC_ORDT_Swap:
                    ott = OrderType.Swap;
                    break;
                default:
                    break;
            }
            return ott;
        }

        /// <summary>
        /// TThostFtdcTimeConditionType枚举型转为TimeConditionType枚举型
        /// </summary>
        /// <param name="tftct">TThostFtdcTimeConditionType枚举型</param>
        /// <returns></returns>
        public static TimeCondition TThostFtdcTimeConditionType_To_TimeCondition(TThostFtdcTimeConditionType tftct)
        {
            TimeCondition tct = TimeCondition.IOC;
            switch (tftct)
            {
                case TThostFtdcTimeConditionType.THOST_FTDC_TC_IOC:
                    break;
                case TThostFtdcTimeConditionType.THOST_FTDC_TC_GFS:
                    tct = TimeCondition.GFS;
                    break;
                case TThostFtdcTimeConditionType.THOST_FTDC_TC_GFD:
                    tct = TimeCondition.GFD;
                    break;
                case TThostFtdcTimeConditionType.THOST_FTDC_TC_GTD:
                    tct = TimeCondition.GTD;
                    break;
                case TThostFtdcTimeConditionType.THOST_FTDC_TC_GTC:
                    tct = TimeCondition.GTC;
                    break;
                case TThostFtdcTimeConditionType.THOST_FTDC_TC_GFA:
                    tct = TimeCondition.GFA;
                    break;
                default:
                    break;
            }
            return tct;
        }

        /// <summary>
        /// TThostFtdcVolumeConditionType枚举型转为VolumeConditionType枚举型
        /// </summary>
        /// <param name="tfvct">TThostFtdcVolumeConditionType枚举型</param>
        /// <returns></returns>
        public static VolumeCondition TThostFtdcVolumeConditionType_To_VolumeCondition(TThostFtdcVolumeConditionType tfvct)
        {
            VolumeCondition vct = VolumeCondition.AV;
            switch (tfvct)
            {
                case TThostFtdcVolumeConditionType.THOST_FTDC_VC_AV:
                    break;
                case TThostFtdcVolumeConditionType.THOST_FTDC_VC_MV:
                    vct = VolumeCondition.MV;
                    break;
                case TThostFtdcVolumeConditionType.THOST_FTDC_VC_CV:
                    vct = VolumeCondition.CV;
                    break;
                default:
                    break;
            }
            return vct;
        }

        /// <summary>
        /// TThostFtdcPosiDirectionType枚举型转为Direction枚举型
        /// </summary>
        /// <param name="tfdt">TThostFtdcPosiDirectionType枚举型</param>
        /// <returns></returns>
        public static Direction TThostFtdcPosiDirectionType_To_Direction(TThostFtdcPosiDirectionType tfdt)
        {
            Direction dir = Direction.Buy;
            switch (tfdt)
            {
                case TThostFtdcPosiDirectionType.THOST_FTDC_PD_Net:
                    throw new Exception("Direction枚举型中没有Net选项。");
                case TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long:
                    break;
                case TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short:
                    dir = Direction.Sell;
                    break;
                default:
                    break;
            }
            return dir;
        }

        /// <summary>
        /// TThostFtdcPositionDateType枚举型转为PositionDateType枚举型
        /// </summary>
        /// <param name="tfpdt">TThostFtdcPositionDateType枚举型</param>
        /// <returns></returns>
        public static PositionDateType TThostFtdcPositionDateType_To_PositionDateType(TThostFtdcPositionDateType tfpdt)
        {
            PositionDateType pdt = PositionDateType.Today;
            switch (tfpdt)
            {
                case TThostFtdcPositionDateType.THOST_FTDC_PSD_Today:
                    break;
                case TThostFtdcPositionDateType.THOST_FTDC_PSD_History:
                    pdt = PositionDateType.History;
                    break;
                default:
                    break;
            }
            return pdt;
        }

        /// <summary>
        /// TThostFtdcHedgeFlagType枚举型转为HedgeFlag枚举型
        /// </summary>
        /// <param name="tfhft">TThostFtdcHedgeFlagType枚举型</param>
        /// <returns></returns>
        public static HedgeFlag TThostFtdcHedgeFlagType_To_HedgeFlag(TThostFtdcHedgeFlagType tfhft)
        {
            HedgeFlag hf = HedgeFlag.Speculation;
            switch (tfhft)
            {
                case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation:
                    break;
                case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Arbitrage:
                    hf = HedgeFlag.Arbitrage;
                    break;
                case TThostFtdcHedgeFlagType.THOST_FTDC_HF_Hedge:
                    hf = HedgeFlag.Hedge;
                    break;
                case TThostFtdcHedgeFlagType.THOST_FTDC_HF_MarketMaker:
                    hf = HedgeFlag.MarketMaker;
                    break;
                default:
                    break;
            }
            return hf;
        }

        /// <summary>
        /// TThostFtdcTradeTypeType枚举型转为TradeType枚举型
        /// </summary>
        /// <param name="tfttt">TThostFtdcTradeTypeType枚举型</param>
        /// <returns></returns>
        public static TradeType TThostFtdcTradeTypeType_To_TradeType(TThostFtdcTradeTypeType tfttt)
        {
            TradeType tt = TradeType.SplitCombination;
            switch (tfttt)
            {
                case TThostFtdcTradeTypeType.THOST_FTDC_TRDT_SplitCombination:
                    break;
                case TThostFtdcTradeTypeType.THOST_FTDC_TRDT_Common:
                    tt = TradeType.Common;
                    break;
                case TThostFtdcTradeTypeType.THOST_FTDC_TRDT_OptionsExecution:
                    tt = TradeType.OptionsExecution;
                    break;
                case TThostFtdcTradeTypeType.THOST_FTDC_TRDT_OTC:
                    tt = TradeType.OTC;
                    break;
                case TThostFtdcTradeTypeType.THOST_FTDC_TRDT_EFPDerived:
                    tt = TradeType.EFPDerived;
                    break;
                case TThostFtdcTradeTypeType.THOST_FTDC_TRDT_CombinationDerived:
                    tt = TradeType.CombinationDerived;
                    break;
                default:
                    break;
            }
            return tt;
        }

        /// <summary>
        /// TimeCondition枚举型转为TThostFtdcTimeConditionType枚举型
        /// </summary>
        /// <param name="tc">TimeCondition枚举型</param>
        /// <returns></returns>
        public static TThostFtdcTimeConditionType TimeCondition_To_TThostFtdcTimeConditionType(TimeCondition tc)
        {
            TThostFtdcTimeConditionType tftct = TThostFtdcTimeConditionType.THOST_FTDC_TC_GFD;
            switch (tc)
            {
                case TimeCondition.GFD:
                    break;
                case TimeCondition.IOC:
                    tftct = TThostFtdcTimeConditionType.THOST_FTDC_TC_IOC;
                    break;
                case TimeCondition.GFS:
                    tftct = TThostFtdcTimeConditionType.THOST_FTDC_TC_GFS;
                    break;
                case TimeCondition.GTD:
                    tftct = TThostFtdcTimeConditionType.THOST_FTDC_TC_GTD;
                    break;
                case TimeCondition.GTC:
                    tftct = TThostFtdcTimeConditionType.THOST_FTDC_TC_GTC;
                    break;
                case TimeCondition.GFA:
                    tftct = TThostFtdcTimeConditionType.THOST_FTDC_TC_GFA;
                    break;
                default:
                    break;
            }
            return tftct;
        }

        /// <summary>
        /// VolumeCondition枚举型转为TThostFtdcVolumeConditionType枚举型
        /// </summary>
        /// <param name="vc">VolumeCondition枚举型</param>
        /// <returns></returns>
        public static TThostFtdcVolumeConditionType VolumeCondition_To_TThostFtdcVolumeConditionType(VolumeCondition vc)
        {
            TThostFtdcVolumeConditionType tfvct = TThostFtdcVolumeConditionType.THOST_FTDC_VC_AV;
            switch (vc)
            {
                case VolumeCondition.AV:
                    break;
                case VolumeCondition.MV:
                    tfvct = TThostFtdcVolumeConditionType.THOST_FTDC_VC_MV;
                    break;
                case VolumeCondition.CV:
                    tfvct = TThostFtdcVolumeConditionType.THOST_FTDC_VC_CV;
                    break;
                default:
                    break;
            }
            return tfvct;
        }

        /// <summary>
        /// ContingentCondition枚举型转为TThostFtdcContingentConditionType枚举型
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static TThostFtdcContingentConditionType ContingentCondition_To_TThostFtdcContingentConditionType(ContingentCondition cc)
        {
            TThostFtdcContingentConditionType tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_Immediately;
            switch (cc)
            {
                case ContingentCondition.Immediately:
                    break;
                case ContingentCondition.Touch:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_Touch;
                    break;
                case ContingentCondition.TouchProfit:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_TouchProfit;
                    break;
                case ContingentCondition.ParkedOrder:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_ParkedOrder;
                    break;
                case ContingentCondition.LastPriceGreaterThanStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_LastPriceGreaterThanStopPrice;
                    break;
                case ContingentCondition.LastPriceGreaterEqualStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_LastPriceGreaterEqualStopPrice;
                    break;
                case ContingentCondition.LastPriceLesserThanStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_LastPriceLesserThanStopPrice;
                    break;
                case ContingentCondition.LastPriceLesserEqualStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_LastPriceLesserEqualStopPrice;
                    break;
                case ContingentCondition.AskPriceGreaterThanStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_AskPriceGreaterThanStopPrice;
                    break;
                case ContingentCondition.AskPriceGreaterEqualStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_AskPriceGreaterEqualStopPrice;
                    break;
                case ContingentCondition.AskPriceLesserThanStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_AskPriceLesserThanStopPrice;
                    break;
                case ContingentCondition.AskPriceLesserEqualStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_AskPriceLesserEqualStopPrice;
                    break;
                case ContingentCondition.BidPriceGreaterThanStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_BidPriceGreaterThanStopPrice;
                    break;
                case ContingentCondition.BidPriceGreaterEqualStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_BidPriceGreaterEqualStopPrice;
                    break;
                case ContingentCondition.BidPriceLesserThanStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_BidPriceLesserThanStopPrice;
                    break;
                case ContingentCondition.BidPriceLesserEqualStopPrice:
                    tfcct = TThostFtdcContingentConditionType.THOST_FTDC_CC_BidPriceLesserEqualStopPrice;
                    break;
                default:
                    break;
            }
            return tfcct;
        }

        /// <summary>
        /// ForceCloseReason枚举型转为TThostFtdcForceCloseReasonType枚举型
        /// </summary>
        /// <param name="fcr">ForceCloseReason枚举型</param>
        /// <returns></returns>
        public static TThostFtdcForceCloseReasonType ForceCloseReason_To_TThostFtdcForceCloseReasonType(ForceCloseReason fcr)
        {
            TThostFtdcForceCloseReasonType tffcrt = TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_NotForceClose;
            switch (fcr)
            {
                case ForceCloseReason.NotForceClose:
                    break;
                case ForceCloseReason.LackDeposit:
                    tffcrt = TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_LackDeposit;
                    break;
                case ForceCloseReason.ClientOverPositionLimit:
                    tffcrt = TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_ClientOverPositionLimit;
                    break;
                case ForceCloseReason.MemberOverPositionLimit:
                    tffcrt = TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_MemberOverPositionLimit;
                    break;
                case ForceCloseReason.NotMultiple:
                    tffcrt = TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_NotMultiple;
                    break;
                case ForceCloseReason.Violation:
                    tffcrt = TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_Violation;
                    break;
                case ForceCloseReason.Other:
                    tffcrt = TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_Other;
                    break;
                case ForceCloseReason.PersonDeliv:
                    tffcrt = TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_PersonDeliv;
                    break;
                default:
                    break;
            }
            return tffcrt;
        }

        /// <summary>
        /// InputOrderField转换为OrderField
        /// </summary>
        public static OrderField InputOrderField_To_OrderField(CThostFtdcInputOrderField iof, CThostFtdcRspInfoField pRspInfo)
        {
            CThostFtdcOrderField cfof = new CThostFtdcOrderField();
            cfof.BrokerID = iof.BrokerID;
            cfof.InvestorID = iof.InvestorID;
            cfof.InstrumentID = iof.InstrumentID;
            cfof.OrderRef = iof.OrderRef;
            cfof.UserID = iof.UserID;
            cfof.CombHedgeFlag = iof.CombHedgeFlag;
            cfof.CombHedgeFlag = iof.CombHedgeFlag;
            cfof.ContingentCondition = iof.ContingentCondition;
            cfof.Direction = iof.Direction;
            cfof.ExchangeID = iof.ExchangeID;
            cfof.ForceCloseReason = iof.ForceCloseReason;
            cfof.GTDDate = iof.GTDDate;
            cfof.LimitPrice = iof.LimitPrice;
            cfof.StopPrice = iof.StopPrice;
            cfof.OrderPriceType = iof.OrderPriceType;
            cfof.TimeCondition = iof.TimeCondition;
            cfof.VolumeTotalOriginal = iof.VolumeTotalOriginal;
            cfof.MinVolume = iof.MinVolume;
            cfof.VolumeCondition = iof.VolumeCondition;
            cfof.StatusMsg = string.Format("{0}: {1}", pRspInfo.ErrorID, pRspInfo.ErrorMsg);
            return new OrderField(cfof);
        }

       
    }
}
