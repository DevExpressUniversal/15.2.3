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
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraReports.Service.Native.DAL;
using DevExpress.XtraReports.Service.Native.Services.Factories;
using DevExpress.XtraReports.Service.Native.Services.Transient;
namespace DevExpress.XtraReports.Service.Native.Services.Domain {
	public class DocumentStoreService : IDocumentStoreService {
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		protected IDocumentMediatorFactory DocumentMediatorFactory {
			get { return documentMediatorFactory; }
		}
		readonly IDocumentMediatorFactory documentMediatorFactory;
		readonly IPollingService pollingService;
		readonly IDALService dalService;
		public DocumentStoreService(
			IDocumentMediatorFactory documentMediatorFactory,
			IPollingService pollingService,
			IDALService dalService) {
			Guard.ArgumentNotNull(documentMediatorFactory, "documentMediatorFactory");
			Guard.ArgumentNotNull(pollingService, "pollingService");
			Guard.ArgumentNotNull(dalService, "dalService");
			this.documentMediatorFactory = documentMediatorFactory;
			this.pollingService = pollingService;
			this.dalService = dalService;
		}
		#region IDocumentContainerService Members
		public virtual DocumentId CreateEmptyDocument(TaskStatus defaultDocumentStatus) {
			return CreateDocumentCore(defaultDocumentStatus);
		}
		public virtual void ClearDocument(DocumentId documentId) {
			using(var mediator = documentMediatorFactory.Create(documentId)) {
				if(mediator.Entity.Status == TaskStatus.InProgress) {
					mediator.DocumentRequestingAction = RequestingAction.Delete;
					mediator.Save();
					return;
				}
				var session = mediator.Session;
				pollingService.Do(() => !PagesRequestInformation.FindUnansweredByDocument(documentId, session).Any());
				Logger.Info("({0}) Clearing document", documentId.Value);
				mediator.Delete();
			}
		}
		#endregion
		DocumentId CreateDocumentCore(TaskStatus defaultDocumentStatus, Action<IDocumentMediator> configureMediator = null) {
			var documentId = DocumentId.GenerateNew();
			using(var mediator = documentMediatorFactory.Create(documentId, MediatorInitialization.New)) {
				if(configureMediator != null) {
					configureMediator(mediator);
				}
				mediator.Entity.Status = defaultDocumentStatus;
				mediator.Save();
				Logger.Info("({0}) StoredDocument is created", documentId.Value);
			}
			return documentId;
		}
	}
}
