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

using DevExpress.Xpf.Core.Native;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
namespace DevExpress.XtraTests {
	[CLSCompliant(false)]
	public class WindowTester : WindowsTesterBase {
		Window window;
		WindowInteropHelper windowInteropHelper;
		public WindowTester(Window window) {
			if(window == null) throw new ArgumentNullException("window");
			this.window = window;
		}
		public Window Window { get { return window; } }
		static object ExitFrame(object f) {
			((DispatcherFrame)f).Continue = false;
			return null;
		}
		public override void DoEvents() {
			DispatcherFrame frame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
					new DispatcherOperationCallback(ExitFrame), frame);
			Dispatcher.PushFrame(frame);
		}
		protected override double GetScreenHeight() { return SystemParameters.PrimaryScreenHeight; }
		protected override double GetScreenWidth() { return SystemParameters.PrimaryScreenWidth; }
		protected override bool HasActiveControl { get { return false; }  }
		protected override IntPtr GetHandle() {  return WindowInteropHelper.Handle; }
		protected override object GetActiveControl() { return null; }
		public virtual void SendLMouseDown(FrameworkElement target, double clientX, double clientY) {
			Point ptScreen = ToScreenPoint(target, new Point(clientX, clientY));
			SendLMouseDown(ptScreen.X, ptScreen.Y);
		}
		public virtual void SendRMouseDown(FrameworkElement target, double clientX, double clientY) {
			Point ptScreen = ToScreenPoint(target, new Point(clientX, clientY));
			SendRMouseDown(ptScreen.X, ptScreen.Y);
		}
		public virtual void SendMouseMove(FrameworkElement target, double clientX, double clientY) {
			Point ptScreen = ToScreenPoint(target, new Point(clientX, clientY));
			SendMouseMove(ptScreen.X, ptScreen.Y);
		}
		public virtual void SendLMouseUp(FrameworkElement target, double clientX, double clientY) {
			Point ptScreen = ToScreenPoint(target, new Point(clientX, clientY));
			SendLMouseUp(ptScreen.X, ptScreen.Y);
		}
		public virtual void SendRMouseUp(FrameworkElement target, double clientX, double clientY) {
			Point ptScreen = ToScreenPoint(target, new Point(clientX, clientY));
			SendRMouseUp(ptScreen.X, ptScreen.Y);
		}
		public virtual void SendLMouseClick(FrameworkElement target, double clientX, double clientY) {
			SendLMouseClick(target, new Point(clientX, clientY));
		}
		public virtual void SendLMouseClick(FrameworkElement target, Point clientPt) {
			Point ptScreen = ToScreenPoint(target, clientPt);
			SendLMouseClick(ptScreen.X, ptScreen.Y);
		}
		public virtual void SendRMouseClick(FrameworkElement target, double clientX, double clientY) {
			SendRMouseClick(target, new Point(clientX, clientY));
		}
		public virtual void SendRMouseClick(FrameworkElement target, Point clientPt) {
			Point ptScreen = ToScreenPoint(target, clientPt);
			SendRMouseClick(ptScreen.X, ptScreen.Y);
		}
		public void MoveMousePointTo(FrameworkElement target, double clientX, double clientY) {
			MoveMousePointTo(target, new Point(clientX, clientY));
		}
		public void MoveMousePointTo(FrameworkElement target, Point clientPt) {
			Point ptScreen = ToScreenPoint(target, clientPt);
			MoveMousePointTo((int)ptScreen.X, (int)ptScreen.Y);
		}
		public void MoveMousePointToImmediately(FrameworkElement target, double clientX, double clientY) {
			MoveMousePointToImmediately(target, new Point(clientX, clientY));
		}
		public void MoveMousePointToImmediately(FrameworkElement target, Point clientPt) {
			Point ptScreen = ToScreenPoint(target, clientPt);
			InputHelper.SendMouseMove((int)ptScreen.X, (int)ptScreen.Y, 0);
		}
 	[System.Security.SecuritySafeCritical]
		public Point ToScreenPoint(FrameworkElement target, Point clientPt) {
			Point pt = LayoutHelper.GetRelativeElementRect(target, window).Location;
			pt.X += clientPt.X;
			pt.Y += clientPt.Y;
			TestImports.Win32Point win32Pt = new TestImports.Win32Point();
			TestImports.ClientToScreen(WindowInteropHelper.Handle, ref win32Pt);
			pt.X += win32Pt.X;
			pt.Y += win32Pt.Y;
			return pt;
		}
		protected WindowInteropHelper WindowInteropHelper {
			get {
				if(windowInteropHelper == null) {
					windowInteropHelper = new WindowInteropHelper(Window);
				}
				return windowInteropHelper;
			}
		}
	}
}
