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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Text;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Menu;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Data.Mask;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Text;
using DevExpress.Utils.Drawing.Helpers;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors.Native;
using DevExpress.Skins;
using DevExpress.XtraEditors.Helpers;
using System.Drawing.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemTextEdit : RepositoryItem {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemTextEdit Properties { get { return this; } }
		public static int MaxToolTipTextLength { get; set; }
		private static readonly object spin = new object();
		private static readonly object beforeShowMenu = new object();
		Image contextImage;
		ContextImageAlignment contextImageAlignment = ContextImageAlignment.Near;
		protected bool fValidateOnEnterKey;
		bool _hideSelection;
		DefaultBoolean _allowNullInput;
		Padding maskBoxPadding;
		protected CharacterCasing fCharacterCasing;
		protected int fMaxLength;
		protected char fPasswordChar;
		bool useSystemPasswordChar;
		protected MaskData fMaskData;
		MaskProperties mask;
		string nullValuePrompt;
		bool nullValuePromptShowForEmptyValue;
		bool showNullValuePromptWhenFocused = false;
		bool useReadOnlyAppearance = true;
		string xlsxFormatString;
		static RepositoryItemTextEdit() {
			MaxToolTipTextLength = 500;
		}
		public RepositoryItemTextEdit() {
			this.maskBoxPadding = Padding.Empty;
			this.nullValuePromptShowForEmptyValue = false;
			this.nullValuePrompt = string.Empty;
			this.xlsxFormatString = string.Empty;
			this.fValidateOnEnterKey = false;
			this._hideSelection = true;
			this._allowNullInput = DefaultBoolean.Default;
			this.useSystemPasswordChar = false;
			this.fPasswordChar = '\0';
			this.fMaxLength = 0;
			this.fCharacterCasing = CharacterCasing.Normal;
			this.fMaskData = new MaskData();
			this.fMaskData.AfterChange += new ChangeEventHandler(OnMaskDataChanged);
			this.mask = CreateMaskProperties();
			this.mask.RepositoryItem = this;
			this.mask.AfterChange += new EventHandler(OnMaskChanged);
		}
		public override DevExpress.XtraPrinting.IVisualBrick GetBrick(PrintCellHelperInfo info) {
			XtraPrinting.TextBrick brick = CreateTextBrick(info) as XtraPrinting.TextBrick;
			brick.Text = CalcPasswordCharMaskedText(info.DisplayText);
			brick.XlsxFormatString = XlsxFormatString;
			return brick;
		}
		protected virtual MaskProperties CreateMaskProperties() {
			return new MaskProperties();
		}
		protected override void Dispose(bool disposing) {
			if(this.mask != null) 
			this.mask.AfterChange -= new EventHandler(OnMaskChanged);
			base.Dispose(disposing);
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemTextEdit source = item as RepositoryItemTextEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.contextImage = source.ContextImage;
				this.contextImageAlignment = source.ContextImageAlignment;
				this.maskBoxPadding = source.MaskBoxPadding;
				this.fCharacterCasing = source.CharacterCasing;
				this.nullValuePrompt = source.NullValuePrompt;
				this.nullValuePromptShowForEmptyValue = source.NullValuePromptShowForEmptyValue;
				this._hideSelection = source.HideSelection;
				this._allowNullInput = source.AllowNullInput;
				this.fMaxLength = source.MaxLength;
				this.fPasswordChar = source.PasswordChar;
				this.useSystemPasswordChar = source.UseSystemPasswordChar;
				this.fValidateOnEnterKey = source.ValidateOnEnterKey;
				this.useReadOnlyAppearance = source.UseReadOnlyAppearance;
				this.Mask.Assign(source.Mask);
			} finally {
				EndUpdate();
			}
			Events.AddHandler(spin, source.Events[spin]);
			Events.AddHandler(beforeShowMenu, source.Events[beforeShowMenu]);
		}
		protected internal virtual bool UseMaskBox { get { return true; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete(ObsoleteText.SRObsoleteMaskData)]
		public  MaskData MaskData {
			get { return fMaskData; }
		}
		[DXCategory(CategoryName.Format), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditMask"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.XtraEditors.Design.MaskDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual MaskProperties Mask {
			get { return mask; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditUseReadOnlyAppearance"),
#endif
 DefaultValue(true)]
		public virtual bool UseReadOnlyAppearance {
			get { return useReadOnlyAppearance; }
			set {
				if(UseReadOnlyAppearance == value) return;
				useReadOnlyAppearance = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditHideSelection"),
#endif
 DefaultValue(true)]
		public virtual bool HideSelection {
			get { return _hideSelection; }
			set {
				if(HideSelection == value) return;
				_hideSelection = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(false), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditShowNullValuePromptWhenFocused")
#else
	Description("")
#endif
]
		public bool ShowNullValuePromptWhenFocused {
			get { return showNullValuePromptWhenFocused; }
			set { showNullValuePromptWhenFocused = value; }
		}
		[Localizable(true), DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditNullValuePrompt"),
#endif
 DefaultValue("")]
		public virtual string NullValuePrompt {
			get { return nullValuePrompt; }
			set {
				if(NullValuePrompt == value) return;
				nullValuePrompt = value;
				OnPropertiesChanged();
			}
		}
		[Localizable(true), DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditNullValuePromptShowForEmptyValue"),
#endif
 DefaultValue(false)]
		public virtual bool NullValuePromptShowForEmptyValue {
			get { return nullValuePromptShowForEmptyValue; }
			set {
				if(NullValuePromptShowForEmptyValue == value) return;
				nullValuePromptShowForEmptyValue = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditValidateOnEnterKey"),
#endif
 DefaultValue(false)]
		public virtual bool ValidateOnEnterKey {
			get { return fValidateOnEnterKey; }
			set {
				if(ValidateOnEnterKey == value) return;
				fValidateOnEnterKey = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Always)]
		public override bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditCharacterCasing"),
#endif
 DefaultValue(CharacterCasing.Normal)]
		public virtual CharacterCasing CharacterCasing {
			get { return fCharacterCasing; }
			set {
				if(CharacterCasing == value) return;
				fCharacterCasing = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditMaxLength"),
#endif
 DefaultValue(0)]
		public virtual int MaxLength {
			get { return fMaxLength; }
			set {
				if(MaxLength == value) return;
				fMaxLength = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual char GetPasswordCharCore(bool forPrinting) {
				if(!IsPasswordBox) return '\0';
				if(!UseSystemPasswordChar) return PasswordChar;
			return forPrinting || Enabled ? '*' : '\u25CF'; 
		}
		protected internal virtual bool IsPasswordBox { get { return PasswordChar != 0 || UseSystemPasswordChar; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditPasswordChar"),
#endif
 DefaultValue('\0')]
		public virtual char PasswordChar {
			get { return fPasswordChar; }
			set {
				if(PasswordChar == value) return;
				fPasswordChar = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditUseSystemPasswordChar"),
#endif
 DefaultValue(false)]
		public virtual bool UseSystemPasswordChar {
			get { return useSystemPasswordChar; }
			set {
				if(UseSystemPasswordChar == value) return;
				useSystemPasswordChar = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditXlsxFormatString"),
#endif
 DefaultValue("")]
		public string XlsxFormatString {
			get {
				return xlsxFormatString;
			}
			set {
				if(xlsxFormatString == value)
					return;
				xlsxFormatString = value;
				OnPropertiesChanged();
			}
		}
		protected string CalcPasswordCharMaskedText(string text) {
			if(!IsPasswordBox) return text;
			else return new string(GetPasswordCharCore(true), text.Length);
		}
		[Browsable(false)]
		public new TextEdit OwnerEdit { get { return base.OwnerEdit as TextEdit; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "TextEdit"; } }
		protected override void OnEnabledChanged() {
			base.OnEnabledChanged();
			if(OwnerEdit != null)
				OwnerEdit.OnTextEditPropertiesChanged();
		}
		void OnMaskDataChanged(object sender, ChangeEventArgs e) {
			Mask.Assign(this.fMaskData);
		}
		protected virtual void OnMaskChanged(object sender, EventArgs e) {
			if(UseMaskBox)
				OnPropertiesChanged();
		}
		protected internal virtual bool AllowValidateOnEnterKey { get { return true; } }
		protected internal virtual bool IsNowReadOnly { get { return ReadOnly; } }
		protected virtual bool AllowCheckExtraKeys { get { return OwnerEdit != null && OwnerEdit.IsMaskBoxAvailable; } }
		static Keys[] navKeys = new Keys[] { Keys.Left, Keys.Right, Keys.Home, Keys.End };
		protected Keys MenuHotKey { get { return Keys.F10 | Keys.Shift; } }
		protected override bool IsNeededKeyCore(Keys keyData) {
			if(AllowCheckExtraKeys) {
				switch(keyData & Keys.KeyCode) {
					case Keys.Up:
					case Keys.Down:
					case Keys.Left:
					case Keys.Right:
						return OwnerEdit.IsNeededCursorKey(keyData);
				}
				if(keyData == MenuHotKey) return true;
			}
			Keys keyCode = keyData & (~Keys.Modifiers);
			if(base.IsNeededKeyCore(keyData)) return true;
			if(!AllowCheckExtraKeys) return false;
			if(Array.IndexOf(navKeys, keyCode) != -1) return true;
			return false;
		}
		protected internal override bool IsNeededChar(char ch) {
			bool res = base.IsNeededChar(ch);
			if(res) return true;
			if(IsNeededTextKeys) {
				if(ch == 8 || (ch > 31)) return true;
			}
			return res;
		}
		protected virtual bool IsNeededTextKeys { get { return OwnerEdit == null || OwnerEdit.MaskBox != null; } }
		protected internal override bool NeededKeysContains(Keys key) {
			if(IsNeededTextKeys) {
				switch(key) {
					case Keys.F2:
					case Keys.A:
					case Keys.Add:
					case Keys.B:
					case Keys.Back:
					case Keys.C:
					case Keys.Clear:
					case Keys.D:
					case Keys.D0:
					case Keys.D1:
					case Keys.D2:
					case Keys.D3:
					case Keys.D4:
					case Keys.D5:
					case Keys.D6:
					case Keys.D7:
					case Keys.D8:
					case Keys.D9:
					case Keys.Decimal:
					case Keys.Delete:
					case Keys.Divide:
					case Keys.E:
					case Keys.End:
					case Keys.F:
					case Keys.F20:
					case Keys.G:
					case Keys.H:
					case Keys.Home:
					case Keys.I:
					case Keys.Insert:
					case Keys.J:
					case Keys.K:
					case Keys.L:
					case Keys.Left:
					case Keys.M:
					case Keys.Multiply:
					case Keys.N:
					case Keys.NumPad0:
					case Keys.NumPad1:
					case Keys.NumPad2:
					case Keys.NumPad3:
					case Keys.NumPad4:
					case Keys.NumPad5:
					case Keys.NumPad6:
					case Keys.NumPad7:
					case Keys.NumPad8:
					case Keys.NumPad9:
					case Keys.Alt:
					case (Keys.RButton | Keys.ShiftKey):
					case Keys.O:
					case Keys.Oem8:
					case Keys.OemBackslash:
					case Keys.OemCloseBrackets:
					case Keys.Oemcomma:
					case Keys.OemMinus:
					case Keys.OemOpenBrackets:
					case Keys.OemPeriod:
					case Keys.OemPipe:
					case Keys.Oemplus:
					case Keys.OemQuestion:
					case Keys.OemQuotes:
					case Keys.OemSemicolon:
					case Keys.Oemtilde:
					case Keys.P:
					case Keys.Q:
					case Keys.R:
					case Keys.Right:
					case Keys.S:
					case Keys.Space:
					case Keys.Subtract:
					case Keys.T:
					case Keys.U:
					case Keys.V:
					case Keys.W:
					case Keys.X:
					case Keys.Y:
					case Keys.Z:
						return true;
				}
			}
			return base.NeededKeysContains(key);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditAllowNullInput"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(DefaultBoolean.Default), RefreshProperties(RefreshProperties.All)]
		public virtual DefaultBoolean AllowNullInput {
			get { return _allowNullInput; }
			set {
				if(AllowNullInput == value) return;
				_allowNullInput = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool IsNullInputAllowed {
			get {
				switch(AllowNullInput) {
					case DefaultBoolean.False:
						return false;
					case DefaultBoolean.True:
						return true;
					default:
						if(OwnerEdit == null)
							return false;
						return OwnerEdit.InplaceType == InplaceType.Grid;
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditSpin"),
#endif
 DXCategory(CategoryName.Events)]
		public event SpinEventHandler Spin {
			add { this.Events.AddHandler(spin, value); }
			remove { this.Events.RemoveHandler(spin, value); }
		}
		protected internal virtual void RaiseSpin(SpinEventArgs e) {
			SpinEventHandler handler = (SpinEventHandler)this.Events[spin];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditBeforeShowMenu"),
#endif
 DXCategory(CategoryName.Events)]
		public event BeforeShowMenuEventHandler BeforeShowMenu {
			add { this.Events.AddHandler(beforeShowMenu, value); }
			remove { this.Events.RemoveHandler(beforeShowMenu, value); }
		}
		protected internal virtual void RaiseBeforeShowMenu(BeforeShowMenuEventArgs e) {
			BeforeShowMenuEventHandler handler = (BeforeShowMenuEventHandler)this.Events[beforeShowMenu];
			if(handler != null) handler(GetEventSender(), e);
		}
		MaskManager _useMaskAsDisplayFormatCachedManager = null;
		MaskProperties _useMaskAsDisplayFormatCachedMask = null;
		object _useMaskAsDisplayFormatCachedEditValue = null;
		protected internal override void RaiseCustomDisplayText(CustomDisplayTextEventArgs e) {
			if(this.Mask.MaskType != MaskType.None && this.Mask.UseMaskAsDisplayFormat) {
				if(IsNullValue(e.Value)) {
					if(this.NullText != null && this.NullText.Length > 0) {
						e.DisplayText = this.NullText;
					} else {
						e.DisplayText = this.GetNullEditText();
					}
				} else {
					try {
						bool setValue = false;
						if(_useMaskAsDisplayFormatCachedManager == null || _useMaskAsDisplayFormatCachedMask == null || !Equals(_useMaskAsDisplayFormatCachedMask, this.Mask)) {
							this._useMaskAsDisplayFormatCachedMask = new MaskProperties(this.Mask);
							if(OwnerEdit == null) {
								this._useMaskAsDisplayFormatCachedManager = this.Mask.CreateDefaultMaskManager();
							} else {
								this._useMaskAsDisplayFormatCachedManager = OwnerEdit.CreateMaskManager(this.Mask);
							}
							setValue = true;
						} else if(!Equals(this._useMaskAsDisplayFormatCachedEditValue, e.Value)) {
							setValue = true;
						}
						if(setValue) {
							if(!this._useMaskAsDisplayFormatCachedManager.IsEditValueAssignedAsFormattedText) {
								this._useMaskAsDisplayFormatCachedManager.SetInitialEditValue(e.Value);
							} else {
								this._useMaskAsDisplayFormatCachedManager.SetInitialEditText(e.DisplayText);
							}
							this._useMaskAsDisplayFormatCachedEditValue = e.Value;
						}
						e.DisplayText = this._useMaskAsDisplayFormatCachedManager.DisplayText;
					} catch { }
				}
			}
			base.RaiseCustomDisplayText(e);
		}
		protected internal virtual bool IsEmptyValue(object editValue) {
			string displayText = GetDisplayText(DisplayFormat, editValue);
			return string.IsNullOrEmpty(displayText);
		}
		protected override bool PostponeDelayedEditValueTimer {
			get {
				if(OwnerEdit != null && OwnerEdit.MaskBox != null && OwnerEdit.MaskBox.Capture)
					return true;
				return base.PostponeDelayedEditValueTimer;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Padding MaskBoxPadding {
			get { return maskBoxPadding; }
			set {
				if(MaskBoxPadding == value) return;
				maskBoxPadding = value;
				LayoutChanged();
			}
		}
		[DefaultValue(null), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTextEditContextImage"),
#endif
 DXCategory(CategoryName.Appearance), 
		Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image ContextImage {
			get { return contextImage; }
			set {
				if(ContextImage == value) return;
				contextImage = value;
				LayoutChanged();
			}
		}
		[DefaultValue(ContextImageAlignment.Near),  DXCategory(CategoryName.Appearance)]
		public ContextImageAlignment ContextImageAlignment {
			get { return contextImageAlignment; }
			set {
				if(ContextImageAlignment == value) return;
				contextImageAlignment = value;
				LayoutChanged();
			}
		}
	}
}
namespace DevExpress.XtraEditors {
	public enum ContextImageAlignment { Near, Far }
	[ToolboxItem(false)]
	public class TextBoxMaskBox : MaskBox, IMouseWheelSupport {
		protected override Size DefaultSize { get { return new Size(300, 16); } }	
		protected override CreateParams CreateParams { 
			get {
				System.Windows.Forms.CreateParams res = base.CreateParams;
				res.ClassStyle |= BaseControl.CS_VREDRAW | BaseControl.CS_HREDRAW;
				return res;
			}
		}
		protected override bool CanSelectAll { 
			get {
				if(OwnerEdit.IsShowNullValuePrompt()) return false;
				return base.CanSelectAll; 
			} 
		}
		public void RaiseValidating(CancelEventArgs e) { 
			FlushPendingEditActions();
			this.OnValidating(e);
		}
		public override void Reset() {
			this.EditValue = this.EditValue;
			base.Reset();
		}
		TextEdit ownerEdit;
		bool ignoreMask = false;
		string displayText = string.Empty;
		public TextBoxMaskBox(TextEdit ownerEdit) {
			this.ownerEdit = ownerEdit;
			this.TabIndex = 0;
			this.BorderStyle = BorderStyle.None;
			this.AutoSize = false;
			this.Multiline = false;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		protected virtual bool AllowSmartMouseWheel { get { return OwnerEdit != null && OwnerEdit.AllowSmartMouseWheel; } }
		protected sealed override void OnMouseWheel(MouseEventArgs ev) {
			if(lockSmartMouse == 0 && AllowSmartMouseWheel && XtraForm.IsAllowSmartMouseWheel(this, ev)) {
				return;
			}
			OnMouseWheelCore(ev);
		}
		bool AllowMouseWheel {
			get {
				TextEdit edit = OwnerEdit as TextEdit;
				if(edit == null) return true;
				return edit.Properties.AllowMouseWheel;
			}
		}
		int lockSmartMouse;
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			if(lockSmartMouse != 0 || !AllowMouseWheel|| !AllowSmartMouseWheel) return;
			try {
				this.lockSmartMouse++;
				if(IsHandleCreated) {
					Point screen = PointToScreen(e.Location);
					DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(Handle,
						DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEWHEEL,
						new IntPtr((e.Delta << 16) + 0),
						new IntPtr(screen.X + (screen.Y << 16)));
				}
				else {
					OnMouseWheelCore(e);
				}
			}
			finally {
				this.lockSmartMouse--;
			}
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseWheel(e);
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(OwnerEdit == null || OwnerEdit.DXAccessible == null || OwnerEdit.DXAccessible.Text == null) 
				return base.CreateAccessibilityInstance();
			return OwnerEdit.DXAccessible.Text.Accessible;
		}
		protected override bool IsInputKey(Keys keyData) {
			bool res = base.IsInputKey(keyData) || OwnerEdit.IsInputKeyCore(keyData);
			return res;
		}
		protected override void ScaleCore(float dx, float dy) { }
		void UpdateSelection() {
			System.Reflection.FieldInfo fi = typeof(TextBox).GetField("selectionSet", System.Reflection.BindingFlags.SetField | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			if(fi != null) fi.SetValue(this, true);
		}
		protected override void OnGotFocus(EventArgs e) {
			UpdateSelection();
			base.OnGotFocus(e);
		}
		protected override bool IsUpdateVisualStateSupressed { get { return this.ignoreMask; } }
		protected override void ForceVisibleText(string value) {
			if(this.ignoreMask)
				value = this.displayText;
			base.ForceVisibleText(value);
		}
		public virtual void OverrideDisplayText(string overrideText) {
			this.ignoreMask = true;
			this.displayText = overrideText;
			ForceVisibleText(overrideText);
		}
		[Browsable(false)]
		public virtual TextEdit OwnerEdit { get { return ownerEdit; } }
		protected override void OnLocalEditAction(object sender, CancelEventArgs e) {
			base.OnLocalEditAction(sender, e);
			if(e.Cancel)
				return;
			OwnerEdit.PendingEditActionPerfomed();
		}
		public virtual object GetEditValue() {
			return IsShouldUseEditValue ? EditValue : EditText;
		}
		public virtual void SetEditValue(object editValue, string displayText, bool ignoreMask) {
			AssignValueLock();
			try {
				this.ignoreMask = ignoreMask;
				if(ignoreMask) {
					this.displayText = displayText;
				}
				bool keepSelection = GetIsSelectAllMode();
				if(IsShouldUseEditValue) {
					EditValue = editValue;
				} else {
					EditText = displayText;
				}
				if(ignoreMask) {
					ForceVisibleText(displayText);
				}
				if(!OwnerEdit.IsLoading && !OwnerEdit.IsDesignMode && IsHandleCreated) {
					if(keepSelection) {
						MaskBoxSelectAll();
					}
				}
			}
			finally {
				AssignValueUnLock();
			}
		}
		protected override void CreateHandle() {
			try {
				base.CreateHandle();
			}
			catch {
				if(OwnerEdit == null || OwnerEdit.InplaceType != InplaceType.Grid) throw;
			}
		}
		public virtual bool IsNeededKey(Keys keyData) {
			if(OwnerEdit != null) return OwnerEdit.IsNeededKey(new KeyEventArgs(keyData));
			return false;
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			if(OwnerEdit != null) OwnerEdit.MouseHere = true;
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(OwnerEdit != null) OwnerEdit.CheckMouseHere();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if((keyData & (~Keys.Modifiers)) == Keys.Tab) {
				if((keyData & Keys.Modifiers) == Keys.Control) {
					if(!OwnerEdit.Properties.IsNeededKey(keyData) && OwnerEdit.InplaceType == InplaceType.Grid) {
						return OwnerEdit.ProcessDialogKeyCore(keyData);
					}
				}
			}
			return base.ProcessDialogKey(keyData);
		}
		const int WM_CONTEXTMENU = 0x007B;
		bool IsOwnerEditMemoEdit { get { return OwnerEdit != null && OwnerEdit is MemoEdit; } }
		bool UseOptimizedRendering {
			get {
				if(!IsOwnerEditMemoEdit) return false;
				return ((MemoEdit)OwnerEdit).UseOptimizedRendering;
			}
		}
		protected override void WndProc(ref Message msg) {
			if(msg.Msg == WM_CONTEXTMENU) {
				Point pt = new Point(msg.LParam.ToInt32());
				pt = pt == new Point(-1, -1) ? pt : PointToClient(Control.MousePosition);
				if(OwnerEdit != null && OwnerEdit.ShowMenu(pt)) {
					return;
				}
			}
			if(OwnerEdit != null && OwnerEdit.OnMaskBoxPreWndProc(ref msg)) return;
			base.WndProc(ref msg);
			if(OwnerEdit == null) return;
			if(msg.Msg == 0x0030) {
				if(Multiline) {
					NativeMethods.SendMessage(Handle,
						0xd3, new IntPtr(0x1 | 0x2), IntPtr.Zero);
				}
			}
			const int WM_NCMOUSEHOVER = 0x02A0, WM_NCMOUSELEAVE = 0x02A2;
			if(msg.Msg == WM_NCMOUSEHOVER) OwnerEdit.MouseHere = true;
			if(msg.Msg == WM_NCMOUSELEAVE) OwnerEdit.CheckMouseHere();
			OwnerEdit.OnMaskBoxWndProc(ref msg);
			if(UseOptimizedRendering) {
				if(msg.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_ERASEBKGND) {
					Message wmPaintMsg = new Message();
					wmPaintMsg.HWnd = msg.HWnd;
					wmPaintMsg.Msg = DevExpress.Utils.Drawing.Helpers.MSG.WM_PAINT;
					base.WndProc(ref wmPaintMsg);
				}
			}
		}
		protected override MaskManager CreateMaskManager(MaskProperties mask) {
			MaskManager result = null;
			if(OwnerEdit != null)
				result = OwnerEdit.CreateMaskManager(mask);
			if(result == null)
				result = base.CreateMaskManager(mask);
			return result;
		}
		protected override bool FireSpinRequest(DXMouseEventArgs e, bool isUp) {
			if(OwnerEdit != null)
				return OwnerEdit.FireSpinRequest(e, isUp);
			else
				return base.FireSpinRequest(e, isUp);
		}
		protected override bool IsNowReadOnly {
			get {
				if(base.IsNowReadOnly)
					return true;
				if(OwnerEdit == null)
					return false;
				return OwnerEdit.Properties.IsNowReadOnly;
			}
		}
		protected override string NullEditText { get { return OwnerEdit.Properties.GetNullEditText(); } }
		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
			base.OnPreviewKeyDown(e);
			if(OwnerEdit != null)
				OwnerEdit.DoPreviewKeyDown(e);
		}
		internal void UpdateHideSelection() {
			if(OwnerEdit == null) return;
			if(IsImeInProgress) return;
			if(OwnerEdit.Properties.HideSelection == HideSelection) return;
			HideSelection = OwnerEdit.Properties.HideSelection;
		}
		protected override void OnHideSelectionChanged(EventArgs e) {
			base.OnHideSelectionChanged(e);
		}
		protected override void OnEndIme() {
			base.OnEndIme();
			UpdateHideSelection();
			if(OwnerEdit != null) OwnerEdit.OnEndIme();
		}
	}
	[DXToolboxItem(DXToolboxItemKind.Free),
	DefaultBindingPropertyEx("Text"),
	Designer("DevExpress.XtraEditors.Design.TextEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	Description("Provides the text editing feature."),
	ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagFilter(typeof(TextEditFilter)), SmartTagAction(typeof(TextEditActions), "EditMask", "Change Mask", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "TextEdit")
]
	public class TextEdit : BaseEdit, IDXMenuSupport, IMouseWheelSupport {
		bool mouseHere = false, tabStop, setSelection = false;
		int maskBoxPropertiesUpdate = 0;
		int lockTabStop = 0;
		TextBoxMaskBox _maskBox;
		public TextEdit() {
			SetStyle(ControlStyles.ContainerControl, true);
			this.tabStop = base.TabStop;
			CreateMaskBox();
		}
		protected override bool AllowPaintBackground {
			get {
				return base.AllowPaintBackground || LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP;
			}
		}
		public override Color BackColor {
			get {
				return InplaceType == InplaceType.Standalone && LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP ? Color.Transparent: base.BackColor;
			}
			set {
				base.BackColor = value;
			}
		}
		protected override Size DefaultSize { 
			get { 
				Size size = new Size(100, 20);
				return size;
			} 
		}
		protected override Size CalcSizeableMaxSize() {
			if(Properties.AutoHeight) return new Size(AutoSizeInLayoutControl ? base.CalcSizeableMaxSize().Width : 0, CalcMinHeight());
			return base.CalcSizeableMaxSize();
		}
		protected override Size CalcSizeableMinSize() {
			return new Size(base.CalcSizeableMinSize().Width, CalcMinHeight());
		}
		protected internal new DevExpress.Accessibility.TextEditAccessible DXAccessible { 
			get {
				return base.DXAccessible as DevExpress.Accessibility.TextEditAccessible;
			}
		}
		DXPopupMenu menu = null;
		[ThreadStatic]
		static ImageCollection _imageList = null;
		static ImageCollection ImageList {
			get {
				if(_imageList == null)
					_imageList = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Images.TextEditMenu.png", typeof(TextEdit).Assembly, new Size(16, 16), Color.Empty);
				return _imageList;
			}
		}
		protected internal override Control InnerControl { get { return MaskBox; } }
		void OnUndoMenuItemSelected(object sender, EventArgs e) {
			this.Undo();
		}
		void OnCutMenuItemSelected(object sender, EventArgs e) {
			this.Cut();
		}
		void OnCopyMenuItemSelected(object sender, EventArgs e) {
			this.Copy();
		}
		void OnPasteMenuItemSelected(object sender, EventArgs e) {
			this.Paste();
		}
		const int WM_CLEAR = 0x303;
		void OnDeleteMenuItemSelected(object sender, EventArgs e) {
			if(IsMaskBoxAvailable && MaskBox != null && MaskBox.IsHandleCreated) {
				NativeMethods.SendMessage(MaskBox.Handle, WM_CLEAR, IntPtr.Zero, IntPtr.Zero);
			} else {
			this.SelectedText = string.Empty;
		}
		}
		void OnSelectAllMenuItemSelected(object sender, EventArgs e) {
			this.SelectAll();
		}
		protected class DXMenuItemTextEdit : DXMenuItem {
			public DXMenuItemTextEdit(StringId id, EventHandler click, Image image) : 
				base(Localizer.Active.GetLocalizedString(id), click, image) {
				this.Tag = id;
			}
			MenuItemUpdateElement updateElement;
			public MenuItemUpdateElement UpdateElement {
				get { return updateElement; }
				set { updateElement = value; }
			}
		}
		protected delegate void MenuItemUpdateHandler(DXMenuItem sender, EventArgs e);
		protected class MenuItemUpdateElement {
			public readonly MenuItemUpdateHandler UpdateMenuItemDelegate;
			public readonly DXMenuItemTextEdit Item;
			public MenuItemUpdateElement(DXMenuItemTextEdit item, MenuItemUpdateHandler updateMenuItemDelegate) {
				this.Item = item;
				this.UpdateMenuItemDelegate = updateMenuItemDelegate;
			}
			public void DoUpdate() {
				if(UpdateMenuItemDelegate != null)
					UpdateMenuItemDelegate.DynamicInvoke(new object[] {Item, EventArgs.Empty});
			}
		}
		void OnUndoMenuItemUpdate(DXMenuItem sender, EventArgs e) {
			sender.Enabled = !Properties.ReadOnly && this.CanUndo;
		}
		void OnCutMenuItemUpdate(DXMenuItem sender, EventArgs e) {
			sender.Enabled = !Properties.ReadOnly && !Properties.IsPasswordBox && this.SelectedText.Length > 0;
		}
		void OnCopyMenuItemUpdate(DXMenuItem sender, EventArgs e) {
			sender.Enabled = !Properties.IsPasswordBox && this.SelectedText.Length > 0;
		}
		void OnPasteMenuItemUpdate(DXMenuItem sender, EventArgs e) {
			sender.Enabled = !Properties.ReadOnly && DevExpress.XtraEditors.Mask.MaskBox.GetClipboardText().Length > 0;
		}
		void OnDeleteMenuItemUpdate(DXMenuItem sender, EventArgs e) {
			sender.Enabled = !Properties.ReadOnly && this.SelectedText.Length > 0;
		}
		void OnSelectAllMenuItemUpdate(DXMenuItem sender, EventArgs e) {
			sender.Enabled = MaskBox != null && !MaskBox.GetIsSelectAllMode();
		}
		protected virtual DXPopupMenu CreateMenu() {
			const int pictureIndexUndo = 0;
			const int pictureIndexCut = 1;
			const int pictureIndexCopy = 2;
			const int pictureIndexPaste = 3;
			const int pictureIndexDelete = 4;
			DXPopupMenu result = new DXPopupMenu();
			DXMenuItemTextEdit item;
			item = new DXMenuItemTextEdit(StringId.TextEditMenuUndo, new EventHandler(OnUndoMenuItemSelected), ImageList.Images[pictureIndexUndo]);
			item.UpdateElement = new MenuItemUpdateElement(item, new MenuItemUpdateHandler(OnUndoMenuItemUpdate));
			result.Items.Add(item);
			item = new DXMenuItemTextEdit(StringId.TextEditMenuCut, new EventHandler(OnCutMenuItemSelected), ImageList.Images[pictureIndexCut]);
			item.UpdateElement = new MenuItemUpdateElement(item, new MenuItemUpdateHandler(OnCutMenuItemUpdate));
			item.BeginGroup = true;
			result.Items.Add(item);
			item = new DXMenuItemTextEdit(StringId.TextEditMenuCopy, new EventHandler(OnCopyMenuItemSelected), ImageList.Images[pictureIndexCopy]);
			item.UpdateElement = new MenuItemUpdateElement(item, new MenuItemUpdateHandler(OnCopyMenuItemUpdate));
			result.Items.Add(item);
			item = new DXMenuItemTextEdit(StringId.TextEditMenuPaste, new EventHandler(OnPasteMenuItemSelected), ImageList.Images[pictureIndexPaste]);
			item.UpdateElement = new MenuItemUpdateElement(item, new MenuItemUpdateHandler(OnPasteMenuItemUpdate));
			result.Items.Add(item);
			item = new DXMenuItemTextEdit(StringId.TextEditMenuDelete, new EventHandler(OnDeleteMenuItemSelected), ImageList.Images[pictureIndexDelete]);
			item.UpdateElement = new MenuItemUpdateElement(item, new MenuItemUpdateHandler(OnDeleteMenuItemUpdate));
			result.Items.Add(item);
			item = new DXMenuItemTextEdit(StringId.TextEditMenuSelectAll, new EventHandler(OnSelectAllMenuItemSelected), null);
			item.UpdateElement = new MenuItemUpdateElement(item, new MenuItemUpdateHandler(OnSelectAllMenuItemUpdate));
			item.BeginGroup = true;
			result.Items.Add(item);
			return result;
		}
		protected virtual DXPopupMenu Menu {
			get { 
				if(menu == null) {
					menu = CreateMenu();
				}
				return menu;
			}
		}
		protected virtual void UpdateMenu() {
			foreach(DXMenuItem item in Menu.Items) {
				DXMenuItemTextEdit itemTextEdit = item as DXMenuItemTextEdit;
				if(itemTextEdit == null) continue;
				MenuItemUpdateElement updateElement = itemTextEdit.UpdateElement;
				if(updateElement == null)
					continue;
				updateElement.DoUpdate();
			}
		}
		int GetTextAscentHeight() {
			int fontHeight;
			GraphicsInfo gInfo = new GraphicsInfo();
			Graphics g = gInfo.AddGraphics(null);
			try {
				fontHeight = TextUtils.GetFontAscentHeight(g, ViewInfo.PaintAppearance.Font);
			}
			finally {
				gInfo.ReleaseGraphics();
				gInfo = null;
			}
			return fontHeight;
		}
		protected internal virtual Point UpdateMenuCoords(Point pos) {
			if(pos != new Point(-1, -1) || MaskBox == null || MaskBox.Visible == false) return pos;
			pos = MaskBox.GetPositionFromCharIndex(MaskBox.MaskBoxSelectionStart + MaskBox.MaskBoxSelectionLength);
			pos.Offset(1, 16);
			return pos;
		}
		protected internal bool ShowMenu(Point pos) {
			if(Properties.ContextMenu != null || MaskBox == null) return false;
			if(Properties.ContextMenuStrip != null) return false;
			MaskBox.Select();
			UpdateMenu();
			BeforeShowMenuEventArgs e = new BeforeShowMenuEventArgs(Menu, pos);
			Properties.RaiseBeforeShowMenu(e);
			pos = e.Location;
			if(e.Location == new Point(-1, -1)) pos = UpdateMenuCoords(pos);
			MenuManagerHelper.ShowMenu(Menu, Properties.LookAndFeel, MenuManager, this, pos);
			if(e.RestoreMenu) menu = null;
			return true;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TextEditTabStop"),
#endif
 DefaultValue(true)]
		public override bool TabStop { 
			get { return tabStop; } 
			set {
				if(TabStop == value) return;
				this.tabStop = value;
				OnTabStopChanged(EventArgs.Empty);
			}
		}
		protected internal bool MouseHere { 
			get { return mouseHere; }
			set {
				if(MouseHere == value) return;
				mouseHere = value;
				if(MouseHere) {
					base.OnMouseEnter(EventArgs.Empty);
				} else {
					base.OnMouseLeave(EventArgs.Empty);
				}
			}
		}
		event DXMenuWndProcHandler IDXMenuSupport.WndProc {
			add { 
				wndProcHandler += value; 
			}
			remove {
				if(wndProcHandler != null)
					wndProcHandler -= value; 
			}
		}
		DXPopupMenu IDXMenuSupport.Menu { get { return Menu; } }
		void IDXMenuSupport.ShowMenu(Point pos) { ShowMenu(pos); }
		event DXMenuWndProcHandler wndProcHandler;
		const int WM_LBUTTONDOWN = 0x0201, WM_LBUTTONDBLCLK = 0x0203;
		protected override void WndProc(ref Message msg) {
			if(wndProcHandler != null) wndProcHandler(this, ref msg);
			if(msg.Msg == WM_LBUTTONDBLCLK || msg.Msg == WM_LBUTTONDOWN) {
				bool prevSelectable = GetStyle(ControlStyles.Selectable);
				try {
					if(MaskBox != null && MaskBox.ContainsFocus) SetStyle(ControlStyles.Selectable, false);
					base.WndProc(ref msg);
				} finally {
					SetStyle(ControlStyles.Selectable, prevSelectable);
				}
			} else {
				base.WndProc(ref msg);
			}
		}
		protected internal bool ProcessDialogKeyCore(Keys keyData) {
			return ProcessDialogKey(keyData);
		}
		bool innerControlBufferedPaint = false;
		void DrawOnGlass(Message m) {
			if(MaskBox == null) return; 
			IntPtr dc = NativeMethods.GetDC(m.HWnd);
			NativeVista.PaintControl(m.HWnd, dc, MaskBox.ClientRectangle, false);
			innerControlBufferedPaint = false;
			NativeMethods.ReleaseDC(m.HWnd, dc);
		}
		const int WM_CONTEXTMENU = 0x007B;
		protected virtual void OnGlassMaskBoxPreWndProc(ref Message msg) {
			if(!innerControlBufferedPaint) {
				if(msg.Msg == MSG.WM_KEYUP || msg.Msg == MSG.WM_IME_NOTIFY || msg.Msg == MSG.WM_USER7441) {
					innerControlBufferedPaint = true;
					DrawOnGlass(msg);
					return;
				}
			}
			return;
		}
		protected internal virtual bool OnMaskBoxPreWndProc(ref Message msg) {
			CheckDoubleClick(ref msg);
			ISupportGlassRegions gr = Parent as ISupportGlassRegions;
			if(gr != null && gr.IsOnGlass(Bounds))
				OnGlassMaskBoxPreWndProc(ref msg);
			return false;
		}
		protected virtual void OnGlassMaskBoxWndProc(ref Message msg) {
			if(msg.Msg == MSG.WM_KEYDOWN || msg.Msg == MSG.WM_MOUSEMOVE) {
				innerControlBufferedPaint = true;
				DrawOnGlass(msg);
				return;
			}
			if(msg.Msg == MSG.WM_PAINT) {
				NativeMethods.PAINTSTRUCT pst = new NativeMethods.PAINTSTRUCT();
				IntPtr hdc = NativeMethods.BeginPaint(Handle, ref pst);
				if(pst.rcPaint.Left == 0 && pst.rcPaint.Right == 0) {
					DrawOnGlass(msg);
					return;
				}
				NativeVista.PaintControl(Handle, hdc, pst.rcPaint.ToRectangle(), false);
				NativeMethods.EndPaint(Handle, ref pst);
				msg.Result = IntPtr.Zero;
				return;
			}
			return;
		}
		protected internal virtual void OnMaskBoxWndProc(ref Message msg) {
			if(wndProcHandler != null) wndProcHandler(this, ref msg);
			ISupportGlassRegions gr = Parent as ISupportGlassRegions;
			if(gr != null && gr.IsOnGlass(Bounds)) 
				OnGlassMaskBoxWndProc(ref msg);
		}
		protected internal virtual void CheckMouseHere() {
			if(IsDisposed || !IsHandleCreated) return;
			Point p = PointToClient(MousePosition);
			MouseHere = ClientRectangle.Contains(p) && (FindForm() != null && FindForm() == Form.ActiveForm);
		}
		protected override void OnMouseEnter(EventArgs e) {
			MouseHere = true;
		}
		protected override void OnMouseLeave(EventArgs e) {
			CheckMouseHere();
		}
		protected override void Dispose(bool disposing) {
			this.wndProcHandler = null;
			if(disposing) {
				DestroyMaskBox();
			}
			base.Dispose(disposing);
		}
		protected void UpdateHideSelection(bool newValue) {
			Properties.BeginUpdate();
			Properties.HideSelection = newValue;
			Properties.CancelUpdate();
			if(MaskBox == null) return;
			MaskBox.UpdateHideSelection();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual TextBoxMaskBox MaskBox { get { return _maskBox; } }
		protected virtual void CreateMaskBox() {
			if(MaskBox != null) DestroyMaskBox();
			this._maskBox = CreateMaskBoxInstance();
			MaskBox.Visible = false;
			MaskBox.KeyDown += new KeyEventHandler(OnMaskBox_KeyDown);
			MaskBox.KeyUp += new KeyEventHandler(OnMaskBox_KeyUp);
			MaskBox.KeyPress += new KeyPressEventHandler(OnMaskBox_KeyPress);
			MaskBox.GotFocus += new EventHandler(OnMaskBox_GotFocus);
			MaskBox.LostFocus += new EventHandler(OnMaskBox_LostFocus);
			MaskBox.MouseEnter += new EventHandler(OnMaskBox_MouseEnter);
			MaskBox.MouseLeave += new EventHandler(OnMaskBox_MouseLeave);
			MaskBox.MouseDown += new MouseEventHandler(OnMaskBox_MouseDown);
			MaskBox.MouseUp += new MouseEventHandler(OnMaskBox_MouseUp);
			MaskBox.MouseMove += new MouseEventHandler(OnMaskBox_MouseMove);
			MaskBox.MouseWheel += new MouseEventHandler(OnMaskBox_MouseWheel);
			MaskBox.DoubleClick += new EventHandler(OnMaskBox_DoubleClick);
			MaskBox.Click += new EventHandler(OnMaskBox_Click);
			MaskBox.Validated += new EventHandler(OnMaskBox_TextValidated);
			MaskBox.EditTextChanged += new EventHandler(OnMaskBox_ValueChanged);
			MaskBox.EditTextChanging += new ChangingEventHandler(OnMaskBox_ValueChanging);
			MaskBox.Font = Font;
			Controls.Add(MaskBox);
		}
		public override bool IsEditorActive {
			get {
				if(!this.Enabled)
					return false;
				IContainerControl container = GetContainerControl();
				if(container == null) return EditorContainsFocus;
				return container.ActiveControl == this || (MaskBox != null && container.ActiveControl == MaskBox);
			}
		}
		protected override void RefreshEnabledState() { 
			if(IsDisposing) return;
			if(ViewInfo != null) ((BaseEditViewInfo)ViewInfo).RefreshDisplayText = true;
			OnTextEditPropertiesChanged();
		}
		protected internal virtual void OnTextEditPropertiesChanged() {
			if(IsLoading || !IsHandleCreated) return;
			LayoutChanged();
			UpdateMaskBoxProperties(true);
		}
		protected virtual TextBoxMaskBox CreateMaskBoxInstance() {
			return new TextBoxMaskBox(this);
		}
		protected virtual void DestroyMaskBox() {
			if(MaskBox == null) return;
			TextBoxMaskBox mask = MaskBox;
			this._maskBox = null;
			mask.KeyDown -= new KeyEventHandler(OnMaskBox_KeyDown);
			mask.KeyUp -= new KeyEventHandler(OnMaskBox_KeyUp);
			mask.KeyPress -= new KeyPressEventHandler(OnMaskBox_KeyPress);
			mask.GotFocus -= new EventHandler(OnMaskBox_GotFocus);
			mask.LostFocus -= new EventHandler(OnMaskBox_LostFocus);
			mask.MouseEnter -= new EventHandler(OnMaskBox_MouseEnter);
			mask.MouseLeave -= new EventHandler(OnMaskBox_MouseLeave);
			mask.MouseDown -= new MouseEventHandler(OnMaskBox_MouseDown);
			mask.MouseUp -= new MouseEventHandler(OnMaskBox_MouseUp);
			mask.MouseMove -= new MouseEventHandler(OnMaskBox_MouseMove);
			mask.MouseWheel -= new MouseEventHandler(OnMaskBox_MouseWheel);
			mask.DoubleClick -= new EventHandler(OnMaskBox_DoubleClick);
			mask.Click -= new EventHandler(OnMaskBox_Click);
			mask.Validated -= new EventHandler(OnMaskBox_TextValidated);
			mask.EditTextChanged -= new EventHandler(OnMaskBox_ValueChanged);
			mask.EditTextChanging -= new ChangingEventHandler(OnMaskBox_ValueChanging);
			mask.Dispose();
		}
		protected internal new TextEditViewInfo ViewInfo { get { return base.ViewInfo as TextEditViewInfo; } }
		Rectangle prevMaskBoxBounds = Rectangle.Empty;
		protected override void OnBeforeUpdateViewInfo() {
			this.prevMaskBoxBounds = ViewInfo.MaskBoxRect;
		}
		protected override void OnAfterUpdateViewInfo() {
			if(this.prevMaskBoxBounds != ViewInfo.MaskBoxRect) UpdateMaskBoxBounds();
		}
		protected void UpdateMaskBoxBounds() {
			if(MaskBox != null) {
				if(ViewInfo.AllowMaskBox) {
					if(!ViewInfo.IsReady) 
						MaskBox.Bounds = ClientRectangle;
					else
						MaskBox.Bounds = ViewInfo.MaskBoxRect;
				}
			}
		}
		protected internal override void OnAppearancePaintChanged() {
			base.OnAppearancePaintChanged();
			if(!IsHandleCreated || !IsAllowMaskBoxPropertiesUpdate) return;
			UpdateMaskBoxProperties(false);
		}
		protected override void LayoutChangedCore() {
			LayoutChanged(false);
		}
		protected internal override void LayoutChanged() {
			LayoutChanged(true);
		}
		protected virtual void LayoutChanged(bool updateMask) {
			if(IsDisposing)
				return;
			base.LayoutChanged();
			if(!IsLayoutLocked && IsHandleCreated) {
				if(!IsAllowMaskBoxPropertiesUpdate) {
					UpdateMaskBoxBounds();
					return;
				}
				try {
					this.lockTabStop++;
					if(MaskBox != null) {
						if(ViewInfo.AllowMaskBox) {
							UpdateMaskBox();
							if(!ViewInfo.IsReady)
								MaskBox.Bounds = ClientRectangle;
							else
								MaskBox.Bounds = ViewInfo.MaskBoxRect;
							if(updateMask) MaskBox.Visible = true;
							base.TabStop = false;
						}
						else {
							base.TabStop = this.tabStop;
							if(updateMask) MaskBox.Visible = false;
						}
					}
				}
				finally {
					this.lockTabStop--;
				}
			}
		}
		protected override void Select(bool directed, bool forward) {
			IContainerControl container = base.GetContainerControl();
			if(container != null) {
				if(IsMaskBoxAvailable) {
					container.ActiveControl = MaskBox;
				} else {
					container.ActiveControl = this;
				}
			}
		}
		protected virtual bool CanTabStop { 
			get { return (MaskBox == null || !ViewInfo.AllowMaskBox) && TabStop; }
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			UpdateMaskBox(true);
		}
		protected void LockMaskBoxPropertiesUpdate() {
			this.maskBoxPropertiesUpdate ++;
		}
		protected void UnlockMaskBoxPropertiesUpdate() {
			this.maskBoxPropertiesUpdate --;
		}
		protected override void OnResize(EventArgs e) {
			LockMaskBoxPropertiesUpdate();
			try {
				base.OnResize(e);
			} finally {
				UnlockMaskBoxPropertiesUpdate();
			}
		}
		protected bool IsAllowMaskBoxPropertiesUpdate { get { return this.maskBoxPropertiesUpdate == 0; } }
		protected virtual void UpdateMaskBox() {
			UpdateMaskBox(false);
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			OnTextChanged(EventArgs.Empty);
			if(IsMaskBoxUpdate) return;
			UpdateMaskBox();
		}
		protected internal void ShowCaret() {
			if(IsMaskBoxAvailable && MaskBox.IsHandleCreated) EditorsNativeMethods.ShowCaret(MaskBox.Handle);
		}
		protected internal void HideCaret() {
			if(IsMaskBoxAvailable && MaskBox.IsHandleCreated) EditorsNativeMethods.HideCaret(MaskBox.Handle);
		}
		protected override void OnTabStopChanged(EventArgs e) {
			if(this.lockTabStop != 0) return;
			base.OnTabStopChanged(e);
			RefreshVisualLayout();
		}
		[Browsable(false)]
		public override bool IsNeedFocus { get { return true; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TextEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemTextEdit Properties { get { return base.Properties as RepositoryItemTextEdit; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "TextEdit"; } }
		protected virtual void OnMaskBox_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyData != Keys.Enter) UpdateResetNullValuePromptOnFocused();
			OnKeyDown(e);
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.Handled) return;
			if(e.KeyData == Keys.Enter && Properties.AllowValidateOnEnterKey) {
				if(EnterMoveNextControl) {
					this.ProcessDialogKey(Keys.Tab);
					e.Handled = true;
				} else if(Properties.ValidateOnEnterKey) {
					DoValidate();
					e.Handled = true;
				}
			}
			if(e.KeyData == Keys.F2) {
				if(IsMaskBoxAvailable) {
					MaskBox.ProcessF2();
				}
				e.Handled = true;
			}
			OnEditorKeyDownProcessNullInputKeys(e);
		}
		protected override void OnEditorKeyPress(KeyPressEventArgs e) {
			base.OnEditorKeyPress(e);
			if(e.Handled)
				return;
			if(e.KeyChar == '\x01') {
				SelectAll();
				e.Handled = true;
			} else if(e.KeyChar == '\x1b') {
				if(ErrorText != null && ErrorText.Length != 0) {
					EditValue = OldEditValue;
					IsModified = false;
					ErrorText = string.Empty;
					UpdateMaskBox();
					SelectAll();
				}
				e.Handled = true;
			} else if(e.KeyChar == '\x1a') {
				if(IsMaskBoxAvailable && MaskBox.MaskBoxCanUndo) {
					MaskBox.MaskBoxUndo();
					e.Handled = true;
				}
			}
		}
		protected virtual void OnMaskBox_KeyUp(object sender, KeyEventArgs e) { 
			OnKeyUp(e); 
		}
		void UpdateResetNullValuePromptOnFocused() {
			if(ViewInfo.IsShowNullValuePrompt() && ViewInfo.ShowNullValuePromptWhenFocused) {
				ViewInfo.ForceHideNullValuePrompt();
				if(!ViewInfo.IsShowNullValuePrompt()) {
					CheckNullPaintAppearance();
					UpdateDisplayText();
					UpdateMaskBoxProperties(false);
				}
			}
		}
		protected virtual void OnMaskBox_KeyPress(object sender, KeyPressEventArgs e) {
			if(e.KeyChar != 13) UpdateResetNullValuePromptOnFocused();
			OnKeyPress(e);
			if((!AcceptsReturn && (e.KeyChar == '\r' || e.KeyChar == '\n'))
				|| (!AcceptsTab && (e.KeyChar == '\t')) 
				|| (!AcceptsSpace && (e.KeyChar == ' '))) 
				e.Handled = true;
			if(EnterMoveNextControl && e.KeyChar == 13) {
				e.Handled = true;
			}
		}
		protected virtual bool AcceptsReturn { get { return false; } }
		protected virtual bool AcceptsTab { get { return false; } }
		protected virtual bool AcceptsSpace { get { return true; } }
		protected internal virtual void PendingEditActionPerfomed() {
			this.IsModified = true;
		}
		protected internal override void FlushPendingEditActions() {
			if(Properties.UseMaskBox && MaskBox != null)
				MaskBox.FlushPendingEditActions();
			base.FlushPendingEditActions();
		}
		void CheckNullPaintAppearance() {
			if(string.IsNullOrEmpty(Properties.NullValuePrompt)) return;
			if(ViewInfo != null) ViewInfo.UpdatePaintAppearance();
		}
		protected override void OnEditorEnter(EventArgs e) {
			base.OnEditorEnter(e);
			CheckNullPaintAppearance();
			UpdateDisplayText();
		}
		protected override void OnEditorLeave(EventArgs e) {
			base.OnEditorLeave(e);
			CheckNullPaintAppearance();
			UpdateDisplayText();
		}
		protected virtual void UpdateDisplayText() {
			if(ViewInfo != null) {
				ViewInfo.Format = Properties.ActiveFormat;
				ViewInfo.RefreshDisplayText = true;
				ViewInfo.UpdateEditValue();
			}
			UpdateMaskBoxDisplayText();
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			if(IsMaskBoxAvailable) {
				if(ContainsFocus && !MaskBox.ContainsFocus) MaskBox.Focus();
				if(!this.setSelection) {
					this.setSelection = true;
					if(InplaceType == InplaceType.Standalone && ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.None)) {
						SelectAll();
					}
				}
			}
		}
		protected virtual bool IsMatch {
			get {
				if(!IsMaskBoxAvailable)
					return true;
				if(MaskBox.IsMatch)
					return true;
				if(Properties.IsNullInputAllowed && Properties.IsNullValue(EditValue))
					return true;
				return false;
			}
		}
		protected virtual void DoIsMatchValidating(CancelEventArgs e) {
			if(e.Cancel)
				return;
			if(IsModified && !IsMatch)
				e.Cancel = true;
		}
		protected override void OnValidating(CancelEventArgs e) {
			CompleteChanges();
			if(IsAllowValidate && MaskBox != null && ViewInfo.AllowMaskBox) {
				MaskBox.RaiseValidating(e);
				DoIsMatchValidating(e);
			}
			base.OnValidating(e);
		}
		protected override void OnValidated(EventArgs e) {
			base.OnValidated(e);
			if(IsAllowValidate && MaskBox != null && ViewInfo.AllowMaskBox) {
				UpdateMaskBoxDisplayText();
			}
		}
		protected override void OnLeave(EventArgs e) {
			ViewInfo.ResetForceHideNullValuePrompt();
			base.OnLeave(e);
		}
		protected virtual void OnMaskBox_TextValidated(object sender, EventArgs e) { }
		protected virtual void OnMaskBox_GotFocus(object sender, EventArgs e) { OnGotFocus(e); }
		protected virtual void OnMaskBox_LostFocus(object sender, EventArgs e) { OnLostFocus(e); }
		protected virtual void OnMaskBox_MouseLeave(object sender, EventArgs e) {
			if(!Bounds.Contains(MousePosition))
				OnMouseLeave(e);
		}
		protected virtual void OnMaskBox_Click(object sender, EventArgs e) { OnClick(e); }
		protected virtual void OnMaskBox_DoubleClick(object sender, EventArgs e) { OnDoubleClick(e); }
		protected virtual void OnMaskBox_MouseEnter(object sender, EventArgs e) { OnMouseEnter(e); }
		int internalTextChange = 0;
		int maskBoxUpdateCount = 0;
		protected override bool IsMaskBoxUpdate { get { return this.maskBoxUpdateCount != 0; } }
		protected virtual void OnMaskBox_ValueChanging(object sender, ChangingEventArgs e) { 
			Properties.RaiseEditValueChanging(e);
		}
		protected virtual void OnMaskBox_ValueChanged(object sender, EventArgs e) {
			Properties.LockFormatParse = true;
			BeginInternalTextChange();
			this.maskBoxUpdateCount ++;
			try {
				EditValue = MaskBox.GetEditValue();
			} finally {
				this.maskBoxUpdateCount --;
				EndInternalTextChange();
			}
		}
		protected internal void BeginInternalTextChange() {
			this.internalTextChange ++;
		}
		protected internal void EndInternalTextChange() {
			this.internalTextChange --;
		}
		protected internal void SetTextCore(string text) {
			BeginInternalTextChange();
			try {
				if(Properties.CharacterCasing != CharacterCasing.Normal) {
					text = Properties.CharacterCasing == CharacterCasing.Upper ? text.ToUpper() : text.ToLower();
				}
				Text = text;
			}
			finally {
				EndInternalTextChange();
			}
		}
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			if(IsLoading || !IsHandleCreated)
				return;
			if(this.internalTextChange == 0) {
				this.setSelection = false;
				if(IsMaskBoxAvailable) {
					MaskBox.ResetSelection();
				}
			}
		}
		protected virtual void OnMaskBox_MouseUp(object sender, MouseEventArgs e) {
			OnMouseUp(ConvertToTextEdit(e));
		}
		protected virtual void OnMaskBox_MouseDown(object sender, MouseEventArgs e) {
			UpdateResetNullValuePromptOnFocused();
			OnMouseDown(ConvertToTextEdit(e));
		}
		protected virtual void OnMaskBox_MouseMove(object sender, MouseEventArgs e) {
			OnMouseMove(ConvertToTextEdit(e));
		}
		protected virtual void OnMaskBox_MouseWheel(object sender, MouseEventArgs e) {
		}
		MouseEventArgs ConvertToTextEdit(MouseEventArgs e) {
			return new MouseEventArgs(e.Button, e.Clicks, e.X + MaskBox.Left, e.Y + MaskBox.Top, e.Delta);
		}
		protected virtual void UpdateMaskBox(bool always) {
			if(MaskBox == null || !IsAllowMaskBoxPropertiesUpdate) return;
			if(!ViewInfo.AllowMaskBox) return;
			MaskBox.SuspendLayout();
			try {
				UpdateMaskBoxProperties(always);
				if(!IsVisualLayoutUpdate)
					UpdateMaskBoxDisplayText();
			} finally {
				MaskBox.ResumeLayout();
			}
		}
		protected internal virtual bool IsMaskBoxAvailable { get { return MaskBox != null && MaskBox.Visible; } }
		protected virtual void UpdateMaskBoxDisplayText() {
			if(MaskBox != null) {
				bool showNullValuePrompt = false;
				object editValue = EditValue;
				bool ignoreMask = Properties.IsUseDisplayFormat;
				string displayText = Properties.GetDisplayText(editValue);
				if(Properties.IsNullValue(editValue))
					editValue = null;
				if(ViewInfo.IsShowNullValuePrompt()) {
					displayText = Properties.NullValuePrompt;
					UpdateMaskBoxProperties(false);
					showNullValuePrompt = true;
				}
				MaskBox.SetEditValue(editValue, displayText, ignoreMask);
				if(showNullValuePrompt) Select(0, 0);
			}
		}
		protected virtual void OnEditorKeyDownProcessNullInputKeys(KeyEventArgs e) {
			if(e.Handled)
				return;
			if(Properties.IsNowReadOnly)
				return;
			if(!Properties.IsNullInputAllowed)
				return;
			if(e.Modifiers == Keys.Control) {
				switch(e.KeyCode) {
					case Keys.D0:
					case Keys.Delete:
						DoNullInputKeysCore(e);
						break;
				}
			}
		}
		protected virtual void DoNullInputKeysCore(KeyEventArgs e) {
			EditValue = null;
			UpdateMaskBoxDisplayText();
			e.Handled = true;
		}
		protected virtual void UpdateMaskBoxProperties(bool always) {
			HorizontalAlignment hAlign = HorizontalAlignment.Left;
			HorzAlignment align = ViewInfo.PaintAppearance.HAlignment;
			if(align == HorzAlignment.Default) align = Properties.DefaultAlignment;
			switch(align) {
				case HorzAlignment.Center:
					hAlign = HorizontalAlignment.Center;
					break;
				case HorzAlignment.Far:
					hAlign = HorizontalAlignment.Right;
					break;
				default:
					hAlign = HorizontalAlignment.Left;
					break;
			}
			Color maskBoxBackColor = Color.FromArgb(255, ViewInfo.PaintAppearance.BackColor);
			if(always || MaskBox.TabStop != TabStop) MaskBox.TabStop = TabStop;
			if(always || MaskBox.ReadOnly != Properties.IsNowReadOnly) MaskBox.ReadOnly = Properties.IsNowReadOnly;
			if(always || MaskBox.TextAlign != hAlign) MaskBox.TextAlign = hAlign;
			if(always || MaskBox.ForeColor != ViewInfo.PaintAppearance.ForeColor) MaskBox.ForeColor = ViewInfo.PaintAppearance.ForeColor;
			if(always || MaskBox.BackColor != maskBoxBackColor) MaskBox.BackColor = maskBoxBackColor;
			if(always || !CompareFonts(MaskBox.Font, ViewInfo.PaintAppearance.Font))	{
				MaskBox.Font = ViewInfo.PaintAppearance.Font;
			}
			if(always || MaskBox.HideSelection != Properties.HideSelection) MaskBox.HideSelection = Properties.HideSelection;
			if(always || MaskBox.AccessibleDescription != AccessibleDescription) MaskBox.AccessibleDescription = AccessibleDescription;
			if(always || MaskBox.AccessibleName != AccessibleName) MaskBox.AccessibleName = AccessibleName;
			if(always || MaskBox.AccessibleRole != AccessibleRole) MaskBox.AccessibleRole = AccessibleRole;
			if(always || MaskBox.TabIndex != 0) MaskBox.TabIndex = 0;
			CharacterCasing characterCasing = IsShowNullValuePrompt() ? CharacterCasing.Normal : Properties.CharacterCasing;
			if(always || MaskBox.CharacterCasing != characterCasing) MaskBox.CharacterCasing = characterCasing;
			int effectiveMaskLength;
			if(Properties.Mask.MaskType == MaskType.None) {
				effectiveMaskLength = Properties.MaxLength;
				MaskBox.MaxLengthForStringMasks = 0;
			} else {
				effectiveMaskLength = 0;
				MaskBox.MaxLengthForStringMasks = Properties.MaxLength;
			}
			if(always || MaskBox.MaxLength != effectiveMaskLength) MaskBox.MaxLength = effectiveMaskLength;
			UpdatePasswordChar();
			if(always || MaskBox.ContextMenu != Properties.ContextMenu)	MaskBox.ContextMenu = Properties.ContextMenu;
			if(always || MaskBox.ContextMenuStrip != Properties.ContextMenuStrip) MaskBox.ContextMenuStrip = Properties.ContextMenuStrip;
			if(always || MaskBox.ReadOnly != Properties.ReadOnly) MaskBox.ReadOnly = Properties.ReadOnly;
			if(Properties.UseMaskBox && MaskBox != null && !MaskBox.Mask.Equals(Properties.Mask)) {
				MaskBox.Mask.Assign(Properties.Mask);
			}
			if(always || MaskBox.CausesValidation != CausesValidation) MaskBox.CausesValidation = CausesValidation;
		}
		protected override void OnEnter(EventArgs e) {
			base.OnEnter(e);
			UpdatePasswordChar();
		}
		protected override void OnEditValueChanging(ChangingEventArgs e) {
			base.OnEditValueChanging(e);
			UpdatePasswordChar();
		}
		protected virtual bool IsExistsNullValuePrompt { 
			get { 
				return !string.Empty.Equals(Properties.NullValuePrompt) &&
				(this.EditValue == null || this.EditValue == DBNull.Value
				|| (string.Empty.Equals(this.EditValue) && Properties.NullValuePromptShowForEmptyValue));
			} 
		}
		protected virtual void UpdatePasswordChar() {
			if(MaskBox == null) return; 
			if((!MaskBox.ContainsFocus && IsExistsNullValuePrompt) || (MaskBox.ContainsFocus && IsExistsNullValuePrompt && Properties.ShowNullValuePromptWhenFocused)) {
				MaskBox.PasswordChar = '\0';
				UpdateMaskBoxSystemPasswordChar(false);
			}
			else {
				MaskBox.PasswordChar = Properties.PasswordChar; 
				UpdateMaskBoxSystemPasswordChar(Properties.UseSystemPasswordChar);
			}
		}
		bool? settingPasswordChar = null;
		void UpdateMaskBoxSystemPasswordChar(bool value) {
			if(settingPasswordChar != null) {
				settingPasswordChar = value;
				return;
			}
			this.settingPasswordChar = value;
			if(IsHandleCreated && value != MaskBox.UseSystemPasswordChar) {
				BeginInvoke(new MethodInvoker(delegate() {
					MaskBox.UseSystemPasswordChar = settingPasswordChar.HasValue ? settingPasswordChar.Value : value;
					this.settingPasswordChar = null;
				}));
			}
			else {
				this.settingPasswordChar = null;
			}
		}
		protected virtual bool CompareFonts(Font f1, Font f2) {
			return f1.Equals(f2);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectionStart {
			get {
				if(IsMaskBoxAvailable && !ViewInfo.IsShowNullValuePrompt())
					return MaskBox.MaskBoxSelectionStart;
				return 0;
			}
			set {
				if(IsMaskBoxAvailable && !ViewInfo.IsShowNullValuePrompt())
					MaskBox.MaskBoxSelectionStart = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectionLength {
			get {
				if(IsMaskBoxAvailable && !ViewInfo.IsShowNullValuePrompt())
					return MaskBox.MaskBoxSelectionLength;
				return 0;
			}
			set {
				if(value < 0) value = 0;
				if(IsMaskBoxAvailable && !ViewInfo.IsShowNullValuePrompt())
					MaskBox.MaskBoxSelectionLength = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SelectedText {
			get {
				if(IsMaskBoxAvailable && !ViewInfo.IsShowNullValuePrompt())
					return MaskBox.MaskBoxSelectedText;
				return "";
			}
			set {
				if(IsMaskBoxAvailable)
					MaskBox.MaskBoxSelectedText = value;
			}
		}
		public void Select(int start, int length) {
			this.setSelection = true;
			if(IsMaskBoxAvailable)
				MaskBox.MaskBoxSelect(start, length);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TextEditText"),
#endif
 DXCategory(CategoryName.Appearance), RefreshProperties(RefreshProperties.All), Bindable(true), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SmartTagProperty("Text", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public override string Text {
			get {
				if(IsMaskBoxAvailable) {
					if(IsShowNullValuePrompt()) return string.Empty;
					return MaskBox.MaskBoxText;
				}
				else
					return base.Text;
			}
			set {
				base.Text = value;
			}
		}
		protected internal bool IsShowNullValuePrompt() { return ViewInfo.IsShowNullValuePrompt(); } 
		public override void SelectAll() {
			if(IsShowNullValuePrompt()) return;
			this.setSelection = true;
			if(IsMaskBoxAvailable) MaskBox.MaskBoxSelectAll();
		}
		public override void DeselectAll() {
			this.setSelection = true;
			if(IsMaskBoxAvailable)
				MaskBox.MaskBoxDeselectAll();
		}
		public override void Reset() {
			base.Reset();
			Properties.LockFormatParse = false;
			if(MaskBox != null) MaskBox.Reset();
			DeselectAll();
		}
		public void Copy() {
			if(IsMaskBoxAvailable) MaskBox.Copy();
		}
		public void Paste() {
			if(IsMaskBoxAvailable) MaskBox.Paste();
		}
		public void Cut() {
			if(IsMaskBoxAvailable) MaskBox.Cut();
		}
		[Browsable(false)]
		public bool CanUndo { 
			get {
				if(IsMaskBoxAvailable) return MaskBox.MaskBoxCanUndo;
				return false;
			}
		}
		public void Undo() {
			if(IsMaskBoxAvailable) MaskBox.MaskBoxUndo();
		}
		public void ScrollToCaret() {
			if(IsMaskBoxAvailable) MaskBox.MaskBoxScrollToCaret();
		}
		[Browsable(false)]
		public bool IsNeededCursorKey(Keys keyData) {
			if(IsMaskBoxAvailable)
				return MaskBox.IsNeededCursorKey(keyData);
			else
				return false;
		}
		protected internal virtual MaskManager CreateMaskManager(MaskProperties mask) {
			return mask.CreateDefaultMaskManager();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TextEditSpin"),
#endif
 DXCategory(CategoryName.Events)]
		public event SpinEventHandler Spin {
			add { Properties.Spin += value; }
			remove { Properties.Spin -= value; }
		}
		protected virtual void OnSpin(SpinEventArgs e) {
			Properties.RaiseSpin(e);
			if(e.Handled)
				return;
			if(IsMaskBoxAvailable)
				e.Handled = MaskBox.MaskBoxSpin(e.IsSpinUp);
		}
		protected internal virtual bool DoSpin(bool isUp) {
			SpinEventArgs e = new SpinEventArgs(isUp);
			OnSpin(e);
			return e.Handled;
		}
		#region hidden
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		#endregion
		protected override void OnImeModeChanged(EventArgs e) { 
			base.OnImeModeChanged(e);
			if(IsMaskBoxAvailable && MaskBox != null)
				MaskBox.ImeMode = ImeMode.Inherit;
		}
		protected override bool IsInputKey(Keys keyData) {
			return base.IsInputKey(keyData) || (keyData == Keys.Enter && (this.Properties.ValidateOnEnterKey && this.IsModified));
		}
		protected override bool NeedLayoutUpdateValidating() {
			return IsShowNullValuePrompt();
		}
		protected internal virtual void OnEndIme() { } 
		protected internal virtual bool AllowSmartMouseWheel { get { return ContainsFocus; } }
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			if(!AllowSmartMouseWheel && !ContainsFocus) return;
			OnMouseWheel(e);
		}
		protected internal virtual bool FireSpinRequest(DXMouseEventArgs e, bool isUp) {
			return DoSpin(isUp);
		}
	}
	public class LinesConverter {
		public static string[] TextToLines(string text) {
			ArrayList strings = new ArrayList();
			string s;
			int n = 0;
			while(n < text.Length) {
				int e = n;
				while (e < text.Length) {
					char ch = text[e];
					if(ch != 13 && ch != 10) 
						e++;
					else 
						break;
				}
				s = text.Substring(n, e - n);
				strings.Add(s);
				if (e < text.Length && text[e] == 13) e++;
				if (e < text.Length && text[e] == 10) e++;
				n = e;
			}
			if (text.Length > 0 && (text[text.Length - 1] == 13 ||  text[text.Length - 1] == 10))
				strings.Add("");
			return strings.ToArray(typeof(String)) as string[];
			}
		public static string LinesToText(string[] value) {
			if (value != null && value.Length > 0) {
				StringBuilder stb = new StringBuilder(value[0]);
				for(int n = 1; n < value.Length; n++) {
					stb.Append("\r\n");
					stb.Append(value[n]);
				}
				return stb.ToString();
			}
			return "";
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class TextEditViewInfo : BaseEditViewInfo, IFormatRuleSupportContextImage {
		protected Rectangle fMaskBoxRect;
		Rectangle contextImageBounds;
		Image contextImage;
		int baselineOffset = 0;
		public TextEditViewInfo(RepositoryItem item) : base(item) {
		}
		public override ObjectState CalcBorderState() {
			if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.WindowsXP)
				return base.CalcBorderState();
			if(Focused)
				return ObjectState.Hot;
			return ObjectState.Normal;
		}
		protected override bool CanFastRecalcViewInfo(Rectangle bounds, Point mousePosition) {
			if(!base.CanFastRecalcViewInfo(bounds, mousePosition)) return false;
			return ContextImage == null;
		}
		protected override BorderPainter GetBorderPainterCore() {
			BorderPainter painter = base.GetBorderPainterCore();
			if(painter is WindowsXPButtonEditBorderPainter && LookAndFeel.ActiveLookAndFeel.UseWindows7Border)
				return new WindowsXPTextBorderPainter();
			return painter;	
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			TextEditViewInfo be = info as TextEditViewInfo;
			if(be == null) return;
			this.fMaskBoxRect = be.fMaskBoxRect;
			this.contextImageBounds = be.contextImageBounds;
			this.contextImage = be.contextImage;
		}
		protected internal virtual Color NullValuePromptForeColor { get { return GetSystemColor(SystemColors.GrayText); } }
		protected override AppearanceDefault CreateDefaultAppearance() {
			if(IsShowNullValuePrompt() || IsLoadingValue) return new AppearanceDefault(NullValuePromptForeColor, GetSystemColor(SystemColors.Window), GetDefaultFont());
			if(!Item.ReadOnly || !Item.UseReadOnlyAppearance) return base.CreateDefaultAppearance();
			return new AppearanceDefault(GetSystemColor(SystemColors.WindowText), GetSystemColor(SystemColors.ControlLight), GetDefaultFont());
		}
		protected override int MaxTextWidth {
			get {
				if(AllowHtmlString && MaskBoxRect.Width > 0) return MaskBoxRect.Width;
				return base.MaxTextWidth;
			}
		}
		protected override Size CalcHtmlTextSize(GraphicsCache cache, string text, int maxWidth) {
			StringCalculateArgs sc = new StringCalculateArgs(cache.Graphics, PaintAppearance, DefaultTextOptions, text, new Rectangle(0, 0, maxWidth, 0), null, GetHtmlContext());
			sc.RoundTextHeight = true;
			var si = StringPainter.Default.Calculate(sc);
			Size size = si.Bounds.Size;
			if(Item.HtmlImages != null) size.Height = Math.Max(size.Height, Item.HtmlImages.ImageSize.Height);
			if(maxWidth != 0) StringInfo = si;
			return size;
		}
		public override bool AllowHtmlString {
			get {
				if(AllowMaskBox) return false;
				return Item.GetAllowHtmlDraw();
			}
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			if(IsShowNullValuePrompt()) PaintAppearance.ForeColor = NullValuePromptForeColor;
		}
		public Image ContextImage { 
			get {
				if(contextImage == null) return Item.ContextImage;
				return contextImage; 
			} 
			set { contextImage = value; } 
		}
		public Rectangle ContextImageBounds { get { return contextImageBounds; } }
		public virtual Padding ContextImagePadding { get { return new Padding(2, 0, 2, 0); } }
		protected virtual Size CalcContextImageSize() {
			if(ContextImage == null) return Size.Empty;
			Size size = ContextImage.Size;
			if(size.IsEmpty) return size;
			size.Width += ContextImagePadding.Horizontal;
			size.Height += ContextImagePadding.Vertical;
			return size;
		}
		public void SetMaskBoxRect(Rectangle bounds) { this.fMaskBoxRect = bounds; } 
		public override Rectangle GetTextBounds() { return MaskBoxRect; }
		public virtual Rectangle MaskBoxRect { get { return fMaskBoxRect; } }
		public new RepositoryItemTextEdit Item { get { return base.Item as RepositoryItemTextEdit; } }
		public override void Reset() {
			this.fMaskBoxRect = Rectangle.Empty;
			base.Reset();
		}
		public virtual bool AllowMaskBox { 
			get {
				if(BaseEditViewInfo.ShowFieldBindings && GetDataBindingText() != string.Empty)
					return false;
				if(OwnerEdit != null && !OwnerEdit.Enabled && !AllowMaskBoxWhenDisabled)
					return false;
				return Editable && Item.Editable;
			} 
		}
		protected virtual bool AllowMaskBoxWhenDisabled { get { return false; } }
		public virtual bool AllowDrawText { get { return true; } }
		public override bool DefaultAllowDrawFocusRect { get { return true; } }
		public override bool AllowDrawContent {
			get {
				if(OwnerEdit == null || !AllowMaskBox) return true;
				return false;
			}
		}
		public new TextEdit OwnerEdit { get { return base.OwnerEdit as TextEdit; } }
		protected override Size CalcContentSize(Graphics g) {
			Size size = base.CalcContentSize(g);
			if(InplaceType != InplaceType.Standalone)
				size.Width += TextSideIndent * 2;
			Size image = CalcContextImageSize();
			size.Width += image.Width;
			size.Height = Math.Max(size.Height, image.Height - ContextImageSizeCompensation);
			size = RectangleHelper.Inflate(size, GetTextBoxContentPadding());
			return size;
		}
		protected virtual Padding GetTextBoxContentPadding() {
			if(!IsSkinLookAndFeel) return Padding.Empty;
			var element = EditorsSkins.GetSkin(LookAndFeel)[EditorsSkins.SkinTextBox];
			if(element == null) return Padding.Empty;
			return element.ContentMargins.ToPadding();
		}
		const int TextSideIndent = 1;
		int GetTextAscentHeight(Graphics g) {
			return TextUtils.GetFontAscentHeight(g, this.PaintAppearance.Font);
		}
		public virtual int TextBaselineOffset { get { return baselineOffset; } }
		protected virtual void CalcTextBaseline(Graphics g) {
			if(TextSize == Size.Empty) baselineOffset = 0;
			else baselineOffset = GetTextAscentHeight() + 1;
		}
		public override void CalcViewInfo(Graphics g) {
			base.CalcViewInfo(g);
			CalcTextBaseline(g);   
		}
		protected virtual Padding MaskBoxPadding { 
			get {
				Padding p = GetTextBoxContentPadding();
				var res = Item.MaskBoxPadding; 
				res.Left += p.Left;
				res.Right += p.Right;
				return res;
			} 
		}
		protected Rectangle CalcMaskBoxRect(Rectangle content) { return CalcMaskBoxRect(content, ref contextImageBounds); }
		protected virtual int ContextImageSizeCompensation { get { return 2; } }
		protected virtual Rectangle CalcMaskBoxRect(Rectangle content, ref Rectangle contextImageBounds) {
			Rectangle r = content;
			contextImageBounds = Rectangle.Empty;
			Size contextImage = CalcContextImageSize();
			if(!contextImage.IsEmpty) {
				if(contextImage.Height <= r.Height + ContextImageSizeCompensation && contextImage.Width <= r.Width) {
					if(r.Height < contextImage.Height) {
						content.Y -= (int)(ContextImageSizeCompensation / 2);
						content.Height += ContextImageSizeCompensation;
					}
					contextImageBounds = RectangleHelper.GetCenterBounds(new Rectangle(content.Location, new Size(contextImage.Width, content.Height)), contextImage);
					r.Width -= contextImage.Width;
					if((RightToLeft && Item.ContextImageAlignment == ContextImageAlignment.Near) ||
						(!RightToLeft && Item.ContextImageAlignment == ContextImageAlignment.Far)) {
						contextImageBounds.X = r.Right;
					} else {
						r.X += contextImage.Width;
					}
				}
			}
			UpdateMaxBoxRectangle(ref r);
			return UpdateMaskBoxPadding(r);
		}
		protected virtual void UpdateMaxBoxRectangle(ref Rectangle rect) {
			if(InplaceType != InplaceType.Standalone)
				rect.Inflate(-TextSideIndent, 0);
		}
		protected Rectangle UpdateMaskBoxPadding(Rectangle maskbox) {
			Rectangle source = maskbox;
			var p = MaskBoxPadding;
			maskbox.X += p.Left;
			maskbox.Y += p.Top;
			maskbox.Width -= p.Horizontal;
			maskbox.Height -= p.Vertical;
			if(maskbox.Width < 1 || maskbox.Height < 1) {
				maskbox = new Rectangle(source.Location, Size.Empty);
			}
			return maskbox;
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			this.fMaskBoxRect = CalcMaskBoxRect(ContentRect, ref contextImageBounds);
			if(AllowHtmlString && TextSize.Height < MaskBoxRect.Height) {
				CalcTextSize(null);
				this.fMaskBoxRect = CalcMaskBoxRect(ContentRect, ref contextImageBounds);
			}
			if(TextSize.Height >= MaskBoxRect.Height) return;
			int h = TextSize.Height;
			if(TextUseFullBounds) h = MaskBoxRect.Height;
			switch(PaintAppearance.GetTextOptions().VAlignment) {
				case VertAlignment.Top: break;
				case VertAlignment.Bottom:
					this.fMaskBoxRect.Y = MaskBoxRect.Bottom - h; break;
				default:
					this.fMaskBoxRect.Y = MaskBoxRect.Y + (MaskBoxRect.Height - h) / 2; 
					break;
			}
			this.fMaskBoxRect.Height = h;
		}
		public override bool IsSupportIncrementalSearch { get { return true; } }
		public override EditHitInfo CalcHitInfo(Point p) {
			EditHitInfo hi = base.CalcHitInfo(p);
			if(Bounds.Contains(p)) {
				if(ContentRect.Contains(p))
					hi.SetHitTest(EditHitTest.MaskBox);
			}
			return hi;
		}
		public override void Offset(int x, int y) { 
			base.Offset(x, y);
			if(!fMaskBoxRect.IsEmpty) this.fMaskBoxRect.Offset(x, y);
			if(!contextImageBounds.IsEmpty) this.contextImageBounds.Offset(x, y);
		}
		protected override int CalcMinHeightCore(Graphics g) { 
			int res = 0;
			string prevDText = this.fDisplayText;
			if(!AllowHtmlString) this.fDisplayText = "";
			try {
				res = base.CalcMinHeightCore(g);
			}
			finally {
				this.fDisplayText = prevDText;
			}
			return res;
		}
		protected internal virtual bool ShowNullValuePromptWhenFocused { get { return Item.ShowNullValuePromptWhenFocused; } }
		bool forceHideNullValuePrompt = false;
		protected internal void ForceHideNullValuePrompt() {
			this.forceHideNullValuePrompt = true;
		}
		protected internal void ResetForceHideNullValuePrompt() {
			this.forceHideNullValuePrompt = false;
		}
		protected internal virtual bool IsShowNullValuePrompt() {
			if((InplaceType == InplaceType.Grid) || Item.ReadOnly || !Item.Enabled) return false;
			if(Item.IsDesignMode || this.forceHideNullValuePrompt) return false;
			if(!Item.IsUseDisplayFormat) {
				if(!ShowNullValuePromptWhenFocused) return false;
				if(forceHideNullValuePrompt) return false;
			}
			if(string.IsNullOrEmpty(Item.NullValuePrompt)) return false;
			if(OwnerEdit != null && OwnerEdit.IsModified) return false;
			if(Item.IsNullValue(EditValue)) return true;
			return Item.NullValuePromptShowForEmptyValue && Item.IsEmptyValue(EditValue);
		}
		protected override string GetDisplayText() {
			if(BaseEdit.IsNotLoadedValue(EditValue)) return string.Empty;
			if(IsShowNullValuePrompt()) return Item.NullValuePrompt;
			string s = base.GetDisplayText();
			if(s != null) {
				if(Item.IsPasswordBox) {
					if(!Item.IsNullValue(this.EditValue))
						return new String(Item.GetPasswordCharCore(false), s.Length);
				}
				if(Item.CharacterCasing != CharacterCasing.Normal) 
					return Item.CharacterCasing == CharacterCasing.Upper ? s.ToUpper() : s.ToLower();
			}
			return s;
		}
		protected virtual bool IsTextToolTipPoint(Point point) {
			return MaskBoxRect.Contains(point);
		}
		protected override ToolTipControlInfo CalcTextToolTipInfo(Point point) {
			if(!IsTextToolTipPoint(point) || (DisplayText == null || DisplayText.Length == 0) || Item.IsPasswordBox) return null;
			return new ToolTipControlInfo("Text", GetToolTipText());
		}
		protected virtual string GetToolTipText() { 
			if(DisplayText == null) return "";
			string res = DisplayText.Length > RepositoryItemTextEdit.MaxToolTipTextLength ? DisplayText.Substring(0, RepositoryItemTextEdit.MaxToolTipTextLength) : DisplayText;
			if(AllowHtmlString) res = StringPainter.Default.RemoveFormat(res);
			return res;
		}
		protected virtual int GetTextToolTipWidth() { return 0; } 
		protected override void CalcShowHint() { 
			this.toolTipInfoCalculated = true;
			ShowTextToolTip = false;
			if(MaskBoxRect.IsEmpty) return;
			Size size = CalcTextSizeCore((Graphics)null, GetToolTipText(), GetTextToolTipWidth());
			if(XPaint.Graphics is XPaintMixed) size.Width++;
			ShowTextToolTip = (MaskBoxRect.Width < size.Width || MaskBoxRect.Height < size.Height);
		} 
		bool IFormatRuleSupportContextImage.SupportContextImage { get { return true; } }
		void IFormatRuleSupportContextImage.SetContextImage(Image image) { ContextImage = image; }
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class TextEditPainter : BaseEditPainter {
		protected virtual void DrawTextBoxArea(ControlGraphicsInfoArgs info) {
			TextEditViewInfo vi = info.ViewInfo as TextEditViewInfo;
			DrawEmptyArea(info, vi.MaskBoxRect);
		}
		protected virtual void DrawEmptyArea(ControlGraphicsInfoArgs info, Rectangle maskBoxRect) {
			TextEditViewInfo vi = info.ViewInfo as TextEditViewInfo;
			if(!vi.FillBackground || maskBoxRect == vi.ContentRect || maskBoxRect.IsEmpty) return;
			Brush brush = vi.PaintAppearance.GetBackBrush(info.Cache, vi.Bounds);
			Rectangle r = vi.ContentRect;
			r.Height = maskBoxRect.Y - vi.ContentRect.Y;
			if(r.Height > 0) info.Paint.FillRectangle(info.Graphics, brush, r);
			r.Y = maskBoxRect.Bottom;
			r.Height = vi.ContentRect.Bottom - maskBoxRect.Bottom;
			if(r.Height > 0) info.Paint.FillRectangle(info.Graphics, brush, r);
			r = vi.ContentRect;
			r.Width = maskBoxRect.X - vi.ContentRect.X;
			if(r.Width > 0) info.Paint.FillRectangle(info.Graphics, brush, r);
			r = vi.ContentRect;
			r.X = maskBoxRect.Right; r.Width = vi.ContentRect.Right - maskBoxRect.Right;
			if(r.Width > 0) info.Paint.FillRectangle(info.Graphics, brush, r);
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			TextEditViewInfo vi = info.ViewInfo as TextEditViewInfo;
			if(!vi.AllowDrawContent) {
				DrawTextBoxArea(info);
				DrawContextImage(info);
				if(vi.OwnerEdit == null || vi.OwnerEdit.MaskBox == null || vi.OwnerEdit.MaskBox.Visible) return;
			} 
			base.DrawContent(info);
			if(vi.AllowDrawContent) DrawContextImage(info);
			DrawText(info);
		}
		protected virtual void DrawContextImage(ControlGraphicsInfoArgs info) {
			TextEditViewInfo vi = info.ViewInfo as TextEditViewInfo;
			if(vi.ContextImageBounds.IsEmpty) return;
			Rectangle r = RectangleHelper.Deflate(vi.ContextImageBounds, vi.ContextImagePadding);
			info.Cache.Paint.DrawImage(info.Graphics, vi.ContextImage, r);
		}
		internal bool IsTextMatch(TextEditViewInfo viewInfo, out int containsIndex, out int matchedLength) {
			return IsTextMatch(viewInfo.DisplayText, viewInfo.MatchedString, viewInfo.MatchedStringUseContains, viewInfo.MatchedStringAllowPartial, out containsIndex, out matchedLength);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		static public bool IsTextMatch(string sourceText, string matchedText, bool allowContains, bool allowPartial, out int containsIndex, out int matchedLength) {
			containsIndex = 0;
			string text = sourceText;
			string matched = matchedText;
			matchedLength = matched.Length;
			if(text == matched) return true;
			if(BaseEdit.StringStartsWidth(text, matched, true)) return true;
			if(allowContains) {
				containsIndex = text.IndexOf(matched, StringComparison.CurrentCultureIgnoreCase);
				if(containsIndex > -1) return true;
			}
			if(allowPartial) {
				for(; matchedLength > 0; matchedLength--) {
					matched = matched.Substring(0, matchedLength);
					if(BaseEdit.StringStartsWidth(text, matched, true)) return true;
					if(allowContains) {
						containsIndex = text.IndexOf(matched, StringComparison.CurrentCultureIgnoreCase);
						if(containsIndex > -1) return true;
					}
				}
			}
			containsIndex = -1;
			return false;
		}
		bool CheckAllowDrawMatchedText(TextEditViewInfo textInfo) {
			if(textInfo.AllowHtmlString && textInfo.DisplayText.Contains("<")) return false;
			return true;
		}
		internal bool CheckDrawMatchedText(ControlGraphicsInfoArgs info) {
			TextEditViewInfo vi = info.ViewInfo as TextEditViewInfo;
			Rectangle r = vi.MaskBoxRect;
			if(vi.MatchedString.Length > 0 && CheckAllowDrawMatchedText(vi)) {
				string text = vi.DisplayText.ToLower();
				string matched = vi.MatchedString.ToLower();
				string matchedText = vi.MatchedString;
				int containsIndex, matchedLength;
				if(IsTextMatch(vi, out containsIndex, out matchedLength)) {
					matchedText = matchedText.Substring(0, matchedLength);
					if(!vi.MatchedStringUseContains) containsIndex = -1;
					DrawMatchedString(info, r, vi.DisplayText, matchedText, vi.PaintAppearance, vi.IsInvertIncrementalSearchString, containsIndex);
					return true;
				}
			}
			return false;
		}
		protected virtual void DrawText(ControlGraphicsInfoArgs info) {
			TextEditViewInfo vi = info.ViewInfo as TextEditViewInfo;
			if(vi.AllowDrawText && !vi.MaskBoxRect.IsEmpty) {
				Rectangle r = vi.MaskBoxRect;
				bool textDrawn = false;
				if(vi.IsSupportIncrementalSearch && CheckDrawMatchedText(info)) {
					textDrawn = true;
				}
				if(!textDrawn) {
					if(BaseEditViewInfo.ShowFieldBindings && vi.GetDataBindingText() != "") return;
					if(info.IsDrawOnGlass && NativeVista.IsVista)
						ObjectPainter.DrawTextOnGlass(info.Graphics, vi.PaintAppearance, vi.DisplayText, r, vi.PaintAppearance.GetStringFormat());
					else
						DrawString(info, r, vi.DisplayText, vi.PaintAppearance);
				}
			}
		}
		protected virtual void DrawString(ControlGraphicsInfoArgs info, Rectangle bounds, string text, AppearanceObject appearance) {
			if(bounds.Width < 1 || bounds.Height < 1) return;
			if(info.IsDrawOnGlass && NativeVista.IsVista)
				ObjectPainter.DrawTextOnGlass(info.Graphics, appearance, text, bounds);
			else {
				if(info.ViewInfo.AllowHtmlString) {
					if(info.ViewInfo.StringInfo == null || info.ViewInfo.StringInfo.SourceString != text || info.ViewInfo.StringInfo.OriginalBounds.Size != bounds.Size) {
						var sc = new StringCalculateArgs(info.Graphics,
							appearance, info.ViewInfo.DefaultTextOptions, text, bounds, null, ((BaseEditViewInfo)info.ViewInfo).GetHtmlContext());
						sc.RoundTextHeight = true;
						info.ViewInfo.StringInfo = StringPainter.Default.Calculate(sc);
					} else
						StringPainter.Default.UpdateLocation(info.ViewInfo.StringInfo, bounds);
					if(ShouldUpdateStringInfoAppearanceColors)
						info.ViewInfo.StringInfo.UpdateAppearanceColors(appearance);
					StringPainter.Default.DrawString(info.Cache, info.ViewInfo.StringInfo);
				}
				else {
					appearance.DrawString(info.Cache, text, bounds, appearance.GetForeBrush(info.Cache), appearance.GetTextOptions().GetStringFormat(info.ViewInfo.DefaultTextOptions));
				}
			}
		}
		protected virtual bool ShouldUpdateStringInfoAppearanceColors { get { return false; } }
		protected virtual void DrawMatchedString(ControlGraphicsInfoArgs info, Rectangle bounds, string text, string matchedText, AppearanceObject appearance, bool invert, int containsIndex) {
			TextEditViewInfo vi = info.ViewInfo as TextEditViewInfo;
			if(matchedText.Length > text.Length) matchedText = text;
			AppearanceDefault highlight = LookAndFeelHelper.GetHighlightSearchAppearance(vi.LookAndFeel, !vi.UseHighlightSearchAppearance);
			info.Cache.Paint.DrawMultiColorString(info.Cache, bounds, text, matchedText, appearance, appearance.GetTextOptions().GetStringFormat(info.ViewInfo.DefaultTextOptions),
				highlight.ForeColor, highlight.BackColor, invert, containsIndex);
		}
	}
	public interface ISupportGlassRegions {
		bool IsOnGlass(Rectangle rect);
	}
}
