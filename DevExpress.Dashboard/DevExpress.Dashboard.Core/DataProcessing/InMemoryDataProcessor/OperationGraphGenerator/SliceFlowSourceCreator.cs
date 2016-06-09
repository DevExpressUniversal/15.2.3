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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public class SliceFlowSourceCreatorCache {
		public static CriteriaOperator GetFilterCriteria(SliceModel model) {
			CriteriaOperator filter = model.FilterCriteria;
			if(model.RowFilters != null)
				filter = CriteriaOperator.And(filter, GenerateRowFiltersCriteria(model.RowFilters));
			return filter;
		}
		static CriteriaOperator GenerateRowFiltersCriteria(RowFiltersModel<DimensionModel>[] rowFilters) {
			CriteriaOperator filter = null;
			foreach(RowFiltersModel<DimensionModel> rowFilter in rowFilters) {
				CriteriaOperator rowFiltersCriteria = null;
				if(rowFilter.FilterType == RowFiltersType.Include) {
					rowFiltersCriteria = CriteriaOperator.Or(CriteriaOperator.Parse("1 = 0"));
				}
				foreach(object[] list in rowFilter.Values) {
					CriteriaOperator rowFilterCriteria = null;
					for(int i = 0; i < list.Length; i++) {
						object v = list[i];
						CriteriaOperator criteria = Helper.IsPivotGridOthersValue(v) ? CriteriaOperator.Parse("1 = 1") 
							: CriteriaOperator.Parse(String.Format("[{0}] = ?", rowFilter.Dimensions[i].Name), v);
						rowFilterCriteria = CriteriaOperator.And(rowFilterCriteria, criteria);
					}
					if(rowFilter.FilterType == RowFiltersType.Include)
						rowFiltersCriteria = CriteriaOperator.Or(rowFiltersCriteria, rowFilterCriteria);
					else
						rowFiltersCriteria = CriteriaOperator.And(rowFiltersCriteria, rowFilterCriteria.Not());
				}
				filter = CriteriaOperator.And(filter, rowFiltersCriteria);
			}
			return filter;
		}
		readonly NullableDictionary<CriteriaOperator, SliceFlowSourceCreator> cache = new NullableDictionary<CriteriaOperator, SliceFlowSourceCreator>();
		readonly SliceDataQuery query;
		readonly StorageSingleFlowSource storageSource;
		readonly SliceModelWrapper wrapper;
		public SliceFlowSourceCreatorCache(SliceDataQuery query, IStorage storage) {
			this.query = query;
			wrapper = new SliceModelWrapper(null);
			storageSource = new StorageSingleFlowSource(storage, wrapper, new AnyDataMemberResolver(query, storage));
		}
		public SliceFlowSourceCreator Get(SliceModel slice) {
			SliceFlowSourceCreator creator;
			CriteriaOperator criteria = GetFilterCriteria(slice);
			wrapper.Model = slice;
			if(!cache.TryGetValue(criteria, out creator)) {
				creator = new SliceFlowSourceCreator(query, wrapper, criteria, storageSource);
				cache[criteria] = creator;
			}
			return creator;
		}		
	}
	public class SliceFlowSourceCreator {
		readonly DataItemResolveManager<DimensionModel> dataSourceDimensionResolver;
		readonly DataItemResolveManager<MeasureModel> dataSourceMeasureResolver;
		readonly TopNCreator topNCreator;
		public DataItemResolveManager<DimensionModel> DataSourceDimensionResolver { get { return dataSourceDimensionResolver; } }
		public DataItemResolveManager<MeasureModel> DataSourceMeasureResolver { get { return dataSourceMeasureResolver; } }
		public TopNCreator TopNCreator { get { return topNCreator; } }
		public SliceFlowSourceCreator(SliceDataQuery query, SliceModelWrapper slice, CriteriaOperator filterCriteria, StorageSingleFlowSource storageSource) {
			dataSourceDimensionResolver = new DataItemResolveManager<DimensionModel>();
			dataSourceMeasureResolver = new DataItemResolveManager<MeasureModel>();
			dataSourceDimensionResolver.Add(new DimensionToFlow(dataSourceDimensionResolver));
			dataSourceMeasureResolver.Add(new MeasureToFlow(dataSourceMeasureResolver));
			dataSourceDimensionResolver.Add(storageSource);
			dataSourceMeasureResolver.Add(storageSource);
			List<DimensionModel> dimensions = query.DataSlices.SelectMany(s => s.Dimensions).Distinct().ToList();
			List<MeasureModel> measures = query.DataSlices.SelectMany(s => s.Measures).Distinct().ToList();
			ByDataItemModelFlowSource<DimensionModel> dimensionsResolver = new ByDataItemModelFlowSource<DimensionModel>(dimensions, dataSourceDimensionResolver);
			ByDataItemModelFlowSource<MeasureModel> measureResolver = new ByDataItemModelFlowSource<MeasureModel>(measures, dataSourceMeasureResolver);
			dataSourceDimensionResolver.Add(dimensionsResolver);
			dataSourceMeasureResolver.Add(measureResolver);
			if(!Object.ReferenceEquals(null, filterCriteria)) {
				DataItemResolveManager<DimensionModel> filterDimensionResolver = dataSourceDimensionResolver.Clone();
				filterDimensionResolver.Add(new ByDataItemModelFlowSource<DimensionModel>(slice.Model.FilterDimensions, filterDimensionResolver));
				SingleFlowOperation filter =
						filterDimensionResolver.GetDataSourceLevelCriteria(
															  CriteriaOperator.ToString(new CriteriaParameterReplacer().Process(filterCriteria)),
															  UnboundColumnType.Boolean);
				dataSourceDimensionResolver.Add(new FilteredSingleFlowSource(filter, dataSourceDimensionResolver));
				dataSourceMeasureResolver.Add(new FilteredSingleFlowSource(filter, dataSourceMeasureResolver));
			}
			dataSourceDimensionResolver.PopUp(dimensionsResolver);
			dataSourceMeasureResolver.PopUp(measureResolver);
			topNCreator = new TopNCreator((d) => dataSourceDimensionResolver.Resolve(d, 0), measures, dataSourceMeasureResolver);
			List<DimensionTopNModel> topNModels = slice.Model.DimensionsTopN.Where(m => m.TopNEnabled && !m.TopNShowOthers).ToList();
			if(topNModels.Count > 0) {
				SingleFlowOperation filter = TopNOthersFlowResolver.GetTopNFlow(topNModels, topNCreator);
				dataSourceDimensionResolver.Add(new FilteredSingleFlowSource(filter, dataSourceDimensionResolver));
				dataSourceMeasureResolver.Add(new FilteredSingleFlowSource(filter, dataSourceMeasureResolver));
			}
			dataSourceDimensionResolver.PopUp(dimensionsResolver);
			dataSourceMeasureResolver.PopUp(measureResolver);
			List<DimensionTopNModel> topNOthersModels = query.DataSlices.SelectMany(s => s.DimensionsTopN.Where(m => m.TopNEnabled && m.TopNShowOthers)).Distinct().ToList();
			if(topNOthersModels.Count > 0) {
				dataSourceDimensionResolver.Add(new TopNOthersFlowResolver( dataSourceDimensionResolver, topNOthersModels, topNCreator));
			}
		}
	}
	class ByUnderlyingSliceFlowSourceCreator {
		readonly DataItemResolveManager dataSourceDimensionResolver = null;
		public SingleFlowByNameSource UnderlyingResolver { get { return dataSourceDimensionResolver; } }
		public ByUnderlyingSliceFlowSourceCreator(UnderlyingDataQuery<SliceDataQuery> query, IStorage storage, AnyDataMemberResolver anyResolver) {
			SliceModel model = query.SliceQuery.DataSlices.First();
			dataSourceDimensionResolver = new DataItemResolveManager<DimensionModel>();
			DataItemResolveManager<DimensionModel> dimr = new DataItemResolveManager<DimensionModel>();
			dimr.Add(new DimensionToFlow(dimr));
			DataItemResolveManager<MeasureModel> mesr = new DataItemResolveManager<MeasureModel>();
			mesr.Add(new MeasureToFlow(mesr));
			StorageSingleFlowSource storageSource = new StorageSingleFlowSource(storage, new SliceModelWrapper(model), anyResolver);
			dataSourceDimensionResolver.Add(storageSource);
			List<MeasureModel> measures = query.SliceQuery.DataSlices.SelectMany(s => s.Measures).Distinct().ToList();
			ByDataItemModelFlowSource<DimensionModel> dimensionResolver = new ByDataItemModelFlowSource<DimensionModel>(query.SliceQuery.DataSlices.SelectMany(s => s.Dimensions).Distinct(), dimr);
			ByDataItemModelFlowSource<MeasureModel> measureResolver = new ByDataItemModelFlowSource<MeasureModel>(measures, mesr);
			dataSourceDimensionResolver.Add(dimensionResolver);
			dataSourceDimensionResolver.Add(measureResolver);
			CriteriaOperator filterCriteria = SliceFlowSourceCreatorCache.GetFilterCriteria(query.SliceQuery.DataSlices.First());
			if(query.RowValues != null)
				foreach(KeyValuePair<DimensionModel, object> pair in query.RowValues)
					filterCriteria = CriteriaOperator.And(filterCriteria, new BinaryOperator(pair.Key.Name, pair.Value));
			if(query.ColumnValues != null)
				foreach(KeyValuePair<DimensionModel, object> pair in query.ColumnValues)
					filterCriteria = CriteriaOperator.And(filterCriteria, new BinaryOperator(pair.Key.Name, pair.Value));
			dataSourceDimensionResolver.PopUp(dimensionResolver);
			dataSourceDimensionResolver.PopUp(measureResolver);
			dataSourceDimensionResolver.ClearCache();
			dimr.ByNameResolvers.AddRange(dataSourceDimensionResolver.ByNameResolvers);
			mesr.ByNameResolvers.AddRange(dataSourceDimensionResolver.ByNameResolvers);
			if(!Object.ReferenceEquals(null, filterCriteria)) {
				DataItemResolveManager filterDimensionResolver = dataSourceDimensionResolver.Clone();
				filterDimensionResolver.Add(new ByDataItemModelFlowSource<DimensionModel>(model.FilterDimensions, dimr));
				SingleFlowOperation filter =
						filterDimensionResolver.GetDataSourceLevelCriteria(
															  CriteriaOperator.ToString(new CriteriaParameterReplacer().Process(filterCriteria)),
															  UnboundColumnType.Boolean
																);
				dataSourceDimensionResolver.Add(new FilteredSingleFlowSource(filter, dataSourceDimensionResolver));
			}
			dataSourceDimensionResolver.PopUp(dimensionResolver);
			dataSourceDimensionResolver.PopUp(measureResolver);
			dataSourceDimensionResolver.ClearCache();
			dimr.ByNameResolvers.Clear();
			mesr.ByNameResolvers.Clear();
			dimr.ByNameResolvers.AddRange(dataSourceDimensionResolver.ByNameResolvers);
			mesr.ByNameResolvers.AddRange(dataSourceDimensionResolver.ByNameResolvers);
			dimr.ClearCache();
			mesr.ClearCache();
			List<DimensionTopNModel> topNModels = model.DimensionsTopN.Where(m => m.TopNEnabled && !m.TopNShowOthers).ToList();
			if(topNModels.Count > 0) {
				TopNCreator topNCreator = new TopNCreator(d => dimr.Resolve(d), measures, mesr);
				SingleFlowOperation filter = TopNOthersFlowResolver.GetTopNFlow(topNModels, topNCreator);
				dataSourceDimensionResolver.Add(new FilteredSingleFlowSource(filter, dataSourceDimensionResolver));
				dimr.ByNameResolvers.Clear();
				mesr.ByNameResolvers.Clear();
				dimr.ByNameResolvers.AddRange(dataSourceDimensionResolver.ByNameResolvers);
				mesr.ByNameResolvers.AddRange(dataSourceDimensionResolver.ByNameResolvers);
				dimr.ClearCache();
				mesr.ClearCache();
			}
			dataSourceDimensionResolver.PopUp(dimensionResolver);
			dataSourceDimensionResolver.PopUp(measureResolver);
			dataSourceDimensionResolver.ClearCache();
		}
	}
	public class AnyDataMemberResolver {
		readonly Dictionary<SliceModel, string> cache = new Dictionary<SliceModel, string>();
		readonly SliceDataQuery query;
		readonly IStorage storage;
		public AnyDataMemberResolver(SliceDataQuery query, IStorage storage) {
			this.query = query;
			this.storage = storage;
		}
		public string GetAnyDataMember(SliceModel model) {
			string any = GetAny(model);
			if(any != null)
				return any;
			foreach(SliceModel smodel in query.DataSlices) {
				any = GetAny(smodel);
				if(!string.IsNullOrEmpty(any))
					return any;
			}
			if(storage.Columns != null && storage.Columns.Count > 0)
				return storage.Columns.First();
			throw new ArgumentException("The datasource contains no columns.");
		}
		string GetAny(SliceModel model) {
			string result;
			if(!cache.TryGetValue(model, out result)) {
				result = GetAnyCore(model);
				cache[model] = result;
			}
			return result;
		}
		static IEnumerable<DimensionModel> GetRowFiltersDimensions(SliceModel model) {
			List<DimensionModel> dims = new List<DimensionModel>();
			RowFiltersModel<DimensionModel>[] rowFilters = model.RowFilters;
			if(rowFilters != null)
				dims.AddRange(rowFilters.SelectMany(f => f.Dimensions).ToArray());
			return dims;
		}
		static string GetAnyCore(SliceModel model) {
			List<DimensionModel> dimensions = model.Dimensions.Concat(model.FilterDimensions).Concat(GetRowFiltersDimensions(model)).Distinct().ToList();
			List<MeasureModel> measures = model.Measures.Concat(model.SummaryAggregations.Select((s) => s.Measure)).Distinct().ToList();
			List<string> ids = dimensions.Select(d => d.Name).Concat(measures.Select(m => m.Name)).ToList();
			DimensionModel dim = dimensions.FirstOrDefault(d => string.IsNullOrEmpty(d.UnboundExpression));
			if(dim != null)
				return dim.DataMember;
			MeasureModel measure = measures.FirstOrDefault(m => string.IsNullOrEmpty(m.UnboundExpression));
			if(measure != null)
				return measure.DataMember;
			string name = GetCalcDataItemDataMember(dimensions, ids);
			if(!string.IsNullOrEmpty(name))
				return name;
			return GetCalcDataItemDataMember(measures, ids);
		}
		static string GetCalcDataItemDataMember<TDataItemModel>(List<TDataItemModel> dims, List<string> ids) where TDataItemModel : DataItemModel<TDataItemModel> {
			foreach(TDataItemModel dimm in dims) {
				if(!string.IsNullOrEmpty(dimm.UnboundExpression)) {
					ColumnNamesCriteriaVisitor visitor = new ColumnNamesCriteriaVisitor(false);
					CriteriaOperator.Parse(dimm.UnboundExpression).Accept(visitor);
					string name = visitor.ColumnNames.FirstOrDefault(n => !ids.Contains(n) && !string.IsNullOrEmpty(n));
					if(!string.IsNullOrEmpty(name))
						return name;
				}
			}
			return null;
		}
	}
}
