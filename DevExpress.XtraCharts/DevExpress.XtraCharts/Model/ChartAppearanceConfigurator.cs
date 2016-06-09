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

using System.Drawing;
using Model = DevExpress.Charts.Model;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public abstract class AppearanceConfiguratorBase {
		void ConfigureGradientFilloptions(FillOptionsBase fillOptions, Model.FillOptions fillOptionsModel) {
			if (fillOptions is RectangleGradientFillOptions) {
				RectangleGradientFillOptions rectangleFillOptions = (RectangleGradientFillOptions)fillOptions;
				rectangleFillOptions.Color2 = ModelConfigaratorHelper.ToColor(fillOptionsModel.Color2);
				rectangleFillOptions.GradientMode = (RectangleGradientMode)fillOptionsModel.GradientMode;
			}
			else if (fillOptions is PolygonGradientFillOptions) {
				PolygonGradientFillOptions polygonFillOptions = (PolygonGradientFillOptions)fillOptions;
				polygonFillOptions.Color2 = ModelConfigaratorHelper.ToColor(fillOptionsModel.Color2);
				switch (fillOptionsModel.GradientMode) {
					case Model.GradientMode.BottomLeftToTopRight:
						polygonFillOptions.GradientMode = PolygonGradientMode.BottomLeftToTopRight;
						break;
					case Model.GradientMode.BottomRightToTopLeft:
						polygonFillOptions.GradientMode = PolygonGradientMode.BottomRightToTopLeft;
						break;
					case Model.GradientMode.BottomToTop:
						polygonFillOptions.GradientMode = PolygonGradientMode.BottomToTop;
						break;
					case Model.GradientMode.FromCenterHorizontal:
						polygonFillOptions.GradientMode = PolygonGradientMode.FromCenter;
						break;
					case Model.GradientMode.FromCenterVertical:
						polygonFillOptions.GradientMode = PolygonGradientMode.FromCenter;
						break;
					case Model.GradientMode.LeftToRight:
						polygonFillOptions.GradientMode = PolygonGradientMode.LeftToRight;
						break;
					case Model.GradientMode.RightToLeft:
						polygonFillOptions.GradientMode = PolygonGradientMode.RightToLeft;
						break;
					case Model.GradientMode.ToCenterHorizontal:
						polygonFillOptions.GradientMode = PolygonGradientMode.ToCenter;
						break;
					case Model.GradientMode.ToCenterVertical:
						polygonFillOptions.GradientMode = PolygonGradientMode.ToCenter;
						break;
					case Model.GradientMode.TopLeftToBottomRight:
						polygonFillOptions.GradientMode = PolygonGradientMode.TopLeftToBottomRight;
						break;
					case Model.GradientMode.TopRightToBottomLeft:
						polygonFillOptions.GradientMode = PolygonGradientMode.TopRightToBottomLeft;
						break;
					case Model.GradientMode.TopToBottom:
						polygonFillOptions.GradientMode = PolygonGradientMode.TopToBottom;
						break;
				}
			}
		}
		protected void ConfigureFillStyle2D(FillStyle2D fillStyle, Model.FillStyle fillStyleModel) {
			switch (fillStyleModel.FillMode) {
				case Model.FillMode.Empty:
					fillStyle.FillMode = FillMode.Empty;
					break;
				case Model.FillMode.Solid:
					fillStyle.FillMode = FillMode.Solid;
					break;
				case Model.FillMode.Gradient:
					fillStyle.FillMode = FillMode.Gradient;
					if (fillStyleModel.Options != null)
						ConfigureGradientFilloptions(fillStyle.Options, fillStyleModel.Options);
					break;
				default:
					fillStyle.FillMode = FillMode.Empty;
					break;
			}
		}
		protected void ConfigureFillStyle3D(FillStyle3D fillStyle, Model.FillStyle fillStyleModel) {
			switch (fillStyleModel.FillMode) {
				case Model.FillMode.Empty:
					fillStyle.FillMode = FillMode3D.Empty;
					break;
				case Model.FillMode.Solid:
					fillStyle.FillMode = FillMode3D.Solid;
					break;
				case Model.FillMode.Gradient:
					fillStyle.FillMode = FillMode3D.Gradient;
					ConfigureGradientFilloptions(fillStyle.Options, fillStyleModel.Options);
					break;
				default:
					fillStyle.FillMode = FillMode3D.Empty;
					break;
			}
		}
		protected void ConfigureShadow(Shadow shadow, Model.Shadow shadowModel) {
			if (shadowModel != null) {
				if (!shadowModel.Color.IsEmpty)
					shadow.Color = ModelConfigaratorHelper.ToColor(shadowModel.Color);
				shadow.Size = shadowModel.Size;
				shadow.Visible = true;
			}
			else
				shadow.Visible = false;
		}
		protected void ConfigureLineStyle(LineStyle lineStyle, Model.LineStyle lineStyleModel) {
			lineStyle.DashStyle = (DashStyle)lineStyleModel.DashStyle;
			lineStyle.Thickness = lineStyleModel.Thickness;
		}
		protected void ConfigureBorder(BorderBase border, Model.Border borderModel) {
			if (!borderModel.Color.IsEmpty)
				border.Color = ModelConfigaratorHelper.ToColor(borderModel.Color);
			if (borderModel.Thickness > 0) {
				border.Visibility = Utils.DefaultBoolean.True;
				border.Thickness = borderModel.Thickness;
			}
			else
				border.Visibility = Utils.DefaultBoolean.False;
		}
		protected void ConfigureDefaultBorder(BorderBase border) {
			border.Color = Color.Empty;
			border.Visibility = Utils.DefaultBoolean.True;
			border.Thickness = 1;
		}
		protected void ConfigureIndents(RectangleIndents indents, Model.RectangleIndents indentsModel) {
			if (indentsModel.All != Model.RectangleIndents.Undefined)
				indents.All = indentsModel.All;
			else {
				indents.Left = indentsModel.Left;
				indents.Top = indentsModel.Top;
				indents.Right = indentsModel.Right;
				indents.Bottom = indentsModel.Bottom;
			}
		}
	}
	public class ChartAppeanranceConfigurator : AppearanceConfiguratorBase {
		DiagrammAppearanceConfiguratorBase diagramConfigurator;
		public DiagrammAppearanceConfiguratorBase DiagramCofigurator { get { return diagramConfigurator; } }
		public ChartAppeanranceConfigurator(Diagram diagram, Model.IModelElementContainer container) {
			diagramConfigurator = DiagrammAppearanceConfiguratorBase.CreateConfigurator(diagram, container);
		}
		void ConfigureLegend(Legend legend, Model.LegendAppearance appearance) {
			legend.BackColor = ModelConfigaratorHelper.ToColor(appearance.BackColor);
			legend.TextColor = ModelConfigaratorHelper.ToColor(appearance.TextColor);
			if (appearance.Border != null)
				ConfigureBorder(legend.Border, appearance.Border);
			else
				ConfigureDefaultBorder(legend.Border);
			if (appearance.FillStyle != null)
				ConfigureFillStyle2D((FillStyle2D)legend.FillStyle, appearance.FillStyle);
			ConfigureShadow(legend.Shadow, appearance.Shadow);
			if (appearance.Margins != null)
				ConfigureIndents(legend.Margins, appearance.Margins);
			if (appearance.Padding != null)
				ConfigureIndents(legend.Padding, appearance.Padding);
		}
		void ConfigureChart(Chart chart, Model.ChartAppearance appearance) {
			chart.BackColor = ModelConfigaratorHelper.ToColor(appearance.BackColor);
			if (appearance.Border != null)
				ConfigureBorder(chart.Border, appearance.Border);
			else
				ConfigureDefaultBorder(chart.Border);
			if (appearance.FillStyle != null)
				ConfigureFillStyle2D((FillStyle2D)chart.FillStyle, appearance.FillStyle);
			if (appearance.Padding != null)
				ConfigureIndents(chart.Padding, appearance.Padding);
			if (appearance.TitleAppearance != null)
				ConfigureChartTitles(chart.Titles, appearance.TitleAppearance);
		}
		void ConfigureChartTitles(ChartTitleCollection titles, Model.ChartTitleAppearance appearance) {
			foreach (ChartTitle title in titles) {
				title.TextColor = ModelConfigaratorHelper.ToColor(appearance.TextColor);
				title.Indent = appearance.Indent;
			}
		}
		public void Configure(Chart chart, Model.ChartAppearanceOptions appearance) {
			if (appearance != null) {
				if (appearance.ChartAppearance != null)
					ConfigureChart(chart, appearance.ChartAppearance);
				if (appearance.LegendAppearance != null)
					ConfigureLegend(chart.Legend, appearance.LegendAppearance);
				if (diagramConfigurator != null) {
					if (appearance.DiagrammAppearance != null)
						diagramConfigurator.Configure(chart.Diagram, appearance.DiagrammAppearance);
					if (appearance.SeriesAppearance != null)
						diagramConfigurator.ConfigureSeries(chart.Series, appearance.SeriesAppearance);
				}
			}
		}
	}
	public abstract class DiagrammAppearanceConfiguratorBase : AppearanceConfiguratorBase {
		public static DiagrammAppearanceConfiguratorBase CreateConfigurator(Diagram diagram, Model.IModelElementContainer container) {
			if (diagram is XYDiagram2D)
				return new CartesianDiagrammAppearanceConfigurator(container);
			if (diagram is XYDiagram3D)
				return new XYDiagram3DAppearanceConfigurator(container);
			if (diagram is RadarDiagram)
				return new RadarDiagramAppearanceConfigurator(container);
			if (diagram is SimpleDiagram)
				return new SimpleDiagramAppearanceConfigurator(container);
			if (diagram is SimpleDiagram3D)
				return new SimpleDiagram3DAppearanceConfigurator(container);
			return null;
		}
		readonly Model.IModelElementContainer container;
		protected Model.IModelElementContainer Container { get { return container; } }
		public DiagrammAppearanceConfiguratorBase(Model.IModelElementContainer container) {
			this.container = container;
		}
		protected void ConfigureSeriesLabel(SeriesLabelBase label, Model.SeriesLabelAppearance appearance) {
			label.BackColor = ModelConfigaratorHelper.ToColor(appearance.BackColor);
			if (appearance.Border != null)
				ConfigureBorder(label.Border, appearance.Border);
			else
				ConfigureDefaultBorder(label.Border);
			if (appearance.FillStyle != null)
				ConfigureFillStyle2D(label.FillStyle, appearance.FillStyle);
			label.LineColor = ModelConfigaratorHelper.ToColor(appearance.LineColor);
			label.LineLength = appearance.LineLength;
			if (appearance.LineStyle != null)
				ConfigureLineStyle(label.LineStyle, appearance.LineStyle);
			label.LineVisibility = DefaultBooleanUtils.ToDefaultBoolean(appearance.LineVisible);
			ConfigureShadow(label.Shadow, appearance.Shadow);
			label.TextColor = ModelConfigaratorHelper.ToColor(appearance.TextColor);
		}
		protected Model.AxisAppearance GetAxisActualAppearance(AxisBase axis, Model.AxisAppearance allAxesAppearance) {
			Model.AxisBase axisModel = Container.FindModelElement(axis) as Model.AxisBase;
			if (axisModel != null && axisModel.Appearance != null)
				return axisModel.Appearance;
			else
				return allAxesAppearance;
		}
		protected abstract void ConfigureView(SeriesViewBase view, Model.SeriesAppearance appearance);
		public abstract void Configure(Diagram diagram, Model.DiagrammAppearance appearance);
		public void ConfigureSeries(SeriesCollection seriesCollection, Model.SeriesAppearance appearance) {
			foreach (Series series in seriesCollection) {
				if (appearance.LabelAppearance != null)
					ConfigureSeriesLabel(series.Label, appearance.LabelAppearance);
				if (appearance.MarkerAppearance != null) {
					MarkerAppearanceConfigurator markerConfigurator = MarkerAppearanceConfigurator.CreateMarkerConfigurator(series.View);
					if (markerConfigurator != null)
						markerConfigurator.Configure(series.View, appearance.MarkerAppearance);
				}
				ConfigureView(series.View, appearance);
			}
		}
		public virtual void ConfigureAxis(AxisBase axis, Model.AxisAppearance appearance) {
			if (appearance != null) {
				axis.GridLines.Color = ModelConfigaratorHelper.ToColor(appearance.GridLinesColor);
				axis.GridLines.MinorColor = ModelConfigaratorHelper.ToColor(appearance.GridLinesMinorColor);
				if (appearance.GridLinesLineStyle != null)
					ConfigureLineStyle(axis.GridLines.LineStyle, appearance.GridLinesLineStyle);
				if (appearance.GridLinesMinorLineStyle != null)
					ConfigureLineStyle(axis.GridLines.MinorLineStyle, appearance.GridLinesMinorLineStyle);
				axis.Interlaced = appearance.Interlaced;
				axis.InterlacedColor = ModelConfigaratorHelper.ToColor(appearance.InterlacedColor);
				axis.Label.TextColor = ModelConfigaratorHelper.ToColor(appearance.LabelTextColor);
			}
		}
	}
	public class CartesianDiagrammAppearanceConfigurator : DiagrammAppearanceConfiguratorBase {
		public CartesianDiagrammAppearanceConfigurator(Model.IModelElementContainer container) : base(container) {
		}
		void ConfigureAxes(XYDiagram2D diagram, Model.AxisAppearance appearance) {
			ConfigureAxis(diagram.ActualAxisX, GetAxisActualAppearance(diagram.ActualAxisX, appearance));
			ConfigureAxis(diagram.ActualAxisY, GetAxisActualAppearance(diagram.ActualAxisY, appearance));
			foreach (Axis2D axis in diagram.ActualSecondaryAxesX)
				ConfigureAxis(axis, GetAxisActualAppearance(axis, appearance));
			foreach (Axis2D axis in diagram.ActualSecondaryAxesY)
				ConfigureAxis(axis, GetAxisActualAppearance(axis, appearance));
		}
		void ConfigureViewBorder(XYDiagramSeriesViewBase view, Model.Border borderModel) {
			if (view is BarSeriesView)
				ConfigureBorder(((BarSeriesView)view).Border, borderModel);
			if (view is AreaSeriesViewBase)
				ConfigureBorder(((AreaSeriesViewBase)view).Border, borderModel);
			if (view is RangeAreaSeriesView) {
				ConfigureBorder(((RangeAreaSeriesView)view).Border1, borderModel);
				ConfigureBorder(((RangeAreaSeriesView)view).Border2, borderModel);
			}
		}
		void ConfigureViewDefaultBorder(XYDiagramSeriesViewBase view) {
			if (view is BarSeriesView)
				ConfigureDefaultBorder(((BarSeriesView)view).Border);
			if (view is AreaSeriesViewBase)
				ConfigureDefaultBorder(((AreaSeriesViewBase)view).Border);
			if (view is RangeAreaSeriesView) {
				ConfigureDefaultBorder(((RangeAreaSeriesView)view).Border1);
				ConfigureDefaultBorder(((RangeAreaSeriesView)view).Border2);
			}
		}
		void ConfigureViewFillStyle(XYDiagramSeriesViewBase view, Model.FillStyle fillStyle) {
			if (view is BarSeriesView)
				ConfigureFillStyle2D(((BarSeriesView)view).FillStyle, fillStyle);
			if (view is AreaSeriesViewBase)
				ConfigureFillStyle2D(((AreaSeriesViewBase)view).FillStyle, fillStyle);
		}
		protected override void ConfigureView(SeriesViewBase view, Model.SeriesAppearance appearance) {
			XYDiagramSeriesViewBase xyView = view as XYDiagramSeriesViewBase;
			if (xyView != null) {
				if (xyView is LineSeriesView && appearance.LineStyle != null)
					ConfigureLineStyle(((LineSeriesView)xyView).LineStyle, appearance.LineStyle);
				if (!appearance.Color.IsEmpty)
					xyView.Color = ModelConfigaratorHelper.ToColor(appearance.Color);
				ConfigureShadow(xyView.Shadow, appearance.Shadow);
				if (appearance.Border != null)
					ConfigureViewBorder(xyView, appearance.Border);
				else
					ConfigureViewDefaultBorder(xyView);
				if (appearance.FillStyle != null)
					ConfigureViewFillStyle(xyView, appearance.FillStyle);
			}
		}
		public override void Configure(Diagram diagram, Model.DiagrammAppearance appearance) {
			XYDiagram2D xyDiagram = (XYDiagram2D)diagram;
			xyDiagram.DefaultPane.BackColor = ModelConfigaratorHelper.ToColor(appearance.BackColor);
			xyDiagram.DefaultPane.BorderColor = ModelConfigaratorHelper.ToColor(appearance.BorderColor);
			xyDiagram.DefaultPane.BorderVisible = appearance.BorderVisible;
			if (appearance.FillStyle != null)
				ConfigureFillStyle2D((FillStyle2D)xyDiagram.DefaultPane.FillStyle, appearance.FillStyle);
			ConfigureShadow(xyDiagram.DefaultPane.Shadow, appearance.Shadow);
			ConfigureAxes(xyDiagram, appearance.AxesAppearance);
			if (appearance.Margins != null)
				ConfigureIndents(xyDiagram.Margins, appearance.Margins);
		}
		public override void ConfigureAxis(AxisBase axis, Model.AxisAppearance appearance) {
			base.ConfigureAxis(axis, appearance);
			Axis2D axis2D = axis as Axis2D;
			if (appearance != null && axis2D != null) {
				axis2D.Color = ModelConfigaratorHelper.ToColor(appearance.Color);
				axis2D.Thickness = appearance.Thickness;
				if (appearance.InterlacedFillStyle != null)
					ConfigureFillStyle2D(axis2D.InterlacedFillStyle, appearance.InterlacedFillStyle);
				if (appearance.TitleAppearance != null)
					axis2D.Title.TextColor = ModelConfigaratorHelper.ToColor(appearance.TitleAppearance.TextColor);
			}
		}
	}
	public class XYDiagram3DAppearanceConfigurator : DiagrammAppearanceConfiguratorBase {
		public XYDiagram3DAppearanceConfigurator(Model.IModelElementContainer container) : base(container) {
		}
		void ConfigureAxes(XYDiagram3D diagram, Model.AxisAppearance appearance) {
			ConfigureAxis(diagram.AxisX, GetAxisActualAppearance(diagram.AxisX, appearance));
			ConfigureAxis(diagram.AxisY, GetAxisActualAppearance(diagram.AxisY, appearance));
		}
		void ConfigureViewFillStyle(XYDiagram3DSeriesViewBase view, Model.FillStyle fillStyle) {
			if (view is Bar3DSeriesView)
				ConfigureFillStyle3D(((Bar3DSeriesView)view).FillStyle, fillStyle);
		}
		protected override void ConfigureView(SeriesViewBase view, Model.SeriesAppearance appearance) {
			XYDiagram3DSeriesViewBase xyView = view as XYDiagram3DSeriesViewBase;
			if (xyView != null) {
				if (!appearance.Color.IsEmpty)
					xyView.Color = ModelConfigaratorHelper.ToColor(appearance.Color);
				if (appearance.FillStyle != null)
					ConfigureViewFillStyle(xyView, appearance.FillStyle);
			}
		}
		public override void Configure(Diagram diagram, Model.DiagrammAppearance appearance) {
			XYDiagram3D xyDiagram = (XYDiagram3D)diagram;
			xyDiagram.BackColor = ModelConfigaratorHelper.ToColor(appearance.BackColor);
			if (appearance.FillStyle != null)
				ConfigureFillStyle3D((FillStyle3D)xyDiagram.FillStyle, appearance.FillStyle);
			ConfigureAxes(xyDiagram, appearance.AxesAppearance);
		}
		public override void ConfigureAxis(AxisBase axis, Model.AxisAppearance appearance) {
			base.ConfigureAxis(axis, appearance);
			Axis3D axis3D = axis as Axis3D;
			if (appearance != null && axis3D != null)
				if (appearance.InterlacedFillStyle != null)
					ConfigureFillStyle3D(axis3D.InterlacedFillStyle, appearance.InterlacedFillStyle);
		}
	}
	public class RadarDiagramAppearanceConfigurator : DiagrammAppearanceConfiguratorBase {
		public RadarDiagramAppearanceConfigurator(Model.IModelElementContainer container) : base(container) {
		}
		void ConfigureAxes(RadarDiagram diagram, Model.AxisAppearance appearance) {
			ConfigureAxis(diagram.AxisX, GetAxisActualAppearance(diagram.AxisX, appearance));
			ConfigureAxis(diagram.AxisY, GetAxisActualAppearance(diagram.AxisY, appearance));
		}
		void ConfigureViewBorder(RadarSeriesViewBase view, Model.Border borderModel) {
			if (view is RadarAreaSeriesView)
				ConfigureBorder(((RadarAreaSeriesView)view).Border, borderModel);
		}
		void ConfigureViewDefaultBorder(RadarSeriesViewBase view) {
			if (view is RadarAreaSeriesView)
				ConfigureDefaultBorder(((RadarAreaSeriesView)view).Border);
		}
		void ConfigureViewFillStyle(RadarSeriesViewBase view, Model.FillStyle fillStyle) {
			if (view is RadarAreaSeriesView)
				ConfigureFillStyle2D(((RadarAreaSeriesView)view).FillStyle, fillStyle);
		}
		protected override void ConfigureView(SeriesViewBase view, Model.SeriesAppearance appearance) {
			RadarSeriesViewBase radarView = view as RadarSeriesViewBase;
			if (radarView != null) {
				if (radarView is RadarLineSeriesView && appearance.LineStyle != null)
					ConfigureLineStyle(((RadarLineSeriesView)radarView).LineStyle, appearance.LineStyle);
				if (!appearance.Color.IsEmpty)
					radarView.Color = ModelConfigaratorHelper.ToColor(appearance.Color);
				ConfigureShadow(radarView.Shadow, appearance.Shadow);
				if (appearance.Border != null)
					ConfigureViewBorder(radarView, appearance.Border);
				else
					ConfigureViewDefaultBorder(radarView);
				if (appearance.FillStyle != null)
					ConfigureViewFillStyle(radarView, appearance.FillStyle);
			}
		}
		public override void Configure(Diagram diagram, Model.DiagrammAppearance appearance) {
			RadarDiagram radarDiagram = (RadarDiagram)diagram;
			radarDiagram.BackColor = ModelConfigaratorHelper.ToColor(appearance.BackColor);
			radarDiagram.BorderColor = ModelConfigaratorHelper.ToColor(appearance.BorderColor);
			radarDiagram.BorderVisible = appearance.BorderVisible;
			if (appearance.FillStyle != null)
				ConfigureFillStyle2D((FillStyle2D)radarDiagram.FillStyle, appearance.FillStyle);
			ConfigureShadow(radarDiagram.Shadow, appearance.Shadow);
			ConfigureAxes(radarDiagram, appearance.AxesAppearance);
			if (appearance.Margins != null)
				ConfigureIndents(radarDiagram.Margins, appearance.Margins);
		}
		public override void ConfigureAxis(AxisBase axis, Model.AxisAppearance appearance) {
			RadarAxis radarAxis = axis as RadarAxis;
			if (appearance != null && radarAxis != null) {
				if (radarAxis is RadarAxisY) {
					((RadarAxisY)radarAxis).Color = ModelConfigaratorHelper.ToColor(appearance.Color);
					((RadarAxisY)radarAxis).Thickness = appearance.Thickness;
				}
				if (appearance.InterlacedFillStyle != null)
					ConfigureFillStyle2D(radarAxis.InterlacedFillStyle, appearance.InterlacedFillStyle);
			}
		}
	}
	public class SimpleDiagramAppearanceConfigurator : DiagrammAppearanceConfiguratorBase {
		public SimpleDiagramAppearanceConfigurator(Model.IModelElementContainer container) : base(container) {
		}
		protected override void ConfigureView(SeriesViewBase view, Model.SeriesAppearance appearance) {
			PieSeriesView pieView = view as PieSeriesView;
			if (pieView != null) {
				if (appearance.Border != null)
					ConfigureBorder(pieView.Border, appearance.Border);
				else
					ConfigureDefaultBorder(pieView.Border);
				if (appearance.FillStyle != null)
					ConfigureFillStyle2D(pieView.FillStyle, appearance.FillStyle);
			}
		}
		public override void Configure(Diagram diagram, Model.DiagrammAppearance appearance) {
			SimpleDiagram simpleDiagram = (SimpleDiagram)diagram;
			if (appearance.Margins != null)
				ConfigureIndents(simpleDiagram.Margins, appearance.Margins);
		}
	}
	public class SimpleDiagram3DAppearanceConfigurator : DiagrammAppearanceConfiguratorBase {
		public SimpleDiagram3DAppearanceConfigurator(Model.IModelElementContainer container) : base(container) {
		}
		protected override void ConfigureView(SeriesViewBase view, Model.SeriesAppearance appearance) {
			Pie3DSeriesView pieView = view as Pie3DSeriesView;
			if (pieView != null) {
				if (appearance.FillStyle != null)
					ConfigureFillStyle3D(pieView.PieFillStyle, appearance.FillStyle);
			}
		}
		public override void Configure(Diagram diagram, Model.DiagrammAppearance appearance) {
		}
	}
	public class MarkerAppearanceConfigurator : AppearanceConfiguratorBase {
		protected void ConfigureMarker(MarkerBase marker, Model.MarkerAppearance appearance) {
			marker.BorderVisible = appearance.BorderVisible;
			marker.BorderColor = ModelConfigaratorHelper.ToColor(appearance.BorderColor);
			if (appearance.FillStyle != null)
				ConfigureFillStyle2D(marker.FillStyle, appearance.FillStyle);
		}
		public static MarkerAppearanceConfigurator CreateMarkerConfigurator(SeriesViewBase view) {
			if (view is PointSeriesView || view is RadarPointSeriesView)
				return new PointMarkerAppearanceConfigurator();
			if (view is BubbleSeriesView)
				return new BubbleMarkerAppearanceConfigurator();
			if (view is LineSeriesView || view is RadarLineSeriesView)
				return new LineMarkerAppearanceConfigurator();
			if (view is AreaSeriesView || view is RadarAreaSeriesView)
				return new AreaMarkerAppearanceConfigurator();
			return null;
		}
		public virtual void Configure(SeriesViewBase view, Model.MarkerAppearance appearance) {
		}
	}
	public class PointMarkerAppearanceConfigurator : MarkerAppearanceConfigurator {
		public override void Configure(SeriesViewBase view, Model.MarkerAppearance appearance) {
			base.Configure(view, appearance);
			SimpleMarker marker = null;
			if (view is PointSeriesView)
				marker = ((PointSeriesView)view).PointMarkerOptions;
			else if (view is RadarPointSeriesView)
				marker = ((RadarPointSeriesView)view).PointMarkerOptions;
			if (marker != null)
				ConfigureMarker(marker, appearance);
		}
	}
	public class BubbleMarkerAppearanceConfigurator : MarkerAppearanceConfigurator {
		public override void Configure(SeriesViewBase view, Model.MarkerAppearance appearance) {
			base.Configure(view, appearance);
			BubbleSeriesView bubbleView = view as BubbleSeriesView;
			if (bubbleView != null)
				ConfigureMarker(bubbleView.BubbleMarkerOptions, appearance);
		}
	}
	public class LineMarkerAppearanceConfigurator : MarkerAppearanceConfigurator {
		public override void Configure(SeriesViewBase view, Model.MarkerAppearance appearance) {
			base.Configure(view, appearance);
			Marker marker = null;
			if (view is LineSeriesView)
				marker = ((LineSeriesView)view).LineMarkerOptions;
			else if (view is RadarLineSeriesView)
				marker = ((RadarLineSeriesView)view).LineMarkerOptions;
			if (marker != null)
				ConfigureMarker(marker, appearance);
		}
	}
	public class AreaMarkerAppearanceConfigurator : MarkerAppearanceConfigurator {
		public override void Configure(SeriesViewBase view, Model.MarkerAppearance appearance) {
			base.Configure(view, appearance);
			Marker marker = null;
			if (view is AreaSeriesView)
				marker = ((AreaSeriesView)view).MarkerOptions;
			else if (view is RadarAreaSeriesView)
				marker = ((RadarAreaSeriesView)view).MarkerOptions;
			if (marker != null)
				ConfigureMarker(marker, appearance);
		}
	}
}
