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
using System.Text;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class SliceModel : DataModelBase<SliceModel> {
		public IList<DimensionModel> Dimensions { get; set; }
		public IList<DimensionTopNModel> DimensionsTopN { get; set; }
		public IList<DimensionSortModel> DimensionsSort { get; set; }
		public RowFiltersModel<DimensionModel>[] RowFilters { get; set; }
		public IList<MeasureModel> Measures { get; set; }
		public IList<DimensionModel> FilterDimensions { get; set; }
		public CriteriaOperator FilterCriteria { get; set; }
		public IList<SummaryAggregationModel> SummaryAggregations { get; private set; }
		public SliceModel() {
			Dimensions = new List<DimensionModel>();
			DimensionsTopN = new List<DimensionTopNModel>();
			DimensionsSort = new List<DimensionSortModel>();
			Measures = new List<MeasureModel>();
			FilterDimensions = new List<DimensionModel>();
			SummaryAggregations = new List<SummaryAggregationModel>();
		}
		protected override int ModelHashCode() {
			return HashcodeHelper.GetCompositeHashCode(Dimensions)
				^ HashcodeHelper.GetCompositeHashCode(DimensionsTopN)
				^ HashcodeHelper.GetCompositeHashCode(DimensionsSort)
				^ HashcodeHelper.GetCompositeHashCode(FilterDimensions)
				^ HashcodeHelper.GetCompositeHashCode(Measures)
				^ HashcodeHelper.GetCompositeHashCode(SummaryAggregations);
		}
		protected override bool ModelEquals(SliceModel other) {
			return Dimensions.SequenceEqual(other.Dimensions)
				&& Measures.SequenceEqual(other.Measures)
				&& SummaryAggregations.SequenceEqual(other.SummaryAggregations);
		}
		public override string ToString() {
			return Dimensions.Count > 0 ? Dimensions.Select(c => c.ToString()).Aggregate((s1, s2) => s1 + ", " + s2) : String.Empty;
		}
	}
	public enum RowFiltersType {
		Include,
		Exclude
	};
	public class RowFiltersModel<T> {
		public IList<T> Dimensions { get; set; }
		public IList<object[]> Values { get; set; }
		public RowFiltersType FilterType { get; set; }
		public RowFiltersModel() {
			FilterType = RowFiltersType.Exclude;
			Dimensions = new List<T>();
			Values = new List<object[]>();
		}
	}
	public class SummaryAggregationModel : DataModelBase<SummaryAggregationModel> {
		public string Name { get; private set; }
		public MeasureModel Measure { get; private set; }
		public DimensionModel Dimension { get; private set; }
		public SummaryItemTypeEx SummaryType { get; private set; }
		public decimal Argument { get; private set; }
		public bool IncludeInGrandTotal { get; private set; }
		public SummaryAggregationModel(string name, DimensionModel dimension, SummaryItemTypeEx summaryType, decimal argument, bool includeInGrandTotal)
			: this(name, summaryType, argument) {
			this.IncludeInGrandTotal = includeInGrandTotal;
			Guard.ArgumentNotNull(dimension, "dimension");
			this.Dimension = dimension;
		}
		public SummaryAggregationModel(string name, MeasureModel measure, SummaryItemTypeEx summaryType, decimal argument, bool includeInGrandTotal)
			: this(name, measure, summaryType, argument) {
			this.IncludeInGrandTotal = includeInGrandTotal;
		}
		public SummaryAggregationModel(string name, MeasureModel measure, SummaryItemTypeEx summaryType, decimal argument)
			: this(name, summaryType, argument) {
			DXContract.Requires(measure != null);
			this.Measure = measure;
		}
		public SummaryAggregationModel(string name, DimensionModel dimension, SummaryItemTypeEx summaryType, decimal argument)
			: this(name, summaryType, argument) {
			DXContract.Requires(dimension != null);
			this.Dimension = dimension;
		}
		public SummaryAggregationModel(string name, SummaryItemTypeEx summaryType, decimal argument) {
			this.Name = name;
			this.SummaryType = summaryType;
			this.Argument = argument;
		}
		protected override int ModelHashCode() {
			return HashcodeHelper.GetCompositeHashCode<object>(Name, Measure, Argument, SummaryType);
		}
		protected override bool ModelEquals(SummaryAggregationModel other) {
			return Name == other.Name &&
				Measure == other.Measure
				&& SummaryType == other.SummaryType
				&& Argument == other.Argument;
		}
	}
}
