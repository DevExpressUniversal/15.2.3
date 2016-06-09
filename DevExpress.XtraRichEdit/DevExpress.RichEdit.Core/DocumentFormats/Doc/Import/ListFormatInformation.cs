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
using System.IO;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.NumberConverters;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocListFormatInformation
	public class DocListFormatInformation {
		public static DocListFormatInformation FromStream(BinaryReader reader, int offset, int size) {
			DocListFormatInformation result = new DocListFormatInformation();
			result.Read(reader, offset, size);
			return result;
		}
		#region Fields
		List<DocListData> listData;
		#endregion
		public DocListFormatInformation() {
			this.listData = new List<DocListData>();
		}
		#region Properties
		public List<DocListData> ListData { get { return this.listData; } }
		#endregion
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			if (size == 0)
				return;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			short listsCount = reader.ReadInt16();
			for (int i = 0; i < listsCount; i++) {
				DocListData item = DocListData.FromStream(reader);
				this.listData.Add(item);
			}
			for (int listIndex = 0; listIndex < listsCount; listIndex++) {
				if (this.listData[listIndex].ListFormatting.SimpleList)
					this.listData[listIndex].LevelsFormatting.Add(DocListLevel.FromStream(reader));
				else 
					for (int levelIndex = 0; levelIndex < DocListFormatting.MaxLevelCount; levelIndex++) {
						this.listData[listIndex].LevelsFormatting.Add(DocListLevel.FromStream(reader));
					}
			}
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			short listCount = (short)this.listData.Count;
			if (listCount == 0)
				return;
			writer.Write(listCount);
			for (int i = 0; i < listCount; i++) {
				this.listData[i].ListFormatting.Write(writer);
			}
			for (int listIndex = 0; listIndex < listCount; listIndex++) {
				int levelCount = this.listData[listIndex].LevelsFormatting.Count;
				for (int levelIndex = 0; levelIndex < levelCount; levelIndex++) {
					this.listData[listIndex].LevelsFormatting[levelIndex].Write(writer);
				}
			}
		}
	}
	#endregion
	#region DocListData
	public class DocListData {
		public static DocListData FromStream(BinaryReader reader) {
			DocListData result = new DocListData();
			result.listFormatting = DocListFormatting.FromStream(reader);
			return result;
		}
		#region Fields
		DocListFormatting listFormatting;
		List<DocListLevel> levelsFormatting;
		#endregion
		public DocListData() {
			this.levelsFormatting = new List<DocListLevel>();
		}
		#region Properties
		public DocListFormatting ListFormatting { 
			get { return listFormatting; }
			protected internal set { listFormatting = value; }
		}
		public List<DocListLevel> LevelsFormatting { get { return levelsFormatting; } }
		#endregion
	}
	#endregion
	#region DocListFormatting
	public class DocListFormatting {
		public static DocListFormatting FromStream(BinaryReader reader) {
			DocListFormatting result = new DocListFormatting();
			result.Read(reader);
			return result;
		}
		#region Fields
		public const int MaxLevelCount = 9;
		int listIdentifier;
		int templateCode;
		short[] levelStyleIdentifiers;
		bool simpleList;
		bool hybrid;
		bool autonumbered;
		byte htmlCompatibilitiesFlags;
		#endregion
		public DocListFormatting() {
			this.levelStyleIdentifiers = new short[MaxLevelCount];
		}
		#region Properties
		public int ListIdentifier {
			get { return listIdentifier; }
			protected internal set { listIdentifier = value; }
		}
		public int TemplateCode {
			get { return templateCode; }
			protected internal set { templateCode = value; }
		}
		public short[] LevelStyleIdentifiers {
			get { return levelStyleIdentifiers; }
			protected internal set { levelStyleIdentifiers = value; }
		}
		public bool SimpleList {
			get { return simpleList; }
			protected internal set { simpleList = value; }
		}
		public bool Autonumbered {
			get { return autonumbered; }
			protected internal set { autonumbered = value; }
		}
		public bool Hybrid {
			get { return hybrid; }
			protected internal set { hybrid = value; }
		}
		#endregion
		protected void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			this.listIdentifier = reader.ReadInt32();
			this.templateCode = reader.ReadInt32();
			for (int levelIndex = 0; levelIndex < MaxLevelCount; levelIndex++) {
				this.levelStyleIdentifiers[levelIndex] = reader.ReadInt16();
			}
			byte flags = reader.ReadByte();
			this.simpleList = (flags & 0x01) != 0;
			this.autonumbered = (flags & 0x04) != 0;
			this.hybrid = (flags & 0x10) != 0;
			this.htmlCompatibilitiesFlags = reader.ReadByte();
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.listIdentifier);
			writer.Write(this.templateCode);
			for (int levelIndex = 0; levelIndex < MaxLevelCount; levelIndex++) {
				writer.Write(this.levelStyleIdentifiers[levelIndex]);
			}
			byte flags = (SimpleList) ? (byte)0x01 : (byte)0x00;
			writer.Write(flags);
			writer.Write(this.htmlCompatibilitiesFlags);
		}
	}
	#endregion
	#region DocListLevel
	public class DocListLevel {
		#region static
		protected internal static bool IsBrackets(char ch) {
			return ch == '{' || ch == '}';
		}
		protected internal static bool IsDoubleBrackets(string displayFormatString, int i) {
			if (i == displayFormatString.Length - 1)
				return false;
			return (displayFormatString[i] == '}' && displayFormatString[i + 1] == '}') ||
					(displayFormatString[i] == '{' && displayFormatString[i + 1] == '{');
		}
		public static DocListLevel FromStream(BinaryReader reader) {
			DocListLevel result = new DocListLevel();
			result.Read(reader);
			return result;
		}
		#endregion
		#region Fields
		DocListLevelProperties listLevelProperties;
		byte[] characterUPX;
		byte[] paragraphUPX;
		string displayTextPlaceHolder;
		int currentLevel;
		#endregion
		#region Properties
		public DocListLevelProperties ListLevelProperties {
			get { return listLevelProperties; }
			protected internal set { listLevelProperties = value; }
		}
		public byte[] CharacterUPX {
			get { return characterUPX; }
			protected internal set { characterUPX = value; }
		}
		public byte[] ParagraphUPX {
			get { return paragraphUPX; }
			protected internal set { paragraphUPX = value; }
		}
		public string DisplayTextPlaceHolder {
			get { return displayTextPlaceHolder; }
			protected internal set { displayTextPlaceHolder = value; }
		}
		#endregion
		protected void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "result");
			ListLevelProperties = DocListLevelProperties.FromStream(reader);
			this.paragraphUPX = reader.ReadBytes(ListLevelProperties.ParagraphUPXSize);
			this.characterUPX = reader.ReadBytes(ListLevelProperties.CharacterUPXSize);
			DisplayTextPlaceHolder = GetDisplayTextPlaceHolder(reader);
		}
		string GetDisplayTextPlaceHolder(BinaryReader reader) {
			int textPlaceholderSize = reader.ReadUInt16() * 2;
			if (textPlaceholderSize >= reader.BaseStream.Length - reader.BaseStream.Position)
				return String.Empty;
			byte[] buffer = reader.ReadBytes(textPlaceholderSize);
			return " " + Encoding.Unicode.GetString(buffer, 0, buffer.Length);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			this.listLevelProperties.Write(writer);
			writer.Write(this.paragraphUPX);
			writer.Write(this.characterUPX);
			writer.Write((short)DisplayTextPlaceHolder.Length);
			writer.Write(Encoding.Unicode.GetBytes(DisplayTextPlaceHolder));
		}
		public string GetDisplayFormatString() {
			List<int> placeHolderIndices = GetPlaceHolderIndices();
			string displayString = ListLevelDisplayTextHelper.CreateDisplayFormatStringCore(placeHolderIndices, DisplayTextPlaceHolder);
			return StringHelper.RemoveSpecialSymbols(displayString);
		}
		public List<int> GetPlaceHolderIndices() {
			List<int> result = new List<int>();
			result.Add(0);
			for (int placeHolderIndex = 0; placeHolderIndex < DocListFormatting.MaxLevelCount; placeHolderIndex++) {
				if (this.listLevelProperties.PlaceholderIndices[placeHolderIndex] == 0)
					break;
				result.Add(this.listLevelProperties.PlaceholderIndices[placeHolderIndex]);
			}
			result.Add(DisplayTextPlaceHolder.Length);
			return result;
		}
		public void SetDisplayFormatString(string displayFormatString) {
			DisplayTextPlaceHolder = string.Empty;
			this.listLevelProperties.PlaceholderIndices = new byte[DocListFormatting.MaxLevelCount];
			this.currentLevel = 0;
			int count = displayFormatString.Length;
			int currentIndex = 0;
			while (currentIndex < count) {
				char ch = displayFormatString[currentIndex];
				if (!IsBrackets(ch)) {
					DisplayTextPlaceHolder += ch;
					currentIndex++;
				}
				else
					currentIndex += AddLevelNumber(displayFormatString, currentIndex);
			}
		}
		int AddLevelNumber(string displayFormatString, int index) {
			if (IsDoubleBrackets(displayFormatString, index)) {
				DisplayTextPlaceHolder += displayFormatString[index];
				return index + 2;
			}
			return AddLevelNumberCore(displayFormatString, index);
		}
		int AddLevelNumberCore(string displayFormatString, int index) {
			index++;
			string value = displayFormatString.Substring(index, displayFormatString.IndexOf('}', index) - index);
			DisplayTextPlaceHolder += (char)int.Parse(value);
			this.listLevelProperties.PlaceholderIndices[currentLevel] = (byte)DisplayTextPlaceHolder.Length;
			this.currentLevel++;
			return value.Length + 2;
		}
	}
	#endregion
	#region DocListLevelFormattingInfo
	[Flags]
	public enum DocListLevelFormatFlags {
		ConvertPreviousLevelNumberingToDecimal = 0x04,
		NoRestart = 0x08,
		RemoveIndent = 0x10,
		Converted = 0x20,
		Legacy = 0x40,
		Tentative = 0x80
	}
	public class DocListLevelProperties {
		const int bulletFormatCode = 0x17;
		const char nothing = '\0';
		const char space = ' ';
		public static DocListLevelProperties FromStream(BinaryReader reader) {
			DocListLevelProperties result = new DocListLevelProperties();
			result.Read(reader);
			return result;
		}
		#region Fields
		int start;
		NumberingFormat numberingFormat;
		bool bulletedList;
		bool convertPreviousLevelNumberingToDecimal;
		bool legacy;
		bool suppressRestart;
		ListNumberAlignment alignment;
		DocListLevelFormatFlags levelFormatFlags;
		byte[] placeholderIndices;
		char separator;
		int legacySpace;
		int legacyIndent;
		byte characterUPXsize;
		byte paragraphUPXsize;
		byte relativeRestartLevel;
		byte htmlCompatibilitiesFlags;
		#endregion
		#region Properties
		public int Start {
			get { return this.start; }
			protected internal set { this.start = value; }
		}
		public NumberingFormat NumberingFormat { 
			get { return this.numberingFormat; }
			protected internal set { this.numberingFormat = value; }
		}
		public bool BulletedList { get { return this.bulletedList; } }
		public bool ConvertPreviousLevelNumberingToDecimal { 
			get { return this.convertPreviousLevelNumberingToDecimal; }
			protected internal set { this.convertPreviousLevelNumberingToDecimal = value; }
		}
		public bool Legacy { 
			get { return this.legacy; }
			protected internal set { this.legacy = value; }
		}
		public bool SuppressRestart { 
			get { return this.suppressRestart; }
			protected internal set { this.suppressRestart = value; }
		}
		public ListNumberAlignment Alignment { 
			get { return this.alignment; }
			protected internal set { this.alignment = value; }
		}
		public byte[] PlaceholderIndices {
			get { return this.placeholderIndices; }
			protected internal set { this.placeholderIndices = value; }
		}
		public char Separator {
			get { return this.separator; }
			protected internal set { this.separator = value; }
		}
		public int LegacySpace {
			get { return this.legacySpace; }
			protected internal set { this.legacySpace = value; }
		}
		public int LegacyIndent {
			get { return this.legacyIndent; }
			protected internal set { this.legacyIndent = value; }
		}
		public byte CharacterUPXSize {
			get { return this.characterUPXsize; }
			protected internal set { this.characterUPXsize = value; }
		}
		public byte ParagraphUPXSize {
			get { return this.paragraphUPXsize; }
			protected internal set { this.paragraphUPXsize = value; }
		}
		public byte RelativeRestartLevel {
			get { return this.relativeRestartLevel; }
			protected internal set { this.relativeRestartLevel = value; }
		}
		#endregion
		protected void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			this.start = Math.Max(0, reader.ReadInt32());
			byte numberFormatCode = reader.ReadByte();
			if (numberFormatCode == bulletFormatCode)
				this.bulletedList = true;
			this.numberingFormat = NumberingFormatCalculator.CalcNumberingFormat(numberFormatCode);
			byte flags = reader.ReadByte();
			this.alignment = AlignmentCalculator.CalcListNumberAlignment((byte)(flags & 0x03));
			this.levelFormatFlags = (DocListLevelFormatFlags)(flags & 0xfc);
			this.convertPreviousLevelNumberingToDecimal = (this.levelFormatFlags & DocListLevelFormatFlags.ConvertPreviousLevelNumberingToDecimal) != 0;
			this.suppressRestart = (this.levelFormatFlags & DocListLevelFormatFlags.NoRestart) != 0;
			this.legacy = (this.levelFormatFlags & DocListLevelFormatFlags.Legacy) != 0;
			this.placeholderIndices = reader.ReadBytes(DocListFormatting.MaxLevelCount);
			this.separator = CalcSeparator(reader.ReadByte());
			this.legacySpace = reader.ReadInt32();
			this.legacyIndent = reader.ReadInt32();
			this.characterUPXsize = reader.ReadByte();
			this.paragraphUPXsize = reader.ReadByte();
			this.relativeRestartLevel = reader.ReadByte();
			this.htmlCompatibilitiesFlags = reader.ReadByte();
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			this.start = Math.Max(0, this.start);
			if (this.start == 0xffff)
				this.start = 0;
			writer.Write(this.start);
			byte numberFormatCode = NumberingFormatCalculator.CalcNumberingFormatCode(this.numberingFormat);
			writer.Write(numberFormatCode);
			byte flags = AlignmentCalculator.CalcListNumberAlignmentCode(this.alignment);
			if (this.convertPreviousLevelNumberingToDecimal)
				this.levelFormatFlags |= DocListLevelFormatFlags.ConvertPreviousLevelNumberingToDecimal;
			if (this.legacy)
				this.levelFormatFlags |= DocListLevelFormatFlags.Legacy;
			if (this.suppressRestart)
				this.levelFormatFlags |= DocListLevelFormatFlags.NoRestart;
			flags |= (byte)this.levelFormatFlags;
			writer.Write(flags);
			writer.Write(this.placeholderIndices);
			writer.Write(CalcSeparatorCode());
			writer.Write(this.legacySpace);
			writer.Write(this.legacyIndent);
			writer.Write(this.characterUPXsize);
			writer.Write(this.paragraphUPXsize);
			writer.Write(this.relativeRestartLevel);
			writer.Write(this.htmlCompatibilitiesFlags);
		}
		char CalcSeparator(byte separatorCode) {
			switch (separatorCode) {
				case 0: return Characters.TabMark;
				case 1: return space;
				default: return nothing;
			}
		}
		byte CalcSeparatorCode() {
			switch (this.separator) {
				case Characters.TabMark: return 0x0;
				case space: return 0x1;
				default: return 0x2;
			}
		}
	}
	#endregion
}
