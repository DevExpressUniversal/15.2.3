#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
namespace DevExpress.Charts.Native {
	public interface ISeriesPoint {
		Scale ArgumentScaleType { get; }
		object UserArgument { get; }
		string QualitativeArgument { get; }
		double NumericalArgument { get; }
		DateTime DateTimeArgument { get; }
		double[] UserValues { get; }
		double[] AnimatedValues { get; }
		DateTime[] DateTimeValues { get; }
		double InternalArgument { get; set; }
		double[] InternalValues { get; set; }
		bool IsEmpty(Scale scale);
		object ToolTipHint { get; }
	}
	public interface ISeriesInteractionData {
		double MinValue { get; }
		double MaxValue { get; }
		void Calculate();
		bool CanSetValueAxisRange { get; }
		bool CanUpdateValueAxisRange { get; }
		void AddSeries(IRefinedSeries series);
		void UpdateMinimumMaximum(IRefinedSeries series, IBasePoint refinedPoint);
	}
	public interface ISeriesPointFactory {
		ISeriesPoint CreateSeriesPoint(object argument);
		ISeriesPoint CreateSeriesPoint(ISeries owner, object argument, double internalArgument, object[] values, double[] internalValues, object tag);
		ISeriesPoint CreateSeriesPoint(ISeries owner, object argument, double internalArgument, object[] values, double[] internalValues, object tag, object hint, object color);
		ISeriesPoint CreateSeriesPoint(ISeries owner, object argument, double internalArgument, object[] values, double[] internalValues, object tag, object hint);
		ISeriesPoint CreateSeriesPoint(ISeries owner, object argument, object[] values, object tag, object[] colors);
	}
	public interface ISeriesTemplate : ISeriesPointFactory {
		ISeries CreateSeriesForBinding(string seriesName, object seriesValue);
		string ArgumentDataMember { get; }
		IList<string> ValueDataMembers { get; }
		string ToolTipHintDataMember { get; }
	}
	public interface ISeriesBase {
		Scale ArgumentScaleType { get; }
		Scale UserArgumentScaleType { get; }
		Scale ValueScaleType { get; }
		ISeriesView SeriesView { get; }
	}
	public interface ISeries : ISeriesBase, ISeriesPointFactory {
		string Name { get; }
		IEnumerable<ISeriesPoint> Points { get; }
		IList<ISeriesPoint> ActualPoints { get; }
		IList<ISeriesPoint> PointsToInsert { get; }
		IList<ISeriesPoint> PointsToRemove { get; }
		bool ArePointsSorted { get; }
		bool ShouldSortPointsInfo { get; }
		bool Visible { get; }							   
		bool LabelsVisibility { get; }
		bool ShouldBeDrawnOnDiagram { get; }				
		string ArgumentDataMember { get; }
		IList<string> ValueDataMembers { get; }
		string ToolTipHintDataMember { get; }
		string ColorDataMember { get; }
		IEnumerable<IDataFilter> DataFilters { get; }
		Conjunction DataFiltersConjunction { get; }
		SortMode SeriesPointsSortingMode { get; }
		SeriesPointKeyNative SeriesPointsSortingKey { get; }
		void SetArgumentScaleType(Scale scaleType);
		void AddSeriesPoint(ISeriesPoint point);
		void ClearColorCache();
	}
	public interface ISeriesView {
		bool Is3DView { get; }
		bool ShouldSortPoints { get; }
		bool NeedSeriesInteraction { get; }
		bool NeedSeriesGroupsInteraction { get; }
		bool NeedFilterVisiblePoints { get; }
		Type PointInterfaceType { get; }
		CompatibleViewType CompatibleViewType { get; }
		double GetRefinedPointMin(RefinedPoint point);
		double GetRefinedPointMax(RefinedPoint point);
		double GetRefinedPointsMin(IList<RefinedPoint> points);
		double GetRefinedPointsMax(IList<RefinedPoint> points);
		double GetRefinedPointAbsMin(RefinedPoint refinedPoint);
		bool IsCorrectValueLevel(ValueLevelInternal valueLevel);
		SeriesContainer CreateContainer();
		SeriesInteractionContainer CreateSeriesGroupsContainer();
		RangeValue GetMinMax(IPointInteraction interaction, int index);
		IList<ISeriesPoint> GenerateRandomPoints(Scale argumentScaleType, Scale valueScaleType);
		MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram,
			CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode);
	}
	public interface ISimpleSeriesView : ISeriesView {
	}
	public interface ISplineSeriesView {
		int LineTensionPercent { get; set; }
		bool ShouldCorrectRanges { get; }
	}
	public interface IAffectsAxisRange {
		IAxisData AxisYData { get; }
		MinMaxValues GetMinMaxValues(IMinMaxValues visualRangeOfOtherAxisForFiltering);
	}
	public interface ISeparatePaneIndicator : IAffectsAxisRange {
		IPane Pane { get; }
	}
	public interface IXYSeriesView : ISeriesView {
		bool SideMarginsEnabled { get; }
		bool CrosshairEnabled { get; }
		string CrosshairLabelPattern { get; }
		int PixelsPerArgument { get; }
		IAxisData AxisXData { get; }
		IAxisData AxisYData { get; }
		IPane Pane { get; }
		ToolTipPointDataToStringConverter CrosshairConverter { get; }
		IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint);
		List<ISeparatePaneIndicator> GetSeparatePaneIndicators();
		List<IAffectsAxisRange> GetIndicatorsAffectRange();
	}
	public interface IXYWSeriesView : IXYSeriesView {
		double MinSize { get; }
		double MaxSize { get; }
		double GetSideMargins(double min, double max);
	}
	public interface IFinancialSeriesView : ISeriesView {  
	}
	public interface IBarSeriesView {
		double BarWidth { get; set; }
	}
	public interface ISplineView {
		int LineTensionPercent { get; set; }
	}
	public interface IStackedView {
	}
	public interface IStackedSplineView : ISplineView, IStackedView {
	}
	public interface ISideBySideBarSeriesView : IBarSeriesView {
		double BarDistance { get; set; }
		int BarDistanceFixed { get; set; }
		bool EqualBarWidth { get; set; }
	}
	public interface ISupportSeriesGroups {
		object SeriesGroup { get; set; }
	}
	public interface INestedDoughnutSeriesView : ISupportSeriesGroups {
		double HoleRadiusPercent { get; }
		double Weight { get; }
		double InnerIndent { get; }
		double ExplodedDistancePercentage { get; }
		bool? IsOutside { get; set; }
		bool HasExplodedPoints(IRefinedSeries refinedSeries);
	}
	public interface ISideBySideStackedBarSeriesView : ISideBySideBarSeriesView, ISupportSeriesGroups {
		object StackedGroup { get; set; }
	}
	public interface IStepSeriesView {
		bool InvertedStep { get; set; }
	}
	public interface IRefinedSeries {
		ISeries Series { get; }
		ISeriesView SeriesView { get; }
		IList<RefinedPoint> Points { get; }
		ActualScaleType ArgumentScaleType { get; }
		ActualScaleType ValueScaleType { get; }
		bool IsFirstInContainer { get; }
		bool IsPointsAutoGenerated { get; }
		int MinVisiblePointIndex { get; }
		int MaxVisiblePointIndex { get; }
		double MinArgument { get; }
		double MaxArgument { get; }
		RefinedPoint GetMinPoint(double argument);
		RefinedPoint GetMaxPoint(double argument);
		IList<RefinedPoint> GetDrawingPoints();
		bool IsSameContainers(IRefinedSeries refinedSeries);
		List<RefinedPoint> FindAllPointsWithSameArgument(RefinedPoint refinedPoint);
	}
	public interface IBasePoint {
		ISeriesPoint SeriesPoint { get; }
		IEnumerable<RefinedPoint> Children { get; }
		bool IsEmpty { get; }
	}
	public interface IArgumentPoint : IBasePoint {
		double Argument { get; set; }
	}
	public interface IValuePoint : IArgumentPoint {
		double Value { get; set; }
	}
	public interface IFunnelPoint : IValuePoint {
		double NormalizedValue { get; }
	}
	public interface IPiePoint : IArgumentPoint {
		double NormalizedValue { get; }
		bool IsMaxPoint { get; }
		bool IsMinPoint { get; }
	}
	public interface IXYPoint : IValuePoint {
	}
	public interface IXYWPoint : IXYPoint {
		double Weight { get; set; }
		double Size { get; }
	}
	public interface IFinancialPoint : IArgumentPoint {
		double Open { get; set; }
		double Close { get; set; }
		double Low { get; set; }
		double High { get; set; }
	}
	public interface IRangePoint : IArgumentPoint {
		double Value1 { get; set; }
		double Value2 { get; set; }
		double Min { get; }
		double Max { get; }
	}
	public interface IStackedPoint : IXYPoint {
		double MinValue { get; }
		double MaxValue { get; }
		double TotalValue { get; }
		double TotalMinValue { get; }
		double TotalMaxValue { get; }
	}
	public interface IFullStackedPoint : IStackedPoint {
		double NormalizedValue { get; }
	}
	public interface ISideBySidePoint : IXYPoint {
		int FixedOffset { get; }
		double Offset { get; }
		double BarWidth { get; }
	}
	public interface INestedDoughnutRefinedSeries {
		double StartOffset { get; }
		double StartOffsetInPixels { get; }
		double HoleRadius { get; }
		double TotalGroupIndentInPixels { get; }
		double NormalizedWeight { get; }
		double ExplodedFactor { get; }
		bool IsExploded { get; }
	}
	public interface IChartDataContainer {
		ISeriesBase SeriesTemplate { get; }
		bool ShouldUseSeriesTemplate { get; }
		bool DesignMode { get; }
	}
	public interface IPatternValuesSource {
		object Argument { get; }
		object Value { get; }
		double PercentValue { get; }
		string Series { get; }
		object SeriesGroup { get; }
		object Value1 { get; }
		object Value2 { get; }
		object ValueDuration { get; }
		double HighValue { get; }
		double LowValue { get; }
		double OpenValue { get; }
		double CloseValue { get; }
		object PointHint { get; }
		double Weight { get; }
	}
}
