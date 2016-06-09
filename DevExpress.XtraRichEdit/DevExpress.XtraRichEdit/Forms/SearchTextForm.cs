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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraTab;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Office.Utils;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.xtraTabControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.cbFndSearchString")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.pgFind")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.pgReplace")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.chbFndMatchCase")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.chbFndFindWholeWord")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.cbFndFindDirection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.cbRplReplaceString")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.cbRplSearchString")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.chbRplMatchCase")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.cbRplFindDirection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.chbRplFindWholeWord")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.btnReplaceAll")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.btnFindNext")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.btnReplaceNext")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.lblFndDirection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.lblRplSearchString")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.lblRplReplaceString")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.lblRplDirection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.lblFndSearchString")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.chbFndRegex")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SearchTextForm.chbRplRegex")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class SearchTextForm : XtraForm {
		#region Fields
		readonly static List<string> searchStrings = new List<string>();
		readonly static List<string> replaceStrings = new List<string>();
		static Point formPreviousLocation;
		readonly SearchFormControllerBase controller;
		bool keepLocation = true;
		#endregion
		SearchTextForm() {
			InitializeComponent();
		}
		public SearchTextForm(SearchFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			ActivePage = controllerParameters.ActivePage;
			Initialize();
			PopulateSearchStringComboBox(this.cbFndSearchString);
			PopulateSearchStringComboBox(this.cbRplSearchString);
			PopulateReplaceStringComboBox(this.cbRplReplaceString);
			UpdateActivePage();
		}
		#region Properties
		public virtual SearchFormActivePage ActivePage { get { return GetActivePage(); } set { SetActivePage(value); } }
		public virtual bool KeepLocation { get { return keepLocation; } set { keepLocation = value; } }
		protected SearchFormControllerBase Controller { get { return controller; } }
		protected RichEditControl Control { get { return (RichEditControl)Controller.Control; } }
		protected TextSearchDirection SearchDirection { get { return Controller.Direction; } }
		DocumentModel DocumentModel { get { return Control.DocumentModel; } }
		SearchParameters FindParameters { get { return DocumentModel.SearchParameters; } }
		DevExpress.XtraTab.XtraTabPage SelectedTab { get { return xtraTabControl.SelectedTabPage; } set { xtraTabControl.SelectedTabPage = value; } }
		List<string> SearchStrings { get { return searchStrings; } }
		List<string> ReplaceStrings { get { return replaceStrings; } }
		string LastSearchString {
			get {
				if (SearchStrings.Count > 0)
					return SearchStrings[SearchStrings.Count - 1];
				else
					return String.Empty;
			}
		}
		string LastReplaceString {
			get {
				if (ReplaceStrings.Count > 0)
					return ReplaceStrings[ReplaceStrings.Count - 1];
				else
					return String.Empty;
			}
		}
		#endregion
		protected virtual IDXPopupMenu<RichEditCommandId> CreateSearchRegexPopupMenu(IButtonEditAdapter edit) {
			SearchRegexPopupMenuBuilder builder = new SearchRegexPopupMenuBuilder(Control, new WinFormsRichEditMenuBuilderUIFactory(), edit);
			return builder.CreatePopupMenu();
		}
		protected virtual IDXPopupMenu<RichEditCommandId> CreateReplaceRegexPopupMenu(IButtonEditAdapter edit) {
			ReplaceRegexPopupMenuBuilder builder = new ReplaceRegexPopupMenuBuilder(Control, new WinFormsRichEditMenuBuilderUIFactory(), edit);
			return builder.CreatePopupMenu();
		}
		protected internal virtual SearchFormControllerBase CreateController(SearchFormControllerParameters controllerParameters) {
			return new FindAndReplaceFormController(controllerParameters);
		}
		protected internal virtual void Initialize() {
			string searchString;
			if (Controller.TryGetSearchStringFromSelection(out searchString)) {
				this.cbFndSearchString.Text = searchString;
				this.cbRplSearchString.Text = searchString;
			}
			else {
				this.cbFndSearchString.Text = LastSearchString;
				this.cbRplSearchString.Text = LastSearchString;
				if (DocumentModel.Selection.Length > 0) {
					this.cbFndFindDirection.SelectedIndex = (int)TextSearchDirection.Down;
					this.cbRplFindDirection.SelectedIndex = (int)TextSearchDirection.Down;
				}
			}
			this.cbRplReplaceString.Text = LastReplaceString;
		}
		protected internal virtual SearchFormActivePage GetActivePage() {
			if (SelectedTab == this.pgFind)
				return SearchFormActivePage.Find;
			if (SelectedTab == this.pgReplace)
				return SearchFormActivePage.Replace;
			return SearchFormActivePage.Undefined;
		}
		protected internal virtual void SetActivePage(SearchFormActivePage activePage) {
			switch (activePage) {
				default:
				case SearchFormActivePage.Find:
					SelectedTab = this.pgFind;
					break;
				case SearchFormActivePage.Replace:
					SelectedTab = this.pgReplace;
					break;
			}
		}
		protected internal virtual void PopulateSearchStringComboBox(ComboBoxEdit comboBox) {
			comboBox.Properties.Items.Clear();
			int count = SearchStrings.Count;
			if (count == 0)
				return;
			for (int i = 0; i < count; i++)
				comboBox.Properties.Items.Add(SearchStrings[i]);
		}
		protected internal virtual void PopulateReplaceStringComboBox(ComboBoxEdit comboBox) {
			comboBox.Properties.Items.Clear();
			int count = ReplaceStrings.Count;
			if (count == 0)
				return;
			for (int i = 0; i < count; i++)
				comboBox.Properties.Items.Add(ReplaceStrings[i]);
		}
		void OnSelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			UpdateActivePage();
		}
		protected internal virtual void UpdateActivePage() {
			if (Controller == null)
				return;
			switch (ActivePage) {
				default:
				case SearchFormActivePage.Find:
					UpdateFindPage();
					Controller.Direction = GetSearchDirection(this.cbFndFindDirection);
					break;
				case SearchFormActivePage.Replace:
					UpdateReplacePage();
					Controller.Direction = GetSearchDirection(this.cbRplFindDirection);
					break;
			}
		}
		protected internal virtual void UpdateFindPage() {
			this.cbFndSearchString.Select();
			this.cbFndSearchString.Text = Controller.SearchString;
			this.chbFndMatchCase.Checked = Controller.CaseSensitive;
			this.chbFndFindWholeWord.Checked = Controller.FindWholeWord;
			this.chbFndRegex.Checked = Controller.RegularExpression;
			this.btnReplaceNext.Visible = false;
			this.btnReplaceAll.Visible = false;
		}
		protected internal virtual void UpdateReplacePage() {
			this.cbRplSearchString.Select();
			this.cbRplSearchString.Text = Controller.SearchString;
			this.cbRplReplaceString.Text = Controller.ReplaceString;
			this.chbRplMatchCase.Checked = Controller.CaseSensitive;
			this.chbRplFindWholeWord.Checked = Controller.FindWholeWord;
			this.chbRplRegex.Checked = Controller.RegularExpression;
			this.btnReplaceNext.Visible = true;
			this.btnReplaceAll.Visible = true;
		}
		void OnFindNextButtonClick(object sender, EventArgs e) {
			Controller.Find();
		}
		void OnReplaceNextButtonClick(object sender, EventArgs e) {
			Controller.Replace();
			this.btnFindNext.Select();
		}
		void OnReplaceAllButtonClick(object sender, EventArgs e) {
			Controller.ReplaceAll();
		}
		void CancelButton_Click(object sender, EventArgs e) {
			this.Close();
		}
		void SearchTextForm_Closed(object sender, FormClosedEventArgs e) {
			if (Controller != null)
				Controller.Dispose();
			if (KeepLocation)
				formPreviousLocation = this.Location;
		}
		void SearchTextForm_Load(object sender, EventArgs e) {
			if (formPreviousLocation != Point.Empty)
				this.Location = formPreviousLocation;
			if (Controller != null && Control.ReadOnly)
				this.pgReplace.PageVisible = false;
		}
		void cbFndSearchString_EditValueChanged(object sender, EventArgs e) {
			string text = this.cbFndSearchString.Text;
			Controller.SearchString = text;
			OnSearchStringChanged(text);
		}
		void cbRplSearchString_EditValueChanged(object sender, EventArgs e) {
			string text = this.cbRplSearchString.Text;
			Controller.SearchString = text;
			OnSearchStringChanged(text);
		}
		void OnSearchStringChanged(string text) {
			if (!String.IsNullOrEmpty(text))
				EnableButtons();
			else
				DisableButtons();
		}
		protected internal virtual void EnableButtons() {
			this.btnFindNext.Enabled = true;
			this.btnReplaceNext.Enabled = true;
			this.btnReplaceAll.Enabled = true;
		}
		protected internal virtual void DisableButtons() {
			this.btnFindNext.Enabled = false;
			this.btnReplaceNext.Enabled = false;
			this.btnReplaceAll.Enabled = false;
		}
		void cbReplaceReplaceString_EditValueChanged(object sender, EventArgs e) {
			Controller.ReplaceString = this.cbRplReplaceString.Text;
		}
		void chbFndMatchCase_CheckedChanged(object sender, EventArgs e) {
			Controller.CaseSensitive = this.chbFndMatchCase.Checked;
		}
		void chbFndFindWholeWord_CheckedChanged(object sender, EventArgs e) {
			Controller.FindWholeWord = this.chbFndFindWholeWord.Checked;
		}
		void chbRplMatchCase_CheckedChanged(object sender, EventArgs e) {
			Controller.CaseSensitive = this.chbRplMatchCase.Checked;
		}
		void chbRplFindWholeWord_CheckedChanged(object sender, EventArgs e) {
			Controller.FindWholeWord = this.chbRplFindWholeWord.Checked;
		}
		void chbRegex_CheckedChanged(object sender, EventArgs e) {
			bool isChecked = this.chbFndRegex.Checked;
			Controller.RegularExpression = isChecked;
			this.cbFndSearchString.Properties.Buttons[1].Enabled = isChecked;
		}
		void chbRplRegex_CheckedChanged(object sender, EventArgs e) {
			bool isChecked = this.chbRplRegex.Checked;
			Controller.RegularExpression = isChecked;
			this.cbRplSearchString.Properties.Buttons[1].Enabled = isChecked;
			this.cbRplReplaceString.Properties.Buttons[1].Enabled = isChecked;
		}
		void cbFndFindDirection_SelectedIndexChanged(object sender, EventArgs e) {
			Controller.Direction = GetSearchDirection(this.cbFndFindDirection);
		}
		void cbRplFindDirection_SelectedIndexChanged(object sender, EventArgs e) {
			Controller.Direction = GetSearchDirection(this.cbRplFindDirection);
		}
		protected TextSearchDirection GetSearchDirection(ComboBoxEdit comboBox) {
			return (TextSearchDirection)comboBox.SelectedIndex;
		}
		void SearchString_AddingMRUItem(object sender, AddingMRUItemEventArgs e) {
			SearchStrings.Add((string)e.Item);
		}
		void ReplaceString_AddingMRUItem(object sender, AddingMRUItemEventArgs e) {
			ReplaceStrings.Add((string)e.Item);
		}
		private void cbSearchString_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if (e.Button.Kind != ButtonPredefines.Right)
				return;
			ButtonEdit editor = sender as ButtonEdit;
			if (editor == null)
				return;
			DXPopupMenu menu = CreateSearchRegexPopupMenu(new ButtonEditAdapter(editor)) as DXPopupMenu;
			if (menu != null)
				MenuManagerHelper.ShowMenu(menu, LookAndFeel, Control.MenuManager, this, GetMenuLocation(editor, e));
		}
		private Point GetMenuLocation(ButtonEdit editor, ButtonPressedEventArgs e) {
			ButtonEditViewInfo evi = editor.GetViewInfo() as ButtonEditViewInfo;
			EditorButtonObjectInfoArgs bvi = evi.ButtonInfoByButton(e.Button);
			Point pt = new Point(bvi.Bounds.Right, bvi.Bounds.Top);
			pt = editor.PointToScreen(pt);
			return PointToClient(pt);
		}
		private void cbRplReplaceString_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if (e.Button.Kind != ButtonPredefines.Right)
				return;
			ButtonEdit editor = sender as ButtonEdit;
			if (editor == null)
				return;
			DXPopupMenu menu = CreateReplaceRegexPopupMenu(new ButtonEditAdapter(editor)) as DXPopupMenu;
			if (menu != null)
				MenuManagerHelper.ShowMenu(menu, LookAndFeel, Control.MenuManager, this, GetMenuLocation(editor, e));
		}
	}
	#region FindAndReplaceFormController
	public class FindAndReplaceFormController : SearchFormControllerBase {
		public FindAndReplaceFormController(SearchFormControllerParameters controllerParameters)
			: base(controllerParameters) {
		}
		protected override SearchHelperBase CreateSearchHelper() {
			return new SearchHelper(Control);
		}
	}
	#endregion
	#region SearchHelper
	public class SearchHelper : SearchHelperBase {
		public SearchHelper(IRichEditControl control)
			: base(control) {
			SubscribeRichEditControlEvents();
		}
		#region Subscribe/Unsubscribe to events
		void SubscribeRichEditControlEvents() {
			if (Control != null && Control.InnerControl != null)
				Control.InnerControl.SearchComplete += OnSearchComplete;
		}
		void UnsubscribeRichEditControlEvents() {
			if (Control != null && Control.InnerControl != null)
				Control.InnerControl.SearchComplete -= OnSearchComplete;
		}
		void OnSearchComplete(object sender, SearchCompleteEventArgs e) {
			OnSearchComplete(e);
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeRichEditControlEvents();
			}
			base.Dispose(disposing);
		}
		protected override bool ShouldContinueSearch(string message) {
			return ShowDialog(message, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}
		protected override void OnStopSearching(string message) {
			RichEditControl control = (RichEditControl)Control;
			control.BeginInvoke(new InvokeDelegate(InvokeMethod), message);
		}
		protected override void OnInvalidRegExp(string message) {
			ShowDialog(message, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		DialogResult ShowDialog(string message, MessageBoxButtons buttons, MessageBoxIcon icon) {
			RichEditControl control = (RichEditControl)Control;
			return XtraMessageBox.Show(control.LookAndFeel, Control, message, Application.ProductName, buttons, icon);
		}
		public delegate void InvokeDelegate(string message);
		public void InvokeMethod(string message) {
			ShowDialog(message, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
	#endregion
	public class ButtonEditAdapter : IButtonEditAdapter {
		readonly ButtonEdit edit;
		public ButtonEditAdapter(ButtonEdit edit) {
			Guard.ArgumentNotNull(edit, "edit");
			this.edit = edit;
		}
		#region IButtonEditAdapter Members
		public string Text { get { return edit.Text; } set { edit.Text = value; } }
		public void Select(int start, int length) {
			edit.Select(start, length);
		}
		#endregion
	}
}
namespace DevExpress.XtraRichEdit.Menu {
}
