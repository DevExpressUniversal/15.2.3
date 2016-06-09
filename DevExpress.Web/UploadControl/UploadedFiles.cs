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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class PostedFile {
		HttpPostedFile httpPostedFile = null;
		HelperPostedFileBase helperPostedFile = null;
		string fileNameInStorage;
		protected internal PostedFile(HttpPostedFile file) {
			this.httpPostedFile = file;
		}
		protected internal PostedFile(HelperPostedFileBase file) {
			this.helperPostedFile = file;
		}
		protected internal HttpPostedFile HttpPostedFile { get { return httpPostedFile; } }
		protected internal HelperPostedFileBase HelperPostedFile { get { return helperPostedFile; } }
#if !SL
	[DevExpressWebLocalizedDescription("PostedFileContentLength")]
#endif
		public long ContentLength {
			get {
				if(HttpPostedFile != null)
					return HttpPostedFile.ContentLength;
				else if(HelperPostedFile != null)
					return HelperPostedFile.FileSize;
				else 
					return 0;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("PostedFileContentType")]
#endif
		public string ContentType {
			get {
				if(HttpPostedFile != null)
					return HttpPostedFile.ContentType;
				else if(HelperPostedFile != null)
					return HelperPostedFile.ContentType;
				else 
					return "";
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("PostedFileFileName")]
#endif
		public string FileName {
			get {
				if(HttpPostedFile != null)
					return HttpPostedFile.FileName;
				else if(HelperPostedFile != null)
					return HelperPostedFile.FileName;
				else
					return "";
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("PostedFileInputStream")]
#endif
		public Stream InputStream {
			get {
				if(HttpPostedFile != null)
					return new PostedFileStream(HttpPostedFile.InputStream);
				else if(HelperPostedFile != null)
					return HelperPostedFile.GetInputStreamInternal();
				else
					return null;
			}
		}
		public string FileNameInStorage {
			get {
				if(string.IsNullOrEmpty(this.fileNameInStorage))
					this.fileNameInStorage = GetFileNameInStorageInternal();
				return this.fileNameInStorage;
			}
			protected internal set {
				this.fileNameInStorage = value;
			}
		}
		public void SaveAs(string fileName) {
			SaveAs(fileName, true);
		}
		protected internal void SaveAs(string fileName, bool allowOverwrite) {
			if(HttpPostedFile != null)
				HttpPostedFile.SaveAs(fileName);
			else if(HelperPostedFile != null)
				HelperPostedFile.SaveAs(fileName, allowOverwrite);
		}
		protected internal bool IsStandardMode() {
			return HttpPostedFile != null;
		}
		protected internal byte[] GetFileBytes() {
			byte[] ret = null;
			if(HttpPostedFile != null)
				ret = CommonUtils.GetBytesFromStream(HttpPostedFile.InputStream);
			else if(HelperPostedFile != null) {
				using(Stream stream = HelperPostedFile.GetInputStreamInternal()) {
					ret = CommonUtils.GetBytesFromStream(stream);
				}
			}
			return ret;
		}
		string GetFileNameInStorageInternal() {
			if(HttpPostedFile != null)
				return HttpPostedFile.FileName;
			else if(HelperPostedFile != null)
				return HelperPostedFile.InternalFileName;
			else
				return string.Empty;
		}
		class PostedFileStream : Stream {
			Stream stream;
			public PostedFileStream(Stream stream) {
				SourceStream = stream;
				SourceStream.Position = 0;
			}
			Stream SourceStream {
				get { return this.stream; }
				set { this.stream = value; }
			}
			protected override void Dispose(bool disposing) {
				SourceStream = null;
			}
			public override bool CanRead { 
				get { return SourceStream.CanRead; } 
			}
			public override bool CanSeek { 
				get { return SourceStream.CanSeek; } 
			}
			public override bool CanWrite { 
				get { return SourceStream.CanWrite; } 
			}
			public override void Flush() { 
				SourceStream.Flush(); 
			}
			public override long Length { 
				get { return SourceStream.Length; } 
			}
			public override long Position {
				get { return SourceStream.Position; }
				set { SourceStream.Position = value; }
			}
			public override int Read(byte[] buffer, int offset, int count) {
				return SourceStream.Read(buffer, offset, count);
			}
			public override long Seek(long offset, SeekOrigin origin) {
				return SourceStream.Seek(offset, origin);
			}
			public override void SetLength(long value) {
				SourceStream.SetLength(value);
			}
			public override void Write(byte[] buffer, int offset, int count) {
				SourceStream.Write(buffer, offset, count);
			}
		}
	}
	public class UploadedFile {
		private PostedFile postedFile = null;
		private byte[] fileBytes = null;
		private bool isValid = true;
		public UploadedFile()
			: base() {
		}
		public UploadedFile(HttpPostedFile file) {
			this.postedFile = new PostedFile(file);
		}
		public UploadedFile(HelperPostedFileBase file) {
			this.postedFile = new PostedFile(file);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadedFilePostedFile"),
#endif
		Obsolete("Use members available at the UploadedFile object level instead.")]
		public PostedFile PostedFile { get { return PostedFileInternal; } }
#if !SL
	[DevExpressWebLocalizedDescription("UploadedFileContentType")]
#endif
		public string ContentType { get { return PostedFileInternal.ContentType; } }
#if !SL
	[DevExpressWebLocalizedDescription("UploadedFileContentLength")]
#endif
		public long ContentLength { get { return PostedFileInternal.ContentLength; } }
#if !SL
	[DevExpressWebLocalizedDescription("UploadedFileFileContent")]
#endif
		public Stream FileContent { get { return PostedFileInternal.InputStream; } }
#if !SL
	[DevExpressWebLocalizedDescription("UploadedFileFileName")]
#endif
		public string FileName { get { return Path.GetFileName(PostedFileInternal.FileName); } }
		public string FileNameInStorage { 
			get { 
				return PostedFileInternal.FileNameInStorage;
			}
			protected internal set {
				PostedFileInternal.FileNameInStorage = value;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("UploadedFileFileBytes")]
#endif
		public byte[] FileBytes {
			get {
				if(fileBytes == null)
					fileBytes = PostedFileInternal.GetFileBytes();
				return fileBytes;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("UploadedFileIsValid")]
#endif
		public bool IsValid {
			get { return isValid; }
			set { isValid = value; }
		}
		protected internal PostedFile PostedFileInternal { get { return this.postedFile; } }
		public void SaveAs(string fileName) {
			SaveAs(fileName, true);
		}
		public void SaveAs(string fileName, bool allowOverwrite) {
			if(!PostedFileInternal.IsStandardMode())
				PostedFileInternal.SaveAs(fileName, allowOverwrite);
			else { 
				HttpRuntimeSection runtimeSection = null;
				if(WebConfigurationManager.GetSection("HttpRuntime") != null)
					runtimeSection = WebConfigurationManager.GetSection("HttpRuntime") as HttpRuntimeSection;
				if(!Path.IsPathRooted(fileName) && (runtimeSection != null) &&
					runtimeSection.RequireRootedSaveAsPath)
					throw new HttpException(string.Format(StringResources.UploadControl_SaveAsRequires, fileName));
				FileStream stream = new FileStream(fileName, allowOverwrite ? FileMode.Create : FileMode.CreateNew);
				try {
					stream.Write(FileBytes, 0, FileBytes.Length);
					stream.Flush();
				} finally {
					stream.Close();
				}
			}
		}
		protected internal void CommitChunkedUpload() {
			PostedFileInternal.HelperPostedFile.CommitChunkedUpload();
		}
		protected internal void AbortChunkedUpload() {
			PostedFileInternal.HelperPostedFile.AbortChunkedUpload();
		}
		protected internal void SetIsValid(bool value) {
			this.isValid = value;
		}
		protected internal bool IsEmpty() {
			return PostedFileInternal == null || (PostedFileInternal.HttpPostedFile == null && 
				PostedFileInternal.HelperPostedFile == null) || string.IsNullOrEmpty(FileName);
		}
	}
}
