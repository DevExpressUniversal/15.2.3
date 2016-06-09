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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public interface IWebBrowserCollection : IDisposable {
		IEnumerator GetEnumerator();
		IEasyTestWebBrowser this[int index] { get; }
		void Add(IEasyTestWebBrowser browser);
		IEasyTestWebBrowser CreateWebBrowser();
		void Remove(IEasyTestWebBrowser browser);
		int Count { get; }
		void Clear();
		void KillAllWebBrowsers();
		void WaitForAllWebBrowsersResponse();
	}
	public class WebBrowserCollection : IWebBrowserCollection {
		private readonly object lockObject = new object();
		private static Size defaultFormSize = new Size(1024, 768);
		public static Size DefaultFormSize {
			get { return defaultFormSize; }
			set {
				defaultFormSize = value;
			}
		}
		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(int hWnd, ref IntPtr lpdwProcessId);
		private List<XAFWebBrowser> browsers = new List<XAFWebBrowser>();
		private List<int> handles = new List<int>();
		private bool isCollectionModified;
		private List<Process> GetIeProcesses() {
			List<Process> ieProcesses = new List<Process>();
			List<int> localHandles;
			lock(lockObject) {
				localHandles = new List<int>(handles);
			}
			foreach(int hwnd in localHandles) {
				try {
					IntPtr processId = IntPtr.Zero;
					GetWindowThreadProcessId(hwnd, ref processId);
					if(processId != IntPtr.Zero) {
						Process ieProcess = Process.GetProcessById((int)processId);
						if(ieProcess != null) {
							ieProcesses.Add(ieProcess);
						}
					}
				}
				catch { }
			}
			return ieProcesses;
		}
		public WebBrowserCollection() {
		}
		public IEnumerator GetEnumerator() {
			return browsers.GetEnumerator();
		}
		public IEasyTestWebBrowser this[int index] {
			get {
				return (XAFWebBrowser)browsers[index];
			}
		}
		public void Add(IEasyTestWebBrowser browser) {
			browsers.Add(browser as XAFWebBrowser);
			ITestWindow window = new BrowserWindowTestControl(browser);
			window.SetWindowSize(DefaultFormSize.Width, DefaultFormSize.Height);
			lock(lockObject) {
				handles.Add(browser.Browser.HWND);
			}
			isCollectionModified = true;
		}
		public IEasyTestWebBrowser CreateWebBrowser() {
			XAFWebBrowser xafWebBrowser = new XAFWebBrowser(this);
			Add(xafWebBrowser);
			return xafWebBrowser;
		}
		public void Remove(IEasyTestWebBrowser browser) {
			browsers.Remove(browser as XAFWebBrowser);
			isCollectionModified = true;
		}
		public int Count {
			get { return browsers.Count; }
		}
		public void Clear() {
			browsers.Clear();
			lock(lockObject) {
				handles.Clear();
			}
		}
		public void KillAllWebBrowsers() {
			List<Process> ieProcesses = GetIeProcesses();
			foreach(Process process in ieProcesses) {
				try {
					process.Kill();
					Thread.Sleep(1);
				}
				catch { }
			}
			Clear();
		}
		public void WaitForAllWebBrowsersResponse() {
			for(int i = 0; i < browsers.Count; i++) {
				IEasyTestWebBrowser browser = browsers[i];
				browser.WaitForBrowserResponse();
			}
			if(isCollectionModified) {
				isCollectionModified = false;
				WaitForAllWebBrowsersResponse();
			}
		}
		#region IDisposable Members
		public void Dispose() {
			Clear();
		}
		#endregion
	}
}
