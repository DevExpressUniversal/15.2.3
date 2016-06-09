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
	partial class PageSetupForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageSetupForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tabPageMargins = new DevExpress.XtraTab.XtraTabPage();
			this.cbMarginsApplyTo = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblMarginsApplyTo = new DevExpress.XtraEditors.LabelControl();
			this.chkLandscape = new DevExpress.XtraEditors.CheckEdit();
			this.chkPortrait = new DevExpress.XtraEditors.CheckEdit();
			this.lblOrientation = new DevExpress.XtraEditors.LabelControl();
			this.edtMarginRight = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblMarginRIght = new DevExpress.XtraEditors.LabelControl();
			this.edtMarginBottom = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblMarginBottom = new DevExpress.XtraEditors.LabelControl();
			this.edtMarginLeft = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.edtMarginTop = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblMarginLeft = new DevExpress.XtraEditors.LabelControl();
			this.lblMarginTop = new DevExpress.XtraEditors.LabelControl();
			this.lblMargins = new DevExpress.XtraEditors.LabelControl();
			this.tabPagePaper = new DevExpress.XtraTab.XtraTabPage();
			this.cbPaperApplyTo = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblPaperApplyTo = new DevExpress.XtraEditors.LabelControl();
			this.cbPaperSize = new DevExpress.XtraEditors.ComboBoxEdit();
			this.edtPaperHeight = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.edtPaperWidth = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblPaperHeight = new DevExpress.XtraEditors.LabelControl();
			this.lblPaperWidth = new DevExpress.XtraEditors.LabelControl();
			this.lblPaperSize = new DevExpress.XtraEditors.LabelControl();
			this.tabPageLayout = new DevExpress.XtraTab.XtraTabPage();
			this.cbLayoutApplyTo = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblLayoutApplyTo = new DevExpress.XtraEditors.LabelControl();
			this.chkDifferentFirstPage = new DevExpress.XtraEditors.CheckEdit();
			this.chkDifferentOddAndEvenPage = new DevExpress.XtraEditors.CheckEdit();
			this.lblHeadersAndFooters = new DevExpress.XtraEditors.LabelControl();
			this.lblSectionStart = new DevExpress.XtraEditors.LabelControl();
			this.cbSectionStart = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblSection = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.tabPageMargins.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbMarginsApplyTo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLandscape.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPortrait.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtMarginRight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtMarginBottom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtMarginLeft.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtMarginTop.Properties)).BeginInit();
			this.tabPagePaper.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperApplyTo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPaperHeight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPaperWidth.Properties)).BeginInit();
			this.tabPageLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbLayoutApplyTo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDifferentFirstPage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDifferentOddAndEvenPage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSectionStart.Properties)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnBtnOkClick);
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.tabPageMargins;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tabPageMargins,
			this.tabPagePaper,
			this.tabPageLayout});
			this.tabPageMargins.Controls.Add(this.cbMarginsApplyTo);
			this.tabPageMargins.Controls.Add(this.lblMarginsApplyTo);
			this.tabPageMargins.Controls.Add(this.chkLandscape);
			this.tabPageMargins.Controls.Add(this.chkPortrait);
			this.tabPageMargins.Controls.Add(this.lblOrientation);
			this.tabPageMargins.Controls.Add(this.edtMarginRight);
			this.tabPageMargins.Controls.Add(this.lblMarginRIght);
			this.tabPageMargins.Controls.Add(this.edtMarginBottom);
			this.tabPageMargins.Controls.Add(this.lblMarginBottom);
			this.tabPageMargins.Controls.Add(this.edtMarginLeft);
			this.tabPageMargins.Controls.Add(this.edtMarginTop);
			this.tabPageMargins.Controls.Add(this.lblMarginLeft);
			this.tabPageMargins.Controls.Add(this.lblMarginTop);
			this.tabPageMargins.Controls.Add(this.lblMargins);
			this.tabPageMargins.Name = "tabPageMargins";
			resources.ApplyResources(this.tabPageMargins, "tabPageMargins");
			resources.ApplyResources(this.cbMarginsApplyTo, "cbMarginsApplyTo");
			this.cbMarginsApplyTo.Name = "cbMarginsApplyTo";
			this.cbMarginsApplyTo.Properties.AccessibleName = resources.GetString("cbMarginsApplyTo.Properties.AccessibleName");
			this.cbMarginsApplyTo.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbMarginsApplyTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbMarginsApplyTo.Properties.Buttons"))))});
			this.cbMarginsApplyTo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.lblMarginsApplyTo.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblMarginsApplyTo, "lblMarginsApplyTo");
			this.lblMarginsApplyTo.Name = "lblMarginsApplyTo";
			resources.ApplyResources(this.chkLandscape, "chkLandscape");
			this.chkLandscape.Name = "chkLandscape";
			this.chkLandscape.Properties.AccessibleName = resources.GetString("chkLandscape.Properties.AccessibleName");
			this.chkLandscape.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkLandscape.Properties.AutoWidth = true;
			this.chkLandscape.Properties.Caption = resources.GetString("chkLandscape.Properties.Caption");
			this.chkLandscape.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkLandscape.Properties.RadioGroupIndex = 1;
			this.chkLandscape.TabStop = false;
			resources.ApplyResources(this.chkPortrait, "chkPortrait");
			this.chkPortrait.Name = "chkPortrait";
			this.chkPortrait.Properties.AccessibleName = resources.GetString("chkPortrait.Properties.AccessibleName");
			this.chkPortrait.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkPortrait.Properties.AutoWidth = true;
			this.chkPortrait.Properties.Caption = resources.GetString("chkPortrait.Properties.Caption");
			this.chkPortrait.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkPortrait.Properties.RadioGroupIndex = 1;
			this.chkPortrait.TabStop = false;
			this.lblOrientation.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblOrientation, "lblOrientation");
			this.lblOrientation.LineVisible = true;
			this.lblOrientation.Name = "lblOrientation";
			resources.ApplyResources(this.edtMarginRight, "edtMarginRight");
			this.edtMarginRight.Name = "edtMarginRight";
			this.edtMarginRight.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.edtMarginRight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtMarginRight.Properties.IsValueInPercent = false;
			this.lblMarginRIght.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblMarginRIght, "lblMarginRIght");
			this.lblMarginRIght.Name = "lblMarginRIght";
			resources.ApplyResources(this.edtMarginBottom, "edtMarginBottom");
			this.edtMarginBottom.Name = "edtMarginBottom";
			this.edtMarginBottom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.edtMarginBottom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtMarginBottom.Properties.IsValueInPercent = false;
			this.lblMarginBottom.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblMarginBottom, "lblMarginBottom");
			this.lblMarginBottom.Name = "lblMarginBottom";
			resources.ApplyResources(this.edtMarginLeft, "edtMarginLeft");
			this.edtMarginLeft.Name = "edtMarginLeft";
			this.edtMarginLeft.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.edtMarginLeft.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtMarginLeft.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.edtMarginTop, "edtMarginTop");
			this.edtMarginTop.Name = "edtMarginTop";
			this.edtMarginTop.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.edtMarginTop.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtMarginTop.Properties.IsValueInPercent = false;
			this.lblMarginLeft.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblMarginLeft, "lblMarginLeft");
			this.lblMarginLeft.Name = "lblMarginLeft";
			this.lblMarginTop.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblMarginTop, "lblMarginTop");
			this.lblMarginTop.Name = "lblMarginTop";
			this.lblMargins.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblMargins, "lblMargins");
			this.lblMargins.LineVisible = true;
			this.lblMargins.Name = "lblMargins";
			this.tabPagePaper.Controls.Add(this.cbPaperApplyTo);
			this.tabPagePaper.Controls.Add(this.lblPaperApplyTo);
			this.tabPagePaper.Controls.Add(this.cbPaperSize);
			this.tabPagePaper.Controls.Add(this.edtPaperHeight);
			this.tabPagePaper.Controls.Add(this.edtPaperWidth);
			this.tabPagePaper.Controls.Add(this.lblPaperHeight);
			this.tabPagePaper.Controls.Add(this.lblPaperWidth);
			this.tabPagePaper.Controls.Add(this.lblPaperSize);
			this.tabPagePaper.Name = "tabPagePaper";
			resources.ApplyResources(this.tabPagePaper, "tabPagePaper");
			resources.ApplyResources(this.cbPaperApplyTo, "cbPaperApplyTo");
			this.cbPaperApplyTo.Name = "cbPaperApplyTo";
			this.cbPaperApplyTo.Properties.AccessibleName = resources.GetString("cbPaperApplyTo.Properties.AccessibleName");
			this.cbPaperApplyTo.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbPaperApplyTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPaperApplyTo.Properties.Buttons"))))});
			this.cbPaperApplyTo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.lblPaperApplyTo.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblPaperApplyTo, "lblPaperApplyTo");
			this.lblPaperApplyTo.Name = "lblPaperApplyTo";
			resources.ApplyResources(this.cbPaperSize, "cbPaperSize");
			this.cbPaperSize.Name = "cbPaperSize";
			this.cbPaperSize.Properties.AccessibleName = resources.GetString("cbPaperSize.Properties.AccessibleName");
			this.cbPaperSize.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbPaperSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPaperSize.Properties.Buttons"))))});
			this.cbPaperSize.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.edtPaperHeight, "edtPaperHeight");
			this.edtPaperHeight.Name = "edtPaperHeight";
			this.edtPaperHeight.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.edtPaperHeight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtPaperHeight.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.edtPaperWidth, "edtPaperWidth");
			this.edtPaperWidth.Name = "edtPaperWidth";
			this.edtPaperWidth.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.edtPaperWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtPaperWidth.Properties.IsValueInPercent = false;
			this.lblPaperHeight.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblPaperHeight, "lblPaperHeight");
			this.lblPaperHeight.Name = "lblPaperHeight";
			this.lblPaperWidth.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblPaperWidth, "lblPaperWidth");
			this.lblPaperWidth.Name = "lblPaperWidth";
			this.lblPaperSize.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblPaperSize, "lblPaperSize");
			this.lblPaperSize.LineVisible = true;
			this.lblPaperSize.Name = "lblPaperSize";
			this.tabPageLayout.Controls.Add(this.cbLayoutApplyTo);
			this.tabPageLayout.Controls.Add(this.lblLayoutApplyTo);
			this.tabPageLayout.Controls.Add(this.chkDifferentFirstPage);
			this.tabPageLayout.Controls.Add(this.chkDifferentOddAndEvenPage);
			this.tabPageLayout.Controls.Add(this.lblHeadersAndFooters);
			this.tabPageLayout.Controls.Add(this.lblSectionStart);
			this.tabPageLayout.Controls.Add(this.cbSectionStart);
			this.tabPageLayout.Controls.Add(this.lblSection);
			this.tabPageLayout.Name = "tabPageLayout";
			resources.ApplyResources(this.tabPageLayout, "tabPageLayout");
			resources.ApplyResources(this.cbLayoutApplyTo, "cbLayoutApplyTo");
			this.cbLayoutApplyTo.Name = "cbLayoutApplyTo";
			this.cbLayoutApplyTo.Properties.AccessibleName = resources.GetString("cbLayoutApplyTo.Properties.AccessibleName");
			this.cbLayoutApplyTo.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbLayoutApplyTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbLayoutApplyTo.Properties.Buttons"))))});
			this.cbLayoutApplyTo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.lblLayoutApplyTo.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblLayoutApplyTo, "lblLayoutApplyTo");
			this.lblLayoutApplyTo.Name = "lblLayoutApplyTo";
			resources.ApplyResources(this.chkDifferentFirstPage, "chkDifferentFirstPage");
			this.chkDifferentFirstPage.Name = "chkDifferentFirstPage";
			this.chkDifferentFirstPage.Properties.AccessibleName = resources.GetString("chkDifferentFirstPage.Properties.AccessibleName");
			this.chkDifferentFirstPage.Properties.AutoWidth = true;
			this.chkDifferentFirstPage.Properties.Caption = resources.GetString("chkDifferentFirstPage.Properties.Caption");
			resources.ApplyResources(this.chkDifferentOddAndEvenPage, "chkDifferentOddAndEvenPage");
			this.chkDifferentOddAndEvenPage.Name = "chkDifferentOddAndEvenPage";
			this.chkDifferentOddAndEvenPage.Properties.AccessibleName = resources.GetString("chkDifferentOddAndEvenPage.Properties.AccessibleName");
			this.chkDifferentOddAndEvenPage.Properties.AutoWidth = true;
			this.chkDifferentOddAndEvenPage.Properties.Caption = resources.GetString("chkDifferentOddAndEvenPage.Properties.Caption");
			this.lblHeadersAndFooters.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblHeadersAndFooters, "lblHeadersAndFooters");
			this.lblHeadersAndFooters.LineVisible = true;
			this.lblHeadersAndFooters.Name = "lblHeadersAndFooters";
			this.lblSectionStart.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblSectionStart, "lblSectionStart");
			this.lblSectionStart.Name = "lblSectionStart";
			resources.ApplyResources(this.cbSectionStart, "cbSectionStart");
			this.cbSectionStart.Name = "cbSectionStart";
			this.cbSectionStart.Properties.AccessibleName = resources.GetString("cbSectionStart.Properties.AccessibleName");
			this.cbSectionStart.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbSectionStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbSectionStart.Properties.Buttons"))))});
			this.cbSectionStart.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.lblSection.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblSection, "lblSection");
			this.lblSection.LineVisible = true;
			this.lblSection.Name = "lblSection";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PageSetupForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.tabPageMargins.ResumeLayout(false);
			this.tabPageMargins.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbMarginsApplyTo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLandscape.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPortrait.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtMarginRight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtMarginBottom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtMarginLeft.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtMarginTop.Properties)).EndInit();
			this.tabPagePaper.ResumeLayout(false);
			this.tabPagePaper.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperApplyTo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPaperHeight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPaperWidth.Properties)).EndInit();
			this.tabPageLayout.ResumeLayout(false);
			this.tabPageLayout.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbLayoutApplyTo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDifferentFirstPage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDifferentOddAndEvenPage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSectionStart.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraTab.XtraTabControl tabControl;
		protected DevExpress.XtraTab.XtraTabPage tabPageMargins;
		protected DevExpress.XtraTab.XtraTabPage tabPagePaper;
		protected DevExpress.XtraTab.XtraTabPage tabPageLayout;
		protected DevExpress.XtraEditors.LabelControl lblMarginLeft;
		protected DevExpress.XtraEditors.LabelControl lblMarginTop;
		protected DevExpress.XtraEditors.LabelControl lblMargins;
		protected DevExpress.XtraEditors.LabelControl lblOrientation;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtMarginRight;
		protected DevExpress.XtraEditors.LabelControl lblMarginRIght;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtMarginBottom;
		protected DevExpress.XtraEditors.LabelControl lblMarginBottom;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtMarginLeft;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtMarginTop;
		protected DevExpress.XtraEditors.CheckEdit chkLandscape;
		protected DevExpress.XtraEditors.CheckEdit chkPortrait;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtPaperHeight;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtPaperWidth;
		protected DevExpress.XtraEditors.LabelControl lblPaperHeight;
		protected DevExpress.XtraEditors.LabelControl lblPaperWidth;
		protected DevExpress.XtraEditors.LabelControl lblPaperSize;
		protected DevExpress.XtraEditors.ComboBoxEdit cbPaperSize;
		protected DevExpress.XtraEditors.LabelControl lblHeadersAndFooters;
		protected DevExpress.XtraEditors.LabelControl lblSectionStart;
		protected DevExpress.XtraEditors.ComboBoxEdit cbSectionStart;
		protected DevExpress.XtraEditors.LabelControl lblSection;
		protected DevExpress.XtraEditors.CheckEdit chkDifferentFirstPage;
		protected DevExpress.XtraEditors.CheckEdit chkDifferentOddAndEvenPage;
		protected DevExpress.XtraEditors.LabelControl lblMarginsApplyTo;
		protected DevExpress.XtraEditors.LabelControl lblPaperApplyTo;
		protected DevExpress.XtraEditors.ComboBoxEdit cbLayoutApplyTo;
		protected DevExpress.XtraEditors.LabelControl lblLayoutApplyTo;
		protected DevExpress.XtraEditors.ComboBoxEdit cbMarginsApplyTo;
		protected DevExpress.XtraEditors.ComboBoxEdit cbPaperApplyTo;
	}
}
