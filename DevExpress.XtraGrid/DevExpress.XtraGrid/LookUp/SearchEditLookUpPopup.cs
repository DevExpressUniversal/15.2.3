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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid.Localization;
namespace DevExpress.XtraGrid.Editors {
	[ToolboxItem(false)]
	public partial class SearchEditLookUpPopup : XtraUserControl {
		RepositoryItemSearchLookUpEdit owner;
		GridControl grid;
		public SearchEditLookUpPopup() {
			InitializeComponent();
		}
		internal SearchEditLookUpPopup(RepositoryItemSearchLookUpEdit owner)
			: this() {
			this.owner = owner;
			if(btFind.Text == "Find") btFind.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.FindControlFindButton);
			if(btClear.Text == "Clear") btClear.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.FindControlClearButton);
			if(btAddNew.Text == "Add New") btAddNew.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.SearchLookUpAddNewButton);
		}
		[Browsable(false)]
		public bool IsFocused {
			get {
				if(ContainsFocus) return true;
				return Grid != null && Grid.IsFocused;
			}
		}
		protected internal virtual void BeforeShowing() {
			lciAddNew.Visibility = Owner.ShowAddNewButton ? LayoutVisibility.Always : LayoutVisibility.Never;
			lciClear.Visibility = Owner.ShowClearButton ? LayoutVisibility.Always : LayoutVisibility.Never;
			lcgAction.Visibility = CheckHasVisibleItems(lcgAction) ? LayoutVisibility.Always : LayoutVisibility.Never;
			if(OwnerEdit != null) {
				teFind.MenuManager = OwnerEdit.MenuManager;
			}
			teFind.EditValue = string.Empty;
			teFind.Focus();
		}
		protected bool CheckHasVisibleItems(LayoutControlGroup group) {
			foreach(BaseLayoutItem item in group.Items) {
				LayoutControlItem controlItem = item as LayoutControlItem;
				if(controlItem == null) continue;
				if(controlItem.Visibility != LayoutVisibility.Never && controlItem.Control != null && !(controlItem.Control is LabelControl)) return true;
			}
			return false;
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			Timer t = new Timer();
			t.Interval = 100;
			t.Tick += delegate(object sender, EventArgs ee) {
				((Timer)sender).Dispose();
				lciGrid.ContentVisible = true;
			};
			t.Start();
		}
		protected SearchLookUpEdit OwnerEdit { get { return Owner == null ? null : Owner.OwnerEdit; } }
		protected RepositoryItemSearchLookUpEdit Owner { get { return owner; } }
		protected PopupSearchLookUpEditForm OwnerForm {
			get {
				if(Owner == null || Owner.OwnerEdit == null || Owner.OwnerEdit == null) return null;
				return Owner.OwnerEdit.PopupFormCore as PopupSearchLookUpEditForm;
			}
		}
		protected GridControl Grid { get { return grid; } }
		protected internal GridView View { get { return Grid == null ? null : Grid.MainView as GridView; } }
		internal void SetupGrid(GridControl grid) {
			this.grid = grid;
			Controls.Add(grid);
			lciGrid.BeginInit();
			Control prev = lciGrid.Control;
			lciGrid.Control = null;
			lciGrid.Control = grid;
			lciGrid.ContentVisible = false;
			prev.Dispose();
			lciGrid.EndInit();
			if(View != null) {
				grid.KeyDown -= View_KeyDown;
				grid.KeyDown += View_KeyDown;
				View.BeginUpdate();
				View.OptionsNavigation.UseTabKey = false;
				View.CancelUpdate();
			}
		}
		void View_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyData == Keys.Up && View != null) {
				if(View.FocusedRowHandle == View.GetVisibleRowHandle(0)) {
					e.Handled = true;
					FindTextBox.Focus();
				}
			}
		}
		public Control Find(string name) {
			Control[] res = Controls.Find(name, true);
			if(res != null && res.Length > 0) return res[0];
			return null;
		}
		public string FindText { get { return teFind.Text; } }
		public virtual TextEdit FindTextBox { get { return teFind; } }
		protected virtual void btFind_Click(object sender, EventArgs e) {
			DoFind(true);
		}
		protected virtual void teFind_EditValueChanged(object sender, EventArgs e) {
			DoFind(false);
		}
		int lockFind = 0;
		protected void DoFind(bool isButtonClick) {
			if(lockFind != 0) return;
			if(OwnerForm != null) OwnerForm.FindClick(FindText, isButtonClick);
		}
		protected void teFind_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyData == Keys.Down) {
				e.Handled = true;
				Grid.Focus();
				if(View.FocusedRowHandle == GridControl.InvalidRowHandle)
					View.FocusedRowHandle = View.GetVisibleRowHandle(0);
				View.MakeRowVisible(View.FocusedRowHandle, false);
			}
			if(e.KeyData == Keys.Enter) {
				e.Handled = true;
				lockFind++;
				try {
					FindTextBox.DoValidate();
				}
				finally {
					lockFind--;
				}
				DoFind(true);
			}
		}
		internal bool IsShouldCloseOnEnter() {
			if(!Grid.Focused) return false;
			if(View.IsFocusedRowLoaded) return true;
			return false;
		}
		private void btAddNew_Click(object sender, EventArgs e) {
			if(OwnerForm != null) OwnerForm.AddNewClick();
		}
		private void btClear_Click(object sender, EventArgs e) {
			if(OwnerForm != null) OwnerForm.ClearClick();
		}
	}
}
