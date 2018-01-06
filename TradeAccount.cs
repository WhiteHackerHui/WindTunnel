using System;
using System.ComponentModel;

namespace WT_CTP
{
    /// <summary>
    /// 封装的交易账户信息类型
    /// </summary>
    public sealed class TradeAccount : INotifyPropertyChanged, IEquatable<TradeAccount>
    {
        #region ==========内部变量==========

        private CThostFtdcTradingAccountField _taf;

        #endregion ==========内部变量==========

        #region ==========属性==========

        [DisplayName("经纪公司代码")]
        public string BrokerID { get { return _taf.BrokerID; } }
        [DisplayName("投资者帐号")]
        public string AccountID { get { return _taf.AccountID; } }
        [DisplayName("上次质押金额")]
        public double PreMortgage { get { return _taf.PreMortgage; } }
        [DisplayName("上次信用额度")]
        public double PreCredit { get { return _taf.PreCredit; } }
        [DisplayName("上次存款额")]
        public double PreDeposit { get { return _taf.PreDeposit; } }
        [DisplayName("上次结算准备金")]
        public double PreBalance { get { return _taf.PreBalance; } }
        [DisplayName("上次占用的保证金")]
        public double PreMargin { get { return _taf.PreMargin; } }
        [DisplayName("利息基数")]
        public double InterestBase { get { return _taf.InterestBase; } }
        [DisplayName("利息收入")]
        public double Interest { get { return _taf.Interest; } }
        [DisplayName("入金金额")]
        public double Deposit { get { return _taf.Deposit; } }
        [DisplayName("出金金额")]
        public double Withdraw { get { return _taf.Withdraw; } }
        [DisplayName("冻结的保证金")]
        public double FrozenMargin { get { return _taf.FrozenMargin; } }
        [DisplayName("冻结的资金")]
        public double FrozenCash { get { return _taf.FrozenCash; } }
        [DisplayName("冻结的手续费")]
        public double FrozenCommission { get { return _taf.FrozenCommission; } }
        [DisplayName("当前保证金总额")]
        public double CurrMargin { get { return _taf.CurrMargin; } }
        [DisplayName("资金差额")]
        public double CashIn { get { return _taf.CashIn; } }
        [DisplayName("手续费")]
        public double Commission { get { return _taf.Commission; } }
        [DisplayName("平仓盈亏")]
        public double CloseProfit { get { return _taf.CloseProfit; } }
        [DisplayName("持仓盈亏")]
        public double PositionProfit { get { return _taf.PositionProfit; } }
        [DisplayName("期货结算准备金")]
        public double Balance { get { return _taf.Balance; } }
        [DisplayName("可用资金")]
        public double Available { get { return _taf.Available; } }
        [DisplayName("可取资金")]
        public double WithdrawQuota { get { return _taf.WithdrawQuota; } }
        [DisplayName("基本准备金")]
        public double Reserve { get { return _taf.Reserve; } }
        [DisplayName("交易日")]
        public string TradingDay { get { return _taf.TradingDay; } }
        [DisplayName("结算编号")]
        public int SettlementID { get { return _taf.SettlementID; } }
        [DisplayName("信用额度")]
        public double Credit { get { return _taf.Credit; } }
        [DisplayName("质押金额")]
        public double Mortgage { get { return _taf.Mortgage; } }
        [DisplayName("交易所保证金")]
        public double ExchangeMargin { get { return _taf.ExchangeMargin; } }
        [DisplayName("投资者交割保证金")]
        public double DeliveryMargin { get { return _taf.DeliveryMargin; } }
        [DisplayName("交易所交割保证金")]
        public double ExchangeDeliveryMargin { get { return _taf.ExchangeDeliveryMargin; } }
        [DisplayName("保底期货结算准备金")]
        public double ReserveBalance { get { return _taf.ReserveBalance; } }
        [DisplayName("币种代码")]
        public string CurrencyID { get { return _taf.CurrencyID; } }
        [DisplayName("上次货币质入金额")]
        public double PreFundMortgageIn { get { return _taf.PreFundMortgageIn; } }
        [DisplayName("上次货币质出金额")]
        public double PreFundMortgageOut { get { return _taf.PreFundMortgageOut; } }
        [DisplayName("货币质入金额")]
        public double FundMortgageIn { get { return _taf.FundMortgageIn; } }
        [DisplayName("货币质出金额")]
        public double FundMortgageOut { get { return _taf.FundMortgageOut; } }
        [DisplayName("货币质押余额")]
        public double FundMortgageAvailable { get { return _taf.FundMortgageAvailable; } }
        [DisplayName("可质押货币金额")]
        public double MortgageableFund { get { return _taf.MortgageableFund; } }
        [DisplayName("特殊产品占用保证金")]
        public double SpecProductMargin { get { return _taf.SpecProductMargin; } }
        [DisplayName("特殊产品冻结保证金")]
        public double SpecProductFrozenMargin { get { return _taf.SpecProductFrozenMargin; } }
        [DisplayName("特殊产品手续费")]
        public double SpecProductCommission { get { return _taf.SpecProductCommission; } }
        [DisplayName("特殊产品冻结手续费")]
        public double SpecProductFrozenCommission { get { return _taf.SpecProductFrozenCommission; } }
        [DisplayName("特殊产品持仓盈亏")]
        public double SpecProductPositionProfit { get { return _taf.SpecProductPositionProfit; } }
        [DisplayName("特殊产品平仓盈亏")]
        public double SpecProductCloseProfit { get { return _taf.SpecProductCloseProfit; } }
        [DisplayName("根据持仓盈亏算法计算的特殊产品持仓盈亏")]
        public double SpecProductPositionProfitByAlg { get { return _taf.SpecProductPositionProfitByAlg; } }
        [DisplayName("特殊产品交易所保证金")]
        public double SpecProductExchangeMargin { get { return _taf.SpecProductExchangeMargin; } }
        [DisplayName("CThostFtdcTradingAccountField实例")]
        public CThostFtdcTradingAccountField CThostFtdcTradingAccountFieldInstance
        {
            get { return _taf; }
            set { this._taf = value; Notify(""); }
        }

        #endregion ==========属性==========

        /// <summary>
        /// 构造器
        /// </summary>
        public TradeAccount() { }
        public TradeAccount(CThostFtdcTradingAccountField cftaf)
        {
            this.CThostFtdcTradingAccountFieldInstance = cftaf;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        /// <summary>
        /// 账户相等性比较：AccountID
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TradeAccount other)
        {
            if (other == null) return false;
            return (this.AccountID == other.AccountID);
        }

    }
}
