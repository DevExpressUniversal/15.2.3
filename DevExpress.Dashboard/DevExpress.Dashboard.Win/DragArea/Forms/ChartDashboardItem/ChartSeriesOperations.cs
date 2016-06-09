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
namespace DevExpress.DashboardWin.Native {
	public interface ICustomOperation<THolder> where THolder : class, IDataItemHolder {
		void Perform(THolder holder);
	}
	public abstract class ChartSeriesOperation<TValue> : ICustomOperation<ChartSeries> {
		readonly TValue value;
		protected TValue Value { get { return value; } }
		protected ChartSeriesOperation(TValue value) {
			this.value = value;
		}
		void ICustomOperation<ChartSeries>.Perform(ChartSeries series) {
			Perform(series);
		}
		protected abstract void Perform(ChartSeries series);
	}
	public class ChartSeriesPlotOnSecondaryAxisOperation : ChartSeriesOperation<bool> {
		public ChartSeriesPlotOnSecondaryAxisOperation(bool plotOnSecondaryAxis)
			: base(plotOnSecondaryAxis) {
		}
		protected override void Perform(ChartSeries series) {
			series.PlotOnSecondaryAxis = Value;
		}
	}
	public class ChartSeriesIgnoreEmptyPointsOperation : ChartSeriesOperation<bool> {
		public ChartSeriesIgnoreEmptyPointsOperation(bool ignoreEmptyPoints)
			: base(ignoreEmptyPoints) {
		}
		protected override void Perform(ChartSeries series) {
			series.IgnoreEmptyPoints = Value;
		}
	}
	public class ChartSeriesShowPointMarkersOperation : ChartSeriesOperation<bool> {
		public ChartSeriesShowPointMarkersOperation(bool showPointMarkers)
			: base(showPointMarkers) {
		}
		protected override void Perform(ChartSeries series) {
			series.ShowPointMarkers = Value;
		}
	}
	public class  ChartSeriesPointLabelsOperation: ChartSeriesOperation<PointLabelOptions> {
		public ChartSeriesPointLabelsOperation(PointLabelOptions pointLabels)
			: base(pointLabels) {
		}
		protected override void Perform(ChartSeries series) {
			series.PointLabelOptions.Assign(Value);
		}
	}
	public class SimpleSeriesTypeOperation : ChartSeriesOperation<SimpleSeriesType>, ICustomOperation<SimpleSeries> {
		public SimpleSeriesTypeOperation(SimpleSeriesType seriesType)
			: base(seriesType) {
		}
		protected override void Perform(ChartSeries series) {
			((SimpleSeries)series).SeriesType = Value;
		}
		void ICustomOperation<SimpleSeries>.Perform(SimpleSeries holder) {
			Perform(holder);
		}
	}
	public class RangeSeriesTypeOperation : ChartSeriesOperation<RangeSeriesType> {
		public RangeSeriesTypeOperation(RangeSeriesType seriesType)
			: base(seriesType) {
		}
		protected override void Perform(ChartSeries series) {
			((RangeSeries)series).SeriesType = Value;
		}
	}
	public class OpenHighLowCloseSeriesTypeOperation : ChartSeriesOperation<OpenHighLowCloseSeriesType> {
		public OpenHighLowCloseSeriesTypeOperation(OpenHighLowCloseSeriesType seriesType)
			: base(seriesType) {
		}
		protected override void Perform(ChartSeries series) {
			((OpenHighLowCloseSeries)series).SeriesType = Value;
		}
	}
}
