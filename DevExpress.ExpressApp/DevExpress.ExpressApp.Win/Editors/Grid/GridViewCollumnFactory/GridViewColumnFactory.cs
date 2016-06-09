#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class GridViewColumnFactory {
		public IList<GridColumn> PopulateColumns(ModelLayoutElementCollection layoutElementCollection, ColumnView columnView, IRepositoryItemCreator repositoryItemCreator, IGridViewOptions editorOptions, IModelSynchronizersHolder modelSynchronizersHolder) {
			Guard.ArgumentNotNull(layoutElementCollection, "layoutElementCollection");
			Guard.ArgumentNotNull(columnView, "columnView");
			Guard.ArgumentNotNull(editorOptions, "editorOptions");
			Dictionary<string, ColumnDataContainer> existingColumns = new Dictionary<string, ColumnDataContainer>();
			List<GridColumn> toDelete = new List<GridColumn>();
			if(modelSynchronizersHolder != null) {
				foreach(GridColumn column in columnView.Columns) {
					IGridColumnModelSynchronizer gridColumnInfo = modelSynchronizersHolder.GetSynchronizer(column) as IGridColumnModelSynchronizer;
					if(gridColumnInfo != null) {
						existingColumns.Add(gridColumnInfo.Model.Id, new ColumnDataContainer(column, gridColumnInfo));
						toDelete.Add(column);
					}
				}
			}
			IList<GridColumn> createdColumns = new List<GridColumn>();
			foreach(IModelColumn columnInfo in layoutElementCollection.GetItems<IModelColumn>()) {
				if(columnInfo != null && layoutElementCollection[columnInfo.Id] != null) {
					ColumnDataContainer gridColumn = null;
					if(existingColumns.TryGetValue(columnInfo.Id, out gridColumn)) {
						gridColumn.GridColumnModelSynchronizer.ApplyModel(gridColumn.Column);
					}
					else {
						gridColumn = AddColumn(columnInfo, columnView, repositoryItemCreator, editorOptions, modelSynchronizersHolder);
						createdColumns.Add(gridColumn.Column);
						existingColumns.Add(columnInfo.Id, gridColumn);
					}
					toDelete.Remove(gridColumn.Column);
				}
			}
			foreach(GridColumn gridColumn in toDelete) {
				RemoveColumn(gridColumn, layoutElementCollection, columnView, modelSynchronizersHolder);
			}
			SetSortIndex(layoutElementCollection, existingColumns);
			SetupColumns(columnView, layoutElementCollection, existingColumns, modelSynchronizersHolder);
			return createdColumns;
		}
		internal ColumnDataContainer AddColumn(IModelColumn columnInfo, ColumnView columnView, IRepositoryItemCreator repositoryItemCreator, IGridViewOptions editorOptions, IModelSynchronizersHolder modelSynchronizersHolder) {
			Guard.ArgumentNotNull(columnInfo, "columnInfo");
			Guard.ArgumentNotNull(columnView, "columnView");
			Guard.ArgumentNotNull(editorOptions, "editorOptions");
			ColumnDataContainer columnDataContainer = CreateColumnCore(columnInfo, columnView, editorOptions);
			SetupColumn(columnDataContainer.Column, columnDataContainer.GridColumnModelSynchronizer, columnView, repositoryItemCreator, editorOptions);
			if(CustomizeGridColumn != null) {
				CustomizeGridColumnEventArgs args = new CustomizeGridColumnEventArgs(columnDataContainer.Column, columnDataContainer.GridColumnModelSynchronizer);
				CustomizeGridColumn(this, args);
			}
			if(modelSynchronizersHolder != null) {
				modelSynchronizersHolder.RegisterSynchronizer(columnDataContainer.Column, columnDataContainer.GridColumnModelSynchronizer);
			}
			return columnDataContainer;
		}
		private void SetupColumns(ColumnView columnView, ModelLayoutElementCollection layoutElementCollection, Dictionary<string, ColumnDataContainer> presentedColumns, IModelSynchronizersHolder modelSynchronizersHolder) {
			Dictionary<GridColumn, IGridColumnModelSynchronizer> allGridColumns = new Dictionary<GridColumn, IGridColumnModelSynchronizer>();
			foreach(KeyValuePair<string, ColumnDataContainer> item in presentedColumns) {
				allGridColumns[item.Value.Column] = item.Value.GridColumnModelSynchronizer;
			}
			SetupColumns(columnView, layoutElementCollection, allGridColumns, modelSynchronizersHolder);
		}
		protected virtual void SetupColumns(ColumnView columnView, ModelLayoutElementCollection layoutElementCollection, Dictionary<GridColumn, IGridColumnModelSynchronizer> gridColumns, IModelSynchronizersHolder modelSynchronizersHolder) {
			SetVisibleIndex(layoutElementCollection, gridColumns);
		}
		private void SetSortIndex(ModelLayoutElementCollection layoutElementCollection, Dictionary<string, ColumnDataContainer> presentedColumns) {
			List<IModelColumn> sortedBySortIndex = new List<IModelColumn>(layoutElementCollection.GetItems<IModelColumn>());
			sortedBySortIndex.Sort(new Comparison<IModelColumn>(ComparisonSortIndex));
			foreach(IModelColumn column in sortedBySortIndex) {
				if(column.SortIndex != -1) {
					ColumnDataContainer columnDataContainer = presentedColumns[column.Id];
					if(!columnDataContainer.GridColumnModelSynchronizer.IsProtectedContentColumn && columnDataContainer.GridColumnModelSynchronizer.CreateColumnWrapper(columnDataContainer.Column).AllowSortingChange) {
						columnDataContainer.Column.SortIndex = column.SortIndex;
					}
				}
			}
		}
		protected virtual void SetVisibleIndex(ModelLayoutElementCollection columns, Dictionary<GridColumn, IGridColumnModelSynchronizer> gridColumns) {
			Dictionary<string, GridColumn> columnsById = new Dictionary<string, GridColumn>();
			foreach(KeyValuePair<GridColumn, IGridColumnModelSynchronizer> columnInfo in gridColumns) {
				columnsById.Add(columnInfo.Value.Model.Id, columnInfo.Key);
			}
			List<IModelColumn> list = new List<IModelColumn>(columns.GetItems<IModelColumn>());
			list.Sort(new ColumnModelNodesComparer());
			int index = 0;
			foreach(IModelLayoutElement modelColumn in list) {
				int visibleIndex = -1;
				if(!modelColumn.Index.HasValue || modelColumn.Index.Value >= 0) {
					visibleIndex = index;
					index++;
				}
				GridColumn column;
				if(columnsById.TryGetValue(modelColumn.Id, out column)) {
					column.VisibleIndex = visibleIndex;
				}
			}
		}
		public void RemoveColumn(GridColumn column, ModelLayoutElementCollection columns, ColumnView columnView, IModelSynchronizersHolder modelSynchronizersHolder) {
			if(column != null) {
				if(columnView != null && columnView.Columns.Contains(column)) {
					if(modelSynchronizersHolder != null) {
						IGridColumnModelSynchronizer columnInfo = modelSynchronizersHolder.GetSynchronizer(column) as IGridColumnModelSynchronizer;
						modelSynchronizersHolder.RemoveSynchronizer(column);
						RemoveColumnInfo(columnInfo, columns);
					}
					columnView.Columns.Remove(column);
				}
				else {
					throw new ArgumentException(string.Format(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.GridColumnDoesNotExist), column.FieldName), "FieldName");
				}
			}
		}
		private void RemoveColumnInfo(IGridColumnModelSynchronizer columnInfo, ModelLayoutElementCollection columns) {
			if(columnInfo != null && columnInfo.Model != null) {
				columns.Remove(columnInfo.Model.Id, true);
			}
		}
		private int ComparisonSortIndex(IModelColumn c1, IModelColumn c2) {
			return c1.SortIndex - c2.SortIndex;
		}
		private ColumnDataContainer CreateColumnCore(IModelColumn columnInfo, ColumnView columnView, IGridViewOptions editorOptions) {
			GridColumn column = null;
			IGridColumnModelSynchronizer gridColumnInfo = null;
			if(CreateCustomColumnCore != null) {
				CreateCustomColumnEventArgs args = new CreateCustomColumnEventArgs(columnInfo, editorOptions.ObjectTypeInfo);
				CreateCustomColumnCore(this, args);
				column = args.Column;
				gridColumnInfo = args.GridColumnInfo;
			}
			if(column == null) {
				if(CreateCustomColumn != null) {
					CreateCustomColumnEventArgs args = new CreateCustomColumnEventArgs(columnInfo, editorOptions.ObjectTypeInfo);
					CreateCustomColumn(this, args);
					column = args.Column;
					gridColumnInfo = args.GridColumnInfo;
				}
				if(column == null) {
					column = CreateGridColumn();
				}
			}
			if(gridColumnInfo == null) {
				gridColumnInfo = CreateGridColumnModelSynchronizer(columnInfo, editorOptions.ObjectTypeInfo, editorOptions.IsAsyncServerMode, false);
			}
			columnView.Columns.Add(column); 
			return new ColumnDataContainer(column, gridColumnInfo);
		}
		protected virtual GridColumn CreateGridColumn() {
			return new GridColumn();
		}
		public virtual GridColumnModelSynchronizer CreateGridColumnModelSynchronizer(IModelColumn modelColumn, ITypeInfo objectTypeInfo, bool isAsyncServerMode, bool isProtectedColumn) {
			return new GridColumnModelSynchronizer(modelColumn, objectTypeInfo, isAsyncServerMode, isProtectedColumn);
		}
		private void SetupColumn(GridColumn column, IGridColumnModelSynchronizer gridColumnInfo, ColumnView columnView, IRepositoryItemCreator repositoryItemCreator, IGridViewOptions editorOptions) {
			SetMemberDependentProperties(column, gridColumnInfo);
			CreateColumnRepositoryItem(column, gridColumnInfo, columnView, repositoryItemCreator, editorOptions);
			column.FieldName = gridColumnInfo.FieldName;
			column.Name = gridColumnInfo.Model.Id;
			if(IsReadOnlyColumn(column, gridColumnInfo)) {
				column.OptionsColumn.AllowEdit = false;
				column.OptionsColumn.TabStop = false;
			}
			string toolTip = gridColumnInfo.Model.ToolTip;
			if(!string.IsNullOrEmpty(toolTip)) {
				column.ToolTip = toolTip;
			}
			gridColumnInfo.ApplyModel(column);
		}
		private bool IsReadOnlyColumn(GridColumn column, IGridColumnModelSynchronizer gridColumnInfo) {
			return gridColumnInfo.MemberInfo != null && (column.ColumnEdit == null) && !gridColumnInfo.MemberInfo.IsList;
		}
		private void CreateColumnRepositoryItem(GridColumn column, IGridColumnModelSynchronizer gridColumnInfo, ColumnView columnView, IRepositoryItemCreator repositoryItemCreator, IGridViewOptions editorOptions) {
			RepositoryItem repositoryItem = TryCreateRepositoryItem(gridColumnInfo, repositoryItemCreator);
			if(repositoryItem != null) {
				column.ColumnEdit = repositoryItem;
				column.AppearanceCell.Options.UseTextOptions = true;
				column.AppearanceCell.TextOptions.HAlignment = WinAlignmentProvider.GetAlignment(gridColumnInfo.MemberInfo.MemberType);
				if((gridColumnInfo.DataAccessMode == CollectionSourceDataAccessMode.Server) || ((gridColumnInfo.ModelMember != null) && gridColumnInfo.ModelMember.Type.IsInterface && !typeof(IComparable).IsAssignableFrom(gridColumnInfo.MemberInfo.MemberType))) {
					column.FieldNameSortGroup = new ObjectEditorHelperBase(XafTypesInfo.Instance.FindTypeInfo(gridColumnInfo.ModelMember.Type), gridColumnInfo.Model).GetFullDisplayMemberName(gridColumnInfo.PropertyName);
				}
				SetupColumnByRepositoryItem(column, columnView, repositoryItem, editorOptions);
				OnCustomizeRepositoryItem(repositoryItem, gridColumnInfo.Model);
			}
		}
		private void SetupColumnByRepositoryItem(GridColumn column, ColumnView columnView, RepositoryItem repositoryItem, IGridViewOptions editorOptions) {
			column.OptionsColumn.AllowEdit = IsDataShownOnDropDownWindow(repositoryItem) ? true : editorOptions.AllowEdit; 
			if(editorOptions.AllowEdit) { 
				column.OptionsColumn.TabStop = !repositoryItem.ReadOnly;
			}
			if(repositoryItem is ILookupEditRepositoryItem) {
				column.FilterMode = editorOptions.LookupColumnFilterMode;
				if(editorOptions.LookupColumnFilterMode == ColumnFilterMode.Value) {
					column.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Equals;
					column.OptionsFilter.FilterBySortField = DefaultBoolean.False;
				}
				else {
					column.OptionsFilter.FilterBySortField = DefaultBoolean.True;
				}
			}
			if(repositoryItem is RepositoryItemMemoExEdit) {
				column.OptionsColumn.AllowGroup = column.OptionsColumn.AllowSort = DefaultBoolean.True;
			}
			else {
				if((repositoryItem is RepositoryItemPictureEdit) && (((RepositoryItemPictureEdit)repositoryItem).CustomHeight > 0)) {
					if(columnView is GridView) {
						((GridView)columnView).OptionsView.RowAutoHeight = true;
					}
				}
				else {
					if(repositoryItem is RepositoryItemRtfEditEx) {
						column.FilterMode = ColumnFilterMode.DisplayText;
					}
				}
			}
		}
		private RepositoryItem TryCreateRepositoryItem(IGridColumnModelSynchronizer gridColumnInfo, IRepositoryItemCreator repositoryItemCreator) {
			RepositoryItem result = null;
			if(CreateCustomRepositoryItem != null) {
				CreateCustomRepositoryItemEventArgs args = new CreateCustomRepositoryItemEventArgs(gridColumnInfo);
				CreateCustomRepositoryItem(this, args);
				result = args.RepositoryItem;
			}
			if(result == null && repositoryItemCreator != null) {
				result = repositoryItemCreator.CreateItem(gridColumnInfo);
			}
			return result;
		}
		private void SetMemberDependentProperties(GridColumn column, IGridColumnModelSynchronizer gridColumnInfo) {
			IMemberInfo memberInfo = gridColumnInfo.MemberInfo;
			if(memberInfo != null) {
				if(memberInfo.MemberType.IsEnum) {
					column.SortMode = ColumnSortMode.Value;
				}
				else {
					if(ModelNodesGeneratorBase.IsBinaryImage(memberInfo)) {
						column.OptionsColumn.AllowSort = DefaultBoolean.False;
						column.OptionsFilter.AllowFilter = false;
					}
					else {
						if(!SimpleTypes.IsSimpleType(memberInfo.MemberType)) {
							column.SortMode = ColumnSortMode.DisplayText;
							column.OptionsColumn.AllowSort = DefaultBoolean.True;
							column.OptionsColumn.AllowGroup = DefaultBoolean.True;
						}
					}
				}
				if((SimpleTypes.IsClass(memberInfo.MemberType) || memberInfo.MemberType.IsInterface)) {
					column.FilterMode = ColumnFilterMode.DisplayText;
				}
				else {
					column.FilterMode = ColumnFilterMode.Value;
				}
			}
		}
		private void OnCustomizeRepositoryItem(RepositoryItem repositoryItem, IModelColumn columnInfo) {
			if(CustomizeRepositoryItem != null) {
				CustomizeRepositoryItem(this, new CustomizeRepositoryItemEventArgs(repositoryItem, columnInfo));
			}
		}
		public event EventHandler<CreateCustomColumnEventArgs> CreateCustomColumn;
		internal event EventHandler<CreateCustomColumnEventArgs> CreateCustomColumnCore;
		public event EventHandler<CustomizeGridColumnEventArgs> CustomizeGridColumn;
		public event EventHandler<CustomizeRepositoryItemEventArgs> CustomizeRepositoryItem;
		public event EventHandler<CreateCustomRepositoryItemEventArgs> CreateCustomRepositoryItem;
		public static bool IsDataShownOnDropDownWindow(RepositoryItem repositoryItem) {
			return DXPropertyEditor.RepositoryItemsTypesWithMandatoryButtons.Contains(repositoryItem.GetType());
		}
	}
}
