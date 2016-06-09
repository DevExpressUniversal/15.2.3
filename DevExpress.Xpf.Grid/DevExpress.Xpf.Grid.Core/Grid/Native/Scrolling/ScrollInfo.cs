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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
#if !SILVERLIGHT
using System.Windows.Media;
#else
using Visual = System.Windows.UIElement;
#endif
namespace DevExpress.Xpf.Grid.Native {
	public abstract class ScrollInfo : DependencyObject, IScrollInfo {
		IScrollInfoOwner owner;
		ScrollInfoBase verticalScrollInfo;
		ScrollInfoBase horizontalScrollInfo;
		ScrollViewer scrollViewer;
		public ScrollInfo(IScrollInfoOwner owner) {
			this.owner = owner;
		}
		protected IScrollInfoOwner Owner { get { return owner; } }
		public abstract ScrollByItemInfo DefineSizeScrollInfo { get; }
		public abstract ScrollByPixelInfo SecondarySizeScrollInfo { get; }
		public virtual ScrollInfoBase HorizontalScrollInfo {
			get {
				if(horizontalScrollInfo == null)
					horizontalScrollInfo = CreateHorizontalScrollInfo();
				return horizontalScrollInfo;
			}
		}
		public virtual ScrollInfoBase VerticalScrollInfo {
			get {
				if(verticalScrollInfo == null)
					verticalScrollInfo = CreateVerticalScrollInfo();
				return verticalScrollInfo;
			}
		}
		public virtual void ClearScrollInfo() {
			horizontalScrollInfo = null;
			verticalScrollInfo = null;
		}
		protected abstract ScrollInfoBase CreateHorizontalScrollInfo();
		protected abstract ScrollInfoBase CreateVerticalScrollInfo();
		#region IScrollInfo Members
		bool canHorizontallyScroll = true;
		bool IScrollInfo.CanHorizontallyScroll {
			get { return canHorizontallyScroll; }
			set { canHorizontallyScroll = value; }
		}
		bool IScrollInfo.CanVerticallyScroll {
			get { return true; }
			set { }
		}
		double IScrollInfo.ExtentHeight { get { return VerticalScrollInfo.Extent; } }
		double IScrollInfo.ExtentWidth { get { return HorizontalScrollInfo.Extent; } }
		double IScrollInfo.VerticalOffset { get { return VerticalScrollInfo.Offset; } }
		double IScrollInfo.HorizontalOffset { get { return HorizontalScrollInfo.Offset; } }
		double IScrollInfo.ViewportHeight { get { return VerticalScrollInfo.Viewport; } }
		double IScrollInfo.ViewportWidth { get { return HorizontalScrollInfo.Viewport; } }
		void IScrollInfo.LineDown() { VerticalScrollInfo.LineDown(); }
		void IScrollInfo.LineUp() { VerticalScrollInfo.LineUp(); }
		void IScrollInfo.LineLeft() { HorizontalScrollInfo.LineUp(); }
		void IScrollInfo.LineRight() { HorizontalScrollInfo.LineDown(); }
		Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle) {
			Rect rect = LayoutHelper.GetRelativeElementRect(visual as UIElement, Owner.ScrollContentPresenter);
			if(rect.Bottom > Owner.ScrollContentPresenter.ActualHeight)
				rect.Y -= rect.Bottom - Owner.ScrollContentPresenter.ActualHeight;
			if(rect.Top < 0)
				rect.Y -= rect.Top;
			if(rect.Right > Owner.ScrollContentPresenter.ActualWidth)
				rect.Width -= Math.Min(rect.Width, rect.Right - Owner.ScrollContentPresenter.ActualWidth);
			if(rect.Left < 0)
				rect.X -= rect.Left;
			FrameworkElement scrollHost = FocusRectPresenter.FindScrollHost(visual, FocusRectPresenter.IsHorizontalScrollHostProperty);
			if(scrollHost != null) {
				Rect scrollHostRect = LayoutHelper.GetRelativeElementRect(scrollHost, owner.ScrollContentPresenter);
				rect.X += scrollHostRect.Left;
			}
			return rect;
		 }
		void IScrollInfo.MouseWheelLeft() { HorizontalScrollInfo.MouseWheelUp(); }
		void IScrollInfo.MouseWheelRight() { HorizontalScrollInfo.MouseWheelDown(); }
		void IScrollInfo.MouseWheelDown() { VerticalScrollInfo.MouseWheelDown(); }
		void IScrollInfo.MouseWheelUp() { VerticalScrollInfo.MouseWheelUp(); }
		void IScrollInfo.PageDown() { VerticalScrollInfo.PageDown(); }
		void IScrollInfo.PageUp() { VerticalScrollInfo.PageUp(); }
		void IScrollInfo.PageLeft() { HorizontalScrollInfo.PageUp(); }
		void IScrollInfo.PageRight() { HorizontalScrollInfo.PageDown(); }
		ScrollViewer IScrollInfo.ScrollOwner { get { return scrollViewer; } set { scrollViewer = value; } }
		void IScrollInfo.SetHorizontalOffset(double offset) { HorizontalScrollInfo.SetOffset(offset); }
		void IScrollInfo.SetVerticalOffset(double offset) { VerticalScrollInfo.SetOffset(offset); }
		#endregion
		public void SetVerticalOffsetForce(double value) {
			VerticalScrollInfo.SetOffsetForce(value);
		}
		public void SetHorizontalOffsetForce(double value) {
			HorizontalScrollInfo.SetOffsetForce(value);
		}
		public virtual void OnScrollInfoChanged() {
			if(scrollViewer != null) {
				scrollViewer.InvalidateScrollInfo();
#if SL          
				if(scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible && scrollViewer.ExtentWidth == scrollViewer.ViewportWidth)
					scrollViewer.InvalidateMeasure();
#else
				Owner.IsTouchScrollBarsMode = ScrollBarExtensions.GetScrollBarMode(scrollViewer) == ScrollBarMode.TouchOverlap;
				Owner.IsHorizontalScrollBarVisible = scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible;
#endif
			}
		}
	}
}
