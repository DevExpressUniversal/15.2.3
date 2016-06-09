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
using DevExpress.Web.Localization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	public class ASPxGridItemEventArgs : EventArgs {
		public ASPxGridItemEventArgs(int visibleIndex, object keyValue) {
			VisibleIndex = visibleIndex;
			KeyValue = keyValue;
		}
		protected object KeyValue { get; private set; }
		protected int VisibleIndex { get; private set; }
	}
	public class ASPxGridEditorEventArgs : ASPxGridItemEventArgs {
		public ASPxGridEditorEventArgs(IWebGridDataColumn column, int visibleIndex, ASPxEditBase editor, object keyValue, object value)
			: base(visibleIndex, keyValue) {
			Column = column;
			Editor = editor;
			Value = value;
		}
		protected IWebGridDataColumn Column { get; private set; }
		protected ASPxEditBase Editor { get; private set; }
		protected object Value { get; private set; }
	}
	public class ASPxGridEditorCreateEventArgs : ASPxGridItemEventArgs {
		EditPropertiesBase editorProperties;
		public ASPxGridEditorCreateEventArgs(IWebGridDataColumn column, int visibleIndex, EditPropertiesBase editorProperties, object keyValue, object value)
			: base(visibleIndex, keyValue) {
			Column = column;
			Value = value;
			this.editorProperties = editorProperties;
		}
		protected IWebGridDataColumn Column { get; private set; }
		protected internal object Value { get; set; }
		protected internal EditPropertiesBase EditorProperties {
			get { return editorProperties; }
			set {
				if(value == null) throw new ArgumentNullException("value");
				editorProperties = value;
			}
		}
	}
	public class ASPxGridHeaderFilterEventArgs : EventArgs {
		public ASPxGridHeaderFilterEventArgs(IWebGridDataColumn column, GridHeaderFilterValues values) {
			Column = column;
			Values = values;
		}
		protected string HeaderFilterShowAllText {
			get {
				if(Column == null || Column.Adapter.Grid == null) return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterShowAllItem);
				return Column.Adapter.Grid.SettingsText.GetHeaderFilterShowAll();
			}
		}
		protected IWebGridDataColumn Column { get; private set; }
		protected internal GridHeaderFilterValues Values { get; private set; }
		protected void AddValue(string displayText, string value) {
			Values.Add(new FilterValue(displayText, value));
		}
		protected void AddValue(string displayText, string value, string query) {
			Values.Add(new FilterValue(displayText, value, CriteriaOperator.Parse(query).ToString()));
		}
		protected void AddShowAll() {
			Values.Add(FilterValue.CreateShowAllValue(HeaderFilterShowAllText));
		}
	}
	public class ASPxGridBeforeHeaderFilterFillItemsEventArgs : ASPxGridHeaderFilterEventArgs {
		public ASPxGridBeforeHeaderFilterFillItemsEventArgs(IWebGridDataColumn column)
			: base(column, new GridHeaderFilterValues()) {
		}
		protected internal bool Handled { get; set; }
		protected void AddValue(string displayText, CriteriaOperator criteria) {
			var query = string.Empty;
			if(!ReferenceEquals(criteria, null))
				query = criteria.ToString();
			Values.Add(new FilterValue(displayText, string.Empty, query));
		}
	}
	public class ASPxGridBeforeColumnGroupingSortingEventArgs : EventArgs {
		public ASPxGridBeforeColumnGroupingSortingEventArgs(IWebGridDataColumn column, ColumnSortOrder oldSortOrder, int oldSortIndex, int oldGroupIndex) {
			Column = column;
			OldSortOrder = oldSortOrder;
			OldSortIndex = oldSortIndex;
			OldGroupIndex = oldGroupIndex;
		}
		protected IWebGridDataColumn Column { get; private set; }
		protected int OldGroupIndex { get; private set; }
		protected ColumnSortOrder OldSortOrder { get; private set; }
		protected int OldSortIndex { get; private set; }
	}
	public class ASPxGridCustomCallbackEventArgs : EventArgs {
		public ASPxGridCustomCallbackEventArgs(string parameters) {
			Parameters = parameters;
		}
		protected internal string Parameters { get; private set; }
		protected internal object Result { get; set; }
	}
	public class ASPxGridCustomButtonCallbackEventArgs : EventArgs {
		public ASPxGridCustomButtonCallbackEventArgs(string buttonID, int visibleIndex) {
			ButtonID = buttonID;
			VisibleIndex = visibleIndex;
		}
		protected string ButtonID { get; private set; }
		protected int VisibleIndex { get; private set; }
	}
	public class ASPxGridAfterPerformCallbackEventArgs : EventArgs {
		public ASPxGridAfterPerformCallbackEventArgs(string callbackName, string[] args) {
			CallbackName = callbackName;
			Args = args;
		}
		protected string CallbackName { get; private set; }
		protected string[] Args { get; private set; }
	}
	public class ASPxGridColumnDataEventArgs : EventArgs {
		public ASPxGridColumnDataEventArgs(IWebGridDataColumn column, int listSourceRow, object value, bool isGetAction) {
			IsGetData = isGetAction;
			Column = column;
			ListSourceRowIndex = listSourceRow;
			Value = value;
		}
		protected WebDataProxy Data { get { return Column.Adapter.Grid.DataProxy; } }
		protected IWebGridDataColumn Column { get; private set; }
		protected int ListSourceRowIndex { get; private set; }
		protected bool IsGetData { get; private set; }
		protected bool IsSetData { get { return !IsGetData; } }
		protected internal object Value { get; set; }
		protected object GetListSourceFieldValue(string fieldName) {
			return GetListSourceFieldValue(ListSourceRowIndex, fieldName);
		}
		protected object GetListSourceFieldValue(int listSourceRowIndex, string fieldName) {
			return Data.GetListSourceRowValue(listSourceRowIndex, fieldName);
		}
	}
	public class ASPxGridColumnDisplayTextEventArgs : EventArgs {
		const bool DefaultEncodeHtml = true;
		public ASPxGridColumnDisplayTextEventArgs(IWebGridDataColumn column, int visibleRowIndex, object _value) {
			Value = _value;
			Column = column;
			VisibleIndex = visibleRowIndex;
			EncodeHtml = CalcEncodeHtml();
		}
		protected internal ASPxGridColumnDisplayTextEventArgs(IWebGridDataColumn column, int visibleRowIndex, object _value, IValueProvider rowValueProvider)
			: this(column, visibleRowIndex, _value) {
			RowValueProvider = rowValueProvider;
		}
		protected ASPxGridBase Grid { get { return Column.Adapter.Grid; } }
		protected WebDataProxy Data { get { return Column.Adapter.DataProxy; } }
		protected IValueProvider RowValueProvider { get; private set; }
		protected IWebGridDataColumn Column { get; private set; }
		protected int VisibleIndex { get; private set; }
		protected internal string DisplayText { get; set; }
		protected object Value { get; set; }
		protected internal bool EncodeHtml { get; set; }
		protected object GetFieldValue(string fieldName) {
			if(RowValueProvider != null)
				return RowValueProvider.GetValue(fieldName); 
			return GetFieldValue(VisibleIndex, fieldName);
		}
		protected object GetFieldValue(int visibleRowIndex, string fieldName) {
			return Data.GetRowValue(visibleRowIndex, fieldName);
		}
		protected virtual bool CalcEncodeHtml() {
			if(Column == null || Grid == null)
				return DefaultEncodeHtml;
			var prop = Grid.RenderHelper.GetColumnEdit(Column);
			return prop != null ? prop.EncodeHtml : DefaultEncodeHtml;
		}
	}
	public class ASPxGridCustomErrorTextEventArgs : EventArgs {
		string errorText;
		public ASPxGridCustomErrorTextEventArgs(Exception exception, GridErrorTextKind errorTextKind, string errorText) {
			ErrorTextKind = errorTextKind;
			Exception = exception;
			this.errorText = errorText;
		}
		protected internal string ErrorText { get { return errorText; } set { errorText = value != null ? value : string.Empty; } }
		protected GridErrorTextKind ErrorTextKind { get; private set; }
		protected Exception Exception { get; private set; }
	}
	public class GridCustomColumnSortEventArgs : EventArgs {
		public GridCustomColumnSortEventArgs(IWebGridDataColumn column, object value1, object value2, ColumnSortOrder sortOrder) {
			Column = column;
			SortOrder = sortOrder;
			Value1 = value1;
			Value2 = value2;
		}
		protected IWebGridDataColumn Column { get; private set; }
		protected ColumnSortOrder SortOrder { get; private set; }
		protected object Value1 { get; private set; }
		protected object Value2 { get; private set; }
		protected int ListSourceRowIndex1 { get; private set; }
		protected int ListSourceRowIndex2 { get; private set; }
		protected int Result { get; set; }
		protected bool Handled { get; set; }
		protected object GetRow1Value(string fieldName) { return GetRowValueCore(ListSourceRowIndex1, fieldName); }
		protected object GetRow2Value(string fieldName) { return GetRowValueCore(ListSourceRowIndex2, fieldName); }
		protected ASPxGridBase Grid { get { return Column.Adapter.Grid; } }
		object GetRowValueCore(int listSourceRow, string fieldName) {
			if(Grid == null) return null;
			return Grid.DataProxy.GetListSourceRowValue(listSourceRow, fieldName);
		}
		internal int? GetSortResult() {
			if(!Handled) return null;
			return Result;
		}
		internal void SetArgs(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder) {
			SortOrder = sortOrder;
			Value1 = value1;
			Value2 = value2;
			Result = 0;
			Handled = false;
			ListSourceRowIndex1 = listSourceRow1;
			ListSourceRowIndex2 = listSourceRow2;
		}
	}
	public abstract class ASPxGridCustomCommandButtonEventArgs : EventArgs {
		public ASPxGridCustomCommandButtonEventArgs(GridCustomCommandButton button, int visibleIndex, bool isEditingItem) {
			CustomButton = button;
			VisibleIndex = visibleIndex;
			IsEditingItem = isEditingItem;
			Visible = DefaultBoolean.Default;
			Enabled = true;
			ButtonID = button.GetID();
			Text = button.GetText();
			Image = new ImageProperties();
			Image.Assign(button.Image);
			Styles = new ButtonControlStyles(null);
			Styles.Assign(button.Styles);
		}
		protected internal string ButtonID { get; private set; }
		protected internal int VisibleIndex { get; private set; }
		protected DefaultBoolean Visible { get; set; }
		protected internal bool Enabled { get; set; }
		protected internal string Text { get; set; }
		protected internal ImageProperties Image { get; private set; }
		protected internal ButtonControlStyles Styles { get; private set; }
		protected bool IsEditingItem { get; private set; }
		protected GridCustomCommandButton CustomButton { get; private set; }
		protected internal abstract GridCommandButtonRenderMode ButtonRenderType { get; }
	}
	public class ASPxGridCommandButtonEventArgs : EventArgs {
		public ASPxGridCommandButtonEventArgs(string text, ImageProperties image, ButtonControlStyles styles, int visibleIndex, bool isEditingRow, GridCommandButtonRenderMode buttonRenderType) {
			Text = text;
			VisibleIndex = visibleIndex;
			IsEditingItem = isEditingRow;
			Visible = true;
			Enabled = true;
			ButtonRenderType = buttonRenderType;
			Image = new ImageProperties();
			Image.CopyFrom(image);
			Styles = new ButtonControlStyles(null);
			Styles.Assign(styles);
		}
		protected internal int VisibleIndex { get; private set; }
		protected bool Visible { get; set; }
		protected internal bool Enabled { get; set; }
		protected internal string Text { get; set; }
		protected internal ImageProperties Image { get; private set; }
		protected internal ButtonControlStyles Styles { get; private set; }
		protected bool IsEditingItem { get; private set; }
		protected internal GridCommandButtonRenderMode ButtonRenderType { get; private set; }
	}
	public class ASPxStartItemEditingEventArgs : CancelEventArgs {
		public ASPxStartItemEditingEventArgs(object editingKeyValue) {
			EditingKeyValue = editingKeyValue;
		}
		protected object EditingKeyValue { get; private set; }
	}
	public abstract class ASPxGridDataValidationEventArgs : EventArgs {
		public ASPxGridDataValidationEventArgs(int visibleIndex, bool isNew) {
			VisibleIndex = visibleIndex;
			IsNewRow = isNew;
			RowError = string.Empty;
			Keys = new OrderedDictionary();
			OldValues = new OrderedDictionary();
			NewValues = new OrderedDictionary();
		}
		protected internal int VisibleIndex { get; private set; }
		protected bool IsNewRow { get; private set; }
		protected internal string RowError { get; set; }
		protected internal OrderedDictionary Keys { get; private set; }
		protected internal OrderedDictionary OldValues { get; private set; }
		protected internal OrderedDictionary NewValues { get; private set; }
		protected internal bool HasErrors { get { return ErrorsInternal.Count > 0 || !string.IsNullOrEmpty(RowError); } }
		protected internal abstract Dictionary<WebColumnBase, string> ErrorsInternal { get; }
	}
	public class ASPxGridSummaryDisplayTextEventArgs : EventArgs {
		string text;
		public ASPxGridSummaryDisplayTextEventArgs(ASPxSummaryItemBase item, object value, string text) {
			Item = item;
			Value = value;
			this.text = text;
		}
		protected internal object Value { get; private set; }
		protected internal ASPxSummaryItemBase Item { get; private set; }
		protected internal string Text { get { return text; } set { text = value ?? string.Empty; } }
	}
	public delegate void FilterControlColumnsCreatedEventHandler(object source, FilterControlColumnsCreatedEventArgs e);
	public class FilterControlColumnsCreatedEventArgs {
		public FilterControlColumnCollection Columns { get; set; }
		public FilterControlColumnsCreatedEventArgs(FilterControlColumnCollection columns) {
			Columns = columns;
		}
	}
}
