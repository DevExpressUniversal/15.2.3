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
using System.Linq.Expressions;
using System.Web.Mvc;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
using DevExpress.Web.Mvc.UI;
namespace DevExpress.Web.Mvc {
	public enum MVCxCardViewColumnType { Default, TextBox, ButtonEdit, CheckBox, ComboBox, DateEdit, SpinEdit, TimeEdit, ColorEdit, DropDownEdit, Memo,
		BinaryImage, Image, HyperLink, ProgressBar, TokenBox }
	public class MVCxCardViewColumn : CardViewEditColumn {
		public MVCxCardViewColumn()
			: base() {
		}
		public MVCxCardViewColumn(string fieldName)
			: base(fieldName) {
		}
		public MVCxCardViewColumn(string fieldName, MVCxCardViewColumnType columnType)
			: this(fieldName) {
			ColumnType = columnType;
		}
		public MVCxCardViewColumn(string fieldName, string caption)
			: base(fieldName, caption) {
		}
		public MVCxCardViewColumn(string fieldName, string caption, MVCxCardViewColumnType columnType)
			: this(fieldName, caption) {
			ColumnType = columnType;
		}
		protected internal MVCxCardViewColumn(ModelMetadata metadata) {
			ColumnAdapter.Metadata = metadata;
		}
		public MVCxCardViewColumnType ColumnType {
			get { return (MVCxCardViewColumnType)ColumnAdapter.ColumnType; }
			set { ColumnAdapter.ColumnType = (int)value; }
		}
		protected internal new MVCxGridDataColumnAdapter ColumnAdapter { get { return (MVCxGridDataColumnAdapter)base.ColumnAdapter; } }
		protected override GridDataColumnAdapter CreateColumnAdapter() {
			return new MVCxGridDataColumnAdapter(this);
		}
		public void SetHeaderTemplateContent(Action<CardViewHeaderTemplateContainer> contentMethod) { HeaderTemplateContentMethod = contentMethod; }
		public void SetHeaderTemplateContent(string content) { HeaderTemplateContent = content; }
		public void SetDataItemTemplateContent(Action<CardViewDataItemTemplateContainer> contentMethod) { DataItemTemplateContentMethod = contentMethod; }
		public void SetDataItemTemplateContent(string content) { DataItemTemplateContent = content; }
		public void SetEditItemTemplateContent(Action<CardViewEditItemTemplateContainer> contentMethod) { EditItemTemplateContentMethod = contentMethod; }
		public void SetEditItemTemplateContent(string content) { EditItemTemplateContent = content; }
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<CardViewHeaderTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string DataItemTemplateContent { get; set; }
		protected internal Action<CardViewDataItemTemplateContainer> DataItemTemplateContentMethod { get; set; }
		protected internal string EditItemTemplateContent { get; set; }
		protected internal Action<CardViewEditItemTemplateContainer> EditItemTemplateContentMethod { get; set; }
		public override void Assign(CollectionItem source) {
			MVCxCardViewColumn column = source as MVCxCardViewColumn;
			if(column != null) {
				HeaderTemplateContent = column.HeaderTemplateContent;
				HeaderTemplateContentMethod = column.HeaderTemplateContentMethod;
				DataItemTemplateContent = column.DataItemTemplateContent;
				DataItemTemplateContentMethod = column.DataItemTemplateContentMethod;
				EditItemTemplateContent = column.EditItemTemplateContent;
				EditItemTemplateContentMethod = column.EditItemTemplateContentMethod;
			}
			base.Assign(source);
		}
		protected internal virtual void AssignState(GridBaseColumnState source) {
			if(source == null)
				return;
			SortOrder = source.SortOrder;
			SortIndex = source.SortIndex;
			Settings.AllowFilterBySearchPanel = source.AllowFilterBySearchPanel;
		}
	}
	public class MVCxCardViewColumnCollection : CardViewColumnCollection {
		public MVCxCardViewColumnCollection()
			: base(null) {
		}
		public MVCxCardViewColumnCollection(IWebControlObject owner)
			: base(owner) {
		}
		public void Add(Action<MVCxCardViewColumn> method) {
			method(Add());
		}
		public MVCxCardViewColumn Add() {
			MVCxCardViewColumn column = new MVCxCardViewColumn();
			Add(column);
			return column;
		}
		public MVCxCardViewColumn Add(string fieldName) {
			return Add(fieldName, string.Empty);
		}
		public MVCxCardViewColumn Add(string fieldName, MVCxCardViewColumnType columnType) {
			return Add(fieldName, string.Empty, columnType);
		}
		public MVCxCardViewColumn Add(string fieldName, string caption) {
			return Add(fieldName, caption, MVCxCardViewColumnType.Default);
		}
		public MVCxCardViewColumn Add(string fieldName, string caption, MVCxCardViewColumnType columnType) {
			MVCxCardViewColumn column = new MVCxCardViewColumn(fieldName, caption, columnType);
			Add(column);
			return column;
		}
	}
	public class MVCxCardViewColumnCollection<CardType> : MVCxCardViewColumnCollection {
		public MVCxCardViewColumnCollection()
			: base() {
		}
		public MVCxCardViewColumnCollection(IWebControlObject owner)
			: base(owner) {
		}
		public MVCxCardViewColumn Add<ValueType>(Expression<Func<CardType, ValueType>> expression, Action<MVCxCardViewColumn> method) {
			MVCxCardViewColumn column = Add(expression);
			if(method != null)
				method(column);
			return column;
		}
		public MVCxCardViewColumn Add<ValueType>(Expression<Func<CardType, ValueType>> expression) {
			ModelMetadata columnMetadata = GetColumnMetadata(expression);
			MVCxCardViewColumn column = new MVCxCardViewColumn(columnMetadata);
			if(expression != null) {
				string fieldName = ExpressionHelper.GetExpressionText(expression);
				column.FieldName = HtmlHelperExtension.HtmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
			}
			Add(column);
			return column;
		}
		static ModelMetadata GetColumnMetadata<ValueType>(Expression<Func<CardType, ValueType>> expression) {
			if(expression == null)
				return null;
			return ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<CardType>());
		}
	}
}
