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
using System.CodeDom.Compiler;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.Design.VSIntegration;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Design.GroupSort;
using DevExpress.XtraReports.Design.Tools;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
namespace DevExpress.XtraReports.Design {
	public class XRVSToolWindow : VSToolWindow {
		const string registryKey = DevExpress.XtraPrinting.Native.NativeSR.RegistryPath + "ToolWindowsVisibility";
		IDesignerEventService service;
		protected override string RegistryKey {
			get { return registryKey; }
		}
		protected override Type ResFinderType {
			get { return typeof(DevExpress.XtraReports.Design.ResFinder); }
		}
		public XRVSToolWindow(IServiceProvider servProvider, string caption, Guid toolWindowGuid, string bitmapResourceName) :
			base(servProvider, caption, toolWindowGuid, bitmapResourceName) {
			service = servProvider.GetService(typeof(IDesignerEventService)) as IDesignerEventService;
		}
		protected override bool CanChangePersistentVisibility(Microsoft.VisualStudio.Shell.Interop.__FRAMESHOW fShow) {
			return base.CanChangePersistentVisibility(fShow) && service != null && service.ActiveDesigner != null && service.ActiveDesigner.RootComponent is XtraReport;
		}
	}
	public class ReportFormattingBar : IToolItem {
		CommandBarFontService commandBarFontService;
		IDesignerHost designerHost;
		IServiceProvider serviceProvider;
		public Guid Kind { get { return ReportToolItemKindHelper.FormattingBar; } }
		public ReportFormattingBar(IServiceProvider serviceProvider) {
			commandBarFontService = new CommandBarFontService(serviceProvider);
			designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			designerHost.AddService(typeof(FontServiceBase), commandBarFontService);
			this.serviceProvider = serviceProvider;
		}
		public void InitTool() {
		}
		public void UpdateView() {
		}
		public void Hide() {
			commandBarFontService.Deactivate();
		}
		public void Close() {
			commandBarFontService.Close();
		}
		public void ShowNoActivate() {
			ShowActivate();
		}
		public void ShowActivate() {
			commandBarFontService.Activate();
		}
		public void Dispose() {
			designerHost.RemoveService(typeof(FontServiceBase));
			commandBarFontService.Dispose();
		}
	}
	public class ReportMenu : VSMenu {
		const string reportsMenuName = "XtraReports";
		public override Guid Kind {
			get { return ReportToolItemKindHelper.Menu; }
		}
		public ReportMenu(IServiceProvider serviceProvider) :
			base(reportsMenuName, serviceProvider) {
		}
	}
	public abstract class ReportToolWindowItemBase : IToolItem {
		protected class StaticContainer {
			public Panel panel;
			public Control control;
			public int usageCounter;
			public IVSToolWindow toolWindow = new VSDummyToolWindow();
		}
		protected IServiceProvider serviceProvider;
		protected IVSToolWindowService toolWindowService;
		string caption;
		string bitmapResourceName;
		Guid kind;
		Guid toolWindowGuid;
		TreeListController controller;
		TreeListController Controller {
			get {
				if(controller == null && Control is ISupportController)
					controller = ((ISupportController)Control).CreateController(serviceProvider);
				return controller;
			}
		}
		public ReportToolWindowItemBase(IServiceProvider serviceProvider, string caption, string bitmapResourceName, Guid kind, Guid toolWindowGuid) {
			this.caption = caption;
			this.serviceProvider = serviceProvider;
			this.bitmapResourceName = bitmapResourceName;
			this.toolWindowService = (IVSToolWindowService)serviceProvider.GetService(typeof(IVSToolWindowService));
			this.kind = kind;
			this.toolWindowGuid = toolWindowGuid;
		}
		protected abstract StaticContainer Container { get; }
		Panel Panel {
			get { return Container.panel; }
			set { Container.panel = value; }
		}
		protected Control Control {
			get { return Container.control; }
			private set { Container.control = value; }
		}
		int UsageCounter {
			get { return Container.usageCounter; }
			set { Container.usageCounter = value; }
		}
		public Guid Kind {
			get { return kind; }
		}
		Guid ToolWindowGuid {
			get { return toolWindowGuid; }
		}
		protected Control HostedControl {
			get { return Panel; }
			set { Panel = (Panel)value; }
		}
		protected abstract Control CreateControl(IServiceProvider serviceProvider);
		IVSToolWindow CreateToolWindow() {
			if(toolWindowService != null)
				return toolWindowService.Create(serviceProvider, caption, ToolWindowGuid);
			XRVSToolWindow toolWindow = new XRVSToolWindow(serviceProvider, caption, ToolWindowGuid, bitmapResourceName);
			return toolWindow;
		}
		public void InitTool() {
			CreateHostedControl();
			if(ToolWindow != null && !ToolWindow.IsValid)
				ToolWindow = CreateToolWindow();
			if(ToolWindow is XRVSToolWindow) {
				VSMenuService menuService = serviceProvider.GetService(typeof(VSMenuService)) as VSMenuService;
				if(menuService != null)
					menuService.RegisterMenuItem(new ToolWindowMenuItem(caption, bitmapResourceName, ToolWindow as XRVSToolWindow));
			}
			ToolWindow.HostControl(HostedControl);
		}
		IVSToolWindow ToolWindow {
			get { return Container.toolWindow; }
			set { if(value != null) Container.toolWindow = value; }
		}
		void CreateHostedControl() {
			if(Panel == null) {
				Panel = new Panel();
				if(Control == null || Control.IsDisposed) {
					Control = CreateControl(serviceProvider);
					SetLookAndFeel(serviceProvider);
				}
				Panel.Controls.Add(Control);
				Control.Dock = DockStyle.Fill;
			}
			UsageCounter++;
			UpdateView();
		}
		protected virtual void SetLookAndFeel(IServiceProvider serviceProvider) {
			DesignLookAndFeelHelper.SetParentLookAndFeel((DevExpress.LookAndFeel.ISupportLookAndFeel)Control, serviceProvider);
		}
		public void UpdateView() {
			if(Controller == null)
				return;
			Controller.CaptureTreeList(Control);
			Controller.UpdateTreeList();
		}
		public void Dispose() {
			if(controller != null)
				controller.Dispose();
			if(Control != null && --UsageCounter <= 0) {
				Control.Dispose();
				Control = null;
			}
			GC.SuppressFinalize(this);
		}
		public void Hide() {
			ToolWindow.Hide(serviceProvider);
		}
		public void Close() {
			ToolWindow.CloseFrame(serviceProvider);
			HostedControl.Controls.Clear();
			HostedControl.PerformLayout(); 
			HostedControl.Dispose();
			HostedControl = null;
		}
		public void ShowNoActivate() {
			ToolWindow.ShowNoActivate(serviceProvider);
		}
		public void ShowActivate() {
			if(ToolWindow is XRVSToolWindow)
				((XRVSToolWindow)ToolWindow).ShowPersistently();
		}
	}
	public class ReportExplorerToolItem : ReportToolWindowItemBase {
		static StaticContainer container = new StaticContainer();
		protected override StaticContainer Container { get { return container; } }
		public ReportExplorerToolItem(IServiceProvider serviceProvider, string caption)
			: base(serviceProvider, caption, "XRDesignReportExplorer.bmp", ReportToolItemKindHelper.ReportExplorer, new Guid("D562C6A0-D2FE-4c19-A8B3-E5CBC09BF375")) {
		}
		protected override Control CreateControl(IServiceProvider serviceProvider) {
			return new ReportTreeView(serviceProvider);
		}
	}
	public class ReportFieldListItem : ReportToolWindowItemBase {
		static StaticContainer container = new StaticContainer();
		protected override StaticContainer Container { get { return container; } }
		public ReportFieldListItem(IServiceProvider serviceProvider, string caption)
			: base(serviceProvider, caption, "XRDesignFieldList.bmp", ReportToolItemKindHelper.FieldList, new Guid("5E929736-0E81-4d52-A78E-546A1649FACA")) {
		}
		protected override Control CreateControl(IServiceProvider serviceProvider) {
			return new FieldListTreeView(serviceProvider);
		}
	}
	public class GroupAndSortToolItem : ReportToolWindowItemBase {
		static StaticContainer container = new StaticContainer();
		protected override StaticContainer Container { get { return container; } }
		public GroupAndSortToolItem(IServiceProvider serviceProvider, string caption)
			: base(serviceProvider, caption, "XRDesignGroupAndSort.bmp", ReportToolItemKindHelper.GroupAndSort, new Guid("EC2C1C6C-CECC-4f7a-869E-9A8F55540944")) {
		}
		protected override Control CreateControl(IServiceProvider serviceProvider) {
			return new GroupSortUserControl();
		}
		protected override void SetLookAndFeel(IServiceProvider serviceProvider) {
			((GroupSortUserControl)Control).SetLookAndFeel(serviceProvider);
		}
	}
	public class ErrorListToolItem : IToolItem {
		IServiceProvider servProvider;
		ErrorListProvider errorListProvider;
		public ErrorListProvider ErrorListProvider {
			get {
				if(errorListProvider == null) {
					Microsoft.VisualStudio.OLE.Interop.IServiceProvider dte = servProvider.GetService(typeof(DTE)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
					if(dte != null) {
						ServiceProvider serviceProvider = new ServiceProvider(dte);
						errorListProvider = new ErrorListProvider(serviceProvider);
					}
				}
				return errorListProvider;
			}
		}
		public ErrorListToolItem(IServiceProvider servProvider) {
			this.servProvider = servProvider;
		}
		void IToolItem.Close() {
		}
		void IToolItem.Hide() {
		}
		void IToolItem.InitTool() {
		}
		Guid IToolItem.Kind {
			get { return ReportToolItemKindHelper.ErrorList; }
		}
		void IToolItem.ShowActivate() {
			if(ErrorListProvider != null && HasErrors) {
				ErrorListProvider.Show();
				ErrorListProvider.BringToFront();
			}
		}
		void IToolItem.ShowNoActivate() {
			if(!Visible && ErrorListProvider != null && HasErrors)
				ErrorListProvider.Show();
		}
		bool Visible {
			get {
				Window window = GetErrorListWindow();
				return window != null && window.Visible;
			}
		}
		Window GetErrorListWindow() {
			const string vsWindowKindErrorList = "{D78612C7-9962-4B83-95D9-268046DAD23A}";
			_DTE dte = servProvider.GetService(typeof(DTE)) as _DTE;
			try {
				if(dte != null)
					return dte.Windows.Item(vsWindowKindErrorList);
			} catch {
			}
			return null;
		}
		void IToolItem.UpdateView() {
			if(ErrorListProvider == null) return;
			ClearErrors();
			try {
				IDesignerHost designerHost = servProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				CompilerErrorCollection errors = new DesignerHostExtensions(designerHost).ValidateReportScripts();
				ScriptControl scriptControl = (ScriptControl)servProvider.GetService(typeof(ScriptControl));
				if(errors != null && scriptControl != null) {
					Window window = GetDesignWindow();
					string document = window != null ? window.Caption : designerHost.RootComponent.Site.Name;
					foreach(CompilerError error in errors) {
						ErrorTask task = new XRErrorTask(this)
						{
							ErrorCategory = TaskErrorCategory.Error,
							Document = document,
							Text = error.ErrorText,
							Line = Math.Max(0, error.GetValidLine(scriptControl.LinesCount) - 1),
							Column = Math.Max(0, error.GetValidColumn(scriptControl.LinesCount) - 1),
							CanDelete = true
						};
						task.Navigate += new EventHandler(task_Navigate);
						task.Removed += new EventHandler(task_Removed);
						ErrorListProvider.Tasks.Add(task);
					}
					scriptControl.ShowErrors(errors);
				}
			} catch(Exception ex) {
				if(ex is DevExpress.XtraReports.Serialization.XRSerializationException) {
				}
			}
			if(!Visible && HasErrors)
				ErrorListProvider.Show();
		}
		Window GetDesignWindow() {
			ProjectItem projectItem = (ProjectItem)servProvider.GetService(typeof(ProjectItem));
			return projectItem != null ? GetDesignWindow(projectItem.Document.Windows) : null;
		}
		Window GetDesignWindow(Windows windows) {
			try {
				for(int i = 1; i <= windows.Count; i++) {
					if(windows.Item(i).Object is IDesignerHost)
						return windows.Item(i);
				}
			} catch {
			}
			return null;
		}
		void task_Removed(object sender, EventArgs e) {
			ErrorTask task = (ErrorTask)sender;
			task.Navigate -= new EventHandler(task_Navigate);
			task.Removed -= new EventHandler(task_Removed);
		}
		void task_Navigate(object sender, EventArgs e) {
			ProjectItem projectItem = (ProjectItem)servProvider.GetService(typeof(ProjectItem));
			if(projectItem != null) {
				Window window = GetDesignWindow(projectItem.Document.Windows);
				if(window != null)
					window.Activate();
			}
			ReportTabControl tabControl = servProvider.GetService(typeof(ReportTabControl)) as ReportTabControl;
			ScriptControl scriptControl = servProvider.GetService(typeof(ScriptControl)) as ScriptControl;
			if(tabControl != null && scriptControl != null) {
				tabControl.SelectedIndex = TabIndices.Scripts;
				scriptControl.SetCaretPosition(((ErrorTask)sender).Line, ((ErrorTask)sender).Column);
			}
		}
		void Execute(Action action) {
			if(ErrorListProvider != null) {
				try {
					ErrorListProvider.SuspendRefresh();
					action();
				} finally {
				}
			}
		}
		void ClearErrors() {
			try {
				ErrorListProvider.SuspendRefresh();
				TaskProvider.TaskCollection tasks = ErrorListProvider.Tasks;
				for(int i = tasks.Count - 1; i >= 0; i--)
					if(tasks[i] is XRErrorTask && ReferenceEquals(((XRErrorTask)tasks[i]).Owner, this))
						tasks.RemoveAt(i);
			} finally {
				ErrorListProvider.ResumeRefresh();
			}
		}
		bool HasErrors {
			get {
				if(ErrorListProvider != null) {
					TaskProvider.TaskCollection tasks = ErrorListProvider.Tasks;
					for(int i = tasks.Count - 1; i >= 0; i--)
						if(tasks[i] is XRErrorTask && ReferenceEquals(((XRErrorTask)tasks[i]).Owner, this))
							return true;
				}
				return false;
			}
		}
		void IDisposable.Dispose() {
			if(ErrorListProvider != null)
				ClearErrors();
		}
	}
	class XRErrorTask : ErrorTask {
		public object Owner { get; private set; }
		public XRErrorTask(object owner) {
			Owner = owner;
		}
	}
	public class FakeAddIn : AddIn {
		public static readonly AddIn Instance = new FakeAddIn();
		#region AddIn Members
		AddIns AddIn.Collection {
			get { throw new NotImplementedException(); }
		}
		bool AddIn.Connected {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		DTE AddIn.DTE {
			get { throw new NotImplementedException(); }
		}
		string AddIn.Description {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		string AddIn.Guid {
			get { throw new NotImplementedException(); }
		}
		string AddIn.Name {
			get { throw new NotImplementedException(); }
		}
		object AddIn.Object {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		string AddIn.ProgID {
			get { return "FakeProgId"; }
		}
		void AddIn.Remove() {
			throw new NotImplementedException();
		}
		string AddIn.SatelliteDllPath {
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
}
