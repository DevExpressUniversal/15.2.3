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
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public class ExportPollingOperationService : IPollingOperationService<PrintingSystemBase, ExportedDocument> {
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		class ExportingDocumentObserver {
			public PrintingSystemBase PrintingSystem { get; set; }
			public ExportDocumentProgressObservable ProgressObservable { get; private set; }
			public ExportingDocumentObserver(PrintingSystemBase ps) {
				PrintingSystem = ps;
				ProgressObservable = new ExportDocumentProgressObservable(ps);
			}
		}
		ConcurrentDictionary<string, ExportingDocumentObserver> exportOperations;
		IExportedDocumentStorage exportedDocumentStorage;
		public ExportPollingOperationService(IExportedDocumentStorage exportedDocumentStorage) {
			this.exportedDocumentStorage = exportedDocumentStorage;
			exportOperations = new ConcurrentDictionary<string, ExportingDocumentObserver>();
		}
		public string StartOperation(PrintingSystemBase printingSystem, Func<PrintingSystemBase, ExportedDocument> doOperation) {
			var currentCulture = CultureInfo.CurrentCulture;
			var currentUICulture = CultureInfo.CurrentUICulture;
			string operationId = Guid.NewGuid().ToString("N");
			string response = null;
			ExportingDocumentObserver exportingDocumentObserver = new ExportingDocumentObserver(printingSystem);
			if(exportOperations.TryAdd(operationId, exportingDocumentObserver))
				response = operationId;
			else
				throw new InvalidOperationException("Unable to perform an export operation.");
			Task.Factory.StartNew<ExportedDocument>(() => {
				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentCulture = currentUICulture;
				return doOperation(printingSystem);
			}, TaskCreationOptions.LongRunning)
			.ContinueWith((t) => {
				Exception operationException = null;
				if(t.IsFaulted && t.Exception != null) {
					operationException = t.Exception;
					Logger.Error("StartExport error: " + t.Exception);
				}
				var documentInfo = new ExportedDocumentInfo() { ExportedDocument = t.Result, ExportException = operationException };
				try {
					exportedDocumentStorage.AddOrUpdate(operationId, documentInfo, (_id, oldDocumentInfo) => {
						oldDocumentInfo.ExportedDocument = documentInfo.ExportedDocument;
						oldDocumentInfo.ExportException = operationException;
						return oldDocumentInfo; 
					});
				} catch {
					if(operationException == null)
						operationException = new InvalidOperationException("Could not add an exported document to the document storage.");
				}
				exportingDocumentObserver.ProgressObservable.Complete(operationException);
			});
			return response;
		}
		public ProgressResponse GetOperationStatus(string id, int timeoutMilliseconds) {
			ExportingDocumentObserver exportOperation;
			if(!exportOperations.TryGetValue(id, out exportOperation)) {
				ExportedDocumentInfo exportedDocumentInfo = null;
				string faultMessage = exportedDocumentStorage.TryTake(id, out exportedDocumentInfo)
					? "There are no export operations with the id: " + id
					: exportedDocumentInfo.ExportException != null ? exportedDocumentInfo.ExportException.Message : "";
				return exportedDocumentInfo != null && exportedDocumentInfo.ExportException == null
					? new ProgressResponse {
						Progress = 100,
						Completed = true,
						RequestAgain = false
					}
					: new ProgressResponse {
						Progress = 0,
						Completed = false,
						FaultMessage = faultMessage,
						RequestAgain = false
					};
			}
			var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
			using(var observer = new DocumentProgressObserver<ExportDocumentProgressObservable, OperationProgress>(exportOperation.ProgressObservable)) {
				var task = observer.GetNextAsync();
				if(!task.Wait(timeout)) {
					return new ProgressResponse { RequestAgain = true };
				}
				OperationProgress result = task.Result;
				return new ProgressResponse {
					Progress = result.Progress,
					Completed = result.IsComplete,
					RequestAgain = !result.IsComplete
				};
			}
		}
		public void CancelOperation(string id) {
			throw new NotSupportedException();
		}
		public ExportedDocument GetOperationResult(string id, params string[] args) {
			ExportedDocumentInfo exportedDocumentInfo;
			if(!exportedDocumentStorage.TryGet(id, out exportedDocumentInfo) || exportedDocumentInfo == null || exportedDocumentInfo.ExportException != null) {
				throw exportedDocumentInfo != null && exportedDocumentInfo.ExportException != null ? exportedDocumentInfo.ExportException : new InvalidOperationException("Could not get an exported document from storage");
			}
			return exportedDocumentInfo.ExportedDocument;
		}
	}
}
