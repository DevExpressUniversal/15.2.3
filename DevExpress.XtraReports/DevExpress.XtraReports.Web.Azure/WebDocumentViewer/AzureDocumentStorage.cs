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
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	public class AzureDocumentStorage : IDocumentStorageCachable {
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		readonly IAzureEntityStorageManager entityStorageManager;
		readonly IAzureFileStorageManager fileStorageManager;
		public AzureDocumentStorage(IAzureEntityStorageManager entityStorageManager, IAzureFileStorageManager fileStorageManager) {
			this.entityStorageManager = entityStorageManager;
			this.fileStorageManager = fileStorageManager;
		}
		#region IDocumentStorageCachable
		public string FindCachedDocumentId(string hash) {
			return null;
		}
		public string CreateNew() {
			var id = Guid.NewGuid().ToString("N");
			entityStorageManager.Save(new GeneratedDocumentInfo() { Id = id });
			return id;
		}
		public bool BlankExists(string id) {
			return entityStorageManager.IsBlankDocumentExists(id);
		}
		public void Update(string id, Action<IWebPreviewWritableDocument> action) {
			var writableDocuemntInfo = new AzurePreviewWritableDocument(id, entityStorageManager);
			action(writableDocuemntInfo);
		}
		public T DoWithBuildResult<T>(string id, int? loadPageIndex, Func<WebPreviewReadableDocument, T> func) {
			GeneratedDocumentInfo documentInfo = entityStorageManager.LoadDocument(id);
			if(documentInfo == null) {
				throw new ArgumentException(string.Format("Generated document '{0}' not found", id), "id");
			}
			WebPreviewReadableDocument readableDocument = new WebPreviewReadableDocument(documentInfo.Document, documentInfo.DrillDownKeys, documentInfo.FaultMessage);
			return func(readableDocument);
		}
		public void Release(string id) {
			entityStorageManager.TryRemove(id);
		}
		public void Clean(TimeSpan timeToLife) {
			entityStorageManager.Clean(timeToLife);
		}
		#endregion
	}
}
