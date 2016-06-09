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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.Xpo;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using Microsoft.Win32;
namespace DevExpress.ExpressApp.ReportsV2.Win {
	public class WinReportServiceController : ReportServiceController {
		public const string XafToolBoxItemsCategory = "XAF Data Source";
		private static String registryKey = Registry.CurrentUser.Name + @"Software\Developer Express\XtraReports";
		internal static void RaiseQueryRootReportComponentName(QueryRootReportComponentNameEventArgs args) {
			RaiseQueryRootReportComponentNameCore(args);
		}
		protected override void ShowReportPreviewCore(string reportContainerHandle, ReportParametersObjectBase parametersObject, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty, ShowViewParameters showViewParameters) {
			IReportContainer reportContainer = ReportDataProvider.ReportsStorage.GetReportContainerByHandle(reportContainerHandle);
			Guard.ArgumentNotNull(reportContainer, "reportContainer");
			ReportPrintTool printTool = new ReportPrintTool(reportContainer.Report);
			SetupBeforePrint(reportContainer.Report, parametersObject, criteria, canApplyCriteria, sortProperty, canApplySortProperty);
			Form winForm = FindOwnerForm();
			if(Application.Model.Options is IModelOptionsWin && ((IModelOptionsWin)(Application.Model.Options)).FormStyle == DevExpress.XtraBars.Ribbon.RibbonFormStyle.Ribbon) {
				printTool.PreviewRibbonForm.RibbonControl.RibbonStyle = ((IModelOptionsWin)(Application.Model.Options)).RibbonOptions.RibbonControlStyle;
				printTool.ShowRibbonPreview(winForm, DevExpress.LookAndFeel.UserLookAndFeel.Default);
				new ReportPrintToolDisposeManager().Attach(printTool, printTool.PreviewRibbonForm);
			}
			else {
				printTool.ShowPreview(winForm, DevExpress.LookAndFeel.UserLookAndFeel.Default);
				new ReportPrintToolDisposeManager().Attach(printTool, printTool.PreviewForm);
			}
#if DebugTest
			GetPrintToolEventArgs args = new GetPrintToolEventArgs(printTool);
			OnGetPrintTool(args);
#endif
		}
		protected override void ShowDesignerCore(XtraReport report, string reportHandle) {
			Guard.ArgumentNotNull(report, "report");
			Guard.ArgumentNotNull(Application, "Application");
			using(IDesignForm designForm = CreateDesignForm()) {
				try {
					if(ReportDataProvider.ReportsStorage != null) {
						ReportDataProvider.ReportsStorage.QuerySubReportUrl += new EventHandler<QuerySubReportUrlEventArgs>(ReportsStorage_QuerySubReportUrl);
					}
					OnDesignFormCreated(designForm, report);
					designForm.OpenReport(report);
					designForm.DesignMdiController.ActiveDesignPanel.FileName = reportHandle;
					if(ReportDataProvider.ReportsStorage.IsNewReportHandle(reportHandle) && designForm.DesignMdiController.ActiveDesignPanel != null) {
						designForm.DesignMdiController.ActiveDesignPanel.ReportState = ReportState.Changed;
					}
					SetupReport(report);
					CustomShowDesignFormEventArgs customShowDesignFormEventArgs = new CustomShowDesignFormEventArgs(designForm, report, reportHandle);
					if(CustomShowDesignForm != null) {
						CustomShowDesignForm(this, customShowDesignFormEventArgs);
					}
					if(!customShowDesignFormEventArgs.Handled) {
						designForm.ShowDialog();
					}
				}
				finally {
					if(ReportDataProvider.ReportsStorage != null) {
						ReportDataProvider.ReportsStorage.QuerySubReportUrl -= new EventHandler<QuerySubReportUrlEventArgs>(ReportsStorage_QuerySubReportUrl);
					}
				}
			}
		}
		protected virtual void OnDesignFormCreated(IDesignForm designForm, XtraReport report) {
			if(DesignFormCreated != null) {
				DesignFormCreated(this, new DesignFormEventArgs(designForm, report));
			}
		}
		protected override void ShowWizardCore(Type reportDataType) {
			Guard.TypeArgumentIs(typeof(IReportDataV2), reportDataType, "reportDataType");
			Guard.ArgumentNotNull(Application, "Application");
			using(IObjectSpace newReportObjectSpace = Application.CreateObjectSpace(reportDataType)) {
				IReportDataV2 reportData = (IReportDataV2)newReportObjectSpace.CreateObject(reportDataType);
				XtraReport newReport = ReportDataProvider.ReportsStorage.LoadReport(reportData);
				if(NewXafReportWizardCommandHandler.RunXafReportWizard(Application, newReport, reportDataType, NewXafReportWizardShowing)) {
					string newReportHandle = ReportDataProvider.ReportsStorage.CreateNewReportHandle(reportDataType);
					ShowDesignerCore(newReport, newReportHandle);
				}
			}
		}
		private void SetupReport(XtraReport report) {
			ReportsModuleV2 reportsModule = ReportsModuleV2.FindReportsModule(Application.Modules);
			if(reportsModule != null && reportsModule.ReportsDataSourceHelper != null) {
				reportsModule.ReportsDataSourceHelper.SetupReport(report);
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
				AddServices(designForm.DesignMdiController);
				designForm.DesignMdiController.AddCommandHandler(new NewXafReportWizardCommandHandler(Application, designForm, NewXafReportWizardShowing));
				designForm.DesignMdiController.DesignPanelLoaded += new DesignerLoadedEventHandler(DesignMdiController_DesignPanelLoaded);
				designForm.DesignMdiController.SetCommandVisibility(ReportCommand.NewReport, CommandVisibility.None);
				designForm.DesignMdiController.SetCommandVisibility(ReportCommand.SaveFileAs, CommandVisibility.None);
				((XtraForm)designForm).Load += new EventHandler(WinReportServiceController_Load);
				((XtraForm)designForm).FormClosed += new FormClosedEventHandler(designForm_FormClosed);
				return designForm;
			}
		}
		private void AddServices(XRDesignMdiController designMdiController) {
			designMdiController.AddService(typeof(ReportTypeService), new XtraReportTypeService());
			designMdiController.AddService(typeof(IFilterEditorControlHelperService), new FilterEditorControlHelperService(Application));
			ReportsModuleV2 reportsModule = ReportsModuleV2.FindReportsModule(Application.Modules);
			if(reportsModule != null && reportsModule.ReportsDataSourceHelper != null) {
			   designMdiController.AddService(typeof(IReportObjectSpaceProvider), reportsModule.ReportsDataSourceHelper.CreateReportObjectSpaceProviderCore(designMdiController));
			}
		}
		private void AddToolboxItem(IServiceProvider serviceProvider) {
			IToolboxService ts = (IToolboxService)serviceProvider.GetService(typeof(IToolboxService));
			ts.AddToolboxItem(new ToolboxItem(typeof(ViewDataSource)), XafToolBoxItemsCategory);
			ts.AddToolboxItem(new ToolboxItem(typeof(CollectionDataSource)), XafToolBoxItemsCategory);
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
			AddToolboxItem(args.DesignerHost);
			new XafReportFileNameSynchronizer(ReportDataProvider.ReportsStorage.CreateNewReportHandle()).Attach((XRDesignPanel)sender);
			ComponentDesigner designer = (ComponentDesigner)args.DesignerHost.GetDesigner(args.DesignerHost.RootComponent);
			ReportTabControl reportTabControl = args.DesignerHost.GetService(typeof(ReportTabControl)) as ReportTabControl;
			reportTabControl.PreviewReportCreated += new EventHandler(reportTabControl_PreviewReportCreated);
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
		private void reportTabControl_PreviewReportCreated(object sender, EventArgs e) {
			ReportTabControl reportTabControl = (ReportTabControl)sender;
			SetupReport(reportTabControl.PreviewReport);
			ReportParametersDataSourceInitializer.SetupMultiValueParametersDataSources(reportTabControl.PreviewReport);
		}
		private void ReportsStorage_QuerySubReportUrl(object sender, QuerySubReportUrlEventArgs e) {
			using(IObjectSpace os = Application.CreateObjectSpace(e.ReportDataType)) {
				using(ListView chooseReportDataListView = Application.CreateListView(os, e.ReportDataType, true)) {
					chooseReportDataListView.CollectionSource.Criteria.Add("IsPredefined", new NullOperator(ReportsModuleV2.FindPredefinedReportTypeMemberName(e.ReportDataType)));
					ShowViewParameters parameters = new ShowViewParameters(chooseReportDataListView);
					parameters.TargetWindow = TargetWindow.NewModalWindow;
					parameters.Context = TemplateContext.PopupWindow;
					DialogController dialogController = Application.CreateController<ReportDataSelectionDialogController>();
					dialogController.Accepting += delegate(object _sender, DialogControllerAcceptingEventArgs args) {
						if(chooseReportDataListView.SelectedObjects.Count > 0) {
							IReportDataV2 reportData = os.GetObject(chooseReportDataListView.SelectedObjects[0]) as IReportDataV2;
							e.SubReportUrl = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
						}
					};
					parameters.Controllers.Add(dialogController);
					Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(null, null));
				}
			}
		}
		private Form FindOwnerForm() {
			if(Frame.Template is UserControl) {
				return ((UserControl)Frame.Template).FindForm();
			}
			return Frame.Template as Form;
		}
		public event EventHandler<CustomShowDesignFormEventArgs> CustomShowDesignForm;
		public event EventHandler<CreateCustomDesignFormEventArgs> CreateCustomDesignForm;
		public event EventHandler<NewXafReportWizardShowingEventArgs> NewXafReportWizardShowing;
		public event EventHandler<DesignFormEventArgs> DesignFormCreated;
		public event EventHandler<CreateCustomXafReportVerbsManagerEventArgs> CreateCustomXafReportVerbsManager;
#if DebugTest
		public class GetPrintToolEventArgs : EventArgs {
			private ReportPrintTool printTool;
			public GetPrintToolEventArgs(ReportPrintTool printTool) {
				this.printTool = printTool;
			}
			public ReportPrintTool PrintTool { get { return printTool; } }
		}
		private void OnGetPrintTool(GetPrintToolEventArgs args) {
			if(GetPrintTool != null) {
				GetPrintTool(this, args);
			}
		}
		public event EventHandler<GetPrintToolEventArgs> GetPrintTool;
#endif
		private class ReportPrintToolDisposeManager {
			private ReportPrintTool printTool;
			private Form previewForm;
			public void Attach(ReportPrintTool printTool, Form previewForm) {
				Utils.Guard.ArgumentNotNull(printTool, "printTool");
				Utils.Guard.ArgumentNotNull(previewForm, "previewForm");
				previewForm.FormClosed += new FormClosedEventHandler(PreviewFormEx_FormClosed);
				this.printTool = printTool;
				this.previewForm = previewForm;
			}
			private void PreviewFormEx_FormClosed(object sender, FormClosedEventArgs e) {
				if(printTool != null) {
					previewForm.FormClosed -= new FormClosedEventHandler(PreviewFormEx_FormClosed);
					printTool.Report.Dispose();
					printTool.Dispose();
					printTool = null;
					previewForm = null;
				}
			}
		}
	}
}
