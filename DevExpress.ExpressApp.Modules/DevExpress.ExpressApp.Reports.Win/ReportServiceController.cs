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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Import;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using Microsoft.Win32;
namespace DevExpress.ExpressApp.Reports.Win {
	public class CustomShowPreviewEventArgs : HandledEventArgs {
		private XafReport report;
		private ReportPrintTool printTool;
		public CustomShowPreviewEventArgs(XafReport report, ReportPrintTool printTool) {
			this.report = report;
			this.printTool = printTool;
		}
		public XafReport Report {
			get { return report; }
		}
		public ReportPrintTool PrintTool {
			get { return printTool; }
		}
	}
	public class CreateCustomDesignFormEventArgs : EventArgs {
		private IDesignForm designForm;
		public IDesignForm DesignForm {
			get { return designForm; }
			set { designForm = value; }
		}
	}
	public class NewXafReportWizardShowingEventArgs : EventArgs {
		private Type reportDataType;
		private INewXafReportWizardParameters wizardParameters;
		private bool handled = false;
		private DialogResult wizardResult = DialogResult.None;
		public NewXafReportWizardShowingEventArgs(Type reportDataType, INewXafReportWizardParameters wizardParameters) {
			this.reportDataType = reportDataType;
			this.wizardParameters = wizardParameters;
		}
		public Type ReportDataType {
			get { return reportDataType; }
		}
		public INewXafReportWizardParameters WizardParameters {
			get { return wizardParameters; }
			set { wizardParameters = value; }
		}
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public DialogResult WizardResult {
			get { return wizardResult; }
			set { wizardResult = value; }
		}
	}
	public class CustomShowDesignFormEventArgs : HandledEventArgs {
		private IDesignForm designForm;
		private IReportData reportData;
		private IObjectSpace reportDataObjectSpace;
		private XafReport report;
		public CustomShowDesignFormEventArgs(IDesignForm designForm, IReportData reportData, IObjectSpace reportDataObjectSpace, XafReport report)
			: base(false) {
			this.designForm = designForm;
			this.reportData = reportData;
			this.reportDataObjectSpace = reportDataObjectSpace;
			this.report = report;
		}
		public IDesignForm DesignForm {
			get { return designForm; }
		}
		public IReportData ReportData {
			get { return reportData; }
		}
		public IObjectSpace ReportDataObjectSpace {
			get { return reportDataObjectSpace; }
		}
		public XafReport Report {
			get { return report; }
		}
	}
	public class DesignFormEventArgs : EventArgs {
		private IDesignForm designForm;
		private XafReport report;
		public DesignFormEventArgs(IDesignForm designForm, XafReport report) {
			this.designForm = designForm;
			this.report = report;
		}
		public IDesignForm DesignForm {
			get { return designForm; }
		}
		public XafReport Report {
			get { return report; }
		}
	}
	public class CreateCustomXafReportVerbsManagerEventArgs : HandledEventArgs {
		public CreateCustomXafReportVerbsManagerEventArgs(IDesignerHost designerHost, ComponentDesigner designer) {
			this.DesignerHost = designerHost;
			this.Designer = designer;
		}
		public XafReportVerbsManager Manager { get; set; }
		public IDesignerHost DesignerHost { get; private set; }
		public ComponentDesigner Designer { get; private set; }
	}
	public class CreateCustomRepxConverterEventArgs : EventArgs {
		public RepxConverter Converter { get; set; }
	}
	internal class XafReportFileNameSynchronizer {
		private string defaultFileName;
		public XafReportFileNameSynchronizer(string defaultFileName) {
			Guard.ArgumentNotNullOrEmpty(defaultFileName, "defaultFileName");
			this.defaultFileName = defaultFileName;
		}
		private void designPanel_ReportStateChanged(object sender, ReportStateEventArgs e) {
			if(e.ReportState == ReportState.Saved) {
				((XRDesignPanel)sender).FileName = ((XRDesignPanel)sender).Report.DisplayName;
			}
			if(e.ReportState == ReportState.Opened && string.IsNullOrEmpty(((XRDesignPanel)sender).FileName)) {
				((XRDesignPanel)sender).FileName = defaultFileName;
			}
		}
		public void Attach(XRDesignPanel designPanel) {
			designPanel.ReportStateChanged += new ReportStateEventHandler(designPanel_ReportStateChanged);
		}
	}
	internal class XafReportFilterValidator {
		private void report_FilterChanging(object sender, FilterChangingEventArgs e) {
			XafReport report = (XafReport)sender;
			if(String.IsNullOrEmpty(report.Filtering.FilterForDesignPreview)
					|| (report.Filtering.FilterForDesignPreview == CriteriaParametersProcessor.Process(report.Filtering.Filter))) {
				CriteriaWrapper criteriaWrapper = CriteriaEditorHelper.GetCriteriaWrapper(e.NewFilter, report.DataType, report.ObjectSpace);
				if((criteriaWrapper.EditableParameters.Count > 0) && String.IsNullOrEmpty(report.Filtering.Filter)) {
					Messaging.DefaultMessaging.Show(CaptionHelper.GetLocalizedText("Messages", "ReportPreviewFilter"),
						CaptionHelper.GetLocalizedText("Captions", "ReportDesignerWarning"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				report.Filtering.FilterForDesignPreview = CriteriaParametersProcessor.Process(e.NewFilter);
			}
		}
		private void report_FilterForDesignPreviewChanging(object sender, FilterChangingEventArgs e) {
			XafReport report = (XafReport)sender;
			CriteriaWrapper criteriaWrapper = CriteriaEditorHelper.GetCriteriaWrapper(e.NewFilter, report.DataType, report.ObjectSpace);
			if(criteriaWrapper.EditableParameters.Count > 0) {
				e.Cancel = true;
				Messaging.DefaultMessaging.Show(CaptionHelper.GetLocalizedText("Messages", "WrongReportPreviewFilter"),
					CaptionHelper.GetLocalizedText("Captions", "ReportDesignerError"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
		public void Attach(XafReport report) {
			if(report != null) {
				report.FilterChanging += new EventHandler<FilterChangingEventArgs>(report_FilterChanging);
				report.FilterForDesignPreviewChanging += new EventHandler<FilterChangingEventArgs>(report_FilterForDesignPreviewChanging);
			}
		}
	}
	public class XafRepxConverter : RepxConverter {
		protected override XtraReport LoadReport(System.IO.Stream rptStream, XtraReport report) {
			XtraReport sourceReport = base.LoadReport(rptStream, report);
			if(OverrideRootComponentNameOnImporting) {
				PropertyDescriptor nameDesignTimePD = TypeDescriptor.GetProperties(report, new Attribute[] { new BrowsableAttribute(true) })["Name"];
				if((nameDesignTimePD != null) && (sourceReport != null )) {
					nameDesignTimePD.SetValue(report, sourceReport.Name);
				}
			}
			return sourceReport;
		}
		[DefaultValue(true)]
		public bool OverrideRootComponentNameOnImporting = true;
	}
	public class XafReportVerbsManager : IDisposable {
		public const string ExportLayoutLocalizedTextItemName = "ExportLayout";
		public const string ImportLayoutLocalizedTextItemName = "ImportLayout";
		private const string defaultExt = ".repx";
		private string rootDirectory;
		private ComponentDesigner designer;
		private static SaveFileDialog CreateSaveFileDialog(string fileName, string defaultDirectory) {
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Filter = DialogFilter;
			fileDialog.InitialDirectory = GetInitialDirectory(fileName, defaultDirectory);
			fileDialog.FileName = fileName;
			fileDialog.Title = CaptionHelper.GetLocalizedText("Captions", "ExportLayout");
			return fileDialog;
		}
		private static string DialogFilter {
			get {
				return String.Format("{0} (*{2})|*{3}|" +
					"{1} (*.*)|*.*",
					CaptionHelper.GetLocalizedText("Captions", "ReportFiles"),
					CaptionHelper.GetLocalizedText("Captions", "AllFiles"), defaultExt, defaultExt);
			}
		}
		private static string GetInitialDirectory(string fileName, string defaultDirectory) {
			try {
				string s = Path.GetDirectoryName(fileName);
				return !string.IsNullOrEmpty(s) ? s : defaultDirectory;
			}
			catch {
				return defaultDirectory;
			}
		}
		private DesignerVerb GetVerbByText(DesignerVerbCollection verbsCollection, string text) {
			foreach(DesignerVerb verb in verbsCollection)
				if(verb.Text == text) {
					return verb;
				}
			return null;
		}
		private void ExportLayoutVerbHandler(object sender, EventArgs e) {
			Guard.ArgumentNotNull(designer.Component, "designer.Component");
			Guard.ArgumentNotNull(designer, "designer");
			ExportLayout((XtraReport)designer.Component, designer);
		}
		private void ImportLayoutVerbHandler(object sender, EventArgs e) {
			Guard.ArgumentNotNull(designer.Component, "designer.Component");
			ImportLayout((XtraReport)designer.Component, designer);
		}
		private IWin32Window GetDesignerWindow() {
			IServiceProvider provider = designer.Component != null ? designer.Component.Site : null;
			if(provider != null) {
				System.Windows.Forms.Design.IUIService serv = provider.GetService(typeof(System.Windows.Forms.Design.IUIService)) as System.Windows.Forms.Design.IUIService;
				if(serv != null) {
					return serv.GetDialogOwnerWindow();
				}
			}
			return null;
		}
		protected string RunOpenEditor() {
			using(OpenFileDialog fileDialog = new OpenFileDialog()) {
				fileDialog.Filter = DialogFilter;
				fileDialog.InitialDirectory = rootDirectory;
				fileDialog.Title = CaptionHelper.GetLocalizedText("Captions", "ImportLayout");
				return DialogRunner.ShowDialog(fileDialog, GetDesignerWindow()) == DialogResult.OK ?
					fileDialog.FileName : string.Empty;
			}
		}
		protected string RunSaveEditor(string defaultUrl) {
			using(SaveFileDialog fileDialog = CreateSaveFileDialog(defaultUrl, rootDirectory)) {
				if(DialogRunner.ShowDialog(fileDialog, GetDesignerWindow()) == DialogResult.OK) {
					return FileHelper.SetValidExtension(fileDialog.FileName, defaultExt, new string[] { defaultExt });
				}
				return string.Empty;
			}
		}
		protected virtual void ExportLayout(XtraReport report, ComponentDesigner designer) {
			string fileName = RunSaveEditor(report.DisplayName);
			if(!string.IsNullOrEmpty(fileName)) {
				report.SaveLayout(fileName);
			}
		}
		protected virtual void ImportLayout(XtraReport report, ComponentDesigner designer) {
			string fileName = RunOpenEditor();
			if(!string.IsNullOrEmpty(fileName)) {
				CreateCustomRepxConverterEventArgs args = new CreateCustomRepxConverterEventArgs();
				if(CreateCustomRepxConverter != null) {
					CreateCustomRepxConverter(this, args);
				}
				if(args.Converter == null) {
					args.Converter = new XafRepxConverter();
				}
				args.Converter.Convert(fileName, report);
			}
		}
		public XafReportVerbsManager() {
			rootDirectory = PathHelper.GetApplicationFolder();
		}
		public virtual void Attach(ComponentDesigner designer) {
			Guard.ArgumentNotNull(designer, "designer");
			this.designer = designer;
			DesignerVerb wizardVerb = GetVerbByText(designer.Verbs, DesignSR.Verb_ReportWizard);
			if(designer.Verbs.Contains(wizardVerb)) {
				designer.Verbs.Remove(wizardVerb);
			}
			designer.Verbs.Add(new DesignerVerb(CaptionHelper.GetLocalizedText("Captions", ExportLayoutLocalizedTextItemName), new EventHandler(ExportLayoutVerbHandler)));
			designer.Verbs.Add(new DesignerVerb(CaptionHelper.GetLocalizedText("Captions", ImportLayoutLocalizedTextItemName), new EventHandler(ImportLayoutVerbHandler)));
		}
		public virtual void Dispose() {
			designer = null;
		}
		public EventHandler<CreateCustomRepxConverterEventArgs> CreateCustomRepxConverter;
	}
	internal class NewXafReportWizardCommandHandler : ICommandHandler, IDisposable {
		private XafApplication application;
		private EventHandler<NewXafReportWizardShowingEventArgs> newXafReportWizardShowingHandler;
		private IDesignForm designForm;
		internal static bool RunXafReportWizard(XafApplication application, IReportData reportData, EventHandler<NewXafReportWizardShowingEventArgs> newXafReportWizardShowingHandler) {
			using(IObjectSpace designerObjectSpace = application.CreateObjectSpace(reportData.GetType())) {
				XafReport newReport = (XafReport)reportData.LoadReport(designerObjectSpace);
				QueryRootReportComponentNameEventArgs raiseQueryRootReportComponentNameArgs = new QueryRootReportComponentNameEventArgs(newReport);
				WinReportServiceController.RaiseQueryRootReportComponentName(raiseQueryRootReportComponentNameArgs);
				if(!raiseQueryRootReportComponentNameArgs.Handled) {
					raiseQueryRootReportComponentNameArgs.Name = raiseQueryRootReportComponentNameArgs.GetDefaultName();
				}
				newReport.Name = raiseQueryRootReportComponentNameArgs.Name;
				WizPageXafWelcome wizPageXafWelcome = null;
				NewXafReportWizardShowingEventArgs args = new NewXafReportWizardShowingEventArgs(reportData.GetType(), new NewXafReportWizardParameters(newReport));
				if(newXafReportWizardShowingHandler != null) {
					newXafReportWizardShowingHandler(null, args);
				}
				if(!args.Handled) {
					using(XRDesignFormEx form = new XRDesignFormEx()) {
						form.OpenReport(args.WizardParameters.Report);
						XtraReportWizardRunner wizard = new XtraReportWizardRunner(newReport);
						wizPageXafWelcome = new WizPageXafWelcome(wizard, application, args.WizardParameters);
						wizPageXafWelcome.ObjectSpace = designerObjectSpace;
						wizard.BeforeRun += delegate(object sender, XRWizardRunnerBeforeRunEventArgs e) {
							for(int i = 0; i < e.WizardPages.Count; i++) {
								if(e.WizardPages[i] is WizPageWelcome) {
									e.WizardPages[i] = wizPageXafWelcome;
									break;
								}
							}
						};
						args.WizardResult = wizard.Run();
					}
				}
				if(args.WizardResult == DialogResult.OK) {
					args.WizardParameters.AssignTo(reportData);
					reportData.SaveReport(newReport);
				}
				newReport.Dispose();
				if(wizPageXafWelcome != null) {
					wizPageXafWelcome.Dispose();
				}
				return args.WizardResult == DialogResult.OK;
			}
		}
		public NewXafReportWizardCommandHandler(XafApplication application, IDesignForm designForm, EventHandler<NewXafReportWizardShowingEventArgs> newXafReportWizardShowingHandler) {
			this.application = application;
			this.designForm = designForm;
			this.newXafReportWizardShowingHandler = newXafReportWizardShowingHandler;
		}
		public bool CanHandleCommand(ReportCommand command, ref bool enableNextHandler) {
			if (command == ReportCommand.NewReportWizard) {
				enableNextHandler = false;
				return true;
			}
			return false;
		}
		public void HandleCommand(ReportCommand command, object[] args) {
			ReportsModule reportsModule = (ReportsModule)application.Modules.FindModule(typeof(ReportsModule)); 
			if(reportsModule == null) {
				throw new InvalidOperationException("The ReportsModule is not found");
			}
			Type reportDataType = reportsModule.ReportDataType;
			using(IObjectSpace newReportObjectSpace = application.CreateObjectSpace(reportDataType)) {
				IReportData reportData = (IReportData)newReportObjectSpace.CreateObject(reportDataType);
				if(RunXafReportWizard(application, reportData, newXafReportWizardShowingHandler)) {
					XafReport report = (XafReport)reportData.LoadReport(application.CreateObjectSpace(reportDataType));
					report.IsObjectSpaceOwner = true;
					designForm.OpenReport(report);
					designForm.DesignMdiController.ActiveDesignPanel.FileName = ReportsModule.XafReportStorageExtension.CreateNewReportHandle(reportData);
				}
			}
		}
		public void Dispose() {
			application = null;
			designForm = null;
			newXafReportWizardShowingHandler = null;
		}
		#region ICommandHandler Members
		public bool CanHandleCommand(ReportCommand command) {
			return command == ReportCommand.NewReportWizard;
		}
		#endregion
	}
	public class QueryRootReportComponentNameEventArgs : HandledEventArgs {
		public QueryRootReportComponentNameEventArgs(XafReport report) {
			Guard.ArgumentNotNull(report, "report");
			this.Report = report;
		}
		public XafReport  Report { get; private set; }
		public string Name { get; set; }
		public string GetDefaultName() {
			DevExpress.XtraReports.Serialization.XRNameCreationService service = new DevExpress.XtraReports.Serialization.XRNameCreationService(null);
			return service.CreateNameByType(new ComponentCollection(new IComponent[] { }), Report.GetType());
		}
	}
	public class WinReportServiceController : ReportServiceController {
		private class DisposeReportOnClosePreviewHelper { 
			private XtraReport report;
			public DisposeReportOnClosePreviewHelper(Form form, XtraReport report) {
				Utils.Guard.ArgumentNotNull(form, "form");
				Utils.Guard.ArgumentNotNull(report, "report");
				form.FormClosed += new FormClosedEventHandler(PreviewFormEx_FormClosed);
				this.report = report;
			}
			private void PreviewFormEx_FormClosed(object sender, FormClosedEventArgs e) {
				report.Dispose();
				report = null;
			}
		}
		private static String registryKey = Registry.CurrentUser.Name + @"Software\Developer Express\XtraReports";
		public static event EventHandler<QueryRootReportComponentNameEventArgs> QueryRootReportComponentName;
		internal static void RaiseQueryRootReportComponentName(QueryRootReportComponentNameEventArgs args) {
			if(QueryRootReportComponentName != null) {
				QueryRootReportComponentName(null, args);
			}
		}
		private void RaiseCreateCustomDesignForm(CreateCustomDesignFormEventArgs args) {
			if(CreateCustomDesignForm != null) {
				CreateCustomDesignForm(this, args);
			}
		}
		protected IDesignForm CreateDesignForm() {
			CreateCustomDesignFormEventArgs args = new CreateCustomDesignFormEventArgs();
			RaiseCreateCustomDesignForm(args);
			if(args.DesignForm != null) {
				return args.DesignForm;
			}
			else {
				IDesignForm designForm;
				if(Application.Model.Options is IModelOptionsWin && ((IModelOptionsWin)(Application.Model.Options)).FormStyle == DevExpress.XtraBars.Ribbon.RibbonFormStyle.Ribbon) {
					designForm = new XRDesignRibbonForm();
					((XRDesignRibbonForm)designForm).RibbonControl.RibbonStyle = ((IModelOptionsWin)(Application.Model.Options)).RibbonOptions.RibbonControlStyle;
					SetFieldListSorting(((XRDesignRibbonForm)designForm).DesignDockManager);
				}
				else {
					designForm = new XRDesignForm();
					SetFieldListSorting(((XRDesignForm)designForm).DesignDockManager);
				}
				designForm.DesignMdiController.AddService(typeof(ReportTypeService), new XafReportTypeService());
				designForm.DesignMdiController.AddCommandHandler(new NewXafReportWizardCommandHandler(Application, designForm, NewXafReportWizardShowing));
				designForm.DesignMdiController.DesignPanelLoaded += new DesignerLoadedEventHandler(DesignMdiController_DesignPanelLoaded);
				designForm.DesignMdiController.SetCommandVisibility(ReportCommand.NewReport, CommandVisibility.None);
				designForm.DesignMdiController.SetCommandVisibility(ReportCommand.SaveFileAs, CommandVisibility.None);
				((XtraForm)designForm).Load += new EventHandler(WinReportServiceController_Load);
				((XtraForm)designForm).FormClosed += new FormClosedEventHandler(designForm_FormClosed);
				return designForm;
			}
		}
		private void WinReportServiceController_Load(object sender, EventArgs e) {
			if(sender is XRDesignForm && ((XRDesignForm)sender).DesignBarManager != null) {
				((XRDesignForm)sender).DesignBarManager.RestoreLayoutFromRegistry(registryKey);
			}
			if(sender is XRDesignRibbonForm && ((XRDesignRibbonForm)sender).DesignDockManager != null) {
				((XRDesignRibbonForm)sender).DesignDockManager.RestoreLayoutFromRegistry(registryKey);
			}
			((XtraForm)sender).Load -= new EventHandler(WinReportServiceController_Load);
		}
		private void SetFieldListSorting(XRDesignDockManager designDockManager) {
			FieldListDockPanel fieldListPanel = designDockManager[DesignDockPanelType.FieldList] as FieldListDockPanel;
			if(fieldListPanel != null) {
				XRDesignFieldList fieldList = fieldListPanel.Controls[0].Controls[0] as XRDesignFieldList;
				if(fieldList != null) {
					fieldList.SortOrder = SortOrder.Ascending;
					fieldList.ShowComplexProperties = ShowComplexProperties.Default;
				}
			}
		}
		private void designForm_FormClosed(object sender, FormClosedEventArgs e) {
			if(sender is XRDesignForm && ((XRDesignForm)sender).DesignBarManager != null) {
				((XRDesignForm)sender).DesignBarManager.SaveLayoutToRegistry(registryKey);
			}
			if(sender is XRDesignRibbonForm && ((XRDesignRibbonForm)sender).DesignDockManager != null) {
				foreach(DockPanel panel in ((XRDesignRibbonForm)sender).DesignDockManager.SavedVisiblePanels) {
					panel.Visibility = DockVisibility.Visible;
				}
				foreach(DockPanel panel in ((XRDesignRibbonForm)sender).DesignDockManager.SavedAutoHidePanels) {
					panel.Visibility = DockVisibility.AutoHide;
				}
				((XRDesignRibbonForm)sender).DesignDockManager.SaveLayoutToRegistry(registryKey);
			}
			((IDesignForm)sender).DesignMdiController.DesignPanelLoaded -= new DesignerLoadedEventHandler(DesignMdiController_DesignPanelLoaded);
			((XtraForm)sender).FormClosed -= new FormClosedEventHandler(designForm_FormClosed);
		}
		private void DesignMdiController_DesignPanelLoaded(object sender, DesignerLoadedEventArgs args) {
			new XafReportFileNameSynchronizer(ReportsModule.XafReportStorageExtension.CreateNewReportHandle()).Attach((XRDesignPanel)sender);
			XafReport report = args.DesignerHost.RootComponent as XafReport;
			if(report != null) {
				report.Filtering.SetApplication(Application);
				new XafReportFilterValidator().Attach(report);
				if(report != null && report.ObjectSpace == null) {
					report.ObjectSpace = Application.CreateObjectSpace(report.DataType);
					report.IsObjectSpaceOwner = true;
				}
			}
			ComponentDesigner designer = (ComponentDesigner)args.DesignerHost.GetDesigner(args.DesignerHost.RootComponent);
			CreateCustomXafReportVerbsManagerEventArgs args1 = new CreateCustomXafReportVerbsManagerEventArgs(args.DesignerHost, designer);
			if(CreateCustomXafReportVerbsManager != null) {
				CreateCustomXafReportVerbsManager(this, args1);
			}
			if(!args1.Handled) {
				args1.Manager = new XafReportVerbsManager();
			}
			if(args1.Manager != null) {
				args1.Manager.Attach(designer);
			}
		}
		private Form FindOwnerForm() {
			if(Frame.Template is UserControl) {
				return ((UserControl)Frame.Template).FindForm();
			}
			return Frame.Template as Form;
		}
		internal protected void ShowReportPreview(XafReport report) {
			ReportPrintTool printTool = new ReportPrintTool(report);
			CustomShowPreviewEventArgs args = new CustomShowPreviewEventArgs(report, printTool);
			OnCustomShowPreview(args);
			if(!args.Handled) {
				Form winForm = FindOwnerForm();
				if(Application.Model.Options is IModelOptionsWin && ((IModelOptionsWin)(Application.Model.Options)).FormStyle == DevExpress.XtraBars.Ribbon.RibbonFormStyle.Ribbon) {
					printTool.PreviewRibbonForm.RibbonControl.RibbonStyle = ((IModelOptionsWin)(Application.Model.Options)).RibbonOptions.RibbonControlStyle;
					printTool.ShowRibbonPreview(winForm, DevExpress.LookAndFeel.UserLookAndFeel.Default);
					new DisposeReportOnClosePreviewHelper(printTool.PreviewRibbonForm, report);
				}
				else {
					printTool.ShowPreview(winForm, DevExpress.LookAndFeel.UserLookAndFeel.Default);
					new DisposeReportOnClosePreviewHelper(printTool.PreviewForm, report);
				}
			}
		}
		protected void OnCustomShowPreview(CustomShowPreviewEventArgs args) {
			if(CustomShowPreview != null) {
				CustomShowPreview(this, args);
			}
		}
		protected void OnNewXafReportWizardShowing(NewXafReportWizardShowingEventArgs args) {
			if(NewXafReportWizardShowing != null) {
				NewXafReportWizardShowing(this, args);
			}
		}
		protected override void ShowPreviewCore(IReportData reportData, CriteriaOperator criteria) {
			IObjectSpace previewObjectSpace = Application.CreateObjectSpace(reportData.GetType());
			XafReport report = (XafReport)reportData.LoadReport(previewObjectSpace);
			report.IsObjectSpaceOwner = true;
			bool showPreview = true;
			if(!CriteriaOperator.Equals(criteria, null)) {
				report.SetFilteringObject(new LocalizedCriteriaWrapper(report.DataType, criteria));
			}
			else if(report.IsParametrized) {
				DetailView parametersDetailView = report.CreateParametersDetailView(Application);
				if(parametersDetailView != null) {
					ShowViewParameters showViewParameters = new ShowViewParameters();
					showViewParameters.CreatedView = parametersDetailView;
					showViewParameters.CreatedView.Caption = reportData.ReportName;
					showViewParameters.TargetWindow = TargetWindow.NewWindow;
					showViewParameters.Context = TemplateContext.PopupWindow;
					PreviewReportDialogController controller = Application.CreateController<PreviewReportDialogController>();
					controller.Initialize(report, this);
					showViewParameters.Controllers.Add(controller);
					showPreview = false;
					Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(null, null));
				}
				else {
				}
			}
			if(showPreview) {
				ShowReportPreview(report);
			}
		}
		protected virtual void ShowDesignerCore(IReportData reportData, IObjectSpace reportDataObjectSpace, Type targetBusinessClass, bool isModified) {
			Guard.ArgumentNotNull(reportData, "reportData");
			Guard.ArgumentNotNull(reportDataObjectSpace, "reportDataObjectSpace");
			Guard.ArgumentNotNull(Application, "Application");
			Guard.CheckObjectFromObjectSpace(reportDataObjectSpace, reportData);
			using(IObjectSpace designerObjectSpace = Application.CreateObjectSpace(reportData.GetType())) {
				XafReport report = (XafReport)reportData.LoadReport(designerObjectSpace);
				if(targetBusinessClass != null) {
					if(!reportDataObjectSpace.IsNewObject(reportData)) {
						throw new ArgumentException("targetBusinessClass"); 
					}
					report.DataType = targetBusinessClass;
				}
				using(IDesignForm designForm = CreateDesignForm()) {
					OnDesignFormCreated(designForm, report);
					designForm.OpenReport(report);
					if(!reportDataObjectSpace.IsNewObject(reportData)) {
						designForm.DesignMdiController.ActiveDesignPanel.FileName = reportData.ReportName;
					}
					else {
						designForm.DesignMdiController.ActiveDesignPanel.FileName = ReportsModule.XafReportStorageExtension.CreateNewReportHandle(reportData);
					}
					if(isModified && designForm.DesignMdiController.ActiveDesignPanel != null) {
						designForm.DesignMdiController.ActiveDesignPanel.ReportState = ReportState.Changed;
					}
					CustomShowDesignFormEventArgs customShowDesignFormEventArgs = new CustomShowDesignFormEventArgs(designForm, reportData, reportDataObjectSpace, report);
					if(CustomShowDesignForm != null) {
						CustomShowDesignForm(this, customShowDesignFormEventArgs);
					}
					if(!customShowDesignFormEventArgs.Handled) {
						designForm.ShowDialog();
					}
				}
			}
		}
		protected virtual void OnDesignFormCreated(IDesignForm designForm, XafReport report) {
			if(DesignFormCreated != null) {
				DesignFormCreated(this, new DesignFormEventArgs(designForm, report));
			}
		}
		protected virtual void ShowWizardCore(Type reportDataType) {
			Guard.TypeArgumentIs(typeof(IReportData), reportDataType, "reportDataType");
			Guard.ArgumentNotNull(Application, "Application");
			using(IObjectSpace newReportObjectSpace = Application.CreateObjectSpace(reportDataType)) {
				IReportData reportData = (IReportData)newReportObjectSpace.CreateObject(reportDataType);
				if(NewXafReportWizardCommandHandler.RunXafReportWizard(Application, reportData, NewXafReportWizardShowing)) {
					Frame.GetController<WinReportServiceController>().ShowDesigner(reportData, newReportObjectSpace, null, true);
				}
			}
		}
		public void ShowDesigner(Type reportDataType, object reportDataKey) {
			using(IObjectSpace reportDataObjectSpace = Application.CreateObjectSpace(reportDataType)) {
				ShowDesigner((IReportData)reportDataObjectSpace.GetObjectByKey(reportDataType, reportDataKey), reportDataObjectSpace, null, false);
			}
		}
		public void ShowDesigner(IReportData reportData) {
			using(IObjectSpace reportDataObjectSpace = Application.CreateObjectSpace(reportData.GetType())) {
				ShowDesigner((IReportData)reportDataObjectSpace.GetObject<IReportData>(reportData), reportDataObjectSpace, null, false);
			}
		}
		public void ShowDesigner(IReportData reportData, IObjectSpace reportDataObjectSpace, Type targetBusinessClass, bool isModified) {
			ShowDesignerCore(reportData, reportDataObjectSpace, targetBusinessClass, isModified);
		}
		public void ShowWizard(Type reportDataType) {
			ShowWizardCore(reportDataType);
		}
		public event EventHandler<CustomShowPreviewEventArgs> CustomShowPreview;
		public event EventHandler<CustomShowDesignFormEventArgs> CustomShowDesignForm;
		public event EventHandler<CreateCustomDesignFormEventArgs> CreateCustomDesignForm;
		public event EventHandler<NewXafReportWizardShowingEventArgs> NewXafReportWizardShowing;
		public event EventHandler<DesignFormEventArgs> DesignFormCreated;
		public event EventHandler<CreateCustomXafReportVerbsManagerEventArgs> CreateCustomXafReportVerbsManager;		
	}
}
