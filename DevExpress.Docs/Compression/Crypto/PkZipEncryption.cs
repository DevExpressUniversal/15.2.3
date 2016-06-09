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
#if !SL
using System.IO.Compression;
using DevExpress.Office.Utils;
#endif
namespace DevExpress.Compression.Internal {
	class PkZipCryptoEncoder {
		uint[] keys;
		public PkZipCryptoEncoder(string password) {
			this.keys = CreateInitialKeys();
			InitializeKeys(password);
		}
		uint[] CreateInitialKeys() {
			uint[] result = new uint[3];
			result[0] = 305419896;
			result[1] = 591751049;
			result[2] = 878082192;
			return result;
		}
		void InitializeKeys(string password) {
			byte[] passwordBytes = ByteUtils.GetPasswordBytes(password);
			int count = passwordBytes.Length;
			for (int i = 0; i < count; i++)
				UpdateKeys(passwordBytes[i]);
		}
		void UpdateKeys(byte value) {
			this.keys[0] = Crc32CheckSum.Calculate(this.keys[0], value);
			this.keys[1] = keys[1] + (keys[0] & 0xff);
			this.keys[1] = keys[1] * 134775813 + 1;
			this.keys[2] = Crc32CheckSum.Calculate(this.keys[2], (byte)(this.keys[1] >> 24));
		}
		byte CalculateDecryptByte() {
			uint temp = this.keys[2] | 2;
			return (byte)(((temp * (temp ^ 1)) >> 8) & 0xff);
		}
		public byte DecryptByte(byte value) {
			byte temp = (byte)((value ^ CalculateDecryptByte()) & 0xff);
			UpdateKeys(temp);
			return temp;
		}
		public byte EncryptByte(byte value) {
			byte temp = (byte)((value ^ CalculateDecryptByte()) & 0xff);
			UpdateKeys(value);
			return temp;
		}
	}
	abstract class PkZipCryptoStreamBase : SubStreamBase {
		PkZipCryptoEncoder cryptoEncoder;
		protected PkZipCryptoStreamBase(StreamProxy streamProxy, string password, byte checkByte)
			: base(streamProxy.BaseStream, streamProxy.StartPositionInBaseStream, streamProxy.Length, streamProxy.IsPackedStream) {
			IsValid = true;
			this.cryptoEncoder = new PkZipCryptoEncoder(password);
			IsValid = Prepare(checkByte);
		}
		protected bool IsValid { get; set; }
		protected abstract bool Prepare(byte checkByte);
		protected override int ReadCore(byte[] buffer, int offset, int count) {
			for (int i = 0; i < count; i++)
				buffer[offset + i] = this.cryptoEncoder.DecryptByte((byte)BaseStream.ReadByte());
			return count;
		}
		public override void WriteCore(byte[] buffer, int offset, int count) {
			for (int i = 0; i < count; i++) {
				byte codedByte = this.cryptoEncoder.EncryptByte(buffer[offset + i]);
				BaseStream.WriteByte(codedByte);
			}
		}
	}
	class PkZipCryptoReadOnlyStream : PkZipCryptoStreamBase {
		public PkZipCryptoReadOnlyStream(StreamProxy streamProxy, string password, byte checkByte)
			: base(streamProxy, password, checkByte) {
		}
		public override bool CanWrite { get { return false; } }
		public override bool CanRead { get { return base.CanRead && IsValid; } }
		protected override bool Prepare(byte checkByte) {
			if (Length < 12)
				return false;
			byte[] header = ReadHeader();
			byte lastHeaderByte = header[header.Length - 1];
			return lastHeaderByte == checkByte;
		}
		byte[] ReadHeader() {
			byte[] header = new byte[12];
			Read(header, 0, header.Length);
			return header;
		}
	}
	class PkZipCryptoWriteOnlyStream : PkZipCryptoStreamBase {
		public PkZipCryptoWriteOnlyStream(StreamProxy streamProxy, string password, byte checkByte)
			: base(streamProxy, password, checkByte) {
		}
		public override bool CanWrite { get { return base.CanWrite && IsValid; } }
		public override bool CanRead { get { return false; } }
		protected override bool Prepare(byte checkByte) {
			WriteHeader(checkByte);
			return true;
		}
		void WriteHeader(byte checkByte) {
			byte[] header = new byte[12];
			Random rnd = new Random();
			rnd.NextBytes(header);
			header[header.Length - 1] = checkByte;
			Write(header, 0, header.Length);
		}
	}
	public class PkZipEncryptionCompressionStrategy : ICompressionStrategy {
		int crc32 = 0;
		string password;
		public PkZipEncryptionCompressionStrategy(string password) {
			this.password = password;
		}
		public CompressionMethod CompressionMethod { get { return CompressionMethod.Deflate; } }
		public int Crc32 { get { return crc32; } }
		public void Compress(Stream sourceStream, Stream targetStream, IZipComplexOperationProgress progress) {
			long totalLength = sourceStream.Length - sourceStream.Position;
			ZipCopyStreamOperationProgress crc32CalculationProgress = new ZipCopyStreamOperationProgress(totalLength, 1);
			ZipCopyStreamOperationProgress compressionProgress = new ZipCopyStreamOperationProgress(totalLength, 10);
			if (progress != null) { 
				progress.AddOperationProgress(crc32CalculationProgress);
				progress.AddOperationProgress(compressionProgress);
			}
			Stream tempStream = sourceStream;
			Crc32Stream crc32Stream = null;
			if (!sourceStream.CanSeek) {
				tempStream = new MemoryStream();
				crc32Stream = new Crc32Stream(sourceStream);
				StreamUtils.CopyStream(crc32Stream, tempStream, crc32CalculationProgress.CopyHandler);
				tempStream.Seek(0, SeekOrigin.Begin);
			}
			else {
				long sourcePosition = sourceStream.Position;
				crc32Stream = new Crc32Stream(sourceStream);
				StreamUtils.MakeReadingPass(crc32Stream, crc32CalculationProgress.CopyHandler);
				sourceStream.Seek(sourcePosition, SeekOrigin.Begin);
			}
			this.crc32 = (int)crc32Stream.ReadCheckSum;
			byte checkByte = (byte)((Crc32 >> 24) & 0xff);
			PkZipCryptoWriteOnlyStream cryptoStream = new PkZipCryptoWriteOnlyStream(StreamProxy.Create(targetStream), this.password, checkByte);
			using (Stream compressStream = CreateDeflateStream(cryptoStream)) {
				StreamUtils.CopyStream(tempStream, compressStream, compressionProgress.CopyHandler);
			}
		}
		public short GetGeneralPurposeBitFlag() {
			return (short)ZipFlags.Encrypted;
		}
		public void PrepareExtraFields(IZipExtraFieldCollection extraFields) {
		}
		Stream CreateDeflateStream(Stream stream) {
			return new DeflateStream(stream, CompressionMode.Compress, true);
		}
	}
	public class PkZipEncryptionDecompressionStrategy : IDecompressionStrategy {
		string password;
		byte checkByte;
		CompressionMethod compressionMethod;
		public PkZipEncryptionDecompressionStrategy(string password, byte checkByte, CompressionMethod compressionMethod) {
			this.password = password;
			this.checkByte = checkByte;
			this.compressionMethod = compressionMethod;
		}
		public Stream Decompress(Stream stream) {
			if (String.IsNullOrEmpty(this.password))
				ZipExceptions.ThrowWrongPasswordException(String.Empty);
			Stream cryptoStream = new PkZipCryptoReadOnlyStream(StreamProxy.Create(stream), password, this.checkByte);
			if (!cryptoStream.CanRead)
				ZipExceptions.ThrowWrongPasswordException(password);
			if (this.compressionMethod == CompressionMethod.Deflate)
				return new DeflateStream(cryptoStream, CompressionMode.Decompress, true);
			return cryptoStream;
		}
	}
	public class PkZipEncryptionInfo : IEncryptionInfo {
		public PkZipEncryptionInfo(byte checkByte, CompressionMethod compressionMethod) {
			CheckByte = checkByte;
			CompressionMethod = compressionMethod;
		}
		public int Crc32 { get; set; }
		public EncryptionType Type { get { return EncryptionType.PkZip; } }
		public byte CheckByte { get; private set; }
		public CompressionMethod CompressionMethod { get; private set; }
		public IDecompressionStrategy CreateDecompressionStrategy(string password) {
			return new PkZipEncryptionDecompressionStrategy(password, CheckByte, CompressionMethod);
		}
	}
}
