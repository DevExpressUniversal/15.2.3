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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraBars;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.edtName")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.cbParent")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.cbNextStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.previewRichEditControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barManager1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barDockControlTop")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barDockControlBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barDockControlLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barDockControlRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemFontEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditFontSizeEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.standaloneBarDockControl2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditStyleEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.standaloneBarDockControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.toggleFontBoldItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemComboBox1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.toggleFontItalicItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.toggleFontUnderlineItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.toggleParagraphAlignmentLeftItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.toggleParagraphAlignmentCenterItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.toggleParagraphAlignmentRightItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.toggleParagraphAlignmentJustifyItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.changeParagraphLineSingleSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.changeParagraphLineDoubleSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.changeParagraphLineSingleHalfSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.spacingIncreaseItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.spacingDecreaseItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.indentDecreaseItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.indentIncreaseItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.fontEditItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemFontEdit2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemColorEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditFontSizeEdit2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditFontSizeEdit3")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditFontSizeEdit4")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemCheckEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.fontSizeEditItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditFontSizeEdit5")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.lblProperties")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.lblFormatting")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.lblSeparator")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemFontEdit3")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditFontSizeEdit6")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.colorEditItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditColorEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemFontEdit4")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditFontSizeEdit7")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditStyleEdit2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.btnFormat")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.lblStyleForFollowingParagraph")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.lblStyleBasedOn")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.lblName")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemFontEdit5")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditFontSizeEdit8")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditStyleEdit3")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.popupMenuFormat")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barFontButtonItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barParagraphButtonItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barTabsButtonItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barLineSpacingSubItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.changeParagraphLineSingleSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.changeParagraphLineSingleHalfSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.changeParagraphLineDoubleSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barFontFormatting")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.barParagraphFormatting")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditStyleEdit4")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.repositoryItemRichEditStyleEdit5")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.cbCurrentStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.lblCurrentStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.EditStyleForm.lblSelectedStyle")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class EditStyleForm : XtraForm {
		EditStyleFormController controller;
		private EditStyleForm() {
			InitializeComponent();
		}
		public EditStyleForm(EditStyleFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			Controller.FillTempRichEdit(previewRichEditControl);
			InitializeForm();
			SubscribeControlsEvents();
			UpdateForm();
		}
		#region Properties
		protected internal EditStyleFormController Controller { get { return controller; } }
		protected internal ParagraphStyle IntermediateParagraphStyle { get { return Controller.IntermediateParagraphStyle; } }
		protected internal CharacterStyle IntermediateCharacterStyle { get { return Controller.IntermediateCharacterStyle; } }
		protected internal bool IsParagraphStyle { get { return Controller.IsParagraphStyle; } }
		protected internal ParagraphProperties ParagraphProperties { get { return IntermediateParagraphStyle.ParagraphProperties; } }
		protected internal CharacterProperties CharacterProperties { get { return Controller.CharacterProperties; } }
		protected internal CharacterStyle CharacterStyleParent {
			get { return (cbParent.SelectedIndex != 0) ? ((CharacterStyle)cbParent.SelectedItem) : null; }
		}
		#endregion
		protected internal virtual EditStyleFormController CreateController(EditStyleFormControllerParameters controllerParameters) {
			return new EditStyleFormController(previewRichEditControl, controllerParameters);
		}
		void ButtonsEnabled(bool value) {
			cbNextStyle.Enabled = value;
			barParagraphButtonItem1.Enabled = value;
			barTabsButtonItem1.Enabled = value;
		}
		protected internal void InitializeForm() {
			Controller.ChangePreviewControlCurrentStyle(previewRichEditControl);
			edtName.Text = Controller.StyleName;
			FillStyleCombo(cbCurrentStyle);
			if (IsParagraphStyle) {
				ButtonsEnabled(true);
				ParagraphStyleCollection styleCollection = Controller.ParagraphSourceStyle.DocumentModel.ParagraphStyles;
				FillStyleCombo<ParagraphStyle>(cbParent, styleCollection, IsParagraphParentValid);
				FillStyleCombo<ParagraphStyle>(cbNextStyle, styleCollection, IsNextValid);
			}
			else {
				cbParent.Properties.Items.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_EmptyParentStyle));
				FillStyleCombo<CharacterStyle>(cbParent, Controller.CharacterSourceStyle.DocumentModel.CharacterStyles, IsCharacterParentValid);
				ButtonsEnabled(false);
			}
			UpdateRichEditBars();
			ApplyPicturesToBarItems();
		}
		private bool IsParagraphParentValid(ParagraphStyle style) {
			return Controller.ParagraphSourceStyle.IsParentValid(style);
		}
		private bool IsCharacterParentValid(CharacterStyle style) {
			return Controller.CharacterSourceStyle.IsParentValid(style);
		}
		private bool IsNextValid(ParagraphStyle style) {
			return true;
		}
		void FillCurrentStyleCombo<T>(ComboBoxEdit comboBoxEdit, StyleCollectionBase<T> styles) where T : StyleBase<T> {
			int styleCount = styles.Count;
			for (int i = 0; i < styleCount; i++)
				comboBoxEdit.Properties.Items.Add(styles[i]);
		}
		protected internal virtual void FillStyleCombo(ComboBoxEdit comboBoxEdit) {
			ComboBoxItemCollection collection = comboBoxEdit.Properties.Items;
			collection.BeginUpdate();
			try {
				DocumentModel model = Controller.Control.InnerControl.DocumentModel;
				FillCurrentStyleCombo<ParagraphStyle>(comboBoxEdit, model.ParagraphStyles);
				FillCurrentStyleCombo<CharacterStyle>(comboBoxEdit, model.CharacterStyles);
			}
			finally {
				collection.EndUpdate();
			}
		}
		protected internal virtual void FillStyleCombo<T>(ComboBoxEdit comboBoxEdit, StyleCollectionBase<T> styleCollection, Predicate<T> match) where T : StyleBase<T> {
			ComboBoxItemCollection collection = comboBoxEdit.Properties.Items;
			collection.BeginUpdate();
			try {
				int count = styleCollection.Count;
				for (int i = 0; i < count; i++)
					if (match(styleCollection[i]))
						collection.Add(styleCollection[i]);
			}
			finally {
				collection.EndUpdate();
			}
		}
		protected internal void UpdateCharacterBars(CharacterFormattingInfo mergedCharacterProperties) {
			UnsubscribeToggleButtonsEvents();
			toggleFontBoldItem1.Checked = mergedCharacterProperties.FontBold;
			toggleFontItalicItem1.Checked = mergedCharacterProperties.FontItalic;
			toggleFontUnderlineItem1.Checked = (mergedCharacterProperties.FontUnderlineType == UnderlineType.Single);
			SubscribeToggleButtonsEvents();
			fontEditItem1.EditValue = mergedCharacterProperties.FontName;
			colorEditItem1.EditValue = mergedCharacterProperties.ForeColor;
			fontSizeEditItem1.EditValue = mergedCharacterProperties.DoubleFontSize/2f;
		}
		void BarEnabled(bool value) {
			toggleParagraphAlignmentLeftItem1.Enabled = value;
			toggleParagraphAlignmentCenterItem1.Enabled = value;
			toggleParagraphAlignmentRightItem1.Enabled = value;
			toggleParagraphAlignmentJustifyItem1.Enabled = value;
			barLineSpacingSubItem1.Enabled = value;
			spacingDecreaseItem1.Enabled = value;
			spacingIncreaseItem1.Enabled = value;
			indentDecreaseItem1.Enabled = value;
			indentIncreaseItem1.Enabled = value;
		}
		protected internal void UpdateRichEditBars() {
			CharacterFormattingInfo mergedCharacterProperties;
			if (Controller.IsParagraphStyle) {
				BarEnabled(true);
				mergedCharacterProperties = Controller.GetIntermediateMergedCharacterProperties(IntermediateParagraphStyle);
				UpdateCharacterBars(mergedCharacterProperties);
				ParagraphAlignment alignment = Controller.GetIntermediateMergedParagraphProperties(IntermediateParagraphStyle).Alignment;
				ParagraphLineSpacing spacing = ParagraphProperties.LineSpacingType;
				UnsubscribeParagraphAlignmentEvents();
				toggleParagraphAlignmentLeftItem1.Checked = (alignment == ParagraphAlignment.Left);
				toggleParagraphAlignmentCenterItem1.Checked = (alignment == ParagraphAlignment.Center);
				toggleParagraphAlignmentRightItem1.Checked = (alignment == ParagraphAlignment.Right);
				toggleParagraphAlignmentJustifyItem1.Checked = (alignment == ParagraphAlignment.Justify);
				SubscribeParagraphAlignmentEvents();
				UnsubscribeParagraphLineSpacingEvents();
				changeParagraphLineSingleSpacingItem1.Checked = (spacing == ParagraphLineSpacing.Single);
				changeParagraphLineSingleHalfSpacingItem1.Checked = (spacing == ParagraphLineSpacing.Sesquialteral);
				changeParagraphLineDoubleSpacingItem1.Checked = (spacing == ParagraphLineSpacing.Double);
				SubscribeParagraphLineSpacingEvents();
			}
			else {
				mergedCharacterProperties = Controller.GetIntermediateMergedCharacterProperties(IntermediateCharacterStyle);
				UpdateCharacterBars(mergedCharacterProperties);
				BarEnabled(false);
			}
		}	  
		protected internal void ApplyPicturesToBarItems() {
			toggleFontBoldItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("Bold");
			toggleFontItalicItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("Italic");
			toggleFontUnderlineItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("Underline");
			toggleParagraphAlignmentLeftItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignLeft");
			toggleParagraphAlignmentCenterItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignCenter");
			toggleParagraphAlignmentRightItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignRight");
			toggleParagraphAlignmentJustifyItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignJustify");
			spacingIncreaseItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("LineSpacing");
			spacingDecreaseItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("SpacingDecrease");
			indentDecreaseItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("IndentDecrease");
			indentIncreaseItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("IndentIncrease");
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
			cbCurrentStyle.SelectedIndexChanged += cbCurrentStyle_SelectedIndexChanged;
			edtName.EditValueChanged += OnStyleNameEditValueChanged;
			cbParent.SelectedIndexChanged += OnParentStyleSelectedIndexChanged;
			if (IsParagraphStyle)
				cbNextStyle.SelectedIndexChanged += OnNextStyleSelectedIndexChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			cbCurrentStyle.SelectedIndexChanged -= cbCurrentStyle_SelectedIndexChanged;
			edtName.EditValueChanged -= OnStyleNameEditValueChanged;
			cbParent.SelectedIndexChanged -= OnParentStyleSelectedIndexChanged;
			if (IsParagraphStyle)
				cbNextStyle.SelectedIndexChanged -= OnNextStyleSelectedIndexChanged;
		}
		protected internal virtual void UpdateFormCore() {
			edtName.Text = Controller.StyleName;
			if (IsParagraphStyle) {
				cbCurrentStyle.SelectedItem = Controller.ParagraphSourceStyle;
				cbParent.SelectedItem = Controller.ParagraphSourceStyle.Parent;
				cbNextStyle.SelectedItem = Controller.ParagraphSourceStyle.NextParagraphStyle;
			}
			else {
				cbCurrentStyle.SelectedItem = Controller.CharacterSourceStyle;
				if (Controller.CharacterSourceStyle.Parent == null)
					cbParent.SelectedIndex = 0;
				else
					cbParent.SelectedItem = Controller.CharacterSourceStyle.Parent;
			}
		}
		protected internal virtual void OnStyleNameEditValueChanged(object sender, EventArgs e) {
			Controller.StyleName = edtName.Text;
		}
		protected internal virtual void OnParentStyleSelectedIndexChanged(object sender, EventArgs e) {
			if (IsParagraphStyle)
				Controller.IntermediateParagraphStyle.Parent = (ParagraphStyle)cbParent.SelectedItem;
			else
				Controller.IntermediateCharacterStyle.Parent = CharacterStyleParent;
			UpdateRichEditBars();
		}
		protected internal virtual void OnNextStyleSelectedIndexChanged(object sender, EventArgs e) {
			Controller.IntermediateParagraphStyle.NextParagraphStyle = (ParagraphStyle)cbNextStyle.SelectedItem;
		}
		private void btnFormat_Click(object sender, EventArgs e) {
			btnFormat.ShowDropDown();
		}
		protected internal void barFontButtonItem1_ItemClick(object sender, ItemClickEventArgs e) {
			IRichEditControl control = Controller.Control;
			MergedCharacterProperties mergedProperties = IsParagraphStyle ? Controller.IntermediateParagraphStyle.GetMergedWithDefaultCharacterProperties() : Controller.IntermediateCharacterStyle.GetMergedWithDefaultCharacterProperties();
			control.ShowFontForm(mergedProperties, ApplyCharacterProperties, null);
		}
		void ApplyCharacterProperties(MergedCharacterProperties properties, object data) {
			Controller.CopyCharacterPropertiesFromMerged(properties);
			UpdateRichEditBars();
		}
		void OnOkClick(object sender, EventArgs e) {
			if (Controller.IsValidName(edtName.Text)) {
				DialogResult = DialogResult.OK;
				Controller.ApplyChanges();
			}
			else
				XtraMessageBox.Show(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_ParagraphStyleNameAlreadyExists), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		protected internal void UnsubscribeToggleButtonsEvents() {
			toggleFontBoldItem1.CheckedChanged -= toggleFontBoldItem1_CheckedChanged;
			toggleFontItalicItem1.CheckedChanged -= toggleFontItalicItem1_CheckedChanged;
			toggleFontUnderlineItem1.CheckedChanged -= toggleFontUnderlineItem1_CheckedChanged;
		}
		protected internal void SubscribeToggleButtonsEvents() {
			toggleFontBoldItem1.CheckedChanged += toggleFontBoldItem1_CheckedChanged;
			toggleFontItalicItem1.CheckedChanged += toggleFontItalicItem1_CheckedChanged;
			toggleFontUnderlineItem1.CheckedChanged += toggleFontUnderlineItem1_CheckedChanged;
		}
		protected internal void toggleFontBoldItem1_CheckedChanged(object sender, ItemClickEventArgs e) {
			Controller.ChangeCurrentValue<bool>(new FontBoldAccessor(), toggleFontBoldItem1.Checked);
		}
		protected internal void toggleFontItalicItem1_CheckedChanged(object sender, ItemClickEventArgs e) {
			Controller.ChangeCurrentValue<bool>(new FontItalicAccessor(), toggleFontItalicItem1.Checked);
		}
		protected internal void toggleFontUnderlineItem1_CheckedChanged(object sender, ItemClickEventArgs e) {
			if (toggleFontUnderlineItem1.Checked)
				Controller.ChangeCurrentValue<UnderlineType>(new FontUnderlineTypeAccessor(), UnderlineType.Single);
			else
				Controller.ChangeCurrentValue<UnderlineType>(new FontUnderlineTypeAccessor(), UnderlineType.None);
		}
		protected internal void fontEditItem1_EditValueChanged(object sender, EventArgs e) {
			Controller.ChangeCurrentValue<string>(new FontNameAccessor(), (string)fontEditItem1.EditValue);
		}
		protected internal void colorEditItem1_EditValueChanged(object sender, EventArgs e) {
			Controller.ChangeCurrentValue<Color>(new ForeColorAccessor(), (Color)colorEditItem1.EditValue);
		}
		protected internal void fontSizeEditItem1_EditValueChanged(object sender, EventArgs e) {
			string text;
			int value;
			if (EditStyleHelper.IsFontSizeValid(fontSizeEditItem1.EditValue, out text, out value))
				Controller.ChangeCurrentValue<int>(new DoubleFontSizeAccessor(), value); 
			else {
				XtraMessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				fontSizeEditItem1.EditValue = CharacterProperties.DoubleFontSize / 2f;
			}
		}
		private void cbCurrentStyle_SelectedIndexChanged(object sender, EventArgs e) {
			cbCurrentStyle.SelectedIndexChanged -= cbCurrentStyle_SelectedIndexChanged;
			IStyle style = cbCurrentStyle.SelectedItem as IStyle;
			string styleName = style.StyleName;
			if (cbCurrentStyle.SelectedItem is ParagraphStyle)
				Controller.Parameters.ParagraphSourceStyle = controller.Model.ParagraphStyles.GetStyleByName(styleName);
			else
				Controller.Parameters.CharacterSourceStyle = controller.Model.CharacterStyles.GetStyleByName(styleName);
			this.controller = CreateController(Controller.Parameters);
			cbCurrentStyle.Properties.Items.Clear();
			cbParent.Properties.Items.Clear();
			cbNextStyle.Properties.Items.Clear();
			cbNextStyle.SelectedIndex = -1;
			InitializeForm();
			cbCurrentStyle.SelectedIndexChanged += cbCurrentStyle_SelectedIndexChanged;
			UpdateForm();
		}
		#region For Paragraph Style Only
		protected internal void barParagraphButtonItem1_ItemClick(object sender, ItemClickEventArgs e) {
			IRichEditControl control = Controller.Control;
			MergedParagraphProperties mergedProperties = Controller.IntermediateParagraphStyle.GetMergedWithDefaultParagraphProperties();
			control.ShowParagraphForm(mergedProperties, ApplyParagraphProperties, null);
		}
		void ApplyParagraphProperties(MergedParagraphProperties properties, object data) {
			Controller.CopyParagraphPropertiesFromMerged(properties);
			UpdateRichEditBars();
		}
		protected internal void barTabsButtonItem1_ItemClick(object sender, ItemClickEventArgs e) {
			IRichEditControl control = Controller.Control;
			TabFormattingInfo info = Controller.IntermediateParagraphStyle.GetTabs();
			DocumentModel model = control.InnerControl.DocumentModel;
			int defaultTabWidth = model.DocumentProperties.DefaultTabWidth;
			control.ShowTabsForm(info, defaultTabWidth, ApplyTabsProperties, null);
		}
		void ApplyTabsProperties(TabFormattingInfo tabInfo, int defaultTabWidth, object data) {
			Controller.IntermediateParagraphStyle.Tabs.SetTabs(tabInfo);
			UpdateRichEditBars();
		}
		protected internal void ChangeActiveItem(object item, ParagraphAlignment alignment) {
			UnsubscribeParagraphAlignmentEvents();
			UncheckeAllParagraphAligmentButtons();
			((BarCheckItem)item).Checked = true;
			ParagraphProperties.Alignment = alignment;
			SubscribeParagraphAlignmentEvents();
		}
		protected internal void ChangeActiveItem(object item, ParagraphLineSpacing spacing) {
			UnsubscribeParagraphLineSpacingEvents();
			UncheckedAllParagraphLineSpacingButtons();
			((BarCheckItem)item).Checked = true;
			ParagraphProperties.LineSpacingType = spacing;
			SubscribeParagraphLineSpacingEvents();
		}
		protected internal void UnsubscribeParagraphAlignmentEvents() {
			toggleParagraphAlignmentLeftItem1.CheckedChanged -= toggleParagraphAlignmentLeftItem1_CheckedChanged;
			toggleParagraphAlignmentCenterItem1.CheckedChanged -= toggleParagraphAlignmentCenterItem1_CheckedChanged;
			toggleParagraphAlignmentRightItem1.CheckedChanged -= toggleParagraphAlignmentRightItem1_CheckedChanged;
			toggleParagraphAlignmentJustifyItem1.CheckedChanged -= toggleParagraphAlignmentJustifyItem1_CheckedChanged;
		}
		protected internal void UncheckeAllParagraphAligmentButtons() {
			toggleParagraphAlignmentLeftItem1.Checked = false;
			toggleParagraphAlignmentCenterItem1.Checked = false;
			toggleParagraphAlignmentRightItem1.Checked = false;
			toggleParagraphAlignmentJustifyItem1.Checked = false;
		}
		protected internal void SubscribeParagraphAlignmentEvents() {
			toggleParagraphAlignmentLeftItem1.CheckedChanged += toggleParagraphAlignmentLeftItem1_CheckedChanged;
			toggleParagraphAlignmentCenterItem1.CheckedChanged += toggleParagraphAlignmentCenterItem1_CheckedChanged;
			toggleParagraphAlignmentRightItem1.CheckedChanged += toggleParagraphAlignmentRightItem1_CheckedChanged;
			toggleParagraphAlignmentJustifyItem1.CheckedChanged += toggleParagraphAlignmentJustifyItem1_CheckedChanged;
		}
		protected internal void toggleParagraphAlignmentLeftItem1_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem(sender, ParagraphAlignment.Left);
		}
		protected internal void toggleParagraphAlignmentCenterItem1_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem(sender, ParagraphAlignment.Center);
		}
		protected internal void toggleParagraphAlignmentRightItem1_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem(sender, ParagraphAlignment.Right);
		}
		protected internal void toggleParagraphAlignmentJustifyItem1_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem(sender, ParagraphAlignment.Justify);
		}
		protected internal void UnsubscribeParagraphLineSpacingEvents() {
			changeParagraphLineSingleSpacingItem1.CheckedChanged -= changeParagraphLineSingleSpacingItem1_CheckedChanged;
			changeParagraphLineSingleHalfSpacingItem1.CheckedChanged -= changeParagraphLineSingleHalfSpacingItem1_CheckedChanged;
			changeParagraphLineDoubleSpacingItem1.CheckedChanged -= changeParagraphLineDoubleSpacingItem1_CheckedChanged;
		}
		protected internal void UncheckedAllParagraphLineSpacingButtons() {
			changeParagraphLineSingleSpacingItem1.Checked = false;
			changeParagraphLineSingleHalfSpacingItem1.Checked = false;
			changeParagraphLineDoubleSpacingItem1.Checked = false;
		}
		protected internal void SubscribeParagraphLineSpacingEvents() {
			changeParagraphLineSingleSpacingItem1.CheckedChanged += changeParagraphLineSingleSpacingItem1_CheckedChanged;
			changeParagraphLineSingleHalfSpacingItem1.CheckedChanged += changeParagraphLineSingleHalfSpacingItem1_CheckedChanged;
			changeParagraphLineDoubleSpacingItem1.CheckedChanged += changeParagraphLineDoubleSpacingItem1_CheckedChanged;
		}
		protected internal void changeParagraphLineSingleSpacingItem1_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem(sender, ParagraphLineSpacing.Single);
		}
		protected internal void changeParagraphLineSingleHalfSpacingItem1_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem(sender, ParagraphLineSpacing.Sesquialteral);
		}
		protected internal void changeParagraphLineDoubleSpacingItem1_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem(sender, ParagraphLineSpacing.Double);
		}
		protected internal void spacingIncreaseItem1_ItemClick(object sender, ItemClickEventArgs e) {
			Controller.IncreaseSpacing();
		}
		protected internal void spacingDecreaseItem1_ItemClick(object sender, ItemClickEventArgs e) {
			Controller.DecreaseSpacing();
		}
		protected internal void indentDecreaseItem1_ItemClick(object sender, ItemClickEventArgs e) {
			Controller.DecreaseIndent();
		}
		protected internal void indentIncreaseItem1_ItemClick(object sender, ItemClickEventArgs e) {
			Controller.IncreaseIndent();
		}
		#endregion
	}
}
