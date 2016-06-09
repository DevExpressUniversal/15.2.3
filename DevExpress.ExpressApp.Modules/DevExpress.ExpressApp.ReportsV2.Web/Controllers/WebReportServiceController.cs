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
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.Web;
using DevExpress.Xpo;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
namespace DevExpress.ExpressApp.ReportsV2.Web {
	public class WebReportServiceController : ReportServiceController {
		protected override void ShowReportPreviewCore(string reportContainerHandle, ReportParametersObjectBase parametersObject, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty, ShowViewParameters showViewParameters) {
			IReportContainer reportContainer = ReportDataProvider.ReportsStorage.GetReportContainerByHandle(reportContainerHandle);
			DetailView previewDetailView = CreateReportPreviewDetailView(reportContainer.Report, ReportsAspNetModuleV2.ReportViewDetailViewWebName);
			ReportViewerContainer reportViewContainer = new ReportViewerContainer(parametersObject, criteria, canApplyCriteria, sortProperty, canApplySortProperty);
			DialogController windowController = null;
			if(Html5ReportViewerMode) {
				SetupBeforePrint(reportContainer.Report, parametersObject, criteria, canApplyCriteria, sortProperty, canApplySortProperty);
				ReportParametersDataSourceInitializer.SetupParametersDataSources(reportContainer.Report);
				if(parametersObject != null) {
					SetupClientParametersObject(reportContainer.Report, parametersObject);
				}
				ReportWebViewerDetailItem viewer = new ReportWebViewerDetailItem(reportContainer.Report, parametersObject, "ReportViewer");
				previewDetailView.AddItem(viewer);
				viewer.PopupMode = ShowReportViewInPopup;
				windowController = CreateReportDesignerDialogController();
			}
			else {
				ReportViewerDetailItem reportViewer = new ReportViewerDetailItem(reportContainerHandle, reportViewContainer, "ReportViewer");
				reportViewer.QueryReport += this.HandleQueryReportEvent;
				reportViewer.CustomizeParameterEditors += HandleCustomizeParameterEditorsEvent;
				previewDetailView.AddItem(reportViewer);
				windowController = CreatePreviewReportDialogController();
				windowController.FrameAssigned += windowController_FrameAssigned;
				windowController.AcceptAction.Active["ReportPreview"] = false;
			}
			previewDetailView.Caption = reportContainer.Report.DisplayName;
			ShowReportView(previewDetailView, windowController, showViewParameters);
		}
		protected override void ShowDesignerCore(XtraReport report, string reportContainerHandle) {
			IObjectSpace os = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(ReportDataProvider.ReportsStorage.GetReportDataTypeFromHandle(reportContainerHandle));
			ReportDesignerDetailItem designerDetailItem = ShowReportDesignerDetailView(os, null);
			designerDetailItem.OpenReport(reportContainerHandle);
		}
		protected virtual void ShowDesignerCore(XtraReport report, IObjectSpace reportDataObjectSpace, ShowViewParameters showViewParameters) {
			ReportDesignerDetailItem designerDetailItem = ShowReportDesignerDetailView(reportDataObjectSpace, showViewParameters);
			designerDetailItem.OpenReport(report, report.DisplayName);
		}
		protected override void ShowWizardCore(Type reportDataType) {
			Guard.TypeArgumentIs(typeof(IReportDataV2), reportDataType, "reportDataType");
			Guard.ArgumentNotNull(Application, "Application");
			WebNewReportWizardShowingEventArgs wizardArgs = new WebNewReportWizardShowingEventArgs(reportDataType, new NewReportWizardParameters(new XtraReport(), reportDataType));
			ConfigureReportName(wizardArgs.WizardParameters.Report);
			OnNewReportWizardShowing(wizardArgs);
			IObjectSpace objectSpace = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(reportDataType);
			ShowViewParameters showViewParameters = new ShowViewParameters();
			DialogController windowController = CreateReportWizardDialogController();
			windowController.Accepting += windowController_Accepting;
			showViewParameters.Controllers.Add(windowController);
			showViewParameters.CreatedView = this.Application.CreateDetailView(objectSpace, wizardArgs.WizardParameters);
			((DetailView)showViewParameters.CreatedView).ViewEditMode = ViewEditMode.Edit;
			showViewParameters.TargetWindow = TargetWindow.NewWindow;
			Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));
		}
		protected virtual DialogController CreateReportDesignerDialogController() {
			DialogController dialogController = Application.CreateController<PreviewReportDialogController>();
			if(DevExpress.ExpressApp.Web.WebApplicationStyleManager.IsNewStyle) {
				dialogController.SaveOnAccept = true;
				dialogController.AcceptAction.Caption = dialogController.CancelAction.Caption;
			}
			else {
				dialogController.AcceptAction.Active["ReportDesigner"] = false;
			}
			dialogController.CancelAction.Active["ReportDesigner"] = false;
			return dialogController;
		}
		protected virtual DialogController CreateReportWizardDialogController() {
			return Application.CreateController<WebReportWizardDialogController>();
		}
		protected virtual void OnCustomizeParameterEditors(CustomizeParameterEditorsEventArgs args) {
			if(CustomizeParameterEditors != null) {
				CustomizeParameterEditors(this, args);
			}
		}
		protected virtual void OnNewReportWizardShowing(WebNewReportWizardShowingEventArgs args) {
			if(NewReportWizardShowing != null) {
				NewReportWizardShowing(this, args);
			}
		}
		protected virtual Type GetReportDataSourceObjectType(XtraReport report) {
			Type result = null;
			ReportsModuleV2 reportsModule = ReportsModuleV2.FindReportsModule(Application.Modules);
			if(reportsModule != null && reportsModule.ReportsDataSourceHelper != null) {
				object dataSource = reportsModule.ReportsDataSourceHelper.GetMasterReportDataSource(report);
				DataSourceBase dataTypeContainer = dataSource as DataSourceBase;
				if(dataTypeContainer != null) {
					result = dataTypeContainer.DataType;
				}
			}
			return result;
		}
		protected virtual void SetupClientParametersObject(XtraReport report, ReportParametersObjectBase parametersObject) {
			var xafParameter = report.Parameters[ReportDataSourceHelper.XafReportParametersObjectName];
			if(xafParameter != null) {
				xafParameter.Value = null;
			}
		}
		internal IModelMemberViewItem GetParameterViewItem(string parameterName, Type parameterType) {
			string uniqueParameterName = "Reports_" + parameterName.Replace('.', '_') + "_" + parameterType.Name;
			UpdatableParameter xafParameter = new UpdatableParameter(uniqueParameterName, parameterType);
			ParameterList parameterList = new ParameterList();
			parameterList.Add(xafParameter);
			ParametersObject.CreateBoundObject(parameterList);
			IModelDetailView detailViewModel = TempDetailViewHelper.CreateTempDetailViewModel(Application.Model, typeof(ParametersObject));
			return (IModelMemberViewItem)detailViewModel.Items[uniqueParameterName];
		}
		private DetailView CreateReportPreviewDetailView(XtraReport report, string viewId) {
			Guard.ArgumentNotNull(Application, "Application");
			Type dataType = GetReportDataSourceObjectType(report); 
			IObjectSpace objectSpace = Application.CreateObjectSpace(dataType);
			return new DetailView((IModelDetailView)Application.Model.Views[viewId], objectSpace, null, Application, false);
		}
		private ReportDesignerDetailItem ShowReportDesignerDetailView(IObjectSpace reportDataObjectSpace, ShowViewParameters showViewParameters) {
			Guard.ArgumentNotNull(Application, "Application");
			DetailView view = new DetailView((IModelDetailView)Application.Model.Views[ReportsAspNetModuleV2.ReportDesignDetailViewWebName], reportDataObjectSpace, null, Application, false);
			ReportDesignerDetailItem designerDetailItem = view.FindItem("ReportDesigner") as ReportDesignerDetailItem;
			DialogController windowController = CreateReportDesignerDialogController();
			ShowReportView(view, windowController, showViewParameters);
			return designerDetailItem;
		}
		private void windowController_Accepting(object sender, DialogControllerAcceptingEventArgs e) {
			INewReportWizardParameters paramsReport = (INewReportWizardParameters)e.AcceptActionArgs.CurrentObject;
			IObjectSpace os = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(paramsReport.ReportDataType);
			DevExpress.Persistent.Validation.Validator.RuleSet.ValidateAll(os, new object[] { paramsReport }, "Accept");
			XafReportStorageWebTool.Instance.NewReportParameters = paramsReport; 
			ShowDesignerCore(paramsReport.Report, os, e.ShowViewParameters);
			e.ShowViewParameters.CreatedView.Closed += CreatedView_Closed;
		}
		void CreatedView_Closed(object sender, EventArgs e) {
			RefreshView();
		}
		private void ConfigureReportName(XtraReport report) {
			QueryRootReportComponentNameEventArgs raiseQueryRootReportComponentNameArgs = new QueryRootReportComponentNameEventArgs(report);
			RaiseQueryRootReportComponentNameCore(raiseQueryRootReportComponentNameArgs);
			if(!raiseQueryRootReportComponentNameArgs.Handled) {
				raiseQueryRootReportComponentNameArgs.Name = raiseQueryRootReportComponentNameArgs.GetDefaultName();
			}
			report.Name = raiseQueryRootReportComponentNameArgs.Name;
		}
		private void ShowReportView(DetailView detailView, DialogController dialogController, ShowViewParameters showViewParameters) {
			if(ShowReportViewInPopup) {
				ShowReportDialog(detailView, dialogController, showViewParameters);
			}
			else {
				Frame.SetView(detailView);
			}
		}
		private bool ShowReportViewInPopup {
			get {
				ReportsAspNetModuleV2 module = (ReportsAspNetModuleV2)Frame.Application.Modules.FindModule(typeof(ReportsAspNetModuleV2));
				if(module != null) {
					return module.DesignAndPreviewDisplayMode == DesignAndPreviewDisplayModes.Popup;
				}
				return true;
			}
		}
		private bool Html5ReportViewerMode {
			get {
				ReportsAspNetModuleV2 module = (ReportsAspNetModuleV2)Frame.Application.Modules.FindModule(typeof(ReportsAspNetModuleV2));
				if(module != null) {
					return module.ReportViewerType == ReportViewerTypes.HTML5;
				}
				return false;
			}
		}
		private void ShowReportDialog(DetailView detailView, DialogController dialogController, ShowViewParameters showViewParameters) {
			bool showDialog = showViewParameters == null;
			showViewParameters = showViewParameters ?? new ShowViewParameters();
			ConfigureReportViewParameters(detailView, dialogController, showViewParameters);
			if(showDialog) {
				Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));
			}
		}
		private void ConfigureReportViewParameters(DetailView detailView, DialogController dialogController, ShowViewParameters showViewParameters) {
			showViewParameters.Controllers.Add(dialogController);
			showViewParameters.CreatedView = detailView;
			showViewParameters.TargetWindow = TargetWindow.NewWindow;
		}
		private void HandleQueryReportEvent(object sender, QueryReportEventArgs e) {
			IReportContainer reportContainer = ReportDataProvider.ReportsStorage.GetReportContainerByHandle(e.ReportHandle);
			Guard.ArgumentNotNull(reportContainer, "reportContainer");
			XtraReport report = reportContainer.Report;
			SetupBeforePrint(report, e.ParametersObject, e.Criteria, e.CanApplyCriteria, e.SortProperty, e.CanApplySortProperty);
			e.Report = report;
		}
		private void HandleCustomizeParameterEditorsEvent(object sender, CustomizeParameterEditorsEventArgs e) {
			CreateCustomParameterEditors(e);
			OnCustomizeParameterEditors(e);
		}
		private void CreateCustomParameterEditors(CustomizeParameterEditorsEventArgs e) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(e.Parameter.Type);
			if(typeInfo != null && typeInfo.IsDomainComponent) {
				Type dataType = GetReportDataSourceObjectType(e.Report);
				IObjectSpace objectSpace = Application.CreateObjectSpace(dataType);
				ASPxPropertyEditor editor = (ASPxPropertyEditor)Application.EditorFactory.CreateDetailViewEditor(false, GetParameterViewItem(e.Parameter.Name, e.Parameter.Type), typeof(ParametersObject), Application, objectSpace);
				ASPxLookupPropertyEditor lookupPropertyEditor = editor as ASPxLookupPropertyEditor;
				if(lookupPropertyEditor != null) {
					lookupPropertyEditor.WebLookupEditorHelper.EditorMode = LookupEditorMode.AllItems;
					lookupPropertyEditor.WebLookupEditorHelper.SmallCollectionItemCount = int.MaxValue;
				}
				editor.ViewEditMode = ViewEditMode.Edit;
				editor.CreateControl();
				editor.CurrentObject = new object();
				editor.ReadValue();
				if(lookupPropertyEditor != null) {
					lookupPropertyEditor.DropDownEdit.ClearingEnabled = false;
					lookupPropertyEditor.DropDownEdit.AddingEnabled = false;
					e.Editor = lookupPropertyEditor.DropDownEdit.DropDown;
					new PropertyEditorDisposeManager().Attach(lookupPropertyEditor, lookupPropertyEditor.DropDownEdit.DropDown);
					e.Editor.ID = editor.Id + "_" + e.Editor.ID;
					lookupPropertyEditor.SetValueToControl(e.Parameter.Value);
					e.ShouldSetParameterValue = false;
				}
				else {
					e.Editor = editor.Editor as ASPxEdit;
					new PropertyEditorDisposeManager().Attach(editor, editor.Editor);
				}
			}
		}
		private void RefreshView() {
			if(Frame != null && Frame.View != null && Frame.View.ObjectSpace != null) {
				Frame.View.ObjectSpace.Refresh();
			}
		}
		private void windowController_FrameAssigned(object sender, EventArgs e) {
			DialogController windowController = sender as DialogController;
			if(windowController.Frame is DevExpress.ExpressApp.Web.PopupWindow) {
				RegisterPopupResizeScript(windowController.Frame as DevExpress.ExpressApp.Web.PopupWindow);
			}
		}
		private void RegisterPopupResizeScript(DevExpress.ExpressApp.Web.PopupWindow popupWindow) {
			string resizeScript = @"var popupControl = GetActivePopupControl(window.parent);
                                        if (popupControl != null) {
                                            popupControl.Resize.AddHandler(function (s, e) {
                                                xafReportViewer.AdjustControlCore();
                                            });
                                        };";
			popupWindow.RegisterStartupScript("addPopupResizeHandle", resizeScript);
		}
		private class PropertyEditorDisposeManager {
			ASPxPropertyEditor propertyEditor;
			WebControl editor;
			private void editor_Disposed(object sender, EventArgs e) {
				DisposeObjects();
			}
			private void editor_Unload(object sender, EventArgs e) {
				DisposeObjects();
			}
			private void DisposeObjects() {
				if(editor != null) {
					editor.Unload -= new EventHandler(editor_Unload);
					editor.Disposed -= new EventHandler(editor_Disposed);
					editor.Dispose();
					editor = null;
				}
				if(propertyEditor != null) {
					propertyEditor.Dispose();
					propertyEditor = null;
				}
			}
			public void Attach(ASPxPropertyEditor propertyEditor, WebControl editor) {
				this.propertyEditor = propertyEditor;
				this.editor = editor;
				editor.Unload += new EventHandler(editor_Unload);
				editor.Disposed += new EventHandler(editor_Disposed);
			}
		}
		public event EventHandler<CustomizeParameterEditorsEventArgs> CustomizeParameterEditors;
		public event EventHandler<WebNewReportWizardShowingEventArgs> NewReportWizardShowing;
		#region Obsolete 15.2
		[Obsolete("This member is not used any longer.", true)]
		protected virtual IReportDataV2 CreateReportData(IObjectSpace os, INewReportWizardParameters paramsReport) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
