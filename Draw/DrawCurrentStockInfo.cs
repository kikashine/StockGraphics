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
    /// 绘制顶部当前股票信息区域
    /// 暂不需使用，使用前应更新调整代码，例如跟随先所在k线索引部分。2020/03/04
    /// </summary>
    public class DrawCurrentStockInfo : DrawBase
    {
        /// <summary>
        /// 是否显示本区域
        /// </summary>
        public bool Display = true;

        private Point _oldp = new Point(-1, -1);
        /// <summary>
        /// 返回绘制顶部当前股票信息区域的实例
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="hBitmap"></param>
        /// <param name="hDC"></param>
        public DrawCurrentStockInfo(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC) : base(x, y, Width, Height, hBitmap, hDC)
        {
        }
        /// <summary>
        /// 在区域绘制鼠标跟随线所在股票的信息
        /// 仅绘制到hDC、hBitmap对应的绘图位图上，若想体现在目标设备上需调用AreaForDraw的Apply()方法
        /// </summary>
        /// <param name="p">鼠标所在坐标</param>
        /// <param name="barwidth">k线柱宽度（像素）</param>
        /// <param name="divide">k线柱之间间隔宽度（像素）</param>
        /// <param name="edgeL">最左侧第一个k线柱的左边沿在bitmap中的x轴坐标值</param>
        /// <param name="edgeR">最右侧第一个k线柱的右边沿在bitmap中的x轴坐标值</param>
        /// <param name="edgeLi">最左侧第一个k线柱的索引值</param>
        /// <param name="edgeRi">最右侧第一个k线柱的索引值</param>
        /// <param name="sds">当前股票数据集合</param>
        public void drawCurrentStockInfo(Point p, int barwidth, int divide, int edgeL, int edgeR, int edgeLi, int edgeRi, StockDataSet sds)
        {
            int ci = -1;
            if (p.X >= edgeL && p.X <= edgeR && edgeR > 0)
            {
                //计算当前鼠标x位置在第几个k线柱范围内。因整数除法无法整除时向下取整，所以+1
                ci = (p.X - edgeL) / (barwidth + divide);
                if (p.X > ci * (barwidth + divide))
                {
                    ci++;
                }
                //计算上次绘制时鼠标x位置在第几个k线柱范围内。
                int bi = (_oldp.X - edgeL) / (barwidth + divide) + 1;

                //鼠标在最左侧k线柱范围内
                if (p.X <= edgeL + barwidth)
                {
                    ci = edgeLi;
                }
                //鼠标在最右侧k线柱范围内
                else if (p.X >= edgeR - barwidth)
                {
                    ci = edgeRi;
                    //p.X = edgeR - (int)Math.Round((float)barwidth / 2);
                }
                //当前鼠标所处k线柱x轴区域与上次绘制时不同
                else if (ci > 0 && ci != bi)
                {
                    //向左挪动
                    if (p.X < _oldp.X)
                    {
                        //判断鼠标挪动到左侧k线柱边缘时，才将跟随线挪到左侧k线柱中间
                        if (p.X < edgeL + (ci) * (barwidth + divide) - divide)
                        {
                            ci = edgeLi + ci - 1;
                            //p.X = edgeL + (ci) * (barwidth + divide) - divide - (int)Math.Round((float)barwidth / 2);
                        }
                        else
                        {
                            ci = -1;
                            //p.X = _oldp.X;
                        }
                    }
                    //向右移动，当ci变化（与bi不同）时，说明鼠标移动到右侧k线区域开始的位置（k线左沿），将跟随线挪到右侧k线柱中间
                    else
                    {
                        ci = edgeLi + ci - 1;
                        //p.X = edgeL + ci * (barwidth + divide) - divide - (int)Math.Round((float)barwidth / 2);
                    }
                }
                //当前鼠标所处k线柱x轴位置与上次绘制时相同，不改变x轴坐标
                else
                {
                    ci = -1;
                    //p.X = _oldp.X;
                }
            }
            if(ci>=0)
            {
                SetROP2(_hdc, BinaryRasterOperations.R2_COPYPEN);
                IntPtr oldbrush = IntPtr.Zero;
                IntPtr brush = IntPtr.Zero;
                Color brushcolor = Color.FromArgb(0x00eeeeee);

                brush = CreateSolidBrush((int)ColorTranslator.ToWin32(brushcolor));
                oldbrush = SelectObject(_hdc, brush);

                Rectangle(_hdc, _x, _y, _x+700, _bottom -1);


                SetROP2(_hdc, BinaryRasterOperations.R2_COPYPEN);
                IntPtr cfont = CreatFont("宋体", 12, FontStyle.Regular);
                IntPtr oldfont = SelectObject(_hdc, cfont);
                string str = "code：" + sds.StockCode;
                TextOutW(_hdc, _x + 3, _y + 3 , str, str.Length);
                str = "i：" + ci;
                TextOutW(_hdc, _x + 3 + 90, _y + 3, str, str.Length);
                str = "开：" + sds.Open(ci);
                TextOutW(_hdc, _x + 3 + 150, _y + 3, str, str.Length);
                str = "收：" + sds.Close(ci);
                TextOutW(_hdc, _x + 3 + 210, _y + 3, str, str.Length);
                str = "高：" + sds.Highest(ci);
                TextOutW(_hdc, _x + 3 + 270, _y + 3, str, str.Length);
                str = "低：" + sds.Lowest(ci);
                TextOutW(_hdc, _x + 3 + 330, _y + 3, str, str.Length);
                str = "量：" + (sds.Volume(ci)/10000).ToString("0.00")+"万";
                TextOutW(_hdc, _x + 3 + 390, _y + 3, str, str.Length);
                str = "量比：" + ((float)(sds.Volume(ci)/ sds.Volume(sds.MaxVolumeIndex))).ToString("0.000");
                TextOutW(_hdc, _x + 3 + 480, _y + 3, str, str.Length);

                if (oldfont != IntPtr.Zero && cfont != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldfont));

                if (oldbrush != IntPtr.Zero && brush != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldbrush));
            }
        }

        public void Dispose()
        {

        }
    }
}
