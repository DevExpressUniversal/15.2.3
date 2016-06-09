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
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class BinTable {
		#region static
		public static BinTable FromStream(BinaryReader reader, int offset, int size, long streamLength) {
			BinTable result = new BinTable();
			result.Read(reader, offset, size, streamLength);
			return result;
		}
		#endregion
		#region Fields
		const int positionSize = 4;
		const int offsetSize = 4;
		const int sectorOffsetBitMask = 0x3fffff;
		List<int> positions;
		List<int> sectorsOffsets;
		#endregion
		public BinTable() {
			this.positions = new List<int>();
			this.sectorsOffsets = new List<int>();
		}
		protected void Read(BinaryReader reader, int offset, int size, long streamLength) {
			Guard.ArgumentNotNull(reader, "reader");
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int binEntriesCount = (size - positionSize) / (positionSize + offsetSize);
			this.positions.AddRange(GetPositions(reader, binEntriesCount + 1));
			this.sectorsOffsets.AddRange(GetSectorsOffsets(reader, binEntriesCount, streamLength));
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			int count = positions.Count;
			for (int i = 0; i < count; i++) {
				writer.Write((uint)positions[i]);
			}
			count = sectorsOffsets.Count;
			for (int i = 0; i < count; i++) {
				writer.Write((uint)(sectorsOffsets[i] / DocContentBuilder.SectorSize));
			}
		}
		int[] GetPositions(BinaryReader reader, int count) {
			int[] positions = new int[count];
			for (int i = 0; i < count; i++) {
				positions[i] = reader.ReadInt32();
			}
			return positions;
		}
		int[] GetSectorsOffsets(BinaryReader reader, int count, long streamLength) {
			int[] sectorsOffsets = new int[count];
			for (int i = 0; i < count; i++) {
				int currentOffset = DocContentBuilder.SectorSize * (reader.ReadInt32() & sectorOffsetBitMask);
				if (currentOffset < streamLength) {
					sectorsOffsets[i] = currentOffset;
					continue;
				}
				if (i > 0)
					sectorsOffsets[i] = sectorsOffsets[i - 1] + DocContentBuilder.SectorSize;
			}
			return sectorsOffsets;
		}
		public List<int> GetBorders(BinaryReader reader) {
			List<int> borders = new List<int>();
			int count = sectorsOffsets.Count;
			for (int i = 0; i < count; i++) {
				int sectorOffset = sectorsOffsets[i];
				borders.AddRange(SectorHelper.GetBorders(reader, sectorOffset));
			}
			return borders;
		}
		public int GetFKPOffset(int fc) {
			int position = positions.BinarySearch(fc);
			if (position < 0)
				position = ~position - 1;
			int fkpIndex = Math.Min(position, sectorsOffsets.Count - 1);
			return this.sectorsOffsets[fkpIndex];
		}
		public void AddEntry(int fcFirst, int fkpPageNumber) {
			positions.Add(fcFirst);
			sectorsOffsets.Add(fkpPageNumber);
		}
		public void AddLastPosition(int lastPosition) {
			positions.Add(lastPosition + 2);
		}
		public void UpdateSectorsOffsets(int offset) {
			int count = this.sectorsOffsets.Count;
			for (int i = 0; i < count; i++) {
				this.sectorsOffsets[i] += offset * DocContentBuilder.SectorSize;
			}
		}
	}
}
