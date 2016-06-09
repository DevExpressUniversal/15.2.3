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
using System.Drawing;
using System.Collections;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections.Specialized;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data;
using System.Collections.Generic;
using DevExpress.Data.Helpers;
namespace DevExpress.XtraGrid.FilterEditor {
	public class GridCriteriaHelper {
		public static bool IsAllowFilterEditor(ColumnView view) {
			if(!view.OptionsFilter.AllowFilterEditor) return false;
			GridView gView = view as GridView;
			if(gView != null && !gView.OptionsCustomization.AllowFilter) return false;
			if(gView != null && gView.OptionsFilter.DefaultFilterEditorView != FilterEditorViewMode.Visual) return true;
			if((gView == null || gView.OptionsFilter.FilterEditorAggregateEditing == FilterControlAllowAggregateEditing.No)
				&& !CriteriaToTreeProcessor.IsConvertibleOperator(view.ActiveFilter.Criteria))
				return false;
			foreach(GridColumn gc in view.Columns)
				if(view.IsColumnAllowFilter(gc)) return true;
			return false;
		}
		public static bool IsAllowFilterEditorForColumn(ColumnView view, GridColumn column) {
			if(column == null) return IsAllowFilterEditor(view);
			return IsAllowFilterEditor(view) && view.IsColumnAllowFilter(column);
		}
		[Obsolete("Use view.ShowFilterEditor(defaultColumn) instead")]
		public static void ShowFilterEditor(ColumnView view, GridColumn column) {
			view.ShowFilterEditor(column);
		}
		public static FilterColumn GetFilterColumnByGridColumn(FilterColumnCollection collection, GridColumn column) {
			if(column == null)
				return null;
			column = column.View.GetColumnFieldNameSortGroup(column);
			foreach(FilterColumn fColumn in collection) {
				GridFilterColumn gcfc = fColumn as GridFilterColumn;
				if(gcfc != null && column == gcfc.GridColumn)
					return fColumn;
			}
			FilterColumn result = null;
			foreach(FilterColumn fColumn in collection) {
				if(IsColumnEquals(column, fColumn))
					result = fColumn;
				if(fColumn.ColumnCaption == column.GetTextCaption())
					return result;
			}
			return result;
		}
		static bool IsColumnEquals(GridColumn gColumn, FilterColumn fColumn) {
			if(fColumn.FieldName == gColumn.FieldName ||
				string.Concat(fColumn.FieldName, "!") == gColumn.FieldName) return true;
			return false;
		}
		public static RepositoryItem CreateDisplayTextEditor(GridColumn gridColumn) {
			RepositoryItem edit;
			object[] popupValues = GetFilterPopupValues(gridColumn);
			if(popupValues == null || popupValues.Length < 1) {
				edit = new RepositoryItemTextEdit();
			}
			else {
				RepositoryItemComboBox cmbb = new RepositoryItemComboBox();
				cmbb.TextEditStyle = TextEditStyles.Standard;
				cmbb.Items.AddRange(popupValues);
				edit = cmbb;
			}
			return edit;
		}
		static object[] GetFilterPopupValues(GridColumn column) {
			return column.View.DataController.FilterHelper.GetUniqueColumnValues(column.ColumnHandle, -1, true, false, null);
		}
		[Obsolete("Use CriteriaOperator.TryParse function instead")]
		public static CriteriaOperator ParseFailSafe(string filterString) {
			return CriteriaOperator.TryParse(filterString);
		}
		public static bool IsColumnFiltered(GridColumn col, CriteriaOperator additionalFilter) {
			if(col.FilterInfo.Type != ColumnFilterType.None)
				return true;
			else
				return ColumnsContainedCollector.IsContained(additionalFilter, new OperandProperty(col.FieldName));
		}
		public static bool IsRichTextEditItem(RepositoryItem item) {
			if(item == null) return false;
			return item.GetType().Name.Contains("RichTextEdit");
		}
	}
	public class GridFilterColumn : FilterColumn {
		public readonly GridColumn GridColumn;
		RepositoryItem resolvedEditor;
		Type resolvedType;
		string resolvedCaption;
		Image resolvedImage;
		public GridFilterColumn(GridColumn gridColumn)
			: base() {
			this.GridColumn = gridColumn;
			this.resolvedImage = null;
		}
		public override RepositoryItem ColumnEditor {
			get {
				if(resolvedEditor == null)
					resolvedEditor = CreateRepository();
				return resolvedEditor;
			}
		}
		public override FilterColumnClauseClass ClauseClass {
			get {
				if(GridColumn.GetFilterMode() == ColumnFilterMode.DisplayText) {
					return FilterColumnClauseClass.String;
				} else if(ColumnEditor is RepositoryItemLookUpEditBase || ColumnEditor is RepositoryItemImageComboBox || ColumnEditor is RepositoryItemRadioGroup) {
					return FilterColumnClauseClass.Lookup;
				} else if(ColumnEditor is RepositoryItemPictureEdit || ColumnEditor is RepositoryItemImageEdit || GridColumn.ColumnEditIsSparkLine) {
					return FilterColumnClauseClass.Blob;
				} else if(GridColumn.ColumnType == typeof(string)) {
					return FilterColumnClauseClass.String;
				} else if(GridColumn.ColumnType == typeof(DateTime) || GridColumn.ColumnType == typeof(DateTime?)) {
					return FilterColumnClauseClass.DateTime;
				} else {
					return FilterColumnClauseClass.Generic;
				}
			}
		}
		public override void Dispose() {
			base.Dispose();
			if(resolvedEditor != null) {
				resolvedEditor.Dispose();
				resolvedEditor = null;
			}
		}
		protected virtual RepositoryItem CreateRepository() {
			if(GridColumn.GetFilterMode() == ColumnFilterMode.DisplayText) {
				return GridCriteriaHelper.CreateDisplayTextEditor(GridColumn);
			} else {
				RepositoryItem columnEdit = GridColumn.RealColumnEdit;
				if(columnEdit == null || GridCriteriaHelper.IsRichTextEditItem(columnEdit)) {
					return new RepositoryItemTextEdit();
				} else if(columnEdit is RepositoryItemBaseProgressBar) {
					return new RepositoryItemSpinEdit();
				} else if(columnEdit is RepositoryItemMemoEdit) {
					RepositoryItem result = new RepositoryItemMemoExEdit();
					((RepositoryItemMemoExEdit)result).ShowIcon = false;
					return result;
				} else if(columnEdit is RepositoryItemRadioGroup) {
					RepositoryItemImageComboBox edit = new RepositoryItemImageComboBox();
					foreach(RadioGroupItem item in ((RepositoryItemRadioGroup)columnEdit).Items)
						edit.Items.Add(new ImageComboBoxItem(item.Description, item.Value, -1));
					return edit;
				} else {
					RepositoryItem result = (RepositoryItem)columnEdit.Clone();
					result.Assign(columnEdit);
					result.ResetEvents();
					return result;
				}
			}
		}
		protected virtual string ResolveCaption() {
			string tempCaption = GridColumn.GetTextCaption();
			if(tempCaption == null)
				tempCaption = string.Empty;
			tempCaption = tempCaption.Replace('\n', ' ');
			tempCaption = tempCaption.Replace('\r', ' ');
			tempCaption = tempCaption.Replace('\t', ' ');
			if(tempCaption != GridColumn.GetTextCaption())
				tempCaption.Replace("  ", " ").Replace("  ", " ");
			return tempCaption;
		}
		static Image GetImage(GridColumn column) {
			Image image = column.Image;
			if(image == null)
				image = ImageCollection.GetImageListImage(column.Images, column.ImageIndex);
			return image;
		}
		public override string ColumnCaption {
			get {
				if(resolvedCaption == null) {
					resolvedCaption = ResolveCaption();
				}
				return resolvedCaption;
			}
		}
		public override Type ColumnType {
			get {
				if(resolvedType == null) {
					resolvedType = ResolveType();
				}
				return resolvedType;
			}
		}
		protected virtual Type ResolveType() {
			if(GridColumn.GetFilterMode() == ColumnFilterMode.DisplayText) return typeof(string);
			Type result = GridColumn.ColumnType;
			Type underlyingType = Nullable.GetUnderlyingType(result);
			if(underlyingType != null)
				result = underlyingType;
			return result;
		}
		public override string FieldName {
			get { return GridColumn.FieldName; }
		}
		public override Image Image {
			get {
				if(resolvedImage == null)
					return GetImage(GridColumn);
				return resolvedImage;
			}
		}
		public virtual void SetRepositoryItem(RepositoryItem item) {
			resolvedEditor = item;
		}
		public override void SetColumnEditor(RepositoryItem item) {
			SetRepositoryItem(item);
		}
		public override void SetColumnCaption(string caption) {
			resolvedCaption = caption;
		}
		public override void SetImage(Image image) {
			resolvedImage = image;
		}
		public override bool AllowItemCollectionEditor { get { return true; } }
		public override RepositoryItem CreateItemCollectionEditor() {
			FilterRepositoryItemCheckedComboBoxEdit re = new FilterRepositoryItemCheckedComboBoxEdit();
			if(GridColumn.View == null) return re;
			CheckedColumnFilterPopup popup = new CheckedColumnFilterPopup(GridColumn.View, GridColumn, GridColumn.View.GridControl, null);
			object[] values = GridColumn.View.GetFilterPopupValuesCore(GridColumn, false, null);
			popup.InitData(values);
			for(int i = 0; i < popup.Item.Items.Count; i++) {
				CheckedListBoxItem item = popup.Item.Items[i];
				if(BaseEdit.IsNotLoadedValue(item.Value)) {
					re.Items.Add(item.Value);
					continue;
				}
				FilterItem fItem = item.Value as FilterItem;
				re.Items.Add(fItem.Value, fItem.Text);
			}
			return re;
		}
	}
	public class GridFilterDetailListColumn : FilterColumn  {
		GridDetailInfo detailInfo;
		GridView parentView;
		FilterColumnCollection filterColumns;
		public GridFilterDetailListColumn(GridView parentView, GridDetailInfo detailInfo) {
			this.parentView = parentView;
			this.detailInfo = detailInfo;
		}
		public GridDetailInfo DetailInfo { get { return detailInfo; } }
		public GridView ParentView { get { return parentView; } }
		public override string ColumnCaption { get { return DetailInfo.Caption; } }
		public override RepositoryItem ColumnEditor { get { return null; } }
		public override Type ColumnType { get { return null; } }
		public override string FieldName {  get { return DetailInfo.RelationName; } }
		public override Image Image { get { return null; } }
		public override bool IsList { get { return true; } }
		public override bool HasChildren { get { return true; } }
		public override List<IBoundProperty> Children {
			get {
				if(filterColumns == null) {
					filterColumns = CreateFilterColumns();
				}
				List<IBoundProperty> list = new List<IBoundProperty>();
				if(filterColumns == null) return list;
				foreach(FilterColumn column in filterColumns) {
					list.Add(column);
				}
				return list;
			}
		}
		protected virtual FilterColumnCollection CreateFilterColumns() {
			ColumnView defaultColumnView = GetDefaultView() as ColumnView;
			if(defaultColumnView != null && defaultColumnView.Columns.Count > 0) return new ViewFilterColumnCollection(defaultColumnView);
			IList list = ParentView.DataController.GetDetailList(0, DetailInfo.RelationIndex);
			if(list == null) {
				BaseDataControllerHelper helper = ParentView.DataController.Helper;
				if(helper != null && helper.RelationList != null) {
					list = helper.RelationList.GetDetailList(0, DetailInfo.RelationIndex);
				}
			}
			if(list == null) return null;
			WinFilterTreeNodeModelBase model = new WinFilterTreeNodeModelBase();
			model.AllowAggregateEditing = ParentView.OptionsFilter.FilterEditorAggregateEditing;
			model.SourceControl = list;
			return model.FilterProperties as FilterColumnCollection;
		}
		protected BaseView GetDefaultView() {
			GridLevelNode levelNode = ParentView.GetLevelNode();
			if (levelNode == null) return null;
			return levelNode.GetChildTemplate(DetailInfo.RelationName);
		}
	}
	public class ViewFilterColumnCollection : FilterColumnCollection {
		public readonly ColumnView View;
		public ViewFilterColumnCollection(ColumnView view) {
			this.View = view;
			Fill();
		}
		protected virtual bool IsValidForFilter(GridColumn gc) {
			if(!gc.Visible && !gc.OptionsColumn.ShowInCustomizationForm && !View.IsParentColumnFieldNameSortGroupExist(gc))
				return false;
			if(gc.FilterInfo != null && !ReferenceEquals(gc.FilterInfo.FilterCriteria, null))
				return true;
			return View.IsColumnAllowFilter(gc);
		}
		protected virtual FilterColumn CreateFilterColumn(GridColumn gridColumn) {
			return new GridFilterColumn(gridColumn);
		}
		protected virtual void Fill() {
			if(View == null) return;   
			foreach(GridColumn gc in View.Columns) {
				if(IsValidForFilter(gc) && !View.IsColumnFieldNameSortGroupExist(gc))
					this.Add(CreateFilterColumn(gc));
			}
			AddListColumns();
		}
		public override string GetValueScreenText(OperandProperty property, object value) {
			FilterColumn col = this[property];
			if(col == null)
				return base.GetValueScreenText(property, value);
			GridColumn gcol = View.Columns[col.FieldName];
			if(gcol == null)
				return base.GetValueScreenText(property, value);
			string res = View.GetFilterDisplayTextByColumn(gcol, value);
			if(value is FunctionOperator) {
				string asfunc = FilterControlViewInfo.GetLocalizedFunctionName((value as FunctionOperator).OperatorType, value as FunctionOperator);
				if(asfunc != null)
					res = asfunc;
			}
			return res;	
		}
		void AddListColumns() {
			GridView gridView = View as GridView;
			if(gridView == null || View.OptionsFilter.FilterEditorAggregateEditing == FilterControlAllowAggregateEditing.No) return;
			GridDetailInfo[] details = gridView != null ? gridView.GetDetailInfo(DevExpress.Data.DataController.InvalidRow, true) : null;
			if (details != null) {
				foreach (GridDetailInfo detail in details) {
					if (detail != null) {
						Add(new GridFilterDetailListColumn(gridView, detail));
					}
				}
			} else {
				if (gridView.LevelNode != null && gridView.LevelNode.HasChildren ) {
					int relationIndex = 0;
					foreach (GridLevelNode node in gridView.LevelNode.Nodes) {
						string relationCaption = gridView.GetRelationName(DevExpress.Data.DataController.InvalidRow, relationIndex);
						if (string.IsNullOrEmpty(relationCaption)) {
							if(node.LevelTemplate != null && !string.IsNullOrEmpty(node.LevelTemplate.ViewCaption)) {
								relationCaption = node.LevelTemplate.ViewCaption;
							} else {
								relationCaption = node.RelationName;
							}
						}
						Add(new GridFilterDetailListColumn(gridView, new GridDetailInfo(relationIndex, node.RelationName, relationCaption)));
						relationIndex++;
					}
				}
			}
		}
	}
	[ToolboxItem(false)]
	public class GridFilterControl : FilterControl {
	}
}
