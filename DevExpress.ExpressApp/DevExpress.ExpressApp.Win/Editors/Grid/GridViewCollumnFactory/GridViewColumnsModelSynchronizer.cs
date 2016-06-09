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
using DevExpress.ExpressApp.Model.Core;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class GridViewColumnsModelSynchronizer : IModelSynchronizable, IDisposable {
		private WinColumnsListEditor listEditor;
		private IModelListView model;
		public GridViewColumnsModelSynchronizer(WinColumnsListEditor listEditor, IModelListView model) {
			this.listEditor = listEditor;
			this.model = model;
		}
		public void ApplyModel() {
			listEditor.PopulateColumns();
		}
		public void SynchronizeModel() {
			foreach(ColumnWrapper column in listEditor.Columns) {
				column.SynchronizeModel();
			}
			SynchronizeVisibleIndexes(listEditor, model);
		}
		private void SynchronizeVisibleIndexes(ColumnsListEditor listEditor, IModelListView model) {
			List<IModelColumn> visibleNodesFromModel = new List<IModelColumn>(model.Columns.GetVisibleColumns());
			visibleNodesFromModel.Sort(new ColumnModelNodesComparer());
			List<ColumnWrapper> visibleColumnsFromControl = listEditor.GetVisibleColumns();
			visibleColumnsFromControl.Sort(delegate(ColumnWrapper column1, ColumnWrapper column2) { return column1.VisibleIndex.CompareTo(column2.VisibleIndex); });
			List<string> visibleColumnsFromControl_ID = new List<string>();
			foreach(ColumnWrapper item in visibleColumnsFromControl) {
				if(!string.IsNullOrEmpty(item.Id)) {
					visibleColumnsFromControl_ID.Add(item.Id);
				}
			}
			for(int i = 0; i < visibleColumnsFromControl_ID.Count; i++) {
				string columnID = visibleColumnsFromControl_ID[i];
				if(!(visibleNodesFromModel.Count > i && visibleNodesFromModel[i].Id == columnID)) {
					IModelColumn modelColumn = model.Columns[columnID];
					if(modelColumn != null) {
						modelColumn.Index = i;
					}
				}
			}
			foreach(IModelColumn node in model.Columns) {
				if(!visibleColumnsFromControl_ID.Contains(node.Id)) {
					node.Index = -1;
				}
			}
		}
		public void Dispose() {
			listEditor = null;
			model = null;
		}
	}
}
