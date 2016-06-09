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

using DevExpress.XtraReports.Web.Azure.WebDocumentViewer.Native;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	public class AzureServiceTableProvider : IAzureServiceTableProvider {
		readonly IAzureStorageAccountProvider storageAccountProvider;
		CloudTableClient tableClient;
		CloudStorageAccount storageAccount;
		CloudTableClient TableClient {
			get {
				if(tableClient == null) {
					tableClient = StorageAccount.CreateCloudTableClient();
				}
				return tableClient;
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
		public AzureServiceTableProvider(IAzureStorageAccountProvider storageAccountProvider) {
			this.storageAccountProvider = storageAccountProvider;
		}
		public CloudTable GetDocumentTable() {
			CloudTable table = TableClient.GetTableReference(ServiceConstatns.DocumentStorageItemName);
			if(!table.Exists()) {
				CreateCloudTable(ServiceConstatns.DocumentStorageItemName);
			}
			return table;
		}
		public CloudTable GetReportTable() {
			CloudTable table = TableClient.GetTableReference(ServiceConstatns.ReportStorageItemName);
			if(!table.Exists()) {
				CreateCloudTable(ServiceConstatns.ReportStorageItemName);
			}
			return table;
		}
		public CloudTableClient GetCloudTableClient() {
			return TableClient;
		}
		CloudTable CreateCloudTable(string name) {
			var table = TableClient.GetTableReference(name);
			table.CreateIfNotExists();
			return table;
		}
	}
}
