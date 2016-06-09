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
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class ASPxGridViewBandColumnsModelSynchronizer : ColumnsModelSynchronizer {
		public ASPxGridViewBandColumnsModelSynchronizer(ASPxGridListEditor listEditor, IModelListView model)
			: base(listEditor, model) {
		}
		protected override void SynchronizeVisibleIndexes(ASPxGridListEditor listEditor, IModelListView model) {
			Dictionary<WebColumnBase, ColumnWrapper> columnsFromControl = new Dictionary<WebColumnBase, ColumnWrapper>();
			foreach(ColumnWrapper wrapper in listEditor.Columns) {
				WebColumnBaseColumnWrapper webColumnWrapper = wrapper as WebColumnBaseColumnWrapper;
				if(webColumnWrapper != null) {
					columnsFromControl.Add(webColumnWrapper.Column, webColumnWrapper);
				}
			}
			SynchronizeVisibleIndexes(listEditor, model, columnsFromControl);
		}
		private void SynchronizeVisibleIndexes(ASPxGridListEditor listEditor, IModelListView model, Dictionary<WebColumnBase, ColumnWrapper> columnsFromControl) {
			ModelBandLayoutItemCollection bandsLayout = new ModelBandLayoutItemCollection(model);
			List<ColumnWrapper> visibleColumnsFromControl = new List<ColumnWrapper>();
			List<string> visibleColumns = new List<string>();
			foreach(WebColumnBase column in listEditor.Grid.Columns) {
				if(column.Visible) {
					visibleColumnsFromControl.Add(columnsFromControl[column]);
				}
				if(column is GridViewBandColumn) {
					visibleColumns.AddRange(SynchronizeVisibleIndexesForBands((GridViewBandColumn)column, columnsFromControl, bandsLayout));
				}
			}
			List<IModelBandedLayoutItem> visibleColumnsFromModel = new List<IModelBandedLayoutItem>(bandsLayout.GetAllVisibleItems());
			List<KeyValuePair<string, IModelBandedLayoutItem>> fastVisibleColumnsFromModel = GetFastVisibleItemsFromModel(visibleColumnsFromModel);
			visibleColumns.AddRange(GetVisibleColumns(visibleColumnsFromControl, visibleColumnsFromModel, fastVisibleColumnsFromModel));
			foreach(IModelBandedLayoutItem node in bandsLayout) {
				if(!visibleColumns.Contains(node.Id)) {
					node.Index = -1;
				}
			}
		}
		private List<string> SynchronizeVisibleIndexesForBands(GridViewBandColumn bandColumn, Dictionary<WebColumnBase, ColumnWrapper> columnsFromControl, ModelBandLayoutItemCollection collection) {
			List<string> visibleColumns = new List<string>();
			List<ColumnWrapper> visibleColumnsFromControl = new List<ColumnWrapper>();
			foreach(WebColumnBase column in bandColumn.Columns) {
				if(column.Visible) {
					visibleColumnsFromControl.Add(columnsFromControl[column]);
				}
				if(column is GridViewBandColumn) {
					visibleColumns.AddRange(SynchronizeVisibleIndexesForBands((GridViewBandColumn)column, columnsFromControl, collection));
				}
			}
			List<IModelBandedLayoutItem> visibleColumnsFromModel = new List<IModelBandedLayoutItem>(collection.GetVisibleChildren(Model.BandsLayout));
			List<KeyValuePair<string, IModelBandedLayoutItem>> fastVisibleColumnsFromModel = GetFastVisibleItemsFromModel(visibleColumnsFromModel);
			visibleColumns.AddRange(GetVisibleColumns(visibleColumnsFromControl, visibleColumnsFromModel, fastVisibleColumnsFromModel));
			return visibleColumns;
		}
		private List<KeyValuePair<string, IModelBandedLayoutItem>> GetFastVisibleItemsFromModel(List<IModelBandedLayoutItem> visibleColumnsFromModel) {
			visibleColumnsFromModel.Sort(new ColumnModelNodesComparer());
			List<KeyValuePair<string, IModelBandedLayoutItem>> fastVisibleColumnsFromModel = new List<KeyValuePair<string, IModelBandedLayoutItem>>(visibleColumnsFromModel.Count);
			foreach(IModelBandedLayoutItem mColumn in visibleColumnsFromModel) {
				fastVisibleColumnsFromModel.Add(new KeyValuePair<string, IModelBandedLayoutItem>(mColumn.Id, mColumn));
			}
			return fastVisibleColumnsFromModel;
		}
		private List<string> GetVisibleColumns(List<ColumnWrapper> visibleColumnsFromControl, List<IModelBandedLayoutItem> visibleColumnsFromModel, List<KeyValuePair<string, IModelBandedLayoutItem>> fastVisibleColumnsFromModel) {
			List<string> visibleColumns = new List<string>(visibleColumnsFromControl.Count);
			ModelBandLayoutItemCollection bandsLayout = new ModelBandLayoutItemCollection(Model);
			List<KeyValuePair<string, int>> fastVisibleColumnsFromControl = new List<KeyValuePair<string, int>>(visibleColumnsFromControl.Count);
			foreach(ColumnWrapper wrapper in visibleColumnsFromControl) {
				string columnID = wrapper.Id;
				if(!string.IsNullOrEmpty(columnID)) {
					fastVisibleColumnsFromControl.Add(new KeyValuePair<string, int>(columnID, wrapper.VisibleIndex));
				}
			}
			fastVisibleColumnsFromControl.Sort((firstPair, nextPair) => {
				return firstPair.Value.CompareTo(nextPair.Value);
			});
			for(int i = 0; i < fastVisibleColumnsFromControl.Count; i++) {
				string columnID = fastVisibleColumnsFromControl[i].Key;
				visibleColumns.Add(columnID);
				if(!(visibleColumnsFromModel.Count > i && fastVisibleColumnsFromModel[i].Key == columnID)) {
					IModelBandedLayoutItem modelColumn = bandsLayout[columnID];
					if(modelColumn != null) {
						modelColumn.Index = i;
					}
				}
			}
			return visibleColumns;
		}
		protected override void ApplyModelAfterPopulateColumns() {
			ModelLayoutElementCollection layoutElementCollection = new ModelLayoutElementCollection(new ModelBandLayoutItemCollection(Model));
			layoutElementCollection.Sort(new ModelColumnSortIndexComparer());
			foreach(IModelLayoutElement element in layoutElementCollection) {
				IModelColumn modelColumn = element as IModelColumn;
				if(modelColumn != null) {
					ColumnWrapper column = ListEditor.FindColumn(modelColumn.Id);
					ApplyModelSortNodes(column, modelColumn);
				}
			}
		}
	}
}
