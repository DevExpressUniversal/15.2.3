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

namespace DevExpress.XtraPrinting.Design.Forms {
	partial class RichTextEditorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
			DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
			DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
			DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
			DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
			DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RichTextEditorForm));
			DevExpress.Utils.SuperToolTip superToolTip3 = new DevExpress.Utils.SuperToolTip();
			DevExpress.Utils.ToolTipTitleItem toolTipTitleItem3 = new DevExpress.Utils.ToolTipTitleItem();
			DevExpress.Utils.ToolTipItem toolTipItem3 = new DevExpress.Utils.ToolTipItem();
			DevExpress.Utils.SuperToolTip superToolTip4 = new DevExpress.Utils.SuperToolTip();
			DevExpress.Utils.ToolTipTitleItem toolTipTitleItem4 = new DevExpress.Utils.ToolTipTitleItem();
			DevExpress.Utils.ToolTipItem toolTipItem4 = new DevExpress.Utils.ToolTipItem();
			this.textBox = new DevExpress.XtraPrinting.Design.Forms.EditControl();
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.fontBar1 = new DevExpress.XtraRichEdit.UI.FontBar();
			this.changeFontNameItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontNameItem();
			this.repositoryItemFontEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.changeFontSizeItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontSizeItem();
			this.repositoryItemRichEditFontSizeEdit1 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.fontSizeIncreaseItem1 = new DevExpress.XtraRichEdit.UI.FontSizeIncreaseItem();
			this.fontSizeDecreaseItem1 = new DevExpress.XtraRichEdit.UI.FontSizeDecreaseItem();
			this.toggleFontBoldItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontBoldItem();
			this.toggleFontItalicItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontItalicItem();
			this.toggleFontUnderlineItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontUnderlineItem();
			this.toggleFontDoubleUnderlineItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontDoubleUnderlineItem();
			this.toggleFontStrikeoutItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontStrikeoutItem();
			this.toggleFontDoubleStrikeoutItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontDoubleStrikeoutItem();
			this.changeFontColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontColorItem();
			this.changeFontBackColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontBackColorItem();
			this.clearFormattingItem1 = new DevExpress.XtraRichEdit.UI.ClearFormattingItem();
			this.showFontFormItem1 = new DevExpress.XtraRichEdit.UI.ShowFontFormItem();
			this.paragraphBar1 = new DevExpress.XtraRichEdit.UI.ParagraphBar();
			this.toggleBulletedListItem1 = new DevExpress.XtraRichEdit.UI.ToggleBulletedListItem();
			this.toggleNumberingListItem1 = new DevExpress.XtraRichEdit.UI.ToggleNumberingListItem();
			this.toggleMultiLevelListItem1 = new DevExpress.XtraRichEdit.UI.ToggleMultiLevelListItem();
			this.decreaseIndentItem1 = new DevExpress.XtraRichEdit.UI.DecreaseIndentItem();
			this.increaseIndentItem1 = new DevExpress.XtraRichEdit.UI.IncreaseIndentItem();
			this.toggleParagraphAlignmentLeftItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentLeftItem();
			this.toggleParagraphAlignmentCenterItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentCenterItem();
			this.toggleParagraphAlignmentRightItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentRightItem();
			this.toggleParagraphAlignmentJustifyItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentJustifyItem();
			this.toggleShowWhitespaceItem1 = new DevExpress.XtraRichEdit.UI.ToggleShowWhitespaceItem();
			this.changeParagraphLineSpacingItem1 = new DevExpress.XtraRichEdit.UI.ChangeParagraphLineSpacingItem();
			this.setSingleParagraphSpacingItem1 = new DevExpress.XtraRichEdit.UI.SetSingleParagraphSpacingItem();
			this.setSesquialteralParagraphSpacingItem1 = new DevExpress.XtraRichEdit.UI.SetSesquialteralParagraphSpacingItem();
			this.setDoubleParagraphSpacingItem1 = new DevExpress.XtraRichEdit.UI.SetDoubleParagraphSpacingItem();
			this.showLineSpacingFormItem1 = new DevExpress.XtraRichEdit.UI.ShowLineSpacingFormItem();
			this.addSpacingBeforeParagraphItem1 = new DevExpress.XtraRichEdit.UI.AddSpacingBeforeParagraphItem();
			this.removeSpacingBeforeParagraphItem1 = new DevExpress.XtraRichEdit.UI.RemoveSpacingBeforeParagraphItem();
			this.addSpacingAfterParagraphItem1 = new DevExpress.XtraRichEdit.UI.AddSpacingAfterParagraphItem();
			this.removeSpacingAfterParagraphItem1 = new DevExpress.XtraRichEdit.UI.RemoveSpacingAfterParagraphItem();
			this.changeParagraphBackColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeParagraphBackColorItem();
			this.showParagraphFormItem1 = new DevExpress.XtraRichEdit.UI.ShowParagraphFormItem();
			this.insertPictureItem1 = new DevExpress.XtraRichEdit.UI.InsertPictureItem();
			this.insertTableItem1 = new DevExpress.XtraRichEdit.UI.InsertTableItem();
			this.headerFooterBar1 = new DevExpress.XtraRichEdit.UI.HeaderFooterBar();
			this.insertPageNumberItem2 = new DevExpress.XtraBars.BarButtonItem();
			this.insertPageCountItem2 = new DevExpress.XtraBars.BarButtonItem();
			this.insertDateItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.insertUserItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.insertBookmarkItem1 = new DevExpress.XtraRichEdit.UI.InsertBookmarkItem();
			this.insertHyperlinkItem1 = new DevExpress.XtraRichEdit.UI.InsertHyperlinkItem();
			this.insertPageNumberItem1 = new DevExpress.XtraRichEdit.UI.InsertPageNumberItem();
			this.insertPageCountItem1 = new DevExpress.XtraRichEdit.UI.InsertPageCountItem();
			this.insertSymbolItem1 = new DevExpress.XtraRichEdit.UI.InsertSymbolItem();
			this.repositoryItemRichEditStyleEdit1 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
			this.okButton = new DevExpress.XtraEditors.SimpleButton();
			this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.richEditBarController1 = new DevExpress.XtraRichEdit.UI.RichEditBarController();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit1)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.richEditBarController1)).BeginInit();
			this.SuspendLayout();
			this.textBox.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Draft;
			this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox.EnableToolTips = true;
			this.textBox.Location = new System.Drawing.Point(0, 62);
			this.textBox.MenuManager = this.barManager1;
			this.textBox.Name = "textBox";
			this.textBox.Options.Comments.ShowAllAuthors = false;
			this.textBox.Options.DocumentCapabilities.Bookmarks = DevExpress.XtraRichEdit.DocumentCapability.Hidden;
			this.textBox.Options.DocumentCapabilities.FloatingObjects = DevExpress.XtraRichEdit.DocumentCapability.Hidden;
			this.textBox.Options.DocumentCapabilities.HeadersFooters = DevExpress.XtraRichEdit.DocumentCapability.Hidden;
			this.textBox.Options.DocumentCapabilities.Hyperlinks = DevExpress.XtraRichEdit.DocumentCapability.Hidden;
			this.textBox.Options.Fields.UseCurrentCultureDateTimeFormat = false;
			this.textBox.Options.MailMerge.KeepLastParagraph = false;
			this.textBox.Size = new System.Drawing.Size(898, 555);
			this.textBox.TabIndex = 0;
			this.textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
			this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.fontBar1,
			this.paragraphBar1,
			this.headerFooterBar1});
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.changeFontNameItem1,
			this.changeFontSizeItem1,
			this.fontSizeIncreaseItem1,
			this.fontSizeDecreaseItem1,
			this.toggleFontBoldItem1,
			this.toggleFontItalicItem1,
			this.toggleFontUnderlineItem1,
			this.toggleFontDoubleUnderlineItem1,
			this.toggleFontStrikeoutItem1,
			this.toggleFontDoubleStrikeoutItem1,
			this.changeFontColorItem1,
			this.changeFontBackColorItem1,
			this.clearFormattingItem1,
			this.showFontFormItem1,
			this.toggleBulletedListItem1,
			this.toggleNumberingListItem1,
			this.toggleMultiLevelListItem1,
			this.decreaseIndentItem1,
			this.increaseIndentItem1,
			this.toggleParagraphAlignmentLeftItem1,
			this.toggleParagraphAlignmentCenterItem1,
			this.toggleParagraphAlignmentRightItem1,
			this.toggleParagraphAlignmentJustifyItem1,
			this.toggleShowWhitespaceItem1,
			this.changeParagraphLineSpacingItem1,
			this.setSingleParagraphSpacingItem1,
			this.setSesquialteralParagraphSpacingItem1,
			this.setDoubleParagraphSpacingItem1,
			this.showLineSpacingFormItem1,
			this.addSpacingBeforeParagraphItem1,
			this.removeSpacingBeforeParagraphItem1,
			this.addSpacingAfterParagraphItem1,
			this.removeSpacingAfterParagraphItem1,
			this.changeParagraphBackColorItem1,
			this.showParagraphFormItem1,
			this.insertTableItem1,
			this.insertPictureItem1,
			this.insertBookmarkItem1,
			this.insertHyperlinkItem1,
			this.insertPageNumberItem1,
			this.insertPageCountItem1,
			this.insertSymbolItem1,
			this.insertPageNumberItem2,
			this.insertPageCountItem2,
			this.insertDateItem1,
			this.insertUserItem1});
			this.barManager1.MaxItemId = 66;
			this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemFontEdit1,
			this.repositoryItemRichEditFontSizeEdit1,
			this.repositoryItemRichEditStyleEdit1});
			this.fontBar1.Control = this.textBox;
			this.fontBar1.DockCol = 0;
			this.fontBar1.DockRow = 0;
			this.fontBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.fontBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.changeFontNameItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeFontSizeItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.fontSizeIncreaseItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.fontSizeDecreaseItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontBoldItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontItalicItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontUnderlineItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontDoubleUnderlineItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontStrikeoutItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontDoubleStrikeoutItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeFontColorItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeFontBackColorItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.clearFormattingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.showFontFormItem1)});
			this.changeFontNameItem1.Edit = this.repositoryItemFontEdit1;
			this.changeFontNameItem1.Id = 4;
			this.changeFontNameItem1.Name = "changeFontNameItem1";
			this.repositoryItemFontEdit1.AutoHeight = false;
			this.repositoryItemFontEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemFontEdit1.Name = "repositoryItemFontEdit1";
			this.changeFontSizeItem1.Edit = this.repositoryItemRichEditFontSizeEdit1;
			this.changeFontSizeItem1.Id = 5;
			this.changeFontSizeItem1.Name = "changeFontSizeItem1";
			this.repositoryItemRichEditFontSizeEdit1.AutoHeight = false;
			this.repositoryItemRichEditFontSizeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemRichEditFontSizeEdit1.Control = this.textBox;
			this.repositoryItemRichEditFontSizeEdit1.Name = "repositoryItemRichEditFontSizeEdit1";
			this.fontSizeIncreaseItem1.Id = 6;
			this.fontSizeIncreaseItem1.Name = "fontSizeIncreaseItem1";
			this.fontSizeDecreaseItem1.Id = 7;
			this.fontSizeDecreaseItem1.Name = "fontSizeDecreaseItem1";
			this.toggleFontBoldItem1.Id = 8;
			this.toggleFontBoldItem1.Name = "toggleFontBoldItem1";
			this.toggleFontItalicItem1.Id = 9;
			this.toggleFontItalicItem1.Name = "toggleFontItalicItem1";
			this.toggleFontUnderlineItem1.Id = 10;
			this.toggleFontUnderlineItem1.Name = "toggleFontUnderlineItem1";
			this.toggleFontDoubleUnderlineItem1.Id = 11;
			this.toggleFontDoubleUnderlineItem1.Name = "toggleFontDoubleUnderlineItem1";
			this.toggleFontStrikeoutItem1.Id = 12;
			this.toggleFontStrikeoutItem1.Name = "toggleFontStrikeoutItem1";
			this.toggleFontDoubleStrikeoutItem1.Id = 13;
			this.toggleFontDoubleStrikeoutItem1.Name = "toggleFontDoubleStrikeoutItem1";
			this.changeFontColorItem1.Id = 16;
			this.changeFontColorItem1.Name = "changeFontColorItem1";
			this.changeFontBackColorItem1.Id = 17;
			this.changeFontBackColorItem1.Name = "changeFontBackColorItem1";
			this.clearFormattingItem1.Id = 23;
			this.clearFormattingItem1.Name = "clearFormattingItem1";
			this.showFontFormItem1.Id = 24;
			this.showFontFormItem1.Name = "showFontFormItem1";
			this.paragraphBar1.Control = this.textBox;
			this.paragraphBar1.DockCol = 2;
			this.paragraphBar1.DockRow = 1;
			this.paragraphBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.paragraphBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleBulletedListItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleNumberingListItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleMultiLevelListItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.decreaseIndentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.increaseIndentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentLeftItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentCenterItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentRightItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentJustifyItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleShowWhitespaceItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeParagraphLineSpacingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeParagraphBackColorItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.showParagraphFormItem1)});
			this.toggleBulletedListItem1.Id = 25;
			this.toggleBulletedListItem1.Name = "toggleBulletedListItem1";
			this.toggleNumberingListItem1.Id = 26;
			this.toggleNumberingListItem1.Name = "toggleNumberingListItem1";
			this.toggleMultiLevelListItem1.Id = 27;
			this.toggleMultiLevelListItem1.Name = "toggleMultiLevelListItem1";
			this.decreaseIndentItem1.Id = 28;
			this.decreaseIndentItem1.Name = "decreaseIndentItem1";
			this.increaseIndentItem1.Id = 29;
			this.increaseIndentItem1.Name = "increaseIndentItem1";
			this.toggleParagraphAlignmentLeftItem1.Id = 30;
			this.toggleParagraphAlignmentLeftItem1.Name = "toggleParagraphAlignmentLeftItem1";
			this.toggleParagraphAlignmentCenterItem1.Id = 31;
			this.toggleParagraphAlignmentCenterItem1.Name = "toggleParagraphAlignmentCenterItem1";
			this.toggleParagraphAlignmentRightItem1.Id = 32;
			this.toggleParagraphAlignmentRightItem1.Name = "toggleParagraphAlignmentRightItem1";
			this.toggleParagraphAlignmentJustifyItem1.Id = 33;
			this.toggleParagraphAlignmentJustifyItem1.Name = "toggleParagraphAlignmentJustifyItem1";
			this.toggleShowWhitespaceItem1.Id = 34;
			this.toggleShowWhitespaceItem1.Name = "toggleShowWhitespaceItem1";
			this.changeParagraphLineSpacingItem1.Id = 35;
			this.changeParagraphLineSpacingItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.setSingleParagraphSpacingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setSesquialteralParagraphSpacingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setDoubleParagraphSpacingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.showLineSpacingFormItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.addSpacingBeforeParagraphItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeSpacingBeforeParagraphItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.addSpacingAfterParagraphItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeSpacingAfterParagraphItem1)});
			this.changeParagraphLineSpacingItem1.Name = "changeParagraphLineSpacingItem1";
			this.setSingleParagraphSpacingItem1.Id = 36;
			this.setSingleParagraphSpacingItem1.Name = "setSingleParagraphSpacingItem1";
			this.setSesquialteralParagraphSpacingItem1.Id = 37;
			this.setSesquialteralParagraphSpacingItem1.Name = "setSesquialteralParagraphSpacingItem1";
			this.setDoubleParagraphSpacingItem1.Id = 38;
			this.setDoubleParagraphSpacingItem1.Name = "setDoubleParagraphSpacingItem1";
			this.showLineSpacingFormItem1.Id = 39;
			this.showLineSpacingFormItem1.Name = "showLineSpacingFormItem1";
			this.addSpacingBeforeParagraphItem1.Id = 40;
			this.addSpacingBeforeParagraphItem1.Name = "addSpacingBeforeParagraphItem1";
			this.removeSpacingBeforeParagraphItem1.Id = 41;
			this.removeSpacingBeforeParagraphItem1.Name = "removeSpacingBeforeParagraphItem1";
			this.addSpacingAfterParagraphItem1.Id = 42;
			this.addSpacingAfterParagraphItem1.Name = "addSpacingAfterParagraphItem1";
			this.removeSpacingAfterParagraphItem1.Id = 43;
			this.removeSpacingAfterParagraphItem1.Name = "removeSpacingAfterParagraphItem1";
			this.changeParagraphBackColorItem1.Id = 44;
			this.changeParagraphBackColorItem1.Name = "changeParagraphBackColorItem1";
			this.showParagraphFormItem1.Id = 45;
			this.showParagraphFormItem1.Name = "showParagraphFormItem1";
			this.insertPictureItem1.Id = 54;
			this.insertPictureItem1.Name = "insertPictureItem1";
			this.insertTableItem1.Id = 56;
			this.insertTableItem1.Name = "insertTableItem1";
			this.headerFooterBar1.BarName = "";
			this.headerFooterBar1.Control = this.textBox;
			this.headerFooterBar1.DockCol = 1;
			this.headerFooterBar1.DockRow = 1;
			this.headerFooterBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.headerFooterBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.insertPictureItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertTableItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertPageNumberItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertPageCountItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertDateItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertUserItem1)});
			this.headerFooterBar1.Text = "";
			this.insertPageNumberItem2.Caption = "Page Number";
			this.insertPageNumberItem2.Glyph = global::DevExpress.XtraPrinting.Design.Properties.Resources.InsertPageNumber_16x16;
			this.insertPageNumberItem2.Id = 62;
			this.insertPageNumberItem2.LargeGlyph = global::DevExpress.XtraPrinting.Design.Properties.Resources.InsertPageNumber_32x32;
			this.insertPageNumberItem2.Name = "insertPageNumberItem2";
			toolTipTitleItem1.Text = "Page Number";
			toolTipItem1.LeftIndent = 6;
			toolTipItem1.Text = "Insert page numbers into the document.";
			superToolTip1.Items.Add(toolTipTitleItem1);
			superToolTip1.Items.Add(toolTipItem1);
			this.insertPageNumberItem2.SuperTip = superToolTip1;
			this.insertPageNumberItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.insertPageNumberItem2_ItemClick);
			this.insertPageCountItem2.Caption = "Page Count";
			this.insertPageCountItem2.Glyph = global::DevExpress.XtraPrinting.Design.Properties.Resources.InsertPageCount_16x16;
			this.insertPageCountItem2.Id = 63;
			this.insertPageCountItem2.LargeGlyph = global::DevExpress.XtraPrinting.Design.Properties.Resources.InsertPageCount_32x32;
			this.insertPageCountItem2.Name = "insertPageCountItem2";
			toolTipTitleItem2.Text = "Page Count";
			toolTipItem2.LeftIndent = 6;
			toolTipItem2.Text = "Insert total page count into the document.";
			superToolTip2.Items.Add(toolTipTitleItem2);
			superToolTip2.Items.Add(toolTipItem2);
			this.insertPageCountItem2.SuperTip = superToolTip2;
			this.insertPageCountItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.insertPageCountItem2_ItemClick);
			this.insertDateItem1.Caption = "Date";
			this.insertDateItem1.Glyph = ((System.Drawing.Image)(resources.GetObject("insertDateItem1.Glyph")));
			this.insertDateItem1.Id = 64;
			this.insertDateItem1.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("insertDateItem1.LargeGlyph")));
			this.insertDateItem1.Name = "insertDateItem1";
			toolTipTitleItem3.Text = "Date";
			toolTipItem3.LeftIndent = 6;
			toolTipItem3.Text = "Insert date printed into the document.";
			superToolTip3.Items.Add(toolTipTitleItem3);
			superToolTip3.Items.Add(toolTipItem3);
			this.insertDateItem1.SuperTip = superToolTip3;
			this.insertDateItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.insertDateItem1_ItemClick);
			this.insertUserItem1.Caption = "User Name";
			this.insertUserItem1.Glyph = ((System.Drawing.Image)(resources.GetObject("insertUserItem1.Glyph")));
			this.insertUserItem1.Id = 65;
			this.insertUserItem1.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("insertUserItem1.LargeGlyph")));
			this.insertUserItem1.Name = "insertUserItem1";
			toolTipTitleItem4.Text = "User Name";
			toolTipItem4.LeftIndent = 6;
			toolTipItem4.Text = "Insert user name into the document.";
			superToolTip4.Items.Add(toolTipTitleItem4);
			superToolTip4.Items.Add(toolTipItem4);
			this.insertUserItem1.SuperTip = superToolTip4;
			this.insertUserItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.insertUserItem1_ItemClick);
			this.barDockControlTop.CausesValidation = false;
			this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
			this.barDockControlTop.Size = new System.Drawing.Size(898, 62);
			this.barDockControlBottom.CausesValidation = false;
			this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControlBottom.Location = new System.Drawing.Point(0, 617);
			this.barDockControlBottom.Size = new System.Drawing.Size(898, 0);
			this.barDockControlLeft.CausesValidation = false;
			this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControlLeft.Location = new System.Drawing.Point(0, 62);
			this.barDockControlLeft.Size = new System.Drawing.Size(0, 555);
			this.barDockControlRight.CausesValidation = false;
			this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControlRight.Location = new System.Drawing.Point(898, 62);
			this.barDockControlRight.Size = new System.Drawing.Size(0, 555);
			this.insertBookmarkItem1.Id = 59;
			this.insertBookmarkItem1.Name = "insertBookmarkItem1";
			this.insertHyperlinkItem1.Id = 60;
			this.insertHyperlinkItem1.Name = "insertHyperlinkItem1";
			this.insertPageNumberItem1.Id = 57;
			this.insertPageNumberItem1.Name = "insertPageNumberItem1";
			this.insertPageCountItem1.Id = 58;
			this.insertPageCountItem1.Name = "insertPageCountItem1";
			this.insertSymbolItem1.Id = 61;
			this.insertSymbolItem1.Name = "insertSymbolItem1";
			this.repositoryItemRichEditStyleEdit1.AutoHeight = false;
			this.repositoryItemRichEditStyleEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemRichEditStyleEdit1.Control = this.textBox;
			this.repositoryItemRichEditStyleEdit1.Name = "repositoryItemRichEditStyleEdit1";
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(707, 12);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(85, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(801, 12);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(85, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.Controls.Add(this.okButton);
			this.panel1.Controls.Add(this.cancelButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 570);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(898, 47);
			this.panel1.TabIndex = 2;
			this.richEditBarController1.BarItems.Add(this.changeFontNameItem1);
			this.richEditBarController1.BarItems.Add(this.changeFontSizeItem1);
			this.richEditBarController1.BarItems.Add(this.fontSizeIncreaseItem1);
			this.richEditBarController1.BarItems.Add(this.fontSizeDecreaseItem1);
			this.richEditBarController1.BarItems.Add(this.toggleFontBoldItem1);
			this.richEditBarController1.BarItems.Add(this.toggleFontItalicItem1);
			this.richEditBarController1.BarItems.Add(this.toggleFontUnderlineItem1);
			this.richEditBarController1.BarItems.Add(this.toggleFontDoubleUnderlineItem1);
			this.richEditBarController1.BarItems.Add(this.toggleFontStrikeoutItem1);
			this.richEditBarController1.BarItems.Add(this.toggleFontDoubleStrikeoutItem1);
			this.richEditBarController1.BarItems.Add(this.changeFontColorItem1);
			this.richEditBarController1.BarItems.Add(this.changeFontBackColorItem1);
			this.richEditBarController1.BarItems.Add(this.clearFormattingItem1);
			this.richEditBarController1.BarItems.Add(this.showFontFormItem1);
			this.richEditBarController1.BarItems.Add(this.toggleBulletedListItem1);
			this.richEditBarController1.BarItems.Add(this.toggleNumberingListItem1);
			this.richEditBarController1.BarItems.Add(this.toggleMultiLevelListItem1);
			this.richEditBarController1.BarItems.Add(this.decreaseIndentItem1);
			this.richEditBarController1.BarItems.Add(this.increaseIndentItem1);
			this.richEditBarController1.BarItems.Add(this.toggleParagraphAlignmentLeftItem1);
			this.richEditBarController1.BarItems.Add(this.toggleParagraphAlignmentCenterItem1);
			this.richEditBarController1.BarItems.Add(this.toggleParagraphAlignmentRightItem1);
			this.richEditBarController1.BarItems.Add(this.toggleParagraphAlignmentJustifyItem1);
			this.richEditBarController1.BarItems.Add(this.toggleShowWhitespaceItem1);
			this.richEditBarController1.BarItems.Add(this.changeParagraphLineSpacingItem1);
			this.richEditBarController1.BarItems.Add(this.setSingleParagraphSpacingItem1);
			this.richEditBarController1.BarItems.Add(this.setSesquialteralParagraphSpacingItem1);
			this.richEditBarController1.BarItems.Add(this.setDoubleParagraphSpacingItem1);
			this.richEditBarController1.BarItems.Add(this.showLineSpacingFormItem1);
			this.richEditBarController1.BarItems.Add(this.addSpacingBeforeParagraphItem1);
			this.richEditBarController1.BarItems.Add(this.removeSpacingBeforeParagraphItem1);
			this.richEditBarController1.BarItems.Add(this.addSpacingAfterParagraphItem1);
			this.richEditBarController1.BarItems.Add(this.removeSpacingAfterParagraphItem1);
			this.richEditBarController1.BarItems.Add(this.changeParagraphBackColorItem1);
			this.richEditBarController1.BarItems.Add(this.showParagraphFormItem1);
			this.richEditBarController1.BarItems.Add(this.insertTableItem1);
			this.richEditBarController1.BarItems.Add(this.insertPictureItem1);
			this.richEditBarController1.BarItems.Add(this.insertBookmarkItem1);
			this.richEditBarController1.BarItems.Add(this.insertHyperlinkItem1);
			this.richEditBarController1.BarItems.Add(this.insertPageNumberItem1);
			this.richEditBarController1.BarItems.Add(this.insertPageCountItem1);
			this.richEditBarController1.BarItems.Add(this.insertSymbolItem1);
			this.richEditBarController1.Control = this.textBox;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(898, 617);
			this.ControlBox = false;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(300, 200);
			this.Name = "RichTextEditorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Rich Text Editor";
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit1)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.richEditBarController1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraPrinting.Design.Forms.EditControl textBox;
		private DevExpress.XtraEditors.SimpleButton okButton;
		private DevExpress.XtraEditors.SimpleButton cancelButton;
		private XtraBars.BarManager barManager1;
		private XtraRichEdit.UI.FontBar fontBar1;
		private XtraRichEdit.UI.ChangeFontNameItem changeFontNameItem1;
		private XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit1;
		private XtraRichEdit.UI.ChangeFontSizeItem changeFontSizeItem1;
		private XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit1;
		private XtraRichEdit.UI.FontSizeIncreaseItem fontSizeIncreaseItem1;
		private XtraRichEdit.UI.FontSizeDecreaseItem fontSizeDecreaseItem1;
		private XtraRichEdit.UI.ToggleFontBoldItem toggleFontBoldItem1;
		private XtraRichEdit.UI.ToggleFontItalicItem toggleFontItalicItem1;
		private XtraRichEdit.UI.ToggleFontUnderlineItem toggleFontUnderlineItem1;
		private XtraRichEdit.UI.ToggleFontDoubleUnderlineItem toggleFontDoubleUnderlineItem1;
		private XtraRichEdit.UI.ToggleFontStrikeoutItem toggleFontStrikeoutItem1;
		private XtraRichEdit.UI.ToggleFontDoubleStrikeoutItem toggleFontDoubleStrikeoutItem1;
		private XtraRichEdit.UI.ChangeFontColorItem changeFontColorItem1;
		private XtraRichEdit.UI.ChangeFontBackColorItem changeFontBackColorItem1;
		private XtraRichEdit.UI.ClearFormattingItem clearFormattingItem1;
		private XtraRichEdit.UI.ShowFontFormItem showFontFormItem1;
		private XtraRichEdit.UI.ParagraphBar paragraphBar1;
		private XtraRichEdit.UI.ToggleBulletedListItem toggleBulletedListItem1;
		private XtraRichEdit.UI.ToggleNumberingListItem toggleNumberingListItem1;
		private XtraRichEdit.UI.ToggleMultiLevelListItem toggleMultiLevelListItem1;
		private XtraRichEdit.UI.DecreaseIndentItem decreaseIndentItem1;
		private XtraRichEdit.UI.IncreaseIndentItem increaseIndentItem1;
		private XtraRichEdit.UI.ToggleParagraphAlignmentLeftItem toggleParagraphAlignmentLeftItem1;
		private XtraRichEdit.UI.ToggleParagraphAlignmentCenterItem toggleParagraphAlignmentCenterItem1;
		private XtraRichEdit.UI.ToggleParagraphAlignmentRightItem toggleParagraphAlignmentRightItem1;
		private XtraRichEdit.UI.ToggleParagraphAlignmentJustifyItem toggleParagraphAlignmentJustifyItem1;
		private XtraRichEdit.UI.ToggleShowWhitespaceItem toggleShowWhitespaceItem1;
		private XtraRichEdit.UI.ChangeParagraphLineSpacingItem changeParagraphLineSpacingItem1;
		private XtraRichEdit.UI.SetSingleParagraphSpacingItem setSingleParagraphSpacingItem1;
		private XtraRichEdit.UI.SetSesquialteralParagraphSpacingItem setSesquialteralParagraphSpacingItem1;
		private XtraRichEdit.UI.SetDoubleParagraphSpacingItem setDoubleParagraphSpacingItem1;
		private XtraRichEdit.UI.ShowLineSpacingFormItem showLineSpacingFormItem1;
		private XtraRichEdit.UI.AddSpacingBeforeParagraphItem addSpacingBeforeParagraphItem1;
		private XtraRichEdit.UI.RemoveSpacingBeforeParagraphItem removeSpacingBeforeParagraphItem1;
		private XtraRichEdit.UI.AddSpacingAfterParagraphItem addSpacingAfterParagraphItem1;
		private XtraRichEdit.UI.RemoveSpacingAfterParagraphItem removeSpacingAfterParagraphItem1;
		private XtraRichEdit.UI.ChangeParagraphBackColorItem changeParagraphBackColorItem1;
		private XtraRichEdit.UI.ShowParagraphFormItem showParagraphFormItem1;
		private XtraRichEdit.Design.RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit1;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
		private System.Windows.Forms.Panel panel1;
		private XtraRichEdit.UI.RichEditBarController richEditBarController1;
		private XtraRichEdit.UI.InsertPictureItem insertPictureItem1;
		private XtraRichEdit.UI.InsertTableItem insertTableItem1;
		private XtraRichEdit.UI.HeaderFooterBar headerFooterBar1;
		private XtraRichEdit.UI.InsertPageNumberItem insertPageNumberItem1;
		private XtraRichEdit.UI.InsertPageCountItem insertPageCountItem1;
		private XtraRichEdit.UI.InsertBookmarkItem insertBookmarkItem1;
		private XtraRichEdit.UI.InsertHyperlinkItem insertHyperlinkItem1;
		private XtraRichEdit.UI.InsertSymbolItem insertSymbolItem1;
		private XtraBars.BarButtonItem insertPageNumberItem2;
		private XtraBars.BarButtonItem insertPageCountItem2;
		private XtraBars.BarButtonItem insertDateItem1;
		private XtraBars.BarButtonItem insertUserItem1;
	}
}
