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

using System.Windows;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class ToolTipLayout : IAnnotationLayout {
		readonly GRealPoint2D anchorPoint;
		readonly double additionalOffset;
		AnnotationLocation location;
		GRealSize2D size;
		GRealRect2D bounds;
		GRealPoint2D offset;
		GRealPoint2D initOffset;
		bool showBeak;
		GRealPoint2D IAnnotationLayout.AnchorPoint { get { return anchorPoint; } }
		GRealRect2D IAnnotationLayout.Bounds { get { return bounds; } set { bounds = value; } }
		AnnotationLocation IAnnotationLayout.Location { get { return location; } set { location = value; } }
		double IAnnotationLayout.CursorOffset { get { return additionalOffset; } }
		GRealPoint2D IAnnotationLayout.InitOffset { get { return initOffset; } }
		GRealPoint2D IAnnotationLayout.Offset { get { return offset; } set { offset = value; } }
		bool IAnnotationLayout.ShowTail { get { return showBeak; } }
		public Point Position { get { return new Point(bounds.Left, bounds.Top); } }
		public GRealSize2D Size { get { return size; } set { size = value; } }
		public ToolTipLayout(Point position, Size size, Point offset) {
			this.location = AnnotationLocation.TopRight;
			this.anchorPoint = new GRealPoint2D(position.X, position.Y);
			this.additionalOffset = 0;
			this.initOffset = new GRealPoint2D(offset.X, offset.Y);
			this.size = new GRealSize2D(size.Width, size.Height);
			this.bounds = new GRealRect2D(anchorPoint.X, anchorPoint.Y, size.Width, size.Height);
			this.showBeak = false;
		}
		public ToolTipLayout(ToolTipLocation location, Point position, Size size, bool showBeak, Point offset, double additionalOffset) {
			this.location = (AnnotationLocation)location;
			this.anchorPoint = new GRealPoint2D(position.X, position.Y);
			this.initOffset = new GRealPoint2D(offset.X, offset.Y);
			this.additionalOffset = additionalOffset;
			this.size = new GRealSize2D(size.Width, size.Height);
			this.bounds = GRealRect2D.Empty;
			this.showBeak = showBeak;
		}
	}
	public class ToolTipLayoutCalculator {
		internal static double CursorOffset { get { return 20.0; } }
		ToolTipControl toolTipControl;
		ToolTipItem ToolTipItem { get { return toolTipControl != null ? toolTipControl.ToolTipItem : null; } }
		bool ShowBeak { get { return ToolTipItem.BeakVisibility == Visibility.Visible; } }
		public ToolTipLayoutCalculator(ToolTipControl toolTipControl) {
			this.toolTipControl = toolTipControl;
		}
		Point CalculateLocalScreenPosition(Point controlAnchorPosition) {
			if (toolTipControl.FlowDirection == FlowDirection.RightToLeft)
				controlAnchorPosition = new Point(toolTipControl.ActualWidth - controlAnchorPosition.X, controlAnchorPosition.Y);
			Point screenOffset = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenPoint(toolTipControl);
			if (toolTipControl.FlowDirection == FlowDirection.RightToLeft)
				screenOffset = new Point(screenOffset.X - toolTipControl.ActualWidth, screenOffset.Y);
			Point screenAnchorPoint = new Point(screenOffset.X + controlAnchorPosition.X, screenOffset.Y + controlAnchorPosition.Y);
			Rect popupScreenRect = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenRect(screenAnchorPoint);
			return new Point(screenAnchorPoint.X - popupScreenRect.X, screenAnchorPoint.Y - popupScreenRect.Y);
		}
		Point CalculateScreenOffset(Point controlAnchorPosition) {
			if (toolTipControl.FlowDirection == FlowDirection.RightToLeft)
				controlAnchorPosition = new Point(toolTipControl.ActualWidth - controlAnchorPosition.X, controlAnchorPosition.Y);
			Point screenOffset = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenPoint(toolTipControl);
			if (toolTipControl.FlowDirection == FlowDirection.RightToLeft)
				screenOffset = new Point(screenOffset.X - toolTipControl.ActualWidth, screenOffset.Y);
			Point screenAnchorPoint = new Point(screenOffset.X + controlAnchorPosition.X, screenOffset.Y + controlAnchorPosition.Y);
			Rect popupScreenRect = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenRect(screenAnchorPoint);
			return new Point(popupScreenRect.X, popupScreenRect.Y);
		}
		Point CalculateScreenAnchorPosition(Point controlAnchorPosition) {
			Point localScreenPosition = CalculateLocalScreenPosition(controlAnchorPosition);
			Point screenAnchorPosition = new Point(localScreenPosition.X - AnnotationPanel.PopupContentShadowPadding,
				localScreenPosition.Y - AnnotationPanel.PopupContentShadowPadding);
			Rect anchorPointScreenRect = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenRect(screenAnchorPosition);
			if (anchorPointScreenRect.Width == 0) {
				Point screenOffset = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenPoint(toolTipControl);
				Point mousePosition = System.Windows.Input.Mouse.GetPosition(toolTipControl);
				Point mouseScreenAnchorPoint = new Point(screenOffset.X + mousePosition.X, screenOffset.Y + mousePosition.Y);
				anchorPointScreenRect = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenRect(mouseScreenAnchorPoint);
			}
			if (screenAnchorPosition.X < anchorPointScreenRect.Left)
				screenAnchorPosition.X = anchorPointScreenRect.Left;			
			return screenAnchorPosition;
		}
		ToolTipLayout CorrectToolTipLayout(ToolTipLayout initialLayout, Point controlAnchorPosition) {
			IAnnotationLayout annotationLayout = (IAnnotationLayout)initialLayout;
			Point screenOffset = CalculateScreenOffset(controlAnchorPosition);
			if (toolTipControl.FlowDirection == FlowDirection.RightToLeft) {
				Size contentSize = toolTipControl.GetToolTipContentSize();
				screenOffset = new Point(screenOffset.X + contentSize.Width + annotationLayout.Offset.X * 2.0, screenOffset.Y);
			}
			annotationLayout.Bounds = new GRealRect2D(annotationLayout.Bounds.Left + AnnotationPanel.PopupContentShadowPadding + screenOffset.X,
				annotationLayout.Bounds.Top + AnnotationPanel.PopupContentShadowPadding + screenOffset.Y,
				annotationLayout.Bounds.Width, annotationLayout.Bounds.Height);
			return initialLayout;
		}
		GRealRect2D CalculateBoundsForFreePosition(Size boundsSize) {
			Point boundsLocation = new Point(0, 0);
			Point screenOffset = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenPoint(toolTipControl);
			if (toolTipControl.FlowDirection == FlowDirection.RightToLeft)
				screenOffset = new Point(screenOffset.X - toolTipControl.ActualWidth, screenOffset.Y);
			Point screenAnchorPoint = new Point(screenOffset.X + boundsLocation.X, screenOffset.Y + boundsLocation.Y);
			Rect popupScreenRect = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenRect(screenAnchorPoint);
			Point localScreenPosition = new Point(screenAnchorPoint.X - popupScreenRect.X, screenAnchorPoint.Y - popupScreenRect.Y);
			boundsLocation = new Point(localScreenPosition.X - AnnotationPanel.PopupContentShadowPadding,
				localScreenPosition.Y - AnnotationPanel.PopupContentShadowPadding);
			if (boundsLocation.X < 0)
				boundsLocation.X = 0;
			if (boundsLocation.Y < 0)
				boundsLocation.Y = 0;
			return new GRealRect2D(boundsLocation.X, boundsLocation.Y, boundsSize.Width, boundsSize.Height);
		}
		ToolTipLayout CalculateLayoutForFreePosition(DockCorner location, Rect layoutRect, Size boundsSize) {
			Point position = new Point(0.0, 0.0);
			Size contentSize = toolTipControl.GetToolTipContentSize();
			switch (location) {
				case DockCorner.TopRight:
				case DockCorner.BottomRight:
					position.X = layoutRect.Right;
					break;
				case DockCorner.TopLeft:
				case DockCorner.BottomLeft:
					position.X = layoutRect.Left;
					break;
			}
			switch (location) {
				case DockCorner.TopRight:
				case DockCorner.TopLeft:
					position.Y = layoutRect.Top;
					break;
				case DockCorner.BottomRight:
				case DockCorner.BottomLeft:
					position.Y = layoutRect.Bottom;
					break;
			}
			Point screenAnchorPosition = CalculateScreenAnchorPosition(position);
			AnnotationLocation convertedLocation = AnnotationLocation.BottomRight;
			switch (location) {
				case DockCorner.TopLeft:
					convertedLocation = AnnotationLocation.BottomRight;
					break;
				case DockCorner.TopRight:
					convertedLocation = AnnotationLocation.BottomLeft;
					break;
				case DockCorner.BottomLeft:
					convertedLocation = AnnotationLocation.TopRight;
					break;
				case DockCorner.BottomRight:
					convertedLocation = AnnotationLocation.TopLeft;
					break;
			}
			ToolTipLayout layout = new ToolTipLayout(screenAnchorPosition, contentSize, ToolTipItem.ToolTipPosition.Offset);
			AnnotationLayoutCalculator.CalculateAutoLayout(layout, convertedLocation, CalculateBoundsForFreePosition(boundsSize));
			return CorrectToolTipLayout(layout, position);
		}
		public ToolTipLayout CalculatePointLayout(Size boundsSize) {
			ToolTipLocation location = ToolTipLocation.TopRight;
			if (ToolTipItem.ToolTipPosition is ToolTipPositionWithLocation)
				location = ((ToolTipPositionWithLocation)ToolTipItem.ToolTipPosition).Location;
			Point controlAnchorPosition = toolTipControl.ToolTipItem.Position;
			Point screenAnchorPosition = CalculateScreenAnchorPosition(controlAnchorPosition);
			Size contentSize = toolTipControl.GetToolTipContentSize();
			Point offset = ShowBeak ? new Point(0, 0) : ToolTipItem.ToolTipPosition.Offset;
			double additionalOffset = (!ShowBeak && ToolTipItem.ToolTipPosition is ToolTipMousePosition) ? CursorOffset : 0.0;
			ToolTipLayout layout = new ToolTipLayout(location, screenAnchorPosition, contentSize, ShowBeak, offset, additionalOffset);
			AnnotationLayoutCalculator.CalculateAutoLayout(layout, (AnnotationLocation)location,
				new GRealRect2D(0, 0, boundsSize.Width - AnnotationPanel.PopupContentShadowPadding, boundsSize.Height - AnnotationPanel.PopupContentShadowPadding));
			return CorrectToolTipLayout(layout, controlAnchorPosition);
		}
		public ToolTipLayout CalculateFreeLayout(Rect layoutRect, Size boundsSize) {
			ToolTipFreePosition toolTipPosition = (ToolTipFreePosition)ToolTipItem.ToolTipPosition;
			return CalculateLayoutForFreePosition(toolTipPosition.DockCorner, layoutRect, boundsSize);
		}
	}
}
