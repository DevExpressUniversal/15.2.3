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
namespace DevExpress.Charts.Native {
	public enum AnnotationLocation {
		TopRight,
		TopLeft,
		BottomRight,
		BottomLeft
	}
	public interface IAnnotationLayout {
		GRealPoint2D AnchorPoint { get; }
		double CursorOffset { get; }
		bool ShowTail { get; }
		GRealPoint2D InitOffset { get; }
		GRealSize2D Size { get; }
		AnnotationLocation Location { get; set; }
		GRealRect2D Bounds { get; set; }
		GRealPoint2D Offset { get; set; }
	}
	public static class AnnotationLayoutCalculator {
		class CrosshairLabelsComparer : IComparer<IAnnotationLayout> {
			readonly GRealPoint2D separator;
			public CrosshairLabelsComparer(GRealPoint2D separator) {
				this.separator = separator;
			}
			public int Compare(IAnnotationLayout layout1, IAnnotationLayout layout2) {
				GRealPoint2D anchorPoint1 = layout1.AnchorPoint;
				GRealPoint2D anchorPoint2 = layout2.AnchorPoint;
				if (Math.Sign(anchorPoint1.X - separator.X) != Math.Sign(anchorPoint2.X - separator.X))
					return (int)(anchorPoint1.X - anchorPoint2.X);
				return (int)(anchorPoint1.Y - anchorPoint2.Y);
			}
		}
		const int defaultTailOffset = 20;
		static AnnotationLocation[] GetPriorityHorizontalLocations(AnnotationLocation location) {
			switch (location) {
				case AnnotationLocation.TopRight:
					return new AnnotationLocation[] { AnnotationLocation.TopRight, AnnotationLocation.TopLeft, AnnotationLocation.BottomRight, AnnotationLocation.BottomLeft };
				case AnnotationLocation.TopLeft:
					return new AnnotationLocation[] { AnnotationLocation.TopLeft, AnnotationLocation.TopRight, AnnotationLocation.BottomLeft, AnnotationLocation.BottomRight };
				case AnnotationLocation.BottomRight:
					return new AnnotationLocation[] { AnnotationLocation.BottomRight, AnnotationLocation.BottomLeft, AnnotationLocation.TopRight, AnnotationLocation.TopLeft };
				case AnnotationLocation.BottomLeft:
					return new AnnotationLocation[] { AnnotationLocation.BottomLeft, AnnotationLocation.BottomRight, AnnotationLocation.TopLeft, AnnotationLocation.TopRight };
			}
			return null;
		}
		static AnnotationLocation[] GetPriorityVerticalLocations(AnnotationLocation location) {
			switch (location) {
				case AnnotationLocation.TopRight:
					return new AnnotationLocation[] { AnnotationLocation.TopRight, AnnotationLocation.BottomRight, AnnotationLocation.TopLeft, AnnotationLocation.BottomLeft };
				case AnnotationLocation.TopLeft:
					return new AnnotationLocation[] { AnnotationLocation.TopLeft, AnnotationLocation.BottomLeft, AnnotationLocation.TopRight, AnnotationLocation.BottomRight };
				case AnnotationLocation.BottomRight:
					return new AnnotationLocation[] { AnnotationLocation.BottomRight, AnnotationLocation.TopRight, AnnotationLocation.BottomLeft, AnnotationLocation.TopLeft };
				case AnnotationLocation.BottomLeft:
					return new AnnotationLocation[] { AnnotationLocation.BottomLeft, AnnotationLocation.TopLeft, AnnotationLocation.BottomRight, AnnotationLocation.TopRight };
			}
			return null;
		}
		static double CalculateLayoutCriteria(GRealRect2D annotationBounds, List<GRealRect2D> reservedRects, GRealRect2D constraintBounds) {
			double criteria = 0;
			if (!constraintBounds.Contains(annotationBounds)) {
				GRealRect2D constraintIntersection = GRealRect2D.Intersect(constraintBounds, annotationBounds);
				criteria += annotationBounds.Width * annotationBounds.Height - constraintIntersection.Width * constraintIntersection.Height;
			}
			if (reservedRects != null) {
				foreach (GRealRect2D rect in reservedRects) {
					GRealRect2D intersection = GRealRect2D.Intersect(rect, annotationBounds);
					criteria += intersection.Width * intersection.Height;
				}
			}
			return criteria;
		}
		static GRealRect2D CalculateLocationBounds(GRealPoint2D anchorPoint,
			GRealSize2D size, AnnotationLocation location, GRealPoint2D offset, double cursorOffset) {
			double x = 0.0, y = 0.0;
			switch (location) {
				case AnnotationLocation.TopRight:
				case AnnotationLocation.BottomRight:
					x = anchorPoint.X + offset.X;
					break;
				case AnnotationLocation.TopLeft:
				case AnnotationLocation.BottomLeft:
					x = anchorPoint.X - size.Width - offset.X;
					break;
			}
			switch (location) {
				case AnnotationLocation.TopRight:
				case AnnotationLocation.TopLeft:
					y = anchorPoint.Y - size.Height - offset.Y;
					break;
				case AnnotationLocation.BottomRight:
				case AnnotationLocation.BottomLeft:
					y = anchorPoint.Y + offset.Y + cursorOffset;
					break;
			}
			return new GRealRect2D(x, y, size.Width, size.Height);
		}
		static GRealPoint2D CalculateLayoutOffset(GRealPoint2D initOffset, AnnotationLocation location, bool showTail) {
			GRealPoint2D offset = initOffset;
			if (showTail)
				offset.X -= defaultTailOffset;
			return offset;
		}
		static void ApplyLayoutParams(IAnnotationLayout layout, GRealRect2D bounds, AnnotationLocation location, GRealPoint2D offset) {
			layout.Bounds = bounds;
			layout.Location = location;
			layout.Offset = offset;
		}
		static void CalculateAutoLayout(IAnnotationLayout layout,
			AnnotationLocation[] priorityLocations, GRealRect2D constraintBounds, List<GRealRect2D> reservedRects) {
			foreach (AnnotationLocation location in priorityLocations) {
				GRealPoint2D offset = CalculateLayoutOffset(layout.InitOffset, location, layout.ShowTail);
				GRealRect2D bounds = CalculateLocationBounds(layout.AnchorPoint, layout.Size, location, offset, layout.CursorOffset);
				bool isIntersected = false;
				if (!isIntersected && reservedRects != null) {
					foreach (GRealRect2D rect in reservedRects) {
						isIntersected = GRealRect2D.IsIntersected(bounds, rect);
						if (isIntersected)
							break;
					}
				}
				if (!isIntersected && constraintBounds.Contains(bounds)) {
					ApplyLayoutParams(layout, bounds, location, offset);
					return;
				}
				if (layout.Bounds.IsEmpty)
					ApplyLayoutParams(layout, bounds, location, offset);
				else {
					double oldLayoutCriteria = CalculateLayoutCriteria(layout.Bounds, reservedRects, constraintBounds);
					double currentLayoutCriteria = CalculateLayoutCriteria(bounds, reservedRects, constraintBounds);
					if (currentLayoutCriteria - oldLayoutCriteria < 0)
						ApplyLayoutParams(layout, bounds, location, offset);
				}
			}
			CorrectLayoutByConstraint(layout, constraintBounds);
		}
		static void CorrectLayoutByConstraint(IAnnotationLayout layout, GRealRect2D constraintBounds) {
			double left = layout.Bounds.Left - constraintBounds.Left;
			double top = layout.Bounds.Top - constraintBounds.Top;
			double right = constraintBounds.Right - layout.Bounds.Right;
			double bottom = constraintBounds.Bottom - layout.Bounds.Bottom;
			double boundsOffsetX = 0.0, boundsOffsetY = 0.0;
			if (!layout.ShowTail) {
				if (left < 0)
					boundsOffsetX = -left;
				else if (right < 0)
					if (left + right >= 0)
						boundsOffsetX = right;
					else
						boundsOffsetX = -left;
				if (top < 0)
					boundsOffsetY = -top;
				else if (bottom < 0)
					if (top + bottom >= 0)
						boundsOffsetY = bottom;
					else
						boundsOffsetY = -top;
			}
			else {
				if (left < 0 && (layout.Location == AnnotationLocation.TopRight || layout.Location == AnnotationLocation.BottomRight)) {
					boundsOffsetX = Math.Abs(layout.Offset.X);
					layout.Offset = new GRealPoint2D(0, layout.Offset.Y);
				}
				else {
					if (right < 0 && (layout.Location == AnnotationLocation.TopLeft || layout.Location == AnnotationLocation.BottomLeft)) {
						boundsOffsetX = -Math.Abs(layout.Offset.X);
						layout.Offset = new GRealPoint2D(0, layout.Offset.Y);
					}
				}
			}
			GRealRect2D correctedBounds = layout.Bounds;
			correctedBounds.Offset(boundsOffsetX, boundsOffsetY);
			layout.Bounds = correctedBounds;
		}
		public static void CalculateAutoLayout(IAnnotationLayout layout, AnnotationLocation defaultLocation, GRealRect2D constraintBounds) {
			CalculateAutoLayout(layout, GetPriorityHorizontalLocations(defaultLocation), constraintBounds, null);
		}
		public static void CalculateAutoLayout(List<IAnnotationLayout> labels, GRealRect2D constraintBounds) {
			List<GRealPoint2D> labelAnchorPoints = new List<GRealPoint2D>();
			foreach (IAnnotationLayout label in labels)
				labelAnchorPoints.Add(label.AnchorPoint);
			GRealPoint2D separator = GeometricUtils.CalcMean(labelAnchorPoints);
			labels.Sort(new CrosshairLabelsComparer(separator));
			List<GRealRect2D> reservedRects = new List<GRealRect2D>();
			foreach (IAnnotationLayout label in labels) {
				AnnotationLocation defaultLocation = (label.AnchorPoint.X < separator.X) ? AnnotationLocation.TopLeft : AnnotationLocation.TopRight;
				CalculateAutoLayout(label, GetPriorityVerticalLocations(defaultLocation), constraintBounds, reservedRects);
				reservedRects.Add(label.Bounds);
			}
		}
		public static void CalculateAutoLayout(List<IAnnotationLayout> labels) {
			foreach (IAnnotationLayout layout in labels) {
				GRealPoint2D offset = CalculateLayoutOffset(layout.InitOffset, layout.Location, layout.ShowTail);
				GRealRect2D bounds = CalculateLocationBounds(layout.AnchorPoint, layout.Size, layout.Location, offset, layout.CursorOffset);
				ApplyLayoutParams(layout, bounds, layout.Location, offset);
			}
		}
	}
}
