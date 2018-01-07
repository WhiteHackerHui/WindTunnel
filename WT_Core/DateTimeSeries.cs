using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WT_Core
{
    public class DateTimeSeries : Collection<DateTime>
    {
        private readonly string _name;

        internal string SeriesName
        {
            get { return _name; }
        }

        /// <summary>
        /// 构建函数(参数勿填)
        /// </summary>
        /// <param name="pSeriesName"></param>
        public DateTimeSeries([CallerMemberName] string pSeriesName = null)
        {
            _name = pSeriesName;
        }

        /// <summary>
        /// 得到倒数第reverseIndex个元素
        /// </summary>
        /// <param name="reverseIndex">逆序</param>
        /// <returns></returns>
        public new DateTime this[int reverseIndex]
        {
            get
            {
                DateTime rtn = DateTime.MinValue;
                int index = Count - 1 - reverseIndex;
                if ( index >= 0)
                {
                    rtn = base[index];
                }
                return rtn;
            }
            set
            {
                int index = Count - 1 - reverseIndex;
                if (index < 0)
                {
                    return;
                }
                base[index] = value;
            }
        }
    }
}
