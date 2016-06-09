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
	partial class TableStyleForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableStyleForm));
			DevExpress.XtraRichEdit.Model.BorderInfo borderInfo1 = new DevExpress.XtraRichEdit.Model.BorderInfo();
			this.edtName = new DevExpress.XtraEditors.TextEdit();
			this.cbParent = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbApplyTo = new DevExpress.XtraRichEdit.Design.ConditionalTypeEdit();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.previewRichEditControl = new DevExpress.XtraRichEdit.Native.PreviewRichEditControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.barFontFormatting = new DevExpress.XtraBars.Bar();
			this.fontEditItem1 = new DevExpress.XtraBars.BarEditItem();
			this.repositoryItemFontEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.fontSizeEditItem1 = new DevExpress.XtraBars.BarEditItem();
			this.repositoryItemRichEditFontSizeEdit5 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.colorEditItem1 = new DevExpress.XtraBars.BarEditItem();
			this.repositoryItemOfficeColorPickEdit3 = new DevExpress.Office.UI.RepositoryItemOfficeColorPickEdit();
			this.toggleFontBoldItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.toggleFontItalicItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.toggleFontUnderlineItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.standaloneBarDockControl1 = new DevExpress.XtraBars.StandaloneBarDockControl();
			this.barParagraphFormatting = new DevExpress.XtraBars.Bar();
			this.changeTableBorderLineStyleItem1 = new DevExpress.XtraRichEdit.UI.ChangeTableBorderLineStyleItem();
			this.repositoryItemBorderLineStyle4 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineStyle();
			this.changeTableBorderLineWeightItem1 = new DevExpress.XtraRichEdit.UI.ChangeTableBorderLineWeightItem();
			this.repositoryItemBorderLineWeight1 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineWeight();
			this.changeTableBorderColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeTableBorderColorItem();
			this.changeTableBordersItem1 = new DevExpress.XtraRichEdit.UI.ChangeTableBordersItem();
			this.toggleTableCellsBottomBorderItem1 = new DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsBottomBorderItem();
			this.toggleTableCellsTopBorderItem1 = new DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsTopBorderItem();
			this.toggleTableCellsLeftBorderItem1 = new DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsLeftBorderItem();
			this.toggleTableCellsRightBorderItem1 = new DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsRightBorderItem();
			this.resetTableCellsAllBordersItem1 = new DevExpress.XtraRichEdit.Native.ResetConditionalTableCellsAllBordersItem();
			this.toggleTableCellsAllBordersItem1 = new DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsAllBordersItem();
			this.toggleTableCellsOutsideBorderItem1 = new DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsOutsideBorderItem();
			this.toggleTableCellsInsideBorderItem1 = new DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsInsideBorderItem();
			this.toggleTableCellsInsideHorizontalBorderItem1 = new DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsInsideHorizontalBorderItem();
			this.toggleTableCellsInsideVerticalBorderItem1 = new DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsInsideVerticalBorderItem();
			this.changeTableCellsShadingItem1 = new DevExpress.XtraRichEdit.Native.ChangeConditionalStyleShadingItem();
			this.changeTableAlignmentItem = new DevExpress.XtraBars.BarSubItem();
			this.toggleTableCellsTopLeftAlignmentItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.toggleTableCellsMiddleLeftAlignmentItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.toggleTableCellsBottomLeftAlignmentItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.toggleTableCellsTopCenterAlignmentItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.toggleTableCellsMiddleCenterAlignmentItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.toggleTableCellsBottomCenterAlignmentItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.toggleTableCellsTopRightAlignmentItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.toggleTableCellsMiddleRightAlignmentItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.toggleTableCellsBottomRightAlignmentItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.standaloneBarDockControl2 = new DevExpress.XtraBars.StandaloneBarDockControl();
			this.barFontButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.barParagraphButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.barTabsButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.repositoryItemRichEditColorEdit1 = new DevExpress.Office.UI.RepositoryItemOfficeColorEdit();
			this.repositoryItemOfficeColorPickEdit1 = new DevExpress.Office.UI.RepositoryItemOfficeColorPickEdit();
			this.repositoryItemOfficeColorPickEdit2 = new DevExpress.Office.UI.RepositoryItemOfficeColorPickEdit();
			this.repositoryItemColorEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
			this.repositoryItemOfficeColorEdit1 = new DevExpress.Office.UI.RepositoryItemOfficeColorEdit();
			this.repositoryItemFontEdit6 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.repositoryItemColorPickEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
			this.repositoryItemColorPickEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
			this.repositoryItemColorEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
			this.repositoryItemBorderLineStyle3 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineStyle();
			this.repositoryItemBorderLineStyle2 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineStyle();
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
			this.repositoryItemFontEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.repositoryItemRichEditFontSizeEdit7 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.repositoryItemRichEditStyleEdit2 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
			this.repositoryItemFontEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.repositoryItemRichEditFontSizeEdit8 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
			this.repositoryItemRichEditStyleEdit3 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
			this.repositoryItemRichEditStyleEdit4 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
			this.repositoryItemRichEditStyleEdit5 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
			this.repositoryItemColorPickEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
			this.repositoryItemBorderLineStyle1 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineStyle();
			this.lblProperties = new DevExpress.XtraEditors.LabelControl();
			this.lblFormatting = new DevExpress.XtraEditors.LabelControl();
			this.lblSeparator = new DevExpress.XtraEditors.LabelControl();
			this.btnFormat = new DevExpress.XtraEditors.DropDownButton();
			this.popupMenuFormat = new DevExpress.XtraBars.PopupMenu(this.components);
			this.lblName = new DevExpress.XtraEditors.LabelControl();
			this.lblStyleBasedOn = new DevExpress.XtraEditors.LabelControl();
			this.lblApplyFormattingTo = new DevExpress.XtraEditors.LabelControl();
			this.cbCurrentStyle = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblCurrentStyle = new DevExpress.XtraEditors.LabelControl();
			this.lblSelectedStyle = new DevExpress.XtraEditors.LabelControl();
			this.richEditBarController1 = new DevExpress.XtraRichEdit.UI.RichEditBarController();
			((System.ComponentModel.ISupportInitialize)(this.edtName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbParent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbApplyTo.Properties)).BeginInit();
			this.previewRichEditControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemOfficeColorPickEdit3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineStyle4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineWeight1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditColorEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemOfficeColorPickEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemOfficeColorPickEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemOfficeColorEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorPickEdit3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorPickEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineStyle3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineStyle2)).BeginInit();
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
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorPickEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineStyle1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenuFormat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCurrentStyle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.richEditBarController1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.edtName, "edtName");
			this.edtName.Name = "edtName";
			resources.ApplyResources(this.cbParent, "cbParent");
			this.cbParent.Name = "cbParent";
			this.cbParent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbParent.Properties.Buttons"))))});
			this.cbParent.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.cbApplyTo, "cbApplyTo");
			this.cbApplyTo.Name = "cbApplyTo";
			this.cbApplyTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbApplyTo.Properties.Buttons"))))});
			this.cbApplyTo.SelectedIndexChanged += new System.EventHandler(this.OnApplyToSelectedIndexChanged);
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnOkButtonClick);
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.previewRichEditControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
			this.previewRichEditControl.Controller = null;
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
			this.previewRichEditControl.Views.SimpleView.Padding = new System.Windows.Forms.Padding(15);
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
			this.fontEditItem1,
			this.fontSizeEditItem1,
			this.barFontButtonItem1,
			this.barParagraphButtonItem1,
			this.barTabsButtonItem1,
			this.changeTableCellsShadingItem1,
			this.changeTableBorderLineStyleItem1,
			this.changeTableBorderColorItem1,
			this.changeTableBorderLineWeightItem1,
			this.changeTableBordersItem1,
			this.toggleTableCellsBottomBorderItem1,
			this.toggleTableCellsTopBorderItem1,
			this.toggleTableCellsLeftBorderItem1,
			this.toggleTableCellsRightBorderItem1,
			this.resetTableCellsAllBordersItem1,
			this.toggleTableCellsAllBordersItem1,
			this.toggleTableCellsOutsideBorderItem1,
			this.toggleTableCellsInsideBorderItem1,
			this.toggleTableCellsInsideHorizontalBorderItem1,
			this.toggleTableCellsInsideVerticalBorderItem1,
			this.changeTableAlignmentItem,
			this.toggleTableCellsTopLeftAlignmentItem1,
			this.toggleTableCellsMiddleLeftAlignmentItem1,
			this.toggleTableCellsBottomLeftAlignmentItem1,
			this.toggleTableCellsTopCenterAlignmentItem1,
			this.toggleTableCellsMiddleCenterAlignmentItem1,
			this.toggleTableCellsBottomCenterAlignmentItem1,
			this.toggleTableCellsTopRightAlignmentItem1,
			this.toggleTableCellsMiddleRightAlignmentItem1,
			this.toggleTableCellsBottomRightAlignmentItem1,
			this.colorEditItem1});
			this.barManager1.MaxItemId = 510;
			this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemRichEditColorEdit1,
			this.repositoryItemBorderLineWeight1,
			this.repositoryItemOfficeColorPickEdit1,
			this.repositoryItemOfficeColorPickEdit2,
			this.repositoryItemColorEdit3,
			this.repositoryItemOfficeColorEdit1,
			this.repositoryItemFontEdit6,
			this.repositoryItemColorPickEdit3,
			this.repositoryItemOfficeColorPickEdit3});
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
			this.fontEditItem1.EditValueChanged += new System.EventHandler(this.OnFontEditItemValueChanged);
			resources.ApplyResources(this.repositoryItemFontEdit2, "repositoryItemFontEdit2");
			this.repositoryItemFontEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemFontEdit2.Buttons"))))});
			this.repositoryItemFontEdit2.Name = "repositoryItemFontEdit2";
			this.fontSizeEditItem1.CausesValidation = true;
			this.fontSizeEditItem1.Edit = this.repositoryItemRichEditFontSizeEdit5;
			this.fontSizeEditItem1.Id = 74;
			this.fontSizeEditItem1.Name = "fontSizeEditItem1";
			this.fontSizeEditItem1.EditValueChanged += new System.EventHandler(this.OnSizeEditItemValueChanged);
			resources.ApplyResources(this.repositoryItemRichEditFontSizeEdit5, "repositoryItemRichEditFontSizeEdit5");
			this.repositoryItemRichEditFontSizeEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditFontSizeEdit5.Buttons"))))});
			this.repositoryItemRichEditFontSizeEdit5.Control = this.previewRichEditControl;
			this.repositoryItemRichEditFontSizeEdit5.Name = "repositoryItemRichEditFontSizeEdit5";
			resources.ApplyResources(this.colorEditItem1, "colorEditItem1");
			this.colorEditItem1.Edit = this.repositoryItemOfficeColorPickEdit3;
			this.colorEditItem1.Id = 509;
			this.colorEditItem1.Name = "colorEditItem1";
			resources.ApplyResources(this.repositoryItemOfficeColorPickEdit3, "repositoryItemOfficeColorPickEdit3");
			this.repositoryItemOfficeColorPickEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemOfficeColorPickEdit3.Buttons"))))});
			this.repositoryItemOfficeColorPickEdit3.Name = "repositoryItemOfficeColorPickEdit3";
			resources.ApplyResources(this.toggleFontBoldItem1, "toggleFontBoldItem1");
			this.toggleFontBoldItem1.Id = 49;
			this.toggleFontBoldItem1.Name = "toggleFontBoldItem1";
			this.toggleFontBoldItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.OnFontBoldItemCheckedChanged);
			resources.ApplyResources(this.toggleFontItalicItem1, "toggleFontItalicItem1");
			this.toggleFontItalicItem1.Id = 53;
			this.toggleFontItalicItem1.Name = "toggleFontItalicItem1";
			this.toggleFontItalicItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.OnFontItalicItemCheckedChanged);
			resources.ApplyResources(this.toggleFontUnderlineItem1, "toggleFontUnderlineItem1");
			this.toggleFontUnderlineItem1.Id = 54;
			this.toggleFontUnderlineItem1.Name = "toggleFontUnderlineItem1";
			this.toggleFontUnderlineItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.OnFontUnderlineItemCheckedChanged);
			this.standaloneBarDockControl1.CausesValidation = false;
			resources.ApplyResources(this.standaloneBarDockControl1, "standaloneBarDockControl1");
			this.standaloneBarDockControl1.Name = "standaloneBarDockControl1";
			this.barParagraphFormatting.BarName = "Paragraph";
			this.barParagraphFormatting.DockCol = 0;
			this.barParagraphFormatting.DockRow = 0;
			this.barParagraphFormatting.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
			this.barParagraphFormatting.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.changeTableBorderLineStyleItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeTableBorderLineWeightItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeTableBorderColorItem1),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.changeTableBordersItem1, DevExpress.XtraBars.BarItemPaintStyle.CaptionInMenu),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeTableCellsShadingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.changeTableAlignmentItem, DevExpress.XtraBars.BarItemPaintStyle.CaptionInMenu)});
			this.barParagraphFormatting.OptionsBar.AllowQuickCustomization = false;
			this.barParagraphFormatting.OptionsBar.DisableClose = true;
			this.barParagraphFormatting.OptionsBar.DisableCustomization = true;
			this.barParagraphFormatting.OptionsBar.DrawDragBorder = false;
			this.barParagraphFormatting.OptionsBar.Hidden = true;
			this.barParagraphFormatting.OptionsBar.UseWholeRow = true;
			this.barParagraphFormatting.StandaloneBarDockControl = this.standaloneBarDockControl2;
			resources.ApplyResources(this.barParagraphFormatting, "barParagraphFormatting");
			this.changeTableBorderLineStyleItem1.Edit = this.repositoryItemBorderLineStyle4;
			borderInfo1.Color = System.Drawing.Color.Black;
			borderInfo1.Frame = false;
			borderInfo1.Offset = 0;
			borderInfo1.Shadow = false;
			borderInfo1.Style = DevExpress.XtraRichEdit.Model.BorderLineStyle.Single;
			borderInfo1.Width = 10;
			this.changeTableBorderLineStyleItem1.EditValue = borderInfo1;
			this.changeTableBorderLineStyleItem1.Id = 385;
			this.changeTableBorderLineStyleItem1.Name = "changeTableBorderLineStyleItem1";
			resources.ApplyResources(this.repositoryItemBorderLineStyle4, "repositoryItemBorderLineStyle4");
			this.repositoryItemBorderLineStyle4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemBorderLineStyle4.Buttons"))))});
			this.repositoryItemBorderLineStyle4.Control = this.previewRichEditControl;
			this.repositoryItemBorderLineStyle4.Name = "repositoryItemBorderLineStyle4";
			this.changeTableBorderLineWeightItem1.Edit = this.repositoryItemBorderLineWeight1;
			this.changeTableBorderLineWeightItem1.EditValue = 20;
			this.changeTableBorderLineWeightItem1.Id = 388;
			this.changeTableBorderLineWeightItem1.Name = "changeTableBorderLineWeightItem1";
			resources.ApplyResources(this.repositoryItemBorderLineWeight1, "repositoryItemBorderLineWeight1");
			this.repositoryItemBorderLineWeight1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemBorderLineWeight1.Buttons"))))});
			this.repositoryItemBorderLineWeight1.Control = this.previewRichEditControl;
			this.repositoryItemBorderLineWeight1.Name = "repositoryItemBorderLineWeight1";
			this.changeTableBorderColorItem1.Id = 387;
			this.changeTableBorderColorItem1.Name = "changeTableBorderColorItem1";
			this.changeTableBordersItem1.Id = 421;
			this.changeTableBordersItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsBottomBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsTopBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsLeftBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsRightBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.resetTableCellsAllBordersItem1, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsAllBordersItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsOutsideBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsInsideBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsInsideHorizontalBorderItem1, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsInsideVerticalBorderItem1)});
			this.changeTableBordersItem1.Name = "changeTableBordersItem1";
			this.toggleTableCellsBottomBorderItem1.Id = 422;
			this.toggleTableCellsBottomBorderItem1.Name = "toggleTableCellsBottomBorderItem1";
			this.toggleTableCellsBottomBorderItem1.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsBorderItemPress);
			this.toggleTableCellsTopBorderItem1.Id = 423;
			this.toggleTableCellsTopBorderItem1.Name = "toggleTableCellsTopBorderItem1";
			this.toggleTableCellsTopBorderItem1.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsBorderItemPress);
			this.toggleTableCellsLeftBorderItem1.Id = 424;
			this.toggleTableCellsLeftBorderItem1.Name = "toggleTableCellsLeftBorderItem1";
			this.toggleTableCellsLeftBorderItem1.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsBorderItemPress);
			this.toggleTableCellsRightBorderItem1.Id = 425;
			this.toggleTableCellsRightBorderItem1.Name = "toggleTableCellsRightBorderItem1";
			this.toggleTableCellsRightBorderItem1.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsBorderItemPress);
			this.resetTableCellsAllBordersItem1.Id = 426;
			this.resetTableCellsAllBordersItem1.Name = "resetTableCellsAllBordersItem1";
			this.resetTableCellsAllBordersItem1.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsBorderItemPress);
			this.toggleTableCellsAllBordersItem1.Id = 427;
			this.toggleTableCellsAllBordersItem1.Name = "toggleTableCellsAllBordersItem1";
			this.toggleTableCellsAllBordersItem1.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsBorderItemPress);
			this.toggleTableCellsOutsideBorderItem1.Id = 428;
			this.toggleTableCellsOutsideBorderItem1.Name = "toggleTableCellsOutsideBorderItem1";
			this.toggleTableCellsOutsideBorderItem1.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsBorderItemPress);
			this.toggleTableCellsInsideBorderItem1.Id = 429;
			this.toggleTableCellsInsideBorderItem1.Name = "toggleTableCellsInsideBorderItem1";
			this.toggleTableCellsInsideBorderItem1.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsBorderItemPress);
			this.toggleTableCellsInsideHorizontalBorderItem1.Id = 430;
			this.toggleTableCellsInsideHorizontalBorderItem1.Name = "toggleTableCellsInsideHorizontalBorderItem1";
			this.toggleTableCellsInsideHorizontalBorderItem1.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsBorderItemPress);
			this.toggleTableCellsInsideVerticalBorderItem1.Id = 431;
			this.toggleTableCellsInsideVerticalBorderItem1.Name = "toggleTableCellsInsideVerticalBorderItem1";
			this.toggleTableCellsInsideVerticalBorderItem1.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsBorderItemPress);
			this.changeTableCellsShadingItem1.Id = 383;
			this.changeTableCellsShadingItem1.Name = "changeTableCellsShadingItem1";
			resources.ApplyResources(this.changeTableAlignmentItem, "changeTableAlignmentItem");
			this.changeTableAlignmentItem.Id = 466;
			this.changeTableAlignmentItem.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsTopLeftAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsMiddleLeftAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsBottomLeftAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsTopCenterAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsMiddleCenterAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsBottomCenterAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsTopRightAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsMiddleRightAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsBottomRightAlignmentItem1)});
			this.changeTableAlignmentItem.Name = "changeTableAlignmentItem";
			resources.ApplyResources(this.toggleTableCellsTopLeftAlignmentItem1, "toggleTableCellsTopLeftAlignmentItem1");
			this.toggleTableCellsTopLeftAlignmentItem1.Id = 468;
			this.toggleTableCellsTopLeftAlignmentItem1.Name = "toggleTableCellsTopLeftAlignmentItem1";
			this.toggleTableCellsTopLeftAlignmentItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsAlignmentItemClick);
			resources.ApplyResources(this.toggleTableCellsMiddleLeftAlignmentItem1, "toggleTableCellsMiddleLeftAlignmentItem1");
			this.toggleTableCellsMiddleLeftAlignmentItem1.Id = 501;
			this.toggleTableCellsMiddleLeftAlignmentItem1.Name = "toggleTableCellsMiddleLeftAlignmentItem1";
			this.toggleTableCellsMiddleLeftAlignmentItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsAlignmentItemClick);
			resources.ApplyResources(this.toggleTableCellsBottomLeftAlignmentItem1, "toggleTableCellsBottomLeftAlignmentItem1");
			this.toggleTableCellsBottomLeftAlignmentItem1.Id = 502;
			this.toggleTableCellsBottomLeftAlignmentItem1.Name = "toggleTableCellsBottomLeftAlignmentItem1";
			this.toggleTableCellsBottomLeftAlignmentItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsAlignmentItemClick);
			resources.ApplyResources(this.toggleTableCellsTopCenterAlignmentItem1, "toggleTableCellsTopCenterAlignmentItem1");
			this.toggleTableCellsTopCenterAlignmentItem1.Id = 503;
			this.toggleTableCellsTopCenterAlignmentItem1.Name = "toggleTableCellsTopCenterAlignmentItem1";
			this.toggleTableCellsTopCenterAlignmentItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsAlignmentItemClick);
			resources.ApplyResources(this.toggleTableCellsMiddleCenterAlignmentItem1, "toggleTableCellsMiddleCenterAlignmentItem1");
			this.toggleTableCellsMiddleCenterAlignmentItem1.Id = 504;
			this.toggleTableCellsMiddleCenterAlignmentItem1.Name = "toggleTableCellsMiddleCenterAlignmentItem1";
			this.toggleTableCellsMiddleCenterAlignmentItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsAlignmentItemClick);
			resources.ApplyResources(this.toggleTableCellsBottomCenterAlignmentItem1, "toggleTableCellsBottomCenterAlignmentItem1");
			this.toggleTableCellsBottomCenterAlignmentItem1.Id = 505;
			this.toggleTableCellsBottomCenterAlignmentItem1.Name = "toggleTableCellsBottomCenterAlignmentItem1";
			this.toggleTableCellsBottomCenterAlignmentItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsAlignmentItemClick);
			resources.ApplyResources(this.toggleTableCellsTopRightAlignmentItem1, "toggleTableCellsTopRightAlignmentItem1");
			this.toggleTableCellsTopRightAlignmentItem1.Id = 506;
			this.toggleTableCellsTopRightAlignmentItem1.Name = "toggleTableCellsTopRightAlignmentItem1";
			this.toggleTableCellsTopRightAlignmentItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsAlignmentItemClick);
			resources.ApplyResources(this.toggleTableCellsMiddleRightAlignmentItem1, "toggleTableCellsMiddleRightAlignmentItem1");
			this.toggleTableCellsMiddleRightAlignmentItem1.Id = 507;
			this.toggleTableCellsMiddleRightAlignmentItem1.Name = "toggleTableCellsMiddleRightAlignmentItem1";
			this.toggleTableCellsMiddleRightAlignmentItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsAlignmentItemClick);
			resources.ApplyResources(this.toggleTableCellsBottomRightAlignmentItem1, "toggleTableCellsBottomRightAlignmentItem1");
			this.toggleTableCellsBottomRightAlignmentItem1.Id = 508;
			this.toggleTableCellsBottomRightAlignmentItem1.Name = "toggleTableCellsBottomRightAlignmentItem1";
			this.toggleTableCellsBottomRightAlignmentItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTableCellsAlignmentItemClick);
			this.standaloneBarDockControl2.CausesValidation = false;
			resources.ApplyResources(this.standaloneBarDockControl2, "standaloneBarDockControl2");
			this.standaloneBarDockControl2.Name = "standaloneBarDockControl2";
			resources.ApplyResources(this.barFontButtonItem1, "barFontButtonItem1");
			this.barFontButtonItem1.Id = 210;
			this.barFontButtonItem1.Name = "barFontButtonItem1";
			this.barFontButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnFontButtonItemClick);
			resources.ApplyResources(this.barParagraphButtonItem1, "barParagraphButtonItem1");
			this.barParagraphButtonItem1.Id = 211;
			this.barParagraphButtonItem1.Name = "barParagraphButtonItem1";
			this.barParagraphButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnParagraphButtonItemClick);
			resources.ApplyResources(this.barTabsButtonItem1, "barTabsButtonItem1");
			this.barTabsButtonItem1.Id = 212;
			this.barTabsButtonItem1.Name = "barTabsButtonItem1";
			this.barTabsButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnTabsButtonItemClick);
			this.repositoryItemRichEditColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemRichEditColorEdit1.Buttons"))))});
			this.repositoryItemRichEditColorEdit1.Name = "repositoryItemRichEditColorEdit1";
			resources.ApplyResources(this.repositoryItemOfficeColorPickEdit1, "repositoryItemOfficeColorPickEdit1");
			this.repositoryItemOfficeColorPickEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemOfficeColorPickEdit1.Buttons"))))});
			this.repositoryItemOfficeColorPickEdit1.Name = "repositoryItemOfficeColorPickEdit1";
			resources.ApplyResources(this.repositoryItemOfficeColorPickEdit2, "repositoryItemOfficeColorPickEdit2");
			this.repositoryItemOfficeColorPickEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemOfficeColorPickEdit2.Buttons"))))});
			this.repositoryItemOfficeColorPickEdit2.Name = "repositoryItemOfficeColorPickEdit2";
			resources.ApplyResources(this.repositoryItemColorEdit3, "repositoryItemColorEdit3");
			this.repositoryItemColorEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemColorEdit3.Buttons"))))});
			this.repositoryItemColorEdit3.Name = "repositoryItemColorEdit3";
			resources.ApplyResources(this.repositoryItemOfficeColorEdit1, "repositoryItemOfficeColorEdit1");
			this.repositoryItemOfficeColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemOfficeColorEdit1.Buttons"))))});
			this.repositoryItemOfficeColorEdit1.Name = "repositoryItemOfficeColorEdit1";
			resources.ApplyResources(this.repositoryItemFontEdit6, "repositoryItemFontEdit6");
			this.repositoryItemFontEdit6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemFontEdit6.Buttons"))))});
			this.repositoryItemFontEdit6.Name = "repositoryItemFontEdit6";
			resources.ApplyResources(this.repositoryItemColorPickEdit3, "repositoryItemColorPickEdit3");
			this.repositoryItemColorPickEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemColorPickEdit3.Buttons"))))});
			this.repositoryItemColorPickEdit3.Name = "repositoryItemColorPickEdit3";
			resources.ApplyResources(this.repositoryItemColorPickEdit2, "repositoryItemColorPickEdit2");
			this.repositoryItemColorPickEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemColorPickEdit2.Buttons"))))});
			this.repositoryItemColorPickEdit2.Name = "repositoryItemColorPickEdit2";
			resources.ApplyResources(this.repositoryItemColorEdit2, "repositoryItemColorEdit2");
			this.repositoryItemColorEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemColorEdit2.Buttons"))))});
			this.repositoryItemColorEdit2.Name = "repositoryItemColorEdit2";
			resources.ApplyResources(this.repositoryItemBorderLineStyle3, "repositoryItemBorderLineStyle3");
			this.repositoryItemBorderLineStyle3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemBorderLineStyle3.Buttons"))))});
			this.repositoryItemBorderLineStyle3.Control = this.previewRichEditControl;
			this.repositoryItemBorderLineStyle3.Name = "repositoryItemBorderLineStyle3";
			resources.ApplyResources(this.repositoryItemBorderLineStyle2, "repositoryItemBorderLineStyle2");
			this.repositoryItemBorderLineStyle2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemBorderLineStyle2.Buttons"))))});
			this.repositoryItemBorderLineStyle2.Control = this.previewRichEditControl;
			this.repositoryItemBorderLineStyle2.Name = "repositoryItemBorderLineStyle2";
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
			resources.ApplyResources(this.repositoryItemColorPickEdit1, "repositoryItemColorPickEdit1");
			this.repositoryItemColorPickEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemColorPickEdit1.Buttons"))))});
			this.repositoryItemColorPickEdit1.Name = "repositoryItemColorPickEdit1";
			resources.ApplyResources(this.repositoryItemBorderLineStyle1, "repositoryItemBorderLineStyle1");
			this.repositoryItemBorderLineStyle1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemBorderLineStyle1.Buttons"))))});
			this.repositoryItemBorderLineStyle1.Control = this.previewRichEditControl;
			this.repositoryItemBorderLineStyle1.Name = "repositoryItemBorderLineStyle1";
			resources.ApplyResources(this.lblProperties, "lblProperties");
			this.lblProperties.LineVisible = true;
			this.lblProperties.Name = "lblProperties";
			resources.ApplyResources(this.lblFormatting, "lblFormatting");
			this.lblFormatting.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblFormatting.LineVisible = true;
			this.lblFormatting.Name = "lblFormatting";
			resources.ApplyResources(this.lblSeparator, "lblSeparator");
			this.lblSeparator.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblSeparator.LineVisible = true;
			this.lblSeparator.Name = "lblSeparator";
			this.btnFormat.DropDownControl = this.popupMenuFormat;
			resources.ApplyResources(this.btnFormat, "btnFormat");
			this.btnFormat.MenuManager = this.barManager1;
			this.btnFormat.Name = "btnFormat";
			this.btnFormat.Click += new System.EventHandler(this.OnButtonFormatClick);
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
			resources.ApplyResources(this.lblApplyFormattingTo, "lblApplyFormattingTo");
			this.lblApplyFormattingTo.Name = "lblApplyFormattingTo";
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
			this.richEditBarController1.BarItems.Add(this.changeTableCellsShadingItem1);
			this.richEditBarController1.BarItems.Add(this.changeTableBorderLineStyleItem1);
			this.richEditBarController1.BarItems.Add(this.changeTableBorderLineWeightItem1);
			this.richEditBarController1.BarItems.Add(this.changeTableBorderColorItem1);
			this.richEditBarController1.BarItems.Add(this.changeTableBordersItem1);
			this.richEditBarController1.BarItems.Add(this.toggleTableCellsBottomBorderItem1);
			this.richEditBarController1.BarItems.Add(this.toggleTableCellsTopBorderItem1);
			this.richEditBarController1.BarItems.Add(this.toggleTableCellsLeftBorderItem1);
			this.richEditBarController1.BarItems.Add(this.toggleTableCellsRightBorderItem1);
			this.richEditBarController1.BarItems.Add(this.resetTableCellsAllBordersItem1);
			this.richEditBarController1.BarItems.Add(this.toggleTableCellsAllBordersItem1);
			this.richEditBarController1.BarItems.Add(this.toggleTableCellsOutsideBorderItem1);
			this.richEditBarController1.BarItems.Add(this.toggleTableCellsInsideBorderItem1);
			this.richEditBarController1.BarItems.Add(this.toggleTableCellsInsideHorizontalBorderItem1);
			this.richEditBarController1.BarItems.Add(this.toggleTableCellsInsideVerticalBorderItem1);
			this.richEditBarController1.Control = this.previewRichEditControl;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblSelectedStyle);
			this.Controls.Add(this.lblCurrentStyle);
			this.Controls.Add(this.lblApplyFormattingTo);
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
			this.Controls.Add(this.cbApplyTo);
			this.Controls.Add(this.cbCurrentStyle);
			this.Controls.Add(this.cbParent);
			this.Controls.Add(this.edtName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TableStyleForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.edtName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbParent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbApplyTo.Properties)).EndInit();
			this.previewRichEditControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemOfficeColorPickEdit3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineStyle4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineWeight1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditColorEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemOfficeColorPickEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemOfficeColorPickEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemOfficeColorEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorPickEdit3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorPickEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineStyle3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineStyle2)).EndInit();
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
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorPickEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineStyle1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenuFormat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCurrentStyle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.richEditBarController1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.TextEdit edtName;
		protected DevExpress.XtraEditors.ComboBoxEdit cbParent;
		protected DevExpress.XtraRichEdit.Design.ConditionalTypeEdit cbApplyTo;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraRichEdit.Native.PreviewRichEditControl previewRichEditControl;
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
		protected DevExpress.XtraEditors.LabelControl lblApplyFormattingTo;
		protected DevExpress.XtraEditors.LabelControl lblStyleBasedOn;
		protected DevExpress.XtraEditors.LabelControl lblName;
		protected DevExpress.XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit5;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit8;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit3;
		protected DevExpress.XtraBars.PopupMenu popupMenuFormat;
		protected DevExpress.XtraBars.BarButtonItem barFontButtonItem1;
		protected DevExpress.XtraBars.BarButtonItem barParagraphButtonItem1;
		protected DevExpress.XtraBars.BarButtonItem barTabsButtonItem1;
		protected DevExpress.XtraBars.Bar barFontFormatting;
		protected DevExpress.XtraBars.Bar barParagraphFormatting;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit4;
		protected DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit5;
		protected XtraEditors.ComboBoxEdit cbCurrentStyle;
		protected XtraEditors.LabelControl lblCurrentStyle;
		protected XtraEditors.LabelControl lblSelectedStyle;
		private XtraEditors.Repository.RepositoryItemColorPickEdit repositoryItemColorPickEdit1;
		private Design.RepositoryItemBorderLineStyle repositoryItemBorderLineStyle1;
		private Design.RepositoryItemBorderLineStyle repositoryItemBorderLineStyle2;
		private XtraEditors.Repository.RepositoryItemColorEdit repositoryItemColorEdit2;
		private Design.RepositoryItemBorderLineStyle repositoryItemBorderLineStyle3;
		private XtraEditors.Repository.RepositoryItemColorPickEdit repositoryItemColorPickEdit2;
		private DevExpress.XtraRichEdit.Native.ChangeConditionalStyleShadingItem changeTableCellsShadingItem1;
		private UI.ChangeTableBorderLineStyleItem changeTableBorderLineStyleItem1;
		private Design.RepositoryItemBorderLineStyle repositoryItemBorderLineStyle4;
		private UI.ChangeTableBorderColorItem changeTableBorderColorItem1;
		private UI.RichEditBarController richEditBarController1;
		private UI.ChangeTableBorderLineWeightItem changeTableBorderLineWeightItem1;
		private Design.RepositoryItemBorderLineWeight repositoryItemBorderLineWeight1;
		private Office.UI.RepositoryItemOfficeColorPickEdit repositoryItemOfficeColorPickEdit2;
		private XtraEditors.Repository.RepositoryItemColorEdit repositoryItemColorEdit3;
		private Office.UI.RepositoryItemOfficeColorEdit repositoryItemOfficeColorEdit1;
		private XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit6;
		private XtraEditors.Repository.RepositoryItemColorPickEdit repositoryItemColorPickEdit3;
		private Office.UI.RepositoryItemOfficeColorPickEdit repositoryItemOfficeColorPickEdit1;
		private UI.ChangeTableBordersItem changeTableBordersItem1;
		private DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsTopBorderItem toggleTableCellsTopBorderItem1;
		private Native.ResetConditionalTableCellsAllBordersItem resetTableCellsAllBordersItem1;
		private Native.ToggleConditionalTableCellsOutsideBorderItem toggleTableCellsOutsideBorderItem1;
		private Native.ToggleConditionalTableCellsInsideBorderItem toggleTableCellsInsideBorderItem1;
		private Native.ToggleConditionalTableCellsInsideHorizontalBorderItem toggleTableCellsInsideHorizontalBorderItem1;
		private Native.ToggleConditionalTableCellsInsideVerticalBorderItem toggleTableCellsInsideVerticalBorderItem1;
		private XtraBars.BarSubItem changeTableAlignmentItem;
		private XtraBars.BarButtonItem toggleTableCellsTopLeftAlignmentItem1;
		private XtraBars.BarButtonItem toggleTableCellsMiddleLeftAlignmentItem1;
		private XtraBars.BarButtonItem toggleTableCellsBottomLeftAlignmentItem1;
		private XtraBars.BarButtonItem toggleTableCellsTopCenterAlignmentItem1;
		private XtraBars.BarButtonItem toggleTableCellsMiddleCenterAlignmentItem1;
		private XtraBars.BarButtonItem toggleTableCellsBottomCenterAlignmentItem1;
		private XtraBars.BarButtonItem toggleTableCellsTopRightAlignmentItem1;
		private XtraBars.BarButtonItem toggleTableCellsMiddleRightAlignmentItem1;
		private XtraBars.BarButtonItem toggleTableCellsBottomRightAlignmentItem1;
		private DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsBottomBorderItem toggleTableCellsBottomBorderItem1;
		private DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsLeftBorderItem toggleTableCellsLeftBorderItem1;
		private DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsRightBorderItem toggleTableCellsRightBorderItem1;
		private DevExpress.XtraRichEdit.Native.ToggleConditionalTableCellsAllBordersItem toggleTableCellsAllBordersItem1;
		private XtraBars.BarEditItem colorEditItem1;
		private Office.UI.RepositoryItemOfficeColorPickEdit repositoryItemOfficeColorPickEdit3;
	}
}
