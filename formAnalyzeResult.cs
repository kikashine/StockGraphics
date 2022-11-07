using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using StockToolKit.Common;
using System.Security.Cryptography;

namespace StockToolKit.Analyze
{
    public partial class formAnalyzeResult : Form
    {
        private PanelDraw _da;

        private DataKeeper _dkpr;


        private Dictionary<string, THashTable<AnalyzeResult>> _result;
        public formAnalyzeResult()
        {
            InitializeComponent();

            Dictionary<string, THashTable<AnalyzeResult>> rst = new Dictionary<string, THashTable<AnalyzeResult>>();

            _dkpr = new DataKeeper();

            _da = new PanelDraw(ref _dkpr, 1330, 940);
            _da.displayDrawCurrentStockInfo = false;
            _da.displayLabelStockInfo = true;
            _da.init();
            _da.Location = new System.Drawing.Point(460, 5);
            this.Controls.Add(_da);
            _da.DoubleClickKBar += _da_DoubleClickKBar;
            dataGridViewResult.Columns.Add("No.", "No.");
            dataGridViewResult.Columns[0].Width = 30;
            dataGridViewResult.Columns.Add("code", "code");
            dataGridViewResult.Columns[1].Width = 45;
            dataGridViewResult.Columns.Add("index", "i");
            dataGridViewResult.Columns[2].Width = 30;
            dataGridViewResult.Columns.Add("date", "date");
            dataGridViewResult.Columns[3].Width = 70;

            showResult(rst);
            Show();
        }
        //public formAnalyzeResult(ref DataKeeper dataKeeper, formModelManage fmm)
        //{
        //    InitializeComponent();
        //    _fmm = fmm;
        //    _dkpr = dataKeeper;
 
        //    _da = new PanelDraw(ref _dkpr,1330,940);
        //    _da.displayDrawCurrentStockInfo = false;
        //    _da.displayLabelStockInfo = true;
        //    _da.init();
        //    _da.Location = new System.Drawing.Point(460, 5);
        //    this.Controls.Add(_da);
        //    _da.DoubleClickKBar += _da_DoubleClickKBar;
        //    dataGridViewResult.Columns.Add("No.", "No.");
        //    dataGridViewResult.Columns[0].Width = 30;
        //    dataGridViewResult.Columns.Add("code", "code");
        //    dataGridViewResult.Columns[1].Width = 45;
        //    dataGridViewResult.Columns.Add("index", "i");
        //    dataGridViewResult.Columns[2].Width = 30;
        //    dataGridViewResult.Columns.Add("date", "date");
        //    dataGridViewResult.Columns[3].Width = 70;

            
        //}

        public void showResult(Dictionary<string, THashTable<AnalyzeResult>> result)
        {
            _result = result;
            int num = 0;
            int width = 0;
            dataGridViewResult.Visible = false;
            dataGridViewResult.Rows.Clear();
            try
            {
                for (int i = 0; i < result["final"].Count; i++)
                {
                    dataGridViewResult.Rows.Add(new object[4] { i, result["final"][i].StockCode, result["final"][i].Index, _dkpr.getStockDataSet(result["final"][i].StockCode).Date(result["final"][i].Index).ToShortDateString() });
                }
               
            }
            catch (Exception err)
            {

            }
            dataGridViewResult.Visible = true;
            //foreach (AnalyzeResult ar in result["final"])
            //{
            //    num++;
            //    dataGridViewResult.Rows.Add(new object[4] { num, ar.StockCode, ar.Index, _dkpr.getStockDataSet(ar.StockCode).Date(ar.Index).ToShortDateString() });
            //}


            //for (int i = 0; i < dataGridViewResult.Columns.Count; i++)
            //{
            //将每一列都调整为自动适应模式
            //dataGridViewResult.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);

            //记录整个DataGridView的宽度
            //width += dataGridViewResult.Columns[i].Width;
            //}
            //判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，
            //则将DataGridView的列自动调整模式设置为显示的列即可，
            //如果是小于原来设定的宽度，将模式改为填充。
            //if (width > dataGridViewResult.Size.Width)
            //{
            //dataGridViewResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            //}
            //else
            //{
            //dataGridViewResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //}

        }
        private void _da_DoubleClickKBar(EventArgs e, string stockcode, int index)
        {
            //_fmm.setStockcodeAndIndexForRun(stockcode, index);
            //this.Hide();
        }

        private void formAnalyzeResult_Shown(object sender, EventArgs e)
        {
            //da.DrawGraphic();
        }

        private void formAnalyzeResult_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void formAnalyzeResult_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewResult_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            string[] types = new string[_result.Keys.Count];
            int i = 0;
            foreach (string key in _result.Keys)
            {
                types[i] = key;
                i++;
            }
            //_da.dr
            _da.drawDataline(_result["final"][e.RowIndex].Index, _dkpr.getStockDataSet(_result["final"][e.RowIndex].StockCode), _result, types);
        }
    }
}
