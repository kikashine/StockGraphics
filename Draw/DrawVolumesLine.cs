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
    /// 绘制股票的交易量
    /// </summary>
    public class DrawVolumesLine : DrawBase
    {
        /// <summary>
        /// 两k线柱之间间隔像素
        /// </summary>
        private int _divide = 3;
        /// <summary>
        /// 参考价格对应的y轴坐标值，当前欲画线y轴坐标是在_basey之上增减偏移量而来
        /// </summary>
        //private int _basey = 0;
        /// <summary>
        /// x轴的中线坐标
        /// </summary>
        private int _basex = 0;
        /// <summary>
        /// y轴方向价格相对增减1%对应的像素数量
        /// </summary>
        private float _onePercentPixel = 1;
        /// <summary>
        /// k线柱宽度
        /// </summary>
        private int _barwidth = 7;

      
        //private KDayDataList _kdlist;
        public DrawVolumesLine(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC) : base(x, y, Width, Height, hBitmap, hDC)
        {
            //_basey = _y + _height / 2;
            _basex = _x + _width / 2;
        }

       
        /// <summary>
        /// 两个k线柱之间的间隔像素数。默认值为3
        /// </summary>
        public int Divide
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
        /// 百分之一的升降幅在Y轴上对应的像素数
        /// </summary>
        public float OnePercentPixels
        {
            get
            {
                return _onePercentPixel;
            }
            set
            {
                _onePercentPixel = value;
            }
        }

        //public void drawVolumesLine(DataDrawInfoList<DataDrawInfo> ddil)
        //{
        //    for (int i = 0; i < ddil.Count; i++)
        //    {
        //        drawVBar(ddil[i].V);
        //    }
        //}
        public void drawVolumesLine(StocksDrawInfo sdi)
        {
            for (int i = sdi.EdgeLi; i <= sdi.EdgeRi; i++)
            {
                drawVBar(sdi.DDInfo((object)i).V);
            }
        }

        public void drawVBar(VDataDrawInfo v)
        {
            IntPtr oldbrush = IntPtr.Zero;
            IntPtr brush = IntPtr.Zero;
            Color brushcolor = Color.FromArgb(0x00cc99ff);
            Color penColor = Color.FromArgb(0x00cc99ff);

            if (v.Trend == Trend.Fall)
            {
                penColor = Color.FromArgb(0x0099cc99);
                brushcolor = Color.FromArgb(0x0099cc99);
            }
            int penWidth = 1;
            IntPtr pen = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(penColor));
            IntPtr oldpen = SelectObject(_hdc, pen);
            brush = CreateSolidBrush((int)ColorTranslator.ToWin32(brushcolor));
            oldbrush = SelectObject(_hdc, brush);

            SetROP2(_hdc, BinaryRasterOperations.R2_COPYPEN);

            int kx =  (int)Math.Round(v.Left);
            int voly = (int)Math.Round(v.Top);

            if (v.Trend == Trend.Fall)
            {
                MoveToEx(_hdc, kx, voly, IntPtr.Zero);
                LineTo(_hdc, kx, _bottom + 1);
                MoveToEx(_hdc, kx, voly, IntPtr.Zero);
                LineTo(_hdc, kx + _barwidth - 1 + 1, voly);
                MoveToEx(_hdc, kx + _barwidth - 1, voly, IntPtr.Zero);
                LineTo(_hdc, kx + _barwidth - 1, _bottom + 1);
            }
            else
            {
                Rectangle(_hdc, kx, voly, kx + _barwidth, _bottom + 1);
            }
            if (oldpen != IntPtr.Zero && pen != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));
            if (oldbrush != IntPtr.Zero && brush != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldbrush));
        }

        //    public void drawVolumesLine(int i, StockDataSet sds)
        //{
        //    /*
        //     */
        //    drawVolumeBar(i, i, sds.Volume(i), sds.Volume(sds.MaxVolumeIndex), sds.Open(i), sds.Close(i));

        //    //向左侧绘制
        //    for (int j = i - 1; j >= 0; j--)
        //    {
        //        //超出显示范围
        //        if ((j - i) * (_barwidth + _divide) + _basex <= 0)
        //        {
        //            //_edgeL = (j - i + 1) * (_barwidth + _divide) + _basex;
        //            break;
        //        }
        //        drawVolumeBar(i, j, sds.Volume(j), sds.Volume(sds.MaxVolumeIndex), sds.Open(j),sds.Close(j));
        //    }

        //    for (int j = i + 1; j < sds.Length - 1; j++)
        //    {
        //        if ((j - i) * (_barwidth + _divide) + _basex >= _width - _barwidth)
        //        {
        //            //_edgeR = (j - i) * (_barwidth + _divide) + _basex - _divide;
        //            break;
        //        }
        //        drawVolumeBar(i, j, sds.Volume(j), sds.Volume(sds.MaxVolumeIndex), sds.Open(j), sds.Close(j));
        //    }
        //}

        //private void drawVolumeBar(int i, int j, double volume, double maxvolume, float open, float close)
        //{
        //    IntPtr oldbrush = IntPtr.Zero;
        //    IntPtr brush = IntPtr.Zero;
        //    Color brushcolor = Color.FromArgb(0x00cc99ff);
        //    Color penColor = Color.FromArgb(0x00cc99ff);

        //    if (open > close)
        //    {
        //        penColor = Color.FromArgb(0x0099cc99);
        //        brushcolor = Color.FromArgb(0x0099cc99);
        //    }
        //    int penWidth = 1;
        //    IntPtr pen = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(penColor));
        //    IntPtr oldpen = SelectObject(_hdc, pen);
        //    brush = CreateSolidBrush((int)ColorTranslator.ToWin32(brushcolor));
        //    oldbrush = SelectObject(_hdc, brush);

        //    SetROP2(_hdc, BinaryRasterOperations.R2_COPYPEN);

        //    int kx = (j - i) * (_barwidth + _divide) + _basex;

        //    double top = maxvolume;

        //    //int ctopy = _bottom - (int)((double)top / (double)top * 100 * _onePercentPixel);


        //    int voly = _bottom - (int)(volume / top * 100 * _onePercentPixel);

        //    if (open > close)
        //    {
        //        MoveToEx(_hdc, kx, voly, IntPtr.Zero);
        //        LineTo(_hdc, kx, _bottom + 1);
        //        MoveToEx(_hdc, kx, voly, IntPtr.Zero);
        //        LineTo(_hdc, kx + _barwidth - 1 + 1, voly);
        //        MoveToEx(_hdc, kx + _barwidth - 1, voly, IntPtr.Zero);
        //        LineTo(_hdc, kx + _barwidth - 1, _bottom + 1);
        //    }
        //    else
        //    {
        //        Rectangle(_hdc, kx, voly, kx + _barwidth, _bottom + 1);
        //    }
        //    if (oldpen != IntPtr.Zero && pen != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));
        //    if (oldbrush != IntPtr.Zero && brush != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldbrush));

        //    //penColor = Color.FromArgb(0x00cc9933);
        //    //pen = CreatePen(PenStyle.PS_DASH, penWidth, (int)ColorTranslator.ToWin32(penColor));
        //    //oldpen = SelectObject(_hdc, pen);
        //    //kx = kx - _divide / 2;
        //    //if(kx<_x)
        //    //{
        //    //    kx = _x;
        //    //}
        //    //MoveToEx(_hdc, kx, ctopy, IntPtr.Zero);
        //    //LineTo(_hdc, kx + _barwidth - 1 + _divide / 2, ctopy);

        //    //if (oldpen != IntPtr.Zero && pen != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));


        //}


        public void Dispose()
        {
        }
    }
}
