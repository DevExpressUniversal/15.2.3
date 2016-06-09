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
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Localization;
using System.Drawing;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BookmarkForm.lblBookmarkName")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BookmarkForm.rgSortBy")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BookmarkForm.lblSortBy")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BookmarkForm.btnDelete")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BookmarkForm.btnGoTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BookmarkForm.btnAdd")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BookmarkForm.lblSplit")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BookmarkForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BookmarkForm.edtBookmarkName")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BookmarkForm.lbBookmarkName")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class BookmarkForm : XtraForm {
		readonly IRichEditControl control;
		readonly BookmarkFormController controller;
		BookmarkForm() {
			InitializeComponent();
		}
		public BookmarkForm(BookmarkFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			UpdateForm();
		}
		public IRichEditControl Control { get { return control; } }
		public BookmarkFormController Controller { get { return controller; } }
		protected virtual BookmarkFormController CreateController(BookmarkFormControllerParameters controllerParameters) {
			return new BookmarkFormController(controllerParameters);
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void SubscribeControlsEvents() {
			this.lbBookmarkName.SelectedIndexChanged += OnlBoxBookmarkNameSelectedIndexChanged;
			this.lbBookmarkName.MouseDoubleClick += OnBookmarkNameMouseDoubleClick;
			this.edtBookmarkName.TextChanged += OnBookmarkNameTextChanged;
			this.edtBookmarkName.KeyDown += OnBookmarkNameKeyDown;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			this.lbBookmarkName.SelectedIndexChanged -= OnlBoxBookmarkNameSelectedIndexChanged;
			this.lbBookmarkName.MouseDoubleClick -= OnBookmarkNameMouseDoubleClick;
			this.edtBookmarkName.TextChanged -= OnBookmarkNameTextChanged;
			this.edtBookmarkName.KeyDown -= OnBookmarkNameKeyDown;
		}
		protected internal virtual void UpdateFormCore() {
			OnSortBySelectedIndexChanged(this, EventArgs.Empty);
			InitializeBookmarkNameEdit();
		}
		protected internal virtual void InitializeBookmarkNameEdit() {
			if (lbBookmarkName.SelectedIndex >= 0)
				edtBookmarkName.Text = lbBookmarkName.SelectedItem.ToString();
			UpdateAddButtonEnabled();
		}
		void OnlBoxBookmarkNameSelectedIndexChanged(object sender, EventArgs e) {
			edtBookmarkName.EditValue = lbBookmarkName.SelectedItem.ToString();
			edtBookmarkName.Focus();
			UpdateButtonsEnabled();
		}
		void OnBookmarkNameTextChanged(object sender, EventArgs e) {
			UpdateAddButtonEnabled();
		}
		void OnBookmarkNameMouseDoubleClick(object sender, MouseEventArgs e) {
			if (lbBookmarkName.SelectedIndex != -1)
				SelectBookmark();
		}
		void OnBookmarkNameKeyDown(object sender, KeyEventArgs e) {
			int itemsCount = lbBookmarkName.Items.Count;
			switch (e.KeyCode) {
				case Keys.Up:
					if (lbBookmarkName.SelectedIndex > 0) {
						lbBookmarkName.SelectedIndex--;
					}
					break;
				case Keys.Down:
					if (itemsCount != 0 && lbBookmarkName.SelectedIndex + 1 < itemsCount) {
						lbBookmarkName.SelectedIndex++;
					}
					break;
				case Keys.PageUp:
					if (itemsCount != 0) {
						lbBookmarkName.SelectedIndex = 0;
					}
					break;
				case Keys.PageDown:
					if (itemsCount != 0) {
						lbBookmarkName.SelectedIndex = itemsCount;
					}
					break;
				default:
					return;
			}
			e.Handled = true;
		}
		void OnAddClick(object sender, EventArgs e) {
			Controller.CreateBookmark(edtBookmarkName.Text, delegate() {
				return ShowWarningDialog() == DialogResult.Yes;
			});
		}
		protected internal virtual DialogResult ShowWarningDialog() {
			string message = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_BookmarkCreationFailing);
			RichEditControl richEditControl = (RichEditControl)Control;
			return XtraMessageBox.Show(richEditControl.LookAndFeel, control, message, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
		}
		void OnDeleteClick(object sender, EventArgs e) {
			Bookmark bookmark = (Bookmark)lbBookmarkName.SelectedItem;
			Controller.DeleteBookmark(bookmark);
			UpdateForm();
		}
		void OnGoToClick(object sender, EventArgs e) {
			SelectBookmark();
		}
		protected internal virtual void SelectBookmark() {
			Bookmark bookmark = (Bookmark)lbBookmarkName.SelectedItem;
			Controller.SelectBookmark(bookmark);
		}
		void OnSortBySelectedIndexChanged(object sender, EventArgs e) {
			IList<Bookmark> bookmarks;
			if (rgSortBy.SelectedIndex == 1)
				bookmarks = Controller.GetBookmarksSortedByLocation(false);
			else
				bookmarks = Controller.GetBookmarksSortedByName(false);
			UpdateLbBookmarkNameItems(bookmarks);
		}
		protected internal virtual void UpdateLbBookmarkNameItems(IList<Bookmark> bookmarks) {
			lbBookmarkName.Items.BeginUpdate();
			Bookmark selectedItem = (Bookmark)lbBookmarkName.SelectedItem;
			lbBookmarkName.Items.Clear();
			for (int i = 0; i < bookmarks.Count; i++) {
				if (!bookmarks[i].Name.Contains("Comment"))
					lbBookmarkName.Items.Add(bookmarks[i]);
			}
			lbBookmarkName.Items.EndUpdate();
			if (bookmarks.Count > 0) {
				if (selectedItem != null)
					lbBookmarkName.SelectedIndex = bookmarks.IndexOf(selectedItem);
				else {
					Bookmark bookmark = Controller.GetCurrentBookmark();
					lbBookmarkName.SelectedIndex = bookmarks.IndexOf(bookmark);
				}
			}
			UpdateButtonsEnabled();
		}
		protected internal virtual void UpdateButtonsEnabled() {
			if (lbBookmarkName.SelectedIndex == -1) {
				btnDelete.Enabled = false;
				btnGoTo.Enabled = false;
				edtBookmarkName.Text = String.Empty;
			}
			else {
				btnDelete.Enabled = true;
				btnGoTo.Enabled = Controller.CanSelectBoomark((Bookmark)lbBookmarkName.SelectedItem);
			}
		}
		protected internal virtual void UpdateAddButtonEnabled() {
			string bookmarkName = edtBookmarkName.Text;
			btnAdd.Enabled = !String.IsNullOrEmpty(bookmarkName) && Controller.ValidateName(bookmarkName);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			if (e.Delta < 0)
				lbBookmarkName.TopIndex++;
			else
				lbBookmarkName.TopIndex--;
		}
		private void edtBookmarkName_SizeChanged(object sender, EventArgs e) {
			AlignBookmarkNameList();
		}
		private void edtBookmarkName_LocationChanged(object sender, EventArgs e) {
			AlignBookmarkNameList();
		}
		private void lbBookmarkName_SizeChanged(object sender, EventArgs e) {
			AlignBookmarkNameList();
		}
		void AlignBookmarkNameList() {			
			Point location = lbBookmarkName.Location;
			location.Y = edtBookmarkName.Bottom;
			location.X = edtBookmarkName.Right - lbBookmarkName.Width;
			lbBookmarkName.Location = location;
		}
	}
}
