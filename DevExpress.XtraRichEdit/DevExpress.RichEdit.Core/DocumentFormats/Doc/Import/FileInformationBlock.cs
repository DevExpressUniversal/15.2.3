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
	#region FileInformationFlags
	[Flags]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217")]
	public enum FileInformationFlags {
		TemplateDocument = 0x0001,
		GlossaryDocument = 0x0002,
		ComplexFormat = 0x0004, 
		HasPictures = 0x0008, 
		QuickSaves = 0x00f0, 
		Encryped = 0x0100, 
		TableStreamType = 0x0200, 
		ReadOnlyRecommended = 0x0400, 
		WritePreservation = 0x0800, 
		ExtendedCharset = 0x1000, 
		LoadOverride = 0x2000,
		FarEast = 0x4000,
		Obfuscated = 0x8000
	}
	#endregion
	#region FileInformationBlock
	public class FileInformationBlock {
		#region Fields
		public const int FIBSize = 0x800;
		const int versionPosition = 0x0002;
		const int flagsPosition = 0x000a;
		const int fibBackCompatiblePosition = 0x000c;
		const int word97SavedFlafPosition = 0x0013;
		const int firstCharacterFileOffsetPosition = 0x0018;
		const int lastCharacterFileOffsetPosition = 0x001c;
		const int cswPosition = 0x0020;
		const int magicCreatedPosition = 0x0022;
		const int lidFEposition = 0x003c;
		const int lastByteFileOffsetPosition = 0x0040;
		const int mainDocumentLengthPosition = 0x004c;
		const int insufficientMemoryCHPXPageNumberPosition = 0x006c;
		const int firstCHPXFormattedDiskPageNumberPosition = 0x0070;
		const int insufficientMemoryPAPXPageNumberPosition = 0x0078;
		const int firstPAPXFormattedDiskPageNumberPosition = 0x007c;
		const int insufficientMemoryLVCPageNumberPosition = 0x0084;
		const int cbRgFcLcbPosition = 0x0098;
		const int styleSheetOffsetOriginalPosition = 0x009a;
		const int styleSheetOffsetPosition = 0x00a2;
		const int footnotesReferenceOffsetPosition = 0x00aa;
		const int sectionTableOffsetPosition = 0x00ca;
		const int paragraphHeightsPosition = 0x00da;
		const int headersFootersPositionsOffsetPosition = 0x00f2;
		const int characterTableOffsetPosition = 0x00fa;
		const int paragraphTableOffsetPosition = 0x0102;
		const int fontTableOffsetPosition = 0x0112;
		const int mainDocumentFieldTablePosition = 0x011a;
		const int endNotesFieldTablePosition = 0x021a;
		const int mainDocumentTextBoxesFieldTablePosition = 0x0262;
		const int headerTextBoxesFieldTablePosition = 0x0272;
		const int mainDocumentTextBoxesTextOffsetPosition = 0x025a;
		const int headerTextBoxesTextBoxesTextOffsetPosition = 0x026a;
		const int bookmarkInfoPosition = 0x0142;
		const int documentPropertiesOffsetPosition = 0x0192;
		const int complexFileInformationOffsetPosition = 0x01a2;
		const int mainDocumentFileShapeAddressesOffsetPosition = 0x01da;
		const int headersDocumentFileShapeAddressesOffsetPosition = 0x01e2;
		const int commentsReferenceOffsetAuthor = 0x01ba;
		const int commentsReferenceOffsetPosition = 0x01ea;
		const int endnotesReferenceOffsetPosition = 0x020a;
		const int drawingObjectDataOffsetPosition = 0x022a;
		const int documentVariablesOffsetPosition = 0x027a;
		const int listFormatInformationOffsetPosition = 0x02e2;
		const int listFormatOverrideInformationOffsetPosition = 0x02ea;
		const int rangeEditPermissionsInformationOffsetPosition = 0x0502;
		const int cswNewPosition = 0x05ba;
		const int rmdThreadingPosition = 0x038a;
		const int documentFileRecordsPosition = 0x03b2;
		const ushort fibVersion97 = 0x00c1;
		const ushort fibVersion2003 = 0x010c;
		const ushort product = 0x6027;
		const ushort lid = 0x0419; 
		const ushort lidFE = 0x0409;
		const ushort fibVersionBackCompatible = 0x00bf;
		const ushort csw = 0x000e;
		const ushort cslw = 0x0016;
		const ushort cswNew = 0x0002;
		const ushort cbRgFcLcb = 0x00a4; 
		const ushort magicCreated = 0x6a62;
		const ushort magicCreatedPrivate = 0x554c;
		const int insufficientMemoryPageNumber = 0x000fffff;
		const int revisedDate = 0x00023f2e;
		const byte word97SavedFlag = 0x10;
		public const short WordBinaryFileSignature = unchecked((short)0xa5ec);
		int version;
		#endregion
		#region Properties
		public int Version { get { return version; } }
		public FileInformationFlags Flags { get; protected internal set; }
		public int FirstCharacterFileOffset { get; protected internal set; }
		public int LastCharacterFileOffset { get; set; }
		public int LastByteFileOffset { get; protected internal set; }
		public int FirstCHPXFormattedDiskPageNumber { get; protected internal set; }
		public int CharactersFormattedDiskPageCount { get; protected internal set; }
		public int FirstPAPXFormattedDiskPageNumber { get; protected internal set; }
		public int ParagraphsFormattedDiskPageCount { get; protected internal set; }
		public int MainDocumentLength { get; protected internal set; }
		protected internal int FootNotesStart { get { return MainDocumentLength; } }
		public int FootNotesLength { get; protected internal set; }
		protected internal int HeadersFootersStart { get { return FootNotesStart + FootNotesLength; } }
		public int HeadersFootersLength { get; protected internal set; }
		protected internal int MacroStart { get { return HeadersFootersStart + HeadersFootersLength; } }
		public int MacroLength { get; protected internal set; }
		protected internal int CommentsStart { get { return MacroStart + MacroLength; } }
		public int CommentsLength { get; protected internal set; }
		protected internal int EndnotesStart { get { return CommentsStart + CommentsLength; } }
		public int EndNotesLength { get; protected internal set; }
		protected internal int MainDocumentTextBoxesStart { get { return EndnotesStart + EndNotesLength; } }
		public int MainDocumentTextBoxesLength { get; protected internal set; }
		protected internal int HeaderTextBoxesStart { get { return MainDocumentTextBoxesStart + MainDocumentTextBoxesLength; } }
		public int HeaderTextBoxesLength { get; protected internal set; }
		public int ComplexFileInformationOffset { get; protected internal set; }
		public int ComplexFileInformationSize { get; protected internal set; }
		public int HeadersFootersPositionsOffset { get; protected internal set; }
		public int HeadersFootersPositionsSize { get; protected internal set; }
		public int CharacterTableOffset { get; protected internal set; }
		public int CharacterTableSize { get; protected internal set; }
		public int ParagraphTableOffset { get; protected internal set; }
		public int ParagraphTableSize { get; protected internal set; }
		public int SectionTableOffset { get; protected internal set; }
		public int SectionTableSize { get; protected internal set; }
		public int ParagraphHeightsOffset { get; protected internal set; }
		public int ParagraphHeightsSize { get; protected internal set; }
		public int StyleSheetOffset { get; protected internal set; }
		public int StyleSheetSize { get; protected internal set; }
		public int FootNotesReferenceOffset { get; protected internal set; }
		public int FootNotesReferenceSize { get; protected internal set; }
		public int FootNotesTextOffset { get; protected internal set; }
		public int FootNotesTextSize { get; protected internal set; }
		public int CommentsReferenceOffset { get; protected internal set; }
		public int CommentsReferenceSize { get; protected internal set; }
		public int CommentsTextOffset { get; protected internal set; }
		public int CommentsTextSize { get; protected internal set; }
		public int CommentsNameTableOffset { get; protected internal set; }
		public int CommentsNameTableSize { get; protected internal set; }
		public int CommentsFirstTableOffset { get; protected internal set; }
		public int CommentsFirstTableSize { get; protected internal set; }
		public int CommentsLimTableOffset { get; protected internal set; }
		public int CommentsLimTableSize { get; protected internal set; }
		public int CommentsAuthorTableOffset { get; protected internal set; }
		public int CommentsAuthorTableSize { get; protected internal set; }
		public int MainDocumentTextBoxesTextOffset { get; protected internal set; }
		public int MainDocumentTextBoxesTextSize { get; protected internal set; }
		public int HeaderTextBoxesTextOffset { get; protected internal set; }
		public int HeaderTextBoxesTextSize { get; protected internal set; }
		public int FontTableOffset { get; protected internal set; }
		public int FontTableSize { get; protected internal set; }
		public int MainDocumentFieldTableOffset { get; protected internal set; }
		public int MainDocumentFieldTableSize { get; protected internal set; }
		public int FootNotesFieldTableOffset { get; protected internal set; }
		public int FootNotesFieldTableSize { get; protected internal set; }
		public int HeadersFootersFieldTableOffset { get; protected internal set; }
		public int HeadersFootersFieldTableSize { get; protected internal set; }
		public int CommentsFieldTableOffset { get; protected internal set; }
		public int CommentsFieldTableSize { get; protected internal set; }
		public int EndNotesFieldTableOffset { get; protected internal set; }
		public int EndNotesFieldTableSize { get; protected internal set; }
		public int MainDocumentTextBoxesFieldTableOffset { get; protected internal set; }
		public int MainDocumentTextBoxesFieldTableSize { get; protected internal set; }
		public int HeaderTextBoxesFieldTableOffset { get; protected internal set; }
		public int HeaderTextBoxesFieldTableSize { get; protected internal set; }
		public int BookmarkNamesTableOffset { get; protected internal set; }
		public int BookmarkNamesTableSize { get; protected internal set; }
		public int BookmarkStartInfoOffset { get; protected internal set; }
		public int BookmarkStartInfoSize { get; protected internal set; }
		public int BookmarkEndInfoOffset { get; protected internal set; }
		public int BookmarkEndInfoSize { get; protected internal set; }
		public int MainDocumentFileShapeTableOffset { get; protected internal set; }
		public int MainDocumentFileShapeTableSize { get; protected internal set; }
		public int EndNotesReferenceOffset { get; protected internal set; }
		public int EndNotesReferenceSize { get; protected internal set; }
		public int EndnotesTextOffset { get; protected internal set; }
		public int EndnotesTextSize { get; protected internal set; }
		public int HeadersFootersFileShapeTableOffset { get; protected internal set; }
		public int HeadersFootersFileShapeTableSize { get; protected internal set; }
		public int DrawingObjectTableOffset { get; protected internal set; }
		public int DrawingObjectTableSize { get; protected internal set; }
		public int DocumentVariablesOffset { get; protected internal set; }
		public int DocumentVariablesSize { get; protected internal set; }
		public int ListFormatInformationOffset { get; protected internal set; }
		public int ListFormatInformationSize { get; protected internal set; }
		public int RangeEditPermissionsInformationOffset { get; protected internal set; }
		public int RangeEditPermissionsInformationSize { get; protected internal set; }
		public int RangeEditPermissionsStartInfoOffset { get; protected internal set; }
		public int RangeEditPermissionsStartInfoSize { get; protected internal set; }
		public int RangeEditPermissionsEndInfoOffset { get; protected internal set; }
		public int RangeEditPermissionsEndInfoSize { get; protected internal set; }
		public int RangeEditPermissionsUsernamesOffset { get; protected internal set; }
		public int RangeEditPermissionsUsernamesSize { get; protected internal set; }
		public int ListFormatOverrideInformationOffset { get; protected internal set; }
		public int ListFormatOverrideInformationSize { get; protected internal set; }
		public int MainTextBoxBreakTableOffset { get; protected internal set; }
		public int MainTextBoxBreakTableSize { get; protected internal set; }
		public int HeadersFootersTextBoxBreakTableOffset { get; protected internal set; }
		public int HeadersFootersTextBoxBreakTableSize { get; protected internal set; }
		public int DocumentPropertiesOffset { get; protected internal set; }
		public int DocumentPropertiesSize { get; protected internal set; }
		public int RmdThreadingOffset { get; protected internal set; }
		public int RmdThreadingSize { get; protected internal set; }
		public int DocumentFileRecordsOffset { get; protected internal set; }
		public int DocumentFileRecordsSize { get; protected internal set; }
		#endregion
		public void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			ReadFormatSpecificInfo(reader);
			ReadFormattedDiskPagesInfo(reader);
			ReadComplexFileInfo(reader);
			ReadDocumentStructureInfo(reader);
			ReadReferenceInfo(reader);
			ReadPieceTablesInfo(reader);
			ReadHeadersFootersInfo(reader);
			ReadFontsInfo(reader);
			ReadStyleSheetInfo(reader);
			ReadFieldsInfo(reader);
			ReadListsInfo(reader);
			ReadBookmarksInfo(reader);
			ReadShapesAndDrawingObjectsInfo(reader);
			ReadDocumentVariablesInfo(reader);
			ReadDocumentPropertiesInfo(reader);
			ReadRangeEditPermissionsInfo(reader);
			ReadRmdThreading(reader);
			ReadDocumentFileRecords(reader);
		}
		void ReadFormatSpecificInfo(BinaryReader reader) {
			reader.BaseStream.Seek(versionPosition, SeekOrigin.Begin); 
			this.version = reader.ReadUInt16();
			reader.BaseStream.Seek(flagsPosition, SeekOrigin.Begin);
			Flags = (FileInformationFlags)reader.ReadUInt16();
		}
		void ReadFormattedDiskPagesInfo(BinaryReader reader) {
			reader.BaseStream.Seek(firstCharacterFileOffsetPosition, SeekOrigin.Begin);
			FirstCharacterFileOffset = reader.ReadInt32();
			LastCharacterFileOffset = reader.ReadInt32();
			reader.BaseStream.Seek(lastByteFileOffsetPosition, SeekOrigin.Begin);
			LastByteFileOffset = reader.ReadInt32();
			reader.BaseStream.Seek(firstCHPXFormattedDiskPageNumberPosition, SeekOrigin.Begin);
			FirstCHPXFormattedDiskPageNumber = reader.ReadInt32();
			CharactersFormattedDiskPageCount = reader.ReadInt32();
			reader.BaseStream.Seek(firstPAPXFormattedDiskPageNumberPosition, SeekOrigin.Begin);
			FirstPAPXFormattedDiskPageNumber = reader.ReadInt32();
			ParagraphsFormattedDiskPageCount = reader.ReadInt32();
		}
		void ReadComplexFileInfo(BinaryReader reader) {
			reader.BaseStream.Seek(complexFileInformationOffsetPosition, SeekOrigin.Begin); 
			ComplexFileInformationOffset = reader.ReadInt32();
			ComplexFileInformationSize = reader.ReadInt32(); 
		}
		void ReadDocumentStructureInfo(BinaryReader reader) {
			reader.BaseStream.Seek(mainDocumentLengthPosition, SeekOrigin.Begin);
			MainDocumentLength = reader.ReadInt32();
			FootNotesLength = reader.ReadInt32();
			HeadersFootersLength = reader.ReadInt32();
			MacroLength = reader.ReadInt32();
			CommentsLength = reader.ReadInt32();
			EndNotesLength = reader.ReadInt32();
			MainDocumentTextBoxesLength = reader.ReadInt32();
			HeaderTextBoxesLength = reader.ReadInt32();
		}
		void ReadReferenceInfo(BinaryReader reader) {
			reader.BaseStream.Seek(footnotesReferenceOffsetPosition, SeekOrigin.Begin);
			FootNotesReferenceOffset = reader.ReadInt32();
			FootNotesReferenceSize = reader.ReadInt32();
			FootNotesTextOffset = reader.ReadInt32();
			FootNotesTextSize = reader.ReadInt32();
			CommentsReferenceOffset = reader.ReadInt32();
			CommentsReferenceSize = reader.ReadInt32();
			CommentsTextOffset = reader.ReadInt32();
			CommentsTextSize = reader.ReadInt32();
			reader.BaseStream.Seek(commentsReferenceOffsetAuthor, SeekOrigin.Begin);
			CommentsAuthorTableOffset = reader.ReadInt32();
			CommentsAuthorTableSize = reader.ReadInt32();
			CommentsNameTableOffset = reader.ReadInt32();
			CommentsNameTableSize = reader.ReadInt32();
			reader.BaseStream.Seek(commentsReferenceOffsetPosition, SeekOrigin.Begin);
			CommentsFirstTableOffset = reader.ReadInt32();
			CommentsFirstTableSize = reader.ReadInt32();
			CommentsLimTableOffset = reader.ReadInt32();
			CommentsLimTableSize = reader.ReadInt32();
			reader.BaseStream.Seek(endnotesReferenceOffsetPosition, SeekOrigin.Begin);
			EndNotesReferenceOffset = reader.ReadInt32();
			EndNotesReferenceSize = reader.ReadInt32();
			EndnotesTextOffset = reader.ReadInt32();
			EndnotesTextSize = reader.ReadInt32();
			reader.BaseStream.Seek(mainDocumentTextBoxesTextOffsetPosition, SeekOrigin.Begin);
			MainDocumentTextBoxesTextOffset = reader.ReadInt32();
			MainDocumentTextBoxesTextSize = reader.ReadInt32();
			reader.BaseStream.Seek(headerTextBoxesTextBoxesTextOffsetPosition, SeekOrigin.Begin);
			HeaderTextBoxesTextOffset = reader.ReadInt32();
			HeaderTextBoxesTextSize = reader.ReadInt32();
		}
		void ReadPieceTablesInfo(BinaryReader reader) {
			reader.BaseStream.Seek(characterTableOffsetPosition, SeekOrigin.Begin); 
			CharacterTableOffset = reader.ReadInt32();
			CharacterTableSize = reader.ReadInt32();
			reader.BaseStream.Seek(paragraphTableOffsetPosition, SeekOrigin.Begin); 
			ParagraphTableOffset = reader.ReadInt32();
			ParagraphTableSize = reader.ReadInt32(); 
			reader.BaseStream.Seek(sectionTableOffsetPosition, SeekOrigin.Begin); 
			SectionTableOffset = reader.ReadInt32();
			SectionTableSize = reader.ReadInt32();
			reader.BaseStream.Seek(paragraphHeightsPosition, SeekOrigin.Begin);
			ParagraphHeightsOffset = reader.ReadInt32();
			ParagraphHeightsSize = reader.ReadInt32();
		}
		void ReadHeadersFootersInfo(BinaryReader reader) {
			reader.BaseStream.Seek(headersFootersPositionsOffsetPosition, SeekOrigin.Begin);
			HeadersFootersPositionsOffset = reader.ReadInt32();
			HeadersFootersPositionsSize = reader.ReadInt32();
		}
		void ReadFontsInfo(BinaryReader reader) {
			reader.BaseStream.Seek(fontTableOffsetPosition, SeekOrigin.Begin); 
			FontTableOffset = reader.ReadInt32();
			FontTableSize = reader.ReadInt32();
		}
		void ReadStyleSheetInfo(BinaryReader reader) {
			reader.BaseStream.Seek(styleSheetOffsetPosition, SeekOrigin.Begin); 
			StyleSheetOffset = reader.ReadInt32();
			StyleSheetSize = reader.ReadInt32(); 
		}
		void ReadFieldsInfo(BinaryReader reader) {
			reader.BaseStream.Seek(mainDocumentFieldTablePosition, SeekOrigin.Begin); 
			MainDocumentFieldTableOffset = reader.ReadInt32();
			MainDocumentFieldTableSize = reader.ReadInt32();
			HeadersFootersFieldTableOffset = reader.ReadInt32();
			HeadersFootersFieldTableSize = reader.ReadInt32();
			FootNotesFieldTableOffset = reader.ReadInt32();
			FootNotesFieldTableSize = reader.ReadInt32();
			CommentsFieldTableOffset = reader.ReadInt32();
			CommentsFieldTableSize = reader.ReadInt32();
			reader.BaseStream.Seek(endNotesFieldTablePosition, SeekOrigin.Begin);
			EndNotesFieldTableOffset = reader.ReadInt32();
			EndNotesFieldTableSize = reader.ReadInt32();
			reader.BaseStream.Seek(mainDocumentTextBoxesFieldTablePosition, SeekOrigin.Begin);
			MainDocumentTextBoxesFieldTableOffset = reader.ReadInt32();
			MainDocumentTextBoxesFieldTableSize = reader.ReadInt32();
			reader.BaseStream.Seek(headerTextBoxesFieldTablePosition, SeekOrigin.Begin);
			HeaderTextBoxesFieldTableOffset = reader.ReadInt32();
			HeaderTextBoxesFieldTableSize = reader.ReadInt32();
		}
		void ReadListsInfo(BinaryReader reader) {
			reader.BaseStream.Seek(listFormatInformationOffsetPosition, SeekOrigin.Begin);
			ListFormatInformationOffset = reader.ReadInt32();
			ListFormatInformationSize = reader.ReadInt32();
			reader.BaseStream.Seek(listFormatOverrideInformationOffsetPosition, SeekOrigin.Begin);
			ListFormatOverrideInformationOffset = reader.ReadInt32();
			ListFormatOverrideInformationSize = reader.ReadInt32();
			MainTextBoxBreakTableOffset = reader.ReadInt32();
			MainTextBoxBreakTableSize = reader.ReadInt32();
			HeadersFootersTextBoxBreakTableOffset = reader.ReadInt32();
			HeadersFootersTextBoxBreakTableSize = reader.ReadInt32();
		}
		void ReadBookmarksInfo(BinaryReader reader) {
			reader.BaseStream.Seek(bookmarkInfoPosition, SeekOrigin.Begin);
			BookmarkNamesTableOffset = reader.ReadInt32();
			BookmarkNamesTableSize = reader.ReadInt32();
			BookmarkStartInfoOffset = reader.ReadInt32();
			BookmarkStartInfoSize = reader.ReadInt32();
			BookmarkEndInfoOffset = reader.ReadInt32();
			BookmarkEndInfoSize = reader.ReadInt32();
		}
		void ReadShapesAndDrawingObjectsInfo(BinaryReader reader) {
			reader.BaseStream.Seek(mainDocumentFileShapeAddressesOffsetPosition, SeekOrigin.Begin); 
			MainDocumentFileShapeTableOffset = reader.ReadInt32();
			MainDocumentFileShapeTableSize = reader.ReadInt32(); 
			reader.BaseStream.Seek(headersDocumentFileShapeAddressesOffsetPosition, SeekOrigin.Begin);
			HeadersFootersFileShapeTableOffset = reader.ReadInt32();
			HeadersFootersFileShapeTableSize = reader.ReadInt32();
			reader.BaseStream.Seek(drawingObjectDataOffsetPosition, SeekOrigin.Begin); 
			DrawingObjectTableOffset = reader.ReadInt32();
			DrawingObjectTableSize = reader.ReadInt32(); 
		}
		void ReadDocumentVariablesInfo(BinaryReader reader) {
			reader.BaseStream.Seek(documentVariablesOffsetPosition, SeekOrigin.Begin);
			DocumentVariablesOffset = reader.ReadInt32();
			DocumentVariablesSize = reader.ReadInt32();
		}
		void ReadDocumentPropertiesInfo(BinaryReader reader) {
			reader.BaseStream.Seek(documentPropertiesOffsetPosition, SeekOrigin.Begin); 
			DocumentPropertiesOffset = reader.ReadInt32();
			DocumentPropertiesSize = reader.ReadInt32();
		}
		void ReadRangeEditPermissionsInfo(BinaryReader reader) {
			reader.BaseStream.Seek(rangeEditPermissionsInformationOffsetPosition, SeekOrigin.Begin);
			RangeEditPermissionsInformationOffset = reader.ReadInt32();
			RangeEditPermissionsInformationSize = reader.ReadInt32();
			RangeEditPermissionsStartInfoOffset = reader.ReadInt32();
			RangeEditPermissionsStartInfoSize = reader.ReadInt32();
			RangeEditPermissionsEndInfoOffset = reader.ReadInt32();
			RangeEditPermissionsEndInfoSize = reader.ReadInt32();
			RangeEditPermissionsUsernamesOffset = reader.ReadInt32();
			RangeEditPermissionsUsernamesSize = reader.ReadInt32();
		}
		void ReadRmdThreading(BinaryReader reader) {
			reader.BaseStream.Seek(rmdThreadingPosition, SeekOrigin.Begin);
			RmdThreadingOffset = reader.ReadInt32();
			RmdThreadingSize = reader.ReadInt32();
		}
		void ReadDocumentFileRecords(BinaryReader reader) {
			reader.BaseStream.Seek(documentFileRecordsPosition, SeekOrigin.Begin);
			DocumentFileRecordsOffset = reader.ReadInt32();
			DocumentFileRecordsSize = reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			WriteFormatSpecificInfo(writer);
			WriteFormattedDiskPagesInfo(writer);
			WriteComplexFileInfo(writer);
			WriteDocumentStructureInfo(writer);
			WriteInsufficientMemoryInfo(writer);
			WriteReferenceInfo(writer);
			WritePieceTableInfo(writer);
			WriteHeadersFootersInfo(writer);
			WriteFontsInfo(writer);
			WriteStyleSheetInfo(writer);
			WriteFieldsInfo(writer);
			WriteListInfo(writer);
			WriteBookmarksInfo(writer);
			WriteShapesAndDrawingObjectsInfo(writer);
			WriteDocumentVariablesInfo(writer);
			WriteDocumentPropertiesInfo(writer);
			WriteRangeEditPermissionsInfo(writer);
			WriteRmdThreading(writer);
		}
		void WriteFormatSpecificInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(0, SeekOrigin.Begin);
			writer.Write(WordBinaryFileSignature);
			writer.Write((ushort)fibVersion97);
			writer.Write(product);
			writer.Write(lid);
			writer.BaseStream.Seek(flagsPosition, SeekOrigin.Begin);
			writer.Write((ushort)Flags);
			writer.Write(fibVersionBackCompatible);
			writer.BaseStream.Seek(word97SavedFlafPosition, SeekOrigin.Begin);
			writer.Write(word97SavedFlag);
			writer.BaseStream.Seek(cswPosition, SeekOrigin.Begin);
			writer.Write(csw);
			writer.BaseStream.Seek(lidFEposition, SeekOrigin.Begin);
			writer.Write(lidFE);
			writer.Write(cslw);
			writer.BaseStream.Seek(cbRgFcLcbPosition, SeekOrigin.Begin);
			writer.Write(cbRgFcLcb);
			writer.BaseStream.Seek(magicCreatedPosition, SeekOrigin.Begin);
			writer.Write(magicCreated);
			writer.Write(magicCreated);
			writer.Write(magicCreatedPrivate);
			writer.Write(magicCreatedPrivate);
			writer.BaseStream.Seek(cswNewPosition, SeekOrigin.Begin);
			writer.Write(cswNew);
			writer.Write(fibVersion2003);
			writer.Write((ushort)0);
		}
		void WriteReferenceInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(footnotesReferenceOffsetPosition, SeekOrigin.Begin);
			writer.Write(FootNotesReferenceOffset);
			writer.Write((uint)FootNotesReferenceSize);
			writer.Write(FootNotesTextOffset);
			writer.Write((uint)FootNotesTextSize);
			writer.Write(CommentsReferenceOffset);
			writer.Write((uint)CommentsReferenceSize);
			writer.Write(CommentsTextOffset);
			writer.Write((uint)CommentsTextSize);
			writer.BaseStream.Seek(commentsReferenceOffsetAuthor, SeekOrigin.Begin);
			writer.Write(CommentsAuthorTableOffset);
			writer.Write((uint)CommentsAuthorTableSize);
			writer.Write(CommentsNameTableOffset);
			writer.Write((uint)CommentsNameTableSize);
			writer.BaseStream.Seek(commentsReferenceOffsetPosition, SeekOrigin.Begin);
			writer.Write(CommentsFirstTableOffset);
			writer.Write((uint)CommentsFirstTableSize);
			writer.Write(CommentsLimTableOffset);
			writer.Write((uint)CommentsLimTableSize);
			writer.BaseStream.Seek(endnotesReferenceOffsetPosition, SeekOrigin.Begin);
			writer.Write(EndNotesReferenceOffset);
			writer.Write((uint)EndNotesReferenceSize);
			writer.Write(EndnotesTextOffset);
			writer.Write((uint)EndnotesTextSize);
			writer.BaseStream.Seek(mainDocumentTextBoxesTextOffsetPosition, SeekOrigin.Begin);
			writer.Write(MainDocumentTextBoxesTextOffset);
			writer.Write((uint)MainDocumentTextBoxesTextSize);
			writer.BaseStream.Seek(headerTextBoxesTextBoxesTextOffsetPosition, SeekOrigin.Begin);
			writer.Write(HeaderTextBoxesTextOffset);
			writer.Write((uint)HeaderTextBoxesTextSize);
		}
		void WriteHeadersFootersInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(headersFootersPositionsOffsetPosition, SeekOrigin.Begin);
			writer.Write(HeadersFootersPositionsOffset);
			writer.Write((uint)HeadersFootersPositionsSize);
		}
		void WriteFormattedDiskPagesInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(firstCharacterFileOffsetPosition, SeekOrigin.Begin);
			writer.Write(FirstCharacterFileOffset);
			writer.Write(LastCharacterFileOffset);
			writer.BaseStream.Seek(lastByteFileOffsetPosition, SeekOrigin.Begin);
			writer.Write(LastByteFileOffset);
			writer.Write(revisedDate);
			writer.Write(revisedDate);
			writer.BaseStream.Seek(firstCHPXFormattedDiskPageNumberPosition, SeekOrigin.Begin);
			writer.Write(FirstCHPXFormattedDiskPageNumber);
			writer.Write(CharactersFormattedDiskPageCount);
			writer.BaseStream.Seek(firstPAPXFormattedDiskPageNumberPosition, SeekOrigin.Begin);
			writer.Write(FirstPAPXFormattedDiskPageNumber);
			writer.Write(ParagraphsFormattedDiskPageCount);
		}
		void WritePieceTableInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(characterTableOffsetPosition, SeekOrigin.Begin);
			writer.Write(CharacterTableOffset);
			writer.Write((uint)CharacterTableSize);
			writer.BaseStream.Seek(paragraphTableOffsetPosition, SeekOrigin.Begin);
			writer.Write(ParagraphTableOffset);
			writer.Write((uint)ParagraphTableSize);
			writer.BaseStream.Seek(sectionTableOffsetPosition, SeekOrigin.Begin);
			writer.Write(SectionTableOffset);
			writer.Write((uint)SectionTableSize);
			writer.BaseStream.Seek(paragraphHeightsPosition, SeekOrigin.Begin);
			writer.Write(ParagraphHeightsOffset);
			writer.Write((uint)ParagraphHeightsSize);
		}
		void WriteDocumentStructureInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(mainDocumentLengthPosition, SeekOrigin.Begin);
			writer.Write(MainDocumentLength);
			writer.Write(FootNotesLength);
			writer.Write(HeadersFootersLength);
			writer.Write(MacroLength);
			writer.Write(CommentsLength);
			writer.Write(EndNotesLength);
			writer.Write(MainDocumentTextBoxesLength);
			writer.Write(HeaderTextBoxesLength);
		}
		void WriteInsufficientMemoryInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(insufficientMemoryCHPXPageNumberPosition, SeekOrigin.Begin);
			writer.Write(insufficientMemoryPageNumber);
			writer.BaseStream.Seek(insufficientMemoryPAPXPageNumberPosition, SeekOrigin.Begin);
			writer.Write(insufficientMemoryPageNumber);
			writer.BaseStream.Seek(insufficientMemoryLVCPageNumberPosition, SeekOrigin.Begin);
			writer.Write(insufficientMemoryPageNumber);
		}
		void WriteStyleSheetInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(styleSheetOffsetOriginalPosition, SeekOrigin.Begin);
			writer.Write(StyleSheetOffset);
			writer.Write((uint)StyleSheetSize);
			writer.Write(StyleSheetOffset);
			writer.Write((uint)StyleSheetSize);
		}
		void WriteFieldsInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(mainDocumentFieldTablePosition, SeekOrigin.Begin);
			writer.Write(MainDocumentFieldTableOffset);
			writer.Write((uint)MainDocumentFieldTableSize);
			writer.Write(HeadersFootersFieldTableOffset);
			writer.Write((uint)HeadersFootersFieldTableSize);
			writer.Write(FootNotesFieldTableOffset);
			writer.Write((uint)FootNotesFieldTableSize);
			writer.Write(CommentsFieldTableOffset);
			writer.Write((uint)CommentsFieldTableSize);
			writer.BaseStream.Seek(endNotesFieldTablePosition, SeekOrigin.Begin);
			writer.Write(EndNotesFieldTableOffset);
			writer.Write((uint)EndNotesFieldTableSize);
			writer.BaseStream.Seek(mainDocumentTextBoxesFieldTablePosition, SeekOrigin.Begin);
			writer.Write(MainDocumentTextBoxesFieldTableOffset);
			writer.Write((uint)MainDocumentTextBoxesFieldTableSize);
			writer.BaseStream.Seek(headerTextBoxesFieldTablePosition, SeekOrigin.Begin);
			writer.Write(HeaderTextBoxesFieldTableOffset);
			writer.Write((uint)HeaderTextBoxesFieldTableSize);
		}
		void WriteFontsInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(fontTableOffsetPosition, SeekOrigin.Begin);
			writer.Write(FontTableOffset);
			writer.Write((uint)FontTableSize);
		}
		void WriteBookmarksInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(bookmarkInfoPosition, SeekOrigin.Begin);
			writer.Write(BookmarkNamesTableOffset);
			writer.Write((uint)BookmarkNamesTableSize);
			writer.Write(BookmarkStartInfoOffset);
			writer.Write((uint)BookmarkStartInfoSize);
			writer.Write(BookmarkEndInfoOffset);
			writer.Write((uint)BookmarkEndInfoSize);
		}
		void WriteComplexFileInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(complexFileInformationOffsetPosition, SeekOrigin.Begin);
			writer.Write(ComplexFileInformationOffset);
			writer.Write((uint)ComplexFileInformationSize);
		}
		void WriteShapesAndDrawingObjectsInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(mainDocumentFileShapeAddressesOffsetPosition, SeekOrigin.Begin);
			writer.Write(MainDocumentFileShapeTableOffset);
			writer.Write((uint)MainDocumentFileShapeTableSize);
			writer.BaseStream.Seek(headersDocumentFileShapeAddressesOffsetPosition, SeekOrigin.Begin);
			writer.Write(HeadersFootersFileShapeTableOffset);
			writer.Write((uint)HeadersFootersFileShapeTableSize);
			writer.BaseStream.Seek(drawingObjectDataOffsetPosition, SeekOrigin.Begin);
			writer.Write(DrawingObjectTableOffset);
			writer.Write((uint)DrawingObjectTableSize);
		}
		void WriteListInfo(BinaryWriter writer) {
			if (DocumentFileRecordsSize != 0) {
				writer.BaseStream.Seek(documentFileRecordsPosition, SeekOrigin.Begin);
				writer.Write(DocumentFileRecordsOffset);
				writer.Write((uint)DocumentFileRecordsSize);
			}
			writer.BaseStream.Seek(listFormatInformationOffsetPosition, SeekOrigin.Begin);
			writer.Write(ListFormatInformationOffset);
			writer.Write((uint)ListFormatInformationSize);
			writer.BaseStream.Seek(listFormatOverrideInformationOffsetPosition, SeekOrigin.Begin);
			writer.Write(ListFormatOverrideInformationOffset);
			writer.Write((uint)ListFormatOverrideInformationSize);
			writer.Write(MainTextBoxBreakTableOffset);
			writer.Write((uint)MainTextBoxBreakTableSize);
			writer.Write(HeadersFootersTextBoxBreakTableOffset);
			writer.Write((uint)HeadersFootersTextBoxBreakTableSize);
		}
		void WriteDocumentVariablesInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(documentVariablesOffsetPosition, SeekOrigin.Begin);
			writer.Write(DocumentVariablesOffset);
			writer.Write(DocumentVariablesSize);
		}
		void WriteDocumentPropertiesInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(documentPropertiesOffsetPosition, SeekOrigin.Begin);
			writer.Write(DocumentPropertiesOffset);
			writer.Write((uint)DocumentPropertiesSize);
		}
		void WriteRangeEditPermissionsInfo(BinaryWriter writer) {
			writer.BaseStream.Seek(rangeEditPermissionsInformationOffsetPosition, SeekOrigin.Begin);
			writer.Write(RangeEditPermissionsInformationOffset);
			writer.Write((uint)RangeEditPermissionsInformationSize);
			writer.Write(RangeEditPermissionsStartInfoOffset);
			writer.Write((uint)RangeEditPermissionsStartInfoSize);
			writer.Write(RangeEditPermissionsEndInfoOffset);
			writer.Write((uint)RangeEditPermissionsEndInfoSize);
			writer.Write(RangeEditPermissionsUsernamesOffset);
			writer.Write((uint)RangeEditPermissionsUsernamesSize);
		}
		void WriteRmdThreading(BinaryWriter writer) {
			writer.BaseStream.Seek(rmdThreadingPosition, SeekOrigin.Begin);
			writer.Write(RmdThreadingOffset);
			writer.Write((uint)RmdThreadingSize);
		}
	}
	#endregion
}
