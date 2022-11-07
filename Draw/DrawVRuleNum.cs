using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 绘制交易量标尺数字
    /// </summary>
    class DrawVRuleNum :DrawBase
    {
        public DrawVRuleNum(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC) : base(x, y, Width, Height, hBitmap, hDC)
        {
            //cfont = CreatFont("宋体", 10, FontStyle.Regular);
            //oldfont = SelectObject(_hdc, cfont);
        }

        /// <summary>
        /// 绘制交易量标尺数字
        /// </summary>
        /// <param name="ratio"></param>
        public void drawVRuleNum(int ratio)
        {
            //10%所占的默认像素数
            int tenper = 10;
            //乘以系数后10%所占的默认像素数
            int curtenper = 10 / ratio;
            IntPtr cfont = CreatFont("宋体", 12, FontStyle.Regular);
            IntPtr oldfont = SelectObject(_hdc, cfont);

            //int mid = _y + _height / 2;
            int count = 0;
            string str = "";
            //每25%画一次
            for (int i = (int)(_bottom - curtenper * 2.5); i > _y;)
            {
                count += 25;
                str = Convert.ToString(count) + '%';
                TextOutW(_hdc, _x + 1, i - 5, str, str.Length);
                i = (int)(i - curtenper * 2.5);
            }

            if (oldfont != IntPtr.Zero && cfont != IntPtr.Zero) DeleteObject(SelectObject(_hdc, oldfont));
        }

        public void Dispose()
        {

        }
    }
}
