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
using DevExpress.XtraSpellChecker.Parser;
using System.Windows.Input;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using System.Windows.Documents;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraSpellChecker.Native {
	public interface ISpellCheckMSWordTextControlController : ISpellCheckTextControlController {
		event SelectionStartChangedEventHandler FormEditorSelectionStartChanged;
		event TextChangedEventHandler FormEditorTextChanged;
		event KeyEventHandler FormEditorKeyDown;
		bool IsPositionInRange(Position posStart, Position posFinish, Position pos);
		Position CaretPosition { get; }
		Position FormEditorSelectionStart { get; set; }
		Position FormEditorSelectionFinish { get; set; }
		string GetFormEditorText(Position start, Position finish);
	}
	public abstract class SimpleTextControlTextControllerBase : SimpleTextController, ISpellCheckTextControlController, IDisposable {
		Control editControl;
		bool hideSelectionStored;
		protected SimpleTextControlTextControllerBase(Control editControl)
			: base() {
			this.editControl = editControl;
			SaveHideSelection();
		}
		public virtual Control EditControl { get { return editControl; } }
		public void Dispose() {
			RestoreHideSelection();
			ResetReadOnly();
			editControl = null;
		}
		public void HideSelection() {
			HideSelectionCore();
		}
		public void ShowSelection() {
			ShowSelectionCore();
		}
		public void Select(Position start, Position finish) {
			SelectCore(start, finish);
		}
		public void UpdateText() {
			UpdateTextCore();
		}
		public bool IsSelectionVisible() {
			return IsSelectionVisibleCore();
		}
		public void ScrollToCaretPos() {
			ScrollToCaretPosCore();
		}
		public string EditControlText {
			get { return GetEditControlText(); }
			set { SetEditControlText(value); }
		}
		protected virtual void SaveHideSelection() {
			hideSelectionStored = !IsSelectionVisible();
		}
		protected virtual void RestoreHideSelection() {
			if (hideSelectionStored)
				HideSelection();
		}
		public Position SelectionStart {
			get { return GetSelectionStartCore(); }
		}
		public Position SelectionFinish {
			get { return GetSelectionFinishCore(); }
		}
		public bool HasSelection {
			get { return HasSelectionCore(); }
		}
		public bool IsReadOnly { get { return GetIsReadOnly(); } }
		protected virtual void ResetReadOnly() { }
		#region Abstract
		protected abstract void HideSelectionCore();
		protected abstract void ShowSelectionCore();
		protected abstract bool IsSelectionVisibleCore();
		protected abstract void SelectCore(Position start, Position finish);
		protected abstract void UpdateTextCore();
		protected abstract void ScrollToCaretPosCore();
		protected abstract Position GetSelectionStartCore();
		protected abstract Position GetSelectionFinishCore();
		protected abstract bool HasSelectionCore();
		protected abstract void SetEditControlText(string value);
		protected abstract string GetEditControlText();
		protected abstract bool GetIsReadOnly();
		#endregion
	}
	public class SimpleTextBoxTextController : SimpleTextControlTextControllerBase {
		bool actualReadonly = false;
		public SimpleTextBoxTextController(TextBox editControl)
			: base(editControl) {
			base.Text = GetEditControlText();
			actualReadonly = editControl.IsReadOnly;
			editControl.IsReadOnly = true;
		}
		public new TextBox EditControl { get { return base.EditControl as TextBox; } }
		protected override void HideSelectionCore() {
		}
		protected override void ShowSelectionCore() {
		}
		protected override bool IsSelectionVisibleCore() {
			return true;
		}
		protected override void SetEditControlText(string value) {
			EditControl.Text = value;
		}
		protected override string GetEditControlText() {
			return EditControl.Text;
		}
		protected override void UpdateTextCore() {
			base.Text = GetEditControlText();
		}
		protected override void ScrollToCaretPosCore() {
		}
		protected override Position GetSelectionStartCore() {
			return new IntPosition(EditControl.SelectionStart);
		}
		protected override Position GetSelectionFinishCore() {
			return new IntPosition(EditControl.SelectionStart + EditControl.SelectionLength);
		}
		protected override bool HasSelectionCore() {
			return EditControl.SelectionLength > 0;
		}
		protected override void SelectCore(Position start, Position finish) {
			int startPosition = GetIntPositionInstance(start).ActualIntPosition;
			int finishPosition = GetIntPositionInstance(finish).ActualIntPosition;
			EditControl.Select(startPosition, finishPosition - startPosition);
		}
		protected override bool ReplaceWordCore(Position start, Position finish, string word) {
			ReplaceText(start, finish, word);
			OnTextModified();
			return true;
		}
		protected internal virtual void ReplaceText(Position start, Position end, string value) {
			int startIntPos = GetIntPositionInstance(start).ActualIntPosition;
			int endIntPos = GetIntPositionInstance(end).ActualIntPosition;
			string temp = Text.Remove(startIntPos, endIntPos - startIntPos);
			Text = temp.Insert(startIntPos, value);
		}
		protected override bool DeleteWordCore(PositionRange range) {
			CalcPositionsForDelete(range);
			ReplaceText(range.Start, range.Finish, String.Empty);
			OnTextModified();
			return true;
		}
		protected override void OnTextModified() {
			EditControl.Text = Text;
		}
		protected override bool GetIsReadOnly() {
			return actualReadonly;
		}
		protected override void ResetReadOnly() {
			EditControl.IsReadOnly = actualReadonly;
		}
	}
	public class SimpleTextEditTextController : SimpleTextControlTextControllerBase {
		bool actualReadonly = false;
		bool selectAllOnGotFocus;
		public SimpleTextEditTextController(TextEdit editControl)
			: base(editControl) {
			base.Text = GetEditControlText();
			actualReadonly = editControl.IsReadOnly;
			this.selectAllOnGotFocus = editControl.SelectAllOnGotFocus;
			editControl.SelectAllOnGotFocus = false;
		}
		public new TextEdit EditControl { get { return base.EditControl as TextEdit; } }
		protected override void HideSelectionCore() {
		}
		protected override void ShowSelectionCore() {
		}
		protected override bool IsSelectionVisibleCore() {
			return true;
		}
		protected override void SetEditControlText(string value) {
			EditControl.Text = value;
		}
		protected override string GetEditControlText() {
			return EditControl.Text;
		}
		protected override void UpdateTextCore() {
			base.Text = GetEditControlText();
		}
		protected override void ScrollToCaretPosCore() {
		}
		protected override Position GetSelectionStartCore() {
			return new IntPosition(EditControl.SelectionStart);
		}
		protected override Position GetSelectionFinishCore() {
			return new IntPosition(EditControl.SelectionStart + EditControl.SelectionLength);
		}
		protected override bool HasSelectionCore() {
			return EditControl.SelectionLength > 0;
		}
		protected override void SelectCore(Position start, Position finish) {
			int startPosition = GetIntPositionInstance(start).ActualIntPosition;
			int finishPosition = GetIntPositionInstance(finish).ActualIntPosition;
			EditControl.Select(startPosition, finishPosition - startPosition);
		}
		protected override bool ReplaceWordCore(Position start, Position finish, string word) {
			ReplaceText(start, finish, word);
			OnTextModified();
			return true;
		}
		protected internal virtual void ReplaceText(Position start, Position end, string value) {
			int startIntPos = GetIntPositionInstance(start).ActualIntPosition;
			int endIntPos = GetIntPositionInstance(end).ActualIntPosition;
			string temp = Text.Remove(startIntPos, endIntPos - startIntPos);
			Text = temp.Insert(startIntPos, value);
		}
		protected override bool DeleteWordCore(PositionRange range) {
			CalcPositionsForDelete(range);
			ReplaceText(range.Start, range.Finish, String.Empty);
			OnTextModified();
			return true;
		}
		protected override void OnTextModified() {
			EditControl.Text = Text;
		}
		protected override bool GetIsReadOnly() {
			return actualReadonly;
		}
		protected override void ResetReadOnly() {
			EditControl.IsReadOnly = actualReadonly;
			EditControl.SelectAllOnGotFocus = this.selectAllOnGotFocus;
		}
	}
	public class SimpleMemoEditTextController : SimpleTextEditTextController {
		public SimpleMemoEditTextController(MemoEdit editControl) : base(editControl) { }
		public new MemoEdit EditControl { get { return base.EditControl as MemoEdit; } }
		protected override bool ReplaceWordCore(Position start, Position finish, string word) {
			int startInt = GetIntPositionInstance(start).ActualIntPosition;
			int finishInt = GetIntPositionInstance(finish).ActualIntPosition;
			Text = Text.Remove(startInt, finishInt - startInt).Insert(startInt, word);
			OnTextModified();
			return true;
		}
		protected override bool DeleteWordCore(PositionRange range) {
			CalcPositionsForDelete(range);
			int startInt = GetIntPositionInstance(range.Start).ActualIntPosition;
			int finishInt = GetIntPositionInstance(range.Finish).ActualIntPosition;
			Text = Text.Remove(startInt, finishInt - startInt);
			OnTextModified();
			return true;
		}
	}
	public class RichTextBoxHelper {
		readonly RichTextBox editControl;
		public RichTextBoxHelper(RichTextBox richTextBox) {
			this.editControl = richTextBox;
		}
		protected RichTextBox EditControl { get { return editControl; } }
		public virtual TextPointer GetPointerFromCharacterIndex(TextPointer position, int index, LogicalDirection direction) {
			return GetPointerFromCharacterIndexCore(position, index, direction);
		}
		TextPointer GetPointerFromCharacterIndexCore(TextPointer position, int index, LogicalDirection direction) {
			int offset = 0;
			while (position != null) {
				TextPointerContext context = position.GetPointerContext(direction);
				if (context == TextPointerContext.Text) {
					int runLenth = position.GetTextRunLength(direction);
					if ((index == offset) || (index < offset + runLenth)) {
						position = position.GetPositionAtOffset(index - offset);
						break;
					}
					offset += runLenth;
				}
				if (IsParagraphEnd(position, direction)) {
					offset++;
					if (index < offset)
						return position;
				}
				if (IsNextElementIsUI(position, direction)) {
					offset++;
					if (index < offset)
						return position;
				}
				position = position.GetNextContextPosition(direction);
			}
			return position;
		}
		internal virtual int GetOffset(TextPointer start, TextPointer finish) {
			int result = 0;
			TextPointer pos = start;
			for (; pos.CompareTo(finish) < 0; pos = pos.GetNextContextPosition(LogicalDirection.Forward)) {
				TextPointerContext context = pos.GetPointerContext(LogicalDirection.Forward);
				if (context == TextPointerContext.Text) {
					Run parent = pos.Parent as Run;
					TextRange range = new TextRange(parent.ContentStart, parent.ContentEnd);
					if (range.Contains(finish)) {
						result += GetOffsetCore(pos, finish);
						break;
					}
					else
						result += pos.GetTextRunLength(LogicalDirection.Forward);
				}
				if (IsParagraphEnd(pos, LogicalDirection.Forward) || IsUIElement(pos, LogicalDirection.Forward))
					result++;
			}
			return result;
		}
		int GetOffsetCore(TextPointer start, TextPointer finish) {
			TextElement element = start.GetAdjacentElement(LogicalDirection.Backward) as TextElement;
			if (element == null)
				return start.GetOffsetToPosition(finish);
			TextPointer end = element.ElementEnd.GetPositionAtOffset(-1);
			return end.CompareTo(finish) < 0 ? start.GetOffsetToPosition(end) : start.GetOffsetToPosition(finish);
		}
		public string GetText() {
			TextPointer position = EditControl.Document.ContentStart;
			StringBuilder result = new StringBuilder();
			while (position != null && position.HasValidLayout) {
				if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
					result.Append(position.GetTextInRun(LogicalDirection.Forward));
				else {
					if (IsParagraphEnd(position, LogicalDirection.Forward))
						result.Append("\n");
					if (IsUIElement(position, LogicalDirection.Forward))
						result.Append(" ");
				}
				position = position.GetNextContextPosition(LogicalDirection.Forward);
			}
			return result.ToString();
		}
		bool IsUIElement(TextPointer position, LogicalDirection direction) {
			return position.GetPointerContext(direction) == TextPointerContext.ElementEnd && (position.Parent is InlineUIContainer || position.Parent is BlockUIContainer);
		}
		bool IsNextElementIsUI(TextPointer position, LogicalDirection direction) {
			position = position.GetNextContextPosition(direction);
			return position != null ? IsUIElement(position, direction) : false;
		}
		bool IsParagraphEnd(TextPointer position, LogicalDirection direction) {
			return position.GetPointerContext(direction) == TextPointerContext.ElementEnd && (position.Parent is Paragraph || position.Parent is LineBreak);
		}
	}
#if !SL
	public class RichTextBoxController : SimpleTextControlTextControllerBase {
		RichTextBoxHelper helper;
		public RichTextBoxController(RichTextBox richTextBox)
			: base(richTextBox) {
			this.helper = new RichTextBoxHelper(richTextBox);
			Text = GetEditControlText();
		}
		public new RichTextBox EditControl { get { return (RichTextBox)base.EditControl; } }
		RichTextBoxHelper Helper { get { return helper; } }
		protected override void HideSelectionCore() {
		}
		protected override void ShowSelectionCore() {
		}
		protected override bool IsSelectionVisibleCore() {
			return true;
		}
		protected override void SelectCore(Position start, Position finish) {
			int startPosition = GetIntPositionInstance(start).ActualIntPosition;
			int finishPosition = GetIntPositionInstance(finish).ActualIntPosition;
			TextPointer pointer = Helper.GetPointerFromCharacterIndex(EditControl.Document.ContentStart, startPosition, LogicalDirection.Forward);
			EditControl.Selection.Select(pointer, Helper.GetPointerFromCharacterIndex(pointer, finishPosition - startPosition, LogicalDirection.Forward));
		}
		protected override void UpdateTextCore() {
			Text = GetEditControlText();
		}
		protected override void ScrollToCaretPosCore() {
		}
		protected override Position GetSelectionStartCore() {
			TextSelection selection = EditControl.Selection;
			int result = Helper.GetOffset(EditControl.Document.ContentStart, selection.Start);
			return new IntPosition(result);
		}
		protected override Position GetSelectionFinishCore() {
			TextSelection selection = EditControl.Selection;
			int result = Helper.GetOffset(EditControl.Document.ContentStart, selection.End);
			return new IntPosition(result);
		}
		protected override bool HasSelectionCore() {
			return !EditControl.Selection.IsEmpty;
		}
		protected override void SetEditControlText(string value) {
			System.Diagnostics.Debug.Assert(false);
		}
		protected override string GetEditControlText() {
			return Helper.GetText();
		}
		protected override bool GetIsReadOnly() {
			return EditControl.IsReadOnly;
		}
		protected override bool ReplaceWordCore(Position start, Position finish, string word) {
			int startPos = GetIntPositionInstance(start).ActualIntPosition;
			int endPos = GetIntPositionInstance(finish).ActualIntPosition;
			TextPointer p1 = Helper.GetPointerFromCharacterIndex(EditControl.Document.ContentStart, startPos, LogicalDirection.Forward);
			TextPointer p2 = Helper.GetPointerFromCharacterIndex(p1, endPos - startPos, LogicalDirection.Forward);
			TextRange textRange = new TextRange(p1, p2);
			textRange.Text = word;
			Text = Text.Remove(startPos, endPos - startPos).Insert(startPos, word);
			OnTextModified();
			return true;
		}
		protected override bool DeleteWordCore(PositionRange range) {
			CalcPositionsForDelete(range);
			ReplaceWordCore(range.Start, range.Finish, String.Empty);
			return true;
		}
		protected override void CalcPositionsForDelete(PositionRange range) {
			int startPos = GetIntPositionInstance(range.Start).ActualIntPosition;
			int endPos = GetIntPositionInstance(range.Finish).ActualIntPosition;
			if (startPos > 0 && Text[startPos - 1] == ' ') {
				range.Start = new IntPosition(startPos - 1);
				return;
			}
			if (endPos < Text.Length - 1 && Text[endPos] == ' ') {
				range.Finish = new IntPosition(endPos + 1);
				return;
			}
		}
	}
#else
	public class RichTextBoxController : SimpleTextControlTextControllerBase {
		public RichTextBoxController(RichTextBox richTextBox)
			: base(richTextBox) {
			Text = GetEditControlText();
		}
		public new RichTextBox EditControl { get { return (RichTextBox)base.EditControl; } }
		protected override void HideSelectionCore() {
		}
		protected override void ShowSelectionCore() {
		}
		protected override bool IsSelectionVisibleCore() {
			return true;
		}
		protected override void SelectCore(Position start, Position finish) {
			int startPosition = GetIntPositionInstance(start).ActualIntPosition;
			int finishPosition = GetIntPositionInstance(finish).ActualIntPosition;
			int length = finishPosition - startPosition;
			TextPointer startPointer = GetPointerForward(EditControl.ContentStart, startPosition);
			EditControl.Selection.Select(startPointer, GetPointerForward(startPointer, length));
		}
		TextPointer GetPointerForward(TextPointer position, int index) {
			return GetPointer(position, index, LogicalDirection.Forward);
		}
		TextPointer GetPointer(TextPointer start, int index, LogicalDirection direction) {
			TextPointer position = start;
			int offset = 0;
			while (position != null) {
				Run run = position.Parent as Run;
				if (run != null) {
					int runLenth = run.Text.Length;
					if ((index == offset) || (index < offset + runLenth))
						return position.GetPositionAtOffset(index - offset, direction);
					offset += runLenth;
					position = run.ElementEnd;
				}
				if (IsParagraphEnd(position, direction)) {
					offset++;
					if (index < offset)
						return position;
				}
				position = position.GetNextInsertionPosition(direction);
			}
			return EditControl.ContentEnd;
		}
		protected override void UpdateTextCore() {
			Text = GetEditControlText();
		}
		protected override void ScrollToCaretPosCore() {
		}
		protected override Position GetSelectionStartCore() {
			TextSelection selection = EditControl.Selection;
			int result = GetPositionOffset(selection.Start, LogicalDirection.Forward);
			return new IntPosition(result);
		}
		protected override Position GetSelectionFinishCore() {
			TextSelection selection = EditControl.Selection;
			int result = GetPositionOffset(selection.End, LogicalDirection.Forward);
			return new IntPosition(result);
		}
		int GetPositionOffset(TextPointer position, LogicalDirection direction) {
			return GetPositionOffsetCore(EditControl.ContentStart, position, direction);
		}
		int GetPositionOffsetCore(TextPointer start, TextPointer position, LogicalDirection direction) {
			int result = 0;
			TextPointer pos = start;
			for (; pos.CompareTo(position) <= 0; pos = pos.GetNextInsertionPosition(direction)) {
				Run parent = pos.Parent as Run;
				if (parent != null) {
					if (parent.ContentStart.CompareTo(position) <= 0 && parent.ContentEnd.CompareTo(position) >= 0) {
						result += GetLength(parent.ContentStart, position);
						break;
					}
					else {
						result += parent.Text.Length;
						pos = parent.ElementEnd;
					}
				}
				if (IsParagraphEnd(pos, direction))
					result++;
			}
			return result;
		}
		int GetLength(TextPointer position1, TextPointer position2) {
			int result = -1;
			LogicalDirection direction = CalcLogicalDirection(position1, position2);
			for (TextPointer i = position1; i.CompareTo(position2) <= 0; i = i.GetNextInsertionPosition(direction))
				if (i.Parent is Run)
					result++;
			return result;
		}
		protected override bool HasSelectionCore() {
			return EditControl.Selection.Start.CompareTo(EditControl.Selection.End) != 0;
		}
		protected override void SetEditControlText(string value) {
			System.Diagnostics.Debug.Assert(false);
		}
		protected override string GetEditControlText() {
			return GetDocumentPlainText(LogicalDirection.Forward);
		}
		protected internal virtual string GetDocumentPlainText(LogicalDirection direction) {
			TextPointer position = EditControl.ContentStart;
			StringBuilder result = new StringBuilder();
			while (position != null) {
				Run run = position.Parent as Run;
				if (run != null) {
					result.Append(run.Text);
					position = run.ElementEnd;
				}
				if (IsParagraphEnd(position, direction))
					result.Append("\n");
				position = position.GetNextInsertionPosition(direction);
			}
			return result.ToString();
		}
		bool IsParagraphEnd(TextPointer position, LogicalDirection direction) {
			Paragraph paragraph = position.Parent as Paragraph;
			return paragraph != null && paragraph.ContentEnd.CompareTo(position) == 0;
		}
		protected override bool GetIsReadOnly() {
			return EditControl.IsReadOnly;
		}
		protected override bool ReplaceWordCore(Position start, Position finish, string word) {
			int startPos = GetIntPositionInstance(start).ActualIntPosition;
			int endPos = GetIntPositionInstance(finish).ActualIntPosition;
			int length = endPos - startPos;
			TextPointer startPointer = GetPointerForward(EditControl.ContentStart, startPos);
			ReplaceWordCore(startPointer, GetPointerForward(startPointer, length), word);
			Text = Text.Remove(startPos, endPos - startPos).Insert(startPos, word);
			OnTextModified();
			return true;
		}
		void ReplaceWordCore(TextPointer start, TextPointer end, string word) {
			LogicalDirection direction = CalcLogicalDirection(start, end);
			int index = 0;
			for (TextPointer i = start; i.CompareTo(end) <= 0; ) {
				Run run = i.Parent as Run;
				if (run != null) {
					int startIndex = GetLength(run.ContentStart, i);
					int count = Math.Min(run.Text.Length - startIndex, word.Length - index);
					if (end.CompareTo(run.ElementEnd) <= 0) {
						int length = GetLength(i, end);
						run.Text = run.Text.Remove(startIndex, length).Insert(startIndex, word.Substring(index));
						break;
					}
					else {
						int length = GetLength(i, run.ContentEnd);
						run.Text = run.Text.Remove(startIndex, length).Insert(startIndex, word.Substring(index, count));
					}
					index += count;
					i = run.ElementEnd;
				}
				i = i.GetNextInsertionPosition(direction);
			}
		}
		LogicalDirection CalcLogicalDirection(TextPointer start, TextPointer end) {
			return start.CompareTo(end) <= 0 ? LogicalDirection.Forward : LogicalDirection.Backward;
		}
		protected override bool DeleteWordCore(PositionRange range) {
			CalcPositionsForDelete(range);
			ReplaceWordCore(range.Start, range.Finish, String.Empty);
			return true;
		}
		protected override void CalcPositionsForDelete(PositionRange range) {
			int startPos = GetIntPositionInstance(range.Start).ActualIntPosition;
			int endPos = GetIntPositionInstance(range.Finish).ActualIntPosition;
			if (startPos > 0 && Text[startPos - 1] == ' ') {
				range.Start = new IntPosition(startPos - 1);
				return;
			}
			if (endPos < Text.Length - 1 && Text[endPos] == ' ') {
				range.Finish = new IntPosition(endPos + 1);
				return;
			}
		}
	}
#endif
	public class MSWordFormEditorHelper {
		#region Fields
		readonly TextEdit editControl;
		int selectionStart = -1;
#if SL
		bool isFirstTimeInitialized;
		Position cachedEditControlSelectionStart;
		Position cachedEditControlSelectionFinish;
		bool useCachedValues;
#endif
		#endregion
		public MSWordFormEditorHelper(TextEdit editControl) {
			this.editControl = editControl;
			SubscribeToEditControlEvents();
		}
		#region Properties
		public TextEdit EditControl { get { return editControl; } }
		#region EditControlSelectionStart
		public Position EditControlSelectionStart {
			get {
#if !SL
				return new IntPosition(EditControl.SelectionStart);
#else
				return useCachedValues ? cachedEditControlSelectionStart : new IntPosition(EditControl.SelectionStart);
#endif
			}
			set {
#if SL
				isFirstTimeInitialized = true;
				cachedEditControlSelectionStart = value;
#endif
				EditControl.SelectionStart = value.ToInt();
			}
		}
		#endregion
		#region EditControlSelectionFinish
		public Position EditControlSelectionFinish {
			get {
#if !SL
				return new IntPosition(EditControl.SelectionStart + EditControl.SelectionLength);
#else
				return useCachedValues ? cachedEditControlSelectionFinish : new IntPosition(EditControl.SelectionStart + EditControl.SelectionLength);
#endif
			}
			set {
#if SL
				cachedEditControlSelectionFinish = value;
				isFirstTimeInitialized = true;
#endif
				EditControl.SelectionLength = (value - EditControlSelectionStart).ToInt();
			}
		}
		#endregion
		#endregion
		#region Events
		void EditControl_KeyUp(object sender, KeyEventArgs e) {
			if (IsArrowKey(e.Key) && !e.Handled && selectionStart != EditControl.SelectionStart)
				RaiseEditControlSelectionStartChanged(new SelectionStartChangedEventArgs(new IntPosition(EditControl.SelectionStart), SelectionChangeReason.Keyboard, e.Key));
		}
		void EditControl_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			RaiseEditControlTextChanged(new TextChangedEventArgs((string)e.NewValue));
		}
		void OnEditControlKeyDown(object sender, KeyEventArgs e) {
#if SL
			useCachedValues = e.Key == Key.Back || e.Key == Key.Delete;
			if (!isFirstTimeInitialized && useCachedValues) {
				int selectionStart = EditControl.SelectionStart;
				if (e.Key == Key.Back)
					selectionStart = EditControl.SelectionStart + 1;
				cachedEditControlSelectionStart = new IntPosition(selectionStart);
				cachedEditControlSelectionFinish = new IntPosition(selectionStart + EditControl.SelectionLength);
			}
#endif
			RaiseEditControlKeyDown(e);
#if SL
			isFirstTimeInitialized = false;
#endif
		}
		#region EditControlSelectionStartChanged
		SelectionStartChangedEventHandler onEditControlSelectionStartChanged;
		public event SelectionStartChangedEventHandler EditControlSelectionStartChanged {
			add { onEditControlSelectionStartChanged += value; }
			remove { onEditControlSelectionStartChanged -= value; }
		}
		void RaiseEditControlSelectionStartChanged(SelectionStartChangedEventArgs e) {
			selectionStart = e.SelectionStart.ToInt();
			if (onEditControlSelectionStartChanged != null)
				onEditControlSelectionStartChanged(this, e);
		}
		#endregion
		#region EditControlTextChanged
		TextChangedEventHandler onEditControlTextChanged;
		public event TextChangedEventHandler EditControlTextChanged {
			add { onEditControlTextChanged += value; }
			remove { onEditControlTextChanged -= value; }
		}
		void RaiseEditControlTextChanged(TextChangedEventArgs e) {
			if (onEditControlTextChanged != null)
				onEditControlTextChanged(this, e);
		}
		#endregion
		#region EditControlKeyDown
		KeyEventHandler onEditControlKeyDown;
		public event KeyEventHandler EditControlKeyDown {
			add { onEditControlKeyDown += value; }
			remove { onEditControlKeyDown -= value; }
		}
		void RaiseEditControlKeyDown(KeyEventArgs e) {
			if (onEditControlKeyDown != null)
				onEditControlKeyDown(this, e);
		}
		#endregion
		#endregion
		bool IsArrowKey(Key key) {
			return key == Key.Left || key == Key.Right || key == Key.Up || key == Key.Down;
		}
		void SubscribeToEditControlEvents() {
			EditControl.EditValueChanged += EditControl_EditValueChanged;
			EditControl.KeyUp += EditControl_KeyUp;
#if !SL
			EditControl.PreviewKeyDown += OnEditControlKeyDown;
#else
			EditControl.AddHandler(TextEdit.KeyDownEvent, new KeyEventHandler(OnEditControlKeyDown), true);
#endif
		}
	}
	#region SimpleTextEditMSWordTextController
	public class SimpleTextEditMSWordTextController : ISpellCheckMSWordTextControlController {
		TextEdit editControl;
		ISpellCheckTextControlController originalController;
		MSWordFormEditorHelper editorHelper;
		public SimpleTextEditMSWordTextController(TextEdit editControl, ISpellCheckTextControlController originalController) {
			Guard.ArgumentNotNull(editControl, "editControl");
			Guard.ArgumentNotNull(originalController, "originalController");
			this.editControl = editControl;
			this.originalController = originalController;
			this.editorHelper = new MSWordFormEditorHelper(this.editControl);
			SubscribeToEditControlEvents();
		}
		#region Properties
		public TextEdit EditControl { get { return editControl; } }
		public ISpellCheckTextControlController OriginalController { get { return originalController; } }
		public string EditControlText { get { return EditControl.Text; } set { EditControl.Text = value; } }
		public string Text { get { return OriginalController.Text; } set { OriginalController.Text = value; } }
		public char this[Position position] { get { return OriginalController[position]; } }
		public Position CaretPosition { get { return new IntPosition(EditControl.SelectionStart); } }
		public Position SelectionFinish { get { return OriginalController.SelectionFinish; } }
		public Position SelectionStart { get { return OriginalController.SelectionStart; } }
		public bool HasSelection { get { return OriginalController.HasSelection; } }
		public bool IsReadOnly { get { return OriginalController.IsReadOnly; } }
		public Position FormEditorSelectionStart { get { return editorHelper.EditControlSelectionStart; } set { editorHelper.EditControlSelectionStart = value; } }
		public Position FormEditorSelectionFinish { get { return editorHelper.EditControlSelectionFinish; } set { editorHelper.EditControlSelectionFinish = value; } }
		#endregion
		#region Events
		protected virtual void SubscribeToEditControlEvents() {
			editorHelper.EditControlTextChanged += OnEditControlTextChanged;
			editorHelper.EditControlSelectionStartChanged += OnEditControlSelectionStartChanged;
			editorHelper.EditControlKeyDown += OnEditControlKeyDown;
		}
		protected virtual void UnSubscribeFromEditControlEvents() {
			editorHelper.EditControlTextChanged -= OnEditControlTextChanged;
			editorHelper.EditControlSelectionStartChanged -= OnEditControlSelectionStartChanged;
			editorHelper.EditControlKeyDown -= OnEditControlKeyDown;
		}
		void OnEditControlTextChanged(object sender, TextChangedEventArgs e) {
			OnFormEditorTextChanged(e);
		}
		void OnEditControlSelectionStartChanged(object sender, SelectionStartChangedEventArgs e) {
			OnFormEditorSelectionStartChanged(e);
		}
		void OnEditControlKeyDown(object sender, KeyEventArgs e) {
			OnFormEditorKeyDown(e);
		}
		#region FormEditorSelectionStartChanged
		SelectionStartChangedEventHandler onFormEditorSelectionStartChanged;
		public event SelectionStartChangedEventHandler FormEditorSelectionStartChanged { add { onFormEditorSelectionStartChanged += value; } remove { onFormEditorSelectionStartChanged -= value; } }
		protected internal virtual void OnFormEditorSelectionStartChanged(SelectionStartChangedEventArgs e) {
			RaiseFormEditorSelectionStartChanged(e);
		}
		protected internal virtual void RaiseFormEditorSelectionStartChanged(SelectionStartChangedEventArgs e) {
			if (onFormEditorSelectionStartChanged != null)
				onFormEditorSelectionStartChanged(this, e);
		}
		#endregion
		#region FormEditorTextChanged
		TextChangedEventHandler onFormEditorTextChanged;
		public event TextChangedEventHandler FormEditorTextChanged { add { onFormEditorTextChanged += value; } remove { onFormEditorTextChanged -= value; } }
		protected internal virtual void OnFormEditorTextChanged(TextChangedEventArgs e) {
			RaiseFormEditorTextChanged(e);
		}
		protected internal virtual void RaiseFormEditorTextChanged(TextChangedEventArgs e) {
			if (onFormEditorTextChanged != null)
				onFormEditorTextChanged(this, e);
		}
		#endregion
		#region FormEditorKeyDown
		KeyEventHandler onFormEditorKeyDown;
		public event KeyEventHandler FormEditorKeyDown { add { onFormEditorKeyDown += value; } remove { onFormEditorKeyDown -= value; } }
		protected internal virtual void OnFormEditorKeyDown(KeyEventArgs e) {
			RaiseFormEditorKeyDown(e);
		}
		protected internal virtual void RaiseFormEditorKeyDown(KeyEventArgs e) {
			if (onFormEditorKeyDown != null)
				onFormEditorKeyDown(this, e);
		}
		#endregion
		#endregion
		public string GetFormEditorText(Position start, Position finish) {
			int startIndex = start.ToInt();
			int length = (finish - start).ToInt();
			return EditControl.Text.Substring(startIndex, length);
		}
		public bool IsPositionInRange(Position posStart, Position posFinish, Position pos) {
			return Position.IsLessOrEqual(pos, posFinish) && Position.IsGreaterOrEqual(pos, posStart);
		}
		public void ScrollToCaretPos() {
			OriginalController.ScrollToCaretPos();
		}
		public void HideSelection() {
			OriginalController.HideSelection();
		}
		public bool IsSelectionVisible() {
			return OriginalController.IsSelectionVisible();
		}
		public void ShowSelection() {
			OriginalController.ShowSelection();
		}
		public bool CanDoNextStep(Position position) {
			return OriginalController.CanDoNextStep(position);
		}
		public void UpdateText() {
			OriginalController.UpdateText();
		}
		public Position GetNextPosition(Position pos) {
			return OriginalController.GetNextPosition(pos);
		}
		public Position GetPrevPosition(Position pos) {
			return OriginalController.GetPrevPosition(pos);
		}
		public string GetPreviousWord(Position pos) {
			return OriginalController.GetPreviousWord(pos);
		}
		public Position GetSentenceEndPosition(Position pos) {
			return OriginalController.GetSentenceEndPosition(pos);
		}
		public Position GetSentenceStartPosition(Position pos) {
			return OriginalController.GetSentenceStartPosition(pos);
		}
		public Position GetTextLength(string text) {
			return OriginalController.GetTextLength(text);
		}
		public string GetWord(Position start, Position finish) {
			return OriginalController.GetWord(start, finish);
		}
		public Position GetWordStartPosition(Position pos) {
			return OriginalController.GetWordStartPosition(pos);
		}
		public bool HasLetters(Position start, Position finish) {
			return OriginalController.HasLetters(start, finish);
		}
		public bool DeleteWord(ref Position start, ref Position finish) {
			if (!OriginalController.DeleteWord(ref start, ref finish))
				return false;
			UpdateEditControlText(start, finish);
			return true;
		}
		public void Select(Position start, Position finish) {
			OriginalController.Select(start, finish);
		}
		public bool ReplaceWord(Position start, Position finish, string word) {
			if (!OriginalController.ReplaceWord(start, finish, word))
				return false;
			UpdateEditControlText(start, finish);
			return true;
		}
		void UpdateEditControlText(Position start, Position finish) {
			Position sentenceStart = GetSentenceStartPosition(start);
			Position sentenceEnd = GetSentenceEndPosition(finish);
			EditControlText = GetWord(sentenceStart, sentenceEnd);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.editControl != null) {
					UnSubscribeFromEditControlEvents();
					this.editControl = null;
				}
				if (this.originalController != null) {
					this.originalController.Dispose();
					this.originalController = null;
				}
			}
		}
	}
	#endregion
}
