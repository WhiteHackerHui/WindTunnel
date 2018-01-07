using System.Collections.Generic;
using System.Reflection;
using System.Collections.Concurrent;
using Numeric = System.Double;
using System.Linq;

namespace WT_Core
{
    /// <summary>
    /// 指标，虚类
    /// </summary>
    public abstract class Indicator
    {
        //K线数据:由Strategy赋值
        internal NumericSeries IndD, IndO, IndH, IndL, IndC, IndV, IndI;

        /// <summary>
		/// 每个tick只处理一次; 策略中被调用
		/// </summary>
		internal bool IsUpdated = false;

        /// <summary>
        /// 字段绑定标识，注意属性不是字段！
        /// </summary>
        internal readonly BindingFlags Bf = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// 所有Indicator中非用户序列的名称，注意：Value字段并不在内，意味着Value不会像Closes等自动增加！
        /// </summary>
        internal readonly List<string> NonCustomSeriesNames = new List<string>( new[] { "IndD", "IndH", "IndL", "IndO", "IndC", "IndV", "IndI", "Input" }); //"Input",

        #region 开高低收等属性

        protected NumericSeries NumDateTimes { get { return this.IndD; } }
        protected NumericSeries Highs { get { return this.IndH; } }
        protected NumericSeries Lows { get { return this.IndL; } }
        protected NumericSeries Opens { get { return this.IndO; } }
        protected NumericSeries Closes { get { return this.IndC; } }
        protected NumericSeries Volumes { get { return this.IndV; } }
        protected NumericSeries OpenInts { get { return this.IndI; } }
        
        #endregion 开高低收等属性

        /// <summary>
        /// 指标构造器
        /// </summary>
        /// <param name="input">输入参数序列，如对于SUM指标，输入参数序列Input就是计算对象</param>
        /// <param name="periods">周期参数，由于可能不止一个，所以设置为参数数组 params；对于SMA，由于周期参数只有一个，所以就只输入一个周期值</param>
        protected Indicator(NumericSeries input, params Numeric[] periods)
        {
            //输入序列（Input）和周期参数（Periods）
            this.Input = input;
            this.Periods = periods;

            //获得父类、子类（Indicator）中所有字段（field）,选出DataSeries类型的实例。
            var fields_In_CurrentType = GetType().GetFields(this.Bf);
            foreach (FieldInfo fi in fields_In_CurrentType)
            {
                bool isCustomSeries = !(NonCustomSeriesNames.IndexOf(fi.Name) >= 0);
                if (isCustomSeries && fi.FieldType == typeof(NumericSeries))
                {
                    NumericSeries customSeries = (NumericSeries)fi.GetValue(this);
                    //若用户自定义序列为null则初始化
                    if (customSeries == null)
                    {
                        fi.SetValue(this, new NumericSeries());
                        customSeries = (NumericSeries)fi.GetValue(this);
                    }
                    //Value等其他用户在子类中定义的DataSeries添加到CustomSeries
                    this.DictCustomSeries[fi.Name] = customSeries;
                    
                    //Value、自定义序列Input（TRs）
                    //customSeries.Idc = this;

                }
            }
        }

        /// <summary>
		/// 逆序索引，对于单值指标，返回倒数第index个值
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Numeric this[int index] { get { return Value[index]; } set { Value[index] = value; } }

        /// <summary>
        /// 自定义指标中声明的 DataSeries，例如自定义序列：K、D、J
        /// </summary>
        public ConcurrentDictionary<string, NumericSeries> DictCustomSeries = new ConcurrentDictionary<string, NumericSeries>();

        /// <summary>
        /// 输入参数序列，如对于SUM指标，输入参数序列Input就是计算对象
        /// </summary>
        public NumericSeries Input = null;

        /// <summary>
		/// 周期参数数组
		/// </summary>
		public Numeric[] Periods = null;

        /// <summary>
        /// 单值指标
        /// </summary>
        public NumericSeries Value = new NumericSeries();

        /// <summary>
		/// 当前bar索引(0开始)
		/// </summary>
		protected int CurrentBarIndex { get { return this.IndD.Count - 1; } }

        /// <summary>
		/// 输入序列数据点数量
		/// </summary>
		protected int Count { get { return this.IndD.Count; } }

        /// <summary>
		/// 取得参数组中的首个参数
		/// </summary>
		protected int Period { get { return (int)this.Periods[0]; } }

        /// <summary>
		/// K线更新，指标计算就在OnBarUpdate中进行。【指标都是根据Bar来计算的】
		/// </summary>
		public abstract void OnBarUpdate();

        /// <summary>
		/// 引发Indicator中的OnBarUpdate
		/// </summary>
		internal void UpdateIndicatorOnBar()
        {
            //如果Bar更新后，指标没有更新，则更新指标；反之则忽略。
            if (!this.IsUpdated)
            {
                this.IsUpdated = true;
                OnBarUpdate();
            }
        }

    }
}
