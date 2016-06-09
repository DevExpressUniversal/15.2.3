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
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Strategies;
using System.Drawing;
using DevExpress.Utils;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.lblNotInDictionary")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.lblChangeTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.lblSuggestions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnIgnore")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnChangeAll")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnAdd")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnSuggest")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnIgnoreAll")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnChange")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnOptions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnUndoLast")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.txtNotInDictionary")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.txtChangeTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.lbcSuggestions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnClose")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.btnDelete")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOutlookStyleForm.lblRepeatedWord")]
#endregion
namespace DevExpress.XtraSpellChecker.Forms {
	public class SpellingOutlookStyleForm : SpellingFormBase {
		#region Generated Fields
		protected internal System.Windows.Forms.Label lblNotInDictionary;
		protected internal System.Windows.Forms.Label lblChangeTo;
		protected internal System.Windows.Forms.Label lblSuggestions;
		protected internal DevExpress.XtraEditors.SimpleButton btnIgnore;
		protected internal DevExpress.XtraEditors.SimpleButton btnChangeAll;
		protected internal DevExpress.XtraEditors.SimpleButton btnAdd;
		protected internal DevExpress.XtraEditors.SimpleButton btnSuggest;
		protected internal DevExpress.XtraEditors.SimpleButton btnIgnoreAll;
		protected internal DevExpress.XtraEditors.SimpleButton btnChange;
		protected internal DevExpress.XtraEditors.SimpleButton btnOptions;
		protected internal DevExpress.XtraEditors.SimpleButton btnUndoLast;
		protected internal DevExpress.XtraEditors.TextEdit txtNotInDictionary;
		protected internal DevExpress.XtraEditors.TextEdit txtChangeTo;
		protected internal DevExpress.XtraEditors.ListBoxControl lbcSuggestions;
		protected internal DevExpress.XtraEditors.SimpleButton btnClose;
		protected internal DevExpress.XtraEditors.SimpleButton btnCancel;
		protected internal SimpleButton btnDelete;
		protected internal Label lblRepeatedWord;
		#endregion
		private System.ComponentModel.Container components = null;
		private static readonly object spellCheckFormResultChanged = new object();
		public SpellingOutlookStyleForm()
			: base() {
			Initialize();
		}
		public SpellingOutlookStyleForm(SpellChecker spellChecker)
			: base(spellChecker) {
			Initialize();
		}
		void Initialize() {
			InitializeComponent(); 
			InitializeIcon();
		}
		protected virtual void InitializeIcon() {
			Icon = ResourceImageHelper.CreateIconFromResources("DevExpress.XtraSpellChecker.Images.SpellChecker.ico", System.Reflection.Assembly.GetExecutingAssembly());
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpellingOutlookStyleForm));
			this.lblNotInDictionary = new System.Windows.Forms.Label();
			this.lblChangeTo = new System.Windows.Forms.Label();
			this.lblSuggestions = new System.Windows.Forms.Label();
			this.lbcSuggestions = new DevExpress.XtraEditors.ListBoxControl();
			this.txtNotInDictionary = new DevExpress.XtraEditors.TextEdit();
			this.txtChangeTo = new DevExpress.XtraEditors.TextEdit();
			this.btnIgnore = new DevExpress.XtraEditors.SimpleButton();
			this.btnChangeAll = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnSuggest = new DevExpress.XtraEditors.SimpleButton();
			this.btnIgnoreAll = new DevExpress.XtraEditors.SimpleButton();
			this.btnChange = new DevExpress.XtraEditors.SimpleButton();
			this.btnOptions = new DevExpress.XtraEditors.SimpleButton();
			this.btnUndoLast = new DevExpress.XtraEditors.SimpleButton();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.lblRepeatedWord = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.lbcSuggestions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtNotInDictionary.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtChangeTo.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblNotInDictionary, "lblNotInDictionary");
			this.lblNotInDictionary.Name = "lblNotInDictionary";
			resources.ApplyResources(this.lblChangeTo, "lblChangeTo");
			this.lblChangeTo.Name = "lblChangeTo";
			resources.ApplyResources(this.lblSuggestions, "lblSuggestions");
			this.lblSuggestions.Name = "lblSuggestions";
			resources.ApplyResources(this.lbcSuggestions, "lbcSuggestions");
			this.lbcSuggestions.ItemAutoHeight = true;
			this.lbcSuggestions.ItemHeight = 16;
			this.lbcSuggestions.Name = "lbcSuggestions";
			this.lbcSuggestions.SelectedIndexChanged += new System.EventHandler(this.lbcSuggestions_SelectedIndexChanged);
			this.lbcSuggestions.DoubleClick += new System.EventHandler(this.lbcSuggestions_DoubleClick);
			resources.ApplyResources(this.txtNotInDictionary, "txtNotInDictionary");
			this.txtNotInDictionary.Name = "txtNotInDictionary";
			this.txtNotInDictionary.Properties.ReadOnly = true;
			resources.ApplyResources(this.txtChangeTo, "txtChangeTo");
			this.txtChangeTo.Name = "txtChangeTo";
			this.txtChangeTo.Validated += new System.EventHandler(this.txtChangeTo_Validated);
			resources.ApplyResources(this.btnIgnore, "btnIgnore");
			this.btnIgnore.Name = "btnIgnore";
			this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
			resources.ApplyResources(this.btnChangeAll, "btnChangeAll");
			this.btnChangeAll.Name = "btnChangeAll";
			this.btnChangeAll.Click += new System.EventHandler(this.btnChangeAll_Click);
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			resources.ApplyResources(this.btnSuggest, "btnSuggest");
			this.btnSuggest.Name = "btnSuggest";
			this.btnSuggest.Click += new System.EventHandler(this.btnSuggest_Click);
			resources.ApplyResources(this.btnIgnoreAll, "btnIgnoreAll");
			this.btnIgnoreAll.Name = "btnIgnoreAll";
			this.btnIgnoreAll.Click += new System.EventHandler(this.btnIgnoreAll_Click);
			resources.ApplyResources(this.btnChange, "btnChange");
			this.btnChange.Name = "btnChange";
			this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
			resources.ApplyResources(this.btnOptions, "btnOptions");
			this.btnOptions.Name = "btnOptions";
			this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
			resources.ApplyResources(this.btnUndoLast, "btnUndoLast");
			this.btnUndoLast.Name = "btnUndoLast";
			this.btnUndoLast.Click += new System.EventHandler(this.btnUndoLast_Click);
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Name = "btnClose";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnClose_Click);
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			resources.ApplyResources(this.lblRepeatedWord, "lblRepeatedWord");
			this.lblRepeatedWord.Name = "lblRepeatedWord";
			this.AcceptButton = this.btnIgnore;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.Controls.Add(this.lblRepeatedWord);
			this.Controls.Add(this.lblSuggestions);
			this.Controls.Add(this.lblChangeTo);
			this.Controls.Add(this.lblNotInDictionary);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnIgnore);
			this.Controls.Add(this.txtNotInDictionary);
			this.Controls.Add(this.lbcSuggestions);
			this.Controls.Add(this.txtChangeTo);
			this.Controls.Add(this.btnChangeAll);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnSuggest);
			this.Controls.Add(this.btnIgnoreAll);
			this.Controls.Add(this.btnChange);
			this.Controls.Add(this.btnOptions);
			this.Controls.Add(this.btnUndoLast);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnCancel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SpellingOutlookStyleForm";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SpellingOutlookStyleForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.lbcSuggestions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtNotInDictionary.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtChangeTo.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private void DoChangeByDoubleClickOnListBox() {
			Suggestion = lbcSuggestions.Text;
			DoChange();
		}
		protected virtual void DoChange() {
			this.SpellingFormResult = SpellCheckOperation.Change;
		}
		protected virtual void DoDelete() {
			this.SpellingFormResult = SpellCheckOperation.Delete;
		}
		protected virtual void DoChangeAll() {
			this.SpellingFormResult = SpellCheckOperation.ChangeAll;
		}
		protected virtual void DoIgnore() {
			this.SpellingFormResult = SpellCheckOperation.Ignore;
		}
		protected virtual void DoIgnoreAll() {
			this.SpellingFormResult = SpellCheckOperation.IgnoreAll;
		}
		protected virtual void DoAddToDictionary() {
			this.SpellingFormResult = SpellCheckOperation.AddToDictionary;
		}
		protected virtual void DoCancel() {
			this.SpellingFormResult = SpellCheckOperation.Cancel;
		}
		protected virtual void DoUndo() {
			this.SpellingFormResult = SpellCheckOperation.Undo;
		}
		protected virtual void DoSuggest() {
		}
		protected virtual void DoShowOptions() {
			this.SpellingFormResult = SpellCheckOperation.Options;
		}
		private void btnIgnore_Click(object sender, System.EventArgs e) {
			DoIgnore();
		}
		private void btnChange_Click(object sender, System.EventArgs e) {
			DoChange();
		}
		private void btnDelete_Click(object sender, EventArgs e) {
			DoDelete();
		}
		private void btnAdd_Click(object sender, System.EventArgs e) {
			DoAddToDictionary();
		}
		private void btnIgnoreAll_Click(object sender, System.EventArgs e) {
			DoIgnoreAll();
		}
		private void btnChangeAll_Click(object sender, System.EventArgs e) {
			DoChangeAll();
		}
		private void btnCancel_Click(object sender, System.EventArgs e) {
			DoCancel();
		}
		private void btnUndoLast_Click(object sender, System.EventArgs e) {
			DoUndo();
		}
		private void lbcSuggestions_DoubleClick(object sender, System.EventArgs e) {
			int index = lbcSuggestions.IndexFromPoint(lbcSuggestions.PointToClient(Control.MousePosition));
			if (index >= 0)
				DoChangeByDoubleClickOnListBox();
		}
		private void btnSuggest_Click(object sender, System.EventArgs e) {
			DoSuggest();
		}
		private void btnOptions_Click(object sender, System.EventArgs e) {
			DoShowOptions();
		}
		private void btnClose_Click(object sender, System.EventArgs e) {
			this.SpellingFormResult = SpellCheckOperation.Cancel;
		}
		private void txtChangeTo_Validated(object sender, System.EventArgs e) {
			Suggestion = (sender as TextEdit).Text;
		}
		public override string Suggestion {
			get { return base.Suggestion; }
			set {
				base.Suggestion = value;
				OnSuggestionChanged();
			}
		}
		protected override string CalcSuggestion() {
			return txtChangeTo.Text;
		}
		protected virtual void OnSuggestionChanged() {
			if (txtChangeTo.Text != Suggestion)
				txtChangeTo.Text = Suggestion;
			if (lbcSuggestions.SelectedValue != null && lbcSuggestions.SelectedValue.ToString() != Suggestion) {
				int index = lbcSuggestions.Items.IndexOf(Suggestion);
				if (index != -1)
					lbcSuggestions.SelectedIndex = index;
			}
		}
		private void lbcSuggestions_SelectedIndexChanged(object sender, System.EventArgs e) {
			ListBoxControl listBox = sender as ListBoxControl;
			if (listBox.SelectedItem != null && listBox.Enabled)
				Suggestion = listBox.SelectedItem.ToString();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			SetMinimumFormSize();
		}
		protected virtual void SetMinimumFormSize() {
			MinimumSize = new Size(477, 248);
		}
		private void SpellingOutlookStyleForm_FormClosed(object sender, FormClosedEventArgs e) {
			this.SpellingFormResult = SpellCheckOperation.Cancel;
		}
	}
}
