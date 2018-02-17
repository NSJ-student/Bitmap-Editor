using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitmatEditor
{
	public partial class Result : Form
	{
		List<UInt32> data;
		int Row;
		int Col;

		public Result(List<UInt32> list, int row, int col)
		{
			InitializeComponent();

			data = list;
			Row = row;
			Col = col;

			int r_cnt = 0;
			int c_cnt = 0;
			int col_byte = (int)Math.Round(((float)(2 * Col) / 8) + 0.5);
			for (r_cnt = 0; r_cnt < Row; r_cnt++)
			{
				string strRow = "";
				for (c_cnt = 0; c_cnt < col_byte; c_cnt++)
				{
					UInt32 d = data.ElementAt(r_cnt * col_byte + c_cnt);
					strRow += "0x" + d.ToString("X2") + " ";
				}
				strRow += "\n";
				rtbResult.AppendText(strRow);
			}
		}
	}
}
