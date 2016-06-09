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
using DevExpress.XtraRichEdit.Model;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Fields {
	#region TocField
	public partial class TocField : CalculatedFieldBase {
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "TOC";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument("o", "n", "t", "l", "f", "a", "b", "c", "d", "p", "s");
		public static CalculatedFieldBase Create() {
			return new TocField();
		}
		#endregion
		readonly TocGeneratorOptions options = new TocGeneratorOptions();
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		public TocGeneratorOptions Options { get { return options; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			options.PageNumberSeparator = instructions.GetString("p");
			options.UseParagraphOutlineLevel = instructions.GetBool("u");
			options.CreateHyperlinks = instructions.GetBool("h");
			options.HeaderLevels = GetLevelsRange(instructions.GetString("o"), "1-9");
			options.BookmarkName = instructions.GetString("b");
			string noPageNumberLevels = instructions.GetString("n");
			if (noPageNumberLevels != null)
				options.NoPageNumberLevels = GetLevelsRange(noPageNumberLevels, "1-9");
			else 
				if (instructions.GetBool("n")) 
					options.NoPageNumberLevels = GetLevelsRange("1-9", "1-9");
			options.AdditionalStyles = GetAdditionalStylesInfo(instructions.GetString("t"));
			options.EntriesLevels = GetLevelsRange(instructions.GetString("l"), String.Empty);
			if (options.EntriesLevels.Count > 0) {
				options.HeaderLevels.Clear();
				options.AdditionalStyles.Clear();
			}
			options.EntriesGroupId = instructions.GetString("f");
			if (!String.IsNullOrEmpty(options.EntriesGroupId)) {
				options.HeaderLevels.Clear();
				options.AdditionalStyles.Clear();
			}
			options.SequenceId = instructions.GetString("c");
			if (!String.IsNullOrEmpty(options.SequenceId)) {
				options.HeaderLevels.Clear();
				options.AdditionalStyles.Clear();
				options.HeaderLevels.Clear();
				options.EntriesGroupId = String.Empty;
			}
			options.PrefixedSequenceId = instructions.GetString("s");
			options.Delimiter = instructions.GetString("d");
			if (String.IsNullOrEmpty(options.Delimiter))
				options.Delimiter = "-";
		}
		#endregion
		DocumentModel targetModel;
		IDocumentLayoutService documentLayoutService;
		List<Field> fieldsToUpdate;
		protected override FieldMailMergeType MailMergeType() {
			return FieldMailMergeType.NonMailMerge;
		}
		public override bool CanPrepare {get {return true;}}
		public override void BeforeCalculateFields(PieceTable sourcePieceTable, Field documentField) {
			base.BeforeCalculateFields(sourcePieceTable, documentField);
			targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			Section section = GetFieldSection(sourcePieceTable, documentField);
			int rightTabPosition = section.Page.Width - section.Margins.Left - section.Margins.Right;
#if DEBUGTEST
			TocGenerator generator = new DevExpress.XtraRichEdit.Tests.TemplateComparerTestTocGenerator(sourcePieceTable.DocumentModel, targetModel, options, rightTabPosition);
#else
			TocGenerator generator = new TocGenerator(sourcePieceTable.DocumentModel, targetModel, options, rightTabPosition);
#endif
			generator.Export();
			documentLayoutService = sourcePieceTable.DocumentModel.GetService<IDocumentLayoutService>();
			if (documentLayoutService == null)
				return;
			targetModel.ReplaceService<IBookmarkResolutionService>(new BookmarkResolutionService(sourcePieceTable));
			targetModel.ReplaceService<IDocumentLayoutService>(documentLayoutService);
			targetModel.BeginUpdate();
			FieldUpdater updater = targetModel.MainPieceTable.FieldUpdater;
			fieldsToUpdate = updater.GetFieldsToUpdate(targetModel.MainPieceTable.Fields, null);			
			updater.PrepareFieldsCore(fieldsToUpdate, UpdateFieldOperationType.Normal);
		}
		private Section GetFieldSection(PieceTable pieceTable, Field documentField) {
			DocumentModel documentModel = pieceTable.DocumentModel;
			if (pieceTable.ContentType.IsMain) {
				DocumentModelPosition position = DocumentModelPosition.FromRunStart(pieceTable, documentField.FirstRunIndex);
				SectionIndex sectionIndex = documentModel.FindSectionIndex(position.LogPosition);
				return documentModel.Sections[sectionIndex];
			}
			else {
				Debug.Assert(false);
				return documentModel.Sections[new SectionIndex(0)];
			}
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			try {
				if (documentLayoutService == null || targetModel == null)
					return new CalculatedFieldValue(targetModel);
				try {
					targetModel.MainPieceTable.FieldUpdater.UpdateFieldsCore(fieldsToUpdate, UpdateFieldOperationType.Normal);
				}
				finally {
					targetModel.RemoveService(typeof(IBookmarkResolutionService));
					targetModel.RemoveService(typeof(IDocumentLayoutService));
				}
				return new CalculatedFieldValue(targetModel);
			}
			finally {
				if(targetModel != null)
					targetModel.EndUpdate();
				targetModel = null;
				documentLayoutService = null;
				fieldsToUpdate = null;
			}
		}
		IList<int> GetLevelsRange(string value, string defaultValue) {
			IList<int> result = GetLevelsRangeCore(value);
			if (result == null)
				result = GetLevelsRangeCore(defaultValue);
			if (result == null)
				result = new List<int>();
			return result;
		}
		IList<int> GetLevelsRangeCore(string value) {
			if (String.IsNullOrEmpty(value))
				return null;
			string[] parts = value.Split('-');
			if (parts == null || parts.Length != 2)
				return null;
			int start;
			if (!Int32.TryParse(parts[0], out start))
				return null;
			int end;
			if (!Int32.TryParse(parts[1], out end))
				return null;
			if (end < start)
				return null;
			List<int> result = new List<int>(end - start + 1);
			for (int i = start; i <= end; i++)
				result.Add(i);
			return result;
		}
		IList<StyleOutlineLevelInfo> GetAdditionalStylesInfo(string value) {
			List<StyleOutlineLevelInfo> result = new List<StyleOutlineLevelInfo>();
			if (String.IsNullOrEmpty(value))
				return result;
			string[] parts = value.Split(',');
			int count = parts.Length / 2;
			for (int i = 0; i < count; i++) {
				string styleName = parts[i * 2].Trim();
				if (String.IsNullOrEmpty(styleName))
					continue;
				string outlineLevelString = parts[i * 2 + 1].Trim();
				int outlineLevel;
				if (Int32.TryParse(outlineLevelString, out outlineLevel) && outlineLevel >= 0 && outlineLevel <= 9)
					result.Add(new StyleOutlineLevelInfo(styleName, outlineLevel));
			}
			return result;
		}
		public override bool CanUseSwitchWithoutArgument(string fieldSpecificSwitch) {
			return fieldSpecificSwitch == "\\n";
		}
	}
#endregion
#region TocGeneratorOptions
	public class TocGeneratorOptions {
#region Fields
		string pageNumberSeparator = String.Empty;
		IList<int> noPageNumberLevels = new List<int>();
		IList<int> headerLevels = new List<int>();
		string entriesGroupId = String.Empty;
		string sequenceId = String.Empty;
		string prefixedSequenceId = String.Empty;
		IList<int> entriesLevels = new List<int>();
		IList<StyleOutlineLevelInfo> additionalStyles = new List<StyleOutlineLevelInfo>();
		bool useParagraphOutlineLevel;
		bool createHyperlinks;
		string delimiter = String.Empty;
		string bookmarkName = String.Empty;
#endregion
#region Properties
		public string PageNumberSeparator { get { return pageNumberSeparator; } set { pageNumberSeparator = value; } }
		public IList<int> NoPageNumberLevels { get { return noPageNumberLevels; } set { noPageNumberLevels = value; } }
		public IList<int> HeaderLevels { get { return headerLevels; } set { headerLevels = value; } }
		public IList<int> EntriesLevels { get { return entriesLevels; } set { entriesLevels = value; } }
		public string EntriesGroupId { get { return entriesGroupId; } set { entriesGroupId = value; } }
		public string SequenceId { get { return sequenceId; } set { sequenceId = value; } }
		public string PrefixedSequenceId { get { return prefixedSequenceId; } set { prefixedSequenceId = value; } }
		public IList<StyleOutlineLevelInfo> AdditionalStyles { get { return additionalStyles; } set { additionalStyles = value; } }
		public bool UseParagraphOutlineLevel { get { return useParagraphOutlineLevel; } set { useParagraphOutlineLevel = value; } }
		public bool CreateHyperlinks { get { return createHyperlinks; } set { createHyperlinks = value; } }
		public string Delimiter { get { return delimiter; } set { delimiter = value; } }
		public string BookmarkName { get { return bookmarkName; } set { bookmarkName = value; } }
#endregion
	}
#endregion
#region StyleOutlineLevelInfo
	public class StyleOutlineLevelInfo {
		readonly string styleName;
		readonly int outlineLevel;
		public StyleOutlineLevelInfo(string styleName, int outlineLevel) {
			this.styleName = styleName;
			this.outlineLevel = outlineLevel;
		}
		public string StyleName { get { return styleName; } }
		public int OutlineLevel { get { return outlineLevel; } }
	}
#endregion
#region TocGenerator
	public class TocGenerator : DocumentModelExporter {
#region Fields
		readonly DocumentModel targetDocumentModel;
		readonly StringBuilder paragraphText = new StringBuilder();
		readonly TocGeneratorOptions options;
		IList<Field> tocFields;
		int rightTabPosition;
		bool analyzeFields;
		DocumentModelPosition start;
		DocumentModelPosition end;
#endregion
		public TocGenerator(DocumentModel documentModel, DocumentModel targetDocumentModel, TocGeneratorOptions options, int rightTabPosition)
			: base(documentModel) {
			Guard.ArgumentNotNull(targetDocumentModel, "targetDocumentModel");
			Guard.ArgumentNotNull(options, "options");
			this.targetDocumentModel = targetDocumentModel;
			this.options = options;
			this.rightTabPosition = rightTabPosition;
		}
#region Properties
		public DocumentModel TargetDocumentModel { get { return targetDocumentModel; } }
		public PieceTable TargetPieceTable { get { return targetDocumentModel.MainPieceTable; } }
		protected internal override bool ShouldExportHiddenText { get { return false; } }
		public TocGeneratorOptions Options { get { return options; } }
#endregion
		protected internal override void ExportDocument() {
			DocumentModel.BeginUpdate();
			try {
				SetTocSearchRange();
				this.tocFields = DocumentModel.MainPieceTable.GetTocFields();
				this.analyzeFields = String.IsNullOrEmpty(options.EntriesGroupId) || options.EntriesLevels.Count >= 0 || String.IsNullOrEmpty(options.SequenceId);
				base.ExportDocument();
				if (TargetDocumentModel.IsEmpty)
					TargetDocumentModel.MainPieceTable.InsertText(DocumentLogPosition.Zero, XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_NoTocEntriesFound));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void SetTocSearchRange() {
			if (String.IsNullOrEmpty(options.BookmarkName)) {
				SetDefaultTocSearchRange();
				return;
			}
			Bookmark bookmark = PieceTable.Bookmarks.FindByName(options.BookmarkName);
			if (bookmark == null) {
				SetDefaultTocSearchRange();
				return;
			}
			start = new DocumentModelPosition(PieceTable);
			start.LogPosition = bookmark.NormalizedStart;
			start.Update();
			end = new DocumentModelPosition(PieceTable);
			end.LogPosition = bookmark.NormalizedEnd;
			end.Update();
		}
		protected internal virtual void SetDefaultTocSearchRange() {
			start = DocumentModelPosition.FromParagraphStart(PieceTable, ParagraphIndex.Zero);
			end = DocumentModelPosition.FromParagraphEnd(PieceTable, new ParagraphIndex(PieceTable.Paragraphs.Count - 1));
		}
		protected internal override void ExportSectionHeadersFooters(Section section) {
		}
		protected internal override bool ShouldSplitRuns() {
			return false;
		}
		protected internal override void PushVisitableDocumentIntervalBoundaryIterator() {
		}
		protected internal override void PopVisitableDocumentIntervalBoundaryIterator() {
		}
		protected internal override void TryToExportBookmarks(RunIndex runIndex, int runOffset) {
		}
		RunIndex currentRunIndex = RunIndex.DontCare;
		protected internal override void ExportRun(RunIndex i) {
			this.currentRunIndex = i;
			base.ExportRun(i);
		}
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			base.ExportFieldCodeStartRun(run);
			if (analyzeFields && currentRunIndex >= start.RunIndex && currentRunIndex <= end.RunIndex) {
				Field field = PieceTable.FindFieldByRunIndex(currentRunIndex);
				if (field != null)
					ProcessField(field);
			}
		}
		protected internal virtual void ProcessField(Field field) {
			DocumentFieldIterator iterator = new DocumentFieldIterator(PieceTable, field);
			FieldScanner scanner = new FieldScanner(iterator, PieceTable.DocumentModel.MaxFieldSwitchLength, PieceTable.DocumentModel.EnableFieldNames, PieceTable.SupportFieldCommonStringFormat);
			Token token = scanner.Scan();
			if (token.ActualKind != TokenKind.OpEQ && token.ActualKind != TokenKind.Eq) {
				CalculatedFieldBase calculatedField = FieldCalculatorService.CreateField(token.Value);
				if (calculatedField != null)
					ProcessCalculatedField(calculatedField, scanner, field);
			}
		}
		protected internal virtual void ProcessCalculatedField(CalculatedFieldBase field, FieldScanner scanner, Field documentField) {
			if (field.GetType() == typeof(TocEntryField)) {
				InitializeField(field, scanner);
				ProcessTocEntryField((TocEntryField)field);
			}
			else if (field.GetType() == typeof(SequenceField)) {
				InitializeField(field, scanner);
				ProcessSequenceField((SequenceField)field, documentField);
			}
		}
		protected internal virtual void InitializeField(CalculatedFieldBase field, FieldScanner scanner) {
			InstructionCollection instructions = FieldCalculatorService.ParseInstructions(scanner, field);
			field.Initialize(PieceTable, instructions);
		}
		protected internal virtual void ProcessTocEntryField(TocEntryField entry) {
			if (options.EntriesLevels.Contains(entry.Level) || (!String.IsNullOrEmpty(options.EntriesGroupId) && options.EntriesGroupId == entry.Id)) {
				string text = entry.Text.Trim();
				if (!String.IsNullOrEmpty(text)) {
					Bookmark bookmark = ObtainTocBookmarkForTocEntry(entry, currentRunIndex + 1);
					AppendTocEntry(text, bookmark, entry.Level);
				}
			}
		}
		protected internal virtual void ProcessSequenceField(SequenceField sequence, Field documentField) {
			if (!String.IsNullOrEmpty(options.SequenceId) && options.SequenceId == sequence.Id && !sequence.HideResult) {
				this.currentParagraphOutlineLevel = 1;
				if (options.PrefixedSequenceId == sequence.Id) {
					string text = GetSequenceFieldResultText(sequence, documentField);
					if (!String.IsNullOrEmpty(text))
						this.pageNumberPrefix = text + options.Delimiter;
				}
			}
		}
		string GetSequenceFieldResultText(SequenceField sequence, Field documentField) {
			using (DocumentModel result = PieceTable.DocumentModel.CreateNew()) {
				return result.InternalAPI.GetDocumentPlainTextContent();
			}
		}
		protected internal override void ExportSection(Section section) {
			base.ExportSection(section);
		}
		int currentParagraphOutlineLevel;
		string pageNumberPrefix;
		TabFormattingInfo paragraphTabs;
		int numberingListIndent;
		protected internal override void ExportParagraphs(ParagraphIndex from, ParagraphIndex to) {
			from = Algorithms.Max(from, start.ParagraphIndex);
			to = Algorithms.Min(to, end.ParagraphIndex);
			base.ExportParagraphs(from, to);
		}
		protected internal override ParagraphIndex ExportParagraph(Paragraph paragraph) {
			this.pageNumberPrefix = String.Empty;
			this.paragraphText.Length = 0;
			this.paragraphTabs = new TabFormattingInfo();
			this.numberingListIndent = -1;
			int outlineLevel = GetActualOutlineLevel(paragraph);
			if (Options.HeaderLevels.Contains(outlineLevel) && CanIncludeParagraphIntoToc(paragraph)) {
				this.currentParagraphOutlineLevel = outlineLevel;
				if (paragraph.IsInList())
					AppendNumberingToParagraph(paragraph);
			}
			else
				this.currentParagraphOutlineLevel = -1;
			return base.ExportParagraph(paragraph);
		}
		void AppendNumberingToParagraph(Paragraph paragraph) {
			string separator = paragraph.GetListLevelSeparator();
			if (separator == "\t") {
				int listLevelIndex = paragraph.GetListLevelIndex();
				IOverrideListLevel listLevel = paragraph.DocumentModel.NumberingLists[paragraph.GetNumberingListIndex()].Levels[listLevelIndex];
				this.numberingListIndent = listLevel.FirstLineIndent;
			}
			this.paragraphText.Append(paragraph.GetNumberingListText() + separator);
		}
		protected internal override void ExportTextRun(TextRun run) {
			string text = run.GetPlainText(PieceTable.TextBuffer);
			text = text.Replace(Characters.LineBreak, ' '); 
			text = text.Replace(new String(Characters.PageBreak, 1), String.Empty); 
			paragraphText.Append(text);
		}
		protected internal override void ExportInlinePictureRun(InlinePictureRun run) {
			if (currentParagraphOutlineLevel < 0)
				return;
			string text = this.paragraphText.ToString().TrimStart();
			if (!String.IsNullOrEmpty(text)) {
				TargetPieceTable.InsertText(TargetPieceTable.DocumentEndLogPosition, text);
				this.paragraphText.Length = 0;
			}
			TargetPieceTable.InsertInlinePicture(TargetPieceTable.DocumentEndLogPosition, run.Image.Clone(TargetPieceTable.DocumentModel));
		}
		protected internal override void ExportParagraphRun(ParagraphRun run) {
			if (currentParagraphOutlineLevel < 0)
				return;
			string text = paragraphText.ToString().Trim();
			Paragraph tocParagraph = TargetPieceTable.Paragraphs.Last;
			if (!String.IsNullOrEmpty(text) || tocParagraph.Length > 1) {
				Bookmark bookmark = ObtainTocBookmarkForHeaderOrOutline(run.Paragraph);
				AppendTocEntry(text, bookmark, this.currentParagraphOutlineLevel);
			}
		}
		void AppendTocEntry(string text, Bookmark bookmark, int level) {
			Paragraph tocParagraph = TargetPieceTable.Paragraphs.Last;
			if (ShouldWritePageNumberIntoToc(level))
				text = ApplyPageNumber(text, tocParagraph);
			DocumentLogPosition textStartPosition = TargetPieceTable.DocumentEndLogPosition;
			TargetPieceTable.InsertText(textStartPosition, text);
			if (ShouldWritePageNumberIntoToc(level)) {
				DocumentLogPosition pageRefPosition = TargetPieceTable.DocumentEndLogPosition;
				string fieldCode = String.Format("PAGEREF {0}", bookmark.Name);
				TargetPieceTable.InsertText(pageRefPosition, fieldCode);
				TargetPieceTable.CreateField(pageRefPosition, fieldCode.Length);
			}
			if (Options.CreateHyperlinks) {
				HyperlinkInfo hyperlinkInfo = new HyperlinkInfo();
				hyperlinkInfo.Anchor = bookmark.Name;
				TargetPieceTable.CreateHyperlink(textStartPosition, text.Length, hyperlinkInfo);
			}
			ApplyTocStyleToParagraph(level, tocParagraph);
			ApplyTabsToParagraph(tocParagraph);
			TargetPieceTable.InsertParagraph(TargetPieceTable.DocumentEndLogPosition);
		}
		void ApplyTabsToParagraph(Paragraph tocParagraph) {
			TabFormattingInfo ownTabs = tocParagraph.GetOwnTabs();
			TabFormattingInfo newTabs = new TabFormattingInfo();
			if (this.numberingListIndent >= 0)
				ownTabs.Add(new TabInfo(this.numberingListIndent + tocParagraph.LeftIndent, TabAlignmentType.Left));
			TabFormattingInfo tocStyleTabs = tocParagraph.ParagraphStyle.GetTabs();
			int count = Math.Max(0, ownTabs.Count - tocStyleTabs.Count);
			for (int i = 0; i < count; i++)
				newTabs.Add(ownTabs[i]);
			tocParagraph.SetOwnTabs(newTabs);
		}
		string ApplyPageNumber(string text, Paragraph target) {
			string separator = String.IsNullOrEmpty(Options.PageNumberSeparator) ? "\t" : Options.PageNumberSeparator;
			if (separator == "\t") {
				TabFormattingInfo ownTabs = target.GetOwnTabs();
				ownTabs.Add(new TabInfo(rightTabPosition, TabAlignmentType.Right, TabLeaderType.Dots));
				target.SetOwnTabs(ownTabs);
			}
			return text + separator + pageNumberPrefix;
		}
		Bookmark ObtainTocBookmarkForHeaderOrOutline(Paragraph paragraph) {
			DocumentLogPosition actualParagraphPosition = CalculateActualParagraphPosition(paragraph);
			Bookmark bookmark = LookupEntireParagraphBookmark(paragraph, actualParagraphPosition);
			if (bookmark != null)
				return bookmark;
			PieceTable pieceTable = paragraph.PieceTable;
			string name = GenerateTocBookmarkName(pieceTable);
			pieceTable.CreateBookmark(actualParagraphPosition, paragraph.LogPosition + paragraph.Length - actualParagraphPosition - 1, name);
			return pieceTable.Bookmarks.FindByName(name);
		}
		Bookmark ObtainTocBookmarkForTocEntry(TocEntryField entry, RunIndex tocEntryFieldCodeFirstRunIndex) {
			Paragraph paragraph = PieceTable.Runs[tocEntryFieldCodeFirstRunIndex].Paragraph;
			DocumentLogPosition pos = paragraph.LogPosition;
			for (RunIndex i = paragraph.FirstRunIndex; i < tocEntryFieldCodeFirstRunIndex; i++)
				pos += PieceTable.Runs[i].Length;
			pos += 4;
			int len = entry.Text.Length;
			Bookmark bookmark = LookupExactBookmark(pos, len);
			if (bookmark != null)
				return bookmark;
			string name = GenerateTocBookmarkName(PieceTable);
			PieceTable.CreateBookmark(pos, len, name);
			return PieceTable.Bookmarks.FindByName(name);
		}
		protected internal virtual string GenerateTocBookmarkName(PieceTable pieceTable) {
			Random rnd = new Random((int)DateTime.Now.Ticks);
			for (; ; ) {
				string name = String.Format("_Toc{0}", rnd.Next());
				if (pieceTable.Bookmarks.FindByName(name) == null)
					return name;
			}
		}
		protected internal virtual Bookmark LookupEntireParagraphBookmark(Paragraph paragraph, DocumentLogPosition actualParagraphPosition) {
			DocumentLogPosition pos = actualParagraphPosition;
			BookmarkCollection bookmarks = paragraph.PieceTable.Bookmarks;
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++) {
				Bookmark bookmark = bookmarks[i];
				if (bookmark.NormalizedStart == pos && (bookmark.Start + bookmark.Length == paragraph.LogPosition + paragraph.Length - 1))
					return bookmark;
			}
			return null;
		}
		protected internal virtual Bookmark LookupZeroLengthBookmarkAtLogPosition(DocumentLogPosition pos) {
			return LookupExactBookmark(pos, 0);
		}
		protected internal virtual Bookmark LookupExactBookmark(DocumentLogPosition pos, int len) {
			BookmarkCollection bookmarks = PieceTable.Bookmarks;
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++) {
				Bookmark bookmark = bookmarks[i];
				if (bookmark.NormalizedStart == pos && bookmark.Length == len)
					return bookmark;
			}
			return null;
		}
		void ApplyTocStyleToParagraph(int level, Paragraph tocParagraph) {
			int styleIndex = CalculateTocParagraphStyleIndex(level);
			if (styleIndex >= 0)
				tocParagraph.ParagraphStyleIndex = styleIndex;
		}
		int CalculateTocParagraphStyleIndex(int level) {
			int index = TargetDocumentModel.GetTocStyle(level);
			if (index >= 0)
				return index;
			int sourceIndex = DocumentModel.GetTocStyle(level);
			if (sourceIndex >= 0)
				return DocumentModel.ParagraphStyles[sourceIndex].Copy(TargetDocumentModel);
			return TargetDocumentModel.CreateTocStyle(level);
		}
		bool ShouldWritePageNumberIntoToc(int level) {
			return !Options.NoPageNumberLevels.Contains(level);
		}
		int GetActualOutlineLevel(Paragraph paragraph) {
			int styleOutlineLevel = GetOutlineLevelByStyleName(paragraph.ParagraphStyle.StyleName);
			if (styleOutlineLevel >= 0)
				return styleOutlineLevel;
			if (Options.UseParagraphOutlineLevel) {
				styleOutlineLevel = paragraph.ParagraphStyle.ParagraphProperties.OutlineLevel;
				if (styleOutlineLevel > 0)
					return styleOutlineLevel;
				return paragraph.ParagraphProperties.OutlineLevel;
			}
			return paragraph.ParagraphStyle.ParagraphProperties.OutlineLevel;
		}
		int GetOutlineLevelByStyleName(string styleName) {
			IList<StyleOutlineLevelInfo> styleLevelInfo = Options.AdditionalStyles;
			int count = styleLevelInfo.Count;
			for (int i = 0; i < count; i++) {
				if (String.Compare(styleLevelInfo[i].StyleName, styleName, StringComparison.CurrentCultureIgnoreCase) == 0)
					return styleLevelInfo[i].OutlineLevel;
			}
			return -1;
		}
		DocumentLogPosition CalculateActualParagraphPosition(Paragraph paragraph) {
			PieceTable pieceTable = paragraph.PieceTable;
			ParagraphCharacterIterator characterIterator = new ParagraphCharacterIterator(paragraph, pieceTable, new VisibleOnlyTextFilter(pieceTable));
			DocumentLogPosition result = paragraph.LogPosition;
			while (IsWhiteSpace(characterIterator.CurrentChar) && !characterIterator.IsEnd) {
				result++;
				characterIterator.Next();
			}
			return result;
		}
		bool IsWhiteSpace(char ch) {
			return Char.IsWhiteSpace(ch) || ch == Characters.NonBreakingSpace || ch == Characters.PageBreak || ch == Characters.ColumnBreak || ch == Characters.LineBreak;
		}
		bool CanIncludeParagraphIntoToc(Paragraph paragraph) {
			int count = tocFields.Count;
			for (int i = 0; i < count; i++) {
				if (IsParagraphInsideField(paragraph, tocFields[i]))
					return false;
			}
			return true;
		}
		bool IsParagraphInsideField(Paragraph paragraph, Field field) {
			return paragraph.FirstRunIndex >= field.Code.Start && paragraph.LastRunIndex <= field.Result.End;
		}
	}
#endregion
}
