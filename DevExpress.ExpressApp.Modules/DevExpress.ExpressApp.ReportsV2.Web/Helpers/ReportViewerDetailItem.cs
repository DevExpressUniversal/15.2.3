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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.DocumentViewer;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.ExpressApp.ReportsV2.Web {
	public class ReportViewerDetailItem : ViewItem {
		private QueryReportEventArgs queryReport;
		private class ReportViewerReportDisposeManager {
			private XtraReport report;
			ASPxDocumentViewer reportViewer;
			private void reportViewer_CacheReportDocument(object sender, CacheReportDocumentEventArgs e) {
				if(HttpContext.Current != null) { 
					HttpContext.Current.Response.ClearHeaders();
				}
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
			}
			public void Attach(ASPxDocumentViewer reportViewer, XtraReport report) {
				this.report = report;
				this.reportViewer = reportViewer;
				reportViewer.CacheReportDocument += new CacheReportDocumentEventHandler(reportViewer_CacheReportDocument);
				reportViewer.Unload += new EventHandler(reportViewer_Unload);
				reportViewer.Disposed += new EventHandler(reportViewer_Disposed);
			}
		}
		public ReportViewerDetailItem(string reportContainerHandle, ReportViewerContainer reportViewContainer, string itemId)
			: base(null, itemId) {
			if(reportViewContainer != null) {
				queryReport = new QueryReportEventArgs(reportContainerHandle, reportViewContainer.ParametersObject, reportViewContainer.Criteria, reportViewContainer.CanApplyCriteria, reportViewContainer.SortProperty, reportViewContainer.CanApplySortProperty);
			}
			else {
				queryReport = new QueryReportEventArgs(reportContainerHandle, null, null, false, null, false);
			}
		}
		public ASPxDocumentViewer ReportViewer {
			get { return Control as ASPxDocumentViewer; }
		}
		protected override object CreateControlCore() {
			InitLocalization();
			OnQueryReport(queryReport);
			Guard.ArgumentNotNull(queryReport.Report, "queryReport.report");
			XtraReport report = queryReport.Report;
			report.DataSourceDemanded += report_DataSourceDemanded;
			ASPxDocumentViewer viewer = new ASPxDocumentViewer();
			viewer.Report = report;
			viewer.ClientInstanceName = "xafReportViewer";
			viewer.CustomizeParameterEditors += ReportViewer_CustomizeParameterEditors;
			viewer.SettingsSplitter.SidePanePosition = DocumentViewerSidePanePosition.Left;
			new ReportViewerReportDisposeManager().Attach(viewer, report);
			return viewer;
		}
		private void report_DataSourceDemanded(object sender, EventArgs e) {
			((XtraReport)sender).DataSourceDemanded -= report_DataSourceDemanded;
			InitReportParameterObject(sender);
		}
		private void InitReportParameterObject(object sender) {
			if(queryReport.ParametersObject != null) {
				var param = ((XtraReport)sender).Parameters[ReportDataSourceHelper.XafReportParametersObjectName];
				if(param != null && param.Value == null) {
					param.Value = queryReport.ParametersObject;
				}
			}
		}
		private void ReportViewer_CustomizeParameterEditors(object sender, CustomizeParameterEditorsEventArgs e) {
			OnCustomizeParameterEditors(e);
		}
		private void InitLocalization() {
			if(ASPxReportControlLocalizer.Active != null) {
				ASPxReportsLocalizer.SetActiveLocalizerProvider(new ASPxReportControlLocalizerProvider());
			}
		}
		protected virtual void OnQueryReport(QueryReportEventArgs args) {
			if(QueryReport != null) {
				QueryReport(this, args);
			}
		}
		protected virtual void OnCustomizeParameterEditors(CustomizeParameterEditorsEventArgs args) {
			if(CustomizeParameterEditors != null) {
				CustomizeParameterEditors(this, args);
			}
		}
		public event EventHandler<CustomizeParameterEditorsEventArgs> CustomizeParameterEditors;
		public event EventHandler<QueryReportEventArgs> QueryReport;
	}
}
