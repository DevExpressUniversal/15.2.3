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
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.Data.Mask;
using DevExpress.Utils.Drawing.Helpers;
using System.Security;
using DevExpress.XtraEditors.Native;
namespace DevExpress.XtraEditors.Mask {
	[ToolboxItem(false)]
	public class MaskBox : TextBox {
		public event EventHandler EditTextChanged;
		public event ChangingEventHandler EditTextChanging;
		MaskManager _manager = null;
		MaskProperties _mask = null;
		int _assignValueCount = 0;
		int _initializeInProgress = 0;
		MaskStrategy.SimpleStrategy _strategySimple;
		MaskStrategy.ManagedStrategy _strategyManaged;
#if DEBUGTEST
		public MaskBox() {
			System.Diagnostics.Debug.Assert(!IsHandleCreated);
			base.DestroyHandle();
		}
#endif
		#region TextBox public properties obsoleters
		[Obsolete("Use MaskBoxSelectAll instead", true)]
		new public void SelectAll() {
			throw new NotImplementedException();
		}
		[Obsolete("Use MaskBoxSelect instead", true)]
		new public void Select(int start, int length) {
			throw new NotImplementedException();
		}
		[Obsolete("Use MaskBoxScrollToCaret instead", true)]
		new public void ScrollToCaret() {
			throw new NotImplementedException();
		}
		[DXCategory(CategoryName.Appearance), Obsolete("Use MaskBoxSelectionStart instead", true)]
		new public int SelectionStart {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		[DXCategory(CategoryName.Appearance), Obsolete("Use MaskBoxSelectionLength instead", true)]
		new public int SelectionLength {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		[DXCategory(CategoryName.Appearance), Obsolete("Use MaskBoxSelectedText instead", true)]
		new public string SelectedText {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		[DXCategory(CategoryName.Appearance), Obsolete("Use MaskBoxText instead", true)]
		new public string Text {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		[Obsolete("Use MaskBoxUndo instead", true)]
		new public void Undo() {
			throw new NotImplementedException();
		}
		[Browsable(false), Obsolete("Use MaskBoxCanUndo instead", true)]
		new public bool CanUndo {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		#endregion TextBox public properties obsoleters
		public void MaskBoxSelectAll() {
			CurrentMaskStrategy.SelectAll();
		}
		public void MaskBoxDeselectAll() {
			CurrentMaskStrategy.DeselectAll();
		}
		public void MaskBoxSelect(int start, int length) {
			CurrentMaskStrategy.Select(start, length);
		}
		public void MaskBoxScrollToCaret() {
			CurrentMaskStrategy.ScrollToCaret();
		}
		[DXCategory(CategoryName.Appearance)]
		public int MaskBoxSelectionStart {
			get {
				return CurrentMaskStrategy.GetSelectionStart();
			}
			set {
				CurrentMaskStrategy.SetSelectionStart(value);
			}
		}
		[DXCategory(CategoryName.Appearance)]
		public int MaskBoxSelectionLength {
			get {
				return CurrentMaskStrategy.GetSelectionLength();
			}
			set {
				CurrentMaskStrategy.SetSelectionLength(value);
			}
		}
		[DXCategory(CategoryName.Appearance)]
		public string MaskBoxSelectedText {
			get {
				return CurrentMaskStrategy.GetSelectedText();
			}
			set {
				CurrentMaskStrategy.SetSelectedText(value);
			}
		}
		[DXCategory(CategoryName.Appearance), RefreshProperties(RefreshProperties.All)]
		public string MaskBoxText {
			get {
				return CurrentMaskStrategy.GetText();
			}
		}
		public bool GetIsSelectAllMode() {
			return CurrentMaskStrategy.GetIsSelectAllMode();
		}
		public void ProcessF2() {
			CurrentMaskStrategy.ProcessF2();
		}
		const int GWL_STYLE = -16, ES_UPPERCASE = 0x0008, ES_LOWERCASE = 0x0010;
		public new CharacterCasing CharacterCasing {
			get {
				return base.CharacterCasing;
			}
			set {
				if(CharacterCasing == value) return;
				if(!IsHandleCreated) {
					base.CharacterCasing = value;
					return;
				}
				try {
					var fi = typeof(TextBox).GetField("characterCasing", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
					fi.SetValue(this, value);
					int style = NativeMethods.GetWindowLong(Handle, GWL_STYLE) & (~(ES_LOWERCASE | ES_UPPERCASE));
					if(value == System.Windows.Forms.CharacterCasing.Upper) style |= ES_UPPERCASE;
					if(value == System.Windows.Forms.CharacterCasing.Lower) style |= ES_LOWERCASE;
					NativeMethods.SetWindowLong(Handle, GWL_STYLE, style);
				}
				catch { }
			}
		}
		public void MaskBoxUndo() {
			CurrentMaskStrategy.Undo();
		}
		[Browsable(false)]
		public bool MaskBoxCanUndo {
			get { return CurrentMaskStrategy.GetCanUndo(); }
		}
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) { }
		#region MaskStrategy
		[SecuritySafeCritical]
		static bool GetKeyboardState(byte[] keyStates) { return GetKeyboardStateCore(keyStates); }
		[System.Runtime.InteropServices.DllImport("USER32.dll", EntryPoint = "GetKeyboardState")]
		static extern bool GetKeyboardStateCore(byte[] keyStates);
		[SecuritySafeCritical]
		static bool SetKeyboardState(byte[] keyStates) { return SetKeyboardStateCore(keyStates); }
		[System.Runtime.InteropServices.DllImport("USER32.dll", EntryPoint = "SetKeyboardState")]
		static extern bool SetKeyboardStateCore(byte[] keyStates);
		const int VK_SHIFT = 0x10;
		const int VK_CONTROL = 0x11;
		const int VK_LEFT = 0x25;
		const int VK_RIGHT = 0x27;
		const int VK_HOME = 0x24;
		const int WM_KEYDOWN = 0x100;
		void CursorAtSelectionStartTo(int position) {
			byte[] savedKeyboardState = new byte[256];
			GetKeyboardState(savedKeyboardState);
			try {
				byte[] patchedKeyboardState = (byte[])savedKeyboardState.Clone();
				patchedKeyboardState[VK_SHIFT] = 128;
				if(position < this.BaseSelectionStart - position) {
					patchedKeyboardState[VK_CONTROL] = 128;
					SetKeyboardState(patchedKeyboardState);
					Message msgHome = new Message();
					msgHome.HWnd = this.Handle;
					msgHome.WParam = (IntPtr)VK_HOME;
					msgHome.Msg = WM_KEYDOWN;
					this.DefWndProc(ref msgHome);
				}
				patchedKeyboardState[VK_CONTROL] = 0;
				SetKeyboardState(patchedKeyboardState);
				bool toLeft = position < this.BaseSelectionStart;
				Message msgCursor = new Message();
				msgCursor.HWnd = this.Handle;
				msgCursor.WParam = (IntPtr)(toLeft ? VK_LEFT : VK_RIGHT);
				msgCursor.Msg = WM_KEYDOWN;
				for(; ; ) {
					int selectionStartCached = this.BaseSelectionStart;
					if(toLeft) {
						if(selectionStartCached <= position)
							break;
					} else {
						if(selectionStartCached >= position)
							break;
					}
					this.DefWndProc(ref msgCursor);
					if(selectionStartCached == this.BaseSelectionStart)
						break;
				}
			} finally {
				SetKeyboardState(savedKeyboardState);
			}
		}
		[Obsolete("Please use SelectAllReversed instead")]
		private void BaseSelectAll() { base.SelectAll(); }
		private void BaseSelect(int start, int length) { base.Select(start, length); }
		private void BaseScrollToCaret() { base.ScrollToCaret(); }
		private int BaseSelectionStart {
			get { return base.SelectionStart; }
			set { base.SelectionStart = value; }
		}
		private int BaseSelectionLength {
			get { return base.SelectionLength; }
			set { base.SelectionLength = value; }
		}
		private string BaseSelectedText {
			get { return base.SelectedText; }
			set { base.SelectedText = value; }
		}
		private string BaseText {
			get { return base.Text; }
			set { base.Text = value; }
		}
		private void BaseUndo() { base.Undo(); }
		private bool BaseCanUndo { get { return base.CanUndo; } }
		private void BaseWndProc(ref Message m) {
			base.WndProc(ref m);
		}
		private void SelectAllReversed() {
			this.BaseSelectionStart = int.MaxValue;
			this.CursorAtSelectionStartTo(0);
		}
		public static string GetClipboardText() {
			IDataObject iData = null;
			try {
				iData = Clipboard.GetDataObject();
			} catch {
			}
			if(iData == null)
				return string.Empty;
			string rv = null;
			if(iData.GetDataPresent(DataFormats.UnicodeText))
				rv = (string)iData.GetData(DataFormats.UnicodeText);
			else if(iData.GetDataPresent(DataFormats.Text))
				rv = (string)iData.GetData(DataFormats.Text);
			return rv ?? string.Empty;
		}
		void ClearStrategies() {
			this._strategyManaged = null;
			this._strategySimple = null;
		}
		private MaskStrategy GetSimpleStrategy() {
			if(_strategySimple == null) {
				_strategySimple = new MaskStrategy.SimpleStrategy(this);
				_strategySimple.CatchUpAfterCreation();
			}
			return _strategySimple;
		}
		private MaskStrategy GetManagedStrategy() {
			if(_strategyManaged == null)
				_strategyManaged = new MaskStrategy.ManagedStrategy(this);
			return _strategyManaged;
		}
		private MaskStrategy CurrentMaskStrategy {
			get {
				if(IsInitializeInProgress)	
					return GetSimpleStrategy();
				else if(Mask.MaskType == MaskType.None)
					return GetSimpleStrategy();
				else
					return GetManagedStrategy();
			}
		}
		private abstract class MaskStrategy {
			readonly MaskBox _owner;
			protected MaskBox Owner { get { return _owner; } }
			protected MaskStrategy(MaskBox owner) {
				this._owner = owner;
			}
			public abstract void SelectAll();
			public abstract void DeselectAll();
			public abstract void Select(int start, int length);
			public abstract void ScrollToCaret();
			public abstract int GetSelectionStart();
			public abstract void SetSelectionStart(int value);
			public abstract int GetSelectionLength();
			public abstract void SetSelectionLength(int value);
			public abstract string GetSelectedText();
			public abstract void SetSelectedText(string value);
			public abstract string GetText();
			public abstract bool GetIsSelectAllMode();
			public abstract void ProcessF2();
			public abstract bool GetIsMatch();
			public abstract string GetVisibleText();
			public abstract string GetEditText();
			public abstract void SetEditText(string value);
			public abstract bool GetIsEditValueDifferFromEditText();
			public abstract void FlushPendingEditActions();
			public abstract object GetEditValue();
			public abstract void SetEditValue(object value);
			public abstract bool IsNeededCursorKey(Keys keyData);
			public abstract void DoAfterKeyDown(KeyEventArgs e);
			public abstract void DoAfterKeyPress(KeyPressEventArgs e);
			public abstract void DoAfterTextChanged(EventArgs e);
			public abstract bool DoSpin(bool isUp);
			public abstract void DoWndProc(ref Message m);
			public abstract void ResetSelection();
			public bool IsRightToLeft { get { return WindowsFormsSettings.GetIsRightToLeft(Owner.RightToLeft); } }
			public void Undo() { Owner.BaseUndo(); }
			public abstract bool GetCanUndo();
			public class SimpleStrategy : MaskStrategy {
				string cachedOldText;
				public SimpleStrategy(MaskBox owner) : base(owner) {
				}
				internal void CatchUpAfterCreation() {
					this.cachedOldText = Owner.BaseText;
				}
				public override void SelectAll() {
					Owner.SelectAllReversed();
				}
				public override void DeselectAll() {
					Owner.BaseSelectionLength = 0;
				}
				public override void Select(int start, int length) {
					Owner.BaseSelect(start, length);
				}
				public override void ScrollToCaret() {
					Owner.BaseScrollToCaret();
				}
				public override int GetSelectionStart() {
					return Owner.BaseSelectionStart;
				}
				public override void SetSelectionStart(int value) {
					Owner.BaseSelectionStart = value;
				}
				public override int GetSelectionLength() {
					return Owner.BaseSelectionLength;
				}
				public override void SetSelectionLength(int value) {
					Owner.BaseSelectionLength = value;
				}
				public override string GetSelectedText() {
					return Owner.BaseSelectedText;
				}
				public override void SetSelectedText(string value) {
					Owner.BaseSelectedText = value;
				}
				public override string GetText() {
					return Owner.BaseText;
				}
				public override bool GetIsSelectAllMode() {
					int selStart = Owner.BaseSelectionStart;
					int selLength = Owner.BaseSelectionLength;
					int textLength = Owner.BaseText.Length;
					return selStart == 0 && selLength == textLength || selStart == -1 && selLength == -1;
				}
				public override void ProcessF2() {
					if(Owner.BaseSelectionLength == 0)
						Owner.SelectAllReversed();
					else
						Owner.BaseSelectionLength = 0;
				}
				public override bool GetIsMatch() {
					return true;
				}
				public override string GetVisibleText() {
					return Owner.BaseText;
				}
				public override string GetEditText() {
					return Owner.BaseText;
				}
				public override void SetEditText(string value) {
					if(value == null)
						value = string.Empty;
					if(GetEditText() == value)
						return;
					if(Owner.CharacterCasing != CharacterCasing.Normal) {
						if(GetEditText().ToLower() == value.ToLower())
							return;
					}
					this.cachedOldText = value;
					Owner.BaseText = value;
				}
				public override bool GetIsEditValueDifferFromEditText() {
					return false;
				}
				public override void FlushPendingEditActions() { }
				public override object GetEditValue() {
					return GetEditText();
				}
				public override void SetEditValue(object value) {
					if(value == null)
						value = string.Empty;
					string stringValue = value.ToString();
					SetEditText(stringValue);
				}
				public override bool IsNeededCursorKey(Keys keyData) {
					if((keyData & Keys.Modifiers) != 0)
						return true;
					if(GetSelectionLength() == GetText().Length) {
						return false;
					}
					if(IsRightToLeft) {
						var k = keyData;
						if(k == Keys.Left) keyData = Keys.Right;
						if(k == Keys.Right) keyData = Keys.Left;
					}
					switch(keyData) {
						case Keys.Left:
							if(GetSelectionStart() == 0 && GetSelectionLength() == 0)
								return false;
							else
								return true;
						case Keys.Right:
							if(GetSelectionStart() == GetText().Length && GetSelectionLength() == 0)
								return false;
							else
								return true;
						case Keys.Up:
						case Keys.Down:
							if(!Owner.Multiline)
								return false;
							if(GetSelectionLength() == GetText().Length)
								return false;
							return true;
						default:
							return true;
					}
				}
				public override void DoAfterKeyDown(KeyEventArgs e) {
					switch(e.KeyCode) {
						case Keys.Up:
							if(Owner.FireSpinRequest(null, true))
								e.Handled = true;
							break;
						case Keys.Down:
							if(Owner.FireSpinRequest(null, false))
								e.Handled = true;
							break;
						default:
							break;
					}
				}
				public override void DoAfterKeyPress(KeyPressEventArgs e) { }
				public override void DoAfterTextChanged(EventArgs e) {
					string newText = Owner.BaseText;
					if(cachedOldText != newText) {
						ChangingEventArgs che = new ChangingEventArgs(cachedOldText, newText);
						Owner.RaiseEditTextChanging(che);
						if(che.Cancel) {
							Owner.BaseText = cachedOldText;
						} else {
							if(ReferenceEquals(newText, che.NewValue)) {
								cachedOldText = newText;
							} else {
								if(che.NewValue == null) {
									newText = string.Empty;
								} else {
									newText = che.NewValue.ToString();
								}
								cachedOldText = newText;
								Owner.BaseText = newText;
							}
							Owner.RaiseEditTextChanged();
						}
					}
				}
				public override bool DoSpin(bool isUp) { return false; }
				public override void DoWndProc(ref Message m) {
					Owner.BaseWndProc(ref m);
				}
				public override void ResetSelection() {
					Owner.MaskBoxSelect(0, 0);
				}
				public override bool GetCanUndo() { return Owner.BaseCanUndo; }
			}
			public class ManagedStrategy : MaskStrategy {
				uint lockEmSetSel = 0;
				uint wndProcDepth = 0;
				bool deferredUpdateVisualStateRequired_ = false;
				int insideManagerChange = 0;
				class ManagerChangesMarker : IDisposable {
					readonly ManagedStrategy Strategy;
					public ManagerChangesMarker(ManagedStrategy strategy) {
						this.Strategy = strategy;
						Strategy.insideManagerChange++;
					}
					public void Dispose() {
						Strategy.insideManagerChange--;
					}
				}
				public bool IsSelectionUpdateMayBeNeeded { get { return insideManagerChange != 0 || IsUpdateVisualStateRequired; } }
				const int EM_SETSEL = 0xb1;
				const int WM_CUT = 0x300;
				const int WM_COPY = 0x301;
				const int WM_PASTE = 0x302;
				const int WM_CLEAR = 0x303;
				const int WM_CAPTURECHANGED = 0x0215;
				const int EM_CHARFROMPOS = 0xd7;
				const int WM_UNDO = 0x304;
				const int EM_UNDO = 0xc7;
				const int MB_ICONERROR = 0x010;
				const int WM_NULL = 0x0000;
				MaskManager Manager {
					get {
						if(Owner._manager == null)
							Owner.ChangeManager();
						return Owner._manager;
					}
				}
				void ProcessBeepOnError() {
					if(Owner.Mask.BeepOnError && !Owner.IsNowReadOnly)
						EditorsNativeMethods.MessageBeep(MB_ICONERROR);
				}
				struct SelectionStruct {
					public readonly int SelectionAnchor;
					public readonly int CursorPosition;
					public SelectionStruct(int selectionAnchor, int cursorPosition) {
						this.SelectionAnchor = selectionAnchor;
						this.CursorPosition = cursorPosition;
					}
				}
				SelectionStruct GetSel() {
					System.Diagnostics.Debug.Assert(Owner.Focused);
					if(Owner.BaseSelectionLength == 0)
						return new SelectionStruct(Owner.BaseSelectionStart, Owner.BaseSelectionStart);
					if(Manager.DisplaySelectionAnchor == Owner.BaseSelectionStart)
						return new SelectionStruct(Owner.BaseSelectionStart, Owner.BaseSelectionStart + Owner.BaseSelectionLength);
					if(Manager.DisplaySelectionAnchor == Owner.BaseSelectionStart + Owner.BaseSelectionLength)
						return new SelectionStruct(Owner.BaseSelectionStart + Owner.BaseSelectionLength, Owner.BaseSelectionStart);
					EditorsNativeMethods.GetCaretPosPoint p;
					EditorsNativeMethods.GetCaretPos(out p);
					Message msg = new Message();
					msg.HWnd = Owner.Handle;
					msg.Msg = EM_CHARFROMPOS;
					msg.LParam = (IntPtr)((p.x) | ((p.y + 2) << 16));
					Owner.DefWndProc(ref msg);
					int currentCursorPosition = (int)msg.Result;
					currentCursorPosition &= 0x0ffff;
					int rangeToSS = Math.Abs(Owner.BaseSelectionStart - currentCursorPosition);
					int rangeToSE = Math.Abs(Owner.BaseSelectionStart + Owner.BaseSelectionLength - currentCursorPosition);
					currentCursorPosition = (rangeToSS < rangeToSE) ? Owner.BaseSelectionStart : (Owner.BaseSelectionStart + Owner.BaseSelectionLength);
					int currentAnchorPosition = currentCursorPosition != Owner.BaseSelectionStart ? Owner.BaseSelectionStart : Owner.BaseSelectionStart + Owner.BaseSelectionLength;
					return new SelectionStruct(currentAnchorPosition, currentCursorPosition);
				}
				bool IsMouseSelectionStarted {
					get {
						if(!Owner.Focused)
							return false;
						if(Owner.Capture)
							return true;
						if((Control.MouseButtons & MouseButtons.Left) != MouseButtons.Left)
							return false;
						Point globalMousePosition = Control.MousePosition;
						Point localMousePosition = Owner.PointToClient(globalMousePosition);
						if(localMousePosition.X < 0 || localMousePosition.Y < 0 || localMousePosition.X >= Owner.Width || localMousePosition.Y >= Owner.Height)
							return false;
						return true;
					}
				}
				void UpdateCursorPos() {
#if DEBUGWriteMaskBox
					System.Diagnostics.Debug.WriteLine(string.Format("UpdateCursorPos; focus {0}, mouse buttons {1}, Capture {2}", Owner.Focused, Control.MouseButtons, Owner.Capture));
#endif
					if(IsMouseSelectionStarted) {
#if DEBUGWriteMaskBox
						System.Diagnostics.Debug.WriteLine("	Updating CursorPos suppressed -- mouse selection started");
#endif
						return;
					}
					lockEmSetSel++;
					try {
						if(!Owner.Focused) {
#if DEBUGWriteMaskBox
						System.Diagnostics.Debug.WriteLine("	Updating CursorPos no focus mode");
#endif
							if(Manager.DisplayCursorPosition >= Manager.DisplaySelectionAnchor) {
								Owner.BaseSelect(Manager.DisplaySelectionStart, Manager.DisplaySelectionLength);
							} else {
#if DEBUGWriteMaskBox
						System.Diagnostics.Debug.WriteLine("		Updating CursorPos no focus mode supressed -- cursor before selection anchor");
#endif
								Owner.BaseSelect(Manager.DisplaySelectionStart, Manager.DisplaySelectionLength);	
							}
							return;
						}
						SelectionStruct current = GetSel();
						if(current.CursorPosition != Manager.DisplayCursorPosition || current.SelectionAnchor != Manager.DisplaySelectionAnchor) {
#if DEBUGWriteMaskBox
							System.Diagnostics.Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "	Updating CursorPos: ccp:{0}, csa:{1}, mcp:{2}, msa:{3}", current.CursorPosition, current.SelectionAnchor, Manager.DisplayCursorPosition, Manager.DisplaySelectionAnchor));
#endif
							if(Manager.DisplayCursorPosition < Manager.DisplaySelectionAnchor) {
								if(Manager.DisplaySelectionAnchor != current.SelectionAnchor
									|| current.CursorPosition > current.SelectionAnchor
									|| current.SelectionAnchor - Manager.DisplayCursorPosition < Manager.DisplayCursorPosition - current.CursorPosition) {
									Owner.BaseSelect(Manager.DisplaySelectionAnchor, 0);
									Owner.CursorAtSelectionStartTo(Manager.DisplayCursorPosition);
								} else {
									Owner.CursorAtSelectionStartTo(Manager.DisplayCursorPosition);
									int newSelAnchor = Owner.BaseSelectionStart + Owner.BaseSelectionLength;
									if(newSelAnchor != current.SelectionAnchor && newSelAnchor == current.CursorPosition) {   
										Owner.BaseSelect(Manager.DisplaySelectionAnchor, 0);
										Owner.CursorAtSelectionStartTo(Manager.DisplayCursorPosition);
									}
								}
							} else {
								Owner.BaseSelect(Manager.DisplaySelectionAnchor, Manager.DisplayCursorPosition - Manager.DisplaySelectionAnchor);
							}
							Owner.BaseScrollToCaret();
						}
					} finally {
						lockEmSetSel--;
					}
				}
				void UpdateVisualState() {
					deferredUpdateVisualStateRequired_ = false;
					if(Owner.IsDisposed)
						return;
#if DEBUGWriteMaskBox
					System.Diagnostics.Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "UpdateVisualState to '{1}' from '{0}' supressed {2}", Owner.BaseText, Manager.DisplayText, Owner.IsUpdateVisualStateSupressed));
#endif
					bool needUpdate = false;
					lockEmSetSel++;
					try {
						if(Owner.IsUpdateVisualStateSupressed) {
							Owner.ForceVisibleText(Owner.VisibleText);
							if(Owner.Focused)
								Owner.SelectAllReversed();
							return;
						}
						if(Owner.BaseText != Manager.DisplayText) {
							needUpdate = true;
							Owner.BaseText = Manager.DisplayText;
#if DEBUGWriteMaskBox
							System.Diagnostics.Debug.WriteLine("	text updated");
#endif
						}
						UpdateCursorPos();
					} finally {
						lockEmSetSel--;
					}
					if(needUpdate)
						Owner.Update();
				}
				bool IsUpdateVisualStateRequired { get { return deferredUpdateVisualStateRequired_; } }
				void UpdateVisualStateRequire() {
					if(deferredUpdateVisualStateRequired_)
						return;
					deferredUpdateVisualStateRequired_ = true;
					if(wndProcDepth == 0) {
						NativeMethods.PostMessage(Owner.Handle, WM_NULL, (IntPtr)0, (IntPtr)0);
					}
				}
				void UpdateVisualStateIfRequired() {
					if(IsUpdateVisualStateRequired)
						UpdateVisualState();
				}
				void UpdateManagerPositionsFromCurrentSelections(int anchor, int caret) {
					int baseTextLength = Owner.BaseText.Length;
					if((anchor == 0 && (caret == -1 || caret >= baseTextLength)) || (caret == 0 && (anchor == -1 || anchor >= baseTextLength))) {
						Manager.SelectAll();
						return;
					}
					Manager.CursorToDisplayPosition(anchor, false);
					Manager.CursorToDisplayPosition(caret, true);
					if(Owner.BaseText != Manager.DisplayText || anchor == caret) {
						UpdateVisualStateRequire();
					}
				}
				void DoClipboardPaste() {
					using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
						if(Manager.Insert(MaskBox.GetClipboardText()))
							UpdateVisualStateRequire();
						else
							ProcessBeepOnError();
					}
				}
				void DoClipboardCut() {
					if(!DoClipboardCopy())
						return;
					if(Manager.IsSelection) {
						using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
							if(Manager.Delete()) {
								UpdateVisualStateRequire();
							} else {
								ProcessBeepOnError();
							}
						}
					}
				}
				bool DoClipboardCopy() {
					if(Owner.PasswordChar != '\0' || Owner.UseSystemPasswordChar)
						return false;
					string str = Owner.MaskBoxSelectedText;
					if(string.IsNullOrEmpty(str))
						return false;
					try {
						Clipboard.SetDataObject(str);
						return true;
					} catch {
						return false;
					}
				}
				void DoClear(ref Message m) {
					using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
						if(Manager.Delete()) {
							if(Manager.DisplayText == GetDelText(Owner.BaseText, Owner.BaseSelectionStart, Owner.BaseSelectionStart + Owner.BaseSelectionLength))
								Owner.BaseWndProc(ref m);
						} else {
							ProcessBeepOnError();
						}
						UpdateVisualStateRequire();
					}
				}
				void DoUndo() {
					using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
						if(Manager.Undo())
							UpdateVisualStateRequire();
						else
							ProcessBeepOnError();
					}
				}
				void DoOnCaptureChanged() {	
					if(!Owner.Focused)
						return;
					SelectionStruct resultSel = GetSel();
					UpdateManagerPositionsFromCurrentSelections(resultSel.SelectionAnchor, resultSel.CursorPosition);
				}
				public ManagedStrategy(MaskBox owner) : base(owner) { }
				public override void SelectAll() {
					Manager.SelectAll();
					UpdateVisualStateRequire();
				}
				public override void DeselectAll() {
					using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
						Manager.CursorToDisplayPosition(Manager.DisplaySelectionStart, false);
						UpdateVisualStateRequire();
					}
				}
				public override void Select(int start, int length) {
					using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
						Manager.CursorToDisplayPosition(start, false);
						Manager.CursorToDisplayPosition(start + length, true);
						UpdateVisualStateRequire();
					}
				}
				public override void ScrollToCaret() {
					UpdateVisualStateIfRequired();
					Owner.BaseScrollToCaret();
				}
				public override int GetSelectionStart() {
					if(Owner.IsUpdateVisualStateSupressed)
						return Owner.BaseSelectionStart;
					if(IsSelectionUpdateMayBeNeeded)
						return Manager.DisplaySelectionStart;
					return Owner.BaseSelectionStart;
				}
				public override void SetSelectionStart(int value) {
					UpdateVisualStateIfRequired();
					Owner.BaseSelectionStart = value;
				}
				public override int GetSelectionLength() {
					if(Owner.IsUpdateVisualStateSupressed)
						return Owner.BaseSelectionLength;
					if(IsSelectionUpdateMayBeNeeded)
						return Manager.DisplaySelectionLength;
					return Owner.BaseSelectionLength;
				}
				public override void SetSelectionLength(int value) {
					UpdateVisualStateIfRequired();
					Owner.BaseSelectionLength = value;
				}
				public override string GetSelectedText() {
					if(Owner.IsUpdateVisualStateSupressed)
						return Owner.BaseSelectedText;
					if(IsSelectionUpdateMayBeNeeded)
						return Manager.DisplayText.Substring(Manager.DisplaySelectionStart, Manager.DisplaySelectionLength);
					return Owner.BaseSelectedText;
				}
				public override void SetSelectedText(string value) {
					using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
						if(Manager.Insert(value))
							UpdateVisualStateRequire();
					}
				}
				public override string GetText() {
					if(Owner.IsUpdateVisualStateSupressed)
						return Owner.BaseText;
					return Manager.DisplayText;
				}
				public override bool GetIsSelectAllMode() {
					return GetText() == GetSelectedText();
				}
				public override void ProcessF2() {
					if(GetIsSelectAllMode()) {
						DeselectAll();
					} else {
						SelectAll();
					}
				}
				public override bool GetIsMatch() {
					return Manager.IsMatch;
				}
				public override string GetVisibleText() {
					if(Owner.IsUpdateVisualStateSupressed)
						return Owner.BaseText;
					return Manager.DisplayText;
				}
				public override string GetEditText() {
					return Manager.GetCurrentEditText();
				}
				public override void SetEditText(string value) {
					if(value == null)
						value = string.Empty;
					if(GetEditText() != value) {
						Manager.SetInitialEditText(value);
					}
					UpdateVisualState();
				}
				public override bool GetIsEditValueDifferFromEditText() {
					return !Manager.IsEditValueAssignedAsFormattedText;
				}
				public override void FlushPendingEditActions() {
					using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
						if(Manager.FlushPendingEditActions())
							UpdateVisualState();
					}
				}
				public override object GetEditValue() {
					return Manager.GetCurrentEditValue();
				}
				public override void SetEditValue(object value) {
					if(!Equals(Manager.GetCurrentEditValue(), value)) {
						Manager.SetInitialEditValue(value);
					}
					UpdateVisualState();
				}
				public override bool IsNeededCursorKey(Keys keyData) {
					if((keyData & Keys.Modifiers) != 0)
						return true;
					if(GetIsSelectAllMode())
						return false;
					switch(keyData & Keys.KeyCode) {
						case Keys.Left:
							return Manager.CursorLeft((keyData & Keys.Shift) == Keys.Shift, true);
						case Keys.Right:
							return Manager.CursorRight((keyData & Keys.Shift) == Keys.Shift, true);
						case Keys.Up:
						case Keys.Down:
						default:
							return false;
					}
				}
				public override void DoAfterKeyDown(KeyEventArgs e) {
					switch(e.KeyCode) {
						case Keys.Up:
							Owner.FireSpinRequest(null, true);
							e.Handled = true;
							break;
						case Keys.Left:
							using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
								if(Manager.CursorLeft(e.Shift)) {
									UpdateVisualStateRequire();
								} else {
									e.Handled = true;
								}
							}
							break;
						case Keys.Down:
							Owner.FireSpinRequest(null, false);
							e.Handled = true;
							break;
						case Keys.Right:
							using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
								if(Manager.CursorRight(e.Shift)) {
									UpdateVisualStateRequire();
								} else {
									e.Handled = true;
								}
							}
							break;
						case Keys.Home:
							using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
								if(Manager.CursorHome(e.Shift)) {
									UpdateVisualStateRequire();
								} else {
									e.Handled = true;
								}
							}
							break;
						case Keys.End:
							using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
								if(Manager.CursorEnd(e.Shift)) {
									UpdateVisualStateRequire();
								} else {
									e.Handled = true;
								}
							}
							break;
						case Keys.Delete:
							using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
								if(Manager.Delete()) {
									if(Manager.DisplayText != GetDelText(Owner.BaseText, Owner.BaseSelectionStart, Owner.BaseSelectionStart + Owner.BaseSelectionLength))
										e.Handled = true;
									UpdateVisualStateRequire();
								} else {
									e.Handled = true;
								}
							}
							break;
						default:
							break;
					}
				}
				string KeyCharToInsertableString(char keyChar) {
					if(!char.IsControl(keyChar))
						return keyChar.ToString(CultureInfo.InvariantCulture);
					if(keyChar == '\r' && Owner.Multiline && Owner.AcceptsReturn)
						return "\r\n";
					if(keyChar == '\t' && Owner.Multiline && Owner.AcceptsTab) {
						return "\t";
					}
					return null;
				}
				public override void DoAfterKeyPress(KeyPressEventArgs e) {
					switch(e.KeyChar) {
						case '\x03':
							DoClipboardCopy();
							e.Handled = true;
							break;
						case '\x16':
							DoClipboardPaste();
							e.Handled = true;
							break;
						case '\x18':
							DoClipboardCut();
							e.Handled = true;
							break;
						case '\b':
							using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
								if(Manager.Backspace()) {
									if(Manager.DisplayText != GetBackspaceText(Owner.BaseText, Owner.BaseSelectionStart, Owner.BaseSelectionStart + Owner.BaseSelectionLength))
										e.Handled = true;
									UpdateVisualStateRequire();
								} else {
									e.Handled = true;
									ProcessBeepOnError();
								}
							}
							break;
						default:
							if(Owner.IsIMEProcessing) {
								break;
							}
							string insertable = KeyCharToInsertableString(e.KeyChar);
							if(insertable == null) {
								e.Handled = true;
								break;
							}
							using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
								if(Manager.Insert(insertable)) {
									if(Manager.IsFinal) {
										e.Handled = true;
										UpdateVisualState();
										Owner.AutoLeaveHandler();
									} else {
										if(Manager.DisplayText != GetChangeText(Owner.BaseText, Owner.BaseSelectionStart, Owner.BaseSelectionStart + Owner.BaseSelectionLength, insertable))
											e.Handled = true;
										UpdateVisualStateRequire();
									}
								} else {
									e.Handled = true;
									ProcessBeepOnError();
								}
							}
							break;
					}
				}
				public override void DoAfterTextChanged(EventArgs e) { }
				public override bool DoSpin(bool isUp) {
					using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
						bool result = isUp ? Manager.SpinUp() : Manager.SpinDown();
						UpdateVisualStateRequire();
					}
					return true;	
				}
				public override void DoWndProc(ref Message m) {
					++wndProcDepth;
					try {
						switch(m.Msg) {
							case WM_PASTE:
								DoClipboardPaste();
								break;
							case WM_CLEAR:
								DoClear(ref m);
								break;
							case WM_CUT:
								DoClipboardCut();
								break;
							case WM_COPY:
								DoClipboardCopy();
								break;
							case EM_SETSEL:
								Owner.BaseWndProc(ref m);
								if(lockEmSetSel == 0) {
									int anchor = (int)m.WParam;
									int caret = (int)m.LParam;
									UpdateManagerPositionsFromCurrentSelections(anchor, caret);
								}
								break;
							case WM_UNDO:
							case EM_UNDO:
								DoUndo();
								break;
							case WM_CAPTURECHANGED:
								DoOnCaptureChanged();
								Owner.BaseWndProc(ref m);
								break;
							default:
								Owner.BaseWndProc(ref m);
								break;
						}
					} finally {
						--wndProcDepth;
						if(wndProcDepth == 0) {
							UpdateVisualStateIfRequired();
						}
					}
				}
				public override void ResetSelection() {
					using(ManagerChangesMarker marker = new ManagerChangesMarker(this)) {
						Manager.CursorHome(false);
						UpdateVisualStateRequire();
					}
				}
				public override bool GetCanUndo() { return Manager.CanUndo; }
				string GetDelText(string beforeText, int _startSel, int _endSel) {
					if(beforeText == null)
						return null;
					int startSel = Math.Min(_startSel, _endSel);
					if(startSel < 0)
						startSel = 0;
					int endSel = Math.Max(_startSel, _endSel);
					if(endSel > beforeText.Length)
						endSel = beforeText.Length;
					if(endSel == startSel) {
						if(endSel == beforeText.Length)
							return null;
						else
							return beforeText.Substring(0, startSel) + beforeText.Substring(endSel + 1);
					} else {
						return beforeText.Substring(0, startSel) + beforeText.Substring(endSel);
					}
				}
				string GetBackspaceText(string beforeText, int _startSel, int _endSel) {
					if(beforeText == null)
						return null;
					int startSel = Math.Min(_startSel, _endSel);
					if(startSel < 0)
						startSel = 0;
					int endSel = Math.Max(_startSel, _endSel);
					if(endSel > beforeText.Length)
						endSel = beforeText.Length;
					if(endSel == startSel) {
						if(startSel == 0)
							return null;
						else
							return beforeText.Substring(0, startSel - 1) + beforeText.Substring(endSel);
					} else {
						return beforeText.Substring(0, startSel) + beforeText.Substring(endSel);
					}
				}
				string GetChangeText(string beforeText, int _startSel, int _endSel, string change) {
					if(beforeText == null || change == null)
						return null;
					int startSel = Math.Min(_startSel, _endSel);
					if(startSel < 0)
						startSel = 0;
					int endSel = Math.Max(_startSel, _endSel);
					if(endSel > beforeText.Length)
						endSel = beforeText.Length;
					return beforeText.Substring(0, startSel) + change + beforeText.Substring(endSel);
				}
			}
		}
		#endregion
		[
		Browsable(true),
		CategoryAttribute("Mask"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public MaskProperties Mask {
			get {
				if(this._mask == null) {
					this.Mask = new MaskProperties();
				}
				return _mask;
			}
			set {
				ChangeManager(value);
				if(this._mask != null) {
					this._mask.BeforeChange -= new ChangeEventHandler(OnMaskBeforeChanged);
					this._mask.AfterChange -= new EventHandler(OnMaskAfterChanged);
				}
				this._mask = value;
				this._mask.BeforeChange += new ChangeEventHandler(OnMaskBeforeChanged);
				this._mask.AfterChange += new EventHandler(OnMaskAfterChanged);
			}
		}
		void OnMaskAfterChanged(object sender, EventArgs e) {
			ChangeManager();
		}
		void OnMaskBeforeChanged(object sender, ChangeEventArgs e) {
			switch(e.Name) {
				case "MaskType":
					ValidateManager((MaskType)e.Value, Mask.EditMask, Mask.AutoComplete, Mask.ShowPlaceHolders, Mask.PlaceHolder, Mask.SaveLiteral, Mask.IgnoreMaskBlank, Mask.Culture);
					break;
				case "EditMask":
					ValidateManager(Mask.MaskType, (string)e.Value, Mask.AutoComplete, Mask.ShowPlaceHolders, Mask.PlaceHolder, Mask.SaveLiteral, Mask.IgnoreMaskBlank, Mask.Culture);
					break;
				case "AutoComplete":
					ValidateManager(Mask.MaskType, Mask.EditMask, (AutoCompleteType)e.Value, Mask.ShowPlaceHolders, Mask.PlaceHolder, Mask.SaveLiteral, Mask.IgnoreMaskBlank, Mask.Culture);
					break;
				case "ShowPlaceHolders":
					ValidateManager(Mask.MaskType, Mask.EditMask, Mask.AutoComplete, (bool)e.Value, Mask.PlaceHolder, Mask.SaveLiteral, Mask.IgnoreMaskBlank, Mask.Culture);
					break;
				case "PlaceHolder":
					ValidateManager(Mask.MaskType, Mask.EditMask, Mask.AutoComplete, Mask.ShowPlaceHolders, (char)e.Value, Mask.SaveLiteral, Mask.IgnoreMaskBlank, Mask.Culture);
					break;
				case "SaveLiteral":
					ValidateManager(Mask.MaskType, Mask.EditMask, Mask.AutoComplete, Mask.ShowPlaceHolders, Mask.PlaceHolder, (bool)e.Value, Mask.IgnoreMaskBlank, Mask.Culture);
					break;
				case "IgnoreMaskBlank":
					ValidateManager(Mask.MaskType, Mask.EditMask, Mask.AutoComplete, Mask.ShowPlaceHolders, Mask.PlaceHolder, Mask.SaveLiteral, (bool)e.Value, Mask.Culture);
					break;
				case "Culture":
					ValidateManager(Mask.MaskType, Mask.EditMask, Mask.AutoComplete, Mask.ShowPlaceHolders, Mask.PlaceHolder, Mask.SaveLiteral, Mask.IgnoreMaskBlank, (CultureInfo)e.Value);
					break;
			}
		}
		protected virtual void AssignValueLock() {
			this._assignValueCount++;
		}
		protected virtual void AssignValueUnLock() {
			this._assignValueCount--;
		}
		protected bool IsAssignValueLock { get { return this._assignValueCount != 0; } }
		protected internal virtual void RaiseEditTextChanged() { 
			if(IsAssignValueLock) return;
			if(EditTextChanged != null) EditTextChanged(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseEditTextChanging(ChangingEventArgs e) { 
			if(IsAssignValueLock) return;
			if(EditTextChanging != null) {
				EditTextChanging(this, e);
			}
		}
		[Browsable(false)]
		public bool IsInitializeInProgress { get { return _initializeInProgress != 0; } }
		public void BeginInitMask() {
			++this._initializeInProgress;
			if(!IsInitializeInProgress)
				ChangeManager();
		}
		public void EndInitMask() {
			--this._initializeInProgress;
			if(!IsInitializeInProgress)
				ChangeManager();
		}
		[DXCategory(CategoryName.Behavior)]
		public bool IsMatch {
			get {
				return CurrentMaskStrategy.GetIsMatch();
			}
		}
		void OnManagerEditTextChanged(object sender, EventArgs e) {
			RaiseEditTextChanged();
		}
		public int MaxLengthForStringMasks;
		void OnManagerEditTextChanging(object sender, MaskChangingEventArgs e) {
			if(IsAssignValueLock)
				return;
			if(this.IsNowReadOnly) {
				e.Cancel = true;
				return;
			}
			if(this.MaxLengthForStringMasks > 0) {
				string newStringValue = e.NewValue as string;
				if(newStringValue != null && newStringValue.Length > this.MaxLengthForStringMasks) {
					string oldStringValue = e.OldValue as string;
					if(oldStringValue == null || oldStringValue.Length <= newStringValue.Length) {
						e.Cancel = true;
						return;
					}
				}
			}
			ChangingEventArgs ee = new ChangingEventArgs(e.OldValue, e.NewValue, e.Cancel);
			RaiseEditTextChanging(ee);
			e.Cancel = ee.Cancel;
			e.NewValue = ee.NewValue;
		}
		protected virtual void OnLocalEditAction(object sender, CancelEventArgs e) {
			if(this.IsNowReadOnly) {
				e.Cancel = true;
				return;
			}
		}
		protected virtual MaskManager CreateMaskManager(MaskProperties mask) {
			return mask.CreateDefaultMaskManager();
		}
		MaskManager CreateMaskManagerWithCheck(MaskProperties mask) {
			MaskManager result = CreateMaskManager(mask);
			if(result == null)
				throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, MaskExceptionsTexts.CreateManagerReturnsNull, mask.MaskType, mask.EditMask));
			return result;
		}
		void ValidateManager(MaskType maskType, string editMask, AutoCompleteType autoComplete, bool showPlaceHolders, char placeHolder, bool saveLiteral, bool ignoreMaskBlank, CultureInfo culture) {
			if(IsInitializeInProgress)
				return;
			if(maskType == MaskType.None)
				return;
			MaskProperties mask = new MaskProperties();
			mask.MaskType = maskType;
			mask.EditMask = editMask;
			mask.AutoComplete = autoComplete;
			mask.ShowPlaceHolders = showPlaceHolders;
			mask.PlaceHolder = placeHolder;
			mask.SaveLiteral = saveLiteral;
			mask.IgnoreMaskBlank = ignoreMaskBlank;
			mask.Culture = culture;
			MaskManager m = CreateMaskManagerWithCheck(mask);
		}
		void ChangeManager(MaskProperties mask) {
			MaskManager m;
			if(IsInitializeInProgress || mask.MaskType == MaskType.None) {
				m = null;
			} else {
				m = CreateMaskManagerWithCheck(mask);
				m.EditTextChanged += new EventHandler(OnManagerEditTextChanged);
				m.EditTextChanging += new MaskChangingEventHandler(OnManagerEditTextChanging);
				m.LocalEditAction += new CancelEventHandler(OnLocalEditAction);
			}
			if(this._manager != null) {
				this._manager.EditTextChanged -= new EventHandler(OnManagerEditTextChanged);
				this._manager.EditTextChanging -= new MaskChangingEventHandler(OnManagerEditTextChanging);
				this._manager.LocalEditAction -= new CancelEventHandler(OnLocalEditAction);
			}
			this._manager = m;
		}
		void ChangeManager() {
			ChangeManager(Mask);
		}
		protected string VisibleText {
			get {
				return CurrentMaskStrategy.GetVisibleText();
			}
		}
		protected virtual void ForceVisibleText(string value) {
			BaseText = value;
		}
		[DXCategory(CategoryName.Data), RefreshProperties(RefreshProperties.All)]
		public string EditText {
			get {
				return CurrentMaskStrategy.GetEditText();
			}
			set {
				AssignValueLock();
				try {
					CurrentMaskStrategy.SetEditText(value);
				} finally {
					AssignValueUnLock();
				}
			}
		}
		[Browsable(false)]
		public virtual bool IsShouldUseEditValue {
			get {
				return CurrentMaskStrategy.GetIsEditValueDifferFromEditText();
			}
		}
		protected internal void FlushPendingEditActions() {
			CurrentMaskStrategy.FlushPendingEditActions();
		}
		[DXCategory(CategoryName.Data)]
		public object EditValue {
			get {
				return CurrentMaskStrategy.GetEditValue();
			}
			set {
				AssignValueLock();
				try {
					CurrentMaskStrategy.SetEditValue(value);
				} finally {
					AssignValueUnLock();
				}
			}
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseWheel(ee);
			if(!ee.Handled && AllowMouseWheel) {
				ee.Handled = this.FireSpinRequest(ee, ee.Delta > 0);
			}
			if(!ee.Handled && OwnerEdit != null)
				ee.Handled = OwnerEdit.SuppressMouseWheel(e);
		}
		BaseEdit OwnerEdit {
			get {
				return Parent as BaseEdit;
			}
		}
		bool AllowMouseWheel {
			get {
				BaseEdit edit = Parent as BaseEdit;
				if(edit == null) return false;
				return edit.Properties.AllowMouseWheel;
			}
		}
		internal bool SelectAllOnEnter = false;
		protected override void OnEnter(EventArgs e) {
			base.OnEnter(e);
			if(SelectAllOnEnter && CanSelectAll) MaskBoxSelectAll();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(SelectAllOnEnter && e.Button == MouseButtons.Left && !isFocusedBeforeMouseDown && CanSelectAll) {
				Capture = false;
				MaskBoxSelectAll();
				Invalidate();
			}
		}
		protected virtual bool CanSelectAll { get { return true; } }
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled)
				return;
			CurrentMaskStrategy.DoAfterKeyDown(e);
		}
		protected virtual void AutoLeaveHandler() {	
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.Handled)
				return;
			CurrentMaskStrategy.DoAfterKeyPress(e);
		}
		public bool IsNeededCursorKey(Keys keyData) {
			return CurrentMaskStrategy.IsNeededCursorKey(keyData);
		}
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			if(inWMKeyDown > 0) {
				BeginInvoke(new MethodInvoker(delegate() {
					CurrentMaskStrategy.DoAfterTextChanged(e);
				}));
			} else
				CurrentMaskStrategy.DoAfterTextChanged(e);
		}
		internal bool IsImeInProgress = false, IsImeResult = true;
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			this.IsImeInProgress = false;
		}
		bool isFocusedBeforeMouseDown = false;
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref Message m) {
			if(m.Msg == MSG.WM_LBUTTONDOWN) isFocusedBeforeMouseDown = Focused;
			if(m.Msg == MSG.WM_IME_COMPOSITION) {
				long val = m.LParam.ToInt64();
				this.IsImeInProgress = true;
				this.IsImeResult = (val & (0x800 | 0x1000)) > 0;
			}
			if(m.Msg == MSG.WM_IME_ENDCOMPOSITION) {
				this.IsImeInProgress = false;
				OnEndIme();
			}
			if(m.Msg == MSG.WM_IME_STARTCOMPOSITION) {
				this.IsImeInProgress = true;
				this.IsImeResult = true;
			}
#if DEBUGWriteMaskBox
						switch(m.Msg) {
							case 0xe:
							case 0xd:
							case 0xb0:
								break;
							default:
								System.Diagnostics.Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Message: {0}", m));
								break;
						}
			System.Diagnostics.Debug.Indent();
#endif
			if(m.Msg == WM_KEYDOWN) inWMKeyDown++;
			try {
				CurrentMaskStrategy.DoWndProc(ref m);
			}
			finally {
				if(m.Msg == WM_KEYDOWN) inWMKeyDown--;
			}
#if DEBUGWriteMaskBox
			System.Diagnostics.Debug.Unindent();
#endif
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		public new bool HideSelection {
			get { return base.HideSelection; }
			set {
				if(IsImeInProgress) return;
				base.HideSelection = value;
			}
		}
		protected virtual void OnEndIme() { }
		int inWMKeyDown = 0;
		protected virtual bool IsUpdateVisualStateSupressed { get { return false; } }
		protected virtual bool FireSpinRequest(DXMouseEventArgs e, bool isUp) {
			return MaskBoxSpin(isUp);
		}
		public bool MaskBoxSpin(bool isUp) {
			return CurrentMaskStrategy.DoSpin(isUp);
		}
		protected override void OnLeave(EventArgs e) {
			this.FlushPendingEditActions();
			base.OnLeave(e);
		}
		protected virtual bool IsNowReadOnly { get { return ReadOnly || this.Capture; } }
		protected virtual string NullEditText { get { return string.Empty; } }
		public void ResetSelection() {
			CurrentMaskStrategy.ResetSelection();
		}
		int imeChar = 0;
		protected override bool ProcessKeyEventArgs(ref Message m) {
			bool isIME = (m.Msg == 0x0286); 
			if(isIME) {
				++imeChar;
				try {
					return base.ProcessKeyEventArgs(ref m);
				} finally {
					--imeChar;
				}
			} else {
				return base.ProcessKeyEventArgs(ref m);
			}
		}
		protected internal bool IsIMEProcessing { get { return this.imeChar != 0; } }
		public virtual void Reset() {
			ClearStrategies();
		}
	}
}
