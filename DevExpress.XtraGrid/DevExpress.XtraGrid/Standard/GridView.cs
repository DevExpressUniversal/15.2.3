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
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data.Details;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Helpers;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Container;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Tab;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Views.Grid.Customization;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraGrid.Localization;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Text;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraGrid.GroupSummaryEditor;
using System.Collections.Generic;
using DevExpress.Data.Summary;
using DevExpress.Utils.Serializing.Helpers;
using System.Drawing.Design;
using DevExpress.Utils.Editors;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraGrid.Frames;
using DevExpress.XtraGrid.Dragging;
using DevExpress.Utils.Gesture;
using DevExpress.XtraGrid.Internal;
using DevExpress.Skins;
using DevExpress.XtraGrid.EditForm.Helpers;
using DevExpress.XtraExport;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export;
namespace DevExpress.XtraGrid.Views.Grid {
	[DesignTimeVisible(false), ToolboxItem(false),
	Designer("DevExpress.XtraGrid.Design.GridViewComponentDesigner, " + AssemblyInfo.SRAssemblyGridDesign)]
	public class GridView : DevExpress.XtraGrid.Views.Base.ColumnView, IGridDesignTime, IDataControllerRelationSupport,
		IDataControllerValidationSupport, IAccessibleGrid, IGridLookUp, ISummaryItemsOwner, IXtraSupportDeserializeCollection {
		Timer scrollTimer;
		Rectangle customizationFormBounds;
		DrawFocusRectStyle focusRectStyle;
		internal DetailTip detailTip;
		bool destroying;
		bool forceDesignMode;
		CustomizationForm customizationForm;
		int topRowIndex, leftCoord, detailVerticalIndent,
			fixedLineWidth;
		GridState state;
		int internalUpdateCount, rowHeight, footerPanelHeight, groupRowHeight, indicatorWidth, columnPanelRowHeight, rowSeparatorHeight, defaultRelationIndex;
		string groupFormat, groupPanelText, previewFieldName, vertScrollTipFieldName, childGridLevelName, newItemRowText;
		int previewLineCount, bestFitMaxRowCount, lockSummary, horzScrollStep, previewIndent, levelIndent;
		bool requireCheckTopRowIndex = false;
		protected GridCellInfo fEditingCell;
		GroupSummarySortInfoCollection groupSummarySortInfo;
		EditFormController editFormController;
		ScrollVisibility horzScrollVisibility, vertScrollVisibility;
		ScrollStyleFlags scrollStyle;
		GridTotalSummaryCollection totalSummary;
		protected GridGroupSummaryItemCollection fGroupSummary;
		protected MasterRowEmptyEventArgs isMasterRowEmptyEventArgs = new MasterRowEmptyEventArgs(0, 0, true);
		ScrollInfo scrollInfo;
		protected Rectangle fViewRect, fSavedNormalRectangle;
		private static readonly object showingPopupEditForm = new object();
		private static readonly object rowClick = new object();
		private static readonly object rowCellClick = new object();
		private static readonly object customDrawHeader = new object();
		private static readonly object customDrawGroupPanel = new object();
		private static readonly object customDrawColumnHeader= new object();
		private static readonly object customDrawRowIndicator= new object();
		private static readonly object customDrawCell= new object(), customDrawRowFooterCell= new object(), customDrawFooterCell= new object();
		private static readonly object customDrawRowPreview= new object(), 
			customDrawGroupRow = new object(), customDrawGroupRowCell = new object(), customDrawRowFooter= new object(), customDrawFooter= new object();
		private static readonly object getThumbnailImage = new object();
		private static readonly object getLoadingImage = new object();
		private static readonly object customColumnGroup = new object();
		private static readonly object groupLevelStyle= new object();
		private static readonly object rowCellStyle= new object();
		private static readonly object rowStyle= new object();
		private static readonly object masterRowEmpty= new object();
		private static readonly object masterRowExpanding = new object();
		private static readonly object masterRowExpanded = new object();
		private static readonly object masterRowCollapsing = new object();
		private static readonly object masterRowCollapsed = new object();
		private static readonly object groupRowExpanding = new object();
		private static readonly object groupRowExpanded = new object();
		private static readonly object groupRowCollapsing = new object();
		private static readonly object groupRowCollapsed = new object();
		private static readonly object masterRowGetLevelDefaultView= new object();
		private static readonly object masterRowGetChildList= new object();
		private static readonly object masterRowGetRelationName= new object();
		private static readonly object masterRowGetRelationDisplayCaption = new object();
		private static readonly object masterRowGetRelationCount= new object();
		private static readonly object dragObjectStart = new object();
		private static readonly object dragObjectOver = new object();
		private static readonly object dragObjectDrop = new object();
		private static readonly object beforePrintRow = new object();
		private static readonly object afterPrintRow = new object();
		private static readonly object leftCoordChanged= new object(), topRowChanged= new object();
		private static readonly object columnWidthChanged= new object();
		private static readonly object customRowCellEdit = new object();
		private static readonly object customRowCellEditForEditing = new object();
		private static readonly object editFormShowing = new object();
		private static readonly object editFormPrepared = new object();
		private static readonly object gridMenuItemClick= new object();
		private static readonly object showGridMenu= new object();
		private static readonly object onPopupMenuShowing = new object();
		private static readonly object cellMerge = new object();
		private static readonly object showCustomizationForm = new object();
		private static readonly object hideCustomizationForm = new object();
		private static readonly object calcRowHeight= new object();
		private static readonly object customSummaryCalculate= new object();
		private static readonly object customSummaryExists= new object();
		private static readonly object calcPreviewText= new object();
		private static readonly object measurePreviewHeight= new object();
		const string defaultGroupFormat = "{0}: [#image]{1} {2}";
		public const string CheckBoxSelectorColumnName = "DX$CheckboxSelectorColumn";
		const int ScrollTimerInterval = 10;
		GridOptionsHint _optionsHint;
		GridOptionsDetail _optionsDetail;
		GridOptionsCustomization _optionsCustomization;
		GridOptionsNavigation _optionsNavigation;
		GridOptionsMenu _optionsMenu;
		GridOptionsEditForm _optionsEditForm;
		GridViewScroller scroller;
		RowCellStyleEventArgs styleEventArgs = new RowCellStyleEventArgs(0, null, GridRowCellState.Default, null);
		public GridView(GridControl ownerGrid) : this() {
			SetGridControl(ownerGrid);
		}
		public GridView() {
			this.defaultRelationIndex = 0;
			this.customizationFormBounds = Rectangle.Empty;
			this.scrollTimer = new Timer();
			this.scrollTimer.Interval = ScrollTimerInterval;
			this.scrollTimer.Tick += new EventHandler(OnScrollTimer_Tick);
			this.horzScrollStep = 3;
			this.fixedLineWidth = 2;
			this.indicatorWidth = -1;
			this.lockSummary = 0;
			this.childGridLevelName = "";
			this.groupSummarySortInfo = new GroupSummarySortInfoCollection(this);
			this._optionsHint = CreateOptionsHint();
			this._optionsDetail = CreateOptionsDetail();
			this._optionsCustomization = CreateOptionsCustomization();
			this._optionsNavigation = CreateOptionsNavigation();
			this._optionsMenu = CreateOptionsMenu();
			this._optionsEditForm = CreateOptionsEditForm();
			this._optionsClipboard = CreateOptionsClipboard();
			this._optionsDetail.Changed += new BaseOptionChangedEventHandler(OnOptionChanged); 
			this._optionsCustomization.Changed += new BaseOptionChangedEventHandler(OnOptionChanged); 
			this._optionsNavigation.Changed += new BaseOptionChangedEventHandler(OnOptionChanged); 
			this._optionsMenu.Changed += new BaseOptionChangedEventHandler(OnMiscOptionChanged); 
			this._optionsHint.Changed += new BaseOptionChangedEventHandler(OnMiscOptionChanged);
			this._optionsEditForm.Changed += new BaseOptionChangedEventHandler(OnOptionChanged); 
			this.vertScrollTipFieldName = "";
			this.footerPanelHeight = this.columnPanelRowHeight = -1;
			horzScrollVisibility = vertScrollVisibility = ScrollVisibility.Auto;
			scrollStyle = DefaultScrollStyle;
			groupRowHeight = rowHeight = -1;
			focusRectStyle = DrawFocusRectStyle.CellFocus;
			bestFitMaxRowCount = -1;
			fGroupSummary = new GridGroupSummaryItemCollection(this);
			fGroupSummary.CollectionChanged += new CollectionChangeEventHandler(OnSummaryCollectionChanged);
			this.totalSummary = new GridTotalSummaryCollection(this);
			this.newItemRowText = this.groupPanelText = string.Empty;
			detailTip = new DetailTip(this, DevExpress.Data.DataController.InvalidRow);
			destroying = false;
			customizationForm = null;
			previewFieldName = "";
			this.forceDesignMode = false;
			this.fEditingCell = null;
			this.leftCoord = 0;
			this.detailVerticalIndent = 0;
			this.topRowIndex = 0;
			this.state = GridState.Normal;
			internalUpdateCount = 0;
			this.groupFormat = defaultGroupFormat;
			this.previewIndent = -1;
			this.levelIndent = -1;
			this.previewLineCount = -1;
			this.fSavedNormalRectangle = Rectangle.Empty;
			this.fViewRect = Rectangle.Empty;
			this.scrollInfo = CreateScrollInfo();
			this.scrollInfo.VScroll_Scroll += new ScrollEventHandler(OnVertScroll);
			this.scrollInfo.HScroll_Scroll += new ScrollEventHandler(OnHorzScroll);
			this.scrollInfo.VScroll_ValueChanged += new EventHandler(OnVScroll);
			this.scrollInfo.HScroll_ValueChanged += new EventHandler(OnHScroll);
		}
		protected override void SetupDataController() {
			base.SetupDataController();
			this.DataController.RelationSupport = this;
			this.DataController.ValidationClient = this;
			DataControllerCore.GroupSummary.CollectionChanged += new CollectionChangeEventHandler(OnSummaryChanged);
			DataControllerCore.TotalSummary.CollectionChanged += new CollectionChangeEventHandler(OnSummaryChanged);
			DataControllerCore.CustomSummary += new CustomSummaryEventHandler(OnDataManager_CustomSummaryEvent);
			DataControllerCore.CustomSummaryExists += new CustomSummaryExistEventHandler(OnDataManager_CustomSummaryExistEvent);
		}
		protected internal override void DestroyDataController() {
			if(DataControllerCore != null) {
				DataControllerCore.GroupSummary.CollectionChanged -= new CollectionChangeEventHandler(OnSummaryChanged);
				DataControllerCore.TotalSummary.CollectionChanged -= new CollectionChangeEventHandler(OnSummaryChanged);
				DataControllerCore.CustomSummary -= new CustomSummaryEventHandler(OnDataManager_CustomSummaryEvent);
				DataControllerCore.CustomSummaryExists -= new CustomSummaryExistEventHandler(OnDataManager_CustomSummaryExistEvent);
			}
			base.DestroyDataController();
		}
		protected override bool CanLeaveFocusOnTab(bool moveForward) {
			int focusedVisible = GetVisibleIndex(FocusedRowHandle);
			if(this.RowCount == 0) return true; 
			if(moveForward && focusedVisible == (this.RowCount - 1)) return true;
			if(!moveForward && focusedVisible == 0) return true;
			return false;
		}
		protected override void LeaveFocusOnTab(bool moveForward) { this.GridControl.ProcessControlTab(moveForward); }
		protected override BaseViewInfo CreateNullViewInfo() { return new NullGridViewInfo(this); }
		protected internal virtual bool FootersIgnoreColumnFormat { get { return false; } }
		protected override BaseViewAppearanceCollection CreateAppearancesPrint() { return new GridViewPrintAppearances(this); }
		void ResetAppearancePrint() { AppearancePrint.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancePrint"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public new GridViewPrintAppearances AppearancePrint { get { return base.AppearancePrint as GridViewPrintAppearances; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewAppearances PaintAppearance { get { return base.PaintAppearance as GridViewAppearances; } }
		protected override BaseViewAppearanceCollection CreateAppearances() { return new GridViewAppearances(this); }
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearance"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public new GridViewAppearances Appearance { get { return base.Appearance as GridViewAppearances; } }
		bool ShouldSerializeOptionsTouch() { return OptionsImageLoad.ShouldSerializeCore(this); }
		void ResetOptionsTouch() { OptionsImageLoad.Reset(); }
		GridViewOptionsImageLoad optionsImageLoad;
		[ DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewOptionsImageLoad OptionsImageLoad {
			get {
				if(optionsImageLoad == null)
					optionsImageLoad = CreateOptionsImageLoad();
				return optionsImageLoad;
			}
		}
		protected virtual GridViewOptionsImageLoad CreateOptionsImageLoad() { return new GridViewOptionsImageLoad(); }
		protected virtual GridOptionsHint CreateOptionsHint() { return new GridOptionsHint(); }
		protected virtual GridOptionsDetail CreateOptionsDetail() { return new GridOptionsDetail(); }
		protected virtual GridOptionsCustomization CreateOptionsCustomization() { return new GridOptionsCustomization(); }
		protected virtual GridOptionsNavigation CreateOptionsNavigation() { return new GridOptionsNavigation(); }
		protected override ColumnViewOptionsView CreateOptionsView() { return new GridOptionsView(); }
		protected override ViewPrintOptionsBase CreateOptionsPrint() { return new GridOptionsPrint(); }
		protected virtual GridOptionsMenu CreateOptionsMenu() { return new GridOptionsMenu(); }
		protected override ColumnViewOptionsFind CreateOptionsFind() { return new GridViewOptionsFind(); }
		protected virtual GridOptionsEditForm CreateOptionsEditForm() { return new GridOptionsEditForm(); }
		protected virtual GridOptionsClipboard CreateOptionsClipboard() { return new GridOptionsClipboard(); }
		protected override ColumnViewOptionsSelection CreateOptionsSelection() { return new GridOptionsSelection(); }
		protected override ColumnViewOptionsBehavior CreateOptionsBehavior() { return new GridOptionsBehavior(this); }
		protected virtual ScrollInfo CreateScrollInfo() {
			return 	new ScrollInfo(this);
		}
		protected override void OnMiscOptionChanged(object sender, BaseOptionChangedEventArgs e) {
			if(CanSynchronized) {
				OnViewPropertiesChanged(SynchronizationMode.Visual);
			}
			if(sender == OptionsHint) LayoutChangedSynchronized();
			FireChanged();
		}
		public override bool IsDataRow(int rowHandle) {
			if(IsGroupRow(rowHandle) || IsFilterRow(rowHandle)) return false;
			return base.IsDataRow(rowHandle);
		}
		public override bool IsValidRowHandle(int rowHandle) {
			if(IsFilterRow(rowHandle) && OptionsView.ShowAutoFilterRow) return true;
			bool res = DataController.IsValidControllerRowHandle(rowHandle);
			if(res) return true;
			if(rowHandle == CurrencyDataController.NewItemRow) {
				if(OptionsView.NewItemRowPosition != NewItemRowPosition.None || DataController.IsNewItemRowEditing) return true;
			}
			return false;
		}
		protected override void OnSelectionChangedCore(SelectionChangedEventArgs e) {
			if(IsCheckboxSelectorMode && GroupCount > 0) {
				if(e.Action == CollectionChangeAction.Add) {
					CheckCheckboxSelectorGroupSelection(e.ControllerRow, true);
				}
				if(e.Action == CollectionChangeAction.Remove) {
					CheckCheckboxSelectorGroupSelection(e.ControllerRow, false);
				}
			}
			base.OnSelectionChangedCore(e);
		}
		bool CheckCheckboxSelectorGroupSelection(int rowHandle, bool select) {
			if(GroupCount == 0) return false;
			if(IsAllChildrenSelectedOrNot(GetParentRowHandle(rowHandle), select)) {
				if(select) 
					SelectRow(GetParentRowHandle(rowHandle));
				else
					UnselectRow(GetParentRowHandle(rowHandle));
				if(CheckCheckboxSelectorGroupSelection(GetParentRowHandle(rowHandle), select)) return true;
			}
			return false;
		}
		bool IsAllChildrenSelectedOrNot(int rowHandle, bool selectionState) {
			GroupRowInfo gi = DataController.GroupInfo.GetGroupRowInfoByHandle(rowHandle);
			if(gi == null) return false;
			List<GroupRowInfo> l = new List<GroupRowInfo>();
			DataController.GroupInfo.GetChildrenGroups(gi, l);
			if(l.Cast<GroupRowInfo>().Where(q=>IsRowSelected(q.Handle) != selectionState).Any()) return false;
			for(int n = 0; n < gi.ChildControllerRowCount; n++) {
				if(IsRowSelected(gi.ChildControllerRow + n) != selectionState) return false;
			}
			return true;
		}
		protected internal override void OnPropertiesChanged() {
			if(!IsLoading) {
				ViewInfo.ResetPositionInfo();
				if(IsPixelScrolling) SetTopRowIndexDirty();
			}
			base.OnPropertiesChanged();
		}
		protected override void OnOptionChanged(object sender, BaseOptionChangedEventArgs e) {
			bool hideEditForm = false;
			if(sender == OptionsBehavior) {
				this.isPixelScrolling = null;
				DataController.AutoUpdateTotalSummary = OptionsBehavior.AutoUpdateTotalSummary;
				DataController.SummariesIgnoreNullValues = OptionsBehavior.SummariesIgnoreNullValues;
				if(DataController.AutoExpandAllGroups != OptionsBehavior.AutoExpandAllGroups) {
					DataController.AutoExpandAllGroups = OptionsBehavior.AutoExpandAllGroups;
					if(OptionsBehavior.AutoExpandAllGroups) DataController.ExpandAll();
				}
				HideEditForm();
				if(!IsInplaceEditFormVisible && e.Name == "EditingMode") {
					DestroyEditFormController();
				}
			}
			if(sender == OptionsEditForm) {
				if(e.Name == "ShowUpdateCancelPanel") hideEditForm = true;
				if(e.Name == "CustomEditFormLayout") {
					HideEditForm();
					if(IsInplaceEditFormVisible && OptionsEditForm.CustomEditFormLayout != null) {
						OptionsEditForm.CustomEditFormLayout = null;
					}
					if(!IsInplaceEditFormVisible) DestroyEditFormController();
				}
			}
			if(sender == OptionsDetail) {
				if(e.Name == "EnableMasterViewMode" && !((bool)e.NewValue)) CollapseAllDetails();
				DataController.DetailEmptyHash.Clear();
			}
			if(sender == OptionsView) {
				if(e.Name == "ColumnAutoWidth") LeftCoord = 0;
				if(e.Name == "ShowPreview") CheckPreviewColumn();
				if(e.Name == "RowAutoHeight") this.isPixelScrolling = null;
			}
			if(sender == OptionsFind) {
				CheckPreviewColumn();
			}
			if(sender == OptionsSelection) {
				CheckCheckboxSelector();
			}
			if(hideEditForm) {
				HideEditForm();
				if(!IsInplaceEditFormVisible) DestroyEditFormController();
			}
			base.OnOptionChanged(sender, e);
			if(sender == OptionsBehavior) {
				if(e.Name == "AllowPartialGroups") {
					if(GroupCount > 0 && DataController.IsReady) RefreshData();
				}
			}
		}
		protected internal override ViewDrawArgs CreateDrawArgs(DXPaintEventArgs e, GraphicsCache cache) {
			if(cache == null) cache = new GraphicsCache(e, Painter.Paint);
			return new GridViewDrawArgs(cache, ViewInfo, ViewInfo.ViewRects.Bounds);
		}
		protected internal override void OnRepositoryItemRefreshRequired(RepositoryItem item) {
			if(!ViewInfo.IsReady) return;
			bool requireInvalide = false;
			if(GroupedColumns.Cast<GridColumn>().FirstOrDefault(q => q.ColumnEdit == item) != null) {
				LayoutChangedSynchronized();
				return;
			}
			foreach(GridRowInfo row in ViewInfo.RowsInfo) {
				GridDataRowInfo dataRow = row as GridDataRowInfo;
				if(dataRow == null) continue;
				foreach(GridCellInfo cell in dataRow.Cells) {
					if(cell.ViewInfo != null && cell.ViewInfo.Item == item) {
						cell.ViewInfo.RefreshDisplayText = true;
						ViewInfo.UpdateCellEditViewInfo(cell, ViewInfo.MousePosition, false, false);
						requireInvalide = true;
					}
				}
			}
			if(requireInvalide) InvalidateRows();
		}
		protected internal bool IsShowCheckboxSelectorInHeader {
			get { return CheckboxSelectorColumn != null && OptionsSelection.ShowCheckBoxSelectorInColumnHeader != DefaultBoolean.False; }
		}
		protected internal bool IsShowCheckboxSelectorInGroupRow { 
			get { return CheckboxSelectorColumn != null && OptionsSelection.ShowCheckBoxSelectorInGroupRow == DefaultBoolean.True && GroupCount  > 0 && !IsDataBoundSelection; }
		}
		protected override void InvalidateSelection(SelectionChangedEventArgs e) {
			if(IsShowCheckboxSelectorInHeader) InvalidateColumnHeader(CheckboxSelectorColumn);
			if(IsShowCheckboxSelectorInGroupRow) {
				ViewInfo.RowsInfo.SetGroupSelectorDirty();
				InvalidateRows();
			}
			if(!this.lockSelectionInvalidate) {
				if(e.Action == CollectionChangeAction.Refresh) {
					UpdateCheckboxSelectorColumnValue(InvalidRowHandle);
					InvalidateRows();
				}
				else {
					InvalidateSelectionRow(e.ControllerRow);
				}
			}
		}
		protected void InvalidateSelectionRow(int rowHandle) {
			UpdateCheckboxSelectorColumnValue(rowHandle);
			InvalidateRow(rowHandle);
		}
		protected internal override void InvertFocusedRowSelectionCore(BaseHitInfo hitInfo) {
			GridHitInfo gridInfo = hitInfo as GridHitInfo;
			if(IsCellSelect && (gridInfo == null || gridInfo.HitTest != GridHitTest.RowIndicator)) {
				InvertCellSelection(FocusedRowHandle, FocusedColumn);
				return;
			}
			base.InvertFocusedRowSelectionCore(hitInfo);
		}
		protected internal override void SelectFocusedRowCore() {
			if(IsCellSelect) 
				SelectFocusedCellCore();
			else
				base.SelectFocusedRowCore();
		}
		public virtual void InvertCellSelection(int rowHandle, GridColumn focusedColumn) {
			if(!IsCellSelect) return;
			if(IsValidRowHandle(rowHandle)) {
				if(IsGroupRow(rowHandle)) {
					InvertRowSelection(rowHandle);
					return;
				}
				if(IsCellSelected(rowHandle, focusedColumn))
					UnselectCell(rowHandle, focusedColumn);
				else
					SelectCell(rowHandle, focusedColumn);
			}
		}
		protected internal virtual void SelectFocusedCellRangeCore(GridCell start, GridCell end, KeyEventArgs e) {
			if(!IsCellSelect || (Control.ModifierKeys & Keys.Control) != 0) return;
			if((Control.ModifierKeys & Keys.Shift) != 0 && e.KeyCode != Keys.Tab) {
				if(SelectionAnchorRowHandle == InvalidRowHandle) {
					SelectionAnchor = new GridCell(start.RowHandle, start.Column);
				}
				if(SelectionAnchorRowHandle != InvalidRowHandle) {
					VisualSelectRowsCore(SelectionAnchor, end, false, true);
					return;
				}
				if(end.RowHandle == start.RowHandle) 
					VisualSelectRowsCore(start, end, true, true);
				else
					SelectFocusedCellCore(e);
			} else {
				SelectFocusedCellCore(e);
			}
		}
		internal void SelectFocusedCellCore() { SelectFocusedCellCore(new KeyEventArgs(Control.ModifierKeys)); }
		protected internal virtual void SelectFocusedCellCore(KeyEventArgs e) {
			if(!IsCellSelect || e.Control) return;
			if(!AllowChangeSelectionOnNavigation && e.KeyCode != Keys.Tab) return;
			if(!e.Shift || e.KeyCode == Keys.Tab) SmartClearSelection();
			if(IsGroupRow(FocusedRowHandle)) {
				base.SelectFocusedRowCore();
				return;
			}
			SelectFocusedCell();
		}
		void SmartClearSelection() {
			if(this.lockSelectionInvalidate) {
				ClearSelectionCore();
				return;
			}
			bool[] old = GetVisibleSelectionInfo();
			this.lockSelectionInvalidate = true;
			try {
				ClearSelectionCore();
			}
			finally {
				this.lockSelectionInvalidate = false;
			}
			InvalidateVisibleSelectionChanges(old);
		}
		protected override void OnSelectionAnchorChanged() { 
			if(SelectionAnchorRowHandle == InvalidRowHandle)
				SetSelectionBounds(InvalidRowHandle, InvalidRowHandle, 0, 0);
			else {
				int index = SelectionAnchor.Column == null ? 0 : SelectionAnchor.Column.VisibleIndex;
				SetSelectionBounds(index, SelectionAnchorRowHandle, index, SelectionAnchorRowHandle);
			}
			base.OnSelectionAnchorChanged();
		}
		public virtual void SelectCellAnchorRange(int startRowHandle, GridColumn startColumn, int endRowHandle, GridColumn endColumn) {
			SelectCellAnchorRange(new GridCell(startRowHandle, startColumn), new GridCell(endRowHandle, endColumn));
		}
		public virtual void SelectCellAnchorRange(GridCell start, GridCell end) {
			VisualSelectRowsCore(start, end, true, true);
		}
		protected override void DoAfterFocusedColumnChanged(GridColumn prevFocusedColumn, GridColumn focusedColumn) {
			if(focusedColumn != null && !IsLockUpdate)
				MakeColumnVisible(focusedColumn);
			if(prevFocusedColumn != null && prevFocusedColumn.View != null)
				UpdateCellViewInfo(FocusedRowHandle, prevFocusedColumn, BaseViewInfo.EmptyPoint);
			UpdateCellViewInfo(FocusedRowHandle, focusedColumn, BaseViewInfo.EmptyPoint);
			UpdateRowViewInfo(FocusedRowHandle);
			base.DoAfterFocusedColumnChanged(prevFocusedColumn, focusedColumn);
			if((Control.ModifierKeys & Keys.Shift) == 0) SelectionAnchorRowHandle = InvalidRowHandle;
		}
		protected override void DoAfterMoveFocusedRowColumn(Keys byKey, int prevFocusedHandle, GridColumn prevFocusedColumn) {
			if(!IsCellSelect) return;
			bool shiftPressed = (Control.ModifierKeys & Keys.Shift) != 0,
				controlPressed = (Control.ModifierKeys & Keys.Control) != 0;
			if(byKey == Keys.LButton) {
				if(!shiftPressed) {
					if(!controlPressed) SelectFocusedCellCore();
					return;
				}
				if(SelectionAnchorRowHandle == InvalidRowHandle)
					SetSelectionAnchor(prevFocusedHandle, prevFocusedColumn);
				SelectAnchorRangeCore(controlPressed, true);
			}
		}
		protected override bool IsFocusedRowSelectedCore() { 
			if(IsCellSelect) return IsCellSelected(FocusedRowHandle, FocusedColumn);
			return base.IsFocusedRowSelectedCore();
		}
		protected void ValidateIndexes(GridCell start, GridCell end, out int startIndex, out int colStart, out int endIndex, out int colEnd) {
			colStart = colEnd = endIndex = startIndex = -1;
			if(start == null || end == null || !start.IsColumnValid || !end.IsColumnValid) return;
			startIndex = GetVisibleIndex(start.RowHandle);
			endIndex = GetVisibleIndex(end.RowHandle);
			colStart = start.Column.VisibleIndex;
			colEnd = end.Column.VisibleIndex;
			if(colStart > colEnd) {
				int a = colEnd; colEnd = colStart; colStart = a;
			}
			if(startIndex > endIndex) {
				int a = endIndex;
				endIndex = startIndex;
				startIndex = a;
			} 
		}
		protected internal override void InitDialogFormProperties(XtraForm form) {
			base.InitDialogFormProperties(form);
			form.FormBorderEffect = this.OptionsMenu.DialogFormBorderEffect;
		}
		class ColumnIndexesComparer : IComparer {
			int IComparer.Compare(object a, object b) {
				GridColumn c1 = (GridColumn)a, c2 = (GridColumn)b;
				return Comparer.Default.Compare(c1.VisibleIndex, c2.VisibleIndex);
			}
		}
		ColumnIndexesComparer indexesComparer = new ColumnIndexesComparer();
		public virtual GridColumn[] GetSelectedCells(int rowHandle) {
			if(!IsCellSelect) return new GridColumn[] { };
			GridRowSelectionInfo info = GetSelectionInfo(rowHandle);
			if(info == null || info.IsEmpty) return new GridColumn[] { };
			GridColumn[] cols = info.GetColumns();
			if(cols.Length > 1)
				Array.Sort(cols, indexesComparer);
			return cols;
		}
		public virtual GridCell[] GetSelectedCells() {
			if(!IsCellSelect) { 
				return new GridCell[] { };
			}
			ArrayList list = new ArrayList();
			int[] rows = GetSelectedRows();
			if(rows.Length == 0) return new GridCell[] { };
			for(int n = 0; n < rows.Length; n++) {
				GridColumn[] cols = GetSelectedCells(rows[n]);
				for(int c = 0; c < cols.Length; c++) {
					list.Add(new GridCell(rows[n], cols[c]));
				}
			}
			return list.ToArray(typeof(GridCell)) as GridCell[];
		}
		public void SelectRows(int startRowHandle, int endRowHandle) { SelectRange(startRowHandle, endRowHandle); }
		public void SelectCells(GridCell start, GridCell end) { SetCellSelection(start, end, true);	}
		public void UnSelectCells(GridCell start, GridCell end) { SetCellSelection(start, end, false);	}
		public void UnSelectCells(int startRowHandle, GridColumn startColumn, int endRowHandle, GridColumn endColumn) {
			UnSelectCells(new GridCell(startRowHandle, startColumn), new GridCell(endRowHandle, endColumn));
		}
		public void SelectCells(int startRowHandle, GridColumn startColumn, int endRowHandle, GridColumn endColumn) {
			SelectCells(new GridCell(startRowHandle, startColumn), new GridCell(endRowHandle, endColumn));
		}
		protected override bool CompareSelection(object prev, object current) {
			if(!IsCellSelect) return base.CompareSelection(prev, current);
			GridCell[] rows1 = prev as GridCell[];
			GridCell[] rows2 = current as GridCell[];
			if(rows1 == rows2) return true;
			if(rows1 == null) return false;
			if(rows2 == null) return false;
			if(rows1.Length != rows2.Length) return false;
			for(int n = 0; n < rows1.Length; n++) {
				GridCell c1 = rows1[n];
				GridCell c2 = rows2[n];
				if(c1.RowHandle != c2.RowHandle) return false;
				if(c1.Column != c2.Column) return false;
			}
			return true;
		}
		protected override object GetCurrentSelection() {
			if(!IsCellSelect) return base.GetCurrentSelection();
			return GetSelectedCells();
		}
		protected virtual void SetCellSelection(GridCell start, GridCell end, bool setSelected) {
			if(start == null || end == null || !start.IsColumnValid || !end.IsColumnValid) return;
			int startIndex, endIndex, colStart, colEnd;
			ValidateIndexes(start, end, out startIndex, out colStart, out endIndex, out colEnd);
			if(startIndex < 0 || endIndex < 0) return;
			BeginSelection();
			try {
				for(int n = startIndex; n < endIndex + 1; n++) {
					for(int c = colStart; c < colEnd + 1; c++) {
						GridColumn col = VisibleColumns[c];
						int row = GetVisibleRowHandle(n);
						if(col != null && IsValidRowHandle(row)) {
							if(setSelected)
								SelectCell(row, col);
							else
								UnselectCell(row, col);
						}
					}
				}
			} finally {
				EndSelection();
			}
		}
		protected virtual void SelectCellAnchorRangeCore(GridCell start, GridCell end, bool clearSelection, Rectangle prevSelection) {
			if(start == null || end == null || !start.IsColumnValid || !end.IsColumnValid) {
				if(clearSelection) ClearSelectionCore();
				return;
			}
			SetSelectionBounds(start.Column.VisibleIndex, start.RowHandle, end.Column.VisibleIndex, end.RowHandle);
			int startIndex, endIndex, colStart, colEnd;
			ValidateIndexes(start, end, out startIndex, out colStart, out endIndex, out colEnd);
			if(startIndex < 0 || endIndex < 0) {
				if(clearSelection) ClearSelectionCore();
				return;
			}
			BeginSelection();
			try {
				if(prevSelection.X != InvalidRowHandle) {
					if(SelectionBounds.IntersectsWith(prevSelection)) {
						UnselectCellsBlock(prevSelection, SelectionBounds);
						clearSelection = false;
					} 
				}
				if(clearSelection) ClearSelectionCore();
				SelectCell(start); 
				for(int n = startIndex; n < endIndex + 1; n++) {
					if(prevSelection.Y != InvalidRowHandle && prevSelection.Contains(colStart, n) && prevSelection.Contains(colEnd, n)) continue;
					int rowHandle = GetVisibleRowHandle(n);
					GridRowSelectionInfo info = GetSelectionInfo(rowHandle);
					bool exists = info != null;
					bool changed = false;
					if(info == null) info = new GridRowSelectionInfo();
					for(int c = colStart; c < colEnd + 1; c++) {
						if(prevSelection.Y != InvalidRowHandle && prevSelection.Contains(c, n)) continue;
						GridColumn col = VisibleColumns[c];
						if(col != null) {
							changed = true;
							info.AddReference(col);
							DataController.Selection.RaiseChanged();
						}
					}
					if(!exists && changed) {
						DataController.Selection.SetSelected(rowHandle, true, info);
					}
				}
			}
			finally {
				EndSelection();
			}
		}
		protected void UnselectCellsBlock(Rectangle prevBlock, Rectangle newBlock) {
			Point i1 = new Point(prevBlock.X, newBlock.X);
			Point i2 = new Point(newBlock.Right, prevBlock.Right);
			Point r1 = new Point(prevBlock.X, prevBlock.Right);
			Point rows = new Point(prevBlock.Y, prevBlock.Y + prevBlock.Height);
			bool requireUnselectCells = i1.X < i1.Y || i2.X < i2.Y,
				requireUnselectRows = r1.X < r1.Y;
			int c;
			for(int n = rows.X; n < rows.Y; n++) {
				int row = GetVisibleRowHandle(n);
				if(n < newBlock.Y || n >= newBlock.Bottom) {
					if(!requireUnselectRows || GetSelectionInfo(row) == null) continue;
					for(c = r1.X; c < r1.Y; c++) UnselectCellRelease(row, VisibleColumns[c]);
				} else {
					if(!requireUnselectCells || GetSelectionInfo(row) == null) continue;
					for(c = i1.X; c < i1.Y; c++) UnselectCellRelease(row, VisibleColumns[c]);
					for(c = i2.X; c < i2.Y; c++) UnselectCellRelease(row, VisibleColumns[c]);
				}
			}
		}
		void UnselectRowRelease(int rowHandle) {
			GridRowSelectionInfo info = GetSelectionInfo(rowHandle);
			if(info == null) return;
			info.ReleaseRow();
			if(info.IsEmpty) UnselectRow(rowHandle);
		}
		void UnselectCellRelease(int rowHandle, GridColumn column) {
			GridRowSelectionInfo info = GetSelectionInfo(rowHandle);
			if(info == null) return;
			info.Release(column);
			if(info.IsEmpty)
				UnselectRow(rowHandle);
			else
				DataController.Selection.RaiseChanged();
		}
		protected internal void SelectFocusedCell() {
			if(IsGroupRow(FocusedRowHandle)) {
				base.SelectFocusedRowCore();
				return;
			}
			SelectCell(FocusedRowHandle, FocusedColumn);
		}
		public void SelectCell(GridCell cell) {
			if(cell == null) return;
			SelectCell(cell.RowHandle, cell.Column);
		}
		public virtual void SelectCell(int rowHandle, GridColumn column) {
			if(column == null || !IsCellSelect) return;
			GridRowSelectionInfo info = GetSelectionInfo(rowHandle);
			if(info == null) 
				info = new GridRowSelectionInfo(column);
			else {
				if(info.Contains(column)) return;
				info.Add(column);
				DataController.Selection.SetActuallyChanged();
			}
			SetCellSelection(rowHandle, info);
		}
		protected virtual void SetCellSelection(int rowHandle, GridRowSelectionInfo info) {
			GridRowSelectionInfo current = GetSelectionInfo(rowHandle);
			if(current == null || current.Count != info.Count) {
				DataController.Selection.SetSelected(rowHandle, true, info);
				return;
			}
			OnDataController_SelectionChanged(this, new SelectionChangedEventArgs(CollectionChangeAction.Add, rowHandle));
		}
		Rectangle selectionBounds = new Rectangle(InvalidRowHandle, InvalidRowHandle, 0, 0);
		protected Rectangle SelectionBounds {
			get { return selectionBounds; }
		}
		protected void SetSelectionBounds(int colIndex1, int rowHandle1, int colIndex2, int rowHandle2) {
			if(colIndex1 > colIndex2) {
				int a = colIndex1; colIndex1 = colIndex2; colIndex2 = a;
			}
			int r1 = GetVisibleIndex(rowHandle1), r2 = GetVisibleIndex(rowHandle2);
			if(r1 != InvalidRowHandle) {
				if(r1 > r2) { 
					int a = r1; r1 = r2; r2 = a;
				}
			}
			rowHandle1 = r1; rowHandle2 = r2;
			selectionBounds = new Rectangle(colIndex1, rowHandle1, (colIndex2 - colIndex1) + 1, (rowHandle2 - rowHandle1) + 1);
		}
		protected virtual void VisualSelectRowsCore(GridCell start, GridCell end, bool controlPressed, bool cells) {
			if(!IsMultiSelect) return;
			if(!IsValidRowHandle(start.RowHandle)) return;
			bool[] oldRows = null;
			Rectangle prevSelection = SelectionBounds;
			if(ViewInfo.RowsInfo != null && ViewInfo.RowsInfo.Count > 0) {
				oldRows = GetVisibleSelectionInfo();
			}
			BeginSelection();
			try {
				if(!IsCellSelect || !cells) {
					if(!controlPressed && AllowChangeSelectionOnNavigation) ClearSelectionCore();
					SelectRow(start.RowHandle);
					SelectRange(start.RowHandle, end.RowHandle);
					if(CheckboxSelectorColumn != null) CheckCheckboxSelectorGroupSelection(start.RowHandle, true); 
				} else {
					SelectCellAnchorRangeCore(start, end, !controlPressed, prevSelection);
				}
			} finally {
				EndSelection();
			}
			if(oldRows != null) InvalidateVisibleSelectionChanges(oldRows);
			else InvalidateSelection(new SelectionChangedEventArgs(CollectionChangeAction.Refresh, InvalidRowHandle));
		}
		protected override void SelectAnchorRangeCore(bool controlPressed, bool allowCells) {
			VisualSelectRowsCore(SelectionAnchor, new GridCell(GetSelectionRowHandle(FocusedRowHandle), FocusedColumn), controlPressed, allowCells);
		}
		public override void SelectAll() {
			if(!IsCellSelect) {
				base.SelectAll();
				return;
			}
			BeginSelection();
			try {
				ClearSelectionCore();
				for(int n = 0; n < RowCount; n++) {
					SelectRow(GetVisibleRowHandle(n));
				}
			}
			finally {
				EndSelection();
			}
		}
		public override void SelectRow(int rowHandle) {
			GridRowSelectionInfo info;
			if(IsCellSelect) {
				if(IsGroupRow(rowHandle)) {
					info = new GridRowSelectionInfo();
					if(FocusedColumn == null)
						info.AddRange(GroupedColumns);
					else
						info.Add(FocusedColumn);
					SetCellSelection(rowHandle, info);
					return;
				}
				info = new GridRowSelectionInfo();
				info.AddRange(VisibleColumns);
				SetCellSelection(rowHandle, info);
				return;
			}
			base.SelectRow(rowHandle);
		}
		public virtual void UnselectCell(int rowHandle, GridColumn column) {
			if(!IsCellSelect) return;
			GridRowSelectionInfo info = GetSelectionInfo(rowHandle);
			if(info == null) return;
			if(!info.Remove(column)) return;
			DataController.Selection.SetActuallyChanged();
			if(info.IsEmpty) {
				DataController.Selection.SetSelected(rowHandle, false);
				return;
			}
			OnDataController_SelectionChanged(this, new SelectionChangedEventArgs(CollectionChangeAction.Refresh, rowHandle));
		}
		public void UnselectCell(GridCell cell) {
			if(cell == null) return;
			UnselectCell(cell.RowHandle, cell.Column);
		}
		public bool IsCellSelected(GridCell cell) {
			if(cell == null) return false;
			return IsCellSelected(cell.RowHandle, cell.Column);
		}
		public virtual bool IsCellSelected(int rowHandle, GridColumn column) {
			if(!IsCellSelect || column == null) return false;
			GridRowSelectionInfo info = GetSelectionInfo(rowHandle);
			if(info == null) return false;
			return info.Contains(column);
		}
		protected virtual GridRowSelectionInfo GetSelectionInfo(int rowHandle) {
			return DataController.Selection.GetSelectedObject(rowHandle) as GridRowSelectionInfo;
		}
		protected override bool IsAutoHeight { get { return OptionsView.RowAutoHeight; } }
		protected override void OnGridControlChanged(GridControl prevControl) {
			base.OnGridControlChanged(prevControl);
			if(ScrollInfo != null) {
				ScrollInfo.RemoveControls(prevControl);
				ScrollInfo.AddControls(GridControl);
			}
		}
		protected internal virtual bool AllowScrollAutoWidth { get { return true; } }
		protected Timer ScrollTimer { get { return scrollTimer; } }
		protected virtual void OnScrollTimer_Tick(object sender, EventArgs e) {
			if(State != GridState.Selection && State != GridState.CellSelection) {
				ScrollTimer.Enabled = false;
				return;
			}
			Rectangle r = ViewInfo.ViewRects.Rows;
			Point p = PointToClient(Control.MousePosition);
			int vScrollDelta = 0, hScrollDelta = 0;
			if(p.Y < r.Top + 10) {
				vScrollDelta = -1;
			} 
			if(p.Y > r.Bottom - 10) {
				vScrollDelta = 1;
			}
			if(IsCellSelect && !OptionsView.ColumnAutoWidth) {
				if(p.X <= r.Left && State != GridState.Selection) hScrollDelta = -10;
				if(p.X >= r.Right) hScrollDelta = 10;
			}
			int prevIndex = TopRowIndex;
			TopRowIndex += vScrollDelta;
			LeftCoord += hScrollDelta;
			if(prevIndex != TopRowIndex && ViewInfo.RowsInfo.Count > 0) {
				if(vScrollDelta == -1) 
					UpdateAccessSelection(ViewInfo.RowsInfo[0].RowHandle, ViewInfo.GetNearestColumn(p));
				else {
					UpdateAccessSelection(ViewInfo.RowsInfo[ViewInfo.RowsInfo.Count - 1].RowHandle, ViewInfo.GetNearestColumn(p));
				}
			}
		}
		protected internal virtual bool GetShowVerticalLines() {
			if(OptionsView.ShowVerticalLines == DefaultBoolean.Default && ViewInfo != null) return ViewInfo.GetDefaultShowVerticalLines();
			return OptionsView.ShowVerticalLines != DefaultBoolean.False;
		}
		protected internal virtual bool GetShowPreviewRowLines() {
			if(OptionsView.ShowPreviewRowLines == DefaultBoolean.Default && ViewInfo != null) return ViewInfo.GetDefaultShowPreviewRowLines();
			return OptionsView.ShowPreviewRowLines != DefaultBoolean.False;
		}
		protected internal virtual bool GetShowHorizontalLines() {
			if(OptionsView.ShowHorizontalLines == DefaultBoolean.Default && ViewInfo != null) return ViewInfo.GetDefaultShowHorizontalLines();
			return OptionsView.ShowHorizontalLines != DefaultBoolean.False; 
		}
		protected override void SetFocusedRowModifiedCore(bool modified) {
			base.SetFocusedRowModifiedCore(modified);
			InvalidateRow(FocusedRowHandle);
		}
		protected internal override bool ValidateEditing() {
			if(IsInplaceEditFormVisible) {
				if(!CheckCloseEditForm()) return false;
				return EndEditOnLeave();
			}
			return base.ValidateEditing();
		}
		public override EditorShowMode GetShowEditorMode() {
			EditorShowMode res = OptionsBehavior.EditorShowMode;
			if(res == EditorShowMode.Default) {
				if(OptionsSelection.MultiSelect && OptionsSelection.MultiSelectMode == GridMultiSelectMode.CheckBoxRowSelect) return EditorShowMode.MouseUp;
				res = IsCellSelect ? EditorShowMode.Click : EditorShowMode.MouseDown;
			}
			return res;
		}
		protected internal virtual void StartAccessSelectionCore(int rowHandle, GridColumn column, bool indicator) {
			SetDefaultState();
			if(!IsDefaultState) return;
			if(IsFilterRow(rowHandle)) return;
			if(IsNewItemRow(rowHandle)) {
				ClearSelection();
				return;
			}
			if(indicator && !OptionsSelection.UseIndicatorForSelection) return;
			try {
				BeginSelection();
				if(indicator) {
					ClearSelectionCore();
				}
				if(DataRowCount == 0 || rowHandle == InvalidRowHandle) return;
				if(IsCellSelect && !indicator) 
					SelectCell(rowHandle, column);
				else
					SelectRow(rowHandle);
				if(CheckboxSelectorColumn != null) CheckCheckboxSelectorGroupSelection(rowHandle, true);
			} finally {
				EndSelection();
			}
			SetSelectionAnchor(rowHandle, column);
			SetStateCore(indicator || !IsCellSelect ? GridState.Selection : GridState.CellSelection);
			if((Control.MouseButtons & MouseButtons.Left) == 0 || !GridControl.Capture) {
				SetDefaultState();
				return;
			}
		}
		protected internal virtual void StartAccessSelection() {
			StartAccessSelectionCore(FocusedRowHandle, VisibleColumns.Count > 0 ? VisibleColumns[0] : null, true);
		}
		protected internal virtual void EndAccessSelection() {
			SelectionAnchorRowHandle = InvalidRowHandle;
			if(ScrollTimer != null) ScrollTimer.Enabled = false;
		}
		protected internal override void OnColumnSummaryCollectionChanged(GridColumn column, CollectionChangeEventArgs e) {
			if(IsDeserializing) return;
			TotalSummary.SetDirty();
			OnSummaryCollectionChanged(TotalSummary, e);
		}
		protected virtual void OnSummaryCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(IsLoading) return;
			SynchronizeSummary();
			if(IsLevelDefault && CanSynchronized) OnViewPropertiesChanged(SynchronizationMode.Data);
		}
		public void BeginSummaryUpdate() {
			this.lockSummary++;
		}
		public void EndSummaryUpdate() {
			if(--this.lockSummary == 0)
				SynchronizeSummary();
		}
		protected override void InternalSynchronizeGroupSummary() {
			if(lockSummary != 0) return;
			if(IsNeedUpdate(GroupSummary, DataController.GroupSummary, GroupSummary.ActiveCount)) {
				SynchronizeSummary(GroupSummary, DataController.GroupSummary);
				DataController.UpdateGroupSummary();
				SetLayoutDirty();
				if(CanSynchronized) OnViewPropertiesChanged(SynchronizationMode.Data);
			}
		}
		protected internal virtual void SynchronizeSummary() {
			if(lockSummary != 0) return;
			bool isNeedUpdate = false;
			TotalSummary.CheckDirty();
			List<SummaryItem> changed = null;
			if(IsNeedUpdate(TotalSummary, DataController.TotalSummary, TotalSummary.ActiveCount)) {
				changed = SynchronizeSummary(TotalSummary, DataController.TotalSummary);
				isNeedUpdate = true;
			}
			isNeedUpdate |= CheckUpdateFormatRuleSummary(ref changed);
			if(changed != null && changed.Count > 0) {
				DataController.UpdateTotalSummary(changed);
				isNeedUpdate = true;
				FormatRules.TryUpdateStateValues();
			}
			if(IsNeedUpdate(GroupSummary, DataController.GroupSummary, GroupSummary.ActiveCount)) {
				changed = SynchronizeSummary(GroupSummary, DataController.GroupSummary);
				isNeedUpdate = true;
				DataController.UpdateGroupSummary(changed);
			}
			if(isNeedUpdate) {
				SetLayoutDirty();
				ViewInfo.IsReady = false;
				if(CanSynchronized) OnViewPropertiesChanged(SynchronizationMode.Data);
			}
			else {
				LayoutChangedSynchronized();
			}
		}
		protected override void SyncFormatRulesSummary() {
		}
		protected override void UpdateFormatRulesSummary() {
			SynchronizeSummary();
			FormatRules.TryUpdateStateValues();
		}
		bool CheckUpdateFormatRuleSummary(ref List<SummaryItem> changed) {
			bool summaryChanged = false;
			DataController.TotalSummary.BeginUpdate();
			try {
				SummaryItem[] changedItems;
				summaryChanged = FormatRules.SummaryInfo.Apply(DataController, out changedItems);
				if(changedItems.Length > 0) {
					if(changed == null) changed = new List<SummaryItem>();
					changed.AddRange(changedItems);
				}
			}
			finally {
				DataController.TotalSummary.CancelUpdate();
			}
			return summaryChanged;
		}
		protected virtual List<SummaryItem> SynchronizeSummary(IList gridSum, SummaryItemCollection summary) {
			List<SummaryItem> changedItems = new List<SummaryItem>();
			if(!DataController.IsReady) return changedItems;
			summary.BeginUpdate();
			try {
				for(int n = gridSum.Count - 1; n >= 0; n--) {
					GridSummaryItem item = gridSum[n] as GridSummaryItem;
					if(item.SummaryItem != null && !summary.Contains(item.SummaryItem)) item.SummaryItem = null;
				}
				for(int n = summary.Count - 1; n >= 0; n--) {
					if(((IGridSummaryFind)gridSum).FindSummaryItem(summary[n]) == null) summary.RemoveAt(n);
				}
				for(int n = 0; n < gridSum.Count; n++) {
					GridSummaryItem sItem = gridSum[n] as GridSummaryItem;
					if(sItem.SummaryItem == null) {
						var item = new SummaryItem(DataController.Columns[sItem.FieldName], sItem.SummaryType);
						changedItems.Add(item);
						sItem.SummaryItem = summary.Add(item);
						sItem.SummaryItem.Tag = sItem;
					}
					else {
						if(!sItem.EqualsSummaryItem() && sItem.SummaryType != SummaryItemType.None) changedItems.Add(sItem.SummaryItem);
						sItem.AssignSummaryItem();
					}
				}
			}
			finally {
				summary.CancelUpdate();
			}
			return changedItems;
		}
		protected internal virtual bool IsNeedUpdate(IList gridSum, SummaryItemCollection summary, int activeCount) {
			if(!DataController.IsReady) return false;
			List<SummaryItem> items = summary.GetSummaryItemByTagType(typeof(GridSummaryItem));
			if(activeCount != SummaryItemCollection.GetActiveCount(items)) return true;
			if(gridSum.Count == 0) return false;
			foreach(GridSummaryItem gItem in gridSum) {
				SummaryItem sItem = summary.GetSummaryItemByTag(gItem);
				if(sItem == null && gItem.SummaryType == SummaryItemType.None) continue;
				if(sItem == null || gItem.SummaryItem != sItem || !gItem.IsEqualsSummaryItem(sItem)) return true;
			}
			return false;
		}
		public void UpdateSummary() {
			DataController.CalcSummary();
			LayoutChanged();
		}
		public void UpdateTotalSummary() {
			DataController.UpdateTotalSummary();
			ViewInfo.UpdateFooterDrawInfo();
			InvalidateFooter();
		}
		public void UpdateGroupSummary() {
			DataController.UpdateGroupSummary();
			LayoutChanged();
		}
		protected internal virtual void BeginNewItemRowEditing() {
			if(ActiveEditor != null) {
				try {
					fAllowCloseEditor = false;
					this.fEditingCell = null;
					DataController.AddNewRow();
				}
				finally {
					fAllowCloseEditor = true;
				}
				DataController.BeginCurrentRowEdit();
				this.fEditingCell = ViewInfo.GetGridCellInfo(FocusedRowHandle, FocusedColumn);
				if(this.fEditingCell != null) {
					ActiveEditor.Location = this.fEditingCell.CellValueRect.Location;
					ActiveEditor.Size = this.fEditingCell.CellValueRect.Size;
				}
				RefreshRow(FocusedRowHandle, false, false);
				SetFocusedRowModified();
			} else {
				DataController.AddNewRow();
				DataController.BeginCurrentRowEdit();
				SetFocusedRowModified();
			}
			if(IsEditFormMode && EditFormController != null) EditFormController.RefreshValues();
		}
		protected override void OnActiveEditor_ValueModified(object sender, EventArgs e) {
			if(this.EditingValueModified) {
				if(IsNewItemRow(FocusedRowHandle) && !DataController.IsNewItemRowEditing) {
					BeginNewItemRowEditing();
				}
			}
			InvalidateRow(FocusedRowHandle); 
			base.OnActiveEditor_ValueModified(sender, e);
		}
		public override int GetVisibleIndex(int rowHandle) {
			if(rowHandle == CurrencyDataController.NewItemRow) {
				if(OptionsView.NewItemRowPosition == NewItemRowPosition.Top) return -1;
				if(OptionsView.NewItemRowPosition == NewItemRowPosition.Bottom) return RowCount - 1;
				if(OptionsView.NewItemRowPosition == NewItemRowPosition.None && DataController.IsNewItemRowEditing) return RowCount - 1;
			}
			return base.GetVisibleIndex(rowHandle);
		}
		public override int GetVisibleRowHandle(int rowVisibleIndex) {
			if(rowVisibleIndex == -1 && OptionsView.NewItemRowPosition == NewItemRowPosition.Top) {
				return CurrencyDataController.NewItemRow;
			}
			if(rowVisibleIndex == RowCount - 1) {
				if(OptionsView.NewItemRowPosition == NewItemRowPosition.Bottom) return CurrencyDataController.NewItemRow;
				if(OptionsView.NewItemRowPosition == NewItemRowPosition.None && DataController.IsNewItemRowEditing) return CurrencyDataController.NewItemRow;
			}
			return base.GetVisibleRowHandle(rowVisibleIndex);
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridViewRowCount")]
#endif
		public override int RowCount {
			get {
				if(DesignMode) return base.RowCount;
				int res = base.RowCount;
				if(OptionsView.NewItemRowPosition == NewItemRowPosition.Bottom) res++;
				if(OptionsView.NewItemRowPosition == NewItemRowPosition.None && DataController.IsNewItemRowEditing) res++;
				return res;
			}
		}
		public override int GetNextVisibleRow(int rowVisibleIndex) {
			return base.GetNextVisibleRow(rowVisibleIndex);
		}
		protected internal override void OnColumnOptionsChanged(GridColumn column, BaseOptionChangedEventArgs e) {
			base.OnColumnOptionsChanged(column, e);
			if(IsDeserializing) return;
			if(FocusedColumn == column) FocusedColumn = GetNearestCanFocusedColumn(column, 0, false);
		}
		protected override void OnColumnAdded(GridColumn column) {
			base.OnColumnAdded(column);
			TotalSummary.SetDirty();
			if(DataController.IsReady) {
				OnSummaryCollectionChanged(TotalSummary, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
			}
		}
		protected override void OnColumnDeleted(GridColumn column) {
			if(IsCheckboxSelector(column)) this.checkboxSelectorColumn = null;
			BeginUpdate();
			try {
				base.OnColumnDeleted(column);
				if(ViewInfo != null && ViewInfo.IsReady) ViewInfo.Clear();
				TotalSummary.SetDirty();
				if(DataController.IsReady) {
					OnSummaryCollectionChanged(TotalSummary, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override void InitializeDataParameters() {
			base.InitializeDataParameters();
			TotalSummary.SetDirty();
		}
		protected internal override void SynchronizeLookDataControllerSettings() {
			SynchronizeSummary();
			base.SynchronizeLookDataControllerSettings();
		}
		protected override void UpdateDataControllerOptions() {
			base.UpdateDataControllerOptions();
			DataController.KeepGroupRowsExpandedOnRefresh = OptionsBehavior.KeepGroupExpandedOnSorting;
			DataController.AutoUpdateTotalSummary = OptionsBehavior.AutoUpdateTotalSummary;
			DataController.AutoExpandAllGroups = OptionsBehavior.AutoExpandAllGroups;
			DataController.SummariesIgnoreNullValues = OptionsBehavior.SummariesIgnoreNullValues;
			DataController.AllowPartialGrouping = AllowPartialGroupsBase;
		}
		protected override void InitializeDataController() {
			base.InitializeDataController();
			SynchronizeSummary();
		}
		protected  virtual void OnDataManager_CustomSummaryEvent(object sender, CustomSummaryEventArgs e) {
			CustomSummaryEventHandler handler = (CustomSummaryEventHandler)this.Events[customSummaryCalculate];
			if(handler != null) handler(this, e);
		}
		protected  virtual void OnDataManager_CustomSummaryExistEvent(object sender, CustomSummaryExistEventArgs e) {
			CustomSummaryExistEventHandler handler = (CustomSummaryExistEventHandler)this.Events[customSummaryExists];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnSummaryChanged(object sender, CollectionChangeEventArgs e) {
			FireChanged();
			BeginSynchronization();
			try {
				OnPropertiesChanged();
			}
			finally {
				EndSynchronization();
			}
		}
		internal void XtraClearGroupSummary(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null || e.Item.ChildProperties.Count == 0) {
				GroupSummary.Clear();
				return;
			}
			ArrayList list = new ArrayList();
			foreach(DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp in e.Item.ChildProperties) {
				object col = XtraFindGroupSummaryItem(new XtraItemEventArgs(this, GroupSummary, xp));
				if(col != null) list.Add(col);
			}
			for(int n = GroupSummary.Count - 1; n >= 0; n--) {
				GridSummaryItem item = GroupSummary[n];
				if(!list.Contains(item)) GroupSummary.RemoveAt(n);
			}
		}
		internal object XtraCreateGroupSummaryItem(XtraItemEventArgs e) { return GroupSummary.XtraCreateSummaryItem(e); }
		internal void XtraSetIndexGroupSummaryItem(XtraSetItemIndexEventArgs e) { GroupSummary.XtraSetIndexSummaryItem(e); }
		internal object XtraFindGroupSummaryItem(XtraItemEventArgs e) { return GroupSummary.XtraFindSummaryItem(e); }
		protected override void OnPopulateColumns() { 
			base.OnPopulateColumns();
			if(GroupSummary.Count > 0) {
				GroupSummary.Refresh();
				SynchronizeSummary();
			}
		}
		[Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), XtraSerializableProperty(true, true, true, 1000)]
		public virtual GridGroupSummaryItemCollection GroupSummary { get { return fGroupSummary; } }
		protected internal virtual GridTotalSummaryCollection TotalSummary { get { return totalSummary; } }
		protected virtual ScrollStyleFlags DefaultScrollStyle {
			get { return ScrollStyleFlags.LiveVertScroll | ScrollStyleFlags.LiveHorzScroll; }
		}
		protected virtual void UpdateRowViewInfo(int rowHandle) {
			if(IsLockUpdate) return;
			if(ViewDisposing) return;
			GridRowInfo ri = ViewInfo.GetGridRowInfo(rowHandle);
			if(ri == null) return;
			ViewInfo.UpdateRowData(ri, true, false);
		}
		protected virtual void UpdateCellViewInfo(int rowHandle, GridColumn column, Point mousePos) {
			if(IsLockUpdate) return;
			if(column == null || rowHandle == DevExpress.Data.DataController.InvalidRow) return;
			GridCellInfo cell = ViewInfo.GetGridCellInfo(rowHandle, column);
			if(cell == null) return;
			if(cell.ViewInfo != null && cell.ViewInfo is IAnimatedItem) 
				XtraAnimator.RemoveObject( ViewInfo, new GridCellId(this, cell));
			cell.ViewInfo = null;
			ViewInfo.RequestCellEditViewInfo(cell);
			ViewInfo.UpdateCellEditViewInfo(cell, mousePos, true, true);
			InvalidateRowCell(rowHandle, column);
		}
		public override void InvalidateHitObject(DevExpress.XtraGrid.Views.Base.ViewInfo.BaseHitInfo hit) {
			GridHitInfo hitInfo = hit as GridHitInfo;
			if(hitInfo.InGroupPanel) 
				InvalidateGroupPanel();
			if(hitInfo.InColumn) {
				InvalidateColumnHeader(hitInfo.Column);
				return;
			}
			if(hitInfo.HitTest == GridHitTest.ColumnButton) { 
				InvalidateColumnHeader(null);
				return;
			}
			if(hitInfo.InFilterPanel) {
				InvalidateFilterPanel();
				return;
			}
			if(hitInfo.InRowCell) {
				InvalidateRowCell(hitInfo.RowHandle, hitInfo.Column);
				return;
			}
		}
		protected override void OnRowMouseEnter(int rowHandle) {
			base.OnRowMouseEnter(rowHandle);
			if(IsDefaultState && WorkAsLookup) {
				if(this.firstMouseEnter) {
					this.firstMouseEnter = false;
					return;
				}
				if(IsDataRow(rowHandle)) {
					if(IsRowVisible(rowHandle) == RowVisibleState.Visible)
						FocusedRowHandle = rowHandle;
				}
			}
		}
		protected internal virtual void VisualExpandGroup(int rowHandle, bool expanded) {
			GridRowInfo row = ViewInfo.RowsInfo.FindRow(rowHandle);
			SetRowExpanded(rowHandle, expanded);
			if(!AllowFixedGroups || expanded) return;
			if(row != null && row.ForcedRow && DataController.IsRowVisible(row.RowHandle)) {
				MakeRowVisible(row.RowHandle, false);
			}
		}
		protected override int GetValidSelectionAnchor(int rowHandle) {
			return GetSelectionRowHandle(rowHandle);
		}
		protected virtual int GetSelectionRowHandle(int rowHandle) {
			if(IsFilterRow(rowHandle) || (IsNewItemRow(rowHandle) && OptionsView.NewItemRowPosition == NewItemRowPosition.Top)) {
				return GetVisibleRowHandle(TopRowIndex);
			}
			if(IsNewItemRow(rowHandle)) {
				return GetVisibleRowHandle(RowCount - 1);
			}
			return rowHandle;
		}
		protected internal virtual void UpdateAccessSelection(int rowHandle, GridColumn column) {
			rowHandle = GetSelectionRowHandle(rowHandle);
			try {
				ScrollTimer.Enabled = false;
				VisualSelectRowsCore(SelectionAnchor, new GridCell(rowHandle, column), false, IsCellSelect && State == GridState.CellSelection);
			}
			finally {
				ScrollTimer.Enabled = true;
			}
		}
		bool[] GetVisibleSelectionInfo() {
			bool[] rows = new bool[ViewInfo.RowsInfo.Count];
			for(int n = 0; n < ViewInfo.RowsInfo.Count; n++) {
				GridRowInfo row = ViewInfo.RowsInfo[n];
				rows[n] = IsRowSelected(row.RowHandle);
			}
			return rows;
		}
		void InvalidateVisibleSelectionChanges(bool[] prevRows) {
			bool[] newRows = GetVisibleSelectionInfo();
			int count = Math.Min(newRows.Length, prevRows.Length);
			for(int n = 0; n < count; n++) {
				if(newRows[n] != prevRows[n] || (IsCellSelect && prevRows[n])) 
					InvalidateSelectionRow(ViewInfo.RowsInfo[n].RowHandle);
			}
		}
		protected override void VisualClientUpdateRow(int controllerRowHandle) {
			GridRowInfo row = ViewInfo.GetGridRowInfo(controllerRowHandle) as GridRowInfo;
			GridDataRowInfo dataRow = row as GridDataRowInfo;
			GridGroupRowInfo groupRow = row as GridGroupRowInfo;
			if(dataRow != null && (dataRow.HasMergedCells || IsCellMerge || (OptionsView.ShowPreview && OptionsView.AutoCalcPreviewLineCount))) {
				SetLayoutDirty();
				return;
			}
			if(dataRow == null && OptionsView.NewItemRowPosition != NewItemRowPosition.None) {
				SetLayoutDirty();
				return;
			}
			if(GroupCount > 0 && GroupSummary.ActiveCount > 0) {
				SetLayoutDirty();
				return;
			}
			base.VisualClientUpdateRow(controllerRowHandle);
		}
		protected internal override Point GetCellPoint(object cellInfo, Point point, bool tooltip) {
			if(IsEditFormMode) {
				if(!tooltip || IsEditFormVisible)
					return new Point(int.MinValue, int.MinValue);
			}
			GridCellInfo cell = cellInfo as GridCellInfo;
			if(cell != null) point.Offset(-cell.CellValueRect.X, -cell.CellValueRect.Y);
			return point; 
		}
		protected override object GetCellInfo(BaseHitInfo hitInfo) { 
			GridHitInfo grid = hitInfo as GridHitInfo;
			return grid != null ? grid.CellInfo : null;
		}
		protected override BaseEditViewInfo GetCellEditInfo(object cellInfo) { 
			GridCellInfo cell = cellInfo as GridCellInfo;
			return cell != null ? cell.ViewInfo : null;
		}
		protected override void OnRowMouseLeave(int rowHandle) {
			base.OnRowMouseLeave(rowHandle);
		}
		protected override void OnHotTrackLeave(BaseHitInfo hitInfo) {
			GridHitInfo hit = hitInfo as GridHitInfo;
			bool needInvalidate = false;
			if(hit.InRowCell) OnCellMouseLeave(hitInfo);
			else 
				needInvalidate = true;
			if(needInvalidate) {
				InvalidateHitObject(hit);
			}
		}
		protected override void OnHotTrackEnter(BaseHitInfo hitInfo) {
			GridHitInfo hit = hitInfo as GridHitInfo;
			bool needInvalidate = false;
			if(hit.InRowCell) OnCellMouseEnter(hitInfo);
			else 
				needInvalidate = true;
			if(needInvalidate)
				InvalidateHitObject(hit);
		}
		protected internal override void OnLostCapture() {
			((GridHandler)Handler).DownPointHitInfo = null;
			if(State == GridState.Selection || State == GridState.CellSelection || IsSizingState) {
				SetDefaultState();
			}
			base.OnLostCapture();
		}
		protected internal virtual void KeyMoveFirst() {
			Scroller.OnMoveFirst();
		}
		protected internal virtual void KeyMoveLastVisible() {
			Scroller.OnMoveLastVisible();
		}
		protected internal override bool CanShowHint { get { return base.CanShowHint || IsScrolling; } }
		protected internal override bool CanIncrementalSearch(GridColumn column) {
			bool res = base.CanIncrementalSearch(column);
			if(res && OptionsBehavior.AllowIncrementalSearch) return true;
			return false;
		}
		protected internal override bool CanDataFilterColumn(GridColumn column) {
			if(column == CheckboxSelectorColumn) return false;
			return base.CanDataFilterColumn(column);
		}
		protected override ToolTipControlInfo GetToolTipObjectInfoCore(GraphicsCache cache, Point p) {
			if(IsScrolling) {
				return new ToolTipControlInfo("ViewScrollBar", GetVertScrollTip(ScrollInfo.VScrollPosition));
			}
			GridHitInfo ht = GetHintObjectInfo() as GridHitInfo;
			if(ht == null) return null;
			if(ht.InColumn) {
				if(GridControl.IsDesignMode) 
					return new ToolTipControlInfo(ht.Column, GetToolTipText(ht.Column, p));
				if(!OptionsHint.ShowColumnHeaderHints) 
					return null;
				if(ht.Column.ToolTip != string.Empty) 
					return new ToolTipControlInfo(ht.Column, GetToolTipText(ht.Column, p));
				if(!ht.Column.OptionsColumn.ShowCaption) 
					return null;
				GridColumnInfoArgs ci = ViewInfo.ColumnsInfo[ht.Column];
				if(ci != null) {
					string description = ht.Column.GetDescription();
					if(!string.IsNullOrEmpty(description) && string.IsNullOrEmpty(ht.Column.ToolTip))
						return new ToolTipControlInfo(ht.Column, description, ht.Column.GetCaption());
					bool fit = Painter.ElementsPainter.Column.IsCaptionFit(cache, ci);
					if(!fit)
						return new ToolTipControlInfo(ht.Column, GetToolTipText(ht.Column, p));
				}
			}
			if(ht.HitTest == GridHitTest.RowIndicator) {
				GridDataRowInfo ri = ViewInfo.GetGridRowInfo(ht.RowHandle) as GridDataRowInfo;
				if(ri != null && ri.ErrorText != null && ri.ErrorText.Length > 0) {
					ToolTipControlInfo res = new ToolTipControlInfo(string.Format("Indicator:{0}", ri.RowHandle), ri.ErrorText, true, ToolTipIconType.None);
					res.ToolTipImage = ri.ErrorIcon;
					return res;
				}
			}
			if(ht.InRowCell && ht.CellInfo != null) {
				ViewInfo.RequestCellEditViewInfo(ht.CellInfo);
				if(ht.CellInfo.ViewInfo != null) {
					Point cellPoint = GetCellPoint(ht.CellInfo, p, true);
					return GetCellEditToolTipInfo(ht.CellInfo.ViewInfo, cellPoint, ht.RowHandle, ht.Column);
				}
			}
			if((ht.HitTest == GridHitTest.Footer || ht.HitTest == GridHitTest.RowFooter) && ht.FooterCell != null && OptionsHint.ShowFooterHints) {
				FooterCellPainter painter = ht.HitTest == GridHitTest.Footer ? Painter.ElementsPainter.FooterCell : Painter.ElementsPainter.GroupFooterCell;
				if(!painter.IsCaptionFit(cache, ht.FooterCell)) {
					return new ToolTipControlInfo(ht.FooterCell, ht.FooterCell.DisplayText) { AllowHtmlText = DefaultBoolean.True };
				}
			}
			return null;
		}
		protected internal override string GetToolTipText(object hintObject, Point p) {
			if(IsScrolling) return GetVertScrollTip(ScrollInfo.VScrollPosition);
			GridColumn column = hintObject as GridColumn;
			if(column != null) {
				if(GridControl.IsDesignMode) {
					return GridDesignTimeHints.DTColumn;
				}
				return column.ToolTip != string.Empty ? column.ToolTip : column.GetTextCaption();
			}
			return ""; 
		}
		protected override BaseHitInfo GetHintObjectInfo() { 
			if(IsScrolling) {
				GridHitInfo hitInfo = new GridHitInfo();
				hitInfo.HitTest = GridHitTest.VScrollBar;
				return hitInfo;
			}
			return ViewInfo.SelectionInfo.HotTrackedInfo; 
		} 
		protected override bool UpdateCellHotInfo(BaseHitInfo hitInfo, Point hitPoint) {
			GridHitInfo hit = hitInfo as GridHitInfo;
			GridCellInfo cell = ViewInfo.GetGridCellInfo(hit);
			if(cell == null || cell.Column == null || cell.Column.View == null) return false;
			ViewInfo.RequestCellEditViewInfo(cell);
			if(cell.ViewInfo == null || cell.Editor == null || !cell.ViewInfo.IsRequiredUpdateOnMouseMove) return false;
			hitPoint = GetCellPoint(cell, hitPoint);
			if(cell.ViewInfo.UpdateObjectState(MouseButtons.None, hitPoint)) return true;
			return false;
		}
		protected internal override bool OnCheckHotTrackMouseMove(BaseHitInfo hitInfo) {
			GridHitInfo hit = hitInfo as GridHitInfo;
			if(hit.InRowCell) return OnCellMouseMove(hitInfo);
			return base.OnCheckHotTrackMouseMove(hitInfo);
		}
		protected internal virtual AppearanceObject GetRowCellStyle(int rowHandle, GridColumn column, GridRowCellState state, AppearanceObject appearance) {
			return RaiseGetRowCellStyle(rowHandle, column, state, appearance);
		}
		protected internal bool IsSameColumnEditor {
			get { return this.Events[customRowCellEdit] == null; }
		}
		protected internal override void RaiseCustomColumnDisplayText(CustomColumnDisplayTextEventArgs e) {
			base.RaiseCustomColumnDisplayText(e);
			if(e.Column == CheckboxSelectorColumn) {
				if(IsNewItemRow(e.RowHandle) || IsFilterRow(e.RowHandle)) e.DisplayText = "";
			}
		}
		protected internal override RepositoryItem GetRowCellRepositoryItem(int rowHandle, GridColumn column) {
			if(column == null) return null;
			if(IsFilterRow(rowHandle) || IsNewItemRow(rowHandle)) {
				if(column == CheckboxSelectorColumn) return GetColumnDefaultRepositoryItem(null);
			}
			DevExpress.XtraEditors.Repository.RepositoryItem ce = column.ColumnEdit;
			if(ce != null && ce.IsDisposed) ce = null;
			if(ce == null) ce = GetColumnDefaultRepositoryItem(column);
			if(IsFilterRow(rowHandle)) {
				if(IsColumnFieldNameSortGroupExist(column)) {
					ce = this.GetColumnFieldNameSortGroup(column).ColumnEdit;
					if(ce == null) ce = GetColumnDefaultRepositoryItem(this.GetColumnFieldNameSortGroup(column));
				}
				ce = GetFilterRowRepositoryItem(this.GetColumnFieldNameSortGroup(column), ce);
			}
			CustomRowCellEditEventHandler handler = (CustomRowCellEditEventHandler)this.Events[customRowCellEdit];
			if(handler != null) {
				CustomRowCellEditEventArgs e = new CustomRowCellEditEventArgs(rowHandle, column, ce);
				handler(this, e);
				if(e.RepositoryItem == null) e.RepositoryItem = ce;
				ce = e.RepositoryItem;
			}
			return ce;
		}
		protected virtual PropertyDescriptorCollection GetSourceProperties(object list) {
			if(list is IXtraList) return (list as IXtraList).GetColumns();
			if(list is ITypedList)
				return (list as ITypedList).GetItemProperties(null);
			IList l = list as IList;
			if(l != null && l.Count > 0) {
				PropertyDescriptorCollection col = TypeDescriptor.GetProperties(l[0]);
				if(col == null || col.Count == 0) return null;
				return col;
			}
			return null;
		}
		protected internal override void CheckInfo() {
			base.CheckInfo();
			if(GridControl == null) return;
			CollapseAllDetails();
			ScrollInfo.UpdateLookAndFeel(ElementsLookAndFeel);
			UpdateColumnsCustomization();
		}
		protected internal virtual GridView ChildGridView {
			get { 
				if(ChildGridLevelName == "" || GridControl == null) return null;
				GridLevelNode levelNode = GetLevelNode();
				if(levelNode == null || !levelNode.HasChildren) return null;
				GridLevelNode child = levelNode.Nodes[ChildGridLevelName];
				GridView res = child != null ? child.LevelTemplate as GridView : null;
				if(res != null && res.SynchronizeClones) return res;
				return null;
			}
		}
		protected internal virtual bool HasAsChild(GridView view) {
			if(view.ParentView == null || view.SourceView == null) return false;
			GridView gv = view;
			while(true) {
				GridView parent = gv.ParentView as GridView;
				if(parent == null || gv.SourceView == null) return false;
				if(parent.ChildGridView != gv.SourceView) return false;
				if(parent == this) return true;
				gv = parent;
			}
		}
		protected internal override void SetDefaultState() {
			SetState((int)GridState.Normal);
		}
		public override void ShowFilterPopup(GridColumn column) {
			GridColumnInfoArgs ci = ViewInfo.ColumnsInfo[column];
			if(ci == null) {
				ci = ViewInfo.GroupPanel.Rows.GetColumnInfo(column);
			}
			if(ci == null) return;
			Focus();
			ShowFilterPopup(ci);
		}
		protected internal void ShowFilterPopup(GridColumnInfoArgs ci) {
			if(ci == null || ci.Column == null) return;
			Rectangle bounds = ci.Bounds;
			if(OptionsView.GetHeaderFilterButtonShowMode() == FilterButtonShowMode.SmartTag) {
				DrawElementInfo info = ci.InnerElements.Find(typeof(DevExpress.XtraEditors.Drawing.GridFilterButtonInfoArgs));
				if(info != null) {
					if(!info.ElementInfo.Bounds.IsEmpty) {
						bounds = info.ElementInfo.Bounds;
					}
				}
			}
			ShowFilterPopup(ci.Column, bounds, GridControl, ci);
		}
		public override object GetFocusedValue() {
			if(IsGroupRow(FocusedRowHandle)) return GetGroupRowValue(FocusedRowHandle);
			return base.GetFocusedValue();
		}
		public override string GetFocusedDisplayText() {
			if(IsGroupRow(FocusedRowHandle)) return GetGroupRowDisplayText(FocusedRowHandle);
			return base.GetFocusedDisplayText();
		}
		public virtual object GetRowCellValue(int rowHandle, string fieldName, OperationCompleted completed) {
			if(IsFilterRow(rowHandle)) return GetFilterRowValue(fieldName);
			return DataController.GetRowValue(rowHandle, fieldName, completed);
		}
		public virtual object GetRowCellValue(int rowHandle, GridColumn column, OperationCompleted completed) {
			column = CheckColumn(column);
			if(column == null) return null;
			if(IsFilterRow(rowHandle)) return GetFilterRowValue(column);
			if(DesignMode) return GetDesignTimeRowCellValue(rowHandle, column);
			return DataController.GetRowValue(rowHandle, column.ColumnHandle, completed);
		}
		BaseImageLoader imageLoader;
		protected internal BaseImageLoader ImageLoader {
			get {
				if(imageLoader == null)
					imageLoader = new GridViewAsyncImageLoader(ViewInfo);
				return imageLoader;
			}
		}
		Hashtable imageInfoCache = new Hashtable();
		protected internal ImageLoadInfo GetImageLoadInfo(int rowHandle, string fieldName, Size maxSize, Size desiredSize, ImageLayoutMode mode) {
			GridViewImageInfoKey key = new GridViewImageInfoKey() { FieldName = fieldName, RowHandle = rowHandle };
			ImageLoadInfo info = GetImageLoadInfoCore(key);
			if(info != null) return info;
			info = CreateImageLoadInfo(rowHandle, fieldName, maxSize, desiredSize, mode);
			imageInfoCache.Add(key, info);
			return info;
		}
		protected internal ImageLoadInfo GetImageLoadInfo(GridCellInfo cell) {
			if(cell.ColumnInfo == null || cell.ColumnInfo.Column == null || cell.RowInfo == null) return null;
			return GetImageLoadInfoCore(cell.RowHandle, cell.Column.FieldName);
		}
		protected override void OnActiveFilterChanged(object sender, EventArgs e) {
			ResetThumbnailCache();
			base.OnActiveFilterChanged(sender, e);
		}
		public override void AddNewRow() {
			ResetThumbnailCache();
			base.AddNewRow();
		}
		protected override void DeleteRowCore(int rowHandle) {
			ResetThumbnailCache();
			base.DeleteRowCore(rowHandle);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetThumbnailCache() {
			ClearLoadInfoCache();
		}
		protected internal void ClearLoadInfoCache() {
			imageInfoCache.Clear();
		}
		protected internal override void OnColumnSortInfoCollectionChanged(CollectionChangeEventArgs e) {
			ResetThumbnailCache();
			base.OnColumnSortInfoCollectionChanged(e);
		}
		protected internal ImageLoadInfo GetImageLoadInfoCore(GridViewImageInfoKey key) {
			if(imageInfoCache.ContainsKey(key)) return (ImageLoadInfo)imageInfoCache[key];
			return null;
		}
		protected internal ImageLoadInfo GetImageLoadInfoCore(int rowHandle, string fieldName) {
			GridViewImageInfoKey key = new GridViewImageInfoKey() { FieldName = fieldName, RowHandle = rowHandle };
			return GetImageLoadInfoCore(key);
		}
		protected internal ImageLoadInfo CreateImageLoadInfo(int rowHandle, string fieldName, Size maxSize, Size desiredSize, ImageLayoutMode mode) {
			ImageContentAnimationType animationType = OptionsImageLoad.AnimationType;
			return new GridViewImageLoadInfo(ViewRowHandleToDataSourceIndex(rowHandle), rowHandle, fieldName, animationType, mode, maxSize, desiredSize);
		}
		public override object GetRowCellValue(int rowHandle, string fieldName) {
			return GetRowCellValue(rowHandle, fieldName, null);
		}
		public override object GetRowCellValue(int rowHandle, GridColumn column) {
			return GetRowCellValue(rowHandle, column, null);
		}
		protected override void SetDataBoundSelectedRowCore(int rowHandle, bool selected) {
			if(IsCheckboxSelectorMode) {
				SetRowCellValueCore(rowHandle, CheckboxSelectorColumn, selected, false);
			}
			else
				base.SetDataBoundSelectedRowCore(rowHandle, selected);
		}
		protected override void SetRowCellValueCore(int rowHandle, GridColumn column, object _value, bool fromEditor) {
			column = CheckColumn(column);
			if(column == null) return;
			if(IsFilterRow(rowHandle)) {
				SetFilterRowValue(column, _value);
				return;
			}
			ResetThumbnailCache();
			base.SetRowCellValueCore(rowHandle, column, _value, fromEditor);
			UpdateRowViewInfo(rowHandle);
			InvalidateRowCell(rowHandle, column);
		}
		protected override object GetSelectedData() { 
			int[] rows = GetSelectedRows();
			if(rows == null) return null;
			ArrayList list = new ArrayList();
			for(int n = 0; n < rows.Length; n++) {
				if(!IsGroupRow(rows[n])) {
					list.Add(GetRow(rows[n]));
				}
			}
			return list;
		}
		protected override string GetText() { 
			int[] rows = IsMultiSelect ? DataController.Selection.GetNormalizedSelectedRowsEx() : GetSelectedRows();
			if(rows == null) return string.Empty;
			int minGroupLevel = 100, dataLevel = 0;
			for(int n = 0; n < rows.Length; n++) {
				if(IsGroupRow(rows[n])) {
					int level = GetRowLevel(rows[n]);
					minGroupLevel = Math.Min(level, minGroupLevel);
					dataLevel = Math.Max(dataLevel, level);
				}
			}
			if(minGroupLevel == 100) {
				minGroupLevel = -1; dataLevel = 0; 
			} else {
				dataLevel = (dataLevel - minGroupLevel) + 1;
			}
			StringBuilder sb = new StringBuilder();
			GetHeadersText(sb);
			int maxCopyRowCount = GetMaxRowCopyCount(rows.Length);
			for(int n = 0; n < maxCopyRowCount; n++) {
				if(!GetRowText(sb, rows[n], minGroupLevel, dataLevel)) continue;
				if(maxCopyRowCount > 1) sb.Append(crlf);
			}
			return sb.ToString();
		}
		protected virtual void GetHeadersText(StringBuilder sb) {
			if(OptionsClipboard.CopyColumnHeaders == DefaultBoolean.False) return;
			if(!GetDataRowText(sb, InvalidRowHandle)) return;
			sb.Append(crlf);
		}
		protected virtual int GetMaxRowCopyCount(int rowsCount) {
			if(MaxRowCopyCount <= 0) return rowsCount;
			return Math.Min(MaxRowCopyCount, rowsCount);
		}
		protected virtual void AppendRowIndent(StringBuilder sb, int count) {
			for(int n = 0; n < count; n++) sb.Append('\t');
		}
		protected virtual bool GetRowText(StringBuilder sb, int rowHandle, int minGroupLevel, int dataLevel) {
			if(!IsValidRowHandle(rowHandle)) return false;
			if(IsGroupRow(rowHandle)) {
				AppendRowIndent(sb, GetRowLevel(rowHandle) - minGroupLevel);
				sb.Append(GetGroupRowDisplayText(rowHandle));
				return true;
			}
			AppendRowIndent(sb, dataLevel);
			return GetDataRowText(sb, rowHandle);
		}
		int GetAnySelectedDataRow() {
			int[] rows = GetSelectedRows();
			if(rows == null || rows.Length == 0) return InvalidRowHandle;
			for(int n = 0; n < rows.Length; n++) {
				if(rows[n] >= 0) return rows[n];
			}
			return rows[0];
		}
		protected virtual bool GetDataRowText(StringBuilder sb, int rowHandle) {
			GridRowSelectionInfo info = null;
			if(rowHandle == InvalidRowHandle) {
				info = GetSelectionInfo(GetAnySelectedDataRow());
			} else 
				info = GetSelectionInfo(rowHandle);
			GridColumnReadOnlyCollection columns = VisibleColumns;
			if(IsCellSelect) {
				columns = new GridColumnReadOnlyCollection(null);
				for(int n = 0; n < VisibleColumns.Count; n++) {
					if(info == null || !info.Contains(VisibleColumns[n])) continue;
					columns.AddCore(VisibleColumns[n]);
				}
			}
			bool containsData = false;
			for(int n = 0; n < columns.Count; n++) {
				string cellText = rowHandle == InvalidRowHandle ? columns[n].GetTextCaption() : GetRowCellDisplayText(rowHandle, columns[n]);
				if(cellText == null) cellText = string.Empty;
				if(cellText.Contains(Environment.NewLine))
					cellText = "\"" + cellText.Replace("\"", "\"\"").Replace(Environment.NewLine, "\x0A") + "\"";
				cellText = cellText.Replace("\t", " ");
				sb.Append(cellText);
				if(n < columns.Count - 1) sb.Append(columnSeparator);
				containsData = true;
			}
			return containsData;
		}
		protected virtual RepositoryItem GetFilterRowRepositoryItem(GridColumn column, RepositoryItem current) {
			if(current is RepositoryItemCheckEdit) return current;
			if(column.GetFilterMode() != ColumnFilterMode.DisplayText) {
				if(current is RepositoryItemImageComboBox) return current;
				if(current is RepositoryItemLookUpEditBase) return current;
				if(column.FilterFieldName != column.FieldName) {
					return GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(string));
				}
			} 
			else {
				return GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(string));
			}
			if(current is RepositoryItemDateEdit) 
				return IsServerMode ? current : GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(string));
			if((current is RepositoryItemButtonEdit)) return GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(string));
			return current;
		}
		protected override void OnActiveEditor_ValueChanged(object sender, EventArgs e) {
			if(IsFilterRow(FocusedRowHandle)) {
				OnFilterRowValueChanging(FocusedColumn, EditingValue);
				return;
			}
			base.OnActiveEditor_ValueChanged(sender, e);
		}
		protected virtual bool IsAllowImmediateUpdateAutoFilter(GridColumn column) {
			if(IsServerMode && GetRowCellRepositoryItem(GridControl.AutoFilterRowHandle, column) is RepositoryItemDateEdit) return false;
			return true;
		}
		protected virtual void OnFilterRowValueChanging(GridColumn column, object _value) {
			if(column == null) return;
			if(!column.OptionsFilter.ImmediateUpdateAutoFilter) return;
			if(!IsAllowImmediateUpdateAutoFilter(column)) return;
			try {
				this.postingEditorValue++;
			SetFilterRowValue(column, _value);
			} finally {
				this.postingEditorValue--;
			}
		}
		protected object GetFilterRowValue(string fieldName) {
			return GetFilterRowValue(Columns[fieldName]);
		}
		protected virtual object GetFilterRowValue(GridColumn column) {
			if(column == null || column == CheckboxSelectorColumn) return null;
			if(column.FilterInfo.Type == ColumnFilterType.AutoFilter ||
				column.FilterInfo.Type == ColumnFilterType.Value)
				return column.FilterInfo.Value;
			return null;
		}
		protected override bool AllowUpdateMRUFilters { get { return this.lockFilterSetRowValue == 0; } }
		protected virtual void UpdateActiveEditorBounds() {
			if(ActiveEditor == null || FocusedColumn == null) return;
			GridCellInfo info = ViewInfo.GetGridCellInfo(FocusedRowHandle, FocusedColumn);
			if(info == null) {
				HideEditor();
				return;
			}
			this.fEditingCell = info;
			Rectangle bounds = GetEditorBounds(info);
			if(bounds.IsEmpty) 
				HideEditor();
			else {
				if(ActiveEditor.Bounds == bounds) return;
				ActiveEditor.Bounds = bounds;
			}
		}
		protected override void UpdateEditorProperties(BaseEdit editor) { 
			if(IsFilterRow(FocusedRowHandle)) {
				editor.Properties.BeginUpdate();
				try {
					if(IsServerMode) editor.Properties.EditValueChangedFiringMode = EditValueChangedFiringMode.Buffered;
					editor.Properties.NullText = "";
					editor.Properties.ReadOnly = false;
					CheckEdit check = editor as CheckEdit;
					if(check != null) {
						check.Properties.AllowGrayed = true;
						check.Properties.ValueGrayed = null;
					}
					ImageComboBoxEdit image = editor as ImageComboBoxEdit;
					if(image != null) 
						image.Properties.Items.Insert(0, new ImageComboBoxItem("", null));
					TextEdit textEdit = editor as TextEdit;
					if(textEdit != null)
						textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
				} finally {
					editor.Properties.EndUpdate();
				}
			}
			base.UpdateEditorProperties(editor);
		}
		protected override void UpdateRowAutoHeight(int rowHandle) {
			if(!IsAutoHeight) return;
			if(!IsFilterRow(FocusedRowHandle) || ActiveEditor == null) {
				LayoutChanged();
				return;
			}
			ViewInfo.SetDataDirty();
			CheckViewInfo();
		}
		int lockFilterSetRowValue = 0;
		protected virtual void SetFilterRowValue(GridColumn column, object _value) {
			if(column == null || this.lockFilterSetRowValue != 0) return;
			this.lockFilterSetRowValue ++; 
			BeginLockFocusedRowChange();
			try {
				if(ActiveEditor != null) {
					ActiveEditor.ErrorText = "";
					HideHint();
				}
				if(_value == DBNull.Value) _value = null;
				bool prev = this.fAllowCloseEditor;
				this.fAllowCloseEditor = false;
				try {
					column.FilterInfo = CreateFilterRowInfo(column, _value);
					if(IsDetailView) CalculateLayoutSynchronized();
				}
				finally {
					this.fAllowCloseEditor = prev;
				}
				UpdateActiveEditorBounds();
			} 
			catch(Exception e) {
				if(ActiveEditor != null && IsFilterRow(FocusedRowHandle) ) {
					string text = e.Message;
					if(text == null || text.Length == 0) text = "Filter Error";
					GridControl.EditorHelper.ShowEditorError(text);
				} else {
					XtraMessageBox.Show(ElementsLookAndFeel, GridControl.FindForm(), e.Message, "Filter Error");
				}
			}
			finally {
				this.lockFilterSetRowValue --;
				EndLockFocusedRowChange();
			}
		}
		protected virtual ColumnFilterInfo CreateFilterRowInfo(GridColumn column, object _value) {
			string strVal = _value == null ? null : _value.ToString();
			if(_value == null || strVal == string.Empty) return ColumnFilterInfo.Empty;
			AutoFilterCondition condition = ResolveAutoFilterCondition(column);
			CriteriaOperator op = CreateAutoFilterCriterion(column, condition, _value, strVal);
			return new ColumnFilterInfo(ColumnFilterType.AutoFilter, _value, op, string.Empty);
		}
		protected AutoFilterCondition ResolveAutoFilterCondition(GridColumn column) {
			RepositoryItem item = GetRowCellRepositoryItem(GridControl.AutoFilterRowHandle, column);
			AutoFilterCondition condition = column.OptionsFilter.AutoFilterCondition;
			if(column.GetFilterMode() == ColumnFilterMode.DisplayText) {
				if(condition == AutoFilterCondition.Default) condition = AutoFilterCondition.Like;
				return condition;
			}
			if(condition == AutoFilterCondition.Default) condition = AutoFilterCondition.Like;
			if(item is RepositoryItemCheckEdit || item is RepositoryItemDateEdit || item is RepositoryItemLookUpEditBase || item is RepositoryItemImageComboBox)
				condition = AutoFilterCondition.Equals;
			return condition;
		}
		protected virtual CriteriaOperator CreateAutoFilterCriterion(GridColumn column, AutoFilterCondition condition, object _value, string strVal) {
			if(condition == AutoFilterCondition.Equals) {
				return DataController.CalcColumnFilterCriteriaByValue(column.FilterFieldName, _value, true, IsRoundDateTime(column), null);
			} else {
				if(string.IsNullOrEmpty(strVal))
					return null;
				OperandProperty prop = new OperandProperty(column.FilterFieldName);
				if(condition == AutoFilterCondition.Contains)
					return new FunctionOperator(FunctionOperatorType.Contains, prop, strVal);
				switch(strVal[0]) {
					case '_':
					case '?':
					case '*':
					case '%':
						return new FunctionOperator(FunctionOperatorType.Contains, prop, strVal.Substring(1));
				}
				return new FunctionOperator(FunctionOperatorType.StartsWith, prop, strVal);
			}
		}
		internal bool ShouldSerializeOptionsBehavior() { return OptionsBehavior.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsBehavior"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new GridOptionsBehavior OptionsBehavior { get { return base.OptionsBehavior as GridOptionsBehavior; } }
		bool ShouldSerializeOptionsHint() { return OptionsHint.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsHint"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public GridOptionsHint OptionsHint {
			get { return _optionsHint; }
		}
		bool ShouldSerializeOptionsDetail() { return OptionsDetail.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsDetail"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public GridOptionsDetail OptionsDetail {
			get { return _optionsDetail; }
		}
		bool ShouldSerializeOptionsCustomization() { return OptionsCustomization.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsCustomization"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public GridOptionsCustomization OptionsCustomization {
			get { return _optionsCustomization; }
		}
		bool ShouldSerializeOptionsNavigation() { return OptionsNavigation.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsNavigation"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public GridOptionsNavigation OptionsNavigation {
			get { return _optionsNavigation; }
		}
		bool ShouldSerializeOptionsSelection() { return OptionsSelection.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsSelection"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new GridOptionsSelection OptionsSelection {
			get { return base.OptionsSelection as GridOptionsSelection; }
		}
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsView"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new GridOptionsView OptionsView { get { return base.OptionsView as GridOptionsView; } }
		bool ShouldSerializeOptionsFind() { return OptionsFind.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsFind"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new GridViewOptionsFind OptionsFind { get { return base.OptionsFind as GridViewOptionsFind; } }
		bool ShouldSerializeOptionsMenu() { return OptionsMenu.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsMenu"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public GridOptionsMenu OptionsMenu {
			get { return _optionsMenu; }
		}
		bool ShouldSerializeOptionsEditForm() { return OptionsEditForm.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsEditForm"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public GridOptionsEditForm OptionsEditForm {
			get { return _optionsEditForm; }
		}
		bool ShouldSerializeOptionsPrint() { return OptionsPrint.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsPrint"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new GridOptionsPrint OptionsPrint {
			get { return (GridOptionsPrint)base.OptionsPrint; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewDetailVerticalIndent"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(0)]
		public int DetailVerticalIndent {
			get { return detailVerticalIndent; }
			set {
				if(value < 0) value = 0;
				if(value > 100) value = 100;
				if(DetailVerticalIndent == value) return;
				detailVerticalIndent = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewFixedLineWidth"),
#endif
 DefaultValue(2), DXCategory(CategoryName.Appearance), XtraSerializableProperty()]
		public virtual int FixedLineWidth {
			get { return fixedLineWidth; }
			set {
				if(value < 1) value = 1;
				if(value > 12) value = 12;
				if(FixedLineWidth == value) return;
				this.fixedLineWidth = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewIndicatorWidth"),
#endif
 DefaultValue(-1), DXCategory(CategoryName.Appearance), XtraSerializableProperty()]
		public virtual int IndicatorWidth {
			get { return indicatorWidth; }
			set {
				if(value < 4) value = -1;
				if(IndicatorWidth == value) return;
				indicatorWidth = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewColumnPanelRowHeight"),
#endif
 DefaultValue(-1), DXCategory(CategoryName.Appearance), XtraSerializableProperty()]
		public virtual int ColumnPanelRowHeight {
			get { return columnPanelRowHeight; }
			set {
				if(value < -1) value = -1;
				if(ColumnPanelRowHeight == value) return;
				columnPanelRowHeight = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewRowSeparatorHeight"),
#endif
 DefaultValue(0), DXCategory(CategoryName.Appearance), XtraSerializableProperty()]
		public virtual int RowSeparatorHeight {
			get { return rowSeparatorHeight; }
			set {
				if(value < 0) value = 0;
				if(value > 100) value = 100;
				if(RowSeparatorHeight == value) return;
				rowSeparatorHeight = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewFooterPanelHeight"),
#endif
 DefaultValue(-1), DXCategory(CategoryName.Appearance), XtraSerializableProperty()]
		public virtual int FooterPanelHeight {
			get { return footerPanelHeight; }
			set {
				if(value < -1) value = -1;
				if(FooterPanelHeight == value) return;
				footerPanelHeight = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewHorzScrollVisibility"),
#endif
 DefaultValue(ScrollVisibility.Auto), XtraSerializableProperty(), DXCategory(CategoryName.Behavior)]
		public ScrollVisibility HorzScrollVisibility {
			get { return horzScrollVisibility; }
			set {
				if(HorzScrollVisibility == value) return;
				horzScrollVisibility = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewVertScrollVisibility"),
#endif
 DefaultValue(ScrollVisibility.Auto), XtraSerializableProperty(), DXCategory(CategoryName.Behavior)]
		public ScrollVisibility VertScrollVisibility {
			get { return vertScrollVisibility; }
			set {
				if(VertScrollVisibility == value) return;
				vertScrollVisibility = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewRowHeight"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public int RowHeight {
			get { return rowHeight; }
			set {
				if(value < 0) value = -1;
				if(RowHeight != value) {
					rowHeight = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGroupRowHeight"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public int GroupRowHeight {
			get { return groupRowHeight; }
			set {
				if(value < 0) value = -1;
				if(value > 100) value = 100;
				if(GroupRowHeight != value) {
					groupRowHeight = value;
					OnPropertiesChanged();
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Obsolete(ObsoleteText.SRGridView_CustomizationRowCount)]
		public int CustomizationRowCount {
			get { return 7; }
			set { }
		}
		protected virtual bool IsGroupCaptionIncludesColumn { get { return !AllowPartialGroups && !IsAlignGroupRowSummariesUnderColumns; } }
		protected internal virtual string GetGroupFormat() {
			if(!IsGroupCaptionIncludesColumn && GroupFormat == defaultGroupFormat) {
				return "[#image]{1} {2}";
			}
			return GroupFormat;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGroupFormat"),
#endif
 DefaultValue(defaultGroupFormat), XtraSerializableProperty(), DXCategory(CategoryName.Appearance), Localizable(true)]
		public string GroupFormat {
			get { return groupFormat; }
			set {
				if(value == null) value = string.Empty;
				if(GroupFormat != value) {
					groupFormat = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewChildGridLevelName"),
#endif
 DefaultValue(""), DXCategory(CategoryName.Appearance), XtraSerializableProperty(), Localizable(true)]
		public string ChildGridLevelName {
			get { return childGridLevelName; }
			set {
				if(value == null) value = string.Empty;
				if(ChildGridLevelName == value) return;
				childGridLevelName = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewVertScrollTipFieldName"),
#endif
 DefaultValue(""),
		Editor("DevExpress.XtraGrid.Design.GridColumnNameEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraGrid.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyGridDesign), XtraSerializableProperty(),
		DXCategory(CategoryName.Appearance)]
		public string VertScrollTipFieldName {
			get { return vertScrollTipFieldName; }
			set {
				if(value == null) value = "";
				if(VertScrollTipFieldName == value) return;
				vertScrollTipFieldName = value;
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPreviewFieldName"),
#endif
 DefaultValue(""), DXCategory(CategoryName.Appearance),
		Editor("DevExpress.XtraGrid.Design.GridColumnNameEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraGrid.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyGridDesign), XtraSerializableProperty()]
		public string PreviewFieldName {
			get { return previewFieldName; }
			set {
				if(value == null) value = "";
				if(PreviewFieldName == value) return;
				previewFieldName = value;
				OnPropertiesChanged();
				CheckPreviewColumn();
			}
		}
		void CheckPreviewColumn() {
			if(OptionsView.ShowPreview) {
				if(DataController.Columns[GridView.PreviewUnboundColumnName] == null) OnColumnUnboundChanged(null);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
		 EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(1001), XtraSerializablePropertyId(LayoutIdData)]
		public string GroupSummarySortInfoState {
			get { return GroupSummarySortInfo.GetStateInfo(); }
			set { GroupSummarySortInfo.RestoreState(value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public GroupSummarySortInfoCollection GroupSummarySortInfo { get { return groupSummarySortInfo; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGroupPanelText"),
#endif
 DefaultValue(""), XtraSerializableProperty(), DXCategory(CategoryName.Appearance), Localizable(true)]
		public string GroupPanelText {
			get { return groupPanelText; }
			set {
				if(value == null) value = string.Empty;
				if(GroupPanelText == value) return;
				groupPanelText = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewNewItemRowText"),
#endif
 DefaultValue(""), XtraSerializableProperty(), DXCategory(CategoryName.Appearance), Localizable(true)]
		public string NewItemRowText {
			get { return newItemRowText; }
			set {
				if(value == null) value = string.Empty;
				if(NewItemRowText == value) return;
				newItemRowText = value;
				OnPropertiesChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete(ObsoleteText.SRGridView_GroupFooterShowMode)]
		public GroupFooterShowMode GroupFooterShowMode {
			get { return OptionsView.GroupFooterShowMode; }
			set { OptionsView.GroupFooterShowMode = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewLevelIndent"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public virtual int LevelIndent {
			get { return levelIndent; }
			set {
				value = Math.Max(-1, value);
				if(LevelIndent == value) return;
				levelIndent = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPreviewIndent"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public virtual int PreviewIndent {
			get { return previewIndent; }
			set {
				value = Math.Max(-1, value);
				if(PreviewIndent == value) return;
				previewIndent = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPreviewLineCount"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public int PreviewLineCount {
			get { return previewLineCount; }
			set {
				if(value < 1) value = -1;
				if(previewLineCount != value) {
					previewLineCount = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewBestFitMaxRowCount"),
#endif
 DefaultValue(-1), DXCategory(CategoryName.Behavior), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int BestFitMaxRowCount {
			get { return OptionsView.BestFitMaxRowCount; } 
			set {
				OptionsView.BestFitMaxRowCount = value;
			}
		}
		[Browsable(false)]
		public override bool IsDefaultState {
			get { return State == GridState.Normal; }
		}
		[Browsable(false)]
		public override bool IsDraggingState { 
			get { 
				if(State == GridState.ColumnDragging) return true;
				return false;
			}
		}
		[Browsable(false)]
		public override bool IsSizingState { get { return State == GridState.ColumnSizing || State == GridState.RowDetailSizing || State == GridState.RowSizing; } }
		[Browsable(false)]
		public override bool IsEditing {
			get { return State == GridState.Editing || IsInplaceEditFormVisible; }
		}
		bool ShouldSerializeCustomizationFormBounds() { return !CustomizationFormBounds.IsEmpty; }
		[Browsable(false), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public Rectangle CustomizationFormBounds {
			get { return customizationFormBounds; }
			set {
				if(value != Rectangle.Empty) {
					Size size = value.Size;
					if(value.Width < 200) value.Width = 0;
					if(value.Height < 100) value.Height = 0;
					size.Width = Math.Max(200, value.Width);
					size.Height = Math.Max(100, value.Height);
					value.Location = DevExpress.Utils.ControlUtils.CalcLocation(value.Location, value.Location, size);
				}
				if(CustomizationFormBounds == value) return;
				customizationFormBounds = value;
				if(CustomizationForm != null && !value.IsEmpty) {
					if(value.Width == 0) value.Width = CustomizationForm.Bounds.Width;
					if(value.Height == 0) value.Height = CustomizationForm.Bounds.Height;
					CustomizationForm.Bounds = value;
				}
			}
		}
		[Browsable(false)]
		public CustomizationForm CustomizationForm { get { return customizationForm; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewDefaultRelationIndex"),
#endif
 DefaultValue(0)]
		public virtual int DefaultRelationIndex {
			get { return defaultRelationIndex; }
			set {
				if(value < 0) value = 0;
				if(DefaultRelationIndex == value) return;
				defaultRelationIndex = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditingValue {
			get {
				if(!IsEditing || ActiveEditor == null) return null;
				return base.EditingValue;
			}
			set {
				base.EditingValue = value;
			}
		}
		[Browsable(false)]
		public bool IsShowDetailButtons {
			get { 
				return AllowMasterDetail && 
					OptionsView.ShowDetailButtons && GetDetailInfo(InvalidRowHandle, true) != null; 
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LeftCoord {
			get { 
				if(OptionsView.ColumnAutoWidth && !AllowScrollAutoWidth) return 0;
				return leftCoord; 
			}
			set { 
				if(!IsLockUpdate) value = Math.Min(CalcHScrollRange() - CalcHScrollableRange(), value);
				if(value < 0) value = 0;
				if(OptionsView.ColumnAutoWidth) {
					if(!AllowScrollAutoWidth || IsDataInitializing) value = 0;
				}
				if(LeftCoord != value) {
					CloseActiveEditor(false);
					leftCoord = value;
					SynchronizeLeftCoord();
					DoLeftCoordChanged();
				}
			}
		}
		[Browsable(false)]
		public GridColumn PressedColumn {
			get { 
				if(State == GridState.ColumnDown)
					return ((GridHandler)Handler).DownPointHitInfo.Column;
				return null;
			}
		}
		int topRowPixel = 0;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TopRowPixel {
			get { return topRowPixel; }
			set {
				value = CheckTopRowPixelCore(value);
				if(value < 0) value = 0;
				if(value == TopRowPixel) return;
				OnScrollPositionChanging(value);
			}
		}
		public void SmoothScroll(int position) {
			((GridViewByPixelScroller)Scroller).SmoothScroll(position - TopRowPixel, false);
		}
		bool? isPixelScrolling = null;
		bool prevIsPixelScrolling = false;
		protected internal bool IsPixelScrolling { 
			get {
				if(isPixelScrolling == null) {
					isPixelScrolling = CheckIsPixelScrolling();
					if(prevIsPixelScrolling != isPixelScrolling.Value) {
						prevIsPixelScrolling = isPixelScrolling.Value;
						OnPixelScrollingChanged();
					}
				}
				return isPixelScrolling.Value; 
			} 
		}
		protected virtual void OnPixelScrollingChanged() {
			this.scroller = null;
			ViewInfo.ResetPixels();
		}
		protected virtual bool CheckIsPixelScrolling() {
			if(OptionsBehavior.AllowPixelScrolling == DefaultBoolean.False) return false;
			if(OptionsBehavior.AllowPixelScrolling == DefaultBoolean.Default && !IsAllowPixelScrollingByDefault) return false;
			if((OptionsView.RowAutoHeight || Events[calcRowHeight] != null) && !IsAllowPixelScrollingAutoRowHeight) return false;
			if(OptionsView.ShowPreview && !IsAllowPixelScrollingPreview) return false;
			return true;
		}
		protected virtual bool IsAllowPixelScrollingAutoRowHeight { get { return false; } }
		protected virtual bool IsAllowPixelScrollingPreview { get { return false; } }
		protected virtual bool IsAllowPixelScrollingByDefault { 
			get {
				if(!WindowsFormsSettings.IsAllowPixelScrolling) return false;
				if(!OptionsDetail.EnableMasterViewMode) return true;
				if(DataController.DetailColumns.Count > 0) return false;
				return true;
			} 
		}
		protected virtual void OnScrollPositionChanging(int newValue) {
			CloseActiveEditor(true);
			int prev = this.topRowPixel;
			try {
				this.topRowPixel = newValue;
				this.lockRefreshRows++;
				try {
					SetTopRowIndex(ViewInfo.CalcVisibleRowByPixel(newValue), true);
				}
				finally {
					this.lockRefreshRows--;
				}
				RefreshRows(true, true);
			}
			catch {
				this.topRowPixel = prev;
				Invalidate();
			}
			if(IsPixelScrolling) SynchronizeTopRowIndex();
		}
		internal bool ValidateTopRowIndexByPixel() {
			if(!IsPixelScrolling) return false;
			int newIndex = ViewInfo.CalcVisibleRowByPixel(TopRowPixel);
			if(TopRowIndex != newIndex) {
				this.topRowIndex = newIndex;
				return true;
			}
			return false;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TopRowIndex {
			get { return topRowIndex; }
			set {
				if(value > 0 && value >= RowCount) value = RowCount - 1;
				if(value < 0) value = 0;
				if(value == TopRowIndex) return;
				if(IsPixelScrolling) {
					TopRowPixel = ViewInfo.CalcPixelPositionByRow(value);
				}
				else {
					SetTopRowIndex(value, false);
				}
			}
		}
		void SetTopRowIndex(int value, bool ignoreCheck) {
			if(value > 0 && value >= RowCount) value = RowCount - 1;
			if(value < 0) value = 0;
			if(value == TopRowIndex) return;
			this.requireCheckTopRowIndex = false;
			if(ignoreCheck) {
				InternalSetTopRowIndex(value);
			} else
				InternalSetTopRowIndex(CheckTopRowIndex(value));
		}
		public override void SynchronizeData(BaseView viewSource) {
			if(viewSource == null) return;
			BeginSynchronization();
			BeginDataUpdate();
			try {
				lockSummary ++;
				try {
					base.SynchronizeData(viewSource);
					GridView gv = viewSource as GridView;
					if(gv == null) return;
					GroupSummary.Assign(gv.GroupSummary);
					TotalSummary.SetDirty();
				}
				finally {
					lockSummary--;
					SynchronizeSummary();
				}
			}
			finally {
				EndDataUpdateCore(true);
				EndSynchronization();
			}
		}
		public override void SynchronizeVisual(BaseView viewSource) {
			if(viewSource == null) return;
			BeginSynchronization();
			BeginUpdate();
			try {
				base.SynchronizeVisual(viewSource);
				GridView gv = viewSource as GridView;
				if(gv == null) return;
				SyncGridProperties(gv);
			}
			finally {
				EndUpdate();
				EndSynchronization();
			}
		}
		protected override void OnResetSerializationProperties(OptionsLayoutBase options) {
			base.OnResetSerializationProperties(options);
			OptionsLayoutGrid optGrid = options as OptionsLayoutGrid;
			if(optGrid == null || optGrid.StoreAllOptions) {
				OptionsHint.Reset();
				OptionsDetail.Reset();
				OptionsCustomization.Reset();
				OptionsNavigation.Reset();
				OptionsMenu.Reset();
			}
		}
		void SyncGridProperties(GridView gv) {
			if(AllowAssignSplitOptions) {
				this.horzScrollVisibility = gv.HorzScrollVisibility;
				this.vertScrollVisibility = gv.VertScrollVisibility;
			}
			this.bestFitMaxRowCount = gv.BestFitMaxRowCount;
			this.childGridLevelName = gv.ChildGridLevelName;
			this.columnPanelRowHeight = gv.ColumnPanelRowHeight;
			this.customizationFormBounds = gv.CustomizationFormBounds;
			this.detailVerticalIndent = gv.DetailVerticalIndent;
			this.fixedLineWidth = gv.FixedLineWidth;
			this.focusRectStyle = gv.FocusRectStyle;
			this.footerPanelHeight = gv.FooterPanelHeight;
			this.groupFormat = gv.GroupFormat;
			this.newItemRowText = gv.NewItemRowText;
			this.groupPanelText = gv.GroupPanelText;
			this.groupRowHeight = gv.GroupRowHeight;
			this.horzScrollStep = gv.HorzScrollStep;
			this.indicatorWidth = gv.IndicatorWidth;
			this.previewFieldName = gv.PreviewFieldName;
			this.previewLineCount = gv.PreviewLineCount;
			this.levelIndent = gv.LevelIndent;
			this.previewIndent = gv.PreviewIndent;
			this.rowHeight = gv.RowHeight;
			this.userCellPadding = gv.UserCellPadding;
			this.rowSeparatorHeight = gv.RowSeparatorHeight;
			this.scrollStyle = gv.ScrollStyle;
			this.vertScrollTipFieldName = gv.VertScrollTipFieldName;
			this.defaultRelationIndex = gv.DefaultRelationIndex;
			this.OptionsEditForm.Assign(gv.OptionsEditForm);
			this.OptionsCustomization.Assign(gv.OptionsCustomization);
			this.OptionsDetail.Assign(gv.OptionsDetail);
			this.OptionsHint.Assign(gv.OptionsHint);
			this.OptionsMenu.Assign(gv.OptionsMenu);
			this.OptionsNavigation.Assign(gv.OptionsNavigation);
			this.OptionsPrint.Assign(gv.OptionsPrint);
		}
		protected internal override void ResetLookUp(bool sameDataSource) {
			this.extraFilter = this.extraFilterText = string.Empty;
			base.ResetLookUp(sameDataSource);
		}
		public override void Assign(BaseView v, bool copyEvents) {
			if(v == null) return;
			BeginUpdate();
			try {
				this.lockSummary ++;
				try {
					base.Assign(v, copyEvents);
				}
				finally {
					this.lockSummary --;
				}
				GridView gv = v as GridView;
				if(gv != null) {
					this.lockSummary ++;
					try {
						SyncGridProperties(gv);
						if(this.GroupSummary.Count != gv.GroupSummary.Count || gv.GroupSummary.Count > 0) {
							GroupSummary.Assign(gv.GroupSummary);
						}
					}
					finally {
						this.lockSummary --;
						SynchronizeSummary();
					}
					AssignGroupSummary(gv);
					if(copyEvents) {
						Events.AddHandler(cellMerge, gv.Events[cellMerge]);
						Events.AddHandler(rowClick, gv.Events[rowClick]);
						Events.AddHandler(rowCellClick, gv.Events[rowCellClick]);
						Events.AddHandler(customColumnGroup, gv.Events[customColumnGroup]);
						Events.AddHandler(showingPopupEditForm, gv.Events[showingPopupEditForm]);
						Events.AddHandler(dragObjectDrop, gv.Events[dragObjectDrop]);
						Events.AddHandler(dragObjectStart, gv.Events[dragObjectStart]);
						Events.AddHandler(dragObjectOver, gv.Events[dragObjectOver]);
						Events.AddHandler(calcPreviewText, gv.Events[calcPreviewText]);
						Events.AddHandler(measurePreviewHeight, gv.Events[measurePreviewHeight]);
						Events.AddHandler(calcRowHeight, gv.Events[calcRowHeight]);
						Events.AddHandler(customDrawCell, gv.Events[customDrawCell]);
						Events.AddHandler(customDrawColumnHeader, gv.Events[customDrawColumnHeader]);
						Events.AddHandler(customDrawFooter, gv.Events[customDrawFooter]);
						Events.AddHandler(customDrawFooterCell, gv.Events[customDrawFooterCell]);
						Events.AddHandler(customDrawGroupPanel, gv.Events[customDrawGroupPanel]);
						Events.AddHandler(customDrawGroupRow, gv.Events[customDrawGroupRow]);
						Events.AddHandler(customDrawGroupRowCell, gv.Events[customDrawGroupRowCell]);
						Events.AddHandler(customDrawRowFooter, gv.Events[customDrawRowFooter]);
						Events.AddHandler(customDrawRowFooterCell, gv.Events[customDrawRowFooterCell]);
						Events.AddHandler(customDrawRowIndicator, gv.Events[customDrawRowIndicator]);
						Events.AddHandler(customDrawRowPreview, gv.Events[customDrawRowPreview]);
						Events.AddHandler(editFormShowing, gv.Events[editFormShowing]);
						Events.AddHandler(editFormPrepared, gv.Events[editFormPrepared]);
						Events.AddHandler(customRowCellEdit, gv.Events[customRowCellEdit]);
						Events.AddHandler(customRowCellEditForEditing, gv.Events[customRowCellEditForEditing]);
						Events.AddHandler(customSummaryCalculate, gv.Events[customSummaryCalculate]);
						Events.AddHandler(customSummaryExists, gv.Events[customSummaryExists]);
						Events.AddHandler(gridMenuItemClick, gv.Events[gridMenuItemClick]);
						Events.AddHandler(groupLevelStyle, gv.Events[groupLevelStyle]);
						Events.AddHandler(groupRowCollapsed, gv.Events[groupRowCollapsed]);
						Events.AddHandler(groupRowCollapsing, gv.Events[groupRowCollapsing]);
						Events.AddHandler(groupRowExpanded, gv.Events[groupRowExpanded]);
						Events.AddHandler(groupRowExpanding, gv.Events[groupRowExpanding]);
						Events.AddHandler(leftCoordChanged, gv.Events[leftCoordChanged]);
						Events.AddHandler(columnWidthChanged, gv.Events[columnWidthChanged]);
						Events.AddHandler(masterRowCollapsed, gv.Events[masterRowCollapsed]);
						Events.AddHandler(masterRowCollapsing, gv.Events[masterRowCollapsing]);
						Events.AddHandler(masterRowEmpty, gv.Events[masterRowEmpty]);
						Events.AddHandler(masterRowExpanded, gv.Events[masterRowExpanded]);
						Events.AddHandler(masterRowExpanding, gv.Events[masterRowExpanding]);
						Events.AddHandler(masterRowGetChildList, gv.Events[masterRowGetChildList]);
						Events.AddHandler(masterRowGetLevelDefaultView, gv.Events[masterRowGetLevelDefaultView]);
						Events.AddHandler(masterRowGetRelationCount, gv.Events[masterRowGetRelationCount]);
						Events.AddHandler(masterRowGetRelationName, gv.Events[masterRowGetRelationName]);
						Events.AddHandler(masterRowGetRelationDisplayCaption, gv.Events[masterRowGetRelationDisplayCaption]);
						Events.AddHandler(rowCellStyle, gv.Events[rowCellStyle]);
						Events.AddHandler(rowStyle, gv.Events[rowStyle]);
						Events.AddHandler(hideCustomizationForm, gv.Events[hideCustomizationForm]);
						Events.AddHandler(showCustomizationForm, gv.Events[showCustomizationForm]);
						Events.AddHandler(showGridMenu, gv.Events[showGridMenu]);
						Events.AddHandler(onPopupMenuShowing, gv.Events[onPopupMenuShowing]);
						Events.AddHandler(topRowChanged, gv.Events[topRowChanged]);
						Events.AddHandler(beforePrintRow, gv.Events[beforePrintRow]);
						Events.AddHandler(afterPrintRow, gv.Events[afterPrintRow]);
						Events.AddHandler(getThumbnailImage, gv.Events[getThumbnailImage]);
						Events.AddHandler(getLoadingImage, gv.Events[getLoadingImage]);
					}
				}
			}
			finally {
				EndUpdate();
			}
		}
		void AssignGroupSummary(GridView gv) {
			if(gv.GroupSummarySortInfo.Count == 0) return;
			GroupSummarySortInfo.BeginUpdate();
			try {
				foreach(GroupSummarySortInfo info in gv.GroupSummarySortInfo) {
					int index = info.SummaryItem.Index;
					int colIndex = info.GroupColumn.AbsoluteIndex;
					if(index >= GroupSummary.Count || colIndex >= Columns.Count) continue;
					GroupSummarySortInfo.Add(GroupSummary[index], info.SortOrder, Columns[colIndex]);
				}
			}
			finally {
				GroupSummarySortInfo.EndUpdate();
			}
		}
		public virtual void BestFitColumns() {
			BestFitColumns(false);
		}
		public virtual void BestFitColumns(bool forceResize) {
			Cursor currentCursor = GetCursor();
			SetCursor(Cursors.WaitCursor);
			try {
				CheckLoaded();
				foreach(GridColumn col in Columns) {
					if(!col.Visible) continue;
					if(!CanResizeColumn(col) && !forceResize) continue;
					int w = ViewInfo.CalcColumnBestWidth(col);
					if(w == 0) continue;
					w = Math.Max(GetColumnMinWidth(col), w);
					if(GetColumnMaxWidth(col) > 0) w = Math.Min(GetColumnMaxWidth(col), w);
					col.width = w;
				}
				ViewInfo.RecalcColumnWidthes();
				LayoutChanged();
			} finally {
				SetCursor(currentCursor);
			}
		}
		public GridColumn AddUnboundColumn() {
			string name;
			for(int n = 1; ; n++) {
				name = string.Format("UnboundColumn{0}", n);
				if(Columns[name] == null) break;
			}
			return AddUnboundColumn(name, "");
		}
		public GridColumn AddUnboundColumn(string columnName, string caption) {
			if(Columns[columnName] != null) return null;
			GridColumn column = Columns.Add();
			column.Name = columnName;
			if(!string.IsNullOrEmpty(caption)) column.Caption = caption;
			column.UnboundType = UnboundColumnType.Object;
			column.OptionsColumn.AllowSort = DefaultBoolean.True;
			column.ShowUnboundExpressionMenu = true;
			ShowUnboundExpressionEditor(column);
			if(string.IsNullOrEmpty(column.UnboundExpression)) {
				column.Dispose();
				return null;
			}
			column.Visible = true;
			return column;
		}
		public new GridHitInfo CalcHitInfo(Point pt) { return base.CalcHitInfo(pt) as GridHitInfo; }
		public new GridHitInfo CalcHitInfo(int x, int y) { return CalcHitInfo(new Point(x, y)); }
		protected override BaseHitInfo CalcHitInfoCore(Point pt) { return ViewInfo.CalcHitInfo(pt); }
		protected internal bool IsAllowBusyIndicator {
			get {
				return CheckAllowNotifications() &&
					(GetWaitOptions() == WaitAnimationOptions.Indicator);
			}
		}
		protected internal bool IsAllowBusyPanel {
			get {
				return CheckAllowNotifications() &&
					(GetWaitOptions() == WaitAnimationOptions.Panel);
			}
		}
		bool forceLoadingPanel = false;
		protected override bool ForceLoadingPanel { get { return forceLoadingPanel; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool LoadingPanelVisible {
			get { return GetIsNotificationInfoVisible(); }
			set {
				if(value)
					ShowLoadingPanel();
				else
					HideLoadingPanel();
			}
		}
		public virtual void ShowLoadingPanel() {
			ChangeLoadingPanelVisiibility(true);
		}
		public virtual void HideLoadingPanel() {
			HideEditor();
			ChangeLoadingPanelVisiibility(false);
		}
		void ChangeLoadingPanelVisiibility(bool newVisibility) {
			if(forceLoadingPanel == newVisibility) return;
			this.forceLoadingPanel = newVisibility;
			CheckAllowNotifications();
			if(!newVisibility) NotificationInfo.HideImmediate();
			Invalidate();
			DelayedInvalidate(NotificationInfo.WaitAnimationShowDelay + 10);
		}
		WaitAnimationOptions GetWaitOptions() {
			if(forceLoadingPanel) return WaitAnimationOptions.Panel;
			if(OptionsView.WaitAnimationOptions == WaitAnimationOptions.Default)
				return WorkAsLookup ? WaitAnimationOptions.Panel : WaitAnimationOptions.Indicator;
			return OptionsView.WaitAnimationOptions;
		}
		public virtual bool CanDragColumn(GridColumn column) {
			if(!GridControl.IsDesignMode && !ForcedDesignMode && (!column.OptionsColumn.AllowMove || !OptionsCustomization.AllowColumnMoving)) return false;
			if(this.VisibleColumns.Count > 1 || SortInfo.GroupCount == 0) 
				return true;
			if(column.GroupIndex > -1 || column.VisibleIndex < 0) 
				return true;
			return false;
		}
		public virtual bool CanExpandMasterRow(int rowHandle) {
			return CanExpandMasterRowEx(rowHandle, CalcDefaultRelationIndex(rowHandle));
		}
		public virtual bool CanExpandMasterRowEx(int rowHandle, int relationIndex) {
			if(!IsMasterRow(rowHandle)) return false;
			bool result = true;
			if(!OptionsDetail.AllowExpandEmptyDetails) {
				result = false;
				if(!IsMasterRowEmptyEx(rowHandle, relationIndex)) {
					IList list = DataController.GetDetailList(rowHandle, relationIndex);
					if(list != null && list.Count > 0) result = true;
				}
			}
			if(GridControl.ShowOnlyPredefinedDetails) {
				GridDetailInfo[] details = GetDetailInfo(rowHandle, true);
				if(details == null || details.Length == 0) result = false;
				else {
					bool prevResult = result;
					result = false;
					foreach(GridDetailInfo detail in details) {
						if(detail.RelationIndex == relationIndex) {
							result = prevResult;
							break;
						}
					}
				}
			}
			MasterRowCanExpandEventHandler handler = (MasterRowCanExpandEventHandler)this.Events[masterRowExpanding];
			if(handler != null) {
				MasterRowCanExpandEventArgs e = new MasterRowCanExpandEventArgs(rowHandle, relationIndex, result);
				handler(this, e);
				result = e.Allow;
			}
			return result;
		}
		protected internal override bool CanCollapseMasterRow(int rowHandle) {
			if(!GetMasterRowExpandedEx(rowHandle, -1)) return true;
			return CanCollapseMasterRowEx(rowHandle, -1);
		}
		public virtual bool CanCollapseMasterRowEx(int rowHandle, int relationIndex) {
			if(!GetMasterRowExpandedEx(rowHandle, relationIndex)) return false;
			MasterRowCanExpandEventHandler handler = (MasterRowCanExpandEventHandler)this.Events[masterRowCollapsing];
			if(handler != null) {
				MasterRowCanExpandEventArgs e = new MasterRowCanExpandEventArgs(rowHandle, relationIndex, true);
				handler(this, e);
				return e.Allow;
			}
			return true;
		}
		public override bool CanResizeColumn(GridColumn column) {
			if(GridControl.IsDesignMode || ForcedDesignMode) return true;
			if(column == null || !OptionsCustomization.AllowColumnResizing) return false;
			return column.OptionsColumn.AllowSize || IsDesignMode;
		}
		public virtual bool CanResizeRow(int rowHandle) {
			if(!OptionsCustomization.AllowRowSizing) return false;
			if(IsGroupRow(rowHandle)) return false;
			return true;
		}
		public virtual bool CanResizeDetailRow(int rowHandle) {
			return true;
		}
		public virtual bool CanShowFilterButton(GridColumn column) {
			if(DataController.IsReady) {
				return OptionsCustomization.AllowFilter && (column == null ||
					IsColumnAllowFilter(column) && 
					(column.VisibleIndex > -1 || column.GroupIndex > -1));
			}
			return false;
		}
		public override void ClearSorting() {
			if(IsEditFormVisible) return;
			base.ClearSorting();
		}
		public override bool CanSortColumn(GridColumn column) { 
			if(!OptionsCustomization.AllowSort) return false;
			if(IsCheckboxSelector(column)) return false;
			if(IsInplaceEditFormVisible) return false;
			return base.CanSortColumn(column);
		}
		public override bool CanGroupColumn(GridColumn column) {
			if(!DataController.CanGroup) return false;
			if(!CanSortColumn(column)) return false;
			if(!OptionsCustomization.AllowGroup) return false;
			if(!column.GetAllowGroup()) return false;
			if(column.GroupIndex == -1 && Columns.Count - 1 == SortInfo.GroupCount) return false;
			return column.GroupIndex > -1 || (Columns.Count > 1 && (VisibleColumns.Count > 1 || column.VisibleIndex == -1));
		}
		protected override void OnBeforeMouseSortColumn() {
			GroupSummarySortInfo.Clear();
		}
		protected internal override bool CanDataSortColumn(GridColumn column) {
			return IsLevelDefault || base.CanDataSortColumn(column);
		}
		protected override bool CanShowColumn(GridColumn column) {
			return column.GroupIndex < 0 || GetShowGroupedColumns();
		}
		public void ClearGrouping() {
			BeginDataUpdate();
			try {
				GroupSummarySortInfo.Clear();
				ArrayList list = new ArrayList();
				foreach(GridColumn column in Columns) {
					if(column.GroupIndex == -1) continue;
					column.UnGroup();
					list.Add(column);
				}
				foreach(GridColumn column in list) {
					if(!column.Visible) column.Visible = true;
				}
			}
			finally {
				EndDataUpdateCore(true);
			}
		}
		public void CollapseAllDetails() { CollapseAllDetailsCore(); }
		protected internal override void CollapseAllDetailsCore() {
			DataController.CollapseDetailRows();
			LayoutChanged();
		}
		public virtual void ExpandAllGroups() {
			CheckLoaded();
			if(!RaiseGroupRowExpanding(InvalidRowHandle)) return;
			DataController.ExpandAll();
			RaiseGroupRowExpanded(InvalidRowHandle);
			UpdateNavigator();
		}
		public virtual void CollapseAllGroups() {
			CheckLoaded();
			if(!RaiseGroupRowCollapsing(InvalidRowHandle)) return;
			DataController.CollapseAll();
			RaiseGroupRowCollapsed(InvalidRowHandle);
			UpdateNavigator();
			if(IsPixelScrolling) { 
				SetTopRowIndexDirty();
				LayoutChanged();
			}
		}
		public virtual void ColumnsCustomization(Point showPoint) {
			DestroyCustomization();
			if(GridControl == null) return;
			customizationForm = CreateCustomizationForm();
			customizationForm.ShowCustomization(showPoint);
			RaiseShowCustomizationForm(EventArgs.Empty);
		}
		public void ColumnsCustomization() {
			ColumnsCustomization(BaseViewInfo.EmptyPoint);
		}
		public void ShowCustomization() {
			ColumnsCustomization(BaseViewInfo.EmptyPoint);
		}
		public void HideCustomization() {
			DestroyCustomization();
		}
		public void DestroyCustomization() {
			if(customizationForm != null) {
				SaveCustomizationFormBounds();
				Form form = customizationForm;
				customizationForm = null;
				form.Visible = false;
				form.Parent = null;
				form.Dispose();
				RaiseHideCustomizationForm(EventArgs.Empty);
			}
		}
		void SaveCustomizationFormBounds() {
			this.customizationFormBounds = CustomizationForm.Bounds;
		}
		protected override void OnLocalizer_Changed(object sender, EventArgs e) {
			if(CustomizationForm != null) {
				DestroyCustomization();
				ColumnsCustomization();
			}
			base.OnLocalizer_Changed(sender, e);
		}
		protected virtual void RecreateColumnsCustomization() {
			if(CustomizationForm == null) return;
			DestroyCustomization();
			ColumnsCustomization();
		}
		protected virtual CustomizationForm CreateCustomizationForm() {
			return new CustomizationForm(this);
		}
		protected internal override bool OnContextMenuClick() {
			GridHitInfo hitInfo = ViewInfo.CreateHitInfo();
			GridRowInfo rowInfo = ViewInfo.GetGridRowInfo(FocusedRowHandle);
			Point hitPoint = ViewInfo.ViewRects.Rows.Location;
			if(rowInfo != null) hitPoint = new Point(rowInfo.DataBounds.Left, rowInfo.DataBounds.Bottom); 
			hitInfo.HitPoint = hitPoint;
			if(IsValidRowHandle(FocusedRowHandle)) {
				hitInfo.HitTest = GridHitTest.Row;
				hitInfo.SetRowHandle(FocusedRowHandle, FocusedListSourceIndex);
				return DoShowGridMenu(GridMenuType.Row, new GridViewMenu(this), hitInfo, true);
			}
			else {
				return DoShowGridMenu(null, hitInfo);
			}
		}
		protected internal override int ScrollPageSize { 
			get {
				int res = ViewInfo.RowsInfo.GetScrollableRowsCount(this);
				bool isLastRowOnPageVisiblePartially = IsRowVisible(GetVisibleRowHandle(ViewInfo.RowsInfo.GetLastVisibleRowIndex())) == RowVisibleState.Partially;
				if(res > 0 && isLastRowOnPageVisiblePartially) res--;
				if(res < 0) res = 0;
				if(res < 1 && ViewInfo.RowsInfo.Count > 0) res = 1;
				return res; 
			} 
		}
		protected internal override void AddToGridControl() {
			if(GridControl != null) {
				ScrollInfo.AddControls(GridControl);
			}
			base.AddToGridControl();
		}
		protected internal override void RemoveFromGridControl() {
			if(GridControl != null) {
				if(detailTip != null) detailTip.HideTip();
				DestroyCustomization();
				if(DataController != null)
					DataController.CollapseDetailRows();
				ScrollInfo.RemoveControls(GridControl);
			}
			base.RemoveFromGridControl();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				fViewDisposing = true;
				if(destroying) return;
				GroupSummarySortInfo.Clear();
				if(ScrollTimer != null) ScrollTimer.Dispose();
				destroying = true;
				detailTip.Dispose();
				BeginUpdate();
				if(DataController != null)
					DataController.CollapseDetailRows();
				DestroyEditFormController();
				HideEditor();
				DestroyCustomization();
				DestroyCheckboxSelector();
				if(ScrollInfo != null) {
					this.scrollInfo.VScroll_Scroll -= new ScrollEventHandler(OnVertScroll);
					this.scrollInfo.HScroll_Scroll -= new ScrollEventHandler(OnHorzScroll);
					this.scrollInfo.VScroll_ValueChanged -= new EventHandler(OnVScroll);
					this.scrollInfo.HScroll_ValueChanged -= new EventHandler(OnHScroll);
					this.scrollInfo.Dispose();
					this.scrollInfo = null;
				}
				if(OptionsEditForm.CustomEditFormLayout != null) {
					OptionsEditForm.CustomEditFormLayout.Dispose();
					OptionsEditForm.CustomEditFormLayout = null;
				}
			}
			base.Dispose(disposing);
		}
		protected internal EditFormController EditFormController {
			get {
				return editFormController;
			}
		}
		protected void EnsureEditFormController() {
			if(EditFormController == null) {
				editFormController = CreateEditFormController();
				editFormController.CloseEditForm += OnEditFormClose;
			}
		}
		void HideEditFormCore() {
			ViewInfo.EditFormBounds = Rectangle.Empty;
			ViewInfo.EditFormRequiredHeight = 0;
			ViewInfo.EditForm = null;
			this.editFormVisible = false;
		}
		void OnEditFormClose(object sender, EventArgs e) {
			if(IsInplaceEditFormVisible) {
				HideEditFormCore();
				ViewInfo.PositionHelper.ResetRow(FocusedRowHandle);
				CheckLayoutEditFormClose();
				Focus();
			}
		}
		internal void CheckLayoutEditFormClose() {
			var updateLayout = new MethodInvoker(() => {
				CheckTopRowIndex();
				LayoutChangedSynchronized();
			});
			if(lockVScroll) {
				GridControl.BeginInvoke(updateLayout);
			}
			else {
				updateLayout();
			}
		}
		protected virtual EditFormController CreateEditFormController() {
			return new EditFormController(this);
		}
		protected virtual void DestroyEditFormController() {
			HideEditForm();
			if(editFormController != null) editFormController.Dispose();
			this.editFormController = null;
		}
		public int GetDataRowHandleByGroupRowHandle(int groupRowHandle) {
			return DataController.GetControllerRowByGroupRow(groupRowHandle);
		}
		public int GetParentRowHandle(int rowHandle) {
			if(IsGroupRow(rowHandle)) return DataController.GroupInfo.GetParentRow(rowHandle);
			GroupRowInfo group = DataController.GroupInfo.GetGroupRowInfoByControllerRowHandle(rowHandle);
			return group != null ? group.Handle : InvalidRowHandle;
		}
		public int GetChildRowCount(int rowHandle) {
			return DataController.GroupInfo.GetChildCount(rowHandle);
		}
		public int GetChildRowHandle(int rowHandle, int childIndex) {
			return DataController.GroupInfo.GetChildRow(rowHandle, childIndex);
		}
		protected override void OnCurrentControllerRowObjectChanged(CurrentRowChangedEventArgs e) {
			if(!IsNewItemRow(FocusedRowHandle)) HideEditForm();
			base.OnCurrentControllerRowObjectChanged(e);
		}
		public override void ShowEditorByKey(KeyEventArgs e) {
			if(IsEditFormMode && IsAllowEditForm(FocusedRowHandle)) {
				if(e.KeyData == Keys.Enter) {
					if(OptionsEditForm.ShowOnEnterKey == DefaultBoolean.False) return;
					ShowEditForm();
				}
				if(e.KeyData == Keys.F2) {
					if(OptionsEditForm.ShowOnF2Key == DefaultBoolean.False) return;
					ShowEditForm();
				}
				return;
			}
			base.ShowEditorByKey(e);
		}
		public override void ShowEditorByKeyPress(KeyPressEventArgs e) {
			if(IsEditFormMode && IsAllowEditForm(FocusedRowHandle)) {
				return;
			}
			base.ShowEditorByKeyPress(e);
		}
		protected internal virtual void RaiseEditFormPrepared(Control rootPanel) {
			EditFormPreparedEventArgs e = new EditFormPreparedEventArgs(FocusedRowHandle, rootPanel, EditFormController.BindableControls);
			EditFormPreparedEventHandler handler = (EditFormPreparedEventHandler)this.Events[editFormPrepared];
			if(handler != null) {
				handler(this, e);
			}
		}
		protected EditFormShowingEventArgs RaiseEditFormShowing() {
			EditFormShowingEventArgs e = new EditFormShowingEventArgs(FocusedRowHandle);
			EditFormShowingEventHandler handler = (EditFormShowingEventHandler)this.Events[editFormShowing];
			if(handler != null) {
				handler(this, e);
			}
			return e;
		}
		protected virtual bool BeforeShowEditForm() {
			if(IsDisposing || !IsDefaultState || GridControl == null) return false;
			DestroyFilterPopup();
			CheckViewInfo();
			if(!Focus() && lockEditorUpdatingByMouse) return false;
			return true;
		}
		public virtual void ShowEditForm() {
			if(!IsEditFormMode) return;
			if(IsInplaceEditForm)
				ShowInplaceEditForm();
			else
				ShowPopupEditForm();
		}
		protected internal virtual bool IsAllowEditForm(int rowHandle) {
			if(!OptionsBehavior.Editable) return false;
			if(IsDataRow(rowHandle)) return true;
			if(IsFilterRow(rowHandle)) return false;
			if(IsNewItemRow(rowHandle)) return true;
			return false;
		}
		bool PreShowEditForm() {
			if(!IsEditFormMode) return false;
			if(!IsAllowEditForm(FocusedRowHandle)) return false;
			if(!BeforeShowEditForm()) return false;
			CloseEditor(true);
			if(IsEditing) return false;
			if(!RaiseEditFormShowing().Allow) return false;
			EnsureEditFormController();
			return true;
		}
		public virtual void ShowPopupEditForm() {
			if(!PreShowEditForm()) return;
			var form = EditFormController.CreateEditForm(FocusedRowHandle);
			if(form != null) {
				Scroller.CancelSmoothScroll();
				CheckNewItemRowEditing();
				GridRowInfo row = ViewInfo.GetGridRowInfo(FocusedRowHandle);
				Point top = ViewRect.Location, bottom = Point.Empty;
				if(row != null) {
					top = row.TotalBounds.Location;
					bottom = new Point(row.TotalBounds.X, row.TotalBounds.Bottom);
				}
				form.StartPosition = FormStartPosition.Manual;
				form.Location = DevExpress.Utils.ControlUtils.CalcLocation(PointToScreen(bottom), PointToScreen(top), form.Size);
				EditFormController.FocusFirst();
				RaiseShowingPopupEditForm(new ShowingPopupEditFormEventArgs(form, FocusedRowHandle, EditFormController.BindableControls));
				ShowEditFormDialogCore(form);
			}
		}
		protected virtual void ShowEditFormDialogCore(Form editForm) {
			editForm.ShowDialog(GridControl.FindForm());
		}
		public virtual void ShowInplaceEditForm() {
			if(!PreShowEditForm()) return;
			Scroller.CancelSmoothScroll();
			UserControl editForm = EditFormController.PrepareEditFormControl(FocusedRowHandle);
			if(editForm == null) return;
			if(!GridControl.Contains(editForm)) {
				editForm.Visible = false;
				GridControl.Controls.Add(editForm);
			}
			ViewInfo.EditFormRequiredHeight = EditFormController.ProposedEditFormHeight;
			ViewInfo.PositionHelper.ResetRow(FocusedRowHandle);
			UpdateVScrollBar();
			if(!Scroller.ShowEditForm(EditFormController.ProposedEditFormHeight)) {
				ViewInfo.EditFormRequiredHeight = 0;
				ViewInfo.PositionHelper.ResetRow(FocusedRowHandle);
				LayoutChangedSynchronized();
				ShowPopupEditForm();
				return;
			}
			ViewInfo.EditForm = editForm;
			this.editFormVisible = true;
			CheckNewItemRowEditing();
			LayoutChangedSynchronized();
		}
		void CheckNewItemRowEditing() {
			if(IsNewItemRow(FocusedRowHandle) && !DataController.IsNewItemRowEditing) {
				try {
					BeginNewItemRowEditing();
				}
				catch {
					HideEditForm();
					throw;
				}
			}
		}
		protected internal BaseView GetDetailViewByKey(object rowKey, int relationIndex) {
			if(DataController == null) return null;
			MasterRowInfo masterRow = DataController.FindRowDetailInfo(rowKey);
			if(masterRow == null) return null;
			DetailInfo di = masterRow.FindDetail(relationIndex);
			if(di != null) return di.DetailOwner as BaseView;
			return null;
		}
		protected internal BaseView GetVisibleDetailViewByKey(object rowKey) {
			return GetDetailViewByKey(rowKey, -1);
		}
		public BaseView GetDetailView(int rowHandle, int relationIndex) {
			if(DataController == null) return null;
			MasterRowInfo masterRow = DataController.GetRowDetailInfo(rowHandle);
			if(masterRow == null) return null;
			DetailInfo di = masterRow.FindDetail(relationIndex);
			if(di != null) return di.DetailOwner as BaseView;
			return null;
		}
		public BaseView GetVisibleDetailView(int rowHandle) {
			return GetDetailView(rowHandle, -1);
		}
		public int GetVisibleDetailRelationIndex(int rowHandle) {
			if(DataController == null) return -1;
			MasterRowInfo rowInfo = DataController.GetRowDetailInfo(rowHandle);
			if(rowInfo == null) return -1;
			DetailInfo di = rowInfo.FindDetail(-1);
			return di == null ? -1 : di.RelationIndex;
		}
		public bool GetMasterRowExpanded(int rowHandle) {
			if(DataController == null) return false;
			return DataController.IsDetailRowExpanded(rowHandle);
		}
		public bool GetMasterRowExpandedEx(int rowHandle, int relationIndex) {
			if(DataController == null) return false;
			return DataController.IsDetailRowExpanded(rowHandle, relationIndex);
		}
		public bool GetRowExpanded(int rowHandle) {
			return DataController.IsRowExpanded(rowHandle);
		}
		public override void MoveNextPage() {
			Scroller.OnMoveNextPage();
		}
		public override void MovePrevPage() {
			Scroller.OnMovePrevPage();
		}
		protected internal void HideEditorByKey() {
			if(!IsInplaceEditFormVisible) {
				HideEditor();
				return;
			}
			HideEditForm();
		}
		public override void HideEditor() {
			if(!IsEditing || !fAllowCloseEditor) return;
			base.HideEditor();
			if(destroying) return;
			GridCellInfo cell = this.fEditingCell;
			this.fEditingCell = null;
			if(cell != null) {
				if(cell.ViewInfo is IAnimatedItem) XtraAnimator.RemoveObject(ViewInfo, new GridCellId(this, cell));
				cell.ViewInfo = null;
			}
			SetStateInt((int)GridState.Normal);
			if(cell != null) {
				ViewInfo.UpdateCellAppearance(cell, true);
				InvalidateRow(cell.RowHandle);
			}
			UpdateNavigator();
		}
		public virtual void InvalidateColumnHeader(GridColumn column) {
			if(!ViewInfo.IsReady) return;
			ViewInfo.PaintAnimatedItems = false;
			if(column == null)
				InvalidateRect(ViewInfo.ViewRects.ColumnPanelActual);
			else {
				GridColumnInfoArgs ci = ViewInfo.ColumnsInfo[column];
				if(ci != null)
					InvalidateRect(ci.Bounds);
			}
		}
		public virtual void InvalidateFilterPanel() {
			if(!IsViewInfoReady || !IsShowFilterPanel) return;
			ViewInfo.PaintAnimatedItems = false;
			InvalidateRect(ViewInfo.FilterPanel.Bounds);
		}
		public virtual void InvalidateFooter() {
			if(!IsViewInfoReady) return;
			ViewInfo.PaintAnimatedItems = false;
			InvalidateRect(ViewInfo.ViewRects.Footer);
		}
		public virtual void InvalidateGroupPanel() {
			if(!IsViewInfoReady || !OptionsView.ShowGroupPanel) return;
			InvalidateRect(ViewInfo.ViewRects.GroupPanel);
		}
		public virtual void InvalidateRow(int rowHandle) {
			if(!IsViewInfoReady) return;
			ViewInfo.PaintAnimatedItems = false;
			InvalidateRow(ViewInfo.GetGridRowInfo(rowHandle));
		}
		protected void InvalidateRow(GridRowInfo rowInfo) {
			if(rowInfo == null) return;
			ViewInfo.PaintAnimatedItems = false;
			GridDataRowInfo dataRow = rowInfo as GridDataRowInfo;
			if(dataRow != null && dataRow.HasMergedCells) {
				InvalidateRows();
				return;
			}
			Rectangle r = rowInfo.TotalBounds;
			if(ViewInfo.ShouldInflateRowOnInvalidate) r.Inflate(0, 1);
			InvalidateRect(r);
		}
		public virtual void InvalidateRowIndicator(int rowHandle) {
			GridRowInfo ri = ViewInfo.GetGridRowInfo(rowHandle);
			if(ri != null) {
				ViewInfo.PaintAnimatedItems = false;
				InvalidateRect(ri.IndicatorRect);
			}
		}
		public virtual void RefreshRowCell(int rowHandle, GridColumn column) {
			if(column == null) return;
			GridCellInfo cell = ViewInfo.GetGridCellInfo(rowHandle, column);
			if(cell != null && cell.MergedCell != null) cell = cell.MergedCell;
			if(cell != null) {
				cell.CellValue = GetRowCellValue(rowHandle, column);
				cell.ViewInfo = null;
				cell.State = GridRowCellState.Dirty;
				ViewInfo.PaintAnimatedItems = false;
				InvalidateRect(cell.Bounds);
			}
		}
		public virtual void InvalidateRowCell(int rowHandle, GridColumn column) {
			if(column == null) return;
			GridCellInfo cell = ViewInfo.GetGridCellInfo(rowHandle, column);
			if(cell != null && cell.MergedCell != null) cell = cell.MergedCell;
			if(cell != null) {
				ViewInfo.PaintAnimatedItems = false;
				InvalidateRect(cell.Bounds);
			}
		}
		public virtual void InvalidateRows() {
			if(!IsViewInfoReady) return;
			ViewInfo.PaintAnimatedItems = false;
			InvalidateRect(ViewInfo.ViewRects.Rows);
		}
		public bool IsGroupRow(int rowHandle) {
			if(IsFilterRow(rowHandle)) return false;
			return DataController.IsGroupRowHandle(rowHandle);
		}
		public bool IsFilterRow(int rowHandle) {
			return rowHandle == GridControl.AutoFilterRowHandle;
		}
		protected internal virtual bool AllowMasterDetail { 
			get { 
				return !IsPixelScrolling && OptionsDetail.EnableMasterViewMode && !WorkAsLookup && !IsSplitView; 
			} 
		}
		public bool IsMasterRow(int rowHandle) {
			if(rowHandle == GridControl.AutoFilterRowHandle) return false;
			return AllowMasterDetail &&
				(DataController != null && DataController.IsDetailRow(rowHandle));
		}
		protected internal bool IsMasterRowEmptyCached(int rowHandle) {
			return AllowMasterDetail &&
				DataController.IsDetailRowEmptyCached(rowHandle, CalcDefaultRelationIndex(rowHandle));
		}
		protected internal bool IsAllMasterRowEmptyCached(int rowHandle) {
			return AllowMasterDetail &&
				DataController.IsDetailRowEmptyCached(rowHandle, -1);
		}
		public bool IsMasterRowEmpty(int rowHandle) {
			return AllowMasterDetail &&
				IsMasterRowEmptyEx(rowHandle, CalcDefaultRelationIndex(rowHandle));
		}
		public virtual bool IsMasterRowEmptyEx(int rowHandle, int relationIndex) {
			if(DesignMode) return true;
			return DataController != null && DataController.IsDetailRowEmpty(rowHandle, relationIndex);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RefreshDetailExpandButton(int rowHandle) {
			if(DataController.DetailEmptyHash.ContainsKey(rowHandle))
				DataController.DetailEmptyHash.Remove(rowHandle);
			LayoutChangedSynchronized();
		}
		public RowVisibleState IsRowVisible(int rowHandle) {
			GridRowInfo ri = ViewInfo.GetGridRowInfo(rowHandle);
			if(ri == null || ri.TotalBounds.Height < 1) return RowVisibleState.Hidden;
			if(ri.TotalBounds.Bottom > ViewInfo.ViewRects.Rows.Bottom || ri.Bounds.Bottom > ViewInfo.ViewRects.Rows.Bottom)
				return RowVisibleState.Partially;
			if(IsFilterRow(rowHandle)) return RowVisibleState.Visible;
			if(IsNewItemRow(rowHandle) && OptionsView.NewItemRowPosition == NewItemRowPosition.Top) return RowVisibleState.Visible;
			if(!ri.ForcedRow) {
				Rectangle scrollableBounds = Scroller.ScrollInfo.GetScrollableBounds(ViewInfo);
				if(ri.TotalBounds.Top < scrollableBounds.Top) return RowVisibleState.Partially;
			}
			return RowVisibleState.Visible;
		}
		protected bool CheckDetailLayout() {
			if(ViewRect.IsEmpty) {
				if(IsLevelDefault) {
					if(CanSynchronized) OnViewPropertiesChanged(SynchronizationMode.Visual);
					return true;
				}
				if(ViewInfo.ViewRects.Bounds.IsEmpty) {
					return true;
				}
			}
			return false;
		}
		protected virtual bool CalculateDataCore() {
			if(!CanCalculateLayout) return false;
			ViewInfo.AutoRowHeightCache.Clear();
			this.calculatedRealViewHeight = -1;
			RefreshRows(false, false);
			ViewInfo.UpdateFooterDrawInfo();
			UpdateScrollBars();
			ViewInfo.IsDataDirty = false;
			CheckTopRowIndex();
			return true;
		}
		protected override bool CalculateData() {
			EnsureRuleValueProviders();
			if(IsEditing && EditingCell != null) { 
					GridCellInfo editingCell = EditingCell;
					if(!CalculateDataCore()) return false;
					GridCellInfo cell = ViewInfo.GetGridCellInfo(editingCell.RowHandle, editingCell.Column);
					if(cell == null || cell.Bounds != editingCell.Bounds) {
						if(IsFilterRow(FocusedRowHandle) && IsServerMode) {
							if(cell != null) {
								UpdateActiveEditorBounds();
								return true;
							}
						}
						HideEditor();
					}
					return true;
			}
			HideEditor();
			return CalculateDataCore();
		}
		protected override bool CheckCalculateLayout() { 
			ViewInfo.AutoRowHeightCache.Clear();
			if(CheckDetailLayout()) return true;
			return false; 
		}
		internal int SafeVisibleCount { get { return DataController.IsReady && !DataController.IsUpdateLocked && !IsDisposing ? RowCount : 0; } }
		int lastVisibleCount = 0;
		protected override void OnBeginUpdate() {
			this.lastVisibleCount = SafeVisibleCount;
			base.OnBeginUpdate();
		}
		protected override void OnEndUpdate() {
			base.OnEndUpdate();
			if(lastVisibleCount != SafeVisibleCount) SetTopRowIndexDirty();
		}
		public override void LayoutChanged() {
			if(!CalculateLayout()) return;
			if(this.requireCheckTopRowIndex) {
				this.requireCheckTopRowIndex = false;
				CheckTopRowIndex();
			}
			if(CustomizationForm != null) {
				SaveCustomizationFormBounds();
				CustomizationForm.CheckAndUpdate();
			}
			base.LayoutChanged();
		}
		protected internal override void ClearMasterCache() {
			base.ClearMasterCache();
			calculatedRealViewHeight = -1;
		}
		public virtual void MakeColumnVisible(GridColumn column) {
			if(column.VisibleIndex < 0 || (OptionsView.ColumnAutoWidth && !AllowScrollAutoWidth)) return;
			if(!ViewInfo.IsReady && ViewInfo.ViewRects.Bounds.IsEmpty) return;
			GridColumnInfoArgs ci = ViewInfo.ColumnsInfo[column];
			if(ci != null && (ViewInfo.IsFixedLeftPaint(ci) || ViewInfo.IsFixedRightPaint(ci))) return;
			if(column.Fixed != FixedStyle.None) return;
			int colLeft = ViewInfo.GetColumnLeftCoord(column),
				colRight, maxRight, minLeft = 0;
			if(ViewInfo.HasFixedLeft) {
				minLeft = (ViewInfo.ViewRects.FixedLeft.Right - ViewInfo.ViewRects.IndicatorWidth) - ViewInfo.ViewRects.Rows.Left;
				colLeft -= (minLeft - FixedLineWidth);
			} else {
				minLeft = 0;
			}
			colRight = colLeft + column.VisibleWidth;
			if(colLeft < LeftCoord) {
				LeftCoord = colLeft;
				return;
			}
			maxRight = ViewInfo.ViewRects.Rows.Width - minLeft - ViewInfo.ViewRects.IndicatorWidth - 1;
			if(ViewInfo.HasFixedRight)
				maxRight = (ViewInfo.ViewRects.FixedRight.Left - ViewInfo.ViewRects.Rows.Left)- FixedLineWidth - ViewInfo.ViewRects.IndicatorWidth - minLeft;
			if(colRight > LeftCoord + maxRight) {
				if(colRight - colLeft >= maxRight)
					LeftCoord = colLeft;
				else
					LeftCoord = colLeft - maxRight + column.VisibleWidth;
			}
		}
		public void MakeRowVisible(int rowHandle) {
			if(IsInitialized && !ViewInfo.IsReady) LayoutChanged();
			MakeRowVisible(rowHandle, false);
		}
		public void MakeRowVisible(int rowHandle, bool invalidate) {
			MakeRowVisibleCore(rowHandle, invalidate);
		}
		int lockMakeRowVisible = 0;
		protected override void MakeRowVisibleCore(int rowHandle, bool invalidate) {
			if(rowHandle == GridControl.InvalidRowHandle || lockMakeRowVisible != 0) return;
			Scroller.OnMakeRowVisibleCore(rowHandle, invalidate);
		}
		int postingEditorValue = 0;
		protected internal override bool PostEditor(bool causeValidation) {
			if(this.postingEditorValue != 0) return true;
			try {
				this.postingEditorValue ++;
				if(ActiveEditor == null || !EditingValueModified || this.fEditingCell == null) return true;
				if((causeValidation && !ValidateEditor()) || this.fEditingCell == null) return false;
				object value = ExtractEditingValue(this.fEditingCell.ColumnInfo.Column, EditingValue);
				SetRowCellValueCore(this.fEditingCell.RowHandle, this.fEditingCell.ColumnInfo.Column, value, true);
			}
			finally {
				this.postingEditorValue --;
			}
			return true;
		}
		protected virtual BaseView GetLevelDefault(int rowHandle, int relationIndex, out string levelName) {
			levelName = GetRelationName(rowHandle, relationIndex);
			GridLevelNode level = GetLevelNode();
			BaseView view = level != null ? level.GetChildTemplate(levelName) : null;
			MasterRowGetLevelDefaultViewEventHandler handler = (MasterRowGetLevelDefaultViewEventHandler)this.Events[masterRowGetLevelDefaultView];
			if(handler != null) {
				MasterRowGetLevelDefaultViewEventArgs e = new MasterRowGetLevelDefaultViewEventArgs(rowHandle, relationIndex, view);
				handler(this, e);
				view = e.DefaultView;
			}
			return view;
		}
		protected internal override IList GetChildDataView(int rowHandle, int relationIndex) {
			IList res = base.GetChildDataView(rowHandle, relationIndex);
			MasterRowGetChildListEventHandler handler = (MasterRowGetChildListEventHandler)this.Events[masterRowGetChildList];
			if(handler != null) {
				MasterRowGetChildListEventArgs e = new MasterRowGetChildListEventArgs(rowHandle, relationIndex, res);
				handler(this, e);
				res = e.ChildList;
			}
			return res;
		}
		public virtual int GetRelationIndex(int rowHandle, string relationName) {
			return DataController.GetRelationIndex(rowHandle, relationName);
		}
		public virtual string GetRelationName(int rowHandle, int relationIndex) {
			return DataController.GetRelationName(rowHandle, relationIndex);
		}
		public virtual string GetRelationDisplayName(int rowHandle, int relationIndex) {
			return DataController.GetRelationDisplayName(rowHandle, relationIndex);
		}
		public virtual int GetRelationCount(int rowHandle) {
			if(DataController == null) return 0;
			return DataController.GetRelationCount(rowHandle);
		}
		protected internal override void ClearDataProperties() {
			base.ClearDataProperties();
			GroupSummary.Clear();
		}
		protected internal override Color BackColor {
			get {
				if(PaintAppearance.Empty.BackColor == Color.Empty) return SystemColors.Control;
				return Color.FromArgb(255, PaintAppearance.Empty.BackColor);
			}
		}
		protected override string DataBoundSelectionField { get { return OptionsSelection.CheckBoxSelectorField; } }
		protected bool UseBoundCheckBoxSelector {  get { return !string.IsNullOrEmpty(OptionsSelection.CheckBoxSelectorField); } }
		internal const string PreviewUnboundColumnName = "_XGridPreviewColumn1999";
		protected override void PopulateCustomUnboundColumns(UnboundColumnInfoCollection unboundCollection) {
			if(!IsServerMode && OptionsView.ShowPreview) {
				unboundCollection.Add(new UnboundColumnInfo(PreviewUnboundColumnName, UnboundColumnType.String, true, "", false));
			}
			if(CheckboxSelectorColumn != null && !UseBoundCheckBoxSelector) unboundCollection.Add(new UnboundColumnInfo(CheckBoxSelectorColumnName, UnboundColumnType.Boolean, false, "", false));
		}
		protected override object GetUnboundDataCore(int listSourceRow1, DataColumnInfo column, object value) {
			if(column.Name == PreviewUnboundColumnName) {
				return GetRowPreviewDisplayTextCore(listSourceRow1, GetRowHandle(listSourceRow1));
			}
			if(column.Name == CheckBoxSelectorColumnName) {
				return GetCheckboxSelectorValue(listSourceRow1);
			}
			return base.GetUnboundDataCore(listSourceRow1, column, value);
		}
		protected override List<IDataColumnInfo> GetFindToColumnsCollection() {
			List<IDataColumnInfo> res = base.GetFindToColumnsCollection();
			if(OptionsFind.SearchInPreview && OptionsView.ShowPreview) {
				if(IsServerMode) {
					if(string.IsNullOrEmpty(PreviewFieldName)) return res;
					if(!GridColumnIDataColumnInfoWrapper.Contains(res, PreviewFieldName)) {
						res.Add(new GridViewPreviewIDataColumnInfoWrapper(this));
					}
					return res;
				}
				res.Add(new GridViewPreviewIDataColumnInfoWrapper(this));
			}
			return res;
		}
		protected internal override void SetupMasterSplitElements() {
			base.SetupMasterSplitElements();
			PropertyCache["ShowFooter"] = OptionsView.ShowFooter;
			PropertyCache["ShowFilterPanelMode"] = OptionsView.ShowFilterPanelMode;
			if(IsSplitSynchronizeViews) {
				if(IsVerticalSplit) {
					OptionsView.ShowFooter = false;
					OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
				}
			}
			PropertyCache["ShowColumnHeaders"] = OptionsView.ShowColumnHeaders;
			PropertyCache["HorzScrollVisibility"] = HorzScrollVisibility;
			PropertyCache["VertScrollVisibility"] = VertScrollVisibility;
			if(IsSplitSynchronizeScrolling) {
				if(IsVerticalSplit) {
					OptionsView.ShowColumnHeaders = true;
					HorzScrollVisibility = ScrollVisibility.Never;
				}
				else {
					VertScrollVisibility = ScrollVisibility.Never;
				}
			}
		}
		protected internal override void SetupChildSplitElements(BaseView master) {
			base.SetupChildSplitElements(master);
			GridView masterView = (GridView) master;
			if(IsSplitSynchronizeViews) {
				if(IsVerticalSplit) {
					OptionsView.ShowGroupPanel = false;
					OptionsView.ShowFooter = masterView.OptionsView.ShowFooter;
					OptionsView.NewItemRowPosition = NewItemRowPosition.None;
					OptionsView.ShowAutoFilterRow = false;
					OptionsFind.AlwaysVisible = false;
				} 
			}
			if(IsSplitSynchronizeScrolling) {
				if(IsVerticalSplit) {
					OptionsView.ShowColumnHeaders = false;
					HorzScrollVisibility = ScrollVisibility.Auto;
					LeftCoord = masterView.LeftCoord;
				} else {
					VertScrollVisibility = ScrollVisibility.Auto;
				}
			}
			HideFindPanel();
		}
		protected internal override void RestoreMasterSplitElements() {
			if(PropertyCache.ContainsKey("ShowFilterPanelMode")) OptionsView.ShowFilterPanelMode = (ShowFilterPanelMode)PropertyCache["ShowFilterPanelMode"];
			if(PropertyCache.ContainsKey("ShowFooter")) OptionsView.ShowFooter = (bool) PropertyCache["ShowFooter"];
			if(PropertyCache.ContainsKey("ShowColumnHeaders")) OptionsView.ShowColumnHeaders = (bool) PropertyCache["ShowColumnHeaders"];
			if(PropertyCache.ContainsKey("HorzScrollVisibility")) HorzScrollVisibility = (ScrollVisibility)PropertyCache["HorzScrollVisibility"];
			if(PropertyCache.ContainsKey("VertScrollVisibility")) VertScrollVisibility = (ScrollVisibility)PropertyCache["VertScrollVisibility"];
		}
		protected internal virtual BaseView CreateLevelDefault(int rowHandle, int relationIndex, out BaseView defaultView, out string levelName) {
			levelName = string.Empty;
			defaultView = null;
			BaseView newGV;
			defaultView = GetLevelDefault(rowHandle, relationIndex, out levelName); 
			bool copyEvents = true;
			if(defaultView != null) 
				newGV = defaultView.CreateInstance();
			else
				newGV = this.CreateInstance();
			if(defaultView == null) {
				defaultView = this;
				copyEvents = false;
			}
			newGV.BeginUpdate();
			try {
				newGV.Assign(defaultView, copyEvents);
				if(!copyEvents) newGV.ClearDataProperties();
			}
			finally {
				newGV.CancelUpdate();
			}
			return newGV;
		}
		protected internal override void ApplyLevelDefaults(BaseView newGV, BaseView defaultView, DetailInfo di, string levelName) {
			base.ApplyLevelDefaults(newGV, defaultView, di, levelName);
			if(defaultView == this) {
				GridView mgv = newGV as GridView;
				mgv.Columns.DestroyColumns();
				mgv.OptionsView.ShowGroupPanel = false;
			} else {
				GridView mgv = newGV as GridView;
			}
			newGV.DataController.AllowIEnumerableDetails = DataController.AllowIEnumerableDetails;
			newGV.DataController.SetDataSource(GridControl.BindingContext, di.DetailList, null);
			if(newGV is ColumnView) {
				ColumnView cgv = newGV as ColumnView;
				if(cgv.Columns.Count == 0) {
					cgv.PopulateColumns();
				}
			}
			if(newGV != null) newGV.Reload();
			if(newGV.TabControl != null) newGV.TabControl.Populate();
		}
		public void CollapseMasterRow(int rowHandle) { CollapseMasterRow(rowHandle, -1); }
		public void CollapseMasterRow(int rowHandle, int relationIndex) {
			if(DataController == null) return;
			if(relationIndex >= GetRelationCount(rowHandle)) return;
			if(!IsMasterRow(rowHandle)) return;
			HideDetailTip();
			bool expanded = GetMasterRowExpandedEx(rowHandle, relationIndex);
			if(!expanded) return;
			if(!CanCollapseMasterRowEx(rowHandle, relationIndex)) return;
			BeginUpdate();
			try {
				if(relationIndex == -1) DataController.CollapseDetailRow(rowHandle);
				else {
					DataController.CollapseDetailRow(rowHandle, relationIndex);
					UpdateActiveDetailView(rowHandle);
				}
			}
			finally {
				EndUpdateCore(true);
			}
			RaiseMasterRowCollapsed(rowHandle, relationIndex);
		}
		protected void UpdateActiveDetailView(int rowHandle) {
			BaseView bv = GetVisibleDetailView(rowHandle);
			if(bv != null) {
				if(bv.TabControl != null) bv.TabControl.Populate();
				if(OptionsDetail.AutoZoomDetail && OptionsDetail.AllowZoomDetail && (bv.IsAllowZoomDetail || bv.ViewRect.X == -10000)) { 
					bv.ZoomView(this);
				}
			}
		}
		internal int CalcDefaultRelationIndex(int rowHandle) {
			GridDetailInfo[] details = GetDetailInfo(rowHandle, true);
			if(details == null || details.Length == 0) return 0;
			GridDetailInfo detail = details[Math.Min(DefaultRelationIndex, details.Length - 1)];
			return detail.RelationIndex;
		}
		internal GridDetailInfo[] GetDetailInfo(int rowHandle, bool shortInfo) {
			if(!AllowMasterDetail || GridControl == null) return null;
			GridLevelNode levelNode = GetLevelNode();
			if(GridControl.ShowOnlyPredefinedDetails && (levelNode == null || !levelNode.HasChildren)) return null;
			ArrayList details = new ArrayList();
			if(GridControl.ShowOnlyPredefinedDetails) {
				foreach(GridLevelNode node in levelNode.Nodes) {
					int id = GetRelationIndex(rowHandle, node.RelationName);
					if(id < -1) continue;
					details.Add(new GridDetailInfo(id, node.RelationName, GetRelationDisplayName(rowHandle, id)));
				}
			} else {
				int count = GetRelationCount(rowHandle);
				for(int n = 0; n < count; n++) {
					string name = GetRelationName(rowHandle, n);
					details.Add(new GridDetailInfo(n, name, shortInfo ? name : GetRelationDisplayName(rowHandle, n)));
				}
			}
			if(details.Count == 0) return null;
			return details.ToArray(typeof(GridDetailInfo)) as GridDetailInfo[];
		}
		public void ExpandMasterRow(int rowHandle, string relationName) { SetMasterRowExpanded(rowHandle, relationName, true); }
		public void ExpandMasterRow(int rowHandle) { ExpandMasterRow(rowHandle, -1); }
		public void ExpandMasterRow(int rowHandle, int relationIndex) { SetMasterRowExpandedEx(rowHandle, relationIndex, true); }
		public void CollapseMasterRow(int rowHandle, string relationName) { SetMasterRowExpanded(rowHandle, relationName, false); }
		public void SetMasterRowExpanded(int rowHandle, string relationName, bool expand) {
			int index = GetRelationIndex(rowHandle, relationName);
			if(index < -1) return;
			SetMasterRowExpandedEx(rowHandle, index, expand);
		}
		public void SetMasterRowExpanded(int rowHandle, bool expand) {
			SetMasterRowExpandedEx(rowHandle, -1, expand);
		}
		public void SetMasterRowExpandedEx(int rowHandle, int relationIndex, bool expand) {
			CheckLoaded();
			if(IsNewItemRow(rowHandle)) return;
			if(!expand) {
				CollapseMasterRow(rowHandle, relationIndex);
				return;
			}
			if(DataController == null) return;
			if(relationIndex >= GetRelationCount(rowHandle)) return;
			if(!IsMasterRow(rowHandle)) return;
			HideDetailTip();
			CloseEditor();
			UpdateCurrentRowAction();
			bool collapseBeforeExpand = false;
			bool switchKeyboardFocusedView = false;
			if(relationIndex == -1) {
				if(GetMasterRowExpanded(rowHandle) == expand) return;
			}
			else {
				if(GetMasterRowExpandedEx(rowHandle, relationIndex) == expand) {
					MasterRowInfo rowInfo = DataController.GetRowDetailInfo(rowHandle);
					if(rowInfo == null) return;
					DetailInfo detailInfo = rowInfo.FindDetail(-1);
					if(detailInfo == null || detailInfo.RelationIndex == relationIndex) return;
					if(!CanExpandMasterRowEx(rowHandle, relationIndex)) return;
					BaseView prev = GetVisibleDetailView(rowHandle);
					if(prev != null) {
						switchKeyboardFocusedView = (GridControl != null && GridControl.FocusedView == prev);
						prev.CloseEditor();
						if(!prev.UpdateCurrentRow()) {
							prev.CheckTabPage();
							return;
						}
						prev.InternalSetViewRectCore(new Rectangle(BaseViewInfo.EmptyPoint, prev.ViewRect.Size));
					}
					BeginUpdate();
					try {
						this.AllowSynchronization = false;
						rowInfo.MakeDetailVisible(relationIndex);
						UpdateActiveDetailView(rowHandle);
						if(OptionsDetail.SmartDetailExpand) {
							Size size = ViewInfo.CalcDetailRowSize(rowHandle);
							CheckDetailVisibility(rowHandle, size);
						}
					}
					finally {
						EndUpdateCore(true);
						this.AllowSynchronization = true;
					}
					if(switchKeyboardFocusedView) {
						GridControl.FocusedView = GetVisibleDetailView(rowHandle);
					}
					RaiseMasterRowExpanded(rowHandle, relationIndex);
					return;
				}
				if(expand && GetMasterRowExpanded(rowHandle)) {
					collapseBeforeExpand = true;
				}
			}
			bool isCalled = false;								   
			bool res = false;										
			if(relationIndex == -1) {
				relationIndex = CalcDefaultRelationIndex(rowHandle);
				isCalled = true;									  
				res = CanExpandMasterRowEx(rowHandle, relationIndex); 
				if(!res) {
					bool found = false;
					int rc = GetRelationCount(rowHandle);
					for(int n = 0; n < rc; n++) {
						if(n == relationIndex) continue;
						res = CanExpandMasterRowEx(rowHandle, n);
						if(res) {
							found = true;
							relationIndex = n;
							break;
						}
					}
					if(!found) return;
				}
			}
			if(isCalled && !res) return;
			if(!isCalled && !CanExpandMasterRowEx(rowHandle, relationIndex)) return;
			BaseView newGV = null;
			try {
				if(collapseBeforeExpand) { 
					BeginUpdate();
					BaseView oldV = GetVisibleDetailView(rowHandle);
					if(oldV != null) {
						oldV.CloseEditor();
						try {
							CancelUpdate();
							if(!oldV.UpdateCurrentRow()) {
								oldV.CheckTabPage();
								return;
							}
						} finally {
							BeginUpdate();
						}
						oldV.InternalSetViewRectCore(new Rectangle(BaseViewInfo.EmptyPoint, oldV.ViewRect.Size));
						switchKeyboardFocusedView = (GridControl != null && oldV == GridControl.FocusedView);
					}
				}
					if(OptionsDetail.AllowOnlyOneMasterRowExpanded)
						CollapseAllDetails();
					DetailInfo di = DataController.ExpandDetailRow(rowHandle, relationIndex);
					if(di == null) return;
					BaseView defaultView;
					string levelName;
					newGV = CreateLevelDefault(rowHandle, relationIndex, out defaultView, out levelName);
					newGV.BeginUpdate();
					di.DetailOwner = newGV;
					di.MasterRow.MakeDetailVisible(di);
					if(di.DetailList == null ) {
						DataController.CollapseDetailRow(rowHandle);
						newGV.Dispose();
						newGV = null;
					}
					else {
						ApplyLevelDefaults(newGV, defaultView, di, levelName);
						GridControl.RegisterView(newGV);
						newGV.OnDetailInitialized();
					} 
				if(newGV == null) DataController.CollapseDetailRow(rowHandle);
			}
			finally {
				if(collapseBeforeExpand) CancelUpdate();
				if(newGV != null) {
					newGV.CancelUpdate();
					if(OptionsDetail.AutoZoomDetail && 
						OptionsDetail.AllowZoomDetail && newGV.IsAllowZoomDetail)
						newGV.ZoomView();
					else {
						Size size = ViewInfo.CalcDetailRowSize(rowHandle);
						newGV.InternalSetViewRectCore(new Rectangle(BaseViewInfo.EmptyPoint, size));
						BaseView view = GridControl.DefaultView;
						bool oldAllowSynchronization = view.AllowSynchronization;
						this.AllowSynchronization = false;
						try {
							try {
								BeginUpdate();
								if(OptionsDetail.SmartDetailExpand) CheckDetailVisibility(rowHandle, size);
							}
							finally {
								EndUpdateCore(true);
							}
						}
						finally {
							this.AllowSynchronization = true;
							view.AllowSynchronization = oldAllowSynchronization;
						}
					}
					newGV.AllowSynchronization = true;
				}
				else
					LayoutChangedSynchronized();
			}
			if(newGV != null && switchKeyboardFocusedView && GridControl != null) GridControl.FocusedView = newGV;
			RaiseMasterRowExpanded(rowHandle, relationIndex);
		}
		int visualExpander = 0;
		protected internal virtual void VisualSetMasterRowExpandedEx(int rowHandle, int relationIndex, bool expand) {
			GridDetailInfo[] info = GetDetailInfo(rowHandle, true);
			if(info == null || info.Length == 0) return;
			if(relationIndex != -1) {
				bool found = false;
				foreach(GridDetailInfo di in info) {
					if(di.RelationIndex == relationIndex) {
						found = true;
						break;
					}
				}
				if(!found) return;
			}
			try {
				this.visualExpander ++;
				SetMasterRowExpandedEx(rowHandle, relationIndex, expand);
			}
			finally {
				this.visualExpander --;
			}
		}
		protected virtual void CheckDetailVisibility(int rowHandle, Size detailSize) {
			if(this.visualExpander == 0) return;
			if(ParentView != null && this != GridControl.DefaultView) return;
			RowVisibleState rv = IsRowVisible(rowHandle);
			if(rv == RowVisibleState.Hidden) return;
			GridRowInfo rowInfo = ViewInfo.GetGridRowInfo(rowHandle);
			if(rowInfo == null) return;
			int vIndex = GetVisibleIndex(rowHandle);
			if(TopRowIndex == vIndex) return;
			Rectangle rows = ViewInfo.ViewRects.Rows;
			int h = rows.Bottom - (rowInfo.Bounds.Bottom + detailSize.Height);
			if(h >= 0) return;
			int newTopIndex = rowInfo.VisibleIndex;
			int ix = ViewInfo.RowsInfo.IndexOf(rowInfo);
			for(int n = 0; n < ix; n++) {
				GridRowInfo ri = ViewInfo.RowsInfo[n];
				if(ri is GridDataRowInfo && ((GridDataRowInfo)ri).IsSpecialRow) continue;
				h+= ri.Bounds.Height;
				if(h >= 0) {
					if(n + 1 < ViewInfo.RowsInfo.Count) newTopIndex = ViewInfo.RowsInfo[n + 1].VisibleIndex;
					break;
				}
			}
			TopRowIndex = newTopIndex;
		}
		protected internal virtual GridViewScroller Scroller {
			get {
				if(scroller == null) scroller = CreateGridScroller();
				return scroller;
			}
		}
		protected virtual GridViewScroller CreateGridScroller() {
			if(IsPixelScrolling) return new GridViewByPixelScroller(this);
			return new GridViewByRowScroller(this);
		}
		public void CollapseGroupRow(int rowHandle) { CollapseGroupRow(rowHandle, false); }
		public void CollapseGroupRow(int rowHandle, bool recursive) { SetRowExpanded(rowHandle, false, recursive); }
		public void ExpandGroupRow(int rowHandle) { ExpandGroupRow(rowHandle, false); }
		public void ExpandGroupRow(int rowHandle, bool recursive) {
			SetRowExpanded(rowHandle, true, recursive);
		}
		public void CollapseGroupLevel(int groupLevel) { CollapseGroupLevel(groupLevel, false); }
		public void CollapseGroupLevel(int groupLevel, bool recursive) { SetGroupLevelExpanded(groupLevel, false, recursive); }
		public void ExpandGroupLevel(int groupLevel) { ExpandGroupLevel(groupLevel, false); }
		public void ExpandGroupLevel(int groupLevel, bool recursive) { SetGroupLevelExpanded(groupLevel, true, recursive); }
		public void SetGroupLevelExpanded(int groupLevel, bool expanded, bool recursive) {
			CheckLoaded();
			if(GroupCount == 0 || groupLevel >= GroupCount) return;
			if(expanded) {
				if(!RaiseGroupRowExpanding(InvalidRowHandle)) return;
				DataController.ExpandLevel(groupLevel, recursive);
				RaiseGroupRowExpanded(InvalidRowHandle);
			}
			else {
				if(!RaiseGroupRowCollapsing (InvalidRowHandle)) return;
				DataController.CollapseLevel(groupLevel, recursive);
				RaiseGroupRowCollapsed(InvalidRowHandle);
			}
			UpdateNavigator();
			SetTopRowIndexDirty();
			LayoutChangedSynchronized(); 
		}
		public void SetRowExpanded(int rowHandle, bool expanded, bool recursive) {
			CheckLoaded();
			if(!IsGroupRow(rowHandle)) return;
			if(expanded) {
				if(!RaiseGroupRowExpanding(rowHandle)) return;
				DataController.ExpandRow(rowHandle, recursive);
				RaiseGroupRowExpanded(rowHandle);
			}
			else {
				if(!RaiseGroupRowCollapsing(rowHandle)) return;
				DataController.CollapseRow(rowHandle, recursive);
				RaiseGroupRowCollapsed(rowHandle);
			}
			UpdateNavigator();
			SetTopRowIndexDirty();
			LayoutChangedSynchronized(); 
		}
		protected internal override void SetTopRowIndexDirty() {
			this.requireCheckTopRowIndex = true;
		}
		public void SetRowExpanded(int rowHandle, bool expanded) {
			if(GetRowExpanded(rowHandle) == expanded) return;
			SetRowExpanded(rowHandle, expanded, false);
		}
		public override void ShowEditorByMouse() {
			if(FilterPopup != null) DestroyFilterPopup();
			base.ShowEditorByMouse();
		}
		protected internal new GridHandler Handler { get { return (GridHandler)base.Handler; } }
		protected override bool CanSendMouseToEditor(BaseEdit editor) {
			GridCellInfo cell = ViewInfo.GetGridCellInfo(FocusedRowHandle, FocusedColumn);
			if(cell != null && cell.ViewInfo != null) {
				ButtonEditViewInfo bi = cell.ViewInfo as ButtonEditViewInfo;
				if(bi != null && bi.Item.Buttons.VisibleCount > 0) {
					bool allow = true;
					if(Handler.CellEditDownPointHitInfo != null) {
						if(Handler.CellEditDownPointHitInfo.HitTest == EditHitTest.Button || Handler.CellEditDownPointHitInfo.HitTest == EditHitTest.Button2)
							return true;
						var hitInfo = ((BaseEditViewInfo)editor.GetViewInfo()).CalcHitInfo(Handler.CellEditDownPointHitInfo.HitPoint);
						if(hitInfo.HitTest == EditHitTest.Button || hitInfo.HitTest == EditHitTest.Button2) return false;
					}
					return allow;
				}
			}
			return true;
		}
		public override void ShowEditor() {
			if(IsDisposing || !IsDefaultState || !GetCanShowEditor(FocusedColumn)) return;
			DestroyFilterPopup();
			CheckViewInfo();
			MakeColumnVisible(FocusedColumn);
			if(!Focus() && lockEditorUpdatingByMouse) return;
			if(!Scroller.ShowEditor()) return;
			GridCellInfo cell = ViewInfo.GetGridCellInfo(FocusedRowHandle, FocusedColumn);
			if(cell == null) return;
			if(AsyncServerModeDataController.IsNoValue(cell.CellValue)) return;
			ActivateEditor(cell);
		}
		public void UpdateColumnsCustomization() {
			if(CustomizationForm != null) {
				CustomizationForm.UpdateListBox();
			}
		}
		internal bool ShouldSerializeScrollStyle() { return ScrollStyle != DefaultScrollStyle; }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewScrollStyle"),
#endif
 Editor(typeof(AttributesEditor), typeof(UITypeEditor)),
		DXCategory(CategoryName.Behavior), XtraSerializableProperty()]
		public ScrollStyleFlags ScrollStyle {
			get { return scrollStyle; }
			set {
				if(ScrollStyle != value) {
					scrollStyle = value;
				}
			}
		}
		protected internal virtual bool DrawFullRowFocus {
			get {
				if(focusRectStyle == DrawFocusRectStyle.RowFullFocus) return true;
				return false;
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewFocusRectStyle"),
#endif
 DefaultValue(DrawFocusRectStyle.CellFocus), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public virtual DrawFocusRectStyle FocusRectStyle {
			get { return focusRectStyle; }
			set {
				if(FocusRectStyle == value) return;
				focusRectStyle = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewHorzScrollStep"),
#endif
 DefaultValue(3), XtraSerializableProperty(), DXCategory(CategoryName.Behavior)]
		public int HorzScrollStep {
			get { return horzScrollStep; }
			set {
				if(value < 1) value = 1;
				if(value > 100) value = 100;
				if(HorzScrollStep == value) return;
				horzScrollStep = value;
				OnPropertiesChanged();
			}
		}
		protected void OnHorzScroll(object sender, ScrollEventArgs e) {
			if((ScrollStyle & ScrollStyleFlags.LiveHorzScroll) != 0) return;
			switch(e.Type) {
				case ScrollEventType.EndScroll:
					LeftCoord = ScrollInfo.HScrollPosition;
					break;
			}
		}
		protected virtual string GetVertScrollTip(int visibleIndex) {
			int rowHandle = GetVisibleRowHandle(visibleIndex);
			string s = string.Empty;
			if(IsGroupRow(rowHandle)) {
				s = "Group: " + GetGroupRowDisplayText(rowHandle);
			} else {
				int fieldHandle = DataController.Columns.GetColumnIndex(VertScrollTipFieldName);
				string dt = string.Empty;
				if(fieldHandle != -1) {
					dt = DataController.GetRowDisplayText(rowHandle, fieldHandle);
				} else {
					dt = rowHandle.ToString();
					if(rowHandle == GridControl.InvalidRowHandle) {
						if(DataRowCount == 0) {
							s = "";
						} 
						else {
							dt = DataRowCount.ToString();
						}
					}
				}
				s = "DataRow: " + dt;
			}
			return s;
		}
		protected internal override BaseViewOfficeScroller CreateScroller() { return new GridViewOfficeScroller(this); }
		protected internal override void SetScrollingState() { 
			SetStateCore(GridState.Scrolling);
		}
		protected internal override bool IsScrollingState {
			get { return State == GridState.Scrolling; }
		}
		bool _isScrolling = false;
		protected bool IsScrolling { 
			get { return _isScrolling; }
			set {
				if(IsScrolling == value) return;
				this._isScrolling = value;
			}
		}
		protected virtual void OnVertScroll(object sender, ScrollEventArgs e) {
			if(IsPixelScrolling) {
				Scroller.CancelSmoothScroll();
			}
			if((ScrollStyle & ScrollStyleFlags.LiveVertScroll) != 0) return;
			switch(e.Type) {
				case ScrollEventType.EndScroll:
					this.IsScrolling = false;
					TopRowIndex = ScrollInfo.VScrollPosition;
					HideHint();
					break;
				default:
					this.IsScrolling = true;
					ShowHint(ScrollInfo.VScroll.PointToScreen(Point.Empty), ToolTipLocation.LeftBottom);
					break;
			}
		}
		protected virtual void OnHScroll(object sender, EventArgs e) {
			if((ScrollStyle & ScrollStyleFlags.LiveHorzScroll) == 0) return;
			try {
				GridControl.EditorHelper.BeginAllowHideException();
				LeftCoord = ScrollInfo.HScrollPosition;
				IncrementPaintScrollCounters();
			}
			catch(HideException) {
				UpdateHScrollBar();
			}
			finally {
				GridControl.EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnDataController_VisibleRowCountChanged(object sender, EventArgs e) {
			ViewInfo.PositionHelper.Check();
			if(IsInplaceEditFormVisible && !IsNewItemRow(FocusedRowHandle) &&
				( IsRowVisible(FocusedRowHandle) == RowVisibleState.Hidden || !DataController.IsRowVisible(FocusedRowHandle))) HideEditForm();
			base.OnDataController_VisibleRowCountChanged(sender, e);
		}
		bool lockVScroll = false;
		internal bool AllowFixedGroups { 
			get {
				if(OptionsBehavior.AllowFixedGroups == DefaultBoolean.False) return false;
				if(GroupCount == 0) return false;
				if(OptionsBehavior.AllowFixedGroups == DefaultBoolean.True) return true;
				if(IsServerMode) return true;
				return false;
			} 
		}
		protected override void CheckSelectFocusedRow() {
			if(IsCheckboxSelectorMode) return;
			base.CheckSelectFocusedRow();
		}
		internal void ClearSelectFocusedRow() {
			BeginSelection();
			try {
				ClearSelection();
				SelectFocusedRowCore();
			}
			finally {
				EndSelection();
			}
		}
		protected internal virtual bool IsColumnHeaderAutoHeight { get { return OptionsView.ColumnHeaderAutoHeight == DefaultBoolean.True; } }
		protected virtual float SwipeLeftZone { get { return 0.4f; } }
		protected virtual float SwipeRightZone { get { return 0.4f; } }
		protected virtual bool AllowSwipeRight { get { return false; } }
		protected virtual bool AllowSwipeLeft { get { return false; } }
		protected virtual bool AllowSwipeGestures { get { return OptionsView.ColumnAutoWidth && (AllowSwipeLeft || AllowSwipeRight); } }
		protected internal override GestureAllowArgs[] CheckAllowGestures(Point point) {
			GridHitInfo hitInfo = CalcHitInfo(point);
			if(hitInfo.InRow || hitInfo.HitTest == GridHitTest.EmptyRow) {
				int allow = GestureHelper.GC_PAN_ALL;
				if(OptionsView.ColumnAutoWidth) allow &= ~ GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY;
				if(AllowSwipeGestures) allow |= GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY;
				return new GestureAllowArgs[] {
					new GestureAllowArgs() { GID = GID.PAN, AllowID = allow},GestureAllowArgs.PressAndTap};
			}
			return GestureAllowArgs.None;
		}
		protected internal override void OnTouchScroll(GestureArgs info, Point delta, ref Point overPan) {
			if(AllowSwipeGestures) {
				CheckSwipeGesture(info, delta);
				delta.X = 0;
				overPan.X = 0;
			}
			Scroller.OnTouchScroll(info, delta, ref overPan);
		}
		bool? swipeStarted = null;
		Point swipeStart = new Point(int.MinValue, int.MinValue);
		int swipeRow = InvalidRowHandle;
		void CheckSwipeGesture(GestureArgs info, Point delta) {
			if(info.IsBegin) {
				swipeRow = InvalidRowHandle;
				GridHitInfo hitInfo = CalcHitInfo(info.Start.Point);
				if(hitInfo.InRow) {
					swipeRow = hitInfo.RowHandle;
					if(!OnSwipeStart(swipeRow)) {
						swipeStarted = null;
						swipeRow = InvalidRowHandle;
						return;
					}
				}
				swipeStarted = true;
				return;
			}
			if(info.IsEnd) {
				swipeRow = InvalidRowHandle;
				swipeStarted = null;
				return;
			}
			if(!swipeStarted.HasValue || !swipeStarted.Value) return;
			if(Math.Abs(info.Start.Y - info.Current.Y) < 50f) {
				float dx = info.Start.X - info.Current.X;
				if (dx > 0 && !SwipeLeftZoneBounds.Contains(info.Start.Point)) return;
				if (dx < 0 && !SwipeRightZoneBounds.Contains(info.Start.Point)) return;
				if (Math.Abs(dx) > SwipeDelta) {
					swipeStarted = false;
					if(dx > 0 && AllowSwipeLeft) OnSwipeLeft(swipeRow);
					if(dx < 0 && AllowSwipeRight) OnSwipeRight(swipeRow);
				}
			}
		}
		protected Rectangle SwipeRightZoneBounds { 
			get { return new Rectangle(ViewInfo.ViewRects.Rows.X, ViewInfo.ViewRects.Rows.Y, (int)(ViewInfo.ViewRects.Rows.Width * SwipeLeftZone), ViewInfo.ViewRects.Rows.Height); } 
		}
		protected Rectangle SwipeLeftZoneBounds {
			get {
				int w = (int)(ViewInfo.ViewRects.Rows.Width * SwipeRightZone);
				return new Rectangle(ViewInfo.ViewRects.Rows.Right - w, ViewInfo.ViewRects.Rows.Y, w, ViewInfo.ViewRects.Rows.Height); }
		}
		protected virtual bool OnSwipeStart(int swipeRow) {
			return true;
		}
		protected virtual void OnSwipeRight(int rowHandle) {
		}
		protected virtual void OnSwipeLeft(int rowHandle) {
		}
		const int SwipeDelta = 100;
		protected bool CheckCloseEditForm() {
			if(!IsInplaceEditFormVisible || (EditFormController.IsUpdatingValues && IsNewItemRow(FocusedRowHandle))) return true;
			if(!GetAllowCancelEditFormOnRowChange() || !AskModifiedCancelAndHide()) return false;
			return true;
		}
		void OnVScroll(object sender, EventArgs e) {
			if(lockVScroll) return;
			lockVScroll = true;
			try {
				if(IsInplaceEditFormVisible) {
					if(!CheckCloseEditForm()) {
						UpdateVScrollBar();
						return;
					}
				}
				if((ScrollStyle & ScrollStyleFlags.LiveVertScroll) == 0) return;
				try {
					GridControl.EditorHelper.BeginAllowHideException();
					Scroller.OnVScroll(ScrollInfo.VScrollPosition);
					IncrementPaintScrollCounters();
				}
				catch(HideException) {
					UpdateVScrollBar();
				}
				finally {
					GridControl.EditorHelper.EndAllowHideException();
				}
			}
			finally {
				lockVScroll = false;
			}
		}
		protected virtual int CalcHScrollableRange() {
			if(ViewInfo == null) return 0;
			return Math.Max(ViewInfo.ViewRects.ColumnPanelWidth - (ViewInfo.ViewRects.FixedRight.Width + ViewInfo.ViewRects.FixedLeft.Width), 0);
		}
		protected virtual int CalcHScrollRange() {
			if(ViewInfo == null) return 0;
			int res = ViewInfo.ViewRects.ColumnTotalWidth - (ViewInfo.ViewRects.FixedRight.Width + ViewInfo.ViewRects.FixedLeft.Width);
			if(ViewInfo.HasFixedLeft) res += FixedLineWidth;
			if(ViewInfo.HasFixedRight) res += FixedLineWidth;
			return res;
		}
		protected internal override void OnActionScroll(ScrollNotifyAction action) {
			if(!CanActionScroll(action)) return;
			if(action == ScrollNotifyAction.MouseMove) {
				var hit = ViewInfo.CalcHitInfo(PointToClient(Control.MousePosition));
				if(hit.HitTest == GridHitTest.RowDetail) return;
				if(ParentView != null) ParentView.OnActionScroll(ScrollNotifyAction.Hide);
			}
			ScrollInfo.OnAction(action);
		}
		void UpdateHScrollBar() {
			ScrollInfo.HScrollVisible = GetHScrollVisible();
			if(ScrollInfo.HScrollVisible) {
				ScrollArgs args = new ScrollArgs();
				args.FireEventsOnAssign = false;
				args.Maximum = CalcHScrollRange();
				args.Value = LeftCoord;
				args.SmallChange = HorzScrollStep;
				if(ViewInfo.ViewRects.ColumnPanelWidth > 0) 
					args.LargeChange = Math.Max(0, Math.Min(CalcHScrollableRange(), args.Maximum)); 
				else {
					args.LargeChange = 0;
				}
				args.Enabled = args.Maximum > args.LargeChange && (!OptionsView.ColumnAutoWidth || AllowScrollAutoWidth);
				args.Check();
				ScrollArgs active = ScrollInfo.HScrollArgs;
				if(!args.IsEquals(active)) {
					ScrollInfo.HScrollArgs = args;
				}
				if(ScrollInfo.HScrollArgs.Value != LeftCoord) {
					UpdateLeftCoord(ScrollInfo.HScrollArgs.Value);
				}
			}
		}
		protected override bool GetAllowHtmlDraw() {
			return OptionsView.AllowHtmlDrawGroups || OptionsView.AllowHtmlDrawHeaders;
		}
		protected internal override bool IsAnyDetails { 
			get {
				if(!OptionsDetail.EnableMasterViewMode) return false;
				if(GridControl == null) return false;
				if(GridControl.Views.Count > 1) return true;
				return false; 
			} 
		}
		bool GetHScrollVisible() { return IsVisible && ViewInfo.HScrollBarPresence == ScrollBarPresence.Visible; } 
		protected override void UpdateScrollBars() {
			int scrollTop = ViewInfo.ViewRects.VScrollLocation;
			Rectangle scBounds = ViewInfo.GetScrollableBounds(true);
			if(!ScrollInfo.IsOverlapScrollBar) scBounds.Y = scrollTop;
			if(scBounds.Top > scrollTop) scrollTop = scBounds.Top;
			Rectangle scrollBounds = ViewInfo.ViewRects.Scroll;
			scrollBounds.Height = (ViewInfo.ViewRects.HScrollLocation == 0 || ScrollInfo.IsOverlapHScrollBar ? scrollBounds.Bottom : ViewInfo.ViewRects.HScrollLocation) - scrollTop;
			scrollBounds.Y = scrollTop;
			if(!ViewInfo.FilterPanel.Bounds.IsEmpty && ScrollInfo.IsOverlapHScrollBar) {
				scrollBounds.Height = ViewInfo.FilterPanel.Bounds.Top - scrollTop;
			}
			ScrollInfo.ClientRect = scrollBounds;
			ScrollInfo.HScrollVisible = GetHScrollVisible();
			Rectangle vscrollBounds = new Rectangle(scrollBounds.Right - ScrollInfo.VScrollSize, scrollBounds.Y, ScrollInfo.VScrollSize, scrollBounds.Height);
			if(IsRightToLeft) vscrollBounds.X = scrollBounds.X;
			if(!ScrollInfo.IsOverlapHScrollBar) vscrollBounds.Height = scrollBounds.Bottom - vscrollBounds.Y - (ScrollInfo.HScrollVisible ? ScrollInfo.HScrollSize : 0);
			if(!ViewInfo.FilterPanel.Bounds.IsEmpty) {
				vscrollBounds.Height = ViewInfo.FilterPanel.Bounds.Y - scrollBounds.Y;
			}
			if(!ViewInfo.ViewRects.Footer.IsEmpty) {
				vscrollBounds.Height = ViewInfo.ViewRects.Footer.Y - scrollBounds.Y;
			}
			ScrollInfo.VScrollSuggestedBounds = vscrollBounds;
			UpdateVScrollBar();
			UpdateHScrollBar();
			ScrollInfo.UpdateScrollRects();
		}
		int ConvertIndexToScrollIndex(int index) {
			return DataController.GetVisibleIndexes().ConvertIndexToScrollIndex(index, AllowFixedGroups);
		}
		int GetScrollableRowCount() {
			if(!AllowFixedGroups) return RowCount;
			VisibleIndexCollection vic = DataController.GetVisibleIndexes();
			if(vic.IsEmpty) return RowCount;
			int res = vic.ScrollableIndexesCount;
			if(OptionsView.NewItemRowPosition == NewItemRowPosition.Bottom) res++;
			return res;
		}
		protected internal override void StartScrolling() {
			if(IsInplaceEditFormVisible) return;
			base.StartScrolling();
		}
		protected internal void DoMouseHWheelScroll(int delta) {
			LeftCoord += delta;
		}
		protected internal void DoMouseWheelScroll(MouseWheelScrollClientArgs e) {
			Scroller.OnMouseWheelScroll(e);
		}
		void UpdateVScrollBar() {
			if(IsPixelScrolling) {
				UpdateVScrollBarPixel();
				return;
			}
			ScrollInfo.VScrollVisible = IsVisible && ViewInfo.VScrollBarPresence == ScrollBarPresence.Visible; 
			if(!ScrollInfo.VScrollVisible) return;
			ScrollArgs args = new ScrollArgs();
			args.FireEventsOnAssign = false;
			bool isLastRowVisible = RowCount > 0 && IsRowVisible(GetVisibleRowHandle(RowCount - 1)) == RowVisibleState.Visible;
			int scrollableRowsCount = ViewInfo.RowsInfo.GetScrollableRowsCount(this);
			bool isLastRowOnPageVisiblePartially = IsRowVisible(GetVisibleRowHandle(ViewInfo.RowsInfo.GetLastVisibleRowIndex())) == RowVisibleState.Partially;
			args.Maximum = Math.Max(0, GetScrollableRowCount() + (isLastRowVisible ? -1 : 0));
			bool forceEnable = false;
			if(!OptionsBehavior.SmartVertScrollBar) {
				args.Maximum += Math.Max(0, Math.Min(1, scrollableRowsCount - 1));
			} else {
				if(isLastRowVisible) {
					int lastRowHandle = GetVisibleRowHandle(RowCount - 1);
					if(GetMasterRowExpanded(lastRowHandle)) {
						if(scrollableRowsCount > 1) {
							forceEnable = true;
							GridDataRowInfo ri = ViewInfo.GetGridRowInfo(lastRowHandle) as GridDataRowInfo;
							if(ri != null && ri.DetailBounds.Bottom + ViewInfo.DetailVertIndent + 1 > ViewInfo.ViewRects.Rows.Bottom) args.Maximum++;
						}
					}
				}
			}
			if(VertScrollVisibility == ScrollVisibility.Always) {
				args.Enabled = TopRowIndex != 0 || !isLastRowVisible || forceEnable;
			}
			args.Value = ConvertIndexToScrollIndex(TopRowIndex);
			args.LargeChange = Math.Max(Math.Min(args.Maximum, scrollableRowsCount - (isLastRowOnPageVisiblePartially ? 1 : 0)), 1);
			ScrollInfo.VScrollArgs = args;
		}
		void UpdateVScrollBarPixel() {
			ScrollInfo.VScrollVisible = IsVisible && ViewInfo.VScrollBarPresence == ScrollBarPresence.Visible; 
			if(!ScrollInfo.VScrollVisible) return;
			ScrollArgs args = new ScrollArgs();
			int pageSize = ViewInfo.GetScrollableBounds(true).Height;
			args.FireEventsOnAssign = false;
			args.Maximum = ViewInfo.VisibleRowsHeight;
			args.Value = TopRowPixel;
			args.SmallChange = Math.Min(10, ViewInfo.MinRowHeight); 
			args.LargeChange = pageSize;
			ScrollInfo.VScrollArgs = args;
		}
		public void InvokeMenuItemClick(GridMenuItemClickEventArgs e) {
			GridMenuItemClickEventHandler handler = (GridMenuItemClickEventHandler)this.Events[gridMenuItemClick];
			if(handler != null) handler(this, e);
		}
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
		protected internal bool DoShowGridMenu(GridViewMenu menu, GridHitInfo hitInfo) {
			return DoShowGridMenu(new GridMenuEventArgs(menu, hitInfo));
		}
		protected internal bool DoShowGridMenu(GridMenuType menuType, GridViewMenu menu, GridHitInfo hitInfo, bool allow) {
			return DoShowGridMenu(new GridMenuEventArgs(menuType, menu, hitInfo, allow));
		}
		protected internal virtual bool DoShowGridMenu(GridMenuEventArgs e) {
			RaisePopupMenuShowing(e);
			RaiseShowGridMenu(e);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
			if (e.Menu == null || !e.Allow || e.Menu.Items.Count == 0) return e.Allow;
			e.Menu.Show(e.Point);
			return true;
		}
		protected virtual void CheckMinColumnWidthes() {
			int colCount = VisibleColumns.Count;
			for(int i = 0; i < colCount; i++) {
				GridColumn col = GetVisibleColumn(i);
				int minWidth = GetColumnMinWidth(col);
				if(col.VisibleWidth < minWidth) 
					col.visibleWidth = minWidth;
				if(!OptionsView.ColumnAutoWidth) {
					if(col.Width < minWidth) 
						col.InternalSetWidth(minWidth);
				}
			}
		}
		protected internal virtual void CheckTopRowIndex() {
			if(IsPixelScrolling) {
				TopRowPixel = CheckTopRowPixel(TopRowPixel);
			}
			else {
				TopRowIndex = CheckTopRowIndex(TopRowIndex);
			}
		}
		int CheckTopRowPixelCore(int newTopRowPixel) {
			if(newTopRowPixel < 0) newTopRowPixel = 0;
			int pageSize = ViewInfo.GetScrollableBounds(false).Height;
			int maximum = ViewInfo.VisibleRowsHeight;
			int scrollable = Math.Max(0, ViewInfo.VisibleRowsHeight - pageSize);
			if(newTopRowPixel > scrollable) return scrollable;
			return newTopRowPixel;
		}
		protected internal virtual int CheckTopRowPixel(int newTopRowPixel) {
			if(!OptionsBehavior.SmartVertScrollBar ||
				newTopRowPixel == 0)
				return newTopRowPixel;
			return CheckTopRowPixelCore(newTopRowPixel);
		}
		protected internal virtual int CheckTopRowIndex(int newTopRowIndex) {
			int res = CheckTopRowIndex2(newTopRowIndex);
			return res;
		}
		internal bool EditFormAllowAdjustTopRowIndex = true;
		protected virtual bool AllowAdjustTopRowIndexByEditForm {
			get {
				return (ViewInfo.EditFormRequiredHeight > 0 && EditFormAllowAdjustTopRowIndex);
			}
		}
		protected internal virtual int CheckTopRowIndex2(int newTopRowIndex) {
			if(!OptionsBehavior.SmartVertScrollBar ||
				newTopRowIndex == 0 || AllowAdjustTopRowIndexByEditForm)
				return newTopRowIndex;
			int startTopRowIndex = newTopRowIndex;
			int maxHeight = ViewInfo.ViewRects.Rows.Height,
				prevRowsHeight = -1;
			Graphics g = GridControl.CreateGraphics();
			try {
				GridRowsLoadInfo rInfo = new GridRowsLoadInfo(g, newTopRowIndex, maxHeight, true, true);
				ViewInfo.LoadRows(rInfo);
				if(rInfo.ResultAllRowsFit) rInfo = null;
				for(;;) {
					if(newTopRowIndex < 0) {
						newTopRowIndex = 0;
						break;
					}
					if(prevRowsHeight != -1 || rInfo == null) {
						rInfo = new GridRowsLoadInfo(g, newTopRowIndex, maxHeight, true, false);
						ViewInfo.LoadRows(rInfo);
					}
					if(startTopRowIndex > newTopRowIndex && rInfo.ResultRowsHeight > maxHeight && newTopRowIndex + rInfo.ResultRowCount == RowCount) {
						newTopRowIndex ++;
						break;
					}
					if(prevRowsHeight != -1 && prevRowsHeight < maxHeight && (!rInfo.ResultAllRowsFit || rInfo.ResultRowsHeight > maxHeight)) {
						newTopRowIndex ++;
						break;
					}
					if(rInfo.ResultRowsHeight < maxHeight)
						newTopRowIndex --;
					else 
						break;
					prevRowsHeight = rInfo.ResultRowsHeight;
				}
			}
			finally {
				g.Dispose();
			}
			this.allowUpdateRowIndexes = TopRowIndex == newTopRowIndex;
			return newTopRowIndex;
		}
		protected override bool UpdateCurrentRowCore(bool force) {
			if(IsFilterRow(FocusedRowHandle)) return true;
			return base.UpdateCurrentRowCore(force);
		}
		protected internal override bool IsColumnAllowFilter(GridColumn column) {
			return base.IsColumnAllowFilter(column) && !IsInplaceEditFormVisible;
		}
		protected override int CheckRowHandle(int currentRowHandle, int newRowHandle) {
			if(!IsValidRowHandle(newRowHandle) && IsFilterRow(currentRowHandle) && !WorkAsLookup) return currentRowHandle;
			return base.CheckRowHandle(currentRowHandle, newRowHandle);
		}
		protected internal override bool CheckCanLeaveRow(int currentRowHandle, bool raiseUpdateCurrentRow) {
			if(!CheckCloseEditForm()) return false;
			return base.CheckCanLeaveRow(currentRowHandle, raiseUpdateCurrentRow);
		}
		protected override void DoChangeFocusedRow(int currentRowHandle, int newRowHandle, bool raiseUpdateCurrentRow) {
			if(!CheckCanLeaveRow(currentRowHandle, raiseUpdateCurrentRow)) return;
			newRowHandle = CheckRowHandle(currentRowHandle, newRowHandle);
			if(currentRowHandle == newRowHandle && IsValidRowHandle(DataController.CurrentControllerRow)) return;
			int prevFocused = FocusedRowHandle;
			SetFocusedRowHandleCore(newRowHandle);
			if(IsInitialized) {
				DataController.CurrentControllerRow = FocusedRowHandle;
				if(fLockUpdate == 0) {
					RefreshRow(currentRowHandle, false);
					RefreshRow(newRowHandle, false);
					if(currentRowHandle != prevFocused) 
						RefreshRow(prevFocused, false);
				}
				MakeRowVisible(FocusedRowHandle, true);
			}
			if(prevFocused == GridControl.AutoFilterRowHandle) UpdateMRU();
			base.DoChangeFocusedRow(prevFocused, FocusedRowHandle, raiseUpdateCurrentRow);
			if(IsNewItemRow(FocusedRowHandle) && AllowChangeSelectionOnNavigation) ClearSelection();
		}
		void UpdateLeftCoord(int value) {
			this.leftCoord = value;
			if(IsLockUpdate) return;
			ViewInfo.CalcAfterHorzScroll(null, ViewRect);
			Invalidate();
			SynchronizeLeftCoord();
		}
		protected virtual void DoLeftCoordChanged() {
			CloseEditor();
			if(fLockUpdate != 0) return;
			ViewInfo.CalcAfterHorzScroll(null, ViewRect);
			UpdateScrollBars();
			InvalidateRows();
			InvalidateColumnHeader(null);
			InvalidateFooter();
			RaiseLeftCoordChanged();
		}
		protected virtual void DoTopRowIndexChanged(int prevTopRowIndex) {
			if(fLockUpdate != 0) return;
			CloseActiveEditor(true);
			if(!IsScrolling) HideHint();
			if(GridControl != null && GridControl.IsHandleCreated) {
				OnHotTrackLeave(CalcHitInfo(PointToClient(Control.MousePosition)));
			}
			DataController.ScrollingCancelAllGetRows();
			RefreshRows(true, true);
			if(IsServerMode) {
				for(int n = 0; n < ViewInfo.RowsInfo.Count; n++) {
					DataController.ScrollingCheckRowLoaded(ViewInfo.RowsInfo[n].RowHandle);
				}
			}
			EventHandler handler = (EventHandler)this.Events[topRowChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void SynchronizeTopRowIndex() {
			if(IsSplitVisible && IsSplitSynchronizeScrolling) {
				if(!IsVerticalSplit) {
					if(IsPixelScrolling) {
						((GridView)SplitOtherView).TopRowPixel = TopRowPixel;
					}
					else {
					((GridView) SplitOtherView).TopRowIndex = TopRowIndex;
					}
					SplitContainer.Update();
				}
			}
		}
		protected virtual void SynchronizeLeftCoord() {
			if(IsSplitVisible && IsSplitSynchronizeScrolling) {
				if(IsVerticalSplit) {
					((GridView) SplitOtherView).LeftCoord = LeftCoord;
					SplitContainer.Update();
				}
			}
		}
		int lockRefreshRows = 0;
		int refreshCounter = 0, redrawCounter = 0;
		protected virtual void RefreshRows(bool useCache, bool afterScroll) {
			if(lockRefreshRows != 0) return;
			if(!IsInitialized) return;
			if(afterScroll) CheckViewInfo();
			if(ViewInfo.IsReady) {
				ViewInfo.CalcAfterVertScroll(null, ViewRect, useCache);
				UpdateScrollBars();
				if(!IsLockInvalidate && afterScroll && Painter.DrawScrolledContents(ViewInfo)) {
					refreshCounter++;
					return;
				}
				redrawCounter++;
				Invalidate();
			} else {
				LayoutChangedSynchronized();
			}
		}
		bool isDataSourceChanged = false;
		protected internal override void SetDataSource(BindingContext context, object dataSource, string dataMember) {
			this.isDataSourceChanged = false;
			base.SetDataSource(context, dataSource, dataMember);
			if(!IsCellSelect && isDataSourceChanged && CheckboxSelectorColumn == null) SelectRow(FocusedRowHandle);
		}
		protected internal override void OnDataSourceChanging() {
			ResetThumbnailCache();
		}
		protected override void OnDataController_DataSourceChanged(object sender, EventArgs e) {
			this.isDataSourceChanged = true;
			if(ViewInfo != null) {
				ViewInfo.AutoRowHeightCache.Clear();
				ViewInfo.ResetPositionInfo();
			}
			this.topRowIndex = 0;
			this.topRowPixel = 0;
			base.OnDataController_DataSourceChanged(sender, e);
			if(DataController != null && !DataController.IsReady) {
				BeginUpdate();
				try {
					foreach(GridColumn col in Columns) {
						col.ResetDataSummaryItem();
					}
				} finally {
					EndUpdateCore(true);
				}
			}
			DestroyEditFormController();
			SynchronizeTopRowIndex();
		}
		protected internal override void RefreshVisibleColumnsList() {
			base.RefreshVisibleColumnsList();
			if(ViewInfo == null) return;
			ViewInfo.UpdateFixedColumnInfo();
		}
		protected override void RefreshRow(int rowHandle, bool updateEditor, bool updateEditorValue) {
			if(!IsInitialized) return;
			GridRowInfo ri = ViewInfo.GetGridRowInfo(rowHandle);
			if(ri == null) return;
			ri.SetDataDirty();
			ri.RowState = GridRowCellState.Dirty;
			InvalidateRow(ri);
			base.RefreshRow(rowHandle, updateEditor, updateEditorValue);
		}
		protected override void SetEditingState() {
			SetStateInt((int)GridState.Editing);
		}
		protected virtual void UpdateSorting() {
			DataController.DoRefresh();
		}
		protected internal override bool GetCanShowEditor(GridColumn column) {
			if(ForceLoadingPanel) return false;
			if(column == CheckboxSelectorColumn) return false;
			if(column != null && IsFilterRow(FocusedRowHandle)) {
				if(!IsColumnAllowAutoFilter(column)) return false;
				return true;
			}
			return base.GetCanShowEditor(column);
		}
		Rectangle GetEditorBounds(GridCellInfo cell) {
			Rectangle r = cell.CellValueRect;
			Rectangle bounds = ViewInfo.UpdateFixedRange(r, cell.ColumnInfo);
			if(bounds.Right > ViewInfo.ViewRects.Rows.Right) {
				bounds.Width = ViewInfo.ViewRects.Rows.Right - bounds.Left;
			}
			if(bounds.Bottom > ViewInfo.ViewRects.Rows.Bottom) {
				bounds.Height = ViewInfo.ViewRects.Rows.Bottom - bounds.Top;
			}
			if(bounds.Width < 1 || bounds.Height < 1) return Rectangle.Empty;;
			return bounds;
		}
		protected virtual AppearanceObject GetEditorAppearance() {
			if(OptionsSelection.EnableAppearanceFocusedCell)
				return ViewInfo.PaintAppearance.FocusedCell;
			if(OptionsSelection.EnableAppearanceFocusedRow)
				return ViewInfo.PaintAppearance.FocusedRow;
			return ViewInfo.PaintAppearance.Row;
		}
		protected internal override void DoNavigatorAction(NavigatorButtonType type) {
			if(IsInplaceEditFormVisible) {
				switch(type) {
					case NavigatorButtonType.First:
					case NavigatorButtonType.Last:
					case NavigatorButtonType.Next:
					case NavigatorButtonType.Prev:
					case NavigatorButtonType.NextPage:
					case NavigatorButtonType.PrevPage:
					case NavigatorButtonType.Remove:
					case NavigatorButtonType.Append:
						if(!CheckCloseEditForm()) return;
						break;
				}
			}
			base.DoNavigatorAction(type);
		}
		protected internal override bool IsNavigatorActionEnabled(NavigatorButtonType type) {
			if(!IsEditFormMode) return base.IsNavigatorActionEnabled(type);
			if(!DataController.IsReady) return false;
			switch(type) {
				case NavigatorButtonType.Edit:
					if(IsInplaceEditFormVisible) return false;
					if(!CanShowEditor) return false;
					return IsAllowEditForm(FocusedRowHandle);
				case NavigatorButtonType.CancelEdit:
				case NavigatorButtonType.EndEdit:
					if(IsInplaceEditFormVisible) return true;
					return false;
			}
			return base.IsNavigatorActionEnabled(type);
		}
		protected override void NavigatorShowEdit() {
			if(IsEditFormMode && IsAllowEditForm(FocusedRowHandle)) {
				ShowEditForm();
				return;
			}
			base.NavigatorShowEdit();
		}
		protected override void NavigatorCancelEdit() {
			if(IsEditFormMode && IsAllowEditForm(FocusedRowHandle)) {
				HideEditForm();
				CancelUpdateCurrentRow();
				return;
			}
			base.NavigatorCancelEdit();
		}
		protected override void NavigatorEndEdit() {
			if(IsEditFormMode && IsAllowEditForm(FocusedRowHandle)) {
				if(!CloseEditForm()) return;
				UpdateCurrentRow();
				return;
			}
			base.NavigatorEndEdit();
		}
		protected virtual void ActivateEditor(GridCellInfo cell) {
			if(cell == null) return;
			if(cell.MergedCell != null) return;
			if(!cell.ColumnInfo.Column.OptionsColumn.AllowEdit && !IsFilterRow(FocusedRowHandle)) {
				return;
			}
			OnActionScroll(ScrollNotifyAction.Hide);
			bool editForm = false;
			if(IsEditFormMode) {
				if(!IsFilterRow(FocusedRowHandle)) editForm = true;
			}
			if(!editForm) 
				ActivateInplaceEditor(cell);
			else
				ActivateEditForm(cell);
		}
		protected override bool IsElementsContainsFocus { get { return base.IsElementsContainsFocus || (ViewInfo.EditForm != null && ViewInfo.EditForm.ContainsFocus && ViewInfo.EditForm.Visible); } }
		bool editFormVisible = false;
		protected internal virtual bool IsShowInplaceEditForm(int rowHandle) {
			if(!IsInplaceEditForm) return false;
			if(!IsInplaceEditFormVisible && ViewInfo.EditFormRequiredHeight == 0) return false;
			if(FocusedRowHandle != rowHandle) return false;
			return true;
		}
		protected internal bool IsPopupEditFormVisible {
			get {
				if(EditFormController == null) return false;
				if(EditFormController.EditForm == null) return false;
				return (EditFormController.EditForm.PopupForm != null && EditFormController.EditForm.RootPanel != null && EditFormController.EditForm.RootPanel.FindForm() != null && EditFormController.EditForm.RootPanel.FindForm().Visible);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsEditFormVisible { get { return IsInplaceEditFormVisible || IsPopupEditFormVisible; } }
		protected internal virtual bool IsInplaceEditFormVisible { get { return editFormVisible && EditFormController != null; } }
		protected internal bool IsEditFormModified { get { return IsInplaceEditFormVisible && EditFormController.IsModified; } }
		protected internal virtual bool IsEditFormMode {
			get { return OptionsBehavior.EditingMode != GridEditingMode.Inplace && OptionsBehavior.EditingMode != GridEditingMode.Default; }
		}
		protected internal virtual bool IsInplaceEditFormHideCurrent {
			get {
				if(!IsInplaceEditForm) return false;
				if(FocusedRowHandle == GridControl.NewItemRowHandle) return true;
				return OptionsBehavior.EditingMode == GridEditingMode.EditFormInplaceHideCurrentRow;
			}
		}
		protected internal virtual bool IsInplaceEditForm {
			get {
				if(OptionsBehavior.EditingMode == GridEditingMode.EditFormInplace || OptionsBehavior.EditingMode == GridEditingMode.EditFormInplaceHideCurrentRow) {
					if(GridControl == null || GridControl.DefaultView != this) return false;
					return true;
				}
				return false;
			}
		}
		protected override bool UpdateCurrentRowAction() { 
			if(!CheckCloseEditForm()) return false;
			return UpdateCurrentRow();
		}
		protected internal bool GetAllowCancelEditFormOnRowChange() {
			return (OptionsEditForm.ActionOnModifiedRowChange != EditFormModifiedAction.Nothing);
		}
		protected internal virtual bool AskModifiedCancelAndHide() {
			if(!IsInplaceEditForm && IsInplaceEditFormVisible) {
				HideEditForm();
				return true; 
			}
			EditFormExitAction result = EditFormController.AskModifiedCancel();
			if(result == EditFormExitAction.CancelExit) return false;
			if(result == EditFormExitAction.CancelResult) HideEditForm();
			if(result == EditFormExitAction.SaveResult) CloseEditForm();
			return !IsInplaceEditFormVisible;
		}
		public virtual void HideEditForm() {
			if(!IsInplaceEditFormVisible) return;
			EditFormController.CancelValues();
			HideEditFormCore();
		}
		public virtual bool CloseEditForm() {
			if(!IsInplaceEditFormVisible) return true;
			if(!EditFormController.CloseForm()) return false;
			HideEditFormCore();
			return true;
		}
		protected virtual void ActivateEditForm(GridCellInfo cell) {
			if(cell.RowHandle == GridControl.NewItemRowHandle) ShowEditForm();
		}
		protected virtual void ActivateInplaceEditor(GridCellInfo cell) {
			this.fEditingCell = cell;
			Rectangle bounds = GetEditorBounds(cell);
			if(bounds.IsEmpty) return;
			RepositoryItem cellEdit = RequestCellEditor(cell);
			ViewInfo.UpdateCellAppearance(cell);
			ViewInfo.RequestCellEditViewInfo(cell);
			AppearanceObject appearance = new AppearanceObject();
			AppearanceHelper.Combine(appearance, new AppearanceObject[] { GetEditorAppearance(), ViewInfo.PaintAppearance.Row, cell.Appearance });
			if(cellEdit != cell.Editor && cellEdit.DefaultAlignment != HorzAlignment.Default) {
				appearance.TextOptions.HAlignment = cellEdit.DefaultAlignment;
			}
			if(ShouldUpdateActiveInplaceEditor(cell.ViewInfo))
				UpdateEditor(cellEdit, new UpdateEditorInfoArgs(GetColumnReadOnly(cell.ColumnInfo.Column), bounds, appearance, cell.CellValue, ElementsLookAndFeel, cell.ViewInfo.ErrorIconText, cell.ViewInfo.ErrorIcon, IsRightToLeft));
			ViewInfo.UpdateCellAppearance(cell, true);
			if(cell != null)
				InvalidateRow(cell.RowHandle);
		}
		protected bool ShouldUpdateActiveInplaceEditor(BaseEditViewInfo vi) {
			if(OptionsImageLoad.AsyncLoad && vi is PictureEditViewInfo){
				PictureEditViewInfo pi = (PictureEditViewInfo)vi;
				return pi.ImageInfo == null || (pi.ImageInfo.IsLoaded && !pi.ImageInfo.IsInAnimation);
			}
			return true;
		}
		protected internal override void OnLookAndFeelChanged() {
			DestroyEditFormController();
			base.OnLookAndFeelChanged();
		}
		protected internal virtual RepositoryItem RequestCellEditor(GridCellInfo cell) {
			RepositoryItem editor = cell.Editor;
			CustomRowCellEditEventHandler handler = (CustomRowCellEditEventHandler)Events[customRowCellEditForEditing];
			if(handler != null) {
				CustomRowCellEditEventArgs args = new CustomRowCellEditEventArgs(cell.RowHandle, cell.Column, editor);
				handler(this, args);
				editor = args.RepositoryItem;
			}
			return GetColumnDefaultRepositoryItemForEditing(cell.Column, editor);
		}
		protected internal override int CalcRealViewHeight(Rectangle viewRect) {
			if(RequireSynchronization != SynchronizationMode.None) {
				CheckSynchronize();
				calculatedRealViewHeight = -1;
			}
			if(calculatedRealViewHeight != -1 && viewRect.Size == ViewRect.Size)
				return calculatedRealViewHeight;
			int result = viewRect.Height;
			GridViewInfo tempViewInfo = BaseInfo.CreateViewInfo(this) as GridViewInfo,
				oldViewInfo = ViewInfo, copyFromInfo = ViewInfo;
			this.fViewInfo = tempViewInfo;
			RefreshVisibleColumnsList();
			for(int i = 0; i < (oldViewInfo.IsReady ? 1 : 2); i++) {
				try {
					ViewInfo.PrepareCalcRealViewHeight(viewRect, copyFromInfo);
					result = ViewInfo.CalcRealViewHeight(viewRect);
				}
				catch {
				}
				copyFromInfo = ViewInfo;
			}
			this.fViewInfo = oldViewInfo;
			calculatedRealViewHeight = result;
			return result;
		}
		protected internal override BaseView CanMoveFocusedRow(int delta) {
			int frh = FocusedRowHandle;
			if( frh == GridView.InvalidRowHandle) {
				frh = GetVisibleRowHandle(0);
				if(frh == GridView.InvalidRowHandle) 
					return null;
			}
			int fv = GetVisibleIndex(frh);
			fv += delta;
			BaseView result = this;
			if(fv < 0) result = null;
			if(fv >= RowCount) 
				result = null;
			if(result == null && IsDetailView) 
				result = ParentView.CanMoveFocusedRow(delta);
			return result;
		}
		protected internal override void OnChildLayoutChanged(BaseView childView) {
			if(internalUpdateCount > 0) {
				return;
			}
			internalUpdateCount ++;
			try {
				LayoutChanged();
				if(childView.NeedReLayout)
					childView.LayoutChanged();
			}
			finally {
				internalUpdateCount --;
			}
		}
		protected internal override void CollapseDetail(BaseView view) {
			if(DataController != null) {
				DataController.CollapseDetailRowByOwner(view);
				LayoutChanged();
			}
		}
		protected internal override void CollapseDetails(int rowHandle) {
			if(DataController != null) {
				bool expanded = GetMasterRowExpanded(rowHandle);
				DataController.CollapseDetailRow(rowHandle);
				LayoutChanged();
				if(expanded) RaiseMasterRowCollapsed(rowHandle, -1);
			}
		}
		protected internal override void OnColumnSizeChanged(GridColumn column) {
			if(IsDeserializing) return;
			if(column.VisibleIndex > -1) {
				if(!OptionsView.ColumnAutoWidth) {
					OnPropertiesChanged();
					return;
				}
				RecalcRealColumnWidthes();
				ViewInfo.RecalcColumnWidthes(column);
				RecalcRealColumnWidthes();
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool AllowPartialGroupsBase {
			get {
				if(OptionsBehavior.AllowPartialGroups != DefaultBoolean.True) return false;
				if(AllowFixedGroups) return false;
				return true;
			}
		}
		protected internal override int GetMaxGroupCount() {
			if(AllowPartialGroupsBase && !AllowMultiPartialGroups) return 1;
			return -1;
		}
		protected virtual bool AllowMultiPartialGroups { get { return false; } }
		protected internal virtual bool AllowPartialGroups {
			get {
				if(!AllowPartialGroupsBase) return false;
				if(SortInfo.GroupCount == 0) return false;
				return true;
			}
		}
		protected internal virtual bool IsAlignGroupRowSummariesUnderColumns {
			get {
				if(OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True) return true;
				if(OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.False) return false;
				return AllowPartialGroups;
			}
		}
		protected internal virtual bool IsAlignGroupRowSummariesUnderColumnsAutoFix {  get { return IsAlignGroupRowSummariesUnderColumns && !OptionsView.ColumnAutoWidth; } }
		protected virtual bool GetShowGroupedColumns() { return AllowPartialGroups || OptionsView.ShowGroupedColumns || IsAlignGroupRowSummariesUnderColumns; }
		protected internal override bool CanShowColumnInCustomizationForm(GridColumn col) {
			if(!col.OptionsColumn.ShowInCustomizationForm) return false;
			if(!OptionsCustomization.AllowColumnMoving || !col.OptionsColumn.AllowMove || !col.OptionsColumn.AllowShowHide) return false;
			return (col.VisibleIndex < 0 && (col.GroupIndex < 0 || GetShowGroupedColumns()));
		}
		protected internal override GridColumn GetNearestCanFocusedColumn(GridColumn col, int delta, bool allowChangeFocusedRow, KeyEventArgs e) {
			GridColumn res = GetNearestCanFocusedColumnCore(col, delta, allowChangeFocusedRow, e);
			if(res == null && e is GridHandler.KeyEnterTabEventArgs) {
				if(GetNearestCanFocusedColumnCore(GetVisibleColumn(0), delta, allowChangeFocusedRow, e) == null) {
					e = new KeyEventArgs(Keys.Enter);
					res = GetNearestCanFocusedColumnCore(col, delta, allowChangeFocusedRow, e);
				}
			}
			if(res == null && !allowChangeFocusedRow) {
				if(FocusedColumn != null && FocusedColumn.OptionsColumn.AllowFocus) return FocusedColumn;
				return GetNearestCanFocusedColumnCore(GetVisibleColumn(0), 0, false, e);
			}
			return res;
		}
		GridColumn GetNearestCanFocusedColumnCore(GridColumn col, int delta, bool allowChangeFocusedRow, KeyEventArgs e) {
			bool isFilter = IsFilterRow(FocusedRowHandle);
			GridColumn prevFocused = FocusedColumn;
			if(col == null) return FocusedColumn;
			if(delta == 0) {
				if((isFilter ? IsColumnAllowAutoFilter(col) : col.OptionsColumn.AllowFocus) && (e.KeyCode != Keys.Tab || col.OptionsColumn.TabStop)) {
					return col;
				}
				if(col.VisibleIndex == VisibleColumns.Count - 1)
					delta = -1;
				else
					delta = 1;
			}
			int vIndex = col.VisibleIndex;
			vIndex += delta;
			int oIndex = -100, stopCount = 0;
			for(;;) {
				if(vIndex >= VisibleColumns.Count) {
					vIndex = VisibleColumns.Count - 1; 
					stopCount ++;
				}
				if(vIndex < 0) { 
					vIndex = 0;
					stopCount ++;
				}
				if(stopCount > 2) {
					col = prevFocused;
					break;
				}
				if(oIndex == -100) oIndex = vIndex;
				else {
					if(vIndex == oIndex) break;
				}
				GridColumn c = GetVisibleColumn(vIndex);
				if(c == null) break;
				vIndex += delta;
				if((isFilter ? !IsColumnAllowAutoFilter(c) : !c.OptionsColumn.AllowFocus) || (e.KeyCode == Keys.Tab && !c.OptionsColumn.TabStop)) {
					if(vIndex == oIndex) break;
					continue;
				}
				col = c;
				break;
			}
			if(col != null && ((isFilter ? !IsColumnAllowAutoFilter(col) : !col.OptionsColumn.AllowFocus) || (e.KeyCode == Keys.Tab && !col.OptionsColumn.TabStop))) col = null;
			if(allowChangeFocusedRow && col != null && prevFocused == col) {
				int prevRow = FocusedRowHandle;
				Keys byKey = e.KeyCode;
				if(e.KeyValue == 0) byKey = delta < 0 ? Keys.Left : Keys.Right;
				DoMoveFocusedRow((delta < 0 ? -1 : 1), e);
				if(prevRow != FocusedRowHandle && !IsGroupRow(FocusedRowHandle))
					col = GetNearestCanFocusedColumn((delta > 0 ? GetVisibleColumn(0) : GetVisibleColumn(VisibleColumns.Count - 1)), 0, false, e);
			}
			return col;
		}
		protected internal override void OnEmbeddedNavigatorSizeChanged() {
			if(ViewDisposing || !IsInitialized) return;
			base.OnEmbeddedNavigatorSizeChanged();
			ScrollInfo.LayoutChanged();
		}
		protected internal override void DoMoveFocusedRow(int delta, KeyEventArgs e) {
			int prevFocusedHandle = FocusedRowHandle;
			GridColumn prevFocusedColumn = FocusedColumn;
			if(!DoBeforeMoveFocusedRow(delta, e)) return;
			try {
				if(FocusedRowHandle == DevExpress.Data.DataController.InvalidRow && RowCount > 0) {
					DoChangeFocusedRow(GetVisibleRowHandle(0), e);
					return;
				}
				int minVisibleIndex = OptionsView.NewItemRowPosition == NewItemRowPosition.Top ? -1 : 0;
				int fv = Math.Max(GetVisibleIndex(FocusedRowHandle), minVisibleIndex);
				if(FocusedRowHandle == GridControl.AutoFilterRowHandle) fv = -1;
				BaseView detail = null;
				fv += delta;
				if(e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) {
					if(e.Alt) {
						int handle = e.KeyCode == Keys.Down ? DataController.GetNextSibling(FocusedRowHandle) : DataController.GetPrevSibling(FocusedRowHandle);
						fv = GetVisibleIndex(handle);
					}
					else {
					if(IsDetailView && !IsZoomedView) {
						if(fv < minVisibleIndex) {
							if(!CheckCanLeaveRow(FocusedRowHandle, true)) return;
							FocusCreator();
							GridControl.FocusedView = ParentView;
							return; 
						}
						if(fv >= RowCount) {
							if(e.KeyCode == Keys.Down) {
								if(IsMasterRow(FocusedRowHandle) && GetMasterRowExpanded(FocusedRowHandle)) {
									detail = GetVisibleDetailView(FocusedRowHandle);
									if(detail != null) {
										if(!CheckCanLeaveRow(FocusedRowHandle, true)) return;
										GridControl.FocusedView = detail;
										return;
									}
								}
							}
							detail = ParentView.CanMoveFocusedRow(1);
							if(detail != null && detail.IsVisible) {
								if(!CheckCanLeaveRow(FocusedRowHandle, true)) return;
								FocusCreator();
								GridControl.FocusedView = detail;
								detail.DoMoveFocusedRow(1, new KeyEventArgs(Keys.Right));
								return;
							}
						}
					}
					if(e.KeyCode == Keys.Down) {
						if(IsMasterRow(FocusedRowHandle) && GetMasterRowExpanded(FocusedRowHandle)) {
							if(!CheckCanLeaveRow(FocusedRowHandle, true)) return;
							GridControl.FocusedView = GetVisibleDetailView(FocusedRowHandle);
							return;
						}
					}
				}
				}
				if(fv < minVisibleIndex) fv = minVisibleIndex;
				if(fv >= RowCount) fv = RowCount - 1;
				DoChangeFocusedRow(GetVisibleRowHandle(fv), e);
				if(prevFocusedHandle != FocusedRowHandle && e.KeyCode == Keys.Up) {
					if(IsMasterRow(FocusedRowHandle) && GetMasterRowExpanded(FocusedRowHandle)) {
						if(!CheckCanLeaveRow(FocusedRowHandle, true)) return;
						GridControl.FocusedView = GetVisibleDetailView(FocusedRowHandle);
					}
				}
			}
			finally {
				DoAfterMoveFocusedRow(e, prevFocusedHandle, null, null);
			}
		}
		protected virtual void DoChangeFocusedRow(int newFocusedRowHandle, KeyEventArgs e) {
			Scroller.OnDoChangedFocusedRow(newFocusedRowHandle, e);
		}
		protected internal override void OnColumnWidthChanged(GridColumn column) { 
			base.OnColumnWidthChanged(column);
			RaiseColumnWidthChanged(new ColumnEventArgs(column));
		}
		protected virtual void EndColumnSizing() {
			GridColumn col = Painter.ReSizingObject as GridColumn;
			GridColumnInfoArgs ci = ViewInfo.ColumnsInfo[col];
			if(ci != null) {
				int newSize = 0;
				if(IsRightToLeft) {
					newSize = ci.Bounds.Right - Painter.CurrentSizerPos;
					if(newSize < 0) newSize = Painter.CurrentSizerPos - ci.Bounds.Right;
				}
				else {
					newSize = Painter.CurrentSizerPos - ci.Bounds.Left;
					if(newSize < 0) newSize = ci.Bounds.Right - Painter.CurrentSizerPos;
				}
				if(newSize < GetColumnMinWidth(col)) newSize = GetColumnMinWidth(col);
				if(GetColumnMaxWidth(col) > 0) newSize = Math.Min(newSize, GetColumnMaxWidth(col));
				col.ResizeCore(newSize, true);
			}
		}
		protected virtual void EndRowSizing() {
			int rowHandle = (int)(Painter.ReSizingObject);
			GridRowInfo ri = ViewInfo.GetGridRowInfo(rowHandle);
			if(ri == null) return;
			int height = (Painter.CurrentSizerPos - ri.DataBounds.Top) / ViewInfo.RowLineCount;
			if(height > 0)
				RowHeight = height;
		}
		protected virtual void EndRowDetailSizing() {
			int rowHandle = (int)(Painter.ReSizingObject);
			GridDataRowInfo ri = ViewInfo.GetGridRowInfo(rowHandle) as GridDataRowInfo;
			if(ri == null) return;
			int height = Painter.CurrentSizerPos - ri.DetailBounds.Top;
			BaseView gv = GetVisibleDetailView(rowHandle);
			if(gv != null && height > 0) {
				gv.DetailHeight = height;
				if(DetailLevel > 0 && ParentView != null) {
					this.calculatedRealViewHeight = -1;
					LayoutChanged();
				}
			}
		}
		internal void EndSizingCore() { EndSizing(); }
		protected virtual void EndSizing() {
			Painter.HideSizerLine();
			if(Painter.StartSizerPos != Painter.CurrentSizerPos && Painter.ReSizingObject != null) {
				if(State == GridState.ColumnSizing) {
					EndColumnSizing();
				}
				if(State == GridState.RowDetailSizing) {
					EndRowDetailSizing();
				}
				if(State == GridState.RowSizing) {
					EndRowSizing();
				}
			}
			Painter.StopSizing();
		}
		protected internal virtual int GetColumnIndent(GridColumn column) {
			int result = 0;
			if(!IsInitialized) return 0;
			if(IsFirstVisibleColumn(column)) {
				if(IsShowDetailButtons) 
					result += ViewInfo.DetailButtonSize.Width;
				result += ViewInfo.CalcRowLevelIndent(null, SortInfo.GroupCount);
			}
			return result;
		}
		protected virtual bool IsFirstVisibleColumn(GridColumn column) {
			return column.VisibleIndex == 0;
		}
		protected internal override bool GetColumnAllowMerge(GridColumn column) {
			DefaultBoolean opt = column.OptionsColumn.AllowMerge;
			if(!IsLoading && opt == DefaultBoolean.Default) {
				if(column.GetIsNonSortableEditor()) return false;
				opt = DefaultBoolean.True;
			}
			bool res = opt != DefaultBoolean.False;
			if(!res || IsLoading || ViewInfo == null) return res;
			return column.VisibleIndex != 0 || !ViewInfo.ShowDetailButtons;
		}
		protected internal override int GetColumnMaxWidth(GridColumn column) {
			if(column.MaxWidth == 0) return 0;
			return column.MaxWidth + GetColumnIndent(column);
		}
		protected internal override int GetColumnMinWidth(GridColumn column) {
			int r = column.MinWidth;
			int result = r + GetColumnIndent(column);
			return result;
		}
		protected internal virtual AppearanceObject GetLevelStyle(int level, bool groupRow) {
			AppearanceObject appearance = ViewInfo.PaintAppearance.GroupRow;
			if(ViewInfo.IsSkinned && !groupRow) { 
				SkinElement skin = GridSkins.GetSkin(this)[GridSkins.SkinGridGroupLevel];
				if(skin != null) {
					AppearanceDefault app = skin.GetAppearanceDefault();
					appearance = (AppearanceObject)appearance.Clone();
					if(app.BackColor != Color.Empty)
						AppearanceHelper.Apply(appearance, app);
				}
			}
			GroupLevelStyleEventHandler handler = (GroupLevelStyleEventHandler)this.Events[groupLevelStyle];
			if(handler == null) return appearance;
			GroupLevelStyleEventArgs e = new GroupLevelStyleEventArgs(level, appearance.Clone() as AppearanceObject);
			handler(this, e);
			return e.LevelAppearance;
		}
		public virtual Hashtable GetGroupSummaryValues(int rowHandle) {
			return DataController.GetGroupSummary(rowHandle);
		}
		public object GetGroupSummaryValue(int rowHandle, GridGroupSummaryItem summaryItem) {
			if(summaryItem == null) throw new ArgumentNullException("summaryItem");
			Hashtable hash = GetGroupSummaryValues(rowHandle);
			if(hash == null) return null;
			return hash[summaryItem];
		}
		public string GetGroupSummaryDisplayText(int rowHandle, GridGroupSummaryItem summaryItem) {
			if(summaryItem == null) throw new ArgumentNullException("summaryItem");
			Hashtable hash = GetGroupSummaryValues(rowHandle);
			if(hash == null || !hash.ContainsKey(summaryItem)) return null;
			object val = hash[summaryItem];
			return summaryItem.GetDisplayText(val, true);
		}
		public virtual string GetGroupSummaryText(int rowHandle) {
			string result = "";
			Hashtable summary = GetGroupSummaryValues(rowHandle);
			if(summary == null) return result;
			int maxCount = summary.Count, n;
			object[] items = new object[maxCount];
			for(n = 0; n < maxCount; n++) { items[n] = null; }
			n = 0;
			foreach(DictionaryEntry dEntry in summary) {
				GridGroupSummaryItem item = dEntry.Key as GridGroupSummaryItem;
				if(item.ShowInGroupColumnFooter != null) continue; 
				items[n++] = dEntry;
			}
			if(n == 0) return result;
			Array.Sort(items, summaryComparer);
			for(n = 0; n < maxCount; n++) {
				if(items[n] == null) continue;
				GridGroupSummaryItem item = ((DictionaryEntry)items[n]).Key as GridGroupSummaryItem;
				result += (result.Length != 0 ? ", " : "") + item.GetDisplayText(((DictionaryEntry)items[n]).Value, true);
			}
			return result;
		}
		public virtual DictionaryEntry GetRowSummaryItem(int rowHandle, GridColumn column) {
			if(column == null) return new DictionaryEntry();
			Hashtable summary = GetGroupSummaryValues(rowHandle);
			if(summary == null) return new DictionaryEntry();
			foreach(DictionaryEntry dEntry in summary) {
				GridGroupSummaryItem item = dEntry.Key as GridGroupSummaryItem;
				if(item.ShowInGroupColumnFooter != column) continue;
				return dEntry;
			}
			return new DictionaryEntry();
		}
		public virtual string GetRowFooterCellText(int rowHandle, GridColumn column) {
			string result = "";
			DictionaryEntry dEntry = GetRowSummaryItem(rowHandle, column);
			if(dEntry.Key == null) return result;
			GridGroupSummaryItem item = dEntry.Key as GridGroupSummaryItem;
			return item.GetDisplayText(dEntry.Value, false);
		}
		public virtual bool IsShowRowFooterCell(int rowHandle, GridColumn column) {
			int gCount = GroupSummary.Count;
			if(GroupSummary.ActiveCount == 0) return false;
			if(column == null) return false;
			for(int n = 0; n < gCount; n++) {
				GridGroupSummaryItem item = GroupSummary[n] as GridGroupSummaryItem;
				if(item.SummaryType != SummaryItemType.None && item.ShowInGroupColumnFooter == column) return true;
			}
			return false;
		}
		public virtual object GetGroupRowPrintValue(int rowHandle) {
			int level = GetRowLevel(rowHandle);
			if(level > -1 && level < GroupCount) {
				DevExpress.XtraEditors.Repository.RepositoryItem ritem = GetRowCellRepositoryItem(rowHandle, SortInfo[level].Column);
				if(ritem == null || ritem.ExportMode == ExportMode.Value)
					return GetGroupRowValue(rowHandle);
				else
					return GetGroupRowDisplayText(rowHandle);
			}
			return GetGroupRowValue(rowHandle);
		}
		public virtual object GetGroupRowValue(int rowHandle) {
			int level = GetRowLevel(rowHandle);
			if(level > -1 && level < GroupCount) return DataController.GetGroupRowValue(rowHandle, SortInfo[level].Column.ColumnInfo);
			return DataController.GetGroupRowValue(rowHandle);
		}
		public virtual object GetGroupRowValue(int rowHandle, GridColumn column) {
			if(column == null) return GetGroupRowValue(rowHandle);
			return DataController.GetGroupRowValue(rowHandle, column.ColumnInfo);
		}
		protected internal virtual string GetGroupRowDisplayText(int rowHandle, bool fullText, out string groupValueDisplayText) {
			groupValueDisplayText = string.Empty;
			string s;
			int rowLevel = GetRowLevel(rowHandle);
			GridColumn column = rowLevel < SortInfo.GroupCount ? SortInfo[rowLevel].Column : null;
			if(column == null) return "";
			object val = GetGroupRowValue(rowHandle, column);
			GridDataColumnSortInfo info = SortData.GetSortInfo(column);
			if(info != null) val = info.UpdateGroupDisplayValue(val);
			s = GetGroupRowDisplayText(rowHandle, column, val, info == null ? column.GroupFormat : info.GetColumnGroupFormat());
			if(info != null) {
				s = info.GetGroupDisplayText(val, s);
				if(!info.AllowImageGroup) fullText = false;
			}
			groupValueDisplayText = s;
			string s2 = GetGroupSummaryText(rowHandle);
			string res = String.Format(GetGroupFormat(), column.GetTextCaption(), s, s2);
			if(!fullText) {
				res = res.Replace(DevExpress.XtraEditors.Drawing.BaseEditorGroupRowPainter.ImageText, "");
				if(OptionsView.AllowHtmlDrawGroups) res = StringPainter.Default.RemoveFormat(res);
			}
			return res;
		}
		public virtual string GetGroupRowDisplayText(int rowHandle, bool fullText) {
			string text;
			return GetGroupRowDisplayText(rowHandle, fullText, out text);
		}
		public string GetGroupRowDisplayText(int rowHandle) { return GetGroupRowDisplayText(rowHandle, false); }
		public virtual int GetRowLevel(int rowHandle) {
			return DataController.GetRowLevel(rowHandle);
		}
		public virtual string GetRowPreviewDisplayText(int rowHandle) {
			return GetRowPreviewDisplayTextCore(GetDataSourceRowIndex(rowHandle), rowHandle);
		}
		protected internal virtual string GetRowPreviewDisplayTextCore(int dataSourceRowIndex, int rowHandle) {
			object value = null;
			if(!string.IsNullOrEmpty(PreviewFieldName) && DataController.IsReady)
				value = DataController.GetListSourceRowValue(dataSourceRowIndex, PreviewFieldName);
			object row = DataController.GetRowByListSourceIndex(dataSourceRowIndex);
			string s = value == null || value == DBNull.Value ? string.Empty : value.ToString();
			RaiseGetPreviewDisplayText(dataSourceRowIndex, row, rowHandle, ref s);
			if(DesignMode && string.IsNullOrEmpty(s)) {
				int lineCount = Math.Min(10, PreviewLineCount < 1 ? 1 : PreviewLineCount);
				for(int n = 0; n < lineCount; n++) {
					s += string.Format("Preview line {0}\xd\xa", n + 1);
				}
			}
			return s;
		}
		public virtual bool IsShowRowFooters() {
			if(OptionsView.GroupFooterShowMode == GroupFooterShowMode.Hidden) return false;
			if(SortInfo.GroupCount == 0) return false;
			if(IsAlignGroupRowSummariesUnderColumns) return false;
			return true;
		}
		public virtual bool IsExistAnyRowFooterCell(int rowHandle) {
			foreach(GridColumn col in Columns) {
				if(col.VisibleIndex == -1) continue;
				if(IsShowRowFooterCell(rowHandle, col)) return true;
			}
			return false;
		}
		public virtual int GetVisibleRowLevel(int rowVisibleIndex) {
			return GetRowLevel(DataController.GetControllerRowHandle(rowVisibleIndex));
		}
		protected internal override void OnGotFocus() {
			if(InternalFocusLock != 0) return;
			base.OnGotFocus();
			RefreshRow(FocusedRowHandle, false, false);
			if(IsMultiSelect && SelectedRowsCount > 0) InvalidateRows();
		}
		protected internal override void OnLostFocus() {
			if(InternalFocusLock != 0) return;
			base.OnLostFocus();
			InvalidateRow(FocusedRowHandle);
			if(IsMultiSelect && SelectedRowsCount > 0) InvalidateRows();
		}
		internal void HideDetailTip() {
			detailTip.HideTip();
		}
		internal void CloseActiveEditor(bool topIndexUpdate) {
			if(GridControl == null) return;
			if(topIndexUpdate && IsFilterRow(FocusedRowHandle)) return;
			if(GridControl.FocusedView != this && GridControl.FocusedView != null) {
				GridControl.FocusedView.CloseEditor();
			}
			CloseEditor();
		}
		int prevTopRowIndex = -1;
		[Browsable(false)]
		public int PrevTopRowIndex { get { return prevTopRowIndex; } }
		protected virtual void InternalSetTopRowIndex(int newTopRowIndex) {
			if(newTopRowIndex >= RowCount) newTopRowIndex = RowCount - 1;
			if(newTopRowIndex < 0) newTopRowIndex = 0;
			if(newTopRowIndex == TopRowIndex) return;
			CloseActiveEditor(true);
			prevTopRowIndex = topRowIndex;
			topRowIndex = newTopRowIndex;
			DoTopRowIndexChanged(prevTopRowIndex);
			SynchronizeTopRowIndex();
		}
		protected virtual void RecalcRealColumnWidthes() {
			if(!OptionsView.ColumnAutoWidth) return;
			int colCount = VisibleColumns.Count;
			for(int i = 0; i < colCount; i++) {
				GridColumn col = GetVisibleColumn(i);
				int minWidth = GetColumnMinWidth(col), maxWidth = GetColumnMaxWidth(col);
				col.InternalSetWidth(col.VisibleWidth);
				if(col.width < minWidth) col.InternalSetWidth(minWidth);
				if(col.width > maxWidth && maxWidth > 0) col.InternalSetWidth(maxWidth);
			}
		}
		protected override bool CanUseFixedStyle { get { return true; } } 
		protected internal override void SetColumnFixedStyle(GridColumn column, FixedStyle newValue) {
			if(IsLoading || IsDeserializing) return;
			RefreshVisibleColumnsList();
			OnPropertiesChanged();
		}
		protected internal override void SetColumnVisibleIndex(GridColumn column, int newValue) {
			base.SetColumnVisibleIndex(column, newValue);
			UpdateColumnsCustomization();
		}
		internal void ShowDetailTipCore(GridHitInfo hi) { ShowDetailTip(hi); }
		protected virtual void ShowDetailTip(GridHitInfo hi) {
			if(!OptionsDetail.EnableDetailToolTip ||
				DataController == null ||	GetRelationCount(hi.RowHandle) < 1) 
				return;
			if(detailTip.RowHandle == hi.RowHandle) {
				return;
			}
			GridDetailInfo[] details = GetDetailInfo(hi.RowHandle, false);
			HideDetailTip();
			detailTip.RowHandle = GridControl.InvalidRowHandle;
			if(details == null || details.Length == 0) return;
			if(details.Length == 1 && GetMasterRowExpanded(hi.RowHandle)) return;
			GridRowInfo ri = ViewInfo.GetGridRowInfo(hi.RowHandle);
			GridCellInfo cell = ViewInfo.GetGridCellInfo(hi);
			AppearanceObject appearance = ViewInfo.PaintAppearance.DetailTip;
			detailTip.RowHandle = hi.RowHandle;
			detailTip.Location = new Point(cell.CellValueRect.Left, cell.Bounds.Top);
			detailTip.Font = appearance.Font;
			detailTip.ForeColor = appearance.ForeColor;
			detailTip.BackColor = appearance.BackColor;
			detailTip.Size = detailTip.CalcSize(details);
			if(!detailTip.CanShowTip) return;
			if(detailTip.Right > ViewInfo.ViewRects.Client.Right) {
				int width = detailTip.Size.Width - (detailTip.Right - ViewInfo.ViewRects.Client.Right);
				if(width < 10) {
					HideDetailTip();
					return;
				}
				detailTip.Size = new Size(width, detailTip.Size.Height);
			}
			if(detailTip.Bottom > ViewInfo.ViewRects.Client.Bottom) {
				detailTip.Top -= (detailTip.Bottom - ViewInfo.ViewRects.Client.Bottom);
			}
			if(GridControl.Controls.GetChildIndex(detailTip, false) == -1) {
				GridControl.Controls.Add(detailTip);
			}
			detailTip.ShowTip();
		}
		protected virtual void StopColumnDragging() {
			GridControl.DragController.CancelDrag();
			ViewInfo.SelectionInfo.ClearPressedInfo();
		}
		protected internal override void OnVisibleChanged() {
			base.OnVisibleChanged();
			if(GridControl != null && GridControl.Visible) {
				ScrollInfo.UpdateVisibility();
			}
			if(GridControl != null && (!GridControl.Visible || GridControl.FindForm() == null || !GridControl.FindForm().Visible)) {
				DestroyCustomization();
				if(State == GridState.ColumnDragging) SetState((int)GridState.Normal);
			}
		}
		protected override void OnActiveFilterEnabledChanged() {
			base.OnActiveFilterEnabledChanged();
			MakeRowVisible(FocusedRowHandle, false);
		}
		protected override void RaiseColumnFilterChanged() {
			if(!DataController.IsUpdateLocked) {
				MakeRowVisible(FocusedRowHandle, false);
				CheckTopRowIndex();
			}
			base.RaiseColumnFilterChanged();
		}
		protected override void SetViewRect(Rectangle newValue) {
			if(ViewRect == newValue) return;
			if(newValue.Width < 0 || newValue.Height < 0)
				newValue = Rectangle.Empty;
			if(ViewDisposing) {
				this.fViewRect = Rectangle.Empty;
				return;
			}
			if(this.fViewRect == newValue) return;
			try {
				fUpdateSize ++;
				bool gridSizeChanged = true;
				int prevCalc = -1;
				if(newValue.Width == this.fViewRect.Width && newValue.Height == fViewRect.Height) {
					prevCalc = calculatedRealViewHeight;
					gridSizeChanged = false;
				}
				if(newValue.IsEmpty) {
					ScrollInfo.ClientRect = newValue;
					prevCalc = -1;
					gridSizeChanged = false;
				} else {
					if(this.fViewRect.IsEmpty) {
						CheckSynchronize();
					}
				}
				try {
					this.fViewRect = newValue;
					LayoutChangedSynchronized();
				}
				finally {
					if(!gridSizeChanged) {
						calculatedRealViewHeight = prevCalc;
					}
				}
				if(gridSizeChanged && IsVisible) {
					InternalSetTopRowIndex(CheckTopRowIndex(TopRowIndex)); 
				}
			}
			finally {
				fUpdateSize --;
			}
		}
		protected internal virtual bool IsCellMerge {
			get {
				return OptionsView.AllowCellMerge && !OptionsView.ShowPreview && RowSeparatorHeight == 0;
			}
		}
		[Browsable(false)]
		public override Rectangle ViewRect {
			get { return fViewRect; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GridState State { get { return state; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridViewIsMultiSelect")]
#endif
		public override bool IsMultiSelect { get { return base.IsMultiSelect && !OptionsView.AllowCellMerge; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsCellSelect { get { return IsMultiSelect && OptionsSelection.MultiSelectMode == GridMultiSelectMode.CellSelect; } }
		protected internal override bool IsAllowZoomDetail {
			get { return OptionsDetail.AllowZoomDetail; }
		}
		protected internal override bool IsAutoCollapseDetail {
			get { return OptionsDetail.AutoZoomDetail; }
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridViewIsVisible")]
#endif
		public override bool IsVisible {
			get { return ViewRect.X > -10000 && !ViewRect.IsEmpty && ViewRect.Right > 0 && ViewRect.Bottom > 0; }
		}
		protected internal override bool NeedReLayout { 
			get { return true; }
		}
		protected internal virtual ScrollInfo ScrollInfo { get { return scrollInfo; } }
		protected internal override void CreateHandles() {
			base.CreateHandles();
			if(IsLoading) return;
			if(ScrollInfo != null && GridControl.IsHandleCreated) ScrollInfo.CreateHandles();
		}
		protected internal virtual int StateInt { get { return (int)state; } }
		protected virtual void SetStateInt(int newState) {
			this.state = (GridState)newState;
		}
		protected internal virtual bool DoIncrementalSearch(string text) {
			if(text == IncrementalText) {
				return false;
			}
			if(text != "") {
				int newRow = FindRow(new FindRowArgs(FocusedRowHandle, FocusedColumn, text, true), 
					delegate(object completed) { 
						IncrementalSearchResult((int)completed, text, true);
					});
				if(newRow == BaseGridController.OperationInProgress) {
					IncrementalText = text;
					return true;
				}
				if(newRow == GridControl.InvalidRowHandle) return true;
				IncrementalSearchResult(newRow, text, false);
			}
			return true;
		}
		protected override void OnAsyncTotalsReceived() {
			if(WorkAsLookup) return;
			if(!IsValidRowHandle(FocusedRowHandle)) FocusedRowHandle = GetVisibleRowHandle(0);
			if(GroupCount > 0 && IsDataRow(FocusedRowHandle) && !DataController.IsRowVisible(FocusedRowHandle)) {
				FocusedRowHandle = GetVisibleRowHandle(0);
			} 
		}
		string lastValidIncrementalText = "";
		protected void IncrementalSearchResult(int row, string text, bool isAsync) {
			if(row == GridControl.InvalidRowHandle) {
				if(isAsync) {
					if(!string.IsNullOrEmpty(lastValidIncrementalText) && IncrementalText.StartsWith(lastValidIncrementalText)) {
						IncrementalText = lastValidIncrementalText;
					}
					else {
						IncrementalText = "";
					}
				}
				return;
			}
			this.lastValidIncrementalText = text;
			SetStateCore(GridState.IncrementalSearch); 
			IncrementalText = text;
			FocusedRowHandle = row;
			SetStateCore(GridState.IncrementalSearch); 
			IncrementalText = text;
		}
		public void StartIncrementalSearch(string start) {
			if(start == null || start.Length == 0) return;
			DoIncrementalSearch(start);
		}
		public void StopIncrementalSearch() {
			if(State == GridState.IncrementalSearch) 
				SetState((int)GridState.Normal);
		}
		internal void SetStateCore(GridState val) { SetState((int)val); }
		protected virtual void SetState(int value) {
			if(StateInt == value) return;
			DestroyFilterPopup();
			if(value != (int)GridState.IncrementalSearch) {
				DataController.CancelFindIncremental();
				IncrementalText = "";
			}
			if(State == GridState.Scrolling) StopScrolling();
			if(State == GridState.Selection) EndAccessSelection();
			bool prevDrag = IsDraggingState, prevNormal = IsDefaultState;
			if(IsSizingState) 
				Painter.StopSizing();
			if(IsEditing) 
				CloseEditor();
			if(IsEditing && !fAllowCloseEditor) return;
			if(prevNormal && value != (int)GridState.Editing) {
				if(value != (int)GridState.ColumnSizing) {
					if(!UpdateCurrentRow()) return;
				}
			}
			SetStateInt((int)value);
			if(IsDefaultState) 
				ViewInfo.SelectionInfo.ClearPressedInfo();
			else {
				HideHint();
			}
			if(prevDrag) {
				StopColumnDragging();
			}
			UpdateNavigator();
			OnStateChanged();
		}
		protected virtual void OnStateChanged() {
			if(State != GridState.ColumnDown && State != GridState.Normal) FireChanged();
		}
		protected internal virtual GridCellInfo EditingCell { get { return fEditingCell; } }
		protected internal new GridViewInfo ViewInfo { get { return base.ViewInfo as GridViewInfo; } }
		protected override string ViewName { get { return "GridView"; } }
		protected internal new GridPainter Painter { get { return base.Painter as GridPainter; } }
		protected override void UpdateSelectedTabPage(ViewTab tabControl, int rowHandle) {
			if(tabControl == null) return;
			int vIndex = GetVisibleDetailRelationIndex(rowHandle);
			ViewTabPage selectedPage = null;
			foreach(ViewTabPage page in tabControl.Pages) {
				if((int)page.Tag == vIndex) {
					selectedPage = page;
					break;
				}
			}
			if(selectedPage != null) tabControl.SelectedPage = selectedPage;
		}
		protected override void PopulateTabMasterData(ViewTab tabControl, int rowHandle) {
			tabControl.HeaderLocation = DetailTabHeaderLocation;
			GridDetailInfo[] details = GetDetailInfo(rowHandle, false);
			if(details == null) return;
			int vIndex = GetVisibleDetailRelationIndex(rowHandle);
			foreach(GridDetailInfo detail in details) {
				if(!OptionsDetail.AllowExpandEmptyDetails && IsMasterRowEmptyEx(rowHandle, detail.RelationIndex))
					continue;
				bool exp =  vIndex == detail.RelationIndex;
				ViewTabPage page = new ViewTabPage(tabControl);
				page.Text = detail.Caption.Trim();
				page.Tag = detail.RelationIndex;
				page.DetailInfo = detail;
				tabControl.Pages.Add(page);
				if(exp) tabControl.SelectedPage = page;
			}
		}
		protected override void OnChildViewTabChanging(BaseView baseView, ViewInfoTabPageChangingEventArgs e) {
			CloseEditor();
			e.Cancel = !UpdateCurrentRowAction();
		}
		protected override void OnChildViewTabChanged(BaseView childView, ViewInfoTabPageChangedEventArgs e) {
			int sourceRowHandle = childView.SourceRowHandle;
			int relIndex = (int)((ViewTabPage)e.Page).Tag;
			bool autoZoom = false;
			if(childView == GridControl.DefaultView) autoZoom = true;
			bool prevAutoZoom = OptionsDetail.AutoZoomDetail, prevAllowZoom = OptionsDetail.AllowZoomDetail;
			if(autoZoom) {
				OptionsDetail.BeginUpdate();
				OptionsDetail.AllowZoomDetail = true;
				OptionsDetail.AutoZoomDetail = true;
				OptionsDetail.CancelUpdate();
			}
			try {
				VisualSetMasterRowExpandedEx(childView.SourceRowHandle, relIndex, true);
			}
			catch(HideException) {
				if(childView.TabControl != null) {
					int vIndex = GetVisibleDetailRelationIndex(childView.SourceRowHandle);
					if(vIndex < childView.TabControl.Pages.Count && vIndex >= 0)
						childView.TabControl.SelectedPage = childView.TabControl.Pages[vIndex];
				}
			}
			finally {
				if(autoZoom) {
					OptionsDetail.BeginUpdate();
					OptionsDetail.AllowZoomDetail = prevAllowZoom;
					OptionsDetail.AutoZoomDetail = prevAutoZoom;
					OptionsDetail.CancelUpdate();
				}
			}
			BaseView view = GetDetailView(sourceRowHandle, relIndex);
			if(view != null) view.Focus();
			if(view == null) view = childView;
			if(view != null && !view.ViewDisposing && view.TabControl != null) {
				view.TabControl.Populate();
				view.TabControl.LayoutChanged();
			}
		}
		public override DevExpress.XtraGrid.Export.BaseExportLink CreateExportLink(DevExpress.XtraExport.IExportProvider provider) {
#pragma warning disable 618
		   return new DevExpress.XtraGrid.Export.GridViewExportLink(this, provider);
#pragma warning restore 618
	   }
		GridOptionsClipboard _optionsClipboard;
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewOptionsClipboard"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public GridOptionsClipboard OptionsClipboard { get { return _optionsClipboard; } }
		ClipboardGridViewImplementor<ColumnImplementer, DataRowImplementer> gridViewImplementorCore = null;
		protected virtual ClipboardGridViewImplementor<ColumnImplementer, DataRowImplementer> gridViewImplementor {
			get {
				if(gridViewImplementorCore == null) gridViewImplementorCore = new ClipboardGridViewImplementor<ColumnImplementer, DataRowImplementer>(this);
				return gridViewImplementorCore;
			}
		}
		ClipboardExportManager<ColumnImplementer, DataRowImplementer> exportManagerCore = null;
		protected virtual ClipboardExportManager<ColumnImplementer, DataRowImplementer> ClipboardManager {
			get {
				if(exportManagerCore == null) exportManagerCore = new ClipboardExportManager<ColumnImplementer, DataRowImplementer>(gridViewImplementor);
				return exportManagerCore;
			}
		}
		public override void CopyToClipboard() {
			if(OptionsClipboard.AllowCopy == DefaultBoolean.False) return;
			BeginShowProgress();
			if(OptionsClipboard.ClipboardMode != ClipboardMode.Formatted || !gridViewImplementor.CanCopyToClipboard()) {
				base.CopyToClipboard();
			} else {
				if(SetDataAwareClipboardData()) {
					EndShowProgress();
					return;
				} else base.CopyToClipboard();
			}
			EndShowProgress();
		}
		void EndShowProgress() {
			if(ProgressWindow != null) {
				ProgressWindow.Dispose();
				ProgressWindow = null;
			}
		}
		void BeginShowProgress() {
			switch(OptionsClipboard.ShowProgress) {
				case ProgressMode.Automatic:
					if(gridViewImplementor.GetSelectedCellsCount() > 10000)
						gridViewImplementor.ShowProgress = true;
					else gridViewImplementor.ShowProgress = false;
					break;
				case ProgressMode.Always:
					gridViewImplementor.ShowProgress = true;
					break;
				case ProgressMode.Never:
					gridViewImplementor.ShowProgress = false;
					break;
			}
			if(gridViewImplementor.ShowProgress) ShowProgressFormForClipboard();
		}
		internal void ShowProgressFormForClipboard() {
			Form form = FindOwnerForm();
			if(form == null) return;
			ProgressWindow = new ProgressWindow();
			ProgressWindow.LookAndFeel.Assign(ElementsLookAndFeel);
			ProgressWindow.SetCaption(XtraPrinting.PrintingSystemActivity.Exporting);
			ProgressWindow.DisableCancel();
			ProgressWindow.ShowCenter(form);
		}
		protected virtual bool SetDataAwareClipboardData() {
			try {
				DataObject data = new DataObject();
				ClipboardManager.AssignOptions(OptionsClipboard);
				ClipboardManager.SetClipboardData(data);
				if(data.GetFormats().Count() == 0) return false;
				Clipboard.SetDataObject(data);
				return true;
			} catch { 
				return false; 
			}
		}
		protected internal override UserControl PrintDesigner {
			get {
				UserControl ctrl = new UserControl();
				GridViewPrinting gvp = new GridViewPrinting();
				gvp.InitFrame(this, "GridView", new Bitmap(16, 16));
				gvp.AutoApply = false;
				gvp.HideCaption();
				gvp.Dock = DockStyle.Fill;
				ctrl.Controls.Add(gvp);
				ctrl.Visible = true;
				ctrl.Dock = DockStyle.Fill;
				ctrl.Size = gvp.UserControlSize;
				return ctrl;
			}
		}
		protected new GridViewPrintInfo PrintInfo { get { return base.PrintInfo as GridViewPrintInfo; } }
		protected internal override bool IsSupportPrinting { get { return true; } }
		protected internal override bool IsRecreateOnMarginChanged { get { return OptionsPrint.AutoWidth; } }
		protected override BaseViewPrintInfo CreatePrintInfoInstance(PrintInfoArgs args) {
			return new GridViewPrintInfo(args);
		}
		GridSummaryComparer summaryComparer = new GridSummaryComparer();
		class GridSummaryComparer : IComparer {
			int IComparer.Compare(object a, object b) {
				if(a == b) return 0;
				if(a == null) return -1;
				if(b == null) return 1;
				GridSummaryItem item1 = ((DictionaryEntry)a).Key as GridSummaryItem;
				GridSummaryItem item2 = ((DictionaryEntry)b).Key as GridSummaryItem;
				return Comparer.Default.Compare(item1.Index, item2.Index);
			}
		}
		protected internal override GroupSummarySortInfoCollection GroupSummarySortInfoCore { get { return GroupSummarySortInfo; } }
		protected internal virtual void OnGroupSummarySortInfoCollectionChanged(CollectionChangeEventArgs e) {
			if(IsLoading) return;
			CheckLoaded();
			SynchronizeSortingAndGrouping();
			FireChanged();
		}
		bool IGridDesignTime.Enabled { get { return true; } }
		bool IGridDesignTime.ForceDesignMode { 
			get { return forceDesignMode; } 
			set { forceDesignMode = value; } }
		protected internal bool ForcedDesignMode { get { return forceDesignMode; } }
		DragDropEffects IGridDesignTime.DoDragDrop(object obj) {
			GridHandler handler = Handler as GridHandler;
			GridControl.Capture = true;
			handler.DoStartDragObject(obj, Size.Empty);
			return DragManager.DragMaster.LastEffect;
		}
		#region Events
		protected internal override bool HasCustomColumnGroupEvent { get { return this.Events[customColumnGroup] != null; } }
		protected internal override void RaiseCustomColumnGroup(CustomColumnSortEventArgs e) {
			CustomColumnSortEventHandler handler = (CustomColumnSortEventHandler)this.Events[customColumnGroup];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseColumnWidthChanged(ColumnEventArgs e) {
			ColumnEventHandler handler = (ColumnEventHandler)this.Events[columnWidthChanged];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseLeftCoordChanged() {
			EventHandler handler = (EventHandler)this.Events[leftCoordChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual bool RaiseGroupRowExpanding(int rowHandle) {
			RowAllowEventHandler handler = (RowAllowEventHandler)this.Events[groupRowExpanding];
			if(handler != null) {
				RowAllowEventArgs e = new RowAllowEventArgs(rowHandle, true);
				handler(this, e);
				return e.Allow;
			}
			return true;
		}
		protected virtual bool RaiseGroupRowCollapsing(int rowHandle) {
			RowAllowEventHandler handler = (RowAllowEventHandler)this.Events[groupRowCollapsing];
			if(handler != null) {
				RowAllowEventArgs e = new RowAllowEventArgs(rowHandle, true);
				handler(this, e);
				return e.Allow;
			}
			return true;
		}
		protected internal virtual ThumbnailImageEventArgs RaiseGetThumbnailImage(ThumbnailImageEventArgs e) {
			GridViewThumbnailImageEventArgs args = (GridViewThumbnailImageEventArgs)e;
			GridViewThumbnailImageEventHandler handler = (GridViewThumbnailImageEventHandler)Events[getThumbnailImage];
			if(handler != null) handler(this, args);
			return e;
		}
		protected internal virtual Image RaiseGetLoadingImage(GetLoadingImageEventArgs e) {
			GetGridViewLoadingImageEventArgs args = (GetGridViewLoadingImageEventArgs)e;
			GetGridViewLoadingImageEventHandler handler = (GetGridViewLoadingImageEventHandler)Events[getLoadingImage];
			if(handler != null) handler(this, args);
			return e.LoadingImage;
		}
		private bool lockChangeExpandCollapse = false;
		protected virtual void OnChangeExpandCollapse(int rowHandle, bool expanded) {
			if(lockChangeExpandCollapse || !IsSplitView) return;
			if(!SplitContainer.GetSynchronizeExpandCollapse()) return;
			this.lockChangeExpandCollapse = true;
			try {
				GridView view = SplitOtherView as GridView;
				if (view != null && view.GroupCount == this.GroupCount) {
					if (expanded) {
						if (rowHandle == InvalidRowHandle) {
							view.ExpandAllGroups();
						}
						else {
							view.ExpandGroupRow(rowHandle);
						}
					}
					else {
						if (rowHandle == InvalidRowHandle) {
							view.CollapseAllGroups();
						}
						else {
							view.CollapseGroupRow(rowHandle);
						}
					}
				}
			} finally {
				this.lockChangeExpandCollapse = false;
			}
		}
		protected virtual void RaiseGroupRowCollapsed(int rowHandle) {
			OnChangeExpandCollapse(rowHandle, false);
			RowEventHandler handler = (RowEventHandler)this.Events[groupRowCollapsed];
			if(handler != null) {
				RowEventArgs e = new RowEventArgs(rowHandle);
				handler(this, e);
			}
		}
		protected virtual void RaiseGroupRowExpanded(int rowHandle) {
			OnChangeExpandCollapse(rowHandle, true);
			RowEventHandler handler = (RowEventHandler)this.Events[groupRowExpanded];
			if(handler != null) {
				RowEventArgs e = new RowEventArgs(rowHandle);
				handler(this, e);
			}
		}
		protected virtual void RaiseMasterRowEmpty(MasterRowEmptyEventArgs e) {
			MasterRowEmptyEventHandler handler = (MasterRowEmptyEventHandler)this.Events[masterRowEmpty];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseMasterRowCollapsed(int rowHandle, int relationIndex) {
			CheckUpdateParentLayout();
			CustomMasterRowEventHandler handler = (CustomMasterRowEventHandler)this.Events[masterRowCollapsed];
			if(handler != null) {
				CustomMasterRowEventArgs e = new CustomMasterRowEventArgs(rowHandle, relationIndex);
				handler(this, e);
			}
		}
		protected virtual void RaiseMasterRowExpanded(int rowHandle, int relationIndex) {
			CheckUpdateParentLayout();
			CustomMasterRowEventHandler handler = (CustomMasterRowEventHandler)this.Events[masterRowExpanded];
			if(handler != null) {
				CustomMasterRowEventArgs e = new CustomMasterRowEventArgs(rowHandle, relationIndex);
				handler(this, e);
			}
		}
		void CheckUpdateParentLayout() {
			if(DetailLevel >= 2 && ParentView != null) { 
				if(ParentView.GetViewInfo() != null) ParentView.GetViewInfo().IsReady = false;
				ParentView.Invalidate();
			}
		}
		[Obsolete()]
		protected virtual void RaiseShowGridMenu(GridMenuEventArgs e) {
			GridMenuEventHandler handler = (GridMenuEventHandler)this.Events[showGridMenu];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaisePopupMenuShowing(PopupMenuShowingEventArgs e) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[onPopupMenuShowing];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseGetPreviewDisplayText(int dataSourceRowIndex, object row, int rowHandle, ref string s) {
			CalcPreviewTextEventHandler handler = (CalcPreviewTextEventHandler)this.Events[calcPreviewText];
			if(handler != null) {
				CalcPreviewTextEventArgs e = new CalcPreviewTextEventArgs(dataSourceRowIndex, rowHandle, row, s);
				handler(this, e);
				s = e.PreviewText;
			}
		}
		protected internal virtual int RaiseMeasurePreviewHeight(int rowHandle) {
			RowHeightEventHandler handler = (RowHeightEventHandler)this.Events[measurePreviewHeight];
			if(handler != null) {
				RowHeightEventArgs e = new RowHeightEventArgs(rowHandle, -1);
				handler(this, e);
				return e.RowHeight;
			}
			return -1;
		}
		protected virtual string RaiseGetRelationName(MasterRowGetRelationNameEventArgs e) {
			MasterRowGetRelationNameEventHandler handler = (MasterRowGetRelationNameEventHandler)this.Events[masterRowGetRelationName];
			if(handler != null) {
				handler(this, e);
			}
			return e.RelationName;
		}
		protected virtual string RaiseGetRelationDisplayCaption(MasterRowGetRelationNameEventArgs e) {
			MasterRowGetRelationNameEventHandler handler = (MasterRowGetRelationNameEventHandler)this.Events[masterRowGetRelationDisplayCaption];
			if(handler != null) {
				handler(this, e);
			}
			return e.RelationName;
		}
		protected internal virtual int RaiseRowHeight(int rowHandle, int rowHeight) {
			RowHeightEventHandler handler = (RowHeightEventHandler)this.Events[calcRowHeight];
			if(handler != null) {
				RowHeightEventArgs e = new RowHeightEventArgs(rowHandle, rowHeight);
				handler(this, e);
				rowHeight = e.RowHeight;
			}
			return rowHeight;
		}
		protected internal virtual void RaiseCustomDrawGroupPanel(CustomDrawEventArgs e) {
			CustomDrawEventHandler handler = (CustomDrawEventHandler)this.Events[customDrawGroupPanel];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawRowFooter(RowObjectCustomDrawEventArgs e) {
			RowObjectCustomDrawEventHandler handler = (RowObjectCustomDrawEventHandler)this.Events[customDrawRowFooter];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawFooter(RowObjectCustomDrawEventArgs e) {
			RowObjectCustomDrawEventHandler handler = (RowObjectCustomDrawEventHandler)this.Events[customDrawFooter];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawRowIndicator(RowIndicatorCustomDrawEventArgs e) {
			RowIndicatorCustomDrawEventHandler handler = (RowIndicatorCustomDrawEventHandler)this.Events[customDrawRowIndicator];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawColumnHeader(EventArgs e) { 
			ColumnHeaderCustomDrawEventHandler handler = (ColumnHeaderCustomDrawEventHandler)this.Events[customDrawColumnHeader];
			if(handler != null) handler(this, e as ColumnHeaderCustomDrawEventArgs);
		}
		protected internal virtual void RaiseCustomDrawFooterCell(FooterCellCustomDrawEventArgs e) {
			FooterCellCustomDrawEventHandler handler = (FooterCellCustomDrawEventHandler)this.Events[customDrawFooterCell];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawRowFooterCell(FooterCellCustomDrawEventArgs e) {
			FooterCellCustomDrawEventHandler handler = (FooterCellCustomDrawEventHandler)this.Events[customDrawRowFooterCell];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawCell(RowCellCustomDrawEventArgs e) {
			RowCellCustomDrawEventHandler handler = (RowCellCustomDrawEventHandler)this.Events[customDrawCell]; 
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawGroupRow(RowObjectCustomDrawEventArgs e) {
			RowObjectCustomDrawEventHandler handler = (RowObjectCustomDrawEventHandler)this.Events[customDrawGroupRow];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawGroupRowCell(RowGroupRowCellEventArgs e) {
			RowGroupRowCellEventHandler handler = (RowGroupRowCellEventHandler)this.Events[customDrawGroupRowCell];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawRowPreview(RowObjectCustomDrawEventArgs e) {
			RowObjectCustomDrawEventHandler handler = (RowObjectCustomDrawEventHandler)this.Events[customDrawRowPreview];
			if(handler != null) handler(this, e);
		}
		bool IsAllowIgnoreStyleEvents() {
			return false;
		}
		protected virtual AppearanceObject RaiseGetRowCellStyle(int rowHandle, GridColumn column, GridRowCellState state, AppearanceObject appearance) {
			RowCellStyleEventHandler handler = (RowCellStyleEventHandler)this.Events[rowCellStyle];
			if(handler == null || IsAllowIgnoreStyleEvents()) return appearance;
			styleEventArgs.state = state;
			styleEventArgs.SetAppearance(appearance);
			styleEventArgs.rowHandle = rowHandle;
			styleEventArgs.column = column;
			handler(this, styleEventArgs);
			return styleEventArgs.Appearance;
		}
		protected internal virtual AppearanceObject RaiseGetRowStyle(int rowHandle, GridRowCellState state, AppearanceObject appearance, out bool highPriority) {
			highPriority = false;
			RowStyleEventHandler handler = (RowStyleEventHandler)this.Events[rowStyle];
			if(handler == null || IsAllowIgnoreStyleEvents()) return appearance;
			RowStyleEventArgs e = new RowStyleEventArgs(rowHandle, state, appearance.Clone() as AppearanceObject);
			handler(this, e);
			highPriority = e.HighPriority;
			return e.Appearance;
		}
		protected internal virtual void RaiseDragObjectDrop(DragObjectDropEventArgs e) {
			DragObjectDropEventHandler handler = (DragObjectDropEventHandler)this.Events[dragObjectDrop];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseBeforePrintRow(CancelPrintRowEventArgs e) {
			BeforePrintRowEventHandler handler = (BeforePrintRowEventHandler)this.Events[beforePrintRow];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseAfterPrintRow(PrintRowEventArgs e) {
			AfterPrintRowEventHandler handler = (AfterPrintRowEventHandler)this.Events[afterPrintRow];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDragObjectOver(DragObjectOverEventArgs e) {
			DragObjectOverEventHandler handler = (DragObjectOverEventHandler)this.Events[dragObjectOver];
			if(handler != null) handler(this, e);
		}
		protected internal virtual bool RaiseDragObjectStart(DragObjectStartEventArgs e) {
			DragObjectStartEventHandler handler = (DragObjectStartEventHandler)this.Events[dragObjectStart];
			if(handler != null) handler(this, e);
			return e.Allow;
		}
		protected internal virtual void RaiseShowingPopupEditForm(ShowingPopupEditFormEventArgs e) {
			ShowingPopupEditFormEventHandler handler = (ShowingPopupEditFormEventHandler)this.Events[showingPopupEditForm];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseHideCustomizationForm(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[hideCustomizationForm];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseShowCustomizationForm(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[showCustomizationForm];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCellMerge(CellMergeEventArgs e) {
			CellMergeEventHandler handler = (CellMergeEventHandler)this.Events[cellMerge];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseRowClick(RowClickEventArgs e) {
			RowClickEventHandler handler = (RowClickEventHandler)this.Events[rowClick];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseRowCellClick(RowCellClickEventArgs e) {
			RowCellClickEventHandler handler = (RowCellClickEventHandler)this.Events[rowCellClick];
			if(handler != null) handler(this, e);
		}
		protected internal bool CanRaiseRowCellUserEvents { get { return IsFocusedRowLoaded; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewBeforePrintRow"),
#endif
 DXCategory(CategoryName.Printing)]
		public event BeforePrintRowEventHandler BeforePrintRow {
			add { this.Events.AddHandler(beforePrintRow, value); }
			remove { this.Events.RemoveHandler(beforePrintRow, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAfterPrintRow"),
#endif
 DXCategory(CategoryName.Printing)]
		public event AfterPrintRowEventHandler AfterPrintRow {
			add { this.Events.AddHandler(afterPrintRow, value); }
			remove { this.Events.RemoveHandler(afterPrintRow, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewRowClick"),
#endif
 DXCategory(CategoryName.Action)]
		public event RowClickEventHandler RowClick {
			add { this.Events.AddHandler(rowClick, value); }
			remove { this.Events.RemoveHandler(rowClick, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewRowCellClick"),
#endif
 DXCategory(CategoryName.Action)]
		public event RowCellClickEventHandler RowCellClick {
			add { this.Events.AddHandler(rowCellClick, value); }
			remove { this.Events.RemoveHandler(rowCellClick, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCellMerge"),
#endif
 DXCategory(CategoryName.Merging)]
		public event CellMergeEventHandler CellMerge {
			add { this.Events.AddHandler(cellMerge, value); }
			remove { this.Events.RemoveHandler(cellMerge, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewDragObjectDrop"),
#endif
 DXCategory(CategoryName.DragDrop)]
		public event DragObjectDropEventHandler DragObjectDrop {
			add { this.Events.AddHandler(dragObjectDrop, value); }
			remove { this.Events.RemoveHandler(dragObjectDrop, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewDragObjectStart"),
#endif
 DXCategory(CategoryName.DragDrop)]
		public event DragObjectStartEventHandler DragObjectStart {
			add { this.Events.AddHandler(dragObjectStart, value); }
			remove { this.Events.RemoveHandler(dragObjectStart, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewDragObjectOver"),
#endif
 DXCategory(CategoryName.DragDrop)]
		public event DragObjectOverEventHandler DragObjectOver {
			add { this.Events.AddHandler(dragObjectOver, value); }
			remove { this.Events.RemoveHandler(dragObjectOver, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomDrawGroupPanel"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event CustomDrawEventHandler CustomDrawGroupPanel {
			add { this.Events.AddHandler(customDrawGroupPanel, value); }
			remove { this.Events.RemoveHandler(customDrawGroupPanel, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomDrawColumnHeader"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event ColumnHeaderCustomDrawEventHandler CustomDrawColumnHeader {
			add { this.Events.AddHandler(customDrawColumnHeader, value); }
			remove { this.Events.RemoveHandler(customDrawColumnHeader, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomDrawRowIndicator"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowIndicatorCustomDrawEventHandler CustomDrawRowIndicator {
			add { this.Events.AddHandler(customDrawRowIndicator, value); }
			remove { this.Events.RemoveHandler(customDrawRowIndicator, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomDrawCell"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowCellCustomDrawEventHandler CustomDrawCell {
			add { this.Events.AddHandler(customDrawCell, value); }
			remove { this.Events.RemoveHandler(customDrawCell, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomDrawRowFooterCell"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event FooterCellCustomDrawEventHandler CustomDrawRowFooterCell {
			add { this.Events.AddHandler(customDrawRowFooterCell, value); }
			remove { this.Events.RemoveHandler(customDrawRowFooterCell, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomDrawFooterCell"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event FooterCellCustomDrawEventHandler CustomDrawFooterCell {
			add { this.Events.AddHandler(customDrawFooterCell, value); }
			remove { this.Events.RemoveHandler(customDrawFooterCell, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomDrawRowPreview"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowObjectCustomDrawEventHandler CustomDrawRowPreview {
			add { this.Events.AddHandler(customDrawRowPreview, value); }
			remove { this.Events.RemoveHandler(customDrawRowPreview, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomDrawGroupRow"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowObjectCustomDrawEventHandler CustomDrawGroupRow {
			add { this.Events.AddHandler(customDrawGroupRow, value); }
			remove { this.Events.RemoveHandler(customDrawGroupRow, value); }
		}
		[ DXCategory(CategoryName.CustomDraw)]
		public event RowGroupRowCellEventHandler CustomDrawGroupRowCell {
			add { this.Events.AddHandler(customDrawGroupRowCell, value); }
			remove { this.Events.RemoveHandler(customDrawGroupRowCell, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomDrawRowFooter"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowObjectCustomDrawEventHandler CustomDrawRowFooter {
			add { this.Events.AddHandler(customDrawRowFooter, value); }
			remove { this.Events.RemoveHandler(customDrawRowFooter, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomDrawFooter"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowObjectCustomDrawEventHandler CustomDrawFooter {
			add { this.Events.AddHandler(customDrawFooter, value); }
			remove { this.Events.RemoveHandler(customDrawFooter, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGetThumbnailImage"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event GridViewThumbnailImageEventHandler GetThumbnailImage {
			add { Events.AddHandler(getThumbnailImage, value); }
			remove { Events.RemoveHandler(getThumbnailImage, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGetLoadingImage"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event GetGridViewLoadingImageEventHandler GetLoadingImage {
			add { Events.AddHandler(getLoadingImage, value); }
			remove { Events.RemoveHandler(getLoadingImage, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGroupLevelStyle"),
#endif
 DXCategory(CategoryName.Appearance)]
		public event GroupLevelStyleEventHandler GroupLevelStyle {
			add { this.Events.AddHandler(groupLevelStyle, value); }
			remove { this.Events.RemoveHandler(groupLevelStyle, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewRowCellStyle"),
#endif
 DXCategory(CategoryName.Appearance)]
		public event RowCellStyleEventHandler RowCellStyle {
			add { this.Events.AddHandler(rowCellStyle, value); }
			remove { this.Events.RemoveHandler(rowCellStyle, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewRowStyle"),
#endif
 DXCategory(CategoryName.Appearance)]
		public event RowStyleEventHandler RowStyle {
			add { this.Events.AddHandler(rowStyle, value); }
			remove { this.Events.RemoveHandler(rowStyle, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMasterRowEmpty"),
#endif
 DXCategory(CategoryName.MasterDetail)]
		public event MasterRowEmptyEventHandler MasterRowEmpty {
			add { this.Events.AddHandler(masterRowEmpty, value); }
			remove { this.Events.RemoveHandler(masterRowEmpty, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMasterRowExpanding"),
#endif
 DXCategory(CategoryName.MasterDetail)]
		public event MasterRowCanExpandEventHandler MasterRowExpanding {
			add { this.Events.AddHandler(masterRowExpanding, value); }
			remove { this.Events.RemoveHandler(masterRowExpanding, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMasterRowExpanded"),
#endif
 DXCategory(CategoryName.MasterDetail)]
		public event CustomMasterRowEventHandler MasterRowExpanded {
			add { this.Events.AddHandler(masterRowExpanded, value); }
			remove { this.Events.RemoveHandler(masterRowExpanded, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMasterRowCollapsing"),
#endif
 DXCategory(CategoryName.MasterDetail)]
		public event MasterRowCanExpandEventHandler MasterRowCollapsing {
			add { this.Events.AddHandler(masterRowCollapsing, value); }
			remove { this.Events.RemoveHandler(masterRowCollapsing, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMasterRowCollapsed"),
#endif
 DXCategory(CategoryName.MasterDetail)]
		public event CustomMasterRowEventHandler MasterRowCollapsed {
			add { this.Events.AddHandler(masterRowCollapsed, value); }
			remove { this.Events.RemoveHandler(masterRowCollapsed, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMasterRowGetLevelDefaultView"),
#endif
 DXCategory(CategoryName.MasterDetail)]
		public event MasterRowGetLevelDefaultViewEventHandler MasterRowGetLevelDefaultView {
			add { this.Events.AddHandler(masterRowGetLevelDefaultView, value); }
			remove { this.Events.RemoveHandler(masterRowGetLevelDefaultView, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMasterRowGetChildList"),
#endif
 DXCategory(CategoryName.MasterDetail)]
		public event MasterRowGetChildListEventHandler MasterRowGetChildList {
			add { this.Events.AddHandler(masterRowGetChildList, value); }
			remove { this.Events.RemoveHandler(masterRowGetChildList, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMasterRowGetRelationName"),
#endif
 DXCategory(CategoryName.MasterDetail)]
		public event MasterRowGetRelationNameEventHandler MasterRowGetRelationName {
			add { this.Events.AddHandler(masterRowGetRelationName, value); }
			remove { this.Events.RemoveHandler(masterRowGetRelationName, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMasterRowGetRelationDisplayCaption"),
#endif
 DXCategory(CategoryName.MasterDetail)]
		public event MasterRowGetRelationNameEventHandler MasterRowGetRelationDisplayCaption {
			add { this.Events.AddHandler(masterRowGetRelationDisplayCaption, value); }
			remove { this.Events.RemoveHandler(masterRowGetRelationDisplayCaption, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMasterRowGetRelationCount"),
#endif
 DXCategory(CategoryName.MasterDetail)]
		public event MasterRowGetRelationCountEventHandler MasterRowGetRelationCount {
			add { this.Events.AddHandler(masterRowGetRelationCount, value); }
			remove { this.Events.RemoveHandler(masterRowGetRelationCount, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewEditFormShowing"),
#endif
 DXCategory(CategoryName.Editor)]
		public event EditFormShowingEventHandler EditFormShowing {
			add { this.Events.AddHandler(editFormShowing, value); }
			remove { this.Events.RemoveHandler(editFormShowing, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewEditFormPrepared"),
#endif
 DXCategory(CategoryName.Editor)]
		public event EditFormPreparedEventHandler EditFormPrepared {
			add { this.Events.AddHandler(editFormPrepared, value); }
			remove { this.Events.RemoveHandler(editFormPrepared, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewShowingPopupEditForm"),
#endif
 DXCategory(CategoryName.Editor)]
		public event ShowingPopupEditFormEventHandler ShowingPopupEditForm {
			add { this.Events.AddHandler(showingPopupEditForm, value); }
			remove { this.Events.RemoveHandler(showingPopupEditForm, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomRowCellEdit"),
#endif
 DXCategory(CategoryName.Editor)]
		public event CustomRowCellEditEventHandler CustomRowCellEdit {
			add { this.Events.AddHandler(customRowCellEdit, value); }
			remove { this.Events.RemoveHandler(customRowCellEdit, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomRowCellEditForEditing"),
#endif
 DXCategory(CategoryName.Editor)]
		public event CustomRowCellEditEventHandler CustomRowCellEditForEditing {
			add { this.Events.AddHandler(customRowCellEditForEditing, value); }
			remove { this.Events.RemoveHandler(customRowCellEditForEditing, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewLeftCoordChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler LeftCoordChanged {
			add { this.Events.AddHandler(leftCoordChanged, value); }
			remove { this.Events.RemoveHandler(leftCoordChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewColumnWidthChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event ColumnEventHandler ColumnWidthChanged {
			add { this.Events.AddHandler(columnWidthChanged, value); }
			remove { this.Events.RemoveHandler(columnWidthChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewTopRowChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler TopRowChanged {
			add { this.Events.AddHandler(topRowChanged, value); }
			remove { this.Events.RemoveHandler(topRowChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGridMenuItemClick"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event GridMenuItemClickEventHandler GridMenuItemClick {
			add { this.Events.AddHandler(gridMenuItemClick, value); }
			remove { this.Events.RemoveHandler(gridMenuItemClick, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewShowGridMenu"),
#endif
 DXCategory(CategoryName.Behavior)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'PopupMenuShowing' instead", false)]
		public event GridMenuEventHandler ShowGridMenu {
			add { this.Events.AddHandler(showGridMenu, value); }
			remove { this.Events.RemoveHandler(showGridMenu, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPopupMenuShowing"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { this.Events.AddHandler(onPopupMenuShowing, value); }
			remove { this.Events.RemoveHandler(onPopupMenuShowing, value); }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCalcRowHeight")
#else
	Description("")
#endif
]
		public event RowHeightEventHandler CalcRowHeight {
			add { this.Events.AddHandler(calcRowHeight, value); }
			remove { this.Events.RemoveHandler(calcRowHeight, value); }
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomSummaryCalculate")
#else
	Description("")
#endif
]
		public event CustomSummaryEventHandler CustomSummaryCalculate {
			add { this.Events.AddHandler(customSummaryCalculate, value); }
			remove { this.Events.RemoveHandler(customSummaryCalculate, value); }
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomSummaryExists")
#else
	Description("")
#endif
]
		public event CustomSummaryExistEventHandler CustomSummaryExists {
			add { this.Events.AddHandler(customSummaryExists, value); }
			remove { this.Events.RemoveHandler(customSummaryExists, value); }
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCalcPreviewText")
#else
	Description("")
#endif
]
		public event CalcPreviewTextEventHandler CalcPreviewText {
			add { this.Events.AddHandler(calcPreviewText, value); }
			remove { this.Events.RemoveHandler(calcPreviewText, value); }
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewMeasurePreviewHeight")
#else
	Description("")
#endif
]
		public event RowHeightEventHandler MeasurePreviewHeight {
			add { this.Events.AddHandler(measurePreviewHeight, value); }
			remove { this.Events.RemoveHandler(measurePreviewHeight, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGroupRowExpanded"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowEventHandler GroupRowExpanded {
			add { this.Events.AddHandler(groupRowExpanded, value); }
			remove { this.Events.RemoveHandler(groupRowExpanded, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGroupRowCollapsed"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowEventHandler GroupRowCollapsed {
			add { this.Events.AddHandler(groupRowCollapsed, value); }
			remove { this.Events.RemoveHandler(groupRowCollapsed, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGroupRowCollapsing"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowAllowEventHandler GroupRowCollapsing {
			add { this.Events.AddHandler(groupRowCollapsing, value); }
			remove { this.Events.RemoveHandler(groupRowCollapsing, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewGroupRowExpanding"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowAllowEventHandler GroupRowExpanding {
			add { this.Events.AddHandler(groupRowExpanding, value); }
			remove { this.Events.RemoveHandler(groupRowExpanding, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewShowCustomizationForm"),
#endif
 DXCategory(CategoryName.Customization)]
		public event EventHandler ShowCustomizationForm {
			add { this.Events.AddHandler(showCustomizationForm, value); }
			remove { this.Events.RemoveHandler(showCustomizationForm, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewHideCustomizationForm"),
#endif
 DXCategory(CategoryName.Customization)]
		public event EventHandler HideCustomizationForm {
			add { this.Events.AddHandler(hideCustomizationForm, value); }
			remove { this.Events.RemoveHandler(hideCustomizationForm, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewCustomColumnGroup"),
#endif
 DXCategory(CategoryName.Sorting)]
		public event CustomColumnSortEventHandler CustomColumnGroup {
			add { this.Events.AddHandler(customColumnGroup, value); }
			remove { this.Events.RemoveHandler(customColumnGroup, value); }
		}
		#endregion
		#region IDataControllerRelationSupport 
		string IDataControllerRelationSupport.GetRelationName(string name, int controllerRow, int relationIndex) {
			return RaiseGetRelationName(new MasterRowGetRelationNameEventArgs(controllerRow, relationIndex, name));
		}
		string IDataControllerRelationSupport.GetRelationDisplayName(string displayName, int controllerRow, int relationIndex) {
			string levelName = displayName;
			BaseView view = null;
			if(view == null)
				view = GetLevelDefault(controllerRow, relationIndex, out levelName);
			if(!string.IsNullOrEmpty(displayName)) levelName = displayName;
			if(view != null) {
				if(view.ViewCaption != "") levelName = view.ViewCaption;
			}
			return RaiseGetRelationDisplayCaption(new MasterRowGetRelationNameEventArgs(controllerRow, relationIndex, levelName));
		}
		bool IDataControllerRelationSupport.IsMasterRowEmpty(bool isEmpty, int controllerRow, int relationIndex) {
			this.isMasterRowEmptyEventArgs.rowHandle = controllerRow;
			this.isMasterRowEmptyEventArgs.relationIndex = relationIndex;
			this.isMasterRowEmptyEventArgs.isEmpty = isEmpty;
			RaiseMasterRowEmpty(this.isMasterRowEmptyEventArgs);
			return this.isMasterRowEmptyEventArgs.IsEmpty;
		}
		IList IDataControllerRelationSupport.GetDetailList(int controllerRow, int relationIndex) {
			return GetChildDataView(controllerRow, relationIndex);
		}
		int IDataControllerRelationSupport.GetRelationCount(int relationCount, int controllerRow) {
			MasterRowGetRelationCountEventHandler handler = (MasterRowGetRelationCountEventHandler)this.Events[masterRowGetRelationCount];
			if(handler != null) {
				MasterRowGetRelationCountEventArgs e = new MasterRowGetRelationCountEventArgs(controllerRow, relationCount);
				handler(this, e);
				relationCount = e.RelationCount;
			}
			return relationCount;
		}
		#endregion IDataControllerRelationSupport 
		#region IDataControllerVisualClient
		bool allowUpdateRowIndexes = true;
		protected override void VisualClientUpdateRowIndexes(int newTopRowIndex) {
			if(IsLockUpdate) return;
			if(!this.allowUpdateRowIndexes || ViewInfo.RowsInfo.GetFirstForcedRowIndexLight() != -1) {
				VisualClientUpdateRows(0);
				return;
			}
			newTopRowIndex = Math.Max(0, newTopRowIndex);
			ViewInfo.UpdateRowIndexes(newTopRowIndex);
			this.topRowIndex = newTopRowIndex;
			VisualClientUpdateScrollBar();
		}
		protected override void VisualClientUpdateRows(int topRowIndexDelta) {
			if(IsLockUpdate) return;
			if(GridControl != null && GridControl.DefaultView != this) {
				VisualClientUpdateLayout();
				return;
			}
			if(topRowIndexDelta != 0 && TopRowIndex > 0) {
				this.requireCheckTopRowIndex = true;
				this.topRowIndex = Math.Max(0, TopRowIndex + topRowIndexDelta);
			}
			if(ActiveEditor != null && topRowIndexDelta == 0) {
				RefreshRows(false, false);
			}
			else {
				SetLayoutDirty();
			}
		}
		protected override void VisualClientUpdateLayout() {
			if(IsLockUpdate || ViewInfo == null) return;
			if(GridControl == null) return;
			if(GridControl.DefaultView == this)
				SetLayoutDirty();
			else {
				SetLayoutDirty();
				ViewInfo.IsReady = false;
			}
			CheckDataControllerError();
		}
		protected override void VisualClientUpdateScrollBar() {
			if(IsLockUpdate) return;
			UpdateScrollBars();
		}
		protected override void VisualClientUpdateTotalSummary() {
			if(IsLockUpdate) return;
			if(FormatRules.Count > 0) {
				FormatRules.ResetValuesReady();
				LayoutChangedSynchronized();
				return;
			}
			ViewInfo.FooterInfo.SetDirty();
			InvalidateFooter();
		}
		protected override int VisualClientTopRowIndex { get { return TopRowIndex; } }
		protected override int VisualClientPageRowCount { get { return ViewInfo.RowsInfo.Count + 1; } }
		protected override int VisualClientVisibleRowCount { get { return ViewInfo.RowsLoadInfo == null ? -1 : ViewInfo.RowsLoadInfo.VisibleRowCount; } }
		#endregion IDataControllerVisualClient
		#region IDataControllerValidationSupport
		IBoundControl IDataControllerValidationSupport.BoundControl { get { return GridControl; } }
		void IDataControllerValidationSupport.OnStartNewItemRow() {
			if(DesignMode) return;
			if(OptionsView.NewItemRowPosition == NewItemRowPosition.None) LayoutChangedSynchronized();
			FocusedRowHandle = CurrencyDataController.NewItemRow;
			if(FocusedRowHandle == CurrencyDataController.NewItemRow) {
				DataController.BeginCurrentRowEdit();
			}
			if(IsDetailView) {
				CheckViewInfo();
				CheckTopRowIndex();
				MakeRowVisible(FocusedRowHandle, false);
			}
			RaiseInitNewRow(new InitNewRowEventArgs(CurrencyDataController.NewItemRow));
		}
		void IDataControllerValidationSupport.OnEndNewItemRow() {
			if(IsInplaceEditFormVisible && GridControl != null && GridControl.IsHandleCreated) {
				GridControl.BeginInvoke(new MethodInvoker(OnEndNewItemRowCore));
			}
			else {
				OnEndNewItemRowCore();
			}
		}
		void OnEndNewItemRowCore() {
			if(OptionsView.NewItemRowPosition == NewItemRowPosition.None) {
				LayoutChangedSynchronized();
				if(GridControl != null && FocusedRowHandle == CurrencyDataController.NewItemRow) {
					if(GridControl.IsHandleCreated) GridControl.BeginInvoke(new MethodInvoker(MoveLastVisibleDelayed));
				}
			}
			RefreshRow(CurrencyDataController.NewItemRow);
		}
		void MoveLastVisibleDelayed() {
			if(FocusedRowHandle == CurrencyDataController.NewItemRow) MoveLastVisible();
		}
		void IDataControllerValidationSupport.OnBeginCurrentRowEdit() { }
		void IDataControllerValidationSupport.OnCurrentRowUpdated(ControllerRowEventArgs e) { 
			RaiseRowUpdated(new RowObjectEventArgs(e.RowHandle, e.Row));
			UpdateNavigator();
		}
		void IDataControllerValidationSupport.OnValidatingCurrentRow(ValidateControllerRowEventArgs e) {
			if(e.RowHandle != FocusedRowHandle) return;
			OnValidatingCurrentRow(e);
		}
		void IDataControllerValidationSupport.OnPostRowException(ControllerRowExceptionEventArgs e) { 
			OnPostRowException(e);
		}
		void IDataControllerValidationSupport.OnPostCellException(ControllerRowCellExceptionEventArgs e) { 
			throw e.Exception;
		}
		protected override void OnCurrentControllerRowChanged(CurrentRowEventArgs e) { 
			if(IsFilterRow(FocusedRowHandle)) {
				return;
			}
			bool lockRowVisible = false;
			if(IsInListChangedEvent && !OptionsBehavior.KeepFocusedRowOnUpdate) {
				lockRowVisible = true;
				lockMakeRowVisible++;
			}
			try {
			FocusedRowHandle = DataController.CurrentControllerRow;
			if(!IsInitialized) return;
			MakeRowVisible(FocusedRowHandle, true);
			}
			finally {
				if(lockRowVisible) lockMakeRowVisible--;
			}
		}
		void IDataControllerValidationSupport.OnControllerItemChanged(ListChangedEventArgs e) { 
			if(OptionsNavigation.AutoFocusNewRow && e.ListChangedType == ListChangedType.ItemAdded) {
				if(IsValidRowHandle(e.NewIndex)) {
					FocusedRowHandle = e.NewIndex;
					ClearSelection();
					SelectRow(FocusedRowHandle);
				}
			}
			if(e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted) {
				if(e.ListChangedType == ListChangedType.ItemDeleted && IsEditing && FocusedRowHandle == e.NewIndex) HideEditor();
				if(ViewInfo != null) ViewInfo.AutoRowHeightCache.Clear();
				ClearPrevSelectionInfo();
				ClearColumnErrors();
				DataController.SyncCurrentRow();
			}
		}
		#endregion IDataControllerValidationSupport
		int IAccessibleGrid.HeaderCount { get {	return GetAccessibleHeaderCount();	} }
		int IAccessibleGrid.RowCount { 
			get { 
				int res = RowCount + (OptionsView.ShowAutoFilterRow ? 1 : 0);
				if(OptionsView.NewItemRowPosition == NewItemRowPosition.Top) res ++;
				return res;
			} 
		}
		int IAccessibleGrid.SelectedRow { get { return RowHandle2AccessibleIndex(FocusedRowHandle);	} }
		ScrollBarBase IAccessibleGrid.HScroll { get { return ScrollInfo.HScrollVisible ? ScrollInfo.HScroll : null; } }
		ScrollBarBase IAccessibleGrid.VScroll { get { return ScrollInfo.VScrollVisible ? ScrollInfo.VScroll : null; } }
		IAccessibleGridRow IAccessibleGrid.GetRow(int index) { 
			int rowHandle = AccessibleIndex2RowHandle(index);
			if(!IsValidRowHandle(rowHandle)) return null;
			if(IsGroupRow(rowHandle)) return new DevExpress.XtraGrid.Accessibility.GridAccessibleGroupRow(this, rowHandle);
			return new DevExpress.XtraGrid.Accessibility.GridAccessibleDataRow(this, rowHandle);
		} 
		int IAccessibleGrid.FindRow(int x, int y) { 
			GridRowInfo row = ViewInfo.RowsInfo.GetInfo(x, y);
			if(row != null) return RowHandle2AccessibleIndex(row.RowHandle);
			return -1;
		}
		protected virtual int GetAccessibleHeaderCount() { return VisibleColumns.Count; }
		protected internal override int AccessibleIndex2RowHandle(int index) {
			if(OptionsView.ShowAutoFilterRow) {
				if(index-- == 0) return GridControl.AutoFilterRowHandle;
			}
			if(OptionsView.NewItemRowPosition == NewItemRowPosition.Top) {
				if(index-- == 0) return GridControl.NewItemRowHandle;
			}
			return GetVisibleRowHandle(index);
		}
		protected internal override int RowHandle2AccessibleIndex(int rowHandle) {
			int vindex = GetVisibleIndex(rowHandle);
			if(vindex > -1) return VisibleIndex2AccessibleIndex(vindex);
			if(IsFilterRow(rowHandle)) return 0;
			if(IsNewItemRow(rowHandle) && OptionsView.NewItemRowPosition == NewItemRowPosition.Top) {
				return 0 + (OptionsView.ShowAutoFilterRow ? 1 : 0);
			}
			return -1;
		}
		protected internal override int VisibleIndex2AccessibleIndex(int visibleIndex) {
			if(visibleIndex < 0) return -1;
			visibleIndex += OptionsView.ShowAutoFilterRow ? 1 : 0;
			visibleIndex += OptionsView.NewItemRowPosition == NewItemRowPosition.Top ? 1 : 0;
			return visibleIndex;
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() { return new DevExpress.XtraGrid.Accessibility.GridViewAccessibleObject(this); }
		void IGridLookUp.Setup() {
			BeginUpdate();
			try {
				FocusRectStyle = DrawFocusRectStyle.RowFocus;
				OptionsView.ShowGroupPanel = false;
				OptionsSelection.EnableAppearanceFocusedCell = false;
			} finally {
				EndUpdate();
			}
		}
		protected override bool AllowAutoPopulateColumns { 
			get { 
				if(!base.AllowAutoPopulateColumns) return false;
				if(LookUpOwner != null && LookUpOwner.IsDesignMode) return false;
				return true;
			} 
		}
		string extraFilter = string.Empty, extraFilterText = string.Empty;
		internal bool firstMouseEnter = true;
		void IGridLookUp.Show(object editValue, string filterText) {
			this.firstMouseEnter = true;
			if(LookUpOwner == null) return;
			if(Columns.Count == 0 && AllowAutoPopulateColumns) PopulateColumns();
			((IGridLookUp)this).SetDisplayFilter(filterText);
			if(LookUpOwner.TextEditStyle == TextEditStyles.DisableTextEditor && RowCount == 0 && ExtraFilterText != string.Empty) ((IGridLookUp)this).SetDisplayFilter(string.Empty);
			int rowHandle = DataController.FindRowByValue(LookUpOwner.ValueMember, editValue, delegate(object args) {
				int row = (int)args;
				if(row >= 0) {
					FocusedRowHandle = row;
					MakeRowVisible(FocusedRowHandle, false);
				}
				else {
					FocusedRowHandle = InvalidRowHandle;
				}
			});
			if(rowHandle == AsyncServerModeDataController.OperationInProgress) {
				rowHandle = FocusedRowHandle;
			}
			BeginUpdate();
			try {
				TopRowIndex = 0;
				FocusedRowHandle = rowHandle;
			} finally {
				EndUpdate();
			}
			MakeRowVisible(FocusedRowHandle, false);
		}
		void IGridLookUp.SetDisplayFilter(string text) {
			if(LookUpOwner == null) return;
			string displayMember = LookUpOwner.DisplayMember;
			this.extraFilterText = this.extraFilter = string.Empty;
			string filter = RowFilter;
			if(text == null) text = string.Empty;
			if(LookUpOwner.GetFilterMode() == PopupFilterMode.Default) {
				this.extraFilterText = this.extraFilter = string.Empty;
				ApplyFindFilter(text);
			}
			else {
				if(LookUpDisplayColumn == null && DataController.Columns[displayMember] == null) return;
				this.extraFilterText = text;
				if(text == "") {
					if(Equals(DataController.FilterCriteria, ActiveFilterCriteria))
						return;
				}
				else {
					this.extraFilter = OnCreateLookupDisplayFilter(text, displayMember);
				}
				ApplyColumnsFilterEx();
			}
			if(!LookUpOwner.OwnerEdit.IsAutoComplete) FocusedRowHandle = InvalidRowHandle;
		}
		protected virtual string OnCreateLookupDisplayFilter(string text, string displayMember) {
			FunctionOperatorType opType = LookUpOwner.GetFilterMode() == PopupFilterMode.StartsWith ? FunctionOperatorType.StartsWith : FunctionOperatorType.Contains;
			CriteriaOperator op = new FunctionOperator(opType, new OperandProperty(displayMember), text);
			return CriteriaOperator.ToString(op);
		}
		protected internal override string ExtraFilterText { get { return extraFilterText; } }
		protected internal override string ExtraFilter { get { return extraFilter; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridViewEditable")]
#endif
		public override bool Editable { get { return base.Editable && !WorkAsLookup; } }
		public void GuessAutoFilterRowValuesFromFilter() {
			if(!OptionsView.ShowAutoFilterRow)
				return;
			foreach(GridColumn c in Columns) {
				if(!c.Visible)
					continue;
				ColumnFilterInfo fi = c.FilterInfo;
				if(fi == null)
					continue;
				if(fi.Type != ColumnFilterType.Custom)
					continue;
				AutoFilterCondition cond = ResolveAutoFilterCondition(c);
				BinaryOperator bop = fi.FilterCriteria as BinaryOperator;
				if(!ReferenceEquals(null, bop)) {
					if(cond != AutoFilterCondition.Equals)
						continue;
					if(bop.OperatorType != BinaryOperatorType.Equal)
						continue;
					OperandValue ov = bop.RightOperand as OperandValue;
					if(ReferenceEquals(null, ov))
						continue;
					object val = ov.Value;
					if(val == null)
						continue;
					c.FilterInfo = new ColumnFilterInfo(ColumnFilterType.AutoFilter, val, fi.FilterString);
				}
				FunctionOperator fop = fi.FilterCriteria as FunctionOperator;
				if(!ReferenceEquals(null, fop)) {
					if(fop.Operands.Count != 2)
						continue;
					OperandValue ov = fop.Operands[1] as OperandValue;
					if(ReferenceEquals(null, ov))
						continue;
					string val = ov.Value as string;
					if(string.IsNullOrEmpty(val))
						continue;
					if(cond == AutoFilterCondition.Contains && fop.OperatorType == FunctionOperatorType.Contains) {
						c.FilterInfo = new ColumnFilterInfo(ColumnFilterType.AutoFilter, val, fi.FilterString);
					} else if(cond == AutoFilterCondition.Like && fop.OperatorType == FunctionOperatorType.Contains) {
						c.FilterInfo = new ColumnFilterInfo(ColumnFilterType.AutoFilter, '%' + val, fi.FilterString);
					} else if(cond == AutoFilterCondition.Like && fop.OperatorType == FunctionOperatorType.StartsWith) {
						c.FilterInfo = new ColumnFilterInfo(ColumnFilterType.AutoFilter, val, fi.FilterString);
					}
				}
			}
		}
		public static bool? GuessAutoFilterRowValuesFromFilterAfterRestoreLayout;
		protected override void OnEndDeserializing(string restoredVersion) {
			base.OnEndDeserializing(restoredVersion);
			if(GuessAutoFilterRowValuesFromFilterAfterRestoreLayout == true)
				GuessAutoFilterRowValuesFromFilter();
		}
		protected internal override string GetNonFormattedCaption(string caption) {
			if(!OptionsView.AllowHtmlDrawHeaders) return caption;
			return StringPainter.Default.RemoveFormat(caption);
		}
		internal bool IsColumnFiltered(GridColumn column) {
			return GridCriteriaHelper.IsColumnFiltered(column, ActiveFilter.NonColumnFilterCriteria);
		}
		#region IXtraSupportDeserializeCollection Members
		void IXtraSupportDeserializeCollection.AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			OnAfterDeserializeCollection(propertyName, e);
		}
		void IXtraSupportDeserializeCollection.BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			OnBeforeDeserializeCollection(propertyName, e);
		}
		bool IXtraSupportDeserializeCollection.ClearCollection(string propertyName, XtraItemEventArgs e) { return false;  }
		protected virtual void OnAfterDeserializeCollection(object propertyName, XtraItemEventArgs e) { }
		protected virtual void OnBeforeDeserializeCollection(object propertyName, XtraItemEventArgs e) { }
		#endregion
		#region ISummaryItemsOwner Members
		void ISummaryItemsOwner.SetItems(List<ISummaryItem> items) {
			try {
				GroupSummary.BeginUpdate();
				for(int i = GroupSummary.Count - 1; i >= 0; i--) {
					GridGroupSummaryItem item = GroupSummary[i] as GridGroupSummaryItem;
					if(item == null) {
						GroupSummary.RemoveAt(i);
						continue;
					}
					if(item.ShowInGroupColumnFooter == null)
						GroupSummary.RemoveAt(i);
				}
				foreach (ISummaryItem item in items) {
					GroupSummary.Add((GridGroupSummaryItem)item);
				}
			} finally {
				GroupSummary.EndUpdate();
			}
		}
		List<ISummaryItem> ISummaryItemsOwner.GetItems() {
			List<ISummaryItem> items = new List<ISummaryItem>();
			foreach (GridGroupSummaryItem item in GroupSummary) {
				if(item.ShowInGroupColumnFooter == null)
				items.Add(item);
			}
			return items;
		}
		ISummaryItem ISummaryItemsOwner.CreateItem(string fieldName, SummaryItemType summaryType) {
			GridGroupSummaryItem summaryItem = new GridGroupSummaryItem();
			summaryItem.FieldName = fieldName;
			summaryItem.SummaryType = summaryType;
			summaryItem.DisplayFormat = GetSummaryItemDisplayFormat(summaryItem);
			return summaryItem;
		}
		List<string> ISummaryItemsOwner.GetFieldNames() {
			List<string> list = new List<string>();
			foreach (GridColumn column in Columns) {
				if (CanBeUsedInGroupSummary(column)) {
					list.Add(column.FieldName);
				}
			}
			return list;
		}
		string ISummaryItemsOwner.GetCaptionByFieldName(string fieldName) {
			return Columns[fieldName].GetTextCaption();
		}
		Type ISummaryItemsOwner.GetTypeByFieldName(string fieldName) {
			return Columns[fieldName].ColumnType;
		}
		protected virtual bool CanBeUsedInGroupSummary(GridColumn column) {
			return CanDataSummaryColumn(column) && (column.Visible || column.CanShowInCustomizationForm);
		}
		internal string GetSummaryItemDisplayFormat(GridGroupSummaryItem summaryItem) {
			SummaryItemType summaryType = summaryItem.SummaryType;
			if ((summaryType == SummaryItemType.Count && string.IsNullOrEmpty(summaryItem.FieldName)) || summaryType == SummaryItemType.None ||
				summaryType == SummaryItemType.Custom) return string.Empty;
			return "(" + Columns[summaryItem.FieldName].GetTextCaption() + ": " + CalcSummaryItemFormat(summaryItem, summaryType) + ")";
		}
		string CalcSummaryItemFormat(GridGroupSummaryItem summaryItem, SummaryItemType summaryType) {
			string ret = summaryItem.GetDisplayFormatByType(summaryType);
			GridColumn col = Columns[summaryItem.FieldName];
			if(col != null && summaryType != SummaryItemType.Count &&
				col.DisplayFormat.FormatType != FormatType.None && col.DisplayFormat.Format != null &&
				!string.IsNullOrEmpty(col.DisplayFormat.FormatString) && col.ColumnType != typeof(TimeSpan)) {
				ret = GetSummaryFormat(col, summaryType);
			}
			return ret;
		}
		#endregion
		public string GetSummaryFormat(GridColumn column, SummaryItemType sumType) {
			string res = "";
			switch(sumType) {
				case SummaryItemType.Sum: res = GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterSumFormat); break;
				case SummaryItemType.Min: res = GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterMinFormat); break;
				case SummaryItemType.Max: res = GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterMaxFormat); break;
				case SummaryItemType.Count: res = GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterCountFormat); break;
				case SummaryItemType.Average: res = GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterAverageFormat); break;
			}
			if(this.FootersIgnoreColumnFormat || sumType == SummaryItemType.Count ||
				(column.DisplayFormat.FormatType == FormatType.None && column.DisplayFormat.Format == null)
				|| string.IsNullOrEmpty(column.DisplayFormat.FormatString)) return res;
			try {
				res = string.Format(res, column.DisplayFormat.GetFormatString());
			}
			catch {
			}
			return res;
		}
		protected internal virtual bool AllowAddNewSummaryItemMenu {
			get {
				return OptionsMenu.ShowAddNewSummaryItem != DefaultBoolean.False;
			}
		}
		protected override void InitializeVisualParameters() {
			CheckCheckboxSelector();
			base.InitializeVisualParameters();
		}
		protected virtual void UpdateCheckboxSelectorColumnValue(int rowHandle) {
			if(CheckboxSelectorColumn == null) return;
			if(rowHandle == InvalidRowHandle) {
				foreach(GridRowInfo r in ViewInfo.RowsInfo) {
					GridDataRowInfo dr = r as GridDataRowInfo;
					GridGroupRowInfo group = r as GridGroupRowInfo;
					if(dr != null) {
						UpdateCheckboxSelectorCell(dr, dr.Cells[CheckboxSelectorColumn]);
					}
					if(group != null) group.SetSelectorStateDirty();
				}
			}
			GridRowInfo row = ViewInfo.RowsInfo.GetInfoByHandle(rowHandle);
			GridDataRowInfo rowInfo = row as GridDataRowInfo;
			GridGroupRowInfo groupInfo = row as GridGroupRowInfo;
			if(rowInfo != null) {
				UpdateCheckboxSelectorCell(rowInfo, rowInfo.Cells[CheckboxSelectorColumn]);
			}
			if(groupInfo != null) groupInfo.SetSelectorStateDirty();
		}
		void UpdateCheckboxSelectorCell(GridDataRowInfo dr, GridCellInfo cell) {
			if(cell == null) return;
			if(dr.IsNewItemRow || dr.IsSpecialRow) return;
			cell.CellValue = IsRowSelected(dr.RowHandle);
			if(cell.ViewInfo != null) cell.ViewInfo.EditValue = cell.CellValue;
		}
		protected internal virtual bool IsCheckboxSelector(GridColumn column) {
			return checkboxSelectorColumn == column;
		}
		protected internal virtual bool IsCheckboxSelectorHitInfo(BaseHitInfo hitInfo) {
			GridHitInfo gridHitInfo = hitInfo as GridHitInfo;
			return CheckboxSelectorColumn != null && gridHitInfo != null && gridHitInfo.Column == CheckboxSelectorColumn;
		}
		protected virtual void DestroyCheckboxSelector() {
			if(checkboxSelectorColumn != null) {
				checkboxSelectorColumn.view = null;
				checkboxSelectorColumn.Dispose();
			}
			this.checkboxSelectorColumn = null;
		}
		protected internal virtual bool IsCheckboxSelectorMode { get { return OptionsSelection.MultiSelectMode == GridMultiSelectMode.CheckBoxRowSelect && IsMultiSelect; } }
		protected virtual void CheckCheckboxSelector() {
			if(IsLoading) return;
			if(IsCheckboxSelectorMode) {
				CreateCheckboxSelectorColumn();
				if(CheckboxSelectorColumn != null) CheckboxSelectorColumn.width = CheckBoxSelectorColumnWidth;
			}
			else {
				DestroyCheckboxSelector();
			}
			RefreshVisibleColumnsList();
		}
		protected int CheckBoxSelectorColumnWidth {
			get {
				if(OptionsSelection.CheckBoxSelectorColumnWidth > 0) return OptionsSelection.CheckBoxSelectorColumnWidth;
				return GridColumn.defaultColumnWidth;
			}
		}
		protected override void PreProcessVisibleColumnsList(ArrayList tempList) {
			if(AllowPartialGroups || IsAlignGroupRowSummariesUnderColumns) {
				for(int n = SortInfo.GroupCount - 1; n >= 0; n--) {
					var columnInfo = SortInfo[n];
					if(columnInfo != null) {
						int i = tempList.IndexOf(columnInfo.Column);
						if(i != 0) {
							if(i != -1) tempList.Remove(columnInfo.Column);
							tempList.Insert(0, columnInfo.Column);
						}
					}
				}
			}
			if(CheckboxSelectorColumn == null) return;
			if(tempList.Contains(CheckboxSelectorColumn)) return;
			tempList.Insert(0, CheckboxSelectorColumn);
		}
		protected override void SetUnboundDataCore(int listSourceRow, DataColumnInfo column, object value) {
			if(column.Name == CheckBoxSelectorColumnName) {
				int row = GetRowHandle(listSourceRow);
				if((bool)value)
					SelectRow(row);
				else
					UnselectRow(row);
				return;
			}
			base.SetUnboundDataCore(listSourceRow, column, value);
		}
		protected internal override void DoAfterMoveFocusedRow(KeyEventArgs e, int prevFocusedHandle, GridColumn prevFocusedColumn, BaseHitInfo hitInfo, bool allowCells) {
			if(CheckboxSelectorColumn != null) {
				if(FocusedColumn == CheckboxSelectorColumn || !OptionsSelection.ResetSelectionClickOutsideCheckboxSelector) {
					if((hitInfo == null || hitInfo.HitTestInt != (int)GridHitTest.RowIndicator) && (e.KeyCode == Keys.LButton || e.KeyCode == Keys.RButton)) return;
				}
			}
			base.DoAfterMoveFocusedRow(e, prevFocusedHandle, prevFocusedColumn, hitInfo, allowCells);
		}
		protected internal override void UpdateColumnHandles() {
			base.UpdateColumnHandles();
			if(CheckboxSelectorColumn != null) CheckboxSelectorColumn.UpdateHandle();
		}
		GridColumn checkboxSelectorColumn = null;
		protected internal GridColumn CheckboxSelectorColumn { get { return checkboxSelectorColumn; } }
		protected virtual void CreateCheckboxSelectorColumn() {
			if(checkboxSelectorColumn != null || GridControl == null) return;
			this.checkboxSelectorColumn = Columns.CreateColumn();
			this.checkboxSelectorColumn.Name = checkboxSelectorColumn.FieldName = UseBoundCheckBoxSelector ? DataBoundSelectionField : CheckBoxSelectorColumnName;
			this.checkboxSelectorColumn.Caption = Localization.GridLocalizer.Active.GetLocalizedString(GridStringId.CheckboxSelectorColumnCaption);
			this.checkboxSelectorColumn.OptionsColumn.ShowInCustomizationForm = false;
			this.checkboxSelectorColumn.OptionsColumn.FixedWidth = true;
			this.checkboxSelectorColumn.OptionsColumn.AllowSize = false;
			this.checkboxSelectorColumn.OptionsColumn.AllowMove = false;
			this.checkboxSelectorColumn.OptionsColumn.AllowShowHide = false;
			this.checkboxSelectorColumn.Width = CheckBoxSelectorColumnWidth;
			this.checkboxSelectorColumn.VisibleIndex = 0;
			this.checkboxSelectorColumn.Visible = true;
			this.checkboxSelectorColumn.OptionsColumn.AllowSort = DefaultBoolean.False;
			this.checkboxSelectorColumn.OptionsColumn.AllowGroup = DefaultBoolean.False;
			this.checkboxSelectorColumn.UnboundType = UnboundColumnType.Boolean;
			this.checkboxSelectorColumn.view = this;
			OnColumnUnboundChanged(checkboxSelectorColumn);
			this.checkboxSelectorColumn.UpdateHandle();
		}
		protected virtual object GetCheckboxSelectorValue(int listSourceRow) {
			return DataController.Selection.GetSelectedByListSource(listSourceRow);
		}
		protected internal virtual bool CheckboxSelectorColumnFocusChangeSelection {
			get {
				if(OptionsSelection.ShowCheckBoxSelectorChangesSelectionNavigation != DefaultBoolean.False) return false;
				return true;
			}
		}
		protected internal override bool AllowChangeSelectionOnNavigation {
			get {
				if(CheckboxSelectorColumn == null) return true;
				if(!CheckboxSelectorColumnFocusChangeSelection) return false;
				return true;
			}
		}
		protected internal void ShowEditorOnMouse(bool onMouseDown) {
			if(FocusedColumn == CheckboxSelectorColumn && CheckboxSelectorColumn != null) {
				if(!onMouseDown && (GetShowEditorMode() == EditorShowMode.MouseDown || GetShowEditorMode() == EditorShowMode.MouseDownFocused)) return;
				InvertRowSelection(FocusedRowHandle);
				return;
			}
			if(onMouseDown) {
				ShowEditorByMouse();
			}
			else {
				ShowEditor();
			}
		}
		protected override bool CheckAllowChangeSelectionOnNavigation(KeyEventArgs e) {
			if(!base.CheckAllowChangeSelectionOnNavigation(e)) return false;
			if(e.Modifiers == Keys.None) {
				if(IsGroupRow(FocusedRowHandle) && IsShowCheckboxSelectorInGroupRow) {
					return false;
				}
			}
			return true;
		}
		protected virtual bool IsGroupSelected(int rowHandle) {
			if(!IsMultiSelect) return false;
			return IsRowSelected(rowHandle);
		}
		protected internal int GetGroupSelectedCount(int rowHandle, bool stopOnFirstUnselected, out bool allSelected) {
			allSelected = true;
			int selCount = IsRowSelected(rowHandle) ? 1 : 0;
			if(selCount == 0) {
				allSelected = false;
				if(stopOnFirstUnselected) return selCount;
			}
			int childCount = GetChildRowCount(rowHandle);
			for(int i = 0; i < childCount; i++) {
				int row = GetChildRowHandle(rowHandle, i);
				if(IsGroupRow(row)) {
					bool sel;
					selCount += GetGroupSelectedCount(row, stopOnFirstUnselected, out sel);
					if(!sel) allSelected = false;
				}
				else {
					bool selected = IsRowSelected(row);
					if(selected) 
						selCount++;
					else 
						allSelected = false;
				}
				if(!allSelected && stopOnFirstUnselected) return selCount;
			}
			return selCount;
		}
		protected internal int GetGroupSelectionState(int rowHandle) { 
			bool allSelected;
			int selCount = GetGroupSelectedCount(rowHandle, false, out allSelected);
			if(allSelected) {
				return 2;
			}
			else {
				if(selCount == 0) return 0;
			}
			return 1;
		}
		protected internal virtual void ChangeGroupRowSelection(int rowHandle) {
			int state = GetGroupSelectionState(rowHandle);
			if(state != 2) {
				SelectAllGroupRows(rowHandle);
			}
			else {
				UnselectAllGroupRows(rowHandle);
			}
		}
		protected void UnselectAllGroupRows(int rowHandle) {
			SetGroupRowSelection(rowHandle, false);
		}
		protected void SelectAllGroupRows(int rowHandle) {
			SetGroupRowSelection(rowHandle, true);
		}
		void SetGroupRowSelection(int rowHandle, bool selected) {
			int childCount = GetChildRowCount(rowHandle);
			if(selected)
				SelectRow(rowHandle);
			else
				UnselectRow(rowHandle);
			for(int i = 0; i < childCount; i++) {
				int row = GetChildRowHandle(rowHandle, i);
				if(selected)
					SelectRow(row);
				else
					UnselectRow(row);
				if(IsGroupRow(row)) {
					SetGroupRowSelection(row, selected);
				}
			}
		}
		protected internal bool IsAllRowsSelected {
			get {
				if(DataControllerCore == null) return false;
				int allRowCount = DataController.VisibleListSourceRowCount + DataController.GroupRowCount;
				return SelectedRowsCount == allRowCount;
			}
		}
		protected internal void SelectAllRows() {
			BeginSelection();
			try {
				if(GroupCount == 0) {
					SelectAll();
				}
				else {
					for(int n = 0; n < DataController.VisibleListSourceRowCount; n++) SelectRow(n);
					for(int n = 0; n < DataController.GroupRowCount; n++) {
						SelectRow(DataController.GroupInfo[n].Handle);
					}
				}
			}
			finally {
				EndSelection();
			}
		}
		protected internal void InvertAllRowsSelection() {
			if(IsAllRowsSelected) {
				ClearSelection();
			}
			else {
				SelectAllRows();
			}
		}
		protected internal virtual bool IsShowCheckboxSelectorInPrintExport {
			get {
				return OptionsSelection.ShowCheckBoxSelectorInPrintExport != DefaultBoolean.False;
			}
		}
		protected internal virtual bool IsShowEditorKey(Keys keyData) {
			if(IsEditFormMode) {
				return keyData == Keys.Enter || keyData == Keys.F2;
			}
			RepositoryItem ritem = GetRowCellRepositoryItem(FocusedRowHandle, FocusedColumn);
			if(ritem != null && ritem.IsActivateKey(keyData)) {
				return true;
			}
			return false;
		}
		protected internal virtual void FocusedEditForm() {
			if(!IsInplaceEditFormVisible) return;
			EditFormController.EditForm.RootPanel.Focus();
		}
		protected internal override bool CanDesignerSelectColumn(GridColumn gridColumn) {
			return gridColumn != CheckboxSelectorColumn;
		}
		Padding userCellPadding = new Padding(0);
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Padding UserCellPadding {
			get { return userCellPadding; }
			set {
				if(userCellPadding == value) return;
				this.userCellPadding = value;
				LayoutChangedSynchronized();
			}
		}
		protected internal virtual Padding GetRowCellPadding(GridColumn column) {
			return Padding.Empty;
		}
		protected internal virtual bool AllowFixedCheckboxSelectorColumn { get { return false; } }
		internal void SuspendImmediateUpdateRowPosition(GridColumn column) {
			if(column != null && column.RealColumnEdit != null) {
				DataController.ImmediateUpdateRowPosition = column.OptionsColumn.ImmediateUpdateRowPosition.ToBoolean(
					column.RealColumnEdit.IsLockDefaultImmediateUpdateRowPosition() ? false : OptionsBehavior.ImmediateUpdateRowPosition);
			}
		}
		internal void ResumeImmediateUpdateRowPosition() {
			DataController.ImmediateUpdateRowPosition = OptionsBehavior.ImmediateUpdateRowPosition;
		}
	}
	public class DataMasterRowEventArgs : EventArgs {
		int rowHandle;
		int relationIndex;
		DetailInfo detailInfo;
		public DataMasterRowEventArgs(int rowHandle, int relationIndex, DetailInfo detailInfo) {
			this.rowHandle = rowHandle;
			this.relationIndex = relationIndex;
			this.detailInfo = detailInfo;
		}
		public int RowHandle { get { return rowHandle; } }
		public int RelationIndex { get { return relationIndex; } }
		public DetailInfo DetailInfo  { get { return detailInfo; } }
	}
	public class InitNewRowEventArgs : EventArgs {
		int rowHandle;
		public InitNewRowEventArgs(int rowHandle) {
			this.rowHandle = rowHandle;
		}
		public int RowHandle { get { return rowHandle; } }
	}
	public class GridViewThumbnailImageEventArgs : ThumbnailImageEventArgs {
		string fieldName;
		public GridViewThumbnailImageEventArgs(string fieldName, int dataSourceIndex, AsyncImageLoader loader, ImageLoadInfo info)
			: base(dataSourceIndex, loader, info) {
			this.fieldName = fieldName;
		}
		public string FieldName { get { return fieldName; } }
	}
	public class GetGridViewLoadingImageEventArgs : GetLoadingImageEventArgs {
		string fieldName;
		public GetGridViewLoadingImageEventArgs(string fieldName, int dataSourceIndex) : base(dataSourceIndex) {
			this.fieldName = fieldName;
		}
		public string FieldName { get { return fieldName; } }
	}
	public delegate void DataMasterRowEventHandler(object sender, DataMasterRowEventArgs e);
	public delegate void InitNewRowEventHandler(object sender, InitNewRowEventArgs e);
	public delegate void ShowingPopupEditFormEventHandler(object sender, ShowingPopupEditFormEventArgs e);
	public delegate void GridViewThumbnailImageEventHandler(object sender, GridViewThumbnailImageEventArgs e);
	public delegate void GetGridViewLoadingImageEventHandler(object sender, GetGridViewLoadingImageEventArgs e);
	public class ShowingPopupEditFormEventArgs : EventArgs {
		XtraForm editForm;
		int rowHandle;
		EditFormBindableControlsCollection bindableControls;
		public ShowingPopupEditFormEventArgs(XtraForm editForm, int rowHandle, EditFormBindableControlsCollection bindableControls) {
			this.rowHandle = rowHandle;
			this.editForm = editForm;
			this.bindableControls = bindableControls;
		}
		public int RowHandle { get { return rowHandle; } }
		public XtraForm EditForm { get { return editForm; } }
		public EditFormBindableControlsCollection BindableControls { get { return bindableControls; } }
	}
	public class GridDetailInfo {
		int relationIndex;
		string relationName;
		string caption;
		public GridDetailInfo(int relationIndex, string relationName, string caption) {
			this.relationIndex = relationIndex;
			this.relationName = relationName;
			this.caption = caption;
		}
		public int RelationIndex { get { return relationIndex; } }
		public string RelationName { get { return relationName; } }
		public string Caption { get { return caption; } }
	}
	public class GridRowSelectionInfo {
		public GridRowSelectionInfo(GridColumn column) {
			Add(column);
		}
		public GridRowSelectionInfo() { }
		HybridDictionary cells = null;
		public void AddRange(ICollection columns) {
			foreach(GridColumn column in columns) {
				Add(column, 0);
			}
		}
		public void Add(GridColumn column) { Add(column, 0); }
		internal void Add(GridColumn column, int blockCount) {
			if(column == null) return;
			CreateCells();
			this.cells[column] = blockCount;
		}
		internal void AddReference(GridColumn column) {
			if(column == null) return;
			CreateCells();
			int bk = 0;
			if(this.cells.Contains(column)) {
				bk = 1 + (int)this.cells[column];
			}
			this.cells[column] = bk;
		}
		internal bool ReleaseRow() {
			if(IsEmpty) return false;
			GridColumn[] columns = new GridColumn[cells.Count];
			cells.Keys.CopyTo(columns, 0);
			bool res = false;
			for(int n = 0; n < columns.Length; n++) {
				res |= Release(columns[n]);
			}
			return res;
		}
		internal bool Release(GridColumn column) {
			if(IsEmpty) return false;
			if(this.cells[column] != null) {
				int bc = (int)this.cells[column];
				if(bc == 0) {
					this.cells.Remove(column);
					if(this.cells.Count == 0) this.cells = null;
					return true;
				} else {
					this.cells[column] = bc - 1;
				}
			}
			return false;
		}
		public bool Remove(GridColumn column) {
			if(IsEmpty) return false;
			if(this.cells[column] != null) {
				this.cells.Remove(column);
				if(this.cells.Count == 0) this.cells = null;
				return true;
			}
			return false;
		}
		public bool Contains(GridColumn column) {
			return !IsEmpty && cells.Contains(column);
		}
		void CreateCells() {
			if(this.cells == null) this.cells = new HybridDictionary();
		}
		public bool IsEmpty { get { return cells == null; } }
		public int Count { get { return this.cells == null ? 0 : this.cells.Count; } }
		public GridColumn[] GetColumns() { 
			if(IsEmpty) return null;
			GridColumn[] columns = new GridColumn[cells.Count];
			cells.Keys.CopyTo(columns, 0);
			return columns;
		}
		public override bool Equals(object obj) {
			GridRowSelectionInfo info = obj as GridRowSelectionInfo;
			if(info == null) return false;
			if(Object.ReferenceEquals(this, obj)) return true;
			if(this.Count != info.Count) return false;
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
