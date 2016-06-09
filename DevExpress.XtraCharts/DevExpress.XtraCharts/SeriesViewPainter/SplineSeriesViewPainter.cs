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
	public class SplineSeriesViewPainter : LineSeriesViewPainter {
		public static BezierStrip CreateStripForLegendMarker(Rectangle bounds) {
			BezierStrip strip = new BezierStrip();
			if (bounds.Width < 4 || bounds.Height < 4)
				return strip;
			const int offsetX = 1;
			const int offsetY = 2;
			int rightOffsetY = offsetY + 1;
			GRealPoint2D p1 = new GRealPoint2D(bounds.Left + offsetX, bounds.Top + bounds.Height / 3);
			GRealPoint2D p2 = new GRealPoint2D(bounds.Left + offsetX, bounds.Top + offsetY);
			GRealPoint2D p3 = new GRealPoint2D(bounds.Left + bounds.Width / 3, bounds.Top + offsetY);
			GRealPoint2D p4 = new GRealPoint2D(bounds.Right - bounds.Width / 3, bounds.Bottom - rightOffsetY);
			GRealPoint2D p5 = new GRealPoint2D(bounds.Right - offsetX, bounds.Bottom - rightOffsetY);
			GRealPoint2D p6 = new GRealPoint2D(bounds.Right - offsetX, bounds.Bottom - bounds.Height / 3 - 1);
			strip.AddRange(new GRealPoint2D[] { p1, p2, p3, p4, p5, p6});
			return strip;
		}
		public SplineSeriesViewPainter(SeriesViewBase view) : base(view) { }
		static void RenderBezier(IRenderer renderer, LineWholeSeriesLayout lineLayout, LineStyle lineStyle, Color lineColor) {
			int lineThickness = lineLayout.LineThickness;
			renderer.EnableAntialiasing(true);
			foreach (BezierStrip strip in lineLayout.Strips) {
				renderer.EnableAntialiasing(lineStyle.AntiAlias);
				renderer.DrawBezier(strip, lineColor, lineThickness, lineStyle);
				renderer.RestoreAntialiasing();
			}
			renderer.RestoreAntialiasing();
		}
		protected internal override void RenderLineLegendMarker(IRenderer renderer, Rectangle bounds, LineDrawOptions lineDrawOptions, SelectionState selectionState) {
			BezierStrip strip = CreateStripForLegendMarker(bounds);
			LineStyle lineStyle = lineDrawOptions.LineStyle;
			Shadow shadow = lineDrawOptions.Shadow;
			shadow.BeforeShadowRender(renderer, 1);
			renderer.EnableAntialiasing(lineStyle.AntiAlias);
			renderer.DrawBezier(strip, lineDrawOptions.Shadow.Color, 1, lineStyle);
			renderer.RestoreAntialiasing();
			shadow.AfterShadowRender(renderer);
			renderer.EnableAntialiasing(lineStyle.AntiAlias);
			renderer.DrawBezier(strip, GetLineColor(lineDrawOptions, selectionState), 1, lineStyle);
			renderer.RestoreAntialiasing();
		}
		public override void RenderWholeSeries(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			LineDrawOptions lineDrawOptions = layout.SeriesLayout.SeriesData.DrawOptions as LineDrawOptions;
			LineWholeSeriesLayout lineLayout = layout as LineWholeSeriesLayout;
			if (lineDrawOptions == null || lineLayout == null || lineLayout.Strips.Count == 0)
				return;
			RenderBezier(renderer, lineLayout, lineDrawOptions.LineStyle, GetLineColor(lineDrawOptions, layout.SeriesLayout.SeriesData.SelectionState));
		}
		public override void RenderWholeSeriesShadow(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			LineDrawOptions lineDrawOptions = layout.SeriesLayout.SeriesData.DrawOptions as LineDrawOptions;
			LineWholeSeriesLayout lineLayout = layout as LineWholeSeriesLayout;
			if (lineDrawOptions == null || lineLayout == null || lineLayout.Strips.Count == 0)
				return;
			Shadow shadow = lineDrawOptions.Shadow;
			shadow.BeforeShadowRender(renderer);
			RenderBezier(renderer, lineLayout, lineDrawOptions.LineStyle, lineDrawOptions.Shadow.Color);
			shadow.AfterShadowRender(renderer);
		}
	}
}
