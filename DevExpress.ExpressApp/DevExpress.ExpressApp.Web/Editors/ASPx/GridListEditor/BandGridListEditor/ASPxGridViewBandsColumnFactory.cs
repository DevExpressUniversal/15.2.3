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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ASPxGridViewBandsColumnFactory : ASPxGridViewColumnFactoryBase {
		internal override List<ColumnInfoWrapper> AddColumns(ModelLayoutElementCollection layoutElementCollection, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode, DataItemTemplateFactoryWrapper templateFactory, bool needCreateEditItemTemplate) {
			GridViewColumnCollection parentColumns = gridView.Columns;
			ModelBandLayoutItemCollection childrenLayoutItems = new ModelBandLayoutItemCollection(layoutElementCollection);
			return AddColumns(parentColumns, null, childrenLayoutItems, gridView, dataAccessMode, templateFactory, needCreateEditItemTemplate);
		}
		private List<ColumnInfoWrapper> AddColumns(GridViewColumnCollection parentColumns, IModelNode parent, ModelBandLayoutItemCollection layoutItemCollection, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode, DataItemTemplateFactoryWrapper templateFactory, bool needCreateEditItemTemplate) {
			List<ColumnInfoWrapper> result = new List<ColumnInfoWrapper>();
			foreach(IModelBandedLayoutItem layoutItem in layoutItemCollection.GetChildren(parent)) {
				ColumnInfoWrapper columnInfoWrapper = CreateColumnInfoWrapper(layoutItem, gridView, dataAccessMode, templateFactory, needCreateEditItemTemplate);
				if(layoutItem is IModelBand) {
					result.AddRange(AddColumns(((GridViewBandColumn)columnInfoWrapper.Column).Columns, layoutItem, layoutItemCollection, gridView, dataAccessMode, templateFactory, needCreateEditItemTemplate));
				}
				parentColumns.Add(columnInfoWrapper.Column);
				result.Add(columnInfoWrapper);
			}
			return result;
		}
		protected override IColumnInfo CreateGridViewDataColumnInfo(IModelNode modelColumn, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode) {
			if(modelColumn is IModelBand) {
				return new GridViewBandColumnInfo((IModelBand)modelColumn);
			}
			else {
				return base.CreateGridViewDataColumnInfo(modelColumn, gridView, dataAccessMode);
			}
		}
		protected override Type FindColumnCreatorType(IModelNode modelNode) {
			if(modelNode is IModelBand) {
				return typeof(ASPxBandColumnCreator);
			}
			else {
				return base.FindColumnCreatorType(modelNode);
			}
		}
		protected override void SetupVisibleIndex(ModelLayoutElementCollection layoutElementCollection, List<ColumnInfoWrapper> columnsInfo) {
			Dictionary<string, GridViewColumn> gridColumns = new Dictionary<string, GridViewColumn>(columnsInfo.Count);
			Dictionary<string, GridViewBandColumn> gridbandColumns = new Dictionary<string, GridViewBandColumn>(columnsInfo.Count);
			foreach(ColumnInfoWrapper columnInfo in columnsInfo) {
				if(columnInfo.Column is GridViewBandColumn) {
					gridbandColumns.Add(columnInfo.GridViewColumnInfo.Model.Id, (GridViewBandColumn)columnInfo.Column);
				}
				else {
					gridColumns.Add(columnInfo.GridViewColumnInfo.Model.Id, columnInfo.Column);
				}
			}
			ModelBandLayoutItemCollection sortedRootLevelItems = new ModelBandLayoutItemCollection(layoutElementCollection).GetChildren(null);
			sortedRootLevelItems.Sort(new ModelBandLayoutNodesComparer(true));
			SetupVisibleIndex(gridColumns, gridbandColumns, sortedRootLevelItems);
		}
		private void SetupVisibleIndexForBandChildren(IModelBand bandModel, Dictionary<string, GridViewColumn> gridColumns, Dictionary<string, GridViewBandColumn> gridbandColumns) {
			ModelBandLayoutItemCollection sortedModelColumns = new ModelBandLayoutItemCollection(bandModel).GetChildren(bandModel);
			sortedModelColumns.Sort(new ModelBandLayoutNodesComparer(true));
			SetupVisibleIndex(gridColumns, gridbandColumns, sortedModelColumns);
		}
		private void SetupVisibleIndex(Dictionary<string, GridViewColumn> gridColumns, Dictionary<string, GridViewBandColumn> gridbandColumns, ModelBandLayoutItemCollection sortedModelColumns) {
			int index = 0;
			foreach(IModelBandedLayoutItem modelColumn in sortedModelColumns) {
				int visibleIndex = -1;
				if(!modelColumn.Index.HasValue || modelColumn.Index >= 0) {
					visibleIndex = index;
					index++;
				}
				else {
					visibleIndex = -1;
				}
				if(modelColumn is IModelColumn) {
					gridColumns[modelColumn.Id].VisibleIndex = visibleIndex;
				}
				if(modelColumn is IModelBand) {
					gridbandColumns[modelColumn.Id].VisibleIndex = visibleIndex;
					SetupVisibleIndexForBandChildren((IModelBand)modelColumn, gridColumns, gridbandColumns);
				}
			}
		}
	}
}
