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
using DevExpress.ExpressApp.Win.Model;
using DevExpress.XtraGrid.Views.BandedGrid;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class BandedGridViewColumnsModelSynchronizer : IModelSynchronizable, IDisposable {
		private WinColumnsListEditor listEditor;
		private IModelListView model;
		public BandedGridViewColumnsModelSynchronizer(WinColumnsListEditor listEditor, IModelListView model) {
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
			SynchronizeBandModel(listEditor.ColumnView as BandedGridView, listEditor.ColumnView as IModelSynchronizersHolder);
		}
		private void SynchronizeBandModel(BandedGridView bandedGridView, IModelSynchronizersHolder modelSynchronizersHolder) {
			if(bandedGridView != null && modelSynchronizersHolder != null) {
				SynchronizeBandModel(bandedGridView.Bands, modelSynchronizersHolder, null);
			}
		}
		private void SynchronizeBandModel(GridBandCollection bands, IModelSynchronizersHolder modelSynchronizersHolder, IModelBand parentBandModel) {
			foreach(GridBand band in bands) {
				IModelSynchronizer bandInfo = modelSynchronizersHolder.GetSynchronizer(band);
				IModelBand bandModel = bandInfo != null ? bandInfo.Model as IModelBand : null;
				if(bandModel != null) {
					bandModel.OwnerBand = parentBandModel;
				}
				if(bandInfo != null) {
					bandInfo.SynchronizeModel(band);
				}
				SynchronizeBandColumnsOwnerBand(modelSynchronizersHolder, band.Columns, bandModel);
				SynchronizeBandColumnsVisibleIndex(modelSynchronizersHolder, band.Columns, bandModel);
				SynchronizeBandModel(band.Children, modelSynchronizersHolder, bandModel);
			}
			SynchronizeBandsVisibleIndex(modelSynchronizersHolder, bands, parentBandModel);
		}
		private void SynchronizeBandColumnsOwnerBand(IModelSynchronizersHolder modelSynchronizersHolder, GridBandColumnCollection columns, IModelBand bandModel) {
			foreach(BandedGridColumn column in columns) {
				IModelSynchronizer columnInfo = modelSynchronizersHolder.GetSynchronizer(column);
				if(columnInfo != null) {
					IModelBandedColumn columnModel = columnInfo.Model as IModelBandedColumn;
					if(columnModel != null) {
						columnModel.OwnerBand = bandModel;
					}
				}
			}
		}
		private void SynchronizeBandsVisibleIndex(IModelSynchronizersHolder modelSynchronizersHolder, GridBandCollection bands, IModelBand bandModel) {
			ModelBandLayoutItemCollection allItems = bandModel != null ? new ModelBandLayoutItemCollection(bandModel) : new ModelBandLayoutItemCollection(model);
			ModelBandLayoutItemCollection visibleColumnsModel = new ModelBandLayoutItemCollection(new ModelBandLayoutItemCollection(allItems.GetVisibleChildren(bandModel)).GetItems<IModelBand>());
			visibleColumnsModel.Sort(new ModelBandLayoutNodesComparer(true));
			List<GridBand> sortedVisibleBandColumns = new List<GridBand>();
			foreach(GridBand band in bands) {
				if(band.VisibleIndex > -1) {
					sortedVisibleBandColumns.Add(band);
				}
				else {
					IModelSynchronizer columnModelSynchronizer = modelSynchronizersHolder.GetSynchronizer(band);
					if(columnModelSynchronizer != null) {
						allItems[columnModelSynchronizer.Model.Id].Index = -1;
					}
				}
			}
			sortedVisibleBandColumns.Sort(delegate(GridBand column1, GridBand column2) { return column1.VisibleIndex.CompareTo(column2.VisibleIndex); });
			for(int i = 0; i < sortedVisibleBandColumns.Count; i++) {
				Component controlItem = sortedVisibleBandColumns[i];
				IModelSynchronizer columnModelSynchronizer = modelSynchronizersHolder.GetSynchronizer(controlItem);
				if(columnModelSynchronizer != null) {
					string bandColumnModelId = columnModelSynchronizer.Model.Id;
					if(!(visibleColumnsModel.Count > i && bandColumnModelId == visibleColumnsModel[i].Id)) {
						IModelBandedLayoutItem modelColumn = (IModelBandedLayoutItem)allItems[bandColumnModelId];
						if(modelColumn != null) {
							modelColumn.Index = i;
						}
					}
				}
			}
		}
		private void SynchronizeBandColumnsVisibleIndex(IModelSynchronizersHolder modelSynchronizersHolder, GridBandColumnCollection columns, IModelBand bandModel) {
			ModelBandLayoutItemCollection allItems = new ModelBandLayoutItemCollection(bandModel);
			ModelBandLayoutItemCollection visibleColumnsModel = new ModelBandLayoutItemCollection(new ModelBandLayoutItemCollection(allItems.GetVisibleChildren(bandModel)).GetItems<IModelBandedColumn>());
			visibleColumnsModel.Sort(new ModelBandedColumnWinNodesComparer());
			List<BandedGridColumn> sortedVisibleBandColumns = new List<BandedGridColumn>();
			foreach(BandedGridColumn column in columns) {
				if(column.VisibleIndex > -1) {
					sortedVisibleBandColumns.Add(column);
				}
				else {
					IModelSynchronizer columnModelSynchronizer = modelSynchronizersHolder.GetSynchronizer(column);
					if(columnModelSynchronizer != null) {
						allItems[columnModelSynchronizer.Model.Id].Index = -1;
					}
				}
			}
			sortedVisibleBandColumns.Sort(new BandedGridColumnComparer());
			int previousRowIndex = -1;
			int currentRowIndex = 0;
			for(int i = 0; i < sortedVisibleBandColumns.Count; i++) {
				BandedGridColumn column = sortedVisibleBandColumns[i];
				if(previousRowIndex != -1 && column.RowIndex > 0 && previousRowIndex != column.RowIndex) {
					previousRowIndex = column.RowIndex;
					currentRowIndex++;
				}
				else {
					previousRowIndex = column.RowIndex;
				}
				IModelSynchronizer columnModelSynchronizer = modelSynchronizersHolder.GetSynchronizer(column);
				if(columnModelSynchronizer != null) {
					string bandColumnModelId = columnModelSynchronizer.Model.Id;
					if(!(visibleColumnsModel.Count > i && visibleColumnsModel.IndexOf(columnModelSynchronizer.Model.Id) == i)) {
						IModelBandedLayoutItem modelColumn = (IModelBandedLayoutItem)allItems[bandColumnModelId];
						if(modelColumn != null) {
							int newIndex = i - currentRowIndex;
							if(newIndex < 0) {
								newIndex = 0; 
							}
							modelColumn.Index = newIndex;
						}
					}
				}
			}
		}
		public void Dispose() {
			listEditor = null;
			model = null;
		}
	}
	internal class ModelBandedColumnWinNodesComparer : ModelLayoutElementNodesComparer<IModelBandedColumnWin> {
		public ModelBandedColumnWinNodesComparer()
			: base(true) {
		}
		public override int Compare(IModelBandedColumnWin column1, IModelBandedColumnWin column2) {
			if(column1.RowIndex == column2.RowIndex) {
				return base.Compare(column1, column2);
			}
			else {
				return column1.RowIndex.CompareTo(column2.RowIndex);
			}
		}
	}
	internal class BandedGridColumnComparer : IComparer<BandedGridColumn> {
		#region IComparer<BandedGridColumn> Members
		public int Compare(BandedGridColumn column1, BandedGridColumn column2) {
			if(column1.RowIndex == column2.RowIndex) {
				return column1.VisibleIndex.CompareTo(column2.VisibleIndex);
			}
			else {
				return column1.RowIndex.CompareTo(column2.RowIndex);
			}
		}
		#endregion
	}
}
