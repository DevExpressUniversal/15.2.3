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

using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChangeChartTypeBuilder
	public class ChangeChartTypeBuilder {
		protected internal virtual void Build(Chart chart, ChartType chartType) {
			ChartBuilderBase builder = ChartBuilderFactory.GetBuilder(chartType);
			ChartViewCollection views = chart.Views;
			IChartView firstView = chart.Views[0];
			if (builder.ViewType != firstView.ViewType)
				InsertNewView(chart, builder, 0);
			else {
				ModifyView(firstView, builder);
				ModifySeries(firstView.Series, builder);
			}
			List<ISeries> series = GetViewsSeries(views, 1);
			AddSeries(views[0], series, builder);
			views.ClearLastViews(1);
			ModifyChart(chart, builder);
		}
		protected void InsertNewView(Chart chart, ChartBuilderBase builder, int index) {
			IChartView view = builder.CreateViewInstance(chart);
			InsertNewViewCore(view, builder, index);
		}
		protected void InsertNewViewCore(IChartView view, ChartBuilderBase builder, int index) {
			ModifyView(view, builder);
			view.Parent.Views.Insert(index, view);
		}
		protected void ModifyView(IChartView view, ChartBuilderBase builder) {
			builder.SetupViewWithoutAxes(view);
			view.Axes = view.Parent.PrimaryAxes;
		}
		protected void ModifySeries(SeriesCollection series, ChartBuilderBase builder) {
			int seriesCount = series.Count;
			for (int i = 0; i < seriesCount; i++)
				builder.SetupSeries(series[i]);
		}
		protected List<ISeries> GetViewsSeries(ChartViewCollection views, int beginViewIndex) {
			List<ISeries> result = new List<ISeries>();
			int viewCount = views.Count;
			for (int i = beginViewIndex; i < viewCount; i++)
				result.AddRange(views[i].Series.InnerList);
			return result;
		}
		protected void AddSeries(IChartView view, List<ISeries> seriesCollection, ChartBuilderBase builder) {
			int seriesCount = seriesCollection.Count;
			for (int i = 0; i < seriesCount; i++) {
				ISeries newSeries = view.CreateSeriesInstance();
				newSeries.CopyFrom(seriesCollection[i]);
				builder.SetupSeries(newSeries);
				view.Series.Add(newSeries);
			}
		}
		protected void ModifyChart(Chart chart, ChartBuilderBase builder) {
			builder.SetupAxes(chart);
			if (chart.Views[0].Is3DView)
				builder.SetupView3D(chart.View3D);
		}
	}
	#endregion
	#region CreateStockVolumeChartTypeBuilder
	public class CreateStockVolumeChartTypeBuilder : ChangeChartTypeBuilder {
		protected internal override void Build(Chart chart, ChartType chartType) {
			VolumeStockChartBuilder builder = ChartBuilderFactory.GetBuilder(chartType) as VolumeStockChartBuilder;
			if (builder == null)
				return;
			ChartViewCollection views = chart.Views;
			IChartView firstView = chart.Views[0];
			if (firstView.ViewType != ChartViewType.Bar)
				CreateViews(chart, builder);
			else
				ModifyViews(chart, builder);
		}
		void CreateViews(Chart chart, VolumeStockChartBuilder builder) {
			BarChartView view = builder.CreateBarViewWithoutAxes(chart);
			InsertNewViewCore(view, builder, 0);
			ChartViewCollection views = chart.Views;
			List<ISeries> series = GetViewsSeries(views, 1);
			AddBarSeries(view, series[0], builder);
			InsertNewView(chart, builder, 1);
			series.RemoveAt(0);
			AddSeries(views[1], series, builder);
			chart.Views.ClearLastViews(2);
			ModifyChart(chart, builder);
		}
		void AddBarSeries(BarChartView view, ISeries series, VolumeStockChartBuilder builder) {
			ISeries newSeries = view.CreateSeriesInstance();
			newSeries.CopyFrom(series);
			builder.SetupSeries(newSeries);
			view.Series.Add(newSeries);
		}
		void ModifyViews(Chart chart, VolumeStockChartBuilder builder) {
			ChartViewCollection views = chart.Views;
			BarChartView barView = views[0] as BarChartView;
			Bar2DChartBuilder.SetupViewWithoutAxes(barView, BarChartDirection.Column, BarChartGrouping.Clustered);
			barView.Axes = barView.Parent.PrimaryAxes;
			builder.SetupSeries(barView.Series[0]);
			IChartView secondView = views[1];
			if (barView.Series.Count > 1 || secondView.ViewType != ChartViewType.Stock)
				CreateStockView(chart, builder);
			else
				ModifyStockView(chart, builder);
		}
		void CreateStockView(Chart chart, VolumeStockChartBuilder builder) {
			InsertNewView(chart, builder, 1);
			ChartViewCollection views = chart.Views;
			IChartView secondView = views[1];
			List<ISeries> series = GetViewsSeries(chart, 1, 1);
			AddSeries(secondView, series, builder);
			views.ClearLastViews(2);
			ModifyChart(chart, builder);
		}
		List<ISeries> GetViewsSeries(Chart chart, int beginViewIndex, int beginViewSeriesIndex) {
			List<ISeries> result = new List<ISeries>();
			ChartViewCollection views = chart.Views;
			SeriesCollection beginViewSeries = views[beginViewIndex].Series;
			int beginViewSeriesCount = beginViewSeries.Count;
			for (int i = beginViewSeriesIndex; i < beginViewSeriesCount; i++)
				result.Add(beginViewSeries[i]);
			result.AddRange(GetViewsSeries(views, beginViewIndex + 1));
			return result;
		}
		void ModifyStockView(Chart chart, VolumeStockChartBuilder builder) {
			ChartViewCollection views = chart.Views;
			IChartView stockView = views[1];
			ModifyView(stockView, builder);
			ModifySeries(stockView.Series, builder);
			List<ISeries> series = GetViewsSeries(views, 2);
			AddSeries(stockView, series, builder);
			views.ClearLastViews(2);
			ModifyChart(chart, builder);
		}
	}
	#endregion
	#region ChangeChartTypeCommand
	public class ChangeChartTypeCommand : ErrorHandledWorksheetCommand {
		#region Static Members
		static List<ChangeChartTypeBuilder> builders = GetBuilders();
		static List<ChangeChartTypeBuilder> GetBuilders() {
			List<ChangeChartTypeBuilder> result = new List<ChangeChartTypeBuilder>();
			result.Add(new ChangeChartTypeBuilder());
			result.Add(new CreateStockVolumeChartTypeBuilder());
			return result;
		}
		#endregion
		#region Fields
		readonly Chart chart;
		readonly ChartType chartType;
		#endregion
		public ChangeChartTypeCommand(IErrorHandler errorHandler, Chart chart, ChartType chartType)
			: base(chart.DocumentModel, errorHandler) {
			this.chart = chart;
			this.chartType = chartType;
		}
		bool IsStockVolume { 
			get { 
				return 
					chartType == ChartType.StockVolumeHighLowClose || 
					chartType == ChartType.StockVolumeOpenHighLowClose; 
			} 
		}
		bool IsSurface { 
			get {
				return 
					chartType == ChartType.Surface || chartType == ChartType.Surface3D ||
					chartType == ChartType.SurfaceWireframe || chartType == ChartType.Surface3DWireframe;
			}
		}
		protected internal override void ExecuteCore() {
			ChangeChartTypeBuilder builder = builders[IsStockVolume ? 1 : 0];
			builder.Build(chart, chartType);
			ChartAxisHelper.CheckArgumentAxis(chart);
		}
		#region Validate
		protected internal override bool Validate() {
			if (!ChartBuilderFactory.HasBuilder(chartType))
				return false;
			int seriesCount = chart.GetSeriesList().Count;
			if (seriesCount == 1 && IsSurface)
				return HandleError(new ModelErrorInfo(ModelErrorType.SurfaceChartMustContainAtLeastTwoSeries));
			if (seriesCount != 3 && chartType == ChartType.StockHighLowClose)
				return HandleError(new ModelErrorInfo(ModelErrorType.IncorrectCreateStockHighLowCloseChart));
			if (seriesCount != 4 && chartType == ChartType.StockOpenHighLowClose)
				return HandleError(new ModelErrorInfo(ModelErrorType.IncorrectCreateStockOpenHighLowCloseChart));
			if (seriesCount != 4 && chartType == ChartType.StockVolumeHighLowClose)
				return HandleError(new ModelErrorInfo(ModelErrorType.IncorrectCreateStockVolumeHighLowCloseChart));
			if (seriesCount != 5 && chartType == ChartType.StockVolumeOpenHighLowClose)
				return HandleError(new ModelErrorInfo(ModelErrorType.IncorrectCreateStockVolumeOpenHighLowCloseChart));
			return true;
		}
		#endregion
	}
	#endregion
}
