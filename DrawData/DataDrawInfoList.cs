using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 废弃
    /// </summary>
    /// <typeparam name="DataDrawInfo"></typeparam>
    public class DataDrawInfoList<DataDrawInfo> : THashTable<DataDrawInfo>
    {
        /// <summary>
        /// 两k线柱之间间隔像素
        /// </summary>
        private int _divide = 3;
        /// <summary>
        /// k参考价格对应的y轴坐标值，当前欲画线y轴坐标是在_basey之上增减偏移量而来
        /// </summary>
        private int _basey = 0;
        /// <summary>
        /// x轴的中线坐标
        /// </summary>
        private int _basex = 0;
        /// <summary>
        /// 绘制的参照日，其open作为初始中间位置
        /// </summary>
        private int _basei = -1;
        /// <summary>
        /// K线y轴方向价格相对增减1%对应的像素数量
        /// </summary>
        private float _oneKPercentPixel = 5;
        /// <summary>
        /// 交易量y轴方向价格相对增减1%对应的像素数量
        /// </summary>
        private float _oneVPercentPixel = 1;
        /// <summary>
        /// k线柱宽度
        /// </summary>
        private int _barwidth = 7;

        /// <summary>
        /// 最右侧k线柱的右边在bitmap上x值
        /// </summary>
        private int _edgeR = 0;
        /// <summary>
        /// 最左侧k线柱的左边在bitmap上x值
        /// </summary>
        private int _edgeL = 0;

        /// <summary>
        /// 最右侧k线柱的索引值
        /// </summary>
        private int _edgeRi = 0;

        /// <summary>
        /// 最左侧k线柱的索引值
        /// </summary>
        private int _edgeLi = 0;

        /// <summary>
        /// 最左侧k线柱左竖边在bitmap上的x轴坐标
        /// </summary>
        public  int EdgeL
        {
            get
            {
                return _edgeL;

            }
            set
            {
                _edgeL = value;
            }
        }
        /// <summary>
        /// 最右侧k线柱右竖边在bitmap上的x轴坐标
        /// </summary>
        public  int EdgeR
        {
            get
            {
                return _edgeR;

            }
            set
            {
                _edgeR = value;
            }
        }
        /// <summary>
        /// 最左侧k线柱的索引值
        /// </summary>
        public  int EdgeLi
        {
            get
            {
                return _edgeLi;

            }
            set
            {
                _edgeLi = value;
            }
        }
        /// <summary>
        /// 最右侧k线柱的索引值
        /// </summary>
        public  int EdgeRi
        {
            get
            {
                return _edgeRi;

            }
            set
            {
                _edgeRi = value;
            }
        }
        /// <summary>
        /// 两个k线柱之间的间隔像素数。默认值为3
        /// </summary>
        public  int Divide
        {
            get
            {
                return _divide;
            }
            set
            {
                _divide = value;
            }
        }
        /// <summary>
        /// k线柱宽度像素数。默认值为7
        /// </summary>
        public  int BarWidth
        {
            get
            {
                return _barwidth;
            }
            set
            {
                _barwidth = value;
            }
        }
        /// <summary>
        /// 百分之一的K升降幅在Y轴上对应的像素数
        /// </summary>
        public  float OneKPercentPixels
        {
            get
            {
                return _oneKPercentPixel;
            }
            set
            {
                _oneKPercentPixel = value;
            }
        }
        /// <summary>
        /// 百分之一的交易量升降幅在Y轴上对应的像素数
        /// </summary>
        public float OneVPercentPixels
        {
            get
            {
                return _oneVPercentPixel;
            }
            set
            {
                _oneVPercentPixel = value;
            }
        }

        public int Basei
        {
            get
            {
                return _basei;
            }
            set
            {
                _basei = value;
            }
        }
    }
}
