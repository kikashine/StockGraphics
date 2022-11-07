using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// k线柱的绘制坐标信息
    /// </summary>
    public struct KDataDrawInfo
    {
        /// <summary>
        /// k线柱左沿在bitmap上的x轴坐标
        /// </summary>
        public float Left;
        /// <summary>
        /// k线柱上沿在bitmap上的y轴坐标
        /// </summary>
        public float Top;
        /// <summary>
        /// k线柱右沿在bitmap上的x轴坐标
        /// </summary>
        public float Right;
        /// <summary>
        /// k线柱下沿在bitmap上的y轴坐标
        /// </summary>
        public float Bottom;
        /// <summary>
        /// k线柱上影线在bitmap上的y轴坐标
        /// </summary>
        public float Highest;
        /// <summary>
        /// k线柱下影线在bitmap上的y轴坐标
        /// </summary>
        public float Lowest;//, BaseY, Basei, Currenti;
        //public float BasePrice;
        public Trend Trend;
        /// <summary>
        /// k线柱的绘制信息
        /// </summary>
        /// <param name="left">k线柱左沿在bitmap上的x轴坐标</param>
        /// <param name="top">k线柱上沿在bitmap上的y轴坐标</param>
        /// <param name="right">k线柱右沿在bitmap上的x轴坐标</param>
        /// <param name="bottom">k线柱下沿在bitmap上的y轴坐标</param>
        /// <param name="Highest">k线柱上影线在bitmap上的y轴坐标</param>
        /// <param name="Lowest">k线柱下影线在bitmap上的y轴坐标</param>
        /// <param name="trend">k线柱的升降趋势</param>
        public KDataDrawInfo(int left, int top, int right, int bottom, int Highest, int Lowest, Trend trend)//, int basei, int currenti, float basePrice, int baseY)
        {

            this.Left = left;

            this.Top = top;

            this.Bottom = bottom;

            this.Right = right;
            this.Highest = Highest;
            this.Lowest = Lowest;
            this.Trend = trend;

            //this.BaseY = baseY;
            //this.Basei = basei;
            //this.BasePrice = basePrice;
            //this.Currenti = currenti;

        }
    }
}
