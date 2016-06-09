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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.Printing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Printing;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPivotGrid {
	public class PivotFieldFilterChangingEventArgs : PivotFieldEventArgs {
		bool cancel;
		IList<object> values;
		PivotFilterType filterType;
		bool showBlanks;
		public PivotFieldFilterChangingEventArgs(PivotGridField field, PivotFilterType filterType, bool showBlanks, IList<object> values)
			: base(field) {
			this.filterType = filterType;
			this.showBlanks = showBlanks;
			this.values = values;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public PivotFilterType FilterType { get { return filterType; } }
		public IList<object> Values { get { return values; } }
		public bool ShowBlanks { get { return showBlanks; } }
	}
	public class PivotGroupEventArgs : EventArgs {
		PivotGridGroup group;
		public PivotGroupEventArgs(PivotGridGroup group) {
			this.group = group;
		}
		public PivotGridGroup Group { get { return group; } }
	}
	public class PivotCustomFilterPopupItemsEventArgs : EventArgs {
		PivotGridFilterItems items;
		public PivotCustomFilterPopupItemsEventArgs(PivotGridFilterItems items) {
			this.items = items;
		}
		public void CheckAllItems(bool isChecked) {
			items.CheckAllItems(isChecked);
		}
		public PivotGridField Field { get { return items.Field as PivotGridField; } }
		public IList<PivotGridFilterItem> Items { get { return items; } }
		public PivotGridFilterItem ShowBlanksItem { get { return items.BlankItem; } }
	}
	public class FieldValueCell : FieldValueCellBase<PivotGridField> {
		public FieldValueCell(PivotFieldValueItem item)
			: base(item) {
		}
		public new FieldValueCell Parent { get { return ParentItem == null ? null : new FieldValueCell(ParentItem); } }
	}
	public class PivotCustomFieldValueCellsEventArgs : PivotCustomFieldValueCellsEventArgsBase<PivotGridField, FieldValueCell> {
		internal PivotCustomFieldValueCellsEventArgs(PivotVisualItemsBase items) : base(items) { }
		protected override FieldValueCell GetCellCore(PivotFieldValueItem item) {
			if(item == null) return null;
			return new FieldValueCell(item);
		}
	}
	public class PivotFieldEventArgs : PivotFieldEventArgsBase<PivotGridField> {
		public PivotFieldEventArgs(PivotGridField field) : base(field) {
		}
	}
	public class PivotCustomGroupIntervalEventArgs : PivotCustomGroupIntervalEventArgsBase<PivotGridField> {
		public PivotCustomGroupIntervalEventArgs(PivotGridField field, object value)
			: base(field, value) {
		}
	}
	public class PivotFieldValueEventArgs : PivotFieldValueEventArgsBase<PivotGridField> {
		public PivotFieldValueEventArgs(PivotFieldValueItem item)
			: base(item) {
		}
		public PivotFieldValueEventArgs(PivotGridField field)
			: base(field) {
		}
		protected PivotCellsViewInfoBase CellsArea { get { return Data.ViewInfo.CellsArea; } }
		public new PivotGridCustomTotal CustomTotal { get { return Item != null ? (PivotGridCustomTotal)Item.CustomTotal : null; } }
		public new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)Item.Data; } }
	}
	public class PivotFieldDisplayTextEventArgs : PivotFieldValueEventArgs {
		string displayText;
		object value;
		public PivotFieldDisplayTextEventArgs(PivotFieldValueItem item, string defaultText)
			: base(item) {
			this.value = Item.Value;
			this.displayText = defaultText;
		}
		public PivotFieldDisplayTextEventArgs(PivotGridField field, object value, string defaultText)
			: base(field) {
			this.value = value;
			this.displayText = defaultText;
		}
		public override object Value { get { return value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public bool IsPopulatingFilterDropdown { get { return Item == null; } }
	}
	public class PivotFieldPropertyChangedEventArgs : PivotFieldEventArgs {
		PivotFieldPropertyName propertyName;
		public PivotFieldPropertyChangedEventArgs(PivotGridField field, PivotFieldPropertyName propertyName)
			: base(field) {
			this.propertyName = propertyName;
		}
		public PivotFieldPropertyName PropertyName {
			get { return propertyName; }
		}
	}
	public class PivotCustomChartDataSourceDataEventArgs : EventArgs {
		PivotChartItemType itemType;
		PivotChartItemDataMember itemDataMember;
		PivotFieldValueEventArgs fieldValueInfo;
		PivotCellValueEventArgs cellInfo;
		object _value;
		public PivotCustomChartDataSourceDataEventArgs(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			this.itemType = itemType;
			this.itemDataMember = itemDataMember;
			this.fieldValueInfo = null;
			this.cellInfo = null;
			switch(ItemType) {
				case PivotChartItemType.ColumnItem:
				case PivotChartItemType.RowItem:
					this.fieldValueInfo = new PivotFieldValueEventArgs(fieldValueItem);
					this.cellInfo = null;
					break;
				case PivotChartItemType.CellItem:
					this.fieldValueInfo = null;
					this.cellInfo = new PivotCellValueEventArgs(cellItem);
					break;
			}
			this._value = value;
		}
		public PivotChartItemType ItemType { get { return itemType; } }
		public PivotChartItemDataMember ItemDataMember { get { return itemDataMember; } }
		public PivotFieldValueEventArgs FieldValueInfo { get { return fieldValueInfo; } }
		public PivotCellValueEventArgs CellInfo { get { return cellInfo; } }
		public object Value {
			get { return _value; }
			set { _value = value; }
		}
	}
	public class PivotCustomRowHeightEventArgs : PivotFieldValueEventArgs {
		int height;
		public PivotCustomRowHeightEventArgs(PivotFieldValueItem item, int height) : base(item) {
			this.height = height;
		}
		protected PivotFieldValueItem FieldCellViewInfo { get { return Item; } }
		public int RowHeight {
			get { return height; }
			set { height = value; }
		}
		public int RowIndex {
			get { return MinIndex; }
		}
		public int ColumnCount { get { return Data.VisualItems.GetLastLevelItemCount(true); } }
		public int RowCount { get { return Data.VisualItems.GetLastLevelItemCount(false); } }
		public object GetRowCellValue(int columnIndex) {
			return GetCellValue(columnIndex, RowIndex);
		}
	}
	public class PivotCustomColumnWidthEventArgs : PivotFieldValueEventArgs {
		int width;
		public PivotCustomColumnWidthEventArgs(PivotFieldValueItem item, int width) : base(item) {
			this.width = width;
		}
		protected PivotFieldValueItem FieldCellViewInfo { get { return Item; } }
		public int ColumnWidth {
			get { return width; }
			set { width = value; }
		}
		public int ColumnLineCount {
			get {
				int columnLineCount = Item.CellLevelCount;
				if(Item.Data.FieldItems.GetFieldCountByArea(PivotArea.ColumnArea) == 0)
					return columnLineCount;
				for(int i = Item.StartLevel; i <= Item.EndLevel; i++) {
					PivotFieldItem field = (PivotFieldItem)Item.Data.FieldItems.GetFieldItemByLevel(true, i);
					if(field == null)
						continue;
					columnLineCount += field.ColumnValueLineCount - 1;
				}
				return columnLineCount;
			}
		}
		public int ColumnIndex {
			get { return MinIndex; }
		}
		public int ColumnCount { get { return Data.VisualItems.GetLastLevelItemCount(true); } }
		public int RowCount { get { return Data.VisualItems.GetLastLevelItemCount(false); } }
		public object GetColumnCellValue(int rowIndex) {
			return GetCellValue(ColumnIndex, rowIndex);
		}
	}
	public class PivotFieldValueCancelEventArgs : PivotFieldValueEventArgs {
		bool cancel = false;
		public PivotFieldValueCancelEventArgs(PivotFieldValueItem item)
			: base(item) {
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class PivotCellBaseEventArgs : PivotCellEventArgsBase<PivotGridField, PivotGridViewInfoData, PivotGridCustomTotal> {
		public PivotCellBaseEventArgs(PivotGridCellItem cellItem)
			: base(cellItem) {
		}
	}
	public class PivotCellValueEventArgs : PivotCellBaseEventArgs {
		object value;
		public PivotCellValueEventArgs(PivotGridCellItem cellItem)
			: base(cellItem) {
			this.value = Item.Value;
		}
		public new object Value {
			get { return this.value; }
			set { this.value = value; }
		}
	}
	public class PivotQueryExceptionEventArgs : EventArgs {
		Exception ex;
		bool handled;
		internal PivotQueryExceptionEventArgs(Exception ex) {
			this.ex = ex;
			this.handled = false;
		}
		public Exception Exception {
			get { return ex; }
		}
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
	public class PivotOlapExceptionEventArgs : PivotQueryExceptionEventArgs {
		internal PivotOlapExceptionEventArgs(Exception ex)
			: base(ex) {
		}
	}
	public class EditValueChangedEventArgs : PivotCellBaseEventArgs {
		BaseEdit editor;
		public BaseEdit Editor { get { return editor; } }
		public EditValueChangedEventArgs(PivotCellViewInfo cellInfo, BaseEdit editor)
			: base(cellInfo) {
			if(cellInfo == null || editor == null) throw new ArgumentException("cellInfo or editor is null");
			this.editor = editor;
		}
	}
	public class PivotCellEventArgs : PivotCellBaseEventArgs {
		PivotGridViewInfo viewInfo;
		internal Rectangle? bounds = null;
		public PivotCellEventArgs(PivotGridCellItem cellItem, PivotGridViewInfo viewInfo) : this(cellItem, viewInfo, null) { }
		public PivotCellEventArgs(PivotGridCellItem cellItem, PivotGridViewInfo viewInfo, Rectangle? bounds)
			: base(cellItem) {
			this.viewInfo = viewInfo;
			this.bounds = bounds;
		}
		protected PivotGridViewInfo ViewInfo { get { return viewInfo; } }
		public bool Focused { get { return Data.VisualItems.IsCellFocused(Item); } }
		public bool Selected { get { return Data.VisualItems.IsCellSelected(Item); } }
		public string DisplayText { get { return Item.Text; } }
		public Rectangle Bounds {
			get {
				PivotCellViewInfo CellViewInfo = Item as PivotCellViewInfo;
				if(CellViewInfo != null) {
					if(ViewInfo.IsHorzScrollControl)
						return CellViewInfo.Bounds;
					return CellViewInfo.PaintBounds;
				} else {
					if(bounds != null)
						return (Rectangle)bounds;
					return new Rectangle(
						ViewInfo.FieldMeasures.GetHeightDifference(true, 0, Item.ColumnIndex),
						   ViewInfo.FieldMeasures.GetHeightDifference(false, 0, Item.RowIndex),
							  ViewInfo.FieldMeasures.GetHeightDifference(true, Item.ColumnIndex, Item.ColumnIndex + 1),
								 ViewInfo.FieldMeasures.GetHeightDifference(false, Item.RowIndex, Item.RowIndex + 1)
						);
				}
			}
		}
	}
	public class CancelPivotCellEditEventArgs : PivotCustomCellEditEventArgs {
		public CancelPivotCellEditEventArgs(PivotCellViewInfo cellViewInfo, RepositoryItem repositoryItem)
			: base(cellViewInfo, repositoryItem) {
		}
		public new RepositoryItem RepositoryItem {
			get { return base.RepositoryItem; }
		}
		bool cancel;
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public class PivotCellDisplayTextEventArgs : PivotCellBaseEventArgs {
		string displayText;
		public PivotCellDisplayTextEventArgs(PivotGridCellItem cellItem)
			: base(cellItem) {
			this.displayText = Item.Text;
		}
		public string DisplayText { get { return displayText; } set { displayText = value; } }
	}
	public class PivotAreaChangingEventArgs : PivotFieldEventArgs {
		int newAreaIndex;
		PivotArea newArea;
		bool allow;
		public PivotAreaChangingEventArgs(PivotGridField field, PivotArea newArea, int newAreaIndex)
			: base(field) {
			this.newArea = newArea;
			this.newAreaIndex = newAreaIndex;
			this.allow = true;
		}
		public int NewAreaIndex { get { return newAreaIndex; } }
		public PivotArea NewArea { get { return newArea; } }
		public bool Allow { get { return allow; } set { allow = value; } }
	}
	public class CustomFieldDataEventArgs : CustomFieldDataEventArgsBase<PivotGridField> {
		public CustomFieldDataEventArgs(PivotGridData data, PivotGridField field, int listSourceRow, object _value) :
		base(data, field, listSourceRow, _value){
		}
	}
	public class PivotGridCustomFieldSortEventArgs : PivotGridCustomFieldSortEventArgsBase<PivotGridField> {
		public PivotGridCustomFieldSortEventArgs(PivotGridData data, PivotGridField field) : base(data, field) {
		}
	}
	public class CustomServerModeSortEventArgs : CustomServerModeSortEventArgsBase<PivotGridField> {
		public CustomServerModeSortEventArgs(PivotGridField field) : base(field) { }
		internal new void SetArgs(PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			base.SetArgs(value0, value1, helper);
		}
	}
	public enum PivotGridMenuType { Header, HeaderArea, FieldValue, HeaderSummaries, Cell };
	public class PopupMenuShowingEventArgs : PivotGridMenuEventArgsBase {
		public PopupMenuShowingEventArgs(PivotGridViewInfo viewInfo, PivotGridMenuType menuType, DXPopupMenu menu, PivotGridField field, PivotArea area, Point point)
			: base(viewInfo, menuType, menu, field, area, point) { }
		protected new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PopupMenuShowingEventArgsField")]
#endif
		public new PivotGridField Field { get { return (PivotGridField)base.Field; } }
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PopupMenuShowingEventArgsHitInfo")]
#endif
		public PivotGridHitInfo HitInfo { get { return ViewInfo.CalcHitInfo(Point); } }
	}
	[Obsolete("Use the PopupMenuShowingEventArgs class instead")]
	public class PivotGridMenuEventArgs : PopupMenuShowingEventArgs {
		public PivotGridMenuEventArgs(PivotGridViewInfo viewInfo, PivotGridMenuType menuType, DXPopupMenu menu, PivotGridField field, PivotArea area, Point point)
			: base(viewInfo, menuType, menu, field, area, point) {
		}
	}
	public class PivotGridMenuEventArgsBase : EventArgs {
		PivotGridViewInfoBase viewInfo;
		PivotGridMenuType menuType;
		DXPopupMenu menu;
		PivotGridFieldBase field;
		PivotArea area;
		Point point;
		bool allow;
		public PivotGridMenuEventArgsBase(PivotGridViewInfoBase viewInfo, PivotGridMenuType menuType, DXPopupMenu menu, PivotGridFieldBase field, PivotArea area, Point point) {
			this.viewInfo = viewInfo;
			this.menu = menu;
			this.menuType = menuType;
			this.field = field;
			this.area = area;
			this.point = point;
			this.allow = true;
		}
		protected PivotGridViewInfoBase ViewInfo { get { return viewInfo; } }
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridMenuEventArgsBaseField")]
#endif
		public PivotGridFieldBase Field { get { return field; } }
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridMenuEventArgsBaseMenuType")]
#endif
		public PivotGridMenuType MenuType { get { return menuType; } }
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridMenuEventArgsBaseArea")]
#endif
		public PivotArea Area { get { return area; } }
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridMenuEventArgsBaseAllow")]
#endif
		public bool Allow {
			get { return allow; }
			set { allow = value; }
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridMenuEventArgsBaseMenu")]
#endif
		public DXPopupMenu Menu {
			get { return menu; }
			set { menu = value; }
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridMenuEventArgsBasePoint")]
#endif
		public Point Point {
			get { return point; }
			set { point = value; }
		}
	}
	public class PivotFieldTooltipShowingEventArgs : EventArgs {
		Point point;
		bool showTooltip;
		string text;
		PivotGridViewInfo viewInfo;
		public PivotFieldTooltipShowingEventArgs(PivotGridViewInfo viewInfo, Point point, string text)
			: base() {
			this.viewInfo = viewInfo;
			this.point = point;
			this.showTooltip = true;
			this.text = text;
		}
		public Point Point { get { return point; } }
		public PivotGridHitInfo HitInfo { get { return viewInfo.CalcHitInfo(Point); } }
		public bool ShowTooltip { get { return showTooltip; } set { showTooltip = value; } }
		public string Text { get { return text; } set { text = value; } }
	}
	public class PivotGridMenuItemClickEventArgs : PivotGridMenuItemClickEventArgsBase {
		public PivotGridMenuItemClickEventArgs(PivotGridViewInfo viewInfo, PivotGridMenuType menuType, DXPopupMenu menu, PivotGridField field, PivotArea area, Point point, DXMenuItem item)
			: base(viewInfo, menuType, menu, field, area, point, item) { }
		protected new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		public new PivotGridField Field { get { return (PivotGridField)base.Field; } }
		public PivotGridHitInfo HitInfo { get { return ViewInfo.CalcHitInfo(Point); } }
	}
	public class PivotGridMenuItemClickEventArgsBase : PivotGridMenuEventArgsBase {
		DXMenuItem item;
		public PivotGridMenuItemClickEventArgsBase(PivotGridViewInfoBase viewInfo, PivotGridMenuType menuType, DXPopupMenu menu, PivotGridFieldBase field, PivotArea area, Point point, DXMenuItem item)
			: base(viewInfo, menuType, menu, field, area, point) {
			this.item = item;
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridMenuItemClickEventArgsBaseItem")]
#endif
		public DXMenuItem Item { get { return item; } }
	}
	public class PivotGridCustomSummaryEventArgs : PivotGridCustomSummaryEventArgsBase<PivotGridField> {
		public PivotGridCustomSummaryEventArgs(PivotGridData data, PivotGridField field, PivotCustomSummaryInfo customSummaryInfo)
			: base(data, field, customSummaryInfo) {
		}
	}
	public class CustomizationFormShowingEventArgs : EventArgs {
		Form customizationForm;
		bool cancel;
		Control parentControl;
		public CustomizationFormShowingEventArgs(Form customizationForm, Control parentControl) {
			this.customizationForm = customizationForm;
			this.parentControl = parentControl;
			this.cancel = false;
		}
		public Form CustomizationForm { get { return customizationForm; } }
		public Control ParentControl { get { return parentControl; } set { parentControl = value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Please use Cancel property")]
		public bool Handled { get { return Cancel; } set { Cancel = value; } }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class CustomEditValueEventArgs : PivotCellBaseEventArgs {
		object value;
		public new object Value { get { return this.value; } set { this.value = value; } }
		public CustomEditValueEventArgs(object value, PivotGridCellItem cellItem)
			: base(cellItem) {
			this.value = value;
		}
	}
	public class PivotCellEditEventArgs : PivotCellBaseEventArgs {
		BaseEdit edit;
		public PivotCellEditEventArgs(PivotCellViewInfo cellViewInfo, BaseEdit edit)
			: base(cellViewInfo) {
			this.edit = edit;
		}
		public BaseEdit Edit { get { return edit; } }
	}
	public class PivotCustomCellEditEventArgs : PivotCellBaseEventArgs {
		RepositoryItem repositoryItem;
		public RepositoryItem RepositoryItem {
			get { return repositoryItem; }
			set {
				if(repositoryItem != value)
					repositoryItem = value;
			}
		}
		public PivotCustomCellEditEventArgs(PivotGridCellItem cellItem, RepositoryItem repositoryItem)
			: base(cellItem) {
			this.repositoryItem = repositoryItem;
		}
	}
	public class CustomExportHeaderEventArgs : CustomExportHeaderEventArgsBase<PivotGridField> {
		ExportAppearanceObject appearance;
		public CustomExportHeaderEventArgs(IVisualBrick brick, PivotFieldItemBase fieldItem, ExportAppearanceObject appearance, PivotGridFieldBase field, ref Rectangle rect) : base(brick, fieldItem, field, ref rect) {
			this.appearance = appearance;
		}
		public ExportAppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null) return;
				appearance = value;
			}
		}
	}
	public class CustomExportFieldValueEventArgs : CustomExportFieldValueEventArgsBase<PivotGridField> {
		ExportAppearanceObject appearance;
		public CustomExportFieldValueEventArgs(IVisualBrick brick, PivotFieldValueItem fieldValueItem, ExportAppearanceObject appearance, ref Rectangle rect)
			: base(brick, fieldValueItem, ref rect) {
			this.appearance = appearance;
		}
		public ExportAppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null)
					return;
				appearance = value;
			}
		}
		public new PivotGridCustomTotal CustomTotal { get { return (PivotGridCustomTotal)Item.CustomTotal; } }
	}
	public class CustomExportCellEventArgs : CustomExportCellEventArgsBase {
		PivotGridCellItem cellItem;
		ExportAppearanceObject appearance;
		PivotGridViewInfo viewInfo;
		PivotGridViewInfoData data;
		GraphicsUnit graphicsUnit;
		public CustomExportCellEventArgs(IVisualBrick brick, PivotGridCellItem cellItem,
				ExportAppearanceObject appearance, PivotGridViewInfo viewInfo, PivotGridViewInfoData data, PivotGridPrinterBase printer, GraphicsUnit graphicsUnit, ref Rectangle rect)
			: base(brick, cellItem, graphicsUnit, appearance, printer, ref rect) {
			this.cellItem = cellItem;
			this.appearance = appearance;
			this.viewInfo = viewInfo;
			this.data = data;
			this.graphicsUnit = graphicsUnit;
		}
		protected PivotGridViewInfo ViewInfo { get { return viewInfo; } }
		protected PivotGridViewInfoData Data { get { return data; } }
		public ExportAppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null) return;
				appearance = value;
			}
		}
		public PivotGridField ColumnField { get { return (PivotGridField)ViewInfo.Data.GetField(CellItem.ColumnField); } }
		public PivotGridField RowField { get { return (PivotGridField)ViewInfo.Data.GetField(CellItem.RowField); } }
		public PivotGridField DataField { get { return (PivotGridField)ViewInfo.Data.GetField(CellItem.DataField); } }
		public bool Selected { get { return Data.VisualItems.IsCellSelected(CellItem); } }
		public bool Focused { get { return Data.VisualItems.IsCellFocused(CellItem); } }
	}
	public class PivotLeftTopCellChangedEventArgs : EventArgs {
		Point oldValue, newValue;
		internal PivotLeftTopCellChangedEventArgs(Point oldValue, Point newValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public Point OldValue { get { return oldValue; } }
		public Point NewValue { get { return newValue; } }
	}
	public class PivotCustomChartDataSourceRowsEventArgs : EventArgs {
		readonly PivotChartDataSource ds;
		readonly IList<PivotChartDataSourceRow> rows;
		internal PivotCustomChartDataSourceRowsEventArgs(PivotChartDataSource ds, IList<PivotChartDataSourceRowBase> rows) {
			this.ds = ds;
			this.rows = new PivotChartDataSourceRowBaseListWrapper<PivotChartDataSourceRow>(rows);
		}
		public IList<PivotChartDataSourceRow> Rows {
			get { return rows; }
		}
		public PivotChartDataSourceRow CreateRow(object series, object argument, object value) {
			return new PivotChartDataSourceRow(ds) {
				Series = series,
				Argument = argument,
				Value = value
			};
		}
	}
   public enum UserAction { 
		None = 0,
		Prefilter = 1,  
		FieldFilter= 2, 
		FieldResize = 3,
		FieldDrag = 4,
		FieldUnboundExpression = 5,
		MenuOpen = 6,
	}
	public class PivotUserActionEventArgs : EventArgs {
		readonly UserAction userAction;
		public PivotUserActionEventArgs(UserAction userAction) {
			this.userAction = userAction;
		}
		public UserAction UserAction { get { return userAction; } }
	}
	public delegate void CustomFieldDataEventHandler(object sender, CustomFieldDataEventArgs e);
	public delegate void PivotGridCustomFieldSortEventHandler(object sender, PivotGridCustomFieldSortEventArgs e);
	public delegate void PivotGridCustomSummaryEventHandler(object sender, PivotGridCustomSummaryEventArgs e);
	public delegate void PivotGroupEventHandler(object sender, PivotGroupEventArgs e);
	public delegate void PivotFieldEventHandler(object sender, PivotFieldEventArgs e);
	public delegate void PivotFieldFilterChangingEventHandler(object sender, PivotFieldFilterChangingEventArgs e);
	public delegate void PivotFieldValueEventHandler(object sender, PivotFieldValueEventArgs e);
	public delegate void PivotFieldValueCancelEventHandler(object sender, PivotFieldValueCancelEventArgs e);
	public delegate void PivotFieldDisplayTextEventHandler(object sender, PivotFieldDisplayTextEventArgs e);
	public delegate void PivotCustomGroupIntervalEventHandler(object sender, PivotCustomGroupIntervalEventArgs e);
	public delegate void PivotCustomChartDataSourceDataEventHandler(object sender, PivotCustomChartDataSourceDataEventArgs e);
	public delegate void PivotCustomChartDataSourceRowsEventHandler(object sender, PivotCustomChartDataSourceRowsEventArgs e);
	public delegate void PivotFieldImageIndexEventHandler(object sender, PivotFieldImageIndexEventArgs e);
	public delegate void PivotCellEventHandler(object sender, PivotCellEventArgs e);
	public delegate void PivotCellDisplayTextEventHandler(object sender, PivotCellDisplayTextEventArgs e);
	public delegate void PivotCustomDrawCellEventHandler(object sender, PivotCustomDrawCellEventArgs e);
	public delegate void PivotCustomAppearanceEventHandler(object sender, PivotCustomAppearanceEventArgs e);
	public delegate void PivotCustomDrawEventHandler(object sender, PivotCustomDrawEventArgs e);
	public delegate void PivotCustomDrawFieldHeaderEventHandler(object sender, PivotCustomDrawFieldHeaderEventArgs e);
	public delegate void PivotCustomDrawHeaderAreaEventHandler(object sender, PivotCustomDrawHeaderAreaEventArgs e);
	public delegate void PivotCustomDrawFieldValueEventHandler(object sender, PivotCustomDrawFieldValueEventArgs e);
	public delegate void PivotCustomFilterPopupItemsEventHandler(object sender, PivotCustomFilterPopupItemsEventArgs e);
	public delegate void PivotCustomFieldValueCellsEventHandler(object sender, PivotCustomFieldValueCellsEventArgs e);
	public delegate void PivotAreaChangingEventHandler(object sender, PivotAreaChangingEventArgs e);
#pragma warning disable 618 // Obsolete
#pragma warning disable 612 // Obsolete
	public delegate void PivotGridMenuEventHandler(object sender, PivotGridMenuEventArgs e);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	public delegate void PivotGridMenuItemClickEventHandler(object sender, PivotGridMenuItemClickEventArgs e);
	public delegate void PivotFieldTooltipShowingEventHandler(object sender, PivotFieldTooltipShowingEventArgs e);
	public delegate void CustomizationFormShowingEventHandler(object sender, CustomizationFormShowingEventArgs e);
	public delegate void EditValueChangedEventHandler(object sender, EditValueChangedEventArgs e);
	public delegate void CustomEditValueEventHandler(object sender, CustomEditValueEventArgs e);
	public delegate void PivotFieldPropertyChangedEventHandler(object sender, PivotFieldPropertyChangedEventArgs e);
	public delegate void PivotOlapExceptionEventHandler(object sender, PivotOlapExceptionEventArgs e);
	public delegate void PivotQueryExceptionEventHandler(object sender, PivotQueryExceptionEventArgs e);
	public delegate void PivotUserActionEventHandler(object sender, PivotUserActionEventArgs e);
}
