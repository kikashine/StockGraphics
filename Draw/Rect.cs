using System.Drawing;


namespace StockToolKit.Analyze
{
    /// <summary>
    /// 绘制区域结构体
    /// </summary>
    public struct Rect
    {

        public int Left, Top, Right, Bottom;

        public Rect(Rectangle r)
        {

            this.Left = r.Left;

            this.Top = r.Top;

            this.Bottom = r.Bottom;

            this.Right = r.Right;

        }

    }
}
