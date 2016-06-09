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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Extensions.Native;
namespace DevExpress.Snap.Extensions.UI {
	public class FilterPopupUserControl : OkCancelUserControl {
		FilterItems filterItems;
		CheckedListBoxControl checkListBox;
		FilterPopupUserControl() { }
		public FilterPopupUserControl(FilterItems filterItems) {
			this.filterItems = filterItems;
			this.checkListBox = new FilterCheckedListBoxControl();
			this.checkListBox.TabIndex = 0;
			this.checkListBox.CheckOnClick = true;
			this.checkListBox.Dock = DockStyle.Fill;
			this.checkListBox.IncrementalSearch = true;
			this.checkListBox.SelectionMode = SelectionMode.MultiExtended;
			this.checkListBox.BorderStyle = BorderStyles.NoBorder;
			SubscribeListEvents();
			this.Controls.Add(checkListBox);
			FillList();
			okCancelButtonsPanel.SendToBack();
		}
		protected override void btnOK_Click(object sender, EventArgs e) {
			filterItems.ApplyFilter();
			base.btnOK_Click(sender, e);
		}
		FilterItems FilterItems { get { return filterItems; } }
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (CheckListBox != null) {
					this.checkListBox.Dispose();
					this.checkListBox = null;
				}
			}
			base.Dispose(disposing);
		}
		CheckedListBoxControl CheckListBox { get { return checkListBox; } }
		CheckState ShowAllCheckState {
			get {
				return CheckedListBoxItem.GetCheckState(FilterItems.CheckState);
			}
		}
		void FillList() {
			CheckListBox.Items.BeginUpdate();
			try {
				AddShowAllItem();
				AddItems();
			}
			finally {
				CheckListBox.Items.EndUpdate();
			}
		}
		void AddShowAllItem() {
			CheckListBox.Items.Add(GetShowAllItemCaption(), ShowAllCheckState);
		}
		void AddItems() {
			CheckListBox.BeginUpdate();
			try {
				foreach (object item in FilterItems) {
					CheckListBox.Items.Add(item, ((FilterItem)item).IsChecked);
				}
			}
			finally {
				CheckListBox.EndUpdate();
			}
		}
		protected string GetShowAllItemCaption() {
			return Localizer.Active.GetLocalizedString(StringId.FilterShowAll);
		}
		void SubscribeListEvents() {
			CheckListBox.ItemCheck += OnCheckListBoxItemCheck;
		}
		void UnsubscribeListEvents() {
			CheckListBox.ItemCheck -= OnCheckListBoxItemCheck;
		}
		void SetFilterItemCheck(int index, bool? isChecked) {
			((FilterItem)CheckListBox.Items[index].Value).IsChecked = isChecked;
		}
		void OnCheckListBoxItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			if (e.Index > 0) {
				SetFilterItemCheck(e.Index, e.State == CheckState.Indeterminate ? null : (bool?)(e.State == CheckState.Checked));
				UnsubscribeListEvents();
				try {
					CheckListBox.SetItemCheckState(0, ShowAllCheckState);
				}
				finally {
					SubscribeListEvents();
				}
			}
			else {
				CheckAllItems(e.State);
			}
		}
		void CheckAllItems(CheckState state) {
			if (state == CheckState.Indeterminate) return;
			FilterItems.CheckAllItems(state == CheckState.Checked);
			UnsubscribeListEvents();
			try {
				if (state == CheckState.Checked)
					CheckListBox.CheckAll();
				else
					CheckListBox.UnCheckAll();
			}
			finally {
				SubscribeListEvents();
			}
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterPopupUserControl));
			this.okCancelButtonsPanel.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this, "$this");
			this.Name = "FilterPopupUserControl";
			this.okCancelButtonsPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
	}
}
