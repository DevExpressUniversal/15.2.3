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
	public class SectionPropertiesHelper {
		public static SectionPropertiesHelper FromStream(BinaryReader mainStreamReader, BinaryReader tableStreamReader, int offset, int size) {
			SectionPropertiesHelper result = new SectionPropertiesHelper();
			result.Read(mainStreamReader, tableStreamReader, offset, size);
			return result;
		}
		const int sectionPropertyDescriptorSize = 12;
		int currentSectionIndex;
		List<int> sectionStartPositions;
		List<int> sectionPropertyDescriptorOffsets;
		public SectionPropertiesHelper() {
			this.sectionStartPositions = new List<int>();
			this.sectionPropertyDescriptorOffsets = new List<int>();
		}
		protected void Read(BinaryReader mainStreamReader, BinaryReader tableReader, int offset, int size) {
			Guard.ArgumentNotNull(mainStreamReader, "mainStreamReader");
			Guard.ArgumentNotNull(tableReader, "tableReader");
			tableReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int sectionsCount = (size - DocConstants.CharacterPositionSize) / (DocConstants.CharacterPositionSize + sectionPropertyDescriptorSize);
			this.sectionStartPositions = GetSectionStartPositions(tableReader, sectionsCount + 1);
			this.sectionPropertyDescriptorOffsets = GetSectionPropertyDescriptorOffsets(tableReader, sectionsCount);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(sectionStartPositions[0]);
			for (int i = 1; i < sectionStartPositions.Count; i++) {
				if (sectionStartPositions[i] != sectionStartPositions[i - 1])
					writer.Write(sectionStartPositions[i]);
			}
			for (int i = 0; i < sectionPropertyDescriptorOffsets.Count; i++) {
				writer.BaseStream.Seek(2, SeekOrigin.Current);
				writer.Write(sectionPropertyDescriptorOffsets[i]);
				writer.Seek(6, SeekOrigin.Current);
			}
		}
		public void UpdateCurrentSectionProperties(BinaryReader mainStreamReader, BinaryReader dataStreamReader, DocPropertyContainer propertyContainer) {
			if (this.currentSectionIndex >= this.sectionPropertyDescriptorOffsets.Count || this.sectionPropertyDescriptorOffsets[this.currentSectionIndex] == -1)
				return;
			mainStreamReader.BaseStream.Seek(this.sectionPropertyDescriptorOffsets[this.currentSectionIndex], SeekOrigin.Begin);
			ushort grpprlSize = mainStreamReader.ReadUInt16();
			byte[] grpprl = mainStreamReader.ReadBytes(grpprlSize);
			DocCommandHelper.Traverse(grpprl, propertyContainer, dataStreamReader);
			this.currentSectionIndex++;
		}
		List<int> GetSectionStartPositions(BinaryReader reader, int count) {
			List<int> sectionStartPositions = new List<int>(count);
			for (int i = 0; i < count; i++) {
				sectionStartPositions.Add(reader.ReadInt32());
			}
			return sectionStartPositions;
		}
		List<int> GetSectionPropertyDescriptorOffsets(BinaryReader reader, int count) {
			List<int> sectionPropertyDescriptorOffsets = new List<int>(count);
			for (int i = 0; i < count; i++) {
				if (this.sectionStartPositions[i] == this.sectionStartPositions[i + 1]) {
					reader.BaseStream.Seek(sectionPropertyDescriptorSize, SeekOrigin.Current);
				}
				else {
					reader.BaseStream.Seek(2, SeekOrigin.Current);
					sectionPropertyDescriptorOffsets.Add(reader.ReadInt32());
					reader.BaseStream.Seek(6, SeekOrigin.Current);
				}
			}
			return sectionPropertyDescriptorOffsets;
		}
		public void AddEntry(int sectionStartPosition, int sepxOffset) {
			this.sectionStartPositions.Add(sectionStartPosition);
			this.sectionPropertyDescriptorOffsets.Add(sepxOffset);
		}
		public void AddLastPosition(int lastPosition) {
			this.sectionStartPositions.Add(lastPosition);
		}
		public void UpdateOffsets(int offset) {
			int count = this.sectionPropertyDescriptorOffsets.Count;
			for (int i = 0; i < count; i++) {
				if (this.sectionPropertyDescriptorOffsets[i] != -1)
					this.sectionPropertyDescriptorOffsets[i] += offset;
			}
		}
	}
}
