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
using System.Linq;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.ExpressApp.ReportsV2.Web {
	public class ReportWebViewerDetailItem : ViewItem {
		XtraReport report;
		ReportParametersObjectBase parametersObject;
		public ReportWebViewerDetailItem(XtraReport report, ReportParametersObjectBase parametersObject, string itemId)
			: base(null, itemId) {
			this.report = report;
			this.parametersObject = parametersObject;
		}
		public ASPxWebDocumentViewer ReportViewer {
			get { return Control as ASPxWebDocumentViewer; }
		}
		internal bool PopupMode { get; set; }
		protected override object CreateControlCore() {
			Guard.ArgumentNotNull(report, "report");
			InitLocalization();
			report.DataSourceDemanded += report_DataSourceDemanded;
			ASPxWebDocumentViewer viewer = CreateASPxWebDocumentViewer();
			viewer.OpenReport(report);
			return viewer;
		}
		void report_DataSourceDemanded(object sender, EventArgs e) {
			((XtraReport)sender).DataSourceDemanded -= report_DataSourceDemanded;
			InitReportParameterObject(sender);
		}
		private void InitReportParameterObject(object sender) {
			if(parametersObject != null) {
				var param = ((XtraReport)sender).Parameters[ReportDataSourceHelper.XafReportParametersObjectName];
				if(param != null && param.Value == null) {
					param.Value = parametersObject;
				}
			}
		}
		private ASPxWebDocumentViewer CreateASPxWebDocumentViewer() {
			ASPxWebDocumentViewer viewer = new ASPxWebDocumentViewer();
			viewer.ClientInstanceName = "xafReportViewer";
			if(PopupMode) {
				viewer.Height = PopupWindow.WindowHeight;
			}
			viewer.ClientSideEvents.Init = "function(s, e) { s.previewModel.reportPreview.zoom(1); }";
			viewer.CssClass = "dx-viewport";
			ApplyCurrentTheme(viewer);
			return viewer;
		}
		protected virtual void ApplyCurrentTheme(ASPxWebDocumentViewer viewer) {
			if(BaseXafPage.CurrentTheme != null && BaseXafPage.CurrentTheme.ToLower().Contains("black")) {
				viewer.ColorScheme = "dark";
			}
		}
		private void InitLocalization() {
			if(ASPxReportControlLocalizer.Active != null) {
				ASPxReportsLocalizer.SetActiveLocalizerProvider(new ASPxReportControlLocalizerProvider());
			}
		}
	}
}
