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
using System.Collections.Generic;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public class ChartViewData : IDisposable {
		const int maxFontSize = 11;
		const int minFontSize = 2;
		const StringAlignment alignment = StringAlignment.Center;
		const StringAlignment lineAlignment = StringAlignment.Center;
		static void Merge(List<ILegendItemData> legendItems, List<ILegendItemData> items) {
			foreach (ILegendItemData item in items) {
				if (!legendItems.Contains(item))
					legendItems.Add(item);
			}
		}
		public static ChartViewData Create(Chart chart, ZPlaneRectangle bounds, bool performRangeCorrection) {
			if (!bounds.AreWidthAndHeightPositive())
				return null;
			ZPlaneRectangle innerBounds = (ZPlaneRectangle)chart.Border.CalcBorderedRect((RectangleF)bounds);
			return innerBounds.AreWidthAndHeightPositive() ? new ChartViewData(chart, bounds, innerBounds, performRangeCorrection) : null;
		}		
		readonly FontFamily fontFamily = FontFamily.GenericSansSerif;
		readonly TextMeasurer textMeasurer = new TextMeasurer();
		readonly Chart chart;
		readonly Diagram diagram;
		readonly Legend legend;
		readonly ZPlaneRectangle bounds;
		readonly ZPlaneRectangle innerBounds;
		readonly ZPlaneRectangle excludeTitlesBounds;
		readonly IList<RefinedSeriesData> seriesDataList;
		readonly DiagramViewData diagramViewData;
		readonly IList<DockableTitleViewData> titlesViewData;
		readonly LegendViewData legendViewData;
		readonly IList<AnnotationViewData> annotationsViewData;
		bool seriesLayoutIsCalculated = false;
		bool isDisposed = false;
		bool DesignMode { get { return chart.Container.DesignMode; } }
		bool HasData { get { return (diagram != null) && (diagram is ISimpleDiagram ? ViewController.HasProcessedNotEmptyPoints : ViewController.HasDataToPresent); } }
		bool IsEmpty { get { return diagramViewData == null || diagramViewData.IsEmpty; } }
		public IList<RefinedSeriesData> SeriesDataList { get { return seriesDataList; } }
		public DiagramViewData DiagramViewData { get { return diagramViewData; } }
		public LegendViewData LegendViewData { get { return legendViewData; } }
		public ViewController ViewController { get { return chart.ViewController; } }
		ChartViewData(Chart chart, ZPlaneRectangle bounds, ZPlaneRectangle innerBounds, bool performRangeCorrection) {
			this.chart = chart;
			this.bounds = bounds;
			this.innerBounds = innerBounds;
			Rectangle excludeTitlesBounds = chart.Padding.DecreaseRectangle((Rectangle)innerBounds);
			titlesViewData = chart.Titles.CalculateViewDataAndBoundsWithoutTitle(textMeasurer, ref excludeTitlesBounds);
			this.excludeTitlesBounds = (ZPlaneRectangle)excludeTitlesBounds;
			legend = chart.Legend;
			diagram = chart.Diagram;
			if (diagram != null) {
				GeometryCalculator geometryCalculator = new GeometryCalculator();
				seriesDataList = new List<RefinedSeriesData>(ViewController.RefinedSeriesForLegend.Count);
				List<ILegendItemData> legendItems = new List<ILegendItemData>();
				foreach (IRefinedSeries refinedSeries in ViewController.RefinedSeriesForLegend) {
					SeriesBase series = (SeriesBase)refinedSeries.Series;
					RefinedSeriesData refinedSeriesData = new RefinedSeriesData(refinedSeries, textMeasurer, geometryCalculator);
					Merge(legendItems, refinedSeriesData.GetLegendItems());
					if (refinedSeries.Series.ShouldBeDrawnOnDiagram && refinedSeries.Points.Count > 0)
						seriesDataList.Add(refinedSeriesData);
				}
				legendItems.AddRange(diagram.GetLegendItems());
				Rectangle diagramBounds;
				if (legend.IsOutside) {
					legendViewData = LegendViewData.Create(legend, legendItems, textMeasurer, excludeTitlesBounds);
					if (legendViewData == null)
						diagramBounds = (legend.ActualVisible && legendItems.Count > 0 && legend.IsMaxPercentagesDefault) ?
							new Rectangle(excludeTitlesBounds.Location, Size.Empty) : excludeTitlesBounds;
					else
						diagramBounds = legendViewData.ModifyBounds(excludeTitlesBounds);
					chart.DataContainer.PivotGridDataSourceOptions.UpdateByDiagramBounds(diagramBounds, seriesDataList);
					diagramViewData = CreateDiagramViewData(textMeasurer, diagramBounds, seriesDataList, performRangeCorrection);
				} else {
					diagramBounds = excludeTitlesBounds;
					chart.DataContainer.PivotGridDataSourceOptions.UpdateByDiagramBounds(diagramBounds, seriesDataList);
					diagramViewData = CreateDiagramViewData(textMeasurer, diagramBounds, seriesDataList, performRangeCorrection);
					if (diagramViewData != null)
						legendViewData = LegendViewData.Create(legend, legendItems, textMeasurer, diagramViewData.LegendMappingBounds);
				}
				diagram.LastBounds = diagramBounds;
			}
			annotationsViewData = chart.AnnotationRepository.CreateViewData(diagramViewData, innerBounds, textMeasurer);
		}		
		DiagramViewData CreateDiagramViewData(TextMeasurer textMeasurer, Rectangle diagramBounds, IList<RefinedSeriesData> refinedSeriesDataList, bool performRangeCorrection) {
			return diagram.CalculateViewData(textMeasurer, diagramBounds, refinedSeriesDataList, performRangeCorrection);			
		}
		RectangleFillStyle GetActualFillStyle() {
			RectangleFillStyle fillStyle = (RectangleFillStyle)chart.FillStyle.Clone();
			if (diagram is Diagram3D) {
				RectangleGradientFillOptions fillOptions = fillStyle.Options as RectangleGradientFillOptions;
				if (fillOptions != null)
					fillOptions.InvertGradientMode();
			}
			return fillStyle;
		}
		void RenderSmallChartText(IRenderer renderer) {
			if (String.IsNullOrEmpty(chart.SmallChartText.Text) || !HasData || !IsEmpty)
				return;
			float padding = (float)(innerBounds.Width * 0.05);
			RectangleF actualBounds = new RectangleF((float)innerBounds.Left + padding, (float)innerBounds.Bottom,
				(float)innerBounds.Width - padding * 2, (float)innerBounds.Height);
			float textSize = GraphicUtils.CalcFontEmSize(textMeasurer, actualBounds,
				chart.SmallChartText.Text, chart.SmallChartText.Font.FontFamily, alignment, lineAlignment);
			if (textSize == 0)
				return;
			if (textSize > chart.SmallChartText.Font.Size)
				textSize = chart.SmallChartText.Font.Size;
			NativeFont font = textSize == chart.SmallChartText.Font.Size ? new NativeFont(chart.SmallChartText.Font) :
				new NativeFontDisposable(new Font(chart.SmallChartText.Font.FontFamily, textSize, chart.SmallChartText.Font.Style));
			renderer.DrawBoundedText(chart.SmallChartText.Text, font,
				chart.SmallChartText.ActualTextColor, chart.SmallChartText, actualBounds, alignment, lineAlignment);
		}
		void RenderDesignModeNoDataPresentation(IRenderer renderer) {
			string message = String.Empty;
			if (ViewController.ActiveSeriesCount == 0 && !chart.ShouldUseSeriesTemplate)
				message = ChartLocalizer.GetString(ChartStringId.MsgEmptyChart);
			else if (diagramViewData == null && diagram != null)
				message = diagram.GetInvisibleDiagramMessage();
			if (String.IsNullOrEmpty(message))
				return;
			RectangleF actualBounds = (RectangleF)excludeTitlesBounds;
			float textSize = GraphicUtils.CalcFontEmSize(textMeasurer, actualBounds, message, fontFamily, alignment, lineAlignment);
			if (textSize < minFontSize)
				return;
			if (textSize > maxFontSize)
				textSize = maxFontSize;
			renderer.DrawBoundedText(message, new NativeFontDisposable(new Font(fontFamily, textSize)),
				GraphicUtils.XorColor(chart.ActualBackColor), new AntialiasingSupport(true, chart.EmptyChartText), actualBounds, alignment, lineAlignment);
		}
		void RenderNoDataPresentation(IRenderer renderer) {
			EmptyChartText emptyText = chart.EmptyChartText;
			if (String.IsNullOrEmpty(emptyText.Text))
				return;
			renderer.DrawBoundedText(emptyText.Text, new NativeFont(emptyText.Font), emptyText.ActualTextColor, emptyText, (RectangleF)excludeTitlesBounds, alignment, lineAlignment);
		}
		void RenderTitles(IRenderer renderer) {
			if ((diagramViewData != null) || DesignMode || !HasData) {
				if (titlesViewData != null)
					foreach (DockableTitleViewData item in titlesViewData)
						item.Render(renderer);
			}
		}
		public void Dispose() {
			if (!isDisposed) {
				if (titlesViewData != null)
					foreach (DockableTitleViewData item in titlesViewData)
						item.Dispose();
				textMeasurer.Dispose();
				isDisposed = true;
			}
		}
		public void CalculateLayout(bool lockDrawingHelper) {
			if (!seriesLayoutIsCalculated) {
				if (diagramViewData != null)
					diagramViewData.CalculateSeriesAndLabelLayout(textMeasurer, lockDrawingHelper ? null : chart.DrawingHelper);
				seriesLayoutIsCalculated = true;
			}
		}
		public GraphicsCommand CreateGraphicsCommand(Rectangle bounds) {
			GraphicsCommand rootCommand = new ContainerGraphicsCommand();
			HitTestController hitTestController = chart.HitTestController;
			if (hitTestController.Enabled)
				rootCommand.AddChildCommand(new HitTestingGraphicsCommand(hitTestController, chart, new HitRegion((Rectangle)innerBounds)));
			GraphicsCommand backgroundCommand = chart.BackImage.CreateGraphicsCommand(innerBounds);
			if (backgroundCommand == null) {
				WholeChartAppearance appearance = ((IChartAppearance)chart.Appearance).WholeChartAppearance;
				BackgroundImage defaultBackImage = appearance.BackImage;
				if (defaultBackImage != null)
					backgroundCommand = defaultBackImage.CreateGraphicsCommand(innerBounds);
				if (backgroundCommand == null) 
					using (RectangleFillStyle fillStyle = GetActualFillStyle()) {
						Color backColor = chart.ActualBackColor;
						backgroundCommand = (backColor.A == 255 && fillStyle.FillMode == FillMode.Solid) ?
							new BackgroundSolidRectangleGraphicsCommand(innerBounds, backColor) :
							fillStyle.CreateGraphicsCommand(innerBounds, backColor, Color.Empty);
					}
			}
			rootCommand.AddChildCommand(backgroundCommand);
			if (diagramViewData != null)
				rootCommand.AddChildCommand(diagramViewData.CreateGraphicsCommand());
			Bitmap contentAbove = new Bitmap(bounds.Width, bounds.Height);
			using (Graphics graphicsAbove = Graphics.FromImage(contentAbove)) {
				GdiPlusRenderer renderer = new GdiPlusRenderer();
				renderer.Reset(graphicsAbove, bounds);
				RenderSmallChartText(renderer);
				RenderTitles(renderer);
				RenderAbove(renderer);
				renderer.Present();
			}
			Rectangle imageBounds = new Rectangle(new Point(), bounds.Size);
			rootCommand.AddChildCommand(new GdiImageGraphicsCommand(imageBounds, contentAbove));
			return rootCommand;
		}
		public void Render(IRenderer renderer) {
			renderer.ProcessHitTestRegion(chart.HitTestController, chart, null, new HitRegion((Rectangle)innerBounds));
			BackgroundImage backImage = chart.BackImage;
			if (backImage == null || backImage.Image == null)
				backImage = ((IChartAppearance)chart.Appearance).WholeChartAppearance.BackImage;
			if (backImage != null && backImage.Image != null)
				backImage.Render(renderer, (Rectangle)innerBounds);
			else {
				using (RectangleFillStyle fillStyle = GetActualFillStyle()) {
					Color backColor = chart.ActualBackColor;
					renderer.FillRectangle((RectangleF)innerBounds, backColor, fillStyle);
				}
			}
			if (diagramViewData != null)
				diagramViewData.Render(renderer);
			RenderSmallChartText(renderer);
			RenderTitles(renderer);
		}
		public void RenderMiddle(IRenderer renderer) {
			if (diagramViewData != null)
				diagramViewData.RenderMiddle(renderer);
		}
		public void RenderAbove(IRenderer renderer) {
			PaneViewData paneViewData = diagramViewData as PaneViewData;
			if (!IsEmpty) {
				diagramViewData.RenderAbove(renderer);
				if (legendViewData != null)
					legendViewData.Render(renderer, diagramViewData.LegendMappingBounds);
			}
			bool hasData = HasData;
			if (!hasData) {
				bool isThereUcheckedInLegendSeries = false;
				if (chart.Legend.UseCheckBoxes)
					foreach (Series ser in chart.Series)
						if (ser.Visible && ser.CheckableInLegend && !ser.CheckedInLegend) {
							isThereUcheckedInLegendSeries = true;
							break;
						}
				if (DesignMode && !isThereUcheckedInLegendSeries)
					RenderDesignModeNoDataPresentation(renderer);
				else
					RenderNoDataPresentation(renderer);
			}
			if ((diagramViewData != null) || DesignMode || !hasData) {
				if (annotationsViewData != null)
					foreach (AnnotationViewData item in annotationsViewData)
						item.Render(renderer);
			}
			RectangularBorder border = chart.Border;
			HitTestState hitState = chart.ContainerAdapter.EnableChartHitTesting ? ((IHitTest)chart).State : new HitTestState();
			BorderHelper.RenderBorder(renderer, border, (Rectangle)bounds, hitState,
				border.ActualThickness, BorderHelper.CalculateBorderColor(border, CommonUtils.GetActualAppearance(chart).WholeChartAppearance.BorderColor, hitState));
		}
	}
}
