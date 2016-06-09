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
using DevExpress.Utils.Zip;
using System.Collections.Generic;
using DevExpress.Utils.Zip.Internal;
using System.Text;
#if !SILVERLIGHT
using System.IO.Compression;
#endif
namespace DevExpress.Compression.Internal {
	class ZipArchiver : InternalZipArchiveCore {
		public ZipArchiver(Stream stream)
			: base(stream) {
			CanContinue = true;
			CurrentEncoding = ZipItem.GetDefaultEncoding();
		}
		#region Properties
		public bool CanContinue { get; private set; }
		Encoding CurrentEncoding { get; set; }
		#endregion
		#region Events
		#region Error
		public event ErrorEventHandler Error;
		protected internal virtual bool RaiseError(Exception e, string itemName) {
			if (Error == null)
				return false;
			ErrorEventArgs args = new ErrorEventArgs(e, itemName);
			Error(this, args);
			return args.CanContinue;
		}
		#endregion
		#endregion
		public void WriteFile(ZipItem zipItem, IZipComplexOperationProgress progress) {
			CentralDirectoryEntry entry = null;
			IContentStreamOwner contenStreamSupport = zipItem as IContentStreamOwner;
			Stream contentStream = null;
			bool isError = false;
			long position = ZipStream.Position;
			Encoding oldEncoding = CurrentEncoding;
			bool isDirectory = zipItem is ZipDirectoryItem;
			try {
				CurrentEncoding = zipItem.Encoding;
				contentStream = contenStreamSupport.ObtainContentStream();
				ICompressionStrategy compressionStrategy = CompressionStrategyFactory.Create(zipItem.Password, zipItem.EncryptionType, isDirectory);
				entry = WriteFile(zipItem.Name, zipItem.LastWriteTime, contentStream, compressionStrategy, progress);
				entry.FileAttributes = (int)zipItem.Attributes;
			} catch (Exception e) {
				CurrentEncoding = oldEncoding;
				isError = true;
				CanContinue = RaiseError(e, zipItem.Name);
			} finally {
				if (!isError) {
					zipItem.IsModified = false;
					if (contenStreamSupport.CanCloseAndDisposeStream) {
						contentStream.Dispose();
						contentStream.Dispose();
					}
				}
				bool isStoped = (progress != null && progress.IsStopped);
				if (isError || isStoped) {
					if (entry != null && CentralDirectory.Contains(entry))
						CentralDirectory.Remove(entry);
					if (ZipStream.CanSeek)
						ZipStream.Seek(position, SeekOrigin.Begin);
					entry = null;
				}
			}
			if (entry == null)
				return;
			ZipExtraFieldNTFS ntfsField = CreateNTFSExtraField(zipItem);
			entry.ExtraFields.Add(ntfsField);
			entry.Comment = zipItem.Comment;
			entry.FileAttributes = (int)zipItem.Attributes;
		}
		protected override IZipExtraFieldCollection CreateExtraFieldCollection() {
			return new ZipExtraFieldComposition();
		}
		ZipExtraFieldNTFS CreateNTFSExtraField(ZipItem zipItem) {
			ZipExtraFieldNTFS ntfsField = new ZipExtraFieldNTFS();
			ntfsField.CreationTime = zipItem.CreationTime;
			ntfsField.LastModificationTime = zipItem.LastWriteTime;
			ntfsField.LastAccessTime = zipItem.LastAccessTime;
			return ntfsField;
		}
		protected override Encoding GetDefaultEncoding() {
			return CurrentEncoding;
		}
	}
	public class InternalZipFilExCollection : List<InternalZipFileEx> {
	}
	public class ZipFileParserEx : InternalZipFileParserCore<InternalZipFileEx> {
		public InternalZipFilExCollection Records { get { return (InternalZipFilExCollection)InnerRecords; } }
		protected override void PorcessFileEntryRecord(BinaryReader reader, Encoding fileNameEncoding) {
			InternalZipFileEx zipFile = CreateZipFileInstance();
			zipFile.DefaultEncoding = fileNameEncoding;
			zipFile.ReadCentralDirectoryFileHeader(reader);
			InternalZipFileEx sourceZipFile = FindRecordByName(zipFile.FileName) as InternalZipFileEx;
			sourceZipFile.ApplyCentralDirectoryFileHeader(zipFile);
		}
		protected override IList<InternalZipFileEx> CreateRecords() {
			return new InternalZipFilExCollection();
		}
	}
	public class InternalZipFileEx : InternalZipFile {
		public string Comment { get; set; }
		public DateTime FileLastAccessTime { get; set; }
		public DateTime FileCreationTime { get; set; }
		public IEncryptionInfo EncryptionInfo { get; set; }
		protected Int16 VersionMadeBy { get; set; }
		public Int32 ExternalAttribute { get; set; }
		protected override void ReadLocalHeader(BinaryReader reader) {
			base.ReadLocalHeader(reader);
			if (IsEncrypted && EncryptionInfo == null)
				EncryptionInfo = new PkZipEncryptionInfo(CheckByte, CompressionMethod);
			if (EncryptionInfo != null)
				EncryptionInfo.Crc32 = Crc32;
			FileLastAccessTime = FileCreationTime = FileLastModificationTime;
		}
		protected override InternalZipExtraFieldFactory CreateInternalZipExtraFieldFactory() {
			return ZipExtraFieldFactoryInstance.Instance;
		}
		public void ApplyCentralDirectoryFileHeader(InternalZipFileEx otherZipFile) {
			if (otherZipFile.GeneralPurposeBitFlag != GeneralPurposeBitFlag)
				ZipExceptions.ThrowBadArchiveException();
			VersionMadeBy = otherZipFile.VersionMadeBy;
			if (CompressionMethod != otherZipFile.CompressionMethod)
				ZipExceptions.ThrowBadArchiveException();
			FileLastModificationTime = otherZipFile.FileLastModificationTime;
			FileLastAccessTime = otherZipFile.FileLastAccessTime;
			FileCreationTime = otherZipFile.FileCreationTime;
			if (Crc32 != otherZipFile.Crc32)
				ZipExceptions.ThrowBadArchiveException();
			if (CompressedSize != otherZipFile.CompressedSize)
				ZipExceptions.ThrowBadArchiveException();
			if (UncompressedSize != otherZipFile.UncompressedSize)
				ZipExceptions.ThrowBadArchiveException();
			if (FileNameLength != otherZipFile.FileNameLength)
				ZipExceptions.ThrowBadArchiveException();
			Comment = otherZipFile.Comment;
			ExternalAttribute = otherZipFile.ExternalAttribute;
		}
		public void ReadCentralDirectoryFileHeader(BinaryReader reader) {
			VersionMadeBy = reader.ReadInt16();
			VersionToExtract = reader.ReadInt16();
			GeneralPurposeBitFlag = (ZipFlags)reader.ReadInt16();
			CompressionMethod = (CompressionMethod)reader.ReadInt16();
			FileCreationTime = FileLastAccessTime = FileLastModificationTime = ZipDateTimeHelper.FromMsDos(reader.ReadInt32());
			Crc32 = reader.ReadInt32();
			CompressedSize = reader.ReadUInt32();
			UncompressedSize = reader.ReadUInt32();
			FileNameLength = reader.ReadInt16();
			Int16 centralDirectoryExtraFieldLength = reader.ReadInt16();
			Int16 commentLength = reader.ReadInt16();
			reader.ReadInt16();
			reader.ReadInt16();
			ExternalAttribute = reader.ReadInt32();
			reader.ReadInt32();
			FileName = ReadString(reader, FileNameLength);
			ZipExtraFieldComposition extraFields = ZipExtraFieldComposition.Read(reader, centralDirectoryExtraFieldLength, ZipExtraFieldFactoryInstance.Instance);
			extraFields.Apply(this);
			Comment = ReadString(reader, commentLength);
		}
		public Stream CreateDecompressionStream(string password) {
			if (String.IsNullOrEmpty(password))
				ZipExceptions.ThrowWrongPasswordException(String.Empty);
			if (!IsEncrypted)
				return CreateDecompressionStream();
			if (EncryptionInfo == null)
				ZipExceptions.ThrowBadArchiveException();
			IDecompressionStrategy decompressionStrategy = EncryptionInfo.CreateDecompressionStrategy(password);
			return decompressionStrategy.Decompress(ContentRawDataStreamProxy.CreateRawStream());
		}
		public override Stream CreateDecompressionStream() {
			if (IsEncrypted)
				ZipExceptions.ThrowWrongPasswordException(String.Empty);
			return base.CreateDecompressionStream();
		}
	}
	public static class CompressionStrategyFactory {
		public static ICompressionStrategy Create(string password, EncryptionType encryptionType, bool isDirectory) {
			if (isDirectory)
				return new StoreCompressionStrategy();
			if (String.IsNullOrEmpty(password) || encryptionType == EncryptionType.None)
				return new DeflateCompressionStrategy();
			if (encryptionType == EncryptionType.PkZip)
				return new PkZipEncryptionCompressionStrategy(password);
			return new AesCompressionStrategy(password, ConvertToEncryptionStrength(encryptionType));
		}
		static AesEncryptionStrength ConvertToEncryptionStrength(EncryptionType encryptionType) {
			switch (encryptionType) {
				case EncryptionType.Aes128:
					return AesEncryptionStrength.Bit128;
				case EncryptionType.Aes192:
					return AesEncryptionStrength.Bit192;
				case EncryptionType.Aes256:
					return AesEncryptionStrength.Bit256;
			}
			return AesEncryptionStrength.Bit128;
		}
	}
}
