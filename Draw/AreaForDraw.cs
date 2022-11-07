using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.IO;
using StockToolKit.Common;
using System.Runtime.InteropServices;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 包含一个bitmap和基本方法的绘图对象，所有的图形均绘制在此bitmap上
    /// </summary>
    public class AreaForDraw:DrawBase
    {
        ////正确完整的GDI绘图基本方法：https://stackoverflow.com/questions/2302550/bitblt-code-not-working

        /// <summary>
        /// 绘图位图，所有图像描绘在位图上，再由Form显示
        /// </summary>
        private Bitmap _bitmap;
        /// <summary>
        /// 绘图图面
        /// </summary>
        private Graphics _g;
        /// <summary>
        /// bitmap与graphic的句柄建立关联
        /// </summary>
        private IntPtr hOldObject = IntPtr.Zero;
        /// <summary>
        /// 当前跟随线所在股票的信息绘图对象
        /// </summary>
        private DrawCurrentStockInfo _drawCurrentStockInfo;
        /// <summary>
        /// k线升降副刻度尺的数字部分的绘图对象
        /// </summary>
        private DrawKRuleNum _drawKRuleNum;
        /// <summary>
        /// 交易量刻度尺的数字部分的绘图对象
        /// </summary>
        private DrawVRuleNum _drawVRuleNum;
        /// <summary>
        /// 框架绘图对象
        /// </summary>
        private DrawFrame _drawFrame;
        /// <summary>
        /// k线升降副刻度尺绘图对象
        /// </summary>
        private DrawKRules _drawKRules;
        /// <summary>
        /// 交易量刻度尺绘图对象
        /// </summary>
        private DrawVRules _drawVRules;

        //private DrawKVSplitSpace _drawKVSplitSpace;
        /// <summary>
        /// 当前跟随线所在股票在底部显示信息的绘图对象
        /// </summary>
        private DrawStockInfoBottom _drawStockInfoBottom;
        /// <summary>
        /// 鼠标跟随线的绘图对象
        /// </summary>
        private DrawCrossLine _drawCrossLine;
        /// <summary>
        /// k线的绘图对象
        /// </summary>
        private DrawKLine _drawKLine;
        /// <summary>
        /// MA线的绘图对象
        /// </summary>
        private DrawMALine _drawMALine;
        /// <summary>
        /// 交易量绘图对象
        /// </summary>
        private DrawVolumesLine _drawVolumesLine;

        /// <summary>
        /// 计算工具，用来计算并保存stock数据对应的绘制信息
        /// </summary>
        private StocksDrawInfo _sdi = null;
        /// <summary>
        /// 绘制分析结果标记
        /// </summary>
        private DrawAnalyzeResultDayBack _dardb;
        /// <summary>
        /// 处于遮挡、无法绘制十字线等状态的Rectangle集合
        /// 绘制对象的名字作为Dictionary的key
        /// </summary>
        private Dictionary<string, Rectangle[]> _maskrect;

        /// <summary>
        /// 绘图的目标设备Graphics关联的上下文句柄
        /// </summary>
        private IntPtr _desthdc = IntPtr.Zero;

        /// <summary>
        /// 设置是否显示顶部当前股票信息区域
        /// </summary>
        private bool _displayDrawCurrentStockInfo = true;
        /// <summary>
        /// 绘图所用Graphics关联的设备上下文句柄
        /// </summary>
        public IntPtr hDC
        {
            get
            {
                return this._hdc;
            }
        }
        /// <summary>
        /// 绘图所用Bitmap的GDI句柄
        /// </summary>
        public IntPtr hBitmap
        {
            get
            {
                return this._hBitmap;
            }
        }
        /// <summary>
        /// 绘图的目标设备Graphics关联的上下文句柄
        /// </summary>
        public IntPtr DestHdc
        {
            get
            {
                return this._desthdc;
            }
        }

        /// <summary>
        /// k线绘制区域
        /// </summary>
        public Rect KLineArea
        {
            get
            {
                return _drawKLine.Area;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DrawWidth">绘图区域宽</param>
        /// <param name="DrawHeight">绘图区域高</param>
        /// <param name="DestHdc">最终所绘内容要拷贝到此设备上下文句柄关联的目的Graphics中</param>
        public AreaForDraw(int DrawWidth, int DrawHeight, IntPtr DestHdc)
        {
            //_cdi = new ComputeDrawInfo();
            _bitmap = new Bitmap(DrawWidth, DrawHeight);
            //_bitmap.SetResolution(600, 600);
            _g = Graphics.FromImage(_bitmap);
            _hdc = _g.GetHdc();
            _hBitmap = _bitmap.GetHbitmap();
            _desthdc = DestHdc;
            _width = DrawWidth;
            _height = DrawHeight;

            //将bitmap与graphic的句柄建立关联
            hOldObject = SelectObject(_hdc, _hBitmap);
            if (hOldObject == IntPtr.Zero)
                throw new Win32Exception();

            SetBkMode(_hdc, 1);

         }
        /// <summary>
        /// 初始化绘图区域内各绘图对象
        /// </summary>
        public void init()
        {
            ResetGraphics();
        }
        /// <summary>
        /// 重置绘图位图
        /// </summary>
        public void ResetGraphics()
        {
            clearBitmap();
            int CurrentStockInfoHEIGHT = 20;

            if (!_displayDrawCurrentStockInfo)
            {
                CurrentStockInfoHEIGHT = 0;
            }
            
            int StockInfoBottomHEIGHT = 20;
            int KRuleNumWIDTH = 35;
            int VolumesLineHEIGHT = 100;

            _drawCurrentStockInfo = new DrawCurrentStockInfo(0, 0, _width, CurrentStockInfoHEIGHT, _hBitmap, _hdc);

            _drawStockInfoBottom = new DrawStockInfoBottom(0, _height - CurrentStockInfoHEIGHT, _width, StockInfoBottomHEIGHT, _hBitmap, _hdc);

            _drawFrame = new DrawFrame(0, _drawCurrentStockInfo.Bottom + 1, _width - KRuleNumWIDTH - 1, _height - _drawCurrentStockInfo.Height - _drawStockInfoBottom.Height, VolumesLineHEIGHT, _hBitmap, _hdc);

            _drawVRules = new DrawVRules(1, _drawFrame.Bottom - 1 - VolumesLineHEIGHT + 1, _drawFrame.Width - 2, VolumesLineHEIGHT, _hBitmap, _hdc);
            _drawKRules = new DrawKRules(1, _drawCurrentStockInfo.Bottom + 1, _drawFrame.Width - 2, _drawFrame.Height - 1 - VolumesLineHEIGHT - 1, _hBitmap, _hdc);

            _drawKRuleNum = new DrawKRuleNum(_drawFrame.Right + 1, _drawCurrentStockInfo.Bottom + 1, KRuleNumWIDTH, _drawKRules.Height, _hBitmap, _hdc);
            _drawVRuleNum = new DrawVRuleNum(_drawFrame.Right + 1, _drawVRules.Top, KRuleNumWIDTH, VolumesLineHEIGHT, _hBitmap, _hdc);
            _drawCrossLine = new DrawCrossLine(1, _drawCurrentStockInfo.Bottom + 1, _drawFrame.Width - 2, _height - _drawCurrentStockInfo.Height - _drawStockInfoBottom.Height - 1, _hBitmap, _hdc);

            _drawKLine = new DrawKLine(_drawKRules.Left, _drawKRules.Top, _drawKRules.Width, _drawKRules.Height, _hBitmap, _hdc);
            _drawVolumesLine = new DrawVolumesLine(_drawVRules.Left, _drawVRules.Top, _drawVRules.Width, _drawVRules.Height, _hBitmap, _hdc);
            _drawMALine = new DrawMALine(_drawKRules.Left, _drawKRules.Top, _drawKRules.Width, _drawKRules.Height, _hBitmap, _hdc);

            _dardb = new DrawAnalyzeResultDayBack(_drawKRules.Left, _drawKRules.Top, _drawKRules.Width, _drawKRules.Height, _hBitmap, _hdc);
        }

        /// <summary>
        /// 用白色重填bitmap
        /// </summary>
        public void clearBitmap()
        {
            IntPtr oldbrush = IntPtr.Zero;
            IntPtr brush = IntPtr.Zero;
            Color brushcolor = Color.White;

            IntPtr oldpen = IntPtr.Zero;
            IntPtr pen = IntPtr.Zero;
            Color penColor = Color.White;
            //宽度为零时，自动生成1像素宽的笔
            int penWidth = 0;

            try
            {
                SetROP2(_hdc, BinaryRasterOperations.R2_COPYPEN);
                brush = CreateSolidBrush((int)ColorTranslator.ToWin32(brushcolor));
                oldbrush = SelectObject(_hdc, brush);

                pen = CreatePen(PenStyle.PS_SOLID, penWidth, (int)ColorTranslator.ToWin32(penColor));
                oldpen = SelectObject(_hdc, pen);

                Rectangle(_hdc, 0, 0, _bitmap.Width, _bitmap.Height);
            }
            finally
            {
                if (oldbrush != IntPtr.Zero && brush != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldbrush));
                if (oldpen != IntPtr.Zero && pen != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));
            }
        }
        /// <summary>
        /// 在绘图位图上绘制鼠标跟随线
        /// 绘制完成后，需要手动调用Apply()方法才会呈现在目标设备上
        /// </summary>
        /// <param name="p">新中心点</param>
        public bool drawCrossLine(Point p)
        {
            //if(_ddil == null)
            //{
            //    return _drawCrossLine.drawCrossLine(p, 0,0,0,0);
            //}
            //return _drawCrossLine.drawCrossLine(p, _ddil.BarWidth, _ddil.Divide, _ddil.EdgeL, _ddil.EdgeR);
            return _drawCrossLine.drawCrossLine(p, _sdi, xToKIndex(p));
        }
        /// <summary>
        /// 清除鼠标跟随线及绘制位置点记录
        /// </summary>
        public void clearCrossLine()
        {

            _drawCrossLine.clear();
        }
        /// <summary>
        /// 绘制框架
        /// </summary>
        public void drawFrame()
        {
            _drawFrame.drawFrame();
        }
        /// <summary>
        /// 绘制k线升降幅比例标尺
        /// </summary>
        public void drawKRuleNum()
        {
            _drawKRuleNum.drawKRuleNum(1);
        }
        /// <summary>
        /// 绘制k线升降幅比例标尺数字
        /// </summary>
        public void drawKRules()
        {
            _drawKRules.drawKRules(1);
        }
        /// <summary>
        /// 绘制交易量标尺
        /// </summary>
        public void drawVRules()
        {
            _drawVRules.drawVRules(1);
        }
        /// <summary>
        /// 绘制交易量例标尺数字
        /// </summary>
        public void drawVRuleNum()
        {
            _drawVRuleNum.drawVRuleNum(1);
        }
        /// <summary>
        /// 绘制k线
        /// </summary>
        public void drawKLine()
        {
            //_drawKLine.drawKLine(_ddil);
            _drawKLine.drawKLine(_sdi);
        }
        /// <summary>
        /// 绘制ma线
        /// </summary>
        public void drawMALine()
        {
            _drawMALine.drawMALine(_sdi);
        }
        //public void drawKLine(int i, StockDataSet sds)
        //{
        //    _drawKLine.drawKLine(i,sds);
        //}
        /// <summary>
        /// 绘制交易量
        /// </summary>
        /// <param name="i">作为绘制基准的k线索引</param>
        /// <param name="sds">需绘制的股票数据集合</param>
        public void drawVolumeLine()
        {
            //_drawVolumesLine.drawVolumesLine(_ddil);
            _drawVolumesLine.drawVolumesLine(_sdi);
        }
        /// <summary>
        /// 绘制指定bitmap中坐标所在的k线柱信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sds"></param>
        public void drawCurrentStockInfo(Point p, StockDataSet sds)
        {
            if(!_displayDrawCurrentStockInfo)
            {
                return;
            }
            _drawCurrentStockInfo.drawCurrentStockInfo(p, _sdi.BarWidth, _sdi.Divide, _sdi.EdgeL, _sdi.EdgeR, _sdi.EdgeLi, _sdi.EdgeRi,sds);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i">当前日索引</param>
        /// <param name="dlar">分析结果列表</param>
        /// <param name="types">需绘制的分析类型</param>
        public void drawAnalyzeResultDayBack(int i, Dictionary<string, THashTable<AnalyzeResult>> dlar, string[] types)
        {
            if (dlar == null || types == null)
            {
                return;
            }
            //_dardb.drawAnalyzeResultDayBack(i, _ddil, dlar, types);
            _dardb.drawAnalyzeResultDayBack(i, _sdi, dlar, types);
        }
        /// <summary>
        /// 计算指定股票数据的绘制信息
        /// </summary>
        /// <param name="i">参照日</param>
        /// <param name="sds">股票数据集合</param>
        public void Compute(int i, StockDataSet sds)
        {
            //_ddil = _cdi.Compute(i, sds, _drawKLine, _drawVolumesLine);

            _sdi = new StocksDrawInfo(sds);
            _sdi.setParams(i, 0, 0, _drawKLine, _drawVolumesLine);
        }
        /// <summary>
        /// 股票图像根据偏移点再次绘制
        /// </summary>
        /// <param name="p">鼠标挪动后的点</param>
        /// <param name="movetimestamp">时间戳（每次鼠标按下时的）</param>
        public void moveKLine(Point p, long movetimestamp)
        {
            try
            {

                if (_sdi.setParamsForMoved(p, movetimestamp))
                {

                    clearCrossLine();
                    clearBitmap();
                    drawFrame();
                    drawKRules();
                    drawVRules();
                    drawKRuleNum();
                    drawVRuleNum();
                    _dardb.reDraw(_sdi);
                    drawKLine();
                    drawMALine();
                    //_afd.drawKLine(i,sds);
                    drawVolumeLine();
                    Apply();
                }
            }
            catch (Exception err)
            {

            }
        }
        /// <summary>
        /// 获取指定point所在K线柱的索引
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int positionToKIndex(Point p)
        {
            //if (_ddil==null || _ddil.Count==0)
            //{
            //    return -1;
            //}
            if (_sdi == null || (_sdi.EdgeLi == -1 && _sdi.EdgeRi == -1))
            {
                return -1;
            }
            //return _cdi.positionToKIndex(p, _ddil);
            return _sdi.positionToKIndex(p);
        }
        /// <summary>
        /// 获取指定point的x轴坐标所在K线柱的索引
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int xToKIndex(Point p)
        {
            //if (_ddil == null || _ddil.Count == 0)
            //{
            //    return -1;
            //}
            if(_sdi == null || (_sdi.EdgeLi==-1 && _sdi.EdgeRi == -1))
            {
                return -1;
            }
            //return _cdi.xToKIndex(p, _ddil);
            return _sdi.xToKIndex(p);
        }
        /// <summary>
        /// 将绘图位图上的内容呈现在目标设备上
        /// </summary>
        public void Apply()
        {
            if (!BitBlt(_desthdc, 0, 0, _width, _height,
                        _hdc, 0, 0, TernaryRasterOperations.SRCCOPY))
            {
                throw new Win32Exception();
            }
        }
        public void Dispose()
        {
            _drawCrossLine.Dispose();
            _drawFrame.Dispose();
            _drawKRuleNum.Dispose();
            if (hOldObject != IntPtr.Zero) SelectObject(_hdc, hOldObject);
            if (_hBitmap != IntPtr.Zero) DeleteObject(_hBitmap);
            if (_hdc != IntPtr.Zero) _g.ReleaseHdc(_hdc);
            _g.Dispose();
            _bitmap.Dispose();
        }
        /// <summary>
        /// 设置是否显示顶部当前股票信息区域
        /// </summary>
        public bool displayDrawCurrentStockInfo
        {
            get
            {
                return _displayDrawCurrentStockInfo;
            }
            set
            {
                _displayDrawCurrentStockInfo = value;
            }
        }

     }
}
