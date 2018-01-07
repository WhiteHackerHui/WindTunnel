using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numeric = System.Double;
using WT_Core;

namespace WT_Core
{
    static public class Functions
    {
        /// <summary>
        /// 计算指定周期内的数值型序列值的平均值，如果Price序列的数量小于Length，则返回Price[0]。
        /// </summary>
        /// <param name="Price"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        static public Numeric Average(NumericSeries Price, int Length)
        {
            if (Price.Count < Length)
            {
                return Price[0];
            }
            else
            {
                Numeric tmp = 0;
                for (int i = 0; i < Length; i++)
                {
                    tmp += Price[i];
                }
                return tmp/Length;
            }
        } 

        /// <summary>
        /// 计算指定周期内的数值序列值的指数平均值，如果Price序列的数量小于Length，则返回Price[0]
        /// </summary>
        /// <returns>经过指数平均的序列</returns>
        static public NumericSeries XAverage(NumericSeries Price, int Length)
        {
            Numeric sFactor = 2.0 / (Length+1);
            NumericSeries XAvgValues = new NumericSeries();
            for (int i = 0; i < Price.Count; i++)
            {
                int j = Price.Count - i;
                if (i==0)
                {
                    XAvgValues.Add(Price[j - 1]);
                }
                else
                {
                    XAvgValues.Add(XAvgValues[j] + sFactor * (Price[j-1] - XAvgValues[j]));
                }
            }
            return XAvgValues;

        }

        /// <summary>
        /// 求和，包含当前元素值(Price[0])
        /// </summary>
        /// <param name="Price"></param>
        /// <param name="Lenth"></param>
        /// <returns></returns>
        static public Numeric Summation(NumericSeries Price, int Lenth)
        {
            Numeric sumValue = 0;
            for (int i = 0; i < Lenth; i++)
            {
                sumValue += Price[i];
            }
            return sumValue;
        }
    } 
}
