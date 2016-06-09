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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public class ReportManagementService : IReportManagementService {
		readonly ConcurrentDictionary<string, ReportInfo> reportsDictionary = new ConcurrentDictionary<string, ReportInfo>();
		readonly IDocumentBuilder buildManager;
		readonly IWebDocumentViewerReportResolver reportResolver;
		public ReportManagementService(IDocumentBuilder buildManager, IWebDocumentViewerReportResolver reportResolver) {
			this.buildManager = buildManager;
			this.reportResolver = reportResolver;
		}
		#region IReportManagementService
		public string GetId(object reportIdentity) {
			XtraReport report = (XtraReport)reportIdentity;
			string reportId = Guid.NewGuid().ToString("N");
			var reportInfo = new ReportInfo(report);
			return reportsDictionary.TryAdd(reportId, reportInfo) ? reportId : null;
		}
		public ReportParametersInfo GetParameters(string id) {
			return DoWithReportById(id, ReportManagementServiceLogic.GetParametersInfo);
		}
		public LookUpValuesResponse GetLookUpValues(string id, Dictionary<string, object> parameters, string[] requiredParameterPaths) {
			return DoWithReportById(id, x => ReportManagementServiceLogic.GetLookUpValues(x, parameters, requiredParameterPaths));
		}
		public StartBuildResponse StartBuild(string id, Dictionary<string, object> parameters, Dictionary<string, bool> drillDownState) {
			return DoWithReportById(id, x => ReportManagementServiceLogic.StartBuild(buildManager, x, parameters, drillDownState));
		}
		public ReportToPreview OpenReport(string reportTypeName) {
			UI.XtraReport report = reportResolver.Resolve(reportTypeName);
			Guard.ArgumentNotNull(report, "report");
			string reportId = GetId(report);
			ReportParametersInfo parametersInfo = GetParameters(reportId);
			return ReportManagementServiceLogic.GetReportToPreview(reportId, report, parametersInfo);
		}
		public void Release(string id) {
			ReportInfo reportInfo;
			if(!reportsDictionary.TryRemove(id, out reportInfo)) {
				return;
			}
			var report = reportInfo.Report;
			Task.Factory.StartNew(() => {
				var stopwatch = Stopwatch.StartNew();
				TimeSpan timeOut = TimeSpan.FromMinutes(2);
				while(ReportManagementServiceLogic.IsNotDisposed(report) && report.PrintingSystem.Document.IsCreating && stopwatch.Elapsed < timeOut) {
					System.Threading.Thread.Sleep(1);
				}
				if(!report.IsDisposed) {
					System.Threading.Thread.Sleep(1000);
					report.Dispose();
				}
			}, TaskCreationOptions.LongRunning);
		}
		public void Clean(TimeSpan lifetime) {
			var now = DateTime.UtcNow;
			List<string> garbageReportIds = reportsDictionary
				.Where(x => now - x.Value.LastAccessTimeStampUtc > lifetime)
				.Select(x => x.Key)
				.ToList();
			ReportInfo ignore;
			foreach(string reportId in garbageReportIds) {
				reportsDictionary.TryRemove(reportId, out ignore);
			}
		}
		#endregion
		T DoWithReportById<T>(string id, Func<XtraReport, T> func) {
			ReportInfo reportInfo;
			if(!reportsDictionary.TryGetValue(id, out reportInfo)) {
				throw new ArgumentException(string.Format("Report '{0}' not found", id), "id");
			}
			lock(reportInfo.SyncRoot) {
				return func(reportInfo.Report);
			}
		}
	}
}
