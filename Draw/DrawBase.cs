using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.IO;
using StockToolKit.Common;
using System.Runtime.InteropServices;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 对GDI函数的封装以及一些基本的图形属性和绘图方法
    /// </summary>
    public class DrawBase
    {
        //正确完整的GDI绘图基本方法：https://stackoverflow.com/questions/2302550/bitblt-code-not-working
        //GDI绘制字符串相关：http://www.cnblogs.com/canson/archive/2011/07/09/2101862.html
        /// <summary>
        /// 图像数据源到目标设备传送函数
        /// </summary>
        /// <param name="hdcDest"></param>
        /// <param name="nXDest"></param>
        /// <param name="nYDest"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="hdcSrc"></param>
        /// <param name="nXSrc"></param>
        /// <param name="nYSrc"></param>
        /// <param name="dwRop"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        protected static extern bool BitBlt(
        IntPtr hdcDest, //目标设备的句柄  
        int nXDest,     // 目标对象的左上角的X坐标  
        int nYDest,     // 目标对象的左上角的Y坐标  
        int nWidth,     // 目标对象的矩形的宽度  
        int nHeight,    // 目标对象的矩形的长度  
        IntPtr hdcSrc,  // 源设备的句柄  
        int nXSrc,      // 源对象的左上角的X坐标  
        int nYSrc,      // 源对象的左上角的Y坐标  
        TernaryRasterOperations dwRop       // 光栅的操作值 指源位图与目标位图以及图案刷的颜色值进行布尔运算的方式，以下列出了常用的光栅操作码及含义
                                            /*
                                BLACKNESS 用黑色填充目标矩形区域.
                                DSTINVERT 将目标矩形图象进行反相. 
                                MERGECOPY 将源矩形图象与指定的图案刷(Pattern)进行布尔"与"运算. 
                                MERGEPAINT 将源矩形图形经过反相后，与目标矩形图象进行布尔"或"运算.
                                NOTSRCCOPY 将源矩形图象经过反相后，复制到目标矩形上.
                                NOTSRCERASE 先将源矩形图象与目标矩形图象进行布尔"或"运算，然后再将得图象进行反相.
                                PATCOPY 将指定的图案刷复制到目标矩形上.
                                PATINVERT 将指定的图案刷与目标矩形图象进行布尔"异或"运算.
                                PATPAINT 先将源矩形图象进行反相，与指定的图案刷进行布尔"或"运算，再与目标矩形图象进行布尔"或"运算SRCAND 将源矩形图象与目标矩形图象进行布尔"与"运算.
                                SRCCOPY 将源矩形图象直接复制到目标矩形上.
                                SRCERASE 将目标矩形图象进行反相，再与源矩形图象进行布尔"与"运算.
                                SRCINVERT 将源矩形图象与目标矩形图象进行布尔"异或"运算.
                                SRCPAINT 将源矩形图象与目标矩形图象进行布尔"或"运算.
                                WHITENESS 用白色填充目标矩形区域.
                                             */
);
        protected enum TernaryRasterOperations
        {

            SRCCOPY = 0x00CC0020, /* dest = source */

            SRCPAINT = 0x00EE0086, /* dest = source OR dest */

            SRCAND = 0x008800C6, /* dest = source AND dest */

            SRCINVERT = 0x00660046, /* dest = source XOR dest */

            SRCERASE = 0x00440328, /* dest = source AND (NOT dest ) */

            NOTSRCCOPY = 0x00330008, /* dest = (NOT source) */

            NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */

            MERGECOPY = 0x00C000CA, /* dest = (source AND pattern) */

            MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest */

            PATCOPY = 0x00F00021, /* dest = pattern */

            PATPAINT = 0x00FB0A09, /* dest = DPSnoo */

            PATINVERT = 0x005A0049, /* dest = pattern XOR dest */

            DSTINVERT = 0x00550009, /* dest = (NOT dest) */

            BLACKNESS = 0x00000042, /* dest = BLACK */

            WHITENESS = 0x00FF0062, /* dest = WHITE */

        };
        /// <summary>
        /// 创建一个与指定设备兼容的内存设备上下文环境（DC）
        /// </summary>
        /// <param name="hdcPtr"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        protected static extern IntPtr CreateCompatibleDC(IntPtr hdcPtr);
        /// <summary>
        /// 选择一对象到指定的设备上下文环境中，该新对象替换先前的相同类型的对象
        /// </summary>
        /// <param name="hdcPtr"></param>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", ExactSpelling = true)]
        protected static extern IntPtr SelectObject(IntPtr hdcPtr, IntPtr hObject);
        /// <summary>
        /// 删除指定的设备上下文环境（Dc）
        /// </summary>
        /// <param name="hdcPtr"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", ExactSpelling = true)]
        protected static extern bool DeleteDC(IntPtr hdcPtr);
        /// <summary>
        /// 删除一个逻辑笔、画笔、字体、位图、区域或者调色板，释放所有与该对象有关的系统资源，在对象被删除之后，指定的句柄也就失效了
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        protected static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// 指定的样式、宽度和颜色创建画笔
        /// </summary>
        /// <param name="fnPenStyle"></param>
        /// <param name="width"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        [DllImport("Gdi32.dll")]
        protected static extern IntPtr CreatePen(PenStyle fnPenStyle, int width, int color);
        /// <summary>
        /// 创建一个具有指定颜色的逻辑实心刷子
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [DllImport("Gdi32.dll")]
        protected static extern IntPtr CreateSolidBrush(int color);
        protected enum PenStyle
        {
            /// <summary>
            /// 实线
            /// </summary>
            PS_SOLID = 0, //实线
            /// <summary>
            /// 段线; 要求笔宽<=1
            /// </summary>
            PS_DASH = 1,//段线; 要求笔宽<=1
            /// <summary>
            /// 点线; 要求笔宽<=1
            /// </summary>
            PS_DOT = 2, //点线; 要求笔宽<=1
            /// <summary>
            /// 线、点; 要求笔宽<=1
            /// </summary>
            PS_DASHDOT = 3, //线、点; 要求笔宽<=1
            /// <summary>
            /// 线、点、点; 要求笔宽<=1
            /// </summary>
            PS_DASHDOTDOT = 4, //线、点、点; 要求笔宽<=1
            /// <summary>
            /// 不可见
            /// </summary>
            PS_NULL = 5, //不可见
            /// <summary>
            /// 实线; 但笔宽是向里扩展
            /// </summary>
            PS_INSIDEFRAME = 6, //实线; 但笔宽是向里扩展
            PS_USERSTYLE = 7,
            PS_ALTERNATE = 8,
            PS_STYLE_MASK = 0x0000000F,

            PS_ENDCAP_ROUND = 0x00000000,
            PS_ENDCAP_SQUARE = 0x00000100,
            PS_ENDCAP_FLAT = 0x00000200,
            PS_ENDCAP_MASK = 0x00000F00,

            PS_JOIN_ROUND = 0x00000000,
            PS_JOIN_BEVEL = 0x00001000,
            PS_JOIN_MITER = 0x00002000,
            PS_JOIN_MASK = 0x0000F000,

            PS_COSMETIC = 0x00000000,
            PS_GEOMETRIC = 0x00010000,
            PS_TYPE_MASK = 0x000F0000
        }
        /// <summary>
        /// 用于设定当前前景色的混合模式
        /// 函数调用成功后返回调用前的模式，调用失败则返回零
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="rop"></param>
        /// <returns></returns>
        [DllImport("Gdi32.dll")]

        protected static extern int SetROP2(System.IntPtr hdc, BinaryRasterOperations rop);
        /*取值
            R2_BLACK//Pixel is always black. 所有绘制出来的像素为黑色
            R2_WHITE//Pixel is always white. 所有绘制出来的像素为白色
            R2_NOP//Pixel remains unchanged. 任何绘制将不改变当前的状态
            R2_NOT//Pixel is the inverse of the screen color. 当前绘制的像素值设为屏幕像素值的反，这样可以覆盖掉上次的绘图，（自动擦除上次绘制的图形）
            R2_COPYPEN//Pixel is the pen color. 使用当前的画笔的颜色
            R2_NOTCOPYPEN//Pixel is the inverse of the pen color. 当前画笔的反色
        //下面是当前画笔的颜色和屏幕色的组合运算得到的绘图模式。
            R2_MERGEPENNOT//Pixel is a combination of the pen color and the inverse of the screen color (final pixel = (NOT screen pixel) OR pen). R2_COPYPEN和R2_NOT的并集
            R2_MASKPENNOT//Pixel is a combination of the colors common to both the pen and the inverse of the screen (final pixel = (NOT screen pixel) AND pen). R2_COPYPEN和R2_NOT的交集
            R2_MERGENOTPEN//Pixel is a combination of the screen color and the inverse of the pen color (final pixel = (NOT pen) OR screen pixel). R2_NOTCOPYPEN和屏幕像素值的并集
            R2_MASKNOTPEN//Pixel is a combination of the colors common to both the screen and the inverse of the pen (final pixel = (NOT pen) AND screen pixel).R2_NOTCOPYPEN和屏幕像素值的交集
            R2_MERGEPEN//Pixel is a combination of the pen color and the screen color (final pixel = pen OR screen pixel). R2_COPYPEN和屏幕像素值的并集
            R2_NOTMERGEPEN//Pixel is the inverse of the R2_MERGEPEN color (final pixel = NOT(pen OR screen pixel)). R2_MERGEPEN的反色
            R2_MASKPEN//Pixel is a combination of the colors common to both the pen and the screen (final pixel = pen AND screen pixel). R2_COPYPEN和屏幕像素值的交集
            R2_NOTMASKPEN//Pixel is the inverse of the R2_MASKPEN color (final pixel = NOT(pen AND screen pixel)). R2_MASKPEN的反色
            R2_XORPEN//Pixel is a combination of the colors that are in the pen or in the screen, but not in both (final pixel = pen XOR screen pixel). R2_COPYPEN和屏幕像素值的异或
            R2_NOTXORPEN//Pixel is the inverse of the R2_XORPEN color (final pixel = NOT(pen XOR screen pixel)). R2_XORPEN的反色*/
        protected enum BinaryRasterOperations
        {
            /// <summary>
            /// 所有绘制出来的像素为黑色
            /// </summary>
            R2_BLACK = 1,
            /// <summary>
            /// R2_MERGEPEN的反色
            /// </summary>
            R2_NOTMERGEPEN = 2,
            /// <summary>
            /// R2_NOTCOPYPEN和屏幕像素值的交集
            /// </summary>
            R2_MASKNOTPEN = 3,
            /// <summary>
            /// 当前画笔的反色
            /// </summary>
            R2_NOTCOPYPEN = 4,
            /// <summary>
            /// R2_COPYPEN和R2_NOT的交集
            /// </summary>
            R2_MASKPENNOT = 5,
            /// <summary>
            /// 当前绘制的像素值设为屏幕像素值的反，这样可以覆盖掉上次的绘图，（自动擦除上次绘制的图形）
            /// </summary>
            R2_NOT = 6,
            /// <summary>
            /// R2_COPYPEN和屏幕像素值的异或
            /// </summary>
            R2_XORPEN = 7,
            /// <summary>
            /// R2_MASKPEN的反色
            /// </summary>
            R2_NOTMASKPEN = 8,
            /// <summary>
            /// R2_COPYPEN和屏幕像素值的交集
            /// </summary>
            R2_MASKPEN = 9,
            /// <summary>
            /// R2_XORPEN的反色
            /// </summary>
            R2_NOTXORPEN = 10,
            /// <summary>
            /// 任何绘制将不改变当前的状态
            /// </summary>
            R2_NOP = 11,
            /// <summary>
            /// R2_NOTCOPYPEN和屏幕像素值的并集
            /// </summary>
            R2_MERGENOTPEN = 12,
            /// <summary>
            /// 使用当前的画笔的颜色
            /// </summary>
            R2_COPYPEN = 13,
            /// <summary>
            ///  R2_COPYPEN和R2_NOT的并集
            /// </summary>
            R2_MERGEPENNOT = 14,
            /// <summary>
            /// R2_COPYPEN和屏幕像素值的并集
            /// </summary>
            R2_MERGEPEN = 15,
            /// <summary>
            /// 所有绘制出来的像素为白色
            /// </summary>
            R2_WHITE = 16
        }
        /// <summary>
        /// 将当前绘图位置移动到某个具体的点，同时也可获得之前位置的坐标
        /// LPPOINT lpPoint：传出参数：一个指向POINT结构的指针，用来存放上一个点的位置，若此参数为NULL，则不保存上一个点的位置
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="lppoint"></param>
        /// <returns></returns>
        [DllImport("Gdi32.dll")]

        protected static extern int MoveToEx(IntPtr hdc, int x, int y, IntPtr lppoint);
        /// <summary>
        /// 用当前画笔画一条线，从当前位置连到一个指定的点。这个函数调用完毕，当前位置变成x,y。
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        [DllImport("Gdi32.dll")]

        protected static extern int LineTo(IntPtr hdc, int X, int Y);
        /// <summary>
        /// 用当前画笔画一个实心矩形
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <returns></returns>
        [DllImport("Gdi32.dll")]
        protected static extern int Rectangle(IntPtr hdc, int left, int top, int right, int bottom);

        /// <summary>
        /// 设置指定DC的背景混合模式，背景混合模式用于文本，填充画刷和当画笔不是实线时
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="iBkMode">1透明，2不透明</param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        protected static extern int SetBkMode(IntPtr hdc, int iBkMode);

        protected const int TRANSPARENT = 1;

        protected const int OPAQUE = 2;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="crColor"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]

        protected static extern uint SetTextColor(IntPtr hdc, int crColor);

        protected enum FontWeight : int
        {

            FW_DONTCARE = 0,

            FW_THIN = 100,

            FW_EXTRALIGHT = 200,

            FW_LIGHT = 300,

            FW_NORMAL = 400,

            FW_MEDIUM = 500,

            FW_SEMIBOLD = 600,

            FW_BOLD = 700,

            FW_EXTRABOLD = 800,

            FW_HEAVY = 900,

        }

        protected enum FontCharSet : byte
        {

            ANSI_CHARSET = 0,

            DEFAULT_CHARSET = 1,

            SYMBOL_CHARSET = 2,

            SHIFTJIS_CHARSET = 128,

            HANGEUL_CHARSET = 129,

            HANGUL_CHARSET = 129,

            GB2312_CHARSET = 134,

            CHINESEBIG5_CHARSET = 136,

            OEM_CHARSET = 255,

            JOHAB_CHARSET = 130,

            HEBREW_CHARSET = 177,

            ARABIC_CHARSET = 178,

            GREEK_CHARSET = 161,

            TURKISH_CHARSET = 162,

            VIETNAMESE_CHARSET = 163,

            THAI_CHARSET = 222,

            EASTEUROPE_CHARSET = 238,

            RUSSIAN_CHARSET = 204,

            MAC_CHARSET = 77,

            BALTIC_CHARSET = 186,

        }

        protected enum FontPrecision : byte
        {

            OUT_DEFAULT_PRECIS = 0,

            OUT_STRING_PRECIS = 1,

            OUT_CHARACTER_PRECIS = 2,

            OUT_STROKE_PRECIS = 3,

            OUT_TT_PRECIS = 4,

            OUT_DEVICE_PRECIS = 5,

            OUT_RASTER_PRECIS = 6,

            OUT_TT_ONLY_PRECIS = 7,

            OUT_OUTLINE_PRECIS = 8,

            OUT_SCREEN_OUTLINE_PRECIS = 9,

            OUT_PS_ONLY_PRECIS = 10,

        }
        protected enum FontClipPrecision : byte
        {

            CLIP_DEFAULT_PRECIS = 0,

            CLIP_CHARACTER_PRECIS = 1,

            CLIP_STROKE_PRECIS = 2,

            CLIP_MASK = 0xf,

            CLIP_LH_ANGLES = (1 << 4),

            CLIP_TT_ALWAYS = (2 << 4),

            CLIP_DFA_DISABLE = (4 << 4),

            CLIP_EMBEDDED = (8 << 4),

        }

        protected enum FontQuality : byte
        {

            DEFAULT_QUALITY = 0,

            DRAFT_QUALITY = 1,

            PROOF_QUALITY = 2,

            NONANTIALIASED_QUALITY = 3,

            ANTIALIASED_QUALITY = 4,

            CLEARTYPE_QUALITY = 5,

            CLEARTYPE_NATURAL_QUALITY = 6,

        }

        [Flags]
        protected enum FontPitchAndFamily : byte
        {

            DEFAULT_PITCH = 0,

            FIXED_PITCH = 1,

            VARIABLE_PITCH = 2,

            FF_DONTCARE = (0 << 4),

            FF_ROMAN = (1 << 4),

            FF_SWISS = (2 << 4),

            FF_MODERN = (3 << 4),

            FF_SCRIPT = (4 << 4),

            FF_DECORATIVE = (5 << 4),

        }

        [Flags]
        protected enum dwDTFormat : int
        {

            DT_TOP = 0, DT_LEFT = 0x00000000, DT_CENTER = 0x00000001, DT_RIGHT = 0x00000002,

            DT_VCENTER = 0x00000004, DT_BOTTOM = 0x00000008, DT_WORDBREAK = 0x00000010, DT_SINGLELINE = 0x00000020,

            DT_EXPANDTABS = 0x00000040, DT_TABSTOP = 0x00000080, DT_NOCLIP = 0x00000100, DT_EXTERNALLEADING = 0x00000200,

            DT_CALCRECT = 0x00000400, DT_NOPREFIX = 0x00000800, DT_INTERNAL = 0x00001000

        };
        /// <summary>
        /// 创建一个逻辑字体
        /// 参考https://blog.csdn.net/to_baidu/article/details/54096849
        /// </summary>
        /// <param name="nHeight">指定字体的字符单元或字符的逻辑单位高度</param>
        /// <param name="nWidth">指定所要求字体的字符的逻辑单位的平均宽度。如果此值为0，字体映射器选择一个closest match值，closest match值是由比较当前设备的特征系数与可使用字体的数字化特征系数之差的绝对值而确定的</param>
        /// <param name="nEscapement">指定移位向量和设备X轴之间的一个角度，以十分之一度为单位。移位向量平行于正文行的基线。</param>
        /// <param name="nOrientation">指定每个字符的基线和设备X轴之间的角度。</param>
        /// <param name="fnWeight">指定字体粗细。在0到1000之间指定字体的权值，如400表示标准体，700表示黑（粗）体，如果此值为0，则使用缺省的权值。</param>
        /// <param name="fdwItalic">指定字体是否为斜体。如果设置为TRUE，则字体设置为斜体。</param>
        /// <param name="fdwUnderline">指定字体是否加下划线。如果设置为TRUE，则字体增加下划线。</param>
        /// <param name="fdwStrikeOut">指定字体是否加删除线。如果设置为TRUE，则字体增加删除线。</param>
        /// <param name="fdwCharSet">指定字符集。</param>
        /// <param name="fdwOutputPrecision">指定输出精度，输出精度定义的输出必须密切匹配请求的字体的高度、宽度、字符定位、移位、字符间距和字符类型。</param>
        /// <param name="fdwClipPrecision">指定裁剪精度，裁剪精度定义如何裁剪部分超出裁剪区的字符。</param>
        /// <param name="fdwQuality">指向输出质量，输出质量定义GDI如何仔细地将逻辑字体属性与实际物理字体属性相匹配。</param>
        /// <param name="fdwPitchAndFamily">指定字体间距和系列，低端二位指定字体的字符间距。</param>
        /// <param name="lpszFace">指向指定字体的字样名的、以\0结束的字符串指针，字符串的长度不能超过32个字符（包括字符\0）。如果 lpszFacename 是 NULL，GDI使用与设备无关的字样。</param>
        /// <returns></returns>
        [DllImport("gdi32", EntryPoint = "CreateFontW", CharSet = CharSet.Auto)]
        protected static extern IntPtr CreateFontW(

        [In] Int32 nHeight,

        [In] Int32 nWidth,

        [In] Int32 nEscapement,

        [In] Int32 nOrientation,

        [In] FontWeight fnWeight,

        [In] Boolean fdwItalic,

        [In] Boolean fdwUnderline,

        [In] Boolean fdwStrikeOut,

        [In] FontCharSet fdwCharSet,

        [In] FontPrecision fdwOutputPrecision,

        [In] FontClipPrecision fdwClipPrecision,

        [In] FontQuality fdwQuality,

        [In] FontPitchAndFamily fdwPitchAndFamily,

        [In] String lpszFace);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FontName"></param>
        /// <param name="Height"></param>
        /// <param name="Style"></param>
        /// <returns></returns>
        protected static IntPtr CreatFont(String FontName, Int32 Height, FontStyle Style)

        {

            IntPtr Result;// = IntPtr.Zero; 

            FontWeight boldWeight = FontWeight.FW_NORMAL;

            Boolean Italic = false;

            Boolean Underline = false;

            Boolean Bold = false;

            if ((Style & FontStyle.Bold) != 0)

            {

                Bold = true;

            }

            if ((Style & FontStyle.Italic) != 0)

            {

                Italic = true;

            }

            if ((Style & FontStyle.Underline) != 0)

            {

                Underline = true;

            }

            if (Bold)

            {

                boldWeight = FontWeight.FW_BOLD;

            }

            Result = CreateFontW(Height, 0, 0, 0, boldWeight, Italic, Underline, false,

            FontCharSet.DEFAULT_CHARSET, FontPrecision.OUT_DEFAULT_PRECIS,

            FontClipPrecision.CLIP_DEFAULT_PRECIS, FontQuality.DRAFT_QUALITY,

            FontPitchAndFamily.DEFAULT_PITCH, FontName);

            return Result;

        }
        /// <summary>
        /// 取得被选进指定设备环境的字体名称
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="nCount"></param>
        /// <param name="lpFaceName"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        protected static extern int GetTextFace(IntPtr hdc, int nCount,[Out] StringBuilder lpFaceName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="lpStr"></param>
        /// <param name="nCount"></param>
        /// <param name="lpRect"></param>
        /// <param name="wFormat"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int DrawText(IntPtr hdc, string lpStr, int nCount, ref Rect lpRect, dwDTFormat wFormat);

        protected const Int32 LF_FACESIZE = 32;

        /// <summary>
        /// 用当前选择字符、背景颜色和正文颜色将一个字符串写到指定位置
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="nXStart"></param>
        /// <param name="nYStart"></param>
        /// <param name="lpString"></param>
        /// <param name="cbString">字符串长度</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        protected static extern bool TextOutW(IntPtr hdc, int nXStart, int nYStart,string lpString, int cbString);

        /// <summary>
        /// 绘图所用Graphics关联的设备上下文句柄
        /// </summary>
        protected IntPtr _hdc = IntPtr.Zero;
        /// <summary>
        /// 绘图所用Bitmap的GDI句柄
        /// </summary>
        protected IntPtr _hBitmap = IntPtr.Zero;

        /// <summary>
        /// 绘制区域在bitmap上的左上角坐标x
        /// </summary>
        protected int _x;
        /// <summary>
        /// 绘制区域在bitmap上的左上角坐标y
        /// </summary>
        protected int _y;
        /// <summary>
        /// 绘制区域底边在bitmap上的坐标y值
        /// </summary>
        protected int _bottom;
        /// <summary>
        /// 绘制区域右边在bitmap上的坐标x值
        /// </summary>
        protected int _right;
        /// <summary>
        /// 绘图位图的宽
        /// </summary>
        protected int _width;
        /// <summary>
        /// 绘图位图的高
        /// </summary>
        protected int _height;
        /// <summary>
        /// 绘图区域
        /// </summary>
        protected Rect _r;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Width">绘制区域的宽度</param>
        /// <param name="Height">绘制区域的高度</param>
        /// <param name="hBitmap">绘图所用Bitmap的GDI句柄</param>
        /// <param name="hDC">绘图所用Graphics关联的设备上下文句柄</param>
        public DrawBase(int x, int y, int Width, int Height, IntPtr hBitmap, IntPtr hDC)
        {
            _hdc = hDC;
            _hBitmap = hBitmap;
            _x = x;
            _y = y;
            _width = Width;
            _height = Height;
            _right = _x + _width - 1;
            _bottom = _y + _height - 1;
            _r= new Rect();
            _r.Left = _x;
            _r.Right = _right;
            _r.Top = _y;
            _r.Bottom = _bottom;
            //Color penColor = Color.Black;
            //int penWidth = 0;
            //pen = CreatePen(PenStyle.PS_INSIDEFRAME, penWidth, (int)ColorTranslator.ToWin32(penColor));
            //oldpen = SelectObject(_hdc, pen);

        }

        public DrawBase()
        {

        }
        public int Height
        {
            get
            {
                return _height;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }
        public int Bottom
        {
            get
            {
                return _bottom;
            }
        }

        public int Right
        {
            get
            {
                return _right;
            }
        }

        public int Top
        {
            get
            {
                return _y;
            }
        }

        public int Left
        {
            get
            {
                return _x;
            }
        }

        public Rect Area
        {
            get
            {
                return _r;
            }
        }
        /// <summary>
        /// 根据直线上的两点，计算直线未被形状集合遮盖的部分
        /// </summary>
        /// <param name="maskedrect"></param>
        /// <param name="b"></param>
        /// <param name="e"></param>
        /// <returns>List<Point>[0]是起始点集合，List<Point>[1]是结束点集合。两个集合索引一一对应</returns>
        private List<Point>[] getCanDrawLinePoints(Rectangle[] maskedrect, Point b, Point e)
        {
            Point tmpp;
            List<Point>[] lps = new List<Point>[2];
            //rect的起点，终点，以及和欲绘线的交叉点
            Point pb, pe, pc = new Point();
            float[] abc1 = getLineABC(b, e);
            float[] abc2;
            for (int i = 0; i < maskedrect.Length; i++)
            {
                if (b.Y > e.Y || (b.Y == e.Y && b.X > e.X))
                {
                    tmpp = b;
                    b = e;
                    e = tmpp;
                }
                //上水平线
                pb = new Point(maskedrect[i].X - 1, maskedrect[i].Y - 1);
                pe = new Point(maskedrect[i].X + maskedrect[i].Width + 1, maskedrect[i].Y - 1);
                //重合
                if (pb.Y + 1 == b.Y && pb.Y + 1 == e.Y)
                {
                    lps[0].Add(b);
                    lps[1].Add(new Point(pb.X, pb.Y));
                    lps[0].Add(e);
                    lps[1].Add(new Point(pe.X, pe.Y));
                }
                abc2 = getLineABC(pb, pe);
                pc = crossPoint2Lines(abc1, abc2);
                //交叉
                if (pc.X >= pb.X && pc.X <= pe.X && abc1[0] != 0)
                {
                    lps[0].Add(b);
                    lps[1].Add(new Point(pc.X, pc.Y));
                }
                //下水平线
                pb = new Point(maskedrect[i].X - 1, maskedrect[i].Y + maskedrect[i].Height + 1);
                pe = new Point(maskedrect[i].X + maskedrect[i].Width + 1, maskedrect[i].Y + maskedrect[i].Height + 1);
                //重合
                if (pb.Y - 1 == b.Y && pb.Y - 1 == e.Y)
                {
                    lps[0].Add(b);
                    lps[1].Add(new Point(pb.X, pb.Y));
                    lps[0].Add(e);
                    lps[1].Add(new Point(pe.X, pe.Y));
                }
                abc2 = getLineABC(pb, pe);
                pc = crossPoint2Lines(abc1, abc2);
                //交叉
                if (pc.X >= pb.X && pc.X <= pe.X && abc1[0] != 0)
                {
                    lps[0].Add(e);
                    lps[1].Add(new Point(pc.X, pc.Y));
                }

                if (b.X > e.X || (b.X == e.X && b.Y > e.Y))
                {
                    tmpp = b;
                    b = e;
                    e = tmpp;
                }
                //左垂线
                pb = new Point(maskedrect[i].X - 1, maskedrect[i].Y - 1);
                pe = new Point(maskedrect[i].X - 1, maskedrect[i].Y + maskedrect[i].Height + 1);
                //重合
                if (pb.X + 1 == b.X && pb.X + 1 == e.X)
                {
                    lps[0].Add(b);
                    lps[1].Add(new Point(pb.X, pb.Y));
                    lps[0].Add(e);
                    lps[1].Add(new Point(pe.X, pe.Y));
                }
                abc2 = getLineABC(pb, pe);
                pc = crossPoint2Lines(abc1, abc2);
                //交叉
                if (pc.Y >= pb.Y && pc.Y <= pe.Y && abc1[1] != 0)
                {
                    lps[0].Add(b);
                    lps[1].Add(new Point(pc.X, pc.Y));
                }
                //右垂线
                pb = new Point(maskedrect[i].X + maskedrect[i].Width + 1, maskedrect[i].Y - 1);
                pe = new Point(maskedrect[i].X + maskedrect[i].Width + 1, maskedrect[i].Y + maskedrect[i].Height + 1);
                //重合
                if (pb.X == b.X && pb.X == e.X)
                {
                    lps[0].Add(b);
                    lps[1].Add(new Point(pb.X, pb.Y));
                    lps[0].Add(e);
                    lps[1].Add(new Point(pe.X, pe.Y));
                }
                abc2 = getLineABC(pb, pe);
                pc = crossPoint2Lines(abc1, abc2);
                //交叉
                if (pc.Y >= pb.Y && pc.Y <= pe.Y && abc1[1] != 0)
                {
                    lps[0].Add(e);
                    lps[1].Add(new Point(pc.X, pc.Y));
                }
            }
            return lps;
        }
        /// <summary>
        /// 根据两点，计算直线公式ax+by+c=0中的abc
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private float[] getLineABC(Point p1, Point p2)
        {
            float[] abc = new float[3];

            //x=p1.X的直线
            if (p1.X == p2.X)
            {
                //a
                abc[0] = 1;
                //b
                abc[1] = 0;
                //c
                abc[2] = p1.X;
            }
            //y=p1.Y的直线
            else if (p1.Y == p2.Y)
            {
                abc[0] = 0;
                abc[1] = 1;
                abc[2] = p1.Y;
            }
            else
            {
                abc[0] = p2.Y - p1.Y;
                abc[1] = -1 * (p2.X - p1.X);
                abc[2] = -1 * (p1.X * p2.Y - p2.X * p1.Y);
            }
            return abc;
        }

        /// <summary>
        /// 根据两条直线的abc，算出交叉点
        /// </summary>
        /// <param name="lineabc1"></param>
        /// <param name="lineabc2"></param>
        /// <returns></returns>
        private Point crossPoint2Lines(float[] lineabc1, float[] lineabc2)
        {
            return new Point(Convert.ToInt32(Math.Round((lineabc1[1] * lineabc2[2] - lineabc2[1] * lineabc1[2]) / (lineabc1[0] * lineabc2[1] - lineabc2[0] * lineabc1[1]))), Convert.ToInt32(Math.Round((lineabc1[0] * lineabc2[2] - lineabc2[0] * lineabc1[2]) / (lineabc2[0] * lineabc1[1] - lineabc1[0] * lineabc2[1]))));
        }

    }
}
