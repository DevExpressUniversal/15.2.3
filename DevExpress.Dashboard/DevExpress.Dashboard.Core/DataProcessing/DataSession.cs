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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor;
using DevExpress.Data;
namespace DevExpress.DashboardCommon.DataProcessing {
	public interface IDataSession : IDataProcessor<SliceDataQuery>, IDisposable {
	}
	public class DataSession : IDataSession {
		public static bool UseOldClientModeEngine { get; set; }
		public static bool CalculateOldClientModeEngine { get { return CheckNewEngineResults || UseOldClientModeEngine; } }
		public static bool CalculateNewClientModeEngine { get { return CheckNewEngineResults || !UseOldClientModeEngine; } }
		public static bool CheckNewEngineResults {
			get {
#if CheckNewClientModeEngineResult
				return true;
#else
				return false;
#endif
			}
		}
		readonly DataSourceModel dataSource;
		readonly PivotGridDataProcessor pivotProcessor;
		readonly InMemorySliceDataProcessor sliceProcessor;
		public DataSession(DataSourceModel dataSource) {
			this.dataSource = dataSource;
			this.pivotProcessor = new PivotGridDataProcessor(dataSource);
			this.sliceProcessor = new InMemorySliceDataProcessor(dataSource);
		}
		DataQueryResult GetDataInternal(SliceDataQuery query, CancellationToken cancellationToken) {		
			List<DataQueryResult> result = new List<DataQueryResult>();
			bool clientMode = dataSource.DataProcessingMode == DataProcessingMode.Client;
			if(clientMode && CalculateNewClientModeEngine)
				result.Add(sliceProcessor.GetData(query, cancellationToken));
			if(!clientMode || CalculateOldClientModeEngine)
				result.Add(pivotProcessor.GetData(query.ToPivotDataQuery(), cancellationToken));
			if(clientMode && CheckNewEngineResults) {
				Helper.AssertStoragesAreEqual(query, result[1].HierarchicalData.Storage, result[0].HierarchicalData.Storage);
				Helper.AssertSummaryAggregationsAreEqual(query, result[1].SummaryAggregationResults, result[0].SummaryAggregationResults);
			}
			if(UseOldClientModeEngine)
				result.Reverse();
			List<SummaryAggregationResult> summaryResults = new List<SummaryAggregationResult>();
			HierarchicalDataParams data = result[0].HierarchicalData;
			foreach(SummaryAggregationResult summaryResult in result[0].SummaryAggregationResults) {
				if(summaryResult.AggModel.IncludeInGrandTotal) {
					StorageSlice slice = data.Storage.GetSlice(new StorageColumn[0]);
					StorageRow row = new StorageRow();
					StorageColumn column = data.Storage.CreateColumn(summaryResult.Name, false);
					row[column] = StorageValue.CreateUnbound(summaryResult.Value);
					StorageRow? foundRow = slice.FindRow(row);
					if(foundRow.HasValue) {
						row = foundRow.Value;
						row[column] = StorageValue.CreateUnbound(summaryResult.Value);
					} else {
						slice.AddRow(row);
					}
				} else {
					summaryResults.Add(summaryResult);
				}
			}
			return new DataQueryResult(data, summaryResults);
		}
		DashboardUnderlyingDataSet GetUnderlyingDataInternal(UnderlyingDataQuery<SliceDataQuery> query) {
			List<DashboardUnderlyingDataSet> result = new List<DashboardUnderlyingDataSet>();
			bool clientMode = dataSource.DataProcessingMode == DataProcessingMode.Client;
			if(clientMode && CalculateNewClientModeEngine)
				result.Add(sliceProcessor.GetUnderlyingData(query));
			if(!clientMode || CalculateOldClientModeEngine)
				result.Add(pivotProcessor.GetUnderlyingData(query.ToPivotDataQuery()));
			if(clientMode && CheckNewEngineResults)
				Helper.CompareUnderlyingDataSet(result[1], result[0]);
			if(UseOldClientModeEngine)
				result.Reverse();
			return result[0];
		}
		#region IDataSession implementation
		DataQueryResult IDataProcessor<SliceDataQuery>.GetData(SliceDataQuery query, CancellationToken cancellationToken) {
			DataQueryResult result;
			try {
				if(ValidateQuery(query, dataSource))
					result = GetDataInternal(query, cancellationToken);
				else
					result = DataQueryResult.Empty;
			} catch {
				result = DataQueryResult.Empty;
			}
			return result;
		}
		DashboardUnderlyingDataSet IDataProcessor<SliceDataQuery>.GetUnderlyingData(UnderlyingDataQuery<SliceDataQuery> query) {
			DashboardUnderlyingDataSet result;
			try {
				if(ValidateQuery(query.SliceQuery, dataSource))
					result = GetUnderlyingDataInternal(query);
				else
					result = null;
			} catch {
				result = null;
			}
			return result;
		}
		public void Dispose() {
			pivotProcessor.Dispose();
		}
		#endregion
		static bool ValidateQuery(SliceDataQuery query, DataSourceModel dataSource) {
			query.DataSlices.ForEach(slice => {
				DXContract.Requires(slice.Dimensions.All(d => d != null));
				DXContract.Requires(slice.Measures.All(d => d != null));
				DXContract.Requires(slice.FilterDimensions.All(d => d != null));
				DXContract.Requires(slice.DimensionsSort.All(d => d != null));
				DXContract.Requires(slice.DimensionsTopN.All(d => d != null));
				DXContract.Requires(slice.DimensionsTopN.All(topn => topn.TopNSubgroup.All(groupDimension => slice.Dimensions.Contains(groupDimension))));
				DXContract.Requires(slice.Dimensions.Distinct().Count() == slice.Dimensions.Count(), "DataSliceDefinition: dublicates of dimensions not allowed");
				DXContract.Requires(slice.Measures.Distinct().Count() == slice.Measures.Count(), "DataSliceDefinition: dublicates of measures not allowed");
				foreach(DimensionModel dimension in slice.Dimensions) {
					DimensionSortModel sortModel = slice.DimensionsSort.SingleOrDefault(sort => sort.SortedDimension == dimension);
					DimensionTopNModel topNModel = slice.DimensionsTopN.SingleOrDefault(topn => topn.DimensionModel == dimension);
					if(topNModel != null) {
						DXContract.Requires(topNModel.TopNCount > 0);
						DXContract.Requires(topNModel.TopNMeasure != null);
					}
				}
			});
			return ValidateDataQuery(query, dataSource) && !CriteriaTreeSearcher.Contains(query.ToPivotDataQuery().FilterCriteria, Helper.GetExcludingAllFilterCriteria());
		}
		static bool ValidateDataQuery(SliceDataQuery query, DataSourceModel dataSource) {
			return new SliceDataQueryValidator(query, dataSource).Validate();
		}
	}
	public class DataQueryResult {
		public HierarchicalDataParams HierarchicalData { get; private set; }
		public IEnumerable<SummaryAggregationResult> SummaryAggregationResults { get; private set; }
		public DataQueryResult(HierarchicalDataParams hierarchicalData, IEnumerable<SummaryAggregationResult> summaryAggregationResults) {
			this.HierarchicalData = hierarchicalData;
			this.SummaryAggregationResults = summaryAggregationResults.ToList();
		}
		public static DataQueryResult Empty {
			get {
				return new DataQueryResult(new HierarchicalDataParams(), new SummaryAggregationResult[0]);
			}
		}
	}
	public class SummaryAggregationResult {
		public string Name { get; private set; }
		public IList<DimensionModel> Slice { get; private set; }
		public SummaryAggregationModel AggModel { get; private set; }
		public object Value { get; private set; }
		public SummaryAggregationResult(string name, IEnumerable<DimensionModel> slice, SummaryAggregationModel aggModel, object value) {
			DXContract.Requires(slice != null);
			DXContract.Requires(aggModel != null);
			this.Name = name;
			this.Slice = slice.ToList();
			this.AggModel = aggModel;
			this.Value = value;
		}
	}
}
