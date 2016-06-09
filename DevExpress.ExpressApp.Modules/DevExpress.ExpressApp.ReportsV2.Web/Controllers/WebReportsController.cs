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
using System.IO;
using System.Web;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.ReportsV2.Web;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
namespace DevExpress.ExpressApp.ReportsV2.Web {
	public enum ReportOutputType { ReportViewer, Mht, Pdf, Rtf, Xls }
	public class WebReportsController : ReportsControllerCore {
		public const string FormatSpecificExportActionsRequiredContext = "ShowFormatSpecificExportActions";
		public const string PreviewInReportViewerContext = "PreviewInReportViewer";
		private IContainer components;
		private SimpleAction previewInReportViewerAction;
		private SimpleAction exportToMhtAction;
		private SimpleAction exportToPdfAction;
		private SimpleAction exportToRtfAction;
		private SimpleAction exportToXlsAction;
		public SimpleAction ExportToMhtAction {
			get { return exportToMhtAction; }
		}
		public SimpleAction ExportToRtfAction {
			get { return exportToRtfAction; }
		}
		public SimpleAction ExportToPdfAction {
			get { return exportToPdfAction; }
		}
		public SimpleAction ExportToXlsAction {
			get { return exportToXlsAction; }
		}
		public SimpleAction PreviewInReportViewerAction {
			get { return previewInReportViewerAction; }
		}
		public void SetFormatSpecificExportActionsVisible(bool active) {
			ExportToMhtAction.Active[FormatSpecificExportActionsRequiredContext] = active;
			ExportToRtfAction.Active[FormatSpecificExportActionsRequiredContext] = active;
			ExportToPdfAction.Active[FormatSpecificExportActionsRequiredContext] = active;
			ExportToXlsAction.Active[FormatSpecificExportActionsRequiredContext] = active;
		}
		protected override void InitializeActions() {
			components = new System.ComponentModel.Container();
			previewInReportViewerAction = new SimpleAction(components);
			previewInReportViewerAction.Id = "ShowReportInPreviewWindowV2";
			previewInReportViewerAction.Caption = "Preview";
			previewInReportViewerAction.ToolTip = "Show report in preview window";
			previewInReportViewerAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			previewInReportViewerAction.Category = "ListEditorInplace";
			previewInReportViewerAction.Execute += new SimpleActionExecuteEventHandler(previewInReportViewerAction_Execute);
			exportToMhtAction = CreateExportReportAction(ReportOutputType.Mht, "Action_Export_ToMHT");
			exportToRtfAction = CreateExportReportAction(ReportOutputType.Rtf, "Action_Export_ToRTF");
			exportToPdfAction = CreateExportReportAction(ReportOutputType.Pdf, "Action_Export_ToPDF");
			exportToXlsAction = CreateExportReportAction(ReportOutputType.Xls, "Action_Export_ToExcel");
			RegisterActions(components);
			ExecuteReportAction = previewInReportViewerAction;
		}
		protected override void OnActivatedReportController() {
			base.OnActivatedReportController();
		}
		protected override void OnDeactivated() {
			ListViewController listViewController = Frame.GetController<ListViewController>();
			if(listViewController != null) {
				listViewController.EditAction.Active.RemoveItem("WebReports");
			}
			base.OnDeactivated();
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			ReportsAspNetModuleV2 module = (ReportsAspNetModuleV2)Frame.Application.Modules.FindModule(typeof(ReportsAspNetModuleV2));
			if(module != null) {
				SetFormatSpecificExportActionsVisible(module.ShowFormatSpecificExportActions);
				previewInReportViewerAction.Active[PreviewInReportViewerContext] = (module.DefaultPreviewFormat == ReportOutputType.ReportViewer);
				if(module.DefaultPreviewFormat != ReportOutputType.ReportViewer && module.ShowFormatSpecificExportActions) {
					switch(module.DefaultPreviewFormat) {
						case ReportOutputType.Mht:
							ExecuteReportAction = ExportToMhtAction;
							break;
						case ReportOutputType.Pdf:
							ExecuteReportAction = ExportToPdfAction;
							break;
						case ReportOutputType.Rtf:
							ExecuteReportAction = ExportToRtfAction;
							break;
						case ReportOutputType.Xls:
							ExecuteReportAction = ExportToXlsAction;
							break;
						default:
							ExecuteReportAction = ExportToMhtAction;
							break;
					}
				}
			}
		}
		private SimpleAction CreateExportReportAction(ReportOutputType outputType, string imageName) {
			SimpleAction exportAction = new SimpleAction(components);
			exportAction.Id = "PrintReportTo_" + outputType;
			exportAction.Caption = outputType.ToString();
			exportAction.ImageName = imageName;
			exportAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			exportAction.Category = "Reports";
			exportAction.Execute += new SimpleActionExecuteEventHandler(exportAction_Execute);
			return exportAction;
		}
		private void previewInReportViewerAction_Execute(Object sender, SimpleActionExecuteEventArgs args) {
			ShowReportPreview(args);
		}
		private void exportAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(sender is SimpleAction) {
				string[] parameters = ((SimpleAction)sender).Id.Split('_');
				if(parameters.Length == 2) {
					ReportOutputType outputType = (ReportOutputType)Enum.Parse(typeof(ReportOutputType), parameters[1]);
					if(WebWindow.CurrentRequestWindow != null) {
						IReportDataV2 reportData = View.ObjectSpace.GetObject(e.CurrentObject) as IReportDataV2;
						string reportContainerHandle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
						string displayName = reportData.DisplayName;
						string url = StreamExportHttpHandler.GetExportingReportResourceUrl(displayName + "." + outputType.ToString().ToLowerInvariant(), reportContainerHandle, outputType);
						string clientScript = string.Format(@"window.location = ""{0}"";", url);
						WebWindow.CurrentRequestWindow.RegisterStartupScript("ExportReport", clientScript);
					}
				}
			}
		}
	}
}
