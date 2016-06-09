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
using System.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
namespace DevExpress.DocumentServices.ServiceModel.Client {
	[ServiceContract(Name = "IReportService")]
#if !WINRT
	[ServiceKnownType(Xpf.Printing.ServiceKnownTypeProvider.GetKnownTypesMethodName, typeof(Xpf.Printing.ServiceKnownTypeProvider))]
#endif
	public interface IAsyncReportService {
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetReportParameters(InstanceIdentity identity, AsyncCallback callback, object asyncState);
		ReportParameterContainer EndGetReportParameters(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginStartBuild(InstanceIdentity identity, ReportBuildArgs buildArgs, AsyncCallback callback, object asyncState);
		DocumentId EndStartBuild(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginStopBuild(DocumentId documentId,  AsyncCallback callback, object asyncState);
		void EndStopBuild(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetBuildStatus(DocumentId documentId, AsyncCallback callback, object asyncState);
		BuildStatus EndGetBuildStatus(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetPages(DocumentId documentId, int[] pageIndexes, PageCompatibility compatibility, AsyncCallback callback, object asyncState);
		byte[] EndGetPages(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetDocumentData(DocumentId documentId, AsyncCallback callback, object asyncState);
		DocumentData EndGetDocumentData(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginStartPrint(DocumentId documentId, PageCompatibility compatibility, AsyncCallback callback, object asyncState);
		PrintId EndStartPrint(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginStopPrint(PrintId printId, AsyncCallback callback, object asyncState);
		void EndStopPrint(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetPrintStatus(PrintId printId, AsyncCallback callback, object asyncState);
		PrintStatus EndGetPrintStatus(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetPrintDocument(PrintId printId, AsyncCallback callback, object asyncState);
		byte[] EndGetPrintDocument(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetLookUpValues(InstanceIdentity identity, ReportParameter[] parameterValues, string[] requiredParameterPaths, AsyncCallback callback, object asyncState);
		ParameterLookUpValues[] EndGetLookUpValues(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginStartUpload(AsyncCallback callback, object asyncState);
		UploadingResourceId EndStartUpload(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUploadResourceChunk(UploadingResourceId id, byte[] data, AsyncCallback callback, object asyncState);
		void EndUploadResourceChunk(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginAssembleDocument(UploadingResourceId id, AsyncCallback callback, object asyncState);
		DocumentId EndAssembleDocument(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginClearDocument(DocumentId documentId, AsyncCallback callback, object asyncState);
		void EndClearDocument(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginStartExport(DocumentId documentId, DocumentExportArgs exportArgs, AsyncCallback callback, object asyncState);
		ExportId EndStartExport(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetExportStatus(ExportId exportId, AsyncCallback callback, object asyncState);
		ExportStatus EndGetExportStatus(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetExportedDocument(ExportId exportId, AsyncCallback callback, object asyncState);
		byte[] EndGetExportedDocument(IAsyncResult ar);
	}
}
