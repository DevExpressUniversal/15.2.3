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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Core;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Web;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.ExpressApp.Reports.Web {
	public class ReportViewerDetailItem : ViewItem {
		private class ReportViewerReportDisposeManager {
			private IObjectSpace objectSpace;
			private XtraReport report;
			private void reportViewer_CacheReportDocument(object sender, CacheReportDocumentEventArgs e) {
				HttpContext.Current.Response.ClearHeaders(); 
			}
			private void reportViewer_Disposed(object sender, EventArgs e) {
				DisposeObjects();
			}
			private void reportViewer_Unload(object sender, EventArgs e) {
				DisposeObjects();
			}
			private void DisposeObjects() {
				if(report != null) {
					report.Dispose();
					report = null;
				}
				if(objectSpace != null) {
					objectSpace.Dispose();
					objectSpace = null;
				}
			}
			public void Attach(ReportViewer reportViewer, XtraReport report, IObjectSpace objectSpace) {
				this.objectSpace = objectSpace;
				this.report = report;
				reportViewer.CacheReportDocument += new CacheReportDocumentEventHandler(reportViewer_CacheReportDocument);
				reportViewer.Unload += new EventHandler(reportViewer_Unload);
				reportViewer.Disposed += new EventHandler(reportViewer_Disposed);
			}
		}
		private IReportData reportData;
		private IObjectSpaceProvider objectSpaceProvider;
		private object filteringObject;
		internal IModelMemberViewItem GetParameterViewItem(string parameterName, Type parameterType) {
			string uniqueParameterName = "Reports_" + parameterName.Replace('.', '_') + "_" + parameterType.Name;
			UpdatableParameter xafParameter = new UpdatableParameter(uniqueParameterName, parameterType);
			ParameterList parameterList = new ParameterList();
			parameterList.Add(xafParameter);
			ParametersObject.CreateBoundObject(parameterList);
			IModelDetailView detailViewModel = TempDetailViewHelper.CreateTempDetailViewModel(application.Model, typeof(ParametersObject));
			return (IModelMemberViewItem)detailViewModel.Items[uniqueParameterName];
		}
		private void parametersPanel_CustomizeParameterEditors(object sender, CustomizeParameterEditorsEventArgs e) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(e.Parameter.Type);
			if(typeInfo != null && typeInfo.IsDomainComponent) {
				ASPxPropertyEditor editor = (ASPxPropertyEditor)application.EditorFactory.CreateDetailViewEditor(false, GetParameterViewItem(e.Parameter.Name, e.Parameter.Type), typeof(ParametersObject), application, objectSpace);
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
					e.Editor = lookupPropertyEditor.DropDownEdit.DropDown;
					e.Editor.ID = editor.Id + "_" + e.Editor.ID;
					lookupPropertyEditor.SetValueToControl(e.Parameter.Value);
					e.ShouldSetParameterValue = false;
				}
				else {
					e.Editor = editor.Editor as ASPxEdit;
				}
			}
		}
		protected override object CreateControlCore() {
			InitLocalization();
			Panel control = new Panel();
			IObjectSpace objectSpace = objectSpaceProvider.CreateObjectSpace();
			XafReport report = (XafReport)reportData.LoadReport(objectSpace);
			ReportViewer viewer = new ReportViewer();
			new ReportViewerReportDisposeManager().Attach(viewer, report, objectSpace);
			report.SetFilteringObject(filteringObject);
			viewer.Report = report;
			viewer.AutoSize = true; 
			ReportToolbar toolbarTop = new ReportToolbar();
			toolbarTop.Width = new Unit("100%");
			toolbarTop.ReportViewer = viewer;
			ReportToolbar toolbarBottom = new ReportToolbar();
			toolbarBottom.Width = new Unit("100%");
			toolbarBottom.ReportViewer = viewer;
			Control viewSiteControl = viewer;
			if(report.RequestParameters && report.Parameters.Count > 0) {
				ReportParametersPanel parametersPanel = new ReportParametersPanel();
				parametersPanel.CustomizeParameterEditors += new EventHandler<CustomizeParameterEditorsEventArgs>(parametersPanel_CustomizeParameterEditors);
				parametersPanel.ReportViewer = viewer;
				parametersPanel.Width = new Unit(240);
				TableEx table = new TableEx();
				table.Width = new Unit("100%");
				table.ID = "ReportTable";
				table.Rows.Add(new TableRow());
				table.Rows[0].ID = "Report";
				TableCell paramsCell = new TableCell();
				paramsCell.Width = new Unit("20%");
				paramsCell.VerticalAlign = VerticalAlign.Top;
				paramsCell.ID = "ReportParamsCell";
				paramsCell.Controls.Add(parametersPanel);
				TableCell viewerCell = new TableCell();
				viewerCell.Width = new Unit("80%");
				viewerCell.ID = "ReportViewerCell";
				viewerCell.Controls.Add(viewer);
				table.Rows[0].Cells.Add(paramsCell);
				table.Rows[0].Cells.Add(viewerCell);
				viewSiteControl = table;
			}
			control.Controls.Add(toolbarTop);
			control.Controls.Add(viewSiteControl);
			control.Controls.Add(toolbarBottom);
			return control;
		}
		public ReportViewerDetailItem(IReportData passedReportData, IObjectSpaceProvider objectSpaceProvider, object filteringObject, string itemId)
			: base(null, itemId) {
			this.objectSpaceProvider = objectSpaceProvider;
			this.reportData = passedReportData;
			this.filteringObject = filteringObject;
		}
		public ReportViewer ReportViewer {
			get {
				if(Control != null) {
					ReportViewer reportViewer = (ReportViewer)Enumerator.Find(((Control)Control).Controls,
						delegate(object current) { return (current is ReportViewer); });
					if(reportViewer != null) {
						return reportViewer;
					}
					TableEx table = (TableEx)Enumerator.Find(((Control)Control).Controls,
						delegate(object current) { return (current is TableEx); });
					if(table != null) {
						return (ReportViewer)Enumerator.Find(table.Rows[0].Cells[1].Controls,
						delegate(object current) { return (current is ReportViewer); });
					}
				}
				return null;
			}
		}
		private IObjectSpace objectSpace;
		private XafApplication application;
		internal void Setup(XafApplication application, IObjectSpace objectSpace) {
			this.application = application;
			this.objectSpace = objectSpace;
		}
		private void InitLocalization() {
			if(ASPxReportControlLocalizer.Active != null) {
				ASPxReportsLocalizer.SetActiveLocalizerProvider(new ASPxReportControlLocalizerProvider());
			}
		}
	}
	public class PreviewActionBehavior {
		public bool ShowXAFPreviewActions;
		public ReportOutputType UseXAFPreviewActionAsDefaultListViewAction = ReportOutputType.Mht;
		public bool showInReportAction;
		public bool UseReportViewerActionAsDefaultListViewAction;
	}
	public enum ReportOutputType { ReportViewer, Mht, Pdf, Rtf, Xls }
	public class ReportsController : ObjectViewController {
		public static string PreviewInReportViewerActionCategory = "ListEditorInplace";
		public const string FormatSpecificPreviewActionsRequiredContext = "ShowFormatSpecificPreviewActions";
		public const string PreviewInReportViewerContext = "PreviewInReportViewer";
		private Type reportDataType;
		private SimpleAction previewInReportViewerAction;
		private SimpleAction previewInMhtAction;
		private SimpleAction previewInPdfAction;
		private SimpleAction previewInRtfAction;
		private SimpleAction previewInXlsAction;
		private SimpleAction defaultListViewAction;
		private System.ComponentModel.IContainer components;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.previewInReportViewerAction = new SimpleAction(this.components);
			this.previewInReportViewerAction.Id = "ShowReportInPreviewWindow";
			this.previewInReportViewerAction.Caption = "Preview";
			this.previewInReportViewerAction.ToolTip = "Show report in preview window";
			this.previewInReportViewerAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			this.previewInReportViewerAction.Category = PreviewInReportViewerActionCategory;
			this.previewInReportViewerAction.Execute += new SimpleActionExecuteEventHandler(previewInReportViewerAction_Execute);
			this.previewInMhtAction = new SimpleAction(this.components);
			this.previewInMhtAction.Id = "PrintReport";
			this.previewInMhtAction.Caption = "mht";
			this.previewInMhtAction.ImageName = "Action_Export_ToMHT";
			this.previewInMhtAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			this.previewInMhtAction.Category = "Reports";
			this.previewInMhtAction.Execute += new SimpleActionExecuteEventHandler(previewInMhtAction_Execute);
			this.previewInRtfAction = new SimpleAction(this.components);
			this.previewInRtfAction.Id = "PrintReportToRtf";
			this.previewInRtfAction.Caption = "rtf";
			this.previewInRtfAction.ImageName = "Action_Export_ToRTF";
			this.previewInRtfAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			this.previewInRtfAction.Category = "Reports";
			this.previewInRtfAction.Execute += new SimpleActionExecuteEventHandler(previewInRtfAction_Execute);
			this.previewInPdfAction = new SimpleAction(this.components);
			this.previewInPdfAction.Id = "PrintReportToPdf";
			this.previewInPdfAction.Caption = "pdf";
			this.previewInPdfAction.ImageName = "Action_Export_ToPDF";
			this.previewInPdfAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			this.previewInPdfAction.Category = "Reports";
			this.previewInPdfAction.Execute += new SimpleActionExecuteEventHandler(previewInPdfAction_Execute);
			this.previewInXlsAction = new SimpleAction(this.components);
			this.previewInXlsAction.Id = "PrintReportToXls";
			this.previewInXlsAction.Caption = "xls";
			this.previewInXlsAction.ImageName = "Action_Export_ToExcel";
			this.previewInXlsAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			this.previewInXlsAction.Category = "Reports";
			this.previewInXlsAction.Execute += new SimpleActionExecuteEventHandler(previewInXlsAction_Execute);
			this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
		}
		private void previewInXlsAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ProcessExportReport(View.ObjectSpace.GetObjectHandle(View.CurrentObject), ReportOutputType.Xls);
		}
		private void previewInPdfAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ProcessExportReport(View.ObjectSpace.GetObjectHandle(View.CurrentObject), ReportOutputType.Pdf);
		}
		private void previewInRtfAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ProcessExportReport(View.ObjectSpace.GetObjectHandle(View.CurrentObject), ReportOutputType.Rtf);
		}
		private void previewInMhtAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ProcessExportReport(View.ObjectSpace.GetObjectHandle(View.CurrentObject), ReportOutputType.Mht);
		}
		private void previewInReportViewerAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			PreviewReportInReportViewer(e.ShowViewParameters, View.ObjectSpace.GetKeyValue(View.CurrentObject));
		}
		private void processCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			if(!e.Handled) {
				if((Frame.Context != TemplateContext.LookupControl) && (Frame.Context != TemplateContext.LookupWindow)
						&& (DefaultListViewAction != null) && DefaultListViewAction.Active && DefaultListViewAction.Enabled) {
					e.Handled = true;
					DefaultListViewAction.DoExecute();
				}
			}
		}
		protected void ProcessExportReport(string reportDataHandle, ReportOutputType outputType) {
			if(WebWindow.CurrentRequestWindow != null) {
				string clientScript = string.Format(@"showDialogWindow(""{0}"", {1}, {2});", ReportExportHttpHandler.GetExportingReportResourceUrl(reportDataHandle, outputType), DevExpress.ExpressApp.Web.PopupWindow.WindowWidth, DevExpress.ExpressApp.Web.PopupWindow.WindowHeight);
				WebWindow.CurrentRequestWindow.RegisterStartupScript("ExportReport", clientScript, true);
			}
		}
		protected void CreateReportPreviewDialog(SimpleActionExecuteEventArgs e, ReportOutputType outputType) {
			PreviewReport(e.ShowViewParameters, View.ObjectSpace.GetKeyValue(View.CurrentObject), outputType);
		}
		protected virtual void PreviewReportInReportViewer(ShowViewParameters showViewParameters, object reportDataKey) {
			Frame.GetController<ReportServiceController>().ShowPreview((IReportData)View.ObjectSpace.GetObjectByKey(reportDataType, reportDataKey));
		}
		protected virtual void PreviewReport(ShowViewParameters showViewParameters, object reportDataKey, ReportOutputType outputType) {
			IObjectSpace parametersObjectSpace = Application.CreateObjectSpace(reportDataType);
			IReportData reportData = (IReportData)parametersObjectSpace.GetObjectByKey(reportDataType, reportDataKey);
			XafReport report = (XafReport)reportData.LoadReport(parametersObjectSpace);
			DetailView detailView = report.CreateParametersDetailView(Application);
			if(detailView == null) {
				detailView = new DetailView(parametersObjectSpace, reportData, Application, false);
				IModelDetailView detailViewModel = TempDetailViewHelper.CreateTempDetailViewModel(Application.Model, reportData.GetType());
				detailViewModel.Layout[0].Remove();
				List<IModelViewItem> itemsToRemove = new List<IModelViewItem>();
				foreach(IModelViewItem item in detailViewModel.Items) {
					itemsToRemove.Add(item);
				}
				foreach(IModelViewItem item in itemsToRemove) {
					item.Remove();
				}
				IModelStaticText detailViewItem = detailViewModel.Items.AddNode<IModelStaticText>();
				detailViewItem.Text = CaptionHelper.GetLocalizedText("Captions", "DownloadReportMessage");
				detailView.SetModel(detailViewModel);
			}
			detailView.Caption = reportData.ReportName;
			detailView.AllowEdit.SetItemValue("", true);
			detailView.ViewEditMode = ViewEditMode.Edit;
			parametersObjectSpace.SetModified(reportData);
			showViewParameters.Context = TemplateContext.PopupWindow;
			showViewParameters.CreatedView = detailView;
			showViewParameters.TargetWindow = TargetWindow.NewWindow;
			showViewParameters.Controllers.Add(CreateReportDialogController(reportData, outputType));
		}
		protected virtual ReportDialogController CreateReportDialogController(IReportData reportData, ReportOutputType outputType) {
			return new ReportDialogController(ObjectSpace.GetObjectHandle(reportData), outputType);
		}
		protected virtual void DisableEditing() {
			View.AllowEdit.SetItemValue(GetType().FullName, false);
			View.AllowNew.SetItemValue(GetType().FullName, false);
			View.AllowDelete.SetItemValue(GetType().FullName, false);
			ListViewController listViewController = Frame.GetController<ListViewController>();
			if(listViewController != null) {
				listViewController.EditAction.Active["WebReports"] = false;
			}
		}
		protected virtual void SetPreviewAsCustomProcessSelectedItem() {
			ListViewProcessCurrentObjectController processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(processCurrentObjectController != null) {
				processCurrentObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(ReportsModule.ActivateReportController(this, out reportDataType)) {
				DisableEditing();
				SetPreviewAsCustomProcessSelectedItem();
			}
		}
		protected override void OnDeactivated() {
			ListViewController listViewController = Frame.GetController<ListViewController>();
			if(listViewController != null) {
				listViewController.EditAction.Active.RemoveItem("WebReports");
			}
			ListViewProcessCurrentObjectController processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(processCurrentObjectController != null) {
				processCurrentObjectController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
			}
			base.OnDeactivated();
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			ReportsAspNetModule module = (ReportsAspNetModule)Frame.Application.Modules.FindModule(typeof(ReportsAspNetModule));
			if(module != null) {
				SetFormatSpecificPreviewActionsVisible(module.ShowFormatSpecificPreviewActions);
				previewInReportViewerAction.Active[PreviewInReportViewerContext] = (module.DefaultPreviewFormat == ReportOutputType.ReportViewer);
				if(module.DefaultPreviewFormat != ReportOutputType.ReportViewer && module.ShowFormatSpecificPreviewActions) {
					switch(module.DefaultPreviewFormat) {
						case ReportOutputType.Mht:
							defaultListViewAction = PreviewInMhtAction;
							break;
						case ReportOutputType.Pdf:
							defaultListViewAction = PreviewInPdfAction;
							break;
						case ReportOutputType.Rtf:
							defaultListViewAction = PreviewInRtfAction;
							break;
						case ReportOutputType.Xls:
							defaultListViewAction = PreviewInXlsAction;
							break;
						default:
							defaultListViewAction = PreviewInMhtAction;
							break;
					}
				}
			}
		}
		protected Type ReportDataType {
			get { return reportDataType; }
		}
		public ReportsController()
			: base() {
			InitializeComponent();
			TargetObjectType = typeof(IReportData);
			RegisterActions(components);
			defaultListViewAction = previewInReportViewerAction;
		}
		public void SetFormatSpecificPreviewActionsVisible(bool active) {
			PreviewInMhtAction.Active[FormatSpecificPreviewActionsRequiredContext] = active;
			PreviewInRtfAction.Active[FormatSpecificPreviewActionsRequiredContext] = active;
			PreviewInPdfAction.Active[FormatSpecificPreviewActionsRequiredContext] = active;
			PreviewInXlsAction.Active[FormatSpecificPreviewActionsRequiredContext] = active;
		}
		public SimpleAction PreviewInMhtAction {
			get { return previewInMhtAction; }
		}
		public SimpleAction PreviewInRtfAction {
			get { return previewInRtfAction; }
		}
		public SimpleAction PreviewInPdfAction {
			get { return previewInPdfAction; }
		}
		public SimpleAction PreviewInXlsAction {
			get { return previewInXlsAction; }
		}
		public SimpleAction PreviewInReportViewerAction {
			get { return previewInReportViewerAction; }
		}
		public SimpleAction DefaultListViewAction {
			get { return defaultListViewAction; }
			set { defaultListViewAction = value; }
		}
	}
	public class ReportDialogController : DialogController {
		private string reportDataHandle;
		private ReportOutputType outputType;
		protected ReportOutputType OutputType {
			get { return outputType; }
		}
		public ReportDialogController()
			: base() {
		}
		public ReportDialogController(string reportDataHandle, ReportOutputType outputType)
			: base() {
			this.SaveOnAccept = false;
			this.reportDataHandle = reportDataHandle;
			this.outputType = outputType;
		}
		protected override SimpleAction CreateAcceptAction() {
			SimpleAction result = new SimpleAction(this, "ReportDialogOK", DialogActionContainerName);
			result.Caption = "OK";
			result.Model.SetValue<bool>("IsPostBackRequired", true);
			return result;
		}
		protected override void Accept(SimpleActionExecuteEventArgs args) {
			((DevExpress.ExpressApp.Web.PopupWindow)Window).ClosureScript = string.Format(@"showDialogWindow(""{0}"", {1}, {2});", ReportExportHttpHandler.GetExportingReportResourceUrl(reportDataHandle, outputType), DevExpress.ExpressApp.Web.PopupWindow.WindowWidth, DevExpress.ExpressApp.Web.PopupWindow.WindowHeight);
		}
	}
	public class DownloadReport : IFileDownloadSource {
		XafReport report;
		public DownloadReport(XafReport report) {
			this.report = report;
		}
		public void WriteOutput(HttpResponse response) {
			ReportViewer.WriteRtfTo(response, report);
		}
	}
}
