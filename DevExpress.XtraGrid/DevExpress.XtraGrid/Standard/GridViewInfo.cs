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
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors.Repository;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Skins;
namespace DevExpress.XtraGrid.Views.Grid.ViewInfo {
	internal class NullGridViewInfo : GridViewInfo {
		public NullGridViewInfo(GridView gridView) : base(gridView) { }
		public override bool IsNull { get { return true; } }
	}
	public class GridCellId : CellId
	{
		public GridCellId(BaseView view, GridCellInfo cellInfo) : base(view.GetRow(cellInfo.RowHandle), cellInfo.Column ) { }
	}
	public class GridViewInfo : ColumnViewInfo, ISupportXtraAnimation, IAsyncImageLoaderClient {
		public const int GroupLineDX = 6,
			GroupLineDY = 10,
			GroupLineDWidth = 100;
		public GridColumnsInfo ColumnsInfo;
		public GridRowInfoCollection RowsInfo;
		public GridRowFooterInfo FooterInfo;
		public int LevelIndent;
		public bool AllowUpdateDetails;
		public int GroupPanelColumnMaxWidth = 200;
		public bool ForceDisplayHeaderFilter = false;
		GridRowInfoCollection cachedRows;
		GroupPanelInfo groupPanel;
		GridViewRects viewRects;
		AutoWidthCalculator widthCalculator;
		bool showDetailButtons, shouldInflateRowOnInvalidate;
		int columnRowHeight, groupFooterCellHeight, groupFooterHeight;
		GridRowsLoadInfo rowsLoadInfo;
		protected GridColumn fFixedLeftColumn, fFixedRightColumn;
		Hashtable autoRowHeightCache;
		Rectangle editFormBounds;
		int editFormRequiredHeight;
		int groupRowMinHeight, groupRowDefaultHeight, minRowHeight, footerCellHeight;
		int actualMinRowHeight = -1;
		ScrollBarPresence hScrollBarPresence, vScrollBarPresence;
		Size plusMinusButtonSize, detailButtonSize;
		TextOptions defaultPreviewOptions;
		GridDataRowInfo emptyDataRow;
		GridGroupRowInfo emptyGroupRow;
		GridViewInfoCellMerger cellMerger;
		public GridViewInfo(GridView gridView) : base(gridView) {
			this.editFormBounds = Rectangle.Empty;
			this.shouldInflateRowOnInvalidate = false;
			this.cellMerger = new GridViewInfoCellMerger(this);
			this.emptyDataRow = CreateRowInfo(0, 0) as GridDataRowInfo;
			this.emptyGroupRow = CreateRowInfo(-1, 0) as GridGroupRowInfo;
			this.showDetailButtons = false;
			this.cachedRows = null;
			this.AllowUpdateDetails = true;
			this.autoRowHeightCache = new Hashtable();
			this.rowsLoadInfo = null;
			this.hScrollBarPresence = this.vScrollBarPresence = ScrollBarPresence.Unknown;
			this.viewRects = CreateViewRects();
			this.columnRowHeight = 20;
			this.groupRowDefaultHeight = this.groupRowMinHeight = this.groupFooterCellHeight = this.groupFooterHeight = this.footerCellHeight = 15;
			this.minRowHeight = 20;
			this.fFixedLeftColumn = fFixedRightColumn = null;
			this.widthCalculator = CreateWidthCalculator();
			this.groupPanel = new GroupPanelInfo(this);
			this.defaultPreviewOptions = new TextOptions(HorzAlignment.Near, VertAlignment.Top, WordWrap.Wrap, Trimming.EllipsisCharacter);
			CalcConstants();
			LockSelectionInfo();
			try {
				Clear();
			}
			finally {
				UnlockSelectionInfo();
			}
		}
		public virtual bool AllowPartialGroups { get { return View.AllowPartialGroups; } }
		Padding padding = new Padding(1, 1, 1, 1);
		public virtual Padding CellPadding { 
			get {
				Padding res = IsSkinned ? GetSkinnedCellPadding() : padding;
				Padding user = UserCellPadding;
				return new Padding(res.Left + user.Left, res.Top + user.Top, res.Right + user.Right, res.Bottom + user.Bottom);
			} 
			set { padding = value; } 
		}
		public Padding UserCellPadding {
			get { return View.UserCellPadding; }
		}
		[Obsolete]
		public int CellValueVIndent { get { return CellPadding.Vertical; } }
		[Obsolete]
		public int CellValueHIndent { get { return CellPadding.Horizontal; } }
		protected override int CellVertIndent { get { return CellPadding.Vertical; } }
		public override Rectangle Bounds { get { return ViewRects.Bounds; } }
		public override Rectangle ClientBounds { get { return ViewRects.Client; } }
		public bool ShouldInflateRowOnInvalidate { get { return shouldInflateRowOnInvalidate; } }
		public GridViewInfoCellMerger CellMerger { get { return cellMerger; } }
		public TextOptions DefaultPreviewOptions { get { return defaultPreviewOptions; } }
		protected override void UpdatePaintAppearanceDefaults() {
			if(View.IsColumnHeaderAutoHeight) PaintAppearance.HeaderPanel.TextOptions.WordWrap = WordWrap.Wrap;
			PaintAppearance.Preview.TextOptions.UpdateDefaultOptions(DefaultPreviewOptions);
		}
		protected GridDataRowInfo EmptyDataRow { get { return emptyDataRow; } }
		protected GridGroupRowInfo EmptyGroupRow { get { return emptyGroupRow; } }
		public new GridViewAppearances PaintAppearance { get { return base.PaintAppearance as GridViewAppearances; } }
		protected override BaseAppearanceCollection CreatePaintAppearances() { return new GridViewAppearances(View); }
		public bool ShowGroupButtons { get { return View != null && View.OptionsView.ShowGroupExpandCollapseButtons; } }
		public bool ShowDetailButtons { get { return showDetailButtons; } set { showDetailButtons = value; } }
		public GridRowInfoCollection CachedRows { get { return cachedRows; } }
		public new GridSelectionInfo SelectionInfo { get { return base.SelectionInfo as GridSelectionInfo; } }
		public GridViewRects ViewRects { get { return viewRects; } }
		public virtual GridColumn FixedLeftColumn { get { return fFixedLeftColumn; } }
		public virtual GridColumn FixedRightColumn { get { return fFixedRightColumn; } }
		public virtual AutoWidthCalculator WidthCalculator { get { return widthCalculator; } }
		public virtual GridPainter Painter { get { return View.Painter as GridPainter; } }
		public virtual int PreviewIndent { 
			get { 
				if(View == null) return 20;
				return View.PreviewIndent == -1 ? 20 : View.PreviewIndent; 
			}
		}
		public virtual void CalcConstants() {
			if(IsNull) return;
			ObjectInfoArgs e = new OpenCloseButtonInfoArgs(null, Rectangle.Empty, false, null, ObjectState.Normal);
			this.plusMinusButtonSize = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, Painter.ElementsPainter.OpenCloseButton, e).Size;
			this.detailButtonSize = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, Painter.ElementsPainter.DetailButton, 
				new DetailButtonObjectInfoArgs()).Size;
			this.LevelIndent = CalcLevelIndent();
		}
		public virtual int CalcLevelIndent() {
			if(View.LevelIndent == -1) return PlusMinusButtonSize.Width + 8;
			return View.LevelIndent;
		}
		public Size DetailButtonSize { get { return detailButtonSize; } }
		public Size PlusMinusButtonSize { get { return plusMinusButtonSize; } }
		public virtual GridViewRects CreateViewRects() {
			return new GridViewRects(this);
		}
		public virtual GridHitInfo CreateHitInfo() { return new GridHitInfo(); } 
		protected virtual AutoWidthCalculator CreateWidthCalculator() {
			return new GridAutoWidthCalculator(View);
		}
		protected override BaseSelectionInfo CreateSelectionInfo() {
			return new GridSelectionInfo(View);
		}
		public override void Clear() {
			CellMerger.Clear();
			GroupPanel.Clear();
			ColumnsInfo = new GridColumnsInfo();
			RowsInfo = new GridRowInfoCollection();
			FooterInfo = new GridRowFooterInfo();
			IsReady = false;
			base.Clear();
		}
		public Rectangle GetScrollableBounds(bool ignoreForced) {
			Rectangle res = ViewRects.Rows;
			GridRowInfo row = RowsInfo.GetFirstScrollableRow(View);
			GridRowInfo nonScroll = RowsInfo.GetLastNonScrollableRow(View, ignoreForced);
			if(row != null) {
				int top = res.Top;
				if(nonScroll != null) {
					if(nonScroll.TotalBounds.Bottom > top) top = nonScroll.TotalBounds.Bottom;
				}
				res.Height = res.Bottom - top;
				res.Y = top;
			}
			return res;
		}
		public new GridView View { get { return base.View as GridView; } }
		public virtual GroupPanelInfo GroupPanel { get { return groupPanel; } }
		public virtual int ColumnRowHeight { get { return columnRowHeight; } } 
		public virtual int FooterCellHeight { get { return footerCellHeight; } } 
		public virtual int GroupFooterCellHeight { get { return groupFooterCellHeight; } } 
		public virtual int GroupFooterHeight { get { return groupFooterHeight; } } 
		public virtual int GroupRowMinHeight { get { return groupRowMinHeight; } set { groupRowMinHeight = value; } }
		public virtual int GroupRowDefaultHeight { get { return groupRowDefaultHeight; } set { groupRowDefaultHeight = value; } }
		public virtual int MinRowHeight { get { return minRowHeight; } }
		public virtual int GetActualMinRowHeight(bool isGroupRow) {
			if(isGroupRow) {
				return GroupRowMinHeight + (View.GetShowHorizontalLines() ? 1 : 0);
			}
			return ActualDataRowMinRowHeight;
		}
		public virtual int ActualDataRowMinRowHeight { 
			get {
				if(actualMinRowHeight == -1) actualMinRowHeight = CalcActualMinRowHeight();
				return actualMinRowHeight;
			}
		}
		protected virtual int CalcActualMinRowHeight() {
			int res = MinRowHeight +
				(View.GetShowHorizontalLines() ? 1 : 0) +
				View.RowSeparatorHeight + (View.RowSeparatorHeight > 0 ? (View.GetShowHorizontalLines() ? 1 : 0) : 0) + CalcSkinRowPadding(null);
			return res;
		}
		public virtual GridRowsLoadInfo RowsLoadInfo { get { return rowsLoadInfo; } }
		public virtual Hashtable AutoRowHeightCache {
			get { return autoRowHeightCache; }
		}
		public Point MousePosition {
			get {
				if(GridControl == null || !IsReady || !GridControl.IsHandleCreated) return BaseViewInfo.EmptyPoint;
				return GridControl.PointToClient(Control.MousePosition);
			}
		}
		public GridRowInfo GetGridRowInfo(int rowHandle) {
			if(!IsReady) return null;
			return RowsInfo.GetInfoByHandle(rowHandle);
		}
		public GridCellInfo GetGridCellInfo(GridHitInfo hi) {
			if(hi.CellInfo != null) return hi.CellInfo;
			if(hi.Column == null) return null;
			return GetGridCellInfo(hi.RowHandle, hi.Column);
		}
		public GridCellInfo GetGridCellInfo(int rowHandle, GridColumn column) {
			GridDataRowInfo ri = GetGridRowInfo(rowHandle) as GridDataRowInfo;
			if(ri == null) return null;
			foreach(GridCellInfo cell in ri.Cells) {
				if(cell.ColumnInfo.Type == GridColumnInfoType.Column) {
					if(cell.Column == column) 
						return cell;
				}
			}
			return null;
		}
		public virtual int GetColumnLeftCoord(GridColumn lookColumn) {
			if(lookColumn.VisibleIndex < 0) return 0;
			int res = 0;
			foreach(GridColumn column in View.VisibleColumns) {
				if(column == lookColumn) return res;
				res += column.VisibleWidth;
			}
			return res;
		}
		public string GetGroupPanelText() {
			if(View.GroupPanelText == string.Empty)
				return GridLocalizer.Active.GetLocalizedString(GridStringId.GridGroupPanelText);
			return View.GroupPanelText;
		}
		public string GetNewItemRowText() {
			if(View.NewItemRowText == string.Empty)
				return GridLocalizer.Active.GetLocalizedString(GridStringId.GridNewRowText);
			return View.NewItemRowText;
		}
		protected internal override Rectangle GetTargetDragRect(int baseHeight) {
			Rectangle rect = ViewRects.Client;
			rect.Height = baseHeight;
			rect.Height += ViewRects.ColumnPanel.Height;
			rect.Height += ViewRects.GroupPanel.Height;
			return rect;
		}
		protected virtual int CalcGroupPanelHeight() {
			int result = 20;
			ArrayList list = PreCalcGroupPanelRows();
			if(list == null && View.SortInfo.GroupCount == 0) {
				Graphics g = GInfo.AddGraphics(null);
				try {
					result = Convert.ToInt32(PaintAppearance.GroupPanel.CalcTextSize(g, 
						GetGroupPanelText(), 0).Height) + 20;
				}
				finally {
					GInfo.ReleaseGraphics();
				}
				return result;
			}
			if(list != null) {
				result = 0;
				GridViewInfo vi = this;
				for(int n = -1; n < list.Count; n ++) {
					if(n == -1) vi = this;
					else vi = (list[n] as GridView).ViewInfo as GridViewInfo;
					if(!vi.IsReady) {
						vi.CalcRectsConstants();
					}
					result += vi.ColumnRowHeight + GroupLineDY;
				}
			}
			else 
				result = GroupLineDY * 2 + ColumnRowHeight + ((ColumnRowHeight / 2) * (IsSingleRowGroupPanel ? 0 : (View.SortInfo.GroupCount - 1)));
			return result;
		}
		public override bool AllowTabControl { get { return View.OptionsDetail.ShowDetailTabs; } }
		public override void PrepareCalcRealViewHeight(Rectangle viewRect, BaseViewInfo oldInfo) {
			GridViewInfo oldViewInfo = oldInfo as GridViewInfo;
			if(oldViewInfo != null) {
				this.IsReady = oldViewInfo.IsReady;
				this.ViewRects.ColumnTotalWidth = oldViewInfo.ViewRects.ColumnTotalWidth;
				this.ViewRects.IndicatorWidth = oldViewInfo.ViewRects.IndicatorWidth;
				this.ViewRects.ColumnPanel = oldViewInfo.ViewRects.ColumnPanel;
			}
		}
		public override int CalcRealViewHeight(Rectangle viewRect) {
			int result = viewRect.Height;
			bool prevAllow = AllowUpdateDetails;
			this.AllowUpdateDetails = false;
			StartRealHeightCalculate();
			try {
				Calc(null, viewRect);
				if(!ViewRects.EmptyRows.IsEmpty && VScrollBarPresence != ScrollBarPresence.Visible) {
					result -= ViewRects.EmptyRows.Height;
				}
			}
			finally {
				this.AllowUpdateDetails = prevAllow;
				EndRealHeightCalculate();
			}
			return result;
		}
		protected virtual int GetFooterPanelHeight() {
			int h = 10;
			Graphics g = GInfo.AddGraphics(null);
			try {
				FooterPanelInfoArgs info = new FooterPanelInfoArgs(GInfo.Cache, ColumnPanelRowCount * GetMaxColumnFooterCount(), FooterCellHeight);
				h = Painter.ElementsPainter.FooterPanel.CalcObjectMinBounds(info).Height;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return Math.Max(h,  ScaleVertical(View.FooterPanelHeight));
		}
		protected internal virtual int GetMaxColumnFooterCount() {
			if(View.TotalSummary.ActiveCount == 0) return 1;
			int max = 1;
			for(int n = 0; n < View.VisibleColumns.Count; n++) {
				max = Math.Max(View.VisibleColumns[n].Summary.ActiveCount, max);
			}
			return max;
		}
		public override Rectangle ViewCaptionBounds { get { return ViewRects.ViewCaption; } }
		public override ObjectPainter FilterPanelPainter { get { return Painter.ElementsPainter.FilterPanel; } }
		public override ObjectPainter ViewCaptionPainter { get { return Painter.ElementsPainter.ViewCaption; } }
		protected virtual ArrayList CalcGridChildrenList() {
			if(ChildGridView == null || GridControl == null || View != GridControl.DefaultView) return null;
			ArrayList list = new ArrayList();
			GridViewInfo viewInfo = this;
			GridView view;
			while((view = viewInfo.ChildGridView) != null) {
				list.Add(view);
				viewInfo = view.ViewInfo as GridViewInfo;
				if(viewInfo == null) return null;
			}
			return list;
		}
		protected virtual ArrayList PreCalcGroupPanelRows() {
			if(!View.OptionsView.ShowChildrenInGroupPanel) return null;
			ArrayList list = CalcGridChildrenList();
			if(list == null || list.Count == 0) return null;
			ArrayList res = new ArrayList();
			bool gpanelVisible = false;
			for(int n = list.Count - 1; n >= 0; n--) {
				GridView view = list[n] as GridView;
				if(gpanelVisible || view.SortInfo.GroupCount > 0) {
					gpanelVisible = true;
					res.Insert(0, view);
				}
			}
			if(res.Count == 0) return null;
			return res;
		}
		protected virtual void CalcGroupDrawInfo() {
			GroupPanel.Clear();
			if(!View.OptionsView.ShowGroupPanel) return;
			int left = ViewRects.GroupPanel.Left,
				top  = ViewRects.GroupPanel.Top;
			Rectangle bounds = ViewRects.GroupPanel;
			ArrayList list = PreCalcGroupPanelRows();
			if(list == null && View.SortInfo.GroupCount == 0) return;
			GroupPanelRow row = CalcGroupPanelRowDrawInfo(bounds, list != null, list != null || IsSingleRowGroupPanel);
			GroupPanel.Rows.Add(row);
			if(list != null) {
				foreach(GridView view in list) {
					GridViewInfo vi = view.ViewInfo as GridViewInfo;
					bounds.Y = row.Bounds.Bottom;
					if(!vi.IsReady) {
						vi.CalcRectsConstants();
					}
					row = vi.CalcGroupPanelRowDrawInfo(bounds, true, true);
					GroupPanel.Rows.Add(row);
				}
			}
		}
		protected virtual bool IsSingleRowGroupPanel {  get { return View.OptionsView.ShowGroupPanelColumnsAsSingleRow; } }
		protected virtual GroupPanelRow CalcGroupPanelRowDrawInfo(Rectangle bounds, bool showCaption, bool lineStyle) {
			GroupPanelRow row = new GroupPanelRow(this);
			int directionDelta = IsRightToLeft ? -1 : 1;
			row.LineStyle = lineStyle;
			int left = bounds.X,
				top  = bounds.Y, height = 0;
			if(IsRightToLeft) left = bounds.Right;
			Rectangle r = bounds;
			left += GroupLineDX * directionDelta;
			if(showCaption)
				row.RowCaption = View.GetViewCaption() + ":";
			for(int i = 0; i < View.SortInfo.GroupCount + (showCaption ? 1 : 0); i++) {
				GridColumn col = null;
				if(showCaption) {
					if(i != 0) {
						col = View.SortInfo[i - 1].Column;
						if(col == null) continue;
					}
				} else {
					col = View.SortInfo[i].Column;
				}
				GridColumnInfoArgs ci = CreateColumnInfo(col);
				ci.HtmlContext = View;
				ci.HeaderPosition = HeaderPositionKind.Special;
				ci.Info.ValidateCoord = false;
				ci.Info.InGroup = true;
				int Y = top + GroupLineDY + i * (ColumnRowHeight / 2 );
				int X = left;
				if(row.LineStyle) Y = top + GroupLineDY / 2;
				Rectangle cBounds = new Rectangle(left, Y, GroupLineDWidth, ColumnRowHeight);
				if(col == null) {
					ci.Type = GridColumnInfoType.CaptionColumn;
					ci.Caption = row.RowCaption;
					cBounds.Width = CalcColumnBestBounds(ci).Width;
				} else {
					cBounds.Width = Math.Min(CalcColumnBestBounds(ci).Width, GroupPanelColumnMaxWidth);
				}
				if(IsRightToLeft) cBounds.X -= cBounds.Width;
				ci.Bounds = cBounds;
				CalcColumnInfo(ci, ref X, false);
				if(col != null)
					row.ColumnsInfo.Add(ci);
				else
					row.CaptionInfo = ci;
				height = Math.Max((ci.Bounds.Bottom - top)+ GroupLineDY / 2, height);
				if(IsRightToLeft)
					left -= (ci.Bounds.Width + GroupLineDX);
				else
				left = ci.Bounds.Right + GroupLineDX;
			}
			r.Height = height;
			row.Bounds = r;
			return row;
		}
		public virtual GridView ChildGridView { get { return View.ChildGridView; } }
		protected virtual int CalcTotalColumnWidth() {
			int totalWidth = 0;
			for(int i = 0; i < View.VisibleColumns.Count; i++) {
				totalWidth += View.VisibleColumns[i].VisibleWidth;
			}
			return totalWidth;
		}
		protected virtual int CalcRestColumnsWidth(GridColumn column) {
			if(column == null) return 0;
			int res = 0;
			for(int n = 0; n < View.VisibleColumns.Count; n++) {
				GridColumn col = View.VisibleColumns[n];
				if(col == column || column == null) {
					res += col.VisibleWidth;
					column = null;
				}
			}
			return res;
		}
		protected virtual bool AllowBehindColumn { get { return true; } }
		protected virtual bool GetHeaderIsTopMost(GridColumnInfoArgs info) {
			return true;
		}
		public virtual bool IsShowHeaders { get { return View.OptionsView.ShowColumnHeaders; } }
		protected virtual void CalcColumnsDrawInfo() {
			int left = ViewRects.ColumnPanel.Left,
				top = ViewRects.ColumnPanel.Top, firstIndex;
			Size size = Size.Empty;
			firstIndex = ViewRects.IndicatorWidth > 0 ? -1 : 0;
			bool leftCoordSubstracted = false;
			int direction = 1;
			if(IsRightToLeft) {
				left = ViewRects.ColumnPanel.Right;
				direction = -1;
			}
			if(FixedLeftColumn == null) {
				left -= View.LeftCoord * direction;
				leftCoordSubstracted = true;
			}
			int count = View.VisibleColumns.Count + (AllowBehindColumn ? 1 : 0);
			GridColumnInfoArgs prevColumn = null;
			for(int i = firstIndex; i < count; i++) {
				GridColumn column = i > -1 ? View.GetVisibleColumn(i) : null;
				GridColumnInfoArgs ci = CreateColumnInfo(column);
				ci.HtmlContext = View;
				if(i == -1) ci.Type = GridColumnInfoType.Indicator;
				ci.Info.CellIndex = i;
				if(i == View.VisibleColumns.Count) 
					ci.Type = GridColumnInfoType.BehindColumn;
				if(column != null && column.Fixed != FixedStyle.Left) {
					if(!leftCoordSubstracted) {
						left -= View.LeftCoord * direction;
						leftCoordSubstracted = true;
					}
				}
				if(column != null && column == FixedRightColumn) {
					int w = CalcRestColumnsWidth(column);
					if(IsRightToLeft) {
						left -= View.FixedLineWidth;
						if(left - w < ViewRects.ColumnPanel.Left) {
							left = ViewRects.ColumnPanel.Left + w;
						}
						if(prevColumn != null && prevColumn.Bounds.Width > 0) prevColumn.TrueBounds = new Rectangle(prevColumn.Bounds.X - 1, prevColumn.Bounds.Y, prevColumn.Bounds.Width + 1, prevColumn.Bounds.Height);
					}
					else {
						left += View.FixedLineWidth;
						if(left + w > ViewRects.ColumnPanel.Right) {
							left = ViewRects.ColumnPanel.Right - w;
						}
						if(prevColumn != null && prevColumn.Bounds.Width > 0) prevColumn.TrueBounds = new Rectangle(prevColumn.Bounds.X, prevColumn.Bounds.Y, prevColumn.Bounds.Width + 1, prevColumn.Bounds.Height);
					}
				}
				if(IsRightToLeft)
					ci.Bounds = new Rectangle(left - (column == null ? 0 : column.visibleWidth), top, column == null ? 0 : column.VisibleWidth, ColumnRowHeight);
				else
					ci.Bounds = new Rectangle(left, top, column == null ? 0 : column.VisibleWidth, ColumnRowHeight);
				ci.Column = column;
				CalcColumnInfo(ci, ref left, i == View.VisibleColumns.Count - 1);
				if(column != null && column == FixedLeftColumn) {
					left += FixedLineWidth * direction;
					if(IsRightToLeft)
						ci.TrueBounds = new Rectangle(ci.Bounds.X - 1, ci.Bounds.Y, ci.Bounds.Width + 1, ci.Bounds.Height);
					else
						ci.TrueBounds = new Rectangle(ci.Bounds.X, ci.Bounds.Y, ci.Bounds.Width + 1, ci.Bounds.Height);
				}
				if(ci.Bounds.Width > 0) ColumnsInfo.Add(ci);
				prevColumn = ci;
			}
			int start = 0;
			if(ColumnsInfo.Count == 0) return;
			if(ColumnsInfo.Count == 1) {
				ColumnsInfo[0].HeaderPosition = HeaderPositionKind.Left;
				return;
			}
			for(int n = start; n < ColumnsInfo.Count; n++) {
				if(n == start) ColumnsInfo[n].HeaderPosition = HeaderPositionKind.Left;
				if(n == ColumnsInfo.Count - 1 && (ColumnsInfo[n].Column == null || ColumnsInfo[n].Column.VisibleIndex == View.VisibleColumns.Count - 1)) {
					if(AllowUseRightHeaderKind) ColumnsInfo[n].HeaderPosition = HeaderPositionKind.Right;
				}
			}
		}
		protected Size CalcColumnBestBounds(GridColumnInfoArgs ci) {
			int res = 0;
			ci.Bounds = new Rectangle(0, 0, 200, ColumnRowHeight);
			CalcColumnInfo(ci, ref res);
			Size size = ci.Bounds.Size;
			GInfo.AddGraphics(null);
			try {
				ci.Cache = GInfo.Cache;
				if(Painter != null)
				size = Painter.ElementsPainter.Column.CalcObjectMinBounds(ci).Size;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return size;
		}
		protected Size CalcColumnBestBounds(GridColumn column) {
			return CalcColumnBestBounds(GridColumnInfoType.Column, column, column.GetCaption());
		}
		protected virtual Size CalcColumnBestBounds(GridColumnInfoType type, GridColumn column, string caption) {
			string realCaption = column != null ? column.GetCaption() : caption;
			GridColumnInfoArgs ci = CreateColumnInfo(column);
			ci.HtmlContext = View;
			ci.Caption = realCaption;
			ci.Type = type;
			return CalcColumnBestBounds(ci);
		}
		protected void CalcColumnInfo(GridColumnInfoArgs ci, ref int lastLeft) {
			CalcColumnInfo(ci, ref lastLeft, false);
		}
		protected virtual void CalcColumnInfo(GridColumnInfoArgs ci, ref int lastLeft, bool lastColumn) {
			int direction = IsRightToLeft ? -1 : 1;
			Size size = ci.Bounds.Size;
			if(size.Width == 0)
				size.Width = (ci.Column == null ? 100 : ci.Column.VisibleWidth);
			if(size.Height == 0) size.Height = ColumnRowHeight;
			ci.Bounds = new Rectangle(ci.Bounds.Location, size);
			ci.IsTopMost = GetHeaderIsTopMost(ci);
			AppearanceObject app = new AppearanceObject();
			AppearanceHelper.Combine(app, new AppearanceObject[] { ci.Column == null ? null : ci.Column.AppearanceHeader, PaintAppearance.HeaderPanel});
			ci.SetAppearance(app);
			Rectangle bounds = ci.Bounds;
			switch(ci.Type) {
				case GridColumnInfoType.Indicator :
					if(IsRightToLeft) {
						bounds.X = ViewRects.ColumnPanel.Right - ViewRects.IndicatorWidth;
					}
					else {
						bounds.X = ViewRects.ColumnPanel.Left;
					}
					bounds.Width  = ViewRects.IndicatorWidth;
					lastLeft += bounds.Width * direction;
					break;
				case GridColumnInfoType.Column:
					if(lastColumn) {
						if(IsRightToLeft)
							lastLeft++;
						else
							bounds.Width += 1;
					}
					break;
				case GridColumnInfoType.BehindColumn :
					if(IsRightToLeft) {
						bounds.Width = lastLeft - ViewRects.ColumnPanelActual.Left;
						bounds.X = ViewRects.ColumnPanelActual.Left - 1;
						if(bounds.Width == 1) bounds = Rectangle.Empty;
					}
					else {
						bounds.Width = (ViewRects.ColumnPanelActual.Right  - lastLeft) + 1;
						if(bounds.Width == 1) bounds = Rectangle.Empty;
					}
					break;
			}
			ci.Bounds = bounds;
			if(ci.Type == GridColumnInfoType.CaptionColumn) {
				Painter.ElementsPainter.Column.CalcObjectBounds(ci);
				return;
			}
			if(ci.Type != GridColumnInfoType.Column) return;
			if(bounds.Width > 0) {
				lastLeft += ci.Bounds.Width * direction;
				if(ci.Info.ValidateCoord) {
					if(ci.Bounds.Right  < ViewRects.ColumnPanelActual.Left ||
						ci.Bounds.Left > ViewRects.ColumnPanelActual.Right) {
						bounds.Width = 0;
						ci.Bounds = bounds;
						return;
					}
				}
			}
			if(Painter != null)
				Painter.ElementsPainter.Column.CalcObjectBounds(ci);
		}
		protected internal virtual void CalcColumnInfoState(GridColumnInfoArgs ci) {
			if(ci.Column == null) return;
			ci.State = ObjectState.Normal;
			if(ci.Info.AllowEffects) {
				if((ci.Info.InGroup && ci.Column == SelectionInfo.PressedColumnInGroup) || (ci.Column == SelectionInfo.PressedColumn)) {
					ci.State = ObjectState.Pressed;
				}
				if(ci.State == ObjectState.Normal) {
					if(SelectionInfo.HotTrackedColumn == ci.Column && ci.Info.InGroup == SelectionInfo.HotTrackedInfo.InGroupPanel) ci.State = ObjectState.Hot;
				}
				ci.DesignTimeSelected = false;
				if(ci.Column != null) {
					ci.DesignTimeSelected = ci.Column.GetSelectedInDesigner();;
				}
			}
			if(ci.InnerElements == null) return;
			bool changed = false;
			foreach(DrawElementInfo info in ci.InnerElements) {
				ObjectInfoArgs args = info.ElementInfo;
				SortedShapeObjectInfoArgs si = args as SortedShapeObjectInfoArgs;
				if(si != null && ci.Column != null) {
					si.Ascending = ci.Column.SortOrder == DevExpress.Data.ColumnSortOrder.Ascending;
					if(View.GroupSummarySortInfo.Count > 0) {
						View.GroupSummarySortInfo.Update(si, ci.Column);
					}
				}
				GridFilterButtonInfoArgs fi = args as GridFilterButtonInfoArgs;
				if(fi == null) continue;
				fi.State = ObjectState.Normal;
				fi.Filtered = View.IsColumnFiltered(ci.Column);
				fi.SetAppearance(fi.Filtered ? PaintAppearance.ColumnFilterButtonActive : PaintAppearance.ColumnFilterButton);
				if(ci.Info.AllowEffects) {
					bool pressed = ci.Column != null && ci.Column == SelectionInfo.PressedColumnFilter;
					bool selected = ci.Column != null && ci.Column == SelectionInfo.HotTrackedColumnFilterButton && 
						ci.Info.InGroup == SelectionInfo.HotTrackedColumnFilterButtonInGroup;
					if(View.FilterPopup != null && View.FilterPopup.Column == ci.Column && ci.IsEquals(View.FilterPopup.Creator)) pressed = true;
					if(pressed)
						fi.State = ObjectState.Pressed;
					else {
						if(selected) fi.State = ObjectState.Hot;
					}
				}
				bool newVisible = fi.Filtered || fi.State != ObjectState.Normal || ci.State != ObjectState.Normal || ForceDisplayHeaderFilter || ci.AutoHeight;
				if(newVisible != info.Visible) changed = true;
				info.Visible = newVisible;
			}
			if(changed) 
				Painter.ElementsPainter.Column.CalcObjectBounds(ci);
		}
		public virtual bool HasFixedLeft { get { return FixedLeftColumn != null; } }
		public virtual bool HasFixedRight { get { return FixedRightColumn != null; } }
		public virtual bool HasFixedColumns {
			get { return FixedRightColumn != null || FixedLeftColumn != null; }
		}
		public virtual bool IsFixedRight(GridColumnInfoArgs ci) {
			return ci != null && ci.Column != null && ci.Column == FixedRightColumn;
		}
		public Rectangle GetGroupRowCellBounds(GridGroupRowInfo groupRow, GridColumn col) {
			var ci = ColumnsInfo[col];
			if(ci == null) return Rectangle.Empty;
			return GetGroupRowCellBounds(groupRow, ci);
		}
		public Rectangle GetGroupRowCellBounds(GridGroupRowInfo groupRow, GridColumnInfoArgs col) {
			if(groupRow == null || col == null) return Rectangle.Empty;
			return new Rectangle(col.Bounds.X, groupRow.DataBounds.Y, col.Bounds.Width - (View.GetShowVerticalLines() ? 1 : 0), groupRow.DataBounds.Height);
		}
		public virtual bool IsFixedLeft(GridColumnInfoArgs ci) {
			return ci != null && ci.Column != null && ci.Column == FixedLeftColumn;
		}
		public virtual bool IsFixedLeftPaint(GridColumnInfoArgs ci) {
			return ci != null && ci.Column != null && ci.Column.Fixed == FixedStyle.Left;
		}
		public virtual bool IsFixedRightPaint(GridColumnInfoArgs ci) {
			return ci != null && ci.Column != null && ci.Column.Fixed == FixedStyle.Right;
		}
		protected virtual void CalcRowCellDrawInfo(GridDataRowInfo ri, GridColumnInfoArgs ci, GridCellInfo cell, GridColumnInfoArgs nextColumn, bool calcEditInfo, GridRow nextRow) {
			cell.RowInfo = ri;
			cell.Bounds = new Rectangle(ci.Bounds.Left, ri.Bounds.Top + ri.RowLineHeight * ci.Info.StartRow,
				ci.Bounds.Width, ri.RowLineHeight * ci.Info.RowCount);
			cell.ColumnInfo = ci;
			if(ci.Type == GridColumnInfoType.Column) {
				cell.CellValue = GetRowCellValue(cell, ri.RowHandle, ci);
			}
			CalcRowCellDrawInfoCore(ri, ci, cell, nextColumn, calcEditInfo, nextRow);
		}
		protected virtual object GetRowCellValue(GridCellInfo cell, int rowHandle, GridColumnInfoArgs ci) {
			if(ImageLoadAsync(ci)) {
				return GetRowCellValueCore(cell.RowHandle, ci.Column.FieldName);
			}
			return View.GetRowCellValue(rowHandle, ci.Column);
		}
		protected bool ImageLoadAsync(GridColumnInfoArgs ci) {
			if(View == null || !View.OptionsImageLoad.AsyncLoad) return false;
			if(View.DataController == null || View.DataController.Helper == null || ci.Column.ColumnHandle == -1) return false;
			return View.DataController.Helper.Columns[ci.Column.ColumnHandle].Type == typeof(Image);
		}
		protected object GetRowCellValueCore(int rowHandle, string fieldName) {
			ImageLoadInfo info = View.GetImageLoadInfoCore(rowHandle, fieldName);
			if(info != null && info.ThumbImage != null) return info.ThumbImage;
			if(View.ImageLoader is AsyncImageLoader) {
				AsyncImageLoader loader = (AsyncImageLoader)View.ImageLoader;
				if(info != null) return loader.GetLoadingImage(info);
				return loader.GetLoadingImage(rowHandle, fieldName);
			}
			return null;
		}
		protected void AddHCellLine(GridCellInfo cell, Rectangle bounds, AppearanceObject appearance) {
			cell.RowInfo.AddLineInfo(cell, bounds, appearance);
		}
		protected void AddVCellLine(GridCellInfo cell, Rectangle bounds, AppearanceObject appearance) {
			cell.RowInfo.AddLineInfo(null, bounds, appearance);
		}
		protected virtual bool GetShowDetailButtonInCell(GridDataRowInfo ri, GridColumnInfoArgs ci) {
			bool showDetailButton = false;
			if(ShowDetailButtons && !ri.IsSpecialRow) {
				if(ci.Info.CellIndex == 0 && ci.Info.StartRow == 0) showDetailButton = true;
			}
			return showDetailButton;
		}
		protected internal virtual void CalcRowCellDrawInfoCore(GridDataRowInfo ri, GridColumnInfoArgs ci, GridCellInfo cell, GridColumnInfoArgs nextColumn, bool calcEditInfo, GridRow nextRow) {
			bool isFirstCell = ci.Info.CellIndex == 0;
			bool showDetailButton = GetShowDetailButtonInCell(ri, ci);
			Rectangle r = cell.Bounds;
			if(!(cell is GridMergedCellInfo)) {
				if(isFirstCell) {
					if(!View.IsRightToLeft) r.X += ri.IndentRect.Width;
					r.Width -= ri.IndentRect.Width;
				}
				else {
					if(IsRightToLeft) {
						if(r.X > ri.IndentRect.X) {
							cell.Bounds = new Rectangle(0, 0, -10, -10);
							return;
						}
						if(!HasFixedLeft || IsFixedLeft(ci)) {
							if(r.Right > ri.IndentRect.X) {
								r.Width = r.Right - ri.IndentRect.X;
								cell.Bounds = r;
							}
						}
					}
					else {
						if(r.Right < ri.IndentRect.Right) {
							cell.Bounds = new Rectangle(0, 0, -10, -10);
							return;
						}
						if(!HasFixedLeft || IsFixedLeft(ci)) {
							if(r.X < ri.IndentRect.Right) {
								r.Width = r.Right - ri.IndentRect.Right;
								r.X = ri.IndentRect.Right;
								cell.Bounds = r;
							}
						}
					}
				}
			}
			if(r.Width < 1) { 
				cell.Bounds = r;
				return;
			}
			if(IsFixedRight(ci)) {
				Rectangle vLine = UpdateFixedRange(new Rectangle(cell.Bounds.Left - View.FixedLineWidth, r.Top, View.FixedLineWidth, r.Height), ci);
				if(IsRightToLeft)
					vLine = UpdateFixedRange(new Rectangle(cell.Bounds.Right, r.Top, View.FixedLineWidth, r.Height), ci);
				if(vLine.Width > 0)	AddVCellLine(cell, vLine, PaintAppearance.FixedLine);
			}
			if(IsFixedLeft(ci)) {
				if(IsRightToLeft)
					AddVCellLine(cell, new Rectangle(r.X - (View.FixedLineWidth), r.Top, View.FixedLineWidth, r.Height), PaintAppearance.FixedLine);
				else
					AddVCellLine(cell, new Rectangle(r.Right, r.Top, View.FixedLineWidth, r.Height), PaintAppearance.FixedLine);
			} else {
				if(ci.Info.AllowRightBorder && View.GetShowVerticalLines() && (nextColumn == null || nextColumn.Column == null || nextColumn.Column != FixedRightColumn)) {
					r = AddVertRTLLine(cell.RowInfo.Lines, r, null, 1, PaintAppearance.VertLine, true, ci);
				}
			}
			if(View.GetShowHorizontalLines() && ci.Info.AllowBottomBorder) {
				int lineHeight = GetHorzLineHeight(ri);
				Rectangle hLine = UpdateFixedRange(new Rectangle(r.Left,r.Bottom - lineHeight, r.Width, lineHeight), ci);
				if(hLine.Width > 0) {
					if(ri.PreviewBounds.Height > 0 || !ri.MasterRowExpanded || !ci.Info.BottomColumn) {
						var line = PaintAppearance.HorzLine;
						if(IsShowRowBreak(ri, null, ri.Level) && AllowPartialGroups) {
							line = GetGroupDividerAppearance();
							if(View.GetShowVerticalLines()) hLine.Width++;
						}
						AddHCellLine(cell, hLine, line);
					}
					else
						AddHCellLine(cell, hLine, ri.Appearance);
				}
				r.Height -= lineHeight;
			}
			cell.Bounds = cell.CellValueRect = r;
			if(showDetailButton) CalcRowCellDetailButton(cell);
			if(ci.Column != null) {
				cell.CellValueRect = Deflate(cell.CellValueRect, CellPadding); 
				cell.CellValueRect = Deflate(cell.CellValueRect, GetRowCellPadding(cell.Column));
				if(calcEditInfo) CreateCellEditViewInfo(cell, calcEditInfo);
			}
		}
		void CalcRowCellDetailButton(GridCellInfo cell) {
			Rectangle r = cell.CellValueRect;
			Rectangle btnRect = r;
			btnRect.Width = DetailButtonSize.Width + 4;
			if(IsRightToLeft) {
				btnRect.X = cell.CellValueRect.Right - btnRect.Width;
				cell.CellValueRect.Width -= btnRect.Width;
			}
			else {
				cell.CellValueRect.X = btnRect.Right;
				cell.CellValueRect.Width -= btnRect.Width;
			}
			cell.CellButtonRect = btnRect;
			cell.CellButtonRect.Inflate(-2, 0);
			cell.CellButtonRect.Y = btnRect.Y +
				(btnRect.Height - DetailButtonSize.Height) / 2;
			cell.CellButtonRect.Size = DetailButtonSize;
		}
		protected virtual Padding GetRowCellPadding(GridColumn column) {
			return View.GetRowCellPadding(column);
		}
		internal static Rectangle Deflate(Rectangle source, Padding padding) {
			source.X += padding.Left;
			source.Y += padding.Top;
			source.Width -= padding.Horizontal;
			source.Height -= padding.Vertical;
			return source;
		}
		protected override bool ShouldAddItem(BaseEditViewInfo vi, CellId id) {
			if(View.OptionsView.GetAnimationType() == GridAnimationType.NeverAnimate) return false;
			GridCellInfo info = vi.Tag as GridCellInfo;
			if(info != null && ShouldRemoveAnimation(info.RowHandle, info.Column.FieldName)) {
				if(View.OptionsView.GetAnimationType() == GridAnimationType.AnimateFocusedItem && info.RowHandle != View.FocusedRowHandle) return false;
			}
			if(vi != null && vi.Bounds.Height < 0) return false;
			if(IsLoadingAnimation(vi)) return false;
			return base.ShouldAddItem(vi, id);
		}
		protected bool IsLoadingAnimation(BaseEditViewInfo vi) {
			if(View == null || !View.OptionsImageLoad.AsyncLoad || !(vi is PictureEditViewInfo)) return false;
			PictureEditViewInfo pi = (PictureEditViewInfo)vi;
			if(pi.ImageInfo == null) return false;
			return (!pi.ImageInfo.IsLoaded || pi.ImageInfo.ImageSize == Size.Empty) && pi.ImageInfo.LoadingStarted;
		}
		protected override void InvalidateAnimatedBounds(IAnimatedItem animItem) {
			BaseEditViewInfo vi = animItem as BaseEditViewInfo;
			GridCellInfo info = vi.Tag as GridCellInfo;
			if(vi == null || info == null) {
				base.InvalidateAnimatedBounds(animItem);
				return;
			}
			View.InvalidateRect(info.CellValueRect);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BaseEditViewInfo RequestCellEditViewInfo(GridCellInfo cell) {
			if(cell == null) return null;
			if(cell.ViewInfo != null || cell.Column == null) return cell.ViewInfo;
			CreateCellEditViewInfo(cell, true);
			UpdateCellEditViewInfo(cell);
			GridCellId id = new GridCellId(View, cell);
			if(ShouldAddItem(cell.ViewInfo, id))
				AddAnimatedItem(id, cell.ViewInfo);
			return cell.ViewInfo;
		}
		protected internal void UpdateCellEditViewInfo(GridCellInfo cell) {
			if(View == null || !View.OptionsImageLoad.AsyncLoad) return;
			if(cell.ViewInfo is PictureEditViewInfo) {
				PictureEditViewInfo pi = (PictureEditViewInfo)cell.ViewInfo;
				if(pi.ImageInfo == null) {
					Size desiredSize = View.OptionsImageLoad.DesiredThumbnailSize == Size.Empty ? cell.ViewInfo.ContentRect.Size : View.OptionsImageLoad.DesiredThumbnailSize;
					ImageLayoutMode mode = GetEditorImageLayoutMode(cell.Editor);
					pi.ImageInfo = View.GetImageLoadInfo(cell.RowHandle, cell.Column.FieldName, cell.ViewInfo.ContentRect.Size, desiredSize, mode);
					pi.ImageInfo.BackColor = View.BackColor;
					if(pi.ImageInfo.ThumbImage == null)
						cell.CellValue = pi.Image = View.ImageLoader.LoadImage(pi.ImageInfo);
					else cell.CellValue = pi.Image = pi.ImageInfo.ThumbImage;
				}
			}
		}
		protected ImageLayoutMode GetEditorImageLayoutMode(RepositoryItem properties) {
			if(properties is RepositoryItemPictureEdit) {
				RepositoryItemPictureEdit ri = (RepositoryItemPictureEdit)properties;
				return ri.GetImageLayoutMode();
			}
			return ImageLayoutMode.Squeeze;
		}
		protected internal void CreateCellEditViewInfo(GridCellInfo cell, bool calc) {
			CreateCellEditViewInfo(cell, calc, true);
		}
		protected internal virtual void CreateCellEditViewInfo(GridCellInfo cell, bool calc, bool allowCache) {
			cell.Editor = View.GetRowCellRepositoryItem(cell.RowHandle, cell.Column);
			if(cell.Editor != null) {
				bool fastRecalc = false;
				cell.ViewInfo = null;
				if(allowCache && GridControl != null) {
					UpdateCellAppearance(cell);
					BaseEditViewInfo info = GridControl.EditorHelper.GetCachedViewInfo(cell.Editor, GetCellHashCode(cell), GetCellEditorBounds(cell));
					if(info != null) {
						cell.ViewInfo = info.Clone() as BaseEditViewInfo;
						fastRecalc = true;
					}
				}
				if(cell.ViewInfo == null) {
					cell.ViewInfo = cell.Editor.CreateViewInfo();
					cell.ViewInfo.RightToLeft = View.IsRightToLeft; 
				}
				cell.ViewInfo.Tag = cell;
				UpdateCellEditViewInfo(cell, MousePosition, fastRecalc, calc);
			}
		}
		protected Rectangle GetCellEditorBounds(GridCellInfo cell) {
			return new Rectangle(Point.Empty, cell.CellValueRect.Size);
		}
		protected virtual void UpdateRowFooters(GridRowInfo ri) {
			foreach(GridRowFooterInfo fi in ri.RowFooters) {
				UpdateRowFooterInfo(ri, fi);
			}
		}
		protected virtual void UpdateRowFooterInfo(GridRowInfo ri, GridRowFooterInfo fi) {
			foreach(GridFooterCellInfoArgs fci in fi.Cells) {
				fci.DisplayText = "";
				fci.Value = null;
				DictionaryEntry dEntry = View.GetRowSummaryItem(fi.RowHandle, fci.Column);
				if(dEntry.Key != null) {
					fci.Value = dEntry.Value;
					fci.DisplayText = ((GridSummaryItem)dEntry.Key).GetDisplayText(dEntry.Value, false);
				}
			}
		}
		protected override void UpdateEditViewInfo(DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo vi) {
			base.UpdateEditViewInfo(vi);
			vi.AllowTextToolTip = View.OptionsHint.ShowCellHints;
		}
		protected void UpdateCellEditViewInfoError(GridCellInfo cell, BaseEditViewInfo editInfo) {
			string error;
			DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType;
			View.GetColumnError(cell.RowHandle, cell.Column, out error, out errorType);
			editInfo.ErrorIconText = error;
			editInfo.ShowErrorIcon = editInfo.ErrorIconText != null && editInfo.ErrorIconText.Length > 0;
			if(editInfo.ShowErrorIcon)
				editInfo.ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorType);
			else
				editInfo.ErrorIcon = null;
		}
		public virtual void UpdateCellEditViewInfo(GridCellInfo cell, Point mousePos, bool canFastRecalculate, bool calc) {
			BaseEditViewInfo editInfo = cell == null ? null : cell.ViewInfo;
			if(editInfo == null) return;
			editInfo.IsLoadingValue = !View.IsRowLoaded(cell.RowHandle);
			UpdateCellAppearance(cell);
			UpdateEditViewInfo(editInfo, cell.Column, cell.RowHandle);
			if(cell is GridMergedCellInfo)
				editInfo.DetailLevel = DetailLevel.Minimum;
			editInfo.EditValue = cell.CellValue;
			editInfo.PaintAppearance = cell.Appearance;
			if(calc) {
				UpdateCellEditViewInfoError(cell, editInfo);
				Rectangle r = GetCellEditorBounds(cell);
				mousePos.Offset(-cell.CellValueRect.X, -cell.CellValueRect.Y);
				cell.RowInfo.FormatInfo.ApplyContextImage(GInfo.Cache, cell.Column, r, cell.RowHandle, editInfo);
				editInfo.UpdateBoundValues(View.DataController, cell.RowHandle);
				if(View.IsEditFormMode) mousePos = new Point(int.MinValue, int.MinValue);
				if(canFastRecalculate)
					editInfo.ReCalcViewInfo(GInfo.Graphics, MouseButtons.None, mousePos, r);
				else {
					editInfo.CalcViewInfo(GInfo.Graphics, MouseButtons.None, mousePos, r);
					if(GridControl != null) {
						if(!editInfo.Bounds.Contains(mousePos) && !IsFocusedCell(cell)) {
							GridControl.EditorHelper.AddCachedViewInfo(editInfo.Item, GetCellHashCode(cell), editInfo.Clone() as BaseEditViewInfo);
						}
					}
				}
			}
		}
		bool IsFocusedCell(GridCellInfo cell) {
			if(cell.ColumnInfo == null || cell.Column == null) return false;
			return View.FocusedRowHandle == cell.RowHandle && View.FocusedColumn == cell.Column;
		}
		string GetCellHashCode(GridCellInfo cell) {
			return string.Concat(cell.Column.GetHashCode().ToString(),((int)CalcRowCellDetailLevel(cell.RowHandle, cell.Column)).ToString(), AppearanceHelper.GetFontHashCode(cell.Appearance.Font), AppearanceHelper.GetTextOptionsHashCode(cell.Appearance.TextOptions, null));
		}
		int CalcColumnGroupBestWidth(Graphics g, GridColumn column) {
			if(!View.DataController.IsGrouped) return 0;
			bool found = false;
			foreach(GridGroupSummaryItem item in View.GroupSummary) {
				if(item.ShowInGroupColumnFooter == column) {
					found = true;
					break;
				}
			}
			if(!found) return 0;
			int topRow = 0, rowCount = 0;
			if(View.IsServerMode) {
				topRow = View.TopRowIndex;
				rowCount = RowsInfo.Count > 10 ? RowsInfo.Count : 30;
			}
			else {
				rowCount = View.BestFitMaxRowCount < 0 ? View.RowCount : View.BestFitMaxRowCount;
			}
			int result = 0;
			GridFooterCellInfoArgs e = new GridFooterCellInfoArgs();
			e.Bounds = new Rectangle(0, 0, 100, 10);
			int margins = View.GetColumnIndent(column) + (ObjectPainter.CalcBoundsByClientRectangle(g, Painter.ElementsPainter.GroupFooterCell, new GridFooterCellInfoArgs(), new Rectangle(0, 0, 100, 10)).Width - 100);
			margins += (ObjectPainter.CalcBoundsByClientRectangle(g, Painter.ElementsPainter.GroupFooterPanel, new StyleObjectInfoArgs(), new Rectangle(0, 0, 100, 10)).Width - 100);
			for(int n = topRow; n < topRow + rowCount; n++ ) {
				int rowHandle = View.GetVisibleRowHandle(n);
				if(!View.IsValidRowHandle(rowHandle)) continue;
				if(!View.IsGroupRow(rowHandle)) continue;
				string text = View.GetRowFooterCellText(rowHandle, column);
				if(string.IsNullOrEmpty(text)) continue;
				int textWidth = Convert.ToInt32(PaintAppearance.GroupFooter.CalcTextSize(g, text, 0).Width);
				result = Math.Max(textWidth + margins + 4, result);
			}
			return result;
		}
		protected virtual bool IsShowFooter {
			get {
				if(View.OptionsView.ShowFooter) return true;
				if(!View.IsSplitVisible) return false;
				GridView other = View.SplitOtherView as GridView;
				if(View.IsVerticalSplit && other.OptionsView.ShowFooter) return true;
				return false;
			}
		}
		public override int CalcColumnBestWidth(GridColumn column) {
			int result = 0, initialWidth = column.Width;
			if(column.OptionsColumn.FixedWidth) return column.Width;
			if(column.RealColumnEdit != null && column.RealColumnEdit.BestFitWidth > -1) 
				return column.RealColumnEdit.BestFitWidth;
			Graphics g = GInfo.AddGraphics(null);
			try {
				result = CalcColumnBestBounds(column).Width;
				if(!View.OptionsView.ShowColumnHeaders) result = 4;
				if(IsShowFooter) {
					result = Math.Max(result, CalcColumnSummaryBestWidth(column, g));
				}
				int rowCount = View.DataRowCount;
				int topRow = ViewTopRowIndex;
				int bestFitRowCount = View.BestFitMaxRowCount;
				if(bestFitRowCount == -1 && View.WorkAsLookup) bestFitRowCount = 300;
				if(View.IsServerMode) bestFitRowCount = RowsInfo.Count > 10 ? RowsInfo.Count : 30;
				if(topRow == GridControl.InvalidRowHandle || (!View.IsServerMode && (bestFitRowCount == -1 || rowCount < bestFitRowCount)))
					topRow = 0;
				else {
					topRow = View.GetVisibleRowHandle(topRow);
					if(rowCount - topRow < bestFitRowCount)
						topRow = rowCount - bestFitRowCount;
					rowCount = bestFitRowCount;
				}
				if(topRow < 0) topRow = 0;
				result = Math.Max(result, CalcColumnGroupBestWidth(g, column));
				int cindent = View.GetColumnIndent(column);
				GridCellInfo cell = new GridCellInfo();
				cell.Appearance = PaintAppearance.Row;
				cell.ColumnInfo = CreateColumnInfo(column);
				cell.ColumnInfo.HtmlContext = View;
				cell.ColumnInfo.Bounds = new Rectangle(0, 0, 100, 20);
				if(column.VisibleIndex == 0) cell.ColumnInfo.Info.CellIndex = 0;
				GridDataRowInfo ri = CreateRowInfo(0, 0) as GridDataRowInfo;
				bool cellCalculated = false, cellEditCreated = false, isSameEditor = View.IsSameColumnEditor;
				bool cellHasValues = false;
				bool usesErrorInfo = View.OptionsView.BestFitUseErrorInfo != DefaultBoolean.False;
				if(View.OptionsView.BestFitUseErrorInfo == DefaultBoolean.Default) {
					if(rowCount > 500) usesErrorInfo = false;
				}
				for(int r = topRow; r < topRow + rowCount; r++) {
					int rowHandle = r; 
					if(!View.DataController.IsValidControllerRowHandle(rowHandle)) break;
					object cellValue = View.GetRowCellValue(rowHandle, column);
					if(AsyncServerModeDataController.IsNoValue(cellValue)) {
						continue;
					}
					cellHasValues = true;
					ri.RowHandle = rowHandle;
					if(ri.ConditionInfo != null) ri.ConditionInfo.Clear();
					if(ri.FormatInfo != null) ri.FormatInfo.Clear();
					UpdateRowAppearance(ri, true);
					if(!cellCalculated) {
						CalcRowCellDrawInfo(ri, cell.ColumnInfo, cell, null, false, null);
						cellCalculated = true;
					}
					cell.CellValue = cellValue;
					if(isSameEditor) {
						if(cellEditCreated)
							UpdateCellEditViewInfo(cell, MousePosition, false, false);
						else
							CreateCellEditViewInfo(cell, false, false);
						cellEditCreated = true;
					} else {
						CreateCellEditViewInfo(cell, false, false);
					}
					BaseEditViewInfo editInfo = cell.ViewInfo;
					editInfo.SetDisplayText(View.RaiseCustomColumnDisplayText(cell.RowHandle, cell.Column, cell.CellValue, editInfo.DisplayText, false));
					ri.FormatInfo.ApplyContextImage(GInfo.Cache, cell.Column, editInfo.Bounds, cell.RowHandle, editInfo);
					if(usesErrorInfo) UpdateCellEditViewInfoError(cell, editInfo);
					int w = editInfo.CalcBestFit(g).Width;
					w += (100 - GetCellEditorBounds(cell).Width) + cindent;
					if(w > result) result = w;
				}
				if(!cellHasValues && bestFitRowCount != 0) return Math.Max(result + 1, initialWidth);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return result + 1;
		}
		protected virtual int CalcColumnSummaryBestWidth(GridColumn column, Graphics g) {
			if(column.Summary.ActiveCount == 0) return 0;
			int result = 0;
			for(int n = 0; n < column.Summary.ActiveCount; n++) {
				GridSummaryItem item = column.Summary.GetActiveItem(n);
				GridFooterCellInfoArgs fci = GetFooterCellInfoArgs();
				fci.SummaryItem = item;
				fci.Value = fci.SummaryItem != null ? fci.SummaryItem.SummaryValue : null;
				fci.DisplayText =GetFooterCellText(fci);
				int footerCellWidth = Painter.ElementsPainter.FooterCell.CalcObjectMinBounds(fci).Width;
				result = Math.Max(result, footerCellWidth);
			}
			return result + 4;
		}
		protected virtual void CalcRowCellsDrawInfo(GridDataRowInfo ri, GridColumnsInfo columnsInfo, GridRow nextRow = null) {
			for(int n = 0; n < columnsInfo.Count; n ++) {
				GridColumnInfoArgs ci = columnsInfo[n];
				if(ci.Type != GridColumnInfoType.Column && ci.Type != GridColumnInfoType.EmptyColumn) {
					ri.Cells.Add(GridCellInfo.EmptyCellInfo);
					continue;
				}
				GridCellInfo cell = CreateCellInfo(ri, ci);
				CalcRowCellDrawInfo(ri, ci, cell, (n + 1 >= columnsInfo.Count ? null : columnsInfo[n + 1]), false, nextRow);
				if(cell.Bounds.Width > 0) 
					ri.Cells.Add(cell);
				else
					ri.Cells.Add(GridCellInfo.EmptyCellInfo);
			}
		}
		protected int CalcRowLevelIndentHeight(GridRowInfo ri, int level, int currentHeight) {
			if(ri.RowFooters.Count == 0) return currentHeight;
			foreach(GridRowFooterInfo fi in ri.RowFooters) {
				if(fi.Level <= level) {
					return fi.Bounds.Top - ri.IndentRect.Top;
				}
			}
			return currentHeight;
		}
		protected Rectangle AddVertRTLLine(IndentInfoCollection indents, Rectangle bounds, object owner, int lineWidth, AppearanceObject appearance, bool updateFixedRange, GridColumnInfoArgs ci) {
			Rectangle target = bounds;
			if(IsRightToLeft) {
				target = new Rectangle(bounds.X, bounds.Y, lineWidth, bounds.Height);
				if(updateFixedRange) target = UpdateFixedRange(target, ci);
				bounds.Width -= lineWidth;
				bounds.X += lineWidth;
			} else {
				target = new Rectangle(bounds.Right - lineWidth, bounds.Y, lineWidth, bounds.Height);
				if(updateFixedRange) target = UpdateFixedRange(target, ci);
				bounds.Width -= lineWidth;
			}
			if(target.Width > 0) {
				IndentInfo info = new IndentInfo(owner, target, appearance);
				indents.Add(info);
			}
			return bounds;
		}
		protected internal int GetGroupDividerHeight() { return 2; }
		protected internal AppearanceObject GetGroupDividerAppearance() {
			return new AppearanceObject() { BackColor = Color.FromArgb(126, 165, 209) };
		}
		bool IsShowRowBreak(GridRowInfo ri, GridRow nextRow, int level = -1) {
			if(level == -1) level = ri.Level;
			int? nextRowHandle;
			int nextRowLevel = 0;
			if(nextRow == null) {
				nextRowHandle = View.GetVisibleRowHandle(View.GetNextVisibleRow(ri.VisibleIndex));
				if(!View.IsValidRowHandle(nextRowHandle.Value)) nextRowHandle = null;
				else nextRowLevel = View.GetRowLevel(nextRowHandle.Value);
			}
			else {
				nextRowHandle = nextRow.RowHandle;
				nextRowLevel = nextRow.Level;
			}
			bool addLine = (nextRowHandle == null && ri.TotalBounds.Bottom <= ViewRects.Rows.Bottom) || (nextRowHandle != null && ri.Level > nextRowLevel && level >= nextRowLevel);
			AppearanceObject line = AllowPartialGroups ? GetGroupDividerAppearance() : PaintAppearance.HorzLine;
			if(!addLine && AllowPartialGroups && nextRowHandle != null && nextRowLevel == ri.Level && !ri.IsGroupRow) {
				if(View.DataController.GetVisibleIndexes().IsSingleGroupRow(nextRowHandle.Value)) {
					addLine = true;
				}
			}
			return addLine;
		}
		protected virtual void CalcRowIndentInfo(GridRowInfo ri, GridRow row, GridRow nextRow) {
			if(ri.Level > 0) {
				for(int n = 0; n < ri.Level; n++) {
					int indentWidth = CalcRowLevelIndent(ri, n);
					int deltaDirection = IsRightToLeft ? -1 : 1;
					Rectangle r = ri.IndentRect;
					r.Width = CalcRowLevelIndent(ri, n + 1) == 0 ? 0 : LevelIndent;
					if(IsRightToLeft) {
						r.X = ri.IndentRect.Right - indentWidth - r.Width;
					}
					else {
						r.X = ri.IndentRect.X + indentWidth * deltaDirection;
					}
					r.Height = CalcRowLevelIndentHeight(ri, n, r.Height);
					if(View.GetShowVerticalLines() && r.Width > 0) {
						r = AddVertRTLLine(ri.Lines, r, null, 1, PaintAppearance.VertLine, false, null);
					}
					if(View.GetShowHorizontalLines()) {
						if(r.Width > 0) {
							bool addLine = IsShowRowBreak(ri, nextRow, n);
							if(addLine) {
								int lineHeight = GetHorzLineHeight(ri);
								ri.AddLineInfo(null, r.Left, r.Bottom - lineHeight,
									r.Width + (AllowPartialGroups ? 1 : 0), lineHeight, AllowPartialGroups ? GetGroupDividerAppearance() : PaintAppearance.HorzLine);
								r.Height -= lineHeight;
							}
						}
					}
					ri.Indents.Add(new IndentInfo(null, r, View.GetLevelStyle(n, false), true));
				}
			}
		}
		int GetFastParentRowHandle(int rowHandle) {
			if(rowHandle >= 0) {
				var res = View.DataController.GroupInfo.GetGroupRowInfoByControllerRowHandleBinary(rowHandle);
				return res == null ? DataController.InvalidRow : res.Handle;
			}
			return View.GetParentRowHandle(rowHandle);
		}
		public virtual GroupDrawMode GetGroupDrawMode() {
			if(IsPrinting) return GroupDrawMode.Standard;
			if(View.OptionsView.GroupDrawMode != GroupDrawMode.Default) return View.OptionsView.GroupDrawMode;
			GridPaintStyle ps = View.PaintStyle as GridPaintStyle;
			return ps != null ? ps.GetGroupDrawMode(View) : GroupDrawMode.Standard;
		}
		public virtual bool IsShowCurrentRowFooter(int rowHandle, int nextRowHandle) {
			if(GridControl.AutoFilterRowHandle == rowHandle || View.IsNewItemRow(rowHandle)) return false;
			if(!View.IsShowRowFooters()) return false;
			bool isGroup = View.IsGroupRow(rowHandle);
			bool isExpanded = View.GetRowExpanded(rowHandle);
			if(isGroup) {
				if(isExpanded) return false;
				if(!isExpanded && View.OptionsView.GroupFooterShowMode != GroupFooterShowMode.VisibleAlways) return false;
				if(View.IsExistAnyRowFooterCell(rowHandle)) return true;
				return View.OptionsView.GroupFooterShowMode == GroupFooterShowMode.VisibleAlways;
			} else {
				int nextLevel = nextRowHandle == GridControl.InvalidRowHandle ? 0 : View.GetRowLevel(nextRowHandle);
				if(nextRowHandle == GridControl.InvalidRowHandle || View.IsGroupRow(nextRowHandle)) {
					if(View.IsExistAnyRowFooterCell(rowHandle)) return true;
					return View.OptionsView.GroupFooterShowMode == GroupFooterShowMode.VisibleAlways;
				}
				return false;
			}
		}
		protected virtual bool IsShowCurrentRowFooter(GridRowInfo ri) {
			return IsShowCurrentRowFooter(ri.RowHandle, View.GetVisibleRowHandle(View.GetNextVisibleRow(ri.VisibleIndex)));
		}
		protected virtual void CalcRowFooterInfo(GridRowInfo ri, GridRow row, GridRow nextRow) {
			int height = ri.RowFooters.RowFootersHeight;
			if(height == 0) return;
			bool isShowCurrentFooter = 	IsShowCurrentRowFooter(ri);
			int startLevel = ri.Level;
			int footerRowHandle = ri.RowHandle;
			if(!ri.IsGroupRow || !isShowCurrentFooter) 
				footerRowHandle = View.GetParentRowHandle(footerRowHandle);
			if(!isShowCurrentFooter) {
				startLevel --;
			}
			int top = ri.TotalBounds.Bottom - height - ri.RowSeparatorBounds.Height;
			int left = ri.IndentRect.Right - (!isShowCurrentFooter ? LevelIndent : 0);
			if(IsRightToLeft) {
				left = ri.TotalBounds.Left;
			}
			ri.RowFooters.Bounds = new Rectangle(left, top, ri.DataBounds.Right - left, height);
			for(int n = 0; n < ri.RowFooters.RowFooterCount; n++) {
				GridRowFooterInfo fi = new GridRowFooterInfo();
				ri.RowFooters.Add(fi);
				fi.RowHandle = footerRowHandle;
				fi.Bounds = ri.Bounds;
				fi.Level = startLevel;
				fi.Bounds.Y = top;
				fi.Bounds.X = left;
				fi.Bounds.Width = ri.DataBounds.Right - fi.Bounds.Left;
				fi.Bounds.Height = GroupFooterHeight;
				top += fi.Bounds.Height;
				if(ri.IndicatorRect.Width > 0) {
					fi.IndicatorRect = ri.IndicatorRect;
					fi.IndicatorRect.Y = fi.Bounds.Y;
					fi.IndicatorRect.Height = fi.Bounds.Height;
				}
				if(View.GetShowHorizontalLines()) {
					ri.AddLineInfo(null, fi.Bounds.Left, fi.Bounds.Bottom - 1,
						fi.Bounds.Width, 1, PaintAppearance.HorzLine);
					fi.Bounds.Height -= 1;
				}
				CalcRowCellsFooterInfo(fi, ri);
				footerRowHandle = View.GetParentRowHandle(footerRowHandle);
				startLevel --;
				left -= LevelIndent;
			}
		}
		protected virtual void CalcRowCellsFooterInfo(GridRowFooterInfo fi, GridRowInfo ri) {
			fi.Cells.Clear();
			Rectangle maxBounds = Painter.ElementsPainter.GroupFooterPanel.GetObjectClientRectangle(new StyleObjectInfoArgs(null, fi.Bounds, null, null, ObjectState.Normal));
			Rectangle r = Rectangle.Empty;
			foreach(GridColumnInfoArgs ci in ColumnsInfo) {
				if(ci.Type == GridColumnInfoType.Column) {
					GridFooterCellInfoArgs fci  =  new GridFooterCellInfoArgs();
					r = new Rectangle(ci.Bounds.Left, maxBounds.Top,
						ci.Bounds.Width, GroupFooterCellHeight * ci.Info.RowCount);
					if(ci.Info.CellIndex == 0) {
						int delta = ri.IndentRect.Width - CalcRowLevelIndent(ri, ri.Level - fi.Level);
						if(!IsRightToLeft) r.X += delta;
						r.Width -= (delta);
					}
					r = Painter.ElementsPainter.GroupFooterPanel.CalcCellBounds(maxBounds, r, GroupFooterCellHeight, ci.Info.StartRow, ci.Info.RowCount);
					fci.Bounds = r;
					fci.ColumnInfo = ci;
					fci.SetAppearance(PaintAppearance.GroupFooter.Clone() as AppearanceObject);
					if(View.IsShowRowFooterCell(fi.RowHandle, ci.Column)) {
						DictionaryEntry dEntry = View.GetRowSummaryItem(fi.RowHandle, fci.Column);
						if(dEntry.Key != null) {
							fci.Value = dEntry.Value;
							fci.SummaryItem = ((GridSummaryItem)dEntry.Key);
							fci.DisplayText = fci.SummaryItem.GetDisplayText(dEntry.Value, false);
						}
						else {
							fci.Visible = false;
						}
					} else {
						fci.Visible = false;
					}
					fi.Cells.Add(fci);
				}
			}
		}
		public virtual void CheckRowData(GridRowInfo ri) {
			if(!IsReady || !ri.IsDataDirty) return;
			UpdateRowData(ri, true, false);
		}
		public virtual void UpdateRowData(GridRowInfo bi, bool updateMouse, bool forPrinting) {
			if(bi.IsGroupRow) {
				UpdateGroupRow(bi as GridGroupRowInfo, updateMouse);
				return;
			}
			GridDataRowInfo ri = bi as GridDataRowInfo;
			ri.SetDataDirty(false);
			ri.PreviewText = "";
			string error;
			if(!forPrinting) {
				DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType;
				View.GetColumnError(ri.RowHandle, null, out error, out errorType);
				ri.ErrorText = error;
				if(ri.ErrorText != null && ri.ErrorText.Length > 0) {
					ri.ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorType);
				}
			}
			ri.ConditionInfo.Clear();
			ri.FormatInfo.Clear();
			if(View.OptionsView.ShowPreview)
				ri.PreviewText = View.GetRowPreviewDisplayText(ri.RowHandle);
			UpdateRowCells(ri, forPrinting);
			if(updateMouse)
				UpdateRowFooters(ri);
		}
		protected virtual void UpdateGroupRow(GridGroupRowInfo groupRow, bool updateMouse) {
			groupRow.SetDataDirty(false);
			groupRow.GroupText = View.GetGroupRowDisplayText(groupRow.RowHandle, true);
			groupRow.EditValue = View.GetGroupRowValue(groupRow.RowHandle);
			if(updateMouse)
				UpdateRowFooters(groupRow);
		}
		protected virtual void UpdateRowCells(GridDataRowInfo ri, bool forPrinting) {
			for(int n = 0; n < ri.Cells.Count; n++) {
				GridCellInfo cell = ri.Cells[n];
				if(cell.ColumnInfo.Type != GridColumnInfoType.Column) continue;
				object newValue = GetRowCellValue(cell, ri.RowHandle, cell.ColumnInfo); 
				cell.State = GridRowCellState.Dirty;
				cell.CellValue = newValue;
				if(!forPrinting) {
					if(cell.ViewInfo is IAnimatedItem && ShouldRemoveAnimation(ri.RowHandle, cell.Column.FieldName)) XtraAnimator.RemoveObject(this, new GridCellId(View, cell));
					cell.ViewInfo = null;
				}
			}
		}
		protected internal bool ShouldRemoveAnimation(int rowHandle, string fieldName) {
			if(View == null || !View.OptionsImageLoad.AsyncLoad || View.IsRowVisible(rowHandle) == RowVisibleState.Hidden) return true;
			ImageLoadInfo info = View.GetImageLoadInfoCore(rowHandle, fieldName);
			return info != null && info.IsLoaded;
		}
		protected internal virtual GridRowCellState CalcRowState(GridRowInfo ri) {
			GridRowCellState state = CalcRowStateCore(ri.RowHandle);
			if(!ri.IsGroupRow) {
				state |= (ri.RowHandle % 2 != 0 ? GridRowCellState.Even : GridRowCellState.Odd);
				if(View.IsCellSelect) state &= (~GridRowCellState.Selected);
			} 
			return state;
		}
		protected virtual GridRowCellState CalcMergedRowCellState(GridMergedCellInfo cell) {
			if(cell.FirstCell == null) return cell.State;
			GridRowCellState state = GridRowCellState.Default;
			foreach(GridCellInfo dc in cell.MergedCells) {
				state |= CalcRowCellState(dc);
			}
			return state;
		}
		protected virtual GridRowCellState CalcRowCellState(GridCellInfo cell) {
			if(cell == GridCellInfo.EmptyCellInfo) return GridRowCellState.Default;
			if(cell is GridMergedCellInfo) return CalcMergedRowCellState(cell as GridMergedCellInfo);
			GridRowCellState state = cell.RowInfo.RowState;
			if(cell.IsDataCell && cell.Column == View.FocusedColumn && cell.RowHandle == View.FocusedRowHandle) {
				if(!View.IsInplaceEditFormVisible) state |= GridRowCellState.FocusedCell;
			}
			if(View.IsCellSelect && View.IsCellSelected(cell.RowHandle, cell.Column)) state |= GridRowCellState.Selected;
			return state;
		}
		protected AppearanceObject[] GetRowMixAppearances(GridRowCellState state, GridRowInfo ri, AppearanceObjectEx column, AppearanceObject focusedCell, AppearanceObjectEx conditionCell) {
			return GetRowMixAppearances(null, state, ri, column, focusedCell, conditionCell);
		}
		public virtual bool IsTransparentFocusedStyle { get { return PaintAppearance.FocusedRow.BackColor.A != 255; } }
		public virtual bool IsTransparentSelectedStyle { get { return PaintAppearance.SelectedRow.BackColor.A != 255 || IsTransparentFocusedStyle; } }
		protected virtual bool IsCellMerging { get { return View.IsCellMerge; } }
		public virtual AppearanceObject GetCellTransparentAppearance(GridCellInfo cell) {
			if(!View.IsCellSelect || !IsTransparentSelectedStyle || cell.Column == null) return null;
			if((cell.State & GridRowCellState.Selected) == 0) return null;
			AppearanceObject res = PaintAppearance.SelectedRow;
			if((cell.State & GridRowCellState.GridFocused) == 0 && View.OptionsSelection.EnableAppearanceHideSelection) res = PaintAppearance.HideSelectionRow;
			if(res != null && res.BackColor.A == 255) res.BackColor = Color.FromArgb(50, res.BackColor);
			return res;
		}
		public virtual AppearanceObject GetRowTransparentAppearance(GridRowInfo ri) {
			if(View.IsCellSelect && !ri.IsGroupRow) return null;
			AppearanceObject res = PaintAppearance.FocusedRow;
			if(View.IsMultiSelect) {
				if(!IsTransparentSelectedStyle) return null;
				if((ri.RowState & GridRowCellState.Focused) == 0) res = PaintAppearance.SelectedRow;
				if((ri.RowState & GridRowCellState.GridFocused) == 0 && View.OptionsSelection.EnableAppearanceHideSelection) res = PaintAppearance.HideSelectionRow;
			} else {
				res = PaintAppearance.FocusedRow;
				if((ri.RowState & GridRowCellState.GridFocused) == 0) {
					if(View.OptionsSelection.EnableAppearanceHideSelection) res = PaintAppearance.HideSelectionRow;
				} else {
					if(!View.IsKeyboardFocused) res = null;
				}
			}
			if(res != null && res.BackColor.A == 255) res.BackColor = Color.FromArgb(50, res.BackColor);
			return res;
		}
		internal bool fDetailIndentDrawingInternal = false;
		protected virtual AppearanceObject[] GetRowMixAppearances(GridCellInfo cellInfo, GridRowCellState state, GridRowInfo bi, AppearanceObjectEx column, AppearanceObject focusedCell, AppearanceObjectEx conditionCell) {
			bool isCell = cellInfo != null, allowHideFocusAppearance = false, rowStyleHighPriority = false;
			AppearanceObject even = PaintAppearance.EvenRow, odd = PaintAppearance.OddRow,
				focused = PaintAppearance.FocusedRow, row = bi.IsGroupRow ? View.GetLevelStyle(bi.Level, true) : PaintAppearance.Row,
				selected = PaintAppearance.SelectedRow;
			AppearanceObjectEx
				conditionRow = null, conditionRowHigh, conditionRowLow, conditionCellHigh, conditionCellLow,
				columnHigh, columnLow;
			GridDataRowInfo ri = bi as GridDataRowInfo;
			if(!View.OptionsView.EnableAppearanceEvenRow) even = null;
			if(!View.OptionsView.EnableAppearanceOddRow) odd = null;
			if((state & GridRowCellState.GridFocused) == 0 && View.OptionsSelection.EnableAppearanceHideSelection) {
				focused = selected = PaintAppearance.HideSelectionRow;
				allowHideFocusAppearance = View.SelectedRowsCount > 1;
			}
			if((state & GridRowCellState.Selected) == 0) selected = null;
			if(!View.OptionsSelection.EnableAppearanceFocusedRow) focused = null;
			if(!View.IsFilterRow(bi.RowHandle) && !View.OptionsSelection.EnableAppearanceFocusedCell) focusedCell = null;
			if(IsCellMerging || bi.IsGroupRow || (ri != null && ri.IsSpecialRow) || (IsPrinting && View.OptionsPrint.UsePrintStyles)) even = odd = null;
			if((state & GridRowCellState.Even) != 0)
				odd = null;
			else
				even = null;
			if(isCell && bi.BaseAppearance != null) {
				row = bi.BaseAppearance;
				rowStyleHighPriority = bi.BaseAppearanceHighPriority;
			}
			else {
				row = View.RaiseGetRowStyle(bi.RowHandle, state, row, out rowStyleHighPriority);
				if(!fDetailIndentDrawingInternal) {
					bi.BaseAppearance = row;
					bi.BaseAppearanceHighPriority = rowStyleHighPriority;
				}
			}
			if(ri != null) {
				conditionRow = ri.RowFormatAppearance;
			}
			columnHigh = columnLow = conditionRowHigh = conditionRowLow = conditionCellHigh = conditionCellLow = null;
			if(bi.IsGroupRow) {
				column = null;
				conditionCell = null;
				conditionRow = null;
			}
			if(column != null) {
				if(column.Options.HighPriority)
					columnHigh = column;
				else
					columnLow = column;
			}
			if(conditionRow != null) {
				if(conditionRow.Options.HighPriority)
					conditionRowHigh = conditionRow;
				else
					conditionRowLow = conditionRow;
			}
			if(conditionCell != null) {
				if(conditionCell.Options.HighPriority)
					conditionCellHigh = conditionCell;
				else
					conditionCellLow = conditionCell;
			}
			if((state & GridRowCellState.Focused) != 0) {
				if(View.IsCellSelect && (state & GridRowCellState.Selected) != 0) {
					focused = focusedCell = null;
				}
				else {
					selected = null;
				}
				if((state & GridRowCellState.GridFocused) == 0) {
					if(View.GridControlIsFocused) {
						if(!allowHideFocusAppearance) focusedCell = focused = null;
					}
					else {
						if(focused != null || focusedCell != null) {
							if(!View.IsKeyboardFocused && !allowHideFocusAppearance)
								focusedCell = focused = null;
							else {
								if(View.OptionsSelection.EnableAppearanceHideSelection && focused != null)
									focused = PaintAppearance.HideSelectionRow;
							}
						}
					}
				}
				if(View.OptionsSelection.InvertSelection) {
					if(isCell && (cellInfo.State & GridRowCellState.FocusedCell) != 0) focusedCell = focused;
					focused = selected = null;
				}
				if(View.IsMultiSelect && (state & GridRowCellState.Selected) == 0) focused = null;
			}
			else {
				focused = null;
			}
			if(IsCellMerging && !bi.IsGroupRow) {
				focused = null;
			}
			if(!isCell && ri != null && ri.IsNewItemRow && (state & GridRowCellState.Focused) == 0) {
				focused = null;
				row = PaintAppearance.TopNewRow;
			}
			if(IsPrinting) {
				focusedCell = focused = selected = null;
			}
			if(cellInfo != null && cellInfo.ViewInfo != null && cellInfo.ViewInfo.MatchedString != string.Empty) {
				if(!View.IsFindFilterActive || View.IsFindFilterFocused) {
					focused = focusedCell = null;
				}
			}
			if(IsTransparentFocusedStyle) focused = focusedCell = null;
			if(IsTransparentSelectedStyle) selected = null;
			return new AppearanceObject[] { rowStyleHighPriority ? row : null, columnHigh, conditionCellHigh, conditionRowHigh, focusedCell, focused, columnLow, conditionCellLow, conditionRowLow, selected, even, odd, row };
		}
		public void UpdateBeforePaint(GridRowInfo ri) {
			CheckRowData(ri);
			GridDataRowInfo dataRow = ri as GridDataRowInfo;
			if(dataRow != null && !dataRow.IsSpecialRow) {
				if(UpdateRowConditionAndFormat(ri.RowHandle, dataRow)) {
					ri.RowState = GridRowCellState.Dirty;
				}
			}
			UpdateRowAppearance(ri);
		}
		public void UpdateRowAppearance(GridRowInfo ri) { UpdateRowAppearance(ri, false); }
		protected internal virtual void UpdateRowAppearance(GridRowInfo ri, bool always) {
			GridRowCellState state = CalcRowState(ri);
			if(state == ri.RowState && !always) return;
			ri.RowState = state;
			UpdateRowAppearanceCore(ri);
		}
		protected internal virtual void UpdateRowAppearanceCore(GridRowInfo ri) {
			if(ri.Appearance == null || ri.Appearance == PaintAppearance.Row) ri.Appearance = new AppearanceObject();
			GetRowAppearance(ri, ri.Appearance, ri.RowState);
		}
		protected internal AppearanceObject GetRowAppearance(GridRowInfo ri, AppearanceObject appearance, GridRowCellState state) {
			AppearanceHelper.Combine(appearance, GetRowMixAppearances(state, ri, null, null, null));
			return appearance;
		}
		public virtual void UpdateRowAppearancePreview(GridDataRowInfo ri) {
			ri.AppearancePreview = null;
			if(View.DrawFullRowFocus) {
				AppearanceObject app = (AppearanceObject)PaintAppearance.Preview.Clone();
				app.BackColor = ri.Appearance.BackColor;
				app.BackColor2 = ri.Appearance.BackColor2;
				app.GradientMode = ri.Appearance.GradientMode;
				if(IsSkinned && ((ri.RowState &  (GridRowCellState.Focused | GridRowCellState.Selected)) != 0)) {
					Color foreColor = GridSkins.GetSkin(View).Colors.GetColor(GridSkins.OptColorPreviewFocusedForeColor, Color.Empty);
					if(foreColor != Color.Empty) app.ForeColor = foreColor;
				}
				ri.AppearancePreview = app;
			}
		}
		public void UpdateCellAppearance(GridCellInfo cell) { UpdateCellAppearance(cell, false); }
		protected internal virtual void UpdateCellAppearance(GridCellInfo cell, bool always) {
			if(cell == GridCellInfo.EmptyCellInfo) return;
			if(cell.State == GridRowCellState.Dirty && cell.RowInfo.RowState == GridRowCellState.Dirty) {
				UpdateRowAppearance(cell.RowInfo);
			}
			GridRowCellState state = CalcRowCellState(cell);
			if(state == cell.State && !always) return;
			cell.State = state;
			UpdateCellAppearanceCore(cell);
		}
		GridPixelPositionCalculatorBase positionHelper;
		public GridPixelPositionCalculatorBase PositionHelper {
			get {
				if(positionHelper == null) positionHelper = CreatePositionHelper();
				return positionHelper;
			}
		}
		internal void ResetPixels() {
			this.positionHelper = null;
		}
		protected virtual GridPixelPositionCalculatorBase CreatePositionHelper() {
			return new GridPixelPositionCalculatorCached(this);
		}
		public int VisibleRowsHeight { get { return PositionHelper.VisibleRowsHeight; } }
		public int CalcPixelPositionByRow(int row) { return PositionHelper.CalcPixelPositionByRow(row); }
		public int CalcVisibleRowByPixel(int pixelPosition) { return PositionHelper.CalcVisibleRowByPixel(pixelPosition); }
		protected internal virtual void UpdateRowIndexes(int newTopRowIndex) {
			if(RowsLoadInfo != null)
				UpdateRowIndexes(newTopRowIndex, RowsLoadInfo.ResultRows);
			bool invalidate = false;
			for(int n = 0; n < RowsInfo.Count; n++) {
				GridRowInfo ri = RowsInfo[n];
				GridDataRowInfo dr = ri as GridDataRowInfo;
				if(dr != null && dr.IsSpecialRow) continue;
				ri.VisibleIndex = newTopRowIndex ++;
				int rowHandle = ri.RowHandle;
				ri.RowHandle = View.GetVisibleRowHandle(ri.VisibleIndex);
				if(rowHandle == View.FocusedRowHandle) {
					invalidate = true;
					ri.SetDataDirty();
				}
			}
			RowsInfo.UpdateHash();
			if(invalidate) {
				View.InvalidateRows();
			}
		}
		protected internal virtual void UpdateCellAppearanceCore(GridCellInfo cell) {
			if(cell.Appearance == null || cell.Appearance == PaintAppearance.Row) cell.Appearance = new AppearanceObject();
			AppearanceObject focusedCell = null;
			AppearanceObjectEx condition = null, cellApp = null;
			if(cell.IsDataCell) {
				if(!cell.RowInfo.IsSpecialRow) {
					if(UpdateRowConditionAndFormat(cell.RowHandle, cell.RowInfo)) {
						cell.RowInfo.RowState = GridRowCellState.Dirty;
					}
				}
				cellApp = cell.Column.AppearanceCell;
				condition = cell.RowInfo.GetConditionAppearance(cell.Column);
				if((cell.State & GridRowCellState.FocusedCell) != 0) {
					if(View.OptionsSelection.InvertSelection && !View.IsEditing) 
						focusedCell = null;
					else
						focusedCell = PaintAppearance.FocusedCell;
				}
			}
			GridRowCellState cellState = cell.State; 
			if(cell.Editor != null && !cell.Editor.AllowFocusedAppearance) 
				cellState &= ~(GridRowCellState.Selected | GridRowCellState.Focused);
			AppearanceHelper.Combine(cell.Appearance, GetRowMixAppearances(cell, cellState, cell.RowInfo, cellApp, focusedCell, condition));
			if(cell.IsDataCell) {
				if(cell.Appearance.GetTextOptions().HAlignment == HorzAlignment.Default)
					cell.Appearance.TextOptions.HAlignment = View.GetRowCellDefaultAlignment(cell.RowHandle, cell.Column, cell.ColumnInfo.Info.DefaultValueAlignment);
				if((cell.State & GridRowCellState.FocusedCell) == 0 || !View.IsEditing)
					cell.Appearance = View.GetRowCellStyle(cell.RowHandle, cell.Column, cell.State, cell.Appearance);
			}
		}
		protected virtual void UpdateDetailRects() {
			GridRowInfoCollection coll = RowsInfo.Clone();
			ArrayList empty = new ArrayList();
			for(int n = View.DataController.ExpandedMasterRowCollection.Count - 1; n >= 0; n--) {
				DevExpress.Data.Details.MasterRowInfo info = View.DataController.ExpandedMasterRowCollection[n];
				DevExpress.Data.Details.DetailInfo detail = info.FindDetail(-1);
				if(detail == null) continue;
				BaseView view = detail.DetailOwner as BaseView;
				int rowHandle = View.GetRowHandle(info.ParentListSourceRow);
				GridDataRowInfo rowInfo = null;
				if(View.IsServerMode) {
					rowHandle = 0;
					rowInfo = coll.FindRowByKey(info.ParentRowKey) as GridDataRowInfo;
				}
				else
					rowInfo = coll.GetInfoByHandle(rowHandle) as GridDataRowInfo;
				if(rowHandle == GridControl.InvalidRowHandle || rowInfo == null || rowInfo.DetailBounds.Height < 1 || rowInfo.TotalBounds.Height < 1) {
					if(view != null) view.InternalSetViewRectCore(Rectangle.Empty);
				} else {
					if(view != null) view.InternalSetViewRectCore(rowInfo.DetailBounds);
				}
			}
		}
		bool IsAutoFilterRow(int rowHandle) {
			return GridControl.AutoFilterRowHandle == rowHandle;
		}
		protected virtual GridRowInfo CreateRowInfo(int rowHandle, int rowLevel) {
			if(View.IsGroupRow(rowHandle))
				return new GridGroupRowInfo(this, rowHandle, rowLevel) { RightToLeft = IsRightToLeft };
			if(View.IsNewItemRow(rowHandle)) 
				return new GridNewItemRow(this, rowHandle) { RightToLeft = IsRightToLeft };
			return new GridDataRowInfo(this, rowHandle, IsAutoFilterRow(rowHandle) ? 0 : rowLevel) { RightToLeft = IsRightToLeft };
		}
		public virtual GridRowInfo CreateRowInfo(int rowHandle, int rowWidth, GridColumnsInfo colInfo) {
			GridRowInfo bi = CreateRowInfo(rowHandle, 0);
			bi.Bounds = new Rectangle(0, 0, rowWidth, 24);
			if(!bi.IsGroupRow) {
				GridDataRowInfo ri = bi as GridDataRowInfo;
				UpdateRowConditionAndFormat(ri.RowHandle, ri);
			}
			UpdateRowAppearance(bi, true);
			return bi;
		}
		public virtual bool IsAlignGroupRowSummariesUnderColumns { get { return View.IsAlignGroupRowSummariesUnderColumns; } }
		public virtual GridRowInfo CreateRowInfo(GridRow row) {
			GridRowInfo bi = CreateRowInfo(row.RowHandle, row.Level);
			bi.ForcedRow = row.ForcedRow;
			bi.ForcedRowLight = row.ForcedRowLight;
			bi.RowKey = row.RowKey;
			bi.VisibleIndex = row.VisibleIndex;
			bi.Appearance = PaintAppearance.Row;
			GridDataRowInfo rowInfo = bi as GridDataRowInfo;
			if(rowInfo != null && !rowInfo.IsNewItemRow) {
				rowInfo.IsMasterRow = View.IsMasterRow(bi.RowHandle);
				rowInfo.IsMasterRowEmpty = false;
				if(View.OptionsDetail.GetSmartDetailExpandButtonMode() == DetailExpandButtonMode.CheckDefaultDetail)
					rowInfo.IsMasterRowEmpty = rowInfo.IsMasterRow && View.IsMasterRowEmptyCached(bi.RowHandle);
				if(View.OptionsDetail.GetSmartDetailExpandButtonMode() == DetailExpandButtonMode.CheckAllDetails)
					rowInfo.IsMasterRowEmpty = rowInfo.IsMasterRow && View.IsAllMasterRowEmptyCached(bi.RowHandle);
			}
			return bi;
		}
		protected virtual void CalcRowPreviewInfo(GridDataRowInfo ri, bool showEditForm) {
			if(View.OptionsView.ShowPreview && !ri.IsSpecialRow && !ri.IsNewItemRow) {
				ri.PreviewText = View.GetRowPreviewDisplayText(ri.RowHandle);
				ri.PreviewIndent = PreviewIndent;
				ri.PreviewBounds.X = ri.DataBounds.X;
				ri.PreviewBounds.Y = ri.DataBounds.Bottom;
				ri.PreviewBounds.Width = CalcRowPreviewWidth(ri.RowHandle);
				ri.PreviewBounds.Height = CalcRowPreviewHeight(ri.RowHandle);
				if(ri.PreviewBounds.Height == 0) 
					ri.PreviewBounds = Rectangle.Empty;
				else {
					if(showEditForm) {
						ri.PreviewBounds.Y += GetEditFormHeight(ri.RowHandle);
					}
				}
				ri.IndentRect.Height += ri.PreviewBounds.Height;
				ri.IndicatorRect.Height += ri.PreviewBounds.Height;
			}
		}
		protected internal virtual int DetailVertIndent { get { return ScaleVertical(View.DetailVerticalIndent); } }
		protected virtual void CalcRowMasterInfo(GridDataRowInfo ri) {
			ri.MasterRowExpanded = ri.IsMasterRow && View.GetMasterRowExpanded(ri.RowHandle);
			if(!ri.MasterRowExpanded) return;
			ri.DetailIndentBounds = ri.IndentRect;
			ri.DetailIndentBounds.X = ri.IndentRect.Right;
			ri.DetailIndentBounds.Width = 1 * LevelIndent + (!HasFixedLeft ? View.LeftCoord : 0);
			ri.DetailIndentBounds.Y = ri.Bounds.Bottom + ri.PreviewBounds.Height;
			if(IsRightToLeft) {
				ri.DetailIndentBounds.X = (ri.IndentRect.Width == 0 ? ri.IndentRect.X : ri.IndicatorRect.Left) - ri.DetailIndentBounds.Width;
			}
			int detailHeight = ri.TotalBounds.Bottom - ri.Bounds.Bottom - ri.PreviewBounds.Height - ri.RowFooters.RowFootersHeight - ri.RowSeparatorBounds.Height;
			if(ri.DetailIndentBounds.Top + detailHeight > ViewRects.Rows.Bottom) {
				int delta = ri.DetailIndentBounds.Top + detailHeight - ViewRects.Rows.Bottom;
				detailHeight -= delta;
				Rectangle bounds = ri.TotalBounds;
				bounds.Height -= delta;
				ri.TotalBounds = bounds;
			}
			ri.DetailIndentBounds.Height = detailHeight;
			ri.DetailBounds = ri.DetailIndentBounds;
			ri.DetailBounds.X = ri.DetailIndentBounds.Right;
			ri.DetailBounds.Width = ri.Bounds.Right - ri.DetailIndentBounds.Right;
			if(IsRightToLeft) {
				ri.DetailBounds.X = ri.Bounds.Left;
				ri.DetailBounds.Width = ri.DetailIndentBounds.X - ri.DetailBounds.X;
			}
			ri.DetailIndicatorBounds = ri.IndicatorRect;
			ri.DetailIndicatorBounds.Height = ri.DetailBounds.Height;
			ri.DetailIndicatorBounds.Y = ri.DetailBounds.Y;
			ri.DetailBounds.Y += DetailVertIndent;
			ri.DetailBounds.Height -= DetailVertIndent * 2;
			if(View.GetShowHorizontalLines()) {
				ri.AddLineInfo(null, ri.DetailIndentBounds.Left,ri.DetailIndentBounds.Bottom - 1,
					ri.DetailIndentBounds.Width, 1, PaintAppearance.HorzLine);
				if(DetailVertIndent > 0) {
					ri.AddLineInfo(null, ri.DetailIndentBounds.Right, ri.DetailIndentBounds.Bottom - 1,
						ri.DataBounds.Right - ri.DetailIndentBounds.Right, 1, PaintAppearance.HorzLine);
					ri.AddLineInfo(null, ri.DetailIndentBounds.Left, ri.DetailIndentBounds.Top,
						ri.DataBounds.Right - ri.DetailIndentBounds.Left, 1, PaintAppearance.HorzLine);
				}
				ri.DetailIndentBounds.Height --;
			}
		}
		protected virtual int GetRowLineCount(int rowHandle, bool isGroupRow) {
			if(isGroupRow) return 1;
			return RowLineCount;
		}
		protected virtual GridRowInfo CheckRowCache(int rowHandle, int newY) {
			if(CachedRows == null) return null;
			GridRowInfo res = CachedRows.FindRow(rowHandle);
			if(res == null) return null;
			res.OffsetContent(0, newY - res.Bounds.Y);
			return res;
		}
		protected internal virtual int CalcRowLevelIndent(GridRowInfo ri, int level) {
			if(ri == null) ri = EmptyDataRow;
			return Painter.ElementsPainter.GroupRow.CalcLevelIndent(ri, level);
		}
		protected virtual int CalcGroupLineHeight(GridRowInfo ri) {
			return 1;
		}
		protected virtual void CalcRowIndents(GridRowInfo ri) {
			if(View.OptionsView.ShowIndicator) {
				var indicator = ri.Bounds;
				indicator.Width = ViewRects.IndicatorWidth;
				if(IsRightToLeft) indicator.X = ri.Bounds.Right - indicator.Width;
				ri.IndicatorRect = indicator;
			}
			ri.Indent = CalcRowLevelIndent(ri, ri.Level);
			ri.IndentRect = ri.TotalBounds;
			Rectangle db;
			if(IsRightToLeft) {
				ri.IndentRect.Width = ri.Indent;
				ri.IndentRect.X = ri.TotalBounds.Right - (ViewRects.IndicatorWidth + ri.Indent);
				if(!HasFixedLeft) ri.IndentRect.X += View.LeftCoord;
				db = new Rectangle(ri.TotalBounds.X, ri.Bounds.Top,
					ri.IndentRect.X - ri.TotalBounds.X, ri.Bounds.Height);
			}
			else {
				ri.IndentRect.X += ViewRects.IndicatorWidth;
				if(!HasFixedLeft) ri.IndentRect.X -= View.LeftCoord;
				ri.IndentRect.Width = ri.Indent;
				db = new Rectangle(ri.IndentRect.Right, ri.Bounds.Top,
				ri.Bounds.Right - ri.IndentRect.Right, ri.Bounds.Height);
				db.Width = ViewRects.DataRectRight - db.X;
			}
			if(ri.IsGroupRow) {
				db = CalcGroupRowDataBounds(ri, db);
			}
			ri.DataBounds = db;
			int separator = GetRowSeparatorHeight(ri.RowHandle, ri.IsGroupRow);
			if(separator != 0) {
				ri.RowSeparatorBounds = ri.Bounds;
				ri.RowSeparatorBounds.Height = separator;
				ri.RowSeparatorBounds.Y = ri.TotalBounds.Bottom - ri.RowSeparatorBounds.Height;
				ri.RowSeparatorBounds.Width = ViewRects.DataRectRight - ri.RowSeparatorBounds.X;
				if(View.GetShowHorizontalLines()) {
					int lineX = ri.Bounds.X;
					Rectangle r = new Rectangle(lineX, ri.RowSeparatorBounds.Bottom - 1, ri.DataBounds.Right - lineX, 1);
					ri.Lines.Add(new IndentInfo(null, r, PaintAppearance.HorzLine));
				}
			}
		}
		protected override ShowButtonModeEnum GetDefaultShowButtonMode(int rowHandle, GridColumn column) {
			if(View.IsEditFormMode) return ShowButtonModeEnum.ShowOnlyInEditor;
			return base.GetDefaultShowButtonMode(rowHandle, column);
		}
		protected virtual Rectangle CalcGroupRowDataBounds(GridRowInfo ri, Rectangle db) {
			if(IsRightToLeft) {
			}
			else {
				if(ViewRects.Rows.Right > db.Right) db.Width = ViewRects.Rows.Right - db.X;
			}
			return db;
		}
		protected virtual void CalcDataRow(GridDataRowInfo ri, GridRow row, GridRow nextRow) {
			if(ri == null) return;
			bool showEditForm = View.IsShowInplaceEditForm(ri.RowHandle) && EditFormRequiredHeight > 0;
			bool showEditFormInstead = showEditForm && View.IsInplaceEditFormHideCurrent;
			string error;
			DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType;
			View.GetColumnError(ri.RowHandle, null, out error, out errorType);
			ri.ErrorText = error;
			if(ri.ErrorText != null && ri.ErrorText.Length > 0) {
				ri.ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorType);
			}
			CalcRowPreviewInfo(ri, showEditForm);
			CalcRowMasterInfo(ri);
			if(!ri.MasterRowExpanded && ri.PreviewBounds.Height > 0 && View.GetShowPreviewRowLines()) {
				ri.PreviewBounds.Height --;
				int x = ri.DataBounds.X;
				if(nextRow == null || nextRow.Level < ri.Level) x = ri.IndentRect.Left + CalcRowLevelIndent(ri, (nextRow == null ? 0 : nextRow.Level));
				ri.AddLineInfo(null, x,
					ri.PreviewBounds.Bottom, ri.DataBounds.Right - x, 1,
					PaintAppearance.HorzLine);
			}
			CalcRowFooterInfo(ri, row, nextRow);
			CalcRowIndentInfo(ri, row, nextRow);
			CalcRowCellsDrawInfo(ri, ColumnsInfo, nextRow);
			if(showEditForm) {
				PrepareEditForm(ri);
			}
		}
		protected virtual void PrepareEditForm(GridDataRowInfo ri) {
			if(View.IsInplaceEditFormHideCurrent) {
				ri.DataBounds = new Rectangle(ri.DataBounds.X, ri.DataBounds.Y, ri.DataBounds.Width, 0);
				ri.Cells.ClearCells();
				ri.PreviewBounds = Rectangle.Empty;
			}
			ri.EditFormRow = true;
			Rectangle r = ViewRects.Rows;
			if(ri.IndicatorRect.Width > 0) {
				ri.IndicatorRect.Height  = ri.TotalBounds.Bottom - ri.IndentRect.Top;
				if(IsRightToLeft) {
					r.Width = ri.IndicatorRect.Left - r.X;
				}
				else {
					r.X = ri.IndicatorRect.Right;
					r.Width = ViewRects.Rows.Right - r.X;
				}
			}
			r.Y = ri.DataBounds.Bottom;
			r.Height = EditFormRequiredHeight;
			if(r.Bottom > ri.TotalBounds.Bottom) r.Height = ri.TotalBounds.Height - r.Y;
			if(r.Bottom > ViewRects.Rows.Bottom) r.Height = ViewRects.Rows.Bottom - r.Y;
			if(r.Height > 0 && EditForm != null) {
				EditFormBounds = r;
				EditForm.Bounds = r;
				if(!EditForm.Visible) {
					EditForm.Dock = DockStyle.None;
					EditForm.Visible = true;
					if(View.EditFormController != null) View.EditFormController.FocusFirst();
				}
				if(View.GetShowHorizontalLines()) {
					int lineX = ri.Bounds.X;
					ri.AddLineInfo(null, lineX, r.Bottom, ri.DataBounds.Right - lineX, 1, PaintAppearance.HorzLine);
				}
			}
			else {
				if(EditForm != null) EditForm.Visible = false;
			}
		}
		protected virtual void UpdateEditFormBounds() {
			if(!AllowUpdateDetails) return;
			if(EditForm == null) return;
			if(EditFormBounds.Width < 1 || EditFormBounds.Height < 1) {
				EditForm.Visible = false;
			}
			else {
				EditForm.Bounds = EditFormBounds;
				EditForm.Visible = true;
			}
		}
		protected virtual void CalcGroupRow(GridGroupRowInfo ri, GridRow row, GridRow nextRow) {
			if(ri == null) return;
			string groupValueText;
			ri.GroupText = View.GetGroupRowDisplayText(ri.RowHandle, true, out groupValueText);
			ri.GroupValueText = groupValueText;
			ri.EditValue = View.GetGroupRowValue(ri.RowHandle);
			ri.GroupExpanded = View.GetRowExpanded(ri.RowHandle);
			ri.DrawMoreIcons = ((ri.IsGroupRow && ri.IsGroupRowExpanded) || row.ForcedRow) && !row.NextRowPrimaryChild;
			ObjectPainter.CalcObjectBounds(GInfo.Graphics, Painter.ElementsPainter.GroupRow, ri);
			CalcSummaryInfo(ri);
			CalcRowFooterInfo(ri, row, nextRow);
			CalcRowIndentInfo(ri, row, nextRow);
		}
		protected virtual void CalcSummaryInfo(GridGroupRowInfo ri) {
			foreach(GridGroupSummaryItem item in View.GroupSummary) {
				if(item.SummaryType == SummaryItemType.None || item.ShowInGroupColumnFooter == null) continue;
				ri.AddColumnSummaryItem(item.ShowInGroupColumnFooter, item, View.GetGroupSummaryValue(ri.RowHandle, item));
			}
		}
		protected virtual void RemoveAnimatedItems(GridRowInfoCollection rows) { 
			if(rows == null) return;
			for(int i = 0; i < rows.Count; i++) {
				RemoveAnimatedItems(rows[i] as GridDataRowInfo);
			}
		}
		protected virtual void CalcRowsDrawInfo() {
			if(AllowUpdateDetails) EditFormBounds = Rectangle.Empty;
			CalcDataRight();
			ViewRects.EmptyRows = Rectangle.Empty;
			int top = ViewRects.Rows.Top;
			GridRow row = null;
			int visibleCount = RowsLoadInfo.ResultRows.Count;
			bool topPositionUpdated = false;
			ViewRects.RowsTotalHeight = 0;
			GridColumnInfoArgs lastColumnInfo = ColumnsInfo.LastColumnInfo;
			GridRowInfo lastRow = null;
			for(int n = 0; n < visibleCount; n++) {
				row = RowsLoadInfo.ResultRows[n];
				if(!topPositionUpdated && View.IsPixelScrolling) {
					topPositionUpdated = CheckUpdateTopPosition(row, ref top);
					if(topPositionUpdated && lastRow != null && lastRow.ForcedRow) {
						if(lastRow.TotalBounds.Bottom > top) lastRow.DrawMoreIcons = true;
					}
				}
				GridRowInfo cached = CheckRowCache(row.RowHandle, top);
				GridRowInfo ri;
				GridRow nextRow = (n + 1 < visibleCount ? RowsLoadInfo.ResultRows[n + 1] : null);
				if(cached != null) {
					this.cachedRows.Remove(cached);
					GridDataRowInfo cachedDataRow = cached as GridDataRowInfo;
					bool allowCachedRow = cachedDataRow == null || cachedDataRow.DetailBounds.IsEmpty;
					if(cachedDataRow != null && cachedDataRow.EditFormRow) allowCachedRow = false;
					if(allowCachedRow) {
						lastRow =  ri = cached;
						top = ri.TotalBounds.Bottom;
						ri.ForcedRow = row.ForcedRow;
						ri.ForcedRowLight = row.ForcedRowLight;
						ri.DrawMoreIcons = !row.NextRowPrimaryChild && (row.ForcedRow || (ri.IsGroupRow && ri.IsGroupRowExpanded)); 
						RowsInfo.AddRow(ri);
						if(top > ViewRects.Rows.Bottom) break;
						continue;
					}
				}
				lastRow = ri = CreateRowInfo(row);
				Rectangle rowBounds = ViewRects.Rows;
				rowBounds = ViewRects.Rows;
				rowBounds.Y = top;
				ri.RowLineHeight = CalcRowHeight(GInfo.Graphics, ri.RowHandle, ri.Level);
				rowBounds.Height = ri.RowLineHeight * GetRowLineCount(ri.RowHandle, ri.IsGroupRow);
				ri.Bounds = rowBounds;
				rowBounds.Height = CalcTotalRowHeight(GInfo.Graphics, ri.RowLineHeight, ri.RowHandle, ri.VisibleIndex, ri.Level, ri.IsGroupRow ? (bool?)null : (bool?)true);
				ri.TotalBounds = rowBounds;
				CalcRowIndents(ri);
				ri.RowFooters.RowFooterCount = GetRowFooterCountEx(ri.RowHandle, ri.VisibleIndex, (ri.IsGroupRow ? (bool?)null : (bool?)true));
				ri.RowFooters.RowFootersHeight = ri.RowFooters.RowFooterCount * GroupFooterHeight;
				CalcDataRow(ri as GridDataRowInfo, row, nextRow);
				CalcGroupRow(ri as GridGroupRowInfo, row, nextRow);
				top = ri.TotalBounds.Bottom;
				RowsInfo.AddRow(ri);
				if(top > ViewRects.Rows.Bottom) break;
			}
			RemoveAnimatedItems(this.cachedRows);
			ViewRects.RowsTotalHeight = top - ViewRects.Rows.Top;
			if(top < ViewRects.Rows.Bottom) {
				Rectangle r = ViewRects.Rows;
				r.Y = top;
				r.Height = ViewRects.Rows.Bottom - top;
				ViewRects.EmptyRows = r;
			}
			CalcRowsMergeInfo();
			if(AllowUpdateDetails) {
				CheckEditFormVisibility();
			}
		}
		void CheckEditFormVisibility() {
			if(EditForm == null) return;
			GridDataRowInfo row = RowsInfo.GetInfoByHandle(View.FocusedRowHandle) as GridDataRowInfo;
			if(row == null || row.TotalBounds.Height < 1) View.HideEditForm();
		}
		private bool CheckUpdateTopPosition(GridRow row, ref int top) {
			if(row.ForcedRow) return false;
			if(View.IsNewItemRow(row.RowHandle) && View.OptionsView.NewItemRowPosition == NewItemRowPosition.Top) return false;
			if(View.IsFilterRow(row.RowHandle)) return false;
			top -= View.TopRowPixel - (CalcPixelPositionByRow(View.TopRowIndex)); 
			return true;
		}
		protected virtual void CalcRowsMergeInfo() {
			if(!IsCellMerging) return;
			CellMerger.Calc();
			CellMerger.CalcEditors();
		}
		protected virtual string GetFooterCellText(GridFooterCellInfoArgs fci) {
			if(fci.SummaryItem == null) return string.Empty;
			return fci.SummaryItem.GetDisplayText(fci.Value, false);
		}
		public virtual void UpdateFooterDrawInfo() {
			GInfo.AddGraphics(null);
			try {
				CalcFooterDrawInfo();
			} finally {
				GInfo.ReleaseGraphics();
			}
		}
		public virtual void CheckFooterData() {
			if(FooterInfo.IsDirty) UpdateFooterData();
		}
		protected virtual void UpdateFooterData() {
			FooterInfo.IsDirty = false;
			foreach(GridFooterCellInfoArgs fci in FooterInfo.Cells) {
				if(fci.Visible) {
					fci.Value = fci.SummaryItem != null ? fci.SummaryItem.SummaryValue : null;
					fci.DisplayText = GetFooterCellText(fci);
				}
			}
		}
		protected virtual void CalcFooterDrawInfo() {
			FooterInfo.IsDirty = false;
			FooterInfo.Cells.Clear();
			FooterInfo.Bounds = ViewRects.Footer;
			Rectangle maxBounds = Painter.ElementsPainter.FooterPanel.GetObjectClientRectangle(new StyleObjectInfoArgs(null, FooterInfo.Bounds, null, null, ObjectState.Normal));
			int maxFooterCount = GetMaxColumnFooterCount();
			foreach(GridColumnInfoArgs ci in ColumnsInfo) {
				if(ci.Type == GridColumnInfoType.Column) {
					int valueCount = Math.Max(1, ci.Column.Summary.ActiveCount);
					if(valueCount < maxFooterCount) valueCount ++;
					for(int n = 0; n < valueCount; n++) {
						CreateFooterCell(ci, n, maxFooterCount, maxBounds);
					}
				}
			}
		}
		protected virtual bool AllowFooterCellUseAllBounds { get { return true; } }
		void CreateFooterCell(GridColumnInfoArgs ci, int n, int maxFooterCount, Rectangle maxBounds) {
			GridFooterCellInfoArgs fci = new GridFooterCellInfoArgs();
			int cellCount = 1;
			fci.SummaryItem = ci.Column.Summary.GetActiveItem(n);
			if(fci.SummaryItem == null) cellCount = maxFooterCount - n;
			Rectangle r = new Rectangle(ci.Bounds.Left, FooterInfo.Bounds.Top,
				ci.Bounds.Width, ci.Info.RowCount * FooterCellHeight);
			r = Painter.ElementsPainter.GroupFooterPanel.CalcCellBounds(maxBounds, r, AllowFooterCellUseAllBounds && maxFooterCount == 1 ? maxBounds.Height : FooterCellHeight, ci.Info.StartRow * maxFooterCount + n * ci.Info.RowCount, ci.Info.RowCount * cellCount);
			fci.Bounds = r;
			fci.SetAppearance(PaintAppearance.FooterPanel.Clone() as AppearanceObject);
			fci.ColumnInfo = ci;
			if(fci.SummaryItem == null) {
				fci.Visible = false;
			}
			else {
				fci.Visible = fci.SummaryItem.Exists;
				fci.Value = fci.SummaryItem.SummaryValue;
				fci.DisplayText = GetFooterCellText(fci);
			}
			FooterInfo.Cells.Add(fci);
		}
		protected GridFooterCellInfoArgs GetFooterCellInfoArgs() {
			GridFooterCellInfoArgs e = new GridFooterCellInfoArgs(GInfo.Cache);
			e.SetAppearance(PaintAppearance.FooterPanel);
			return e;
		}
		protected virtual int CalcFooterCellHeight() {
			int maxY = 0;
			Graphics g = GInfo.AddGraphics(null);
			try {
				GridFooterCellInfoArgs e = GetFooterCellInfoArgs();
				maxY = Painter.ElementsPainter.FooterCell.CalcObjectMinBounds(e).Height + 1;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return Math.Max(maxY, 12);
		}
		protected virtual int CalcGroupFooterHeight() {
			CalcGroupFooterCellHeight();
			int h;
			Graphics g = GInfo.AddGraphics(null);
			try {
				ObjectInfoArgs info = new FooterPanelInfoArgs(GInfo.Cache, ColumnPanelRowCount, GroupFooterCellHeight);
				h = Painter.ElementsPainter.GroupFooterPanel.CalcObjectMinBounds(info).Height;
				if(View.GetShowHorizontalLines()) {
					h ++;
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return Math.Max(h, 12);
		}
		protected virtual void CalcGroupFooterCellHeight() {
			this.groupFooterCellHeight = 12;
			int maxY = 0;
			Graphics g = GInfo.AddGraphics(null);
			try {
				GridFooterCellInfoArgs e = new GridFooterCellInfoArgs(GInfo.Cache);
				e.SetAppearance(PaintAppearance.GroupFooter);
				maxY = Painter.ElementsPainter.GroupFooterCell.CalcObjectMinBounds(e).Height + 1;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			this.groupFooterCellHeight = Math.Max(maxY, this.groupFooterCellHeight);
		}
		protected virtual int CalcMinIndicatorWidth() {
			int res = GridPainter.Indicator.ImageSize.Width;
			Graphics g = GInfo.AddGraphics(null);
			try {
				IndicatorObjectInfoArgs e = new IndicatorObjectInfoArgs(Painter.ElementsPainter.GetIndicatorImages(GridPainter.Indicator));
				e.Graphics = g;
				e.SetAppearance(PaintAppearance.HeaderPanel);
				e.Bounds = new Rectangle(0, 0, res, 16);
				res = Painter.ElementsPainter.Indicator.CalcObjectMinBounds(e).Width;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		protected internal virtual GridColumnInfoArgs CreateColumnInfo(GraphicsCache cache, GridColumn column) {
			GridColumnInfoArgs res = new GridColumnInfoArgs(cache, column);
			res.AutoHeight = View.IsColumnHeaderAutoHeight;
			res.RightToLeft = View.IsRightToLeft;
			UpdateColumnInfo(res);
			return res;
		}
		protected internal GridColumnInfoArgs CreateColumnInfo(GridColumn column) {
			return CreateColumnInfo(null, column);
		}
		protected virtual int CalcMinColumnRowHeight(int headerHeight) {
			headerHeight = Math.Max(headerHeight, 12);
			int maxY = 0;
			Graphics g = GInfo.AddGraphics(null);
			try {
				GridColumnInfoArgs e = CreateColumnInfo(GInfo.Cache, null);
				e.HtmlContext = View;
				e.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), new GlyphElementInfoArgs(View.Images, 0, View.GetMaxHeightColumnImage()), StringAlignment.Near));
				if(View.CanShowFilterButton(null)) 
					e.InnerElements.Add(Painter.ElementsPainter.FilterButton, new GridFilterButtonInfoArgs());
				e.SetAppearance(PaintAppearance.HeaderPanel);
				maxY = Painter.ElementsPainter.Column.CalcObjectMinBounds(e).Height;
				if(View.OptionsView.AllowHtmlDrawHeaders || View.IsColumnHeaderAutoHeight) {
					e.UseHtmlTextDraw = View.OptionsView.AllowHtmlDrawHeaders;
					for(int n = 0; n < Math.Min(ColumnViewInfo.AutoHeightCalculateMaxColumnCount, View.VisibleColumns.Count); n++) {
						GridColumn column = View.VisibleColumns[n];
						AppearanceObject app = new AppearanceObject();
						AppearanceHelper.Combine(app, new AppearanceObject[] { column.AppearanceHeader, PaintAppearance.HeaderPanel });
						e.SetAppearance(app);
						e.Caption = column.GetCaption();
						if(View.IsColumnHeaderAutoHeight) {
							e.CaptionRect = Rectangle.Empty;
							e.Bounds = new Rectangle(0, 0, column.VisibleWidth, 0);
							e.Column = column;
							e.CreateInnerCollection();
							e.UpdateCaption();
						}
						maxY = Math.Max(maxY, Painter.ElementsPainter.Column.CalcObjectMinBounds(e).Height);
					}
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return Math.Max(maxY, headerHeight);
		}
		protected override int MinAllowedEditorHeight { get { return View.FormatRules.Count > 0 ? 16 : 0; } }
		protected virtual int CalcMinRowHeight() {
			int maxH = 0;
			Graphics g = GInfo.AddGraphics(null);
			try {
				AppearanceObject[] appearances = new AppearanceObject[] {
					PaintAppearance.Row,
					PaintAppearance.EvenRow,
					PaintAppearance.OddRow,
					PaintAppearance.FocusedRow
				};
				maxH = CalcMinEditorHeight(appearances);
				if(View.OptionsView.ShowIndicator) {
					ObjectInfoArgs args = new IndicatorObjectInfoArgs(new Rectangle(0, 0, 10, 10), null, null, 0, IndicatorKind.Row);
					args.Cache = GInfo.Cache;
					args.Bounds = Painter.ElementsPainter.Indicator.CalcObjectMinBounds(args);
					maxH = Math.Max(Painter.ElementsPainter.Indicator.CalcBoundsByClientRectangle(args).Height, maxH);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return maxH;
		}
		protected bool IntInRange(int x, int left, int right) {
			if(right < left) {
				int temp = left;
				left = right;
				right = temp;
			}
			return (x >= left && x < right);
		}
		protected virtual GridFooterCellInfoArgs CalcFooterHitInfo(Point pt, bool onlyFixedColumns, GridRowFooterInfo footerInfo) {
			foreach(GridFooterCellInfoArgs fci in footerInfo.Cells) {
				if(onlyFixedColumns && !IsFixedLeftPaint(fci.ColumnInfo) && !IsFixedRightPaint(fci.ColumnInfo)) continue;
				if(GridDrawing.PtInRect(fci.Bounds, pt)) return fci;
			}
			return null;
		}
		protected GridFooterCellInfoArgs CalcFooterHitInfo(Point pt, GridRowFooterInfo footerInfo) {
			if(HasFixedLeft || HasFixedRight) {
				GridFooterCellInfoArgs fci = CalcFooterHitInfo(pt, true, footerInfo);
				if(fci != null) return fci;
			}
			return CalcFooterHitInfo(pt, false, footerInfo);
		}
		protected virtual GridColumnInfoArgs CalcColumnHitInfo(Point pt, bool onlyFixedColumns, GridColumnsInfo cols) {
			foreach(GridColumnInfoArgs ci in cols) {
				if(!IsFixedLeftPaint(ci) && !IsFixedRightPaint(ci) && onlyFixedColumns) continue;
				if(!ci.Bounds.IsEmpty && IntInRange(pt.X, ci.Bounds.Left, ci.Bounds.Right)) {
					if(ci.Type == GridColumnInfoType.EmptyColumn) continue;
					return ci;
				}
			}
			return null;
		}
		protected GridColumnInfoArgs CalcColumnHitInfo(Point pt) { return CalcColumnHitInfo(pt, ColumnsInfo); }
		public virtual GridColumnInfoArgs CalcColumnHitInfo(Point pt, GridColumnsInfo cols) {
			if(HasFixedLeft || HasFixedRight) {
				GridColumnInfoArgs ci = CalcColumnHitInfo(pt, true, cols);
				if(ci != null) return ci;
			}
			return CalcColumnHitInfo(pt, false, cols);
		}
		protected virtual GridCellInfo CalcCellHitInfo(GridDataRowInfo ri, Point pt, bool onlyFixedColumns) {
			foreach(GridCellInfo ci in ri.Cells) {
				if(onlyFixedColumns && !IsFixedLeftPaint(ci.ColumnInfo) && !IsFixedRightPaint(ci.ColumnInfo)) continue;
				GridCellInfo check = ci.MergedCell == null ? ci : ci.MergedCell;
				if(GridDrawing.PtInRect(check.Bounds, pt)) {
					return check;
				}
			}
			return null;
		}
		protected virtual GridCellInfo CalcCellHitInfo(GridDataRowInfo ri, Point pt) {
			if(HasFixedLeft || HasFixedRight) {
				GridCellInfo ci = CalcCellHitInfo(ri, pt, true);
				if(ci != null) {
					if(ci.MergedCell != null) return ci.MergedCell;
					return ci;
				}
			}
			return CalcCellHitInfo(ri, pt, false);
		}
		private bool IsPointInFilterButton(GridColumnInfoArgs ci, Point pt) {
			foreach(DrawElementInfo info in ci.InnerElements) {
				if(!info.Visible) continue;
				if(!(info.ElementInfo is GridFilterButtonInfoArgs)) continue;
				if(GridDrawing.PtInRect(info.ElementInfo.Bounds, pt)) {
					return true;
				}
			}
			return false;
		}
		protected virtual void CalcRowHitInfo(Point pt, GridRowInfo ri, GridHitInfo hi) {
			if(ri.IsGroupRow) 
				CalcGroupRowHitInfo(pt, ri as GridGroupRowInfo, hi);
			else
				CalcDataRowHitInfo(pt, ri as GridDataRowInfo, hi);
			if(hi.CheckHitTest(ri.RowFooters.Bounds, pt, GridHitTest.RowFooter)) {
				foreach(GridRowFooterInfo rfi in ri.RowFooters) {
					if(GridDrawing.PtInRect(rfi.Bounds , pt)) {
						hi.SetRowHandle(rfi.RowHandle, View.GetDataSourceRowIndex(rfi.RowHandle));
						GridFooterCellInfoArgs fci = CalcFooterHitInfo(pt, rfi);
						if(fci != null && fci.ColumnInfo != null) {
							hi.FooterCell = fci;
							hi.ColumnInfo = fci.ColumnInfo;
							hi.Column = fci.Column;
						}
						return;
					}
				}
				return;
			}
		}
		protected virtual void CalcGroupRowHitInfo(Point pt, GridGroupRowInfo ri, GridHitInfo hi) {
			if(hi.CheckHitTest(ri.IndicatorRect, pt, GridHitTest.RowIndicator)) return;
			if(hi.CheckHitTest(ri.Bounds, pt, GridHitTest.Row)) {
				hi.CheckHitTest(ri.ButtonBounds, pt, GridHitTest.RowGroupButton);
				if(ri.SelectorInfo != null) hi.CheckHitTest(ri.SelectorInfo.Bounds, pt, GridHitTest.RowGroupCheckSelector);
				if(IsAlignGroupRowSummariesUnderColumns && hi.HitTest == GridHitTest.Row) {
					CalcGroupRowCellHitInfo(pt, ri, hi);
				}
			}
		}
		void CalcGroupRowCellHitInfo(Point pt, GridGroupRowInfo ri, GridHitInfo hi) {
			foreach(GridColumnInfoArgs ci in ColumnsInfo) {
				if(ci.Column != null && ci.Column.GroupIndex < 0) {
					if(hi.CheckHitTest(GetGroupRowCellBounds(ri, ci), pt, GridHitTest.RowGroupCell)) {
						hi.ColumnInfo = ci;
						hi.Column = ci.Column;
						return;
					}
				}
			}
		}
		protected virtual void CalcDataRowHitInfo(Point pt, GridDataRowInfo ri, GridHitInfo hi) {
			if(hi.CheckHitTest(ri.PreviewBounds, pt, GridHitTest.RowPreview)) return;
			if(hi.CheckHitTest(ri.IndicatorRect, pt, GridHitTest.RowIndicator)) {
				hi.CheckHitTest(pt.Y, ri.DataBounds.Bottom - 2, ri.DataBounds.Bottom, GridHitTest.RowEdge);
				return;
			}
			if(ri.IsMasterRow) {
				if(hi.CheckHitTest(ri.DetailIndicatorBounds, pt, GridHitTest.RowDetailIndicator)) {
					hi.CheckHitTest(pt.Y, ri.DetailBounds.Bottom - 3, ri.DetailBounds.Bottom, GridHitTest.RowDetailEdge);
					return;
				}
				if(IntInRange(pt.X, ri.DetailIndentBounds.Left, ri.DetailBounds.Right) &&
						IntInRange(pt.Y, ri.DetailBounds.Top, ri.DetailBounds.Bottom)) {
					hi.HitTest = GridHitTest.RowDetail;
					hi.CheckHitTest(pt.Y, ri.DetailBounds.Bottom - 3, ri.DetailBounds.Bottom, GridHitTest.RowDetailEdge);
					return;
				}
			}
			if(hi.CheckHitTest(ri.Bounds, pt, GridHitTest.Row)) {
				GridCellInfo ci = CalcCellHitInfo(ri, pt);
				if(ci != null) {
					hi.ColumnInfo = ci.ColumnInfo;
					hi.CellInfo = ci;
					switch(ci.ColumnInfo.Type) {
						case GridColumnInfoType.Indicator :
							hi.HitTest = GridHitTest.RowIndicator;
							break;
						case GridColumnInfoType.BehindColumn:
							hi.HitTest = GridHitTest.Row;
							return;
						case GridColumnInfoType.EmptyColumn :
							hi.HitTest = GridHitTest.Row;
							return;
					}
					if(hi.HitTest == GridHitTest.Row) {
						hi.HitTest = GridHitTest.RowCell;
						hi.Column = ci.Column;
						if(GridDrawing.PtInRect(ci.CellButtonRect, pt))
							hi.HitTest = GridHitTest.CellButton;
					}
				}
				if(hi.HitTest == GridHitTest.RowCell && (ci is GridMergedCellInfo) &&
					ci.Bounds.Bottom > ri.DataBounds.Bottom) return;
				hi.CheckHitTest(pt.Y, ri.DataBounds.Bottom - 2, ri.DataBounds.Bottom, GridHitTest.RowEdge);
				if(hi.Column == null) hi.Column = GetNearestColumn(pt);
				return;
			}
			hi.HitTest = GridHitTest.Row;
			return;
		}
		public GridColumn GetNearestColumn(Point pt) {
			return GetNearestColumn(pt, 10000);
		}
		protected virtual GridColumn GetNearestColumn(Point pt, int minDelta) {
			if(!IsReady) return null;
			GridColumn res = null;
			res = GetNearestColumnCore(pt, ref minDelta, true);
			if(res != null) return res;
			return GetNearestColumnCore(pt, ref minDelta, false);
		}
		GridColumn GetNearestColumnCore(Point pt, ref int minDelta, bool onlyFixedRight) {
			GridColumn res = null;
			foreach(GridColumnInfoArgs info in ColumnsInfo) {
				if(info.Column == null) continue;
				if(onlyFixedRight && !IsFixedRightPaint(info)) continue;
				if(info.Bounds.Contains(pt)) return info.Column;
				if(IntInRange(pt.X, info.Bounds.X, info.Bounds.Right)) return info.Column;
				int delta = Math.Abs(pt.X - info.Bounds.X);
				if(delta < minDelta && !onlyFixedRight) {
					res = info.Column;
					minDelta = delta;
				}
			}
			return res;
		}
		public int GetNearestRowHandle(Point pt) {
			GridRowInfo row = GetNearestRow(pt);
			if(row == null) return GridControl.InvalidRowHandle;
			return row.RowHandle;
		}
		public virtual GridRowInfo GetNearestRow(Point pt) {
			return GetNearestRow(pt, false);
		}
		public virtual GridRowInfo GetNearestRow(Point pt, bool onlyDataRows) {
			GridRowInfo res = EmptyDataRow;
			if(!IsReady) return res;
			int minDelta = 10000;
			foreach(GridRowInfo row in RowsInfo) {
				if(onlyDataRows) {
					GridDataRowInfo dr = row as GridDataRowInfo;
					if(dr == null || dr.IsSpecialRow) continue;
				}
				if(row.TotalBounds.Contains(pt)) return row;
				int delta = Math.Abs(pt.Y - row.TotalBounds.Top);
				if(delta < minDelta) {
					res = row;
					minDelta = delta;
				}
			}
			return res;
		}
		public virtual GridHitInfo CalcHitInfo(Point pt) {
			GridHitInfo hi = CreateHitInfo();
			hi.View = View;
			hi.HitPoint = pt;
			if(!IsReady || IsDataDirty) return hi;
			if(View.CustomizationForm != null && View.CustomizationForm.Visible) {
				if(hi.CheckHitTest(View.CustomizationForm.RectangleToScreen(new Rectangle(Point.Empty, View.CustomizationForm.Bounds.Size)), View.GridControl.PointToScreen(pt), GridHitTest.CustomizationForm)) return hi;
			}
			if(CheckMasterTabHitTest(hi, pt)) {
				hi.HitTest = GridHitTest.MasterTabPageHeader;
				return hi;
			}
			if(hi.CheckHitTest(ViewRects.ViewCaption, pt, GridHitTest.ViewCaption)) return hi;
			if(hi.CheckHitTest(ViewRects.GroupPanel, pt, GridHitTest.GroupPanel)) {
				return CalcGroupPanelHitInfo(pt, hi);
			}
			if(View.ScrollInfo != null && View.ScrollInfo.VScrollVisible && !View.ScrollInfo.IsOverlapHScrollBar && hi.CheckHitTest(View.ScrollInfo.VScrollRect, pt, GridHitTest.VScrollBar)) return hi;
			if(View.ScrollInfo != null && View.ScrollInfo.HScrollVisible && !View.ScrollInfo.IsOverlapScrollBar && hi.CheckHitTest(View.ScrollInfo.HScrollRect, pt, GridHitTest.HScrollBar)) return hi;
			if(HasFixedLeft || HasFixedRight) {
				Rectangle r;
				if(HasFixedLeft) {
					r = ViewRects.FixedLeft;
					r.Width = View.FixedLineWidth;
					r.X = ViewRects.FixedLeft.Right - r.Width;
					if(hi.CheckHitTest(r, pt, GridHitTest.FixedLeftDiv)) return hi;
				}
				if(HasFixedRight) {
					r = ViewRects.FixedRight;
					r.Width = View.FixedLineWidth;
					r.X -= r.Width;
					if(hi.CheckHitTest(r, pt, GridHitTest.FixedRightDiv)) return hi;
				}
			}
			if(View.OptionsView.ShowColumnHeaders && hi.CheckHitTest(ViewRects.ColumnPanel, pt, GridHitTest.ColumnPanel)) {
				return CalcColumnPanelHitInfo(pt, hi);
			}
			if(GridDrawing.PtInRect(ViewRects.Rows, pt)) {
				return CalcRowsHitInfo(pt, hi);
			}
			if(hi.CheckHitTest(ViewRects.Footer, pt, GridHitTest.Footer)) {
				GridFooterCellInfoArgs fci = CalcFooterHitInfo(pt, FooterInfo);
				if(fci != null && fci.ColumnInfo != null) {
					hi.Column = fci.Column;
					hi.FooterCell = fci;
				}
				return hi;
			}
			if(hi.CheckHitTest(FilterPanel.Bounds, pt, GridHitTest.FilterPanel)) {
				hi.CheckHitTest(FilterPanel.TextBounds, pt, GridHitTest.FilterPanelText);
				hi.CheckHitTest(FilterPanel.CloseButtonInfo.Bounds, pt, GridHitTest.FilterPanelCloseButton);
				hi.CheckHitTest(FilterPanel.ActiveButtonInfo.Bounds, pt, GridHitTest.FilterPanelActiveButton);
				hi.CheckHitTest(FilterPanel.CustomizeButtonInfo.Bounds, pt, GridHitTest.FilterPanelCustomizeButton);
				hi.CheckHitTest(FilterPanel.MRUButtonInfo.Bounds, pt, GridHitTest.FilterPanelMRUButton);
				return hi;
			}
			return hi;
		}
		GridHitInfo CalcRowsHitInfo(Point pt, GridHitInfo hi) {
			foreach(GridRowInfo ri in RowsInfo) {
				if(GridDrawing.PtInRect(ri.TotalBounds, pt)) {
					hi.SetRowHandle(ri.RowHandle, View.GetDataSourceRowIndex(ri.RowHandle));
					CalcRowHitInfo(pt, ri, hi);
					return hi;
				}
			}
			hi.HitTest = GridHitTest.EmptyRow;
			return hi;
		}
		GridHitInfo CalcGroupPanelHitInfo(Point pt, GridHitInfo hi) {
			GroupPanelRow row = GroupPanel.Rows.RowByPoint(pt);
			if(row != null) {
				GridColumnInfoArgs ci = row.ColumnByPoint(pt);
				if(ci != null && ci.Type == GridColumnInfoType.Column) {
					hi.Column = ci.Column;
					hi.HitTest = GridHitTest.GroupPanelColumn;
					if(IsPointInFilterButton(ci, pt))
						hi.HitTest = GridHitTest.GroupPanelColumnFilterButton;
				}
			}
			return hi;
		}
		GridHitInfo CalcColumnPanelHitInfo(Point pt, GridHitInfo hi) {
			GridColumnInfoArgs ci = CalcColumnHitInfo(pt);
			if(ci == null || !GridDrawing.PtInRect(ci.Bounds, pt))
				return hi;
			hi.ColumnInfo = ci;
			switch(ci.Type) {
				case GridColumnInfoType.Indicator:
					hi.HitTest = GridHitTest.ColumnButton;
					return hi;
				case GridColumnInfoType.EmptyColumn:
					return hi;
				case GridColumnInfoType.BehindColumn:
					return hi;
				default:
					hi.Column = ci.Column;
					hi.HitTest = GridHitTest.Column;
					bool leftEdge, rightEdge;
					leftEdge = IntInRange(pt.X, ci.Bounds.Left, ci.Bounds.Left + ControlUtils.ColumnResizeEdgeSize);
					rightEdge = IntInRange(pt.X, ci.Bounds.Right - ControlUtils.ColumnResizeEdgeSize, ci.Bounds.Right);
					if(leftEdge || rightEdge) {
						if(IsRightToLeft) {
							if(rightEdge && (ci.Column != null && ci.Column.VisibleIndex == 0)) break;
							if(leftEdge && ci.Column != null) {
								if(IsFixedRight(ci)) break;
							}
							hi.HitTest = GridHitTest.ColumnEdge;
							if(rightEdge && ci.Column != null) {
								CheckLeftEdgeHitTest(hi, ci);
							}
						}
						else {
							if(leftEdge && (ci.Column != null && ci.Column.VisibleIndex == 0))
								break;
							if(rightEdge && (ci.Column != null && ci.Column.VisibleIndex == View.VisibleColumns.Count - 1 && View.OptionsView.ColumnAutoWidth))
								break;
							if(leftEdge && ci.Column != null) {
								if(IsFixedRight(ci)) break;
							}
							hi.HitTest = GridHitTest.ColumnEdge;
							if(leftEdge && ci.Column != null) {
								CheckLeftEdgeHitTest(hi, ci);
							}
						}
					}
					if(IsPointInFilterButton(ci, pt)) {
						hi.HitTest = GridHitTest.ColumnFilterButton;
					}
					break;
			}
			return hi;
		}
		protected virtual void CheckLeftEdgeHitTest(GridHitInfo hi, GridColumnInfoArgs ci) {
			int deltaDirection = IsRightToLeft ? 1 : 1;
			GridColumn col = View.GetVisibleColumn(ci.Column.VisibleIndex - deltaDirection);
			if(ColumnsInfo[col] != null) {
				hi.Column = col;
				hi.ColumnInfo = ColumnsInfo[col];
			}
		}
		public GridRowCollection LoadRows(GridRowsLoadInfo e) {
			while(true) {
				try {
					GridRowCollection res = LoadRowsCore(e);
					return res;
				}
				catch {
					if(View.IsAsyncInProgress) continue;
					throw;
				}
			}
		}
		protected virtual GridRowCollection LoadRowsCore(GridRowsLoadInfo e) {
			return View.Scroller.LoadRowsCore(this, e);
		}
		protected virtual int GetRowSeparatorHeight(int rowHandle, bool isGroupRow) {
			if(View.IsFilterRow(rowHandle) || (View.IsNewItemRow(rowHandle) && NewItemRow == NewItemRowPosition.Top)) {
				return Painter.ElementsPainter.SpecialTopRowIndent.CalcObjectMinBounds(new ObjectInfoArgs(GInfo.Cache)).Height;				
			}
			if(isGroupRow || View.RowSeparatorHeight == 0) return 0;
			if(rowHandle >= (!View.IsDesignMode ? View.DataRowCount - (NewItemRow == NewItemRowPosition.Bottom ? 0 : 1) : 1)) return 0;
			return View.RowSeparatorHeight + (View.GetShowHorizontalLines() ? 1 : 0);
		}
		protected virtual int GetHorzLineHeight(int rowHandle) {
			if(!View.GetShowHorizontalLines()) return 0;
			if(!AllowPartialGroups) return 1;
			return GetHorzLineHeight(new GridRowInfo(this, rowHandle, View.SortInfo.GroupCount) { VisibleIndex = View.GetVisibleIndex(rowHandle) });
		}
		protected virtual int GetHorzLineHeight(GridRowInfo ri) {
			if(!View.GetShowHorizontalLines()) return 0;
			if(!AllowPartialGroups) return 1;
			if(IsShowRowBreak(ri, null)) return GetGroupDividerHeight();
			return 1;
		}
		public virtual int CalcTotalRowHeight(Graphics g, int rowLineHeight, int rowHandle, int rowVisibleIndex, int level, bool? isRowExpanded) {
			bool isGroupRow = View.IsGroupRow(rowHandle);
			if(rowLineHeight < 1) rowLineHeight = CalcRowHeight(g, rowHandle, level);
			int rowDataHeight;
			int result = rowDataHeight = rowLineHeight * GetRowLineCount(rowHandle, isGroupRow);
			result += CalcRowPreviewHeight(rowHandle);
			result += GetTotalFooterRowHeight(rowHandle, rowVisibleIndex, isRowExpanded);
			result += CalcDetailRowHeight(rowHandle);
			result += GetRowSeparatorHeight(rowHandle, isGroupRow);
			result += GetEditFormHeight(rowHandle);
			if(View.IsShowInplaceEditForm(rowHandle) && View.IsInplaceEditFormHideCurrent) {
				result -= (rowDataHeight + CalcRowPreviewHeight(rowHandle));
			}
			return result;
		}
		internal bool allowEditFormHeight = true;
		protected virtual int GetEditFormHeight(int rowHandle) {
			if(!View.IsInplaceEditForm) return 0;
			if(View.IsShowInplaceEditForm(rowHandle) && allowEditFormHeight) {
				int height = EditFormRequiredHeight;
				if(height > 0) height += View.GetShowHorizontalLines() ? 1 : 0;
				return height;
			}
			return 0;
		}
		protected virtual int GetRowFooterCountEx(int rowHandle, int rowVisibleIndex, bool? isRowExpanded) {
			if(!View.IsShowRowFooters()) return 0;
			bool isExpanded = isRowExpanded.HasValue ? isRowExpanded.Value : View.GetRowExpanded(rowHandle);
			int nextRowHandle = View.GetVisibleRowHandle(View.GetNextVisibleRow(rowVisibleIndex));
			return GetRowFooterCount(rowHandle, nextRowHandle, isExpanded);
		}
		public virtual int GetRowFooterCount(int rowHandle, int nextRowHandle, bool isExpanded) {
			if(View.IsFilterRow(rowHandle) || View.IsNewItemRow(rowHandle)) return 0;
			int retCount = 0;
			bool isGroup = View.IsGroupRow(rowHandle);
			int nextLevel = nextRowHandle == GridControl.InvalidRowHandle ? 0 : View.GetRowLevel(nextRowHandle);
			int currentLevel = View.GetRowLevel(rowHandle);
			if(isGroup) {
				if(isExpanded) return 0;
				if(!isExpanded) {
					retCount = View.OptionsView.GroupFooterShowMode == GroupFooterShowMode.VisibleAlways ? 1 : 0;
					if(nextLevel >= currentLevel) return retCount;
					retCount += (currentLevel - nextLevel);
				} 
				else
					retCount = 1;
			} else {
				if(nextRowHandle == GridControl.InvalidRowHandle || View.IsGroupRow(nextRowHandle) || View.IsNewItemRow(nextRowHandle)) {
					if(nextLevel >= currentLevel) return 0;
					retCount = currentLevel - nextLevel;
				} else
					retCount = 0;
			}
			if(retCount > 0) {
				if(!View.IsExistAnyRowFooterCell(rowHandle)) retCount = View.OptionsView.GroupFooterShowMode == GroupFooterShowMode.VisibleAlways ? 1 : 0;
			}
			return retCount;
		}
		protected virtual int GetTotalFooterRowHeight(int rowHandle, int rowVisibleIndex, bool? isRowExpanded) {
			int count = GetRowFooterCountEx(rowHandle, rowVisibleIndex, isRowExpanded);
			if(count == 0) return 0;
			return GroupFooterHeight * count; 
		}
		public virtual ScrollBarPresence HScrollBarPresence {
			get {
				if(hScrollBarPresence == ScrollBarPresence.Unknown) UpdateHScrollBarPresence(false);
				return hScrollBarPresence;
			}
		}
		public virtual ScrollBarPresence VScrollBarPresence {
			get { 
				if(vScrollBarPresence == ScrollBarPresence.Unknown) UpdateVScrollBarPresence(false);
				return vScrollBarPresence;
			}
		}
		public virtual ScrollVisibility HorzScrollVisibility { 
			get {
				if(View.GridControl != null && View.GridControl.UseEmbeddedNavigator && View.GridControl.DefaultView == View) return ScrollVisibility.Always;
				return View.HorzScrollVisibility;
			}
		}
		protected virtual void UpdateHScrollBarPresence(bool adv) {
			if(HorzScrollVisibility != ScrollVisibility.Auto) {
				if(HorzScrollVisibility == ScrollVisibility.Never) {
					this.hScrollBarPresence = ScrollBarPresence.Hidden;
				} else {
					this.hScrollBarPresence = ScrollBarPresence.Visible;
				}
				return;
			}
			if(View.OptionsView.ColumnAutoWidth && !View.AllowScrollAutoWidth) {
				this.hScrollBarPresence = ScrollBarPresence.Hidden;
				return;
			}
			this.hScrollBarPresence = ScrollBarPresence.Calculable;
			if(!adv) {
				if(View.LeftCoord > 0 || ViewRects.ColumnTotalWidth - VScrollSize > ViewRects.ColumnPanelWidth) {
					if(View.OptionsView.ColumnAutoWidth && View.LeftCoord == 0) 
						hScrollBarPresence = ScrollBarPresence.Calculable;
					else
					this.hScrollBarPresence = ScrollBarPresence.Visible;
				}
				return;
			}
			if(ViewRects.ColumnTotalWidth > ViewRects.ColumnPanelWidth || View.LeftCoord > 0) {
				this.hScrollBarPresence = ScrollBarPresence.Visible;
			} else {
				this.hScrollBarPresence = ScrollBarPresence.Hidden;
			}
		}
		public NewItemRowPosition NewItemRow { get { return View.OptionsView.NewItemRowPosition; } }
		public virtual bool IsAnyCalculableScrollBars {
			get { return HScrollBarPresence == ScrollBarPresence.Calculable || VScrollBarPresence == ScrollBarPresence.Calculable; }
		}
		protected virtual void UpdateVScrollBarPresence(bool adv) {
			if(View.VertScrollVisibility != ScrollVisibility.Auto) {
				if(View.VertScrollVisibility == ScrollVisibility.Never) {
					this.vScrollBarPresence = ScrollBarPresence.Hidden;
				} else {
					this.vScrollBarPresence = ScrollBarPresence.Visible;
				}
				return;
			}
			this.vScrollBarPresence = ScrollBarPresence.Calculable;
			if(View.RowCount == 1) {
				this.vScrollBarPresence = ScrollBarPresence.Hidden;
				return;
			}
			if(!adv) return;
			if(!RowsLoadInfo.ResultAllRowsFit || ((View.IsPixelScrolling ? ViewTopRowPixel : ViewTopRowIndex) > 0)) {
				this.vScrollBarPresence = ScrollBarPresence.Visible;
				return;
			}
			if(RowsLoadInfo.UseMinRowHeight) return;
			this.vScrollBarPresence = ScrollBarPresence.Hidden;
		}
		protected internal virtual int ViewTopRowIndex { get { return IsRealHeightCalculate ? 0 : View.TopRowIndex; } }
		protected internal virtual int ViewTopRowPixel { get { return IsRealHeightCalculate ? 0 : View.TopRowPixel; } }
		protected virtual void UpdateScrollBarPresence(Rectangle bounds, bool adv) {
			UpdateVScrollBarPresence(adv);
			if(adv) CalcRects(bounds, true);
			UpdateHScrollBarPresence(adv);
			if(adv) CalcRects(bounds, true);
		}
		protected virtual Rectangle CalcTabClientRect() {
			Rectangle r = CalcTabClientRect(CalcBorderRect(ViewRects.Bounds));
			return r;
		}
		protected virtual Rectangle CalcScrollRect() {
			Rectangle r = CalcTabClientRect();
			return r;
		}
		protected internal int HScrollSize { get { return View.ScrollInfo.HScrollSize;} }
		protected internal int VScrollSize { get { return View.ScrollInfo.VScrollSize; } }
		protected virtual Rectangle CalcClientRect() {
			Rectangle r = CalcScrollRect();
			if(VScrollBarPresence != ScrollBarPresence.Hidden) {
				if(!View.ScrollInfo.IsOverlapScrollBar) {
					if(IsRightToLeft) r.X += VScrollSize;
					r.Width = Math.Max(0, r.Width - VScrollSize);
				}
			}
			return r;
		}
		protected internal virtual void RemoveInvisibleAnimations() {
			if(View == null || !View.OptionsImageLoad.AsyncLoad) return;
			for(int i = 0; i < XtraAnimator.Current.Animations.Count; i++ ) {
				if(XtraAnimator.Current.Animations[i] is GridViewImageShowingAnimationInfo) {
					GridViewImageShowingAnimationInfo ai = (GridViewImageShowingAnimationInfo)XtraAnimator.Current.Animations[i];
					RemoveInvisibleAnimationCore(ai);
				}
			}
		}
		protected internal void RemoveInvisibleAnimationCore(GridViewImageShowingAnimationInfo ai) {
			if(ai.Item == null || ai.Item.LoadInfo == null) return;
			int rowHandle = View.GetRowHandle(ai.Item.LoadInfo.DataSourceIndex);
			if(View.IsRowVisible(rowHandle) != RowVisibleState.Hidden) return;
			ai.Item.LoadInfo.IsInAnimation = false;
			XtraAnimator.Current.Animations.Remove(ai);
		}
		protected virtual void CalcRectsConstants() {
			CalcColumnTotalWidth();
			ViewRects.MinIndicatorWidth = CalcMinIndicatorWidth();
			this.columnRowHeight = CalcMinColumnRowHeight(ScaleVertical(View.ColumnPanelRowHeight));
			this.groupFooterHeight = CalcGroupFooterHeight();
			this.footerCellHeight = CalcFooterCellHeight();
			this.minRowHeight = CalcMinRowHeight();
			this.actualMinRowHeight = -1;
			CalcGroupRowHeight();
		}
		protected virtual void CalcColumnTotalWidth() {
			ViewRects.ColumnTotalWidth = CalcTotalColumnWidth();
		}
		protected virtual void CalcDataRight() {
			int res = ViewRects.ColumnPanel.Right;
			GridColumnInfoArgs lastColumnInfo = ColumnsInfo.LastColumnInfo;
			if(lastColumnInfo != null && lastColumnInfo.Bounds.Right < res)
				res = lastColumnInfo.Bounds.Right;
			ViewRects.DataRectRight = res;
			if(IsRightToLeft) ViewRects.DataRectRight = viewRects.ColumnPanel.Right;
		}
		public virtual void CalcRects(Rectangle bounds, bool partital) {
			Rectangle r = Rectangle.Empty;
			ViewRects.Bounds = bounds;
			ViewRects.Scroll = CalcScrollRect();
			ViewRects.Client = CalcClientRect();
			FilterPanel.Bounds = Rectangle.Empty;
			if(!partital) {
				CalcRectsConstants();
			}
			if(View.OptionsView.ShowIndicator) {
				ViewRects.IndicatorWidth = Math.Max(ScaleHorizontal(View.IndicatorWidth), ViewRects.MinIndicatorWidth);
			}
			int minTop = ViewRects.Client.Top;
			int maxBottom = ViewRects.Client.Bottom;
			if(View.OptionsView.ShowViewCaption) {
				r = ViewRects.Scroll;
				r.Y = minTop;
				r.Height = CalcViewCaptionHeight(ViewRects.Client);
				ViewRects.ViewCaption = r;
				minTop = ViewRects.ViewCaption.Bottom;
			}
			minTop = UpdateFindControlVisibility(new Rectangle(ViewRects.Scroll.X, minTop, ViewRects.Scroll.Width, maxBottom - minTop), false).Y;
			if(View.OptionsView.ShowGroupPanel) {
				r = ViewRects.Scroll;
				r.Y = minTop;
				r.Height = CalcGroupPanelHeight();
				ViewRects.GroupPanel = r;
				minTop = ViewRects.GroupPanel.Bottom;
			}
			minTop = CalcRectsColumnPanel(minTop);
			ViewRects.VScrollLocation = minTop;
			if(View.IsShowFilterPanel) {
				r = ViewRects.Scroll;
				int fPanel = GetFilterPanelHeight();
				r.Y = maxBottom - fPanel;
				r.Height = fPanel;
				FilterPanel.Bounds = r;
				maxBottom = r.Top;
			}
			ViewRects.HScrollLocation = maxBottom;
			if(HScrollBarPresence == ScrollBarPresence.Visible) {
				if(!View.ScrollInfo.IsOverlapHScrollBar) maxBottom -= HScrollSize;
			}
			if(View.OptionsView.ShowFooter) {
				r = ViewRects.Scroll;
				r.Height = GetFooterPanelHeight();
				r.Y = maxBottom - r.Height;
				ViewRects.Footer = r;
				maxBottom = r.Top;
			}
			r = ViewRects.Client;
			r.Y = minTop;
			r.Height = maxBottom - minTop;
			ViewRects.Rows = r;
		}
		public virtual int ColumnPanelRowCount { get { return 1; } }
		public virtual int RowLineCount { get { return 1; } }
		protected virtual int CalcRectsColumnPanel(int startY) {
			Rectangle r = ViewRects.Client;
			r.Y = startY;
			r.Height = ColumnRowHeight * ColumnPanelRowCount;
			ViewRects.ColumnPanel = r;
			if(View.OptionsView.ShowColumnHeaders) {
				startY = ViewRects.ColumnPanel.Bottom;
			}
			return startY;
		}
		public virtual void CalcAfterHorzScroll(Graphics g, Rectangle bounds) {
			if(!IsReady) return;
			GInfo.AddGraphics(g);
			LockSelectionInfo();
			try {
				Clear();
				CalcConstants();
				CalcGridInfo();
			} finally {
				UnlockSelectionInfo();
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual int GetAutoWidthColumnPanelWidth() {
			int res = ViewRects.ColumnPanelWidth;
			if(HasFixedLeft) res -= View.FixedLineWidth;
			if(HasFixedRight) res -= View.FixedLineWidth;
			return res;
		}
		public virtual void RecalcColumnWidthes() {
			RecalcColumnWidthes(-1, View.OptionsView.ColumnAutoWidth, GetAutoWidthColumnPanelWidth());
		}
		public virtual void RecalcColumnWidthes(GridColumn column) {
			RecalcColumnWidthes(column.VisibleIndex + 1, View.OptionsView.ColumnAutoWidth, GetAutoWidthColumnPanelWidth());
		}
		public void RecalcColumnWidthes(int startColumn, bool isAutoWidth, int maxVWidth) {
			if(maxVWidth == 0) return;
			RecalcColumnWidthes(new GridAutoWidthCalculatorArgs(null, isAutoWidth, maxVWidth, startColumn, -1));
		}
		public virtual void RecalcColumnWidthes(GridAutoWidthCalculatorArgs args) {
			WidthCalculator.Calc(args);
			WidthCalculator.UpdateRealObjects(args);
		}
		protected override void UpdateTabControl() {
			if(IsRealHeightCalculate) return;
			if(!AllowUpdateDetails) return;
			if(!ShowTabControl) {
				TabControl.Bounds = Rectangle.Empty;
				return;
			} 
			TabControl.Bounds = CalcBorderRect(ViewRects.Bounds);
		}
		public override void SetPaintAppearanceDirty() {
			base.SetPaintAppearanceDirty();
		}
		protected virtual int CalcScrollRowHeight() {
			int res = ViewRects.Rows.Height;
			if(View.IsPixelScrolling) {
				res += CalcTotalRowHeight(null, 0, View.GetVisibleRowHandle(View.TopRowIndex), View.TopRowIndex, 0, null) + ActualDataRowMinRowHeight;
			}
			return res;
		}
		public override void Calc(Graphics g, Rectangle bounds) {
			if(IsNull) return;
			PositionHelper.Check();
			ValidateTopRowIndexByPixel();
			base.CalcViewInfo();
			CheckNavigator();
			CalcConstants();
			ShowDetailButtons = View.IsShowDetailButtons;
			ViewRects.Clear();
			UpdateFixedColumnInfo();
			GInfo.AddGraphics(g);
			try {
				Clear();
				CalcRects(bounds, false);
				UpdateTabControl();
				UpdateScrollBarPresence(bounds, false);
				PositionHelper.Check();
				CalcRects(bounds, true);
				RecalcColumnWidthes();
				if(View.IsColumnHeaderAutoHeight) CalcRects(bounds, false);
				CalcColumnTotalWidth();
				int savedTotalWidth = ViewRects.ColumnTotalWidth;
				GridRowsLoadInfo prevInfo = this.rowsLoadInfo;
				this.rowsLoadInfo = new GridRowsLoadInfo(GInfo.Graphics, -1, CalcScrollRowHeight(), false, !IsRowAutoHeight);
				LoadRows(RowsLoadInfo);
				if(IsAnyCalculableScrollBars) {
					UpdateScrollBarPresence(bounds, true);
					RecalcColumnWidthes();
					CalcColumnTotalWidth();
				}
				bool needRecalc = IsAnyCalculableScrollBars;
				if(IsRowAutoHeight && savedTotalWidth != ViewRects.ColumnTotalWidth) {
					needRecalc = true;
					AutoRowHeightCache.Clear();
				}
				if(needRecalc) {
					int savedWidth = ViewRects.Rows.Width;
					this.rowsLoadInfo = new GridRowsLoadInfo(GInfo.Graphics, -1, CalcScrollRowHeight(), false, false);
					LoadRows(RowsLoadInfo);
					UpdateScrollBarPresence(bounds, true);
					RecalcColumnWidthes();
					if(View.IsColumnHeaderAutoHeight) CalcRects(bounds, false);
					if(savedWidth != ViewRects.Rows.Width && (RowsLoadInfo.HasDetails || View.OptionsView.ShowPreview)) {
						this.rowsLoadInfo = new GridRowsLoadInfo(GInfo.Graphics, -1, CalcScrollRowHeight(), false, false);
						LoadRows(RowsLoadInfo);
					}
				}
				CalcRects(bounds, true);
				CalcGridInfo();
				UpdateFixedRects();
				DoUnloadRows(RowsInfo, prevInfo);
				RowsInfo.VertScrollBarVisible = VScrollBarPresence == ScrollBarPresence.Visible;
				RemoveInvisibleItems();
				UpdateFindControlVisibility();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		void CheckNavigator() {
			if(View.GridControl == null || !View.GridControl.UseEmbeddedNavigator) return;
			((GridControlNavigator)View.GridControl.EmbeddedNavigator).UpdateLayout();
		}
		protected virtual void ValidateTopRowIndexByPixel() {
			View.ValidateTopRowIndexByPixel();
		}
		public virtual void CalcAfterVertScroll(Graphics g, Rectangle bounds, bool useCache) {
			if(IsNull) return;
			GInfo.AddGraphics(g);
			try {
				CalcAfterVertScrollCore(GInfo.Graphics, bounds, useCache);
			} finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected void FullRecalcScroll(Graphics g, Rectangle bounds) {
			SelectionInfo.LockClear();
			try {
				Calc(g, bounds);
			} finally {
				SelectionInfo.UnLockClear();
			}
		}
		protected virtual void RemoveAnimatedItems(GridDataRowInfo info) {
			if(info == null) return;
			for(int i = 0; i < info.Cells.Count; i++) {
				if(info.Cells[i].ViewInfo is IAnimatedItem ) XtraAnimator.RemoveObject(this, new GridCellId(View, info.Cells[i]));
			}
		}
		protected virtual void CalcAfterVertScrollCore(Graphics g, Rectangle bounds, bool useCache) {
			if(!IsReady || ViewTopRowIndex == 0 || View.ValidateTopRowIndexByPixel()) {
				FullRecalcScroll(g, bounds);
				return;
			}
			CellMerger.Clear();
			bool scrollVisible = VScrollBarPresence == ScrollBarPresence.Visible;
			this.cachedRows = useCache ? RowsInfo : null;
			RowsInfo = new GridRowInfoCollection();
			GridRowsLoadInfo prevInfo = this.rowsLoadInfo;
			this.rowsLoadInfo = new GridRowsLoadInfo(GInfo.Graphics, -1, CalcScrollRowHeight(), false, false);
			LoadRows(RowsLoadInfo);
			if(VScrollBarPresence == ScrollBarPresence.Hidden) {
				UpdateVScrollBarPresence(true);
				scrollVisible = VScrollBarPresence == ScrollBarPresence.Visible;
				if(this.cachedRows != null && this.cachedRows.VertScrollBarVisible != scrollVisible) this.cachedRows = null;
				RowsInfo.VertScrollBarVisible = VScrollBarPresence == ScrollBarPresence.Visible;
				if(VScrollBarPresence != ScrollBarPresence.Hidden) {
					this.rowsLoadInfo = prevInfo;
					FullRecalcScroll(g, bounds);
					return;
				}
			}
			scrollVisible = VScrollBarPresence == ScrollBarPresence.Visible;
			if(View.IsInListChangedEvent || (this.cachedRows != null && this.cachedRows.VertScrollBarVisible != scrollVisible)) this.cachedRows = null;
			CalcRowsDrawInfo();
			DoUnloadRows(RowsInfo, prevInfo);
			this.cachedRows = null;
			RowsInfo.VertScrollBarVisible = VScrollBarPresence == ScrollBarPresence.Visible;
			if(AllowUpdateDetails) {
				UpdateDetailRects();
				UpdateEditFormBounds();
			}
		}
		protected virtual void DoUnloadRows(GridRowInfoCollection visible, GridRowsLoadInfo prevRows) {
		}
		protected virtual void CalcGridInfo() {
			this.shouldInflateRowOnInvalidate = View.OptionsView.ShowIndicator && Painter.ElementsPainter.ShouldInflateRowOnInvalidate();
			CalcGroupDrawInfo();
			CalcColumnsDrawInfo();
			UpdateFixedRects();
			CalcRowsDrawInfo();
			CalcFooterDrawInfo();
			CalcFilterDrawInfo();
			if(AllowUpdateDetails) {
				UpdateDetailRects();
				UpdateEditFormBounds();
			}
			this.IsReady = true;
		}
		public int CalcRowHeight(Graphics graphics, int rowHandle, int level) {
			return CalcRowHeight(graphics, rowHandle, MinRowHeight, level, true, null);
		}
		protected virtual int GetGroupRowHeight(int rowHandle, int level) {
			if(GroupRowDefaultHeight == GroupRowMinHeight) return GroupRowMinHeight;
			if(level == -1) level = View.GetRowLevel(rowHandle);
			if(level == 0) return GroupRowDefaultHeight;
			return Math.Min(GroupRowMinHeight + 4, GroupRowDefaultHeight);
		}
		public virtual int CalcRowHeight(Graphics graphics, int rowHandle, int min, int level, bool useCache, GridColumnsInfo columns) {
			int result;
			Graphics g = GInfo.AddGraphics(graphics);
			try {
				if(View.IsGroupRow(rowHandle)) {
					result = GetGroupRowHeight(rowHandle, level);
				} else {
					result = ScaleVertical(View.RowHeight);
					if(IsRowAutoHeight) {
						result = CalcRowAutoHeight(g, columns, rowHandle, useCache, level);
					}
					result = Math.Max(min, result);
					result += CalcSkinRowPadding(g);
				}
				result = View.RaiseRowHeight(rowHandle, result);
				if(result < min) result = min;
				if(View.GetShowHorizontalLines()) {
					if(View.IsGroupRow(rowHandle)) {
						result += Painter.ElementsPainter.GroupRow.GetGridLineHeight(EmptyGroupRow);
					} else
						result += GetHorzLineHeight(rowHandle);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return result;
		}
		protected virtual Padding GetSkinnedCellPadding() {
			SkinElement info = GridSkins.GetSkin(View)[GridSkins.SkinGridCell];
			if(info == null) return padding;
			var margins = info.ContentMargins;
			return new Padding(margins.Left, margins.Top, margins.Right, margins.Bottom);
		}
		int CalcSkinRowPadding(Graphics graphics) {
			if(!IsSkinned) return 0;
			Graphics g = GInfo.AddGraphics(graphics);
			try {
				SkinElementInfo info = new SkinElementInfo(GridSkins.GetSkin(View)[GridSkins.SkinGridRow], Rectangle.Empty);
				Rectangle r = new Rectangle(0, 0, 0, 100);
				return ObjectPainter.CalcBoundsByClientRectangle(g, SkinElementPainter.Default, info, r).Height - r.Height;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual void CalcGroupRowHeight() {
			int minHeight = 0, defaultHeight = 0;
			GInfo.AddGraphics(null);
			try {
				int groupCount = View.GroupCount == 0 ? 1 : View.GroupCount;
				for(int n = 0; n < groupCount; n++) {
					GridGroupRowInfo row = new GridGroupRowInfo(this, -1, n); 
					row.Graphics = GInfo.Graphics;
					row.GroupText = View.GetGroupFormat();
					UpdateRowAppearance(row);
					minHeight = Math.Max(minHeight, Painter.ElementsPainter.GroupRow.CalcObjectMinBounds(row).Height);
					defaultHeight = Math.Max(defaultHeight, Painter.ElementsPainter.GroupRow.CalcObjectDefaultBounds(row).Height);
				}
			} finally {
				GInfo.ReleaseGraphics();
			}
			minHeight = Math.Max(minHeight, ScaleVertical(View.GroupRowHeight));
			defaultHeight = Math.Max(defaultHeight, minHeight);
			if(View.GroupRowHeight > 0) defaultHeight = minHeight;
			GroupRowMinHeight = minHeight;
			GroupRowDefaultHeight = defaultHeight;
		}
		protected virtual bool IsRowAutoHeight { get { return View.OptionsView.RowAutoHeight; } }
		public bool AllowUseRightHeaderKind = true;
		public virtual GridColumnsInfo CreateVisibleColumnsInfo(ref int rowWidth) {
			GridColumnsInfo colInfo = new GridColumnsInfo();
			rowWidth = 0;
			int vc = View.VisibleColumns.Count;
			for(int n = 0; n < vc; n++) {
				GridColumn col = View.VisibleColumns[n] as GridColumn;
				GridColumnInfoArgs info = CreateColumnInfo(col);
				info.HtmlContext = View;
				if(n == 0) info.HeaderPosition = HeaderPositionKind.Left;
				if(n == vc - 1 && AllowUseRightHeaderKind) info.HeaderPosition = HeaderPositionKind.Right;
				info.Bounds = new Rectangle(rowWidth, 0, col.VisibleWidth, 10);
				rowWidth += info.Bounds.Width;
				colInfo.Add(info); 
			}
			return colInfo;
		}
		protected virtual void UpdateColumnInfo(GridColumnInfoArgs res) {
			if(res.Column == View.CheckboxSelectorColumn && View.CheckboxSelectorColumn != null) {
				UpdateCheckboxSelectorColumnInfo(res);
			}
		}
		protected virtual void UpdateCheckboxSelectorColumnInfo(GridColumnInfoArgs res) {
			if(!View.IsShowCheckboxSelectorInHeader) return;
			res.InnerElements.Clear();
			var info = res.InnerElements.Add(CheckPainterHelper.GetPainter(View.ElementsLookAndFeel), new ColumnCheckboxSelectorInfoArgs(View));
			info.Alignment = StringAlignment.Center;
		}
		public virtual GridCellInfo CreateCellInfo(GridDataRowInfo ri, GridColumnInfoArgs columnInfo) {
			GridCellInfo cell = new GridCellInfo();
			cell.RowInfo = ri;
			cell.ColumnInfo = columnInfo;
			if(columnInfo.Type == GridColumnInfoType.Column) {
				cell.Appearance = PaintAppearance.Row;
				cell.Editor = View.GetRowCellRepositoryItem(cell.RowHandle, cell.Column);
			}
			return cell;
		}
		protected virtual int CalcRowAutoHeight(Graphics g, GridColumnsInfo colInfo, int rowHandle, bool useCache, int level) {
			if(View.IsGroupRow(rowHandle)) {
				return GetGroupRowHeight(rowHandle, level);
			}
			int result = Math.Max(MinRowHeight, ScaleVertical(View.RowHeight)), vc = View.VisibleColumns.Count;
			if(vc == 0) return result;
			if(useCache) {
				object realHeight = AutoRowHeightCache[rowHandle];
				if(realHeight != null) return (int)realHeight;
			}
			GridDataRowInfo ri = CreateRowInfo(rowHandle, 0) as GridDataRowInfo;
			int lastX = 0;
			if(colInfo == null) {
				colInfo = CreateVisibleColumnsInfo(ref lastX);
			}
			else {
				for(int n = 0; n < colInfo.Count; n++) {
					lastX = Math.Max(lastX, colInfo[n].Bounds.Right);
				}
			}
			GridColumnInfoArgs savedColumnInfo = null;
			Rectangle savedColumnBounds = Rectangle.Empty;
			if(colInfo.Count > 0 && View.GroupCount > 0) {
				savedColumnInfo = colInfo[0];
				Rectangle colBounds = savedColumnBounds = colInfo[0].Bounds;
				colBounds.Width = Math.Max(0, colBounds.Width - CalcRowLevelIndent(ri, View.GetRowLevel(rowHandle)));
				colInfo[0].Bounds = colBounds;
			}
			else {
				if(colInfo.LastColumnInfo != null) {
					savedColumnInfo = colInfo.LastColumnInfo;
					var lb = savedColumnBounds = colInfo.LastColumnInfo.Bounds;
					lb.Width ++;
					colInfo.LastColumnInfo.Bounds = lb;
				}
			}
			ri.Bounds = new Rectangle(0, 0, lastX, 10);
			if(IsRightToLeft) ri.IndentRect = new Rectangle(lastX, 0, 0, 0);
			UpdateRowAppearance(ri, true);
			CalcRowCellsDrawInfo(ri, colInfo);
			GInfo.AddGraphics(g);
			try {
				foreach(GridCellInfo ci in ri.Cells) {
					AppearanceObject cellStyle = ci.Appearance;
					BaseEditViewInfo editInfo = RequestCellEditViewInfo(ci);
					IHeightAdaptable ah = editInfo as IHeightAdaptable;
					if(ah != null) {
						result = Math.Max(ah.CalcHeight(GInfo.Cache, ci.CellValueRect.Width) + CellPadding.Horizontal, result);
					}
				}
			}
			finally {
				GInfo.ReleaseGraphics();
				if(savedColumnInfo != null) savedColumnInfo.Bounds = savedColumnBounds;
			}
			if(useCache)
				AutoRowHeightCache[rowHandle] = result;
			return result;
		}
		public virtual int CalcRowPreviewWidth(int rowHandle) {
			int maxWidth = ViewRects.ColumnTotalWidth;
			if(View.OptionsView.ColumnAutoWidth || HasFixedLeft) {
				if(View.OptionsView.ColumnAutoWidth) maxWidth = ViewRects.ColumnPanelWidth;
				else {
					maxWidth = Math.Min(ViewRects.ColumnTotalWidth, ViewRects.ColumnPanelWidth);
				}
			}
			return maxWidth - CalcRowLevelIndent(null, View.GetRowLevel(rowHandle));
		}
		public virtual int CalcRowPreviewHeight(int rowHandle) {
			if(!View.OptionsView.ShowPreview || View.IsGroupRow(rowHandle) || View.IsFilterRow(rowHandle) || View.IsNewItemRow(rowHandle)) return 0;
			AppearanceObject appearance = PaintAppearance.Preview;
			int res = (View.GetShowPreviewRowLines() ? 1 : 0); 
			int eventHeight = View.RaiseMeasurePreviewHeight(rowHandle);
			if(eventHeight != -1) return eventHeight == 0 ? 0 : res + eventHeight;
			bool autoCalc = (View.OptionsView.AutoCalcPreviewLineCount && View.PreviewLineCount == -1);
			int prLineCount = View.PreviewLineCount;
			if(!autoCalc && prLineCount < 1) prLineCount = 1;
			string fullText = (View.OptionsView.AutoCalcPreviewLineCount ? View.GetRowPreviewDisplayText(rowHandle) : "Wg");
			string s = (autoCalc ? fullText : "Wg");
			if(View.OptionsView.AutoCalcPreviewLineCount && s.Length == 0) return 0;
			Graphics g = GInfo.AddGraphics(null);
			try {
				SizeF size, sizeFull;
				if(s == "Wg") {
					size = sizeFull = new SizeF(100, appearance.FontHeight);
				} else {
					sizeFull = size = appearance.CalcTextSize(g, s, CalcRowPreviewWidth(rowHandle) - PreviewIndent - GridRowPreviewPainter.PreviewTextIndent * 2);
				}
				if(s != fullText) sizeFull = appearance.CalcTextSize(g, fullText, CalcRowPreviewWidth(rowHandle) - PreviewIndent - GridRowPreviewPainter.PreviewTextIndent * 2);
				size.Height *= 	(autoCalc ? 1 : prLineCount);
				if(!autoCalc && View.OptionsView.AutoCalcPreviewLineCount) {
					if(sizeFull.Height < size.Height) size = sizeFull;
				}
				int sizeHeight = Convert.ToInt32(size.Height);
				res += sizeHeight + GridRowPreviewPainter.PreviewTextVIndent * 2;
				if(sizeHeight == 0) res = 0;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res; 
		}
		public virtual Size CalcDetailRowSize(int rowHandle) {
			Size size = new Size(0, CalcDetailRowHeight(rowHandle));
			if(size.Height > 0) {
				size.Width = CalcDetailRowWidth(rowHandle);
			}
			return size;
		}
		protected virtual int CalcDetailRowWidth(int rowHandle) {
			return ViewRects.ColumnPanelWidth -
				(LevelIndent * (View.GetRowLevel(rowHandle) + 1));
		}
		protected virtual int CalcDetailRowHeight(int rowHandle) {
			int result = 0;
			if(View.IsMasterRow(rowHandle)) {
				if(View.GetMasterRowExpanded(rowHandle)) {
					BaseView gv = View.GetVisibleDetailView(rowHandle);
					if(gv != null) {
						Rectangle r = new Rectangle(0, 0, gv.ViewRect.Width, ScaleVertical(gv.DetailHeight));
						if(r.Width == 0 || View.IsUpdateViewRect)
							r.Width = CalcDetailRowWidth(rowHandle);
						result += gv.CalcRealViewHeight(r);
					}
					else
						result += ScaleVertical(View.DetailHeight);
					result += DetailVertIndent * 2;
					if(View.DetailTabHeaderLocation == DevExpress.XtraTab.TabHeaderLocation.Left ||
						View.DetailTabHeaderLocation == DevExpress.XtraTab.TabHeaderLocation.Right) {
						if(AllowTabControl) {
							result = Math.Max(result, DefaultMinVertTabDetailHeight);
						}
					}
				}
			}
			return result;
		}
		public static int DefaultMinVertTabDetailHeight = 150;
		protected internal virtual GridColumn FindFixedLeftColumn() {
			if(FixedLeftColumn == null || ColumnsInfo[FixedLeftColumn] != null) return FixedLeftColumn;
			int index = View.VisibleColumns.IndexOf(FixedLeftColumn);
			if(index < 1) return FixedLeftColumn;
			while(index > 0) {
				GridColumn column = View.VisibleColumns[--index];
				if(ColumnsInfo[column] != null) return column;
			}
			return FixedLeftColumn;
		}
		protected internal virtual GridColumn FindFixedRightColumn() {
			if(FixedRightColumn == null || ColumnsInfo[FixedRightColumn] != null) return FixedRightColumn;
			int index = View.VisibleColumns.IndexOf(FixedRightColumn);
			if(index < 1) return FixedRightColumn;
			while(index < View.VisibleColumns.Count) {
				GridColumn column = View.VisibleColumns[++index];
				if(ColumnsInfo[column] != null) return column;
			}
			return FixedRightColumn;
		}
		protected virtual void UpdateFixedRects() {
			ViewRects.FixedLeft = ViewRects.FixedRight = Rectangle.Empty;
			Rectangle r = Rectangle.Empty;
			GridColumnInfoArgs ci;
			if(FixedLeftColumn != null) {
				ci = ColumnsInfo[FindFixedLeftColumn()];
				if(ci != null) {
					r = ViewRects.Client;
					if(IsRightToLeft) {
						r.X = ci.Bounds.X; 
						r.Width = Math.Max(ViewRects.Client.Right - View.IndicatorWidth - r.X, 0);
					}
					else {
						r.X += ViewRects.IndicatorWidth;
						r.Width = Math.Max((ci.Bounds.Right + View.FixedLineWidth) - r.Left, 0);
					}
					ViewRects.FixedLeft = r;
				}
			}
			if(FixedRightColumn != null) {
				ci = ColumnsInfo[FindFixedRightColumn()];
				if(ci != null) {
					r = ViewRects.Client;
					if(IsRightToLeft) {
						r.Width = Math.Max((ci.Bounds.Right + View.FixedLineWidth) - r.Left, 0);
					}
					else {
						r.X = Math.Max(ci.Bounds.Left, ViewRects.FixedLeft.Right);
						r.Width = Math.Max(ViewRects.Client.Right - r.X, 0);
					}
					ViewRects.FixedRight = r;
				}
			}
		}
		public virtual Rectangle UpdateFixedRange(Rectangle rect, GridColumnInfoArgs ci) {
			if(ci != null && IsFixedLeftPaint(ci)) return rect;
			return UpdateFixedRange(rect, (ci == null || !IsFixedRightPaint(ci)));
		}
		public Rectangle UpdateFixedRange(Rectangle rect) { return UpdateFixedRange(rect, true); }
		protected virtual Rectangle UpdateFixedRange(Rectangle rect, bool checkRight) {
			Rectangle l = ViewRects.FixedLeft, r = ViewRects.FixedRight, res = rect;
			if(l.IsEmpty && r.IsEmpty) return rect;
			if(IsRightToLeft)
				return UpdateFixedRangeRightToLeft(rect, l, r, checkRight);
			if(!l.IsEmpty) {
				if(rect.X < l.Right) {
					res.X = l.Right;
					res.Width = Math.Max(rect.Right - res.X, 0);
				}
				if(res.Width == 0) return res;
			}
			rect = res;
			if(!r.IsEmpty && checkRight) {
				r.X -= View.FixedLineWidth;
				r.Width += View.FixedLineWidth;
				if(rect.Left > r.Left) {
					res.Width = 0;
					return res;
				}
				if(rect.Right > r.Left) {
					res.Width = Math.Max(r.Left - rect.X, 0);
				}
			}
			if(res.Right > ViewRects.ColumnPanelActual.Right) 
				res.Width = Math.Max(0, ViewRects.ColumnPanelActual.Right - res.Left);
			return res;
		}
		Rectangle UpdateFixedRangeRightToLeft(Rectangle rect, Rectangle l, Rectangle r, bool checkRight) {
			Rectangle res = rect;
			if(!l.IsEmpty) {
				l.X -= View.FixedLineWidth;
				l.Width += View.FixedLineWidth;
				if(rect.X > l.X) {
					rect.Width = 0;
					return rect;
				}
				if(rect.Right > l.X) {
					res.Width = Math.Max(l.X - res.X, 0);
				}
				if(res.Width == 0) return res;
			}
			rect = res;
			if(!r.IsEmpty && checkRight) {
				if(rect.Left <= r.Right) {
					res.X = r.Right;
					res.Width = Math.Max(rect.Right - res.X, 0);
					return res;
				}
			}
			return res;
		}
		public virtual void UpdateFixedColumnInfo() {
			this.fFixedLeftColumn = this.fFixedRightColumn = null;
			if(View.VisibleColumns.Count == 1) return;
			foreach(GridColumn column in View.VisibleColumns) {
				if(column.Fixed == FixedStyle.Left) {
					this.fFixedLeftColumn = column;
				}
				if(this.fFixedRightColumn == null && column.Fixed == FixedStyle.Right) {
					this.fFixedRightColumn = column;
				}
			}
			if(View.CheckboxSelectorColumn != null) {
				if(FixedLeftColumn == View.CheckboxSelectorColumn && !View.AllowFixedCheckboxSelectorColumn) {
					this.fFixedLeftColumn = null;
					View.CheckboxSelectorColumn.SetFixedCore(FixedStyle.None);
				}
				if(FixedLeftColumn != null) {
					View.CheckboxSelectorColumn.SetFixedCore(FixedStyle.Left);
				}
			}
			foreach(GridColumn column in View.VisibleColumns) {
				if(column.temporaryFixed) {
					column.temporaryFixed = false;
					column.SetFixedCore(FixedStyle.None);
				}
			}
			if(View.IsAlignGroupRowSummariesUnderColumnsAutoFix) {
				foreach(GridColumn column in View.VisibleColumns) {
					if(column.GroupIndex >= 0) {
						column.SetFixedCore(FixedStyle.Left);
						column.temporaryFixed = true;
					}
				}
			}
		}
		protected virtual bool ShouldProcessRow(GridDataRowInfo row) {
			if(row == null) return false;
			if(row.RowHandle != View.FocusedRowHandle && View.OptionsView.GetAnimationType() == GridAnimationType.AnimateFocusedItem) return false;
			if(row == null) return false;
			return true;
		}
		protected virtual bool ShouldProcessCell(GridCellInfo cell) {
			if(cell == null || !cell.IsDataCell) return false;
			IAnimatedItem item = cell.ViewInfo as IAnimatedItem;
			if(item == null || item.FramesCount < 2) return false;
			return true;
		}
		protected internal override void UpdateAnimatedItems() {
			RemoveInvisibleItems();
		}
		protected override BaseEditViewInfo HasItem(CellId id) {
			if(id == null) return null;
			int rowIndex, cellIndex;
			for(rowIndex = 0; rowIndex < RowsInfo.Count; rowIndex++) {
				GridDataRowInfo rowInfo = RowsInfo[rowIndex] as GridDataRowInfo;
				if(!ShouldProcessRow(rowInfo)) continue;
				for(cellIndex = 0; cellIndex < rowInfo.Cells.Count; cellIndex++) {
					if(!ShouldProcessCell(rowInfo.Cells[cellIndex])) continue;
					GridCellId cellId = new GridCellId(View, rowInfo.Cells[cellIndex]);
					if (id == cellId) return rowInfo.Cells[cellIndex].ViewInfo;
				}
			}
			return null;
		}
		protected override void AddAnimatedItems() {
			if(View.OptionsView.GetAnimationType() == GridAnimationType.NeverAnimate) return;
			int rowIndex, cellIndex;
			for(rowIndex = 0; rowIndex < RowsInfo.Count; rowIndex++) {
				GridDataRowInfo rowInfo = RowsInfo[rowIndex] as GridDataRowInfo;
				if(!ShouldProcessRow(rowInfo)) continue;
				for(cellIndex = 0; cellIndex < rowInfo.Cells.Count; cellIndex++) {
					if(!ShouldProcessCell(rowInfo.Cells[cellIndex])) continue;
					GridCellId id = new GridCellId(View, rowInfo.Cells[cellIndex]);
					if(ShouldAddItem(rowInfo.Cells[cellIndex].ViewInfo, id))
						AddAnimatedItem(id, rowInfo.Cells[cellIndex].ViewInfo);
				}
			}				
		}
		protected override bool ShouldStopAnimation(IAnimatedItem item) {
			if(View.OptionsView.GetAnimationType() == GridAnimationType.NeverAnimate) return true;
			BaseEditViewInfo vi = item as BaseEditViewInfo;
			if(vi == null) return false;
			GridCellInfo info = vi.Tag as GridCellInfo;
			if(info != null && ShouldRemoveAnimation(info.RowHandle, info.Column.FieldName)) {
				if(info.RowHandle != View.FocusedRowHandle && View.OptionsView.GetAnimationType() == GridAnimationType.AnimateFocusedItem) return true;
			}
			return false;
		}
		public virtual int FixedLineWidth {
			get { return View.FixedLineWidth; }
		}
		bool? defaultShowVerticalLines, defaultShowHorizontalLines, defaultShowPreviewRowLines;
		internal bool GetDefaultShowVerticalLines() {
			if(!defaultShowVerticalLines.HasValue) {
				if(View.PaintStyle == null) return true;
				defaultShowVerticalLines = ((GridPaintStyle)View.PaintStyle).GetDefaultShowVerticalLines(View);
			}
			return defaultShowVerticalLines.Value;
		}
		internal bool GetDefaultShowPreviewRowLines() {
			if(!defaultShowPreviewRowLines.HasValue) {
				if(View.PaintStyle == null) return true;
				defaultShowPreviewRowLines = ((GridPaintStyle)View.PaintStyle).GetDefaultShowPreviewRowLines(View);
			}
			return defaultShowPreviewRowLines.Value;
		}
		internal bool GetDefaultShowHorizontalLines() {
			if(!defaultShowHorizontalLines.HasValue) {
				if(View.PaintStyle == null) return true;
				defaultShowHorizontalLines = ((GridPaintStyle)View.PaintStyle).GetDefaultShowHorizontalLines(View);
			}
			return defaultShowHorizontalLines.Value;
		}
		internal void ResetPositionInfo() {
			PositionHelper.Reset();
		}
		protected internal UserControl EditForm { get; set; }
		protected internal int EditFormRequiredHeight {
			get { return editFormRequiredHeight;  }
			set { editFormRequiredHeight = value; }
		}
		public Rectangle EditFormBounds {
			get { return editFormBounds; }
			set { editFormBounds = value; }
		}
		public void AddAnimation(ImageLoadInfo info) {
			if(View.GridControl == null || !View.GridControl.IsHandleCreated) return;
			View.GridControl.BeginInvoke(new Action<ImageLoadInfo>(OnRunAnimation), info);
		}
		public void ForceItemRefresh(ImageLoadInfo imageInfo) {
			if(View.GridControl == null || !View.GridControl.IsHandleCreated) return;
			View.GridControl.Invoke(new Action<ImageLoadInfo>(RefreshItem), imageInfo);
		}
		Random rand = new Random();
		protected internal void OnRunAnimation(ImageLoadInfo info) {
			RemoveInvisibleAnimations();
			bool isRandomShow = View != null && View.OptionsImageLoad.RandomShow;
			int delay = isRandomShow ? rand.Next() % 300 : 0;
			if(info.RenderImageInfo == null) return;
			int ms = View.OptionsImageLoad.AnimationType == ImageContentAnimationType.None ? 0 : 1000;
			XtraAnimator.Current.AddAnimation(new GridViewImageShowingAnimationInfo(this, info.InfoId, info.RenderImageInfo, ms + delay, delay));
		}
		protected internal virtual void RefreshItem(ImageLoadInfo info) {
			RefreshItemCore(info);
		}
		protected internal void RefreshItemCore(ImageLoadInfo info) {
			View.DataController.RefreshRow(info.RowHandle);
		}
		public ThumbnailImageEventArgs RaiseGetThumbnailImage(ThumbnailImageEventArgs e) {
			if(View == null) return null;
			return View.RaiseGetThumbnailImage(e);
		}
		public Image RaiseGetLoadingImage(GetLoadingImageEventArgs e) {
			if(View == null) return null;
			return View.RaiseGetLoadingImage(e);
		}
	}
	public class GridRowsLoadInfo {
		Graphics graphics;
		GridRowCollection resultRows;
		int topVisibleRowIndex, maxRowsHeight, resultRowCount, resultRowsHeight;
		int visibleRowCount = 0;
		bool useMinRowHeight, calcOnly;
		bool resultAllRowsFit, hasDetails;
		bool isPixelIndex = false;
		public GridRowsLoadInfo(Graphics graphics, int topVisibleRowIndex) : this(graphics, topVisibleRowIndex, -1, false, true) { }
		public GridRowsLoadInfo(Graphics graphics, int topVisibleRowIndex, int maxRowsHeight, bool calcOnly, bool useMinRowHeight) {
			this.graphics = graphics;
			this.topVisibleRowIndex = topVisibleRowIndex;
			this.maxRowsHeight = maxRowsHeight;
			this.useMinRowHeight = useMinRowHeight;
			this.calcOnly = calcOnly;
			this.resultRows = null;
			this.hasDetails = false;
			ClearResults();
		}
		public int VisibleRowCount { get { return visibleRowCount; } set { visibleRowCount = value; } } 
		public bool IsPixelIndex { get { return isPixelIndex; } set { isPixelIndex = value; } }
		public virtual void ClearResults() {
			this.resultRowCount = this.resultRowsHeight = 0;
			this.resultAllRowsFit = false;
			this.hasDetails = false;
		}
		public GridRowCollection ResultRows { get { return resultRows; } set { resultRows = value; } }
		public int ResultRowCount { get { return resultRowCount; } set { resultRowCount = value; } }
		public int ResultRowsHeight { get { return resultRowsHeight; } set { resultRowsHeight = value; } }
		public bool ResultAllRowsFit { get { return resultAllRowsFit; } set { resultAllRowsFit = value; } }
		public int MaxRowsHeight { get { return maxRowsHeight; } }
		public bool UseMinRowHeight { get { return useMinRowHeight; } }
		public bool CalcOnly { get { return calcOnly; } }
		public Graphics Graphics { get { return graphics; } }
		public int TopVisibleRowIndex { get { return topVisibleRowIndex; } }
		public bool HasDetails { get { return hasDetails; } set { hasDetails = value; } }
	}
	public class GridViewImageShowingAnimationInfo : ImageShowingAnimationInfo {
		public GridViewImageShowingAnimationInfo(ISupportXtraAnimation anim, object animationId, RenderImageViewInfo imageInfo, int ms, int delay)
			: base(anim, animationId, imageInfo, ms, delay) {
			this.imageUpdated = false;
		}
		bool imageUpdated;
		protected override void FrameStepCore() {
			float k = ((float)(CurrentFrame - FirstAnimationFrame)) / (FrameCount - FirstAnimationFrame);
			if(k >= 0) {
				Item.LoadInfo.IsLoaded = true;
				if(!imageUpdated) {
					imageUpdated = true;
					if(!(AnimatedObject is GridViewInfo)) return;
					GridViewInfo vi = (GridViewInfo)AnimatedObject;
					vi.RefreshItem(Item.LoadInfo);
				}
			}
			base.FrameStepCore();
		}
		protected override void Invalidate() {
			if(!(AnimatedObject is GridViewInfo)) return;
			GridViewInfo vi = (GridViewInfo)AnimatedObject;
			vi.RefreshItemCore(Item.LoadInfo);
		}
		protected override RectangleF CalcPushAnimationRectangle(RectangleF rect, float spline) {
			rect.Y = EndBounds.Y;
			rect.X = (-ImageShowingAnimationHelper.EasyOutAnimationOdds - EndBounds.Width) * spline + EndBounds.Width + ImageShowingAnimationHelper.EasyOutAnimationOdds;
			rect.Width = EndBounds.Width;
			rect.Height = EndBounds.Height;
			return rect;
		}
	}
	public class GridViewImageLoadInfo : ImageLoadInfo {
		string fieldName;
		public GridViewImageLoadInfo(int datasourceindex, int rowIndex, string fieldName, ImageContentAnimationType animationType, ImageLayoutMode mode, Size maxSize, Size desiredSize)
			: base(datasourceindex, rowIndex, animationType, mode, maxSize, desiredSize) {
			this.fieldName = fieldName;
		}
		public string FieldName { get { return fieldName; } }
		public override ImageInfoKeyBase CreateImageInfoKey() {
			return new GridViewImageInfoKey() { RowHandle = RowHandle, FieldName = FieldName };
		}
	}
	public class GridViewImageInfoKey : ImageInfoKeyBase {
		public string FieldName { get; set; }
		public override bool Equals(object obj) {
			GridViewImageInfoKey key = (GridViewImageInfoKey)obj;
			return key.FieldName == FieldName && key.RowHandle == RowHandle;
		}
		public override int GetHashCode() {
			return FieldName.GetHashCode() ^ RowHandle;
		}
	}
	public class GridViewAsyncImageLoader : AsyncImageLoader {
		public GridViewAsyncImageLoader(IAsyncImageLoaderClient viewInfo) : base(viewInfo) { }
		protected override Image RaiseGetLoadingImage(ImageLoadInfo info) {
			GridViewImageLoadInfo gvImageInfo = (GridViewImageLoadInfo)info;
			return ViewInfo.RaiseGetLoadingImage(new GetGridViewLoadingImageEventArgs(gvImageInfo.FieldName, gvImageInfo.DataSourceIndex));
		}
		protected override ThumbnailImageEventArgs RaiseGetThumbnailImage(ImageLoadInfo info) {
			GridViewImageLoadInfo gvImageInfo = (GridViewImageLoadInfo)info;
			return ViewInfo.RaiseGetThumbnailImage(new GridViewThumbnailImageEventArgs(gvImageInfo.FieldName, gvImageInfo.DataSourceIndex, this, gvImageInfo));
		}
		protected override Image RaiseGetLoadingImage(string fieldName, int dataSourceIndex) {
			return ViewInfo.RaiseGetLoadingImage(new GetGridViewLoadingImageEventArgs(fieldName, dataSourceIndex));
		}
	}
}
