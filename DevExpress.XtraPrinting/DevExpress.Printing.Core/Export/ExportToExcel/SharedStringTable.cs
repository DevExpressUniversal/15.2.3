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
using System.Collections;
namespace DevExpress.XtraExport {
	[CLSCompliant(false)]
	public class SSTList {
		ArrayList list;
		public SSTList() {
			list = new ArrayList();
		}
		public int Count {
			get {
				return list.Count;
			}
		}
		public SSTBlock this[int index] {
			get {
				return (SSTBlock)list[index];
			}
		}
		public int Add() {
			return list.Add(new SSTBlock());
		}
	}
	[CLSCompliant(false)]
	public class SSTStringInfoList {
		ArrayList list;
		public SSTStringInfoList() {
			list = new ArrayList();
		}
		public int Count {
			get {
				return list.Count;
			}
		}
		public SSTStringInfo this[int index] {
			get {
				return (SSTStringInfo)list[index];
			}
		}
		public void Add(int count) {
			for(int i = 0; i < count; i++)
				list.Add(new SSTStringInfo());
		}
		public int Capacity {
			get {
				return list.Capacity;
			}
			set {
				if(value > 0 && value > list.Capacity)
					list.Capacity = value;
			}
		}
	}
	[CLSCompliant(false)]
	public class XlsStringTable {
		ExtSST extSST = new ExtSST();
		SSTList sst;
		SSTStringInfoList stringsInfo;
		public XlsStringTable() {
			sst = new SSTList();
			stringsInfo = new SSTStringInfoList();
			Clear();
		}
		public int UniqueStringCount {
			get {
				if(sst.Count > 0)
					return BitConverter.ToInt32(sst[0].Data.GetElements(1 * 4, 4), 0);
				else
					return 0;
			}
		}
		public int SSTLength {
			get {
				return sst.Count;
			}
		}
		public int PackedSize {
			get {
				int result = 0;
				for(int i = 0; i < SSTLength; i++)
					result += sst[i].DataSize + 4;
				if(result != 0) {
					CreateExtSST(0);
					result += extSST.DataSize + 4;
				}
				return result;
			}
		}
		bool IsOptimize() {
			return XlsConsts.Optimization == XlsExportOptimization.BySize;
		}
		int GetHashCode(string string_, int count) {
			int result = 0;
			return result;
		}
		bool CheckString(string string_, ushort block, ushort offset, ushort size) {
			byte[] str = StringToByteArray(string_, 0);
			if((size + offset) <= sst[block].DataSize)
				return sst[block].Data.Compare(offset, str, size);
			else {
				bool result = true;
				int strPos = 0;
				int len = 0;
				while(size > 0) {
					len = sst[block].DataSize - offset;
					if(len < size) {
						result &= sst[block].Data.Compare(offset, str, strPos, len);
						size -= (ushort)len;
						strPos += len;
						offset = 1;
						block++;
					}
					else {
						result &= sst[block].Data.Compare(offset, str, strPos, size);
						break;
					}
				}
				return result;
			}
		}
		int IndexOf(string string_) {
			if(!IsOptimize())
				return -1;
			ushort srcLen = Convert.ToUInt16(string_.Length);
			if(srcLen > 32768)
				srcLen = 32768;
			int result = -1;
			srcLen <<= 1;
			ushort hashCode = Convert.ToUInt16(GetHashCode(string_, srcLen));
			for(int i = 0; i < UniqueStringCount; i++) {
				if(stringsInfo[i].HashCode == hashCode &&
					srcLen == stringsInfo[i].StrSize) {
					if(CheckString(string_, stringsInfo[i].Block,
						(ushort)((int)stringsInfo[i].Offset + 3),
						stringsInfo[i].StrSize)) {
						result = i;
						break;
					}
				}
			}
			return result;
		}
		int AddBlock() {
			int result = SSTLength;
			sst.Add();
			sst[result].FillElements(0);
			sst[result].RecType = XlsConsts.Continue;
			return result;
		}
		void AddStringInfo(string string_, ushort block, ushort offset, ushort size) {
			int infoCount = stringsInfo.Count;
			int infoIndex = UniqueStringCount;
			if(infoCount <= UniqueStringCount) {
				stringsInfo.Capacity += 512;
				stringsInfo.Add(512);
			}
			stringsInfo[infoIndex].HashCode = (ushort)GetHashCode(string_, size);
			stringsInfo[infoIndex].StrSize = size;
			stringsInfo[infoIndex].Block = block;
			stringsInfo[infoIndex].Offset = offset;
		}
		void AddStringToBlock(byte[] str, SSTBlock dest, ushort size) {
			dest.Data.SetElement(dest.DataSize, (byte)1);
			dest.DataSize++;
			byte[] buf;
			if(str.Length > size) {
				buf = new byte[size];
				for(int i = 0; i < size; i++)
					buf[i] = str[i];
			}
			else
				buf = str;
			dest.Data.SetElements(dest.DataSize, buf);
			dest.DataSize += size;
		}
		byte[] StringToByteArray(string string_, int offset) {
			int length = string_.Length * 2;
			if(offset < 0 || offset >= length)
				return null;
			byte[] result = new byte[length];
			int j = 0;
			for(int i = 0; i < string_.Length; i++) {
				result[j] = ((byte)string_[i]);
				result[j + 1] = (byte)(((short)string_[i]) >> 8);
				j += 2;
			}
			byte[] res = new byte[length - offset];
			for(int i = offset; i < length; i++)
				res[i - offset] = result[i];
			return res;
		}
		void InsertStr(string string_) {
			short endBlock = (short)(SSTLength - 1);
			if(endBlock < 0) {
				endBlock = (short)AddBlock();
				sst[endBlock].RecType = XlsConsts.SST;
				sst[endBlock].DataSize = 8;
				sst[endBlock].StringOffset = 8;
			}
			ushort strSize = (ushort)string_.Length;
			if(strSize > 32768)
				strSize = 32768;
			strSize <<= 1;
			if((sst[endBlock].DataSize + 4) > XlsConsts.MaxBlockSize)
				endBlock = (short)AddBlock();
			ushort writeSize = (ushort)(XlsConsts.MaxBlockSize -
				(sst[endBlock].DataSize + 3));
			if(writeSize > strSize)
				writeSize = strSize;
			else {
				if((writeSize & 0x1) != 0)
					writeSize--;
			}
			if(sst[endBlock].StringCount == 0)
				sst[endBlock].StringOffset = sst[endBlock].DataSize;
			AddStringInfo(string_, (ushort)endBlock, sst[endBlock].DataSize, strSize);
			sst[endBlock].StringCount++;
			sst[endBlock].Data.SetElements(sst[endBlock].DataSize,
				BitConverter.GetBytes((ushort)(strSize >> 1)));
			sst[endBlock].DataSize += 2;
			AddStringToBlock(StringToByteArray(string_, 0), sst[endBlock], writeSize);
			ushort offset = 0;
			while((strSize - writeSize) > 0) {
				offset += writeSize;
				strSize -= writeSize;
				endBlock = (short)AddBlock();
				if(strSize > (XlsConsts.MaxBlockSize - 1))
					writeSize = (ushort)(XlsConsts.MaxBlockSize - 1);
				else
					writeSize = strSize;
				AddStringToBlock(StringToByteArray(string_, offset), sst[endBlock], writeSize);
			}
		}
		int GetSkipSize(ushort block) {
			int result = 4;
			for(int i = 1; i < block; i++)
				result += sst[i].DataSize;
			return result;
		}
		void CreateExtSST(int sstOffset) {
			if(SSTLength == 0)
				return;
			ushort stringCount = 8;
			int blocksCount = 1;
			while((UniqueStringCount - stringCount * blocksCount) > 0) {
				stringCount += 8;
				if(blocksCount < 127)
					if((UniqueStringCount - stringCount * blocksCount) > 0)
						blocksCount++;
			}
			while(((blocksCount - 1) * stringCount) > UniqueStringCount)
				blocksCount--;
			extSST.DataSize = (ushort)(2 + blocksCount * 8);
			extSST.StringPerBlock = stringCount;
			for(int i = 0; i < blocksCount; i++) {
				extSST.Data[i].StreamOffset =
					sstOffset +
					GetSkipSize(stringsInfo[i * stringCount].Block) +
					stringsInfo[i * stringCount].Offset;
			}
		}
		public int Add(string string_) {
			if(string_.Length > 4096)
				string_ = string_.Remove(4096, string_.Length - 4096);
			int result = IndexOf(string_);
			int val = 0;
			if(result == -1) {
				result = UniqueStringCount;
				InsertStr(string_);
				val = BitConverter.ToInt32(sst[0].Data.GetElements(1 * 4, 4), 0);
				val++;
				sst[0].Data.SetElements(1 * 4, BitConverter.GetBytes(val));
			}
			val = BitConverter.ToInt32(sst[0].Data.GetElements(0, 4), 0);
			val++;
			sst[0].Data.SetElements(0, BitConverter.GetBytes(val));
			return result;
		}
		public void Clear() {
			extSST.RecType = XlsConsts.ExtSST;
		}
		public void SaveToStream(XlsStream stream, int position) {
			if(position < 0)
				position = (int)stream.Position;
			CreateExtSST(position);
			for(int i = 0; i < SSTLength; i++)
				sst[i].WriteToStream(stream);
			if(extSST.DataSize > 0)
				extSST.WriteToStream(stream);
		}
	}
	[CLSCompliant(false)]
	public class SSTBlock {
		public ushort StringCount;
		public ushort StringOffset;
		public ushort RecType;
		public ushort DataSize;
		public DynamicByteBuffer Data;
		public SSTBlock() {
			Data = new DynamicByteBuffer();
			Data.Alloc(8192);
		}
		public void FillElements(byte data) {
			StringCount = (ushort)data;
			StringOffset = (ushort)data;
			RecType = (ushort)data;
			DataSize = (ushort)data;
			Data.FillElements(0, Data.Size, (byte)0);
		}
		public void WriteToStream(XlsStream stream) {
			stream.Write(BitConverter.GetBytes(RecType), 0, 2);
			stream.Write(BitConverter.GetBytes(DataSize), 0, 2);
			Data.WriteToStream(stream, DataSize);
		}
	}
	[CLSCompliant(false)]
	public class ExtSSTBlock {
		public int StreamOffset;
		public ushort StringOffset;
		public ushort Reserved;
		public void WriteToStream(XlsStream stream, ref int size) {
			if(size >= 4) {
				stream.Write(BitConverter.GetBytes(StreamOffset), 0, 4);
				size -= 4;
				if(size >= 2) {
					stream.Write(BitConverter.GetBytes(StringOffset), 0, 2);
					size -= 2;
					if(size >= 2) {
						stream.Write(BitConverter.GetBytes(Reserved), 0, 2);
						size -= 2;
					}
				}
			}
		}
	}
	[CLSCompliant(false)]
	public class ExtSST {
		public ushort RecType;
		public ushort DataSize;
		public ushort StringPerBlock;
		public ExtSSTBlock[] Data;
		public ExtSST() {
			Data = new ExtSSTBlock[256];
			for(int i = 0; i < 256; i++)
				Data[i] = new ExtSSTBlock();
		}
		public void WriteToStream(XlsStream stream) {
			stream.Write(BitConverter.GetBytes(RecType), 0, 2);
			stream.Write(BitConverter.GetBytes(DataSize), 0, 2);
			int size = DataSize;
			if(size >= 2) {
				stream.Write(BitConverter.GetBytes(StringPerBlock), 0, 2);
				size -= 2;
				if(size > 0) {
					for(int i = 0; i < 256; i++) {
						Data[i].WriteToStream(stream, ref size);
						if(size <= 0)
							break;
					}
				}
			}
		}
	}
	[CLSCompliant(false)]
	public class SSTStringInfo {
		public ushort HashCode;
		public ushort StrSize;
		public ushort Block;
		public ushort Offset;
	}
}
