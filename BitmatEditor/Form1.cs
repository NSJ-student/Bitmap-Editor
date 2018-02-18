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
    public partial class Form1 : Form
    {
		string[] colorSystem = {
			"565RGB"  
		};
		DotMatrix matrix;
		Color? SelectedColor;
		RectangleF MatrixArea;
		Dot SelectedDot;
		Dot MouseDownDot;
		bool bDrag = false;
		bool bCtrlPressed = false;

        public Form1()
        {
            InitializeComponent();
			matrix = new DotMatrix(1, 1);

			cbColor.DataSource = colorSystem;
			pDiaplay.Width = 2 * gbControl.Location.X + gbControl.Width;
			SelectedColor = null;
			SelectedDot = null;
			MouseDownDot = null;
			MatrixArea = new RectangleF(20, 20,
				this.ClientSize.Width - pDiaplay.Width - 50,
				this.ClientSize.Height - 50);
			tlpProperty.Visible = false;
			nudBitsPerColor_ValueChanged(null, null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void nudRowCnt_KeyUp(object sender, KeyEventArgs e)
		{
			matrix = new DotMatrix((int)nudRowCnt.Value, (int)nudColCnt.Value);
			Invalidate(true);
        }

        private void nudColCnt_KeyUp(object sender, KeyEventArgs e)
		{
			matrix = new DotMatrix((int)nudRowCnt.Value, (int)nudColCnt.Value);
			Invalidate(true);
        }

        private void nudRowCnt_ValueChanged(object sender, EventArgs e)
		{
			matrix = new DotMatrix((int)nudRowCnt.Value, (int)nudColCnt.Value);
			Invalidate(true);
        }

        private void nudColCnt_ValueChanged(object sender, EventArgs e)
		{
			matrix = new DotMatrix((int)nudRowCnt.Value, (int)nudColCnt.Value);
			Invalidate(true);
        }

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			Brush brush = new SolidBrush(Color.White);
			e.Graphics.FillRectangle(brush, MatrixArea);
			matrix.Draw(e, MatrixArea);
		}

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			MatrixArea.Size = new Size(this.ClientSize.Width - pDiaplay.Width - 50, this.ClientSize.Height - 50);
			Invalidate(true);
		}

		private void btnColor_Click(object sender, EventArgs e)
		{
			ColorDialog cDialog = new ColorDialog();
			DialogResult res = cDialog.ShowDialog();
			if (res == System.Windows.Forms.DialogResult.OK)
			{
				btnColorValue.BackColor = cDialog.Color;
				btnColorValue.Text = (cDialog.Color.ToArgb() & 0xFFFFFF).ToString("X");
				if (SelectedDot != null)
					SelectedDot.SetColor(btnColorValue.BackColor);
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			List<UInt32> list = matrix.ToBitmapArray(ColorToBit, (int)nudBitsPerColor.Value);
			Result res = new Result(list, (int)nudRowCnt.Value, (int)nudColCnt.Value, (int)nudBitsPerColor.Value);
			res.Show();
		}

		private bool IsListColorExist(Color color)
		{
			int cnt;

			for (cnt = 0; cnt < lvColorList.Items.Count; cnt++)
			{
				ListViewItem item = lvColorList.Items[cnt];
				if (item.SubItems[2].BackColor == color)
					return true;
			}

			return false;
		}
		private ListViewItem SearchListViewItem(Color color)
		{
			for (int cnt = 0; cnt < lvColorList.Items.Count; cnt++)
			{
				ListViewItem item = lvColorList.Items[cnt];
				if (item.SubItems[2].BackColor == color)
					return item;
			}

			return null;
		}
		private ListViewItem SearchListViewItem(int index)
		{
			for (int cnt = 0; cnt < lvColorList.Items.Count; cnt++)
			{
				ListViewItem item = lvColorList.Items[cnt];
				if (item.SubItems[0].Text.Equals(index.ToString()))
					return item;
			}

			return null;
		}

		private void lvColorList_DoubleClick(object sender, EventArgs e)
		{
			ColorDialog cDialog = new ColorDialog();
			DialogResult res = cDialog.ShowDialog();
			if (res == System.Windows.Forms.DialogResult.OK)
			{
				SelectedColor = cDialog.Color;
				ListViewItemChangeColor(lvColorList.SelectedItems[0], cDialog.Color);
			}
		}

		private void cbFillAreaColor_CheckedChanged(object sender, EventArgs e)
		{
			if (cbFillAreaColor.Checked)
			{
				if (cbBrushColor.Checked)
					cbBrushColor.Checked = false;
			}
		}
		private void cbFillColor_CheckedChanged(object sender, EventArgs e)
		{
			if(cbBrushColor.Checked)
			{
				if (cbFillAreaColor.Checked)
					cbFillAreaColor.Checked = false;
			}
		}

		private void nudBitsPerColor_ValueChanged(object sender, EventArgs e)
		{
			int curCnt = lvColorList.Items.Count;
			int targetCnt = (int)Math.Pow(2, (double)nudBitsPerColor.Value);

			if (curCnt < targetCnt)
			{
				for(int cnt=0; cnt<(targetCnt-curCnt); cnt++)
				{
					AddColorList(Color.Black);
				}
			}

			if (curCnt > targetCnt)
			{
				for (int cnt = 0; cnt < (curCnt - targetCnt); cnt++)
				{
					lvColorList.Items.RemoveAt(lvColorList.Items.Count-1);
				}
			}
		}

		private bool AddColorList(Color? color)
		{
			if (color == null)
				return false;
			//if (IsListColorExist((Color)color) == true)
			//	return false;

			ListViewItem item = new ListViewItem();
			item.UseItemStyleForSubItems = false;
			item.SubItems[0] = new ListViewItem.ListViewSubItem(item,
				lvColorList.Items.Count.ToString());
			item.SubItems.Add(new ListViewItem.ListViewSubItem(item,
				(((Color)color).ToArgb() & 0xFFFFFF).ToString("X6")));
			item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "", (Color)color, (Color)color, null));
			lvColorList.Items.Add(item);
			return true;
		}
		private void ListViewItemChangeColor(ListViewItem item, Color color)
		{
			item.UseItemStyleForSubItems = false;
			item.SubItems[1].Text = (color.ToArgb() & 0xFFFFFF).ToString("X6");
			item.SubItems[2].BackColor = color;
		}

		private void lvColorList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(lvColorList.SelectedItems.Count > 0)
			{
				ListViewItem item = lvColorList.SelectedItems[0];
				SelectedColor = item.SubItems[2].BackColor;
				lbSelectedColor.Text = item.SubItems[0].Text;
			}
			else
			{
				SelectedColor = null;
				lbSelectedColor.Text = "N";
			}
		}

		private UInt32 ColorToBit(Color? color)
		{
			if (color == null)
				return 0;

			ListViewItem item = SearchListViewItem((Color)color);
			return UInt32.Parse(item.SubItems[0].Text);
		}

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			Dot selectedItem = matrix.SelectDot(MatrixArea, e.X, e.Y);
			if (selectedItem != null)
			{
				MouseDownDot = selectedItem;
				if (cbBrushColor.Checked && bCtrlPressed)
				{
					if (selectedItem.BackColor != SelectedColor)
					{
						selectedItem.SetColor(SelectedColor);
						bDrag = true;
						Rectangle rect = new Rectangle(
							(int)Math.Ceiling(MatrixArea.X),
							(int)Math.Ceiling(MatrixArea.Y),
							(int)Math.Ceiling(MatrixArea.Width),
							(int)Math.Ceiling(MatrixArea.Height)
							);
						Invalidate(rect, true);
					}
				}
			}
		}

		private void Form1_MouseMove(object sender, MouseEventArgs e)
		{
			if (cbBrushColor.Checked && bDrag)
			{
				Dot selectedItem = matrix.SelectDot(MatrixArea, e.X, e.Y);
				if (selectedItem != null)
				{
					if(selectedItem.BackColor != SelectedColor)
					{
						selectedItem.SetColor(SelectedColor);
						bDrag = true;
						Rectangle rect = new Rectangle(
							(int)Math.Ceiling(MatrixArea.X),
							(int)Math.Ceiling(MatrixArea.Y),
							(int)Math.Ceiling(MatrixArea.Width),
							(int)Math.Ceiling(MatrixArea.Height)
							);
						Invalidate(rect, true);
					}
				}
			}
		}
		private void Form1_MouseUp(object sender, MouseEventArgs e)
		{
			if(bDrag)
			{
				bDrag = false;
				Dot item = matrix.SelectDot(MatrixArea, e.X, e.Y);
				if (item != null)
				{
					if (cbBrushColor.Checked)
					{
						if (item.BackColor != SelectedColor)
						{
							item.SetColor(SelectedColor);
							Rectangle rect = new Rectangle(
								(int)Math.Ceiling(MatrixArea.X),
								(int)Math.Ceiling(MatrixArea.Y),
								(int)Math.Ceiling(MatrixArea.Width),
								(int)Math.Ceiling(MatrixArea.Height)
								);
							Invalidate(rect, true);
						}
					}
				}
				return;
			}


			Dot selectedItem = matrix.SelectDot(MatrixArea, e.X, e.Y);
			if (selectedItem != null)
			{
				if(selectedItem != MouseDownDot)
				{
					return;
				}
				selectedItem.ToggleFocus();

				if (selectedItem.Selected)
				{
					if (SelectedDot != null)
						SelectedDot.Selected = false;
					SelectedDot = selectedItem;

					if (SelectedColor != null)
					{
						if (cbBrushColor.Checked)
						{
							SelectedDot.SetColor(SelectedColor);
						}
						else if (cbFillAreaColor.Checked)
						{
							matrix.FillAreaColor(SelectedDot, SelectedDot.BackColor, SelectedColor);
						}
					}

					tlpProperty.Visible = true;
					lbRowValue.Text = selectedItem.Row.ToString();
					lbColValue.Text = selectedItem.Column.ToString();
					if (selectedItem.BackColor != null)
					{
						btnColorValue.BackColor = (Color)selectedItem.BackColor;
						btnColorValue.Text = (((Color)selectedItem.BackColor).ToArgb() & 0xFFFFFF).ToString("X");
					}
					else
					{
						btnColorValue.BackColor = (Color)SystemColors.Control;
						btnColorValue.Text = "";
					}
				}
				else
				{
					if (SelectedDot != null)
					{
						if (cbBrushColor.Checked)
						{
							SelectedDot.SetColor(null);
						}
						SelectedDot = null;
					}
					tlpProperty.Visible = false;
				}

				Rectangle rect = new Rectangle(
					(int)Math.Ceiling(MatrixArea.X),
					(int)Math.Ceiling(MatrixArea.Y),
					(int)Math.Ceiling(MatrixArea.Width),
					(int)Math.Ceiling(MatrixArea.Height)
					);
				Invalidate(rect, true);
			}
			else
			{
				if (SelectedDot != null)
				{
					SelectedDot.Selected = false;
					SelectedDot = null;
					tlpProperty.Visible = false;

					Rectangle rect = new Rectangle(
						(int)Math.Ceiling(MatrixArea.X),
						(int)Math.Ceiling(MatrixArea.Y),
						(int)Math.Ceiling(MatrixArea.Width),
						(int)Math.Ceiling(MatrixArea.Height)
						);
					Invalidate(rect, true);
				}
			}
		}

		private void Form1_MouseLeave(object sender, EventArgs e)
		{
			if (bDrag)
			{
				bDrag = false;
			}
			if(bCtrlPressed)
			{
				bCtrlPressed = false;
			}
		}

		private void Form1_KeyUp(object sender, KeyEventArgs e)
		{
			if ((e.KeyData & Keys.Control) != 0)
			{
				bCtrlPressed = true;
			}
			else
			{
				bCtrlPressed = false;
			}
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyData & Keys.Control) != 0)
			{
				bCtrlPressed = true;
			}
			else
			{
				bCtrlPressed = false;
			}
		}
	}
}
