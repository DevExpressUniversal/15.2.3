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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public enum DocumentViewKind {
		NormalView = 0x00,
		OutlineView = 0x01,
		PageView = 0x02
	}
	public enum ZoomType {
		None = 0x0000,
		FullPage = 0x1000,
		PageWidth = 0x2000
	}
	public enum GutterPosition {
		Side = 0x0000,
		Top = 0x8000
	}
	public enum AutoformatDocumentType {
		Normal = 0x00,
		Letter = 0x01,
		Email = 0x02
	}
	public class DocDocumentProperties {
		public static DocDocumentProperties FromStream(BinaryReader reader, int offset) {
			DocDocumentProperties result = new DocDocumentProperties();
			result.Read(reader, offset);
			return result;
		}
		public static DocDocumentProperties CreateDefault() {
			DocDocumentProperties result = new DocDocumentProperties();
			result.widowControl = 0x02;
			result.footNotePosition = FootNotePosition.BelowText;
			result.footNoteNumberingRestartType = LineNumberingRestart.Continuous;
			result.footnoteInitialNumber = 1;
			result.outlineDirtySave = 0x0001;
			result.hyphCapitals = 0x0800;
			result.displayFieldResults = 0x0008;
			result.trueTypeFontsByDefault = 0x0080;
			result.displayFieldResults = 0x0008;
			result.trueTypeFontsByDefault = 0x0080;
			result.showRevisionMarkings = 0x0800;
			result.printRevisionMarkings = 0x1000;
			result.hyphenationHotZone = 0x0168;
			result.dateCreated = DateTime.Now;
			result.dateRevised = DateTime.Now;
			result.dateLastPrinted = DateTime.MinValue;
			result.pagesCount = 1;
			result.paragraphsCount = 1;
			result.endNotePosition = FootNotePosition.EndOfSection;
			result.shadeFormFields = 0x1000;
			result.includeFootnotesAndEndNotesInWordsCount = 0x8000;
			result.viewKind = DocumentViewKind.OutlineView;
			result.zoomPercentage = 100;
			result.zoomType = ZoomType.None;
			result.gutterPosition = GutterPosition.Side;
			result.doptypography = new DocumentTypographyInfo();
			result.EnforceProtection = false;
			result.ProtectionType = DocumentProtectionType.ReadOnly;
			result.PasswordHash = BitConverter.GetBytes(0);
			return result;
		}
		#region Fields
		const ushort defaultCompatibilities60 = 0xf000;
		const uint defaultCompatibilities80 = 0x0010f000;
		const int passwordHashPosition = 78;
		const int protectionPosition = 598;
		const int backgroundPosition = 598;
		byte differentOddAndEvenPage;
		byte displayBackgroundShape;
		byte widowControl;
		FootNotePosition footNotePosition;
		LineNumberingRestart footNoteNumberingRestartType;
		int footnoteInitialNumber;
		ushort outlineDirtySave;
		ushort onlyMacPics;
		ushort onlyWinPics;
		ushort labelDoc;
		ushort hyphCapitals;
		ushort backup;
		ushort exactWordsCount;
		ushort displayHiddenContent;
		ushort displayFieldResults;
		ushort lockAnnotations;
		ushort mirrorMargins;
		ushort trueTypeFontsByDefault;
		ushort autoHyphen;
		ushort formNoFields;
		ushort linkStyles;
		ushort revMarking;
		ushort protectedFromEditing;
		ushort displayFormFieldsSelection;
		ushort showRevisionMarkings;
		ushort printRevisionMarkings;
		ushort lockRevisionMarkings;
		ushort containsEmbeddedFonts;
		ushort enforceProtection;
		short hyphenationHotZone; 
		short hyphenationConsecutiveLimit; 
		DateTime dateCreated;
		DateTime dateRevised;
		DateTime dateLastPrinted;
		short revisionNumber;
		int timeEdited;
		int charactersCount;
		int wordsCount;
		int paragraphsCount;
		short pagesCount;
		LineNumberingRestart endNoteNumberingRestartType;
		int endnoteInitialNumber;
		FootNotePosition endNotePosition;
		ushort shadeFormFields;
		ushort includeFootnotesAndEndNotesInWordsCount;
		int linesCount;
		int wordsCountInNotes;
		int charactersCountInNotes;
		short pagesCountInNotes;
		int paragraphsCountInNotes;
		int linesCountInNotes;
		int documentProtectionPasswordKey;
		DocumentViewKind viewKind;
		short zoomPercentage;
		ZoomType zoomType;
		GutterPosition gutterPosition;
		AutoformatDocumentType documentType;
		DocumentTypographyInfo doptypography;
		#endregion
		#region Properties
		public bool DifferentOddAndEvenPages {
			get { return differentOddAndEvenPage != 0; }
			protected internal set { differentOddAndEvenPage = (value) ? (byte)1 : (byte)0; }
		}
		public bool DisplayBackgroundShape {
			get { return displayBackgroundShape != 0; }
			protected internal set { displayBackgroundShape = (value) ? (byte)1 : (byte)0; }
		}
		protected byte fWidowControl { get { return widowControl; } }
		public FootNotePosition FootNotePosition {
			get { return footNotePosition; }
			protected internal set { footNotePosition = value; }
		}
		public LineNumberingRestart FootNoteNumberingRestartType {
			get { return footNoteNumberingRestartType; }
			protected internal set { footNoteNumberingRestartType = value; }
		}
		public int FootNoteInitialNumber {
			get { return footnoteInitialNumber; }
			protected internal set { footnoteInitialNumber = value; }
		}
		public bool OnlyMacPics { get { return onlyMacPics != 0; } }
		public bool OnlyWinPics { get { return onlyWinPics != 0; } }
		public bool LabelDoc { get { return labelDoc != 0; } }
		public bool HyphCapitals { get { return hyphCapitals != 0; } }
		public bool AutoHyphen { get { return autoHyphen != 0; } }
		public bool FormNoFields { get { return formNoFields != 0; } }
		public bool LinkStyles { get { return linkStyles != 0; } }
		public bool RevisionMarking { get { return revMarking != 0; } }
		public bool Backup { get { return backup != 0; } }
		public bool ExactWordsCount { get { return exactWordsCount != 0; } }
		public bool DisplayHiddenContent { get { return displayHiddenContent != 0; } }
		public bool DisplayFieldResults { get { return displayFieldResults != 0; } }
		public bool LockAnnotations { get { return lockAnnotations != 0; } }
		public bool MirrorMargins { get { return mirrorMargins != 0; } }
		public bool TrueTypeFontsByDefault { get { return trueTypeFontsByDefault != 0; } }
		public bool ProtectedFromEditing { get { return protectedFromEditing != 0; } }
		public bool DisplayFormFieldsSelection { get { return displayFormFieldsSelection != 0; } }
		public bool ShowRevisionMarkings { get { return showRevisionMarkings != 0; } }
		public bool PrintRevisionMarkings { get { return printRevisionMarkings != 0; } }
		public bool LockRevisionMarkings { get { return lockRevisionMarkings != 0; } }
		public bool ContainsEmbeddedFonts { get { return containsEmbeddedFonts != 0; } }
		public bool EnforceProtection {
			get { return enforceProtection != 0; }
			protected internal set { enforceProtection = (value) ? (ushort)1 : (ushort)0; }
		}
		public byte[] PasswordHash { get; protected internal set; }
		public DocumentProtectionType ProtectionType { get; protected internal set; }
		public short DefaultTabWidth { get; set; } 
		public short HyphenationHotZone {
			get { return hyphenationHotZone; }
			protected internal set { hyphenationHotZone = value; }
		}
		public short HyphenationConsecutiveLimit {
			get { return hyphenationConsecutiveLimit; }
			protected internal set { hyphenationConsecutiveLimit = value; }
		}
		public DateTime DateCreated {
			get { return dateCreated; }
			protected internal set { dateCreated = value; }
		}
		public DateTime DateRevised { 
			get { return dateRevised; }
			protected internal set { dateRevised = value; }
		}
		public DateTime DateLastPrinter {
			get { return dateLastPrinted; }
			protected internal set { dateLastPrinted = value; }
		}
		public short RevisionNumber {
			get { return revisionNumber; }
			protected internal set { revisionNumber = value; }
		}
		public int CharactersCount {
			get { return charactersCount; }
			protected internal set { charactersCount = value; }
		}
		public int WordsCount {
			get { return wordsCount; }
			protected internal set { wordsCount = value; }
		}
		public int ParagraphsCount {
			get { return paragraphsCount; }
			protected internal set { paragraphsCount = value; }
		}
		public short PagesCount {
			get { return pagesCount; }
			protected internal set { pagesCount = value; }
		}
		public LineNumberingRestart EndNoteNumberingRestartType {
			get { return endNoteNumberingRestartType; }
			protected internal set { endNoteNumberingRestartType = value; }
		}
		public int EndnoteInitialNumber {
			get { return this.endnoteInitialNumber; }
			protected internal set { this.endnoteInitialNumber = value; }
		}
		public FootNotePosition EndNotePosition {
			get { return this.endNotePosition; }
			protected internal set { this.endNotePosition = value; }
		}
		public bool IncludeFootnotesAndEndnotesInWordCount { get { return this.includeFootnotesAndEndNotesInWordsCount != 0; } }
		public GutterPosition GutterPosition {
			get { return this.gutterPosition; }
			protected internal set { this.gutterPosition = value; }
		}
		public AutoformatDocumentType DocumentType {
			get { return this.documentType; }
			protected internal set { this.documentType = value; }
		}
		public DocumentTypographyInfo Typography {
			get { return this.doptypography; }
			protected internal set { this.doptypography = value; }
		}
		#endregion
		protected void Read(BinaryReader reader, int offset) {
			Guard.ArgumentNotNull(reader, "reader");
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			ReadOddEvenPageSettingsAndFootNoteSettings(reader);
			ReadDocCreatedInfo(reader);
			ReadDisplaySettings(reader);
			ReadDefaultTabWidth(reader);
			ReadHyphenationSettings(reader);
			ReadDocStatistics(reader);
			ReadEndNoteSettings(reader);
			ReadDocStatistics2(reader);
			ReadDocZoomAndPositionInfo(reader);
			ReadDocumentProtection(reader, offset);
			ReadPageBackground(reader, offset);
		}
		void ReadOddEvenPageSettingsAndFootNoteSettings(BinaryReader reader) {
			byte byteFlags = reader.ReadByte();
			this.differentOddAndEvenPage = (byte)(byteFlags & 0x01);
			this.widowControl = (byte)(byteFlags & 0x02);
			this.footNotePosition = FootNotePositionCalculator.CalcFootNotePosition(byteFlags & 0x60);
			reader.BaseStream.Seek(1, SeekOrigin.Current); 
			ushort shortFlags = reader.ReadUInt16();
			this.footNoteNumberingRestartType = FootNoteNumberingRestartCalculator.CalcFootNoteNumberingRestart(shortFlags & 0x0003);
			this.footnoteInitialNumber = shortFlags >> 2;
		}
		void ReadDocCreatedInfo(BinaryReader reader) {
			ushort shortFlags = reader.ReadUInt16();
			this.outlineDirtySave = (ushort)(shortFlags & 0x0001);
			this.onlyMacPics = (ushort)(shortFlags & 0x0100);
			this.onlyWinPics = (ushort)(shortFlags & 0x0200);
			this.labelDoc = (ushort)(shortFlags & 0x0400);
			this.hyphCapitals = (ushort)(shortFlags & 0x0800);
			this.autoHyphen = (ushort)(shortFlags & 0x1000);
			this.formNoFields = (ushort)(shortFlags & 0x2000);
			this.linkStyles = (ushort)(shortFlags & 0x4000);
			this.revMarking = (ushort)(shortFlags & 0x8000);
		}
		void ReadDisplaySettings(BinaryReader reader) {
			ushort shortFlags = reader.ReadUInt16();
			this.backup = (ushort)(shortFlags & 0x0001);
			this.exactWordsCount = (ushort)(shortFlags & 0x0002);
			this.displayHiddenContent = (ushort)(shortFlags & 0x0004);
			this.displayFieldResults = (ushort)(shortFlags & 0x0008);
			this.lockAnnotations = (ushort)(shortFlags & 0x0010);
			this.mirrorMargins = (ushort)(shortFlags & 0x0020);
			this.trueTypeFontsByDefault = (ushort)(shortFlags & 0x0080);
			this.protectedFromEditing = (ushort)(shortFlags & 0x0200);
			this.displayFormFieldsSelection = (ushort)(shortFlags & 0x0400);
			this.showRevisionMarkings = (ushort)(shortFlags & 0x0800);
			this.printRevisionMarkings = (ushort)(shortFlags & 0x1000);
			this.lockRevisionMarkings = (ushort)(shortFlags & 0x4000);
			this.containsEmbeddedFonts = (ushort)(shortFlags & 0x8000);
			reader.BaseStream.Seek(2, SeekOrigin.Current); 
		}
		void ReadDefaultTabWidth(BinaryReader reader) {
			DefaultTabWidth = (short)reader.ReadUInt16();
			reader.BaseStream.Seek(2, SeekOrigin.Current); 
		}
		void ReadHyphenationSettings(BinaryReader reader) {
			this.hyphenationHotZone = (short)reader.ReadUInt16();
			this.hyphenationConsecutiveLimit = (short)reader.ReadUInt16();
			reader.BaseStream.Seek(2, SeekOrigin.Current); 
		}
		void ReadDocStatistics(BinaryReader reader) {
			this.dateCreated = ReadDTTM(reader.ReadUInt32());
			this.dateRevised = ReadDTTM(reader.ReadUInt32());
			this.dateLastPrinted = ReadDTTM(reader.ReadUInt32());
			this.revisionNumber = reader.ReadInt16();
			this.timeEdited = reader.ReadInt32();
			this.wordsCount = reader.ReadInt32();
			this.charactersCount = reader.ReadInt32();
			this.pagesCount = reader.ReadInt16();
			this.paragraphsCount = reader.ReadInt32();
		}
		void ReadEndNoteSettings(BinaryReader reader) {
			ushort shortFlags = reader.ReadUInt16();
			this.endNoteNumberingRestartType = FootNoteNumberingRestartCalculator.CalcFootNoteNumberingRestart(shortFlags & 0x0003);
			this.endnoteInitialNumber = shortFlags >> 2;
			shortFlags = reader.ReadUInt16();
			this.endNotePosition = FootNotePositionCalculator.CalcFootNotePosition(shortFlags & 0x0003);
			this.shadeFormFields = (ushort)(shortFlags & 0x1000);
			this.includeFootnotesAndEndNotesInWordsCount = (ushort)(shortFlags & 0x8000);
		}
		void ReadDocStatistics2(BinaryReader reader) {
			this.linesCount = reader.ReadInt32();
			this.wordsCountInNotes = reader.ReadInt32();
			this.charactersCountInNotes = reader.ReadInt32();
			this.pagesCountInNotes = reader.ReadInt16();
			this.paragraphsCountInNotes = reader.ReadInt32();
			this.linesCountInNotes = reader.ReadInt32();
		}
		void ReadDocZoomAndPositionInfo(BinaryReader reader) {
			this.documentProtectionPasswordKey = reader.ReadInt32();
			ushort shortFlags = reader.ReadUInt16();
			this.viewKind = (DocumentViewKind)(shortFlags & 0x0007);
			this.zoomPercentage = (short)(shortFlags >> 3);
			this.zoomType = (ZoomType)(shortFlags & 0x3000);
			this.gutterPosition = (GutterPosition)(shortFlags & 0x8000);
			reader.BaseStream.Seek(2, SeekOrigin.Current); 
			this.documentType = (AutoformatDocumentType)reader.ReadInt16();
			this.doptypography = DocumentTypographyInfo.FromStream(reader);
		}
		void ReadDocumentProtection(BinaryReader reader, int offset) {
			int protectionOffset = offset + protectionPosition;
			if (protectionOffset >= reader.BaseStream.Length)
				return;
			reader.BaseStream.Seek(offset + passwordHashPosition, SeekOrigin.Begin);
			int hash = reader.ReadInt32();
			if (hash != 0) {
				PasswordHash = BitConverter.GetBytes(hash);
				Array.Reverse(PasswordHash);
			}
			reader.BaseStream.Seek(protectionOffset, SeekOrigin.Begin);
			ushort shortFlags = reader.ReadUInt16();
			enforceProtection = (ushort)(shortFlags & 0x0008);
			ProtectionType = DocumentProtectionTypeCalculator.CalcDocumentProtectionType((short)((shortFlags & 0x0070) >> 4));
		}
		void ReadPageBackground(BinaryReader reader, int offset) {
			int backgroundOffset = offset + backgroundPosition;
			if (backgroundOffset >= reader.BaseStream.Length)
				return;
			reader.BaseStream.Seek(backgroundOffset, SeekOrigin.Begin);
			byte byteFlags = reader.ReadByte();
			this.displayBackgroundShape = (byte)(byteFlags & 0x80);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			int offset = (int)writer.BaseStream.Position;
			WriteDefaultFlags(writer);
			WriteDefaultTabWidth(writer);
			WriteHyphenationSettings(writer);
			WriteDocStatistics(writer);
			WriteEndNoteSettings(writer);
			WriteDocStatistics2(writer);
			WriteDocZoomAndPosiitonInfo(writer);
			WritePageBackgroundAndDocumentProtection(writer, offset);
		}
		void WriteDefaultFlags(BinaryWriter writer) {
			byte byteFlags = (byte)(differentOddAndEvenPage | (fWidowControl << 1) | (FootNotePositionCalculator.CalcFootNotePositionTypeCodeForDocumentProperties(FootNotePosition) << 5));
			writer.Write(byteFlags);
			writer.BaseStream.Seek(1, SeekOrigin.Current);
			ushort shortFlags = (ushort)((ushort)(this.footnoteInitialNumber << 2) | (ushort)this.footNoteNumberingRestartType);
			writer.Write(shortFlags);
			shortFlags = (ushort)(outlineDirtySave
				| onlyMacPics | onlyWinPics | labelDoc | hyphCapitals | autoHyphen | formNoFields | linkStyles | revMarking);
			writer.Write(shortFlags);
			if (EnforceProtection)
				lockAnnotations = 0x0010;
			shortFlags = (ushort)(backup | exactWordsCount | displayHiddenContent | displayFieldResults
				| lockAnnotations | mirrorMargins | trueTypeFontsByDefault | protectedFromEditing | displayFormFieldsSelection
				| showRevisionMarkings | printRevisionMarkings | lockRevisionMarkings | containsEmbeddedFonts);
			writer.Write(shortFlags);
		}
		void WriteDefaultTabWidth(BinaryWriter writer) {
			writer.Write(defaultCompatibilities60);
			writer.Write(DefaultTabWidth);
			writer.BaseStream.Seek(2, SeekOrigin.Current);
		}
		void WriteHyphenationSettings(BinaryWriter writer) {
			writer.Write(this.hyphenationHotZone);
			writer.Write(this.hyphenationConsecutiveLimit);
			writer.BaseStream.Seek(2, SeekOrigin.Current);
		}
		void WritePageBackgroundAndDocumentProtection(BinaryWriter writer, int offset) {
			long originalOffset = writer.BaseStream.Position;
			writer.BaseStream.Seek(offset + passwordHashPosition, SeekOrigin.Begin);
			writer.Write(PasswordHash);
			originalOffset = Math.Max(originalOffset, writer.BaseStream.Position);
			ushort flags = 0;
			flags |= (ushort)(enforceProtection << 3);
			flags |= (ushort)(DocumentProtectionTypeCalculator.CalcDocumentProtectionTypeCode(ProtectionType) << 4);
			flags |= (ushort)(displayBackgroundShape << 7);
			writer.BaseStream.Seek(offset + backgroundPosition, SeekOrigin.Begin);
			writer.Write(flags);
			originalOffset = Math.Max(originalOffset, writer.BaseStream.Position);
			writer.BaseStream.Seek(originalOffset, SeekOrigin.Begin);
		}
		void WriteDocStatistics(BinaryWriter writer) {
			WriteDTTM(writer, this.dateCreated);
			WriteDTTM(writer, this.dateRevised);
			WriteDTTM(writer, this.dateLastPrinted);
			writer.Write(this.revisionNumber);
			writer.Write(this.timeEdited);
			writer.Write(this.wordsCount);
			writer.Write(this.charactersCount);
			writer.Write(this.pagesCount);
			writer.Write(this.paragraphsCount);
		}
		void WriteEndNoteSettings(BinaryWriter writer) {
			ushort shortFlags = (ushort)((ushort)(this.endnoteInitialNumber << 2) | (ushort)(EndNoteNumberingRestartType == LineNumberingRestart.Continuous ? 0 : 1));
			writer.Write(shortFlags);
			shortFlags = (ushort)((ushort)FootNotePositionCalculator.CalcFootNotePositionTypeCode(EndNotePosition) | this.shadeFormFields | this.includeFootnotesAndEndNotesInWordsCount);
			writer.Write(shortFlags);
		}
		void WriteDocStatistics2(BinaryWriter writer) {
			writer.Write(this.linesCount);
			writer.Write(this.wordsCountInNotes);
			writer.Write(this.charactersCountInNotes);
			writer.Write(this.pagesCountInNotes);
			writer.Write(this.paragraphsCountInNotes);
			writer.Write(this.linesCountInNotes);
		}
		void WriteDocZoomAndPosiitonInfo(BinaryWriter writer) {
			writer.Write(this.documentProtectionPasswordKey);
			ushort shortFlags = (ushort)((ushort)this.viewKind | (ushort)(this.zoomPercentage << 3) | (ushort)this.zoomType | (ushort)this.gutterPosition);
			writer.Write(shortFlags);
			writer.Write(defaultCompatibilities80);
			writer.Write((short)this.documentType);
			this.doptypography.Write(writer);
			for (int i = 0; i < 54; i++)
				writer.Write((short)0);
			writer.Write(defaultCompatibilities80);
			for (int i = 0; i < 52; i++)
				writer.Write((short)0);
		}
		DateTime ReadDTTM(uint dttm) {
			if (dttm == 0)
				return DateTime.MinValue;
			int minutes = Convert.ToInt32(dttm & 0x003f);
			int hours = Convert.ToInt32((dttm & 0x07C0) >> 6);
			int daysOfMonth = Convert.ToInt32((dttm & 0xf800) >> 11);
			int months = Convert.ToInt32((dttm & 0x000f0000) >> 16);
			int years = Convert.ToInt32((dttm & 0x1ff00000) >> 20) + 1900;
			if (months == 0 || daysOfMonth == 0)
				return DateTime.MinValue;
			return new DateTime(years, months, daysOfMonth, hours, minutes, 0);
		}
		void WriteDTTM(BinaryWriter writer, DateTime dateTime) {
			if (dateTime == DateTime.MinValue) {
				writer.Write((uint)0);
				return;
			}
			int minutes = dateTime.Minute;
			int hours = dateTime.Hour << 6;
			int dayOfMonth = dateTime.Day << 11;
			int month = dateTime.Month << 16;
			int year = (dateTime.Year - 1900) << 20;
			writer.Write((uint)(minutes | hours | dayOfMonth | month | year));
		}
	}
	#region DocumentTypographyInfo
	public class DocumentTypographyInfo {
		short flags;
		short followingPunctsLength;
		short leadingPunctsLength;
		char[] followingPuncts;
		char[] leadingPuncts;
		public static DocumentTypographyInfo FromStream(BinaryReader reader) {
			DocumentTypographyInfo result = new DocumentTypographyInfo();
			result.Read(reader);
			return result;
		}
		public DocumentTypographyInfo() {
			this.followingPuncts = new char[101];
			this.leadingPuncts = new char[51];
		}
		protected internal short FollowingPunctsLength { get { return this.followingPunctsLength; } }
		protected internal short LeadingPunctsLength { get { return this.leadingPunctsLength; } }
		protected void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			this.flags = reader.ReadInt16();
			this.followingPunctsLength = reader.ReadInt16();
			this.leadingPunctsLength = reader.ReadInt16();
			char[] buffer = Encoding.Unicode.GetChars(reader.ReadBytes(202));
			for (int i = 0; i < 101; i++) {
				this.followingPuncts[i] = buffer[i];
			}
			buffer = Encoding.Unicode.GetChars(reader.ReadBytes(102));
			for (int i = 0; i < 51; i++) {
				this.leadingPuncts[i] = buffer[i];
			}
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.flags);
			writer.Write(this.followingPunctsLength);
			writer.Write(this.leadingPunctsLength);
			for (int i = 0; i < 101; i++) {
				writer.Write(Encoding.Unicode.GetBytes(new char[] { this.followingPuncts[i] }));
			}
			for (int i = 0; i < 51; i++) {
				writer.Write(Encoding.Unicode.GetBytes(new char[] { this.followingPuncts[i] }));
			}
		}
	}
	#endregion
}
