using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using StockToolKit.Common;
using System.Collections.Generic;

namespace StockToolKit.Analyze
{
    class PanelDraw : Panel
    {
        //       [DllImport("gdi32.dll")]

        //       private static extern bool BitBlt(
        //   IntPtr hdcDest, //目标设备的句柄  
        //   int nXDest,     // 目标对象的左上角的X坐标  
        //   int nYDest,     // 目标对象的左上角的Y坐标  
        //   int nWidth,     // 目标对象的矩形的宽度  
        //   int nHeight,    // 目标对象的矩形的长度  
        //   IntPtr hdcSrc,  // 源设备的句柄  
        //   int nXSrc,      // 源对象的左上角的X坐标  
        //   int nYSrc,      // 源对象的左上角的Y坐标  
        //   TernaryRasterOperations dwRop       // 光栅的操作值 指源位图与目标位图以及图案刷的颜色值进行布尔运算的方式，以下列出了常用的光栅操作码及含义
        //                                       /*
        //                           BLACKNESS 用黑色填充目标矩形区域.
        //                           DSTINVERT 将目标矩形图象进行反相. 
        //                           MERGECOPY 将源矩形图象与指定的图案刷(Pattern)进行布尔"与"运算. 
        //                           MERGEPAINT 将源矩形图形经过反相后，与目标矩形图象进行布尔"或"运算.
        //                           NOTSRCCOPY 将源矩形图象经过反相后，复制到目标矩形上.
        //                           NOTSRCERASE 先将源矩形图象与目标矩形图象进行布尔"或"运算，然后再将得图象进行反相.
        //                           PATCOPY 将指定的图案刷复制到目标矩形上.
        //                           PATINVERT 将指定的图案刷与目标矩形图象进行布尔"异或"运算.
        //                           PATPAINT 先将源矩形图象进行反相，与指定的图案刷进行布尔"或"运算，再与目标矩形图象进行布尔"或"运算SRCAND 将源矩形图象与目标矩形图象进行布尔"与"运算.
        //                           SRCCOPY 将源矩形图象直接复制到目标矩形上.
        //                           SRCERASE 将目标矩形图象进行反相，再与源矩形图象进行布尔"与"运算.
        //                           SRCINVERT 将源矩形图象与目标矩形图象进行布尔"异或"运算.
        //                           SRCPAINT 将源矩形图象与目标矩形图象进行布尔"或"运算.
        //                           WHITENESS 用白色填充目标矩形区域.
        //                                        */
        //);
        //       protected enum TernaryRasterOperations
        //       {

        //           SRCCOPY = 0x00CC0020, /* dest = source */

        //           SRCPAINT = 0x00EE0086, /* dest = source OR dest */

        //           SRCAND = 0x008800C6, /* dest = source AND dest */

        //           SRCINVERT = 0x00660046, /* dest = source XOR dest */

        //           SRCERASE = 0x00440328, /* dest = source AND (NOT dest ) */

        //           NOTSRCCOPY = 0x00330008, /* dest = (NOT source) */

        //           NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */

        //           MERGECOPY = 0x00C000CA, /* dest = (source AND pattern) */

        //           MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest */

        //           PATCOPY = 0x00F00021, /* dest = pattern */

        //           PATPAINT = 0x00FB0A09, /* dest = DPSnoo */

        //           PATINVERT = 0x005A0049, /* dest = pattern XOR dest */

        //           DSTINVERT = 0x00550009, /* dest = (NOT dest) */

        //           BLACKNESS = 0x00000042, /* dest = BLACK */

        //           WHITENESS = 0x00FF0062, /* dest = WHITE */

        //       };
        //       public const int SRCCOPY = 0xCC0020;
        //       [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        //       public static extern IntPtr CreateCompatibleDC(IntPtr hdcPtr);
        //         [DllImport("gdi32.dll", ExactSpelling = true)]
        //       public static extern bool DeleteDC(IntPtr hdcPtr);
        //       [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        //       public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// ClickKBar事件的委托
        /// </summary>
        /// <param name="index"></param>
        public delegate void ClickKBarEventHandler(MouseEventArgs e, string stockcode, int index);
        /// <summary>
        /// ClickKBar事件委托的实例，用来触发事件
        /// </summary>
        private ClickKBarEventHandler OnClickKBarEvent;

        /// <summary>
        /// ClickKBar事件访问器，订阅事件时将调用方问器
        /// </summary>
        public event ClickKBarEventHandler ClickKBar
        {
            add { OnClickKBarEvent += new ClickKBarEventHandler(value); }
            remove { OnClickKBarEvent -= new ClickKBarEventHandler(value); }
        }
        /// <summary>
        /// ClickKBar事件的委托
        /// </summary>
        /// <param name="index"></param>
        public delegate void DoubleClickKBarEventHandler(EventArgs e, string stockcode, int index);
        /// <summary>
        /// ClickKBar事件委托的实例，用来触发事件
        /// </summary>
        private DoubleClickKBarEventHandler OnDoubleClickKBarEvent;

        /// <summary>
        /// ClickKBar事件访问器，订阅事件时将调用方问器
        /// </summary>
        public event DoubleClickKBarEventHandler DoubleClickKBar
        {
            add { OnDoubleClickKBarEvent += new DoubleClickKBarEventHandler(value); }
            remove { OnDoubleClickKBarEvent -= new DoubleClickKBarEventHandler(value); }
        }

        /// <summary>
        /// 绘制所需Graphics
        /// </summary>
        private Graphics _g;
        /// <summary>
        /// 绘制所需窗体句柄
        /// </summary>
        private IntPtr _hdc = IntPtr.Zero;

        private long _tmstamp = 0;
        /// <summary>
        /// 绘制所有内容的缓存画板
        /// </summary>
        private AreaForDraw _afd;
        /// <summary>
        /// 存储绘图用股票数据
        /// </summary>
        private DataKeeper _dkpr;

        private string _stockcode;
        /// <summary>
        /// 显示鼠标跟随线所在K线柱的股票信息
        /// </summary>
        private labelStockInfo _lsi;
        private bool _lsimousedown;

        private Point _lsilastpos;
        /// <summary>
        /// 记录鼠标点击的点
        /// </summary>
        private Point _clickp = new Point(-1, -1);

        private System.ComponentModel.Container components = null;

        private bool _displabelStockInfo = true;

        private long _mousedown = -1;

        public PanelDraw(ref DataKeeper datakeeper, int width, int height)
        {
            _dkpr = datakeeper;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Size = new System.Drawing.Size(width, height);
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            //this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ClickKBar += LabelDraw_ClickKBar;
            this.DoubleClickKBar += LabelDraw_DoubleClickKBar;


            _g = this.CreateGraphics();
            _hdc = _g.GetHdc();
            _afd = new AreaForDraw(this.Width, this.Height, _hdc);

            _lsi = new labelStockInfo(130, 280);
            this.Controls.Add(_lsi);

        }
        public void init()
        {
            _afd.ResetGraphics();
            _afd.drawFrame();
            _afd.drawKRules();
            _afd.drawVRules();
            _afd.drawKRuleNum();
            _afd.drawVRuleNum();
            _afd.Apply();

            _lsi.Top = _afd.KLineArea.Top + 10;
            _lsi.Left = _afd.KLineArea.Left + 10;
            _lsi.MouseDown += _lsi_MouseDown;
            _lsi.MouseUp += _lsi_MouseUp;
            _lsi.MouseMove += _lsi_MouseMove;
            _lsi.Paint += _lsi_Paint;
        }
        /// <summary>
        /// 设置是否显示顶部当前股票信息区域
        /// 默认显示
        /// </summary>
        public bool displayDrawCurrentStockInfo
        {
            get
            {
                return _afd.displayDrawCurrentStockInfo;
            }
            set
            {
                _afd.displayDrawCurrentStockInfo = value;
            }
        }
        /// <summary>
        /// 设置是否显示鼠标跟随线所在K线柱的股票信息Label
        /// 默认显示
        /// </summary>
        public bool displayLabelStockInfo
        {
            get
            {
                return _displabelStockInfo;
            }
            set
            {
                _displabelStockInfo = value;
                _lsi.Visible = value;
            }
        }
        private void _lsi_Paint(object sender, PaintEventArgs e)
        {
          
        }

        private void _lsi_MouseMove(object sender, MouseEventArgs e)
        {
            if (_lsimousedown)
            {
                Point p = new Point(_lsi.Location.X + e.X - _lsilastpos.X, _lsi.Location.Y + e.Y - _lsilastpos.Y);
                //if(p.X<=_afd.KLineArea.Left || p.Y<=_afd.KLineArea.Top || p.X+_lsi.Width>=_afd.KLineArea.Right || p.Y+_lsi.Height >=_afd.KLineArea.Bottom)
                //{
                //    return;
                //}
                if (p.X <= _afd.KLineArea.Left)
                {
                    p.X = _afd.KLineArea.Left;
                }
                if(p.Y <= _afd.KLineArea.Top)
                {
                    p.Y = _afd.KLineArea.Top;
                }
                if(p.X + _lsi.Width >= _afd.KLineArea.Right)
                {
                    p.X = _afd.KLineArea.Right - _lsi.Width;
                }
                if(p.Y + _lsi.Height >= _afd.KLineArea.Bottom)
                {
                    p.Y = _afd.KLineArea.Bottom - _lsi.Height;
                }
                _lsi.Location = p;
               // _lsi.Paint();
                return;
            }
        }

        private void _lsi_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _lsi.BackColor = Color.White;
                _lsimousedown = false;
            }
        }

        private void _lsi_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _lsimousedown = true;
                _lsilastpos = e.Location;
            }
        }

        private void LabelDraw_DoubleClickKBar(EventArgs e, string stockcode, int index)
        {
            //throw new NotImplementedException();
        }

        private void LabelDraw_ClickKBar(MouseEventArgs e, string stockcode, int index)
        {
            //throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (_hdc != IntPtr.Zero) _g.ReleaseHdc(_hdc);
            _g.Dispose();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose(); 
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            
            _afd.Apply();
            base.OnPaint(e);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            _clickp = new Point(e.X, e.Y);
            //点击在k线绘制范围内
            if (e.X >= _afd.KLineArea.Left && e.X <= _afd.KLineArea.Right && e.Y >= _afd.KLineArea.Top && e.Y <= _afd.KLineArea.Bottom)
            {
                int i = _afd.positionToKIndex(_clickp);
                //鼠标点击落在k线柱上，引发ClickKBar事件
                if (i >= 0)
                {
                    OnClickKBar(e, _stockcode, i);
                }
            }
            base.OnMouseClick(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            //点击在k线绘制范围内
            if (_clickp.X >= _afd.KLineArea.Left && _clickp.X <= _afd.KLineArea.Right && _clickp.Y >= _afd.KLineArea.Top && _clickp.Y <= _afd.KLineArea.Bottom)
            {
                int i = _afd.positionToKIndex(new Point(_clickp.X, _clickp.Y));
                //鼠标点击落在k线柱上，引发ClickKBar事件
                if (i >= 0)
                {
                    OnDoubleClickKBar(e, _stockcode, i);
                }
            }
            base.OnDoubleClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _mousedown = DateTime.Now.Ticks;
            base.OnMouseDown(e);

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _mousedown = -1;
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {

            Point ptscurrent = new Point();

            ptscurrent.X = e.X;
            ptscurrent.Y = e.Y;

            if(_mousedown>0)
            {
                //拖动k线图像
                _afd.moveKLine(ptscurrent, _mousedown);
            }
            else
            {
                //绘制跟随线
                if (_afd.drawCrossLine(ptscurrent))
                {
                    if (_stockcode != null && _stockcode != "")
                    {
                        _afd.drawCurrentStockInfo(ptscurrent, _dkpr.getStockDataSet(_stockcode));
                        _lsi.showStockData(_dkpr.getStockDataSet(_stockcode), _afd.xToKIndex(ptscurrent));
                    }

                    _afd.Apply();
                }
            }

            base.OnMouseMove(e);
        }

        protected void OnClickKBar(MouseEventArgs e, string stockcode, int i)
        {
            OnClickKBarEvent(e, _stockcode, i);
        }
        protected void OnDoubleClickKBar(EventArgs e, string stockcode, int i)
        {
            OnDoubleClickKBarEvent(e, _stockcode, i);
        }
        /// <summary>
        /// 绘制指定股票及分析结果的图形
        /// </summary>
        /// <param name="i">参照日的k线数据索引</param>
        /// <param name="sds">股票数据集合</param>
        /// <param name="dlar">分析结果数据集合</param>
        /// <param name="types">需绘制的分析结果类型</param>
        public void drawDataline(int i, StockDataSet sds, Dictionary<string, THashTable<AnalyzeResult>> dlar, string[] types)
        {
            _stockcode = sds.StockCode;
            _afd.clearCrossLine();
            _afd.ResetGraphics();
            _afd.drawFrame();
            _afd.drawKRules();
            _afd.drawVRules();
            _afd.drawKRuleNum();
            _afd.drawVRuleNum();
            _afd.Compute(i, sds);
            _afd.drawAnalyzeResultDayBack(i, dlar, types);
            _afd.drawKLine();
            _afd.drawMALine();
            //_afd.drawKLine(i,sds);
            _afd.drawVolumeLine();
            _afd.Apply();
        }
    }
}
