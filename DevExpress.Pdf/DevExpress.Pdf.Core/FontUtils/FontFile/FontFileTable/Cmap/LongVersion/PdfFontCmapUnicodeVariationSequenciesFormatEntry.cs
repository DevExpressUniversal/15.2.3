#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

namespace DevExpress.Pdf.Native {
	public class PdfFontCmapUnicodeVariationSequenciesFormatEntry : PdfFontCmapFormatEntry {
		static int GetInt24(byte[] array) {
			return array != null && array.Length == 3 ? (array[0] << 16) + (array[1] << 8) + array[2] : 0;
		}
		const int headerLength = 10;
		const int defaultUVSTableSize = 4;
		const int nonDefaultUVSTableSize = 5;
		const int variationSelectorRecordSize = 11;
		readonly PdfFontCmapUnicodeVariationSelectorRecord[] variationSelectorRecords;
		public override int Length { 
			get {
				int length = headerLength + variationSelectorRecords.Length * variationSelectorRecordSize;
				foreach (PdfFontCmapUnicodeVariationSelectorRecord variationSelectorRecord in variationSelectorRecords) {
					DefaultUVSTable[] dTables = variationSelectorRecord.DefaultUVSTables;
					if (dTables != null)
						length += (4 + dTables.Length * defaultUVSTableSize);
					NonDefaultUVSTable[] ndTables = variationSelectorRecord.NonDefaultUVSTables;
					if (ndTables != null)
						length += (4 + ndTables.Length * nonDefaultUVSTableSize);
				}
				return length;
			}
		}
		public PdfFontCmapUnicodeVariationSelectorRecord[] VariationSelectorRecords { get { return variationSelectorRecords; } }
		protected override PdfFontCmapFormatID Format { get { return PdfFontCmapFormatID.UnicodeVariationSequences; } }
		public PdfFontCmapUnicodeVariationSequenciesFormatEntry(PdfFontPlatformID platformId, PdfFontEncodingID encodingId, PdfBinaryStream stream)
			: base(platformId, encodingId) {
			long startPosition = stream.Position - 2;
			stream.ReadInt();
			int variationSelectorRecordsCount = stream.ReadInt();
			variationSelectorRecords = new PdfFontCmapUnicodeVariationSelectorRecord[variationSelectorRecordsCount];
			for (int i = 0; i < variationSelectorRecordsCount; i++) {
				int varSelector = GetInt24(stream.ReadArray(3));
				int defaultUVSOffset = stream.ReadInt();
				int nonDefaultUVSOffset = stream.ReadInt();
				long pos = stream.Position;
				DefaultUVSTable[] defaultUVSTables = null;
				NonDefaultUVSTable[] nonDefaultUVSTables = null;
				if (defaultUVSOffset != 0) {
					stream.Position = startPosition + defaultUVSOffset;
					int numUnicodeValueRanges = stream.ReadInt();
					defaultUVSTables = new DefaultUVSTable[numUnicodeValueRanges];
					for (int j = 0; j < numUnicodeValueRanges; j++)
						defaultUVSTables[j] = new DefaultUVSTable(GetInt24(stream.ReadArray(3)), stream.ReadByte());
				}
				if (nonDefaultUVSOffset != 0) {
					stream.Position = startPosition + nonDefaultUVSOffset;
					int numUVSMappings = stream.ReadInt();
					nonDefaultUVSTables = new NonDefaultUVSTable[numUVSMappings];
					for (int j = 0; j < numUVSMappings; j++)
						nonDefaultUVSTables[j] = new NonDefaultUVSTable(GetInt24(stream.ReadArray(3)), stream.ReadShort());
				}
				stream.Position = pos;
				variationSelectorRecords[i] = new PdfFontCmapUnicodeVariationSelectorRecord(varSelector, defaultUVSTables, nonDefaultUVSTables);
			}
			stream.Position = startPosition + Length;
		}
		public override void Write(PdfBinaryStream tableStream) {
			base.Write(tableStream);
			tableStream.WriteInt(Length);
			tableStream.WriteInt(variationSelectorRecords.Length);
			int offset = headerLength + variationSelectorRecords.Length * variationSelectorRecordSize;
			foreach (PdfFontCmapUnicodeVariationSelectorRecord varSelectorRecord in variationSelectorRecords)
				offset += varSelectorRecord.Write(tableStream, offset);
			foreach (PdfFontCmapUnicodeVariationSelectorRecord varSelectorRecord in variationSelectorRecords) {
				DefaultUVSTable[] dTables = varSelectorRecord.DefaultUVSTables;
				if (dTables != null) {
					tableStream.WriteInt(dTables.Length);
					foreach (DefaultUVSTable dTable in dTables)
						dTable.Write(tableStream);
				}
				NonDefaultUVSTable[] ndTables = varSelectorRecord.NonDefaultUVSTables;
				if (ndTables != null) {
					tableStream.WriteInt(ndTables.Length);
					foreach (NonDefaultUVSTable ndTable in ndTables)
						ndTable.Write(tableStream);
				}
			}
		}
	}
}
