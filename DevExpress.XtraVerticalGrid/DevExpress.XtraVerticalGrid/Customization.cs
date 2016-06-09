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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DevExpress.XtraVerticalGrid.Localization;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraVerticalGrid.Painters;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTab;
using DevExpress.Utils.Paint;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraVerticalGrid.Rows {
	public class VGridCustomizationForm : XtraForm {
		XtraTabControl tabControl1;
		XtraTabPage tpRows;
		XtraTabPage tpCategories;
		SimpleButton btNew;
		SimpleButton btDelete;
		private DevExpress.XtraVerticalGrid.Rows.VGridCustomizationForm.RowsListBox lbRows;
		private DevExpress.XtraVerticalGrid.Rows.VGridCustomizationForm.RowsListBox lbCategories;
		BaseHandler handler;
		VGridNewCategoryForm categoryForm;
		VGridControlBase grid;
		BaseRow pressedRow;
		internal VGridCustomizationForm(VGridControlBase grid, BaseHandler handler) {
			InitializeComponent();
			this.Text = VGridLocalizer.Active.GetLocalizedString(VGridStringId.RowCustomizationText);
			this.btNew.Text = VGridLocalizer.Active.GetLocalizedString(VGridStringId.RowCustomizationNewCategoryText);
			this.btDelete.Text = VGridLocalizer.Active.GetLocalizedString(VGridStringId.RowCustomizationDeleteCategoryText);
			this.tpRows.Text = VGridLocalizer.Active.GetLocalizedString(VGridStringId.RowCustomizationTabPageRowsText);
			this.tpCategories.Text = VGridLocalizer.Active.GetLocalizedString(VGridStringId.RowCustomizationTabPageCategoriesText);
			this.grid = grid;
			this.handler = handler;
			this.pressedRow = null;
			this.categoryForm = null;
			this.MinimumSize = MinFormSize;
			FillRows();
			UpdateStyle();
			UpdateSize();
		}
		public override RightToLeft RightToLeft { 
			get {
				if(Grid == null) return base.RightToLeft;
				return Grid.RightToLeft; 
			}
			set { base.RightToLeft = value; } 
		}
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		internal ListBoxItemCollection RowHeaders { get { return lbRows.Items; } }
		internal ListBoxItemCollection CategoryHeaders { get { return lbCategories.Items; } }
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.tabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.tpRows = new DevExpress.XtraTab.XtraTabPage();
			this.lbRows = new DevExpress.XtraVerticalGrid.Rows.VGridCustomizationForm.RowsListBox();
			this.tpCategories = new DevExpress.XtraTab.XtraTabPage();
			this.btDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btNew = new DevExpress.XtraEditors.SimpleButton();
			this.lbCategories = new DevExpress.XtraVerticalGrid.Rows.VGridCustomizationForm.RowsListBox();
			((System.ComponentModel.ISupportInitialize)(this.tabControl1)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tpRows.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbRows)).BeginInit();
			this.tpCategories.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbCategories)).BeginInit();
			this.SuspendLayout();
			this.tabControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.tpRows,
																					  this.tpCategories});
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedTabPage = this.tpCategories;
			this.tabControl1.Size = new System.Drawing.Size(172, 226);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
																						this.tpRows,
																						this.tpCategories});
			this.tpRows.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.lbRows});
			this.tpRows.Name = "tpRows";
			this.tpRows.Size = new System.Drawing.Size(166, 210);
			this.lbRows.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbRows.ItemHeight = 16;
			this.lbRows.Name = "lbRows";
			this.lbRows.Size = new System.Drawing.Size(166, 210);
			this.lbRows.TabIndex = 0;
			this.lbRows.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lb_KeyDown);
			this.lbRows.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lb_MouseDown);
			this.lbRows.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lb_MeasureItem);
			this.lbRows.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lb_MouseUp);
			this.lbRows.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lb_MouseMove);
			this.lbRows.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lb_DrawItem);
			this.tpCategories.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.btDelete,
																					   this.btNew,
																					   this.lbCategories});
			this.tpCategories.Name = "tpCategories";
			this.tpCategories.Size = new System.Drawing.Size(166, 210);
			this.btDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btDelete.Enabled = false;
			this.btDelete.Location = new System.Drawing.Point(90, 181);
			this.btDelete.Name = "btDelete";
			this.btDelete.Size = new System.Drawing.Size(66, 24);
			this.btDelete.TabIndex = 2;
			this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
			this.btNew.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btNew.Location = new System.Drawing.Point(8, 181);
			this.btNew.Name = "btNew";
			this.btNew.Size = new System.Drawing.Size(64, 24);
			this.btNew.TabIndex = 1;
			this.btNew.Click += new System.EventHandler(this.btNew_Click);
			this.lbCategories.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.lbCategories.ItemHeight = 16;
			this.lbCategories.Location = new System.Drawing.Point(0, 0);
			this.lbCategories.Name = "lbCategories";
			this.lbCategories.Size = new System.Drawing.Size(166, 168);
			this.lbCategories.TabIndex = 0;
			this.lbCategories.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lb_KeyDown);
			this.lbCategories.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lb_MouseDown);
			this.lbCategories.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lb_MeasureItem);
			this.lbCategories.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lb_MouseUp);
			this.lbCategories.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lb_MouseMove);
			this.lbCategories.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lb_DrawItem);
			this.lbCategories.SelectedIndexChanged += new System.EventHandler(this.lbCategories_SelectedIndexChanged);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(200, 226);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.tabControl1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "VGridCustomizationForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			((System.ComponentModel.ISupportInitialize)(this.tabControl1)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tpRows.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lbRows)).EndInit();
			this.tpCategories.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lbCategories)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		internal void SwitchTabPage(BaseRow row) {
			if(row == null) return;
			if(row is CategoryRow) {
				tabControl1.SelectedTabPage = tpCategories;
				return;
			}
			tabControl1.SelectedTabPage = tpRows;
		}
		private void btNew_Click(object sender, System.EventArgs e) {
			categoryForm = new VGridNewCategoryForm(Grid);
			if(categoryForm.ShowDialog(this) == DialogResult.OK)
				FillRows();
			categoryForm.Dispose();
			categoryForm = null;
		}
		private void btDelete_Click(object sender, System.EventArgs e) {
			if(lbCategories.SelectedIndex == -1) return;
			BaseRowHeaderInfo rh = (BaseRowHeaderInfo)lbCategories.Items[lbCategories.SelectedIndex];
			if(Grid.InternalCustomizationFormDeletingCategory((CategoryRow)rh.Row))
				FillRows();
		}
		private void lbCategories_SelectedIndexChanged(object sender, System.EventArgs e) {
			btDelete.Enabled = lbCategories.SelectedIndex != -1;
		}
		private void lb_DrawItem(object sender, DevExpress.XtraEditors.ListBoxDrawItemEventArgs e) {
			RowsListBox lb = sender as RowsListBox;
			if(e.Index < 0 || e.Index > lb.Items.Count - 1) return;
			BaseRowHeaderInfo rh = (BaseRowHeaderInfo)lb.Items[e.Index];
			if(rh.HeaderRect.Width != lb.ClientRectangle.Width)
				rh.Calc(new Rectangle(Point.Empty, new Size(lb.ClientRectangle.Width, rh.HeaderRect.Height)), rh.Row.Grid.ViewInfo, null, false, null);
			DrawRow(e.Graphics, rh, (e.State & DrawItemState.Selected) != 0, e.Bounds);
			e.Handled = true;
		}
		private void lb_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e) {
			RowsListBox lb = sender as RowsListBox;
			if(e.Index < 0 || e.Index > lb.Items.Count - 1) return;
			BaseRowHeaderInfo rh = (BaseRowHeaderInfo)lb.Items[e.Index];
			e.ItemHeight = rh.HeaderRect.Height;
		}
		private void lb_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyData == Keys.Escape) {
				if(IsCustomizationDragging) {
					CancelDrag();
					Handler.KeyDown(e);
				}
				Grid.CloseEditor();
			}
		}
		private void lb_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			Grid.CloseEditor();
			if((e.Button & MouseButtons.Left) == 0) return;
			BaseRowHeaderInfo rh = GetRowInfoByPoint(new Point(e.X, e.Y));
			if(rh != null) {
				RowsListBox lb = sender as RowsListBox;
				lb.Capture = true;
				PressedRow = rh.Row;
				Handler.MouseDown(GetGridMouseEventArgs(e));
			}
		}
		private void lb_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(IsCustomizationDragging) {
				Handler.MouseMove(GetGridMouseEventArgs(e));
			}
		}
		private void lb_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(IsCustomizationDragging) {
				RowsListBox lb = sender as RowsListBox;
				lb.Capture = false;
				Handler.MouseUp(GetGridMouseEventArgs(e));
			}
			CancelDrag();
		}
		private MouseEventArgs GetGridMouseEventArgs(MouseEventArgs e) {
			Point pt = ToGridPoint(new Point(e.X, e.Y));
			return new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);
		}
		private void DrawRow(Graphics g, BaseRowHeaderInfo rh, bool selected, Rectangle bounds) {
			Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
			Graphics  gg = Graphics.FromImage(bmp);
			try {
				using(GraphicsCache cache = new GraphicsCache(gg)) {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(cache.Graphics, rh.HeaderRect)) {
						PaintEventArgs e = new PaintEventArgs(bg.Graphics, rh.HeaderRect);
						Painter.DrawRowHeader(e, rh, viewInfo.RC);
						if(selected)
							XPaint.Graphics.DrawFocusRectangle(bg.Graphics, rh.HeaderRect, rh.Style.ForeColor, rh.Style.BackColor);
						else
							bg.Graphics.FillRectangle(rh.GetHorzLineBrush(viewInfo.RC), rh.HeaderRect.Left, rh.HeaderRect.Bottom - 1, rh.HeaderRect.Width, 1);
						bg.Render();
					}
				}
			}
			finally {
				gg.Dispose();
			}
			g.DrawImage(bmp, bounds);
		}
		private Point ToGridPoint(Point pt) {
			return Grid.PointToClient(ActiveListBox.PointToScreen(pt));
		}
		private void CancelDrag() {
			BaseRow rowInvalidate = PressedRow;
			Capture = false;
			PressedRow = null;
			ActiveListBox.InvalidateRow(rowInvalidate);
		}
		private BaseViewInfo viewInfo { get { return Grid.ViewInfo; } }
		protected VGridControlBase Grid { get { return grid; } }
		protected BaseHandler Handler { get { return handler; } }
		private VGridPainter Painter { get { return Grid.Painter; } }
		private RowsListBox ActiveListBox { 
			get {
				if(tabControl1.SelectedTabPage == tpRows)
					return lbRows;
				return lbCategories;
			} 
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridCustomizationFormPressedRow")]
#endif
		public BaseRow PressedRow { 
			get { return pressedRow; }
			set { 
				if(PressedRow != value) {
					ActiveListBox.InvalidateRow(PressedRow);
					pressedRow = value;
					ActiveListBox.InvalidateRow(PressedRow);
				}
			}
		}
		private bool IsCustomizationDragging { get { return (PressedRow != null); } }
		internal void ShowCustomization(Point location) {
			if(location == VGridStore.DefaultCustomizationFormLocation) {
				if(Grid.CustomizationFormBounds == VGridStore.DefaultCustomizationFormBounds) {
					location = Grid.PointToScreen(new Point(Grid.Bounds.Right - this.Size.Width, Grid.Bounds.Bottom - this.Size.Height));
					if(location.X + this.Size.Width > SystemInformation.PrimaryMonitorSize.Width)
						location.X = SystemInformation.PrimaryMonitorSize.Width - this.Size.Width;
				}
				else
					location = Grid.CustomizationFormBounds.Location;
				location = DevExpress.Utils.ControlUtils.CalcLocation(location, location, Size);
			}
			this.Location = location;
			Show();
			Grid.Focus();
		}
		internal bool ListBoxContains(Point pt) {
			pt = ActiveListBox.PointToClient(Grid.PointToScreen(pt));
			return ActiveListBox.Bounds.Contains(pt);
		}
		internal BaseRowHeaderInfo GetRowInfoByGridPoint(Point pt) {
			pt = ActiveListBox.PointToClient(Grid.PointToScreen(pt));
			return GetRowInfoByPoint(pt);
		}
		private BaseRowHeaderInfo GetRowInfoByPoint(Point pt) {
			int index = ActiveListBox.IndexFromPoint(pt);
			if(index == -1) return null;
			return (BaseRowHeaderInfo)ActiveListBox.Items[index];
		}
		internal void FillRows() {
			RowOperation op = new SortCustomizationRows(Grid, lbRows, lbCategories);
			Grid.RowsIterator.DoOperation(op);
			btDelete.Enabled = false;
			lbCategories.SelectedIndex = -1;
		}
		protected internal void UpdateStyle() {
			LookAndFeel.Assign(Grid.ElementsLookAndFeel);
			lbRows.LookAndFeel.ParentLookAndFeel = Grid.ElementsLookAndFeel;
			lbCategories.LookAndFeel.ParentLookAndFeel = Grid.ElementsLookAndFeel;
			tabControl1.LookAndFeel.ParentLookAndFeel = Grid.ElementsLookAndFeel;
			btNew.LookAndFeel.ParentLookAndFeel = Grid.ElementsLookAndFeel;
			btDelete.LookAndFeel.ParentLookAndFeel = Grid.ElementsLookAndFeel;
			if(categoryForm != null)
				categoryForm.UpdateStyle();
		}
		protected internal static Rectangle CheckCustomizationFormBounds(Rectangle value) {
			if(value == Rectangle.Empty) return value;
			value.Width = Math.Max(MinFormSize.Width, value.Width);
			value.Height = Math.Max(MinFormSize.Height, value.Height);
			value.Location = DevExpress.Utils.ControlUtils.CalcLocation(value.Location, value.Location, value.Size);
			return value;
		}
		public void UpdateSize() {
			if(!Grid.CustomizationFormBounds.IsEmpty)
				this.Size = Grid.CustomizationFormBounds.Size;
		}
		protected static Size MinFormSize { get { return new Size(200, 150); } }
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridCustomizationFormCategoryItemWidth")]
#endif
		public int CategoryItemWidth { get { return lbCategories.ClientRectangle.Width; } }
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridCustomizationFormRowItemWidth")]
#endif
		public int RowItemWidth { get { return lbRows.ClientRectangle.Width; } }
		[ToolboxItem(false)]
		public class RowsListBox : DevExpress.XtraEditors.ListBoxControl {
			public void InvalidateRow(BaseRow row) {
				foreach(BaseListBoxViewInfo.ItemInfo itemInfo in ViewInfo.VisibleItems) {
					BaseRowHeaderInfo rh = (BaseRowHeaderInfo)itemInfo.Item;
					if(rh.Row != row) continue;
					Invalidate(rh.HeaderRect);
					return;
				}
			}
			public new Rectangle ClientRectangle { get { return ViewInfo.ContentRect; } }
			protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
				CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
			}
		}
	}
	internal class VGridNewCategoryForm : XtraForm {
		private LabelControl label1;
		private LabelControl groupBox1;
		private SimpleButton btCancel;
		private SimpleButton btOk;
		private TextEdit tbCaption;
		protected VGridControlBase grid;
		public VGridNewCategoryForm(VGridControlBase grid) {
			InitializeComponent();
			this.Text = VGridLocalizer.Active.GetLocalizedString(VGridStringId.RowCustomizationNewCategoryFormText);
			this.label1.Text = VGridLocalizer.Active.GetLocalizedString(VGridStringId.RowCustomizationNewCategoryFormLabelText);
			this.grid = grid;
			UpdateStyle();
			int indent = label1.Width + 20;
			tbCaption.Left = tbCaption.Left + indent;
			tbCaption.Width = tbCaption.Width - indent;
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.tbCaption = new DevExpress.XtraEditors.TextEdit();
			this.label1 = new DevExpress.XtraEditors.LabelControl();
			this.groupBox1 = new DevExpress.XtraEditors.LabelControl();
			this.btCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btOk = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.tbCaption.Properties)).BeginInit();
			this.SuspendLayout();
			this.tbCaption.EditValue = "";
			this.tbCaption.Location = new System.Drawing.Point(12, 13);
			this.tbCaption.Name = "tbCaption";
			this.tbCaption.Size = new System.Drawing.Size(252, 20);
			this.tbCaption.TabIndex = 0;
			this.tbCaption.TextChanged += new System.EventHandler(this.tbCaption_TextChanged);
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(0, 13);
			this.label1.TabIndex = 1;
			this.groupBox1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.groupBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.groupBox1.LineVisible = true;
			this.groupBox1.Location = new System.Drawing.Point(-2, 33);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(282, 13);
			this.groupBox1.TabIndex = 2;
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(192, 50);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(72, 24);
			this.btCancel.TabIndex = 4;
			this.btCancel.Text = "Cancel";
			this.btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOk.Enabled = false;
			this.btOk.Location = new System.Drawing.Point(104, 50);
			this.btOk.Name = "btOk";
			this.btOk.Size = new System.Drawing.Size(72, 24);
			this.btOk.TabIndex = 3;
			this.btOk.Text = "OK";
			this.AcceptButton = this.btOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.btCancel;
			this.ClientSize = new System.Drawing.Size(280, 81);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbCaption);
			this.Controls.Add(this.btOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "VGridNewCategoryForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.VGridNewCategoryForm_Closing);
			((System.ComponentModel.ISupportInitialize)(this.tbCaption.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected internal void UpdateStyle() {
			LookAndFeel.Assign(grid.ElementsLookAndFeel);
			btOk.LookAndFeel.ParentLookAndFeel = grid.ElementsLookAndFeel;
			btCancel.LookAndFeel.ParentLookAndFeel = grid.ElementsLookAndFeel;
			tbCaption.LookAndFeel.ParentLookAndFeel = grid.ElementsLookAndFeel;
		}
		private void VGridNewCategoryForm_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult == DialogResult.OK)
				e.Cancel = !grid.InternalCustomizationFormCreatingCategory(CategoryCaption);
		}
		private void tbCaption_TextChanged(object sender, System.EventArgs e) {
			btOk.Enabled = (tbCaption.Text != string.Empty);
		}
		protected string CategoryCaption { get { return tbCaption.Text; } }
	}	
}
