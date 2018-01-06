using System;
using System.Collections.Generic;
using System.ComponentModel;
using WT_Core;

namespace WT_CTP
{
    /// <summary>
    /// 合并后的持仓
    /// </summary>
    public sealed class Position : INotifyPropertyChanged, IEquatable<Position>, IEqualityComparer<Position>
    {
        #region ==========属性==========

        [DisplayName("合约代码")]
        public string InstrumentID { get; set; }
        [DisplayName("经纪商编号")]
        public string BrokerID { get; set; }
        [DisplayName("投资者代码")]
        public string InvestorID { get; set; }
        [DisplayName("持仓方向")]
        public Direction PosiDirection { get; set; }
        [DisplayName("投机套保标志")]
        public HedgeFlag HedgeFlag { get; set; }
        [DisplayName("持仓日期类型")]
        public PositionDateType PositionDateType { get; set; }
        [DisplayName("昨仓持仓数量")]
        public int YdPosition { get; set; }
        [DisplayName("今仓持仓数量")]
        public int TdPosition { get; set; }
        [DisplayName("总持仓数量")]
        public int TotalPosition { get { return this.YdPosition + this.TdPosition; } }
        [DisplayName("多头冻结")]
        public int LongFrozen { get; set; }
        [DisplayName("空头冻结")]
        public int ShortFrozen { get; set; }
        [DisplayName("开多仓冻结金额")]
        public double LongFrozenAmount { get; set; }
        [DisplayName("开空仓冻结金额")]
        public double ShortFrozenAmount { get; set; }
        [DisplayName("开仓量")]
        public int OpenVolume { get; set; }
        [DisplayName("平仓量")]
        public int CloseVolume { get; set; }
        [DisplayName("开仓金额")]
        public double OpenAmount { get; set; }
        [DisplayName("平仓量")]
        public double CloseAmount { get; set; }
        [DisplayName("持仓成本")]
        public double PositionCost { get; set; }
        [DisplayName("上次占用的保证金")]
        public double PreMargin { get; set; }
        [DisplayName("占用的保证金")]
        public double UseMargin { get; set; }
        [DisplayName("冻结的保证金")]
        public double FrozenMargin { get; set; }
        [DisplayName("冻结的资金")]
        public double FrozenCash { get; set; }
        [DisplayName("冻结的手续费")]
        public double FrozenCommission { get; set; }
        [DisplayName("资金差额")]
        public double CashIn { get; set; }
        [DisplayName("手续费")]
        public double Commission { get; set; }
        [DisplayName("平仓盈亏")]
        public double CloseProfit { get; set; }
        [DisplayName("持仓盈亏")]
        public double PositionProfit { get; set; }        
        [DisplayName("上次结算价")]
        public double PreSettlementPrice { get; set; }
        [DisplayName("本次结算价")]
        public double SettlementPrice { get; set; }
        [DisplayName("交易日")]
        public string TradingDay { get; set; }
        [DisplayName("结算编号")]
        public int SettlementID { get; set; }
        [DisplayName("开仓成本")]
        public double OpenCost { get; set; }
        [DisplayName("交易所保证金")]
        public double ExchangeMargin { get; set; }
        [DisplayName("组合成交形成的持仓")]
        public int CombPosition { get; set; }
        [DisplayName("组合多头冻结")]
        public int CombLongFrozen { get; set; }
        [DisplayName("组合空头冻结")]
        public int CombShortFrozen { get; set; }
        [DisplayName("逐日盯市平仓盈亏")]
        public double CloseProfitByDate { get; set; }
        [DisplayName("逐笔对冲平仓盈亏")]
        public double CloseProfitByTrade { get; set; }
        [DisplayName("保证金率")]
        public double MarginRateByMoney { get; set; }
        [DisplayName("保证金率(按手数)")]
        public double MarginRateByVolume { get; set; }
        [DisplayName("执行冻结")]
        public int StrikeFrozen { get; set; }
        [DisplayName("执行冻结金额")]
        public double StrikeFrozenAmount { get; set; }
        [DisplayName("放弃执行冻结")]
        public int AbandonFrozen { get; set; }
        [DisplayName("最新价")]
        public double LastPrice { get; set; }
        [DisplayName("平均开仓价格")]
        public double AvgOpenPrice { get; set; }
        [DisplayName("开仓后到现在的总盈亏")]
        public double OpenProfit { get; set; }

        #endregion ==========属性==========

        /// <summary>
        /// 构造函数
        /// </summary>
        public Position(string InstrumentID, Direction Direction, HedgeFlag Hedge)
        {
            this.InstrumentID = InstrumentID;
            this.PosiDirection = Direction;
            this.HedgeFlag = Hedge;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Position()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        /// <summary>
        /// 持仓相等性判断：InstrumentID、PosiDirection、HedgeFlag 都相同
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Position other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return this.InstrumentID == other.InstrumentID && this.PosiDirection == other.PosiDirection && this.HedgeFlag == other.HedgeFlag;
            }
        }

        bool IEqualityComparer<Position>.Equals(Position x, Position y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<Position>.GetHashCode(Position obj)
        {
            return obj.InstrumentID.GetHashCode() ^ obj.PosiDirection.ToString().GetHashCode() ^ obj.HedgeFlag.ToString().GetHashCode();
        }
    }
}
