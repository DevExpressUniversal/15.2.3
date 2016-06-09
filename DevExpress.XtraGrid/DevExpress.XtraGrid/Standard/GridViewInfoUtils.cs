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
using System.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Accessibility;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Dragging;
using DevExpress.XtraGrid.Views.Base;
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.XtraGrid.Views.Grid.ViewInfo {
	public enum ScrollBarPresence { Unknown, Calculable, Hidden, Visible }
	#region GridViewRects
	public class GridViewRects {
		GridViewInfo viewInfo;
		Rectangle bounds, client, groupPanel, columnPanel, rows, footer, emptyRows, scroll, fixedLeft, fixedRight, viewCaption;
		int vScrollLocation, hScrollLocation;
		public int ColumnTotalWidth, IndicatorWidth, RowsTotalHeight, MinIndicatorWidth;
		public int DataRectRight;
		public GridViewRects(GridViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			Clear();
		}
		public GridViewInfo ViewInfo { get { return viewInfo; } }
		public GridView View { get { return ViewInfo.View; } }
		public virtual void Clear() {
			viewCaption = fixedLeft = fixedRight = scroll = bounds = emptyRows = groupPanel = columnPanel = rows = footer =  client = Rectangle.Empty;
			this.DataRectRight = 0;
			this.ColumnTotalWidth = 0;
			this.IndicatorWidth = 0;
			this.MinIndicatorWidth = 8;
			this.RowsTotalHeight = 0;
		}
		public int ColumnPanelLeft { 
			get {
				if(View.IsRightToLeft) return ColumnPanel.Left;
				return ColumnPanel.Left + IndicatorWidth; 
			} 
		}
		public int ColumnPanelRight {
			get {
				if(View.IsRightToLeft) return ColumnPanel.Right - IndicatorWidth;
				return ColumnPanel.Right;
			}
		}
		public int ColumnPanelWidth { 
			get { 
				if(ColumnPanel.Width > 0) return ColumnPanel.Width - IndicatorWidth; 
				int res = 0;
				foreach(GridColumn col in View.VisibleColumns) {
					res += col.VisibleWidth;
				}
				return res - IndicatorWidth;
			}
		}
		public Rectangle FixedLeft { get { return fixedLeft; } set { fixedLeft = value; } }
		public Rectangle FixedRight { get { return fixedRight; } set { fixedRight = value; } }
		public int VScrollLocation { get { return vScrollLocation; } set { vScrollLocation = value; } }
		public int HScrollLocation { get { return hScrollLocation; } set { hScrollLocation = value; } }
		public Rectangle Scroll { get { return scroll; } set { scroll = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle Client { get { return client; } set { client = value; } }
		public Rectangle ViewCaption { get { return viewCaption; } set { viewCaption = value; } }
		public Rectangle GroupPanel { get { return groupPanel; } set { groupPanel = value; } }
		public Rectangle ColumnPanel { get { return columnPanel; } set { columnPanel = value; } }
		public Rectangle ColumnPanelActual {
			get {
				Rectangle res = ColumnPanel;
				if(ViewInfo.IsRightToLeft) {
					res.X = Scroll.Left;
					res.Width = ColumnPanel.Right - res.X;
				}
				else {
					res.Width = Scroll.Right - res.X;
				}
				return res;
			}
		}
		public Rectangle Rows { get { return rows; } set { rows = value; } }
		public Rectangle Footer { get { return footer; } set { footer = value; } }
		public Rectangle EmptyRows { get { return emptyRows; } set { emptyRows = value; } }
		public virtual void AssignTo(GridViewRects vr) {
			vr.ViewCaption = this.ViewCaption;
			vr.FixedLeft = this.FixedLeft;
			vr.FixedRight = this.FixedRight;
			vr.Client = this.Client;
			vr.ColumnPanel = this.ColumnPanel;
			vr.ColumnTotalWidth = this.ColumnTotalWidth;
			vr.EmptyRows = this.EmptyRows;
			vr.Footer = this.Footer;
			vr.GroupPanel = this.GroupPanel;
			vr.IndicatorWidth = this.IndicatorWidth;
			vr.Rows = this.Rows;
			vr.RowsTotalHeight = this.RowsTotalHeight;
			vr.Bounds = this.Bounds;
		}
	}
	#endregion
	public class GroupPanelInfo {
		GridViewInfo viewInfo;
		GroupPanelRowCollection rows;
		public GroupPanelInfo(GridViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			this.rows = new GroupPanelRowCollection();
		}
		public GridViewInfo ViewInfo { get { return viewInfo; } }
		public GroupPanelRowCollection Rows { get { return rows; } }
		public virtual void Clear() {
			Rows.Clear();
		}
	}
	public class GroupPanelRow {
		bool lineStyle;
		Rectangle bounds;
		GridColumnInfoArgs captionInfo;
		string rowCaption;
		GridViewInfo viewInfo;
		public GridColumnsInfo ColumnsInfo;
		public GroupPanelRow(GridViewInfo viewInfo) {
			this.lineStyle = false;
			this.viewInfo = viewInfo;
			this.ColumnsInfo = new GridColumnsInfo();
		}
		public virtual bool LineStyle { get { return lineStyle; } set { lineStyle = value; } }
		public virtual void Clear() {
			this.rowCaption = "";
			this.bounds = Rectangle.Empty;
			this.captionInfo = null;
			ColumnsInfo.Clear();
		}
		public string RowCaption { get { return rowCaption; } set { rowCaption = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public GridColumnInfoArgs CaptionInfo { get { return captionInfo; } set { captionInfo = value; } }
		public GridViewInfo ViewInfo { get { return viewInfo; } }
		public DevExpress.XtraGrid.Views.Grid.Drawing.GridPainter Painter { get { return ViewInfo.Painter; } }
		public virtual GridColumnInfoArgs ColumnByPoint(Point pt) {
			foreach(GridColumnInfoArgs info in ColumnsInfo) {
				if(info.Bounds.Contains(pt)) return info;
			}
			return null;
		}
	}
	public class GroupPanelRowCollection : CollectionBase {
		public GroupPanelRow this[int index] { get { return List[index] as GroupPanelRow; } }
		public virtual GroupPanelRow Add(GridViewInfo viewInfo) {
			GroupPanelRow row = new GroupPanelRow(viewInfo);
			Add(row);
			return row;
		}
		public virtual int Add(GroupPanelRow row) {
			return List.Add(row);
		}
		public GroupPanelRow RowByPoint(Point pt) {
			foreach(GroupPanelRow row in this) {
				if(row.Bounds.Contains(pt)) return row;
			}
			return null;
		}
		public GridColumnInfoArgs GetColumnInfo(GridColumn column) {
			foreach(GroupPanelRow row in this) {
				GridColumnInfoArgs ci = row.ColumnsInfo[column];
				if(ci != null) return ci;
			}
			return null;
		}
		public virtual GroupPanelRow RowByView(GridView view) {
			foreach(GroupPanelRow row in this) {
				if(row.ViewInfo.View == view) return row;
			}
			return null;
		}
	}
	public class GridMergedCellInfo : GridCellInfo {
		GridCellInfoCollection mergedCells;
		public GridMergedCellInfo(GridColumnInfoArgs column) : base(column, null, Rectangle.Empty) {
			mergedCells = new GridCellInfoCollection();
		}
		public GridCellInfoCollection MergedCells { get { return mergedCells; } }
		public void AddMergedCell(GridCellInfo cell) {
			cell.MergedCell = this;
			cell.RowInfo.HasMergedCells = true;
			MergedCells.Add(cell);
		}
		internal void Clear() {
			for(int n = 0; n < MergedCells.Count; n ++) { 
				GridCellInfo cell = MergedCells[n];
				if(cell == null) continue;
				cell.MergedCell = null;
				cell.RowInfo.HasMergedCells = false;
			}
			MergedCells.ClearCells();
		}
		public void Update() {
			Bounds = GetBounds();
			CellValue = FirstCell == null ? null : FirstCell.CellValue;
		}
		public GridCellInfo FirstCell { get { return MergedCells.Count > 0 ? MergedCells[0] : null; } }
		protected Rectangle GetBounds() {
			Rectangle res = Rectangle.Empty;
			if(FirstCell == null) return res;
			res = FirstCell.Bounds;
			var scrollable = RowInfo.ViewInfo.GetScrollableBounds(false);
			if(res.Top < scrollable.Top) {
				res.Height = res.Bottom - scrollable.Y;
				res.Y = scrollable.Y;
			}
			if(MergedCells.Count > 1) {
				GridCellInfo cell = MergedCells[MergedCells.Count - 1];
				res.Height = cell.Bounds.Bottom - res.Top;
			}
			if(RowInfo.ViewInfo.View.GetShowHorizontalLines()) res.Height ++;
			if(!RowInfo.ViewInfo.IsFixedLeft(this.ColumnInfo) && RowInfo.ViewInfo.View.GetShowVerticalLines()) {
				if(!RowInfo.ViewInfo.IsRightToLeft) res.Width++;
				else {
					res.X--;
					res.Width++;
				}
			}
			return res;
		}
		public override GridDataRowInfo RowInfo { 
			get {
				if(MergedCells.Count == 0) return null;
				return MergedCells[0].RowInfo;
			}
		}
	}
	public class GridMergedCellInfoCollection : GridCellInfoCollection {
		public override void ClearCells() {
			for(int n = Count - 1; n >= 0; n--) {
				this[n].Clear();
			}
			Clear();
		}
		public new GridMergedCellInfo this[int index] { get { return base[index] as GridMergedCellInfo; } }
	}
	public class GridCellInfo {
		Rectangle bounds;
		GridDataRowInfo rowInfo;
		GridRowCellState state;
		GridMergedCellInfo mergedCell = null;
		public Rectangle CellValueRect, CellButtonRect;
		public GridColumnInfoArgs ColumnInfo;
		public object CellValue;
		public DevExpress.XtraEditors.Repository.RepositoryItem Editor;
		public DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo ViewInfo;
		public bool CustomDrawHandled = false;
		AppearanceObject appearance = null;
		[ThreadStatic]
		static GridCellInfo emptyCellInfo;
		public static GridCellInfo EmptyCellInfo  {
			get {
				if(emptyCellInfo == null) {
					emptyCellInfo = new GridCellInfo();
					emptyCellInfo.ColumnInfo = new GridColumnInfoArgs(null);
					emptyCellInfo.ColumnInfo.Type = GridColumnInfoType.EmptyColumn;
				}
				return emptyCellInfo;
			}
		}
		public GridCellInfo() : this(null, null, Rectangle.Empty) {}
		public GridCellInfo(GridColumnInfoArgs column, GridDataRowInfo rowInfo, Rectangle rect) {
			this.state = GridRowCellState.Dirty;
			this.rowInfo = rowInfo;
			this.Editor = null;
			ViewInfo = null;
			CellValue = null;
			this.bounds = CellValueRect = rect;
			CellButtonRect = Rectangle.Empty;
			ColumnInfo = column;
		}
		public bool IsMerged { get { return MergedCell != null; } }
		public GridMergedCellInfo MergedCell { get { return mergedCell; } set { mergedCell = value; } }
			public GridRowCellState State { get { return state; } set { state = value; } }
		public AppearanceObject Appearance {
			get { return appearance; }
			set { appearance = value; }
		}
		public bool IsDataCell { get { return Column != null; } }
		public GridColumn Column { get { return ColumnInfo.Column; } }
		public virtual void OffsetContent(int x, int y) {
			bounds.Offset(x, y);
			CellValueRect.Offset(x, y);
			if(!CellButtonRect.IsEmpty) CellButtonRect.Offset(x, y);
		}
		public virtual GridDataRowInfo RowInfo { 
			get { return rowInfo; } 
			set { 
				rowInfo = value; 
			} 
		}
		public virtual int RowHandle { get { return RowInfo.RowHandle; } }
		public virtual Rectangle Bounds { 
			get { return bounds; }
			set { bounds = value; }
		}
	}
	#region GridRowInfo
	public class GridRowInfo : ObjectInfoArgs {
		bool allowDrawBackground, isDataDirty;
		Rectangle totalBounds, dataBounds;
		int rowHandle, visibleIndex, level;
		GridRowCellState rowState;
		GridViewInfo viewInfo;
		object rowKey;
		public int RowLineHeight = 0;
		public IndentInfoCollection Lines;
		public IndentInfoCollection Indents;
		public GridRowFooterCollection RowFooters;
		public AppearanceObject Appearance, BaseAppearance;
		public bool BaseAppearanceHighPriority, DrawMoreIcons, RightToLeft;
		bool forcedRow;
		bool forcedRowLight = false;
		public GridRowInfo(GridViewInfo viewInfo, int rowHandle, int level) {
			this.DrawMoreIcons = false;
			this.BaseAppearanceHighPriority = false;
			this.isDataDirty = false;
			this.rowKey = null;
			this.viewInfo = viewInfo;
			this.allowDrawBackground = true;
			this.rowHandle = rowHandle;
			this.visibleIndex = 0;
			this.IndicatorRect = this.RowSeparatorBounds = this.IndentRect = this.dataBounds = this.totalBounds = Rectangle.Empty;
			this.rowState = GridRowCellState.Dirty;
			this.level = level;
			this.BaseAppearance = this.Appearance = null;
			Lines = new IndentInfoCollection();
			Indents = new IndentInfoCollection();
			RowFooters = new GridRowFooterCollection();
		}
		public bool ForcedRow {
			get { return forcedRow; }
			set {
				if(ForcedRow == value) return;
				forcedRow = value;
			}
		}
		public bool ForcedRowLight {
			get { return forcedRowLight; }
			set {
				if(ForcedRowLight == value) return;
				forcedRowLight = value;
			}
		}
		public bool IsDataDirty { get { return isDataDirty; } }
		public void SetDataDirty() { SetDataDirty(true); }
		internal void SetDataDirty(bool val) {
			this.isDataDirty = val;
		}
		public object RowKey { get { return rowKey; } set { rowKey = value; } }
		public int Level { get { return level; } }
		public GridViewInfo ViewInfo { get { return viewInfo; } }
		public GridView View { get { return ViewInfo == null ? null : ViewInfo.View; } }
		public bool AllowDrawBackground { get { return allowDrawBackground; } set { allowDrawBackground = value; } }
		public GridRowCellState RowState { get { return rowState; } set { rowState = value; } }
		public int RowHandle { get { return rowHandle; } set { rowHandle = value; } }
		public int VisibleIndex { get { return visibleIndex; } set { visibleIndex = value; } }
		public virtual bool IsGroupRow { get { return false; } }
		public virtual bool IsGroupRowExpanded { get { return false; } }
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			if(!this.totalBounds.IsEmpty) this.totalBounds.Offset(x, y);
			if(!this.dataBounds.IsEmpty) this.dataBounds.Offset(x, y);
			if(!RowSeparatorBounds.IsEmpty) RowSeparatorBounds.Offset(x, y); 
			if(!IndentRect.IsEmpty) IndentRect.Offset(x, y);
			if(!IndicatorRect.IsEmpty) IndicatorRect.Offset(x, y);
			Lines.OffsetContent(x, y);
			RowFooters.OffsetContent(x, y);
			Indents.OffsetContent(x, y);
		}
		public int Indent = 0;
		public Rectangle RowSeparatorBounds, IndentRect, IndicatorRect;
		public Rectangle TotalBounds { get { return totalBounds; } set { totalBounds = value; } }
		public Rectangle DataBounds { get { return dataBounds; } set { dataBounds = value; } }
		public void AddLineInfo(object indentOwner, int x, int y, int width, int height, AppearanceObject appearance) {
			AddLineInfo(indentOwner, new Rectangle(x, y, width, height), appearance);
		}
		public void AddLineInfo(object indentOwner, Rectangle r, AppearanceObject appearance) {
			Lines.Add(new IndentInfo(indentOwner, r, appearance));
		}
		public void AddLineInfo(object indentOwner, Rectangle r, AppearanceObject appearance, bool isGroupStyle) {
			Lines.Add(new IndentInfo(indentOwner, r, appearance, isGroupStyle));
		}
	}
	public class GridGroupSummaryItemInfo {
		public GridGroupSummaryItem SummaryItem { get; set;}
		public object Value { get; set; }
	}
	public class GridGroupRowInfo : GridRowInfo {
		string groupText, groupValueText;
		EditorGroupRowArgs editorInfo;
		object editValue;
		GroupRowCheckboxSelectorInfoArgs selectorInfo;
		CheckObjectPainter selectorPainter;
		Dictionary<GridColumn, GridGroupSummaryItemInfo> summaryItems;
		public GridGroupRowInfo(GridViewInfo viewInfo, int rowHandle, int level) : base(viewInfo, rowHandle, level) { 
			this.groupValueText = this.groupText = string.Empty;
			this.editValue = null;
			this.GroupExpanded = false;
			this.selectorPainter = CheckPainterHelper.GetPainter(viewInfo.View.ElementsLookAndFeel);
			this.editorInfo = null;
		}
		public ObjectPainter SelectorPainter { 
			get {
				if(selectorPainter == null) {
					this.selectorPainter = CheckPainterHelper.GetPainter(ViewInfo.View.ElementsLookAndFeel);
				}
				return selectorPainter; 
			} 
		}
		protected Dictionary<GridColumn, GridGroupSummaryItemInfo> ColumnSummaryItems { get { return summaryItems; } }
		public bool HasColumnSummaryItems { get { return ColumnSummaryItems != null && ColumnSummaryItems.Count > 0; } }
		public void AddColumnSummaryItem(GridColumn column, GridGroupSummaryItem item, object value) {
			if(summaryItems == null) summaryItems = new Dictionary<GridColumn, GridGroupSummaryItemInfo>();
			summaryItems[column] = new GridGroupSummaryItemInfo() { SummaryItem = item, Value = value };
		}
		public GridGroupSummaryItemInfo GetColumnSummaryItem(GridColumn column) {
			GridGroupSummaryItemInfo res;
			if(ColumnSummaryItems.TryGetValue(column, out res)) return res;
			return null;
		}
		public GroupRowCheckboxSelectorInfoArgs SelectorInfo {
			get { return selectorInfo; }
			set { selectorInfo = value; }
		}
		public GridColumn Column { get { return Level < ViewInfo.View.GroupCount ? ViewInfo.View.SortInfo[Level].Column : null; } }
		public EditorGroupRowArgs EditorInfo { get { return editorInfo; } }
		public void CreateEditorInfo(Rectangle bounds, Color foreColor) {
			RepositoryItem item = Column == null ? null : ViewInfo.View.GetRowCellRepositoryItem(RowHandle, Column);
			this.editorInfo = new EditorGroupRowArgs(bounds, Appearance, item, EditValue, GroupText);
			this.editorInfo.HtmlContext = ViewInfo.View;
			this.editorInfo.AllowHtmlDraw = ViewInfo.View.OptionsView.AllowHtmlDrawGroups;
			this.editorInfo.GroupValueText = GroupValueText;
			this.editorInfo.ForeColor = foreColor;
			this.editorInfo.MatchedText = "";
			this.editorInfo.UseHighlightSearchAppearance = true;
			this.editorInfo.RightToLeft = RightToLeft;
			if(ViewInfo.View.IsAllowHighlightFind(Column)) {
				this.editorInfo.MatchedText = ViewInfo.View.GetFindMatchedText(Column, GroupText);
			}
		}
		public ObjectPainter CreatePainter() { 
			if(EditorInfo == null) return new ObjectPainter();
			return EditorInfo.Properties == null ? new BaseEditorGroupRowPainter() : EditorInfo.Properties.CreateGroupPainter();
		}		
		public object EditValue { get { return editValue; } set { editValue = value; } }
		public AppearanceObject AppearanceGroupButton { get { return ViewInfo.PaintAppearance.GroupButton; } }
		public Rectangle ButtonBounds = Rectangle.Empty;
		public override bool IsGroupRow { get { return true; } }
		public override bool IsGroupRowExpanded { get { return GroupExpanded; } }
		public string GroupText { get { return groupText; } set { groupText = value; } }
		public string GroupValueText { get { return groupValueText; } set { groupValueText = value; } }
		public bool GroupExpanded;
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			if(SelectorInfo != null) SelectorInfo.OffsetContent(x, y);
			if(!ButtonBounds.IsEmpty) ButtonBounds.Offset(x, y);
		}
		public void SetSelectorStateDirty() {
			if(SelectorInfo == null) return;
			SelectorInfo.IsDirty = true;
		}
		public void UpdateSelectorState() {
			if(SelectorInfo == null) return;
			SelectorInfo.IsDirty = false;
			int state = View.GetGroupSelectionState(RowHandle);
			if(state == 2) SelectorInfo.CheckState = CheckState.Checked;
			if(state == 1) SelectorInfo.CheckState = CheckState.Indeterminate;
			if(state == 0) SelectorInfo.CheckState = CheckState.Unchecked;
		}
	}
	public class GridNewItemRow : GridDataRowInfo {
		public GridNewItemRow(GridViewInfo viewInfo, int rowHandle) : base(viewInfo, rowHandle, 0) { }
		public override bool IsNewItemRow { get { return true; } }
	}
	public class GridDataRowInfo : GridRowInfo, IRowConditionFormatProvider {
		public bool EditFormRow = false;
		public GridCellInfoCollection Cells;
		public Rectangle  
			PreviewBounds,
			DetailBounds,
			DetailIndentBounds,
			DetailIndicatorBounds;
		public bool IsMasterRow, MasterRowExpanded, HasMergedCells = false;
		public bool IsMasterRowEmpty;
		public int PreviewIndent; 
		public string PreviewText, ErrorText;
		internal Image ErrorIcon;
		ConditionInfo conditionInfo;
		RowFormatRuleInfo formatInfo;
		AppearanceObject appearancePreview;
		bool isLoading = false;
		public GridDataRowInfo(GridViewInfo viewInfo, int rowHandle, int level) : base(viewInfo, rowHandle, level) { 
			this.PreviewText = string.Empty;
			this.conditionInfo = new ConditionInfo();
			this.formatInfo = new RowFormatRuleInfo(viewInfo.View);
			Cells = new GridCellInfoCollection();
			DetailIndicatorBounds = PreviewBounds = DetailIndentBounds = DetailBounds = Rectangle.Empty;
			MasterRowExpanded = false;
			IsMasterRow = false;
			IsMasterRowEmpty = false;
			PreviewIndent = 0;
			ErrorText = null;
		}
		public AppearanceObject AppearancePreview { 
			get { 
				if(appearancePreview == null) appearancePreview = (AppearanceObject)ViewInfo.PaintAppearance.Preview.Clone();
				return appearancePreview;
			} 
			set {
				appearancePreview = value;
			}
		}
		internal void SetIsLoading(bool loading) { this.isLoading = loading; }
		public virtual bool IsLoading { get { return isLoading && !IsNewItemRow && !IsSpecialRow; } }
		public virtual bool IsNewItemRow { get { return false; } }
		public virtual bool IsSpecialRow { get { return IsNewItemRow || ViewInfo.View.IsFilterRow(RowHandle); } }
		public AppearanceObjectEx RowFormatAppearance {
			get {
				var res = FormatInfo.RowAppearance;
				if(res != null) return res;
				return ConditionInfo.RowConditionAppearance;
			}
		}
		public ConditionInfo ConditionInfo { get { return conditionInfo; } }
		public RowFormatRuleInfo FormatInfo { get { return formatInfo; } }
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			if(!PreviewBounds.IsEmpty) PreviewBounds.Offset(x, y);
			if(!DetailBounds.IsEmpty) DetailBounds.Offset(x, y);
			if(!DetailIndentBounds.IsEmpty) DetailIndentBounds.Offset(x, y);
			if(!DetailIndicatorBounds.IsEmpty) DetailIndicatorBounds.Offset(x, y);
			Cells.OffsetContent(x, y);
		}
		protected internal AppearanceObjectEx GetConditionAppearance(GridColumn column) {
			var res = FormatInfo.GetCellAppearance(column);
			if(res != null) return res;
			return ConditionInfo.GetCellAppearance(column);
		}
	}
	#endregion
	public class GridRowInfoCollection : List<GridRowInfo>  {
		public bool VertScrollBarVisible = false;
		Hashtable hash = new Hashtable();
		[Obsolete]
		public new void Clear() {
			base.Clear();
		}
		[Obsolete]
		public new void Add(GridRowInfo info) {
			AddRow(info);
		}
		public virtual void AddRow(GridRowInfo info) { 
			base.Add(info); 
			Hash[info.RowHandle] = info;
		}
		public new GridRowInfo this[int index] { 
			get {
				if(index < 0 || index >= Count) return null;
				return base[index] as GridRowInfo; 
			} 
		}
		public GridRowInfo FindRowByKey(object rowKey) {
			for(int n = Count - 1; n >= 0; n --) {
				GridRowInfo res = this[n];
				if(res.RowKey != null && Object.Equals(res.RowKey, rowKey)) return res;
			}
			return null;
		}
		public void SetGroupSelectorDirty() {
			foreach(GridRowInfo row in this) {
				GridGroupRowInfo group = row as GridGroupRowInfo;
				if(group != null) group.SetSelectorStateDirty();
			}
		}
		public GridRowInfoCollection Clone() {
			GridRowInfoCollection res = new GridRowInfoCollection();
			foreach(GridRowInfo row in this) res.AddRow(row);
			return res;
		}
		public GridRowInfo FindRow(int rowHandle) {
			for(int n = Count - 1; n >= 0; n --) {
				GridRowInfo res = this[n];
				if(res.RowHandle == rowHandle) return res;
			}
			return null;
		}
		public GridRowInfo FindRowByVisibleIndex(int visibleIndex) {
			for(int n = Count - 1; n >= 0; n --) {
				GridRowInfo res = this[n];
				if(res.VisibleIndex == visibleIndex) return res;
			}
			return null;
		}
		public int GetFirstForcedRowIndexLight() {
			for(int n = 0; n < Count; n++) {
				if(this[n].ForcedRowLight) return n;
			}
			return -1;
		}
		public int GetLastForcedRowIndex() {
			for(int n = Count - 1; n >= 0; n--) {
				if(this[n].ForcedRow) return n;
			}
			return -1;
		}
		public GridRowInfo GetLastNonScrollableRow(GridView view, bool ignoreForced) {
			int res = GetFirstScrollableRowIndex(view, ignoreForced) - 1;
			return res > -1 ? this[res] : null;
		}
		public int GetFirstScrollableRowIndex(GridView view) { return GetFirstScrollableRowIndex(view, false); }
		public GridRowInfo GetFirstScrollableRow(GridView view) { return GetFirstScrollableRow(view, false); }
		public GridRowInfo GetFirstScrollableRow(GridView view, bool ignoreForced) {
			int res = GetFirstScrollableRowIndex(view, ignoreForced);
			if(res != -1) return this[res];
			return null;
		}
		public virtual int GetFirstScrollableRowIndex(GridView view, bool ignoreForced) {
			for(int n = 0; n < Count; n++) {
				if(!IsScrollableRow(view, this[n], ignoreForced)) continue;
				return n;
			}
			return -1;
		}
		public int GetLastVisibleRowIndex() {
			for(int n = Count - 1; n >= 0; n--) {
				if(this[n].TotalBounds.Height > 0) return this[n].VisibleIndex;
			}
			return 0;
		}
		public bool IsScrollableRow(GridView view, GridRowInfo rowInfo, bool ignoreForced) {
			GridDataRowInfo row = rowInfo as GridDataRowInfo;
			if(row != null && row.IsSpecialRow) {
				if(!row.IsNewItemRow || view.OptionsView.NewItemRowPosition == NewItemRowPosition.Top)
					return false;
			}
			if(!ignoreForced && rowInfo.ForcedRow) return false;
			return true;
		}
		public int GetScrollableRowsCount(GridView view) {
			int res = 0;
			for(int n = 0; n < Count; n++) {
				if(!IsScrollableRow(view, this[n], false)) continue;
				if(view.AllowFixedGroups) {
					GridGroupRowInfo group = this[n] as GridGroupRowInfo;
					if(group != null && group.GroupExpanded) continue;
				}
				if(this[n].TotalBounds.Height < 1) continue;
				res ++;
			}
			return res;
		}
		public int GetScrollableRowsHeight(GridView view, int maxBottom) {
			int res = 0;
			for(int n = 0; n < Count; n++) {
				if(!IsScrollableRow(view, this[n], false)) continue;
				if(view.AllowFixedGroups) {
					GridGroupRowInfo group = this[n] as GridGroupRowInfo;
					if(group != null && group.GroupExpanded) continue;
				}
				if(this[n].TotalBounds.Height < 1) continue;
				if(this[n].TotalBounds.Bottom > maxBottom) break;
				res += this[n].TotalBounds.Height;
			}
			return res;
		}
		public GridRowInfo GetInfo(int x, int y) {
			foreach(GridRowInfo row in this) {
				if(row.TotalBounds.Height > 0 && row.TotalBounds.Contains(x, y)) return row;
			}
			return null;
		}
		public GridRowInfo GetInfoByHandle(int rowHandle) {
			return Hash[rowHandle] as GridRowInfo;
		}
		protected Hashtable Hash { get { return hash; } }
		public virtual void ClearRows() {
			base.Clear();
			Hash.Clear();
		}
		protected internal void UpdateHash() {
			Hash.Clear();
			for(int n = 0; n < Count; n++) {
				Hash[this[n].RowHandle] = this[n];
			}
		}
		public GridRowInfo NextRow(GridRowInfo ri) {
			int i = IndexOf(ri);
			if(i < 0 || i == Count - 1) return null;
			return this[i + 1];
		}
	}
	public enum GridColumnInfoType { CaptionColumn, Column, Indicator, BehindColumn, EmptyColumn }
	public class GridColumnsInfo : CollectionBase {
		public GridColumnsInfo() {
		}
		public virtual void Add(GridColumnInfoArgs ci) { 
			List.Add(ci); 
		}
		public GridColumnInfoArgs this[GridColumn column] {
			get {
				if(column == null) return null;
				for(int n = 0; n < Count; n++) {
					GridColumnInfoArgs ci = this[n];
					if(ci.Column == column) return ci;
				}
				return null;
			}
		}
		public GridColumnInfoArgs LastColumnInfo { 
			get {
				for(int n = Count - 1; n > -1; n--) {
					GridColumnInfoArgs ci = this[n];
					if(ci.Type == GridColumnInfoType.Column)
						return ci;
				}
				return null;
			}
		}
		public int IndexOf(GridColumnInfoArgs ci) { return List.IndexOf(ci); }
		internal GridColumnInfoArgs FirstNonGroupColumnInfo {
			get {
				for(int n = 0; n < Count; n++) {
					GridColumnInfoArgs ci = this[n];
					if(ci.Type == GridColumnInfoType.Column && ci.Column.GroupIndex < 0)
						return ci;
				}
				return null;
			}
		}
		public GridColumnInfoArgs FirstColumnInfo { 
			get {
				for(int n = 0; n < Count; n++) {
					GridColumnInfoArgs ci = this[n];
					if(ci.Type == GridColumnInfoType.Column)
						return ci;
				}
				return null;
			}
		}
		public GridColumnInfoArgs this[int index] { 
			get { 
				if(index < 0 || index >= Count) return null;
				return List[index] as GridColumnInfoArgs; 
			} 
		}
		public int ColumnsCount {
			get {
				int count = 0;
				foreach(GridColumnInfoArgs ci in this) {
					if(ci.Type == GridColumnInfoType.Column)
						count++;
				}
				return count;
			}
		}
	}
	public abstract class CellMerger {
		protected static readonly object empty = new object();
		protected static readonly object finish = new object();
		protected abstract object GetCellValue(int row, int column);
		protected virtual internal int RaiseMergeEvent(int row1, int row2, int column) {
			return 3;
		}
		public virtual int GetCellMergeCount(int startRow, int row, int column, object prevValue) {
			int res = 0;
			while(true) {
				object cell = GetCellValue(row, column);
				if(cell == finish) break;
				if(cell == empty) break;
				int merge = RaiseMergeEvent(startRow, row, column);
				if(merge == 0 || (merge == 3 && (cell == prevValue || object.Equals(cell, prevValue)))) {
					res ++;
					row ++;
					continue;
				}
				break;
			}
			return res;
		}
		public virtual int CalcMergeCount(int startRow, int column) {
			while(true) {
				object val = GetCellValue(startRow, column);
				if(val == finish) return -1;
				if(val == empty) return 0;
				int mergeCount = GetCellMergeCount(startRow, startRow + 1, column, val);
				return mergeCount;
			}
		}
	}
	public class GridViewInfoCellMerger : CellMerger {
		CellMergeEventArgs mergeArgs = new CellMergeEventArgs(0, 0, null);
		GridMergedCellInfoCollection mergedCells;
		GridViewInfo viewInfo;
		public GridViewInfoCellMerger(GridViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			this.mergedCells = new GridMergedCellInfoCollection();
		}
		public virtual void Clear() {
			MergedCells.ClearCells();
		}
		public GridMergedCellInfoCollection MergedCells { get { return mergedCells; } }
		protected GridViewInfo ViewInfo { get { return viewInfo; } }
		protected override internal int RaiseMergeEvent(int row1, int row2, int column) {
			GridColumnInfoArgs columnInfo = ViewInfo.ColumnsInfo[column];
			mergeArgs.Setup(GetRowHandle(row1), GetRowHandle(row2), columnInfo == null ? null : columnInfo.Column);
			ViewInfo.View.RaiseCellMerge(mergeArgs);
			return this.mergeArgs.Handled ? (this.mergeArgs.Merge ? 0 : -1) : 3;
		}
		int GetRowHandle(int row) { 
			GridDataRowInfo dr = GetRowInfo(row);
			return dr == null ? GridControl.InvalidRowHandle : dr.RowHandle;
		}
		GridDataRowInfo GetRowInfo(int row) {
			if(row >= this.viewInfo.RowsInfo.Count) return null;
			GridDataRowInfo dr = this.viewInfo.RowsInfo[row] as GridDataRowInfo;
			if(dr == null || dr.IsNewItemRow || this.viewInfo.View.IsFilterRow(dr.RowHandle) || dr.MasterRowExpanded) return null;
			return dr;
		}
		protected override object GetCellValue(int row, int column) {
			if(row >= this.viewInfo.RowsInfo.Count) return finish;
			GridDataRowInfo dr = GetRowInfo(row);
			if(dr == null) return empty;
			return column < dr.Cells.Count ? dr.Cells[column].CellValue : empty;
		}
		public virtual void Calc() {
			for(int c = 0; c < ViewInfo.ColumnsInfo.Count; c++) {
				GridColumnInfoArgs info = ViewInfo.ColumnsInfo[c];
				if(info.Column == null) continue;
				if(!info.Column.GetAllowMerge()) continue;
				Calc(c);
			}
		}
		public virtual void CalcEditors() {
			foreach(GridMergedCellInfo mergedCell in MergedCells) {
				mergedCell.Update();
				ViewInfo.UpdateCellAppearance(mergedCell, true);
				int index = ViewInfo.ColumnsInfo.IndexOf(mergedCell.ColumnInfo);
				GridColumnInfoArgs nextColumn = index + 1 < ViewInfo.ColumnsInfo.Count ? ViewInfo.ColumnsInfo[index + 1] : null;
				ViewInfo.CalcRowCellDrawInfoCore(mergedCell.RowInfo, mergedCell.ColumnInfo, mergedCell, nextColumn, true, null);
			}
		}
		protected virtual void Calc(int column) {
			int index = 0;
			while(true) {
				int count = CalcMergeCount(index, column);
				if(count == -1) break;
				if(count == 0) {
					index ++;
					continue;
				}
				GridMergedCellInfo merged = new GridMergedCellInfo(ViewInfo.ColumnsInfo[column]);
				for(int n = 0; n < count + 1; n++) {
					GridDataRowInfo row = ViewInfo.RowsInfo[index + n] as GridDataRowInfo;
					if(row.Cells[column] == GridCellInfo.EmptyCellInfo) return; 
					merged.AddMergedCell(row.Cells[column]);
				}
				MergedCells.Add(merged);
				index += count + 1;
			}
		}
	}
	public class GridRowFooterCollection : CollectionBase {
		public int RowFootersHeight;
		public int RowFooterCount;
		public Rectangle Bounds;
		public GridRowFooterCollection() {
			Clear();
		}
		public void Add(GridRowFooterInfo footer) { 
			List.Add(footer);
		}
		protected override void OnClearComplete() {
			this.Bounds = Rectangle.Empty;
			RowFootersHeight = 0;
			RowFooterCount = 0;
			base.OnClearComplete();
		}
		public GridRowFooterInfo this[int index] {
			get { return List[index] as GridRowFooterInfo; }
		}
		public virtual void OffsetContent(int x, int y) {
			if(!Bounds.IsEmpty) Bounds.Offset(x, y);
			foreach(GridRowFooterInfo info in this) info.OffsetContent(x, y);
		}
	}
	public class GridCellInfoCollection : List<GridCellInfo> {
		public GridCellInfo this[GridColumn column] { 
			get {
				return Find(q => q.Column == column);
			} 
		}
		public virtual void ClearCells() {
			Clear();
		}
		public virtual void OffsetContent(int x, int y) {
			foreach(GridCellInfo info in this) info.OffsetContent(x, y);
		}
	}
	public class GridFooterCellInfoCollection : CollectionBase {
		public int Add(GridFooterCellInfoArgs info) {
			return List.Add(info);
		}
		public GridFooterCellInfoArgs this[int index] { get { return List[index] as GridFooterCellInfoArgs; } }
		public virtual void OffsetContent(int x, int y) {
			foreach(GridFooterCellInfoArgs info in this) info.OffsetContent(x, y);
		}
	}
	public class GridRowFooterInfo {
		public int Indent, Level;
		public int RowHandle;
		public GridFooterCellInfoCollection Cells;
		public Rectangle Bounds, IndicatorRect;
		bool isDirty;
		public GridRowFooterInfo() {
			Cells = new GridFooterCellInfoCollection();
			Clear();
			this.isDirty = false;
		}
		public virtual void Clear() {
			Level = Indent = 0;
			IndicatorRect = Bounds = Rectangle.Empty;
			RowHandle = GridControl.InvalidRowHandle;
			Cells.Clear();
		}
		public bool IsDirty { get { return isDirty; } set { isDirty = value; } }
		public void SetDirty() { this.isDirty = true; }
		public virtual void OffsetContent(int x, int y) {
			Bounds.Offset(x, y);
			if(!IndicatorRect.IsEmpty) IndicatorRect.Offset(x, y);
			Cells.OffsetContent(x, y);
		}
	}
	public enum GridHitTest { None, Column, ColumnEdge, ColumnButton, ColumnFilterButton, ColumnPanel,
		RowCell, RowIndicator, RowGroupButton, RowGroupCheckSelector, Row, RowPreview, RowDetail, RowDetailEdge, 
		RowDetailIndicator, EmptyRow, GroupPanel, GroupPanelColumn, GroupPanelColumnFilterButton, Footer, CellButton,
		CustomizationForm, FilterPanel, FilterPanelCloseButton, RowFooter, RowEdge, FixedLeftDiv, FixedRightDiv, VScrollBar, HScrollBar,
		FilterPanelActiveButton, FilterPanelText, FilterPanelMRUButton, FilterPanelCustomizeButton, ViewCaption, MasterTabPageHeader, RowGroupCell
	};
	public class GridHitInfo : BaseHitInfo {
		static GridHitInfo empty = null;
		internal static GridHitInfo Empty {
			get { 
				if(empty == null) empty = new GridHitInfo();
				return empty;
			}
		}
		protected GridCellInfo fCellInfo;
		protected GridColumnInfoArgs fColumnInfo;
		GridFooterCellInfoArgs footerCell;
		GridColumn fColumn;
		int rowHandle;
		GridHitTest hitTest;
		public GridHitInfo() {
			Clear();
		}
		public override void Clear() {
			base.Clear();
			this.fColumn = null;
			this.fCellInfo = null;
			this.fColumnInfo = null;
			Column = null;
			this.rowHandle = GridControl.InvalidRowHandle;
			this.hitTest = GridHitTest.None;
		}
		protected virtual void SetHitTest(int val) {
			this.hitTest = (GridHitTest)val;
		}
		public int RowHandle { get { return rowHandle; } set { rowHandle = value; } }
		internal int ListSourceRowIndex { get; set; }
		internal GridCellInfo CellInfo { get { return fCellInfo; } set { fCellInfo = value; } }
		internal GridColumnInfoArgs ColumnInfo { get { return fColumnInfo; } set { fColumnInfo = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public GridFooterCellInfoArgs FooterCell { 
			get { return footerCell; } 
			internal set { footerCell = value; } 
		}
		protected internal override int HitTestInt { get { return (int)hitTest; } }
		public new GridView View { get { return (GridView)base.View; } set { base.View = value; } }
		public virtual GridHitTest HitTest { get { return hitTest; } set { SetHitTest((int)value); } }
		public virtual GridColumn Column { get { return fColumn; } set { fColumn = value; } }
		public virtual bool InGroupPanel {
			get {
				return IsValid && 
					(HitTest == GridHitTest.GroupPanel || HitTest == GridHitTest.GroupPanelColumn ||
					HitTest == GridHitTest.GroupPanelColumnFilterButton);
			}
		}
		protected internal bool CheckHitTest(int x, int left, int right, GridHitTest hitTest) {
			if(right < left) {
				int temp = left;
				left = right;
				right = temp;
			}
			if(x >= left && x < right) {
				HitTest = hitTest;
				return true;
			}
			return false;
		}
		protected internal bool CheckHitTest(Rectangle bounds, Point point, GridHitTest hitTest) {
			if(GridDrawing.PtInRect(bounds, point)) {
				HitTest = hitTest;
				return true;
			}
			return false;
		}
		public virtual bool InFilterPanel {
			get { 
				return IsValid && (HitTest == GridHitTest.FilterPanel || HitTest == GridHitTest.FilterPanelActiveButton ||
					HitTest == GridHitTest.FilterPanelCloseButton || HitTest == GridHitTest.FilterPanelMRUButton ||
					HitTest == GridHitTest.FilterPanelText || HitTest == GridHitTest.FilterPanelCustomizeButton);
			}
		}
		public virtual bool InGroupColumn {
			get {
				return IsValid && 
					(HitTest == GridHitTest.GroupPanelColumn ||
					HitTest == GridHitTest.GroupPanelColumnFilterButton);
			}
		}
		public virtual bool InColumn { 
			get { 
				return IsValid && Column != null &&
					(HitTest == GridHitTest.Column || HitTest == GridHitTest.ColumnFilterButton);
			}
		}
		public virtual bool InColumnPanel {
			get {
				return IsValid && (HitTest == GridHitTest.Column || 
					HitTest == GridHitTest.ColumnEdge || 
					HitTest == GridHitTest.ColumnPanel ||
					HitTest == GridHitTest.ColumnFilterButton);
			}
		}
		public virtual bool InRowCell {
			get {
				return IsValid && (HitTest == GridHitTest.RowCell ||
					HitTest == GridHitTest.CellButton);
			}
		}
		public virtual bool InDataRow {
			get {
				if(!InRow) return false;
				return RowHandle >= 0;
			}
		}
		public virtual bool InGroupRow {
			get {
				if(!InRow || InDataRow) return false;
				if(GridControl.AutoFilterRowHandle == RowHandle || GridControl.NewItemRowHandle == RowHandle) return false;
				return true;
			}
		}
		public virtual bool InRow {
			get {
				return IsValid && (HitTest == GridHitTest.Row ||
					HitTest == GridHitTest.RowCell ||
					HitTest == GridHitTest.RowPreview ||
					HitTest == GridHitTest.RowGroupButton ||
					HitTest == GridHitTest.RowGroupCheckSelector ||
					HitTest == GridHitTest.RowIndicator ||
					HitTest == GridHitTest.CellButton ||
					HitTest == GridHitTest.RowDetail || 
					HitTest == GridHitTest.RowDetailIndicator ||
					HitTest == GridHitTest.RowDetailEdge || 
					HitTest == GridHitTest.RowEdge || 
					HitTest == GridHitTest.RowGroupCell);
			}
		}
		internal void SetRowHandle(int rowHandle, int listSourceRowIndex) {
			this.rowHandle = rowHandle;
			this.ListSourceRowIndex = listSourceRowIndex;
		}
	}
	public class GroupRowCheckboxSelectorInfoArgs : CheckObjectInfoArgs {
		public GroupRowCheckboxSelectorInfoArgs() : base(new AppearanceObject()) {
			IsDirty = true;
		}
		public bool IsDirty { get; set; }
	}
	public class ColumnCheckboxSelectorInfoArgs : CheckObjectInfoArgs {
		GridView view;
		public ColumnCheckboxSelectorInfoArgs(GridView view)
			: base(new AppearanceObject()) {
			this.view = view;
		}
		public override CheckState CheckState {
			get {
				if(view == null) return CheckState.Unchecked;
				int selectedRowCount = view.SelectedRowsCount;
				if(selectedRowCount == 0) return System.Windows.Forms.CheckState.Unchecked;
				if(view.IsAllRowsSelected) return System.Windows.Forms.CheckState.Checked;
				return System.Windows.Forms.CheckState.Indeterminate;
			}
		}
	}
}
