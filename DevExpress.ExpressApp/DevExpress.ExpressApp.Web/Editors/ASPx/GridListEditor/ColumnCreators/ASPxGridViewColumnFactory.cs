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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ASPxGridViewColumnFactory : ASPxGridViewColumnFactoryBase {
		internal override List<ColumnInfoWrapper> AddColumns(ModelLayoutElementCollection layoutElementCollection, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode, DataItemTemplateFactoryWrapper templateFactory, bool needCreateEditItemTemplate) {
			List<ColumnInfoWrapper> result = new List<ColumnInfoWrapper>();
			foreach(IModelNode modelColumn in layoutElementCollection) {
				Guard.TypeArgumentIs(typeof(IModelColumn), modelColumn.GetType(), "modelColumn");
				ColumnInfoWrapper columnInfoWrapper = CreateColumnInfoWrapper(modelColumn, gridView, dataAccessMode, templateFactory, needCreateEditItemTemplate);
				gridView.Columns.Add(columnInfoWrapper.Column);
				result.Add(columnInfoWrapper);
			}
			return result;
		}
		protected override void SetupVisibleIndex(ModelLayoutElementCollection layoutElementCollection, List<ColumnInfoWrapper> columnsInfo) {
			Dictionary<string, GridViewColumn> gridColumns = new Dictionary<string, GridViewColumn>(columnsInfo.Count);
			foreach(ColumnInfoWrapper columnInfo in columnsInfo) {
				gridColumns.Add(columnInfo.GridViewColumnInfo.Model.Id, columnInfo.Column);
			}
			List<IModelNode> sortedModelColumns = new List<IModelNode>(layoutElementCollection);
			sortedModelColumns.Sort(new ColumnModelNodesComparer());
			int index = 0;
			foreach(IModelLayoutElement modelColumn in sortedModelColumns) {
				int visibleIndex = -1;
				if(modelColumn.Index.HasValue) {
					if(modelColumn.Index.Value >= 0) {
						visibleIndex = index;
						index++;
					}
					else {
						visibleIndex = -1;
					}
				}
				else {
					visibleIndex = index;
					index++;
				}
				gridColumns[modelColumn.Id].VisibleIndex = visibleIndex;
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class ASPxGridViewColumnFactoryBase {
		private static Dictionary<Type, Type> editor_ColumnCreator_AssociationCollection = new Dictionary<Type, Type>();
		static ASPxGridViewColumnFactoryBase() {
			RegisterColumnCreators();
		}
		public ASPxGridViewColumnFactoryBase() {
		}
		public Dictionary<GridViewColumn, IColumnInfo> PopulateColumns(ModelLayoutElementCollection layoutElementCollection, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode, DataItemTemplateFactoryWrapper templateFactory, bool needCreateEditItemTemplate) {
			List<ColumnInfoWrapper> columnsInfo = AddColumns(layoutElementCollection, gridView, dataAccessMode, templateFactory, needCreateEditItemTemplate);
			Dictionary<GridViewColumn, IColumnInfo> result = new Dictionary<GridViewColumn, IColumnInfo>(columnsInfo.Count);
			foreach(ColumnInfoWrapper columnInfo in columnsInfo) {
				result.Add(columnInfo.Column, columnInfo.GridViewColumnInfo);
			}
			SetupVisibleIndex(layoutElementCollection, columnsInfo);
			return result;
		}
		internal ColumnInfoWrapper AddColumn(IModelColumn modelColumn, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode, DataItemTemplateFactoryWrapper templateFactory, bool needCreateEditItemTemplate) {
			ColumnInfoWrapper result = CreateColumnInfoWrapper(modelColumn, gridView, dataAccessMode, templateFactory, needCreateEditItemTemplate);
			gridView.Columns.Add(result.Column);
			return result;
		}
		internal abstract List<ColumnInfoWrapper> AddColumns(ModelLayoutElementCollection layoutElementCollection, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode, DataItemTemplateFactoryWrapper templateFactory, bool needCreateEditItemTemplate);
		protected abstract void SetupVisibleIndex(ModelLayoutElementCollection layoutElementCollection, List<ColumnInfoWrapper> columnsInfo);
		protected ColumnInfoWrapper CreateColumnInfoWrapper(IModelNode modelNode, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode, DataItemTemplateFactoryWrapper templateFactory, bool needCreateEditItemTemplate) {
			GridViewColumn column = null;
			IColumnInfo gridViewDataColumnInfo = null;
			bool needCustomizeDataItem = true;
			bool needCreateDataItemTemplate = false;
			IModelColumn modelColumn = modelNode as IModelColumn;
			if(CreateCustomGridViewDataColumnCore != null && modelColumn != null) { 
				CreateCustomGridViewDataColumnEventArgs args = new CreateCustomGridViewDataColumnEventArgs(modelColumn);
				CreateCustomGridViewDataColumnCore(this, args);
				column = args.Column;
				gridViewDataColumnInfo = args.GridViewDataColumnInfo;
				if(column != null) {
					Guard.ArgumentNotNull(gridViewDataColumnInfo, "gridViewDataColumnInfo");
					needCustomizeDataItem = false;
				}
			}
			if(column == null) {
				if(CreateCustomGridViewDataColumn != null && modelColumn != null) {
					CreateCustomGridViewDataColumnEventArgs args = new CreateCustomGridViewDataColumnEventArgs(modelColumn);
					CreateCustomGridViewDataColumn(this, args);
					column = args.Column;
					gridViewDataColumnInfo = args.GridViewDataColumnInfo;
				}
				if(column == null) {
					IColumnCreator columnCreator = CreateColumnCreator(modelNode);
					needCreateDataItemTemplate = columnCreator is ASPxDefaultColumnCreator;
					column = columnCreator.CreateGridViewColumn();
				}
				if(gridViewDataColumnInfo == null) {
					gridViewDataColumnInfo = CreateGridViewDataColumnInfo(modelNode, gridView, dataAccessMode);
				}
			}
			gridViewDataColumnInfo.ApplyModel(column);
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if(needCustomizeDataItem && dataColumn != null) {
				OnCustomizeDataItemTemplate(modelColumn, dataColumn, needCreateDataItemTemplate, templateFactory);
				OnCustomizeEditItemTemplate(modelColumn, dataColumn, templateFactory, needCreateEditItemTemplate);
				OnCustomizeGridViewDataColumn(modelColumn, dataColumn);
			}
			return new ColumnInfoWrapper(column, gridViewDataColumnInfo);
		}
		protected virtual IColumnInfo CreateGridViewDataColumnInfo(IModelNode modelColumn, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode) {
			return new GridViewDataColumnInfo((IModelColumn)modelColumn, gridView, dataAccessMode);
		}
		private void OnCustomizeEditItemTemplate(IModelColumn modelColumn, GridViewDataColumn column, DataItemTemplateFactoryWrapper templateFactory, bool needCreateEditItemTemplate) {
			if(column.EditItemTemplate == null && needCreateEditItemTemplate) {
				column.ReadOnly = false;
				bool handled = false;
				if(CreateCustomEditItemTemplate != null) {
					CustomizeDataItemTemplateEventArgs args = new CustomizeDataItemTemplateEventArgs(modelColumn, column);
					CreateCustomEditItemTemplate(this, args);
					handled = args.Handled;
				}
				if(column.EditItemTemplate == null && !handled && !string.IsNullOrEmpty(column.FieldName) && templateFactory != null) {
					column.EditItemTemplate = templateFactory.CreateColumnTemplate(modelColumn, ViewEditMode.Edit);
				}
			}
		}
		private void OnCustomizeDataItemTemplate(IModelColumn modelColumn, GridViewDataColumn column, bool needCreateDataItemTemplate, DataItemTemplateFactoryWrapper templateFactory) {
			if(column.DataItemTemplate == null) {
				bool handled = false;
				bool createDefaultDataItemTemplate = false;
				if(CreateCustomDataItemTemplate != null) {
					CustomizeDataItemTemplateEventArgs args = new CustomizeDataItemTemplateEventArgs(modelColumn, column);
					CreateCustomDataItemTemplate(this, args);
					handled = args.Handled;
					createDefaultDataItemTemplate = args.CreateDefaultDataItemTemplate;
				}
				if(column.DataItemTemplate == null && !string.IsNullOrEmpty(column.FieldName) 
					&& (needCreateDataItemTemplate || createDefaultDataItemTemplate)
					&& !handled && templateFactory != null) {
				   column.DataItemTemplate = templateFactory.CreateColumnTemplate(modelColumn, ViewEditMode.View);
				}
			}
		}
		private void OnCustomizeGridViewDataColumn(IModelColumn modelColumn, GridViewDataColumn column) {
			if(CustomizeGridViewDataColumn != null) {
				CustomizeGridViewDataColumnEventArgs args = new CustomizeGridViewDataColumnEventArgs(column, modelColumn);
				CustomizeGridViewDataColumn(this, args);
			}
		}
		internal IColumnCreator CreateColumnCreator(IModelNode modelNode) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Type columnCreatorType = FindColumnCreatorType(modelNode);
			Guard.ArgumentNotNull(columnCreatorType, "columnCreatorType");
			return (IColumnCreator)TypeHelper.CreateInstance(columnCreatorType, modelNode);
		}
		protected virtual Type FindColumnCreatorType(IModelNode modelNode) {
			if(modelNode is IModelColumn) {
				return GetColumnCreatorType(((IModelColumn)modelNode).PropertyEditorType);
			}
			return null;
		}
		private static void RegisterColumnCreators() {
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxEnumPropertyEditor), typeof(ASPxEnumColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxByteArrayPropertyEditor), typeof(ASPxStringColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxBytePropertyEditor), typeof(ASPxIntColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxIntPropertyEditor), typeof(ASPxIntColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxInt64PropertyEditor), typeof(ASPxIntColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxDoublePropertyEditor), typeof(ASPxIntColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxFloatPropertyEditor), typeof(ASPxIntColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxDecimalPropertyEditor), typeof(ASPxIntColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxStringPropertyEditor), typeof(ASPxStringColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxBooleanPropertyEditor), typeof(ASPxBooleanColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxDateTimePropertyEditor), typeof(ASPxDateTimeColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxColorPropertyEditor), typeof(ASPxColorColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxLookupPropertyEditor), typeof(ASPxStringColumnCreator));
			editor_ColumnCreator_AssociationCollection.Add(typeof(ASPxObjectPropertyEditor), typeof(ASPxStringColumnCreator));
		}
		private Type GetColumnCreatorType(Type propertyEditorType) {
			Type result = null;
			if(propertyEditorType != null) {
				if(!editor_ColumnCreator_AssociationCollection.TryGetValue(propertyEditorType, out result)) {
					result = typeof(ASPxDefaultColumnCreator);
				}
			}
			else {
				result = typeof(ASPxDefaultColumnCreator);
			}
			return result;
		}
		public event EventHandler<CreateCustomGridViewDataColumnEventArgs> CreateCustomGridViewDataColumn;
		internal event EventHandler<CreateCustomGridViewDataColumnEventArgs> CreateCustomGridViewDataColumnCore;
		public event EventHandler<CustomizeGridViewDataColumnEventArgs> CustomizeGridViewDataColumn;
		public event EventHandler<CustomizeDataItemTemplateEventArgs> CreateCustomDataItemTemplate;
		public event EventHandler<CustomizeDataItemTemplateEventArgs> CreateCustomEditItemTemplate;
	}
}
