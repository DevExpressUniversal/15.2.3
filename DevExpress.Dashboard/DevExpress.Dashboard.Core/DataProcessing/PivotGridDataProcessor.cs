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
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System.ComponentModel;
using DevExpress.Compatibility.System;
namespace DevExpress.DashboardCommon.DataProcessing {
	public static class DataProcessorExtensions {
		public static DataQueryResult GetData<TQuery>(this IDataProcessor<TQuery> dataProcessor, TQuery query) {
			return dataProcessor.GetData(query, CancellationToken.None);
		}
		public static DashboardUnderlyingDataSet GetUnderlyingData<TQuery>(this IDataProcessor<TQuery> dataProcessor, UnderlyingDataQuery<TQuery> query) {
			return dataProcessor.GetUnderlyingData(query);
		}
	}
	public interface IDataProcessor<TQuery> {
		DataQueryResult GetData(TQuery query, CancellationToken cancellationToken);
		DashboardUnderlyingDataSet GetUnderlyingData(UnderlyingDataQuery<TQuery> query);
	}
	public static class PivotGridDataProcessorExtentions {
		public static UnboundExpressionMode ToPivotMode(this ExpressionMode level) {
			switch(level) {
				case ExpressionMode.DataSourceLevel:
					return UnboundExpressionMode.DataSource;
				case ExpressionMode.SummaryLevel:
					return UnboundExpressionMode.UseSummaryValues;
				case ExpressionMode.AggregateFunction:
					return UnboundExpressionMode.UseAggregateFunctions;
			}
			throw new DashboardInternalException("Unexpected ExpressionLevel");
		}
	}
	class PivotGridDataProcessor : IDataProcessor<PivotDataQuery>, IDisposable {
		class SummaryAggregationAcessor {
			public string Name { get; set; }
			public IList<DimensionModel> Slice { get; set; }
			public SummaryAggregationModel SummaryAggregationModel { get; set; }
			public object Value { get; set; }
		}
		DataSourceModel dataSource;
		PivotGridData pivot;
		PivotDataQuery currentState;
		IList<SummaryAggregationAcessor> summaryAggregationData = new List<SummaryAggregationAcessor>();
		public PivotGridDataProcessor(DataSourceModel dataSource) {
			DXContract.Requires(dataSource != null);
			this.dataSource = dataSource;
			this.currentState = new PivotDataQuery();
			pivot = new PivotGridData();
			pivot.OLAPDataProvider = OLAPDataProvider.Adomd;
			pivot.OlapEnableUnboundFields = true;
			pivot.OptionsOLAP.FilterByUniqueName = true;
			pivot.OptionsView.ShowAllTotals();
			pivot.OptionsView.ShowTotalsForSingleValues = true;
			pivot.OptionsView.ShowGrandTotalsForSingleValues = true;
			pivot.OptionsDataField.AreaIndex = 0;
			pivot.OptionsData.DataFieldUnboundExpressionMode = DataFieldUnboundExpressionMode.UseSummaryValues;
			pivot.OptionsOLAP.UsePrefilter = true;
			pivot.OptionsOLAP.AllowDuplicatedMeasures = true;
			pivot.OptionsOLAP.UseDefaultMeasure = true;
			PivotDataEventsImplementor eventsImplementor = new PivotDataEventsImplementor(pivot);
			pivot.EventsImplementor = eventsImplementor;
			SetPivotDataDataSource(dataSource);
		}
		public DataQueryResult GetData(PivotDataQuery query, CancellationToken cancellationToken) {
			pivot.CancellationToken = cancellationToken;
			ValidateQuery(query);
			IEnumerable<PivotDataQuery.EquatableIndicator> diff = currentState.QualitativeEquals(query);
			bool needSetupPivot = diff.ContainsAny(PivotDataQuery.EquatableIndicator.DataSchema, PivotDataQuery.EquatableIndicator.Filters);
			bool needApplyExpand = needSetupPivot || diff.Contains(PivotDataQuery.EquatableIndicator.ExpandState);
			if(needSetupPivot)
				SetupPivot(query);
			if(needApplyExpand)
				ApplyExpand(query.ColumnsExpandState, query.RowsExpandState);
			currentState = query;
			return new DataQueryResult(
				GetHierarchicalData(pivot, query),
				summaryAggregationData.Select(item => new SummaryAggregationResult(item.Name, item.Slice, item.SummaryAggregationModel, item.Value)));
		}
		DashboardUnderlyingDataSet IDataProcessor<PivotDataQuery>.GetUnderlyingData(UnderlyingDataQuery<PivotDataQuery> query) {
			List<object> rowValues = query.RowValues == null ? new List<object>() : query.RowValues.Select(v => v.Value).ToList();
			List<object> columnValues = query.ColumnValues == null ? new List<object>() : query.ColumnValues.Select(v => v.Value).ToList();
			ValidateQuery(query.SliceQuery);
			IEnumerable<PivotDataQuery.EquatableIndicator> diff = currentState.QualitativeEquals(query.SliceQuery);
			bool needSetupPivot = diff.ContainsAny(PivotDataQuery.EquatableIndicator.DataSchema, PivotDataQuery.EquatableIndicator.Filters);
			bool containsHiddenOthers = false;
			List<PivotGridFieldBase> actualFields;
			try {
				pivot.BeginUpdate();
				if(needSetupPivot)
					SetupPivot(query.SliceQuery);
				List<PivotGridFieldBase> columnFields = pivot.GetFieldsByArea(PivotArea.ColumnArea, false);
				List<PivotGridFieldBase> rowFields = pivot.GetFieldsByArea(PivotArea.RowArea, false);
				if(!pivot.IsOLAP) {
					for(int i = columnValues.Count; i < columnFields.Count; i++) {
						PivotGridFieldBase field = columnFields[i];
						if(field.TopValueCount != 0 && !field.TopValueShowOthers) {
							containsHiddenOthers = true;
							break;
						}
					}
					if(!containsHiddenOthers) {
						for(int i = rowValues.Count; i < rowFields.Count; i++) {
							PivotGridFieldBase field = rowFields[i];
							if(field.TopValueCount > 0 && !field.TopValueShowOthers) {
								containsHiddenOthers = true;
								break;
							}
						}
					}
				}
				if(!containsHiddenOthers) {
					CriteriaOperator criteria = null;
					for(int i = 0; i < columnValues.Count; i++)
						criteria = CriteriaOperator.And(criteria, new BinaryOperator(columnFields[i].Name, columnValues[i]));
					for(int i = 0; i < rowValues.Count; i++)
						criteria = CriteriaOperator.And(criteria, new BinaryOperator(rowFields[i].Name, rowValues[i]));
					pivot.Prefilter.Criteria = CriteriaOperator.And(pivot.Prefilter.Criteria, criteria);
					foreach(PivotGridFieldBase field in pivot.Fields)
						if(field.Area == PivotArea.DataArea)
							field.Visible = false;
						else
							field.Area = PivotArea.FilterArea;
				} else {
				}
				Dictionary<string, PivotGridFieldBase> dic = new Dictionary<string, PivotGridFieldBase>();
				for(int i = 0; i < query.DataMembers.Count; i++) {
					string dataMember = query.DataMembers[i];
					PivotGridFieldBase field = pivot.Fields[dataMember];
					if(field == null)
						field = pivot.Fields.OfType<PivotGridFieldBase>().FirstOrDefault((p) => p.DrillDownColumnName == dataMember);
					if(field == null)
						field = pivot.Fields.Add(dataMember, PivotArea.FilterArea);
					dic[dataMember] = field;
				}
				actualFields = dic.OrderByDescending((pair) => pair.Value.IsOLAPField && pair.Value.Area == PivotArea.DataArea).Select((pair) => pair.Value).ToList();
			} catch {
				return null;
			} finally {
				pivot.EndUpdate();
			}
			currentState = new PivotDataQuery();
			if(actualFields.Count == 0)
				return null;
			PivotDrillDownDataSource ds;
			int columnIndex;
			int rowIndex;
			if(containsHiddenOthers) {
				columnIndex = pivot.PivotDataSource.GetVisibleIndexByValues(true, ArrayFromIList(columnValues));
				rowIndex = pivot.PivotDataSource.GetVisibleIndexByValues(false, ArrayFromIList(rowValues));
			} else {
				columnIndex = -1;
				rowIndex = -1;
			}
			if(pivot.QueryDataSource != null)
				ds = pivot.QueryDataSource.GetDrillDownDataSource(columnIndex, rowIndex, 0, -1, (pivot.IsOLAP ? actualFields.Select((f) => f.OLAPDrillDownColumnName) : actualFields.Select((f) => f.FieldName)).Distinct().ToList());
			else
				ds = pivot.PivotDataSource.GetDrillDownDataSource(columnIndex, rowIndex, 0, -1);
			return WrapUnderlyingDataSource(ds, actualFields);
		}
		object[] ArrayFromIList(IList source) {
			if(source == null)
				return null;
			object[] array = new object[source.Count];
			source.CopyTo(array, 0);
			return array;
		}
		DashboardUnderlyingDataSet WrapUnderlyingDataSource(PivotDrillDownDataSource underlyingDataSource, List<PivotGridFieldBase> actualFields) {
			if(underlyingDataSource != null)
				return new DashboardUnderlyingDataSet(new DashboardUnderlyingDataSetInternal(underlyingDataSource, actualFields, pivot.QueryDataSource != null, pivot.IsOLAP));
			return null;
		}
		public void Dispose() {
			pivot.Dispose();
		}
		void SetupPivot(PivotDataQuery query) {
			IEnumerable<DimensionModel> allDimensions = query.Columns.Concat(query.Rows).ToArray();
			IEnumerable<IEnumerable<DimensionModel>> slices = query.ResultSlices.Select(slice => slice.Dimensions).ToList();
			pivot.BeginUpdate();
			try {
				ClearPivot();
				PivotFieldBuilder fieldBuilder = new PivotFieldBuilder(pivot);
				query.Columns.ForEach(d => fieldBuilder.AddDimension(d, PivotArea.ColumnArea));
				query.ColumnsTopN.ForEach(fieldBuilder.AddTopN);
				query.ColumnsSort.ForEach(fieldBuilder.AddSort);
				query.Rows.ForEach(d => fieldBuilder.AddDimension(d, PivotArea.RowArea));
				query.RowsTopN.ForEach(fieldBuilder.AddTopN);
				query.RowsSort.ForEach(fieldBuilder.AddSort);
				query.Measures.ForEach(m => fieldBuilder.AddMeasure(m));
				query.FilterDimensions
					.Where(d => UseAsFilter(allDimensions, d))
					.ForEach(d => fieldBuilder.AddDimension(d, PivotArea.FilterArea));
				pivot.Prefilter.Criteria = query.FilterCriteria; 
				ApplySummaryAggregations(query, fieldBuilder);
				pivot.OptionsData.AutoExpandGroups = query.ColumnsExpandState.ExpandAction == ExpandAction.Collapse && query.RowsExpandState.ExpandAction == ExpandAction.Collapse ?
					DefaultBoolean.True : DefaultBoolean.False;
				if(query.Columns.Count > 0 && SliceToPivotHelper<DimensionModel>.IsCalcGrandTotalOnCrossAxis(query.Rows, slices))
					fieldBuilder.AddTotals(query.Columns[0]);
				if(query.Rows.Count > 0 && SliceToPivotHelper<DimensionModel>.IsCalcGrandTotalOnCrossAxis(query.Columns, slices))
					fieldBuilder.AddTotals(query.Rows[0]);
				SliceToPivotHelper<DimensionModel>.GetSummaryLevels(query.Columns, query.Rows, slices)
					.Select(d => getNextLevel(query.Columns.Contains(d) ? query.Columns : query.Rows, d))
					.NotNull()
					.ForEach(d => fieldBuilder.AddTotals(d));
			} finally {
				pivot.EndUpdate();
			}
		}
		AggregationItemValue GetAggregationSummaryItem(SummaryAggregationModel summAggModel, ResultSliceModel slice) {
			SummaryAggregationAcessor result = new SummaryAggregationAcessor() {Name = summAggModel.Name,  Slice = slice.Dimensions, SummaryAggregationModel = summAggModel };
			summaryAggregationData.Add(result);
			return new AggregationItemValue(summAggModel.SummaryType, summAggModel.Argument, (x) => { result.Value = x; });
		}
		void ApplySummaryAggregations(PivotDataQuery query, PivotFieldBuilder fieldBuilder) {
			IList<AggregationLevel> aggList = new List<AggregationLevel>();
			foreach(var slice in query.ResultSlices) {
				DimensionModel columnD = SliceToPivotHelper<DimensionModel>.GetAxisComponent(query.Columns, slice.Dimensions).LastOrDefault();
				DimensionModel rowD = SliceToPivotHelper<DimensionModel>.GetAxisComponent(query.Rows, slice.Dimensions).LastOrDefault();
				int columnIndex = columnD == null ? -1 : query.Columns.IndexOf(columnD);
				int rowIndex = rowD == null ? -1 : query.Rows.IndexOf(rowD);
				List<AggregationCalculation> calcs = slice.SummaryAggregations.Where(agg => agg.Measure != null).GroupBy(agg => agg.Measure).Select(measureGroup => {
					PivotGridFieldBase data = fieldBuilder.AddMeasure(measureGroup.Key);
					IList<AggregationItemValue> aggItems = measureGroup.Select(aggModel => GetAggregationSummaryItem(aggModel, slice)).ToList();
					return new AggregationCalculation(aggItems, AggregationCalculatationTarget.Data, pivot.GetFieldsByArea(PivotArea.DataArea, false).IndexOf(data));
				}).ToList();
				calcs.AddRange(slice.SummaryAggregations.Where(agg => agg.Dimension != null).GroupBy(agg => agg.Dimension).Select(measureGroup => {
					PivotGridFieldBase field = fieldBuilder.GetField(measureGroup.Key);
					IList<AggregationItemValue> aggItems = measureGroup.Select(aggModel => GetAggregationSummaryItem(aggModel, slice)).ToList();
					return new AggregationCalculation(aggItems, field.IsColumn ? AggregationCalculatationTarget.Column : AggregationCalculatationTarget.Row, pivot.GetFieldsByArea(field.Area, false).IndexOf(field));
				}));
				aggList.Add(new AggregationLevel(calcs, rowIndex, columnIndex));
			}
			pivot.SetAggregations(aggList);
		}
		void ApplyExpand(AxisExpandModel columnsExpand, AxisExpandModel rowsExpand) {
			bool autoExpandGroups = columnsExpand.ExpandAction == ExpandAction.Collapse && rowsExpand.ExpandAction == ExpandAction.Collapse;
			pivot.AutoExpandGroups = autoExpandGroups;
			IQueryDataSource pivotQueryDataSource = pivot.QueryDataSource;
			if(pivotQueryDataSource == null) {
				Action<bool, IEnumerable<object[]>, bool> apply = (expand, actionValues, isColumn) => {
					bool isOlap = pivot.IsOLAP;
					foreach(object[] values in actionValues) {
						if(isOlap)
							pivot.ChangeExpandedByUniqueValues(isColumn, values, expand);
						else
							pivot.ChangeExpanded(isColumn, values, expand);
					}
				};
				pivot.ChangeExpandedAll(true, columnsExpand.ExpandAction == ExpandAction.Collapse);
				pivot.ChangeExpandedAll(false, rowsExpand.ExpandAction == ExpandAction.Collapse);
				apply(columnsExpand.ExpandAction == ExpandAction.Expand, columnsExpand.Values, true);
				apply(rowsExpand.ExpandAction == ExpandAction.Expand, rowsExpand.Values, false);
			}
			else {
				pivotQueryDataSource.LoadCollapsedState(true, columnsExpand.ToPivotCollapsedState());
				pivotQueryDataSource.LoadCollapsedState(false, rowsExpand.ToPivotCollapsedState());
			}
		}
		void SetPivotDataDataSource(DataSourceModel dataSource) {
			if(dataSource.ListSource != null)
				pivot.ListSource = dataSource.ListSource;
			else if(dataSource.PivotDataSource != null) {
				pivot.PivotDataSource = (IPivotGridDataSource)((ICloneable)dataSource.PivotDataSource).Clone();
				pivot.EnsureOlapConnected();
			}
		}
		void ClearPivot() {
			summaryAggregationData.Clear();
			pivot.Fields.Clear();
			pivot.SetAggregations(new List<AggregationLevel>());
		}
		bool UseAsFilter(IEnumerable<DimensionModel> allDimensions, DimensionModel hiddenD) {
			if(dataSource.IsOlap)
				return !allDimensions.Any(d => d.DataMember == hiddenD.DataMember); 
			else
				return true;
		}
		DimensionModel getNextLevel(IEnumerable<DimensionModel> axis, DimensionModel dimension) {
			return axis.SkipWhile(d => d != dimension).Skip(1).FirstOrDefault();
		}
		static HierarchicalDataParams GetHierarchicalData(PivotGridData pivot, PivotDataQuery query) {
			SliceDescription[] slices = query.ResultSlices.Select(s => new SliceDescription(s.Dimensions)).ToArray();
			DataStorageGeneratorResult result = DataStorageGenerator.Generate(pivot, pivot.GetFieldsByArea(PivotArea.DataArea, false), slices);
			return new HierarchicalDataParams {
				Storage = result.Storage,
				SortOrderSlices = result.SortOrderSlices
			};
		}
		static void ValidateQuery(PivotDataQuery query) {
			Func<IList<DimensionModel>, IList<DimensionTopNModel>, bool> checkTopN = (axisMembers, axisTopN) => {
				bool result = true;
				for(int i = 0; i < axisTopN.Count; i++) {
					var topN = axisTopN[i];
					int positionInAxis = axisMembers.IndexOf(topN.DimensionModel);
					bool axisContains = positionInAxis >= 0;
					bool subgroupValid = topN.TopNSubgroup.Count() == 0 || topN.TopNSubgroup.SequenceEqual(axisMembers.Take(positionInAxis));
					result &= axisContains && subgroupValid;
				}
				return result;
			};
			var axisDimensions = query.Columns.Concat(query.Rows);
			int totalDimensions = query.Columns.Count + query.Rows.Count;
			bool topNvalid = checkTopN(query.Columns, query.ColumnsTopN)
						  && checkTopN(query.Rows, query.RowsTopN);
			bool dimesnionNotDuplicated = axisDimensions.Distinct().Count() == totalDimensions;
			bool dimensionNameNotDuplicated = axisDimensions.Select(r => r.Name).Distinct().Count() == totalDimensions;
			bool valid = topNvalid && dimesnionNotDuplicated && dimensionNameNotDuplicated;
			if(!valid)
				throw new ArgumentException("PivotDataQuery invalid");
		}
	}
	class PivotFieldBuilder {
		PivotGridData pivot;
		IEnumerable<PivotGridFieldBase> Fields { get { return pivot.Fields.Cast<PivotGridFieldBase>(); } }
		public PivotFieldBuilder(PivotGridData pivot) {
			DXContract.Requires(pivot != null);
			this.pivot = pivot;
		}
		public PivotGridFieldBase AddDimension(DimensionModel dimension, PivotArea area) {
			DXContract.Requires(dimension != null);
			DXContract.Requires(area != PivotArea.DataArea);
			PivotGridFieldBase field = GetField(dimension);
			if(field == null) {
				field = CreateField(dimension, area);
				SetupField(field, dimension);
			}
			return field;
		}
		public PivotGridFieldBase AddMeasure(MeasureModel measure) {
			DXContract.Requires(measure != null);
			PivotGridFieldBase field = GetField(measure);
			if(field == null) {
				field = CreateField(measure, PivotArea.DataArea);
				SetupField(field, measure);
			}
			return field;
		}
		public void AddTopN(DimensionTopNModel topN) {
			if(topN.TopNEnabled) {
				PivotGridFieldBase field = GetField(topN.DimensionModel);
				field.TopValueCount = topN.TopNCount;
				field.TopValueShowOthers = topN.TopNShowOthers;
				field.SortOrder = topN.TopNDirection;
				field.SortBySummaryInfo.Field = AddMeasure(topN.TopNMeasure);
			}
		}
		public void AddSort(DimensionSortModel sort) {
			PivotGridFieldBase field = GetField(sort.SortedDimension );
			field.SortMode = sort.SortMode;
			field.SortOrder = sort.SortOrder;
			if(sort.SortByMeasure != null && sort.SortByMeasure.Measure != null)
				field.SortBySummaryInfo.Field = AddMeasure(sort.SortByMeasure.Measure);
		}
		public void AddTotals(DimensionModel d) {
			GetField(d).CalculateTotals = true;
		}
		public PivotGridFieldBase GetField<TDataItemModel>(DataItemModel<TDataItemModel> item) where TDataItemModel : DataItemModel<TDataItemModel> {
			return Fields.SingleOrDefault(d => d.Name == item.Name);
		}
		PivotGridFieldBase CreateField<TDataItemModel>(DataItemModel<TDataItemModel> baseInfo, PivotArea area) where TDataItemModel : DataItemModel<TDataItemModel> {
			PivotGridFieldBase field = pivot.Fields.Add(baseInfo.DataMember, area);
			field.Name = baseInfo.Name;
			field.Options.OLAPFilterUsingWhereClause = PivotOLAPFilterUsingWhereClause.Auto;
			field.UnboundType = baseInfo.UnboundType;
			field.UnboundExpression = baseInfo.UnboundExpression;
			field.UnboundExpressionMode = baseInfo.UnboundMode.ToPivotMode();
			field.DrillDownColumnName = baseInfo.DrillDownName;
			return field;
		}
		void SetupField(PivotGridFieldBase field, DimensionModel dimension) {
			field.CalculateTotals = false;
			field.GroupInterval = dimension.GroupInterval;
		}
		void SetupField(PivotGridFieldBase field, MeasureModel measure) {
			if(measure.UnboundMode != ExpressionMode.SummaryLevel) {
				field.SummaryType = measure.SummaryType == SummaryType.CountDistinct ? PivotSummaryType.Count : (PivotSummaryType)measure.SummaryType;
				field.UseDecimalValuesForMaxMinSummary = measure.DecimalSummary;
			}
			field.Visible = measure.IsVisible;
		}
	}
}
