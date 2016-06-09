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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public class InMemoryDocumentStorage : IDocumentStorageCachable {
		#region inner types
		class InMemoryPreviewDocument : IWebPreviewWritableDocument {
			public DateTime LastAccessTimeUtc { get; private set; }
			public object SyncRoot { get; private set; }
			public Document Document { get; private set; }
			public Dictionary<string, bool> DrillDownKeys { get; private set; }
			public Exception BuildException { get; private set; }
			public string Cache { get; private set; }
			public InMemoryPreviewDocument() {
				LastAccessTimeUtc = DateTime.UtcNow;
				SyncRoot = new object();
			}
			public void AssignSuccessDocument(Document document, Dictionary<string, bool> drillDownKeys, string cache) {
				Document = document;
				DrillDownKeys = drillDownKeys;
				Cache = cache;
			}
			public void AssignBuildException(Exception buildException) {
				BuildException = buildException;
			}
			public WebPreviewReadableDocument CreateReadable() {
				LastAccessTimeUtc = DateTime.UtcNow;
				string faultMessage = DocumentManagementServiceLogic.GetFaultMessage(BuildException);
				return new WebPreviewReadableDocument(Document, DrillDownKeys, faultMessage);
			}
		}
		#endregion
		readonly ConcurrentDictionary<string, InMemoryPreviewDocument> documents = new ConcurrentDictionary<string, InMemoryPreviewDocument>();
		#region IDocumentStorage
		public bool BlankExists(string id) {
			InMemoryPreviewDocument previewDocument;
			if(documents.TryGetValue(id, out previewDocument) && previewDocument.Document == null && previewDocument.BuildException == null)
				return true;
			return false;
		}
		public string CreateNew() {
			var id = Guid.NewGuid().ToString("N");
			var previewDocument = new InMemoryPreviewDocument();
			documents.AddOrUpdate(id, previewDocument, (_, oldValue) => previewDocument);
			return id;
		}
		public void Update(string id, Action<IWebPreviewWritableDocument> action) {
			InMemoryPreviewDocument previewDocument = GetPreviewDocument(id);
			action(previewDocument);
		}
		public T DoWithBuildResult<T>(string id, int? loadPageIndex, Func<WebPreviewReadableDocument, T> func) {
			InMemoryPreviewDocument previewDocument = GetPreviewDocument(id);
			lock(previewDocument.SyncRoot) {
				var readableDocument = previewDocument.CreateReadable();
				return func(readableDocument);
			}
		}
		public void Release(string id) {
			InMemoryPreviewDocument ignore;
			if(!documents.TryRemove(id, out ignore)) {
				ThrowDocumentNotFound(id);
			}
		}
		public void Clean(TimeSpan timeToLife) {
			var now = DateTime.UtcNow;
			List<string> garbageDocumentIds = documents
				.Where(x => now - x.Value.LastAccessTimeUtc > timeToLife)
				.Select(x => x.Key)
				.ToList();
			InMemoryPreviewDocument ignore;
			foreach(string documentId in garbageDocumentIds) {
				documents.TryRemove(documentId, out ignore);
			}
		}
		public string FindCachedDocumentId(string hash) {
			Guard.ArgumentNotNull(hash, "hash");
			var id = documents
				.Where(x => x.Value.Cache == hash)
				.Select(x => x.Key)
				.FirstOrDefault();
			return id;
		}
		#endregion
		InMemoryPreviewDocument GetPreviewDocument(string id) {
			InMemoryPreviewDocument previewDocument;
			if(!documents.TryGetValue(id, out previewDocument)) {
				ThrowDocumentNotFound(id);
			}
			return previewDocument;
		}
		static void ThrowDocumentNotFound(string id) {
			throw new ArgumentException(string.Format("Document '{0}' not found", id), "id");
		}
	}
}
