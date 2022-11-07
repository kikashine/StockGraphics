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
    /// 绘制分析结果标记
    /// </summary>
    public class DrawAnalyzeResultDayBack : DrawBase
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
        /// y轴方向价格相对增减1%对应的像素数量
        /// </summary>
        private float _onePercentPixel = 5;
        /// <summary>
        /// k线柱宽度
        /// </summary>
        private int _barwidth = 7;

        private Dictionary<string, THashTable<AnalyzeResult>> _dlar;

        private string[] _types;

        private int _cindex;
        public DrawAnalyzeResultDayBack(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC) : base(x, y, Width, Height, hBitmap, hDC)
        {
            _basey = _y + _height / 2;
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
        /// <summary>
        /// 绘制分析结果标记
        /// </summary>
        /// <param name="cindex">当前日索引</param>
        /// <param name="sdi">根据股票价格、交易量、绘图区域信息等数据获得绘图数据</param>
        /// <param name="dlar">分析结果列表。分析类型作为Dictionary的索引，类型对应的分析结果列表作为Dictionary中该索引对应的值</param>
        /// <param name="types">需绘制的分析类型</param>
        public void drawAnalyzeResultDayBack(int cindex, StocksDrawInfo sdi, Dictionary<string, THashTable<AnalyzeResult>> dlar, string[] types)
        {
            _dlar = dlar;
            _types = types;
            _cindex = cindex;
            draw(sdi);
        }
        /// <summary>
        /// 根据给出的k线绘制数据重新绘制分析结果标记
        /// </summary>
        /// <param name="sdi">根据股票价格、交易量、绘图区域信息等数据获得绘图数据</param>
        public void reDraw(StocksDrawInfo sdi)
        {
            if (_dlar == null || _types == null)
            {
                return;
            }
            draw(sdi);
        }
        private void draw(StocksDrawInfo sdi)
        {
            //与结果中各日对应的分析类型。k线数据索引值作为THashTable的索引，各日对应的分析类型列表作为List<string>
            THashTable<List<string>> ctypes = new THashTable<List<string>>();
            //改变分析结果列表的数据结构，变为以k线数据索引值作为索引，该日对应结果类型列表作为值
            for (int i = 0; i < _types.Length; i++)
            {
                //结果中存在分析类型
                if (_dlar.ContainsKey(_types[i]))
                {
                    //遍历该结果类型的结果
                    for (int j = 0; j < _dlar[_types[i]].Count; j++)
                    {
                        //int类型的key是以object类型存储在thashtable中的
                        if (!ctypes.ContainsKey((object)(_dlar[_types[i]][j].Index)))
                        {
                            ctypes.Add(_dlar[_types[i]][j].Index, new List<string>());
                        }
                        ctypes[(object)_dlar[_types[i]][j].Index].Add(_types[i]);

                    }
                }
            }

            //遍历绘制标记
            foreach (int idx in ctypes.Keys)
            {
                if (idx >= sdi.EdgeLi && idx <= sdi.EdgeRi)
                {
                    if (idx == _cindex)
                    {
                        DrawCurentBack(sdi.DDInfo((object)idx).K);
                    }
                    drawBar(sdi.DDInfo((object)idx).K, ctypes[(object)idx]);
                }
            }
        }
        /// <summary>
        /// 绘制指定k线柱和指定类型的标记
        /// </summary>
        /// <param name="k">指定k线柱的绘制信息</param>
        /// <param name="types">类型列表</param>
        private void drawBar(KDataDrawInfo k, List<string> types)
        {
            int khighy = (int)Math.Round(k.Top);
            int klowy = (int)Math.Round(k.Bottom);
            int kx = (int)Math.Round(k.Left);
            int khighesty = (int)Math.Round(k.Highest);
            int klowesty = (int)Math.Round(k.Lowest);

            //IntPtr oldbrush = IntPtr.Zero;
            int penWidth = 1;
            //IntPtr pen = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(Color.Orange));

            IntPtr[] oldpen = new IntPtr[3];
            IntPtr[] pen = new IntPtr[3];
            pen[0] = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(Color.LightSkyBlue));
            pen[1] = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(Color.Gold));
            pen[2] = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(Color.FromArgb(0x00ffffaa)));

            IntPtr[] oldbrush = new IntPtr[3];
            IntPtr[] brush = new IntPtr[3];
            brush[0] = CreateSolidBrush((int)ColorTranslator.ToWin32(Color.LightSkyBlue));
            brush[1] = CreateSolidBrush((int)ColorTranslator.ToWin32(Color.Gold));
            brush[2] = CreateSolidBrush((int)ColorTranslator.ToWin32(Color.FromArgb(0x00ffffaa)));
     

            SetROP2(_hdc, BinaryRasterOperations.R2_COPYPEN);

            int[] h = new int[types.Count];
            //主体向上方完全超出显示范围
            if (klowy < _y && khighy < _y)
            {

            }
            //主体上沿向上方超出显示范围
            else if (khighy - 3 - (types.Count - 1) * 4 - 3 < _y && klowy >= _y)
            {
                //khighy = _y;
                if (klowy + 3 + (types.Count - 1) * 4 + 3 > _bottom)
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        oldpen[i] = SelectObject(_hdc, pen[i]);
                        oldbrush[i] = SelectObject(_hdc, brush[i]);
                        Rectangle(_hdc, kx + _barwidth / 2 - 2, _bottom  - (i + 1) * 4, kx + _barwidth / 2 + 3, _bottom  - (i + 1) * 4 + 1 + 3);
                    }
                }
                else
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        oldpen[i] = SelectObject(_hdc, pen[i]);
                        oldbrush[i] = SelectObject(_hdc, brush[i]);
                        Rectangle(_hdc, kx + _barwidth / 2 - 2, klowy + 3 + i * 4, kx + _barwidth / 2 + 3, klowy + 3 + i * 4 + 1 + 3);
                    }
                }
            }
            //主体整体向下方超出显示范围
            else if (klowy > _bottom && khighy > _bottom)
            {

            }
            //主体下沿向下方超出显示范围
            else if (klowy + 3 + (types.Count - 1) * 4 + 3 > _bottom && khighy <= _bottom)
            {
                //klowy = _bottom;
                if (khighy - 2 - (types.Count - 1) * 4 - 3 < _y)
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        oldpen[i] = SelectObject(_hdc, pen[i]);
                        oldbrush[i] = SelectObject(_hdc, brush[i]);
                        Rectangle(_hdc, kx + _barwidth / 2 - 2, _y + i * 4, kx + _barwidth / 2 + 3, _y + i * 4 + 1 + 3);
                    }
                }
                else
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        oldpen[i] = SelectObject(_hdc, pen[i]);
                        oldbrush[i] = SelectObject(_hdc, brush[i]);
                        Rectangle(_hdc, kx + _barwidth / 2 - 2, khighy - 3 - i * 4, kx + _barwidth / 2 + 3, khighy - 3 - i * 4  - 1 - 3);
                    }
                }
            }
            //主体在显示范围内
            else
            {
                for (int i = 0; i < types.Count; i++)
                {
                    oldpen[i] = SelectObject(_hdc, pen[i]);
                    oldbrush[i] = SelectObject(_hdc, brush[i]);
                    Rectangle(_hdc, kx + _barwidth / 2 - 2, klowy + 3 + i * 4, kx + _barwidth / 2 + 3, klowy + 3 + i * 4 + 1 + 3);
                }
            }

            if (oldbrush[0] != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldbrush[0]));
            for (int i = 0; i < oldbrush.Length; i++)
            {
                if (oldbrush[i] != IntPtr.Zero)
                {
                    DeleteObject(oldbrush[i]);
                }
                if (brush[i] != IntPtr.Zero)
                {
                    DeleteObject(brush[i]);
                }
            }
            if (oldpen[0] != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen[0]));
            for (int i = 0; i < oldpen.Length; i++)
            {
                if (oldpen[i] != IntPtr.Zero)
                {
                    DeleteObject(oldpen[i]);
                }
                if (pen[i] != IntPtr.Zero)
                {
                    DeleteObject(pen[i]);
                }
            }
        }
        /// <summary>
        /// 绘制当前日的背景色
        /// </summary>
        /// <param name="k">指定k线柱的绘制信息</param>
        private void DrawCurentBack(KDataDrawInfo k)
        {
            int khighy = (int)Math.Round(k.Top);
            int klowy = (int)Math.Round(k.Bottom);
            int kx = (int)Math.Round(k.Left);

            int penWidth = 1;
            IntPtr pen = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(Color.Orange));
            IntPtr oldpen = SelectObject(_hdc, pen);
            //IntPtr oldbrush = IntPtr.Zero;
            IntPtr oldbrush = new IntPtr();
            IntPtr brush = CreateSolidBrush((int)ColorTranslator.ToWin32(Color.Orange));

            oldbrush = SelectObject(_hdc, brush);

            SetROP2(_hdc, BinaryRasterOperations.R2_COPYPEN);

            //主体向上方完全超出显示范围
            if (klowy < _y && khighy < _y)
            {

            }
            //主体上沿向上方超出显示范围
            else if (khighy - 2 < _y && klowy + 2 >= _y)
            {
                //khighy = _y;
                if (klowy + 2  > _bottom)
                {
                    Rectangle(_hdc, kx - 2, _y, (int)Math.Round(k.Right) + 3, _bottom + 1);
                }
                else
                {
                    Rectangle(_hdc, kx - 2, _y, (int)Math.Round(k.Right + 3), klowy + 3);
                }
            }
            //主体整体向下方超出显示范围
            else if (klowy > _bottom && khighy > _bottom)
            {

            }
            //主体下沿向下方超出显示范围
            else if (klowy  + 2> _bottom && khighy  <= _bottom)
            {
                //klowy = _bottom;
                if (khighy - 2 < _y)
                {
                    Rectangle(_hdc, kx - 2, _y, (int)Math.Round(k.Right + 3), _bottom + 1);
                }
                else
                {
                    Rectangle(_hdc, kx - 2, khighy - 2, (int)Math.Round(k.Right + 3), _bottom + 1);
                }
            }
            //主体在显示范围内
            else
            {
                Rectangle(_hdc, kx - 2, khighy - 2, (int)Math.Round(k.Right + 3), klowy + 3);
            }
            if (oldbrush != IntPtr.Zero && brush != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldbrush));
            if (oldpen != IntPtr.Zero && pen != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));
        }
        public void Dispose()
        {
        }

    }
}

