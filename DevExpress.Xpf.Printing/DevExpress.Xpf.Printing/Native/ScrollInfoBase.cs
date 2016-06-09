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
using DevExpress.Utils;
#if !SL
using System.Windows.Media;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public abstract class ScrollInfoBase : IScrollInfo {
		#region Nested Types
		protected enum ScrollMode { Line, MouseWheel, Page }
		protected enum ScrollDirection { Up, Down }
		#endregion
		#region Fields
		protected readonly FrameworkElement scrollablePageView;
		readonly IPreviewModel model;
		const double ScrollFactor = 3d / 100d;
		protected const double ScrollWheelFactor = 3;
		public event EventHandler<EventArgs> HorizontalOffsetChanged;
		public event EventHandler<EventArgs> VerticalOffsetChanged;
		ScrollViewer scrollOwner;
		Thickness pageMargin;
		double horizontalOffset;
		double verticalOffset;
		#endregion
		#region Properties
		internal double PageViewWidth { get; private set; }
		internal double PageViewHeight { get; private set; }
		public virtual Point PageWithMarginPosition { get; set; }
		public Thickness PageMargin {
			get {
				return pageMargin;
			}
			set {
				if(pageMargin == value)
					return;
				pageMargin = value;
				InvalidateScrollInfo();
			}
		}
		protected double ScrollablePageViewWidth {
			get {
				if(model == null || model.PageCount == 0)
					return 0;
				return PageViewWidth + pageMargin.Left + pageMargin.Right;
			}
		}
		protected abstract double ScrollablePageViewHeight { get; }
		double ScrollableWidth {
			get { return Math.Max(0, ExtentWidth - ViewportWidth); }
		}
		double ScrollableHeight {
			get { return Math.Max(0, ExtentHeight - ViewportHeight); }
		}
		double ScrollLineWidth {
			get {
				if(scrollOwner == null)
					return 0;
				return ScrollFactor * ViewportWidth;
			}
		}
		protected double ScrollLineHeight {
			get {
				return ScrollFactor * ViewportHeight;
			}
		}
		protected double ScrollablePageViewLocalVerticalOffset {
			get;
			private set;
		}
		double HorizontalRelativityFactor {
			get { return (ScrollablePageViewWidth - ViewportWidth) / ScrollableWidth; }
		}
		double VerticalRelativityFactor {
			get {
				double scrollablePageHeight = ScrollableHeight / model.PageCount;
				return (ScrollablePageViewHeight - ViewportHeight) / scrollablePageHeight;
			}
		}
		#endregion
		#region Constructors
		public ScrollInfoBase(FrameworkElement scrollablePageView, IPreviewModel model, Thickness pageMargin) {
			Guard.ArgumentNotNull(scrollablePageView, "scrollablePageView");
			this.scrollablePageView = scrollablePageView;
			this.model = model;
			if(model != null) {
				PageViewWidth = model.PageViewWidth;
				PageViewHeight = model.PageViewHeight;
				model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
			}
			this.pageMargin = pageMargin;
		}
		#endregion
		#region IScrollInfo Members
		public bool CanHorizontallyScroll {
			get { return model != null && ScrollOwner != null; }
			set { }
		}
		public bool CanVerticallyScroll {
			get { return model != null && ScrollOwner != null; }
			set { }
		}
		public abstract double ExtentHeight { get; }
		public virtual double ExtentWidth {
			get {
				if(model == null || model.PageCount == 0)
					return 0;
				return ScrollablePageViewWidth;
			}
		}
		public double HorizontalOffset {
			get {
				return horizontalOffset;
			}
		}
		public void LineLeft() {
			SetHorizontalOffset(Math.Max(0, HorizontalOffset - ScrollLineWidth));
			InvalidateScrollInfo();
		}
		public void LineRight() {
			SetHorizontalOffset(Math.Min(HorizontalOffset + ScrollLineWidth, ScrollableWidth));
			InvalidateScrollInfo();
		}
		public void LineDown() {
			SetVerticalOffset(Math.Min(GetVerticalScrollOffset(ScrollMode.Line, ScrollDirection.Down), ScrollableHeight));
			InvalidateScrollInfo();
		}
		public void LineUp() {
			SetVerticalOffset(Math.Max(GetVerticalScrollOffset(ScrollMode.Line, ScrollDirection.Up), 0));
			InvalidateScrollInfo();
		}
#if SL
		public abstract Rect MakeVisible(UIElement element, Rect rectangle);
#else
		public abstract Rect MakeVisible(Visual visual, Rect rectangle);
#endif
		public void MouseWheelDown() {
			SetVerticalOffset(Math.Min(GetVerticalScrollOffset(ScrollMode.MouseWheel, ScrollDirection.Down), ScrollableHeight));
			InvalidateScrollInfo();
		}
		public void MouseWheelLeft() {
			SetHorizontalOffset(Math.Min(0, HorizontalOffset - ScrollWheelFactor * ScrollLineWidth));
			InvalidateScrollInfo();
		}
		public void MouseWheelRight() {
			SetHorizontalOffset(Math.Min(HorizontalOffset + ScrollLineWidth, ScrollWheelFactor * ScrollableWidth));
			InvalidateScrollInfo();
		}
		public void MouseWheelUp() {
			SetVerticalOffset(Math.Max(GetVerticalScrollOffset(ScrollMode.MouseWheel, ScrollDirection.Up), 0));
			InvalidateScrollInfo();
		}
		public void PageDown() {
			SetVerticalOffset(Math.Min(GetVerticalScrollOffset(ScrollMode.Page, ScrollDirection.Down), ScrollableHeight));
			InvalidateScrollInfo();
		}
		public void PageLeft() {
			SetHorizontalOffset(Math.Min(0, HorizontalOffset - ViewportWidth));
			InvalidateScrollInfo();
		}
		public void PageRight() {
			SetHorizontalOffset(Math.Min(HorizontalOffset + ViewportWidth, ScrollableWidth));
			InvalidateScrollInfo();
		}
		public void PageUp() {
			SetVerticalOffset(Math.Max(GetVerticalScrollOffset(ScrollMode.Page, ScrollDirection.Up), 0));
			InvalidateScrollInfo();
		}
		public ScrollViewer ScrollOwner {
			get {
				return scrollOwner;
			}
			set {
				scrollOwner = value;
			}
		}
		public void SetHorizontalOffset(double offset) {
			if(horizontalOffset == offset)
				return;
			horizontalOffset = offset;
			OnHorizontalOffsetChanged();
			scrollablePageView.InvalidateArrange();
		}
		public virtual void SetVerticalOffset(double offset) {
			if(VerticalOffset == offset)
				return;
			VerticalOffset = offset;
			OnVerticalOffsetChanged();
			scrollablePageView.InvalidateArrange();
		}
		public double VerticalOffset {
			get { return verticalOffset; }
			protected set {
				verticalOffset = value;
				UpdateScrollablePageViewLocalVerticalOffset();
			}
		}
		public double ViewportHeight {
			get { return scrollablePageView.ActualHeight; }
		}
		public double ViewportWidth {
			get { return scrollablePageView.ActualWidth; }
		}
		#endregion
		#region Methods
		public abstract double GetTransformX();
		public abstract double GetTransformY();
		protected double GetStep(ScrollMode scrollMode) {
			if(ScrollablePageViewHeight > ViewportHeight) {
				if(scrollMode == ScrollMode.Line)
					return ScrollLineHeight;
				if(scrollMode == ScrollMode.MouseWheel)
					return ScrollWheelFactor * ScrollLineHeight;
				return ViewportHeight;
			}
			return model.PageCount == 1 ? 0 : ScrollableHeight / (model.PageCount - 1);
		}
		public bool IsHorizontalScrollDataValid() {
			return model != null && model.PageCount > 0 && ScrollableWidth > 0 && ScrollOwner != null;
		}
		public bool IsVerticalScrollDataValid() {
			return model != null && model.PageCount > 0 && ScrollableHeight > 0 && ScrollOwner != null;
		}
		public void ValidateScrollData() {
			horizontalOffset = Math.Max(0, Math.Min(horizontalOffset, ScrollableWidth));
			verticalOffset = Math.Max(0, Math.Min(verticalOffset, ScrollableHeight));
		}
		public abstract void SetCurrentPageIndex();
		protected virtual void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if(e.PropertyName == "PageViewWidth") {
				PageViewWidth = model.PageViewWidth;
				return;
			}
			if(e.PropertyName == "PageViewHeight") {
				double oldScrollableHeight = ScrollableHeight;
				PageViewHeight = model.PageViewHeight;
				if(oldScrollableHeight != 0) {
					double newScrollableHeight = ScrollableHeight;
					verticalOffset = newScrollableHeight / oldScrollableHeight * verticalOffset;
					scrollablePageView.InvalidateArrange();
				}
				return;
			}
			if(e.PropertyName == "PageCount" || e.PropertyName == "Zoom") {
				scrollablePageView.InvalidateArrange();
				return;
			}
		}
		protected abstract double GetVerticalScrollOffset(ScrollMode scrollMode, ScrollDirection scrollDirection);
		public void InvalidateScrollInfo() {
			if(scrollOwner != null)
				scrollOwner.InvalidateScrollInfo();
		}
		protected void UpdateScrollablePageViewLocalVerticalOffset() {
			ScrollablePageViewLocalVerticalOffset = (model != null && model is IDocumentPreviewModel && model.PageCount != 0)
				? verticalOffset - ScrollableHeight / model.PageCount * ((IDocumentPreviewModel)model).CurrentPageIndex
				: 0d;
		}
		void OnHorizontalOffsetChanged() {
			if(HorizontalOffsetChanged != null)
				HorizontalOffsetChanged(this, EventArgs.Empty);
		}
		void OnVerticalOffsetChanged() {
			if(VerticalOffsetChanged != null)
				VerticalOffsetChanged(this, EventArgs.Empty);
		}
		#endregion
	}
}
