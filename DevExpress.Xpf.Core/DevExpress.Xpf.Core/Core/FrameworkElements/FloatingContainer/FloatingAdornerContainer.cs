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
using System.Windows.Documents;
using System.Windows.Input;
#if !DXWINDOW
using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public class FloatingAdornerContainer : FloatingContainer {
		static FloatingAdornerContainer() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingAdornerContainer), new FrameworkPropertyMetadata(typeof(FloatingAdornerContainer)));
		}
		protected internal AdornerContentHolder ContentHolder { get; set; }
		protected internal PlacementAdorner PlacementAdorner { get; set; }
		protected override FloatingMode GetFloatingMode() {
			return FloatingMode.Adorner;
		}
		protected override void CloseCore() {
			base.CloseCore();
			if(IsClosingCanceled) return;
			PlacementAdorner.Unregister(ContentHolder);
			this.RemoveDelayedExecute();
			ContentHolder = null;
			Deactivate();
		}
		protected override UIElement InitDialogCorrectOwner(UIElement element) {
			return (UIElement)LayoutHelper.GetTopContainerWithAdornerLayer(element);
		}
		protected override void OnSizeToContentChangedCore(SizeToContent newVal) {
			base.OnSizeToContentChangedCore(newVal);
			this.DelayedExecute(() => {
				if(newVal != System.Windows.SizeToContent.Manual)
					isAutoSizeUpdating++;
				UpdateStartupLocation();
				if(newVal != System.Windows.SizeToContent.Manual)
					isAutoSizeUpdating--;
			});
		}
		protected override void OnHided() {
			Deactivate();
			base.OnHided();
		}
		public override void Activate() {
			SetValue(FloatingContainer.IsActiveProperty, true);
		}
		public bool InvertLeftAndRightOffsets {
			get { return (FlowDirection == System.Windows.FlowDirection.RightToLeft) && PlacementAdorner.FlowDirection == FlowDirection; }
		}
		private bool _CanUseSizingMargin = true;
		public bool UseSizingMargin {
			get { return _CanUseSizingMargin; }
			set { _CanUseSizingMargin = value; }
		}
		protected void Deactivate() {
			if(DeactivateOnClose) {
				if(PlacementAdorner != null && PlacementAdorner.IsActivated)
					PlacementAdorner.Deactivate();
			}
		}
		protected virtual PlacementAdorner CreatePlacementAdorner() {
			return new PlacementAdorner(Owner);
		}
		protected override UIElement CreateContentContainer() {
			PlacementAdorner = FindPlacementAdorner(Owner);
			if(PlacementAdorner == null) {
				PlacementAdorner = CreatePlacementAdorner();
				PlacementAdorner.KeyDown += new KeyEventHandler(OnPlacementAdornerKeyDown);
				PlacementAdorner.PlacementSurface.SizeChanged += new SizeChangedEventHandler(OnPlacementAdornerSizeChanged);
			}
			ContentHolder = CreateAdornerContentHolder();
			ContentHolder.Focusable = ContainerFocusable;
			return ContentHolder;
		}
		protected virtual void OnPlacementAdornerKeyDown(object sender, KeyEventArgs e) {
			if(CloseOnEscape && e.Key == Key.Escape) {
				e.Handled = true;
				ProcessHiding();
			}
		}
		protected override bool IsAlive {
			get { return ContentHolder != null; }
		}
		int lockFloatingBoundsChanging = 0;
		protected override void UpdateFloatingBoundsCore(Rect bounds) {
			if(lockFloatingBoundsChanging > 0) return;
			lockFloatingBoundsChanging++;
			bounds = new Rect(bounds.Location, EnsureAutoSize(bounds.Size));
			ActualSize = bounds.Size;
			PlacementAdorner.SetBoundsInContainer(ContentHolder, bounds);
			lockFloatingBoundsChanging--;
		}
		protected override void OnLocationChanged(Point newLocation) {
			FloatLocation = CheckLocation(newLocation, GetWorkingArea(), GetSizingMargin());
		}
		protected override void CalcTopOffset(double absHChange, ref double dy, ref double sy) {
			absHChange = CheckHChange(FloatLocation.Y, absHChange, GetWorkingArea(), GetSizingMargin());
			base.CalcTopOffset(absHChange, ref dy, ref sy);
		}
		protected override void CalcLeftOffset(double absWChange, ref double dx, ref double sx) {
			if(!InvertLeftAndRightOffsets) {
				absWChange = CheckWChange(FloatLocation.X, absWChange, GetWorkingArea(), GetSizingMargin());
				dx = absWChange;
				sx = -absWChange;
			}
			else sx = -absWChange;
		}
		protected override void CalcRightOffset(double absWChange, ref double dx, ref double sx) {
			if(InvertLeftAndRightOffsets) {
				absWChange = -CheckWChange(FloatLocation.X, -absWChange, GetWorkingArea(), GetSizingMargin());
				dx = absWChange;
				sx = absWChange;
			}
			else sx = absWChange;
		}
		void OnPlacementAdornerSizeChanged(object sender, SizeChangedEventArgs e) {
			FloatLocation = CheckLocation(FloatLocation, GetWorkingArea(), GetSizingMargin());
		}
		Rect GetWorkingArea() {
			return new Rect(PlacementAdorner.PlacementSurface.RenderSize);
		}
		Thickness GetSizingMargin() {
			return UseSizingMargin ? new Thickness(5, 5, 20, 30) : new Thickness();
		}
		Size EnsureAutoSize(Size size) {
			if(SizeToContent != System.Windows.SizeToContent.Manual) {
				Size autoSize = GetLayoutAutoSize();
				if(autoSize != Size.Empty) {
					Rect realBounds = PlacementAdorner.GetBoundsInContainer(ContentHolder);
					double w = autoSize.Width; double h = autoSize.Height;
					double realW = (realBounds.Width == 0) ? double.NaN : realBounds.Width;
					double realH = (realBounds.Height == 0) ? double.NaN : realBounds.Height;
					if(SizeToContent == System.Windows.SizeToContent.Width)
						w = MeasureAutoSize(size.Width, w, realW);
					if(SizeToContent == System.Windows.SizeToContent.Height)
						h = MeasureAutoSize(size.Height, h, realH);
					if(SizeToContent == System.Windows.SizeToContent.WidthAndHeight) {
						w = MeasureAutoSize(size.Width, w, realW);
						h = MeasureAutoSize(size.Height, h, realH);
					}
					size = new Size(w, h);
					if(FloatSize != size) {
						lockFloatingBoundsChanging++;
						if(FloatSize != new Size(0, 0))
							FloatSize = size;
						if(realW != w)
							realBounds.Width = w;
						if(realH != h)
							realBounds.Height = h;
						PlacementAdorner.SetBoundsInContainer(ContentHolder, realBounds);
						lockFloatingBoundsChanging--;
					}
				}
			}
			return size;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.DelayedExecute(() =>
			{
				UpdateStartupLocation();
			});
		}
		protected virtual void UpdateStartupLocation() {
			if(ContainerStartupLocation == WindowStartupLocation.Manual ||
				ContainerStartupLocation == WindowStartupLocation.CenterScreen)
				return;
			ActualSize = EnsureAutoSize(FloatSize);
			Point startupLocation = new Point((Owner.ActualWidth - ActualSize.Width) * 0.5, (Owner.ActualHeight - ActualSize.Height) * 0.5);
			UpdateFloatingBoundsCore(new Rect(startupLocation, FloatSize));
			if(FloatLocation != startupLocation)
				EnsureRelativeLocation(startupLocation);
		}
		protected override void OnIsOpenChanged(bool isOpen) {
			base.OnIsOpenChanged(isOpen);
			if(PlacementAdorner == null)
				return;
			PlacementAdorner.PlacementSurface.SizeChanged -= new SizeChangedEventHandler(OnPlacementAdornerSizeChanged);
			PlacementAdorner.KeyDown -= new KeyEventHandler(OnPlacementAdornerKeyDown);
			if(isOpen) {
				PlacementAdorner.KeyDown += new KeyEventHandler(OnPlacementAdornerKeyDown);
				PlacementAdorner.PlacementSurface.SizeChanged += new SizeChangedEventHandler(OnPlacementAdornerSizeChanged);
			}
		}
		protected override void UpdateIsOpenCore(bool isOpen) {
			if(isOpen && !PlacementAdorner.IsActivated)
				PlacementAdorner.Activate();
			if(isOpen) {
				isAutoSizeUpdating++;
				try {
					if(AllowShowAnimations) ContentHolder.Opacity = 0;
					Show();
				}
				finally {
					OnOpened();
					--isAutoSizeUpdating;
				}
			}
			else Hide();
		}
		protected void OnOpened() {
			UpdateStartupLocation();
			if(ContainerFocusable)
				PostFocus();
			EnsureMinSize();
			if(AllowShowAnimations) ContentHolder.BeginAnimation(OpacityProperty,
				new System.Windows.Media.Animation.DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(150))));
		}
		protected void PostFocus() {
			if(Content is FrameworkElement) {
				Dispatcher.BeginInvoke(new Action(PostFocusCore),
					System.Windows.Threading.DispatcherPriority.Loaded);
			}
		}
		void PostFocusCore() {
#if DXWINDOW
			FrameworkElement elementToFocus = LayoutHelper.FindElement(Content as FrameworkElement, (e) => e.Focusable);
#else
			FrameworkElement elementToFocus = Native.LayoutHelper.FindElement(Content as FrameworkElement, (e) => e.Focusable);
#endif
			if(elementToFocus != null)
				PostFocusCore(elementToFocus);
		}
		void PostFocusCore(FrameworkElement elementToFocus) {
			Keyboard.Focus(elementToFocus);
			elementToFocus.Focus();
		}
		protected void Show() {
			PlacementAdorner.SetVisible(ContentHolder, true);
		}
		protected void Hide() {
			PlacementAdorner.SetVisible(ContentHolder, false);
		}
		protected override void AddDecoratorToContentContainer(NonLogicalDecorator decorator) {
			ContentHolder.Child = decorator;
			PlacementAdorner.Register(ContentHolder);
			PlacementAdorner.SetVisible(ContentHolder, true);
		}
		protected virtual AdornerContentHolder CreateAdornerContentHolder() {
			return new AdornerContentHolder(this);
		}
		static PlacementAdorner FindPlacementAdorner(UIElement owner) {
			AdornerLayer layer = AdornerHelper.FindAdornerLayer(owner);
			Adorner[] adorners = new Adorner[] { };
			if(layer != null)
				adorners = layer.GetAdorners(owner);
			return (adorners != null) ? (PlacementAdorner)Array.Find(
					adorners, (adorner) => adorner is PlacementAdorner
				) : null;
		}
		void EnsureRelativeLocation(Point floatLocation) {
			lockFloatingBoundsChanging++;
			FloatLocation = floatLocation;
			lockFloatingBoundsChanging--;
		}
	}
}
