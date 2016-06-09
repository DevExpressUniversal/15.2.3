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
using DevExpress.Compression.Internal;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Utils.Zip.Internal;
using System.Diagnostics;
using System.ComponentModel;
namespace DevExpress.Compression.Internal {
	public interface IContentStreamOwner {
		Stream ObtainContentStream();
		bool CanCloseAndDisposeStream { get; }
	}
}
namespace DevExpress.Compression {
	public abstract class ZipItem : IContentStreamOwner {
		#region GetDefaultEncoding
		internal static Encoding GetDefaultEncoding() {
#if !SL && !DXPORTABLE
			return Encoding.GetEncoding(System.Globalization.CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
#else
			return DXEncoding.ASCII;
#endif
		}
		#endregion
		IZipItemOwner owner;
		DateTime creationTimeUtc;
		DateTime lastWriteTimeUtc;
		DateTime lastAccessTimeUtc;
		string comment;
		string name;
		string password;
		EncryptionType encryptionType;
		Encoding encoding;
		protected ZipItem(string name) {
			this.name = name;
			DateTime now = DateTime.Now.ToUniversalTime();
			CreationTimeUtc = now;
			LastWriteTimeUtc = now;
			LastAccessTimeUtc = now;
			Comment = String.Empty;
			Attributes = FileAttributes.Normal;
			Password = String.Empty;
			EncryptionType = Compression.EncryptionType.PkZip;
		}
		#region Properties
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemName")]
#endif
		public string Name {
			get { return name; }
			set {
				if (this.name == value)
					return;
				string oldValue = name;
				this.name = value;
				PropertyChanged("Name", oldValue, value);
				IsModified = true;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemPassword")]
#endif
		public string Password {
			get { return password; }
			set {
				if (password == value)
					return;
				password = value;
				if (EncryptionType == Compression.EncryptionType.None && !String.IsNullOrEmpty(value))
					EncryptionType = ZipArchive.DefaultEncryption;
				IsModified = true;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemComment")]
#endif
		public string Comment {
			get { return comment; }
			set {
				if (Comment == value)
					return;
				comment = value;
				IsModified = true;
			}
		}
		protected internal bool IsModified { get; set; }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemUncompressedSize")]
#endif
		public long UncompressedSize { get; internal set; }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemCompressedSize")]
#endif
		public long CompressedSize { get; protected set; }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemCreationTime")]
#endif
		public DateTime CreationTime { get { return CreationTimeUtc.ToLocalTime(); } set { CreationTimeUtc = value.ToUniversalTime(); } }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemCreationTimeUtc")]
#endif
		public DateTime CreationTimeUtc {
			get { return creationTimeUtc; }
			set {
				creationTimeUtc = value.ToUniversalTime();
				IsModified = true;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemLastWriteTime")]
#endif
		public DateTime LastWriteTime { get { return LastWriteTimeUtc.ToLocalTime(); } set { LastWriteTimeUtc = value.ToUniversalTime(); } }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemLastWriteTimeUtc")]
#endif
		public DateTime LastWriteTimeUtc {
			get { return lastWriteTimeUtc; }
			set {
				lastWriteTimeUtc = value.ToUniversalTime();
				IsModified = true;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemLastAccessTime")]
#endif
		public DateTime LastAccessTime { get { return LastAccessTimeUtc.ToLocalTime(); } set { LastAccessTimeUtc = value.ToUniversalTime(); } }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemLastAccessTimeUtc")]
#endif
		public DateTime LastAccessTimeUtc {
			get { return lastAccessTimeUtc; }
			set {
				lastAccessTimeUtc = value.ToUniversalTime();
				IsModified = true;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemAttributes")]
#endif
		public FileAttributes Attributes { get; set; }
		protected abstract bool CanCloseAndDisposeStream { get; }
		protected IZipItemOwner Owner { get { return owner; } }
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemEncryptionType")]
#endif
		public virtual EncryptionType EncryptionType {
			get {
				return encryptionType;
			}
			set {
				if (encryptionType == value)
					return;
				encryptionType = value;
				IsModified = true;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipItemEncoding")]
#endif
		public Encoding Encoding {
			get {
				if (encoding == null)
#if SL                    
					encoding = DXEncoding.Default;
#else
					encoding = GetDefaultEncoding();
#endif
				return encoding;
			}
			set {
				if (encoding == value)
					return;
				encoding = value;
				IsModified = true;
			}
		}
		#endregion
		protected abstract Stream ObtainContentStream();
		#region IContentStreamOwner
		Stream IContentStreamOwner.ObtainContentStream() {
			return ObtainContentStream();
		}
		bool IContentStreamOwner.CanCloseAndDisposeStream {
			get { return CanCloseAndDisposeStream; }
		}
		#endregion
		#region Extract
		public void Extract() {
			Extract(Environment.CurrentDirectory);
		}
		public void Extract(string directory) {
			if (Owner == null)
				return;
			Extract(directory, Owner.OptionsBehavior.AllowFileOverwrite);
		}
		public void Extract(string directory, AllowFileOverwriteMode allowFileOverwrite) {
			string fileName = Path.Combine(directory, Name);
			FileInfo fileInfo = new FileInfo(fileName);
			if (fileInfo.Exists) {
				AllowFileOverwriteMode conflictMode = allowFileOverwrite;
				if (conflictMode == AllowFileOverwriteMode.Forbidden)
					return;
				if (conflictMode == AllowFileOverwriteMode.Custom && !Owner.CanOverwriteFile(this, fileInfo.FullName))
					return;
			}
			string directoryName = Path.GetDirectoryName(fileName);
			DirectoryInfo directoryNameInfo = new DirectoryInfo(directoryName);
			if (!directoryNameInfo.Exists)
				directoryNameInfo.Create();
#if !SL
			if ((Attributes & FileAttributes.Directory) == FileAttributes.Directory || CanConsiderAsDirectory()) {
				ExtractAsDirectory(directoryNameInfo);
				return;
			}
#endif
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				Extract(stream);
			}
		}
#if !SL
		bool CanConsiderAsDirectory() {
			return CompressedSize == 0 && UncompressedSize == 0 && Name.EndsWith("/");
		}
#endif
#if !SL
		void ExtractAsDirectory(DirectoryInfo directoryNameInfo) {
			directoryNameInfo.Attributes = Attributes;
			directoryNameInfo.CreationTime = CreationTime;
			directoryNameInfo.LastAccessTime = LastAccessTime;
			directoryNameInfo.LastWriteTime = LastWriteTime;
		}
#endif
		public void Extract(Stream stream) {
			Stream contentStream = ObtainContentStream();
			try {
				long oldPosition = 0;
				if (contentStream.CanSeek)
					oldPosition = contentStream.Position;
				StreamUtils.CopyStream(contentStream, stream);
				if (contentStream.CanSeek)
					contentStream.Seek(oldPosition, SeekOrigin.Begin);
			} catch (Exception e) {
				throw e;
			} finally {
				if (CanCloseAndDisposeStream)
					contentStream.Dispose();
			}
		}
		#endregion
		#region Open
		public Stream Open() {
			return ObtainContentStream();
		}
		#endregion
		internal void SetOwner(IZipItemOwner owner) {
			this.owner = owner;
		}
		void PropertyChanged(string propertyName, object oldValue, object newValue) {
			if (Owner == null)
				return;
			Owner.ItemChanged(this, propertyName, oldValue, newValue);
		}
	}
	public class UnzipItem : ZipItem {
		InternalZipFileEx zipFile;
		bool canChangeEncryptionType = true;
		public UnzipItem(InternalZipFileEx zipFile)
			: base(zipFile.FileName) {
			Guard.ArgumentNotNull(zipFile, "zipFile");
			this.zipFile = zipFile;
			UncompressedSize = this.zipFile.UncompressedSize;
			CompressedSize = this.zipFile.CompressedSize;
			LastWriteTime = this.zipFile.FileLastModificationTime;
			LastAccessTime = this.zipFile.FileLastAccessTime;
			CreationTime = this.zipFile.FileCreationTime;
			Comment = zipFile.Comment;
			Attributes = (FileAttributes)this.zipFile.ExternalAttribute;
			if (this.zipFile.EncryptionInfo != null)
				EncryptionType = this.zipFile.EncryptionInfo.Type;
			else
				EncryptionType = Compression.EncryptionType.None;
			IsModified = false;
			this.canChangeEncryptionType = false;
		}
		protected override bool CanCloseAndDisposeStream { get { return true; } }
		public override EncryptionType EncryptionType {
			get {
				return base.EncryptionType;
			}
			set {
				if (!this.canChangeEncryptionType)
					throw new NotSupportedException();
				base.EncryptionType = value;
			}
		}
		protected override Stream ObtainContentStream() {
			string actualPassword = Password;
			if (String.IsNullOrEmpty(actualPassword) && Owner != null)  
				actualPassword = Owner.Password;
			Stream decompressionStream = (String.IsNullOrEmpty(actualPassword)) ? this.zipFile.CreateDecompressionStream() : this.zipFile.CreateDecompressionStream(actualPassword);
			DecompressionStream resultStream = new DecompressionStream(decompressionStream, UncompressedSize);
			return resultStream;
		}
	}
	public class DecompressionStream : Stream {
		long length;
		Stream baseStream;
		public DecompressionStream(Stream baseStream, long length) {
			Guard.ArgumentNotNull(baseStream, "baseStream");
			Debug.Assert(length >= 0);
			this.length = length;
			this.baseStream = baseStream;
		}
		public override long Length { get { return length; } }
		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return false; } }
		public override long Position {
			get {
				throw new NotSupportedException();
			}
			set {
				throw new NotSupportedException();
			}
		}
		public override void Flush() {
			throw new NotSupportedException();
		}
		public override int Read(byte[] buffer, int offset, int count) {
			return this.baseStream.Read(buffer, offset, count);
		}
		public override long Seek(long offset, SeekOrigin origin) {
			throw new NotSupportedException();
		}
		public override void SetLength(long value) {
			throw new NotSupportedException();
		}
		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}
	}
	public class ZipDirectoryItem : ZipItem {
		string directoryName;
		public ZipDirectoryItem(string name, string dirName)
			: base(name) {
			Guard.ArgumentNotNull(dirName, "dirName");
			DirectoryName = dirName;
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipDirectoryItemDirectoryName")]
#endif
		public string DirectoryName {
			get { return directoryName; }
			set {
				if (directoryName == value)
					return;
				directoryName = value;
				OnDirectoryNameChanged();
			}
		}
		void OnDirectoryNameChanged() {
			DirectoryInfo dirInfo = new DirectoryInfo(DirectoryName);
			UncompressedSize = 0;
			Attributes = dirInfo.Attributes;
		}
		protected override bool CanCloseAndDisposeStream { get { return true; } }
		protected override Stream ObtainContentStream() {
			return new MemoryStream();
		}
	}
	public class ZipFileItem : ZipItem {
		string fileName;
		public ZipFileItem(string name, string fileName)
			: base(name) {
			Guard.ArgumentNotNull(fileName, "fileName");
			FileName = fileName;
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipFileItemFileName")]
#endif
		public string FileName {
			get { return fileName; }
			private set {
				if (fileName == value)
					return;
				fileName = value;
				OnFileNameChanged();
			}
		}
		protected override bool CanCloseAndDisposeStream {
			get { return true; }
		}
		protected override Stream ObtainContentStream() {
			FileInfo fileInfo = new FileInfo(FileName);
			if (!fileInfo.Exists)
				return null;
			return fileInfo.OpenRead();
		}
		void OnFileNameChanged() {
			FileInfo fileInfo = new FileInfo(FileName);
			UncompressedSize = fileInfo.Length;
			Attributes = fileInfo.Attributes;
		}
	}
	public class ZipStreamItem : ZipItem {
		Stream contentStream;
		public ZipStreamItem(string name)
			: base(name) {
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipStreamItemContentStream")]
#endif
		public Stream ContentStream {
			get { return contentStream; }
			set {
				contentStream = value;
				if (contentStream != null)
					UncompressedSize = contentStream.Length;
				else
					UncompressedSize = 0;
				IsModified = true;
			}
		}
		protected override bool CanCloseAndDisposeStream {
			get { return false; }
		}
		protected override Stream ObtainContentStream() {
			return ContentStream;
		}
	}
	public class ZipByteArrayItem : ZipItem {
		byte[] content;
		public ZipByteArrayItem(string name, byte[] content)
			: base(name) {
			Content = content;
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipByteArrayItemContent")]
#endif
		public byte[] Content {
			get { return content; }
			set {
				if (content == value)
					return;
				content = value;
				OnContentChanged();
			}
		}
		protected override bool CanCloseAndDisposeStream { get { return true; } }
		void OnContentChanged() {
			UncompressedSize = Content.Length;
		}
		protected override Stream ObtainContentStream() {
			return new MemoryStream(Content);
		}
	}
	public class ZipTextItem : ZipItem {
		string content;
		public ZipTextItem(String name)
			: base(name) {
			Content = String.Empty;
			ContentEncoding = Encoding.UTF8;
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipTextItemContent")]
#endif
		public string Content {
			get { return content; }
			set {
				this.content = value;
				OnContentChanged();
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("ZipTextItemContentEncoding")]
#endif
		public Encoding ContentEncoding { get; set; }
		protected override bool CanCloseAndDisposeStream {
			get { return true; }
		}
		protected override Stream ObtainContentStream() {
			TextReadonlyStream stream = new TextReadonlyStream(Content, ContentEncoding);
			return stream;
		}
		void OnContentChanged() {
			UncompressedSize = Content.Length;
		}
	}
	public class TextReadonlyStream : Stream {
		readonly string text;
		readonly Encoding encoding;
		Stream innerStream;
		public TextReadonlyStream(string text, Encoding encoding) {
			this.text = text;
			this.encoding = encoding;
		}
		#region Properties
		Stream InnerStream {
			get {
				if (innerStream == null) {
					byte[] bytes = encoding.GetBytes(text);
					innerStream = new MemoryStream(bytes);
				}
				return innerStream;
			}
		}
		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return true; } }
		public override bool CanWrite { get { return false; } }
		public override long Length {
			get {
#if !SL
				if (innerStream == null && encoding.IsSingleByte)
					return text.Length;
#endif
				return InnerStream.Length;
			}
		}
		public override long Position {
			get {
				if (innerStream == null)
					return 0;
				else
					return InnerStream.Position;
			}
			set {
				if (Position == value)
					return;
				InnerStream.Position = value;
			}
		}
		#endregion
		public override void Flush() {
		}
		public override int Read(byte[] buffer, int offset, int count) {
			return InnerStream.Read(buffer, offset, count);
		}
		public override long Seek(long offset, SeekOrigin origin) {
			return InnerStream.Seek(offset, origin);
		}
		public override void SetLength(long value) {
			InnerStream.SetLength(value);
		}
		public override void Write(byte[] buffer, int offset, int count) {
		}
	}
}
