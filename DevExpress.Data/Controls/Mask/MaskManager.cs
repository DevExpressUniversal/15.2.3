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
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils;
#region Mask_Tests
#if DEBUGTEST && !SILVERLIGHT
using NUnit.Framework;
#endif
#endregion
namespace DevExpress.Data.Mask {
	public delegate void MaskChangingEventHandler(object sender, MaskChangingEventArgs e);
	public class MaskChangingEventArgs : CancelEventArgs {
		object oldValue;
		object newValue;
		public MaskChangingEventArgs(object oldValue, object newValue) : this(oldValue, newValue, false) { }
		public MaskChangingEventArgs(object oldValue, object newValue, bool cancel)
			: base(cancel) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public object OldValue { get { return oldValue; } }
		public object NewValue {
			get { return newValue; }
			set { newValue = value; }
		}
	}
	public abstract class MaskManager {
		protected MaskManager() { }
		public abstract string GetCurrentEditText();
		public abstract object GetCurrentEditValue();
		public virtual bool IsEditValueAssignedAsFormattedText { get { return false; } }
		public abstract void SetInitialEditText(string initialEditText);
		public abstract void SetInitialEditValue(object initialEditValue);
		public virtual bool IsMatch { get { return true; } }
		public virtual bool IsFinal { get { return false; } }
		public bool IsSelection { get { return DisplayCursorPosition != DisplaySelectionAnchor; } }
		public abstract string DisplayText { get; }
		public abstract int DisplayCursorPosition { get; }
		public abstract int DisplaySelectionAnchor { get; }
		public int DisplaySelectionStart {
			get {
				return (DisplayCursorPosition < DisplaySelectionAnchor) ? DisplayCursorPosition : DisplaySelectionAnchor;
			}
		}
		public int DisplaySelectionEnd {
			get {
				return (DisplayCursorPosition > DisplaySelectionAnchor) ? DisplayCursorPosition : DisplaySelectionAnchor;
			}
		}
		public int DisplaySelectionLength {
			get {
				return (DisplayCursorPosition > DisplaySelectionAnchor) ? DisplayCursorPosition - DisplaySelectionAnchor : DisplaySelectionAnchor - DisplayCursorPosition;
			}
		}
		public abstract bool Insert(string insertion);
		public abstract bool Delete();
		public abstract bool Backspace();
		public abstract bool CanUndo { get; }
		public abstract bool Undo();
		public abstract bool CursorToDisplayPosition(int newPosition, bool forceSelection);
		public abstract void SelectAll();
		public bool CursorLeft(bool forceSelection) {
			return CursorLeft(forceSelection, false);
		}
		public abstract bool CursorLeft(bool forceSelection, bool isNeededKeyCheck);
		public bool CursorRight(bool forceSelection) {
			return CursorRight(forceSelection, false);
		}
		public abstract bool CursorRight(bool forceSelection, bool isNeededKeyCheck);
		public abstract bool CursorHome(bool forceSelection);
		public abstract bool CursorEnd(bool forceSelection);
		public abstract bool SpinUp();
		public abstract bool SpinDown();
		public virtual bool FlushPendingEditActions() {
			return false;	
		}
	#region Events
		public event EventHandler EditTextChanged;
		public event MaskChangingEventHandler EditTextChanging;
		public event CancelEventHandler LocalEditAction;
		protected void RaiseEditTextChanged() {
			if(EditTextChanged != null)
				EditTextChanged(this, EventArgs.Empty);
		}
		protected virtual bool RaiseEditTextChanging(object newEditValue) {
			if(EditTextChanging != null) {
				MaskChangingEventArgs e = new MaskChangingEventArgs(GetCurrentEditValue(), newEditValue);
				EditTextChanging(this, e);
				return !e.Cancel;
			}
			return true;
		}
		protected bool RaiseModifyWithoutEditValueChange() {
			if(LocalEditAction != null) {
				CancelEventArgs e = new CancelEventArgs();
				LocalEditAction(this, e);
				if(e.Cancel)
					return false;
			}
			return true;
		}
	#endregion
	}
	public abstract class MaskManagerState {
		public abstract bool IsSame(MaskManagerState comparedState);
	}
	public abstract class MaskManagerStated<TState> : MaskManager where TState: MaskManagerState {
		protected enum StateChangeType { Insert, Delete, Terminator }
		TState currentState, backupState;
		protected TState CurrentState { get { return currentState; } }
		StateChangeType backupType = StateChangeType.Terminator;
		public override bool CanUndo { get { return backupState != null && currentState != null; } }
		public override bool Undo() {
			if(!CanUndo)
				return false;
			if(!RaiseEditTextChanging(GetEditText(backupState)))
				return false;
			backupType = StateChangeType.Terminator;
			var tmpState = currentState;
			currentState = backupState;
			backupState = tmpState;
			RaiseEditTextChanged();
			return true;
		}
		void ApplyInternal(TState newState, StateChangeType changeType) {
			if(backupType != changeType) {
				if(changeType != StateChangeType.Terminator) {
					backupState = currentState;
				}
				backupType = changeType;
			}
			currentState = newState;
		}
		protected bool Apply(TState newState, StateChangeType changeType, bool isNeededKeyCheck) {
			if(!IsValid(newState))
				return false;
			if(newState.IsSame(CurrentState))
				return false;
			string newEditText = GetEditText(newState);
			if(isNeededKeyCheck)
				return true;
			if(GetCurrentEditText() != newEditText) {
				if(!RaiseEditTextChanging(newEditText))
					return false;
				ApplyInternal(newState, changeType);
				RaiseEditTextChanged();
			} else {
				ApplyInternal(newState, changeType);
			}
			return true;
		}
		protected virtual bool IsValid(TState newState) {
			return true;
		}
		protected bool Apply(TState newState, StateChangeType changeType) {
			return Apply(newState, changeType, false);
		}
		protected void SetInitialState(TState newState) {
			backupState = null;
			currentState = newState;
			backupType = StateChangeType.Terminator;
		}
		protected MaskManagerStated(TState initialState)
			: base() {
			SetInitialState(initialState);
		}
		TState cachedDState = null;
		int cachedDCP = -1;
		int cachedDSA = -1;
		string cachedDT = null;
		void VerifyCache() {
			if(!object.ReferenceEquals(cachedDState, CurrentState)) {
				cachedDState = CurrentState;
				cachedDCP = -1;
				cachedDSA = -1;
				cachedDT = null;
			}
		}
		public override bool SpinUp() {
			return CursorLeft(false);
		}
		public override bool SpinDown() {
			return CursorRight(false);
		}
		public sealed override int DisplayCursorPosition {
			get {
				VerifyCache();
				if(cachedDCP < 0) {
					cachedDCP = GetCursorPosition(CurrentState);
				}
				return cachedDCP;
			}
		}
		public sealed override int DisplaySelectionAnchor {
			get {
				VerifyCache();
				if(cachedDSA < 0) {
					cachedDSA = GetSelectionAnchor(CurrentState);
				}
				return cachedDSA;
			}
		}
		public sealed override string DisplayText {
			get {
				VerifyCache();
				if(cachedDT == null) {
					cachedDT = GetDisplayText(CurrentState);
				}
				return cachedDT;
			}
		}
		public sealed override object GetCurrentEditValue() {
			return GetEditValue(CurrentState);
		}
		protected abstract int GetCursorPosition(TState state);
		protected abstract int GetSelectionAnchor(TState state);
		protected abstract string GetDisplayText(TState state);
		protected abstract string GetEditText(TState state);
		protected abstract object GetEditValue(TState state);
		public sealed override string GetCurrentEditText() {
			return GetEditText(CurrentState);
		}
	}
	public class MaskLogicResult {
		string editText;
		int cursorPosition;
		public string EditText { get { return editText; } }
		public int CursorPosition { get { return cursorPosition; } }
		public MaskLogicResult(string editText, int cursorPosition) {
			this.editText = editText;
			this.cursorPosition = cursorPosition;
		}
	}
	public class MaskManagerPlainTextState : MaskManagerState {
		string editText;
		int cursorPosition, selectionAnchor;
		public string EditText { get { return editText; } }
		public int CursorPosition { get { return cursorPosition; } }
		public int SelectionAnchor { get { return selectionAnchor; } }
		public static MaskManagerPlainTextState Empty = new MaskManagerPlainTextState(string.Empty, 0, 0);
		public MaskManagerPlainTextState(string editText, int cursorPosition, int selectionAnchor) {
			this.editText = editText;
			this.cursorPosition = cursorPosition;
			this.selectionAnchor = selectionAnchor;
		}
		public override bool IsSame(MaskManagerState comparedState) {
			if(comparedState == null)
				return false;
			if(this.GetType() != comparedState.GetType())
				throw new NotImplementedException(MaskExceptionsTexts.InternalErrorNonSpecific);
			MaskManagerPlainTextState comparedCommonState = (MaskManagerPlainTextState)comparedState;
			if(this.EditText != comparedCommonState.EditText)
				return false;
			if(this.CursorPosition != comparedCommonState.CursorPosition)
				return false;
			if(this.SelectionAnchor != comparedCommonState.SelectionAnchor)
				return false;
			return true;
		}
	}
	public class MaskManagerPlainText: MaskManagerCommon<MaskManagerPlainTextState> {
		public MaskManagerPlainText() : base(MaskManagerPlainTextState.Empty) { }
		protected override MaskManagerPlainTextState CreateStateForApply(string editText, int cursorPosition, int selectionAnchor) {
			return new MaskManagerPlainTextState(editText, cursorPosition, selectionAnchor);
		}
		protected override MaskManagerPlainTextState GetEmptyState() {
			return MaskManagerPlainTextState.Empty;
		}
		protected bool Apply(string editText, int cursorPosition, int selectionAnchor, StateChangeType changeType) {
			return Apply(CreateStateForApply(editText, cursorPosition, selectionAnchor), changeType);
		}
		public override bool Insert(string insertion) {
			StateChangeType changeType = (insertion.Length == 0 && IsSelection) ? StateChangeType.Delete : StateChangeType.Insert;
			int selectionStart = Math.Min(CurrentState.CursorPosition, CurrentState.SelectionAnchor);
			int selectionEnd = Math.Max(CurrentState.CursorPosition, CurrentState.SelectionAnchor);
			string head = CurrentState.EditText.Substring(0, selectionStart);
			string tail = CurrentState.EditText.Substring(selectionEnd);
			string newText = head + insertion + tail;
			int cursorPosition = head.Length + insertion.Length;
			return Apply(newText, cursorPosition, cursorPosition, changeType);
		}
		public override bool Delete() {
			if(IsSelection)
				return Insert(string.Empty);
			string head = CurrentState.EditText.Substring(0, CurrentState.CursorPosition);
			string tail = CurrentState.EditText.Substring(CurrentState.CursorPosition);
			if(tail.Length < 1)
				return false;
			return Apply(head + tail.Substring(1), CurrentState.CursorPosition, CurrentState.CursorPosition, StateChangeType.Delete);
		}
		public override bool Backspace() {
			if(IsSelection)
				return Insert(string.Empty);
			string head = CurrentState.EditText.Substring(0, CurrentState.CursorPosition);
			string tail = CurrentState.EditText.Substring(CurrentState.CursorPosition);
			if(head.Length < 1)
				return false;
			return Apply(head.Substring(0, head.Length - 1) + tail, CurrentState.CursorPosition - 1, CurrentState.CursorPosition - 1, StateChangeType.Delete);
		}
		protected override string GetEditText(MaskManagerPlainTextState state) {
			return state.EditText;
		}
		protected override string GetDisplayText(MaskManagerPlainTextState state) {
			return state.EditText;
		}
		protected override int GetCursorPosition(MaskManagerPlainTextState state) {
			return state.CursorPosition;
		}
		protected override int GetSelectionAnchor(MaskManagerPlainTextState state) {
			return state.SelectionAnchor;
		}
		public override void SelectAll() {
			string text = CurrentState.EditText;
			Apply(text, 0, text.Length, StateChangeType.Terminator);
		}
		public override bool IsEditValueAssignedAsFormattedText {
			get { return true; }
		}
	}
	public abstract class MaskManagerCommon<TState>: MaskManagerStated<TState> where TState: MaskManagerPlainTextState {
		protected abstract TState CreateStateForApply(string editText, int cursorPosition, int selectionAnchor);
		protected abstract TState GetEmptyState();
		protected MaskManagerCommon(TState initialState) : base(initialState) { }
		protected bool MoveCursorTo(int newPosition, bool forceSelection, bool isNeededKeyCheck) {
			return Apply(CreateStateForApply(GetCurrentEditText(), newPosition, forceSelection ? CurrentState.SelectionAnchor : newPosition), StateChangeType.Terminator, isNeededKeyCheck);
		}
		protected bool MoveCursorTo(int newPosition, bool forceSelection) {
			return MoveCursorTo(newPosition, forceSelection, false);
		}
		protected virtual bool IsValidCursorPosition(int testedPositionInEditText) {
			if(testedPositionInEditText < 0)
				return false;
			if(testedPositionInEditText > CurrentState.EditText.Length)
				return false;
			return true;
		}
		public override bool CursorLeft(bool forceSelection, bool isNeededKeyCheck) {
			for(int nextCursorPosition = CurrentState.CursorPosition - 1; nextCursorPosition >= 0; --nextCursorPosition) {
				if(IsValidCursorPosition(nextCursorPosition)) {
					return MoveCursorTo(nextCursorPosition, forceSelection, isNeededKeyCheck);
				}
			}
			if(IsSelection && !forceSelection) {
				return MoveCursorTo(CurrentState.CursorPosition, false, isNeededKeyCheck);
			}
			return false;
		}
		public override bool CursorRight(bool forceSelection, bool isNeededKeyCheck) {
			for(int nextCursorPosition = CurrentState.CursorPosition + 1; nextCursorPosition <= CurrentState.EditText.Length; ++nextCursorPosition) {
				if(IsValidCursorPosition(nextCursorPosition)) {
					return MoveCursorTo(nextCursorPosition, forceSelection, isNeededKeyCheck);
				}
			}
			if(IsSelection && !forceSelection) {
				return MoveCursorTo(CurrentState.CursorPosition, false, isNeededKeyCheck);
			}
			return false;
		}
		public override bool CursorToDisplayPosition(int newPosition, bool forceSelection) {
			for(int i = 0; i <= GetCurrentEditText().Length; ++i) {
				if(IsValidCursorPosition(newPosition + i)) {
					return MoveCursorTo(newPosition + i, forceSelection);
				}
				if(IsValidCursorPosition(newPosition - i)) {
					return MoveCursorTo(newPosition - i, forceSelection);
				}
			}
			return false; 
		}
		public override bool CursorHome(bool forceSelection) {
			return CursorToDisplayPosition(0, forceSelection);
		}
		public override bool CursorEnd(bool forceSelection) {
			return CursorToDisplayPosition(DisplayText.Length, forceSelection);
		}
		public override void SetInitialEditText(string initialEditText) {
			SetInitialState(GetEmptyState());
			if(!Insert(initialEditText)) {
				bool inserted = false;
				foreach(char ch in initialEditText) {
					if(Insert(ch.ToString(CultureInfo.InvariantCulture)))
						inserted = true;
				}
				if(!inserted)
					Insert(string.Empty);
			}
			SetInitialState(CreateStateForApply(CurrentState.EditText, CurrentState.CursorPosition, CurrentState.SelectionAnchor));
		}
		protected override object GetEditValue(TState state) {
			return GetEditText(state);
		}
		public override void SetInitialEditValue(object initialEditValue) {
			SetInitialEditText(string.Format(CultureInfo.InvariantCulture, "{0}", initialEditValue));
		}
	}
	public abstract class MaskManagerSelectAllEnhancer<TNestedMaskManager>: MaskManager where TNestedMaskManager: MaskManager {
		public readonly TNestedMaskManager Nested;
		bool _isForcedSelectAll;
		protected bool IsSelectAllEnforced { get { return _isForcedSelectAll; } }
		protected virtual bool IsNestedCanSelectAll { get { return true; } }
		public override void SelectAll() {
			Nested.SelectAll();
			bool isSelectAllAlready = IsNestedCanSelectAll && Nested.DisplayCursorPosition == 0 && Nested.DisplaySelectionAnchor == Nested.DisplayText.Length;
			_isForcedSelectAll = !isSelectAllAlready;
		}
		protected void ClearSelectAllFlag() {
			_isForcedSelectAll = false;
		}
		protected virtual bool MakeChange(Func<bool> changeWithTrueWhenSuccessfull) {
			bool rv = changeWithTrueWhenSuccessfull();
			if(rv)
				ClearSelectAllFlag();
			return rv;
		}
		protected virtual bool MakeCursorOp(Func<bool> cursorOpWithTrueWhenSuccessfull) {
			bool wasSelectAll = IsSelectAllEnforced;
			ClearSelectAllFlag();
			bool rv = cursorOpWithTrueWhenSuccessfull();
			return wasSelectAll || rv;
		}
		protected virtual bool MakeSpinOrUndoOp(Func<bool> spinOrUndoOpWithTrueWhenSuccessfull) {
			return MakeCursorOp(spinOrUndoOpWithTrueWhenSuccessfull);
		}
		protected MaskManagerSelectAllEnhancer(TNestedMaskManager nested) {
			this.Nested = nested;
			this.Nested.EditTextChanged += new EventHandler(Nested_EditTextChanged);
			this.Nested.EditTextChanging += new MaskChangingEventHandler(Nested_EditTextChanging);
			this.Nested.LocalEditAction += new CancelEventHandler(Nested_LocalEditAction);
		}
		void Nested_LocalEditAction(object sender, CancelEventArgs e) {
			e.Cancel = !RaiseModifyWithoutEditValueChange();
		}
		void Nested_EditTextChanging(object sender, MaskChangingEventArgs e) {
			e.Cancel = !RaiseEditTextChanging(e.NewValue);
		}
		void Nested_EditTextChanged(object sender, EventArgs e) {
			RaiseEditTextChanged();
		}
		public override string GetCurrentEditText() {
			return Nested.GetCurrentEditText();
		}
		public override object GetCurrentEditValue() {
			return Nested.GetCurrentEditValue();
		}
		public override void SetInitialEditText(string initialEditText) {
			ClearSelectAllFlag();
			Nested.SetInitialEditText(initialEditText);
		}
		public override void SetInitialEditValue(object initialEditValue) {
			ClearSelectAllFlag();
			Nested.SetInitialEditValue(initialEditValue);
		}
		public override bool IsEditValueAssignedAsFormattedText {
			get { return Nested.IsEditValueAssignedAsFormattedText; }
		}
		public override string DisplayText {
			get { return Nested.DisplayText; }
		}
		public override int DisplayCursorPosition {
			get {
				if(IsSelectAllEnforced)
					return 0;
				else
					return Nested.DisplayCursorPosition;
			}
		}
		public override int DisplaySelectionAnchor {
			get {
				if(IsSelectAllEnforced)
					return DisplayText.Length;
				else
					return Nested.DisplaySelectionAnchor;
			}
		}
		public override bool Insert(string insertion) {
			return MakeChange(() => Nested.Insert(insertion));
		}
		public override bool Delete() {
			return MakeChange(() => Nested.Delete());
		}
		public override bool Backspace() {
			return MakeChange(() => Nested.Backspace());
		}
		public override bool CanUndo {
			get { return Nested.CanUndo; }
		}
		public override bool Undo() {
			return MakeSpinOrUndoOp(() => Nested.Undo());
		}
		public override bool CursorToDisplayPosition(int newPosition, bool forceSelection) {
			return MakeCursorOp(() => Nested.CursorToDisplayPosition(newPosition, forceSelection));
		}
		public override bool CursorLeft(bool forceSelection, bool isNeededKeyCheck) {
			return MakeCursorOp(() => Nested.CursorLeft(forceSelection, isNeededKeyCheck));
		}
		public override bool CursorRight(bool forceSelection, bool isNeededKeyCheck) {
			return MakeCursorOp(() => Nested.CursorRight(forceSelection, isNeededKeyCheck));
		}
		public override bool CursorHome(bool forceSelection) {
			return MakeCursorOp(() => Nested.CursorHome(forceSelection));
		}
		public override bool CursorEnd(bool forceSelection) {
			return MakeCursorOp(() => Nested.CursorEnd(forceSelection));
		}
		public override bool SpinUp() {
			return MakeSpinOrUndoOp(() => Nested.SpinUp());
		}
		public override bool SpinDown() {
			return MakeSpinOrUndoOp(() => Nested.SpinDown());
		}
		public override bool FlushPendingEditActions() {
			bool rv = Nested.FlushPendingEditActions();
			if(rv && IsSelectAllEnforced)
				ClearSelectAllFlag();
			return rv;
		}
		public override bool IsFinal {
			get {
				return Nested.IsFinal;
			}
		}
		public override bool IsMatch {
			get {
				return Nested.IsMatch;
			}
		}
	}
	public abstract class MaskManagerFormattingBase<TState>: MaskManagerCommon<TState> where TState: MaskManagerPlainTextState {
		protected MaskManagerFormattingBase(TState initialState) : base(initialState) { }
		protected override string GetDisplayText(TState state) {
			return CreateFormattedText(GetEditText(state));
		}
		protected override int GetCursorPosition(TState state) {
			return GetFormattedPosition(GetEditText(state), state.CursorPosition);
		}
		protected override int GetSelectionAnchor(TState state) {
			return GetFormattedPosition(GetEditText(state), state.SelectionAnchor);
		}
		public override bool CursorToDisplayPosition(int newPosition, bool forceSelection) {
			int editPosition = GetEditPosition(GetEditText(CurrentState), newPosition);
			return base.CursorToDisplayPosition(editPosition, forceSelection);
		}
		protected virtual int GetEditPosition(string editText, int formattedPosition) {
			int leftBound = 0;
			int rightBound = editText.Length;
			for(; ; ) {
				if(rightBound - leftBound <= 1) {
					if(GetFormattedPosition(editText, rightBound) <= formattedPosition) {
						return rightBound;
					} else {
						return leftBound;
					}
				}
				int candidateBound = (leftBound + rightBound) / 2;
				int candidateFormatted = GetFormattedPosition(editText, candidateBound);
				if(candidateBound == formattedPosition)
					return candidateBound;
				if(candidateFormatted < formattedPosition) {
					leftBound = candidateBound;
				} else {
					rightBound = candidateBound;
				}
			}
		}
		protected abstract string CreateFormattedText(string editText);
		protected abstract int GetFormattedPosition(string editText, int editPosition);
	}
	public static class DateTimeFormatHelper {
		public static string GetDateSeparator(CultureInfo culture) {
			return culture.GetDateSeparator();
		}
		public static string GetTimeSeparator(CultureInfo culture) {
			return culture.GetTimeSeparator();
		}
	}
}
