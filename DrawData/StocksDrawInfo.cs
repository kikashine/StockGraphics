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
    /// 根据股票价格、交易量、绘图区域信息等数据获得绘图坐标数据
    /// </summary>
    public class StocksDrawInfo : DrawBase
    {
        /// <summary>
        /// 根据参照日的open价格为基准，计算得出的全部原始绘图数据。此数据不包含具体需绘制的坐标数据
        /// 数据y轴方向仅使用相对基准的幅度百分比描述，x轴方向不包含bar宽度和间隔距离等信息
        /// </summary>
        private THashTable<DataDrawInfo> _originalddil;
        /// <summary>
        /// 结合画布坐标、偏移量、bar宽度、间隔距离、y轴百分比与像素对应关系、x轴向显示数量等计算的实际绘图数据，包含股票各数据的需绘制坐标。
        /// </summary>
        private THashTable<DataDrawInfo> _drawddil;
        /// <summary>
        /// 作为计算基准的Bar的k线数据索引
        /// </summary>
        private int _basei = -1;
        /// <summary>
        /// 股票数据集合
        /// </summary>
        private StockDataSet _sds;

        /// <summary>
        /// 两k线柱之间间隔像素
        /// </summary>
        private int _divide = 0;
        /// <summary>
        /// k线y轴方向价格相对增减1%对应的像素数量
        /// </summary>
        private float _oneKPercentPixel = 1;
        /// <summary>
        /// 交易量y轴方向价格相对增减1%对应的像素数量
        /// </summary>
        private float _oneVPercentPixel = 1;
        /// <summary>
        /// k线柱宽度
        /// </summary>
        private int _barwidth = 1;
        /// <summary>
        /// 
        /// </summary>
        private int _offsetx = 0;

        private int _offsety = 0;

        //private int _width;
        /// <summary>
        /// 显示区域左边缘在bitmap中的x坐标
        /// </summary>
        private int _left;
        /// <summary>
        /// k线显示区域高度
        /// </summary>
        private int _kheight;
        /// <summary>
        /// 交易量显示区域高度
        /// </summary>
        private int _vheight;

        //private int _right;
        /// <summary>
        /// k线显示区域上边缘在bitmap中的y坐标
        /// </summary>
        private int _ktop;
        /// <summary>
        /// k线显示区域下边缘在bitmap中的y坐标
        /// </summary>
        private int _kbottom;
        /// <summary>
        /// 交易量显示区域上边缘在bitmap中的y坐标
        /// </summary>
        private int _vtop;
        /// <summary>
        /// 交易量显示区域下边缘在bitmap中的y坐标
        /// </summary>
        private int _vbottom;
        /// <summary>
        /// basei对应Bar左竖边的x轴值
        /// </summary>
        private int _basex;
        /// <summary>
        /// basei对应Bar左上角的Y轴值
        /// </summary>
        private int _basey;

        /// <summary>
        /// 最右侧k线柱的右边在bitmap上x值
        /// </summary>
        private int _edgeR = 0;
        /// <summary>
        /// 最左侧k线柱的左边在bitmap上x值
        /// </summary>
        private int _edgeL = 0;

        /// <summary>
        /// 最右侧k线柱的索引值
        /// </summary>
        private int _edgeRi = -1;

        /// <summary>
        /// 最左侧k线柱的索引值
        /// </summary>
        private int _edgeLi = -1;
        /// <summary>
        /// 当前绘制图像的版本，重新设置股票数据参数、移动图像等会改变版本
        /// </summary>
        private string _version = "";
        /// <summary>
        /// 上一次有效绘制的时间戳
        /// </summary>
        private long _tmstamp = DateTime.Now.Ticks;
        /// <summary>
        /// 鼠标移动过程中，上一次被重新计算绘制信息的鼠标所处位置
        /// </summary>
        private Point _oldp = new Point(-1, -1);
        /// <summary>
        /// 一次按下鼠标键开始拖动的起始时间戳
        /// </summary>
        private long _movetimestamp = -1;
        /// <summary>
        /// 最左侧k线柱左竖边在bitmap上的x轴坐标
        /// </summary>
        public int EdgeL
        {
            get
            {
                return _edgeL;

            }
            set
            {
                _edgeL = value;
            }
        }
        /// <summary>
        /// 最右侧k线柱右竖边在bitmap上的x轴坐标
        /// </summary>
        public int EdgeR
        {
            get
            {
                return _edgeR;

            }
            set
            {
                _edgeR = value;
            }
        }
        /// <summary>
        /// 最左侧k线柱的索引值
        /// </summary>
        public int EdgeLi
        {
            get
            {
                return _edgeLi;

            }
            set
            {
                _edgeLi = value;
            }
        }
        /// <summary>
        /// 最右侧k线柱的索引值
        /// </summary>
        public int EdgeRi
        {
            get
            {
                return _edgeRi;

            }
            set
            {
                _edgeRi = value;
            }
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
        /// 百分之一的K升降幅在Y轴上对应的像素数
        /// </summary>
        public float OneKPercentPixels
        {
            get
            {
                return _oneKPercentPixel;
            }
            set
            {
                _oneKPercentPixel = value;
            }
        }
        /// <summary>
        /// 百分之一的交易量升降幅在Y轴上对应的像素数
        /// </summary>
        public float OneVPercentPixels
        {
            get
            {
                return _oneVPercentPixel;
            }
            set
            {
                _oneVPercentPixel = value;
            }
        }
        /// <summary>
        /// 作为计算基准的Bar的k线数据索引
        /// </summary>
        public int Basei
        {
            get
            {
                return _basei;
            }
        }
        public StocksDrawInfo(StockDataSet sds)
        {
            _originalddil = new THashTable<DataDrawInfo>();
            _drawddil = new THashTable<DataDrawInfo>();
            _sds = sds;
            //y轴绘图位置基准值
            float basey = 0;
            //以数据列表中最后一天的open作为初始基准值，然后逐天向左逆推
            float basep = _sds.Open(_sds.Length - 1);


            for (int i = _sds.Length - 1; i >= 0; i--)
            {
                DataDrawInfo r = new DataDrawInfo();
                KDataDrawInfo k = new KDataDrawInfo();
                //当日的open作为当日其他价格的基准值，左日的open以右日的close为基准值
                //相对于基准值的升降幅度比例最终会按百分比计算对应的升降幅度所占像素数(像素数每百分之一 x 幅度百分比)
                //某个价格的幅度 = 相对基准值的升降幅度 + 基准值的幅度，即此价格的幅度为相对最后（_sds.Length - 1）日open的幅度
                //因此，_sds.Length - 1日open对应的绘图位图y轴位置若已知，则某个价格的y轴值 = _sds.Length - 1日open的y轴值 + 幅度百分比 x 像素数每百分之一
                //同理，m日open对应的绘图位图y轴位置若已知，则某个价格的y轴值 = m日open的y轴值 + 幅度百分比之差 x 像素数每百分之一
                if (i == _sds.Length - 1)
                {
                    if (_sds.Close(i) >= _sds.Open(i))
                    {
                        k.Top = (_sds.Close(i) - basep) / basep;
                        //open的y值基准值设置为0
                        k.Bottom = 0;
                        k.Highest = (_sds.Highest(i) - basep) / basep;
                        k.Lowest = (_sds.Lowest(i) - basep) / basep;
                        k.Trend = Trend.Rise;
                        //上涨时open为底部，此时底部作为左日的y值基准值
                        basey = k.Bottom;

                    }
                    else
                    {
                        k.Bottom = (_sds.Close(i) - basep) / basep;
                        //open的y值基准值设置为0
                        k.Top = 0;
                        k.Highest = (_sds.Highest(i) - basep) / basep;
                        k.Lowest = (_sds.Lowest(i) - basep) / basep;
                        k.Trend = Trend.Fall;
                        //下降时open为顶部，此时顶部作为左日的y值基准值
                        basey = k.Top;


                    }

                }
                else
                {
                    if (_sds.Close(i) >= _sds.Open(i))
                    {   //由于是由右侧向左逆推，
                        //因此由 basey（即右侧日open的y值） = k.top（即当日close的y值） +  (basep（即右侧日open价格） - _sds.Close(i)) / _sds.Close(i)
                        //推出close的y值k.top
                        k.Top = basey - (basep - _sds.Close(i)) / _sds.Close(i);
                        //推出open的y值
                        k.Bottom = k.Top - (sds.Close(i) - _sds.Open(i)) / _sds.Open(i);
                        //由open的y值推出上影线的y值
                        k.Highest = k.Bottom + (_sds.Highest(i) - _sds.Open(i)) / _sds.Open(i);
                        //由open的y值推出下影线的y值
                        k.Lowest = k.Bottom + (_sds.Lowest(i) - _sds.Open(i)) / _sds.Open(i);
                        k.Trend = Trend.Rise;
                        //上涨时open为底部，此时底部作为左日的y值基准值
                        basey = k.Bottom;


                    }
                    else
                    {
                        //由于是由右侧向左逆推，
                        //因此由 basey（即右侧日open的y值） = k.Bottom（即当日close的y值） +  (basep（即右侧日open价格） - _sds.Close(i)) / _sds.Close(i)
                        //推出close的y值k.Bottom
                        k.Bottom = basey - (basep - _sds.Close(i)) / _sds.Close(i);
                        //推出open的y值
                        k.Top = k.Bottom - (_sds.Close(i) - _sds.Open(i)) / _sds.Open(i);
                        //由open的y值推出上影线的y值
                        k.Highest = k.Top + (_sds.Highest(i) - _sds.Open(i)) / _sds.Open(i);
                        //由open的y值推出下影线的y值
                        k.Lowest = k.Top + (_sds.Lowest(i) - _sds.Open(i)) / _sds.Open(i);
                        k.Trend = Trend.Fall;
                        //下降时open为顶部，此时顶部作为左日的y值基准值
                        basey = k.Top;

                    }


                }
                if(_sds.Close(i) == _sds.Open(i) && i>0)
                {
                    if(_sds.Close(i)< _sds.Close(i-1))
                    {
                        k.Trend = Trend.Fall;
                    }
                }
                //此时不计算k线柱的左右沿在x轴的绘图坐标，只记录索引，以便真正计算坐标时使用该索引用乘法计算坐标
                k.Left = i;
                k.Right = i;

                double top = _sds.Volume(_sds.MaxVolumeIndex);

                float voly = (float)(_sds.Volume(i) / top);

                VDataDrawInfo v = new VDataDrawInfo();
                v.Left = i;
                v.Right = i;
                v.Top = voly;
                v.Bottom = 0;
                v.Trend = k.Trend;
                r.K = k;
                r.V = v;
                //r.MA = ma;
                _originalddil.Add(i, r);
                basep = _sds.Open(i);
            }

            int[] ctpidx = new int[_sds.MAType.Length];
            TrendPiece ctp;
            float t = 0;
            float b = 0;
            float tmpy = 0;
            for (int y = 0; y <= ctpidx.Length - 1; y++)
            {
                //ctpidx[y] = _sds.MATPList(_sds.MAType[y]).Count - 1;
                ctpidx[y] = 0;
            }
            for (int i = 0; i <= _sds.Length - 1; i++)
            {
                if(i==3)
                {
                    int asdfas = 1;
                }
                MADataDrawInfo ma = new MADataDrawInfo(_sds.MAType.Length);
                for (int j = 0; j <= _sds.MAType.Length - 1; j++)
                {
                    ctpidx[j] = getCurrentTrendPieceIndex(_sds.MATPList(_sds.MAType[j]), i, ctpidx[j]);
                    ctp = _sds.MATPList(_sds.MAType[j])[ctpidx[j]];

                    if (_sds.Close(ctp.idxBegin) >=_sds.Open(ctp.idxBegin))
                    {
                        if (ctp.Trend == Trend.Rise)
                        {
                            b = _originalddil[(object)ctp.idxBegin].K.Bottom + (_sds.MA(ctp.idxBegin, _sds.MAType[j]) - _sds.Open(ctp.idxBegin)) / _sds.Open(ctp.idxBegin);
                        }
                        if (ctp.Trend == Trend.Fall)
                        {
                            t = _originalddil[(object)ctp.idxBegin].K.Bottom + (_sds.MA(ctp.idxBegin, _sds.MAType[j]) - _sds.Open(ctp.idxBegin)) / _sds.Open(ctp.idxBegin);
                        }
                        if (ctp.Trend == Trend.Flat)
                        {
                            b = t = _originalddil[(object)ctp.idxBegin].K.Bottom + (_sds.MA(ctp.idxBegin, _sds.MAType[j]) - _sds.Open(ctp.idxBegin)) / _sds.Open(ctp.idxBegin);
                        }
                    }
                    else
                    {
                        if (ctp.Trend == Trend.Rise)
                        {
                            b = _originalddil[(object)ctp.idxBegin].K.Top + (_sds.MA(ctp.idxBegin, _sds.MAType[j]) - _sds.Open(ctp.idxBegin)) / _sds.Open(ctp.idxBegin);
                        }
                        if (ctp.Trend == Trend.Fall)
                        {
                            t = _originalddil[(object)ctp.idxBegin].K.Top + (_sds.MA(ctp.idxBegin, _sds.MAType[j]) - _sds.Open(ctp.idxBegin)) / _sds.Open(ctp.idxBegin);
                        }
                        if (ctp.Trend == Trend.Flat)
                        {
                            b = t = _originalddil[(object)ctp.idxBegin].K.Top + (_sds.MA(ctp.idxBegin, _sds.MAType[j]) - _sds.Open(ctp.idxBegin)) / _sds.Open(ctp.idxBegin);
                        }
                    }

                    if (_sds.Close(ctp.idxEnd) >= _sds.Open(ctp.idxEnd))
                    {
                        if (ctp.Trend == Trend.Rise)
                        {
                            t = _originalddil[(object)ctp.idxEnd].K.Bottom + (_sds.MA(ctp.idxEnd, _sds.MAType[j]) - _sds.Open(ctp.idxEnd)) / _sds.Open(ctp.idxEnd);
                        }
                        if (ctp.Trend == Trend.Fall)
                        {
                            b = _originalddil[(object)ctp.idxEnd].K.Bottom + (_sds.MA(ctp.idxEnd, _sds.MAType[j]) - _sds.Open(ctp.idxEnd)) / _sds.Open(ctp.idxEnd);
                        }
                    }
                    else
                    {
                        if (ctp.Trend == Trend.Rise)
                        {
                            t = _originalddil[(object)ctp.idxEnd].K.Top + (_sds.MA(ctp.idxEnd, _sds.MAType[j]) - _sds.Open(ctp.idxEnd)) / _sds.Open(ctp.idxEnd);
                        }
                        if (ctp.Trend == Trend.Fall)
                        {
                            b = _originalddil[(object)ctp.idxEnd].K.Top + (_sds.MA(ctp.idxEnd, _sds.MAType[j]) - _sds.Open(ctp.idxEnd)) / _sds.Open(ctp.idxEnd);
                        }
                    }
                    if(ctp.Trend== Trend.Rise)
                    {
                        tmpy = (t - b) / (_sds.MA(ctp.idxEnd, _sds.MAType[j]) - _sds.MA(ctp.idxBegin, _sds.MAType[j]));
                        tmpy = b+ (_sds.MA(i, _sds.MAType[j]) - _sds.MA(ctp.idxBegin, _sds.MAType[j])) * tmpy;
                    }
                    else if (ctp.Trend == Trend.Fall)
                    {
                        tmpy = (t - b) / (_sds.MA(ctp.idxBegin, _sds.MAType[j]) - _sds.MA(ctp.idxEnd, _sds.MAType[j]));
                        tmpy =  b+ (_sds.MA(i, _sds.MAType[j]) - _sds.MA(ctp.idxEnd, _sds.MAType[j])) * tmpy;
                    }
                    else if (ctp.Trend == Trend.Flat)
                    {
                        tmpy = b;
                    }
                    ma.Values[j] = new PointF(i, tmpy);
                }
                _originalddil[(object)i].MA = ma;
            }
        }
        /// <summary>
        /// 获取指定的k线数据索引所处于曲线片段的索引
        /// 从参数输入的曲线片段索引开始，递减此索引进行比对计算
        /// </summary>
        /// <param name="tplist">曲线片段集合</param>
        /// <param name="i">当前日的k线数据索引</param>
        /// <param name="cindex">开始进行比对曲线片段的索引</param>
        /// <returns>曲线片段的索引</returns>
        private int getCurrentTrendPieceIndex(TrendPieceList tplist,int i,int cindex)
        {
            for(;cindex<= tplist.Count-1; cindex++)
            {
                if(i>= tplist[cindex].idxBegin && i<= tplist[cindex].idxEnd)
                {
                    return cindex;

                }
            }
            return -1;
        }

        public void setParams(int i, int offsetX, int offsetY, DrawKLine dkl, DrawVolumesLine dvl)
        {
            _drawddil = new THashTable<DataDrawInfo>();
            _version = _sds.StockCode + offsetX + offsetY + dkl.Left + dkl.Right + dkl.Top + dkl.Bottom;
            _basei = i;
            _offsetx = offsetX;
            _offsety = offsetY;

            _width = dkl.Width;

            _kheight = dkl.Height;
            _vheight = dvl.Height;

            _left = dkl.Left;
            _right = dkl.Right;

            _ktop = dkl.Top;
            _kbottom = dkl.Bottom;
            _vtop = dvl.Top;
            _vbottom = dvl.Bottom;

            _barwidth = dkl.BarWidth;
            _divide = dkl.Divide;
            _oneKPercentPixel = dkl.OnePercentPixels;
            _oneVPercentPixel = dvl.OnePercentPixels;

            _basey = _ktop + _kheight / 2 + offsetY;
            _basex = _left + _width / 2 + _offsetx;

            _edgeLi = i - (_basex - _left)/(_barwidth+_divide);
            if(_edgeLi < 0)
            {
                _edgeLi = 0;
            }
            //第0个bar和_left之间还有大于一个bar加间隔的距离，即基准值bar的左边到left的距离要大于i个bar加间隔的宽度
            //因此需要让k线居左
            if (_basex - i * (_barwidth + _divide) - _left > _barwidth + _divide)
            {
                _basex = _basex - (_barwidth + _divide) * ((_basex - _left) / (_barwidth + _divide)) + i * (_barwidth + _divide);
            }
            _edgeL = _basex - (i - _edgeLi) * (_barwidth + _divide);

            _edgeRi = i + (_right - _basex) / (_barwidth + _divide) - 1;
            if (_edgeRi > _sds.Length - 1)
            {
                _edgeRi = _sds.Length - 1;
            }
            _edgeR = _basex + (_edgeRi - i) * (_barwidth + _divide)+ _barwidth;

        }
        /// <summary>
        /// 根据移动的新位置重新设置计算显示信息的参数
        /// </summary>
        /// <param name="p">在绘制位图中新位置point</param>
        /// <param name="movetimestamp">按下鼠标键的时间戳，用来判断是否属于同一次按下鼠标键后的移动</param>
        /// <returns></returns>
        public bool setParamsForMoved(Point p,long movetimestamp)
        {
            if (DateTime.Now.Ticks - _tmstamp < 100000)
            {
                return false;
            }

            //鼠标移动到可绘制区域之外
            if (p.X < _left || p.Y < _ktop || p.X > _right || p.Y > _kbottom)
            {
                return false;
            }
            //此回按下鼠标键后的首次移动
            if (_oldp.X == -1 && _oldp.Y == -1)
            {
                _movetimestamp = movetimestamp;
                _oldp = p;
                return false;
            }
            //重新按下了鼠标键（和上面的判断重复）
            if (movetimestamp != _movetimestamp)
            {
                _movetimestamp = movetimestamp;
                _oldp = p;
                return false;
            }
            //鼠标当前位置和上一次重新计算位置的横向移动距离并非刚好间隔若干个bar加间隔
            //实现横向每次移动一个bar加间隔的距离
            if ((p.X - _oldp.X) % (_barwidth + _divide) != 0)
            {
                //当前的位置和上次重新计算位置的横向距离不足1个bar加间隔，此时图像不做横向移动
                if (Math.Abs(p.X-_oldp.X) < _barwidth + _divide)
                {
                    p.X = _oldp.X;
                }
                //大于1个bar加间隔，使图像横向移动正好整数个bar加间隔，实现横向每次移动一个bar加间隔的距离
                else
                {
                    p.X = _oldp.X + (p.X - _oldp.X)/(_barwidth + _divide) * (_barwidth + _divide);
                }
                
                //return false;
            }
            //鼠标向右侧拖动并且左侧边缘的k线柱索引是0
            //既k线已经向右拖动到头，无法继续拖动
            if (_edgeLi == 0 && p.X - _oldp.X > 0)
            {
                p.X = _oldp.X;
            }
            //k线已经向左拖动到头，无法继续拖动
            if ( _edgeRi == _sds.Length - 1 && p.X - _oldp.X < 0)
            {
                p.X = _oldp.X;
            }
            //经过处理后鼠标新位置相对于原位置没有变化
            if(p.X==_oldp.X && p.Y==_oldp.Y)
            {
                return false;
            }
            string newversion = _sds.StockCode + p.X + p.Y + _left + _right + _ktop + _kbottom;
            if (newversion == _version)
            {
                return false;
            }
            _version = newversion;

            _drawddil = new THashTable<DataDrawInfo>();
            //int offsetx= _offsetx + p.X - _oldp.X;
            //int offsety= _offsety + p.Y - _oldp.Y;
            _offsetx += p.X - _oldp.X;
            _offsety += p.Y - _oldp.Y;
           
            //_basey = _ktop + _kheight / 2 + _offsety;
            //_basex = _left + _width / 2 + _offsetx;
            //累计偏移量到基础参照值
            _basey += p.Y - _oldp.Y;
            _basex += p.X - _oldp.X;

            //当参照bar的左竖边向左首次移出显示区域后，_basex - _left的绝对值（_basex - _left为负值）不足一个bar加上间隔的宽度，即此时_edgeLi仍会等于_basei
            //因此参照bar会作为有效绘制内容绘制可显示的右侧部分在显示区域中。所以此时应让_edgeLi = _basei + 1，以便不显示_basei。
            //以此推论，当参照bar向左移出显示区域后，_edgeLi应该向右侧多计算一个
            if (_basex< _left)
            {
                _edgeLi = _basei - ((_basex - _left) / (_barwidth + _divide) - 1);
            }
            else
            {
                _edgeLi = _basei - (_basex - _left) / (_barwidth + _divide);
            }
            //计算得到左边缘bar的k线数据索引小于0，需要置为0，且取消偏移量的累计
            if (_edgeLi < 0)
            {
                _edgeLi = 0;
                _offsetx -= p.X - _oldp.X;
                _basex -= p.X - _oldp.X;
            }

            _edgeL = _basex - (_basei - _edgeLi) * (_barwidth + _divide);

            _edgeRi = _basei + (_right - _basex) / (_barwidth + _divide) - 1;
            //计算得到左边缘bar的k线数据索引超出范围，需要置为最大值，且取消偏移量的累计
            if (_edgeRi > _sds.Length - 1)
            {
                _edgeRi = _sds.Length - 1;
                _offsetx -= p.X - _oldp.X;
                _basex -= p.X - _oldp.X;
            }
            _edgeR = _basex + (_edgeRi - _basei) * (_barwidth + _divide) + _barwidth;
            _oldp = p;
            _tmstamp = DateTime.Now.Ticks;
            return true;
        }
        /// <summary>
        /// 获取指定索引的实际绘制信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataDrawInfo DDInfo(object index)
        {
            if (!_originalddil.ContainsKey(index))
            {
                return null;
            }
            int ci = (int)index;
            //index不在可绘制数据范围内
            if(ci<_edgeLi || ci>_edgeRi)
            {
                return null;
            }
            //已存在计算好的数据
            if(_drawddil.ContainsKey(index))
            {
                return _drawddil[index];
            }
            
            DataDrawInfo r = new DataDrawInfo();
            KDataDrawInfo k = new KDataDrawInfo();
            VDataDrawInfo v = new VDataDrawInfo();
            MADataDrawInfo ma = new MADataDrawInfo(_originalddil[index].MA.Values.Length);

            //当前需计算的数据，k线索引在参照日左侧
            if (ci<_basei)
            {
                k.Left = _basex - (_basei - ci) * (_barwidth + _divide);
            }
            //当前需计算的数据，k线索引在参照日右侧
            else if (ci>_basei)
            {
                k.Left = _basex + (ci - _basei) * (_barwidth + _divide);
            }
            else
            {
                k.Left = _basex;
            }
            k.Right = k.Left + _barwidth - 1;
            //参照日上升，open为bottom，以bottom为幅度的参照值
            if(_sds.Close(_basei)>=_sds.Open(_basei))
            {
                k.Bottom = _basey - (_originalddil[index].K.Bottom - _originalddil[(object)_basei].K.Bottom) * 100 * _oneKPercentPixel;
                k.Top = _basey - (_originalddil[index].K.Top - _originalddil[(object)_basei].K.Bottom) * 100 * _oneKPercentPixel;
                k.Highest = _basey - (_originalddil[index].K.Highest - _originalddil[(object)_basei].K.Bottom) * 100 * _oneKPercentPixel;
                k.Lowest = _basey - (_originalddil[index].K.Lowest - _originalddil[(object)_basei].K.Bottom) * 100 * _oneKPercentPixel;
            }
            //参照日下降，open为top，以top为幅度的参照值
            else
            {
                k.Bottom = _basey - (_originalddil[index].K.Bottom - _originalddil[(object)_basei].K.Top) * 100 * _oneKPercentPixel;
                k.Top = _basey - (_originalddil[index].K.Top - _originalddil[(object)_basei].K.Top) * 100 * _oneKPercentPixel;
                k.Highest = _basey - (_originalddil[index].K.Highest - _originalddil[(object)_basei].K.Top) * 100 * _oneKPercentPixel;
                k.Lowest = _basey - (_originalddil[index].K.Lowest - _originalddil[(object)_basei].K.Top) * 100 * _oneKPercentPixel;
            }
            k.Trend = _originalddil[index].K.Trend;
            r.K = k;

            v.Top = _vbottom - (int)(_originalddil[index].V.Top * 100 * _oneVPercentPixel) + 1;
            //交易量幅度小于1%时，占0像素，此时交易量柱上沿低于_bottom，因此需令上沿等于_bottom
            if (v.Top > _vbottom)
            {
                v.Top = _vbottom;
            }

            v.Bottom = _vbottom;
            v.Left = k.Left;
            v.Right = k.Right;
            v.Trend = k.Trend;
            r.V = v;
            //参照日上升，open为bottom，以bottom为幅度的参照值
            if (_sds.Close(_basei) >= _sds.Open(_basei))
            {
                for (int j = 0; j <= _originalddil[index].MA.Values.Length - 1; j++)
                {
                    ma.Values[j] = new PointF(k.Left + (int)((_barwidth - 1) / 2), _basey - (_originalddil[index].MA.Values[j].Y - _originalddil[(object)_basei].K.Bottom) * 100 * _oneKPercentPixel);
                }
            }
            //参照日下降，open为top，以top为幅度的参照值
            else
            {
                for (int j = 0; j <= _originalddil[index].MA.Values.Length - 1; j++)
                {
                    ma.Values[j] = new PointF(k.Left + (int)((_barwidth - 1) / 2), _basey - (_originalddil[index].MA.Values[j].Y - _originalddil[(object)_basei].K.Top) * 100 * _oneKPercentPixel);
                }
            }
            r.MA = ma;
            _drawddil.Add(index, r);
            return r;
        }

        /// <summary>
        /// 获取指定point所在K线柱的索引
        /// 判定范围在k线柱的上下沿各扩展1px
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ddil">股票的绘制信息</param>
        /// <returns>k线柱索引。若point未落在k线柱上，返回-1</returns>
        public int positionToKIndex(Point p)
        {
            int ci = -1;
            if (p.X >= _edgeL && p.X <= _edgeR && _edgeL > 0)
            {
                //计算当前鼠标x位置在第几个k线柱范围内。因整数除法无法整除时向下取整，所以+1
                ci = (p.X - _edgeL) / (_barwidth + _divide);
                if (p.X > ci * (_barwidth + _divide))
                {
                    ci++;
                }
                //鼠标在最左侧k线柱范围内
                if (p.X <= _edgeL + _barwidth)
                {
                    ci = _edgeLi;
                }
                //鼠标在最右侧k线柱范围内
                else if (p.X >= _edgeR - _barwidth)
                {
                    ci = _edgeRi;
                    //p.X = edgeR - (int)Math.Round((float)barwidth / 2);
                }
                //判断鼠标挪动到左侧k线柱边缘时，才将跟随线挪到左侧k线柱中间
                else if (p.X < _edgeL + (ci) * (_barwidth + _divide) - _divide
                    &&
                    p.X > _edgeL + (ci - 1) * (_barwidth + _divide)
                    )
                {
                    ci = _edgeLi + ci - 1;
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
                if (p.Y > _drawddil[(object)ci].K.Top - 1 && p.Y < _drawddil[(object)ci].K.Bottom + 1)
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
        public int xToKIndex(Point p)
        {
            int ci = -1;
            if (p.X >= _edgeL && p.X <= _edgeR && _edgeR > 0)
            {
                //计算当前鼠标x位置在第几个k线柱范围内。因整数除法无法整除时向下取整，所以+1
                ci = (p.X - _edgeL) / (_barwidth + _divide);
                if (p.X >= _edgeL + ci * (_barwidth + _divide) - 1)
                {
                    ci++;
                }
                //鼠标在最左侧k线柱范围内
                if (p.X <= _edgeL + _barwidth)
                {
                    ci = _edgeLi;
                }
                //鼠标在最右侧k线柱范围内
                else if (p.X >= _edgeR - _barwidth)
                {
                    ci = _edgeRi;
                    //p.X = edgeR - (int)Math.Round((float)barwidth / 2);
                }
                //判断鼠标挪动到左侧k线柱边缘时，才将跟随线挪到左侧k线柱中间
                else if (p.X < _edgeL + (ci) * (_barwidth + _divide) - _divide
                    &&
                    p.X > _edgeL + (ci - 1) * (_barwidth + _divide)
                    )
                {
                    ci = _edgeLi + ci - 1;
                    //p.X = edgeL + (ci) * (barwidth + divide) - divide - (int)Math.Round((float)barwidth / 2);
                }
                else if (p.X > _edgeL + (ci - 1) * (_barwidth + _divide) - 1
                    &&
                    p.X < _edgeL + (ci) * (_barwidth + _divide) - _divide
                    )
                {
                    ci = _edgeLi + ci - 1;
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
