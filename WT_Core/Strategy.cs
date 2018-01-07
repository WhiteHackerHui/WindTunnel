using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Numeric = System.Double;

namespace WT_Core
{
    /// <summary>
	/// 策略
	/// </summary>
	[DefaultProperty("Name")]
    public partial class Strategy : CustomTypeDescriptor,INotifyPropertyChanged
    {
        internal readonly BindingFlags Bf = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
        internal readonly List<string> NonCustomSeriesNames = new List<string>(new[] { "NumDateTimes", "Highs", "Lows", "Opens", "Closes", "Volumes", "OpenInts"});
        /// <summary>
		/// 用户自定义序列
		/// </summary>
		private readonly List<NumericSeries> _costomSeries = new List<NumericSeries>();

        /// <summary>
		/// 指标集合
		/// </summary>
		private readonly List<Indicator> _indicators = new List<Indicator>();

        public List<Indicator> Indicators { get { return _indicators; } }

        /// <summary>
		/// 初始化
		/// </summary>
		public abstract void Initialize();

        /// <summary>
        /// 继承类需完成的策略主体函数
        /// </summary>
        public abstract void OnBarUpdate();

        /// <summary>
		/// 策略所用的数据序列：Data0，Data1等的 BarSeries
		/// </summary>
		[Browsable(false)]
        public List<BarSeries> BarSerieses { get; private set; }

        /// <summary>
		/// 策略数据集合：Data0，Data1 ...
		/// </summary>
		[Browsable(false)]
        public List<StrategyData> StrategyDatas { get; set; }
        
        /// <summary>
        /// 策略名称
        /// </summary>
        private string _name = string.Empty;

        /// <summary>
        /// 构造器：Strategy
		/// </summary>
		protected Strategy()
        {
            this.BarSerieses = new List<BarSeries>();
            this.StrategyDatas = new List<StrategyData>();
            //属性参数字典处理
            DicProperties.Clear();
            FieldInfo[] fis = GetType().GetFields(this.Bf);
            foreach (var v in fis )
            {
                object[] ps = v.GetCustomAttributes(typeof(ParameterAttribute), false);
                if (ps.Length <= 0)
                {
                    continue;
                }
                ParameterAttribute pa = (ParameterAttribute)ps[0];
                Property p = new Property(v.Name, v.GetValue(this)) { Category = pa.Category, Description = pa.Description };
                Add(p);
            }
        }

        /// <summary>
		/// 开多仓：买开
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格 (非PriceTick整数倍会报错,请先行处理.)</param>
		/// <param name="pRemark">注释</param>
		public void Buy(Numeric pLots, Numeric pPrice, string pRemark = "")
        {
            if (PositionNet<0)
            {
                this.StrategyDatas[0].BuyToCover(-PositionNet, pPrice, pRemark);
            }
            this.StrategyDatas[0].Buy((int)pLots, pPrice, pRemark);
        }

        /// <summary>
        /// 平多仓：卖平
        /// </summary>
        /// <param name="pLots"> 手数 </param>
        /// <param name="pPrice"> 价格  (非PriceTick整数倍会报错,请先行处理.)</param>
        /// <param name="pRemark">注释</param>
        public void Sell(Numeric pLots, Numeric pPrice, string pRemark = "")
        {
            this.StrategyDatas[0].Sell((int)pLots, pPrice, pRemark);
        }

        /// <summary>
        /// 开空仓：卖开
        /// </summary>
        /// <param name="pLots"> 手数 </param>
        /// <param name="pPrice"> 价格  (非PriceTick整数倍会报错,请先行处理.)</param>
        /// <param name="pRemark">注释</param>
        public void SellShort(Numeric pLots, Numeric pPrice, string pRemark = "")
        {
            if (PositionNet > 0)
            {
                this.StrategyDatas[0].Sell(PositionNet, pPrice, pRemark);
            }
            this.StrategyDatas[0].SellShort((int)pLots, pPrice, pRemark);
        }

        /// <summary>
        /// 平空仓：买平
        /// </summary>
        /// <param name="pLots"> 手数 </param>
        /// <param name="pPrice"> 价格  (非PriceTick整数倍会报错,请先行处理.)</param>
        /// <param name="pRemark">注释</param>
        public void BuyToCover(Numeric pLots, Numeric pPrice, string pRemark = "")
        {
            this.StrategyDatas[0].BuyToCover((int)pLots, pPrice, pRemark);
        }


        /// <summary>
		/// 报单操作记录，返回StrategyDatas[0]的报单操作记录
		/// </summary>
		[Description("报单操作列表"), Category("交易")]
        [Browsable(false)]
        public List<OrderItem> Operations { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].Operations; } }

        #region 策略状态:对data[0]的引用

        /// <summary>
        /// 当前持仓手数:多
        /// </summary>
        [Description("当前持仓手数:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public int PositionLong { get { return this.StrategyDatas[0].PositionLong; } }

        /// <summary>
        /// 当前持仓手数:空
        /// </summary>
        [Description("当前持仓手数:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public int PositionShort { get { return this.StrategyDatas[0].PositionShort; } }

        /// <summary>
        /// 当前持仓手数:净
        /// </summary>
        [Description("当前持仓手数:净"), Category("状态"), ReadOnly(true), Browsable(false)]
        public int PositionNet { get { return this.StrategyDatas[0].PositionNet; } }

        /// <summary>
        /// 当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)
        /// </summary>
        [Description("当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric EntryDateLong { get { return this.StrategyDatas[0].EntryDateLong; } }

        /// <summary>
        /// 当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)
        /// </summary>
        [Description("当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric EntryDateShort { get { return this.StrategyDatas[0].EntryDateShort; } }

        /// <summary>
        /// 当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)
        /// </summary>
        [Description("当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric LastEntryDateLong { get { return this.StrategyDatas[0].LastEntryDateLong; } }

        /// <summary>
        /// 当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)
        /// </summary>
        [Description("当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric LastEntryDateShort { get { return this.StrategyDatas[0].LastEntryDateShort; } }

        /// <summary>
        /// 当前持仓首个建仓价格:多
        /// </summary>
        [Description("当前持仓首个建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric EntryPriceLong { get { return this.StrategyDatas[0].EntryPriceLong; } }

        /// <summary>
        /// 当前持仓首个建仓价格:空
        /// </summary>
        [Description("当前持仓首个建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric EntryPriceShort { get { return this.StrategyDatas[0].EntryPriceShort; } }

        /// <summary>
        /// 当前持仓最后建仓价格:多
        /// </summary>
        [Description("当前持仓最后建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric LastEntryPriceLong { get { return this.StrategyDatas[0].LastEntryPriceLong; } }

        /// <summary>
        /// 当前持仓最后建仓价格:空
        /// </summary>
        [Description("当前持仓最后建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric LastEntryPriceShort { get { return this.StrategyDatas[0].LastEntryPriceShort; } }

        /// <summary>
        /// 当前持仓平均建仓价格:多
        /// </summary>
        [Description("当前持仓平均建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric AvgEntryPriceLong { get { return this.StrategyDatas[0].AvgEntryPriceLong; } }

        /// <summary>
        /// 当前持仓平均建仓价格:空
        /// </summary>
        [Description("当前持仓平均建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric AvgEntryPriceShort { get { return this.StrategyDatas[0].AvgEntryPriceShort; } }

        /// <summary>
        /// 当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)
        /// </summary>
        [Description("当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceEntryLong { get { return this.StrategyDatas[0].BarsSinceEntryLong; } }

        /// <summary>
        /// 当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)
        /// </summary>
        [Description("当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceEntryShort { get { return this.StrategyDatas[0].BarsSinceEntryShort; } }

        /// <summary>
        /// 当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)
        /// </summary>
        [Description("当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceLastEntryLong { get { return this.StrategyDatas[0].BarsSinceLastEntryLong; } }

        /// <summary>
        /// 当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)
        /// </summary>
        [Description("当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceLastEntryShort { get { return this.StrategyDatas[0].BarsSinceLastEntryShort; } }

        /// <summary>
        /// 最近平仓位置到当前位置的Bar计数:多(从0开始计数)
        /// </summary>
        [Description("最近平仓位置到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceExitLong { get { return this.StrategyDatas[0].BarsSinceExitLong; } }

        /// <summary>
        /// 最近平仓位置到当前位置的Bar计数:空(从0开始计数)
        /// </summary>
        [Description("最近平仓位置到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric BarsSinceExitShort { get { return this.StrategyDatas[0].BarsSinceExitShort; } }

        /// <summary>
        /// 最近平仓时间:多(yyyyMMdd.HHmmss)
        /// </summary>
        [Description("平仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric ExitDateLong { get { return this.StrategyDatas[0].ExitDateLong; } }

        ///<summary>
        ///	最近平仓时间:空(yyyyMMdd.HHmmss)
        ///</summary>
        [Description("平仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric ExitDateShort { get { return this.StrategyDatas[0].ExitDateShort; } }

        /// <summary>
        /// 最近平仓价格:多
        /// </summary>
        [Description("平仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric ExitPriceLong { get { return this.StrategyDatas[0].ExitPriceLong; } }

        /// <summary>
        /// 最近平仓价格:空
        /// </summary>
        [Description("平仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric ExitPriceShort { get { return this.StrategyDatas[0].ExitPriceShort; } }

        /// <summary>
        /// 当前持仓浮动盈亏:多
        /// </summary>
        [Description("浮动盈亏:多"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric PositionProfitLong { get { return this.StrategyDatas[0].PositionProfitLong; } }

        /// <summary>
        /// 当前持仓浮动盈亏:空
        /// </summary>
        [Description("浮动盈亏:空"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric PositionProfitShort { get { return this.StrategyDatas[0].PositionProfitShort; } }

        /// <summary>
        /// 当前持仓浮动盈亏:净
        /// </summary>
        [Description("浮动盈亏:净"), Category("状态"), ReadOnly(true), Browsable(false)]
        public Numeric PositionProfit { get { return this.StrategyDatas[0].PositionProfit; } }
        #endregion

        #region data[0]的属性：Closes、Highs等等

        /// <summary>
        /// 策略名称
        /// </summary>
        [Description("名称"), Category("设计"), Browsable(false)]
        public string Name
        {
            get
            {
                if (_name == string.Empty)
                {
                    string nameLast = DicProperties.Values.Select(p => GetType().GetField(p.Name, this.Bf)).Where(fieldInfo => fieldInfo != null).Aggregate(string.Empty, (current, fieldInfo) => current + (fieldInfo.GetValue(this) + ","));
                    nameLast = nameLast.TrimEnd(',');
                    this._name = GetType().FullName + (string.IsNullOrEmpty(nameLast) ? "" : ("(" + nameLast + ")"));
                }
                return this._name;
            }
            set { this._name = value; }
        }

        /// <summary>
        /// 合约
        /// </summary>
        [Description("合约"), Category("设计")]
        [Browsable(false)]
        public string InstrumentID { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].InstrumentInfo.InstrumentID; } }

        /// <summary>
        /// 周期数
        /// </summary>
        [Description("周期数"), Category("设计")]
        [Browsable(false)]
        public int Interval { get { return StrategyDatas.Count == 0 ? 0 : this.StrategyDatas[0].Interval; } }

        /// <summary>
        /// 周期类型：默认是分钟
        /// </summary>
        [Description("周期类型"), Category("设计")]
        [Browsable(false)]
        public IntervalType IntervalType { get { return StrategyDatas.Count == 0 ? IntervalType.Min : this.StrategyDatas[0].IntervalType; } }

        /// <summary>
        /// 合约信息
        /// </summary>
        public InstrumentInfo InstrumentInfo { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].InstrumentInfo; } }

        /// <summary>
        /// 当前K线索引(由左向右从0开始)
        /// </summary>
        [Description("当前K线索引"), Category("数据")]
        [Browsable(false)]
        public int CurrentBarIndex { get { return StrategyDatas.Count == 0 ? -1 : this.StrategyDatas[0].CurrentBarIndex; } }

        /// <summary>
        /// 实际行情(无数据时为UpdateTime == null)
        /// </summary>
        [Description("分笔数据"), Category("数据")]
        [Browsable(false)]
        public Tick Tick { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].Tick; } }

        /// <summary>
        /// 时间(yyyyMMdd.HHmmss)
        /// </summary>
        [Browsable(false)]
        public NumericSeries NumDateTimes { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].NumDateTimes; } }

        /// <summary>
        /// 最高价
        /// </summary>
        [Browsable(false)]
        public NumericSeries Highs { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].Highs; } }

        /// <summary>
        /// 最低价
        /// </summary>
        [Browsable(false)]
        public NumericSeries Lows { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].Lows; } }

        /// <summary>
        /// 开盘价
        /// </summary>
        [Browsable(false)]
        public NumericSeries Opens { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].Opens; } }

        /// <summary>
        /// 收盘价
        /// </summary>
        [Browsable(false)]
        public NumericSeries Closes { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].Closes; } }

        /// <summary>
        /// 成交量
        /// </summary>
        [Browsable(false)]
        public NumericSeries Volumes { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].Volumes; } }

        /// <summary>
        /// 持仓量
        /// </summary>
        [Browsable(false)]
        public NumericSeries OpenInts { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].OpenInts; } }

        #endregion

        /// <summary>
		/// 返回策略的参数列表(name:value),以','分隔
		/// </summary>
		/// <returns></returns>
		public string GetParams()
        {
            return "(" + DicProperties.Values.Aggregate(string.Empty, (current, p) => current + (p.Name + ":" + p.Value + ",")).TrimEnd(',') + ")";
        }

        /// <summary>
		/// 从字符串中获得参数
		/// </summary>
		/// <param name="pParameters"></param>
		public void FromString(string pParameters)
        {
            if (string.IsNullOrEmpty(pParameters))
                return;
            for (int i = 0; i < pParameters.Split(',').Length; i++)
            {
                var para = pParameters.Split(',')[i];
                var p = DicProperties.ElementAt(i);
                p.Value.Value = Convert.ChangeType(para, p.Value.Value.GetType());

                var fi = this.GetType().GetField(p.Key, this.Bf);
                if (fi != null)
                    fi.SetValue(this, p.Value.Value);   //配置字段的值
            }
        }

        public override string ToString()
        {
            string nameLast = DicProperties.Values.Aggregate(string.Empty, (current, p) => current + (p.Value + ",")).TrimEnd(',');
            return GetType().FullName + (string.IsNullOrEmpty(nameLast) ? "" : ("(" + nameLast + ")"));
        }

        /// <summary>
        /// 参数设置，DicProperties
		/// </summary>
		/// <param name="pParamName"> </param>
		/// <param name="pValue"> </param>
		public void SetParameterValue(string pParamName, object pValue)
        {
            FieldInfo fi = GetType().GetField(pParamName, this.Bf);
            if (fi == null)
            {
                return;
            }
            fi.SetValue(this, Convert.ChangeType(pValue, fi.FieldType));
            DicProperties[pParamName].Value = Convert.ChangeType(pValue, fi.FieldType);
        }

        /// <summary>
		/// Strategy装载BarSerieses，将历史BarSeries加入到StrategyData中
		/// </summary>
		public void LoadBarSerieses(params BarSeries[] barSeries)
        {
            //清理StrategyData和BarSerieses
            this.StrategyDatas.Clear();
            this.BarSerieses.Clear();

            bool _real = false;
           
            foreach (BarSeries bars in barSeries)
            {
                //装载策略中使用到的BarSeries至StrategyData
                this.BarSerieses.Add(bars);
                StrategyData sd = new StrategyData(bars);
                //每当上个Bar完成更新，则调用策略更新
                bars.OnLastBarComplete += this.UpdateStrategy;
                
                //sd.OnRtnOrder += (o, d) =>
                //{
                //    if (_rtnOrder != null && _real)
                //        _rtnOrder(o, d, this);
                //};
                this.StrategyDatas.Add(sd);
            }

            //初始化TB相关数据，初始化 OpenD、HighD、LowD、CloseD、MinMove、PriceScale
            TBInit(); 

            //调用客户初始化函数
            this.Initialize();

            #region ==========初始化所有指标和用户自定义序列==========

            //所有指标赋值
            this._indicators.Clear();
            foreach (var idx in GetType().GetFields(this.Bf)
                .Where(n => n.FieldType.BaseType == typeof(Indicator)))
            {
                Indicator i = (Indicator)idx.GetValue(this);
                if (i == null)
                {
                    throw new Exception("指标未初始化!");
                }
                this._indicators.Add(i);
            }

            //【添加】重置所有指标的输入,指向新的new 的 strategyData
            foreach (var i in this._indicators)
            {
                ResetInput(i, true);
            }

            //所有用户自定义序列
            this._costomSeries.Clear();
            var fields_In_CurrentType = GetType().GetFields(this.Bf).Where(n => n.FieldType == typeof(NumericSeries)).ToArray();
            foreach (var fi in fields_In_CurrentType)
            {
                //非K线数据:存入
                if (this.NonCustomSeriesNames.IndexOf((string)fi.GetType().GetProperty("Name").GetValue(fi, null)) >= 0)
                {
                    continue;
                }
                NumericSeries customSeries = (NumericSeries)fi.GetValue(this);
                if (customSeries == null)
                {
                    fi.SetValue(this, new NumericSeries());
                }
                this._costomSeries.Add((NumericSeries)fi.GetValue(this)); //setvalue后要重新getvalue
            }

            #endregion ==========初始化所有指标和用户自定义序列==========

            //this.Test();
            _real = true;   //控制实盘发单

            //this.Initialize(); //再次调用客户初始化函数: 首次调用时,数据源不正确
        }

        /// <summary>
		/// 数据更新时调用:先更新指标数据,再调用通过继承实现的OnbarUpdate函数。
        /// 【注意】更新顺序为：StrategyDatas[0].OnTick -> Strategy.Update (Indicator.Update)
		/// </summary>
		public void UpdateStrategy()
        {
            //1、按照Bar来更新Date和Time序列
            foreach (NumericSeries t in this._costomSeries)
            {
                var s = t ?? new NumericSeries();
                while (s.Count < this.StrategyDatas[0].Bars.Count)
                    s.New();
            }

            //2、非K线序列自动增加元素
            foreach (var indicator in this._indicators) 
            {
                this.AddBarInCustomSeriesOfIndicator(indicator);
            }

            //3、TB关键字更新：这里更新了自定义TB的Date和Time序列！
            this.TBKeyWordUpdate();    
            
            //4、跨周期数据更新
            this.periodUpper();

            //5、【指标更新计算】如果策略中的指标，其Input为OCLHVI的，则对指标进行更新计算
            foreach (var indicator in this._indicators)
            {
                //更新指标的计算！
                if (new[] { indicator.IndC, indicator.IndD, indicator.IndH, indicator.IndI,
                    indicator.IndL, indicator.IndO, indicator.IndV }.ToList().IndexOf(indicator.Input) >= 0)
                {
                    indicator.UpdateIndicatorOnBar();
                }
            }

            //6、【策略更新计算】调用用户策略！
            this.OnBarUpdate();
        }

        /// <summary>
        /// 对于Indicator中的非K线序列添加 0 元素
        /// </summary>
        private void AddBarInCustomSeriesOfIndicator(Indicator pIdx)
        {
            //1、自动增加非K线DataSeries（Value、TRs）元素
            pIdx.IsUpdated = false;
            foreach (var customSeries_In_Indicator in pIdx.DictCustomSeries.Values)
            {
                while (customSeries_In_Indicator.Count < pIdx.Input.Count)
                {
                    customSeries_In_Indicator.Add(0); //pIdx.Input[0]
                    continue;
                }
            }
            ////2、对Indicator中的Indicator中的非K线元素自动增加
            //var nestIndicators = pIdx.GetType().GetFields(this.Bf).Where(f => f.FieldType.BaseType == typeof(Indicator)).ToList();
            //foreach (var id in nestIndicators)
            //{
            //    AddBarInCustomSeriesOfIndicator((Indicator)id.GetValue(pIdx));
            //}
        }

        /// <summary>
        /// 重置指标中的dataseries
        /// </summary>
        private void ResetInput(Indicator pIdx, bool pClearValue)
        {
            pIdx.IndD = this.NumDateTimes;
            pIdx.IndO = this.Opens;
            pIdx.IndH = this.Highs;
            pIdx.IndL = this.Lows;
            pIdx.IndC = this.Closes;
            pIdx.IndV = this.Volumes;
            pIdx.IndI = this.OpenInts;
            if (pIdx.Input == null)
            {
                pIdx.Input = this.Closes;   //Indicator的Input为空时，自动赋予Closes。
            }
            else
            {
                MemberInfo mi = GetType().GetMember(pIdx.Input.SeriesName, this.Bf)[0];
                switch (mi.MemberType)
                {
                    case MemberTypes.Field:
                        pIdx.Input = (NumericSeries)((FieldInfo)mi).GetValue(this);
                        break;
                    case MemberTypes.Property:
                        pIdx.Input = (NumericSeries)((PropertyInfo)mi).GetValue(this, null);
                        break;
                }
            }
            //i.Input.bars.Clear();		//会清掉K线数据
            if (pClearValue)
                pIdx.Value.Clear();

            //对于嵌套Indicator进行ResetInput操作
            var nestIndicators = pIdx.GetType().GetFields(this.Bf).Where(f => f.FieldType.BaseType == typeof(Indicator)).ToList();
            foreach (var fi in nestIndicators)
            {
                ResetInput((Indicator)fi.GetValue(pIdx), pClearValue);
            }
        }

        private bool _isTrading = false;
 
        [DisplayName("是否交易中")]
        public bool IsTrading
        {
            get { return this._isTrading; }
            set { this._isTrading = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string PropertyName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName)); }

    }
}
