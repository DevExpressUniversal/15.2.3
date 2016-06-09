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

extern alias Platform;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using Microsoft.Windows.Design.Metadata;
using System.Security;
namespace DevExpress.Charts.Designer.Native {
	public class AddSeriesCommand : ChartCommandBase {
		readonly string seriesNamePrefix = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DefaultSeriesNamePrefix);
		const string paneIndexName = "Pane";
		Type seriesType;
		string caption;
		string imageName;
		double valueOffset = 0;
		double argumentOffset = 0;
		Type previousAddedSeriesType;
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public AddSeriesCommand(WpfChartModel model, Type seriesType)
			: base(model) {
			this.seriesType = seriesType;
			this.imageName = GlyphUtils.Series + seriesType.Name;
			ChartDesignerStringIDs captionID = (ChartDesignerStringIDs)Enum.Parse(typeof(ChartDesignerStringIDs), seriesType.Name);
			this.caption = ChartDesignerLocalizer.GetString(captionID);
		}
		IEnumerable<SeriesPoint> CreatePointsForFunnelSeries(Series series) {
			const int seriesPointCount = 5;
			Type currentSeriesType = series.GetType();
			if (previousAddedSeriesType == currentSeriesType)
				this.argumentOffset++;
			previousAddedSeriesType = currentSeriesType;
			List<SeriesPoint> points = new List<SeriesPoint>();
			Random rnd = new Random();
			for (int i = seriesPointCount; i > 0; i--) {
				double valueBase = 15 * i + rnd.Next(0, 10);
				SeriesPoint point = new SeriesPoint(i + this.argumentOffset, valueBase);
				points.Add(point);
			}
			return points;
		}
		IEnumerable<SeriesPoint> CreatePointsForPolarSeries(Series series) {
			const int seriesPointCount = 4;
			Type currentSeriesType = series.GetType();
			if (previousAddedSeriesType == currentSeriesType)
				this.argumentOffset++;
			previousAddedSeriesType = currentSeriesType;
			List<SeriesPoint> points = new List<SeriesPoint>();
			Random rnd = new Random();
			for (int i = seriesPointCount; i > 0; i--) {
				double valueBase = 10 * i + rnd.Next(0, 10);
				SeriesPoint point = new SeriesPoint((i + this.argumentOffset - 1) * 90, valueBase);
				points.Add(point);
			}
			return points;
		}
		IEnumerable<SeriesPoint> CreatePoints(Series series) {
			const int seriesPointCount = 5;
			Type currentSeriesType = series.GetType();
			if (previousAddedSeriesType == currentSeriesType && !currentSeriesType.Name.Contains("Stacked") && !currentSeriesType.Name.Contains("SideBySide"))
				this.argumentOffset++;
			previousAddedSeriesType = currentSeriesType;
			this.valueOffset += 1.5;
			List<SeriesPoint> points = new List<SeriesPoint>();
			Random rnd = new Random();
			for (int i = 1; i <= seriesPointCount; i++) {
				double valueBase = 3 * i - Math.Pow(-1, i) * 2 + this.valueOffset;
				SeriesPoint point = new SeriesPoint(i + this.argumentOffset, valueBase);
				if (series is RangeBarSeries2D)
					RangeBarSeries2D.SetValue2(point, valueBase + rnd.Next(-5, 5));
				if (series is RangeAreaSeries2D)
					RangeAreaSeries2D.SetValue2(point, valueBase + rnd.Next(-5, 5));
				if (series is BubbleSeries2D)
					BubbleSeries2D.SetWeight(point, valueBase + rnd.Next(0, 5));
				if (series is BubbleSeries3D)
					BubbleSeries3D.SetWeight(point, valueBase + rnd.Next(0, 5));
				points.Add(point);
			}
			return points;
		}
		IEnumerable<SeriesPoint> CreatePointsForFinacialSeries(Series series) {
			const int pointCount = 30;
			List<SeriesPoint> points = new List<SeriesPoint>();
			Type currentSeriesType = series.GetType();
			if (previousAddedSeriesType == currentSeriesType && !currentSeriesType.Name.Contains("Stacked") && !currentSeriesType.Name.Contains("SideBySide"))
				this.argumentOffset++;
			previousAddedSeriesType = currentSeriesType;
			Random rnd = new Random();
			double preCloseValue = rnd.Next(50, 150);
			for (int i = 0; i < pointCount; i++) {
				double openValue = preCloseValue;
				double closeValue = openValue + rnd.Next(-20, 20);
				double lowValue = Math.Min(openValue, closeValue) - rnd.Next(0, 10);
				double highValue = Math.Max(openValue, closeValue) + rnd.Next(0, 10);
				preCloseValue = closeValue;
				SeriesPoint sp = new SeriesPoint(DateTime.Today.AddDays(i));
				FinancialSeries2D.SetLowValue(sp, lowValue);
				FinancialSeries2D.SetOpenValue(sp, openValue);
				FinancialSeries2D.SetCloseValue(sp, closeValue);
				FinancialSeries2D.SetHighValue(sp, highValue);
				points.Add(sp);
			}
			return points;
		}
		IEnumerable<SeriesPoint> CreatePointsForSeries(Series series) {
			if (series is FinancialSeries2D)
				return CreatePointsForFinacialSeries(series);
			else if (series is FunnelSeries2D)
				return CreatePointsForFunnelSeries(series);
			else if (series is PolarLineSeries2D || series is PolarPointSeries2D || series is PolarAreaSeries2D || series is PolarLineScatterSeries2D)
				return CreatePointsForPolarSeries(series);
			else
				return CreatePoints(series);
		}
		[SecuritySafeCritical]
		List<IModelItem> CreatePointsForSeries(IEditingContext context, IEnumerable<SeriesPoint> points) {
			List<IModelItem> pointItems = new List<IModelItem>();
			foreach (SeriesPoint point in points) {
				IModelItem pointItem = context.CreateItem(typeof(SeriesPoint));
				pointItem.Properties["Argument"].SetValue(point.Argument);
				pointItem.Properties["Value"].SetValue(point.Value);
				if (seriesType.IsSubclassOf(typeof(RangeBarSeries2D))) {
					DXPropertyIdentifier value2Property = new DXPropertyIdentifier(typeof(RangeBarSeries2D), "Value2");
					pointItem.Properties[value2Property].SetValue(RangeBarSeries2D.GetValue2(point));
				}
				if (seriesType == typeof(RangeAreaSeries2D)) {
					DXPropertyIdentifier value2Property = new DXPropertyIdentifier(typeof(RangeAreaSeries2D), "Value2");
					pointItem.Properties[value2Property].SetValue(RangeAreaSeries2D.GetValue2(point));
				}
				if (seriesType == typeof(BubbleSeries2D)) {
					DXPropertyIdentifier weightProperty = new DXPropertyIdentifier(typeof(BubbleSeries2D), "Weight");
					pointItem.Properties[weightProperty].SetValue(BubbleSeries2D.GetWeight(point));
				}
				if (seriesType == typeof(BubbleSeries3D)) {
					DXPropertyIdentifier weightProperty = new DXPropertyIdentifier(typeof(BubbleSeries3D), "Weight");
					pointItem.Properties[weightProperty].SetValue(BubbleSeries3D.GetWeight(point));
				}
				if (seriesType.IsSubclassOf(typeof(FinancialSeries2D))) {
					DXPropertyIdentifier lowValueProperty = new DXPropertyIdentifier(typeof(FinancialSeries2D), "LowValue");
					DXPropertyIdentifier openValueProperty = new DXPropertyIdentifier(typeof(FinancialSeries2D), "OpenValue");
					DXPropertyIdentifier closeValueProperty = new DXPropertyIdentifier(typeof(FinancialSeries2D), "CloseValue");
					DXPropertyIdentifier highValueProperty = new DXPropertyIdentifier(typeof(FinancialSeries2D), "HighValue");
					pointItem.Properties[lowValueProperty].SetValue(FinancialSeries2D.GetLowValue(point));
					pointItem.Properties[openValueProperty].SetValue(FinancialSeries2D.GetOpenValue(point));
					pointItem.Properties[closeValueProperty].SetValue(FinancialSeries2D.GetCloseValue(point));
					pointItem.Properties[highValueProperty].SetValue(FinancialSeries2D.GetHighValue(point));
				}
				pointItems.Add(pointItem);
			}
			return pointItems;
		}
		protected override bool CanExecute(object parameter) {
			if (PreviewChart.Diagram == null)
				return true;
			Type supportedDiagramType = ChartDesignerPropertiesProvider.GetSupportedDiagramType(this.seriesType);
			if (supportedDiagramType == PreviewChart.Diagram.GetType())
				return true;
			else
				return false;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series series = historyItem.NewValue as Series;
			PreviewChart.Diagram.Series.Remove(series);
			return PreviewChart;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series series = historyItem.NewValue as Series;
			if (series != null)
				PreviewChart.Diagram.Series.Add(series);
			return series;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem seriesModelItem = chartModelItem.Context.CreateItem(seriesType);
			if (seriesType.IsSubclassOf(typeof(FinancialSeries2D)))
				seriesModelItem.Properties["ArgumentScaleType"].SetValue(ScaleType.DateTime);
			seriesModelItem.Properties["DisplayName"].SetValue(((Series)historyItem.NewValue).DisplayName);
			List<IModelItem> pointItems = CreatePointsForSeries(chartModelItem.Context, (IEnumerable<SeriesPoint>)historyItem.ExecuteCommandInfo.AdditionalInfo);
			foreach (IModelItem pointItem in pointItems)
				seriesModelItem.Properties["Points"].Collection.Add(pointItem);
			int paneIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexName];
			if (paneIndex >= 0) {
				IModelItem paneModelItem = chartModelItem.Properties["Diagram"].Value.Properties["Panes"].Collection[paneIndex];
				string paneName =  "pane" + paneIndex;
				paneModelItem.Properties["Name"].SetValue(paneName);
				IModelItem bindingItem = designTimeProvider.CreateBindingItem(seriesModelItem, paneName);
				DXPropertyIdentifier panePropIdentifier = new DXPropertyIdentifier(typeof(XYDiagram2D), "SeriesPane");
				seriesModelItem.Properties[panePropIdentifier].SetValue(bindingItem);
			}
			chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection.Add(seriesModelItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Series series = (Series)Activator.CreateInstance(seriesType);
			if (series is FinancialSeries2D)
				series.ArgumentScaleType = ScaleType.DateTime;
			series.DisplayName = seriesNamePrefix + " " + chartControl.Diagram.Series.Count;
			series.Points.AddRange(CreatePointsForSeries(series));
			if (historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexName] >= 0)
				XYDiagram2D.SetSeriesPane((XYSeries)series, ((XYDiagram2D)(chartControl.Diagram)).Panes[historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexName]]);
			chartControl.Diagram.Series.Add(series);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem result = new CompositeHistoryItem();
			Diagram diagram = PreviewChart.Diagram;
			Type diagramType = ChartDesignerPropertiesProvider.GetSupportedDiagramType(seriesType);
			if (diagram == null) {
				CreateDiagramCommand diagramCommand = new CreateDiagramCommand(ChartModel);
				result.HistoryItems.Add(diagramCommand.RuntimeExecute(diagramType).HistoryItem);
			}
			Series series = (Series)Activator.CreateInstance(seriesType);
			IEnumerable<SeriesPoint> points = CreatePointsForSeries(series);
			series.Points.AddRange(points);
			PreviewChart.Diagram.Series.Add(series);
			series.DisplayName = seriesNamePrefix + " " + PreviewChart.Diagram.Series.Count;
			int paneIndex = -1;
			if (ChartModel.SelectedObject is Pane && series is XYSeries2D && ChartModel.SelectedObject != ((XYDiagram2D)PreviewChart.Diagram).ActualDefaultPane) {
				XYDiagram2D.SetSeriesPane((XYSeries)series, (Pane)ChartModel.SelectedObject);
				paneIndex = ((XYDiagram2D)PreviewChart.Diagram).Panes.IndexOf((Pane)ChartModel.SelectedObject);
			}
			ElementIndexItem elIndItem = new ElementIndexItem(paneIndexName, paneIndex);
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, points, elIndItem);
			result.HistoryItems.Add(new HistoryItem(info, this, PreviewChart.Diagram, null, series));
			return new CommandResult(result, series);
		}
	}
	public class CreateDiagramCommand : ChartCommandBase {
		public override string Caption {
			get { return "Create Diagram"; }
		}
		public override string ImageName {
			get { return null; }
		}
		public CreateDiagramCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			if (PreviewChart.Diagram == null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			Type diagramType = parameter as Type;
			Diagram newDiagram = Activator.CreateInstance(diagramType) as Diagram;
			HistoryItem result = new HistoryItem(new ExecuteCommandInfo(parameter), this, PreviewChart, PreviewChart.Diagram, newDiagram);
			PreviewChart.Diagram = newDiagram;
			ChartModel.RecursivelyUpdateChildren();
			return new CommandResult(result);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			PreviewChart.Diagram = historyItem.OldValue as Diagram;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			PreviewChart.Diagram = historyItem.NewValue as Diagram;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Type diagramType = historyItem.ExecuteCommandInfo.Parameter as Type;
			Diagram newDiagram = Activator.CreateInstance(diagramType) as Diagram;
			chartControl.Diagram = newDiagram;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			Type diagramType = historyItem.ExecuteCommandInfo.Parameter as Type;
			if (diagramType != null) {
				chartModelItem.Properties["Diagram"].SetValue(chartModelItem.Context.CreateItem(diagramType));
			}
		}
	}
	public class RemoveDiagramCommand : ChartCommandBase {
		public override string Caption {
			get { return "Remove Diagram"; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemoveDiagramCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return true;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			HistoryItem result = new HistoryItem(new ExecuteCommandInfo(parameter), this, PreviewChart, PreviewChart.Diagram, null);
			PreviewChart.Diagram = null;
			return new CommandResult(result, PreviewChart);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			PreviewChart.Diagram = historyItem.OldValue as Diagram;
			return PreviewChart.Diagram;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			PreviewChart.Diagram = historyItem.NewValue as Diagram;
			return PreviewChart;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			chartControl.Diagram = null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].SetValue(null);
		}
	}
	public class ChangeChartPalette : ChartCommandBase {
		Palette palette;
		string caption;
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeChartPalette(WpfChartModel chartModel, Palette palette)
			: base(chartModel) {
			this.palette = palette;
			caption = palette.PaletteName;
		}
		protected override bool CanExecute(object parameter) {
			return true;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(info, this, ChartModel.Chart, ChartModel.Chart.Palette, this.palette);
			ChartModel.Chart.Palette = this.palette;
			return new CommandResult(historyItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			chartControl.Palette = palette;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Palette"].SetValue(palette);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			PreviewChart.Palette = (Palette)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			PreviewChart.Palette = (Palette)historyItem.NewValue;
			return null;
		}
	}
}
