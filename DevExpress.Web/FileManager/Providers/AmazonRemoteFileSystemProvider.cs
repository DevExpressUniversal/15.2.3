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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class AmazonFileSystemProvider : CloudFileSystemProviderBase {
		public AmazonFileSystemProvider(string rootFolder)
			: base(rootFolder) {
		}
		const string XAmzDate = "X-Amz-Date";
		const string XAmzAlgorithm = "X-Amz-Algorithm";
		const string XAmzCredential = "X-Amz-Credential";
		const string XAmzSignedHeaders = "X-Amz-SignedHeaders";
		const string Terminator = "aws4_request";
		const string Scheme = "AWS4";
		const string Algorithm = "HMAC-SHA256";
		const string HMACSHA256 = "HMACSHA256";
		const string CreatedFoldersCacheKey = "aspxAmazonProviderFoldersCache";
		Regex CompressWhitespaceRegex = new Regex("\\s+");
		bool contentsLoaded = false;
		List<string> createdFoldersCache;
		List<FileManagerFolder> folders = new List<FileManagerFolder>();
		List<FileManagerFile> files = new List<FileManagerFile>();
		List<Contents> contents = new List<Contents>();
		HashAlgorithm CanonicalRequestHashAlgorithm = HashAlgorithm.Create("SHA-256");
		AmazonS3Helper helper;
		AmazonUploadStorageProvider uploadProvider;
		public string AccessKeyID { get; set; }
		public string SecretAccessKey { get; set; }
		public string BucketName { get; set; }
		public string Region { get; set; }
		AmazonS3Helper Helper {
			get {
				if(helper == null) {
					helper = new AmazonS3Helper(AccessKeyID, SecretAccessKey, BucketName, Region);
					helper.CloudRequestEvent += helper_CloudRequestEvent;
					helper.CloudRequestExceptionEvent += helper_CloudRequestExceptionEvent;
				}
				return helper;
			}
		}
		AmazonUploadStorageProvider UploadProvider {
			get {
				if(this.uploadProvider == null)
					this.uploadProvider = new AmazonUploadStorageProvider(Helper);
				return this.uploadProvider;
			}
		}
		string InternalRootFolder { get { return PreparePath(RootFolder); } }
		string ApiUrl { get { return "https://" + BucketName + ".s3.amazonaws.com/"; } }
		List<string> CreatedFoldersCache {
			get {
				if(HttpContext.Current == null)
					return createdFoldersCache == null ? new List<string>() : createdFoldersCache; 
				else
					return HttpContext.Current.Session[CreatedFoldersCacheKey] == null ? new List<string>() : (List<string>)HttpContext.Current.Session[CreatedFoldersCacheKey]; 
			}
			set {
				if(HttpContext.Current == null)
					createdFoldersCache = value;
				else
					HttpContext.Current.Session[CreatedFoldersCacheKey] = value;
			}
		}
		public override bool Exists(FileManagerFolder folder) {
			if(PreparePath(folder.FullName) == string.Empty)
				return true;
			EnsureContentsLoaded();
			return folders.Contains(folder);
		}
		public override bool Exists(FileManagerFile file) {
			EnsureContentsLoaded();
			return files.Contains(file);
		}
		public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder) {
			EnsureContentsLoaded();
			var newFolders = folders.FindAll(f => PreparePath(f.FullName) == PreparePath(parentFolder.FullName + PathSeparator + f.Name));
			return newFolders;
		}
		public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder) {
			EnsureContentsLoaded();
			var newFiles = files.FindAll(f => PreparePath(f.FullName) == PreparePath(folder.FullName + PathSeparator + f.Name));
			return newFiles;
		}
		public override long GetLength(FileManagerFile file) {
			EnsureContentsLoaded();
			return long.Parse(contents.Find(c => PreparePath(c.Key) == PreparePath(file.FullName)).Size);
		}
		public override DateTime GetLastWriteTime(FileManagerFile file) {
			EnsureContentsLoaded();
			return Convert.ToDateTime(contents.Find(c => PreparePath(c.Key) == PreparePath(file.FullName)).LastModified);
		}
		public override void DeleteFile(FileManagerFile file) {
			EnsureContentsLoaded();
			CreateParentFolders(file.Folder);
			Helper.DeleteItem(PreparePath(file.FullName));
			LoadContents();
		}
		public override void DeleteFolder(FileManagerFolder folder) {
			EnsureContentsLoaded();
			CreateParentFolders(folder.Parent);
			var itemPaths = files.Where(f => PreparePath(f.FullName).StartsWith(PreparePath(folder.FullName))).Select(f => PreparePath(f.FullName)).ToList();
			itemPaths.Add(PreparePath(folder.FullName) + PathSeparator);
			Helper.DeleteItems(itemPaths);
			LoadContents();
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
			CopyFile(file, file.Folder, false, name);
			DeleteFile(file);
		}
		public override void RenameFolder(FileManagerFolder folder, string name) {
			CopyFolder(folder, folder.Parent, false, name);
			DeleteFolder(folder);
		}
		public override void CreateFolder(FileManagerFolder parent, string name) {
			var parentPath = PreparePath(parent.FullName);
			Helper.CreateFolder((string.IsNullOrEmpty(parentPath) ? "" : parentPath + PathSeparator) + name + PathSeparator);
			LoadContents();
		}
		public override void UploadFile(FileManagerFolder folder, string fileName, Stream content) {
			using(content) {
				string objectName = (string.IsNullOrEmpty(PreparePath(folder.FullName)) ? "" : PreparePath(folder.FullName) + PathSeparator) + fileName;
				string contentType = GetContentType(Path.GetExtension(fileName));
				UploadProvider.UploadObject(objectName, content, contentType);
			}
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
			WebResponse resp = request.GetResponse();
			try {
				return resp;
			} catch {
				resp.Close();
				return null;
			}
		}
		void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder, bool loadContents, string newFileName = "") {
			var destinationPath = PreparePath(newParentFolder.FullName) + PathSeparator + (string.IsNullOrEmpty(newFileName) ? file.Name : newFileName);
			CopyObject(BucketName + PathSeparator + PreparePath(file.FullName), destinationPath);
			if(loadContents)
				LoadContents();
		}
		void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder, bool loadContents, string newFolderName = "") {
			var folderPath = PreparePath(folder.FullName);
			var folderItems = files.Where(f => PreparePath(f.FullName).StartsWith(folderPath));
			if(folderItems.Count() == 0)
				CreateFolder(newParentFolder, string.IsNullOrEmpty(newFolderName) ? folder.Name : newFolderName);
			else {
				var newFolderPath = PreparePath(newParentFolder.FullName);
				var lastSeparatorIndex = folderPath.LastIndexOf(PathSeparator);
				foreach(var file in folderItems) {
					var filePath = lastSeparatorIndex > -1 ? PreparePath(file.FullName).Substring(lastSeparatorIndex + 1) : PreparePath(file.FullName);
					if(!string.IsNullOrEmpty(newFolderName))
						filePath = newFolderName + filePath.Substring(folder.Name.Length);
					CopyObject(BucketName + PathSeparator + PreparePath(file.FullName), newFolderPath + PathSeparator + filePath);
				}
			}
			if(loadContents)
				LoadContents();
		}
		void CopyObject(string path, string destinationPath) {
			if(destinationPath.StartsWith(PathSeparator))
				destinationPath = destinationPath.Substring(1);
			Helper.CopyItem(destinationPath, path);
		}
		void CreateParentFolders(FileManagerFolder parent) {
			var foldersCache = CreatedFoldersCache;
			while(parent.Parent != null) {
				var path = PreparePath(parent.FullName) + PathSeparator;
				parent = parent.Parent;
				if(foldersCache.Contains(path))
					break;
				Helper.CreateFolder(path);
				foldersCache.Add(path);				
			}
			CreatedFoldersCache = foldersCache;
		}
		void EnsureContentsLoaded() {
			if(!contentsLoaded)
				LoadContents();
		}
		void LoadContents() {
			contents = GetContentsList();
			files.Clear();
			folders.Clear();
			var rootFolderLength = InternalRootFolder.Length;
			foreach(var item in contents) {
				var path = item.Key;
				if(path.EndsWith(PathSeparator))
					folders.Add(new FileManagerFolder(this, path.Substring(rootFolderLength).TrimEnd('/')));
				else if(path.Length > rootFolderLength) {
					files.Add(new FileManagerFile(this, path.Substring(rootFolderLength)));
					while(path.IndexOf(PathSeparator) > -1 && path != InternalRootFolder) {
						path = path.Substring(0, path.LastIndexOf(PathSeparator));
						if(folders.Exists(f => PreparePath(f.FullName) == path))
							continue;
						folders.Add(new FileManagerFolder(this, path.Substring(rootFolderLength)));
					}
				}
			}
			contentsLoaded = true;
		}
		void helper_CloudRequestEvent(object sender, CloudRequestEventArgs e) {
			RaiseRequestEvent(new FileManagerCloudProviderRequestEventArgs(e.Request));
		}
		void helper_CloudRequestExceptionEvent(object sender, CloudRequestExceptionEventArgs e) {
			e.Exception = new FileManagerCloudAccessFailedException(e.ResponseCode, e.RequestResult);
		}
		protected override string PreparePath(string path) {
			return base.PreparePath(path).TrimStart('/');
		}
		List<Contents> GetContentsList() {
			List<Contents> contentsList = new List<Contents>();
			bool finished = false;
			do {
				string marker = null;
				if(contentsList.Count > 0) {
					Contents lastItem = contentsList[contentsList.Count - 1];
					marker = lastItem.Key;
				}
				string resultStr = Helper.GetContentsList(InternalRootFolder, marker);
				finished = ProcessGetContentsListResult(resultStr, contentsList);
			} while(!finished);
			return contentsList;
		}
		bool ProcessGetContentsListResult(string resultString, List<Contents> contentsList) {
			XDocument doc = XDocument.Parse(resultString);
			string isTruncatedStr = GetFirstXmlElementValue(doc.Root, "IsTruncated");
			bool isTruncated = bool.Parse(isTruncatedStr);
			IEnumerable<Contents> contentsQuery = doc.Root.Elements().
				Where(el => el.Name.LocalName == "Contents").
				Select(el => new Contents {
					Key = GetFirstXmlElementValue(el, "Key"),
					LastModified = GetFirstXmlElementValue(el, "LastModified"),
					Size = GetFirstXmlElementValue(el, "Size")
				});
			foreach(Contents contents in contentsQuery)
				contentsList.Add(contents);
			return !isTruncated;
		}
		string GetFirstXmlElementValue(XElement parentElement, string name) {
			return parentElement.Elements().First(el => el.Name.LocalName == name).Value;
		}
		public class Contents {
			public string Key { get; set; }
			public string LastModified { get; set; }
			public string Size { get; set; }
		}
	}
}
