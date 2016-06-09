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
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using Model = DevExpress.Charts.Model;
namespace DevExpress.Xpf.Charts.ModelSupport {
	public static class ModelConfiguratorHelper {
		public static Color ToColor(Model.ColorARGB model) {
			return model != Model.ColorARGB.Empty ? Color.FromArgb(model.A, model.R, model.G, model.B) : Colors.Transparent;
		}
		public static SolidColorBrush CreateSolidBrush(Model.ColorARGB model) {
			if (model == Model.ColorARGB.Empty || model.A == 0)
				return null;
			else
				return new SolidColorBrush(Color.FromArgb(model.A, model.R, model.G, model.B));
		}
	}
	public abstract class AppearanceConfiguratorBase {
		DashStyle CreateDashStyle(Model.DashStyle dashModel) {
			DashStyle dashStyle = new DashStyle();
			dashStyle.Dashes = new DoubleCollection();
			switch (dashModel) {
				case Model.DashStyle.Dash:
					dashStyle.Dashes.Add(2);
					dashStyle.Dashes.Add(2);
					return dashStyle;
				case Model.DashStyle.DashDot:
					dashStyle.Dashes.Add(2);
					dashStyle.Dashes.Add(2);
					dashStyle.Dashes.Add(0);
					dashStyle.Dashes.Add(2);
					return dashStyle;
				case Model.DashStyle.DashDotDot:
					dashStyle.Dashes.Add(2);
					dashStyle.Dashes.Add(2);
					dashStyle.Dashes.Add(0);
					dashStyle.Dashes.Add(2);
					dashStyle.Dashes.Add(0);
					dashStyle.Dashes.Add(2);
					return dashStyle;
				case Model.DashStyle.Dot:
					dashStyle.Dashes.Add(0);
					dashStyle.Dashes.Add(2);
					return dashStyle;
				default:
					return null;
			}
		}
		protected Brush CreateFillStyleBrush(Model.FillStyle fillStyle, Model.ColorARGB color) {
			if (fillStyle == null || fillStyle.FillMode != Model.FillMode.Gradient)
				return ModelConfiguratorHelper.CreateSolidBrush(color);
			if(fillStyle.Options == null)
				return ModelConfiguratorHelper.CreateSolidBrush(color);
			LinearGradientBrush gradientBrush = new LinearGradientBrush();
			GradientStop stop1 = new GradientStop() {
				Color = ModelConfiguratorHelper.ToColor(color),
				Offset = 0.0
			};
			gradientBrush.GradientStops.Add(stop1);
			GradientStop stop2 = new GradientStop() {
				Color = ModelConfiguratorHelper.ToColor(fillStyle.Options.Color2),
				Offset = 1.0
			};
			gradientBrush.GradientStops.Add(stop2);
			GradientStop stop0 = null;
			switch (fillStyle.Options.GradientMode) {
				case Model.GradientMode.BottomLeftToTopRight:
					gradientBrush.StartPoint = new Point(0, 1);
					gradientBrush.EndPoint = new Point(1, 0);
					break;
				case Model.GradientMode.BottomRightToTopLeft:
					gradientBrush.StartPoint = new Point(1, 1);
					gradientBrush.EndPoint = new Point(0, 0);
					break;
				case Model.GradientMode.BottomToTop:
					gradientBrush.StartPoint = new Point(0.5, 1);
					gradientBrush.EndPoint = new Point(0.5, 0);
					break;
				case Model.GradientMode.FromCenterHorizontal:
					gradientBrush.StartPoint = new Point(0, 0);
					gradientBrush.EndPoint = new Point(1, 0);
					stop0 = new GradientStop() {
						Color = ModelConfiguratorHelper.ToColor(fillStyle.Options.Color2),
						Offset = 0.0
					};
					gradientBrush.GradientStops.Insert(0, stop0);
					gradientBrush.GradientStops[1].Offset = 0.5;
					gradientBrush.GradientStops[2].Offset = 1.0;
					break;
				case Model.GradientMode.FromCenterVertical:
					gradientBrush.StartPoint = new Point(0, 0);
					gradientBrush.EndPoint = new Point(0, 1);
					stop0 = new GradientStop() {
						Color = ModelConfiguratorHelper.ToColor(fillStyle.Options.Color2),
						Offset = 0.0
					};
					gradientBrush.GradientStops.Insert(0, stop0);
					gradientBrush.GradientStops[1].Offset = 0.5;
					gradientBrush.GradientStops[2].Offset = 1.0;
					break;
				case Model.GradientMode.ToCenterHorizontal:
					gradientBrush.StartPoint = new Point(0, 0);
					gradientBrush.EndPoint = new Point(1, 0);
					stop0 = new GradientStop() {
						Color = ModelConfiguratorHelper.ToColor(color),
						Offset = 1.0
					};
					gradientBrush.GradientStops.Add(stop0);
					gradientBrush.GradientStops[1].Offset = 0.5;
					break;
				case Model.GradientMode.ToCenterVertical:
					gradientBrush.StartPoint = new Point(0, 0);
					gradientBrush.EndPoint = new Point(0, 1);
					stop0 = new GradientStop() {
						Color = ModelConfiguratorHelper.ToColor(color),
						Offset = 1.0
					};
					gradientBrush.GradientStops.Add(stop0);
					gradientBrush.GradientStops[1].Offset = 0.5;
					break;
				case Model.GradientMode.LeftToRight:
					gradientBrush.StartPoint = new Point(0, 0.5);
					gradientBrush.EndPoint = new Point(1, 0.5);
					break;
				case Model.GradientMode.RightToLeft:
					gradientBrush.StartPoint = new Point(1, 0.5);
					gradientBrush.EndPoint = new Point(0, 0.5);
					break;
				case Model.GradientMode.TopLeftToBottomRight:
					gradientBrush.StartPoint = new Point(0, 0);
					gradientBrush.EndPoint = new Point(1, 1);
					break;
				case Model.GradientMode.TopRightToBottomLeft:
					gradientBrush.StartPoint = new Point(1, 0);
					gradientBrush.EndPoint = new Point(0, 1);
					break;
				case Model.GradientMode.TopToBottom:
					gradientBrush.StartPoint = new Point(0.5, 0);
					gradientBrush.EndPoint = new Point(0.5, 1);
					break;
				default:
					break;
			}
			return gradientBrush;
		}
		protected void ConfigureShadow(UIElement element, Model.Shadow shadowModel) {
			DropShadowEffect shadow = new DropShadowEffect();
			if (!shadowModel.Color.IsEmpty)
				shadow.Color = ModelConfiguratorHelper.ToColor(shadowModel.Color);
			shadow.ShadowDepth = shadowModel.Size;
			element.Effect = shadow;
		}
		protected void ConfigureBorder(Control control, Model.Border borderModel) {
			if (!borderModel.Color.IsEmpty)
				control.BorderBrush = ModelConfiguratorHelper.CreateSolidBrush(borderModel.Color);
			control.BorderThickness = new Thickness(borderModel.Thickness);
		}
		protected void ConfigureSeriesBorder(SeriesBorder border, Model.Border borderModel) {
			if (!borderModel.Color.IsEmpty)
				border.Brush = ModelConfiguratorHelper.CreateSolidBrush(borderModel.Color);
			if (border.LineStyle == null)
				border.LineStyle = new LineStyle();
			border.LineStyle.Thickness = borderModel.Thickness;
		}
		protected void ConfigurePadding(Control control, Model.RectangleIndents indentsModel) {
			if (indentsModel.All != Model.RectangleIndents.Undefined)
				control.Padding = new Thickness(indentsModel.All);
			else
				control.Padding = new Thickness(indentsModel.Left, indentsModel.Top, indentsModel.Right, indentsModel.Bottom);
		}
		protected void ConfigureMargins(Control control, Model.RectangleIndents indentsModel) {
			if (indentsModel.All != Model.RectangleIndents.Undefined)
				control.Margin = new Thickness(indentsModel.All);
			else
				control.Margin = new Thickness(indentsModel.Left, indentsModel.Top, indentsModel.Right, indentsModel.Bottom);
		}
		protected void ConfigureLineStyle(LineStyle lineStyle, Model.LineStyle lineStyleModel) {
			lineStyle.DashStyle = CreateDashStyle(lineStyleModel.DashStyle);
			lineStyle.Thickness = lineStyleModel.Thickness;
		}
	}
	public class ChartAppeanranceConfigurator : AppearanceConfiguratorBase {
		Model.IModelElementContainer container;
		public ChartAppeanranceConfigurator(Model.IModelElementContainer container) {
			this.container = container;
		}
		void ConfigureLegend(Legend legend, Model.LegendAppearance appearance) {
			if (!appearance.BackColor.IsEmpty)
				legend.Background = CreateFillStyleBrush(appearance.FillStyle, appearance.BackColor);
			if (!appearance.TextColor.IsEmpty)
				legend.Foreground = ModelConfiguratorHelper.CreateSolidBrush(appearance.TextColor);
			if (appearance.Border != null)
				ConfigureBorder(legend, appearance.Border);
			if (appearance.Shadow != null)
				ConfigureShadow(legend, appearance.Shadow);
			if (appearance.Margins != null)
				ConfigureMargins(legend, appearance.Margins);
			if (appearance.Padding != null)
				ConfigurePadding(legend, appearance.Padding);
		}
		void ConfigureChart(ChartControl chart, Model.ChartAppearance appearance) {
			if (!appearance.BackColor.IsEmpty)
				chart.Background = CreateFillStyleBrush(appearance.FillStyle, appearance.BackColor);
			if (appearance.Border != null)
				ConfigureBorder(chart, appearance.Border);
			if (appearance.Padding != null)
				ConfigurePadding(chart, appearance.Padding);
			if (appearance.TitleAppearance != null)
				ConfigureChartTitles(chart.Titles, appearance.TitleAppearance);
		}
		void ConfigureChartTitles(TitleCollection titles, Model.ChartTitleAppearance appearance) {
			foreach (Title title in titles)
				if (!appearance.TextColor.IsEmpty)
					title.Foreground = ModelConfiguratorHelper.CreateSolidBrush(appearance.TextColor);
		}
		public void Configure(ChartControl chart, Model.ChartAppearanceOptions appearance) {
			if (appearance != null) {
				if (appearance.ChartAppearance != null)
					ConfigureChart(chart, appearance.ChartAppearance);
				if (appearance.LegendAppearance != null && chart.Legend != null)
					ConfigureLegend(chart.Legend, appearance.LegendAppearance);
				DiagrammAppearanceConfiguratorBase diagramConfigurator = DiagrammAppearanceConfiguratorBase.CreateConfigurator(chart.Diagram, container);
				if (diagramConfigurator != null) {
					if (appearance.DiagrammAppearance != null)
						diagramConfigurator.Configure(chart.Diagram, appearance.DiagrammAppearance);
					if (appearance.SeriesAppearance != null)
						diagramConfigurator.ConfigureSeries(chart.Diagram.Series, appearance.SeriesAppearance);
				}
			}
		}
	}
	public abstract class DiagrammAppearanceConfiguratorBase : AppearanceConfiguratorBase {
		public static DiagrammAppearanceConfiguratorBase CreateConfigurator(Diagram diagram, Model.IModelElementContainer container) {
			if (diagram is XYDiagram2D)
				return new CartesianDiagrammAppearanceConfigurator(container);
			if (diagram is CircularDiagram2D)
				return new CircularDiagramAppearanceConfigurator(container);
			if (diagram is SimpleDiagram2D)
				return new SimpleDiagramAppearanceConfigurator(container);
			if (diagram is XYDiagram3D)
				return new XYDiagram3DAppearanceConfigurator(container);
			if (diagram is SimpleDiagram3D)
				return new SimpleDiagram3DAppearanceConfigurator(container);
			return null;
		}
		readonly Model.IModelElementContainer container;
		protected Model.IModelElementContainer Container { get { return container; } }
		public DiagrammAppearanceConfiguratorBase(Model.IModelElementContainer container) {
			this.container = container;
		}
		protected Model.AxisAppearance GetAxisActualAppearance(AxisBase axis, Model.AxisAppearance allAxesAppearance) {
			Model.AxisBase axisModel = null;
			if (axis != null)
				axisModel = Container.FindModelElement(axis) as Model.AxisBase;
			if (axisModel != null && axisModel.Appearance != null)
				return axisModel.Appearance;
			else
				return allAxesAppearance;
		}
		protected void ConfigureSeriesLabel(SeriesLabel label, Model.SeriesLabelAppearance appearance) {
			if (!appearance.BackColor.IsEmpty)
				label.Background = CreateFillStyleBrush(appearance.FillStyle, appearance.BackColor);
			if (appearance.Border != null)
				ConfigureBorder(label, appearance.Border);
			label.Indent = appearance.LineLength;
			label.ConnectorVisible = appearance.LineVisible;
			if (appearance.Shadow != null)
				ConfigureShadow(label, appearance.Shadow);
			if (!appearance.TextColor.IsEmpty)
				label.Foreground = ModelConfiguratorHelper.CreateSolidBrush(appearance.TextColor);
		}
		protected abstract void ConfigureView(Series view, Model.SeriesAppearance appearance);
		public abstract void Configure(Diagram diagram, Model.DiagrammAppearance appearance);
		public void ConfigureSeries(SeriesCollection seriesCollection, Model.SeriesAppearance appearance) {
			foreach (Series series in seriesCollection) {
				if (appearance.LabelAppearance != null)
					ConfigureSeriesLabel(series.ActualLabel, appearance.LabelAppearance);
				ConfigureView(series, appearance);
			}
		}
		public virtual void ConfigureAxis(AxisBase axis, Model.AxisAppearance appearance) {
			if (appearance != null) {
				if (!appearance.GridLinesColor.IsEmpty)
					axis.GridLinesBrush = ModelConfiguratorHelper.CreateSolidBrush(appearance.GridLinesColor);
				if (!appearance.GridLinesMinorColor.IsEmpty)
					axis.GridLinesMinorBrush = ModelConfiguratorHelper.CreateSolidBrush(appearance.GridLinesMinorColor);
				if (appearance.GridLinesLineStyle != null) {
					axis.GridLinesLineStyle = new LineStyle();
					ConfigureLineStyle(axis.GridLinesLineStyle, appearance.GridLinesLineStyle);
				}
				if (appearance.GridLinesMinorLineStyle != null) {
					axis.GridLinesMinorLineStyle = new LineStyle();
					ConfigureLineStyle(axis.GridLinesMinorLineStyle, appearance.GridLinesMinorLineStyle);
				}
				axis.Interlaced = appearance.Interlaced;
				if (!appearance.InterlacedColor.IsEmpty)
					axis.InterlacedBrush = CreateFillStyleBrush(appearance.InterlacedFillStyle, appearance.InterlacedColor);
				if (!appearance.LabelTextColor.IsEmpty)
					axis.ActualLabel.Foreground = ModelConfiguratorHelper.CreateSolidBrush(appearance.LabelTextColor);
			}
		}
	}
	public class CartesianDiagrammAppearanceConfigurator : DiagrammAppearanceConfiguratorBase {
		public CartesianDiagrammAppearanceConfigurator(Model.IModelElementContainer container) : base(container) {
		}
		void ConfigureAxes(XYDiagram2D diagram, Model.AxisAppearance appearance) {
			ConfigureAxis(diagram.ActualAxisX, GetAxisActualAppearance(diagram.ActualAxisX, appearance));
			ConfigureAxis(diagram.ActualAxisY, GetAxisActualAppearance(diagram.ActualAxisY, appearance));
			foreach (Axis2D axis in diagram.SecondaryAxesX)
				ConfigureAxis(axis, GetAxisActualAppearance(axis, appearance));
			foreach (Axis2D axis in diagram.SecondaryAxesY)
				ConfigureAxis(axis, GetAxisActualAppearance(axis, appearance));
		}
		void ConfigureViewBorder(XYSeries2D view, Model.Border borderModel) {
			if (view is AreaSeries2D)
				if (borderModel == null || borderModel.Thickness == 0)
					((AreaSeries2D)view).Border = null;
				else {
					((AreaSeries2D)view).Border = new SeriesBorder();
					ConfigureSeriesBorder(((AreaSeries2D)view).Border, borderModel);
				}
			if (view is AreaStackedSeries2D)
				if (borderModel == null || borderModel.Thickness == 0)
					((AreaStackedSeries2D)view).Border = null;
				else {
					((AreaStackedSeries2D)view).Border = new SeriesBorder();
					ConfigureSeriesBorder(((AreaStackedSeries2D)view).Border, borderModel);
				}
			if (view is RangeAreaSeries2D)
				if (borderModel == null || borderModel.Thickness == 0) {
					((RangeAreaSeries2D)view).Border1 = null;
					((RangeAreaSeries2D)view).Border2 = null;
				}
				else {
					((RangeAreaSeries2D)view).Border1 = new SeriesBorder();
					((RangeAreaSeries2D)view).Border2 = new SeriesBorder();
					ConfigureSeriesBorder(((RangeAreaSeries2D)view).Border1, borderModel);
					ConfigureSeriesBorder(((RangeAreaSeries2D)view).Border2, borderModel);
				}
		}
		protected override void ConfigureView(Series view, Model.SeriesAppearance appearance) {
			XYSeries2D xySeries = view as XYSeries2D;
			if (xySeries != null) {
				if (xySeries is LineSeries2D && appearance.LineStyle != null) {
					((LineSeries2D)xySeries).LineStyle = new LineStyle();
					ConfigureLineStyle(((LineSeries2D)xySeries).LineStyle, appearance.LineStyle);
				}
				if (!appearance.Color.IsEmpty)
					xySeries.Brush = ModelConfiguratorHelper.CreateSolidBrush(appearance.Color);
				if (appearance.Shadow != null)
					ConfigureShadow(xySeries, appearance.Shadow);
				ConfigureViewBorder(xySeries, appearance.Border);
			}
		}
		public override void Configure(Diagram diagram, Model.DiagrammAppearance appearance) {
			XYDiagram2D xyDiagram = (XYDiagram2D)diagram;
			if (!appearance.BackColor.IsEmpty)
				xyDiagram.ActualDefaultPane.DomainBrush = CreateFillStyleBrush(appearance.FillStyle, appearance.BackColor);
			if (appearance.BorderVisible && !appearance.BorderColor.IsEmpty)
				xyDiagram.ActualDefaultPane.DomainBorderBrush = ModelConfiguratorHelper.CreateSolidBrush(appearance.BorderColor);
			else
				xyDiagram.ActualDefaultPane.DomainBorderBrush = null;
			if (appearance.Shadow != null)
				ConfigureShadow(xyDiagram.ActualDefaultPane, appearance.Shadow);
			ConfigureAxes(xyDiagram, appearance.AxesAppearance);
			if (appearance.Margins != null)
				ConfigureMargins(xyDiagram, appearance.Margins);
		}
		public override void ConfigureAxis(AxisBase axis, Model.AxisAppearance appearance) {
			base.ConfigureAxis(axis, appearance);
			Axis2D axis2D = axis as Axis2D;
			if (appearance != null && axis2D != null) {
				if (!appearance.Color.IsEmpty)
					axis2D.Brush = ModelConfiguratorHelper.CreateSolidBrush(appearance.Color);
				axis2D.Thickness = appearance.Thickness;
				if (appearance.TitleAppearance != null && axis2D.Title != null && !appearance.TitleAppearance.TextColor.IsEmpty)
					axis2D.Title.Foreground = ModelConfiguratorHelper.CreateSolidBrush(appearance.TitleAppearance.TextColor);
			}
		}
	}
	public class CircularDiagramAppearanceConfigurator : DiagrammAppearanceConfiguratorBase {
		public CircularDiagramAppearanceConfigurator(Model.IModelElementContainer container) : base(container) {
		}
		void ConfigureAxes(CircularDiagram2D diagram, Model.AxisAppearance appearance) {
			AxisBase axisX = diagram is RadarDiagram2D ? (AxisBase)((RadarDiagram2D)diagram).ActualAxisX : (AxisBase)((PolarDiagram2D)diagram).ActualAxisX;
			AxisBase axisY = diagram is RadarDiagram2D ? (AxisBase)((RadarDiagram2D)diagram).ActualAxisY : (AxisBase)((PolarDiagram2D)diagram).ActualAxisY;
			ConfigureAxis(axisX, GetAxisActualAppearance(axisX, appearance));
			ConfigureAxis(axisY, GetAxisActualAppearance(axisY, appearance));
		}
		void ConfigureViewBorder(CircularSeries2D view, Model.Border borderModel) {
			if (view is CircularAreaSeries2D)
				if (borderModel == null || borderModel.Thickness == 0)
					((CircularAreaSeries2D)view).Border = null;
				else {
					((CircularAreaSeries2D)view).Border = new SeriesBorder();
					ConfigureSeriesBorder(((CircularAreaSeries2D)view).Border, borderModel);
				}
		}
		protected override void ConfigureView(Series view, Model.SeriesAppearance appearance) {
			CircularSeries2D circualrSeries = view as CircularSeries2D;
			if (circualrSeries != null) {
				if (circualrSeries is CircularLineSeries2D && appearance.LineStyle != null)
					ConfigureLineStyle(((CircularLineSeries2D)circualrSeries).LineStyle, appearance.LineStyle);
				if (!appearance.Color.IsEmpty)
					circualrSeries.Brush = ModelConfiguratorHelper.CreateSolidBrush(appearance.Color);
				if (appearance.Shadow != null)
					ConfigureShadow(circualrSeries, appearance.Shadow);
				if (appearance.Border != null)
					ConfigureViewBorder(circualrSeries, appearance.Border);
			}
		}
		public override void Configure(Diagram diagram, Model.DiagrammAppearance appearance) {
			CircularDiagram2D radarDiagram = (CircularDiagram2D)diagram;
			if (!appearance.BackColor.IsEmpty)
				radarDiagram.DomainBrush = CreateFillStyleBrush(appearance.FillStyle, appearance.BackColor);
			if (appearance.BorderVisible && !appearance.BorderColor.IsEmpty)
				radarDiagram.DomainBorderBrush = ModelConfiguratorHelper.CreateSolidBrush(appearance.BorderColor);
			else
				radarDiagram.DomainBorderBrush = null;
			if (appearance.Shadow != null)
				ConfigureShadow(radarDiagram, appearance.Shadow);
			ConfigureAxes(radarDiagram, appearance.AxesAppearance);
			if (appearance.Margins != null)
				ConfigureMargins(radarDiagram, appearance.Margins);
		}
		public override void ConfigureAxis(AxisBase axis, Model.AxisAppearance appearance) {
			base.ConfigureAxis(axis, appearance);
			if (appearance != null && axis is CircularAxisY2D) {
				if (!appearance.Color.IsEmpty)
					((CircularAxisY2D)axis).Brush = ModelConfiguratorHelper.CreateSolidBrush(appearance.Color);
				((CircularAxisY2D)axis).Thickness = appearance.Thickness;
			}
		}
	}
	public class SimpleDiagramAppearanceConfigurator : DiagrammAppearanceConfiguratorBase {
		public SimpleDiagramAppearanceConfigurator(Model.IModelElementContainer container) : base(container) {
		}
		protected override void ConfigureView(Series view, Model.SeriesAppearance appearance) {
		}
		public override void Configure(Diagram diagram, Model.DiagrammAppearance appearance) {
			SimpleDiagram2D simpleDiagram = (SimpleDiagram2D)diagram;
			if (appearance.Margins != null)
				ConfigureMargins(simpleDiagram, appearance.Margins);
		}
	}
	public class XYDiagram3DAppearanceConfigurator : DiagrammAppearanceConfiguratorBase {
		public XYDiagram3DAppearanceConfigurator(Model.IModelElementContainer container) : base(container) {
		}
		void ConfigureAxes(XYDiagram3D diagram, Model.AxisAppearance appearance) {
			ConfigureAxis(diagram.ActualAxisX, GetAxisActualAppearance(diagram.ActualAxisX, appearance));
			ConfigureAxis(diagram.ActualAxisY, GetAxisActualAppearance(diagram.ActualAxisY, appearance));
		}
		protected override void ConfigureView(Series view, Model.SeriesAppearance appearance) {
			XYSeries3D xySeries = view as XYSeries3D;
			if (xySeries != null)
				if (!appearance.Color.IsEmpty)
					xySeries.Brush = ModelConfiguratorHelper.CreateSolidBrush(appearance.Color);
		}
		public override void Configure(Diagram diagram, Model.DiagrammAppearance appearance) {
			XYDiagram3D xyDiagram = (XYDiagram3D)diagram;
			if (!appearance.BackColor.IsEmpty)
				xyDiagram.DomainBrush = CreateFillStyleBrush(appearance.FillStyle, appearance.BackColor);
			ConfigureAxes(xyDiagram, appearance.AxesAppearance);
			if (appearance.Margins != null)
				ConfigureMargins(xyDiagram, appearance.Margins);
		}
	}
	public class SimpleDiagram3DAppearanceConfigurator : DiagrammAppearanceConfiguratorBase {
		public SimpleDiagram3DAppearanceConfigurator(Model.IModelElementContainer container) : base(container) {
		}
		protected override void ConfigureView(Series view, Model.SeriesAppearance appearance) {
		}
		public override void Configure(Diagram diagram, Model.DiagrammAppearance appearance) {
			SimpleDiagram3D simpleDiagram = (SimpleDiagram3D)diagram;
			if (appearance.Margins != null)
				ConfigureMargins(simpleDiagram, appearance.Margins);
		}
	}
}
