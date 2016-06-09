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
using DevExpress.XtraSpellChecker.Strategies;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraSpellChecker.Native;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.xtraTabControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.xtraTabPage1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.lblLanguage")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.lblChange")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.imgGeneralOpt")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.chkNumbers")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.chkUpperCase")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.chkEmails")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.imgCustomDic")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.lblChooseDic")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.imgInternationDics")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.cbLanguage")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.btnEditDictionary")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.chkWebSites")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.btnOK")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.btnApply")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.grpGeneral")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.grpEditCustomDic")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.grpInternationDocs")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.chkMixedCase")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingOptionsForm.chkRepeatedWords")]
#endregion
namespace DevExpress.XtraSpellChecker.Forms {
	public class SpellingOptionsForm : BaseSpellCheckForm {
		#region Generated Fields
		protected internal DevExpress.XtraTab.XtraTabControl xtraTabControl1;
		protected internal DevExpress.XtraTab.XtraTabPage xtraTabPage1;
		protected internal Label lblLanguage;
		protected internal Label lblChange;
		protected internal PictureEdit imgGeneralOpt;
		protected internal CheckEdit chkNumbers;
		protected internal CheckEdit chkUpperCase;
		protected internal CheckEdit chkEmails;
		protected internal PictureEdit imgCustomDic;
		protected internal Label lblChooseDic;
		protected internal PictureEdit imgInternationDics;
		protected internal ComboBoxEdit cbLanguage;
		protected internal SimpleButton btnEditDictionary;
		protected internal CheckEdit chkWebSites;
		protected internal SimpleButton btnOK;
		protected internal SimpleButton btnCancel;
		protected internal SimpleButton btnApply;
		protected internal DevExpress.XtraEditors.GroupControl grpGeneral;
		protected internal DevExpress.XtraEditors.GroupControl grpEditCustomDic;
		protected internal DevExpress.XtraEditors.GroupControl grpInternationDocs;
		protected internal DevExpress.XtraEditors.CheckEdit chkMixedCase;
		protected internal DevExpress.XtraEditors.CheckEdit chkRepeatedWords;
		#endregion
		private System.ComponentModel.Container components = null;
		public SpellingOptionsForm() {
			Initialize();
		}
		public SpellingOptionsForm(SpellChecker spellChecker)
			: base(spellChecker) {
			Initialize();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					UnsubscribeFromControlEvents();
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpellingOptionsForm));
			this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
			this.grpGeneral = new DevExpress.XtraEditors.GroupControl();
			this.chkRepeatedWords = new DevExpress.XtraEditors.CheckEdit();
			this.chkMixedCase = new DevExpress.XtraEditors.CheckEdit();
			this.chkNumbers = new DevExpress.XtraEditors.CheckEdit();
			this.chkUpperCase = new DevExpress.XtraEditors.CheckEdit();
			this.chkEmails = new DevExpress.XtraEditors.CheckEdit();
			this.imgGeneralOpt = new DevExpress.XtraEditors.PictureEdit();
			this.chkWebSites = new DevExpress.XtraEditors.CheckEdit();
			this.grpEditCustomDic = new DevExpress.XtraEditors.GroupControl();
			this.lblChange = new System.Windows.Forms.Label();
			this.btnEditDictionary = new DevExpress.XtraEditors.SimpleButton();
			this.imgCustomDic = new DevExpress.XtraEditors.PictureEdit();
			this.grpInternationDocs = new DevExpress.XtraEditors.GroupControl();
			this.cbLanguage = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblLanguage = new System.Windows.Forms.Label();
			this.imgInternationDics = new DevExpress.XtraEditors.PictureEdit();
			this.lblChooseDic = new System.Windows.Forms.Label();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
			this.xtraTabControl1.SuspendLayout();
			this.xtraTabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpGeneral)).BeginInit();
			this.grpGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkRepeatedWords.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkMixedCase.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkNumbers.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkUpperCase.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEmails.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgGeneralOpt.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkWebSites.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpEditCustomDic)).BeginInit();
			this.grpEditCustomDic.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgCustomDic.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpInternationDocs)).BeginInit();
			this.grpInternationDocs.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbLanguage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgInternationDics.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.xtraTabControl1, "xtraTabControl1");
			this.xtraTabControl1.Name = "xtraTabControl1";
			this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
			this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.xtraTabPage1});
			this.xtraTabPage1.Controls.Add(this.grpGeneral);
			this.xtraTabPage1.Controls.Add(this.grpEditCustomDic);
			this.xtraTabPage1.Controls.Add(this.grpInternationDocs);
			this.xtraTabPage1.Name = "xtraTabPage1";
			resources.ApplyResources(this.xtraTabPage1, "xtraTabPage1");
			resources.ApplyResources(this.grpGeneral, "grpGeneral");
			this.grpGeneral.Controls.Add(this.chkRepeatedWords);
			this.grpGeneral.Controls.Add(this.chkMixedCase);
			this.grpGeneral.Controls.Add(this.chkNumbers);
			this.grpGeneral.Controls.Add(this.chkUpperCase);
			this.grpGeneral.Controls.Add(this.chkEmails);
			this.grpGeneral.Controls.Add(this.imgGeneralOpt);
			this.grpGeneral.Controls.Add(this.chkWebSites);
			this.grpGeneral.Name = "grpGeneral";
			resources.ApplyResources(this.chkRepeatedWords, "chkRepeatedWords");
			this.chkRepeatedWords.Name = "chkRepeatedWords";
			this.chkRepeatedWords.Properties.AutoWidth = true;
			this.chkRepeatedWords.Properties.Caption = resources.GetString("chkRepeatedWords.Properties.Caption");
			resources.ApplyResources(this.chkMixedCase, "chkMixedCase");
			this.chkMixedCase.Name = "chkMixedCase";
			this.chkMixedCase.Properties.AutoWidth = true;
			this.chkMixedCase.Properties.Caption = resources.GetString("chkMixedCase.Properties.Caption");
			resources.ApplyResources(this.chkNumbers, "chkNumbers");
			this.chkNumbers.Name = "chkNumbers";
			this.chkNumbers.Properties.AutoWidth = true;
			this.chkNumbers.Properties.Caption = resources.GetString("chkNumbers.Properties.Caption");
			resources.ApplyResources(this.chkUpperCase, "chkUpperCase");
			this.chkUpperCase.Name = "chkUpperCase";
			this.chkUpperCase.Properties.AutoWidth = true;
			this.chkUpperCase.Properties.Caption = resources.GetString("chkUpperCase.Properties.Caption");
			resources.ApplyResources(this.chkEmails, "chkEmails");
			this.chkEmails.Name = "chkEmails";
			this.chkEmails.Properties.AutoWidth = true;
			this.chkEmails.Properties.Caption = resources.GetString("chkEmails.Properties.Caption");
			resources.ApplyResources(this.imgGeneralOpt, "imgGeneralOpt");
			this.imgGeneralOpt.Name = "imgGeneralOpt";
			this.imgGeneralOpt.Properties.AllowFocused = false;
			this.imgGeneralOpt.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("imgGeneralOpt.Properties.Appearance.BackColor")));
			this.imgGeneralOpt.Properties.Appearance.Options.UseBackColor = true;
			this.imgGeneralOpt.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.imgGeneralOpt.Properties.ReadOnly = true;
			this.imgGeneralOpt.Properties.ShowMenu = false;
			resources.ApplyResources(this.chkWebSites, "chkWebSites");
			this.chkWebSites.Name = "chkWebSites";
			this.chkWebSites.Properties.AutoWidth = true;
			this.chkWebSites.Properties.Caption = resources.GetString("chkWebSites.Properties.Caption");
			resources.ApplyResources(this.grpEditCustomDic, "grpEditCustomDic");
			this.grpEditCustomDic.Controls.Add(this.lblChange);
			this.grpEditCustomDic.Controls.Add(this.btnEditDictionary);
			this.grpEditCustomDic.Controls.Add(this.imgCustomDic);
			this.grpEditCustomDic.Name = "grpEditCustomDic";
			resources.ApplyResources(this.lblChange, "lblChange");
			this.lblChange.Name = "lblChange";
			resources.ApplyResources(this.btnEditDictionary, "btnEditDictionary");
			this.btnEditDictionary.Name = "btnEditDictionary";
			this.btnEditDictionary.Click += new System.EventHandler(this.btnEditDictionary_Click);
			resources.ApplyResources(this.imgCustomDic, "imgCustomDic");
			this.imgCustomDic.Name = "imgCustomDic";
			this.imgCustomDic.Properties.AllowFocused = false;
			this.imgCustomDic.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("imgCustomDic.Properties.Appearance.BackColor")));
			this.imgCustomDic.Properties.Appearance.Options.UseBackColor = true;
			this.imgCustomDic.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.imgCustomDic.Properties.ReadOnly = true;
			this.imgCustomDic.Properties.ShowMenu = false;
			resources.ApplyResources(this.grpInternationDocs, "grpInternationDocs");
			this.grpInternationDocs.Controls.Add(this.cbLanguage);
			this.grpInternationDocs.Controls.Add(this.lblLanguage);
			this.grpInternationDocs.Controls.Add(this.imgInternationDics);
			this.grpInternationDocs.Controls.Add(this.lblChooseDic);
			this.grpInternationDocs.Name = "grpInternationDocs";
			resources.ApplyResources(this.cbLanguage, "cbLanguage");
			this.cbLanguage.Name = "cbLanguage";
			this.cbLanguage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbLanguage.Properties.Buttons"))))});
			this.cbLanguage.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lblLanguage, "lblLanguage");
			this.lblLanguage.Name = "lblLanguage";
			resources.ApplyResources(this.imgInternationDics, "imgInternationDics");
			this.imgInternationDics.Name = "imgInternationDics";
			this.imgInternationDics.Properties.AllowFocused = false;
			this.imgInternationDics.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("imgInternationDics.Properties.Appearance.BackColor")));
			this.imgInternationDics.Properties.Appearance.Options.UseBackColor = true;
			this.imgInternationDics.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.imgInternationDics.Properties.ReadOnly = true;
			this.imgInternationDics.Properties.ShowMenu = false;
			resources.ApplyResources(this.lblChooseDic, "lblChooseDic");
			this.lblChooseDic.Name = "lblChooseDic";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.xtraTabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SpellingOptionsForm";
			this.Load += new System.EventHandler(this.SpellingOptionsForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
			this.xtraTabControl1.ResumeLayout(false);
			this.xtraTabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grpGeneral)).EndInit();
			this.grpGeneral.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chkRepeatedWords.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkMixedCase.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkNumbers.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkUpperCase.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEmails.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgGeneralOpt.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkWebSites.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpEditCustomDic)).EndInit();
			this.grpEditCustomDic.ResumeLayout(false);
			this.grpEditCustomDic.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgCustomDic.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpInternationDocs)).EndInit();
			this.grpInternationDocs.ResumeLayout(false);
			this.grpInternationDocs.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbLanguage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgInternationDics.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void SubscribeToControlEvents() {
			this.cbLanguage.SelectedIndexChanged += new EventHandler(OnLanguageChanged);
			this.chkRepeatedWords.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			this.chkMixedCase.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			this.chkNumbers.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			this.chkUpperCase.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			this.chkEmails.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			this.chkWebSites.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
		}
		void UnsubscribeFromControlEvents() {
			this.cbLanguage.SelectedIndexChanged -= new EventHandler(OnLanguageChanged);
			this.chkRepeatedWords.CheckedChanged -= new System.EventHandler(this.checkBox_CheckedChanged);
			this.chkMixedCase.CheckedChanged -= new System.EventHandler(this.checkBox_CheckedChanged);
			this.chkNumbers.CheckedChanged -= new System.EventHandler(this.checkBox_CheckedChanged);
			this.chkUpperCase.CheckedChanged -= new System.EventHandler(this.checkBox_CheckedChanged);
			this.chkEmails.CheckedChanged -= new System.EventHandler(this.checkBox_CheckedChanged);
			this.chkWebSites.CheckedChanged -= new System.EventHandler(this.checkBox_CheckedChanged);
		}
		void Initialize() {
			InitializeComponent(); 
			InitializeIcon();
			PopulateLanguages();
			InitializeControls();
			SubscribeToControlEvents();
		}
		protected virtual void InitializeIcon() {
			Icon = ResourceImageHelper.CreateIconFromResources("DevExpress.XtraSpellChecker.Images.SpellChecker.ico", System.Reflection.Assembly.GetExecutingAssembly());
		}
		protected virtual void InitializeControls() {
			if (SpellChecker == null) return;
			this.chkEmails.Checked = SpellChecker.OptionsSpelling.IsIgnoreEmails();
			this.chkNumbers.Checked = SpellChecker.OptionsSpelling.IsIgnoreWordsWithNumbers();
			this.chkUpperCase.Checked = SpellChecker.OptionsSpelling.IsIgnoreUpperCaseWords();
			this.chkWebSites.Checked = SpellChecker.OptionsSpelling.IsIgnoreUri();
			this.chkMixedCase.Checked = SpellChecker.OptionsSpelling.IsIgnoreMixedCaseWords();
			this.chkRepeatedWords.Checked = SpellChecker.OptionsSpelling.IsIgnoreRepeatedWords();
			UndateEditDictionaryButton();
		}
		protected virtual SpellCheckerCustomDictionary CustomDictionary {
			get {
				DictionaryHelper helper = SpellChecker.CreateDictionaryHelper();
				return helper.GetCustomDictionary(Culture);
			}
		}
		protected virtual void OnLanguageChanged(object sender, EventArgs e) {
			UndateEditDictionaryButton();
		}
		void UndateEditDictionaryButton() {
			this.btnEditDictionary.Enabled = CustomDictionary != null;
		}
		protected virtual void PopulateLanguages() {
			if (SpellChecker == null)
				return;
			SharedDictionaryStorage sharedStorage = new SharedDictionaryStorage();
			cbLanguage.Properties.Items.BeginUpdate();
			try {
				cbLanguage.Properties.Items.Clear();
				DictionaryCollection dictionaries = new DictionaryCollection();
				dictionaries.AddRange(SpellChecker.Dictionaries);
				dictionaries.AddRange(sharedStorage.Dictionaries);
				AddLanguagesToComboBox(dictionaries);
			}
			finally {
				cbLanguage.Properties.Items.EndUpdate();
			}
			LanguageItem currentLanguage = new LanguageItem(SpellChecker.SearchStrategy.ActualCulture);
			this.cbLanguage.SelectedIndex = cbLanguage.Properties.Items.IndexOf(currentLanguage);
			if (this.cbLanguage.SelectedIndex == -1) {
				cbLanguage.Properties.Items.Insert(0, currentLanguage);
				this.cbLanguage.SelectedIndex = 0;
			}
		}
		void AddLanguagesToComboBox(DictionaryCollection dictionaryCollection) {
			List<CultureInfo> cultureInfos = GetCultures(dictionaryCollection);
			int count = cultureInfos.Count;
			ComboBoxItemCollection items = cbLanguage.Properties.Items;
			for (int i = 0; i < count; i++)
				items.Add(new LanguageItem(cultureInfos[i]));
		}
		List<CultureInfo> GetCultures(DictionaryCollection dictionaryCollection) {
			List<CultureInfo> result = new List<CultureInfo>();
			int count = dictionaryCollection.Count;
			for (int i = 0; i < count; i++) {
				CultureInfo culture = dictionaryCollection[i].Culture;
				if (!result.Contains(culture))
					result.Add(culture);
			}
			if (result.Count > 1 && !result.Contains(CultureInfo.InvariantCulture))
				result.Add(CultureInfo.InvariantCulture);
			return result;
		}
		private void checkBox_CheckedChanged(object sender, System.EventArgs e) {
			if (this.Visible)
				btnApply.Enabled = true;
		}
		private void btnApply_Click(object sender, System.EventArgs e) {
			(sender as SimpleButton).Enabled = false;
			Apply();
		}
		internal void Apply(){
			SpellChecker.OptionsSpelling.BeginUpdate();
			try {
				SpellChecker.OptionsSpelling.IgnoreUpperCaseWords = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(IgnoreUpperCase);
				SpellChecker.OptionsSpelling.IgnoreEmails = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(IgnoreEmails);
				SpellChecker.OptionsSpelling.IgnoreWordsWithNumbers = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(IgnoreWordsWithDigits);
				SpellChecker.OptionsSpelling.IgnoreUri = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(IgnoreUri);
				SpellChecker.OptionsSpelling.IgnoreMixedCaseWords = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(IgnoreMixedCaseWords);
				SpellChecker.OptionsSpelling.IgnoreRepeatedWords = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(IgnoreRepeatedWords);
			}
			finally {
				SpellChecker.OptionsSpelling.EndUpdate();
			}
			SpellChecker.Culture = Culture;
			SpellChecker.SetSpellCheckerOptions(SpellChecker.EditControl, SpellChecker.OptionsSpelling);
		}
		private void btnOK_Click(object sender, System.EventArgs e) {
			Apply();
		}
		public bool IgnoreEmails { get { return chkEmails.Checked; } }
		[Obsolete("This property has become obsolete. Use the IgnoreUri property instead.")]
		public bool IgnoreUrls { get { return chkWebSites.Checked; } }
		public bool IgnoreUri { get { return chkWebSites.Checked; } }
		public bool IgnoreUpperCase { get { return chkUpperCase.Checked; } }
		public bool IgnoreWordsWithDigits { get { return chkNumbers.Checked; } }
		public bool IgnoreMixedCaseWords { get { return chkMixedCase.Checked; } }
		public bool IgnoreRepeatedWords { get { return chkRepeatedWords.Checked; } }
		public CultureInfo Culture { get { return ((LanguageItem)cbLanguage.SelectedItem).Culture; } }
		private void btnEditDictionary_Click(object sender, EventArgs e) {
			EditCustomDictionary();
		}
		protected virtual void EditCustomDictionary() {
			SpellChecker.FormsManager.ShowEditDictionaryForm(CustomDictionary);
		}
		protected override bool ShouldSaveFormInformation() {
			return true;
		}
		private void SpellingOptionsForm_Load(object sender, EventArgs e) {
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			imgGeneralOpt.Image = DevExpress.Utils.ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraSpellChecker.Images.GeneralOptions.png", asm);
			imgCustomDic.Image = DevExpress.Utils.ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraSpellChecker.Images.EditCustomDictionary.png", asm);
			imgInternationDics.Image = DevExpress.Utils.ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraSpellChecker.Images.InternationalDictionaries.png", asm);
		}
	}
	public class LanguageItem {
		CultureInfo culture;
		public LanguageItem(CultureInfo culture) {
			this.culture = culture;
		}
		public CultureInfo Culture { get { return culture; } }
		public override string ToString() {
			return culture.DisplayName;
		}
		public override bool Equals(object obj) {
			LanguageItem item = obj as LanguageItem;
			if (item != null)
				return culture.Equals(item.Culture);
			else
				return false;
		}
		public override int GetHashCode() {
			return Culture.GetHashCode();
		}
	}
}
