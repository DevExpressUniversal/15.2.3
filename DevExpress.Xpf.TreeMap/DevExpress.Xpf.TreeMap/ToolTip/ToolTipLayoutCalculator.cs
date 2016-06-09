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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.TreeMap.Native {
	public static class ToolTipLayoutHelper {
		static Point CalculateLocalScreenPosition(Point controlAnchorPosition, FrameworkElement tagretElement) {
			if (tagretElement.FlowDirection == FlowDirection.RightToLeft)
				controlAnchorPosition = new Point(tagretElement.ActualWidth - controlAnchorPosition.X, controlAnchorPosition.Y);
			Point screenOffset = ScreenHelper.GetScreenPoint(tagretElement);
			if (tagretElement.FlowDirection == FlowDirection.RightToLeft)
				screenOffset = new Point(screenOffset.X - tagretElement.ActualWidth, screenOffset.Y);
			Point screenAnchorPoint = new Point(screenOffset.X + controlAnchorPosition.X, screenOffset.Y + controlAnchorPosition.Y);
			return new Point(screenAnchorPoint.X, screenAnchorPoint.Y);
		}
		public static ToolTipLayout CalculateLayout(Point controlAnchorPosition, FrameworkElement tagretElement) {
			Point transformedPoint = CorrectPointByTransform(controlAnchorPosition, tagretElement);
			Point localScreenPosition = CalculateLocalScreenPosition(transformedPoint, tagretElement);
			Rect anchorPointScreenRect = ScreenHelper.GetScreenRect(localScreenPosition);
			if (anchorPointScreenRect.Width == 0) {
				Point screenOffset = ScreenHelper.GetScreenPoint(tagretElement);
				Point mousePosition = Mouse.GetPosition(tagretElement);
				Point mouseScreenAnchorPoint = new Point(screenOffset.X + mousePosition.X, screenOffset.Y + mousePosition.Y);
				anchorPointScreenRect = ScreenHelper.GetScreenRect(mouseScreenAnchorPoint);
			}
			if (localScreenPosition.X < anchorPointScreenRect.Left)
				localScreenPosition.X = anchorPointScreenRect.Left;
			return new ToolTipLayout(localScreenPosition, anchorPointScreenRect);
		}
		static Point CorrectPointByTransform(Point point, FrameworkElement tagretElement) {
			Point controlOffset = tagretElement.PointToScreen(new Point(0, 0));
			Point controlBoundsOffset = tagretElement.PointToScreen(new Point(tagretElement.ActualWidth, tagretElement.ActualHeight));
			double scaleX = Math.Abs(controlBoundsOffset.X - controlOffset.X) / tagretElement.ActualWidth;
			double scaleY = Math.Abs(controlBoundsOffset.Y - controlOffset.Y) / tagretElement.ActualHeight;
			Point correctedPoint = ScreenHelper.GetScaledPoint(new Point(point.X * scaleX, point.Y * scaleY));
			return correctedPoint;
		}
		public static Size CorrectSizeByTransform(Size size, FrameworkElement tagretElement) {
			Point leftTopPoint = CorrectPointByTransform(new Point(0, 0), tagretElement);
			Point bottomRightPoint = CorrectPointByTransform(new Point(size.Width, size.Height), tagretElement);
			return new Size(bottomRightPoint.X - leftTopPoint.X, bottomRightPoint.Y - leftTopPoint.Y);
		}
	}
	public class ToolTipLayout {
		readonly Point anchorPoint;
		readonly Rect screenBounds;
		public Point AnchorPoint { get { return anchorPoint; } }
		public Rect ScreenBounds { get { return screenBounds; } }
		public ToolTipLayout(Point anchorPoint, Rect screenBounds) {
			this.anchorPoint = anchorPoint;
			this.screenBounds = screenBounds;
		}
	}
	public abstract class ToolTipLayoutStrategy {
		public static ToolTipLayoutStrategy Create(FlowDirection flowDirection) {
			if (flowDirection == FlowDirection.LeftToRight)
				return new LeftToRightLayoutStrategy();
			else
				return new RightToLeftLayoutStrategy();
		}
		double GetHorizontalShadowOffset(double width, HorizontalAlignment hBeakAlignment) {
			return hBeakAlignment == LeftToolTipAlignment ? -width : width;
		}
		double GetVerticalShadowOffset(double height, VerticalAlignment vBeakAlignment) {
			return vBeakAlignment == VerticalAlignment.Top ? -height : height;
		}
		protected abstract HorizontalAlignment LeftToolTipAlignment { get; }
		protected abstract HorizontalAlignment RightToolTipAlignment { get; }
		protected abstract double GetCenterOffsetX(Point anchorPoint, Size size);
		protected abstract double GetLeftOffsetX(Point anchorPoint, Size size);
		protected abstract double GetRightOffsetX(Point anchorPoint, Size size);
		protected abstract bool IsLeftOverflow(double offset, Size size, Rect screenBounds);
		protected abstract bool IsRightOverflow(double offset, Size size, Rect screenBounds);
		protected bool IsTopOverflow(double offset, Size size, Rect screenBounds) {
			return offset < screenBounds.Top;
		}
		protected double GetBottomOffsetY(Point anchorPoint, Size size) {
			return anchorPoint.Y;
		}
		protected double GetTopOffsetY(Point anchorPoint, Size size) {
			return anchorPoint.Y - size.Height;
		}
		public Point CalculateToolTipPosition(Point anchorPoint, Size toolTipSize, Rect screenBounds) {
			double offsetX = GetCenterOffsetX(anchorPoint, toolTipSize);
			if (IsLeftOverflow(offsetX, toolTipSize, screenBounds))
				offsetX = GetLeftOffsetX(anchorPoint, toolTipSize);
			else if (IsRightOverflow(offsetX, toolTipSize, screenBounds))
				offsetX = GetRightOffsetX(anchorPoint, toolTipSize);
			double offsetY = GetTopOffsetY(anchorPoint, toolTipSize);
			if (IsTopOverflow(offsetY, toolTipSize, screenBounds))
				offsetY = GetBottomOffsetY(anchorPoint, toolTipSize);
			return new Point(offsetX, offsetY);
		}
		public HorizontalAlignment CalculateHorizontalBeakPosition(Point anchorPoint, Size toolTipSize, Rect screenBounds) {
			double offsetX = GetCenterOffsetX(anchorPoint, toolTipSize);
			if (IsLeftOverflow(offsetX, toolTipSize, screenBounds))
				return LeftToolTipAlignment;
			if (IsRightOverflow(offsetX, toolTipSize, screenBounds))
				return RightToolTipAlignment;
			return HorizontalAlignment.Center;
		}
		public VerticalAlignment CalculateVerticalBeakPosition(Point anchorPoint, Size toolTipSize, Rect screenBounds) {
			double offsetY = GetTopOffsetY(anchorPoint, toolTipSize);
			if (IsTopOverflow(offsetY, toolTipSize, screenBounds))
				return VerticalAlignment.Top;
			return VerticalAlignment.Bottom;
		}
		public Point CorrectPositionByShadow(Point anchorPoint, FrameworkElement target, VerticalAlignment vBeakAlignment, HorizontalAlignment hBeakAlignment, Thickness shadowPadding) {
			double offsetX = 0.0;
			if (hBeakAlignment == LeftToolTipAlignment)
				offsetX = shadowPadding.Left;
			else if (hBeakAlignment == RightToolTipAlignment)
				offsetX = shadowPadding.Right;
			double offsetY = 0.0;
			if (vBeakAlignment == VerticalAlignment.Top)
				offsetY = shadowPadding.Top;
			else if (vBeakAlignment == VerticalAlignment.Bottom)
				offsetY = shadowPadding.Bottom;
			Size shadowSize = ToolTipLayoutHelper.CorrectSizeByTransform(new Size(offsetX, offsetY), target);
			offsetX = GetHorizontalShadowOffset(shadowSize.Width, hBeakAlignment);
			offsetY = GetVerticalShadowOffset(shadowSize.Height, vBeakAlignment);
			return new Point(anchorPoint.X + offsetX, anchorPoint.Y + offsetY);
		}
	}
	public class LeftToRightLayoutStrategy : ToolTipLayoutStrategy {
		protected override HorizontalAlignment LeftToolTipAlignment { get { return HorizontalAlignment.Left; } }
		protected override HorizontalAlignment RightToolTipAlignment { get { return HorizontalAlignment.Right; } }
		protected override double GetCenterOffsetX(Point anchorPoint, Size size) {
			return anchorPoint.X - 0.5 * size.Width;
		}
		protected override double GetLeftOffsetX(Point anchorPoint, Size size) {
			return anchorPoint.X;
		}
		protected override bool IsLeftOverflow(double offset, Size size, Rect screenBounds) {
			return offset < screenBounds.Left;
		}
		protected override double GetRightOffsetX(Point anchorPoint, Size size) {
			return anchorPoint.X - size.Width;
		}
		protected override bool IsRightOverflow(double offset, Size size, Rect screenBounds) {
			return (offset + size.Width) > screenBounds.Right;
		}
	}
	public class RightToLeftLayoutStrategy : ToolTipLayoutStrategy {
		protected override HorizontalAlignment LeftToolTipAlignment { get { return HorizontalAlignment.Right; } }
		protected override HorizontalAlignment RightToolTipAlignment { get { return HorizontalAlignment.Left; } }
		protected override double GetCenterOffsetX(Point anchorPoint, Size size) {
			return anchorPoint.X + 0.5 * size.Width;
		}
		protected override double GetLeftOffsetX(Point anchorPoint, Size size) {
			return anchorPoint.X + size.Width;
		}
		protected override bool IsLeftOverflow(double offset, Size size, Rect screenBounds) {
			return (offset - size.Width) < screenBounds.Left;
		}
		protected override double GetRightOffsetX(Point anchorPoint, Size size) {
			return anchorPoint.X;
		}
		protected override bool IsRightOverflow(double offset, Size size, Rect screenBounds) {
			return offset > screenBounds.Right;
		}
	}
}
