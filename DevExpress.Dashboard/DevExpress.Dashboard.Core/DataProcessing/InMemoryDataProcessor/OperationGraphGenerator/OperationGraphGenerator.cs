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
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using System.Linq.Expressions;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public static class OperationGraphGenerator {
		static bool IsEmptySlice(SliceModel slice) {
			bool empty = false;
			RowFiltersModel<DimensionModel>[] rowFilterList = slice.RowFilters;
			if(rowFilterList != null) {
				foreach(RowFiltersModel<DimensionModel> rowFilter in rowFilterList) {
					empty |= rowFilter.FilterType == RowFiltersType.Include && rowFilter.Values.Count == 0;
				}
			}
			return empty;
		}
		public static OperationGraph Generate(SliceDataQuery query, IStorage storage) {
			DXContract.Requires(storage != null, "storage is null");
			OperationGraph graph = new OperationGraph();
			List<DimensionTopNModel> topNs = query.DataSlices.SelectMany(s => s.DimensionsTopN).Where(d => d.TopNEnabled && !d.TopNShowOthers).Distinct().ToList();
			foreach(SliceModel slice in query.DataSlices)
				foreach(DimensionTopNModel topN in topNs)
					DXContract.Requires(slice.DimensionsTopN.Contains(topN) == true); 
			SliceFlowSourceCreatorCache cache = new SliceFlowSourceCreatorCache(query, storage);
			foreach(SliceModel slice in query.DataSlices) {
				if(IsEmptySlice(slice))
					continue;
				SliceFlowSourceCreator sourceFlowSource = cache.Get(slice);
				if(slice.DimensionsSort.Count != 0)
					DXContract.Requires(slice.Dimensions.Count == slice.DimensionsSort.Count); 
				Group group = new Group(slice.Dimensions.Select(d => sourceFlowSource.DataSourceDimensionResolver.Resolve(d)).ToArray());
				MultiScanBuffer multiScanBuffer = new MultiScanBuffer(group);
				List<NamedFlow> groupOutputs = slice.Dimensions.Select((d, i) =>
												   new NamedFlow(d.Name, new Extract(multiScanBuffer, i))
																).ToList();
				SummaryMeasureFlowSource resolveMeasure = new SummaryMeasureFlowSource(slice.Measures, group, sourceFlowSource.DataSourceMeasureResolver);
				List<NamedFlow> aggOutputs = slice.Measures.Select(m => new NamedFlow(m.Name, resolveMeasure.Resolve(m))).ToList();
				foreach(SummaryAggregationModel agg in slice.SummaryAggregations) {
					SingleFlowOperation inputFlow;
					if(agg.Measure != null)
						inputFlow = resolveMeasure.Resolve(agg.Measure);
					else
						inputFlow = groupOutputs[slice.Dimensions.IndexOf(agg.Dimension)].Flow;
					SingleBlockOperation aggregateMeasure = DataItemModelToFlowHelper.CreateSummaryAggregationFlow(agg.SummaryType, agg.Argument, inputFlow);
					graph.SummaryAggregations.Add(new SummaryAggregationsKey(slice, agg), aggregateMeasure);
				}
				List<NamedFlow> outputs = groupOutputs.Concat(aggOutputs).ToList();
				outputs = SortCreator.CreateSort(slice, outputs, sourceFlowSource, resolveMeasure);
				foreach(NamedFlow p in outputs)
					graph.Output.Add(new ResultKey(slice, p.DataItemID), new Buffer(p.Flow));
			}
			return graph;
		}
		public static OperationGraph GenerateUnderlying(UnderlyingDataQuery<SliceDataQuery> query, IStorage storage) {
			DXContract.Requires(storage != null, "storage is null");
			OperationGraph graph = new OperationGraph();
			AnyDataMemberResolver anyResolver = new AnyDataMemberResolver(query.SliceQuery, storage);
			ByUnderlyingSliceFlowSourceCreator sourceFlowSource = new ByUnderlyingSliceFlowSourceCreator(query, storage, anyResolver);
			foreach(string name in query.DataMembers)
				graph.Output.Add(new KeyValuePair<ResultKey, Buffer>(new ResultKey(null, name), new Buffer(sourceFlowSource.UnderlyingResolver.Resolve(name))));
			return graph;
		}
	}
	public class SliceModelWrapper {
		SliceModel model;
		public SliceModel Model {
			get { return model; }
			set { model = value; }
		}
		public SliceModelWrapper(SliceModel model) {
			this.model = model;
		}
	}
	public class OperationGraph {
		public IDictionary<ResultKey, Buffer> Output { get; private set; }
		public IDictionary<SummaryAggregationsKey, SingleBlockOperation> SummaryAggregations { get; private set; }
		public OperationGraph() {
			this.Output = new Dictionary<ResultKey, Buffer>();
			this.SummaryAggregations = new Dictionary<SummaryAggregationsKey, SingleBlockOperation>();
		}
	}
	public class SummaryAggregationsKey : Tuple<SliceModel, SummaryAggregationModel> {
		public SliceModel SliceModel { get { return Item1; } }
		public SummaryAggregationModel SummaryAggregationModel { get { return Item2; } }
		public SummaryAggregationsKey(SliceModel sliceModel, SummaryAggregationModel summaryAggregationModel) : base(sliceModel, summaryAggregationModel) { }
	}
	public class ResultKey : Tuple<SliceModel, string> {
		public SliceModel SliceModel { get { return Item1; } }
		public string DataItemID { get { return Item2; } }
		public ResultKey(SliceModel sliceModel, string dataItemID) : base(sliceModel, dataItemID) { }
	}
	public class NamedFlow : Tuple<string, SingleFlowOperation> {
		public string DataItemID { get { return Item1; } }
		public SingleFlowOperation Flow { get { return Item2; } }
		public NamedFlow(string dataItemID, SingleFlowOperation flow) : base(dataItemID, flow) { }
	}
}
