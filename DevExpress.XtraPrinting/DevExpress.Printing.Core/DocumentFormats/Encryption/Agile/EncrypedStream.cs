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
using System.Security.Cryptography;
namespace DevExpress.Office.Crypto.Agile {
	public class EncryptedStream : Stream {
		ICipherProvider cipher;
		Stream dataStream;
		byte[] contentBuffer = new byte[4096];
		long contentPosition;
		long contentLength;
		bool isLengthDirty;
		bool isBufferDirty;
		public EncryptedStream(ICipherProvider cipher, Stream stream) {
			this.cipher = cipher;
			this.dataStream = stream;
			if (stream.Length > 0) {
				stream.Position = 0;
				BinaryReader reader = new BinaryReader(stream);
				this.contentLength = reader.ReadInt64();
				this.MoveToOffset(0, true);
			}
		}
		public override bool CanRead { get { return this.dataStream.CanRead; } }
		public override bool CanSeek { get { return this.dataStream.CanSeek; } }
		public override bool CanWrite { get { return this.dataStream.CanWrite; } }
		public override long Length { get { return this.contentLength; } }
		public override long Position { get { return this.contentPosition; } set { MoveToOffset(value, false); } }
		public override void SetLength(long value) {
			long streamSize = ToRealOffset(RoundToBlock(value));
			this.dataStream.SetLength(streamSize);
			SetContentLength(value);
		}
		public override long Seek(long offset, SeekOrigin origin) {
			if (origin == SeekOrigin.Begin)
				return this.Position = offset;
			if (origin == SeekOrigin.Current)
				return this.Position += offset;
			if (origin == SeekOrigin.End)
				return this.Position = this.Length + offset;
			throw new ArgumentOutOfRangeException();
		}
		public override int Read(byte[] buffer, int offset, int count) {
			int originalOffset = offset;
			long bytesRemaining = Math.Max(0, this.Length - this.Position);
			count = (int)Math.Min(count, bytesRemaining);
			int bufferOffset = (int)(this.Position % this.contentBuffer.Length);
			while (count > 0) {
				int bytesToRead = Math.Min(this.contentBuffer.Length - bufferOffset, count);
				Buffer.BlockCopy(this.contentBuffer, bufferOffset, buffer, offset, bytesToRead);
				this.MoveToOffset(this.Position + bytesToRead, false);
				offset += bytesToRead;
				count -= bytesToRead;
				bufferOffset = 0;
			}
			return offset - originalOffset;
		}
		public override void Write(byte[] buffer, int offset, int count) {
			int bufferOffset = (int)(this.Position % this.contentBuffer.Length);
			while (count > 0) {
				int bytesToWrite = Math.Min(this.contentBuffer.Length - bufferOffset, count);
				Buffer.BlockCopy(buffer, offset, this.contentBuffer, bufferOffset, bytesToWrite);
				this.isBufferDirty = true;
				this.MoveToOffset(this.Position + bytesToWrite, false);
				offset += bytesToWrite;
				count -= bytesToWrite;
				bufferOffset = 0;
			}
			if (this.Position > this.Length)
				SetContentLength(this.Position);
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				Flush();
			base.Dispose(disposing);
		}
		public override void Flush() {
			if (this.isBufferDirty)
				MoveToOffset(this.Position, true);
			if (this.isLengthDirty) {
				if (this.Length > 0) {
					long roundedLength = RoundToBlock(this.Length);
					this.dataStream.SetLength(ToRealOffset(roundedLength));
					this.dataStream.Position = 0;
					BinaryWriter writer = new BinaryWriter(this.dataStream);
					writer.Write(this.Length);
					writer.Flush();
				}
				else
					this.dataStream.SetLength(0);
				this.isLengthDirty = false;
			}
			this.dataStream.Flush();
		}
		void SetContentLength(long newLength) {
			if (this.Length != newLength) {
				this.isLengthDirty = true;
				this.contentLength = newLength;
			}
		}
		long ToRealOffset(long offset) {
			return offset + sizeof(long);
		}
		long RoundToBlock(long value) {
			return ((value + this.cipher.BlockBytes - 1) / this.cipher.BlockBytes) * this.cipher.BlockBytes;
		}
		void MoveToOffset(long newPosition, bool forceRefresh) {
			long oldBlockIndex = this.Position / this.contentBuffer.Length;
			long newBlockIndex = newPosition / this.contentBuffer.Length;
			if (oldBlockIndex != newBlockIndex || forceRefresh) {
				if (this.isBufferDirty) {
					using (ICryptoTransform encryptor = this.cipher.GetEncryptor((int)oldBlockIndex, 0)) {
						encryptor.TransformInPlace(this.contentBuffer, 0, this.contentBuffer.Length);
					}
					this.dataStream.Position = ToRealOffset(oldBlockIndex * this.contentBuffer.Length);
					this.dataStream.Write(this.contentBuffer, 0, this.contentBuffer.Length);
					this.isBufferDirty = false;
				}
				this.dataStream.Position = ToRealOffset(newBlockIndex * this.contentBuffer.Length);
				int bytesRead = this.dataStream.Read(this.contentBuffer, 0, this.contentBuffer.Length);
				if (bytesRead < this.contentBuffer.Length) {
					byte[] paddingBytes = new byte[this.contentBuffer.Length - bytesRead];
					EncryptionSession.GetRandomBytes(paddingBytes);
					Array.Copy(paddingBytes, 0, this.contentBuffer, bytesRead, paddingBytes.Length);
				}
				using (ICryptoTransform decryptor = this.cipher.GetDecryptor((int)newBlockIndex, 0)) {
					decryptor.TransformInPlace(this.contentBuffer, 0, bytesRead);
				}
			}
			this.contentPosition = newPosition;
		}
	}
}
