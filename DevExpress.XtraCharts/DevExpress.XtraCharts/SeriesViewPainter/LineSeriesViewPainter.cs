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
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class LineSeriesViewPainter : PointSeriesViewPainter {
		public static void RenderLine(IRenderer renderer, Rectangle mappingBounds, LineWholeSeriesLayout lineLayout, LineStyle lineStyle, Color lineColor, LineCap lineCap, LineSeriesView lineView) {
			List<LineStrip> strips = new List<LineStrip>();
			if (lineStyle.DashStyle == DashStyle.Solid || !LargeScaleHelper.IsValid(lineLayout.Strips))
				foreach (LineStrip lineStrip in lineLayout.Strips)
					strips.AddRange(TruncatedLineCalculator.SplitLineStrip(lineStrip, mappingBounds));
			else
				foreach (LineStrip lineStrip in lineLayout.Strips)
					strips.Add(lineStrip);
			if (strips.Count == 0)
				return;
			bool antiAlias = lineStyle.AntiAlias;
			foreach (LineStrip strip in strips) {
				if (lineView != null)
					antiAlias = lineView.GetActualAntialiasing(strip.Count);
				renderer.EnableAntialiasing(antiAlias);
				renderer.DrawLines(strip, lineColor, lineLayout.LineThickness, lineStyle, lineCap);
				renderer.RestoreAntialiasing();
			}
		}
		public static void RenderLineLegendMarker(IRenderer renderer, Rectangle bounds, LineStyle lineStyle, Color lineColor, LineCap lineCap, Shadow shadow) {
			int lineLevel = bounds.Top + (bounds.Height - 1) / 2;
			DiagramPoint p1 = new DiagramPoint(bounds.Left, lineLevel);
			DiagramPoint p2 = new DiagramPoint(bounds.Right, lineLevel);
			if (shadow != null) {
				shadow.BeforeShadowRender(renderer, 1);
				renderer.DrawLine((Point)p1, (Point)p2, shadow.Color, 1, lineStyle, lineCap);
				shadow.AfterShadowRender(renderer, 1);				
			}
			renderer.DrawLine((Point)p1, (Point)p2, lineColor, 1, lineStyle, lineCap);
		}
		protected virtual LineCap LineCap { get { return LineCap.Round; } }
		public LineSeriesViewPainter(SeriesViewBase view) : base(view) {
		}
		void RenderLine(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout, LineDrawOptions lineDrawOptions, Color lineColor) {
			LineWholeSeriesLayout lineLayout = layout as LineWholeSeriesLayout;
			if (lineLayout != null && lineLayout.Strips.Count > 0)
				RenderLine(renderer, mappingBounds, lineLayout, lineDrawOptions.LineStyle, lineColor, LineCap, View as LineSeriesView);
		}
		protected Color GetLineColor(LineDrawOptions lineDrawOptions, SelectionState selectionState) {
			return MixSelfColor(lineDrawOptions.Color, selectionState);
		}
		protected internal virtual void RenderLineLegendMarker(IRenderer renderer, Rectangle bounds, LineDrawOptions lineDrawOptions, SelectionState selectionState) {
			RenderLineLegendMarker(renderer, bounds, lineDrawOptions.LineStyle, GetLineColor(lineDrawOptions, selectionState), LineCap, lineDrawOptions.Shadow);
		}
		protected internal override Color GetMarkerSelfColor(PointDrawOptionsBase drawOptions) {
			Color markerColor = ((LineDrawOptions)drawOptions).Marker.Color;
			return markerColor != Color.Empty ? markerColor : base.GetMarkerSelfColor(drawOptions);
		}
		public override void RenderWholeSeries(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			LineDrawOptions lineDrawOptions = layout.SeriesLayout.DrawOptions as LineDrawOptions;
			if (lineDrawOptions != null) {
				Color lineColor = GetLineColor(lineDrawOptions, layout.SeriesLayout.SeriesData.SelectionState);
				RenderLine(renderer, mappingBounds, layout, lineDrawOptions, lineColor);
			}
		}
		public override void RenderWholeSeriesShadow(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			LineDrawOptions lineDrawOptions = layout.SeriesLayout.DrawOptions as LineDrawOptions;
			if (lineDrawOptions != null) {
				Shadow shadow = lineDrawOptions.Shadow;
				 if ((shadow != null) && shadow.Visible) {
					shadow.BeforeShadowRender(renderer);
					RenderLine(renderer, mappingBounds, layout, lineDrawOptions, lineDrawOptions.Shadow.Color);
					shadow.AfterShadowRender(renderer);
				}
			}
		}
		public override void RenderLegendMarker(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			PointDrawOptions pointDrawOptions = seriesPointDrawOptions as PointDrawOptions;
			LineDrawOptions lineDrawOptions = seriesDrawOptions as LineDrawOptions;
			if (pointDrawOptions == null || lineDrawOptions == null)
				return;
			RenderLineLegendMarker(renderer, bounds, lineDrawOptions, selectionState);
			Rectangle pointMarkerBounds = GraphicUtils.InflateRect(bounds, -2, -2);
			if (pointMarkerBounds.AreWidthAndHeightPositive())
				RenderPointLegendMarker(renderer, pointMarkerBounds, pointDrawOptions, selectionState);
		}
	}
}
