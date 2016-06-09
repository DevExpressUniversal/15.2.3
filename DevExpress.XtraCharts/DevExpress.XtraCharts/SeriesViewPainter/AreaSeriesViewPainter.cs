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
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class AreaSeriesViewPainter : PointSeriesViewPainter {
		const int maxMarkerInLegendSize = 20;
		readonly IAreaSeriesView view;
		protected new IAreaSeriesView View { get { return view; } }
		protected virtual bool CanOptimizePolygons { get { return true; } }
		public AreaSeriesViewPainter(IAreaSeriesView view) : base((SeriesViewBase)view) {
			this.view = view;
		}
		static bool IsVerticalOrHorizontalLine(GRealPoint2D point1, GRealPoint2D point2) {
			return point1.Y == point2.Y || point1.X == point2.X;
		}
		static void ExtendBorderStrip(LineStrip strip, Rectangle mappingBounds) {
			if (!strip.IsEmpty && mappingBounds.AreWidthAndHeightPositive()) {
				double diagonalLength = MathUtils.CalcLength2D(new DiagramPoint(mappingBounds.Left, mappingBounds.Bottom),
															   new DiagramPoint(mappingBounds.Right, mappingBounds.Top));
				GRealPoint2D firstPoint = strip[0];
				for (int i = 0; i < strip.Count; i++) {
					GRealPoint2D secondPoint = strip[1];
					double length = MathUtils.CalcLength2D(firstPoint, secondPoint);
					if (length != 0) {
						GRealPoint2D firstAdditionalPoint = MathUtils.GetPointInDirection2D(secondPoint,
							new DiagramVector(firstPoint.X - secondPoint.X, firstPoint.Y - secondPoint.Y, 0), diagonalLength + length);
						if (!IsVerticalOrHorizontalLine(firstPoint, firstAdditionalPoint))
							strip.Extend(firstAdditionalPoint, true);
						firstPoint = strip[strip.Count - 1];
						for (i = strip.Count - 2; i >= 0; i--) {
							secondPoint = strip[strip.Count - 2];
							length = MathUtils.CalcLength2D(firstPoint, secondPoint);
							if (length != 0) {
								GRealPoint2D lastAdditionalPoint = MathUtils.GetPointInDirection2D(secondPoint,
									new DiagramVector(firstPoint.X - secondPoint.X, firstPoint.Y - secondPoint.Y, 0), diagonalLength + length);
								if (!IsVerticalOrHorizontalLine(firstPoint, lastAdditionalPoint))
									strip.Extend(lastAdditionalPoint, false);
								break;
							}
						}
						break;
					}
				}
			}
		}
		Color GetActualBorderColor(CustomBorder border, AreaDrawOptions drawOptions) {
			if (border.Color == Color.Empty && view.Appearance.BorderColor != Color.Empty)
				return view.Appearance.BorderColor;
			Color automaticBorderColor = HitTestColors.MixColors(Color.FromArgb(100, 0, 0, 0), drawOptions.Color);
			automaticBorderColor = Color.FromArgb(90, automaticBorderColor);
			return BorderHelper.CalculateBorderColor(border, automaticBorderColor);
		}
		void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions pointDrawOptions, DrawOptions drawOptions, SelectionState selectionState) {
			AreaDrawOptions areaPointDrawOptions = pointDrawOptions as AreaDrawOptions;
			if (areaPointDrawOptions == null)
				return;
			IPolygon pointPolygon = CalculateLegendMarkerPolygon(areaPointDrawOptions, CalculateBoundsForLegendMarker(bounds));
			RangeStrip strip = CreateLegendMarkerStrip(bounds, pointPolygon != null);
			renderer.EnablePolygonAntialiasing(true);
			StripsUtils.Render(renderer, strip, areaPointDrawOptions.FillStyle.Options, drawOptions.Color, drawOptions.ActualColor2, HitTestState, null, selectionState);
			RenderBorder(renderer, areaPointDrawOptions, Rectangle.Empty, strip, true);
			if (pointPolygon != null)
				RenderPoint(renderer, pointPolygon, areaPointDrawOptions, selectionState);
			renderer.RestorePolygonAntialiasing();
		}
		void RenderShadowForLegendMarker(IRenderer renderer, Rectangle bounds, DrawOptions drawOptions) {
			AreaDrawOptions areaDrawOptions = drawOptions as AreaDrawOptions;
			if (areaDrawOptions == null)
				return;
			IPolygon pointPolygon = CalculateLegendMarkerPolygon(areaDrawOptions, CalculateBoundsForLegendMarker(bounds));
			RangeStrip strip = CreateLegendMarkerStrip(bounds, pointPolygon != null);
			areaDrawOptions.Shadow.Render(renderer, strip, 1);
			areaDrawOptions.Shadow.Render(renderer, strip.TopStrip, 1, 1);
			if (pointPolygon != null)
				pointPolygon.RenderShadow(renderer, areaDrawOptions.Shadow, 1);
		}
		protected void RenderAreaBorder(IRenderer renderer, CustomBorder border, Rectangle mappingBounds, AreaDrawOptions drawOptions, RangeStrip rangeStrip, LineStrip lineStrip, bool isLegendBorder) {
			if (border == null)
				return;
			int borderThickness = isLegendBorder ? 1 : border.Thickness;
			Color drawingColor;
			if (border.ActualVisibility)
				drawingColor = GetActualBorderColor(border, drawOptions);
			else {
				drawingColor = drawOptions.Color;
				borderThickness = 1;
			}
			LineStrip borderStrip = lineStrip.CreateUniqueStrip();
			LargeScaleHelper.Validate(borderStrip);
			if (view.Closed && !borderStrip.IsEmpty)
				borderStrip.Add(new GRealPoint2D(borderStrip[0].X, borderStrip[0].Y));
			StripsUtils.Render(renderer, borderStrip, drawingColor, borderThickness);
		}
		protected virtual void RenderBorder(IRenderer renderer, AreaDrawOptions drawOptions, Rectangle mappingBounds, RangeStrip strip, bool isLegendBorder) {
			RenderAreaBorder(renderer, drawOptions.Border, mappingBounds, drawOptions, strip, strip.TopStrip, isLegendBorder);
		}
		protected virtual Rectangle CalculateBoundsForLegendMarker(Rectangle bounds) {
			Rectangle newBounds = new Rectangle(bounds.Location.X, bounds.Location.Y, bounds.Width, bounds.Height * 3 / 4);
			if (Math.Min(newBounds.Width, newBounds.Height) > maxMarkerInLegendSize) {
				int deltaX = newBounds.Width - maxMarkerInLegendSize > 0 ? (newBounds.Width - maxMarkerInLegendSize) / 2 : 0;
				int deltaY = newBounds.Height - maxMarkerInLegendSize > 0 ? (newBounds.Height - maxMarkerInLegendSize) / 2 : 0;
				newBounds.Inflate(-deltaX, -deltaY);
			}
			return newBounds;
		}
		protected virtual RangeStrip CreateLegendMarkerStrip(Rectangle bounds, bool pointMarkerVisible) {
			return StripsUtils.CreateAreaMarkerStrip(View, bounds, pointMarkerVisible);
		}
		protected internal override Color GetMarkerSelfColor(PointDrawOptionsBase drawOptions) {
			Color markerColor = ((AreaDrawOptions)drawOptions).Marker.Color;
			return markerColor != Color.Empty ? markerColor : base.GetMarkerSelfColor(drawOptions);
		}
		protected override IPolygon CalculateLegendMarkerPolygon(PointDrawOptionsBase drawOptions, Rectangle bounds) {
			return View.MarkerOptions == null ? null : base.CalculateLegendMarkerPolygon(drawOptions, bounds);
		}
		public override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			if (View.MarkerOptions != null)
				base.Render(renderer, mappingBounds, pointLayout, drawOptions);
		}
		public override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			if (View.MarkerOptions != null) 
				base.RenderShadow(renderer, mappingBounds, pointLayout, drawOptions);
		}
		public override void RenderWholeSeries(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			AreaWholeSeriesLayout areaLayout = layout as AreaWholeSeriesLayout;
			if (areaLayout == null || areaLayout.Strips.Count == 0)
				return;
			AreaDrawOptions drawOptions = layout.SeriesLayout.DrawOptions as AreaDrawOptions;
			if (drawOptions == null)
				return;
			PolygonFillStyle actualFillStyle = (PolygonFillStyle)drawOptions.FillStyle.Clone();
			if (actualFillStyle.FillMode == FillMode.Gradient && view.Rotated)
				((PolygonGradientFillOptions)actualFillStyle.Options).RotateGradientMode();
			foreach (RangeStrip strip in areaLayout.Strips) {
				bool antiAlias = View.GetActualAntialiasing(Math.Max(strip.TopStrip.Count, strip.BottomStrip.Count));
				renderer.EnableAntialiasing(antiAlias);
				renderer.EnablePolygonOptimization(false);
				StripsUtils.Render(renderer, strip, actualFillStyle.Options, drawOptions.Color, drawOptions.ActualColor2, HitTestState, (ZPlaneRectangle)mappingBounds, layout.SeriesLayout.SeriesData.SelectionState);
				RenderBorder(renderer, drawOptions, mappingBounds, strip, false);
				renderer.RestorePolygonOptimization();
				renderer.RestoreAntialiasing();
			}
		}
		public override void RenderWholeSeriesShadow(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			AreaWholeSeriesLayout areaLayout = (AreaWholeSeriesLayout)layout;
			if (areaLayout == null || areaLayout.Strips.Count == 0)
				return;
			AreaDrawOptions areaDrawOptions = areaLayout.SeriesLayout.DrawOptions as AreaDrawOptions;
			if (areaDrawOptions == null)
				return;
			bool shouldDrawBorder = View.Border != null;
			foreach (RangeStrip strip in areaLayout.Strips) {
				areaDrawOptions.Shadow.Render(renderer, strip);
				if (shouldDrawBorder) {
					LineStrip borderStrip = strip.TopStrip.CreateUniqueStrip();
					LargeScaleHelper.Validate(borderStrip);
					areaDrawOptions.Shadow.Render(renderer, borderStrip, 1);
				}
			}
		}
		public override void RenderLegendMarker(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			RenderShadowForLegendMarker(renderer, bounds, seriesDrawOptions);
			RenderLegendMarkerInternal(renderer, bounds, seriesPointDrawOptions, seriesDrawOptions, selectionState);
		}
	}
}
