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
	#region Formatted Disk Page (abstract class)
	public abstract class FormattedDiskPageBase {
		readonly List<int> fc;
		readonly List<byte> innerOffsets;
		protected FormattedDiskPageBase() {
			this.fc = new List<int>();
			this.innerOffsets = new List<byte>();
		}
		protected List<int> FC { get { return fc; } }
		protected List<byte> InnerOffsets { get { return innerOffsets; } }
		protected virtual void Read(BinaryReader reader, int offset) {
			Guard.ArgumentNotNull(reader, "reader");
			fc.AddRange(SectorHelper.GetBorders(reader, offset));
			ReadInnerOffsets(reader);
		}
		public int GetFirstOffset() {
			return fc[0];
		}
		public void AddLastPosition(int filePosition) {
			fc.Add(filePosition);
		}
		public byte GetInnerOffset(int fc) {
			if (InnerOffsets.Count == 0)
				return 0;
			int index = Math.Min(CalculateIndex(fc), InnerOffsets.Count - 1);
			return InnerOffsets[index];
		}
		protected int CalculateIndex(int fc) {
			int position = FC.BinarySearch(fc);
			if (position < 0)
				position = Math.Max(~position - 1, 0);
			return position;
		}
		public virtual void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			long startPosition = writer.BaseStream.Position;
			WriteFilePositions(writer);
			WriteInnerOffsets(writer);
			WriteByteProperties(writer, startPosition);
			writer.BaseStream.Seek(startPosition + DocConstants.LastByteOffsetInSector, SeekOrigin.Begin);
			writer.Write((byte)InnerOffsets.Count);
		}
		protected void WriteFilePositions(BinaryWriter writer) {
			int count = FC.Count;
			for (int i = 0; i < count; i++) {
				writer.Write(FC[i]);
			}
		}
		protected abstract void ReadInnerOffsets(BinaryReader reader);
		protected abstract void WriteInnerOffsets(BinaryWriter writer);
		protected abstract void WriteByteProperties(BinaryWriter writer, long startPosition);
	}
	#endregion
	#region Formatted Disk Page for PAPXs
	public class PAPXFormattedDiskPage : FormattedDiskPageBase {
		#region static
		public static PAPXFormattedDiskPage FromStream(BinaryReader reader, int offset) {
			PAPXFormattedDiskPage result = new PAPXFormattedDiskPage();
			result.Read(reader, offset);
			return result;
		}
		#endregion
		#region Fields
		const int basePageSize = 510;
		const int styleDescriptorSize = 2;
		const int padByteSize = 1;
		const int sizeOfGrpprlSizeAndStyleDescriptor = 1;
		const int paragraphHeightSize = 12;
		const int bxSize = 13;
		List<int> styleIndexes;
		List<byte[]> grppapx;
		#endregion
		public PAPXFormattedDiskPage() {
			this.styleIndexes = new List<int>();
			this.grppapx = new List<byte[]>();
		}
		protected override void ReadInnerOffsets(BinaryReader reader) {
			int count = FC.Count - 1;
			for (int i = 0; i < count; i++) {
				InnerOffsets.Add(reader.ReadByte());
				reader.BaseStream.Seek(paragraphHeightSize, SeekOrigin.Current);
			}
		}
		public bool TryToAddGrpprlAndPosition(int filePosition, int styleIndex, byte[] grpprl) {
			int papxLength = grpprl.Length + sizeOfGrpprlSizeAndStyleDescriptor + styleDescriptorSize;
			if (grpprl.Length % 2 == 0)
				papxLength += padByteSize;
			int grpprlOffset = (InnerOffsets.Count == 0) ? basePageSize - papxLength : (InnerOffsets[InnerOffsets.Count - 1] * 2) - papxLength;
			if (grpprlOffset < (DocConstants.CharacterPositionSize + bxSize) * (InnerOffsets.Count + 1) + DocConstants.CharacterPositionSize)
				return false;
			FC.Add(filePosition);
			InnerOffsets.Add((byte)(grpprlOffset / 2));
			styleIndexes.Add(styleIndex);
			grppapx.Add(grpprl);
			return true;
		}
		protected override void WriteInnerOffsets(BinaryWriter writer) {
			int count = InnerOffsets.Count;
			byte[] reserved = new byte[paragraphHeightSize];
			for (int i = 0; i < count; i++) {
				writer.Write(InnerOffsets[i]);
				writer.Write(reserved);
			}
		}
		protected override void WriteByteProperties(BinaryWriter writer, long startPosition) {
			int count = grppapx.Count;
			for (int i = 0; i < count; i++) {
				writer.BaseStream.Seek(startPosition + InnerOffsets[i] * 2, SeekOrigin.Begin);
				if (grppapx[i].Length % 2 == 0) {
					writer.Write((byte)0);
					writer.Write((byte)((grppapx[i].Length + styleDescriptorSize) / 2));
				} 
				else
					writer.Write((byte)((grppapx[i].Length + styleDescriptorSize + padByteSize) / 2));
				writer.Write((ushort)styleIndexes[i]);
				writer.Write(grppapx[i]);
			}
		}
	}
	#endregion
	#region Formatted Disk Page for CHPXs
	public class CHPXFormattedDiskPage : FormattedDiskPageBase {
		const int filePositionSize = 4;
		const int lastFilePositionSize = 4;
		const int innerOffsetSize = 1;
		const int sizeOfGrpprlSize = 1;
		#region Fields
		List<byte[]> grpchpx;
		int lastActiveInnerOffset = DocConstants.LastByteOffsetInSector;
		#endregion
		public static CHPXFormattedDiskPage FromStream(BinaryReader reader, int offset) {
			CHPXFormattedDiskPage result = new CHPXFormattedDiskPage();
			result.Read(reader, offset);
			return result;
		}
		public CHPXFormattedDiskPage() {
			this.grpchpx = new List<byte[]>();
		}
		protected int LastActiveInnerOffset { get { return lastActiveInnerOffset; } set { lastActiveInnerOffset = value; } }
		protected override void ReadInnerOffsets(BinaryReader reader) {
			int count = FC.Count - 1;
			for (int i = 0; i < count; i++) {
				InnerOffsets.Add(reader.ReadByte());
			}
		}
		public bool TryToAddGrpprlAndPosition(int filePosition, byte[] grpprl) {
			int chpxLength = (grpprl.Length == 0) ? 0 : grpprl.Length + sizeOfGrpprlSize;
			int grpprlOffset = (grpprl.Length == 0) ? 0 : (LastActiveInnerOffset - chpxLength);
			if (grpprlOffset % 2 != 0)
				grpprlOffset--;
			if (grpprlOffset != 0)
				LastActiveInnerOffset = grpprlOffset;
			if (LastActiveInnerOffset < (filePositionSize + innerOffsetSize) * (InnerOffsets.Count + 1) + lastFilePositionSize)
				return false;
			FC.Add(filePosition);
			InnerOffsets.Add((byte)(grpprlOffset / 2));
			grpchpx.Add(grpprl);
			return true;
		}
		protected override void WriteInnerOffsets(BinaryWriter writer) {
			int count = InnerOffsets.Count;
			for (int i = 0; i < count; i++) {
				writer.Write(InnerOffsets[i]);
			}
		}
		protected override void WriteByteProperties(BinaryWriter writer, long startPosition) {
			int count = grpchpx.Count;
			for (int i = 0; i < count; i++) {
				writer.BaseStream.Seek(startPosition + InnerOffsets[i] * 2, SeekOrigin.Begin);
				writer.Write((byte)grpchpx[i].Length);
				writer.Write(grpchpx[i]);
			}
		}
	}
	#endregion
}
