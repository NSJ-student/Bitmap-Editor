using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BitmatEditor
{
    class Dot
    {
        int RowVal;
		int ColVal;
        Color? FillColor;
        bool bFocus;

		public bool Selected
		{
			get
			{
				return bFocus;
			}
			set
			{
				bFocus = value;
			}
		}
		public int Row
		{
			get
			{
				return RowVal;
			}
		}
		public int Column
		{
			get
			{
				return ColVal;
			}
		}
		public Color? BackColor
		{
			get
			{
				return FillColor;
			}
		}
		public Dot(int row, int col, Color? color)
        {
			RowVal = row;
			ColVal = col;
            FillColor = color;
            bFocus = false;
        }
        public void SetColor(Color? color)
        {
            FillColor = color;
        }
		public void ToggleFocus()
		{
			if (bFocus) bFocus = false;
			else bFocus = true;
		}
        public void Draw(PaintEventArgs e, RectangleF area)
		{
			RectangleF rect = new RectangleF(
				(area.X + area.Width * ColVal), 
				(area.Y + area.Height * Row),
				area.Width, 
				area.Height );
			Color color = (FillColor != null) ? (Color)FillColor : (Color)SystemColors.Control;

			if (bFocus)
			{
				HatchBrush brush = new HatchBrush(HatchStyle.DottedDiamond, Color.Black, color);
				e.Graphics.FillRectangle(brush, rect);
			}
			else
			{
				Brush brush = new SolidBrush(color);
				e.Graphics.FillRectangle(brush, rect);
			}

			Pen pen;
			pen = new Pen(Color.Black);
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
			e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
    }

    class DotMatrix
    {
		public delegate UInt32 ColorToBits(Color? color);
		List<List<Dot>> DotMat;
		int Row;
		int Col;

		public DotMatrix(int row, int col)
        {
			int r_cnt = 0;
			int c_cnt = 0;

			DotMat = new List<List<Dot>>();
			for (r_cnt = 0; r_cnt < row; r_cnt++ )
			{
				DotMat.Add(new List<Dot>());
				for(c_cnt=0; c_cnt<col; c_cnt++)
				{
					List<Dot> item = DotMat[r_cnt];
					DotMat[r_cnt].Add(new Dot(r_cnt, c_cnt, null));
				}
			}
			Row = row;
			Col = col;
		}

		public void Draw(PaintEventArgs e, RectangleF area)
		{
			int r_cnt = 0;
			int c_cnt = 0;
			RectangleF areaF = new RectangleF(area.X, area.Y, area.Width / Col, area.Height / Row);

			for (r_cnt = 0; r_cnt < Row; r_cnt++)
			{
				for (c_cnt = 0; c_cnt < Col; c_cnt++)
				{
					List<Dot> item = DotMat[r_cnt];
					item[c_cnt].Draw(e, areaF);
				}
			}
		}

		public Dot SelectDot(RectangleF matrixArea, int x, int y)
		{
			int r_cnt = 0;
			int c_cnt = 0;

			for (r_cnt = 0; r_cnt < Row; r_cnt++ )
			{
				for(c_cnt=0; c_cnt<Col; c_cnt++)
				{
					if (x < 20) continue;
					if (y < 20) continue;
					if((x < matrixArea.X + matrixArea.Width * (c_cnt + 1) / Col) &&
					   (y < matrixArea.Y + matrixArea.Height * (r_cnt + 1) / Row))
					{
						List<Dot> item = DotMat[r_cnt];
						return item[c_cnt];
					}

				}
			}

			return null;
		}

		public bool IsColorExists(Color color)
		{
			int r_cnt = 0;
			int c_cnt = 0;

			for (r_cnt = 0; r_cnt < Row; r_cnt++)
			{
				for (c_cnt = 0; c_cnt < Col; c_cnt++)
				{
					List<Dot> item = DotMat[r_cnt];
					if((Color)item[c_cnt].BackColor == color)
					{
						return true;
					}

				}
			}

			return false;
		}

		public void FillAreaColor(Dot dot, Color? orgColor, Color? color)
		{
			int curRow = dot.Row;
			int curCol = dot.Column;

			if (orgColor == color)
				return;

			if(curRow > 0)
			{
				Dot upper = DotMat[curRow - 1][curCol];
				if(upper.BackColor == orgColor)
				{
					dot.SetColor(color);
					FillAreaColor(upper, orgColor, color);
				}
			}
			if (curCol > 0)
			{
				Dot left = DotMat[curRow][curCol - 1];
				if (left.BackColor == orgColor)
				{
					dot.SetColor(color);
					FillAreaColor(left, orgColor, color);
				}
			}
			if (curRow < Row-1)
			{
				Dot lower = DotMat[curRow + 1][curCol];
				if (lower.BackColor == orgColor)
				{
					dot.SetColor(color);
					FillAreaColor(lower, orgColor, color);
				}
			}
			if (curCol < Col - 1)
			{
				Dot right = DotMat[curRow][curCol + 1];
				if (right.BackColor == orgColor)
				{
					dot.SetColor(color);
					FillAreaColor(right, orgColor, color);
				}
			}
			if (dot.BackColor == orgColor)
			{
				dot.SetColor(color);
			}
		}

		public List<UInt32> ToBitmapArray(ColorToBits ColorToBit)
		{
			List<UInt32> list = new List<UInt32>();
			int r_cnt = 0;
			int c_cnt = 0;
			int col_byte = (2 * Col) / 8;

			for (r_cnt = 0; r_cnt < Row; r_cnt++)
			{
				List<Dot> item = DotMat[r_cnt];
				for (c_cnt = 0; c_cnt < col_byte; c_cnt++)
				{
					UInt32 dot1 = ColorToBit(item[4 * c_cnt + 0].BackColor);
					UInt32 dot2 = ColorToBit(item[4 * c_cnt + 1].BackColor);
					UInt32 dot3 = ColorToBit(item[4 * c_cnt + 2].BackColor);
					UInt32 dot4 = ColorToBit(item[4 * c_cnt + 3].BackColor);

					UInt32 result = (dot1 << 6) | (dot2 << 4) | (dot3 << 2) | (dot4);

					list.Add(result);
				}

				int rest = Col % 4;
				if(rest > 0)
				{
					UInt32 result = 0;
					for (int temp = 0; temp < rest; temp++ )
					{
						UInt32 dot = ColorToBit(item[4 * col_byte + temp].BackColor);
						result |= dot << ( 2 * (3-temp) );
					}
					list.Add(result);
				}
			}

			return list;
		}

    }
}
