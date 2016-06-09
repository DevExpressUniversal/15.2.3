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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using ChartsModel = DevExpress.Charts.Model;
namespace DevExpress.XtraSpreadsheet.Model.ModelChart {
	#region BarChartViewBuilder
	public class BarChartViewBuilder : ModelViewBuilderBase {
		public BarChartViewBuilder(ModelChartBuilder modelBuilder, BarChartView view)
			: base(modelBuilder, view) {
		}
		protected new BarChartView View { get { return base.View as BarChartView; } }
		protected override ChartsModel.SeriesModel CreateModelSeries(ISeries series) {
			if (View.Grouping == BarChartGrouping.Stacked)
				return new ChartsModel.StackedBarSeries();
			if (View.Grouping == BarChartGrouping.PercentStacked)
				return new ChartsModel.FullStackedBarSeries();
			return new ChartsModel.SideBySideBarSeries();
		}
		protected override void SetupSeries(ChartsModel.SeriesModel modelSeries, ISeries series) {
			base.SetupSeries(modelSeries, series);
			ChartsModel.BarSeries barSeries = modelSeries as ChartsModel.BarSeries;
			if (barSeries != null) {
				double gapWidth = View.GapWidth;
				if (View.Grouping == BarChartGrouping.Clustered)
					gapWidth /= View.Series.Count;
				barSeries.BarWidth = 100.0 * GetWidthCoeff() / (gapWidth + 100.0);
			}
		}
		double GetWidthCoeff() {
			if (View.Axes != null && View.Axes.Count > 0) {
				DateAxis axis = View.Axes[0] as DateAxis;
				if (axis != null) {
					TimeUnits timeUnits = axis.GetTimeUnits();
					if (timeUnits == TimeUnits.Months)
						return 30.0;
					if (timeUnits == TimeUnits.Years)
						return 365.0;
				}
			}
			return 1.0;
		}
		protected override void SetupSeriesShowInLegend(ChartsModel.SeriesModel modelSeries) {
			modelSeries.ShowInLegend = !IsDeletedFromLegend(ModelBuilder.SeriesIndex);
		}
	}
	#endregion
	#region Bar3DChartViewBuilder
	public class Bar3DChartViewBuilder : ModelViewBuilderBase {
		public Bar3DChartViewBuilder(ModelChartBuilder modelBuilder, Bar3DChartView view)
			: base(modelBuilder, view) {
		}
		protected new Bar3DChartView View { get { return base.View as Bar3DChartView; } }
		protected override List<ISeries> GetSeriesList() {
			List<ISeries> result = base.GetSeriesList();
			if ((View.Grouping == BarChartGrouping.Standard) ||
				(View.Grouping == BarChartGrouping.Clustered && View.BarDirection == BarChartDirection.Bar))
				result.Reverse();
			return result;
		}
		protected override ChartsModel.SeriesModel CreateModelSeries(ISeries series) {
			if (View.Grouping == BarChartGrouping.Stacked)
				return new ChartsModel.StackedBarSeries();
			if (View.Grouping == BarChartGrouping.PercentStacked)
				return new ChartsModel.FullStackedBarSeries();
			if (View.Grouping == BarChartGrouping.Standard)
				return new ChartsModel.ManhattanBarSeries();
			return new ChartsModel.SideBySideBarSeries();
		}
		protected override void SetupSeriesShowInLegend(ChartsModel.SeriesModel modelSeries) {
			modelSeries.ShowInLegend = !IsDeletedFromLegend(ModelBuilder.SeriesIndex);
		}
		protected override void SetupSeries(ChartsModel.SeriesModel modelSeries, ISeries series) {
			base.SetupSeries(modelSeries, series);
			ChartsModel.ISupportBar3DModelSeries bar3dModelSeries = modelSeries as ChartsModel.ISupportBar3DModelSeries;
			if (bar3dModelSeries != null)
				bar3dModelSeries.Model = GetBar3DShape(((BarSeries)series).Shape);
		}
		ChartsModel.Bar3DModel GetBar3DShape(BarShape seriesShape) {
			if (seriesShape == BarShape.Auto)
				seriesShape = View.Shape;
			switch (seriesShape) {
				case BarShape.Cylinder:
					return ChartsModel.Bar3DModel.Cylinder;
				case BarShape.Cone:
				case BarShape.ConeToMax:
					return ChartsModel.Bar3DModel.Cone;
				case BarShape.Pyramid:
				case BarShape.PyramidToMax:
					return ChartsModel.Bar3DModel.Pyramid;
			}
			return ChartsModel.Bar3DModel.Box;
		}
	}
	#endregion
}
