using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 交易量的绘制坐标信息
    /// </summary>
    public struct VDataDrawInfo
    {

        public float Left, Top, Right, Bottom;
        public Trend Trend;

        /// <summary>
        /// 交易量的绘制信息
        /// </summary>
        /// <param name="left">交易量的左沿在bitmap中的x坐标</param>
        /// <param name="top">交易量的上沿在bitmap中的y坐标</param>
        /// <param name="right">交易量的右沿在bitmap中的x坐标</param>
        /// <param name="bottom">交易量的下沿在bitmap中的y坐标</param>
        /// <param name="trend">当日价格的升降趋势</param>
        public VDataDrawInfo(int left, int top, int right, int bottom, Trend trend)
        {

            this.Left = left;

            this.Top = top;

            this.Bottom = bottom;

            this.Right = right;

            this.Trend = trend;


        }

    }
}
