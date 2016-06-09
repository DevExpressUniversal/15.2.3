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

using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChartViewSeriesDirection
	public enum ChartViewSeriesDirection {
		Horizontal,
		Vertical,
	}
	#endregion
	#region ChartViewType
	public enum ChartViewType {
		None,
		Area,
		Area3D,
		Bar,
		Bar3D,
		Bubble,
		Doughnut,
		Line,
		Line3D,
		OfPie,
		Pie,
		Pie3D,
		Radar,
		Scatter,
		Stock,
		Surface,
		Surface3D
	}
	#endregion
	#region AxisGroupType
	public enum AxisGroupType {
		Empty,
		CategoryValue,
		CategoryValueSeries,
		XY,
	}
	#endregion
	#region IChartView
	public interface IChartView : ISupportsCopyFrom<IChartView> {
		IChart Parent { get; }
		AxisGroup Axes { get; set; }
		SeriesCollection Series { get; }
		ChartType ChartType { get; }
		ChartViewType ViewType { get; }
		bool Is3DView { get; }
		bool IsSingleSeriesView { get; }
		int IndexOfView { get; }
		bool IsContained { get; }
		AxisGroupType AxesType { get; }
		IChartView CloneTo(IChart parent);
		void CopyFromWithoutSeries(IChartView value);
		void Visit(IChartViewVisitor visitor);
		ChartViewSeriesDirection SeriesDirection { get; }
		DataLabelPosition DefaultDataLabelPosition { get; }
		void ResetToStyle();
		ISeries CreateSeriesInstance();
		void OnRangeInserting(InsertRangeNotificationContext context);
		void OnRangeRemoving(RemoveRangeNotificationContext context);
	}
	#endregion
	#region IChartViewVisitor
	public interface IChartViewVisitor {
		void Visit(Area3DChartView view);
		void Visit(AreaChartView view);
		void Visit(Bar3DChartView view);
		void Visit(BarChartView view);
		void Visit(BubbleChartView view);
		void Visit(DoughnutChartView view);
		void Visit(Line3DChartView view);
		void Visit(LineChartView view);
		void Visit(OfPieChartView view);
		void Visit(Pie3DChartView view);
		void Visit(PieChartView view);
		void Visit(RadarChartView view);
		void Visit(ScatterChartView view);
		void Visit(StockChartView view);
		void Visit(Surface3DChartView view);
		void Visit(SurfaceChartView view);
	}
	#endregion
	#region ISupportsHiLowLines
	public interface ISupportsHiLowLines {
		bool ShowHiLowLines { get; set; }
		ShapeProperties HiLowLinesProperties { get; }
	}
	#endregion
	#region ISupportsDropLines
	public interface ISupportsDropLines {
		bool ShowDropLines { get; set; }
		ShapeProperties DropLinesProperties { get; }
	}
	#endregion
	#region ISupportsUpDownBars
	public interface ISupportsUpDownBars {
		bool ShowUpDownBars { get; set; }
		ChartUpDownBars UpDownBars { get; }
	}
	#endregion
	#region ISupportsSeriesLines
	public interface ISupportsSeriesLines {
		bool IsSeriesLinesApplicable { get; }
		SeriesLinesCollection SeriesLines { get; }
	}
	#endregion
	#region IChartViewWithGapWidth
	public interface IChartViewWithGapWidth {
		int GapWidth { get; set; }
		void SetGapWidthCore(int value);
	}
	#endregion
	#region IChartViewWithGapDepth
	public interface IChartViewWithGapDepth {
		int GapDepth { get; set; }
		void SetGapDepthCore(int value);
	}
	#endregion
}
