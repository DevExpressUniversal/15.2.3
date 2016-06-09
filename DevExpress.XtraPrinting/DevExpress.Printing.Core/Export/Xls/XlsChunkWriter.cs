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
using System.IO;
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraExport.Xls {
	#region IXlsChunk
	public interface IXlsChunk {
		byte[] Data { get; set; }
		int GetMaxDataSize();
		void Write(BinaryWriter writer);
	}
	#endregion
	#region XlsChunk
	public class XlsChunk : IXlsChunk {
		#region Fields
		byte[] data = new byte[0];
		#endregion
		#region Properties
		public short RecordType { get; private set; }
		public byte[] Data {
			get { return data; }
			set {
				if(value != null) {
					if(value.Length > GetMaxDataSize())
						throw new ArgumentException("Data exceed maximun record data length");
					data = value;
				}
				else
					data = new byte[0];
			}
		}
		#endregion
		public XlsChunk(short recordType) {
			RecordType = recordType;
		}
		public void Write(BinaryWriter writer) {
			writer.Write(RecordType);
			writer.Write(GetSize());
			WriteCore(writer);
		}
		protected virtual void WriteCore(BinaryWriter writer) {
			if(this.data.Length > 0)
				writer.Write(this.data);
		}
		protected virtual short GetSize() {
			return (short)this.data.Length;
		}
		public virtual int GetMaxDataSize() {
			return XlsDefs.MaxRecordDataSize;
		}
	}
	#endregion
	#region XlsChunkWriter
	public class XlsChunkWriter : BinaryWriter {
		#region Fields
		BinaryWriter streamWriter;
		IXlsChunk chunk;
		IXlsChunk continueChunk;
		XlsChunkWriterInternalStream internalStream = null;
		BinaryWriter internalWriter = null;
		bool hasHighBytes;
		long stringValuePosition;
		long blockPosition;
		bool suppressAutoFlush;
		#endregion
		public XlsChunkWriter(BinaryWriter streamWriter, IXlsChunk firstChunk, IXlsChunk continueChunk)
			: base() {
			Guard.ArgumentNotNull(streamWriter, "streamWriter");
			Guard.ArgumentNotNull(firstChunk, "firstChunk");
			Guard.ArgumentNotNull(continueChunk, "continueChunk");
			this.streamWriter = streamWriter;
			this.chunk = firstChunk;
			this.continueChunk = continueChunk;
			this.internalStream = new XlsChunkWriterInternalStream();
			this.internalWriter = new BinaryWriter(this.internalStream);
			this.blockPosition = -1;
			this.stringValuePosition = -1;
		}
		#region Overrides
		public override Stream BaseStream {
			get {
				return this.internalWriter.BaseStream;
			}
		}
#if !DXPORTABLE
		public override void Close() {
			Dispose(true);
		}
#endif
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Flush();
				FreeInternals();
				this.streamWriter = null;
				this.continueChunk = null;
				this.chunk = null;
			}
		}
		public override void Flush() {
			this.suppressAutoFlush = false;
			this.internalWriter.Flush();
			FlushChunks();
			if(this.internalStream.Length > 0)
				WriteChunk();
		}
		public override long Seek(int offset, SeekOrigin origin) {
			return this.internalWriter.Seek(offset, origin);
		}
		public override void Write(bool value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		public override void Write(byte[] buffer) {
			this.internalWriter.Write(buffer);
			FlushChunks();
		}
		public override void Write(byte value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		public override void Write(byte[] buffer, int index, int count) {
			this.internalWriter.Write(buffer, index, count);
			FlushChunks();
		}
		public override void Write(char ch) {
			this.internalWriter.Write(ch);
			FlushChunks();
		}
		public override void Write(char[] chars) {
			this.internalWriter.Write(chars);
			FlushChunks();
		}
		public override void Write(char[] chars, int index, int count) {
			this.internalWriter.Write(chars, index, count);
			FlushChunks();
		}
		public override void Write(double value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		public override void Write(float value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		public override void Write(int value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		public override void Write(long value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		[CLSCompliant(false)]
		public override void Write(sbyte value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		public override void Write(short value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		public override void Write(string value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		[CLSCompliant(false)]
		public override void Write(uint value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		[CLSCompliant(false)]
		public override void Write(ulong value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
		[CLSCompliant(false)]
		public override void Write(ushort value) {
			this.internalWriter.Write(value);
			FlushChunks();
		}
#endregion
		public int SpaceInCurrentRecord {
			get { return (int)(this.chunk.GetMaxDataSize() - this.internalStream.Length); }
		}
		public bool SuppressAutoFlush {
			get { return suppressAutoFlush; }
			set {
				if(suppressAutoFlush != value) {
					suppressAutoFlush = value;
					FlushChunks();
				}
			}
		}
		public int Capacity {
			get { return (int)this.internalStream.Capacity; }
			set { this.internalStream.Capacity = value; }
		}
		public void BeginRecord(int requiredSpace) {
			if(SpaceInCurrentRecord < requiredSpace)
				Flush();
		}
		public void BeginStringValue(bool hasHighBytes) {
			EndBlock();
			this.stringValuePosition = this.internalStream.Position;
			this.hasHighBytes = hasHighBytes;
		}
		public void EndStringValue() {
			this.stringValuePosition = -1;
			this.hasHighBytes = false;
		}
		public void BeginBlock() {
			EndStringValue();
			this.blockPosition = this.internalStream.Position;
		}
		public void EndBlock() {
			this.blockPosition = -1;
		}
		public void SetNextChunk(IXlsChunk chunk) {
			this.chunk = chunk;
		}
#region Internals
		void FlushChunks() {
			if(this.suppressAutoFlush) return;
			while(this.internalStream.Length > this.chunk.GetMaxDataSize()) {
				WriteChunk();
			}
		}
		void WriteChunk() {
			this.chunk.Data = GetChunkData();
			this.chunk.Write(this.streamWriter);
			this.chunk = this.continueChunk;
		}
		byte[] GetChunkData() {
			byte[] result;
			int maxRecordDataSize = this.chunk.GetMaxDataSize();
			if(this.internalStream.Length <= maxRecordDataSize) {
				result = this.internalStream.ToArray();
				this.internalStream.RemoveBytes((int)this.internalStream.Length);
				EndStringValue();
				EndBlock();
			}
			else {
				int length = maxRecordDataSize;
				if(this.blockPosition != -1) {
					length = (int)this.blockPosition;
					if(length == 0)
						throw new Exception("Block exceed maximum Xls record data size");
				}
				else if((this.stringValuePosition != -1) && this.hasHighBytes) {
					int charLen = (int)(length - this.stringValuePosition) / 2;
					length = (int)this.stringValuePosition + charLen * 2;
				}
				result = this.internalStream.ToArray(length);
				if(this.stringValuePosition != -1)
					length--;
				this.internalStream.RemoveBytes(length);
				if(this.blockPosition != -1)
					this.blockPosition = 0;
				else if(this.stringValuePosition != -1) {
					long position = this.internalStream.Position;
					this.internalStream.Position = 0;
					this.internalWriter.Write(this.hasHighBytes);
					this.stringValuePosition = 1;
					this.internalStream.Position = position;
				}
			}
			return result;
		}
		void FreeInternals() {
			if(this.internalStream != null) {
				this.internalStream.Dispose();
				this.internalStream = null;
			}
			if(this.internalWriter != null) {
				((IDisposable)this.internalWriter).Dispose();
				this.internalWriter = null;
			}
		}
#endregion
	}
#endregion
#region XlsChunkWriterInternalStream
	public class XlsChunkWriterInternalStream : Stream {
		const int defaultCapacity = 0x10000;
		const int copyBufferSize = 0x4000;
		int capacity;
		byte[] internalBuffer;
		byte[] copyBuffer;
		int length = 0;
		int position = 0;
		bool disposed = false;
		public XlsChunkWriterInternalStream() {
			this.capacity = defaultCapacity;
			this.internalBuffer = new byte[this.capacity];
			this.copyBuffer = new byte[copyBufferSize];
		}
		public XlsChunkWriterInternalStream(int capacity) {
			if(capacity < 0)
				capacity = defaultCapacity;
			this.capacity = capacity;
			this.internalBuffer = new byte[this.capacity];
			this.copyBuffer = new byte[copyBufferSize];
		}
		public long Capacity {
			get {
				CheckDisposed();
				return capacity;
			}
			set {
				CheckDisposed();
				int newCapacity = (int)value;
				if(newCapacity < 0)
					newCapacity = defaultCapacity;
				if(newCapacity < length)
					newCapacity = length;
				if(capacity != newCapacity) {
					capacity = newCapacity;
					byte[] tmp = internalBuffer;
					internalBuffer = new byte[capacity];
					Array.Copy(tmp, 0, internalBuffer, 0, length);
				}
			}
		}
#region Overrides
		public override bool CanRead {
			get {
				CheckDisposed();
				return true;
			}
		}
		public override bool CanSeek {
			get {
				CheckDisposed();
				return true;
			}
		}
		public override bool CanWrite {
			get {
				CheckDisposed();
				return true;
			}
		}
		public override long Length {
			get {
				CheckDisposed();
				return length;
			}
		}
		public override long Position {
			get {
				CheckDisposed();
				return position;
			}
			set {
				CheckDisposed();
				position = (int)value;
				if(position < 0) position = 0;
				if(position > length) {
					long count = Math.Min(position, capacity) - length;
					for(int i = 0; i < count; i++)
						internalBuffer[i + length] = 0;
				}
			}
		}
		protected override void Dispose(bool disposing) {
			this.disposed = true;
			this.internalBuffer = null;
			this.copyBuffer = null;
		}
		public override void Flush() {
			CheckDisposed();
		}
		public override long Seek(long offset, SeekOrigin origin) {
			CheckDisposed();
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
			CheckDisposed();
			CheckArguments(buffer, offset, count);
			if(count == 0) return 0;
			if(position >= length) return 0;
			int rest = length - position;
			int bytesRead = count;
			if(bytesRead > rest)
				bytesRead = (int)rest;
			Array.Copy(internalBuffer, position, buffer, offset, bytesRead);
			return bytesRead;
		}
		public override void Write(byte[] buffer, int offset, int count) {
			CheckDisposed();
			CheckArguments(buffer, offset, count);
			if((position + count) > length)
				SetLength(position + count);
			if(count != 0) {
				Array.Copy(buffer, offset, internalBuffer, position, count);
				position += count;
			}
		}
		public override void SetLength(long value) {
			CheckDisposed();
			int newLength = (int)value;
			if(newLength < 0)
				newLength = 0;
			if(length != newLength) {
				if(value > capacity)
					Capacity = (newLength / defaultCapacity + 1) * defaultCapacity;
				length = newLength;
			}
		}
#endregion
		public byte[] ToArray(int size) {
			if(size < 0)
				throw new ArgumentOutOfRangeException("size less than 0");
			byte[] result = new byte[size];
			if(size > 0)
				Array.Copy(internalBuffer, result, Math.Min(size, length));
			return result;
		}
		public byte[] ToArray() {
			return ToArray(length);
		}
		public void RemoveBytes(int count) {
			if(count < 0)
				throw new ArgumentOutOfRangeException("count less than 0");
			if(count == 0) return;
			if(count >= length) {
				length = 0;
				position = 0;
			}
			else {
				int bytesToMove = length - count;
				int pos = 0;
				while(bytesToMove > 0) {
					if(bytesToMove > copyBufferSize) {
						Array.Copy(internalBuffer, pos + count, copyBuffer, 0, copyBufferSize);
						Array.Copy(copyBuffer, 0, internalBuffer, pos, copyBufferSize);
						pos += copyBufferSize;
						bytesToMove -= copyBufferSize;
					}
					else {
						Array.Copy(internalBuffer, pos + count, copyBuffer, 0, bytesToMove);
						Array.Copy(copyBuffer, 0, internalBuffer, pos, bytesToMove);
						bytesToMove = 0;
					}
				}
				length -= count;
				position -= count;
				if(position < 0) position = 0;
			}
		}
		void CheckDisposed() {
			if(this.disposed)
				throw new ObjectDisposedException(this.ToString());
		}
		void CheckArguments(byte[] buffer, int offset, int count) {
			if(buffer == null)
				throw new ArgumentNullException("buffer");
			if((offset + count) > buffer.Length)
				throw new ArgumentException("offset + count exceed buffer length");
			if(offset < 0)
				throw new ArgumentOutOfRangeException("offset less than 0");
			if(count < 0)
				throw new ArgumentOutOfRangeException("count less than 0");
		}
	}
#endregion
}
