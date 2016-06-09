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
using DevExpress.Utils;
using Microsoft.WindowsAzure.Storage;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	public class AzureStorageAccountProvider : IAzureStorageAccountProvider {
		readonly CloudStorageAccount storageAccount;
		public AzureStorageAccountProvider(AzureConnectionStringProvider connectionStringProvider) {
			Guard.ArgumentNotNull(connectionStringProvider, "connectionStringProvider");
			Guard.ArgumentIsNotNullOrEmpty(connectionStringProvider.CloudStorageConnectionString, "CloudStorageConnectionString");
			CloudStorageAccount storageAccount;
			if(!CloudStorageAccount.TryParse(connectionStringProvider.CloudStorageConnectionString, out storageAccount))
				throw new ArgumentException("Could not parse connection string of the cloud storage account");
			this.storageAccount = storageAccount;
		}
		public CloudStorageAccount GetStorageAccount() {
			return storageAccount;
		}
	}
}
