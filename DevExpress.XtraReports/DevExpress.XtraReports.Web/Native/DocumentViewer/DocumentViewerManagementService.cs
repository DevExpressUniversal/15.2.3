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
using System.Threading.Tasks;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.ReportServer.ServiceModel.Native.RemoteOperations;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Web.Native.DocumentViewer {
	public class DocumentViewerManagementService : IDocumentViewerManagementService {
		readonly IReportServiceClient client;
		event Action<DocumentMapTreeViewNode, Dictionary<string, bool>> receiveDocumentData;
		public DocumentViewerManagementService(IReportServiceClient client) {
			Guard.ArgumentNotNull(client, "client");
			this.client = client;
		}
		#region IDocumentViewerManagementService
		public Task<ReportParameterContainer> GetReportParameters(InstanceIdentity identity) {
			var tcs = new TaskCompletionSource<ReportParameterContainer>();
			EventHandler<ScalarOperationCompletedEventArgs<ReportParameterContainer>> getParametersHandler = null;
			client.GetReportParametersCompleted += getParametersHandler = (s, e) => {
				client.GetReportParametersCompleted -= getParametersHandler;
				TaskCompletionSourceResultHandler.ProcessResult(tcs, e, () => e.Result);
			};
			client.GetReportParametersAsync(identity, null);
			return tcs.Task;
		}
		public Task<ParameterLookUpValues[]> GetLookupValues(InstanceIdentity identity, ReportParameter[] parameterValues, string[] requiredParameterPaths) {
			var tcs = new TaskCompletionSource<ParameterLookUpValues[]>();
			EventHandler<ScalarOperationCompletedEventArgs<ParameterLookUpValues[]>> completed = null;
			completed = (_, e) => {
				client.GetLookUpValuesCompleted -= completed;
				TaskCompletionSourceResultHandler.ProcessResult(tcs, e, () => e.Result);
			};
			client.GetLookUpValuesCompleted += completed;
			client.GetLookUpValuesAsync(identity, parameterValues, requiredParameterPaths, null);
			return tcs.Task;
		}
		public Task<RemoteDocumentInformation> BuildAsync(InstanceIdentity identity, ReportBuildArgs buildArgs) {
			var tcs = new TaskCompletionSource<RemoteDocumentInformation>();
			ReportBuildArgs args = buildArgs ?? Helper.CreateReportBuildArgs(new DefaultValueParameterContainer(), null, null, null);
			var operation = new CreateDocumentOperation(client, identity, args, false, Helper.DefaultStatusUpdateInterval);
			int pageCount = 0;
			EventHandler<CreateDocumentCompletedEventArgs> completeHandler = null;
			EventHandler<CreateDocumentProgressEventArgs> progressHandler = null;
			operation.Progress += progressHandler = (s, e) => {
				pageCount = e.PageCount;
			};
			operation.Completed += completeHandler = (s, e) => {
				operation.Completed -= completeHandler;
				operation.Progress -= progressHandler;
				if(!TaskCompletionSourceResultHandler.TryProcessException(tcs, e)) {
					ProcessDocumentData(tcs, e.DocumentId, pageCount);
				}
			};
			operation.Start();
			return tcs.Task;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public void OnReceiveDocumentData(Action<DocumentMapTreeViewNode, Dictionary<string, bool>> receiveDocumentData) {
			this.receiveDocumentData += receiveDocumentData;
		}
		public Task<byte[]> GetPageAsync(DocumentId documentId, int pageIndex) {
			return GetPagesAsync(documentId, new[] { pageIndex });
		}
		public Task<byte[]> GetPagesAsync(DocumentId documentId, int[] pageIndexes) {
			var tcs = new TaskCompletionSource<byte[]>();
			EventHandler<ScalarOperationCompletedEventArgs<byte[]>> getPagesHandler = null;
			client.GetPagesCompleted += getPagesHandler = (s, e) => {
				client.GetPagesCompleted -= getPagesHandler;
				TaskCompletionSourceResultHandler.ProcessResult(tcs, e, () => e.Result);
			};
			client.GetPagesAsync(documentId, pageIndexes, PageCompatibility.Prnx, null);
			return tcs.Task;
		}
		public Task<byte[]> Export(DocumentId documentId, ExportFormat exportFormat, ExportOptions exportOptions) {
			var tcs = new TaskCompletionSource<byte[]>();
			var factory = new RemoteOperationFactory(client, documentId, Helper.DefaultStatusUpdateInterval);
			var operation = factory.CreateExportDocumentOperation(exportFormat, exportOptions);
			EventHandler<ExportDocumentCompletedEventArgs> completed = null;
			operation.Completed += completed = (s, e) => {
				operation.Completed -= completed;
				TaskCompletionSourceResultHandler.ProcessResult(tcs, e, () => e.Data);
			};
			operation.Start();
			return tcs.Task;
		}
		#endregion
		void ProcessDocumentData(TaskCompletionSource<RemoteDocumentInformation> tcs, DocumentId documentId, int pageCount) {
			EventHandler<ScalarOperationCompletedEventArgs<DocumentData>> getDocumentData = null;
			client.GetDocumentDataCompleted += getDocumentData = (s, e) => {
				client.GetDocumentDataCompleted -= getDocumentData;
				TaskCompletionSourceResultHandler.ProcessResult(tcs, e, () => RaiseOnReceiveDocumentMapAndCreateResult(documentId, pageCount, e.Result.DocumentMap, e.Result.DrillDownKeys));
			};
			client.GetDocumentDataAsync(documentId, null);
		}
		RemoteDocumentInformation RaiseOnReceiveDocumentMapAndCreateResult(DocumentId id, int pageCount, DocumentMapTreeViewNode documentMap, Dictionary<string, bool> drillDownKeys) {
			if(receiveDocumentData != null) {
				receiveDocumentData(documentMap, drillDownKeys);
			}
			return new RemoteDocumentInformation {
				DocumentId = id,
				PageCount = pageCount
			};
		}
	}
}
