#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class CircularBuffer<T> {
		int length;
		int elementIndex;
		int elemetsCount;
		public int Length { get { return length; } }
		public int Index { get { return elementIndex; } }
		public int End { get { return (elementIndex + elemetsCount) % length; } }
		public int LogicalEnd { get { return elementIndex + elemetsCount; } }
		public int Count { get { return elemetsCount; } set { elemetsCount = value; } }
		public int FreeSpace { get { return length - elemetsCount; } }
		public T[] Buffer { get; set; }
		public T this[int i] {
			get {
				if (i > elemetsCount)
					throw new Exception("out of range exception");
				return Buffer[(elementIndex + i) % length];
			}
		}
		public CircularBuffer(int length) {
			this.length = length;
			Buffer = new T[length];
		}
		internal void BlockCopy(Array source, int sourceOffset, Array targer, int targetOffset, int count) {
			Array.Copy(source, sourceOffset, targer, targetOffset, count);
		}
		public void Clean() {
			elementIndex = 0;
			elemetsCount = 0;
		}
		public void Push(T[] source, int start, int count) {
			if (count > FreeSpace)
				throw new Exception("too many records in circular buffer");
			if (LogicalEnd + count > Length && End != 0) {
				BlockCopy(source, start, this.Buffer, End, Length - End);
				BlockCopy(source, start + Length - End, this.Buffer, 0, count - Length + End);
			}
			else
				BlockCopy(source, start, this.Buffer, End, count);
			this.elemetsCount += count;
		}
		public void PushOne(T value) {
			if (FreeSpace == 0)
				throw new Exception("too many records on circular buffer");
			this.Buffer[End] = value;
			this.elemetsCount++;
		}
		public int Pull(T[] target, int maxCount) {
			if (Count < 1)
				return -1;
			int realCount = maxCount > this.elemetsCount ? this.elemetsCount : maxCount;
			if (LogicalEnd > length && length - Index < realCount) {
				BlockCopy(this.Buffer, elementIndex, target, 0, length - Index);
				BlockCopy(this.Buffer, 0, target, length - Index, realCount - length + Index);
			}
			else
				BlockCopy(this.Buffer, elementIndex, target, 0, realCount);
			this.elemetsCount -= realCount;
			if (this.elemetsCount == 0)
				this.elementIndex = 0;
			else
				this.elementIndex = (this.elementIndex + realCount) % length;
			return realCount;
		}
		public int Copy(T[] target, int maxCount) {
			if (Count < 1)
				return -1;
			int realCount = maxCount > this.elemetsCount ? this.elemetsCount : maxCount;
			if (LogicalEnd > length && length - Index < realCount) {
				BlockCopy(this.Buffer, elementIndex, target, 0, length - Index);
				BlockCopy(this.Buffer, 0, target, length - Index, realCount - length + Index);
			}
			else
				BlockCopy(this.Buffer, elementIndex, target, 0, realCount);
			return realCount;
		}
		public T PullOne() {
			if (Count < 1)
				throw new Exception("not enough records in circular buffer");
			T value = this.Buffer[elementIndex];
			this.elemetsCount -= 1;
			if (this.elemetsCount == 0)
				this.elementIndex = 0;
			else
				this.elementIndex = (this.elementIndex + 1) % length;
			return value;
		}
	}
	public class ByteBuffer : CircularBuffer<byte> {
		public ByteBuffer(int length)
			: base(length) {
		}
		public void Push(Stream source, int offsetInFile, int count) {
			if (count > FreeSpace)
				throw new Exception("too many records in circular buffer");
			source.Position = offsetInFile;
			if (LogicalEnd + count > Length) {
				BlockRead(source, this.Buffer, End, Length - End);
				BlockRead(source, this.Buffer, 0, count - Length + End);
			}
			else
				BlockRead(source, this.Buffer, End, count);
			this.Count += count;
		}
		internal void BlockRead(Stream source, byte[] targer, int targetOffset, int count) {
			source.Read(targer, targetOffset, count);
		}
	}
}
