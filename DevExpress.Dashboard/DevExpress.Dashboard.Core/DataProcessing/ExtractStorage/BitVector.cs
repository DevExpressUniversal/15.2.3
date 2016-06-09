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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing {
	public struct BitVector : IEnumerable<bool> {
		public struct VectorEnumerator : IEnumerator<bool> {
			int pos;
			BitVector vector;
			public bool Current { get { return vector[pos]; } }
			object IEnumerator.Current { get { return Current; } }
			public VectorEnumerator(BitVector vector) {
				this.pos = -1;
				this.vector = vector;
			}
			public void Dispose() {
				pos = int.MaxValue;
			}
			public bool MoveNext() {
				pos++;
				return pos < vector.Length;
			}
			public void Reset() {
				pos = -1;
			}
		}
		const int BitsPerInt = 32;
		const uint fillMask = 0xffffffff;
		readonly int offsetStart;
		readonly int offsetEnd;
		readonly uint[] storage;
		public bool this[int index] {
			get { return Get(index); }
			set { Set(index, value); }
		}
		public int Length { get { return offsetEnd - offsetStart; } }
		[CLSCompliant(false)]
		public BitVector(int offsetStart, int offsetEnd, uint[] storage) {
			this.offsetStart = offsetStart;
			this.offsetEnd = offsetEnd;
			this.storage = storage;
		}
		public BitVector(int count) {
			this.offsetStart = 0;
			this.offsetEnd = count;
			this.storage = new uint[count / BitsPerInt + (count % BitsPerInt == 0 ? 0 : 1)];
		}
		public bool Get(int index) {
			if(index < 0 || index >= offsetEnd - offsetStart)
				throw new IndexOutOfRangeException();
			return Get(storage, index + offsetStart);
		}
		public void Set(int index, bool value) {
			if(index < 0 || index >= offsetEnd - offsetStart)
				throw new IndexOutOfRangeException();
			Set(storage, index + offsetStart, value);
		}
		public void FillRange(int start, int end, bool value) {
			int size = offsetEnd - offsetStart;
			if(start < 0 || start > size || end < 0 || end > size)
				throw new IndexOutOfRangeException();
			Fill(storage, start + offsetStart, end + offsetStart, value);
		}
		public void CopyTo(int position, BitVector destination, int destPosition, int count) {
			int internalPos = position + offsetStart;
			int internalDestPos = destPosition + destination.offsetStart;
			for(int i = 0; i < count; i++)
				destination[i + internalDestPos] = this[i + internalPos];
		}
		public VectorEnumerator GetEnumerator() {
			return new VectorEnumerator(this);
		}
		IEnumerator<bool> IEnumerable<bool>.GetEnumerator() {
			return new VectorEnumerator(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return new VectorEnumerator(this);
		}
		#region static
		static bool Get(uint[] storage, int index) {
			uint mask = (uint)1 << (index % BitsPerInt);
			return (storage[index / BitsPerInt] & mask) != 0;
		}
		static void Set(uint[] storage, int index, bool value) {
			uint mask = (uint)1 << (index % BitsPerInt);
			if(value)
				storage[index / BitsPerInt] |= mask;
			else
				storage[index / BitsPerInt] &= ~mask;
		}
		static void Fill(uint[] storage, int start, int end, bool value) {
			int startBitNum = start % BitsPerInt;
			uint startMask = fillMask << startBitNum;
			int endBitNum = end % BitsPerInt;
			uint endMask = fillMask >> (BitsPerInt - endBitNum - 1);
			int startStorageIndex = start / BitsPerInt;
			int endStorageIndex = end / BitsPerInt;
			if(startStorageIndex == endStorageIndex) {
				uint mask = startMask & endMask;
				if(value)
					storage[startStorageIndex] |= mask;
				else
					storage[startStorageIndex] &= ~mask;
			} else {
				if(value) {
					storage[startStorageIndex] |= startMask;
					for(int i = startStorageIndex + 1; i < endStorageIndex; i++)
						storage[i] = fillMask;
					storage[endStorageIndex] |= endMask;
				} else {
					storage[startStorageIndex] &= ~startMask;
					for(int i = startStorageIndex + 1; i < endStorageIndex; i++)
						storage[i] = 0;
					storage[endStorageIndex] &= ~endMask;
				}
			}
		}
		#endregion
	}
}
