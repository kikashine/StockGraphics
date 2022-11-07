using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace StockToolKit.Common
{
    public class Utility
    {
        public static string DBPath1 = @"c:\temp\StockAT\dbfile";
        public static string DBPath2 = @".\dbfile";
        public static string DBPath3 = @"C:\StockAT\dbfilecq";
        public static string DBPath4 = @".\dbfilecq";
        public static string DBPath5 = @"C:\StockAT\dbfile\qx";
        public static string DBPath6 = @".\dbfile\qx";

        public static void log(string msg)
        {
            //临时屏蔽，需要改进为对log队列的操作，防止文件写入冲突。
            return;
            if (File.Exists(@".\log.txt"))
            {
                FileStream fs = new FileStream(@".\log.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sr = new StreamWriter(fs);
                //sr.WriteLine(DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "   " + msg);
                sr.WriteLine(msg);
                sr.Close();
                fs.Close();
            }
            else
            {
                FileStream fs = new FileStream(@".\log.txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sr = new StreamWriter(fs);
                //sr.WriteLine(DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "   " + msg);
                sr.WriteLine(msg);
                sr.Close();
                fs.Close();
            }
        }

        //public static Dictionary<string, StockInfo> GetStockList(bool UseARListFile, string filename, string Date)
        //{
        //    string strSRLine = "";
        //    //Hashtable stocklisttmp = GetStockList();
        //    Hashtable stocklist = new Hashtable();
        //    //string[] codelist;
        //    if (UseARListFile)
        //    {
        //        if (!File.Exists(filename))
        //        {
        //            //stocklist = stocklisttmp;
        //            return GetStockList();
        //        }
        //        using (StreamReader SReader = File.OpenText(filename))
        //        {
        //            while (!SReader.EndOfStream)
        //            {
        //                strSRLine = SReader.ReadLine();
        //                if (strSRLine.IndexOf("<row>") >= 0)
        //                {
        //                    Hashtable stock = new Hashtable();
        //                    strSRLine = SReader.ReadLine();
        //                    //int sstart = 0;
        //                    //int send = 0;
        //                    while (strSRLine.IndexOf("</row>") < 0)
        //                    {

        //                        if (strSRLine.IndexOf("<arfd.StockCode>") >= 0)
        //                        {
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "</?[^>]+>", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\t", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\r", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\n", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, " ", "");
        //                            stock.Add("StockCode", strSRLine);
        //                        }
        //                        else if (strSRLine.IndexOf("<arfd.MarketCode>") >= 0)
        //                        {
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "</?[^>]+>", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\t", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\r", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\n", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, " ", "");
        //                            stock.Add("MarketCode", strSRLine);
        //                        }
        //                        else if (strSRLine.IndexOf("<arfd.AnalyzeDay>") >= 0)
        //                        {
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "</?[^>]+>", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\t", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\r", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\n", "");
        //                            //strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, " ", "");
        //                            stock.Add("AnalyzeDay", strSRLine);
        //                        }
        //                        else if (strSRLine.IndexOf("<arfd.DataSource>") >= 0)
        //                        {
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "</?[^>]+>", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\t", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\r", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\n", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, " ", "");
        //                            stock.Add("DataSource", strSRLine);
        //                        }
        //                        else if (strSRLine.IndexOf("<arfd.Industry>") >= 0)
        //                        {
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "</?[^>]+>", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\t", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\r", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, "\n", "");
        //                            strSRLine = System.Text.RegularExpressions.Regex.Replace(strSRLine, " ", "");
        //                            stock.Add("Industry", strSRLine);
        //                        }
        //                        strSRLine = SReader.ReadLine();
        //                    }
        //                    if (!stocklist.ContainsKey(stock["StockCode"].ToString()))
        //                    {
        //                        stocklist.Add(stock["StockCode"].ToString(), new List<Hashtable>());
        //                    }
        //                    ((List<Hashtable>)stocklist[stock["StockCode"].ToString()]).Add(stock);
        //                    continue;
        //                }

        //            }
        //            SReader.Close();
        //        }

        //    }
        //    else
        //    {
        //        return GetStockList();
        //    }
        //    if (stocklist.Count == 0)
        //    {
        //        //stocklist = stocklisttmp;
        //    }
        //    return stocklist;
        //}
        public static Dictionary<string, StockInfo> GetStockList()
        {
            //return GetStockList_StockBlock();
            //return GetStockList_industry1();
            return GetStockList_industry2();
        }
        /// <summary>
        /// 读取股票列表
        /// 使用industry.ini文件
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, StockInfo> GetStockList_industry1()
        {
            Dictionary<string, StockInfo> stockdict = new Dictionary<string, StockInfo>();
            //Hashtable stocklist = new Hashtable();
            Hashtable stockListtmp = new Hashtable();
            Hashtable name = new Hashtable();
            //Hashtable unit;
            string[] cl;
            string strSRLine = "";
            if (!File.Exists(@".\industry.ini"))
            {
                return stockdict;
            }

            //读取股票和板块代码
            using (StreamReader SReader = new StreamReader(@".\industry.ini", Encoding.Default))
            {
                while (true)
                {
                    strSRLine = SReader.ReadLine();
                    if (strSRLine == "[industry]")
                    {
                        break;
                    }
                }
                while (true)
                {
                    strSRLine = SReader.ReadLine();
                    if (strSRLine != "" && strSRLine != "[name]")
                    {
                        stockListtmp.Add(strSRLine.Split('=')[0], strSRLine.Split('=')[1]);
                    }
                    else if (strSRLine == "[name]")
                    {
                        break;
                    }
                }
                while (true)
                {
                    strSRLine = SReader.ReadLine();
                    if (strSRLine != "" && strSRLine != "[hash]")
                    {
                        name.Add(strSRLine.Split(';')[0].Split('=')[0], strSRLine.Split(';')[0].Split('=')[1]);
                    }
                    else if (strSRLine == "[hash]")
                    {
                        break;
                    }
                }
            }
            string ind = "";
            string mkt = "";
            string stk = "";
            foreach (string key in stockListtmp.Keys)
            {
                cl = stockListtmp[key].ToString().Split(',');
                for (int i = 0; i < cl.Length; i++)
                {

                    //unit = new Hashtable();
                    //if (stocklist.ContainsKey(cl[i]))
                    //{

                       // unit = (Hashtable)stocklist[cl[i]];
                        //unit["Industry"] = unit["Industry"] + "|" + name[key];
                        //continue;
                    //}
                    if(stockdict.ContainsKey(cl[i]))
                    {
                        continue;
                    }
                    stk = cl[i];

                    //unit.Add("StockCode", cl[i]);
                    if (cl[i].StartsWith("0") || cl[i].StartsWith("3"))
                    {
                        mkt = "she";
                        //unit.Add("MarketCode", "she");
                    }
                    else if (cl[i].StartsWith("6"))
                    {
                        mkt = "sha";
                        //unit.Add("MarketCode", "sha");
                    }
                    else
                    {
                        continue;
                    }
                    if (name.ContainsKey(key))
                    {
                        ind = (string)name[key];
                        //unit.Add("Industry", name[key]);
                    }
                    else
                    {
                        ind = "null";
                        //unit.Add("Industry", "null");
                    }
                    stockdict.Add(stk, new StockInfo(stk, mkt, ind));

                    //stocklist.Add(cl[i], unit);
                }
            }

            return stockdict;
        }
        public static Dictionary<string, StockInfo> GetStockList_industry2()
        {
            Dictionary<string, StockInfo> stockdict = new Dictionary<string, StockInfo>();
            //Hashtable stocklist = new Hashtable();
            Hashtable stockListtmp = new Hashtable();
            Hashtable name = new Hashtable();
            //Hashtable unit;
            string[] cl;
            string strSRLine = "";
            string strtmpcl = "";
            int tmpindexof = -1;
            string[] strst;
            if (!File.Exists(@".\block_content_industry.ini") || !File.Exists(@".\block_tree.ini"))
            {
                //return stocklist;
                return stockdict;
            }

            //读取股票和板块代码
            using (StreamReader SReader = new StreamReader(@".\block_content_industry.ini", Encoding.Default))
            {
                while (SReader.Peek() > -1)
                {
                    strSRLine = SReader.ReadLine();
                    if (strSRLine == "DFF8=@")
                    {
                        break;
                    }
                }
                while (SReader.Peek() > -1)
                {
                    strSRLine = SReader.ReadLine();
                    if (strSRLine != "")
                    {
                        strtmpcl = strSRLine.Split('=')[1];
                        if(strtmpcl.EndsWith(","))
                        {
                            strtmpcl = strtmpcl.Substring(0, strtmpcl.Length - 1);
                        }
                        stockListtmp.Add(strSRLine.Split('=')[0], strtmpcl);
                    }
                }
            }

            using (StreamReader SReader = new StreamReader(@".\block_tree.ini", Encoding.Default))
            {
                while (SReader.Peek() > -1)
                {
                    strSRLine = SReader.ReadLine();
                    if (strSRLine == "[block_tree/1/DFF8]")
                    {
                        break;
                    }
                }
                while (SReader.Peek() > -1)
                {
                    strSRLine = SReader.ReadLine();
                    tmpindexof = strSRLine.IndexOf("[block_tree");
                    if (strSRLine != "" && tmpindexof == -1)
                    {
                        name.Add(strSRLine.Split('=')[0], strSRLine.Split('=')[1]);
                    }
                    else if (tmpindexof > - 1)
                    {
                        break;
                    }
                }
            }
            string ind = "";
            string mkt = "";
            string stk = "";
            foreach (string key in stockListtmp.Keys)
            {
                cl = stockListtmp[key].ToString().Split(',');
                for (int i = 0; i < cl.Length; i++)
                {
                    strst = cl[i].Split(':');
                    //unit = new Hashtable();
                    //if (stocklist.ContainsKey(strst[1]))
                    //{
                    //    unit = (Hashtable)stocklist[strst[1]];
                    //    unit["Industry"] = unit["Industry"] + "|" + name[key];
                    //    continue;
                    //}
                    if (stockdict.ContainsKey(strst[1]))
                    {
                        stockdict[strst[1]].Industry = stockdict[strst[1]].Industry + "|" + name[key];
                        continue;
                    }
                    stk = strst[1];
                    //unit.Add("StockCode", strst[1]);
                    if (strst[0] == "17" || strst[0] == "22")
                    {
                        mkt = "sha";
                        //unit.Add("MarketCode", "sha");
                    }
                    else if (strst[0] == "33")
                    {
                        mkt = "she";
                        //unit.Add("MarketCode", "she");
                    }
                    else
                    {
                        continue;
                    }
                    if (name.ContainsKey(key))
                    {
                        ind = (string)name[key];
                        //unit.Add("Industry", name[key]);
                    }
                    else
                    {
                        ind = null;
                        //unit.Add("Industry", "null");
                    }
                    //stocklist.Add(strst[1], unit);
                    stockdict.Add(stk, new StockInfo(stk, mkt, ind));
                }
            }

            return stockdict;
        }
            public static string toDBDate(DateTime dt)
        {
            //string DateStart = dt.Year + "-";
            //if (dt.Month < 10)
            //{
            //    DateStart += "0";
            //}
            //DateStart += dt.Month + "-";
            //if (dt.Day < 10)
            //{
            //    DateStart += "0";
            //}
            //DateStart += dt.Day + " 00:00:00";
            //return DateStart;
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string toDBDate(string dtstr)
        {
            //return "";
            char splitsyb = ' ';
            if (dtstr.IndexOf('/') > 0)
            {
                splitsyb = '/';
            }
            else
            {
                splitsyb = '-';
            }
            string tmp = dtstr.Split(' ')[0];
            DateTime dt = new DateTime(Convert.ToInt32(tmp.Split(splitsyb)[0]), Convert.ToInt32(tmp.Split(splitsyb)[1]), Convert.ToInt32(tmp.Split(splitsyb)[2]));
            return toDBDate(dt);
        }

        /// <summary>
        /// 得到k线数据最近更新时间
        /// </summary>
        /// <param name="date"></param>
        /// <param name="Source"></param>
        /// <returns></returns>
        //public static string getDataUpdate(int Source)
        //{
        //    DBWorkerCheckNew dbw = new DBWorkerCheckNew();
        //    string SSQL = "select * from D1";
        //    DataTable dt = new DataTable();
        //    dbw.Select(SSQL, dt, Source);

        //    if (dt.Rows.Count == 0)
        //    {
        //        return "1970-01-01 00:00:00";
        //    }
        //    else
        //    {
        //        return dt.Rows[0][0].ToString();
        //    }
        //}
        ///// <summary>
        ///// 更新新k线数据导入的时间
        ///// </summary>
        ///// <param name="date"></param>
        ///// <param name="Source"></param>
        ///// <returns></returns>
        //public static bool setDataUpdate(DateTime date, int Source)
        //{

        //    DBWorkerCheckNew dbw = new DBWorkerCheckNew();
        //    if (dbw.writeDB(date, Source))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public static Hashtable GetStockht(string stockcode)
        {
            Hashtable unit = new Hashtable();
            unit.Add("StockCode", stockcode);
            if (stockcode.StartsWith("0") || stockcode.StartsWith("3"))
            {
                unit.Add("MarketCode", "she");
            }
            else if (stockcode.StartsWith("6"))
            {
                unit.Add("MarketCode", "sha");
            }
            return unit;
        }
    }
}
