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
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.API.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Services;
using DevExpress.XtraRichEdit.Export;
using DevExpress.Office;
using DocumentModel = DevExpress.XtraRichEdit.Model.DocumentModel;
using MailMergeHelper = DevExpress.XtraRichEdit.Model.MailMergeHelper;
using SearchOptionsInternal = DevExpress.XtraRichEdit.Model.SearchOptions;
using TextSearchBase = DevExpress.XtraRichEdit.Model.ISearchStrategy;
using TextSearchByStringForward = DevExpress.XtraRichEdit.Model.TextSearchByStringForwardStrategy;
using TextSearchByStringBackward = DevExpress.XtraRichEdit.Model.TextSearchByStringBackwardStrategy;
using TextSearchByRegexForward = DevExpress.XtraRichEdit.Model.TextSearchByRegexForwardStrategy;
using TextSearchByRegexBackward = DevExpress.XtraRichEdit.Model.TextSearchByRegexBackwardStrategy;
using IVisibleTextFilter = DevExpress.XtraRichEdit.Model.IVisibleTextFilter;
using TableAutoFitBehaviorType = DevExpress.XtraRichEdit.Model.TableAutoFitBehaviorType;
using DocumentProperties = DevExpress.XtraRichEdit.Model.DocumentProperties;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraRichEdit.API.Native.Implementation;
#if !SL
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.API.Native {
	#region InsertOptions
	[ComVisible(true)]
	public enum InsertOptions {
		KeepSourceFormatting,
		MatchDestinationFormatting
	}
	#endregion
	#region SubDocument
	[ComVisible(true)]
	public interface SubDocument {
		DocumentRange Range { get; }
		int Length { get; }
		Color PageBackColor { get; }
		bool ShowPageBackground { get; }
		ParagraphCollection Paragraphs { get; }
		FieldCollection Fields { get; }
		BookmarkCollection Bookmarks { get; }
		HyperlinkCollection Hyperlinks { get; }
		CustomMarkCollection CustomMarks { get; }
		TableCollection Tables { get; }
		ShapeCollection Shapes { get; }
		DocumentImageCollection Images { get; }
		CommentCollection Comments { get; }
		DocumentRange InsertDocumentContent(DocumentPosition pos, string fileName, DocumentFormat format);
		DocumentRange InsertDocumentContent(DocumentPosition pos, string fileName, DocumentFormat format, InsertOptions insertOptions);
		DocumentRange InsertDocumentContent(DocumentPosition pos, string fileName, DocumentFormat format, string sourceUri);
		DocumentRange InsertDocumentContent(DocumentPosition pos, string fileName, DocumentFormat format, string sourceUri, InsertOptions insertOptions);
		DocumentRange InsertDocumentContent(DocumentPosition pos, Stream stream, DocumentFormat format);
		DocumentRange InsertDocumentContent(DocumentPosition pos, Stream stream, DocumentFormat format, InsertOptions insertOptions);
		DocumentRange InsertDocumentContent(DocumentPosition pos, Stream stream, DocumentFormat format, string sourceUri);
		DocumentRange InsertDocumentContent(DocumentPosition pos, Stream stream, DocumentFormat format, string sourceUri, InsertOptions insertOptions);
		DocumentRange InsertDocumentContent(DocumentPosition pos, DocumentRange range);
		DocumentRange InsertDocumentContent(DocumentPosition pos, DocumentRange range, InsertOptions insertOptions);
		DocumentRange AppendDocumentContent(string fileName, DocumentFormat format);
		DocumentRange AppendDocumentContent(string fileName, DocumentFormat format, string sourceUri);
		DocumentRange AppendDocumentContent(string fileName, DocumentFormat format, string sourceUri, InsertOptions insertOptions);
		DocumentRange AppendDocumentContent(Stream stream, DocumentFormat format);
		DocumentRange AppendDocumentContent(Stream stream, DocumentFormat format, string sourceUri);
		DocumentRange AppendDocumentContent(Stream stream, DocumentFormat format, string sourceUri, InsertOptions insertOptions);
		DocumentRange AppendDocumentContent(DocumentRange range);
		DocumentRange AppendDocumentContent(DocumentRange range, InsertOptions insertOptions);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ParagraphCollection.Insert(DocumentPosition pos)' method instead.")]
		Paragraph InsertParagraph(DocumentPosition pos);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ParagraphCollection.Insert(DocumentPosition pos, InsertOptions insertOptions)' method instead.")]
		Paragraph InsertParagraph(DocumentPosition pos, InsertOptions insertOptions);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ParagraphCollection.Append()' method instead.")]
		Paragraph AppendParagraph();
		DocumentRange InsertText(DocumentPosition pos, string text);
		DocumentRange AppendText(string text);
		DocumentRange InsertSingleLineText(DocumentPosition pos, string text);
		DocumentRange AppendSingleLineText(string text);
		DocumentRange InsertRtfText(DocumentPosition pos, string rtfText);
		DocumentRange InsertRtfText(DocumentPosition pos, string rtfText, InsertOptions insertOptions);
		DocumentRange AppendRtfText(string rtfText);
		DocumentRange AppendRtfText(string rtfText, InsertOptions insertOptions);
		DocumentRange InsertHtmlText(DocumentPosition pos, string htmlText);
		DocumentRange InsertHtmlText(DocumentPosition pos, string htmlText, InsertOptions insertOptions);
		DocumentRange AppendHtmlText(string htmlText);
		DocumentRange AppendHtmlText(string htmlText, InsertOptions insertOptions);
#if !DXPORTABLE
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.DocumentImageCollection.Insert(DocumentPosition pos, DocumentImageSource imageSource)' method instead.")]
		DocumentImage InsertImage(DocumentPosition pos, DocumentImageSource imageSource);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.DocumentImageCollection.Append(DocumentImageSource imageSource)' method instead.")]
		DocumentImage AppendImage(DocumentImageSource imageSource);
#if !SL
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.DocumentImageCollection.Insert(DocumentPosition pos, System.Drawing.Image image)' method instead.")]
		DocumentImage InsertImage(DocumentPosition pos, System.Drawing.Image image);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.DocumentImageCollection.Append(System.Drawing.Image image)' method instead.")]
		DocumentImage AppendImage(System.Drawing.Image image);
#endif
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ShapeCollection.InsertPicture(DocumentPosition pos, DocumentImageSource imageSource)' method instead.")]
		Shape InsertPicture(DocumentPosition pos, DocumentImageSource imageSource);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ShapeCollection.InsertTextBox(DocumentPosition pos)' method instead.")]
		Shape InsertTextBox(DocumentPosition pos);
#if !SL
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ShapeCollection.InsertPicture(DocumentPosition pos, System.Drawing.Image image)' method instead.")]
		Shape InsertPicture(DocumentPosition pos, System.Drawing.Image image);
#endif
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.TableCollection.Create(DocumentPosition pos, int rowCount, int columnCount)' method instead.")]
		Table InsertTable(DocumentPosition pos, int rowCount, int columnCount);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.TableCollection.Create(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehavior)' method instead.")]
		Table InsertTable(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehavior);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.TableCollection.Create(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehavior, int fixedColumnWidths)' method instead.")]
		Table InsertTable(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehavior, int fixedColumnWidths);
#endif
		DocumentPosition CreatePosition(int start);
		DocumentRange CreateRange(int start, int length);
		DocumentRange CreateRange(DocumentPosition start, int length);
		void Delete(DocumentRange range);
		void SelectAll();
#if !DXPORTABLE
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ReadOnlyParagraphCollection.Get(DocumentPosition pos)' method instead.")]
		Paragraph GetParagraph(DocumentPosition pos);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ReadOnlyParagraphCollection.Get(DocumentRange range)' method instead.")]
		ParagraphCollection GetParagraphs(DocumentRange range);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ReadOnlyDocumentImageCollection.Get(DocumentRange range)' method instead.")]
		DocumentImageCollection GetImages(DocumentRange range);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ReadOnlyDocumentShapeCollection.Get(DocumentRange range)' method instead.")]
		ShapeCollection GetShapes(DocumentRange range);
#endif
		ParagraphProperties BeginUpdateParagraphs(DocumentRange range);
		void EndUpdateParagraphs(ParagraphProperties properties);
		CharacterProperties BeginUpdateCharacters(int start, int length);
		CharacterProperties BeginUpdateCharacters(DocumentPosition start, int length);
		CharacterProperties BeginUpdateCharacters(DocumentRange range);
		void EndUpdateCharacters(CharacterProperties properties);
		void ApplySyntaxHighlight(List<SyntaxHighlightToken> tokens);
		RangePermissionCollection BeginUpdateRangePermissions();
		void EndUpdateRangePermissions(RangePermissionCollection permissions);
		void CancelUpdateRangePermissions(RangePermissionCollection permissions);
		string GetText(DocumentRange range);
		string GetText(DocumentRange range, PlainTextDocumentExporterOptions options);
		string GetText(DocumentRange range, TextFragmentOptions getTextOptions);
		string GetText(DocumentRange range, PlainTextDocumentExporterOptions options, TextFragmentOptions getTextOptions);
		string GetRtfText(DocumentRange range);
		string GetRtfText(DocumentRange range, RtfDocumentExporterOptions options);
		string GetHtmlText(DocumentRange range, IUriProvider provider);
		string GetHtmlText(DocumentRange range, IUriProvider provider, HtmlDocumentExporterOptions options);
		string GetMhtText(DocumentRange range);
		string GetMhtText(DocumentRange range, MhtDocumentExporterOptions options);
		string GetWordMLText(DocumentRange range);
		string GetWordMLText(DocumentRange range, WordMLDocumentExporterOptions options);
		byte[] GetOpenXmlBytes(DocumentRange range);
		byte[] GetOpenXmlBytes(DocumentRange range, OpenXmlDocumentExporterOptions options);
#if !DXPORTABLE
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.BookmarkCollection.Create(DocumentPosition start, int length, string name)' method instead.")]
		Bookmark CreateBookmark(DocumentPosition start, int length, string name);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.BookmarkCollection.Create(DocumentRange range, string name)' method instead.")]
		Bookmark CreateBookmark(DocumentRange range, string name);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.CustomMarkCollection.Create(DocumentPosition position, object userData)' method instead.")]
		CustomMark CreateCustomMark(DocumentPosition position, object userData);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.CustomMarkCollection.Remove(CustomMark customMark)' method instead.")]
		void DeleteCustomMark(CustomMark customMark);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.CustomMarkCollection.GetByVisualInfo(DevExpress.XtraRichEdit.Layout.Export.CustomMarkVisualInfo customMarkVisualInfo)' method instead.")]
		CustomMark GetCustomMarkByVisualInfo(DevExpress.XtraRichEdit.Layout.Export.CustomMarkVisualInfo customMarkVisualInfo);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.BookmarkCollection.Select(Bookmark bookmark)' method instead.")]
		void SelectBookmark(Bookmark bookmark);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.BookmarkCollection.Remove(Bookmark bookmark)' method instead.")]
		void RemoveBookmark(Bookmark bookmark);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.HyperlinkCollection.Create(DocumentPosition start, int length)' method instead.")]
		Hyperlink CreateHyperlink(DocumentPosition start, int length);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.HyperlinkCollection.Create(DocumentRange range)' method instead.")]
		Hyperlink CreateHyperlink(DocumentRange range);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.HyperlinkCollection.Remove(Hyperlink hyperlink)' method instead.")]
		void RemoveHyperlink(Hyperlink hyperlink);
#endif
		void BeginUpdate();
		void EndUpdate();
		void Replace(DocumentRange range, string text);
		DocumentRange[] FindAll(string textToFind, SearchOptions options, DocumentRange range);
		DocumentRange[] FindAll(string textToFind, SearchOptions options);
		DocumentRange[] FindAll(System.Text.RegularExpressions.Regex regex, DocumentRange range);
		DocumentRange[] FindAll(System.Text.RegularExpressions.Regex regex);
		int ReplaceAll(string textToFind, string replaceWith, SearchOptions options, DocumentRange range);
		int ReplaceAll(string textToFind, string replaceWith, SearchOptions options);
		int ReplaceAll(System.Text.RegularExpressions.Regex regex, string replaceWith, DocumentRange range);
		int ReplaceAll(System.Text.RegularExpressions.Regex regex, string replaceWith);
		ISearchResult StartSearch(string textToFind, SearchOptions options, SearchDirection direction, DocumentRange range);
		ISearchResult StartSearch(string textToFind, SearchOptions options, SearchDirection direction);
		ISearchResult StartSearch(string textToFind);
		IRegexSearchResult StartSearch(System.Text.RegularExpressions.Regex regex, DocumentRange range);
		IRegexSearchResult StartSearch(System.Text.RegularExpressions.Regex regex);
#if !DXPORTABLE
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.TableCollection.GetTableCell(DocumentPosition pos)' method instead.")]
		TableCell GetTableCell(DocumentPosition pos);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ParagraphCollection.AddParagraphsToList(DocumentRange range, NumberingList list, int levelIndex)' method instead.")]
		void AddParagraphsToList(DocumentRange range, NumberingList list, int levelIndex);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ParagraphCollection.AddParagraphToList(Paragraph paragraph, NumberingList list, int levelIndex)' method instead.")]
		void AddParagraphToList(Paragraph paragraph, NumberingList list, int levelIndex);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ParagraphCollection.AddParagraphToList(Paragraph paragraph, int numberingListIndex, int levelIndex)' method instead.")]
		void AddParagraphToList(Paragraph paragraph, int numberingListIndex, int levelIndex);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ParagraphCollection.RemoveNumberingFromParagraph(Paragraph paragraph)' method instead.")]
		void RemoveNumberingFromParagraph(Paragraph paragraph);
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.ParagraphCollection.RemoveNumberingFromParagraphs(DocumentRange range)' method instead.")]
		void RemoveNumberingFromParagraphs(DocumentRange range);
#endif
		void SetPageBackground(Color pageBackColor, bool showPageBackground);
		void SetPageBackground(Color pageBackColor);
		void SetPageBackground(bool showPageBackground);
		SubDocumentType GetSubDocumentType();
	}
	#endregion
	#region Document
	[ComVisible(true)]
	public interface Document : SubDocument {
		DocumentUnit Unit { get; set; }
		SectionCollection Sections { get; }
		CharacterStyleCollection CharacterStyles { get; }
		ParagraphStyleCollection ParagraphStyles { get; }
		TableStyleCollection TableStyles { get; }
		CharacterPropertiesBase DefaultCharacterProperties { get; }
		ParagraphPropertiesBase DefaultParagraphProperties { get; }
		TablePropertiesBase DefaultTableProperties { get; }
		DocumentVariableCollection Variables { get; }
		NumberingListCollection NumberingLists { get; }
		AbstractNumberingListCollection AbstractNumberingLists { get; }
		string RtfText { get; set; }
		string HtmlText { get; set; }
		string MhtText { get; set; }
		string WordMLText { get; set; }
		string Text { get; set; }
		byte[] OpenXmlBytes { get; set; }
		byte[] OpenDocumentBytes { get; set; }
		bool IsEmpty { get; }
		bool DifferentOddAndEvenPages { get; set; }
		UpdateDocVariablesBeforePrint UpdateDocVariablesBeforePrint { get; set; }
		event EventHandler ContentChanged;
		event EventHandler ModifiedChanged;
		event CalculateDocumentVariableEventHandler CalculateDocumentVariable;
		void LoadDocument(string fileName, DocumentFormat format);
		void LoadDocument(string fileName, DocumentFormat format, string sourceUri);
		void LoadDocument(Stream stream, DocumentFormat format);
		void LoadDocument(Stream stream, DocumentFormat format, string sourceUri);
		void SaveDocument(string fileName, DocumentFormat format);
		void SaveDocument(Stream stream, DocumentFormat format);
		Section InsertSection(DocumentPosition pos);
		Section AppendSection();
		Section GetSection(DocumentPosition pos);
		DocumentPosition CaretPosition { get; set; }
		DocumentRange Selection { get; set; }
		SelectionCollection Selections { get; }
		bool IsDocumentProtected { get; }
		void Copy();
		void Copy(DocumentRange range);
		void Cut();
		void Cut(DocumentRange range);
		void Paste();
		void Paste(DocumentFormat format);
		DocumentExportCapabilities RequiredExportCapabilities { get; }
		MailMergeOptions CreateMailMergeOptions();
		void MailMerge(string fileName, DocumentFormat format);
		void MailMerge(Stream stream, DocumentFormat format);
		void MailMerge(Document targetDocument);
		void MailMerge(MailMergeOptions options, string fileName, DocumentFormat format);
		void MailMerge(MailMergeOptions options, Stream stream, DocumentFormat format);
		void MailMerge(MailMergeOptions options, Document targetDocument);
		void Protect(string password);
		void Unprotect();
		void ChangeActiveDocument(SubDocument document);
		bool MakeAllTablesWordCompatible();
		List<string> GetAuthors();
	}
	#endregion
	#region DocumentHelper
	public static class DocumentHelper {
		public static int GetParagraphStart(Paragraph paragraph) {
			return ((DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraph)paragraph).InnerParagraph.LogPosition - DevExpress.XtraRichEdit.Model.DocumentLogPosition.Zero;
		}
		public static int GetParagraphLength(Paragraph paragraph) {
			return ((DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraph)paragraph).InnerParagraph.Length;
		}
	}
	#endregion
	#region ISearchResult
	[ComVisible(true)]
	public interface ISearchResult {
		DocumentRange CurrentResult { get; }
		void Reset();
		bool FindNext();
		void Replace(string replaceWith);
	}
	#endregion
	#region SearchOptions
	[ComVisible(true)]
	[Flags]
	public enum SearchOptions {
		None = DevExpress.XtraRichEdit.Model.SearchOptions.None,
		CaseSensitive = DevExpress.XtraRichEdit.Model.SearchOptions.CaseSensitive,
		WholeWord = DevExpress.XtraRichEdit.Model.SearchOptions.WholeWord,
	}
	#endregion
	#region SearchDirection
	[ComVisible(true)]
	public enum SearchDirection {
		Forward = DevExpress.XtraRichEdit.Model.Direction.Forward,
		Backward = DevExpress.XtraRichEdit.Model.Direction.Backward
	}
	#endregion
	#region SubDocumentType
	public enum SubDocumentType {
		Main,
		Header,
		Footer,
		FootNote,
		EndNote,
		TextBox,
		Comment
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
	using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	using ModelPositionAnchor = DevExpress.XtraRichEdit.Model.DocumentModelPositionAnchor;
	using ModelParagraph = DevExpress.XtraRichEdit.Model.Paragraph;
	using ModelParagraphIndex = DevExpress.XtraRichEdit.Model.ParagraphIndex;
	using ModelParagraphCollection = DevExpress.Office.Utils.List<DevExpress.XtraRichEdit.Model.Paragraph, DevExpress.XtraRichEdit.Model.ParagraphIndex>;
	using ModelSection = DevExpress.XtraRichEdit.Model.Section;
	using ModelSectionIndex = DevExpress.XtraRichEdit.Model.SectionIndex;
	using ModelRunInfo = DevExpress.XtraRichEdit.Model.RunInfo;
	using ModelRunIndex = DevExpress.XtraRichEdit.Model.RunIndex;
	using ModelSelection = DevExpress.XtraRichEdit.Model.Selection;
	using ModelField = DevExpress.XtraRichEdit.Model.Field;
	using ModelBookmark = DevExpress.XtraRichEdit.Model.Bookmark;
	using ModelHyperlinkInfo = DevExpress.XtraRichEdit.Model.HyperlinkInfo;
	using ModelBookmarkCollection = DevExpress.XtraRichEdit.Model.BookmarkCollection;
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using ModelCustomMark = DevExpress.XtraRichEdit.Model.CustomMark;
	using ModelTable = DevExpress.XtraRichEdit.Model.Table;
	using ModelTableCell = DevExpress.XtraRichEdit.Model.TableCell;
	using ModelWidthUnit = DevExpress.XtraRichEdit.Model.WidthUnit;
	using ModelWidthUnitType = DevExpress.XtraRichEdit.Model.WidthUnitType;
	using ModelInlinePictureRun = DevExpress.XtraRichEdit.Model.InlinePictureRun;
	using ModelFloatingObjectAnchorRun = DevExpress.XtraRichEdit.Model.FloatingObjectAnchorRun;
	using ModelComment = DevExpress.XtraRichEdit.Model.Comment;
	using ModelCommentCollection = DevExpress.XtraRichEdit.Model.CommentCollection;
	using DevExpress.XtraRichEdit.Commands;
	using DevExpress.XtraRichEdit.Commands.Internal;
	using DevExpress.XtraRichEdit.Model.History;
	using DevExpress.XtraRichEdit.Localization;
	using System.Text.RegularExpressions;
	using DevExpress.Utils.Commands;
	using System.ComponentModel;
	using DevExpress.Office.Utils;
	using DevExpress.Office.Services;
	using DevExpress.Office.API.Internal;
	using System.Collections.ObjectModel;
	#region NativeSubDocument
	public class NativeSubDocument : SubDocument {
		#region Fields
		int referenceCount = 1;
		UnitConverter unitConverter;
		readonly ModelPieceTable pieceTable;
		readonly InnerRichEditDocumentServer server;
		NativeParagraphCollection paragraphs;
		NativeFieldCollection fields;
		NativeBookmarkCollection bookmarks;
		NativeHyperlinkCollection hyperlinks;
		NativeCustomMarkCollection customMarks;
		NativeTableCollection tables;
		NativeShapeCollection shapes;
		NativeDocumentImageCollection images;
		NativeCommentCollection comments;
		#endregion
		internal NativeSubDocument(ModelPieceTable pieceTable, InnerRichEditDocumentServer server) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(server, "server");
			this.pieceTable = pieceTable;
			this.server = server;
			CreateApiObjects();
			Initialize();
			SubscribeInternalAPIEvents();
			OnUnitsChanged();
		}
		#region Properties
		protected internal InnerRichEditDocumentServer DocumentServer { get { return server; } }
		protected internal DevExpress.XtraRichEdit.Model.PieceTable PieceTable { get { return pieceTable; } }
		protected internal InternalAPI InternalAPI { get { return DocumentModel.InternalAPI; } }
		protected internal DevExpress.XtraRichEdit.Model.DocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		protected internal UnitConverter UnitConverter { get { return unitConverter; } }
		protected internal virtual NativeDocument MainDocument { get { return server.NativeDocument; } }
		protected internal virtual TableCellStyle GetTableCellStyle(DevExpress.XtraRichEdit.Model.TableCell cell) {
			return null;
		}
		protected internal virtual DevExpress.XtraRichEdit.Model.TableCellStyle GetInnerTableCellStyle(TableCellStyle style) {
			return null;
		}
		public DocumentUnit Unit {
			get {
				CheckValid();
				return DocumentServer.Unit;
			}
			set {
				CheckValid();
				DocumentServer.Unit = value;
			}
		}
		public ParagraphCollection Paragraphs {
			get {
				CheckValid();
				return paragraphs;
			}
		}
		public FieldCollection Fields {
			get {
				CheckValid();
				return fields;
			}
		}
		public BookmarkCollection Bookmarks {
			get {
				CheckValid();
				return bookmarks;
			}
		}
		public CustomMarkCollection CustomMarks {
			get {
				CheckValid();
				return customMarks;
			}
		}
		public HyperlinkCollection Hyperlinks {
			get {
				CheckValid();
				return hyperlinks;
			}
		}
		public TableCollection Tables {
			get {
				CheckValid();
				return tables;
			}
		}
		public ShapeCollection Shapes {
			get {
				CheckValid();
				return shapes;
			}
		}
		public DocumentImageCollection Images {
			get {
				CheckValid();
				return images;
			}
		}
		public CommentCollection Comments {
			get {
				CheckValid();
				return comments;
			}
		}
		public DocumentRange Range {
			get {
				CheckValid();
				DevExpress.XtraRichEdit.Model.PieceTable pieceTable = PieceTable;
				DevExpress.XtraRichEdit.Model.ParagraphCollection paragraphs = pieceTable.Paragraphs;
				return CreateNativeRange(ModelPosition.FromParagraphStart(pieceTable, paragraphs.First.Index), ModelPosition.FromParagraphEnd(pieceTable, paragraphs.Last.Index));
			}
		}
		public int Length {
			get {
				CheckValid();
				ModelParagraph lastParagraph = pieceTable.Paragraphs.Last;
				return lastParagraph.LogPosition - ModelLogPosition.Zero + lastParagraph.Length;
			}
		}
		protected internal DocumentPosition EndPosition {
			get {
				DevExpress.XtraRichEdit.Model.PieceTable pieceTable = PieceTable;
				DevExpress.XtraRichEdit.Model.ParagraphCollection paragraphs = pieceTable.Paragraphs;
				return new NativeDocumentPosition(this, ModelPosition.FromParagraphEnd(pieceTable, paragraphs.Last.Index));
			}
		}
		public virtual int ReferenceCount {
			get { return referenceCount; }
			set {
				if (ReferenceCount == value)
					return;
				int oldValue = ReferenceCount;
				referenceCount = value;
				OnReferenceCountChanged(oldValue, value);
			}
		}
		public Color PageBackColor {
			get {
				CheckValid();
				return DocumentModel.DocumentProperties.PageBackColor;
			}
		}
		public bool ShowPageBackground {
			get {
				CheckValid();
				return DocumentModel.DocumentProperties.DisplayBackgroundShape;
			}
		}
		#endregion
		void OnReferenceCountChanged(int oldValue, int newValue) {
			if (newValue <= 0)
				Invalidate();
			else if (oldValue <= 0 && newValue > 0) {
				Initialize();
				SubscribeInternalAPIEvents();
			}
		}
		protected internal void CheckValid() {
			if (ReferenceCount <= 0)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseInvalidDocument);
		}
		protected internal virtual void CreateApiObjects() {
			this.paragraphs = new NativeParagraphCollection(this);
			this.fields = new NativeFieldCollection(this);
			this.hyperlinks = new NativeHyperlinkCollection(this);
			this.customMarks = new NativeCustomMarkCollection(this);
			this.tables = new NativeTableCollection(this);
			this.bookmarks = new NativeBookmarkCollection(this);
			this.shapes = new NativeShapeCollection(this);
			this.images = new NativeDocumentImageCollection(this);
			this.comments = new NativeCommentCollection(this, false);
		}
		protected internal virtual void Initialize() {
			PopulateParagraphs();
			PopulateFields();
			PopulateBookmarks();
			PopulateHyperlinks();
			PopulateCustomMarks();
			PopulateTables();
			PopulateComments();
		}
		protected internal virtual void Invalidate() {
			UnsubscribeInternalAPIEvents();
			paragraphs.Clear();
			fields.Clear();
			hyperlinks.Clear();
			customMarks.Clear();
			bookmarks.Invalidate();
			tables.Clear();
			shapes.InnerList.Clear();
			images.InnerList.Clear();
			comments.Clear();
		}
		protected internal virtual void OnUnitsChanged() {
			this.unitConverter = InternalAPI.UnitConverters[Unit];
		}
		protected internal virtual void PopulateParagraphs() {
			this.paragraphs.Clear();
			PieceTable.Paragraphs.ForEach(RegisterParagraph);
		}
		protected internal virtual void RegisterParagraph(ModelParagraph paragraph) {
			NativeParagraph nativeParagraph = new NativeParagraph(this, paragraph);
			paragraphs.Add(nativeParagraph);
		}
		protected internal virtual void PopulateFields() {
			this.fields.Clear();
			PieceTable.Fields.ForEach(RegisterField);
		}
		protected internal virtual void RegisterField(ModelField field) {
			fields.Add(new NativeField(this, field));
		}
		protected internal virtual void PopulateBookmarks() {
			this.bookmarks.Clear();
			PieceTable.Bookmarks.ForEach(RegisterBookmark);
		}
		protected internal virtual void PopulateCustomMarks() {
			this.customMarks.Clear();
			PieceTable.CustomMarks.ForEach(RegisterCustomMark);
		}
		protected internal virtual void PopulateComments() {
			this.comments.Clear();
			PieceTable.Comments.ForEach(RegisterComments);
		}
		protected internal virtual void RegisterBookmark(ModelBookmark bookmark) {
			bookmarks.Add(new NativeBookmark(this, bookmark));
		}
		protected internal virtual void RegisterCustomMark(ModelCustomMark customMark) {
			customMarks.Add(new NativeCustomMark(this, customMark));
		}
		protected internal virtual void RegisterComments(ModelComment comment) {
			comments.Add(new NativeComment(this, comment));
		}
		protected internal virtual void PopulateHyperlinks() {
			this.hyperlinks.Clear();
			foreach (int fieldIndex in PieceTable.HyperlinkInfos) {
				NativeHyperlink hyperlink = new NativeHyperlink(this, PieceTable, PieceTable.Fields[fieldIndex]);
				hyperlinks.Add(hyperlink);
			}
		}
		protected internal virtual void PopulateTables() {
			this.tables.Clear();
			PieceTable.Tables.ForEach(RegisterTable);
		}
		protected internal virtual void PopulateShapes() {
		}
		protected internal virtual void PopulateImages() {
			this.images.InnerList.Clear();
		}
		protected internal virtual void RegisterTable(ModelTable table) {
			tables.Insert(table.Index, new NativeTable(this, table));
		}
		protected internal virtual void UnRegisterTable(ModelTable table) {
			for (int i = tables.Count - 1; i >= 0; i--) {
				if (tables[i].ModelTable == table) {
					tables[i].IsValid = false;
					tables.RemoveAt(i);
					return;
				}
			}
		}
		#region InternalAPI interop
		protected internal virtual void SubscribeInternalAPIEvents() {
			InternalAPI.ParagraphInserted += OnParagraphInserted;
			InternalAPI.ParagraphRemoved += OnParagraphRemoved;
			InternalAPI.ParagraphMerged += OnParagraphMerged;
			InternalAPI.FieldInserted += OnFieldInserted;
			InternalAPI.FieldRemoved += OnFieldRemoved;
			InternalAPI.HyperlinkInfoInserted += OnHyperlinkInfoInserted;
			InternalAPI.HyperlinkInfoDeleted += OnHyperlinkInfoDeleted;
			InternalAPI.CustomMarkInserted += OnCustomMarkInserted;
			InternalAPI.CustomMarkDeleted += OnCustomMarkDeleted;
			PieceTable.Tables.CollectionChanged += OnTablesCollectionChanged;
		}
		protected internal virtual void UnsubscribeInternalAPIEvents() {
			InternalAPI.ParagraphInserted -= OnParagraphInserted;
			InternalAPI.ParagraphRemoved -= OnParagraphRemoved;
			InternalAPI.ParagraphMerged -= OnParagraphMerged;
			InternalAPI.FieldInserted -= OnFieldInserted;
			InternalAPI.FieldRemoved -= OnFieldRemoved;
			InternalAPI.HyperlinkInfoInserted -= OnHyperlinkInfoInserted;
			InternalAPI.HyperlinkInfoDeleted -= OnHyperlinkInfoDeleted;
			InternalAPI.CustomMarkInserted -= OnCustomMarkInserted;
			InternalAPI.CustomMarkDeleted -= OnCustomMarkDeleted;
			PieceTable.Tables.CollectionChanged -= OnTablesCollectionChanged;
		}
		void OnParagraphInserted(object sender, DevExpress.XtraRichEdit.Model.ParagraphEventArgs e) {
			if (!Object.ReferenceEquals(PieceTable, e.PieceTable))
				return;
			int paragraphIndex = ParagraphIndexToInt(e.ParagraphIndex);
			NativeParagraph paragraph = new NativeParagraph(this, PieceTable.Paragraphs[e.ParagraphIndex]);
			paragraphs.Insert(paragraphIndex, paragraph);
		}
		void OnParagraphRemoved(object sender, DevExpress.XtraRichEdit.Model.ParagraphEventArgs e) {
			if (!Object.ReferenceEquals(PieceTable, e.PieceTable))
				return;
			OnParagraphRemovedCore(e);
		}
		void OnParagraphMerged(object sender, DevExpress.XtraRichEdit.Model.ParagraphEventArgs e) {
			if (!Object.ReferenceEquals(PieceTable, e.PieceTable))
				return;
			OnParagraphRemovedCore(e);
		}
		void OnParagraphRemovedCore(DevExpress.XtraRichEdit.Model.ParagraphEventArgs e) {
			int paragraphIndex = ParagraphIndexToInt(e.ParagraphIndex);
			NativeParagraph paragraph = paragraphs[paragraphIndex];
			paragraph.IsValid = false;
			paragraphs.RemoveAt(paragraphIndex);
		}
		void OnFieldInserted(object sender, DevExpress.XtraRichEdit.Model.FieldEventArgs e) {
			if (!Object.ReferenceEquals(PieceTable, e.PieceTable))
				return;
			int fieldIndex = e.FieldIndex;
			NativeField field = new NativeField(this, PieceTable.Fields[fieldIndex]);
			fields.Insert(fieldIndex, field);
		}
		void OnFieldRemoved(object sender, DevExpress.XtraRichEdit.Model.FieldEventArgs e) {
			if (!Object.ReferenceEquals(PieceTable, e.PieceTable))
				return;
			int fieldIndex = e.FieldIndex;
			NativeField field = fields[fieldIndex];
			field.IsValid = false;
			fields.RemoveAt(fieldIndex);
		}
		void OnHyperlinkInfoInserted(object sender, DevExpress.XtraRichEdit.Model.HyperlinkInfoEventArgs e) {
			if (!Object.ReferenceEquals(PieceTable, e.PieceTable))
				return;
			NativeHyperlink hyperlink = new NativeHyperlink(this, PieceTable, PieceTable.Fields[e.FieldIndex]);
			hyperlinks.Add(hyperlink);
		}
		void OnHyperlinkInfoDeleted(object sender, DevExpress.XtraRichEdit.Model.HyperlinkInfoEventArgs e) {
			if (!Object.ReferenceEquals(PieceTable, e.PieceTable))
				return;
			NativeHyperlink hyperlink = hyperlinks.Find(e.FieldIndex);
			hyperlink.IsValid = false;
			hyperlinks.Remove(hyperlink);
		}
		void OnCustomMarkInserted(object sender, DevExpress.XtraRichEdit.Model.CustomMarkEventArgs e) {
			if (!Object.ReferenceEquals(PieceTable, e.PieceTable))
				return;
			NativeCustomMark customMark = new NativeCustomMark(this, PieceTable.CustomMarks[e.CustomMarkIndex]);
			customMarks.Insert(e.CustomMarkIndex, customMark);
		}
		void OnCustomMarkDeleted(object sender, DevExpress.XtraRichEdit.Model.CustomMarkEventArgs e) {
			if (!Object.ReferenceEquals(PieceTable, e.PieceTable))
				return;
			int customMarkIndex = e.CustomMarkIndex;
			NativeCustomMark customMark = customMarks[e.CustomMarkIndex];
			customMark.IsValid = false;
			customMarks.RemoveAt(customMarkIndex);
		}
		void OnTablesCollectionChanged(object sender, CollectionChangedEventArgs<ModelTable> e) {
			switch (e.Action) {
				case CollectionChangedAction.Add:
					RegisterTable(e.Element);
					break;
				case CollectionChangedAction.Remove:
					UnRegisterTable(e.Element);
					break;
				case CollectionChangedAction.Clear:
					tables.Clear();
					break;
				case CollectionChangedAction.EndBatchUpdate:
					PopulateTables();
					break;
			}
		}
		#endregion
		internal int ParagraphIndexToInt(DevExpress.XtraRichEdit.Model.ParagraphIndex paragraphIndex) {
			return ((IConvertToInt<DevExpress.XtraRichEdit.Model.ParagraphIndex>)paragraphIndex).ToInt();
		}
		protected int SectionIndexToInt(DevExpress.XtraRichEdit.Model.SectionIndex sectionIndex) {
			return ((IConvertToInt<DevExpress.XtraRichEdit.Model.SectionIndex>)sectionIndex).ToInt();
		}
		public void BeginUpdate() {
			CheckValid();
			DocumentServer.BeginUpdate();
			DocumentModel.History.BeginTransaction();
		}
		public void EndUpdate() {
			CheckValid();
			DocumentModel.History.EndTransaction();
			DocumentServer.EndUpdate();
		}
		#region Unit conversion
		protected internal virtual int UnitsToLayoutUnits(float value) {
			float modelValue = UnitConverter.ToUnits(value);
			return (int)Math.Round(DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(modelValue));
		}
		protected internal virtual float LayoutUnitsToUnits(int value) {
			float modelValue = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(value);
			return UnitConverter.FromUnits(modelValue);
		}
		protected internal virtual int UnitsToModelUnits(float value) {
			return (int)Math.Round(UnitConverter.ToUnits(value));
		}
		protected internal virtual float UnitsToModelUnitsF(float value) {
			return UnitConverter.ToUnits(value);
		}
		protected internal virtual float ModelUnitsToUnits(int value) {
			return UnitConverter.FromUnits(value);
		}
		protected internal virtual float ModelUnitsToUnitsF(float value) {
			return UnitConverter.FromUnits(value);
		}
		protected internal virtual float? ModelUnitsToUnits(int? value) {
			if (value.HasValue)
				return ModelUnitsToUnits(value.Value);
			else
				return null;
		}
		protected internal virtual float? ModelUnitsToUnitsF(float? value) {
			if (value.HasValue)
				return ModelUnitsToUnitsF(value.Value);
			else
				return null;
		}
		#endregion
		protected internal virtual ModelLogPosition NormalizeLogPosition(ModelLogPosition pos) {
			return NormalizeLogPosition(PieceTable, pos);
		}
		protected internal virtual ModelLogPosition NormalizeLogPosition(ModelPieceTable pieceTable, ModelLogPosition pos) {
			ModelLogPosition result = Algorithms.Max(pos, ModelLogPosition.Zero);
			result = Algorithms.Min(result, pieceTable.DocumentEndLogPosition);
			return result;
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraphCollection.Insert(DocumentPosition pos)' method instead.")]
		public Paragraph InsertParagraph(DocumentPosition pos) {
			return InsertParagraph(pos, InsertOptions.MatchDestinationFormatting);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraphCollection.Insert(DocumentPosition pos, InsertOptions insertOptions)' method instead.")]
		public Paragraph InsertParagraph(DocumentPosition pos, InsertOptions insertOptions) {
			CheckValid();
			CheckDocumentPosition(pos);
			NativeDocumentPosition nativePosition = (NativeDocumentPosition)pos;
			ModelParagraphIndex paragraphIndex = nativePosition.Position.ParagraphIndex;
			ModelLogPosition logPosition = NormalizeLogPosition(nativePosition.Position.LogPosition);
			PieceTable.InsertParagraph(logPosition);
			DevExpress.XtraRichEdit.Model.Paragraph paragraph = PieceTable.Paragraphs[paragraphIndex + 1];
			if (paragraph.IsInList() && insertOptions == InsertOptions.KeepSourceFormatting)
				PieceTable.RemoveNumberingFromParagraph(paragraph);
			return Paragraphs[ParagraphIndexToInt(paragraphIndex + 1)];
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraphCollection.Append()' method instead.")]
		public Paragraph AppendParagraph() {
			return InsertParagraph(EndPosition);
		}
		internal DocumentRange InsertContentCore(DocumentPosition pos, DocumentContentInserter inserter) {
			CheckValid();
			CheckDocumentPosition(pos);
			NativeDocumentPosition nativePosition = (NativeDocumentPosition)pos;
			ModelLogPosition logPosition = nativePosition.Position.LogPosition;
			logPosition = NormalizeLogPosition(logPosition);
			if (!inserter.InsertContent(PieceTable, logPosition))
				return CreateZeroLengthRange(logPosition);
			ModelLogPosition logPosition2 = NormalizeLogPosition(nativePosition.Position.LogPosition);
			int length = logPosition2 - logPosition;
			if (inserter.Append)
				logPosition++;
			if (length > 0)
				return CreateRange(logPosition, length);
			else
				return CreateZeroLengthRange(logPosition);
		}
		public DocumentRange InsertText(DocumentPosition pos, string text) {
			return InsertContentCore(pos, new DocumentTextContentInserter(text));
		}
		public DocumentRange AppendText(string text) {
			return InsertContentCore(EndPosition, new DocumentTextContentInserter(text, true));
		}
		public DocumentRange InsertSingleLineText(DocumentPosition pos, string text) {
			return InsertContentCore(pos, new DocumentSingleLineTextContentInserter(text));
		}
		public DocumentRange AppendSingleLineText(string text) {
			return InsertSingleLineText(EndPosition, text);
		}
		public DocumentRange InsertRtfText(DocumentPosition pos, string rtfText, InsertOptions insertOptions) {
			return InsertContentCore(pos, new DocumentRtfTextContentInserter(rtfText, insertOptions));
		}
		public DocumentRange InsertRtfText(DocumentPosition pos, string rtfText) {
			return InsertRtfText(pos, rtfText, InsertOptions.MatchDestinationFormatting);
		}
		public DocumentRange AppendRtfText(string rtfText, InsertOptions insertOptions) {
			return InsertRtfText(EndPosition, rtfText, insertOptions);
		}
		public DocumentRange AppendRtfText(string rtfText) {
			return InsertRtfText(EndPosition, rtfText);
		}
		public DocumentRange InsertHtmlText(DocumentPosition pos, string htmlText, InsertOptions insertOptions) {
			return InsertContentCore(pos, new DocumentHtmlTextContentInserter(htmlText, insertOptions));
		}
		public DocumentRange InsertHtmlText(DocumentPosition pos, string htmlText) {
			return InsertHtmlText(pos, htmlText, InsertOptions.MatchDestinationFormatting);
		}
		public DocumentRange AppendHtmlText(string htmlText, InsertOptions insertOptions) {
			return InsertHtmlText(EndPosition, htmlText, insertOptions);
		}
		public DocumentRange AppendHtmlText(string htmlText) {
			return InsertHtmlText(EndPosition, htmlText);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeDocumentImageCollection.Insert(DocumentPosition pos, DocumentImageSource imageSource)' method instead.")]
		public DocumentImage InsertImage(DocumentPosition pos, DocumentImageSource imageSource) {
			CheckValid();
			CheckDocumentPosition(pos);
			OfficeImage image = imageSource.CreateImage(DocumentModel);
			if (image == null)
				return null;
			ModelRunInfo runInfo = new ModelRunInfo(PieceTable);
			ModelLogPosition logPosition = NormalizeLogPosition(pos.LogPosition);
			PieceTable.InsertInlinePicture(logPosition, image);
			PieceTable.CalculateRunInfoStart(logPosition, runInfo);
			return NativeDocumentImage.CreateUnsafe(this, runInfo.Start.RunIndex);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeDocumentImageCollection.Append(DocumentImageSource imageSource)' method instead.")]
		public DocumentImage AppendImage(DocumentImageSource imageSource) {
			return InsertImage(EndPosition, imageSource);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeShapeCollection.InsertPicture(DocumentPosition pos, DocumentImageSource imageSource)' method instead.")]
		public Shape InsertPicture(DocumentPosition pos, DocumentImageSource imageSource) {
			return InsertShapePicture(pos, imageSource);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeShapeCollection.InsertShapePicture(DocumentPosition pos, DocumentImageSource imageSource)' method instead.")]
		protected internal virtual Shape InsertShapePicture(DocumentPosition pos, DocumentImageSource imageSource) {
			CheckValid();
			CheckDocumentPosition(pos);
			OfficeImage image = imageSource.CreateImage(DocumentModel);
			if (image == null)
				return null;
			ModelRunInfo runInfo = new ModelRunInfo(PieceTable);
			ModelLogPosition logPosition = NormalizeLogPosition(pos.LogPosition);
			logPosition = PieceTable.FindParagraph(logPosition).LogPosition;
			DocumentModel.BeginUpdate();
			try {
				PieceTable.InsertFloatingObjectAnchor(logPosition);
				PieceTable.CalculateRunInfoStart(logPosition, runInfo);
				ModelFloatingObjectAnchorRun run = (ModelFloatingObjectAnchorRun)PieceTable.Runs[runInfo.Start.RunIndex];
				DevExpress.XtraRichEdit.Model.PictureFloatingObjectContent content = new DevExpress.XtraRichEdit.Model.PictureFloatingObjectContent(run, image);
				run.SetContent(content);
				Size size = DocumentModel.UnitConverter.TwipsToModelUnits(image.SizeInTwips);
				run.FloatingObjectProperties.ActualSize = size;
				content.SetOriginalSize(size);
				return NativeShape.CreateUnsafe(this, runInfo.Start.RunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeShapeCollection.InsertTextBox(DocumentPosition pos)' method instead.")]
		public virtual Shape InsertTextBox(DocumentPosition pos) {
			CheckValid();
			CheckDocumentPosition(pos);
			ModelRunInfo runInfo = new ModelRunInfo(PieceTable);
			ModelLogPosition logPosition = NormalizeLogPosition(pos.LogPosition);
			DocumentModel.BeginUpdate();
			try {
				PieceTable.InsertFloatingObjectAnchor(logPosition);
				PieceTable.CalculateRunInfoStart(logPosition, runInfo);
				ModelFloatingObjectAnchorRun run = (ModelFloatingObjectAnchorRun)PieceTable.Runs[runInfo.Start.RunIndex];
				DevExpress.XtraRichEdit.Model.TextBoxContentType textBoxContentType = new DevExpress.XtraRichEdit.Model.TextBoxContentType(DocumentModel);
				DevExpress.XtraRichEdit.Model.TextBoxFloatingObjectContent content = new DevExpress.XtraRichEdit.Model.TextBoxFloatingObjectContent(run, textBoxContentType);
				run.SetContent(content);
				Size size = DocumentModel.UnitConverter.TwipsToModelUnits(new Size(3 * 1440, 2 * 1440));
				run.FloatingObjectProperties.ActualSize = size;
				return NativeShape.CreateUnsafe(this, runInfo.Start.RunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
#if !SL && !DXPORTABLE
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeDocumentImageCollection.Insert(DocumentPosition pos, System.Drawing.Image image)' method instead.")]
		public DocumentImage InsertImage(DocumentPosition pos, System.Drawing.Image image) {
			return InsertImage(pos, DocumentImageSource.FromImage(image));
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeDocumentImageCollection.Append(System.Drawing.Image image)' method instead.")]
		public DocumentImage AppendImage(System.Drawing.Image image) {
			return InsertImage(EndPosition, image);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeShapeCollection.InsertPicture(DocumentPosition pos, System.Drawing.Image image)' method instead.")]
		public Shape InsertPicture(DocumentPosition pos, System.Drawing.Image image) {
			return InsertShapePicture(pos, DocumentImageSource.FromImage(image));
		}
#endif
		public void InvalidateDocumentLayout() {
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeTableCollection.Create(DocumentPosition pos, int rowCount, int columnCount)' method instead.")]
		public Table InsertTable(DocumentPosition pos, int rowCount, int columnCount) {
			return InsertTable(pos, rowCount, columnCount, AutoFitBehaviorType.AutoFitToContents, Int32.MinValue);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeTableCollection.Create(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehaviorType)' method instead.")]
		public Table InsertTable(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehaviorType) {
			return InsertTable(pos, rowCount, columnCount, autoFitBehaviorType, Int32.MinValue);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeTableCollection.Create(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehaviorType, int fixedColumnWidths)' method instead.")]
		public Table InsertTable(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehaviorType, int fixedColumnWidths) {
			CheckValid();
			CheckDocumentPosition(pos);
			ModelLogPosition logPosition = NormalizeLogPosition(pos.LogPosition);
			ModelTable newTable;
			DocumentModel.BeginUpdate();
			try {
				newTable = PieceTable.InsertTable(logPosition, rowCount, columnCount, (TableAutoFitBehaviorType)autoFitBehaviorType, UnitsToModelUnits(fixedColumnWidths));
				int styleIndex = DocumentModel.TableStyles.GetStyleIndexByName(DevExpress.XtraRichEdit.Model.TableStyleCollection.TableSimpleStyleName);
				if (styleIndex >= 0)
					newTable.StyleIndex = styleIndex;
			}
			finally {
				DocumentModel.EndUpdate();
			}
			return Tables[newTable.Index];
		}
		public DocumentRange CreateRange(int start, int length) {
			CheckValid();
			Guard.ArgumentNonNegative(length, "length");
			ModelRunInfo rangeInfo = PieceTable.FindRunInfo(new ModelLogPosition(start), length + 1);
			return CreateNativeRange(rangeInfo.Start, rangeInfo.End);
		}
		public DocumentRange CreateRange(DocumentPosition start, int length) {
			CheckValid();
			Guard.ArgumentNonNegative(length, "length");
			ModelRunInfo rangeInfo = PieceTable.FindRunInfo(start.LogPosition, length + 1);
			return CreateNativeRange(rangeInfo.Start, rangeInfo.End);
		}
		protected virtual NativeDocumentRange CreateNativeRange(ModelPosition start, ModelPosition end) {
			return new NativeDocumentRange(this, start, end);
		}
		protected internal DocumentRange CreateRange(ModelLogPosition start, int length) {
			return CreateRange(((IConvertToInt<ModelLogPosition>)start).ToInt(), length);
		}
		protected internal NativeDocumentRange CreateZeroLengthRange(ModelLogPosition logPosition) {
			return new NativeDocumentRange(CreatePositionCore(logPosition), CreatePositionCore(logPosition));
		}
		public DocumentPosition CreatePosition(int start) {
			CheckValid();
			return CreatePositionCore(new ModelLogPosition(start));
		}
		protected internal virtual NativeDocumentPosition CreatePositionCore(ModelLogPosition pos) {
			DevExpress.XtraRichEdit.Model.PieceTable pieceTable = PieceTable;
			ModelRunInfo rangeInfo = new ModelRunInfo(pieceTable);
			pieceTable.CalculateRunInfoStart(pos, rangeInfo);
			return CreateNativePosition(rangeInfo.Start);
		}
		protected virtual NativeDocumentPosition CreateNativePosition(ModelPosition pos) {
			return new NativeDocumentPosition(this, pos);
		}
		public void Delete(DocumentRange range) {
			CheckValid();
			Guard.ArgumentNotNull(range, "range");
			CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			int length = nativeRange.NormalizedEnd.LogPosition - nativeRange.NormalizedStart.LogPosition;
			bool documentLastParagraphSelected = false;
			if (nativeRange.NormalizedEnd.LogPosition > PieceTable.DocumentEndLogPosition) {
				length -= nativeRange.NormalizedEnd.LogPosition - PieceTable.DocumentEndLogPosition;
				documentLastParagraphSelected = true;
			}
			if (length <= 0)
				return;
			PieceTable.DeleteContent(nativeRange.NormalizedStart.LogPosition, length, documentLastParagraphSelected);
		}
		public void SelectAll() {
			CheckValid();
			ModelSelection selection = DocumentModel.Selection;
			DocumentModel.BeginUpdate();
			try {
				selection.ClearMultiSelection();
				selection.Start = PieceTable.DocumentStartLogPosition;
				selection.End = PieceTable.DocumentEndLogPosition + 1;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		internal class ParagraphRange {
			readonly ModelParagraphIndex start;
			readonly ModelParagraphIndex end;
			public ParagraphRange(ModelParagraphIndex start, ModelParagraphIndex end) {
				this.start = start;
				this.end = end;
			}
			public ModelParagraphIndex Start { get { return start; } }
			public ModelParagraphIndex End { get { return end; } }
			public int Length { get { return End - Start + 1; } }
		}
		internal ParagraphRange CalculateParagraphsRange(DocumentRange range) {
			CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			ModelParagraphIndex firstParagraphIndex = nativeRange.NormalizedStart.Position.ParagraphIndex;
			DevExpress.XtraRichEdit.Model.DocumentModelPosition lastPosition = nativeRange.NormalizedEnd.Position;
			ModelParagraphIndex lastParagraphIndex = lastPosition.ParagraphIndex;
			if (lastPosition.LogPosition == PieceTable.Paragraphs[lastParagraphIndex].LogPosition)
				lastParagraphIndex = Algorithms.Max(firstParagraphIndex, lastParagraphIndex - 1);
			return new ParagraphRange(firstParagraphIndex, lastParagraphIndex);
		}
		public ParagraphProperties BeginUpdateParagraphs(DocumentRange range) {
			CheckValid();
			Guard.ArgumentNotNull(range, "range");
			ParagraphRange paragraphsRange = CalculateParagraphsRange(range);
			DevExpress.XtraRichEdit.Model.ParagraphCollection modelParagraphs = PieceTable.Paragraphs;
			ModelParagraphCollection paragraphs = new ModelParagraphCollection();
			paragraphs.AddRange(modelParagraphs.GetRange(paragraphsRange.Start, paragraphsRange.Length));
			NativeParagraphProperties result = new NativeParagraphProperties(this, paragraphs);
			InternalAPI.RegisterModifier(result);
			DocumentModel.BeginUpdate();
			return result;
		}
		public void EndUpdateParagraphs(ParagraphProperties properties) {
			CheckValid();
			Guard.ArgumentNotNull(properties, "properties");
			NativeParagraphProperties props = (NativeParagraphProperties)properties;
			InternalAPI.UnregisterModifier(props);
			props.IsValid = false;
			DocumentModel.EndUpdate();
		}
		public CharacterProperties BeginUpdateCharacters(int start, int length) {
			CheckValid();
			return BeginUpdateCharactersCore(new ModelLogPosition(start), new ModelLogPosition(start + length));
		}
		public CharacterProperties BeginUpdateCharacters(DocumentPosition start, int length) {
			CheckValid();
			return BeginUpdateCharactersCore(start.LogPosition, start.LogPosition + length);
		}
		public CharacterProperties BeginUpdateCharacters(DocumentRange range) {
			CheckValid();
			Guard.ArgumentNotNull(range, "range");
			CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			return BeginUpdateCharactersCore(nativeRange.NormalizedStart.LogPosition, nativeRange.NormalizedEnd.LogPosition);
		}
		public CharacterProperties BeginUpdateCharactersCore(ModelLogPosition start, ModelLogPosition end) {
			DocumentModel.BeginUpdate(); 
			start = NormalizeLogPosition(start);
			end = Algorithms.Min(end, PieceTable.DocumentEndLogPosition + 1);
			int length = end - start;
			NativeCharacterProperties result = new NativeCharacterProperties(this, start, length);
			InternalAPI.RegisterModifier(result);
			return result;
		}
		public void EndUpdateCharacters(CharacterProperties properties) {
			CheckValid();
			Guard.ArgumentNotNull(properties, "properties");
			NativeCharacterProperties props = (NativeCharacterProperties)properties;
			InternalAPI.UnregisterModifier(props);
			props.IsValid = false;
			DocumentModel.EndUpdate();
		}
		public RangePermissionCollection BeginUpdateRangePermissions() {
			RangePermissionCollection result = new RangePermissionCollection();
			DevExpress.XtraRichEdit.Model.RangePermissionCollection modelPermissions = this.PieceTable.RangePermissions;
			int count = modelPermissions.Count;
			for (int i = 0; i < count; i++)
				result.Add(CreateRangePermission(modelPermissions[i]));
			return result;
		}
		RangePermission CreateRangePermission(DevExpress.XtraRichEdit.Model.RangePermission rangePermission) {
			int length = rangePermission.End - rangePermission.Start;
			DocumentRange range;
			if (length > 0)
				range = CreateRange(rangePermission.Start, length);
			else
				range = CreateZeroLengthRange(rangePermission.Start);
			RangePermission result = new RangePermission(range);
			result.UserName = rangePermission.UserName;
			result.Group = rangePermission.Group;
			return result;
		}
		public void EndUpdateRangePermissions(RangePermissionCollection permissions) {
			DocumentModel.BeginUpdate();
			try {
				DevExpress.XtraRichEdit.Model.RangePermissionCollection modelPermissions = this.PieceTable.RangePermissions;
				for (int i = modelPermissions.Count - 1; i >= 0; i--)
					PieceTable.DeleteRangePermission(modelPermissions[i]);
				if (permissions != null)
					ApplyNewRangePermissions(permissions);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void ApplyNewRangePermissions(RangePermissionCollection permissions) {
			int count = permissions.Count;
			for (int i = 0; i < count; i++)
				ApplyNewRangePermission(permissions[i]);
		}
		void ApplyNewRangePermission(RangePermission rangePermission) {
			if (String.IsNullOrEmpty(rangePermission.UserName) && String.IsNullOrEmpty(rangePermission.Group))
				return;
			NativeDocumentPosition nativePositionStart = (NativeDocumentPosition)rangePermission.Range.Start;
			NativeDocumentPosition nativePositionEnd = (NativeDocumentPosition)rangePermission.Range.End;
			ModelLogPosition logPositionStart = NormalizeLogPosition(nativePositionStart.Position.LogPosition);
			ModelLogPosition logPositionEnd = NormalizeLogPosition(nativePositionEnd.Position.LogPosition);
			DevExpress.XtraRichEdit.Model.RangePermissionInfo info = new DevExpress.XtraRichEdit.Model.RangePermissionInfo();
			info.UserName = rangePermission.UserName;
			info.Group = rangePermission.Group;
			PieceTable.ApplyDocumentPermission(logPositionStart, logPositionEnd, info);
		}
		public void CancelUpdateRangePermissions(RangePermissionCollection permissions) {
		}
		public void SetPageBackground(Color pageBackColor, bool showPageBackground) {
			CheckValid();
			DocumentProperties documentProperties = DocumentModel.DocumentProperties;
			documentProperties.BeginUpdate();
			try {
				documentProperties.PageBackColor = pageBackColor;
				documentProperties.DisplayBackgroundShape = showPageBackground;
			}
			finally {
				documentProperties.EndUpdate();
			}
		}
		public void SetPageBackground(Color pageBackColor) {
			CheckValid();
			DocumentModel.DocumentProperties.PageBackColor = pageBackColor;
		}
		public void SetPageBackground(bool showPageBackground) {
			CheckValid();
			DocumentModel.DocumentProperties.DisplayBackgroundShape = showPageBackground;
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeBookmarkCollection.Create(DocumentPosition start, int length, string name)' method instead.")]
		public Bookmark CreateBookmark(DocumentPosition start, int length, string name) {
			CheckValid();
			Guard.ArgumentNotNull(start, "start");
			Guard.ArgumentNonNegative(length, "length");
			Guard.ArgumentNotNull(name, "name");
			if (!ModelBookmark.IsNameValid(name))
				throw new ArgumentException(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_IncorrectBookmarkName));
			if (DocumentModel.ActivePieceTable.Bookmarks.FindByName(name) != null)
				throw new InvalidOperationException(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_DuplicateBookmark));
			CheckDocumentPosition(start);
			PieceTable.CreateBookmark(NormalizeLogPosition(start.LogPosition), length, name.Trim());
			return bookmarks.LastInsertedBookmark;
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeBookmarkCollection.Create(DocumentRange range, string name)' method instead.")]
		public Bookmark CreateBookmark(DocumentRange range, string name) {
			return CreateBookmark(range.Start, range.Length, name);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XXtraRichEdit.API.Native.Implementation.NativeCustomMarkCollection.Create(DocumentPosition position, object userData)' method instead.")]
		public CustomMark CreateCustomMark(DocumentPosition position, object userData) {
			CheckValid();
			Guard.ArgumentNotNull(position, "position");
			CheckDocumentPosition(position);
			int prevCount = customMarks.Count;
			PieceTable.CreateCustomMark(NormalizeLogPosition(position.LogPosition), userData);
			if (customMarks.Count > prevCount)
				return customMarks[prevCount];
			else
				return null;
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XXtraRichEdit.API.Native.Implementation.NativeCustomMarkCollection.Remove(CustomMark customMark)' method instead.")]
		public void DeleteCustomMark(CustomMark customMark) {
			CheckValid();
			Guard.ArgumentNotNull(customMark, "customMark");
			int index = ((NativeCustomMarkCollection)CustomMarks).IndexOf((NativeCustomMark)customMark);
			if (index < 0)
				Exceptions.ThrowArgumentException("customMark", customMark);
			PieceTable.DeleteCustomMark(index);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XXtraRichEdit.API.Native.Implementation.NativeCustomMarkCollection.GetByVisualInfo(DevExpress.XtraRichEdit.Layout.Export.CustomMarkVisualInfo customMarkVisualInfo)' method instead.")]
		public CustomMark GetCustomMarkByVisualInfo(DevExpress.XtraRichEdit.Layout.Export.CustomMarkVisualInfo customMarkVisualInfo) {
			CheckValid();
			Guard.ArgumentNotNull(customMarkVisualInfo, "customMarkVisualInfo");
			int count = customMarks.Count;
			for (int i = 0; i < count; i++)
				if (customMarkVisualInfo.CustomMark == ((NativeCustomMark)CustomMarks[i]).ModelCustomMark)
					return CustomMarks[i];
			return null;
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeBookmarkCollection.Select(Bookmark bookmark)' method instead.")]
		public void SelectBookmark(Bookmark bookmark) {
			if (!Object.ReferenceEquals(PieceTable, DocumentModel.ActivePieceTable))
				throw new InvalidOperationException(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_SelectBookmarkError));
			CheckValid();
			Guard.ArgumentNotNull(bookmark, "bookmark");
			CheckDocumentRange(bookmark.Range);
			NativeDocumentRange range = (NativeDocumentRange)bookmark.Range;
			SetSelectionCore(range.Start.LogPosition, range.End.LogPosition, false);
		}
		protected internal void SetSelectionCore(ModelLogPosition start, ModelLogPosition end) {
			SetSelectionCore(start, end, true);
		}
		protected internal void SetSelectionCore(ModelLogPosition start, ModelLogPosition end, bool forceUpdateTableSelection) {
			DocumentModel.BeginUpdate();
			try {
				start = NormalizeLogPosition(DocumentModel.Selection.PieceTable, start);
				end = Algorithms.Min(end, DocumentModel.Selection.PieceTable.DocumentEndLogPosition + 1);
				ModelSelection selection = DocumentModel.Selection;
				selection.BeginUpdate();
				try {
					selection.ClearMultiSelection();
					selection.Start = start;
					selection.End = end;
					selection.SetStartCell(start);
					if (start != end && forceUpdateTableSelection) {
						selection.UpdateTableSelectionEnd(end);
						selection.UpdateTableSelectionStart(start);
					}
				}
				finally {
					selection.EndUpdate();
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeBookmarkCollection.Remove(Bookmark bookmark)' method instead.")]
		public void RemoveBookmark(Bookmark bookmark) {
			CheckValid();
			Guard.ArgumentNotNull(bookmark, "bookmark");
			int count = bookmarks.Count;
			int index = 0;
			for (; index < count; index++) {
				if (String.Equals(bookmarks[index].Name, bookmark.Name, StringComparison.Ordinal))
					break;
			}
			if (index == count)
				return;
			PieceTable.DeleteBookmark(index);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeHyperlinkCollection.Create(DocumentPosition start, int length)' method instead.")]
		public Hyperlink CreateHyperlink(DocumentPosition start, int length) {
			CheckValid();
			Guard.ArgumentNotNull(start, "start");
			Guard.ArgumentNonNegative(length, "length");
			CheckDocumentPosition(start);
			int prevCount = hyperlinks.Count;
			PieceTable.CreateHyperlink(start.LogPosition, length, new ModelHyperlinkInfo());
			if (hyperlinks.Count > prevCount)
				return hyperlinks[prevCount];
			return null;
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeHyperlinkCollection.Create(DocumentRange range)' method instead.")]
		public Hyperlink CreateHyperlink(DocumentRange range) {
			return CreateHyperlink(range.Start, range.Length);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeHyperlinkCollection.HyperlinkCollection.Remove(Hyperlink hyperlink)' method instead.")]
		public void RemoveHyperlink(Hyperlink hyperlink) {
			CheckValid();
			Guard.ArgumentNotNull(hyperlink, "hyperlink");
			PieceTable.DeleteHyperlink(((NativeHyperlink)hyperlink).Field);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraphCollection.GetParagraph(DocumentPosition pos)' method instead.")]
		public Paragraph GetParagraph(DocumentPosition pos) {
			CheckValid();
			CheckDocumentPosition(pos);
			ModelParagraphIndex paragraphIndex = PieceTable.FindParagraphIndex(pos.LogPosition, false);
			int index = ((IConvertToInt<ModelParagraphIndex>)paragraphIndex).ToInt();
			if (index < 0)
				return null;
			return Paragraphs[index];
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeTableCollection.GetTableCell(DocumentPosition pos)' method instead.")]
		public TableCell GetTableCell(DocumentPosition pos) {
			Paragraph paragraph = GetParagraph(pos);
			NativeParagraph nativeParagraph = (NativeParagraph)paragraph;
			if (nativeParagraph == null)
				return null;
			Model.TableCell cell = nativeParagraph.InnerParagraph.GetCell();
			return cell != null ? Tables[cell.Table.Index][cell.RowIndex, cell.IndexInRow] : null;
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraphCollection.Get(DocumentRange range)' method instead.")]
		public ParagraphCollection GetParagraphs(DocumentRange range) {
			CheckValid();
			Guard.ArgumentNotNull(range, "range");
			ParagraphRange paragraphsRange = CalculateParagraphsRange(range);
			int firstIndex = ((IConvertToInt<ModelParagraphIndex>)paragraphsRange.Start).ToInt();
			int lastIndex = firstIndex + paragraphsRange.Length - 1;
			NativeParagraphCollection result = new NativeParagraphCollection(this);
			for (int i = firstIndex; i <= lastIndex; i++)
				result.Add(paragraphs[i]);
			return result;
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeDocumentImageCollection.Get(DocumentRange range)' method instead.")]
		public DocumentImageCollection GetImages(DocumentRange range) {
			CheckValid();
			CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			ModelRunIndex firstRunIndex = nativeRange.NormalizedStart.Position.RunIndex;
			ModelRunIndex lastRunIndex = nativeRange.NormalizedEnd.Position.RunIndex;
			NativeDocumentImageCollection result = new NativeDocumentImageCollection(this);
			for (ModelRunIndex i = firstRunIndex; i <= lastRunIndex; i++) {
				NativeDocumentImage image = NativeDocumentImage.TryCreate(this, PieceTable.Runs[i], i);
				if (image != null)
					result.InnerList.Add(image);
			}
			return result;
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeShapeCollection.GetShapes(DocumentRange range)' method instead.")]
		public ShapeCollection GetShapes(DocumentRange range) {
			CheckValid();
			CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			ModelRunIndex firstRunIndex = nativeRange.NormalizedStart.Position.RunIndex;
			ModelRunIndex lastRunIndex = nativeRange.NormalizedEnd.Position.RunIndex;
			NativeShapeCollection result = new NativeShapeCollection(this);
			for (ModelRunIndex i = firstRunIndex; i <= lastRunIndex; i++) {
				NativeShape shape = NativeShape.TryCreate(this, PieceTable.Runs[i], i);
				if (shape != null && shape.Range.End <= range.End)
					result.InnerList.Add(shape);
			}
			return result;
		}
		protected internal DevExpress.XtraRichEdit.Model.SelectionRangeCollection GetSelectionRangeCollection(DocumentRange range) {
			DevExpress.XtraRichEdit.Model.SelectionRange selectionRange = GetSelectionRange(range);
			return selectionRange != null ? new DevExpress.XtraRichEdit.Model.SelectionRangeCollection() { selectionRange } : null;
		}
		DevExpress.XtraRichEdit.Model.SelectionRangeCollection GetSelectionRangeCollection(SelectionCollection selectionCollection) {
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection result = new Model.SelectionRangeCollection();
			foreach (DocumentRange range in selectionCollection) {
				DevExpress.XtraRichEdit.Model.SelectionRange selectionRange = GetSelectionRange(range);
				if (selectionRange != null)
					result.Add(selectionRange);
			}
			return result.Count > 0 ? result : null;
		}
		DevExpress.XtraRichEdit.Model.SelectionRange GetSelectionRange(DocumentRange range) {
			int length = range.Length;
			if (length <= 0)
				return null;
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			ModelLogPosition start = NormalizeLogPosition(nativeRange.NormalizedStart.LogPosition);
			ModelLogPosition end = Algorithms.Min(nativeRange.NormalizedEnd.LogPosition, PieceTable.DocumentEndLogPosition + 1);
			length = end - start;
			if (length <= 0)
				return null;
			return new DevExpress.XtraRichEdit.Model.SelectionRange(start, length);
		}
		CopySelectionManager CreateCopySelectionManagerForGetContent() {
			CopySelectionManager result = new CopySelectionManager(DocumentServer);
			result.FixLastParagraph = true;
			result.AllowCopyWholeFieldResult = true;
			result.DefaultPropertiesCopyOptions = DevExpress.XtraRichEdit.Model.DefaultPropertiesCopyOptions.Always;
			return result;
		}
		public string GetText(DocumentRange range) {
			return GetText(range, new TextFragmentOptions());
		}
		public string GetText(DocumentRange range, TextFragmentOptions getTextOptions) {
			return GetText(range, null, getTextOptions);
		}
		public string GetText(DocumentRange range, PlainTextDocumentExporterOptions options) {
			return GetText(range, options, null);
		}
		public string GetText(DocumentRange range, PlainTextDocumentExporterOptions options, TextFragmentOptions getTextOptions) {
			CheckValid();
			CheckDocumentRange(range);
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection selection = GetSelectionRangeCollection(range);
			if (selection == null)
				return String.Empty;
			CopySelectionManager manager = CreateCopySelectionManagerForGetContent();
			if (options != null && options.ExportFinalParagraphMark == Export.PlainText.ExportFinalParagraphMark.Always)
				manager.FixLastParagraph = false;
			return manager.GetPlainText(PieceTable, selection, options, getTextOptions);
		}
		internal string GetText(SelectionCollection selectionCollection, PlainTextDocumentExporterOptions options, TextFragmentOptions getTextOptions) {
			CheckValid();
			CheckSelectionCollection(selectionCollection);
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection selection = GetSelectionRangeCollection(selectionCollection);
			if (selection == null)
				return String.Empty;
			CopySelectionManager manager = CreateCopySelectionManagerForGetContent();
			if (options != null && options.ExportFinalParagraphMark == Export.PlainText.ExportFinalParagraphMark.Always)
				manager.FixLastParagraph = false;
			return manager.GetPlainText(PieceTable, selection, options, getTextOptions);
		}
		public string GetRtfText(DocumentRange range) {
			RtfDocumentExporterOptions options = new RtfDocumentExporterOptions();
			options.ExportFinalParagraphMark = DevExpress.XtraRichEdit.Export.Rtf.ExportFinalParagraphMark.Never;
			return GetRtfText(range, options, true, true);
		}
		public string GetRtfText(DocumentRange range, RtfDocumentExporterOptions options) {
			return GetRtfText(range, options, false, false);
		}
		protected internal string GetRtfText(DocumentRange range, RtfDocumentExporterOptions options, bool forceRaiseBeforeExport, bool forceRaiseAfterExport) {
			CheckValid();
			CheckDocumentRange(range);
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection selection = GetSelectionRangeCollection(range);
			if (selection == null)
				return String.Empty;
			CopySelectionManager manager = CreateCopySelectionManagerForGetContent();
			return manager.GetRtfText(PieceTable, selection, options, forceRaiseBeforeExport, forceRaiseAfterExport);
		}
		internal string GetRtfText(SelectionCollection selectionCollection, RtfDocumentExporterOptions options) {
			CheckValid();
			CheckSelectionCollection(selectionCollection);
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection selection = GetSelectionRangeCollection(selectionCollection);
			if (selection == null)
				return String.Empty;
			CopySelectionManager manager = CreateCopySelectionManagerForGetContent();
			return manager.GetRtfText(PieceTable, selection, options, false, false);
		}
		public string GetHtmlText(DocumentRange range, IUriProvider provider) {
			return GetHtmlText(range, provider, null);
		}
		public string GetHtmlText(DocumentRange range, IUriProvider provider, HtmlDocumentExporterOptions options) {
			CheckValid();
			CheckDocumentRange(range);
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection selection = GetSelectionRangeCollection(range);
			if (selection == null)
				return String.Empty;
			CopySelectionManager manager = CreateCopySelectionManagerForGetContent();
			return manager.GetHtmlText(PieceTable, selection, provider, options);
		}
		public string GetMhtText(DocumentRange range) {
			return GetMhtText(range, null);
		}
		public string GetMhtText(DocumentRange range, MhtDocumentExporterOptions options) {
			CheckValid();
			CheckDocumentRange(range);
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection selection = GetSelectionRangeCollection(range);
			if (selection == null)
				return String.Empty;
			CopySelectionManager manager = CreateCopySelectionManagerForGetContent();
			return manager.GetMhtText(PieceTable, selection, options);
		}
		public string GetWordMLText(DocumentRange range) {
			return GetWordMLText(range, null);
		}
		public string GetWordMLText(DocumentRange range, WordMLDocumentExporterOptions options) {
			CheckValid();
			CheckDocumentRange(range);
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection selection = GetSelectionRangeCollection(range);
			if (selection == null)
				return String.Empty;
			CopySelectionManager manager = CreateCopySelectionManagerForGetContent();
			return manager.GetWordMLText(PieceTable, selection, options);
		}
		public byte[] GetOpenXmlBytes(DocumentRange range) {
			return GetOpenXmlBytes(range, null);
		}
		public byte[] GetOpenXmlBytes(DocumentRange range, OpenXmlDocumentExporterOptions options) {
			CheckValid();
			CheckDocumentRange(range);
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection selection = GetSelectionRangeCollection(range);
			if (selection == null)
				return null;
			CopySelectionManager manager = CreateCopySelectionManagerForGetContent();
			return manager.GetOpenXmlBytes(PieceTable, selection, options);
		}
		public ISearchResult StartSearch(string textToFind, SearchOptions options, SearchDirection direction, DocumentRange range) {
			CheckValid();
			CheckDocumentRange(range);
			if (direction == SearchDirection.Forward)
				return new NativeSearchResultForward(this, textToFind, options, range);
			else
				return new NativeSearchResultBackward(this, textToFind, options, range);
		}
		public ISearchResult StartSearch(string textToFind, SearchOptions options, SearchDirection direction) {
			CheckValid();
			if (direction == SearchDirection.Forward)
				return new NativeSearchResultForward(this, textToFind, options, Range);
			else
				return new NativeSearchResultBackward(this, textToFind, options, Range);
		}
		public ISearchResult StartSearch(string textToFind) {
			CheckValid();
			return new NativeSearchResultForward(this, textToFind, SearchOptions.None, Range);
		}
		public IRegexSearchResult StartSearch(Regex regex, DocumentRange range) {
			return StartSearch(regex, range, DocumentModel.SearchOptions.RegExResultMaxGuaranteedLength);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IRegexSearchResult StartSearch(Regex regex, DocumentRange range, int maxGuaranteedSearchResultLength) {
			CheckValid();
			CheckDocumentRange(range);
			if ((regex.Options & RegexOptions.RightToLeft) != 0)
				return new NativeRegexSearchResultBackward(this, regex, range, maxGuaranteedSearchResultLength);
			else
				return new NativeRegexSearchResultForward(this, regex, range, maxGuaranteedSearchResultLength);
		}
		public IRegexSearchResult StartSearch(Regex regex) {
			return StartSearch(regex, Range);
		}
		public DocumentRange[] FindAll(Regex regex, DocumentRange range) {
			return FindAll(regex, range, DocumentModel.SearchOptions.RegExResultMaxGuaranteedLength);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DocumentRange[] FindAll(Regex regex, DocumentRange range, int maxGuaranteedSearchResultLength) {
			CheckValid();
			CheckDocumentRange(range);
			NativeRegexSearch search;
			if ((regex.Options & RegexOptions.RightToLeft) != 0)
				search = new NativeRegexSearchBackward(this, regex, maxGuaranteedSearchResultLength);
			else
				search = new NativeRegexSearchForward(this, regex, maxGuaranteedSearchResultLength);
			return search.Matches(range);
		}
		public DocumentRange[] FindAll(System.Text.RegularExpressions.Regex regex) {
			return FindAll(regex, Range);
		}
		public DocumentRange[] FindAll(string textToFind, SearchOptions options, DocumentRange range) {
			CheckValid();
			CheckDocumentRange(range);
			List<DocumentRange> result = new List<DocumentRange>();
			ISearchResult searchResult = StartSearch(textToFind, options, SearchDirection.Forward, range);
			while (searchResult.FindNext())
				result.Add(searchResult.CurrentResult);
			return result.ToArray();
		}
		public DocumentRange[] FindAll(string textToFind, SearchOptions options) {
			return FindAll(textToFind, options, Range);
		}
		void SubDocument.Replace(DocumentRange range, string text) {
			this.Replace(range, text);
		}
		protected internal int Replace(DocumentRange range, string text) {
			CheckValid();
			CheckDocumentRange(range);
			ModelLogPosition start = range.Start.LogPosition;
			ModelLogPosition end = range.End.LogPosition;
			int length = end - start;
			if (end == Range.End.LogPosition)
				length--;
			ModelLogPosition endLogPositionBefore = PieceTable.DocumentEndLogPosition;
			PieceTable.ReplaceText(start, length, text);
			ModelLogPosition endLogPositionAfter = PieceTable.DocumentEndLogPosition;
			return endLogPositionAfter - endLogPositionBefore;
		}
		public int ReplaceAll(string textToFind, string replaceWith, SearchOptions options, DocumentRange range) {
			CheckValid();
			CheckDocumentRange(range);
			int replacementCount = 0;
			ISearchResult searchResult = StartSearch(textToFind, options, SearchDirection.Forward, range);
			DocumentModel.BeginUpdate();
			try {
				while (searchResult.FindNext()) {
					searchResult.Replace(replaceWith);
					replacementCount++;
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
			return replacementCount;
		}
		public int ReplaceAll(string textToFind, string replaceWith, SearchOptions options) {
			return ReplaceAll(textToFind, replaceWith, options, Range);
		}
		public int ReplaceAll(Regex regex, string replaceWith, DocumentRange range) {
			CheckValid();
			Guard.ArgumentNotNull(regex, "regex");
			Guard.ArgumentNotNull(range, "range");
			CheckDocumentRange(range);
			IRegexSearchResult result = StartSearch(regex, range);
			int replacementCount = 0;
			DocumentModel.BeginUpdate();
			try {
				while (result.FindNext()) {
					result.Replace(replaceWith);
					replacementCount++;
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
			return replacementCount;
		}
		public int ReplaceAll(Regex regex, string replaceWith) {
			return ReplaceAll(regex, replaceWith, Range);
		}
		protected internal virtual void CheckDocumentPosition(DocumentPosition documentPosition) {
			NativeDocumentPosition pos = (NativeDocumentPosition)documentPosition;
			if (!Object.ReferenceEquals(pos.Position.PieceTable, PieceTable))
				ThrowDocumentPositionPieceTableMismatch();
		}
		protected internal virtual void CheckDocumentRange(DocumentRange documentRange) {
			NativeDocumentRange range = (NativeDocumentRange)documentRange;
			if (!Object.ReferenceEquals(range.Start.Position.PieceTable, PieceTable))
				ThrowDocumentPositionPieceTableMismatch();
		}
		void CheckSelectionCollection(SelectionCollection collection) {
			if (!Object.ReferenceEquals(collection.Document.PieceTable, PieceTable))
				ThrowDocumentPositionPieceTableMismatch();
		}
		protected internal virtual void ThrowDocumentPositionPieceTableMismatch() {
			RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_DocumentPositionDoesntMatchDocument);
		}
		public DocumentRange InsertDocumentContent(DocumentPosition pos, Stream stream, DocumentFormat format) {
			return InsertDocumentContent(pos, stream, format, InsertOptions.MatchDestinationFormatting);
		}
		public DocumentRange InsertDocumentContent(DocumentPosition pos, Stream stream, DocumentFormat format, InsertOptions insertOptions) {
			return InsertDocumentContent(pos, stream, format, String.Empty, insertOptions);
		}
		public DocumentRange InsertDocumentContent(DocumentPosition pos, Stream stream, DocumentFormat format, string sourceUri) {
			return InsertDocumentContent(pos, stream, format, sourceUri, InsertOptions.MatchDestinationFormatting);
		}
		public DocumentRange InsertDocumentContent(DocumentPosition pos, Stream stream, DocumentFormat format, string sourceUri, InsertOptions insertOptions) {
			return InsertContentCore(pos, new DocumentStreamContentInserter(stream, format, sourceUri, insertOptions));
		}
		public DocumentRange InsertDocumentContent(DocumentPosition pos, string fileName, DocumentFormat format) {
			return InsertDocumentContent(pos, fileName, format, fileName, InsertOptions.MatchDestinationFormatting);
		}
		public DocumentRange InsertDocumentContent(DocumentPosition pos, string fileName, DocumentFormat format, InsertOptions insertOptions) {
			return InsertDocumentContent(pos, fileName, format, fileName, insertOptions);
		}
		public DocumentRange InsertDocumentContent(DocumentPosition pos, string fileName, DocumentFormat format, string sourceUri) {
			return InsertDocumentContent(pos, fileName, format, sourceUri, InsertOptions.MatchDestinationFormatting);
		}
		public DocumentRange InsertDocumentContent(DocumentPosition pos, string fileName, DocumentFormat format, string sourceUri, InsertOptions insertOptions) {
			if (format == DocumentFormat.Undefined)
				format = DocumentModel.AutodetectDocumentFormat(fileName);
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				return InsertContentCore(pos, new DocumentStreamContentInserter(stream, format, sourceUri, insertOptions));
			}
		}
		public DocumentRange InsertDocumentContent(DocumentPosition pos, DocumentRange range) {
			return InsertDocumentContent(pos, range, InsertOptions.MatchDestinationFormatting);
		}
		public DocumentRange InsertDocumentContent(DocumentPosition pos, DocumentRange range, InsertOptions insertOptions) {
			return InsertContentCore(pos, new DocumentRangeContentInserter(range, insertOptions));
		}
		public DocumentRange AppendDocumentContent(Stream stream, DocumentFormat format) {
			return AppendDocumentContent(stream, format, String.Empty);
		}
		public DocumentRange AppendDocumentContent(Stream stream, DocumentFormat format, string sourceUri) {
			return AppendDocumentContent(stream, format, sourceUri, InsertOptions.MatchDestinationFormatting);
		}
		public DocumentRange AppendDocumentContent(Stream stream, DocumentFormat format, string sourceUri, InsertOptions insertOptions) {
			return AppendDocumentContentCore(new DocumentStreamContentInserter(stream, format, sourceUri, insertOptions));
		}
		public DocumentRange AppendDocumentContent(string fileName, DocumentFormat format) {
			return AppendDocumentContent(fileName, format, fileName);
		}
		public DocumentRange AppendDocumentContent(string fileName, DocumentFormat format, string sourceUri) {
			return AppendDocumentContent(fileName, format, sourceUri, InsertOptions.MatchDestinationFormatting);
		}
		public DocumentRange AppendDocumentContent(string fileName, DocumentFormat format, string sourceUri, InsertOptions insertOptions) {
			if (format == DocumentFormat.Undefined)
				format = DocumentModel.AutodetectDocumentFormat(fileName);
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				return AppendDocumentContentCore(new DocumentStreamContentInserter(stream, format, sourceUri, insertOptions));
			}
		}
		public DocumentRange AppendDocumentContent(DocumentRange range) {
			return AppendDocumentContent(range, InsertOptions.MatchDestinationFormatting);
		}
		public DocumentRange AppendDocumentContent(DocumentRange range, InsertOptions insertOptions) {
			return AppendDocumentContentCore(new DocumentRangeContentInserter(range, insertOptions));
		}
		protected internal virtual DocumentRange AppendDocumentContentCore(DocumentContentInserter inserter) {
			inserter.Append = true;
			return InsertContentCore(EndPosition, inserter);
		}
		public void ApplySyntaxHighlight(List<SyntaxHighlightToken> tokens) {
#if DEBUGTEST || DEBUG
			ValidateAPIParagraphs();
#endif
			DocumentModel.BeginUpdate();
			try {
				int count = tokens.Count;
				List<ModelLogPosition> positions = new List<ModelLogPosition>(count);
				int lastPosition = -1;
				for (int i = 0; i < count; i++) {
					SyntaxHighlightToken token = tokens[i];
					if (token.Start != lastPosition)
						positions.Add(new ModelLogPosition(token.Start));
					if (i < count - 1 && !CanMergeTokens(token, tokens[i + 1]))
						positions.Add(new ModelLogPosition(token.End));
					lastPosition = token.End;
				}
				int positionsCount = positions.Count;
#if DEBUTEST || DEBUG
				for (int i = 1; i < positionsCount; i++)
					if (positions[i] <= positions[i - 1])
						Exceptions.ThrowArgumentException("tokens", tokens);
#endif
				while (positionsCount > 0 && positions[positionsCount - 1] >= PieceTable.DocumentEndLogPosition) {
					positions.RemoveAt(positionsCount - 1);
					positionsCount--;
				}
				PieceTable.MultipleSplitTextRun(positions);
				DevExpress.XtraRichEdit.Model.RunIndex runIndex = DevExpress.XtraRichEdit.Model.RunIndex.Zero;
				DevExpress.XtraRichEdit.Model.TextRunCollection runs = PieceTable.Runs;
				DevExpress.XtraRichEdit.Model.RunIndex runCount = new DevExpress.XtraRichEdit.Model.RunIndex(runs.Count);
				int currentPosition = 0;
				for (int i = 0; i < count; i++) {
					SyntaxHighlightToken token = tokens[i];
					int start = token.Start;
					while (currentPosition < start && runIndex < runCount) {
						currentPosition += runs[runIndex].Length;
						runIndex++;
					}
					if (runIndex >= runCount)
						break;
					int end = token.End;
					while (currentPosition < end && runIndex < runCount) {
						token.Properties.ApplyTo(runs[runIndex]);
						currentPosition += runs[runIndex].Length;
						runIndex++;
					}
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		bool CanMergeTokens(SyntaxHighlightToken token, SyntaxHighlightToken nextToken) {
			return token.End == nextToken.Start && token.Properties.Equals(nextToken.Properties);
		}
		void ValidateAPIParagraphs() {
			int count = Paragraphs.Count;
			if (Paragraphs[0].ParagraphIndex != DevExpress.XtraRichEdit.Model.ParagraphIndex.Zero)
				Exceptions.ThrowInternalException();
			for (int i = 1; i < count; i++) {
				if (Paragraphs[i].ParagraphIndex != Paragraphs[i - 1].ParagraphIndex + 1)
					Exceptions.ThrowInternalException();
			}
		}
		public float GetWidthUnitFixedValue(ModelWidthUnit unitValue) {
			if (unitValue.Type == ModelWidthUnitType.ModelUnits)
				return ModelUnitsToUnits(unitValue.Value);
			else
				return 0;
		}
		public void SetWidthUnitFixedValue(ModelWidthUnit unitValue, float value) {
			unitValue.Type = ModelWidthUnitType.ModelUnits;
			unitValue.Value = UnitsToModelUnits(value);
		}
		public float GetWidthValue(ModelWidthUnit unitValue) {
			if (unitValue.Type == ModelWidthUnitType.ModelUnits)
				return ModelUnitsToUnits(unitValue.Value);
			else
				return unitValue.Value;
		}
		public void SetWidthValue(ModelWidthUnit unitValue, float value) {
			if (unitValue.Type == ModelWidthUnitType.ModelUnits)
				unitValue.Value = UnitsToModelUnits(value);
			else
				unitValue.Value = (int)Math.Round(value);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraphCollection.AddParagraphToList(Paragraph paragraph, NumberingList list, int levelIndex)' method instead.")]
		public void AddParagraphToList(Paragraph paragraph, NumberingList list, int levelIndex) {
			AddParagraphsToList(paragraph.Range, list, levelIndex);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraphCollection.AddParagraphToList(Paragraph paragraph, int numberingListIndex, int levelIndex)' method instead.")]
		public void AddParagraphToList(Paragraph paragraph, int numberingListIndex, int levelIndex) {
			AddParagraphsToList(paragraph.Range, MainDocument.NumberingLists[numberingListIndex], levelIndex);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraphCollection.AddParagraphsToList(DocumentRange range, NumberingList list, int levelIndex)' method instead.")]
		public void AddParagraphsToList(DocumentRange range, NumberingList list, int levelIndex) {
			CheckValid();
			CheckDocumentRange(range);
			DocumentModel.BeginUpdate();
			try {
				NativeParagraphCollection paragraphs = (NativeParagraphCollection)GetParagraphs(range);
				DevExpress.XtraRichEdit.Model.NumberingList innerNumberingList = ((NativeNumberingList)list).InnerNumberingList;
				int index = ((NativeNumberingListCollection)(MainDocument.NumberingLists)).InnerLists.IndexOf(innerNumberingList);
				if (index < 0)
					throw new InvalidOperationException(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_NumberingListNotInListCollection));
				Model.NumberingListIndex numberingListIndex = new Model.NumberingListIndex(index);
				for (int i = 0; i < paragraphs.Count; i++) {
					NativeParagraph paragraph = paragraphs[i];
					DevExpress.XtraRichEdit.Model.Paragraph modelParagraph = paragraph.InnerParagraph;
					if (modelParagraph.IsInList())
						modelParagraph.PieceTable.RemoveNumberingFromParagraph(modelParagraph);
					this.PieceTable.AddNumberingListToParagraph(modelParagraph, numberingListIndex, levelIndex);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraphCollection.RemoveNumberingFromParagraph(Paragraph paragraph)' method instead.")]
		public void RemoveNumberingFromParagraph(Paragraph paragraph) {
			RemoveNumberingFromParagraphs(paragraph.Range);
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeParagraphCollection.RemoveNumberingFromParagraphs(DocumentRange range)' method instead.")]
		public void RemoveNumberingFromParagraphs(DocumentRange range) {
			CheckValid();
			CheckDocumentRange(range);
			DocumentModel.BeginUpdate();
			try {
				NativeParagraphCollection paragraphs = (NativeParagraphCollection)GetParagraphs(range);
				for (int i = 0; i < paragraphs.Count; i++) {
					NativeParagraph paragraph = paragraphs[i];
					DevExpress.XtraRichEdit.Model.Paragraph modelParagraph = paragraph.InnerParagraph;
					if (modelParagraph.IsInList())
						modelParagraph.PieceTable.RemoveNumberingFromParagraph(modelParagraph);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public bool MakeAllTablesWordCompatible() {
			bool modified = false;
			foreach (Table table in tables) {
				modified |= table.MakeWordCompatible();
			}
			return modified;
		}
		public SubDocumentType GetSubDocumentType() {
			if (pieceTable.IsMain)
				return SubDocumentType.Main;
			if (pieceTable.IsHeader)
				return SubDocumentType.Header;
			if (pieceTable.IsFooter)
				return SubDocumentType.Footer;
			if (pieceTable.IsFootNote)
				return SubDocumentType.FootNote;
			if (pieceTable.IsEndNote)
				return SubDocumentType.EndNote;
			if (pieceTable.IsTextBox)
				return SubDocumentType.TextBox;
			if (pieceTable.IsComment)
				return SubDocumentType.Comment;
			throw new NotImplementedException();
		}
	}
	#endregion
	#region TextFragmentOptions
	public class TextFragmentOptions {
		bool preserveOriginalNumbering;
		bool allowExtendingDocumentRange = true;
		public bool PreserveOriginalNumbering { get { return preserveOriginalNumbering; } set { preserveOriginalNumbering = value; } }
		public bool AllowExtendingDocumentRange { get { return allowExtendingDocumentRange; } set { allowExtendingDocumentRange = value; } }
	}
	#endregion
	#region NativeDocument
	public class NativeDocument : NativeSubDocument, Document {
		#region Fields
		NativeSubDocument activeSubDocument;
		NativeSectionCollection sections;
		NativeCharacterStyleCollection characterStyles;
		NativeParagraphStyleCollection paragraphStyles;
		NativeTableStyleCollection tableStyles;
		CharacterPropertiesBase defaultCharacterProperties;
		ParagraphPropertiesBase defaultParagraphProperties;
		TablePropertiesBase defaultTableProperties;
		TableCellPropertiesBase defaultTableCellProperties;
		NativeAbstractNumberingListCollection abstractNumberingLists;
		NativeNumberingListCollection numberingLists;
		#endregion
		protected internal NativeDocument(ModelPieceTable pieceTable, InnerRichEditDocumentServer server)
			: base(pieceTable, server) {
			UpdateActiveSubDocument();
			DocumentModel.SelectionReseted += DocumentModel_SelectionReseted;
		}
		void DocumentModel_SelectionReseted(object sender, EventArgs e) {
			ResetSelectionsCollection();
		}
		#region Properties
		public bool IsDocumentProtected { get { return DocumentModel.ProtectionProperties.EnforceProtection; } }
		public bool DifferentOddAndEvenPages {
			get {
				CheckValid();
				return DocumentModel.DocumentProperties.DifferentOddAndEvenPages;
			}
			set {
				CheckValid();
				DocumentModel.DocumentProperties.DifferentOddAndEvenPages = value;
			}
		}
		public UpdateDocVariablesBeforePrint UpdateDocVariablesBeforePrint {
			get {
				CheckValid();
				return DocumentModel.DocumentProperties.UpdateDocVariablesBeforePrint;
			}
			set {
				CheckValid();
				DocumentModel.DocumentProperties.UpdateDocVariablesBeforePrint = value;
			}
		}
		public ParagraphPropertiesBase DefaultParagraphProperties {
			get {
				CheckValid();
				return defaultParagraphProperties;
			}
		}
		public CharacterPropertiesBase DefaultCharacterProperties {
			get {
				CheckValid();
				return defaultCharacterProperties;
			}
		}
		public TablePropertiesBase DefaultTableProperties {
			get {
				CheckValid();
				return defaultTableProperties;
			}
		}
		public TableCellPropertiesBase DefaultTableCellProperties {
			get {
				CheckValid();
				return defaultTableCellProperties;
			}
		}
		public NativeSubDocument ActiveSubDocument { get { return activeSubDocument; } }
		public SectionCollection Sections { get { return sections; } }
		public CharacterStyleCollection CharacterStyles { get { return characterStyles; } }
		public ParagraphStyleCollection ParagraphStyles { get { return paragraphStyles; } }
		public TableStyleCollection TableStyles { get { return tableStyles; } }
		public NumberingListCollection NumberingLists { get { return numberingLists; } }
		public AbstractNumberingListCollection AbstractNumberingLists { get { return abstractNumberingLists; } }
		public override int ReferenceCount { get { return 1; } set { } }
		public string RtfText {
			get { return InternalAPI.RtfText; }
			set {
				UnsubscribeInternalAPIEventsCore();
				try {
					InternalAPI.RtfText = value;
				}
				finally {
					SubscribeInternalAPIEventsCore();
				}
			}
		}
		public string HtmlText {
			get { return InternalAPI.HtmlText; }
			set {
				UnsubscribeInternalAPIEventsCore();
				try {
					InternalAPI.HtmlText = value;
				}
				finally {
					SubscribeInternalAPIEventsCore();
				}
			}
		}
		public string MhtText {
			get { return InternalAPI.MhtText; }
			set {
				UnsubscribeInternalAPIEventsCore();
				try {
					InternalAPI.MhtText = value;
				}
				finally {
					SubscribeInternalAPIEventsCore();
				}
			}
		}
		public string WordMLText {
			get { return InternalAPI.WordMLText; }
			set {
				UnsubscribeInternalAPIEventsCore();
				try {
					InternalAPI.WordMLText = value;
				}
				finally {
					SubscribeInternalAPIEventsCore();
				}
			}
		}
		public string XamlText {
			get { return InternalAPI.XamlText; }
			set {
				UnsubscribeInternalAPIEventsCore();
				try {
					InternalAPI.XamlText = value;
				}
				finally {
					SubscribeInternalAPIEventsCore();
				}
			}
		}
		public string Text {
			get { return InternalAPI.Text; }
			set {
				UnsubscribeInternalAPIEventsCore();
				try {
					InternalAPI.Text = value;
				}
				finally {
					SubscribeInternalAPIEventsCore();
				}
			}
		}
		public byte[] OpenXmlBytes {
			get { return InternalAPI.OpenXmlBytes; }
			set {
				UnsubscribeInternalAPIEventsCore();
				try {
					InternalAPI.OpenXmlBytes = value;
				}
				finally {
					SubscribeInternalAPIEventsCore();
				}
			}
		}
		public byte[] OpenDocumentBytes {
			get { return InternalAPI.OpenDocumentBytes; }
			set {
				UnsubscribeInternalAPIEventsCore();
				try {
					InternalAPI.OpenDocumentBytes = value;
				}
				finally {
					SubscribeInternalAPIEventsCore();
				}
			}
		}
		public DocumentRange Selection {
			get { return GetSelection(); }
			set { SetSelection((NativeDocumentRange)value); }
		}
		SelectionCollection selections;
		public SelectionCollection Selections {
			get {
				if (selections == null)
					selections = new SelectionCollection(DocumentModel.Selection, this);
				return selections;
			}
		}
		public DocumentPosition CaretPosition {
			get { return GetCaretPosition(); }
			set { SetCaretPosition((NativeDocumentPosition)value); }
		}
		public DocumentVariableCollection Variables { get { return DocumentModel.Variables; } }
		public bool IsEmpty { get { return DocumentModel.IsEmpty; } }
		#endregion
		#region Events
		#region ContentChanged
		public event EventHandler ContentChanged { add { DocumentServer.ContentChanged += value; } remove { DocumentServer.ContentChanged -= value; } }
		#endregion
		#region ModifiedChanged
		public event EventHandler ModifiedChanged { add { DocumentServer.ModifiedChanged += value; } remove { DocumentServer.ModifiedChanged -= value; } }
		#endregion
		#region CalculateDocumentVariable
		public event CalculateDocumentVariableEventHandler CalculateDocumentVariable { add { DocumentServer.CalculateDocumentVariable += value; } remove { DocumentServer.CalculateDocumentVariable -= value; } }
		#endregion
		#endregion
		private void ResetSelectionsCollection() {
			if (this.selections == null)
				return;
			this.selections.Clear();
			this.selections = null;
		}
		protected internal override void CreateApiObjects() {
			base.CreateApiObjects();
			this.sections = CreateNativeSections();
		}
		protected virtual NativeSectionCollection CreateNativeSections() {
			return new NativeSectionCollection();
		}
		protected internal override void Initialize() {
			base.Initialize();
			this.characterStyles = new NativeCharacterStyleCollection(this);
			this.paragraphStyles = new NativeParagraphStyleCollection(this);
			this.tableStyles = new NativeTableStyleCollection(this);
			this.defaultParagraphProperties = new NativeDefaultParagraphProperties(this, DocumentModel.DefaultParagraphProperties);
			this.defaultCharacterProperties = new NativeDefaultCharacterProperties(DocumentModel.DefaultCharacterProperties);
			this.defaultTableProperties = new NativeDefaultTableProperties(this, DocumentModel.DefaultTableProperties);
			this.defaultTableCellProperties = new NativeDefaultTableCellProperties(this, DocumentModel.DefaultTableCellProperties);
			this.abstractNumberingLists = new NativeAbstractNumberingListCollection(this);
			this.numberingLists = new NativeNumberingListCollection(this);
			PopulateSections();
		}
		protected internal override void Invalidate() {
			base.Invalidate();
			sections.Clear();
			characterStyles.Invalidate();
			paragraphStyles.Invalidate();
			tableStyles.Invalidate();
			abstractNumberingLists.Invalidate();
			numberingLists.Invalidate();
		}
		protected internal virtual void PopulateSections() {
			this.sections.Clear();
			DocumentModel.Sections.ForEach(RegisterSection);
		}
		protected internal virtual void RegisterSection(ModelSection section) {
			NativeSection nativeSection = CreateNativeSection(section);
			this.sections.Add(nativeSection);
		}
		protected virtual NativeSection CreateNativeSection(ModelSection section) {
			return new NativeSection(this, section);
		}
		protected internal override void SubscribeInternalAPIEvents() {
			SubscribeInternalAPIEventsCore();
			InternalAPI.DocumentReplaced += OnDocumentReplaced;
		}
		protected internal override void UnsubscribeInternalAPIEvents() {
			UnsubscribeInternalAPIEventsCore();
			InternalAPI.DocumentReplaced -= OnDocumentReplaced;
		}
		protected internal virtual void UnsubscribeInternalAPIEventsCore() {
			base.UnsubscribeInternalAPIEvents();
			InternalAPI.SectionInserted -= OnSectionInserted;
			InternalAPI.SectionRemoved -= OnSectionRemoved;
			DocumentModel.InnerDocumentCleared -= OnDocumentCleared;
			DocumentModel.EndDocumentUpdate -= OnEndDocumentUpdate;
		}
		protected internal virtual void SubscribeInternalAPIEventsCore() {
			base.SubscribeInternalAPIEvents();
			InternalAPI.SectionInserted += OnSectionInserted;
			InternalAPI.SectionRemoved += OnSectionRemoved;
			DocumentModel.InnerDocumentCleared += OnDocumentCleared;
			DocumentModel.EndDocumentUpdate += OnEndDocumentUpdate;
		}
		void OnDocumentCleared(object sender, EventArgs e) {
			Initialize();
		}
		void OnEndDocumentUpdate(object sender, DevExpress.XtraRichEdit.Model.DocumentUpdateCompleteEventArgs e) {
			if ((e.DeferredChanges.ChangeActions & DevExpress.XtraRichEdit.Model.DocumentModelChangeActions.ActivePieceTableChanged) != 0)
				UpdateActiveSubDocument();
		}
		void OnDocumentReplaced(object sender, EventArgs e) {
			Initialize();
		}
		void OnSectionInserted(object sender, DevExpress.XtraRichEdit.Model.SectionEventArgs e) {
			int sectionIndex = SectionIndexToInt(e.SectionIndex);
			NativeSection section = CreateNativeSection(DocumentModel.Sections[e.SectionIndex]);
			this.sections.Insert(sectionIndex, section);
		}
		void OnSectionRemoved(object sender, DevExpress.XtraRichEdit.Model.SectionEventArgs e) {
			int sectionIndex = SectionIndexToInt(e.SectionIndex);
			NativeSection section = sections[sectionIndex];
			section.IsValid = false;
			sections.RemoveAt(sectionIndex);
		}
		public void LoadDocument(Stream stream, DocumentFormat format) {
			LoadDocument(stream, format, String.Empty);
		}
		public void LoadDocument(Stream stream, DocumentFormat format, string sourceUri) {
			LoadDocumentCore(stream, format, sourceUri);
		}
		public void LoadDocument(string fileName, DocumentFormat format) {
			LoadDocument(fileName, format, String.Empty);
		}
		public void LoadDocument(string fileName, DocumentFormat format, string sourceUri) {
			if (format == DocumentFormat.Undefined)
				format = DocumentModel.AutodetectDocumentFormat(fileName);
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				LoadDocumentCore(stream, format, sourceUri);
			}
		}
		protected internal virtual void LoadDocumentCore(Stream stream, DocumentFormat format, string sourceUri) {
			IDocumentImportManagerService importManagerService = DocumentModel.GetService<IDocumentImportManagerService>();
			if (importManagerService == null)
				throw new InvalidOperationException("Could not find service: IDocumentImportManagerService");
			DocumentImportHelper importHelper = new DocumentImportHelper(DocumentModel);
			importHelper.Import(stream, format, sourceUri, importManagerService);
		}
		public void SaveDocument(Stream stream, DocumentFormat format) {
			SaveDocumentCore(stream, format, String.Empty);
		}
		public void SaveDocumentCore(Stream stream, DocumentFormat format, string targetUri) {
			InnerRichEditDocumentServer.SaveDocumentCore(DocumentModel, stream, format, targetUri);
		}
		public void SaveDocument(string fileName, DocumentFormat format) {
			SaveDocument(DocumentModel, fileName, format);
		}
		protected static void SaveDocument(DocumentModel documentModel, string fileName, DocumentFormat format) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				InnerRichEditDocumentServer.SaveDocumentCore(documentModel, stream, format, fileName);
			}
		}
		public Section InsertSection(DocumentPosition pos) {
			return InsertSectionCore(pos, false);
		}
		public Section AppendSection() {
			return InsertSectionCore(EndPosition, true);
		}
		Section InsertSectionCore(DocumentPosition pos, bool append) {
			CheckDocumentPosition(pos);
			NativeDocumentPosition nativePosition = (NativeDocumentPosition)pos;
			ModelLogPosition logPosition = NormalizeLogPosition(nativePosition.Position.LogPosition);
			int sectionIndex = SectionIndexToInt(DocumentModel.FindSectionIndex(logPosition));
			DocumentModel.InsertSection(logPosition);
			if (append)
				sectionIndex = Sections.Count != 0 ? Sections.Count - 1 : sectionIndex;
			return Sections[sectionIndex];
		}
		public void ChangeActiveDocument(SubDocument document) {
			IRichEditControl control = DocumentServer.Owner as IRichEditControl;
			if (control == null)
				return;
			ModelPieceTable pieceTable = ((NativeSubDocument)document).PieceTable;
			if (Object.ReferenceEquals(DocumentModel.ActivePieceTable, pieceTable))
				return;
			ChangeActivePieceTable(control, pieceTable);
			UpdateActiveSubDocument();
		}
		void ChangeActivePieceTable(IRichEditControl control, ModelPieceTable pieceTable) {
			if (pieceTable.IsMain || pieceTable.IsTextBox || pieceTable.IsComment) {
				ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(control, pieceTable, null, -1);
				command.ForceExecute(command.CreateDefaultCommandUIState());
			}
			else if (pieceTable.IsHeader) {
				MakeNearestHeaderActiveCommand command = new MakeNearestHeaderActiveCommand(control, (DevExpress.XtraRichEdit.Model.SectionHeader)pieceTable.ContentType);
				command.ForceExecute(command.CreateDefaultCommandUIState());
			}
			else if (pieceTable.IsFooter) {
				MakeNearestFooterActiveCommand command = new MakeNearestFooterActiveCommand(control, (DevExpress.XtraRichEdit.Model.SectionFooter)pieceTable.ContentType);
				command.ForceExecute(command.CreateDefaultCommandUIState());
			}
		}
		void UpdateActiveSubDocument() {
			if (activeSubDocument != null)
				this.activeSubDocument.ReferenceCount--;
			this.activeSubDocument = GetActiveSubDocument();
			this.activeSubDocument.ReferenceCount++;
		}
		NativeSubDocument GetActiveSubDocument() {
			if (DocumentModel.ActivePieceTable.IsMain)
				return this;
			if (ShouldCreateNewSubDocument())
				return CreateNativeSubDocument(DocumentModel.ActivePieceTable.ContentType.PieceTable, DocumentServer);
			else
				return this;
		}
		protected virtual NativeSubDocument CreateNativeSubDocument(ModelPieceTable pieceTable, InnerRichEditDocumentServer server) {
			return new NativeSubDocument(pieceTable, server);
		}
		bool ShouldCreateNewSubDocument() {
			return DocumentModel.ActivePieceTable.ContentType is DevExpress.XtraRichEdit.Model.SectionHeaderFooterBase ||
				DocumentModel.ActivePieceTable.ContentType is DevExpress.XtraRichEdit.Model.TextBoxContentType ||
				DocumentModel.ActivePieceTable.ContentType is DevExpress.XtraRichEdit.Model.FootNote ||
				DocumentModel.ActivePieceTable.ContentType is DevExpress.XtraRichEdit.Model.EndNote ||
				DocumentModel.ActivePieceTable.ContentType is DevExpress.XtraRichEdit.Model.CommentContentType;
		}
		protected internal NativeDocumentRange GetSelection() {
			CheckValid();
			ModelRunInfo rangeInfo = DocumentModel.Selection.Interval;
			return new NativeDocumentRange(this.activeSubDocument, rangeInfo.NormalizedStart, rangeInfo.NormalizedEnd);
		}
		protected internal void SetSelection(NativeDocumentRange value) {
			CheckValid();
			Guard.ArgumentNotNull(value, "Selection");
			this.activeSubDocument.CheckDocumentRange(value);
			SetSelectionCore(value.Start.LogPosition, value.End.LogPosition);
		}
		protected internal virtual NativeDocumentPosition GetCaretPosition() {
			CheckValid();
			ModelRunInfo rangeInfo = DocumentModel.Selection.Interval;
			return new NativeDocumentPosition(this.activeSubDocument, rangeInfo.End);
		}
		protected internal virtual void SetCaretPosition(NativeDocumentPosition pos) {
			CheckValid();
			Guard.ArgumentNotNull(pos, "CaretPosition");
			this.activeSubDocument.CheckDocumentPosition(pos);
			ModelLogPosition logPosition = pos.LogPosition;
			ModelLogPosition maxLogPosition = this.activeSubDocument.PieceTable.DocumentEndLogPosition;
			if (logPosition >= maxLogPosition)
				logPosition = maxLogPosition;
			SetSelectionCore(logPosition, logPosition);
		}
		public void Copy(DocumentRange range) {
			Guard.ArgumentNotNull(range, "range");
			CheckDocumentRange(range);
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection selection = GetSelectionRangeCollection(range);
			if (selection == null)
				return;
			CopySelectionManager manager = new CopySelectionManager(DocumentServer);
			manager.CopyDocumentRange(PieceTable, selection);
		}
		public void Copy() {
			IRichEditControl control = DocumentServer.Owner as IRichEditControl;
			if (control == null)
				return;
			CopySelectionCommand command = new CopySelectionCommand(control);
			command.ForceExecute(command.CreateDefaultCommandUIState());
		}
		public void Cut(DocumentRange range) {
			DocumentModel.BeginUpdate();
			try {
				Copy(range);
				Delete(range);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void Cut() {
			IRichEditControl control = DocumentServer.Owner as IRichEditControl;
			if (control == null)
				return;
			CutSelectionCommand command = new CutSelectionCommand(control);
			command.ForceExecute(command.CreateDefaultCommandUIState());
		}
		public void Paste() {
			IRichEditControl control = DocumentServer.Owner as IRichEditControl;
			if (control == null)
				return;
			PasteSelectionCommand command = new PasteSelectionCommand(control);
			command.ForceExecute(command.CreateDefaultCommandUIState());
		}
		public void Paste(DocumentFormat format) {
			IRichEditControl control = DocumentServer.Owner as IRichEditControl;
			if (control == null)
				return;
			PasteSelectionCommand command = new PasteSelectionCommand(control);
			command.Format = format;
			command.ForceExecute(command.CreateDefaultCommandUIState());
		}
		public DocumentExportCapabilities RequiredExportCapabilities {
			get {
				DevExpress.XtraRichEdit.Model.DocumentExportCapabilitiesCalculator calculator;
				calculator = new DevExpress.XtraRichEdit.Model.DocumentExportCapabilitiesCalculator(DocumentModel);
				return calculator.Calculate();
			}
		}
		public Section GetSection(DocumentPosition pos) {
			NativeSubDocument subDocument = GetSubDocument(pos);
			DevExpress.XtraRichEdit.Model.SectionHeaderFooterBase headerFooter = subDocument.PieceTable.ContentType as DevExpress.XtraRichEdit.Model.SectionHeaderFooterBase;
			ModelSectionIndex sectionIndex;
			if (headerFooter != null)
				sectionIndex = headerFooter.GetSectionIndex();
			else {
				CheckDocumentPosition(pos);
				sectionIndex = DocumentModel.FindSectionIndex(pos.LogPosition, false);
			}
			int index = ((IConvertToInt<ModelSectionIndex>)sectionIndex).ToInt();
			if (index < 0)
				return null;
			return Sections[index];
		}
		private NativeSubDocument GetSubDocument(DocumentPosition pos) {
			NativeDocumentPosition nativePosition = pos as NativeDocumentPosition;
			if (nativePosition != null)
				return nativePosition.Document;
			else
				return activeSubDocument;
		}
		public MailMergeOptions CreateMailMergeOptions() {
			return DocumentServer.CreateMailMergeOptions();
		}
		public void MailMerge(string fileName, DocumentFormat format) {
			DocumentServer.MailMerge(fileName, format);
		}
		public void MailMerge(Stream stream, DocumentFormat format) {
			DocumentServer.MailMerge(stream, format);
		}
		public void MailMerge(Document document) {
			DocumentServer.MailMerge(document);
		}
		public void MailMerge(MailMergeOptions options, string fileName, DocumentFormat format) {
			DocumentServer.MailMerge(options, fileName, format);
		}
		public void MailMerge(MailMergeOptions options, Stream stream, DocumentFormat format) {
			DocumentServer.MailMerge(options, stream, format);
		}
		public void MailMerge(MailMergeOptions options, Document targetDocument) {
			DocumentServer.MailMerge(options, targetDocument);
		}
		public void Protect(string password) {
			DocumentModel.EnforceDocumentProtection(password);
		}
		public void Unprotect() {
			DocumentModel.ForceRemoveDocumentProtection();
		}
		public void RemoveTable(Table deletedTable) {
			Tables.Remove(deletedTable);
		}
		public virtual List<string> GetAuthors() {
			return DocumentModel.GetAuthors();
		}
	}
	#endregion
	#region NativeSearchResultBase (abstract class)
	public abstract class NativeSearchResultBase : ISearchResult {
		#region Fields
		readonly string findText;
		readonly SearchOptions options;
		readonly DocumentRange range;
		readonly NativeSubDocument document;
		readonly TextSearchBase textSearch;
		DocumentRange currenResult;
		ModelLogPosition startPosition;
		ModelLogPosition endPosition;
		#endregion
		protected NativeSearchResultBase(NativeSubDocument document, string findText, SearchOptions searchOptions, DocumentRange range) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(range, "range");
			this.document = document;
			if (String.IsNullOrEmpty(findText))
				Exceptions.ThrowArgumentException("findText", findText);
			this.findText = findText;
			this.options = searchOptions;
			this.range = range;
			this.textSearch = CreateTextSearch();
			SetInitialState();
		}
		#region Properties
		public DocumentRange CurrentResult { get { return currenResult; } }
		protected ModelLogPosition StartPosition { get { return startPosition; } set { startPosition = value; } }
		protected ModelLogPosition EndPosition { get { return endPosition; } set { endPosition = value; } }
		protected DocumentRange Range { get { return range; } }
		protected string FindText { get { return findText; } }
		protected SearchOptionsInternal SearchOptions { get { return (SearchOptionsInternal)options; } }
		protected NativeSubDocument Document { get { return document; } }
		#endregion
		protected void SetInitialState() {
			this.currenResult = null;
			this.startPosition = ModelLogPosition.MinValue;
			this.endPosition = ModelLogPosition.MaxValue;
		}
		public virtual bool FindNext() {
			UpdateSearchInterval();
			if (this.startPosition > this.endPosition)
				return false;
			ModelRunInfo runInfo = textSearch.Match(FindText, SearchOptions, startPosition, endPosition).Value;
			if (runInfo == null)
				return false;
			this.currenResult = CreateDocumentRange(runInfo);
			ChangeSearchInterval(runInfo);
			return true;
		}
		public virtual void Replace(string replaceWith) {
			Guard.ArgumentNotNull(replaceWith, "replaceWith");
			if (CurrentResult == null || CurrentResult.Length == 0)
				return;
			int deltaLength = Document.Replace(CurrentResult, replaceWith);
			ChangeSearchIntervalAfterReplace(deltaLength);
		}
		protected virtual DocumentRange CreateDocumentRange(ModelRunInfo rangeInfo) {
			NativeDocumentPosition position = new NativeDocumentPosition(Document, rangeInfo.Start);
			return this.document.CreateRange(position, rangeInfo.End.LogPosition - rangeInfo.Start.LogPosition);
		}
		public void Reset() {
			SetInitialState();
		}
		protected abstract void UpdateSearchInterval();
		protected abstract TextSearchBase CreateTextSearch();
		protected abstract void ChangeSearchInterval(ModelRunInfo rangeInfo);
		protected abstract void ChangeSearchIntervalAfterReplace(int deltaLength);
	}
	#endregion
	#region NativeSearchResultForward
	public class NativeSearchResultForward : NativeSearchResultBase {
		public NativeSearchResultForward(NativeSubDocument document, string findText, SearchOptions searchOptions, DocumentRange range)
			: base(document, findText, searchOptions, range) {
		}
		protected override void UpdateSearchInterval() {
			StartPosition = Algorithms.Max(StartPosition, Range.Start.LogPosition);
			EndPosition = Algorithms.Min(Range.End.LogPosition, Document.PieceTable.DocumentEndLogPosition);
		}
		protected override void ChangeSearchInterval(ModelRunInfo rangeInfo) {
			StartPosition = rangeInfo.Start.LogPosition + 1;
		}
		protected override TextSearchBase CreateTextSearch() {
			return new TextSearchByStringForward(Document.PieceTable);
		}
		protected override void ChangeSearchIntervalAfterReplace(int deltaLength) {
			StartPosition = StartPosition + deltaLength;
		}
	}
	#endregion
	#region NativeSearchResultBackward
	public class NativeSearchResultBackward : NativeSearchResultBase {
		public NativeSearchResultBackward(NativeSubDocument document, string findText, SearchOptions searchOptions, DocumentRange range)
			: base(document, findText, searchOptions, range) {
		}
		protected override void UpdateSearchInterval() {
			StartPosition = Range.Start.LogPosition;
			ModelLogPosition initialEnd = Algorithms.Min(Range.End.LogPosition, Document.PieceTable.DocumentEndLogPosition);
			EndPosition = Algorithms.Min(EndPosition, initialEnd);
		}
		protected override void ChangeSearchInterval(ModelRunInfo rangeInfo) {
			EndPosition = rangeInfo.End.LogPosition - 1;
		}
		protected override TextSearchBase CreateTextSearch() {
			return new TextSearchByStringBackward(Document.PieceTable);
		}
		protected override void ChangeSearchIntervalAfterReplace(int deltaLength) {
			StartPosition = CurrentResult.Start.LogPosition - 1;
		}
	}
	#endregion
	#region DocumentContentInserter (abstract class)
	public abstract class DocumentContentInserter {
		bool append;
		bool resetMerging;
		protected DocumentContentInserter(bool resetMerging) {
			this.resetMerging = resetMerging;
		}
		protected DocumentContentInserter()
			: this(false) {
		}
		public bool Append { get { return append; } set { append = value; } }
		public abstract bool CanInsertContent();
		public bool InsertContent(ModelPieceTable pieceTable, ModelLogPosition logPosition) {
			if (!CanInsertContent())
				return false;
			pieceTable.DocumentModel.BeginUpdate();
			try {
				if (Append) {
					pieceTable.InsertParagraph(pieceTable.DocumentEndLogPosition);
					bool result = InsertContentCore(pieceTable, logPosition + 1);
					CopyFormatting(pieceTable);
					ModelParagraph lastParagraph = pieceTable.Paragraphs.Last;
					pieceTable.DeleteContent(lastParagraph.LogPosition - 1, 1, false);
					return result;
				}
				else {
					if (this.resetMerging)
						pieceTable.DocumentModel.ResetMerging();
					return InsertContentCore(pieceTable, logPosition);
				}
			}
			finally {
				pieceTable.DocumentModel.EndUpdate();
			}
		}
		void CopyFormatting(ModelPieceTable pieceTable) {
			DevExpress.XtraRichEdit.Model.TextRunCollection runs = pieceTable.Runs;
			DevExpress.XtraRichEdit.Model.TextRunBase lastRun = runs.Last;
			DevExpress.XtraRichEdit.Model.TextRunBase previosToLastRun = runs[new ModelRunIndex(runs.Count - 2)];
			lastRun.CharacterProperties.CopyFrom(previosToLastRun.CharacterProperties);
			lastRun.CharacterStyleIndex = previosToLastRun.CharacterStyleIndex;
			DevExpress.XtraRichEdit.Model.ParagraphCollection paragraphs = pieceTable.Paragraphs;
			ModelParagraph lastParagraph = paragraphs.Last;
			ModelParagraph previosToLastParagraph = paragraphs[new ModelParagraphIndex(paragraphs.Count - 2)];
			lastParagraph.ParagraphProperties.CopyFrom(previosToLastParagraph.ParagraphProperties);
			lastParagraph.ParagraphStyleIndex = previosToLastParagraph.ParagraphStyleIndex;
		}
		public abstract bool InsertContentCore(ModelPieceTable pieceTable, ModelLogPosition logPosition);
	}
	#endregion
	#region DocumentTextContentInserter
	public class DocumentTextContentInserter : DocumentContentInserter {
		readonly string text;
		public DocumentTextContentInserter(string text, bool resetMerging)
			: base(resetMerging) {
			this.text = text;
		}
		public DocumentTextContentInserter(string text) {
			this.text = text;
		}
		public override bool CanInsertContent() {
			return !String.IsNullOrEmpty(text);
		}
		public override bool InsertContentCore(ModelPieceTable pieceTable, ModelLogPosition logPosition) {
			pieceTable.InsertPlainText(logPosition, text);
			return true;
		}
	}
	#endregion
	#region DocumentSingleLineTextContentInserter
	public class DocumentSingleLineTextContentInserter : DocumentContentInserter {
		readonly string text;
		public DocumentSingleLineTextContentInserter(string text) {
			this.text = text;
		}
		public override bool CanInsertContent() {
			return !String.IsNullOrEmpty(text);
		}
		public override bool InsertContentCore(ModelPieceTable pieceTable, ModelLogPosition logPosition) {
			pieceTable.InsertText(logPosition, text);
			return true;
		}
	}
	#endregion
	#region FormattedDocumentContentInserter (abstract class)
	public abstract class FormattedDocumentContentInserter : DocumentContentInserter {
		readonly string text;
		readonly InsertOptions insertOptions;
		protected FormattedDocumentContentInserter(string text, InsertOptions insertOptions) {
			this.text = text;
			this.insertOptions = insertOptions;
		}
		public override bool CanInsertContent() {
			return !String.IsNullOrEmpty(text);
		}
		public override bool InsertContentCore(ModelPieceTable pieceTable, ModelLogPosition logPosition) {
			pieceTable.DocumentModel.BeginUpdate();
			try {
				Model.PieceTablePasteTextContentConvertedToDocumentModelCommandBase command = CreateCommand(pieceTable, insertOptions);
				command.PasteContent(new DevExpress.XtraRichEdit.Model.ClipboardStringContent(text), logPosition, null);
			}
			finally {
				pieceTable.DocumentModel.EndUpdate();
			}
			return true;
		}
		protected abstract Model.PieceTablePasteTextContentConvertedToDocumentModelCommandBase CreateCommand(ModelPieceTable pieceTable, InsertOptions insertOptions);
	}
	#endregion
	#region DocumentRtfTextContentInserter
	public class DocumentRtfTextContentInserter : FormattedDocumentContentInserter {
		public DocumentRtfTextContentInserter(string text, InsertOptions insertOptions)
			: base(text, insertOptions) {
		}
		protected override Model.PieceTablePasteTextContentConvertedToDocumentModelCommandBase CreateCommand(ModelPieceTable pieceTable, InsertOptions insertOptions) {
			return new Model.PieceTablePasteRtfTextCommand(pieceTable, insertOptions);
		}
	}
	#endregion
	#region DocumentHtmlTextContentInserter
	public class DocumentHtmlTextContentInserter : FormattedDocumentContentInserter {
		public DocumentHtmlTextContentInserter(string text, InsertOptions insertOptions)
			: base(text, insertOptions) {
		}
		protected override Model.PieceTablePasteTextContentConvertedToDocumentModelCommandBase CreateCommand(ModelPieceTable pieceTable, InsertOptions insertOptions) {
			return new Model.PieceTablePasteHtmlTextCommand(pieceTable, insertOptions);
		}
	}
	#endregion
	#region DocumentModelContentInserter (abstract class)
	public abstract class DocumentModelContentInserter : DocumentContentInserter {
		#region Fields
		InsertOptions insertOptions;
		#endregion
		protected DocumentModelContentInserter(InsertOptions insertOptions) {
			this.insertOptions = insertOptions;
		}
		#region Properties
		public InsertOptions InsertOptions { get { return insertOptions; } }
		protected abstract bool CopyLastParagraph { get; }
		#endregion
		public override bool CanInsertContent() {
			return true;
		}
		public override bool InsertContentCore(ModelPieceTable pieceTable, ModelLogPosition logPosition) {
			DocumentModel documentModel = CreateDocumentModel(pieceTable.DocumentModel);
			if (documentModel == null)
				return false;
			using (documentModel) {
				documentModel.IntermediateModel = true;
				DevExpress.XtraRichEdit.Model.PieceTableInsertContentConvertedToDocumentModelCommand command;
				command = new DevExpress.XtraRichEdit.Model.PieceTableInsertContentConvertedToDocumentModelCommand(pieceTable, documentModel, logPosition, InsertOptions, false);
				command.CopyLastParagraph = CopyLastParagraph;
				command.Execute();
			}
			return true;
		}
		protected internal abstract DocumentModel CreateDocumentModel(DocumentModel targetDocumentModel);
	}
	#endregion
	#region DocumentStreamContentInserter
	public class DocumentStreamContentInserter : DocumentModelContentInserter {
		readonly Stream stream;
		readonly DocumentFormat format;
		readonly string sourceUri;
		public DocumentStreamContentInserter(Stream stream, DocumentFormat format, string sourceUri, InsertOptions insertOptions)
			: base(insertOptions) {
			this.stream = stream;
			this.format = format;
			this.sourceUri = sourceUri;
		}
		protected override bool CopyLastParagraph { get { return true; } }
		protected internal override DocumentModel CreateDocumentModel(DocumentModel targetDocumentModel) {
			DocumentModel result = targetDocumentModel.CreateNew();
			result.IntermediateModel = true;
			IUriProviderService service = targetDocumentModel.GetService<IUriProviderService>();
			if (service != null)
				result.ReplaceService<IUriProviderService>(service);
			IUriStreamService uriStreamService = targetDocumentModel.GetService<IUriStreamService>();
			if (uriStreamService != null)
				result.ReplaceService<IUriStreamService>(uriStreamService);
			BeforeImportEventHandler translateOnBeforeImport = delegate(object sender, BeforeImportEventArgs e) {
				targetDocumentModel.RaiseBeforeImport(e.DocumentFormat, e.Options);
			};
			result.BeforeImport += translateOnBeforeImport;
			try {
				IDocumentImportManagerService importManagerService = targetDocumentModel.GetService<IDocumentImportManagerService>();
				if (importManagerService == null)
					throw new InvalidOperationException("Could not find service: IDocumentImportManagerService");
				DocumentImportHelper importHelper = new DocumentImportHelper(result);
				importHelper.Import(stream, format, sourceUri, importManagerService);
			}
			finally {
				result.BeforeImport -= translateOnBeforeImport;
				result.RemoveService(typeof(IUriProviderService));
				result.RemoveService(typeof(IUriStreamService));
			}
			return result;
		}
	}
	#endregion
	public class DocumentRangeContentInserter : DocumentModelContentInserter {
		readonly DocumentRange sourceRange;
		public DocumentRangeContentInserter(DocumentRange sourceRange, InsertOptions insertOptions)
			: base(insertOptions) {
			this.sourceRange = sourceRange;
		}
		protected override bool CopyLastParagraph { get { return false; } }
		protected internal override DocumentModel CreateDocumentModel(DocumentModel targetDocumentModel) {
			NativeDocumentRange nativeSourceRange = (NativeDocumentRange)sourceRange;
			NativeSubDocument document = nativeSourceRange.Start.Document;
			DevExpress.XtraRichEdit.Model.SelectionRangeCollection selectionRanges = document.GetSelectionRangeCollection(sourceRange);
			if (selectionRanges == null)
				return null;
			CopySelectionManager manager = new CopySelectionManager(document.DocumentServer);
			return manager.CreateDocumentModel(DevExpress.XtraRichEdit.Model.ParagraphNumerationCopyOptions.CopyIfWholeSelected, InsertOptions == InsertOptions.MatchDestinationFormatting ? DevExpress.XtraRichEdit.Model.FormattingCopyOptions.UseDestinationStyles : Model.FormattingCopyOptions.KeepSourceFormatting, document.PieceTable, selectionRanges);
		}
	}
}
