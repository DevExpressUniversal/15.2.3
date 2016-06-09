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
using System.Globalization;
using System.Text;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraSpellChecker.Parser;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.SpellChecker {
	#region Old
	#endregion
	public enum CheckSpellingState {
		BeginningCheck,
		Restart,
		WaitForNextPage,
		StopCheck
	}
	#region SpellCheckerControllerBase (abstract class)
	public abstract class SpellCheckerControllerBase : IDisposable {
		#region Fields
		InnerRichEditControl control;
		PieceTable pieceTable;
		DocumentLayout documentLayout;
		bool isDisposed;
		#endregion
		protected SpellCheckerControllerBase(InnerRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.pieceTable = control.DocumentModel.MainPieceTable;
			this.documentLayout = control.ActiveView.DocumentLayout;
		}
		protected SpellCheckerControllerBase() {
		}
		#region Properties
		public InnerRichEditControl Control { get { return control; } }
		public virtual PieceTable PieceTable {
			get { return pieceTable; }
			set {
				Guard.ArgumentNotNull(value, "PieceTable");
				if (pieceTable == value)
					return;
				pieceTable = value;
				OnPieceTableChanged();
			}
		}
		public bool IsDisposed { get { return isDisposed; } }
		protected BoxMeasurer BoxMeasurer { get { return Control.Measurer; } }
		protected DocumentLayout DocumentLayout { get { return documentLayout; } set { documentLayout = value; } }
		protected PageCollection Pages { get { return DocumentLayout.Pages; } }
		protected DocumentModel DocumentModel { get { return Control.DocumentModel; } }
		#endregion
		public virtual void SubscribeToEvents() {
		}
		public virtual void UnsubscribeToEvents() {
		}
		protected virtual SpellCheckerErrorBoxCalculator CreateErrorBoxCalculator() {
			return new SpellCheckerErrorBoxCalculator(DocumentLayout, BoxMeasurer, PieceTable);
		}
		protected virtual void OnPieceTableChanged() {
		}
		public virtual void Reset() {
		}
		public virtual void CheckPages(int from) {
			if (!CanCheckDocument())
				return;
			int startIndex = DocumentLayout.FirstVisiblePageIndex;
			int endIndex = DocumentLayout.LastVisiblePageIndex;
			if (startIndex > endIndex || endIndex < 0)
				return;
			for (int i = startIndex; i <= endIndex; i++) {
				Page page = Pages[i];
				if (page.CheckSpellingComplete)
					continue;
				CheckPage(page);
				page.CheckSpellingComplete = true;
			}
		}
		protected internal virtual void CheckPage(Page page) {
			int pageIndex = page.PageIndex;
			HeaderPageArea header = page.Header;
			if (header != null) {
				PieceTable = header.Header.PieceTable;
				CheckHeaderFooter(header, pageIndex);
			}
			FooterPageArea footer = page.Footer;
			if (footer != null) {
				PieceTable = footer.Footer.PieceTable;
				CheckHeaderFooter(footer, pageIndex);
			}
			CheckFloatingObjects(page);
			CheckComments(page);
			PieceTable = DocumentModel.MainPieceTable;
			CheckPageContent(page);
		}
		void CheckComments(Page page) {
			if (page.InnerComments == null || page.InnerComments.Count == 0)
				return;
			int count = page.InnerComments.Count;
			for (int i = 0; i < count; i++) {
				CommentViewInfo viewInfo = page.InnerComments[i];
				if (viewInfo.CommentDocumentLayout != null)
					CheckComment(viewInfo);
			}
		}
		void CheckComment(CommentViewInfo viewInfo) {
			DocumentLayout prevDocumentLayout = DocumentLayout;
			DocumentLayout = viewInfo.CommentDocumentLayout;
			PieceTable prevPieceTable = PieceTable;
			PieceTable = viewInfo.Comment.Content.PieceTable;
			try {
				CheckPieceTable();
			}
			finally {
				DocumentLayout = prevDocumentLayout;
				PieceTable = prevPieceTable;
			}
		}
		void CheckPieceTable() {
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(PieceTable, PieceTable.DocumentStartLogPosition);
			DocumentModelPosition end = PositionConverter.ToDocumentModelPosition(PieceTable, PieceTable.DocumentEndLogPosition);
			CheckContent(start, end, 0);
		}
		void CheckFloatingObjects(Page page) {
			CheckFloatingObjects(page.BackgroundFloatingObjects);
			CheckFloatingObjects(page.GetSortedNonBackgroundFloatingObjects());
		}
		void CheckFloatingObjects(IList<FloatingObjectBox> floatingObjects) {
			if (floatingObjects == null)
				return;
			int count = floatingObjects.Count;
			for (int i = 0; i < count; i++) {
				FloatingObjectBox floatingObject = floatingObjects[i];
				if (floatingObject.PieceTable == PieceTable && floatingObject.DocumentLayout != null)
					CheckTextBoxContent(floatingObject);
			}
		}
		void CheckTextBoxContent(FloatingObjectBox floatingObject) {
			FloatingObjectAnchorRun run = (FloatingObjectAnchorRun)floatingObject.PieceTable.Runs[floatingObject.StartPos.RunIndex];
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (!textBoxContent.TextBox.IsTextBox)
				return;
			DocumentLayout prevDocumentLayout = DocumentLayout;
			DocumentLayout = floatingObject.DocumentLayout;
			PieceTable prevPieceTable = PieceTable;
			PieceTable = textBoxContent.TextBox.PieceTable;
			try {
				CheckPieceTable();
			}
			finally {
				DocumentLayout = prevDocumentLayout;
				PieceTable = prevPieceTable;
			}
		}
		void CheckPageContent(Page page) {
			DocumentModelPosition start = page.GetFirstPosition(PieceTable);
			DocumentModelPosition end = page.GetLastPosition(PieceTable);
			CheckContent(start, end, page.PageIndex);
		}
		DocumentModelPosition CalculateStartPosition(Page page, DocumentModelPosition prevPosition) {
			return prevPosition != null ? DocumentModelPosition.MoveForward(prevPosition) : page.GetFirstPosition(PieceTable);
		}
		protected virtual bool CanCheckDocument() {
			return !IsDisposed && DocumentLayout != null;
		}
		protected internal virtual void CheckHeaderFooter(PageArea area, int pageIndex) {
			DocumentModelPosition start = area.GetFirstPosition(PieceTable);
			DocumentModelPosition end = area.GetLastPosition(PieceTable);
			CheckContent(start, end, pageIndex);
		}
		protected internal abstract void CheckContent(DocumentModelPosition start, DocumentModelPosition end, int pageIndex);
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				this.control = null;
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region Old 2
	#endregion
	#region EmptySpellCheckerController
	public class EmptySpellCheckerController : SpellCheckerControllerBase {
		public EmptySpellCheckerController() {
		}
		public override PieceTable PieceTable { get { return base.PieceTable; } set { } }
		public override void Reset() {
		}
		protected override void Dispose(bool disposing) {
		}
		protected internal override void CheckContent(DocumentModelPosition start, DocumentModelPosition end, int pageIndex) {
		}
		public override void CheckPages(int from) {
		}
	}
	#endregion
	#region SyntaxCheckController
	public class SyntaxCheckController : SpellCheckerControllerBase {
		readonly ISyntaxCheckService syntaxChecker;
		public SyntaxCheckController(InnerRichEditControl control, ISyntaxCheckService syntaxChecker)
			: base(control) {
			Guard.ArgumentNotNull(syntaxChecker, "syntaxChecker");
			this.syntaxChecker = syntaxChecker;
		}
		protected internal override void CheckContent(DocumentModelPosition start, DocumentModelPosition end, int pageIndex) {
			RunInfo[] intervals = syntaxChecker.Check(start, end);
			int count = intervals.Length;
			SpellCheckerErrorBoxCalculator calculator = CreateErrorBoxCalculator();
			for (int i = 0; i < count; i++)
				calculator.Calculate(pageIndex, intervals[i].Start, intervals[i].End, SpellingError.Syntax);
		}
	}
	#endregion
	#region StringBuffer
	public class StringBuffer {
		#region Fields
		readonly PieceTable pieceTable;
		int bufferSize;
		int defaultAppendSize;
		StringBuilder buffer = new StringBuilder();
		DocumentModelPosition start;
		DocumentModelPosition end;
		#endregion
		public StringBuffer(PieceTable pieceTable, int bufferSize, int defaultAppendSize) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			Guard.ArgumentNonNegative(bufferSize, "bufferSize");
			this.bufferSize = bufferSize;
			Guard.ArgumentNonNegative(defaultAppendSize, "defaultAppendSize");
			this.defaultAppendSize = defaultAppendSize;
			this.start = new DocumentModelPosition(pieceTable);
			this.end = new DocumentModelPosition(pieceTable);
			this.buffer = new StringBuilder();
		}
		public StringBuffer(PieceTable pieceTable)
			: this(pieceTable, 120, 10) {
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public int BufferSize { get { return bufferSize; } }
		public int DefaultAppendSize { get { return defaultAppendSize; } }
		public char this[int charIndex] { get { return buffer[charIndex]; } set { buffer[charIndex] = value; } }
		public DocumentModelPosition Start { get { return start; } set { start = value; } }
		public DocumentModelPosition End { get { return end; } set { end = value; } }
		public int Length { get { return buffer.Length; } set { buffer.Length = value; } }
		public bool IsEmpty { get { return Start == End; } }
		protected internal StringBuilder Buffer { get { return buffer; } }
		#endregion
		public void Initialize(DocumentModelPosition pos) {
			this.buffer.Length = 0;
			this.start = pos.Clone();
			this.end = pos.Clone();
		}
		public DocumentModelPosition GetPosition(DocumentModelPosition start, int offset) {
			TextRunCollection runs = PieceTable.Runs;
			RunIndex lastRunIndex = new RunIndex(runs.Count - 1);
			int runOffset = 0;
			DocumentLogPosition pos = start.LogPosition;
			DocumentLogPosition startRunPos = start.RunStartLogPosition;
			int totalOffset = 0;
			for (RunIndex i = start.RunIndex; i <= lastRunIndex; i++) {
				TextRunBase run = runs[i];
				runOffset = startRunPos + run.Length - pos;
				if (PieceTable.VisibleTextFilter.IsRunVisible(i)) {
					if ((offset < totalOffset + runOffset) || i == lastRunIndex) {
						DocumentModelPosition result = new DocumentModelPosition(PieceTable);
						result.LogPosition = Algorithms.Min((pos + (offset - totalOffset)), PieceTable.DocumentEndLogPosition);
						result.RunStartLogPosition = startRunPos;
						result.RunIndex = i;
						result.ParagraphIndex = run.Paragraph.Index;
						return result;
					}
					totalOffset += runOffset;
				}
				pos += runOffset;
				startRunPos += run.Length;
			}
			Exceptions.ThrowInternalException();
			return null;
		}
		public DocumentModelPosition GetPosition(int index) {
			return GetPosition(Start, index);
		}
		public void AppendBuffer(StringBuffer buffer) {
			if (End.LogPosition >= PieceTable.DocumentEndLogPosition)
				return;
			int delta = BufferSize - buffer.Length;
			if (delta <= 0)
				delta = DefaultAppendSize;
			DocumentModelPosition endPosition = GetPosition(End, delta);
			AppendBufferCore(End, endPosition);
			End = endPosition;
		}
		void AppendBufferCore(DocumentModelPosition start, DocumentModelPosition end) {
			if (end.LogPosition - start.LogPosition <= 0)
				return;
			TextRunBase startRun = PieceTable.Runs[start.RunIndex];
			if (start.RunIndex == end.RunIndex) {
				AppendRunText(start.RunIndex, start.RunOffset, end.RunOffset - 1);
				return;
			}
			AppendRunText(start.RunIndex, start.RunOffset, startRun.Length - 1);
			for (RunIndex i = start.RunIndex + 1; i < end.RunIndex; i++)
				AppendRunText(i);
			if (end.RunOffset > 0)
				AppendRunText(end.RunIndex, 0, end.RunOffset - 1);
		}
		void AppendRunText(RunIndex index) {
			if (PieceTable.VisibleTextFilter.IsRunVisible(index))
				buffer.Append(PieceTable.Runs[index].GetNonEmptyText(PieceTable.TextBuffer));
		}
		void AppendRunText(RunIndex index, int from, int to) {
			if (PieceTable.VisibleTextFilter.IsRunVisible(index))
				buffer.Append(PieceTable.Runs[index].GetText(PieceTable.TextBuffer, from, to));
		}
		public bool ShiftBuffer(DocumentModelPosition pos, int offset) {
			if (pos == null || pos == Start)
				return false;
			string newValue = ToString(offset);
			buffer.Length = 0;
			buffer.Append(newValue);
			Start = pos.Clone();
			return true;
		}
		public string ToString(int startIndex) {
			return this.buffer.ToString(startIndex, Length - startIndex);
		}
		public override string ToString() {
			return this.buffer.ToString();
		}
		public string ToString(int index, int count) {
			return this.buffer.ToString(index, count);
		}
	}
	#endregion
	#region SpellCheckerController
	public class SpellCheckerController : SpellCheckerControllerBase {
		protected const int UpperLimit = 2000;
		protected const int LowerLimit = 1000;
		#region Fields
		ISpellChecker spellChecker;
		readonly IgnoreListManager ignoreListManager;
		readonly CheckAsYouTypeTextController textController;
		#endregion
		public SpellCheckerController(InnerRichEditControl control)
			: base(control) {
			this.spellChecker = control.SpellChecker;
			this.ignoreListManager = new IgnoreListManager(DocumentModel);
			this.textController = new CheckAsYouTypeTextController(control);
			SpellChecker.UnregisterIgnoreList(control.Owner);
			SpellChecker.RegisterIgnoreList(control.Owner, ignoreListManager);
			SubscribeToEvents();
		}
		protected SpellCheckerController()
			: base() {
		}
		#region Properties
		public ISpellChecker SpellChecker { get { return spellChecker; } }
		protected CultureInfo Culture { get { return SpellChecker.Culture; } }
		protected internal IgnoreListManager IgnoreListManager { get { return ignoreListManager; } }
		protected internal CheckAsYouTypeTextController TextController { get { return textController; } }
		#endregion
		#region Subscribe/Unsubscribe Events
		public override void SubscribeToEvents() {
			if (SpellChecker != null)
				SubscribeSpellCheckerEvents();
		}
		public override void UnsubscribeToEvents() {
			if (SpellChecker != null)
				UnsubscribeSpellCheckerEvents();
		}
		protected internal virtual void SubscribeSpellCheckerEvents() {
			SpellChecker.AfterCheckWord += new AfterCheckWordEventHandler(OnAfterCheckWord);
			SpellChecker.SpellCheckModeChanged += new EventHandler(OnSpellCheckModeChanged);
			SpellChecker.CultureChanged += new EventHandler(OnSpellCheckerCultureChanged);
			SpellChecker.WordAdded += new WordAddedEventHandler(OnWordAdded);
			SpellChecker.CustomDictionaryChanged += new EventHandler(OnCustomDictionaryChanged);
			SpellChecker.GetOptions(Control.Owner).OptionChanged += new EventHandler(OnOptionsChanged);
		}
		protected internal virtual void UnsubscribeSpellCheckerEvents() {
			SpellChecker.AfterCheckWord -= new AfterCheckWordEventHandler(OnAfterCheckWord);
			SpellChecker.SpellCheckModeChanged -= new EventHandler(OnSpellCheckModeChanged);
			SpellChecker.CultureChanged -= new EventHandler(OnSpellCheckerCultureChanged);
			SpellChecker.WordAdded -= new WordAddedEventHandler(OnWordAdded);
			SpellChecker.CustomDictionaryChanged -= new EventHandler(OnCustomDictionaryChanged);
			SpellChecker.GetOptions(Control.Owner).OptionChanged -= new EventHandler(OnOptionsChanged);
		}
		protected internal virtual void OnAfterCheckWord(object sender, AfterCheckWordEventArgs e) {
			if (!IsIgnoreOperation(e.Operation) || e.EditControl != Control.Owner)
				return;
			DocumentPosition position = e.StartPosition as DocumentPosition;
			if (position != null) {
				PieceTable pieceTable = position.Position.PieceTable;
				DocumentModelPosition modelPos = PositionConverter.ToDocumentModelPosition(pieceTable, position.LogPosition);
				ResetDocumentFormatting(modelPos.RunIndex);
			}
			else
				ResetDocumentFormatting(RunIndex.Zero);
		}
		protected internal virtual bool IsIgnoreOperation(SpellCheckOperation operation) {
			return operation == SpellCheckOperation.Ignore || IsIgnoreAllOperation(operation);
		}
		protected internal virtual bool IsIgnoreAllOperation(SpellCheckOperation operation) {
			return operation == SpellCheckOperation.IgnoreAll || operation == SpellCheckOperation.SilentIgnore;
		}
		protected internal virtual void OnSpellCheckModeChanged(object sender, EventArgs e) {
			DocumentModel.ResetSpellCheck(RunIndex.Zero, RunIndex.MaxValue, true);
		}
		protected internal virtual void OnSpellCheckerCultureChanged(object sender, EventArgs e) {
			ResetSpellCheck(RunIndex.Zero);
		}
		protected internal virtual void OnWordAdded(object sender, WordAddedEventArgs e) {
			ResetSpellCheck(RunIndex.Zero);
		}
		protected internal virtual void OnOptionsChanged(object sender, EventArgs e) {
			ResetSpellCheck(RunIndex.Zero);
		}
		protected internal virtual void OnCustomDictionaryChanged(object sender, EventArgs e) {
			ResetSpellCheck(RunIndex.Zero);
		}
		void ResetDocumentFormatting(RunIndex startRunIndex) {
			if (SpellChecker.SpellCheckMode == SpellCheckMode.AsYouType)
				DocumentModel.ResetSpellCheck(startRunIndex, RunIndex.MaxValue, true);
		}
		void ResetSpellCheck(RunIndex startRunIndex) {
			ResetSpellCheck(startRunIndex, RunIndex.MaxValue);
		}
		void ResetSpellCheck(RunIndex startRunIndex, RunIndex endRunIndex) {
			if (SpellChecker.SpellCheckMode == SpellCheckMode.AsYouType)
				DocumentModel.ResetSpellCheck(startRunIndex, endRunIndex);
		}
		#endregion
		public override void Reset() {
			base.Reset();
			TextController.Reset();
		}
		protected internal override void CheckContent(DocumentModelPosition start, DocumentModelPosition end, int pageIndex) {
			if (PieceTable.SpellCheckerManager.MisspelledIntervals.Count > LowerLimit)
				return;
			SpellCheckerWordIterator wordIterator = new SpellCheckerWordIterator(PieceTable);
			DocumentModelPosition intervalStart;
			if (start.LogPosition == PieceTable.DocumentStartLogPosition)
				intervalStart = start;
			else
				intervalStart = wordIterator.MoveToWordStart(start, false);
			DocumentModelPosition intervalEnd;
			if (end.LogPosition == PieceTable.DocumentEndLogPosition)
				intervalEnd = end;
			else {
				DocumentModelPosition wordEnd = wordIterator.MoveToWordEnd(end);
				intervalEnd = Algorithms.Max(wordEnd, DocumentModelPosition.MoveForward(end));
			}
			if (CheckContentCore(intervalStart, intervalEnd))
				CalculateErrorBoxes(start, end, pageIndex);
			else
				ClearErrorBoxes(DocumentLogPosition.Zero, end.LogPosition);
		}
		bool CheckContentCore(DocumentModelPosition start, DocumentModelPosition end) {
			SpellCheckerInterval[] intervals = PieceTable.SpellCheckerManager.PopUncheckedIntervals(start, end);
			if (intervals == null || intervals.Length == 0)
				return true;
			int count = intervals.Length;
			for (int index = 0; index < count; index++) {
				SpellCheckerInterval interval = intervals[index];
				PieceTable.SpellCheckerManager.RemoveMisspelledIntervals(interval.Start, interval.End);
				if (!CheckInterval(interval))
					return false;
			}
			return true;
		}
		protected internal virtual void ClearErrorBoxes(DocumentLogPosition from, DocumentLogPosition to) {
			SpellCheckerErrorBoxCalculator calculator = CreateErrorBoxCalculator();
			calculator.ClearErrorBoxes(from, to);
		}
		protected internal virtual void CalculateErrorBoxes(DocumentModelPosition start, DocumentModelPosition end, int pageIndex) {
			MisspelledInterval[] intervals = PieceTable.SpellCheckerManager.MisspelledIntervals.GetIntervals(start.LogPosition, end.LogPosition);
			if (intervals == null || intervals.Length == 0)
				return;
			SpellCheckerErrorBoxCalculator calculator = CreateErrorBoxCalculator();
			int count = intervals.Length;
			for (int index = 0; index < count; index++) {
				RunInfo interval = intervals[index].Interval;
				Debug.Assert(interval.Start < interval.End);
				DocumentModelPosition boxStartPos = Algorithms.Max(start, interval.Start);
				DocumentModelPosition boxEndPos = Algorithms.Min(end, DocumentModelPosition.MoveBackward(interval.End));
				calculator.Calculate(pageIndex, boxStartPos, boxEndPos, intervals[index].ErrorType);
			}
		}
		protected internal virtual bool CheckInterval(SpellCheckerInterval interval) {
			DocumentModelPosition current = interval.Interval.Start.Clone();
			DocumentModelPosition endModelPosition = interval.Interval.End.Clone();
			DocumentPosition end = new DocumentPosition(endModelPosition);
			while (current < endModelPosition) {
				ISpellingErrorInfo errorInfo = SpellChecker.Check(Control.Owner, TextController, new DocumentPosition(current), end);
				if (errorInfo == null)
					break;
				DocumentModelPosition startPosition = ((DocumentPosition)errorInfo.WordStartPosition).Position;
				DocumentModelPosition endPosition = ((DocumentPosition)errorInfo.WordEndPosition).Position;
				CalculateMisspelledInterval(startPosition, endPosition, errorInfo.Word, errorInfo.Error);
				current.CopyFrom(endPosition);
				if (IsUpperLimitExceeded())
					return false;
			}
			return true;
		}
		protected internal virtual bool IsUpperLimitExceeded() {
			return PieceTable.SpellCheckerManager.MisspelledIntervals.Count > UpperLimit;
		}
		protected virtual void CalculateMisspelledInterval(DocumentModelPosition start, DocumentModelPosition end, string word, SpellingError error) {
			if (!IgnoreListManager.IgnoredList.Contains(start.LogPosition, end.LogPosition, word))
				PieceTable.SpellCheckerManager.CreateMisspelledInterval(start, end, error);
		}
		protected override bool CanCheckDocument() {
			return base.CanCheckDocument() && SpellChecker.SpellCheckMode == SpellCheckMode.AsYouType;
		}
		protected override void OnPieceTableChanged() {
			base.OnPieceTableChanged();
			TextController.SetPieceTable(PieceTable);
		}
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (spellChecker != null) {
					UnsubscribeToEvents();
					spellChecker.UnregisterIgnoreList(Control.Owner);
					spellChecker = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
	}
	#endregion
	#region CheckAsYouTypeTextController
	public class CheckAsYouTypeTextController : RichEditTextController {
		SpellCheckerWordIterator wordIterator;
		PieceTable pieceTable;
		public CheckAsYouTypeTextController(InnerRichEditControl control)
			: base(control) {
			SetPieceTable(control.DocumentModel.ActivePieceTable);
		}
		protected override PieceTable PieceTable { get { return pieceTable; } }
		internal void SetPieceTable(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
			Reset();
		}
		protected override SpellCheckerWordIterator GetWordIterator(PieceTable pieceTable) {
			if (this.wordIterator == null || !Object.ReferenceEquals(this.wordIterator.PieceTable, PieceTable))
				this.wordIterator = new SpellCheckerWordIterator(PieceTable);
			return wordIterator;
		}
		public void Reset() {
			this.wordIterator = null;
		}
	}
	#endregion
	#region SpellCheckerInterval (abstract class)
	public abstract class SpellCheckerInterval : ChangableDocumentInterval {
		protected SpellCheckerInterval(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void OnChangedCore() {
		}
	}
	#endregion
	#region UncheckedInterval
	public class UncheckedInterval : SpellCheckerInterval {
		public UncheckedInterval(PieceTable pieceTable)
			: base(pieceTable) {
		}
	}
	#endregion
	#region MisspelledInterval
	public class MisspelledInterval : SpellCheckerInterval {
		SpellingError errorType;
		public MisspelledInterval(PieceTable pieceTable, SpellingError errorType)
			: base(pieceTable) {
			this.errorType = errorType;
		}
		public SpellingError ErrorType { get { return errorType; } set { errorType = value; } }
		protected internal override void OnChangedCore() {
			base.OnChangedCore();
			Debug.Assert(Start < End);
		}
	}
	#endregion
	#region IgnoredInterval
	public class IgnoredInterval : SpellCheckerInterval {
		public IgnoredInterval(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void OnChangedCore() {
			base.OnChangedCore();
			Debug.Assert(Start < End);
		}
	}
	#endregion
	#region SpellCheckerIntervalCollection<T> (abstract class)
	public abstract class SpellCheckerIntervalCollection<T> where T : SpellCheckerInterval {
		#region SpellCheckerInervalAndLogPositionComparer (inner class)
		class SpellCheckerIntervalAndLogPositionComparer : IComparable<T> {
			readonly DocumentLogPosition position;
			public SpellCheckerIntervalAndLogPositionComparer(DocumentLogPosition position) {
				this.position = position;
			}
			#region IComparable<T> Members
			public int CompareTo(T other) {
				if (position < other.Start)
					return 1;
				if (position >= other.End)
					return -1;
				return 0;
			}
			#endregion
		}
		#endregion
		#region SpellCheckerInervalAndIntervalComparer (inner class)
		class SpellCheckerIntervalAndIntervalComparer : IComparable<T> {
			readonly DocumentLogPosition start;
			readonly DocumentLogPosition end;
			public SpellCheckerIntervalAndIntervalComparer(DocumentLogPosition start, DocumentLogPosition end) {
				this.start = start;
				this.end = end;
			}
			#region IComparable<T> Members
			public int CompareTo(T other) {
				if (end <= other.Start)
					return 1;
				if (start >= other.End)
					return -1;
				return 0;
			}
			#endregion
		}
		#endregion
		readonly PieceTable pieceTable;
		readonly List<T> innerList;
		protected SpellCheckerIntervalCollection(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.innerList = new List<T>();
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public T this[int index] { get { return InnerList[index]; } set { Add(value); } }
		public int Count { get { return InnerList.Count; } }
		protected internal List<T> InnerList { get { return innerList; } }
		#endregion
		public virtual void Clear() {
			InnerList.Clear();
		}
		public virtual void Add(T newInterval) {
			Debug.Assert(Object.ReferenceEquals(PieceTable, newInterval.PieceTable));
			int index = BinarySearch(newInterval.Start);
			if (index >= 0)
				Exceptions.ThrowInternalException();
			index = ~index;
			InnerList.Insert(index, newInterval);
		}
		public virtual void Remove(T interval) {
			InnerList.Remove(interval);
		}
		public virtual T[] GetIntervals(DocumentLogPosition start, DocumentLogPosition end) {
			int startIndex = BinarySearch(start);
			if (startIndex < 0)
				startIndex = ~startIndex;
			int endIndex = BinarySearch(Algorithms.Max(end - 1, DocumentLogPosition.Zero));
			if (endIndex < 0)
				endIndex = (~endIndex) - 1;
			if (startIndex > endIndex)
				return null;
			return InnerList.GetRange(startIndex, endIndex - startIndex + 1).ToArray();
		}
		public virtual T FindInerval(DocumentLogPosition position) {
			int index = BinarySearch(position);
			if (index >= 0)
				return InnerList[index];
			index = ~index;
			index--;
			if (index < 0)
				return null;
			T interval = InnerList[index];
			if (interval.End == position)
				return interval;
			return null;
		}
		public virtual T FindInerval(DocumentLogPosition start, DocumentLogPosition end) {
			int index = BinarySearch(start, end);
			return index >= 0 ? InnerList[index] : null;
		}
		public virtual void RemoveRange(T[] range) {
			int count = range.Length;
			for (int index = 0; index < count; index++)
				InnerList.Remove(range[index]);
		}
		public virtual void RemoveRange(DocumentLogPosition start, DocumentLogPosition end) {
			T[] intervals = GetIntervals(start, end);
			if (intervals != null)
				RemoveRange(intervals);
		}
		public virtual void Remove(DocumentLogPosition position) {
			T interval = FindInerval(position);
			if (interval != null)
				InnerList.Remove(interval);
		}
		protected internal int BinarySearch(DocumentLogPosition position) {
			return Algorithms.BinarySearch(InnerList, new SpellCheckerIntervalAndLogPositionComparer(position));
		}
		protected internal int BinarySearch(DocumentLogPosition start, DocumentLogPosition end) {
			return Algorithms.BinarySearch(InnerList, new SpellCheckerIntervalAndIntervalComparer(start, end));
		}
		protected internal virtual void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			for (int i = 0; i < Count; i++)
				InnerList[i].OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
		}
		protected internal virtual void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			for (int i = 0; i < Count; i++)
				InnerList[i].OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		protected internal virtual void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			for (int i = 0; i < Count; i++)
				InnerList[i].OnRunMerged(paragraphIndex, runIndex, deltaRunLength);
		}
		protected internal virtual void OnParagraphRemoved(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			for (int i = 0; i < Count; i++)
				InnerList[i].OnParagraphRemoved(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		protected internal virtual void OnParagraphMerged(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			for (int i = 0; i < Count; i++)
				InnerList[i].OnParagraphMerged(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		protected internal virtual void OnParagraphInserted(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			for (int i = 0; i < Count; i++)
				InnerList[i].OnParagraphInserted(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		protected internal virtual void OnRunSplit(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			for (int i = 0; i < Count; i++)
				InnerList[i].OnRunSplit(paragraphIndex, runIndex, splitOffset);
		}
		protected internal virtual void OnRunJoined(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			for (int i = 0; i < Count; i++)
				InnerList[i].OnRunJoined(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		protected internal virtual void OnRunUnmerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			for (int i = 0; i < Count; i++)
				InnerList[i].OnRunUnmerged(paragraphIndex, runIndex, deltaRunLength);
		}
	}
	#endregion
	#region UncheckedIntervalCollection
	public class UncheckedIntervalCollection : SpellCheckerIntervalCollection<UncheckedInterval> {
		public UncheckedIntervalCollection(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public virtual UncheckedInterval[] ObtainIntervals(DocumentModelPosition start, DocumentModelPosition end) {
			int startIndex = BinarySearch(start.LogPosition);
			if (startIndex < 0)
				startIndex = ~startIndex;
			else if (SplitInterval(startIndex, start))
				startIndex++;
			int endIndex = BinarySearch(Algorithms.Max(end.LogPosition - 1, DocumentLogPosition.Zero));
			if (endIndex < 0)
				endIndex = (~endIndex) - 1;
			else
				SplitInterval(endIndex, end);
			if (startIndex > endIndex)
				return null;
			return InnerList.GetRange(startIndex, endIndex - startIndex + 1).ToArray();
		}
		protected internal virtual bool SplitInterval(int index, DocumentModelPosition position) {
			UncheckedInterval oldInterval = InnerList[index];
			if (oldInterval.Start >= position.LogPosition || position.LogPosition >= oldInterval.End)
				return false;
			UncheckedInterval newInterval = new UncheckedInterval(PieceTable);
			newInterval.SetEndCore(oldInterval.Interval.End);
			newInterval.SetStartCore(position.Clone());
			InnerList.Insert(index + 1, newInterval);
			oldInterval.SetEndCore(position.Clone());
			return true;
		}
		public override void Add(UncheckedInterval newInterval) {
			Debug.Assert(Object.ReferenceEquals(PieceTable, newInterval.PieceTable));
			UncheckedInterval[] inervals = ObtainIntervals(newInterval.Interval.Start, newInterval.Interval.End);
			if (inervals != null)
				RemoveRange(inervals);
			int index = BinarySearch(newInterval.Start);
			if (index >= 0)
				Exceptions.ThrowInternalException();
			index = ~index;
			InnerList.Insert(index, newInterval);
			TryMergeIntervals(index);
		}
		protected internal virtual void TryMergeIntervals(int index) {
			if (index > 0 && CanMerge(index - 1, index)) {
				Merge(index - 1, index);
				index--;
			}
			if (index < Count - 1 && CanMerge(index, index + 1))
				Merge(index, index + 1);
		}
		protected bool CanMerge(int firstIndex, int lastIndex) {
			return InnerList[firstIndex].End == InnerList[lastIndex].Start;
		}
		protected void Merge(int firstIndex, int lastIndex) {
			InnerList[firstIndex].SetEndCore(InnerList[lastIndex].Interval.End);
			InnerList.RemoveAt(lastIndex);
		}
		protected internal override void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			for (int i = Count - 1; i >= 0; i--) {
				InnerList[i].OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
				if (InnerList[i].Length == 0)
					InnerList.RemoveAt(i);
			}
		}
	}
	#endregion
	#region MisspelledIntervalCollection
	public class MisspelledIntervalCollection : SpellCheckerIntervalCollection<MisspelledInterval> {
		public MisspelledIntervalCollection(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public virtual void AddIfNotExists(MisspelledInterval newInterval) {
			Debug.Assert(Object.ReferenceEquals(PieceTable, newInterval.PieceTable));
			int index = BinarySearch(newInterval.Start);
			if (index < 0) {
				index = ~index;
				InnerList.Insert(index, newInterval);
			}
			else {
				MisspelledInterval oldInterval = InnerList[index];
				if (oldInterval.Start != newInterval.Start || oldInterval.End != newInterval.End)
					Exceptions.ThrowInternalException();
			}
		}
	}
	#endregion
	#region IgnoredList
	public class IgnoredList : SpellCheckerIntervalCollection<MisspelledInterval> {
		readonly List<string> ignoreAllList;
		public IgnoredList(PieceTable pieceTable)
			: base(pieceTable) {
			this.ignoreAllList = new List<string>();
		}
		protected internal List<string> IgnoreAllList { get { return ignoreAllList; } }
		public virtual bool Contains(DocumentLogPosition start, DocumentLogPosition end, string word) {
			if (Contains(word))
				return true;
			return FindInerval(start, end) != null;
		}
		public virtual bool Contains(string word) {
			return IgnoreAllList.Contains(word);
		}
		public virtual bool Remove(string word) {
			return IgnoreAllList.Remove(word);
		}
		protected internal void Add(DocumentModelPosition start, DocumentModelPosition end) {
			MisspelledInterval interval = new MisspelledInterval(PieceTable, SpellingError.Unknown);
			interval.SetEndCore(end.Clone());
			interval.SetStartCore(start.Clone());
			Add(interval);
		}
		public virtual void Add(string word) {
			IgnoreAllList.Add(word);
		}
		public override void Clear() {
			base.Clear();
			ignoreAllList.Clear();
		}
	}
	#endregion
	#region SpellCheckerManager
	public class SpellCheckerManager : IDocumentModelStructureChangedListener {
		#region Fields
		readonly PieceTable pieceTable;
		readonly MisspelledIntervalCollection misspelledIntervals;
		readonly UncheckedIntervalCollection uncheckedIntervals;
		readonly IgnoredList ignoredIntervals;
		DocumentLogPosition modifiedWordStart;
		DocumentLogPosition modifiedWordEnd;
		bool shouldCheckDocument;
		#endregion
		public SpellCheckerManager(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.misspelledIntervals = new MisspelledIntervalCollection(pieceTable);
			this.uncheckedIntervals = new UncheckedIntervalCollection(pieceTable);
			this.ignoredIntervals = new IgnoredList(PieceTable);
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public MisspelledIntervalCollection MisspelledIntervals { get { return misspelledIntervals; } }
		public UncheckedIntervalCollection UncheckedIntervals { get { return uncheckedIntervals; } }
		public IgnoredList IgnoredList { get { return ignoredIntervals; } }
		internal DocumentLogPosition ModifiedWordStart { get { return modifiedWordStart; } }
		internal DocumentLogPosition ModifiedWordEnd { get { return modifiedWordEnd; } }
		internal bool ShouldCheckDocument { get { return shouldCheckDocument; } set { shouldCheckDocument = value; } }
		#endregion
		protected internal virtual SpellCheckerManager CreateInstance(PieceTable pieceTable) {
			return new SpellCheckerManager(pieceTable);
		}
		protected internal virtual void Clear() {
			this.shouldCheckDocument = true;
			ClearIntervals();
			ClearModifiedWordInfo();
		}
		protected internal virtual void ClearIntervals() {
			MisspelledIntervals.Clear();
			UncheckedIntervals.Clear();
			IgnoredList.Clear();
		}
		protected internal virtual void ClearModifiedWordInfo() {
			this.modifiedWordStart = new DocumentLogPosition(-1);
			this.modifiedWordEnd = new DocumentLogPosition(-1);
			UnhandleSelectionChanged();
		}
		protected internal void ResetModification() {
			if (ModifiedWordEnd > ModifiedWordStart && ModifiedWordEnd <= PieceTable.DocumentEndLogPosition)
				ProcessModifiedWord();
			ClearModifiedWordInfo();
		}
		protected internal virtual void Initialize() {
			InitializeUncheckedInerval();
		}
		protected internal virtual void InitializeUncheckedInerval() {
			UncheckedIntervals.Clear();
			UncheckedInterval interval = new UncheckedInterval(PieceTable);
			interval.Start = PieceTable.DocumentStartLogPosition;
			interval.End = PieceTable.DocumentEndLogPosition;
			UncheckedIntervals.Add(interval);
		}
		public virtual SpellCheckerInterval[] PopUncheckedIntervals(DocumentModelPosition start, DocumentModelPosition end) {
			UncheckedInterval[] intervals = UncheckedIntervals.ObtainIntervals(start, end);
			if (intervals != null)
				UncheckedIntervals.RemoveRange(intervals);
			return intervals;
		}
		public virtual void RemoveMisspelledIntervals(DocumentLogPosition start, DocumentLogPosition end) {
			MisspelledInterval[] intervals = MisspelledIntervals.GetIntervals(start, end);
			if (intervals == null || intervals.Length == 0)
				return;
			if (intervals[0].Start == start && intervals[0].ErrorType == SpellingError.Repeating) {
				int count = intervals.Length;
				for (int i = 1; i < count; i++)
					MisspelledIntervals.Remove(intervals[i]);
			}
			else
				MisspelledIntervals.RemoveRange(intervals);
		}
		public virtual void CreateMisspelledInterval(DocumentModelPosition start, DocumentModelPosition end, SpellingError errorType) {
			MisspelledInterval interval = new MisspelledInterval(PieceTable, errorType);
			interval.SetEndCore(end.Clone());
			interval.SetStartCore(start.Clone());
			MisspelledIntervals.AddIfNotExists(interval);
		}
		protected virtual void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			DocumentModelPosition start = DocumentModelPosition.FromRunStart(PieceTable, newRunIndex);
			DocumentModelPosition end = DocumentModelPosition.FromRunEnd(PieceTable, newRunIndex);
			MisspelledIntervals.Remove(start.LogPosition);
			IgnoredList.Remove(start.LogPosition);
			OnRunInsertedCore(paragraphIndex, newRunIndex, length, historyNotificationId);
			CalculateUncheckedInterval(start, end);
		}
		private void OnRunInsertedCore(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			MisspelledIntervals.OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
			UncheckedIntervals.OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
			IgnoredList.OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		protected internal virtual void CalculateUncheckedInterval(DocumentModelPosition start, DocumentModelPosition end) {
			if (IsRangeVisible(start, end)) {
				CalculateUncheckedIntervalCore(start, end);
				CalculateModifiedWordInfo(end);
			}
			else
				ResetModification();
		}
		void CalculateModifiedWordInfo(DocumentModelPosition end) {
			SpellCheckerWordIterator iterator = new SpellCheckerWordIterator(PieceTable);
			DocumentModelPosition wordStart = iterator.MoveToWordStart(end);
			DocumentModelPosition wordEnd = iterator.MoveToWordEnd(wordStart);
			this.modifiedWordStart = wordStart.LogPosition;
			this.modifiedWordEnd = wordEnd.LogPosition;
			UncheckedInterval[] intervals = UncheckedIntervals.ObtainIntervals(wordStart, wordEnd);
			if (intervals != null) {
				UncheckedIntervals.RemoveRange(intervals);
				HandleSelectionChanged();
			}
		}
		void HandleSelectionChanged() {
			UnhandleSelectionChanged();
			PieceTable.DocumentModel.InnerSelectionChanged += OnSelectionChanged;
			PieceTable.DocumentModel.SelectionReseted += OnSelectionReseted;
		}
		void UnhandleSelectionChanged() {
			PieceTable.DocumentModel.InnerSelectionChanged -= OnSelectionChanged;
			PieceTable.DocumentModel.SelectionReseted -= OnSelectionReseted;
		}
		internal bool IsModifiedWord(DocumentModelPosition start, DocumentModelPosition end) {
			return ModifiedWordStart == start.LogPosition && ModifiedWordEnd == end.LogPosition;
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			DocumentLogPosition caretPosition = PieceTable.DocumentModel.Selection.NormalizedEnd;
			if (PieceTable.DocumentModel.Selection.Length > 0 || (caretPosition < ModifiedWordStart || caretPosition > ModifiedWordEnd)) {
				ProcessModifiedWord();
				ClearModifiedWordInfo();
			}
		}
		void OnSelectionReseted(object sender, EventArgs e) {
			ClearModifiedWordInfo();
		}
		void ProcessModifiedWord() {
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(PieceTable, ModifiedWordStart);
			DocumentModelPosition end = PositionConverter.ToDocumentModelPosition(PieceTable, ModifiedWordEnd);
			if (!IsRangeVisible(start, end))
				return;
			CalculateUncheckedIntervalCore(start, end);
			PieceTable.DocumentModel.ResetSpellCheck(start.RunIndex, end.RunIndex, true);
		}
		bool IsRangeVisible(DocumentModelPosition start, DocumentModelPosition end) {
			IVisibleTextFilter textFilter = PieceTable.VisibleTextFilter;
			RunIndex startRunIndex = start.RunIndex;
			RunIndex endRunIndex = end.RunIndex;
			if (startRunIndex == endRunIndex)
				return textFilter.IsRunVisible(startRunIndex);
			if (end.RunOffset == 0) {
				endRunIndex--;
				if (endRunIndex == startRunIndex)
					return textFilter.IsRunVisible(startRunIndex);
			}
			for (RunIndex index = startRunIndex; index <= endRunIndex; index++) {
				if (textFilter.IsRunVisible(index))
					return true;
			}
			return false;
		}
		protected internal virtual void CalculateUncheckedIntervalCore(DocumentModelPosition start, DocumentModelPosition end) {
			UncheckedInterval uncheckedInterval = new UncheckedInterval(PieceTable);
			uncheckedInterval.SetEndCore(end.Clone());
			uncheckedInterval.SetStartCore(start.Clone());
			SpellCheckerWordIterator iterator = new SpellCheckerWordIterator(PieceTable);
			iterator.MoveToPrevWordStart(uncheckedInterval.Interval.Start);
			iterator.MoveToNextWordEnd(uncheckedInterval.Interval.End);
			UncheckedInterval[] intervals = UncheckedIntervals.ObtainIntervals(uncheckedInterval.Interval.Start, uncheckedInterval.Interval.End);
			if (intervals != null)
				UncheckedIntervals.RemoveRange(intervals);
			UncheckedIntervals.Add(uncheckedInterval);
		}
		protected virtual void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			DocumentModelPosition position = DocumentModelPosition.FromRunStart(PieceTable, runIndex);
			DocumentLogPosition start = position.LogPosition;
			DocumentLogPosition end = start + length;
			MisspelledIntervals.RemoveRange(start, end);
			IgnoredList.RemoveRange(start, end);
			OnRunRemovedCore(paragraphIndex, runIndex, length, historyNotificationId);
			CalculateUncheckedInterval(position, position);
		}
		private void OnRunRemovedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			MisspelledIntervals.OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
			UncheckedIntervals.OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
			IgnoredList.OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
		}
		protected virtual void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			DocumentModelPosition start = DocumentModelPosition.FromRunStart(PieceTable, runIndex);
			start.LogPosition += (PieceTable.Runs[runIndex].Length - deltaRunLength);
			DocumentModelPosition end = DocumentModelPosition.FromRunEnd(PieceTable, runIndex);
			MisspelledIntervals.Remove(start.LogPosition);
			IgnoredList.Remove(start.LogPosition);
			OnRunMergedCore(paragraphIndex, runIndex, deltaRunLength);
			CalculateUncheckedInterval(start, end);
		}
		private void OnRunMergedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			MisspelledIntervals.OnRunMerged(paragraphIndex, runIndex, deltaRunLength);
			UncheckedIntervals.OnRunMerged(paragraphIndex, runIndex, deltaRunLength);
			IgnoredList.OnRunMerged(paragraphIndex, runIndex, deltaRunLength);
		}
		protected virtual void OnParagraphRemoved(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			MisspelledIntervals.OnParagraphRemoved(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			UncheckedIntervals.OnParagraphRemoved(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			IgnoredList.OnParagraphRemoved(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		protected virtual void OnParagraphMerged(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			MisspelledIntervals.OnParagraphMerged(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			UncheckedIntervals.OnParagraphMerged(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			IgnoredList.OnParagraphMerged(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		protected virtual void OnParagraphInserted(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			MisspelledIntervals.OnParagraphInserted(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			UncheckedIntervals.OnParagraphInserted(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			IgnoredList.OnParagraphInserted(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		protected virtual void OnRunSplit(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			MisspelledIntervals.OnRunSplit(paragraphIndex, runIndex, splitOffset);
			UncheckedIntervals.OnRunSplit(paragraphIndex, runIndex, splitOffset);
			IgnoredList.OnRunSplit(paragraphIndex, runIndex, splitOffset);
		}
		protected virtual void OnRunJoined(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			MisspelledIntervals.OnRunJoined(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			UncheckedIntervals.OnRunJoined(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			IgnoredList.OnRunJoined(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		protected virtual void OnRunUnmerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			DocumentModelPosition position = DocumentModelPosition.FromRunStart(PieceTable, runIndex);
			DocumentLogPosition start = position.LogPosition + pieceTable.Runs[runIndex].Length;
			DocumentLogPosition end = start - deltaRunLength;
			MisspelledIntervals.RemoveRange(start, end);
			IgnoredList.RemoveRange(start, end);
			MisspelledIntervals.OnRunUnmerged(paragraphIndex, runIndex, deltaRunLength);
			IgnoredList.OnRunUnmerged(paragraphIndex, runIndex, deltaRunLength);
			UncheckedIntervals.OnRunUnmerged(paragraphIndex, runIndex, deltaRunLength);
			CalculateUncheckedInterval(position, position);
		}
		#region IDocumentModelStructureChangedListener Members
		void IDocumentModelStructureChangedListener.OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(pieceTable, PieceTable))
				return;
			OnParagraphInserted(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(pieceTable, PieceTable))
				return;
			OnParagraphRemoved(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(pieceTable, PieceTable))
				return;
			OnParagraphMerged(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			if (!Object.ReferenceEquals(pieceTable, PieceTable))
				return;
			OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			if (!Object.ReferenceEquals(pieceTable, PieceTable))
				return;
			OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			if (!Object.ReferenceEquals(pieceTable, PieceTable))
				return;
			OnRunSplit(paragraphIndex, runIndex, splitOffset);
		}
		void IDocumentModelStructureChangedListener.OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			if (!Object.ReferenceEquals(pieceTable, PieceTable))
				return;
			OnRunJoined(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (!Object.ReferenceEquals(pieceTable, PieceTable))
				return;
			OnRunMerged(paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (!Object.ReferenceEquals(pieceTable, PieceTable))
				return;
			OnRunUnmerged(paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
		}
		void IDocumentModelStructureChangedListener.OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
		#endregion
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (PieceTable != null && PieceTable.DocumentModel != null)
					UnhandleSelectionChanged();
			}
		}
	}
	#endregion
	#region EmptySpellCheckerManager
	public class EmptySpellCheckerManager : SpellCheckerManager {
		public EmptySpellCheckerManager(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override SpellCheckerManager CreateInstance(PieceTable pieceTable) {
			return new EmptySpellCheckerManager(pieceTable);
		}
		protected internal override void Clear() {
		}
		protected internal override void Initialize() {
		}
		protected override void OnParagraphInserted(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
		}
		protected override void OnParagraphMerged(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
		}
		protected override void OnParagraphRemoved(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
		}
		protected override void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
		}
		protected override void OnRunJoined(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
		}
		protected override void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
		}
		protected override void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
		}
		protected override void OnRunSplit(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
		}
		protected override void OnRunUnmerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
		}
		protected override void Dispose(bool disposing) {
		}
	}
	#endregion
}
