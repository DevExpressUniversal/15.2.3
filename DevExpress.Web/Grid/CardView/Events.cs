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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
namespace DevExpress.Web {
	public class ASPxCardViewColumnDisplayTextEventArgs : ASPxGridColumnDisplayTextEventArgs {
		public ASPxCardViewColumnDisplayTextEventArgs(CardViewColumn column, int visibleIndex, object _value)
			: base(column, visibleIndex, _value) {
		}
		internal ASPxCardViewColumnDisplayTextEventArgs(CardViewColumn column, int visibleIndex, object _value, IValueProvider rowValueProvider)
			: base(column, visibleIndex, _value, rowValueProvider) {
		}
		public new CardViewColumn Column { get { return (CardViewColumn)base.Column; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
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
	public class ASPxCardViewColumnDataEventArgs : ASPxGridColumnDataEventArgs {
		public ASPxCardViewColumnDataEventArgs(CardViewColumn column, int listSourceRow, object value, bool isGetAction)
			: base(column, listSourceRow, value, isGetAction) {
		}
		public new CardViewColumn Column { get { return base.Column as CardViewColumn; } }
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
	public class ASPxCardViewHeaderFilterEventArgs : ASPxGridHeaderFilterEventArgs {
		public ASPxCardViewHeaderFilterEventArgs(CardViewColumn column, GridHeaderFilterValues values)
			: base(column, values) {
		}
		public new CardViewColumn Column { get { return base.Column as CardViewColumn; } }
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
	public class ASPxCardViewBeforeHeaderFilterFillItemsEventArgs : ASPxGridBeforeHeaderFilterFillItemsEventArgs {
		public ASPxCardViewBeforeHeaderFilterFillItemsEventArgs(CardViewColumn column)
			: base(column) {
		}
		public new bool Handled { get { return base.Handled; } set { base.Handled = value; } }
		public new CardViewColumn Column { get { return base.Column as CardViewColumn; } }
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
	public class ASPxCardViewCommandButtonEventArgs : ASPxGridCommandButtonEventArgs {
		public ASPxCardViewCommandButtonEventArgs(CardViewCommandButtonType buttonType, int visibleIndex, bool isEditingCard)
			: this(buttonType, visibleIndex, isEditingCard, GridCommandButtonRenderMode.Default, string.Empty, null, null) {
		}
		public ASPxCardViewCommandButtonEventArgs(CardViewCommandButtonType buttonType, int visibleIndex, bool isEditingCard, GridCommandButtonRenderMode buttonRenderType, string text, ImageProperties image, ButtonControlStyles styles)
			: base(text, image, styles, visibleIndex, isEditingCard, buttonRenderType) {
			ButtonType = buttonType;
		}
		public CardViewCommandButtonType ButtonType { get; private set; }
		public bool IsEditingCard { get { return IsEditingItem; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
		public new bool Visible { get { return base.Visible; } set { base.Visible = value; } }
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		public new string Text { get { return base.Text; } set { base.Text = value; } }
		public new ImageProperties Image { get { return base.Image; } }
		public new ButtonControlStyles Styles { get { return base.Styles; } }
	}
	public class ASPxCardViewCustomCommandButtonEventArgs : ASPxGridCustomCommandButtonEventArgs {
		public ASPxCardViewCustomCommandButtonEventArgs(CardViewCustomCommandButton button, int visibleIndex, bool isEditingCard)
			: base(button, visibleIndex, isEditingCard) {
		}
		public new string ButtonID { get { return base.ButtonID; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
		public new DefaultBoolean Visible { get { return base.Visible; } set { base.Visible = value; } }
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		public new string Text { get { return base.Text; } set { base.Text = value; } }
		public bool IsEditingCard { get { return base.IsEditingItem; } }
		public new ImageProperties Image { get { return base.Image; } }
		public new ButtonControlStyles Styles { get { return base.Styles; } }
		protected new CardViewCustomCommandButton CustomButton { get { return base.CustomButton as CardViewCustomCommandButton; } }
		protected CardViewCommandLayoutItem LayoutItem { get { return CustomButton.LayoutItem; } }
		protected internal override GridCommandButtonRenderMode ButtonRenderType { get { return LayoutItem.ButtonRenderMode; } }
	}
	public class ASPxCardViewEditorEventArgs : ASPxGridEditorEventArgs {
		public ASPxCardViewEditorEventArgs(CardViewColumn column, int visibleIndex, ASPxEditBase editor, object keyValue, object value)
			: base(column, visibleIndex, editor, keyValue, value) {
		}
		public new object KeyValue { get { return base.KeyValue; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
		public new ASPxEditBase Editor { get { return base.Editor; } }
		public new object Value { get { return base.Value; } }
		public new CardViewColumn Column { get { return base.Column as CardViewColumn; } }
	}
	public class ASPxCardViewSearchPanelEditorCreateEventArgs : ASPxGridEditorCreateEventArgs {
		public ASPxCardViewSearchPanelEditorCreateEventArgs(EditPropertiesBase editorProperties, object value)
			: base(null, -1, editorProperties, null, value) {
		}
		public new EditPropertiesBase EditorProperties { get { return base.EditorProperties; } set { base.EditorProperties = value; } }
		public new object Value { get { return base.Value; } set { base.Value = value; } }
	}
	public class ASPxCardViewSearchPanelEditorEventArgs : ASPxGridEditorEventArgs {
		public ASPxCardViewSearchPanelEditorEventArgs(ASPxEditBase editor, object value)
			: base(null, -1, editor, null, value) {
		}
		public new ASPxEditBase Editor { get { return base.Editor; } }
		public new object Value { get { return base.Value; } }
	}
	public class ASPxCardViewBeforeColumnSortingEventArgs : ASPxGridBeforeColumnGroupingSortingEventArgs {
		public ASPxCardViewBeforeColumnSortingEventArgs(CardViewColumn column, ColumnSortOrder oldSortOrder, int oldSortIndex)
			: base(column, oldSortOrder, oldSortIndex, -1) {
		}
		public new CardViewColumn Column { get { return base.Column as CardViewColumn; } }
		public new ColumnSortOrder OldSortOrder { get { return base.OldSortOrder; } }
		public new int OldSortIndex { get { return base.OldSortIndex; } }
	}
	public class ASPxCardViewCustomCallbackEventArgs : ASPxGridCustomCallbackEventArgs {
		public ASPxCardViewCustomCallbackEventArgs(string parameters)
			: base(parameters) { }
		public new string Parameters { get { return base.Parameters; } }
	}
	public class ASPxCardViewCustomDataCallbackEventArgs : ASPxCardViewCustomCallbackEventArgs {
		public ASPxCardViewCustomDataCallbackEventArgs(string parameters) : base(parameters) { }
		public new object Result { get { return base.Result; } set { base.Result = value; } }
	}
	public class ASPxCardViewCustomButtonCallbackEventArgs : ASPxGridCustomButtonCallbackEventArgs {
		public ASPxCardViewCustomButtonCallbackEventArgs(string buttonID, int visibleIndex)
			: base(buttonID, visibleIndex) {
		}
		public new string ButtonID { get { return base.ButtonID; } }
		public new int VisibleIndex { get { return base.VisibleIndex; } }
	}
	public class ASPxCardViewAfterPerformCallbackEventArgs : ASPxGridAfterPerformCallbackEventArgs {
		public ASPxCardViewAfterPerformCallbackEventArgs(string callbackName, string[] args)
			: base(callbackName, args) {
		}
		public new string CallbackName { get { return base.CallbackName; } }
		public new string[] Args { get { return base.Args; } }
	}
	public class CardViewCustomColumnSortEventArgs : GridCustomColumnSortEventArgs {
		public CardViewCustomColumnSortEventArgs(CardViewColumn column, object value1, object value2, ColumnSortOrder sortOrder)
			: base(column, value1, value2, sortOrder) {
		}
		public new CardViewColumn Column { get { return base.Column as CardViewColumn; } }
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
	public class ASPxCardViewClientJSPropertiesEventArgs : CustomJSPropertiesEventArgs {
		public ASPxCardViewClientJSPropertiesEventArgs(Dictionary<string, object> properties)
			: base(properties) {
		}
	}
	public class ASPxCardViewCustomErrorTextEventArgs : ASPxGridCustomErrorTextEventArgs {
		public ASPxCardViewCustomErrorTextEventArgs(GridErrorTextKind errorTextKind, string errorText) : this(null, errorTextKind, errorText) { }
		public ASPxCardViewCustomErrorTextEventArgs(Exception exception, GridErrorTextKind errorTextKind, string errorText)
			: base(exception, errorTextKind, errorText) {
		}
		public new string ErrorText { get { return base.ErrorText; } set { base.ErrorText = value; } }
		public new GridErrorTextKind ErrorTextKind { get { return base.ErrorTextKind; } }
		public new Exception Exception { get { return base.Exception; } }
	}
	public class ASPxStartCardEditingEventArgs : ASPxStartItemEditingEventArgs {
		public ASPxStartCardEditingEventArgs(object editingKeyValue)
			: base(editingKeyValue) {
		}
		public new object EditingKeyValue { get { return base.EditingKeyValue; } }
	}
	public class ASPxCardViewDataValidationEventArgs : ASPxGridDataValidationEventArgs {
		public ASPxCardViewDataValidationEventArgs(bool isNew) : this(-1, isNew) { }
		public ASPxCardViewDataValidationEventArgs(int visibleIndex, bool isNew)
			: base(visibleIndex, isNew) {
			Errors = new Dictionary<CardViewColumn, string>();
		}
		public new int VisibleIndex { get { return base.VisibleIndex; } }
		public bool IsNewCard { get { return base.IsNewRow; } }
		public string CardError { get { return base.RowError; } set { base.RowError = value; } }
		public new OrderedDictionary Keys { get { return base.Keys; } }
		public new OrderedDictionary OldValues { get { return base.OldValues; } }
		public new OrderedDictionary NewValues { get { return base.NewValues; } }
		public new bool HasErrors { get { return base.HasErrors; } }
		public Dictionary<CardViewColumn, string> Errors { get; private set; }
		protected internal override Dictionary<WebColumnBase, string> ErrorsInternal { get { return Errors.ToDictionary(e => e.Key as WebColumnBase, e => e.Value); } }
	}
	public class ASPxCardViewSummaryDisplayTextEventArgs : ASPxGridSummaryDisplayTextEventArgs {
		public ASPxCardViewSummaryDisplayTextEventArgs(ASPxCardViewSummaryItem item, object value, string text) : base(item, value, text) { }
		public new object Value { get { return base.Value; } }
		public new ASPxCardViewSummaryItem Item { get { return base.Item as ASPxCardViewSummaryItem; } }
		public new string Text { get { return base.Text; } set { base.Text = value; } }
	}
	public delegate void ASPxCardViewColumnDisplayTextEventHandler(object sender, ASPxCardViewColumnDisplayTextEventArgs e);
	public delegate void ASPxCardViewColumnDataEventHandler(object sender, ASPxCardViewColumnDataEventArgs e);
	public delegate void ASPxCardViewHeaderFilterEventHandler(object sender, ASPxCardViewHeaderFilterEventArgs e);
	public delegate void ASPxCardViewBeforeHeaderFilterFillItemsEventHandler(object sender, ASPxCardViewBeforeHeaderFilterFillItemsEventArgs e);
	public delegate void ASPxCardViewCommandButtonEventHandler(object sender, ASPxCardViewCommandButtonEventArgs e);
	public delegate void ASPxCardViewCustomButtonEventHandler(object sender, ASPxCardViewCustomCommandButtonEventArgs e);
	public delegate void ASPxCardViewEditorEventHandler(object sender, ASPxCardViewEditorEventArgs e);
	public delegate void ASPxCardViewSearchPanelEditorCreateEventHandler(object sender, ASPxCardViewSearchPanelEditorCreateEventArgs e);
	public delegate void ASPxCardViewSearchPanelEditorEventHandler(object sender, ASPxCardViewSearchPanelEditorEventArgs e);
	public delegate void ASPxCardViewBeforeColumnSortingEventHandler(object sender, ASPxCardViewBeforeColumnSortingEventArgs e);
	public delegate void ASPxCardViewCustomCallbackEventHandler(object sender, ASPxCardViewCustomCallbackEventArgs e);
	public delegate void ASPxCardViewCustomDataCallbackEventHandler(object sender, ASPxCardViewCustomDataCallbackEventArgs e);
	public delegate void ASPxCardViewCustomButtonCallbackEventHandler(object sender, ASPxCardViewCustomButtonCallbackEventArgs e);
	public delegate void ASPxCardViewAfterPerformCallbackEventHandler(object sender, ASPxCardViewAfterPerformCallbackEventArgs e);
	public delegate void ASPxCardViewCustomColumnSortEventHandler(object sender, CardViewCustomColumnSortEventArgs e);
	public delegate void ASPxCardViewClientJSPropertiesEventHandler(object sender, ASPxCardViewClientJSPropertiesEventArgs e);
	public delegate void ASPxCardViewCustomErrorTextEventHandler(object sender, ASPxCardViewCustomErrorTextEventArgs e);
	public delegate void ASPxStartCardEditingEventHandler(object sender, ASPxStartCardEditingEventArgs e);
	public delegate void ASPxCardViewDataValidationEventHandler(object sender, ASPxCardViewDataValidationEventArgs e);
	public delegate void ASPxCardViewSummaryDisplayTextEventHandler(object sender, ASPxCardViewSummaryDisplayTextEventArgs e);
}
