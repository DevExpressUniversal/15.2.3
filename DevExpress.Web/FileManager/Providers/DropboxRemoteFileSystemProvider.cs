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
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.Script.Serialization;
using DevExpress.Utils.OAuth;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class DropboxFileSystemProvider : CloudFileSystemProviderBase {
		public DropboxFileSystemProvider(string rootFolder)
			: base(rootFolder) {
		}
		const string CursorKey = "aspxDropboxCursor";
		const string CacheKey = "aspxDropboxProviderItemsCache";
		const int ChunkSize = 4194304;
		bool cacheRefreshed = false;
		DropboxHelper helper;
		DropboxUploadStorageProvider uploadProvider;
		DropboxHelper Helper {
			get {
				if(helper == null) {
					helper = new DropboxHelper(AccessTokenValue);
					helper.CloudRequestEvent += helper_CloudRequestEvent;
					helper.CloudRequestExceptionEvent += helper_CloudRequestExceptionEvent;
				}
				return helper;
			}
		}
		DropboxUploadStorageProvider UploadProvider {
			get {
				if(this.uploadProvider == null)
					this.uploadProvider = new DropboxUploadStorageProvider(Helper);
				return this.uploadProvider;
			}
		}
		public string AccessTokenValue { get; set; }
		string Cursor {
			get { return HttpContext.Current.Session[CursorKey] == null ? string.Empty : HttpContext.Current.Session[CursorKey].ToString(); }
			set { HttpContext.Current.Session[CursorKey] = value; }
		}
		List<ItemInfo> ItemsCache {
			get {
				List<ItemInfo> itemsCache = HttpContext.Current.Session[CacheKey] as List<ItemInfo>;
				if(itemsCache == null || itemsCache.Count(i => PreparePath(i.path).ToLower().StartsWith(PreparePath(RootFolder).ToLower())) != itemsCache.Count) {
					Cursor = null;
					return new List<ItemInfo>();
				}
				return itemsCache;
			}
			set { HttpContext.Current.Session[CacheKey] = value; }
		}
		public override bool Exists(FileManagerFolder folder) {
			return GetItemExists(folder);
		}
		public override bool Exists(FileManagerFile file) {
			return GetItemExists(file);
		}
		public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder) {
			foreach(ItemInfo dir in GetItems(parentFolder, true))
				yield return new FileManagerFolder(this, dir.path.Substring(PreparePath(RootFolder).Length));
		}
		public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder) {
			foreach(var file in GetItems(folder, false))
				yield return new FileManagerFile(this, file.path.Substring(PreparePath(RootFolder).Length));
		}
		public override long GetLength(FileManagerFile file) {
			return GetItemInfo(file).bytes;
		}
		public override DateTime GetLastWriteTime(FileManagerFile file) {
			return DateTime.Parse(GetItemInfo(file).modified);
		}
		public override void DeleteFile(FileManagerFile file) {
			DeleteNode(file.FullName);
		}
		public override void DeleteFolder(FileManagerFolder folder) {
			DeleteNode(folder.FullName);
		}
		public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			MoveNode(file.FullName, newParentFolder.FullName + PathSeparator + file.Name);
		}
		public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			MoveNode(folder.FullName, newParentFolder.FullName + PathSeparator + folder.Name);
		}
		public override void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			CopyNode(file.FullName, newParentFolder.FullName + PathSeparator + file.Name);
		}
		public override void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			CopyNode(folder.FullName, newParentFolder.FullName + PathSeparator + folder.Name);
		}
		public override void RenameFile(FileManagerFile file, string name) {
			CopyNode(file.FullName, file.Folder.FullName + PathSeparator + name, false);
			DeleteNode(file.FullName);
		}
		public override void RenameFolder(FileManagerFolder folder, string name) {
			var path = PreparePath(folder.FullName);
			CopyNode(path, path.Substring(0, path.LastIndexOf(PathSeparator) + 1) + name, false);
			DeleteNode(path);
		}
		public override void CreateFolder(FileManagerFolder parent, string name) {
			var path = PreparePath(parent.FullName) + PathSeparator + name;
			Helper.CreateFolder(path);
			RefreshCache();
		}
		[SecuritySafeCritical]
		public override void UploadFile(FileManagerFolder folder, string fileName, Stream content) {
			string filePath = (string.IsNullOrEmpty(PreparePath(folder.FullName)) ? "" : PreparePath(folder.FullName) + PathSeparator) + fileName;
			using(content) {
				UploadProvider.UploadObject(filePath, content);
			}
		}
		[SecuritySafeCritical]
		public override string GetDownloadUrl(FileManagerFile[] files) {
			var file = files[0];
			if(!Exists(file))
				return string.Empty;
			var serializer = new JavaScriptSerializer();
			MediaInfo mediaData = serializer.Deserialize<MediaInfo>(Helper.GetFileLink(PreparePath(file.FullName)));
			return mediaData.url + "?dl=1";
		}
		public override Stream GetThumbnail(FileManagerFile file) {
			return GetFileStream(file, GetFileResponse);
		}
		protected override WebResponse GetFileResponse(FileManagerFile file) {
			HttpWebRequest request = Helper.GetFileResponse(PreparePath(file.FullName));
			RaiseRequestEvent(new FileManagerCloudProviderRequestEventArgs(request));
			WebResponse resp = request.GetResponse();
			try {
				return resp;
			} catch {
				resp.Close();
				return null;
			}
		}
		WebResponse GetFileThumbnailResponse(FileManagerFile file) {
			HttpWebRequest request = Helper.GetFileThumbnail(PreparePath(file.FullName));
			RaiseRequestEvent(new FileManagerCloudProviderRequestEventArgs(request));
			WebResponse resp = request.GetResponse();
			try {
				return resp;
			}
			catch {
				resp.Close();
				return null;
			}
		}
		void CopyNode(string path, string targetPath, bool refreshCahche = true) {
			Helper.CopyNode(PreparePath(path), PreparePath(targetPath));
			if(refreshCahche)
				RefreshCache();
		}
		void DeleteNode(string path) {
			Helper.DeleteNode(PreparePath(path));
			RefreshCache();
		}
		void MoveNode(string path, string targetPath) {
			Helper.MoveNode(PreparePath(path), PreparePath(targetPath));
			RefreshCache();
		}
		[SecuritySafeCritical]
		void RefreshCache() {
			var itemsCache = ItemsCache;
			var serializer = new JavaScriptSerializer();
			var deltaData = new DeltaInfo();
			while(deltaData.Has_More) {
				var partialDeltaData = serializer.Deserialize<DeltaInfo>(Helper.GetDelta(PreparePath(RootFolder), Cursor));
				deltaData.Entries.AddRange(partialDeltaData.Entries);
				deltaData.Has_More = partialDeltaData.Has_More;
				Cursor = partialDeltaData.Cursor;
			}
			foreach(var entry in deltaData.Entries) {
				var properties = entry[1] as Dictionary<string, object>;
				var path = entry[0].ToString();
				if(properties != null) {
					var itemIndex = itemsCache.IndexOf(GetItemInfo(path));
					var itemInfo = new ItemInfo(properties);
					if(itemIndex == -1)
						itemsCache.Add(itemInfo);
					else
						itemsCache[itemIndex] = itemInfo;
				} else
					itemsCache.Remove(GetItemInfo(path));
			}
			itemsCache.Sort((e0, e1) => e0.path.CompareTo(e1.path));
			ItemsCache = itemsCache;
			cacheRefreshed = true;
		}
		void EnsureCacheRefreshed() {
			if(!cacheRefreshed)
				RefreshCache();
		}
		bool GetItemExists(FileManagerItem item) {
			var path = PreparePath(item.FullName);
			EnsureCacheRefreshed();
			if(path == PathSeparator)
				return true;
			return GetItemInfo(path) != null;
		}
		protected override string PreparePath(string path) {
			path = base.PreparePath(path);
			if(!path.StartsWith(PathSeparator))
				path = PathSeparator + path;
			return path;
		}
		ItemInfo GetItemInfo(FileManagerItem item) {
			return GetItemInfo(PreparePath(item.FullName));
		}
		ItemInfo GetItemInfo(string path) {
			return ItemsCache.Find(i => i.path.ToLower() == PreparePath(path.ToLower()));
		}
		List<ItemInfo> GetItems(FileManagerFolder folder, bool isDirectoires) {
			EnsureCacheRefreshed();
			var path = PreparePath(folder.FullName);
			var items = ItemsCache.Where(i => i.is_dir == isDirectoires &&
												i.path.ToLower().StartsWith((path == PathSeparator ? path : path + PathSeparator).ToLower()) &&
												i.path.ToLower() != path.ToLower() &&
												i.path.Substring(path.Length + 1).IndexOf(PathSeparator) == -1 &&
												i.path != PathSeparator).ToList();
			return items == null ? new List<ItemInfo>() : items;
		}
		void helper_CloudRequestEvent(object sender, CloudRequestEventArgs e) {
			RaiseRequestEvent(new FileManagerCloudProviderRequestEventArgs(e.Request));
		}
		void helper_CloudRequestExceptionEvent(object sender, CloudRequestExceptionEventArgs e) {
			e.Exception = new FileManagerCloudAccessFailedException(e.ResponseCode, e.RequestResult);
		}
		class ItemInfo {
			public ItemInfo() : base() { }
			public ItemInfo(Dictionary<string, object> properties) {
				foreach(var property in properties) {
					var itemProperty = this.GetType().GetProperty(property.Key);
					if(itemProperty == null)
						continue;
					itemProperty.SetValue(this, property.Value, new object[0]);
				}
				this.thumb_created = false;
			}
			public string rev { get; set; }
			public bool thumb_exists { get; set; }
			public string path { get; set; }
			public bool is_dir { get; set; }
			public string client_mtime { get; set; }
			public string icon { get; set; }
			public int bytes { get; set; }
			public string modified { get; set; }
			public string size { get; set; }
			public string root { get; set; }
			public string mime_type { get; set; }
			public int revision { get; set; }
			public List<ItemInfo> contents { get; set; }
			public bool thumb_created { get; set; }
		}
		class DeltaInfo {
			public DeltaInfo() {
				Has_More = true;
				Entries = new List<List<object>>();
			}
			public string Cursor { get; set; }
			public bool Has_More { get; set; }
			public bool Reset { get; set; }
			public List<List<object>> Entries { get; set; }
		}
		class MediaInfo {
			public string url { get; set; }
			public string expires { get; set; }
		}
		class ChunkedUploadInfo {
			public ChunkedUploadInfo() {
				upload_id = string.Empty;
			}
			public string upload_id { get; set; }
			public string offset { get; set; }
			public string expires { get; set; }
		}
	}
}
