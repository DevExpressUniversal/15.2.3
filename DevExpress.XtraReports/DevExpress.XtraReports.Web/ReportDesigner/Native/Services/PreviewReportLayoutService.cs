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

using System;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public class PreviewReportLayoutService : IPreviewReportLayoutService {
		readonly Func<IReportManagementService> reportManagementServiceFactory;
		public PreviewReportLayoutService(Func<IReportManagementService> reportManagementServiceFactory) {
			this.reportManagementServiceFactory = reportManagementServiceFactory;
		}
		public ReportToPreview InitializePreview(string reportLayout) {
			Guard.ArgumentNotNull(reportLayout, "reportLayoutJson");
			IReportManagementService reportManagementService = reportManagementServiceFactory();
			XtraReport report = ReportLayoutJsonSerializer.CreateReportFromJson(reportLayout);
			report.ReportPrintOptions.ActivateDesignTimeProperties(true);
			InitializeReportInstance(report);
			string reportId = reportManagementService.GetId(report);
			ReportParametersInfo parametersInfo = reportManagementService.GetParameters(reportId);
			return ReportManagementServiceLogic.GetReportToPreview(reportId, report, parametersInfo);
		}
		protected virtual void InitializeReportInstance(XtraReport report) { }
	}
}
