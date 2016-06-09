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
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraCharts.Design {
	internal class ChartImageListBoxInnerControl : XtraScrollableControl {
		public class ControlsMatrix  {
			List<ChartImageListBoxItem> list = new List<ChartImageListBoxItem>();
			Size bounds;
			Size elementSize;
			Size offset;
			Size maxSize = Size.Empty;
			int endRowElementsCount;
			int pageButtonStep = 0;
			int selectedIndex = -1;
			public Control[] Controls { get { return list.ToArray(); } }
			public int ItemCount { get { return list.Count; } }
			public int RowsCount { get { return maxSize.Height; } }
			public int ColumnsCount { get { return maxSize.Width; } }
			public int CurrentIndex { 
				get { return list.Count > 0 ? selectedIndex : -1; }
				set { selectedIndex = value; }
			}
			public Point Current { 
				get { 
					int y = selectedIndex / ColumnsCount;
					int x = selectedIndex - y * ColumnsCount;
					return new Point(x, y);
				}
			}
			public ChartImageListBoxItem CurrentElement { get { return CurrentIndex > -1 ? list[CurrentIndex] : null; } }
			public ChartImageListBoxItem this[int x, int y] { 
				get { 
					int index = ColumnsCount * y + x;
					return index >= list.Count ? list[list.Count - 1] : list[index];
				}
			}
			public void Initialize(List<ChartImageListBoxItem> list, Size bounds, Size elementSize, Size offset) {
				this.list = list;
				Update(bounds, elementSize, offset);
			}
			public void Update(Size bounds, Size elementSize, Size offset) {
				this.bounds = bounds;
				this.elementSize = elementSize;
				this.offset = offset;
				maxSize = CalculateMatrixSize(bounds, elementSize);
				endRowElementsCount = list.Count - (RowsCount - 1) * ColumnsCount;
				pageButtonStep = bounds.Height / elementSize.Height;
				if (list.Count > 0)
					UpdateLocation(bounds, elementSize, offset);
			}
			public void Clear() {
				maxSize = Size.Empty;
				endRowElementsCount = 0;
				pageButtonStep = 0;
				selectedIndex = -1;
				list.Clear();
				UpdateLocation(bounds, elementSize, offset);
			}
			public void MoveUp(ref Point current) {
				current.Y = current.Y - 1 >= 0 ? current.Y - 1 : 0;
				LimitX(ref current);
			}
			public void MoveDown(ref Point current) {
				current.Y = current.Y + 1 < RowsCount ? current.Y + 1 : RowsCount - 1;
				LimitX(ref current);
			}
			public void MoveLeft(ref Point current) {
				if (current.X > 0 || current.Y > 0) {
					int x = current.X - 1;
					if (x < 0) {
						current.X = ColumnsCount - 1;
						MoveUp(ref current);
					}
					else
						current.X--;
				}
			}
			public void MoveRight(ref Point current) {
				if (current.Y < RowsCount - 1 || current.X < endRowElementsCount - 1) {
					int x = current.X + 1;
					if (x > ColumnsCount - 1) {
						current.X = 0;
						MoveDown(ref current);
					}
					else
						current.X++;
				}
			}
			public void MoveHome(ref Point current) {
				current.X = 0;
				current.Y = 0;
			}
			public void MoveEnd(ref Point current) {
				current.Y = RowsCount - 1;
				current.X = ColumnsCount - 1;
				LimitX(ref current);
			}
			public void MovePageUp(ref Point current) {
				for (int i = 0; i < pageButtonStep; i++)
					MoveUp(ref current);
			}
			public void MovePageDown(ref Point current) {
				for (int i = 0; i < pageButtonStep; i++)
					MoveDown(ref current);
			}
			public int PointToIndex(int x, int y) {
				return ColumnsCount * y + x;
			}
			void LimitX(ref Point point) {
				if (point.Y == RowsCount - 1 && point.X >= endRowElementsCount)
					point.X = endRowElementsCount - 1;
			}
			void UpdateLocation(Size bounds, Size elementSize, Size offset) {
				for (int y = 0; y < RowsCount; y++)
					for (int x = 0; x < ColumnsCount; x++) {
						if (y == RowsCount - 1 && x > endRowElementsCount - 1) 
							return;
						this[x, y].Location = new Point(x * elementSize.Width - offset.Width, y * elementSize.Height - offset.Height);
					}
			}
			Size CalculateMatrixSize(Size bounds, Size elementSize) {
				int columns = bounds.Width / elementSize.Width > 0 ? bounds.Width / elementSize.Width : 1;
				int rows = list.Count / columns;
				if (list.Count % columns > 0)
					rows++;
				return new Size(columns, rows);
			}
		}
		ControlsMatrix matrix = new ControlsMatrix();
		ImageCollection collection = new ImageCollection();
		ChartImageListBoxContainer container;
		Size ImageSize { get { return new Size(collection.ImageSize.Width, collection.ImageSize.Height); } }
		int InnerWidth { get { return Size.Width - DefaultVScrollBarWidth; } }
		public int ItemCount { get { return matrix.ItemCount; } }
		public int SelectedIndex { 
			get { return matrix.CurrentIndex; }
			set { ChangeSelectedControl(value); }
		}
		public ChartImageListBoxInnerControl(ChartImageListBoxContainer container) {
			this.container = container;
		}
		public void Initialize(ImageCollection images) {
			collection = new ImageCollection();
			collection.ImageSize = images.ImageSize;
			foreach (Image image in images.Images)
				collection.AddImage(image);
			UpdateControlsList();
			UpdateSelectedItem();
		}
		public void Clear() {
			collection.Clear();
			matrix.Clear();
		}
		public void RaiseApplyChanges() {
			container.RaiseApplyChanges();
		}
		protected override void OnSizeChanged(EventArgs e) {
			SuspendLayout();
			matrix.Update(new Size(InnerWidth, Size.Height), ImageSize, CalcOffset());
			base.OnSizeChanged(e);
			ResumeLayout();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			BeginInvoke(new MethodInvoker(delegate() { ScrollControlIntoView(matrix.CurrentElement); }));
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			Point current = matrix.Current;
			switch (keyData) {
				case Keys.Up:
					matrix.MoveUp(ref current);
					break;
				case Keys.Down:
					matrix.MoveDown(ref current);
					break;
				case Keys.Left:
					matrix.MoveLeft(ref current);
					break;
				case Keys.Right:
					matrix.MoveRight(ref current);
					break;
				case Keys.Home:
					matrix.MoveHome(ref current);
					break;
				case Keys.End:
					matrix.MoveEnd(ref current);
					break;
				case Keys.PageUp:
					matrix.MovePageUp(ref current);
					break;
				case Keys.PageDown:
					matrix.MovePageDown(ref current);
					break;
				default:
					return base.ProcessDialogKey(keyData);
			}
			ChangeSelectedControl(matrix.PointToIndex(current.X, current.Y));
			return true;
		}
		void UpdateSelectedItem() {
			ChartImageListBoxItem item = matrix.CurrentElement;
			if (item != null) {
				item.Refresh();
				item.Focus();
			}
		}
		void ChangeSelectedControl(int index) {
			ChartImageListBoxItem prev = matrix.CurrentElement;
			matrix.CurrentIndex = index;
			if (prev != null)
				prev.Refresh();
			UpdateSelectedItem();
		}
		void UpdateControlsList() {
			Controls.Clear();
			int count = collection.Images.Count;
			List<ChartImageListBoxItem> list = new List<ChartImageListBoxItem>(count);
			for (int index = 0; index < count; index++) 
				list.Add(CreateImageControl(collection.Images[index], index));
			matrix.Initialize(list, new Size(InnerWidth, Size.Height), ImageSize, CalcOffset());
			Controls.AddRange(matrix.Controls);
		}
		ChartImageListBoxItem CreateImageControl(Image image, int index) {
			ChartImageListBoxItem styleEdit = new ChartImageListBoxItem(this, index);
			styleEdit.Size = ImageSize;
			styleEdit.Image = image;
			styleEdit.BorderStyle = BorderStyles.NoBorder;
			styleEdit.Properties.ReadOnly = true;
			return styleEdit;
		}
		Size CalcOffset() {
			return new Size(-DisplayRectangle.X, -DisplayRectangle.Y);
		}
		private void InitializeComponent() {
			this.SuspendLayout();
			this.ResumeLayout(false);
		}
	}
	delegate bool DrawFocusedDelegate();
	internal class ChartImageListBoxItemPainter : StyleEditPainter {
		readonly DrawFocusedDelegate drawFocused;
		protected override int PenWidth { get { return 5; } }
		protected override int Fraction { get { return 12; } }
		public ChartImageListBoxItemPainter(UserLookAndFeel lookAndFeel, DrawFocusedDelegate drawFocused) : base(lookAndFeel) {
			this.drawFocused = drawFocused;
		}
		protected override Color GetFocusColor(Color baseColor) {
			return Color.FromArgb(150, baseColor);
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			if (drawFocused())
				DrawCustomBorder(info);
		}
		protected override void DrawFocusRect(ControlGraphicsInfoArgs info) {
		}
	}
	internal class ChartImageListBoxItem : PictureEdit {
		readonly ChartImageListBoxInnerControl container;
		readonly int index;
		ChartImageListBoxItemPainter painter;
		protected override BaseControlPainter Painter {
			get {
				if (painter == null)
					painter = new ChartImageListBoxItemPainter(LookAndFeel, delegate() { return index == container.SelectedIndex; });
				return painter;
			}
		}
		public ChartImageListBoxItem(ChartImageListBoxInnerControl container, int index) {
			this.container = container;
			this.index = index;
		}
		protected override void OnGotFocus(EventArgs e) {
			ChartImageListBoxInnerControl container = Parent as ChartImageListBoxInnerControl;
			if (container != null)
				container.SelectedIndex = index;
			base.OnGotFocus(e);
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left)
				ApplyChanges();
			else
				base.OnMouseDoubleClick(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if (e.KeyCode == Keys.Return)
				ApplyChanges();
			else
				base.OnKeyDown(e);
		}
		void ApplyChanges() {
			ChartImageListBoxInnerControl container = Parent as ChartImageListBoxInnerControl;
			if (container != null)
				container.RaiseApplyChanges();
		}
	}
}
