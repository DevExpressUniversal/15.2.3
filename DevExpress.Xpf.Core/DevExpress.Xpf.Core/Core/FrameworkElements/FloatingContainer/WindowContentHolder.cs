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
using System.Security;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Input;
#if DXWINDOW
using LH = DevExpress.Internal.DXWindow.LayoutHelper;
namespace DevExpress.Internal.DXWindow {
#else
using LH = DevExpress.Xpf.Core.Native.LayoutHelper;
namespace DevExpress.Xpf.Core {
#endif
	public class WindowContentHolder : Window {
		internal static bool SetHwndSourceOwner(Window window, FrameworkElement owner) {
			HwndSource hwndSource = PresentationSource.FromDependencyObject(owner) as HwndSource;
			if(hwndSource != null) {
				new WindowInteropHelper(window) { Owner = hwndSource.Handle };
				return true;
			}
			return false;
		}
		public BaseFloatingContainer Container { get; private set; }
		internal bool UseScreenCoordinates { get; set; }
		public WindowContentHolder(BaseFloatingContainer container) {
			Container = container;
			if(container is FloatingContainer && ((FloatingContainer)container).Caption != null) Title = ((FloatingContainer)container).Caption;
			ShowInTaskbar = false;
			AllowsTransparency = true;
			WindowStyle = WindowStyle.None;
			ResizeMode = ResizeMode.NoResize;
			Background = Brushes.Transparent;
			WindowStartupLocation = WindowStartupLocation.Manual;
			SetValue(FloatingContainer.IsActiveProperty, true);
			SetValue(FloatingContainer.IsMaximizedProperty, false);
			DependencyProperty property = typeof(KeyboardNavigation).GetField("ShowKeyboardCuesProperty", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField).GetValue(null) as DependencyProperty;
			var binding = new Binding() { Path = new PropertyPath(property), Source = this };
			FrameworkElement dObject = container.Content as FrameworkElement;
			if(dObject != null) dObject.SetBinding(property, binding);
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			if(!Container.UseActiveStateOnly)
				SetValue(FloatingContainer.IsActiveProperty, e.NewValue);
		}
		protected WindowInteropHelper interopHelperCore;
		protected override void OnSourceInitialized(System.EventArgs e) {
			base.OnSourceInitialized(e);
			if(interopHelperCore == null) interopHelperCore = new WindowInteropHelper(this);
			HwndSource.FromHwnd(interopHelperCore.Handle).AddHook(HwndSourceHookHandler);
			if(WindowStartupLocation == WindowStartupLocation.Manual) return;
			FrameworkElement owner = Container.Owner;
			if(owner != null) {
				Point relativeLocation = owner.PointFromScreen(new Point(Left, Top));
				EnsureRelativeLocation(relativeLocation);
			}
		}
		[SecuritySafeCritical]
		protected virtual IntPtr HwndSourceHookHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if(msg == NativeMethods.WM_NCACTIVATE) {
				if(Container != null && Container.GetValue(FloatingContainer.TagProperty) != null) {
					DependencyObject obj = Container.GetValue(FloatingContainer.TagProperty) as DependencyObject;
					if(obj != null) obj.SetValue(FloatingContainer.IsActiveProperty, (int)wParam > 0);
				}
				return IntPtr.Zero;
			}
			return IntPtr.Zero;
		}
		int lockFloatingBoundsChanging = 0;
		public void SetFloatingBounds(FrameworkElement owner, Rect bounds) {
			if(lockFloatingBoundsChanging > 0) return;
			lockFloatingBoundsChanging++;
			try {
				TrySetOwner(owner);
				TryCorrectBoundsAsync(owner, bounds);
			}
			finally { lockFloatingBoundsChanging--; }
		}
		protected virtual void TryCorrectBoundsAsync(FrameworkElement owner, Rect bounds) {
			CorrectBoundsCore(owner, bounds);
		}
		protected void CorrectBoundsCore(FrameworkElement owner, Rect bounds) {
			if(bounds.Size.IsEmpty || (bounds.Width == 0 && bounds.Height == 0)) return;
			bounds = CorrectBounds(owner, bounds, UseScreenCoordinates);
			SetBounds(bounds);
		}
		protected virtual void SetBounds(Rect bounds) {
			Left = bounds.Left;
			Top = bounds.Top;
			Width = bounds.Width;
			Height = bounds.Height;
		}
		public void SetStartupLocation(FrameworkElement owner, WindowStartupLocation location) {
			TrySetOwner(owner);
			WindowStartupLocation = location;
		}
		internal static Rect CorrectBounds(FrameworkElement owner, Rect bounds, bool useScreenCoordinates) {
			if(owner != null) {
				try {
					PresentationSource pSource = PresentationSource.FromDependencyObject(owner);
					Point location = bounds.Location;
					if(!useScreenCoordinates && pSource != null)
						location = owner.PointToScreen(bounds.Location);
					if(pSource != null) {
						bounds = new Rect(
								pSource.CompositionTarget.TransformFromDevice.Transform(location), bounds.Size
							);
					}
					else bounds = new Rect(location, bounds.Size);
				}
				catch { }
			}
			return bounds;
		}
		bool? IsNonWPFHosted;
		protected virtual void TrySetOwnerCore(Window containerWindow) {
			Owner = containerWindow;
		}
		void TrySetOwner(FrameworkElement owner) {
			if(Owner != null || owner == null || Owner == owner || (IsNonWPFHosted.HasValue && IsNonWPFHosted.Value)) return;
			Window containerWindow = LH.FindParentObject<Window>(owner);
			if(containerWindow == null) {
				if(IsNonWPFHosted == null) {
					if(SetHwndSourceOwner(this, owner)) {
						IsNonWPFHosted = true;
					}
				}
			}
			else {
				if(containerWindow.IsVisible) {
					TrySetOwnerCore(containerWindow);
					Subscribe(Owner);
				}
			}
		}
		protected virtual void Subscribe(Window ownerWindow) {
			if(ownerWindow == null) return;
			ownerWindow.LocationChanged += OwnerWindow_LocationChanged;
			ownerWindow.SizeChanged += OwnerWindow_SizeChanged;
		}
		protected virtual void UnSubscribe(Window ownerWindow) {
			if(ownerWindow == null) return;
			ownerWindow.LocationChanged -= OwnerWindow_LocationChanged;
			ownerWindow.SizeChanged -= OwnerWindow_SizeChanged;
		}
		void OwnerWindow_SizeChanged(object sender, SizeChangedEventArgs e) {
			TryCheckRelativeLocationAsync(sender);
		}
		void OwnerWindow_LocationChanged(object sender, System.EventArgs e) {
			TryCheckRelativeLocationAsync(sender);
		}
		protected virtual void TryCheckRelativeLocationAsync(object sender) {
			CheckRelativeLocation();
		}
		protected virtual Point GetRelativeLocation(FrameworkElement owner) {
			return owner.PointFromScreen(new Point(Left, Top));
		}
		protected internal void CheckRelativeLocation() {
			FrameworkElement owner = Container.Owner;
			if(owner != null) {
#if DXWINDOW
				if(ScreenHelper.IsAttachedToPresentationSource(owner)) {
#else
				if(DevExpress.Xpf.Core.Native.ScreenHelper.IsAttachedToPresentationSource(owner)) {
#endif
					Point relativeLocation = GetRelativeLocation(owner);
					if(relativeLocation != Container.FloatLocation)
						EnsureRelativeLocation(relativeLocation);
				}
				else UnSubscribe(Owner);
			}
		}
		protected override void OnClosed(System.EventArgs e) {
			UnSubscribe(Owner);
			base.OnClosed(e);
		}
		void EnsureRelativeLocation(Point floatLocation) {
			lockFloatingBoundsChanging++;
			EnsureRelativeLocationCore(floatLocation);
			lockFloatingBoundsChanging--;
		}
		protected virtual void EnsureRelativeLocationCore(Point floatLocation) {
			Container.FloatLocation = floatLocation;
		}
	}
}
