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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.WinExplorer;
using DevExpress.XtraGrid.WinExplorer;
namespace DevExpress.XtraGrid.Design {
	public interface IPrintDesigner {
		void ApplyOptions(bool setOptions);
		void HideCaption();
	}
	public class XViewsPrinting {
		#region Init
		DevExpress.XtraGrid.GridControl grid;
		public XViewsPrinting(DevExpress.XtraGrid.GridControl grid) : this(grid, false) {}
		public XViewsPrinting(DevExpress.XtraGrid.GridControl grid, bool setAppearancePreview) {
			this.grid = grid;
			if(grid != null) {
				grid.MainView.BeginUpdate();
				try {
					InitData();
					InitEditors();
					SetEditors();
					InitBands();
					BestFitColumns();
					if(setAppearancePreview) SetAppearancePreview();
				} finally {
					grid.MainView.EndUpdate();
				}
			}
		}
		void InitBands() {
			if(grid.MainView.BaseInfo.ViewName.IndexOf("Band") == -1) return;
			DevExpress.XtraGrid.Views.BandedGrid.BandedGridView bv = grid.MainView as DevExpress.XtraGrid.Views.BandedGrid.BandedGridView;
			if(bv == null) return;
			bv.Bands.Clear();
			bv.Bands.Add();
			for(int i = 0; i < bv.Columns.Count; i ++) {
				bv.Bands[0].Columns.Add(bv.Columns[i]);
				bv.Columns[i].Visible = true;
			}
			bv.Bands.Add();
			bv.Bands[1].Columns.Add(bv.Columns["Unit Price"]);
			bv.Bands[1].Columns.Add(bv.Columns["Discontinued"]);
			bv.Bands[0].Caption = Properties.Resources.PreviewBandNameProduct;
			bv.Bands[1].Caption = Properties.Resources.PreviewBandNameOthers;
		}
		RepositoryItemImageComboBox pi;
		RepositoryItemCheckEdit ce;
		RepositoryItemSpinEdit se;
		RepositoryItemTextEdit te;
		static string[] comboItems = new string[]{Properties.Resources.PreviewCategoryBeverages, Properties.Resources.PreviewCategoryCondiments, Properties.Resources.PreviewCategoryConfections, 
			Properties.Resources.PreviewCategoryDairyProducts, Properties.Resources.PreviewCategoryGrains, Properties.Resources.PreviewCategoryMeat, Properties.Resources.PreviewCategoryProduce};
		void InitEditors() {
			int i = 0;
			pi = new RepositoryItemImageComboBox();
			ce = new RepositoryItemCheckEdit(); 
			se = new RepositoryItemSpinEdit();
			te = new RepositoryItemTextEdit();
			se.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
			se.Mask.EditMask = "c";
			grid.RepositoryItems.Add(pi);
			grid.RepositoryItems.Add(ce);
			grid.RepositoryItems.Add(se);
			foreach(string s in comboItems) {
				pi.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(s, i, i));
				i++;
			}
			pi.SmallImages = Images;
		}
		static ImageList images = null;
		static ImageList Images {
			get {
				if(images == null) {
					images = ResourceImageHelper.CreateImageListFromResources("DevExpress.XtraGrid.Printing.Categories.bmp", typeof(XViewsPrinting).Assembly, new Size(16, 16), Color.Magenta, ColorDepth.Depth32Bit);
				}
				return images;
			}
		}
		static DataSet data = null;
		static DataSet Data {
			get {
				if(data == null) {
					data = new DataSet();
					System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraGrid.Printing.XView.xml");
					data.ReadXml(stream);
					stream.Close();
				}
				return data;
			}
		}
		public object GetDataTable() {
			if(grid.MainView is WinExplorerView) { 
				WinExplorerViewDataSource source = new WinExplorerViewDataSource();
				Image img = Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraGrid.Images.InsertImage_32x32.png"));
				source.Groups.Add(new WinExplorerViewItemGroup() { Name = "Group 1" });
				source.Groups.Add(new WinExplorerViewItemGroup() { Name = "Group 2" });
				for(int i = 0; i < 10; i++) {
					WinExplorerViewItem item = new WinExplorerViewItem() { Text = "Item " + (i + 1).ToString(), Description = "Description text " + (i + 1).ToString(), Group = i < 10 ? source.Groups[0] : source.Groups[1], Image = img };
					source.Items.Add(item);
				}
				return source;
			}
			return Data.Tables[0].Copy();
		}
		void InitData() {
			grid.DataSource = GetDataTable();
			grid.ForceInitialize();
			if(grid.MainView is WinExplorerView)
				return;
			grid.MainView.PopulateColumns();
			ColumnView cv = grid.MainView as ColumnView;
			if(cv == null) return;
			cv.Columns["Product Name"].Caption = Properties.Resources.PreviewColumnProductName;
			cv.Columns["Unit Price"].Caption = Properties.Resources.PreviewColumnUnitPrice;
			cv.Columns["Category"].Caption = Properties.Resources.PreviewColumnCategory;
			cv.Columns["Discontinued"].Caption = Properties.Resources.PreviewColumnDiscontinued;
		}
		#endregion
		#region Set & Update
		void SetEditors() {
			if(grid.MainView is WinExplorerView) {
				((WinExplorerView)grid.MainView).ColumnSet.TextColumn.ColumnEdit = te;
				return;
			}   
			((ColumnView)grid.MainView).Columns["Category"].ColumnEdit = pi;	
			((ColumnView)grid.MainView).Columns["Discontinued"].ColumnEdit = ce;	
			DevExpress.XtraGrid.Columns.GridColumn col = ((ColumnView)grid.MainView).Columns["Unit Price"];
			col.ColumnEdit = se;
			col.DisplayFormat.FormatType = FormatType.Numeric;
			col.DisplayFormat.FormatString = "c";
			col.SummaryItem.SummaryType = SummaryItemType.Sum;
			col.SummaryItem.DisplayFormat = "{0:c}";
		}
		void BestFitColumns() {
			if(grid != null) {
				if(grid.MainView is DevExpress.XtraGrid.Views.Grid.GridView)
					((GridView)grid.MainView).BestFitColumns();
			}
		}
		public void Update() {
			grid.MainView.PopulateColumns();
			BestFitColumns();
			SetEditors();
			InitBands();
		}
		void SetAppearancePreviewGridView(GridView view) {
			view.Columns["Discontinued"].GroupIndex = 0;
			view.ExpandAllGroups();
			view.Columns["Unit Price"].FilterInfo = new DevExpress.XtraGrid.Columns.ColumnFilterInfo("[Unit Price] > 10");
			view.Columns["Product Name"].SummaryItem.SummaryType = SummaryItemType.Count;
			view.OptionsView.ShowFooter = true;
			view.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
		}
		void SetAppearancePreviewCardView(CardView view) {
			view.Columns["Unit Price"].FilterInfo = new DevExpress.XtraGrid.Columns.ColumnFilterInfo("[Unit Price] > 10");
			view.MaximumCardColumns = 2;
			view.OptionsBehavior.AutoHorzWidth = true;
		}
		public void SetAppearancePreview() {
			if(grid == null) return;
			if(grid.MainView is GridView) SetAppearancePreviewGridView(grid.MainView as GridView);
			if(grid.MainView is CardView) SetAppearancePreviewCardView(grid.MainView as CardView);
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class PreviewGrid: DevExpress.XtraGrid.GridControl {
		protected GridView CurrentView {
			get { return this.MainView as GridView; }
		}
		public PreviewGrid(Control parent, string name) {
			this.Enabled = false;
			this.UseDisabledStatePainter = false;
			this.MainView = this.CreateView(name);;
			this.Dock = DockStyle.Fill;
			parent.Controls.Add(this);
			new XViewsPrinting(this);
			CurrentView.FixedLineWidth = 10;
			CurrentView.OptionsBehavior.AutoUpdateTotalSummary = false;
			CurrentView.OptionsBehavior.Editable = false;
			CurrentView.OptionsBehavior.SmartVertScrollBar = false;
			CurrentView.OptionsView.ShowGroupPanel = false;
			CurrentView.OptionsView.ShowIndicator = false;
			CurrentView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			CurrentView.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.view_CustomDrawColumnHeader);
			CurrentView.CustomDrawGroupRow += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.view_CustomDrawGroupRow);
			CurrentView.CustomDrawRowFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.view_CustomDrawRowFooterCell);
			CurrentView.CustomDrawFooter += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.view_CustomDrawFooter);
			CurrentView.CustomDrawFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.view_CustomDrawFooterCell);
			CurrentView.CustomDrawRowFooter += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.view_CustomDrawRowFooter);
			CurrentView.OptionsSelection.EnableAppearanceFocusedRow = false;
			CurrentView.OptionsSelection.EnableAppearanceFocusedCell = false;
			for(int i = 0; i < CurrentView.Columns.Count; i++)
				CurrentView.Columns[i].OptionsFilter.AllowFilter = false;
			CurrentView.OptionsView.AutoCalcPreviewLineCount = true;
			CurrentView.CalcPreviewText += new DevExpress.XtraGrid.Views.Grid.CalcPreviewTextEventHandler(this.view_CalcPreviewText);
			CurrentView.GroupSummary.Add(SummaryItemType.Count, "Product Name", CurrentView.Columns["Product Name"]);
			GridColumn col = CurrentView.Columns["Unit Price"];
			CurrentView.GroupSummary.Add(SummaryItemType.Max, "Unit Price", col, CurrentView.GetSummaryFormat(col, SummaryItemType.Max), col.DisplayFormat.Format);
		}
		#region Custom Draw
		private void view_CalcPreviewText(object sender, DevExpress.XtraGrid.Views.Grid.CalcPreviewTextEventArgs e) {
			GridView view = sender as GridView;
			if(view == null) return;
			if(e.RowHandle >= 0) {
				DataRow row = view.GetDataRow(e.RowHandle);
				if(row != null && (decimal)row["Unit Price"] > 30)
					e.PreviewText = string.Format(Properties.Resources.PreviewRowDescription, row["Product Name"]);	
			}
		}
		public void DrawBrick(Rectangle r, AppearanceObject app, GraphicsCache cache, Graphics g, string caption) {
			DrawBrick(r, app, cache, g, caption, 1, 1);
		}
		public void DrawBrick(Rectangle r, AppearanceObject app, GraphicsCache cache, Graphics g, string caption, int dx, int dy) {
			app.FillRectangle(cache, r);
			if(g != null) {
				Color borderColor = this.MainView.PaintAppearance.GetAppearance("HorzLine").BackColor;
				Pen p = new Pen(borderColor);
				r.Height -= dx;
				r.Width -= dy;
				using(p) {
					g.DrawRectangle(p, r);
				}
			}
			r.Inflate(-3, 0);
			if(caption != "")
				app.DrawString(cache, caption, r);
		}
		protected void view_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e) {
			DrawBrick(e.Bounds, e.Appearance, e.Cache, e.Graphics, e.Info.Caption, 1, 0);
			e.Handled = true;
		}
		protected void view_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e) {
			DrawBrick(e.Bounds, e.Appearance, e.Cache, e.Graphics, "");
			e.Handled = true;
		}
		protected void view_CustomDrawRowFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e) {
			DrawBrick(e.Bounds, e.Appearance, e.Cache, null, "");
			e.Handled = true;
		}
		protected void view_CustomDrawRowFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e) {
			DrawBrick(e.Bounds, e.Appearance, e.Cache, e.Graphics, e.Info.DisplayText);
			e.Handled = true;
		}
		protected void view_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e) {
			DrawBrick(e.Bounds, e.Appearance, e.Cache, e.Graphics, e.Info.DisplayText);
			e.Handled = true;
		}
		protected void view_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e) {
			DrawBrick(e.Bounds, e.Appearance, e.Cache, null, ((GridView)this.MainView).GetGroupRowDisplayText(e.RowHandle));
			e.Handled = true;
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class PreviewCardGrid: DevExpress.XtraGrid.GridControl {
		protected CardView CurrentView {
			get { return this.MainView as CardView; }
		}
		public PreviewCardGrid(Control parent) {
			this.Enabled = false;
			this.UseDisabledStatePainter = false;
			this.MainView = this.CreateView("CardView");;
			this.Dock = DockStyle.Fill;
			parent.Controls.Add(this);
			new XViewsPrinting(this);
			CurrentView.FocusedCardTopFieldIndex = 0;
			CurrentView.Images = null;
			CurrentView.OptionsView.ShowHorzScrollBar = false;
			CurrentView.OptionsView.ShowLines = false;
			CurrentView.OptionsView.ShowCardExpandButton = false;
			CurrentView.OptionsView.ShowQuickCustomizeButton = false;
		}
	}
	[ToolboxItem(false)]
	public class PreviewLayoutGrid : DevExpress.XtraGrid.GridControl {
		protected LayoutView CurrentView {
			get { return this.MainView as LayoutView; }
		}
		public PreviewLayoutGrid(Control parent) {
			this.Enabled = false;
			this.UseDisabledStatePainter = false;
			this.MainView = this.CreateView("LayoutView"); ;
			this.Dock = DockStyle.Fill;
			parent.Controls.Add(this);
			new XViewsPrinting(this);
			CurrentView.VisibleRecordIndex = 0;
			CurrentView.Images = null;
			CurrentView.OptionsView.ShowCardLines = false;
			CurrentView.OptionsBehavior.ScrollVisibility = ScrollVisibility.Never;
			CurrentView.OptionsView.ShowCardExpandButton = false;
			CurrentView.OptionsView.ShowHeaderPanel = false;
		}
	}
}
