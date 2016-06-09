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
using System.Globalization;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.XtraPivotGrid;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class ItemModelBuilder {
		static string AbsoluteVariationPivotFieldName = "{0}-{1}_AbsVar";
		static string PercentVariationPivotFieldName = "{0}-{1}_PerVar";
		static string PercentOfTargetPivotFieldName = "{0}-{1}_PerOfTar";
		static string DeltaMainValuePivotFieldNameFormat = "{0}-{1}_MainValue";
		static string DeltaSubValue1PivotFieldName = "{0}-{1}_SubValue1";
		static string DeltaSubValue2PivotFieldName = "{0}-{1}_SubValue2";
		static string DeltaIsGoodPivotFieldNameFormat = "{0}-{1}_IsGood";
		static string DeltaIndicatorTypePivotFieldNameFormat = "{0}-{1}_IndicatorType";
		static DataFieldType[] nonNumericalTypes = new DataFieldType[] { DataFieldType.Bool, DataFieldType.Custom, DataFieldType.Text, DataFieldType.Unknown };
		readonly Func<DataItem, string> getName;
		readonly DataSourceInfo dataSourceInfo;
		readonly IActualParametersProvider provider;
		public bool MeasureConvertToDecimalForNonNumerical { get; set; }
		IDataSourceSchema DataSourceSchema { get { return dataSourceInfo.GetPickManager(); } }
		string DataMember { get { return dataSourceInfo != null ? dataSourceInfo.DataMember : null; } }
		public ItemModelBuilder(DataSourceInfo dataSourceInfo, Func<DataItem, string> getName, IActualParametersProvider provider) {
			Guard.ArgumentNotNull(provider, "IActualParametersProvider");
			this.dataSourceInfo = dataSourceInfo;
			this.getName = getName;
			this.provider = provider;
			this.MeasureConvertToDecimalForNonNumerical = false;
		}
		public Tuple<DimensionModel, DimensionSortModel, DimensionTopNModel> GetDimensionModel(Dimension dimension, bool createSortAndTopNModel) {
			DXContract.Requires(dimension != null);
			DimensionModel dimensionModel = new DimensionModel(dimension.DataMember, getName(dimension));
			GroupIntervalInfo groupIntervalInfo = GroupIntervalInfo.GetInstance(dimension, DataSourceSchema != null ? DataSourceSchema.GetFieldType(dimension.DataMember) : DataFieldType.Unknown);
			dimensionModel.GroupInterval = groupIntervalInfo != null ? groupIntervalInfo.PivotGroupInterval : PivotGroupInterval.Default;
			CalculatedField calculatedField = GetCalculatedFiled(dimensionModel.DataMember);
			if(calculatedField != null) {
				dimensionModel.DrillDownName = calculatedField.Name;
				dimensionModel.UnboundType = calculatedField.DataType.ToUnboundColumnType();
				dimensionModel.UnboundExpression = GetCalculatedFieldExpression(calculatedField);
			}
			DimensionSortModel sortModel = null;
			DimensionTopNModel topnModel = null;
			if(createSortAndTopNModel) {
				sortModel = new DimensionSortModel(dimensionModel);
				SortingOptions actualSortingOptions = dimension.GetActualSortingOptions();
				DimensionSortOrder actualSortOrder = actualSortingOptions.SortOrder;
				if(actualSortOrder == DimensionSortOrder.None) {
					sortModel.SortMode = PivotSortMode.None;
				} else {
					DimensionSortMode? actualSortMode = actualSortingOptions.SortMode;
					MeasureModel sortByMeasure = actualSortingOptions.SortByMeasure == null ? null : GetMeasureModel(actualSortingOptions.SortByMeasure);
					if(sortByMeasure != null) 
						sortModel.SortByMeasure = new DimensionSortByMeasureModel(sortByMeasure);
					sortModel.SortMode = actualSortMode.HasValue && actualSortMode != DimensionSortMode.Value ? (PivotSortMode)actualSortMode : PivotSortMode.Default;
					sortModel.SortOrder = (PivotSortOrder)actualSortOrder;
				}
				bool enabled = dimension.TopNOptions.Enabled && dimension.TopNOptions.Measure != null;
				if(enabled) {
					topnModel = new DimensionTopNModel(dimensionModel);
					topnModel.TopNEnabled = true;
					topnModel.TopNCount = dimension.TopNOptions.ActualCount;
					topnModel.TopNShowOthers = dimension.TopNOptions.ShowOthers;
					topnModel.TopNMeasure = GetMeasureModel(dimension.TopNOptions.Measure);
					topnModel.TopNDirection = sortModel.SortOrder;
				}
			}
			return new Tuple<DimensionModel, DimensionSortModel, DimensionTopNModel>(dimensionModel, sortModel, topnModel);
		}
		public IEnumerable<DimensionModel> GetDimensionModel(IEnumerable<Dimension> dimensionList, params Dimension[] dimensions) {
			return dimensionList.Concat(dimensions).NotNull().Select(d => GetDimensionModel(d, false).Item1).ToList();
		}
		public MeasureModel GetMeasureModel(Measure measure) {
			DXContract.Requires(measure != null);
			string name = getName(measure);
			if(string.IsNullOrEmpty(name))
				return null;
			bool countD = measure.SummaryType == SummaryType.CountDistinct;
			CalculatedField calcField = GetCalculatedFiled(measure.DataMember);
			MeasureModel measureModel = new MeasureModel(countD || calcField != null ? string.Empty : measure.DataMember, name);
			measureModel.SummaryType = measure.SummaryType;
			measureModel.FieldType = DataSourceSchema == null ? DataFieldType.Unknown : DataSourceSchema.GetFieldType(measure.DataMember);
			measureModel.DecimalSummary = MeasureConvertToDecimalForNonNumerical && nonNumericalTypes.Contains(measureModel.FieldType);
			measureModel.IsVisible = true;
			if(countD) {
				string unboundExpression = calcField == null ? null : GetCalculatedFieldExpression(calcField);
				if(string.IsNullOrEmpty(unboundExpression))
					measureModel.UnboundExpression = new FunctionOperator(FunctionOperatorType.Custom,
						new OperandValue(DashboardDistinctCountFunction.FunctionName),
						new OperandProperty(measure.DataMember)).ToString();
				else
					measureModel.UnboundExpression = string.Format("{0}({1})", DashboardDistinctCountFunction.FunctionName, unboundExpression);
				measureModel.UnboundType = UnboundColumnType.Object;
				measureModel.UnboundMode = ExpressionMode.AggregateFunction;
			} else if(calcField != null) {
				string criteria = GetCalculatedFieldExpression(calcField);
				measureModel.DrillDownName = calcField.Name;
				measureModel.UnboundType = calcField.DataType.ToUnboundColumnType();
				measureModel.UnboundExpression = criteria;
				measureModel.UnboundMode = DevExpress.PivotGrid.CriteriaVisitors.HasAggregateCriteriaChecker.Check(CriteriaOperator.Parse(criteria)) ? ExpressionMode.AggregateFunction : ExpressionMode.DataSourceLevel;
			}
			return measureModel;
		}
		public IEnumerable<MeasureModel> GetMeasureModel(IEnumerable<Measure> measureList, params Measure[] measures) {
			return measureList.Concat(measures).NotNull().Select(d => GetMeasureModel(d)).ToList();
		}
		public IEnumerable<MeasureModel> GetDeltaMeasures(DeltaMeasureInfo delta) {
			Func<string, UnboundColumnType, MeasureModel> createUnbound = (name, type) => new MeasureModel("", name) { UnboundType = type, UnboundMode = ExpressionMode.SummaryLevel};
			DeltaOptions deltaOptions = delta.Options;
			MeasureModel actual = GetMeasureModel(delta.Actual);
			MeasureModel target = GetMeasureModel(delta.Target);
			MeasureModel absVariation = createUnbound(string.Format(AbsoluteVariationPivotFieldName, actual.Name, target.Name), UnboundColumnType.Decimal);
			absVariation.UnboundExpression = string.Format("[{0}]-[{1}]", actual.Name, target.Name);
			MeasureModel percentVariation = createUnbound(string.Format(PercentVariationPivotFieldName, actual.Name, target.Name), UnboundColumnType.Decimal);
			percentVariation.UnboundExpression = string.Format("Iif([{1}]==0,0,[{0}]/[{1}])", absVariation.Name, target.Name);
			MeasureModel percentOfTarget = createUnbound(string.Format(PercentOfTargetPivotFieldName, actual.Name, target.Name), UnboundColumnType.Decimal);
			percentOfTarget.UnboundExpression = string.Format("1+[{0}]", percentVariation.Name);
			MeasureModel mainValue = createUnbound(string.Format(DeltaMainValuePivotFieldNameFormat, actual.Name, target.Name), UnboundColumnType.Decimal);
			MeasureModel subValue1 = createUnbound(string.Format(DeltaSubValue1PivotFieldName, actual.Name, target.Name), UnboundColumnType.Decimal);
			MeasureModel subValue2 = createUnbound(string.Format(DeltaSubValue2PivotFieldName, actual.Name, target.Name), UnboundColumnType.Decimal);
			switch(deltaOptions.ValueType) {
				case DeltaValueType.ActualValue:
					mainValue.UnboundExpression = string.Format("[{0}]", actual.Name);
					subValue1.UnboundExpression = string.Format("[{0}]", absVariation.Name);
					subValue2.UnboundExpression = string.Format("[{0}]", percentVariation.Name);
					break;
				case DeltaValueType.AbsoluteVariation:
					mainValue.UnboundExpression = string.Format("[{0}]", absVariation.Name);
					subValue1.UnboundExpression = string.Format("[{0}]", actual.Name);
					subValue2.UnboundExpression = string.Format("[{0}]", percentVariation.Name);
					break;
				case DeltaValueType.PercentVariation:
					mainValue.UnboundExpression = string.Format("[{0}]", percentVariation.Name);
					subValue1.UnboundExpression = string.Format("[{0}]", actual.Name);
					subValue2.UnboundExpression = string.Format("[{0}]", absVariation.Name);
					break;
				case DeltaValueType.PercentOfTarget:
					mainValue.UnboundExpression = string.Format("[{0}]", percentOfTarget.Name);
					subValue1.UnboundExpression = string.Format("[{0}]", actual.Name);
					subValue2.UnboundExpression = string.Format("[{0}]", absVariation.Name);
					break;
			}
			string comparisonResultExpression;
			if(deltaOptions.ResultIndicationThresholdType == DeltaIndicationThresholdType.Absolute)
				comparisonResultExpression = String.Format("Iif(Abs([{0}]-[{1}]) <= {2},0,Iif([{0}]<[{1}],-1,1))",
					actual.Name, target.Name, deltaOptions.ResultIndicationThreshold.ToString(CultureInfo.InvariantCulture));
			else
				comparisonResultExpression = String.Format("Iif([{1}]!=0,Iif(Abs([{3}]) <= {2},0,Iif([{0}]<[{1}],-1,1)),Iif([{0}] == 0,0,Iif([{0}]<[{1}],-1,1)))",
					actual.Name, target.Name, (deltaOptions.ResultIndicationThreshold / 100).ToString(CultureInfo.InvariantCulture),
					percentVariation.Name);
			MeasureModel isGood = createUnbound(String.Format(DeltaIsGoodPivotFieldNameFormat, actual.Name, target.Name), UnboundColumnType.Boolean);
			MeasureModel indicatorType = createUnbound(String.Format(DeltaIndicatorTypePivotFieldNameFormat, actual.Name, target.Name),
				UnboundColumnType.Integer);
			switch(deltaOptions.ResultIndicationMode) {
				case DeltaIndicationMode.GreaterIsGood:
					isGood.UnboundExpression = String.Format("{0}!=-1", comparisonResultExpression);
					indicatorType.UnboundExpression = String.Format("Iif({0}>0,{1},Iif({0}<0,{2},{3}))",
						comparisonResultExpression, (int)IndicatorType.UpArrow, (int)IndicatorType.DownArrow, (int)IndicatorType.None);
					break;
				case DeltaIndicationMode.LessIsGood:
					isGood.UnboundExpression = String.Format("{0}!= 1", comparisonResultExpression);
					indicatorType.UnboundExpression = String.Format("Iif({0}>0,{1},Iif({0}<0,{2},{3}))",
						comparisonResultExpression, (int)IndicatorType.UpArrow, (int)IndicatorType.DownArrow, (int)IndicatorType.None);
					break;
				case DeltaIndicationMode.WarningIfGreater:
					isGood.UnboundExpression = "True";
					indicatorType.UnboundExpression = String.Format("Iif({0}>0,{1},{2})",
						comparisonResultExpression, (int)IndicatorType.Warning, (int)IndicatorType.None);
					break;
				case DeltaIndicationMode.WarningIfLess:
					isGood.UnboundExpression = "True";
					indicatorType.UnboundExpression = String.Format("Iif({0}<0,{1},{2})",
						comparisonResultExpression, (int)IndicatorType.Warning, (int)IndicatorType.None);
					break;
				default:
					isGood.UnboundExpression = "True";
					indicatorType.UnboundExpression = String.Format("{0}", (int)IndicatorType.None);
					break;
			}
			return new[] { actual, target, 
				absVariation, percentVariation, percentOfTarget, 
				 isGood, indicatorType };
		}
		CalculatedField GetCalculatedFiled(string dataMember) {
			return dataSourceInfo.DataSource == null || dataSourceInfo.DataSource.GetShouldProvideFakeData() || dataSourceInfo.DataSource.CalculatedFields == null ? null : dataSourceInfo.DataSource.CalculatedFields[dataMember];
		}
		string GetCalculatedFieldExpression(CalculatedField calculatedField) {
			return dataSourceInfo.DataSource.GetExpandedCalculatedFieldExpression(calculatedField, DataMember, provider, true);
		}
	}
	static class DataQueryExtensions {
		public static PivotDataQuery ToPivotDataQuery(this SliceDataQuery query) {
			DXContract.Requires(query.DataSlices.Count > 0);
			DXContract.Requires(query.DataSlices.All(s => s.FilterDimensions.SequenceEqual(query.DataSlices[0].FilterDimensions)));
			DXContract.Requires(query.DataSlices.All(s => CriteriaComparer.Default.Equals(s.FilterCriteria, query.DataSlices[0].FilterCriteria)));
			DXContract.Requires(query.DataSlices.SelectMany(s => s.Dimensions).Distinct().Except(query.Axis1.Concat(query.Axis2)).Count() == 0);
			IEnumerable<DimensionTopNModel> topNModels = query.DataSlices.SelectMany(s => s.DimensionsTopN).Distinct();
			IEnumerable<DimensionSortModel> sortModels = query.DataSlices.SelectMany(s => s.DimensionsSort).Distinct();
			IEnumerable<IEnumerable<DimensionModel>> slices = query.DataSlices.Select(slice => slice.Dimensions).ToList();
			PivotDataQuery result = new PivotDataQuery();
			result.Columns.AddRange(query.Axis1);
			result.ColumnsTopN.AddRange(topNModels.Where(topN => result.Columns.Contains(topN.DimensionModel)));
			result.ColumnsSort.AddRange(sortModels.Where(sort => result.Columns.Contains(sort.SortedDimension )));
			result.Rows.AddRange(query.Axis2);
			result.RowsTopN.AddRange(topNModels.Where(topN => result.Rows.Contains(topN.DimensionModel)));
			result.RowsSort.AddRange(sortModels.Where(sort => result.Rows.Contains(sort.SortedDimension )));
			result.Measures.AddRange(query.DataSlices.SelectMany(s => s.Measures).Distinct());
			result.FilterDimensions.AddRange(query.DataSlices[0].FilterDimensions);
			result.FilterCriteria = query.DataSlices[0].FilterCriteria;
			result.ResultSlices.AddRange(query.DataSlices.Select(slice => new ResultSliceModel(slice.Dimensions, slice.Measures, slice.SummaryAggregations)));
			result.ColumnsExpandState = GenerateExpandModel(query.Axis1, query.DataSlices);
			result.RowsExpandState = GenerateExpandModel(query.Axis2, query.DataSlices);
			return result;
		}
		public static UnderlyingDataQuery<PivotDataQuery> ToPivotDataQuery(this UnderlyingDataQuery<SliceDataQuery> query) {
			return new UnderlyingDataQuery<PivotDataQuery>(query.SliceQuery.ToPivotDataQuery(), query.DataMembers, query.RowValues, query.ColumnValues);
		}
		static AxisExpandModel GenerateExpandModel(IEnumerable<DimensionModel> axis, IEnumerable<SliceModel> slices) {
			AxisExpandModel model = AxisExpandModel.FullExpandModel();
			if(axis.Count() > 1) {
				SliceModel secondSlice = slices.FirstOrDefault(s=>Helper.SequenceEqualsAsSet(s.Dimensions, axis.Take(2)));
				if(secondSlice != null) {
					ExpandAction expandAction = secondSlice.RowFilters == null || secondSlice.RowFilters != null && secondSlice.RowFilters[0].FilterType == RowFiltersType.Exclude ? ExpandAction.Collapse : ExpandAction.Expand;
					model = new AxisExpandModel(expandAction);
					if(expandAction == ExpandAction.Collapse) {
						SliceModel lastSlice = slices.First(s => Helper.SequenceEqualsAsSet(s.Dimensions, axis));
						if(lastSlice.RowFilters != null) {
							List<RowFiltersModel<DimensionModel>> rowFilters = lastSlice.RowFilters.OrderByDescending(filters => filters.Dimensions.Count).ToList();
							foreach(RowFiltersModel<DimensionModel> filters in rowFilters) {
								model.Values.AddRange(filters.Values);
							}
						}
					} else {
						for(int i = 1; i < axis.Count(); i++) {
							SliceModel slice = slices.First(s => Helper.SequenceEqualsAsSet(s.Dimensions, axis.Take(i + 1)));
							if(slice.RowFilters != null) {
								RowFiltersModel<DimensionModel> filters = slice.RowFilters[0];
								model.Values.AddRange(filters.Values);
							}
						}
					}
				}
			}
			return model;
		}
	}
	public class DeltaMeasureInfo {
		public Measure Actual { get; private set; }
		public Measure Target { get; private set; }
		public DeltaOptions Options { get; private set; }
		public DeltaMeasureInfo(Measure actual, Measure target, DeltaOptions options) {
			this.Actual = actual;
			this.Target = target;
			this.Options = options;
		}
	}
	class SliceDataQueryBuilder {
		class DimensionModelsContainer {
			public IList<DimensionModel> DimensionModels { get; private set; }
			public IList<DimensionSortModel> SortModels { get; private set; }
			public IList<DimensionTopNModel> TopNModels { get; private set; }
			public static DimensionModelsContainer Create(ItemModelBuilder builder, IEnumerable<Dimension> dimesnions) {
				DimensionModelsContainer container = new DimensionModelsContainer();
				container.DimensionModels = new List<DimensionModel>();
				container.SortModels = new List<DimensionSortModel>();
				container.TopNModels = new List<DimensionTopNModel>();
				List<DimensionModel> models = dimesnions.Select((dimension) => {
														 var tuple = builder.GetDimensionModel(dimension, true);
														 container.DimensionModels.Add(tuple.Item1);
														 if(tuple.Item2 != null)
															 container.SortModels.Add(tuple.Item2);
														 if(tuple.Item3 != null)
															 container.TopNModels.Add(tuple.Item3);
														 return tuple.Item1;
				}).ToList();
				for(int i = 0; i < models.Count; i++) {
					DimensionSortByMeasureModel sortModel = container.SortModels[i].SortByMeasure;
					if(sortModel != null)
						sortModel.Dimensions = models.Take(i + 1).ToList();
				}
				return container;
			}
		}
		readonly ItemModelBuilder builder;
		IList<DimensionModel> filterDimensions;
		CriteriaOperator filter;
		SliceDataQuery query = new SliceDataQuery();
		public IEnumerable<DimensionModel> Axis1 { get { return query.Axis1; } }
		public IEnumerable<DimensionModel> Axis2 { get { return query.Axis2; } }
		SliceDataQueryBuilder(ItemModelBuilder builder, IEnumerable<Dimension> filterDimensions, CriteriaOperator filter) {
			this.builder = builder;
			this.filterDimensions = filterDimensions.Select(d => builder.GetDimensionModel(d, false).Item1).ToList();
			this.filter = filter;
		}
		SliceDataQueryBuilder(ItemModelBuilder builder, IEnumerable<Dimension> filterDimensions, CriteriaOperator filter, SliceDataQuery query)
			: this(builder, filterDimensions, filter) {
			this.query = query;
		}
		public SliceModel AddSlice(IEnumerable<Dimension> dimensions, IEnumerable<Measure> measures) {
			return AddSlice(dimensions, measures, new DeltaMeasureInfo[0], null);
		}
		public SliceModel AddSlice(IEnumerable<Dimension> dimensions, IEnumerable<Measure> measures, IEnumerable<DeltaMeasureInfo> deltaMeasures) {
			return AddSlice(dimensions, measures, deltaMeasures, null);
		}
		public SliceModel AddSlice(IEnumerable<Dimension> dimensions, IEnumerable<Measure> measures, IEnumerable<DeltaMeasureInfo> deltaMeasures, RowFiltersModel<Dimension>[] rowFilters) {
			SliceModel slice = CreateSlice(dimensions, measures, deltaMeasures, rowFilters);
			SliceModel existingSlice = query.DataSlices.FirstOrDefault(sm => Helper.SequenceEqualsAsSet(slice.Dimensions, sm.Dimensions));
			if(existingSlice != null)
				existingSlice.Measures = existingSlice.Measures.Concat(slice.Measures).Distinct().ToList();
			else
				query.DataSlices.Add(slice);
			return slice;
		}
		public void SetAxes(IEnumerable<Dimension> axis1, IEnumerable<Dimension> axis2) {
			query.Axis1 = builder.GetDimensionModel(axis1);
			query.Axis2 = builder.GetDimensionModel(axis2);
			Action<IEnumerable<Dimension>> addAxisSlice = (axis) => {
				SliceModel axis1Slice = CreateSlice(axis, new Measure[0], new DeltaMeasureInfo[0], null);
				HashSet<DimensionModel> axisSliceSignature = new HashSet<DimensionModel>(axis1Slice.Dimensions);
				if(!query.DataSlices.Any(s => axisSliceSignature.SetEquals(s.Dimensions)))
					query.DataSlices.Add(axis1Slice);
			};
			if(axis1.Count() > 0)
				addAxisSlice(axis1);
			if(axis2.Count() > 0)
				addAxisSlice(axis2);
		}
		public void AddSliceAggregation(string name, Dimension axis1dimension, Dimension axis2dimension, Measure measure, IEnumerable<SummaryItemTypeEx> types, decimal argument) {
			MeasureModel measureModel = builder.GetMeasureModel(measure);
			Func<SummaryItemTypeEx, SummaryAggregationModel> createModel = (type) => new SummaryAggregationModel(name, measureModel, type, argument);
			DimensionModel model1 = axis1dimension == null ? null : builder.GetDimensionModel(axis1dimension, false).Item1;
			DimensionModel model2 = axis2dimension == null ? null : builder.GetDimensionModel(axis2dimension, false).Item1;
			IEnumerable<DimensionModel> axisComponent1 = query.Axis1.Take(query.Axis1.IndexOf(model1) + 1).ToList();
			IEnumerable<DimensionModel> axisComponent2 = query.Axis2.Take(query.Axis2.IndexOf(model2) + 1).ToList();
			IEnumerable<DimensionModel> sliceDimensions = axisComponent1.Concat(axisComponent2).ToList();
			AddSliceAggregationCore(types, createModel, sliceDimensions);
		}
		public void AddSliceAggregation(string name, Dimension targetDimension, IEnumerable<SummaryItemTypeEx> types, decimal argument) {
			DimensionModel dimensionModel = builder.GetDimensionModel(targetDimension, false).Item1;
			Func<SummaryItemTypeEx, SummaryAggregationModel> createModel = (type) => new SummaryAggregationModel(name, dimensionModel, type, argument);
			AddSliceAggregationCore(types, createModel, query.Axis1.Concat(query.Axis2));
		}
		void AddSliceAggregationCore(IEnumerable<SummaryItemTypeEx> types, Func<SummaryItemTypeEx, SummaryAggregationModel> createModel, IEnumerable<DimensionModel> sliceDimensions) {
			IList<SummaryAggregationModel> aggModels = types.Select(createModel).ToList();
			SliceModel slice = query.DataSlices.FirstOrDefault(s => s.Dimensions.SequenceEqual(sliceDimensions));
			if(slice == null) {
				slice = new SliceModel {
					Dimensions = sliceDimensions.ToArray(),
					FilterDimensions = filterDimensions,
					FilterCriteria = filter
				};
				query.DataSlices.Add(slice);
			}
			slice.SummaryAggregations.AddRange(aggModels);
		}
		public SliceDataQuery FinalQuery() {
			IEnumerable<SliceModel> resultSlices = query.DataSlices.Where(slice => !(slice.Dimensions.Count == 0 && slice.Measures.Count == 0 && slice.SummaryAggregations.Count == 0)).ToList();
			IEnumerable<DimensionModel> allDimensions = resultSlices.SelectMany(sl => sl.Dimensions);
			List<DimensionTopNModel> allThroughtsTopNs = query.DataSlices.SelectMany(s => s.DimensionsTopN).Where(t => t.TopNEnabled && !t.TopNShowOthers).Distinct().ToList();
			foreach(SliceModel model in resultSlices) {
				if(allThroughtsTopNs.Count > 0) {
					List<DimensionTopNModel> tops = allThroughtsTopNs.Concat(model.DimensionsTopN).Distinct().ToList();
					model.DimensionsTopN.Clear();
					model.DimensionsTopN.AddRange(tops);
				}
				if(model.Dimensions.Count == 0)
					continue;
				bool axisable = true;
				List<DimensionModel> axisDimensions = (query.Axis1.Contains(model.Dimensions[0]) ? query.Axis1 : query.Axis2).ToList();
				for(int i = 0; i < model.Dimensions.Count; i++) {
					if(axisDimensions.Count <= i || model.Dimensions[i] != axisDimensions[i]) {
						axisable = false;
						break;
					}
				}
				if(!axisable)
					model.DimensionsSort.Clear();
				CriteriaOperator criteria = model.FilterCriteria;
				if(!Object.ReferenceEquals(null, criteria)) {
					ColumnNamesCriteriaVisitor visitor = new ColumnNamesCriteriaVisitor(false);
					criteria.Accept(visitor);
					foreach(string name in visitor.ColumnNames) {
						if(model.FilterDimensions.FirstOrDefault(d => d.Name == name) == null) {
							DimensionModel filterDimension = allDimensions.FirstOrDefault(d => d.Name == name);
							if(filterDimension != null)
								model.FilterDimensions.Add(allDimensions.FirstOrDefault(d => d.Name == name));
						}
					}
				}
			}
			query.DataSlices.Clear();
			query.DataSlices.AddRange(resultSlices);
			SliceDataQuery finalQuery = query;
			query = null; 
			return finalQuery;
		}
		SliceModel CreateSlice(IEnumerable<Dimension> dimensions, IEnumerable<Measure> measures, IEnumerable<DeltaMeasureInfo> deltaMeasures, RowFiltersModel<Dimension>[] rowFilters) {
			DXContract.Requires(dimensions.All(d => d != null));
			DXContract.Requires(measures.All(m => m != null));
			DXContract.Requires(deltaMeasures.All(m => m != null));
			IList<MeasureModel> singleMeasureModels = builder.GetMeasureModel(measures).ToList();
			IList<MeasureModel> deltaMeasureModels = deltaMeasures.SelectMany(delta => {
				if(delta.Actual != null && delta.Target != null)
					return builder.GetDeltaMeasures(delta);
				else if((delta.Actual ?? delta.Target) != null)
					return new[] { builder.GetMeasureModel(delta.Actual ?? delta.Target) };
				else
					return new MeasureModel[0];
			}).NotNull().ToList();
			DimensionModelsContainer mainDimensionModels = DimensionModelsContainer.Create(builder, dimensions);
			IList<MeasureModel> measureModels = singleMeasureModels.Concat(deltaMeasureModels).ToList();
			SliceModel slice = new SliceModel {
				Dimensions = mainDimensionModels.DimensionModels,
				Measures = measureModels,
				FilterDimensions = filterDimensions,
				FilterCriteria = filter
			};
			slice.DimensionsSort.AddRange(mainDimensionModels.SortModels);
			slice.DimensionsTopN.AddRange(mainDimensionModels.TopNModels);
			if(rowFilters != null) {
				slice.RowFilters = rowFilters.Select(rf =>
					new RowFiltersModel<DimensionModel> {
						Dimensions = DimensionModelsContainer.Create(builder, rf.Dimensions).DimensionModels,
						FilterType = rf.FilterType,
						Values = rf.Values
					}).ToArray();
			}
			return slice;
		}
		public static SliceDataQueryBuilder CreateWithQuery(ItemModelBuilder builder, IEnumerable<Dimension> filterDimensions, CriteriaOperator filter, SliceDataQuery query) {
			return new SliceDataQueryBuilder(builder, filterDimensions, filter, query);
		}
		public static SliceDataQueryBuilder CreateEmpty(ItemModelBuilder builder, IEnumerable<Dimension> filterDimensions, CriteriaOperator filter) {
			return new SliceDataQueryBuilder(builder, filterDimensions, filter);
		}
		public static SliceDataQueryBuilder CreateWithPivotModel(ItemModelBuilder builder, IEnumerable<Dimension> axis1, IEnumerable<Dimension> axis2, IEnumerable<Measure> measures,
			IEnumerable<Dimension> filterDimensions, CriteriaOperator filter) {
				return CreateWithPivotModel(builder, new PivotAxisModel<Dimension> { Dimensions = axis1 }, new PivotAxisModel<Dimension> { Dimensions = axis2 }, measures, new DeltaMeasureInfo[0], filterDimensions, filter);
		}
		public static SliceDataQueryBuilder CreateWithPivotModel(ItemModelBuilder builder, IEnumerable<Dimension> axis1, IEnumerable<Dimension> axis2, IEnumerable<Measure> measures,
			IEnumerable<DeltaMeasureInfo> deltaMeasures, IEnumerable<Dimension> filterDimensions, CriteriaOperator filter) {
				return CreateWithPivotModel(builder, new PivotAxisModel<Dimension> { Dimensions = axis1 }, new PivotAxisModel<Dimension> { Dimensions = axis2 }, measures, deltaMeasures, filterDimensions, filter);
		}
		public static SliceDataQueryBuilder CreateWithPivotModel(ItemModelBuilder builder, PivotAxisModel<Dimension> axis1, PivotAxisModel<Dimension> axis2, IEnumerable<Measure> measures,
			IEnumerable<DeltaMeasureInfo> deltaMeasures, IEnumerable<Dimension> filterDimensions, CriteriaOperator filter) {
			SliceDataQueryBuilder queryBuilder = SliceDataQueryBuilder.CreateEmpty(builder, filterDimensions, filter);
			var slices = PivotToSliceHelper<Dimension>.PivotModelToSlices(axis1, axis2);
			foreach(var slice in slices) {
				SliceModel sliceModel = queryBuilder.AddSlice(slice.Dimensions, measures, deltaMeasures, slice.RowFiltersModel);
			}
			queryBuilder.SetAxes(axis1.Dimensions, axis2.Dimensions);
			return queryBuilder;
		}
	}
}
