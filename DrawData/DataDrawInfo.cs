using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 一日stock数据的绘制坐标信息
    /// </summary>
    public class DataDrawInfo
    {
        /// <summary>
        /// k线绘制坐标信息
        /// </summary>
        public KDataDrawInfo K;
        /// <summary>
        /// 交易量绘制坐标信息
        /// </summary>
        public VDataDrawInfo V;
        /// <summary>
        /// 简单移动平均值的绘制坐标信息
        /// </summary>
        public MADataDrawInfo MA;

    }




}
