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

using DevExpress.Xpf.Controls.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
namespace DevExpress.Xpf.WindowsUI {
	public class WinUIMessageBox : DXMessageBox {
		#region static
		static WinUIMessageBox() {
			var dProp = new DependencyPropertyRegistrator<WinUIMessageBox>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		public new static MessageBoxResult Show(string messageBoxText) {
			return Show(null, messageBoxText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public new static MessageBoxResult Show(string messageBoxText, string caption) {
			return Show(null, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public new static MessageBoxResult Show(FrameworkElement owner, string messageBoxText) {
			return Show(owner, messageBoxText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public new static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button) {
			return Show(null, messageBoxText, caption, button, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public new static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption) {
			return Show(owner, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public new static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon) {
			return Show(null, messageBoxText, caption, button, icon, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public new static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button) {
			return Show(owner, messageBoxText, caption, button, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public new static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult) {
			return Show(null, messageBoxText, caption, button, icon, defaultResult, MessageBoxOptions.None);
		}
		public new static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon) {
			return Show(owner, messageBoxText, caption, button, icon, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public new static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options) {
			return Show(null, messageBoxText, caption, button, icon, defaultResult, options);
		}
		public new static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult) {
			return Show(owner, messageBoxText, caption, button, icon, defaultResult, MessageBoxOptions.None);
		}
		public new static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options) {
			return Show(owner, messageBoxText, caption, button, icon, defaultResult, options, FloatingMode.Window);
		}
		public new static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options, FloatingMode desiredFloatingMode) {
			return ShowCore(WinUIMessageBoxCreator.Default, owner, messageBoxText, caption, button, icon, defaultResult, options, desiredFloatingMode, false, 11500);
		}
		public new static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options, FloatingMode desiredFloatingMode, bool allowShowAnimatoin) {
			return ShowCore(WinUIMessageBoxCreator.Default, owner, messageBoxText, caption, button, icon, defaultResult, options, desiredFloatingMode, false, 11500);
		}
		#endregion
		protected override void InitFloatingContainerCore(FrameworkElement owner, string messageBoxText, string caption, MessageBoxImage icon, MessageBoxButton button, FloatingMode desiredFloatingMode, bool showCloseButton, bool closeOnEscape, bool allowShowAnimation) {
			base.InitFloatingContainerCore(owner, messageBoxText, caption, icon, button, desiredFloatingMode, showCloseButton, closeOnEscape, allowShowAnimation);
			if(desiredFloatingMode == FloatingMode.Popup) return;
			fc.ContainerTemplate = null;
			fc.SizeToContent = SizeToContent.Manual;
			fc.ContainerStartupLocation = WindowStartupLocation.Manual;
			var ownerWindow = LayoutHelper.FindParentObject<Window>(owner) ?? Window.GetWindow(owner);
			bool hasOwnerWindow = ownerWindow != null && ownerWindow.IsLoaded;
			bool isMaximized = hasOwnerWindow && ownerWindow.WindowState == WindowState.Maximized;
			bool isMinimized = hasOwnerWindow && ownerWindow.WindowState == WindowState.Minimized;
			if(hasOwnerWindow) {
				if(!double.IsNaN(ownerWindow.Width) && !double.IsNaN(ownerWindow.Height))
					fc.FloatSize = new Size(ownerWindow.Width, ownerWindow.Height);
			}
			fc.FloatLocation = new Point(0, 0);
			if(desiredFloatingMode == FloatingMode.Window) {
				var fwc = fc as FloatingWindowContainer;
				if(fwc != null) {
					if(hasOwnerWindow && !isMinimized) {
						if(ownerWindow.WindowState == WindowState.Normal) {
							Point ownerOffset = owner.PointToScreen(new Point(0, 0));
							ownerOffset = ownerWindow.TransformToDeviceDPI(ownerOffset);
							fc.FloatLocation = new Point(ownerWindow.Left - ownerOffset.X, ownerWindow.Top - ownerOffset.Y);
						}
						if(isMaximized) {
							var workingArea = LayoutHelper.GetScreenRect(ownerWindow);
							Point ownerOffset = owner.PointToScreen(new Point(0, 0));
							ownerOffset = ownerWindow.TransformToDeviceDPI(ownerOffset);
							fc.FloatLocation = new Point(workingArea.Location.X - ownerOffset.X, workingArea.Location.Y - ownerOffset.Y);
							fc.FloatSize = workingArea.Size;
						}
					}
					else {
						fwc.Window.WindowState = WindowState.Maximized;
					}
				}
			}
			if(desiredFloatingMode == FloatingMode.Adorner) {
				var fac = fc as FloatingAdornerContainer;
				if(fac != null) fac.UseSizingMargin = false;
				if(isMaximized) {
					fc.FloatSize = new Size(ownerWindow.ActualWidth, ownerWindow.ActualHeight);
				}
			}
			this.Caption = caption;
		}
		protected override FloatingContainer CreateFloatingContainer(FloatingMode floatingMode) {
			switch(floatingMode) {
				case FloatingMode.Window:
					return new WinUIFloatingWindowContainer();
				default:
					return base.CreateFloatingContainer(floatingMode);
			}
		}
		#region WndProc
		private IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled) {
			switch(message) {
				case Native.WM_INITMENUPOPUP:
					if((lParam.ToInt32() / 65536) != 0) {
						IntPtr hMenu = Native.GetSystemMenu(hwnd, false);
						Int32 flags = Native.MF_DISABLED | Native.MF_GRAYED;
						int n = Native.GetMenuItemCount(hMenu);
						for(int i = 0; i < n - 1; i++) {
							Native.EnableMenuItem(hMenu, i, Native.MF_BYPOSITION | flags);
						}
						Native.DrawMenuBar(hwnd);
					}
					break;
				case Native.WM_SYSCOMMAND:
					int command = wParam.ToInt32() & 0xfff0;
					if(command == Native.SC_MOVE) {
						handled = true;
					}
					break;
			}
			return IntPtr.Zero;
		}
		#endregion
		class WinUIMessageBoxCreator : DXMessageBoxCreator {
			static WinUIMessageBoxCreator _Default;
			public static WinUIMessageBoxCreator Default {
				get {
					if(_Default == null) _Default = new WinUIMessageBoxCreator();
					return _Default;
				}
			}
			public override DXMessageBox Create() {
				return new WinUIMessageBox();
			}
		}
		class WinUIFloatingWindowContainer : FloatingWindowContainer {
			protected override WindowContentHolder CreateWindow() {
				return new WinUIMessageBoxContentHolder(this);
			}
		}
		class WinUIMessageBoxContentHolder : WindowContentHolder {
			public WinUIMessageBoxContentHolder(BaseFloatingContainer container)
				: base(container) {
			}
			protected override IntPtr HwndSourceHookHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
				switch(msg) {
					case 0x0024:
						Native.WmGetMinMaxInfo(hwnd, lParam);
						handled = true;
						return (System.IntPtr)0;
				}
				return base.HwndSourceHookHandler(hwnd, msg, wParam, lParam, ref handled);
			}
		}
	}
}
