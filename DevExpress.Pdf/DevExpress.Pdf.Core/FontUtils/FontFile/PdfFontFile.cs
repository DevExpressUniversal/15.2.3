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

using System;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfFontFile : PdfDisposableObject {
		const int tableDirectoryOffset = 12;
		static readonly byte[] openTypeVersion = new byte[] { 0x4f, 0x54, 0x54, 0x4f };
		static readonly byte[] ttfVersion = new byte[] { 0x00, 0x01, 0x00, 0x00 };
		static readonly SortedSet<string> subsetTableTags = new SortedSet<string>() { PdfFontHeadTableDirectoryEntry.EntryTag, PdfFontHheaTableDirectoryEntry.EntryTag, PdfFontMaxpTableDirectoryEntry.EntryTag, 
																					  PdfFontHmtxTableDirectoryEntry.EntryTag, PdfTrueTypeGlyfTableDirectoryEntry.EntryTag, 
																					  PdfTrueTypeLocaTableDirectoryEntry.EntryTag, "cvt ", "fpgm", "prep" };
		public static byte[] TTFVersion { get { return ttfVersion; } }
		public static byte[] GetCFFData(byte[] fontFileData) {
			int length = fontFileData.Length;
			if (length >= openTypeVersion.Length && fontFileData[0] == openTypeVersion[0] && fontFileData[1] == openTypeVersion[1] && fontFileData[2] == openTypeVersion[2] && fontFileData[3] == openTypeVersion[3])
				using (PdfFontFile file = new PdfFontFile(openTypeVersion, fontFileData)) {
					PdfFontTableDirectoryEntry cffEntry = file.GetTable<PdfOpenTypeCFFTableDirectoryEntry>(PdfOpenTypeCFFTableDirectoryEntry.EntryTag);
					if (cffEntry != null)
						return cffEntry.TableData;
				}
			return null;
		}
		readonly byte[] version;
		readonly SortedDictionary<string, PdfFontTableDirectoryEntry> tableDictionary = new SortedDictionary<string, PdfFontTableDirectoryEntry>();
		PdfFontHheaTableDirectoryEntry hhea;
		PdfFontCmapTableDirectoryEntry cMap;
		PdfFontKernTableDirectoryEntry kern;		
		PdfFontHmtxTableDirectoryEntry hmtx;
		float ttfToPdfFactor = 1000f / 2048;
		public PdfFontHeadTableDirectoryEntry Head { get { return GetTable<PdfFontHeadTableDirectoryEntry>(PdfFontHeadTableDirectoryEntry.EntryTag); } }
		public PdfFontMaxpTableDirectoryEntry Maxp { get { return GetTable<PdfFontMaxpTableDirectoryEntry>(PdfFontMaxpTableDirectoryEntry.EntryTag); } }
		public PdfFontOS2TableDirectoryEntry OS2   { get { return GetTable<PdfFontOS2TableDirectoryEntry>(PdfFontOS2TableDirectoryEntry.EntryTag); } }
		public PdfFontPostTableDirectoryEntry Post { get { return GetTable<PdfFontPostTableDirectoryEntry>(PdfFontPostTableDirectoryEntry.EntryTag); } }
		public PdfFontNameTableDirectoryEntry Name { get { return GetTable<PdfFontNameTableDirectoryEntry>(PdfFontNameTableDirectoryEntry.EntryTag); } }
		public PdfTrueTypeLocaTableDirectoryEntry Loca { get { return GetTable<PdfTrueTypeLocaTableDirectoryEntry>(PdfTrueTypeLocaTableDirectoryEntry.EntryTag); } }
		public PdfTrueTypeGlyfTableDirectoryEntry Glyf { get { return GetTable<PdfTrueTypeGlyfTableDirectoryEntry>(PdfTrueTypeGlyfTableDirectoryEntry.EntryTag); } }
		public PdfFontHheaTableDirectoryEntry Hhea	 { 
			get { 
				if (hhea == null)
					hhea = GetTable<PdfFontHheaTableDirectoryEntry>(PdfFontHheaTableDirectoryEntry.EntryTag); 
				return hhea;
			} 
		}
		public PdfFontCmapTableDirectoryEntry CMap { 
			get { 
				if (cMap == null)
					cMap = GetTable<PdfFontCmapTableDirectoryEntry>(PdfFontCmapTableDirectoryEntry.EntryTag);
				return cMap;
			}
		}
		public PdfFontKernTableDirectoryEntry Kern	 {
			get { 
				if (kern == null)
					kern = GetTable<PdfFontKernTableDirectoryEntry>(PdfFontKernTableDirectoryEntry.EntryTag);
				return kern;
			}
		}
		PdfFontHmtxTableDirectoryEntry Hmtx { 
			get { 
				if (hmtx == null)
					hmtx = GetTable<PdfFontHmtxTableDirectoryEntry>(PdfFontHmtxTableDirectoryEntry.EntryTag); 
				return hmtx;
			} 
		}
		public PdfFontFile(IFont font) {
			version = openTypeVersion;
			PdfFontCmapTableDirectoryEntry cmap = new PdfFontCmapTableDirectoryEntry(font);
			AddTable(new PdfOpenTypeCFFTableDirectoryEntry(font));
			AddTable(new PdfFontOS2TableDirectoryEntry(font));
			AddTable(cmap);
			AddTable(new PdfFontHeadTableDirectoryEntry(font));
			AddTable(new PdfFontHheaTableDirectoryEntry(font));
			AddTable(new PdfFontHmtxTableDirectoryEntry(font));
			AddTable(new PdfFontMaxpTableDirectoryEntry(font));
			AddTable(new PdfFontNameTableDirectoryEntry(cmap, font.BaseFont));
			AddTable(new PdfFontPostTableDirectoryEntry(font));
		}
		public PdfFontFile(byte[] version, byte[] data) {
			this.version = version;
			using (PdfBinaryStream stream = new PdfBinaryStream(data))
				ReadTables(stream);
		}
		public PdfFontFile(PdfBinaryStream stream) {
			version = TTFVersion;
			ReadTables(stream);
		}
		public void AddTable(PdfFontTableDirectoryEntry table) {
			tableDictionary[table.Tag] = table;
		}
		public byte[] CreateSubset(PdfCharacterCache cache) {
			PdfTrueTypeGlyfTableDirectoryEntry glyf = Glyf;
			if (glyf != null)
				glyf.CreateSubset(this, cache);
			return GetData(subsetTableTags);
		}
		public byte[] GetData() {
			return GetData(tableDictionary.Keys);
		}
		public float GetCharacterWidth(int glyphIndex) {
			PdfFontHmtxTableDirectoryEntry hmtx = Hmtx;
			if (hmtx == null) 
				return 0;
			short[] advanceWidths = hmtx.AdvanceWidths;
			if (advanceWidths == null) {
				PdfFontHheaTableDirectoryEntry hhea = Hhea;
				 if (hhea == null)
					 return 0;
				PdfFontMaxpTableDirectoryEntry maxp = Maxp;
				advanceWidths = hmtx.FillAdvanceWidths(hhea.NumberOfHMetrics, maxp == null ? 0 : maxp.NumGlyphs);
			}
			return glyphIndex < advanceWidths.Length ? (advanceWidths[glyphIndex] * ttfToPdfFactor) : 0;
		}
		void ReadTables(PdfBinaryStream stream) {
			long startOffset = stream.Position;
			stream.ReadInt();
			short numTables = stream.ReadShort();
			stream.ReadShort();
			stream.ReadShort();
			stream.Position = startOffset + tableDirectoryOffset;
			for (short i = 0; i < numTables; i++) {
				string tag = stream.ReadString(4);
				stream.ReadInt();
				int offset = stream.ReadInt();
				int length = stream.ReadInt();
				long currentPosition = stream.Position;
				stream.Position = offset;
				AddTable(PdfFontTableDirectoryEntry.Create(tag, stream.ReadArray(length)));
				stream.Position = currentPosition;
			}
			PdfFontHeadTableDirectoryEntry head = Head;
			if (head != null)
				ttfToPdfFactor = 1000f / head.UnitsPerEm;
			PdfTrueTypeLocaTableDirectoryEntry loca = Loca;
			if (loca != null)
				loca.ReadOffsets(this);
			PdfTrueTypeGlyfTableDirectoryEntry glyf = Glyf;
			if (glyf != null)
				glyf.ReadGlyphs(this);
		}
		T GetTable<T>(string key) where T : PdfFontTableDirectoryEntry {
			PdfFontTableDirectoryEntry entry;
			return tableDictionary.TryGetValue(key, out entry) ? (entry as T) : null;
		}
		byte[] GetData(ICollection<string> tablesToWrite) {
			short numTables = 0;
			foreach (KeyValuePair<string, PdfFontTableDirectoryEntry> entry in tableDictionary)
				if (tablesToWrite.Contains(entry.Key))
					numTables++;
			int entryOffset = tableDirectoryOffset + numTables * 16;
			using (PdfBinaryStream stream = new PdfBinaryStream()) {
				stream.WriteArray(version);
				stream.WriteShort(numTables);
				short factor = Convert.ToInt16(Math.Pow(2, Math.Floor(Math.Log(numTables, 2))));
				short searchRange = (short)(factor * 16);
				stream.WriteShort(searchRange);
				stream.WriteShort(Convert.ToInt16(Math.Log(factor, 2)));
				stream.WriteShort((short)(numTables * 16 - searchRange));
				foreach (KeyValuePair<string, PdfFontTableDirectoryEntry> entry in tableDictionary)
					if (tablesToWrite.Contains(entry.Key))
						entryOffset += entry.Value.Write(stream, entryOffset);
				foreach (KeyValuePair<string, PdfFontTableDirectoryEntry> entry in tableDictionary)
					if (tablesToWrite.Contains(entry.Key))
						stream.WriteArray(entry.Value.TableData);
				stream.Position = 0;
				return stream.ReadArray((int)stream.Length);
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) 
				foreach (KeyValuePair<string, PdfFontTableDirectoryEntry> entry in tableDictionary)
					entry.Value.Dispose();
		}
	}
}
