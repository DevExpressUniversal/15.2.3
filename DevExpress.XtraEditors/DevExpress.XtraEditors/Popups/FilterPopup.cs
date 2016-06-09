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
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.Utils.Controls;
using System.ComponentModel;
namespace DevExpress.XtraEditors.Repository {
	[ToolboxItem(false)]
	public class FilterCheckedListBoxControl : CheckedListBoxControl {
		protected override BaseCheckedListBoxControl.CheckedIndexCollection CheckedIndicesCore {
			get { return CheckedIndicesInternal; }
		}
		protected override void SetItemCheckStateCore(int index, CheckState value) {
			Items[index].CheckStateInternal = value;
		}
	}
	public class FilterPopupContainerForm : BlobBasePopupForm {
		IFilterItems filterItems;
		CheckedListBoxControl checkListBox;
		public FilterPopupContainerForm(BlobBaseEdit ownerEdit, IFilterItems filterItems)
			: base(ownerEdit) {
			this.filterItems = filterItems;
			this.checkListBox = CreateCheckListBox();
			SubscribeListEvents();
			this.Controls.Add(checkListBox);
			UpdateCheckListBox();
			if(!IsDeferredFilling)
				FillList();
		}
		public IFilterItems FilterItems { get { return filterItems; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(CheckListBox != null) {
					this.checkListBox.Dispose();
					this.checkListBox = null;
				}
			}
			base.Dispose(disposing);
		}
		protected virtual bool IsDeferredFilling { get { return false; } }
		protected override Control EmbeddedControl { get { return CheckListBox; } }
		public CheckedListBoxControl CheckListBox { get { return checkListBox; } protected set { checkListBox = value; } }
		protected virtual CheckedListBoxControl CreateCheckListBox() {
			CheckedListBoxControl res = new FilterCheckedListBoxControl();
			InitializeCheckListBox(res);
			return res;
		}
		protected virtual void InitializeCheckListBox(CheckedListBoxControl checkedListBox) {
			checkedListBox.BorderStyle = BorderStyles.Simple;
			checkedListBox.Appearance.Assign(OwnerEdit.Properties.AppearanceDropDown);
			checkedListBox.LookAndFeel.ParentLookAndFeel = OwnerEdit.LookAndFeel;
			checkedListBox.Visible = false;
			checkedListBox.CheckOnClick = true;
		}
		protected void UpdateCheckListBox() {
			CheckListBox.BeginUpdate();
			try {
				CheckListBox.Appearance.Assign(ViewInfo.PaintAppearanceContent);
			} finally {
				CheckListBox.EndUpdate();
			}
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) {
				e.Handled = true;
				OwnerEdit.ClosePopup();
				return;
			}
			base.ProcessKeyDown(e);
		}
		protected override bool ProcessEscapeKeyDown(KeyEventArgs e) {
			if(CheckListBox.IncrementalSearch && !string.IsNullOrEmpty(CheckListBox.CurrentSearch))
				return true;
			return base.ProcessEscapeKeyDown(e);
		}
		public override void ShowPopupForm() {
			BeginControlUpdate();
			try {
				UpdateCheckListBox();
			} finally {
				EndControlUpdate();
			}
			base.ShowPopupForm();
			UpdateOKButtonEnabled();
			FocusFormControl(CheckListBox);
		}
		protected virtual CheckState ShowAllCheckState {
			get {
				return CheckedListBoxItem.GetCheckState(FilterItems.CheckState);
			}
		}
		protected virtual void FillList() {
			CheckListBox.Items.BeginUpdate();
			try {
				AddShowAllItem();
				AddItems();
			} finally {
				CheckListBox.Items.EndUpdate();
			}
		}
		protected virtual void AddShowAllItem() {
			CheckListBox.Items.Add(GetShowAllItemCaption(), ShowAllCheckState);
		}
		protected virtual void AddItems() {
			CheckListBox.BeginUpdate();
			try {
				for(int i = 0; i < FilterItems.Count; i++) {
					CheckListBox.Items.Add(FilterItems[i], FilterItems[i].IsChecked);
				}
			} finally {
				CheckListBox.EndUpdate();
			}
		}
		protected virtual string GetShowAllItemCaption() {
			return Localizer.Active.GetLocalizedString(StringId.FilterShowAll);
		}
		protected virtual void SubscribeListEvents() {
			CheckListBox.ItemCheck += OnCheckListBoxItemCheck;
		}
		protected virtual void UnsubscribeListEvents() {
			CheckListBox.ItemCheck -= OnCheckListBoxItemCheck;
		}
		protected virtual void SetFilterItemCheck(int index, bool? isChecked) {
			((IFilterItem)CheckListBox.Items[index].Value).IsChecked = isChecked;
		}
		protected virtual void OnCheckListBoxItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			if(e.Index > 0) {
				SetFilterItemCheck(e.Index, e.State == CheckState.Indeterminate ? null : (bool?)(e.State == CheckState.Checked));
				UnsubscribeListEvents();
				try {
					CheckListBox.SetItemCheckState(0, ShowAllCheckState);
				} finally {
					SubscribeListEvents();
				}
			} else {
				CheckAllItems(e.State);
			}
			UpdateOKButtonEnabled();
		}
		protected virtual void UpdateOKButtonEnabled() {
			OkButton.Enabled = FilterItems.CanAccept;
		}
		protected virtual void CheckAllItems(CheckState state) {
			if(state == CheckState.Indeterminate) return;
			FilterItems.CheckAllItems(state == CheckState.Checked);
			UnsubscribeListEvents();
			try {
				if(state == CheckState.Checked)
					CheckListBox.CheckAll();
				else
					CheckListBox.UnCheckAll();
			} finally {
				SubscribeListEvents();
			}
		}
	}
	public class FilterPopupContainerEdit : BlobBaseEdit {
		IFilterItems filterItems;
		public Size PopupFormSize { get { return PopupForm.Size; } }
		protected IFilterItems FilterItems { get { return filterItems; } set { filterItems = value; } }
		public FilterPopupContainerEdit(IFilterItems filterItems)
			: base() {
			this.filterItems = filterItems;
		}
		void ApplyFilter() {
			FilterItems.ApplyFilter();
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new FilterPopupContainerForm(this, FilterItems);
		}
		protected internal override void ClosePopup(PopupCloseMode closeMode) {
			base.ClosePopup(closeMode);
			if(closeMode == PopupCloseMode.Normal)
				ApplyFilter();
		}
	}
}
