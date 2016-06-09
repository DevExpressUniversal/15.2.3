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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Native;
using DevExpress.Design.VSIntegration;
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Skins;
using DevExpress.Utils.About;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Templates;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UserDesigner.Native;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel.Design.Serialization;
using DevExpress.DataAccess.UI.Native.Sql;
using DevExpress.DataAccess.Wizard;
using DevExpress.XtraReports.Wizards3;
namespace DevExpress.XtraReports.Design {
	public class _ReportDesigner : ReportDesigner, IToolShellProvider {
		#region inner classes
		class DebugService : IDebugService {
			IServiceProvider servProvider;
			public DebugService(IServiceProvider servProvider) {
				this.servProvider = servProvider;
			}
			bool IDebugService.IsDebugging {
				get {
					IVsMonitorSelection monitorSelectionService = (IVsMonitorSelection)servProvider.GetService(typeof(IVsMonitorSelection));
					if(monitorSelectionService != null) {
						uint debuggingCookie;
						Guid rguidCmdUI = new Guid("{48EA4A80-F14E-4107-88FA-8D0016F30B9C}");
						monitorSelectionService.GetCmdUIContextCookie(ref rguidCmdUI, out debuggingCookie);
						int pfActive = 1;
						monitorSelectionService.IsCmdUIContextActive(debuggingCookie, out pfActive);
						return pfActive == 0;
					}
					return false;
				}
			}
		}
		class VSUndoService : IUndoService {
			IServiceProvider servProvider;
			public VSUndoService(IServiceProvider servProvider) {
				this.servProvider = servProvider;
			}
			public void ClearUndoStack() {
				try {
					Microsoft.VisualStudio.OLE.Interop.IOleUndoManager manager = servProvider.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IOleUndoManager)) as Microsoft.VisualStudio.OLE.Interop.IOleUndoManager;
					if(manager != null)
						manager.DiscardFrom(null);
				} catch {
				}
			}
		}
		class WindowsFormsEditorService : ReplaceableService<IWindowsFormsEditorService>, IWindowsFormsEditorService {
			public WindowsFormsEditorService(IWindowsFormsEditorService baseServ, IDesignerHost host)
				: base(baseServ, host) {
			}
			public void CloseDropDown() {
				OriginalService.CloseDropDown();
			}
			public void DropDownControl(Control control) {
				OriginalService.DropDownControl(control);
			}
			public DialogResult ShowDialog(Form dialog) {
				if(dialog is ISupportLookAndFeel)
					LookAndFeelProviderHelper.SetParentLookAndFeel((ISupportLookAndFeel)dialog, DesignerHost);
				return OriginalService.ShowDialog(dialog);
			}
		}
		class PropertyFilterService : IPropertyFilterService {
			public void PreFilterProperties(System.Collections.IDictionary properties, object component) {
				if(properties.Contains(XRComponentPropertyNames.DataSource)) {
					PropertyDescriptor oldPropertyDescriptor = (PropertyDescriptor)properties[XRComponentPropertyNames.DataSource];
					properties[XRComponentPropertyNames.DataSource] = XRAccessor.CreateProperty(oldPropertyDescriptor.ComponentType, oldPropertyDescriptor,
						new Attribute[] { new EditorAttribute("DevExpress.XtraReports.Design.DataSourceEditorVS," + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(UITypeEditor)) });
				}
				if(properties.Contains(XRComponentPropertyNames.DataAdapter)) {
					PropertyDescriptor oldPropertyDescriptor = (PropertyDescriptor)properties[XRComponentPropertyNames.DataAdapter];
					properties[XRComponentPropertyNames.DataAdapter] = XRAccessor.CreateProperty(oldPropertyDescriptor.ComponentType, oldPropertyDescriptor,
						new Attribute[] { new EditorAttribute("DevExpress.XtraReports.Design.DataAdapterEditorVS," + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(UITypeEditor)) });
				}
			}
			public void PreFilterEvents(IDictionary events, object component) { }
		}
		protected class ReportSourcePickerVS : ReportSourcePicker {
			Type rootType;
			public ReportSourcePickerVS(Type rootType) {
				this.rootType = rootType;
			}
			protected override string[] GetTypeNames() {
				IDesignerHost host = (IDesignerHost)context.GetService(typeof(IDesignerHost));
				System.Diagnostics.Debug.Assert(host != null);
				try {
					IDTEService dteService = (IDTEService)context.GetService(typeof(IDTEService));
					if(dteService != null)
						return dteService.GetClassesInfo(rootType, new string[] { host.RootComponentClassName });
				} catch {
				}
				return new string[] { };
			}
		}
		protected class TypeComparer : IComparer {
			public int Compare(object x, object y) {
				return Comparer.Default.Compare(((Type)x).Name, ((Type)y).Name);
			}
		}
		private class VSDialogRunner : DialogRunner {
			IUIService uiService;
			public VSDialogRunner(IUIService uiService) {
				this.uiService = uiService;
			}
			protected override DialogResult RunDialog(CommonDialog dialog, IWin32Window ownerWindow) {
				DialogResult dialogResult = DialogResult.Cancel;
				lock(typeof(DialogRunner)) {
					IntPtr focus = Win32.GetFocus();
					try {
						dialogResult = dialog.ShowDialog(ownerWindow);
					} catch(Exception ex) {
						if(ex != null && uiService != null)
							uiService.ShowError(ex);
					} finally {
						if(focus != IntPtr.Zero)
							Win32.SetFocus(new System.Runtime.InteropServices.HandleRef(null, focus));
					}
				}
				return dialogResult;
			}
			protected override IWin32Window GetDefaultOwnerWindow() {
				IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
				return owner ?? base.GetDefaultOwnerWindow();
			}
		}
		protected class VSNewParameterEditorForm : NewParameterEditorForm {
			public VSNewParameterEditorForm(IDesignerHost designerHost) : base(designerHost) { }
			[Editor(typeof(DataSourceEditorVS), typeof(UITypeEditor)), TypeConverter(typeof(DevExpress.Data.Design.DataSourceConverter))]
			public override object DataSource {
				get {
					return base.DataSource;
				}
				set {
					base.DataSource = value;
				}
			}
			[Editor(typeof(DataAdapterEditorVS), typeof(UITypeEditor)), TypeConverter(typeof(DataAdapterConverter))]
			public override object DataAdapter {
				get {
					return base.DataAdapter;
				}
				set {
					base.DataAdapter = value;
				}
			}
		}
		class ApplicationPathService : IApplicationPathService {
			IDesignerHost designerHost;
			public ApplicationPathService(IDesignerHost designerHost) {
				this.designerHost = designerHost;
			}
			#region IApplicationPathService Members
			public string[] GetBinDirectories() {
				const string STR_OutputPath = "OutputPath";
				const string STR_FullPath = "FullPath";
				const string STR_BinDirectory = @"\Bin";
				const string projectTypeGuidWebSite = "{E24C65DC-7377-472b-9ABA-BC803B73C61A}";
				ProjectItem projectItem = designerHost.GetService(typeof(ProjectItem)) as ProjectItem;
				if(projectItem == null)
					return new string[0];
				EnvDTE.Project project = projectItem.ContainingProject;
				if(project == null || project.ConfigurationManager == null)
					return new string[0];
				EnvDTE.Configuration activeConfiguration = project.ConfigurationManager.ActiveConfiguration;
				string binDirectory = string.Empty;
				string fullPath = project.Properties.Item(STR_FullPath).Value.ToString();
				if(activeConfiguration != null && activeConfiguration.Properties != null) {
					string outputPath = activeConfiguration.Properties.Item(STR_OutputPath).Value.ToString();
					binDirectory = Path.Combine(fullPath, outputPath);
				} else {
					Regex regex = new Regex(projectTypeGuidWebSite, RegexOptions.IgnoreCase);
					if(regex.IsMatch(project.Kind))
						binDirectory = fullPath + STR_BinDirectory;
				}
				return !string.IsNullOrEmpty(binDirectory) ? new string[] { binDirectory } : new string[0];
			}
			#endregion
		}
		#endregion
		static ToolShellController reportToolController;
		DesignerExtenders designerExtenders;
		CommandEvents commandEvents;
		public override bool IsActive {
			get {
				return base.IsActive || IsActiveDocument;
			}
		}
		bool IsActiveDocument {
			get {
				ProjectItem projectItem = GetService(typeof(ProjectItem)) as ProjectItem;
				return projectItem != null && projectItem.DTE != null && projectItem.DTE.ActiveDocument != null ? 
					Reference.Equals(projectItem.DTE.ActiveDocument.ProjectItem, projectItem) : 
					false;
			}
		}
		DevExpress.Data.Utils.IToolShell IToolShellProvider.ToolShell {
			get { return reportTool; }
		}
		static _ReportDesigner() {
			SkinManager.EnableFormSkins();
			DevExpress.DataAccess.Sql.SqlDataSource.DisableCustomQueryValidation = true;
		}
		public _ReportDesigner()
			: base() {
			DialogRunner.Instance = new VSDialogRunner(null);
			DevExpress.XtraReports.Native.Compiler.IncludeAppDomainReferences = false;
		}
		protected override bool ShouldSaveToLayout(IDataContainer dataContainer) {
			return false;
		}
		protected virtual void OnAbout() {
		}
		protected override ITemplateProvider GetTemplateProvider() {
			return base.GetTemplateProvider() ?? DXTemplateProvider.CreateReportTemplateProvider();
		}
		public override TypePickerBase CreateReportSourcePicker() {
			return new ReportSourcePickerVS(typeof(XtraReport));
		}
		protected virtual void OnResetToolbox() {
			Cursor currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try {
				string category = AssemblyInfo.DXTabReportControls;
				IToolboxService toolboxSvc = GetService(typeof(IToolboxService)) as IToolboxService;
				ToolboxItemCollection items = toolboxSvc.GetToolboxItems(category);
				foreach(ToolboxItem item in items)
					toolboxSvc.RemoveToolboxItem(item);
				ToolboxItem[][] subcategorizedItems = XRToolboxService.GroupItemsBySubCategory(new ToolboxItemCollection(ToolboxHelper.XRToolboxItems), fDesignerHost);
				foreach(ToolboxItem[] subcategory in subcategorizedItems)
					for(int i = 0; i < subcategory.Length; i++) {
						ToolboxItem toolboxItem = subcategory[i];
						if(CanAddToolboxItem(toolboxItem))
							toolboxSvc.AddToolboxItem(toolboxItem, category);
					}
				toolboxSvc.SelectedToolboxItemUsed();
			} finally {
				Cursor.Current = currentCursor;
			}
		}
		protected virtual bool CanAddToolboxItem(ToolboxItem toolboxItem) {
			return true;
		}
		protected override void ActivateMenuCommands() {
			base.ActivateMenuCommands();
			MenuCommandHandler commandHandler = fDesignerHost.GetService(typeof(MenuCommandHandler)) as MenuCommandHandler;
			if(commandHandler == null)
				return;
			CommandID viewCode;
			IXRCommandIdService commandIdService = (IXRCommandIdService)fDesignerHost.GetService(typeof(IXRCommandIdService));
			if(commandIdService == null) {
				const int cmdidViewCode = 333;
				viewCode = new CommandID(FormattingCommands.EnvCommandSet, cmdidViewCode);
			} else {
				viewCode = commandIdService.ViewCodeCommandId;
			}
			IMenuCommandService menuService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			commandHandler.AddCommand(new MenuCommandHandler.CommandSetItemWrapper(menuService, new EventHandler(commandHandler.OnStatusAnySelection), viewCode, WrappedCommands.ViewCode));
		}
		protected override void ValidateDataAdapter() {
			new VSDataContainerDataAdapterHelper(this).ValidateDataAdapter();
		}
		protected override string[] GetFilteredProperties() {
			if((ProjectHelper.IsWebProject(fDesignerHost) || ProjectHelper.IsWebApplication(fDesignerHost)) && !ProjectHelper.IsLightSwitchProject(fDesignerHost))
				return new string[] { "Localizable", "Language", "LoadLanguage" };
			return new string[0];
		}
		protected override DevExpress.Data.Utils.IToolShell GetReportTool(IServiceProvider srvProvider) {
			DevExpress.Data.Utils.IToolShell toolShell = new DevExpress.Data.Utils.ToolShell();
			FillReportToolShell(srvProvider, toolShell);
			this.fDesignerHost.AddService(typeof(DevExpress.Data.Utils.IToolShell), toolShell);
			return base.GetReportTool(srvProvider);
		}
		protected virtual void FillReportToolShell(IServiceProvider srvProvider, DevExpress.Data.Utils.IToolShell toolShell) {
			toolShell.AddToolItem(new ReportFieldListItem(srvProvider, VSDesignSR.MenuItem_FieldList));
			toolShell.AddToolItem(new ReportExplorerToolItem(srvProvider, VSDesignSR.MenuItem_ReportExplorer));
			toolShell.AddToolItem(new GroupAndSortToolItem(srvProvider, VSDesignSR.MenuItem_GroupAndSort));
			toolShell.AddToolItem(new ErrorListToolItem(srvProvider));
			toolShell.AddToolItem(new ReportFormattingBar(srvProvider));
			toolShell.AddToolItem(new ReportMenu(srvProvider));
		}
		protected override void InitializeServiceHelper() {
			base.InitializeServiceHelper();
			IMenuCommandService menuCommandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
			if(menuCommandService != null) {
				fDesignerHost.RemoveService(typeof(IMenuCommandService));
				XRMenuCommandService xrMenuCommandService = new XRMenuCommandService(menuCommandService, this.ReportFrame, fDesignerHost);
				servHelper.AddService(typeof(IMenuCommandService), xrMenuCommandService);
				servHelper.AddService(typeof(IXRMenuCommandService), xrMenuCommandService);
			}
			IWindowsFormsEditorService serv = (IWindowsFormsEditorService)GetService(typeof(IWindowsFormsEditorService));
			if(serv != null) {
				fDesignerHost.RemoveService(typeof(IWindowsFormsEditorService));
				IWindowsFormsEditorService newServ = new WindowsFormsEditorService(serv, fDesignerHost);
				servHelper.AddService(typeof(IWindowsFormsEditorService), newServ);
			}
			servHelper.AddService(typeof(IPropertyFilterService), CreatePropertyFilterService());
			servHelper.AddService(typeof(IApplicationPathService), new ApplicationPathService(fDesignerHost));
			IProjectEnvironmentService projectEnvironmentService = new ProjectEnvironmentService(fDesignerHost);
			ProjectEnvironmentHelper.RegisterProjectEnvironmentService(projectEnvironmentService);
			servHelper.AddService(typeof(IOutputService), new DTEOutputService(fDesignerHost.GetService(typeof(DTE)) as _DTE));
			servHelper.ReplaceService(typeof(IUndoService), new VSUndoService(fDesignerHost));
			servHelper.AddService(typeof(IDebugService), new DebugService(fDesignerHost));
			servHelper.ReplaceService(typeof(ISqlWizardOptionsProvider), new SqlWizardOptionsProvider(() => SqlWizardOptions.EnableCustomSql));
		}
		protected virtual IPropertyFilterService CreatePropertyFilterService() {
			return new PropertyFilterService();
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new RootReportDesignerActionList4(this));
			base.RegisterActionLists(list);
		}
		protected override void InitializeCore(IComponent component) {
			base.InitializeCore(component);
			DialogRunner.Instance = new VSDialogRunner(fDesignerHost.GetService<IUIService>());
			fDesignerHost.AddService(typeof(DevExpress.Data.Utils.IConnectionStringsService), new ConnectionStringsService(fDesignerHost));
			fDesignerHost.ReplaceService<ISolutionTypesProvider>(new PureSolutionTypesProvider());
			IDTEService dteService = fDesignerHost.GetService(typeof(IDTEService)) as IDTEService;
			if(dteService == null)
				fDesignerHost.AddService(typeof(IDTEService), new DTEService(fDesignerHost));
			VSMenuService menuService = fDesignerHost.GetService(typeof(VSMenuService)) as VSMenuService;
			if(menuService != null) {
				DesignerVerb verb = DevExpress.Utils.Design.DXSmartTagsHelper.CreateSupportVerb(this);
				if(verb != null)
					menuService.RegisterMenuItem(new CommandMenuItem(verb.Text, InvokeSupportVerb) { BeginGroup = true });
#if DEBUG
				menuService.RegisterMenuItem(new CommandMenuItem(VSDesignSR.MenuItem_ResetToolbox, OnResetToolbox));
#endif
				menuService.RegisterMenuItem(new CommandMenuItem(VSDesignSR.MenuItem_About, OnAbout) { BeginGroup = true });
			}
			INameCreationService nameService = fDesignerHost.GetService<INameCreationService>();
			if(nameService != null) {
				fDesignerHost.ReplaceService<INameCreationService>(new VSNameCreationService(nameService, fDesignerHost.Container));
			}
		}
		void InvokeSupportVerb() {
			DesignerVerb verb = DevExpress.Utils.Design.DXSmartTagsHelper.CreateSupportVerb(this);
			if(verb != null)
				verb.Invoke();
		}
		protected override void OnComponentAdded(object source, ComponentEventArgs e) {
			base.OnComponentAdded(source, e);
			ValidateDataSource(e.Component);
		}
		void ValidateDataSource(IComponent component) {
			if(!BindingHelper.IsListSource(component))
				return;
			IDataAdapter dataAdapter = GetSelectedComponent() as IDataAdapter;
			if(dataAdapter == null)
				return;
			XtraReportBase reportBase = FindReportBase(dataAdapter);
			if(reportBase != null) {
				reportBase.DataSource = component;
				SetDataMember(reportBase, component);
			}
		}
		XtraReportBase FindReportBase(object dataAdapter) {
			if(RootReport.DataAdapter == dataAdapter)
				return RootReport;
			NestedComponentEnumerator en = new NestedComponentEnumerator(RootReport.Bands);
			while(en.MoveNext()) {
				XtraReportBase reportBase = en.Current as XtraReportBase;
				if(reportBase == null)
					continue;
				if(reportBase.DataAdapter == dataAdapter)
					return reportBase;
			}
			return null;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(commandEvents != null) {
					commandEvents.BeforeExecute -= BeforeCommandExecute;
					commandEvents = null;
				}
				DisposeObject(designerExtenders);
				designerExtenders = null;
				fDesignerHost.RemoveService(typeof(IInheritanceService));
				fDesignerHost.RemoveService(typeof(IDTEService));
				fDesignerHost.RemoveService(typeof(DevExpress.Data.Utils.IConnectionStringsService));
				fDesignerHost.RemoveService(typeof(DevExpress.Data.Utils.IToolShell));
				fDesignerHost.RemoveService(typeof(ISolutionTypesProvider));
			}
		}
		protected override void OnImportException(Exception ex) {
			using(CustomLocalizer localizer = new CustomLocalizer()) {
				localizer.SetLocalizedString(XtraEditors.Controls.StringId.XtraMessageBoxOkButtonText, "Help");
				localizer.SetLocalizedString(XtraEditors.Controls.StringId.XtraMessageBoxCancelButtonText, "Close");
				if(DialogResult.OK == NotificationService.ShowMessage<XtraReport>(LookAndFeelProviderHelper.GetLookAndFeel(fDesignerHost), fDesignerHost.GetOwnerWindow(),
					"An error occurred during the import. See details in the Output window.", ReportLocalizer.GetString(ReportStringId.Msg_ErrorTitle), MessageBoxButtons.OKCancel, MessageBoxIcon.Error))
					ProcessLaunchHelper.StartProcess("http://isc.devexpress.com/Thread/WorkplaceDetails/K18371");
			}
		}
		protected override void OnDesignerActivate(object sender, System.EventArgs e) {
			IToolboxService tbxService = GetService(typeof(IToolboxService)) as IToolboxService;
			if(tbxService != null)
				try {
					tbxService.SelectedCategory = GetToolboxSelectedCategory();
				} catch { }
			base.OnDesignerActivate(sender, e);
		}
		protected virtual string GetToolboxSelectedCategory() {
			return AssemblyInfo.DXTabReportControls;
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			properties[XRComponentPropertyNames.ScriptSecurityPermissions] = XtraReports.Native.XRAccessor.CreateProperty(typeof(XtraReport),
				(PropertyDescriptor)properties[XRComponentPropertyNames.ScriptSecurityPermissions],
				new Attribute[] { new System.ComponentModel.BrowsableAttribute(true), new EditorAttribute(typeof(ScriptSecurityPermissionEditor), typeof(System.Drawing.Design.UITypeEditor)) });
		}
		protected override ILookAndFeelService CreateLookAndFeelService() {
			return new VSLookAndFeelService(fDesignerHost);
		}
		protected override LockService CreateLockService() {
			return new VSLockService(fDesignerHost);
		}
		protected override InheritanceHelperService GetInheritanceHelperService() {
			return new VSInheritanceHelperService(fDesignerHost);
		}
		protected override void OnLoadComplete(object sender, System.EventArgs e) {
			base.OnLoadComplete(sender, e);
			RemoveReportSourcesFromHost();
			_DTE dte = fDesignerHost.GetService(typeof(DTE)) as _DTE;
			if(dte != null) {
				commandEvents = dte.Events.get_CommandEvents("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 0) as EnvDTE.CommandEvents;
				if(commandEvents != null)
					commandEvents.BeforeExecute += BeforeCommandExecute;
			}
			IOutputService serv = fDesignerHost.GetService<IOutputService>();
			if(serv == null) return;
			EnsureTraceListener(NativeSR.TraceSource, serv);
			IList<string> errors = GetErrors();
			foreach(var error in errors)
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, error);
		}
		static void EnsureTraceListener(string traceSource, IOutputService serv) {
			const string name = "DXperience.Listener.Design";
			TraceSource ts = DevExpress.XtraPrinting.Tracer.GetSource(traceSource, SourceLevels.Error | SourceLevels.Warning | SourceLevels.Information | SourceLevels.ActivityTracing);
			TraceListener listener = ts.Listeners[name];
			if(listener == null)
				ts.Listeners.Add(new DesignTraceListener(serv, name));
		}
		IList<string> GetErrors() {
			const string format = "The {0} band can't be displayed, because it doesn't have a corresponding field in the report class. To avoid this error, set its GenerateMember property to True.";
			List<string> errors = new List<string>();
			foreach(Band band in RootReport.Bands) {
				if(band.Site == null) {
					string item = string.Format(format, band.Name);
					errors.Add(item);
				}
			}
			return errors;
		}
		void BeforeCommandExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault) {
			const int fileSaveAll = 224;
			const int fileSaveSelectedItems = 331;
			const int buildBuildSolution = 882;
			const int buildRebuildSolution = 883;
			const int classViewProjectBuild = 892;
			const int classViewProjectRebuild = 893;
			if(!IsActive) return;
			if(Array.IndexOf<int>(new int[] { fileSaveSelectedItems, fileSaveAll }, ID) >= 0)
				new DesignerHostExtensions(fDesignerHost).CommitInplaceEditors();
			else if(Array.IndexOf<int>(new int[] { buildBuildSolution, buildRebuildSolution, classViewProjectBuild, classViewProjectRebuild }, ID) >= 0) {
				ScriptControl scriptControl = GetService(typeof(ScriptControl)) as ScriptControl;
				if(scriptControl != null)
					scriptControl.ValidateScript();
			}
		}
		void RemoveReportSourcesFromHost() {
			NestedComponentEnumerator en = new NestedComponentEnumerator(RootReport.Bands);
			while(en.MoveNext()) {
				if(en.Current is SubreportBase) {
					XtraReport report = ((SubreportBase)en.Current).ReportSource;
					if(report != null && report.Site != null) {
						PropertyDescriptor generateMember = TypeDescriptor.GetProperties(report)["GenerateMember"];
						if(generateMember != null)
							generateMember.SetValue(report, false);
						fDesignerHost.Container.Remove(report);
					}
				}
			}
		}
		protected override void ActivateReportTool() {
			if(reportToolController == null)
				reportToolController = new ToolShellController(fDesignerHost);
		}
		protected override void DeactivateReportTool() {
		}
		protected override NewParameterEditorForm CreateNewParameterEditorForm() {
			return new VSNewParameterEditorForm(fDesignerHost);
		}
	}
	class RootReportDesignerActionList4 : XRComponentDesignerActionList {
		class ResourceWriterBuilder : DevExpress.Utils.Serializers.MultiTargetBuilder, IResourceWriterBuilder {
			public ResourceWriterBuilder(IServiceProvider provider) : base(provider) { }
			protected override ConstructorInfo CreateConstructor(Type delegateType) {
				return null;
			}
			public ResourceWriter CreateResourceWriter(MemoryStream stream) {
				ResourceWriter writer = new ResourceWriter(stream);
				if(!skipMultitargetService) {
					writer.GetType().GetProperty("TypeNameConverter").SetValue(writer, typeNameConverter, null);
				}
				return writer;
			}
		}
		public RootReportDesignerActionList4(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			actionItems.Add(new DesignerActionMethodItem(this, "Import", DesignSR.Verb_Import, NativeSR.CatAction));
			actionItems.Add(new DesignerActionMethodItem(this, "Save", DesignSR.Verb_Save, NativeSR.CatAction));
		}
		void Save() {
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			XtraReport report = host != null ? (XtraReport)host.RootComponent : null;
			if(report == null) return;
			using(SaveFileDialog fileDialog = DevExpress.XtraReports.UserDesigner.XRDesignPanel.CreateSaveFileDialog(report, "", report.Name)) {
				fileDialog.Filter = "Report XML Files (*.repx)|*.repx|Report Files (*.repx)|*.repx|All Files (*.*)|*.*";
				if(fileDialog.ShowDialog() != DialogResult.OK) return;
				DevExpress.Data.Utils.IConnectionStringsService svc = (DevExpress.Data.Utils.IConnectionStringsService)GetService(typeof(DevExpress.Data.Utils.IConnectionStringsService));
				try {
					if(svc != null) svc.PatchConnection();
					host.AddService(typeof(IResourceWriterBuilder), new ResourceWriterBuilder(host));
					if(fileDialog.FilterIndex == 1)
						report.SaveLayoutToXml(fileDialog.FileName);
					else
						report.SaveLayout(fileDialog.FileName);
				} catch(Exception ex) {
					NotificationService.ShowException<XtraReport>(DevExpress.LookAndFeel.DesignService.LookAndFeelProviderHelper.GetLookAndFeel(host), host.GetOwnerWindow(), ex);
				} finally {
					if(svc != null) svc.RestoreConnection();
					host.RemoveService(typeof(IResourceWriterBuilder));
				}
			}
		}
		void Import() {
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			XtraReport report = host != null ? (XtraReport)host.RootComponent : null;
			if(report != null && Attribute.GetCustomAttribute(report.GetType(), typeof(RootClassAttribute)) == null) {
				NotificationService.ShowMessage<XtraReport>(LookAndFeelProviderHelper.GetLookAndFeel(host), host.GetOwnerWindow(),
					"The open/import operation is not available for an inherited report because of possible\n" +
					"conflicts between bands of the source report and bands of the loaded report layout.", 
					ReportLocalizer.GetString(ReportStringId.Msg_ErrorTitle), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			IMenuCommandService menuCommandService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if(menuCommandService != null)
				menuCommandService.GlobalInvoke(VerbCommands.Import);
		}
	}
	public interface IXRCommandIdService {
		CommandID ViewCodeCommandId { get; }
	}
	public static class VSDesignSR {
		public static readonly string
			MenuItem_FieldList = "Field List",
			MenuItem_ReportExplorer = "Report Explorer",
			MenuItem_GroupAndSort = "Group and Sort",
			MenuItem_ResetToolbox = "Reset Toolbox",
			MenuItem_About = ReportLocalizer.GetString(ReportStringId.Verb_About);
	}
}
