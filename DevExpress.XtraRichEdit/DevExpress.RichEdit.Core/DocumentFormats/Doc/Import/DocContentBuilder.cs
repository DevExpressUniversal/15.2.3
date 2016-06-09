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
using DevExpress.XtraRichEdit.Export.Doc;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocumentStreams
	public static class DocumentStreams {
		public static readonly string MainStreamName = "WordDocument";
		public static readonly string Stream0TableName = "0Table";
		public static readonly string Stream1TableName = "1Table";
		public static readonly string DataStreamName = "Data";
		public static readonly string SummaryInformationName = "\u0005SummaryInformation";
		public static readonly string DocumentSummaryInformationName = "\u0005DocumentSummaryInformation";
	}
	#endregion
	public delegate void ProcessSymbolDelegate(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer);
	#region DocContentBuilder
	public class DocContentBuilder : IBinaryContentBuilder, IDisposable {
		#region Fields
		public const int LastSupportedVersion = 105; 
		public const int SectorSize = 512;
		PackageFileReader packageFileHelper;
		List<TextRunBorder> textRunBorders;
		VirtualStreamBinaryReader mainStreamReader;
		BinaryReader dataStreamReader;
		DocPieceTable pieceTable;
		FormattedDiskPageHelper fkpHelper;
		SectionPropertiesHelper sectionPropertiesHelper;
		DocCommandFactory factory;
		DocStyleSheet styleSheet;
		DocDocumentProperties docProperties;
		DocumentVariables docVariables;
		DocListFormatInformation listFormatInfo;
		DocListOverrideFormatInformation listFormatOverrideInfo;
		DocumentFileRecords docFileRecords;
		DocContentIterator iterator;
		Dictionary<char, ProcessSymbolDelegate> specialSymbolProcessors;
		Dictionary<char, ProcessSymbolDelegate> expectedSpecialSymbolsProcessors;
		DocDocumentImporterOptions options;
		#endregion
		public DocContentBuilder(DocumentModel model, DocDocumentImporterOptions options) {
			this.factory = new DocCommandFactory(model);
			this.options = options;
		}
		#region Properties
		protected internal DocPieceTable PieceTable { get { return pieceTable; } }
		protected internal FormattedDiskPageHelper FKPHelper { get { return fkpHelper; } }
		protected internal SectionPropertiesHelper SectionPropertiesHelper { get { return this.sectionPropertiesHelper; } }
		protected internal VirtualStreamBinaryReader MainStreamReader { get { return mainStreamReader; } }
		protected internal BinaryReader DataReader { get { return dataStreamReader; } }
		protected internal DocCommandFactory Factory { get { return factory; } }
		public PackageFileReader FileReader { get { return packageFileHelper; } }
		public DocContentIterator Iterator { get { return iterator; } }
		public DocListFormatInformation ListInfo { get { return listFormatInfo; } }
		public DocListOverrideFormatInformation ListOverrideInfo { get { return listFormatOverrideInfo; } }
		public DocStyleSheet StyleSheet { get { return styleSheet; } }
		public DocFontManager FontManager { get; private set; }
		public DocDocumentProperties DocumentProperties { get { return docProperties; } }
		public DocumentVariables DocumentVariables { get { return docVariables; } }
		public DocumentFileRecords DocFileRecords { get { return docFileRecords; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.packageFileHelper != null) {
					this.packageFileHelper.Dispose();
					this.packageFileHelper = null;
				}
				if (this.factory != null) {
					this.factory.Dispose();
					this.factory = null;
				}
				if (this.fkpHelper != null) {
					this.fkpHelper.Dispose();
					this.fkpHelper = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DocContentBuilder() {
			Dispose(false);
		}
		#endregion
		public void BuildDocumentContent(Stream stream, DocumentModel model) {
			OpenPackage(stream);
			Initialize(model);
			ReadTextStream();
		}
		void OpenPackage(Stream stream) {
			this.packageFileHelper = new PackageFileReader(stream);
			this.textRunBorders = new List<TextRunBorder>();
			this.mainStreamReader = this.packageFileHelper.GetCachedPackageFileReader(DocumentStreams.MainStreamName);
			this.dataStreamReader = this.packageFileHelper.GetCachedPackageFileReader(DocumentStreams.DataStreamName);
			if (mainStreamReader == null)
				DocImporter.ThrowInvalidDocFile();
		}
		void Initialize(DocumentModel model) {
			FileInformationBlock fib = new FileInformationBlock();
			fib.Read(MainStreamReader);
			this.factory.Version = fib.Version;
			CheckFileInfo(fib);
			string tableStreamName = ((fib.Flags & FileInformationFlags.TableStreamType) != 0) ? DocumentStreams.Stream1TableName 
				: DocumentStreams.Stream0TableName;
			BinaryReader tableStreamReader = FileReader.GetCachedPackageFileReader(tableStreamName);
			if (tableStreamReader == null)
				DocImporter.ThrowInvalidDocFile();
			CreateSpecialSymbolsProcessors();
			CreateExpectedSpecialSymbolsProcessors();
			GetTableStreamInformation(tableStreamReader, fib);
			CalculateTextRunBorders();
			this.iterator = new DocContentIterator(fib, MainStreamReader, tableStreamReader, model);
		}
		void CheckFileInfo(FileInformationBlock fib) {
			if (fib.Version <= LastSupportedVersion)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UnsupportedDocVersion);
			if ((fib.Flags & FileInformationFlags.Encryped) == FileInformationFlags.Encryped)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_EncryptedFile);
		}
		void CreateSpecialSymbolsProcessors() {
			this.specialSymbolProcessors = new Dictionary<char, ProcessSymbolDelegate>(7);
			this.specialSymbolProcessors.Add(TextCodes.InlinePicture, InsertInlinePicture);
			this.specialSymbolProcessors.Add(TextCodes.AutoNumberedFootNoteReference, InsertAutoNumberedFootnoteReference);
			this.specialSymbolProcessors.Add(TextCodes.AnnotationReference, InsertAnnotationReference);
			this.specialSymbolProcessors.Add(TextCodes.FloatingObjectAnchor, InsertFloatingObject);
			this.specialSymbolProcessors.Add(TextCodes.FieldBegin, InsertFieldBegin);
			this.specialSymbolProcessors.Add(TextCodes.FieldSeparator, InsertFieldSeparator);
			this.specialSymbolProcessors.Add(TextCodes.FieldEnd, InsertFieldEnd);
			this.specialSymbolProcessors.Add(TextCodes.SpecialSymbol, InsertSpecialSymbol);
		}
		void CreateExpectedSpecialSymbolsProcessors() {
			this.expectedSpecialSymbolsProcessors = new Dictionary<char,ProcessSymbolDelegate>();
			this.expectedSpecialSymbolsProcessors.Add(TextCodes.AutoNumberedFootNoteReference, InsertAutoNumberedFootnoteReference);
			this.expectedSpecialSymbolsProcessors.Add(TextCodes.FieldBegin, InsertExpectedFieldBegin);
			this.expectedSpecialSymbolsProcessors.Add(TextCodes.FieldSeparator, InsertExpectedFieldSeparator);
			this.expectedSpecialSymbolsProcessors.Add(TextCodes.FieldEnd, InsertExpectedFieldEnd);
		}
		void GetTableStreamInformation(BinaryReader tableStreamReader, FileInformationBlock fib) {
			this.pieceTable = GetPieceTable(tableStreamReader, fib);
			this.fkpHelper = GetFormattedDiskPageHelper(tableStreamReader, fib);
			this.styleSheet = DocStyleSheet.FromStream(tableStreamReader, fib.StyleSheetOffset, fib.StyleSheetSize);
			if (fib.FontTableSize != 0)
				FontManager = new DocFontManager(tableStreamReader, fib.FontTableOffset);
			this.sectionPropertiesHelper = SectionPropertiesHelper.FromStream(MainStreamReader, tableStreamReader, fib.SectionTableOffset, fib.SectionTableSize);
			GetListInformation(tableStreamReader, fib);
			this.docProperties = DocDocumentProperties.FromStream(tableStreamReader, fib.DocumentPropertiesOffset);
			this.factory.DocumentProperties = this.docProperties;
			this.docVariables = DocumentVariables.FromStream(tableStreamReader, fib.DocumentVariablesOffset, fib.DocumentVariablesSize);
			this.docFileRecords = DocumentFileRecords.FromStream(tableStreamReader, fib.DocumentFileRecordsOffset, fib.DocumentFileRecordsSize);
		}
		void CalculateTextRunBorders() {
			this.textRunBorders = FKPHelper.GetTextRunBorders();
			int count = PieceTable.PcdCount;
			for (int i = 0; i < count; i++) {
				CheckPieceDescriptorBorders(i);
			}
		}
		void CheckPieceDescriptorBorders(int i) {
			int offset = PieceTable.GetOffset(i);
			int length = PieceTable.GetLength(i);
			int index = Algorithms.BinarySearch(textRunBorders, new TextRunBorderComparable(offset));
			TextRunBorder textRunBorder;
			if (index < 0) {
				textRunBorder = new TextRunBorder(offset, TextRunStartReasons.TextRunMark);
				this.textRunBorders.Insert(~index, textRunBorder);
			}
			index = Algorithms.BinarySearch(textRunBorders, new TextRunBorderComparable(offset + length));
			if (index < 0) {
				textRunBorder = new TextRunBorder(offset + length, TextRunStartReasons.TextRunMark);
				this.textRunBorders.Insert(~index, textRunBorder);
			}
		}
		DocPieceTable GetPieceTable(BinaryReader reader, FileInformationBlock fib) {
			ComplexFileInformation clxInfo = ComplexFileInformation.FromStream(reader, fib.ComplexFileInformationOffset, fib.ComplexFileInformationSize);
			if (clxInfo.PieceTable != null)
				return DocPieceTable.FromByteArray(clxInfo.PieceTable);
			int lastCharacterPosition = fib.MainDocumentLength + fib.FootNotesLength + fib.HeadersFootersLength;
			return DocPieceTable.CreateDefault(fib.FirstCharacterFileOffset, lastCharacterPosition);
		}
		FormattedDiskPageHelper GetFormattedDiskPageHelper(BinaryReader reader, FileInformationBlock fib) {
			long mainStreamLength = MainStreamReader.BaseStream.Length;
			BinTable paragraphsBinTable = BinTable.FromStream(reader, fib.ParagraphTableOffset, fib.ParagraphTableSize, mainStreamLength);
			BinTable charactersBinTable = BinTable.FromStream(reader, fib.CharacterTableOffset, fib.CharacterTableSize, mainStreamLength);
			return new FormattedDiskPageHelper(MainStreamReader, DataReader, paragraphsBinTable, charactersBinTable);
		}
		void GetListInformation(BinaryReader reader, FileInformationBlock fib) {
			this.listFormatInfo = DocListFormatInformation.FromStream(reader, fib.ListFormatInformationOffset, fib.ListFormatInformationSize);
			this.listFormatOverrideInfo = DocListOverrideFormatInformation.FromStream(reader, fib.ListFormatOverrideInformationOffset, fib.ListFormatOverrideInformationSize);
		}
		void ReadTextStream() {
			int count = PieceTable.PcdCount;
			for (int i = 0; i < count; i++) {
				ProcessPieceDescriptor(i);
			}
			DocPropertyContainer lastPropertiesContainer = Iterator.MainTextDocObjects[Iterator.MainTextDocObjects.Count - 1].PropertyContainer;
			SectionPropertiesHelper.UpdateCurrentSectionProperties(MainStreamReader, DataReader, lastPropertiesContainer);
		}
		void ProcessPieceDescriptor(int pcdIndex) {
			List<TextRunBorder> innerBorders = GetInnerBorders(pcdIndex);
			int count = innerBorders.Count - 1;
			for (int i = 0; i < count; i++)
				if (Iterator.ShouldProcessTextRun())
					ProcessTextRunBorder(pcdIndex, innerBorders[i], innerBorders[i + 1]);
		}
		public void ProcessTextRunBorder(int pcdIndex, TextRunBorder currentBorder, TextRunBorder nextBorder) {
			DocPropertyContainer propertyContainer = FKPHelper.UpdateCharacterProperties(currentBorder.Offset, Factory);
			DocObjectInfo objectInfo = GetDocObjectInfo(pcdIndex, currentBorder, nextBorder);
			if (!(propertyContainer.IsDeleted && options.IgnoreDeletedText)) {
				ProcessObjectInfo(objectInfo, propertyContainer);
				Iterator.Destination.AddRange(GetBorderDocObjects(nextBorder.Reason, currentBorder.Offset, objectInfo, propertyContainer));
			}
			Iterator.UpdateState();
		}
		void ProcessObjectInfo(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			if (String.IsNullOrEmpty(objectInfo.Text))
				return;
			DocObjectCollection destination = Iterator.Destination;
			if (propertyContainer.CharacterInfo != null && propertyContainer.CharacterInfo.Special)
				ProcessNonTextCharacters(destination, objectInfo, propertyContainer);
			else
				ProcessTextCharacters(destination, objectInfo, propertyContainer);
		}
		void ProcessTextCharacters(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			char ch = objectInfo.Text[0];
			ProcessSymbolDelegate expectedSymbolDelegate;
			if (this.expectedSpecialSymbolsProcessors.TryGetValue(ch, out expectedSymbolDelegate))
				expectedSymbolDelegate(destination, objectInfo, propertyContainer);
			else {
				destination.Add(DocObjectFactory.Instance.CreateDocObject(DocObjectType.TextRun, objectInfo, propertyContainer));
				objectInfo.Position += objectInfo.Text.Length; 
			}
		}
		void ProcessNonTextCharacters(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			int count = objectInfo.Text.Length;
			for (int i = 0; i < count; i++) {
				char ch = objectInfo.Text[i];
				ProcessSymbolDelegate specialCharacterProcessor;
				if (!this.specialSymbolProcessors.TryGetValue(ch, out specialCharacterProcessor))
					specialCharacterProcessor = SkipUnsupportedObject;
				specialCharacterProcessor(destination, objectInfo, propertyContainer);
				objectInfo.Position++;
			}
		}
		DocObjectCollection GetBorderDocObjects(TextRunStartReasons reason, int offset, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			DocObjectCollection result = new DocObjectCollection();
			if ((reason & TextRunStartReasons.ParagraphMark) == TextRunStartReasons.ParagraphMark)
				result.Add(GetDocObjectByParagraphMark(offset, objectInfo, propertyContainer));
			if((reason & TextRunStartReasons.SectionMark) == TextRunStartReasons.SectionMark) {
				FKPHelper.UpdateParagraphProperties(offset, propertyContainer);
				result.Add(GetSection(objectInfo, propertyContainer));
			}
			if ((reason & TextRunStartReasons.TableUnitMark) == TextRunStartReasons.TableUnitMark) {
				FKPHelper.UpdateParagraphProperties(offset, propertyContainer);
				result.Add(GetTableUnit(objectInfo, propertyContainer));
			}
			return result;
		}
		IDocObject GetDocObjectByParagraphMark(int offset, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			FKPHelper.UpdateParagraphProperties(offset, propertyContainer);
			if (propertyContainer.ParagraphInfo != null && propertyContainer.ParagraphInfo.NestedTableTrailer)
				return DocObjectFactory.Instance.CreateDocObject(DocObjectType.TableRow, objectInfo, propertyContainer);
			if (propertyContainer.ParagraphInfo != null && propertyContainer.ParagraphInfo.InnerTableCell)
				return DocObjectFactory.Instance.CreateDocObject(DocObjectType.TableCell, objectInfo, propertyContainer);
			return DocObjectFactory.Instance.CreateDocObject(DocObjectType.Paragraph, objectInfo, propertyContainer);
		}
		DocObjectInfo GetDocObjectInfo(int pcdIndex, TextRunBorder currentBorder, TextRunBorder nextBorder) {
			string text = GetString(currentBorder.Offset, nextBorder.Offset - currentBorder.Offset, PieceTable.GetEncoding(pcdIndex));
			int position = Iterator.AdvanceNext(text.Length);
			if ((nextBorder.Reason & TextRunStartReasons.ParagraphMark) == 0)
				return new DocObjectInfo(position, text);
			CorrectParagraphBorderType(nextBorder, pcdIndex);
			if ((nextBorder.Reason & TextRunStartReasons.ColumnBreak) != TextRunStartReasons.ColumnBreak)
				text = text.Remove(text.Length - 1);
			return new DocObjectInfo(position, text);
		}
		DocImageCollection GetPictures(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			PictureDescriptor pictureDescriptor = PictureDescriptor.FromStream(DataReader, propertyContainer.DataStreamOffset);
			return new DocImageCollection(objectInfo, propertyContainer, pictureDescriptor);
		}
		DocFloatingObjectBase GetFloatingObject(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			return Iterator.GetFloatingObject(objectInfo, propertyContainer);
		}
		DocObjectBase GetFieldData(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			FieldType fieldType = Iterator.GetCurrentFieldType();
			switch (fieldType) {
				case FieldType.Hyperlink:
					DocHyperlinkInfo hyperlinkInfo = new DocHyperlinkInfo(DataReader, propertyContainer.DataStreamOffset);
					return new DocHyperlinkFieldData(objectInfo, propertyContainer, hyperlinkInfo);
				default:
					return null;
			}
		}
		DocObjectBase GetTableUnit(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			if (propertyContainer.ParagraphInfo != null && propertyContainer.ParagraphInfo.TableTrailer)
				return new DocTableRow(objectInfo, propertyContainer);
			else
				return new DocTableCell(objectInfo, propertyContainer);
		}
		DocSection GetSection(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			SectionPropertiesHelper.UpdateCurrentSectionProperties(MainStreamReader, DataReader, propertyContainer);
			return new DocSection(objectInfo, propertyContainer);
		}
		void SkipUnsupportedObject(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
		}
		void InsertFloatingObject(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			DocFloatingObjectBase floatingObject = GetFloatingObject(objectInfo, propertyContainer);
			if (floatingObject != null)
				destination.Add(floatingObject);
			else
				destination.Add(new UnsupportedObject(objectInfo.Position, propertyContainer));
		}
		void InsertAutoNumberedFootnoteReference(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			destination.Add(Iterator.GetNoteObject(objectInfo, propertyContainer));
		}
		void InsertAnnotationReference(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			destination.Add(DocObjectFactory.Instance.CreateDocObject(DocObjectType.AnnotationReference, objectInfo, propertyContainer));
		}
		void InsertInlinePicture(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			if (propertyContainer.CharacterInfo != null && propertyContainer.CharacterInfo.BinaryData) {
				InsertFieldData(destination, objectInfo, propertyContainer);
				return;
			}
			if (propertyContainer.CharacterInfo != null && propertyContainer.CharacterInfo.Ole2Object)
				return;
			InsertInlinePictureCore(destination, objectInfo, propertyContainer);
		}
		void InsertInlinePictureCore(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			DocImageCollection pictures = GetPictures(objectInfo, propertyContainer);
			int count = pictures.Count;
			for (int pictureIndex = 0; pictureIndex < count; pictureIndex++) {
				destination.Add(pictures[pictureIndex]);
			}
		}
		void InsertFieldData(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			DocObjectBase fieldData = GetFieldData(objectInfo, propertyContainer);
			if (fieldData != null)
				destination.Add(fieldData);
		}
		void InsertFieldBegin(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			if (Iterator.AdvanceField(propertyContainer))
				destination.Add(DocObjectFactory.Instance.CreateDocObject(DocObjectType.FieldBegin, objectInfo, propertyContainer));
		}
		void InsertFieldSeparator(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			if (Iterator.AdvanceField(propertyContainer))
				destination.Add(DocObjectFactory.Instance.CreateDocObject(DocObjectType.FieldSeparator, objectInfo, propertyContainer));
		}
		void InsertFieldEnd(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			if (Iterator.AdvanceField(propertyContainer)) {
				destination.Add(DocObjectFactory.Instance.CreateDocObject(DocObjectType.FieldEnd, objectInfo, propertyContainer));
				Iterator.CheckFieldCompatibility(destination);
			}
		}
		void InsertExpectedFieldBegin(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			destination.Add(DocObjectFactory.Instance.CreateDocObject(DocObjectType.ExpectedFieldBegin, objectInfo, propertyContainer));
		}
		void InsertExpectedFieldSeparator(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			destination.Add(DocObjectFactory.Instance.CreateDocObject(DocObjectType.ExpectedFieldSeparator, objectInfo, propertyContainer));
		}
		void InsertExpectedFieldEnd(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			destination.Add(DocObjectFactory.Instance.CreateDocObject(DocObjectType.ExpectedFieldEnd, objectInfo, propertyContainer));
		}
		void InsertSpecialSymbol(DocObjectCollection destination, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			if (propertyContainer.CharacterInfo == null)
				return;
			string specialCharacter = new string(new char[] { propertyContainer.CharacterInfo.Symbol });
			DocObjectInfo symbolInfo = new DocObjectInfo(objectInfo.Position, specialCharacter);
			destination.Add(DocObjectFactory.Instance.CreateDocObject(DocObjectType.TextRun, symbolInfo, propertyContainer));
		}
		List<TextRunBorder> GetInnerBorders(int pcdIndex) {
			IList<TextRunBorder> borders = this.textRunBorders;
			List<TextRunBorder> innerBorders = new List<TextRunBorder>();
			int offset = PieceTable.GetOffset(pcdIndex);
			int length = PieceTable.GetLength(pcdIndex);
			int lowIndex = Algorithms.BinarySearch<TextRunBorder>(borders, new TextRunBorderComparable(offset));
			int highIndex = Algorithms.BinarySearch<TextRunBorder>(borders, new TextRunBorderComparable(offset + length));
			for (int i = lowIndex; i <= highIndex; i++) {
				innerBorders.Add(borders[i]);
			}
			return innerBorders;
		}
		string GetString(int offset, int length, Encoding encoding) {
			MainStreamReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			byte[] buffer = MainStreamReader.ReadBytes(length);
			return encoding.GetString(buffer, 0, buffer.Length);
		}
		void CorrectParagraphBorderType(TextRunBorder paragraphBorder, int pcdIndex) {
			int offset = (PieceTable.GetEncoding(pcdIndex) == Encoding.Unicode) ? 2 : 1;
			MainStreamReader.BaseStream.Seek(paragraphBorder.Offset - offset, SeekOrigin.Begin);
			char markType = MainStreamReader.ReadChar();
			if ((markType == TextCodes.SectionMark) && (paragraphBorder.Offset != this.textRunBorders[this.textRunBorders.Count - 1].Offset)) {
				paragraphBorder.Reason |= TextRunStartReasons.SectionMark;
				paragraphBorder.Reason &= ~TextRunStartReasons.ParagraphMark;
			}
			if (markType == TextCodes.ColumnBreak) {
				paragraphBorder.Reason |= TextRunStartReasons.ColumnBreak;
				paragraphBorder.Reason &= ~TextRunStartReasons.ParagraphMark;
			}
			if (markType == TextCodes.TableUnitMark) {
				paragraphBorder.Reason |= TextRunStartReasons.TableUnitMark;
				paragraphBorder.Reason &= ~TextRunStartReasons.ParagraphMark;
			}
		}
		public void BeginEmbeddedContent() {
			Iterator.BeginEmbeddedContent();
		}
		public void EndEmbeddedContent() {
			Iterator.EndEmbeddedContent();
		}
	}
	#endregion
	#region UnsupportedObject
	public class UnsupportedObject : IDocObject {
		readonly DocPropertyContainer propertyContainer;
		public UnsupportedObject(int position, DocPropertyContainer propertyContainer) {
			this.propertyContainer = propertyContainer;
			this.Position = position;
			this.Length = 1;
		}
		public DocObjectType DocObjectType {
			get { return Doc.DocObjectType.UnsupportedObject; }
		}
		public DocPropertyContainer PropertyContainer {
			get { return propertyContainer; }
		}
		public int Position { get; set; }
		public int Length { get; set; }
	}
	#endregion 
	public class DocFontManager {
		string previousFont = DocCharacterFormattingInfo.DefaultFontName;
		bool shouldResetFont = false;
		public DocFontManager(BinaryReader reader, int offset) {
			Guard.ArgumentNotNull(reader, "reader");
			DocumentFonts = GetFonts(reader, offset);
		}
		protected internal List<DocFontFamilyName> DocumentFonts { get; private set; }
		List<DocFontFamilyName> GetFonts(BinaryReader reader, int fontTableOffset) {
			reader.BaseStream.Seek(fontTableOffset, SeekOrigin.Begin);
			short fontsCount = reader.ReadInt16();
			short extraDataSize = reader.ReadInt16();
			List<DocFontFamilyName> fonts = new List<DocFontFamilyName>(fontsCount);
			for (int i = 0; i < fontsCount; i++) {
				DocFontFamilyName docFontFamilyName = DocFontFamilyName.FromStream(reader);
				reader.BaseStream.Seek(extraDataSize, SeekOrigin.Current);
				fonts.Add(docFontFamilyName);
			}
			return fonts;
		}
		public void SetFontName(DocPropertyContainer propertyContainer) {
			if (TryResetFontName(propertyContainer))
				return;
			CharacterInfo characterInfo = propertyContainer.CharacterInfo;
			short index = characterInfo.Special ? characterInfo.SpecialCharactersFontFamilyNameIndex : propertyContainer.FontFamilyNameIndex;
			characterInfo.FormattingInfo.FontName = GetFontName(index);
			if (characterInfo.Special)
				shouldResetFont = true;
			else
				previousFont = characterInfo.FormattingInfo.FontName;
		}
		bool TryResetFontName(DocPropertyContainer propertyContainer) {
			CharacterInfo characterInfo = propertyContainer.CharacterInfo;
			if (characterInfo != null && characterInfo.FormattingOptions.UseFontName)
				return false;
			if (shouldResetFont) {
				propertyContainer.Update(ChangeActionTypes.Character);
				propertyContainer.CharacterInfo.FormattingInfo.FontName = previousFont;
				shouldResetFont = false;
			}
			return true;
		}
		public string GetFontName(short index) {
			if (index >= 0 && index < DocumentFonts.Count)
				return DocumentFonts[index].FontName;
			return DocCharacterFormattingInfo.DefaultFontName;
		}
	}
}
