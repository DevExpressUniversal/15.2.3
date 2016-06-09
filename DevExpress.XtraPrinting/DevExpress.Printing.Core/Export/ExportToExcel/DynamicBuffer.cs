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
using System.Text;
namespace DevExpress.XtraExport {
	public class DynamicBuferException : ApplicationException {
		public DynamicBuferException(string message)
			: base(message) {
		}
	}
	[CLSCompliant(false)]
	abstract public class DynamicBufferBase<T> {
		List<T[]> buffer;
		protected int bufferSize;
		public DynamicBufferBase() {
			buffer = new List<T[]>();
		}
		public DynamicBufferBase(int capacity) {
			if(capacity > 0)
				buffer = new List<T[]>(capacity);
			else
				buffer = new List<T[]>();
		}
		public DynamicBufferBase(T[] data) {
			buffer = new List<T[]>();
			buffer.Add(data);
			bufferSize = data.Length;
		}
		public T[] this[int index] {
			get {
				return buffer[index];
			}
		}
		public T[] Data {
			get {
				T[] result = NewArray(bufferSize);
				int k = 0;
				for(int i = 0; i < Count; i++)
					for(int j = 0; j < this[i].Length; j++)
						SetItem(result, k++, GetItem(this[i], j));
				return result;
			}
		}
		public int Count {
			get {
				return buffer.Count;
			}
		}
		public int Size {
			get {
				return bufferSize;
			}
		}
		void CheckOutOfBuffer(bool condition) {
			if(!condition)
				throw new DynamicBuferException("offset is out of buffer");
		}
		protected void OffsetToInternalIndices(int offset, out int listIndex, out int arrayIndex) {
			int size = 0;
			listIndex = 0;
			arrayIndex = 0;
			for(int i = 0; i < Count; i++) {
				size += this[i].Length;
				if(offset < size) {
					listIndex = i;
					arrayIndex = offset - (size - this[i].Length);
					break;
				}
			}
		}
		protected T[] NewArray(int size) {
			return new T[size];
		}
		protected T GetItem(T[] array, int index) {
			return array[index];
		}
		protected void SetItem(T[] array, int index, T data) {
			array[index] = data;
		}
		protected bool EqualItems(T item1, T item2) {
			return item1.Equals(item2);
		}
		public void Realloc(int size) {
			if(size == 0)
				Clear();
			else {
				if(size > bufferSize)
					buffer.Add(NewArray(size - bufferSize));
				bufferSize = size;
			}
		}
		public void Alloc(int size) {
			if(size > 0) {
				buffer.Add(NewArray(size));
				bufferSize += size;
			}
		}
		public void Clear() {
			buffer.Clear();
			bufferSize = 0;
		}
		public T GetElement(int offset) {
			CheckOutOfBuffer(offset >= 0 && offset < bufferSize);
			int listIndex = 0;
			int arrayIndex = 0;
			OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
			return GetItem(this[listIndex], arrayIndex);
		}
		public T[] GetElements(int offset, int count) {
			if(count <= 0)
				return null;
			CheckOutOfBuffer(offset >= 0 && (offset + count) <= bufferSize);
			T[] result = NewArray(count);
			int listIndex = 0;
			int arrayIndex = 0;
			int dataOffset = 0;
			OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
			for(int i = arrayIndex; i < this[listIndex].Length; i++) {
				if(dataOffset >= count)
					return result;
				SetItem(result, dataOffset, GetItem(this[listIndex], i));
				dataOffset++;
			}
			if(listIndex < (Count - 1)) {
				for(int i = listIndex + 1; i < Count; i++) {
					for(int j = 0; j < this[i].Length; j++) {
						if(dataOffset >= count)
							return result;
						SetItem(result, dataOffset, GetItem(this[i], j));
						dataOffset++;
					}
				}
			}
			return result;
		}
		public void SetElement(int offset, T data) {
			CheckOutOfBuffer(offset >= 0 && offset < bufferSize);
			int listIndex = 0;
			int arrayIndex = 0;
			OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
			SetItem(this[listIndex], arrayIndex, data);
		}
		public void SetElements(int offset, T[] data, int dOffset, int size) {
			if((dOffset + size) > data.Length || (offset + size) > bufferSize)
				return;
			CheckOutOfBuffer(dOffset >= 0 && (dOffset + size) <= data.Length);
			CheckOutOfBuffer(offset >= 0 && (offset + size) <= bufferSize);
			if(data.Length > 0) {
				int listIndex = 0;
				int arrayIndex = 0;
				int dataOffset = dOffset;
				OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
				for(int i = arrayIndex; i < this[listIndex].Length; i++) {
					if((dataOffset - dOffset) >= size)
						return;
					SetItem(this[listIndex], i, GetItem(data, dataOffset));
					dataOffset++;
				}
				if(listIndex < (Count - 1)) {
					for(int i = listIndex + 1; i < Count; i++) {
						for(int j = 0; j < this[i].Length; j++) {
							if((dataOffset - dOffset) >= size)
								return;
							SetItem(this[i], j, GetItem(data, dataOffset));
							dataOffset++;
						}
					}
				}
			}
		}
		public void SetElements(int offset, T[] data, int size) {
			SetElements(offset, data, 0, size);
		}
		public void SetElements(int offset, T[] data) {
			SetElements(offset, data, 0, data.Length);
		}
		public void FillElements(int offset, int count, T data) {
			CheckOutOfBuffer(offset >= 0 && (offset + count) <= bufferSize);
			int listIndex = 0;
			int arrayIndex = 0;
			int dataOffset = 0;
			OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
			for(int i = arrayIndex; i < this[listIndex].Length; i++) {
				if(dataOffset >= count)
					return;
				SetItem(this[listIndex], i, data);
				dataOffset++;
			}
			if(listIndex < (Count - 1)) {
				for(int i = listIndex + 1; i < Count; i++) {
					for(int j = 0; j < this[i].Length; j++) {
						if(dataOffset >= count)
							return;
						SetItem(this[i], j, data);
						dataOffset++;
					}
				}
			}
		}
		public bool Compare(int offset, T[] data, int dOffset, int size) {
			if((dOffset + size) > data.Length || (offset + size) > bufferSize)
				return false;
			CheckOutOfBuffer(dOffset >= 0 && (dOffset + size) <= data.Length);
			CheckOutOfBuffer(offset >= 0 && (offset + size) <= bufferSize);
			if(data.Length <= 0) 
				return false;
			int listIndex = 0;
			int arrayIndex = 0;
			int dataOffset = dOffset;
			OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
			bool? result = CompareBlock(arrayIndex, listIndex, data, size, ref dataOffset);
			if(result.HasValue) return result.Value;
			if(listIndex < (Count - 1)) {
				for(int i = listIndex + 1; i < Count; i++) {
					result = CompareBlock(0, i, data, size, ref dataOffset);
					if(result.HasValue) return result.Value;
				}
			}
			return true;
		}
		bool? CompareBlock(int arrayIndex, int listIndex, T[] data, int size, ref int dataOffset) {
			int initialDataOffset = dataOffset;
			for(int i = arrayIndex; i < this[listIndex].Length; i++) {
				if(dataOffset >= data.Length || dataOffset >= size + initialDataOffset)
					return true;
				if(!EqualItems(GetItem(this[listIndex], i), GetItem(data, dataOffset)))
					return false;
				dataOffset++;
			}
			return null;
		}
		public bool Compare(int offset, T[] data, int size) {
			return Compare(offset, data, 0, size);
		}
		public bool Compare(int offset, T[] data) {
			return Compare(offset, data, 0, data.Length);
		}
	}
	[CLSCompliant(false)]
	public sealed class DynamicByteBuffer : DynamicBufferBase<byte> {
		public DynamicByteBuffer()
			: base() {
		}
		public DynamicByteBuffer(int capacity)
			: base(capacity) {
		}
		public DynamicByteBuffer(byte[] data)
			: base(data) {
		}
		public void WriteToStream(XlsStream stream, int size) {
			if(size > 0 && size <= bufferSize) {
				int listIndex = 0;
				int arrayIndex = 0;
				OffsetToInternalIndices(size - 1, out listIndex, out arrayIndex);
				byte[] array = null;
				for(int i = 0; i < listIndex; i++) {
					array = this[i];
					stream.Write(array, 0, array.Length);
				}
				array = this[listIndex];
				int itemSize = array.Length / this[listIndex].Length;
				stream.Write(array, 0, (arrayIndex + 1) * itemSize);
			}
		}
		public void WriteToStream(XlsStream stream) {
			for(int i = 0; i < Count; i++) {
				byte[] array = this[i];
				stream.Write(array, 0, array.Length);
			}
		}
	}
	[CLSCompliant(false)]
	public sealed class DynamicMergeRectBuffer : DynamicBufferBase<MergeRect> {
		public DynamicMergeRectBuffer()
			: base() {
		}
		public DynamicMergeRectBuffer(int capacity)
			: base(capacity) {
		}
		public DynamicMergeRectBuffer(MergeRect[] data)
			: base(data) {
		}
	}
}
