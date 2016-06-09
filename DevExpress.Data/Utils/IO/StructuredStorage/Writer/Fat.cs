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
namespace DevExpress.Utils.StructuredStorage.Internal.Writer {
	#region AbstractFat (abstract class)
	[CLSCompliant(false)]
	public abstract class AbstractFat {
		readonly List<UInt32> entries = new List<UInt32>();
		readonly StructuredStorageContext context;
		UInt32 currentEntry;
		protected AbstractFat(StructuredStorageContext context) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
		}
		public List<UInt32> Entries { get { return entries; } }
		public UInt32 CurrentEntry { get { return currentEntry; } set { currentEntry = value; } }
		public StructuredStorageContext Context { get { return context; } }
		internal UInt32 WriteChain(UInt32 entryCount) {
			if (entryCount == 0)
				return SectorType.EndOfChain;
			UInt32 startSector = currentEntry;
			for (int i = 0; i < entryCount - 1; i++) {
				currentEntry++;
				entries.Add(currentEntry);
			}
			currentEntry++;
			entries.Add(SectorType.EndOfChain);
			return startSector;
		}
		internal abstract void Write();
	}
	#endregion
	#region Fat
	[CLSCompliant(false)]
	public class Fat : AbstractFat {
		#region Fields
		readonly List<UInt32> diFatEntries = new List<UInt32>();
		UInt32 numFatSectors;
		UInt32 numDiFatSectors;
		UInt32 diFatStartSector;
		#endregion
		public Fat(StructuredStorageContext context)
			: base(context) {
		}
		#region Properties
		internal UInt32 NumFatSectors { get { return numFatSectors; } }
		internal UInt32 NumDiFatSectors { get { return numDiFatSectors; } }
		internal UInt32 DiFatStartSector { get { return diFatStartSector; } }
		#endregion
		UInt32 WriteDiFatEntriesToFat(UInt32 sectorCount) {
			if (sectorCount == 0)
				return SectorType.EndOfChain;
			UInt32 startSector = CurrentEntry;
			for (int i = 0; i < sectorCount; i++) {
				CurrentEntry++;
				Entries.Add(SectorType.Dif);
			}
			return startSector;
		}
		void writeDiFatSectorsToStream(UInt32 fatStartSector) {
			for (UInt32 i = 0; i < numFatSectors; i++)
				diFatEntries.Add(fatStartSector + i);
			for (int i = 0; i < 109; i++) {
				if (i < diFatEntries.Count)
					Context.Header.WriteNextDiFatSector(diFatEntries[i]);
				else
					Context.Header.WriteNextDiFatSector(SectorType.Free);
			}
			if (diFatEntries.Count <= 109)
				return;
			List<UInt32> greaterDiFatEntries = new List<UInt32>();
			for (int i = 0; i < diFatEntries.Count - 109; i++)
				greaterDiFatEntries.Add(diFatEntries[i + 109]);
			UInt32 diFatLink = diFatStartSector + 1;
			int addressesInSector = Context.Header.SectorSize / 4;
			int sectorSplit = addressesInSector;
			while (greaterDiFatEntries.Count >= sectorSplit) {
				greaterDiFatEntries.Insert(sectorSplit - 1, diFatLink);
				diFatLink++;
				sectorSplit += addressesInSector;
			}
			for (int i = greaterDiFatEntries.Count; i % (Context.Header.SectorSize / 4) != 0; i++)
				greaterDiFatEntries.Add(SectorType.Free);
			greaterDiFatEntries.RemoveAt(greaterDiFatEntries.Count - 1);
			greaterDiFatEntries.Add(SectorType.EndOfChain);
			List<byte> output = Context.InternalBitConverter.GetBytes(greaterDiFatEntries);
			if (output.Count % Context.Header.SectorSize != 0)
				ThrowInconsistencyException();
			Context.TempOutputStream.WriteSectors(output.ToArray(), Context.Header.SectorSize, SectorType.Free);
		}
		internal override void Write() {
			numDiFatSectors = 0;
			while (true) {
				UInt32 numDiFatSectorsOld = numDiFatSectors;
				numFatSectors = (UInt32)Math.Ceiling((double)(Entries.Count * 4) / (double)Context.Header.SectorSize) + numDiFatSectors;
				numDiFatSectors = (numFatSectors <= 109) ? 0 : (UInt32)Math.Ceiling((double)((numFatSectors - 109) * 4) / (double)(Context.Header.SectorSize - 1));
				if (numDiFatSectorsOld == numDiFatSectors)
					break;
			}
			diFatStartSector = WriteDiFatEntriesToFat(numDiFatSectors);
			writeDiFatSectorsToStream(CurrentEntry);
			for (int i = 0; i < numFatSectors; i++)
				Entries.Add(SectorType.Fat);
			Context.TempOutputStream.WriteSectors((Context.InternalBitConverter.GetBytes(Entries)).ToArray(), Context.Header.SectorSize, SectorType.Free);
		}
		internal void ThrowInconsistencyException() {
			throw new Exception("Inconsistency found while writing DiFat.");
		}
	}
	#endregion
	#region MiniFat
	[CLSCompliant(false)]
	public class MiniFat : AbstractFat {
		UInt32 miniFatStart = SectorType.Free;
		UInt32 numMiniFatSectors = 0x0;
		public MiniFat(StructuredStorageContext context)
			: base(context) {
		}
		internal UInt32 MiniFatStart { get { return miniFatStart; } }
		internal UInt32 NumMiniFatSectors { get { return numMiniFatSectors; } }
		internal override void Write() {
			numMiniFatSectors = (UInt32)Math.Ceiling((double)(Entries.Count * 4) / (double)Context.Header.SectorSize);
			miniFatStart = Context.Fat.WriteChain(numMiniFatSectors);
			Context.TempOutputStream.WriteSectors(Context.InternalBitConverter.GetBytes(Entries).ToArray(), Context.Header.SectorSize, SectorType.Free);
		}
	}
	#endregion
}
