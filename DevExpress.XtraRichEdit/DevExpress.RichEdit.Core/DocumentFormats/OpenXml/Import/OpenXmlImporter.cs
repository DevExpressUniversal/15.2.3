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
using System.Drawing;
using System.IO;
using System.Xml;
using System.Security;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Zip;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.XtraRichEdit.Import.Rtf;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region OpenXmlImporter
	public class OpenXmlImporter : WordProcessingMLBaseImporter, IDisposable, IOpenXmlImporter {
		#region Fields
		PackageFileCollection packageFiles;
		Stack<OpenXmlRelationCollection> documentRelationsStack;
		string documentRootFolder;
		Stack<PackageFileStreams> packageFileStreamsStack;
		Stack<Dictionary<string, OfficeNativeImage>> packageImagesStack;
		List<AltChunkInfo> altChunkInfos;
		Dictionary<string, FootNote> footNotes;
		Dictionary<string, EndNote> endNotes;
		#endregion
		public OpenXmlImporter(DocumentModel documentModel, OpenXmlDocumentImporterOptions options)
			: base(documentModel, options) {
		}
		#region Properties
		public OpenXmlDocumentImporterOptions Options { get { return (OpenXmlDocumentImporterOptions)InnerOptions; } }
		public PackageFileCollection PackageFiles { get { return packageFiles; } }
		public Stack<OpenXmlRelationCollection> DocumentRelationsStack { get { return documentRelationsStack; } }
		public override OpenXmlRelationCollection DocumentRelations { get { return documentRelationsStack.Peek(); } }
		public override string DocumentRootFolder { get { return documentRootFolder; } set { documentRootFolder = value; } }
		protected internal PackageFileStreams PackageFileStreams { get { return packageFileStreamsStack.Peek(); } }
		protected internal Dictionary<string, OfficeNativeImage> PackageImages { get { return packageImagesStack.Peek(); } }
		protected internal Stack<PackageFileStreams> PackageFileStreamsStack { get { return packageFileStreamsStack; } }
		protected internal Stack<Dictionary<string, OfficeNativeImage>> PackageImagesStack { get { return packageImagesStack; } }
		protected internal Dictionary<string, FootNote> FootNotes { get { return footNotes; } }
		protected internal Dictionary<string, EndNote> EndNotes { get { return endNotes; } }
		public override string WordProcessingNamespaceConst { get { return OpenXmlExporter.WordProcessingNamespaceConst; } }
		public override string OfficeNamespace { get { return OpenXmlExporter.OfficeNamespaceConst; } }
		public override string W14NamespaceConst { get { return WordProcessingMLBaseExporter.W14NamespaceConst; } }
		public override string W15NamespaceConst { get { return WordProcessingMLBaseExporter.W15NamespaceConst; } }
		public override string DrawingMLNamespaceConst { get { return WordProcessingMLBaseExporter.DrawingMLNamespaceConst; } }
		public override string RelationsNamespace { get { return String.Empty; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				DisposePackageFiles();
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public void Import(Stream stream) {
			OpenPackage(stream);
			OpenXmlRelationCollection rootRelations = ImportRelations("_rels/.rels");
			string fileName = LookupRelationTargetByType(rootRelations, OpenXmlExporter.OfficeDocumentType, String.Empty, "document.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader == null) {
				CheckIsEncryptedFile(stream);
				Exceptions.ThrowArgumentException("stream", stream);
				return;
			}
			if (!ReadToRootElement(reader, "document")) {
				Exceptions.ThrowArgumentException("stream", stream);
				return;
			}
			this.documentRootFolder = Path.GetDirectoryName(fileName);
			this.documentRelationsStack = new Stack<OpenXmlRelationCollection>();
			this.packageFileStreamsStack = new Stack<PackageFileStreams>();
			this.packageImagesStack = new Stack<Dictionary<string, OfficeNativeImage>>();
			this.documentRelationsStack.Push(ImportRelations(documentRootFolder + "/_rels/" + Path.GetFileName(fileName) + ".rels"));
			this.packageFileStreamsStack.Push(new PackageFileStreams());
			this.packageImagesStack.Push(new Dictionary<string, OfficeNativeImage>());
			ImportMainDocument(reader, stream);
		}
		void CheckIsEncryptedFile(Stream stream) {
			if (!ValidateStream(stream)) return;
			using (PackageFileReader reader = new PackageFileReader(stream))
				if (reader.GetCachedPackageFileReader("EncryptionInfo") != null)
					RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_EncryptedFile);
		}
		bool ValidateStream(Stream stream) {
			if (stream == null)
				return false;
			byte[] magicNumberBuffer = new byte[8];
			stream.Seek(0, SeekOrigin.Begin);
			stream.Read(magicNumberBuffer, 0, 8);
			return AbstractHeader.MAGIC_NUMBER == BitConverter.ToUInt64(magicNumberBuffer, 0);
		}
		protected internal virtual void DisposePackageFiles() {
			if (PackageFiles == null)
				return;
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++)
				PackageFiles[i].Stream.Dispose();
			PackageFiles.Clear();
			this.packageFiles = null;
		}
		protected internal virtual void OpenPackage(Stream stream) {
			this.packageFiles = GetPackageFiles(stream);
		}
		protected internal virtual PackageFileCollection GetPackageFiles(Stream stream) {
			InternalZipFileCollection files = InternalZipArchive.Open(stream);
			int count = files.Count;
			PackageFileCollection result = new PackageFileCollection();
			for (int i = 0; i < count; i++) {
				InternalZipFile file = files[i];
				result.Add(new PackageFile(file.FileName.Replace('\\', '/'), file.FileDataStream, (int)file.UncompressedSize));
			}
			return result;
		}
		protected internal virtual Stream GetPackageFileStream(string fileName) {
			PackageFile file = GetPackageFile(fileName);
			if (file == null)
				return null;
			return file.Stream;
		}
		protected internal virtual PackageFile GetPackageFile(string fileName) {
			if (PackageFiles == null)
				return null;
			fileName = fileName.Replace('\\', '/');
			if (fileName.StartsWith("/"))
				fileName = fileName.Substring(1);
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++) {
				PackageFile file = PackageFiles[i];
				if (String.Compare(file.FileName, fileName, StringComparison.CurrentCultureIgnoreCase) == 0)
					return file;
			}
			return null;
		}
		protected internal virtual XmlReader GetPackageFileXmlReader(string fileName) {
			Stream stream = GetPackageFileStream(fileName);
			if (stream == null)
				return null;
			return CreateXmlReader(stream);
		}
		protected internal virtual OpenXmlRelationCollection ImportRelations(string fileName) {
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				return ImportRelationsCore(reader);
			else
				return new OpenXmlRelationCollection();
		}
		protected internal virtual OpenXmlRelationCollection ImportRelationsCore(XmlReader reader) {
			OpenXmlRelationCollection result = new OpenXmlRelationCollection();
			if (!ReadToRootElement(reader, "Relationships", OpenXmlExporter.PackageRelsNamespace))
				return result;
			ImportContent(reader, new RelationshipsDestination(this, result));
			return result;
		}
		protected internal virtual string LookupRelationTargetByType(OpenXmlRelationCollection relations, string type, string rootFolder, string defaultFileName) {
			OpenXmlRelation relation = relations.LookupRelationByType(type);
			return CalculateRelationTargetCore(relation, rootFolder, defaultFileName);
		}
		protected internal virtual string LookupRelationTargetById(OpenXmlRelationCollection relations, string id, string rootFolder, string defaultFileName) {
			OpenXmlRelation relation = relations.LookupRelationById(id);
			return CalculateRelationTargetCore(relation, rootFolder, defaultFileName);
		}
		protected internal virtual OfficeImage LookupImageByRelationId(string relationId, string rootFolder) {
			OfficeNativeImage rootImage;
			if (PackageImages.TryGetValue(relationId, out rootImage))
				return new OfficeReferenceImage(DocumentModel, rootImage);
			Stream stream = LookupPackageFileStreamByRelationId(relationId, rootFolder, false);
			if (stream == null)
				return null;
			else {
				try {
					OfficeReferenceImage image = DocumentModel.CreateImage(stream);
					if (image != null)
						PackageImages.Add(relationId, image.NativeRootImage);
					return image;
				}
				catch {
					return null;
				}
			}
		}
		internal OfficeImage LookupMetafileByRelationId(string relationId, string rootFolder, int pictureWidth, int pictureHeight) {
			OfficeNativeImage rootImage;
			if (PackageImages.TryGetValue(relationId, out rootImage))
				return new OfficeReferenceImage(DocumentModel, rootImage);
			Stream stream = LookupPackageFileStreamByRelationId(relationId, rootFolder, false);
			if (stream == null)
				return null;
#if !SL
			RtfImageInfo info = new RtfImageInfoWin(DocumentModel);
#else
			RtfImageInfo info = new RtfImageInfoSL(DocumentModel);
#endif
			try {
				byte[] buffer = ((MemoryStream)stream).ToArray();
				using (MemoryStream pictureStream = new MemoryStream(buffer, 0, buffer.Length, true, true)) {
					info.LoadMetafileFromStream(pictureStream, Office.PInvoke.Win32.MapMode.Anisotropic, pictureWidth, pictureHeight);
					if (info.RtfImage != null) {
						PackageImages.Add(relationId, info.RtfImage.NativeRootImage);
						return info.RtfImage;
					}
				}
			}
			catch { }
			return null;
		}
		protected internal virtual Stream LookupPackageFileStreamByRelationId(string relationId, string rootFolder, bool cacheStream) {
			Stream stream;
			if (PackageFileStreams.TryGetValue(relationId, out stream)) {
				stream.Seek(0, SeekOrigin.Begin);
				return stream;
			}
			stream = CreatePackageFileStreamByRelationId(relationId, rootFolder);
			if (stream == null)
				return null;
			stream.Seek(0, SeekOrigin.Begin);
			PackageFileStreams.Add(relationId, stream);
			return stream;
		}
		protected internal virtual Stream CreatePackageFileStreamByRelationId(string relationId, string rootFolder) {
			string fileName = LookupRelationTargetById(DocumentRelations, relationId, rootFolder, String.Empty);
			PackageFile file = GetPackageFile(fileName);
			if (file == null)
				return null;
			return file.SeekableStream;
		}
		protected internal virtual string CalculateRelationTargetCore(OpenXmlRelation relation, string rootFolder, string defaultFileName) {
			if (relation == null) {
				if (String.IsNullOrEmpty(rootFolder))
					return defaultFileName;
				else
					return rootFolder + "/" + defaultFileName;
			}
			if (relation.Target.StartsWith("/"))
				return relation.Target;
			else {
				if (String.IsNullOrEmpty(rootFolder))
					return relation.Target;
				else
					return rootFolder + "/" + relation.Target;
			}
		}
		#region Settings
		protected internal virtual void ImportSettings() {
			string fileName = LookupRelationTargetByType(DocumentRelations, OpenXmlExporter.OfficeDocumentSettings, DocumentRootFolder, "settings.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportSettingsCore(reader);
		}
		protected internal virtual void ImportSettingsCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "settings"))
				return;
			ImportContent(reader, new DocumentSettingsDestination(this));
		}
		#endregion
		protected internal virtual void ImportWebSettings() {
			string fileName = LookupRelationTargetByType(DocumentRelations, OpenXmlExporter.OfficeWebSettingsType, DocumentRootFolder, "webSettings.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportWebSettingsCore(reader);
		}
		protected internal virtual void ImportWebSettingsCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "webSettings"))
				return;
			ImportContent(reader, new DocumentWebSettingsDestination(this));
		}
		#region Styles
		protected internal virtual void ImportStyles() {
			string fileName = LookupRelationTargetByType(DocumentRelations, OpenXmlExporter.OfficeStylesType, DocumentRootFolder, "styles.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportStylesCore(reader);
		}
		protected internal virtual void ImportStylesCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "styles"))
				return;
			ImportContent(reader, new StylesDestination(this));
			CreateStylesHierarchy();
			LinkStyles();
		}
		#endregion
		#region Numbering
		protected internal virtual void ImportNumbering() {
			string fileName = LookupRelationTargetByType(DocumentRelations, OpenXmlExporter.OfficeNumberingType, DocumentRootFolder, "numbering.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportNumberingCore(reader);
		}
		protected internal virtual void ImportNumberingCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "numbering"))
				return;
			ImportContent(reader, new NumberingsDestination(this));
			CreateNumberingLists();
		}
		#endregion
		#region FootNotes & EndNotes
		protected internal virtual void ImportFootNotes() {
			if (!DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return;
			string fileName = LookupRelationTargetByType(DocumentRelations, OpenXmlExporter.OfficeFootNoteType, DocumentRootFolder, "footnotes.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportFootNotesCore(reader);
		}
		protected internal virtual void ImportEndNotes() {
			if (!DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return;
			string fileName = LookupRelationTargetByType(DocumentRelations, OpenXmlExporter.OfficeEndNoteType, DocumentRootFolder, "endnotes.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportEndNotesCore(reader);
		}
		protected internal virtual void ImportFootNotesCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "footnotes"))
				return;
			ImportContent(reader, new FootNotesDestination(this));
		}
		protected internal virtual void ImportEndNotesCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "endnotes"))
				return;
			ImportContent(reader, new EndNotesDestination(this));
		}
		#endregion
		#region Themes
		protected internal virtual void ImportThemes() {
			string fileName = LookupRelationTargetByType(DocumentRelations, OpenXmlExporter.OfficeThemesType, DocumentRootFolder, "theme/theme1.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null) {
				BeforeImportThemes();
				ImportThemesCore(reader);
				AfterImportThemes();
			}
		}
		private void AfterImportThemes() {
			PackageFileStreamsStack.Pop();
			DocumentRelationsStack.Pop();
		}
		private void ImportThemesCore(XmlReader reader){
			if (!ReadToRootAElement(reader, "theme"))
				return;
			ImportThemeCore(reader);
		}
		protected override void PrepareOfficeTheme() {
			OfficeThemeBase<DocumentFormat> theme = new OfficeThemeBase<DocumentFormat>();
			DocumentModel.OfficeTheme = theme;
			ActualDocumentModel = theme;
		}
		private void BeforeImportThemes(){
			DocumentRelationsStack.Push(ImportRelations(DocumentRootFolder + "/_rels/" + "theme/theme1.xml" + ".rels"));
			PackageFileStreamsStack.Push(new PackageFileStreams());
		}
		#endregion
		#region CommentsExtended
		protected internal virtual void ImportCommentsExtended() {
			string fileName = LookupRelationTargetByType(DocumentRelations, OpenXmlExporter.OfficeCommentsExtendedType, DocumentRootFolder, "commentsExtended.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null) {
				BeforeImportCommentsExtended();
				ImportCommentsExtendedCore(reader);
				AfterImportCommentsExtended();
			}
		}
		void BeforeImportCommentsExtended() {
			DocumentRelationsStack.Push(ImportRelations(DocumentRootFolder + "/_rels/" + "commentsExtended.xml" + ".rels"));
			PackageFileStreamsStack.Push(new PackageFileStreams());
		}
		void AfterImportCommentsExtended() {
			PackageFileStreamsStack.Pop();
			DocumentRelationsStack.Pop();
		}
		protected internal virtual void ImportCommentsExtendedCore(XmlReader reader) {
			if (!ReadToRootW15Element(reader, "commentsEx"))
				return;
			ImportContent(reader, new CommentsExDestination(this));
		}
		#endregion
		#region Comments
		protected internal virtual void ImportComments() {
			string fileName = LookupRelationTargetByType(DocumentRelations, OpenXmlExporter.OfficeCommentType, DocumentRootFolder, "comments.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null) {
				BeforeImportComment();
				ImportCommentsCore(reader);
				AfterImportComment();
			}
		}
		void BeforeImportComment() {
			DocumentRelationsStack.Push(ImportRelations(DocumentRootFolder + "/_rels/" + "comments.xml" + ".rels"));
			PackageFileStreamsStack.Push(new PackageFileStreams());
			PackageImagesStack.Push(new Dictionary<string, OfficeNativeImage>());
		}
		void AfterImportComment() {
			PackageFileStreamsStack.Pop();
			DocumentRelationsStack.Pop();
			PackageImagesStack.Pop();
		}
		protected internal virtual void ImportCommentsCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "comments"))
				return;
			ImportContent(reader, new CommentsDestination(this));
		}
		#endregion
		protected override void BeforeImportMainDocument() {
			this.altChunkInfos = new List<AltChunkInfo>();
			this.footNotes = new Dictionary<string, FootNote>();
			this.endNotes = new Dictionary<string, EndNote>();
			ImportSettings();
			ImportThemes();
			ImportWebSettings();
			ImportStyles();
			ImportNumbering();
			ImportFootNotes();
			ImportEndNotes();
			ImportComments();
			ImportCommentsExtended();
			LinkParagraphStylesWithNumberingLists();
			LinkNumberingListStyles();
			base.BeforeImportMainDocument();
		}
		protected override Destination CreateMainDocumentDestination() {
			return new DocumentDestination(this);
		}
		protected override void AfterImportMainDocument() {
			base.AfterImportMainDocument();
			InsertAltChunks();
			DocumentModel.NormalizeZOrder();			
			PieceTable.CheckIntegrity();
		}
		protected virtual void InsertAltChunks() {
			int count = altChunkInfos.Count;
			for (int i = count - 1; i >= 0; i--)
				InsertAltChunk(altChunkInfos[i]);
		}
		protected virtual void InsertAltChunk(AltChunkInfo chunkInfo) {
			string relId = chunkInfo.RelationId;
			string relationTarget = LookupRelationTargetById(DocumentRelations, relId, DocumentRootFolder, String.Empty);
			if (String.IsNullOrEmpty(relationTarget))
				return;
			Stream stream = LookupPackageFileStreamByRelationId(relId, DocumentRootFolder, true);
			if (stream == null)
				return;
			PieceTable targetPieceTable = chunkInfo.PieceTable;
			DocumentModel altChunkDocumentModel = null;
			DocumentFormat documentFormat = DocumentModel.AutodetectDocumentFormat(relationTarget, false);
			if (documentFormat == DocumentFormat.Undefined) {
				altChunkDocumentModel = targetPieceTable.DocumentModel.CreateNew(false, false);
				bool invalidFormat = false;
				altChunkDocumentModel.InvalidFormatException += delegate(object sender, RichEditInvalidFormatExceptionEventArgs e) {
					invalidFormat = true;
				};
				long initialPosition = stream.Position;
				try {
					documentFormat = DocumentFormat.WordML;
					altChunkDocumentModel.LoadDocument(stream, documentFormat, relationTarget);
					altChunkDocumentModel.MainPieceTable.InsertParagraph(altChunkDocumentModel.MainPieceTable.DocumentEndLogPosition);					
				}
				catch (Exception) {
					invalidFormat = true;
				}
				if (invalidFormat) {
					altChunkDocumentModel.Dispose();
					altChunkDocumentModel = null;
					documentFormat = DocumentFormat.PlainText;
					stream.Position = initialPosition;
				}
			}			
			if (altChunkDocumentModel == null) {				
				altChunkDocumentModel = targetPieceTable.DocumentModel.CreateNew(false, false);
				altChunkDocumentModel.LoadDocument(stream, documentFormat, relationTarget);
				altChunkDocumentModel.MainPieceTable.InsertParagraph(altChunkDocumentModel.MainPieceTable.DocumentEndLogPosition);
			}
			if (chunkInfo.LogPosition == targetPieceTable.DocumentEndLogPosition + 1)
				targetPieceTable.InsertParagraph(chunkInfo.LogPosition - 1);
			if (chunkInfo.LogPosition > targetPieceTable.DocumentEndLogPosition + 1)
				return;
			targetPieceTable.InsertDocumentModelContent(altChunkDocumentModel, chunkInfo.LogPosition, false, false, true);
		}
		public override void ThrowInvalidFile() {
			throw new Exception("Invalid OpenXml file");
		}
		protected internal override string GetWordProcessingMLValue(WordProcessingMLValue value) {
			return value.OpenXmlValue;
		}
		protected internal override SectionTextDirectionDestination CreateOpenXmlSectionTextDirectionDestination() {
			return new SectionTextDirectionDestination(this);
		}
		protected internal override SectionTextDirectionDestination CreateWordMLSectionTextDirectionDestination() {
			return null;
		}
		protected internal override ParagraphPropertiesBaseDestination CreateStyleParagraphPropertiesDestination(StyleDestinationBase styleDestination, ParagraphFormattingBase paragraphFormatting, TabFormattingInfo tabs) {
			return new StyleParagraphPropertiesDestination(this, styleDestination, paragraphFormatting, tabs);
		}
		#region Conversion and Parsing utilities
		public override bool ConvertToBool(string value) {
			if (value == "1" || value == "on" || value == "true")
				return true;
			if (value == "0" || value == "off" || value == "false")
				return false;
			ThrowInvalidFile();
			return false;
		}
		#endregion
		protected internal virtual void AddAltChunkInfo(AltChunkInfo altChunkInfo) {
			this.altChunkInfos.Add(altChunkInfo);			
		}
		protected internal override int RegisterFootNote(FootNote note, string id) {
			if (!String.IsNullOrEmpty(id)) {
				this.footNotes[id] = note;
				int index = DocumentModel.FootNotes.Count;
				DocumentModel.FootNotes.Add(note);
				return index;
			}
			else
				return -1;
		}
		protected internal override int RegisterEndNote(EndNote note, string id) {
			if (!String.IsNullOrEmpty(id)) {
				this.endNotes[id] = note;
				int index = DocumentModel.EndNotes.Count;
				DocumentModel.EndNotes.Add(note);
				return index;
			}
			else
				return -1;
		}
		protected internal virtual void CheckVersion() {
		}
	}
	#endregion
	#region OpenXmlInputPosition
	public class OpenXmlInputPosition : InputPosition {
		#region Fields
		readonly CharacterFormattingBase paragraphMarkCharacterFormatting;
		readonly ParagraphFormattingBase paragraphFormatting;
		readonly TabFormattingInfo paragraphTabs;
		int paragraphStyleIndex;
		int paragraphMarkCharacterStyleIndex;
		#endregion
		public OpenXmlInputPosition(PieceTable pieceTable)
			: base(pieceTable) {
			this.paragraphMarkCharacterFormatting = new CharacterFormattingBase(pieceTable, pieceTable.DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex);
			this.paragraphFormatting = new ParagraphFormattingBase(pieceTable, pieceTable.DocumentModel, ParagraphFormattingInfoCache.DefaultItemIndex, ParagraphFormattingOptionsCache.EmptyParagraphFormattingOptionIndex);
			this.paragraphTabs = new TabFormattingInfo();
		}
		#region Properties
		public CharacterFormattingBase ParagraphMarkCharacterFormatting { get { return paragraphMarkCharacterFormatting; } }
		public ParagraphFormattingBase ParagraphFormatting { get { return paragraphFormatting; } }
		public TabFormattingInfo ParagraphTabs { get { return paragraphTabs; } }
		public int ParagraphMarkCharacterStyleIndex { get { return paragraphMarkCharacterStyleIndex; } set { paragraphMarkCharacterStyleIndex = value; } }
		public int ParagraphStyleIndex { get { return paragraphStyleIndex; } set { paragraphStyleIndex = value; } }
		#endregion
	}
	#endregion
	#region OpenXmlImportHelper
	public class OpenXmlImportHelper {
		const int maxTintValue = 255;
		public ColorModelInfo SaveColorModelInfo(WordProcessingMLBaseImporter importer, XmlReader reader, string attribute) {
			ColorModelInfo result = ColorModelInfo.Create(DXColor.Empty);
			result.Rgb = ConvertAttributeToColor(importer, reader, attribute);
			result.Theme = importer.GetWpEnumValue<ThemeColorIndex>(reader, "themeColor", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None);
			string value = reader.GetAttribute("themeShade");
			if (!String.IsNullOrEmpty(value)) 
				result.Tint = ModifyShadeToTint(importer, value);
			value = reader.GetAttribute("themeTint");
			if (!String.IsNullOrEmpty(value))
				result.Tint = maxTintValue / importer.ConvertToInt(value);
			return result;
		}
		double ModifyShadeToTint(WordProcessingMLBaseImporter importer, string value){
			return -(maxTintValue/ importer.ConvertToInt(value));
		}
		Color ConvertAttributeToColor(WordProcessingMLBaseImporter importer, XmlReader reader, string attribute) {
			Color result;
			string value = importer.ReadAttribute(reader, attribute); 
			if (value == "auto")
				result = DXColor.Empty;
			else
				result = importer.GetWpSTColorValue(reader, attribute);
			return result;
		}
		public ColorModelInfo SaveFillInfo(WordProcessingMLBaseImporter importer, XmlReader reader) {
			ColorModelInfo result = ColorModelInfo.Create(DXColor.Empty);
			result.Rgb = importer.GetWpSTColorValue(reader, "fill");
			result.Theme = importer.GetWpEnumValue<ThemeColorIndex>(reader, "themeFill", OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None);
			string value = reader.GetAttribute("themeFillShade");
			if (!String.IsNullOrEmpty(value))
				result.Tint = ModifyShadeToTint(importer, value);
			value = reader.GetAttribute("themeFillTint");
			if (!String.IsNullOrEmpty(value))
				result.Tint = maxTintValue / importer.ConvertToInt(value);
			return result;
		}
	}
	#endregion
}
