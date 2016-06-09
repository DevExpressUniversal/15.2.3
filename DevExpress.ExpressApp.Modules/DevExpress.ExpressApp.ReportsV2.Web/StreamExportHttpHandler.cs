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
using System.IO;
using System.Web;
using System.Web.SessionState;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.ExpressApp.Web;
namespace DevExpress.ExpressApp.ReportsV2.Web {
	public class StreamExportHttpHandler : IXafHttpHandler {
		private readonly object lockObject = new object();
		public static string GetExportingReportResourceUrl(string name, string reportHandle, ReportOutputType outputType) {
			return string.Format("DXX.axd?handlerName=StreamExportResource&name={0}&reportHandle={1}&outputType={2}", name, reportHandle, outputType);
		}
		public void ProcessRequest(HttpContext context) {
			lock(lockObject) {
				string reportHandle = context.Request.QueryString["reportHandle"];
				string name = context.Request.QueryString["name"];
				ReportOutputType outputType = (ReportOutputType)Enum.Parse(typeof(ReportOutputType), context.Request.QueryString["outputType"]);
				ReportsModuleV2 module = ReportsModuleV2.FindReportsModule(ApplicationReportObjectSpaceProvider.ContextApplication.Modules);
				if(!String.IsNullOrEmpty(reportHandle) && module != null) {
					XtraReport report = null;
					try {
						IReportContainer reportContainer = ReportDataProvider.ReportsStorage.GetReportContainerByHandle(reportHandle);
						Guard.ArgumentNotNull(reportContainer, "reportContainer");
						report = reportContainer.Report;
						module.ReportsDataSourceHelper.SetupBeforePrint(report, null, null, false, null, false);
						WriteReportPreviewToResponse(name, outputType, report);
					}
					finally {
						if(report != null) {
							report.Dispose();
						}
					}
				}
			}
		}
		protected virtual HttpResponse Response {
			get {
				return HttpContext.Current.Response;
			}
		}
		private void WriteReportPreviewToResponse(string name, ReportOutputType outputType, XtraReport report) {
			string contentType = "";
			using(MemoryStream stream = new MemoryStream()) {
				switch(outputType) {
					case ReportOutputType.Mht:
						report.ExportToMht(stream);
						contentType = "message/rfc822";
						break;
					case ReportOutputType.Rtf:
						report.ExportToRtf(stream);
						contentType = "application/msword";
						break;
					case ReportOutputType.Pdf:
						report.ExportToPdf(stream);
						contentType = "application/pdf";
						break;
					case ReportOutputType.Xls:
						contentType = "application/vnd.ms-excel";
						report.ExportToXls(stream);
						break;
				}
				HttpResponse response = Response;
				response.Clear();
				response.BufferOutput = false;   
				response.Cache.SetCacheability(HttpCacheability.NoCache);
				response.ContentType = contentType;
				response.AddHeader("Content-Length", stream.Length.ToString());
				response.AddHeader("content-disposition", "attachment; filename=\"" + name + "\"");
				response.ContentEncoding = System.Text.Encoding.UTF8;
				stream.WriteTo(response.OutputStream);
			}
		}
		public bool CanProcessRequest(HttpRequest request) {
			return request.QueryString["handlerName"] == "StreamExportResource";
		}
		public SessionStateBehavior SessionClientMode {
			get { return SessionStateBehavior.Required; }
		}
	}
}
