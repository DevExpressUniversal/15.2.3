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
using System.Security.Cryptography;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Utils.Zip.Internal;
#if !SL
using System.IO.Compression;
#endif
namespace DevExpress.Compression.Internal {
	public enum AesVendorVersion { AE1 = 1, AE2 = 2 }
	public enum AesEncryptionStrength { Bit128 = 128, Bit192 = 192, Bit256 = 256 }
	public class ZipExtraFieldAes : ZipExtraField {
		public const short HeaderId = -26367;
		byte[] VendorIdBytes = new byte[] { 0x41, 0x45 }; 
		public ZipExtraFieldAes() {
			VendorVersion = AesVendorVersion.AE1;
			CompressionMethod = CompressionMethod.Deflate;
		}
		public override short Id { get { return HeaderId; } }
		public override short ContentSize { get { return 7; } }
		public AesVendorVersion VendorVersion { get; set; }
		public CompressionMethod CompressionMethod { get; set; }
		public AesEncryptionStrength EncryptionStrength { get; set; }
		public override ExtraFieldType Type { get { return ExtraFieldType.Both; } }
		public override void AssignRawData(BinaryReader reader) {
			VendorVersion = (AesVendorVersion)reader.ReadInt16();
			byte[] vendorBytes = reader.ReadBytes(2);
			if (!ByteUtils.CompareBytes(vendorBytes, VendorIdBytes))
				ZipExceptions.ThrowBadArchiveException();
			EncryptionStrength = ConvertToEncryptionStrength(reader.ReadByte());
			CompressionMethod = (CompressionMethod)reader.ReadInt16();
		}
		public override void Apply(InternalZipFile zipFile) {
			InternalZipFileEx zipFileEx = zipFile as InternalZipFileEx;
			if (zipFileEx == null)
				return;
			zipFileEx.EncryptionInfo = new AesEncryptionInfo(VendorVersion, EncryptionStrength, CompressionMethod);
		}
		public override void Write(System.IO.BinaryWriter writer) {
			writer.Write((short)VendorVersion);
			writer.Write(VendorIdBytes);
			writer.Write(ConvertToEncryptionStrengthByte(EncryptionStrength));
			writer.Write((short)CompressionMethod);
		}
		AesEncryptionStrength ConvertToEncryptionStrength(byte value) {
			switch (value) {
				case 1:
					return AesEncryptionStrength.Bit128;
				case 2:
					return AesEncryptionStrength.Bit192;
				case 3:
					return AesEncryptionStrength.Bit256;
			}
			return AesEncryptionStrength.Bit128;
		}
		byte ConvertToEncryptionStrengthByte(AesEncryptionStrength encryptionStrength) {
			switch (encryptionStrength) {
				case AesEncryptionStrength.Bit128:
					return 1;
				case AesEncryptionStrength.Bit192:
					return 2;
				case AesEncryptionStrength.Bit256:
					return 3;
			}
			return 1;
		}
	}
	public class AesEncryptionInfo : IEncryptionInfo {
		public AesEncryptionInfo(AesVendorVersion vendorVersion, AesEncryptionStrength encryptionStrength, CompressionMethod compressionMethod) {
			VendorVersion = vendorVersion;
			EncryptionStrength = encryptionStrength;
			CompressionMethod = compressionMethod;
			switch (encryptionStrength) {
				case AesEncryptionStrength.Bit128:
					Type = EncryptionType.Aes128;
					break;
				case AesEncryptionStrength.Bit192:
					Type = EncryptionType.Aes192;
					break;
				case AesEncryptionStrength.Bit256:
					Type = EncryptionType.Aes256;
					break;
			}
		}
		public int Crc32 { get; set; }
		public CompressionMethod CompressionMethod { get; private set; }
		public AesVendorVersion VendorVersion { get; private set; }
		protected AesEncryptionStrength EncryptionStrength { get; set; }
		public EncryptionType Type {
			get;
			private set;
		}
		public IDecompressionStrategy CreateDecompressionStrategy(string password) {
			return new AesDecompressionStrategy(password, EncryptionStrength, CompressionMethod, VendorVersion, Crc32);
		}
	}
	public class AesAlgorithmSettings {
		string password;
		CompressionMethod compressionMethod;
		int keyBitCount;
		int keyBytes;
		public AesAlgorithmSettings(string password, AesEncryptionStrength encryptionStrength, CompressionMethod compressionMethod) {
			this.password = password;
			this.keyBitCount = (int)encryptionStrength;
			this.keyBytes = this.keyBitCount / 8;
			System.Diagnostics.Debug.Assert(this.keyBitCount == 128 || this.keyBitCount == 192 || this.keyBitCount == 256);
			this.compressionMethod = compressionMethod;
		}
		public string Password { get { return password; } }
		public CompressionMethod CompressionMethod { get { return compressionMethod; } }
		public ICryptoTransform CryptoTransform { get; private set; }
		public HMACSHA1 AuthenticationKeyCalculator { get; private set; }
		public byte[] EncryptionKey { get; private set; }
		public byte[] AuthenticationCode { get; private set; }
		public byte[] VerificationValue { get; private set; }
		public byte[] Salt { get; private set; }
		public int KeyBitCount { get { return keyBitCount; } }
		public AesAlgorithmSettings Clone() {
			return new AesAlgorithmSettings(this.password, (AesEncryptionStrength)this.keyBitCount, this.compressionMethod);
		}
		public void Init() {
			Salt = new byte[keyBitCount / 16];
			Random rnd = new Random();
			rnd.NextBytes(Salt);
			InitCore();
		}
		public void Init(AesEncryptionHeader header) {
			Salt = header.Salt;
			InitCore();
		}
		void InitCore() {
			GenerateEncryptionKeys(Salt);
			CryptoTransform = CreateAesCryptoTransform(this.password, Salt, EncryptionKey);
			AuthenticationKeyCalculator = new HMACSHA1(AuthenticationCode);
		}
		void GenerateEncryptionKeys(byte[] saltBuffer) {
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(ByteUtils.GetPasswordBytes(this.password), saltBuffer, 1000);
			EncryptionKey = rfc2898DeriveBytes.GetBytes(keyBytes);
			AuthenticationCode = rfc2898DeriveBytes.GetBytes(keyBytes);
			VerificationValue = rfc2898DeriveBytes.GetBytes(2);
		}
		#region CreateAesCryptoTransform
		ICryptoTransform CreateAesCryptoTransform(string password, byte[] salt, byte[] key) {
#if SL
			Aes aes = new AesManaged();
#else
			Aes aes = Aes.Create();
			aes.Mode = CipherMode.ECB;
			aes.Padding = PaddingMode.None;
#endif
			aes.KeySize = this.keyBitCount;
			return aes.CreateEncryptor(key, new byte[16]);
		}
		#endregion
	}
	public class AesEncryptionHeader {
		public static AesEncryptionHeader Read(Stream stream, int keyBitCount) {
			AesEncryptionHeader header = new AesEncryptionHeader(keyBitCount);
			stream.Read(header.Salt, 0, header.Salt.Length);
			stream.Read(header.VerificationValue, 0, 2);
			return header;
		}
		public AesEncryptionHeader(int keyBitCount) {
			Salt = new byte[keyBitCount / 16];
			VerificationValue = new byte[2];
		}
		public byte[] Salt { get; set; }
		public byte[] VerificationValue { get; set; }
		public int Length { get { return Salt.Length + VerificationValue.Length; } }
		public void Write(Stream stream) {
			stream.Write(Salt, 0, Salt.Length);
			stream.Write(VerificationValue, 0, 2);
		}
	}
	public class AesDecompressionStrategy : IDecompressionStrategy {
		int keyBitCount;
		AesAlgorithmSettings aesSettings;
		AesVendorVersion vendorVersion;
		int crc32;
		public AesDecompressionStrategy(string password, AesEncryptionStrength encryptionStrength, CompressionMethod compressionMethod, AesVendorVersion vendorVersion, int crc32) {
			this.keyBitCount = (int)encryptionStrength;
			System.Diagnostics.Debug.Assert(this.keyBitCount == 128 || this.keyBitCount == 192 || this.keyBitCount == 256);
			this.aesSettings = new AesAlgorithmSettings(password, encryptionStrength, compressionMethod);
			this.vendorVersion = vendorVersion;
			this.crc32 = crc32;
		}
		public Stream Decompress(Stream stream) {
			AesDecompressionReadOnlyStream resultStream = new AesDecompressionReadOnlyStream(stream, this.aesSettings, this.vendorVersion, this.crc32);
			return resultStream;			
		}
	}
	public class AesDecompressionReadOnlyStream : Stream {
		AesCrtCryptoStream aesCrtCryptoStream;
		Stream streamToRead;
		Stream baseStream;
		AesAlgorithmSettings settings;
		bool isCompleted = false;
		int crc32;
		uint checkSum;
		AesVendorVersion vendorVersion;
		public AesDecompressionReadOnlyStream(Stream baseStream, AesAlgorithmSettings settings, AesVendorVersion vendorVersion, int crc32) {
			this.baseStream = baseStream;
			this.settings = settings;
			this.vendorVersion = vendorVersion;
			this.crc32 = crc32;
			AesEncryptionHeader header = AesEncryptionHeader.Read(baseStream, this.settings.KeyBitCount);
			settings.Init(header);
			if (vendorVersion == AesVendorVersion.AE1)  
				this.checkSum = Crc32CheckSumCalculator.Instance.InitialCheckSumValue;
			if (!ByteUtils.CompareBytes(header.VerificationValue, settings.VerificationValue))
				ZipExceptions.ThrowWrongPasswordException(settings.Password);
			long encryptedDataLength = baseStream.Length - (header.Length + 10);
			this.aesCrtCryptoStream = new AesCrtCryptoStream(this.baseStream, encryptedDataLength, settings.CryptoTransform, settings.AuthenticationKeyCalculator);
			this.streamToRead = this.aesCrtCryptoStream;
			if (settings.CompressionMethod == CompressionMethod.Deflate)
				this.streamToRead = new DeflateStream(this.streamToRead, CompressionMode.Decompress);
		}
		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return false; } }
		public override void Flush() {
			throw new NotSupportedException();
		}
		public override long Length {
			get { throw new NotSupportedException(); }
		}
		public override long Position {
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}
		public override int Read(byte[] buffer, int offset, int count) {
			if (this.isCompleted)
				return 0;
			int byteCount = this.streamToRead.Read(buffer, offset, count);
			if (this.vendorVersion == AesVendorVersion.AE1)  
				this.checkSum = Crc32CheckSumCalculator.Instance.UpdateCheckSum(this.checkSum, buffer, offset, byteCount);
			if (byteCount != count) {
				this.isCompleted = true;
				VerifyStream();
			}
			return byteCount;
		}
		void VerifyStream() {
			byte[] messageAuthenticationCode = new byte[10];
			this.baseStream.Read(messageAuthenticationCode, 0, 10);
			if (this.aesCrtCryptoStream.Length == 0)
				this.settings.AuthenticationKeyCalculator.ComputeHash(new byte[] { });
			if (!ByteUtils.CompareFirstBytes(messageAuthenticationCode, this.settings.AuthenticationKeyCalculator.Hash, 10))
				ZipExceptions.ThrowWrongPasswordException(this.settings.Password);
			if (this.vendorVersion == AesVendorVersion.AE1 && this.crc32 != (int)Crc32CheckSumCalculator.Instance.GetFinalCheckSum(this.checkSum))
				ZipExceptions.ThrowBadArchiveException();
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
	public class AesCompressionStrategy : ICompressionStrategy {
		AesAlgorithmSettings aesSettings;
		int crc32;
		public AesCompressionStrategy(string password, AesEncryptionStrength encryptionStrength) {
			this.aesSettings = new AesAlgorithmSettings(password, encryptionStrength, CompressionMethod.Deflate);
		}
		public CompressionMethod CompressionMethod { get { return CompressionMethod.AESEncryption; } }
		public int Crc32 { get { return crc32; } }
		public void Compress(Stream sourceStream, Stream targetStream, IZipComplexOperationProgress progress) {
			this.aesSettings.Init();
			AesEncryptionHeader header = new AesEncryptionHeader(this.aesSettings.KeyBitCount);
			header.Salt = this.aesSettings.Salt;
			header.VerificationValue = this.aesSettings.VerificationValue;
			header.Write(targetStream);
			ZipCopyStreamOperationProgress copyProgress = new ZipCopyStreamOperationProgress(sourceStream.Length);
			if (progress != null)
				progress.AddOperationProgress(copyProgress);
			AesCrtCryptoStream cryptoStream = new AesCrtCryptoStream(targetStream, 0, this.aesSettings.CryptoTransform, this.aesSettings.AuthenticationKeyCalculator);
			if (sourceStream.Length == 0)
				StreamUtils.CopyStream(sourceStream, cryptoStream);
			else {
				Crc32Stream crc32Stream = new Crc32Stream(sourceStream);
				using (DeflateStream deflateStream = new DeflateStream(cryptoStream, CompressionMode.Compress, true)) {
					StreamUtils.CopyStream(crc32Stream, deflateStream, copyProgress.CopyHandler);
				}
				crc32 = (int)crc32Stream.ReadCheckSum;
			}
			if (copyProgress.IsStopped)
				return;
			cryptoStream.FinalizeStream();
			targetStream.Write(this.aesSettings.AuthenticationKeyCalculator.Hash, 0, 10);
		}
		public short GetGeneralPurposeBitFlag() {
			return (short)ZipFlags.Encrypted;
		}
		public void PrepareExtraFields(IZipExtraFieldCollection extraFields) {
			ZipExtraFieldAes field = new ZipExtraFieldAes();
			field.EncryptionStrength = (AesEncryptionStrength)this.aesSettings.KeyBitCount;
			extraFields.Add(field);
		}
	}
	public class AesCrtCryptoStream : Stream {
		internal const int BlockSize = 16;
		Stream baseStream;
		long length;
		ICryptoTransform aesBlockTransform;
		ICryptoTransform authenticationKeyCalculator;
		byte[] counterBuf = new byte[BlockSize];
		byte[] prevBuf = new byte[BlockSize];
		int prevBufSize = 0;
		long position;
		public AesCrtCryptoStream(Stream baseStream, long length, ICryptoTransform aesBlockTransform, ICryptoTransform authenticationKeyCalculator) {
			Guard.ArgumentNotNull(baseStream, "baseStream");
			Guard.ArgumentNotNull(aesBlockTransform, "transform");
			Guard.ArgumentNotNull(authenticationKeyCalculator, "mac");
			this.baseStream = baseStream;
			this.length = length;
			this.aesBlockTransform = aesBlockTransform;
			this.authenticationKeyCalculator = authenticationKeyCalculator;
			this.position = 0;
		}
		#region Properties
		public ICryptoTransform AesBlockTransform { get { return aesBlockTransform; } }
		public ICryptoTransform AuthenticationKeyCalculator { get { return authenticationKeyCalculator; } }
		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return true; } }
		public override long Length { get { return length; } }
		public override long Position {
			get { return position; }
			set {
				throw new NotSupportedException();
			}
		}
		#endregion
		public override int Read(byte[] buffer, int offset, int count) {
			System.Diagnostics.Debug.Assert(count > 0);
			if (this.position >= Length)
				return 0;
			int prevBufSizeReaded = 0;
			if (this.prevBufSize > 0) {
				prevBufSizeReaded = Math.Min(count, this.prevBufSize);
				Buffer.BlockCopy(this.prevBuf, 0, buffer, offset, prevBufSizeReaded);
				offset += prevBufSizeReaded;
				count -= prevBufSizeReaded;
				this.position += prevBufSizeReaded;
				this.prevBufSize -= prevBufSizeReaded;
				Buffer.BlockCopy(this.prevBuf, prevBufSizeReaded, this.prevBuf, 0, this.prevBufSize);
				if (count <= 0)
					return prevBufSizeReaded;
			}
			return prevBufSizeReaded + ReadCore(buffer, offset, count);
		}
		int ReadCore(byte[] buffer, int offset, int count) {
			long nextPosition = this.position + count;
			int actualCount = (nextPosition > Length) ? (int)Math.Max(Length - this.position, 0) : count;
			int completeBlockCount = actualCount / BlockSize;
			int reminderBlockSize = actualCount % BlockSize;
			bool canFinalizeCompleteBlock = (nextPosition >= Length && reminderBlockSize == 0);
			if (canFinalizeCompleteBlock) {
				completeBlockCount--;
				reminderBlockSize = BlockSize;
			}
			for (int i = 0; i < completeBlockCount; i++) {
				int currentOffset = offset + i * BlockSize;
				ReadBlock(buffer, currentOffset);
			}
			long nextAlignedPosition = this.position + (long)(BlockSize * Math.Ceiling((1.0 * actualCount) / BlockSize));
			if (nextAlignedPosition != nextPosition && nextAlignedPosition < Length && actualCount == count) {
				ReadBlock(this.prevBuf, 0);
				this.prevBufSize = (int)(nextAlignedPosition - nextPosition);
				int prevBufOffset = BlockSize - this.prevBufSize;
				int currentOffset = offset + completeBlockCount * BlockSize;
				Buffer.BlockCopy(this.prevBuf, 0, buffer, currentOffset, prevBufOffset);
				Buffer.BlockCopy(this.prevBuf, prevBufOffset, this.prevBuf, 0, this.prevBufSize);
			} else if (reminderBlockSize != 0) {
				int currentOffset = offset + completeBlockCount * BlockSize;
				ReadFinalBlock(buffer, currentOffset, reminderBlockSize);
			}
			this.position += actualCount;
			return actualCount;
		}
		void ReadBlock(byte[] buffer, int offset) {
			IncrementBytes(this.counterBuf);
			this.baseStream.Read(buffer, offset, BlockSize);
			this.authenticationKeyCalculator.TransformBlock(buffer, offset, BlockSize, null, 0);
			byte[] encodedCounterBuffer = new byte[BlockSize];
			this.aesBlockTransform.TransformBlock(counterBuf, 0, BlockSize, encodedCounterBuffer, 0);
			XorBytes(buffer, offset, encodedCounterBuffer);
		}
		void ReadFinalBlock(byte[] buffer, int offset, int reminderBlockSize) {
			IncrementBytes(this.counterBuf);
			this.baseStream.Read(buffer, offset, reminderBlockSize);
			this.authenticationKeyCalculator.TransformFinalBlock(buffer, offset, reminderBlockSize);
			byte[] encodedCounterBuffer = new byte[BlockSize];
			encodedCounterBuffer = this.aesBlockTransform.TransformFinalBlock(counterBuf, 0, BlockSize);
			XorBytes(buffer, offset, encodedCounterBuffer);
		}
		public override void Write(byte[] buffer, int offset, int count) {
			int completedByteCount = CompletePrevBuffer(buffer, offset, count);
			offset += completedByteCount;
			count -= completedByteCount;
			if (count <= 0) {
				this.position += completedByteCount;
				return;
			}
			WritePrevBuf();
			int completeBlockCount = count / BlockSize;
			int reminderBlockSize = count % BlockSize;
			if (reminderBlockSize == 0 && completeBlockCount > 0) {
				completeBlockCount--;
				reminderBlockSize = BlockSize;
			}
			byte[] encodedCounterBuffer = new byte[BlockSize];
			byte[] tempDataBuffer = new byte[BlockSize];
			for (int i = 0; i < completeBlockCount; i++) {
				IncrementBytes(this.counterBuf);
				int currentOffset = offset + i * BlockSize;
				Buffer.BlockCopy(buffer, currentOffset, tempDataBuffer, 0, BlockSize);
				this.aesBlockTransform.TransformBlock(this.counterBuf, 0, BlockSize, encodedCounterBuffer, 0);
				XorBytes(tempDataBuffer, 0, encodedCounterBuffer);
				this.authenticationKeyCalculator.TransformBlock(tempDataBuffer, 0, BlockSize, null, 0);
				this.baseStream.Write(tempDataBuffer, 0, BlockSize);
			}
			this.prevBufSize = reminderBlockSize;
			Array.Clear(this.prevBuf, 0, this.prevBuf.Length);
			int lastBlockOffset = (completeBlockCount < 0) ? 0 : offset + completeBlockCount * BlockSize;
			Buffer.BlockCopy(buffer, lastBlockOffset, this.prevBuf, 0, this.prevBufSize);
			this.position += count;
		}
		void XorBytes(byte[] target, int targetOffset, byte[] pattern) {
			int count = pattern.Length;
			count = Math.Min(count, target.Length - targetOffset);
			for (int i = 0; i < count; i++) {
				int dataIndex = i + targetOffset;
				target[dataIndex] = (byte)(target[dataIndex] ^ pattern[i]);
			}
		}
		void IncrementBytes(byte[] counterBuf) {
			int count = counterBuf.Length;
			for (int i = 0; i < count; i++) {
				counterBuf[i]++;
				bool hasCarry = counterBuf[i] == 0;
				if (!hasCarry)
					break;
			}
		}
		public void FinalizeStream() {
			byte[] tempDataBuffer = new byte[BlockSize];
			Buffer.BlockCopy(this.prevBuf, 0, tempDataBuffer, 0, prevBufSize);
			IncrementBytes(counterBuf);
			byte[] encodedCounterBuffer = this.aesBlockTransform.TransformFinalBlock(counterBuf, 0, BlockSize);
			XorBytes(tempDataBuffer, 0, encodedCounterBuffer);
			this.authenticationKeyCalculator.TransformFinalBlock(tempDataBuffer, 0, this.prevBufSize);
			this.baseStream.Write(tempDataBuffer, 0, prevBufSize);
		}
		private int CompletePrevBuffer(byte[] buffer, int offset, int count) {
			if (this.prevBufSize <= 0)
				return 0;
			int reminderBytes = Math.Min(count, BlockSize - this.prevBufSize);
			Buffer.BlockCopy(buffer, offset, this.prevBuf, prevBufSize, reminderBytes);
			this.prevBufSize += reminderBytes;
			return reminderBytes;
		}
		private void WritePrevBuf() {
			if (prevBufSize <= 0)
				return;
			byte[] tempDataBuffer = new byte[BlockSize];
			byte[] encodedCounterBuffer = new byte[BlockSize];
			Buffer.BlockCopy(this.prevBuf, 0, tempDataBuffer, 0, this.prevBufSize);
			IncrementBytes(counterBuf);
			this.aesBlockTransform.TransformBlock(counterBuf, 0, BlockSize, encodedCounterBuffer, 0);
			XorBytes(tempDataBuffer, 0, encodedCounterBuffer);
			this.authenticationKeyCalculator.TransformBlock(tempDataBuffer, 0, BlockSize, null, 0);
			this.baseStream.Write(tempDataBuffer, 0, prevBufSize);
		}
		#region rudiment
		public override void Flush() {
		}
		public override long Seek(long offset, SeekOrigin origin) {
			throw new NotSupportedException();
		}
		public override void SetLength(long value) {
			throw new NotSupportedException();
		}
		#endregion
	}
}
