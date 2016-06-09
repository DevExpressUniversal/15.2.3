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
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.RichEdit.Localization;
namespace DevExpress.Xpf.RichEdit {
	#region RichEditControlStringId
	public enum RichEditControlStringId {
		Caption_BookmarkForm,
		Caption_BookmarkFormAddButton,
		Caption_BookmarkFormDeleteButton,
		Caption_BookmarkFormGoToButton,
		Caption_BookmarkFormBookmarkNameLabel,
		Caption_BookmarkFormSortByLabel,
		Caption_BookmarkFormSortByNameRadioButton,
		Caption_BookmarkFormSortByLocationRadioButton,
		Caption_DeleteTableCellsForm,
		Caption_DeleteTableCellsFormShiftCellsLeftRadioButton,
		Caption_DeleteTableCellsFormShiftCellsUpRadioButton,
		Caption_DeleteTableCellsFormDeleteEntireRowRadioButton,
		Caption_DeleteTableCellsFormDeleteEntireColumnRadioButton,
		Caption_DocumentProtectionQueryNewPasswordForm,
		Caption_DocumentProtectionQueryNewPasswordFormEnterNewPasswordLabel,
		Caption_DocumentProtectionQueryNewPasswordFormReenterPasswordToConfirmLabel,
		Caption_DocumentProtectionQueryPasswordForm,
		Caption_DocumentProtectionQueryPasswordFormPasswordLabel,
		Caption_FontForm,
		Caption_FontFormNameLabel,
		Caption_FontFormStyleLabel,
		Caption_FontFormNormalListBox,
		Caption_FontFormBoldListBox,
		Caption_FontFormItalicListBox,
		Caption_FontFormBoldItalicListBox,
		Caption_FontFormSizeLabel,
		Caption_FontFormForeColorLabel,
		Caption_FontFormUnderlineLabel,
		Caption_FontFormUnderlineColorLabel,
		Caption_FontFormEffectsLabel,
		Caption_FontFormStrikeNormalRadioButton,
		Caption_FontFormStrikeoutRadioButton,
		Caption_FontFormDoubleStrikeoutRadionButton,
		Caption_FontFormAllCapsCheckEdit,
		Caption_FontFormHiddenCheckEdit,
		Caption_FontFormUnderlineWordsOnlyCheckEdit,
		Caption_FontFormScriptNormalRadioButton,
		Caption_FontFormSuperscriptRadioButton,
		Caption_FontFormSubscriptRadioButton,
		Caption_HyperlinkFormTextToDisplayLabel,
		Caption_HyperlinkFormScreenTipLabel,
		Caption_HyperlinkFormTargetFrameLabel,
		Caption_HyperlinkFormLinkToLabel,
		Caption_HyperlinkFormExistingFileOrWebPageRadioButton,
		Caption_HyperlinkFormPlaceInThisDocumentRadioButton,
		Caption_HyperlinkFormAddressLabel,
		Caption_InsertTableCellsForm,
		Caption_InsertTableCellsFormShiftCellsRightRadioButton,
		Caption_InsertTableCellsFormShiftCellsDownRadioButton,
		Caption_InsertTableCellsFormInsertEntireRowRadioButton,
		Caption_InsertTableCellsFormInsertEntireColumnRadioButton,
		Caption_InsertTableForm,
		Caption_InsertTableFormTableSizeLabel,
		Caption_InsertTableFormNumberOfColumnsLabel,
		Caption_InsertTableFormNumberOfRowsLabel,
		Caption_ParagraphForm,
		Caption_ParagraphFormGeneralLabel,
		Caption_ParagraphFormAlignmentLabel,
		Caption_ParagraphFormOutlineLevelLabel,
		Caption_ParagraphFormIndentationLabel,
		Caption_ParagraphFormSpacingLabel,
		Caption_ParagraphFormTabsButton,
		Caption_ParagraphFormPaginationLabel,
		Caption_ParagraphFormIndentsAndSpacing,
		Caption_ParagraphFormLineAndPageBreaks,
		Caption_ParagraphFormKeepLinesTogetherLabel,
		Caption_ParagraphFormPageBreakBeforeLabel,
		Caption_ParagraphFormContextualSpacingCheckEdit,
		Caption_FloatingObjectLayoutForm,
		Caption_FloatingObjectLayoutFormPosition,
		Caption_FloatingObjectLayoutFormHorizontalLabel,
		Caption_FloatingObjectLayoutFormVerticalLabel,
		Caption_FloatingObjectLayoutFormOptionsLabel,
		Caption_FloatingObjectLayoutFormTextWrapping,
		Caption_FloatingObjectLayoutFormSize,
		Caption_FloatingObjectLayoutFormHorizontalAbsolutePositionRadioButton,
		Caption_FloatingObjectLayoutFormHorizontalPositionAlignmentRadioButton,
		Caption_FloatingObjectLayoutFormHorizontalPositionTypeLabel,
		Caption_FloatingObjectLayoutFormHorizontalAbsolutePositionLabel,
		Caption_FloatingObjectLayoutFormVerticalPositionAlignmentRadioButton,
		Caption_FloatingObjectLayoutFormVerticalAbsolutePositionRadioButton,
		Caption_FloatingObjectLayoutFormVerticalPositionTypeLabel,
		Caption_FloatingObjectLayoutFormVerticalAbsolutePositionLabel,
		Caption_FloatingObjectLayoutFormLockAnchorCheckEdit,
		Caption_FloatingObjectLayoutFormWrappingStyleLabel,
		Caption_FloatingObjectLayoutFormWrapTextLabel,
		Caption_FloatingObjectLayoutFormBothSidesRadioButton,
		Caption_FloatingObjectLayoutFormLeftOnlyRadioButton,
		Caption_FloatingObjectLayoutFormRightOnlyRadioButton,
		Caption_FloatingObjectLayoutFormLargestOnlyRadioButton,
		Caption_FloatingObjectLayoutFormDistanceFromTextLabel,
		Caption_FloatingObjectLayoutFormTopLabel,
		Caption_FloatingObjectLayoutFormBottomLabel,
		Caption_FloatingObjectLayoutFormLeftLabel,
		Caption_FloatingObjectLayoutFormRightLabel,
		Caption_FloatingObjectLayoutFormRotationLabel,
		Caption_FloatingObjectLayoutFormLockAspectRatioCheckEdit,
		Caption_FloatingObjectLayoutFormSquarePreset,
		Caption_FloatingObjectLayoutFormTightPreset,
		Caption_FloatingObjectLayoutFormThoughtPreset,
		Caption_FloatingObjectLayoutFormTopAndBottomPreset,
		Caption_FloatingObjectLayoutFormBehindTextPreset,
		Caption_FloatingObjectLayoutFormInFrontOfTextPreset,
		Caption_FloatingObjectLayoutFormRotateLabel,
		Caption_FloatingObjectLayoutFormScaleLabel,
		Caption_FloatingObjectLayoutFormTabSizeHeightLabel,
		Caption_FloatingObjectLayoutFormTabSizeHeightAbsoluteLabel,
		Caption_FloatingObjectLayoutFormTabSizeWidthLabel,
		Caption_FloatingObjectLayoutFormTabSizeWidthAbsoluteLabel,
		Caption_FloatingObjectLayoutFormTabSizeOriginalSizeLabel,
		Caption_FloatingObjectLayoutFormTabSizeOriginalSizeWidthLabel,
		Caption_FloatingObjectLayoutFormTabSizeOriginalSizeHeightLabel,
		Caption_FloatingObjectLayoutFormTabSizeResetButton,
		Caption_EditStyleForm,
		Caption_EditStyleFormFormatButton,
		Caption_EditStyleFormFontButton,
		Caption_EditStyleFormParagraphButton,
		Caption_EditStyleFormTabsButton,
		Caption_EditStyleFormStyleNameTextEdit,
		Caption_EditStyleFormCurrentStyleNameComboBox,
		Caption_EditStyleFormParentStyleNameComboBox,
		Caption_EditStyleFormNextStyleNameComboBox,
		Caption_EditStyleFormLineSpacingBarSubItem,
		Caption_EditStyleFormBoldBarCheckItemDescription,
		Caption_EditStyleFormItalicBarCheckItemDescription,
		Caption_EditStyleFormUnderlineBarCheckItemDescription,
		Caption_EditStyleFormLeftBarCheckItemDescription,
		Caption_EditStyleFormCenterBarCheckItemDescription,
		Caption_EditStyleFormRightBarCheckItemDescription,
		Caption_EditStyleFormJustifyBarCheckItemDescription,
		Caption_EditStyleFormIncreaseParagraphSpacingBarCheckItemDescription,
		Caption_EditStyleFormDecreaseParagraphSpacingBarCheckItemDescription,
		Caption_EditStyleFormDecreaseIndentBarCheckItemDescription,
		Caption_EditStyleFormIncreaseIndentBarCheckItemDescription,
		Caption_ParagraphIndentationLeftLabel,
		Caption_ParagraphIndentationRightLabel,
		Caption_ParagraphIndentationSpecialLabel,
		Caption_ParagraphIndentationByLabel,
		Caption_ParagraphSpacingBeforeLabel,
		Caption_ParagraphSpacingAfterLabel,
		Caption_ParagraphSpacingLineSpacingLabel,
		Caption_ParagraphSpacingAtLabel,
		Caption_RangeEditingPermissionsForm,
		Caption_RangeEditingPermissionsFormUsersLabel,
		Caption_RangeEditingPermissionsFormGroupsLabel,
		Caption_SplitTableCellsForm,
		Caption_SplitTableCellsFormNumberOfColumnsLable,
		Caption_SplitTableCellsFormNumberOfRowsLable,
		Caption_SplitTableCellsFormMergeCellsBeforeSplitCheckEdit,
		Msg_Warning,
		Caption_TabsForm,
		Caption_TabsFormTabStopPositionLabel,
		Caption_TabsFormDefaultTabStopsLabel,
		Caption_TabsFormTabStopsToBeClearedLabel,
		Caption_TabsFormAlignmentLabel,
		Caption_TabsFormLeftRadioButton,
		Caption_TabsFormRightRadioButton,
		Caption_TabsFormDecimalRadionButton,
		Caption_TabsFormCenterRadioButton,
		Caption_TabsFormLeaderLabel,
		Caption_TabsFormNoneRadioButton,
		Caption_TabsFormHyphensRadioButton,
		Caption_TabsFormEqualSignRadioButton,
		Caption_TabsFormDotsRadioButton,
		Caption_TabsFormUnderlineRadioButton,
		Caption_TabsFormMiddleDotsRadioButton,
		Caption_TabsFormThickLineRadioButton,
		Caption_TabsFormSetButton,
		Caption_TabsFormClearButton,
		Caption_TabsFormClearAllButton,
		Caption_LineNumberingForm,
		Caption_LineNumberingFormAddLineNumberingCheckEdit,
		Caption_LineNumberingStartAtLabel,
		Caption_LineNumberingCountByLabel,
		Caption_LineNumberingFromTextLabel,
		Caption_LineNumberingNumberingLabel,
		Caption_LineNumberingRestartEachPageRadioButton,
		Caption_LineNumberingRestartEachSectionRadioButton,
		Caption_LineNumberingContinuousRadioButton,
		Caption_PasteSpecialForm,
		Caption_PasteSpecialPasteAsLabel,
		Caption_PageSetupForm,
		Caption_PageSetupMarginsTab,
		Caption_PageSetupPaperTab,
		Caption_PageSetupLayoutTab,
		Caption_PageSetupMarginsLabel,
		Caption_PageSetupTopMarginLabel,
		Caption_PageSetupBottomMarginLabel,
		Caption_PageSetupLeftMarginLabel,
		Caption_PageSetupRightMarginLabel,
		Caption_PageSetupOrientationLabel,
		Caption_PageSetupOrientationPortraitRadioButton,
		Caption_PageSetupOrientationLandscapeRadioButton,
		Caption_PageSetupApplyToLabel,
		Caption_PageSetupPaperSizeLabel,
		Caption_PageSetupPaperWidthLabel,
		Caption_PageSetupPaperHeightLabel,
		Caption_PageSetupSectionLabel,
		Caption_PageSetupSectionStartLabel,
		Caption_PageSetupHeadersAndFootersLabel,
		Caption_PageSetupDifferentOddAndEvenLabel,
		Caption_PageSetupDifferentFirstPageLabel,
		Caption_TableOptionsForm,
		Caption_TableOptionsFormDefaultCellMarginsLabel,
		Caption_TableOptionsFormTopLabel,
		Caption_TableOptionsFormLeftLabel,
		Caption_TableOptionsFormRightLabel,
		Caption_TableOptionsFormBottomLabel,
		Caption_TableOptionsFormDefaultCellSpacingLabel,
		Caption_TableOptionsFormAllowSpacingBetweenCellsCheckEdit,
		Caption_TableOptionsFormOptionsLabel,
		Caption_TableOptionsFormAutomaticallyResizeCheckEdit,
		Caption_TableCellOptionsForm,
		Caption_TableCellOptionsFormCellMarginsLabel,
		Caption_TableCellOptionsFormSameAsWholeTableCheckEdit,
		Caption_TableCellOptionsFormTopLabel,
		Caption_TableCellOptionsFormLeftLabel,
		Caption_TableCellOptionsFormBottomLabel,
		Caption_TableCellOptionsFormRightLabel,
		Caption_TableCellOptionsFormOptionsLabel,
		Caption_TableCellOptionsFormWrapTextCheckEdit,
		Caption_TableCellOptionsFormFitTextCheckEdit,
		Caption_TablePropertiesForm,
		Caption_TablePropertiesFormTableTab,
		Caption_TablePropertiesFormRowTab,
		Caption_TablePropertiesFormColumnTab,
		Caption_TablePropertiesFormCellTab,
		Caption_TablePropertiesFormSizeLabel,
		Caption_TablePropertiesFormTableAlignmentLabel,
		Caption_TablePropertiesFormLeftRadioButton,
		Caption_TablePropertiesFormCenterRadioButton,
		Caption_TablePropertiesFormRightRadioButton,
		Caption_TablePropertiesFormTableIndentLabel,
		Caption_TablePropertiesFormOptionsButton,
		Caption_TablePropertiesFormBordersButton,
		Caption_TablePropertiesFormRowLabel,
		Caption_TablePropertiesFormOptionsLabel,
		Caption_TablePropertiesFormAllowRowToBreakAcrossPagesCheckEdit,
		Caption_TablePropertiesFormRepeatHeaderCheckEdit,
		Caption_TablePropertiesFormColumnLabel,
		Caption_TablePropertiesFormCellVerticalAlignmentLabel,
		Caption_TablePropertiesFormTopRadioButton,
		Caption_TablePropertiesFormBottomRadioButton,
		Caption_BorderShadingForm,
		Caption_BorderShadingFormBordersTab,
		Caption_BorderShadingFormShadingTab,
		Caption_BorderShadingFormSettingsLabel,
		Caption_BorderShadingFormNoneLabel,
		Caption_BorderShadingFormBoxLabel,
		Caption_BorderShadingFormAllLabel,
		Caption_BorderShadingFormGridLabel,
		Caption_BorderShadingFormCustomLabel,
		Caption_BorderShadingFormFillLabel,
		Caption_BorderShadingUserControlPreviewLabel,
		Caption_BorderShadingUserControlClickLabel,
		Caption_BorderShadingUserControlButtonsLabel,
		Caption_BorderShadingUserControlApplyLabel,
		Caption_BorderShadingUserControlOptionsButton,
		Caption_BorderShadingTypeLineUserControlStyleLabel,
		Caption_BorderShadingTypeLineUserControlColorLabel,
		Caption_BorderShadingTypeLineUserControlWidthLabel,
		Caption_TableSizePreferredWidthCheckEdit,
		Caption_TableSizeMeasureInLabel,
		Caption_TableRowHeightSpecifyHeightCheckEdit,
		Caption_TableRowHeightHeightIsLabel,
		Caption_ColumnsSetupForm,
		Caption_ColumnsSetupFormColumnNumberLabel,
		Caption_ColumnsSetupFormColumnWidthLabel,
		Caption_ColumnsSetupFormColumnSpacingLabel,
		Caption_ColumnsSetupFormApplyToLabel,
		Caption_ColumnsSetupFormPresetsGroup,
		Caption_ColumnsSetupFormWidthAndSpacingGroup,
		Caption_ColumnsSetupFormNumberOfColumnsLabel,
		Caption_ColumnsSetupFormEqualColumnWidthCheckEdit,
		Caption_ColumnsSetupFormOneColumnPreset,
		Caption_ColumnsSetupFormTwoColumnsPreset,
		Caption_ColumnsSetupFormThreeColumnsPreset,
		Caption_ColumnsSetupFormLeftNarrowColumnPreset,
		Caption_ColumnsSetupFormRightNarrowColumnPreset,
		Caption_SymbolForm,
		Caption_SymbolFormSearchByCode,
		Caption_SymbolFormFontName,
		Caption_SymbolFormCharacterSet,
		Caption_SymbolFormFilter,
		Caption_PageCategoryHeaderFooterTools,
		Caption_PageCategoryTableTools,
		Caption_PageFile,
		Caption_PageHome,
		Caption_PageInsert,
		Caption_PageMailings,
		Caption_PageView,
		Caption_PageHeaderFooterToolsDesign,
		Caption_PagePageLayout,
		Caption_PageTableLayout,
		Caption_PageTableDesign,
		Caption_PageReview,
		Caption_PageReferences,
		Caption_PageFloatingObjectPictureToolsFormat,
		Caption_PageCategoryFloatingObjectPictureTools,
		Caption_GroupFont,
		Caption_GroupParagraph,
		Caption_GroupClipboard,
		Caption_GroupCommon,
		Caption_GroupEditing,
		Caption_GroupStyles,
		Caption_GroupZoom,
		Caption_GroupShow,
		Caption_GroupIllustrations,
		Caption_GroupText,
		Caption_GroupTables,
		Caption_GroupTableTable,
		Caption_GroupTableRowsAndColumns,
		Caption_GroupTableMerge,
		Caption_GroupTableCellSize,
		Caption_GroupTableAlignment,
		Caption_GroupTableStyles,
		Caption_GroupTableDrawBorders,
		Caption_GroupSymbols,
		Caption_GroupLinks,
		Caption_GroupPages,
		Caption_GroupHeaderFooter,
		Caption_GroupHeaderFooterToolsDesignClose,
		Caption_GroupHeaderFooterToolsDesignNavigation,
		Caption_GroupMailMerge,
		Caption_GroupDocumentViews,
		Caption_GroupHeaderFooterToolsDesignOptions,
		Caption_GroupPageSetup,
		Caption_GroupPageBackground,
		Caption_GroupDocumentProtection,
		Caption_GroupDocumentProofing,
		Caption_GroupLanguage,
		Caption_GroupComment,
		Caption_GroupTracking,
		Caption_GroupTableOfContents,
		Caption_GroupCaptions,
		Caption_GroupWriteInsertFields,
		Caption_GroupPreviewResults,
		Caption_GroupFloatingPictureToolsArrange,
		Caption_GroupFloatingPictureToolsShapeStyles,
		Caption_CommonCharactersToggleButton,
		Caption_SpecialCharactersToggleButton,
		Caption_ReviewingPaneForm,
		Caption_StyleGalleryItemText,
		Msg_TargetTypeNotSupported
	}
	#endregion
	#region RichEditStringIdConverter
	public class RichEditStringIdConverter : StringIdConverter<RichEditControlStringId> {
		static RichEditStringIdConverter() {
			XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_BookmarkFormAddButton);
		}
		protected override XtraLocalizer<RichEditControlStringId> Localizer { get { return XpfRichEditLocalizer.Active; } }
	}
	#endregion
}
namespace DevExpress.Xpf.RichEdit.Localization {
	#region XpfRichEditLocalizer
	public class XpfRichEditLocalizer : DXLocalizer<RichEditControlStringId> {
		static XpfRichEditLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<RichEditControlStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(RichEditControlStringId.Caption_BookmarkForm, "Bookmark");
			AddString(RichEditControlStringId.Caption_BookmarkFormAddButton, "Add");
			AddString(RichEditControlStringId.Caption_BookmarkFormDeleteButton, "Delete");
			AddString(RichEditControlStringId.Caption_BookmarkFormGoToButton, "Go To");
			AddString(RichEditControlStringId.Caption_BookmarkFormBookmarkNameLabel, "Bookmark Name:");
			AddString(RichEditControlStringId.Caption_BookmarkFormSortByLabel, "Sort By:");
			AddString(RichEditControlStringId.Caption_BookmarkFormSortByNameRadioButton, "Name");
			AddString(RichEditControlStringId.Caption_BookmarkFormSortByLocationRadioButton, "Location");
			AddString(RichEditControlStringId.Caption_DeleteTableCellsForm, "Delete Cells");
			AddString(RichEditControlStringId.Caption_DeleteTableCellsFormShiftCellsLeftRadioButton, "Shift cells left");
			AddString(RichEditControlStringId.Caption_DeleteTableCellsFormShiftCellsUpRadioButton, "Shift cells up");
			AddString(RichEditControlStringId.Caption_DeleteTableCellsFormDeleteEntireRowRadioButton, "Delete entire row");
			AddString(RichEditControlStringId.Caption_DeleteTableCellsFormDeleteEntireColumnRadioButton, "Delete entire column");
			AddString(RichEditControlStringId.Caption_DocumentProtectionQueryNewPasswordForm, "Start Enforcing Protection");
			AddString(RichEditControlStringId.Caption_DocumentProtectionQueryNewPasswordFormEnterNewPasswordLabel, "Enter new password (optional):");
			AddString(RichEditControlStringId.Caption_DocumentProtectionQueryNewPasswordFormReenterPasswordToConfirmLabel, "Reenter password to confirm:");
			AddString(RichEditControlStringId.Caption_DocumentProtectionQueryPasswordForm, "Unprotect document");
			AddString(RichEditControlStringId.Caption_DocumentProtectionQueryPasswordFormPasswordLabel, "Password:");
			AddString(RichEditControlStringId.Caption_FontForm, "Font");
			AddString(RichEditControlStringId.Caption_FontFormNameLabel, "Font:");
			AddString(RichEditControlStringId.Caption_FontFormStyleLabel, "Font style:");
			AddString(RichEditControlStringId.Caption_FontFormNormalListBox, "Normal");
			AddString(RichEditControlStringId.Caption_FontFormBoldListBox, "Bold");
			AddString(RichEditControlStringId.Caption_FontFormItalicListBox, "Italic");
			AddString(RichEditControlStringId.Caption_FontFormBoldItalicListBox, "Bold Italic");
			AddString(RichEditControlStringId.Caption_FontFormSizeLabel, "Size:");
			AddString(RichEditControlStringId.Caption_FontFormForeColorLabel, "Font Color:");
			AddString(RichEditControlStringId.Caption_FontFormUnderlineLabel, "Underline style:");
			AddString(RichEditControlStringId.Caption_FontFormUnderlineColorLabel, "Underline color:");
			AddString(RichEditControlStringId.Caption_FontFormEffectsLabel, "Effects");
			AddString(RichEditControlStringId.Caption_FontFormStrikeNormalRadioButton, "Normal");
			AddString(RichEditControlStringId.Caption_FontFormStrikeoutRadioButton, "Strikeout");
			AddString(RichEditControlStringId.Caption_FontFormDoubleStrikeoutRadionButton, "Double Strikeout");
			AddString(RichEditControlStringId.Caption_FontFormAllCapsCheckEdit, "All caps");
			AddString(RichEditControlStringId.Caption_FontFormHiddenCheckEdit, "Hidden");
			AddString(RichEditControlStringId.Caption_FontFormUnderlineWordsOnlyCheckEdit, "Underline words only");
			AddString(RichEditControlStringId.Caption_FontFormScriptNormalRadioButton, "Normal");
			AddString(RichEditControlStringId.Caption_FontFormSuperscriptRadioButton, "Superscript");
			AddString(RichEditControlStringId.Caption_FontFormSubscriptRadioButton, "Subscript");
			AddString(RichEditControlStringId.Caption_HyperlinkFormTextToDisplayLabel, "Text to display:");
			AddString(RichEditControlStringId.Caption_HyperlinkFormScreenTipLabel, "ScreenTip:");
			AddString(RichEditControlStringId.Caption_HyperlinkFormTargetFrameLabel, "Target frame:");
			AddString(RichEditControlStringId.Caption_HyperlinkFormLinkToLabel, "Link to:");
			AddString(RichEditControlStringId.Caption_HyperlinkFormExistingFileOrWebPageRadioButton, "Existing file or web page");
			AddString(RichEditControlStringId.Caption_HyperlinkFormPlaceInThisDocumentRadioButton, "Place in this document");
			AddString(RichEditControlStringId.Caption_HyperlinkFormAddressLabel, "Address:");
			AddString(RichEditControlStringId.Caption_InsertTableCellsForm, "Insert Cells");
			AddString(RichEditControlStringId.Caption_InsertTableCellsFormShiftCellsRightRadioButton, "Shift cells right");
			AddString(RichEditControlStringId.Caption_InsertTableCellsFormShiftCellsDownRadioButton, "Shift cells down");
			AddString(RichEditControlStringId.Caption_InsertTableCellsFormInsertEntireRowRadioButton, "Insert entire row");
			AddString(RichEditControlStringId.Caption_InsertTableCellsFormInsertEntireColumnRadioButton, "Insert entire column");
			AddString(RichEditControlStringId.Caption_InsertTableForm, "Insert Table");
			AddString(RichEditControlStringId.Caption_InsertTableFormTableSizeLabel, "Table Size");
			AddString(RichEditControlStringId.Caption_InsertTableFormNumberOfColumnsLabel, "Number of columns:");
			AddString(RichEditControlStringId.Caption_InsertTableFormNumberOfRowsLabel, "Number of rows:");
			AddString(RichEditControlStringId.Caption_ParagraphForm, "Paragraph");
			AddString(RichEditControlStringId.Caption_ParagraphFormGeneralLabel, "General");
			AddString(RichEditControlStringId.Caption_ParagraphFormAlignmentLabel, "Alignment:");
			AddString(RichEditControlStringId.Caption_ParagraphFormOutlineLevelLabel, "Outline level:");
			AddString(RichEditControlStringId.Caption_ParagraphFormIndentationLabel, "Indentation");
			AddString(RichEditControlStringId.Caption_ParagraphFormSpacingLabel, "Spacing");
			AddString(RichEditControlStringId.Caption_ParagraphFormTabsButton, "Tabs...");
			AddString(RichEditControlStringId.Caption_ParagraphFormPaginationLabel, "Pagination");
			AddString(RichEditControlStringId.Caption_ParagraphFormIndentsAndSpacing, "Indents And Spacing");
			AddString(RichEditControlStringId.Caption_ParagraphFormLineAndPageBreaks, "Line And Page Breaks");
			AddString(RichEditControlStringId.Caption_ParagraphFormKeepLinesTogetherLabel, "Keep lines together");
			AddString(RichEditControlStringId.Caption_ParagraphFormPageBreakBeforeLabel, "Page break before");
			AddString(RichEditControlStringId.Caption_ParagraphFormContextualSpacingCheckEdit, "Don't add space between paragraphs of the same style");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutForm, "Layout");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormPosition, "Position");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormHorizontalLabel, "Horizontal");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormVerticalLabel, "Vertical");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormOptionsLabel, "Options");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTextWrapping, "Text Wrapping");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormSize, "Size");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormHorizontalAbsolutePositionRadioButton, "Absolute position");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormHorizontalPositionAlignmentRadioButton, "Alignment");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormHorizontalPositionTypeLabel, "relative to");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormHorizontalAbsolutePositionLabel, "to the right of");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormVerticalPositionAlignmentRadioButton, "Alignment");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormVerticalAbsolutePositionRadioButton, "Absolute position");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormVerticalPositionTypeLabel, "relative to");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormVerticalAbsolutePositionLabel, "below");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormLockAnchorCheckEdit, "Lock anchor");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormWrappingStyleLabel, "Wrapping style");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormWrapTextLabel, "Wrap text");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormBothSidesRadioButton, "Both sides");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormLeftOnlyRadioButton, "Left only");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormRightOnlyRadioButton, "Right only");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormLargestOnlyRadioButton, "Largest only");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormDistanceFromTextLabel, "Distance from text");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTopLabel, "Top");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormBottomLabel, "Bottom");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormLeftLabel, "Left");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormRightLabel, "Right");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormRotationLabel, "Rotation:");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormLockAspectRatioCheckEdit, "Lock aspect ratio");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormSquarePreset, "Square");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTightPreset, "Tight");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormThoughtPreset, "Through");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTopAndBottomPreset, "Top and bottom");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormBehindTextPreset, "Behind text");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormInFrontOfTextPreset, "In front of text");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormRotateLabel, "Rotate");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormScaleLabel, "Scale");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTabSizeHeightLabel, "Height");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTabSizeHeightAbsoluteLabel, "Absolute");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTabSizeWidthLabel, "Width");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTabSizeWidthAbsoluteLabel, "Absolute");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTabSizeOriginalSizeLabel, "Original size");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTabSizeOriginalSizeWidthLabel, "Width:");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTabSizeOriginalSizeHeightLabel, "Height:");
			AddString(RichEditControlStringId.Caption_FloatingObjectLayoutFormTabSizeResetButton, "Reset");
			AddString(RichEditControlStringId.Caption_EditStyleForm, "Edit style");
			AddString(RichEditControlStringId.Caption_EditStyleFormFormatButton, "Format");
			AddString(RichEditControlStringId.Caption_EditStyleFormFontButton, "Font...");
			AddString(RichEditControlStringId.Caption_EditStyleFormParagraphButton, "Paragraph...");
			AddString(RichEditControlStringId.Caption_EditStyleFormTabsButton, "Tabs...");
			AddString(RichEditControlStringId.Caption_EditStyleFormStyleNameTextEdit, "Name:");
			AddString(RichEditControlStringId.Caption_EditStyleFormCurrentStyleNameComboBox, "Current style:");
			AddString(RichEditControlStringId.Caption_EditStyleFormParentStyleNameComboBox, "Style based on:");
			AddString(RichEditControlStringId.Caption_EditStyleFormNextStyleNameComboBox, "Style for following paragraph:");
			AddString(RichEditControlStringId.Caption_EditStyleFormLineSpacingBarSubItem, "Line Spacing");
			AddString(RichEditControlStringId.Caption_EditStyleFormBoldBarCheckItemDescription, "Bold");
			AddString(RichEditControlStringId.Caption_EditStyleFormItalicBarCheckItemDescription, "Italic");
			AddString(RichEditControlStringId.Caption_EditStyleFormUnderlineBarCheckItemDescription, "Underline");
			AddString(RichEditControlStringId.Caption_EditStyleFormLeftBarCheckItemDescription, "Left");
			AddString(RichEditControlStringId.Caption_EditStyleFormCenterBarCheckItemDescription, "Center");
			AddString(RichEditControlStringId.Caption_EditStyleFormRightBarCheckItemDescription, "Right");
			AddString(RichEditControlStringId.Caption_EditStyleFormJustifyBarCheckItemDescription, "Justify");
			AddString(RichEditControlStringId.Caption_EditStyleFormIncreaseParagraphSpacingBarCheckItemDescription, "Increase Paragraph Spacing");
			AddString(RichEditControlStringId.Caption_EditStyleFormDecreaseParagraphSpacingBarCheckItemDescription, "Decrease Paragraph Spacing");
			AddString(RichEditControlStringId.Caption_EditStyleFormDecreaseIndentBarCheckItemDescription, "Decrease Indent");
			AddString(RichEditControlStringId.Caption_EditStyleFormIncreaseIndentBarCheckItemDescription, "Increase Indent");
			AddString(RichEditControlStringId.Caption_ParagraphIndentationLeftLabel, "Left:");
			AddString(RichEditControlStringId.Caption_ParagraphIndentationRightLabel, "Right:");
			AddString(RichEditControlStringId.Caption_ParagraphIndentationSpecialLabel, "Special:");
			AddString(RichEditControlStringId.Caption_ParagraphIndentationByLabel, "By:");
			AddString(RichEditControlStringId.Caption_ParagraphSpacingBeforeLabel, "Before:");
			AddString(RichEditControlStringId.Caption_ParagraphSpacingAfterLabel, "After:");
			AddString(RichEditControlStringId.Caption_ParagraphSpacingLineSpacingLabel, "Line spacing:");
			AddString(RichEditControlStringId.Caption_ParagraphSpacingAtLabel, "At:");
			AddString(RichEditControlStringId.Caption_RangeEditingPermissionsForm, "Editing Permissions");
			AddString(RichEditControlStringId.Caption_RangeEditingPermissionsFormUsersLabel, "Users:");
			AddString(RichEditControlStringId.Caption_RangeEditingPermissionsFormGroupsLabel, "Groups:");
			AddString(RichEditControlStringId.Caption_SplitTableCellsForm, "Split Cells");
			AddString(RichEditControlStringId.Caption_SplitTableCellsFormNumberOfColumnsLable, "Number of columns:");
			AddString(RichEditControlStringId.Caption_SplitTableCellsFormNumberOfRowsLable, "Number of rows:");
			AddString(RichEditControlStringId.Caption_SplitTableCellsFormMergeCellsBeforeSplitCheckEdit, "Merge cells before split");
			AddString(RichEditControlStringId.Msg_Warning, "Warning");
			AddString(RichEditControlStringId.Caption_TabsForm, "Tabs");
			AddString(RichEditControlStringId.Caption_TabsFormTabStopPositionLabel, "Tab stop position:");
			AddString(RichEditControlStringId.Caption_TabsFormDefaultTabStopsLabel, "Default tab stops:");
			AddString(RichEditControlStringId.Caption_TabsFormTabStopsToBeClearedLabel, "Tab stops to be cleared:");
			AddString(RichEditControlStringId.Caption_TabsFormAlignmentLabel, "Alignment");
			AddString(RichEditControlStringId.Caption_TabsFormLeftRadioButton, "Left");
			AddString(RichEditControlStringId.Caption_TabsFormRightRadioButton, "Right");
			AddString(RichEditControlStringId.Caption_TabsFormDecimalRadionButton, "Decimal");
			AddString(RichEditControlStringId.Caption_TabsFormCenterRadioButton, "Center");
			AddString(RichEditControlStringId.Caption_TabsFormLeaderLabel, "Leader");
			AddString(RichEditControlStringId.Caption_TabsFormNoneRadioButton, "None");
			AddString(RichEditControlStringId.Caption_TabsFormHyphensRadioButton, "Hyphens");
			AddString(RichEditControlStringId.Caption_TabsFormEqualSignRadioButton, "Equal sign");
			AddString(RichEditControlStringId.Caption_TabsFormDotsRadioButton, "Dots");
			AddString(RichEditControlStringId.Caption_TabsFormUnderlineRadioButton, "Underline");
			AddString(RichEditControlStringId.Caption_TabsFormMiddleDotsRadioButton, "Middle dots");
			AddString(RichEditControlStringId.Caption_TabsFormThickLineRadioButton, "Thick line");
			AddString(RichEditControlStringId.Caption_TabsFormSetButton, "Set");
			AddString(RichEditControlStringId.Caption_TabsFormClearButton, "Clear");
			AddString(RichEditControlStringId.Caption_TabsFormClearAllButton, "Clear All");
			AddString(RichEditControlStringId.Caption_LineNumberingForm, "Line Numbers");
			AddString(RichEditControlStringId.Caption_LineNumberingFormAddLineNumberingCheckEdit, "Add line numbering");
			AddString(RichEditControlStringId.Caption_LineNumberingStartAtLabel, "Start at:");
			AddString(RichEditControlStringId.Caption_LineNumberingCountByLabel, "Count by:");
			AddString(RichEditControlStringId.Caption_LineNumberingFromTextLabel, "From text:");
			AddString(RichEditControlStringId.Caption_LineNumberingNumberingLabel, "Numbering:");
			AddString(RichEditControlStringId.Caption_LineNumberingRestartEachPageRadioButton, "Restart each page");
			AddString(RichEditControlStringId.Caption_LineNumberingRestartEachSectionRadioButton, "Restart each section");
			AddString(RichEditControlStringId.Caption_LineNumberingContinuousRadioButton, "Continuous");
			AddString(RichEditControlStringId.Caption_PasteSpecialForm, "Paste Special");
			AddString(RichEditControlStringId.Caption_PasteSpecialPasteAsLabel, "Paste As:");
			AddString(RichEditControlStringId.Caption_PageSetupForm, "Page Setup");
			AddString(RichEditControlStringId.Caption_PageSetupMarginsTab, "Margins");
			AddString(RichEditControlStringId.Caption_PageSetupPaperTab, "Paper");
			AddString(RichEditControlStringId.Caption_PageSetupLayoutTab, "Layout");
			AddString(RichEditControlStringId.Caption_PageSetupMarginsLabel, "Margins");
			AddString(RichEditControlStringId.Caption_PageSetupTopMarginLabel, "Top:");
			AddString(RichEditControlStringId.Caption_PageSetupBottomMarginLabel, "Bottom:");
			AddString(RichEditControlStringId.Caption_PageSetupLeftMarginLabel, "Left:");
			AddString(RichEditControlStringId.Caption_PageSetupRightMarginLabel, "Right:");
			AddString(RichEditControlStringId.Caption_PageSetupOrientationLabel, "Orientation");
			AddString(RichEditControlStringId.Caption_PageSetupOrientationPortraitRadioButton, "Portrait");
			AddString(RichEditControlStringId.Caption_PageSetupOrientationLandscapeRadioButton, "Landscape");
			AddString(RichEditControlStringId.Caption_PageSetupApplyToLabel, "Apply To:");
			AddString(RichEditControlStringId.Caption_PageSetupPaperSizeLabel, "Paper size");
			AddString(RichEditControlStringId.Caption_PageSetupPaperWidthLabel, "Width:");
			AddString(RichEditControlStringId.Caption_PageSetupPaperHeightLabel, "Height:");
			AddString(RichEditControlStringId.Caption_PageSetupSectionLabel, "Section");
			AddString(RichEditControlStringId.Caption_PageSetupSectionStartLabel, "Section start:");
			AddString(RichEditControlStringId.Caption_PageSetupHeadersAndFootersLabel, "Headers and footers");
			AddString(RichEditControlStringId.Caption_PageSetupDifferentOddAndEvenLabel, "Different odd and even");
			AddString(RichEditControlStringId.Caption_PageSetupDifferentFirstPageLabel, "Different first page");
			AddString(RichEditControlStringId.Caption_TableOptionsForm, "Table Options");
			AddString(RichEditControlStringId.Caption_TableOptionsFormDefaultCellMarginsLabel, "Default cell margins");
			AddString(RichEditControlStringId.Caption_TableOptionsFormTopLabel, "Top:");
			AddString(RichEditControlStringId.Caption_TableOptionsFormLeftLabel, "Left:");
			AddString(RichEditControlStringId.Caption_TableOptionsFormRightLabel, "Right:");
			AddString(RichEditControlStringId.Caption_TableOptionsFormBottomLabel, "Bottom:");
			AddString(RichEditControlStringId.Caption_TableOptionsFormDefaultCellSpacingLabel, "Default cell spacing");
			AddString(RichEditControlStringId.Caption_TableOptionsFormAllowSpacingBetweenCellsCheckEdit, "Allow spacing between cells");
			AddString(RichEditControlStringId.Caption_TableOptionsFormOptionsLabel, "Options");
			AddString(RichEditControlStringId.Caption_TableOptionsFormAutomaticallyResizeCheckEdit, "Automatically resize to fit contents");
			AddString(RichEditControlStringId.Caption_TableCellOptionsForm, "Cell Options");
			AddString(RichEditControlStringId.Caption_TableCellOptionsFormCellMarginsLabel, "Cell margins");
			AddString(RichEditControlStringId.Caption_TableCellOptionsFormSameAsWholeTableCheckEdit, "Same as the whole table");
			AddString(RichEditControlStringId.Caption_TableCellOptionsFormTopLabel, "Top:");
			AddString(RichEditControlStringId.Caption_TableCellOptionsFormLeftLabel, "Left:");
			AddString(RichEditControlStringId.Caption_TableCellOptionsFormBottomLabel, "Bottom:");
			AddString(RichEditControlStringId.Caption_TableCellOptionsFormRightLabel, "Right:");
			AddString(RichEditControlStringId.Caption_TableCellOptionsFormOptionsLabel, "Options");
			AddString(RichEditControlStringId.Caption_TableCellOptionsFormWrapTextCheckEdit, "Wrap text");
			AddString(RichEditControlStringId.Caption_TableCellOptionsFormFitTextCheckEdit, "Fit text");
			AddString(RichEditControlStringId.Caption_TablePropertiesForm, "Table Properties");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormTableTab, "Table");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormRowTab, "Row");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormColumnTab, "Column");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormCellTab, "Cell");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormSizeLabel, "Size");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormTableAlignmentLabel, "Alignment");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormLeftRadioButton, "Left");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormCenterRadioButton, "Center");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormRightRadioButton, "Right");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormTableIndentLabel, "Indent from left:");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormOptionsButton, "Options...");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormBordersButton, "Borders and Shading...");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormRowLabel, "Row");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormOptionsLabel, "Options");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormAllowRowToBreakAcrossPagesCheckEdit, "Allow row to break across pages");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormRepeatHeaderCheckEdit, "Repeat as header row at the top of each page");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormColumnLabel, "Column");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormCellVerticalAlignmentLabel, "Vertical alignment");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormTopRadioButton, "Top");
			AddString(RichEditControlStringId.Caption_TablePropertiesFormBottomRadioButton, "Bottom");
			AddString(RichEditControlStringId.Caption_TableSizePreferredWidthCheckEdit, "Preferred width:");
			AddString(RichEditControlStringId.Caption_TableSizeMeasureInLabel, "Measure in:");
			AddString(RichEditControlStringId.Caption_TableRowHeightSpecifyHeightCheckEdit, "Specify height:");
			AddString(RichEditControlStringId.Caption_TableRowHeightHeightIsLabel, "Row height is:");
			AddString(RichEditControlStringId.Caption_BorderShadingForm, "Borders and Shading");
			AddString(RichEditControlStringId.Caption_BorderShadingFormAllLabel, "All");
			AddString(RichEditControlStringId.Caption_BorderShadingFormBordersTab, "Borders");
			AddString(RichEditControlStringId.Caption_BorderShadingFormBoxLabel, "Box");
			AddString(RichEditControlStringId.Caption_BorderShadingFormCustomLabel, "Custom");
			AddString(RichEditControlStringId.Caption_BorderShadingFormFillLabel, "Fill");
			AddString(RichEditControlStringId.Caption_BorderShadingFormGridLabel, "Grid");
			AddString(RichEditControlStringId.Caption_BorderShadingFormNoneLabel, "None");
			AddString(RichEditControlStringId.Caption_BorderShadingFormSettingsLabel, "Setting:");
			AddString(RichEditControlStringId.Caption_BorderShadingFormShadingTab, "Shading");
			AddString(RichEditControlStringId.Caption_BorderShadingUserControlPreviewLabel, "Preview");
			AddString(RichEditControlStringId.Caption_BorderShadingUserControlClickLabel, "Click on diagram below or use");
			AddString(RichEditControlStringId.Caption_BorderShadingUserControlButtonsLabel, "buttons to apply  borders");
			AddString(RichEditControlStringId.Caption_BorderShadingUserControlApplyLabel, "Apply to:");
			AddString(RichEditControlStringId.Caption_BorderShadingUserControlOptionsButton, "Options");
			AddString(RichEditControlStringId.Caption_BorderShadingTypeLineUserControlColorLabel, "Color:");
			AddString(RichEditControlStringId.Caption_BorderShadingTypeLineUserControlStyleLabel, "Style:");
			AddString(RichEditControlStringId.Caption_BorderShadingTypeLineUserControlWidthLabel, "Width:");
			AddString(RichEditControlStringId.Caption_ColumnsSetupForm, "Columns");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormColumnNumberLabel, "Col #:");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormColumnWidthLabel, "Width:");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormColumnSpacingLabel, "Spacing:");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormApplyToLabel, "Apply to:");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormPresetsGroup, "Presets");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormWidthAndSpacingGroup, "Width and spacing");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormNumberOfColumnsLabel, "Number of columns:");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormEqualColumnWidthCheckEdit, "Equal column width");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormOneColumnPreset, "One");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormTwoColumnsPreset, "Two");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormThreeColumnsPreset, "Three");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormLeftNarrowColumnPreset, "Left");
			AddString(RichEditControlStringId.Caption_ColumnsSetupFormRightNarrowColumnPreset, "Right");
			AddString(RichEditControlStringId.Caption_SymbolForm, "Symbol");
			AddString(RichEditControlStringId.Caption_SymbolFormSearchByCode, "Search by code:");
			AddString(RichEditControlStringId.Caption_SymbolFormFontName, "Font name:");
			AddString(RichEditControlStringId.Caption_SymbolFormCharacterSet, "Character set:");
			AddString(RichEditControlStringId.Caption_SymbolFormFilter, "Filter:");
			AddString(RichEditControlStringId.Caption_PageCategoryHeaderFooterTools, "Header & Footer Tools");
			AddString(RichEditControlStringId.Caption_PageCategoryTableTools, "Table Tools");
			AddString(RichEditControlStringId.Caption_PageFile, "File");
			AddString(RichEditControlStringId.Caption_PageHome, "Home");
			AddString(RichEditControlStringId.Caption_PageInsert, "Insert");
			AddString(RichEditControlStringId.Caption_PageMailings, "Mail Merge");
			AddString(RichEditControlStringId.Caption_PageView, "View");
			AddString(RichEditControlStringId.Caption_PageHeaderFooterToolsDesign, "Design");
			AddString(RichEditControlStringId.Caption_PagePageLayout, "Page Layout");
			AddString(RichEditControlStringId.Caption_PageTableLayout, "Layout");
			AddString(RichEditControlStringId.Caption_PageTableDesign, "Design");
			AddString(RichEditControlStringId.Caption_PageReview, "Review");
			AddString(RichEditControlStringId.Caption_PageReferences, "References");
			AddString(RichEditControlStringId.Caption_PageFloatingObjectPictureToolsFormat, "Format");
			AddString(RichEditControlStringId.Caption_GroupFont, "Font");
			AddString(RichEditControlStringId.Caption_GroupParagraph, "Paragraph");
			AddString(RichEditControlStringId.Caption_GroupClipboard, "Clipboard");
			AddString(RichEditControlStringId.Caption_GroupEditing, "Editing");
			AddString(RichEditControlStringId.Caption_GroupCommon, "Common");
			AddString(RichEditControlStringId.Caption_GroupStyles, "Styles");
			AddString(RichEditControlStringId.Caption_GroupZoom, "Zoom");
			AddString(RichEditControlStringId.Caption_GroupShow, "Show");
			AddString(RichEditControlStringId.Caption_GroupIllustrations, "Illustrations");
			AddString(RichEditControlStringId.Caption_GroupText, "Text");
			AddString(RichEditControlStringId.Caption_GroupTables, "Tables");
			AddString(RichEditControlStringId.Caption_GroupTableTable, "Table");
			AddString(RichEditControlStringId.Caption_GroupTableRowsAndColumns, "Rows & Columns");
			AddString(RichEditControlStringId.Caption_GroupTableMerge, "Merge");
			AddString(RichEditControlStringId.Caption_GroupTableCellSize, "Cell Size");
			AddString(RichEditControlStringId.Caption_GroupTableAlignment, "Alignment");
			AddString(RichEditControlStringId.Caption_GroupTableStyles, "Table Styles");
			AddString(RichEditControlStringId.Caption_GroupTableDrawBorders, "Draw Borders");
			AddString(RichEditControlStringId.Caption_GroupSymbols, "Symbols");
			AddString(RichEditControlStringId.Caption_GroupLinks, "Links");
			AddString(RichEditControlStringId.Caption_GroupPages, "Pages");
			AddString(RichEditControlStringId.Caption_GroupHeaderFooter, "Header & Footer");
			AddString(RichEditControlStringId.Caption_GroupHeaderFooterToolsDesignClose, "Close");
			AddString(RichEditControlStringId.Caption_GroupHeaderFooterToolsDesignNavigation, "Navigation");
			AddString(RichEditControlStringId.Caption_GroupMailMerge, "Mail Merge");
			AddString(RichEditControlStringId.Caption_GroupDocumentViews, "Document Views");
			AddString(RichEditControlStringId.Caption_GroupHeaderFooterToolsDesignOptions, "Options");
			AddString(RichEditControlStringId.Caption_GroupPageSetup, "Page Setup");
			AddString(RichEditControlStringId.Caption_GroupPageBackground, "Page Background");
			AddString(RichEditControlStringId.Caption_GroupDocumentProtection, "Protect");
			AddString(RichEditControlStringId.Caption_GroupDocumentProofing, "Proofing");
			AddString(RichEditControlStringId.Caption_GroupLanguage, "Language");
			AddString(RichEditControlStringId.Caption_GroupComment, "Comment");
			AddString(RichEditControlStringId.Caption_GroupTracking, "Tracking");
			AddString(RichEditControlStringId.Caption_GroupTableOfContents, "Table of Contents");
			AddString(RichEditControlStringId.Caption_GroupCaptions, "Captions");
			AddString(RichEditControlStringId.Caption_GroupWriteInsertFields, "Write & Insert Fields");
			AddString(RichEditControlStringId.Caption_GroupPreviewResults, "Preview Results");
			AddString(RichEditControlStringId.Caption_PageCategoryFloatingObjectPictureTools, "Picture Tools");
			AddString(RichEditControlStringId.Caption_GroupFloatingPictureToolsArrange, "Arrange");
			AddString(RichEditControlStringId.Caption_GroupFloatingPictureToolsShapeStyles, "Shape Styles");
			AddString(RichEditControlStringId.Caption_CommonCharactersToggleButton, "Common Characters");
			AddString(RichEditControlStringId.Caption_SpecialCharactersToggleButton, "Special Characters");
			AddString(RichEditControlStringId.Caption_StyleGalleryItemText, "AaBbCcDdEe");
			AddString(RichEditControlStringId.Caption_ReviewingPaneForm, "Reviewing Pane");
			AddString(RichEditControlStringId.Msg_TargetTypeNotSupported, "The specified target type is not supported.");			
		}
		#endregion
		public static XtraLocalizer<RichEditControlStringId> CreateDefaultLocalizer() {
			return new XpfRichEditResLocalizer();
		}
		public static string GetString(RichEditControlStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<RichEditControlStringId> CreateResXLocalizer() {
			return new XpfRichEditResLocalizer();
		}
	}
	#endregion
	#region XpfRichEditResLocalizer
	public class XpfRichEditResLocalizer : DXResXLocalizer<RichEditControlStringId> {
		public XpfRichEditResLocalizer()
			: base(new XpfRichEditLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraRichEdit.LocalizationRes", typeof(XpfRichEditResLocalizer).Assembly);
		}
	}
	#endregion
}
