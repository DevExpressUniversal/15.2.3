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
	partial class EditStyleForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditStyleForm));
			this.edtName = new DevExpress.XtraEditors.TextEdit();
			this.cbParent = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbNextStyle = new DevExpress.XtraEditors.ComboBoxEdit();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.previewRichEditControl = new DevExpress.XtraRichEdit.RichEditControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barManager1 = new DevExpress.XtraBars.BarManager();
			this.barFontFormatting = new DevExpress.XtraBars.Bar();
			this.fontEditItem1 = new DevExpress.XtraBars.BarEditItem();
			this.repositoryItemFontEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.fontSizeEditItem1 = new DevExpress.XtraBars.BarEditItem();
			this.repositoryItemRichEditFontSizeEdit5 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.colorEditItem1 = new DevExpress.XtraBars.BarEditItem();
			this.repositoryItemOfficeColorPickEdit1 = new DevExpress.Office.UI.RepositoryItemOfficeColorPickEdit();
			this.toggleFontBoldItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.toggleFontItalicItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.toggleFontUnderlineItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.standaloneBarDockControl1 = new DevExpress.XtraBars.StandaloneBarDockControl();
			this.barParagraphFormatting = new DevExpress.XtraBars.Bar();
			this.toggleParagraphAlignmentLeftItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.toggleParagraphAlignmentCenterItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.toggleParagraphAlignmentRightItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.toggleParagraphAlignmentJustifyItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.barLineSpacingSubItem1 = new DevExpress.XtraBars.BarSubItem();
			this.changeParagraphLineSingleSpacingItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.changeParagraphLineSingleHalfSpacingItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.changeParagraphLineDoubleSpacingItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.spacingIncreaseItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.spacingDecreaseItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.indentDecreaseItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.indentIncreaseItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.standaloneBarDockControl2 = new DevExpress.XtraBars.StandaloneBarDockControl();
			this.barFontButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.barParagraphButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.barTabsButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.repositoryItemFontEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.repositoryItemRichEditFontSizeEdit1 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.repositoryItemRichEditStyleEdit1 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
			this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.repositoryItemColorEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
			this.repositoryItemRichEditFontSizeEdit2 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.repositoryItemRichEditFontSizeEdit3 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.repositoryItemRichEditFontSizeEdit4 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.repositoryItemFontEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.repositoryItemRichEditFontSizeEdit6 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.repositoryItemRichEditColorEdit1 = new DevExpress.Office.UI.RepositoryItemOfficeColorEdit();
			this.repositoryItemFontEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.repositoryItemRichEditFontSizeEdit7 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.repositoryItemRichEditStyleEdit2 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
			this.repositoryItemFontEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.repositoryItemRichEditFontSizeEdit8 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.repositoryItemRichEditStyleEdit3 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
			this.repositoryItemRichEditStyleEdit4 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
			this.repositoryItemRichEditStyleEdit5 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
			this.lblProperties = new DevExpress.XtraEditors.LabelControl();
			this.lblFormatting = new DevExpress.XtraEditors.LabelControl();
			this.lblSeparator = new DevExpress.XtraEditors.LabelControl();
			this.btnFormat = new DevExpress.XtraEditors.DropDownButton();
			this.popupMenuFormat = new DevExpress.XtraBars.PopupMenu();
			this.lblName = new DevExpress.XtraEditors.LabelControl();
			this.lblStyleBasedOn = new DevExpress.XtraEditors.LabelControl();
			this.lblStyleForFollowingParagraph = new DevExpress.XtraEditors.LabelControl();
			this.cbCurrentStyle = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblCurrentStyle = new DevExpress.XtraEditors.LabelControl();
			this.lblSelectedStyle = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.edtName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbParent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbNextStyle.Properties)).BeginInit();
			this.previewRichEditControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemOfficeColorPickEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditColorEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenuFormat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCurrentStyle.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.edtName, "edtName");
			this.edtName.Name = "edtName";
			resources.ApplyResources(this.cbParent, "cbParent");
			this.cbParent.Name = "cbParent";
			this.cbParent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbParent.Properties.Buttons"))))});
			this.cbParent.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.cbNextStyle, "cbNextStyle");
			this.cbNextStyle.Name = "cbNextStyle";
			this.cbNextStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbNextStyle.Properties.Buttons"))))});
			this.cbNextStyle.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnOkClick);
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.previewRichEditControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
			this.previewRichEditControl.Controls.Add(this.barDockControlLeft);
			this.previewRichEditControl.Controls.Add(this.barDockControlRight);
			this.previewRichEditControl.Controls.Add(this.barDockControlBottom);
			this.previewRichEditControl.Controls.Add(this.barDockControlTop);
			resources.ApplyResources(this.previewRichEditControl, "previewRichEditControl");
			this.previewRichEditControl.MenuManager = this.barManager1;
			this.previewRichEditControl.Name = "previewRichEditControl";
			this.previewRichEditControl.Options.Fields.UseCurrentCultureDateTimeFormat = false;
			this.previewRichEditControl.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
			this.previewRichEditControl.Options.HorizontalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
			this.previewRichEditControl.Options.MailMerge.KeepLastParagraph = false;
			this.previewRichEditControl.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
			this.previewRichEditControl.Options.VerticalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barManager1.AllowCustomization = false;
			this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.barFontFormatting,
			this.barParagraphFormatting});
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.DockControls.Add(this.standaloneBarDockControl2);
			this.barManager1.DockControls.Add(this.standaloneBarDockControl1);
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.toggleFontBoldItem1,
			this.toggleFontItalicItem1,
			this.toggleFontUnderlineItem1,
			this.toggleParagraphAlignmentLeftItem1,
			this.toggleParagraphAlignmentRightItem1,
			this.toggleParagraphAlignmentCenterItem1,
			this.toggleParagraphAlignmentJustifyItem1,
			this.spacingIncreaseItem1,
			this.spacingDecreaseItem1,
			this.indentDecreaseItem1,
			this.indentIncreaseItem1,
			this.fontEditItem1,
			this.fontSizeEditItem1,
			this.barFontButtonItem1,
			this.barParagraphButtonItem1,
			this.barTabsButtonItem1,
			this.barLineSpacingSubItem1,
			this.changeParagraphLineSingleSpacingItem1,
			this.changeParagraphLineSingleHalfSpacingItem1,
			this.changeParagraphLineDoubleSpacingItem1,
			this.colorEditItem1});
			this.barManager1.MaxItemId = 223;
			this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemFontEdit1,
			this.repositoryItemRichEditFontSizeEdit1,
			this.repositoryItemRichEditStyleEdit1,
			this.repositoryItemComboBox1,
			this.repositoryItemFontEdit2,
			this.repositoryItemColorEdit1,
			this.repositoryItemRichEditFontSizeEdit2,
			this.repositoryItemRichEditFontSizeEdit3,
			this.repositoryItemRichEditFontSizeEdit4,
			this.repositoryItemCheckEdit1,
			this.repositoryItemRichEditFontSizeEdit5,
			this.repositoryItemFontEdit3,
			this.repositoryItemRichEditFontSizeEdit6,
			this.repositoryItemRichEditColorEdit1,
			this.repositoryItemFontEdit4,
			this.repositoryItemRichEditFontSizeEdit7,
			this.repositoryItemRichEditStyleEdit2,
			this.repositoryItemFontEdit5,
			this.repositoryItemRichEditFontSizeEdit8,
			this.repositoryItemRichEditStyleEdit3,
			this.repositoryItemRichEditStyleEdit4,
			this.repositoryItemRichEditStyleEdit5,
			this.repositoryItemOfficeColorPickEdit1});
			this.barFontFormatting.BarName = "Font";
			this.barFontFormatting.DockCol = 0;
			this.barFontFormatting.DockRow = 0;
			this.barFontFormatting.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
			this.barFontFormatting.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.fontEditItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.fontSizeEditItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.colorEditItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontBoldItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontItalicItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontUnderlineItem1)});
			this.barFontFormatting.OptionsBar.AllowQuickCustomization = false;
			this.barFontFormatting.OptionsBar.DisableClose = true;
			this.barFontFormatting.OptionsBar.DisableCustomization = true;
			this.barFontFormatting.OptionsBar.DrawDragBorder = false;
			this.barFontFormatting.OptionsBar.Hidden = true;
			this.barFontFormatting.OptionsBar.UseWholeRow = true;
			this.barFontFormatting.StandaloneBarDockControl = this.standaloneBarDockControl1;
			resources.ApplyResources(this.barFontFormatting, "barFontFormatting");
			this.fontEditItem1.Edit = this.repositoryItemFontEdit2;
			this.fontEditItem1.Id = 67;
			this.fontEditItem1.Name = "fontEditItem1";
			resources.ApplyResources(this.fontEditItem1, "fontEditItem1");
			this.fontEditItem1.EditValueChanged += new System.EventHandler(this.fontEditItem1_EditValueChanged);
			resources.ApplyResources(this.repositoryItemFontEdit2, "repositoryItemFontEdit2");
			this.repositoryItemFontEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemFontEdit2.Buttons"))))});
			this.repositoryItemFontEdit2.Name = "repositoryItemFontEdit2";
			this.fontSizeEditItem1.CausesValidation = true;
			this.fontSizeEditItem1.Edit = this.repositoryItemRichEditFontSizeEdit5;
			this.fontSizeEditItem1.Id = 74;
			this.fontSizeEditItem1.Name = "fontSizeEditItem1";
			this.fontSizeEditItem1.EditValueChanged += new System.EventHandler(this.fontSizeEditItem1_EditValueChanged);
			resources.ApplyResources(this.repositoryItemRichEditFontSizeEdit5, "repositoryItemRichEditFontSizeEdit5");
			this.repositoryItemRichEditFontSizeEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditFontSizeEdit5.Buttons"))))});
			this.repositoryItemRichEditFontSizeEdit5.Control = this.previewRichEditControl;
			this.repositoryItemRichEditFontSizeEdit5.Name = "repositoryItemRichEditFontSizeEdit5";
			resources.ApplyResources(this.colorEditItem1, "colorEditItem1");
			this.colorEditItem1.Edit = this.repositoryItemOfficeColorPickEdit1;
			this.colorEditItem1.Id = 222;
			this.colorEditItem1.Name = "colorEditItem1";
			this.colorEditItem1.EditValueChanged += new System.EventHandler(this.colorEditItem1_EditValueChanged);
			resources.ApplyResources(this.repositoryItemOfficeColorPickEdit1, "repositoryItemOfficeColorPickEdit1");
			this.repositoryItemOfficeColorPickEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemOfficeColorPickEdit1.Buttons"))))});
			this.repositoryItemOfficeColorPickEdit1.Name = "repositoryItemOfficeColorPickEdit1";
			resources.ApplyResources(this.toggleFontBoldItem1, "toggleFontBoldItem1");
			this.toggleFontBoldItem1.Id = 49;
			this.toggleFontBoldItem1.Name = "toggleFontBoldItem1";
			this.toggleFontBoldItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.toggleFontBoldItem1_CheckedChanged);
			resources.ApplyResources(this.toggleFontItalicItem1, "toggleFontItalicItem1");
			this.toggleFontItalicItem1.Id = 53;
			this.toggleFontItalicItem1.Name = "toggleFontItalicItem1";
			this.toggleFontItalicItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.toggleFontItalicItem1_CheckedChanged);
			resources.ApplyResources(this.toggleFontUnderlineItem1, "toggleFontUnderlineItem1");
			this.toggleFontUnderlineItem1.Id = 54;
			this.toggleFontUnderlineItem1.Name = "toggleFontUnderlineItem1";
			this.toggleFontUnderlineItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.toggleFontUnderlineItem1_CheckedChanged);
			this.standaloneBarDockControl1.CausesValidation = false;
			resources.ApplyResources(this.standaloneBarDockControl1, "standaloneBarDockControl1");
			this.standaloneBarDockControl1.Name = "standaloneBarDockControl1";
			this.barParagraphFormatting.BarName = "Paragraph";
			this.barParagraphFormatting.DockCol = 0;
			this.barParagraphFormatting.DockRow = 0;
			this.barParagraphFormatting.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
			this.barParagraphFormatting.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentLeftItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentCenterItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentRightItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentJustifyItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLineSpacingSubItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.spacingIncreaseItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.spacingDecreaseItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.indentDecreaseItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.indentIncreaseItem1)});
			this.barParagraphFormatting.OptionsBar.AllowQuickCustomization = false;
			this.barParagraphFormatting.OptionsBar.DisableClose = true;
			this.barParagraphFormatting.OptionsBar.DisableCustomization = true;
			this.barParagraphFormatting.OptionsBar.DrawDragBorder = false;
			this.barParagraphFormatting.OptionsBar.Hidden = true;
			this.barParagraphFormatting.OptionsBar.UseWholeRow = true;
			this.barParagraphFormatting.StandaloneBarDockControl = this.standaloneBarDockControl2;
			resources.ApplyResources(this.barParagraphFormatting, "barParagraphFormatting");
			resources.ApplyResources(this.toggleParagraphAlignmentLeftItem1, "toggleParagraphAlignmentLeftItem1");
			this.toggleParagraphAlignmentLeftItem1.Id = 55;
			this.toggleParagraphAlignmentLeftItem1.Name = "toggleParagraphAlignmentLeftItem1";
			this.toggleParagraphAlignmentLeftItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.toggleParagraphAlignmentLeftItem1_CheckedChanged);
			resources.ApplyResources(this.toggleParagraphAlignmentCenterItem1, "toggleParagraphAlignmentCenterItem1");
			this.toggleParagraphAlignmentCenterItem1.Id = 57;
			this.toggleParagraphAlignmentCenterItem1.Name = "toggleParagraphAlignmentCenterItem1";
			this.toggleParagraphAlignmentCenterItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.toggleParagraphAlignmentCenterItem1_CheckedChanged);
			resources.ApplyResources(this.toggleParagraphAlignmentRightItem1, "toggleParagraphAlignmentRightItem1");
			this.toggleParagraphAlignmentRightItem1.Id = 56;
			this.toggleParagraphAlignmentRightItem1.Name = "toggleParagraphAlignmentRightItem1";
			this.toggleParagraphAlignmentRightItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.toggleParagraphAlignmentRightItem1_CheckedChanged);
			resources.ApplyResources(this.toggleParagraphAlignmentJustifyItem1, "toggleParagraphAlignmentJustifyItem1");
			this.toggleParagraphAlignmentJustifyItem1.Id = 58;
			this.toggleParagraphAlignmentJustifyItem1.Name = "toggleParagraphAlignmentJustifyItem1";
			this.toggleParagraphAlignmentJustifyItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.toggleParagraphAlignmentJustifyItem1_CheckedChanged);
			resources.ApplyResources(this.barLineSpacingSubItem1, "barLineSpacingSubItem1");
			this.barLineSpacingSubItem1.Id = 213;
			this.barLineSpacingSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.changeParagraphLineSingleSpacingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeParagraphLineSingleHalfSpacingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeParagraphLineDoubleSpacingItem1)});
			this.barLineSpacingSubItem1.Name = "barLineSpacingSubItem1";
			resources.ApplyResources(this.changeParagraphLineSingleSpacingItem1, "changeParagraphLineSingleSpacingItem1");
			this.changeParagraphLineSingleSpacingItem1.Id = 214;
			this.changeParagraphLineSingleSpacingItem1.Name = "changeParagraphLineSingleSpacingItem1";
			resources.ApplyResources(this.changeParagraphLineSingleHalfSpacingItem1, "changeParagraphLineSingleHalfSpacingItem1");
			this.changeParagraphLineSingleHalfSpacingItem1.Id = 215;
			this.changeParagraphLineSingleHalfSpacingItem1.Name = "changeParagraphLineSingleHalfSpacingItem1";
			resources.ApplyResources(this.changeParagraphLineDoubleSpacingItem1, "changeParagraphLineDoubleSpacingItem1");
			this.changeParagraphLineDoubleSpacingItem1.Id = 216;
			this.changeParagraphLineDoubleSpacingItem1.Name = "changeParagraphLineDoubleSpacingItem1";
			resources.ApplyResources(this.spacingIncreaseItem1, "spacingIncreaseItem1");
			this.spacingIncreaseItem1.Id = 63;
			this.spacingIncreaseItem1.Name = "spacingIncreaseItem1";
			this.spacingIncreaseItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.spacingIncreaseItem1_ItemClick);
			resources.ApplyResources(this.spacingDecreaseItem1, "spacingDecreaseItem1");
			this.spacingDecreaseItem1.Id = 64;
			this.spacingDecreaseItem1.Name = "spacingDecreaseItem1";
			this.spacingDecreaseItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.spacingDecreaseItem1_ItemClick);
			resources.ApplyResources(this.indentDecreaseItem1, "indentDecreaseItem1");
			this.indentDecreaseItem1.Id = 65;
			this.indentDecreaseItem1.Name = "indentDecreaseItem1";
			this.indentDecreaseItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.indentDecreaseItem1_ItemClick);
			resources.ApplyResources(this.indentIncreaseItem1, "indentIncreaseItem1");
			this.indentIncreaseItem1.Id = 66;
			this.indentIncreaseItem1.Name = "indentIncreaseItem1";
			this.indentIncreaseItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.indentIncreaseItem1_ItemClick);
			this.standaloneBarDockControl2.CausesValidation = false;
			resources.ApplyResources(this.standaloneBarDockControl2, "standaloneBarDockControl2");
			this.standaloneBarDockControl2.Name = "standaloneBarDockControl2";
			resources.ApplyResources(this.barFontButtonItem1, "barFontButtonItem1");
			this.barFontButtonItem1.Id = 210;
			this.barFontButtonItem1.Name = "barFontButtonItem1";
			this.barFontButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barFontButtonItem1_ItemClick);
			resources.ApplyResources(this.barParagraphButtonItem1, "barParagraphButtonItem1");
			this.barParagraphButtonItem1.Id = 211;
			this.barParagraphButtonItem1.Name = "barParagraphButtonItem1";
			this.barParagraphButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barParagraphButtonItem1_ItemClick);
			resources.ApplyResources(this.barTabsButtonItem1, "barTabsButtonItem1");
			this.barTabsButtonItem1.Id = 212;
			this.barTabsButtonItem1.Name = "barTabsButtonItem1";
			this.barTabsButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barTabsButtonItem1_ItemClick);
			resources.ApplyResources(this.repositoryItemFontEdit1, "repositoryItemFontEdit1");
			this.repositoryItemFontEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemFontEdit1.Buttons"))))});
			this.repositoryItemFontEdit1.Name = "repositoryItemFontEdit1";
			resources.ApplyResources(this.repositoryItemRichEditFontSizeEdit1, "repositoryItemRichEditFontSizeEdit1");
			this.repositoryItemRichEditFontSizeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditFontSizeEdit1.Buttons"))))});
			this.repositoryItemRichEditFontSizeEdit1.Control = this.previewRichEditControl;
			this.repositoryItemRichEditFontSizeEdit1.Name = "repositoryItemRichEditFontSizeEdit1";
			resources.ApplyResources(this.repositoryItemRichEditStyleEdit1, "repositoryItemRichEditStyleEdit1");
			this.repositoryItemRichEditStyleEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditStyleEdit1.Buttons"))))});
			this.repositoryItemRichEditStyleEdit1.Control = this.previewRichEditControl;
			this.repositoryItemRichEditStyleEdit1.Name = "repositoryItemRichEditStyleEdit1";
			resources.ApplyResources(this.repositoryItemComboBox1, "repositoryItemComboBox1");
			this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemComboBox1.Buttons"))))});
			this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
			resources.ApplyResources(this.repositoryItemColorEdit1, "repositoryItemColorEdit1");
			this.repositoryItemColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemColorEdit1.Buttons"))))});
			this.repositoryItemColorEdit1.Name = "repositoryItemColorEdit1";
			resources.ApplyResources(this.repositoryItemRichEditFontSizeEdit2, "repositoryItemRichEditFontSizeEdit2");
			this.repositoryItemRichEditFontSizeEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditFontSizeEdit2.Buttons"))))});
			this.repositoryItemRichEditFontSizeEdit2.Control = null;
			this.repositoryItemRichEditFontSizeEdit2.Name = "repositoryItemRichEditFontSizeEdit2";
			resources.ApplyResources(this.repositoryItemRichEditFontSizeEdit3, "repositoryItemRichEditFontSizeEdit3");
			this.repositoryItemRichEditFontSizeEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditFontSizeEdit3.Buttons"))))});
			this.repositoryItemRichEditFontSizeEdit3.Control = null;
			this.repositoryItemRichEditFontSizeEdit3.Name = "repositoryItemRichEditFontSizeEdit3";
			resources.ApplyResources(this.repositoryItemRichEditFontSizeEdit4, "repositoryItemRichEditFontSizeEdit4");
			this.repositoryItemRichEditFontSizeEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditFontSizeEdit4.Buttons"))))});
			this.repositoryItemRichEditFontSizeEdit4.Control = null;
			this.repositoryItemRichEditFontSizeEdit4.Name = "repositoryItemRichEditFontSizeEdit4";
			resources.ApplyResources(this.repositoryItemCheckEdit1, "repositoryItemCheckEdit1");
			this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
			resources.ApplyResources(this.repositoryItemFontEdit3, "repositoryItemFontEdit3");
			this.repositoryItemFontEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemFontEdit3.Buttons"))))});
			this.repositoryItemFontEdit3.Name = "repositoryItemFontEdit3";
			resources.ApplyResources(this.repositoryItemRichEditFontSizeEdit6, "repositoryItemRichEditFontSizeEdit6");
			this.repositoryItemRichEditFontSizeEdit6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditFontSizeEdit6.Buttons"))))});
			this.repositoryItemRichEditFontSizeEdit6.Control = this.previewRichEditControl;
			this.repositoryItemRichEditFontSizeEdit6.Name = "repositoryItemRichEditFontSizeEdit6";
			resources.ApplyResources(this.repositoryItemRichEditColorEdit1, "repositoryItemRichEditColorEdit1");
			this.repositoryItemRichEditColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditColorEdit1.Buttons"))))});
			this.repositoryItemRichEditColorEdit1.Name = "repositoryItemRichEditColorEdit1";
			resources.ApplyResources(this.repositoryItemFontEdit4, "repositoryItemFontEdit4");
			this.repositoryItemFontEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemFontEdit4.Buttons"))))});
			this.repositoryItemFontEdit4.Name = "repositoryItemFontEdit4";
			resources.ApplyResources(this.repositoryItemRichEditFontSizeEdit7, "repositoryItemRichEditFontSizeEdit7");
			this.repositoryItemRichEditFontSizeEdit7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditFontSizeEdit7.Buttons"))))});
			this.repositoryItemRichEditFontSizeEdit7.Control = this.previewRichEditControl;
			this.repositoryItemRichEditFontSizeEdit7.Name = "repositoryItemRichEditFontSizeEdit7";
			resources.ApplyResources(this.repositoryItemRichEditStyleEdit2, "repositoryItemRichEditStyleEdit2");
			this.repositoryItemRichEditStyleEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditStyleEdit2.Buttons"))))});
			this.repositoryItemRichEditStyleEdit2.Control = this.previewRichEditControl;
			this.repositoryItemRichEditStyleEdit2.Name = "repositoryItemRichEditStyleEdit2";
			resources.ApplyResources(this.repositoryItemFontEdit5, "repositoryItemFontEdit5");
			this.repositoryItemFontEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemFontEdit5.Buttons"))))});
			this.repositoryItemFontEdit5.Name = "repositoryItemFontEdit5";
			resources.ApplyResources(this.repositoryItemRichEditFontSizeEdit8, "repositoryItemRichEditFontSizeEdit8");
			this.repositoryItemRichEditFontSizeEdit8.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditFontSizeEdit8.Buttons"))))});
			this.repositoryItemRichEditFontSizeEdit8.Control = this.previewRichEditControl;
			this.repositoryItemRichEditFontSizeEdit8.Name = "repositoryItemRichEditFontSizeEdit8";
			resources.ApplyResources(this.repositoryItemRichEditStyleEdit3, "repositoryItemRichEditStyleEdit3");
			this.repositoryItemRichEditStyleEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditStyleEdit3.Buttons"))))});
			this.repositoryItemRichEditStyleEdit3.Control = this.previewRichEditControl;
			this.repositoryItemRichEditStyleEdit3.Name = "repositoryItemRichEditStyleEdit3";
			resources.ApplyResources(this.repositoryItemRichEditStyleEdit4, "repositoryItemRichEditStyleEdit4");
			this.repositoryItemRichEditStyleEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditStyleEdit4.Buttons"))))});
			this.repositoryItemRichEditStyleEdit4.Control = null;
			this.repositoryItemRichEditStyleEdit4.Name = "repositoryItemRichEditStyleEdit4";
			resources.ApplyResources(this.repositoryItemRichEditStyleEdit5, "repositoryItemRichEditStyleEdit5");
			this.repositoryItemRichEditStyleEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditStyleEdit5.Buttons"))))});
			this.repositoryItemRichEditStyleEdit5.Control = null;
			this.repositoryItemRichEditStyleEdit5.Name = "repositoryItemRichEditStyleEdit5";
			resources.ApplyResources(this.lblProperties, "lblProperties");
			this.lblProperties.LineVisible = true;
			this.lblProperties.Name = "lblProperties";
			resources.ApplyResources(this.lblFormatting, "lblFormatting");
			this.lblFormatting.LineVisible = true;
			this.lblFormatting.Name = "lblFormatting";
			resources.ApplyResources(this.lblSeparator, "lblSeparator");
			this.lblSeparator.LineVisible = true;
			this.lblSeparator.Name = "lblSeparator";
			this.btnFormat.DropDownControl = this.popupMenuFormat;
			resources.ApplyResources(this.btnFormat, "btnFormat");
			this.btnFormat.MenuManager = this.barManager1;
			this.btnFormat.Name = "btnFormat";
			this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
			this.popupMenuFormat.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barFontButtonItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.barParagraphButtonItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.barTabsButtonItem1)});
			this.popupMenuFormat.Manager = this.barManager1;
			this.popupMenuFormat.Name = "popupMenuFormat";
			resources.ApplyResources(this.lblName, "lblName");
			this.lblName.Name = "lblName";
			resources.ApplyResources(this.lblStyleBasedOn, "lblStyleBasedOn");
			this.lblStyleBasedOn.Name = "lblStyleBasedOn";
			resources.ApplyResources(this.lblStyleForFollowingParagraph, "lblStyleForFollowingParagraph");
			this.lblStyleForFollowingParagraph.Name = "lblStyleForFollowingParagraph";
			resources.ApplyResources(this.cbCurrentStyle, "cbCurrentStyle");
			this.cbCurrentStyle.Name = "cbCurrentStyle";
			this.cbCurrentStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbCurrentStyle.Properties.Buttons"))))});
			this.cbCurrentStyle.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lblCurrentStyle, "lblCurrentStyle");
			this.lblCurrentStyle.Name = "lblCurrentStyle";
			resources.ApplyResources(this.lblSelectedStyle, "lblSelectedStyle");
			this.lblSelectedStyle.LineVisible = true;
			this.lblSelectedStyle.Name = "lblSelectedStyle";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblSelectedStyle);
			this.Controls.Add(this.lblCurrentStyle);
			this.Controls.Add(this.lblStyleForFollowingParagraph);
			this.Controls.Add(this.lblStyleBasedOn);
			this.Controls.Add(this.lblName);
			this.Controls.Add(this.btnFormat);
			this.Controls.Add(this.lblSeparator);
			this.Controls.Add(this.lblFormatting);
			this.Controls.Add(this.lblProperties);
			this.Controls.Add(this.standaloneBarDockControl1);
			this.Controls.Add(this.standaloneBarDockControl2);
			this.Controls.Add(this.previewRichEditControl);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cbNextStyle);
			this.Controls.Add(this.cbCurrentStyle);
			this.Controls.Add(this.cbParent);
			this.Controls.Add(this.edtName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditStyleForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.edtName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbParent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbNextStyle.Properties)).EndInit();
			this.previewRichEditControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemOfficeColorPickEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditColorEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenuFormat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCurrentStyle.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.TextEdit edtName;
		protected DevExpress.XtraEditors.ComboBoxEdit cbParent;
		protected DevExpress.XtraEditors.ComboBoxEdit cbNextStyle;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected RichEditControl previewRichEditControl;
		protected DevExpress.XtraBars.BarManager barManager1;
		protected DevExpress.XtraBars.BarDockControl barDockControlTop;
		protected DevExpress.XtraBars.BarDockControl barDockControlBottom;
		protected DevExpress.XtraBars.BarDockControl barDockControlLeft;
		protected DevExpress.XtraBars.BarDockControl barDockControlRight;
		protected DevExpress.XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit1;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit1;
		protected DevExpress.XtraBars.StandaloneBarDockControl standaloneBarDockControl2;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit1;
		protected DevExpress.XtraBars.StandaloneBarDockControl standaloneBarDockControl1;
		protected DevExpress.XtraBars.BarCheckItem toggleFontBoldItem1;
		protected DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
		protected DevExpress.XtraBars.BarCheckItem toggleFontItalicItem1;
		protected DevExpress.XtraBars.BarCheckItem toggleFontUnderlineItem1;
		protected DevExpress.XtraBars.BarCheckItem toggleParagraphAlignmentLeftItem1;
		protected DevExpress.XtraBars.BarCheckItem toggleParagraphAlignmentCenterItem1;
		protected DevExpress.XtraBars.BarCheckItem toggleParagraphAlignmentRightItem1;
		protected DevExpress.XtraBars.BarCheckItem toggleParagraphAlignmentJustifyItem1;
		protected DevExpress.XtraBars.BarButtonItem spacingIncreaseItem1;
		protected DevExpress.XtraBars.BarButtonItem spacingDecreaseItem1;
		protected DevExpress.XtraBars.BarButtonItem indentDecreaseItem1;
		protected DevExpress.XtraBars.BarButtonItem indentIncreaseItem1;
		protected DevExpress.XtraBars.BarEditItem fontEditItem1;
		protected DevExpress.XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit2;
		protected DevExpress.XtraEditors.Repository.RepositoryItemColorEdit repositoryItemColorEdit1;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit2;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit3;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit4;
		protected DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
		protected DevExpress.XtraBars.BarEditItem fontSizeEditItem1;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit5;
		protected DevExpress.XtraEditors.LabelControl lblProperties;
		protected DevExpress.XtraEditors.LabelControl lblFormatting;
		protected DevExpress.XtraEditors.LabelControl lblSeparator;
		protected DevExpress.XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit3;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit6;
		protected DevExpress.Office.UI.RepositoryItemOfficeColorEdit repositoryItemRichEditColorEdit1;
		protected DevExpress.XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit4;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit7;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit2;
		protected DevExpress.XtraEditors.DropDownButton btnFormat;
		protected DevExpress.XtraEditors.LabelControl lblStyleForFollowingParagraph;
		protected DevExpress.XtraEditors.LabelControl lblStyleBasedOn;
		protected DevExpress.XtraEditors.LabelControl lblName;
		protected DevExpress.XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit5;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit8;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit3;
		protected DevExpress.XtraBars.PopupMenu popupMenuFormat;
		protected DevExpress.XtraBars.BarButtonItem barFontButtonItem1;
		protected DevExpress.XtraBars.BarButtonItem barParagraphButtonItem1;
		protected DevExpress.XtraBars.BarButtonItem barTabsButtonItem1;
		protected DevExpress.XtraBars.BarSubItem barLineSpacingSubItem1;
		protected DevExpress.XtraBars.BarCheckItem changeParagraphLineSingleSpacingItem1;
		protected DevExpress.XtraBars.BarCheckItem changeParagraphLineSingleHalfSpacingItem1;
		protected DevExpress.XtraBars.BarCheckItem changeParagraphLineDoubleSpacingItem1;
		protected DevExpress.XtraBars.Bar barFontFormatting;
		protected DevExpress.XtraBars.Bar barParagraphFormatting;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit4;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit5;
		protected XtraEditors.ComboBoxEdit cbCurrentStyle;
		protected XtraEditors.LabelControl lblCurrentStyle;
		protected XtraEditors.LabelControl lblSelectedStyle;
		private XtraBars.BarEditItem colorEditItem1;
		private Office.UI.RepositoryItemOfficeColorPickEdit repositoryItemOfficeColorPickEdit1;
	}
}
