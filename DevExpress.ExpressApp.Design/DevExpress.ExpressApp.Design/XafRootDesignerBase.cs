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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.ExpressApp.Reports.Web;
using DevExpress.Persistent.Base;
using DevExpress.Utils.About;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design.Serialization;
using Microsoft.VisualStudio.Shell.Interop;
using DevExpress.ExpressApp.ReportsV2.Web;
namespace DevExpress.ExpressApp.Design {
	public class ProjectIsNotBuiltException : Exception {
		public ProjectIsNotBuiltException() : base("") { }
		public ProjectIsNotBuiltException(string message) : base(message) { }
		public ProjectIsNotBuiltException(string message, Exception innerException) : base(message, innerException) { }
	}
	public class RootDesignerService {
		private readonly XafRootDesignerBase designer;
		public RootDesignerService(XafRootDesignerBase designer) {
			this.designer = designer;
		}
		public XafRootDesignerBase Designer {
			get { return designer; }
		}
	}
	public class XafDesignerFilterService : ITypeDescriptorFilterService {
		public ITypeDescriptorFilterService oldService = null;
		public XafDesignerFilterService() { }
		public XafDesignerFilterService(ITypeDescriptorFilterService oldService) {
			this.oldService = oldService;
		}
		public bool FilterAttributes(IComponent component, IDictionary attributes) {
			if(oldService != null)
				oldService.FilterAttributes(component, attributes);
			return true;
		}
		public bool FilterEvents(IComponent component, IDictionary events) {
			if(oldService != null)
				oldService.FilterEvents(component, events);
			return true;
		}
		public bool FilterProperties(IComponent component, IDictionary properties) {
			if(oldService != null)
				oldService.FilterProperties(component, properties);
			if(properties.Contains("Localizable")) {
				properties.Remove("Localizable");
			}
			if(properties.Contains("Language")) {
				properties.Remove("Language");
			}
			return true;
		}
	}
	public abstract class XafRootDesignerBase : ComponentDesigner, IRootDesigner, IToolboxUser, IEditorNavigation {
		public static bool Enable_PopupMenu = true;
		public static bool Enable_IVsRunningDocumentTable2 = true;
		public static bool Enable_DTE = true;
		private readonly List<MenuCommand> menuCommands = new List<MenuCommand>();
		private readonly Dictionary<string, ErrorTask> warnings = new Dictionary<string, ErrorTask>();
		private DesignErrorControl errorControl;
		private Guid traceGuid = Guid.Empty;
		private readonly List<string> deserializationErrors = new List<string>();
		private List<string> allowedFilterStrings = null;
		private bool isLastBuildSuceeded = false;
		private EventHandler focusChanged;
		private DTE dte;
		private uint _rdtEventsCookie = 0;
		private ProjectWrapper projectWrapper;
		protected ProjectWrapper ProjectWrapper {
			get {
				if(projectWrapper == null) {
					ProjectItem pItem = (ProjectItem)GetService(typeof(ProjectItem));
					if(pItem == null) {
						throw new Exception("Cannot create project wrapper");
					}
					projectWrapper = ProjectWrapper.Create(pItem.ContainingProject);
				}
				return projectWrapper;
			}
		}
		protected Control view;
		private Assembly ReflectionHelper_AssemblyResolve(object sender, AssemblyResolveEventArgs args) {
			if(TypeResolutionService != null) {
				return TypeResolutionService.GetAssembly(args.AssemblyName);
			}
			return null;
		}
		private void userTypeWarning_Removed(object sender, EventArgs e) {
			((ErrorTask)sender).Removed -= new EventHandler(userTypeWarning_Removed);
			if(warnings.ContainsValue((ErrorTask)sender)) {
				warnings.Remove(((ErrorTask)sender).Text);
			}
		}
		private void errorControl_BuildProjectClicked(object sender, EventArgs e) {
			DesignErrorControl control = (DesignErrorControl)sender;
			try {
				isLastBuildSuceeded = ProjectWrapper.Build();
				if(isLastBuildSuceeded) {
					ReopenDesigner(true);
				}
				else {
					ShowErrorControl("Build failed because of the errors in your code.\r\nThe current and related projects should be built before the Designer can be loaded.\r\nFix the errors and then use the link below to build the projects and reload the Designer.");
				}
			}
			catch(Exception ex) {
				TraceException(ex);
				throw;
			}
		}
		private void ReopenDesigner(bool repitForError) {
			try {
				ProjectItem projectItem = (ProjectItem)GetService(typeof(ProjectItem));
				bool isClosed = false;
				foreach(Document document in projectItem.DTE.Documents) {
					if(document.ProjectItem == projectItem) {
						foreach(EnvDTE.Window currentWindow in document.Windows) {
							IDesignerHost designerHost = currentWindow.Object as IDesignerHost;
							if(designerHost == DesignerHost) {
								currentWindow.Close(vsSaveChanges.vsSaveChangesNo);
								isClosed = true;
								break;
							}
						}
					}
					if(isClosed) {
						break;
					}
				}
				EnvDTE.Window window = projectItem.Open(EnvDTE.Constants.vsViewKindDesigner);
				window.SetFocus();
			}
			catch(Exception e) {
				if(repitForError) {
					ReopenDesigner(false);
				}
				else {
					ShowErrorControl(e);
				}
			}
		}
		private void DesignerHost_LoadComplete(object sender, EventArgs e) {
			try {
				DesignerHost.LoadComplete -= new EventHandler(DesignerHost_LoadComplete);
				XafDesignerHelper.ForceSaveLicxIfNeed(new GetProjectWrapperDelegate(delegate { return ProjectWrapper; }), GetService, TraceException);
				if(!IsProjectBuilt()) {
					ShowErrorControl("");
				}
				else if(deserializationErrors.Count > 0) {
					ShowErrorControl(deserializationErrors[0]);
				}
				else {
					if(errorControl != null) {
						RemoveErrorControl();
					}
					try {
						InitializeView();
						LogDesignerOpened();
						if(XafDesignerHelper.CheckLicenseExpired()) {
							ShowLicenseErrorControl();
						}
					}
					catch(Exception ex) {
						TraceException(ex);
						ShowErrorControl(ex);
					}
				}
			}
			catch(Exception ex) {
				TraceException(ex);
				throw;
			}
		}
		public void ShowLicenseErrorControl() {
			foreach(Control _control in view.Controls) {
				_control.Hide();
			}
			view.Controls.Add(XafDesignerHelper.GetLicenseErrorControl());
		}
		private void View_FocusChanged(object sender, EventArgs e) {
			RaiseFocusChanged();
		}
		private ErrorList GetErrorList() {
			DTE2 dte2 = (DTE2)GetService(typeof(DTE));
			return dte2.ToolWindows.ErrorList;
		}
		private string GetProjectPath() {
			ProjectItem projectItem = (ProjectItem)GetService(typeof(ProjectItem));
			if(projectItem != null) {
				return Path.GetDirectoryName(projectItem.ContainingProject.FileName);
			}
			return "";
		}
		private void InitTracing() {
			InitTracing(Component);
		}
		private void InitTracing(IComponent component) {
			if(component == null || component.Site == null) {
				return;
			}
			traceGuid = component.Site.GetType().GUID;
			string projectPath = GetProjectPath();
			if(!string.IsNullOrEmpty(projectPath)) {
				string logFileName = Path.Combine(GetProjectPath(), component.Site.Name + ".Designer.log");
				Tracing.Initialize(traceGuid, logFileName);
			}
		}
		private IList<string> AllowedFilterStrings {
			get {
				if(allowedFilterStrings == null) {
					allowedFilterStrings = GetAllowedFilterStringsCore();
				}
				return allowedFilterStrings;
			}
		}
		private bool IsToolFiltersAllowed(Type toolType) {
			Attribute[] attributes = Attribute.GetCustomAttributes(toolType, typeof(ToolboxItemFilterAttribute));
			if(attributes.Length == 1 && typeof(ModuleBase).IsAssignableFrom(toolType)
				&& ((ToolboxItemFilterAttribute)attributes[0]).FilterString == "Xaf"
				&& ((ToolboxItemFilterAttribute)attributes[0]).FilterType == ToolboxItemFilterType.Require) {
				return true;
			}
			IEnumerator enumerator = attributes.GetEnumerator();
			while(enumerator.MoveNext()) {
				ToolboxItemFilterAttribute attribute = (ToolboxItemFilterAttribute)enumerator.Current;
				if(AllowedFilterStrings.Contains(attribute.FilterString) && attribute.FilterType == ToolboxItemFilterType.Allow) {
					return true;
				}
			}
			return false;
		}
		private object TryToGetService(Type serviceType) {
			object result = GetService(serviceType);
			if(result == null) {
				throw new Exception(string.Format("Could not obtain {0} interface.", serviceType.Name));
			}
			return result;
		}
		private void SolutionEvents_BeforeClosing() {
			IVsRunningDocumentTable pRDT = GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
			if(pRDT != null) {
				if(_rdtEventsCookie != 0) {
					pRDT.UnadviseRunningDocTableEvents(_rdtEventsCookie);
					_rdtEventsCookie = 0;
				}
			}
		}
		protected abstract void InitializeView();
		protected abstract Control CreateView();
		protected virtual void LogDesignerOpened() {
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					RemoveErrorControl();
					if(view != null && !view.IsDisposed) {
						view.Dispose();
					}
					view = null;
					focusChanged = null;
					DesignerHost.LoadComplete -= new EventHandler(DesignerHost_LoadComplete);
					RemoveWarnings();
					if(XafRootDesignerBase.Enable_PopupMenu) {
						IMenuCommandService menuService = MenuCommandService;
						if(menuService != null) {
							foreach(MenuCommand menuCommand in menuCommands) {
								menuService.RemoveCommand(menuCommand);
							}
							menuCommands.Clear();
						}
					}
					IVsRunningDocumentTable pRDT = GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
					if(pRDT != null) {
						if(_rdtEventsCookie != 0) {
							pRDT.UnadviseRunningDocTableEvents(_rdtEventsCookie);
							_rdtEventsCookie = 0;
						}
					}
					ReflectionHelper.AssemblyResolve -= new AssemblyResolveEventHandler(ReflectionHelper_AssemblyResolve);
				}
			}
			catch(Exception e) {
				TraceException(e);
				throw;
			}
		}
		protected virtual bool IsToolSupported(Type toolType) {
			try {
				return typeof(ModuleBase).IsAssignableFrom(toolType) && typeof(ModuleBase) != toolType && IsToolFiltersAllowed(toolType);
			}
			catch(Exception e) {
				TraceException(e);
				throw;
			}
		}
		protected virtual List<string> GetAllowedFilterStringsCore() {
			List<string> resultList = new List<string>();
			Type rootType = DesignerHost.GetType(DesignerHost.RootComponentClassName);
			if(rootType != null) {
				Attribute[] attributes = Attribute.GetCustomAttributes(rootType, typeof(ToolboxItemFilterAttribute));
				foreach(Attribute attribute in attributes) {
					ToolboxItemFilterAttribute filter = attribute as ToolboxItemFilterAttribute;
					if(filter != null && filter.FilterType == ToolboxItemFilterType.Allow) {
						if(filter.FilterString.StartsWith("Xaf.Platform.")) {
							resultList.Add(filter.FilterString);
						}
					}
				}
			}
			if(resultList.Count == 0) {
				resultList.Add("Xaf.Platform.Win");
				resultList.Add("Xaf.Platform.Web");
			}
			return resultList;
		}
		protected bool IsProjectBuilt() {
			if(Component.Site == null) {
				return false;
			}
			ITypeResolutionService typeService = (ITypeResolutionService)Component.Site.GetService(typeof(ITypeResolutionService));
			Type type = typeService.GetType(DesignerHost.RootComponentClassName);
			return type != null;
		}
		protected void ShowErrorControl(Exception e) {
			if(errorControl == null) {
				errorControl = new DesignErrorControl();
				foreach(Control control in view.Controls) {
					control.Hide();
				}
				view.Controls.Add(errorControl);
				errorControl.BuildProjectClicked += new EventHandler(errorControl_BuildProjectClicked);
			}
			if(e != null) {
				errorControl.SetErrorMessage(e);
			}
		}
		protected void ShowErrorControl(string errorMessage) {
			ShowErrorControl(new Exception(errorMessage));
		}
		protected void RemoveErrorControl() {
			if(errorControl != null) {
				Control parent = errorControl.Parent;
				if(parent != null) {
					parent.Controls.Remove(errorControl);
					foreach(Control control in parent.Controls) {
						control.Show();
					}
				}
				errorControl.Dispose();
				errorControl = null;
			}
		}
		protected void RaiseFocusChanged() {
			if(focusChanged != null) {
				focusChanged(this, EventArgs.Empty);
			}
		}
		protected void RemoveWarnings() {
			foreach(ErrorTask warning in warnings.Values) {
				if(ErrorListProvider.Tasks.Contains(warning)) {
					ErrorListProvider.Tasks.Remove(warning);
				}
			}
			warnings.Clear();
		}
		protected void RemoveWarning(string key) {
			if(ErrorListProvider != null && warnings.ContainsKey(key)) {
				ErrorTask warning = warnings[key];
				if(ErrorListProvider.Tasks.Contains(warning)) {
					ErrorListProvider.Tasks.Remove(warning);
				}
				warnings.Remove(key);
				warning = null;
			}
		}
		protected void AddWarning(string key, string warningText) {
			if(ErrorListProvider == null) {
				return;
			}
			if(!warnings.ContainsKey(key)) {
				ErrorTask warning = new ErrorTask();
				warning.ErrorCategory = TaskErrorCategory.Warning;
				warning.Text = warningText;
				warning.Removed += new EventHandler(userTypeWarning_Removed);
				warnings.Add(key, warning);
				ErrorListProvider.Tasks.Add(warning);
			}
			ErrorList errorList = GetErrorList();
			errorList.Parent.Activate();
			errorList.ShowWarnings = true;
		}
		protected void AddWarning(string warningText) {
			AddWarning(warningText, warningText);
		}
		protected ErrorListProvider ErrorListProvider {
			get { return (ErrorListProvider)GetService(typeof(ErrorListProvider)); }
		}
		static XafRootDesignerBase() {
			XafDesignerHelper.ShowAboutProductsEx();
		}
		public XafRootDesignerBase() {
		}
		public override void Initialize(IComponent component) {
			try {
				deserializationErrors.Clear();
				ReflectionHelper.Reset();
				XafTypesInfo.HardReset();
				DevExpress.ExpressApp.Xpo.XpoTypesInfoHelper.ForceInitialize();
				ReflectionHelper.AssemblyResolve += new AssemblyResolveEventHandler(ReflectionHelper_AssemblyResolve);
				base.Initialize(component);
				InitTracing();
				SelectionService.SetSelectedComponents(new object[] { Component }, SelectionTypes.Auto);
				DesignerHost.LoadComplete += new EventHandler(DesignerHost_LoadComplete);
				if(DesignerHost.GetService(typeof(RootDesignerService)) == null) {
					DesignerHost.AddService(typeof(RootDesignerService), new RootDesignerService(this));
				}
				ITypeDescriptorFilterService typeDescriptorFilterService = (ITypeDescriptorFilterService)DesignerHost.GetService(typeof(ITypeDescriptorFilterService));
				DesignerHost.RemoveService(typeof(ITypeDescriptorFilterService));
				DesignerHost.AddService(typeof(ITypeDescriptorFilterService), new XafDesignerFilterService(typeDescriptorFilterService));
				view = CreateView();
				((IEditorNavigation)view).FocusChanged += new EventHandler(View_FocusChanged);
				if(Enable_DTE) {
					dte = (DTE)GetService(typeof(DTE));
					dte.Events.SolutionEvents.BeforeClosing += new _dispSolutionEvents_BeforeClosingEventHandler(SolutionEvents_BeforeClosing);
				}
			}
			catch(Exception e) {
				TraceException(e);
				throw;
			}
		}
		public void ReferenceContainingAssembly(Type type) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(type, "type");
			AssemblyName assemblyName = type.Assembly.GetName();
			Project projectToReference = null;
			foreach(Project project in dte.Solution.Projects) {
				if(project.Properties != null) {
					try {
						if(assemblyName.Name == (project.Properties.Item("AssemblyName").Value as string)) {
							projectToReference = project;
							break;
						}
					}
					catch(ArgumentException e) {
						string message = string.Format("Unable to obtain the 'AssemblyName' property for the '{0}' project.", project.Name);
						TraceException(new ArgumentException(message, e));
					}
				}
			}
			if(projectToReference != null && dte.ActiveDocument != null && dte.ActiveDocument.ProjectItem != null) {
				Project activeProject = dte.ActiveDocument.ProjectItem.ContainingProject;
				if(activeProject != null && activeProject != projectToReference) {
					VSLangProj.VSProject projectObject = activeProject.Object as VSLangProj.VSProject;
					if(projectObject != null) {
						bool isAlreadyReferenced = false;
						foreach(VSLangProj.Reference reference in projectObject.References) {
							if(reference.SourceProject == projectToReference) {
								isAlreadyReferenced = true;
								break;
							}
						}
						if(!isAlreadyReferenced) {
							projectObject.References.AddProject(projectToReference);
						}
					}
				}
			}
			TypeResolutionService.ReferenceAssembly(assemblyName);
		}
		public DTE DTE {
			get { return dte; }
		}
		public void ProcessDeserializationErrors(CodeDomSerializer sender, IList errors) {
			if(errors.Count > 0) {
				foreach(object error in errors) {
					this.deserializationErrors.Add(error.ToString());
					if(error is Exception) {
						TraceException((Exception)error);
					}
					else {
						TraceException(new Exception(error.ToString()));
					}
				}
			}
		}
		public void AddMenuCommand(MenuCommand menuCommand) {
			if(MenuCommandService != null && MenuCommandService.FindCommand(menuCommand.CommandID) == null) {
				MenuCommandService.AddCommand(menuCommand);
				menuCommands.Add(menuCommand);
			}
		}
		public void ShowContextMenu(Point p) {
			IMenuCommandService menuService = MenuCommandService;
			if(menuService != null) {
				try {
					if(SelectionService.SelectionCount >= 1) {
						menuService.ShowContextMenu(MenuCommands.SelectionMenu, p.X, p.Y);
					}
					else {
						menuService.ShowContextMenu(MenuCommands.ContainerMenu, p.X, p.Y);
					}
				}
				catch(Exception ex) {
					TraceException(ex);
					MessageBox.Show(ex.Message);
				}
			}
		}
		public ToolboxItem DeserializeToolboxItem(IDataObject data) {
			IToolboxService toolboxService = ToolboxService;
			if(toolboxService != null && toolboxService.IsToolboxItem(data, DesignerHost)) {
				return toolboxService.DeserializeToolboxItem(data, DesignerHost);
			}
			return null;
		}
		public void TraceException(Exception e) {
			if(traceGuid != Guid.Empty)
				Tracing.LogError(traceGuid, e);
		}
		public Type GetToolboxItemType(ToolboxItem item) {
			Type type = item.GetType(DesignerHost);
			if(type == null) {
				type = item.GetType(null);
			}
			return type;
		}
		public virtual void AddModule(ToolboxItem item) {
		}
		public IDesignerHost DesignerHost {
			get { return (IDesignerHost)TryToGetService(typeof(IDesignerHost)); }
		}
		public IToolboxService ToolboxService {
			get { return (IToolboxService)TryToGetService(typeof(IToolboxService)); }
		}
		public ISelectionService SelectionService {
			get { return (ISelectionService)TryToGetService(typeof(ISelectionService)); }
		}
		public IComponentChangeService ComponentChangeService {
			get { return (IComponentChangeService)TryToGetService(typeof(IComponentChangeService)); }
		}
		public IMenuCommandService MenuCommandService {
			get { return (IMenuCommandService)TryToGetService(typeof(IMenuCommandService)); }
		}
		public ITypeResolutionService TypeResolutionService {
			get { return (ITypeResolutionService)TryToGetService(typeof(ITypeResolutionService)); }
		}
		public INameCreationService NameCreationService {
			get { return (INameCreationService)TryToGetService(typeof(INameCreationService)); }
		}
		public object GetView(ViewTechnology technology) {
			if(technology != ViewTechnology.Default) {
				throw new ArgumentException();
			}
			return view;
		}
		public ViewTechnology[] SupportedTechnologies {
			get { return new ViewTechnology[] { ViewTechnology.Default }; }
		}
		private readonly object lockObject = new object();
		private readonly Dictionary<ToolboxItem, bool> toolboxItemsSuppotrtedDictionary = new Dictionary<ToolboxItem, bool>();
		public bool GetToolSupported(ToolboxItem tool) {
			try {
				lock(lockObject) {
					bool isSupported;
					if(!toolboxItemsSuppotrtedDictionary.TryGetValue(tool, out isSupported)) {
						Type toolType = GetToolboxItemType(tool);
						isSupported = IsToolSupported(toolType);
						toolboxItemsSuppotrtedDictionary.Add(tool, isSupported);
					}
					return isSupported;
				}
			}
			catch(Exception e) {
				TraceException(e);
				throw;
			}
		}
		public void ToolPicked(ToolboxItem tool) {
			if(tool != null) {
				Type itemType = GetToolboxItemType(tool);
				if(typeof(ModuleBase).IsAssignableFrom(itemType)) {
					AddModule(tool);
				}
			}
		}
		void IEditorNavigation.NavigateTo(string text) {
			((IEditorNavigation)view).NavigateTo(text);
		}
		string IEditorNavigation.GetState() {
			return ((IEditorNavigation)view).GetState();
		}
		void IEditorNavigation.SetValue(string name, object value) {
			((IEditorNavigation)view).SetValue(name, value);
		}
		event EventHandler IEditorNavigation.FocusChanged {
			add { focusChanged += value; }
			remove { focusChanged -= value; }
		}
	}
	public class XafDesignerDesigntimeLicenseContext : DesigntimeLicenseContext {
		public XafDesignerDesigntimeLicenseContext() {
		}
		public override LicenseUsageMode UsageMode {
			get {
				return LicenseUsageMode.Designtime;
			}
		}
	}
	public delegate ProjectWrapper GetProjectWrapperDelegate();
	public delegate object GetServiceDelegate(Type serviceType);
	public delegate void TraceExceptionDelegate(Exception e);
	public class XafDesignerHelper {
		public static void ShowAboutProductsEx() {
		}
		public static bool CheckLicenseExpired() {
			return false;
		}
		public static Control GetLicenseErrorControl() {
			TextBox errorMessageTextBox = new TextBox();
			errorMessageTextBox.BackColor = Color.Gray;
			errorMessageTextBox.BorderStyle = BorderStyle.None;
			errorMessageTextBox.Dock = DockStyle.Fill;
			errorMessageTextBox.Font = new Font(Control.DefaultFont.FontFamily, 20);
			errorMessageTextBox.Location = new Point(0, 0);
			errorMessageTextBox.Multiline = true;
			errorMessageTextBox.Name = "errorMessageTextBox";
			errorMessageTextBox.ReadOnly = true;
			errorMessageTextBox.TabIndex = 0;
			errorMessageTextBox.TabStop = false;
			return errorMessageTextBox;
		}
		public static void ForceSaveLicxIfNeed(GetProjectWrapperDelegate projectWrapper, GetServiceDelegate getService, TraceExceptionDelegate traceException) {
			try {
				IVsSolution solution = getService.Invoke(typeof(SVsSolution)) as IVsSolution;
				if(solution != null) {
					IVsHierarchy hierarchy;
					solution.GetProjectOfUniqueName(projectWrapper.Invoke().FullName, out hierarchy);
					if(hierarchy != null) {
						IVsProjectSpecialFiles files = hierarchy as IVsProjectSpecialFiles;
						if(files != null) {
							string fileName;
							uint num;
							files.GetFile(-1001, 3, out num, out fileName);
							if(!string.IsNullOrEmpty(fileName)) {
								DesignerDocDataService designerDocDataService = (DesignerDocDataService)getService.Invoke(typeof(DesignerDocDataService));
								designerDocDataService.Flush();
								DocData docData = designerDocDataService.GetFileDocData(fileName, FileAccess.ReadWrite, null);
								if(docData != null) {
									designerDocDataService.SaveDocData(docData);
								}
							}
						}
					}
				}
			}
			catch(Exception ex) {
				traceException.Invoke(ex);
			}
		}
		public static string AddLicx(IVsHierarchy hierarchy, IServiceProvider serviceProvider) {
			IVsProjectSpecialFiles files = hierarchy as IVsProjectSpecialFiles;
			if(files != null) {
				string fileName;
				uint num;
				files.GetFile(-1001, 3, out num, out fileName);
				if(!string.IsNullOrEmpty(fileName)) {
					AddTypeInfo(fileName, serviceProvider);
				}
				return fileName;
			}
			return null;
		}
		private static void AddTypeInfo(string fileName, IServiceProvider serviceProvider) {
			string content;
			using(StreamReader sr = new StreamReader(fileName)) {
				content = sr.ReadToEnd();
			}
			if(!content.Contains(typeof(SystemModule.SystemModule).AssemblyQualifiedName)) {
				if(ProjectWrapper.CanEditFile(new string[] { fileName }, serviceProvider)) {
					using(StreamWriter sw = new StreamWriter(fileName)) {
						if(!string.IsNullOrEmpty(content)) {
							content += Environment.NewLine;
						}
						sw.Write(content + typeof(SystemModule.SystemModule).AssemblyQualifiedName);
					}
				}
			}
		}
	}
}
