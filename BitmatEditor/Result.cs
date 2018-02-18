using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BitmatEditor
{
	public partial class Result : Form
	{
		List<UInt32> data;
		int Row;
		int Col;
		int BitsPerColor;

		public Result(List<UInt32> list, int row, int col, int bpc)
		{
			InitializeComponent();

			data = list;
			Row = row;
			Col = col;
			BitsPerColor = bpc;

			MakeString();
		}

		public void MakeString()
		{
			int r_cnt = 0;
			int c_cnt = 0;
			int col_byte = (int)Math.Round(((float)(BitsPerColor * Col) / 8) + 0.5);
			for (r_cnt = 0; r_cnt < Row; r_cnt++)
			{
				string strRow = "";
				for (c_cnt = 0; c_cnt < col_byte; c_cnt++)
				{
					UInt32 d = data.ElementAt(r_cnt * col_byte + c_cnt);
					strRow += "0x" + d.ToString("X2") + ", ";
				}
				strRow += "\n";
				rtbResult.AppendText(strRow);
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();

			if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string path = Path.Combine(dialog.SelectedPath, "test.txt");
				//string path = "test.txt";
				FileStream stream;
				if(!File.Exists(path))
				{
					try
					{
						File.Create(path, 1024).Close();
					}
					catch(FileNotFoundException em)
					{
						MessageBox.Show(string.Format("Access Denied to [{0}]", dialog.SelectedPath), "Access Denied");
						return;
					}
				}
				stream = File.Open(path, FileMode.Open);

				rtbResult.SaveFile(stream, RichTextBoxStreamType.PlainText);
				stream.Close();
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
			this.Dispose();
		}
	}
}
