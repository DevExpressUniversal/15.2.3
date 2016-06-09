#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Linq;
using System.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Service.Native.DAL;
using DevExpress.XtraReports.Service.Native.Services.BinaryStore;
using DevExpress.XtraReports.Service.Native.Services.Factories;
using DevExpress.XtraReports.Service.Native.Services.Transient;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Service.Native.Services.Domain {
	public class DocumentBuildService : IDocumentBuildService {
		static Document LoadPages(IDocumentMediator mediator, int[] pageIndexes) {
			using(CultureSwitcher.FromMediator(mediator)) {
				return mediator.LoadPages(pageIndexes);
			}
		}
		readonly IDocumentMediatorFactory documentMediatorFactory;
		readonly IDocumentBuildFactory documentBuildFactory;
		readonly IPollingService pollingService;
		readonly IDocumentStoreService documentStoreService;
		readonly IDocumentExportService documentExportService;
		readonly IThreadFactoryService threadFactoryService;
		readonly ISerializationService serializationService;
		readonly IBinaryDataStorageServiceProvider binaryDataStorageServiceProvider;
		public DocumentBuildService(
			IDocumentMediatorFactory documentMediatorFactory,
			IDocumentBuildFactory documentBuildFactory,
			IPollingService pollingService,
			IDocumentStoreService documentStoreService,
			IDocumentExportService documentExportService,
			IThreadFactoryService threadFactoryService,
			ISerializationService serializationService,
			IBinaryDataStorageServiceProvider binaryDataStorageServiceProvider) {
			Guard.ArgumentNotNull(documentMediatorFactory, "documentMediatorFactory");
			Guard.ArgumentNotNull(documentBuildFactory, "documentBuildFactory");
			Guard.ArgumentNotNull(pollingService, "pollingService");
			Guard.ArgumentNotNull(documentStoreService, "documentStoreService");
			Guard.ArgumentNotNull(documentExportService, "documentExportService");
			Guard.ArgumentNotNull(threadFactoryService, "threadFactoryService");
			Guard.ArgumentNotNull(serializationService, "serializationService");
			Guard.ArgumentNotNull(binaryDataStorageServiceProvider, "binaryDataStorageServiceProvider");
			this.documentMediatorFactory = documentMediatorFactory;
			this.documentBuildFactory = documentBuildFactory;
			this.pollingService = pollingService;
			this.documentStoreService = documentStoreService;
			this.documentExportService = documentExportService;
			this.threadFactoryService = threadFactoryService;
			this.serializationService = serializationService;
			this.binaryDataStorageServiceProvider = binaryDataStorageServiceProvider;
		}
		#region IDocumentBuildService Members
		public DocumentId StartBuild(XtraReport report, ReportConfiguration reportConfiguration) {
			var documentId = documentStoreService.CreateEmptyDocument(TaskStatus.InProgress);
			var builder = documentBuildFactory.CreateDocumentBuilder();
			threadFactoryService.Start(() => builder.Build(documentId, report, reportConfiguration));
			return documentId;
		}
		public void StopBuild(DocumentId documentId) {
			using(var mediator = documentMediatorFactory.Create(documentId)) {
				if(mediator.DocumentRequestingAction != RequestingAction.None) {
					return;
				}
				mediator.DocumentRequestingAction = RequestingAction.Stop;
				mediator.Save();
			}
		}
		public BuildStatus GetBuildStatus(DocumentId documentId) {
			using(var mediator = documentMediatorFactory.Create(documentId)) {
				var entity = mediator.Entity;
				return new BuildStatus {
					DocumentId = documentId,
					Status = entity.Status,
					PageCount = entity.PageCount,
					ProgressPosition = entity.ProgressPosition,
					Fault = mediator.DocumentFault
				};
			}
		}
		public string GetPage(DocumentId documentId, int pageIndex, PageCompatibility compatibility, int? addition) {
			var pageIndexes = new[] { pageIndex };
			return GetPagesCore(documentId, compatibility, pageIndexes, addition,
				document => documentExportService.ExportPage(document, pageIndex, compatibility, addition),
				response => serializationService.DeserializeStrings(response).FirstOrDefault());
		}
		public byte[] GetPages(DocumentId documentId, int[] pageIndexes, PageCompatibility compatibility, int? addition) {
			return GetPagesCore(documentId, compatibility, pageIndexes, addition,
				document => documentExportService.ExportPages(document, pageIndexes, compatibility, addition),
				response => response);
		}
		public DocumentData GetDocumentData(DocumentId documentId) {
			using(var mediator = documentMediatorFactory.Create(documentId)) {
				var entity = mediator.Entity;
				if(entity.Status != TaskStatus.Complete) {
					throw new FaultException(Messages.FaultDocumentBuildingIsNotCompleted);
				}
				return new DocumentData {
					Name = mediator.DocumentName,
					DocumentMap = mediator.DocumentMap,
					SerializedPageData = mediator.SerializedPageData,
					SerializedWatermark = mediator.SerializedWatermark,
					ExportOptions = mediator.SerializedExportOptions,
					DrillDownKeys = mediator.DrillDownKeys,
					AvailableExportModes = mediator.DocumentAvailableExportModes,
					HiddenOptions = entity.HiddenOptions,
					CanChangePageSettings = entity.CanChangePageSettings
				};
			}
		}
		#endregion
		T GetPagesCore<T>(
			DocumentId documentId, PageCompatibility compatibility, int[] pageIndexes, int? addition,
			Func<Document, T> getFromDocument,
			Func<byte[], T> getFromPageResponse) {
			Guard.ArgumentNotNull(getFromDocument, "getFromDocument");
			Guard.ArgumentNotNull(getFromPageResponse, "getFromPageResponse");
			PagesRequestInformation pageRequestInformation = null;
			IDocumentMediator mediator = null;
			try {
				lock(documentBuildFactory.SyncRoot) {
					mediator = documentMediatorFactory.Create(documentId);
					switch(mediator.Entity.Status) {
						case TaskStatus.InProgress:
							pageRequestInformation = mediator.EnqueuePagesRequest(pageIndexes, compatibility, addition);
							mediator.Save();
							break;
						case TaskStatus.Complete:
							var document = LoadPages(mediator, pageIndexes);
							return getFromDocument(document);
						case TaskStatus.Fault:
						default:
							throw new FaultException(Messages.FaultDocumentBuildingIsNotCompleted);
					}
				}
				var serializedPageContent = GetPagesResponse(pageRequestInformation, mediator.Session);
				return getFromPageResponse(serializedPageContent);
			} finally {
				if(mediator != null) {
					mediator.Dispose();
				}
			}
		}
		byte[] GetPagesResponse(PagesRequestInformation pageRequest, UnitOfWork session) {
			byte[] result = null;
			pollingService.Do(() => {
				lock(documentBuildFactory.SyncRoot) {
					pageRequest.Reload();
					if(pageRequest.ResponseInformation != null) {
						var binaryDataStorageService = binaryDataStorageServiceProvider.GetService();
						var key = pageRequest.ResponseInformation.ExternalKey;
						result = binaryDataStorageService.LoadBytes(key, session);
						binaryDataStorageService.Delete(key, session);
					}
					return result != null;
				}
			});
			session.Delete(pageRequest);
			session.CommitChanges();
			return result;
		}
	}
}
