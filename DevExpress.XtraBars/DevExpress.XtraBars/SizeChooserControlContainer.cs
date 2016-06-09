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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Controls {
	[ToolboxItem(false)]
	public class SizeChooserPopupControlContainer : PopupControlContainer {
		#region Fields
		SizeChooserItem[,] items;
		Bitmap itemBitmap;
		Size imageSize;
		int maxPageColumns = 2;
		int maxPageRows = 3;
		int selPageColumns;
		int selPageRows;
		LabelControl label;
		PanelControl panel;
		bool canMouseSelect;
		bool commited;
		#endregion
		public SizeChooserPopupControlContainer() {
			itemBitmap = LoadItemBitmap();
			imageSize = itemBitmap.Size;
			InitLabel();
		}
		#region Properties
		int PageColumns { get { return (items != null) ? items.GetLength(1) : 0; } }
		int PageRows { get { return (items != null) ? items.GetLength(0) : 0; } }
		public PanelControl Panel { get { return panel; } }
		public int SelectedColumns { get { return selPageColumns; } }
		public int SelectedRows { get { return selPageRows; } }
		public bool Commited { get { return commited; } }
		protected internal virtual int DefaultMaxPageColumns { get { return 50; } }
		protected internal virtual int DefaultMaxPageRows { get { return 50; } }
		protected internal virtual int DefaultPageColumns { get { return 3; } }
		protected internal virtual int DefaultPageRows { get { return 2; } }
		protected internal virtual int InnerMargin { get { return 5; } }
		protected virtual string CancelButtonCaption { get { return BarLocalizer.Active.GetLocalizedString(BarString.CancelButton); } }
		protected virtual string SizeStringFormat { get { return "{0} x {1}"; } }
		#endregion
		void InitLabel() {
			this.label = new LabelControl();
			this.panel = new PanelControl();
			this.panel.Controls.Add(this.label);
			Controls.Add(panel);
			this.label.MouseMove += OnLabelMouseMove;
			this.label.MouseDown += OnLabelMouseDown;
			this.label.Text = CancelButtonCaption;
			this.label.ImeMode = ImeMode.NoControl;
			this.label.Dock = DockStyle.Fill;
			this.label.Name = "label";
			this.label.TabIndex = 0;
			this.label.Appearance.TextOptions.VAlignment = VertAlignment.Center;
			this.label.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
			this.label.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
			this.label.AutoSizeMode = LabelAutoSizeMode.None;
			this.panel.Dock = DockStyle.Bottom;
			this.panel.BorderStyle = BorderStyles.Style3D;
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(188, 29);
		}
		protected internal virtual Bitmap LoadItemBitmap() {
			return ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraBars.Images.SizeItem.png", Assembly.GetExecutingAssembly());
		}
		void OnLabelMouseDown(object sender, MouseEventArgs e) {
			if (canMouseSelect) {
				HideCancel();
			}
		}
		void OnLabelMouseMove(object sender, MouseEventArgs e) {
			if (canMouseSelect) {
				selPageColumns = -1;
				selPageRows = -1;
				UpdateSelectionView(0, 0);
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if (items == null || itemBitmap == null) return;
			for (int i = 0; i < items.GetLength(0); i++)
				for (int j = 0; j < items.GetLength(1); j++)
					items[i, j].Draw(e.Graphics, InnerMargin, itemBitmap);
		}
		public override void ShowPopup(BarManager manager, Point p) {
			PerformInitializationBeforeShowPopup(manager);
			base.ShowPopup(manager, p);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if (canMouseSelect) {
				Point pt = PointToClient(MousePosition);
				SetSelectionParams(pt);
				UpdateSelectionView(0, 0);
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			Point pt = PointToClient(MousePosition);
			if (!GetItemRect(items).Contains(pt))
				HideCancel();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			canMouseSelect = false;
			return HandleKey(keyData) ? true : base.ProcessDialogKey(keyData);
		}
		protected override void OnPopup() {
			base.OnPopup();
			commited = false;
		}
		void PerformInitializationBeforeShowPopup(BarManager manager) {
			selPageRows = -1;
			selPageColumns = -1;
			int columns, rows;
			maxPageColumns = DefaultMaxPageColumns;
			maxPageRows = DefaultMaxPageRows;
			SetPageColumns(out columns, DefaultPageColumns);
			SetPageRows(out rows, DefaultPageRows);
			canMouseSelect = true;
			this.items = CreateItems(rows, columns);
		}
		bool HandleKey(Keys key) {
			switch (key) {
				case Keys.Down:
					selPageRows++;
					UpdateSelectionView(1, 1);
					return true;
				case Keys.Right:
					selPageColumns++;
					UpdateSelectionView(1, 1);
					return true;
				case Keys.Up:
					selPageRows--;
					UpdateSelectionView(1, 1);
					return true;
				case Keys.Left:
					selPageColumns--;
					UpdateSelectionView(1, 1);
					return true;
				case Keys.Enter:
					HideCommit();
					return true;
				case Keys.Escape:
					HideCancel();
					return true;
			}
			return false;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			HideCommit();
		}
		void HideCommit() {
			Hide(true);
		}
		void HideCancel() {
			Hide(false);
		}
		void Hide(bool commited) {
			this.commited = commited;
			HidePopup();
		}
		void UpdateSelectionView(int minColumns, int minRows) {
			selPageColumns = Math.Min(maxPageColumns, Math.Max(minColumns, selPageColumns));
			selPageRows = Math.Min(maxPageRows, Math.Max(minRows, selPageRows));
			UpdatePages(selPageColumns, selPageRows);
			SelectPages();
			Invalidate();
			UpdateLabelText(selPageColumns, selPageRows);
		}
		void UpdateLabelText(int columns, int rows) {
			label.Text = (rows <= 0 || columns <= 0) ? CancelButtonCaption :
				String.Format(SizeStringFormat, rows, columns);
		}
		void SelectPages() {
			for (int i = 0; i < items.GetLength(0); i++) {
				for (int j = 0; j < items.GetLength(1); j++) {
					SizeChooserItem item = items[i, j];
					item.Selected = (selPageColumns > j && selPageRows > i);
				}
			}
		}
		void UpdatePages(int columns, int rows) {
			if ((columns > PageColumns && columns <= maxPageColumns) ||
				(rows > PageRows && rows <= maxPageRows)) {
				SetPageColumns(out columns, Math.Max(columns, PageColumns));
				SetPageRows(out rows, Math.Max(rows, PageRows));
				this.items = CreateItems(rows, columns);
			}
		}
		void SetSelectionParams(Point point) {
			selPageRows = 0;
			selPageColumns = 0;
			if (items == null)
				return;
			Rectangle rect = new Rectangle(0, 0, point.X, point.Y);
			for (int i = 0; i < items.GetLength(0); i++) {
				for (int j = 0; j < items.GetLength(1); j++) {
					if (items[i, j].Rect.IntersectsWith(rect)) {
						selPageRows = i + 1;
						selPageColumns = Math.Max(j + 1, selPageColumns);
					}
				}
			}
			if (selPageColumns > 0 && selPageRows > 0) {
				rect = items[selPageRows - 1, selPageColumns - 1].Rect;
				int dw = SystemInformation.Border3DSize.Height;
				if (point.X > rect.Right + dw) selPageColumns++;
				if (point.Y > rect.Bottom + dw) selPageRows++;
			}
		}
		void SetPageColumns(out int columns, int val) {
			columns = Math.Min(maxPageColumns, val);
		}
		void SetPageRows(out int rows, int val) {
			rows = Math.Min(maxPageRows, val);
		}
		SizeChooserItem[,] CreateItems(int rows, int columns) {
			SizeChooserItem[,] result = new SizeChooserItem[rows, columns];
			int dw = InnerMargin;
			Rectangle rect = new Rectangle(dw, dw, imageSize.Width + 2 * dw, imageSize.Height + 2 * dw);
			for (int i = 0; i < rows; i++) {
				for (int j = 0; j < columns; j++) {
					result[i, j] = new SizeChooserItem(rect);
					rect.X += (rect.Width + dw);
				}
				rect.X = dw;
				rect.Y += (rect.Height + dw);
			}
			System.Diagnostics.Debug.Assert(result.Length > 0);
			SetControlSize(GetItemSize(result));
			return result;
		}
		void SetControlSize(Size size) {
			ClientSize = size;
			Form form = FindForm();
			if (form != null)
				form.Size = size;
		}
		Size GetItemSize(SizeChooserItem[,] items) {
			Rectangle rect = GetItemRect(items);
			int width = rect.Right + InnerMargin;
			UpdatePanelHeight(rect.Width, items.GetLength(0), items.GetLength(1));
			int height = rect.Bottom + panel.Height + 2 * InnerMargin;
			return new Size(width, height);
		}
		Rectangle GetItemRect(SizeChooserItem[,] items) {
			if (items.GetLength(0) > 0 && items.GetLength(1) > 0) {
				Rectangle r1 = items[0, 0].Rect;
				Rectangle r2 = items[items.GetLength(0) - 1, items.GetLength(1) - 1].Rect;
				return Rectangle.Union(r1, r2);
			}
			return Rectangle.Empty;
		}
		protected virtual void UpdatePanelHeight(int width, int rows, int columns) {
		}
	}
}
namespace DevExpress.XtraBars.InternalItems {
	public class SizeChooserItem {
		Rectangle rect = Rectangle.Empty;
		bool selected;
		internal Rectangle Rect { get { return rect; } set { rect = value; } }
		internal bool Selected { get { return selected; } set { selected = value; } }
		public SizeChooserItem(Rectangle rect) {
			this.rect = rect;
		}
		public void Draw(Graphics gr, int innerMargin, Image image) {
			using (SolidBrush brush = new SolidBrush(selected ? SystemColors.ActiveCaption : SystemColors.Window))
				gr.FillRectangle(brush, rect);
			using (Pen pen = new Pen(SystemColors.InactiveCaption))
				gr.DrawRectangle(pen, new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1));
			gr.DrawImage(image, Rectangle.Inflate(rect, -innerMargin, -innerMargin));
		}
	}
}
