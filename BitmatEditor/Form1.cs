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
		Color? SetColor;
		RectangleF MatrixArea;
		Dot SelectedDot;
        public Form1()
        {
            InitializeComponent();
			matrix = new DotMatrix(1, 1);

			cbColor.DataSource = colorSystem;
			pDiaplay.Width = 2 * gbControl.Location.X + gbControl.Width;
			SetColor = null;
			SelectedDot = null;
			MatrixArea = new RectangleF(20, 20,
				this.ClientSize.Width - pDiaplay.Width - 50,
				this.ClientSize.Height - 50);
			tlpProperty.Visible = false;
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

		private void Form1_MouseUp(object sender, MouseEventArgs e)
		{
			Dot selectedItem = matrix.SelectDot(MatrixArea, e.X, e.Y);
			if (selectedItem != null)
			{
				if(selectedItem.Selected)
				{
					tlpProperty.Visible = true;
					lbRowValue.Text = selectedItem.Row.ToString();
					lbColValue.Text = selectedItem.Column.ToString();

					if(SelectedDot != null)
						SelectedDot.Selected = false;
					SelectedDot = selectedItem;
					if (cbFillColor.Checked)
					{
						SelectedDot.SetColor(btnFillColor.BackColor);
						if(ListColorExist(btnFillColor.BackColor) == false)
						{
							ListViewItem item = new ListViewItem();
							item.SubItems[0] = new ListViewItem.ListViewSubItem(item, 
								lvColorList.Items.Count.ToString());
							item.SubItems.Add(new ListViewItem.ListViewSubItem(item,
								(btnFillColor.BackColor.ToArgb() & 0xFFFFFF).ToString("X")));
							item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "", btnFillColor.BackColor, btnFillColor.BackColor, null));
							lvColorList.Items.Add(item);
						}
					}

					if (selectedItem.BackColor != null)
					{
						btnSelectedColor.BackColor = (Color)selectedItem.BackColor;
						btnSelectedColor.Text = (((Color)selectedItem.BackColor).ToArgb() & 0xFFFFFF).ToString("X");
					}
					else
					{
						btnSelectedColor.BackColor = (Color)SystemColors.Control;
						btnSelectedColor.Text = "";
					}
				}
				else
				{
					if (SelectedDot != null)
					{
						if (cbFillColor.Checked)
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

		private void btnFillColor_Click(object sender, EventArgs e)
		{
			ColorDialog cDialog = new ColorDialog();
			DialogResult res = cDialog.ShowDialog();
			if(res == System.Windows.Forms.DialogResult.OK)
			{
				SetColor = cDialog.Color;
				btnFillColor.BackColor = cDialog.Color;
				btnFillColor.Text = (SetColor.Value.ToArgb() & 0xFFFFFF).ToString("X");
			}
		}

		private void btnColor_Click(object sender, EventArgs e)
		{
			ColorDialog cDialog = new ColorDialog();
			DialogResult res = cDialog.ShowDialog();
			if (res == System.Windows.Forms.DialogResult.OK)
			{
				btnSelectedColor.BackColor = cDialog.Color;
				btnSelectedColor.Text = (cDialog.Color.ToArgb() & 0xFFFFFF).ToString("X");
				if (SelectedDot != null)
					SelectedDot.SetColor(btnSelectedColor.BackColor);
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{

		}

		private bool ListColorExist(Color color)
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
    }
}
