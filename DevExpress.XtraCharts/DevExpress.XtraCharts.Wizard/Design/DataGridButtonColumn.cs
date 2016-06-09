#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraCharts.Design {
	public class DataGridButtonColumn : DataGridColumnStyle {
		protected Rectangle depressedBounds;
		DataGridColumnStylePadding padding = new DataGridColumnStylePadding(0, 0, 0, 0);
		Size controlSize = new Size(29, 20);
		IMouseValidator mouseValidator;
		internal IMouseValidator MouseValidator { get { return mouseValidator; } }
		public event ButtonColumnClickHandler Click;
		public DataGridColumnStylePadding Padding {
			get { return padding; }
			set {
				padding = value;
				UpdateCellSize();
			}
		}
		public virtual Size ControlSize {
			get { return controlSize; }
			set {
				controlSize = value;
				UpdateCellSize();
			}
		}
		public DataGridButtonColumn(IMouseValidator mouseValidator) : base() {
			this.mouseValidator = mouseValidator;
			WidthChanged += new EventHandler(DataGridButtonColumn_WidthChanged);
		}
		void DataGrid_MouseDown(object sender, MouseEventArgs e) {
			if (mouseValidator == null || !mouseValidator.CanMouseDown)
				return;
			DataGrid.HitTestInfo info = DataGridTableStyle.DataGrid.HitTest(e.X, e.Y);
			if (e.Button == MouseButtons.Left &&
				info.Type == DataGrid.HitTestType.Cell &&
				DataGridTableStyle.GridColumnStyles.IndexOf(this) == info.Column) {
				Rectangle cursorRect = new Rectangle(e.X, e.Y, 1, 1);
				Rectangle cellBounds = DataGridTableStyle.DataGrid.GetCellBounds(info.Row, info.Column);
				Rectangle buttonBounds = GetControlBounds(cellBounds);
				if (cursorRect.IntersectsWith(buttonBounds)) {
					depressedBounds = cellBounds;
					DataGridTableStyle.DataGrid.Invalidate(cellBounds);
				}
			}
		}
		internal virtual void DataGrid_MouseUp(object sender, MouseEventArgs e) {
			DataGrid.HitTestInfo info = DataGridTableStyle.DataGrid.HitTest(e.X, e.Y);
			if (!depressedBounds.Equals(Rectangle.Empty)) {
				Rectangle cellBounds = DataGridTableStyle.DataGrid.GetCellBounds(info.Row, info.Column);
				if (depressedBounds.Equals(cellBounds)) {
					Rectangle cursorRect = new Rectangle(e.X, e.Y, 1, 1);
					Rectangle buttonBounds = GetControlBounds(cellBounds);
					if (cursorRect.IntersectsWith(buttonBounds) && MouseValidator != null && MouseValidator.CanMouseDown)
						if (Click != null && Click.GetInvocationList().Length > 0)
							Click(new ButtonColumnEventArgs(info.Row, info.Column));
				}
				depressedBounds = Rectangle.Empty;
				DataGridTableStyle.DataGrid.Invalidate(cellBounds);
			}
		}
		void DataGridButtonColumn_WidthChanged(object sender, EventArgs e) {
			UpdateCellSize();
		}
		void UpdateCellSize() {
			Width = GetPreferredSize(null, null).Width;
		}
		protected Rectangle GetControlBounds(Rectangle cellBounds) {
			return new Rectangle(cellBounds.X + Padding.Left, cellBounds.Y + Padding.Top, controlSize.Width, controlSize.Height);
		}
		protected override void SetDataGridInColumn(DataGrid value) {
			base.SetDataGridInColumn(value);
			DataGridTableStyle.DataGrid.MouseDown += new MouseEventHandler(DataGrid_MouseDown);
			DataGridTableStyle.DataGrid.MouseUp += new MouseEventHandler(DataGrid_MouseUp);
		}
		protected override void Abort(int rowNum) {
		}
		protected override bool Commit(CurrencyManager dataSource, int rowNum) {
			return true;
		}
		protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible) {
		}
		protected override int GetMinimumHeight() {
			return GetPreferredHeight(null, null);
		}
		protected override int GetPreferredHeight(Graphics gr, object value) {
			return GetPreferredSize(null, null).Height;
		}
		protected override Size GetPreferredSize(Graphics gr, object value) {
			return new Size(controlSize.Width + padding.Left + padding.Right, controlSize.Height + padding.Top + padding.Bottom);
		}
		protected override void Paint(Graphics gr, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight) {
			gr.FillRectangle(backBrush, bounds);
			Rectangle controlBounds = GetControlBounds(bounds);
			bool drawFocusRectangle = true;
			Rectangle focusBounds = controlBounds;
			focusBounds.Inflate(-4, -4);
			Rectangle fontBounds = focusBounds;
			fontBounds.Inflate(-3, -3);
			ButtonState bs;
			if (depressedBounds != Rectangle.Empty && depressedBounds == bounds)
				bs = ButtonState.Pushed;
			else {
				bs = ButtonState.Inactive;
				drawFocusRectangle = false;
			}
			ControlPaint.DrawButton(gr, controlBounds, bs);
			if (drawFocusRectangle)
				ControlPaint.DrawFocusRectangle(gr, focusBounds, Color.Gray, Control.DefaultBackColor);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			sf.FormatFlags = StringFormatFlags.DirectionRightToLeft | StringFormatFlags.FitBlackBox;
			using (Brush brush = new SolidBrush(Color.Black)) {
				gr.DrawString(GetColumnValueAtRow(source, rowNum).ToString(), DataGridTableStyle.DataGrid.Font, brush, fontBounds, sf);
			}
		}
		protected override void Paint(Graphics gr, Rectangle bounds, CurrencyManager source, int rowNum) {
			Paint(gr, bounds, source, rowNum, false);
		}
		protected override void Paint(Graphics gr, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight) {
			Paint(gr, bounds, source, rowNum, Brushes.White, Brushes.Black, false);
		}
	}
	public class ColorGridButtonColumn : DataGridButtonColumn {
		Color color = Color.Empty;
		CurrencyManager listManager;
		public event ButtonColumnClickHandler ClearColorClick;
		public Color Color {
			get { return color; }
			set { color = value; }
		}
		public ColorGridButtonColumn(IMouseValidator mouseValidator, CurrencyManager listManager) : base(mouseValidator) 
		{
			this.listManager = listManager;
		}
		protected override void Paint(Graphics gr, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight) {
			gr.FillRectangle(backBrush, bounds);
			Rectangle controlBounds = GetControlBounds(bounds);
			Rectangle focusBounds = controlBounds;
			focusBounds.Inflate(-4, -4);
			Rectangle fontBounds = focusBounds;
			fontBounds.Inflate(-3, -3);
			bool drawFocus = depressedBounds != Rectangle.Empty && depressedBounds == bounds;
			SeriesPoint point = source.List[rowNum] as SeriesPoint;
			if (point != null)
				this.Color = point.Color;
			if (this.Color.IsEmpty) {
				if (drawFocus)
					ControlPaint.DrawFocusRectangle(gr, focusBounds, Color.Gray, Control.DefaultBackColor);
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				sf.FormatFlags = StringFormatFlags.DirectionRightToLeft | StringFormatFlags.FitBlackBox;
				using (Brush brush = new SolidBrush(Color.Black))
					gr.DrawString("...", DataGridTableStyle.DataGrid.Font, brush, fontBounds, sf);
			}
			else {
				focusBounds.Width = focusBounds.Width - 16;
				using (Brush brush = new SolidBrush(Color))
					gr.FillRectangle(brush, focusBounds);
				if (drawFocus)
					ControlPaint.DrawFocusRectangle(gr, focusBounds, Color.Gray, Control.DefaultBackColor);
				using (Image cross = global::DevExpress.XtraCharts.Wizard.Properties.Resources.cross_16x16)
					gr.DrawImage(cross, new Point(focusBounds.Location.X + focusBounds.Width + 1, controlBounds.Location.Y + controlBounds.Height / 2 - cross.Height / 2));
			}
		}
		internal override void DataGrid_MouseUp(object sender, MouseEventArgs e) {
			DataGrid.HitTestInfo info = DataGridTableStyle.DataGrid.HitTest(e.X, e.Y);
			if (listManager.List.Count <= info.Row || info.Row < 0)
				return;
			SeriesPoint point = listManager.List[info.Row] as SeriesPoint;
			if (point == null)
				return;
			if (point.Color.IsEmpty) {
				base.DataGrid_MouseUp(sender, e);
				return;
			}
			if (!depressedBounds.Equals(Rectangle.Empty)) {
				Rectangle cellBounds = DataGridTableStyle.DataGrid.GetCellBounds(info.Row, info.Column);
				if (depressedBounds.Equals(cellBounds)) {
					Rectangle cursorRect = new Rectangle(e.X, e.Y, 1, 1);
					Rectangle crossRect = GetCrossBounds(cellBounds);
					if (cursorRect.IntersectsWith(crossRect) && MouseValidator != null && MouseValidator.CanMouseDown) {
						if (ClearColorClick != null && ClearColorClick.GetInvocationList().Length > 0)
							ClearColorClick(new ButtonColumnEventArgs(info.Row, info.Column));
					}
					else
						base.DataGrid_MouseUp(sender, e);
				}
				depressedBounds = Rectangle.Empty;
				DataGridTableStyle.DataGrid.Invalidate(cellBounds);
			}
		}
		Rectangle GetCrossBounds(Rectangle cellBounds) {
			Rectangle controlBounds = GetControlBounds(cellBounds);
			Rectangle focusBounds = controlBounds;
			focusBounds.Inflate(-4, -4);
			focusBounds.Width = focusBounds.Width - 16;
			Point point = new Point(focusBounds.Location.X + focusBounds.Width + 1, controlBounds.Location.Y + controlBounds.Height / 2 - 8);
			return new Rectangle(point, new Size(16, 16));
		}
	}
	public interface IMouseValidator {
		bool CanMouseDown { get; }
	}
	public delegate void ButtonColumnClickHandler(ButtonColumnEventArgs e);
	public class ButtonColumnEventArgs : EventArgs {
		int row, column;
		public int Row { get { return row; } }
		public int Column { get { return column; } }
		public ButtonColumnEventArgs(int row, int column) : base() {
			this.row = row;
			this.column = column;
		}
	}
	public class DataGridColumnStylePadding {
		int left;
		int right;
		int bottom;
		int top;
		public int Left { get { return left; } set { left = value; } }
		public int Right { get { return right; } set { right = value; } }
		public int Bottom { get { return bottom; } set { bottom = value; } }
		public int Top { get { return top; } set { top = value; } }
		public DataGridColumnStylePadding(int value) {
			left = value;
			right = value;
			bottom = value;
			top = value;
		}
		public DataGridColumnStylePadding(int left, int right, int bottom, int top) {
			this.left = left;
			this.right = right;
			this.bottom = bottom;
			this.top = top;
		}
	}
}
