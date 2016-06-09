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
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.Web {
	public delegate void CustomFilterExpressionDisplayTextEventHandler(object sender, CustomFilterExpressionDisplayTextEventArgs e);
	public class CustomFilterExpressionDisplayTextEventArgs : EventArgs {
		CriteriaOperator criteria;
		string displayText;
		string filterExpression;
		public CustomFilterExpressionDisplayTextEventArgs(string filterExpression, string displayText) {
			this.filterExpression = filterExpression;
			this.displayText = displayText;
			EncodeHtml = true;
		}
		public string FilterExpression { get { return filterExpression; } }
		public CriteriaOperator Criteria { 
			get {
				if(Object.ReferenceEquals(criteria, null))
					criteria = CriteriaOperator.Parse(filterExpression);
				return criteria; 
			} 
		}
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public bool EncodeHtml { get; set; }
	}
	public delegate void FilterControlOperationVisibilityEventHandler(object sender, FilterControlOperationVisibilityEventArgs e);
	public class FilterControlOperationVisibilityEventArgs : EventArgs {		
		IFilterablePropertyInfo propertyInfo;
		ClauseType operation;
		bool visible = true;
		public FilterControlOperationVisibilityEventArgs(IFilterablePropertyInfo propertyInfo, ClauseType operation) {
			this.propertyInfo = propertyInfo;
			this.operation = operation;
		}
		public IFilterablePropertyInfo PropertyInfo { get { return propertyInfo; } }
		public ClauseType Operation { get { return operation; } }
		public bool Visible { get { return visible; } set { visible = value; } }
	}
	public delegate void FilterControlParseValueEventHandler(object sender, FilterControlParseValueEventArgs e);
	public class FilterControlParseValueEventArgs : EventArgs {
		IFilterablePropertyInfo propertyInfo;
		string text;
		object value;
		bool handled;
		public FilterControlParseValueEventArgs(IFilterablePropertyInfo propertyInfo, string text) {
			this.propertyInfo = propertyInfo;
			this.text = text;
		}
		public IFilterablePropertyInfo PropertyInfo { get { return propertyInfo; } }
		public string Text { get { return text; } }
		public object Value { get { return value; } set { this.value = value; } }
		public bool Handled { get { return handled; } set { handled = value; } }
	}
	public delegate void FilterControlCustomValueDisplayTextEventHandler(object sender, FilterControlCustomValueDisplayTextEventArgs e);
	public class FilterControlCustomValueDisplayTextEventArgs : EventArgs {
		IFilterablePropertyInfo propertyInfo;
		object value;
		string displayText;
		public FilterControlCustomValueDisplayTextEventArgs(IFilterablePropertyInfo propertyInfo, object value, string displayText) {
			this.propertyInfo = propertyInfo;
			this.displayText = displayText;
			this.value = value;
			EncodeHtml = true;
		}
		public IFilterablePropertyInfo PropertyInfo { get { return propertyInfo; } }
		public object Value { get { return value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public bool EncodeHtml { get; set; }
	}
	public delegate void FilterControlCriteriaValueEditorInitializeEventHandler(object sender, FilterControlCriteriaValueEditorInitializeEventArgs e);
	public class FilterControlCriteriaValueEditorInitializeEventArgs : EventArgs {
		public FilterControlColumn Column { get; private set; }
		public ASPxEditBase Editor { get; private set; }
		public object Value { get; private set; }
		public FilterControlCriteriaValueEditorInitializeEventArgs(FilterControlColumn column, ASPxEditBase editor, object value) {
			Column = column;
			Editor = editor;
			Value = value;
		}
	}
	public delegate void FilterControlCriteriaValueEditorCreateEventHandler(object sender, FilterControlCriteriaValueEditorCreateEventArgs e);
	public class FilterControlCriteriaValueEditorCreateEventArgs : EventArgs {
		public FilterControlColumn Column { get; private set; }
		public EditPropertiesBase EditorProperties { get; set; }
		public object Value { get; set; }
		public FilterControlCriteriaValueEditorCreateEventArgs(FilterControlColumn column, EditPropertiesBase editorProperties, object value) {
			Column = column;
			EditorProperties = editorProperties;
			Value = value;
		}
	}
}
