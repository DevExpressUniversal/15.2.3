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
using System.Threading;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public interface IWebBrowserEvents {
		void InitWebBrowserEvents(object webBrowserInstance);
	}
	public interface IEasyTestWebBrowser {
		IWebBrowser2 Browser { get; }
		object Document { get; }
		void WaitForBrowserResponse();
		void Navigate(string url);
		IntPtr DialogWindowHandle { get; }
		int BrowserWindowHandle { get; }
		event EventHandler<NavigateErrorArgs> OnNavigateError;
	}
	public abstract class XAFWebBrowserBase : IEasyTestWebBrowser, IWebBrowserEvents {
		public static int InitialTimeout = 120000;
		public static int Timeout = 10000;
		private bool isQuit;
		private bool isDownloadComplete;
		private DateTime lastEventCallTime;
		protected IWebBrowserCollection owner;
		private AxHost.ConnectionPointCookie eventCookie;
		private WebBrowserEvent webBrowserEvent;
		public XAFWebBrowserBase(IWebBrowserCollection owner) {
			this.owner = owner;
		}
		#region IApplicationAdapterWebBrowser Members
		public abstract IWebBrowser2 Browser { get; }
		public object Document {
			get {
				if(isQuit) {
					throw new Exception("An attempt to get the document was made after quitting");
				}
				return Browser.Document;
			}
		}
		public virtual int BrowserWindowHandle { get { return Browser.HWND; } }
		public virtual IntPtr DialogWindowHandle {
			get {
				int browserHWND = -1;
				browserHWND = BrowserWindowHandle;
				IntPtr result = IntPtr.Zero;
				if(browserHWND != -1) {
					int popupWindowHwnd = DevExpress.ExpressApp.EasyTest.WebAdapter.Utils.Win32Helper.GetLastActivePopup(new IntPtr(browserHWND));
					if(popupWindowHwnd != browserHWND) {
						return new IntPtr(popupWindowHwnd);
					}
					int browserProcessId = 0;
					Win32Helper.GetWindowThreadProcessId(new IntPtr(browserHWND), out browserProcessId);
					if(browserProcessId != 0) {
						result = GetChildWndForProcess(new IntPtr(0), "#32770", browserProcessId);
					}
				}
				return result;
			}
		}
		public virtual IntPtr GetChildWndForProcess(IntPtr parent, string wndClassName, int processId) {
			Win32Helper win32Helper = new Win32Helper();
			return win32Helper.GetChildWndForProcess(parent, wndClassName, processId, -1);
		}
		public void WaitForBrowserResponse() {
			WaitForBrowserResponse(Timeout);
		}
		public void WaitForBrowserResponse(int timeout) {
			EasyTestTracer.Tracer.InProcedure("Browser.WaitForBrowserResponse()");
			while(true) {
				Application.DoEvents();
				if(isQuit)
					break;
				if(!Browser.Busy || !(DialogWindowHandle == IntPtr.Zero)) {
					break;
				}
				Thread.Sleep(200);
			}
			EasyTestTracer.Tracer.LogText("WaitBrowserEvents. Timeout = " + timeout);
			WebCommandAdapter.ExecuteTimeoutFunction(timeout, delegate(object obj) {
				Application.DoEvents();
				TimeSpan dif = DateTime.Now.Subtract(lastEventCallTime);
				if(((dif.TotalMilliseconds > 200)) || isQuit) {
					if(FileDownloadDialogHelper.FindFileDownloadDialog() != IntPtr.Zero) {
						return true;
					}
					if(isDownloadComplete) {
						return true;
					}
				}
				else {
					Thread.Sleep(30);
				}
				return false;
			});
			EasyTestTracer.Tracer.OutProcedure("Browser.WaitForBrowserResponse()");
		}
		public void Navigate(string url) {
			EasyTestTracer.Tracer.InProcedure(string.Format("Navigate({0})", url));
			Object EmptyParam = null;
			Browser.Navigate(url, ref EmptyParam, ref EmptyParam, ref EmptyParam, ref EmptyParam);
			WaitForBrowserResponse(InitialTimeout);
			EasyTestTracer.Tracer.OutProcedure(string.Format("Navigate({0})", url));
		}
		#endregion
		protected void webBrowserEvent_Quit(object sender, EventArgs e) {
			EasyTestTracer.Tracer.InProcedure("webBrowserEvent_Quit");
			owner.Remove(this);
			isQuit = true;
			EasyTestTracer.Tracer.OutProcedure("webBrowserEvent_Quit");
		}
		void webBrowserEvent_OnNewWindow2(object sender, NewWindow2EventArgs e) {
			e.NewBrowser = owner.CreateWebBrowser().Browser;
		}
		void webBrowserEvent_OnDownloadBegin(object sender, EventArgs e) {
			EasyTestTracer.Tracer.LogVerboseText("webBrowserEvent_OnDownloadBegin");
			lastEventCallTime = DateTime.Now;
			isDownloadComplete = false;
		}
		void webBrowserEvent_OnDownloadComplete(object sender, EventArgs e) {
			EasyTestTracer.Tracer.LogVerboseText("webBrowserEvent_OnDownloadComplete");
			isDownloadComplete = true;
			lastEventCallTime = DateTime.Now;
		}
		void webBrowserEvent_OnDocumentComplete(object sender, DocumentCompleteArgs e) {
			EasyTestTracer.Tracer.LogVerboseText(string.Format("webBrowserEvent_OnDocumentComplete('{0}')", e.URL));
			if(e.URL.Contains("http")) {
				lastEventCallTime = DateTime.Now;
			}
		}
		void webBrowserEvent_BeforeNavigate(object sender, EventArgs e) {
			EasyTestTracer.Tracer.LogVerboseText("webBrowserEvent_BeforeNavigate");
		}
		void webBrowserEvent_OnNavigateError(object sender, NavigateErrorArgs e) {
			EasyTestTracer.Tracer.LogVerboseText(string.Format("NavigateError url: {0}, statusCode:{1}", e.Url, e.StatusCode));
			if(OnNavigateError != null) {
				OnNavigateError(this, e);
			}
		}
		#region IWebBrowserEvents
		void IWebBrowserEvents.InitWebBrowserEvents(object webBrowserInstance) {
			webBrowserEvent = new WebBrowserEvent();
			eventCookie = new AxHost.ConnectionPointCookie(webBrowserInstance, webBrowserEvent, typeof(DWebBrowserEvents2));
			webBrowserEvent.OnNewWindow2 += new EventHandler<NewWindow2EventArgs>(webBrowserEvent_OnNewWindow2);
			webBrowserEvent.OnDownloadBegin += new EventHandler(webBrowserEvent_OnDownloadBegin);
			webBrowserEvent.OnDownloadComplete += new EventHandler(webBrowserEvent_OnDownloadComplete);
			webBrowserEvent.OnDocumentComplete += new EventHandler<DocumentCompleteArgs>(webBrowserEvent_OnDocumentComplete);
			webBrowserEvent.BeforeNavigate += new EventHandler(webBrowserEvent_BeforeNavigate);
			webBrowserEvent.Quit += new EventHandler(webBrowserEvent_Quit);
			webBrowserEvent.OnNavigateError += new EventHandler<NavigateErrorArgs>(webBrowserEvent_OnNavigateError);
		}
		#endregion
		public event EventHandler<NavigateErrorArgs> OnNavigateError;
	}
}
