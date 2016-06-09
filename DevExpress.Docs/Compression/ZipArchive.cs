#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using System.IO;
using System.Text;
using DevExpress.Utils;
using System.Collections;
using DevExpress.Utils.Zip;
using DevExpress.Office.Utils;
using System.Collections.Generic;
using DevExpress.Compression.Internal;
using DevExpress.Utils.Zip.Internal;
using System.ComponentModel;
#if !SILVERLIGHT
using System.IO.Compression;
#endif
namespace DevExpress.Compression {
	public class ZipArchive : IZipItemOwner, IEnumerable, IEnumerable<ZipItem>, IDisposable {
		internal const EncryptionType DefaultEncryption = EncryptionType.PkZip;
		public static ZipArchive Read(string fileName) {
			return Read(fileName, ZipItem.GetDefaultEncoding());
		}
		public static ZipArchive Read(Stream stream) {
			return Read(stream, ZipItem.GetDefaultEncoding());
		}
		public static ZipArchive Read(Stream stream, Encoding encoding) {
			ZipFileParserEx parser = new ZipFileParserEx();
			parser.Parse(stream, encoding);
			List<InternalZipFileEx> zipFiles = parser.Records;
			return new ZipArchive(zipFiles);
		}
		public static ZipArchive Read(string fileName, Encoding encoding) {
			FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			ZipArchive zip = Read(stream, encoding);
			zip.SetStreamToDispose(stream);
			return zip;
		}
		string fileName;
		Stream streamToDispose;
		string password;
		Dictionary<string, ZipItem> itemDictionary;
		List<ZipItem> itemList = new List<ZipItem>();
		public ZipArchive() {
			this.itemDictionary = CreateItemDictionary();
			OptionsBehavior = new ZipArchiveOptionsBehavior();
			LongOperationState = new LongOperationState();
		}
		protected virtual Dictionary<string, ZipItem> CreateItemDictionary() {
			return new Dictionary<string, ZipItem>(StringComparer.OrdinalIgnoreCase);
		}
		ZipArchive(List<InternalZipFileEx> zipFiles)
			: this() {
			Guard.ArgumentNotNull(zipFiles, "zipFiles");
			InitializeUnzipItems(zipFiles);
			IsModified = false;
			Password = String.Empty;
			EncryptionType = EncryptionType.None;
		}
		#region Properties
		LongOperationState LongOperationState { get; set; }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipArchiveOptionsBehavior")]
#endif
		public ZipArchiveOptionsBehavior OptionsBehavior { get; private set; }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipArchiveFileName")]
#endif
		public string FileName { get { return fileName; } set { fileName = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipArchiveCount")]
#endif
		public int Count { get { return this.itemList.Count; } }
		public ZipItem this[int index] {
			get {
				if (index < 0 || index >= this.itemList.Count)
					return null;
				return this.itemList[index];
			}
		}
		public ZipItem this[string name] {
			get {
				string actualName = ZipNameUtils.MakeValidName(name);
				if (!this.itemDictionary.ContainsKey(actualName))
					return null;
				return this.itemDictionary[actualName];
			}
		}
		protected bool IsModified { get; set; }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipArchivePassword")]
#endif
		public string Password {
			get { return password; }
			set {
				if (password == value)
					return;
				password = value;
				if (EncryptionType == Compression.EncryptionType.None && !String.IsNullOrEmpty(value))
					EncryptionType = ZipArchive.DefaultEncryption;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipArchiveEncryptionType")]
#endif
		public EncryptionType EncryptionType { get; set; }
		#endregion
		#region Events
		#region OnAllowFileOverwrite
#if !SL
	[DevExpressDocsLocalizedDescription("ZipArchiveAllowFileOverwrite")]
#endif
		public event AllowFileOverwriteEventHandler AllowFileOverwrite;
		protected virtual bool RaiseAllowFileOverwrite(ZipItem item, string targetFile) {
			AllowFileOverwriteEventArgs ea = new AllowFileOverwriteEventArgs(item, targetFile);
			AllowFileOverwriteEventHandler handler = AllowFileOverwrite;
			if (handler != null)
				handler(this, ea);
			return !ea.Cancel;
		}
		#endregion
		#region Error
#if !SL
	[DevExpressDocsLocalizedDescription("ZipArchiveError")]
#endif
		public event ErrorEventHandler Error;
		protected internal virtual void RaiseError(ErrorEventArgs args) {
			if (Error == null)
				return;
			Error(this, args);
		}
		#endregion
		#region Progress
#if !SL
	[DevExpressDocsLocalizedDescription("ZipArchiveProgress")]
#endif
		public event ProgressEventHandler Progress;
		protected internal virtual void RaiseProgress(ProgressEventArgs args) {
			if (Progress == null)
				return;
			Progress(this, args);
		}
		#endregion
#if !SL
	[DevExpressDocsLocalizedDescription("ZipArchiveItemAdding")]
#endif
		public event ZipItemAddingEventHandler ItemAdding;
		protected internal virtual void RaiseItemAdding(ZipItemAddingEventArgs args) {
			if (ItemAdding == null)
				return;
			ItemAdding(this, args);
		}
		#endregion
		public ZipTextItem AddText(string name) {
			if (String.IsNullOrEmpty(name))
				Exceptions.ThrowArgumentException("name", name);
			ZipTextItem item = new ZipTextItem(name);
			SetDateTimeAttributesToNow(item);
			AddInternalItem(item);
			return item;
		}
		public ZipTextItem AddText(string name, string content) {
			ZipTextItem item = AddText(name);
			item.Content = content;
			return item;
		}
		public ZipTextItem AddText(string name, string content, Encoding encoding) {
			ZipTextItem item = AddText(name, content);
			item.ContentEncoding = encoding;
			return item;
		}
		public ZipStreamItem AddStream(string name, Stream stream) {
			if (String.IsNullOrEmpty(name))
				Exceptions.ThrowArgumentException("name", name);
			ZipStreamItem item = new ZipStreamItem(name);
			item.ContentStream = stream;
			SetDateTimeAttributesToNow(item);
			AddInternalItem(item);
			return item;
		}
		public ZipByteArrayItem AddByteArray(string name, byte[] content) {
			if (String.IsNullOrEmpty(name))
				Exceptions.ThrowArgumentException("name", name);
			ZipByteArrayItem item = new ZipByteArrayItem(name, content);
			SetDateTimeAttributesToNow(item);
			AddInternalItem(item);
			return item;
		}
		public ZipFileItem AddFile(string fileName) {
			return AddFile(fileName, null);
		}
		public ZipFileItem AddFile(string fileName, string relativePath) {
			if (String.IsNullOrEmpty(fileName))
				Exceptions.ThrowArgumentException("fileName", fileName);
			FileInfo file = new FileInfo(fileName);
			if (!file.Exists)
				Exceptions.ThrowArgumentException("fileName", fileName);
			string name = ZipNameUtils.GetZipItemName(fileName, relativePath);
			ZipFileItem item = new ZipFileItem(name, fileName);
			item.CreationTimeUtc = file.CreationTime.ToUniversalTime();
			item.LastAccessTimeUtc = file.LastAccessTime.ToUniversalTime();
			item.LastWriteTimeUtc = file.LastWriteTime.ToUniversalTime();
			AddInternalItem(item);
			return item;
		}
		public void AddFiles(IEnumerable<string> fileNames) {
			if (fileNames == null)
				Exceptions.ThrowArgumentException("name", fileNames);
			LongOperationState.Start();
			try {
				foreach (string fileName in fileNames) {
					AddFile(fileName);
					if (!LongOperationState.CanContinue)
						break;
				}
			}
			finally {
				LongOperationState.End();
			}
		}
		public void AddFiles(IEnumerable<string> fileNames, string archivePath) {
			if (fileNames == null)
				Exceptions.ThrowArgumentException("name", fileNames);
			LongOperationState.Start();
			try {
				foreach (string fileName in fileNames) {
					AddFile(fileName, archivePath);
					if (!LongOperationState.CanContinue)
						break;
				}
			}
			finally {
				LongOperationState.End();
			}
		}
		public ZipDirectoryItem AddDirectory(string path) {
			AddDirectory(path, null);
			return null;
		}
		public ZipDirectoryItem AddDirectory(string path, string archivePath) {
			if (String.IsNullOrEmpty(path))
				Exceptions.ThrowArgumentException("path", path);
			RelativePathCalculator relativePathHelper = new RelativePathCalculator(path, archivePath);
			LongOperationState.Start();
			try {
				AddDirectoryCore(path, relativePathHelper);
			}
			catch (Exception e) {
				ErrorEventArgs errorArgs = new ErrorEventArgs(e, String.Empty);
				RaiseError(errorArgs);
				if (!errorArgs.CanContinue)
					LongOperationState.Cancel();
			}
			finally {
				LongOperationState.End();
			}
			return null;
		}
		void AddDirectoryCore(string path, RelativePathCalculator relativePathHelper) {
			path = RelativePathCalculator.GetPathClosedByDirectorySeparatorChar(path);
			DirectoryInfo info = new DirectoryInfo(path);
			AddDirectoryItem(relativePathHelper, info);
			foreach (FileInfo file in info.EnumerateFiles()) {
				AddFile(relativePathHelper.CalculatePath(file.FullName), relativePathHelper.CalculateArchivePath(file.FullName));
				if (!LongOperationState.CanContinue)
					break;
			}
			foreach (DirectoryInfo dir in info.EnumerateDirectories()) {
				AddDirectoryCore(dir.FullName, relativePathHelper);
				if (!LongOperationState.CanContinue)
					break;
			}
		}
		void AddDirectoryItem(RelativePathCalculator pathCalculator, DirectoryInfo dirInfo) {
			string dirPath = pathCalculator.CalculatePath(dirInfo.FullName);
			string relativePath = pathCalculator.CalculateArchivePath(dirInfo.FullName);
			string name = ZipNameUtils.GetZipDirItemName(dirPath, relativePath);
			if (String.IsNullOrEmpty(name))
				return;
			ZipDirectoryItem item = new ZipDirectoryItem(name, dirInfo.FullName);
			item.CreationTimeUtc = dirInfo.CreationTime.ToUniversalTime();
			item.LastAccessTimeUtc = dirInfo.LastAccessTime.ToUniversalTime();
			item.LastWriteTimeUtc = dirInfo.LastWriteTime.ToUniversalTime();
			AddInternalItem(item);
		}
		public void RemoveItem(ZipItem item) {
			Guard.ArgumentNotNull(item, "item");
			RemoveItemCore(item.Name);
		}
		public void RemoveItem(string name) {
			string actualName = ZipNameUtils.MakeValidName(name);
			RemoveItemCore(actualName);
		}
		void RemoveItemCore(string name) {
			ZipItem item = this[name];
			if (item == null)
				return;
			this.itemDictionary.Remove(name);
			this.itemList.Remove(item);
			item.SetOwner(null);
			IsModified = true;
		}
		public ZipDirectoryItem UpdateDirectory(string path) {
			UpdateDirectory(path, null);
			return null;
		}
		public ZipDirectoryItem UpdateDirectory(string path, string archivePath) {
			if (String.IsNullOrEmpty(path))
				Exceptions.ThrowArgumentException("path", path);
			RelativePathCalculator relativePathHelper = new RelativePathCalculator(path, archivePath);
			UpdateDirectoryCore(path, relativePathHelper);
			return null;
		}
		void UpdateDirectoryCore(string path, RelativePathCalculator relativePathHelper) {
			DirectoryInfo info = new DirectoryInfo(path);
			foreach (FileInfo file in info.EnumerateFiles()) {
				UpdateFile(relativePathHelper.CalculatePath(file.FullName), relativePathHelper.CalculateArchivePath(file.FullName));
			}
			foreach (DirectoryInfo dir in info.EnumerateDirectories()) {
				UpdateDirectoryCore(dir.FullName, relativePathHelper);
			}
		}
		public ZipFileItem UpdateFile(string fileName, string relativePath) {
			if (String.IsNullOrEmpty(fileName))
				Exceptions.ThrowArgumentException("fileName", fileName);
			string itemName = ZipNameUtils.GetZipItemName(fileName, relativePath);
			RemoveItem(itemName);
			return AddFile(fileName, relativePath);
		}
		public ZipFileItem UpdateFile(string fileName) {
			return UpdateFile(fileName, null);
		}
		public ZipTextItem UpdateText(string name, string content) {
			if (String.IsNullOrEmpty(name))
				Exceptions.ThrowArgumentException("name", fileName);
			RemoveItem(name);
			return AddText(name, content);
		}
		public ZipTextItem UpdateText(string name, string content, Encoding encoding) {
			if (String.IsNullOrEmpty(name))
				Exceptions.ThrowArgumentException("name", name);
			RemoveItem(name);
			return AddText(name, content, encoding);
		}
		public ZipStreamItem UpdateStream(string name, Stream stream) {
			if (String.IsNullOrEmpty(name))
				Exceptions.ThrowArgumentException("name", name);
			RemoveItem(name);
			return AddStream(name, stream);
		}
		void SetDateTimeAttributesToNow(ZipItem item) {
			item.CreationTime = GetTimeNow();
			item.LastAccessTime = GetTimeNow();
			item.LastWriteTime = GetTimeNow();
		}
		public bool IsValid() {
			return true;
		}
		public void Save(string fileName) {
			try {
				using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
					Save(stream);
					stream.SetLength(stream.Position);
					IsModified = false;
				}
			}
			catch (Exception e) {
				RaiseError(new ErrorEventArgs(e, String.Empty));
			}
		}
		public void Save(Stream stream) {
			using (ZipArchiver archiver = new ZipArchiver(stream)) {
				archiver.Error += OnError;
				int count = itemDictionary.Count;
				ZipTotalProgress totalProgress = new ZipTotalProgress(count);
				totalProgress.NotifyProgress += OnProgressNotify;
				foreach (KeyValuePair<string, ZipItem> item in itemDictionary) {
					if (!archiver.CanContinue)
						break;
					ZipItem zipItem = item.Value;
					IZipComplexOperationProgress itemProgress = totalProgress.BeginItemProcess();
					archiver.WriteFile(zipItem, itemProgress);
					totalProgress.EndItemProcess();
					if (totalProgress.IsStopped || !archiver.CanContinue)
						break;
					zipItem.IsModified = false;
				}
			}
			IsModified = false;
		}
		void OnProgressNotify(object sender, EventArgs e) {
			ZipTotalProgress progress = sender as ZipTotalProgress;
			if (progress == null)
				return;
			ProgressEventArgs args = new ProgressEventArgs(progress.CurrentProgress);
			RaiseProgress(args);
			if (!args.CanContinue)
				progress.Stop();
		}
		void OnError(object sender, ErrorEventArgs args) {
			RaiseError(args);
		}
		public void Dispose() {
			int count = this.itemList.Count;
			for (int i = 0; i < count; i++)
				this.itemList[i].SetOwner(null);
			this.itemList.Clear();
			this.itemDictionary.Clear();
			if (this.streamToDispose != null)
				this.streamToDispose.Dispose();
		}
		void SetStreamToDispose(Stream streamToDispose) {
			this.streamToDispose = streamToDispose;
		}
		protected virtual DateTime GetTimeNow() {
			return DateTime.Now;
		}
		void InitializeUnzipItems(List<InternalZipFileEx> zipFiles) {
			int count = zipFiles.Count;
			for (int i = 0; i < count; i++) {
				InternalZipFileEx zipFile = zipFiles[i];
				UnzipItem item = new UnzipItem(zipFile);
				AddInternalItemCore(item);
			}
		}
		void AddInternalItemCore(ZipItem item) {
			System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(item.Name));
			this.itemDictionary[item.Name] = item;
			this.itemList.Add(item);
			item.SetOwner(this);
			IsModified = true;
		}
		void AddInternalItem(ZipItem item) {
			ZipItemAddingEventArgs args = new ZipItemAddingEventArgs(item);
			RaiseItemAdding(args);
			if (args.Action == ZipItemAddingAction.Cancel)
				return;
			if (args.Action == ZipItemAddingAction.Stop) {
				LongOperationState.Cancel();
				return;
			}
			AddInternalItemCore(item);
			item.Password = Password;
			item.EncryptionType = EncryptionType;
		}
		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerator<ZipItem> IEnumerable<ZipItem>.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerator<ZipItem> GetEnumerator() {
			int count = this.itemList.Count;
			for (int i = 0; i < count; i++)
				yield return this.itemList[i];
		}
		#endregion
		#region IZipItemOwner implementation
		string IZipItemOwner.Password { get { return Password; } set { Password = value; } }
		bool IZipItemOwner.CanOverwriteFile(ZipItem zipItem, string targetFilePath) {
			return RaiseAllowFileOverwrite(zipItem, targetFilePath); ;
		}
		void IZipItemOwner.ItemChanged(ZipItem zipItem, string propertyName, object oldValue, object newValue) {
			if (zipItem == null)
				return;
			if (propertyName == "Name") {
				string oldName = Convert.ToString(oldValue);
				RemoveItemCore(oldName);
				AddInternalItem(zipItem);
			}
		}
		#endregion
		#region Extract
		public void Extract() {
			for (int i = 0; i < Count; i++) {
				ZipItem item = this.itemList[i];
				item.Extract();
			}
		}
		public void Extract(string path, AllowFileOverwriteMode mode) {
			for (int i = 0; i < Count; i++) {
				ZipItem item = this.itemList[i];
				item.Extract(path, mode);
			}
		}
		public void Extract(string path) {
			for (int i = 0; i < Count; i++) {
				ZipItem item = this.itemList[i];
				item.Extract(path);
			}
		}
		#endregion
	}
	public class ZipComplexOperationProgress : IZipComplexOperationProgress {
		public ZipComplexOperationProgress() {
			OperationProgressList = new List<IZipOperationProgress>();
		}
		List<IZipOperationProgress> OperationProgressList { get; set; }
		double totalOperationWeight;
		public double CurrentProgress { get; private set; }
		public double Weight { get { return totalOperationWeight; } }
		public bool IsStopped {
			get {
				int count = OperationProgressList.Count;
				for (int i = 0; i < count; i++) {
					if (OperationProgressList[i].IsStopped)
						return true;
				}
				return false;
			}
		}
		public event EventHandler NotifyProgress;
		public void Stop() {
			int count = OperationProgressList.Count;
			for (int i = 0; i < count; i++)
				OperationProgressList[i].Stop();
		}
		void IZipComplexOperationProgress.AddOperationProgress(IZipOperationProgress progress) {
			System.Diagnostics.Debug.Assert(progress != this);
			OperationProgressList.Add(progress);
			progress.NotifyProgress += OnProgressItemNotifyProgress;
			this.totalOperationWeight += progress.Weight;
		}
		protected void RaiseProgress() {
			if (NotifyProgress == null)
				return;
			NotifyProgress(this, EventArgs.Empty);
		}
		void OnProgressItemNotifyProgress(object sender, EventArgs e) {
			int count = OperationProgressList.Count;
			double totalPorgress = 0;
			for (int i = 0; i < count; i++) {
				IZipOperationProgress item = OperationProgressList[i];
				totalPorgress += item.CurrentProgress * item.Weight / this.totalOperationWeight;
			}
			CurrentProgress = totalPorgress;
			RaiseTotalItemProgress();
		}
		void RaiseTotalItemProgress() {
			if (NotifyProgress != null)
				NotifyProgress(this, EventArgs.Empty);
		}
	}
	public class ZipTotalProgress : IZipOperationProgress {
		double currentProgress;
		int totalItemCount;
		double currentIndx = 0;
		IZipComplexOperationProgress currentItemProgress;
		bool isStopped;
		public ZipTotalProgress(int totalItemCount) {
			this.totalItemCount = totalItemCount;
		}
		public double Weight { get { return 1; } }
		public double CurrentProgress { get { return currentProgress; } }
		public bool IsStopped {
			get {
				if (this.currentItemProgress != null && this.currentItemProgress.IsStopped)
					return true;
				return isStopped;
			}
		}
		public event EventHandler NotifyProgress;
		public void Stop() {
			this.isStopped = true;
			if (this.currentItemProgress != null)
				this.currentItemProgress.Stop();
		}
		public IZipComplexOperationProgress BeginItemProcess() {
			RecalculateProgress();
			this.currentItemProgress = new ZipComplexOperationProgress();
			SubscribeItemProgress(this.currentItemProgress);
			return this.currentItemProgress;
		}
		public void EndItemProcess() {
			if (this.currentItemProgress != null) {
				UnsubscribeItemProgress(this.currentItemProgress);
				this.currentItemProgress = null;
			}
			this.currentIndx++;
			RecalculateProgress();
			RaiseNotifyProgress();
		}
		void SubscribeItemProgress(IZipComplexOperationProgress progress) {
			progress.NotifyProgress += OnItemNotifyProgress;
		}
		void UnsubscribeItemProgress(IZipComplexOperationProgress progress) {
			progress.NotifyProgress -= OnItemNotifyProgress;
		}
		void OnItemNotifyProgress(object sender, EventArgs e) {
			RecalculateProgress();
			RaiseNotifyProgress();
		}
		void RecalculateProgress() {
			double currentItemProgress = this.currentIndx / this.totalItemCount;
			if (this.currentItemProgress != null) {
				double nextItemProgress = (this.currentIndx + 1) / this.totalItemCount;
				currentItemProgress = currentItemProgress + (nextItemProgress - currentItemProgress) * this.currentItemProgress.CurrentProgress;
				if (nextItemProgress < currentItemProgress)
					currentItemProgress = nextItemProgress;
			}
			this.currentProgress = currentItemProgress;
		}
		void RaiseNotifyProgress() {
			if (NotifyProgress != null)
				NotifyProgress(this, EventArgs.Empty);
		}
	}
	public enum ZipItemAddingAction { Continue, Stop, Cancel }
}
namespace DevExpress.Compression.Internal {
	public interface IZipItemOwner {
		string Password { get; set; }
		ZipArchiveOptionsBehavior OptionsBehavior { get; }
		bool CanOverwriteFile(ZipItem zipItem, string targetFilePath);
		void ItemChanged(ZipItem zipItem, string propertyName, object oldValue, object newValue);
	}
}
