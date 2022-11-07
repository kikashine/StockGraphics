using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 绘制交易量标尺
    /// </summary>
    class DrawVRules : DrawBase
    {
        public DrawVRules(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC) : base(x, y, Width, Height, hBitmap, hDC)
        {
        }
        /// <summary>
        /// 绘制交易量标尺
        /// </summary>
        /// <param name="ratio"></param>
        public void drawVRules(int ratio)
        {
            //10%所占的默认像素数
            int tenper = 10;
            //乘以系数后10%所占的默认像素数
            int curtenper = 10 / ratio;
            Color penColor = Color.FromArgb(0x00cccccc);
            int penWidth = 1;
            IntPtr pen1 = CreatePen(PenStyle.PS_DOT, penWidth, (int)ColorTranslator.ToWin32(penColor));
            IntPtr oldpen = SelectObject(_hdc, pen1);
            SetROP2(_hdc, BinaryRasterOperations.R2_COPYPEN);

            int mid = _y + _height / 2;
            MoveToEx(_hdc, _x + 1, mid, IntPtr.Zero);
            LineTo(_hdc, _right - 1, mid);
            //每25%画一条线
            for (int i = (int)(mid - curtenper * 2.5); i > _y;)
            {
                MoveToEx(_hdc, _x + 1, i, IntPtr.Zero);
                LineTo(_hdc, _right - 1, i);
                i = (int)(i - curtenper * 2.5);
            }
            for (int i = (int)(mid + curtenper * 2.5); i < _bottom;)
            {
                MoveToEx(_hdc, _x + 1, i, IntPtr.Zero);
                LineTo(_hdc, _right - 1, i);
                i = (int)(i + curtenper * 2.5);

            }

            if (oldpen != IntPtr.Zero && pen1 != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));
        }
        public void Dispose()
        {


        }
    }
}
