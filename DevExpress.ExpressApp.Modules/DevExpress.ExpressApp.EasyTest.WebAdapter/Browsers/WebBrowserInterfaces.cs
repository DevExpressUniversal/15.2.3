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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public class NewWindow2EventArgs : EventArgs {
		private object newBrowser;
		public object NewBrowser {
			get { return newBrowser; }
			set {
				newBrowser = value;
			}
		}
		private bool cancel;
		public bool Cancel {
			get { return cancel; }
			set {
				cancel = value;
			}
		}
	}
	public class DocumentCompleteArgs : EventArgs {
		private string uRL;
		public string URL {
			get { return uRL; }
			set {
				uRL = value;
			}
		}
		public DocumentCompleteArgs(string uRL) {
			this.uRL = uRL;
		}
	}
	public class NavigateErrorArgs : EventArgs {
		public NavigateErrorArgs(object url, object frame, object statusCode, bool cancel) {
			this.Url = url;
			this.Frame = frame;
			this.StatusCode = statusCode;
			this.Cancel = cancel;
		}
		public object Url {
			get;
			private set;
		}
		public object Frame {
			get;
			private set;
		}
		public object StatusCode {
			get;
			private set;
		}
		public bool Cancel {
			get;
			private set;
		}
	}
	[ClassInterface(ClassInterfaceType.None)]
	public class WebBrowserEvent : DWebBrowserEvents2 {
		public event EventHandler<NewWindow2EventArgs> OnNewWindow2;
		public event EventHandler OnDownloadBegin;
		public event EventHandler OnDownloadComplete;
		public event EventHandler BeforeNavigate;
		public event EventHandler<DocumentCompleteArgs> OnDocumentComplete;
		public event EventHandler Quit;
		public event EventHandler<NavigateErrorArgs> OnNavigateError;
		#region DWebBrowserEvents2 Members
		public void StatusTextChange(string text) {
		}
		public void ProgressChange(int progress, int progressMax) {
		}
		public void CommandStateChange(long command, bool enable) {
		}
		public void DownloadBegin() {
			if(OnDownloadBegin != null) {
				OnDownloadBegin(this, EventArgs.Empty);
			}
		}
		public void DownloadComplete() {
			if(OnDownloadComplete != null) {
				OnDownloadComplete(this, EventArgs.Empty);
			}
		}
		public void TitleChange(string text) {
		}
		public void PropertyChange(string szProperty) {
		}
		public void BeforeNavigate2(object pDisp, ref object URL, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel) {
			if(BeforeNavigate != null) {
				BeforeNavigate(this, EventArgs.Empty);
			}
		}
		public void NewWindow2(ref object pDisp, ref bool cancel) {
			if(OnNewWindow2 != null) {
				NewWindow2EventArgs args = new NewWindow2EventArgs();
				args.Cancel = cancel;
				OnNewWindow2(this, args);
				pDisp = args.NewBrowser;
				cancel = args.Cancel;
			}
		}
		public void NavigateComplete2(object pDisp, ref object URL) {
		}
		public void DocumentComplete(object pDisp, ref object URL) {
			if(OnDocumentComplete != null) {
				OnDocumentComplete(this, new DocumentCompleteArgs(URL.ToString()));
			}
		}
		public void OnQuit() {
			if(Quit != null) {
				Quit(this, EventArgs.Empty);
			}
		}
		public void OnVisible(bool visible) {
		}
		public void OnToolBar(bool toolBar) {
		}
		public void OnMenuBar(bool menuBar) {
		}
		public void OnStatusBar(bool statusBar) {
		}
		public void OnFullScreen(bool fullScreen) {
		}
		public void OnTheaterMode(bool theaterMode) {
		}
		public void WindowSetResizable(bool resizable) {
		}
		public void WindowSetLeft(int left) {
		}
		public void WindowSetTop(int top) {
		}
		public void WindowSetWidth(int width) {
		}
		public void WindowSetHeight(int height) {
		}
		public void WindowClosing(bool isChildWindow, ref bool cancel) {
		}
		public void ClientToHostWindow(ref long cx, ref long cy) {
		}
		public void SetSecureLockIcon(int secureLockIcon) {
		}
		public void FileDownload(ref bool cancel) {
		}
		public void NavigateError(object pDisp, ref object URL, ref object frame, ref object statusCode, ref bool cancel) {
			if(OnNavigateError != null) {
				OnNavigateError(this, new NavigateErrorArgs(URL, frame, statusCode, cancel));
			}
		}
		public void PrintTemplateInstantiation(object pDisp) {
		}
		public void PrintTemplateTeardown(object pDisp) {
		}
		public void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone) {
		}
		public void PrivacyImpactedStateChange(bool bImpacted) {
		}
		#endregion
	}
	[ComImport, Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType(TypeLibTypeFlags.FHidden)]
	public interface DWebBrowserEvents2 {
		[DispId(0x66)]
		void StatusTextChange([In] string text);
		[DispId(0x6c)]
		void ProgressChange([In] int progress, [In] int progressMax);
		[DispId(0x69)]
		void CommandStateChange([In] long command, [In] bool enable);
		[DispId(0x6a)]
		void DownloadBegin();
		[DispId(0x68)]
		void DownloadComplete();
		[DispId(0x71)]
		void TitleChange([In] string text);
		[DispId(0x70)]
		void PropertyChange([In] string szProperty);
		[DispId(250)]
		void BeforeNavigate2([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers, [In, Out] ref bool cancel);
		[DispId(0xfb)]
		void NewWindow2([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object pDisp, [In, Out] ref bool cancel);
		[DispId(0xfc)]
		void NavigateComplete2([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);
		[DispId(0x103)]
		void DocumentComplete([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);
		[DispId(0xfd)]
		void OnQuit();
		[DispId(0xfe)]
		void OnVisible([In] bool visible);
		[DispId(0xff)]
		void OnToolBar([In] bool toolBar);
		[DispId(0x100)]
		void OnMenuBar([In] bool menuBar);
		[DispId(0x101)]
		void OnStatusBar([In] bool statusBar);
		[DispId(0x102)]
		void OnFullScreen([In] bool fullScreen);
		[DispId(260)]
		void OnTheaterMode([In] bool theaterMode);
		[DispId(0x106)]
		void WindowSetResizable([In] bool resizable);
		[DispId(0x108)]
		void WindowSetLeft([In] int left);
		[DispId(0x109)]
		void WindowSetTop([In] int top);
		[DispId(0x10a)]
		void WindowSetWidth([In] int width);
		[DispId(0x10b)]
		void WindowSetHeight([In] int height);
		[DispId(0x107)]
		void WindowClosing([In] bool isChildWindow, [In, Out] ref bool cancel);
		[DispId(0x10c)]
		void ClientToHostWindow([In, Out] ref long cx, [In, Out] ref long cy);
		[DispId(0x10d)]
		void SetSecureLockIcon([In] int secureLockIcon);
		[DispId(270)]
		void FileDownload([In, Out] ref bool cancel);
		[DispId(0x10f)]
		void NavigateError([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL, [In] ref object frame, [In] ref object statusCode, [In, Out] ref bool cancel);
		[DispId(0xe1)]
		void PrintTemplateInstantiation([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp);
		[DispId(0xe2)]
		void PrintTemplateTeardown([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp);
		[DispId(0xe3)]
		void UpdatePageStatus([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object nPage, [In] ref object fDone);
		[DispId(0x110)]
		void PrivacyImpactedStateChange([In] bool bImpacted);
	}
	[ComImport, Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E"), TypeLibType(TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FHidden), SuppressUnmanagedCodeSecurity]
	public interface IWebBrowser2 {
		[DispId(100)]
		void GoBack();
		[DispId(0x65)]
		void GoForward();
		[DispId(0x66)]
		void GoHome();
		[DispId(0x67)]
		void GoSearch();
		[DispId(0x68)]
		void Navigate([In] string Url, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);
		[DispId(-550)]
		void Refresh();
		[DispId(0x69)]
		void Refresh2([In] ref object level);
		[DispId(0x6a)]
		void Stop();
		[DispId(200)]
		object Application { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
		[DispId(0xc9)]
		object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
		[DispId(0xca)]
		object Container { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
		[DispId(0xcb)]
		object Document { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
		[DispId(0xcc)]
		bool TopLevelContainer { get; }
		[DispId(0xcd)]
		string Type { get; }
		[DispId(0xce)]
		int Left { get; set; }
		[DispId(0xcf)]
		int Top { get; set; }
		[DispId(0xd0)]
		int Width { get; set; }
		[DispId(0xd1)]
		int Height { get; set; }
		[DispId(210)]
		string LocationName { get; }
		[DispId(0xd3)]
		string LocationURL { get; }
		[DispId(0xd4)]
		bool Busy { get; }
		[DispId(300)]
		void Quit();
		[DispId(0x12d)]
		void ClientToWindow(out int pcx, out int pcy);
		[DispId(0x12e)]
		void PutProperty([In] string property, [In] object vtValue);
		[DispId(0x12f)]
		object GetProperty([In] string property);
		[DispId(0)]
		string Name { get; }
		[DispId(-515)]
		int HWND { get; }
		[DispId(400)]
		string FullName { get; }
		[DispId(0x191)]
		string Path { get; }
		[DispId(0x192)]
		bool Visible { get; set; }
		[DispId(0x193)]
		bool StatusBar { get; set; }
		[DispId(0x194)]
		string StatusText { get; set; }
		[DispId(0x195)]
		int ToolBar { get; set; }
		[DispId(0x196)]
		bool MenuBar { get; set; }
		[DispId(0x197)]
		bool FullScreen { get; set; }
		[DispId(500)]
		void Navigate2([In] ref object URL, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);
		[DispId(0x1f5)]
		OLECMDF QueryStatusWB([In] OLECMDID cmdID);
		[DispId(0x1f6)]
		void ExecWB([In] OLECMDID cmdID, [In] OLECMDEXECOPT cmdexecopt, ref object pvaIn, IntPtr pvaOut);
		[DispId(0x1f7)]
		void ShowBrowserBar([In] ref object pvaClsid, [In] ref object pvarShow, [In] ref object pvarSize);
		[DispId(-525)]
		WebBrowserReadyState ReadyState { get; }
		[DispId(550)]
		bool Offline { get; set; }
		[DispId(0x227)]
		bool Silent { get; set; }
		[DispId(0x228)]
		bool RegisterAsBrowser { get; set; }
		[DispId(0x229)]
		bool RegisterAsDropTarget { get; set; }
		[DispId(0x22a)]
		bool TheaterMode { get; set; }
		[DispId(0x22b)]
		bool AddressBar { get; set; }
		[DispId(0x22c)]
		bool Resizable { get; set; }
	}
	public enum OLECMDF {
		OLECMDF_DEFHIDEONCTXTMENU = 0x20,
		OLECMDF_ENABLED = 2,
		OLECMDF_INVISIBLE = 0x10,
		OLECMDF_LATCHED = 4,
		OLECMDF_NINCHED = 8,
		OLECMDF_SUPPORTED = 1
	}
	public enum OLECMDEXECOPT {
		OLECMDEXECOPT_DODEFAULT,
		OLECMDEXECOPT_PROMPTUSER,
		OLECMDEXECOPT_DONTPROMPTUSER,
		OLECMDEXECOPT_SHOWHELP
	}
	public enum OLECMDID {
		OLECMDID_PAGESETUP = 8,
		OLECMDID_PRINT = 6,
		OLECMDID_PRINTPREVIEW = 7,
		OLECMDID_PROPERTIES = 10,
		OLECMDID_SAVEAS = 4
	}
	public enum WebBrowserReadyState {
		Uninitialized,
		Loading,
		Loaded,
		Interactive,
		Complete
	}
}
