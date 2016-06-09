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

using System.Text;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfFontNameTableDirectoryEntry : PdfFontTableDirectoryEntry {
		public const string EntryTag = "name";
		const string nameFontSubfamily = "Regular";
		const string nameVersion = "0.0";
		const int maxNameLength = 31;
		readonly List<PdfFontNameRecord> namingTable = new List<PdfFontNameRecord>();
		string familyName = null;
		string macFamilyName = null;
		string postScriptName = null;
		bool shouldWrite = false;
		public string FamilyName {
			get {
				if (familyName == null)
					familyName = FindName(PdfFontPlatformID.Microsoft, PdfFontEncodingID.UGL, PdfFontLanguageID.EnglishUnitedStates, PdfFontNameID.FontFamily);
				return familyName;
			}
		}
		public string MacFamilyName {
			get {
				if (macFamilyName == null)
					macFamilyName = FindName(PdfFontPlatformID.Macintosh, PdfFontEncodingID.Undefined, PdfFontLanguageID.English, PdfFontNameID.FontFamily);
				return macFamilyName;
			}
		}
		public string PostScriptName {
			get {
				if (postScriptName == null)
					postScriptName = FindName(PdfFontPlatformID.Macintosh, PdfFontEncodingID.Undefined, PdfFontLanguageID.English, PdfFontNameID.PostscriptName);
				return postScriptName;
			}
		}
		public PdfFontNameTableDirectoryEntry(byte[] tableData)
			: base(EntryTag, tableData) {
			PdfBinaryStream binaryStream = TableStream;
			if (binaryStream.Length > 6) {
				short format = binaryStream.ReadShort();
				short count = binaryStream.ReadShort();
				short offset = binaryStream.ReadShort();
				for (int i = 0; i < count; i++)
					namingTable.Add(new PdfFontNameRecord(binaryStream, offset));
			}
		}
		public PdfFontNameTableDirectoryEntry(PdfFontCmapTableDirectoryEntry cmapEntry, string fontName) : base(EntryTag) {
			Create(cmapEntry, fontName);
		}
		public void Create(PdfFontCmapTableDirectoryEntry cmapEntry, string fontName) {
			if (fontName.Length > maxNameLength)
				fontName = fontName.Substring(0, maxNameLength);
			Dictionary<PdfFontNameID, byte[]> namesDictionary = new Dictionary<PdfFontNameID, byte[]>();
			Encoding encoding = Encoding.BigEndianUnicode;
			byte[] fontNameBytes = encoding.GetBytes(fontName);
			namesDictionary.Add(PdfFontNameID.FontFamily, fontNameBytes);
			namesDictionary.Add(PdfFontNameID.FontSubfamily, encoding.GetBytes(nameFontSubfamily));
			namesDictionary.Add(PdfFontNameID.FullFontName, fontNameBytes);
			namesDictionary.Add(PdfFontNameID.UniqueFontId, fontNameBytes);
			namesDictionary.Add(PdfFontNameID.Version, encoding.GetBytes(nameVersion));
			namesDictionary.Add(PdfFontNameID.PostscriptName, fontNameBytes);
			namingTable.Clear();
			foreach (PdfFontCmapFormatEntry cmapRecord in cmapEntry.CMapTables) {
				PdfFontPlatformID platformId = cmapRecord.PlatformId;
				PdfFontEncodingID encodingId = cmapRecord.EncodingId;
				PdfFontLanguageID languageId = platformId == PdfFontPlatformID.Microsoft ? PdfFontLanguageID.EnglishUnitedStates : PdfFontLanguageID.English;
				foreach (KeyValuePair<PdfFontNameID, byte[]> entry in namesDictionary)
					namingTable.Add(new PdfFontNameRecord(platformId, languageId, entry.Key, encodingId, entry.Value));
			}
			shouldWrite = true;
		}
		string FindName(PdfFontPlatformID platform, PdfFontEncodingID encoding, PdfFontLanguageID language, PdfFontNameID id) {
			foreach (PdfFontNameRecord record in namingTable)
				if (record.PlatformID == platform && record.EncodingID == encoding && record.LanguageID == language && record.NameID == id) 
					return record.Name;
			return string.Empty;
		}
		protected override void ApplyChanges() {
			base.ApplyChanges();
			if (shouldWrite) {
				PdfBinaryStream tableStream = CreateNewStream();
				tableStream.WriteShort(0);
				short namingTableLength = (short)namingTable.Count;
				tableStream.WriteShort(namingTableLength);
				tableStream.WriteShort((short)(6 + namingTableLength * 12));
				short nameOffset = 0;
				foreach (PdfFontNameRecord nameRecord in namingTable) {
					tableStream.WriteShort((short)nameRecord.PlatformID);
					tableStream.WriteShort((short)nameRecord.EncodingID);
					tableStream.WriteShort((short)nameRecord.LanguageID);
					tableStream.WriteShort((short)nameRecord.NameID);
					byte[] nameBytes = nameRecord.NameBytes;
					short nameLength = (short)(nameBytes == null ? 0 : nameBytes.Length);
					tableStream.WriteShort(nameLength);
					tableStream.WriteShort(nameOffset);
					nameOffset += nameLength;
				}
				foreach (PdfFontNameRecord nameRecord in namingTable) {
					if (nameRecord.NameBytes != null)
						tableStream.WriteArray(nameRecord.NameBytes);
				}
			}
		}
	}
}
