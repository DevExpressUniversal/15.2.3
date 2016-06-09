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
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal abstract class ColumnsModelSynchronizer : IModelSynchronizable, IDisposable {
		private ASPxGridListEditor listEditor;
		private IModelListView model;
		protected ASPxGridListEditor ListEditor {
			get { return listEditor; }
		}
		protected IModelListView Model {
			get { return model; }
		}
		public ColumnsModelSynchronizer(ASPxGridListEditor listEditor, IModelListView model) {
			this.listEditor = listEditor;
			this.model = model;
		}
		protected void ApplyModelSortNodes(ColumnWrapper column, IModelColumn model) {
			if(column.AllowSortingChange) {
				if(model.SortIndex != -1 && model.SortOrder == Data.ColumnSortOrder.None) {
					column.SortIndex = model.SortIndex;
				}
				else if(model.SortIndex == -1 && model.SortOrder != Data.ColumnSortOrder.None) {
					column.SortOrder = model.SortOrder;
				}
				else {
					column.SortOrder = model.SortOrder;
					column.SortIndex = model.SortIndex;
				}
			}
			if(column.AllowGroupingChange) {
				column.GroupInterval = (DateTimeGroupInterval)Enum.Parse(typeof(DateTimeGroupInterval), model.GroupInterval.ToString());
				column.GroupIndex = model.GroupIndex;
			}
		}
		public virtual void ApplyModel() {
			listEditor.PopulateColumns();
			ApplyModelAfterPopulateColumns();
		}
		public virtual void SynchronizeModel() {
			foreach(ColumnWrapper column in listEditor.Columns) {
				column.SynchronizeModel();
			}
			SynchronizeVisibleIndexes(listEditor, model);
		}
		public void Dispose() {
			listEditor = null;
			model = null;
		}
		protected abstract void ApplyModelAfterPopulateColumns();
		protected abstract void SynchronizeVisibleIndexes(ASPxGridListEditor listEditor, IModelListView model);
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class ASPxGridViewColumnsModelSynchronizer : ColumnsModelSynchronizer {
		public ASPxGridViewColumnsModelSynchronizer(ASPxGridListEditor listEditor, IModelListView model)
			: base(listEditor, model) {
		}
		protected override void SynchronizeVisibleIndexes(ASPxGridListEditor listEditor, IModelListView model) { 
			List<IModelColumn> visibleColumnsFromModel = new List<IModelColumn>(model.Columns.GetVisibleColumns());
			visibleColumnsFromModel.Sort(new ColumnModelNodesComparer());
			List<KeyValuePair<string, IModelColumn>> fastVisibleColumnsFromModel = new List<KeyValuePair<string, IModelColumn>>(visibleColumnsFromModel.Count);
			foreach(IModelColumn mColumn in visibleColumnsFromModel) {
				fastVisibleColumnsFromModel.Add(new KeyValuePair<string, IModelColumn>(mColumn.Id, mColumn));
			}
			List<ColumnWrapper> visibleColumnsFromControl = listEditor.GetVisibleColumns();
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
			List<string> visibleColumns = new List<string>(fastVisibleColumnsFromControl.Count);
			for(int i = 0; i < fastVisibleColumnsFromControl.Count; i++) {
				string columnID = fastVisibleColumnsFromControl[i].Key;
				visibleColumns.Add(columnID);
				if(!(visibleColumnsFromModel.Count > i && fastVisibleColumnsFromModel[i].Key == columnID)) {
					IModelColumn modelColumn = model.Columns[columnID];
					if(modelColumn != null) {
						modelColumn.Index = i;
					}
				}
			}
			foreach(IModelColumn node in model.Columns) {
				if(!visibleColumns.Contains(node.Id)) {
					node.Index = -1;
				}
			}
		}
		protected override void ApplyModelAfterPopulateColumns() {
			ModelLayoutElementCollection layoutElementCollection = new ModelLayoutElementCollection(Model.Columns);
			layoutElementCollection.Sort(new ModelColumnSortIndexComparer());
			foreach(IModelColumn modelColumn in layoutElementCollection) {
				ColumnWrapper column = ListEditor.FindColumn(modelColumn.Id);
				ApplyModelSortNodes(column, modelColumn);
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelColumnSortIndexComparer : IComparer<IModelNode> {
		public int Compare(IModelNode x, IModelNode y) {
			IModelColumn column1 = x as IModelColumn;
			IModelColumn column2 = y as IModelColumn;
			if(column1 != null && column2 != null) {
				return column1.SortIndex.CompareTo(column2.SortIndex);
			}
			return 0;
		}
	}
}
