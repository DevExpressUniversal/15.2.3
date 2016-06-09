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
using System.Text;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data.Filtering;
namespace DevExpress.DashboardCommon.DataProcessing {
	interface IQualitativeEquatable<T, TIndicator> {
		TIndicator QualitativeEquals(T other);
	}
	class PivotDataQuery : DataModelBase<PivotDataQuery>, IQualitativeEquatable<PivotDataQuery, IEnumerable<PivotDataQuery.EquatableIndicator>> {
		public enum EquatableIndicator { DataSchema, Filters, ExpandState }
		readonly IList<DimensionModel> columns = new List<DimensionModel>();
		readonly IList<DimensionTopNModel> columnsTopN = new List<DimensionTopNModel>();
		readonly IList<DimensionSortModel> columnsSort = new List<DimensionSortModel>();
		readonly IList<DimensionModel> rows = new List<DimensionModel>();
		readonly IList<DimensionTopNModel> rowsTopN = new List<DimensionTopNModel>();
		readonly IList<DimensionSortModel> rowsSort = new List<DimensionSortModel>();
		readonly IList<MeasureModel> measures = new List<MeasureModel>();
		readonly IList<DimensionModel> filterDimensions = new List<DimensionModel>();
		readonly IList<ResultSliceModel> result = new List<ResultSliceModel>();
		AxisExpandModel columnsExpandState = AxisExpandModel.FullExpandModel();
		AxisExpandModel rowsExpandState = AxisExpandModel.FullExpandModel();
		public IList<DimensionModel> Columns { get { return columns; } }
		public IList<DimensionTopNModel> ColumnsTopN { get { return columnsTopN; } }
		public IList<DimensionSortModel> ColumnsSort { get { return columnsSort; } }
		public AxisExpandModel ColumnsExpandState { get { return columnsExpandState; } set { columnsExpandState = value; } }
		public IList<DimensionModel> Rows { get { return rows; } }
		public IList<DimensionTopNModel> RowsTopN { get { return rowsTopN; } }
		public IList<DimensionSortModel> RowsSort { get { return rowsSort; } }
		public AxisExpandModel RowsExpandState { get { return rowsExpandState; } set { rowsExpandState = value; } }
		public IList<MeasureModel> Measures { get { return measures; } }
		public IList<DimensionModel> FilterDimensions { get { return filterDimensions; } }
		public CriteriaOperator FilterCriteria { get; set; }
		public IList<ResultSliceModel> ResultSlices { get { return result; } }
		public IEnumerable<PivotDataQuery.EquatableIndicator> QualitativeEquals(PivotDataQuery other) {
			IList<PivotDataQuery.EquatableIndicator> result = new List<PivotDataQuery.EquatableIndicator>();
			Action<bool, PivotDataQuery.EquatableIndicator> check = (predicate, indicator) => {
				if(!predicate)
					result.Add(indicator);
			};
			check(Columns.SequenceEqual(other.Columns), EquatableIndicator.DataSchema);
			check(ColumnsTopN.SequenceEqual(other.ColumnsTopN), EquatableIndicator.DataSchema);
			check(ColumnsSort.SequenceEqual(other.ColumnsSort), EquatableIndicator.DataSchema);
			check(ColumnsExpandState.Equals(other.ColumnsExpandState), EquatableIndicator.ExpandState);
			check(Rows.SequenceEqual(other.Rows), EquatableIndicator.DataSchema);
			check(RowsTopN.SequenceEqual(other.RowsTopN), EquatableIndicator.DataSchema);
			check(RowsSort.SequenceEqual(other.RowsSort), EquatableIndicator.DataSchema);
			check(RowsExpandState.Equals(other.RowsExpandState), EquatableIndicator.ExpandState);
			check(ResultSlices.SequenceEqual(other.ResultSlices), EquatableIndicator.DataSchema);
			check(Measures.SequenceEqual(other.Measures), EquatableIndicator.DataSchema);
			check(FilterDimensions.SequenceEqual(other.FilterDimensions), EquatableIndicator.Filters);
			check(CriteriaComparer.Default.Equals(FilterCriteria, other.FilterCriteria), EquatableIndicator.Filters);
			return result;
		}
		protected override int ModelHashCode() {
			return 0;
		}
		protected override bool ModelEquals(PivotDataQuery other) {
			return QualitativeEquals(other).Count() == 0;
		}
	}
	public class ResultSliceModel : DataModelBase<ResultSliceModel> {
		public IList<DimensionModel> Dimensions { get; private set; }
		public IList<MeasureModel> Measures { get; private set; }
		public IList<SummaryAggregationModel> SummaryAggregations { get; private set; }
		public ResultSliceModel(IEnumerable<DimensionModel> dimensions, IEnumerable<MeasureModel> measures) : this(dimensions, measures, new SummaryAggregationModel[0]) { }
		public ResultSliceModel(IEnumerable<DimensionModel> dimensions, IEnumerable<MeasureModel> measures, IEnumerable<SummaryAggregationModel> summaryAggregations) {
			Dimensions = dimensions.ToList();
			Measures = measures.ToList();
			SummaryAggregations = summaryAggregations.ToList();
		}
		protected override int ModelHashCode() {
			return HashcodeHelper.GetCompositeHashCode(Dimensions)
				  ^ HashcodeHelper.GetCompositeHashCode(Measures)
				  ^ HashcodeHelper.GetCompositeHashCode(SummaryAggregations);
		}
		protected override bool ModelEquals(ResultSliceModel other) {
			return Dimensions.SequenceEqual(other.Dimensions)
				&& Measures.SequenceEqual(other.Measures)
				&& SummaryAggregations.SequenceEqual(other.SummaryAggregations);
		}
	}
}
