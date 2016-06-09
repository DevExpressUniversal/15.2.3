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
using DevExpress.XtraReports.Web.Azure.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	public class AzureReportManagementService : IReportManagementService {
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		readonly IDocumentBuilder buildManager;
		readonly IWebDocumentViewerReportResolver reportResolver;
		readonly IAzureEntityStorageManager entityStorageManager;
		readonly IAzureCommunicationService communicationService;
		readonly ConcurrentDictionary<string, ReportInfo> reportsDictionary = new ConcurrentDictionary<string, ReportInfo>();
		readonly Dictionary<string, TypedAction> broadcastMessagesHandlers = new Dictionary<string, TypedAction>();
		public AzureReportManagementService(IDocumentBuilder buildManager, IWebDocumentViewerReportResolver reportResolver, IAzureEntityStorageManager entityStorageManager, IAzureCommunicationService communicationService) {
			this.buildManager = buildManager;
			this.reportResolver = reportResolver;
			this.entityStorageManager = entityStorageManager;
			this.communicationService = communicationService;
			FillBroadcastActionsDictionary();
		}
		#region IReportManagementService
		public string GetId(object reportIdentity) {
			XtraReport report = (XtraReport)reportIdentity;
			string reportId = Guid.NewGuid().ToString("N");
			var reportInfo = new ReportInfo(report);
			entityStorageManager.Save(reportId, report);
			Func<bool> stopPredicate = () => { return reportInfo.IsDisposeRequested; };
			communicationService.ProcessIncomingBroadcastMessages(reportId, stopPredicate, broadcastMessagesHandlers);
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
		public ReportToPreview OpenReport(string reportName) {
			var report = reportResolver.Resolve(reportName);
			Guard.ArgumentNotNull(report, "report");
			string reportId = GetId(report);
			ReportParametersInfo parametersInfo = GetParameters(reportId);
			return ReportManagementServiceLogic.GetReportToPreview(reportId, report, parametersInfo);
		}
		public void Release(string id) {
			Release(id, false);
		}
		public void Clean(TimeSpan timeToLife) {
			var now = DateTime.UtcNow;
			List<string> garbageReportIds = reportsDictionary
				.Where(x => now - x.Value.LastAccessTimeStampUtc > timeToLife)
				.Select(x => x.Key)
				.ToList();
			ReportInfo ignore;
			foreach(string reportId in garbageReportIds) {
				reportsDictionary.TryRemove(reportId, out ignore);
			}
		}
		#endregion
		void Release(string id, bool isRemoteRequest) {
			ReportInfo reportInfo;
			if(!reportsDictionary.TryRemove(id, out reportInfo)) {
				return;
			}
			reportInfo.IsDisposeRequested = true;
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
			if(!isRemoteRequest) {
				communicationService.SendBroadcastMessage(id, RequestActionName.Release.ToString().ToLower(), id, TimeSpan.FromMinutes(30));
			}
		}
		void FillBroadcastActionsDictionary() {
			broadcastMessagesHandlers.Add(RequestActionName.Release.ToString().ToLower(), new TypedAction((s) => { Release(s, true); return null; }));
		}
		T DoWithReportById<T>(string id, Func<XtraReport, T> func) {
			ReportInfo reportInfo;
			if(!reportsDictionary.TryGetValue(id, out reportInfo)) {
				XtraReport report = entityStorageManager.LoadReport(id);
				if(report == null)
					throw new ArgumentException(string.Format("Report '{0}' not found", id), "id");
				reportInfo = new ReportInfo(report);
				reportsDictionary.AddOrUpdate(id, reportInfo, (_rId1, _rId2) => { return reportInfo; });
			}
			lock(reportInfo.SyncRoot) {
				return func(reportInfo.Report);
			}
		}
	}
}
