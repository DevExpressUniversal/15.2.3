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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.SpellChecker;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Utils.Internal;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.IO;
using System.Diagnostics;
#else
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region RunInfo
	public class RunInfo : ICloneable<RunInfo>, ISupportsCopyFrom<RunInfo> {
		#region Fields
		DocumentModelPosition start;
		DocumentModelPosition end;
		#endregion
		public RunInfo(PieceTable pieceTable) {
			this.start = CreateDocumentModelPosition(pieceTable);
			this.end = CreateDocumentModelPosition(pieceTable);
		}
		#region Properties
		public DocumentModelPosition Start { get { return start; } }
		public DocumentModelPosition End { get { return end; } }
		public DocumentModelPosition NormalizedStart { get { return Algorithms.Min(start, end); } }
		public DocumentModelPosition NormalizedEnd { get { return Algorithms.Max(start, end); } }
		#endregion
		#region ICloneable<RunInfo> Members
		public RunInfo Clone() {
			RunInfo clone = CreateEmptyClone();
			clone.CopyFrom(this);
			return clone;
		}
		protected virtual RunInfo CreateEmptyClone() {
			return new RunInfo(Start.PieceTable);
		}
		#endregion
		#region ISupportsCopyFrom<RunInfo> Members
		public void CopyFrom(RunInfo value) {
			this.Start.CopyFrom(value.Start);
			this.End.CopyFrom(value.End);
		}
		#endregion
		protected virtual DocumentModelPosition CreateDocumentModelPosition(PieceTable pieceTable) {
			return new DocumentModelPosition(pieceTable);
		}
		public override bool Equals(object obj) {
			RunInfo info = obj as RunInfo;
			if (info == null)
				return false;
			return info.Start == Start && info.End == End;
		}
		public override int GetHashCode() {
			return Start.GetHashCode() & End.GetHashCode();
		}
		protected void SetStartAnchorCore(DocumentModelPosition value) {
			start = value;
		}
		protected void SetEndAnchorCore(DocumentModelPosition value) {
			end = value;
		}
#if DEBUG
		public override string ToString() {
			return String.Format("[{0}] - [{1}]", Start.ToString(), End.ToString());
		}
#endif
	}
	#endregion
	#region ReferencedRunInfoBase
	public abstract class ReferencedRunInfoBase : RunInfo {
		protected ReferencedRunInfoBase(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public new ReferencedDocumentModelPositionBase Start { get { return (ReferencedDocumentModelPositionBase)base.Start; } }
		public new ReferencedDocumentModelPositionBase End { get { return (ReferencedDocumentModelPositionBase)base.End; } }
		public virtual void RegisterInterval(DocumentModelPositionManager documentModelPositionManager) {
			SetStartAnchorCore(documentModelPositionManager.RegisterNewPosition(Start));
			SetEndAnchorCore(documentModelPositionManager.RegisterNewPosition(End));
		}
		public virtual void AttachInterval(DocumentModelPositionManager documentModelPositionManager) {
			documentModelPositionManager.AttachPosition(Start);
			documentModelPositionManager.AttachPosition(End);
		}
		public virtual void DetachInterval(DocumentModelPositionManager documentModelPositionManager) {
			documentModelPositionManager.DetachPosition(Start);
			documentModelPositionManager.DetachPosition(End);
		}
	}
	#endregion
	#region ReferencedBookmarkRunInfo
	public class ReferencedBookmarkRunInfo : ReferencedRunInfoBase {
		public ReferencedBookmarkRunInfo(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public new ReferencedBookmarkDocumentModelPosition Start { get { return (ReferencedBookmarkDocumentModelPosition)base.Start; } }
		public new ReferencedBookmarkDocumentModelPosition End { get { return (ReferencedBookmarkDocumentModelPosition)base.End; } }
		protected override RunInfo CreateEmptyClone() {
			return new ReferencedBookmarkRunInfo(Start.PieceTable);
		}
		protected override DocumentModelPosition CreateDocumentModelPosition(PieceTable pieceTable) {
			return new ReferencedBookmarkDocumentModelPosition(pieceTable);
		}
	}
	#endregion
	#region ReferencedIntervalRunInfo
	public class ReferencedIntervalRunInfo : ReferencedRunInfoBase {
		public ReferencedIntervalRunInfo(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public new ReferencedBookmarkDocumentModelPosition Start { get { return (ReferencedBookmarkDocumentModelPosition)base.Start; } }
		public new ReferencedBookmarkDocumentModelPosition End { get { return (ReferencedBookmarkDocumentModelPosition)base.End; } }
		protected override RunInfo CreateEmptyClone() {
			return new ReferencedIntervalRunInfo(Start.PieceTable);
		}
		protected override DocumentModelPosition CreateDocumentModelPosition(PieceTable pieceTable) {
			return new ReferencedIntervalDocumentModelPosition(pieceTable);
		}
	}
	#endregion
	#region LastInsertedRunInfo<TRun, TRunHistoryItem> (abstract)
	public abstract class LastInsertedRunInfo<TRun, TRunHistoryItem>
		where TRun : TextRunBase
		where TRunHistoryItem : TextRunBaseHistoryItem {
		#region Fields
		PieceTable pieceTable;
		RunIndex runIndex;
		TRun run;
		TRunHistoryItem historyItem;
		#endregion
		protected LastInsertedRunInfo() {
			Reset(null);
		}
		#region Properties
		#region Run
		public TRun Run {
			[System.Diagnostics.DebuggerStepThrough]
			get { return run; }
			[System.Diagnostics.DebuggerStepThrough]
			set { run = value; }
		}
		#endregion
		#region RunIndex
		public RunIndex RunIndex {
			[System.Diagnostics.DebuggerStepThrough]
			get { return runIndex; }
			[System.Diagnostics.DebuggerStepThrough]
			set { runIndex = value; }
		}
		#endregion
		#region HistoryItem
		public TRunHistoryItem HistoryItem {
			[System.Diagnostics.DebuggerStepThrough]
			get { return historyItem; }
			[System.Diagnostics.DebuggerStepThrough]
			set { historyItem = value; }
		}
		#endregion
		#region PieceTable
		protected internal PieceTable PieceTable {
			[System.Diagnostics.DebuggerStepThrough]
			get { return pieceTable; }
			[System.Diagnostics.DebuggerStepThrough]
			set { pieceTable = value; }
		}
		#endregion
		#endregion
		public virtual void Reset(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
			this.run = null;
			this.runIndex = new RunIndex(-1);
		}
	}
	#endregion
	#region LastInsertedRunInfo
	public class LastInsertedRunInfo : LastInsertedRunInfo<TextRun, TextRunInsertedHistoryItem> {
		DocumentLogPosition logPosition;
		#region LogPosition
		public DocumentLogPosition LogPosition {
			[System.Diagnostics.DebuggerStepThrough]
			get { return logPosition; }
			[System.Diagnostics.DebuggerStepThrough]
			set { logPosition = value; }
		}
		#endregion
		public override void Reset(PieceTable pieceTable) {
			base.Reset(pieceTable);
			this.logPosition = new DocumentLogPosition(-1);
		}
	}
	#endregion
	#region LastInsertedPictureRunInfo
	public class LastInsertedInlinePictureRunInfo : LastInsertedRunInfo<InlinePictureRun, InlinePictureRunInsertedHistoryItem> { }
	#endregion
	#region LastInsertedSeparatorRunInfo
	public class LastInsertedSeparatorRunInfo : LastInsertedRunInfo<SeparatorTextRun, SeparatorRunInsertedHistoryItem> { }
	#endregion
	#region ContentTypeBase
	public abstract class ContentTypeBase {
		readonly PieceTable pieceTable;
		protected ContentTypeBase(DocumentModel documentModel) {
			this.pieceTable = documentModel.CreatePieceTable(this);
		}
		protected ContentTypeBase(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		protected internal DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public virtual bool IsMain { get { return true; } }
		public virtual bool IsHeaderFooter { get { return false; } }
		public virtual bool IsFooter { get { return false; } }
		public virtual bool IsHeader { get { return false; } }
		public virtual bool IsNote { get { return false; } }
		public virtual bool IsFootNote { get { return false; } }
		public virtual bool IsEndNote { get { return false; } }
		public virtual bool IsTextBox { get { return false; } }
		public virtual bool IsComment { get { return false; } }
		public virtual bool IsReferenced { get { return true; } }
		protected internal virtual SectionIndex LookupSectionIndexByParagraphIndex(ParagraphIndex paragraphIndex) {
			SectionParagraphIndexComparable predicate = new SectionParagraphIndexComparable(paragraphIndex);
			return OfficeAlgorithms.BinarySearch(DocumentModel.Sections, predicate);
		}
		protected internal virtual void ApplyChanges(DocumentModelChangeType changeType, RunIndex startRunIndex, RunIndex endRunIndex) {
			DocumentModel.ApplyChanges(PieceTable, changeType, startRunIndex, endRunIndex);
		}
		protected internal virtual void ApplyChangesCore(DocumentModelChangeActions actions, RunIndex startRunIndex, RunIndex endRunIndex) {
			DocumentModel.ApplyChangesCore(PieceTable, actions, startRunIndex, endRunIndex);
		}
		protected internal virtual void ApplySectionFormatting(DocumentLogPosition logPositionStart, int length, SectionPropertyModifierBase modifier) {
			DocumentModel.ApplySectionFormatting(logPositionStart, length, modifier);
		}
		protected internal virtual Nullable<T> ObtainSectionsPropertyValue<T>(DocumentLogPosition logPositionStart, int length, SectionPropertyModifier<T> modifier) where T : struct {
			return DocumentModel.ObtainSectionsPropertyValue<T>(logPositionStart, length, modifier);
		}
		protected internal virtual void FixLastParagraphOfLastSection(int originalParagraphCount) {
			FixLastParagraphOfLastSectionHistoryItem item = new FixLastParagraphOfLastSectionHistoryItem(PieceTable);
			item.OriginalParagraphCount = originalParagraphCount;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		[System.Diagnostics.Conditional("DEBUG")]
		internal void CheckIntegrity() {
#if DEBUGTEST
			PieceTable.CheckIntegrity();
#endif
		}
		protected internal virtual SpellCheckerManager CreateSpellCheckerManager(PieceTable pieceTable) {
			return new EmptySpellCheckerManager(pieceTable);
		}
		protected internal virtual void SetPageCount(int pageCount) {
		}
	}
	#endregion
	#region MainContentType
	public class MainContentType : ContentTypeBase {
		public MainContentType(DocumentModel documentModel)
			: base(documentModel) {
		}
		protected internal override void SetPageCount(int pageCount) {
			DocumentModel.ExtendedDocumentProperties.SetPages(pageCount);
		}
	}
	#endregion
	public class CustomCreateVisibleTextFilterEventArgs : EventArgs {
		IVisibleTextFilter textFilter;
		public CustomCreateVisibleTextFilterEventArgs(IVisibleTextFilter textFilter) {
			TextFilter = textFilter;
		}
		public IVisibleTextFilter TextFilter {
			get { return textFilter; }
			set {
				Guard.ArgumentNotNull(value, "value");
				this.textFilter = value;
			}
		}
	}
	public delegate void CustomCreateVisibleTextFilterEventHandler(object sender, CustomCreateVisibleTextFilterEventArgs e);
	#region PieceTable
	public partial class PieceTable : IDocumentModelStructureChangedListener, IDocumentModelPart {
		#region Fields
		readonly ContentTypeBase contentType;
		readonly DocumentModel documentModel;
		readonly ChunkedStringBuilder textBuffer;
		readonly TextRunCollection runs; 
		readonly ParagraphCollection paragraphs;  
		readonly TableCollection tables;  
		readonly FieldCollection fields; 
		readonly BookmarkCollection bookmarks; 
		readonly CommentCollection comments;
		readonly RangePermissionCollection rangePermissions; 
		readonly TableCellsManager myTables; 
		readonly HyperlinkInfoCollection hyperlinkInfos; 
		readonly CustomMarkCollection customMarks;
		readonly List<TextBoxContentType> textBoxes;
		IVisibleTextFilter visibleTextFilter;
		IVisibleTextFilter navigationVisibleTextFilter;
		readonly TextInserter textInserter;
		readonly LayoutDependentTextInserter layoutDependentTextInserter;
		readonly FootNoteRunInserter footNoteRunInserter;
		readonly EndNoteRunInserter endNoteRunInserter;
		readonly ParagraphInserter paragraphInserter;
		readonly FieldUpdater fieldUpdater;
		private readonly int id;
		SpellCheckerManager spellCheckerManager;
		bool suppressTableIntegrityCheck;
		bool shouldForceUpdateIntervals = true;
		#endregion
		public PieceTable(DocumentModel documentModel, ContentTypeBase contentType) {
			this.contentType = contentType;
			this.documentModel = documentModel;
			this.id = documentModel.GetIdForNewPeaceTable();
			this.textBuffer = new ChunkedStringBuilder();
			this.runs = new TextRunCollection();
			this.paragraphs = new ParagraphCollection();
			this.tables = new TableCollection();
			this.fields = new FieldCollection(this);
			this.bookmarks = new BookmarkCollection(this);
			this.comments = new CommentCollection(this);
			this.rangePermissions = new RangePermissionCollection(this);
			this.myTables = new TableCellsManager(this);
			this.hyperlinkInfos = new HyperlinkInfoCollection(); 
			this.customMarks = new CustomMarkCollection();
			this.textBoxes = new List<TextBoxContentType>();
			this.calculatorCache = new NumberingListCalculatorCache(documentModel);
			this.spellCheckerManager = CreateSpellCheckerManager();
			SetShowHiddenText(false);
			Clear();
			this.textInserter = new TextInserter(this);
			this.layoutDependentTextInserter = new LayoutDependentTextInserter(this);
			this.footNoteRunInserter = new FootNoteRunInserter(this);
			this.endNoteRunInserter = new EndNoteRunInserter(this);
			this.paragraphInserter = new ParagraphInserter(this);
			this.fieldUpdater = CreateFieldUpdater();
		}
		#region Properties
		public int Id { get { return id; } }
		public bool IsMain { get { return contentType.IsMain; } }
		public bool IsHeaderFooter { get { return contentType.IsHeaderFooter; } }
		public bool IsFooter { get { return contentType.IsFooter; } }
		public bool IsHeader { get { return contentType.IsHeader; } }
		public bool IsNote { get { return contentType.IsNote; } }
		public bool IsFootNote { get { return contentType.IsFootNote; } }
		public bool IsEndNote { get { return contentType.IsEndNote; } }
		public bool IsTextBox { get { return contentType.IsTextBox; } }
		public bool IsComment { get { return contentType.IsComment; } }
		public bool IsReferenced { get { return contentType.IsReferenced; } }
		public Dictionary<Paragraph, string> PrecalculatedNumberingListTexts { get; set; }
		protected internal ContentTypeBase ContentType { get { return contentType; } }
		protected internal virtual bool SupportFieldCommonStringFormat { get { return false; } }
		public ChunkedStringBuilder TextBuffer {
			[System.Diagnostics.DebuggerStepThrough]
			get { return textBuffer; }
		}
		#region Runs
		public TextRunCollection Runs {
			[System.Diagnostics.DebuggerStepThrough]
			get { return runs; }
		}
		#endregion
		#region Paragraphs
		public ParagraphCollection Paragraphs {
			[System.Diagnostics.DebuggerStepThrough]
			get { return paragraphs; }
		}
		#endregion
		#region Tables
		public TableCollection Tables {
			[System.Diagnostics.DebuggerStepThrough]
			get { return tables; }
		}
		#endregion
		#region Fields
		public FieldCollection Fields {
			[System.Diagnostics.DebuggerStepThrough]
			get { return fields; }
		}
		#endregion
		#region Bookmarks
		public BookmarkCollection Bookmarks {
			[System.Diagnostics.DebuggerStepThrough]
			get { return bookmarks; }
		}
		#endregion
		#region Comments
		public CommentCollection Comments {
			[System.Diagnostics.DebuggerStepThrough]
			get { return comments; }
		}
		#endregion
		#region RangePermissions
		public RangePermissionCollection RangePermissions {
			[System.Diagnostics.DebuggerStepThrough]
			get { return rangePermissions; }
		}
		#endregion
		#region HyperlinkInfos
		public HyperlinkInfoCollection HyperlinkInfos {
			[System.Diagnostics.DebuggerStepThrough]
			get { return hyperlinkInfos; }
		}
		#endregion
		#region CustomMarks
		public CustomMarkCollection CustomMarks {
			[System.Diagnostics.DebuggerStepThrough]
			get { return customMarks; }
		}
		#endregion
		#region TextBoxes
		protected internal List<TextBoxContentType> TextBoxes {
			[System.Diagnostics.DebuggerStepThrough]
			get { return textBoxes; }
		}
		#endregion
		#region DocumentModel
		public DocumentModel DocumentModel {
			[System.Diagnostics.DebuggerStepThrough]
			get { return documentModel; }
		}
		#endregion
		IDocumentModel IDocumentModelPart.DocumentModel { get { return this.DocumentModel; } }
		#region VisibleTextFilter
		public IVisibleTextFilter VisibleTextFilter {
			[System.Diagnostics.DebuggerStepThrough]
			get { return visibleTextFilter; }
		}
		#endregion
		#region NavigationVisibleTextFilter
		public IVisibleTextFilter NavigationVisibleTextFilter {
			[System.Diagnostics.DebuggerStepThrough]
			get { return navigationVisibleTextFilter; }
		}
		#endregion
		#region DocumentStartLogPosition
		public DocumentLogPosition DocumentStartLogPosition {
			[System.Diagnostics.DebuggerStepThrough]
			get { return DocumentLogPosition.MinValue; }
		}
		#endregion
		#region DocumentEndLogPosition
		public DocumentLogPosition DocumentEndLogPosition {
			get {
				Paragraph lastParagraph = Paragraphs.Last;
				return lastParagraph.LogPosition + lastParagraph.Length - 1;
			}
		}
		#endregion
		public TableCellsManager TableCellsManager { get { return myTables; } }
		#region LastInsertedRunInfo
		public LastInsertedRunInfo LastInsertedRunInfo {
			[System.Diagnostics.DebuggerStepThrough]
			get { return DocumentModel.GetLastInsertedRunInfo(this); }
		}
		#endregion
		#region LastInsertedInlinePictureRunInfo
		public LastInsertedInlinePictureRunInfo LastInsertedInlinePictureRunInfo {
			[System.Diagnostics.DebuggerStepThrough]
			get { return DocumentModel.GetLastInsertedInlinePictureRunInfo(this); }
		}
		#endregion
		#region LastInsertedSeparatorRunInfo
		public LastInsertedSeparatorRunInfo LastInsertedSeparatorRunInfo {
			[System.Diagnostics.DebuggerStepThrough]
			get { return ((DocumentModel)DocumentModel).GetLastInsertedSeparatorRunInfo(this); }
		}
		#endregion
		#region LastInsertedFloatingObjectAnchorRunInfo
		public LastInsertedFloatingObjectAnchorRunInfo LastInsertedFloatingObjectAnchorRunInfo {
			[System.Diagnostics.DebuggerStepThrough]
			get { return DocumentModel.GetLastInsertedFloatingObjectAnchorRunInfo(this); }
		}
		#endregion
		public bool IsEmpty { get { return DocumentEndLogPosition - DocumentStartLogPosition <= 0; } }
		internal bool SuppressTableIntegrityCheck { get { return suppressTableIntegrityCheck; } set { suppressTableIntegrityCheck = value; } }
		internal SpellCheckerManager SpellCheckerManager {
			get { return spellCheckerManager; }
			set {
				Guard.ArgumentNotNull(value, "SpellCheckerManager");
				spellCheckerManager = value;
			}
		}
		public FieldUpdater FieldUpdater { get { return fieldUpdater; } }
		#endregion
		public virtual void Clear() {
			RunIndex count = new RunIndex(runs.Count);
			for (RunIndex i = RunIndex.Zero; i < count; i++) {
				IDisposable run = runs[i] as IDisposable;
				if (run != null)
					run.Dispose();
			}
			this.runs.Clear();
			this.paragraphs.Clear();
			this.tables.Clear();
			this.fields.Clear();
			this.comments.Clear();
			this.bookmarks.Clear();
			this.rangePermissions.Clear();
			this.myTables.Clear();
			this.hyperlinkInfos.Clear();
			this.customMarks.Clear();
			this.textBoxes.Clear();
			this.textBuffer.Clear();
			this.spellCheckerManager.Clear();
		}
		protected virtual void BookmarksClear() {
		}
		protected internal List<Field> CollectFieldsToProcess(Field parent) {
			List<Field> result = new List<Field>();
			int count = Fields.Count;
			for (int i = 0; i < count; i++) {
				if (Fields[i].Parent == parent)
					result.Add(Fields[i]);
			}
			return result;
		}
		protected internal void RemoveFieldWithCode(Field field) {
			DocumentModel.BeginUpdate();
			RemoveField(field);
			DeleteContent(GetRunLogPosition(field.LastRunIndex), 1, false);
			DocumentLogPosition startLogPosition = GetRunLogPosition(field.FirstRunIndex);
			DeleteContent(startLogPosition, GetRunLogPosition(field.Result.Start) - startLogPosition, false);
			DocumentModel.EndUpdate();
		}
		public virtual void ProcessFieldsRecursive(Field parent, Function<bool, Field> validateField) {
			Guard.ArgumentNotNull(validateField, "validateField");
			BookmarksClear();
			bool hasDeletedFields;
			do {
				hasDeletedFields = false;
				List<Field> fields = CollectFieldsToProcess(parent);
				int count = fields.Count;
				for (int i = 0; i < count; i++) {
					if (fields[i].DisableUpdate)
						continue;
					if (!validateField(fields[i])) {
						RemoveFieldWithCode(fields[i]);
						hasDeletedFields = true;
					}
					else
						ProcessFieldsRecursive(fields[i], validateField);
				}
			} while (hasDeletedFields);
		}
		protected virtual FieldUpdater CreateFieldUpdater() {
			return new FieldUpdater(this);
		}
		protected internal SpellCheckerManager CreateSpellCheckerManager() {
			return contentType.CreateSpellCheckerManager(this);
		}
		internal string GetRunText(RunIndex runIndex) {
			TextRunBase run = Runs[runIndex];
			return run.GetTextFast(textBuffer);
		}
		internal string GetRunNonEmptyText(RunIndex runIndex) {
			TextRunBase run = Runs[runIndex];
			return run.GetNonEmptyText(textBuffer);
		}
		public string GetRunPlainText(RunIndex runIndex) {
			TextRunBase run = Runs[runIndex];
			return run.GetPlainText(textBuffer);
		}
		public DocumentLogPosition GetRunLogPosition(RunIndex runIndex) {
			TextRunBase run = Runs[runIndex];
			DocumentLogPosition logPosition = run.Paragraph.LogPosition;
			for (RunIndex i = run.Paragraph.FirstRunIndex; i < runIndex; i++)
				logPosition += runs[i].Length;
			return logPosition;
		}
		public DocumentLogPosition GetRunLogPosition(TextRunBase run) {
			Paragraph paragraph = run.Paragraph;
			DocumentLogPosition logPosition = paragraph.LogPosition;
			RunIndex endRunIndex = paragraph.LastRunIndex;
			for (RunIndex i = paragraph.FirstRunIndex; i <= endRunIndex; i++) {
				if (runs[i] != run)
					logPosition += runs[i].Length;
				else
					break;
			}
			return logPosition;
		}
		internal string GetTextFromSingleRun(FormatterPosition startPos, FormatterPosition endPos) {
			TextRunBase run = Runs[startPos.RunIndex];
			return run.GetText(textBuffer, startPos.Offset, endPos.Offset);
		}
		internal string GetPlainText(DocumentModelPosition startPos, DocumentModelPosition endPos) {
			if (endPos.LogPosition - startPos.LogPosition <= 0)
				return String.Empty;
			TextRunBase run = Runs[startPos.RunIndex];
			if (startPos.RunIndex == endPos.RunIndex)
				return run.GetPlainText(textBuffer, startPos.RunOffset, endPos.RunOffset - 1);
			StringBuilder sb = new StringBuilder();
			if (startPos.LogPosition == run.Paragraph.LogPosition)
				GetNumberingListText(startPos.RunIndex, sb);
			sb.Append(run.GetPlainText(textBuffer, startPos.RunOffset, run.Length - 1));
			RunIndex endPosIndex = endPos.RunIndex;
			for (RunIndex i = startPos.RunIndex + 1; i < endPosIndex; i++) {
				GetNumberingListText(i, sb);
				sb.Append(GetRunPlainText(i));
			}
			if (endPos.RunOffset > 0) {
				run = Runs[endPos.RunIndex];
				GetNumberingListText(endPos.RunIndex, sb);
				sb.Append(run.GetPlainText(textBuffer, 0, endPos.RunOffset - 1));
			}
			return sb.ToString();
		}
		internal LangInfo? GetLanguageInfo(DocumentModelPosition start, DocumentModelPosition end) {
			LangInfo result = runs[start.RunIndex].LangInfo;
			if (start.RunIndex != end.RunIndex) {
				for (RunIndex i = start.RunIndex + 1; i < end.RunIndex; i++) {
					if (!result.Equals(Runs[i].LangInfo))
						return null;
				}
			}
			return result;
		}
		internal bool ShouldCheckWord(DocumentModelPosition start, DocumentModelPosition end) {
			if (!runs[start.RunIndex].NoProof)
				return true;
			if (start.RunIndex != end.RunIndex) {
				for (RunIndex i = start.RunIndex + 1; i < end.RunIndex; i++) {
					if (!Runs[i].NoProof)
						return true;
				}
			}
			return false;
		}
		internal string GetPlainText2(FormatterPosition startPos, FormatterPosition endPos) {
			TextRunBase run = Runs[startPos.RunIndex];
			if (startPos.RunIndex == endPos.RunIndex)
				return run.GetPlainText(textBuffer, startPos.Offset, endPos.Offset);
			StringBuilder sb = new StringBuilder();
			if (run.Paragraph.FirstRunIndex == startPos.RunIndex && startPos.Offset <= 0)
				GetNumberingListText(startPos.RunIndex, sb);
			sb.Append(run.GetPlainText(textBuffer, startPos.Offset, run.Length - 1));
			RunIndex endPosIndex = endPos.RunIndex;
			for (RunIndex i = startPos.RunIndex + 1; i < endPosIndex; i++) {
				GetNumberingListText(i, sb);
				sb.Append(GetRunPlainText(i));
			}
			run = Runs[endPos.RunIndex];
			GetNumberingListText(endPos.RunIndex, sb);
			sb.Append(run.GetPlainText(textBuffer, 0, endPos.Offset));
			return sb.ToString();
		}
		internal string GetPlainText2(DocumentModelPosition startPos, DocumentModelPosition endPos) {
			FormatterPosition startFormatterPos = PositionConverter.ToFormatterPosition(startPos);
			FormatterPosition endFormatterPos = PositionConverter.ToFormatterPosition(endPos);
			return GetPlainText2(startFormatterPos, endFormatterPos);
		}
		internal string GetFilteredPlainText(DocumentModelPosition start, DocumentModelPosition end, Predicate<RunIndex> predicate) {
			if (end.LogPosition - start.LogPosition <= 0)
				return String.Empty;
			StringBuilder result = new StringBuilder();
			TextRunBase startRun = Runs[start.RunIndex];
			if (start.RunIndex == end.RunIndex) {
				if (predicate(start.RunIndex))
					return startRun.GetPlainText(textBuffer, start.RunOffset, end.RunOffset - 1);
				return String.Empty;
			}
			if (predicate(start.RunIndex))
				result.Append(startRun.GetPlainText(textBuffer, start.RunOffset, startRun.Length - 1));
			for (RunIndex i = start.RunIndex + 1; i < end.RunIndex; i++) {
				if (predicate(i))
					result.Append(GetRunPlainText(i));
			}
			if (end.RunOffset > 0 && predicate(end.RunIndex))
				result.Append(Runs[end.RunIndex].GetPlainText(textBuffer, 0, end.RunOffset - 1));
			return result.ToString();
		}
		internal void GetNumberingListText(RunIndex index, StringBuilder sb) {
			Paragraph paragraph = Runs[index].Paragraph;
			if (paragraph.IsInList() && index == paragraph.FirstRunIndex)
				sb.Append(paragraph.GetNumberingListText());
		}
		internal RunInfo FindRunInfo(DocumentLogPosition logPositionStart, int length) {
			RunInfo result = new RunInfo(this);
			CalculateRunInfoStart(logPositionStart, result);
			CalculateRunInfoEnd(logPositionStart + length - 1, result);
			return result;
		}
		internal void CalculateRunInfoStart(DocumentLogPosition logPosition, RunInfo result) {
			RunIndex runIndex;
			result.Start.LogPosition = logPosition;
			logPosition = Algorithms.Min(logPosition, DocumentEndLogPosition);
			result.Start.ParagraphIndex = FindParagraphIndex(logPosition, true);
			result.Start.RunStartLogPosition = FindRunStartLogPosition(Paragraphs[result.Start.ParagraphIndex], logPosition, out runIndex);
			result.Start.RunIndex = runIndex;
		}
		internal void CalculateRunInfoEnd(DocumentLogPosition logPosition, RunInfo result) {
			RunIndex runIndex;
			result.End.LogPosition = logPosition;
			logPosition = Algorithms.Min(logPosition, DocumentEndLogPosition);
			result.End.ParagraphIndex = FindParagraphIndex(logPosition, true);
			result.End.RunStartLogPosition = FindRunStartLogPosition(Paragraphs[result.End.ParagraphIndex], logPosition, out runIndex);
			result.End.RunIndex = runIndex;
		}
		public DocumentLogPosition FindRunStartLogPosition(Paragraph paragraph, DocumentLogPosition logPosition, out RunIndex runIndex) {
			if (paragraph.EndLogPosition == logPosition) {
				runIndex = paragraph.LastRunIndex;
				return logPosition;
			}
			DocumentLogPosition pos = paragraph.LogPosition;
			RunIndex lastRunIndex = paragraph.LastRunIndex;
			for (RunIndex i = paragraph.FirstRunIndex; i <= lastRunIndex; i++) {
				TextRunBase run = runs[i];
				DocumentLogPosition nextPos = pos + run.Length;
				if (logPosition >= pos && logPosition < nextPos) {
					runIndex = i;
					return pos;
				}
				pos = nextPos;
			}
			Exceptions.ThrowArgumentException("logPosition", logPosition);
			runIndex = new RunIndex(-1);
			return new DocumentLogPosition(-1);
		}
		public ParagraphIndex FindParagraphIndex(DocumentLogPosition logPosition) {
			return FindParagraphIndex(logPosition, true);
		}
		internal ParagraphIndex FindParagraphIndex(DocumentLogPosition logPosition, bool strictSearch) {
			ParagraphIndex result = FindParagraphIndexCore(logPosition);
			if (result < new ParagraphIndex(0)) {
				if (result == new ParagraphIndex(~Paragraphs.Count))
					return new ParagraphIndex(Paragraphs.Count - 1);
				else {
					Exceptions.ThrowArgumentException("logPosition", logPosition);
					return new ParagraphIndex(-1);
				}
			}
			else
				return result;
		}
		protected internal ParagraphIndex FindParagraphIndexCore(DocumentLogPosition logPosition) {
			return Paragraphs.SearchByLogPosition(logPosition);
		}
		internal Paragraph FindParagraph(DocumentLogPosition logPosition) {
			return Paragraphs[FindParagraphIndex(logPosition)];
		}
		protected internal void RemoveField(Field field) {
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				if (HyperlinkInfos.IsHyperlink(field.Index))
					RemoveHyperlinkInfo(field.Index);
				RemoveFieldHistoryItem item = new RemoveFieldHistoryItem(this);
				item.RemovedFieldIndex = field.Index;
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		internal void AddFieldToTable(Field field, int index) {
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				InsertFieldHistoryItem item = new InsertFieldHistoryItem(this);
				item.InsertedFieldIndex = index;
				item.InsertedField = field;
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		RunIndex InsertObjectCore(ObjectInserter inserter, ParagraphIndex paragraphIndex, DocumentLogPosition logPosition) {
			return InsertObjectCore(inserter, paragraphIndex, logPosition, false);
		}
		protected RunIndex InsertObjectCore(ObjectInserter inserter, ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, bool forceVisible) {
			if (inserter.CanMerge(logPosition)) {
				inserter.Merge(logPosition, paragraphIndex);
				return LastInsertedRunInfo.RunIndex;
			}
			Paragraph paragraph = Paragraphs[paragraphIndex];
			RunIndex runIndex;
			DocumentLogPosition pos = FindRunStartLogPosition(paragraph, logPosition, out runIndex);
			RunIndex newRunIndex;
			if (logPosition == pos)
				newRunIndex = runIndex;
			else {
				SplitTextRun(paragraph.Index, runIndex, logPosition - pos);
				newRunIndex = runIndex + 1;
			}
			inserter.PerformInsert(paragraph, newRunIndex, logPosition, forceVisible);
			return newRunIndex;
		}
		public void InsertParagraphCore(InputPosition pos) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			RunIndex runIndex = InsertParagraphCore(pos.ParagraphIndex, pos.LogPosition, false);
			InheritParagraphRunStyle(pos, (ParagraphRun)Runs[runIndex]);
		}
		internal void InsertParagraphCoreNoInheritParagraphRunStyle(InputPosition pos) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			InsertParagraphCore(pos.ParagraphIndex, pos.LogPosition, false);
			pos.LogPosition++;
			pos.ParagraphIndex++;
		}
		public void InsertSectionParagraphCore(InputPosition pos) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			RunIndex runIndex = InsertSectionParagraphCore(pos.ParagraphIndex, pos.LogPosition, false);
			InheritParagraphRunStyle(pos, (ParagraphRun)Runs[runIndex]);
		}
		internal void InheritParagraphRunStyleCore(InputPosition pos, ParagraphRun paragraphRun) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			CharacterProperties characterProperties = paragraphRun.CharacterProperties;
			characterProperties.CopyFrom(new CharacterFormattingBase(documentModel.MainPieceTable, documentModel, pos.CharacterFormatting.InfoIndex, pos.CharacterFormatting.OptionsIndex));
			paragraphRun.CharacterStyleIndex = pos.CharacterStyleIndex;
		}
		internal void InheritParagraphRunStyle(InputPosition pos, ParagraphRun paragraphRun) {
			InheritParagraphRunStyleCore(pos, paragraphRun);
			pos.LogPosition++;
			pos.ParagraphIndex++;
		}
		internal RunIndex InsertParagraphCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition) {
			return InsertParagraphCore(paragraphIndex, logPosition, false);
		}
		internal RunIndex InsertParagraphCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, bool forceVisible) {
			textBuffer.Append(Characters.ParagraphMark);
			return InsertObjectCore(paragraphInserter, paragraphIndex, logPosition, forceVisible);
		}
		internal RunIndex InsertSectionParagraphCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, bool forceVisible) {
			textBuffer.Append(Characters.SectionMark);
			SectionInserter inserter = new SectionInserter(this);
			return InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
		}
		internal InlinePictureRun InsertInlineImageCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, OfficeImage image, float scaleX, float scaleY) {
			return InsertInlineImageCore(paragraphIndex, logPosition, image, scaleX, scaleY, false, false);
		}
		internal InlinePictureRun InsertInlineImageCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, OfficeImage image, float scaleX, float scaleY, bool useScreenDpi) {
			return InsertInlineImageCore(paragraphIndex, logPosition, image, scaleX, scaleY, useScreenDpi, false);
		}
		internal InlinePictureRun InsertInlineImageCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, OfficeImage image, float scaleX, float scaleY, bool useScreenDpi, bool forceVisible) {
			DocumentModel.ImageCache.RegisterImage(image.EncapsulatedOfficeNativeImage);
			textBuffer.Append(Characters.ObjectMark);
			InlinePictureInserter inserter = new InlinePictureInserter(this, image, scaleX, scaleY, useScreenDpi);
			RunIndex newRunIndex = InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
			return (InlinePictureRun)Runs[newRunIndex];
		}
		internal FloatingObjectAnchorRun InsertFloatingObjectAnchorCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition) {
			return InsertFloatingObjectAnchorCore(paragraphIndex, logPosition, false);
		}
		internal FloatingObjectAnchorRun InsertFloatingObjectAnchorCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, bool forceVisible) {
			textBuffer.Append(Characters.FloatingObjectMark);
			FloatingObjectAnchorInserter inserter = new FloatingObjectAnchorInserter(this);
			InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
			return LastInsertedFloatingObjectAnchorRunInfo.Run;
		}
		internal FloatingObjectAnchorRun InsertFloatingObjectAnchorCore(InputPosition pos) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			InsertFloatingObjectAnchorCore(pos.ParagraphIndex, pos.LogPosition, false);
			LastInsertedFloatingObjectAnchorRunInfo lastInsertedFloatingObjectAnchorRunInfo = LastInsertedFloatingObjectAnchorRunInfo;
			FloatingObjectAnchorRun run = lastInsertedFloatingObjectAnchorRunInfo.Run;
			run.ApplyFormatting(pos);
			pos.LogPosition++;
			return run;
		}
		internal void InsertInlineCustomObjectCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, IInlineCustomObject customObject, float scaleX, float scaleY, bool forceVisible) {
			textBuffer.Append(Characters.ObjectMark);
			InlineCustomObjectInserter inserter = new InlineCustomObjectInserter(this, customObject, scaleX, scaleY);
			InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
		}
		internal RunIndex InsertFieldResultEndRunCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition) {
			return InsertFieldResultEndRunCore(paragraphIndex, logPosition, false);
		}
		internal RunIndex InsertFieldResultEndRunCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, bool forceVisible) {
			textBuffer.Append('\u203A');
			FieldResultEndRunInserter inserter = new FieldResultEndRunInserter(this);
			return InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
		}
		internal RunIndex InsertFieldResultEndRunCore(InputPosition pos) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			RunIndex runIndex = InsertFieldResultEndRunCore(pos.ParagraphIndex, pos.LogPosition);
			Runs[runIndex].ApplyFormatting(pos);
			pos.LogPosition++;
			return runIndex;
		}
		internal RunIndex InsertFieldCodeStartRunCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition) {
			return InsertFieldCodeStartRunCore(paragraphIndex, logPosition, false);
		}
		internal RunIndex InsertFieldCodeStartRunCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, bool forceVisible) {
			textBuffer.Append('{');
			FieldCodeStartRunInserter inserter = new FieldCodeStartRunInserter(this);
			return InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
		}
		internal RunIndex InsertFieldCodeStartRunCore(InputPosition pos) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			RunIndex runIndex = InsertFieldCodeStartRunCore(pos.ParagraphIndex, pos.LogPosition);
			Runs[runIndex].ApplyFormatting(pos);
			pos.LogPosition++;
			return runIndex;
		}
		internal RunIndex InsertFieldCodeEndRunCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition) {
			return InsertFieldCodeEndRunCore(paragraphIndex, logPosition, false);
		}
		internal RunIndex InsertFieldCodeEndRunCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, bool forceVisible) {
			textBuffer.Append('}');
			FieldCodeEndRunInserter inserter = new FieldCodeEndRunInserter(this);
			return InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
		}
		internal RunIndex InsertFieldCodeEndRunCore(InputPosition pos) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			RunIndex runIndex = InsertFieldCodeEndRunCore(pos.ParagraphIndex, pos.LogPosition);
			Runs[runIndex].ApplyFormatting(pos);
			pos.LogPosition++;
			return runIndex;
		}
		public void InsertSeparatorTextRunCore(InputPosition pos) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			InsertSeparatorTextRunCore(pos.ParagraphIndex, pos.LogPosition);
			LastInsertedSeparatorRunInfo.Run.ApplyFormatting(pos);
			pos.LogPosition++;
		}
		protected internal virtual RunIndex InsertSeparatorTextRunCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition) {
			TextBuffer.Append(SeparatorTextRun.SeparatorText);
			SeparatorTextRunInserter inserter = new SeparatorTextRunInserter(this);
			return InsertObjectCore(inserter, paragraphIndex, logPosition, false);
		}
		internal void AddParagraphToList(ParagraphIndex paragraphIndex, NumberingListIndex numberingListIndex, int listLevelIndex) {
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				AddParagraphToListHistoryItem item = new AddParagraphToListHistoryItem(this);
				item.ParagraphIndex = paragraphIndex;
				item.NumberingListIndex = numberingListIndex;
				item.ListLevelIndex = listLevelIndex;
				documentModel.History.Add(item);
				item.Execute();
			}
		}
		internal void ApplyListLevelIndexToParagraph(Paragraph paragraph, int levelIndex) {
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				ChangeParagraphListLevelHistoryItem item = new ChangeParagraphListLevelHistoryItem(this, paragraph.Index, paragraph.GetOwnListLevelIndex(), levelIndex);
				documentModel.History.Add(item);
				item.Execute();
			}
		}
		internal void RemoveParagraphFromList(ParagraphIndex paragraphIndex) {
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				RemoveParagraphFromListHistoryItem item = new RemoveParagraphFromListHistoryItem(this);
				item.ParagraphIndex = paragraphIndex;
				documentModel.History.Add(item);
				item.Execute();
			}
		}
		public void InsertTextCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, string text) {
			InsertTextCore(paragraphIndex, logPosition, text, false);
		}
		public void InsertTextCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, string text, bool forceVisible) {
			InsertTextCoreWithoutSplit(paragraphIndex, logPosition, text, forceVisible);
			if (!DocumentModel.DeferredChanges.IsSetContentMode) {
				if (DocumentModel.IsUpdateLocked)
					DocumentModel.DeferredChanges.GetRunIndicesForSplit(this).Add(LastInsertedRunInfo.RunIndex);
				else
					SplitTextRunByCharset(LastInsertedRunInfo.RunIndex);
			}
		}
		public void InsertTextCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, char text) {
			InsertTextCore(paragraphIndex, logPosition, text, false);
		}
		public void InsertTextCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, char text, bool forceVisible) {
			textBuffer.Append(text);
			textInserter.TextLength = 1;
			InsertObjectCore(textInserter, paragraphIndex, logPosition, forceVisible);
			if (!DocumentModel.DeferredChanges.IsSetContentMode) {
				if (DocumentModel.IsUpdateLocked)
					DocumentModel.DeferredChanges.GetRunIndicesForSplit(this).Add(LastInsertedRunInfo.RunIndex);
				else
					SplitTextRunByCharset(LastInsertedRunInfo.RunIndex);
			}
		}
		protected internal void InsertTextCoreWithoutSplit(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, string text, bool forceVisible) {
			textBuffer.Append(text);
			textInserter.TextLength = text.Length;
			InsertObjectCore(textInserter, paragraphIndex, logPosition, forceVisible);
		}
		protected internal void InsertLayoutDependentTextRun(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, FieldResultFormatting formatting) {
			string text = "#";
			textBuffer.Append(text);
			layoutDependentTextInserter.TextLength = text.Length;
			layoutDependentTextInserter.FieldResultFormatting = formatting;
			InsertObjectCore(layoutDependentTextInserter, paragraphIndex, logPosition);
		}
		protected internal FootNoteRun InsertFootNoteRun(InputPosition pos, int noteIndex) {
			InsertFootNoteRun(pos.ParagraphIndex, pos.LogPosition, noteIndex);
			LastInsertedRunInfo lastInsertedRunInfo = LastInsertedRunInfo;
			FootNoteRun run = (FootNoteRun)lastInsertedRunInfo.Run;
			run.ApplyFormatting(pos);
			pos.LogPosition++;
			return run;
		}
		protected internal FootNoteRun InsertFootNoteRun(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, int noteIndex) {
			InsertFootNoteRunCore(paragraphIndex, logPosition, FootNoteNumberResultFormatting.Instance, noteIndex, footNoteRunInserter);
			return (FootNoteRun)LastInsertedRunInfo.Run;
		}
		protected internal EndNoteRun InsertEndNoteRun(InputPosition pos, int noteIndex) {
			InsertEndNoteRun(pos.ParagraphIndex, pos.LogPosition, noteIndex);
			LastInsertedRunInfo lastInsertedRunInfo = LastInsertedRunInfo;
			EndNoteRun run = (EndNoteRun)lastInsertedRunInfo.Run;
			run.ApplyFormatting(pos);
			pos.LogPosition++;
			return run;
		}
		protected internal EndNoteRun InsertEndNoteRun(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, int noteIndex) {
			InsertFootNoteRunCore(paragraphIndex, logPosition, EndNoteNumberResultFormatting.Instance, noteIndex, endNoteRunInserter);
			return (EndNoteRun)LastInsertedRunInfo.Run;
		}
		protected internal void InsertFootNoteRunCore<T>(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, FieldResultFormatting formatting, int noteIndex, FootNoteRunInserterBase<T> inserter) where T : FootNoteBase<T> {
			string text = "#";
			textBuffer.Append(text);
			inserter.TextLength = text.Length;
			inserter.FieldResultFormatting = formatting;
			inserter.NoteIndex = noteIndex;
			InsertObjectCore(inserter, paragraphIndex, logPosition);
		}
		public void InsertTextCore(InputPosition pos, string text) {
			InsertTextCore(pos, text, false);
		}
		public void InsertTextCore(InputPosition pos, string text, bool forceVisible) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			if (textInserter.CanMerge(pos.LogPosition)) {
				if (!LastInsertedRunInfo.Run.MatchFormatting(pos.CharacterFormatting.Info, pos.CharacterFormatting.Options, pos.CharacterStyleIndex) ||
					(forceVisible && IsPosHidden(pos)))
					DocumentModel.ResetMerging();
			}
			InsertTextCoreWithoutSplit(pos.ParagraphIndex, pos.LogPosition, text, forceVisible);
			LastInsertedRunInfo lastInsertedRunInfo = LastInsertedRunInfo;
			lastInsertedRunInfo.Run.ApplyFormatting(pos, forceVisible);
			if (!DocumentModel.DeferredChanges.IsSetContentMode)
				SplitTextRunByCharset(lastInsertedRunInfo.RunIndex);
			pos.LogPosition += text.Length;
		}
		internal void InsertTextCoreNoResetMergeNoApplyFormatting(InputPosition pos, string text, bool forceVisible) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			InsertTextCoreWithoutSplit(pos.ParagraphIndex, pos.LogPosition, text, forceVisible);
			LastInsertedRunInfo lastInsertedRunInfo = LastInsertedRunInfo;
			if (!DocumentModel.DeferredChanges.IsSetContentMode)
				SplitTextRunByCharset(lastInsertedRunInfo.RunIndex);
			pos.LogPosition += text.Length;
		}
		bool IsPosHidden(InputPosition pos) {
			if (pos.CharacterFormatting.Options.UseHidden)
				return pos.CharacterFormatting.Hidden;
			else
				return false;
		}
		protected internal virtual void SplitTextRunsByCharset() {
			if (!DocumentModel.BehaviorOptions.UseFontSubstitution)
				return;
			RunMergedCharacterPropertiesCachedResult cachedResult = new RunMergedCharacterPropertiesCachedResult();
			for (RunIndex i = new RunIndex(runs.Count - 1); i >= RunIndex.Zero; i--) {
				TextRunBase run = runs[i];
				if (run is TextRun) {
					run.EnsureMergedCharacterFormattingCacheIndexCalculated(cachedResult);
					SplitTextRunByCharsetCore(i);
				}
			}
		}
		protected internal virtual void SplitTextRunByCharset(RunIndex runIndex) {
			if (!DocumentModel.BehaviorOptions.UseFontSubstitution)
				return;
			SplitTextRunByCharsetCore(runIndex);
		}
		protected internal virtual void SplitTextRunByCharsetCore(RunIndex runIndex) {
			TextRunBase run = Runs[runIndex];
			FontCache fontCache = DocumentModel.FontCache;
			string sourceRunFontName = run.FontName;
			FontCharacterSet sourceFontCharacterSet = fontCache.GetFontCharacterSet(sourceRunFontName);
			if (sourceFontCharacterSet == null)
				return;
			string text = run.GetTextFast(textBuffer);
			int splitOffset = GetFirstSplitChar(sourceFontCharacterSet, text);
			if (splitOffset < 0)
				return;
			ParagraphIndex paragraphIndex = run.Paragraph.Index;
			int textLength = text.Length;
			int runStartOffset = splitOffset;
			if (splitOffset > 0) {
				SplitTextRun(paragraphIndex, runIndex, splitOffset);
				runIndex++;
			}
			DocumentCapabilitiesOptions capabilities = DocumentModel.DocumentCapabilities;
			DocumentCapability characterFormattingCapability = capabilities.CharacterFormatting;
			capabilities.CharacterFormatting = DocumentCapability.Default;
			try {
				string prevRunFontName = sourceRunFontName;
				while (splitOffset < textLength) {
					string subsFontName = sourceFontCharacterSet.ContainsChar(text[splitOffset]) ? sourceRunFontName : fontCache.FindSubstituteFont(sourceRunFontName, text[splitOffset]);
					if (subsFontName != prevRunFontName) {
						if (splitOffset - runStartOffset > 0) {
							SplitTextRun(paragraphIndex, runIndex, splitOffset - runStartOffset);
							if (prevRunFontName != sourceRunFontName)
								Runs[runIndex].FontName = prevRunFontName;
							runStartOffset = splitOffset;
							runIndex++;
						}
						prevRunFontName = subsFontName;
					}
					splitOffset++;
				}
				if (splitOffset - runStartOffset > 0 && prevRunFontName != sourceRunFontName)
					Runs[runIndex].FontName = prevRunFontName;
			}
			finally {
				capabilities.CharacterFormatting = characterFormattingCapability;
			}
		}
		protected int GetFirstSplitChar(FontCharacterSet fontCharacterSet, string text) {
			int count = text.Length;
			for (int i = 0; i < count; i++)
				if (!fontCharacterSet.ContainsChar(text[i]))
					return i;
			return -1;
		}
		internal void AppendText(InputPosition pos, char ch) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			InsertTextCore(pos, new String(ch, 1));
		}
		internal void AppendText(InputPosition pos, string text) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			InsertTextCore(pos, text);
		}
		internal void AppendImage(InputPosition pos, OfficeImage image, float scaleX, float scaleY, bool useScreenDpi) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			InsertInlineImageCore(pos.ParagraphIndex, pos.LogPosition, image, scaleX, scaleY, useScreenDpi);
			LastInsertedInlinePictureRunInfo lastInsertedInlinePictureRunInfo = LastInsertedInlinePictureRunInfo;
			InlinePictureRun run = lastInsertedInlinePictureRunInfo.Run;
			run.ApplyFormatting(pos);
			pos.LogPosition++;
		}
		public void AppendImage(InputPosition pos, OfficeImage image, float scaleX, float scaleY) {
			AppendImage(pos, image, scaleX, scaleY, false);
		}
		public void AppendCustomRun(InputPosition pos, ICustomRunObject customRunObject) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			InsertCustomRunCore(pos.ParagraphIndex, pos.LogPosition, customRunObject, false);
			CustomRun run = (CustomRun)Runs[new RunIndex(Runs.Count - 2)];
			run.ApplyFormatting(pos);
			pos.LogPosition++;
		}
		public void AppendDataContainerRun(InputPosition pos, IDataContainer dataContainer) {
			Debug.Assert(Object.ReferenceEquals(pos.PieceTable, this));
			InsertDataContainerRunCore(pos.ParagraphIndex, pos.LogPosition, dataContainer, false);
			DataContainerRun run = (DataContainerRun)Runs[new RunIndex(Runs.Count - 2)];
			run.ApplyFormatting(pos);
			pos.LogPosition++;
		}
		internal void RecalcParagraphsPositions(ParagraphIndex from, int deltaLength, int deltaRunIndex) {
			if (multipleRunSplitCount > 0) {
				RecalcParagraphsPositionsCore(lastShiftedParagraphIndex + 1, from, lastDeltaLength, lastDeltaRunIndex);
			}
			Paragraph paragraph = Paragraphs[from];
			paragraph.SetLength(paragraph.Length + deltaLength);
			RunIndex newLastRunIndex = paragraph.LastRunIndex + deltaRunIndex;
#if UseOldIndicies
			Paragraphs[from].SetLastRunIndex(newLastRunIndex);
#endif
			paragraph.SetRelativeLastRunIndexWithoutCheck(newLastRunIndex);
			if (multipleRunSplitCount == 0) {
				RecalcParagraphsPositionsCore(from + 1, deltaLength, deltaRunIndex);
			}
			else {
				lastShiftedParagraphIndex = from;
				lastDeltaLength += deltaLength;
				lastDeltaRunIndex += deltaRunIndex;
			}
		}
		internal void RecalcParagraphsPositionsCore(ParagraphIndex from, int deltaLength, int deltaRunIndex) {
			ParagraphIndex to = new ParagraphIndex(Paragraphs.Count - 1);
			RecalcParagraphsPositionsCore(from, to, deltaLength, deltaRunIndex);
		}
		internal void RecalcParagraphsPositionsCore(ParagraphIndex from, ParagraphIndex to, int deltaLength, int deltaRunIndex) {
			Paragraphs.RecalcParagraphsPositionsCore(from, to, deltaLength, deltaRunIndex);
		}
		internal void RecalcFieldPosition(Field field, RunIndex runIndex, int deltaRunIndex) {
			if (field.LastRunIndex < runIndex)
				return;
			if (field.Code.Start < runIndex && field.Code.End >= runIndex) {
				field.Code.ShiftEndRunIndex(deltaRunIndex);
				field.Result.ShiftRunIndex(deltaRunIndex);
				Fields.ClearCounters();
			}
			else if (field.Result.Start <= runIndex && field.Result.End >= runIndex) {
				field.Result.ShiftEndRunIndex(deltaRunIndex);
			}
			else {
				field.Code.ShiftRunIndex(deltaRunIndex);
				field.Result.ShiftRunIndex(deltaRunIndex);
			}
		}
		internal void RecalcFieldsPositions(RunIndex runIndex, int deltaRunIndex) {
			int count = Fields.Count;
			if (count == 0 || Fields.Last.LastRunIndex < runIndex)
				return;
			for (int i = 0; i < count; i++)
				RecalcFieldPosition(Fields[i], runIndex, deltaRunIndex);
		}
		internal void RecalcFieldsIndices(int from, int deltaIndex) {
			int count = Fields.Count;
			for (int i = from; i < count; i++)
				Fields[i].Index += deltaIndex;
		}
		internal void ApplyCharacterStyleCore(DocumentLogPosition logPositionStart, int length, RunPropertyModifierBase modifier) {
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				RunInfo rinfo = ObtainAffectedRunInfo(logPositionStart, length);
				if (rinfo.Start.RunIndex >= new RunIndex(0) && rinfo.End.RunIndex >= rinfo.Start.RunIndex) {
					ChangeCharacterStyle(rinfo, modifier);
					TryToJoinRuns(rinfo);
				}
			}
		}
		internal void ApplyParagraphStyleCore(DocumentLogPosition logPositionStart, int length, RunPropertyModifierBase modifier) {
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				RunInfo rinfo = FindRunInfo(logPositionStart, length);
				ParagraphIndex endParagraphIndex = rinfo.End.ParagraphIndex;
				for (ParagraphIndex i = rinfo.Start.ParagraphIndex; i <= endParagraphIndex; i++)
					ChangeParagraphStyle(Paragraphs[i], modifier);
			}
		}
		void ChangeParagraphStyle(Paragraph paragraph, RunPropertyModifierBase modifier) {
			RunIndex lastRunIndex = paragraph.LastRunIndex;
			for (RunIndex i = paragraph.FirstRunIndex; i <= lastRunIndex; i++)
				modifier.ModifyTextRun(Runs[i], i);
		}
		internal RunInfo ObtainAffectedRunInfo(DocumentLogPosition logPositionStart, int length) {
			Debug.Assert(DocumentModel.IsUpdateLockedOrOverlapped);
			RunInfo rinfo = FindRunInfo(logPositionStart, length);
			if (rinfo.End.RunEndLogPosition != rinfo.End.LogPosition)
				SplitTextRun(rinfo.End.ParagraphIndex, rinfo.End.RunIndex, rinfo.End.RunOffset + 1);
			if (rinfo.Start.RunStartLogPosition != logPositionStart) {
				SplitTextRun(rinfo.Start.ParagraphIndex, rinfo.Start.RunIndex, rinfo.Start.RunOffset);
				rinfo.Start.RunStartLogPosition = rinfo.Start.LogPosition;
				rinfo.Start.RunIndex++;
				rinfo.End.RunIndex++;
			}
			return rinfo;
		}
		internal bool ObtainRunsPropertyValue<T>(DocumentLogPosition logPositionStart, int length, RunPropertyModifier<T> modifier, out T value) {
			RunInfo rinfo = FindRunInfo(logPositionStart, length);
			value = modifier.GetRunPropertyValue(Runs[rinfo.Start.RunIndex]);
			for (RunIndex i = rinfo.Start.RunIndex + 1; i <= rinfo.End.RunIndex; i++) {
				T runValue = modifier.GetRunPropertyValue(Runs[i]);
				if (!runValue.Equals(value))
					return false;
			}
			return true;
		}
		internal bool ObtainRunsPropertyValueIgnoreParagraphRun<T>(DocumentLogPosition logPositionStart, int length, RunPropertyModifier<T> modifier, out T value) {
			RunInfo rinfo = FindRunInfo(logPositionStart, length);
			RunIndex index = rinfo.Start.RunIndex;
			while ((Runs[index] is ParagraphRun) && (index < rinfo.End.RunIndex)) {
				index++;
			}
			value = modifier.GetRunPropertyValue(Runs[index]);
			for (RunIndex i = rinfo.Start.RunIndex + 1; i <= rinfo.End.RunIndex; i++) {
				TextRun run = Runs[i] as TextRun;
				if (run == null)
					continue;
				T runValue = modifier.GetRunPropertyValue(Runs[i]);
				if (!runValue.Equals(value))
					return false;
			}
			return true;
		}
		internal T ObtainMergedRunsPropertyValue<T>(DocumentLogPosition logPositionStart, int length, MergedRunPropertyModifier<T> modifier) {
			RunInfo rinfo = FindRunInfo(logPositionStart, length);
			RunIndex index = rinfo.Start.RunIndex;
			for (RunIndex i = index; i <= rinfo.End.RunIndex; i++) {
				if (modifier.CanModifyRun(Runs[i])) {
					index = i;
					break;
				}
			}
			T value = modifier.GetRunPropertyValue(Runs[index]);
			for (RunIndex i = index + 1; i <= rinfo.End.RunIndex; i++) {
				if (modifier.CanModifyRun(Runs[i])) {
					T runValue = modifier.GetRunPropertyValue(Runs[i]);
					value = modifier.Merge(value, runValue);
				}
			}
			return value;
		}
		internal MergedCharacterProperties ObtainMergedNumberingPropertyValue(DocumentLogPosition logPositionStart, int length, MergedRunPropertyModifier<MergedCharacterProperties> modifier) {
			RunInfo rinfo = FindRunInfo(logPositionStart, length);
			ParagraphIndex start = rinfo.Start.ParagraphIndex;
			MergedCharacterProperties value = modifier.Merge(Runs[Paragraphs[start].LastRunIndex].GetMergedCharacterProperties(), Paragraphs[start].GetNumerationCharacterProperties());
			for (ParagraphIndex i = start + 1; i <= rinfo.End.ParagraphIndex; i++) {
				MergedCharacterProperties runValue = modifier.Merge(Runs[Paragraphs[i].LastRunIndex].GetMergedCharacterProperties(), Paragraphs[i].GetNumerationCharacterProperties());
				value = modifier.Merge(value, runValue);
			}
			return value;
		}
		internal Nullable<T> ObtainParagraphsPropertyValue<T>(DocumentLogPosition logPositionStart, int length, ParagraphPropertyModifier<T> modifier) where T : struct {
			RunInfo rinfo = FindRunInfo(logPositionStart, length);
			T value = modifier.GetParagraphPropertyValue(Paragraphs[rinfo.Start.ParagraphIndex]);
			for (ParagraphIndex i = rinfo.Start.ParagraphIndex + 1; i <= rinfo.End.ParagraphIndex; i++) {
				T paragraphValue = modifier.GetParagraphPropertyValue(Paragraphs[i]);
				if (!paragraphValue.Equals(value))
					return null;
			}
			return value;
		}
		internal T ObtainMergedParagraphsPropertyValue<T>(DocumentLogPosition logPositionStart, int length, MergedParagraphPropertyModifier<T> modifier) {
			RunInfo rinfo = FindRunInfo(logPositionStart, length);
			T value = modifier.GetParagraphPropertyValue(Paragraphs[rinfo.Start.ParagraphIndex]);
			for (ParagraphIndex i = rinfo.Start.ParagraphIndex + 1; i <= rinfo.End.ParagraphIndex; i++) {
				T paragraphValue = modifier.GetParagraphPropertyValue(Paragraphs[i]);
				value = modifier.Merge(value, paragraphValue);
			}
			return value;
		}
		protected internal virtual Nullable<T> ObtainSectionsPropertyValue<T>(DocumentLogPosition logPositionStart, int length, SectionPropertyModifier<T> modifier) where T : struct {
			return contentType.ObtainSectionsPropertyValue<T>(logPositionStart, length, modifier);
		}
		internal void TryToJoinRuns(RunInfo rinfo) {
			Debug.Assert(DocumentModel.IsUpdateLockedOrOverlapped);
			ParagraphIndex endParagraphIndex = rinfo.End.ParagraphIndex;
			for (ParagraphIndex i = rinfo.Start.ParagraphIndex; i <= endParagraphIndex; i++)
				TryToJoinRuns(Paragraphs[i], rinfo);
		}
		void TryToJoinRuns(Paragraph paragraph, RunInfo rinfo) {
			Debug.Assert(DocumentModel.IsUpdateLockedOrOverlapped);
			RunIndex lastRunIndex = Algorithms.Min(rinfo.End.RunIndex, paragraph.LastRunIndex - 1);
			RunIndex firstRunIndex = Algorithms.Max(rinfo.Start.RunIndex, paragraph.FirstRunIndex);
			for (RunIndex i = lastRunIndex; i > firstRunIndex; i--) {
				if (Runs[i - 1].CanJoinWith(Runs[i]))
					JoinTextRuns(paragraph.Index, i - 1);
			}
		}
		internal void ChangeCharacterStyle(RunInfo rinfo, RunPropertyModifierBase modifier) {
			ParagraphIndex endParagraphIndex = rinfo.End.ParagraphIndex;
			for (ParagraphIndex i = rinfo.Start.ParagraphIndex; i <= endParagraphIndex; i++)
				ChangeCharacterStyle(rinfo, Paragraphs[i], modifier);
		}
		void ChangeCharacterStyle(RunInfo rinfo, Paragraph paragraph, RunPropertyModifierBase modifier) {
			RunIndex lastRunIndex = Algorithms.Min(rinfo.End.RunIndex, paragraph.LastRunIndex);
			RunIndex firstRunIndex = Algorithms.Max(rinfo.Start.RunIndex, paragraph.FirstRunIndex);
			for (RunIndex i = firstRunIndex; i <= lastRunIndex; i++)
				modifier.ModifyTextRun(Runs[i], i);
		}
		void ResetCharacterStyle(TextRunBase run, RunIndex runIndex) {
			RunCharacterStyleModifier modifier = new RunCharacterStyleModifier(-1);
			modifier.ModifyTextRun(run, runIndex);
		}
		internal void JoinTextRuns(ParagraphIndex paragraphIndex, RunIndex firstRunIndex) {
			documentModel.History.BeginTransaction();
			try {
				TextRunsJoinedHistoryItem item = new TextRunsJoinedHistoryItem(this);
				item.RunIndex = firstRunIndex;
				item.ParagraphIndex = paragraphIndex;
				documentModel.History.Add(item);
				item.Execute();
			}
			finally {
				documentModel.History.EndTransaction();
			}
		}
		public void SplitTextRun(ParagraphIndex paragraphIndex, RunIndex runIndex, int offset) {
			documentModel.History.BeginTransaction();
			try {
				TextRunSplitHistoryItem item = new TextRunSplitHistoryItem(this);
				item.RunIndex = runIndex;
				item.SplitOffset = offset;
				item.ParagraphIndex = paragraphIndex;
				documentModel.History.Add(item);
				item.Execute();
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
		internal void MultipleSplitTextRun(List<DocumentLogPosition> positions) {
			documentModel.History.BeginTransaction();
			try {
				MultipleTextRunSplitHistoryItem item = new MultipleTextRunSplitHistoryItem(this);
				item.Positions = positions;
				documentModel.History.Add(item);
				item.Execute();
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
		public void SetFont(DocumentLogPosition logPositionStart, int length, Font font) {
			RunFontModifier modifier = new RunFontModifier(font);
			ApplyCharacterFormatting(logPositionStart, length, modifier);
		}
		public void SetForeColor(DocumentLogPosition logPositionStart, int length, Color foreColor) {
			RunForeColorModifier modifier = new RunForeColorModifier(foreColor);
			ApplyCharacterFormatting(logPositionStart, length, modifier);
		}
		public void SetBackColor(DocumentLogPosition logPositionStart, int length, Color backColor) {
			RunBackColorModifier modifier = new RunBackColorModifier(backColor);
			ApplyCharacterFormatting(logPositionStart, length, modifier);
		}
		internal void ApplyCharacterFormatting(DocumentLogPosition logPositionStart, int length, RunPropertyModifierBase modifier) {
			if (logPositionStart < DocumentLogPosition.MinValue)
				Exceptions.ThrowArgumentException("logPositionStart", logPositionStart);
			if (length <= 0)
				Exceptions.ThrowArgumentException("length", length);
			ApplyCharacterStyleCore(logPositionStart, length, modifier);
		}
		internal void ApplyParagraphFormatting(DocumentLogPosition logPositionStart, int length, ParagraphPropertyModifierBase modifier) {
			if (logPositionStart < DocumentLogPosition.MinValue)
				Exceptions.ThrowArgumentException("logPositionStart", logPositionStart);
			if (length <= 0)
				Exceptions.ThrowArgumentException("length", length);
			ApplyParagraphFormattingCore(logPositionStart, length, modifier);
		}
		protected internal void ApplyParagraphFormattingCore(DocumentLogPosition logPositionStart, int length, ParagraphPropertyModifierBase modifier) {
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				RunInfo rinfo = FindRunInfo(logPositionStart, length);
				for (ParagraphIndex i = rinfo.Start.ParagraphIndex; i <= rinfo.End.ParagraphIndex; i++)
					modifier.ModifyParagraph(paragraphs[i], i);
			}
		}
		public void ApplyCharacterStyle(DocumentLogPosition logPositionStart, int length, int styleIndex, bool resetProperties) {
			if (logPositionStart < DocumentLogPosition.MinValue)
				Exceptions.ThrowArgumentException("logPositionStart", logPositionStart);
			if (length <= 0)
				Exceptions.ThrowArgumentException("length", length);
			RunCharacterStyleModifier modifier = new RunCharacterStyleModifier(styleIndex, resetProperties);
			ApplyCharacterStyleCore(logPositionStart, length, modifier);
			CheckIntegrity();
		}
		public void ApplyCharacterStyle(DocumentLogPosition logPositionStart, int length, int styleIndex) {
			ApplyCharacterStyle(logPositionStart, length, styleIndex, true);
		}
		public void ApplyParagraphStyle(DocumentLogPosition logPositionStart, int length, int styleIndex) {
			if (logPositionStart < DocumentLogPosition.MinValue)
				Exceptions.ThrowArgumentException("logPositionStart", logPositionStart);
			if (length <= 0)
				Exceptions.ThrowArgumentException("length", length);
			RunCharacterStyleModifier modifier = new RunCharacterStyleModifier(styleIndex);
			if (ShouldApplyStyleToParagraphs(logPositionStart, length))
				ApplyParagraphStyleCore(logPositionStart, length, modifier);
			else
				ApplyCharacterStyleCore(logPositionStart, length, modifier);
			CheckIntegrity();
		}
		public void ApplyTableStyle(DocumentLogPosition logPositionStart, int length, int styleIndex) {
			if (logPositionStart < DocumentLogPosition.MinValue)
				Exceptions.ThrowArgumentException("logPositionStart", logPositionStart);
			List<Table> tables = FindCurrentTables(logPositionStart, length);
			ApplyTableStyleCore(tables, styleIndex);
			CheckIntegrity();
		}
		public void ApplyTableCellStyle(DocumentLogPosition logPositionStart, int length, int styleIndex) {
			if (logPositionStart < DocumentLogPosition.MinValue)
				Exceptions.ThrowArgumentException("logPositionStart", logPositionStart);
			List<TableCell> tables = FindCurrentCells(logPositionStart, length);
			ApplyTableCellStyleCore(tables, styleIndex);
			CheckIntegrity();
		}
		private List<Table> FindCurrentTables(DocumentLogPosition logPositionStart, int length) {
			ParagraphIndex startParagraphIndex = FindParagraphIndex(logPositionStart);
			ParagraphIndex endParagraphIndex = FindParagraphIndex(logPositionStart + length);
			List<Table> tables = new List<Table>();
			for (ParagraphIndex i = startParagraphIndex; i <= endParagraphIndex; i++) {
				TableCell currentCell = Paragraphs[i].GetCell();
				if (currentCell != null) {
					Table currentTable = currentCell.Table;
					if (!tables.Contains(currentTable))
						tables.Add(currentTable);
				}
			}
			return tables;
		}
		private List<TableCell> FindCurrentCells(DocumentLogPosition logPositionStart, int length) {
			ParagraphIndex startParagraphIndex = FindParagraphIndex(logPositionStart);
			ParagraphIndex endParagraphIndex = FindParagraphIndex(logPositionStart + length);
			List<TableCell> cells = new List<TableCell>();
			for (ParagraphIndex i = startParagraphIndex; i <= endParagraphIndex; i++) {
				TableCell currentCell = Paragraphs[i].GetCell();
				if (currentCell != null) {
					if (!cells.Contains(currentCell))
						cells.Add(currentCell);
				}
			}
			return cells;
		}
		private void ResetTableAllUse(Table table) {
			DocumentModel.BeginUpdate();
			try {
				TablePropertiesOptions.Mask tableUseValue = table.TableProperties.UseValue;
				tableUseValue &= (~TablePropertiesOptions.Mask.UsePreferredWidth);
				tableUseValue &= (~TablePropertiesOptions.Mask.UseTableLook);
				table.TableProperties.ResetUse(tableUseValue);
				TableCellProcessorDelegate resetAllUse = delegate(TableCell cell) {
					TableCellPropertiesOptions.Mask cellUseValue = cell.Properties.UseValue;
					cellUseValue &= (~TableCellPropertiesOptions.Mask.UsePreferredWidth);
					cell.Properties.ResetUse(cellUseValue);
				};
				table.ForEachCell(resetAllUse);
				new TableConditionalFormattingController(table).ResetCachedProperties(0);
				RunIndex start = Paragraphs[table.FirstRow.FirstCell.StartParagraphIndex].FirstRunIndex;
				RunIndex end = Paragraphs[table.LastRow.LastCell.EndParagraphIndex].LastRunIndex;
				ApplyChangesCore(TableChangeActionCalculator.CalculateChangeActions(TableChangeType.TableStyle), start, end);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		internal void ApplyTableStyleCore(List<Table> tables, int styleIndex) {
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				for (int i = 0; i < tables.Count; i++) {
					ResetTableAllUse(tables[i]);
					tables[i].StyleIndex = styleIndex;
				}
			}
		}
		internal void ApplyTableCellStyleCore(List<TableCell> cells, int styleIndex) {
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				for (int i = 0; i < cells.Count; i++)
					cells[i].StyleIndex = styleIndex;
			}
		}
		internal bool ShouldApplyStyleToParagraphs(DocumentLogPosition logPositionStart, int length) {
			RunInfo rinfo = FindRunInfo(logPositionStart, length);
			TextRunBase startRun = Runs[rinfo.Start.RunIndex];
			TextRunBase endRun = Runs[rinfo.End.RunIndex];
			if (!Object.ReferenceEquals(startRun.Paragraph, endRun.Paragraph))
				return true;
			bool isStartRunIndexMatchStartOfParagraph = (rinfo.Start.RunIndex == new RunIndex(0) || Runs[rinfo.Start.RunIndex - 1] is ParagraphRun);
			if (isStartRunIndexMatchStartOfParagraph) {
				if (rinfo.Start.RunStartLogPosition == logPositionStart) {
					if (endRun is ParagraphRun)
						return true;
					bool isEndPositionMatchEndOfParagraph = (rinfo.End.RunEndLogPosition == rinfo.End.LogPosition && Runs[rinfo.End.RunIndex + 1] is ParagraphRun);
					if (isEndPositionMatchEndOfParagraph)
						return true;
				}
			}
			return false;
		}
		protected internal void ApplySectionFormatting(DocumentLogPosition logPositionStart, int length, SectionPropertyModifierBase modifier) {
			contentType.ApplySectionFormatting(logPositionStart, length, modifier);
		}
		internal RunIndex CalculateRunIndex(TextRunBase run) {
			return CalculateRunIndex(run, RunIndex.Zero);
		}
		internal RunIndex CalculateRunIndex(TextRunBase run, RunIndex defaultResultWhenNotFound) {
			Paragraph paragraph = run.Paragraph;
			RunIndex firstRunIndex = paragraph.FirstRunIndex;
			RunIndex lastRunIndex = paragraph.LastRunIndex;
			if (firstRunIndex == new RunIndex(-1))
				return defaultResultWhenNotFound;
			RunIndex maxRunIndex = new RunIndex(runs.Count - 1);
			if (firstRunIndex > maxRunIndex)
				return defaultResultWhenNotFound;
			lastRunIndex = Algorithms.Min(lastRunIndex, maxRunIndex);
			for (RunIndex i = firstRunIndex; i <= lastRunIndex; i++)
				if (Runs[i] == run)
					return i;
			return defaultResultWhenNotFound;
		}
		internal void ClearFontCacheIndices() {
			Runs.ForEach(ClearRunFontCacheIndex);
		}
		void ClearRunFontCacheIndex(TextRunBase run) {
			run.ResetFontCacheIndex();
		}
		protected internal virtual void ToggleFieldCodesFromCommandOrApi(Field field) {
			ToggleFieldCodes(field);
		}		
		protected internal virtual void ToggleFieldCodes(Field field) {
			DocumentModel.BeginUpdate();
			try {
				ToggleFieldCodesHistoryItem item = new ToggleFieldCodesHistoryItem(this, Fields.IndexOf(field));
				DocumentModel.History.Add(item);
				item.Redo();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void ToggleFieldLocked(Field field) {
			DocumentModel.BeginUpdate();
			try {
				ToggleFieldLockedHistoryItem item = new ToggleFieldLockedHistoryItem(this, Fields.IndexOf(field));
				DocumentModel.History.Add(item);
				item.Redo();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void ToggleAllFieldCodes(bool showCodes) {
			DocumentModel.BeginUpdate();
			try {
				int count = Fields.Count;
				for (int i = 0; i < count; i++) {
					Field field = Fields[i];
					if (field.IsCodeView != showCodes)
						ToggleFieldCodes(field);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public Field FindFieldByRunIndex(RunIndex runIndex) {
			int index = FindFieldIndexByRunIndex(runIndex);
			if (index >= 0)
				return Fields[index];
			return null;
		}
		internal int FindFieldIndexByRunIndex(RunIndex runIndex) {
			return FindFieldIndexByRunIndex(runIndex, delegate(Field field) {
				return true;
			});
		}
		protected internal int FindFieldIndexByRunIndexCore(RunIndex runIndex) {
			int count = Fields.Count;
			FieldRunIndexComparable comparator = new FieldRunIndexComparable(runIndex);
			int index = Algorithms.BinarySearch(Fields, comparator);
			if (index < 0) {
				index = ~index;
				if (index >= count)
					return ~index;
			}
			return index;
		}
		private int FindFieldIndexByRunIndex(RunIndex runIndex, Predicate<Field> predicate) {
			int index = FindFieldIndexByRunIndexCore(runIndex);
			if (index < 0)
				return index;
			Field field = Fields[index];
			do {
				if (runIndex >= field.FirstRunIndex && predicate(field)) {
					Debug.Assert(runIndex <= field.LastRunIndex);
					return field.Index;
				}
				field = field.Parent;
			}
			while (field != null);
			return ~index;
		}
		protected internal List<Field> GetHyperlinkFields(RunIndex start, RunIndex end) {
			List<Field> result = new List<Field>();
			int startIndex = FindFieldIndexByRunIndex(start, IsHyperlinkField);
			if (startIndex < 0)
				startIndex = ~startIndex;
			if (startIndex > Fields.Count)
				return result;
			int endIndex = FindFieldIndexByRunIndex(end, IsHyperlinkField);
			if (endIndex < 0)
				endIndex = ~endIndex - 1;
			if (endIndex < startIndex)
				return result;
			for (int i = startIndex; i <= endIndex; i++) {
				Field field = Fields[i];
				if (IsHyperlinkField(field))
					result.Add(field);
			}
			return result;
		}
		internal Field FindParentFieldByRunIndex(RunIndex runIndex) {
			for (int index = Fields.Count - 1; index >= 0; index--) {
				Field field = Fields[index];
				if (field.ContainsRun(runIndex))
					return field;
			}
			return null;
		}
		internal IList<Field> GetEntireFieldsFromInterval(RunIndex start, RunIndex end) {
			List<Field> result = new List<Field>();
			int count = Fields.Count;
			for (int index = 0; index < count; index++) {
				Field field = Fields[index];
				if (start <= field.FirstRunIndex) {
					if (end >= field.LastRunIndex)
						result.Add(field);
					else
						break;
				}
			}
			return result;
		}
		internal int GetInsertIndex(Field field) {
			int count = Fields.Count;
			for (int index = 0; index < count; index++) {
				if (field.FirstRunIndex > Fields[index].FirstRunIndex) {
					if (field.LastRunIndex < Fields[index].LastRunIndex)
						return index;
				}
				else
					return ~index;
			}
			return ~count;
		}
		internal int[] GetChildFieldIndexes(Field field) {
			List<int> result = new List<int>();
			int count = Fields.Count;
			for (int index = 0; index < count; index++) {
				if (field.FirstRunIndex < Fields[index].FirstRunIndex) {
					if (field.LastRunIndex > Fields[index].LastRunIndex)
						result.Add(index);
					else
						break;
				}
			}
			return result.ToArray();
		}
		public Comment FindCommentByDocumentLogPosition(DocumentLogPosition logPosition) {
			int index = FindCommentIndexByDocumentLogPosition(logPosition);
			if (index >= 0)
				return Comments[index];
			return null;
		}
		internal int FindCommentIndexByDocumentLogPosition(DocumentLogPosition logPosition) {
			return FindCommentIndexByDocumentLogPosition(logPosition, delegate(Comment comment) {
				return true;
			});
		}
		protected internal int FindCommentIndexByDocumentLogPositionCore(DocumentLogPosition logPosition){
			int count = Comments.Count;
			CommentDocumentLogPositionComparable comparator = new CommentDocumentLogPositionComparable(logPosition);
			int index = Algorithms.BinarySearch(Comments.InnerList, comparator);
			if (index < 0) {
				index = ~index;
				if (index >= count)
					return ~index;
			}
			return index;
		}
		private int FindCommentIndexByDocumentLogPosition(DocumentLogPosition logPosition, Predicate<Comment> predicate) {
			int index = FindCommentIndexByDocumentLogPositionCore(logPosition);
			if (index < 0)
				index = ~index;
			if (index >= Comments.Count)
				return ~index;
			Comment comment = Comments[index];
			DocumentLogPosition start = comment.Start;
			DocumentLogPosition end = comment.End;
			if (logPosition >= start && logPosition <= end) {
				if (index < Comments.Count - 2) {
					do
						index++;
					while (logPosition >= Comments[index].Start && logPosition <= Comments[index].End && 
						Comments[index - 1].Start <= Comments[index].Start && Comments[index - 1].End >= Comments[index].End && index <= Comments.Count - 2);
					index--;
					return index;
				}
				return index;
			}
			if (logPosition < start) {
				for (int i = index + 1; i < Comments.Count; i++) {
					if (logPosition > Comments[i].Start && logPosition < Comments[i - 1].Start)
						return i;
				}
				return ~index;
			}
			return ~index;
		}
		protected internal virtual bool ShouldForceUpdateIntervals() {
			return this.shouldForceUpdateIntervals;
		}
		protected internal virtual void UpdateIntervals() {
			Bookmarks.UpdateIntervals();
			RangePermissions.UpdateIntervals();
			Comments.UpdateIntervals();
			this.shouldForceUpdateIntervals = false;
		}
		protected internal virtual void OnBeginSetContent() {
			this.shouldForceUpdateIntervals = true;
		}
		protected internal virtual void OnEndSetContent() {
			SplitTextRunsByCharset();
			UpdateIntervals();
			SpellCheckerManager.Initialize();
		}
		protected internal HyperlinkInfo GetHyperlinkInfo(Field field) {
			if (!HyperlinkInfos.IsHyperlink(field.Index))
				return HyperlinkInfo.Empty;
			return HyperlinkInfos[field.Index];
		}
		protected internal Field GetHyperlinkField(RunIndex runIndex) {
			Field field = FindFieldByRunIndex(runIndex);
			if (field != null && hyperlinkInfos.IsHyperlink(field.Index))
				return field;
			return null;
		}
		protected internal bool HasInlinePicture(RunInfo runInfo) {
			RunIndex startRunIndex = runInfo.NormalizedStart.RunIndex;
			RunIndex endRunIndex = runInfo.NormalizedEnd.RunIndex;
			for (RunIndex i = startRunIndex; i < endRunIndex; i++) {
				if (Runs[i] is InlinePictureRun)
					return true;
			}
			return false;
		}
		protected internal virtual void ConvertParagraphsToTable(ParagraphIndex firstParagraphIndex, int rowCount, int cellCount) {
			DocumentModel.BeginUpdate();
			try {
				CreateTableHistoryItem item = new CreateTableHistoryItem(this, firstParagraphIndex, rowCount, cellCount);
				DocumentModel.History.Add(item);
				item.Execute();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void PerformTextRunSplit(SortedRunIndexCollection runIndices) {
			if (!DocumentModel.BehaviorOptions.UseFontSubstitution)
				return;
			int count = runIndices.Count;
			for (int i = count - 1; i >= 0; i--)
				SplitTextRunByCharset(runIndices[i]);
		}
		#region Text
		public void InsertText(DocumentLogPosition logPosition, string text) {
			InsertText(logPosition, text, false);
		}
		public void InsertText(DocumentLogPosition logPosition, string text, bool forceVisible) {
			PieceTableInsertTextAtLogPositionCommand command = new PieceTableInsertTextAtLogPositionCommand(this, logPosition, text, forceVisible);
			command.Execute();
		}
		internal void InsertText(InputPosition position, string text, bool forceVisible) {
			Debug.Assert(Object.ReferenceEquals(position.PieceTable, this));
			PieceTableInsertTextAtInputPositionCommand command = new PieceTableInsertTextAtInputPositionCommand(this, position, text, forceVisible);
			command.Execute();
		}
		internal void InsertPlainText(DocumentLogPosition logPosition, string text) {
			InsertPlainText(logPosition, text, false);
		}
		internal void InsertPlainText(DocumentLogPosition logPosition, string text, bool forceVisible) {
			PieceTableInsertPlainTextAtLogPositionCommand command = new PieceTableInsertPlainTextAtLogPositionCommand(this, logPosition, text, forceVisible);
			command.Execute();
		}
		internal void InsertPlainText(InputPosition position, string text) {
			PieceTableInsertPlainTextAtInputPositionCommand command = new PieceTableInsertPlainTextAtInputPositionCommand(this, position, text, false);
			command.Execute();
		}
		#endregion
		#region Paragraphs
		public Paragraph InsertParagraph(DocumentLogPosition logPosition) {
			return InsertParagraph(logPosition, false);
		}
		public Paragraph InsertParagraph(DocumentLogPosition logPosition, bool forceVisible) {
			PieceTableInsertParagraphAtLogPositionCommand command = new PieceTableInsertParagraphAtLogPositionCommand(this, logPosition, forceVisible);
			command.Execute();
			return Paragraphs[command.ParagraphIndex];
		}
		public Paragraph InsertParagraph(InputPosition inputPosition, DocumentLogPosition logPosition, bool forceVisible) {
			PieceTableInsertParagraphAtInputPositionCommand command = new PieceTableInsertParagraphAtInputPositionCommand(this, logPosition, forceVisible, inputPosition);
			command.Execute();
			return Paragraphs[command.ParagraphIndex];
		}
		internal void ApplyNumberingToInsertedParagraph(ParagraphIndex paragraphIndex) {
			Paragraph originalParagraph = Paragraphs[paragraphIndex + 1];
			if (originalParagraph.IsInList()) {
				Paragraph newParagraph = Paragraphs[paragraphIndex];
				AddParagraphToList(paragraphIndex, originalParagraph.GetOwnNumberingListIndex(), originalParagraph.GetOwnListLevelIndex());
				if (!originalParagraph.IsEmpty || !newParagraph.IsEmpty) {
					CopyCharacterPropertiesToParagraphMark(originalParagraph, newParagraph);
				}
				else {
					PieceTable pieceTable = originalParagraph.PieceTable;
					if (originalParagraph.IsInNonStyleList()) {
						int normalStyleIndex = pieceTable.DocumentModel.ParagraphStyles.DefaultItemIndex;
						DeleteNumerationFromParagraphAndChangeParagraphStyle(originalParagraph, normalStyleIndex);
						DeleteNumerationFromParagraphAndChangeParagraphStyle(newParagraph, normalStyleIndex);
					}
					else {
						pieceTable.RemoveParagraphFromList(originalParagraph.Index);
						pieceTable.RemoveParagraphFromList(newParagraph.Index);
					}
				}
			}
			else if (originalParagraph.GetOwnNumberingListIndex() == NumberingListIndex.NoNumberingList) {
				AddParagraphToList(paragraphIndex, originalParagraph.GetOwnNumberingListIndex(), originalParagraph.GetOwnListLevelIndex());
			}
		}
		void DeleteNumerationFromParagraphAndChangeParagraphStyle(Paragraph paragraph, int paragraphStyleIndex) {
			paragraph.ResetRunsCharacterFormatting();
			paragraph.ParagraphStyleIndex = paragraphStyleIndex;
			if (paragraph.IsInList())
				paragraph.PieceTable.RemoveNumberingFromParagraph(paragraph);
			if (paragraph.GetOwnNumberingListIndex() == NumberingListIndex.NoNumberingList) {
				paragraph.ResetNumberingListIndex(NumberingListIndex.ListIndexNotSetted);
				paragraph.ParagraphProperties.ResetUse(ParagraphFormattingOptions.Mask.UseFirstLineIndent | ParagraphFormattingOptions.Mask.UseLeftIndent);
			}
		}
		protected virtual void CopyCharacterPropertiesToParagraphMark(Paragraph sourceParagraph, Paragraph targetParagraph) {
			TextRunCollection runs = sourceParagraph.PieceTable.Runs;
			TextRunBase sourceRun = runs[sourceParagraph.LastRunIndex];
			TextRunBase targetRun = runs[targetParagraph.LastRunIndex];
			targetRun.CharacterStyleIndex = sourceRun.CharacterStyleIndex;
			targetRun.CharacterProperties.CopyFrom(sourceRun.CharacterProperties);
		}
		#endregion
		#region InlinePictures
		public InlinePictureRun InsertInlinePicture(DocumentLogPosition logPosition, OfficeImage image, int scaleX, int scaleY) {
			return InsertInlinePicture(logPosition, image, scaleX, scaleY, false);
		}
		public InlinePictureRun InsertInlinePicture(DocumentLogPosition logPosition, OfficeImage image, int scaleX, int scaleY, bool forceVisible) {
			PieceTableInsertInlinePictureAtLogPositionCommand command = new PieceTableInsertInlinePictureAtLogPositionCommand(this, logPosition, image, scaleX, scaleY, forceVisible);
			command.Execute();
			return (InlinePictureRun)command.Result;
		}
		public InlinePictureRun InsertInlinePicture(DocumentLogPosition logPosition, OfficeImage image) {
			return InsertInlinePicture(logPosition, image, 100, 100, false);
		}
		#endregion
		#region InlineCustomObjects
		public void InsertInlineCustomObject(DocumentLogPosition logPosition, IInlineCustomObject customObject, int scaleX, int scaleY, bool forceVisible) {
			PieceTableInsertInlineCustomObjectAtLogPositionCommand command = new PieceTableInsertInlineCustomObjectAtLogPositionCommand(this, logPosition, customObject, scaleX, scaleY, forceVisible);
			command.Execute();
		}
		public void InsertInlineCustomObject(DocumentLogPosition logPosition, IInlineCustomObject customObject) {
			InsertInlineCustomObject(logPosition, customObject, 100, 100, false);
		}
		#endregion
		#region InsertCustomRun
		public void InsertCustomRun(DocumentLogPosition logPosition, ICustomRunObject customRunObject, bool forceVisible) {
			PieceTableInsertCustomRunAtLogPositionCommand command = new PieceTableInsertCustomRunAtLogPositionCommand(this, logPosition, customRunObject, forceVisible);
			command.Execute();
		}
		public void InsertCustomRunCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, ICustomRunObject customRunObject, bool forceVisible) {
			textBuffer.Append(Characters.ObjectMark);
			CustomRunInserter inserter = new CustomRunInserter(this, customRunObject);
			InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
		}
		#endregion
		#region FloatingObjectAnchors
		public void InsertFloatingObjectAnchor(DocumentLogPosition logPosition) {
			InsertFloatingObjectAnchor(logPosition, false);
		}
		public void InsertFloatingObjectAnchor(DocumentLogPosition logPosition, bool forceVisible) {
			if (Runs[FindRunInfo(logPosition, 1).Start.RunIndex] is FieldResultEndRun) {
				logPosition++;
			}
			if (ContentType.IsTextBox)
				Exceptions.ThrowInternalException();
			PieceTableInsertFloatingObjectAnchorAtLogPositionCommand command = new PieceTableInsertFloatingObjectAnchorAtLogPositionCommand(this, logPosition, forceVisible);
			command.Execute();
		}
		#endregion
		#region Tables
		internal Table CreateTableCore(TableCell sourceCell) {
			PieceTableCreateEmptyTableCommand command = new PieceTableCreateEmptyTableCommand(this, sourceCell);
			command.Execute();
			return command.NewTable;
		}
		internal TableRow CreateTableRowCore(Table table) {
			return CreateTableRowCore(table, table.Rows.Count);
		}
		internal TableRow CreateTableRowCore(Table table, int rowIndex) {
			PieceTableCreateRowEmptyCommand command = new PieceTableCreateRowEmptyCommand(this, table, rowIndex);
			command.Execute();
			return command.InsertedRow;
		}
		internal TableCell CreateTableCellCore(TableRow row, ParagraphIndex start, ParagraphIndex end) {
			return CreateTableCellCore(row, row.Cells.Count, start, end);
		}
		internal TableCell CreateTableCellCore(TableRow row, int insertedIndex, ParagraphIndex start, ParagraphIndex end) {
			PieceTableCreateCellEmptyCommand command = new PieceTableCreateCellEmptyCommand(this, row, insertedIndex, start, end);
			command.Execute();
			return command.InsertedCell;
		}
		public void DeleteTableCore(Table deletedTable) {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				DeleteTableHistoryItem item = new DeleteTableHistoryItem(this, deletedTable);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		internal void DeleteEmptyTableRowCore(int tableIndex, int rowIndex) {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				Table table = this.Tables[tableIndex];
				TableRowCollection rows = table.Rows;
				TableRow row = rows[rowIndex];
				TableCellCollection cells = row.Cells;
				RunIndex runIndex = Paragraphs[cells.First.StartParagraphIndex].FirstRunIndex;
				int cellsCount = cells.Count;
				int rowCount = rows.Count;
				for (int i = cellsCount - 1; i >= 0; i--) {
					TableCell cell = cells[i];
					if (cell.VerticalMerging != MergingState.None) {
						int columnIndex = TableCellVerticalBorderCalculator.GetStartColumnIndex(cell, false);
						TableCell nextRowCell = rowIndex + 1 < rowCount ? TableCellVerticalBorderCalculator.GetCellByStartColumnIndex(rows[rowIndex + 1], columnIndex, false) : null;
						if (cell.VerticalMerging == MergingState.Restart) {
							if (nextRowCell != null) {
								Debug.Assert(nextRowCell.VerticalMerging == MergingState.Continue);
								TableCell afterNextRowCell = rowIndex + 2 < rowCount ? TableCellVerticalBorderCalculator.GetCellByStartColumnIndex(rows[rowIndex + 2], columnIndex, false) : null;
								if (afterNextRowCell != null && afterNextRowCell.VerticalMerging == MergingState.Continue)
									nextRowCell.VerticalMerging = MergingState.Restart;
								else
									nextRowCell.VerticalMerging = MergingState.None;
							}
						}
						else {
							Debug.Assert(rowIndex > 0);
							TableCell prevRowCell = TableCellVerticalBorderCalculator.GetCellByStartColumnIndex(rows[rowIndex - 1], columnIndex, false);
							if (prevRowCell != null && prevRowCell.VerticalMerging == MergingState.Restart) {
								if (nextRowCell == null || nextRowCell.VerticalMerging != MergingState.Continue)
									prevRowCell.VerticalMerging = MergingState.None;
							}
						}
					}
					DeleteTableCellWithNestedTables(tableIndex, rowIndex, i);
				}
				DeleteTableRowHistoryItem item = new DeleteTableRowHistoryItem(this, tableIndex, rowIndex);
				DocumentModel.History.Add(item);
				item.RunIndex = runIndex;
				item.Execute();
			}
		}
		internal void DeleteEmptyTableCellCore(int tableIndex, int rowIndex, int cellIndex) {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				DeleteEmptyTableCellHistoryItem item = new DeleteEmptyTableCellHistoryItem(this, tableIndex, rowIndex, cellIndex);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		internal void ConvertParagraphsIntoTableRow(TableRow row, ParagraphIndex index, int paragraphCount) {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				ConvertParagraphsIntoTableRowHistoryItem item = new ConvertParagraphsIntoTableRowHistoryItem(this, row, index, paragraphCount);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		internal void ChangeCellEndParagraphIndex(TableCell tableCell, ParagraphIndex index) {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				ChangeCellEndParagraphIndexHistoryItem item = new ChangeCellEndParagraphIndexHistoryItem(this, tableCell, index);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		internal void ChangeCellStartParagraphIndex(TableCell tableCell, ParagraphIndex index) {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				ChangeCellStartParagraphIndexHistoryItem item = new ChangeCellStartParagraphIndexHistoryItem(this, tableCell, index);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		#endregion
		#region SeparatorRuns
		public void InsertSeparator(DocumentLogPosition logPosition) {
			InsertSeparator(logPosition, false);
		}
		public void InsertSeparator(DocumentLogPosition logPosition, bool forceVisible) {
			PieceTableInsertSeparatorAtLogPositionCommand command = new PieceTableInsertSeparatorAtLogPositionCommand(this, logPosition, forceVisible);
			command.Execute();
		}
		#endregion
		#region Fields
		public Field CreateField(DocumentLogPosition logPosition, int length) {
			return CreateField(logPosition, length, false);
		}
		public Field CreateField(DocumentLogPosition logPosition, int length, bool forceVisible) {
			PieceTableCreateFieldCommand command = new PieceTableCreateFieldCommand(this, logPosition, length, forceVisible);
			command.Execute();
			return command.InsertedField;
		}
		public Field CreateField(DocumentLogPosition startCode, DocumentLogPosition endCode, int resultLength, bool forceVisible) {
			PieceTableCreateFieldWithResultCommand command = new PieceTableCreateFieldWithResultCommand(this, startCode, endCode, resultLength, forceVisible);
			command.Execute();
			return command.InsertedField;
		}
		protected internal virtual void ChangeFieldCode(Field field, string code) {
			bool isCodeView = field.IsCodeView;
			field.IsCodeView = true;
			try {
				DocumentModelPosition startCode = DocumentModelPosition.FromRunEnd(this, field.Code.Start);
				DocumentModelPosition endCode = DocumentModelPosition.FromRunStart(this, field.Code.End);
				int length = endCode.LogPosition - startCode.LogPosition;
				ReplaceText(startCode.LogPosition, length, code);
			}
			finally {
				field.IsCodeView = isCodeView;
			}
		}
		protected internal virtual void ChangeFieldResult(Field field, string result) {
			bool isCodeView = field.IsCodeView;
			field.IsCodeView = false;
			try {
				DocumentLogPosition start = DocumentModelPosition.FromRunStart(this, field.Result.Start).LogPosition;
				DocumentLogPosition end = DocumentModelPosition.FromRunStart(this, field.Result.End).LogPosition;
				ReplaceText(start, end - start, result);
			}
			finally {
				field.IsCodeView = isCodeView;
			}
		}
		protected internal virtual void DeleteFieldWithoutResult(Field field) {
			PieceTableDeleteFieldWithoutResultCommand command = new PieceTableDeleteFieldWithoutResultCommand(this, field);
			command.Execute();
		}
		protected internal List<Field> GetFieldsInsideInterval(RunIndex firstRunIndex, RunIndex lastRunIndex) {
			List<Field> result = new List<Field>();
			int index = FindFieldIndexByRunIndexCore(firstRunIndex);
			if (index < 0)
				return result;
			int count = fields.Count;
			while (index < count) {
				Field field = fields[index];
				if (field.LastRunIndex > lastRunIndex)
					break;
				if (field.FirstRunIndex >= firstRunIndex)
					result.Add(field);
				index++;
			}
			return result;
		}
		protected internal virtual void UpdateTableOfContents(UpdateFieldOperationType operationType) {
		}
		protected internal virtual CalculateFieldResult CalculateFieldResult(Field field, MailMergeDataMode mailMergeDataMode, UpdateFieldOperationType updateType) {
			IFieldCalculatorService fieldCalculatorService = DocumentModel.GetService<IFieldCalculatorService>();
			if (fieldCalculatorService != null) {
				return fieldCalculatorService.CalculateField(this, field, mailMergeDataMode, updateType);
			}
			else {
				return new CalculateFieldResult(CalculatedFieldValue.Null, UpdateFieldOperationType.Normal);
			}
		}
		#endregion
		#region Bookmarks
		protected internal Bookmark CreateBookmarkCore(DocumentLogPosition position, int length, string name) {
			return CreateBookmarkCore(position, length, name, false);
		}
		protected internal Bookmark CreateBookmarkCore(DocumentLogPosition position, int length, string name, bool forceUpdateInterval) {
			InsertBookmarkHistoryItem item = new InsertBookmarkHistoryItem(this);
			item.Position = position;
			item.Length = length;
			item.BookmarkName = ValidateBookmarkName(name);
			item.ForceUpdateInterval = forceUpdateInterval;
			DocumentModel.History.Add(item);
			item.Execute();
			return Bookmarks[item.IndexToInsert];
		}
		string ValidateBookmarkName(string name) {
			if (!DocumentModel.BookmarkOptions.AllowNameResolution)
				return name;
			HashSet<string> bookmarkNames = new HashSet<string>();
			int count = Bookmarks.Count;
			for (int i = 0; i < count; i++)
				bookmarkNames.Add(Bookmarks[i].Name);
			if (!bookmarkNames.Contains(name))
				return name;
			int index = 1;
			string prefix = name;
			do {
				name = string.Format("{0}_{1}", prefix, index);
				index++;
			} while (bookmarkNames.Contains(name));
			return name;
		}
		public void CreateBookmark(DocumentLogPosition position, int length, string name) {
			DocumentModel.BeginUpdate();
			try {
				Bookmark bookmark = CreateBookmarkCore(position, length, name);
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout;
				RunInfo runInfo = bookmark.Interval;
				ApplyChangesCore(changeActions, runInfo.NormalizedStart.RunIndex, runInfo.NormalizedEnd.RunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void DeleteBookmark(int bookmarkIndex) {
			DocumentModel.BeginUpdate();
			try {
				Bookmark bookmark = Bookmarks[bookmarkIndex];
				DeleteBookmarkCore(bookmarkIndex);
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout;
				RunInfo runInfo = bookmark.Interval;
				ApplyChangesCore(changeActions, runInfo.NormalizedStart.RunIndex, runInfo.NormalizedEnd.RunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void DeleteBookmarkCore(int bookmarkIndex) {
			DeleteBookmarkHistoryItem item = new DeleteBookmarkHistoryItem(this);
			item.DeletedBookmarkIndex = bookmarkIndex;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected internal List<Bookmark> GetEntireBookmarks(DocumentLogPosition start, int length) {
			return GetEntireBookmarksCore(Bookmarks, start, length);
		}
		protected internal List<T> GetEntireBookmarksCore<T>(BookmarkBaseCollection<T> bookmarks, DocumentLogPosition start, int length) where T : BookmarkBase {
			DocumentLogPosition end = start + length;
			List<T> result = new List<T>();
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++) {
				if (bookmarks[i].Start >= start && bookmarks[i].End <= end && bookmarks[i].Start < end)
					result.Add(bookmarks[i]);
			}
			return result;
		}
		#endregion
		#region Comments
		protected internal Comment CreateCommentCore(DocumentLogPosition position, int length, string name, string author, DateTime date, Comment parentComment, CommentContentType content) {
			InsertCommentHistoryItem item = new InsertCommentHistoryItem(this);
			item.Position = position;
			item.Length = length;
			item.Name = name;
			item.Author = author;
			item.Date = date;
			item.ParentComment = parentComment;
			item.Content = content;
			DocumentModel.History.Add(item);
			item.Execute();
			return Comments[item.IndexToInsert];
		}
		public Comment CreateComment(DocumentLogPosition position, int length,  string author, DateTime date, CommentContentType content) {
			return CreateComment(position, length,  author, date, null, content);
		}
		public Comment CreateComment(DocumentLogPosition position, int length, string author, DateTime date, Comment parentComment, CommentContentType content) {
			Comment result;
			DocumentModel.BeginUpdate();
			try {
				result = CreateCommentCore(position, length, CalculateCommentName(author), author,  date, parentComment, content);
				DocumentModel.CommentOptions.BeginUpdate();
				DocumentModel.CommentOptions.Visibility = RichEditCommentVisibility.Visible;
				DocumentModel.CaculateVisibleAuthors();
				DocumentModel.CommentOptions.EndUpdate();
				RunInfo runInfo = result.Interval;
				ApplyChanges(DocumentModelChangeType.CommentOptionsVisibilityChanged, runInfo.NormalizedStart.RunIndex, runInfo.NormalizedEnd.RunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			return result;
		}
		internal static string CalculateCommentName(string author) {
			if (String.IsNullOrEmpty(author))
				return String.Empty;
			int length = author.Length;
			char[] charsRead = new char[length];
			using (StringReader reader = new StringReader(author)) {
				reader.Read(charsRead, 0, length);
			}
			StringBuilder reformattedText = new StringBuilder();
			using (StringWriter writer = new StringWriter(reformattedText)) {
				foreach (char c in charsRead) {
					if (char.IsLetter(c) && char.IsUpper(c)) {
						writer.Write(c);
					}
				}
			}
			return reformattedText.ToString();
		}
		protected internal void DeleteComment(int commentIndex) {
			DocumentModel.BeginUpdate();
			try {
				Comment comment = Comments[commentIndex];
				DeleteCommentCore(commentIndex);
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.DeleteComment | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout;
				RunInfo runInfo = comment.Interval;
				ApplyChangesCore(changeActions, runInfo.NormalizedStart.RunIndex, runInfo.NormalizedEnd.RunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void DeleteCommentCore(int commentIndex) {
			DeleteCommentHistoryItem item = new DeleteCommentHistoryItem(this);
			item.DeletedBookmarkIndex = commentIndex;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected internal List<Comment> GetEntireComments(DocumentLogPosition start, int length) {
			return GetEntireBookmarksCore(Comments, start, length);
		}
		#endregion
		#region CustomMarks
		protected internal CustomMark InsertCustomMark(int customMarkIndex, CustomMark customMark) {
			InsertCustomMarkHistoryItem item = new InsertCustomMarkHistoryItem(this);
			item.CustomMarkIndex = customMarkIndex;
			item.CustomMark = customMark;
			DocumentModel.History.Add(item);
			item.Execute();
			return CustomMarks[customMarkIndex];
		}
		public void DeleteCustomMark(int customMarkIndex) {
			DocumentModel.BeginUpdate();
			try {
				RunIndex runIndex = CustomMarks[customMarkIndex].Position.RunIndex;
				DeleteCustomMarkCore(customMarkIndex);
				ApplyChangesCore(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout, runIndex, runIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void DeleteCustomMarkCore(int customMarkIndex) {
			DeleteCustomMarkHistoryItem item = new DeleteCustomMarkHistoryItem(this);
			item.CustomMarkIndex = customMarkIndex;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected internal CustomMark CreateCustomMarkCore(DocumentLogPosition postion, object userData) {
			Paragraph paragraph = FindParagraph(postion);
			RunIndex runIndex;
			DocumentLogPosition runStart = FindRunStartLogPosition(paragraph, postion, out runIndex);
			DocumentModelPosition documentModelPosition = new DocumentModelPosition(this);
			documentModelPosition.LogPosition = postion;
			documentModelPosition.ParagraphIndex = paragraph.Index;
			documentModelPosition.RunIndex = runIndex;
			documentModelPosition.RunStartLogPosition = runStart;
			CustomMark customMark = new CustomMark(documentModelPosition, userData);
			InsertCustomMark(CustomMarks.Count, customMark);
			return customMark;
		}
		public void CreateCustomMark(DocumentLogPosition position, object userData) {
			DocumentModel.BeginUpdate();
			try {
				CustomMark customMark = CreateCustomMarkCore(position, userData);
				RunIndex runIndex = customMark.Position.RunIndex;
				ApplyChangesCore(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout, runIndex, runIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region DataContainers
		public void InsertDataContainerRun(DocumentLogPosition logPosition, IDataContainer dataContainer, bool forceVisible) {
			PieceTableInsertDataContainerAtLogPositionCommand command = new PieceTableInsertDataContainerAtLogPositionCommand(this, logPosition, dataContainer, forceVisible);
			command.Execute();
		}
		public void InsertDataContainerRunCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, IDataContainer dataContainer, bool forceVisible) {
			textBuffer.Append(Characters.ObjectMark);
			DataContainerRunInserter inserter = new DataContainerRunInserter(this, dataContainer);
			InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
		}
		#endregion
		#region Document Protection
		public void ApplyDocumentPermission(DocumentLogPosition start, DocumentLogPosition end, RangePermissionInfo info) {
			if (end - start < 0)
				return;
			DocumentModel.BeginUpdate();
			try {
				InsertRangePermissionHistoryItem item = new InsertRangePermissionHistoryItem(this);
				item.Position = start;
				item.Length = end - start;
				item.IndexToInsert = DocumentModel.Cache.RangePermissionInfoCache.GetItemIndex(info);
				DocumentModel.History.Add(item);
				item.Execute();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void RemoveDocumentPermission(DocumentLogPosition start, DocumentLogPosition end, RangePermissionInfo info) {
			if (end - start < 0)
				return;
			DocumentModel.BeginUpdate();
			try {
				RemoveRangePermissionHistoryItem item = new RemoveRangePermissionHistoryItem(this);
				item.Position = start;
				item.Length = end - start;
				item.IndexToInsert = DocumentModel.Cache.RangePermissionInfoCache.GetItemIndex(info);
				DocumentModel.History.Add(item);
				item.Execute();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void DeleteRangePermission(RangePermission permission) {
			DeleteRangePermissionHistoryItem item = new DeleteRangePermissionHistoryItem(this);
			item.DeletedRangePermission = permission;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected internal virtual RunInfo ApplyDocumentPermissionCore(DocumentLogPosition start, DocumentLogPosition end, int rangePermissionInfoIndex) {
			RangePermissionCollectionEx mergedRanges = ExtractMergedRanges(rangePermissionInfoIndex);
			RangePermission rangePermission = CreateRangePermission(start, end, rangePermissionInfoIndex);
			mergedRanges.Add(rangePermission);
			AppendMergedRanges(mergedRanges);
			if (mergedRanges.Count > 0)
				return mergedRanges[0].Interval;
			else
				return null;
		}
		protected internal virtual RunInfo RemoveDocumentPermissionCore(DocumentLogPosition start, DocumentLogPosition end, int rangePermissionInfoIndex) {
			RangePermissionCollectionEx mergedRanges = ExtractMergedRanges(rangePermissionInfoIndex);
			RunInfo result;
			if (mergedRanges.Count > 0)
				result = mergedRanges[0].Interval;
			else
				result = null;
			RangePermission rangePermission = CreateRangePermission(start, end, rangePermissionInfoIndex);
			mergedRanges.Remove(rangePermission);
			AppendMergedRanges(mergedRanges);
			return result;
		}
		RangePermission CreateRangePermission(DocumentLogPosition start, DocumentLogPosition end, int rangePermissionInfoIndex) {
			RangePermission rangePermission = new RangePermission(this, start, end);
			rangePermission.Properties.SetIndexInitial(rangePermissionInfoIndex);
			return rangePermission;
		}
		protected internal List<RangePermission> GetEntireRangePermissions(DocumentLogPosition start, int length) {
			return GetEntireBookmarksCore(RangePermissions, start, length);
		}
		protected internal RangePermissionCollectionEx ExtractMergedRanges(int rangePermissionInfoIndex) {
			RangePermissionCollectionEx mergedRanges = new RangePermissionCollectionEx(this);
			for (int i = RangePermissions.Count - 1; i >= 0; i--) {
				RangePermission rangePermission = RangePermissions[i];
				if (rangePermission.Properties.Index == rangePermissionInfoIndex) {
					mergedRanges.Add(rangePermission);
					RangePermissions.RemoveAt(i);
				}
			}
			return mergedRanges;
		}
		protected internal void AppendMergedRanges(RangePermissionCollectionEx mergedRanges) {
			int count = mergedRanges.Count;
			for (int i = 0; i < count; i++) {
				RangePermissions.Add(mergedRanges[i]);
			}
		}
		protected internal bool CanContainCompositeContent() {
			if (IsComment)
				return false;
			return true;
		}
		protected internal bool CanEditSelection() {
			Selection selection = DocumentModel.Selection;
			if (!Object.ReferenceEquals(selection.PieceTable, this))
				return false;
			if (!DocumentModel.IsDocumentProtectionEnabled)
				return true;
			return CanEditSelectionItems(selection.Items);
		}
		protected internal bool CanEditSelectionItems(List<SelectionItem> items) {
			if (!DocumentModel.IsDocumentProtectionEnabled)
				return true;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				SelectionItem selectionItem = items[i];
				if (!CanEditRange(selectionItem.NormalizedStart, selectionItem.NormalizedEnd))
					return false;
			}
			return true;
		}
		protected internal bool CanEditRange(DocumentLogPosition start, int length) {
			if (!DocumentModel.IsDocumentProtectionEnabled)
				return true;
			DocumentLogPosition end = start + length;
			return CanEditRange(start, end);
		}
		protected internal virtual bool CanEditRange(DocumentLogPosition start, DocumentLogPosition end) {
			if (!DocumentModel.IsDocumentProtectionEnabled)
				return true;
			RangePermissionCollection permissions = RangePermissions;
			int count = permissions.Count;
			for (int i = 0; i < count; i++) {
				RangePermission rangePermission = permissions[i];
				if (((start == end && start >= rangePermission.Start && start <= rangePermission.End) || (start != end && rangePermission.Contains(start, end))) && IsPermissionGranted(rangePermission))
					return true;
			}
			return false;
		}
		protected internal bool IsPermissionGranted(RangePermission rangePermission) {
			AuthenticationOptions options = DocumentModel.AuthenticationOptions;
			if ((!String.IsNullOrEmpty(options.UserName) && String.Compare(rangePermission.UserName, options.UserName, StringComparison.CurrentCultureIgnoreCase) == 0) ||
				(!String.IsNullOrEmpty(options.EMail) && String.Compare(rangePermission.UserName, options.EMail, StringComparison.CurrentCultureIgnoreCase) == 0))
				return true;
			if (String.Compare(rangePermission.Group, "Everyone", StringComparison.CurrentCultureIgnoreCase) == 0 ||
				(!String.IsNullOrEmpty(options.Group) && String.Compare(rangePermission.Group, options.Group, StringComparison.CurrentCultureIgnoreCase) == 0))
				return true;
			return false;
		}
		protected internal virtual RangePermissionCollection ObtainRangePermissionsMatchSelection() {
			RangePermissionCollection result = new RangePermissionCollection(this);
			List<SelectionItem> items = DocumentModel.Selection.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++)
				ObtainRangePermissionsMatchSelectionItem(result, items[i]);
			return result;
		}
		protected internal virtual void ObtainRangePermissionsMatchSelectionItem(RangePermissionCollection target, SelectionItem item) {
			int count = RangePermissions.Count;
			for (int i = 0; i < count; i++) {
				RangePermission permission = RangePermissions[i];
				if (permission.Start == item.NormalizedStart && permission.End == item.NormalizedEnd)
					target.Add(permission);
			}
		}
		protected internal virtual RangePermissionCollection ObtainRangePermissionsWithSelectionInside() {
			RangePermissionCollection result = new RangePermissionCollection(this);
			List<SelectionItem> items = DocumentModel.Selection.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++)
				ObtainRangePermissionsWithSelectionItemInside(result, items[i]);
			return result;
		}
		protected internal virtual void ObtainRangePermissionsWithSelectionItemInside(RangePermissionCollection target, SelectionItem item) {
			int count = RangePermissions.Count;
			for (int i = 0; i < count; i++) {
				RangePermission permission = RangePermissions[i];
				if (item.NormalizedStart >= permission.Start && item.NormalizedEnd <= permission.End)
					target.Add(permission);
			}
		}
		#endregion
		#region Hyperlinks
		public void CreateHyperlink(DocumentLogPosition position, int length, HyperlinkInfo info) {
			CreateHyperlink(position, length, info, false);
		}
		public void CreateHyperlink(DocumentLogPosition position, int length, HyperlinkInfo info, bool forceVisible) {
			DocumentModel.BeginUpdate();
			try {
				Field field = CreateHyperlinkField(position, length, info, forceVisible);
				ApplyHyperlinkStyle(field, true);
				ToggleFieldCodes(field);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void DeleteHyperlink(Field field) {
			if (!IsHyperlinkField(field))
				Exceptions.ThrowArgumentException("field", field);
			DocumentModel.BeginUpdate();
			try {
				TextRunBase sourceRun = Runs[field.Code.Start];
				ApplyEmptyStyle(sourceRun.GetMergedCharacterProperties(), GetFieldResultRunInfo(field));
				DeleteFieldWithoutResult(field);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void ApplyEmptyStyle(MergedCharacterProperties source, RunInfo runInfo) {
			int styleIndex = DocumentModel.CharacterStyles.GetStyleIndexByName(CharacterStyleCollection.HyperlinkStyleName);
			if (runInfo.Start.RunIndex < new RunIndex(0) || runInfo.End.RunIndex < runInfo.Start.RunIndex)
				return;
			ReplaceRunCharacterStylePropertiesModifier modifier = new ReplaceRunCharacterStylePropertiesModifier(styleIndex, source);
			using (HistoryTransaction transaction = new HistoryTransaction(documentModel.History)) {
				ChangeCharacterStyle(runInfo, modifier);
				TryToJoinRuns(runInfo);
			}
		}
		protected internal HyperlinkInfo InsertHyperlinkInfo(int fieldIndex, HyperlinkInfo info) {
			InsertHyperlinkInfoHistoryItem item = new InsertHyperlinkInfoHistoryItem(this);
			item.FieldIndex = fieldIndex;
			item.HyperlinkInfo = info;
			DocumentModel.History.Add(item);
			item.Redo();
			return HyperlinkInfos[fieldIndex];
		}
		protected internal void RemoveHyperlinkInfo(int fieldIndex) {
			DeleteHyperlinkInfoHistoryItem item = new DeleteHyperlinkInfoHistoryItem(this);
			item.FieldIndex = fieldIndex;
			DocumentModel.History.Add(item);
			item.Redo();
		}
		protected internal void ReplaceHyperlinkInfo(int fieldIndex, HyperlinkInfo newInfo) {
			HyperlinkInfo oldInfo = HyperlinkInfos[fieldIndex];
			ReplaceHyperlinkInfoHistoryItem item = new ReplaceHyperlinkInfoHistoryItem(this);
			item.OldInfo = oldInfo;
			item.NewInfo = newInfo;
			item.FieldIndex = fieldIndex;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected internal bool IsHyperlinkField(Field field) {
			return HyperlinkInfos.IsHyperlink(field.Index);
		}
		protected internal Field CreateHyperlinkField(DocumentLogPosition start, int length, HyperlinkInfo info) {
			return CreateHyperlinkField(start, length, info, false);
		}
		protected internal Field CreateHyperlinkField(DocumentLogPosition start, int length, HyperlinkInfo info, bool forceVisible) {
			HyperlinkInstructionBuilder builder = new HyperlinkInstructionBuilder(info);
			string instruction = builder.GetFieldInstruction();
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				InsertText(start, instruction, forceVisible);
				DocumentLogPosition endCodePosition = start + instruction.Length;
				Field field = CreateField(start, endCodePosition, length, forceVisible);
				InsertHyperlinkInfo(field.Index, info);
				return field;
			}
		}
		protected internal virtual void UpdateHyperlinkFieldCode(Field field) {
			HyperlinkInfo info = HyperlinkInfos[field.Index];
			HyperlinkInstructionBuilder builder = new HyperlinkInstructionBuilder(info);
			ChangeFieldCode(field, builder.GetFieldInstruction());
		}
		protected internal void ApplyHyperlinkStyle(RunInfo runInfo, bool applyDefaultHyperlinkStyle) {
			if (runInfo == null || runInfo.NormalizedStart > runInfo.NormalizedEnd)
				Exceptions.ThrowArgumentException("runInfo", runInfo);
			int styleIndex = DocumentModel.CharacterStyles.GetStyleIndexByName(CharacterStyleCollection.HyperlinkStyleName);
			RunCharacterStyleKeepOldStylePropertiesModifier modifier = new RunCharacterStyleKeepOldStylePropertiesModifier(styleIndex, applyDefaultHyperlinkStyle);
			ChangeCharacterStyle(runInfo, modifier);
		}
		protected internal void ApplyHyperlinkStyle(Field hyperlink, bool applyDefaultHyperlinkStyle) {
			if (hyperlink.Result.End == hyperlink.Result.Start)
				return;
			ApplyHyperlinkStyle(GetFieldResultRunInfo(hyperlink), applyDefaultHyperlinkStyle);
		}
		protected internal RunInfo GetFieldResultRunInfo(Field field) {
			RunInfo result = new RunInfo(this);
			DocumentModelPosition.SetRunStart(result.Start, field.Result.Start);
			DocumentModelPosition.SetRunStart(result.End, field.Result.End - 1);
			result.End.LogPosition = result.End.RunEndLogPosition;
			return result;
		}
		protected internal RunInfo GetFieldRunInfo(Field field) {
			RunInfo result = new RunInfo(this);
			DocumentModelPosition.SetRunStart(result.Start, field.Code.Start);
			DocumentModelPosition.SetRunStart(result.End, field.Result.End);
			return result;
		}
		#endregion
		#region Numbering Lists
		public void AddNumberingListToParagraph(Paragraph paragraph, NumberingListIndex numberingListIndex, int listLevelIndex) {
			Guard.ArgumentNotNull(paragraph, "paragraph");
			NumberingListCollection numberingLists = DocumentModel.NumberingLists;
			if (!IsValidNumberingListIndex(numberingListIndex) || numberingListIndex >= new NumberingListIndex(numberingLists.Count))
				Exceptions.ThrowArgumentException("numberingListIndex", numberingListIndex);
			if (!IsNumberingListLevelIndexValid(paragraph, numberingListIndex, listLevelIndex))
				Exceptions.ThrowArgumentException("listLevelIndex", listLevelIndex);
			if (paragraph.IsInNonStyleList())
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_InvalidParagraphContainNumbering);
			DocumentModel.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					AddParagraphToList(paragraph.Index, numberingListIndex, listLevelIndex);
				}
				DocumentModel.InvalidateDocumentLayoutFrom(paragraph.FirstRunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected virtual bool IsNumberingListLevelIndexValid(Paragraph paragraph, NumberingListIndex numberingListIndex, int listLevelIndex) {
			if (listLevelIndex < 0)
				return false;
			NumberingListIndex actualNumberingListIndex = numberingListIndex;
			if (numberingListIndex < NumberingListIndex.MinValue)
				actualNumberingListIndex = paragraph.ParagraphStyle.GetNumberingListIndex();
			if (actualNumberingListIndex < NumberingListIndex.MinValue)
				return numberingListIndex == NumberingListIndex.NoNumberingList;
			return listLevelIndex < DocumentModel.NumberingLists[actualNumberingListIndex].Levels.Count;
		}
		protected virtual bool IsValidNumberingListIndex(NumberingListIndex numberingListIndex) {
			return numberingListIndex >= NumberingListIndex.MinValue || numberingListIndex == NumberingListIndex.NoNumberingList;
		}
		protected internal virtual void DeleteNumerationFromParagraph(Paragraph paragraph) {
			Guard.ArgumentNotNull(paragraph, "paragraph");
			if (!paragraph.IsInList())
				Exceptions.ThrowArgumentException("paragraph", paragraph);
			WriteParagraphLeftIndent(paragraph);
			RemoveNumberingFromParagraph(paragraph);
		}
		protected internal virtual void WriteParagraphLeftIndent(Paragraph paragraph) {
			NumberingList numberingList = DocumentModel.NumberingLists[paragraph.GetNumberingListIndex()];
			IListLevel level = DocumentModel.NumberingLists[paragraph.GetNumberingListIndex()].Levels[paragraph.GetListLevelIndex()];
			if (NumberingListHelper.GetListType(numberingList) == NumberingType.MultiLevel) {
				if (paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Hanging)
					paragraph.LeftIndent = paragraph.LeftIndent - paragraph.FirstLineIndent;
				else
					paragraph.LeftIndent -= level.ListLevelProperties.OriginalLeftIndent;
			}
			else
				paragraph.LeftIndent -= level.ListLevelProperties.OriginalLeftIndent;
			paragraph.FirstLineIndentType = ParagraphFirstLineIndent.None;
			paragraph.FirstLineIndent = 0;
			if (paragraph.LeftIndent < 0)
				paragraph.LeftIndent = 0;
		}
		public void RemoveNumberingFromParagraph(Paragraph paragraph) {
			Guard.ArgumentNotNull(paragraph, "paragraph");
			if (!paragraph.IsInList())
				Exceptions.ThrowArgumentException("paragraph", paragraph);
			DocumentModel.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					if (paragraph.IsInNonStyleList())
						RemoveParagraphFromList(paragraph.Index);
					else {
						if (paragraph.GetOwnNumberingListIndex() == NumberingListIndex.ListIndexNotSetted) {
							int leftIndent = paragraph.LeftIndent;
							paragraph.ParagraphProperties.LeftIndent = leftIndent;
						}
						AddNumberingListToParagraph(paragraph, NumberingListIndex.NoNumberingList, 0);
					}
				}
				DocumentModel.InvalidateDocumentLayoutFrom(paragraph.FirstRunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region Tables
		public Table InsertTable(DocumentLogPosition logPosition, int rowCount, int cellCount) {
			return InsertTable(logPosition, rowCount, cellCount, TableAutoFitBehaviorType.AutoFitToContents, Int32.MinValue, Int32.MinValue, false);
		}
		public Table InsertTable(DocumentLogPosition logPosition, int rowCount, int cellCount, int fixedColumnWidths) {
			return InsertTable(logPosition, rowCount, cellCount, TableAutoFitBehaviorType.FixedColumnWidth, fixedColumnWidths, Int32.MinValue, false);
		}
		public Table InsertTable(DocumentLogPosition logPosition, int rowCount, int cellCount, TableAutoFitBehaviorType autoFitBehavior, int fixedColumnWidths) {
			return InsertTable(logPosition, rowCount, cellCount, autoFitBehavior, fixedColumnWidths, Int32.MinValue, false);
		}
		public Table InsertTable(DocumentLogPosition logPosition, int rowCount, int cellCount, TableAutoFitBehaviorType autoFitBehavior, int fixedColumnWidths, int outerColumnWidth, bool forceVisible) {
			return InsertTable(logPosition, rowCount, cellCount, autoFitBehavior, fixedColumnWidths, outerColumnWidth, forceVisible, false);
		}
		protected internal virtual Table InsertTable(DocumentLogPosition logPosition, int rowCount, int cellCount, TableAutoFitBehaviorType autoFitBehavior, int fixedColumnWidths, int outerColumnWidth, bool forceVisible, bool matchHorizontalTableIndentsToTextEdge) {
			Guard.ArgumentPositive(rowCount, "rowCount");
			Guard.ArgumentPositive(cellCount, "cellCount");
			ParagraphIndex targetParagraphIndex = ParagraphIndex.Zero;
			DocumentModel.BeginUpdate();
			try {
				Table table = null;
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					targetParagraphIndex = FindParagraphIndex(logPosition);
					Paragraph paragraph = Paragraphs[targetParagraphIndex];
					if (paragraph.LogPosition != logPosition) {
						InsertParagraph(logPosition, forceVisible);
						logPosition++;
						targetParagraphIndex++;
					}
					int newParagraphCount = rowCount * cellCount;
					InsertParagraphs(logPosition, newParagraphCount, forceVisible);
					ConvertParagraphsToTable(targetParagraphIndex, rowCount, cellCount);
					table = Paragraphs[targetParagraphIndex].GetCell().Table;
					table.InitializeColumnWidths(autoFitBehavior, fixedColumnWidths, outerColumnWidth, matchHorizontalTableIndentsToTextEdge);
					ValidateTableIndent(table);
				}
				DocumentModel.InvalidateDocumentLayout();
				table.TableLook = TableLookTypes.ApplyFirstRow | TableLookTypes.ApplyFirstColumn | TableLookTypes.DoNotApplyColumnBanding;
				return table;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void ValidateTableIndent(Table table) {
			ParagraphIndex tableStartParagraphIndex = table.StartParagraphIndex;
			Paragraph startParagraph = Paragraphs[tableStartParagraphIndex];
			int leftIndent = startParagraph.LeftIndent;
			int firstLineIndent = startParagraph.FirstLineIndent;
			ParagraphFirstLineIndent firstLineIndentType = startParagraph.FirstLineIndentType;
			if (leftIndent == 0 && firstLineIndent == 0 && firstLineIndentType == ParagraphFirstLineIndent.None)
				return;
			if (leftIndent != 0) {
				table.TableProperties.TableIndent.Type = WidthUnitType.ModelUnits;
				table.TableProperties.TableIndent.Value = leftIndent;
			}
			ParagraphIndex tableEndParagraphIndex = table.EndParagraphIndex;
			for (ParagraphIndex i = tableStartParagraphIndex; i <= tableEndParagraphIndex; i++) {
				Paragraph paragraph = Paragraphs[i];
				paragraph.LeftIndent = 0;
				paragraph.FirstLineIndent = 0;
				paragraph.FirstLineIndentType = ParagraphFirstLineIndent.None;
			}
		}
		void InsertParagraphs(DocumentLogPosition logPosition, int count, bool forceVisible) {
			if (count > 0)
				InsertParagraph(logPosition, forceVisible);
			for (int i = 0; i < count - 1; i++)
				InsertParagraph(logPosition + i, forceVisible);
		}
		public void InsertTableRowBelow(TableRow patternRow, bool forceVisible) {
			PieceTableInsertTableRowBelowCommand command = new PieceTableInsertTableRowBelowCommand(this, patternRow, forceVisible);
			command.Execute();
		}
		public void InsertTableRowAbove(TableRow patternRow, bool forceVisible) {
			PieceTableInsertTableRowAboveCommand command = new PieceTableInsertTableRowAboveCommand(this, patternRow, forceVisible);
			command.Execute();
		}
		public void MergeTableCellsHorizontally(TableCell cell, int count) {
			PieceTableMergeTableCellsHorizontallyCommand command = new PieceTableMergeTableCellsHorizontallyCommand(this, cell, count);
			command.Execute();
		}
		public void MergeTableCellsVertically(TableCell cell, int count) {
			PieceTableMergeTableCellsVerticallyCommand command = new PieceTableMergeTableCellsVerticallyCommand(this, cell, count);
			command.Execute();
		}
		public void SplitTable(int tableIndex, int rowIndex, bool forceVisible) {
			PieceTableSplitTableCommand command = new PieceTableSplitTableCommand(this, tableIndex, rowIndex, forceVisible);
			command.Execute();
		}
		public void InsertColumnToTheLeft(TableCell patternCell, bool forceVisible) {
			PieceTableInsertColumnToTheLeft command = new PieceTableInsertColumnToTheLeft(this, patternCell, forceVisible);
			command.Execute();
		}
		public void InsertColumnToTheRight(TableCell patternCell, bool forceVisible) {
			PieceTableInsertColumnToTheRight command = new PieceTableInsertColumnToTheRight(this, patternCell, forceVisible);
			command.Execute();
		}
		public void DeleteTableColumns(SelectedCellsCollection selectedCells, IInnerRichEditDocumentServerOwner server) {
			PieceTableDeleteTableColumnsCommand command = new PieceTableDeleteTableColumnsCommand(this, selectedCells, server);
			command.Execute();
		}
		public void InsertTableCellWithShiftToTheDown(TableCell patternCell, bool forceVisible, IInnerRichEditDocumentServerOwner server) {
			PieceTableInsertTableCellWithShiftToTheDownCommand command = new PieceTableInsertTableCellWithShiftToTheDownCommand(this, patternCell, forceVisible, server);
			command.Execute();
		}
		public void DeleteTableCellWithContent(TableCell deletedCell, IInnerRichEditDocumentServerOwner server) {
			DeleteTableCellWithContent(deletedCell, true, server, true);
		}
		public void DeleteTableCellWithContent(TableCell deletedCell, bool canNormalizeCellVerticalMerging, IInnerRichEditDocumentServerOwner server, bool useDeltaBetweenColumnsUpdate) {
			PieceTableDeleteTableCellWithContentCommand command = new PieceTableDeleteTableCellWithContentCommand(this, deletedCell, server);
			command.CanNormalizeCellVerticalMerging = canNormalizeCellVerticalMerging;
			command.UseDeltaBetweenColumnsUpdate = useDeltaBetweenColumnsUpdate;
			command.Execute();
		}
		public void DeleteTableCellWithContentKnownWidths(TableCell deletedCell, bool canNormalizeCellVerticalMerging, IInnerRichEditDocumentServerOwner server, TableWidthsContainer container, bool useDeltaBetweenColumnsUpdate) {
			PieceTableDeleteTableCellWithContentKnownWidthsCommand command = new PieceTableDeleteTableCellWithContentKnownWidthsCommand(this, deletedCell, server, container);
			command.CanNormalizeCellVerticalMerging = canNormalizeCellVerticalMerging;
			command.UseDeltaBetweenColumnsUpdate = useDeltaBetweenColumnsUpdate;
			command.Execute();
		}
		public void DeleteTableCellsWithShiftToTheUp(SelectedCellsCollection selectedCells) {
			PieceTableDeleteTableCellsWithShiftToTheUpCommand command = new PieceTableDeleteTableCellsWithShiftToTheUpCommand(this, selectedCells);
			command.Execute();
		}
		public void InsertTableCellToTheRight(TableCell patternCell, bool forceVisible, IInnerRichEditDocumentServerOwner server) {
			PieceTableInsertTableCellToTheRight command = new PieceTableInsertTableCellToTheRight(this, patternCell, forceVisible, server);
			command.Execute();
		}
		public void InsertTableCellToTheLeft(TableCell patternCell, bool forceVisible, IInnerRichEditDocumentServerOwner server) {
			PieceTableInsertTableCellToTheLeft command = new PieceTableInsertTableCellToTheLeft(this, patternCell, forceVisible, server);
			command.Execute();
		}
		public void DeleteTableCellWithNestedTables(int tableIndex, int rowIndex, int cellIndex) {
			PieceTableDeleteTableCellWithNestedTablesCommand command = new PieceTableDeleteTableCellWithNestedTablesCommand(this, tableIndex, rowIndex, cellIndex);
			command.Execute();
		}
		public void SplitTableCellsHorizontally(TableCell cell, int partsCount, IInnerRichEditDocumentServerOwner server) {
			SplitTableCellsHorizontally(cell, partsCount, false, server);
		}
		public void SplitTableCellsHorizontally(TableCell cell, int partsCount, bool forceVisible, IInnerRichEditDocumentServerOwner server) {
			PieceTableSplitTableCellsHorizontally command = new PieceTableSplitTableCellsHorizontally(this, cell, partsCount, forceVisible, server);
			command.Execute();
		}
		public void JoinTables(Table topTable, Table bottomTable) {
			if (Object.ReferenceEquals(topTable, bottomTable))
				return;
			PieceTableJoinTablesCommand command = new PieceTableJoinTablesCommand(this, topTable, bottomTable);
			command.Execute();
		}
		public void JoinTables(Table[] tables) {
			PieceTableJoinSeveralTablesCommand command = new PieceTableJoinSeveralTablesCommand(this, tables);
			command.Execute();
		}
		protected internal void MoveTableRowToOtherTable(Table targetTable, TableRow row) {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				MoveTableRowToOtherTableHistoryItem item = new MoveTableRowToOtherTableHistoryItem(this, targetTable, row);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		protected internal void DeleteTableFromTableCollection(Table deletedTable) {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				DeleteTableFromTableCollectionHistoryItem item = new DeleteTableFromTableCollectionHistoryItem(this, deletedTable);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		public void SplitTableCellsVertically(TableCell patternCell, int partsCount, int columnsCount, bool forceVisible) {
			PieceTableSplitTableCellsVertically command = new PieceTableSplitTableCellsVertically(this, patternCell, partsCount, columnsCount, forceVisible);
			command.Execute();
		}
		protected internal virtual void DeleteSelectedTables(RunInfo runInfo, bool documentLastParagraphSelected) {
			if (runInfo.Start == runInfo.End && !documentLastParagraphSelected)
				return;
			ParagraphIndex startParagraphIndex = runInfo.Start.ParagraphIndex;
			ParagraphIndex endParagraphIndex = runInfo.End.ParagraphIndex;
			TableCellsManager.TableCellNode root = TableCellsManager.GetCellSubTree(startParagraphIndex, endParagraphIndex, Int32.MinValue);
			DeleteTablesByNestedLevel(root, runInfo, documentLastParagraphSelected);
		}
		void DeleteTablesByNestedLevel(TableCellsManager.TableCellNode current, RunInfo runInfo, bool documentLastParagraphSelected) {
			if (current == null)
				return;
			for (int i = current.ChildNodes.Count - 1; i >= 0; i--) {
				TableCellsManager.TableCellNode currentNode = current.ChildNodes[i];
				if (currentNode.ChildNodes != null)
					DeleteTablesByNestedLevel(currentNode, runInfo, documentLastParagraphSelected);
				Table table = currentNode.Cell.Table;
				if (currentNode.Cell != null && Object.ReferenceEquals(table.FirstRow.FirstCell, currentNode.Cell) && ShouldDeleteTable(table, runInfo, documentLastParagraphSelected))
					DeleteTableCore(table);
			}
		}
		protected internal bool ShouldDeleteTable(Table table, RunInfo runInfo, bool documentLastParagraphSelected) {
			ParagraphIndex tableStartParagraphIndex = table.FirstRow.FirstCell.StartParagraphIndex;
			ParagraphIndex tableEndParagraphIndex = table.LastRow.LastCell.EndParagraphIndex;
			RunIndex tableStartRunIndex = Paragraphs[tableStartParagraphIndex].FirstRunIndex;
			RunIndex tableEndRunIndex = Paragraphs[tableEndParagraphIndex].LastRunIndex;
			RunIndex runStartIndex = runInfo.Start.RunIndex;
			RunIndex runEndIndex = runInfo.End.RunIndex;
			if (runStartIndex < tableStartRunIndex && (tableEndRunIndex < runEndIndex || documentLastParagraphSelected))
				return true;
			if (runStartIndex == tableStartRunIndex && (tableEndRunIndex < runEndIndex || documentLastParagraphSelected))
				return true;
			if (runStartIndex < tableStartRunIndex && tableEndRunIndex == runEndIndex)
				return true;
			return false;
		}
		#endregion
		protected internal virtual void DeleteTableWithContent(Table deletedTable) {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				Paragraph tableStartParagraph = Paragraphs[deletedTable.StartParagraphIndex];
				Paragraph tableEndParagraph = Paragraphs[deletedTable.EndParagraphIndex];
				DocumentLogPosition startLogPosition = tableStartParagraph.LogPosition;
				int length = tableEndParagraph.EndLogPosition - startLogPosition + 1;
				RunInfo runInfo = ObtainAffectedRunInfo(startLogPosition, length);
				DeleteSelectedTables(runInfo, true);
				DeleteContent(startLogPosition, length, startLogPosition + length >= DocumentEndLogPosition);
			}
		}
		protected internal virtual void DeleteTableRowWithContent(TableRow deletedRow) {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				DocumentLogPosition start = Paragraphs[deletedRow.Cells.First.StartParagraphIndex].LogPosition;
				Paragraph endParagraph = Paragraphs[deletedRow.Cells.Last.EndParagraphIndex];
				DocumentLogPosition end = endParagraph.EndLogPosition;
				int length = end - start + 1;
				DeleteEmptyTableRowCore(deletedRow.Table.Index, deletedRow.IndexInTable);
				DeleteContent(start, length, start + length >= DocumentEndLogPosition);
			}
		}
		public void DeleteBackContent(DocumentLogPosition logPosition, int length, bool documentLastParagraphSelected) {
			DeleteContent(logPosition, length, documentLastParagraphSelected, false, false, false, true);
		}
		public void DeleteContent(DocumentLogPosition logPosition, int length, bool documentLastParagraphSelected) {
			DeleteContent(logPosition, length, documentLastParagraphSelected, false);
		}
		public virtual void DeleteContent(DocumentLogPosition logPosition, int length, bool documentLastParagraphSelected, bool allowPartiallyDeletingField) {
			DeleteContent(logPosition, length, documentLastParagraphSelected, allowPartiallyDeletingField, false);
		}
		public virtual void DeleteContent(DocumentLogPosition logPosition, int length, bool documentLastParagraphSelected, bool allowPartiallyDeletingField, bool forceRemoveInnerFields) {
			DeleteContent(logPosition, length, documentLastParagraphSelected, allowPartiallyDeletingField, forceRemoveInnerFields, false);
		}
		public virtual void DeleteContent(DocumentLogPosition logPosition, int length, bool documentLastParagraphSelected, bool allowPartiallyDeletingField, bool forceRemoveInnerFields, bool leaveFieldIfResultIsRemoved) {
			DeleteContent(logPosition, length, documentLastParagraphSelected, allowPartiallyDeletingField, forceRemoveInnerFields, leaveFieldIfResultIsRemoved, false);
		}
		public virtual void DeleteContent(DocumentLogPosition logPosition, int length, bool documentLastParagraphSelected, bool allowPartiallyDeletingField, bool forceRemoveInnerFields, bool leaveFieldIfResultIsRemoved, bool backspacePressed) {
			PieceTableDeleteTextCommand command = CreatePieceTableDeleteTextCommand(logPosition, length);
			command.AllowPartiallyDeletingField = allowPartiallyDeletingField;
			command.DocumentLastParagraphSelected = documentLastParagraphSelected;
			command.ForceRemoveInnerFields = forceRemoveInnerFields;
			command.LeaveFieldIfResultIsRemoved = leaveFieldIfResultIsRemoved;
			command.BackspacePressed = backspacePressed;
			command.Execute();
			DocumentModel.CheckIntegrity();
		}
		protected virtual PieceTableDeleteTextCommand CreatePieceTableDeleteTextCommand(DocumentLogPosition logPosition, int length) {
			return new PieceTableDeleteTextCommand(this, logPosition, length);
		}
		protected internal SectionIndex LookupSectionIndexByParagraphIndex(ParagraphIndex paragraphIndex) {
			return contentType.LookupSectionIndexByParagraphIndex(paragraphIndex);
		}
		#region IDocumentModelStructureChangedListener Members
		void IDocumentModelStructureChangedListener.OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnParagraphInserted(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnParagraphRemoved(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnParagraphMerged(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnBeginMultipleRunSplit();
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnEndMultipleRunSplit();
		}
		void IDocumentModelStructureChangedListener.OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnRunSplit(paragraphIndex, runIndex, splitOffset);
		}
		void IDocumentModelStructureChangedListener.OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnRunJoined(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnRunMerged(paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnRunUnmerged(paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnFieldInserted(fieldIndex);
		}
		void IDocumentModelStructureChangedListener.OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			Debug.Assert(Object.ReferenceEquals(this, pieceTable));
			OnFieldRemoved(fieldIndex);
		}
		#endregion
		#region IDocumentModelStructureChangedListener overrides
		protected internal virtual void OnParagraphInserted(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged)
				OnParagraphInsertedCore(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(TableCellsManager, this, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(DocumentModel, this, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		protected virtual void OnParagraphInsertedCore(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(Bookmarks, this, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(RangePermissions, this, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(CustomMarks, this, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(SpellCheckerManager, this, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(Comments, this, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		protected internal virtual void OnParagraphRemoved(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged)
				OnParagraphRemovedCore(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(TableCellsManager, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(DocumentModel, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		protected internal virtual void OnCommentInserted(Comment comment, int commentIndex, bool onExecute) {
			if (!DocumentModel.DeferredChanges.IsSetContentMode) {
				DocumentModel.RaiseCommentInserted(new CommentEventArgs(this, comment, commentIndex, onExecute));
			}		 
		}
		protected internal virtual void OnCommentDeleted(Comment comment, int commentIndex, bool onExecute) {
			if (!DocumentModel.DeferredChanges.IsSetContentMode)
				DocumentModel.RaiseCommentDeleted(new CommentEventArgs(this, comment, commentIndex, onExecute));
		}
		protected internal virtual void OnCommentChangedViaAPI(Comment comment, int commentIndex) {
			if (!DocumentModel.DeferredChanges.IsSetContentMode)
				documentModel.RaiseCommentChangedViaAPI(new CommentEventArgs(this, comment, commentIndex, true ));
		}
		protected virtual void OnParagraphRemovedCore(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(Bookmarks, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(RangePermissions, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(CustomMarks, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(SpellCheckerManager, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(Comments, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		protected internal virtual void OnParagraphMerged(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged)
				OnParagraphMergedCore(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(TableCellsManager, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(DocumentModel, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		protected virtual void OnParagraphMergedCore(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(Bookmarks, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(RangePermissions, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(CustomMarks, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(SpellCheckerManager, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(Comments, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		protected internal virtual void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			Debug.Assert(length > 0);
			RecalcParagraphsPositions(paragraphIndex, length, 1);
			RecalcFieldsPositions(newRunIndex, 1); 
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged)
				OnRunInsertedCore(paragraphIndex, newRunIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunInserted(TableCellsManager, this, paragraphIndex, newRunIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunInserted(DocumentModel, this, paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		protected virtual void OnRunInsertedCore(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			DocumentModelStructureChangedNotifier.NotifyRunInserted(Bookmarks, this, paragraphIndex, newRunIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunInserted(RangePermissions, this, paragraphIndex, newRunIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunInserted(CustomMarks, this, paragraphIndex, newRunIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunInserted(SpellCheckerManager, this, paragraphIndex, newRunIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunInserted(Comments, this, paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		protected internal virtual void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			Debug.Assert(length > 0);
			RecalcParagraphsPositions(paragraphIndex, -length, -1);
			RecalcFieldsPositions(runIndex, -1); 
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged)
				OnRunRemovedCore(paragraphIndex, runIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(TableCellsManager, this, paragraphIndex, runIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(DocumentModel, this, paragraphIndex, runIndex, length, historyNotificationId);
		}
		protected virtual void OnRunRemovedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(Bookmarks, this, paragraphIndex, runIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(RangePermissions, this, paragraphIndex, runIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(CustomMarks, this, paragraphIndex, runIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(SpellCheckerManager, this, paragraphIndex, runIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(Comments, this, paragraphIndex, runIndex, length, historyNotificationId);
		}
		protected internal virtual void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			RecalcParagraphsPositions(paragraphIndex, deltaRunLength, 0);
			DocumentModelStructureChangedNotifier.NotifyRunMerged(DocumentModel, this, paragraphIndex, runIndex, deltaRunLength);
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged)
				OnRunMergedCore(paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunMerged(TableCellsManager, this, paragraphIndex, runIndex, deltaRunLength);
			Debug.Assert(Runs[runIndex].Paragraph.Index == paragraphIndex);
		}
		protected virtual void OnRunMergedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			DocumentModelStructureChangedNotifier.NotifyRunMerged(Bookmarks, this, paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunMerged(RangePermissions, this, paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunMerged(CustomMarks, this, paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunMerged(SpellCheckerManager, this, paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunMerged(Comments, this, paragraphIndex, runIndex, deltaRunLength);
		}
		protected internal virtual void OnRunUnmerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			RecalcParagraphsPositions(paragraphIndex, deltaRunLength, 0);
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(DocumentModel, this, paragraphIndex, runIndex, deltaRunLength);
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged)
				OnRunUnmergedCore(paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(TableCellsManager, this, paragraphIndex, runIndex, deltaRunLength);
			Debug.Assert(Runs[runIndex].Paragraph.Index == paragraphIndex);
		}
		protected virtual void OnRunUnmergedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(Bookmarks, this, paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(RangePermissions, this, paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(CustomMarks, this, paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(SpellCheckerManager, this, paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(Comments, this, paragraphIndex, runIndex, deltaRunLength);
		}
		int multipleRunSplitCount;
		ParagraphIndex lastShiftedParagraphIndex;
		int lastDeltaLength;
		int lastDeltaRunIndex;
		protected internal virtual void OnBeginMultipleRunSplit() {
			multipleRunSplitCount++;
			if (multipleRunSplitCount == 1) {
				lastShiftedParagraphIndex = new ParagraphIndex(-1);
				lastDeltaLength = 0;
				lastDeltaRunIndex = 0;
			}
			Paragraphs.OnBeginMultipleRunSplit();
			DocumentModelStructureChangedNotifier.NotifyBeginMultipleRunSplit(DocumentModel, this);
		}
		protected internal virtual void OnEndMultipleRunSplit() {
			multipleRunSplitCount--;
			if (multipleRunSplitCount == 0)
				RecalcParagraphsPositionsCore(lastShiftedParagraphIndex + 1, lastDeltaLength, lastDeltaRunIndex);
			Paragraphs.OnEndMultipleRunSplit();
			DocumentModelStructureChangedNotifier.NotifyEndMultipleRunSplit(DocumentModel, this);
		}
		protected internal virtual void OnRunSplit(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			Debug.Assert(splitOffset > 0);
			RecalcParagraphsPositions(paragraphIndex, 0, 1);
			RecalcFieldsPositions(runIndex, 1); 
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged)
				OnRunSplitCore(paragraphIndex, runIndex, splitOffset);
			DocumentModelStructureChangedNotifier.NotifyRunSplit(TableCellsManager, this, paragraphIndex, runIndex, splitOffset);
			DocumentModelStructureChangedNotifier.NotifyRunSplit(DocumentModel, this, paragraphIndex, runIndex, splitOffset);
		}
		protected virtual void OnRunSplitCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			DocumentModelStructureChangedNotifier.NotifyRunSplit(Bookmarks, this, paragraphIndex, runIndex, splitOffset);
			DocumentModelStructureChangedNotifier.NotifyRunSplit(RangePermissions, this, paragraphIndex, runIndex, splitOffset);
			DocumentModelStructureChangedNotifier.NotifyRunSplit(CustomMarks, this, paragraphIndex, runIndex, splitOffset);
			DocumentModelStructureChangedNotifier.NotifyRunSplit(SpellCheckerManager, this, paragraphIndex, runIndex, splitOffset);
			DocumentModelStructureChangedNotifier.NotifyRunSplit(Comments, this, paragraphIndex, runIndex, splitOffset);
		}
		protected internal virtual void OnRunJoined(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			Debug.Assert(splitOffset > 0);
			Debug.Assert(tailRunLength > 0);
			RecalcParagraphsPositions(paragraphIndex, 0, -1);
			RecalcFieldsPositions(joinedRunIndex, -1); 
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged)
				OnRunJoinedCore(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunJoined(TableCellsManager, this, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunJoined(DocumentModel, this, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		protected virtual void OnRunJoinedCore(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			DocumentModelStructureChangedNotifier.NotifyRunJoined(Bookmarks, this, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunJoined(RangePermissions, this, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunJoined(CustomMarks, this, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunJoined(SpellCheckerManager, this, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunJoined(Comments, this, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		protected internal virtual void OnFieldInserted(int fieldIndex) {
			RecalcFieldsIndices(fieldIndex + 1, 1);
			HyperlinkInfos.RecalcKeys(fieldIndex, 1);
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged) {
				DocumentModelStructureChangedNotifier.NotifyFieldInserted(CustomMarks, this, fieldIndex);
			}
			DocumentModelStructureChangedNotifier.NotifyFieldInserted(TableCellsManager, this, fieldIndex);
			DocumentModelStructureChangedNotifier.NotifyFieldInserted(DocumentModel, this, fieldIndex);
		}
		protected internal virtual void OnFieldRemoved(int fieldIndex) {
			RecalcFieldsIndices(fieldIndex, -1);
			HyperlinkInfos.RecalcKeys(fieldIndex, -1);
			if (!DocumentModel.DeferredChanges.IsSetContentMode || DocumentModel.ForceNotifyStructureChanged) {
				DocumentModelStructureChangedNotifier.NotifyFieldRemoved(CustomMarks, this, fieldIndex);
			}
			DocumentModelStructureChangedNotifier.NotifyFieldRemoved(TableCellsManager, this, fieldIndex);
			DocumentModelStructureChangedNotifier.NotifyFieldRemoved(DocumentModel, this, fieldIndex);
		}
		#endregion
		protected internal void ApplyChanges(DocumentModelChangeType changeType, RunIndex startRunIndex, RunIndex endRunIndex) {
			contentType.ApplyChanges(changeType, startRunIndex, endRunIndex);
		}
		protected internal void ApplyChangesCore(DocumentModelChangeActions actions, RunIndex startRunIndex, RunIndex endRunIndex) {
			contentType.ApplyChangesCore(actions, startRunIndex, endRunIndex);
		}
		protected internal event CustomCreateVisibleTextFilterEventHandler CustomCreateVisibleTextFilter;
		protected internal virtual IVisibleTextFilter CreateVisibleTextFilter(bool showHiddenText) {
			IVisibleTextFilter result = CreateVisibleTextFilterCore(showHiddenText);
			if (CustomCreateVisibleTextFilter != null) {
				CustomCreateVisibleTextFilterEventArgs e = new CustomCreateVisibleTextFilterEventArgs(result);
				CustomCreateVisibleTextFilter(this, e);
				result = e.TextFilter;
			}
			return result;
		}
		protected internal virtual IVisibleTextFilter CreateVisibleTextFilterCore(bool showHiddenText) {
			if (showHiddenText)
				return new EmptyTextFilter(this);
			else
				return new VisibleTextFilter(this);
		}
		protected internal virtual IVisibleTextFilter CreateNavigationVisibleTextFilter(bool showHiddenText) {
			if (showHiddenText)
				return new EmptyTextFilterSkipFloatingObjects(this);
			else
				return new VisibleTextFilterSkipFloatingObjects(this);
		}		
		protected internal void SetShowHiddenText(bool value) {
			this.visibleTextFilter = CreateVisibleTextFilter(value);
			this.navigationVisibleTextFilter = CreateNavigationVisibleTextFilter(value);
		}
		protected internal void InsertDocumentModelContent(DocumentModel documentModel, DocumentLogPosition pos) {
			InsertDocumentModelContent(documentModel, pos, false, false, false);
		}
		protected internal void InsertDocumentModelContent(DocumentModel documentModel, DocumentLogPosition pos, bool suppressParentFieldsUpdate, bool suppressFieldsUpdate, bool copyBetweenInternalModels) {
			PieceTableInsertContentConvertedToDocumentModelCommand command = new PieceTableInsertContentConvertedToDocumentModelCommand(this, documentModel, pos, false);
			command.CopyBetweenInternalModels = copyBetweenInternalModels;
			command.SuppressParentFieldsUpdate = suppressParentFieldsUpdate;
			command.SuppressFieldsUpdate = suppressFieldsUpdate;
			command.Execute();
		}
		[System.Diagnostics.Conditional("DEBUG")]
		internal void CheckIntegrity() {
#if DEBUGTEST && !DXPORTABLE
			DevExpress.XtraRichEdit.Tests.DocumentModelIntegrityValidator.CheckPieceTableIntegrity(this);
#endif
		}
		[System.Diagnostics.Conditional("DEBUG")]
		internal void ForceCheckTablesIntegrity() {
#if DEBUGTEST && !DXPORTABLE
			DevExpress.XtraRichEdit.Tests.DocumentModelIntegrityValidator.ForceCheckTablesIntegrity(this);
#endif
		}
		protected internal virtual RunInfo GetRunInfoByTableCell(TableCell cell) {
			Paragraph startParagraph = Paragraphs[cell.StartParagraphIndex];
			Paragraph endParagraph = Paragraphs[cell.EndParagraphIndex];
			DocumentLogPosition startLogPosition = startParagraph.LogPosition;
			int length = endParagraph.EndLogPosition - startLogPosition + 1;
			return FindRunInfo(startLogPosition, length);
		}
		protected internal virtual void FixTables() {
			int count = Tables.Count;
			for (int i = 0; i < count; i++) {
				Table table = Tables[i];
				table.Normalize();
				table.NormalizeRows();
				table.NormalizeTableGrid();
				table.NormalizeCellColumnSpans();
			}
		}
		protected internal virtual void ResetParagraphs() {
			ResetParagraphs(ParagraphIndex.Zero, new ParagraphIndex(Paragraphs.Count - 1));
		}
		protected internal virtual void ResetParagraphs(ParagraphIndex from, ParagraphIndex to) {
			ParagraphCollection paragraphs = Paragraphs;
			from = Algorithms.Max(from, ParagraphIndex.Zero);
			to = Algorithms.Min(to, new ParagraphIndex(paragraphs.Count - 1));
			from = DocumentChangesHandler.EnsurePositionVisibleWhenHiddenTextNotShown(DocumentModel, DocumentModelPosition.FromParagraphStart(this, from)).ParagraphIndex;
			foreach (Paragraph paragraph in paragraphs.Range(from, to)) {
				paragraph.BoxCollection.InvalidateBoxes();
			}
			TableCellsManager.ResetCachedTableLayoutInfo(from, to);
			calculatorCache.Clear();
		}
		public void ReplaceTextWithPicture(DocumentLogPosition position, int length, OfficeImage image) {
			DocumentModel.BeginUpdate();
			try {
				DeleteContent(position, length, false);
				InsertInlinePicture(position, image);
				DocumentModel.Selection.SetStartCell(DocumentModel.Selection.Start);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void ReplaceTextWithMultilineText(DocumentLogPosition position, int length, string text) {
			DocumentModel.BeginUpdate();
			try {
				DeleteContent(position, length, false);
				InsertPlainText(position, text);
				DocumentModel.Selection.SetStartCell(DocumentModel.Selection.Start);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#region ReplaceText
		public void ReplaceTextAndInheritFormatting(DocumentLogPosition position, int length, string text) {
			DocumentModel.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					RunInfo runInfo = ObtainAffectedRunInfo(position, length);
					ReplaceRunsWithNew(runInfo, text);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void ReplaceRunsWithNew(RunInfo runInfo, string text) {
			DocumentModelPosition start = runInfo.Start.Clone();
			DocumentModelPosition end = runInfo.End;
			int index = ReplaceRunsWithNewCore(start, end, text, 0);
			DocumentLogPosition startLogPosition = start.LogPosition;
			DocumentLogPosition endLogPosition = end.LogPosition;
			if (startLogPosition <= endLogPosition) {
				int length = endLogPosition - startLogPosition + (endLogPosition < DocumentEndLogPosition ? 1 : 0);
				DeleteContent(startLogPosition, length, endLogPosition == DocumentEndLogPosition);
			}
			if (index < text.Length)
				InsertText(startLogPosition, text.Substring(index), false);
		}
		int ReplaceRunsWithNewCore(DocumentModelPosition start, DocumentModelPosition end, string text, int index) {
			while (start <= end && index < text.Length) {
				RunIndex runIndex = start.RunIndex;
				TextRunBase run = Runs[runIndex];
				if (!VisibleTextFilter.IsRunVisible(runIndex)) {
					RunIndex visibleRun = VisibleTextFilter.GetNextVisibleRunIndex(runIndex);
					DocumentModelPosition.SetRunStart(start, visibleRun);
					return ReplaceRunsWithNewCore(start, end, text, index);
				}
				int substringLength = Math.Min(run.Length, text.Length - index);
				string subText = text.Substring(index, substringLength);
				ReplaceTextAndInheritFormatting(start.LogPosition, subText);
				index += substringLength;
				MovePositionToNext(start, substringLength);
			}
			return index;
		}
		void MovePositionToNext(DocumentModelPosition pos, int offset) {
			for (int i = 0; i < offset; i++)
				DocumentModelPosition.MoveForwardCore(pos);
		}
		void ReplaceTextAndInheritFormatting(DocumentLogPosition pos, string text) {
			DocumentLogPosition endPosition = pos + text.Length;
			InsertText(endPosition, text);
			DeleteContent(pos, endPosition - pos, false);
		}
		public void ReplaceText(DocumentLogPosition position, int length, string text) {
			if (position < DocumentLogPosition.Zero)
				Exceptions.ThrowArgumentException("position", position);
			Guard.ArgumentNonNegative(length, "length");
			bool isLastParagraphSelected = position + length > DocumentEndLogPosition;
			DocumentModel.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					DocumentLogPosition positionToInsert = position + length;
					if (!String.IsNullOrEmpty(text))
						InsertPlainText(positionToInsert, text);
					PieceTableDeleteTextCommand command = CreatePieceTableDeleteTextCommand(position, length);
					command.DocumentLastParagraphSelected = isLastParagraphSelected;
					command.LeaveFieldIfResultIsRemoved = true;
					command.Execute();
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
			DocumentModel.CheckIntegrity();
		}
		#endregion
		protected internal virtual void ResetFormattingCaches(ResetFormattingCacheType resetFromattingCacheType) {
			ParagraphCollection paragraphs = Paragraphs;
			foreach (Paragraph paragraph in paragraphs) {
				paragraph.ResetCachedIndices(resetFromattingCacheType);
			}
		}
		protected internal virtual void ModifyHyperlinkCode(Field field, HyperlinkInfo info) {
			DocumentModel.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					ReplaceHyperlinkInfo(field.Index, info);
					UpdateHyperlinkFieldCode(field);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void ModifyHyperlinkResult(Field field, string result) {
			DocumentModel.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					ChangeFieldResult(field, result);
					DocumentLogPosition start = DocumentModelPosition.FromRunStart(this, field.Result.Start).LogPosition;
					ApplyHyperlinkStyle(FindRunInfo(start, result.Length), true);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual List<Field> GetTocFields() {
			List<Field> result = new List<Field>();
			FieldCollection fields = Fields;
			int count = fields.Count;
			for (int i = 0; i < count; i++)
				if (IsTocField(fields[i]))
					result.Add(fields[i]);
			return result;
		}
		protected internal Token GetFieldToken(Field field) {
			DocumentFieldIterator iterator = new DocumentFieldIterator(this, field);
			FieldScanner scanner = new FieldScanner(iterator, DocumentModel.MaxFieldSwitchLength, DocumentModel.EnableFieldNames, SupportFieldCommonStringFormat);
			return scanner.Scan();
		}
		protected internal virtual bool IsTocField(Field field) {
			return GetFieldToken(field).Value == TocField.FieldType;
		}
		protected internal virtual FieldCalculatorService CreateFieldCalculatorService() {
			return new FieldCalculatorService();
		}
		public void MergeCells(SelectedCellsCollection selectedCellsCollection) {
			MergeCellsHorizontally(selectedCellsCollection);
			TableCell normalizedStartCell = selectedCellsCollection.NormalizedFirst.NormalizedStartCell;
			MergeTableCellsVertically(normalizedStartCell, selectedCellsCollection.RowsCount);
		}
		protected internal virtual void MergeCellsHorizontally(SelectedCellsCollection selectedCells) {
			int topRowIndex = selectedCells.GetNormalizedTopRowIndex();
			int bottomRowIndex = selectedCells.GetNormalizedBottomRowIndex();
			int direction = bottomRowIndex > topRowIndex ? 1 : -1;
			int count = Math.Abs(bottomRowIndex - topRowIndex);
			for (int i = 0; i <= count; i++) {
				SelectedCellsIntervalInRow cellsInterval = selectedCells[topRowIndex + i * direction];
				TableCell startCell = cellsInterval.NormalizedStartCell;
				int columnSpan = cellsInterval.GetNormalizedColumnSpan(); ;
				if (startCell.VerticalMerging == MergingState.Restart)
					i += MergeCellsHorizontallyCore(startCell, columnSpan);
				MergeTableCellsHorizontally(startCell, columnSpan);
			}
		}
		protected internal virtual int MergeCellsHorizontallyCore(TableCell startCell, int mergedCellsColumnSpan) {
			int columnIndex = TableCellVerticalBorderCalculator.GetStartColumnIndex(startCell, false);
			List<TableCell> cells = TableCellVerticalBorderCalculator.GetVerticalSpanCells(startCell, columnIndex, false);
			int cellsCount = cells.Count;
			for (int i = 1; i < cellsCount; i++)
				MergeTableCellsHorizontally(cells[i], mergedCellsColumnSpan);
			return cellsCount - 1;
		}
		protected internal virtual DeleteContentOperation CreateDeleteContentOperation() {
			return new DeleteContentOperation(this);
		}
		protected internal virtual List<Bookmark> GetBookmarks(bool includeHiddenBookmarks) {
			if (includeHiddenBookmarks)
				return Bookmarks.InnerList;
			else {
				List<Bookmark> result = new List<Bookmark>();
				List<Bookmark> bookmarks = Bookmarks.InnerList;
				int count = bookmarks.Count;
				for (int i = 0; i < count; i++) {
					if (!bookmarks[i].IsHidden)
						result.Add(bookmarks[i]);
				}
				return result;
			}
		}
		protected internal void AddPieceTables(List<PieceTable> result, bool includeUnreferenced) {
			if ((includeUnreferenced || this.IsReferenced) && !result.Contains(this))
				result.Add(this);
			int count = TextBoxes.Count;
			for (int i = 0; i < count; i++) {
				if (this != textBoxes[i].PieceTable)
					textBoxes[i].PieceTable.AddPieceTables(result, includeUnreferenced);
			}
			count = Comments.Count;
			for (int i = 0; i < count; i++)
				Comments[i].Content.PieceTable.AddPieceTables(result, includeUnreferenced);
		}
		protected internal virtual void EnsureImageLoadComplete() {
			Runs.ForEach(EnsureImageLoadComplete);
		}
		protected internal virtual void EnsureImageLoadComplete(TextRunBase run) {
			InlinePictureRun inlinePictureRun = run as InlinePictureRun;
			if (inlinePictureRun != null)
				InternalOfficeImageHelper.EnsureLoadComplete(inlinePictureRun.Image);
		}
		protected internal virtual void PreprocessContentBeforeExport(DocumentFormat format) {
		}
		internal List<IZOrderedObject> GetFloatingObjectList() {
			List<IZOrderedObject> floatingObjectList = new List<IZOrderedObject>();
			RunIndex count = new RunIndex(Runs.Count);
			for (RunIndex i = RunIndex.Zero; i < count; i++) {
				TextRunBase run = Runs[i];
				FloatingObjectAnchorRun anchorRun = run as FloatingObjectAnchorRun;
				if (anchorRun != null)
					floatingObjectList.Add(anchorRun.FloatingObjectProperties);
			}
			FloatingObjectZOrderComparer comparer = new FloatingObjectZOrderComparer();
			floatingObjectList.Sort(comparer);
			return floatingObjectList;
		}
		protected internal DocumentModelCopyManager GetCopyManager(PieceTable sourcePieceTable, DevExpress.XtraRichEdit.API.Native.InsertOptions insertOptions) {
			switch (insertOptions) {
				case DevExpress.XtraRichEdit.API.Native.InsertOptions.KeepSourceFormatting:
					return GetCopyManagerCore(sourcePieceTable, ParagraphNumerationCopyOptions.CopyAlways, FormattingCopyOptions.KeepSourceFormatting);
				case DevExpress.XtraRichEdit.API.Native.InsertOptions.MatchDestinationFormatting:
					return GetCopyManagerCore(sourcePieceTable, ParagraphNumerationCopyOptions.CopyIfWholeSelected, FormattingCopyOptions.UseDestinationStyles);
				default:
					return null;
			}
		}
		protected internal virtual DocumentModelCopyManager GetCopyManagerCore(PieceTable sourcePieceTable, ParagraphNumerationCopyOptions paragraphNumerationCopyOptions, FormattingCopyOptions formattingCopyOptions) {
			return new DocumentModelCopyManager(sourcePieceTable, this, paragraphNumerationCopyOptions, formattingCopyOptions);
		}
		NumberingListCalculatorCache calculatorCache;
		protected internal virtual int[] GetRangeListCounters(Paragraph paragraph) {
			Debug.Assert(paragraph.IsInList());
			return calculatorCache.GetRangeListCounters(paragraph);
		}				
	}
	#endregion
	public class NumberingListCalculatorCache {
		readonly Dictionary<AbstractNumberingListIndex, NumberingListCountersCalculator> cache;
		readonly DocumentModel documentModel;
		ParagraphIndex lastProcessedParagraphIndex;
		public NumberingListCalculatorCache(DocumentModel documentModel) {
			this.cache = new Dictionary<AbstractNumberingListIndex, NumberingListCountersCalculator>();
			this.documentModel = documentModel;
			lastProcessedParagraphIndex = new ParagraphIndex(-1);
		}
		DocumentModel DocumentModel { get { return documentModel; } }
		public int[] GetRangeListCounters(Paragraph paragraph) {
			Debug.Assert(paragraph.IsInList());
			ParagraphIndex index = paragraph.Index;
			if (index <= lastProcessedParagraphIndex)
				Clear();
			foreach (Paragraph currentParagraph in paragraph.PieceTable.Paragraphs.Range(lastProcessedParagraphIndex + 1, index)) {
				AbstractNumberingListIndex listIndex = currentParagraph.GetAbstractNumberingListIndex();
				if (listIndex < AbstractNumberingListIndex.MinValue)
					continue;
				NumberingListCountersCalculator calculator = GetCacheItem(listIndex);
				int listLevelIndex = currentParagraph.GetListLevelIndex();
				if(calculator.ShouldAdvanceListLevelCounters(currentParagraph, DocumentModel.AbstractNumberingLists[listIndex]))
					calculator.AdvanceListLevelCounters(currentParagraph, listLevelIndex);
				if (currentParagraph == paragraph) {
					lastProcessedParagraphIndex = index;
					return calculator.GetActualRangeCounters(listLevelIndex);
				}
			}
			Debug.Assert(false);
			return null;
		}
		public void Clear() {
			cache.Clear();
			lastProcessedParagraphIndex = new ParagraphIndex(-1);
		}
		NumberingListCountersCalculator GetCacheItem(AbstractNumberingListIndex listIndex) {
			NumberingListCountersCalculator result;
			if (cache.TryGetValue(listIndex, out result))
				return result;
			AbstractNumberingList list = DocumentModel.AbstractNumberingLists[listIndex];
			result = DocumentModel.CreateNumberingListCountersCalculator(list);
			result.BeginCalculateCounters();
			cache.Add(listIndex, result);
			return result;
		}
	}
}
