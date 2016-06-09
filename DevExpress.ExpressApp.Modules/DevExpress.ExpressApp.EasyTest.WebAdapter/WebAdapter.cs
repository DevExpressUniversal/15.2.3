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

using mshtml;
using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.EasyTest.Framework;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Commands;
using DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public class WebAdapter : IApplicationAdapter, ICommandsRegistrator {
		private class User32 {
			[DllImport("user32.dll")]
			public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rect);
		}
		private const string UrlParamName = "Url";
		public const string ApplicationViewExtension = "html";
		private const string SingleWebDevParamName = "SingleWebDev";
		private void CheckWebDevProcesses() {
			EasyTestTracer.Tracer.InProcedure(string.Format("CheckWebDevProcesses"));
			string[] webServerNames = new string[] { "WebDev.WebServer", "WebDev.WebServer20", "WebDev.WebServer40", "iisexpress" };
			EasyTestTracer.Tracer.LogText("WebDev processes:");
			foreach(string name in webServerNames) {
				Process[] otherWebDevWebServerProcesses = Process.GetProcessesByName(name);
				if(otherWebDevWebServerProcesses.Length > 0) {
					for(int i = 0; i < otherWebDevWebServerProcesses.Length; i++) {
						otherWebDevWebServerProcesses[i].Refresh();
						string responding = "Responding: " + otherWebDevWebServerProcesses[i].Responding.ToString();
						EasyTestTracer.Tracer.LogText(otherWebDevWebServerProcesses[i].ProcessName + " " + responding);
					}
				}
			}
			EasyTestTracer.Tracer.OutProcedure(string.Format("CheckWebDevProcesses"));
		}
		private void CreateBrowser(string url) {
			EasyTestTracer.Tracer.InProcedure(string.Format("CreateBrowser({0})", url));
			if(!CreateBrowserCore(url, true)) {
				KillAllProcessForName("iexplore");
				CreateBrowserCore(url, false);
			}
			EasyTestTracer.Tracer.OutProcedure(string.Format("CreateBrowser({0})", url));
		}
		private bool CreateBrowserCore(string url, bool catchException) {
			EasyTestTracer.Tracer.InProcedure(string.Format("CreateBrowserCore({0})", url));
			try {
				CheckWebDevProcesses();
				WebBrowsers.Clear();
				isFirstNavigateError = true;
				IEasyTestWebBrowser xafWebBrowser = webBrowsers.CreateWebBrowser();
				xafWebBrowser.OnNavigateError += new EventHandler<NavigateErrorArgs>(xafWebBrowser_OnNavigateError);
				xafWebBrowser.Browser.Visible = true;
				xafWebBrowser.Browser.Stop();
				xafWebBrowser.WaitForBrowserResponse();
				xafWebBrowser.Navigate(url);
				xafWebBrowser.WaitForBrowserResponse();
				return true;
			}
			catch(COMException e) {
				if(catchException) {
					EasyTestTracer.Tracer.LogError(e);
					return false;
				}
				else {
					throw;
				}
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure(string.Format("CreateBrowserCore({0})", url));
			}
		}
		private bool isFirstNavigateError = true;
		private void xafWebBrowser_OnNavigateError(object sender, NavigateErrorArgs e) {
			IEasyTestWebBrowser xafWebBrowser = (IEasyTestWebBrowser)sender;
			int statusCode;
			int.TryParse(e.StatusCode as string, out statusCode);
			if(isFirstNavigateError && statusCode == 503) {
				EasyTestTracer.Tracer.LogText("eeeeeeeeeeeeeeeeeeeeeeee");
				EasyTestTracer.Tracer.LogText(string.Format("{2}{2}{2}NavigateError1 url: {0}, statusCode:{1}, repeat.{2}", (string)e.Url, e.StatusCode, Environment.NewLine));
				EasyTestTracer.Tracer.LogText("Thread sleep begin");
				Thread.Sleep(200);
				EasyTestTracer.Tracer.LogText("Thread sleep end");
				xafWebBrowser.Navigate((string)e.Url);
				xafWebBrowser.WaitForBrowserResponse();
			}
			else {
				EasyTestTracer.Tracer.LogText(string.Format("{2}{2}{2}WebBrowser NavigateError url: {0}, statusCode:{1}.{2}", (string)e.Url, e.StatusCode, Environment.NewLine));
			}
			isFirstNavigateError = false;
		}
		public Point ActiveBrowserLocation {
			get {
				IntPtr handle = ActiveBrowserWindowHandle;
				if(handle != IntPtr.Zero) {
					Rectangle rect;
					User32.GetWindowRect(handle, out rect);
					return rect.Location;
				}
				return Point.Empty;
			}
		}
		public Point ActiveBrowserBodyLocation {
			get {
				XAFWebBrowserBase br = ActiveBrowser as XAFWebBrowserBase;
				IntPtr handle = ActiveBrowserWindowHandle;
				if(br != null && handle != IntPtr.Zero) {
					int browserProcessId = 0;
					Win32Helper.GetWindowThreadProcessId(handle, out browserProcessId);
					IntPtr bodyHandle = br.GetChildWndForProcess(ActiveBrowserWindowHandle, "Internet Explorer_Server", browserProcessId);
					if(bodyHandle != IntPtr.Zero) {
						Rectangle rect;
						User32.GetWindowRect(bodyHandle, out rect);
						return rect.Location;
					}
				}
				return Point.Empty;
			}
		}
		private IEasyTestWebBrowser ActiveBrowser {
			get {
				if(WebBrowsers != null && WebBrowsers[WebBrowsers.Count - 1] != null && WebBrowsers[WebBrowsers.Count - 1].Browser != null) {
					return WebBrowsers[WebBrowsers.Count - 1];
				}
				return null;
			}
		}
		private IntPtr ActiveBrowserWindowHandle {
			get {
				IEasyTestWebBrowser activeBrowser = ActiveBrowser;
				if(activeBrowser != null) {
					return (IntPtr)activeBrowser.BrowserWindowHandle;
				}
				else {
					return IntPtr.Zero;
				}
			}
		}
		private IntPtr ActiveDialogWindowHandle {
			get {
				IEasyTestWebBrowser activeBrowser = ActiveBrowser;
				if(activeBrowser != null) {
					return activeBrowser.DialogWindowHandle;
				}
				else {
					return IntPtr.Zero;
				}
			}
		}
		private void ExecuteProcess(string exeName, string arguments) {
			Process process = new Process();
			process.StartInfo.FileName = exeName;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.UseShellExecute = true;
			process.Start();
			if(!process.WaitForExit(60000)) {
				process.Kill();
				throw new Exception(string.Format("The '{0}{1}' process has hung up.", exeName, arguments));
			}
		}
		protected IWebBrowserCollection webBrowsers = null;
		protected virtual void RestartIIS() {
			ExecuteProcess("net", " stop w3svc");
			ExecuteProcess("net", " start w3svc");
		}
		public IHTMLDocument2 GetDocument(int index) {
			if(!(WebBrowsers[index].Document is IHTMLDocument2)) {
				throw new AdapterException(String.Format("It's impossible to create document class for the browser object " +
					"(actual class name is {0})", WebBrowsers[index].Document.ToString()));
			}
			return (IHTMLDocument2)WebBrowsers[index].Document;
		}
		public IWebBrowserCollection WebBrowsers {
			get { return webBrowsers; }
		}
		public WebAdapter() {
			using(Form form = new Form()) {
				form.Controls.Add(new System.Windows.Forms.WebBrowser());
				form.Size = new Size(1, 1);
				form.Visible = true;
			}
		}
		#region IApplicationAdapter Members
		public virtual void KillApplication(TestApplication testApplication, KillApplicationConext context) {
			webBrowsers.KillAllWebBrowsers();
			bool isSingleWebDev = testApplication.FindParamValue(SingleWebDevParamName) != null;
			if(testApplication.FindParamValue("DontKillWebDev") == null) {
				if(isSingleWebDev) {
					if(context != KillApplicationConext.TestNormalEnded) {
						WebDevWebServerHelper.KillWebDevWebServer();
					}
				}
				else {
					WebDevWebServerHelper.KillWebDevWebServer();
				}
			}
			if(testApplication.FindParamValue("DontKillIISExpress") == null) {
				if(isSingleWebDev) {
					if(context != KillApplicationConext.TestNormalEnded) {
						IISExpressHelper.KillIISExpressServer();
					}
				}
				else {
					IISExpressHelper.KillIISExpressServer();
				}
			}
		}
		private bool GetParamValue(string name, bool defaultValue, TestApplication testApplication) {
			string paramValue = testApplication.FindParamValue(name);
			bool result;
			if(string.IsNullOrEmpty(paramValue) || !bool.TryParse(paramValue, out result)) {
				result = defaultValue;
			}
			return result;
		}
		private static void LoadFileInfo(string rootDirName) {
			string[] files = Directory.GetFiles(rootDirName);
			for(int i = 0; i < files.Length; i++) {
				string fileName = files[i];
				FileInfo fileInfo = new FileInfo(fileName);
				bool test = fileInfo.IsReadOnly;
			}
			string[] directories = Directory.GetDirectories(rootDirName);
			for(int j = 0; j < directories.Length; j++) {
				string dir = directories[j];
				LoadFileInfo(dir);
			}
		}
		public virtual void RunApplication(TestApplication testApplication) {
			string url = testApplication.GetParamValue(UrlParamName);
			string webBrowserType = testApplication.FindParamValue("WebBrowserType");
			Uri uri = new Uri(url);
			if(string.IsNullOrEmpty(webBrowserType)) {
				webBrowserType = "Default";
			}
			if(webBrowserType == "Default") {
				webBrowsers = new WebBrowserCollection();
			}
			else {
				webBrowsers = new StandaloneWebBrowserCollection();
			}
			string physicalPath = testApplication.FindParamValue("PhysicalPath");
			if(string.IsNullOrEmpty(physicalPath) && !GetParamValue("DontRestartIIS", false, testApplication)) {
				RestartIIS();
			}
			else {
				if(GetParamValue("UseIISExpress", false, testApplication) || !WebDevWebServerHelper.DevWebServerExist(physicalPath)) {
					if(!GetParamValue("DontRunIISExpress", false, testApplication) && !string.IsNullOrEmpty(physicalPath)) {
						LoadFileInfo(Path.GetFullPath(physicalPath));
						if(testApplication.FindParamValue(SingleWebDevParamName) == null) {
							if(IISExpressHelper.IsIISExpressServerStarted(uri)) {
								IISExpressHelper.KillIISExpressServer();
							}
							IISExpressHelper.RunIISExpressServer(Path.GetFullPath(physicalPath), uri.Port.ToString());
						}
						else {
							if(!IISExpressHelper.IsIISExpressServerStarted(uri)) {
								IISExpressHelper.RunIISExpressServer(Path.GetFullPath(physicalPath), uri.Port.ToString());
							}
						}
					}
				}
				else {
					if(!GetParamValue("DontRunWebDev", false, testApplication) && !string.IsNullOrEmpty(physicalPath)) {
						LoadFileInfo(Path.GetFullPath(physicalPath));
						if(testApplication.FindParamValue(SingleWebDevParamName) == null) {
							if(WebDevWebServerHelper.IsWebDevServerStarted(uri)) {
								WebDevWebServerHelper.KillWebDevWebServer();
							}
							WebDevWebServerHelper.RunWebDevWebServer(Path.GetFullPath(physicalPath), uri.Port.ToString());
						}
						else {
							if(!WebDevWebServerHelper.IsWebDevServerStarted(uri)) {
								WebDevWebServerHelper.RunWebDevWebServer(Path.GetFullPath(physicalPath), uri.Port.ToString());
							}
						}
					}
				}
			}
			if(testApplication.FindParamValue("DefaultWindowSize") != null) {
				WebBrowserCollection.DefaultFormSize = GetWindowSize(testApplication.GetParamValue("DefaultWindowSize"));
			}
			string waitDebuggerAttached = testApplication.FindParamValue("WaitDebuggerAttached");
			if(!string.IsNullOrEmpty(waitDebuggerAttached)) {
				Thread.Sleep(8000);
				if(Debugger.IsAttached) {
					MessageBox.Show("Start web application?", "Warning", MessageBoxButtons.OK);
				}
			}
			DateTime current = DateTime.Now;
			while(!WebDevWebServerHelper.IsWebDevServerStarted(uri) && ((TimeSpan)DateTime.Now.Subtract(current)).TotalSeconds < 60) {
				Thread.Sleep(200);
			}
			CreateBrowser(url);
		}
		public static Size GetWindowSize(string parameterValue) {
			Size windowSize = Size.Empty;
			Regex regexp = new Regex(@"(\d+)x(\d+)", RegexOptions.IgnoreCase);
			Match match = regexp.Match(parameterValue);
			if(match.Success && match.Groups.Count == 3) {
				windowSize = new Size(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
			}
			return windowSize;
		}
		public ApplicationView[] GetApplicationViews() {
			ApplicationView jpegView = ImageHelper.CreateApplicationView(ActiveBrowserWindowHandle);
			ApplicationView htmlView = new ApplicationView();
			IHTMLElement HTMLElement = (IHTMLElement)((IHTMLDocument3)GetDocument(WebBrowsers.Count - 1)).getElementsByTagName("HTML").item(0, 0);
			MemoryStream stream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(stream, System.Text.Encoding.Unicode);
			streamWriter.Write(HTMLElement.outerHTML);
			streamWriter.Flush();
			htmlView.applicationViewStream = stream;
			htmlView.fileExtension = ApplicationViewExtension;
			List<ApplicationView> result = new List<ApplicationView>();
			if(jpegView.applicationViewStream != null && jpegView.applicationViewStream.Length > 0) {
				result.Add(jpegView);
			}
			if(htmlView.applicationViewStream != null && htmlView.applicationViewStream.Length > 0) {
				result.Add(htmlView);
			}
			return result.ToArray();
		}
		public virtual ICommandAdapter CreateCommandAdapter() {
			FileDownloadDialogHelper.SaveDialogOpened = false;
			WebCommandAdapter webCommandAdapter = new WebCommandAdapter(this);
			webCommandAdapter.WaitForBrowserResponse(false);
			Win32Helper.MoveMousePointTo(new Point(0, 0));
			return webCommandAdapter;
		}
		public virtual void CloseApplication() {
			this.WebBrowsers.KillAllWebBrowsers();
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			try {
				if(webBrowsers != null) {
					webBrowsers.Dispose();
					webBrowsers = null;
				}
			}
			catch { }
		}
		#endregion
		public void WaitForBrowserResponse() {
			try {
				EasyTestTracer.Tracer.InProcedure("WebAdapter WaitForBrowserResponse");
				webBrowsers.WaitForAllWebBrowsersResponse();
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("WebAdapter WaitForBrowserResponse");
			}
		}
		#region ICommandsRegistrator Members
		public virtual void RegisterCommands(IRegisterCommand registrator) {
			registrator.RegisterCommand("SetTableSelection", typeof(SetTableSelectionCommand));
			registrator.RegisterCommand("ExecuteScript", typeof(ExecuteScriptCommand));
			registrator.RegisterCommand("CheckValidationResult", typeof(WebCheckValidationResultCommand));
			registrator.RegisterCommand("BrowserAction", typeof(BrowserActionCommand));
			registrator.RegisterCommand("MouseClick", typeof(MouseClickCommand));
			registrator.RegisterCommand("AutoTest", typeof(AutoTestCommand));
			registrator.RegisterCommand("SetMaxWaitCallbackTime", typeof(SetMaxWaitCallbackTimeCommand));
			registrator.RegisterCommand("SetMaxWaitTimeoutTime", typeof(SetMaxWaitTimeoutTimeCommand));
			registrator.RegisterCommand("SetThrowExceptionOnWaitCallbackTime", typeof(SetThrowExceptionOnWaitCallbackTimeCommand));
			registrator.RegisterCommand("SetThrowExceptionOnWaitTimeoutTime", typeof(SetThrowExceptionOnWaitTimeoutTimeCommand));
		}
		#endregion
		private void KillAllProcessForName(string processName) {
			try {
				Process[] processesByName = Process.GetProcessesByName(processName);
				foreach(Process process in processesByName) {
					KillProcess(process);
				}
			}
			catch { }
		}
		private void KillProcess(System.Diagnostics.Process process) {
			try {
				if(process != null && !process.HasExited) {
					try {
						process.Kill();
					}
					catch { }
				}
			}
			catch(InvalidOperationException) {
			}
		}
	}
}
