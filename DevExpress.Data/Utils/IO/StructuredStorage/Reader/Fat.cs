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
namespace DevExpress.Utils.StructuredStorage.Internal.Reader {
	#region AbstractFat (abstract class)
	[CLSCompliant(false)]
	public abstract class AbstractFat {
		#region Fields
		readonly Header header;
		readonly InputHandler fileHandler;
		readonly int addressesPerSector;
		readonly UInt64 maxSectorsInFile;
		#endregion
		protected AbstractFat(Header header, InputHandler fileHandler) {
			Guard.ArgumentNotNull(header, "header");
			Guard.ArgumentNotNull(fileHandler, "fileHandler");
			this.header = header;
			this.fileHandler = fileHandler;
			this.addressesPerSector = (int)header.SectorSize / 4;
			this.maxSectorsInFile = (fileHandler.IOStreamSize + SectorSize - 1) / SectorSize;
		}
		#region Properties
		public Header Header { get { return header; } }
		public int AddressesPerSector { get { return addressesPerSector; } }
		public InputHandler FileHandler { get { return fileHandler; } }
		#endregion
		public List<UInt32> GetSectorChain(UInt32 startSector, UInt64 maxCount, string name) {
			return GetSectorChain(startSector, maxCount, name, false);
		}
		public List<UInt32> GetSectorChain(UInt32 startSector, UInt64 maxCount, string name, bool immediateCycleCheck) {
			List<UInt32> result = new List<UInt32>();
			result.Add(startSector);
			while (true) {
				UInt32 nextSectorInStream = this.GetNextSectorInChain(result[result.Count - 1]);
				if (nextSectorInStream == SectorType.Dif || nextSectorInStream == SectorType.Fat || nextSectorInStream == SectorType.Free)
					InvalidSectorInChainException();
				if (nextSectorInStream == SectorType.EndOfChain)
					break;
				if (immediateCycleCheck)
					if (result.Contains(nextSectorInStream))
						ThrowCycleDetectedException(name);
				result.Add(nextSectorInStream);
				if ((UInt64)(result.Count) > maxSectorsInFile)
					break;
			}
			return result;
		}
		internal void ThrowCycleDetectedException(string chain) {
			throw new Exception(chain + " contains a cycle.");
		}
		internal void ThrowChainSizeMismatchException(string name) {
			throw new Exception("The number of sectors used by " + name + " does not match the specified size.");
		}
		internal void InvalidSectorInChainException() {
			throw new Exception("Chain could not be build due to an invalid sector id.");
		}
		internal int UncheckedRead(byte[] array, int offset, int count) {
			return fileHandler.UncheckedRead(array, offset, count);
		}
		protected abstract UInt32 GetNextSectorInChain(UInt32 currentSector);
		public abstract long SeekToPositionInSector(long sector, long position);
		public abstract UInt16 SectorSize { get; }
	}
	#endregion
	#region Fat
	[CLSCompliant(false)]
	public class Fat : AbstractFat {
		#region Fields
		readonly List<UInt32> sectorsUsedByFat = new List<UInt32>();
		readonly List<UInt32> sectorsUsedByDiFat = new List<UInt32>();
		#endregion
		public Fat(Header header, InputHandler fileHandler)
			: base(header, fileHandler) {
			Init();
		}
		public override UInt16 SectorSize { get { return Header.SectorSize; } }
		public override long SeekToPositionInSector(long sector, long position) {
			return FileHandler.SeekToPositionInSector(sector, position);
		}
		protected override UInt32 GetNextSectorInChain(UInt32 currentSector) {
			UInt32 sectorInFile = sectorsUsedByFat[(int)(currentSector / AddressesPerSector)];
			FileHandler.SeekToPositionInSector(sectorInFile, 4 * (currentSector % AddressesPerSector));
			return FileHandler.ReadUInt32();
		}
		void Init() {
			ReadFirst109SectorsUsedByFAT();
			ReadSectorsUsedByFatFromDiFat();
			CheckConsistency();
		}
		void ReadFirst109SectorsUsedByFAT() {
			const int HeaderSector = -1;
			FileHandler.SeekToPositionInSector(HeaderSector, 0x4C);
			UInt32 fatSector;
			for (int i = 0; i < 109; i++) {
				fatSector = FileHandler.ReadUInt32();
				if (fatSector == SectorType.Free)
					break;
				sectorsUsedByFat.Add(fatSector);
			}
		}
		void ReadSectorsUsedByFatFromDiFat() {
			if (Header.DiFatStartSector == SectorType.EndOfChain || Header.NoSectorsInDiFatChain == 0x0)
				return;
			FileHandler.SeekToSector(Header.DiFatStartSector);
			bool lastFatSectorFound = false;
			sectorsUsedByDiFat.Add(Header.DiFatStartSector);
			while (true) {
				for (int i = 0; i < AddressesPerSector - 1; i++) {
					UInt32 fatSector = FileHandler.ReadUInt32();
					if (fatSector == SectorType.Free) {
						lastFatSectorFound = true;
						break;
					}
					sectorsUsedByFat.Add(fatSector);
				}
				if (lastFatSectorFound)
					break;
				UInt32 nextDiFatSector = FileHandler.ReadUInt32();
				if (nextDiFatSector == SectorType.Free || nextDiFatSector == SectorType.EndOfChain)
					break;
				sectorsUsedByDiFat.Add(nextDiFatSector);
				FileHandler.SeekToSector(nextDiFatSector);
				if (sectorsUsedByDiFat.Count > Header.NoSectorsInDiFatChain)
					ThrowChainSizeMismatchException("DiFat");
			}
		}
		void CheckConsistency() {
			if (sectorsUsedByDiFat.Count != Header.NoSectorsInDiFatChain || sectorsUsedByFat.Count != Header.NoSectorsInFatChain)
				ThrowChainSizeMismatchException("Fat/DiFat");
		}
	}
	#endregion
	#region MiniFat
	[CLSCompliant(false)]
	public class MiniFat : AbstractFat {
		#region Fields
		List<UInt32> sectorsUsedByMiniFat = new List<UInt32>();
		List<UInt32> sectorsUsedByMiniStream = new List<UInt32>();
		readonly Fat fat;
		readonly UInt32 miniStreamStart;
		readonly UInt64 sizeOfMiniStream;
		#endregion
		internal MiniFat(Fat fat, Header header, InputHandler fileHandler, UInt32 miniStreamStart, UInt64 sizeOfMiniStream)
			: base(header, fileHandler) {
			Guard.ArgumentNotNull(fat, "fat");
			this.fat = fat;
			this.miniStreamStart = miniStreamStart;
			this.sizeOfMiniStream = sizeOfMiniStream;
			Init();
		}
		public override UInt16 SectorSize { get { return Header.MiniSectorSize; } }
		public override long SeekToPositionInSector(long sector, long position) {
			int sectorInMiniStreamChain = (int)((sector * Header.MiniSectorSize) / fat.SectorSize);
			int offsetInSector = (int)((sector * Header.MiniSectorSize) % fat.SectorSize);
			if (position < 0)
				throw new ArgumentOutOfRangeException("position");
			return FileHandler.SeekToPositionInSector(sectorsUsedByMiniStream[sectorInMiniStreamChain], offsetInSector + position);
		}
		protected override UInt32 GetNextSectorInChain(UInt32 currentSector) {
			if (sectorsUsedByMiniFat.Count == 0)
				return SectorType.EndOfChain;
			UInt32 sectorInFile = sectorsUsedByMiniFat[(int)(currentSector / AddressesPerSector)];
			FileHandler.SeekToPositionInSector(sectorInFile, 4 * ((int)currentSector % AddressesPerSector));
			return FileHandler.ReadUInt32();
		}
		void Init() {
			ReadSectorsUsedByMiniFAT();
			ReadSectorsUsedByMiniStream();
			CheckConsistency();
		}
		void ReadSectorsUsedByMiniFAT() {
			if (Header.MiniFatStartSector == SectorType.EndOfChain || Header.NoSectorsInMiniFatChain == 0x0)
				return;
			sectorsUsedByMiniFat = fat.GetSectorChain(Header.MiniFatStartSector, Header.NoSectorsInMiniFatChain, "MiniFat");
		}
		void ReadSectorsUsedByMiniStream() {
			if (miniStreamStart == SectorType.EndOfChain || sizeOfMiniStream == 0)
				return;
			try {
				sectorsUsedByMiniStream = fat.GetSectorChain(miniStreamStart, (UInt64)Math.Ceiling((double)sizeOfMiniStream / Header.SectorSize), "MiniStream");
			}
			catch {
				sectorsUsedByMiniStream = fat.GetSectorChain(miniStreamStart, 1 + (UInt64)Math.Ceiling((double)sizeOfMiniStream / Header.SectorSize), "MiniStream");
			}
		}
		void CheckConsistency() {
			if(sectorsUsedByMiniFat.Count != Header.NoSectorsInMiniFatChain && sizeOfMiniStream != 0)
				ThrowChainSizeMismatchException("MiniFat");
			if(sectorsUsedByMiniStream.Count != Math.Ceiling((double)sizeOfMiniStream / Header.SectorSize) && sectorsUsedByMiniStream.Count != 1 + Math.Ceiling((double)sizeOfMiniStream / Header.SectorSize))
				ThrowChainSizeMismatchException("MiniStream");
		}
	}
	#endregion
}
