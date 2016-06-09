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

namespace DevExpress.XtraSpreadsheet.Forms {
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
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabPage = new DevExpress.XtraTab.XtraTabPage();
			this.lblTall = new DevExpress.XtraEditors.LabelControl();
			this.edtFitToHeight = new DevExpress.XtraEditors.SpinEdit();
			this.lblPagesWideBy = new DevExpress.XtraEditors.LabelControl();
			this.edtFitToWidth = new DevExpress.XtraEditors.SpinEdit();
			this.rgrpFitToPage = new DevExpress.XtraEditors.RadioGroup();
			this.lblNormalSize = new DevExpress.XtraEditors.LabelControl();
			this.edtScale = new DevExpress.XtraEditors.SpinEdit();
			this.lblScaling = new DevExpress.XtraEditors.LabelControl();
			this.edtPrintQuality = new DevExpress.XtraEditors.LookUpEdit();
			this.edtPaperSize = new DevExpress.XtraEditors.LookUpEdit();
			this.rgrpOrientation = new DevExpress.XtraEditors.RadioGroup();
			this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
			this.btnPrintPreview = new DevExpress.XtraEditors.SimpleButton();
			this.edtFirstPageNumber = new DevExpress.XtraEditors.TextEdit();
			this.lblFirstPageNumber = new DevExpress.XtraEditors.LabelControl();
			this.lblPrintQuality = new DevExpress.XtraEditors.LabelControl();
			this.lblPaperSize = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.lblOrientation = new DevExpress.XtraEditors.LabelControl();
			this.xtraTabMargins = new DevExpress.XtraTab.XtraTabPage();
			this.pageMarginsPreviewControl = new DevExpress.XtraSpreadsheet.Forms.Design.PageMarginsPreviewControl();
			this.xtraTabHeaderFooter = new DevExpress.XtraTab.XtraTabPage();
			this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
			this.headerPreview = new DevExpress.XtraSpreadsheet.Forms.Design.HeaderFooterPreviewControl();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.footerPreview = new DevExpress.XtraSpreadsheet.Forms.Design.HeaderFooterPreviewControl();
			this.edtFooter = new DevExpress.XtraEditors.LookUpEdit();
			this.edtHeader = new DevExpress.XtraEditors.LookUpEdit();
			this.chkAlignWithPageMargins = new DevExpress.XtraEditors.CheckEdit();
			this.chkScaleWithDocument = new DevExpress.XtraEditors.CheckEdit();
			this.chkDifferentFirstPage = new DevExpress.XtraEditors.CheckEdit();
			this.chkDifferentOddAndEvenPages = new DevExpress.XtraEditors.CheckEdit();
			this.lblFooter = new DevExpress.XtraEditors.LabelControl();
			this.btnCustomHeaderFooter = new DevExpress.XtraEditors.SimpleButton();
			this.lblHeader = new DevExpress.XtraEditors.LabelControl();
			this.xtraTabSheet = new DevExpress.XtraTab.XtraTabPage();
			this.rgrpPageOrder = new DevExpress.XtraEditors.RadioGroup();
			this.edtCellErrorsAs = new DevExpress.XtraEditors.LookUpEdit();
			this.edtComments = new DevExpress.XtraEditors.LookUpEdit();
			this.lblPageOrder = new DevExpress.XtraEditors.LabelControl();
			this.lblCellErrorsAs = new DevExpress.XtraEditors.LabelControl();
			this.lblComments = new DevExpress.XtraEditors.LabelControl();
			this.chkRowAndColumnHeadings = new DevExpress.XtraEditors.CheckEdit();
			this.chkDraftQuality = new DevExpress.XtraEditors.CheckEdit();
			this.chkGridlines = new DevExpress.XtraEditors.CheckEdit();
			this.lblPrint = new DevExpress.XtraEditors.LabelControl();
			this.edtPrintArea = new DevExpress.XtraEditors.TextEdit();
			this.lblPrintArea = new DevExpress.XtraEditors.LabelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.xtraTabPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtFitToHeight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFitToWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpFitToPage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtScale.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPrintQuality.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPaperSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpOrientation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFirstPageNumber.Properties)).BeginInit();
			this.xtraTabMargins.SuspendLayout();
			this.xtraTabHeaderFooter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtHeader.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAlignWithPageMargins.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkScaleWithDocument.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDifferentFirstPage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDifferentOddAndEvenPages.Properties)).BeginInit();
			this.xtraTabSheet.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rgrpPageOrder.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtCellErrorsAs.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtComments.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkRowAndColumnHeadings.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDraftQuality.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkGridlines.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPrintArea.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.xtraTabPage;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.xtraTabPage,
			this.xtraTabMargins,
			this.xtraTabHeaderFooter,
			this.xtraTabSheet});
			this.xtraTabPage.Controls.Add(this.lblTall);
			this.xtraTabPage.Controls.Add(this.edtFitToHeight);
			this.xtraTabPage.Controls.Add(this.lblPagesWideBy);
			this.xtraTabPage.Controls.Add(this.edtFitToWidth);
			this.xtraTabPage.Controls.Add(this.rgrpFitToPage);
			this.xtraTabPage.Controls.Add(this.lblNormalSize);
			this.xtraTabPage.Controls.Add(this.edtScale);
			this.xtraTabPage.Controls.Add(this.lblScaling);
			this.xtraTabPage.Controls.Add(this.edtPrintQuality);
			this.xtraTabPage.Controls.Add(this.edtPaperSize);
			this.xtraTabPage.Controls.Add(this.rgrpOrientation);
			this.xtraTabPage.Controls.Add(this.btnPrint);
			this.xtraTabPage.Controls.Add(this.btnPrintPreview);
			this.xtraTabPage.Controls.Add(this.edtFirstPageNumber);
			this.xtraTabPage.Controls.Add(this.lblFirstPageNumber);
			this.xtraTabPage.Controls.Add(this.lblPrintQuality);
			this.xtraTabPage.Controls.Add(this.lblPaperSize);
			this.xtraTabPage.Controls.Add(this.labelControl1);
			this.xtraTabPage.Controls.Add(this.lblOrientation);
			this.xtraTabPage.Name = "xtraTabPage";
			resources.ApplyResources(this.xtraTabPage, "xtraTabPage");
			resources.ApplyResources(this.lblTall, "lblTall");
			this.lblTall.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblTall.Name = "lblTall";
			resources.ApplyResources(this.edtFitToHeight, "edtFitToHeight");
			this.edtFitToHeight.Name = "edtFitToHeight";
			this.edtFitToHeight.Properties.AccessibleName = resources.GetString("edtFitToHeight.Properties.AccessibleName");
			this.edtFitToHeight.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.SpinButton;
			this.edtFitToHeight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFitToHeight.Properties.Buttons"))))});
			this.edtFitToHeight.Properties.Increment = new decimal(new int[] {
			5,
			0,
			0,
			0});
			this.edtFitToHeight.Properties.Mask.EditMask = resources.GetString("edtFitToHeight.Properties.Mask.EditMask");
			resources.ApplyResources(this.lblPagesWideBy, "lblPagesWideBy");
			this.lblPagesWideBy.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPagesWideBy.Name = "lblPagesWideBy";
			resources.ApplyResources(this.edtFitToWidth, "edtFitToWidth");
			this.edtFitToWidth.Name = "edtFitToWidth";
			this.edtFitToWidth.Properties.AccessibleName = resources.GetString("edtFitToWidth.Properties.AccessibleName");
			this.edtFitToWidth.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.SpinButton;
			this.edtFitToWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFitToWidth.Properties.Buttons"))))});
			this.edtFitToWidth.Properties.Increment = new decimal(new int[] {
			5,
			0,
			0,
			0});
			this.edtFitToWidth.Properties.Mask.EditMask = resources.GetString("edtFitToWidth.Properties.Mask.EditMask");
			resources.ApplyResources(this.rgrpFitToPage, "rgrpFitToPage");
			this.rgrpFitToPage.Name = "rgrpFitToPage";
			this.rgrpFitToPage.Properties.AccessibleName = resources.GetString("rgrpFitToPage.Properties.AccessibleName");
			this.rgrpFitToPage.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.rgrpFitToPage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgrpFitToPage.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgrpFitToPage.Properties.Items"))), resources.GetString("rgrpFitToPage.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgrpFitToPage.Properties.Items2"))), resources.GetString("rgrpFitToPage.Properties.Items3"))});
			resources.ApplyResources(this.lblNormalSize, "lblNormalSize");
			this.lblNormalSize.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblNormalSize.Name = "lblNormalSize";
			resources.ApplyResources(this.edtScale, "edtScale");
			this.edtScale.Name = "edtScale";
			this.edtScale.Properties.AccessibleName = resources.GetString("edtScale.Properties.AccessibleName");
			this.edtScale.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.SpinButton;
			this.edtScale.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtScale.Properties.Buttons"))))});
			this.edtScale.Properties.Increment = new decimal(new int[] {
			5,
			0,
			0,
			0});
			this.edtScale.Properties.Mask.EditMask = resources.GetString("edtScale.Properties.Mask.EditMask");
			resources.ApplyResources(this.lblScaling, "lblScaling");
			this.lblScaling.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblScaling.LineVisible = true;
			this.lblScaling.Name = "lblScaling";
			resources.ApplyResources(this.edtPrintQuality, "edtPrintQuality");
			this.edtPrintQuality.Name = "edtPrintQuality";
			this.edtPrintQuality.Properties.AccessibleName = resources.GetString("edtPrintQuality.Properties.AccessibleName");
			this.edtPrintQuality.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtPrintQuality.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtPrintQuality.Properties.Buttons"))))});
			this.edtPrintQuality.Properties.NullText = resources.GetString("edtPrintQuality.Properties.NullText");
			this.edtPrintQuality.Properties.ShowFooter = false;
			this.edtPrintQuality.Properties.ShowHeader = false;
			resources.ApplyResources(this.edtPaperSize, "edtPaperSize");
			this.edtPaperSize.Name = "edtPaperSize";
			this.edtPaperSize.Properties.AccessibleName = resources.GetString("edtPaperSize.Properties.AccessibleName");
			this.edtPaperSize.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtPaperSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtPaperSize.Properties.Buttons"))))});
			this.edtPaperSize.Properties.NullText = resources.GetString("edtPaperSize.Properties.NullText");
			this.edtPaperSize.Properties.ShowFooter = false;
			this.edtPaperSize.Properties.ShowHeader = false;
			resources.ApplyResources(this.rgrpOrientation, "rgrpOrientation");
			this.rgrpOrientation.Name = "rgrpOrientation";
			this.rgrpOrientation.Properties.AccessibleName = resources.GetString("rgrpOrientation.Properties.AccessibleName");
			this.rgrpOrientation.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.rgrpOrientation.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgrpOrientation.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgrpOrientation.Properties.Items"))), resources.GetString("rgrpOrientation.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgrpOrientation.Properties.Items2"))), resources.GetString("rgrpOrientation.Properties.Items3"))});
			resources.ApplyResources(this.btnPrint, "btnPrint");
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			resources.ApplyResources(this.btnPrintPreview, "btnPrintPreview");
			this.btnPrintPreview.Name = "btnPrintPreview";
			this.btnPrintPreview.Click += new System.EventHandler(this.btnPrintPreview_Click);
			resources.ApplyResources(this.edtFirstPageNumber, "edtFirstPageNumber");
			this.edtFirstPageNumber.Name = "edtFirstPageNumber";
			this.edtFirstPageNumber.Properties.AccessibleName = resources.GetString("edtFirstPageNumber.Properties.AccessibleName");
			this.edtFirstPageNumber.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			resources.ApplyResources(this.lblFirstPageNumber, "lblFirstPageNumber");
			this.lblFirstPageNumber.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblFirstPageNumber.Name = "lblFirstPageNumber";
			resources.ApplyResources(this.lblPrintQuality, "lblPrintQuality");
			this.lblPrintQuality.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPrintQuality.Name = "lblPrintQuality";
			resources.ApplyResources(this.lblPaperSize, "lblPaperSize");
			this.lblPaperSize.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPaperSize.Name = "lblPaperSize";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.lblOrientation, "lblOrientation");
			this.lblOrientation.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblOrientation.LineVisible = true;
			this.lblOrientation.Name = "lblOrientation";
			this.xtraTabMargins.Controls.Add(this.pageMarginsPreviewControl);
			this.xtraTabMargins.Name = "xtraTabMargins";
			resources.ApplyResources(this.xtraTabMargins, "xtraTabMargins");
			this.pageMarginsPreviewControl.BottomMargin = 0F;
			this.pageMarginsPreviewControl.DrawPanelLandscapeOrientation = false;
			this.pageMarginsPreviewControl.DrawPanelPortraitOrientation = false;
			this.pageMarginsPreviewControl.FooterMargin = 0F;
			this.pageMarginsPreviewControl.HeaderMargin = 0F;
			this.pageMarginsPreviewControl.IsCenterHorizontally = false;
			this.pageMarginsPreviewControl.IsCenterVertically = false;
			this.pageMarginsPreviewControl.LeftMargin = 0F;
			resources.ApplyResources(this.pageMarginsPreviewControl, "pageMarginsPreviewControl");
			this.pageMarginsPreviewControl.Name = "pageMarginsPreviewControl";
			this.pageMarginsPreviewControl.RightMargin = 0F;
			this.pageMarginsPreviewControl.TopMargin = 0F;
			this.xtraTabHeaderFooter.Controls.Add(this.labelControl8);
			this.xtraTabHeaderFooter.Controls.Add(this.labelControl7);
			this.xtraTabHeaderFooter.Controls.Add(this.labelControl6);
			this.xtraTabHeaderFooter.Controls.Add(this.labelControl3);
			this.xtraTabHeaderFooter.Controls.Add(this.labelControl5);
			this.xtraTabHeaderFooter.Controls.Add(this.headerPreview);
			this.xtraTabHeaderFooter.Controls.Add(this.labelControl4);
			this.xtraTabHeaderFooter.Controls.Add(this.footerPreview);
			this.xtraTabHeaderFooter.Controls.Add(this.edtFooter);
			this.xtraTabHeaderFooter.Controls.Add(this.edtHeader);
			this.xtraTabHeaderFooter.Controls.Add(this.chkAlignWithPageMargins);
			this.xtraTabHeaderFooter.Controls.Add(this.chkScaleWithDocument);
			this.xtraTabHeaderFooter.Controls.Add(this.chkDifferentFirstPage);
			this.xtraTabHeaderFooter.Controls.Add(this.chkDifferentOddAndEvenPages);
			this.xtraTabHeaderFooter.Controls.Add(this.lblFooter);
			this.xtraTabHeaderFooter.Controls.Add(this.btnCustomHeaderFooter);
			this.xtraTabHeaderFooter.Controls.Add(this.lblHeader);
			this.xtraTabHeaderFooter.Name = "xtraTabHeaderFooter";
			resources.ApplyResources(this.xtraTabHeaderFooter, "xtraTabHeaderFooter");
			resources.ApplyResources(this.labelControl8, "labelControl8");
			this.labelControl8.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl8.LineVisible = true;
			this.labelControl8.Name = "labelControl8";
			resources.ApplyResources(this.labelControl7, "labelControl7");
			this.labelControl7.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.labelControl7.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl7.LineVisible = true;
			this.labelControl7.Name = "labelControl7";
			resources.ApplyResources(this.labelControl6, "labelControl6");
			this.labelControl6.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl6.LineVisible = true;
			this.labelControl6.Name = "labelControl6";
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl3.LineVisible = true;
			this.labelControl3.Name = "labelControl3";
			resources.ApplyResources(this.labelControl5, "labelControl5");
			this.labelControl5.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.labelControl5.LineVisible = true;
			this.labelControl5.Name = "labelControl5";
			this.headerPreview.HeaderFooterValue = null;
			resources.ApplyResources(this.headerPreview, "headerPreview");
			this.headerPreview.Name = "headerPreview";
			this.headerPreview.Provider = null;
			resources.ApplyResources(this.labelControl4, "labelControl4");
			this.labelControl4.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.labelControl4.LineVisible = true;
			this.labelControl4.Name = "labelControl4";
			this.footerPreview.HeaderFooterValue = null;
			resources.ApplyResources(this.footerPreview, "footerPreview");
			this.footerPreview.Name = "footerPreview";
			this.footerPreview.Provider = null;
			resources.ApplyResources(this.edtFooter, "edtFooter");
			this.edtFooter.Name = "edtFooter";
			this.edtFooter.Properties.AccessibleName = resources.GetString("edtFooter.Properties.AccessibleName");
			this.edtFooter.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtFooter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFooter.Properties.Buttons"))))});
			this.edtFooter.Properties.ShowFooter = false;
			this.edtFooter.Properties.ShowHeader = false;
			resources.ApplyResources(this.edtHeader, "edtHeader");
			this.edtHeader.Name = "edtHeader";
			this.edtHeader.Properties.AccessibleName = resources.GetString("edtHeader.Properties.AccessibleName");
			this.edtHeader.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtHeader.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtHeader.Properties.Buttons"))))});
			this.edtHeader.Properties.ShowFooter = false;
			this.edtHeader.Properties.ShowHeader = false;
			resources.ApplyResources(this.chkAlignWithPageMargins, "chkAlignWithPageMargins");
			this.chkAlignWithPageMargins.Name = "chkAlignWithPageMargins";
			this.chkAlignWithPageMargins.Properties.AccessibleName = resources.GetString("chkAlignWithPageMargins.Properties.AccessibleName");
			this.chkAlignWithPageMargins.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkAlignWithPageMargins.Properties.AutoWidth = true;
			this.chkAlignWithPageMargins.Properties.Caption = resources.GetString("chkAlignWithPageMargins.Properties.Caption");
			resources.ApplyResources(this.chkScaleWithDocument, "chkScaleWithDocument");
			this.chkScaleWithDocument.Name = "chkScaleWithDocument";
			this.chkScaleWithDocument.Properties.AccessibleName = resources.GetString("chkScaleWithDocument.Properties.AccessibleName");
			this.chkScaleWithDocument.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkScaleWithDocument.Properties.AutoWidth = true;
			this.chkScaleWithDocument.Properties.Caption = resources.GetString("chkScaleWithDocument.Properties.Caption");
			resources.ApplyResources(this.chkDifferentFirstPage, "chkDifferentFirstPage");
			this.chkDifferentFirstPage.Name = "chkDifferentFirstPage";
			this.chkDifferentFirstPage.Properties.AccessibleName = resources.GetString("chkDifferentFirstPage.Properties.AccessibleName");
			this.chkDifferentFirstPage.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkDifferentFirstPage.Properties.AutoWidth = true;
			this.chkDifferentFirstPage.Properties.Caption = resources.GetString("chkDifferentFirstPage.Properties.Caption");
			resources.ApplyResources(this.chkDifferentOddAndEvenPages, "chkDifferentOddAndEvenPages");
			this.chkDifferentOddAndEvenPages.Name = "chkDifferentOddAndEvenPages";
			this.chkDifferentOddAndEvenPages.Properties.AccessibleName = resources.GetString("chkDifferentOddAndEvenPages.Properties.AccessibleName");
			this.chkDifferentOddAndEvenPages.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkDifferentOddAndEvenPages.Properties.AutoWidth = true;
			this.chkDifferentOddAndEvenPages.Properties.Caption = resources.GetString("chkDifferentOddAndEvenPages.Properties.Caption");
			resources.ApplyResources(this.lblFooter, "lblFooter");
			this.lblFooter.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblFooter.Name = "lblFooter";
			resources.ApplyResources(this.btnCustomHeaderFooter, "btnCustomHeaderFooter");
			this.btnCustomHeaderFooter.Name = "btnCustomHeaderFooter";
			this.btnCustomHeaderFooter.Click += new System.EventHandler(this.btnCustomHeaderFooter_Click);
			resources.ApplyResources(this.lblHeader, "lblHeader");
			this.lblHeader.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblHeader.Name = "lblHeader";
			this.xtraTabSheet.Controls.Add(this.rgrpPageOrder);
			this.xtraTabSheet.Controls.Add(this.edtCellErrorsAs);
			this.xtraTabSheet.Controls.Add(this.edtComments);
			this.xtraTabSheet.Controls.Add(this.lblPageOrder);
			this.xtraTabSheet.Controls.Add(this.lblCellErrorsAs);
			this.xtraTabSheet.Controls.Add(this.lblComments);
			this.xtraTabSheet.Controls.Add(this.chkRowAndColumnHeadings);
			this.xtraTabSheet.Controls.Add(this.chkDraftQuality);
			this.xtraTabSheet.Controls.Add(this.chkGridlines);
			this.xtraTabSheet.Controls.Add(this.lblPrint);
			this.xtraTabSheet.Controls.Add(this.edtPrintArea);
			this.xtraTabSheet.Controls.Add(this.lblPrintArea);
			this.xtraTabSheet.Name = "xtraTabSheet";
			resources.ApplyResources(this.xtraTabSheet, "xtraTabSheet");
			resources.ApplyResources(this.rgrpPageOrder, "rgrpPageOrder");
			this.rgrpPageOrder.Name = "rgrpPageOrder";
			this.rgrpPageOrder.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgrpPageOrder.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgrpPageOrder.Properties.Items"))), resources.GetString("rgrpPageOrder.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgrpPageOrder.Properties.Items2"))), resources.GetString("rgrpPageOrder.Properties.Items3"))});
			resources.ApplyResources(this.edtCellErrorsAs, "edtCellErrorsAs");
			this.edtCellErrorsAs.Name = "edtCellErrorsAs";
			this.edtCellErrorsAs.Properties.AccessibleName = resources.GetString("edtCellErrorsAs.Properties.AccessibleName");
			this.edtCellErrorsAs.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtCellErrorsAs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtCellErrorsAs.Properties.Buttons"))))});
			this.edtCellErrorsAs.Properties.NullText = resources.GetString("edtCellErrorsAs.Properties.NullText");
			this.edtCellErrorsAs.Properties.ShowFooter = false;
			this.edtCellErrorsAs.Properties.ShowHeader = false;
			resources.ApplyResources(this.edtComments, "edtComments");
			this.edtComments.Name = "edtComments";
			this.edtComments.Properties.AccessibleName = resources.GetString("edtComments.Properties.AccessibleName");
			this.edtComments.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtComments.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtComments.Properties.Buttons"))))});
			this.edtComments.Properties.NullText = resources.GetString("edtComments.Properties.NullText");
			this.edtComments.Properties.ShowFooter = false;
			this.edtComments.Properties.ShowHeader = false;
			resources.ApplyResources(this.lblPageOrder, "lblPageOrder");
			this.lblPageOrder.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPageOrder.LineVisible = true;
			this.lblPageOrder.Name = "lblPageOrder";
			resources.ApplyResources(this.lblCellErrorsAs, "lblCellErrorsAs");
			this.lblCellErrorsAs.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblCellErrorsAs.Name = "lblCellErrorsAs";
			resources.ApplyResources(this.lblComments, "lblComments");
			this.lblComments.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblComments.Name = "lblComments";
			resources.ApplyResources(this.chkRowAndColumnHeadings, "chkRowAndColumnHeadings");
			this.chkRowAndColumnHeadings.Name = "chkRowAndColumnHeadings";
			this.chkRowAndColumnHeadings.Properties.AccessibleName = resources.GetString("chkRowAndColumnHeadings.Properties.AccessibleName");
			this.chkRowAndColumnHeadings.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkRowAndColumnHeadings.Properties.AutoWidth = true;
			this.chkRowAndColumnHeadings.Properties.Caption = resources.GetString("chkRowAndColumnHeadings.Properties.Caption");
			resources.ApplyResources(this.chkDraftQuality, "chkDraftQuality");
			this.chkDraftQuality.Name = "chkDraftQuality";
			this.chkDraftQuality.Properties.AccessibleName = resources.GetString("chkDraftQuality.Properties.AccessibleName");
			this.chkDraftQuality.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkDraftQuality.Properties.AutoWidth = true;
			this.chkDraftQuality.Properties.Caption = resources.GetString("chkDraftQuality.Properties.Caption");
			resources.ApplyResources(this.chkGridlines, "chkGridlines");
			this.chkGridlines.Name = "chkGridlines";
			this.chkGridlines.Properties.AccessibleName = resources.GetString("chkGridlines.Properties.AccessibleName");
			this.chkGridlines.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkGridlines.Properties.AutoWidth = true;
			this.chkGridlines.Properties.Caption = resources.GetString("chkGridlines.Properties.Caption");
			resources.ApplyResources(this.lblPrint, "lblPrint");
			this.lblPrint.LineVisible = true;
			this.lblPrint.Name = "lblPrint";
			resources.ApplyResources(this.edtPrintArea, "edtPrintArea");
			this.edtPrintArea.Name = "edtPrintArea";
			this.edtPrintArea.Properties.AccessibleName = resources.GetString("edtPrintArea.Properties.AccessibleName");
			this.edtPrintArea.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			resources.ApplyResources(this.lblPrintArea, "lblPrintArea");
			this.lblPrintArea.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPrintArea.Name = "lblPrintArea";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PageSetupForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.xtraTabPage.ResumeLayout(false);
			this.xtraTabPage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtFitToHeight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFitToWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpFitToPage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtScale.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPrintQuality.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPaperSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpOrientation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFirstPageNumber.Properties)).EndInit();
			this.xtraTabMargins.ResumeLayout(false);
			this.xtraTabHeaderFooter.ResumeLayout(false);
			this.xtraTabHeaderFooter.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtHeader.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAlignWithPageMargins.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkScaleWithDocument.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDifferentFirstPage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDifferentOddAndEvenPages.Properties)).EndInit();
			this.xtraTabSheet.ResumeLayout(false);
			this.xtraTabSheet.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.rgrpPageOrder.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtCellErrorsAs.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtComments.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkRowAndColumnHeadings.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDraftQuality.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkGridlines.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPrintArea.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraTab.XtraTabControl tabControl;
		private XtraTab.XtraTabPage xtraTabPage;
		private XtraTab.XtraTabPage xtraTabMargins;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraTab.XtraTabPage xtraTabHeaderFooter;
		private XtraTab.XtraTabPage xtraTabSheet;
		private XtraEditors.LabelControl lblOrientation;
		private XtraEditors.LabelControl lblPrintQuality;
		private XtraEditors.LabelControl lblPaperSize;
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.TextEdit edtFirstPageNumber;
		private XtraEditors.LabelControl lblFirstPageNumber;
		private XtraEditors.SimpleButton btnPrint;
		private XtraEditors.SimpleButton btnPrintPreview;
		private Design.PageMarginsPreviewControl pageMarginsPreviewControl;
		private XtraEditors.LabelControl lblFooter;
		private XtraEditors.SimpleButton btnCustomHeaderFooter;
		private XtraEditors.LabelControl lblHeader;
		private XtraEditors.CheckEdit chkAlignWithPageMargins;
		private XtraEditors.CheckEdit chkScaleWithDocument;
		private XtraEditors.CheckEdit chkDifferentFirstPage;
		private XtraEditors.CheckEdit chkDifferentOddAndEvenPages;
		private XtraEditors.CheckEdit chkRowAndColumnHeadings;
		private XtraEditors.CheckEdit chkDraftQuality;
		private XtraEditors.CheckEdit chkGridlines;
		private XtraEditors.LabelControl lblPrint;
		private XtraEditors.TextEdit edtPrintArea;
		private XtraEditors.LabelControl lblPrintArea;
		private XtraEditors.LabelControl lblPageOrder;
		private XtraEditors.LabelControl lblCellErrorsAs;
		private XtraEditors.LabelControl lblComments;
		private XtraEditors.RadioGroup rgrpOrientation;
		private XtraEditors.LookUpEdit edtPaperSize;
		private XtraEditors.LookUpEdit edtPrintQuality;
		private XtraEditors.LookUpEdit edtFooter;
		private XtraEditors.LookUpEdit edtHeader;
		private XtraEditors.RadioGroup rgrpPageOrder;
		private XtraEditors.LookUpEdit edtCellErrorsAs;
		private XtraEditors.LookUpEdit edtComments;
		private XtraEditors.SpinEdit edtScale;
		private XtraEditors.LabelControl lblScaling;
		private XtraEditors.LabelControl lblNormalSize;
		private Design.HeaderFooterPreviewControl headerPreview;
		private Design.HeaderFooterPreviewControl footerPreview;
		private XtraEditors.LabelControl labelControl4;
		private XtraEditors.LabelControl labelControl5;
		private XtraEditors.LabelControl labelControl8;
		private XtraEditors.LabelControl labelControl7;
		private XtraEditors.LabelControl labelControl6;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.LabelControl lblTall;
		private XtraEditors.SpinEdit edtFitToHeight;
		private XtraEditors.LabelControl lblPagesWideBy;
		private XtraEditors.SpinEdit edtFitToWidth;
		private XtraEditors.RadioGroup rgrpFitToPage;
	}
}
