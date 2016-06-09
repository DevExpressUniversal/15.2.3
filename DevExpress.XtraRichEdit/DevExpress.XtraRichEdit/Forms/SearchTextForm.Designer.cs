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

namespace DevExpress.XtraRichEdit.Forms {
	partial class SearchTextForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchTextForm));
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
			this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
			this.pgFind = new RichEditTabPage();
			this.chbFndRegex = new DevExpress.XtraEditors.CheckEdit();
			this.cbFndSearchString = new DevExpress.XtraEditors.MRUEdit();
			this.cbFndFindDirection = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblFndSearchString = new DevExpress.XtraEditors.LabelControl();
			this.chbFndMatchCase = new DevExpress.XtraEditors.CheckEdit();
			this.chbFndFindWholeWord = new DevExpress.XtraEditors.CheckEdit();
			this.lblFndDirection = new DevExpress.XtraEditors.LabelControl();
			this.pgReplace = new RichEditTabPage();
			this.chbRplRegex = new DevExpress.XtraEditors.CheckEdit();
			this.cbRplReplaceString = new DevExpress.XtraEditors.MRUEdit();
			this.cbRplSearchString = new DevExpress.XtraEditors.MRUEdit();
			this.cbRplFindDirection = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chbRplMatchCase = new DevExpress.XtraEditors.CheckEdit();
			this.chbRplFindWholeWord = new DevExpress.XtraEditors.CheckEdit();
			this.lblRplSearchString = new DevExpress.XtraEditors.LabelControl();
			this.lblRplReplaceString = new DevExpress.XtraEditors.LabelControl();
			this.lblRplDirection = new DevExpress.XtraEditors.LabelControl();
			this.btnReplaceAll = new DevExpress.XtraEditors.SimpleButton();
			this.btnFindNext = new DevExpress.XtraEditors.SimpleButton();
			this.btnReplaceNext = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
			this.xtraTabControl.SuspendLayout();
			this.pgFind.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chbFndRegex.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFndSearchString.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFndFindDirection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbFndMatchCase.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbFndFindWholeWord.Properties)).BeginInit();
			this.pgReplace.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chbRplRegex.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbRplReplaceString.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbRplSearchString.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbRplFindDirection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbRplMatchCase.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbRplFindWholeWord.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
			this.xtraTabControl.Name = "xtraTabControl";
			this.xtraTabControl.SelectedTabPage = this.pgFind;
			this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.pgFind,
			this.pgReplace});
			this.xtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.OnSelectedPageChanged);
			this.pgFind.Controls.Add(this.chbFndRegex);
			this.pgFind.Controls.Add(this.cbFndSearchString);
			this.pgFind.Controls.Add(this.cbFndFindDirection);
			this.pgFind.Controls.Add(this.lblFndSearchString);
			this.pgFind.Controls.Add(this.chbFndMatchCase);
			this.pgFind.Controls.Add(this.chbFndFindWholeWord);
			this.pgFind.Controls.Add(this.lblFndDirection);
			this.pgFind.Name = "pgFind";
			resources.ApplyResources(this.pgFind, "pgFind");
			this.chbFndRegex.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chbFndRegex, "chbFndRegex");
			this.chbFndRegex.Name = "chbFndRegex";
			this.chbFndRegex.Properties.AutoWidth = true;
			this.chbFndRegex.Properties.Caption = resources.GetString("chbFndRegex.Properties.Caption");
			this.chbFndRegex.CheckedChanged += new System.EventHandler(this.chbRegex_CheckedChanged);
			resources.ApplyResources(this.cbFndSearchString, "cbFndSearchString");
			this.cbFndSearchString.Name = "cbFndSearchString";
			this.cbFndSearchString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFndSearchString.Properties.Buttons")))),
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFndSearchString.Properties.Buttons1"))), resources.GetString("cbFndSearchString.Properties.Buttons2"), ((int)(resources.GetObject("cbFndSearchString.Properties.Buttons3"))), ((bool)(resources.GetObject("cbFndSearchString.Properties.Buttons4"))), ((bool)(resources.GetObject("cbFndSearchString.Properties.Buttons5"))), ((bool)(resources.GetObject("cbFndSearchString.Properties.Buttons6"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("cbFndSearchString.Properties.Buttons7"))), ((System.Drawing.Image)(resources.GetObject("cbFndSearchString.Properties.Buttons8"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, resources.GetString("cbFndSearchString.Properties.Buttons9"), ((object)(resources.GetObject("cbFndSearchString.Properties.Buttons10"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("cbFndSearchString.Properties.Buttons11"))), ((bool)(resources.GetObject("cbFndSearchString.Properties.Buttons12"))))});
			this.cbFndSearchString.Properties.ValidateOnEnterKey = false;
			this.cbFndSearchString.AddingMRUItem += new DevExpress.XtraEditors.Controls.AddingMRUItemEventHandler(this.SearchString_AddingMRUItem);
			this.cbFndSearchString.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbSearchString_ButtonClick);
			this.cbFndSearchString.EditValueChanged += new System.EventHandler(this.cbFndSearchString_EditValueChanged);
			resources.ApplyResources(this.cbFndFindDirection, "cbFndFindDirection");
			this.cbFndFindDirection.Name = "cbFndFindDirection";
			this.cbFndFindDirection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFndFindDirection.Properties.Buttons"))))});
			this.cbFndFindDirection.Properties.Items.AddRange(new object[] {
			resources.GetString("cbFndFindDirection.Properties.Items"),
			resources.GetString("cbFndFindDirection.Properties.Items1"),
			resources.GetString("cbFndFindDirection.Properties.Items2")});
			this.cbFndFindDirection.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbFndFindDirection.SelectedIndexChanged += new System.EventHandler(this.cbFndFindDirection_SelectedIndexChanged);
			resources.ApplyResources(this.lblFndSearchString, "lblFndSearchString");
			this.lblFndSearchString.Name = "lblFndSearchString";
			this.chbFndMatchCase.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chbFndMatchCase, "chbFndMatchCase");
			this.chbFndMatchCase.Name = "chbFndMatchCase";
			this.chbFndMatchCase.Properties.AutoWidth = true;
			this.chbFndMatchCase.Properties.Caption = resources.GetString("chbFndMatchCase.Properties.Caption");
			this.chbFndMatchCase.CheckedChanged += new System.EventHandler(this.chbFndMatchCase_CheckedChanged);
			this.chbFndFindWholeWord.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chbFndFindWholeWord, "chbFndFindWholeWord");
			this.chbFndFindWholeWord.Name = "chbFndFindWholeWord";
			this.chbFndFindWholeWord.Properties.AutoWidth = true;
			this.chbFndFindWholeWord.Properties.Caption = resources.GetString("chbFndFindWholeWord.Properties.Caption");
			this.chbFndFindWholeWord.CheckedChanged += new System.EventHandler(this.chbFndFindWholeWord_CheckedChanged);
			resources.ApplyResources(this.lblFndDirection, "lblFndDirection");
			this.lblFndDirection.Name = "lblFndDirection";
			this.pgReplace.Controls.Add(this.chbRplRegex);
			this.pgReplace.Controls.Add(this.cbRplReplaceString);
			this.pgReplace.Controls.Add(this.cbRplSearchString);
			this.pgReplace.Controls.Add(this.cbRplFindDirection);
			this.pgReplace.Controls.Add(this.chbRplMatchCase);
			this.pgReplace.Controls.Add(this.chbRplFindWholeWord);
			this.pgReplace.Controls.Add(this.lblRplSearchString);
			this.pgReplace.Controls.Add(this.lblRplReplaceString);
			this.pgReplace.Controls.Add(this.lblRplDirection);
			this.pgReplace.Name = "pgReplace";
			resources.ApplyResources(this.pgReplace, "pgReplace");
			this.chbRplRegex.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chbRplRegex, "chbRplRegex");
			this.chbRplRegex.Name = "chbRplRegex";
			this.chbRplRegex.Properties.AutoWidth = true;
			this.chbRplRegex.Properties.Caption = resources.GetString("chbRplRegex.Properties.Caption");
			this.chbRplRegex.CheckedChanged += new System.EventHandler(this.chbRplRegex_CheckedChanged);
			resources.ApplyResources(this.cbRplReplaceString, "cbRplReplaceString");
			this.cbRplReplaceString.Name = "cbRplReplaceString";
			this.cbRplReplaceString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbRplReplaceString.Properties.Buttons")))),
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbRplReplaceString.Properties.Buttons1"))), resources.GetString("cbRplReplaceString.Properties.Buttons2"), ((int)(resources.GetObject("cbRplReplaceString.Properties.Buttons3"))), ((bool)(resources.GetObject("cbRplReplaceString.Properties.Buttons4"))), ((bool)(resources.GetObject("cbRplReplaceString.Properties.Buttons5"))), ((bool)(resources.GetObject("cbRplReplaceString.Properties.Buttons6"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("cbRplReplaceString.Properties.Buttons7"))), ((System.Drawing.Image)(resources.GetObject("cbRplReplaceString.Properties.Buttons8"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, resources.GetString("cbRplReplaceString.Properties.Buttons9"), ((object)(resources.GetObject("cbRplReplaceString.Properties.Buttons10"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("cbRplReplaceString.Properties.Buttons11"))), ((bool)(resources.GetObject("cbRplReplaceString.Properties.Buttons12"))))});
			this.cbRplReplaceString.Properties.ValidateOnEnterKey = false;
			this.cbRplReplaceString.AddingMRUItem += new DevExpress.XtraEditors.Controls.AddingMRUItemEventHandler(this.ReplaceString_AddingMRUItem);
			this.cbRplReplaceString.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbRplReplaceString_ButtonClick);
			this.cbRplReplaceString.EditValueChanged += new System.EventHandler(this.cbReplaceReplaceString_EditValueChanged);
			resources.ApplyResources(this.cbRplSearchString, "cbRplSearchString");
			this.cbRplSearchString.Name = "cbRplSearchString";
			this.cbRplSearchString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbRplSearchString.Properties.Buttons")))),
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbRplSearchString.Properties.Buttons1"))), resources.GetString("cbRplSearchString.Properties.Buttons2"), ((int)(resources.GetObject("cbRplSearchString.Properties.Buttons3"))), ((bool)(resources.GetObject("cbRplSearchString.Properties.Buttons4"))), ((bool)(resources.GetObject("cbRplSearchString.Properties.Buttons5"))), ((bool)(resources.GetObject("cbRplSearchString.Properties.Buttons6"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("cbRplSearchString.Properties.Buttons7"))), ((System.Drawing.Image)(resources.GetObject("cbRplSearchString.Properties.Buttons8"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject3, resources.GetString("cbRplSearchString.Properties.Buttons9"), ((object)(resources.GetObject("cbRplSearchString.Properties.Buttons10"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("cbRplSearchString.Properties.Buttons11"))), ((bool)(resources.GetObject("cbRplSearchString.Properties.Buttons12"))))});
			this.cbRplSearchString.Properties.ValidateOnEnterKey = false;
			this.cbRplSearchString.AddingMRUItem += new DevExpress.XtraEditors.Controls.AddingMRUItemEventHandler(this.SearchString_AddingMRUItem);
			this.cbRplSearchString.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbSearchString_ButtonClick);
			this.cbRplSearchString.EditValueChanged += new System.EventHandler(this.cbRplSearchString_EditValueChanged);
			resources.ApplyResources(this.cbRplFindDirection, "cbRplFindDirection");
			this.cbRplFindDirection.Name = "cbRplFindDirection";
			this.cbRplFindDirection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbRplFindDirection.Properties.Buttons"))))});
			this.cbRplFindDirection.Properties.Items.AddRange(new object[] {
			resources.GetString("cbRplFindDirection.Properties.Items"),
			resources.GetString("cbRplFindDirection.Properties.Items1"),
			resources.GetString("cbRplFindDirection.Properties.Items2")});
			this.cbRplFindDirection.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbRplFindDirection.SelectedIndexChanged += new System.EventHandler(this.cbRplFindDirection_SelectedIndexChanged);
			this.chbRplMatchCase.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chbRplMatchCase, "chbRplMatchCase");
			this.chbRplMatchCase.Name = "chbRplMatchCase";
			this.chbRplMatchCase.Properties.AutoWidth = true;
			this.chbRplMatchCase.Properties.Caption = resources.GetString("chbRplMatchCase.Properties.Caption");
			this.chbRplMatchCase.CheckedChanged += new System.EventHandler(this.chbRplMatchCase_CheckedChanged);
			this.chbRplFindWholeWord.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chbRplFindWholeWord, "chbRplFindWholeWord");
			this.chbRplFindWholeWord.Name = "chbRplFindWholeWord";
			this.chbRplFindWholeWord.Properties.AutoWidth = true;
			this.chbRplFindWholeWord.Properties.Caption = resources.GetString("chbRplFindWholeWord.Properties.Caption");
			this.chbRplFindWholeWord.CheckedChanged += new System.EventHandler(this.chbRplFindWholeWord_CheckedChanged);
			resources.ApplyResources(this.lblRplSearchString, "lblRplSearchString");
			this.lblRplSearchString.Name = "lblRplSearchString";
			resources.ApplyResources(this.lblRplReplaceString, "lblRplReplaceString");
			this.lblRplReplaceString.Name = "lblRplReplaceString";
			resources.ApplyResources(this.lblRplDirection, "lblRplDirection");
			this.lblRplDirection.Name = "lblRplDirection";
			resources.ApplyResources(this.btnReplaceAll, "btnReplaceAll");
			this.btnReplaceAll.Name = "btnReplaceAll";
			this.btnReplaceAll.Click += new System.EventHandler(this.OnReplaceAllButtonClick);
			resources.ApplyResources(this.btnFindNext, "btnFindNext");
			this.btnFindNext.Name = "btnFindNext";
			this.btnFindNext.Click += new System.EventHandler(this.OnFindNextButtonClick);
			resources.ApplyResources(this.btnReplaceNext, "btnReplaceNext");
			this.btnReplaceNext.Name = "btnReplaceNext";
			this.btnReplaceNext.Click += new System.EventHandler(this.OnReplaceNextButtonClick);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.CancelButton_Click);
			this.AcceptButton = this.btnFindNext;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.xtraTabControl);
			this.Controls.Add(this.btnReplaceNext);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnReplaceAll);
			this.Controls.Add(this.btnFindNext);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SearchTextForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SearchTextForm_Closed);
			this.Load += new System.EventHandler(this.SearchTextForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
			this.xtraTabControl.ResumeLayout(false);
			this.pgFind.ResumeLayout(false);
			this.pgFind.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chbFndRegex.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFndSearchString.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFndFindDirection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbFndMatchCase.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbFndFindWholeWord.Properties)).EndInit();
			this.pgReplace.ResumeLayout(false);
			this.pgReplace.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chbRplRegex.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbRplReplaceString.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbRplSearchString.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbRplFindDirection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbRplMatchCase.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbRplFindWholeWord.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraTab.XtraTabControl xtraTabControl;
		protected DevExpress.XtraEditors.MRUEdit cbFndSearchString;
		protected DevExpress.XtraTab.XtraTabPage pgFind;
		protected DevExpress.XtraTab.XtraTabPage pgReplace;
		protected DevExpress.XtraEditors.CheckEdit chbFndMatchCase;
		protected DevExpress.XtraEditors.CheckEdit chbFndFindWholeWord;
		protected DevExpress.XtraEditors.ComboBoxEdit cbFndFindDirection;
		protected DevExpress.XtraEditors.MRUEdit cbRplReplaceString;
		protected DevExpress.XtraEditors.MRUEdit cbRplSearchString;
		protected DevExpress.XtraEditors.CheckEdit chbRplMatchCase;
		protected DevExpress.XtraEditors.ComboBoxEdit cbRplFindDirection;
		protected DevExpress.XtraEditors.CheckEdit chbRplFindWholeWord;
		protected DevExpress.XtraEditors.SimpleButton btnReplaceAll;
		protected DevExpress.XtraEditors.SimpleButton btnFindNext;
		protected DevExpress.XtraEditors.SimpleButton btnReplaceNext;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.LabelControl lblFndDirection;
		protected DevExpress.XtraEditors.LabelControl lblRplSearchString;
		protected DevExpress.XtraEditors.LabelControl lblRplReplaceString;
		protected DevExpress.XtraEditors.LabelControl lblRplDirection;
		protected DevExpress.XtraEditors.LabelControl lblFndSearchString;
		protected DevExpress.XtraEditors.CheckEdit chbFndRegex;
		protected DevExpress.XtraEditors.CheckEdit chbRplRegex;
	}
}
