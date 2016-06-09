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
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.ReportServer.ServiceModel.Native.RemoteOperations {
	public class RemoteOperationFactory {
		readonly IReportServiceClient client;
		readonly TimeSpan updateInterval;
		readonly DocumentId documentId;
		public RemoteOperationFactory(IReportServiceClient client, DocumentId documentId, TimeSpan updateInterval) {
			Guard.ArgumentNotNull(client, "client");
			Guard.ArgumentNotNull(documentId, "documentId");
			Guard.ArgumentNotNull(updateInterval, "updateInterval");
			this.client = client;
			this.updateInterval = updateInterval;
			this.documentId = documentId;
		}
		public RequestPagesOperation CreateRequestPagesOperation(int[] pageIndexes, IBrickPagePairFactory factory) {
			return new RequestPagesOperation(client, documentId, updateInterval, pageIndexes, factory);
		}
		public ExportDocumentOperation CreateExportDocumentOperation(ExportFormat format, ExportOptions options) {
			return CreateExportDocumentOperation(format, options, null);
		}
		public ExportDocumentOperation CreateExportDocumentOperation(ExportFormat format, ExportOptions options, object customArgs) {
			return new ExportDocumentOperation(client, documentId, format, options, updateInterval, customArgs);
		}
	}
}
