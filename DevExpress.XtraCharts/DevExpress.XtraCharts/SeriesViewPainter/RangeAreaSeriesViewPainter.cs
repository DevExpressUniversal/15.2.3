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
	public class RangeAreaSeriesViewPainter : AreaSeriesViewPainter {
		public static RangeStrip CreateRangeAreaLegendMarkerStrip(IGeometryStripCreator creator, Rectangle bounds, bool pointMarkerVisible) {
			RangeStrip strip = StripsUtils.CreateAreaMarkerStrip(creator, bounds, pointMarkerVisible);
			LineStrip bottomStrip = new LineStrip();
			bottomStrip.Add(new GRealPoint2D(bounds.Left, bounds.Bottom));
			bottomStrip.Add(new GRealPoint2D(bounds.Left + bounds.Width / 3.0, bounds.Bottom));
			bottomStrip.Add(new GRealPoint2D(bounds.Left + bounds.Width / 2.0, bounds.Top + bounds.Height / 2.0));
			bottomStrip.Add(new GRealPoint2D(bounds.Left + 2 * bounds.Width / 3.0, bounds.Bottom));
			bottomStrip.Add(new GRealPoint2D(bounds.Right, bounds.Bottom));
			strip.BottomStrip = bottomStrip;
			return strip;
		}
		RangeAreaSeriesView RangeAreaView { get { return (RangeAreaSeriesView)View; } }
		public RangeAreaSeriesViewPainter(IAreaSeriesView view) : base(view) {
		}
		Color GetMarkerDrawColor(Marker marker, RangeAreaDrawOptions rangeAreaDrawOptions, SelectionState selectionState) {
			Color selfColor = marker.Color.IsEmpty ? rangeAreaDrawOptions.Color : marker.Color;
			return MixSelfColor(selfColor, selectionState);
		}
		void RenderMarker(IRenderer renderer, Marker marker, IPolygon polygon, RangeAreaDrawOptions rangeAreaDrawOptions, SelectionState selectionState) {
			Color markerColor = GetMarkerDrawColor(marker, rangeAreaDrawOptions, selectionState);
			Color borderColor = GetBorderDrawColor(rangeAreaDrawOptions, marker);
			int borderThickness = GetBorderDrawThickness(marker);
			marker.Render(renderer, polygon, markerColor, rangeAreaDrawOptions.ActualColor2, borderColor, borderThickness);
		}
		protected override IPolygon CalculateLegendMarkerPolygon(PointDrawOptionsBase drawOptions, Rectangle bounds) {
			return null;
		}
		protected override RangeStrip CreateLegendMarkerStrip(Rectangle bounds, bool pointMarkerVisible) {
			return CreateRangeAreaLegendMarkerStrip(View, bounds, pointMarkerVisible);
		}
		protected override void RenderBorder(IRenderer renderer, AreaDrawOptions drawOptions, Rectangle mappingBounds, RangeStrip strip, bool isLegendBorder) {
			RangeAreaDrawOptions rangeAreaDrawOptions = drawOptions as RangeAreaDrawOptions;
			if (rangeAreaDrawOptions == null)
				return;
			RenderAreaBorder(renderer, rangeAreaDrawOptions.Border1, mappingBounds, rangeAreaDrawOptions, strip, strip.BottomStrip, isLegendBorder);
			RenderAreaBorder(renderer, rangeAreaDrawOptions.Border2, mappingBounds, rangeAreaDrawOptions, strip, strip.TopStrip, isLegendBorder);
		}
		public override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			RangeAreaSeriesPointLayout rangeAreaLayout = pointLayout as RangeAreaSeriesPointLayout;
			RangeAreaDrawOptions rangeAreaDrawOptions = drawOptions as RangeAreaDrawOptions;
			if (rangeAreaLayout == null || rangeAreaDrawOptions == null)
				return;
			if (RangeAreaView.Marker2 != null && rangeAreaLayout.Polygon != null &&
				!RectangleF.Intersect(mappingBounds, rangeAreaLayout.Polygon.Rect).IsEmpty)
				rangeAreaLayout.Polygon.RenderShadow(renderer, rangeAreaDrawOptions.Shadow, -1);
			if (RangeAreaView.Marker1 != null && rangeAreaLayout.ZeroPolygon != null && !RectangleF.Intersect(mappingBounds, rangeAreaLayout.ZeroPolygon.Rect).IsEmpty)
				rangeAreaLayout.ZeroPolygon.RenderShadow(renderer, rangeAreaDrawOptions.Shadow, -1);
		}
		public override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			RangeAreaSeriesPointLayout rangeAreaLayout = pointLayout as RangeAreaSeriesPointLayout;
			RangeAreaDrawOptions rangeAreaDrawOptions = drawOptions as RangeAreaDrawOptions;
			if (rangeAreaLayout == null || rangeAreaDrawOptions == null)
				return;
			if (RangeAreaView.Marker2 != null && rangeAreaLayout.Polygon != null && !RectangleF.Intersect(mappingBounds, rangeAreaLayout.Polygon.Rect).IsEmpty)
				RenderMarker(renderer, rangeAreaDrawOptions.Marker2, rangeAreaLayout.Polygon, rangeAreaDrawOptions, pointLayout.PointData.SelectionState);
			if (RangeAreaView.Marker1 != null && rangeAreaLayout.ZeroPolygon != null && !RectangleF.Intersect(mappingBounds, rangeAreaLayout.ZeroPolygon.Rect).IsEmpty)
				RenderMarker(renderer, rangeAreaDrawOptions.Marker1, rangeAreaLayout.ZeroPolygon, rangeAreaDrawOptions, pointLayout.PointData.SelectionState);
		}
	}
}
