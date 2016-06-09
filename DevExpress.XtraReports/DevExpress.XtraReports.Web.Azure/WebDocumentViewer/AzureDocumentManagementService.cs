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
using DevExpress.XtraReports.Web.Azure.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	public enum RequestActionName {
		GetPage,
		GetBrickMap,
		GetBuildStatus,
		StopBuild,
		Release
	}
	public class AzureDocumentManagementService : IDocumentManagementService {
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		static readonly TimeSpan serverLongTimeOut = TimeSpan.FromMinutes(2);
		static readonly int requestsTimeOut = 120;
		readonly ConcurrentDictionary<string, DocumentInfo> hotDocuments = new ConcurrentDictionary<string, DocumentInfo>();
		readonly IDocumentStorage documentStorage;
		readonly IReportHashCodeGenerator documentHashGenerator;
		readonly IPollingOperationService<PrintingSystemBase, ExportedDocument> documentExportService;
		readonly IAzureCommunicationService communicationService;
		readonly Dictionary<string, TypedAction> requestHandlers = new Dictionary<string, TypedAction>();
		public AzureDocumentManagementService(IDocumentStorage documentStorage, IReportHashCodeGenerator documentHashGenerator, IPollingOperationService<PrintingSystemBase, ExportedDocument> documentExportService, IAzureCommunicationService communicationService) {
			this.documentStorage = documentStorage;
			this.documentHashGenerator = documentHashGenerator;
			this.documentExportService = documentExportService;
			this.communicationService = communicationService;
			FillRequestDictionaries();
		}
		#region IDocumentManagementService
		public DocumentDataResponse GetDocumentData(string id) {
			return documentStorage.DoWithBuildResult(id, x => DocumentManagementServiceLogic.GetDocumentData(x.Document, x.DrillDownKeys));
		}
		public BrickMapResponse GetBrickMap(BrickMapRequest brickMapRequest) {
			Func<BrickMapResponse> remoteDocumentFunc = () => {
				string actionName = ToLowerCaseString(RequestActionName.GetBrickMap);
				string jsonArgs = JsonSerializer.Stringify<BrickMapRequest>(brickMapRequest, null);
				return communicationService.Request<BrickMapResponse>(brickMapRequest.DocumentId, actionName, jsonArgs, requestsTimeOut, null);
			};
			return DoWithDocument(brickMapRequest.DocumentId, brickMapRequest.PageIndex, x => DocumentManagementServiceLogic.GetBrickMap(x, brickMapRequest.PageIndex), remoteDocumentFunc);
		}
		public FindTextResponse FindText(FindTextRequest findTextRequest) {
			return documentStorage.DoWithBuildResult(findTextRequest.DocumentId, x => DocumentManagementServiceLogic.FindText(x.Document, findTextRequest.Text, findTextRequest.MatchCase, findTextRequest.WholeWord));
		}
		public IWebPreviewDocument CreateNew() {
			var id = documentStorage.CreateNew();
			var info = new DocumentInfo(id);
			hotDocuments.AddOrUpdate(id, info, (_1, _2) => info);
			Func<bool> stopListeningPredicate = () => { return info.IsStopBuildRequested || info.IsDisposeDocumentRequested || info.IsBuildCompleted; };
			communicationService.ProcessIncomingSessionRequests(id, stopListeningPredicate, requestHandlers, null);
			return info;
		}
		public byte[] GetPage(DocumentPageRequest pageRequest) {
			Func<byte[]> remodeGetPage = () => {
				string actionName = ToLowerCaseString(RequestActionName.GetPage);
				string jsonArgs = JsonSerializer.Stringify<DocumentPageRequest>(pageRequest, null);
				return communicationService.Request<byte[]>(pageRequest.DocumentId, actionName, jsonArgs, requestsTimeOut, null);
			};
			return DoWithDocument(pageRequest.DocumentId, pageRequest.PageIndex, x => DocumentManagementServiceLogic.GetPage(x, pageRequest.PageIndex, pageRequest.Resolution), remodeGetPage);
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
		public BuildStatusResponse GetBuildStatus(BuildStatusRequest buildStatusRequest) {
			string id = buildStatusRequest.DocumentId;
			DocumentInfo info;
			if(hotDocuments.TryGetValue(buildStatusRequest.DocumentId, out info)) {
				return GetHotBuildStatus(info.ProgressObservable, buildStatusRequest.TimeOut);
			} else if(documentStorage.BlankExists(id)) {
				string jsonArgs = JsonSerializer.Stringify<BuildStatusRequest>(buildStatusRequest, null);
				string actionName = ToLowerCaseString(RequestActionName.GetBuildStatus);
				return communicationService.Request<BuildStatusResponse>(buildStatusRequest.DocumentId, actionName, jsonArgs, buildStatusRequest.TimeOut, null);
			}
			return documentStorage.DoWithBuildResult(id, x => DocumentManagementServiceLogic.GetColdBuildStatus(x.Document, x.FaultMessage));
		}
		public void UpdateHot(string id, XtraPrinting.Document document) {
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
				Dictionary<string, bool> ddks = noBuildException ? drillDownService.GetSerializableState() : null;
				string documentHash = noBuildException ? documentHashGenerator.Generate(report) : null;
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
		public void SyncExecuteAccumulatedRequests(string id) {
			DocumentInfo info;
			if(hotDocuments.TryGetValue(id, out info)) {
				SyncExecuteAccumulatedRequests(info);
			}
		}
		public void StopBuild(string id) {
			DocumentInfo info;
			if(hotDocuments.TryGetValue(id, out info)) {
				info.IsStopBuildRequested = true;
			} else if(documentStorage.BlankExists(id)) {
				communicationService.Request(id, ToLowerCaseString(RequestActionName.StopBuild), id, TimeSpan.FromMinutes(30));
			} else {
				DocumentManagementService.ThrowDocumentNotFoundException(id);
			}
		}
		public void Release(string id) {
			Release(id, false);
		}
		#endregion
		void Release(string id, bool fromRemoteRequest) {
			DocumentInfo info;
			if(hotDocuments.TryRemove(id, out info)) {
				info.IsDisposeDocumentRequested = true;
			} else if(!fromRemoteRequest && documentStorage.BlankExists(id)) {
				communicationService.Request(id, ToLowerCaseString(RequestActionName.Release), id, TimeSpan.FromMinutes(30));
			} else {
				documentStorage.Release(id);
			}
		}
		void FillRequestDictionaries() {
			requestHandlers.Add(ToLowerCaseString(RequestActionName.Release), new TypedAction((s) => { Release(s, true); return null; }));
			requestHandlers.Add(ToLowerCaseString(RequestActionName.StopBuild), new TypedAction((s) => { StopBuild(s); return null; }));
			requestHandlers.Add(ToLowerCaseString(RequestActionName.GetBrickMap), new TypedAction((s) => {
				var request = ActionHelper.Read<BrickMapRequest>(s);
				return GetBrickMap(request);
			}, typeof(BrickMapResponse)));
			requestHandlers.Add(ToLowerCaseString(RequestActionName.GetBuildStatus), new TypedAction((s) => {
				var request = ActionHelper.Read<BuildStatusRequest>(s);
				return GetBuildStatus(request);
			}, typeof(BuildStatusResponse)));
			requestHandlers.Add(ToLowerCaseString(RequestActionName.GetPage), new TypedAction((s) => {
				var request = ActionHelper.Read<DocumentPageRequest>(s);
				return GetPage(request);
			}, typeof(byte[])));
		}
		string ToLowerCaseString(Enum val) {
			return val.ToString().ToLower();
		}
		void SyncExecuteAccumulatedRequests(DocumentInfo documentInfo) {
			IDocumentManagerRequest request;
			while(documentInfo.Requests.TryTake(out request)) {
				request.Execute(documentInfo.Document);
			}
		}
		T DoWithDocument<T>(string id, int? loadPageIndex, Func<Document, T> func, Func<T> remoteDocumentFunc) {
			DocumentInfo info;
			if(hotDocuments.TryGetValue(id, out info)) {
				return WaitRequestResult(info, func);
			} else if(documentStorage.BlankExists(id)) {
				return remoteDocumentFunc();
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
