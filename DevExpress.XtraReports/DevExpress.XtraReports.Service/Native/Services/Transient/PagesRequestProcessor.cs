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
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Service.Native.DAL;
using DevExpress.XtraReports.Service.Native.Services.BinaryStore;
using DevExpress.XtraReports.Service.Native.Services.Domain;
using DevExpress.XtraReports.Service.Native.Services.Factories;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public class PagesRequestProcessor : IPagesRequestProcessor {
		static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		readonly IDocumentMediatorFactory documentMediatorFactory;
		readonly IThreadFactoryService threadFactoryService;
		readonly ISerializationService serializationService;
		readonly IDocumentExportService documentExportService;
		readonly IDocumentBuildFactory documentBuildFactory;
		readonly IBinaryDataStorageServiceProvider binaryDataStorageServiceProvider;
#if DEBUGTEST
		internal event Action<byte[]> PageExportedTEST;
#endif
		public PagesRequestProcessor(
			IDocumentMediatorFactory documentMediatorFactory,
			IThreadFactoryService threadFactoryService,
			ISerializationService serializationService,
			IDocumentExportService documentExportService,
			IDocumentBuildFactory documentBuildFactory,
			IBinaryDataStorageServiceProvider binaryDataStorageServiceProvider) {
			Guard.ArgumentNotNull(documentMediatorFactory, "documentMediatorFactory");
			Guard.ArgumentNotNull(threadFactoryService, "threadFactoryService");
			Guard.ArgumentNotNull(serializationService, "serializationService");
			Guard.ArgumentNotNull(documentExportService, "documentExportService");
			Guard.ArgumentNotNull(documentBuildFactory, "documentBuildFactory");
			Guard.ArgumentNotNull(binaryDataStorageServiceProvider, "binaryDataStorageServiceProvider");
			this.documentMediatorFactory = documentMediatorFactory;
			this.threadFactoryService = threadFactoryService;
			this.serializationService = serializationService;
			this.documentExportService = documentExportService;
			this.documentBuildFactory = documentBuildFactory;
			this.binaryDataStorageServiceProvider = binaryDataStorageServiceProvider;
		}
		#region IPagesRequestProcessor Members
		public void Process(PrintingSystemBase printingSystem, DocumentId documentId, IDuplexEventWaitHandle waitHandle) {
			while(true) {
				lock(documentBuildFactory.SyncRoot) {
					using(var mediator = documentMediatorFactory.Create(documentId)) {
						var request = mediator.DequeuePagesRequest();
						if(request != null) {
							try {
								CreatePage(printingSystem.Document, request, documentId);
							} catch(Exception e) {
								Logger.Error("({0}) CreatePage can't be performed - {1}", documentId.Value, e);
							} finally {
								mediator.Save();
							}
						}
					}
				}
				if(waitHandle.Wait(threadFactoryService.SleepTimeout)) {
					Logger.Info("({0}) PagesRequestProcessor.Process - minor thread is completed", documentId.Value);
					break;
				}
			}
		}
		#endregion
		void CreatePage(Document document, PagesRequestInformation request, DocumentId documentId) {
			var pageIndexes = serializationService.DeserializeIndexes(request.SerializedPageIndexes);
			byte[] result = null;
			try {
				result = documentExportService.ExportPages(document, pageIndexes, request.Compatibility, request.Addition, true);
			} catch(Exception exception) {
				Logger.Error("({0}) PagesRequestProcessor.CreatePage - {1}", documentId.Value, exception);
			}
			IBinaryDataStorageService binaryDataStorageService = binaryDataStorageServiceProvider.GetService();
			string externalKey = binaryDataStorageService.Create(result, request.Session); 
			request.ResponseInformation = new PagesResponseInformation(request.Session) { ExternalKey = externalKey };
#if DEBUGTEST
			if(PageExportedTEST != null) {
				PageExportedTEST(result);
			}
#endif
		}
	}
}
