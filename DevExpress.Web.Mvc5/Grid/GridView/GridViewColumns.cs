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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.Internal;
	public enum MVCxGridViewColumnType { Default, TextBox, ButtonEdit, CheckBox, ComboBox, DateEdit, SpinEdit, TimeEdit, ColorEdit, DropDownEdit, Memo, 
		BinaryImage, Image, HyperLink, ProgressBar, TokenBox }
	public class MVCxGridViewColumn : GridViewEditDataColumn, IDateEditIDResolver {
		public MVCxGridViewColumn()
			: base() {
		}
		public MVCxGridViewColumn(string fieldName)
			: base(fieldName) {
		}
		public MVCxGridViewColumn(string fieldName, MVCxGridViewColumnType columnType)
			: this(fieldName) {
			ColumnType = columnType;
		}
		public MVCxGridViewColumn(string fieldName, string caption)
			: base(fieldName, caption) {
		}
		public MVCxGridViewColumn(string fieldName, string caption, MVCxGridViewColumnType columnType)
			: this(fieldName, caption) {
			ColumnType = columnType;
		}
		protected internal MVCxGridViewColumn(ModelMetadata metadata) {
			ColumnAdapter.Metadata = metadata;
		}
		public MVCxGridViewColumnType ColumnType {
			get { return (MVCxGridViewColumnType)ColumnAdapter.ColumnType; }
			set { ColumnAdapter.ColumnType = (int)value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string FilterExpression { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShowInFilterControl { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxGridView Grid { get { return (MVCxGridView)base.Grid; } }
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<GridViewHeaderTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string HeaderCaptionTemplateContent { get; set; }
		protected internal Action<GridViewHeaderTemplateContainer> HeaderCaptionTemplateContentMethod { get; set; }
		protected internal string FooterTemplateContent { get; set; }
		protected internal Action<GridViewFooterCellTemplateContainer> FooterTemplateContentMethod { get; set; }
		protected internal string FilterTemplateContent { get; set; }
		protected internal Action<GridViewFilterCellTemplateContainer> FilterTemplateContentMethod { get; set; }
		protected internal string DataItemTemplateContent { get; set; }
		protected internal Action<GridViewDataItemTemplateContainer> DataItemTemplateContentMethod { get; set; }
		protected internal string EditItemTemplateContent { get; set; }
		protected internal Action<GridViewEditItemTemplateContainer> EditItemTemplateContentMethod { get; set; }
		protected internal string GroupRowTemplateContent { get; set; }
		protected internal Action<GridViewGroupRowTemplateContainer> GroupRowTemplateContentMethod { get; set; }
		public void SetHeaderTemplateContent(Action<GridViewHeaderTemplateContainer> contentMethod) {
			HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			HeaderTemplateContent = content;
		}
		public void SetHeaderCaptionTemplateContent(Action<GridViewHeaderTemplateContainer> contentMethod) {
			HeaderCaptionTemplateContentMethod = contentMethod;
		}
		public void SetHeaderCaptionTemplateContent(string content) {
			HeaderCaptionTemplateContent = content;
		}
		public void SetFooterTemplateContent(Action<GridViewFooterCellTemplateContainer> contentMethod) {
			FooterTemplateContentMethod = contentMethod;
		}
		public void SetFooterTemplateContent(string content) {
			FooterTemplateContent = content;
		}
		public void SetFilterTemplateContent(Action<GridViewFilterCellTemplateContainer> contentMethod) {
			FilterTemplateContentMethod = contentMethod;
		}
		public void SetFilterTemplateContent(string content) {
			FilterTemplateContent = content;
		}
		public void SetDataItemTemplateContent(Action<GridViewDataItemTemplateContainer> contentMethod) {
			DataItemTemplateContentMethod = contentMethod;
		}
		public void SetDataItemTemplateContent(string content) {
			DataItemTemplateContent = content;
		}
		public void SetEditItemTemplateContent(Action<GridViewEditItemTemplateContainer> contentMethod) {
			EditItemTemplateContentMethod = contentMethod;
		}
		public void SetEditItemTemplateContent(string content) {
			EditItemTemplateContent = content;
		}
		public void SetGroupRowTemplateContent(Action<GridViewGroupRowTemplateContainer> contentMethod) {
			GroupRowTemplateContentMethod = contentMethod;
		}
		public void SetGroupRowTemplateContent(string content) {
			GroupRowTemplateContent = content;
		}
		public override void Assign(CollectionItem source) {
			MVCxGridViewColumn column = source as MVCxGridViewColumn;
			if (column != null) {
				HeaderTemplateContent = column.HeaderTemplateContent;
				HeaderTemplateContentMethod = column.HeaderTemplateContentMethod;
				HeaderCaptionTemplateContent = column.HeaderCaptionTemplateContent;
				FooterTemplateContent = column.FooterTemplateContent;
				FooterTemplateContentMethod = column.FooterTemplateContentMethod;
				FilterTemplateContent = column.FilterTemplateContent;
				FilterTemplateContentMethod = column.FilterTemplateContentMethod;
				DataItemTemplateContent = column.DataItemTemplateContent;
				DataItemTemplateContentMethod = column.DataItemTemplateContentMethod;
				EditItemTemplateContent = column.EditItemTemplateContent;
				EditItemTemplateContentMethod = column.EditItemTemplateContentMethod;
				GroupRowTemplateContent = column.GroupRowTemplateContent;
				GroupRowTemplateContentMethod = column.GroupRowTemplateContentMethod;
			}
			base.Assign(source);
		}
		protected internal virtual void AssignState(GridBaseColumnState source) {
			if (source == null)
				return;
			SortOrder = source.SortOrder;
			SortIndex = source.SortIndex;
			GroupIndex = source.GroupIndex;
			Settings.AllowFilterBySearchPanel = source.AllowFilterBySearchPanel;
			if(Grid != null && Grid.IsCallback || !string.IsNullOrEmpty(source.FilterExpression))
				Settings.AutoFilterCondition = source.AutoFilterCondition;
		}
		protected internal override Type GetDataType() {
			Type type = base.GetDataType();
			if(type == typeof(object) && Grid != null && Grid.EnableCustomOperations) {
				GridBaseColumnState columnState = Grid.CustomOperationViewModel.Columns
					.Where(c => c.FieldName == FieldName)
					.SingleOrDefault();
				if(columnState != null && columnState.DataType != null)
					type = columnState.DataType;
			}
			return type;
		}
		string IDateEditIDResolver.GetDateEditIdByDataItemName(string dataItemName) {
			MVCxGridViewColumn targetColumn = Grid.Columns[dataItemName] as MVCxGridViewColumn;
			return targetColumn != null && targetColumn != this && targetColumn.ColumnType == MVCxGridViewColumnType.DateEdit 
				? Grid.RenderHelper.GetEditorId(targetColumn) : "";
		}
		string[] IDateEditIDResolver.GetPossibleDataItemNames() {
			return null;
		}
		protected internal new MVCxGridDataColumnAdapter ColumnAdapter { get { return (MVCxGridDataColumnAdapter)base.ColumnAdapter; } }
		protected override GridDataColumnAdapter CreateColumnAdapter() {
			return new MVCxGridDataColumnAdapter(this);
		}
	}
	public class MVCxGridViewBandColumn<RowType>: MVCxGridViewBandColumn {
		public MVCxGridViewBandColumn(string caption)
			: base(caption) {
		}
		public MVCxGridViewBandColumn()
			: base() {
		}
		public new MVCxGridViewColumnCollection<RowType> Columns { get { return (MVCxGridViewColumnCollection<RowType>)base.Columns; } }
		protected override GridViewColumnCollection CreateColumnCollection() {
			return new MVCxGridViewColumnCollection<RowType>(this);
		}
	}
	public class MVCxGridViewBandColumn : GridViewBandColumn {
		public MVCxGridViewBandColumn(string caption)
			: base(caption) {
		}
		public MVCxGridViewBandColumn()
			: base() {
		}
		public new MVCxGridViewColumnCollection Columns { get { return (MVCxGridViewColumnCollection)base.Columns; } }
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<GridViewHeaderTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string HeaderCaptionTemplateContent { get; set; }
		protected internal Action<GridViewHeaderTemplateContainer> HeaderCaptionTemplateContentMethod { get; set; }
		public void SetHeaderTemplateContent(Action<GridViewHeaderTemplateContainer> contentMethod) {
			HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			HeaderTemplateContent = content;
		}
		public void SetHeaderCaptionTemplateContent(Action<GridViewHeaderTemplateContainer> contentMethod) {
			HeaderCaptionTemplateContentMethod = contentMethod;
		}
		public void SetHeaderCaptionTemplateContent(string content) {
			HeaderCaptionTemplateContent = content;
		}
		protected override GridViewColumnCollection CreateColumnCollection() {
			return new MVCxGridViewColumnCollection(this);
		}
	}
	public class MVCxGridViewCommandColumn : GridViewCommandColumn {
		public MVCxGridViewCommandColumn()
			: base() {
			Visible = false;
			VisibleIndex = -1;
		}
		public new bool Visible {
			get { return base.Visible; }
			set {
				if(Visible != value) {
					base.Visible = value;
					if(value)
						VisibleIndex = VisibleIndex < 0 ? 0 : VisibleIndex;
					else
						VisibleIndex = -1;
				}
			}
		}
		public override int VisibleIndex {
			get { return base.VisibleIndex; }
			set {
				base.VisibleIndex = value;
				Visible = value > -1;
			}
		}
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<GridViewHeaderTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string HeaderCaptionTemplateContent { get; set; }
		protected internal Action<GridViewHeaderTemplateContainer> HeaderCaptionTemplateContentMethod { get; set; }
		protected internal string FooterTemplateContent { get; set; }
		protected internal Action<GridViewFooterCellTemplateContainer> FooterTemplateContentMethod { get; set; }
		protected internal string FilterTemplateContent { get; set; }
		protected internal Action<GridViewFilterCellTemplateContainer> FilterTemplateContentMethod { get; set; }
		public void SetHeaderTemplateContent(Action<GridViewHeaderTemplateContainer> contentMethod) {
			HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			HeaderTemplateContent = content;
		}
		public void SetHeaderCaptionTemplateContent(Action<GridViewHeaderTemplateContainer> contentMethod) {
			HeaderCaptionTemplateContentMethod = contentMethod;
		}
		public void SetHeaderCaptionTemplateContent(string content) {
			HeaderCaptionTemplateContent = content;
		}
		public void SetFooterTemplateContent(Action<GridViewFooterCellTemplateContainer> contentMethod) {
			FooterTemplateContentMethod = contentMethod;
		}
		public void SetFooterTemplateContent(string content) {
			FooterTemplateContent = content;
		}
	}
	public class MVCxGridViewColumnCollection<RowType>: MVCxGridViewColumnCollection {
		public MVCxGridViewColumnCollection()
			: base() {
		}
		public MVCxGridViewColumnCollection(IWebControlObject owner)
			: base(owner) {
		}
		public MVCxGridViewColumn Add<ValueType>(Expression<Func<RowType, ValueType>> expression, Action<MVCxGridViewColumn> method) {
			MVCxGridViewColumn column = Add(expression);
			if (method != null)
				method(column);
			return column;
		}
		public MVCxGridViewColumn Add<ValueType>(Expression<Func<RowType, ValueType>> expression) {
			ModelMetadata columnMetadata = GetColumnMetadata(expression);
			MVCxGridViewColumn column = new MVCxGridViewColumn(columnMetadata);
			if (expression != null) {
				string fieldName = ExpressionHelper.GetExpressionText(expression);
				column.FieldName = HtmlHelperExtension.HtmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
			}
			Add(column);
			return column;
		}
		public new MVCxGridViewBandColumn<RowType> AddBand() {
			var column = new MVCxGridViewBandColumn<RowType>();
			Add(column);
			return column;
		}
		public new MVCxGridViewBandColumn<RowType> AddBand(string caption) {
			var column = new MVCxGridViewBandColumn<RowType>(caption);
			Add(column);
			return column;
		}
		public MVCxGridViewBandColumn<RowType> AddBand<ValueType>(Expression<Func<RowType, ValueType>> expression, Action<MVCxGridViewBandColumn<RowType>> method) {
			MVCxGridViewBandColumn<RowType> column = AddBand(expression);
			if (method != null)
				method(column);
			return column;
		}
		public MVCxGridViewBandColumn<RowType> AddBand<ValueType>(Expression<Func<RowType, ValueType>> expression) {
			MVCxGridViewBandColumn<RowType> column = AddBand();
			if (expression != null) {
				ModelMetadata columnMetadata = GetColumnMetadata(expression);
				column.Caption = columnMetadata.DisplayName;
				column.Visible = columnMetadata.ShowForDisplay;
			}
			return column;
		}
		static ModelMetadata GetColumnMetadata<ValueType>(Expression<Func<RowType, ValueType>> expression) {
			if (expression == null)
				return null;
			return ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<RowType>());
		}
	}
	public class MVCxGridViewColumnCollection : GridViewColumnCollection {
		public MVCxGridViewColumnCollection()
			: base(null) {
		}
		public MVCxGridViewColumnCollection(IWebControlObject owner)
			: base(owner) {
		}
		public void Add(Action<MVCxGridViewColumn> method) {
			method(Add());
		}
		public MVCxGridViewColumn Add() {
			MVCxGridViewColumn column = new MVCxGridViewColumn();
			Add(column);
			return column;
		}
		public MVCxGridViewColumn Add(string fieldName) {
			return Add(fieldName, string.Empty);
		}
		public MVCxGridViewColumn Add(string fieldName, MVCxGridViewColumnType columnType) {
			return Add(fieldName, string.Empty, columnType);
		}
		public MVCxGridViewColumn Add(string fieldName, string caption) {
			return Add(fieldName, caption, MVCxGridViewColumnType.Default);
		}
		public MVCxGridViewColumn Add(string fieldName, string caption, MVCxGridViewColumnType columnType) {
			MVCxGridViewColumn column = new MVCxGridViewColumn(fieldName, caption, columnType);
			Add(column);
			return column;
		}
		public void AddBand(Action<MVCxGridViewBandColumn> method) {
			method(AddBand());
		}
		public MVCxGridViewBandColumn AddBand() {
			MVCxGridViewBandColumn column = new MVCxGridViewBandColumn();
			Add(column);
			return column;
		}
		public MVCxGridViewBandColumn AddBand(string caption) {
			MVCxGridViewBandColumn column = new MVCxGridViewBandColumn(caption);
			Add(column);
			return column;
		}
		protected override Type GetKnownType() {
			return typeof(GridViewColumn);
		}
	}
}
