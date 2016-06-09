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

using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Xpf.Printing;
namespace DevExpress.XtraReports.Service {
	[ServiceContract]
	[ServiceKnownType(ServiceKnownTypeProvider.GetKnownTypesMethodName, typeof(ServiceKnownTypeProvider))]
	public interface IReportService {
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		ReportParameterContainer GetReportParameters(InstanceIdentity identity);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		DocumentId StartBuild(InstanceIdentity identity, ReportBuildArgs buildArgs);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		void StopBuild(DocumentId documentId);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		BuildStatus GetBuildStatus(DocumentId documentId);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		string GetPage(DocumentId documentId, int pageIndex, PageCompatibility compatibility);
		[OperationContract]
		byte[] GetPages(DocumentId documentId, int[] pageIndexes, PageCompatibility compatibility);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		DocumentData GetDocumentData(DocumentId documentId);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		PrintId StartPrint(DocumentId documentId, PageCompatibility compatibility);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		void StopPrint(PrintId printId);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		PrintStatus GetPrintStatus(PrintId printId);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		Stream GetPrintDocument(PrintId printId);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		ParameterLookUpValues[] GetLookUpValues(InstanceIdentity identity, ReportParameter[] parameterValues, string[] requiredParameterPaths);
		#region ExportService
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		void ClearDocument(DocumentId documentId);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		ExportId StartExport(DocumentId documentId, DocumentExportArgs exportArgs);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		ExportStatus GetExportStatus(ExportId exportId);
		[OperationContract]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		Stream GetExportedDocument(ExportId exportId);
		#endregion
	}
}
