using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WT_CTP
{
    /// <summary>
    /// 封装的合约信息
    /// </summary>
    public sealed class InstrumentField : INotifyPropertyChanged
    {
        #region ==========内部变量==========

        private CThostFtdcInstrumentField _if;
        private string _baseCode;

        #endregion ==========内部变量==========

        #region ==========属性==========
        [DisplayName("创建日")]
        public string CreateDate { get { return _if.CreateDate; } }
        [DisplayName("交割月")]
        public int DeliveryMonth { get { return _if.DeliveryMonth; } }
        [DisplayName("交割年份")]
        public int DeliveryYear { get { return _if.DeliveryYear; } }
        [DisplayName("结束交割日")]
        public string EndDelivDate { get { return _if.EndDelivDate; } }
        [DisplayName("交易所代码")]
        public string ExchangeID { get { return _if.ExchangeID; } }
        [DisplayName("合约在交易所的代码")]
        public string ExchangeInstID { get { return _if.ExchangeInstID; } }
        [DisplayName("到期日")]
        public string ExpireDate { get { return _if.ExpireDate; } }
        [DisplayName("合约生命周期状态")]
        public TThostFtdcInstLifePhaseType InstLifePhase { get { return _if.InstLifePhase; } }
        [DisplayName("合约代码")]
        public string InstrumentID { get { return _if.InstrumentID; } }
        [DisplayName("合约名称")]
        public string InstrumentName { get { return _if.InstrumentName; } }
        [DisplayName("当前是否交易")]
        public int IsTrading { get { return _if.IsTrading; } }
        [DisplayName("多头保证金率")]
        public double LongMarginRatio { get { return _if.LongMarginRatio; } }
        [DisplayName("限价单最大下单量")]
        public int MaxLimitOrderVolume { get { return _if.MaxLimitOrderVolume; } }
        [DisplayName("市价单最大下单量")]
        public int MaxMarketOrderVolume { get { return _if.MaxMarketOrderVolume; } }
        [DisplayName("限价单最小下单量")]
        public int MinLimitOrderVolume { get { return _if.MinLimitOrderVolume; } }
        [DisplayName("市价单最小下单量")]
        public int MinMarketOrderVolume { get { return _if.MinMarketOrderVolume; } }
        [DisplayName("上市日")]
        public string OpenDate { get { return _if.OpenDate; } }
        [DisplayName("持仓日期类型")]
        public TThostFtdcPositionDateTypeType PositionDateType { get { return _if.PositionDateType; } }
        [DisplayName("持仓类型")]
        public TThostFtdcPositionTypeType PositionType { get { return _if.PositionType; } }
        [DisplayName("最小变动价位")]
        public double PriceTick { get { return _if.PriceTick; } }
        [DisplayName("产品类型")]
        public TThostFtdcProductClassType ProductClass { get { return _if.ProductClass; } }
        [DisplayName("产品代码")]
        public string ProductID { get { return _if.ProductID; } }
        [DisplayName("空头保证金率")]
        public double ShortMarginRatio { get { return _if.ShortMarginRatio; } }
        [DisplayName("开始交割日")]
        public string StartDelivDate { get { return _if.StartDelivDate; } }
        [DisplayName("合约数量乘数")]
        public int VolumeMultiple { get { return _if.VolumeMultiple; } }
        [DisplayName("期货、期权种类代码")]
        public string BaseCode { get { return _baseCode; } }
        [DisplayName("合约信息实例")]
        public CThostFtdcInstrumentField CThostFtdcInstrumentFieldInstance
        {
            get { return _if; }
            set { _if = value; Notify(""); }
        }

        #endregion ==========属性==========

        /// <summary>
        /// 构造函数
        /// </summary>
        public InstrumentField(CThostFtdcInstrumentField instrumentField)
        {
            this._if = instrumentField;
            Regex r = new Regex(@"\d");
            int index = r.Match(_if.InstrumentID).Index;
            _baseCode = _if.InstrumentID.Substring(0, index);
            Notify("");
        }

        /// <summary>
        /// 通知属性变化事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 通知调用事件
        /// </summary>
        /// <param name="PropertyName"></param>
        private void Notify(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

    }
}
