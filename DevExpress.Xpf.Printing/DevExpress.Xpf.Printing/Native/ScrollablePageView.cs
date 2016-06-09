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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class ScrollablePageView : Control, IScrollInfo {
		#region Fields and Properties
		public static readonly DependencyProperty ModelProperty;
		public static readonly DependencyProperty PageMarginProperty;
		InputController inputController;
		readonly Size InfiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
		protected readonly TranslateTransform transform = new TranslateTransform();
#if !SL
		readonly ManipulationHelper manipulationHelper;
#endif
		protected PageDraggingImplementer pageDraggingImplementer;
		protected ScrollInfoBase scrollInfo;
		protected double pageWithMarginPositionX;
		protected double pageWithMarginPositionY;
		public IPreviewModel Model {
			get { return (IPreviewModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		public Thickness PageMargin {
			get { return (Thickness)GetValue(PageMarginProperty); }
			set { SetValue(PageMarginProperty, value); }
		}
#if DEBUGTEST
		public PageDraggingImplementer TEST_PageDraggingImplementer { get { return pageDraggingImplementer; } }
#endif
		#endregion
		#region Constructors
		static ScrollablePageView() {
			ModelProperty = DependencyPropertyManager.Register(
			   "Model",
			   typeof(IPreviewModel),
			   typeof(ScrollablePageView),
			   new PropertyMetadata(null, ModelChangedCallback));
			PageMarginProperty =
				DependencyPropertyManager.Register("PageMargin",
				typeof(Thickness),
				typeof(ScrollablePageView),
				new PropertyMetadata(new Thickness(3), PageMarginChangedCallback));
#if !SL
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollablePageView), new FrameworkPropertyMetadata(typeof(ScrollablePageView)));
#endif
		}
		public ScrollablePageView() {
#if SL
			this.DefaultStyleKey = typeof(ScrollablePageView);
#endif
			KeyDown += ScrollablePageView_KeyDown;
			KeyUp += ScrollablePageView_KeyUp;
			MouseLeftButtonDown += ScrollablePageView_MouseLeftButtonDown;
			MouseLeftButtonUp += ScrollablePageView_MouseLeftButtonUp;
			MouseRightButtonDown += ScrollablePageView_MouseRightButtonDown;
			MouseWheel += ScrollablePageView_MouseWheel;
			MouseMove += ScrollablePageView_MouseMove;
			Loaded += ScrollablePageView_Loaded;
			SizeChanged += ScrollablePageView_SizeChanged;
#if !SL
			IsManipulationEnabled = true;
			manipulationHelper = new ManipulationHelper(this);
#endif
			scrollInfo = new NullScrollInfo(this);
		}
		#endregion
		#region IScrollInfo Members
		public bool CanHorizontallyScroll {
			get { return scrollInfo.CanHorizontallyScroll; }
			set { scrollInfo.CanHorizontallyScroll = value; }
		}
		public bool CanVerticallyScroll {
			get { return scrollInfo.CanVerticallyScroll; }
			set { scrollInfo.CanVerticallyScroll = value; }
		}
		public double ExtentHeight {
			get {
				return scrollInfo.ExtentHeight;
			}
		}
		public double ExtentWidth {
			get {
				return scrollInfo.ExtentWidth;
			}
		}
		public double HorizontalOffset {
			get {
				return scrollInfo.HorizontalOffset;
			}
		}
		public void LineRight() {
			scrollInfo.LineRight();
		}
		public void LineLeft() {
			scrollInfo.LineLeft();
		}
		public void MouseWheelLeft() {
			scrollInfo.MouseWheelLeft();
		}
		public void MouseWheelRight() {
			scrollInfo.MouseWheelRight();
		}
		public void PageDown() {
			scrollInfo.PageDown();
		}
		public void MouseWheelDown() {
			scrollInfo.MouseWheelDown();
		}
		public void LineDown() {
			scrollInfo.LineDown();
		}
		public void PageUp() {
			scrollInfo.PageUp();
		}
		public void MouseWheelUp() {
			scrollInfo.MouseWheelUp();
		}
		public void LineUp() {
			scrollInfo.LineUp();
		}
		public void PageLeft() {
			scrollInfo.PageLeft();
		}
		public void PageRight() {
			scrollInfo.PageRight();
		}
		public ScrollViewer ScrollOwner {
			get { return scrollInfo.ScrollOwner; }
			set {
				UnsubscribeScrollOwnerFromEvents();
				scrollInfo.ScrollOwner = value;
				SubscribeScrollOwnerToEvents();
			}
		}
		public void SetHorizontalOffset(double offset) {
			scrollInfo.SetHorizontalOffset(offset);
		}
		public void SetVerticalOffset(double offset) {
			scrollInfo.SetVerticalOffset(offset);
		}
		public double VerticalOffset {
			get { return scrollInfo.VerticalOffset; }
		}
		public double ViewportHeight {
			get { return scrollInfo.ViewportHeight; }
		}
		public double ViewportWidth {
			get { return scrollInfo.ViewportWidth; }
		}
#if SL
		public Rect MakeVisible(UIElement element, Rect rectangle) {
			return scrollInfo.MakeVisible(element, rectangle);
		}
#else
		public Rect MakeVisible(Visual visual, Rect rectangle) {
			if(visual == this)
				return rectangle;
			return scrollInfo.MakeVisible(visual, rectangle);
		}
#endif
		#endregion
		#region Methods
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			var border = GetPageBorder();
			if(border != null)
				border.RenderTransform = transform;
		}
		void ScrollablePageView_MouseWheel(object sender, MouseWheelEventArgs e) {
			if(inputController != null && InputController.AreModifiersPressed) {
				inputController.HandleMouseWheel(e.Delta);
				e.Handled = true;
			}
		}
		void ScrollablePageView_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			Focus();
			if(inputController != null && InputController.AreModifiersPressed)
				inputController.HandleMouseDown(MouseButton.Right);
		}
		void ScrollablePageView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			Focus();
			if(inputController != null && InputController.AreModifiersPressed)
				inputController.HandleMouseDown(MouseButton.Left);
			if(pageDraggingImplementer != null && pageDraggingImplementer.IsPageDraggingEnabled) {
				Point position = e.GetPosition(this);
				pageDraggingImplementer.HandleMouseDown(position);
			} else
				SendMouseDownToModel(e);
		}
		void ScrollablePageView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			if(pageDraggingImplementer != null && pageDraggingImplementer.IsPageDraggingEnabled) {
				bool handled;
				pageDraggingImplementer.HandleMouseUp(out handled);
				e.Handled = handled;
			} else {
				if(Model != null && Model.PageContent != null)
					Model.PageContent.IsHitTestVisible = true;
			}
		}
		void ScrollablePageView_MouseMove(object sender, MouseEventArgs e) {
			SendMouseMoveToModel(e);
		}
		void ScrollablePageView_KeyDown(object sender, KeyEventArgs e) {
#if SL
			if(IsScrollNavigationByKeyUsed(e.Key) || e.Key == Key.Unknown)
#else
			if(IsScrollNavigationByKeyUsed(e.Key) || e.Key == Key.System)
#endif
				return;
			else if(inputController != null)
				inputController.HandleKeyDown(e.Key);
			if(pageDraggingImplementer != null) {
				pageDraggingImplementer.HandleKeyDown(e.Key);
				if(pageDraggingImplementer.IsPageDraggingEnabled && Model != null && Model.PageContent != null)
					Model.PageContent.IsHitTestVisible = false;
			}
			if(ShouldSuppressScrollNavigation(e.Key))
				e.Handled = true;
		}
		void ScrollablePageView_KeyUp(object sender, KeyEventArgs e) {
			if(pageDraggingImplementer != null) {
				pageDraggingImplementer.HandleKeyUp(e.Key);
				if(!pageDraggingImplementer.IsPageDraggingEnabled && Model != null && Model.PageContent != null)
					Model.PageContent.IsHitTestVisible = true;
			}
		}
		void SubscribeScrollOwnerToEvents() {
			if(ScrollOwner != null) {
				ScrollOwner.MouseMove += ScrollOwner_MouseMove;
				ScrollOwner.MouseLeftButtonUp += ScrollOwner_MouseUp;
				ScrollOwner.MouseDoubleClick += ScrollOwner_MouseDoubleClick;
			}
		}
		void UnsubscribeScrollOwnerFromEvents() {
			if(ScrollOwner != null) {
				ScrollOwner.MouseMove -= ScrollOwner_MouseMove;
				ScrollOwner.MouseLeftButtonUp -= ScrollOwner_MouseUp;
				ScrollOwner.MouseDoubleClick -= ScrollOwner_MouseDoubleClick;
			}
		}
		void ScrollOwner_MouseUp(object sender, MouseButtonEventArgs e) {
			if(pageDraggingImplementer == null || (pageDraggingImplementer != null && !pageDraggingImplementer.IsPageDraggingEnabled))
				SendMouseUpToModel(e);
		}
		void ScrollOwner_MouseMove(object sender, MouseEventArgs e) {
			if(pageDraggingImplementer != null) {
				Point position = e.GetPosition(this);
				pageDraggingImplementer.HandleMouseMove(position);
			}
		}
		void ScrollOwner_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if(pageDraggingImplementer == null || !pageDraggingImplementer.IsPageDraggingEnabled) {
				SendMouseDoubleClickToModel(e);
			}
		}
		void SendMouseDoubleClickToModel(MouseEventArgs e) {
			if(Model != null) {
				Model.HandlePreviewDoubleClick(e, this);
			}
		}
		void SendMouseMoveToModel(MouseEventArgs e) {
			if(Model != null)
				Model.HandlePreviewMouseMove(e, this);
		}
		void SendMouseDownToModel(MouseButtonEventArgs e) {
			if(Model != null)
				Model.HandlePreviewMouseLeftButtonDown(e, this);
		}
		void SendMouseUpToModel(MouseButtonEventArgs e) {
			if(Model != null) {
				Model.HandlePreviewMouseLeftButtonUp(e, this);
			}
		}
#if !SL
		protected override void OnManipulationStarting(ManipulationStartingEventArgs e) {
			base.OnManipulationStarting(e);
			manipulationHelper.OnManipulationStarting(e);
		}
		protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e) {
			base.OnManipulationInertiaStarting(e);
			manipulationHelper.OnManipulationInertiaStarting(e);
		}
		protected override void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			base.OnManipulationDelta(e);
			Model.Zoom *= Math.Max(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.Y);
			manipulationHelper.OnManipulationDelta(e);
		}
#endif
		bool IsScrollNavigationByKeyUsed(Key pressedKey) {
			bool isHorizontalScrollByKeyAllowed = (pressedKey == Key.Left || pressedKey == Key.Right) && (ExtentWidth - ViewportWidth > 0);
			bool isVerticalScrollByKeyAllowed = (pressedKey == Key.Up || pressedKey == Key.Down) && (ExtentHeight - ViewportHeight > 0);
			return !InputController.AreModifiersPressed && ((pressedKey == Key.PageDown || pressedKey == Key.PageUp) || (isHorizontalScrollByKeyAllowed || isVerticalScrollByKeyAllowed));
		}
		bool ShouldSuppressScrollNavigation(Key pressedKey) {
			return InputController.AreModifiersPressed &&
				((pressedKey == Key.Left || pressedKey == Key.Right || pressedKey == Key.Up || pressedKey == Key.Down) || (pressedKey == Key.PageDown || pressedKey == Key.PageUp
#if !SL
				|| pressedKey == Key.Prior || pressedKey == Key.Next
#endif
));
		}
		void ScrollablePageView_Loaded(object sender, RoutedEventArgs e) {
			var border = GetPageBorder();
			if(border != null)
				border.RenderTransform = transform;
		}
		void ScrollablePageView_SizeChanged(object sender, SizeChangedEventArgs e) {
#if SL
			InvalidateMeasure();
#else
			scrollInfo.InvalidateScrollInfo();
#endif
			if(Model != null && Model.ZoomMode is ZoomFitModeItem) {
				SyncZoom(((ZoomFitModeItem)Model.ZoomMode).ZoomFitMode);
			}
		}
		protected void SyncZoom(ZoomFitMode mode) {
			if(Model.PageContent == null || Model.PageContent.Width.IsNotNumber() || Model.PageContent.Height.IsNotNumber())
				return;
			double yZoom = 100 * (ViewportHeight - PageMargin.Top - PageMargin.Bottom - BorderThickness.Top - BorderThickness.Left) / Model.PageContent.Height;
			double xZoom = 100 * (ViewportWidth - PageMargin.Left - PageMargin.Right - BorderThickness.Left - BorderThickness.Right) / Model.PageContent.Width;
			switch(mode) {
				case ZoomFitMode.PageHeight: Model.SetZoom(yZoom); break;
				case ZoomFitMode.PageWidth: Model.SetZoom(xZoom); break;
				case ZoomFitMode.WholePage: Model.SetZoom(yZoom > xZoom ? xZoom : yZoom); break;
				default: throw new NotSupportedException();
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			var pageBorder = GetPageBorder();
			if(pageBorder == null)
				return constraint;
			pageBorder.Measure(InfiniteSize);
			return new Size(double.IsInfinity(constraint.Width) ? pageBorder.DesiredSize.Width : constraint.Width,
				double.IsInfinity(constraint.Height) ? pageBorder.DesiredSize.Height : constraint.Height);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			base.ArrangeOverride(arrangeBounds);
			FrameworkElement pageBorder = GetPageBorder();
			if(pageBorder != null) {
				double centerPagePositionX = arrangeBounds.Width > pageBorder.DesiredSize.Width ? (arrangeBounds.Width - pageBorder.DesiredSize.Width) / 2 : 0;
				double centerPagePositionY = arrangeBounds.Height > pageBorder.DesiredSize.Height ? (arrangeBounds.Height - pageBorder.DesiredSize.Height) / 2 : 0;
				Point pageDraggingOffset = pageDraggingImplementer != null ? pageDraggingImplementer.DragOffset : new Point();
				pageWithMarginPositionX = Math.Round(centerPagePositionX) + pageDraggingOffset.X;
				pageWithMarginPositionY = Math.Round(centerPagePositionY) + pageDraggingOffset.Y;
				pageBorder.Arrange(new Rect(new Point(Math.Max(0d, pageWithMarginPositionX), Math.Max(0d, pageWithMarginPositionY)), pageBorder.DesiredSize));
			}
			scrollInfo.ValidateScrollData();
			scrollInfo.InvalidateScrollInfo();
			if(scrollInfo.IsVerticalScrollDataValid())
				scrollInfo.SetCurrentPageIndex();
			transform.X = scrollInfo.IsHorizontalScrollDataValid() ? Math.Round(scrollInfo.GetTransformX()) : 0d;
			transform.Y = scrollInfo.IsVerticalScrollDataValid() ? Math.Round(scrollInfo.GetTransformY()) : 0d;
			if(pageBorder != null) {
#if SL
			UIElement topLevelVisual = Application.Current.RootVisual;
#else
				UIElement topLevelVisual = (UIElement)LayoutHelper.GetTopLevelVisual(this);
#endif
				Point renderOffset = RenderOffsetHelper.GetCorrectedRenderOffset(pageBorder, topLevelVisual);
				transform.X += renderOffset.X;
				transform.Y += renderOffset.Y;
			}
			return arrangeBounds;
		}
		public virtual void UpdatePagePosition() {
		}
		protected FrameworkElement GetPageBorder() {
			return (FrameworkElement)GetTemplateChild("pageBorder");
		}
		static void ModelChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			((ScrollablePageView)sender).OnModelChanged(e);
		}
		protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e) {
			var scrollOwner = scrollInfo.ScrollOwner;
			if(e.OldValue != null) {
				IPreviewModel oldModel = (IPreviewModel)e.OldValue;
				scrollInfo = new NullScrollInfo(this);
				oldModel.PropertyChanged -= Model_PropertyChanged;
			}
			if(e.NewValue != null) {
				IPreviewModel newModel = (IPreviewModel)e.NewValue;
				newModel.PropertyChanged += Model_PropertyChanged;
				pageDraggingImplementer = new PageDraggingImplementer(newModel, this, PageDraggingType.DragViaScrollViewer);
				inputController = Model.InputController;
				if(newModel.UseSimpleScrolling) {
					scrollInfo = new SimpleScrollInfo(this, newModel, this.PageMargin);
				} else {
					if(newModel is IDocumentPreviewModel) {
						scrollInfo = new ScrollInfo(this, (IDocumentPreviewModel)newModel, this.PageMargin);
					}
				}
			}
			scrollInfo.ScrollOwner = scrollOwner;
		}
		protected virtual void Model_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(Model.ZoomMode is ZoomFitModeItem && (e.PropertyName == "ZoomMode" || e.PropertyName == "IsCreating" || e.PropertyName == "PageContent")) {
				SyncZoom(((ZoomFitModeItem)Model.ZoomMode).ZoomFitMode);
			}
		}
		static void PageMarginChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			((ScrollablePageView)sender).OnPageMarginChanged(e);
		}
		void OnPageMarginChanged(DependencyPropertyChangedEventArgs e) {
			scrollInfo.PageMargin = (Thickness)e.NewValue;
		}
		#endregion
	}
}
