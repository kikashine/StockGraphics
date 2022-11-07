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
    /// 计算工具，用来计算stock数据对应的绘制信息
    /// 废弃
    /// </summary>
    public class ComputeDrawInfo:DrawBase
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
        /// k线y轴方向价格相对增减1%对应的像素数量
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

        private new static int _bottom = 0;

        private long _tmstamp = 0;

        private Point _oldp = new Point(0, 0);
        /// <summary>
        /// 计算一个stock各日各项数据的绘制信息
        /// </summary>
        /// <param name="i">绘制的参照日，其open作为初始中间位置</param>
        /// <param name="sds">股票数据集合</param>
        /// <param name="dkl">k线绘制区域，其属性用来计算绘制信息</param>
        /// <param name="dvl">交易量绘制区域，其属性用来计算绘制信息</param>
        /// <returns></returns>
        public DataDrawInfoList<DataDrawInfo> Compute(int i,StockDataSet sds, DrawKLine dkl, DrawVolumesLine dvl)
        {
            _y = dkl.Top;
            _x = dkl.Left;
            
            _barwidth = dkl.BarWidth;
            _divide = dkl.Divide;
            _oneKPercentPixel = dkl.OnePercentPixels;
            _oneVPercentPixel = dvl.OnePercentPixels;
            _basey = dkl.Top + dkl.Height / 2;
            _basex = dkl.Left + dkl.Width / 2;
            if(_basex - i * (dkl.BarWidth + dkl.Divide)-_x> dkl.BarWidth + dkl.Divide)
            {
                _basex = _basex - (dkl.BarWidth + dkl.Divide)*((_basex - _x)/(dkl.BarWidth + dkl.Divide)) + i * (dkl.BarWidth + dkl.Divide);
            }
            DataDrawInfoList<DataDrawInfo> _krects = new DataDrawInfoList<DataDrawInfo>();
            DataDrawInfo r = new DataDrawInfo();

            _krects.BarWidth = dkl.BarWidth;
            _krects.Divide = dkl.Divide;
            _krects.OneKPercentPixels = dkl.OnePercentPixels;
            _krects.OneVPercentPixels = dvl.OnePercentPixels;
            /*
             * 描绘k线柱原理
             * 价格升降百分比坐标系。像素数/百分比是固定的。即不同的股票不同的价格，升降相同的价格百分比，体现在y轴上的高度是一样的
             * i日是基准参照日，在x轴正中央描绘。open(i)的y轴取值作为初始参照值用来推演计算其它值的y轴位置。
             * i右侧，每日open、上下影线以前一日close的y值作为参照，当日close以当日open的y值作为参照，依据升降比例计算y值。
             * i左侧，需要逆推y值，当日close由右侧日open及open的y值推出，当日open由当日close及close的y值推出。
             */
            //初始化参考坐标，此值与open(i)对应
            _basey = dkl.Top + dkl.Height / 2;
            //绘制i所在k线柱
            //k线绘制区域的bottom与交易量绘制区域的bottom不同，_bottom是类的属性，在各计算方法中被使用，所以在调用计算不同区域的计算方法前需赋值
            _bottom = dkl.Bottom;
            r.K = computeKBar(i, i, sds.Open(i), sds.Open(i), sds.Close(i), sds.Highest(i), sds.Lowest(i));
            _bottom = dvl.Bottom;
            r.V = drawVolumeBar(i, i, sds.Volume(i), sds.Volume(sds.MaxVolumeIndex), sds.Open(i), sds.Close(i));
            _krects.Add(i, r);
            //记录i所在k线柱的_basey(close价格的y轴坐标值)，作为接下来向右侧绘制k线柱的初始化参考坐标值，其值与close(i)对应。
            int baseyi = _basey;
            //向左侧计算时，设定open(i)与_y + _height / 2对应，再加上close(j)，逆推出close(j)对应的y值，并以此计算open(j)、close(j-1)......
            _basey = dkl.Top + dkl.Height / 2;
            //向左侧绘制
            for (int j = i - 1; j >= 0; j--)
            {
                r = new DataDrawInfo();
                //超出显示范围
                if ((j - i) * (_barwidth + _divide) + _basex <= _x)
                {
                    _krects.EdgeL = (j - i + 1) * (_barwidth + _divide) + _basex;
                    _krects.EdgeLi = j + 1;
                    break;
                }
                if(j==0)
                {
                    _krects.EdgeL = (j - i) * (_barwidth + _divide) + _basex;
                    _krects.EdgeLi = j;
                }
                _bottom = dkl.Bottom;
                //以右侧open作为k线柱参考
                r.K = computeKBar(i, j, sds.Open(j + 1), sds.Open(j), sds.Close(j), sds.Highest(j), sds.Lowest(j));
                _bottom = dvl.Bottom;
                r.V = drawVolumeBar(i, j, sds.Volume(j), sds.Volume(sds.MaxVolumeIndex), sds.Open(j), sds.Close(j));
                _krects.Add(j, r);

            }
            //恢复close(i)的y值，以此作为向右计算的参考
            _basey = baseyi;
            for (int j = i + 1; j <= sds.Length - 1; j++)
            {
                r = new DataDrawInfo();
                if ((j - i) * (_barwidth + _divide) + _basex >= dkl.Width - _barwidth)
                {
                    _krects.EdgeR = (j - i) * (_barwidth + _divide) + _basex - _divide;
                    _krects.EdgeRi = j - 1;
                    break;
                }
                if(j== sds.Length - 1)
                {
                    _krects.EdgeR = (j - i) * (_barwidth + _divide) + _basex - _divide;
                    _krects.EdgeRi = j;
                }
                _bottom = dkl.Bottom;
                r.K = computeKBar(i, j, sds.Close(j - 1), sds.Open(j), sds.Close(j), sds.Highest(j), sds.Lowest(j));
                _bottom = dvl.Bottom;
                r.V = drawVolumeBar(i, j, sds.Volume(j), sds.Volume(sds.MaxVolumeIndex), sds.Open(j), sds.Close(j));
                _krects.Add(j, r);
            }

            return _krects;
        }

        /// <summary>
        /// 计算一个k线柱的绘制信息
        /// </summary>
        /// <param name="basei">参照日索引</param>
        /// <param name="currentj">当前日索引</param>
        /// <param name="basePrice">参照价格</param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <param name="highest"></param>
        /// <param name="lowest"></param>
        /// <returns></returns>
        private KDataDrawInfo computeKBar(int basei, int currentj, float basePrice, float open, float close, float highest, float lowest)
        {

            int basey = _basey;
            //open、close中的低值
            float low;
            //open、close中的高值
            float high;
            //低值对应的y值
            int klowy;
            //高值对应的y值
            int khighy;
            //上影线对应的y值
            int khighesty;
            //下影线对应的y值
            int klowesty;
            //左侧日close对应的y值
            //int kbasePriceby = -1;
            //当日k线柱主体左上角x坐标
            int kx = (currentj - basei) * (_barwidth + _divide) + _basex;
            //下降
            if (open > close)
            {
                low = close;
                high = open;
                //绘制右侧
                if (basei <= currentj)
                {
                    //此时_basey为左侧日close的y值，basePrice为左侧日close
                    khighy = _basey - (int)(((open - basePrice) / Math.Abs(basePrice)) * 100 * _oneKPercentPixel);
                    //close以open为基准
                    klowy = khighy - (int)(((close - open) / Math.Abs(open)) * 100 * _oneKPercentPixel);
                    khighesty = khighy - (int)(((highest - open) / Math.Abs(open)) * 100 * _oneKPercentPixel);
                    klowesty = khighy - (int)(((lowest - open) / Math.Abs(open)) * 100 * _oneKPercentPixel);
                    //close的y值作为右侧日的基准
                    _basey = klowy;
                }
                //绘制左侧
                else
                {
                    //此时_basey为右侧日open的y值，basePrice为右侧日open，逆推当日close的y值
                    klowy = _basey + (int)(((basePrice - close) / Math.Abs(close)) * 100 * _oneKPercentPixel);
                    khighy = klowy + (int)(((low - open) / Math.Abs(open)) * 100 * _oneKPercentPixel);
                    //kbasePriceby = khighy + (int)(((open - basePriceb) / basePriceb) * 100 * _onePercentPX);
                    khighesty = khighy - (int)(((highest - open) / Math.Abs(open)) * 100 * _oneKPercentPixel);
                    klowesty = khighy - (int)(((lowest - open) / Math.Abs(open)) * 100 * _oneKPercentPixel);
                    //open的y值作为左侧日的基准
                    _basey = khighy;
                }


            }
            //上升
            else
            {
                low = open;
                high = close;
                //绘制右侧
                if (basei <= currentj)
                {
                    klowy = _basey - (int)(((open - basePrice) / Math.Abs(basePrice)) * 100 * _oneKPercentPixel);
                    khighy = klowy - (int)(((high - open) / Math.Abs(open)) * 100 * _oneKPercentPixel);
                    khighesty = klowy - (int)(((highest - low) / Math.Abs(low)) * 100 * _oneKPercentPixel);
                    klowesty = klowy - (int)(((lowest - low) / Math.Abs(low)) * 100 * _oneKPercentPixel);
                    _basey = khighy;
                }
                //绘制左侧
                else
                {
                    khighy = _basey + (int)(((basePrice - close) / Math.Abs(close)) * 100 * _oneKPercentPixel);
                    klowy = khighy + (int)(((high - open) / Math.Abs(open)) * 100 * _oneKPercentPixel);
                    //kbasePriceby = klowy + (int)(((open - basePriceb) / basePriceb) * 100 * _onePercentPX);
                    khighesty = klowy - (int)(((highest - open) / Math.Abs(open)) * 100 * _oneKPercentPixel);
                    klowesty = klowy - (int)(((lowest - open) / Math.Abs(open)) * 100 * _oneKPercentPixel);
                    _basey = klowy;
                }



            }

            //价格升降比小到不够1个像素，补足为1个像素，以便在显示时体现
            if (klowy == khighy)
            {
                if (low < high)
                {
                    klowy = khighy + 1;
                }
                else if (low > high)
                {
                    klowy = khighy - 1;
                }
            }
            if (klowesty == klowy && lowest < low)
            {
                klowesty = klowy + 1;
            }
            if (khighesty == khighy && high < highest)
            {
                khighesty = khighy - 1;
            }

            int kbarl = -1;
            int kbarr = -1;
            int kbart = -1;
            int kbarb = -1;
            int kbarhighest = -1;
            int kbarlowest = -1;
            Trend trend = Trend.Flat;
            //开始绘制
            if (open > close)
            {
                trend = Trend.Fall;
            }
            else
            {
                if (open < close)
                {
                    trend = Trend.Rise;
                }
            }
            //绘制主体矩形
            //主体向上方完全超出显示范围
            if (klowy < _y && khighy < _y)
            {

            }
            //主体上沿向上方超出显示范围
            else if (khighy < _y && klowy >= _y)
            {
                kbarl = kx;
                kbarr = kx + _barwidth - 1;
                //超出绘制区域上沿时，令k线柱上沿的y值为超出绘制区域上沿1像素，方便判断鼠标点击等区域，同时区分与绘制区域下沿正好相等的情况
                kbart = _y - 1;
                if (klowy > _bottom)
                {
                    //超出绘制区域下沿时，令k线柱下沿的y值为超出绘制区域下沿1像素，方便判断鼠标点击等区域，同时区分与绘制区域下沿正好相等的情况
                    kbarb = _bottom + 1;
                }
                else
                {
                    kbarb = klowy;
                }
            }

            //主体整体向下方超出显示范围
            else if (klowy > _bottom && khighy > _bottom)
            {

            }
            //主体下沿向下方超出显示范围
            else if (klowy > _bottom && khighy <= _bottom)
            {
                if (klowy < _y)
                {
                    kbart = _y;
                }
                else
                {
                    kbart = khighy;
                }

                kbarl = kx;
                kbarr = kx + _barwidth - 1;
                kbarb = _bottom + 1;
            }
            //主体在显示范围内
            else
            {
                kbarl = kx;
                kbarr = kx + _barwidth - 1;
                kbart = khighy;
                kbarb = klowy;
            }

            //绘制上下影线
            //if (open > close)
            //{
            //上影线和上沿向上超出显示范围
            if (khighy <= _y && khighesty <= _y)
            {
                kbarhighest = _y - 1;
            }
            //上沿在显示范围内，上影线高点向上超出显示范围
            else if (khighy > _y && khighesty < _y)
            {
                kbarhighest = _y - 1;
            }
            //上沿在显示范围顶部以下，上影线在显示范围顶部以下
            else if (khighy > _y && khighesty >= _y)
            {
                //上沿向下超出显示范围，上影线向下超出显示范围
                if (khighy >= _bottom && khighesty >= _bottom)
                {
                    kbarhighest = _bottom + 1;
                }
                //上沿向下超出显示范围，上影线高点在显示范围以内
                else if (khighy >= _bottom && khighesty < _bottom)
                {
                    kbarhighest = khighesty;
                }
                //上沿在显示范围内，上影线在显示范围以内
                else if (khighy <= _bottom && khighesty < _bottom)
                {
                    kbarhighest = khighesty;
                }

            }

            //下影线和下沿向下超出显示范围
            if (klowy >= _bottom && klowesty >= _bottom)
            {
                kbarlowest = _bottom + 1;
            }
            //下沿在显示范围以内，下影线低点向下超出显示范围
            else if (klowy < _bottom && klowesty > _bottom)
            {
                kbarlowest = _bottom + 1;
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
                    kbarlowest = klowesty;
                }
                //下沿、下影线在显示范围以内
                else if (klowy >= _y && klowesty > _y)
                {
                    kbarlowest = klowesty;
                }
            }


            KDataDrawInfo r = new KDataDrawInfo();
            r.Left = kbarl;
            r.Right = kbarr;
            r.Top = kbart;
            r.Bottom = kbarb;
            r.Highest = kbarhighest;
            r.Lowest = kbarlowest;
            r.Trend = trend;
            return r;
        }

        /// <summary>
        /// 鼠标拖动图像时重新计算绘图信息
        /// </summary>
        /// <param name="krects"></param>
        /// <param name="sds"></param>
        /// <param name="dkl"></param>
        /// <param name="dvl"></param>
        /// <returns></returns>
        public bool MovedReCompute(Point p, int i, ref DataDrawInfoList<DataDrawInfo> ddil, StockDataSet sds, DrawKLine dkl, DrawVolumesLine dvl)
        {
            //鼠标位移
            int mx = 0;
            //鼠标位移
            int my = 0;
            //移动后k线柱上沿y轴坐标
            int newtop = -1;
            //移动后k线柱下沿y轴坐标
            int newbottom = -1;
            if (DateTime.Now.Ticks - _tmstamp < 100000)
            {
                return false;
            }
            //鼠标移动到可绘制区域之外
            if (p.X < _x || p.Y < _y || p.X > _right || p.Y > _bottom)
            {
                return false;
            }
            if(ddil == null || ddil.Count == 0)
            {
                return false;
            }
            if (ddil != null && i >= 0)
            {
                //mx = ddil[(object)i].K.Left;

                //p.X = ddil[(object)i].K.Left + ddil.BarWidth / 2;
            }
            else if (ddil != null)
            {
                mx = _oldp.X;
                //p.X = _oldp.X;
                //p = _oldp;
            }
            my = p.Y;
            if(mx==_oldp.X && my==_oldp.Y)
            {
                return false;
            }
            foreach(object key in ddil.Keys)
            {
                //当前柱移动后水平位置超出左侧显示范围
                if (ddil[key].K.Left + mx < dkl.Left)
                {
                    ddil.Remove(key);
                }
                //当前柱移动后水平位置超出右侧显示范围
                else if (ddil[key].K.Right + mx > dkl.Right)
                {
                    ddil.Remove(key);
                }
                //当前柱移动后上沿位置超出上方显示范围，且移动前未超出显示范围，需重新计算上沿
                if (ddil[key].K.Top + my < dkl.Top && ddil[key].K.Top != -1)
                {
                    //当前柱移动后下沿位置超出下方显示范围，且移动前未超出显示范围，需重新计算下沿
                    if (ddil[key].K.Bottom + my < dkl.Top && ddil[key].K.Bottom != -1)
                    {
                        newtop = -1;
                        newbottom = -1;
                    }
                    else
                    {
                        newtop = dkl.Top;
                        //newbottom = ddil[key].K.Bottom + my;
                    }
                }

                //当前柱移动后下沿位置超出上方显示范围，且移动前未超出显示范围，需重新计算下沿
                if (ddil[key].K.Bottom + my > dkl.Bottom && ddil[key].K.Bottom != -1)
                {
                    //当前柱移动后上沿位置超出下方显示范围，且移动前未超出显示范围，需重新计算上沿
                    if (ddil[key].K.Top + my > dkl.Bottom && ddil[key].K.Top != -1)
                    {
                        newtop = -1;
                        newbottom = -1;
                    }
                    else
                    {

                    }
                }

                //上影线移动前超出显示范围，需重新计算上影线
                if (ddil[key].K.Highest == -1)
                {

                }
                //下影线移动前超出显示范围，需重新计算上影线
                if (ddil[key].K.Lowest == - 1)
                {

                }
                //移动前kbar处于显示区域外，需重新计算k线柱及影线
                if (ddil[key].K.Bottom == -1 && ddil[key].K.Top == -1)
                {
                    
                }

                //if(ddil[key].K.Lowest + my < dkl.Top)
                //{
                //    //ddil[key].K
                //}
                //else if(ddil[key].K.Highest + my > dkl.Bottom)
                //{

                //}
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i">参照日索引</param>
        /// <param name="j">当日索引</param>
        /// <param name="volume">当日交易量</param>
        /// <param name="maxvolume">参照交易量，计算时当作100%幅度</param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <returns></returns>
        private VDataDrawInfo drawVolumeBar(int i, int j, double volume, double maxvolume, float open, float close)
        {

            int kx = (j - i) * (_barwidth + _divide) + _basex;

            double top = maxvolume;

            //int ctopy = _bottom - (int)((double)top / (double)top * 100 * _onePercentPixel);


            int voly = _bottom - (int)(volume / top * 100 * _oneVPercentPixel) + 1;
            //交易量幅度小于1%时，占0像素，此时交易量柱上沿低于_bottom，因此需令上沿等于_bottom
            if (voly > _bottom)
            {
                voly = _bottom;
            }
            int kbarl = kx;
            int kbarr = kx + _barwidth - 1;
            int kbart = voly;
            int kbarb = _bottom;
            Trend trend = Trend.Flat;
            //开始绘制
            if (open > close)
            {
                trend = Trend.Fall;
            }
            else
            {
                if (open < close)
                {
                    trend = Trend.Rise;
                }
            }
            VDataDrawInfo r = new VDataDrawInfo();
            r.Left = kbarl;
            r.Right = kbarr;
            r.Top = kbart;
            r.Bottom = kbarb;
            r.Trend = trend;
            return r;


        }

        /// <summary>
        /// 获取指定point所在K线柱的索引
        /// 判定范围在k线柱的上下沿各扩展1px
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ddil">股票的绘制信息</param>
        /// <returns>k线柱索引。若point未落在k线柱上，返回-1</returns>
        public int positionToKIndex(Point p, DataDrawInfoList<DataDrawInfo> ddil)
        {
            int ci = -1;
            if (p.X >= ddil.EdgeL && p.X <= ddil.EdgeR && ddil.EdgeR > 0)
            {
                //计算当前鼠标x位置在第几个k线柱范围内。因整数除法无法整除时向下取整，所以+1
                ci = (p.X - ddil.EdgeL) / (ddil.BarWidth + ddil.Divide);
                if (p.X > ci * (ddil.BarWidth + ddil.Divide))
                {
                    ci++;
                }
                //鼠标在最左侧k线柱范围内
                if (p.X <= ddil.EdgeL + ddil.BarWidth)
                {
                    ci = ddil.EdgeLi;
                }
                //鼠标在最右侧k线柱范围内
                else if (p.X >= ddil.EdgeR - ddil.BarWidth)
                {
                    ci = ddil.EdgeRi;
                    //p.X = edgeR - (int)Math.Round((float)barwidth / 2);
                }
                //判断鼠标挪动到左侧k线柱边缘时，才将跟随线挪到左侧k线柱中间
                else if (p.X < ddil.EdgeL + (ci) * (ddil.BarWidth + ddil.Divide) - ddil.Divide
                    &&
                    p.X > ddil.EdgeL + (ci - 1) * (ddil.BarWidth + ddil.Divide)
                    )
                {
                    ci = ddil.EdgeLi + ci - 1;
                    //p.X = edgeL + (ci) * (barwidth + divide) - divide - (int)Math.Round((float)barwidth / 2);
                }
                else
                {
                    ci = -1;
                    //p.X = _oldp.X;
                }
            }
            if (ci >= 0)
            {
                if (p.Y > ddil[(object)ci].K.Top - 1 && p.Y < ddil[(object)ci].K.Bottom + 1)
                {

                }
                else
                {
                    ci = -1;
                }
            }
            return ci;
        }

        /// <summary>
        /// 获取指定point的x坐标所在K线绘图信息的索引
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ddil">股票的绘制信息</param>
        /// <returns>k线柱索引。若point的x轴未落在k线柱上，返回-1</returns>
        public int xToKIndex(Point p, DataDrawInfoList<DataDrawInfo> ddil)
        {
            int ci = -1;
            if (p.X >= ddil.EdgeL && p.X <= ddil.EdgeR && ddil.EdgeR > 0)
            {
                //计算当前鼠标x位置在第几个k线柱范围内。因整数除法无法整除时向下取整，所以+1
                ci =  (p.X - ddil.EdgeL) / (ddil.BarWidth + ddil.Divide);
                if (p.X >= ddil.EdgeL + ci * (ddil.BarWidth + ddil.Divide) - 1)
                {
                    ci++;
                }
                //鼠标在最左侧k线柱范围内
                if (p.X <= ddil.EdgeL + ddil.BarWidth)
                {
                    ci = ddil.EdgeLi;
                }
                //鼠标在最右侧k线柱范围内
                else if (p.X >= ddil.EdgeR - ddil.BarWidth)
                {
                    ci = ddil.EdgeRi;
                    //p.X = edgeR - (int)Math.Round((float)barwidth / 2);
                }
                //判断鼠标挪动到左侧k线柱边缘时，才将跟随线挪到左侧k线柱中间
                else if (p.X < ddil.EdgeL + (ci) * (ddil.BarWidth + ddil.Divide) - ddil.Divide
                    &&
                    p.X > ddil.EdgeL + (ci - 1) * (ddil.BarWidth + ddil.Divide)
                    )
                {
                    ci = ddil.EdgeLi + ci - 1;
                    //p.X = edgeL + (ci) * (barwidth + divide) - divide - (int)Math.Round((float)barwidth / 2);
                }
                else if (p.X > ddil.EdgeL + (ci - 1) * (ddil.BarWidth + ddil.Divide) - 1
                    &&
                    p.X < ddil.EdgeL + (ci) * (ddil.BarWidth + ddil.Divide)- ddil.Divide
                    )
                {
                    ci = ddil.EdgeLi + ci -1;
                    //p.X = edgeL + (ci) * (barwidth + divide) - divide - (int)Math.Round((float)barwidth / 2);
                }
                else
                {
                    ci = -1;
                    //p.X = _oldp.X;
                }
            }
            return ci;
        }
    }
}
