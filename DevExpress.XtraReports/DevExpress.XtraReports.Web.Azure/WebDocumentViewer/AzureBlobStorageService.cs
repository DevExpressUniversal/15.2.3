﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.Utils;
using DevExpress.XtraReports.Web.Azure.WebDocumentViewer.Native;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	class AzureBlobStorageService : IAzureBlobStorageService {
		CloudStorageAccount storageAccount;
		CloudBlobClient blobClient;
		CloudBlobContainer commonContainer;
		readonly IAzureStorageAccountProvider storageAccountProvider;
		CloudBlobClient BlobClient {
			get {
				if(blobClient == null) {
					blobClient = StorageAccount.CreateCloudBlobClient();
				}
				return blobClient;
			}
		}
		CloudStorageAccount StorageAccount {
			get {
				if(storageAccount == null) {
					storageAccount = storageAccountProvider.GetStorageAccount();
				}
				return storageAccount;
			}
		}
		CloudBlobContainer CommonContainer {
			get {
				if(commonContainer == null)
					commonContainer = CreateBlobContainer(ServiceConstatns.CommonStorageItemName);
				return commonContainer;
			}
		}
		public AzureBlobStorageService(IAzureStorageAccountProvider storageAccountProvider) {
			Guard.ArgumentNotNull(storageAccountProvider, "storageAccountProvider");
			this.storageAccountProvider = storageAccountProvider;
		}
		public CloudBlobContainer GetReportBlobContainer() {
			return CommonContainer;
		}
		public CloudBlobContainer GetDocumentBlobContainer() {
			return CommonContainer;
		}
		public CloudBlobContainer GetCommonBlobContainer() {
			return CommonContainer;
		}
		public CloudBlobClient GetCloudBlobClient() {
			return BlobClient;
		}
		CloudBlobContainer CreateBlobContainer(string name) {
			var container = BlobClient.GetContainerReference(name);
			container.CreateIfNotExists(BlobContainerPublicAccessType.Container);
			return container;
		}
	}
}
