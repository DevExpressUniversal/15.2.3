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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using System.Linq;
namespace DevExpress.DashboardWin.Native {
	public class ChartSeriesOptions : IChartSeriesOptions {
		static SimpleSeriesType[] allovedTypesForShowPointMarkers = {
			SimpleSeriesType.Line,
			SimpleSeriesType.Spline,
			SimpleSeriesType.StepLine,
			SimpleSeriesType.StackedLine,
			SimpleSeriesType.SplineArea,
			SimpleSeriesType.Area,
			SimpleSeriesType.StepArea,
		};
		static SimpleSeriesType[] stackedTypes = {
			SimpleSeriesType.FullStackedLine,
			SimpleSeriesType.StackedSplineArea,
			SimpleSeriesType.FullStackedSplineArea,
			SimpleSeriesType.StackedArea,
			SimpleSeriesType.FullStackedArea
		};
		static SimpleSeriesType[] barTypes = {
			SimpleSeriesType.Bar,
			SimpleSeriesType.FullStackedBar,
			SimpleSeriesType.StackedBar
		};
		static bool IsRangeArea(ChartSeriesConverter converter) {
			ChartRangeSeriesConverter rangeConverter = converter as ChartRangeSeriesConverter;
			if(rangeConverter != null && rangeConverter.SeriesType == RangeSeriesType.RangeArea)
				return true;
			return false;
		}
		public static bool ShowPointMarkersEnable(ChartSeriesConverter converter) {
			if(IsRangeArea(converter))
				return true;
			ChartSimpleSeriesConverter simpleConverter = converter as ChartSimpleSeriesConverter;
			return simpleConverter != null && allovedTypesForShowPointMarkers.Contains(simpleConverter.SeriesType);
		}
		public static bool IgnoreEmptyPointsEnable(ChartSeriesConverter converter) {
			if(IsRangeArea(converter))
				return true;
			ChartSimpleSeriesConverter simpleConverter = converter as ChartSimpleSeriesConverter;
			if(simpleConverter != null && stackedTypes.Contains(simpleConverter.SeriesType))
				return true;
			return ShowPointMarkersEnable(converter);
		}
		public static bool PositionEnable(ChartSeriesConverter converter) {
			ChartSimpleSeriesConverter simpleConverter = converter as ChartSimpleSeriesConverter;
			return simpleConverter != null && simpleConverter.SeriesType == SimpleSeriesType.Bar || converter is ChartWeightedSeriesConverter;
		}
		public static bool BarPointLabelOptionsEnable(ChartSeriesConverter converter) {
			ChartSimpleSeriesConverter simpleConverter = converter as ChartSimpleSeriesConverter;
			return simpleConverter != null && barTypes.Contains(simpleConverter.SeriesType);
		}
		public static bool BubblePointLabelOptionsEnable(ChartSeriesConverter converter) {
			return converter is ChartWeightedSeriesConverter;
		}
		ChartSelectorContext context;
		bool plotOnSecondaryAxis;
		bool ignoreEmptyPoints;
		bool showPointMarkers;
		PointLabelOptions pointLabel;
		public bool PlotOnSecondaryAxis {
			get { return plotOnSecondaryAxis; }
			set { plotOnSecondaryAxis = value; }
		}
		public bool IgnoreEmptyPoints {
			get { return ignoreEmptyPoints; }
			set { ignoreEmptyPoints = value; }
		}
		public bool ShowPointMarkers {
			get { return showPointMarkers; }
			set { showPointMarkers = value; }
		}
		public PointLabelOptions PointLabelOptions {
			get { return pointLabel; }
		}
		public ChartSeriesOptions(ChartSelectorContext context, ChartSeries series) {
			this.context = context;
			this.plotOnSecondaryAxis = series.PlotOnSecondaryAxis;
			this.ignoreEmptyPoints = series.IgnoreEmptyPoints;
			this.showPointMarkers = series.ShowPointMarkers;
			this.pointLabel = new PointLabelOptions();
			this.pointLabel.Assign(series.PointLabelOptions);
		}
		ChartSeriesPlotOnSecondaryAxisOperation PlotOnSecondaryAxisOperation { get { return new ChartSeriesPlotOnSecondaryAxisOperation(plotOnSecondaryAxis); } }
		ChartSeriesIgnoreEmptyPointsOperation IgnoreEmptyPointsOperation { get { return new ChartSeriesIgnoreEmptyPointsOperation(ignoreEmptyPoints); } }
		ChartSeriesShowPointMarkersOperation ShowPointMarkersOperation { get { return new ChartSeriesShowPointMarkersOperation(showPointMarkers); } }
		ChartSeriesPointLabelsOperation PointLabelsOperation { get { return new ChartSeriesPointLabelsOperation(pointLabel); } }
		public void AddRedoRecords(SelectorContextHistoryItem historyItem, int groupIndex) {
			context.AddRedoRecord(historyItem, groupIndex, PlotOnSecondaryAxisOperation);
			context.AddRedoRecord(historyItem, groupIndex, IgnoreEmptyPointsOperation);
			context.AddRedoRecord(historyItem, groupIndex, ShowPointMarkersOperation);
			context.AddRedoRecord(historyItem, groupIndex, PointLabelsOperation);
		}
		public void AddUndoRecords(SelectorContextHistoryItem historyItem, int groupIndex) {
			context.AddUndoRecord(historyItem, groupIndex, PlotOnSecondaryAxisOperation);
			context.AddUndoRecord(historyItem, groupIndex, IgnoreEmptyPointsOperation);
			context.AddUndoRecord(historyItem, groupIndex, ShowPointMarkersOperation);
			context.AddUndoRecord(historyItem, groupIndex, PointLabelsOperation);
		}
	}
}
