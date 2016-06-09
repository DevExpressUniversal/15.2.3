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
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public sealed class RefinedSeriesData : List<RefinedPointData>, ICheckableLegendItemData {
		readonly bool isFirstInInteraction;
		readonly bool disposeFont;
		readonly bool disposeMarkerImage;
		readonly bool markerVisible;
		readonly bool textVisible;
		readonly Color textColor;
		readonly DrawOptions drawOptions;
		readonly DrawOptions legendDrawOptions;
		readonly ChartImageSizeMode markerImageSizeMode;
		readonly IRefinedSeries refinedSeries;
		readonly Size markerSize;
		readonly string legendText = string.Empty;
		readonly WholeSeriesViewData wholeViewData;
		Font font;
		Image markerImage;
		Legend Legend {
			get { return Series.View.Chart.Legend; }
		}
		public bool DisposeFont {
			get { return disposeFont; }
		}
		public bool DisposeMarkerImage {
			get { return disposeMarkerImage; }
		}
		public bool MarkerVisible {
			get { return markerVisible; }
		}
		public bool TextVisible {
			get { return textVisible; }
		}
		public Color TextColor {
			get { return textColor; }
		}
		public DrawOptions DrawOptions {
			get { return drawOptions; }
		}
		public DrawOptions LegendDrawOptions {
			get { return legendDrawOptions; }
		}
		public Font Font {
			get { return font; }
		}
		public Image MarkerImage {
			get { return markerImage; }
		}
		public ChartImageSizeMode MarkerImageSizeMode {
			get { return markerImageSizeMode; }
		}
		public Series Series {
			get { return (Series)refinedSeries.Series; }
		}
		public IRefinedSeries RefinedSeries {
			get { return refinedSeries; }
		}
		public Size MarkerSize {
			get { return markerSize; }
		}
		public WholeSeriesViewData WholeViewData {
			get { return wholeViewData; }
		}
		public bool IsFirstInInteraction {
			get { return isFirstInInteraction; }
		}
		public SelectionState SelectionState { get; set; }
		public RefinedSeriesData(IRefinedSeries refinedSeries, TextMeasurer textMeasurer, GeometryCalculator geometryCalculator) {
			this.refinedSeries = refinedSeries;
			this.isFirstInInteraction = refinedSeries.IsFirstInContainer;
			drawOptions = Series.View.CreateSeriesDrawOptions();
			legendDrawOptions = (DrawOptions)drawOptions.Clone();
			if (Series.ShowInLegend && (!Series.View.ActualColorEach || Series.Chart.Legend.UseCheckBoxes))
				legendText = Series.ActualLegendText;
			CustomDrawSeriesEventArgs e = new CustomDrawSeriesEventArgs(drawOptions, Series, legendText, Legend.TextVisible, Legend.ActualTextColor,
				Legend.Font, Legend.MarkerVisible, Legend.MarkerSize, legendDrawOptions, null, ChartImageSizeMode.AutoSize);
			Series.ContainerAdapter.OnCustomDrawSeries(e);
			legendText = e.LegendText;
			textVisible = e.LegendTextVisible;
			textColor = e.LegendTextColor;
			font = e.LegendFont;
			markerVisible = e.LegendMarkerVisible;
			markerSize = e.LegendMarkerSize;
			markerImage = e.LegendMarkerImage;
			markerImageSizeMode = e.LegendMarkerImageSizeMode;
			disposeMarkerImage = e.DisposeLegendMarkerImage;
			disposeFont = e.DisposeLegendFont;
			SetSelectionMode();
			if (Series.View.ActualColorEach)
				DisposeCustomImage(disposeMarkerImage);
			if (Series.ShouldBeDrawnOnDiagram) {
				if (Series.View.ShouldCalculatePointsData || Series.View.ActualColorEach) {
					if (Series.ContainerAdapter != null && (Series.ContainerAdapter.ShouldCustomDrawSeriesPoints))
						CalculatePointsData();
					else
						CalculatePointsDataWithoutEvents();
					foreach (RefinedPointData pointData in this)
						foreach (SeriesLabelViewData labelViewData in pointData.LabelViewData)
							labelViewData.CalculateTextSize(textMeasurer, Series.Label, Series.Label.Font);
				}
				wholeViewData = Series.View.CalculateWholeSeriesViewData(this, geometryCalculator);
			}
		}
		private void SetSelectionMode() {
			SeriesSelectionMode selectionMode = Series.View.Chart.SeriesSelectionMode;
			if (Series.HitState.IsSeriesHot(selectionMode))
				this.SelectionState = SelectionState.HotTracked;
			else if (Series.HitState.IsSeriesSelect(selectionMode))
				this.SelectionState = SelectionState.Selected;
			else
				this.SelectionState = SelectionState.Normal;
		}
		#region ILegendItem implementation
		string ILegendItemData.Text { get { return legendText; } }
		object ILegendItemData.RepresentedObject { get { return Series; } }
		Color ICheckableLegendItemData.MainColor { get { return legendDrawOptions.Color; } }
		void ILegendItemData.RenderMarker(IRenderer renderer, Rectangle bounds) {
			foreach (RefinedPointData pointData in this) {
				if (pointData.MarkerImage != markerImage)
					pointData.DisposeCustomMarkerImage();
				if (pointData.Font != font)
					pointData.DisposeCustomFont();
			}
			Series.RenderLegendItem(renderer, bounds, legendDrawOptions, legendDrawOptions, this, null);  
		}
		bool ICheckableLegendItemData.CheckedInLegend {
			get { return Series.CheckedInLegend; }
			set { Series.CheckedInLegend = value; }
		}
		bool ICheckableLegendItemData.Disabled {
			get { return false; }
		}
		bool ICheckableLegendItemData.UseCheckBox {
			get { return Legend.UseCheckBoxes && Series.CheckableInLegend; }
		}
		#endregion
		void CalculatePointsDataWithoutEvents() {
			Series.HitState.LegendSelectionState = SelectionState.Normal;
			IList<RefinedPoint> points = refinedSeries.Points;
			for (int i = refinedSeries.MinVisiblePointIndex; i <= refinedSeries.MaxVisiblePointIndex; i++) {
				if (i >= 0 && i < points.Count) {
					RefinedPoint refinedPoint = points[i];
					DrawOptions pointDrawOptions = (DrawOptions)drawOptions.Clone();
					pointDrawOptions.InitializeSeriesPointDrawOptions(Series.View, refinedSeries, i);
					DrawOptions pointLegendDrawOptions;
					if (Series.View.ActualColorEach) {
						pointLegendDrawOptions = (DrawOptions)legendDrawOptions.Clone();
						pointLegendDrawOptions.InitializeSeriesPointDrawOptions(Series.View, refinedSeries, i);
					}
					else
						pointLegendDrawOptions = (DrawOptions)legendDrawOptions.Clone();
					SeriesLabelViewData[] labelViewData = Series.Label != null ? Series.Label.CalculateViewData(refinedPoint) : new SeriesLabelViewData[0];
					string pointLegendText = ConstructPointLegendText(refinedPoint);
					SelectionState selectionState = CalculateSelectionState(refinedPoint.SeriesPoint);
					Add(new RefinedPointData(this, refinedPoint, pointDrawOptions,
						pointLegendDrawOptions, markerVisible, markerSize,
						markerImageSizeMode, null, false, textVisible,
						textColor, labelViewData, pointLegendText, font, false, selectionState
						));
				}
			}
		}
		void CalculatePointsData() {
			Series.HitState.LegendSelectionState = SelectionState.Normal;
			IList<RefinedPoint> points = refinedSeries.Points;
			DrawOptions seriesLegendDrawOptions = Series.View.ActualColorEach ? null : (DrawOptions)legendDrawOptions.Clone();
			for (int i = refinedSeries.MinVisiblePointIndex; i <= refinedSeries.MaxVisiblePointIndex; i++) {
				RefinedPoint refinedPoint = points[i];
				DrawOptions pointDrawOptions = (DrawOptions)drawOptions.Clone();
				pointDrawOptions.InitializeSeriesPointDrawOptions(Series.View, refinedSeries, i);
				DrawOptions pointLegendDrawOptions;
				if (Series.View.ActualColorEach) {
					pointLegendDrawOptions = (DrawOptions)legendDrawOptions.Clone();
					pointLegendDrawOptions.InitializeSeriesPointDrawOptions(Series.View, refinedSeries, i);
				}
				else
					pointLegendDrawOptions = seriesLegendDrawOptions;
				string labelText = String.Empty;
				string secondLabelText = String.Empty;
				SeriesLabelViewData[] labelViewData = Series.Label != null ? Series.Label.CalculateViewData(refinedPoint) : new SeriesLabelViewData[0];
				if (labelViewData.Length > 0) {
					labelText = labelViewData[0].Text;
					if (labelViewData.Length > 1)
						secondLabelText = labelViewData[1].Text;
				}
				string pointLegendText = ConstructPointLegendText(refinedPoint);
				SelectionState selectionState = CalculateSelectionState(refinedPoint.SeriesPoint);
				CustomDrawSeriesPointEventArgs e = new CustomDrawSeriesPointEventArgs(pointDrawOptions, Series, DevExpress.XtraCharts.SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint),
					labelText, secondLabelText, pointLegendText, textVisible, textColor, font, markerVisible, markerSize, pointLegendDrawOptions, markerImage,
					markerImageSizeMode, selectionState);
				Series.ContainerAdapter.OnCustomDrawSeriesPoint(e);
				if (labelViewData.Length > 0) {
					labelViewData[0].Text = e.LabelText;
					if (labelViewData.Length > 1)
						labelViewData[1].Text = e.SecondLabelText;
				}
				if (Series.HitState.LegendSelectionState == SelectionState.Normal && e.SelectionState != SelectionState.Normal ||
					Series.HitState.LegendSelectionState == SelectionState.Selected && e.SelectionState == SelectionState.HotTracked)
					Series.HitState.LegendSelectionState = e.SelectionState;
				if (Series.View.Chart.SeriesSelectionMode == SeriesSelectionMode.Series && (
					this.SelectionState == SelectionState.Normal && e.SelectionState != SelectionState.Normal ||
					this.SelectionState == SelectionState.Selected && e.SelectionState == SelectionState.HotTracked))
					this.SelectionState = e.SelectionState;
				Add(new RefinedPointData(this, refinedPoint, pointDrawOptions,
					pointLegendDrawOptions, e.LegendMarkerVisible, e.LegendMarkerSize,
					e.LegendMarkerImageSizeMode, e.LegendMarkerImage, e.DisposeLegendMarkerImage, e.LegendTextVisible,
					e.LegendTextColor, labelViewData, e.LegendText, e.LegendFont, e.DisposeLegendFont, e.SelectionState));
			}
		}
		void DisposeCustomFont(bool shouldDispose) {
			if (shouldDispose && font != null) {
				font.Dispose();
				font = null;
			}
		}
		void DisposeCustomImage(bool shouldDispose) {
			if (shouldDispose && markerImage != null) {
				markerImage.Dispose();
				markerImage = null;
			}
		}
		SelectionState CalculateSelectionState(ISeriesPoint point) {
			SelectionState selectionState = SelectionState.Normal;
			if (Series.ChartContainer != null) {
				if (Series.HitState.IsPointHot(point, Series.View.Chart.SeriesSelectionMode))
					selectionState = SelectionState.HotTracked;
				else if (Series.HitState.IsPointSelect(point, Series.View.Chart.SeriesSelectionMode))
					selectionState = SelectionState.Selected;
			}
			return selectionState;
		}
		List<ILegendItemData> GetSeriesPointsLegendItems() {
			List<ILegendItemData> items = new List<ILegendItemData>();
			bool shouldDisposeFont = disposeFont;
			bool shouldDisposeImage = disposeMarkerImage;
			foreach (RefinedPointData pointData in this)
				if (pointData.IsValidLegendItem) {
					items.Add(pointData);
					if (disposeFont && pointData.Font != null && pointData.Font == font) {
						pointData.DisposeFont = true;
						shouldDisposeFont = false;
					}
					if (disposeMarkerImage && pointData.MarkerImage != null && pointData.MarkerImage == markerImage) {
						pointData.DisposeMarkerImage = true;
						shouldDisposeImage = false;
					}
				}
			DisposeCustomFont(shouldDisposeFont && font != Legend.Font);
			DisposeCustomImage(shouldDisposeImage);
			return items;
		}
		List<ILegendItemData> GetIndicatorLegendItems() {
			List<ILegendItemData> legendItems = new List<ILegendItemData>();
			XYDiagram2DSeriesViewBase view = Series.View as XYDiagram2DSeriesViewBase;
			if (view != null) {
				foreach (Indicator indicator in view.Indicators)
					if (indicator.Visible && indicator.ShowInLegend) {
						IndicatorBehavior behavior = indicator.IndicatorBehavior;
						if (!Legend.UseCheckBoxes && behavior.Visible ||
						(Legend.UseCheckBoxes && indicator.CheckableInLegend) ||
						(Legend.UseCheckBoxes && !indicator.CheckableInLegend && indicator.OwningSeries.ShouldBeDrawnOnDiagram))
							legendItems.Add(behavior);
					}
			}
			return legendItems;
		}
		string ConstructPointLegendText(RefinedPoint refinedPoint) {
			string pointLegendText = String.Empty;
			if (Series.ShowInLegend && Series.View.ActualColorEach) {
				PatternParser patternParser = new PatternParser(Series.ActualLegendTextPattern, Series.View);
				patternParser.SetContext(refinedPoint, Series);
				pointLegendText = patternParser.GetText();
			}
			return pointLegendText;
		}
		public List<ILegendItemData> GetLegendItems() {
			List<ILegendItemData> items = new List<ILegendItemData>();
			Series series = Series;
			ILegendItemProvider colorizerInLegend = series.Colorizer as ILegendItemProvider;
			List<LegendItem> colorizerLegend = new List<LegendItem>();
			if (colorizerInLegend != null && colorizerInLegend.ShowInLegend && series.ShowInLegend)
				colorizerLegend = series.ColorProvider.GetLegendItemsForColorizer(series);
			if (series.ShowInLegend && colorizerInLegend == null || colorizerLegend.Count == 0) {
				if (series.View.ShouldAddSeriesPointsInLegend())
					items.AddRange(GetSeriesPointsLegendItems());
				else if (!String.IsNullOrEmpty(legendText))
					items.Add(this);
			}
			else
				items.AddRange(colorizerLegend);
			items.AddRange(GetIndicatorLegendItems());
			return items;
		}
	}
}
