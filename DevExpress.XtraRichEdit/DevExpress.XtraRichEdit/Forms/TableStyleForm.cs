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
using System.Collections.Generic;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.UI;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.Office.Internal;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.edtName")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.cbParent")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.cbApplyTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.previewRichEditControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barManager1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barDockControlTop")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barDockControlBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barDockControlLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barDockControlRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemFontEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditFontSizeEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.standaloneBarDockControl2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditStyleEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.standaloneBarDockControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.toggleFontBoldItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemComboBox1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.toggleFontItalicItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.toggleFontUnderlineItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.toggleParagraphAlignmentLeftItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.toggleParagraphAlignmentCenterItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.toggleParagraphAlignmentRightItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.toggleParagraphAlignmentJustifyItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.changeParagraphLineSingleSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.changeParagraphLineDoubleSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.changeParagraphLineSingleHalfSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.spacingIncreaseItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.spacingDecreaseItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.indentDecreaseItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.indentIncreaseItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.fontEditItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemFontEdit2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemColorEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditFontSizeEdit2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditFontSizeEdit3")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditFontSizeEdit4")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemCheckEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.fontSizeEditItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditFontSizeEdit5")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.lblProperties")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.lblFormatting")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.lblSeparator")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemFontEdit3")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditFontSizeEdit6")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.colorEditItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditColorEdit1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemFontEdit4")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditFontSizeEdit7")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditStyleEdit2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.btnFormat")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.lblApplyFormattingTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.lblStyleBasedOn")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.lblName")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemFontEdit5")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditFontSizeEdit8")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditStyleEdit3")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.popupMenuFormat")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barFontButtonItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barParagraphButtonItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barTabsButtonItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barLineSpacingSubItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.changeParagraphLineSingleSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.changeParagraphLineSingleHalfSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.changeParagraphLineDoubleSpacingItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barFontFormatting")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.barParagraphFormatting")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditStyleEdit4")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.repositoryItemRichEditStyleEdit5")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.cbCurrentStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.lblCurrentStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableStyleForm.lblSelectedStyle")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class TableStyleForm : XtraForm {
		TableStyleFormControllerBase controller;
		readonly Dictionary<string, KeyValuePair<VerticalAlignment, ParagraphAlignment>> alignments = new Dictionary<string, KeyValuePair<VerticalAlignment, ParagraphAlignment>>();
		protected TableStyleForm() {
			InitializeComponent();
		}
		public TableStyleForm(FormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			previewRichEditControl.Controller = controller;
			cbCurrentStyle.Enabled = ((TableStyleFormControllerParametersBase)controllerParameters).CurrentStyleEnabled;
			InitializeForm();
			SubscribeControlsEvents();
			UpdateForm();
			InitializeAlignmentSubItem();
		}
		#region Properties
		protected virtual TableStyleFormControllerBase Controller { get { return controller; } }
		protected internal TableStyle IntermediateTableStyle { get { return ((TableStyleFormController)Controller).IntermediateTableStyle; } }
		private ConditionalTableStyleFormattingTypes ConditionalStyleType { get { return Controller.ConditionalStyleType; } }
		protected internal TableConditionalStyle CurrentConditionalStyle { get { return Controller.ConditionalStyleProperties != null ? Controller.ConditionalStyleProperties[ConditionalStyleType] : null; } }
		private TableStyle SourceStyle { get { return ((TableStyleFormController)Controller).TableSourceStyle; } }
		protected CharacterProperties CharacterProperties { get { return Controller.CharacterProperties; } }
		private Dictionary<string, KeyValuePair<VerticalAlignment, ParagraphAlignment>> Alignments { get { return alignments; } }
		#endregion
		protected internal virtual TableStyleFormControllerBase CreateController(FormControllerParameters controllerParameters) {
			return new TableStyleFormController(previewRichEditControl, controllerParameters);
		}
		protected virtual void ChangeConditionalType() {
			Table currentTable = previewRichEditControl.DocumentModel.ActivePieceTable.Tables[0];
			EditStyleHelper.ChangeConditionalType(currentTable, ConditionalStyleType);
		}
		protected virtual void SetParentStyle() {
			IntermediateTableStyle.Parent = (cbParent.SelectedIndex != 0) ? (TableStyle)cbParent.SelectedItem : null;
		}
		protected virtual void FillCurrentStyleComboCore(ComboBoxEdit comboBoxEdit) {
			FillCurrentStyleCombo<TableStyle>(comboBoxEdit, SourceStyle.DocumentModel.TableStyles);
		}
		protected virtual void FillParentStyleCombo() {
			FillParentStyleCombo(cbParent, SourceStyle.DocumentModel.TableStyles);
		}
		protected virtual void InitializeConditionalStyleType() {
			((TableStyleFormController)Controller).ConditionalStyleType = ConditionalTableStyleFormattingTypes.WholeTable;
		}
		protected virtual void UpdateFormCore() {
			TableStyle parent = SourceStyle.Parent;
			cbCurrentStyle.SelectedItem = SourceStyle;
			if (parent == null)
				cbParent.SelectedIndex = 0;
			else
				cbParent.SelectedItem = parent;
		}
		protected void InitializeAlignmentDictionary() {
			alignments.Add(toggleTableCellsTopLeftAlignmentItem1.Name, new KeyValuePair<VerticalAlignment, ParagraphAlignment>(VerticalAlignment.Top, ParagraphAlignment.Left));
			alignments.Add(toggleTableCellsMiddleLeftAlignmentItem1.Name, new KeyValuePair<VerticalAlignment, ParagraphAlignment>(VerticalAlignment.Center, ParagraphAlignment.Left));
			alignments.Add(toggleTableCellsBottomLeftAlignmentItem1.Name, new KeyValuePair<VerticalAlignment, ParagraphAlignment>(VerticalAlignment.Bottom, ParagraphAlignment.Left));
			alignments.Add(toggleTableCellsTopCenterAlignmentItem1.Name, new KeyValuePair<VerticalAlignment, ParagraphAlignment>(VerticalAlignment.Top, ParagraphAlignment.Center));
			alignments.Add(toggleTableCellsMiddleCenterAlignmentItem1.Name, new KeyValuePair<VerticalAlignment, ParagraphAlignment>(VerticalAlignment.Center, ParagraphAlignment.Center));
			alignments.Add(toggleTableCellsBottomCenterAlignmentItem1.Name, new KeyValuePair<VerticalAlignment, ParagraphAlignment>(VerticalAlignment.Bottom, ParagraphAlignment.Center));
			alignments.Add(toggleTableCellsTopRightAlignmentItem1.Name, new KeyValuePair<VerticalAlignment, ParagraphAlignment>(VerticalAlignment.Top, ParagraphAlignment.Right));
			alignments.Add(toggleTableCellsMiddleRightAlignmentItem1.Name, new KeyValuePair<VerticalAlignment, ParagraphAlignment>(VerticalAlignment.Center, ParagraphAlignment.Right));
			alignments.Add(toggleTableCellsBottomRightAlignmentItem1.Name, new KeyValuePair<VerticalAlignment, ParagraphAlignment>(VerticalAlignment.Bottom, ParagraphAlignment.Right));
		}
		private void InitializeAlignmentSubItem() {
			InitializeAlignmentDictionary();
			string title = XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeTableCellAlignmentPlaceholder);
			string description = XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeTableCellAlignmentPlaceholderDescription);
			changeTableAlignmentItem.SuperTip = DevExpress.XtraBars.Commands.Internal.SuperToolTipHelper.CreateSuperToolTip(title, description, new BarShortcut());
		}
		protected internal void InitializeForm() {
			Controller.FillTempRichEdit(previewRichEditControl);
			InitializeConditionalStyleType();
			edtName.Text = Controller.StyleName;
			FillCurrentStyleCombo(cbCurrentStyle);
			cbParent.Properties.Items.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_EmptyParentStyle));
			FillParentStyleCombo();
			cbApplyTo.EditValue = ConditionalTableStyleFormattingTypes.WholeTable;
			UpdateRichEditBars();
			ApplyPicturesToBarItems();
		}
		protected void FillCurrentStyleCombo<T>(ComboBoxEdit comboBoxEdit, StyleCollectionBase<T> styles) where T : StyleBase<T> {
			int styleCount = styles.Count;
			for (int i = 0; i < styleCount; i++) {
				if (styles[i] != styles.DefaultItem)
					comboBoxEdit.Properties.Items.Add(styles[i]);
			}
		}
		protected internal virtual void FillCurrentStyleCombo(ComboBoxEdit comboBoxEdit) {
			ComboBoxItemCollection collection = comboBoxEdit.Properties.Items;
			collection.BeginUpdate();
			try {
				FillCurrentStyleComboCore(comboBoxEdit);
			}
			finally {
				collection.EndUpdate();
			}
		}
		protected internal virtual void FillParentStyleCombo(ComboBoxEdit comboBoxEdit, StyleCollectionBase<TableStyle> styles) {
			ComboBoxItemCollection collection = comboBoxEdit.Properties.Items;
			collection.BeginUpdate();
			try {
				int count = styles.Count;
				for (int i = 0; i < count; i++)
					if (SourceStyle.IsParentValid(styles[i]))
						collection.Add(styles[i]);
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
			UnsubscribeCharacterPropertiesEvents();
			fontEditItem1.EditValue = mergedCharacterProperties.FontName;
			colorEditItem1.EditValue = mergedCharacterProperties.ForeColor;
			fontSizeEditItem1.EditValue = mergedCharacterProperties.DoubleFontSize/2f;
			SubscribeCharacterPropertiesEvents();
		}
		protected internal void UpdateTableBars(CombinedCellPropertiesInfo mergedCharacterProperties) {
			BorderInfo borderInfo = (BorderInfo)(changeTableBorderLineStyleItem1.EditValue);
			borderInfo.Color = mergedCharacterProperties.Borders.LeftBorder.Color;
			borderInfo.Width = mergedCharacterProperties.Borders.LeftBorder.Width;
			borderInfo.Style = mergedCharacterProperties.Borders.LeftBorder.Style;
		}
		protected internal void UpdateRichEditBars() {
			UpdateCharacterBars(Controller.GetMergedWithDefaultCharacterProperties());
			UpdateTableBars(Controller.GetMergedTableCellProperties());
		}
		protected internal void ApplyPicturesToBarItems() {
			toggleFontBoldItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("Bold");
			toggleFontItalicItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("Italic");
			toggleFontUnderlineItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("Underline");
			changeTableAlignmentItem.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignTopLeft");
			toggleTableCellsTopLeftAlignmentItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignTopLeft");
			toggleTableCellsMiddleLeftAlignmentItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignMiddleLeft");
			toggleTableCellsBottomLeftAlignmentItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignBottomLeft");
			toggleTableCellsTopCenterAlignmentItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignTopCenter");
			toggleTableCellsMiddleCenterAlignmentItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignMiddleCenter");
			toggleTableCellsBottomCenterAlignmentItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignBottomCenter");
			toggleTableCellsTopRightAlignmentItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignTopRight");
			toggleTableCellsMiddleRightAlignmentItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignMiddleRight");
			toggleTableCellsBottomRightAlignmentItem1.Glyph = EditStyleHelper.LoadSmallImageToGlyph("AlignBottomRight");
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				edtName.Text = Controller.StyleName;
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void SubscribeControlsEvents() {
			cbCurrentStyle.SelectedIndexChanged += OnCurrentStyleSelectedIndexChanged;
			edtName.EditValueChanged += OnStyleNameEditValueChanged;
			cbParent.SelectedIndexChanged += OnParentStyleSelectedIndexChanged;
			changeTableCellsShadingItem1.ColorChanged += OnTableCellsShadingColorChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			cbCurrentStyle.SelectedIndexChanged -= OnCurrentStyleSelectedIndexChanged;
			edtName.EditValueChanged -= OnStyleNameEditValueChanged;
			cbParent.SelectedIndexChanged -= OnParentStyleSelectedIndexChanged;
			changeTableCellsShadingItem1.ColorChanged -= OnTableCellsShadingColorChanged;
		}
		protected internal void SubscribeToggleButtonsEvents() {
			toggleFontBoldItem1.CheckedChanged += OnFontBoldItemCheckedChanged;
			toggleFontItalicItem1.CheckedChanged += OnFontItalicItemCheckedChanged;
			toggleFontUnderlineItem1.CheckedChanged += OnFontUnderlineItemCheckedChanged;
		}
		protected internal void UnsubscribeToggleButtonsEvents() {
			toggleFontBoldItem1.CheckedChanged -= OnFontBoldItemCheckedChanged;
			toggleFontItalicItem1.CheckedChanged -= OnFontItalicItemCheckedChanged;
			toggleFontUnderlineItem1.CheckedChanged -= OnFontUnderlineItemCheckedChanged;
		}
		protected internal void SubscribeCharacterPropertiesEvents() {
			fontEditItem1.EditValueChanged += OnFontEditItemValueChanged;
			colorEditItem1.EditValueChanged += OnColorEditItemValueChanged;
			fontSizeEditItem1.EditValueChanged += OnSizeEditItemValueChanged;
		}
		protected internal void UnsubscribeCharacterPropertiesEvents() {
			fontEditItem1.EditValueChanged -= OnFontEditItemValueChanged;
			colorEditItem1.EditValueChanged -= OnColorEditItemValueChanged;
			fontSizeEditItem1.EditValueChanged -= OnSizeEditItemValueChanged;
		}
		protected internal virtual void OnStyleNameEditValueChanged(object sender, EventArgs e) {
			Controller.StyleName = edtName.Text;
		}
		protected internal virtual void OnParentStyleSelectedIndexChanged(object sender, EventArgs e) {
			ChangeConditionalType();
			SetParentStyle();
			UpdateRichEditBars();
		}
		protected internal virtual void OnTableCellsShadingColorChanged(object sender, DevExpress.XtraRichEdit.Native.StyleShadingEventArgs e) {
			Controller.ChangeConditionalCurrentValue<Color>(new BackgroundColorAccessor(), e.Color);
		}
		private void OnButtonFormatClick(object sender, EventArgs e) {
			btnFormat.ShowDropDown();
		}
		void OnOkButtonClick(object sender, EventArgs e) {
			if (Controller.IsValidName(edtName.Text)) {
				DialogResult = DialogResult.OK;
				Controller.ApplyChanges();
			}
			else
				XtraMessageBox.Show(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_ParagraphStyleNameAlreadyExists), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		protected internal void OnFontBoldItemCheckedChanged(object sender, ItemClickEventArgs e) {
			Controller.ChangeConditionalCurrentValue<bool>(new FontBoldAccessor(), toggleFontBoldItem1.Checked);
		}
		protected internal void OnFontItalicItemCheckedChanged(object sender, ItemClickEventArgs e) {
			Controller.ChangeConditionalCurrentValue<bool>(new FontItalicAccessor(), toggleFontItalicItem1.Checked);
		}
		protected internal void OnFontUnderlineItemCheckedChanged(object sender, ItemClickEventArgs e) {
			if (toggleFontUnderlineItem1.Checked)
				Controller.ChangeConditionalCurrentValue<UnderlineType>(new FontUnderlineTypeAccessor(), UnderlineType.Single);
			else
				Controller.ChangeConditionalCurrentValue<UnderlineType>(new FontUnderlineTypeAccessor(), UnderlineType.None);
		}
		protected internal void OnFontEditItemValueChanged(object sender, EventArgs e) {
			Controller.ChangeConditionalCurrentValue<string>(new FontNameAccessor(), (string)fontEditItem1.EditValue);
		}
		protected internal void OnColorEditItemValueChanged(object sender, EventArgs e) {
			Controller.ChangeConditionalCurrentValue<Color>(new ForeColorAccessor(), (Color)colorEditItem1.EditValue);
		}
		protected internal void OnSizeEditItemValueChanged(object sender, EventArgs e) {
			string text;
			int value;
			if (EditStyleHelper.IsFontSizeValid(fontSizeEditItem1.EditValue, out text, out value))
				Controller.ChangeConditionalCurrentValue<int>(new DoubleFontSizeAccessor(), value); 
			else {
				XtraMessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				fontSizeEditItem1.EditValue = CharacterProperties.DoubleFontSize /2f;
			}
		}
		protected virtual void SetNewStyleToController(string styleName) {
			TableStyleFormControllerParameters parameters = (TableStyleFormControllerParameters)Controller.Parameters;
			parameters.TableSourceStyle = parameters.TableSourceStyle.DocumentModel.TableStyles.GetStyleByName(styleName);
		}
		private void OnCurrentStyleSelectedIndexChanged(object sender, EventArgs e) {
			cbCurrentStyle.SelectedIndexChanged -= OnCurrentStyleSelectedIndexChanged;
			IStyle style = cbCurrentStyle.SelectedItem as IStyle;
			string styleName = style.StyleName;
			SetNewStyleToController(styleName);
			this.controller = CreateController(Controller.Parameters);
			cbCurrentStyle.Properties.Items.Clear();
			cbParent.Properties.Items.Clear();
			InitializeForm();
			cbCurrentStyle.SelectedIndexChanged += OnCurrentStyleSelectedIndexChanged;
			UpdateForm();
		}
		protected internal void OnFontButtonItemClick(object sender, ItemClickEventArgs e) {
			ChangeConditionalType();
			MergedCharacterProperties mergedProperties;
			if (CurrentConditionalStyle == null)
				mergedProperties = new MergedCharacterProperties(new CharacterFormattingInfo(), new CharacterFormattingOptions());
			else
				mergedProperties = Controller.GetMergedCharacterProperties();
			IRichEditControl control = Controller.Control;
			control.ShowFontForm(mergedProperties, ApplyCharacterProperties, null);
		}
		void ApplyCharacterProperties(MergedCharacterProperties properties, object data) {
			Controller.AddStyle();
			Controller.CopyCharacterPropertiesFromMerged(properties);
			UpdateRichEditBars();
		}
		protected internal void OnParagraphButtonItemClick(object sender, ItemClickEventArgs e) {
			ChangeConditionalType();
			MergedParagraphProperties mergedProperties;
			if (CurrentConditionalStyle == null)
				mergedProperties = new MergedParagraphProperties(new ParagraphFormattingInfo(), new ParagraphFormattingOptions());
			else
				mergedProperties = Controller.GetMergedParagraphProperties();
			IRichEditControl control = Controller.Control;
			control.ShowParagraphForm(mergedProperties, ApplyParagraphProperties, null);
		}
		void ApplyParagraphProperties(MergedParagraphProperties properties, object data) {
			Controller.AddStyle();
			Controller.CopyParagraphPropertiesFromMerged(properties);
			UpdateRichEditBars();
		}
		protected internal void OnTabsButtonItemClick(object sender, ItemClickEventArgs e) {
			ChangeConditionalType();
			IRichEditControl control = Controller.Control;
			TabFormattingInfo info = Controller.GetTabInfo();
			DocumentModel model = control.InnerControl.DocumentModel;
			int defaultTabWidth = model.DocumentProperties.DefaultTabWidth;
			control.ShowTabsForm(info, defaultTabWidth, ApplyTabsProperties, null);
		}
		void ApplyTabsProperties(TabFormattingInfo tabInfo, int defaultTabWidth, object data) {
			Controller.AddStyle();
			Controller.Tabs.SetTabs(tabInfo);
			UpdateRichEditBars();
		}
		protected virtual void OnApplyToSelectedIndexChangedCore() {
			((TableStyleFormController)Controller).UnsubscribeConditionalStyleEvents();
			((TableStyleFormController)Controller).ConditionalStyleType = (ConditionalTableStyleFormattingTypes)cbApplyTo.EditValue;
			ChangeConditionalType();
			((TableStyleFormController)Controller).SubscribeConditionalStyleEvents();
			UpdateRichEditBars();
		}
		private void OnApplyToSelectedIndexChanged(object sender, EventArgs e) {
			OnApplyToSelectedIndexChangedCore();
		}
		private void OnTableCellsAlignmentItemClick(object sender, ItemClickEventArgs e) {
			ChangeConditionalType();
			changeTableAlignmentItem.Glyph = e.Item.Glyph;
			KeyValuePair<VerticalAlignment, ParagraphAlignment> alignment = Alignments[e.Item.Name];
			Controller.ChangeConditionalCurrentAlignmentValue(alignment.Key, alignment.Value);
		}
		protected virtual void OnTableCellsBorderItemPressCore() {
			Controller.AddStyle();
			((TableStyleFormController)Controller).SubscribeConditionalStyleEvents();
			ChangeConditionalType();
		}
		private void OnTableCellsBorderItemPress(object sender, ItemClickEventArgs e) {
			OnTableCellsBorderItemPressCore();
		}
	}
}
namespace DevExpress.XtraRichEdit.Native {
	public class ChangeConditionalStyleShadingItem : ChangeTableCellsShadingItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ChangeConditionalStyleShadingCommand command = new ChangeConditionalStyleShadingCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		#region Events
		EventHandler<StyleShadingEventArgs> colorChanged;
		public event EventHandler<StyleShadingEventArgs> ColorChanged {
			add { colorChanged += value; }
			remove { colorChanged -= value; }
		}
		#endregion
		protected override void OnInternalColorChanged() {
			StyleShadingEventArgs args = new StyleShadingEventArgs();
			args.Color = this.Color;
			if (colorChanged != null)
				colorChanged(this, args);
			base.OnInternalColorChanged();
		}
	}
	public class StyleShadingEventArgs : EventArgs {
		public Color Color { get; set; }
	}
	class ChangeConditionalStyleShadingCommand : ChangeTableCellsShadingCommand {
		public ChangeConditionalStyleShadingCommand(IRichEditControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
		}
	}
	public class ToggleConditionalTableCellsBordersCommandBase : ToggleTableCellsBordersCommandBase {
		protected ToggleConditionalTableCellsBordersCommandBase(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { throw new NotImplementedException(); } }
		public override XtraRichEditStringId DescriptionStringId { get { throw new NotImplementedException(); } }
		protected internal ITableBorders CurrentBorders { get { return ((PreviewRichEditControl)Control).CurrentBorders; } } 
		protected internal override void CollectBorders(ITableCellBorders cellBorders, SelectedTableCellPositionInfo cellPositionInfo, List<BorderBase> borders) {
		}
		protected override void CollectTableBorders(TableBorders tableBorders, List<BorderBase> borders) {
		}
	}
	public class ToggleConditionalTableCellsBottomBorderItem : ToggleTableCellsBottomBorderItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ToggleConditionalTableCellsBottomBorderCommand command = new ToggleConditionalTableCellsBottomBorderCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	public class ToggleConditionalTableCellsBottomBorderCommand : ToggleConditionalTableCellsBordersCommandBase {
		public ToggleConditionalTableCellsBottomBorderCommand(IRichEditControl control) : base(control) { }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomBorder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomBorderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableCellsBottomBorder; } }
		public override string ImageName { get { return "BorderBottom"; } }
		protected internal override void ResetBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.BottomBorder;
			base.ResetBorder(localBorder);
		}
		protected internal override void ModifyBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.BottomBorder;
			base.ModifyBorder(localBorder);
		}
		protected internal override bool CalculateIsChecked(List<BorderBase> borders) {
			List<BorderBase> localBorders = new List<BorderBase>();
			localBorders.Add(CurrentBorders.BottomBorder);
			return base.CalculateIsChecked(localBorders);
		}
	}
	public class ToggleConditionalTableCellsTopBorderItem : ToggleTableCellsTopBorderItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ToggleConditionalTableCellsTopBorderCommand command = new ToggleConditionalTableCellsTopBorderCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	public class ToggleConditionalTableCellsTopBorderCommand : ToggleConditionalTableCellsBordersCommandBase {
		public ToggleConditionalTableCellsTopBorderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsTopBorder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsTopBorderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableCellsTopBorder; } }
		public override string ImageName { get { return "BorderTop"; } }
		protected internal override void ResetBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.TopBorder;
			base.ResetBorder(localBorder);
		}
		protected internal override void ModifyBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.TopBorder;
			base.ModifyBorder(localBorder);
		}
		protected internal override bool CalculateIsChecked(List<BorderBase> borders) {
			List<BorderBase> localBorders = new List<BorderBase>();
			localBorders.Add(CurrentBorders.TopBorder);
			return base.CalculateIsChecked(localBorders);
		}
	}
	public class ToggleConditionalTableCellsLeftBorderItem : ToggleTableCellsLeftBorderItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ToggleConditionalTableCellsLeftBorderCommand command = new ToggleConditionalTableCellsLeftBorderCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	public class ToggleConditionalTableCellsLeftBorderCommand : ToggleConditionalTableCellsBordersCommandBase {
		public ToggleConditionalTableCellsLeftBorderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsLeftBorder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsLeftBorderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableCellsLeftBorder; } }
		public override string ImageName { get { return "BorderLeft"; } }
		protected internal override void ResetBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.LeftBorder;
			base.ResetBorder(localBorder);
		}
		protected internal override void ModifyBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.LeftBorder;
			base.ModifyBorder(localBorder);
		}
		protected internal override bool CalculateIsChecked(List<BorderBase> borders) {
			List<BorderBase> localBorders = new List<BorderBase>();
			localBorders.Add(CurrentBorders.LeftBorder);
			return base.CalculateIsChecked(localBorders);
		}
	}
	public class ToggleConditionalTableCellsRightBorderItem : ToggleTableCellsRightBorderItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ToggleConditionalTableCellsRightBorderCommand command = new ToggleConditionalTableCellsRightBorderCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	public class ToggleConditionalTableCellsRightBorderCommand : ToggleConditionalTableCellsBordersCommandBase {
		public ToggleConditionalTableCellsRightBorderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsRightBorder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsRightBorderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableCellsRightBorder; } }
		public override string ImageName { get { return "BorderRight"; } }
		protected internal override void ResetBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.RightBorder;
			base.ResetBorder(localBorder);
		}
		protected internal override void ModifyBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.RightBorder;
			base.ModifyBorder(localBorder);
		}
		protected internal override bool CalculateIsChecked(List<BorderBase> borders) {
			List<BorderBase> localBorders = new List<BorderBase>();
			localBorders.Add(CurrentBorders.RightBorder);
			return base.CalculateIsChecked(localBorders);
		}
	}
	public class ResetConditionalTableCellsAllBordersItem : ResetTableCellsAllBordersItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ResetConditionalTableCellsBordersCommand command = new ResetConditionalTableCellsBordersCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	public class ResetConditionalTableCellsBordersCommand : ToggleConditionalTableCellsBordersCommandBase {
		public ResetConditionalTableCellsBordersCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ResetTableCellsBorders; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ResetTableCellsBordersDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ResetTableCellsAllBorders; } }
		public override string ImageName { get { return "BorderNone"; } }
		protected internal override void ResetBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.LeftBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.RightBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.TopBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.BottomBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.InsideHorizontalBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.InsideVerticalBorder;
			base.ResetBorder(localBorder);
		}
		protected internal override bool CalculateIsChecked(List<BorderBase> borders) {
			return true;
		}
	}
	public class ToggleConditionalTableCellsAllBordersItem : ToggleTableCellsAllBordersItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ToggleConditionalTableCellsAllBordersBorderCommand command = new ToggleConditionalTableCellsAllBordersBorderCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	public class ToggleConditionalTableCellsAllBordersBorderCommand : ToggleConditionalTableCellsBordersCommandBase {
		public ToggleConditionalTableCellsAllBordersBorderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsAllBorders; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsAllBordersDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableCellsAllBorders; } }
		public override string ImageName { get { return "BordersAll"; } }
		protected internal override void ResetBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.LeftBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.RightBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.TopBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.BottomBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.InsideHorizontalBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.InsideVerticalBorder;
			base.ResetBorder(localBorder);
		}
		protected internal override void ModifyBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.LeftBorder;
			base.ModifyBorder(localBorder);
			localBorder = CurrentBorders.RightBorder;
			base.ModifyBorder(localBorder);
			localBorder = CurrentBorders.TopBorder;
			base.ModifyBorder(localBorder);
			localBorder = CurrentBorders.BottomBorder;
			base.ModifyBorder(localBorder);
			localBorder = CurrentBorders.InsideHorizontalBorder;
			base.ModifyBorder(localBorder);
			localBorder = CurrentBorders.InsideVerticalBorder;
			base.ModifyBorder(localBorder);
		}
		protected internal override bool CalculateIsChecked(List<BorderBase> borders) {
			List<BorderBase> localBorders = new List<BorderBase>();
			localBorders.Add(CurrentBorders.LeftBorder);
			localBorders.Add(CurrentBorders.RightBorder);
			localBorders.Add(CurrentBorders.TopBorder);
			localBorders.Add(CurrentBorders.BottomBorder);
			localBorders.Add(CurrentBorders.InsideHorizontalBorder);
			localBorders.Add(CurrentBorders.InsideVerticalBorder);
			return base.CalculateIsChecked(localBorders);
		}
	}
	public class ToggleConditionalTableCellsOutsideBorderItem : ToggleTableCellsOutsideBorderItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ToggleConditionalTableCellsOutsideBorderCommand command = new ToggleConditionalTableCellsOutsideBorderCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	public class ToggleConditionalTableCellsOutsideBorderCommand : ToggleConditionalTableCellsBordersCommandBase {
		public ToggleConditionalTableCellsOutsideBorderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsOutsideBorder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsOutsideBorderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableCellsOutsideBorder; } }
		public override string ImageName { get { return "BordersOutside"; } }
		protected internal override void ResetBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.LeftBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.RightBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.TopBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.BottomBorder;
			base.ResetBorder(localBorder);
		}
		protected internal override void ModifyBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.LeftBorder;
			base.ModifyBorder(localBorder);
			localBorder = CurrentBorders.RightBorder;
			base.ModifyBorder(localBorder);
			localBorder = CurrentBorders.TopBorder;
			base.ModifyBorder(localBorder);
			localBorder = CurrentBorders.BottomBorder;
			base.ModifyBorder(localBorder);
		}
		protected internal override bool CalculateIsChecked(List<BorderBase> borders) {
			List<BorderBase> localBorders = new List<BorderBase>();
			localBorders.Add(CurrentBorders.LeftBorder);
			localBorders.Add(CurrentBorders.RightBorder);
			localBorders.Add(CurrentBorders.TopBorder);
			localBorders.Add(CurrentBorders.BottomBorder);
			return base.CalculateIsChecked(localBorders);
		}
	}
	public class ToggleConditionalTableCellsInsideBorderItem : ToggleTableCellsInsideBorderItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ToggleConditionalTableCellsInsideBorderCommand command = new ToggleConditionalTableCellsInsideBorderCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	public class ToggleConditionalTableCellsInsideBorderCommand : ToggleConditionalTableCellsBordersCommandBase {
		public ToggleConditionalTableCellsInsideBorderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideBorder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideBorderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableCellsInsideBorder; } }
		public override string ImageName { get { return "BordersInside"; } }
		protected internal override void ResetBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.InsideHorizontalBorder;
			base.ResetBorder(localBorder);
			localBorder = CurrentBorders.InsideVerticalBorder;
			base.ResetBorder(localBorder);
		}
		protected internal override void ModifyBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.InsideHorizontalBorder;
			base.ModifyBorder(localBorder);
			localBorder = CurrentBorders.InsideVerticalBorder;
			base.ModifyBorder(localBorder);
		}
		protected internal override bool CalculateIsChecked(List<BorderBase> borders) {
			List<BorderBase> localBorders = new List<BorderBase>();
			localBorders.Add(CurrentBorders.InsideHorizontalBorder);
			localBorders.Add(CurrentBorders.InsideVerticalBorder);
			return base.CalculateIsChecked(localBorders);
		}
	}
	public class ToggleConditionalTableCellsInsideHorizontalBorderItem : ToggleTableCellsInsideHorizontalBorderItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ToggleConditionalTableCellsInsideHorizontalBorderCommand command = new ToggleConditionalTableCellsInsideHorizontalBorderCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	public class ToggleConditionalTableCellsInsideHorizontalBorderCommand : ToggleConditionalTableCellsBordersCommandBase {
		public ToggleConditionalTableCellsInsideHorizontalBorderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideHorizontalBorder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideHorizontalBorderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableCellsInsideHorizontalBorder; } }
		public override string ImageName { get { return "BorderInsideHorizontal"; } }
		protected internal override void ResetBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.InsideHorizontalBorder;
			base.ResetBorder(localBorder);
		}
		protected internal override void ModifyBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.InsideHorizontalBorder;
			base.ModifyBorder(localBorder);
		}
		protected internal override bool CalculateIsChecked(List<BorderBase> borders) {
			List<BorderBase> localBorders = new List<BorderBase>();
			localBorders.Add(CurrentBorders.InsideHorizontalBorder);
			return base.CalculateIsChecked(localBorders);
		}
	}
	public class ToggleConditionalTableCellsInsideVerticalBorderItem : ToggleTableCellsInsideVerticalBorderItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			ToggleConditionalTableCellsInsideVerticalBorderCommand command = new ToggleConditionalTableCellsInsideVerticalBorderCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	public class ToggleConditionalTableCellsInsideVerticalBorderCommand : ToggleConditionalTableCellsBordersCommandBase {
		public ToggleConditionalTableCellsInsideVerticalBorderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideVerticalBorder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideVerticalBorderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableCellsInsideVerticalBorder; } }
		public override string ImageName { get { return "BorderInsideVertical"; } }
		protected internal override void ResetBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.InsideVerticalBorder;
			base.ResetBorder(localBorder);
		}
		protected internal override void ModifyBorder(BorderBase border) {
			BorderBase localBorder = CurrentBorders.InsideVerticalBorder;
			base.ModifyBorder(localBorder);
		}
		protected internal override bool CalculateIsChecked(List<BorderBase> borders) {
			List<BorderBase> localBorders = new List<BorderBase>();
			localBorders.Add(CurrentBorders.InsideVerticalBorder);
			return base.CalculateIsChecked(localBorders);
		}
	}
}
