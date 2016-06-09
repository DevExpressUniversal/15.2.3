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
using DevExpress.ExpressApp.Win.Model;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class BandedGridViewColumnFactory : GridViewColumnFactory {
		public override GridColumnModelSynchronizer CreateGridColumnModelSynchronizer(IModelColumn modelColumn, DC.ITypeInfo objectTypeInfo, bool isAsyncServerMode, bool isProtectedColumn) {
			return new BandedGridColumnModelSynchronizer(modelColumn, objectTypeInfo, isAsyncServerMode, isProtectedColumn);
		}
		protected override GridColumn CreateGridColumn() {
			BandedGridColumn result = new BandedGridColumn();
			result.AutoFillDown = true;
			return result;
		}
		protected override void SetupColumns(ColumnView columnView, ModelLayoutElementCollection layoutElementCollection, Dictionary<GridColumn, IGridColumnModelSynchronizer> gridColumns, IModelSynchronizersHolder modelSynchronizersHolder) {
			base.SetupColumns(columnView, layoutElementCollection, gridColumns, modelSynchronizersHolder);
			BandedGridView bandedGridView = (BandedGridView)columnView;
			bandedGridView.Bands.Clear();
			ModelBandLayoutItemCollection bandLayoutItemCollection = new ModelBandLayoutItemCollection(layoutElementCollection);
			CreateBands(bandLayoutItemCollection, null, bandedGridView.Bands, gridColumns, modelSynchronizersHolder);
		}
		private Dictionary<string, BandedGridColumn> GetChildrenColumns(IModelBand bandModel, Dictionary<GridColumn, IGridColumnModelSynchronizer> gridColumns) {
			Dictionary<string, BandedGridColumn> result = new Dictionary<string, BandedGridColumn>();
			Dictionary<string, BandedGridColumn> columnsById = new Dictionary<string, BandedGridColumn>();
			foreach(KeyValuePair<GridColumn, IGridColumnModelSynchronizer> columnInfo in gridColumns) {
				columnsById.Add(columnInfo.Value.Model.Id, (BandedGridColumn)columnInfo.Key);
			}
			ModelBandLayoutItemCollection bandLayoutItemCollection = new ModelBandLayoutItemCollection(bandModel);
			IList<IModelBandedColumnWin> bandChildrenColumn = bandLayoutItemCollection.GetChildren(bandModel).GetItems<IModelBandedColumnWin>();
			foreach(IModelBandedLayoutItem item in bandChildrenColumn) {
				result.Add(item.Id, columnsById[item.Id]);
			}
			return result;
		}
		private void CreateBands(ModelBandLayoutItemCollection model, IModelBand parentBandModel, GridBandCollection bandCollection, Dictionary<GridColumn, IGridColumnModelSynchronizer> gridColumns, IModelSynchronizersHolder modelSynchronizersHolder) {
			List<IModelBand> bands = new List<IModelBand>(model.GetChildren(parentBandModel).GetItems<IModelBand>());
			bands.Sort(new ModelBandLayoutNodesComparer(true));
			foreach(IModelBand bandModel in bands) {
				GridBand band = CreateBandCore(bandModel, bandCollection, modelSynchronizersHolder);
				CreateBands(model, bandModel, band.Children, gridColumns, modelSynchronizersHolder);
				AddColumnsInBand(band, bandModel, model, gridColumns);
			}
		}
		private GridBand CreateBandCore(IModelBand bandModel, GridBandCollection bandCollection, IModelSynchronizersHolder modelSynchronizersHolder) {
			GridBand newGridBand = null;
			GridBandModelSynchronizer gridBandModelSynchronizer = null;
			if(CreateCustomGridBand != null) {
				CreateCustomGridBandEventArgs args = new CreateCustomGridBandEventArgs(bandModel);
				CreateCustomGridBand(this, args);
				newGridBand = args.CustomGridBand;
				gridBandModelSynchronizer = args.CustomGridBandModelSynchronizer;
			}
			if(newGridBand == null) {
				newGridBand = CreateBand(bandModel);
			}
			if(gridBandModelSynchronizer == null) {
				gridBandModelSynchronizer = CreateGridBandInfoByDefault(bandModel);
			}
			bandCollection.Add(newGridBand);
			gridBandModelSynchronizer.ApplyModel(newGridBand);
			modelSynchronizersHolder.RegisterSynchronizer(newGridBand, gridBandModelSynchronizer);
			return newGridBand;
		}
		protected virtual GridBand CreateBand(IModelBand bandModel) {
			return new GridBand();
		}
		protected virtual GridBandModelSynchronizer CreateGridBandInfoByDefault(IModelBand bandModel) {
			return new GridBandModelSynchronizer(bandModel);
		}
		private void AddColumnsInBand(GridBand band, IModelBand bandModel, ModelBandLayoutItemCollection model, Dictionary<GridColumn, IGridColumnModelSynchronizer> gridColumns) {
			Dictionary<string, BandedGridColumn> childrenColumn = GetChildrenColumns(bandModel, gridColumns);
			SetColumnsVisibleIndex(model, bandModel, childrenColumn);
			List<BandedGridColumn> sortedColumns = new List<BandedGridColumn>();
			foreach(KeyValuePair<string, BandedGridColumn> columnInfo in childrenColumn) {
				sortedColumns.Add(columnInfo.Value);
			}
			sortedColumns.Sort(new BandedGridColumnComparer());
			foreach(BandedGridColumn childColumn in sortedColumns) {
				childColumn.OwnerBand = band;
				if(childColumn.Visible) {
					band.Columns.Add(childColumn);
				}
			}
		}
		private void SetColumnsVisibleIndex(ModelBandLayoutItemCollection model, IModelBand bandModel, Dictionary<string, BandedGridColumn> bandColumns) {
			List<IModelBandedColumnWin> list = new List<IModelBandedColumnWin>(model.GetChildren(bandModel).GetItems<IModelBandedColumnWin>());
			list.Sort(new ModelBandLayoutNodesComparer(true));
			Dictionary<int, int> visibleIndexByRowLevel = new Dictionary<int, int>();
			foreach(IModelBandedLayoutItem modelColumn in list) {
				int visibleIndex = -1;
				BandedGridColumn column;
				if(bandColumns.TryGetValue(modelColumn.Id, out column)) {
					if(!modelColumn.Index.HasValue || modelColumn.Index >= 0) {
						int index;
						visibleIndexByRowLevel.TryGetValue(column.RowIndex, out index);
						visibleIndex = index;
						index++;
						visibleIndexByRowLevel[column.RowIndex] = index;
					}
				}
				if(column != null) {
					column.VisibleIndex = visibleIndex;
				}
			}
		}
		protected override void SetVisibleIndex(ModelLayoutElementCollection columns, Dictionary<GridColumn, IGridColumnModelSynchronizer> gridColumns) {
		}
		public event EventHandler<CreateCustomGridBandEventArgs> CreateCustomGridBand;
	}
}
