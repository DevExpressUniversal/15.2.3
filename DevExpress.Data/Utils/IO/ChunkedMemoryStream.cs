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
using DevExpress.Utils;
namespace DevExpress.Office.Utils {
	#region ChunkedMemoryStream
	public class ChunkedMemoryStream : Stream, ISupportsCopyFrom<ChunkedMemoryStream> {
		#region Fields
		public const int DefaultMaxBufferSize = 65536;
		int maxBufferSize = DefaultMaxBufferSize;
		long totalLength;
		long position;
		readonly List<byte[]> buffers = new List<byte[]>();
		#endregion
		#region Properties
		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return true; } }
		public override bool CanWrite { get { return true; } }
		public override long Length { get { return totalLength; } }
		public override long Position { get { return position; } set { position = value; } }
		public int MaxBufferSize { get { return maxBufferSize; } protected internal set { maxBufferSize = value; } }
		protected internal List<byte[]> Buffers { get { return buffers; } }
		#endregion
		public IList<byte[]> GetBuffers() {
			return buffers;
		}
		public override void Flush() {
		}
		public byte[] ToArray() {
			byte[] result = new byte[totalLength];
			int offset = 0;
			int count = buffers.Count - 1;
			for (int i = 0; i < count; i++) {
				Array.Copy(buffers[i], 0, result, offset, maxBufferSize);
				offset += maxBufferSize;
			}
			long bytesLeft = totalLength % maxBufferSize;
			if (bytesLeft > 0)
				Array.Copy(buffers[count], 0, result, offset, (int)bytesLeft);
			return result;
		}
		public override long Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.Begin:
					position = offset;
					break;
				case SeekOrigin.Current:
					position += offset;
					break;
				case SeekOrigin.End:
					position = totalLength + offset;
					break;
			}
			if (position < 0)
				ThrowArgumentException("offset", offset);
			return position;
		}
		static void ThrowArgumentException(string propName, object val) {
			string valueStr = (val != null) ? val.ToString() : "null";
			string s = String.Format("'{0}' is not a valid value for '{1}'", valueStr, propName);
			throw new ArgumentException(s);
		}
		public override void SetLength(long value) {
			Guard.ArgumentNonNegative(value, "value");
			int requiredBufferCount = (int)(value / maxBufferSize);
			if ((value % maxBufferSize) != 0)
				requiredBufferCount++;
			int existingBuffersCount = buffers.Count;
			if (requiredBufferCount < existingBuffersCount)
				buffers.RemoveRange(requiredBufferCount, existingBuffersCount - requiredBufferCount);
			else {
				int count = requiredBufferCount - existingBuffersCount;
				for (int i = 0; i < count; i++)
					buffers.Add(new byte[maxBufferSize]);
			}
			this.totalLength = value;
			if (position > value)
				position = value;
		}
		void EnsureBuffers(long size) {
			if (size <= buffers.Count * maxBufferSize)
				return;
			SetLength(size);
		}
		public override void Write(byte[] buffer, int offset, int count) {
			if (count <= 0)
				return;
			EnsureBuffers(position + count);
			this.totalLength = Math.Max(position + count, totalLength);
			int positionBufferIndex = (int)(position / maxBufferSize);
			int currentBufferOffset = (int)(position % maxBufferSize);
			byte[] currentBuffer = buffers[positionBufferIndex];
			int available = maxBufferSize - currentBufferOffset;
			if (count <= available) {
				Array.Copy(buffer, offset, currentBuffer, currentBufferOffset, count);
				this.position += count;
				return;
			}
			else {
				Array.Copy(buffer, offset, currentBuffer, currentBufferOffset, available);
				offset += available;
				count -= available;
				this.position += available;
				positionBufferIndex++;
			}
			while (count >= maxBufferSize) {
				currentBuffer = buffers[positionBufferIndex];
				Array.Copy(buffer, offset, currentBuffer, 0, maxBufferSize);
				offset += maxBufferSize;
				count -= maxBufferSize;
				this.position += maxBufferSize;
				positionBufferIndex++;
			}
			if (count > 0) {
				currentBuffer = buffers[positionBufferIndex];
				Array.Copy(buffer, offset, currentBuffer, 0, count);
				this.position += count;
			}
		}
		public override int Read(byte[] buffer, int offset, int count) {
			count = Math.Min(count, (int)(Length - position));
			if (count <= 0)
				return 0;
			int result = count;
			int positionBufferIndex = (int)(position / maxBufferSize);
			byte[] currentBuffer = buffers[positionBufferIndex];
			int currentBufferOffset = (int)(position % maxBufferSize);
			int bytesLeft = MaxBufferSize - currentBufferOffset;
			if (count < bytesLeft) {
				Array.Copy(currentBuffer, currentBufferOffset, buffer, offset, count);
				position += count;
				return count;
			}
			else {
				Array.Copy(currentBuffer, currentBufferOffset, buffer, offset, bytesLeft);
				offset += bytesLeft;
				count -= bytesLeft;
				position += bytesLeft;
				positionBufferIndex++;
			}
			while (count >= maxBufferSize) {
				currentBuffer = buffers[positionBufferIndex];
				Array.Copy(currentBuffer, 0, buffer, offset, maxBufferSize);
				offset += maxBufferSize;
				count -= maxBufferSize;
				this.position += maxBufferSize;
				positionBufferIndex++;
			}
			if (count > 0) {
				currentBuffer = buffers[positionBufferIndex];
				Array.Copy(currentBuffer, 0, buffer, offset, count);
				this.position += count;
			}
			return result;
		}
		public override bool Equals(object obj) {
			ChunkedMemoryStream stream = obj as ChunkedMemoryStream;
			return Equals(stream);
		}
		public bool Equals(ChunkedMemoryStream stream) {
			if(stream == null)
				return false;
			IList<byte[]> buffers1 = this.GetBuffers();
			IList<byte[]> buffers2 = stream.GetBuffers();
			if(buffers1.Count != buffers2.Count)
				return false;
			for(int i = 0; i < buffers1.Count; i++) {
				if(buffers1[i].Length != buffers2[i].Length)
					return false;
				for(int k = 0; i < buffers1[i].Length; k++)
					if(buffers1[i][k] != buffers2[i][k])
						return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ISupportsCopyFrom<ChunkedMemoryStream> Members
		public void CopyFrom(ChunkedMemoryStream value) {
			int count = value.buffers.Count;
			for(int i = 0; i < count; i++)
				this.buffers.Add(value.buffers[i]);
			this.totalLength = value.totalLength;
		}
		#endregion
	}
	#endregion
}
