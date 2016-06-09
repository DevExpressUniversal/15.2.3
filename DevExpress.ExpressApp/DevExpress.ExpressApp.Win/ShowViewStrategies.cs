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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Templates;
namespace DevExpress.ExpressApp.Win {
	public abstract class WinShowViewStrategyBase : ShowViewStrategyBase {
		#region WaitCursorScope
		private sealed class WaitCursorScope : IDisposable {
			private Cursor backup;
			private WinWindow window;
			public WaitCursorScope() {
				backup = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;
			}
			public WaitCursorScope(WinWindow window)
				: this() {
				this.window = window;
				this.window.Activated += new EventHandler(window_Activated);
			}
			private void window_Activated(object sender, EventArgs e) {
				Cursor.Current = backup;
			}
			public void Dispose() {
				Cursor.Current = backup;
				if(window != null) {
					window.Activated -= new EventHandler(window_Activated);
					window = null;
				}
			}
		}
		#endregion
		private List<WinWindow> explorers = new List<WinWindow>();
		private List<WinWindow> inspectors = new List<WinWindow>();
		protected bool internalCloseMainWindow = false;
		private void StartupWindow_Load(object sender, EventArgs e) {
			Form startupForm = (Form)sender;
			startupForm.Load -= new EventHandler(StartupWindow_Load);
			WinWindow startupWindow = FindWindowByForm(startupForm);
			OnStartupWindowLoad(startupWindow);
		}
		private void window_Closing(object sender, CancelEventArgs e) {
			if(!e.Cancel) {
				OnWindowClosing((WinWindow)sender, e);
			}
		}
		private void window_Closed(object sender, EventArgs e) {
			WinWindow window = (WinWindow)sender;
			window.Closed -= new EventHandler(window_Closed);
			OnWindowClosed(window);
			if(!internalCloseMainWindow && windows.Count == 0) {
				ExitApplication();
			}
		}
		private bool CloseWindows(ICollection<WinWindow> windows) {
			List<WinWindow> clone = new List<WinWindow>(windows);
			foreach(WinWindow win in clone) {
				if(!win.IsClosing && !win.Close())
					return false;
			}
			return true;
		}
		private void AddWindow(WinWindow window) {
			if(!windows.Contains(window)) {
				window.Closed += new EventHandler(window_Closed);
				window.Closing += new CancelEventHandler(window_Closing);
				windows.Add(window);
				if(IsExplorer(window)) {
					explorers.Add(window);
				}
				else {
					inspectors.Add(window);
				}
				AfterAddWindow(window);
			}
		}
		private void RemoveWindow(WinWindow window) {
			windows.Remove(window);
			if(IsExplorer(window)) {
				explorers.Remove(window);
			}
			else {
				inspectors.Remove(window);
			}
		}
		protected virtual WinWindow CreateWindow(ShowViewParameters parameters, ShowViewSource showViewSource, bool isMain) {
			WinWindow window = null;
			try {
				TemplateContext context = isMain ? TemplateContext.ApplicationWindow : CalculateTemplateContext(parameters);
				window = (WinWindow)Application.CreateWindow(context, parameters.Controllers, isMain);
				AddWindow(window);
				if(!isMain && showViewSource != null) {
					window.SetView(parameters.CreatedView, showViewSource.SourceFrame);
				}
				SetAndApplySettings(window);
			}
			catch {
				if(window != null) {
					RemoveWindow(window);
					window.Dispose();
				}
				throw;
			}
			return window;
		}
		protected virtual void SetAndApplySettings(WinWindow window) {
			ISupportStoreSettings template = window.Template as ISupportStoreSettings;
			if(template != null) {
				template.SetSettings(Application.GetTemplateCustomizationModel(window.Template));
				template.ReloadSettings();
			}
		}
		protected void ReloadSettings(WinWindow window) {
			if(window.Template is ISupportStoreSettings) {
				((ISupportStoreSettings)window.Template).ReloadSettings();
			}
		}
		protected virtual void ExitApplication() {
			System.Windows.Forms.Application.Exit();
		}
		protected virtual void OnWindowClosing(WinWindow window, CancelEventArgs e) {
			List<WinWindow> childWindows = GetChildWindows(window);
			if(!CloseWindows(childWindows)) {
				e.Cancel = true;
			}
		}
		protected virtual List<WinWindow> GetChildWindows(WinWindow winWindow) {
			if(IsExplorer(winWindow)) {
				return inspectors;
			}
			return new List<WinWindow>();
		}
		protected virtual void OnWindowClosed(WinWindow window) {
			RemoveWindow(window);
		}
		protected WinWindow FindWindowByForm(Form form) {
			foreach(WinWindow window in Windows) {
				if(window.Form == form) {
					return window;
				}
			}
			return null;
		}
		protected bool IsExplorer(Form form) {
			return form is IXafDocumentsHostWindow;
		}
		protected bool IsExplorer(WinWindow window) {
			return IsExplorer(window.Form);
		}
		protected virtual void ShowWindow(WinWindow window) {
			window.Show();
		}
		protected List<WinWindow> windows = new List<WinWindow>();
		protected virtual TemplateContext CalculateTemplateContext(ShowViewParameters parameters) {
			TemplateContext context = parameters.Context;
			if(context == TemplateContext.Undefined) {
				if(parameters.Controllers.Exists((Controller c) => { return (c is DialogController); })) {
					context = TemplateContext.PopupWindow;
				}
				else {
					context = TemplateContext.View;
				}
			}
			return context;
		}
		protected virtual WindowStyle CalculateWindowStyle(ShowViewParameters parameters, ShowViewSource showViewSource) {
			return WindowStyle.Inspector;
		}
		protected bool IsNotNewWindowRequired(WinWindow existingWindow, View view) {
			if(view is DetailView) {
				if(!(existingWindow.View is DetailView) || ((DetailView)existingWindow.View).ObjectTypeInfo.IsPersistent) {
					ViewShortcut existingWindowViewShortcut = existingWindow.View.CreateShortcut();
					return
						String.IsNullOrEmpty(existingWindowViewShortcut[ViewShortcut.IsNewObject])
						||
						!Boolean.Parse(existingWindowViewShortcut[ViewShortcut.IsNewObject]);
				}
				else {
					return view.CurrentObject == existingWindow.View.CurrentObject;
				}
			}
			else {
				return true;
			}
		}
		protected virtual bool IsCompatibleWindow(WinWindow existingWindow, View view, ViewShortcut viewShortcut) {
			return
				(existingWindow.View != null)
				&&
				(existingWindow.View.CreateShortcut() == viewShortcut)
				&&
				IsNotNewWindowRequired(existingWindow, view);
		}
		protected virtual WinWindow FindWindowByView(View view) {
			ViewShortcut viewShortcut = view.CreateShortcut();
			foreach(WinWindow existingWindow in Windows) {
				if(IsCompatibleWindow(existingWindow, view, viewShortcut)) {
					return existingWindow;
				}
			}
			return null;
		}
		protected virtual void AfterAddWindow(WinWindow window) { }
		protected virtual void DisposeDialogWindow(WinWindow window) {
			window.Dispose();
		}
		protected virtual void ShowDialogWindow(WinWindow window) {
			window.ShowDialog();
		}
		protected void ShowViewInWindow(Window window, Frame sourceFrame, View view) {
			using(new WaitCursorScope()) {
				window.SetView(view, sourceFrame);
			}
		}
		protected override Window ShowViewInNewWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			WindowStyle windowType = CalculateWindowStyle(parameters, showViewSource);
			WinWindow window;
			using(new WaitCursorScope()) {
				if(windowType == WindowStyle.Explorer) {
					window = ShowViewInExplorer(parameters, showViewSource);
				}
				else {
					window = ShowViewInInspector(parameters, showViewSource);
				}
			}
			return window;
		}
		protected virtual void ActivateForm(Form form) {
			form.Activate();
		}
		protected virtual WinWindow CreateInspectorWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			WinWindow existentWindow = FindWindowByView(parameters.CreatedView);
			if(existentWindow != null && existentWindow.Form != null && !IsNewWindowForced(parameters, showViewSource)) {
				ActivateForm(existentWindow.Form);
				return existentWindow;
			}
			else {
				return CreateWindow(parameters, showViewSource, false);
			}
		}
		protected virtual WinWindow CreateExplorerWindow(ShowViewParameters parameters) {
			return CreateWindow(parameters, null, true);
		}
		protected virtual WinWindow ShowViewInInspector(ShowViewParameters parameters, ShowViewSource showViewSource) {
			WinWindow window = CreateInspectorWindow(parameters, showViewSource);
			ShowWindow(window);
			return window;
		}
		protected virtual WinWindow ShowViewInExplorer(ShowViewParameters parameters, ShowViewSource showViewSource) {
			WinWindow window = FindWindowByView(parameters.CreatedView);
			if(window == null || IsNewWindowForced(parameters, showViewSource)) {
				window = ShowViewInExplorerCore(parameters, showViewSource);
			}
			else {
				ShowWindow(window);
			}
			return window;
		}
		protected virtual WinWindow ShowViewInExplorerCore(ShowViewParameters parameters, ShowViewSource showViewSource) {
			WinWindow window = CreateExplorerWindow(parameters);
			ShowWindow(window);
			ShowViewInInspector(parameters, showViewSource);
			return window;
		}
		protected virtual bool IsNewWindowForced(ShowViewParameters parameters, ShowViewSource showViewSource) {
			return GetPreferredNewWindowTargetByKeys() == NewWindowTarget.Separate;
		}
		protected virtual NewWindowTarget CalculateNewWindowTarget(ShowViewParameters parameters, ShowViewSource showViewSource) {
			NewWindowTarget result = parameters.NewWindowTarget;
			if(result == NewWindowTarget.Default) {
				result = GetPreferredNewWindowTargetByKeys();
			}
			return result;
		}
		protected override void ShowViewFromNestedView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			parameters.CreatedView.IsRoot = !IsSameObjectSpace(parameters, showViewSource);
			ShowViewInModalWindow(parameters, showViewSource);
		}
		protected override void ShowViewInModalWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			TemplateContext context = CalculateTemplateContext(parameters);
			WinWindow window = (WinWindow)Application.CreateWindow(context, parameters.Controllers, parameters.CreateAllControllers, false);
			try {
				using(new WaitCursorScope(window)) {
					window.SetView(parameters.CreatedView, showViewSource.SourceFrame);
					SetAndApplySettings(window);
					ShowDialogWindow(window);
				}
			}
			finally {
				DisposeDialogWindow(window);
			}
		}
		protected override void ShowViewFromCommonView(ShowViewParameters parameters, ShowViewSource showViewSource) { }
		protected override void ShowViewFromLookupView(ShowViewParameters parameters, ShowViewSource showViewSource) { }
		protected override void ShowViewInCurrentWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			showViewSource.SourceFrame.SetView(parameters.CreatedView, showViewSource.SourceFrame);
		}
		protected virtual void OnStartupWindowLoad(WinWindow startupWindow) {
			if(StartupWindowLoad != null) {
				StartupWindowLoad(this, EventArgs.Empty);
			}
		}
		protected virtual Keys GetControlModifierKeys() {
			return Control.ModifierKeys;
		}
		protected virtual NewWindowTarget GetPreferredNewWindowTargetByKeys() {
			Keys controlModifierKeys = GetControlModifierKeys();
			if((controlModifierKeys & Keys.Control) == Keys.Control) {
				return NewWindowTarget.MdiChild;
			}
			else if((controlModifierKeys & Keys.Shift) == Keys.Shift) {
				return NewWindowTarget.Separate;
			}
			else {
				return NewWindowTarget.Default;
			}
		}
		protected abstract UIType GetUIType();
		protected virtual WinWindow ShowStartupWindowCore() {
			ShowViewParameters showViewParameters = new ShowViewParameters();
			showViewParameters.Context = TemplateContext.ApplicationWindow;
			WinWindow startupWindow = CreateExplorerWindow(showViewParameters);
			startupWindow.Form.Load += new EventHandler(StartupWindow_Load);
			ShowWindow(startupWindow);
			return startupWindow;
		}
		public override void Dispose() {
			if(windows != null) {
				foreach(var window in windows) {
					window.Closed -= new EventHandler(window_Closed);
					window.Closing -= new CancelEventHandler(window_Closing);
				}
				CloseAllWindows();
				windows = null;
			}
		}
		public WinShowViewStrategyBase(XafApplication application) : base(application) { }
		public void ShowStartupWindow() {
			ShowStartupWindowCore();
		}
		public virtual bool CloseAllWindows() {
			internalCloseMainWindow = true;
			try {
				return CloseWindows(windows);
			}
			finally {
				internalCloseMainWindow = false;
			}
		}
		public WinWindow MainWindow {
			get { return FindWindowByForm(WinWindow.LastActiveExplorer); }
		}
		public ReadOnlyCollection<WinWindow> Windows {
			get { return windows.AsReadOnly(); }
		}
		public List<WinWindow> Explorers {
			get { return explorers; }
		}
		public List<WinWindow> Inspectors {
			get { return inspectors; }
		}
		public UIType UIType {
			get { return GetUIType(); }
		}
		public event EventHandler StartupWindowLoad;
	}
	public class ShowInSingleWindowStrategy : WinShowViewStrategyBase {
		protected override void ShowViewFromCommonView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			Frame sourceFrame = showViewSource.SourceFrame;
			if(sourceFrame != null && sourceFrame.Template is Form && ((Form)sourceFrame.Template).Modal) {
				sourceFrame.SetView(parameters.CreatedView, sourceFrame);
			}
			else if(sourceFrame is Window && !((Window)sourceFrame).IsMain) {
				ShowViewInNewWindow(parameters, showViewSource);
			}
			else {
				ShowViewInWindow(MainWindow, sourceFrame, parameters.CreatedView);
			}
		}
		protected override void ShowViewFromLookupView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			ShowViewInModalWindow(parameters, showViewSource);
		}
		protected override UIType GetUIType() {
			return UIType.SingleWindowSDI;
		}
		public ShowInSingleWindowStrategy(XafApplication application) : base(application) { }
	}
	public class ShowInMultipleWindowsStrategy : WinShowViewStrategyBase {
		private void ShowDetailView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			DetailView detailView = parameters.CreatedView as DetailView;
			WinWindow window = FindWindowByView(detailView);
			if(window != null) {
				if(window.View != detailView) {
					detailView.Dispose();
				}
				window.Show();
			}
			else {
				if(parameters.Context == TemplateContext.Undefined) {
					parameters.Context = TemplateContext.View;
				}
				ShowViewInNewWindow(parameters, showViewSource);
			}
		}
		private void ShowViewByType(ShowViewParameters parameters, ShowViewSource showViewSource) {
			if(parameters.CreatedView is DetailView) {
				ShowDetailView(parameters, showViewSource);
			}
			else {
				ShowViewInWindow(MainWindow, showViewSource.SourceFrame, parameters.CreatedView);
			}
		}
		public ShowInMultipleWindowsStrategy(XafApplication application) : base(application) { }
		protected override void ShowViewFromCommonView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			ShowViewByType(parameters, showViewSource);
		}
		protected override void ShowViewFromLookupView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			ShowViewByType(parameters, showViewSource);
		}
		protected override Window ShowViewInNewWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			WinWindow window = FindWindowByView(parameters.CreatedView);
			if((window != null) && (window != MainWindow)) {
				if(window.View != parameters.CreatedView) {
					parameters.CreatedView.Dispose();
				}
				window.Show();
			}
			else {
				base.ShowViewInNewWindow(parameters, showViewSource);
			}
			return window;
		}
		protected override UIType GetUIType() {
			return UIType.MultipleWindowSDI;
		}
	}
}
