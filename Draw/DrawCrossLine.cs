using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace StockToolKit.Analyze
{
    public class DrawCrossLine:DrawBase
    {
        //正确完整的GDI绘图基本方法：https://stackoverflow.com/questions/2302550/bitblt-code-not-working

        private IntPtr oldpen = IntPtr.Zero;
        private IntPtr pen = IntPtr.Zero;
        private Point _oldp = new Point(-1,-1);
        /// <summary>
        /// 时间戳标记，用来记录上一次绘制跟随线的时间
        /// </summary>
        private long _tmstamp = DateTime.Now.Ticks;
        //private IntPtr _desthdc = IntPtr.Zero;

        public DrawCrossLine(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC) : base(x, y, Width, Height, hBitmap, hDC)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sdi"></param>
        /// <param name="i">k线柱索引。当前鼠标x坐标在该k线柱宽度范围内</param>
        /// <returns></returns>
        public bool drawCrossLine(Point p, StocksDrawInfo sdi, int i)
        {
            if (DateTime.Now.Ticks - _tmstamp < 100000)
            {
                return false;
            }

            //鼠标移动到可绘制区域之外
            if (p.X < _x || p.Y < _y || p.X > _right || p.Y > _bottom)
            {
                return false;
            }
            try
            {
                if (sdi != null && i >= 0)
                {
                    p.X = (int)Math.Round(sdi.DDInfo((object)i).K.Left) + sdi.BarWidth / 2;
                }
                else if(sdi != null)
                {
                    p.X = _oldp.X;
                    //p = _oldp;
                }
                SetROP2(_hdc, BinaryRasterOperations.R2_NOT);
                MoveToEx(_hdc, _oldp.X, _y, IntPtr.Zero);
                LineTo(_hdc, _oldp.X, _y + _height);
                MoveToEx(_hdc, _x, _oldp.Y, IntPtr.Zero);
                LineTo(_hdc, _x + _width, _oldp.Y);
                MoveToEx(_hdc, p.X, _y, IntPtr.Zero);
                LineTo(_hdc, p.X, _y + _height);
                MoveToEx(_hdc, _x, p.Y, IntPtr.Zero);
                LineTo(_hdc, _x + _width, p.Y);
                _oldp = p;

                _tmstamp = DateTime.Now.Ticks;
            }
            finally
            {

            }

            return true;
        }
      
        /// <summary>
        /// 在上一个跟随线的位置绘制反色线，并清除上一次的坐标记录
        /// </summary>
        public void clear()
        {
            SetROP2(_hdc, BinaryRasterOperations.R2_NOT);
            MoveToEx(_hdc, _oldp.X, _y, IntPtr.Zero);
            LineTo(_hdc, _oldp.X, _y + _height);
            MoveToEx(_hdc, _x, _oldp.Y, IntPtr.Zero);
            LineTo(_hdc, _x + _width, _oldp.Y);
            _oldp = new Point(-1, -1);
        }
        public void Dispose()
        {
            if (oldpen != IntPtr.Zero && pen != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));
        }
    }
}
