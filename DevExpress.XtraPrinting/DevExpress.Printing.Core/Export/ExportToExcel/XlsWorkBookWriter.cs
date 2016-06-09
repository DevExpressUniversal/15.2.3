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
	[CLSCompliant(false)]
	public class XlsWorkBookWriter {
		#region OLE consts
		const ulong oleSignature = 0xE11AB1A1E011CFD0;
		const uint oleDifBlock = 0xFFFFFFFC;
		const uint oleSpecBlock = 0xFFFFFFFD;
		const uint oleEndOfChain = 0xFFFFFFFE;
		const uint oleUnused = 0xFFFFFFFF;
		const uint oleEmpty = 0x00000000;
		const uint oleDllVersion = 0x0003003E;
		const ushort olePlatformOrder = 0xFFFE;
		const int oleSectorsInMasterFat = 109;
		const int oleBlockIdPerBigBlock = 128;
		const int oleMaxBlockIdInBigBlock = 127;
		const int oleBigBlockShift = 9;
		const int oleSmallBlockShift = 6;
		const int oleReservedSectorCount = 2;
		const uint oleMiniSectorMaxSize = 0x000001000;
		const int oleSmallBlockSize = 1 << oleSmallBlockShift;
		const int oleBigBlockSize = 1 << oleBigBlockShift;
		const int oleDirBlockSize = 128;
		const int oleIndexSize = 4;
		const string oleRoot = "Root Entry";
		const string oleWorkBook = "Workbook";
		#endregion
		DynamicByteBuffer buffer;
		int bufferSize;
		int streamSize;
		int sectCount;
		bool isSmallFile;
		readonly OleFileHeader headerTemplate;
		public XlsWorkBookWriter() {
			buffer = new DynamicByteBuffer();
			#region Ole header template
			headerTemplate = new OleFileHeader();
			headerTemplate.Signature = oleSignature;
			headerTemplate.ClsId[0] = oleEmpty;
			headerTemplate.ClsId[1] = oleEmpty;
			headerTemplate.OleVersion = oleDllVersion;
			headerTemplate.ByteOrder = olePlatformOrder;
			headerTemplate.SectorShift = oleBigBlockShift;
			headerTemplate.MiniSectorShift = oleSmallBlockShift;
			headerTemplate.Reserved = (ushort)oleEmpty;
			headerTemplate.Reserved1 = oleEmpty;
			headerTemplate.Reserved2 = oleEmpty;
			headerTemplate.CountSectFat = 1;
			headerTemplate.SectDirStart = 1;
			headerTemplate.TransSignature = oleEmpty;
			headerTemplate.MiniSectorCutOff = oleMiniSectorMaxSize;
			headerTemplate.SectMiniFatStart = oleEndOfChain;
			headerTemplate.CountSectMiniFat = oleEmpty;
			headerTemplate.SectDifStart = oleEndOfChain;
			headerTemplate.CountSectDif = oleEmpty;
			#endregion
		}
		void Check(bool condition) {
			if(!condition)
				throw new Exception("WorkBook exception");
		}
		int RoundDiv(int number, int denominator) {
			int result;
			result = number / denominator;
			if(number % denominator != 0)
				result++;
			return result;
		}
		int GetDirEntryOffset(int index) {
			return ((oleReservedSectorCount << oleBigBlockShift) + index * oleDirBlockSize);
		}
		int GetDifSectorOffset(int sector) {
			return (GetSectDifStart() +
				(sector * oleBlockIdPerBigBlock) + 1) * 512;
		}
		int GetFatSectorOffset(int sector) {
			if(sector == 0)
				return 512;
			else {
				if(sector < oleSectorsInMasterFat)
					return (sector + 2) * 512;
				else {
					sector -= oleSectorsInMasterFat;
					int difBlock = 0;
					while((sector - oleMaxBlockIdInBigBlock) >= 0) {
						sector -= oleMaxBlockIdInBigBlock;
						difBlock++;
					}
					return
						(BitConverter.ToInt32(
						buffer.GetElements(
						GetDifSectorOffset(difBlock) + sector * 4, 4), 0) + 1) * 512;
				}
			}
		}
		int GetCountSectFat() {
			return BitConverter.ToInt32(buffer.GetElements(44, 4), 0);
		}
		int GetCountSectDif() {
			return BitConverter.ToInt32(buffer.GetElements(72, 4), 0);
		}
		int GetSectDifStart() {
			return BitConverter.ToInt32(buffer.GetElements(68, 4), 0);
		}
		int GetDirStartSector(int offset) {
			return BitConverter.ToInt32(buffer.GetElements(offset + 116, 4), 0);
		}
		void IncCurrentIndexAndSetValue(uint value_, ref int index, ref int curSector, ref int sector) {
			buffer.SetElements(curSector + index * 4, BitConverter.GetBytes(value_));
			if(index == oleMaxBlockIdInBigBlock) {
				sector++;
				curSector = GetFatSectorOffset(sector);
				index = 0;
			}
			else
				index++;
		}
		void CreateEntry(string name, OleDirEntryType type, OleDirEntry entry, int offset) {
			entry.EntryType = type;
			entry.BFlag = Convert.ToByte(type == OleDirEntryType.Stream);
			entry.LeftSib = oleUnused;
			entry.RightSib = oleUnused;
			entry.ChildSib = oleUnused;
			if(type == OleDirEntryType.Stream || type == OleDirEntryType.Root) {
				entry.NameLen = Convert.ToUInt16((name.Length + 1) << 1);
				if(entry.NameLen != 1)
					for(int i = 0; i < (entry.NameLen - 2) >> 1; i++)
						entry.Name[i] = name[i];
			}
			entry.CopyToBuffer(offset, buffer);
		}
		void ReallocBuffer(int size) {
			bufferSize = (int)((RoundDiv(size, (int)oleMiniSectorMaxSize) + 1) * oleMiniSectorMaxSize);
			try {
				buffer.Realloc(bufferSize);
			}
			finally {
				bufferSize = size;
			}
		}
		void CreateHeader() {
			int offset = headerTemplate.CopyToBuffer(0, buffer);
			buffer.FillElements(offset, 109 * 4, (byte)0xFF); 
			if(!isSmallFile) {
				int countSectFat = RoundDiv(sectCount + 3, oleMaxBlockIdInBigBlock);
				buffer.SetElements(44, 
					BitConverter.GetBytes(countSectFat));
				int countSectDif = GetCountSectDif();
				if(countSectFat > oleSectorsInMasterFat) {
					int count = countSectFat - oleSectorsInMasterFat;
					countSectDif = RoundDiv(count, oleMaxBlockIdInBigBlock);
					buffer.SetElements(72, 
						BitConverter.GetBytes(countSectDif));
					buffer.SetElements(68, 
						BitConverter.GetBytes(oleSectorsInMasterFat + oleReservedSectorCount));
				}
				ReallocBuffer((countSectFat + countSectDif + oleReservedSectorCount) << oleBigBlockShift);
			}
			else {
				buffer.SetElements(60, 
					BitConverter.GetBytes((uint)2));
				buffer.SetElements(64, 
					BitConverter.GetBytes((uint)1));
			}
		}
		void CreateDif() {
			int index = 0;
			int curSect = 0;
			int curDif = GetDifSectorOffset(curSect);
			int id = 0;
			for(int i = oleSectorsInMasterFat - 1; i < GetCountSectFat() - 1; i++) {
				int sectorId = i - 108;
				if(index == oleMaxBlockIdInBigBlock) {
					buffer.SetElements(curDif + oleMaxBlockIdInBigBlock * 4,
						BitConverter.GetBytes((uint)(sectorId + 111 + id)));
					curDif = GetDifSectorOffset(curSect + 1);
					index = 0;
					curSect++;
				}
				if(((sectorId + id - 1) % oleBlockIdPerBigBlock) == 0)
					id++;
				buffer.SetElements(curDif + index * 4,
					BitConverter.GetBytes((uint)(sectorId + 110 + id)));
				index++;
			}
			buffer.FillElements(curDif + index * 4, (oleBlockIdPerBigBlock - index) * oleIndexSize, (byte)0xFF);
		}
		void CreateDir() {
			int dirOffset_0 = GetDirEntryOffset(0);
			int dirOffset_1 = GetDirEntryOffset(1);
			buffer.FillElements(dirOffset_0, oleBigBlockSize, Convert.ToByte(oleEmpty));
			CreateEntry(oleRoot, OleDirEntryType.Root, new OleDirEntry(),
				dirOffset_0);
			CreateEntry(oleWorkBook, OleDirEntryType.Stream, new OleDirEntry(),
				dirOffset_1);
			buffer.SetElements(dirOffset_0 + 76, 
				BitConverter.GetBytes((uint)1));
			if(!isSmallFile) {
				buffer.SetElements(dirOffset_0 + 116, 
					BitConverter.GetBytes(oleEndOfChain));
				int countSectFat = GetCountSectFat();
				int countSectDif = GetCountSectDif();
				buffer.SetElements(dirOffset_1 + 116, 
					BitConverter.GetBytes(countSectFat + countSectDif + 1));
			}
			else {
				buffer.SetElements(dirOffset_0 + 116, 
					BitConverter.GetBytes((int)3));
				buffer.SetElements(dirOffset_0 + 120, 
					BitConverter.GetBytes(sectCount << oleBigBlockShift));
			}
			buffer.SetElements(dirOffset_1 + 120, 
				BitConverter.GetBytes(streamSize));
		}
		void CreateFat() {
			if(!isSmallFile) {
				int countSectFat = GetCountSectFat();
				for(int i = 0; i < Math.Min(countSectFat, oleSectorsInMasterFat); i++) {
					if(i == 0)
						buffer.SetElements(76, 
							BitConverter.GetBytes((uint)0));
					else
						buffer.SetElements(76 + (i * 4), 
							BitConverter.GetBytes(i + 1));
				}
				if(GetCountSectDif() > 0)
					CreateDif();
				CreateLocalFat();
			}
			else {
				buffer.SetElements(76, 
					BitConverter.GetBytes((uint)0));
				CreateSmallFat();
			}
		}
		void CreateLocalFat() {
			int index = 0;
			int sector = 0;
			int dif = 0;
			int curSector = GetFatSectorOffset(sector);
			IncCurrentIndexAndSetValue(oleSpecBlock,
				ref index, ref curSector, ref sector);
			IncCurrentIndexAndSetValue(oleEndOfChain,
				ref index, ref curSector, ref sector);
			for(int i = 1; i < GetCountSectFat() + GetCountSectDif(); i++) {
				if(GetCountSectDif() > 0) {
					if((dif + GetSectDifStart() - 1) == i) {
						dif += oleBlockIdPerBigBlock;
						IncCurrentIndexAndSetValue(oleDifBlock,
							ref index, ref curSector, ref sector);
						continue;
					}
				}
				IncCurrentIndexAndSetValue(oleSpecBlock,
					ref index, ref curSector, ref sector);
			}
			int dirOffset = GetDirEntryOffset(1);
			for(int i = GetDirStartSector(dirOffset) + 1; i < GetDirStartSector(dirOffset) + sectCount; i++)
				IncCurrentIndexAndSetValue((uint)i, ref index, ref curSector, ref sector);
			IncCurrentIndexAndSetValue(oleEndOfChain,
				ref index, ref curSector, ref sector);
			if(index != 0) {
				int i = oleBlockIdPerBigBlock - index;
				if(i > 0)
					buffer.FillElements(curSector + index * 4, i * 4, (byte)0xFF);
			}
		}
		void CreateSmallFat() {
			int bigFatOffset = oleBigBlockSize;
			int smallFatOffset = 3 << oleBigBlockShift;
			int blockCount = RoundDiv(streamSize, oleSmallBlockSize);
			buffer.FillElements(bigFatOffset, oleBigBlockSize, (byte)0xFF); 
			buffer.SetElements(bigFatOffset, BitConverter.GetBytes(oleSpecBlock));
			buffer.SetElements(bigFatOffset + 4, BitConverter.GetBytes(oleEndOfChain));
			buffer.SetElements(bigFatOffset + 8, BitConverter.GetBytes(oleEndOfChain));
			int i = 3;
			while((i - 3) < (sectCount - 1)) {
				buffer.SetElements(bigFatOffset + (i * 4), BitConverter.GetBytes((uint)(i + 1)));
				i++;
			}
			buffer.SetElements(bigFatOffset + (i * 4), BitConverter.GetBytes(oleEndOfChain));
			for(i = 0; i < blockCount - 1; i++)
				buffer.SetElements(smallFatOffset + (i * 4), BitConverter.GetBytes((uint)(i + 1)));
			buffer.SetElements(smallFatOffset + ((blockCount - 1) * 4), BitConverter.GetBytes(oleEndOfChain));
			buffer.FillElements(smallFatOffset + blockCount * 4,
				(oleBlockIdPerBigBlock - blockCount) * oleIndexSize, (byte)0xFF);
		}
		public void CreateOleStream(int dataSize, XlsStream dstStream) {
			Check(dataSize > 0 && dstStream != null);
			streamSize = dataSize;
			sectCount = RoundDiv(streamSize, oleBigBlockSize);
			int size = RoundDiv(sectCount, oleBlockIdPerBigBlock) + 3;
			isSmallFile = streamSize < oleMiniSectorMaxSize;
			if(!isSmallFile)
				ReallocBuffer(oleBigBlockSize * (size + RoundDiv(size, oleMaxBlockIdInBigBlock)));
			else
				ReallocBuffer(4 << oleBigBlockShift);
			CreateHeader();
			CreateDir();
			CreateFat();
			buffer.WriteToStream(dstStream, bufferSize);
		}
	}
	[CLSCompliant(false)]
	public class OleFileHeader {
		public ulong Signature;
		public ulong[] ClsId = new ulong[2];
		public uint OleVersion;
		public ushort ByteOrder;
		public ushort SectorShift;
		public ushort MiniSectorShift;
		public ushort Reserved;
		public uint Reserved1;
		public uint Reserved2;
		public uint CountSectFat;
		public uint SectDirStart;
		public uint TransSignature;
		public uint MiniSectorCutOff;
		public uint SectMiniFatStart;
		public uint CountSectMiniFat;
		public uint SectDifStart;
		public uint CountSectDif;
		public uint[] SectFat = new uint[109];
		public int CopyToBuffer(int offset, DynamicByteBuffer buffer) {
			int result = 0;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Signature));
			result += 8;
			buffer.SetElements(offset + result, BitConverter.GetBytes(ClsId[0]));
			buffer.SetElements(offset + result, BitConverter.GetBytes(ClsId[1]));
			result += 16;
			buffer.SetElements(offset + result, BitConverter.GetBytes(OleVersion));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(ByteOrder));
			result += 2;
			buffer.SetElements(offset + result, BitConverter.GetBytes(SectorShift));
			result += 2;
			buffer.SetElements(offset + result, BitConverter.GetBytes(MiniSectorShift));
			result += 2;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Reserved));
			result += 2;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Reserved1));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Reserved2));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(CountSectFat));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(SectDirStart));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(TransSignature));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(MiniSectorCutOff));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(SectMiniFatStart));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(CountSectMiniFat));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(SectDifStart));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(CountSectDif));
			result += 4;
			return result;
		}
	}
	public enum OleDirEntryType {
		Invalid,
		Storage,
		Stream,
		LockBytes,
		Property,
		Root
	}
	[CLSCompliant(false)]
	public class OleDirEntry {
		public char[] Name = new char[32];
		public ushort NameLen;
		public OleDirEntryType EntryType;
		public byte BFlag;
		public uint LeftSib;
		public uint RightSib;
		public uint ChildSib;
		public Guid Guid_;
		public int UserFlag;
		public ulong C_Time;
		public ulong M_Time;
		public int StartSector;
		public int Size;
		public int Reserved;
		public int CopyToBuffer(int offset, DynamicByteBuffer buffer) {
			int result = 0;
			for(int i = 0; i < 32; i++) {
				buffer.SetElements(offset + result, BitConverter.GetBytes(Name[i]));
				result += 2;
			}
			buffer.SetElements(offset + result, BitConverter.GetBytes(NameLen));
			result += 2;
			buffer.SetElement(offset + result, (byte)EntryType);
			result++;
			buffer.SetElement(offset + result, BFlag);
			result++;
			buffer.SetElements(offset + result, BitConverter.GetBytes(LeftSib));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(RightSib));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(ChildSib));
			result += 4;
			buffer.SetElements(offset + result, Guid_.ToByteArray());
			result += 16;
			buffer.SetElements(offset + result, BitConverter.GetBytes(UserFlag));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(C_Time));
			result += 8;
			buffer.SetElements(offset + result, BitConverter.GetBytes(M_Time));
			result += 8;
			buffer.SetElements(offset + result, BitConverter.GetBytes(StartSector));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Size));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Reserved));
			result += 4;
			return result;
		}
	}
}
