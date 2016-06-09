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
using System.Data;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;	
using DevExpress.Utils.Drawing;	
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Dragging;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraEditors.Design;
namespace DevExpress.XtraGrid.Views.Base {
	public class CustomRowCellEventArgs : EventArgs {
		internal GridColumn column;
		internal int rowHandle;
		public CustomRowCellEventArgs(int rowHandle, GridColumn column) {
			this.column = column; 
			this.rowHandle = rowHandle;
		}
		public GridColumn Column { get { return column; } }
		public int RowHandle { get { return rowHandle; } }
		public object CellValue { get { return ColumnView.GetRowCellValueCore(rowHandle, column); } }
	}
	public class RowCellAlignmentEventArgs : CustomRowCellEventArgs {
		HorzAlignment horzAlignment;
		public RowCellAlignmentEventArgs(int rowHandle, GridColumn column, HorzAlignment horzAlignment) : base(rowHandle, column) {
			this.horzAlignment = horzAlignment;
		}
		public HorzAlignment HorzAlignment {
			get { return horzAlignment; }
			set { horzAlignment = value; }
		}
	}
	public class RowEventArgs : EventArgs {
		int rowHandle;
		public RowEventArgs(int rowHandle) {
			this.rowHandle = rowHandle;
		}
		public int RowHandle { get { return rowHandle; } }
	}
	public class RowObjectEventArgs : RowEventArgs {
		object row;
		public RowObjectEventArgs(int rowHandle, object row) : base(rowHandle) {
			this.row = row;
		}
		public object Row { get { return row; } }
	}
	public class RowAllowEventArgs : RowEventArgs {
		bool allow;
		public RowAllowEventArgs(int rowHandle, bool allow) : base(rowHandle) {
			this.allow = allow;
		}
		public bool Allow { 
			get { return allow; } 
			set { allow = value; }
		}
	}
	public class ValidateRowEventArgs : RowObjectEventArgs {
		bool valid;
		string errorText;
		public ValidateRowEventArgs(int rowHandle, object row)
			: base(rowHandle, row) {
			this.valid = true;
			this.errorText = "";
		}
		public bool Valid {
			get { return valid; }
			set { valid = value; }
		}
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
		protected internal void TryValidateViaAnnotationAttributes(ColumnView view, GridColumn column) {
			var annotationAttributes = column.ColumnAnnotationAttributes;
			if(Valid && annotationAttributes != null) {
				object value = view.GetRowCellValue(RowHandle, column);
				string errorMessage;
				if(!annotationAttributes.TryValidateValue(value, out errorMessage)) {
					Valid = false;
					ErrorText = errorMessage;
					return;
				}
				if(object.ReferenceEquals(null, Row))
					return;
				if(!annotationAttributes.TryValidateValue(value, Row, out errorMessage)) {
					Valid = false;
					ErrorText = errorMessage;
				}
			}
		}
	}
	public delegate void ValidateRowEventHandler(object sender, ValidateRowEventArgs e);
	public delegate void RowAllowEventHandler(object sender, RowAllowEventArgs e);
	public delegate void RowEventHandler(object sender, RowEventArgs e);
	public delegate void RowObjectEventHandler(object sender, RowObjectEventArgs e);
	public delegate void RowCellAlignmentEventHandler(object sender, RowCellAlignmentEventArgs e);
	public delegate void InvalidRowExceptionEventHandler(object sender, InvalidRowExceptionEventArgs e);
	public class InvalidRowExceptionEventArgs : ExceptionEventArgs {
		int rowHandle;
		object row;
		public InvalidRowExceptionEventArgs(Exception except, string errorText, int rowHandle, object row) : base(errorText, except) {
			this.rowHandle = rowHandle;
			this.row = row;
		}
		public object Row { get { return row; } }
		public int RowHandle { get { return rowHandle; } }
	}
	public class ColumnSortGroupChangeEventArgs : EventArgs {
		int prevIndex;
		int newIndex;
		GridColumn column;
		public ColumnSortGroupChangeEventArgs(GridColumn column, int prevIndex, int newIndex) {
			this.column = column;
			this.prevIndex = prevIndex;
			this.newIndex = newIndex;
		}
		public int PrevIndex { get { return prevIndex; } }
		public int NewIndex { get { return newIndex; } }
		public GridColumn Column { get { return column; } }
	}
	public class CustomDrawObjectEventArgs : CustomDrawEventArgs {
		ObjectPainter painter;
		ObjectInfoArgs info;
		public CustomDrawObjectEventArgs(GraphicsCache cache, ObjectPainter painter, ObjectInfoArgs info, AppearanceObject appearance) : base(cache,  info.Bounds, appearance) {
			this.info = info;
			this.painter = painter;
		}
		public ObjectPainter Painter { get { return painter; } }
		public ObjectInfoArgs Info { get { return info; } }
	}
	public class RowPreviewCustomDrawEventArgs : RowObjectCustomDrawEventArgs {
		GridDataRowInfo rowInfo;
		public RowPreviewCustomDrawEventArgs(GraphicsCache cache, ObjectPainter painter, GridDataRowInfo rowInfo) : base(cache, rowInfo.RowHandle, painter, rowInfo, rowInfo.AppearancePreview) {
			this.rowInfo = rowInfo;
		}
		public override Rectangle Bounds { get { return rowInfo.PreviewBounds; } }
	}
	public class RowObjectCustomDrawEventArgs : CustomDrawObjectEventArgs {
		int rowHandle;
		public RowObjectCustomDrawEventArgs(GraphicsCache cache, int rowHandle, ObjectPainter painter, ObjectInfoArgs info, AppearanceObject appearance) : base(cache, painter, info, appearance) {
			this.rowHandle = rowHandle;
		}
		public int RowHandle { get { return rowHandle; } }
	}
	public class RowGroupRowCellEventArgs {
		GraphicsCache cache;
		Rectangle bounds, captionBounds;
		GridGroupRowInfo groupRow;
		AppearanceObject appearance;
		string displayText;
		object value;
		GridGroupSummaryItem summaryItem;
		bool handled;
		public RowGroupRowCellEventArgs(GraphicsCache cache, Rectangle bounds, Rectangle captionBounds, GridGroupRowInfo groupRow, AppearanceObject appearance, string displayText,
				object value, GridGroupSummaryItem summaryItem) {
			this.cache = cache;
			this.bounds = bounds;
			this.captionBounds = captionBounds;
			this.groupRow = groupRow;
			this.appearance = appearance;
			this.displayText = displayText;
			this.value = value;
			this.summaryItem = summaryItem;
		}
		public GraphicsCache Cache { get { return cache; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle CaptionBounds { get { return captionBounds; } }
		public GridGroupRowInfo GroupRow { get { return groupRow; } }
		public int RowHandle { get { return GroupRow.RowHandle; } }
		public AppearanceObject Appearance { get { return appearance; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public object Value { get { return value; } }
		public GridGroupSummaryItem SummaryItem { get { return summaryItem; } }
		public bool Handled {  get { return handled; } set { handled = value; } }
	}
	public class RowCellObjectCustomDrawEventArgs : RowObjectCustomDrawEventArgs {
		GridColumn column;
		public RowCellObjectCustomDrawEventArgs(GraphicsCache cache, int rowHandle, GridColumn column, ObjectPainter painter, ObjectInfoArgs info, AppearanceObject appearance) : base(cache, rowHandle, painter, info, appearance) {
			this.column = column;
		}
		public GridColumn Column { get { return column; } }
	}
	public class RowCellCustomDrawEventArgs : CustomDrawEventArgs {
		internal GridColumn column;
		internal int rowHandle;
		internal object cellValue;
		internal string displayText;
		internal bool changed;
		object cell;
		internal RowCellCustomDrawEventArgs(GraphicsCache cache, CardFieldInfo fieldInfo) : base(cache, fieldInfo.EditViewInfo.Bounds, fieldInfo.PaintAppearanceValue) {
			Setup(fieldInfo);
		}
		internal RowCellCustomDrawEventArgs(GraphicsCache cache, GridCellInfo cellInfo) : base(cache, cellInfo.CellValueRect, cellInfo.Appearance) {
			Setup(cellInfo);
		}
		public RowCellCustomDrawEventArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, int rowHandle, GridColumn column, object cellValue, string displayText) : base(cache, bounds, appearance) {
			this.rowHandle = rowHandle;
			this.column = column;
			this.cellValue = cellValue;
			this.displayText = displayText;
		}
		public int RowHandle { get { return rowHandle; } }
		public GridColumn Column { get { return column; } }
		public object CellValue {
			get { return cellValue; }
			set {
				if(cellValue == value) return;
				if(cellValue != null && cellValue.Equals(value)) return;
				cellValue = value;
				changed = true;
			}
		}
		public string DisplayText {
			get { return displayText; }
			set {
				if(DisplayText == value) return;
				displayText = value; 
				changed = true;
			}
		}
		public object Cell { get { return cell; } }
		internal void Setup(CardFieldInfo fieldInfo) {
			this.cell = fieldInfo;
			this.cellValue = fieldInfo.EditViewInfo.EditValue;
			this.displayText = fieldInfo.EditViewInfo.DisplayText;
			this.rowHandle = fieldInfo.Card.RowHandle;
			this.column = fieldInfo.Column;
			SetupBase(fieldInfo.PaintAppearanceValue, fieldInfo.EditViewInfo.Bounds);
			this.changed = false;
		}
		internal void Setup(GridCellInfo cellInfo) {
			this.cell = cellInfo;
			this.cellValue = cellInfo.CellValue;
			this.rowHandle = cellInfo.RowHandle;
			this.column = cellInfo.Column;
			this.displayText = cellInfo.Editor == null ? string.Empty : cellInfo.ViewInfo.DisplayText;
			SetupBase(cellInfo.Appearance, cellInfo.CellValueRect);
			this.changed = false;
		}
	}
	public delegate void CustomDrawObjectEventHandler(object sender, CustomDrawObjectEventArgs e);
	public delegate void RowCellCustomDrawEventHandler(object sender, RowCellCustomDrawEventArgs e);
	public delegate void RowObjectCustomDrawEventHandler(object sender, RowObjectCustomDrawEventArgs e);
	public delegate void RowGroupRowCellEventHandler(object sender, RowGroupRowCellEventArgs e);
	public class CellValueChangedEventArgs : EventArgs {
		private int rowHandle;
		private GridColumn column;
		private object value;
		public CellValueChangedEventArgs(int rowHandle, GridColumn column, object value) {
			this.rowHandle = rowHandle; 
			this.column = column;
			this.value = value;
		}
		public object Value { get { return value; } 
		}
		public int RowHandle { get { return rowHandle; } 
		}
		public GridColumn Column { get { return column; } 
		}
	}
	public class FocusedRowChangedEventArgs : EventArgs {
		private int prevFocusedRowHandle, focusedRowHandle;
		public FocusedRowChangedEventArgs(int prevFocusedRowHandle, int focusedRowHandle) {
			this.prevFocusedRowHandle = prevFocusedRowHandle;
			this.focusedRowHandle = focusedRowHandle;
		}
		public int FocusedRowHandle { get { return focusedRowHandle; } 
		}
		public int PrevFocusedRowHandle { get { return prevFocusedRowHandle; } 
		}
	}
	public class FocusedRowObjectChangedEventArgs : RowObjectEventArgs {
		int focusedRowHandle;
		internal int lockCounter = 0;
		public FocusedRowObjectChangedEventArgs(int focusedRowHandle, object row) : base(focusedRowHandle, row) {
			this.focusedRowHandle = focusedRowHandle;
		}
		public int FocusedRowHandle {
			get { return focusedRowHandle; }
		}
	}
	public class FocusedColumnChangedEventArgs : EventArgs {
		private GridColumn prevFocusedColumn, focusedColumn;
		public FocusedColumnChangedEventArgs(GridColumn prevFocusedColumn, GridColumn focusedColumn) {
			this.prevFocusedColumn = prevFocusedColumn;
			this.focusedColumn = focusedColumn;
		}
		public GridColumn FocusedColumn { get { return focusedColumn; } 
		}
		public GridColumn PrevFocusedColumn { get { return prevFocusedColumn; } 
		}
	}
	public class FilterControlEventArgs : EventArgs {
		private IFilterControl ifControl;
		private FilterBuilder form;
		bool show = true;
		public FilterControlEventArgs(FilterBuilder form, IFilterControl ifControl) {
			this.ifControl = ifControl;
			this.form = form;
		}
		public FilterControl FilterControl { get { return ifControl as FilterControl; } }
		public IFilterControl IFilterControl { get { return ifControl; } }
		public FilterBuilder FilterBuilder { get { return form; } }
		public bool ShowFilterEditor { get { return show; } set { show = value; } 
		}
	}
	public class UnboundExpressionEditorEventArgs : EventArgs {
		private ExpressionEditorForm form;
		private GridColumn column;
		bool show = true;
		public UnboundExpressionEditorEventArgs(ExpressionEditorForm form, GridColumn column) {
			this.form = form;
			this.column = column;
		}
		public ExpressionEditorForm ExpressionEditorForm {
			get { return form; }
		}
		public GridColumn Column {
			get { return column; }
		}
		public bool ShowExpressionEditor {
			get { return show; }
			set { show = value; }
		}
	}
	public delegate void CellValueChangedEventHandler(object sender, CellValueChangedEventArgs e);
	public delegate void FocusedRowChangedEventHandler(object sender, FocusedRowChangedEventArgs e);
	public delegate void FocusedRowObjectChangedEventHandler(object sender, FocusedRowObjectChangedEventArgs e);
	public delegate void FocusedColumnChangedEventHandler(object sender, FocusedColumnChangedEventArgs e);
	public delegate void FilterControlEventHandler(object sender, FilterControlEventArgs e);
	public delegate void UnboundExpressionEditorEventHandler(object sender, UnboundExpressionEditorEventArgs e);
	public class DragObjectStartEventArgs : EventArgs {
		object dragObject;
		bool allow;
		public DragObjectStartEventArgs(object dragObject) {
			this.dragObject = dragObject;
			this.allow = true;
		}
		public object DragObject { get { return dragObject; } }
		public bool Allow {
			get { return allow; }
			set { allow = value; }
		}
	}
	public class DragObjectOverEventArgs : EventArgs {
		object dragObject;
		PositionInfo dropInfo;
		public DragObjectOverEventArgs(object dragObject, PositionInfo dropInfo) {
			this.dragObject = dragObject;
			this.dropInfo = dropInfo;
		}
		public object DragObject { get { return dragObject; } }
		public PositionInfo DropInfo { get { return dropInfo; } }
	}
	public class DragObjectDropEventArgs : EventArgs {
		object dragObject;
		PositionInfo dropInfo;
		public DragObjectDropEventArgs(object dragObject, PositionInfo dropInfo) {
			this.dragObject = dragObject;
			this.dropInfo = dropInfo;
		}
		public object DragObject { get { return dragObject; } }
		public PositionInfo DropInfo { get { return dropInfo; } }
		public bool Canceled { get { return !DropInfo.Valid; } }
	}
	public class ColumnEventArgs : EventArgs {
		GridColumn column;
		public ColumnEventArgs(GridColumn column) {
			this.column = column;
		}
		public GridColumn Column { get { return column; } }
	}
	public class CustomColumnDisplayTextEventArgs : EventArgs {
		GridColumn column;
		int rowHandle, listSourceRow;
		string displayText;
		object _value;
		bool forGroupRow = false;
		public CustomColumnDisplayTextEventArgs(int rowHandle, GridColumn column, object _value) : this(rowHandle, column, _value, false) { }
		public CustomColumnDisplayTextEventArgs(int rowHandle, GridColumn column, object _value, bool forGroupRow) {
			SetArgs(DataController.InvalidRow, rowHandle, column, _value, string.Empty, forGroupRow);
		}
		public object Value { get { return _value; } }
		public GridColumn Column { get { return column; } }
		internal int RowHandle { get { return rowHandle; } }
		public int ListSourceRowIndex {
			get {
				if(listSourceRow == GridControl.InvalidRowHandle) {
					listSourceRow = (RowHandle == GridControl.InvalidRowHandle || column.View == null) ? GridControl.InvalidRowHandle : column.View.DataController.GetListSourceRowIndex(RowHandle);
				}
				return listSourceRow;
			}
		}
		public int GroupRowHandle { get { return forGroupRow ? RowHandle : GridControl.InvalidRowHandle; } }
		public bool IsForGroupRow { get { return forGroupRow; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		internal void SetArgs(int listSourceIndex, int rowHandle, GridColumn column, object _value, string displayText, bool forGroupRow) {
			this.column = column;
			this.rowHandle = rowHandle;
			this._value = _value;
			this.displayText = displayText;
			this.listSourceRow = listSourceIndex;
			this.forGroupRow = forGroupRow;
		}
	}
	public class RowFilterEventArgs : EventArgs {
		int listSourceRow;
		bool handled, visible;
		internal void GoTo(int listSourceRow, bool fit) {
			this.listSourceRow = listSourceRow;
			this.handled = false;
			this.visible = fit;
		}
		public RowFilterEventArgs(int listSourceRow) {
			GoTo(listSourceRow, true);
		}
		public bool Handled { get { return handled; } set { handled = value; } }
		public bool Visible { get { return visible; } set { visible = value; } }
		public int ListSourceRow { get { return listSourceRow; } }
	}
	public class CustomColumnDataEventArgs : EventArgs {
		GridColumn column;
		object _value = null;
		int listSourceRow;
		bool isGetAction = true;
		object row = null;
		public CustomColumnDataEventArgs(GridColumn column, int listSourceRow, object _value, bool isGetAction) {
			this.isGetAction = isGetAction;
			this.column = column;
			this.listSourceRow = listSourceRow;
			this._value = _value;
		}
		public GridColumn Column { get { return column; } }
		public int ListSourceRowIndex { get { return listSourceRow; } }
		public object Row {
			get {
				if(row != null) return row;
				if(column.View != null) {
					row = column.View.DataController.GetRowByListSourceIndex(listSourceRow);
				}
				return row;
			}
		}
		public bool IsGetData { get { return isGetAction; } }
		public bool IsSetData { get { return !IsGetData; } }
		public object Value { get { return _value; } set { _value = value; } }
		internal void SetArgs(GridColumn column, int listSourceRow, object _value, bool isGetAction) {
			this.column = column;
			this.listSourceRow = listSourceRow;
			this._value = _value;
			this.isGetAction = isGetAction;
			this.row = null;
		}
	}
	public class CustomColumnSortEventArgs : EventArgs {
		GridColumn column;
		ColumnSortOrder sortOrder;
		bool handled = false;
		Hashtable rowIndexes = null;
		internal object value1, value2;
		int  result = 0;
		int listSourceRow1, listSourceRow2;
		public CustomColumnSortEventArgs(GridColumn column, object value1, object value2, ColumnSortOrder sortOrder) {
			this.sortOrder = sortOrder;
			this.listSourceRow1 = this.listSourceRow2 = GridControl.InvalidRowHandle; 
			this.column = column;
			this.value1 = value1;
			this.value2 = value2;
		}
		public ColumnSortOrder SortOrder { get { return sortOrder; } }
		public GridColumn Column { get { return column; } set { column = value; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public object Value1 { get { return value1; } }
		public object Value2 { get { return value2; } }
		public int  Result {
			get { return result; }
			set { result = value; }
		}
		internal int? GetSortResult() { 
			if(!Handled)
				return null;
			return Result;
		}
		internal object GetRow(int listIndex) { return column == null || column.View == null ? null : column.View.GetRowByDataSourceIndex(listIndex); }
		public object RowObject1 { get { return GetRow(listSourceRow1); } }
		public object RowObject2 { get { return GetRow(listSourceRow2); } }
		public int ListSourceRowIndex1 { get { return listSourceRow1; } }
		public int ListSourceRowIndex2 { get { return listSourceRow2; } }
		int CalcRow(int listSourceRow) {
			if(this.rowIndexes == null) this.rowIndexes = new Hashtable();
			object res = this.rowIndexes[listSourceRow];
			if(res != null) return (int)res;
			int val = (listSourceRow == GridControl.InvalidRowHandle || column.View == null) ? GridControl.InvalidRowHandle : column.View.DataController.GetControllerRow(listSourceRow);
			this.rowIndexes[listSourceRow] = val;
			return val;
		}
		internal void SetArgs(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder) {
			this.sortOrder = sortOrder;
			this.value1 = value1;
			this.value2 = value2;
			this.result = 0;
			this.handled = false;
			this.listSourceRow1 = listSourceRow1;
			this.listSourceRow2 = listSourceRow2;
		}
	}
	public delegate void CustomColumnDisplayTextEventHandler(object sender, CustomColumnDisplayTextEventArgs e);
	public delegate void CustomColumnSortEventHandler(object sender, CustomColumnSortEventArgs e);
	public delegate void CustomColumnDataEventHandler(object sender, CustomColumnDataEventArgs e);
	public delegate void ColumnEventHandler(object sender, ColumnEventArgs e);
	public delegate void RowFilterEventHandler(object sender, RowFilterEventArgs e);
	public delegate void DragObjectDropEventHandler(object sender, DragObjectDropEventArgs e);
	public delegate void DragObjectOverEventHandler(object sender, DragObjectOverEventArgs e);
	public delegate void DragObjectStartEventHandler(object sender, DragObjectStartEventArgs e);
	public delegate void BeforePrintRowEventHandler(object sender, CancelPrintRowEventArgs e);
	public delegate void AfterPrintRowEventHandler(object sender, PrintRowEventArgs e);
}
