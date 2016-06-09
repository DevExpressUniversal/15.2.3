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
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.UI;
using Microsoft.VisualStudio.TemplateWizard;
using DevExpress.XtraReports.Design.Wizard;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Parameters;
using DevExpress.DataAccess.Design;
namespace DevExpress.XtraReports.Design {
	abstract class ReportTemplateWizardBase : IWizard {
		protected EnvDTE.ProjectItem projectItem;
		protected IDesignerHost Host {
			private set;
			get;
		}
		protected XtraReport Report {
			get { return Host != null ? (XtraReport)Host.RootComponent : null; }
		}
		string VSLanguage {
			get {
				return (projectItem != null && projectItem.FileCodeModel != null)
					? projectItem.FileCodeModel.Language
					: string.Empty;
			}
		}
		public virtual void BeforeOpeningFile(EnvDTE.ProjectItem projectItem) {
		}
		public virtual void ProjectFinishedGenerating(EnvDTE.Project project) {
		}
		public virtual void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem) {
			if(this.projectItem == null) {
				this.projectItem = projectItem;
			}
		}
		public virtual void RunFinished() {
			if(projectItem == null) return;
			AddWinReferences(projectItem.ContainingProject);
			System.Threading.SynchronizationContext.Current.Post(state => RunFinishedCore(), null);
		}
		static void AddWinReferences(EnvDTE.Project project) {
			if(!ProjectHelper.IsWinProject(project)) return;
			object outputType = ProjectHelper.GetPropertyValue(project, "OutputType");
			if(outputType == null) return;
			if(!Equals(Enum.ToObject(typeof(VSLangProj.prjOutputType), outputType), VSLangProj.prjOutputType.prjOutputTypeWinExe)) 
				return;
			Type[] types = new Type[] {
				typeof(DevExpress.Utils.ResFinder),
				typeof(DevExpress.Utils.UI.ResFinder),
				typeof(DevExpress.XtraEditors.BaseEdit),
				typeof(DevExpress.XtraPrinting.ResFinder),
				typeof(DevExpress.XtraReports.Design.ReportDesigner)
			};
			foreach(Type type in types)
				ProjectHelper.AddReference(project, type.Assembly.FullName);
		}
		protected virtual void RunFinishedCore() {
			try {
				EnvDTE.Window window = projectItem.Open(EnvDTE.Constants.vsViewKindDesigner);
				if(window != null) {
					window.Activate();
					Host = window.Object as IDesignerHost;
					if(Report != null)
						Report.ScriptLanguage = ToScriptLanguage(VSLanguage);
				}
			} catch {
			}
		}
		public virtual void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) {
		}
		public virtual bool ShouldAddProjectItem(string filePath) {
			return true;
		}
		static ScriptLanguage ToScriptLanguage(string language) {
			return language.Equals(EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB, StringComparison.OrdinalIgnoreCase)
				? ScriptLanguage.VisualBasic
				: ScriptLanguage.CSharp;
		}
	}
	interface IWizardService {
		string ClassName { get; }
	}
	class ReportTemplateWizard : ReportTemplateWizardBase, IServiceProvider, IWizardService, IParameterService {
		static ReportTemplateWizard() {
			DevExpress.Skins.SkinManager.EnableFormSkins();
		}
		ServiceContainer serviceContainer;
		VSXtraReportModel model;
		string className;
		string IWizardService.ClassName {
			get {
				return className;
			}
		}
		public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) {
			base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);
			IServiceProvider serviceProvider = new Microsoft.VisualStudio.Shell.ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)automationObject);
			serviceContainer = new ServiceContainer(serviceProvider);
			EnvDTE._DTE dte = (EnvDTE._DTE)automationObject;
			object[] projects = dte.ActiveSolutionProjects as object[];
			EnvDTE.Project project = projects != null && projects.Length > 0 ? projects[0] as EnvDTE.Project : null;
			if(project != null)
				serviceContainer.AddService(typeof(EnvDTE.Project), project);
			LookAndFeelService serv = new VSLookAndFeelService(serviceContainer);
			serv.InitializeRootLookAndFeel(new UserLookAndFeel(this));
			serviceContainer.AddService(typeof(ILookAndFeelService), serv);
			serviceContainer.AddService(typeof(IWizardService), this);
			serviceContainer.AddService(typeof(IParameterService), this);
			string referenceInfo = null;
			if(ProjectHelper.IsAnotherDxVerionReferenced(project, ref referenceInfo)) {
				Version currentVersion = this.GetType().Assembly.GetName().Version;
				string stringCurrentVersion = currentVersion.Major + "." + currentVersion.Minor + "." + currentVersion.Build;
				ShowError(serv, "This project already contains references to DevExpress controls of a different version.\nThe current version is " + stringCurrentVersion + ".\nIncorrect reference: " + referenceInfo);
				throw new WizardCancelledException();
			}
			if(ProjectHelper.IsWebProject(serviceContainer) && !ValidateAppCode(replacementsDictionary)) {
				ShowError(serv, "A report class should be added to the 'App_Code' folder instead.");
				throw new WizardCancelledException();
			}
			className = ValidateRootName(replacementsDictionary["$rootname$"]);
			model = CreateModel();
			if(model == null) return;
			replacementsDictionary["$safeitemrootname$"] = className;
			if(model.BaseReportType != null) {
				if(model.BaseReportType.Assembly != null && project != null)
					ProjectHelper.AddReferenceFromFile(project, model.BaseReportType.AssemblyLocation);
				replacementsDictionary["$reportbaseclassname$"] = model.BaseReportType.FullName;
			} else {
				replacementsDictionary["$reportbaseclassname$"] = typeof(XtraReport).FullName;
			}
		}
		void ShowError(LookAndFeelService serv, string message) {
			IUIService uiService = serviceContainer.GetService(typeof(IUIService)) as IUIService;
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			DevExpress.XtraEditors.XtraMessageBox.Show(serv.LookAndFeel, owner, message, ReportStringId.UD_ReportDesigner.GetString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		static string ValidateRootName(string targetName) {
			char ch1 = '_';
			if(targetName.Length > 0) {
				int num1 = targetName.IndexOf('.');
				if(num1 > 0) {
					targetName = targetName.Substring(0, num1);
				}
				char[] chArray1 = targetName.ToCharArray();
				for(int num2 = 0; num2 < chArray1.Length; num2++) {
					if(!char.IsLetterOrDigit(chArray1[num2])) {
						chArray1[num2] = ch1;
					}
				}
				targetName = new string(chArray1);
				if(char.IsDigit(chArray1[0])) {
					targetName = ch1 + targetName;
				}
			}
			return targetName;
		}
		static bool ValidateAppCode(Dictionary<string, string> replacementsDictionary) {
			string relurlnamespace;
			replacementsDictionary.TryGetValue("$relurlnamespace$", out relurlnamespace);
			if(string.IsNullOrEmpty(relurlnamespace)) {
				replacementsDictionary["$appcode$"] = @"App_Code\";
				return true;
			} else if(relurlnamespace.StartsWith("App_Code", StringComparison.OrdinalIgnoreCase)) {
				replacementsDictionary["$appcode$"] = string.Empty;
				return true;
			}
			return false;
		}
		public override bool ShouldAddProjectItem(string filePath) {
			return model != null;
		}
		protected override void RunFinishedCore() {
			base.RunFinishedCore();
			if(Host == null || model == null || model.BaseReportType != null) 
				return;
			try {
				UndoEngine undoEngine = Host.GetService(typeof(UndoEngine)) as UndoEngine;
				undoEngine.ExecuteWithoutUndo(() => BuildModel() );
			} catch {
			}
		}
		void BuildModel() {
			using(DesignerTransaction transaction = Host.CreateTransaction(DesignSR.Trans_CreateComponents)) {
				try {
					var c = new DataComponentCreator().CreateDataComponent(model);
					DevExpress.XtraReports.Wizards3.IWizardCustomizationServiceExtentions.CreateReportSafely(null, Host, model, c, c != null ? c.DataMember : string.Empty);
					foreach(DevExpress.Data.IParameter param in parameters) {
						Host.Container.Add((Parameter)param, param.Name);
						Report.Parameters.Add((Parameter)param);
					}
					IBandViewInfoService service = Host.GetService(typeof(IBandViewInfoService)) as IBandViewInfoService;
					if(service != null) {
						service.InvalidateViewInfo();
						service.Invalidate();
					}
					transaction.Commit();
				} catch {
					transaction.Cancel();
				}
			}
		}
		VSXtraReportModel CreateModel() {
			var lookAndFeelService = ((IServiceProvider)this).GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			UserLookAndFeel lookAndFeel = lookAndFeelService != null ? lookAndFeelService.LookAndFeel : UserLookAndFeel.Default;
			IUIService uiService = (IUIService)((IServiceProvider)this).GetService(typeof(IUIService));
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			var runner = new VSXtraReportWizardRunner(this, lookAndFeel, owner);
			IParameterService parameterService = (IParameterService)((IServiceProvider)this).GetService(typeof(IParameterService));
			var solutionTypesProvider = new PureSolutionTypesProvider();
			var connectionStringsProvider = new VSConnectionStringsService(this);
			var connectionStorageService = new VSConnectionStorageService(this);
			var client = new VSXtraReportWizardClient(connectionStorageService, solutionTypesProvider, connectionStringsProvider, parameterService, this) {
				CustomQueryValidator = new VSCustomQueryValidator(), Options = SqlWizardOptions.EnableCustomSql
			};
			if(runner.Run(client)) {
				VSXtraReportModel wizardModel = runner.WizardModel;
				DataComponentCreator.SaveConnectionIfShould(wizardModel, connectionStorageService);
				return wizardModel;
			}
			throw new WizardCancelledException(); 
		}
		object IServiceProvider.GetService(Type serviceType) {
			object service = serviceContainer.GetService(serviceType);
			return service ?? (Host != null ? Host.GetService(serviceType) : null);
		}
		#region IParameterService Members
		List<DevExpress.Data.IParameter> parameters = new List<DevExpress.Data.IParameter>();
		public IEnumerable<DevExpress.Data.IParameter> Parameters {
			get { return parameters; }
		}
		public void AddParameter(DevExpress.Data.IParameter parameter) {
			parameters.Add(parameter);
			((DevExpress.XtraReports.Parameters.Parameter)parameter).Name = string.Format("{0}{1}", typeof(DevExpress.XtraReports.Parameters.Parameter).Name.ToLower(), parameters.Count);
		}
		public DevExpress.Data.IParameter CreateParameter(Type type) {
			return new DevExpress.XtraReports.Parameters.Parameter() { Type = type };
		}
		public bool CanCreateParameters { get { return true; } }
		public string AddParameterString { get { return "New Report Parameter..."; } } 
		public string CreateParameterString { get { return "Report Parameter"; } } 
		#endregion
	}
}
namespace DevExpress.XtraReports.Design.Wizard {
	using DevExpress.XtraReports.Wizards3;
	using DevExpress.XtraReports.Wizards3.Presenters;
	using DevExpress.XtraReports.Wizards3.Views;
	using DevExpress.Utils;
	using DevExpress.Data.XtraReports.Wizard;
	using EnvDTE;
	using System.Reflection;
	using DevExpress.Entity.ProjectModel;
	using DevExpress.Data.Entity;
	using DevExpress.Data.WizardFramework;
	class VSXtraReportModel : XtraReportModel {
		public TypeInfo BaseReportType {
			get;
			set;
		}
		public VSXtraReportModel() {
		}
		public VSXtraReportModel(XtraReportModel model)
			: base(model) {
		}
		public override object Clone() {
			return new VSXtraReportModel(this);
		}	
	}
	interface IVSXtraReportWizardClient : IDataSourceWizardClientUI {
		IServiceProvider ServiceProvider2 { get; }
	}
	interface IVSChooseReportTypePageViewExtended : IChooseReportTypePageViewExtended { 
		void ShowError(string message);
	}
	class VSXtraReportWizardClient : DataSourceWizardClientUI, IVSXtraReportWizardClient { 
		readonly IServiceProvider serviceProvider;
		public VSXtraReportWizardClient(IConnectionStorageService connectionProvider, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IParameterService parameterService, IServiceProvider serviceProvider)
			: base(connectionProvider, parameterService, solutionTypesProvider, connectionStringsProvider) {
			this.serviceProvider = serviceProvider;
			PropertyGridServices = serviceProvider;
		}
		public IServiceProvider ServiceProvider2 { get { return serviceProvider; } }
	}
	class VSXtraReportWizardRunner : XtraReportWizardRunner<VSXtraReportModel, IVSXtraReportWizardClient> {
		IServiceProvider serviceProvider;
		public VSXtraReportWizardRunner(IServiceProvider serviceProvider, UserLookAndFeel lookAndFeel, IWin32Window owner)
			: base(lookAndFeel, owner) {
			this.serviceProvider = serviceProvider;
		}
		protected override WizardPageFactoryBase<VSXtraReportModel, IVSXtraReportWizardClient> CreatePageFactory(IVSXtraReportWizardClient client) {
			return new VSXtraReportWizardPageFactory(client, ProjectHelper.GetActiveProject(serviceProvider));
		}
		protected override Type StartPage {
			get {
				return typeof(VSLocalChooseReportTypePage);
			}
		}
		protected override System.Drawing.Size WizardSize { get { return new System.Drawing.Size(690, 464); } }
	}
	class VSXtraReportWizardPageFactory : XtraReportWizardPageFactory<VSXtraReportModel, IVSXtraReportWizardClient> {
		public VSXtraReportWizardPageFactory(IVSXtraReportWizardClient client, Project project)
			: base(client) {
				Container.RegisterInstance<Project>(project);
		}
		protected override void RegisterDependencies(IVSXtraReportWizardClient client) {
			base.RegisterDependencies(client);
			Container.RegisterInstance<IServiceProvider>(client.ServiceProvider2);
			Container.RegisterType<VSLocalChooseReportTypePage>();
			Container.RegisterType<IVSChooseReportTypePageViewExtended, VSChooseReportTypePageView>();
			Container.RegisterType<InheritedReportPage>();
			Container.RegisterType<IInheritedReportPageView, InheritedReportView>();
		}
	}
	class VSLocalChooseReportTypePage : ChooseReportTypePageEx<VSXtraReportModel> {
		readonly Project project;
		readonly bool anyConnections;
		public VSLocalChooseReportTypePage(IVSChooseReportTypePageViewExtended view, Project project, IEnumerable<SqlDataConnection> dataConnections)
			: base(view, dataConnections, DataSourceTypes.All) {
			this.project = project;
			anyConnections = dataConnections.Any();
		}
		public override Type GetNextPageType() {
			if(View.ReportType == ReportType.ReportStorage)
				return typeof(InheritedReportPage);
			Type nextPageType = base.GetNextPageType();
			if(nextPageType == typeof(ChooseDataSourceTypePage<VSXtraReportModel>) &&
			   ProjectHelper.IsWebProject(project))
				return anyConnections
					? typeof(ChooseConnectionPage<VSXtraReportModel>)
					: typeof(ConnectionPropertiesPage<VSXtraReportModel>);
			return nextPageType;
		}
		public override void Commit() {
			if(View.ReportType == ReportType.Standard && ProjectHelper.IsInClientProfile(project)) {
				((IVSChooseReportTypePageViewExtended)View).ShowError("Unable to create a Data-bound Report:\nThe Client Profile is specified as the target framework of the current assembly.");
				throw new WizardCancelledException();
			}
			base.Commit();
		}
	}
	class VSChooseReportTypePageView : ChooseReportTypePageView, IVSChooseReportTypePageViewExtended {
		protected override void InitializeGallery(DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup) {
			base.InitializeGallery(galleryItemGroup);
			DevExpress.XtraBars.Ribbon.GalleryItem galleryItem1 = new DevExpress.XtraBars.Ribbon.GalleryItem();
			galleryItem1.Description = "Inherited Report";
			galleryItem1.Image = ResourceImageHelper.CreateImageFromResources(DevExpress.XtraReports.Design.ResFinder.GetFullName("Images.InheritedReport.png"), DevExpress.XtraReports.Design.ResFinder.Assembly);
			galleryItem1.Tag = DevExpress.Data.XtraReports.Wizard.ReportType.ReportStorage;
			galleryItemGroup.Items.Add(galleryItem1);
			this.reportTypeGallery.Gallery.ItemImagePadding = new DevExpress.Skins.SkinPaddingEdges() { Bottom = 12, Left = 0, Right = 0, Top = 6 };
		}
		void IVSChooseReportTypePageViewExtended.ShowError(string message) {
			DevExpress.XtraEditors.XtraMessageBox.Show(this.LookAndFeel, this, message, ReportStringId.UD_ReportDesigner.GetString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	interface IInheritedReportPageView : IWizardPageView {
		event EventHandler SelectedItemChanged;
		TypeInfo SelectedItem { get; }
		IServiceProvider ServiceProvider { get; set; }
		bool CanSelectAssembly { get; set; }
		void AddItems(IList<TypeInfo> typeInfos);
	}
	class TypeInfo {
		public TypeInfo(string fullName) {
			FullName = fullName;
			int index = fullName.LastIndexOf(".");
			Name = index >= 0 ? fullName.Substring(index + 1) : fullName;
			NameSpace = index >= 0 ? fullName.Substring(0, index) : string.Empty;
		}
		public string FullName { get; private set; }
		public string Name { get; private set; }
		public string NameSpace { get; private set; } 
		public string AssemblyLocation { get; set;}
		public Assembly Assembly { get; set; }
	}
	class InheritedReportPage : WizardPageBase<IInheritedReportPageView, VSXtraReportModel> {
		Project project;
		string projectOutputLocation;
		string ProjectOutputLocation {
			get {
				if(string.IsNullOrEmpty(projectOutputLocation))
					projectOutputLocation = DTEHelper.BuildProject(project);
				return projectOutputLocation;
			}
		}
		public override bool MoveNextEnabled {
			get { return false; }
		}
		public override bool FinishEnabled {
			get { return View.SelectedItem != null; }
		}
		IServiceProvider ServiceProvider2 { get; set; }
		public InheritedReportPage(IInheritedReportPageView view, Project project, IServiceProvider serviceProvider2)
			: base(view) {
			this.project = project;
			ServiceProvider2 = serviceProvider2;
		}
		public override void Begin() {			
			if(project == null) return;
			View.CanSelectAssembly = true;
			View.ServiceProvider = ServiceProvider2;
			View.SelectedItemChanged -= View_SelectedItemChanged;
			View.SelectedItemChanged += View_SelectedItemChanged;
			View.AddItems(CreateTypeInfos());
		}
		IList<TypeInfo> CreateTypeInfos() {
			List<string> typeNames = ReportTypesProvider.GetTypeNames(project.ProjectItems);
			List<TypeInfo> typeInfos = new List<TypeInfo>(typeNames.Count);
			foreach(string fullTypeName in typeNames)
				typeInfos.Add(new TypeInfo(fullTypeName) { AssemblyLocation = ProjectOutputLocation });
			return typeInfos;
		}
		void View_SelectedItemChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		public override void Commit() {
			if(View.SelectedItem != null)
				Model.BaseReportType = View.SelectedItem;
		}
	}
}
