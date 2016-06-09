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
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public class SpreadsheetChartStyles : IDisposable {
		public const int PresetsCount = 48;
		protected const int ChartPresetStyleWidth = 93;
		protected const int ChartPresetStyleHeight = 56;
		protected DocumentModel thumbnailDocumentModel;
		protected Size ImageSize;
		protected DocumentModel DocumentModel { get; private set; }
		protected ASPxSpreadsheet Spreadsheet { get; private set; }
		public SpreadsheetChartStyles(ASPxSpreadsheet spreadsheetControl) {
			ImageSize = new Size(ChartPresetStyleWidth, ChartPresetStyleHeight);
			Spreadsheet = spreadsheetControl;
			if(Spreadsheet.GetCurrentWorkSessions() != null)
				DocumentModel = spreadsheetControl.GetCurrentWorkSessions().DocumentModel;
			thumbnailDocumentModel = new DocumentModel();
		}
		public string[] GetChartPresetStylesGallery() {
			string[] result = null;
			IChartControllerFactoryService service = DocumentModel.GetService<IChartControllerFactoryService>();
			if(service != null)
				this.thumbnailDocumentModel.AddService(typeof(IChartControllerFactoryService), service);
			try {
				Chart chart = CreateThumbnailChart(DocumentModel.ActiveSheet.Selection.SelectedChart);
				result = CreateChartImage(chart);
			} finally {
				if(service != null)
					this.thumbnailDocumentModel.RemoveService(typeof(IChartControllerFactoryService));
			}
			return result;
		}
		protected string[] CreateChartImage(Chart chart) {
			string[] chartStylesGallery = new string[PresetsCount];
			if(chart == null || DocumentModel == null)
				return null;
			IChartControllerFactoryService service = DocumentModel.GetService<IChartControllerFactoryService>();
			if(service == null || service.Factory == null || chart.Controller == null || chart.Controller.ChartModel == null)
				return null;
			chart.Antialiasing = ChartAntialiasing.Disabled;
			IDrawingFill[] fillArray = new IDrawingFill[] { chart.ShapeProperties.Fill, chart.ShapeProperties.Outline.Fill, chart.PlotArea.ShapeProperties.Fill };
			chart.ShapeProperties.Fill = DrawingFill.None;
			chart.ShapeProperties.Outline.Fill = DrawingFill.None;
			chart.PlotArea.ShapeProperties.Fill = DrawingFill.None;
			for(int i = 0; i < PresetsCount; i++) {
				chart.Style = i + 1;
				var img = chart.GetChartImage(ImageSize);
				if(i == 30) {
					chart.PlotArea.ShapeProperties.Fill = fillArray[2];
				}
				if(i == 38) {
					chart.ShapeProperties.Fill = fillArray[0];
					chart.ShapeProperties.Outline.Fill = fillArray[1];
				}
				chartStylesGallery[i] = GetImageBytesURL(img.GetImageBytes(DevExpress.Office.Utils.OfficeImageFormat.Png));
			}
			return chartStylesGallery;
		}
		protected string GetImageBytesURL(byte[] imageBytes) {
			return BinaryStorage.GetImageUrl(Spreadsheet, imageBytes, BinaryStorageMode.Cache);
		}
		protected Chart CreateThumbnailChart(Chart originalChart) {
			this.thumbnailDocumentModel.BeginSetContent();
			try {
				this.thumbnailDocumentModel.OfficeTheme = DocumentModel.OfficeTheme.Clone();
				this.thumbnailDocumentModel.Sheets.Add(this.thumbnailDocumentModel.CreateWorksheet());
				this.thumbnailDocumentModel.ActiveSheetIndex = 0;
				Chart result = new Chart(this.thumbnailDocumentModel.ActiveSheet);
				result.BeginUpdate();
				try {
					IChartView view = CreateThumbnailView(result, originalChart.Views[0]);
					if(view == null)
						return null;
					result.Views.Add(view);
				} finally {
					result.EndUpdate();
				}
				return result;
			} finally {
				this.thumbnailDocumentModel.EndSetContent(DocumentModelChangeType.None);
			}
		}
		protected IChartView CreateThumbnailView(Chart targetChart, IChartView originalView) {
			ChartViewThumbnailFactory factory = new ChartViewThumbnailFactory(targetChart);
			originalView.Visit(factory);
			factory.SetupChart();
			return factory.Result;
		}
		#region IDisposable Members
		public void Dispose() {
			if(thumbnailDocumentModel != null) {
				thumbnailDocumentModel.Dispose();
				thumbnailDocumentModel = null;
			}
		}
		#endregion
	}
	public class ChartViewThumbnailFactory : IChartViewVisitor {
		readonly Chart targetChart;
		readonly DocumentModel targetDocumentModel;
		public ChartViewThumbnailFactory(Chart targetChart) {
			Guard.ArgumentNotNull(targetChart, "targetChart");
			this.targetChart = targetChart;
			this.targetDocumentModel = targetChart.DocumentModel;
		}
		public IChartView Result { get; set; }
		void CreateTwoAxes() {
			InsertChartCommandBase.CreateTwoPrimaryAxes(targetChart, false, true, AxisPosition.Bottom, AxisPosition.Left, AxisCrossBetween.Between);
			HideAxes();
		}
		void HideAxes() {
			foreach(AxisBase axis in targetChart.PrimaryAxes)
				axis.Delete = true;
		}
		void AddSeries(SeriesBase series, int[] values) {
			ChartDataReference dataReference = new ChartDataReference(targetDocumentModel, Result.SeriesDirection, true);
			VariantArray array = VariantArray.Create(values.Length, 1);
			for(int i = 0; i < values.Length; i++)
				array[i] = values[i];
			dataReference.CachedValue = VariantValue.FromArray(array);
			series.Values = dataReference;
			int index = Result.Series.Count;
			series.Index = index;
			series.Order = index;
			Result.Series.Add(series);
		}
		void AddBarSeries(int[] values) {
			BarSeries series = new BarSeries(Result);
			AddSeries(series, values);
		}
		void AddLineSeries(int[] values, bool showMarkers) {
			LineSeries series = new LineSeries(Result);
			AddSeries(series, values);
			series.Marker.Symbol = showMarkers ? MarkerStyle.Auto : MarkerStyle.None;
		}
		void AddAreaSeries(int[] values) {
			AreaSeries series = new AreaSeries(Result);
			AddSeries(series, values);
		}
		void AddPieSeries(int[] values, int explosionPercent) {
			PieSeries series = new PieSeries(Result);
			AddSeries(series, values);
			series.Explosion = explosionPercent;
		}
		void AddScatterSeries(int[] values, bool showLines, bool showMarkers, bool smoothLines) {
			ScatterSeries series = new ScatterSeries(Result);
			AddSeries(series, values);
			series.Marker.Symbol = showMarkers ? MarkerStyle.Auto : MarkerStyle.None;
			series.ShapeProperties.Outline.Fill = showLines ? DrawingFill.Automatic : DrawingFill.None;
			series.Smooth = smoothLines;
		}
		void AddBubbleSeries(int[] values, bool bubble3D) {
			BubbleSeries series = new BubbleSeries(Result);
			AddSeries(series, values);
			series.Bubble3D = bubble3D;
		}
		void AddRadarSeries(int[] values, bool showMarkers) {
			RadarSeries series = new RadarSeries(Result);
			AddSeries(series, values);
			series.Marker.Symbol = showMarkers ? MarkerStyle.Auto : MarkerStyle.None;
		}
		void SetupView3D(int xRotation, int yRotation, int perspective, bool rightAngleAxes) {
			View3DOptions view3D = targetChart.View3D;
			view3D.BeginUpdate();
			try {
				view3D.XRotation = xRotation;
				view3D.YRotation = yRotation;
				view3D.Perspective = perspective;
				view3D.RightAngleAxes = rightAngleAxes;
			} finally {
				view3D.EndUpdate();
			}
		}
		#region IChartViewVisitor implementation
		public void Visit(SurfaceChartView view) {
		}
		public void Visit(Surface3DChartView view) {
		}
		public void Visit(StockChartView view) {
			StockChartView thumbnailView = new StockChartView(targetChart);
			Result = thumbnailView;
			CreateTwoAxes();
			if(view.Series.Count > 3)
				AddLineSeries(new int[] { 8, 6 }, false);
			AddLineSeries(new int[] { 12, 10 }, false);
			AddLineSeries(new int[] { 6, 4 }, false);
			AddLineSeries(new int[] { 10, 8 }, false);
		}
		#region Scatter
		public void Visit(ScatterChartView view) {
			ScatterChartView thumbnailView = new ScatterChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			InsertChartCommandBase.CreateTwoPrimaryAxes(targetChart, true, true, AxisPosition.Bottom, AxisPosition.Left, AxisCrossBetween.Between);
			HideAxes();
			bool showLines = CalculateShowLines(view);
			bool showMarkers = CalculateShowMarkers(view);
			bool smoothLines = CalculateSmoothLines(view);
			AddScatterSeries(new int[] { 4, 6, 8 }, showLines, showMarkers, smoothLines);
			if(view.Series.Count > 1)
				AddScatterSeries(new int[] { 8, 10, 5 }, showLines, showMarkers, smoothLines);
		}
		bool CalculateShowLines(ChartViewWithVaryColors view) {
			if(view.Series.Count > 0) {
				SeriesWithErrorBarsAndTrendlines series = view.Series[0] as SeriesWithErrorBarsAndTrendlines;
				if(series != null)
					return series.ShapeProperties.Outline.Fill != DrawingFill.None;
			}
			return false;
		}
		bool CalculateShowMarkers(ChartViewWithVaryColors view) {
			if(view.Series.Count > 0) {
				SeriesWithMarkerAndSmooth series = view.Series[0] as SeriesWithMarkerAndSmooth;
				if(series != null)
					return series.Marker.Symbol != MarkerStyle.None;
			}
			return false;
		}
		bool CalculateSmoothLines(ChartViewWithVaryColors view) {
			if(view.Series.Count > 0) {
				SeriesWithMarkerAndSmooth series = view.Series[0] as SeriesWithMarkerAndSmooth;
				if(series != null)
					return series.Smooth;
			}
			return false;
		}
		#endregion
		#region Radar
		public void Visit(RadarChartView view) {
			RadarChartView thumbnailView = new RadarChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			HideAxes();
			thumbnailView.RadarStyle = view.RadarStyle;
			bool showMarkers = CalculateShowMarkers(view);
			AddRadarSeries(new int[] { 8, 4, 6, 8 }, showMarkers);
			if(view.Series.Count > 1)
				AddRadarSeries(new int[] { 9, 10, 8, 10 }, showMarkers);
		}
		bool CalculateShowMarkers(RadarChartView view) {
			if(view.Series.Count > 0) {
				RadarSeries series = view.Series[0] as RadarSeries;
				if(series != null)
					return series.Marker.Symbol != MarkerStyle.None;
			}
			return false;
		}
		#endregion
		public void Visit(PieChartView view) {
			PieChartView thumbnailView = new PieChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			int explosionPercent = CalculateExplosionPercent(view);
			AddPieSeries(new int[] { 60, 20, 10, 10 }, explosionPercent);
		}
		int CalculateExplosionPercent(ChartViewBase view) {
			if(view.Series.Count > 0) {
				PieSeries pieSeries = view.Series[0] as PieSeries;
				if(pieSeries != null)
					return pieSeries.Explosion;
			}
			return 0;
		}
		public void Visit(Pie3DChartView view) {
			SetupView3D(30, 0, 30, false);
			Pie3DChartView thumbnailView = new Pie3DChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			int explosionPercent = CalculateExplosionPercent(view);
			AddPieSeries(new int[] { 60, 20, 10, 10 }, explosionPercent);
		}
		public void Visit(OfPieChartView view) {
		}
		public void Visit(LineChartView view) {
			LineChartView thumbnailView = new LineChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			CreateTwoAxes();
			bool showMarkers = CalculateShowMarkers(view);
			AddLineSeries(new int[] { 8, 4, 6, 8 }, showMarkers);
			if(view.Series.Count > 1)
				AddLineSeries(new int[] { 9, 10, 8, 10 }, showMarkers);
		}
		public void Visit(Line3DChartView view) {
			SetupView3D(20, 15, 30, false);
			Line3DChartView thumbnailView = new Line3DChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			CreateTwoAxes();
			AddLineSeries(new int[] { 8, 4, 6, 8 }, false);
			if(view.Series.Count > 1)
				AddLineSeries(new int[] { 9, 10, 8, 10 }, false);
		}
		public void Visit(DoughnutChartView view) {
			DoughnutChartView thumbnailView = new DoughnutChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			int explosionPercent = CalculateExplosionPercent(view);
			AddPieSeries(new int[] { 60, 20, 10, 10 }, explosionPercent);
			thumbnailView.HoleSize = view.HoleSize;
		}
		public void Visit(BubbleChartView view) {
			BubbleChartView thumbnailView = new BubbleChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			InsertChartCommandBase.CreateTwoPrimaryAxes(targetChart, true, true, AxisPosition.Bottom, AxisPosition.Left, AxisCrossBetween.Between);
			HideAxes();
			bool bubble3D = CalculateBubble3D(view);
			thumbnailView.Bubble3D = view.Bubble3D;
			AddBubbleSeries(new int[] { 2, 1, 2 }, bubble3D);
		}
		bool CalculateBubble3D(ChartViewWithVaryColors view) {
			if(view.Series.Count > 0) {
				BubbleSeries series = view.Series[0] as BubbleSeries;
				if(series != null)
					return series.Bubble3D;
			}
			return false;
		}
		public void Visit(BarChartView view) {
			BarChartView thumbnailView = new BarChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			thumbnailView.BarDirection = view.BarDirection;
			CreateTwoAxes();
			AddBarSeries(new int[] { 10, 6 });
			if(view.Series.Count > 1) {
				AddBarSeries(new int[] { 6, 10 });
				AddBarSeries(new int[] { 5, 5 });
			}
		}
		public void Visit(Bar3DChartView view) {
			SetupView3D(20, 15, 30, view.Grouping != BarChartGrouping.Standard);
			Bar3DChartView thumbnailView = new Bar3DChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			thumbnailView.BarDirection = view.BarDirection;
			thumbnailView.Shape = view.Shape;
			CreateTwoAxes();
			AddBarSeries(new int[] { 10, 6 });
			if(view.Series.Count > 1) {
				AddBarSeries(new int[] { 6, 10 });
				AddBarSeries(new int[] { 5, 5 });
			}
		}
		public void Visit(AreaChartView view) {
			AreaChartView thumbnailView = new AreaChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			CreateTwoAxes();
			AddAreaSeries(new int[] { 10, 10, 6, 8, 10 });
			if(view.Series.Count > 1)
				AddAreaSeries(new int[] { 8, 8, 4, 2, 3 });
		}
		public void Visit(Area3DChartView view) {
			SetupView3D(20, 15, 30, false);
			Area3DChartView thumbnailView = new Area3DChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			CreateTwoAxes();
			AddAreaSeries(new int[] { 10, 10, 6, 8, 10 });
			if(view.Series.Count > 1)
				AddAreaSeries(new int[] { 8, 8, 4, 2, 3 });
		}
		#endregion
		public void SetupChart() {
			targetChart.ShapeProperties.Outline.Fill = DrawingFill.None;
		}
	}
}
