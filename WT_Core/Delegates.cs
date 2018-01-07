using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WT_Core
{

    /// <summary>
    /// 自定义集合变化事件
    /// </summary>
    /// <param name="pType">pType：添加元素：1；删除元素：-1；更新元素：0</param>
    /// <param name="pNew"></param>
    /// <param name="pOld"></param>
    public delegate void CollectionChange(int pType, object pNew, object pOld);

    /// <summary>
    /// 通知上一个Bar数据生成完毕
    /// </summary>
    public delegate void LastBarComplete();

}
