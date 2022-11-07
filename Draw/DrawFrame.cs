using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace StockToolKit.Analyze
{
    public class DrawFrame:DrawBase
    {
        private int _VolumesLineHEIGHT;
        /// <summary>
        /// 生成绘制框架线的实例
        /// </summary>
        /// <param name="Width">框架宽度</param>
        /// <param name="Height">框架高度</param>
        /// <param name="g">绘图所用Graphics</param>
        /// <param name="hBitmap">绘图所用Bitmap的GDI句柄</param>
        /// <param name="hDC">绘图所用Graphics关联的设备上下文句柄</param>
        public DrawFrame(int x, int y, int Width, int Height, int VolumesLineHEIGHT,IntPtr hBitmap, IntPtr hDC) : base(x, y, Width, Height, hBitmap, hDC)
        {
            _VolumesLineHEIGHT = VolumesLineHEIGHT;
        }
        /// <summary>
        /// 绘制框架
        /// </summary>
        public void drawFrame()
        {
            IntPtr oldpen = IntPtr.Zero;
            IntPtr pen = IntPtr.Zero;
            try
            {
                Color penColor = Color.Black;
                int penWidth = 1;
                pen = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(penColor));
                oldpen = SelectObject(_hdc, pen);

                SetROP2(_hdc, BinaryRasterOperations.R2_COPYPEN);
                MoveToEx(_hdc, _x, _y, IntPtr.Zero);
                LineTo(_hdc, _x, _bottom);
                MoveToEx(_hdc, _x, _bottom, IntPtr.Zero);
                LineTo(_hdc, _right, _bottom);
                MoveToEx(_hdc, _x, _bottom - 1 - _VolumesLineHEIGHT , IntPtr.Zero);
                LineTo(_hdc, _right, _bottom - 1 - _VolumesLineHEIGHT);
                MoveToEx(_hdc, _right, _bottom, IntPtr.Zero);
                LineTo(_hdc, _right, _y - 1);

                if (oldpen != IntPtr.Zero && pen != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));

            }
            finally
            {

            }
            //_g.DrawLine(new Pen(Color.Black, 1), new Point(1, 1), new Point(1, _height));
            //_g.DrawLine(new Pen(Color.Black, 1), new Point(1, 1), new Point(_width, 1));
        }
        public void Dispose()
        {
            
           
        }

    }
}
