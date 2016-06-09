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

using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using Model = DevExpress.Charts.Model;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public abstract class SeriesConfiguratorFactoryBase {
		Dictionary<Type, Type> viewTypes;
		Dictionary<Type, Type> configuratorTypes;
		protected Dictionary<Type, Type> SupportedViewTypes { get { return viewTypes; } }
		protected Dictionary<Type, Type> SupportedConfiguratorTypes { get { return configuratorTypes; } }
		public SeriesConfiguratorFactoryBase() {
			viewTypes = new Dictionary<Type, Type>();
			configuratorTypes = new Dictionary<Type, Type>();
			FillSupportedViewTypes();
			FillSupportedConfiguratorTypes();
		}
		protected abstract void FillSupportedViewTypes();
		protected abstract void FillSupportedConfiguratorTypes();
		public SeriesConfiguratorBase CreateSeriesConfigurator(SeriesViewBase view) {
			Type configuratorType;
			if(configuratorTypes.TryGetValue(view.GetType(), out configuratorType))
				return (SeriesConfiguratorBase)Activator.CreateInstance(configuratorType);
			return null;
		}
		public SeriesViewBase CreateSeriesView(Model.SeriesModel series) {
			Type viewType;
			if(viewTypes.TryGetValue(series.GetType(), out viewType))
				return (SeriesViewBase)Activator.CreateInstance(viewType);
			return null;
		}
	}
	public class SeriesConfiguratorFactory : SeriesConfiguratorFactoryBase {
		protected override void FillSupportedViewTypes() {
			SupportedViewTypes[typeof(Model.BubbleSeries)] = typeof(BubbleSeriesView);
			SupportedViewTypes[typeof(Model.PointSeries)] = typeof(PointSeriesView);
			SupportedViewTypes[typeof(Model.RadarPointSeries)] = typeof(RadarPointSeriesView);
			SupportedViewTypes[typeof(Model.PolarPointSeries)] = typeof(PolarPointSeriesView);
			SupportedViewTypes[typeof(Model.ScatterLineSeries)] = typeof(ScatterLineSeriesView);
			SupportedViewTypes[typeof(Model.LineSeries)] = typeof(LineSeriesView);
			SupportedViewTypes[typeof(Model.SplineSeries)] = typeof(SplineSeriesView);
			SupportedViewTypes[typeof(Model.StepLineSeries)] = typeof(StepLineSeriesView);
			SupportedViewTypes[typeof(Model.RadarLineSeries)] = typeof(RadarLineSeriesView);
			SupportedViewTypes[typeof(Model.PolarLineSeries)] = typeof(PolarLineSeriesView);
			SupportedViewTypes[typeof(Model.StackedLineSeries)] = typeof(StackedLineSeriesView);
			SupportedViewTypes[typeof(Model.FullStackedLineSeries)] = typeof(FullStackedLineSeriesView);
			SupportedViewTypes[typeof(Model.SideBySideBarSeries)] = typeof(SideBySideBarSeriesView);
			SupportedViewTypes[typeof(Model.StackedBarSeries)] = typeof(StackedBarSeriesView);
			SupportedViewTypes[typeof(Model.FullStackedBarSeries)] = typeof(FullStackedBarSeriesView);
			SupportedViewTypes[typeof(Model.SideBySideStackedBarSeries)] = typeof(SideBySideStackedBarSeriesView);
			SupportedViewTypes[typeof(Model.SideBySideFullStackedBarSeries)] = typeof(SideBySideFullStackedBarSeriesView);
			SupportedViewTypes[typeof(Model.RangeBarSeries)] = typeof(OverlappedRangeBarSeriesView);
			SupportedViewTypes[typeof(Model.SideBySideRangeBarSeries)] = typeof(SideBySideRangeBarSeriesView);
			SupportedViewTypes[typeof(Model.AreaSeries)] = typeof(AreaSeriesView);
			SupportedViewTypes[typeof(Model.SplineAreaSeries)] = typeof(SplineAreaSeriesView);
			SupportedViewTypes[typeof(Model.StepAreaSeries)] = typeof(StepAreaSeriesView);
			SupportedViewTypes[typeof(Model.RangeAreaSeries)] = typeof(RangeAreaSeriesView);
			SupportedViewTypes[typeof(Model.StackedAreaSeries)] = typeof(StackedAreaSeriesView);
			SupportedViewTypes[typeof(Model.StackedSplineAreaSeries)] = typeof(StackedSplineAreaSeriesView);
			SupportedViewTypes[typeof(Model.FullStackedAreaSeries)] = typeof(FullStackedAreaSeriesView);
			SupportedViewTypes[typeof(Model.FullStackedSplineAreaSeries)] = typeof(FullStackedSplineAreaSeriesView);
			SupportedViewTypes[typeof(Model.RadarAreaSeries)] = typeof(RadarAreaSeriesView);
			SupportedViewTypes[typeof(Model.PolarAreaSeries)] = typeof(PolarAreaSeriesView);
			SupportedViewTypes[typeof(Model.StockSeries)] = typeof(StockSeriesView);
			SupportedViewTypes[typeof(Model.CandleStickSeries)] = typeof(CandleStickSeriesView);
			SupportedViewTypes[typeof(Model.PieSeries)] = typeof(PieSeriesView);
			SupportedViewTypes[typeof(Model.DonutSeries)] = typeof(DoughnutSeriesView);
			SupportedViewTypes[typeof(Model.NestedDonutSeries)] = typeof(NestedDoughnutSeriesView);
		}
		protected override void FillSupportedConfiguratorTypes() {
			SupportedConfiguratorTypes[typeof(BubbleSeriesView)] = typeof(BubbleSeriesConfigurator);
			SupportedConfiguratorTypes[typeof(PointSeriesView)] = typeof(PointBasedSeriesConfigurator<PointSeriesView>);
			SupportedConfiguratorTypes[typeof(RadarPointSeriesView)] = typeof(RadarPointBasedSeriesConfigurator<RadarPointSeriesView>);
			SupportedConfiguratorTypes[typeof(PolarPointSeriesView)] = typeof(RadarPointBasedSeriesConfigurator<PolarPointSeriesView>);
			SupportedConfiguratorTypes[typeof(ScatterLineSeriesView)] = typeof(LineBasedSeriesConfigurator<ScatterLineSeriesView>);
			SupportedConfiguratorTypes[typeof(LineSeriesView)] = typeof(LineBasedSeriesConfigurator<LineSeriesView>);
			SupportedConfiguratorTypes[typeof(SplineSeriesView)] = typeof(LineBasedSeriesConfigurator<SplineSeriesView>);
			SupportedConfiguratorTypes[typeof(StepLineSeriesView)] = typeof(LineBasedSeriesConfigurator<StepLineSeriesView>);
			SupportedConfiguratorTypes[typeof(StackedLineSeriesView)] = typeof(LineBasedSeriesConfigurator<StackedLineSeriesView>);
			SupportedConfiguratorTypes[typeof(FullStackedLineSeriesView)] = typeof(LineBasedSeriesConfigurator<FullStackedLineSeriesView>);
			SupportedConfiguratorTypes[typeof(RadarLineSeriesView)] = typeof(RadarLineBasedSeriesConfigurator<RadarLineSeriesView>);
			SupportedConfiguratorTypes[typeof(PolarLineSeriesView)] = typeof(RadarLineBasedSeriesConfigurator<PolarLineSeriesView>);
			SupportedConfiguratorTypes[typeof(SideBySideBarSeriesView)] = typeof(BarBasedSeriesConfigurator<SideBySideBarSeriesView>);
			SupportedConfiguratorTypes[typeof(StackedBarSeriesView)] = typeof(BarBasedSeriesConfigurator<StackedBarSeriesView>);
			SupportedConfiguratorTypes[typeof(FullStackedBarSeriesView)] = typeof(BarBasedSeriesConfigurator<FullStackedBarSeriesView>);
			SupportedConfiguratorTypes[typeof(SideBySideStackedBarSeriesView)] = typeof(BarBasedSeriesConfigurator<SideBySideStackedBarSeriesView>);
			SupportedConfiguratorTypes[typeof(SideBySideFullStackedBarSeriesView)] = typeof(BarBasedSeriesConfigurator<SideBySideFullStackedBarSeriesView>);
			SupportedConfiguratorTypes[typeof(OverlappedRangeBarSeriesView)] = typeof(BarBasedSeriesConfigurator<OverlappedRangeBarSeriesView>);
			SupportedConfiguratorTypes[typeof(SideBySideRangeBarSeriesView)] = typeof(BarBasedSeriesConfigurator<SideBySideRangeBarSeriesView>);
			SupportedConfiguratorTypes[typeof(AreaSeriesView)] = typeof(AreaBasedSeriesConfigurator<AreaSeriesView>);
			SupportedConfiguratorTypes[typeof(SplineAreaSeriesView)] = typeof(AreaBasedSeriesConfigurator<SplineAreaSeriesView>);
			SupportedConfiguratorTypes[typeof(StepAreaSeriesView)] = typeof(AreaBasedSeriesConfigurator<StepAreaSeriesView>);
			SupportedConfiguratorTypes[typeof(RangeAreaSeriesView)] = typeof(AreaBasedSeriesConfigurator<RangeAreaSeriesView>);
			SupportedConfiguratorTypes[typeof(StackedAreaSeriesView)] = typeof(AreaBasedSeriesConfigurator<StackedAreaSeriesView>);
			SupportedConfiguratorTypes[typeof(StackedSplineAreaSeriesView)] = typeof(AreaBasedSeriesConfigurator<StackedSplineAreaSeriesView>);
			SupportedConfiguratorTypes[typeof(FullStackedAreaSeriesView)] = typeof(AreaBasedSeriesConfigurator<FullStackedAreaSeriesView>);
			SupportedConfiguratorTypes[typeof(RadarAreaSeriesView)] = typeof(RadarAreaBasedSeriesConfigurator<RadarAreaSeriesView>);
			SupportedConfiguratorTypes[typeof(PolarAreaSeriesView)] = typeof(RadarAreaBasedSeriesConfigurator<PolarAreaSeriesView>);
			SupportedConfiguratorTypes[typeof(StockSeriesView)] = typeof(FinancialSeriesConfigurator<StockSeriesView>);
			SupportedConfiguratorTypes[typeof(CandleStickSeriesView)] = typeof(FinancialSeriesConfigurator<CandleStickSeriesView>);
			SupportedConfiguratorTypes[typeof(PieSeriesView)] = typeof(PieSeriesConfigurator);
			SupportedConfiguratorTypes[typeof(DoughnutSeriesView)] = typeof(DonutSeriesConfigurator);
			SupportedConfiguratorTypes[typeof(NestedDoughnutSeriesView)] = typeof(NestedDonutSeriesConfigurator);
		}
	}
	public class Series3DConfiguratorFactory : SeriesConfiguratorFactoryBase {
		protected override void FillSupportedViewTypes() {
			SupportedViewTypes[typeof(Model.LineSeries)] = typeof(Line3DSeriesView);
			SupportedViewTypes[typeof(Model.SplineSeries)] = typeof(Spline3DSeriesView);
			SupportedViewTypes[typeof(Model.StepLineSeries)] = typeof(StepLine3DSeriesView);
			SupportedViewTypes[typeof(Model.StackedLineSeries)] = typeof(StackedLine3DSeriesView);
			SupportedViewTypes[typeof(Model.FullStackedLineSeries)] = typeof(FullStackedLine3DSeriesView);
			SupportedViewTypes[typeof(Model.SideBySideBarSeries)] = typeof(SideBySideBar3DSeriesView);
			SupportedViewTypes[typeof(Model.StackedBarSeries)] = typeof(StackedBar3DSeriesView);
			SupportedViewTypes[typeof(Model.FullStackedBarSeries)] = typeof(FullStackedBar3DSeriesView);
			SupportedViewTypes[typeof(Model.SideBySideStackedBarSeries)] = typeof(SideBySideStackedBar3DSeriesView);
			SupportedViewTypes[typeof(Model.SideBySideFullStackedBarSeries)] = typeof(SideBySideFullStackedBar3DSeriesView);
			SupportedViewTypes[typeof(Model.ManhattanBarSeries)] = typeof(ManhattanBarSeriesView);
			SupportedViewTypes[typeof(Model.AreaSeries)] = typeof(Area3DSeriesView);
			SupportedViewTypes[typeof(Model.SplineAreaSeries)] = typeof(SplineArea3DSeriesView);
			SupportedViewTypes[typeof(Model.StepAreaSeries)] = typeof(StepArea3DSeriesView);
			SupportedViewTypes[typeof(Model.RangeAreaSeries)] = typeof(RangeArea3DSeriesView);
			SupportedViewTypes[typeof(Model.StackedAreaSeries)] = typeof(StackedArea3DSeriesView);
			SupportedViewTypes[typeof(Model.StackedSplineAreaSeries)] = typeof(StackedSplineAreaSeriesView);
			SupportedViewTypes[typeof(Model.FullStackedAreaSeries)] = typeof(FullStackedArea3DSeriesView);
			SupportedViewTypes[typeof(Model.FullStackedSplineAreaSeries)] = typeof(FullStackedSplineArea3DSeriesView);
			SupportedViewTypes[typeof(Model.PieSeries)] = typeof(Pie3DSeriesView);
			SupportedViewTypes[typeof(Model.DonutSeries)] = typeof(Doughnut3DSeriesView);
		}
		protected override void FillSupportedConfiguratorTypes() {
			SupportedConfiguratorTypes[typeof(Line3DSeriesView)] = typeof(Line3DBasedSeriesConfigurator<Line3DSeriesView>);
			SupportedConfiguratorTypes[typeof(Spline3DSeriesView)] = typeof(Line3DBasedSeriesConfigurator<Spline3DSeriesView>);
			SupportedConfiguratorTypes[typeof(StepLine3DSeriesView)] = typeof(Line3DBasedSeriesConfigurator<StepLine3DSeriesView>);
			SupportedConfiguratorTypes[typeof(StackedLine3DSeriesView)] = typeof(Line3DBasedSeriesConfigurator<StackedLine3DSeriesView>);
			SupportedConfiguratorTypes[typeof(FullStackedLine3DSeriesView)] = typeof(Line3DBasedSeriesConfigurator<FullStackedLine3DSeriesView>);
			SupportedConfiguratorTypes[typeof(SideBySideBar3DSeriesView)] = typeof(Bar3DBasedSeriesConfigurator<SideBySideBar3DSeriesView>);
			SupportedConfiguratorTypes[typeof(StackedBar3DSeriesView)] = typeof(Bar3DBasedSeriesConfigurator<StackedBar3DSeriesView>);
			SupportedConfiguratorTypes[typeof(FullStackedBar3DSeriesView)] = typeof(Bar3DBasedSeriesConfigurator<FullStackedBar3DSeriesView>);
			SupportedConfiguratorTypes[typeof(SideBySideStackedBar3DSeriesView)] = typeof(Bar3DBasedSeriesConfigurator<SideBySideStackedBar3DSeriesView>);
			SupportedConfiguratorTypes[typeof(SideBySideFullStackedBar3DSeriesView)] = typeof(Bar3DBasedSeriesConfigurator<SideBySideFullStackedBar3DSeriesView>);
			SupportedConfiguratorTypes[typeof(ManhattanBarSeriesView)] = typeof(Bar3DBasedSeriesConfigurator<ManhattanBarSeriesView>);
			SupportedConfiguratorTypes[typeof(Area3DSeriesView)] = typeof(Area3DBasedSeriesConfigurator<Area3DSeriesView>);
			SupportedConfiguratorTypes[typeof(SplineArea3DSeriesView)] = typeof(Area3DBasedSeriesConfigurator<SplineArea3DSeriesView>);
			SupportedConfiguratorTypes[typeof(StepArea3DSeriesView)] = typeof(Area3DBasedSeriesConfigurator<StepArea3DSeriesView>);
			SupportedConfiguratorTypes[typeof(RangeArea3DSeriesView)] = typeof(Area3DBasedSeriesConfigurator<RangeArea3DSeriesView>);
			SupportedConfiguratorTypes[typeof(StackedArea3DSeriesView)] = typeof(Area3DBasedSeriesConfigurator<StackedArea3DSeriesView>);
			SupportedConfiguratorTypes[typeof(StackedSplineArea3DSeriesView)] = typeof(Area3DBasedSeriesConfigurator<StackedSplineArea3DSeriesView>);
			SupportedConfiguratorTypes[typeof(FullStackedArea3DSeriesView)] = typeof(Area3DBasedSeriesConfigurator<FullStackedArea3DSeriesView>);
			SupportedConfiguratorTypes[typeof(FullStackedSplineArea3DSeriesView)] = typeof(Area3DBasedSeriesConfigurator<FullStackedSplineArea3DSeriesView>);
			SupportedConfiguratorTypes[typeof(Pie3DSeriesView)] = typeof(GenericPie3DSeriesConfigurator<Pie3DSeriesView>);
			SupportedConfiguratorTypes[typeof(Doughnut3DSeriesView)] = typeof(GenericPie3DSeriesConfigurator<Doughnut3DSeriesView>);
		}
	}
}
