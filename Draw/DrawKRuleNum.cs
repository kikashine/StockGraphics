using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 绘制右侧K线比例坐标数字
    /// </summary>
    public class DrawKRuleNum : DrawBase
    {
        //private IntPtr oldfont;
        //private IntPtr cfont;
        public DrawKRuleNum(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC):base(x,y,Width,Height,hBitmap,hDC)
        {
            //cfont = CreatFont("宋体", 10, FontStyle.Regular);
            //oldfont = SelectObject(_hdc, cfont);
        }
        /// <summary>
        /// 绘制右侧K线比例坐标数字
        /// </summary>
        /// <param name="ratio"></param>
        public void drawKRuleNum(int ratio)
        {
            //10%所占的默认像素数
            int tenper = 50;
            //乘以系数后10%所占的默认像素数
            int curtenper = 50 / ratio;
            IntPtr cfont = CreatFont("宋体", 12, FontStyle.Regular);
            IntPtr oldfont = SelectObject(_hdc, cfont);

            int mid = _y + _height / 2;
            int count = 0;
            string str = "";
            //绘制0%
            TextOutW(_hdc, _x + 1, mid - 6, "0%", 2);
            //从0%向上每20%画一次
            for (int i = mid - curtenper * 2; i > _y;)
            {
                //绘制的数字，每次加20
                count += 20;
                str = Convert.ToString(count) + '%';
                TextOutW(_hdc, _x + 1, i - 6, str, str.Length);
                i = i - curtenper * 2;
            }
            count = 0;
            //从0%向下每20%画一次
            for (int i = mid + curtenper * 2; i < _bottom;)
            {
                count -= 20;
                str = Convert.ToString(count) + '%';
                TextOutW(_hdc, _x + 1, i - 6, str, str.Length);
                i = i + curtenper * 2;

            }

            if (oldfont != IntPtr.Zero && cfont != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldfont));
        }

        public void Dispose()
        {

        }
    }
}
