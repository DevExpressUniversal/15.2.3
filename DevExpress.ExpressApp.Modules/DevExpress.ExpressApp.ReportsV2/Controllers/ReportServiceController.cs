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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Xpo;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2 {
	public abstract class ReportServiceController : Controller {
		public static event EventHandler<QueryRootReportDataSourceComponentEventArgs> QueryRootReportDataSourceComponent;
		public static event EventHandler<QueryRootReportComponentNameEventArgs> QueryRootReportComponentName;
		internal static void RaiseQueryRootReportDataSourceComponent(QueryRootReportDataSourceComponentEventArgs args) {
			if(QueryRootReportDataSourceComponent != null) {
				QueryRootReportDataSourceComponent(null, args);
			}
		}
		protected internal static void RaiseQueryRootReportComponentNameCore(QueryRootReportComponentNameEventArgs args) {
			if(QueryRootReportComponentName != null) {
				QueryRootReportComponentName(null, args);
			}
		}
		protected abstract void ShowReportPreviewCore(string reportContainerHandle, ReportParametersObjectBase parametersObject, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty, ShowViewParameters showViewParameters);
		protected abstract void ShowDesignerCore(XtraReport report, string reportHandle);
		protected abstract void ShowWizardCore(Type reportDataType);
		public void ShowPreview(string reportContainerHandle) {
			Guard.ArgumentNotNullOrEmpty(reportContainerHandle, "reportContainerHandle");
			IReportContainer reportContainer = ReportDataProvider.ReportsStorage.GetReportContainerByHandle(reportContainerHandle, false);
			ShowReportPreview(reportContainerHandle, reportContainer.ParametersObjectType);
		}
		public void ShowPreview(string reportContainerHandle, CriteriaOperator criteria) {
			Guard.ArgumentNotNullOrEmpty(reportContainerHandle, "reportContainerHandle");
			ShowReportPreview(reportContainerHandle, null, criteria, true, null, false, null);
		}
		public void ShowPreview(string reportContainerHandle, SortProperty[] sortProperty) {
			Guard.ArgumentNotNullOrEmpty(reportContainerHandle, "reportContainerHandle");
			ShowReportPreview(reportContainerHandle, null, null, false, sortProperty, true, null);
		}
		public void ShowPreview(string reportContainerHandle, CriteriaOperator criteria, SortProperty[] sortProperty) {
			Guard.ArgumentNotNullOrEmpty(reportContainerHandle, "reportContainerHandle");
			ShowReportPreview(reportContainerHandle, null, criteria, true, sortProperty, true, null);
		}
		public void ShowDesigner(Type reportDataType, object reportDataKey) {
			using(IObjectSpace reportDataObjectSpace = Application.CreateObjectSpace(reportDataType)) {
				IReportDataV2 reportData = (IReportDataV2)reportDataObjectSpace.GetObjectByKey(reportDataType, reportDataKey);
				ShowDesignerCore(ReportDataProvider.ReportsStorage.LoadReport(reportData), ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData));
			}
		}
		public void ShowDesigner(XtraReport report, string reportHandle) {
			ShowDesignerCore(report, reportHandle);
		}
		public void ShowWizard(Type reportDataType) {
			ShowWizardCore(reportDataType);
		}
		public void SetupBeforePrint(XtraReport report) {
			SetupBeforePrint(report, null, null, false, null, false);
		}
		public void SetupBeforePrint(XtraReport report, ReportParametersObjectBase parametersObject, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty) {
			ReportsModuleV2 reportsModule = ReportsModuleV2.FindReportsModule(Application.Modules);
			if(reportsModule != null && reportsModule.ReportsDataSourceHelper != null) {
				reportsModule.ReportsDataSourceHelper.SetupBeforePrint(report, parametersObject, criteria, canApplyCriteria, sortProperty, canApplySortProperty);
			}
		}
		protected void HandleAccept(Object sender, DialogControllerAcceptingEventArgs e) {
			string reportContainerHandle = (string)((WindowController)sender).Tag;
			((DialogController)sender).Accepting -= HandleAccept;
			ReportParametersObjectBase reportParametersObject = (ReportParametersObjectBase)e.AcceptActionArgs.CurrentObject;
			ShowReportPreview(reportContainerHandle, reportParametersObject, reportParametersObject.GetCriteria(), true, reportParametersObject.GetSorting(), true, e.AcceptActionArgs.ShowViewParameters);
		}
		protected ReportParametersObjectBase CreateReportParametersObject(Type parametersObjectType, IObjectSpaceCreator objectSpaceProvider) {
			Guard.ArgumentNotNull(objectSpaceProvider, "objectSpaceProvider");
			ReportParametersObjectBase reportParametersObject = null;
			if(typeof(ReportParametersObjectBase).IsAssignableFrom(parametersObjectType)) {
				reportParametersObject = (ReportParametersObjectBase)TypeHelper.CreateInstance(parametersObjectType, objectSpaceProvider);
			}
			return reportParametersObject;
		}
		protected DetailView CreateParametersDetailView(ReportParametersObjectBase reportParametersObject) {
			Guard.ArgumentNotNull(reportParametersObject, "reportParametersObject");
			CreateCustomParametersDetailViewEventArgs args = new CreateCustomParametersDetailViewEventArgs(reportParametersObject, Application);
			OnCreateCustomParametersDetail(args);
			DetailView detailView = null;
			if(args.Handled) {
				detailView = args.DetailView;
			}
			else {
				detailView = Application.CreateDetailView(reportParametersObject.ObjectSpace, reportParametersObject, false); 
			}
			if(detailView != null && detailView.Items.Count == 0) {
				detailView.Dispose();
				detailView = null;
			}
			return detailView;
		}
		protected virtual void OnCustomShowPreview(CustomShowPreviewEventArgs args) {
			if(CustomShowPreview != null) {
				CustomShowPreview(this, args);
			}
		}
		protected virtual void OnCreateCustomParametersDetail(CreateCustomParametersDetailViewEventArgs args) {
			if(CreateCustomParametersDetailView != null) {
				CreateCustomParametersDetailView(this, args);
			}
		}
		protected virtual DialogController CreatePreviewReportDialogController() {
			return Application.CreateController<PreviewReportDialogController>();
		}
		private void ShowReportPreview(string reportContainerHandle, Type parametersObjectType) {
			if(parametersObjectType != null) {
				ShowParametersDetailView(reportContainerHandle, parametersObjectType);
			}
			else {
				ShowReportPreview(reportContainerHandle, null, null, false, null, false, null);
			}
		}
		private void ShowReportPreview(string reportContainerHandle, ReportParametersObjectBase parametersObject, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty, ShowViewParameters showViewParameters) {
			Guard.ArgumentNotNullOrEmpty(reportContainerHandle, "reportContainerHandle");
			CustomShowPreviewEventArgs args = new CustomShowPreviewEventArgs(reportContainerHandle, parametersObject, criteria, canApplyCriteria, sortProperty, canApplySortProperty, showViewParameters);
			OnCustomShowPreview(args);
			if(!args.Handled) {
				ShowReportPreviewCore(reportContainerHandle, parametersObject, criteria, canApplyCriteria, sortProperty, canApplySortProperty, showViewParameters);
			}
		}
		private void ShowParametersDetailView(string reportContainerHandle, Type parametersObjectType) {
			ReportParametersObjectBase reportParametersObject = CreateReportParametersObject(parametersObjectType, ApplicationReportObjectSpaceProvider.Instance);
			if(reportParametersObject != null) {
				DetailView parametersDetailView = CreateParametersDetailView(reportParametersObject);
				if(parametersDetailView != null && reportParametersObject != null) {
					parametersDetailView.ViewEditMode = Editors.ViewEditMode.Edit;
					IReportContainer reportContainer = ReportDataProvider.ReportsStorage.GetReportContainerByHandle(reportContainerHandle, false);
					parametersDetailView.Caption = reportContainer.DisplayName;
					DialogController controller = CreatePreviewReportDialogController();
					controller.Tag = reportContainerHandle;
					controller.Accepting += HandleAccept;
					ShowViewParameters showViewParameters = new ShowViewParameters();
					showViewParameters.Controllers.Add(controller);
					showViewParameters.CreatedView = parametersDetailView;
					showViewParameters.TargetWindow = TargetWindow.NewWindow;
					showViewParameters.Context = TemplateContext.PopupWindow; 
					Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<CustomShowPreviewEventArgs> CustomShowPreview;
		public event EventHandler<CreateCustomParametersDetailViewEventArgs> CreateCustomParametersDetailView;
	}
}
