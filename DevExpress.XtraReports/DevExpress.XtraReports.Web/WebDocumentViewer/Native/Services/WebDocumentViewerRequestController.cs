#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.IO;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public class WebDocumentViewerRequestController : IWebDocumentViewerRequestController {
		readonly IReportManagementService reportManagementService;
		readonly IDocumentManagementService documentManagementService;
		public WebDocumentViewerRequestController(IReportManagementService reportManagementService, IDocumentManagementService documentManagementService) {
			this.reportManagementService = reportManagementService;
			this.documentManagementService = documentManagementService;
		}
		#region IWebDocumentViewerRequestController
		[WebApiHttpAction("getDocumentData")]
		public DocumentDataResponse GetDocumentData(string documentId) {
			return documentManagementService.GetDocumentData(documentId);
		}
		[WebApiHttpAction("getLookUpValues")]
		public LookUpValuesResponse GetLookUpValues(LookUpValuesRequest lookUpValuesRequest) {
			return reportManagementService.GetLookUpValues(lookUpValuesRequest.ReportId, lookUpValuesRequest.Parameters, lookUpValuesRequest.RequiredParameterPaths);
		}
		[WebApiHttpAction("getBrickMap")]
		public BrickMapResponse GetBrickMap(BrickMapRequest brickMapRequest) {
			return documentManagementService.GetBrickMap(brickMapRequest);
		}
		[WebApiHttpAction("findText")]
		public FindTextResponse FindText(FindTextRequest findTextRequest) {
			return documentManagementService.FindText(findTextRequest);
		}
		[WebApiHttpAction("startBuild")]
		public StartBuildResponse StartBuild(StartBuildRequest startBuildRequest) {
			return reportManagementService.StartBuild(startBuildRequest.ReportId, startBuildRequest.Parameters, startBuildRequest.DrillDownKeys);
		}
		[WebApiHttpAction("getBuildStatus")]
		public BuildStatusResponse GetBuildStatus(BuildStatusRequest buildStatusRequest) {
			return documentManagementService.GetBuildStatus(buildStatusRequest);
		}
		[WebHttpAction("getPage", "image/png")]
		public byte[] GetPage(DocumentPageRequest pageRequest) {
			return documentManagementService.GetPage(pageRequest);
		}
		[WebHttpAction("exportTo")]
		public BinaryHttpActionResult ExportTo(DocumentExportRequest exportRequest) {
			ExportOptions exportOptions = DeserializeExportOptionsFromJson(exportRequest.ExportOptions);
			ExportedDocument result = documentManagementService.ExportTo(exportRequest.DocumentId, exportRequest.Format, exportOptions);
			return new BinaryHttpActionResult(result.Bytes, result.ContentType, result.FileName, result.ContentDisposition);
		}
		[WebApiHttpAction("startExport")]
		public string StartExport(DocumentExportRequest exportRequest) {
			ExportOptions exportOptions = DeserializeExportOptionsFromJson(exportRequest.ExportOptions);
			return documentManagementService.StartExport(exportRequest.DocumentId, exportRequest.Format, exportOptions);
		}
		[WebApiHttpAction("getExportStatus")]
		public ProgressResponse GetExportStatus(OperationProgressRequest request) {
			return documentManagementService.GetExportStatus(request.Id, request.TimeOut);
		}
		[WebApiHttpAction("getExportResult")]
		public BinaryHttpActionResult GetExportResult(ExportResultRequest exportRequest) {
			ExportedDocument result = documentManagementService.GetExportResult(exportRequest.Id, exportRequest.Printable);
			return new BinaryHttpActionResult(result.Bytes, result.ContentType, result.FileName, result.ContentDisposition);
		}
		[WebApiHttpAction("openReport")]
		public ReportToPreview OpenReport(string reportTypeName) {
			return reportManagementService.OpenReport(reportTypeName);
		}
		[WebApiHttpAction("close")]
		public void Close(ReportIdDocumentIdPair idPair) {
			try {
				if(!string.IsNullOrEmpty(idPair.DocumentId))
					documentManagementService.Release(idPair.DocumentId);
			} finally {
				if(!string.IsNullOrEmpty(idPair.ReportId))
					reportManagementService.Release(idPair.ReportId);
			}
		}
		[WebApiHttpAction("stopBuild")]
		public void StopBuild(string documentId) {
			documentManagementService.StopBuild(documentId);
		}
		#endregion
		static ExportOptions DeserializeExportOptionsFromJson(string json) {
			var result = new ExportOptions();
			if(string.IsNullOrEmpty(json) || json == "{}") {
				return result;
			}
			string jsonExportOptions = "{ \"PreviewSerializer\": { \"@version\":\"" + AssemblyInfo.Version + "\"," + json + "}}";
			var jsonExportOptionsBytes = JsonConverter.JsonToXml(jsonExportOptions);
			using(var stream = new MemoryStream(jsonExportOptionsBytes)) {
				result.RestoreFromStream(stream);
			}
			return result;
		}
	}
}
