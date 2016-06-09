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
using System.IO;
using System.Text;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office.PInvoke;
using DevExpress.Office.NumberConverters;
using DevExpress.Utils.Zip;
using DevExpress.Office.Model;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Rtf {
#if !SL
	#region RTFDecoderFallbackBuffer
	public class RTFDecoderFallbackBuffer : DecoderFallbackBuffer {
		char[] chars;
		int count;
		int position;
		public override bool Fallback(byte[] bytesUnknown, int index) {
			count = bytesUnknown.Length;
			chars = new char[count];
			for (int i = 0; i < count; i++)
				chars[i] = (char)bytesUnknown[i];
			position = 0;
			return true;
		}
		public override char GetNextChar() {
			if (position >= count)
				return '\0';
			else {
				char result = chars[position];
				position++;
				return result;
			}
		}
		public override bool MovePrevious() {
			position--;
			return position >= 0;
		}
		public override int Remaining { get { return count - position; } }
		public override void Reset() {
			position = 0;
		}
	}
	#endregion
	#region RTFDecoderFallback
	public class RTFDecoderFallback : DecoderFallback {
		public override DecoderFallbackBuffer CreateFallbackBuffer() {
			return new RTFDecoderFallbackBuffer();
		}
		public override int MaxCharCount { get { return 2; } }
	}
	#endregion
#endif
	#region CharacterDecoder (abstract class)
	public abstract class CharacterDecoder {
		readonly Encoding encoding;
		protected CharacterDecoder(int codePage) {
			this.encoding = CreateCodePageEncoding(codePage);
			if (!DXEncoding.IsSingleByteEncoding(Encoding)) {
				encoding = (Encoding)encoding.Clone();
#if !SL && !DXPORTABLE
				Encoding.DecoderFallback = new RTFDecoderFallback();
#endif
			}
		}
		protected Encoding Encoding { get { return encoding; } }
		internal static Encoding CreateCodePageEncoding(int codePage) {
			try {
				return DXEncoding.GetEncodingFromCodePage((int)(((uint)codePage) & 0xFFFF));
			}
			catch {
				return EmptyEncoding.Instance;
			}
		}
	}
	#endregion
	#region CodePageCharacterDecoder
	public class CodePageCharacterDecoder : CharacterDecoder {
		readonly List<byte> bytes;
		public CodePageCharacterDecoder(int codePage)
			: base(codePage) {
			this.bytes = new List<byte>(4096);
		}
		public virtual void ProcessChar(RtfImporter importer, char ch) {
			if (bytes.Count == bytes.Capacity)
				Flush(importer);
			bytes.Add((byte)ch);
		}
		void FlushByChar(RtfImporter importer, char[] chars) {
			int count = chars.Length;
			DestinationBase destination = importer.Destination;
			for (int i = 0; i < count; i++) {
				destination.ProcessChar(chars[i]);
			}
		}
		void FlushByString(RtfImporter importer, char[] chars) {
			importer.Destination.ProcessText(new String(chars));
		}
		public virtual void Flush(RtfImporter importer) {
			if (bytes.Count > 0) {
				char[] chars = Encoding.GetChars(bytes.ToArray());
				int count = chars.Length;
				if (!importer.Destination.CanAppendText || count <= 1) {
					FlushByChar(importer, chars);
				}
				else {
					FlushByString(importer, chars);
				}
				bytes.Clear();
			}
		}
	}
	#endregion
	#region EmptyCharacterDecoder
	public class EmptyCharacterDecoder : CodePageCharacterDecoder {
		public EmptyCharacterDecoder()
			: base(DXEncoding.GetEncodingCodePage(RtfImporter.DefaultEncoding)) {
		}
		public override void ProcessChar(RtfImporter importer, char ch) {
			importer.Destination.ProcessChar(ch);
		}
		public override void Flush(RtfImporter importer) {
		}
	}
	#endregion
	#region SkipCharacterDecoder
	public class SkipCharacterDecoder : CodePageCharacterDecoder {
		public SkipCharacterDecoder()
			: base(DXEncoding.GetEncodingCodePage(RtfImporter.DefaultEncoding)) {
		}
		public override void ProcessChar(RtfImporter importer, char ch) {
		}
		public override void Flush(RtfImporter importer) {
		}
	}
	#endregion
	#region RtfCharacterFormattingInfo
	public class RtfFormattingInfo {
		#region Fields
		int unicodeCharacterByteCount = 1;
		CodePageCharacterDecoder decoder;
		int codePage;
		int parentStyleIndex = -1;
		#endregion
		public RtfFormattingInfo() {
			this.CodePage = DXEncoding.GetEncodingCodePage(RtfImporter.DefaultEncoding);
		}
		#region Properties
		public int UnicodeCharacterByteCount {
			get { return unicodeCharacterByteCount; }
			set {
				if (value < 0)
					RtfImporter.ThrowInvalidRtfFile();
				unicodeCharacterByteCount = value;
			}
		}
		public bool Deleted { get; set; }
		protected internal CodePageCharacterDecoder Decoder { get { return decoder; } }
		protected internal void SetDecoder(CodePageCharacterDecoder decoder) {
			Guard.ArgumentNotNull(decoder, "decoder");
			this.decoder = decoder;
		}
		public int CodePage {
			get { return codePage; }
			set {
				if (codePage == value)
					return;
				codePage = value;
				decoder = ChooseDecoder();
			}
		}
		protected internal virtual CodePageCharacterDecoder ChooseDecoder() {
			return new CodePageCharacterDecoder(CodePage);
		}
		public int ParentStyleIndex { get { return parentStyleIndex; } set { parentStyleIndex = value; } }
		#endregion
		public RtfFormattingInfo Clone() {
			RtfFormattingInfo clone = CreateEmptyClone();
			clone.CopyFrom(this);
			return clone;
		}
		protected internal virtual RtfFormattingInfo CreateEmptyClone() {
			return new RtfFormattingInfo();
		}
		public void CopyFrom(RtfFormattingInfo info) {
			unicodeCharacterByteCount = info.unicodeCharacterByteCount;
			CodePage = info.CodePage;
			Deleted = info.Deleted;
		}
	}
	#endregion
	#region RtfParagraphFormattingInfo
	public class RtfParagraphFormattingInfo : ParagraphFormattingInfo {
		#region Fields
		TabFormattingInfo tabs = new TabFormattingInfo();
		TabAlignmentType tabAlignment = TabInfo.DefaultAlignment;
		TabLeaderType tabLeader = TabInfo.DefaultLeader;
		NumberingListIndex numberingListIndex = NumberingListIndex.ListIndexNotSetted;
		int listLevelIndex = 0;
		int rtfLineSpacingType;
		int rtfLineSpacingMultiplier = 1;
		bool inTableParagraph;
		int nestingLevel = 0;
		int styleIndex = 0;
		int parentStyleIndex = 0;
		int styleLink = -1;
		int nextStyle = -1;
		BorderInfo processedBorder;
		int rtfTableStyleIndexForRowOrCell;
		#endregion
		public RtfParagraphFormattingInfo() {
			WidowOrphanControl = true;
		}
		#region Properties
		public TabAlignmentType TabAlignment { get { return tabAlignment; } set { tabAlignment = value; } }
		public TabLeaderType TabLeader { get { return tabLeader; } set { tabLeader = value; } }
		public int RtfLineSpacingType { get { return rtfLineSpacingType; } set { rtfLineSpacingType = value; } }
		public int RtfLineSpacingMultiplier { get { return rtfLineSpacingMultiplier; } set { rtfLineSpacingMultiplier = value; } }
		public TabFormattingInfo Tabs { get { return tabs; } }
		public bool InTableParagraph { get { return inTableParagraph; } set { inTableParagraph = value; } }
		public int NestingLevel { get { return nestingLevel; } set { nestingLevel = value; } }
		public NumberingListIndex NumberingListIndex { get { return numberingListIndex; } set { numberingListIndex = value; } }
		public int ListLevelIndex { get { return listLevelIndex; } set { listLevelIndex = value; } }
		public int StyleIndex { get { return styleIndex; } set { styleIndex = value; } }
		public int ParentStyleIndex { get { return parentStyleIndex; } set { parentStyleIndex = value; } }
		public int StyleLink { get { return styleLink; } set { styleLink = value; } }
		public int NextStyle { get { return nextStyle; } set { nextStyle = value; } }
		public BorderInfo ProcessedBorder { get { return processedBorder; } set { processedBorder = value; } }
		public int RtfTableStyleIndexForRowOrCell { get { return rtfTableStyleIndexForRowOrCell; } set { rtfTableStyleIndexForRowOrCell = value; } }
		#endregion
		public new RtfParagraphFormattingInfo Clone() {
			RtfParagraphFormattingInfo clone = new RtfParagraphFormattingInfo();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(RtfParagraphFormattingInfo info) {
			base.CopyFrom(info);
			this.tabs = info.Tabs.Clone();
			this.tabAlignment = info.TabAlignment;
			this.tabLeader = info.TabLeader;
			this.rtfLineSpacingType = info.rtfLineSpacingType;
			this.rtfLineSpacingMultiplier = info.rtfLineSpacingMultiplier;
			this.inTableParagraph = info.inTableParagraph;
			this.nestingLevel = info.NestingLevel;
			this.numberingListIndex = info.NumberingListIndex;
			this.listLevelIndex = info.ListLevelIndex;
			this.styleIndex = info.StyleIndex;
			this.parentStyleIndex = info.ParentStyleIndex;
			this.styleLink = info.StyleLink;
			this.nextStyle = info.NextStyle;
			this.rtfTableStyleIndexForRowOrCell = info.rtfTableStyleIndexForRowOrCell;
		}
		public ParagraphLineSpacing CalcLineSpacingType() {
			if (rtfLineSpacingMultiplier < 0)
				return ParagraphLineSpacing.Single;
			else if (rtfLineSpacingMultiplier == 0) {
				if (rtfLineSpacingType == 0)
					return ParagraphLineSpacing.Single;
				else if (rtfLineSpacingType > 0)
					return ParagraphLineSpacing.AtLeast;
				else
					return ParagraphLineSpacing.Exactly;
			}
			else {
				if (rtfLineSpacingType == 240)
					return ParagraphLineSpacing.Single;
				else if (rtfLineSpacingType == 360)
					return ParagraphLineSpacing.Sesquialteral;
				else if (rtfLineSpacingType == 480)
					return ParagraphLineSpacing.Double;
				else if (rtfLineSpacingType <= 0)
					return ParagraphLineSpacing.Single;
				else
					return ParagraphLineSpacing.Multiple;
			}
		}
		public float CalcLineSpacing(DocumentModelUnitConverter unitConverter) {
			if (rtfLineSpacingMultiplier < 0)
				return 0;
			else if (rtfLineSpacingMultiplier == 0)
				return rtfLineSpacingType != 0 ? Math.Max(unitConverter.TwipsToModelUnits(Math.Abs(rtfLineSpacingType)), 1) : 0;
			else {
				if (rtfLineSpacingType < 0)
					return 0.0f;
				else
					return rtfLineSpacingType / 240.0f;
			}
		}
	}
	#endregion
	#region RtfSectionFormattingInfo
	public class RtfSectionFormattingInfo {
		#region Fields
		readonly PageInfo page;
		readonly MarginsInfo margins;
		readonly PageNumberingInfo pageNumbering;
		readonly GeneralSectionInfo generalSectionInfo;
		readonly LineNumberingInfo lineNumbering;
		readonly SectionFootNote footNote;
		readonly SectionFootNote endNote;
		bool restartPageNumbering;
		readonly ColumnsInfo columns;
		int currentColumnIndex;
		#endregion
		public RtfSectionFormattingInfo(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.page = new PageInfo();
			this.margins = new MarginsInfo();
			this.pageNumbering = new PageNumberingInfo();
			this.generalSectionInfo = new GeneralSectionInfo();
			this.lineNumbering = new LineNumberingInfo();
			this.columns = new ColumnsInfo();
			this.footNote = new SectionFootNote(documentModel);
			this.endNote = new SectionFootNote(documentModel);
		}
		#region Properties
		public PageInfo Page { get { return page; } }
		public MarginsInfo Margins { get { return margins; } }
		public PageNumberingInfo PageNumbering { get { return pageNumbering; } }
		public GeneralSectionInfo GeneralSectionInfo { get { return generalSectionInfo; } }
		public LineNumberingInfo LineNumbering { get { return lineNumbering; } }
		public SectionFootNote FootNote { get { return footNote; } }
		public SectionFootNote EndNote { get { return endNote; } }
		public bool RestartPageNumbering { get { return restartPageNumbering; } set { restartPageNumbering = value; } }
		public ColumnsInfo Columns { get { return columns; } }
		public int CurrentColumnIndex { get { return currentColumnIndex; } set { currentColumnIndex = value; } }
		#endregion
		public void InitializeDefault(DocumentModel documentModel) {
			DocumentCache cache = documentModel.Cache;
			margins.CopyFrom(cache.MarginsInfoCache[0]);
			pageNumbering.CopyFrom(cache.PageNumberingInfoCache[0]);
			generalSectionInfo.CopyFrom(cache.GeneralSectionInfoCache[0]);
			lineNumbering.CopyFrom(cache.LineNumberingInfoCache[0]);
			columns.CopyFrom(cache.ColumnsInfoCache[0]);
			footNote.CopyFrom(cache.FootNoteInfoCache[FootNoteInfoCache.DefaultFootNoteItemIndex]);
			endNote.CopyFrom(cache.FootNoteInfoCache[FootNoteInfoCache.DefaultEndNoteItemIndex]);
		}
		public void SetCurrentColumnWidth(int value) {
			EnsureCurrentColumnExists();
			columns.Columns[currentColumnIndex].Width = value;
		}
		public void SetCurrentColumnSpace(int value) {
			EnsureCurrentColumnExists();
			columns.Columns[currentColumnIndex].Space = value;
		}
		protected internal virtual void EnsureCurrentColumnExists() {
			List<ColumnInfo> columnCollection = columns.Columns;
			if (columnCollection.Count < currentColumnIndex + 1) {
				for (int i = columnCollection.Count; i <= currentColumnIndex; i++)
					columnCollection.Add(new ColumnInfo());
			}
		}
		public void CopyFrom(RtfSectionFormattingInfo info) {
			this.Page.CopyFrom(info.Page);
			this.Margins.CopyFrom(info.Margins);
			this.PageNumbering.CopyFrom(info.PageNumbering);
			this.GeneralSectionInfo.CopyFrom(info.GeneralSectionInfo);
			this.LineNumbering.CopyFrom(info.LineNumbering);
			this.Columns.CopyFrom(info.Columns);
			this.FootNote.CopyFrom(info.FootNote);
			this.EndNote.CopyFrom(info.EndNote);
		}
	}
	#endregion
	public class ControlCharTranslatorTable : Dictionary<char, TranslateControlCharHandler> {
	}
	public class KeywordTranslatorTable : Dictionary<string, TranslateKeywordHandler> {
	}
	#region RtfImageInfo (abstract class)
	public abstract partial class RtfImageInfo {
		#region Fields
		readonly DocumentModel documentModel;
		IUniqueImageId imageId;
		OfficeReferenceImage rtfImage;
		Size sizeInModelUnits;
		int scaleX;
		int scaleY;
		bool pseudoInline;
		#endregion
		protected RtfImageInfo(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		#region Properties
		protected DocumentModel DocumentModel { get { return documentModel; } }
		public OfficeReferenceImage RtfImage { get { return rtfImage; } set { rtfImage = value; } }
		public Size SizeInModelUnits { get { return sizeInModelUnits; } set { sizeInModelUnits = value; } }
		public int ScaleX { get { return scaleX; } set { scaleX = value; } }
		public int ScaleY { get { return scaleY; } set { scaleY = value; } }
		public bool PseudoInline { get { return pseudoInline; } set { pseudoInline = value; } }
		public IUniqueImageId ImageId { get { return imageId; } set { imageId = value; } }
		#endregion
		public abstract void LoadMetafileFromStream(MemoryStream stream, Win32.MapMode mapMode, int pictureWidth, int pictureHeight);
		public abstract void LoadImageFromStream(Stream stream);
		public abstract void LoadDibFromStream(Stream stream, int width, int height,  int bytesInLine);
		public abstract void LoadBitmapFromStream(MemoryStream stream, int width, int height, int colorPlanesCount, int bitsPerPixel, int bytesInLine);
	}
	#endregion
#if !SL
	#region RtfImageInfoWin
	public class RtfImageInfoWin : RtfImageInfo {
		public RtfImageInfoWin(DocumentModel documentModel)
			: base(documentModel) {
		}
		protected internal virtual void CreateImage(MemoryStreamBasedImage nativeImage) {
			RtfImage = DocumentModel.CreateImage(nativeImage);
		}
		public override void LoadMetafileFromStream(MemoryStream stream, Win32.MapMode mapMode, int pictureWidth, int pictureHeight) {
			if (object.ReferenceEquals(ImageId, null)) {
				long position = stream.Position;
				int crc32;
				using (Crc32Stream crc32Stream = new Crc32Stream(stream)) {
					crc32Stream.ReadToEnd();
					crc32 = (int)crc32Stream.ReadCheckSum;
				}
				ImageId = new Crc32ImageId(crc32);
				stream.Seek(position, SeekOrigin.Begin);
			}
			CreateImage(new MemoryStreamBasedImage(MetafileHelper.CreateMetafile(stream, mapMode, pictureWidth, pictureHeight), null, ImageId));
#if !DXPORTABLE
			OfficeMetafileImageWin metafile = RtfImage.NativeRootImage as OfficeMetafileImageWin;
			if (metafile != null && (pictureWidth <= 0 || pictureHeight <= 0)) {
				MetafilePhysicalDimensionCalculator calc = new MetafilePhysicalDimensionCalculator();
				Size sz = calc.Calculate((System.Drawing.Imaging.Metafile)metafile.NativeImage, stream.GetBuffer());
				if (pictureWidth > 0)
					sz.Width = pictureWidth;
				if (pictureHeight > 0)
					sz.Height = pictureHeight;
				metafile.MetafileSizeInHundredthsOfMillimeter = sz;
			}
#endif
		}
		public override void LoadImageFromStream(Stream stream) {
			CreateImage(ImageLoaderHelper.ImageFromStream(stream, ImageId));
		}
		public override void LoadDibFromStream(Stream stream, int width, int height, int bytesInLine) {
			CreateImage(DibHelper.CreateDib(stream, width, height, bytesInLine, ImageId));
		}
		public override void LoadBitmapFromStream(MemoryStream stream, int width, int height, int colorPlanesCount, int bitsPerPixel, int bytesInLine) {
			CreateImage(BitmapHelper.CreateBitmap(stream, width, height, colorPlanesCount, bitsPerPixel, bytesInLine, ImageId));
		}
	}
	#endregion
#else
	#region RtfImageInfoSL
		public class RtfImageInfoSL : RtfImageInfo {
		public RtfImageInfoSL(DocumentModel documentModel)
			: base(documentModel) {
			RtfImage = new OfficeReferenceImage(documentModel, new OfficeImageSL());
		}
		public new OfficeReferenceImage RtfImage { get { return base.RtfImage; } set { base.RtfImage = value; } }
		public override void LoadMetafileFromStream(MemoryStream stream, Win32.MapMode mapMode, int pictureWidth, int pictureHeight) {
			OfficeImageSL image = new OfficeImageSL();
			try {
				image.LoadMetafileFromStream(stream, mapMode, pictureWidth, pictureHeight);
				RtfImage = new OfficeReferenceImage(DocumentModel, image);				
			}
			catch {
				string imageUri = "DevExpress.XtraRichEdit.Images.ImagePlaceHolder.png";
				Stream stream2 = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(imageUri);
				image.LoadImageFromStream(stream2);
				RtfImage = new OfficeReferenceImage(DocumentModel, image);
			}
		}
		public override void LoadImageFromStream(Stream stream) {
			OfficeImageSL image = new OfficeImageSL();
			image.LoadImageFromStream(stream);
			RtfImage = new OfficeReferenceImage(DocumentModel, image);
		}
		public override void LoadDibFromStream(Stream stream, int width, int height, int bytesInLine) {
			OfficeImageSL image = new OfficeImageSL();
			image.LoadDibFromStream(stream, width, height, bytesInLine);
			RtfImage = new OfficeReferenceImage(DocumentModel, image);
		}
		public override void LoadBitmapFromStream(MemoryStream stream, int width, int height, int colorPlanesCount, int bitsPerPixel, int bytesInLine) {
			OfficeImageSL image = new OfficeImageSL();
			image.LoadBitmapFromStream(stream, width, height, colorPlanesCount, bitsPerPixel, bytesInLine);
			RtfImage = new OfficeReferenceImage(DocumentModel, image);
		}
	}
	#endregion
#endif
	#region RtfCharset
	public enum RtfCharset {
		Ansi, Mac, PC, Pca
	};
	#endregion
	#region RtfColorCollection
	public class RtfColorCollection : ColorCollection {
		public Color GetRtfColorById(int id) {
			if (id < 0 || id >= Count)
				return DXColor.Empty;
			return this[id];
		}
	}
	#endregion
	public enum RtfFontFamily {
		Default,
		Roman,
		Swiss,
		Modern,
		Script,
		Decor,
		Tech,
		Bidi
	}
	#region RtfFontInfo
	public class RtfFontInfo {
		int id;
		int charset = -1;
		string name = String.Empty;
		RtfFontFamily fontFamily;
		public int Id { get { return id; } set { id = value; } }
		public int Charset { get { return charset; } set { charset = value; } }
		public string Name { get { return name; } set { name = value; } }
		public RtfFontFamily FontFamily { get { return fontFamily; } set { fontFamily = value; } }
	}
	#endregion
	#region RtfFontInfoCollection
	public class RtfFontInfoCollection : List<RtfFontInfo> {
		internal readonly RtfFontInfo defaultRtfFontInfo;
		public RtfFontInfoCollection(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			defaultRtfFontInfo = CreateDefaultRtfFontInfo(documentModel);
		}
		static RtfFontInfo CreateDefaultRtfFontInfo(DocumentModel documentModel) {
			RtfFontInfo info = new RtfFontInfo();
			info.Name = "Times New Roman";
			info.Id = int.MaxValue;
			return info;
		}
		public RtfFontInfo GetRtfFontInfoById(int id) {
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].Id == id)
					return this[i];
			return defaultRtfFontInfo;
		}
	}
	#endregion
	#region RtfDocumentProperties
	public class RtfDocumentProperties {
		#region Fields
		readonly RtfColorCollection colors;
		readonly RtfListTable listTable;
		readonly ListOverrideTable listOverrideTable;
		readonly RtfFontInfoCollection fonts;
		readonly RtfSectionFormattingInfo defaultSectionProperties;
		readonly List<string> userNames;
		RtfCharset charset = RtfCharset.Ansi;
		int defaultFontNumber;
		int defaultCodePage = DXEncoding.GetEncodingCodePage(RtfImporter.DefaultEncoding);
		bool listTableComplete;
		bool listOverrideTableComplete;
		#endregion
		public RtfDocumentProperties(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.defaultSectionProperties = new RtfSectionFormattingInfo(documentModel);
			this.defaultSectionProperties.InitializeDefault(documentModel);
			this.colors = new RtfColorCollection();
			this.listTable = new RtfListTable();
			this.listOverrideTable = new ListOverrideTable();
			this.fonts = new RtfFontInfoCollection(documentModel);
			this.userNames = new List<string>();
		}
		#region Properties
		public RtfCharset Charset { get { return charset; } set { charset = value; } }
		public int DefaultFontNumber { get { return defaultFontNumber; } set { defaultFontNumber = value; } }
		public RtfColorCollection Colors { get { return colors; } }
		public RtfFontInfoCollection Fonts { get { return fonts; } }
		public int DefaultCodePage { get { return defaultCodePage; } set { defaultCodePage = value; } }
		public RtfSectionFormattingInfo DefaultSectionProperties { get { return defaultSectionProperties; } }
		public RtfListTable ListTable { get { return listTable; } }
		public ListOverrideTable ListOverrideTable { get { return listOverrideTable; } }
		public bool ListTableComplete { get { return listTableComplete; } set { listTableComplete = value; } }
		public bool ListOverrideTableComplete { get { return listOverrideTableComplete; } set { listOverrideTableComplete = value; } }
		internal List<string> UserNames { get { return userNames; } }
		#endregion
	}
	#endregion
	#region PopRtfStateResult
	public enum PopRtfStateResult {
		StackNonEmpty = 0,
		StackEmpty = 1,
	}
	#endregion
	public struct ParagraphTableStyleIndex {
		readonly ParagraphIndex paragraphIndex;
		readonly int rtfTableStyleIndex;
		public ParagraphTableStyleIndex(ParagraphIndex paragraphIndex, int rtfTableStyleIndex) {
			this.paragraphIndex = paragraphIndex;
			this.rtfTableStyleIndex = rtfTableStyleIndex;			
		}
		public ParagraphIndex ParagraphIndex { get {return paragraphIndex; } }
		public int RtfTableStyleIndex {get { return rtfTableStyleIndex; } }
	}
	public class ParagraphToTableStyleIndexMap {
		List<ParagraphTableStyleIndex> innerList;
		public ParagraphToTableStyleIndexMap() {
			innerList = new List<ParagraphTableStyleIndex>();
		}
		public void Add(ParagraphIndex paragraphIndex, int tableStyleIndex) {
			innerList.Add(new ParagraphTableStyleIndex(paragraphIndex, tableStyleIndex));
#if DEBUGTEST || DEBUG
			if(innerList.Count >= 2)
				Debug.Assert(innerList[innerList.Count - 1].ParagraphIndex > innerList[innerList.Count - 2].ParagraphIndex);
#endif
		}
		public ParagraphTableStyleIndex this[int index] { get {return innerList[index];} }
		public int Count { get { return innerList.Count; } }
	}
	#region RtfPieceTableInfo
	public class RtfPieceTableInfo : ImportPieceTableInfoBase<RTFImportCommentInfo> {
		#region Fields
		readonly RtfInputPosition position;
		readonly Stack<RtfFieldInfo> fields;
		readonly RtfTableReader tableReader;
		readonly ParagraphToTableStyleIndexMap paragraphTableStyles;
		#endregion
		public RtfPieceTableInfo(RtfImporter importer, PieceTable pieceTable)
			: base(pieceTable) {
			this.position = new RtfInputPosition(PieceTable);
			this.position.CharacterFormatting.ReplaceInfo(PieceTable.DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseAll));
			this.position.CharacterFormatting.BeginUpdate();
			this.position.CharacterFormatting.DoubleFontSize = 24;
			this.position.CharacterFormatting.FontName = "Times New Roman";
			this.position.CharacterFormatting.EndUpdate();
			this.fields = new Stack<RtfFieldInfo>(); 
			this.tableReader = new RtfTableReader(importer);
			this.paragraphTableStyles = new ParagraphToTableStyleIndexMap();
		}
		RtfPieceTableInfo(PieceTable pieceTable, RtfInputPosition position, Dictionary<string, ImportBookmarkInfo> bookmarks, Dictionary<string, ImportRangePermissionInfo> rangePermissions, Dictionary<string, RTFImportCommentInfo> comments, ImportCommentInfo activeComment, Stack<RtfFieldInfo> fields, RtfTableReader tableReader, ParagraphToTableStyleIndexMap paragraphTableStyles)
			: base(pieceTable, bookmarks, rangePermissions, comments, activeComment) {
			this.position = position;
			this.fields = fields;
			this.tableReader = tableReader;
			this.paragraphTableStyles = paragraphTableStyles;
		}
		public RtfPieceTableInfo CreateCopy() {
			return new RtfPieceTableInfo(PieceTable, Position, Bookmarks, RangePermissions, Comments, ActiveComment, Fields, TableReader, ParagraphTableStyles);
		}
		#region Properties
		public RtfInputPosition Position { get { return position; } }
		public Stack<RtfFieldInfo> Fields { get { return fields; } }
		public RtfTableReader TableReader { get { return tableReader; } }
		public ParagraphToTableStyleIndexMap ParagraphTableStyles { get { return paragraphTableStyles; } }
		#endregion
	}
	#endregion
	#region RtfImporterCache
	public class RtfImporterCache {
		readonly WidthUnitInfoCache unitInfoCache;
		readonly BorderInfoCache borderInfoCache;
		readonly TableCellGeneralSettingsInfoCache tableCellGeneralSettingsInfoCache;
		readonly TableRowGeneralSettingsInfoCache tableRowGeneralSettingsInfoCache;
		readonly HeightUnitInfoCache heightUnitInfoCache;
		readonly TableGeneralSettingsInfoCache tableGeneralSettingsInfoCache;
		readonly TableFloatingPositionInfoCache tableFloatingPositionInfoCache;
		public RtfImporterCache(DocumentModelUnitConverter converter) {
			this.unitInfoCache = new WidthUnitInfoCache(converter);
			this.borderInfoCache = new BorderInfoCache(converter);
			this.tableCellGeneralSettingsInfoCache = new TableCellGeneralSettingsInfoCache(converter);
			this.tableRowGeneralSettingsInfoCache = new TableRowGeneralSettingsInfoCache(converter);
			this.heightUnitInfoCache = new HeightUnitInfoCache(converter);
			this.tableGeneralSettingsInfoCache = new TableGeneralSettingsInfoCache(converter);
			this.tableFloatingPositionInfoCache = new TableFloatingPositionInfoCache(converter);
		}
		public WidthUnitInfoCache UnitInfoCache { get { return unitInfoCache; } }
		public BorderInfoCache BorderInfoCache { get { return borderInfoCache; } }
		public TableCellGeneralSettingsInfoCache TableCellGeneralSettingsInfoCache { get { return tableCellGeneralSettingsInfoCache; } }
		public TableRowGeneralSettingsInfoCache TableRowGeneralSettingsInfoCache { get { return tableRowGeneralSettingsInfoCache; } }
		public HeightUnitInfoCache HeightUnitInfoCache { get { return heightUnitInfoCache; } }
		public TableGeneralSettingsInfoCache TableGeneralSettingsInfoCache { get { return tableGeneralSettingsInfoCache; } }
		public TableFloatingPositionInfoCache TableFloatingPositionInfoCache { get { return tableFloatingPositionInfoCache; } }
	}
	#endregion
	public class RtfNumberingListInfo {
		readonly int rtfNumberingListIndex;
		readonly int listLevelIndex;
		public RtfNumberingListInfo(int rtfNumberingListIndex, int listLevelIndex) {
			this.rtfNumberingListIndex = rtfNumberingListIndex;
			this.listLevelIndex = listLevelIndex;
		}
		public int RtfNumberingListIndex { get { return rtfNumberingListIndex; } }
		public int ListLevelIndex { get { return listLevelIndex; } }
	}
	#region RtfImporter
	public class RtfImporter : RichEditDocumentModelImporter, IDisposable, IRtfImporter {
		#region Fields
		RtfParserStateManager stateManager;
		int optionalGroupLevel = int.MaxValue;
		int skipCount;
		Stream rtfStream;
		readonly RtfDocumentProperties docProperties;
		readonly Dictionary<int, int> paragraphStyleCollectionIndex;
		readonly Dictionary<int, int> characterStyleCollectionIndex;
		readonly Dictionary<int, int> tableStyleCollectionIndex;
		readonly Dictionary<int, int> linkParagraphStyleIndexToCharacterStyleIndex;
		readonly Dictionary<int, int> nextParagraphStyleIndexTable;
		readonly Dictionary<int, NumberingListIndex> listOverrideIndexToNumberingListIndexMap;
		readonly Dictionary<ParagraphStyle, RtfNumberingListInfo> paragraphStyleListOverrideIndexMap;
		readonly Dictionary<NumberingListIndex, RtfOldListLevelInfo> numberingListToOldListLevelInfoMap;
		readonly RtfImporterCache cache;
		readonly List<string> commentsId;
		readonly Dictionary<string, int> commentsRef;
		readonly Dictionary<int, string> commentsIndexRef;
		int refIndex = 0;
		static readonly Encoding defaultEncoding = CreateDefaultEncoding();
		static Encoding CreateDefaultEncoding() {
			Encoding result = DXEncoding.Default;
			if (result.CodePage == 65000 || result.CodePage == 65001 || result.CodePage == 1200 || result.CodePage == 1201)
				return DXEncoding.GetEncoding(1252);
			else
				return result;
		}
		#endregion
		public RtfImporter(DocumentModel documentModel, RtfDocumentImporterOptions options)
			: base(documentModel, options) {
			this.cache = new RtfImporterCache(documentModel.UnitConverter);
			this.docProperties = new RtfDocumentProperties(documentModel);
			this.paragraphStyleCollectionIndex = new Dictionary<int, int>();
			paragraphStyleCollectionIndex[0] = 0;
			this.characterStyleCollectionIndex = new Dictionary<int, int>();
			this.tableStyleCollectionIndex = new Dictionary<int, int>();
			this.listOverrideIndexToNumberingListIndexMap = new Dictionary<int, NumberingListIndex>();
			this.linkParagraphStyleIndexToCharacterStyleIndex = new Dictionary<int, int>();
			this.nextParagraphStyleIndexTable = new Dictionary<int, int>();
			this.paragraphStyleListOverrideIndexMap = new Dictionary<ParagraphStyle, RtfNumberingListInfo>();
			this.numberingListToOldListLevelInfoMap = new Dictionary<NumberingListIndex, RtfOldListLevelInfo>();
			this.commentsId = new List<string>();
			this.commentsRef = new Dictionary<string, int>();
			this.commentsIndexRef = new Dictionary<int, string>();
			this.UpdateFields = true;
		}
		#region Properties
		bool IRtfImporter.UpdateFields { get { return this.UpdateFields; } set { this.UpdateFields = value; } }
		internal bool UpdateFields { get; set; }
		internal RtfParserStateManager StateManager { get { return stateManager; } }
		internal RtfPieceTableInfo PieceTableInfo { get { return StateManager.PieceTableInfo; } }
		internal PieceTable PieceTable { get { return PieceTableInfo.PieceTable; } }
		internal int OptionalGroupLevel { get { return optionalGroupLevel; } set { optionalGroupLevel = value; } }
		internal DestinationBase Destination { get { return StateManager.Destination; } set { StateManager.Destination = value; } }
		internal RtfDocumentProperties DocumentProperties { get { return docProperties; } }
		internal RtfInputPosition Position { get { return PieceTableInfo.Position; } }
		internal long RtfStreamPosition { get { return rtfStream != null ? rtfStream.Position : -1; } }
		internal virtual RtfTableReader TableReader { get { return PieceTableInfo.TableReader; } }
		internal Dictionary<int, int> ParagraphStyleCollectionIndex { get { return paragraphStyleCollectionIndex; } }
		internal Dictionary<int, int> CharacterStyleCollectionIndex { get { return characterStyleCollectionIndex; } }
		internal Dictionary<int, int> TableStyleCollectionIndex { get { return tableStyleCollectionIndex; } }
		internal Dictionary<int, int> LinkParagraphStyleIndexToCharacterStyleIndex { get { return linkParagraphStyleIndexToCharacterStyleIndex; } }
		internal Dictionary<int, int> NextParagraphStyleIndexTable { get { return nextParagraphStyleIndexTable; } }
		internal Dictionary<int, NumberingListIndex> ListOverrideIndexToNumberingListIndexMap { get { return listOverrideIndexToNumberingListIndexMap; } }
		internal Dictionary<ParagraphStyle, RtfNumberingListInfo> ParagraphStyleListOverrideIndexMap { get { return paragraphStyleListOverrideIndexMap; } }
		internal Dictionary<NumberingListIndex, RtfOldListLevelInfo> NumberingListToOldListLevelInfoMap { get { return numberingListToOldListLevelInfoMap; } }
		internal Dictionary<string, ImportBookmarkInfo> Bookmarks { get { return PieceTableInfo.Bookmarks; } }
		internal Dictionary<string, ImportRangePermissionInfo> RangePermissions { get { return PieceTableInfo.RangePermissions; } }
		internal Dictionary<string, RTFImportCommentInfo> Comments { get { return PieceTableInfo.Comments; } }
		internal string ActiveCommentAuthor { get; set; }
		internal string ActiveCommentName { get; set; }
		internal List<string> CommentsId { get { return commentsId; } }
		internal Dictionary<string, int> CommentsRef { get { return commentsRef; } }
		internal Dictionary<int, string> CommentsIndexRef { get { return commentsIndexRef; } }
		internal int RefIndex { get { return refIndex; } set { refIndex = value; } }
		internal Stack<RtfFieldInfo> Fields { get { return PieceTableInfo.Fields; } }
		internal RtfImporterCache Cache { get { return cache; } }
		public RtfDocumentImporterOptions Options { get { return (RtfDocumentImporterOptions)InnerOptions; } }
		public static Encoding DefaultEncoding { get { return defaultEncoding; } }
		#endregion
		protected virtual RtfParserStateManager CreateParserStateManager() {
			return new RtfParserStateManager(this);
		}
		protected internal RTFImportCommentInfo GetCommentInfo(string name) {
			RTFImportCommentInfo result;
			if (!Comments.TryGetValue(name, out result)) {
				result = new RTFImportCommentInfo(DocumentModel.MainPieceTable);
				result.Name = name;
				Comments.Add(name, result);
			}
			return result;
		}
		public void Import(Stream stream) {
			if (stream == null)
				Exceptions.ThrowArgumentException("stream", stream);
			if (!stream.CanRead)
				Exceptions.ThrowArgumentException("Stream doesn't supports reading data", null);
			if (!stream.CanSeek)
				Exceptions.ThrowArgumentException("Stream doesn't supports seeking", null);
			ProgressIndication.Begin(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_Loading), (int)stream.Position, (int)(stream.Length - stream.Position), (int)(stream.Position));
			try {
				ImportCore(stream);
			}
			finally {
				ProgressIndication.End();
			}
		}
		protected virtual void CheckSignature(Stream stream) {
			byte[] signature = new byte[5];
			int bytesRead = stream.Read(signature, 0, signature.Length);
			stream.Seek(-bytesRead, SeekOrigin.Current);
			if (bytesRead != signature.Length || signature[0] != '{' || signature[1] != '\\' || signature[2] != 'r' || signature[3] != 't' || signature[4] != 'f')
				ThrowInvalidRtfFile();
		}
		protected internal void InitializeStateManager() {
			Debug.Assert(stateManager == null);
			this.stateManager = CreateParserStateManager();
			this.stateManager.Initialize();
		}
		protected void ImportCore(Stream stream) {
			DocumentModel.BeginSetContent();
			try {
				InitializeStateManager();
				this.rtfStream = stream;
				try {
					ImportRtfStream(stream);
				}
				finally {
					this.rtfStream = null;
				}
			}
			finally {
				DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, UpdateFields, Options.PasteFromIE, Options.UpdateField.GetNativeOptions());
			}
		}
		protected virtual void ImportRtfStream(Stream stream) {
			DocumentModel.DefaultCharacterProperties.FontName = "Times New Roman";
			DocumentModel.DefaultCharacterProperties.DoubleFontSize = 24;
			ClearNumberingList();
			CheckSignature(stream);
			char ch = (char)stream.ReadByte();
#if DEBUGTEST
			if (ch != '{') {
				stream.Seek(-1, SeekOrigin.Current);
				StateManager.SavedStates.Pop();
				StateManager.PieceTables.Pop();
			}
#else
			Debug.Assert(ch == '{');
#endif
			int nextChar = -1;
			for (; ; ) {
				int intChar;
				if (nextChar == -1)
					intChar = stream.ReadByte();
				else {
					intChar = nextChar;
					nextChar = -1;
				}
				if (intChar < 0)
					break;
				ch = (char)intChar;
				RtfParsingState parsingState = stateManager.ParsingState;
				if (parsingState == RtfParsingState.BinData)
					ParseBinChar(ch);
				else {
					switch (ch) {
						case '{':
							FlushDecoder();
							PushRtfState();
							ProgressIndication.SetProgress((int)stream.Position);
							break;
						case '}':
							FlushDecoder();
							ProgressIndication.SetProgress((int)stream.Position);
							if (PopRtfState() == PopRtfStateResult.StackEmpty)
								SkipDataBeyondOuterBrace(stream);
							break;
						case '\\':
							char lastReadChar = ParseRtfKeyword(stream);
							if (lastReadChar != ' ')
								nextChar = lastReadChar;
							break;
						case '\r':
							ParseCR();
							break;
						case '\n':
							ParseLF();
							break;
						case '\0':
							break;
						default:
							if (parsingState == RtfParsingState.Normal)
								ParseChar(ch);
							else
								ParseHexChar(stream, ch);
							break;
					}
				}
			}
			if (BinCharCount != 0)
				ThrowUnexpectedEndOfFile();
			if (stateManager.SavedStates.Count > 0)
				ThrowUnexpectedEndOfFile();
			if (Fields.Count != 0)
				ThrowInvalidRtfFile();
			DefaultDestination lastDestination = Destination as DefaultDestination;
			if (lastDestination == null)
				Exceptions.ThrowInternalException();
			lastDestination.FinalizePieceTableCreation();
			CheckTablesStructure();
		}
		void CheckTablesStructure() {
			List<PieceTable> pieceTables = DocumentModel.GetPieceTables(false);
			foreach (PieceTable pieceTable in pieceTables) {
				foreach (Table table in pieceTable.Tables) {
					CheckTableStructure(table);
				}
			}
		}
		void CheckTableStructure(Table table) {
			TableRowCollection rows = table.Rows;
			int rowCount = rows.Count;
			if (rowCount <= 1)
				return;
			TableRow firstRow = rows[0];
			int totalColumnCount = firstRow.GridBefore + firstRow.GridAfter;
			TableCellCollection cells = firstRow.Cells;
			int cellCount = cells.Count;
			for (int i = 0; i < cellCount; i++) {
				totalColumnCount += cells[i].ColumnSpan;
			}
			int[] prevRowCellIndices = new int[totalColumnCount];
			CalculateRowCellIndices(prevRowCellIndices, firstRow);
			for (int i = 1; i < rowCount; i++) {
				CheckVerticalMerging(prevRowCellIndices, rows[i]);
				CalculateRowCellIndices(prevRowCellIndices, rows[i]);
			}
		}
		void CheckVerticalMerging(int[] prevRowCellIndices, TableRow tableRow) {
			TableCellCollection cells = tableRow.Cells;
			int cellCount = cells.Count;
			int currentColumnIndex = tableRow.GridBefore;
			for (int i = 0; i < cellCount; i++) {
				if (cells[i].VerticalMerging == MergingState.Continue) {
					int expectedIndex = prevRowCellIndices[currentColumnIndex];
					if (currentColumnIndex > 0 && prevRowCellIndices[currentColumnIndex - 1] == expectedIndex)
						ThrowInvalidFile();
					int lastColumnIndex = currentColumnIndex + cells[i].ColumnSpan - 1;
					if (lastColumnIndex + 1 < prevRowCellIndices.Length && prevRowCellIndices[lastColumnIndex + 1] == expectedIndex)
						ThrowInvalidFile();
					for (int j = currentColumnIndex + 1; j <= lastColumnIndex; j++) {
						if (prevRowCellIndices[j] != expectedIndex)
							ThrowInvalidFile();
					}
				}
				currentColumnIndex += cells[i].ColumnSpan;
			}
		}
		void CalculateRowCellIndices(int[] cellIndices, TableRow row) {
			TableCellCollection cells = row.Cells;
			int cellCount = cells.Count;
			int columnIndex = 0;
			columnIndex = ApplyCellIndices(cellIndices, columnIndex, row.GridBefore, 0);
			for (int i = 0; i < cellCount; i++) {
				columnIndex = ApplyCellIndices(cellIndices, columnIndex, cells[i].ColumnSpan, i + 1);
			}
			columnIndex = ApplyCellIndices(cellIndices, columnIndex, row.GridAfter, cellCount + 1);
		}
		int ApplyCellIndices(int[] cellIndices, int startColumnIndex, int columnCount, int cellIndex) {
			if (startColumnIndex + columnCount > cellIndices.Length)
				ThrowInvalidFile();
			for (int i = startColumnIndex; i < startColumnIndex + columnCount; i++)
				cellIndices[i] = cellIndex;
			return startColumnIndex + columnCount;
		}
		static void SkipDataBeyondOuterBrace(Stream stream) {
			stream.Seek(0, SeekOrigin.End);
		}
		int CodePageFromCharset(int charset) {
			if (charset >= 0)
				return DXEncoding.CodePageFromCharset(charset);
			else
				return docProperties.DefaultCodePage;
		}
		internal void SetFont(RtfFontInfo fontInfo) {
			Position.SetFont(fontInfo.Name);
			int codePage = CodePageFromCharset(fontInfo.Charset);
			SetCodePage(codePage);
		}
		internal void SetCodePage(int codePage) {
			Position.RtfFormattingInfo.CodePage = codePage;
		}
		void ClearNumberingList() {
			DocumentModel.NumberingLists.Clear();
		}
		internal void ApplyCharacterProperties(CharacterProperties characterProperties, CharacterFormattingInfo characterFormattingInfo, MergedCharacterProperties parentCharacterFormatting) {
			ApplyCharacterProperties(characterProperties, characterFormattingInfo, parentCharacterFormatting, true);
		}
		internal void ApplyCharacterProperties(CharacterProperties characterProperties, CharacterFormattingInfo characterFormattingInfo, MergedCharacterProperties parentCharacterFormatting, bool resetUse) {
			characterProperties.BeginUpdate();
			try {
				if (resetUse)
					characterProperties.ResetAllUse();
				CharacterFormattingInfo parentCharacterInfo = parentCharacterFormatting.Info;
				if (characterFormattingInfo.AllCaps != parentCharacterInfo.AllCaps)
					characterProperties.AllCaps = characterFormattingInfo.AllCaps;
				if (characterFormattingInfo.Hidden != parentCharacterInfo.Hidden)
					characterProperties.Hidden = characterFormattingInfo.Hidden;
				if (characterFormattingInfo.FontBold != parentCharacterInfo.FontBold)
					characterProperties.FontBold = characterFormattingInfo.FontBold;
				if (characterFormattingInfo.FontItalic != parentCharacterInfo.FontItalic)
					characterProperties.FontItalic = characterFormattingInfo.FontItalic;
				if (characterFormattingInfo.FontName != parentCharacterInfo.FontName)
					characterProperties.FontName = characterFormattingInfo.FontName;
				if (characterFormattingInfo.DoubleFontSize != parentCharacterInfo.DoubleFontSize)
					characterProperties.DoubleFontSize = characterFormattingInfo.DoubleFontSize;
				if (characterFormattingInfo.FontUnderlineType != parentCharacterInfo.FontUnderlineType)
					characterProperties.FontUnderlineType = characterFormattingInfo.FontUnderlineType;
				if (characterFormattingInfo.ForeColor != parentCharacterInfo.ForeColor)
					characterProperties.ForeColor = characterFormattingInfo.ForeColor;
				if (characterFormattingInfo.BackColor != parentCharacterInfo.BackColor)
					characterProperties.BackColor = characterFormattingInfo.BackColor;
				if (characterFormattingInfo.Script != parentCharacterInfo.Script)
					characterProperties.Script = characterFormattingInfo.Script;
				if (characterFormattingInfo.StrikeoutColor != parentCharacterInfo.StrikeoutColor)
					characterProperties.StrikeoutColor = characterFormattingInfo.StrikeoutColor;
				if (characterFormattingInfo.FontStrikeoutType != parentCharacterInfo.FontStrikeoutType)
					characterProperties.FontStrikeoutType = characterFormattingInfo.FontStrikeoutType;
				if (characterFormattingInfo.StrikeoutWordsOnly != parentCharacterInfo.StrikeoutWordsOnly)
					characterProperties.StrikeoutWordsOnly = characterFormattingInfo.StrikeoutWordsOnly;
				if (characterFormattingInfo.UnderlineColor != parentCharacterInfo.UnderlineColor)
					characterProperties.UnderlineColor = characterFormattingInfo.UnderlineColor;
				if (characterFormattingInfo.UnderlineWordsOnly != parentCharacterInfo.UnderlineWordsOnly)
					characterProperties.UnderlineWordsOnly = characterFormattingInfo.UnderlineWordsOnly;
				if (!characterFormattingInfo.LangInfo.Equals(parentCharacterInfo.LangInfo))
					characterProperties.LangInfo = characterFormattingInfo.LangInfo;
				if (characterFormattingInfo.NoProof != parentCharacterInfo.NoProof)
					characterProperties.NoProof = characterFormattingInfo.NoProof;
			}
			finally {
				characterProperties.EndUpdate();
			}
		}
		internal void ApplySectionFormatting() {
			ApplySectionFormatting(false);
		}
		internal void ApplySectionFormatting(bool skipNumbering) {
			RtfSectionFormattingInfo sectionFormatting = Position.SectionFormattingInfo;
			SectionCollection sections = DocumentModel.Sections;
			Section section = sections.Last;
			sectionFormatting.Page.ValidatePaperKind(DocumentModel.UnitConverter);
			section.Page.CopyFrom(sectionFormatting.Page);
			section.Margins.CopyFrom(sectionFormatting.Margins);
			section.PageNumbering.CopyFrom(sectionFormatting.PageNumbering);
			section.GeneralSettings.CopyFrom(sectionFormatting.GeneralSectionInfo);
			section.LineNumbering.CopyFrom(sectionFormatting.LineNumbering);
			section.Columns.CopyFrom(sectionFormatting.Columns);
			section.FootNote.CopyFrom(sectionFormatting.FootNote);
			section.EndNote.CopyFrom(sectionFormatting.EndNote);
			RtfParagraphFormattingInfo paragraphFormatting = Position.ParagraphFormattingInfo;
			NumberingListIndex listIndex = paragraphFormatting.NumberingListIndex;
			if (!skipNumbering && listIndex >= new NumberingListIndex(0) && DocumentFormatsHelper.ShouldInsertNumbering(DocumentModel)) {
				Paragraph paragraph = PieceTable.Paragraphs[Position.ParagraphIndex];
				if (!paragraph.IsInList()) 
					AddNumberingListToParagraph(paragraph, paragraphFormatting.NumberingListIndex, paragraphFormatting.ListLevelIndex);
			}
			else {
				Position.CurrentOldSimpleListIndex = NumberingListIndex.ListIndexNotSetted;
				Position.CurrentOldMultiLevelListIndex = NumberingListIndex.ListIndexNotSetted;
			}
			if (sections.Count == 1 || sectionFormatting.RestartPageNumbering)
				section.PageNumbering.StartingPageNumber = sectionFormatting.PageNumbering.StartingPageNumber;
			else
				section.PageNumbering.StartingPageNumber = -1;
		}
		protected internal void AddNumberingListToParagraph(Paragraph paragraph, NumberingListIndex listIndex, int levelIndex) {
			if (!DocumentFormatsHelper.ShouldInsertMultiLevelNumbering(DocumentModel))
				levelIndex = 0;
			ConvertListLevelsToAnotherType(listIndex);
			PieceTable.AddNumberingListToParagraph(paragraph, listIndex, levelIndex);
		}
		#region ConvertListLevelsToAnotherType
		protected internal void ConvertListLevelsToAnotherType(NumberingListIndex listIndex) {
			NumberingList list = DocumentModel.NumberingLists[listIndex];
			NumberingType type = NumberingListHelper.GetListType(list);
			if (DocumentFormatsHelper.NeedReplaceSimpleToBulletNumbering(DocumentModel) && type == NumberingType.Simple)
				ConvertListLevelsToAnotherTypeCore(list.Levels, Characters.MiddleDot.ToString(), "Symbol", NumberingFormat.Bullet);
			else if (DocumentFormatsHelper.NeedReplaceBulletedLevelsToDecimal(DocumentModel) && type == NumberingType.Bullet)
				ConvertListLevelsToAnotherTypeCore(list.Levels, "{0}.", DocumentModel.DefaultCharacterProperties.FontName, NumberingFormat.Decimal);
		}
		protected internal void ConvertListLevelsToAnotherTypeCore(IReadOnlyIListLevelCollection levels, string displayFormat, string fontName, NumberingFormat format) {
			for (int i = 0; i < levels.Count; i++) {
				IListLevel level = levels[i];
				level.ListLevelProperties.DisplayFormatString = displayFormat;
				level.CharacterProperties.FontName = fontName;
				level.ListLevelProperties.Format = format;
			}
		}
		#endregion
		int lastParagraphFormattingCacheIndex = -1;
		int lastParagraphPropertiesCacheIndex = -1;
		internal void ApplyParagraphFormattingCore(Paragraph paragraph, RtfParagraphFormattingInfo paragraphFormatting) {
			paragraph.ParagraphStyleIndex = paragraphFormatting.StyleIndex;
			ParagraphProperties paragraphProperties = paragraph.ParagraphProperties;
			Debug.Assert(!paragraphProperties.IsUpdateLocked); 
			int paragraphFormattingCacheIndex = DocumentModel.Cache.ParagraphFormattingInfoCache.GetItemIndex(paragraphFormatting);
			if (paragraphFormattingCacheIndex == lastParagraphFormattingCacheIndex) {
				paragraphProperties.SetIndexInitial(lastParagraphPropertiesCacheIndex);
			}
			else {
				paragraphProperties.ReplaceInfo(new ParagraphFormattingBase(PieceTable, DocumentModel, paragraphFormattingCacheIndex, ParagraphFormattingOptionsCache.RootParagraphFormattingOptionIndex), paragraphProperties.GetBatchUpdateChangeActions());
				this.lastParagraphFormattingCacheIndex = paragraphFormattingCacheIndex;
				this.lastParagraphPropertiesCacheIndex = paragraphProperties.Index;
			}
		}
		int lastParagraphFrameFormattingCacheIndex = -1;
		int lastParagraphFramePropertiesCacheIndex = -1;
		internal void ApplyParagraphFrameFormatting(Paragraph paragraph, ParagraphFrameFormattingInfo paragraphFrameFormatting) {
			paragraph.FrameProperties = new FrameProperties(paragraph);
			FrameProperties frameProperties = paragraph.FrameProperties;
			Debug.Assert(!frameProperties.IsUpdateLocked); 
			int paragraphFrameFormattingCacheIndex = DocumentModel.Cache.ParagraphFrameFormattingInfoCache.GetItemIndex(paragraphFrameFormatting);
			if (paragraphFrameFormattingCacheIndex == lastParagraphFrameFormattingCacheIndex) {
				frameProperties.SetIndexInitial(lastParagraphFramePropertiesCacheIndex);
			}
			else {
				frameProperties.ReplaceInfo(new ParagraphFrameFormattingBase(PieceTable, DocumentModel, paragraphFrameFormattingCacheIndex, ParagraphFrameFormattingOptionsCache.RootParagraphFrameFormattingOptionIndex), frameProperties.GetBatchUpdateChangeActions());
			}
			this.lastParagraphFrameFormattingCacheIndex = paragraphFrameFormattingCacheIndex;
			this.lastParagraphFramePropertiesCacheIndex = frameProperties.Index;
		}
		internal void ApplyParagraphFormatting(ParagraphIndex paragraphIndex, bool sectionBreak) {
			Paragraph paragraph = PieceTable.Paragraphs[paragraphIndex];
			RtfParagraphFormattingInfo paragraphFormatting = Position.ParagraphFormattingInfo;
			ApplyParagraphFormattingCore(paragraph, paragraphFormatting);
			if (DocumentFormatsHelper.ShouldInsertNumbering(DocumentModel)) {
				NumberingListIndex listIndex = paragraphFormatting.NumberingListIndex;
				if (listIndex >= new NumberingListIndex(0) || listIndex == NumberingListIndex.NoNumberingList) {
					if (!(sectionBreak && paragraph.IsEmpty)) {
						if (listIndex >= NumberingListIndex.MinValue)
							AddNumberingListToParagraph(paragraph, listIndex, paragraphFormatting.ListLevelIndex);
						else
							PieceTable.AddNumberingListToParagraph(paragraph, listIndex, 0);
					}
				}
				else if (!Position.CurrentOldListSkipNumbering) {
					Position.CurrentOldMultiLevelListIndex = NumberingListIndex.ListIndexNotSetted;
					Position.CurrentOldSimpleListIndex = NumberingListIndex.ListIndexNotSetted;
				}
			}
			paragraph.SetOwnTabs(paragraphFormatting.Tabs);
			if(Position.ParagraphFormattingInfo.RtfTableStyleIndexForRowOrCell != 0)
				PieceTableInfo.ParagraphTableStyles.Add(paragraphIndex, Position.ParagraphFormattingInfo.RtfTableStyleIndexForRowOrCell);
		}
		internal void ApplyParagraphProperties(ParagraphProperties paragraphProperties, int parentParagraphRtfStyleIndex, RtfParagraphFormattingInfo paragraphFormatting) {
			MergedParagraphProperties parentParagraphProperties = GetStyleMergedParagraphProperties(parentParagraphRtfStyleIndex);
			ApplyLineSpacing(paragraphFormatting);
			ApplyParagraphProperties(paragraphProperties, paragraphFormatting, parentParagraphProperties);
		}
		internal void ApplyLineSpacing(RtfParagraphFormattingInfo paragraphFormatting) {
			paragraphFormatting.LineSpacingType = paragraphFormatting.CalcLineSpacingType();
			paragraphFormatting.LineSpacing = paragraphFormatting.CalcLineSpacing(UnitConverter);
		}
		internal void ApplyParagraphProperties(ParagraphProperties paragraphProperties, ParagraphFormattingInfo paragraphFormatting, MergedParagraphProperties parentParagraphProperties) {
			ApplyParagraphProperties(paragraphProperties, paragraphFormatting, parentParagraphProperties, true);
		}
		internal void ApplyParagraphProperties(ParagraphProperties paragraphProperties, ParagraphFormattingInfo paragraphFormatting, MergedParagraphProperties parentParagraphProperties, bool resetUse) {
			paragraphProperties.BeginUpdate();
			try {
				if (resetUse)
					paragraphProperties.ResetAllUse();
				ParagraphFormattingInfo parentParagraphInfo = parentParagraphProperties.Info;
				if (paragraphFormatting.Alignment != parentParagraphInfo.Alignment)
					paragraphProperties.Alignment = paragraphFormatting.Alignment;
				if (paragraphFormatting.RightIndent != parentParagraphInfo.RightIndent)
					paragraphProperties.RightIndent = paragraphFormatting.RightIndent;
				if (paragraphFormatting.FirstLineIndentType != parentParagraphInfo.FirstLineIndentType)
					paragraphProperties.FirstLineIndentType = paragraphFormatting.FirstLineIndentType;
				int actualLeftIndent = paragraphFormatting.LeftIndent;
				if (actualLeftIndent != parentParagraphInfo.LeftIndent)
					paragraphProperties.LeftIndent = actualLeftIndent;
				if (paragraphFormatting.FirstLineIndent != parentParagraphInfo.FirstLineIndent)
					paragraphProperties.FirstLineIndent = paragraphFormatting.FirstLineIndent;
				if (paragraphFormatting.SuppressHyphenation != parentParagraphInfo.SuppressHyphenation)
					paragraphProperties.SuppressHyphenation = paragraphFormatting.SuppressHyphenation;
				if (paragraphFormatting.SuppressLineNumbers != parentParagraphInfo.SuppressLineNumbers)
					paragraphProperties.SuppressLineNumbers = paragraphFormatting.SuppressLineNumbers;
				if (paragraphFormatting.ContextualSpacing != parentParagraphInfo.ContextualSpacing)
					paragraphProperties.ContextualSpacing = paragraphFormatting.ContextualSpacing;
				if (paragraphFormatting.PageBreakBefore != parentParagraphInfo.PageBreakBefore)
					paragraphProperties.PageBreakBefore = paragraphFormatting.PageBreakBefore;
				if (paragraphFormatting.BeforeAutoSpacing != parentParagraphInfo.BeforeAutoSpacing)
					paragraphProperties.BeforeAutoSpacing = paragraphFormatting.BeforeAutoSpacing;
				if (paragraphFormatting.AfterAutoSpacing != parentParagraphInfo.AfterAutoSpacing)
					paragraphProperties.AfterAutoSpacing = paragraphFormatting.AfterAutoSpacing;
				if (paragraphFormatting.KeepWithNext != parentParagraphInfo.KeepWithNext)
					paragraphProperties.KeepWithNext = paragraphFormatting.KeepWithNext;
				if (paragraphFormatting.KeepLinesTogether != parentParagraphInfo.KeepLinesTogether)
					paragraphProperties.KeepLinesTogether = paragraphFormatting.KeepLinesTogether;
				if (paragraphFormatting.WidowOrphanControl != parentParagraphInfo.WidowOrphanControl)
					paragraphProperties.WidowOrphanControl = paragraphFormatting.WidowOrphanControl;
				if (paragraphFormatting.OutlineLevel != parentParagraphInfo.OutlineLevel)
					paragraphProperties.OutlineLevel = paragraphFormatting.OutlineLevel;
				if (paragraphFormatting.BackColor != parentParagraphInfo.BackColor)
					paragraphProperties.BackColor = paragraphFormatting.BackColor;
				if (paragraphFormatting.SpacingBefore != parentParagraphInfo.SpacingBefore)
					paragraphProperties.SpacingBefore = paragraphFormatting.SpacingBefore;
				if (paragraphFormatting.SpacingAfter != parentParagraphInfo.SpacingAfter)
					paragraphProperties.SpacingAfter = paragraphFormatting.SpacingAfter;
				bool shouldApplyLineSpacing = paragraphFormatting.LineSpacingType != parentParagraphInfo.LineSpacingType || paragraphFormatting.LineSpacing != parentParagraphInfo.LineSpacing;
				if (shouldApplyLineSpacing) {
					paragraphProperties.LineSpacingType = paragraphFormatting.LineSpacingType;
					paragraphProperties.LineSpacing = paragraphFormatting.LineSpacing;
				}
				if (paragraphFormatting.LeftBorder != parentParagraphInfo.LeftBorder)
					paragraphProperties.LeftBorder = paragraphFormatting.LeftBorder;
				if (paragraphFormatting.RightBorder != parentParagraphInfo.RightBorder)
					paragraphProperties.RightBorder = paragraphFormatting.RightBorder;
				if (paragraphFormatting.TopBorder != parentParagraphInfo.TopBorder)
					paragraphProperties.TopBorder = paragraphFormatting.TopBorder;
				if (paragraphFormatting.BottomBorder != parentParagraphInfo.BottomBorder)
					paragraphProperties.BottomBorder = paragraphFormatting.BottomBorder;
			}
			finally {
				paragraphProperties.EndUpdate();
			}
		}
		internal MergedParagraphProperties GetStyleMergedParagraphProperties(int rtfStyleIndex) {
			int styleIndex = GetParagraphStyleIndex(rtfStyleIndex);
			return GetStyleMergedParagraphProperties(DocumentModel.ParagraphStyles[styleIndex].GetMergedParagraphProperties());
		}
		internal MergedParagraphProperties GetStyleMergedParagraphProperties(MergedParagraphProperties parentMergedProperties) {
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(parentMergedProperties);
			merger.Merge(DocumentModel.DefaultParagraphProperties);
			return merger.MergedProperties;
		}
		internal MergedTableProperties GetStyleMergedTableProperties(MergedTableProperties parentMergedProperties) {
			TablePropertiesMerger merger = new TablePropertiesMerger(parentMergedProperties);
			merger.Merge(DocumentModel.DefaultTableProperties);
			return merger.MergedProperties;
		}
		internal MergedTableRowProperties GetStyleMergedTableRowProperties(MergedTableRowProperties parentMergedProperties) {
			TableRowPropertiesMerger merger = new TableRowPropertiesMerger(parentMergedProperties);
			merger.Merge(DocumentModel.DefaultTableRowProperties);
			return merger.MergedProperties;
		}
		internal MergedTableCellProperties GetStyleMergedTableCellProperties(MergedTableCellProperties parentMergedProperties) {
			TableCellPropertiesMerger merger = new TableCellPropertiesMerger(parentMergedProperties);
			merger.Merge(DocumentModel.DefaultTableCellProperties);
			return merger.MergedProperties;
		}
		internal MergedCharacterProperties GetStyleMergedParagraphCharacterProperties(int rtfCharacterStyleIndex, int rtfParagraphStyleIndex) {
			int paragraphStyleIndex = GetParagraphStyleIndex(rtfParagraphStyleIndex);
			int characterStyleIndex = GetCharacterStyleIndex(rtfCharacterStyleIndex);
			MergedCharacterProperties characterResult = DocumentModel.CharacterStyles[characterStyleIndex].GetMergedCharacterProperties();
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(characterResult);
			MergedCharacterProperties paragraphResult = DocumentModel.ParagraphStyles[paragraphStyleIndex].GetMergedCharacterProperties();
			merger.Merge(paragraphResult);
			CharacterProperties defaultProperties = DocumentModel.DefaultCharacterProperties;
			merger.Merge(defaultProperties);
			return merger.MergedProperties;
		}
		protected internal virtual int GetCharacterStyleIndex(int rtfCharacterStyleIndex) {
			if (CharacterStyleCollectionIndex.ContainsKey(rtfCharacterStyleIndex))
				return CharacterStyleCollectionIndex[rtfCharacterStyleIndex];
			return 0;
		}
		protected internal virtual int GetParagraphStyleIndex(int rtfParagraphStyleIndex) {
			if (ParagraphStyleCollectionIndex.ContainsKey(rtfParagraphStyleIndex))
				return ParagraphStyleCollectionIndex[rtfParagraphStyleIndex];
			return 0;
		}
		protected internal int GetTableStyleIndex(int rtfTableStyleIndex) {
			return GetTableStyleIndex(rtfTableStyleIndex, 0);
		}
		protected internal virtual int GetTableStyleIndex(int rtfTableStyleIndex, int unknowStyleIndex) {
			if (TableStyleCollectionIndex.ContainsKey(rtfTableStyleIndex))
				return TableStyleCollectionIndex[rtfTableStyleIndex];
			return unknowStyleIndex;
		}
		internal MergedCharacterProperties GetStyleMergedCharacterProperties(int rtfStyleIndex) {
			int styleIndex = GetCharacterStyleIndex(rtfStyleIndex);
			return GetStyleMergedCharacterProperties(DocumentModel.CharacterStyles[styleIndex].GetMergedCharacterProperties());
		}
		internal MergedCharacterProperties GetStyleMergedCharacterProperties(MergedCharacterProperties parentMergedProperties) {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(parentMergedProperties);
			merger.Merge(DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties;
		}
		internal void ApplyTableProperties(TableProperties tableProperties, MergedTableProperties parentTableProperties) {
			tableProperties.ResetAllUse();
			CombinedTablePropertiesInfo parentTablePropertiesInfo = parentTableProperties.Info;
			CombinedTablePropertiesInfo tablePropertiesInfo = new CombinedTablePropertiesInfo(tableProperties).Clone();
			if (tablePropertiesInfo.Borders.TopBorder != parentTablePropertiesInfo.Borders.TopBorder)
				tableProperties.Borders.TopBorder.CopyFrom(tablePropertiesInfo.Borders.TopBorder);
			if (tablePropertiesInfo.Borders.RightBorder != parentTablePropertiesInfo.Borders.RightBorder)
				tableProperties.Borders.RightBorder.CopyFrom(tablePropertiesInfo.Borders.RightBorder);
			if (tablePropertiesInfo.Borders.LeftBorder != parentTablePropertiesInfo.Borders.LeftBorder)
				tableProperties.Borders.LeftBorder.CopyFrom(tablePropertiesInfo.Borders.LeftBorder);
			if (tablePropertiesInfo.Borders.BottomBorder != parentTablePropertiesInfo.Borders.BottomBorder)
				tableProperties.Borders.BottomBorder.CopyFrom(tablePropertiesInfo.Borders.BottomBorder);
			if (tablePropertiesInfo.Borders.InsideHorizontalBorder != parentTablePropertiesInfo.Borders.InsideHorizontalBorder)
				tableProperties.Borders.InsideHorizontalBorder.CopyFrom(tablePropertiesInfo.Borders.InsideHorizontalBorder);
			if (tablePropertiesInfo.Borders.InsideVerticalBorder != parentTablePropertiesInfo.Borders.InsideVerticalBorder)
				tableProperties.Borders.InsideVerticalBorder.CopyFrom(tablePropertiesInfo.Borders.InsideVerticalBorder);
			if (!tablePropertiesInfo.CellMargins.Top.Equals(parentTablePropertiesInfo.CellMargins.Top)) {
				tableProperties.CellMargins.Top.CopyFrom(tablePropertiesInfo.CellMargins.Top);
			}
			if (!tablePropertiesInfo.CellMargins.Right.Equals(parentTablePropertiesInfo.CellMargins.Right)) {
				tableProperties.CellMargins.Right.CopyFrom(tablePropertiesInfo.CellMargins.Right);
			}
			if (!tablePropertiesInfo.CellMargins.Left.Equals(parentTablePropertiesInfo.CellMargins.Left)) {
				tableProperties.CellMargins.Left.CopyFrom(tablePropertiesInfo.CellMargins.Left);
			}
			if (!tablePropertiesInfo.CellMargins.Bottom.Equals(parentTablePropertiesInfo.CellMargins.Bottom)) {
				tableProperties.CellMargins.Bottom.CopyFrom(tablePropertiesInfo.CellMargins.Bottom);
			}
			if (!tablePropertiesInfo.CellSpacing.Equals(parentTablePropertiesInfo.CellSpacing))
				tableProperties.CellSpacing.CopyFrom(tablePropertiesInfo.CellSpacing);
			if (!tablePropertiesInfo.FloatingPosition.Equals(parentTablePropertiesInfo.FloatingPosition))
				tableProperties.FloatingPosition.CopyFrom(tablePropertiesInfo.FloatingPosition);
			if (tablePropertiesInfo.GeneralSettings.IsTableOverlap != parentTablePropertiesInfo.GeneralSettings.IsTableOverlap)
				tableProperties.IsTableOverlap = tablePropertiesInfo.GeneralSettings.IsTableOverlap;
			if (tablePropertiesInfo.GeneralSettings.TableLayout != parentTablePropertiesInfo.GeneralSettings.TableLayout)
				tableProperties.TableLayout = tablePropertiesInfo.GeneralSettings.TableLayout;
			if (tablePropertiesInfo.GeneralSettings.TableLook != parentTablePropertiesInfo.GeneralSettings.TableLook)
				tableProperties.TableLook = tablePropertiesInfo.GeneralSettings.TableLook;
			if (tablePropertiesInfo.GeneralSettings.TableStyleColBandSize != parentTablePropertiesInfo.GeneralSettings.TableStyleColBandSize)
				tableProperties.TableStyleColBandSize = tablePropertiesInfo.GeneralSettings.TableStyleColBandSize;
			if (tablePropertiesInfo.GeneralSettings.TableStyleRowBandSize != parentTablePropertiesInfo.GeneralSettings.TableStyleRowBandSize)
				tableProperties.TableStyleRowBandSize = tablePropertiesInfo.GeneralSettings.TableStyleRowBandSize;
			if (!tablePropertiesInfo.PreferredWidth.Equals(parentTablePropertiesInfo.PreferredWidth))
				tableProperties.PreferredWidth.CopyFrom(tablePropertiesInfo.PreferredWidth);
			if (!tablePropertiesInfo.TableIndent.Equals(parentTablePropertiesInfo.TableIndent))
				tableProperties.TableIndent.CopyFrom(tablePropertiesInfo.TableIndent);
		}
		internal void ApplyTableRowProperties(TableRowProperties rowProperties, MergedTableRowProperties parentRowProperties) {
			rowProperties.ResetAllUse();
			CombinedTableRowPropertiesInfo parentRowPropertiesInfo = parentRowProperties.Info;
			CombinedTableRowPropertiesInfo rowPropertiesInfo = new CombinedTableRowPropertiesInfo(rowProperties).Clone();
			if (!rowPropertiesInfo.CellSpacing.Equals(parentRowPropertiesInfo.CellSpacing))
				rowProperties.CellSpacing.CopyFrom(rowPropertiesInfo.CellSpacing);
			if (rowPropertiesInfo.GeneralSettings.CantSplit != parentRowPropertiesInfo.GeneralSettings.CantSplit)
				rowProperties.CantSplit = rowPropertiesInfo.GeneralSettings.CantSplit;
			if (rowPropertiesInfo.GeneralSettings.GridAfter != parentRowPropertiesInfo.GeneralSettings.GridAfter)
				rowProperties.GridAfter = rowPropertiesInfo.GeneralSettings.GridAfter;
			if (rowPropertiesInfo.GeneralSettings.GridBefore != parentRowPropertiesInfo.GeneralSettings.GridBefore)
				rowProperties.GridBefore = rowPropertiesInfo.GeneralSettings.GridBefore;
			if (rowPropertiesInfo.GeneralSettings.Header != parentRowPropertiesInfo.GeneralSettings.Header)
				rowProperties.Header = rowPropertiesInfo.GeneralSettings.Header;
			if (rowPropertiesInfo.GeneralSettings.HideCellMark != parentRowPropertiesInfo.GeneralSettings.HideCellMark)
				rowProperties.HideCellMark = rowPropertiesInfo.GeneralSettings.HideCellMark;
			if (rowPropertiesInfo.GeneralSettings.TableRowAlignment != parentRowPropertiesInfo.GeneralSettings.TableRowAlignment)
				rowProperties.TableRowAlignment = rowPropertiesInfo.GeneralSettings.TableRowAlignment;
			if (!rowPropertiesInfo.Height.Equals(parentRowPropertiesInfo.Height))
				rowProperties.Height.CopyFrom(rowPropertiesInfo.Height);
			if (!rowPropertiesInfo.WidthAfter.Equals(parentRowPropertiesInfo.WidthAfter))
				rowProperties.WidthAfter.CopyFrom(rowPropertiesInfo.WidthAfter);
			if (!rowPropertiesInfo.WidthBefore.Equals(parentRowPropertiesInfo.WidthBefore))
				rowProperties.WidthBefore.CopyFrom(rowPropertiesInfo.WidthBefore);
		}
		internal void ApplyTableCellProperties(TableCellProperties cellProperties, MergedTableCellProperties parentCellProperties) {
			cellProperties.ResetAllUse();
			CombinedCellPropertiesInfo parentCellPropertiesInfo = parentCellProperties.Info;
			CombinedCellPropertiesInfo cellPropertiesInfo = new CombinedCellPropertiesInfo(cellProperties).Clone();
			if (cellPropertiesInfo.Borders.TopBorder != parentCellPropertiesInfo.Borders.TopBorder)
				cellProperties.Borders.TopBorder.CopyFrom(cellPropertiesInfo.Borders.TopBorder);
			if (cellPropertiesInfo.Borders.RightBorder != parentCellPropertiesInfo.Borders.RightBorder)
				cellProperties.Borders.RightBorder.CopyFrom(cellPropertiesInfo.Borders.RightBorder);
			if (cellPropertiesInfo.Borders.LeftBorder != parentCellPropertiesInfo.Borders.LeftBorder)
				cellProperties.Borders.LeftBorder.CopyFrom(cellPropertiesInfo.Borders.LeftBorder);
			if (cellPropertiesInfo.Borders.BottomBorder != parentCellPropertiesInfo.Borders.BottomBorder)
				cellProperties.Borders.BottomBorder.CopyFrom(cellPropertiesInfo.Borders.BottomBorder);
			if (cellPropertiesInfo.Borders.InsideHorizontalBorder != parentCellPropertiesInfo.Borders.InsideHorizontalBorder)
				cellProperties.Borders.InsideHorizontalBorder.CopyFrom(cellPropertiesInfo.Borders.InsideHorizontalBorder);
			if (cellPropertiesInfo.Borders.InsideVerticalBorder != parentCellPropertiesInfo.Borders.InsideVerticalBorder)
				cellProperties.Borders.InsideVerticalBorder.CopyFrom(cellPropertiesInfo.Borders.InsideVerticalBorder);
			if (cellPropertiesInfo.Borders.TopLeftDiagonalBorder != parentCellPropertiesInfo.Borders.TopLeftDiagonalBorder)
				cellProperties.Borders.TopLeftDiagonalBorder.CopyFrom(cellPropertiesInfo.Borders.TopLeftDiagonalBorder);
			if (cellPropertiesInfo.Borders.TopRightDiagonalBorder != parentCellPropertiesInfo.Borders.TopRightDiagonalBorder)
				cellProperties.Borders.TopRightDiagonalBorder.CopyFrom(cellPropertiesInfo.Borders.TopRightDiagonalBorder);
			if (!cellPropertiesInfo.CellMargins.Top.Equals(parentCellPropertiesInfo.CellMargins.Top))
				cellProperties.CellMargins.Top.CopyFrom(cellPropertiesInfo.CellMargins.Top);
			if (!cellPropertiesInfo.CellMargins.Right.Equals(parentCellPropertiesInfo.CellMargins.Right))
				cellProperties.CellMargins.Right.CopyFrom(cellPropertiesInfo.CellMargins.Right);
			if (!cellPropertiesInfo.CellMargins.Left.Equals(parentCellPropertiesInfo.CellMargins.Left))
				cellProperties.CellMargins.Left.CopyFrom(cellPropertiesInfo.CellMargins.Left);
			if (!cellPropertiesInfo.CellMargins.Bottom.Equals(parentCellPropertiesInfo.CellMargins.Bottom))
				cellProperties.CellMargins.Bottom.CopyFrom(cellPropertiesInfo.CellMargins.Bottom);
			if (cellPropertiesInfo.GeneralSettings.BackgroundColor != parentCellPropertiesInfo.GeneralSettings.BackgroundColor)
				cellProperties.BackgroundColor = cellPropertiesInfo.GeneralSettings.BackgroundColor;
			if (cellPropertiesInfo.GeneralSettings.ForegroundColor != parentCellPropertiesInfo.GeneralSettings.ForegroundColor)
				cellProperties.ForegroundColor = cellPropertiesInfo.GeneralSettings.ForegroundColor;
			if (cellPropertiesInfo.GeneralSettings.ShadingPattern != parentCellPropertiesInfo.GeneralSettings.ShadingPattern)
				cellProperties.ShadingPattern = cellPropertiesInfo.GeneralSettings.ShadingPattern;
			if (cellPropertiesInfo.GeneralSettings.CellConditionalFormatting != parentCellPropertiesInfo.GeneralSettings.CellConditionalFormatting)
				cellProperties.CellConditionalFormatting = cellPropertiesInfo.GeneralSettings.CellConditionalFormatting;
			if (cellPropertiesInfo.GeneralSettings.ColumnSpan != parentCellPropertiesInfo.GeneralSettings.ColumnSpan)
				cellProperties.ColumnSpan = cellPropertiesInfo.GeneralSettings.ColumnSpan;
			if (cellPropertiesInfo.GeneralSettings.FitText != parentCellPropertiesInfo.GeneralSettings.FitText)
				cellProperties.FitText = cellPropertiesInfo.GeneralSettings.FitText;
			if (cellPropertiesInfo.GeneralSettings.HideCellMark != parentCellPropertiesInfo.GeneralSettings.HideCellMark)
				cellProperties.HideCellMark = cellPropertiesInfo.GeneralSettings.HideCellMark;
			if (cellPropertiesInfo.GeneralSettings.NoWrap != parentCellPropertiesInfo.GeneralSettings.NoWrap)
				cellProperties.NoWrap = cellPropertiesInfo.GeneralSettings.NoWrap;
			if (cellPropertiesInfo.GeneralSettings.TextDirection != parentCellPropertiesInfo.GeneralSettings.TextDirection)
				cellProperties.TextDirection = cellPropertiesInfo.GeneralSettings.TextDirection;
			if (cellPropertiesInfo.GeneralSettings.VerticalAlignment != parentCellPropertiesInfo.GeneralSettings.VerticalAlignment)
				cellProperties.VerticalAlignment = cellPropertiesInfo.GeneralSettings.VerticalAlignment;
			if (cellPropertiesInfo.GeneralSettings.VerticalMerging != parentCellPropertiesInfo.GeneralSettings.VerticalMerging)
				cellProperties.VerticalMerging = cellPropertiesInfo.GeneralSettings.VerticalMerging;
			if (!cellPropertiesInfo.PreferredWidth.Equals(parentCellPropertiesInfo.PreferredWidth))
				cellProperties.PreferredWidth.CopyFrom(cellPropertiesInfo.PreferredWidth);
		}
		#region ParseBinChar
		int binCharCount;
		public int BinCharCount {
			get { return binCharCount; }
			set {
				if (value <= 0)
					Exceptions.ThrowArgumentException("BinCharCount", value);
				binCharCount = value;
			}
		}
		protected virtual void ParseBinChar(char ch) {
#if DEBUGTEST
			Debug.Assert(stateManager.ParsingState == RtfParsingState.BinData);
#endif
			Destination.ProcessBinChar(ch);
			binCharCount--;
			if (binCharCount <= 0) {
				stateManager.ParsingState = RtfParsingState.Normal;
				DecreaseSkipCount();
			}
		}
		#endregion
		#region ParseHexChar
		protected virtual void ParseHexChar(Stream stream, char ch) {
#if DEBUGTEST
			Debug.Assert(stateManager.ParsingState == RtfParsingState.HexData);
#endif
			int hexValue = HexToInt(ch) << 4;
			hexValue += HexToInt(ReadChar(stream));
			stateManager.ParsingState = RtfParsingState.Normal;
			ParseChar((char)hexValue);
		}
		internal static int HexToInt(char ch) {
			if (Char.IsDigit(ch))
				return ch - '0';
			else {
				if (Char.IsLower(ch)) {
					if (ch < 'a' || ch > 'f')
						ThrowInvalidHexException();
					return 10 + ch - 'a';
				}
				else {
					if (ch < 'A' || ch > 'F')
						ThrowInvalidHexException();
					return 10 + ch - 'A';
				}
			}
		}
		#endregion
		internal void ParseUnicodeChar(char ch) {
#if DEBUGTEST
			Debug.Assert(skipCount == 0);
#endif
			FlushDecoder();
			Destination.ProcessChar(ch);
			skipCount = Position.RtfFormattingInfo.UnicodeCharacterByteCount;
		}
		internal void ParseCharWithoutDecoding(char ch) {
#if DEBUGTEST
			Debug.Assert(stateManager.ParsingState == RtfParsingState.Normal);
#endif
			FlushDecoder();
			if (skipCount == 0) {
				Destination.ProcessChar(ch);
			}
			DecreaseSkipCount();
		}
		protected internal virtual void FlushDecoder() {
			Position.RtfFormattingInfo.Decoder.Flush(this);
		}
		protected internal virtual void ParseChar(char ch) {
#if DEBUGTEST
			Debug.Assert(stateManager.ParsingState == RtfParsingState.Normal);
#endif
			if (skipCount == 0) {
				Position.RtfFormattingInfo.Decoder.ProcessChar(this, ch);
			}
			else
				DecreaseSkipCount();
		}
		protected virtual void PushRtfState() {
#if DEBUGTEST
			Debug.Assert(stateManager.ParsingState == RtfParsingState.Normal);
			Debug.Assert(skipCount == 0);
#endif
			stateManager.PushState();
		}
		protected virtual PopRtfStateResult PopRtfState() {
#if DEBUGTEST
			Debug.Assert(stateManager.ParsingState == RtfParsingState.Normal);
#endif
			DestinationBase oldDestination = Destination;
			if (stateManager.SavedStates.Count == OptionalGroupLevel)
				OptionalGroupLevel = int.MaxValue;
			stateManager.PopState();
			if (stateManager.SavedStates.Count >= OptionalGroupLevel)
				stateManager.Destination = new SkipDestination(this);
			oldDestination.AfterPopRtfState();
			skipCount = 0;
			return stateManager.SavedStates.Count == 0 ? PopRtfStateResult.StackEmpty : PopRtfStateResult.StackNonEmpty;
		}
		public static bool IsAlpha(char ch) {
			return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
		}
		public static bool IsDigit(char ch) {
			return (ch >= '0' && ch <= '9');
		}
		StringBuilder keyword = new StringBuilder(32, 32);
		StringBuilder parameterValueString = new StringBuilder();
		protected virtual char ParseRtfKeyword(Stream stream) {
#if DEBUGTEST
			Debug.Assert(stateManager.ParsingState == RtfParsingState.Normal);
#endif
			char ch = ReadChar(stream);
			if (!IsAlpha(ch)) {
				TranslateControlChar(ch);
				return ' ';
			}
			FlushDecoder();
			keyword.Length = 0;
			while (IsAlpha(ch)) {
				keyword.Append(ch);
				ch = ReadChar(stream);
			}
			parameterValueString.Length = 0;
			bool isNegative;
			if (ch == '-') {
				isNegative = true;
				parameterValueString.Append(ch);
				ch = ReadChar(stream);
			}
			else
				isNegative = false;
			if (isNegative && !IsDigit(ch)) {
				parameterValueString.Length = 0;
			}
			else {
				while (IsDigit(ch)) {
					parameterValueString.Append(ch);
					ch = ReadChar(stream);
				}
			}
			bool hasParameter = (parameterValueString.Length != 0);
			int parameterValue = hasParameter ? StringToInt(parameterValueString.ToString()) : 0;
			TranslateKeyword(keyword.ToString(), parameterValue, hasParameter);
			return ch;
		}
		protected internal virtual int StringToInt(string value) {
			int result;
			if (Int32.TryParse(value, out result))
				return result;
			else
				return 0;
		}
		protected virtual void ParseCR() {
#if DEBUGTEST
			Debug.Assert(stateManager.ParsingState == RtfParsingState.Normal);
#endif
		}
		protected virtual void ParseLF() {
#if DEBUGTEST
			Debug.Assert(stateManager.ParsingState == RtfParsingState.Normal);
#endif
		}
		internal void DecreaseSkipCount() {
			skipCount = Math.Max(0, skipCount - 1);
		}
		protected virtual void TranslateControlChar(char ch) {
			if (skipCount == 0 || ch == '\'')
				Destination.ProcessControlChar(ch);
			else
				DecreaseSkipCount();
		}
		protected virtual void TranslateKeyword(string keyword, int parameterValue, bool hasParameter) {
			if (skipCount == 0 || keyword == "bin") {
				bool keywordProcessed = Destination.ProcessKeyword(keyword, parameterValue, hasParameter);
				if (keywordProcessed) {
					if (!(stateManager.Destination is SkipDestination))
						OptionalGroupLevel = int.MaxValue;
				}
				else {
					if (OptionalGroupLevel < int.MaxValue)
						stateManager.Destination = new SkipDestination(this);
				}
			}
			else
				DecreaseSkipCount();
		}
		static char ReadChar(Stream stream) {
			int intChar = stream.ReadByte();
			if (intChar < 0)
				ThrowUnexpectedEndOfFile();
			return (char)intChar;
		}
		static void ThrowInvalidHexException() {
			throw new ArgumentException("Invalid hex value");
		}
		static void ThrowUnexpectedEndOfFile() {
			throw new ArgumentException("Unexpected end of file");
		}
		internal static void ThrowInvalidRtfFile() {
			throw new ArgumentException("Invalid RTF file");
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (stateManager != null) {
					stateManager.Dispose();
					stateManager = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		internal void InsertParagraph() {
			if (Position.RtfFormattingInfo.Deleted && Options.IgnoreDeletedText)
				return;
			ParagraphIndex insertedParagraphIndex = Position.ParagraphIndex;
			Position.ParagraphFormattingInfo.LineSpacing = Position.ParagraphFormattingInfo.CalcLineSpacing(UnitConverter);
			Position.ParagraphFormattingInfo.LineSpacingType = Position.ParagraphFormattingInfo.CalcLineSpacingType();
			if (!Position.UseLowAnsiCharactersFontName)
				PieceTable.InsertParagraphCore(Position);
			else {
				string oldFontName = Position.CharacterFormatting.FontName;
				Position.CharacterFormatting.FontName = Position.LowAnsiCharactersFontName;
				PieceTable.InsertParagraphCore(Position);
				Position.CharacterFormatting.FontName = oldFontName;
			}
			ApplyParagraphFormatting(insertedParagraphIndex, false);
			ApplyFrameProperties(insertedParagraphIndex);
		}
		internal void ApplyFrameProperties(ParagraphIndex paragraphIndex) {
			if (!DocumentModel.DocumentCapabilities.ParagraphFramesAllowed)
				return;
			Paragraph paragraph = PieceTable.Paragraphs[paragraphIndex];
			ParagraphFrameFormattingInfo paragraphFrameFormatting = Position.ParagraphFrameFormattingInfo;
			if (paragraphFrameFormatting != null)
				ApplyParagraphFrameFormatting(paragraph, paragraphFrameFormatting);
		}
		internal void ApplyFormattingToLastParagraph() {
			Position.ParagraphFormattingInfo.LineSpacing = Position.ParagraphFormattingInfo.CalcLineSpacing(UnitConverter);
			Position.ParagraphFormattingInfo.LineSpacingType = Position.ParagraphFormattingInfo.CalcLineSpacingType();
			Paragraph paragraph = PieceTable.Paragraphs.Last;
			ApplyParagraphFormatting(paragraph.Index, false);
			PieceTable.InheritParagraphRunStyleCore(Position, (ParagraphRun)PieceTable.Runs[paragraph.LastRunIndex]);
		}
		internal void InsertSection() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				ParagraphIndex paragraphIndex = Position.ParagraphIndex;
				Position.ParagraphFormattingInfo.LineSpacing = Position.ParagraphFormattingInfo.CalcLineSpacing(UnitConverter);
				Position.ParagraphFormattingInfo.LineSpacingType = Position.ParagraphFormattingInfo.CalcLineSpacingType();
				PieceTable.InsertSectionParagraphCore(Position);
				ApplyParagraphFormatting(paragraphIndex, true);
				DocumentModel.SafeEditor.PerformInsertSectionCore(paragraphIndex); 
			}
		}
		protected internal virtual void LinkParagraphStylesWithNumberingLists() {
			ParagraphStyleCollection styles = DocumentModel.ParagraphStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++)
				LinkParagraphStyleWithNumberingLists(styles[i]);
		}
		protected internal virtual void LinkParagraphStyleWithNumberingLists(ParagraphStyle style) {
			RtfNumberingListInfo info;
			if (!ParagraphStyleListOverrideIndexMap.TryGetValue(style, out info))
				return;
			NumberingListIndex index;
			if (!ListOverrideIndexToNumberingListIndexMap.TryGetValue(info.RtfNumberingListIndex, out index))
				return;
			if (index < new NumberingListIndex(0) || index >= new NumberingListIndex(DocumentModel.NumberingLists.Count))
				return;
			style.SetNumberingListIndex(index);
			style.SetNumberingListLevelIndex(info.ListLevelIndex);
		}
		protected internal virtual string GetUserNameById(int id) {
			int index = id - 1;
			if (index < 0 || index + 1 > DocumentProperties.UserNames.Count)
				return String.Empty;
			else
				return DocumentProperties.UserNames[index];
		}
		#region ThrowInvalidFile
		public override void ThrowInvalidFile() {
			throw new Exception("Invalid rtf file");
		}
		#endregion
	}
	#endregion
	#region RtfParsingState
	public enum RtfParsingState {
		Normal,
		BinData,
		HexData
	}
	#endregion
	#region RtfParserStateManager
	public class RtfParserStateManager : IDisposable {
		internal class StateItem {
			readonly RtfInputPositionState inputPositionState;
			readonly DestinationBase destination;
			public StateItem(RtfInputPositionState inputPositionState, DestinationBase destination) {
				this.inputPositionState = inputPositionState;
				this.destination = destination;
			}
			public RtfInputPositionState InputPositionState { get { return inputPositionState; } }
			public DestinationBase Destination { get { return destination; } }
		}
		#region Fields
		readonly Stack<StateItem> savedStates;
		readonly Stack<RtfPieceTableInfo> pieceTables;
		readonly RtfImporter rtfImporter;
		RtfParsingState parsingState = RtfParsingState.Normal;
		DestinationBase destination;
		#endregion
		public RtfParserStateManager(RtfImporter rtfImporter) {
			Guard.ArgumentNotNull(rtfImporter, "rtfImporter");
			this.rtfImporter = rtfImporter;
			this.savedStates = new Stack<StateItem>();
			this.pieceTables = new Stack<RtfPieceTableInfo>();
		}
		protected internal virtual void Initialize() {
			this.pieceTables.Push(new RtfPieceTableInfo(Importer, Importer.DocumentModel.MainPieceTable));
			this.destination = CreateDefaultDestination();
			this.savedStates.Push(new StateItem(null, destination));
		}
		#region Properties
		internal Stack<StateItem> SavedStates { get { return savedStates; } }
		internal Stack<RtfPieceTableInfo> PieceTables { get { return pieceTables; } }
		protected RtfImporter Importer { get { return rtfImporter; } }
		public RtfParsingState ParsingState { get { return parsingState; } set { parsingState = value; } }
		public DestinationBase Destination {
			get { return destination; }
			set {
				SetDestinationCore(value, true);
			}
		}
		public RtfPieceTableInfo PieceTableInfo {
			get { return pieceTables.Peek(); }
		}
		#endregion
		protected internal virtual DestinationBase CreateDefaultDestination() {
			return new DefaultDestination(Importer, Importer.PieceTable);
		}
		protected internal virtual void SetDestinationCore(DestinationBase value, bool updatePieceTableInfo) {
			Guard.ArgumentNotNull(value, "Destination");
			if (!Object.ReferenceEquals(value.PieceTable, destination.PieceTable) && updatePieceTableInfo) {
				if (pieceTables.Count > 0) {
					pieceTables.Pop();
					pieceTables.Push(new RtfPieceTableInfo(Importer, value.PieceTable));
				}
			}
			destination = value;
		}
		public void PushState() {
			savedStates.Push(new StateItem(Importer.Position.GetState(), Destination));
			pieceTables.Push(PieceTableInfo);
			Destination = Destination.Clone();
			Destination.IncreaseGroupLevel();
		}
		public void PopState() {
			DestinationBase nestedDestination = Destination;
			StateItem state = savedStates.Peek();
			DestinationBase newDestination = state.Destination;
			newDestination.BeforeNestedGroupFinished(nestedDestination);
			destination.BeforePopRtfState();
			savedStates.Pop();
			bool samePieceTable = Object.ReferenceEquals(Destination.PieceTable, newDestination.PieceTable);
			RtfPieceTableInfo pieceTableInfo = pieceTables.Pop();
			SetDestinationCore(newDestination, false);
			pieceTables.Push(pieceTableInfo);
			Destination.NestedGroupFinished(nestedDestination);
			if (state.InputPositionState != null && samePieceTable)
				Importer.Position.SetState(state.InputPositionState);
			pieceTables.Pop();
			Destination.AfterNestedGroupFinished(nestedDestination);
			if (pieceTables.Count <= 0)
				pieceTables.Push(pieceTableInfo);
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (destination != null) {
					destination.Dispose();
					destination = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
}
