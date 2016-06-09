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
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraExport.Xls {
	using DevExpress.Utils.Crypt;
	using DevExpress.XtraSpreadsheet.Internal;
	#region XlsReader
	public class XlsReader : IDisposable {
		BinaryReader baseReader;
		public XlsReader(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			baseReader = reader;
		}
		protected internal BinaryReader BaseReader { get { return baseReader; } }
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			this.baseReader = null;
		}
		#endregion
		public virtual bool ReadBoolean() {
			return BaseReader.ReadBoolean();
		}
		public virtual byte ReadByte() {
			return BaseReader.ReadByte();
		}
		public virtual byte[] ReadBytes(int count) {
			return BaseReader.ReadBytes(count);
		}
		public virtual double ReadDouble() {
			return BaseReader.ReadDouble();
		}
		public virtual short ReadInt16() {
			return BaseReader.ReadInt16();
		}
		public virtual int ReadInt32() {
			return BaseReader.ReadInt32();
		}
		public virtual long ReadInt64() {
			return BaseReader.ReadInt64();
		}
		[CLSCompliant(false)]
		public virtual ushort ReadUInt16() {
			return BaseReader.ReadUInt16();
		}
		[CLSCompliant(false)]
		public virtual uint ReadUInt32() {
			return BaseReader.ReadUInt32();
		}
		[CLSCompliant(false)]
		public virtual ulong ReadUInt64() {
			return BaseReader.ReadUInt64();
		}
		public virtual byte[] ReadNotCryptedBytes(int count) {
			return BaseReader.ReadBytes(count);
		}
		public virtual short ReadNotCryptedInt16() {
			return BaseReader.ReadInt16();
		}
		public virtual int ReadNotCryptedInt32() {
			return BaseReader.ReadInt32();
		}
		[CLSCompliant(false)]
		public virtual ushort ReadNotCryptedUInt16() {
			return BaseReader.ReadUInt16();
		}
		public long StreamLength { get { return BaseReader.BaseStream.Length; } }
		public virtual long Position {
			get {
				return BaseReader.BaseStream.Position;
			}
			set {
				BaseReader.BaseStream.Position = value;
			}
		}
		public virtual long Seek(long offset, SeekOrigin origin) {
			return BaseReader.BaseStream.Seek(offset, origin);
		}
	}
	#endregion
	#region XlsRC4EncryptedReader
	public class XlsRC4EncryptedReader : XlsReader {
		#region Fields
		const int blockSize = 1024;
		ARC4KeyGen keygen;
		ARC4Cipher cipher;
		int blockCount;
		int bytesCount;
		#endregion
		public XlsRC4EncryptedReader(BinaryReader reader, string password, byte[] salt)
			: base(reader) {
			this.keygen = new ARC4KeyGen(password, salt);
			this.cipher = new ARC4Cipher(keygen.DeriveKey(0));
			ResetCipher(Position);
		}
		public override bool ReadBoolean() {
			byte buf = BaseReader.ReadByte();
			return Decrypt(buf) != 0;
		}
		public override byte ReadByte() {
			byte buf = BaseReader.ReadByte();
			return Decrypt(buf);
		}
		public override byte[] ReadBytes(int count) {
			byte[] buf = BaseReader.ReadBytes(count);
			return Decrypt(buf);
		}
		public override double ReadDouble() {
			byte[] buf = BaseReader.ReadBytes(sizeof(double));
			return BitConverter.ToDouble(Decrypt(buf), 0);
		}
		public override short ReadInt16() {
			byte[] buf = BaseReader.ReadBytes(sizeof(short));
			return BitConverter.ToInt16(Decrypt(buf), 0);
		}
		public override int ReadInt32() {
			byte[] buf = BaseReader.ReadBytes(sizeof(int));
			return BitConverter.ToInt32(Decrypt(buf), 0);
		}
		public override long ReadInt64() {
			byte[] buf = BaseReader.ReadBytes(sizeof(long));
			return BitConverter.ToInt64(Decrypt(buf), 0);
		}
		[CLSCompliant(false)]
		public override ushort ReadUInt16() {
			byte[] buf = BaseReader.ReadBytes(sizeof(ushort));
			return BitConverter.ToUInt16(Decrypt(buf), 0);
		}
		[CLSCompliant(false)]
		public override uint ReadUInt32() {
			byte[] buf = BaseReader.ReadBytes(sizeof(uint));
			return BitConverter.ToUInt32(Decrypt(buf), 0);
		}
		[CLSCompliant(false)]
		public override ulong ReadUInt64() {
			byte[] buf = BaseReader.ReadBytes(sizeof(ulong));
			return BitConverter.ToUInt64(Decrypt(buf), 0);
		}
		public override byte[] ReadNotCryptedBytes(int count) {
			byte[] buf = BaseReader.ReadBytes(count);
			SeekKeyStream(buf.Length);
			return buf;
		}
		public override short ReadNotCryptedInt16() {
			SeekKeyStream(sizeof(Int16));
			return BaseReader.ReadInt16();
		}
		public override int ReadNotCryptedInt32() {
			SeekKeyStream(sizeof(Int32));
			return BaseReader.ReadInt32();
		}
		[CLSCompliant(false)]
		public override ushort ReadNotCryptedUInt16() {
			SeekKeyStream(sizeof(UInt16));
			return BaseReader.ReadUInt16();
		}
		public override long Position {
			get {
				return BaseReader.BaseStream.Position;
			}
			set {
				Seek(value, SeekOrigin.Begin);
			}
		}
		public override long Seek(long offset, SeekOrigin origin) {
			long position = BaseReader.BaseStream.Seek(offset, origin);
			if((origin == SeekOrigin.Current) && (offset > 0) && (offset < (blockSize - this.bytesCount)))
				SeekKeyStream((int)offset);
			else
				ResetCipher(position);
			return position;
		}
		protected override void Dispose(bool disposing) {
			this.keygen = null;
			this.cipher = null;
			base.Dispose(disposing);
		}
		#region Internals
		void ResetCipher(long position) {
			this.blockCount = (int)(position / blockSize);
			this.bytesCount = (int)(position % blockSize);
			this.cipher.UpdateKey(this.keygen.DeriveKey(this.blockCount));
			this.cipher.Reset(this.bytesCount);
		}
		byte Decrypt(byte input) {
			byte output = this.cipher.Decrypt(input);
			this.bytesCount++;
			if(this.bytesCount == blockSize) {
				this.bytesCount = 0;
				this.blockCount++;
				this.cipher.UpdateKey(this.keygen.DeriveKey(blockCount));
			}
			return output;
		}
		byte[] Decrypt(byte[] input) {
			int count = input.Length;
			for(int i = 0; i < count; i++) {
				input[i] = this.cipher.Decrypt(input[i]);
				this.bytesCount++;
				if(this.bytesCount == blockSize) {
					this.bytesCount = 0;
					this.blockCount++;
					this.cipher.UpdateKey(this.keygen.DeriveKey(blockCount));
				}
			}
			return input;
		}
		void SeekKeyStream(int offset) {
			for(int i = 0; i < offset; i++)
				Decrypt((byte)0);
		}
		#endregion
	}
	#endregion
	#region IXlsContentBuilder
	public interface IXlsContentBuilder {
		void ReadSubstream();
	}
	#endregion
	#region XlsCommandStream
	public class XlsCommandStream : Stream {
		class BlockItem {
			public BlockItem(long startPosition, long basePosition, int size) {
				StartPosition = startPosition;
				BasePosition = basePosition;
				Size = size;
			}
			public long StartPosition { get; private set; }
			public long BasePosition { get; private set; }
			public int Size { get; private set; }
		}
		class BlockInfo : BlockItem {
			public BlockInfo(BlockItem item, int position)
				: base(item.StartPosition, item.BasePosition, item.Size) {
				InnerPosition = position;
			}
			public int InnerPosition { get; private set; }
		}
		class BlockComparable : IComparable<BlockItem> {
			long position;
			public BlockComparable(long position) {
				this.position = position;
			}
			#region IComparable<BlockItem> Members
			public int CompareTo(BlockItem other) {
				if(position < other.StartPosition)
					return 1;
				if(position >= (other.StartPosition + other.Size))
					return -1;
				return 0;
			}
			#endregion
		}
		const short beginOfSubstreamTypeCode = 0x0809;
		XlsReader reader;
		IXlsContentBuilder contentBuilder;
		short[] typeCodes;
		List<BlockItem> blocks = new List<BlockItem>();
		long position;
		public XlsCommandStream(XlsReader reader, int firstBlockSize) {
			Guard.ArgumentNotNull(reader, "reader");
			this.reader = reader;
			this.typeCodes = new short[0];
			AddBlock(reader.Position, firstBlockSize);
		}
		public XlsCommandStream(XlsReader reader, short[] typeCodes, int firstBlockSize) {
			Guard.ArgumentNotNull(reader, "reader");
			Guard.ArgumentNotNull(typeCodes, "typeCodes");
			Guard.ArgumentPositive(typeCodes.Length, "typeCodes length");
			this.reader = reader;
			this.typeCodes = typeCodes;
			AddBlock(reader.Position, firstBlockSize);
		}
		public XlsCommandStream(IXlsContentBuilder contentBuilder, XlsReader reader, short[] typeCodes, int firstBlockSize)
			: this(reader, typeCodes, firstBlockSize) {
			Guard.ArgumentNotNull(contentBuilder, "contentBuilder");
			this.contentBuilder = contentBuilder;
		}
		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return true; } }
		public override bool CanWrite { get { return false; } }
		public override long Length {
			get {
				long result = 0;
				for(int i = 0; i < blocks.Count; i++)
					result += blocks[i].Size;
				return result;
			}
		}
		public override long Position {
			get { return position; }
			set {
				if(position == value) return;
				long length = Length;
				if(value <= 0) {
					position = 0;
					reader.Position = blocks[0].BasePosition;
				}
				else if(value < length) {
					BlockInfo info = FindBlock(value);
					position = value;
					reader.Position = info.BasePosition + info.InnerPosition;
				}
				else {
					BlockItem block = blocks[blocks.Count - 1];
					position = length;
					reader.Position = block.BasePosition + block.Size;
					while(true) {
						if(!TryReadNextBlock()) {
							position = length;
							return;
						}
						block = blocks[blocks.Count - 1];
						length += block.Size;
						if(value < length) {
							BlockInfo info = FindBlock(value);
							position = value;
							reader.Position = info.BasePosition + info.InnerPosition;
							return;
						}
						else {
							reader.Seek(block.Size, SeekOrigin.Current);
						}
					}
				}
			}
		}
		protected override void Dispose(bool disposing) {
			this.reader = null;
			base.Dispose(disposing);
		}
		public override void Flush() {
		}
		public override long Seek(long offset, SeekOrigin origin) {
			switch(origin) {
				case SeekOrigin.Begin:
					Position = offset;
					break;
				case SeekOrigin.Current:
					Position = Position + offset;
					break;
				case SeekOrigin.End:
					Position = Length + offset;
					break;
			}
			return Position;
		}
		public override int Read(byte[] buffer, int offset, int count) {
			if(count <= 0) return 0;
			int bytesRead = 0;
			int bytesToRead = count;
			while(bytesToRead > 0) {
				BlockInfo info = FindBlock(position);
				if(info == null) {
					if(!TryReadNextBlock())
						return bytesRead;
					info = new BlockInfo(blocks[blocks.Count - 1], 0);
				}
				int restOfBlock = info.Size - info.InnerPosition;
				int bytesCount = Math.Min(bytesToRead, restOfBlock);
				byte[] buf = reader.ReadBytes(bytesCount);
				Array.Copy(buf, 0, buffer, offset, bytesCount);
				position += bytesCount;
				offset += bytesCount;
				bytesRead += bytesCount;
				bytesToRead -= bytesCount;
			}
			return bytesRead;
		}
		public void SkipTillEndOfBlock() {
			BlockInfo info = FindBlock(position);
			if(info == null)
				return;
			int restOfBlock = info.Size - info.InnerPosition;
			if(restOfBlock > 0)
				Seek(restOfBlock, SeekOrigin.Current);
		}
		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}
		public override void SetLength(long value) {
			throw new NotSupportedException();
		}
		public short GetNextTypeCode() {
			if((reader.StreamLength - reader.Position) < 2)
				return 0;
			short typeCode = reader.ReadNotCryptedInt16();
			reader.Seek(-2, SeekOrigin.Current);
			return typeCode;
		}
		bool TryReadNextBlock() {
			if((reader.StreamLength - reader.Position) < 2)
				return false;
			short typeCode = reader.ReadNotCryptedInt16();
			if(IsAppropriateTypeCode(typeCode)) {
				int dataSize = reader.ReadNotCryptedUInt16();
				if(dataSize > XlsDefs.MaxRecordDataSize)
					throw new Exception(string.Format("Record data size greater than {0}", XlsDefs.MaxRecordDataSize));
				int frtHeaderSize = GetFutureRecordHeaderSize(typeCode);
				if(frtHeaderSize > 0) {
					reader.ReadBytes(frtHeaderSize);
					dataSize -= frtHeaderSize;
				}
				AddBlock(reader.Position, dataSize);
				return true;
			}
			else if(typeCode != beginOfSubstreamTypeCode || this.contentBuilder == null) {
				reader.Seek(-2, SeekOrigin.Current);
				return false;
			}
			reader.Seek(-2, SeekOrigin.Current);
			this.contentBuilder.ReadSubstream();
			return TryReadNextBlock();
		}
		public bool IsAppropriateTypeCode(short typeCode) {
			for(int i = 0; i < typeCodes.Length; i++) {
				if(typeCodes[i] == typeCode)
					return true;
			}
			return false;
		}
		int GetFutureRecordHeaderSize(short typeCode) {
			switch(typeCode) {
				case 0x0812:
					return 4;
				case 0x0875:
				case 0x087f:
				case 0x089f:
					return 12;
			}
			return 0;
		}
		BlockInfo FindBlock(long position) {
			int index = Algorithms.BinarySearch<BlockItem>(blocks, new BlockComparable(position));
			if(index >= 0) {
				BlockItem item = blocks[index];
				return new BlockInfo(item, (int)(position - item.StartPosition));
			}
			return null;
		}
		void AddBlock(long basePosition, int size) {
			long startPosition = 0;
			int count = blocks.Count;
			if(count > 0) {
				BlockItem lastItem = blocks[count - 1];
				startPosition = lastItem.StartPosition + lastItem.Size;
			}
			blocks.Add(new BlockItem(startPosition, basePosition, size));
		}
	}
	#endregion
}
