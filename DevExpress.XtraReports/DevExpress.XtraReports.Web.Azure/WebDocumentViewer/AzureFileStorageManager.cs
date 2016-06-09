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
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	class AzureFileStorageManager : IAzureFileStorageManager {
		CloudBlobContainer reportBlobContainer;
		CloudBlobContainer documentBlobContainer;
		CloudBlobContainer commonBlobContainer;
		readonly IAzureBlobStorageService blobStorageService;
		protected virtual CloudBlobContainer ReportBlobContainer {
			get {
				if(reportBlobContainer == null) {
					reportBlobContainer = blobStorageService.GetReportBlobContainer();
				}
				return reportBlobContainer;
			}
		}
		protected virtual CloudBlobContainer CommonBlobContainer {
			get {
				if(commonBlobContainer == null) {
					commonBlobContainer = blobStorageService.GetCommonBlobContainer();
				}
				return commonBlobContainer;
			}
		}
		protected virtual CloudBlobContainer DocumentBlobContainer {
			get {
				if(documentBlobContainer == null) {
					documentBlobContainer = blobStorageService.GetDocumentBlobContainer();
				}
				return documentBlobContainer;
			}
		}
		public AzureFileStorageManager(IAzureBlobStorageService blobStorageService) {
			this.blobStorageService = blobStorageService;
		}
		#region IAzureFileStorageManager
		public Stream GetDocument(string path) {
			return GetFileStream(path, DocumentBlobContainer);
		}
		public string SaveDocument(string fileName, Stream body) {
			return SaveFileFromStream(fileName, body, DocumentBlobContainer);
		}
		public void DeleteDocument(string fileName) {
			DeleteFile(fileName, DocumentBlobContainer);
		}
		public Stream GetReport(string path) {
			return GetFileStream(path, ReportBlobContainer);
		}
		public string SaveReport(string fileName, Stream body) {
			return SaveFileFromStream(fileName, body, ReportBlobContainer);
		}
		public void DeleteReport(string fileName) {
			DeleteFile(fileName, ReportBlobContainer);
		}
		public Stream GetFile(string path) {
			return GetFileStream(path, CommonBlobContainer);
		}
		public string SaveFile(string fileName, Stream body) {
			return SaveFileFromStream(fileName, body, CommonBlobContainer);
		}
		public void DeleteFile(string fileName) {
			DeleteFile(fileName, CommonBlobContainer);
		}
		#endregion
		Stream GetFileStream(string path, CloudBlobContainer container) {
			var documentBlob = container.GetBlockBlobReference(path);
			var stream = new MemoryStream();
			documentBlob.DownloadToStream(stream);
			return stream;
		}
		string SaveFileFromStream(string blobName, Stream stream, CloudBlobContainer container) {
			if(string.IsNullOrEmpty(blobName))
				blobName = Guid.NewGuid().ToString("N");
			CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
			stream.Position = 0;
			blockBlob.UploadFromStream(stream);
			return blobName;
		}
		void DeleteFile(string fileName, CloudBlobContainer container) {
			var blob = container.GetBlobReference(fileName);
			blob.Delete();
		}
	}
}
