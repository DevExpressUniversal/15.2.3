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
using System.Text;
using DevExpress.XtraReports.Web;
using System.Web;
using DevExpress.ExpressApp.Web;
using System.Web.SessionState;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.Reports.Web {
	public class ReportExportHttpHandler : IXafHttpHandler {
		private readonly object lockObject = new object();
		private void WriteReportPreviewToResponse(XafReport report, ReportOutputType outputType) {
			switch(outputType) {
				case ReportOutputType.Mht:
					ReportViewer.WriteMhtTo(HttpContext.Current.Response, report);
					break;
				case ReportOutputType.Rtf:
					ReportViewer.WriteRtfTo(HttpContext.Current.Response, report);
					break;
				case ReportOutputType.Pdf:
					ReportViewer.WritePdfTo(HttpContext.Current.Response, report);
					break;
				case ReportOutputType.Xls:
					ReportViewer.WriteXlsTo(HttpContext.Current.Response, report);
					break;
			}
		}
		public ReportExportHttpHandler() { }
		public static string GetExportingReportResourceUrl(string reportDataHandle, ReportOutputType outputType) {
			return string.Format("DXX.axd?handlerName=ReportExportResource&handle={0}&outputType={1}", reportDataHandle, outputType);
		}
		public void ProcessRequest(HttpContext context) {
			lock(lockObject) {
				string reportDataHandle = context.Request.QueryString["handle"];
				ReportOutputType outputType = (ReportOutputType)Enum.Parse(typeof(ReportOutputType), context.Request.QueryString["outputType"]);
				if(!String.IsNullOrEmpty(reportDataHandle)) {
					Type objectType = null;
					String keyString = "";
					ObjectHandleHelper.ParseObjectHandle(WebApplication.Instance.TypesInfo, reportDataHandle, out objectType, out keyString);
					IObjectSpace objectSpace = WebApplication.Instance.CreateObjectSpace(objectType);
					IReportData reportData = (IReportData)objectSpace.GetObjectByHandle(reportDataHandle);
					XafReport report = (XafReport)reportData.LoadReport(objectSpace);
					WriteReportPreviewToResponse(report, outputType);
				}
			}
		}
		public bool CanProcessRequest(HttpRequest request) {
			return request.QueryString["handlerName"] == "ReportExportResource";
		}
		public SessionStateBehavior SessionClientMode {
			get { return SessionStateBehavior.Required; }
		}
	}
}
