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
namespace DevExpress.XtraCharts.Native {
	public class AxisTickmarksViewData : AxisElementViewDataBase {
		public static AxisTickmarksViewData Create(Axis2D axis, AxisIntervalMapping mapping, int axisOffset, int elementOffset, AxisGridDataEx gridData) {
			return new AxisTickmarksViewData(axis, mapping, axisOffset, elementOffset, AxisElementLocation.Outside, false, gridData);
		}
		public static AxisTickmarksViewData CreateCrossAxis(Axis2D axis, AxisIntervalMapping mapping, int axisOffset, AxisGridDataEx gridData) {
			return new AxisTickmarksViewData(axis, mapping, axisOffset, 1, AxisElementLocation.Inside, true, gridData);
		}
		readonly bool isCrossAxis;
		readonly List<Rectangle> majorRects;
		readonly List<Rectangle> minorRects;
		readonly Rectangle? scrollingCorrectionRect;
		AxisTickmarksViewData(Axis2D axis, AxisIntervalMapping mapping, int axisOffset, int elementOffset, AxisElementLocation location, bool isCrossAxis, AxisGridDataEx gridData) : base(axis, mapping, axisOffset, elementOffset, location) {
			this.isCrossAxis = isCrossAxis;
			Tickmarks tickmarks = axis.Tickmarks;
			majorRects = tickmarks.Visible ? CalculateRects(gridData.Items.VisibleValues, tickmarks.Length, tickmarks.Thickness) : new List<Rectangle>();
			minorRects = tickmarks.MinorVisible ? CalculateRects(gridData.MinorValues, tickmarks.MinorLength, tickmarks.MinorThickness) : new List<Rectangle>();
			if (PaneAxesContainer.CanScrollAxis(axis) && axis.XYDiagram2D.IsScrollingEnabled)
				scrollingCorrectionRect = GraphicUtils.MakeRectangle(GetScreenPoint(Double.NegativeInfinity, 0, 0), 
																	 GetScreenPoint(Double.PositiveInfinity, 0, tickmarks.MaxLength));
		}
		Rectangle GetBounds(List<Rectangle> rects) {
			Rectangle bounds = Rectangle.Empty;
			foreach (Rectangle rect in majorRects) {
				if (bounds.IsEmpty)
					bounds = rect;
				else
					bounds = Rectangle.Union(bounds, rect);
			}
			return bounds;
		}
		List<Rectangle> CalculateRects(List<double> values, int length, int thickness) {
			int firstHalf, secondHalf;
			ThicknessUtils.SplitToHalfs(thickness, HitState, out firstHalf, out secondHalf);
			Axis.Tickmarks.ApplySignToHalfs(ref firstHalf, ref secondHalf);
			int offset = length;
			if (isCrossAxis || HitState.Normal)
				offset--;
			List<Rectangle> rects = new List<Rectangle>();
			foreach (double value in values)
				rects.Add(GraphicUtils.MakeRectangle(GetScreenPoint(value, firstHalf, 0), GetScreenPoint(value, secondHalf, offset)));
			return rects;
		}
		public void Render(IRenderer renderer, IEnumerable<Rectangle> rects) {
			foreach (Rectangle rect in rects)
				renderer.FillRectangle(rect, Axis.ActualColor);
			HitTestController hitTestController = HitTestController;
			if (hitTestController.Enabled)
				foreach (Rectangle rect in rects)
					renderer.ProcessHitTestRegion(hitTestController, Axis, null, new HitRegion(rect));
		}
		public override void CalculateDiagramBoundsCorrection(RectangleCorrection correction) {
			if (scrollingCorrectionRect == null) {
				correction.Update(majorRects);
				correction.Update(minorRects);
			}
			else 
				correction.Update(scrollingCorrectionRect.Value);
		}
		public override void Render(IRenderer renderer) {
			Render(renderer, majorRects);
			Render(renderer, minorRects);
		}
		public Rectangle GetBounds() {
			if (majorRects.Count == 0 && minorRects.Count == 0)
				return Rectangle.Empty;
			Rectangle bounds = GetBounds(majorRects);
			Rectangle minorBounds = GetBounds(minorRects);
			if (bounds.IsEmpty)
				bounds = minorBounds;
			else if (!minorBounds.IsEmpty)
				bounds = Rectangle.Union(bounds, minorBounds);
			return bounds;
		}
	}
}
