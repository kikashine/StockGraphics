using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockToolKit.Common;
using System.Drawing;

namespace StockToolKit.Analyze
{
    public class DrawKLine : DrawBase
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

        ///// <summary>
        ///// 最右侧k线柱的右边在bitmap上x值
        ///// </summary>
        //private int _edgeR = 0;
        ///// <summary>
        ///// 最左侧k线柱的左边在bitmap上x值
        ///// </summary>
        //private int _edgeL = 0;

        ///// <summary>
        ///// 最右侧k线柱的索引值
        ///// </summary>
        //private int _edgeRi = 0;
        ///// <summary>
        ///// 最左侧k线柱的索引值
        ///// </summary>
        //private int _edgeLi = 0;

        private THashTable<KDataDrawInfo> _krects;
        //private KDayDataList _kdlist;
        public DrawKLine(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC) : base(x, y, Width, Height, hBitmap, hDC)
        {
            _basey = _y + _height / 2;
            _basex = _x + _width / 2;
        }

        ///// <summary>
        ///// 最左侧k线柱左竖边在bitmap上的x轴坐标
        ///// </summary>
        //public int EdgeL
        //{
        //    get {
        //        return _edgeL;

        //    }
        //}
        ///// <summary>
        ///// 最右侧k线柱右竖边在bitmap上的x轴坐标
        ///// </summary>
        //public int EdgeR
        //{
        //    get
        //    {
        //        return _edgeR;

        //    }
        //}
        ///// <summary>
        ///// 最左侧k线柱的索引值
        ///// </summary>
        //public int EdgeLi
        //{
        //    get
        //    {
        //        return _edgeLi;

        //    }
        //}
        ///// <summary>
        ///// 最右侧k线柱的索引值
        ///// </summary>
        //public int EdgeRi
        //{
        //    get
        //    {
        //        return _edgeRi;

        //    }
        //}
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

        //public void drawKLine(DataDrawInfoList<DataDrawInfo> ddil)
        //{
        //    for (int i = 0; i < ddil.Count; i++)
        //    {
        //        drawKBar(ddil[i].K);
        //    }
        //}
        /// <summary>
        /// 将K线绘制在绘图区域中
        /// </summary>
        /// <param name="sdi">k线的绘图坐标数据</param>
        public void drawKLine(StocksDrawInfo sdi)
        {
            for(int i=sdi.EdgeLi;i<=sdi.EdgeRi;i++)
            {
                drawKBar(sdi.DDInfo((object)i).K);
            }
        }
        /// <summary>
        /// 将一个k线柱绘制在绘图区域中
        /// </summary>
        /// <param name="k"></param>
        public void drawKBar(KDataDrawInfo k)
        {
            int khighy = (int)Math.Round(k.Top);
            int klowy = (int)Math.Round(k.Bottom);
            int kx = (int)Math.Round(k.Left);
            int khighesty = (int)Math.Round(k.Highest);
            int klowesty = (int)Math.Round(k.Lowest);
            //实际的上影线高于k线柱上沿，但取整后与k线柱上沿相等，需让上影线高于上沿1像素
            if(k.Highest<k.Top && khighesty == khighy)
            {
                khighesty = khighy - 1;
            }
            //下影线同理
            if (k.Lowest > k.Bottom && klowesty == klowy)
            {
                klowesty = klowy + 1;
            }

            IntPtr oldbrush = IntPtr.Zero;
            IntPtr brush = IntPtr.Zero;
            Color brushcolor = Color.FromArgb(0x00cc99ff);
            Color penColor = Color.FromArgb(0x00cc99ff);

            if (k.Trend == Trend.Fall)
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


            //开始绘制
            if (k.Trend == Trend.Fall)
            {
                //绘制主体矩形
                //主体向上方完全超出显示范围
                if (klowy < _y && khighy < _y)
                {

                }
                //主体上沿向上方超出显示范围
                else if (khighy < _y && klowy >= _y)
                {
                    MoveToEx(_hdc, kx + _barwidth - 1, _y, IntPtr.Zero);
                    if (klowy>_bottom)
                    {
                        LineTo(_hdc, kx + _barwidth - 1, _bottom + 1);
                    }
                    else
                    {
                        LineTo(_hdc, kx + _barwidth - 1, klowy + 1);
                    }
                    MoveToEx(_hdc, kx, _y, IntPtr.Zero);
                    LineTo(_hdc, kx, klowy + 1);
                    MoveToEx(_hdc, kx, klowy, IntPtr.Zero);
                    LineTo(_hdc, kx + _barwidth - 1 + 1, klowy);
                }

                //主体整体向下方超出显示范围
                else if (klowy > _bottom && khighy > _bottom)
                {

                }
                //主体下沿向下方超出显示范围
                else if (klowy > _bottom && khighy <= _bottom)
                {
                    MoveToEx(_hdc, kx + _barwidth - 1, _bottom, IntPtr.Zero);
                    if (khighy < _y)
                    {
                        LineTo(_hdc, kx + _barwidth - 1, _y - 1);
                    }
                    else
                    {
                        LineTo(_hdc, kx + _barwidth - 1, khighy - 1);
                    }

                    MoveToEx(_hdc, kx, _bottom, IntPtr.Zero);
                    LineTo(_hdc, kx, khighy - 1);

                    MoveToEx(_hdc, kx, khighy, IntPtr.Zero);
                    LineTo(_hdc, kx + _barwidth - 1 + 1, khighy);
                }
                //主体在显示范围内
                else
                {
                    //左边，下向上
                    MoveToEx(_hdc, kx, klowy, IntPtr.Zero);
                    LineTo(_hdc, kx, khighy - 1);
                    //右边，下向上
                    MoveToEx(_hdc, kx + _barwidth - 1, klowy, IntPtr.Zero);
                    LineTo(_hdc, kx + _barwidth - 1, khighy - 1);
                    //上边，左向右
                    MoveToEx(_hdc, kx, khighy, IntPtr.Zero);
                    LineTo(_hdc, kx + _barwidth - 1 + 1, khighy);
                    //下边，左向右
                    MoveToEx(_hdc, kx, klowy, IntPtr.Zero);
                    LineTo(_hdc, kx + _barwidth - 1 + 1, klowy);
                }
            }
            else
            {
                if (klowy < _y && khighy < _y)
                {

                }
                else if (khighy < _y && klowy >= _y)
                {
                    if (klowy > _bottom)
                    {
                        Rectangle(_hdc, kx, _y, kx + _barwidth, _bottom + 1);
                    }
                    else
                    {
                        Rectangle(_hdc, kx, _y, kx + _barwidth, klowy + 1);
                    }
                }
                else if (klowy > _bottom && khighy > _bottom)
                {

                }
                else if (klowy > _bottom && khighy <= _bottom)
                {
                    if (khighy < _y)
                    {
                        Rectangle(_hdc, kx, _y, kx + _barwidth, _bottom + 1);
                    }
                    else
                    {
                        Rectangle(_hdc, kx, khighy, kx + _barwidth, _bottom + 1);
                    }
                }
                else
                {
                    Rectangle(_hdc, kx, khighy, kx + _barwidth, klowy + 1);
                }

            }
            //绘制上下影线
            //if (open > close)
            //{
            //上影线和上沿向上超出显示范围
            if (khighy <= _y && khighesty <= _y)
            {

            }
            //上沿在显示范围内，上影线高点向上超出显示范围
            else if (khighy > _y && khighesty < _y)
            {
                MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), khighy, IntPtr.Zero);
                LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), _y - 1);
            }
            //上沿在显示范围顶部以下，上影线在显示范围顶部以下
            else if (khighy > _y && khighesty >= _y)
            {
                //上沿向下超出显示范围，上影线向下超出显示范围
                if (khighy >= _bottom && khighesty >= _bottom)
                {

                }
                //上沿向下超出显示范围，上影线高点在显示范围以内
                else if (khighy >= _bottom && khighesty < _bottom)
                {
                    MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), _bottom, IntPtr.Zero);
                    LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), khighesty - 1);
                }
                //上沿在显示范围内，上影线在显示范围以内
                else if (khighy <= _bottom && khighesty < _bottom)
                {
                    MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), khighy, IntPtr.Zero);
                    LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), khighesty - 1);
                }

            }

            //下影线和下沿向下超出显示范围
            if (klowy >= _bottom && klowesty >= _bottom)
            {

            }
            //下沿在显示范围以内，下影线低点向下超出显示范围
            else if (klowy < _bottom && klowesty > _bottom)
            {
                MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), klowy, IntPtr.Zero);
                LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), _bottom + 1);
            }
            //下沿在显示范围底部以上，下影线在显示范围底部以上
            else if (klowy < _bottom && klowesty <= _bottom)
            {
                //下影线和下沿向上超出显示范围
                if (klowy <= _y && klowesty <= _y)
                {

                }
                //下沿向上超出显示范围，//下影线低值在显示范围内
                else if (klowy <= _y && klowesty > _y)
                {
                    MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), _y, IntPtr.Zero);
                    LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), klowesty + 1);
                }
                //下沿、上影线在显示范围以内
                else if (klowy >= _y && klowesty > _y)
                {
                    MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), klowy, IntPtr.Zero);
                    LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), klowesty + 1);
                }
            }

            if (oldpen != IntPtr.Zero && pen != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));
            if (oldbrush != IntPtr.Zero && brush != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldbrush));
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="i"></param>
        ///// <param name="sds"></param>
        //public void drawKLine(int i, StockDataSet sds)
        //{
        //    _krects = new THashTable<KDataDrawInfo>();
        //    KDataDrawInfo r;
        //    /*
        //     * 描绘k线柱原理
        //     * 价格升降百分比坐标系。像素数/百分比是固定的。即不同的股票不同的价格，升降相同的价格百分比，体现在y轴上的高度是一样的
        //     * i日是基准参照日，在x轴正中央描绘。open(i)的y轴取值作为初始参照值用来推演计算其它值的y轴位置。
        //     * i右侧，每日open、上下影线以前一日close的y值作为参照，当日close以当日open的y值作为参照，依据升降比例计算y值。
        //     * i左侧，需要逆推y值，当日close由右侧日open及open的y值推出，当日open由当日close及close的y值推出。
        //     */
        //    //初始化参考坐标，此值与open(i)对应
        //    _basey = _y + _height / 2;
        //    //绘制i所在k线柱
        //    r = drawKBar(i, i, sds.Open(i), sds.Open(i), sds.Close(i), sds.Highest(i), sds.Lowest(i));
        //    _krects.Add(i, r);
        //    //记录i所在k线柱的_basey(close价格的y轴坐标值)，作为接下来向右侧绘制k线柱的初始化参考坐标值，其值与close(i)对应。
        //    int baseyi = _basey;
        //    //向左侧计算时，设定open(i)与_y + _height / 2对应，再加上close(j)，逆推出close(j)对应的y值，并以此计算open(j)、close(j-1)......
        //    _basey = _y + _height / 2;
        //    //向左侧绘制
        //    for (int j = i - 1; j >= 0; j--)
        //    {
        //        //超出显示范围
        //        if ((j - i) * (_barwidth + _divide) + _basex <= _x)
        //        {
        //            _edgeL = (j - i + 1) * (_barwidth + _divide) + _basex;
        //            _edgeLi = j + 1;
        //            break;
        //        }
        //        //以右侧open作为k线柱参考
        //        r = drawKBar(i, j, sds.Open(j + 1), sds.Open(j), sds.Close(j), sds.Highest(j), sds.Lowest(j));
        //        _krects.Add(j, r);

        //    }
        //    //恢复close(i)的y值，以此作为向右计算的参考
        //    _basey = baseyi;
        //    for (int j = i + 1; j < sds.Length - 1; j++)
        //    {
        //        if ((j - i) * (_barwidth + _divide) + _basex >= _width - _barwidth)
        //        {
        //            _edgeR = (j - i) * (_barwidth + _divide) + _basex - _divide;
        //            _edgeRi = j - 1;
        //            break;
        //        }
        //        r = drawKBar(i, j, sds.Close(j - 1), sds.Open(j), sds.Close(j), sds.Highest(j), sds.Lowest(j));
        //        _krects.Add(j, r);
        //    }
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="basei">基准参照日索引</param>
        ///// <param name="currentj">需绘制日索引</param>
        ///// <param name="basePrice">参照价格</param>
        ///// <param name="open"></param>
        ///// <param name="close"></param>
        ///// <param name="highest"></param>
        ///// <param name="lowest"></param>
        ///// <param name="basePriceb">左侧日close价格。绘制i左侧k线时做参考值计算当日上下影线</param>
        //private KDataDrawInfo drawKBar(int basei, int currentj, float basePrice, float open, float close, float highest, float lowest)
        //{
        //    int basey = _basey;

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

        //    //open、close中的低值
        //    float low;
        //    //open、close中的高值
        //    float high;
        //    //低值对应的y值
        //    int klowy;
        //    //高值对应的y值
        //    int khighy;
        //    //上影线对应的y值
        //    int khighesty;
        //    //下影线对应的y值
        //    int klowesty;
        //    //左侧日close对应的y值
        //    //int kbasePriceby = -1;
        //    //当日k线柱主体左上角x坐标
        //    int kx = (currentj - basei) * (_barwidth + _divide) + _basex;
        //    //下降
        //    if (open > close)
        //    {
        //        low = close;
        //        high = open;
        //        //绘制右侧
        //        if (basei <= currentj)
        //        {
        //            //此时_basey为左侧日close的y值，basePrice为左侧日close
        //            khighy = _basey - (int)(((open - basePrice) / basePrice) * 100 * _onePercentPixel);
        //            //close以open为基准
        //            klowy = khighy - (int)(((close - open) / open) * 100 * _onePercentPixel);
        //            khighesty = khighy - (int)(((highest - open) / open) * 100 * _onePercentPixel);
        //            klowesty = khighy - (int)(((lowest - open) / open) * 100 * _onePercentPixel);
        //            //close的y值作为右侧日的基准
        //            _basey = klowy;
        //        }
        //        //绘制左侧
        //        else
        //        {
        //            //此时_basey为右侧日open的y值，basePrice为右侧日open，逆推当日close的y值
        //            klowy = _basey + (int)(((basePrice - close) / close) * 100 * _onePercentPixel);
        //            khighy = klowy + (int)(((low - open) / open) * 100 * _onePercentPixel);
        //            //kbasePriceby = khighy + (int)(((open - basePriceb) / basePriceb) * 100 * _onePercentPX);
        //            khighesty = khighy - (int)(((highest - open) / open) * 100 * _onePercentPixel);
        //            klowesty = khighy - (int)(((lowest - open) / open) * 100 * _onePercentPixel);
        //            //open的y值作为左侧日的基准
        //            _basey = khighy;
        //        }


        //    }
        //    //上升
        //    else
        //    {
        //        low = open;
        //        high = close;
        //        //绘制右侧
        //        if (basei <= currentj)
        //        {
        //            klowy = _basey - (int)(((open - basePrice) / basePrice) * 100 * _onePercentPixel);
        //            khighy = klowy - (int)(((high - open) / open) * 100 * _onePercentPixel);
        //            khighesty = klowy - (int)(((highest - low) / low) * 100 * _onePercentPixel);
        //            klowesty = klowy - (int)(((lowest - low) / low) * 100 * _onePercentPixel);
        //            _basey = khighy;
        //        }
        //        //绘制左侧
        //        else
        //        {
        //            khighy = _basey + (int)(((basePrice - close) / close) * 100 * _onePercentPixel);
        //            klowy = khighy + (int)(((high - open) / open) * 100 * _onePercentPixel);
        //            //kbasePriceby = klowy + (int)(((open - basePriceb) / basePriceb) * 100 * _onePercentPX);
        //            khighesty = klowy - (int)(((highest - open) / open) * 100 * _onePercentPixel);
        //            klowesty = klowy - (int)(((lowest - open) / open) * 100 * _onePercentPixel);
        //            _basey = klowy;
        //        }



        //    }

        //    //价格升降比小到不够1个像素，补足为1个像素，以便在显示时体现
        //    if (klowy == khighy)
        //    {
        //        if (low < high)
        //        {
        //            klowy = khighy + 1;
        //        }
        //        else if (low > high)
        //        {
        //            klowy = khighy - 1;
        //        }
        //    }
        //    if (klowesty == klowy && lowest < low)
        //    {
        //        klowesty = klowy + 1;
        //    }
        //    if (khighesty == khighy && high < highest)
        //    {
        //        khighesty = khighy - 1;
        //    }

        //    int kbarl = 0;
        //    int kbarr = 0;
        //    int kbart = 0;
        //    int kbarb = 0;
        //    //开始绘制
        //    if (open > close)
        //    {
        //        //绘制主体矩形
        //        //主体向上方完全超出显示范围
        //        if (klowy < _y && khighy < _y)
        //        {

        //        }
        //        //主体上沿向上方超出显示范围
        //        else if (khighy < _y && klowy >= _y)
        //        {
        //            kbarl = kx;
        //            kbarr = kx + _barwidth - 1;
        //            kbart = _y;
        //            kbarb = klowy;
        //            MoveToEx(_hdc, kx, _y, IntPtr.Zero);
        //            LineTo(_hdc, kx, klowy + 1);
        //            MoveToEx(_hdc, kx + _barwidth - 1, _y, IntPtr.Zero);
        //            LineTo(_hdc, kx + _barwidth - 1, klowy + 1);
        //            MoveToEx(_hdc, kx, klowy, IntPtr.Zero);
        //            LineTo(_hdc, kx + _barwidth - 1 + 1, klowy);
        //        }

        //        //主体整体向下方超出显示范围
        //        else if (klowy > _bottom && khighy > _bottom)
        //        {

        //        }
        //        //主体下沿向下方超出显示范围
        //        else if (klowy > _bottom && khighy <= _bottom)
        //        {
        //            kbarl = kx;
        //            kbarr = kx + _barwidth - 1;
        //            kbart = khighy;
        //            kbarb = _bottom;
        //            MoveToEx(_hdc, kx, _bottom, IntPtr.Zero);
        //            LineTo(_hdc, kx, khighy - 1);
        //            MoveToEx(_hdc, kx + _barwidth - 1, _bottom, IntPtr.Zero);
        //            LineTo(_hdc, kx + _barwidth - 1, khighy - 1);
        //            MoveToEx(_hdc, kx, khighy, IntPtr.Zero);
        //            LineTo(_hdc, kx + _barwidth - 1 + 1, khighy);
        //        }
        //        //主体在显示范围内
        //        else
        //        {
        //            kbarl = kx;
        //            kbarr = kx + _barwidth - 1;
        //            kbart = khighy;
        //            kbarb = klowy;
        //            MoveToEx(_hdc, kx, klowy, IntPtr.Zero);
        //            LineTo(_hdc, kx, khighy - 1);
        //            MoveToEx(_hdc, kx + _barwidth - 1, klowy, IntPtr.Zero);
        //            LineTo(_hdc, kx + _barwidth - 1, khighy - 1);
        //            MoveToEx(_hdc, kx, khighy, IntPtr.Zero);
        //            LineTo(_hdc, kx + _barwidth - 1 + 1, khighy);
        //            MoveToEx(_hdc, kx, klowy, IntPtr.Zero);
        //            LineTo(_hdc, kx + _barwidth - 1 + 1, klowy);
        //        }
        //    }
        //    else
        //    {
        //        if (klowy < _y && khighy < _y)
        //        {

        //        }
        //        else if (khighy < _y && klowy >= _y)
        //        {
        //            kbarl = kx;
        //            kbarr = kx + _barwidth - 1;
        //            kbart = _y;
        //            kbarb = klowy;
        //            Rectangle(_hdc, kx, _y, kx + _barwidth, klowy + 1);
        //        }
        //        else if (klowy > _bottom && khighy > _bottom)
        //        {

        //        }
        //        else if (klowy > _bottom && khighy <= _bottom)
        //        {
        //            kbarl = kx;
        //            kbarr = kx + _barwidth - 1;
        //            kbart = khighy;
        //            kbarb = _bottom;
        //            Rectangle(_hdc, kx, khighy, kx + _barwidth, _bottom + 1);
        //        }
        //        else
        //        {
        //            kbarl = kx;
        //            kbarr = kx + _barwidth - 1;
        //            kbart = khighy;
        //            kbarb = klowy;
        //            Rectangle(_hdc, kx, khighy, kx + _barwidth, klowy + 1);
        //        }

        //    }
        //    //绘制上下影线
        //    //if (open > close)
        //    //{
        //    //上影线和上沿向上超出显示范围
        //    if (khighy <= _y && khighesty <= _y)
        //    {

        //    }
        //    //上沿在显示范围内，上影线高点向上超出显示范围
        //    else if (khighy > _y && khighesty < _y)
        //    {
        //        MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), khighy, IntPtr.Zero);
        //        LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), _y - 1);
        //    }
        //    //上沿在显示范围顶部以下，上影线在显示范围顶部以下
        //    else if (khighy > _y && khighesty >= _y)
        //    {
        //        //上沿向下超出显示范围，上影线向下超出显示范围
        //        if (khighy >= _bottom && khighesty >= _bottom)
        //        {

        //        }
        //        //上沿向下超出显示范围，上影线高点在显示范围以内
        //        else if (khighy >= _bottom && khighesty < _bottom)
        //        {
        //            MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), _bottom, IntPtr.Zero);
        //            LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), khighesty - 1);
        //        }
        //        //上沿在显示范围内，上影线在显示范围以内
        //        else if (khighy <= _bottom && khighesty < _bottom)
        //        {
        //            MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), khighy, IntPtr.Zero);
        //            LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), khighesty - 1);
        //        }

        //    }

        //    //下影线和下沿向下超出显示范围
        //    if (klowy >= _bottom && klowesty >= _bottom)
        //    {

        //    }
        //    //下沿在显示范围以内，下影线低点向下超出显示范围
        //    else if (klowy < _bottom && klowesty > _bottom)
        //    {
        //        MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), klowy, IntPtr.Zero);
        //        LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), _bottom + 1);
        //    }
        //    //下沿在显示范围底部以上，下影线在显示范围底部以上
        //    else if (klowy < _bottom && klowesty <= _bottom)
        //    {
        //        //下影线和下沿向上超出显示范围
        //        if (klowy <= _y && klowesty <= _y)
        //        {

        //        }
        //        //下沿向上超出显示范围，//下影线低值在显示范围内
        //        else if (klowy <= _y && klowesty > _y)
        //        {
        //            MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), _y, IntPtr.Zero);
        //            LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), klowesty + 1);
        //        }
        //        //下沿、上影线在显示范围以内
        //        else if (klowy >= _y && klowesty > _y)
        //        {
        //            MoveToEx(_hdc, kx + (int)((_barwidth - 1) / 2), klowy, IntPtr.Zero);
        //            LineTo(_hdc, kx + (int)((_barwidth - 1) / 2), klowesty + 1);
        //        }
        //    }

        //    if (oldpen != IntPtr.Zero && pen != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldpen));
        //    if (oldbrush != IntPtr.Zero && brush != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldbrush));

        //    KDataDrawInfo r = new KDataDrawInfo();
        //    r.Left = kbarl;
        //    r.Right = kbarr;
        //    r.Top = kbart;
        //    r.Bottom = kbarb;
        //    r.BaseY = basey;
        //    r.Basei = basei;
        //    r.BasePrice = basePrice;
        //    r.Currenti = currentj;
        //    return r;
        //}

        ///// <summary>
        ///// 获取指定point所在K线柱的索引
        ///// 判定范围在k线柱的上下沿各扩展1px
        ///// </summary>
        ///// <param name="p"></param>
        ///// <returns>k线柱索引。若point未落在k线柱上，返回-1</returns>
        //public int positionToKIndex(Point p)
        //{
        //    int ci = -1;
        //    if (p.X >= _edgeL && p.X <= _edgeR && _edgeR > 0)
        //    {
        //        //计算当前鼠标x位置在第几个k线柱范围内。因整数除法无法整除时向下取整，所以+1
        //        ci = (p.X - _edgeL) / (_barwidth + _divide);
        //        if (p.X > ci * (_barwidth + _divide))
        //        {
        //            ci++;
        //        }
        //        //鼠标在最左侧k线柱范围内
        //        if (p.X <= _edgeL + _barwidth)
        //        {
        //            ci = _edgeLi;
        //        }
        //        //鼠标在最右侧k线柱范围内
        //        else if (p.X >= _edgeR - _barwidth)
        //        {
        //            ci = _edgeRi;
        //            //p.X = edgeR - (int)Math.Round((float)barwidth / 2);
        //        }
        //                //判断鼠标挪动到左侧k线柱边缘时，才将跟随线挪到左侧k线柱中间
        //                else if (p.X < _edgeL + (ci) * (_barwidth + _divide) - _divide
        //                    &&
        //                    p.X > _edgeL + (ci-1) * (_barwidth + _divide)
        //                    )
        //                {
        //                    ci = _edgeLi + ci - 1;
        //                    //p.X = edgeL + (ci) * (barwidth + divide) - divide - (int)Math.Round((float)barwidth / 2);
        //                }
        //                else
        //                {
        //                    ci = -1;
        //                    //p.X = _oldp.X;
        //                }
        //    }
        //    if(ci>=0)
        //    {
        //        if (p.Y > _krects[(object)ci].K.Top - 1 && p.Y < _krects[(object)ci].K.Bottom + 1)
        //        {

        //        }
        //        else
        //        {
        //            ci = -1;
        //        }
        //    }
        //    return ci;
        //}
        public void Dispose()
        {
        }

    }
}
