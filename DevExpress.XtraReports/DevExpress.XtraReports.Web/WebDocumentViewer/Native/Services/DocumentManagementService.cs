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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native.DrillDown;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public class DocumentManagementService : IDocumentManagementService {
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		static readonly TimeSpan serverLongTimeOut = TimeSpan.FromMinutes(2);
		readonly ConcurrentDictionary<string, DocumentInfo> hotDocuments = new ConcurrentDictionary<string, DocumentInfo>();
		readonly IDocumentStorage documentStorage;
		readonly IReportHashCodeGenerator documentHashGenerator;
		readonly IPollingOperationService<PrintingSystemBase, ExportedDocument> documentExportService;
		public DocumentManagementService(IDocumentStorage documentStorage, IReportHashCodeGenerator documentHashGenerator, IPollingOperationService<PrintingSystemBase, ExportedDocument> documentExportService) {
			this.documentStorage = documentStorage;
			this.documentHashGenerator = documentHashGenerator;
			this.documentExportService = documentExportService;
		}
		#region IDocumentStorage
		public DocumentDataResponse GetDocumentData(string id) {
			return documentStorage.DoWithBuildResult(id, x => DocumentManagementServiceLogic.GetDocumentData(x.Document, x.DrillDownKeys));
		}
		public IWebPreviewDocument CreateNew() {
			var id = documentStorage.CreateNew();
			var info = new DocumentInfo(id);
			hotDocuments.AddOrUpdate(id, info, (_1, _2) => info);
			return info;
		}
		public void UpdateHot(string id, Document document) {
			DocumentInfo documentInfo;
			if(!hotDocuments.TryGetValue(id, out documentInfo)) {
				throw new ArgumentException(string.Format("Document '{0}' not found", id), "id");
			}
			documentInfo.Document = document;
		}
		public void UpdateCold(string id, XtraReport report, Exception optionalBuildException) {
			DocumentInfo documentInfo;
			if(!hotDocuments.TryGetValue(id, out documentInfo)) {
				return;
			}
			bool noBuildException = optionalBuildException == null;
			try {
				var drillDownService = report.GetService<IDrillDownService>();
				Dictionary<string, bool> ddks = noBuildException
					? drillDownService.GetSerializableState()
					: null;
				string documentHash = noBuildException
					? documentHashGenerator.Generate(report)
					: null;
				documentStorage.Update(id, x => {
					if(optionalBuildException != null) {
						x.AssignBuildException(optionalBuildException);
						return;
					}
					x.AssignSuccessDocument(documentInfo.Document, ddks, documentHash);
				});
				documentInfo.Complete(optionalBuildException);
			} finally {
				DocumentInfo ignore;
				hotDocuments.TryRemove(id, out ignore);
				SyncExecuteAccumulatedRequests(documentInfo);
			}
		}
		public byte[] GetPage(DocumentPageRequest pageRequest) {
			return DoWithDocument(pageRequest.DocumentId, pageRequest.PageIndex, x => DocumentManagementServiceLogic.GetPage(x, pageRequest.PageIndex, pageRequest.Resolution));
		}
		public ExportedDocument ExportTo(string id, string format, ExportOptions options) {
			return documentStorage.DoWithBuildResult(id, x => DocumentManagementServiceLogic.ExportTo(x.Document.PrintingSystem, format, options));
		}
		public string StartExport(string id, string format, ExportOptions options) {
			return documentStorage.DoWithBuildResult(id, x => documentExportService.StartOperation(x.Document.PrintingSystem, (ps) => DocumentManagementServiceLogic.ExportTo(ps, format, options)));
		}
		public ProgressResponse GetExportStatus(string exportOperationId, int timeOut) {
			return documentExportService.GetOperationStatus(exportOperationId, timeOut);
		}
		public ExportedDocument GetExportResult(string exportOperationId, bool printable) {
			return documentExportService.GetOperationResult(exportOperationId, printable.ToString());
		}
		public void SyncExecuteAccumulatedRequests(string id) {
			DocumentInfo info;
			if(hotDocuments.TryGetValue(id, out info))
				SyncExecuteAccumulatedRequests(info);
		}
		void SyncExecuteAccumulatedRequests(DocumentInfo documentInfo) {
			IDocumentManagerRequest request;
			while(documentInfo.Requests.TryTake(out request)) {
				request.Execute(documentInfo.Document);
			}
		}
		public void StopBuild(string id) {
			DocumentInfo info;
			if(!hotDocuments.TryGetValue(id, out info)) {
				ThrowDocumentNotFoundException(id);
			}
			info.IsStopBuildRequested = true;
		}
		public void Release(string id) {
			DocumentInfo info;
			if(hotDocuments.TryRemove(id, out info))
				info.IsDisposeDocumentRequested = true;
			else
				documentStorage.Release(id);
		}
		public BrickMapResponse GetBrickMap(BrickMapRequest brickMapRequest) {
			return DoWithDocument(brickMapRequest.DocumentId, brickMapRequest.PageIndex, x => DocumentManagementServiceLogic.GetBrickMap(x, brickMapRequest.PageIndex));
		}
		public FindTextResponse FindText(FindTextRequest findTextRequest) {
			return DoWithDocument(findTextRequest.DocumentId, x => DocumentManagementServiceLogic.FindText(x, findTextRequest.Text, findTextRequest.MatchCase, findTextRequest.WholeWord));
		}
		public BuildStatusResponse GetBuildStatus(BuildStatusRequest buildStatusRequest) {
			string id = buildStatusRequest.DocumentId;
			DocumentInfo info;
			if(hotDocuments.TryGetValue(buildStatusRequest.DocumentId, out info)) {
				return GetHotBuildStatus(info.ProgressObservable, buildStatusRequest.TimeOut);
			}
			return documentStorage.DoWithBuildResult(id, x => DocumentManagementServiceLogic.GetColdBuildStatus(x.Document, x.FaultMessage));
		}
		#endregion
		public static void ThrowDocumentNotFoundException(string id) {
			throw new ArgumentException(string.Format("Document '{0}' not found", id), "id");
		}
		T DoWithDocument<T>(string id, Func<Document, T> func) {
			return DoWithDocument(id, null, func);
		}
		T DoWithDocument<T>(string id, int? loadPageIndex, Func<Document, T> func) {
			DocumentInfo info;
			if(hotDocuments.TryGetValue(id, out info)) {
				return WaitRequestResult(info, func);
			}
			return documentStorage.DoWithBuildResult(id, loadPageIndex, x => func(x.Document));
		}
		static T WaitRequestResult<T>(DocumentInfo info, Func<Document, T> func) {
			var request = new DocumentManagerRequest<T>(func);
			info.Requests.Add(request);
			var task = request.GetResultAsync();
			if(task.Wait(serverLongTimeOut)) {
				return task.Result;
			}
			throw new TimeoutException();
		}
		static BuildStatusResponse GetHotBuildStatus(HotWebDocumentProgressObservable progressObservable, int timeoutMilliseconds) {
			var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
			using(var observer = new DocumentProgressObserver<HotWebDocumentProgressObservable, HotWebPreviewDocumentProgress>(progressObservable)) {
				var task = observer.GetNextAsync();
				if(!task.Wait(timeout)) {
					return new BuildStatusResponse { RequestAgain = true };
				}
				HotWebPreviewDocumentProgress result = task.Result;
				return new BuildStatusResponse {
					Progress = result.Progress,
					PageCount = result.PagesCount,
					Completed = false,
					RequestAgain = true
				};
			}
		}
	}
}
