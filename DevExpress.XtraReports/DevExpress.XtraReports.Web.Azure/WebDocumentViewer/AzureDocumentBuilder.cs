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
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.Native.DrillDown;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
using Thread = System.Threading.Thread;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	public class AzureDocumentBuilder : IDocumentBuilder {
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		readonly IReportHashCodeGenerator reportHashCodeGenerator;
		readonly IDocumentManagementService documentManagementService;
		readonly IDocumentStorageCachable documentStorageCachable;
		readonly UrlResolver urlResolver;
		public AzureDocumentBuilder(IReportHashCodeGenerator reportHashCodeGenerator, IDocumentManagementService documentManagementService, IDocumentStorage documentStorage, UrlResolver urlResolver) {
			this.reportHashCodeGenerator = reportHashCodeGenerator;
			this.documentManagementService = documentManagementService;
			this.documentStorageCachable = documentStorage as IDocumentStorageCachable;
			this.urlResolver = urlResolver;
		}
		#region IBuildManager
		public string StartBuild(XtraReport report, Dictionary<string, bool> drillDownState) {
			var reportDrillDownService = report.GetService<IDrillDownService>();
			ReportManagementServiceLogic.UpdateDrillDownState(reportDrillDownService, drillDownState);
			string cachedId;
			if(TryGetCachedDocumentId(report, out cachedId)) {
				return cachedId;
			}
			var backgroundBuildStrategy = new WebBackgroundPageBuildEngineStrategy();
			report.PrintingSystem.ReplaceService<BackgroundPageBuildEngineStrategy>(backgroundBuildStrategy);
			IWebPreviewDocument info = documentManagementService.CreateNew();
			string documentId = info.Id;
			var webBackgroundService = new WebBackgroundService(report.PrintingSystem, () => info.IsStopBuildRequested);
			report.PrintingSystem.ReplaceService<IBackgroundService>(webBackgroundService);
			report.PrintingSystem.ReplaceService<UrlResolver>(urlResolver);
			var currentCulture = CultureInfo.CurrentCulture;
			var currentUICulture = CultureInfo.CurrentUICulture;
			Task.Factory.StartNew(() => {
				Exception buildException = null;
				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentCulture = currentUICulture;
				try {
					var stopped = false;
					report.CreateDocument(true);
					if(!info.IsDisposeDocumentRequested)
						documentManagementService.UpdateHot(documentId, report.PrintingSystem.Document);
					while(report.PrintingSystem.Document.IsCreating) {
						documentManagementService.SyncExecuteAccumulatedRequests(documentId);
						if(!stopped && info.IsStopBuildRequested) {
							report.StopPageBuilding();
							stopped = true;
						}
						if(report.PrintingSystem.Document.IsCreating)
							backgroundBuildStrategy.DoTick();
					}
				} catch(Exception ex) {
					Logger.Error("StartBuild: " + ex);
					buildException = ex;
				}
				if(!info.IsDisposeDocumentRequested) {
					documentManagementService.UpdateCold(documentId, report, buildException);
				}
			}, TaskCreationOptions.LongRunning);
			return documentId;
		}
		#endregion
		bool TryGetCachedDocumentId(XtraReport report, out string id) {
			id = null;
			if(documentStorageCachable == null) {
				return false;
			}
			var cache = reportHashCodeGenerator.Generate(report);
			if(cache == null) {
				return false;
			}
			id = documentStorageCachable.FindCachedDocumentId(cache);
			return id != null;
		}
	}
}
