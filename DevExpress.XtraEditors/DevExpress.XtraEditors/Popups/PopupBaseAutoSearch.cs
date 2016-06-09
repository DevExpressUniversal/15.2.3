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
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using DevExpress.Accessibility;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ListControls;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemPopupBaseAutoSearchEdit : RepositoryItemPopupBase {
		bool immediatePopup, popupSizeable;
		public RepositoryItemPopupBaseAutoSearchEdit() {
			this.immediatePopup = DefaultImmeditatePopup; 
			this.popupSizeable = DefaultPopupSizeable; 
		}
		protected virtual bool DefaultImmeditatePopup { get { return false; } }
		protected virtual bool DefaultPopupSizeable { get { return false; } }
		[Browsable(false)]
		public new PopupBaseAutoSearchEdit OwnerEdit { get { return base.OwnerEdit as PopupBaseAutoSearchEdit; } }
		public override void Assign(RepositoryItem item) {
			RepositoryItemPopupBaseAutoSearchEdit source = item as RepositoryItemPopupBaseAutoSearchEdit;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this.immediatePopup = source.ImmediatePopup;
				this.popupSizeable = source.PopupSizeable;
			}
			finally {
				EndUpdate();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseAutoSearchEditImmediatePopup"),
#endif
 DefaultValue(false)]
		public virtual bool ImmediatePopup {
			get { return immediatePopup; }
			set {
				if(ImmediatePopup == value) return;
				immediatePopup = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseAutoSearchEditPopupSizeable"),
#endif
 DefaultValue(false)]
		public virtual bool PopupSizeable {
			get { return popupSizeable; }
			set {
				if(PopupSizeable == value) return;
				popupSizeable = value;
				OnPropertiesChanged();
			}
		}
		protected internal override bool NeededKeysContains(Keys key) {
			if(OwnerEdit != null) {
				if(key == Keys.Left || key == Keys.Right) {
					if(OwnerEdit.InplaceType != InplaceType.Standalone)
						return OwnerEdit.AutoSearchText != "";
					if(key == Keys.Left) {
						return OwnerEdit.SelectionStart != 0 || OwnerEdit.SelectionLength != 0 || OwnerEdit.AutoSearchText != "";
					} else {
						return OwnerEdit.SelectionStart != 0 || OwnerEdit.AutoSearchText != "";
					}
				}
			}
			return base.NeededKeysContains(key);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DevExpress.XtraEditors.Mask.MaskProperties Mask {
			get { return base.Mask; }
		}
	}
}
namespace DevExpress.XtraEditors {
	public abstract class PopupBaseAutoSearchEdit : PopupBaseEdit {
		string autoSearchText = string.Empty;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PopupBaseAutoSearchEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemPopupBaseAutoSearchEdit Properties { get { return base.Properties as RepositoryItemPopupBaseAutoSearchEdit; } }
		public override void Reset() {
			base.Reset();
			CheckInplaceAutoSearchText();
		}
		protected override void OnEditorKeyPress(KeyPressEventArgs e) {
			base.OnEditorKeyPress(e);
			if(!e.Handled && CanProcessAutoSearchText) {
				ProcessChar(e);
			}
		}
		protected virtual bool CanProcessAutoSearchText { 
			get { 
				return !Properties.ReadOnly && Properties.TextEditStyle != TextEditStyles.HideTextEditor; 
			} 
		}
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual string AutoSearchText {
			get { return autoSearchText; }
			set {
				if(value == null) value = string.Empty;
				if(Properties.CharacterCasing != CharacterCasing.Normal) {
					value = Properties.CharacterCasing == CharacterCasing.Upper ? value.ToUpper() : value.ToLower();
				}
				autoSearchText = value; }
		}
		protected internal string GetAutoSearchTextFilter() {
			if(!IsMaskBoxAvailable || SelectionLength == 0) return AutoSearchText;
			if(SelectionStart < AutoSearchText.Length) return AutoSearchText.Substring(0, SelectionStart);
			return AutoSearchText;
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.Handled) return;
			if(e.KeyData == Keys.Escape) {
				if(AutoSearchText != "") {
					CheckAutoSearchText(true);
					UpdateMaskBoxDisplayText();
					SelectAll();
				}
				return;
			}
			if((e.KeyData == Keys.Left || e.KeyData == Keys.Right) && CanProcessAutoSearchText) {
				ProcessAutoSearchNavKey(e);
			}
		}
		protected virtual bool AllowAutoSearchSelectionLength { get { return true; } }
		protected virtual void ProcessAutoSearchNavKey(KeyEventArgs e) {
			if(!CanProcessAutoSearchText) return;
			if(AutoSearchText == string.Empty) return;
			if(Properties.TextEditStyle == TextEditStyles.DisableTextEditor) {
				bool leftArrow = e.KeyData == Keys.Left;
				if(leftArrow) {
					if(AutoSearchText.Length > 0) {
						AutoSearchText = AutoSearchText.Substring(0, AutoSearchText.Length - 1);
					}
				} else {
					if(IsMaskBoxAvailable) {
						if(AutoSearchText.Length == 0) return;
					}
					AutoSearchText = Text.Substring(0, Math.Min(AutoSearchText.Length + 1, Text.Length));
				}
				e.Handled = true;
			} else {
				bool leftArrow = e.KeyData == Keys.Left;
				int selectionStart = SelectionStart;
				int selectionLength = SelectionLength;
				if(leftArrow) {
					if(selectionStart > 0) {
						selectionStart --;
						selectionLength ++;
					}
				} else {
					if(selectionStart < Text.Length) {
						selectionStart ++;
						selectionLength = Math.Max(0, selectionLength - 1);
					}
				}
				bool found = FindCurrentText() != -1;
				if(selectionLength > 0 && !found) selectionLength = 0;
				SelectionStart = selectionStart;
				if(AllowAutoSearchSelectionLength) SelectionLength = selectionLength;
				if(found && (selectionStart > 0 || selectionLength > 0)) AutoSearchText = Text.Substring(0, Math.Min(selectionStart, Text.Length));
				e.Handled = true;
			}
			LayoutChanged();
		}
		const char backSpaceChar = '\b';
		protected internal virtual void ProcessChar(KeyPressEventArgs e) {
			ProcessAutoSearchChar(e);
		}
		protected virtual void ProcessAutoSearchChar(KeyPressEventArgs e) {
			if(Properties.ReadOnly) return;
			char charCode = e.KeyChar;
			if(Properties.CharacterCasing != CharacterCasing.Normal) {
				charCode = Properties.CharacterCasing == CharacterCasing.Lower ? Char.ToLower(e.KeyChar) : Char.ToUpper(e.KeyChar);
			}
			if(Char.IsControl(charCode) && charCode != backSpaceChar) return;
			if(IsMaskBoxAvailable && Properties.Mask.MaskType != MaskType.None) return;
			KeyPressHelper helper = (IsMaskBoxAvailable ? new KeyPressHelper(MaskBox.MaskBoxText, SelectionStart, SelectionLength, Properties.MaxLength) :
				new KeyPressHelper(AutoSearchText, Properties.MaxLength));
			helper.ProcessChar(e.KeyChar);
			AutoSearchText = helper.Text;
			e.Handled = true;
			ProcessFindItem(helper, charCode);
		}
		protected virtual int FindCurrentText() { return FindItem(Text, 0); }
		protected virtual int FindItem(string text, int startIndex) {
			return -1;
		}
		protected virtual void FindUpdatePopupSelectedItem(int itemIndex) {  }
		protected virtual void FindUpdateEditValue(int itemIndex, bool jopened) { }
		protected virtual void DoImmediatePopup(int itemIndex, char pressedKey) { 
			if(pressedKey != backSpaceChar)
				ShowPopup();
		}
		protected override void OnPopupClosing(CloseUpEventArgs e) {
			if(e.AcceptValue || IsClearAutoSearchOnCancelPopup)
			AutoSearchText = string.Empty;
			base.OnPopupClosing(e);
		}
		protected virtual bool IsClearAutoSearchOnCancelPopup { get { return true; } }
		protected virtual bool IsImmediatePopup { get { return Properties.ImmediatePopup; } }
		protected virtual void ProcessFindItem(KeyPressHelper helper, char pressedKey) {
			if(Properties.ReadOnly) return;
			int selectionStart = helper.SelectionStart;
			string searchText = AutoSearchText;
			int itemIndex = FindItem(searchText, 0);
			bool isOpened = IsPopupOpen;
			if(IsImmediatePopup) {
				DoImmediatePopup(itemIndex, pressedKey);
				if(!isOpened && IsPopupOpen) 
					itemIndex = FindItem(searchText, 0);
			}
			if(IsPopupOpen) {
				FindUpdatePopupSelectedItem(itemIndex);
			}
			if(itemIndex != -1) {
				FindUpdateEditValue(itemIndex, false);
				if(IsMaskBoxAvailable) {
					UpdateMaskBoxDisplayText();
					selectionStart = helper.GetCorrectedAutoSearchSelectionStart(Text, pressedKey);
					SelectionStart = selectionStart;
					if(AllowAutoSearchSelectionLength) SelectionLength = Text.Length - selectionStart;
				}
			}
			else {
				if(IsMaskBoxAvailable) {
					Properties.LockFormatParse = true;
					FindUpdateEditValueAutoSearchText();
					UpdateMaskBoxDisplayText();
					SelectionStart = Math.Max(0, selectionStart);
					SelectionLength = 0;
				} else
					AutoSearchText = AutoSearchText.Length > 1 ? AutoSearchText.Substring(0, AutoSearchText.Length - 1) : string.Empty;
			}
			LayoutChanged();
		}
		protected virtual void FindUpdateEditValueAutoSearchText() {
			if(Properties.ReadOnly) return;
			SetTextCore(AutoSearchText);
		}
		protected override void OnEnter(EventArgs e) {
			if(!IsPopupOpen && InplaceType == InplaceType.Standalone) 
				CheckAutoSearchText();
			base.OnEnter(e);
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			CheckAutoSearchText();
		}
		protected override void OnLeave	(EventArgs e) { 
			if(InplaceType == InplaceType.Standalone)
			CheckInplaceAutoSearchText();
			base.OnLeave(e);
		}
		protected void CheckInplaceAutoSearchText() {
			if(InplaceType == InplaceType.Standalone)
				CheckAutoSearchText();
		}
		protected void CheckAutoSearchText() { CheckAutoSearchText(false); }
		protected internal virtual void CheckAutoSearchText(bool emptyAutoSearchText) {
			string prevText = AutoSearchText;
			if((!IsPopupOpen && !IsEditorActive) || emptyAutoSearchText) { 
				AutoSearchText = string.Empty;
			}
			if(AutoSearchText.Length > 0) {
				string text = Text.ToLower();
				string stext = AutoSearchText.ToLower();
				if(!text.StartsWith(stext)) {
					AutoSearchText = string.Empty;
				}
			}
			if(prevText != AutoSearchText) {
				ViewInfo.MatchedString = string.Empty;
				Invalidate();
			}
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class PopupBaseAutoSearchEditViewInfo : PopupBaseEditViewInfo {
		public PopupBaseAutoSearchEditViewInfo(RepositoryItem item) : base(item) {	}
		public new PopupBaseAutoSearchEdit OwnerEdit { get { return base.OwnerEdit as PopupBaseAutoSearchEdit; } }
		protected override void UpdateFromEditor() {
			base.UpdateFromEditor();
			if(OwnerEdit == null) return;
			string autoText = OwnerEdit.AutoSearchText;
			if(!Enabled || autoText.Length == 0) MatchedString = "";
			else {
				MatchedString = autoText;
			}
		}
	}
}
