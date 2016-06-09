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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Flyout;
using DevExpress.Xpf.Editors.Flyout.Native;
using DevExpress.Xpf.Navigation;
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.Controls.Primitives {
	public class FlyoutAdornerControl : FlyoutControl {
		FlyoutAdornerContentControl flyoutAdornerContentControl;
		public FlyoutAdornerControl() {
			DefaultStyleKey = typeof(FlyoutAdornerControl);
			AllowOutOfScreen = true;
			AllowMoveAnimation = false;
		}
		protected override FlyoutContainer GetFlyoutContainer() {
			if(flyoutAdornerContentControl != null) {
				flyoutAdornerContentControl.IsOpen = false;
			}
			flyoutAdornerContentControl = (FlyoutAdornerContentControl)GetTemplateChild("PART_Popup");
			return new AdornerFlyoutContainer() { FlyoutAdornerContentControl = flyoutAdornerContentControl };
		}
		protected override void UpdatePopupLocation(Point location) {
			if(!IsPopupVisible || !IsValid(location))
				return;
			FrameworkElement contentControl = (FrameworkElement)GetTemplateChild("PART_cc");
			if(contentControl == null)
				return;
			Point popupOffset = NormalizeToScreenOrigin(location);
			FrameworkElement owner = DevExpress.Xpf.Core.Native.LayoutHelper.GetTopLevelVisual(this);
			if(owner != null) {
				MatrixTransform transform = this.TransformToVisual(owner) as MatrixTransform;
				Matrix matrix = transform.Matrix;
				matrix.Invert();
				transform = new MatrixTransform(Math.Abs(matrix.M11), matrix.M12, matrix.M21, Math.Abs(matrix.M22), 0, 0);
				if(!transform.Matrix.IsIdentity) {
					popupOffset = transform.Transform(popupOffset);
					contentControl.LayoutTransform = transform;
				}
			}
			popupOffset = TransformToDeviceDPI(owner, popupOffset);
			FrameworkElement container = LayoutHelper.GetTopContainerWithAdornerLayer(Window.GetWindow(this)) as FrameworkElement ?? owner;
			Point windowOffset = container.PointFromScreen(popupOffset);
			contentControl.SetLeft(windowOffset.X + HorizontalOffset);
			contentControl.SetTop(windowOffset.Y + VerticalOffset);
		}
		static Point TransformToDeviceDPI(FrameworkElement element, Point point) {
			var t = PresentationSource.FromVisual(element).CompositionTarget.TransformToDevice;
			return t.Transform(point);
		}
		protected override Point NormalizeToScreenOrigin(Point point) {
			return point;
		}
		protected override void ClosePopup() {
			if(IsLoaded)
				base.ClosePopup();
			else
				if(!IsOpen)
					FlyoutContainer.Do(x => x.IsOpen = false);
		}
		protected override void IsOpenChanged(bool newValue) {
			if(RenderGrid != null)
				RenderGrid.InvalidateMeasure();
			base.IsOpenChanged(newValue);
		}
	}
	[ContentProperty("Content")]
	public class FlyoutAdornerContentControl : Control {
		#region static
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof(object), typeof(FlyoutAdornerContentControl), new PropertyMetadata(null, OnContentChanged));
		static void OnContentChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((FlyoutAdornerContentControl)dObj).OnContentChanged(e.OldValue, e.NewValue);
		}
		public static readonly DependencyProperty IsOpenProperty =
			DependencyProperty.Register("IsOpen", typeof(bool), typeof(FlyoutAdornerContentControl), new PropertyMetadata(false, OnIsOpenChanged));
		static void OnIsOpenChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((FlyoutAdornerContentControl)dObj).OnIsOpenChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		#endregion
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public bool IsOpen {
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				List<object> list = new List<object>();
				if(container != null)
					list.Add(container);
				if(Content != null)
					list.Add(Content);
				return list.GetEnumerator();
			}
		}
		FloatingContainer container;
		public FlyoutAdornerContentControl() { }
		protected virtual void OnContentChanged(object oldValue, object newValue) {
			if(oldValue != null)
				RemoveLogicalChild(oldValue);
			if(newValue != null)
				AddLogicalChild(newValue);
			if(container != null)
				EnsureFloatingContainer();
		}
		protected virtual void OnIsOpenChanged(bool oldValue, bool newValue) {
			if(container == null)
				container = CreateFloatingContainer();
			EnsureFloatingContainer();
			container.IsOpen = newValue;
			if(newValue) {
				RaiseOpened();
			} else {
				RemoveLogicalChild(container);
				container.Content = null;
				container.Owner = null;
				container.Close();
				container = null;
				RaiseClosed();
			}
		}
		protected virtual FloatingContainer CreateFloatingContainer() {
			FrameworkElement owner = LayoutHelper.GetTopContainerWithAdornerLayer(Window.GetWindow(this)) as FrameworkElement;
			if(owner == null) {
				owner = this;
			}
			var container = new FlyoutFloatingContainer() { Owner = owner };
			container.ShowContentOnly = true;
			container.SizeToContent = SizeToContent.Manual;
			container.UseSizingMargin = false;
			container.UseActiveStateOnly = true;
			container.FloatSize = new Size(owner.ActualWidth, owner.ActualHeight);
			owner.SizeChanged += owner_SizeChanged;
			AddLogicalChild(container);
			return container;
		}
		void owner_SizeChanged(object sender, SizeChangedEventArgs e) {
			if(container != null) {
				container.FloatSize = e.NewSize;
			}
		}
		void EnsureFloatingContainer() {
			container.BeginUpdate();
			container.Content = Content;
			container.EndUpdate();
		}
		public event EventHandler Closed;
		public event EventHandler Opened;
		protected virtual void RaiseOpened() {
			if(Opened != null)
				Opened(this, EventArgs.Empty);
		}
		protected virtual void RaiseClosed() {
			if(Closed != null)
				Closed(this, EventArgs.Empty);
		}
		class FlyoutFloatingContainer : FloatingAdornerContainer {
			public FlyoutFloatingContainer() {
				ContainerFocusable = false;
			}
			protected override PlacementAdorner CreatePlacementAdorner() {
				return new FlyoutPlacementAdorner(Owner);
			}
		}
		class FlyoutPlacementAdorner : PlacementAdorner {
			public FlyoutPlacementAdorner(UIElement container)
				: base(container) {
			}
			protected override BaseSurfacedAdorner.BaseAdornerSurface CreateAdornerSurface() {
				return new FlyoutAdornerSurface(this);
			}
			class FlyoutAdornerSurface : AdornerSurface {
				public FlyoutAdornerSurface(PlacementAdorner adorner)
					: base(adorner, true) {
				}
				protected override Size MeasureOverride(Size availableSize) {
					foreach(KeyValuePair<UIElement, PlacementItemInfo> pair in PlacementInfos) {
						pair.Key.Measure(pair.Value.Bounds.Size);
					}
					return new Size();
				}
			}
		}
	}
	class AdornerFlyoutContainer : FlyoutContainer {
		public FlyoutAdornerContentControl FlyoutAdornerContentControl { get; set; }
		public override FrameworkElement Element {
			get { return FlyoutAdornerContentControl; }
		}
		public override UIElement Child {
			get { return FlyoutAdornerContentControl != null ? (UIElement)FlyoutAdornerContentControl.Content : null; }
			set {
				if(FlyoutAdornerContentControl != null) {
					FlyoutAdornerContentControl.Content = value;
				}
			}
		}
		public override bool IsOpen {
			get { return FlyoutAdornerContentControl.IsOpen; }
			set { FlyoutAdornerContentControl.IsOpen = value; }
		}
		protected override void OpenedSubscribe(EventHandler value) {
			FlyoutAdornerContentControl.Opened += value;
		}
		protected override void OpenedUnSubscribe(EventHandler value) {
			FlyoutAdornerContentControl.Opened -= value;
		}
		protected override void ClosedSubscribe(EventHandler value) {
			FlyoutAdornerContentControl.Closed += value;
		}
		protected override void ClosedUnSubscribe(EventHandler value) {
			FlyoutAdornerContentControl.Closed -= value;
		}
	}
	public class FlyoutDecorator : Control {
		#region static
		public static readonly DependencyProperty FlyoutShowModeProperty =
			DependencyProperty.Register("FlyoutShowMode", typeof(FlyoutShowMode), typeof(FlyoutDecorator), new PropertyMetadata(FlyoutShowMode.Adorner, OnFlyoutShowModeChanged));
		static void OnFlyoutShowModeChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((FlyoutDecorator)dObj).OnFlyoutShowModeChanged(e.OldValue, e.NewValue);
		}
		#endregion static
		public FlyoutShowMode FlyoutShowMode {
			get { return (FlyoutShowMode)GetValue(FlyoutShowModeProperty); }
			set { SetValue(FlyoutShowModeProperty, value); }
		}
		public FlyoutControl ActualFlyoutControl {
			get { return FlyoutShowMode == FlyoutShowMode.Popup ? PartPopupFlyoutControl : PartAdornerFlyoutControl; }
		}
		FlyoutControl PartAdornerFlyoutControl;
		FlyoutControl PartPopupFlyoutControl;
		public FlyoutDecorator() {
			DefaultStyleKey = typeof(FlyoutDecorator);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartPopupFlyoutControl = GetTemplateChild("PART_FlyoutPopupControl") as FlyoutControl;
			PartAdornerFlyoutControl = GetTemplateChild("PART_FlyoutAdornerControl") as FlyoutControl;
		}
		protected virtual void OnFlyoutShowModeChanged(object oldValue, object newValue) { }
	}
}
