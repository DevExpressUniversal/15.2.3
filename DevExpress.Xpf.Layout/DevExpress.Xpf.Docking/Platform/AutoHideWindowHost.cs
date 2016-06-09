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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Platform.Win32;
using DevExpress.Xpf.Docking.VisualElements;
namespace DevExpress.Xpf.Docking.Platform {
	[ContentProperty("Content")]
	public class AutoHideWindowHost : HwndHost {
		#region static
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty BackgroundProperty;
		static AutoHideWindowHost() {
			var dProp = new DependencyPropertyRegistrator<AutoHideWindowHost>();
			dProp.Register("Content", ref ContentProperty, (object)null,
				(d, e) => ((AutoHideWindowHost)d).OnContentChanged(e.OldValue, e.NewValue));
			dProp.Register("Background", ref BackgroundProperty, (Brush)null,
				(d, e) => ((AutoHideWindowHost)d).OnBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue));
		}
		#endregion
		public AutoHideWindowHost() {
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			var autoHidePane = LayoutHelper.FindParentObject<AutoHidePane>(this);
			if(autoHidePane != null) {
				WindowRoot.Pane = autoHidePane;
				bool fHorz = (AutoHideTray.GetOrientation(autoHidePane) == Orientation.Vertical);
				BindingHelper.SetBinding(this, fHorz ? WidthProperty : HeightProperty, autoHidePane, "PanelSize");
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ClearValue(ContentProperty);
			}
			base.Dispose(disposing);
		}
		protected virtual void OnContentChanged(object oldValue, object newValue) {
			if(oldValue is UIElement)
				WindowRoot.Child = null;
			if(newValue is UIElement) {
				WindowRoot.Child = (UIElement)newValue;
				DockLayoutManager.SetDockLayoutManager(WindowRoot, DockLayoutManager.GetDockLayoutManager(this));
			}
		}
		protected override System.Runtime.InteropServices.HandleRef BuildWindowCore(System.Runtime.InteropServices.HandleRef hwndParent) {
			const int windowStyle = 0x40000000 | 0x04000000 | 0x02000000;
			hwndHost = new HwndSource(new HwndSourceParameters()
			{
				ParentWindow = hwndParent.Handle,
				WindowStyle = windowStyle,
				Width = 1,
				Height = 1,
			});
			hwndHost.RootVisual = WindowRoot;
			hwndHost.ContentRendered += OnContentRendered;
			if(Background is SolidColorBrush)
				hwndHost.CompositionTarget.BackgroundColor = ((SolidColorBrush)Background).Color;
			return new HandleRef(this, hwndHost.Handle);
		}
		bool isContentRendered;
		void OnContentRendered(object sender, EventArgs e) {
			isContentRendered = true;
		}
		protected virtual void OnBackgroundChanged(Brush oldValue, Brush newValue) {
			if(hwndHost != null && Background is SolidColorBrush)
				hwndHost.CompositionTarget.BackgroundColor = ((SolidColorBrush)Background).Color;
		}
		protected override void DestroyWindowCore(System.Runtime.InteropServices.HandleRef hwnd) {
			if(hwndHost != null) {
				hwndHost.Dispose();
				hwndHost = null;
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			WindowRoot.Measure(constraint);
			return WindowRoot.DesiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			WindowRoot.Arrange(new Rect(finalSize));
			return finalSize;
		}
		Locker windowPosLocker = new Locker();
		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if(msg == (int)WM.WM_WINDOWPOSCHANGING) {
				const int flags = 0x0001 | 0x0002;
				if(isContentRendered && !windowPosLocker.IsLocked) {
					windowPosLocker.Lock();
					try {
						NativeHelper.SetWindowPosSafe(hwndHost.Handle, IntPtr.Zero, 0, 0, 0, 0, flags);
					}
					finally {
						windowPosLocker.Unlock();
					}
				}
			}
			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}
		protected override bool HasFocusWithinCore() {
			return false;
		}
		protected override System.Windows.Media.HitTestResult HitTestCore(System.Windows.Media.PointHitTestParameters hitTestParameters) {
			return WindowRoot.Hit(hitTestParameters);
		}
		protected override System.Windows.Media.GeometryHitTestResult HitTestCore(System.Windows.Media.GeometryHitTestParameters hitTestParameters) {
			return base.HitTestCore(hitTestParameters);			
		}
		HwndSource hwndHost = null;
		AutoHideWindowRoot _windowRoot;
		AutoHideWindowRoot WindowRoot {
			get {
				if(_windowRoot == null) {
					_windowRoot = new AutoHideWindowRoot() { Host = this };
					System.Windows.Input.KeyboardNavigation.SetTabNavigation(_windowRoot, System.Windows.Input.KeyboardNavigationMode.Cycle);
					AddLogicalChild(_windowRoot);
				}
				return _windowRoot;
			}
		}
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}
		#region internal
		public class AutoHideWindowRoot : Decorator {
			public AutoHidePane Pane { get; set; }
			public System.Windows.Media.HitTestResult Hit(System.Windows.Media.PointHitTestParameters hitTestParameters) {
				return HitTestHelper.GetHitResult(this, hitTestParameters.HitPoint);
			}
			public AutoHideWindowHost Host { get; set; }
		}
		#endregion
	}
}
