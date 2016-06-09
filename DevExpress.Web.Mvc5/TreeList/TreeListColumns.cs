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
using DevExpress.Data;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxTreeList;
using System.Web.Mvc;
using System.Linq.Expressions;
using DevExpress.Web.Mvc.UI;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc {
	public enum MVCxTreeListColumnType {
		Default, BinaryImage, ButtonEdit, CheckBox, ColorEdit, ComboBox, DateEdit, DropDownEdit,
		HyperLink, Image, Memo, ProgressBar, SpinEdit, TextBox, TimeEdit, TokenBox
	}
	public class MVCxTreeListColumn : TreeListEditDataColumn, IDateEditIDResolver {
		ModelMetadata metadata;
		MVCxTreeListColumnType columnType = MVCxTreeListColumnType.Default;
		public MVCxTreeListColumn()
			: base() {
		}
		public MVCxTreeListColumn(string fieldName)
			: base(fieldName) {
		}
		public MVCxTreeListColumn(string fieldName, MVCxTreeListColumnType columnType)
			: this(fieldName) {
			this.columnType = columnType;
		}
		public MVCxTreeListColumn(string fieldName, string caption)
			: base(fieldName, caption) {
		}
		public MVCxTreeListColumn(string fieldName, string caption, MVCxTreeListColumnType columnType)
			: this(fieldName, caption) {
			this.columnType = columnType;
		}
		protected internal MVCxTreeListColumn(ModelMetadata metadata)
			: base() {
			Metadata = metadata;
		}
		public MVCxTreeListColumnType ColumnType {
			get { return columnType; }
			set {
				if (columnType == value) return;
				columnType = value;
				ResetEditProperties();
				ExtensionsHelper.ConfigureEditPropertiesByMetadata(PropertiesEdit, Metadata);
			}
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
			set {
				if (metadata == value) return;
				metadata = value;
				Caption = Metadata.DisplayName;
				ReadOnly = Metadata.IsReadOnly;
				Visible = Metadata.ShowForDisplay;
				ExtensionsHelper.ConfigureEditPropertiesByMetadata(PropertiesEdit, Metadata);
			} 
		}
		protected internal string HeaderCaptionTemplateContent { get; set; }
		protected internal Action<TreeListHeaderTemplateContainer> HeaderCaptionTemplateContentMethod { get; set; }
		protected internal string GroupFooterCellTemplateContent { get; set; }
		protected internal Action<TreeListFooterCellTemplateContainer> GroupFooterCellTemplateContentMethod { get; set; }
		protected internal string FooterCellTemplateContent { get; set; }
		protected internal Action<TreeListFooterCellTemplateContainer> FooterCellTemplateContentMethod { get; set; }
		protected internal string DataCellTemplateContent { get; set; }
		protected internal Action<TreeListDataCellTemplateContainer> DataCellTemplateContentMethod { get; set; }
		protected internal string EditCellTemplateContent { get; set; }
		protected internal Action<TreeListEditCellTemplateContainer> EditCellTemplateContentMethod { get; set; }
		public void SetHeaderCaptionTemplateContent(string content) {
			HeaderCaptionTemplateContent = content;
		}
		public void SetHeaderCaptionTemplateContent(Action<TreeListHeaderTemplateContainer> contentMethod) {
			HeaderCaptionTemplateContentMethod = contentMethod;
		}
		public void SetGroupFooterCellTemplateContent(string content) {
			GroupFooterCellTemplateContent = content;
		}
		public void SetGroupFooterCellTemplateContent(Action<TreeListFooterCellTemplateContainer> contentMethod) {
			GroupFooterCellTemplateContentMethod = contentMethod;
		}
		public void SetFooterCellTemplateContent(string content) {
			FooterCellTemplateContent = content;
		}
		public void SetFooterCellTemplateContent(Action<TreeListFooterCellTemplateContainer> contentMethod) {
			FooterCellTemplateContentMethod = contentMethod;
		}
		public void SetDataCellTemplateContent(string content) {
			DataCellTemplateContent = content;
		}
		public void SetDataCellTemplateContent(Action<TreeListDataCellTemplateContainer> contentMethod) {
			DataCellTemplateContentMethod = contentMethod;
		}
		public void SetEditCellTemplateContent(string content) {
			EditCellTemplateContent = content;
		}
		public void SetEditCellTemplateContent(Action<TreeListEditCellTemplateContainer> contentMethod) {
			EditCellTemplateContentMethod = contentMethod;
		}
		public override void Assign(CollectionItem source) {
			MVCxTreeListColumn column = source as MVCxTreeListColumn;
			if(column != null) {
				ColumnType = column.ColumnType;
			}
			base.Assign(source);
		}
		protected override EditPropertiesBase CreateEditProperties() {
			switch(ColumnType) {
				case MVCxTreeListColumnType.TextBox:
					return new TextBoxProperties(this);
				case MVCxTreeListColumnType.ButtonEdit:
					return new ButtonEditProperties(this);
				case MVCxTreeListColumnType.CheckBox:
					return new CheckBoxProperties(this);
				case MVCxTreeListColumnType.ComboBox:
					return new ComboBoxProperties(this);
				case MVCxTreeListColumnType.DateEdit:
					return new DateEditProperties(this);
				case MVCxTreeListColumnType.SpinEdit:
					return new SpinEditProperties(this);
				case MVCxTreeListColumnType.TimeEdit:
					return new TimeEditProperties(this);
				case MVCxTreeListColumnType.ColorEdit:
					return new ColorEditProperties(this);
				case MVCxTreeListColumnType.DropDownEdit:
					return new DropDownEditProperties(this);
				case MVCxTreeListColumnType.Memo:
					return new MemoProperties(this);
				case MVCxTreeListColumnType.BinaryImage:
					return new BinaryImageEditProperties(this);
				case MVCxTreeListColumnType.Image:
					return new ImageEditProperties(this);
				case MVCxTreeListColumnType.HyperLink:
					return new HyperLinkProperties(this);
				case MVCxTreeListColumnType.ProgressBar:
					return new ProgressBarProperties(this);
				case MVCxTreeListColumnType.TokenBox:
					return new TokenBoxProperties(this);
				default:
					return new TextBoxProperties(this);
			}
		}
		string IDateEditIDResolver.GetDateEditIdByDataItemName(string dataItemName) {
			MVCxTreeList treeList = TreeList as MVCxTreeList;
			MVCxTreeListColumn targetColumn = treeList.Columns[dataItemName] as MVCxTreeListColumn;
			return targetColumn != null && targetColumn != this && targetColumn.ColumnType == MVCxTreeListColumnType.DateEdit
				? treeList.GetEditorID(targetColumn) : "";
		}
		string[] IDateEditIDResolver.GetPossibleDataItemNames() {
			return null;
		}
	}
	public class MVCxTreeListCommandColumn : TreeListCommandColumn {
		public MVCxTreeListCommandColumn(TreeListSettings settings)
			: base() {
			TreeListSettings = settings;
			Visible = false;
			VisibleIndex = -1;
		}
		protected TreeListSettings TreeListSettings { get; set; }
		public new bool Visible {
			get { return base.Visible; }
			set {
				if(Visible != value) {
					base.Visible = value;
					if(value)
						VisibleIndex = VisibleIndex < 0 ? TreeListSettings.Columns.Count : VisibleIndex;
					else
						VisibleIndex = -1;
				}
			}
		}
		public override int VisibleIndex {
			get { return base.VisibleIndex; }
			set {
				if(VisibleIndex != value) {
					base.VisibleIndex = value;
					Visible = value > -1;
				}
			}
		}
		protected internal string HeaderCaptionTemplateContent { get; set; }
		protected internal Action<TreeListHeaderTemplateContainer> HeaderCaptionTemplateContentMethod { get; set; }
		protected internal string GroupFooterCellTemplateContent { get; set; }
		protected internal Action<TreeListFooterCellTemplateContainer> GroupFooterCellTemplateContentMethod { get; set; }
		protected internal string FooterCellTemplateContent { get; set; }
		protected internal Action<TreeListFooterCellTemplateContainer> FooterCellTemplateContentMethod { get; set; }
		public void SetHeaderCaptionTemplateContent(string content) {
			HeaderCaptionTemplateContent = content;
		}
		public void SetHeaderCaptionTemplateContent(Action<TreeListHeaderTemplateContainer> contentMethod) {
			HeaderCaptionTemplateContentMethod = contentMethod;
		}
		public void SetGroupFooterCellTemplateContent(string content) {
			GroupFooterCellTemplateContent = content;
		}
		public void SetGroupFooterCellTemplateContent(Action<TreeListFooterCellTemplateContainer> contentMethod) {
			GroupFooterCellTemplateContentMethod = contentMethod;
		}
		public void SetFooterCellTemplateContent(string content) {
			FooterCellTemplateContent = content;
		}
		public void SetFooterCellTemplateContent(Action<TreeListFooterCellTemplateContainer> contentMethod) {
			FooterCellTemplateContentMethod = contentMethod;
		}
	}
	public class MVCxTreeListColumnCollection<RowType>: MVCxTreeListColumnCollection {
		public MVCxTreeListColumnCollection() {
		}
		public MVCxTreeListColumnCollection(IWebControlObject owner)
			: base(owner) {
		}
		public MVCxTreeListColumn Add<ValueType>(Expression<Func<RowType, ValueType>> expression, Action<MVCxTreeListColumn> method) {
			MVCxTreeListColumn column = Add(expression);
			if (method != null)
				method(column);
			return column;
		}
		public MVCxTreeListColumn Add<ValueType>(Expression<Func<RowType, ValueType>> expression) {
			ModelMetadata columnMetadata = GetColumnMetadata(expression);
			MVCxTreeListColumn column = new MVCxTreeListColumn(columnMetadata);
			if (expression != null) {
				string fieldName = ExpressionHelper.GetExpressionText(expression);
				column.FieldName = HtmlHelperExtension.HtmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
			}
			Add(column);
			return column;
		}
		static ModelMetadata GetColumnMetadata<ValueType>(Expression<Func<RowType, ValueType>> expression) {
			if (expression == null)
				return null;
			return ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<RowType>());
		}
	}
	public class MVCxTreeListColumnCollection : TreeListColumnCollection {
		public MVCxTreeListColumnCollection()
			: base(null) {
		}
		public MVCxTreeListColumnCollection(IWebControlObject owner)
			: base(owner) {
		}
		public void Add(Action<MVCxTreeListColumn> method) {
			method(Add());
		}
		public MVCxTreeListColumn Add() {
			MVCxTreeListColumn column = new MVCxTreeListColumn();
			Add(column);
			return column;
		}
		public MVCxTreeListColumn Add(string fieldName) {
			return Add(fieldName, string.Empty);
		}
		public MVCxTreeListColumn Add(string fieldName, MVCxTreeListColumnType columnType) {
			return Add(fieldName, string.Empty, columnType);
		}
		public MVCxTreeListColumn Add(string fieldName, string caption) {
			return Add(fieldName, caption, MVCxTreeListColumnType.Default);
		}
		public MVCxTreeListColumn Add(string fieldName, string caption, MVCxTreeListColumnType columnType) {
			MVCxTreeListColumn column = new MVCxTreeListColumn(fieldName, caption, columnType);
			Add(column);
			return column;
		}
		protected override Type GetKnownType() { 
			return typeof(TreeListColumn); 
		}
	}
	public class MVCxTreeListSummaryCollection : TreeListSummaryCollection {
		public MVCxTreeListSummaryCollection() 
			: base(null) {
		}
		public TreeListSummaryItem Add(SummaryItemType summaryType, string fieldName) {
			return AddInternal(new TreeListSummaryItem {
				SummaryType = summaryType, FieldName = fieldName, ShowInColumn = fieldName 
			});
		}
		public void Add(Action<TreeListSummaryItem> method) {
			method(Add());
		}
		public TreeListSummaryItem Add() {
			TreeListSummaryItem summaryItem = new TreeListSummaryItem();
			Add(summaryItem);
			return summaryItem;
		}
	}
}
