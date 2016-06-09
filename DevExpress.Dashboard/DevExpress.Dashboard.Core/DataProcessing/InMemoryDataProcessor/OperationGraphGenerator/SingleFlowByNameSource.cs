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
using DevExpress.PivotGrid.ServerMode;
using DevExpress.Data.PivotGrid;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public abstract class SingleFlowByNameSource {
		readonly NullableDictionary<string, SingleFlowOperation> opCache = new NullableDictionary<string, SingleFlowOperation>();
		protected SingleFlowByNameSource() {
		}
		protected abstract SingleFlowOperation ResolveCore(string name);
		public SingleFlowOperation Resolve(string name) {
			SingleFlowOperation op;
			if(!opCache.TryGetValue(name, out op)) {
				op = ResolveCore(name);
				opCache[name] = op;
			} else {
				return op;
			}
			return op;
		}
		public abstract SingleFlowOperation GetAnyFlow();
		public virtual void ClearCache() {
			opCache.Clear();
			criteriaCache.Clear();
		}
		readonly NullableDictionary<CriteriaOperator, SingleFlowOperation> criteriaCache = new NullableDictionary<CriteriaOperator, SingleFlowOperation>();
		public SingleFlowOperation GetDataSourceLevelCriteria(string expression, UnboundColumnType type) {
			CriteriaOperator op = ColumnCriteriaHelper.WrapToType(CriteriaOperator.Parse(expression), type);
			SingleFlowOperation result;
			if(!criteriaCache.TryGetValue(op, out result)) {
				result = DataSourceLevelProjectCreator.Process(op, this);
				criteriaCache[op] = result;
			}
			return result;
		}
	}
	public abstract class ByDataItemModelFlowSourceBase<TDataItemModel> where TDataItemModel : DataItemModel<TDataItemModel> {
		readonly DataItemResolveManager<TDataItemModel> manager;
		readonly NullableDictionary<TDataItemModel, SingleFlowOperation> opCache = new NullableDictionary<TDataItemModel, SingleFlowOperation>();
		protected DataItemResolveManager<TDataItemModel> Manager { get { return manager; } }
		public ByDataItemModelFlowSourceBase(DataItemResolveManager<TDataItemModel> manager) {
			this.manager = manager;
		}
		public SingleFlowOperation Resolve(TDataItemModel model) {
			SingleFlowOperation op;
			if(!opCache.TryGetValue(model, out op)) {
				op = ResolveCore(model);
				opCache[model] = op;
			} else {
				return op;
			}
			return op;
		}
		protected abstract SingleFlowOperation ResolveCore(TDataItemModel model);
		public abstract ByDataItemModelFlowSourceBase<TDataItemModel> Clone(DataItemResolveManager<TDataItemModel> clone);
		internal void ClearCache() {
			opCache.Clear();
		}
	}
	public class DataItemResolveManager : SingleFlowByNameSource {
		List<SingleFlowByNameSource> byNameResolvers = new List<SingleFlowByNameSource>();
		public List<SingleFlowByNameSource> ByNameResolvers { get { return byNameResolvers; } }
		public DataItemResolveManager() : base() { }
		protected override SingleFlowOperation ResolveCore(string name) {
			return Resolve(name, byNameResolvers.Count - 1);
		}
		public SingleFlowOperation Resolve(string name, SingleFlowByNameSource source) {
			return Resolve(name, byNameResolvers.IndexOf(source) - 1);
		}
		SingleFlowOperation Resolve(string name, int startIndex) {
			if(startIndex >= 0)
				for(int i = startIndex; i >= 0; i--) {
					SingleFlowOperation result = byNameResolvers[i].Resolve(name);
					if(result != null)
						return result;
				}
			throw new ArgumentOutOfRangeException(name);
		}
		public void Add(SingleFlowByNameSource resolver) {
			byNameResolvers.Add(resolver);
			ClearCache();
		}
		public override SingleFlowOperation GetAnyFlow() {
			return GetAnyFlow(byNameResolvers.Count - 1);
		}
		private SingleFlowOperation GetAnyFlow(int startIndex) {
			if(startIndex >= 0)
				for(int i = startIndex; i >= 0; i--) {
					SingleFlowOperation result = byNameResolvers[i].GetAnyFlow();
					if(result != null)
						return result;
				}
			throw new ArgumentOutOfRangeException();
		}
		public SingleFlowOperation GetAnyFlow(SingleFlowByNameSource resolver) {
			return GetAnyFlow(byNameResolvers.IndexOf(resolver) - 1);
		}
		public DataItemResolveManager Clone() {
			DataItemResolveManager result = new DataItemResolveManager();
			result.byNameResolvers.AddRange(byNameResolvers);
			return result;
		}
		public void PopUp(SingleFlowByNameSource resolver) {
			byNameResolvers.Remove(resolver);
			byNameResolvers.Add(resolver);
			ClearCache();
		}
		public override void ClearCache() {
			base.ClearCache();
			foreach(SingleFlowByNameSource resolverCore in byNameResolvers)
				resolverCore.ClearCache();
		}
	}
	public class DataItemResolveManager<TDataItemModel> : DataItemResolveManager where TDataItemModel : DataItemModel<TDataItemModel> {
		List<ByDataItemModelFlowSourceBase<TDataItemModel>> byDataItemResolvers = new List<ByDataItemModelFlowSourceBase<TDataItemModel>>();
		public List<ByDataItemModelFlowSourceBase<TDataItemModel>> ByDataItemResolvers { get { return byDataItemResolvers; } }
		public DataItemResolveManager() : base() { }
		public SingleFlowOperation Resolve(TDataItemModel dataItem) {
			return Resolve(dataItem, byDataItemResolvers.Count - 1);
		}
		public SingleFlowOperation Resolve(TDataItemModel dataItem, ByDataItemModelFlowSourceBase<TDataItemModel> source) {
			return Resolve(dataItem, byDataItemResolvers.IndexOf(source) - 1);
		}
		public SingleFlowOperation Resolve(TDataItemModel dataItem, int startIndex) {
			for(int i = startIndex; i >= 0; i--) {
				SingleFlowOperation result = byDataItemResolvers[i].Resolve(dataItem);
				if(result != null)
					return result;
			}
			throw new ArgumentOutOfRangeException(dataItem.Name);
		}
		public void Add(ByDataItemModelFlowSourceBase<TDataItemModel> resolver) {
			byDataItemResolvers.Add(resolver);
			ClearCache();
		}
		public override void ClearCache() {
			base.ClearCache();
			foreach(ByDataItemModelFlowSourceBase<TDataItemModel> resolverCore in byDataItemResolvers)
				resolverCore.ClearCache();
		}
		public new DataItemResolveManager<TDataItemModel> Clone() {
			DataItemResolveManager<TDataItemModel> clone = new DataItemResolveManager<TDataItemModel>();
			foreach(ByDataItemModelFlowSourceBase<TDataItemModel> resolver in ByDataItemResolvers)
				clone.ByDataItemResolvers.Add(resolver.Clone(clone));
			clone.ByNameResolvers.AddRange(ByNameResolvers);
			return clone;
		}
	}
	class DimensionToFlow : ByDataItemModelFlowSourceBase<DimensionModel> {
		public DimensionToFlow(DataItemResolveManager<DimensionModel> manager) : base(manager) { }
		protected override SingleFlowOperation ResolveCore(DimensionModel model) {
			return DataItemModelToFlowHelper.CreateDimensionFlow(model, Manager);
		}
		public override ByDataItemModelFlowSourceBase<DimensionModel> Clone(DataItemResolveManager<DimensionModel> clone) {
			return new DimensionToFlow(clone);
		}
	}
	class MeasureToFlow : ByDataItemModelFlowSourceBase<MeasureModel> {
		public MeasureToFlow(DataItemResolveManager<MeasureModel> manager)
			: base(manager) {
		}
		protected override SingleFlowOperation ResolveCore(MeasureModel model) {
			if(model.UnboundMode != ExpressionMode.DataSourceLevel)
				throw new InvalidOperationException(model.UnboundMode.ToString());
			return DataItemModelToFlowHelper.CreateMeasureFlow(model, false, null, Manager);
		}
		public override ByDataItemModelFlowSourceBase<MeasureModel> Clone(DataItemResolveManager<MeasureModel> clone) {
			return new MeasureToFlow(clone);
		}
	}
	class FilteredSingleFlowSource : SingleFlowByNameSource {
		SingleFlowOperation filter;
		DataItemResolveManager source;
		public FilteredSingleFlowSource(SingleFlowOperation filter, DataItemResolveManager source)
			: base() {
			this.filter = filter;
			this.source = source;
		}
		protected override SingleFlowOperation ResolveCore(string name) {
			return new Select(source.Resolve(name, this), filter);
		}
		public override SingleFlowOperation GetAnyFlow() {
			return new Select(source.GetAnyFlow(this), filter);
		}
	}
	public class StorageSingleFlowSource : SingleFlowByNameSource {
		readonly IStorage storage;
		readonly SliceModelWrapper slice;
		readonly AnyDataMemberResolver anyResolver;
		public StorageSingleFlowSource(IStorage storage, SliceModelWrapper slice, AnyDataMemberResolver anyResolver)
			: base() {
			this.storage = storage;
			this.slice = slice;
			this.anyResolver = anyResolver;
		}
		protected override SingleFlowOperation ResolveCore(string name) {
			return new Scan(name, storage);
		}
		public override SingleFlowOperation GetAnyFlow() {
			return Resolve(anyResolver.GetAnyDataMember(slice.Model));
		}
	}
	public class ByDataItemModelFlowSource<TDataItemModel> : SingleFlowByNameSource where TDataItemModel : DataItemModel<TDataItemModel> {
		readonly IEnumerable<TDataItemModel> dataItemModels;
		readonly Stack<string> stack = new Stack<string>();
		readonly DataItemResolveManager manager;
		protected DataItemResolveManager Manager { get { return manager; } }
		public ByDataItemModelFlowSource(IEnumerable<TDataItemModel> dataItemModels, DataItemResolveManager<TDataItemModel> manager)
			: base() {
			this.dataItemModels = dataItemModels;
			this.manager = manager;
		}
		protected override SingleFlowOperation ResolveCore(string name) {
			TDataItemModel dataItemModel = dataItemModels.FirstOrDefault(d => d.Name == name || d.DrillDownName == name);
			if(dataItemModel != null) {
				if(stack.Contains(name))
					throw new ArgumentException("recursive: " + string.Join(",", stack.ToArray()));
				stack.Push(name);
				SingleFlowOperation result = ((DataItemResolveManager<TDataItemModel>)manager).Resolve(dataItemModel);
				stack.Pop();
				return result;
			}
			return null;
		}
		public override SingleFlowOperation GetAnyFlow() {
			return null;
		}
	}
	public class SummaryMeasureFlowSource : SingleFlowByNameSource {
		readonly IList<MeasureModel> sliceMeasures;
		readonly Group group;
		readonly SingleFlowByNameSource dataSourceMeasureSource;
		readonly ScanBuffer groupIndexes;
		public SummaryMeasureFlowSource(IList<MeasureModel> sliceMeasures, Group group, SingleFlowByNameSource dataSourceMeasureSource)
			: base() {
			this.sliceMeasures = sliceMeasures;
			this.group = group;
			if(group != null && group.Dimensions != null && group.Dimensions.Any(g => true)) {
				ExtractIndexes extractIndexes = new ExtractIndexes(group);
				groupIndexes = new ScanBuffer(extractIndexes);
			} else
				groupIndexes = null;
			this.dataSourceMeasureSource = dataSourceMeasureSource;
		}
		public SingleFlowOperation Resolve(MeasureModel measure) {
			return DataItemModelToFlowHelper.CreateMeasureFlow(measure, true, this, dataSourceMeasureSource);
		}
		protected override SingleFlowOperation ResolveCore(string name) {
			return Resolve(sliceMeasures.Where(m => m.Name == name).First());
		}
		public override SingleFlowOperation GetAnyFlow() {
			if(group.Dimensions.Count() == 0)
				return dataSourceMeasureSource.GetAnyFlow();
			else
				return new Extract(new MultiScanBuffer(group), 0);
		}
		Dictionary<Tuple<SingleFlowOperation, SummaryType>, SingleFlowOperation> summaryCache = new Dictionary<Tuple<SingleFlowOperation, SummaryType>, SingleFlowOperation>();
		public SingleFlowOperation CreateAggregate(SingleFlowOperation flow, SummaryType summaryType) {
			Tuple<SingleFlowOperation, SummaryType> key = new Tuple<SingleFlowOperation, SummaryType>(flow, summaryType);
			SingleFlowOperation result;
			if(!summaryCache.TryGetValue(key, out result)) {
				result = DataItemModelToFlowHelper.CreateAggregate(groupIndexes, flow, summaryType);
				summaryCache[key] = result;
			}
			return result;
		}
	}
	public class TopNCreator {
		readonly SingleFlowByNameSource dataSourceMeasureResolver;
		readonly IList<MeasureModel> sliceMeasures;
		readonly Func<DimensionModel, SingleFlowOperation> resolveDimensionFlow;
		readonly Dictionary<DimensionTopNModel, SingleFlowOperation> cache = new Dictionary<DimensionTopNModel,SingleFlowOperation>();
		public TopNCreator(Func<DimensionModel, SingleFlowOperation> resolveDimensionFlow, IList<MeasureModel> sliceMeasures, SingleFlowByNameSource dataSourceMeasureResolver) {
			this.sliceMeasures = sliceMeasures;
			this.dataSourceMeasureResolver = dataSourceMeasureResolver;
			this.resolveDimensionFlow = resolveDimensionFlow;
		}
		public SingleFlowOperation GetBitFlow(DimensionTopNModel topNModel) {
			SingleFlowOperation result;
			if(!cache.TryGetValue(topNModel, out result)) {
				result = GetBitFlowCore(topNModel);
				cache[topNModel] = result;
			}
			return result;			
		}
		private SingleFlowOperation GetBitFlowCore(DimensionTopNModel topNModel) {
			SingleFlowOperation itemDataSourceFlow = resolveDimensionFlow(topNModel.DimensionModel);
			Group group = new Group(new SingleFlowOperation[] { itemDataSourceFlow });
			SummaryMeasureFlowSource resolveMeasure = new SummaryMeasureFlowSource(sliceMeasures, group, dataSourceMeasureResolver);
			SingleFlowOperation aggregateFlow = DataItemModelToFlowHelper.CreateMeasureFlow(topNModel.TopNMeasure, true, resolveMeasure, dataSourceMeasureResolver);
			TopN topNFlow = new TopN(aggregateFlow, topNModel.TopNCount, topNModel.TopNDirection == XtraPivotGrid.PivotSortOrder.Ascending ? TopNMode.Bottom : TopNMode.Top);
			Join toppedJoin = new Join(
										  new SingleFlowOperation[] { itemDataSourceFlow },
										  new SingleBlockOperation[] { new Buffer(new Extract(new MultiScanBuffer(group), 0)) },
										  new SingleBlockOperation[] { topNFlow });
			SingleFlowOperation filteredFlow = new Extract(toppedJoin, 0);
			return filteredFlow;
		}
	}
	class TopNOthersFlowResolver : ByDataItemModelFlowSourceBase<DimensionModel> {
		public static SingleFlowOperation GetTopNFlow(List<DimensionTopNModel> topNModels, TopNCreator topNCreator) {
			SingleFlowOperation[] bitFlows = new SingleFlowOperation[topNModels.Count];
			for(int i = 0; i < topNModels.Count; i++) {
				DimensionTopNModel topNModel = topNModels[i];
				SingleFlowOperation filteredFlow = topNCreator.GetBitFlow(topNModel);
				bitFlows[i] = filteredFlow;
			}
			SingleFlowOperation filter;
			if(bitFlows.Length == 1)
				filter = bitFlows[0];
			else
				filter = new Project(string.Join(" And ", Enumerable.Range(0, bitFlows.Length).Select(s => string.Format("[v{0}]", s)).ToArray()), bitFlows);
			return filter;
		}
		readonly List<DimensionTopNModel> topNModels;
		readonly TopNCreator topNCreator;
		public TopNOthersFlowResolver(DataItemResolveManager<DimensionModel> manager, List<DimensionTopNModel> topNModels, TopNCreator topNCreator)
			: base(manager) {
			this.topNModels = topNModels;
			this.topNCreator = topNCreator;
		}
		protected override SingleFlowOperation ResolveCore(DimensionModel model) {
			DimensionTopNModel topNModel = topNModels.FirstOrDefault(m => m.DimensionModel.Name == model.Name);
			SingleFlowOperation source = Manager.Resolve(model, this);
			return CreateToppedFlow(topNModel, source);
		}
		SingleFlowOperation CreateToppedFlow(DimensionTopNModel topNModel, SingleFlowOperation source) {
			if(topNModel != null) {
				SingleFlowOperation bitFlow = topNCreator.GetBitFlow(topNModel);
				return new ScanBuffer(new Buffer(new ProjectOthers(source, bitFlow, false)));
			} else
				return source;
		}
		public override ByDataItemModelFlowSourceBase<DimensionModel> Clone(DataItemResolveManager<DimensionModel> clone) {
			return new TopNOthersFlowResolver(clone, topNModels, topNCreator);
		}
	}
}
