using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 绘制底部当前股票信息区域
    /// </summary>
    public class DrawStockInfoBottom : DrawBase
    {
        public DrawStockInfoBottom(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC) : base(x, y, Width, Height, hBitmap, hDC)
        {
        }
    }
}
