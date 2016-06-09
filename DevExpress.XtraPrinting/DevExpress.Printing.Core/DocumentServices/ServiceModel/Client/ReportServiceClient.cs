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
using System.ComponentModel;
using System.ServiceModel.Channels;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
namespace DevExpress.DocumentServices.ServiceModel.Client {
	public class ReportServiceClient : ServiceClientBase, IReportServiceClient {
		#region Properties
		protected new IAsyncReportService Channel {
			get { return (IAsyncReportService)base.Channel; }
		}
		#endregion
		#region ctor
		public ReportServiceClient(IAsyncReportService channel)
			: base((IChannel)channel) {
		}
		#endregion
		#region GetReportInformation
		public void GetReportParametersAsync(InstanceIdentity identity, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetReportParameters, () => GetReportParametersCompleted);
			Channel.BeginGetReportParameters(identity, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<ReportParameterContainer>> GetReportParametersCompleted;
		#endregion
		#region GetLookUpValues
		[Obsolete("Use the GetLookUpValuesAsync method instead.")]
		public void GetLookUpValues(InstanceIdentity identity, ReportParameter[] parameterValues, string[] requiredParameterPaths, object asyncState) {
		}
		public void GetLookUpValuesAsync(InstanceIdentity identity, ReportParameter[] parameterValues, string[] requiredParameterPaths, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetLookUpValues, () => GetLookUpValuesCompleted);
			Channel.BeginGetLookUpValues(identity, parameterValues, requiredParameterPaths, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<ParameterLookUpValues[]>> GetLookUpValuesCompleted;
		#endregion
		#region StartBuild
		public void StartBuildAsync(InstanceIdentity identity, ReportBuildArgs buildArgs, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndStartBuild, () => StartBuildCompleted);
			Channel.BeginStartBuild(identity, buildArgs, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<DocumentId>> StartBuildCompleted;
		#endregion
		#region StopBuild
		public void StopBuildAsync(DocumentId documentId, object asyncState) {
			AsyncCallback callback = ar => EndVoidOperation(ar, Channel.EndStopBuild, () => StopBuildCompleted);
			Channel.BeginStopBuild(documentId, callback, asyncState);
		}
		public event EventHandler<System.ComponentModel.AsyncCompletedEventArgs> StopBuildCompleted;
		#endregion
		#region GetBuildStatus
		public void GetBuildStatusAsync(DocumentId documentId, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetBuildStatus, () => GetBuildStatusCompleted);
			Channel.BeginGetBuildStatus(documentId, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<BuildStatus>> GetBuildStatusCompleted;
		#endregion
		#region GetPages
		public void GetPagesAsync(DocumentId documentId, int[] pageIndexes, PageCompatibility compatibility, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetPages, () => GetPagesCompleted);
			Channel.BeginGetPages(documentId, pageIndexes, compatibility, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<byte[]>> GetPagesCompleted;
		#endregion
		#region GetDocumentData
		public void GetDocumentDataAsync(DocumentId documentId, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetDocumentData, () => GetDocumentDataCompleted);
			Channel.BeginGetDocumentData(documentId, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<DocumentData>> GetDocumentDataCompleted;
		#endregion
		#region StartPrint
		public void StartPrintAsync(DocumentId documentId, PageCompatibility compatibility, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndStartPrint, () => StartPrintCompleted);
			Channel.BeginStartPrint(documentId, compatibility, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<PrintId>> StartPrintCompleted;
		#endregion
		#region StopPrint
		public void StopPrintAsync(PrintId printId, object asyncState) {
			AsyncCallback callback = ar => EndVoidOperation(ar, Channel.EndStopPrint, () => StopPrintCompleted);
			Channel.BeginStopPrint(printId, callback, asyncState);
		}
		public event EventHandler<AsyncCompletedEventArgs> StopPrintCompleted;
		#endregion
		#region GetPrintStatus
		public void GetPrintStatusAsync(PrintId printId, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetPrintStatus, () => GetPrintStatusCompleted);
			Channel.BeginGetPrintStatus(printId, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<PrintStatus>> GetPrintStatusCompleted;
		#endregion
		#region GetPrintDocument
		public void GetPrintDocumentAsync(PrintId printId, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetPrintDocument, () => GetPrintDocumentCompleted);
			Channel.BeginGetPrintDocument(printId, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<byte[]>> GetPrintDocumentCompleted;
		#endregion
		#region ClearDocument
		public void ClearDocumentAsync(DocumentId documentId, object asyncState) {
			AsyncCallback callback = ar => EndVoidOperation(ar, Channel.EndClearDocument, () => ClearDocumentCompleted);
			Channel.BeginClearDocument(documentId, callback, asyncState);
		}
		public event EventHandler<AsyncCompletedEventArgs> ClearDocumentCompleted;
		#endregion
		#region StartExport
		public void StartExportAsync(DocumentId documentId, DocumentExportArgs exportArgs, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndStartExport, () => StartExportCompleted);
			Channel.BeginStartExport(documentId, exportArgs, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<ExportId>> StartExportCompleted;
		#endregion
		#region GetExportStatus
		public void GetExportStatusAsync(ExportId exportId, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetExportStatus, () => GetExportStatusCompleted);
			Channel.BeginGetExportStatus(exportId, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<ExportStatus>> GetExportStatusCompleted;
		#endregion
		#region GetExportedDocument
		public void GetExportedDocumentAsync(ExportId exportId, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetExportedDocument, () => GetExportedDocumentCompleted);
			Channel.BeginGetExportedDocument(exportId, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<byte[]>> GetExportedDocumentCompleted;
		#endregion
	}
}
