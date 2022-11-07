using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockToolKit.Common;
using System.Drawing;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 简单移动平均值的绘制坐标信息
    /// </summary>
    public struct MADataDrawInfo
    {

        //public float MA4,MA8,MA12,MA24;
        /// <summary>
        /// 需在bitmap绘制ma的点信息数组
        /// </summary>
        public PointF[] Values;

        /// <summary>
        /// 简单移动平均值的绘制信息
        /// </summary>
        /// <param name="count">当日ma的数量</param>
        public MADataDrawInfo(int count)
        {
            Values = new PointF[count];
        }

    }
}
