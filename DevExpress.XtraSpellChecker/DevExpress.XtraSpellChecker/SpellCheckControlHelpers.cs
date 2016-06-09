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
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.XtraSpellChecker.Controls;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.XtraSpellChecker.Parser;
using System.Globalization;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraSpellChecker.Native {
	public interface ISpellCheckMSWordTextControlController : ISpellCheckTextControlController {
		event SelectionStartChangedEventHandler MemoSelectionStartChanged;
		event TextChangedEventHandler MemoTextChanged;
		event KeyEventHandler MemoKeyDown;
		bool IsPositionInRange(Position posStart, Position posFinish, Position pos);
		Position CaretPosition { get; }
		Position FormEditorSelectionStart { get; }
		Position FormEditorSelectionFinish { get; }
		string GetFormEditorText(Position start, Position finish);
	}
	public abstract class SimpleTextControlTextControllerBase : SimpleTextController, ISpellCheckTextControlController, IDisposable {
		Control editControl;
		bool hideSelectionStored;
		protected SimpleTextControlTextControllerBase(Control editControl)
			: base() {
			this.editControl = editControl;
			SaveHideSelection();
			SubscribeToEvents();
		}
		public virtual Control EditControl { get { return editControl; } }
		public void Dispose() {
			UnsubscribeFromEvents();
			RestoreHideSelection();
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
		public void Focus() {
			EditControl.Focus();
		}
		protected virtual void SubscribeToEvents() {
			EditControl.TextChanged += OnTextChanged;
		}
		protected virtual void UnsubscribeFromEvents() {
			EditControl.TextChanged -= OnTextChanged;
		}
		void OnTextChanged(object sender, EventArgs e) {
			UpdateTextCore();
		}
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
	public class SimpleTextBoxBaseTextController : SimpleTextControlTextControllerBase {
		public SimpleTextBoxBaseTextController(TextBoxBase editControl)
			: base(editControl) {
			UpdateTextCore();
		}
		public new TextBoxBase EditControl { get { return base.EditControl as TextBoxBase; } }
		protected override void HideSelectionCore() {
			EditControl.HideSelection = true;
		}
		protected override void ShowSelectionCore() {
			EditControl.HideSelection = false;
		}
		protected override bool IsSelectionVisibleCore() {
			return !EditControl.HideSelection;
		}
		protected override void SetEditControlText(string value) {
			EditControl.Text = value;
		}
		protected override string GetEditControlText() {
			return EditControl.Text;
		}
		protected override void SelectCore(Position start, Position finish) {
			IntPosition startPosition = GetIntPositionInstance(start);
			IntPosition finishPosition = GetIntPositionInstance(finish);
			EditControl.SelectionStart = startPosition.ActualIntPosition;
			EditControl.SelectionLength = finishPosition.ActualIntPosition - startPosition.ActualIntPosition;
		}
		protected override void UpdateTextCore() {
			Text = EditControlText;
		}
		protected override void ScrollToCaretPosCore() {
			EditControl.ScrollToCaret();
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
		protected override bool ReplaceWordCore(Position start, Position finish, string word) {
			Select(start, CorrectFinishPositionsForReplace(finish, word));
			EditControl.SelectedText = word;
			return true;
		}
		protected override bool DeleteWordCore(PositionRange range) {
			CalcPositionsForDelete(range);
			Select(range.Start, range.Finish);
			EditControl.SelectedText = string.Empty;
			return true;
		}
		protected override bool GetIsReadOnly() {
			return EditControl.ReadOnly;
		}
	}
	public class SimpleTextBoxTextController : SimpleTextBoxBaseTextController {
		public SimpleTextBoxTextController(TextBoxBase editControl) : base(editControl) { }
		public new TextBox EditControl { get { return base.EditControl as TextBox; } }
	}
	public class RichTextBoxTextController : DevExpress.XtraSpellChecker.Native.SimpleTextBoxBaseTextController {
		public RichTextBoxTextController(RichTextBox editControl) : base(editControl) { }
		public new RichTextBox EditControl { get { return base.EditControl as RichTextBox; } }
	}
	public class SimpleTextEditTextController : SimpleTextControlTextControllerBase {
		public SimpleTextEditTextController(TextEdit editControl)
			: base(editControl) {
			UpdateTextCore();
		}
		public new TextEdit EditControl { get { return base.EditControl as TextEdit; } }
		protected override void SetEditControlText(string value) {
			EditControl.Text = value;
		}
		protected override string GetEditControlText() {
			return EditControl.Text;
		}
		protected override void HideSelectionCore() {
			EditControl.Properties.HideSelection = true;
		}
		protected override void ShowSelectionCore() {
			EditControl.Properties.HideSelection = false;
		}
		protected override bool IsSelectionVisibleCore() {
			return !EditControl.Properties.HideSelection;
		}
		protected override void UpdateTextCore() {
			Text = EditControlText;
		}
		protected override void SelectCore(Position start, Position finish) {
			IntPosition startPosition = GetIntPositionInstance(start);
			IntPosition finishPosition = GetIntPositionInstance(finish);
			EditControl.SelectionStart = startPosition.ActualIntPosition;
			EditControl.SelectionLength = finishPosition.ActualIntPosition - startPosition.ActualIntPosition;
		}
		protected override bool ReplaceWordCore(Position start, Position finish, string word) {
			Select(start, CorrectFinishPositionsForReplace(finish, word));
			EditControl.SelectedText = word;
			return true;
		}
		protected override bool DeleteWordCore(PositionRange range) {
			CalcPositionsForDelete(range);
			Select(range.Start, range.Finish);
			EditControl.SelectedText = string.Empty;
			return true;
		}
		protected override void ScrollToCaretPosCore() {
			EditControl.ScrollToCaret();
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
		protected override bool GetIsReadOnly() {
			return EditControl.Properties.ReadOnly;
		}
	}
	public class SimpleTextEditMSWordTextController : ISpellCheckMSWordTextControlController {
		CustomSpellCheckMemoEdit editControl;
		ISpellCheckTextControlController originalController;
		public SimpleTextEditMSWordTextController(CustomSpellCheckMemoEdit editControl, ISpellCheckTextControlController originalController) {
			Guard.ArgumentNotNull(editControl, "editControl");
			Guard.ArgumentNotNull(originalController, "originalController");
			this.editControl = editControl;
			this.originalController = originalController;
			SubscribeToEditControlEvents();
		}
		#region Properties
		public CustomSpellCheckMemoEdit EditControl { get { return editControl; } }
		public ISpellCheckTextControlController OriginalController { get { return originalController; } }
		public string EditControlText { get { return EditControl.Text; } set { EditControl.Text = value; } }
		public string Text { get { return OriginalController.Text; } set { OriginalController.Text = value; } }
		public char this[Position position] { get { return OriginalController[position]; } }
		public Position CaretPosition { get { return new IntPosition(EditControl.SelectionStart); } }
		public Position SelectionFinish { get { return OriginalController.SelectionFinish; } }
		public Position SelectionStart { get { return OriginalController.SelectionStart; } }
		public bool HasSelection { get { return OriginalController.HasSelection; } }
		public bool IsReadOnly { get { return OriginalController.IsReadOnly; } }
		public Position FormEditorSelectionStart { get { return new IntPosition(EditControl.SelectionStart); } }
		public Position FormEditorSelectionFinish { get { return new IntPosition(EditControl.SelectionStart + EditControl.SelectionLength); } }
		#endregion
		#region Events
		protected virtual void SubscribeToEditControlEvents() {
			EditControl.EditValueChanging += EditControl_EditValueChanging;
			EditControl.SelectionStartChanged += EditControl_SelectionStartChanged;
			EditControl.KeyDown += EditControl_KeyDown;
		}
		protected virtual void UnSubscribeFromEditControlEvents() {
			EditControl.EditValueChanging -= EditControl_EditValueChanging;
			EditControl.SelectionStartChanged -= EditControl_SelectionStartChanged;
			EditControl.KeyDown -= EditControl_KeyDown;
		}
		void EditControl_EditValueChanging(object sender, ChangingEventArgs e) {
			OnMemoTextChanged(new TextChangedEventArgs(e.NewValue.ToString()));
		}
		void EditControl_SelectionStartChanged(object sender, SelectionStartChangedEventArgs e) {
			OnMemoSelectionStartChanged(e);
		}
		void EditControl_KeyDown(object sender, KeyEventArgs e) {
			OnMemoKeyDown(e);
		}
		#region MemoSelectionStartChanged
		SelectionStartChangedEventHandler memoSelectionStartChanged;
		public event SelectionStartChangedEventHandler MemoSelectionStartChanged { add { memoSelectionStartChanged += value; } remove { memoSelectionStartChanged -= value; } }
		protected internal virtual void OnMemoSelectionStartChanged(SelectionStartChangedEventArgs e) {
			RaiseMemoSelectionStartChanged(e);
		}
		protected internal virtual void RaiseMemoSelectionStartChanged(SelectionStartChangedEventArgs e) {
			if (memoSelectionStartChanged != null)
				memoSelectionStartChanged(this, e);
		}
		#endregion
		#region MemoTextChanged
		TextChangedEventHandler memoTextChanged;
		public event TextChangedEventHandler MemoTextChanged { add { memoTextChanged += value; } remove { memoTextChanged -= value; } }
		protected internal virtual void OnMemoTextChanged(TextChangedEventArgs e) {
			RaiseMemoTextChanged(e);
		}
		protected internal virtual void RaiseMemoTextChanged(TextChangedEventArgs e) {
			if (memoTextChanged != null)
				memoTextChanged(this, e);
		}
		#endregion
		#region MemoKeyDown
		KeyEventHandler memoKeyDown;
		public event KeyEventHandler MemoKeyDown { add { memoKeyDown += value; } remove { memoKeyDown -= value; } }
		protected internal virtual void OnMemoKeyDown(KeyEventArgs e) {
			RaiseMemoKeyDown(e);
		}
		protected internal virtual void RaiseMemoKeyDown(KeyEventArgs e) {
			if (memoKeyDown != null)
				memoKeyDown(this, e);
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
			EditControl.ScrollToCaret();
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
}
