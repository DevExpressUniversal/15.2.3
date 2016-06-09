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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using Microsoft.Win32;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Internal {
	public class HelperUploadManager {
		protected internal static readonly TimeSpan TempFileExpirationTime = TimeSpan.FromMinutes(20);
		public static Cache Cache { get { return HttpRuntime.Cache; } }
		public static PartialUploadHelperBase FindUploadHelper(string uploadKey) {
			return Cache[uploadKey] as PartialUploadHelperBase;
		}
		public static void RemoveUploadHelper(string uploadKey) {
			Cache.Remove(uploadKey);
		}
		static void RemovedCallback(string key, object value, CacheItemRemovedReason reason) {
			PartialUploadHelperBase helper = value as PartialUploadHelperBase;
			helper.Dispose();
		}
		HttpContext context;
		string uploadKey;
		HelperUploadPackage uploadPackage;
		UploadInternalSettingsBase uploadSettings;
		PartialUploadHelperBase uploadHelper;
		protected HttpContext Context { 
			get {
				return this.context;
			}
		}
		protected string UploadKey { 
			get {
				return this.uploadKey;
			}
		}
		protected HelperUploadPackage UploadPackage {
			get {
				if(this.uploadPackage == null)
					this.uploadPackage = GetUploadPackage();
				return this.uploadPackage;
			}
		}
		protected PartialUploadHelperBase UploadHelper {
			get {
				if(this.uploadHelper == null)
					this.uploadHelper = FindUploadHelper(UploadKey);
				return this.uploadHelper;
			}
			set {
				this.uploadHelper = value;
			}
		}
		protected UploadInternalSettingsBase UploadSettings {
			get {
				if(this.uploadSettings == null)
					this.uploadSettings = UploadSecurityHelper.GetUploadSettingsByID(UploadPackage.SettingsID);
				return this.uploadSettings;
			}
		}
		public HelperUploadManager(HttpContext context, string uploadKey) {
			this.context = context;
			this.uploadKey = uploadKey;
		}
		public UploadProgressStatus ProcessRequest() {
			try {
				VerifyRequest();
				return UnsafeProcessRequest();
			}
			catch(Exception ex) {
				string errorMessage = ASPxUploadControl.HandleCallbackExceptionInternal(ex);
				return GetErrorStatus(errorMessage);
			}
		}
		void VerifyRequest() {
			string uploaderID = ASPxUploadControl.GetHelperUploadingCallbackParam(Context.Request);
			UploadSecurityHelper.VerifyUploadParams(UploadKey, UploadPackage.SettingsID, uploaderID, UploadPackage.Signature);
		}
		UploadProgressStatus UnsafeProcessRequest() {
			UploadProgressStatus status = null;
			if(UploadPackage.IsCancel)
				CancelUploading();
			else
				ProcessUploadPackage();
			return status;
		}
		UploadProgressStatus ProcessUploadPackage() {
			UploadProgressStatus status = null;
			if(UploadPackage.IsNewUploading) {
				if(UploadHelper != null)
					RemoveUploadHelper();
				CreateUploadHelper();
			}
			if(UploadHelper != null) {
				UploadHelper.AppendPackage(UploadPackage);
				status = GetSuccessStatus();
			}
			else
				status = GetErrorStatus(null);
			return status;
		}
		HelperUploadPackage GetUploadPackage() {
			HelperUploadPackage package = new HelperUploadPackage();
			package.Load(Context.Request);
			return package;
		}
		UploadProgressStatus GetSuccessStatus() {
			UploadProgressStatus status = new UploadProgressStatus(0);
			HelperPostedFileBase file = UploadHelper.GetCurrentProcessedFile();
			status.UpdateStatus(file.FileName, file.FileSize, file.UploadedSize, "", UploadHelper.UploadedLength, UploadHelper.TotalLength);
			return status;
		}
		UploadProgressStatus GetErrorStatus(string errorMessage) {
			if(string.IsNullOrEmpty(errorMessage))
				errorMessage = ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_UnspecifiedError);
			return new UploadProgressStatus(errorMessage);
		}
		void CreateUploadHelper() {
			this.uploadHelper = UploadSettings.CreatePartialUploadHelper(UploadKey);
			Cache.Insert(UploadKey, UploadHelper, null, Cache.NoAbsoluteExpiration,
				TempFileExpirationTime, CacheItemPriority.NotRemovable, new CacheItemRemovedCallback(RemovedCallback));
		}
		void RemoveUploadHelper() {
			RemoveUploadHelper(UploadKey);
			UploadHelper = null;
		}
		void CancelUploading() {
			UploadHelper.Dispose();
			RemoveUploadHelper();
		}
	}
	public class HelperUploadPackage {
		bool isCancel = false;
		bool isNewUploading = false;
		string settingsID = string.Empty;
		long totalSize = 0;
		int fileIndex = 0;
		string fileName = "";
		long fileSize = 0;
		string fileType = "";
		byte[] chunk = null;
		int chunkSize = 0;
		string signature = string.Empty;
		public bool IsCancel { get { return isCancel; } }
		public bool IsNewUploading { get { return isNewUploading; } }
		public long TotalSize { get { return totalSize; } }
		public string SettingsID { get { return settingsID; } }
		public int FileIndex { get { return fileIndex; } }
		public string FileName { get { return fileName; } }
		public long FileSize { get { return fileSize; } }
		public string FileType { get { return fileType; } }
		public int ChunkSize { get { return chunkSize; } }
		public byte[] Chunk { get { return chunk; } }
		public string Signature { get { return signature; } }
		public HelperUploadPackage() {
		}
		public void Load(HttpRequest request) {
			if(request.Form.Count == 0) {
				Stream inputStream = request.InputStream;
				byte[] content = new byte[inputStream.Length];
				inputStream.Read(content, 0, (int)inputStream.Length);
				LoadFromContent(content, request.ContentEncoding);
			}
			else
				LoadFromRequest(request);
		}
		void LoadFromContent(byte[] content, Encoding encoding) {
			string inputData = encoding.GetString(content);
			this.isCancel = GetAttributeValue("IsCancel:", "\r\n", inputData) == "true" ? true : false;
			this.isNewUploading = GetAttributeValue("IsNewUploading:", "\r\n", inputData) == "true" ? true : false;
			this.settingsID = GetAttributeValue("SettingsID:", "\r\n", inputData);
			long.TryParse(GetAttributeValue("TotalSize:", "\r\n", inputData), out this.totalSize);
			int.TryParse(GetAttributeValue("FileIndex:", "\r\n", inputData), out this.fileIndex);
			this.fileName = GetAttributeValue("FileName:", "\r\n", inputData);
			long.TryParse(GetAttributeValue("FileSize:", "\r\n", inputData), out this.fileSize);
			this.fileType = GetAttributeValue("FileType:", "\r\n", inputData);
			int.TryParse(GetAttributeValue("ChunkSize:", "\r\n", inputData), out this.chunkSize);
			this.signature = GetAttributeValue("Signature:", "\r\n", inputData);
			string encodingData = GetAttributeValue("EncodingData:", null, inputData);
			this.chunk = System.Convert.FromBase64String(encodingData);
		}
		void LoadFromRequest(HttpRequest request) {
			NameValueCollection form = request.Form;
			this.isCancel = form["IsCancel"] == "true" ? true : false;
			this.isNewUploading = form["IsNewUploading"] == "true" ? true : false;
			this.settingsID = form["SettingsID"];
			long.TryParse(form["TotalSize"], out this.totalSize);
			int.TryParse(form["FileIndex"], out this.fileIndex);
			this.fileName = form["FileName"];
			long.TryParse(form["FileSize"], out this.fileSize);
			this.fileType = form["FileType"];
			int.TryParse(form["ChunkSize"], out this.chunkSize);
			this.signature = form["Signature"];
			if(ChunkSize != 0) {
				Stream inputStream = request.Files[0].InputStream;
				this.chunk = new byte[inputStream.Length];
				inputStream.Read(this.chunk, 0, (int)inputStream.Length);
			}
			else
				this.chunk = new byte[0];
		}
		string GetAttributeValue(string attributeName, string endMarker, string inputData) {
			int startPos = inputData.IndexOf(attributeName);
			if(startPos != -1) {
				startPos += attributeName.Length;
				int endPos = !string.IsNullOrEmpty(endMarker) ? inputData.IndexOf(endMarker, startPos) : inputData.Length;
				if(endPos == -1)
					endPos = inputData.Length;
				return inputData.Substring(startPos, endPos - startPos);
			}
			return "";
		}
	}
	public class PartialUploadHelperBase : IDisposable {
		public PartialUploadHelperBase(string uploadKey) {
			Key = uploadKey;
			Files = new Dictionary<int, HelperPostedFileBase>();
			UploadedLength = 0;
			TotalLength = 0;
			FileIndex = 0;
			LastAccessedTime = DateTime.UtcNow;
		}
		public UploadInternalSettingsBase UploadSettings { get; set; }
		public Dictionary<int, HelperPostedFileBase> Files { get; private set; }
		public string Key { get; private set; }
		public int FileIndex { get; private set; }
		public long TotalLength { get; private set; }
		public long UploadedLength { get; private set; }
		public DateTime LastAccessedTime { get; private set; }
		public long CurrentChunkIndex { get; set; }
		public void AppendPackage(HelperUploadPackage package) {
			EnsureInitialized(package);
			FileIndex = package.FileIndex;
			HelperPostedFileBase postedFile = GetCurrentProcessedFile();
			if(postedFile == null) {
				if(MaxFileCountExceeded())
					throw new Exception(StringResources.UploadControl_UnspecifiedErrorText);
				postedFile = CreatePostedFile(package.FileName, package.FileSize, package.FileType);
				Files.Add(FileIndex, postedFile);
				CurrentChunkIndex = 0;
			}
			try {
				if(!ValidateFileSize(postedFile, package))
					throw new Exception(StringResources.UploadControl_UnspecifiedErrorText);
				postedFile.AppendBytes(package.Chunk, package.ChunkSize, CurrentChunkIndex);
				UploadedLength += package.ChunkSize;
				CurrentChunkIndex++;
			}
			catch {
				Dispose();
				throw;
			}
			LastAccessedTime = DateTime.UtcNow;
		}
		public void Dispose() {
			foreach(HelperPostedFileBase file in Files.Values) {
				try {
					file.Dispose();
				}
				catch { }
			}
			Files.Clear();
		}
		public HelperPostedFileBase GetCurrentProcessedFile() {
			HelperPostedFileBase postedFile;
			if(Files.TryGetValue(FileIndex, out postedFile))
				return postedFile;
			return null;
		}
		protected virtual void EnsureInitialized(HelperUploadPackage package) {
			if(UploadSettings == null)
				UploadSettings = UploadSecurityHelper.GetUploadSettingsByID(package.SettingsID);
			if(TotalLength == 0)
				TotalLength = package.TotalSize;
		}
		protected virtual string GetInternalFileName(string fileName) {
			return string.Empty;
		}
		protected HelperPostedFileBase CreatePostedFile(string fileName, long fileSize, string fileType) {
			HelperPostedFileBase postedFile = UploadSettings.CreatePostedFile(fileName);
			postedFile.InternalFileName = GetInternalFileName(fileName);
			postedFile.FileSize = fileSize;
			postedFile.ContentType = fileType;
			return postedFile;
		}
		bool ValidateFileSize(HelperPostedFileBase postedFile, HelperUploadPackage package) {
			return (package.ChunkSize >= 0) && ((UploadSettings.MaxFileSize == 0) || (postedFile.UploadedSize + package.ChunkSize <= UploadSettings.MaxFileSize));
		}
		bool MaxFileCountExceeded() {
			return UploadSettings.MaxFileCount > 0 && Files.Count >= UploadSettings.MaxFileCount;
		}
		void IDisposable.Dispose() {
			Dispose();
		}
	}
	public class FileSystemPartialUploadHelper : PartialUploadHelperBase {
		public const string TempFileExtension = ".tmp";
		public FileSystemPartialUploadHelper(string uploadKey)
			: base(uploadKey) {
		}
		string TempFolderUrl { get; set; }
		UploadDefaultInternalSettings Settings {
			get {
				return UploadSettings as UploadDefaultInternalSettings;
			}
		}
		protected override void EnsureInitialized(HelperUploadPackage package) {
			base.EnsureInitialized(package);
			if(string.IsNullOrEmpty(TempFolderUrl))
				TempFolderUrl = Settings.TemporaryFolder;
			try {
				PrepareTempFolder();
			}
			catch {
				throw new Exception(string.Format(ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_AccessDeniedError), TempFolderUrl));
			}
		}
		protected override string GetInternalFileName(string fileName) {
			string tempFolderPath = GetTempFolderPath();
			string uniqueFileName = ASPxUploadControl.TemporaryFileNamePrefix + Key + Path.GetRandomFileName() + TempFileExtension;
			if(tempFolderPath.EndsWith(@"\", StringComparison.Ordinal))
				return tempFolderPath + uniqueFileName;
			return (tempFolderPath + @"\" + uniqueFileName);
		}
		void PrepareTempFolder() {
			string tempFolderPath = GetTempFolderPath();
			if(!Directory.Exists(tempFolderPath))
				Directory.CreateDirectory(tempFolderPath);
		}
		string GetTempFolderPath() {
			return UrlUtils.ResolvePhysicalPath(TempFolderUrl);
		}
	}
	public class StoragePartialUploadHelper : PartialUploadHelperBase {
		public StoragePartialUploadHelper(string uploadKey)
			: base(uploadKey) {
		}
		protected override string GetInternalFileName(string fileName) {
			return ASPxUploadControl.GetFileNameInStorage(fileName);
		}
	}
	public class HelperPostedFileBase : IDisposable {
		string contentType;
		protected bool IsInProgress { get; set; }
		protected bool IsCommited { get; set; }
		protected bool IsChunkedUpload { get; set; }
		public UploadInternalSettingsBase Settings { get; set; }
		public string ContentType {
			get {
				if(string.IsNullOrEmpty(this.contentType))
					this.contentType = MimeMapping.GetMimeMapping(FileName);
				return this.contentType;
			}
			set {
				this.contentType = value;
			}
		}
		public string FileName { get; set; }
		public long FileSize { get; set; }
		public FileStream InputStream { 
			get {
				return GetInputStream();
			}
		}
		public long UploadedSize { get; set; }
		public string InternalFileName { get; set; }
		public HelperPostedFileBase()
			: this(string.Empty, 0, string.Empty, string.Empty) {
		}
		public HelperPostedFileBase(string fileName, long fileSize, string contentType, string keyName) {
			FileName = fileName;
			FileSize = fileSize;
			ContentType = contentType;
			InternalFileName = keyName;
		}
		public virtual void SaveAs(string fileName, bool allowOverwrite) {
		}
		public void AppendBytes(byte[] chunk, int chunkSize, long chunkIndex) {
			if(chunkIndex == 0)
				StartChunkedUpload();
			AppendBytesCore(chunk, chunkSize, chunkIndex);
		}
		public bool IsEmpty() {
			return string.IsNullOrEmpty(FileName);
		}
		public void CommitChunkedUpload() {
			CommitChunkedUploadCore();
			IsCommited = true;
			IsInProgress = false;
		}
		public void AbortChunkedUpload() {
			AbortChunkedUploadCore();
			IsCommited = false;
			IsInProgress = false;
		}
		public void SetContent(Stream content) {
			SetContentCore(content);
			IsChunkedUpload = false;
		}
		public virtual void Dispose() {
		}
		protected internal virtual Stream GetInputStreamInternal() {
			return InputStream;
		}
		protected virtual FileStream GetInputStream() {
			return null;
		}
		protected virtual void AppendBytesCore(byte[] chunk, int chunkSize, long chunkIndex) {
			throw new NotImplementedException();
		}
		protected virtual void StartChunkedUploadCore() {
		}
		protected virtual void CommitChunkedUploadCore() {
		}
		protected virtual void AbortChunkedUploadCore() {
		}
		protected virtual void SetContentCore(Stream content) {
			throw new NotImplementedException();
		}
		protected void StartChunkedUpload() {
			StartChunkedUploadCore();
			IsInProgress = true;
			IsChunkedUpload = true;
		}
		void IDisposable.Dispose() {
			Dispose();
		}
	}
	public class TemporaryPostedFile : HelperPostedFileBase {
		public TemporaryPostedFile()
			: base() {
		}
		public TemporaryPostedFile(string fileName, long fileSize, string contentType, string internalFileName)
			: base(fileName, fileSize, contentType, internalFileName) {
		}
		public override void SaveAs(string fileName, bool allowOverwrite) {
			HttpRuntimeSection runtimeSection = null;
			if(WebConfigurationManager.GetSection("HttpRuntime") != null)
				runtimeSection = WebConfigurationManager.GetSection("HttpRuntime") as HttpRuntimeSection;
			if(!Path.IsPathRooted(fileName) && (runtimeSection != null) &&
				runtimeSection.RequireRootedSaveAsPath)
				throw new HttpException(string.Format(StringResources.UploadControl_SaveAsRequires, fileName));
			File.Copy(InternalFileName, fileName, allowOverwrite);
		}
		public override void Dispose() {
			if(!string.IsNullOrEmpty(InternalFileName))
				File.Delete(InternalFileName);
		}
		protected override void AppendBytesCore(byte[] chunk, int chunkSize, long chunkIndex) {
			using(FileStream stream = File.Open(InternalFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {
				stream.Seek(0, SeekOrigin.End);
				stream.Write(chunk, 0, chunkSize);
				UploadedSize += chunkSize;
				stream.Flush();
				stream.Close();
			}
		}
		protected override FileStream GetInputStream() {
			return File.OpenRead(InternalFileName);
		}
	}
	public class StoragePostedFile : HelperPostedFileBase {
		UploadStorageProviderBase StorageProvider { get; set; }
		public StoragePostedFile(UploadStorageProviderBase storageProvider, string fileName)
			: base(fileName, 0, string.Empty, string.Empty) {
			StorageProvider = storageProvider;
		}
		protected override void StartChunkedUploadCore() {
			StorageProvider.StartChunkedUpload(InternalFileName, ContentType);
		}
		protected override void AppendBytesCore(byte[] chunk, int chunkSize, long chunkIndex) {
			using(MemoryStream chunkStream = new MemoryStream(chunk)){
				StorageProvider.UploadObjectChunk(InternalFileName, chunkStream, chunkIndex);
			}
			UploadedSize += chunkSize;
		}
		protected override void CommitChunkedUploadCore() {
			StorageProvider.CompleteChunkedUpload(InternalFileName);
		}
		protected override void AbortChunkedUploadCore() {
			StorageProvider.AbortChunkedUpload(InternalFileName);
		}
		protected override void SetContentCore(Stream content) {
			StorageProvider.UploadObject(InternalFileName, content, ContentType);
		}
		public override void Dispose() {
			if(IsInProgress && IsChunkedUpload)
				AbortChunkedUpload();
		}
	}
	public class UploadStorageProviderBase {
		protected const int BytesInMegabyte = 1024 * 1024;
		public void UploadObject(string objectName, Stream objectData, string contentType) {
			if(MaxSingleUploadDataLength > 0 && objectData.Length > MaxSingleUploadDataLength)
				UploadObjectByChunks(objectName, objectData, contentType);
			else
				UploadEntireObject(objectName, objectData, contentType);
		}
		public void UploadObject(string objectName, Stream objectData) {
			UploadObject(objectName, objectData, null);
		}
		public virtual void UploadEntireObject(string objectName, Stream objectData, string contentType) {
			throw new NotImplementedException();
		}
		public virtual void UploadObjectChunk(string objectName, Stream chunkData, long chunkIndex) {
			throw new NotImplementedException();
		}
		public virtual void StartChunkedUpload(string objectName, string contentType) {
		}
		public virtual void CompleteChunkedUpload(string objectName) {
		}
		public virtual void AbortChunkedUpload(string objectName) {
		}
		protected virtual int MaxSingleUploadDataLength {
			get { throw new NotImplementedException(); }
		}
		protected virtual int DefaultChunkSize {
			get { throw new NotImplementedException(); }
		}
		protected void UploadObjectByChunks(string objectName, Stream objectStream, string contentType) {
			StartChunkedUpload(objectName, contentType);
			try {
				UploadObjectByChunksCore(objectName, objectStream, contentType);
				CompleteChunkedUpload(objectName);
			}
			catch {
				AbortChunkedUpload(objectName);
				throw;
			}
		}
		protected void UploadObjectByChunksCore(string objectName, Stream objectStream, string contentType) {
			long uploadedSize = 0;
			long objectSize = objectStream.Length;
			long remainingSize = objectSize;
			long chunkIndex = 0;
			byte[] chunkData = null;
			while(uploadedSize < objectSize) {
				if(remainingSize < DefaultChunkSize)
					chunkData = new byte[remainingSize];
				else if(chunkData == null)
					chunkData = new byte[DefaultChunkSize];
				int currentChunkSize = chunkData.Length;
				objectStream.Read(chunkData, 0, currentChunkSize);
				using(MemoryStream partStream = new MemoryStream(chunkData)) {
					UploadObjectChunk(objectName, partStream, chunkIndex);
				}
				uploadedSize += currentChunkSize;
				remainingSize -= currentChunkSize;
				chunkIndex++;
			}
		}
	}
	public class FileSystemStorageProvider : UploadStorageProviderBase {
		string UploadFolder { get; set; }
		public FileSystemStorageProvider(string uploadFolder) {
			UploadFolder = PrepareUploadFolder(uploadFolder);			
		}
		public override void UploadObjectChunk(string objectName, Stream chunkData, long chunkIndex) {
			using(FileStream stream = File.Open(GetFilePath(objectName), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {
				stream.Seek(0, SeekOrigin.End);
				chunkData.CopyTo(stream);
			}
		}
		public override void UploadEntireObject(string objectName, Stream objectData, string contentType) {
			UploadObjectChunk(objectName, objectData, 0);
		}
		public override void AbortChunkedUpload(string objectName) {
			if(!string.IsNullOrEmpty(objectName))
				File.Delete(GetFilePath(objectName));
		}
		protected override int MaxSingleUploadDataLength {
			get {
				return 0;
			}
		}
		protected string GetFilePath(string objectName) {
			return Path.Combine(UploadFolder, objectName);
		}
		string PrepareUploadFolder(string uploadFolder) {
			if(string.IsNullOrEmpty(uploadFolder))
				throw new Exception("FileSystemSettings.UploadFolder is not specified");
			if(!Path.IsPathRooted(uploadFolder))
				uploadFolder = UrlUtils.ResolvePhysicalPath(uploadFolder);
			if(!Directory.Exists(uploadFolder))
				throw new DirectoryNotFoundException();
			return uploadFolder;
		}
	}
	public class AzureUploadStorageProvider : UploadStorageProviderBase {
		const int MaxPutBlobDataLength = 32 * BytesInMegabyte;
		const int PutBlockDataLength = 2 * BytesInMegabyte;
		AzureBlobStorageHelper AzureHelper { get; set; }
		long BlockCount { get; set; }
		string ObjectName { get; set; }
		string ContentType { get; set; }
		bool IsEmptyObject { get; set; }
		public AzureUploadStorageProvider(AzureBlobStorageHelper azureHelper) {
			AzureHelper = azureHelper;
		}
		public override void UploadEntireObject(string objectName, Stream objectData, string contentType) {
			AzureHelper.PutBlob(objectName, objectData, contentType);
		}
		public override void StartChunkedUpload(string objectName, string contentType) {
			BlockCount = 0;
			ObjectName = objectName;
			ContentType = contentType;
			IsEmptyObject = false;
		}
		public override void UploadObjectChunk(string objectName, Stream chunkData, long chunkIndex) {
			IsEmptyObject = chunkIndex == 0 && chunkData.Length == 0;
			if(!IsEmptyObject) {
				AzureHelper.PutBlock(objectName, chunkData, (int)chunkData.Length, chunkIndex);
				BlockCount++;
			}
		}
		public override void CompleteChunkedUpload(string objectName) {
			if(IsEmptyObject) {
				using(MemoryStream emptyStream = new MemoryStream()) {
					UploadEntireObject(ObjectName, emptyStream, ContentType);
				}
			}
			else
				AzureHelper.PutBlockList(objectName, BlockCount, ContentType);
		}
		protected override int MaxSingleUploadDataLength { get { return MaxPutBlobDataLength; } }
		protected override int DefaultChunkSize { get { return PutBlockDataLength; } }
	}
	public class AmazonUploadStorageProvider : UploadStorageProviderBase {
		const int MaxPutObjectDataLength = 64 * BytesInMegabyte;
		const int UploadPartDataLength = 5 * BytesInMegabyte;
		AmazonS3Helper AmazonHelper { get; set; }
		string UploadId { get; set; }
		List<string> ETags { get; set; }
		public AmazonUploadStorageProvider(AmazonS3Helper amazonHelper) {
			AmazonHelper = amazonHelper;
		}
		public override void UploadEntireObject(string objectName, Stream objectData, string contentType) {
			AmazonHelper.PutObject(objectName, objectData, contentType);
		}
		public override void StartChunkedUpload(string objectName, string contentType) {
			if(ETags == null)
				ETags = new List<string>();
			UploadId = AmazonHelper.InitiateMultipartUpload(objectName, contentType);
		}
		public override void UploadObjectChunk(string objectName, Stream chunkData, long chunkIndex) {
			string eTag = AmazonHelper.UploadPart(objectName, chunkData, UploadId, (int)chunkIndex + 1);
			ETags.Add(eTag);
		}
		public override void CompleteChunkedUpload(string objectName) {
			AmazonHelper.CompleteMultipartUpload(objectName, UploadId, ETags);
			ClearUploadState();
		}
		public override void AbortChunkedUpload(string objectName) {
			AmazonHelper.AbortMultipartUpload(objectName, UploadId);
			ClearUploadState();
		}
		protected override int MaxSingleUploadDataLength { get { return MaxPutObjectDataLength; } }
		protected override int DefaultChunkSize { get { return UploadPartDataLength; } }
		void ClearUploadState() {
			ETags.Clear();
			UploadId = null;
		}
	}
	public class DropboxUploadStorageProvider : UploadStorageProviderBase {
		const int UploadPartDataLength = 4 * BytesInMegabyte;
		const int MaxPutObjectDataLength = UploadPartDataLength;
		DropboxHelper DropboxHelper { get; set; }
		string UploadFolder { get; set; }
		string UploadId { get; set; }
		long UploadedSize;
		public DropboxUploadStorageProvider(DropboxHelper dropboxHelper)
			: this(dropboxHelper, string.Empty) {
		}
		public DropboxUploadStorageProvider(DropboxHelper dropboxHelper, string uploadFolder) {
			DropboxHelper = dropboxHelper;
			UploadFolder = uploadFolder;
		}
		public override void UploadEntireObject(string objectName, Stream objectData, string contentType) {
			DropboxHelper.PutObject(GetObjectPath(objectName), objectData, contentType);
		}
		public override void StartChunkedUpload(string objectName, string contentType) {
			ClearUploadState();
		}
		public override void UploadObjectChunk(string objectName, Stream chunkData, long chunkIndex) {
			var chunkLength = chunkData.Length;
			UploadId = DropboxHelper.UploadPart(chunkData, UploadId, UploadedSize);
			UploadedSize += chunkLength;
		}
		public override void CompleteChunkedUpload(string objectName) {
			DropboxHelper.CompleteMultipartUpload(GetObjectPath(objectName), UploadId);
			ClearUploadState();
		}
		public override void AbortChunkedUpload(string objectName) {
			ClearUploadState();
		}
		protected override int MaxSingleUploadDataLength { get { return MaxPutObjectDataLength; } }
		protected override int DefaultChunkSize { get { return UploadPartDataLength; } }
		string GetObjectPath(string objectName) {
			return Path.Combine(UploadFolder, objectName);
		}
		void ClearUploadState() {
			UploadId = null;
			UploadedSize = 0;
		}
	}
	internal static class UploadSecurityHelper {
		static object securityKeyLock = new object();
		static byte[] securityKey = null;
		static object settingsDictionaryLock = new object();
		static ConcurrentDictionary<string, UploadInternalSettingsBase> settingsDictionary;
		static object settingsKeyDictionaryLock = new object();
		static ConcurrentDictionary<UploadInternalSettingsBase, string> settingsKeyDictionary;
		static byte[] SecurityKey {
			get {
				lock(securityKeyLock) {
					if(securityKey == null)
						securityKey = Guid.NewGuid().ToByteArray();
					return securityKey;
				}
			}
		}
		internal static ConcurrentDictionary<string, UploadInternalSettingsBase> SettingsDictionary {
			get {
				lock(settingsDictionaryLock) {
					if(settingsDictionary == null)
						settingsDictionary = new ConcurrentDictionary<string, UploadInternalSettingsBase>();
					return settingsDictionary;
				}
			}
		}
		internal static ConcurrentDictionary<UploadInternalSettingsBase, string> SettingsKeyDictionary {
			get {
				lock(settingsKeyDictionaryLock) {
					if(settingsKeyDictionary == null)
						settingsKeyDictionary = new ConcurrentDictionary<UploadInternalSettingsBase, string>();
					return settingsKeyDictionary;
				}
			}
		}
		internal static UploadInternalSettingsBase GetUploadSettingsByID(string id) {
			return SettingsDictionary[id];
		}
		internal static string GetUploadSettingsID(UploadInternalSettingsBase settings) {
			ConcurrentDictionary<UploadInternalSettingsBase, string> settingsKeyDictionary = SettingsKeyDictionary;
			string settingsID = string.Empty;
			if(!settingsKeyDictionary.TryGetValue(settings, out settingsID)) {
				settingsID = Guid.NewGuid().ToString("N");
				settingsKeyDictionary.AddOrUpdate(settings, settingsID, (sett, settID) => settID);
				SettingsDictionary.AddOrUpdate(settingsID, settings, (settID, sett) => sett);
			}
			return settingsID;
		}
		internal static string GetSignature(string uploadingKey, string uploadSettingsID, string clientID) {
			string dataStr = uploadingKey + uploadSettingsID + clientID;
			byte[] data = Encoding.Default.GetBytes(dataStr);
			byte[] hash;
			using(HMACSHA256 hasher = new HMACSHA256(SecurityKey)) {
				hash = hasher.ComputeHash(data);
			}
			return Convert.ToBase64String(hash);
		}
		internal static void VerifyUploadParams(string uploadingKey, string uploadSettingsID, string clientID, string signature) {
			string actualSignature = GetSignature(uploadingKey, uploadSettingsID, clientID);
			if(string.Compare(signature, actualSignature, StringComparison.InvariantCulture) != 0)
				throw new Exception(ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_OperationTimeoutError));
		}
	}
	internal class TemporaryFolderCleaner {
		static readonly TimeSpan GeneralCleanupInterval = TimeSpan.FromMinutes(5);
		static readonly TimeSpan FirstCleanupInterval = TimeSpan.FromSeconds(10);
		static object activateLock = new object();
		static TemporaryFolderCleaner Instance { get; set; }
		public static void Activate() {
			lock(activateLock) {
				if(Instance == null) {
					Instance = new TemporaryFolderCleaner();
					Instance.SetTimeout(FirstCleanupInterval);
				}
			}
		}
		static List<string> GetTempFolderList() {
			return UploadSecurityHelper.SettingsDictionary.Values.
				OfType<UploadDefaultInternalSettings>().
				Select(settings => settings.TemporaryFolder).
				Distinct().
				ToList();
		}
		void Execute() {
			PerformCleanup();
			SetTimeout(GeneralCleanupInterval);
		}
		void SetTimeout(TimeSpan cleanupInterval) {
			HelperUploadManager.Cache.Insert(ASPxUploadControl.TemporaryFolderCleanerKey, this, null, DateTime.UtcNow + cleanupInterval,
				Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, new CacheItemRemovedCallback(RemovedCallback));
		}
		void RemovedCallback(string key, object value, CacheItemRemovedReason reason) {
			Execute();
		}
		void PerformCleanup() {
			foreach(string tempFolder in GetTempFolderList()) {
				try {
					RemoveExpiredTempFiles(tempFolder);
				}
				catch {
				}
			}
		}
		void RemoveExpiredTempFiles(string tempFolder) {
			int uploadKeyLength = ASPxUploadControl.GetNewUploadingKey().Length;
			int fileNameMinLength = ASPxUploadControl.TemporaryFileNamePrefix.Length + uploadKeyLength + FileSystemPartialUploadHelper.TempFileExtension.Length + 1;
			DateTime expirationTime = DateTime.UtcNow - HelperUploadManager.TempFileExpirationTime;
			string physFolderPath = UrlUtils.ResolvePhysicalPath(tempFolder);
			DirectoryInfo dir = new DirectoryInfo(physFolderPath);
			if(dir.Exists) {
				FileInfo[] files = dir.GetFiles(ASPxUploadControl.TemporaryFileNamePrefix + "*");
				foreach(FileInfo file in files.Where(f => f.Name.Length >= fileNameMinLength)) {
					string uploadKey = file.Name.Substring(ASPxUploadControl.TemporaryFileNamePrefix.Length, uploadKeyLength);
					PartialUploadHelperBase helper = HelperUploadManager.FindUploadHelper(uploadKey);
					if(helper == null)
						SafeRemoveFile(file);
					else if(helper.LastAccessedTime < expirationTime)
						HelperUploadManager.RemoveUploadHelper(uploadKey);
				}
			}
		}
		void SafeRemoveFile(FileInfo file) {
			try {
				file.Delete();
			}
			catch { }
		}
	}
	public static class MimeMapping {
		static Dictionary<string, string> Mappings { get; set; }
		static object mappingsLocker = new Object();
		static void LoadMappings() {
			try {
				using(RegistryKey classesRootKey = Registry.ClassesRoot, mimeRootKey = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type")) {
					string[] subKeyNames = classesRootKey.GetSubKeyNames();
					foreach(string keyName in subKeyNames) {
						string currentExt = keyName;
						if(string.IsNullOrWhiteSpace(currentExt) || !currentExt.StartsWith("."))
							continue;
						using(RegistryKey curKey = classesRootKey.OpenSubKey(currentExt)) {
							string currentType = curKey.GetValue("Content Type") as string;
							if(!string.IsNullOrWhiteSpace(currentType))
								AddMapping(currentExt, currentType);
						}
					}
					subKeyNames = mimeRootKey.GetSubKeyNames();
					string extension = string.Empty;
					foreach(string keyName in subKeyNames) {
						string currentType = keyName;
						if(string.IsNullOrWhiteSpace(currentType))
							continue;
						using(RegistryKey curKey = mimeRootKey.OpenSubKey(currentType)) {
							string currentExt = curKey.GetValue("Extension") as string;
							if(!string.IsNullOrWhiteSpace(currentExt))
								AddMapping(currentExt, currentType);
						}
					}
				}
			} 
			catch { }
		}
		static void AddMapping(string fileExtension, string mimeType) {
			if(!Mappings.ContainsKey(fileExtension))
				Mappings.Add(fileExtension, mimeType);
		}
		public static string GetMimeMapping(string fileName) {
			lock(mappingsLocker) {
				if(Mappings == null) {
					Mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					LoadMappings();
				}
				string ext = Path.GetExtension(fileName);
				if(!string.IsNullOrEmpty(ext)) {
					string mimeType;
					if(Mappings.TryGetValue(ext, out mimeType))
						return mimeType;
				}
				return "application/octetstream";
			}
		}
	}
}
