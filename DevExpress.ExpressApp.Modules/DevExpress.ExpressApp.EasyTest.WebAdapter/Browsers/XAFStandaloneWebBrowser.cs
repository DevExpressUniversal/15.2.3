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
using System.Diagnostics;
using System.Runtime.InteropServices;
using DevExpress.EasyTest.Framework;
using System.Windows.Forms;
using System.Threading;
using mshtml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Permissions;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public class StandaloneWebBrowserControl : WebBrowser {
		enum WindowsMessages {
			WM_PARENTNOTIFY = 0x210,
			WM_DESTROY = 0x2,
		};
		private IWebBrowserEvents webBrowserEvents;
		protected virtual void OnQuit() {
			if(Quit != null) {
				Quit(this, EventArgs.Empty);
			}
		}
		[System.Security.Permissions.PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void WndProc(ref Message m) {
			if(m.Msg == (int)WindowsMessages.WM_PARENTNOTIFY) {
				int wp = m.WParam.ToInt32();
				int X = wp & 0xFFFF;
				if(X == (int)WindowsMessages.WM_DESTROY)
					this.OnQuit();
			}
			base.WndProc(ref m);
		}
		public StandaloneWebBrowserControl(IWebBrowserEvents webBrowserEvents) {
			this.webBrowserEvents = webBrowserEvents;
			ScriptErrorsSuppressed = true;
		}
		protected override void CreateSink() {
			object activeXInstance = base.ActiveXInstance;
			if(activeXInstance != null) {
				webBrowserEvents.InitWebBrowserEvents(activeXInstance);
			}
		}
		public EventHandler<EventArgs> Quit { get; set; }
	}
	public class XAFStandaloneWebBrowser : XAFWebBrowserBase {
		private StandaloneWebBrowserControl webBrowser;
		public XAFStandaloneWebBrowser(StandaloneWebBrowserCollection owner)
			: base(owner) {
			webBrowser = new StandaloneWebBrowserControl(this);
			webBrowser.Quit += new EventHandler<EventArgs>(
				base.webBrowserEvent_Quit
				);
		}
		#region IApplicationAdapterWebBrowser Members
		public override IWebBrowser2 Browser {
			get { return (IWebBrowser2)webBrowser.ActiveXInstance; }
		}
		#endregion
		public WebBrowser WebBrowser { get { return webBrowser; } }
		public override int BrowserWindowHandle {
			get {
				int result = -1;
				webBrowser.Invoke(new ThreadStart(delegate() {
					result = (int)webBrowser.Handle;
				}));
				return result;
			}
		}
		public override IntPtr GetChildWndForProcess(IntPtr parent, string wndClassName, int processId) {
			Win32Helper win32Helper = new Win32Helper();
			return win32Helper.GetChildWndForProcess(parent, wndClassName, -1, processId);
		}
	}
	public class StandaloneWebBrowserCollection : IDisposable, IWebBrowserCollection {
		private bool isApplicationStarted = false;
		private bool isCollectionModified = false;
		private List<XAFStandaloneWebBrowser> browsers = new List<XAFStandaloneWebBrowser>();
		#region IWebBrowserCollection Members
		public IEnumerator GetEnumerator() {
			return browsers.GetEnumerator();
		}
		public IEasyTestWebBrowser this[int index] {
			get { return browsers[index]; }
		}
		private XAFStandaloneWebBrowser CreateFormPlacedBrowser() {
			XAFStandaloneWebBrowser result;
			Form form = new Form();
			form.Width = 800;
			form.Height = 600;
			result = new XAFStandaloneWebBrowser(this);
			result.WebBrowser.Dock = DockStyle.Fill;
			form.Controls.Add(result.WebBrowser);
			form.Show();
			return result;
		}
		Form mainForm;
		public IEasyTestWebBrowser CreateWebBrowser() {
			XAFStandaloneWebBrowser result = null;
			if(isApplicationStarted) {
				mainForm.Invoke(new ThreadStart(delegate() {
					result = CreateFormPlacedBrowser();
				}));
			}
			else {
				AutoResetEvent createControlevent = new AutoResetEvent(false);
				Thread thread = new Thread(new ThreadStart(delegate() {
					result = CreateFormPlacedBrowser();
					if(!isApplicationStarted) {
						isApplicationStarted = true;
						mainForm = result.WebBrowser.Parent as Form;
						createControlevent.Set();
						Application.Run(mainForm);
					}
				}));
				thread.Name = "Application";
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
				createControlevent.WaitOne();
			}
			Add(result);
			return result;
		}
		public void Add(IEasyTestWebBrowser browser) {
			browsers.Add(browser as XAFStandaloneWebBrowser);
			isCollectionModified = true;
		}
		public void Remove(IEasyTestWebBrowser browser) {
			mainForm.Invoke(new ThreadStart(delegate() {
				((Form)((XAFStandaloneWebBrowser)browser).WebBrowser.Parent).Close();
			}));
			browsers.Remove(browser as XAFStandaloneWebBrowser);
			isCollectionModified = true;
		}
		public int Count {
			get { return browsers.Count; }
		}
		public void Clear() {
			if(browsers.Count > 0) {
				Form browserForm = ((Form)browsers[0].WebBrowser.Parent);
				browserForm.Invoke(new ThreadStart(delegate() {
					browserForm.Close();
				}));
			}
			browsers.Clear();
		}
		public void KillAllWebBrowsers() {
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
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Clear();
		}
		#endregion
	}
}
