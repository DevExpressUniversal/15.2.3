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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class PointSeriesViewPainter {
		readonly SeriesViewBase view;
		bool MarkerVisible {
			get {
				PointSeriesViewBase pointView = view as PointSeriesViewBase;
				if (pointView != null)
					return pointView.ActualMarkerVisible;
				else {
					RadarPointSeriesView radarPointView = view as RadarPointSeriesView;
					if (radarPointView != null)
						return radarPointView.ActualMarkerVisible;
				}
				return false;
			}
		}
		protected SeriesViewBase View { get { return view; } }
		protected SeriesHitTestState HitTestState { get { return view.HitTestState; } }
		protected IChartAppearance ActualAppearance { get { return view.ActualAppearance; } }
		public PointSeriesViewPainter(SeriesViewBase view) {
			this.view = view;
		}
		Color GetMarkerDrawColor(PointDrawOptionsBase drawOptions, SelectionState renderSelectionState) {
			Color selfColor = GetMarkerSelfColor(drawOptions);
			return GraphicUtils.CorrectColorBySelectionState(selfColor, renderSelectionState);
		}
		protected internal Color GetBorderDrawColor(PointDrawOptionsBase drawOptions, MarkerBase marker) {
			CustomBorder border = marker.Border;
			if (border.Color.IsEmpty) {
				Color borderColorFromAppearance = ActualAppearance.MarkerAppearance.BorderColor;
				if (!borderColorFromAppearance.IsEmpty)
					return borderColorFromAppearance;
			}
			Color automaticBorderColor = HitTestColors.MixColors(Color.FromArgb(20, 0, 0, 0), drawOptions.Color);
			return BorderHelper.CalculateBorderColor(border, automaticBorderColor);
		}
		protected static int GetBorderDrawThickness(MarkerBase marker) {
			return BorderHelper.CalculateBorderThickness(marker.Border, 1);
		}
		protected virtual IPolygon CalculateLegendMarkerPolygon(PointDrawOptionsBase drawOptions, Rectangle bounds) {
			return MarkerVisible ? drawOptions.Marker.CalculatePolygon(bounds) : null;
		}
		protected internal virtual Color GetMarkerSelfColor(PointDrawOptionsBase drawOptions) {
			return drawOptions.Color;
		}
		protected Color MixSelfColor(Color selfColor, SelectionState selectionState) {
			return GraphicUtils.CorrectColorBySelectionState(selfColor, selectionState);
		}
		protected void RenderPointLegendMarker(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, SelectionState selectionState) {
			PointDrawOptionsBase pointDrawOptions = seriesPointDrawOptions as PointDrawOptionsBase;
			IPolygon polygon = CalculateLegendMarkerPolygon(pointDrawOptions, bounds);
			if (polygon == null)
				return;
			polygon.RenderShadow(renderer, pointDrawOptions.Shadow, 1);
			RenderPoint(renderer, polygon, pointDrawOptions, selectionState);
		}
		protected void RenderPoint(IRenderer renderer, IPolygon polygon, PointDrawOptionsBase pointDrawOptions, SelectionState renderSelectionState) {
			pointDrawOptions.Marker.Render(renderer, polygon, GetMarkerDrawColor(pointDrawOptions, renderSelectionState), pointDrawOptions.ActualColor2, GetBorderDrawColor(pointDrawOptions, pointDrawOptions.Marker), GetBorderDrawThickness(pointDrawOptions.Marker));
		}
		public virtual void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			PointSeriesPointLayout pointPointLayout = pointLayout as PointSeriesPointLayout;
			PointDrawOptionsBase pointDrawOptions = drawOptions as PointDrawOptionsBase;
			if (pointPointLayout == null || pointDrawOptions == null || pointPointLayout.Polygon == null ||
				RectangleF.Intersect(mappingBounds, pointPointLayout.Polygon.Rect).IsEmpty)
				return;
			RenderPoint(renderer, pointPointLayout.Polygon, pointDrawOptions, pointPointLayout.PointData.SelectionState);
		}
		public virtual void RenderWholeSeries(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
		}
		public virtual void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			PointSeriesPointLayout pointPointLayout = pointLayout as PointSeriesPointLayout;
			PointDrawOptionsBase pointDrawOptions = drawOptions as PointDrawOptionsBase;
			if (pointPointLayout == null || pointDrawOptions == null || pointPointLayout.Polygon == null || RectangleF.Intersect(mappingBounds, pointPointLayout.Polygon.Rect).IsEmpty)
				return;
			pointPointLayout.Polygon.RenderShadow(renderer, pointDrawOptions.Shadow, -1);
		}
		public virtual void RenderWholeSeriesShadow(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
		}
		public virtual void RenderLegendMarker(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			RenderPointLegendMarker(renderer, bounds, seriesPointDrawOptions, selectionState);
		}
	}
}
