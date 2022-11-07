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
    /// 绘制均线
    /// </summary>
    public class DrawMALine : DrawBase
    {
        /// <summary>
        /// 两k线柱之间间隔像素
        /// </summary>
        private int _divide = 3;
        /// <summary>
        /// 参考价格对应的y轴坐标值，当前欲画线y轴坐标是在_basey之上增减偏移量而来
        /// </summary>
        private int _basey = 0;
        /// <summary>
        /// x轴的中线坐标
        /// </summary>
        private int _basex = 0;
        /// <summary>
        /// k线柱宽度
        /// </summary>
        private int _barwidth = 7;

        public DrawMALine(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC) : base(x, y, Width, Height, hBitmap, hDC)
        {
            _basey = _y + _height / 2;
            _basex = _x + _width / 2;
        }

        /// <summary>
        /// k线柱宽度像素数。默认值为7
        /// </summary>
        public int BarWidth
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
     /// 绘制MA线
     /// </summary>
     /// <param name="sdi">绘图数据</param>
        public void drawMALine(StocksDrawInfo sdi)
        {
            for (int i = sdi.EdgeLi; i <= sdi.EdgeRi - 1; i++)
            {
                drawLine(sdi.DDInfo((object)i).MA, sdi.DDInfo((object)(i+1)).MA);
            }
        }
        /// <summary>
        /// 绘制两天之间的MA线
        /// </summary>
        /// <param name="maa">当日MA绘图数据</param>
        /// <param name="mab">次日MA绘图数据</param>
        private void drawLine(MADataDrawInfo maa, MADataDrawInfo mab)
        {
            //两点xy值
            int maay = -1;
            int maby = -1;
            int maax = -1;
            int mabx = -1;
            //超出绘图范围时边界与ma的交点坐标
            int mcby = -1;
            int mcbx = -1;
 
            SetROP2(_hdc, BinaryRasterOperations.R2_COPYPEN);
            int[] color = new int[4];
            color[0] = 0x00eebbee;
            color[1] = 0x00ddcc77;
            color[2] = 0x0088ccdd;
            color[3] = 0x00c1b095;
            Color penColor = Color.FromArgb(color[0]);
            int penWidth = 1;
            IntPtr pen = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(penColor));
            IntPtr oldpen = SelectObject(_hdc, pen);
            for (int i = 0; i <= maa.Values.Length - 1; i++)
            {
                maay = (int)Math.Round(maa.Values[i].Y);
                maby = (int)Math.Round(mab.Values[i].Y);
                maax = (int)Math.Round(maa.Values[i].X);
                mabx = (int)Math.Round(mab.Values[i].X);

                //两点均在显示区域外
                if ((maay < _y && maby < _y) || (maay > _bottom && maby > _bottom))
                {
                    continue;
                }
                //一点在上沿外
                if ((maay <= _y && maby >= _y) || (maay >= _y && maby <= _y))
                {
                    mcby = _y;
                    //计算与上沿交点的x坐标值
                    if (((maby - maay) * (0 - _right) - (mabx - maax) * (_y - _y)) == 0)
                    {
                        mcbx = 0;
                    }
                    else
                    {
                        mcbx = ((mabx - maax) * (0 - _right) * (_y - maay) -
                                0 * (mabx - maax) * (_y - _y) + maax * (maby - maay) * (0 - _right)) /
                                ((maby - maay) * (0 - _right) - (mabx - maax) * (_y - _y));
                    }
                    if (maay <= _y && maby >= _y)
                    {
                        maay = mcby;
                        maax = mcbx;
                    }
                    else if (maay >= _y && maby <= _y)
                    {
                        maby = mcby;
                        mabx = mcbx;
                    }
                }
                //一点在下沿外
                else if ((maay >= _bottom && maby <= _bottom) || (maay <= _bottom && maby >= _bottom))
                {
                    mcby = _bottom;
                    //计算与下沿交点的y坐标值
                    if(((maby - maay) * (0 - _right) - (mabx - maax) * (_bottom - _bottom))==0)
                    {
                        mcbx = 0;
                    }
                    else
                    {
                        mcbx = ((mabx - maax) * (0 - _right) * (_bottom - maay) -
                        0 * (mabx - maax) * (_bottom - _bottom) + maax * (maby - maay) * (0 - _right)) /
                        ((maby - maay) * (0 - _right) - (mabx - maax) * (_bottom - _bottom));
                    }

                    if (maay >= _bottom && maby <= _bottom)
                    {
                        maay = mcby;
                        maax = mcbx;
                    }
                    else if (maay <= _bottom && maby >= _bottom)
                    {
                        maby = mcby;
                        mabx = mcbx;
                    }
                }
                MoveToEx(_hdc, maax, maay, IntPtr.Zero);
                LineTo(_hdc, mabx, maby);
                if (i == maa.Values.Length - 1)
                {
                    continue;
                }
                penColor = Color.FromArgb(color[i+1]);
                pen = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(penColor));
                DeleteObject(SelectObject(_hdc, pen));
            }
            if (oldpen != IntPtr.Zero && pen != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));
        }
          public void Dispose()
        {
        }

    }
}
