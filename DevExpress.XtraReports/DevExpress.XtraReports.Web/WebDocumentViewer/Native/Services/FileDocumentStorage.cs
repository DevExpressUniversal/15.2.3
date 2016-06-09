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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.InternalAccess;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public class FileDocumentStorage : IDocumentStorageCachable {
		#region inner types
		[DataContract]
		class DocumentBuildFault {
			[DataMember]
			public string Message { get; set; }
		}
		class FileWebPreviewWritableDocument : IWebPreviewWritableDocument {
			const string
				VirtualDocumentFileName = "virtualDocument.prnx",
				DrillDownKeysFileName = "drillDownKeys.json",
				BuildExceptionFileName = "buildException.json",
				CacheFileName = ".cache";
			public static string ReadCache(string directoryPath) {
				var cacheFilePath = Path.Combine(directoryPath, CacheFileName);
				if(!File.Exists(cacheFilePath)) {
					return null;
				}
				return File.ReadAllText(cacheFilePath);
			}
			readonly string directoryPath;
			public FileWebPreviewWritableDocument(string directoryPath) {
				this.directoryPath = directoryPath;
			}
			public void AssignSuccessDocument(Document document, Dictionary<string, bool> drillDownKeys, string cache) {
				Guard.ArgumentNotNull(document, "document");
				var documentFilePath = Path.Combine(directoryPath, VirtualDocumentFileName);
				using(var documentStream = File.Create(documentFilePath)) {
					PrintingSystemAccessor.SaveIndependentPages(document.PrintingSystem, documentStream);
				}
				if(drillDownKeys != null) {
					SaveToFile(drillDownKeys, DrillDownKeysFileName);
				}
				if(cache != null) {
					SaveStringToFile(cache, CacheFileName);
				}
			}
			public void AssignBuildException(Exception exception) {
				var contract = new DocumentBuildFault {
					Message = exception.Message
				};
				SaveToFile(contract, BuildExceptionFileName);
			}
			public T DoWithBuildResult<T>(int? loadPageIndex, Func<WebPreviewReadableDocument, T> func) {
				using(var ps = XtraReport.CreatePrintingSystem()) {
					var documentFilePath = Path.Combine(directoryPath, VirtualDocumentFileName);
					Document document = null;
					if(File.Exists(documentFilePath)) {
						if(loadPageIndex.HasValue) {
							PrintingSystemAccessor.LoadVirtualDocument(ps, documentFilePath, loadPageIndex.Value);
						} else {
							ps.LoadDocument(documentFilePath);
						}
						document = ps.Document;
					}
					var ddks = LoadFromFile<Dictionary<string, bool>>(DrillDownKeysFileName);
					var buildFaultContract = LoadFromFile<DocumentBuildFault>(BuildExceptionFileName);
					string faultMessage = buildFaultContract == null ? null : buildFaultContract.Message;
					var webPreviewReadableDocument = new WebPreviewReadableDocument(document, ddks, faultMessage);
					return func(webPreviewReadableDocument);
				}
			}
			void SaveToFile<T>(T obj, string fileName) {
				var filePath = GetFilePath(fileName);
				using(var fileStream = File.Create(filePath)) {
					var serializer = new DataContractJsonSerializer(typeof(T));
					serializer.WriteObject(fileStream, obj);
				}
			}
			void SaveStringToFile(string value, string fileName) {
				var filePath = GetFilePath(fileName);
				using(var fileStream = new StreamWriter(filePath)) {
					fileStream.AutoFlush = true;
					fileStream.Write(value);
				}
			}
			T LoadFromFile<T>(string fileName) {
				var filePath = GetFilePath(fileName);
				if(!File.Exists(filePath)) {
					return default(T);
				}
				using(var fileStream = File.OpenRead(filePath)) {
					var serializer = new DataContractJsonSerializer(typeof(T));
					return (T)serializer.ReadObject(fileStream);
				}
			}
			string GetFilePath(string fileName) {
				return Path.Combine(directoryPath, fileName);
			}
		}
		#endregion
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		readonly string workingDirectory;
		public FileDocumentStorage(FileDocumentStorageSettings settings) {
			workingDirectory = settings.WorkingDirectory;
		}
		#region IDocumentStorage
		public string CreateNew() {
			var result = Guid.NewGuid().ToString("N");
			var dir = Path.Combine(workingDirectory, result);
			Directory.CreateDirectory(dir);
			return result;
		}
		public bool BlankExists(string id) {
			throw new NotSupportedException();
		}
		public void Update(string id, Action<IWebPreviewWritableDocument> action) {
			var directoryPath = GetDocumentDirectoryPath(id);
			var previewDocument = new FileWebPreviewWritableDocument(directoryPath);
			action(previewDocument);
		}
		public T DoWithBuildResult<T>(string id, int? loadPageIndex, Func<WebPreviewReadableDocument, T> func) {
			var directoryPath = GetDocumentDirectoryPath(id);
			var previewDocument = new FileWebPreviewWritableDocument(directoryPath);
			return previewDocument.DoWithBuildResult(loadPageIndex, func);
		}
		public void Release(string id) {
			var directoryPath = GetDocumentDirectoryPath(id);
			Directory.Delete(directoryPath, true);
		}
		public void Clean(TimeSpan timeToLife) {
			var now = DateTime.UtcNow;
			var garbageDirectories = Directory.EnumerateDirectories(workingDirectory)
				.Where(x => now - Directory.GetLastAccessTimeUtc(x) > timeToLife)
				.ToList();
			foreach(var directoryPath in garbageDirectories) {
				Directory.Delete(directoryPath, true);
			}
		}
		public string FindCachedDocumentId(string hash) {
			Guard.ArgumentNotNull(hash, "hash");
			var directoryPath = Directory.EnumerateDirectories(workingDirectory)
			   .FirstOrDefault(x => FileWebPreviewWritableDocument.ReadCache(x) == hash);
			if(directoryPath == null) {
				return null;
			}
			return Path.GetFileName(directoryPath);
		}
		#endregion
		string GetDocumentDirectoryPath(string id) {
			var result = Path.Combine(workingDirectory, id);
			if(!Directory.Exists(result)) {
				throw new ArgumentException(string.Format("Document '{0} not found", id), "id");
			}
			return result;
		}
	}
}
