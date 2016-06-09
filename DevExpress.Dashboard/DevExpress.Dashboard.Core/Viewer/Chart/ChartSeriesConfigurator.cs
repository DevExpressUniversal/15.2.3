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
using System.Text;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.XtraCharts;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class ChartSeriesConfigurator {
		public static ChartSeriesConfigurator CreateInstance(ChartSeriesTemplateViewModel viewModel, string argumentDataMember, bool ignoreEmptyArgument) {
			ChartSeriesViewModelType seriesType = viewModel.SeriesType;
			switch(seriesType) {
				case ChartSeriesViewModelType.Bar:
				case ChartSeriesViewModelType.StackedBar:
				case ChartSeriesViewModelType.FullStackedBar:
				case ChartSeriesViewModelType.Point:
				case ChartSeriesViewModelType.Line:
				case ChartSeriesViewModelType.StackedLine:
				case ChartSeriesViewModelType.FullStackedLine:
				case ChartSeriesViewModelType.StepLine:
				case ChartSeriesViewModelType.Spline:
				case ChartSeriesViewModelType.Area:
				case ChartSeriesViewModelType.StackedArea:
				case ChartSeriesViewModelType.FullStackedArea:
				case ChartSeriesViewModelType.StepArea:
				case ChartSeriesViewModelType.SplineArea:
				case ChartSeriesViewModelType.StackedSplineArea:
				case ChartSeriesViewModelType.FullStackedSplineArea:
					return new SimpleSeriesConfigurator(viewModel, argumentDataMember, ignoreEmptyArgument);
				case ChartSeriesViewModelType.HighLowClose:
					return new HighLowCloseSeriesConfigurator(viewModel, argumentDataMember, ignoreEmptyArgument);
				case ChartSeriesViewModelType.CandleStick:
				case ChartSeriesViewModelType.Stock:
					return new OpenHighLowCloseSeriesConfigurator(viewModel, argumentDataMember, ignoreEmptyArgument);
				case ChartSeriesViewModelType.SideBySideRangeBar:
				case ChartSeriesViewModelType.RangeArea:
					return new RangeSeriesConfigurator(viewModel, argumentDataMember, ignoreEmptyArgument);
				case ChartSeriesViewModelType.Weighted:
					return new WeightedSeriesConfigurator(viewModel, argumentDataMember, ignoreEmptyArgument);
				default:
					throw new NotSupportedException(seriesType.ToString());
			}
		}
		protected static string FormatValue(DashboardStringId stringId, string displayText) {
			return DashboardLocalizer.GetString(stringId) + ": " + displayText;
		}
		readonly string argumentDataMember;
		readonly ChartSeriesTemplateViewModel viewModel;
		readonly bool ignoreEmptyArgument;
		public ChartSeriesTemplateViewModel ViewModel { get { return viewModel; } }
		public bool OnlyPercentValues { get { return viewModel.OnlyPercentValues; } }
		protected abstract string Prefix { get; }
		protected ChartSeriesConfigurator(ChartSeriesTemplateViewModel viewModel, string argumentDataMember, bool ignoreEmptyArgument) {
			this.argumentDataMember = argumentDataMember;
			this.viewModel = viewModel;
			this.ignoreEmptyArgument = ignoreEmptyArgument;
		}
		void ConfigureSeries(Series series, AxisPoint axisPoint, string seriesName) {
			series.Tag = axisPoint;
			series.Name = seriesName;
			if(ignoreEmptyArgument)
				series.DataFilters.Add(new DataFilter {
					Condition = DataFilterCondition.NotEqual,
					ColumnName = argumentDataMember,
					DataType = typeof(string),
					Value = DashboardSpecialValues.NullValue
				});
		}
		protected abstract void ConfigureView(Series series);
		protected abstract void ConfigureData(Series series);
		public abstract string GetValueString(string[] displayTexts);
		public string GetCrosshairValueString(string[] displayTexts) {
			return Prefix + GetValueString(displayTexts);
		}
		protected virtual void ConfigureEmptyPointsBehavior(Series series) {
			if(ViewModel.IgnoreEmptyPoints && !String.IsNullOrEmpty(ViewModel.DataMembers[0])) {
				series.DataFilters.Add(new DataFilter {
					Condition = DataFilterCondition.NotEqual,
					ColumnName = ViewModel.DataMembers[0],
					DataType = typeof(object),
					Value = null
				});
			}
		}
		public void AppendSeparator(StringBuilder firstLabel, StringBuilder secondLabel) {
			firstLabel.Append(Prefix);
			secondLabel.Append(Prefix);
		}
		public virtual void AppendValueString(object[] values, string[] displayTexts, StringBuilder firstLabel, StringBuilder secondLabel) {
			String valueString = GetValueString(displayTexts);
			firstLabel.Append(valueString);
		}
		public virtual void AppendSeriesName(object[] values, Series series, StringBuilder firstLabel, StringBuilder secondLabel) {
			firstLabel.Append(series.Name);
		}
		public Series CreateSeries(AxisPoint axisPoint, string seriesName) {
			Series series = new Series();
			series.ArgumentDataMember = argumentDataMember;
			series.ColorDataMember = ChartColorMultiDimensionalDataPropertyDescriptor.ColorMember;
			ConfigureView(series);
			ConfigureData(series);
			series.LabelsVisibility = DefaultBoolean.False;
			ConfigureSeries(series, axisPoint, seriesName);
			ConfigureEmptyPointsBehavior(series);
			return series;
		}
	}
	public class SimpleSeriesConfigurator : ChartSeriesConfigurator {
		protected override string Prefix { get { return ": "; } }
		public SimpleSeriesConfigurator(ChartSeriesTemplateViewModel viewModel, string argumentDataMember, bool ignoreEmptyArgument)
			: base(viewModel, argumentDataMember, ignoreEmptyArgument) {
		}
		protected override void ConfigureView(Series series) {
			SeriesViewBase view;
			switch(ViewModel.SeriesType) {
				case ChartSeriesViewModelType.StackedBar:
					view = new StackedBarSeriesView();
					break;
				case ChartSeriesViewModelType.FullStackedBar:
					view = new FullStackedBarSeriesView();
					break;
				case ChartSeriesViewModelType.Point:
					view = new PointSeriesView();
					break;
				case ChartSeriesViewModelType.Line:
					view = new LineSeriesView();
					break;
				case ChartSeriesViewModelType.StackedLine:
					view = new StackedLineSeriesView();
					break;
				case ChartSeriesViewModelType.FullStackedLine:
					view = new FullStackedLineSeriesView();
					break;
				case ChartSeriesViewModelType.StepLine:
					view = new StepLineSeriesView();
					break;
				case ChartSeriesViewModelType.Spline:
					view = new SplineSeriesView();
					break;
				case ChartSeriesViewModelType.Area:
					view = new AreaSeriesView();
					break;
				case ChartSeriesViewModelType.StackedArea:
					view = new StackedAreaSeriesView();
					break;
				case ChartSeriesViewModelType.FullStackedArea:
					view = new FullStackedAreaSeriesView();
					break;
				case ChartSeriesViewModelType.StepArea:
					view = new StepAreaSeriesView();
					break;
				case ChartSeriesViewModelType.SplineArea:
					view = new SplineAreaSeriesView();
					break;
				case ChartSeriesViewModelType.StackedSplineArea:
					view = new StackedSplineAreaSeriesView();
					break;
				case ChartSeriesViewModelType.FullStackedSplineArea:
					view = new FullStackedSplineAreaSeriesView();
					break;
				default:
					view = new SideBySideBarSeriesView();
					break;
			}
			series.View = view;
		}
		protected override void ConfigureData(Series series) {
			series.ValueDataMembers[0] = ViewModel.DataMembers[0];
		}
		public override string GetValueString(string[] displayTexts) {
			return displayTexts[0];
		}
	}
	public abstract class TwoValuesSeriesConfigurator : ChartSeriesConfigurator {
		protected override string Prefix { get { return ": "; } }
		protected TwoValuesSeriesConfigurator(ChartSeriesTemplateViewModel viewModel, string argumentDataMember, bool ignoreEmptyArgument)
			: base(viewModel, argumentDataMember, ignoreEmptyArgument) {
		}
		protected override void ConfigureData(Series series) {
			series.ValueDataMembers[0] = ViewModel.DataMembers[0];
			series.ValueDataMembers[1] = ViewModel.DataMembers[1];
		}
		public override string GetValueString(string[] displayTexts) {
			return displayTexts[0] + " - " + displayTexts[1];
		}
	}
	public class RangeSeriesConfigurator : TwoValuesSeriesConfigurator {
		public RangeSeriesConfigurator(ChartSeriesTemplateViewModel viewModel, string argumentDataMember, bool ignoreEmptyArgument)
			: base(viewModel, argumentDataMember, ignoreEmptyArgument) {
		}
		protected override void ConfigureView(Series series) {
			if(ViewModel.SeriesType == ChartSeriesViewModelType.RangeArea)
				series.View = new RangeAreaSeriesView();
			else
				series.View = new SideBySideRangeBarSeriesView();
		}
		int GetMaxValueIndex(object[] values) {
			decimal value1 = Convert.ToDecimal(values[0]);
			decimal value2 = Convert.ToDecimal(values[1]);
			if(value1 > value2) return 0;
			return 1;
		}
		public override void AppendValueString(object[] values, string[] displayTexts, StringBuilder firstLabel, StringBuilder secondLabel) {
			int maxValueIndex = GetMaxValueIndex(values);
			int minValueIndex = maxValueIndex == 0 ? 1 : 0;
			firstLabel.Append(displayTexts[minValueIndex]);
			secondLabel.Append(displayTexts[maxValueIndex]);
		}
		public override void AppendSeriesName(object[] values, Series series, StringBuilder firstLabel, StringBuilder secondLabel) {
			int maxValueIndex = GetMaxValueIndex(values);
			int minValueIndex = maxValueIndex == 0 ? 1 : 0;
			bool twoValues = ViewModel.MeasureCaptions.Count == 2;
			firstLabel.Append(ViewModel.MeasureCaptions[twoValues ? minValueIndex : 0]);
			secondLabel.Append(ViewModel.MeasureCaptions[twoValues ? maxValueIndex : 0]);
		}
	}
	public class WeightedSeriesConfigurator : TwoValuesSeriesConfigurator {
		public WeightedSeriesConfigurator(ChartSeriesTemplateViewModel viewModel, string argumentDataMember, bool ignoreEmptyArgument)
			: base(viewModel, argumentDataMember, ignoreEmptyArgument) {
		}
		protected override void ConfigureView(Series series) {
			series.View = new BubbleSeriesView();
		}
	}
	public class HighLowCloseSeriesConfigurator : ChartSeriesConfigurator {
		protected override string Prefix { get { return "\n"; } }
		public HighLowCloseSeriesConfigurator(ChartSeriesTemplateViewModel viewModel, string argumentDataMember, bool ignoreEmptyArgument)
			: base(viewModel, argumentDataMember, ignoreEmptyArgument) {
		}
		protected override void ConfigureView(Series series) {
			StockSeriesView view = new StockSeriesView();
			view.ShowOpenClose = StockType.Close;
			series.View = view;
		}
		protected override void ConfigureData(Series series) {
			series.ValueDataMembers[0] = ViewModel.DataMembers[1]; 
			series.ValueDataMembers[1] = ViewModel.DataMembers[0]; 
			string closeDataMember = ViewModel.DataMembers[2]; 
			series.ValueDataMembers[2] = closeDataMember;
			series.ValueDataMembers[3] = closeDataMember;
		}
		public override string GetValueString(string[] displayTexts) {
			return FormatValue(DashboardStringId.HighCaption, displayTexts[0]) + "\n " +
				FormatValue(DashboardStringId.LowCaption, displayTexts[1]) + "\n " +
				FormatValue(DashboardStringId.CloseCaption, displayTexts[2]);
		}
	}
	public class OpenHighLowCloseSeriesConfigurator : ChartSeriesConfigurator {
		protected override string Prefix { get { return "\n"; } }
		public OpenHighLowCloseSeriesConfigurator(ChartSeriesTemplateViewModel viewModel, string argumentDataMember, bool ignoreEmptyArgument)
			: base(viewModel, argumentDataMember, ignoreEmptyArgument) {
		}
		protected override void ConfigureView(Series series) {
			series.View = ViewModel.SeriesType == ChartSeriesViewModelType.CandleStick ?
				(SeriesViewBase)new CandleStickSeriesView() : (SeriesViewBase)new StockSeriesView();
		}
		protected override void ConfigureData(Series series) {
			series.ValueDataMembers[0] = ViewModel.DataMembers[2]; 
			series.ValueDataMembers[1] = ViewModel.DataMembers[1]; 
			series.ValueDataMembers[2] = ViewModel.DataMembers[0]; 
			series.ValueDataMembers[3] = ViewModel.DataMembers[3]; 
		}
		public override string GetValueString(string[] displayTexts) {
			return FormatValue(DashboardStringId.OpenCaption, displayTexts[0]) + "\n " +
				FormatValue(DashboardStringId.HighCaption, displayTexts[1]) + "\n " +
				FormatValue(DashboardStringId.LowCaption, displayTexts[2]) + "\n " +
				FormatValue(DashboardStringId.CloseCaption, displayTexts[3]);
		}
	}
}
