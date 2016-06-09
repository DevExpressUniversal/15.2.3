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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class AzureFileSystemProvider : CloudFileSystemProviderBase {
		public AzureFileSystemProvider(string rootFolder)
			: base(rootFolder) {
		}
		const string EmptyFolderBlobName = "aspxAzureEmptyFolderBlob";
		List<FileManagerFolder> folders = new List<FileManagerFolder>();
		List<FileManagerFile> files = new List<FileManagerFile>();
		List<Blob> blobs = new List<Blob>();
		bool entriesLoaded = false;
		AzureBlobStorageHelper helper;
		AzureUploadStorageProvider uploadProvider;
		public string StorageAccountName { get; set; }
		public string AccessKey { get; set; }
		public string ContainerName { get; set; }
		string APIUrl { get { return "https://" + StorageAccountName + ".blob.core.windows.net/"; } }
		string PreparedRootFolder { get { return PreparePath(RootFolder); } }
		AzureBlobStorageHelper Helper {
			get {
				if(helper == null) {
					helper = new AzureBlobStorageHelper(StorageAccountName, AccessKey, ContainerName);
					helper.CloudRequestEvent += helper_CloudRequestEvent;
					helper.CloudRequestExceptionEvent += helper_CloudRequestExceptionEvent;
				}
				return helper;
			}
		}
		AzureUploadStorageProvider UploadProvider {
			get {
				if(this.uploadProvider == null)
					this.uploadProvider = new AzureUploadStorageProvider(Helper);
				return this.uploadProvider;
			}
		}
		public override bool Exists(FileManagerFolder folder) {
			if(PreparePath(folder.FullName) == PreparedRootFolder)
				return true;
			EnsureEntriesLoaded();
			return folders.Contains(folder);
		}
		public override bool Exists(FileManagerFile file) {
			EnsureEntriesLoaded();
			return files.Contains(file);
		}
		public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder) {
			EnsureEntriesLoaded();
			var newFolders = folders.FindAll(f => PreparePath(f.FullName) == PreparePath(parentFolder.FullName + PathSeparator + f.Name));
			return newFolders;
		}
		public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder) {
			EnsureEntriesLoaded();
			var newFiles = files.FindAll(f => f.RelativeName.StartsWith(folder.RelativeName) && f.RelativeName.Substring(folder.RelativeName.Length + 1).IndexOf("\\") == -1);
			var emptyFolderBlobIndex = newFiles.FindIndex(f => f.Name == EmptyFolderBlobName);
			if(emptyFolderBlobIndex != -1) {
				if(newFiles.Count > 1) {
					var emptyBlobPath = PreparePath(folder.FullName) + PathSeparator + EmptyFolderBlobName;
					DeleteItem(emptyBlobPath);
					files.RemoveAt(files.FindIndex(f => PreparePath(f.FullName) == emptyBlobPath));
				}
				newFiles.RemoveAt(emptyFolderBlobIndex);
			}
			return newFiles;
		}
		public override long GetLength(FileManagerFile file) {
			return long.Parse(GetBlob(file.FullName).Size);
		}
		public override DateTime GetLastWriteTime(FileManagerFile file) {
			return Convert.ToDateTime(GetBlob(file.FullName).LastModified);
		}
		public override void DeleteFile(FileManagerFile file) {
			var folder = file.Folder;
			var files = GetFiles(file.Folder);
			DeleteItem(PreparePath(file.FullName));
			if(files.Count() == 1 && !string.IsNullOrEmpty(PreparePath(folder.FullName)) && files.ElementAt(0).FullName == file.FullName)
				CreateEmptyFolderBlob(PreparePath(folder.FullName) + PathSeparator + EmptyFolderBlobName);
			LoadEntries();
		}
		public override void DeleteFolder(FileManagerFolder folder) {
			foreach(var blob in blobs.Where(b => PrepareUrl(b.Url).StartsWith(PreparePath(folder.FullName))))
				DeleteItem(PrepareUrl(blob.Url));
			LoadEntries();
		}
		public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			CopyFile(file, newParentFolder, false);
			DeleteFile(file);
		}
		public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			CopyFolder(folder, newParentFolder, false);
			DeleteFolder(folder);
		}
		public override void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			CopyFile(file, newParentFolder, true);
		}
		public override void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			CopyFolder(folder, newParentFolder, true);
		}
		public override void RenameFile(FileManagerFile file, string name) {
			var path = PreparePath(file.FullName);
			CopyFile(file, file.Folder, false, name);
			DeleteFile(file);
		}
		public override void RenameFolder(FileManagerFolder folder, string name) {
			var folderPath = PreparePath(folder.RelativeName);
			var newFolderPath = folderPath.IndexOf(PathSeparator) > -1 ? folderPath.Substring(0, folderPath.LastIndexOf(PathSeparator)) : string.Empty;
			CopyFolder(folder, new FileManagerFolder(this, newFolderPath), false, name);
			DeleteFolder(folder);
		}
		public override void CreateFolder(FileManagerFolder parent, string name) {
			var parentPath = PreparePath(parent.FullName);
			CreateEmptyFolderBlob((string.IsNullOrEmpty(parentPath) ? string.Empty : parentPath + PathSeparator) + name + PathSeparator + EmptyFolderBlobName);
			LoadEntries();
		}
		public override void UploadFile(FileManagerFolder folder, string fileName, Stream content) {
			string blobName = (string.IsNullOrEmpty(PreparePath(folder.FullName)) ? "" : PreparePath(folder.FullName) + PathSeparator) + fileName;
			string contentType = GetContentType(Path.GetExtension(fileName));
			UploadProvider.UploadObject(blobName, content, contentType);
		}
		public override string GetDownloadUrl(FileManagerFile[] files) {
			var file = files[0];
			if(!Exists(file))
				return string.Empty;
			return Helper.GetDownloadUrl(PreparePath(file.FullName));
		}
		protected override WebResponse GetFileResponse(FileManagerFile file) {
			HttpWebRequest request = Helper.GetRequest(PreparePath(file.FullName), "GET");
			RaiseRequestEvent(new FileManagerCloudProviderRequestEventArgs(request));
			var resp = request.GetResponse();
			try {
				return resp;
			} catch {
				resp.Close();
				return null;
			}
		}
		void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder, bool loadEntries, string newFileName = "") {
			var destinationPath = PreparePath(newParentFolder.FullName) + PathSeparator + (string.IsNullOrEmpty(newFileName) ? file.Name : newFileName);
			CopyBlob(APIUrl + ContainerName + PathSeparator + PreparePath(file.FullName), destinationPath);
			if(loadEntries)
				LoadEntries();
		}
		void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder, bool loadEntries, string newFolderName = "") {
			var folderPath = PreparePath(folder.FullName);
			var newFolderPath = PreparePath(newParentFolder.FullName);
			var lastSeparatorIndex = folderPath.LastIndexOf(PathSeparator);
			foreach(var blob in blobs.Where(b => PrepareUrl(b.Url).StartsWith(folderPath))) {
				var blobPath = lastSeparatorIndex > -1 ? PrepareUrl(blob.Url).Substring(lastSeparatorIndex + 1) : PrepareUrl(blob.Url);
				if(!string.IsNullOrEmpty(newFolderName))
					blobPath = newFolderName + blobPath.Substring(folder.Name.Length);
				CopyBlob(blob.Url, newFolderPath + PathSeparator + blobPath);
			}
			if(loadEntries)
				LoadEntries();
		}
		void CopyBlob(string path, string destinationPath) {
			if(destinationPath.StartsWith(PathSeparator))
				destinationPath = destinationPath.Substring(1);
			Helper.CopyItem(destinationPath, path);
		}
		void DeleteItem(string path) {
			Helper.DeleteItem(path);
		}
		void CreateEmptyFolderBlob(string path) {
			Helper.PutBlob(path, null);
		}
		void LoadEntries() {
			folders.Clear();
			files.Clear();
			blobs.Clear();
			blobs = GetBlobsList();
			foreach(var blob in blobs) {
				var path = PrepareUrl(blob.Url);
				var file = new FileManagerFile(this, path.Substring(PreparedRootFolder.Length));
				files.Add(file);
				while(path.IndexOf(PathSeparator) > -1 && path != PreparedRootFolder) {
					path = path.Substring(0, path.LastIndexOf(PathSeparator));
					if(folders.Exists(f => PreparePath(f.FullName) == path))
						continue;
					folders.Add(new FileManagerFolder(this, path.Substring(PreparedRootFolder.Length)));
				}
			}
			entriesLoaded = true;
		}
		void EnsureEntriesLoaded() {
			if(!entriesLoaded)
				LoadEntries();
		}
		void helper_CloudRequestEvent(object sender, CloudRequestEventArgs e) {
			RaiseRequestEvent(new FileManagerCloudProviderRequestEventArgs(e.Request));
		}
		void helper_CloudRequestExceptionEvent(object sender, CloudRequestExceptionEventArgs e) {
			e.Exception = new FileManagerCloudAccessFailedException(e.ResponseCode, e.RequestResult);
		}
		Blob GetBlob(string fileFullName) {
			return blobs.Find(b => PrepareUrl(b.Url) == PreparePath(fileFullName));
		}
		protected override string PreparePath(string path) {
			return base.PreparePath(path).TrimStart('/');
		}
		string PrepareUrl(string url) {
			return url.Replace(APIUrl + ContainerName + PathSeparator, string.Empty);
		}
		List<Blob> GetBlobsList() {
			var response = Helper.GetBlobsResponse(PreparedRootFolder);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(response);
			XmlNodeList nodes = doc.GetElementsByTagName("Blob");
			var blobsList = new List<Blob>();
			foreach(XmlNode node in nodes) {
				using(MemoryStream stream = new MemoryStream()) {
					StreamWriter stw = new StreamWriter(stream);
					stw.Write(node.OuterXml);
					stw.Flush();
					stream.Position = 0;
					XmlSerializer serializer = new XmlSerializer(typeof(Blob));
					using(StreamReader reader = new StreamReader(stream))
						blobsList.Add((Blob)serializer.Deserialize(reader));
				}
			}
			return blobsList;
		}
		public class Blob {
			public string Name { get; set; }
			public string Url { get; set; }
			public string LastModified { get; set; }
			public string Etag { get; set; }
			public string Size { get; set; }
		}
	}
}
