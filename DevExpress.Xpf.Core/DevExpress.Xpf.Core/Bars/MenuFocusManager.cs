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
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Bars {
	public static class MenuFocusManager {
		public static readonly DependencyProperty ChildWindowRetainsFocusOnParentBarItemClickProperty = DependencyProperty.RegisterAttached("ChildWindowRetainsFocusOnParentBarItemClick", typeof(bool), typeof(MenuFocusManager),
			new PropertyMetadata(false, OnChildWindowRetainsFocusOnParentBarItemClickChanged));
		public static bool GetChildWindowRetainsFocusOnParentBarItemClick(DependencyObject target) {
			return (bool)target.GetValue(ChildWindowRetainsFocusOnParentBarItemClickProperty);
		}
		public static void SetChildWindowRetainsFocusOnParentBarItemClick(DependencyObject target, bool value) {
			target.SetValue(ChildWindowRetainsFocusOnParentBarItemClickProperty, value);
		}
		static void OnChildWindowRetainsFocusOnParentBarItemClickChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			OnChildWindowRetainsFocusOnParentBarItemClickChanged(dObj, (bool)e.OldValue, (bool)e.NewValue);
		}
		static void OnChildWindowRetainsFocusOnParentBarItemClickChanged(DependencyObject dObj, bool oldValue, bool newValue) {
			if(newValue)
				Subscribe(dObj);
			else
				Unsubscribe(dObj);
		}
		static Dictionary<object, int> lockHash = new Dictionary<object, int>();
		static object syncObj = new object();
		static void RemoveHookCore(Window ownerWindow) {
			var hook = new HwndSourceHook(FilterMessage);
			var source = HwndSource.FromVisual(ownerWindow) as HwndSource;
			if(source != null) source.RemoveHook(hook);
		}
		static void AddHookCore(Window ownerWindow) {
			var hook = new HwndSourceHook(FilterMessage);
			var source = HwndSource.FromVisual(ownerWindow) as HwndSource;
			source.AddHook(hook);
		}
		static void Unsubscribe(DependencyObject owner) {
			Window ownerWindow = owner as Window ?? Window.GetWindow(owner);
			lock (syncObj) {
				if(--lockHash[ownerWindow] == 0) {
					ownerWindow.Loaded -= new RoutedEventHandler(ownerWindow_Loaded);
					ownerWindow.Closed -= OwnerWindow_Closed;
					RemoveHookCore(ownerWindow);
					lockHash.Remove(ownerWindow);
				}
			}
		}
		static void Subscribe(DependencyObject owner) {
			Window ownerWindow = owner as Window ?? Window.GetWindow(owner);
			if(ownerWindow == null) return;
			lock(syncObj) {
				int value;
				if(!lockHash.TryGetValue(ownerWindow, out value)) {
					lockHash.Add(ownerWindow, 0);
					if(ScreenHelper.IsAttachedToPresentationSource(ownerWindow))
						AddHookCore(ownerWindow);
					else
						ownerWindow.Loaded += new RoutedEventHandler(ownerWindow_Loaded);
					ownerWindow.Closed += OwnerWindow_Closed;
				}
				++lockHash[ownerWindow];
			}
		}
		static void OwnerWindow_Closed(object sender, EventArgs e) {
			Window ownerWindow = sender as Window;
			Unsubscribe(ownerWindow);
		}
		static void ownerWindow_Loaded(object sender, RoutedEventArgs e) {
			Window ownerWindow = sender as Window;
			ownerWindow.Loaded -= new RoutedEventHandler(ownerWindow_Loaded);
			AddHookCore(ownerWindow);
		}
		static IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			const int WM_MOUSEACTIVATE = 0x0021;
			const int MA_NOACTIVATE = 3;
			switch(msg) {
				case WM_MOUSEACTIVATE:
					if(CheckIsMenuActivated(hwnd)) {
						handled = true;
						return new IntPtr(MA_NOACTIVATE);
					}
					break;
			}
			return IntPtr.Zero;
		}
		#region Hit Testing
		static bool isMenuActivated;
		static bool CheckIsMenuActivated(IntPtr hWnd) {
			Window ownerWindow = HwndSource.FromHwnd(hWnd).RootVisual as Window;
			if(ownerWindow == null) return false;
			isMenuActivated = false;
			Point hitPoint = Mouse.GetPosition(ownerWindow);
			VisualTreeHelper.HitTest(ownerWindow, null, HitTestResult, new PointHitTestParameters(hitPoint));
			return isMenuActivated;
		}
		static HitTestResultBehavior HitTestResult(HitTestResult result) {
			DependencyObject visualHit = result.VisualHit;
			BarItemLinkControl barItem = GetVisualParent<BarItemLinkControl>(visualHit);
			if(barItem != null) {
				isMenuActivated = true;
				return HitTestResultBehavior.Stop;
			}
			return HitTestResultBehavior.Continue;
		}
		static T GetVisualParent<T>(object childObject) where T : Visual {
			DependencyObject child = childObject as DependencyObject;
			while((child != null) && !(child is T)) {
				child = VisualTreeHelper.GetParent(child);
			}
			return child as T;
		}
		#endregion
	}  
}
