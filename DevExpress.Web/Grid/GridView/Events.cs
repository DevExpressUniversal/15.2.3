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

using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Web.Data;
using DevExpress.Web.Internal;
using DevExpress.Web.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	public enum GridViewRowType { Data, Group, Preview, Detail, InlineEdit, EditForm, EditingErrorRow, Footer, GroupFooter, Filter, EmptyDataRow, PagerEmptyRow, Title, Header, BatchEditNewDataRow }
	public enum GridViewTableCommandCellType { Data, Filter }
	public enum GridViewDetailRowButtonState { Visible, Hidden }
	public enum GridErrorTextKind { General, RowValidate };
	public enum GridViewAutoFilterEventKind { CreateCriteria, ExtractDisplayText };
	public class ASPxGridViewItemEventArgs : ASPxGridItemEventArgs {
		public ASPxGridViewItemEventArgs(int visibleIndex, object keyValue)
			: base(visibleIndex, keyValue) {
		}
		public new object KeyValue { get { return base.KeyValue; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
	}
	public class ASPxGridViewColumnDisplayTextEventArgs : ASPxGridColumnDisplayTextEventArgs {
		public ASPxGridViewColumnDisplayTextEventArgs(GridViewDataColumn column, int visibleIndex, object _value)
			: base(column, visibleIndex, _value) {
		}
		internal ASPxGridViewColumnDisplayTextEventArgs(GridViewDataColumn column, int visibleIndex, object _value, IValueProvider rowValueProvider)
			: base(column, visibleIndex, _value, rowValueProvider) {
		}
		public new GridViewDataColumn Column { get { return (GridViewDataColumn)base.Column; } }
		public int VisibleRowIndex { get { return base.VisibleIndex; } }
		public new string DisplayText { get { return base.DisplayText; } set { base.DisplayText = value; } }
		public new object Value { get { return base.Value; } set { base.Value = value; } }
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new object GetFieldValue(string fieldName) {
			return base.GetFieldValue(fieldName);
		}
		public new object GetFieldValue(int visibleIndex, string fieldName) {
			return base.GetFieldValue(visibleIndex, fieldName);
		}
	}
	public class ASPxGridViewColumnDataEventArgs : ASPxGridColumnDataEventArgs {
		public ASPxGridViewColumnDataEventArgs(GridViewDataColumn column, int listSourceRow, object value, bool isGetAction)
			: base(column, listSourceRow, value, isGetAction) {
		}
		public new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
		public new int ListSourceRowIndex { get { return base.ListSourceRowIndex; } }
		public new bool IsGetData { get { return base.IsGetData; } }
		public new bool IsSetData { get { return base.IsSetData; } }
		public new object Value { get { return base.Value; } set { base.Value = value; } }
		public new object GetListSourceFieldValue(string fieldName) {
			return base.GetListSourceFieldValue(fieldName);
		}
		public new object GetListSourceFieldValue(int listSourceRowIndex, string fieldName) {
			return base.GetListSourceFieldValue(listSourceRowIndex, fieldName);
		}
	}
	public class ASPxGridViewHeaderFilterEventArgs : ASPxGridHeaderFilterEventArgs {
		public ASPxGridViewHeaderFilterEventArgs(GridViewDataColumn column, GridHeaderFilterValues values)
			: base(column, values) {
		}
		public new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
		public new GridHeaderFilterValues Values { get { return base.Values; } }
		public new void AddValue(string displayText, string value) {
			base.AddValue(displayText, value);
		}
		public new void AddValue(string displayText, string value, string query) {
			base.AddValue(displayText, value, query);
		}
		public new void AddShowAll() {
			base.AddShowAll();
		}
	}
	public class ASPxGridViewBeforeHeaderFilterFillItemsEventArgs : ASPxGridBeforeHeaderFilterFillItemsEventArgs {
		public ASPxGridViewBeforeHeaderFilterFillItemsEventArgs(GridViewDataColumn column)
			: base(column) {
		}
		public new bool Handled { get { return base.Handled; } set { base.Handled = value; } }
		public new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
		public new GridHeaderFilterValues Values { get { return base.Values; } }
		public new void AddValue(string displayText, string value) {
			base.AddValue(displayText, value);
		}
		public new void AddValue(string displayText, string value, string query) {
			base.AddValue(displayText, value, query);
		}
		public new void AddValue(string displayText, CriteriaOperator criteria) {
			base.AddValue(displayText, criteria);
		}
		public new void AddShowAll() {
			base.AddShowAll();
		}
	}
	public class ASPxGridViewCommandButtonEventArgs : ASPxGridCommandButtonEventArgs {
		public ASPxGridViewCommandButtonEventArgs(GridViewCommandColumn column, ColumnCommandButtonType buttonType, int visibleIndex, bool isEditingRow)
			: this(column, buttonType, string.Empty, null, null, visibleIndex, isEditingRow, GridCommandButtonRenderMode.Default) {
		}
		public ASPxGridViewCommandButtonEventArgs(GridViewCommandColumn column, ColumnCommandButtonType buttonType, string text, ImageProperties image, ButtonControlStyles styles, int visibleIndex, bool isEditingRow, GridCommandButtonRenderMode buttonRenderType)
			: base(text, image, styles, visibleIndex, isEditingRow, buttonRenderType) {
			Column = column;
			ButtonType = buttonType;
		}
		public ColumnCommandButtonType ButtonType { get; private set; }
		public GridViewCommandColumn Column { get; private set; }
		public bool IsEditingRow { get { return IsEditingItem; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
		public new bool Visible { get { return base.Visible; } set { base.Visible = value; } }
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		public new string Text { get { return base.Text; } set { base.Text = value; } }
		public new ImageProperties Image { get { return base.Image; } }
		public new ButtonControlStyles Styles { get { return base.Styles; } }
	}
	public class ASPxGridViewCustomButtonEventArgs : ASPxGridCustomCommandButtonEventArgs {
		public ASPxGridViewCustomButtonEventArgs(GridViewCommandColumnCustomButton button, int visibleIndex, GridViewTableCommandCellType cellType, bool isEditingRow)
			: base(button, visibleIndex, isEditingRow) {
			CellType = cellType;
		}
		public GridViewCommandColumn Column { get { return CustomButton.Column; } }
		public GridViewTableCommandCellType CellType { get; private set; }
		public new string ButtonID { get { return base.ButtonID; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
		public new DefaultBoolean Visible { get { return base.Visible; } set { base.Visible = value; } }
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		public new string Text { get { return base.Text; } set { base.Text = value; } }
		public bool IsEditingRow { get { return base.IsEditingItem; } }
		public new ImageProperties Image { get { return base.Image; } }
		public new ButtonControlStyles Styles { get { return base.Styles; } }
		protected new GridViewCommandColumnCustomButton CustomButton { get { return base.CustomButton as GridViewCommandColumnCustomButton; } }
		protected internal override GridCommandButtonRenderMode ButtonRenderType { get { return Column.ButtonType; } }
	}
	public class ASPxGridViewEditorEventArgs : ASPxGridEditorEventArgs {
		public ASPxGridViewEditorEventArgs(GridViewDataColumn column, int visibleIndex, ASPxEditBase editor, object keyValue, object value)
			: base(column, visibleIndex, editor, keyValue, value) {
		}
		public new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
		public new ASPxEditBase Editor { get { return base.Editor; } }
		public new object KeyValue { get { return base.KeyValue; } }
		public new object Value { get { return base.Value; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
	}
	public class ASPxGridViewTableRowEventArgs : ASPxGridViewItemEventArgs {
		public ASPxGridViewTableRowEventArgs(GridViewTableRow row)
			: this(row, null) {
		}
		public ASPxGridViewTableRowEventArgs(GridViewTableRow row, object keyValue)
			: base(row.VisibleIndex, keyValue) {
			Row = row;
		}
		public TableRow Row { get; private set; }
		public GridViewRowType RowType { get { return GridRow.RowType; } }
		public object GetValue(string fieldName) { return Grid.GetRowValues(VisibleIndex, fieldName); }
		public new object KeyValue { get { return Grid.DataProxy.HasCorrectKeyFieldName ? Grid.DataProxy.GetRowKeyValue(VisibleIndex) : null; } }
		protected ASPxGridView Grid { get { return GridRow.Grid; } }
		protected GridViewTableRow GridRow { get { return Row as GridViewTableRow; } }
	}
	public class ASPxGridViewTableDataCellEventArgs : ASPxGridViewItemEventArgs {
		public ASPxGridViewTableDataCellEventArgs(GridViewTableDataCell cell, object keyValue)
			: base(cell.VisibleIndex, keyValue) {
			Cell = cell;
		}
		public TableCell Cell { get; private set; }
		public GridViewDataColumn DataColumn { get { return DataCell.DataColumn; } }
		public object CellValue { get { return GetValue(DataColumn.FieldName); } }
		public object GetValue(string fieldName) { return Grid.GetRowValues(VisibleIndex, fieldName); }
		protected ASPxGridView Grid { get { return DataCell.Grid; } }
		protected GridViewTableDataCell DataCell { get { return Cell as GridViewTableDataCell; } }
	}
	public class ASPxGridViewTableCommandCellEventArgs : ASPxGridViewItemEventArgs {
		GridViewTableBaseCommandCell cell;
		public ASPxGridViewTableCommandCellEventArgs(GridViewTableBaseCommandCell cell, object keyValue)
			: base(cell.VisibleIndex, keyValue) {
			this.cell = cell;
		}
		public TableCell Cell { get { return cell; } }
		public GridViewCommandColumn CommandColumn { get { return cell.Column; } }
		public GridViewTableCommandCellType CommandCellType { get { return cell.CellType; } }
	}
	public class ASPxGridViewRowCommandEventArgs : ASPxGridViewItemEventArgs {
		public ASPxGridViewRowCommandEventArgs(int visibleIndex, object keyValue, CommandEventArgs commandArgs, object commandSource)
			: base(visibleIndex, keyValue) {
			CommandArgs = commandArgs;
			CommandSource = commandSource;
		}
		public CommandEventArgs CommandArgs { get; private set; }
		public object CommandSource { get; private set; }
	}
	public class ASPxGridViewEditorCreateEventArgs : ASPxGridEditorCreateEventArgs {
		public ASPxGridViewEditorCreateEventArgs(GridViewDataColumn column, int visibleIndex, EditPropertiesBase editorProperties, object keyValue, object value)
			: base(column, visibleIndex, editorProperties, keyValue, value) {
		}
		public new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
		public new EditPropertiesBase EditorProperties { get { return base.EditorProperties; } set { base.EditorProperties = value; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
		public new object KeyValue { get { return base.KeyValue; } }
		public new object Value { get { return base.Value; } set { base.Value = value; } }
	}
	public class ASPxGridViewSearchPanelEditorCreateEventArgs : ASPxGridViewEditorCreateEventArgs {
		public ASPxGridViewSearchPanelEditorCreateEventArgs(EditPropertiesBase editorProperties, object value)
			: base(null, -1, editorProperties, null, value) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewDataColumn Column { get { return base.Column; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new object KeyValue { get { return base.KeyValue; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new int VisibleIndex { get { return base.VisibleIndex; } }
	}
	public class ASPxGridViewSearchPanelEditorEventArgs : ASPxGridViewEditorEventArgs {
		public ASPxGridViewSearchPanelEditorEventArgs(ASPxEditBase editor, object value)
			: base(null, -1, editor, null, value) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewDataColumn Column { get { return base.Column; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new object KeyValue { get { return base.KeyValue; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new int VisibleIndex { get { return base.VisibleIndex; } }
	}
	public class ASPxGridViewBeforeColumnGroupingSortingEventArgs : ASPxGridBeforeColumnGroupingSortingEventArgs {
		public ASPxGridViewBeforeColumnGroupingSortingEventArgs(GridViewDataColumn column, ColumnSortOrder oldSortOrder, int oldSortIndex, int oldGroupIndex)
			: base(column, oldSortOrder, oldSortIndex, oldGroupIndex) {
		}
		public new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
		public new ColumnSortOrder OldSortOrder { get { return base.OldSortOrder; } }
		public new int OldSortIndex { get { return base.OldSortIndex; } }
		public new int OldGroupIndex { get { return base.OldGroupIndex; } }
	}
	public class ASPxGridViewCustomCallbackEventArgs : ASPxGridCustomCallbackEventArgs {
		public ASPxGridViewCustomCallbackEventArgs(string parameters) 
			: base(parameters) { }
		public new string Parameters { get { return base.Parameters; } }
	}
	public class ASPxGridViewCustomDataCallbackEventArgs : ASPxGridViewCustomCallbackEventArgs {
		public ASPxGridViewCustomDataCallbackEventArgs(string parameters) : base(parameters) { }
		public new object Result { get { return base.Result; } set { base.Result = value; } }
	}
	public class ASPxGridViewCustomButtonCallbackEventArgs : ASPxGridCustomButtonCallbackEventArgs {
		public ASPxGridViewCustomButtonCallbackEventArgs(string buttonID, int visibleIndex)
			: base(buttonID, visibleIndex) {
		}
		public new string ButtonID { get { return base.ButtonID; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
	}
	public class ASPxGridViewAfterPerformCallbackEventArgs : ASPxGridAfterPerformCallbackEventArgs {
		public ASPxGridViewAfterPerformCallbackEventArgs(string callbackName, string[] args)
			: base(callbackName, args) {
		}
		public new string CallbackName { get { return base.CallbackName; } }
		public new string[] Args { get { return base.Args; } }
	}
	public class ASPxGridViewSummaryDisplayTextEventArgs : ASPxGridSummaryDisplayTextEventArgs {
		public ASPxGridViewSummaryDisplayTextEventArgs(ASPxSummaryItem item, object value, string text, int visibleIndex, bool isGroupRow):base(item, value, text) {
			VisibleIndex = visibleIndex;
			IsGroupSummary = isGroupRow;
			IsTotalSummary = !IsGroupSummary;
		}
		public int VisibleIndex { get; private set; }
		public new object Value { get { return base.Value; } }
		public bool IsGroupSummary { get; private set; }
		public bool IsTotalSummary { get; private set; }
		public new ASPxSummaryItem Item { get { return base.Item as ASPxSummaryItem; } }
		public new string Text { get { return base.Text; } set { base.Text = value; } }
	}
	public class ASPxGridViewDetailRowButtonEventArgs : ASPxGridViewItemEventArgs {
		public ASPxGridViewDetailRowButtonEventArgs(int visibleIndex, object keyValue, bool isExpanded)
			: base(visibleIndex, keyValue) {
			ButtonState = GridViewDetailRowButtonState.Visible;
			IsExpanded = isExpanded;
		}
		public bool IsExpanded { get; private set; }
		public GridViewDetailRowButtonState ButtonState { get; set; }
	}
	public class ASPxGridViewAutoFilterEventArgs : EventArgs {
		public ASPxGridViewAutoFilterEventArgs(GridViewDataColumn column, CriteriaOperator criteria, GridViewAutoFilterEventKind kind, string value) {
			Column = column;
			Criteria = criteria;
			Kind = kind;
			Value = value;
		}
		public GridViewAutoFilterEventKind Kind { get; private set; }
		public GridViewDataColumn Column { get; private set; }
		public CriteriaOperator Criteria { get; set; }
		public string Value { get; set; }
	}
	public class ASPxGridViewOnClickRowFilterEventArgs : EventArgs {
		public ASPxGridViewOnClickRowFilterEventArgs(GridViewAutoFilterEventKind kind) {
			Kind = kind;
			Criteria = new Dictionary<string, CriteriaOperator>();
			Values = new Dictionary<string, string>();
		}
		public GridViewAutoFilterEventKind Kind { get; set; }
		public Dictionary<string, CriteriaOperator> Criteria { get; private set; }
		public Dictionary<string, string> Values { get; internal set; }
	}
	public class CustomColumnSortEventArgs : GridCustomColumnSortEventArgs {
		public CustomColumnSortEventArgs(GridViewDataColumn column, object value1, object value2, ColumnSortOrder sortOrder)
			: base(column, value1, value2, sortOrder) {
		}
		public new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
		public new ColumnSortOrder SortOrder { get { return base.SortOrder; } }
		public new object Value1 { get { return base.Value1; } }
		public new object Value2 { get { return base.Value2; } }
		public new int ListSourceRowIndex1 { get { return base.ListSourceRowIndex1; } }
		public new int ListSourceRowIndex2 { get { return base.ListSourceRowIndex2; } }
		public new int Result { get { return base.Result; } set { base.Result = value; } }
		public new bool Handled { get { return base.Handled; } set { base.Handled = value; } }
		public new object GetRow1Value(string fieldName) { return base.GetRow1Value(fieldName); }
		public new object GetRow2Value(string fieldName) { return base.GetRow2Value(fieldName); }
	}
	public class ASPxGridViewClientJSPropertiesEventArgs : CustomJSPropertiesEventArgs {
		public ASPxGridViewClientJSPropertiesEventArgs() : base() { }
		public ASPxGridViewClientJSPropertiesEventArgs(Dictionary<string, object> properties)
			: base(properties) {
		}
	}
	public class ASPxGridViewEditFormEventArgs : EventArgs {
		public ASPxGridViewEditFormEventArgs(WebControl editForm) {
			EditForm = editForm;
		}
		public WebControl EditForm { get; private set; }
	}
	public class ASPxGridViewTableFooterCellEventArgs : EventArgs {
		public ASPxGridViewTableFooterCellEventArgs(ASPxGridView grid, GridViewColumn column, int visibleIndex, TableCell cell) {
			Grid = grid;
			Column = column;
			VisibleIndex = visibleIndex;
			Cell = cell;
			IsTotalFooter = VisibleIndex < 0;
		}
		public GridViewColumn Column { get; private set; }
		public int VisibleIndex { get; private set; }
		public bool IsTotalFooter { get; private set; }
		public TableCell Cell { get; private set; }
		protected ASPxGridView Grid { get; private set; }
		public object GetSummaryValue(ASPxSummaryItem item) {
			if(IsTotalFooter)
				return Grid.GetTotalSummaryValue(item);
			return Grid.GetGroupSummaryValue(VisibleIndex, item);
		}
	}
	public class ASPxGridViewDetailRowEventArgs : EventArgs {
		public ASPxGridViewDetailRowEventArgs(int visibleIndex, bool expanded) {
			VisibleIndex = visibleIndex;
			Expanded = expanded;
		}
		public int VisibleIndex { get; private set; }
		public bool Expanded { get; private set; }
	}
	public class ASPxGridViewContextMenuEventArgs : EventArgs {
		protected internal ASPxGridViewContextMenuEventArgs(GridViewContextMenuHelper helper) {
			MenuHelper = helper;
			MenuType = MenuHelper.MenuType;
		}
		public GridViewContextMenuItemCollection Items { get { return MenuHelper.Items; } }
		public GridViewContextMenuType MenuType { get; private set; }
		protected GridViewContextMenuHelper MenuHelper { get; private set; }
		public GridViewContextMenuItem CreateItem(GridViewContextMenuCommand command) {
			return new GridViewContextMenuItem(command);
		}
		public GridViewContextMenuItem CreateItem(string text, string name) {
			return new GridViewContextMenuItem(text, name);
		}
	}
	public class ASPxGridViewContextMenuItemVisibilityEventArgs : EventArgs {
		protected internal ASPxGridViewContextMenuItemVisibilityEventArgs(GridViewContextMenuHelper helper) {
			MenuHelper = helper;
			Items = MenuHelper.Items;
			MenuType = MenuHelper.MenuType;
		}
		GridViewContextMenuHelper MenuHelper { get; set; }
		public GridViewContextMenuItemCollection Items { get; private set; }
		public GridViewContextMenuType MenuType { get; private set; }
		public void SetVisible(GridViewContextMenuItem item, bool visible) {
			MenuHelper.SetVisible(item, visible);
		}
		public void SetVisible(GridViewContextMenuItem item, GridViewColumn column, bool visible) {
			MenuHelper.SetVisible(item, column, visible);
		}
		public void SetVisible(GridViewContextMenuItem item, int visibleIndex, bool visible) {
			MenuHelper.SetVisible(item, visibleIndex, visible);
		}
		public void SetEnabled(GridViewContextMenuItem item, bool enabled) {
			MenuHelper.SetEnabled(item, enabled);
		}
		public void SetEnabled(GridViewContextMenuItem item, GridViewColumn column, bool enabled) {
			MenuHelper.SetEnabled(item, column, enabled);
		}
		public void SetEnabled(GridViewContextMenuItem item, int visibleIndex, bool enabled) {
			MenuHelper.SetEnabled(item, visibleIndex, enabled);
		}
	}
	public class ASPxGridViewContextMenuItemClickEventArgs : EventArgs {
		public ASPxGridViewContextMenuItemClickEventArgs(GridViewContextMenuType menuType, GridViewContextMenuItem item, int elementIndex) {
			MenuType = menuType;
			Item = item;
			ElementIndex = elementIndex;
		}
		public GridViewContextMenuType MenuType { get; private set; }
		public GridViewContextMenuItem Item { get; private set; }
		public int ElementIndex { get; private set; }
		public bool Handled { get; set; }
	}
	public class ASPxGridViewAddSummaryItemViaContextMenuEventArgs : EventArgs {
		public ASPxGridViewAddSummaryItemViaContextMenuEventArgs(ASPxSummaryItem summaryItem, GridViewDataColumn column) {
			SummaryItem = summaryItem;
			Column = column;
		}
		public ASPxSummaryItem SummaryItem { get; private set; }
		public GridViewDataColumn Column { get; private set; }
	}
	public class ASPxGridViewCustomErrorTextEventArgs : ASPxGridCustomErrorTextEventArgs {
		public ASPxGridViewCustomErrorTextEventArgs(GridErrorTextKind errorTextKind, string errorText) : this(null, errorTextKind, errorText) { }
		public ASPxGridViewCustomErrorTextEventArgs(Exception exception, GridErrorTextKind errorTextKind, string errorText)
			: base(exception, errorTextKind, errorText) {
		}
		public new string ErrorText { get { return base.ErrorText; } set { base.ErrorText = value; } }
		public new GridErrorTextKind ErrorTextKind { get { return base.ErrorTextKind; } }
		public new Exception Exception { get { return base.Exception; } }
	}
	public delegate void ASPxGridViewColumnDisplayTextEventHandler(object sender, ASPxGridViewColumnDisplayTextEventArgs e);
	public delegate void ASPxGridViewColumnDataEventHandler(object sender, ASPxGridViewColumnDataEventArgs e);
	public delegate void ASPxGridViewBeforeHeaderFilterFillItemsEventHandler(object sender, ASPxGridViewBeforeHeaderFilterFillItemsEventArgs e);
	public delegate void ASPxGridViewHeaderFilterEventHandler(object sender, ASPxGridViewHeaderFilterEventArgs e);
	public delegate void ASPxGridViewCommandButtonEventHandler(object sender, ASPxGridViewCommandButtonEventArgs e);
	public delegate void ASPxGridViewCustomButtonEventHandler(object sender, ASPxGridViewCustomButtonEventArgs e);
	public delegate void ASPxGridViewEditorEventHandler(object sender, ASPxGridViewEditorEventArgs e);
	public delegate void ASPxGridViewSearchPanelEditorCreateEventHandler(object sender, ASPxGridViewSearchPanelEditorCreateEventArgs e);
	public delegate void ASPxGridViewSearchPanelEditorEventHandler(object sender, ASPxGridViewSearchPanelEditorEventArgs e);
	public delegate void ASPxGridViewBeforeColumnGroupingSortingEventHandler(object sender, ASPxGridViewBeforeColumnGroupingSortingEventArgs e);
	public delegate void ASPxGridViewCustomCallbackEventHandler(object sender, ASPxGridViewCustomCallbackEventArgs e);
	public delegate void ASPxGridViewCustomDataCallbackEventHandler(object sender, ASPxGridViewCustomDataCallbackEventArgs e);
	public delegate void ASPxGridViewCustomButtonCallbackEventHandler(object sender, ASPxGridViewCustomButtonCallbackEventArgs e);
	public delegate void ASPxGridViewAfterPerformCallbackEventHandler(object sender, ASPxGridViewAfterPerformCallbackEventArgs e);
	public delegate void ASPxGridViewCustomColumnSortEventHandler(object sender, CustomColumnSortEventArgs e);
	public delegate void ASPxGridViewClientJSPropertiesEventHandler(object sender, ASPxGridViewClientJSPropertiesEventArgs e);
	public delegate void ASPxGridViewCustomErrorTextEventHandler(object sender, ASPxGridViewCustomErrorTextEventArgs e);
	public delegate void ASPxGridViewSummaryDisplayTextEventHandler(object sender, ASPxGridViewSummaryDisplayTextEventArgs e);
	public delegate void ASPxGridViewDetailRowButtonEventHandler(object sender, ASPxGridViewDetailRowButtonEventArgs e);
	public delegate void ASPxGridViewAutoFilterEventHandler(object sender, ASPxGridViewAutoFilterEventArgs e);
	public delegate void ASPxGridViewOnClickRowFilterEventHandler(object sender, ASPxGridViewOnClickRowFilterEventArgs e);
	public delegate void ASPxGridViewEditFormEventHandler(object sender, ASPxGridViewEditFormEventArgs e);
	public delegate void ASPxGridViewTableFooterCellEventHandler(object sender, ASPxGridViewTableFooterCellEventArgs e);
	public delegate void ASPxGridViewDetailRowEventHandler(object sender, ASPxGridViewDetailRowEventArgs e);
	public delegate void ASPxGridViewFillContextMenuItemsEventHandler(object sender, ASPxGridViewContextMenuEventArgs e);
	public delegate void ASPxGridViewAddSummaryItemViaContextMenuEventHandler(object sender, ASPxGridViewAddSummaryItemViaContextMenuEventArgs e);
	public delegate void ASPxGridViewContextMenuItemVisibilityEventHandler(object sender, ASPxGridViewContextMenuItemVisibilityEventArgs e);
	public delegate void ASPxGridViewContextMenuItemClickEventHandler(object sender, ASPxGridViewContextMenuItemClickEventArgs e);
	public delegate void ASPxGridViewEditorCreateEventHandler(object sender, ASPxGridViewEditorCreateEventArgs e);
	public delegate void ASPxGridViewRowCommandEventHandler(object sender, ASPxGridViewRowCommandEventArgs e);
	public delegate void ASPxGridViewTableRowEventHandler(object sender, ASPxGridViewTableRowEventArgs e);
	public delegate void ASPxGridViewTableDataCellEventHandler(object sender, ASPxGridViewTableDataCellEventArgs e);
	public delegate void ASPxGridViewTableCommandCellEventHandler(object sender, ASPxGridViewTableCommandCellEventArgs e);
}
namespace DevExpress.Web.Data {
	public class ASPxDataValidationEventArgs : ASPxGridDataValidationEventArgs {
		public ASPxDataValidationEventArgs(bool isNew) : this(-1, isNew) { }
		public ASPxDataValidationEventArgs(int visibleIndex, bool isNew)
			: base(visibleIndex, isNew) {
			Errors = new Dictionary<GridViewColumn, string>();
		}
		public new int VisibleIndex { get { return base.VisibleIndex; } }
		public new bool IsNewRow { get { return base.IsNewRow; } }
		public new string RowError { get { return base.RowError; } set { base.RowError = value; } }
		public new OrderedDictionary Keys { get { return base.Keys; } }
		public new OrderedDictionary OldValues { get { return base.OldValues; } }
		public new OrderedDictionary NewValues { get { return base.NewValues; } }
		public new bool HasErrors { get { return base.HasErrors; } }
		public Dictionary<GridViewColumn, string> Errors { get; private set; }
		protected internal override Dictionary<WebColumnBase, string> ErrorsInternal { get { return Errors.ToDictionary(e => e.Key as WebColumnBase, e => e.Value); } }
	}
	public class ASPxStartRowEditingEventArgs : ASPxStartItemEditingEventArgs {
		public ASPxStartRowEditingEventArgs(object editingKeyValue)
			: base(editingKeyValue) {
		}
		public new object EditingKeyValue { get { return base.EditingKeyValue; } }
	}
	public delegate void ASPxStartRowEditingEventHandler(object sender, ASPxStartRowEditingEventArgs e);
	public delegate void ASPxDataValidationEventHandler(object sender, ASPxDataValidationEventArgs e);
}
