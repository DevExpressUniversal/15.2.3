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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Controls.Native;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
namespace DevExpress.Xpf.WindowsUI.Internal.Flyout {
	public abstract partial class FlyoutBase {
		private static void OnPromotedMouseButton(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left) {
				e.Handled = true;
			}
		}
		private static void OnPreviewKeyboardInputProviderAcquireFocus(object sender, KeyboardInputProviderAcquireFocusEventArgs e) {
			MenuFlyout menu = (MenuFlyout)sender;
			if(!menu.IsKeyboardFocusWithin && !menu.HasPushedMenuMode) {
				menu.PushMenuMode(true);
			}
		}
		private static void OnKeyboardInputProviderAcquireFocus(object sender, KeyboardInputProviderAcquireFocusEventArgs e) {
			MenuFlyout menu = (MenuFlyout)sender;
			if(!menu.IsKeyboardFocusWithin && !e.FocusAcquired && menu.IsAcquireFocusMenuMode) {
				AssertionException.IsTrue(menu.HasPushedMenuMode);
				menu.PopMenuMode();
			}
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			if(IsKeyboardFocusWithin) {
				if(!IsMenuMode) {
					IsMenuMode = true;
				}
			}
			else {
				if(IsMenuMode && HasCapture) {
					IsMenuMode = false;
				}
			}
			InvokeMenuOpenedClosedAutomationEvent(IsKeyboardFocusWithin);
		}
		private void InvokeMenuOpenedClosedAutomationEvent(bool open) {
			AutomationEvents automationEvent = open ? AutomationEvents.MenuOpened : AutomationEvents.MenuClosed;
			if(AutomationPeer.ListenerExists(automationEvent)) {
				AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(this);
				if(peer != null) {
					if(open) {
						Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate(object param) {
							peer.RaiseAutomationEvent(automationEvent);
							return null;
						}), null);
					}
					else {
						peer.RaiseAutomationEvent(automationEvent);
					}
				}
			}
		}
		private bool IsDescendant(DependencyObject node) {
			return IsDescendant(this, node);
		}
		internal static bool IsDescendant(DependencyObject reference, DependencyObject node) {
			bool success = false;
			DependencyObject curr = node;
			while(curr != null) {
				if(curr == reference) {
					success = true;
					break;
				}
				curr = VisualTreeHelper.GetParent(curr);
			}
			return success;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			Key key = e.Key;
			switch(key) {
				case Key.Escape: {
						KeyboardLeaveMenuMode();
						e.Handled = true;
						Service.Do(x => x.Hide(this));
					}
					break;
				case Key.System:
					if((e.SystemKey == Key.LeftAlt) ||
						(e.SystemKey == Key.RightAlt) ||
						(e.SystemKey == Key.F10)) {
						KeyboardLeaveMenuMode();
						e.Handled = true;
						Service.Do(x => x.Hide(this));
					}
					break;
			}
		}
		internal bool AllowMouseCapture { get; set; }
		protected virtual bool DoCapture(IInputElement element, CaptureMode mode) {
			if(AllowMouseCapture) {
				return Mouse.Capture(element, mode);
			}
			return true;
		}
		private static void OnLostMouseCapture(object sender, MouseEventArgs e) {
			MenuFlyout menu = sender as MenuFlyout;
			if(Mouse.Captured != menu) {
				if(e.OriginalSource == menu) {
					if(Mouse.Captured == null || !MenuFlyout.IsDescendant(menu, Mouse.Captured as DependencyObject)) {
						menu.IsMenuMode = false;
					}
				}
				else {
					if(MenuFlyout.IsDescendant(menu, e.OriginalSource as DependencyObject)) {
						if(menu.IsMenuMode && Mouse.Captured == null && Native.GetCapture() == IntPtr.Zero) {
							Mouse.Capture(menu, CaptureMode.SubTree);
							e.Handled = true;
						}
					}
					else {
						menu.IsMenuMode = false;
					}
				}
			}
		}
		[SecurityCritical]
		protected void RestorePreviousFocus() {
			if(IsKeyboardFocusWithin) {
				IntPtr hwndWithFocus = Native.GetFocus();
				HwndSource hwndSourceWithFocus = hwndWithFocus != IntPtr.Zero ? HwndSource.FromHwnd(hwndWithFocus) : null;
				if(hwndSourceWithFocus != null) {
					Keyboard.Focus(null);
				}
				else {
					Keyboard.ClearFocus();
				}
			}
		}
		internal virtual void KeyboardLeaveMenuMode() {
			if(IsMenuMode) {
				IsMenuMode = false;
			}
			else {
				RestorePreviousFocus();
			}
		}
		internal bool HasCapture {
			get { return Mouse.Captured == this; }
		}
		internal virtual void OnLeaveMenuMode() { }
		protected virtual void FocusOnOpen() {
			if(!IsKeyboardFocused && !IsKeyboardFocusWithin)
				Focus();
		}
		internal bool IsMenuMode {
			get {
				return _bitFlags[(int)MenuFlyoutFlags.IsMenuMode];
			}
			set {
				Debug.Assert(CheckAccess(), "IsMenuMode requires context access");
				bool isMenuMode = _bitFlags[(int)MenuFlyoutFlags.IsMenuMode];
				if(isMenuMode != value) {
					isMenuMode = _bitFlags[(int)MenuFlyoutFlags.IsMenuMode] = value;
					if(isMenuMode) {
						if(!IsDescendant(this, Mouse.Captured as Visual) && !this.DoCapture(this, CaptureMode.SubTree)) {
							isMenuMode = _bitFlags[(int)MenuFlyoutFlags.IsMenuMode] = false;
						}
						else {
							if(!HasPushedMenuMode) {
								PushMenuMode(false);
							}
							FocusOnOpen();
						}
					}
					if(!isMenuMode) {
						OnLeaveMenuMode();
						if(HasPushedMenuMode) {
							PopMenuMode();
						}
						if(HasCapture) {
							Mouse.Capture(null);
						}
						RestorePreviousFocus();
					}
				}
			}
		}
		[SecurityCritical]
		private void PushMenuMode(bool isAcquireFocusMenuMode) {
			AssertionException.IsNull(_pushedMenuMode);
			_pushedMenuMode = PresentationSource.FromVisual(this);
			AssertionException.IsNotNull(_pushedMenuMode);
			IsAcquireFocusMenuMode = isAcquireFocusMenuMode;
			InputManager.Current.PushMenuMode(_pushedMenuMode);
		}
		[SecurityCritical]
		private void PopMenuMode() {
			Debug.Assert(_pushedMenuMode != null);
			PresentationSource pushedMenuMode = _pushedMenuMode;
			_pushedMenuMode = null;
			IsAcquireFocusMenuMode = false;
			InputManager.Current.PopMenuMode(pushedMenuMode);
		}
		private bool HasPushedMenuMode {
			[SecurityCritical]
			get { return _pushedMenuMode != null; }
		}
		private bool IsAcquireFocusMenuMode {
			get { return _bitFlags[(int)MenuFlyoutFlags.IsAcquireFocusMenuMode]; }
			set { _bitFlags[(int)MenuFlyoutFlags.IsAcquireFocusMenuMode] = value; }
		}
		[SecurityCritical]
		private PresentationSource _pushedMenuMode;
		private BitVector32 _bitFlags = new BitVector32(0);
		private enum MenuFlyoutFlags {
			IsMenuMode = 0x04,
			IsAcquireFocusMenuMode = 0x10,
		}
	}
}
