using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Numeric = System.Double;

namespace WT_Core
{
    //数据序列，Close、Open序列等等
    public class NumericSeries : Collection<Numeric>
    {
        //变量
        private readonly string _name;
        
        /// <summary>
        /// 序列名称
        /// </summary>
        internal string SeriesName
        {
            get { return _name; }
        }

        /// <summary>
        /// 指标中的DataSeries在索引Get操作时计算指标
        /// </summary>
        internal Indicator Idc = null;

        
        /// <summary>
        /// 构建函数(参数勿填)，如 DataSeries Closes = New DataSeries(),则Closes的Name属性就是_name，且不会改变(readonly)
        /// </summary>
        /// <param name="pSeriesName"></param>
        public NumericSeries([CallerMemberName] string pSeriesName = null)
        {
            _name = pSeriesName;
        }

        /// <summary>
        /// 获得倒数第index的值
        /// </summary>
        /// <param name="reverseIndex"></param>
        /// <returns></returns>
        public new Numeric this[int reverseIndex]
        {
            get
            {
                Numeric rtn = 0; //Numeric.ZeroNaN;
                //if (this.Idc != null)// && !this.idc.IsOperated)
                //{
                //    //在策略调用时处理:此处会导致循环调用
                //    //this.idc.isUpdated = false;
                //    this.Idc.UpdateIndicator();// .OnBarUpdate();
                //}
                int index = base.Count - 1 - reverseIndex;
                if (index >= 0)
                {
                    rtn = base[index];
                }
                return rtn;
            }
            set
            {
                int index = base.Count - 1 - reverseIndex;
                if (index < 0)
                {
                    return;
                }
                base[index] = value;

                ////【注意】对于Input序列，所有修改Input[0]的操作都会自动调用更新指标！
                //if (this.Idc != null && reverseIndex == 0)// && !this.idc.IsOperated)
                //{
                //    //在策略调用时处理:此处会导致循环调用
                //    //this.idc.isUpdated = false;
                //    this.Idc.UpdateIndicator();// .OnBarUpdate();
                //}
            }
        }

        public void New()
        {
            if (Count == 0)
                base.Add(0);
            else
                base.Add(this[0]);
        }

        public new void Add(Numeric p)
        {
            base.Add(p);
        }

    }
}
