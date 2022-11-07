using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockToolKit.Common;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace StockToolKit.Analyze
{
    public class labelStockInfo :Label
    {
        private System.ComponentModel.Container components = null;

      
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
        //        return cp;
        //    }
        //}

        public labelStockInfo(int width, int height)
        {
            //this.BackColor = System.Drawing.SystemColors.Window;
            this.Size = new System.Drawing.Size(width, height);
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //this.BorderStyle = DashStyle.Solid;
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            //this.BackColor = Color.Transparent;

            //this.ForeColor = Color.Transparent;
            // this.ForeColor = Color.FromArgb(50, Color.White);
            this.BackColor = Color.White;
        }

        public void showStockData(StockDataSet sds, int i)
        {
            Graphics g = this.CreateGraphics();//获取Graphics对象。
            if (i < 0)
            {
                //g.Clear(Color.White);
                return;
            }
            
            string text = "aaaaa";//获取当前要绘制的行的显示文本。
            //Pen risepen = new Pen(Color.FromArgb(204, 153, 255), 1);
            //Pen fallpen = new Pen(Color.FromArgb(0x0099CC99), 1);
            Font textfont = new Font("宋体", (float)9.3, FontStyle.Regular);
            Color color = Color.FromArgb(204, 153, 255);
            if (sds.Open(i) > sds.Close(i))
            {
                color = Color.FromArgb(0x0099CC99);
            }

            //Brush b = new SolidBrush(Color.FromArgb(0, Color.White));
            g.Clear(Color.White);
            //dashpen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            //Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height/2);
            //g.FillRectangle(b, rect);
            //绘制选中时要显示的蓝色边框。

            // g.DrawRectangle(Pens.Blue, 0, 0, this.Width, this.Height);
            int h = 12;
            text = sds.StockCode;
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, 0, this.Width - 1, h), Color.Black,
                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "i：" + i;
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = sds.Date(i).Date.ToShortDateString();
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "开：";
             TextRenderer.DrawText(g, text, textfont, new Rectangle(0,  h - 11, this.Width - 1, h), Color.Black,
                                   TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = sds.Open(i).ToString("0.00");
            TextRenderer.DrawText(g, text, textfont, new Rectangle(25, h - 11, this.Width - 1, h), color,
                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "收：";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = sds.Close(i).ToString("0.00");
            TextRenderer.DrawText(g, text, textfont, new Rectangle(25, h - 11, this.Width - 1, h), color,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "高：";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = sds.Highest(i).ToString("0.00");
            TextRenderer.DrawText(g, text, textfont, new Rectangle(25, h - 11, this.Width - 1, h), color,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "低：" ;
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = sds.Lowest(i).ToString("0.00");
            TextRenderer.DrawText(g, text, textfont, new Rectangle(25, h - 11, this.Width - 1, h), color,
                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "当日升：";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = (sds.PriceRise(sds.Open(i), sds.Close(i))*100).ToString("0.00") + "%";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(50, h - 11, this.Width - 1, h), color,
                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "前日升：";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            if (i-1>=0)
            {
                text =(sds.PriceRise(sds.Close(i - 1), sds.Close(i))*100).ToString("0.00") + "%";
            }
            else
            {
                text = "--";
            }
            TextRenderer.DrawText(g, text, textfont, new Rectangle(50, h - 11, this.Width - 1, h), color,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "量：";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = (sds.Volume(i)/10000).ToString("0.00") + "万";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(25, h - 11, this.Width - 1, h), color,
                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "量比：";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = ((float)(sds.Volume(i) / sds.Volume(sds.MaxVolumeIndex))).ToString("0.000");
            TextRenderer.DrawText(g, text, textfont, new Rectangle(35, h - 11, this.Width - 1, h), color,
                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "ma4：";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = sds.MA(i,"4").ToString("0.00");
            TextRenderer.DrawText(g, text, textfont, new Rectangle(35, h - 11, this.Width - 1, h), Color.FromArgb(0x00eebbee),
                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "ma8：";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = sds.MA(i, "8").ToString("0.00");
            TextRenderer.DrawText(g, text, textfont, new Rectangle(35, h - 11, this.Width - 1, h), Color.FromArgb(0x00ddcc77),
                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "ma12：";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = sds.MA(i, "12").ToString("0.00");
            TextRenderer.DrawText(g, text, textfont, new Rectangle(35, h - 11, this.Width - 1, h), Color.FromArgb(0x0088ccdd),
                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            h += 12;
            text = "ma24：";
            TextRenderer.DrawText(g, text, textfont, new Rectangle(0, h - 11, this.Width - 1, h), Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            text = sds.MA(i, "24").ToString("0.00");
            TextRenderer.DrawText(g, text, textfont, new Rectangle(35, h - 11, this.Width - 1, h), Color.FromArgb(0x00c1b095),
                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }
 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
