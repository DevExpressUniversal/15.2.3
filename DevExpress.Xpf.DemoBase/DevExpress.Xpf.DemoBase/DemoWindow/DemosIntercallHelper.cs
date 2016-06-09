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
using System.Diagnostics;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using DevExpress.DemoData.Helpers;
using System.Globalization;
using System.Windows.Controls;
using DevExpress.DemoData.Core;
namespace DevExpress.DemoData {
	public interface IBackButton {
		bool DesiredVisibility { get; }
		event EventHandler DesiredVisibilityChanged;
		bool ActualVisibility { get; set; }
		event EventHandler Click;
		void DoClick();
	}
	public static class DemosIntercallHelper {
#if DEBUG
		const int TimeoutInSeconds = 1200;
#else
		const int TimeoutInSeconds = 55;
#endif
		static string xpfDemosExePath;
		static IntPtr mainWindowHandle;
		static IntPtr demoMainWindowHandle;
		static WindowState mainWindowState;
		static IBackButton mainBackButton;
		static IBackButton demoBackButton;
		static bool savedDemoBackButtonVisibility;
		static Action<string, string> savedLoadDemoAction;
		static AutoResetEvent waitDemoStart;
		static IntPtr demoWindowHandle;
		static string demo = null;
		static string module = null;
		[SecuritySafeCritical]
		public static void RunXpfDemo(string demoName, string moduleTitle, bool newInstance = false) {
			Process process = null;
			demoWindowHandle = IntPtr.Zero;
			waitDemoStart = null;
			SaveWindowPosition();
			Application.Current.MainWindow.IsHitTestVisible = false;
			BackgroundHelper.DoInBackground(() => {
				try {
					waitDemoStart = new AutoResetEvent(false);
					string demoArg = BookmarkHelper.UrlHelper.Screen(demoName);
					string moduleArg = BookmarkHelper.UrlHelper.Screen(moduleTitle);
					if(EnvironmentHelper.IsClickOnce) {
						StartClickOnceDemoHelper.Start(string.Format("?H={0}&{1}&{2}", mainWindowHandle, demoArg, moduleArg));
					} else {
						ProcessStartInfo psi = new ProcessStartInfo(DemoLauncherExePath, string.Format("/H:{0} {1} {2}", mainWindowHandle, demoArg, moduleArg));
						psi.WindowStyle = ProcessWindowStyle.Minimized;
						process = Process.Start(psi);
					}
					if(!waitDemoStart.WaitOne(TimeoutInSeconds * 1000))
						throw new InvalidOperationException();
					if(!ChangeCurrentDemo(demoWindowHandle))
						throw new InvalidOperationException();
				} catch {
					try {
						if(process != null)
							process.Kill();
					} catch { }
					process = null;
					demoWindowHandle = IntPtr.Zero;
					MessageBox.Show("Cannot start demo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}, () => {
				if(demoWindowHandle != IntPtr.Zero) {
					try {
						WinApiHelper.SendData(demoWindowHandle, Encoding.UTF8.GetBytes("L"));
					} catch {
						Environment.Exit(0);
					}
					if(!newInstance) {
						Environment.Exit(0);
					} else {
						process = null;
						demoWindowHandle = IntPtr.Zero;
						try {
							Application.Current.MainWindow.IsHitTestVisible = true;
						} catch { }
					}
				}
			}, 0);
		}
		[SecuritySafeCritical]
		public static bool RegisterDemo(Action<string, string> loadDemoAction) {
			GetDemoAndModule();
			if(demo == "install") {
				Environment.Exit(0);
				return false;
			}
			demoMainWindowHandle = GetMainHandle();
			if(demoMainWindowHandle == IntPtr.Zero) {
				loadDemoAction(demo, module);
				return false;
			}
			savedLoadDemoAction = loadDemoAction;
			Window w = Application.Current.MainWindow;
			WindowInteropHelper wih = new WindowInteropHelper(w);
			WinApiHelper.WinProcHook hook = WinApiHelper.WinProcHook.Get(wih.Handle);
			hook.OnDataReceived += OnDataFromDemoChooserReceived;
			hook.ProcessWmCopyData = true;
			WinApiHelper.SendData(demoMainWindowHandle, Encoding.UTF8.GetBytes("r" + HandleToString(wih.Handle)));
			return true;
		}
		public static void RegisterMainBackButton(IBackButton backButton) {
			mainBackButton = backButton;
			mainBackButton.Click += OnMainBackButtonClick;
			mainBackButton.DesiredVisibilityChanged += OnMainBackButtonDesiredVisibilityChanged;
			OnMainBackButtonDesiredVisibilityChanged(mainBackButton, EventArgs.Empty);
		}
		public static void RegisterDemoBackButton(IBackButton backButton) {
			demoBackButton = backButton;
			demoBackButton.Click += OnDemoBackButtonClick;
			demoBackButton.DesiredVisibilityChanged += OnDemoBackButtonDesiredVisibilityChanged;
			OnDemoBackButtonDesiredVisibilityChanged(demoBackButton, EventArgs.Empty);
		}
		static IntPtr GetMainHandle() {
			return StringToHandle(EnvironmentHelper.GetParameter("H").Value);
		}
		static void OnMainBackButtonClick(object sender, EventArgs e) {
			DoBackButtonClick();
		}
		static void OnDemoBackButtonClick(object sender, EventArgs e) {
			DoBackButtonClick();
		}
		static void DoBackButtonClick() {
			if(demoBackButton == null) return;
			demoBackButton.DoClick();
		}
		static void OnDemoBackButtonDesiredVisibilityChanged(object sender, EventArgs e) {
			savedDemoBackButtonVisibility = demoBackButton.DesiredVisibility;
			UpdateBackButtonVisibilities();
		}
		static void OnMainBackButtonDesiredVisibilityChanged(object sender, EventArgs e) {
			UpdateBackButtonVisibilities();
		}
		static void UpdateBackButtonVisibilities() {
			if(mainBackButton != null)
				mainBackButton.ActualVisibility = mainBackButton.DesiredVisibility && savedDemoBackButtonVisibility;
			bool demoBackButtonActualVisiblity = savedDemoBackButtonVisibility && (mainBackButton == null || !mainBackButton.ActualVisibility);
			UpdateDemoBackButtonVisibility(demoBackButtonActualVisiblity);
		}
		static void UpdateDemoBackButtonVisibility(bool demoBackButtonActualVisiblity) {
			if(demoBackButton == null) return;
			demoBackButton.ActualVisibility = demoBackButtonActualVisiblity;
		}
		static void SaveWindowPosition() {
			Window mainWindow = Application.Current.MainWindow;
			if(mainWindowHandle == IntPtr.Zero) {
				mainWindowHandle = new WindowInteropHelper(mainWindow).Handle;
				WinApiHelper.WinProcHook hook = WinApiHelper.WinProcHook.Get(mainWindowHandle);
				hook.OnDataReceived += OnDataFromDemoReceived;
				hook.ProcessWmCopyData = true;
			}
			mainWindowState = mainWindow.WindowState;
		}
		static void OnDataFromDemoReceived(byte[] data) {
			string s = Encoding.UTF8.GetString(data);
			if(string.IsNullOrEmpty(s)) return;
			switch(s[0]) {
				case 'r': OnRegisterMessageReceived(s.SafeSubstring(1)); break;
			}
		}
		static void OnDataFromDemoChooserReceived(byte[] data) {
			string s = Encoding.UTF8.GetString(data);
			if(string.IsNullOrEmpty(s)) return;
			switch(s[0]) {
				case 'L': OnLoadRealDemoReceived(); break;
			}
		}
		static void GetDemoAndModule() {
			demo = string.Empty;
			module = string.Empty;
			List<string> args = EnvironmentHelper.GetParameter(null).Values;
			if(args.Count > 0)
				demo = BookmarkHelper.UrlHelper.Unscreen(args[0]);
			if(args.Count > 1)
				module = BookmarkHelper.UrlHelper.Unscreen(args[1]);
			if(string.IsNullOrEmpty(demo))
				demo = "DemoChooser";
		}
		static void OnLoadRealDemoReceived() {
			savedLoadDemoAction(demo, module);
		}
		static void OnRegisterMessageReceived(string s) {
			demoWindowHandle = StringToHandle(s);
			waitDemoStart.Set();
		}
		static bool ChangeCurrentDemo(IntPtr demoWindowHandle) {
			for(int i = 10; --i >= 0; ) {
				Thread.Sleep(10);
				if(!WinApiHelper.CopyWindowPlacement(mainWindowHandle, demoWindowHandle)) return false;
				if(!WinApiHelper.SetWindowState(demoWindowHandle, mainWindowState)) return false;
			}
			return true;
		}
		static string DemoLauncherExePath {
			get {
				if(xpfDemosExePath == null) {
					xpfDemosExePath = PathHelper.GetDemoExePath("DemoLauncher");
				}
				return xpfDemosExePath;
			}
		}
		static string HandleToString(IntPtr handle) {
			return handle.ToInt64().ToString();
		}
		static IntPtr StringToHandle(string s) {
			if(string.IsNullOrEmpty(s)) return IntPtr.Zero;
			try {
				return new IntPtr(long.Parse(s));
			} catch {
				return IntPtr.Zero;
			}
		}
	}
}
