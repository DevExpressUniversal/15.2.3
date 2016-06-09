#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Commands;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public class BrowserWindowTestControl : TestControlBase, ITestWindow, IControlReadOnlyText, IControlAct, IControlMouse {
		WebCommandAdapter commandAdapter;
		IEasyTestWebBrowser webBrowser;
		public BrowserWindowTestControl(IEasyTestWebBrowser webBrowser) {
			this.webBrowser = webBrowser;
		}
		public BrowserWindowTestControl(IEasyTestWebBrowser webBrowser, WebCommandAdapter commandAdapter)
			: this(webBrowser) {
			this.commandAdapter = commandAdapter;
		}
		#region ITestWindow Members
		[SecuritySafeCritical]
		private IntPtr GetActiveWindowHandleCore() {
			const string WindowClassName = "Internet Explorer_Server";
			IntPtr parent = new IntPtr(webBrowser.Browser.HWND);
			IntPtr child = new Win32Helper().GetChildWndByClassName(parent, WindowClassName);
			int mainWindowHandle = commandAdapter.WebAdapter.WebBrowsers[0].Browser.HWND;
			if(mainWindowHandle != webBrowser.Browser.HWND) {
				Win32Helper.SetForegroundWindow(new IntPtr(mainWindowHandle));
				Win32Helper.SetForegroundWindow(new IntPtr(webBrowser.Browser.HWND));
			}
			return child != IntPtr.Zero ? child : parent;
		}
		[SecuritySafeCritical]
		public IntPtr GetActiveWindowHandle() {
			try {
				commandAdapter.ExecuteScript("if(document.activeElement) { document.activeElement.blur(); }");
				Win32Helper.MoveMousePointTo(new Point(0, 0));
			}
			catch { }
			return GetActiveWindowHandleCore();
		}
		[SecuritySafeCritical]
		public void SetWindowSize(int width, int height) {
			int hwnd = -1;
			try {
				hwnd = webBrowser.Browser.HWND;
			}
			catch(COMException) {
				hwnd = Win32Helper.GetRootWindow(webBrowser.BrowserWindowHandle);
			}
			ImageHelper.SetWindowSize(new IntPtr(hwnd), width, height);
		}
		private void AddGetActivePopupControlScript() {
			commandAdapter.AddScript(@"
var activePopupControls = window.parent.ActivePopupControls;
if (activePopupControls && activePopupControls.length > 0) {
    return activePopupControls[activePopupControls.length - 1];
}
return null;", "GetActivePopupControl", "");
		}
		public string Caption {
			get {
				AddGetActivePopupControlScript();
				return (string)commandAdapter.ExecuteScript(@"
                    var popupControl = this.GetActivePopupControl();
                    if (popupControl != null && !popupControl.NewStyle){
                        return popupControl.GetHeaderText(); 
                    } else {
                        return document.title;
                    }");
			}
		}
		public void Close() {
			AddGetActivePopupControlScript();
			commandAdapter.ExecuteScript(@"
                    var popupControl = this.GetActivePopupControl();
                    if (popupControl != null){
                        popupControl.OnCloseButtonClick(-1); 
                    } else {
                        window.close();
                    }");
		}
		#endregion
		#region IControlReadOnlyText Members
		public string Text {
			get {
				if(commandAdapter != null) {
					ITestControl testControl = commandAdapter.CreateTestControl(TestControlType.Field, "ErrorInfo");
					return testControl.GetInterface<IControlReadOnlyText>().Text;
				}
				return null;
			}
		}
		#endregion
		#region IControlAct Members
		public void Act(string value) {
			if(value == "Refresh") {
				IntPtr activeHWnd = GetActiveWindowHandle();
				IntPtr ieFrameHWnd = Win32Helper.GetParentIEFrameWindow(activeHWnd);
				if(ieFrameHWnd != IntPtr.Zero) {
					Win32Helper.SetForegroundWindow(new IntPtr(webBrowser.Browser.HWND));
					System.Windows.Forms.SendKeys.SendWait("{F5}");
				}
			}
			else if(value == "Back") {
				webBrowser.Browser.GoBack();
			}
			else if(value == "Forward") {
				webBrowser.Browser.GoForward();
			}
			else if(value != null && value.StartsWith("http")) {
				webBrowser.Navigate(value);
			}
			else {
				throw new AdapterOperationException("The '" + value + "' browser action is not valid.");
			}
			commandAdapter.WaitForBrowserResponse(false);
		}
		#endregion
		#region IControlMouse Members
		public void MouseClick(int left, int top) {
			IntPtr windowHandle = GetActiveWindowHandleCore();
			Win32Helper.MouseClick(windowHandle, new Point(left, top), System.Windows.Forms.MouseButtons.Left);
			commandAdapter.WaitForBrowserResponse(false);
		}
		public void MouseMove(int left, int top) {
			IntPtr windowHandle = GetActiveWindowHandleCore();
			DevExpress.ExpressApp.EasyTest.WebAdapter.Utils.Win32Helper.POINT globalPoint = new DevExpress.ExpressApp.EasyTest.WebAdapter.Utils.Win32Helper.POINT(left, top);
			Win32Helper.MapWindowPoints(windowHandle, IntPtr.Zero, ref globalPoint, 1);
			Point point = new Point(globalPoint.X, globalPoint.Y);
			Win32Helper.MoveMousePointTo(point);
			commandAdapter.WaitForBrowserResponse(false);
		}
		#endregion
	}
}
