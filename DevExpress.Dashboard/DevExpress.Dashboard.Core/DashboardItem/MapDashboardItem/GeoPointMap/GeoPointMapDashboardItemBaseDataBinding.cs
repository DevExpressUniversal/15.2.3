#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon {
	public partial class MapDashboardItem {
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			return TooltipMeasures.NotNull();
		}
	}
	public partial class GeoPointMapDashboardItemBase {
		Dictionary<GeoPointEx, List<object>> olapUniqueCoordinates = new Dictionary<GeoPointEx,List<object>>();
		#region ISliceDataQueryProvider implementation
		protected IEnumerable<Dimension> QueryGeoPointDimensions { get { return new[] { Latitude, Longitude }; } }
		protected virtual IEnumerable<Dimension> QueryDimensions { get { return QueryGeoPointDimensions.Concat(TooltipDimensions).NotNull(); } }
		internal Dictionary<GeoPointEx, List<object>> OlapUniqueCoordinates { get { return olapUniqueCoordinates; } }
		internal IGeoPointClusterizator Clusterizer { get { return clusterizator; } }
		internal bool HasClusteredResult {  get { return Clusterizer != null && Clusterizer.HasClusteredResult; } }
		protected override SliceDataQuery GetDataQueryInternal(IActualParametersProvider provider) {
			if(!IsMapReady)
				return new SliceDataQuery();
			IEnumerable<Dimension> dimensions = QueryDimensions;
			SliceDataQueryBuilder queryBuilder = null;
			ItemModelBuilder itemBuilder = new ItemModelBuilder(DataSourceModel.DataSourceInfo, GetDataItemUniqueName, provider);
			if(IsBackCompatibilityDataSlicesRequired) {
				queryBuilder = SliceDataQueryBuilder.CreateWithPivotModel(itemBuilder, dimensions, new Dimension[0],
					QueryMeasures, QueryFilterDimensions, GetQueryFilterCriteria(provider));
			} else {
				queryBuilder = SliceDataQueryBuilder.CreateEmpty(itemBuilder, QueryFilterDimensions, GetQueryFilterCriteria(provider));
				queryBuilder.AddSlice(dimensions, QueryMeasures);
				queryBuilder.AddSlice(dimensions.Except<Dimension>(TooltipDimensions), QueryMeasures); 
				queryBuilder.SetAxes(dimensions, new Dimension[0]);
			}
			return queryBuilder.FinalQuery();
		}
		protected override MultidimensionalDataDTO PrepareDataInternal(DataQueryResult queryResult, SliceDataQuery query, IActualParametersProvider parameters, ColorRepository coloringCache, IDictionary<string, object> filter) {
			if(!IsMapReady)
				 return new MultidimensionalDataDTO() { HierarchicalDataParams = new HierarchicalDataParams() };
			MultidimensionalDataDTO data = base.PrepareDataInternal(queryResult, query, parameters, coloringCache, filter);
			if(!EnableClustering)
				return data;
			if(currentMapInfo == null)
				return new MultidimensionalDataDTO() { HierarchicalDataParams = new HierarchicalDataParams() };
			return GetClusterizedData(query, data.HierarchicalDataParams);
		}
		#endregion
		#region Clusterization over DataSession
		IGeoPointClusterizator clusterizator;
		MapClusterizationRequestInfo currentMapInfo;
		IGeoPointClusterizator CreateClusterizator() {
			IGeoPointClusterizator clusterizator = new GeoPointClusterizator();
			clusterizator.UpdateMapInfo(currentMapInfo);
			DimensionValueSet selectionDimensionValueSet = ((IMasterFilter)this).Parameters.First().Values;
			IEnumerable<ISelectionRow> selection = selectionDimensionValueSet.GetValuesSet(new[] { this.Latitude, this.Longitude });
			if(IsOlapDataSource && EnableClustering)
				selection = GetOlapCoordinates(selection);
			IList<GeoPointEx> selectionPoints = selection.Select(row => new GeoPointEx(Helper.ConvertToDouble(row[0]), Helper.ConvertToDouble(row[1]))).ToList();
			clusterizator.UpdateSelection(selectionPoints);
			return clusterizator;
		}
		IEnumerable<ISelectionRow> GetOlapCoordinates(IEnumerable<ISelectionRow> data) {
			return olapUniqueCoordinates
					.Where(olap => data.Any(row => object.Equals(row[0], olap.Value[0]) && object.Equals(row[1], olap.Value[1])))
					.Select(olap => new List<object> { olap.Key.Latitude, olap.Key.Longitude }.AsSelectionRow());
		}
		MultidimensionalDataDTO GetClusterizedData(SliceDataQuery firstProcessingQuery, HierarchicalDataParams hData) {
			InternalMapDataMembersContainer dataMembers = new InternalMapDataMembersContainer();
			FillClientDataDataMembers(dataMembers);
			clusterizator = CreateClusterizator();
			if(!clusterizator.Initialized)
				return new MultidimensionalDataDTO() { HierarchicalDataParams = new HierarchicalDataParams() };
			ClusterizedMapGeoPointData clusterizedData = clusterizator.CreateClusteredData(IsOlapDataSource, hData, dataMembers);
			olapUniqueCoordinates = clusterizedData.OlapUniqueCoordinates;
			if(clusterizedData.Count == 0)
				return new MultidimensionalDataDTO() { HierarchicalDataParams = new HierarchicalDataParams() };
			ClusterizedMapDataEngine cde = new ClusterizedMapDataEngine(clusterizedData, dataMembers);
			cde.Calculate();
			return new MultidimensionalDataDTO() { HierarchicalDataParams = cde.GetHierarchicalDataParams() };
		}
		#endregion
		protected override void RemoveMeasuresFromDataStorage(DataStorage storage, SliceDataQuery query, IActualParametersProvider provider) {
			if(EnableClustering)
				return;
			base.RemoveMeasuresFromDataStorage(storage, query, provider);
		}
		internal IValuesSet PrepareSelectedValues(IValuesSet values) {
			IEnumerable<ISelectionRow> data = values;
			if(!EnableClustering)
				return values;
			EnsureCacheCreated();
			if(clusterizator == null)
				return values;
			if(IsOlapDataSource)
				data = GetOlapCoordinates(data);
			IEnumerable<ISelectionRow> preparedData = data
				  .Select(row => new GeoPointEx(Helper.ConvertToDouble(row[0]), Helper.ConvertToDouble(row[1])))
				  .Select(point => clusterizator.GetCluster(point))
				  .Distinct()
				  .Where(point => point != null)
				  .Select(point => new List<object> { point.Latitude, point.Longitude }.AsSelectionRow());
			return preparedData.AsValuesSet();
		}
		internal IValuesSet PrepareBeforeSetMasterFilter(IValuesSet valueSet) {
			if(!EnableClustering)
				return valueSet;
			EnsureCacheCreated();
			if(clusterizator == null)
				return valueSet;
			IEnumerable<ISelectionRow> selectionRows = valueSet;
			IList<GeoPointEx> points = new List<GeoPointEx>();
			foreach (ISelectionRow row in selectionRows)
				points.Add(new GeoPointEx(Helper.ConvertToDouble(row[0]), Helper.ConvertToDouble(row[1])));
			points = clusterizator.GetClusteredData(points);
			IEnumerable<ISelectionRow> values = IsOlapDataSource ?
				points.Select(point => olapUniqueCoordinates[point].AsSelectionRow()) :
				points.Select(x => new List<object> { x.Latitude, x.Longitude }.AsSelectionRow());
			return values.AsValuesSet();
		}
		void EnsureCacheCreated() {
			if(clusterizator != null && currentMapInfo != null)
				clusterizator.UpdateMapInfo(currentMapInfo);
		}
	}
}
