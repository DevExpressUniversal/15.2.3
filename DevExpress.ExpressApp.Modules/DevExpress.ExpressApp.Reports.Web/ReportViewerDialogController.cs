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
using System.Drawing;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.Reports.Web {
	public class ReportViewerDialogController : DialogController {
		public const string PreviewActionId = "ReportViewerDialogController_Preview";
		public const string CloseActionId = "ReportViewerDialogController_Close";
		private IReportData reportData;
		private XafReport report;
		private IObjectSpace objectSpace;
		private CriteriaOperator filter;
		protected DetailView parametersDetailView;
		protected DetailView previewDetailView;
		private View startupView;
		private ReportViewerDetailItem reportViewer;
		private SimpleAction previewWithParametersAction;
		private SimpleAction closeAction;
		private void closeAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			Window.Close(false);
		}
		private void previewWithParametersAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			EnsurePreviewReportDetailView();
			e.ShowViewParameters.CreateAllControllers = false;
			e.ShowViewParameters.CreatedView = previewDetailView;
			e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
		}
		private void EnsurePreviewReportDetailView() {
			if(previewDetailView == null || previewDetailView.IsDisposed) {
				object filteringObject;
				if(!object.Equals(filter, null)) {
					filteringObject = new LocalizedCriteriaWrapper(report.DataType, filter);
				} else {
					filteringObject = report.GetFilteringObject();
				}
				previewDetailView = new DetailView((IModelDetailView)Application.Model.Views["ReportViewer_DetailView"], objectSpace, this.reportData, Application, false);
				reportViewer = new ReportViewerDetailItem(reportData, Application.ObjectSpaceProvider, filteringObject, "ReportViewer");
				previewDetailView.AddItem(reportViewer);
			}
		}
		private void Window_ViewChanging(object sender, ViewChangingEventArgs e) {
			e.DisposeOldView = (Window.View != previewDetailView && Window.View != parametersDetailView);
			previewWithParametersAction.Active["IsParametersStep"] = (e.View == parametersDetailView);
			if(e.View != null) {
				e.View.ScrollPosition = Point.Empty;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			this.Window.ViewChanging += new EventHandler<ViewChangingEventArgs>(Window_ViewChanging);
			if(reportViewer != null) {
				reportViewer.Setup(Application, objectSpace);
			}
		}
		protected override void Cancel(SimpleActionExecuteEventArgs args) {
			base.Cancel(args);
			Window.Dispose();
		}
		protected override void Accept(SimpleActionExecuteEventArgs args) {
			Window.Dispose();
		}
		public ReportViewerDialogController()
			: base() {
			SaveOnAccept = false;
			previewWithParametersAction = new SimpleAction(this, PreviewActionId, DialogController.DialogActionContainerName);
			previewWithParametersAction.Caption = "Preview";
			previewWithParametersAction.Execute += new SimpleActionExecuteEventHandler(previewWithParametersAction_Execute);
			closeAction = new SimpleAction(this, CloseActionId, DialogController.DialogActionContainerName);
			closeAction.Caption = "Close";
			closeAction.Execute += new SimpleActionExecuteEventHandler(closeAction_Execute);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(objectSpace != null) {
				objectSpace.Dispose();
				objectSpace = null;
			}
			if(parametersDetailView != null) {
				parametersDetailView.Dispose();
				parametersDetailView = null;
			}
			if(previewDetailView != null) {
				previewDetailView.Dispose();
				previewDetailView = null;
			}
		}
		public void Setup(IReportData passedReportData, CriteriaOperator filter) {
			this.AcceptAction.Active["ReportPreview"] = false;
			this.CancelAction.Active["ReportPreview"] = false;
			this.filter = filter;
			objectSpace = Application.CreateObjectSpace(passedReportData.GetType());
			this.reportData = (IReportData)objectSpace.GetObject(passedReportData);
			report = (XafReport)reportData.LoadReport(objectSpace);
			parametersDetailView = report.CreateParametersDetailView(Application);
			if(parametersDetailView != null && parametersDetailView.Items.Count == 0) {
				parametersDetailView.Dispose();
				parametersDetailView = null;
			}
			EnsurePreviewReportDetailView();
			if(parametersDetailView == null || !object.ReferenceEquals(filter, null)) {
				previewWithParametersAction.Active["ReportHasParameters"] = false;
				startupView = previewDetailView;
			} else {
				parametersDetailView.AllowEdit.SetItemValue("", true);
				parametersDetailView.ViewEditMode = ViewEditMode.Edit;
				parametersDetailView.Caption = this.reportData.ReportName;
				objectSpace.SetModified(this.reportData);
				startupView = parametersDetailView;
			}
		}
		public View CurrentView {
			get { return startupView; }
		}
		public DetailView ParametersDetailView {
			get { return parametersDetailView; }
		}
		public DetailView PreviewDetailView {
			get { return previewDetailView; }
		}
	}
}
