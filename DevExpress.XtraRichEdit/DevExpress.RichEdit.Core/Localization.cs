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
using System.Reflection;
using System.Resources;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization.Internal;
namespace DevExpress.XtraRichEdit.Localization {
	#region XtraRichEditStringId
	public enum XtraRichEditStringId {
		Msg_IsNotValid,
		Msg_UnsupportedDocVersion,
		Msg_EncryptedFile,
		Msg_MagicNumberNotFound,
		Msg_InternalError,
		Msg_UseDeletedStyleError,
		Msg_UseDeletedParagraphError,
		Msg_UseDeletedFieldError,
		Msg_UseDeletedSectionError,
		Msg_UseDeletedBookmarkError,
		Msg_UseDeletedCommentError,
		Msg_UseDeletedCustomMarkError,
		Msg_UseDeletedHyperlinkError,
		Msg_UseDeletedTableError,
		Msg_UseDeletedTableRowError,
		Msg_UseDeletedTableCellError,
		Msg_UseInvalidParagraphProperties,
		Msg_UseInvalidCharacterProperties,
		Msg_DocumentPositionDoesntMatchDocument,
		Msg_UseInvalidDocument,
		Msg_UnsupportedFormatException,
		Msg_PrintingUnavailable,
		Msg_CreateHyperlinkError,
		Msg_SelectBookmarkError,
		Msg_SelectCommentError,
		Msg_IncorrectBookmarkName,
		Msg_InvalidNavigateUri,
		Msg_IncorrectPattern,
		Msg_DocumentProtectionInvalidPassword,
		Msg_DocumentProtectionInvalidPasswordConfirmation,
		Msg_TopBottomSectionMarginsTooLarge,
		Msg_LeftRightSectionMarginsTooLarge,
		Msg_InvalidNumberingListIndex,
		Msg_InvalidImageFile,
		MenuCmd_SimpleView,
		MenuCmd_DraftView,
		MenuCmd_ReadingView,
		MenuCmd_PrintView,
		MenuCmd_SimpleViewDescription,
		MenuCmd_DraftViewDescription,
		MenuCmd_ReadingViewDescription,
		MenuCmd_PrintViewDescription,
		MenuCmd_FitToPage,
		MenuCmd_FitToPageDescription,
		MenuCmd_FitHeight,
		MenuCmd_FitHeightDescription,
		MenuCmd_FitWidth,
		MenuCmd_FitWidthDescription,
		MenuCmd_Zoom,
		MenuCmd_ZoomDescription,
		MenuCmd_ZoomIn,
		MenuCmd_ZoomInDescription,
		MenuCmd_ZoomOut,
		MenuCmd_ZoomOutDescription,
		MenuCmd_Undo,
		MenuCmd_UndoDescription,
		MenuCmd_Redo,
		MenuCmd_RedoDescription,
		MenuCmd_ClearUndo,
		MenuCmd_ClearUndoDescription,
		MenuCmd_ScrollDown,
		MenuCmd_ScrollDownDescription,
		MenuCmd_EnsureCaretVisibleVertically,
		MenuCmd_EnsureCaretVisibleVerticallyDescription,
		MenuCmd_EnsureCaretVisibleHorizontally,
		MenuCmd_EnsureCaretVisibleHorizontallyDescription,
		MenuCmd_MoveForward,
		MenuCmd_MoveForwardDescription,
		MenuCmd_MoveBackward,
		MenuCmd_MoveBackwardDescription,
		MenuCmd_MoveToStartOfLine,
		MenuCmd_MoveToStartOfLineDescription,
		MenuCmd_MoveToEndOfLine,
		MenuCmd_MoveToEndOfLineDescription,
		MenuCmd_MoveLineUp,
		MenuCmd_MoveLineUpDescription,
		MenuCmd_MoveLineDown,
		MenuCmd_MoveLineDownDescription,
		MenuCmd_MovePreviousParagraph,
		MenuCmd_MovePreviousParagraphDescription,
		MenuCmd_MoveNextParagraph,
		MenuCmd_MoveNextParagraphDescription,
		MenuCmd_MovePreviousWord,
		MenuCmd_MovePreviousWordDescription,
		MenuCmd_MoveNextWord,
		MenuCmd_MoveNextWordDescription,
		MenuCmd_MovePreviousPage,
		MenuCmd_MovePreviousPageDescription,
		MenuCmd_MoveNextPage,
		MenuCmd_MoveNextPageDescription,
		MenuCmd_MoveToBeginOfDocument,
		MenuCmd_MoveToBeginOfDocumentDescription,
		MenuCmd_MoveToEndOfDocument,
		MenuCmd_MoveToEndOfDocumentDescription,
		MenuCmd_MoveScreenUp,
		MenuCmd_MoveScreenUpDescription,
		MenuCmd_MoveScreenDown,
		MenuCmd_MoveScreenDownDescription,
		MenuCmd_SelectAll,
		MenuCmd_SelectAllDescription,
		MenuCmd_DeselectAll,
		MenuCmd_DeselectAllDescription,
		MenuCmd_SymbolFormInsertButton,
		MenuCmd_InsertTableElement,
		MenuCmd_InsertParagraph,
		MenuCmd_InsertParagraphDescription,
		MenuCmd_InsertLineBreak,
		MenuCmd_InsertLineBreakDescription,
		MenuCmd_InsertText,
		MenuCmd_InsertTextDescription,
		MenuCmd_OvertypeText,
		MenuCmd_OvertypeTextDescription,
		MenuCmd_InsertBulletList,
		MenuCmd_InsertBulletListDescription,
		MenuCmd_InsertMultilevelList,
		MenuCmd_InsertMultilevelListDescription,
		MenuCmd_InsertSimpleList,
		MenuCmd_InsertSimpleListDescription,
		MenuCmd_InsertField,
		MenuCmd_InsertFieldDescription,
		MenuCmd_InsertPageNumberField,
		MenuCmd_InsertPageNumberFieldDescription,
		MenuCmd_InsertPageCountField,
		MenuCmd_InsertPageCountFieldDescription,
		MenuCmd_InsertMergeField,
		MenuCmd_InsertMergeFieldDescription,
		MenuCmd_InsertTabToParagraph,
		MenuCmd_InsertTabToParagraphDescription,
		MenuCmd_InsertTable,
		MenuCmd_InsertTableDescription,
		MenuCmd_InsertTableRowAbove,
		MenuCmd_InsertTableRowAboveDescription,
		MenuCmd_InsertTableRowBelow,
		MenuCmd_InsertTableRowBelowDescription,
		MenuCmd_InsertTableCells,
		MenuCmd_InsertTableCellsDescription,
		MenuCmd_DeleteTableCells,
		MenuCmd_DeleteTableCellsMenuItem,
		MenuCmd_DeleteTableCellsDescription,
		MenuCmd_DeleteTableRows,
		MenuCmd_DeleteTableRowsDescription,
		MenuCmd_DeleteTable,
		MenuCmd_DeleteTableDescription,
		MenuCmd_DeleteTableElements,
		MenuCmd_DeleteTableElementsDescription,
		MenuCmd_DeleteTableColumns,
		MenuCmd_DeleteTableColumnsDescription,
		MenuCmd_ShowInsertMergeFieldForm,
		MenuCmd_ShowInsertMergeFieldFormDescription,
		MenuCmd_DeleteNumerationFromParagraph,
		MenuCmd_DeleteNumerationFromParagraphDescription,
		MenuCmd_IncrementNumerationFromParagraph,
		MenuCmd_IncrementNumerationFromParagraphDescription,
		MenuCmd_DecrementNumerationFromParagraph,
		MenuCmd_DecrementNumerationFromParagraphDescription,
		MenuCmd_TabKey,
		MenuCmd_TabKeyDescription,
		MenuCmd_ShiftTabKey,
		MenuCmd_ShiftTabKeyDescription,
		MenuCmd_BackSpaceKey,
		MenuCmd_BackSpaceKeyDescription,
		MenuCmd_InsertTab,
		MenuCmd_InsertTabDescription,
		MenuCmd_InsertPageBreak,
		MenuCmd_InsertPageBreakDescription,
		MenuCmd_InsertPageBreak2,
		MenuCmd_InsertPageBreak2Description,
		MenuCmd_InsertNonBreakingSpace,
		MenuCmd_InsertNonBreakingSpaceDescription,
		MenuCmd_InsertColumnBreak,
		MenuCmd_InsertColumnBreakDescription,
		MenuCmd_InsertEnDash,
		MenuCmd_InsertEnDashDescription,
		MenuCmd_InsertEmDash,
		MenuCmd_InsertEmDashDescription,
		MenuCmd_InsertCopyrightSymbol,
		MenuCmd_InsertCopyrightSymbolDescription,
		MenuCmd_InsertRegisteredTrademarkSymbol,
		MenuCmd_InsertRegisteredTrademarkSymbolDescription,
		MenuCmd_InsertTrademarkSymbol,
		MenuCmd_InsertTrademarkSymbolDescription,
		MenuCmd_InsertEllipsis,
		MenuCmd_InsertEllipsisDescription,
		MenuCmd_InsertOpeningSingleQuotationMark,
		MenuCmd_InsertOpeningSingleQuotationMarkDescription,
		MenuCmd_InsertClosingSingleQuotationMark,
		MenuCmd_InsertClosingSingleQuotationMarkDescription,
		MenuCmd_InsertOpeningDoubleQuotationMark,
		MenuCmd_InsertOpeningDoubleQuotationMarkDescription,
		MenuCmd_InsertClosingDoubleQuotationMark,
		MenuCmd_InsertClosingDoubleQuotationMarkDescription,
		MenuCmd_InsertSymbol,
		MenuCmd_InsertSymbolDescription,
		MenuCmd_InsertPicture,
		MenuCmd_InsertPictureDescription,
		MenuCmd_InsertBreak,
		MenuCmd_InsertBreakDescription,
		MenuCmd_InsertSectionBreakNextPage,
		MenuCmd_InsertSectionBreakNextPageDescription,
		MenuCmd_InsertSectionBreakOddPage,
		MenuCmd_InsertSectionBreakOddPageDescription,
		MenuCmd_InsertSectionBreakEvenPage,
		MenuCmd_InsertSectionBreakEvenPageDescription,
		MenuCmd_InsertSectionBreakContinuous,
		MenuCmd_InsertSectionBreakContinuousDescription,
		MenuCmd_ToggleFontBold,
		MenuCmd_ToggleFontBoldDescription,
		MenuCmd_ToggleFontItalic,
		MenuCmd_ToggleFontItalicDescription,
		MenuCmd_ToggleHiddenText,
		MenuCmd_ToggleHiddenTextDescription,
		MenuCmd_ToggleFontUnderline,
		MenuCmd_ToggleFontUnderlineDescription,
		MenuCmd_ToggleFontDoubleUnderline,
		MenuCmd_ToggleFontDoubleUnderlineDescription,
		MenuCmd_ToggleFontStrikeout,
		MenuCmd_ToggleFontStrikeoutDescription,
		MenuCmd_ToggleFontDoubleStrikeout,
		MenuCmd_ToggleFontDoubleStrikeoutDescription,
		MenuCmd_IncrementFontSize,
		MenuCmd_IncrementFontSizeDescription,
		MenuCmd_DecrementFontSize,
		MenuCmd_DecrementFontSizeDescription,
		MenuCmd_ChangeFontColor,
		MenuCmd_ChangeFontColorDescription,
		MenuCmd_HighlightText,
		MenuCmd_HighlightTextDescription,
		MenuCmd_ChangeFontName,
		MenuCmd_ChangeFontNameDescription,
		MenuCmd_ChangeFontSize,
		MenuCmd_ChangeFontSizeDescription,
		MenuCmd_ChangeStyle,
		MenuCmd_ChangeStyleDescription,
		MenuCmd_ChangeColumnCount,
		MenuCmd_ChangeColumnCountDescription,
		MenuCmd_IncreaseFontSize,
		MenuCmd_IncreaseFontSizeDescription,
		MenuCmd_DecreaseFontSize,
		MenuCmd_DecreaseFontSizeDescription,
		MenuCmd_FontSuperscript,
		MenuCmd_FontSuperscriptDescription,
		MenuCmd_FontSubscript,
		MenuCmd_FontSubscriptDescription,
		MenuCmd_ParagraphAlignmentLeft,
		MenuCmd_ParagraphAlignmentLeftDescription,
		MenuCmd_ParagraphAlignmentCenter,
		MenuCmd_ParagraphAlignmentCenterDescription,
		MenuCmd_ParagraphAlignmentRight,
		MenuCmd_ParagraphAlignmentRightDescription,
		MenuCmd_ParagraphAlignmentJustify,
		MenuCmd_ParagraphAlignmentJustifyDescription,
		MenuCmd_ChangeParagraphBackColor,
		MenuCmd_ChangeParagraphBackColorDescription,
		MenuCmd_SetSingleParagraphSpacing,
		MenuCmd_SetSingleParagraphSpacingDescription,
		MenuCmd_SetSesquialteralParagraphSpacing,
		MenuCmd_SetSesquialteralParagraphSpacingDescription,
		MenuCmd_SetDoubleParagraphSpacing,
		MenuCmd_SetDoubleParagraphSpacingDescription,
		MenuCmd_AddSpacingBeforeParagraph,
		MenuCmd_AddSpacingBeforeParagraphDescription,
		MenuCmd_AddSpacingAfterParagraph,
		MenuCmd_AddSpacingAfterParagraphDescription,
		MenuCmd_RemoveSpacingBeforeParagraph,
		MenuCmd_RemoveSpacingBeforeParagraphDescription,
		MenuCmd_RemoveSpacingAfterParagraph,
		MenuCmd_RemoveSpacingAfterParagraphDescription,
		MenuCmd_ChangeTableCellShading,
		MenuCmd_ChangeTableCellShadingDescription,
		MenuCmd_ToggleTableCellsAllBorders,
		MenuCmd_ToggleTableCellsAllBordersDescription,
		MenuCmd_ToggleTableCellsOutsideBorder,
		MenuCmd_ToggleTableCellsOutsideBorderDescription,
		MenuCmd_ToggleTableCellsInsideBorder,
		MenuCmd_ToggleTableCellsInsideBorderDescription,
		MenuCmd_ToggleTableCellsTopBorder,
		MenuCmd_ToggleTableCellsTopBorderDescription,
		MenuCmd_ToggleTableCellsBottomBorder,
		MenuCmd_ToggleTableCellsBottomBorderDescription,
		MenuCmd_ToggleTableCellsLeftBorder,
		MenuCmd_ToggleTableCellsLeftBorderDescription,
		MenuCmd_ToggleTableCellsRightBorder,
		MenuCmd_ToggleTableCellsRightBorderDescription,
		MenuCmd_ToggleTableCellsInsideHorizontalBorder,
		MenuCmd_ToggleTableCellsInsideHorizontalBorderDescription,
		MenuCmd_ToggleTableCellsInsideVerticalBorder,
		MenuCmd_ToggleTableCellsInsideVerticalBorderDescription,
		MenuCmd_ResetTableCellsBorders,
		MenuCmd_ResetTableCellsBordersDescription,
		MenuCmd_ChangeCurrentBorderRepositoryItemLineStyle,
		MenuCmd_ChangeCurrentBorderRepositoryItemLineStyleDescription,
		MenuCmd_ChangeCurrentBorderRepositoryItemLineThickness,
		MenuCmd_ChangeCurrentBorderRepositoryItemLineThicknessDescription,
		MenuCmd_ChangeCurrentBorderRepositoryItemColor,
		MenuCmd_ChangeCurrentBorderRepositoryItemColorDescription,
		MenuCmd_ChangeTableBorders,
		MenuCmd_ChangeTableBordersDescription,
		MenuCmd_ChangeTableCellAlignmentPlaceholder,
		MenuCmd_ChangeTableCellAlignmentPlaceholderDescription,
		MenuCmd_ChangeTableCellsContentAlignment,
		MenuCmd_ChangeTableCellsContentAlignmentDescription,
		MenuCmd_ToggleTableCellsTopLeftAlignment,
		MenuCmd_ToggleTableCellsTopLeftAlignmentDescription,
		MenuCmd_ToggleTableCellsTopCenterAlignment,
		MenuCmd_ToggleTableCellsTopCenterAlignmentDescription,
		MenuCmd_ToggleTableCellsTopRightAlignment,
		MenuCmd_ToggleTableCellsTopRightAlignmentDescription,
		MenuCmd_ToggleTableCellsMiddleLeftAlignment,
		MenuCmd_ToggleTableCellsMiddleLeftAlignmentDescription,
		MenuCmd_ToggleTableCellsMiddleCenterAlignment,
		MenuCmd_ToggleTableCellsMiddleCenterAlignmentDescription,
		MenuCmd_ToggleTableCellsMiddleRightAlignment,
		MenuCmd_ToggleTableCellsMiddleRightAlignmentDescription,
		MenuCmd_ToggleTableCellsBottomLeftAlignment,
		MenuCmd_ToggleTableCellsBottomLeftAlignmentDescription,
		MenuCmd_ToggleTableCellsBottomCenterAlignment,
		MenuCmd_ToggleTableCellsBottomCenterAlignmentDescription,
		MenuCmd_ToggleTableCellsBottomRightAlignment,
		MenuCmd_ToggleTableCellsBottomRightAlignmentDescription,
		MenuCmd_ToggleTableAutoFitPlaceholder,
		MenuCmd_ToggleTableAutoFitPlaceholderDescription,
		MenuCmd_ToggleTableAutoFitContents,
		MenuCmd_ToggleTableAutoFitContentsDescription,
		MenuCmd_ToggleTableAutoFitWindow,
		MenuCmd_ToggleTableAutoFitWindowDescription,
		MenuCmd_ToggleTableFixedColumnWidth,
		MenuCmd_ToggleTableFixedColumnWidthDescription,
		MenuCmd_Delete,
		MenuCmd_DeleteDescription,
		MenuCmd_DeleteCore,
		MenuCmd_DeleteCoreDescription,
		MenuCmd_DeleteBack,
		MenuCmd_DeleteBackDescription,
		MenuCmd_DeleteBackCore,
		MenuCmd_DeleteBackCoreDescription,
		MenuCmd_DeleteWord,
		MenuCmd_DeleteWordDescription,
		MenuCmd_DeleteWordCore,
		MenuCmd_DeleteWordCoreDescription,
		MenuCmd_DeleteWordBack,
		MenuCmd_DeleteWordBackDescription,
		MenuCmd_DeleteWordBackCore,
		MenuCmd_DeleteWordBackCoreDescription,
		MenuCmd_CopySelection,
		MenuCmd_CopySelectionDescription,
		MenuCmd_Paste,
		MenuCmd_PasteDescription,
		MenuCmd_PastePlainText,
		MenuCmd_PastePlainTextDescription,
		MenuCmd_PasteRtfText,
		MenuCmd_PasteRtfTextDescription,
		MenuCmd_PasteSilverlightXamlText,
		MenuCmd_PasteSilverlightXamlTextDescription,
		MenuCmd_PasteHtmlText,
		MenuCmd_PasteHtmlTextDescription,
		MenuCmd_PasteImage,
		MenuCmd_PasteImageDescription,
		MenuCmd_PasteMetafileImage,
		MenuCmd_PasteMetafileImageDescription,
		MenuCmd_PasteFiles,
		MenuCmd_PasteFilesDescription,
		MenuCmd_ShowPasteSpecialForm,
		MenuCmd_ShowPasteSpecialFormDescription,
		MenuCmd_CutSelection,
		MenuCmd_CutSelectionDescription,
		MenuCmd_ToggleWhitespace,
		MenuCmd_ToggleWhitespaceDescription,
		MenuCmd_ToggleOvertype,
		MenuCmd_ToggleOvertypeDescription,
		MenuCmd_ToggleShowTableGridLines,
		MenuCmd_ToggleShowTableGridLinesDescription,
		MenuCmd_ToggleShowHorizontalRuler,
		MenuCmd_ToggleShowHorizontalRulerDescription,
		MenuCmd_ToggleShowVerticalRuler,
		MenuCmd_ToggleShowVerticalRulerDescription,
		MenuCmd_FindAndSelectForward,
		MenuCmd_FindAndSelectForwardDescription,
		MenuCmd_FindAndSelectBackward,
		MenuCmd_FindAndSelectBackwardDescription,
		MenuCmd_ReplaceForward,
		MenuCmd_ReplaceForwardDescription,
		MenuCmd_ReplaceBackward,
		MenuCmd_ReplaceBackwardDescription,
		MenuCmd_FindNext,
		MenuCmd_FindNextDescription,
		MenuCmd_FindPrev,
		MenuCmd_FindPrevDescription,
		MenuCmd_NewEmptyDocument,
		MenuCmd_NewEmptyDocumentDescription,
		MenuCmd_LoadDocument,
		MenuCmd_LoadDocumentDescription,
		MenuCmd_SaveDocument,
		MenuCmd_SaveDocumentDescription,
		MenuCmd_SaveDocumentAs,
		MenuCmd_SaveDocumentAsDescription,
		MenuCmd_ShowFontForm,
		MenuCmd_ShowFontFormDescription,
		MenuCmd_ShowParagraphForm,
		MenuCmd_ShowParagraphFormDescription,
		MenuCmd_ShowEditStyleForm,
		MenuCmd_ShowEditStyleFormDescription,
		MenuCmd_ShowTabsForm,
		MenuCmd_ShowTabsFormDescription,
		MenuCmd_ShowLineSpacingForm,
		MenuCmd_ShowLineSpacingFormDescription,
		MenuCmd_EnterKey,
		MenuCmd_EnterKeyDescription,
		MenuCmd_ShowNumberingList,
		MenuCmd_ShowNumberingListDescription,
		MenuCmd_ShowFloatingObjectLayoutOptionsForm,
		MenuCmd_ShowFloatingObjectLayoutOptionsFormDescription,
		MenuCmd_ShowSymbol,
		MenuCmd_ShowSymbolDescription,
		MenuCmd_ShowBookmarkForm,
		MenuCmd_ShowBookmarkFormDescription,
		MenuCmd_ShowHyperlinkForm,
		MenuCmd_ShowHyperlinkFormDescription,
		MenuCmd_ShowRangeEditingPermissionsForm,
		MenuCmd_ShowRangeEditingPermissionsFormDescription,
		MenuCmd_Hyperlink,
		MenuCmd_HyperlinkDescription,
		MenuCmd_Bookmark,
		MenuCmd_BookmarkDescription,
		MenuCmd_CheckSyntax,
		MenuCmd_CheckSyntaxDescription,
		MenuCmd_CheckSpelling,
		MenuCmd_CheckSpellingDescription,
		MenuCmd_IgnoreMistakenWord,
		MenuCmd_IgnoreMistakenWordDescription,
		MenuCmd_IgnoreAllMistakenWords,
		MenuCmd_IgnoreAllMistakenWordsDescription,
		MenuCmd_AddWordToDictionary,
		MenuCmd_AddWordToDictionaryDescription,
		MenuCmd_DeleteRepeatedWord,
		MenuCmd_DeleteRepeatedWordDescription,
		MenuCmd_ToggleSpellCheckAsYouType,
		MenuCmd_ToggleSpellCheckAsYouTypeDescription,
		MenuCmd_ChangeColumnSize,
		MenuCmd_ChangeColumnSizeDescription,
		MenuCmd_ChangeIndent,
		MenuCmd_ChangeIndentDescription,
		MenuCmd_ChangeParagraphLeftIndent,
		MenuCmd_ChangeParagraphLeftIndentDescription,
		MenuCmd_ChangeParagraphRightIndent,
		MenuCmd_ChangeParagraphRightIndentDescription,
		MenuCmd_ChangeParagraphFirstLineIndent,
		MenuCmd_ChangeParagraphFirstLineIndentDescription,
		MenuCmd_IncrementParagraphLeftIndent,
		MenuCmd_IncrementParagraphLeftIndentDescription,
		MenuCmd_IncrementIndent,
		MenuCmd_IncrementIndentDescription,
		MenuCmd_DecrementParagraphLeftIndent,
		MenuCmd_DecrementParagraphLeftIndentDescription,
		MenuCmd_DecrementIndent,
		MenuCmd_DecrementIndentDescription,
		MenuCmd_ChangeParagraphStyle,
		MenuCmd_ChangeParagraphStyleDescription,
		MenuCmd_PlaceCaretToPhysicalPoint,
		MenuCmd_PlaceCaretToPhysicalPointDescription,
		MenuCmd_ChangeCharacterStyle,
		MenuCmd_ChangeCharacterStyleDescription,
		MenuCmd_Find,
		MenuCmd_FindDescription,
		MenuCmd_Replace,
		MenuCmd_ReplaceDescription,
		MenuCmd_ReplaceText,
		MenuCmd_ReplaceTextDescription,
		MenuCmd_ReplaceAllForward,
		MenuCmd_ReplaceAllForwardDescription,
		MenuCmd_ReplaceAllBackward,
		MenuCmd_ReplaceAllBackwardDescription,
		MenuCmd_ScrollToPage,
		MenuCmd_ScrollToPageDescription,
		MenuCmd_Print,
		MenuCmd_PrintDescription,
		MenuCmd_QuickPrint,
		MenuCmd_QuickPrintDescription,
		MenuCmd_PrintPreview,
		MenuCmd_PrintPreviewDescription,
		MenuCmd_BrowserPrint,
		MenuCmd_BrowserPrintDescription,
		MenuCmd_BrowserPrintPreview,
		MenuCmd_BrowserPrintPreviewDescription,
		MenuCmd_ChangeMistakenWord,
		MenuCmd_ChangeMistakenWordDescription,
		MenuCmd_ClearFormatting,
		MenuCmd_ClearFormattingDescription,
		MenuCmd_CreateField,
		MenuCmd_CreateFieldDescription,
		MenuCmd_CreateBookmark,
		MenuCmd_CreateBookmarkDescription,
		MenuCmd_SelectBookmark,
		MenuCmd_SelectBookmarkDescription,
		MenuCmd_DeleteBookmark,
		MenuCmd_DeleteBookmarkDescription,
		MenuCmd_UpdateField,
		MenuCmd_UpdateFieldDescription,
		MenuCmd_UpdateFields,
		MenuCmd_UpdateFieldsDescription,
		MenuCmd_ToggleFieldCodes,
		MenuCmd_ToggleFieldCodesDescription,
		MenuCmd_ToggleFieldLocked,
		MenuCmd_ToggleFieldLockedDescription,
		MenuCmd_LockField,
		MenuCmd_LockFieldDescription,
		MenuCmd_UnlockField,
		MenuCmd_UnlockFieldDescription,
		MenuCmd_ShowAllFieldResults,
		MenuCmd_ShowAllFieldResultsDescription,
		MenuCmd_ShowAllFieldCodes,
		MenuCmd_ShowAllFieldCodesDescription,
		MenuCmd_ToggleViewMergedData,
		MenuCmd_ToggleViewMergedDataDescription,
		MenuCmd_SelectFieldNextToCaret,
		MenuCmd_SelectFieldNextToCaretDescription,
		MenuCmd_SelectFieldPrevToCaret,
		MenuCmd_SelectFieldPrevToCaretDescription,
		MenuCmd_CreateHyperlink,
		MenuCmd_CreateHyperlinkDescription,
		MenuCmd_InsertHyperlink,
		MenuCmd_InsertHyperlinkDescription,
		MenuCmd_CollapseOrExpandFormulaBar,
		MenuCmd_CollapseOrExpandFormulaBarDescription,
		MenuCmd_SwitchToDraftView,
		MenuCmd_SwitchToDraftViewDescription,
		MenuCmd_SwitchToPrintLayoutView,
		MenuCmd_SwitchToPrintLayoutViewDescription,
		MenuCmd_SwitchToSimpleView,
		MenuCmd_SwitchToSimpleViewDescription,
		MenuCmd_OpenHyperlink,
		MenuCmd_OpenHyperlinkDescription,
		MenuCmd_RemoveHyperlink,
		MenuCmd_RemoveHyperlinkDescription,
		MenuCmd_ModifyHyperlink,
		MenuCmd_ModifyHyperlinkDescription,
		MenuCmd_MergeTableCells,
		MenuCmd_MergeTableCellsDescription,
		MenuCmd_SplitTable,
		MenuCmd_SplitTableDescription,
		MenuCmd_SplitTableCells,
		MenuCmd_SplitTableCellsMenuItem,
		MenuCmd_SplitTableCellsDescription,
		MenuCmd_InsertTableColumnToTheLeft,
		MenuCmd_InsertTableColumnToTheLeftDescription,
		MenuCmd_InsertTableColumnToTheRight,
		MenuCmd_InsertTableColumnToTheRightDescription,
		MenuCmd_OpenHyperlinkAtCaretPosition,
		MenuCmd_OpenHyperlinkAtCaretPositionDescription,
		MenuCmd_EditHyperlink,
		MenuCmd_EditHyperlinkDescription,
		MenuCmd_EditPageHeader,
		MenuCmd_EditPageHeaderDescription,
		MenuCmd_EditPageFooter,
		MenuCmd_EditPageFooterDescription,
		MenuCmd_ClosePageHeaderFooter,
		MenuCmd_ClosePageHeaderFooterDescription,
		MenuCmd_GoToPage,
		MenuCmd_GoToPageDescription,
		MenuCmd_GoToPageHeader,
		MenuCmd_GoToPageHeaderDescription,
		MenuCmd_GoToPageFooter,
		MenuCmd_GoToPageFooterDescription,
		MenuCmd_ToggleHeaderFooterLinkToPrevious,
		MenuCmd_ToggleHeaderFooterLinkToPreviousDescription,
		MenuCmd_GoToPreviousHeaderFooter,
		MenuCmd_GoToPreviousHeaderFooterDescription,
		MenuCmd_GoToNextHeaderFooter,
		MenuCmd_GoToNextHeaderFooterDescription,
		MenuCmd_ToggleDifferentFirstPage,
		MenuCmd_ToggleDifferentFirstPageDescription,
		MenuCmd_ToggleDifferentOddAndEvenPages,
		MenuCmd_ToggleDifferentOddAndEvenPagesDescription,
		MenuCmd_ChangeParagraphLineSpacing,
		MenuCmd_ChangeParagraphLineSpacingDescription,
		MenuCmd_ChangeSectionPageOrientation,
		MenuCmd_ChangeSectionPageOrientationDescription,
		MenuCmd_ChangeSectionPagePaperKind,
		MenuCmd_ChangeSectionPagePaperKindDescription,
		MenuCmd_SetSectionOneColumn,
		MenuCmd_SetSectionOneColumnDescription,
		MenuCmd_SetSectionTwoColumns,
		MenuCmd_SetSectionTwoColumnsDescription,
		MenuCmd_SetSectionThreeColumns,
		MenuCmd_SetSectionThreeColumnsDescription,
		MenuCmd_SetSectionColumns,
		MenuCmd_SetSectionColumnsDescription,
		MenuCmd_SetLandscapePageOrientation,
		MenuCmd_SetLandscapePageOrientationDescription,
		MenuCmd_SetPortraitPageOrientation,
		MenuCmd_SetPortraitPageOrientationDescription,
		MenuCmd_ChangeSectionPageMargins,
		MenuCmd_ChangeSectionPageMarginsDescription,
		MenuCmd_SetNormalSectionPageMargins,
		MenuCmd_SetNormalSectionPageMarginsDescription,
		MenuCmd_SetNarrowSectionPageMargins,
		MenuCmd_SetNarrowSectionPageMarginsDescription,
		MenuCmd_SetModerateSectionPageMargins,
		MenuCmd_SetModerateSectionPageMarginsDescription,
		MenuCmd_SetWideSectionPageMargins,
		MenuCmd_SetWideSectionPageMarginsDescription,
		MenuCmd_NextDataRecord,
		MenuCmd_NextDataRecordDescription,
		MenuCmd_PreviousDataRecord,
		MenuCmd_PreviousDataRecordDescription,
		MenuCmd_LastDataRecord,
		MenuCmd_LastDataRecordDescription,
		MenuCmd_FirstDataRecord,
		MenuCmd_FirstDataRecordDescription,
		MenuCmd_MailMergeSaveDocumentAsCommand,
		MenuCmd_MailMergeSaveDocumentAsCommandDescription,
		MenuCmd_ResetCharacterFormatting,
		MenuCmd_ResetCharacterFormattingDescription,
		MenuCmd_SelectTableColumns,
		MenuCmd_SelectTableColumnsDescription,
		MenuCmd_SelectTableCell,
		MenuCmd_SelectTableCellDescription,
		MenuCmd_SelectTableRow,
		MenuCmd_SelectTableRowDescription,
		MenuCmd_SelectTable,
		MenuCmd_SelectTableDescription,
		MenuCmd_SelectTableElements,
		MenuCmd_SelectTableElementsDescription,
		MenuCmd_ProtectDocument,
		MenuCmd_ProtectDocumentDescription,
		MenuCmd_UnprotectDocument,
		MenuCmd_UnprotectDocumentDescription,
		MenuCmd_CapitalizeEachWordTextCase,
		MenuCmd_CapitalizeEachWordTextCaseDescription,
		MenuCmd_MakeTextUpperCase,
		MenuCmd_MakeTextUpperCaseDescription,
		MenuCmd_MakeTextLowerCase,
		MenuCmd_MakeTextLowerCaseDescription,
		MenuCmd_ToggleTextCase,
		MenuCmd_ToggleTextCaseDescription,
		MenuCmd_ChangeTextCase,
		MenuCmd_ChangeTextCaseDescription,
		MenuCmd_ChangeSectionLineNumbering,
		MenuCmd_ChangeSectionLineNumberingDescription,
		MenuCmd_SetSectionLineNumberingNone,
		MenuCmd_SetSectionLineNumberingNoneDescription,
		MenuCmd_SetSectionLineNumberingContinuous,
		MenuCmd_SetSectionLineNumberingContinuousDescription,
		MenuCmd_SetSectionLineNumberingRestartNewPage,
		MenuCmd_SetSectionLineNumberingRestartNewPageDescription,
		MenuCmd_SetSectionLineNumberingRestartNewSection,
		MenuCmd_SetSectionLineNumberingRestartNewSectionDescription,
		MenuCmd_ParagraphSuppressLineNumbers,
		MenuCmd_ParagraphSuppressLineNumbersDescription,
		MenuCmd_ParagraphSuppressHyphenation,
		MenuCmd_ParagraphSuppressHyphenationDescription,
		MenuCmd_ShowLineNumberingForm,
		MenuCmd_ShowLineNumberingFormDescription,
		MenuCmd_ShowPageSetupForm,
		MenuCmd_ShowPageSetupFormDescription,
		MenuCmd_ShowColumnsSetupForm,
		MenuCmd_ShowColumnsSetupFormDescription,
		MenuCmd_ShowTablePropertiesForm,
		MenuCmd_ShowTablePropertiesFormDescription,
		MenuCmd_ShowTablePropertiesFormMenuItem,
		MenuCmd_ShowTablePropertiesFormDescriptionMenuItem,
		MenuCmd_ShowTableOptionsForm,
		MenuCmd_ShowTableOptionsFormDescription,
		MenuCmd_IncrementParagraphOutlineLevel,
		MenuCmd_IncrementParagraphOutlineLevelDescription,
		MenuCmd_DecrementParagraphOutlineLevel,
		MenuCmd_DecrementParagraphOutlineLevelDescription,
		MenuCmd_SetParagraphBodyTextLevel,
		MenuCmd_SetParagraphBodyTextLevelDescription,
		MenuCmd_SetParagraphHeadingLevel,
		MenuCmd_SetParagraphHeadingLevelDescription,
		MenuCmd_AddParagraphsToTableOfContents,
		MenuCmd_AddParagraphsToTableOfContentsDescription,
		MenuCmd_InsertTableOfContents,
		MenuCmd_InsertTableOfContentsDescription,
		MenuCmd_InsertTableOfEquations,
		MenuCmd_InsertTableOfEquationsDescription,
		MenuCmd_InsertTableOfFigures,
		MenuCmd_InsertTableOfFiguresDescription,
		MenuCmd_InsertTableOfTables,
		MenuCmd_InsertTableOfTablesDescription,
		MenuCmd_InsertTableOfFiguresPlaceholder,
		MenuCmd_InsertTableOfFiguresPlaceholderDescription,
		MenuCmd_InsertEquationsCaption,
		MenuCmd_InsertEquationsCaptionDescription,
		MenuCmd_InsertFiguresCaption,
		MenuCmd_InsertFiguresCaptionDescription,
		MenuCmd_InsertTablesCaption,
		MenuCmd_InsertTablesCaptionDescription,
		MenuCmd_InsertCaptionPlaceholder,
		MenuCmd_InsertCaptionPlaceholderDescription,
		MenuCmd_UpdateTableOfContents,
		MenuCmd_UpdateTableOfContentsDescription,
		MenuCmd_UpdateTableOfFigures,
		MenuCmd_UpdateTableOfFiguresDescription,
		MenuCmd_SetFloatingObjectSquareTextWrapType,
		MenuCmd_SetFloatingObjectSquareTextWrapTypeDescription,
		MenuCmd_SetFloatingObjectBehindTextWrapType,
		MenuCmd_SetFloatingObjectBehindTextWrapTypeDescription,
		MenuCmd_SetFloatingObjectInFrontOfTextWrapType,
		MenuCmd_SetFloatingObjectInFrontOfTextWrapTypeDescription,
		MenuCmd_SetFloatingObjectThroughTextWrapType,
		MenuCmd_SetFloatingObjectThroughTextWrapTypeDescription,
		MenuCmd_SetFloatingObjectTightTextWrapType,
		MenuCmd_SetFloatingObjectTightTextWrapTypeDescription,
		MenuCmd_SetFloatingObjectTopAndBottomTextWrapType,
		MenuCmd_SetFloatingObjectTopAndBottomTextWrapTypeDescription,
		MenuCmd_SetFloatingObjectTopLeftAlignment,
		MenuCmd_SetFloatingObjectTopLeftAlignmentDescription,
		MenuCmd_SetFloatingObjectTopCenterAlignment,
		MenuCmd_SetFloatingObjectTopCenterAlignmentDescription,
		MenuCmd_SetFloatingObjectTopRightAlignment,
		MenuCmd_SetFloatingObjectTopRightAlignmentDescription,
		MenuCmd_SetFloatingObjectMiddleLeftAlignment,
		MenuCmd_SetFloatingObjectMiddleLeftAlignmentDescription,
		MenuCmd_SetFloatingObjectMiddleCenterAlignment,
		MenuCmd_SetFloatingObjectMiddleCenterAlignmentDescription,
		MenuCmd_SetFloatingObjectMiddleRightAlignment,
		MenuCmd_SetFloatingObjectMiddleRightAlignmentDescription,
		MenuCmd_SetFloatingObjectBottomLeftAlignment,
		MenuCmd_SetFloatingObjectBottomLeftAlignmentDescription,
		MenuCmd_SetFloatingObjectBottomCenterAlignment,
		MenuCmd_SetFloatingObjectBottomCenterAlignmentDescription,
		MenuCmd_SetFloatingObjectBottomRightAlignment,
		MenuCmd_SetFloatingObjectBottomRightAlignmentDescription,
		MenuCmd_FloatingObjectBringForward,
		MenuCmd_FloatingObjectBringForwardDescription,
		MenuCmd_FloatingObjectBringToFront,
		MenuCmd_FloatingObjectBringToFrontDescription,
		MenuCmd_FloatingObjectBringInFrontOfText,
		MenuCmd_FloatingObjectBringInFrontOfTextDescription,
		MenuCmd_FloatingObjectSendBackward,
		MenuCmd_FloatingObjectSendBackwardDescription,
		MenuCmd_FloatingObjectSendToBack,
		MenuCmd_FloatingObjectSendToBackDescription,
		MenuCmd_FloatingObjectSendBehindText,
		MenuCmd_FloatingObjectSendBehindTextDescription,
		MenuCmd_ChangeFloatingObjectTextWrapType,
		MenuCmd_ChangeFloatingObjectTextWrapTypeDescription,
		MenuCmd_ChangeFloatingObjectAlignment,
		MenuCmd_ChangeFloatingObjectAlignmentDescription,
		MenuCmd_FloatingObjectBringForwardPlaceholder,
		MenuCmd_FloatingObjectBringForwardPlaceholderDescription,
		MenuCmd_FloatingObjectSendBackwardPlaceholder,
		MenuCmd_FloatingObjectSendBackwardPlaceholderDescription,
		MenuCmd_ShowPageMarginsSetupForm,
		MenuCmd_ShowPageMarginsSetupFormDescription,
		MenuCmd_ShowPagePaperSetupForm,
		MenuCmd_ShowPagePaperSetupFormDescription,
		MenuCmd_ChangeFloatingObjectFillColor,
		MenuCmd_ChangeFloatingObjectFillColorDescription,
		MenuCmd_ChangeFloatingObjectOutlineColor,
		MenuCmd_ChangeFloatingObjectOutlineColorDescription,
		MenuCmd_ChangeFloatingObjectOutlineWidth,
		MenuCmd_ChangeFloatingObjectOutlineWidthDescription,
		MenuCmd_InsertTextBox,
		MenuCmd_InsertTextBoxDescription,
		MenuCmd_InsertFloatingObjectPicture,
		MenuCmd_InsertFloatingObjectPictureDescription,
		MenuCmd_ChangePageColor,
		MenuCmd_ChangePageColorDescription,
		MenuCmd_ToggleFirstRow,
		MenuCmd_ToggleFirstRowDescription,
		MenuCmd_ToggleLastRow,
		MenuCmd_ToggleLastRowDescription,
		MenuCmd_ToggleFirstColumn,
		MenuCmd_ToggleFirstColumnDescription,
		MenuCmd_ToggleLastColumn,
		MenuCmd_ToggleLastColumnDescription,
		MenuCmd_ToggleBandedRows,
		MenuCmd_ToggleBandedRowsDescription,
		MenuCmd_ToggleBandedColumn,
		MenuCmd_ToggleBandedColumnDescription,
		MenuCmd_NewTableStyle,
		MenuCmd_ModifyTableStyle,
		MenuCmd_DeleteTableStyle,
		MenuCmd_EditTOC,
		MenuCmd_ChangeLanguage,
		MenuCmd_ChangeLanguageDescription,
		MenuCmd_Language,
		MenuCmd_LanguageDescription,
		MenuCmd_ChangeNoProof,
		MenuCmd_ChangeNoProofDescription,
		MenuCmd_Comment,
		MenuCmd_CommentDescription,
		MenuCmd_ShowCommentForm,
		MenuCmd_ShowCommentFormDescription,
		MenuCmd_NewComment,
		MenuCmd_NewCommentDescription,
		MenuCmd_SelectComment,
		MenuCmd_SelectCommentDescription,
		MenuCmd_DeleteComment,
		MenuCmd_DeleteCommentDescription,
		MenuCmd_DeleteOneComment,
		MenuCmd_DeleteOneCommentDescription,
		MenuCmd_DeleteAllCommentsShown,
		MenuCmd_DeleteAllCommentsShownDescription,
		MenuCmd_DeleteAllComments,
		MenuCmd_DeleteAllCommentsDescription,
		MenuCmd_PreviousComment,
		MenuCmd_PreviousCommentDescription,
		MenuCmd_NextComment,
		MenuCmd_NextCommentDescription,
		MenuCmd_ReviewingPane,
		MenuCmd_ReviewingPaneDescription,
		MenuCmd_ToggleAuthorVisibility,
		MenuCmd_ToggleAuthorVisibilityDescription,
		MenuCmd_Reviewers,
		MenuCmd_ReviewersDescription,
		MenuCmd_None,
		Msg_InvalidBeginInit,
		Msg_InvalidEndInit,
		Msg_InvalidBeginUpdate,
		Msg_InvalidEndUpdate,
		Msg_InvalidSetCharacterProperties,
		Msg_InvalidSetParagraphProperties,
		Msg_InvalidParentStyle,
		Msg_InvalidDocumentModel,
		Msg_TableIntegrityError,
		Msg_InvalidParagraphContainNumbering,
		Msg_InvalidCopyFromDocumentModel,
		Msg_InvalidNumber,
		Msg_InvalidFontSize,
		Msg_InvalidValueRange,
		Msg_InvalidDivisor,
		Msg_UsedWrongUnit,
		Msg_InvalidTabStop,
		Msg_VariableDeletedOrMissed,
		Msg_SearchComplete,
		Msg_SearchInForwardDirectionComplete,
		Msg_SearchInBackwardDirectionComplete,
		Msg_StyleAlreadyLinked,
		Msg_ErrorLinkDeletedStyle,
		Msg_SearchItemNotFound,
		Msg_SearchInSelectionComplete,
		Msg_ContinueSearchFromBeginningQuestion,
		Msg_ContinueSearchFromEndQuestion,
		Msg_ContinueSearchInRemainderQuestion,
		Msg_ReplacementsCount,
		Msg_InvalidStyleName,
		Msg_IncorrectNumericFieldFormat,
		Msg_SyntaxErrorInFieldPattern,
		Msg_UnmatchedQuotesInFieldPattern,
		Msg_UnknownSwitchArgument,
		Msg_UnexpectedEndOfFormula,
		Msg_MissingOperator,
		Msg_ZeroDivide,
		Msg_ClickToFollowHyperlink,
		Msg_BookmarkCreationFailing,
		FileFilterDescription_AllFiles,
		FileFilterDescription_AllSupportedFiles,
		FileFilterDescription_DocFiles,
		FileFilterDescription_HtmlFiles,
		FileFilterDescription_MhtFiles,
		FileFilterDescription_RtfFiles,
		FileFilterDescription_TextFiles,
		FileFilterDescription_OpenXmlFiles,
		FileFilterDescription_OpenDocumentFiles,
		FileFilterDescription_WordMLFiles,
		FileFilterDescription_XamlFiles,
		FileFilterDescription_ePubFiles,
		FileFilterDescription_PDFFiles,
		FileFilterDescription_BitmapFiles,
		FileFilterDescription_JPEGFiles,
		FileFilterDescription_PNGFiles,
		FileFilterDescription_GifFiles,
		FileFilterDescription_TiffFiles,
		FileFilterDescription_EmfFiles,
		FileFilterDescription_WmfFiles,
		DefaultStyleName_ArticleSection,
		DefaultStyleName_Normal,
		DefaultStyleName_heading1,
		DefaultStyleName_heading2,
		DefaultStyleName_heading3,
		DefaultStyleName_heading4,
		DefaultStyleName_heading5,
		DefaultStyleName_heading6,
		DefaultStyleName_heading7,
		DefaultStyleName_heading8,
		DefaultStyleName_heading9,
		DefaultStyleName_index1,
		DefaultStyleName_index2,
		DefaultStyleName_index3,
		DefaultStyleName_index4,
		DefaultStyleName_index5,
		DefaultStyleName_index6,
		DefaultStyleName_index7,
		DefaultStyleName_index8,
		DefaultStyleName_index9,
		DefaultStyleName_toc1,
		DefaultStyleName_toc2,
		DefaultStyleName_toc3,
		DefaultStyleName_toc4,
		DefaultStyleName_toc5,
		DefaultStyleName_toc6,
		DefaultStyleName_toc7,
		DefaultStyleName_toc8,
		DefaultStyleName_toc9,
		DefaultStyleName_NormalIndent,
		DefaultStyleName_footnotetext,
		DefaultStyleName_annotationtext,
		DefaultStyleName_header,
		DefaultStyleName_footer,
		DefaultStyleName_indexheading,
		DefaultStyleName_caption,
		DefaultStyleName_CommentReference,
		DefaultStyleName_CommentSubject,
		DefaultStyleName_CommentText,
		DefaultStyleName_tableoffigures,
		DefaultStyleName_envelopeaddress,
		DefaultStyleName_envelopereturn,
		DefaultStyleName_footnotereference,
		DefaultStyleName_annotationreference,
		DefaultStyleName_linenumber,
		DefaultStyleName_pagenumber,
		DefaultStyleName_endnotereference,
		DefaultStyleName_endnotetext,
		DefaultStyleName_tableofauthorities,
		DefaultStyleName_macrotoaheading,
		DefaultStyleName_MacroText,
		DefaultStyleName_NoteHeading,
		DefaultStyleName_List,
		DefaultStyleName_List2,
		DefaultStyleName_List3,
		DefaultStyleName_List4,
		DefaultStyleName_List5,
		DefaultStyleName_ListBullet,
		DefaultStyleName_ListBullet2,
		DefaultStyleName_ListBullet3,
		DefaultStyleName_ListBullet4,
		DefaultStyleName_ListBullet5,
		DefaultStyleName_ListNumber,
		DefaultStyleName_ListNumber2,
		DefaultStyleName_ListNumber3,
		DefaultStyleName_ListNumber4,
		DefaultStyleName_ListNumber5,
		DefaultStyleName_Title,
		DefaultStyleName_Closing,
		DefaultStyleName_Signature,
		DefaultStyleName_DefaultParagraphFont,
		DefaultStyleName_BodyText,
		DefaultStyleName_ListContinue,
		DefaultStyleName_ListContinue2,
		DefaultStyleName_ListContinue3,
		DefaultStyleName_ListContinue4,
		DefaultStyleName_ListContinue5,
		DefaultStyleName_MessageHeader,
		DefaultStyleName_Salutation,
		DefaultStyleName_Date,
		DefaultStyleName_BodyTextFirstIndent,
		DefaultStyleName_BodyTextFirstIndent2,
		DefaultStyleName_Strong,
		DefaultStyleName_Subtitle,
		DefaultStyleName_BodyText2,
		DefaultStyleName_BodyText3,
		DefaultStyleName_BodyTextIndent,
		DefaultStyleName_BodyTextIndent2,
		DefaultStyleName_BodyTextIndent3,
		DefaultStyleName_BlockText,
		DefaultStyleName_HyperlinkFollowed,
		DefaultStyleName_HyperlinkStrongEmphasis,
		DefaultStyleName_Emphasis,
		DefaultStyleName_FollowedHyperlink,
		DefaultStyleName_DocumentMap,
		DefaultStyleName_PlainText,
		DefaultStyleName_EmailSignature,
		DefaultStyleName_HTMLTopofForm,
		DefaultStyleName_HTMLBottomofForm,
		DefaultStyleName_NormalWeb,
		DefaultStyleName_HTMLAcronym,
		DefaultStyleName_HTMLAddress,
		DefaultStyleName_HTMLCite,
		DefaultStyleName_HTMLCode,
		DefaultStyleName_HTMLDefinition,
		DefaultStyleName_HTMLKeyboard,
		DefaultStyleName_HTMLPreformatted,
		DefaultStyleName_HTMLSample,
		DefaultStyleName_HTMLTypewriter,
		DefaultStyleName_HTMLVariable,
		DefaultStyleName_NormalTable,
		DefaultStyleName_annotationsubject,
		DefaultStyleName_NoList,
		DefaultStyleName_OutlineList1,
		DefaultStyleName_OutlineList2,
		DefaultStyleName_OutlineList3,
		DefaultStyleName_TableSimple1,
		DefaultStyleName_TableSimple2,
		DefaultStyleName_TableSimple3,
		DefaultStyleName_TableClassic1,
		DefaultStyleName_TableClassic2,
		DefaultStyleName_TableClassic3,
		DefaultStyleName_TableClassic4,
		DefaultStyleName_TableColorful1,
		DefaultStyleName_TableColorful2,
		DefaultStyleName_TableColorful3,
		DefaultStyleName_TableColumns1,
		DefaultStyleName_TableColumns2,
		DefaultStyleName_TableColumns3,
		DefaultStyleName_TableColumns4,
		DefaultStyleName_TableColumns5,
		DefaultStyleName_TableGrid1,
		DefaultStyleName_TableGrid2,
		DefaultStyleName_TableGrid3,
		DefaultStyleName_TableGrid4,
		DefaultStyleName_TableGrid5,
		DefaultStyleName_TableGrid6,
		DefaultStyleName_TableGrid7,
		DefaultStyleName_TableGrid8,
		DefaultStyleName_TableList1,
		DefaultStyleName_TableList2,
		DefaultStyleName_TableList3,
		DefaultStyleName_TableList4,
		DefaultStyleName_TableList5,
		DefaultStyleName_TableList6,
		DefaultStyleName_TableList7,
		DefaultStyleName_TableList8,
		DefaultStyleName_TableNormal,
		DefaultStyleName_Table3Deffects1,
		DefaultStyleName_Table3Deffects2,
		DefaultStyleName_Table3Deffects3,
		DefaultStyleName_TableContemporary,
		DefaultStyleName_TableElegant,
		DefaultStyleName_TableProfessional,
		DefaultStyleName_TableSubtle1,
		DefaultStyleName_TableSubtle2,
		DefaultStyleName_TableWeb1,
		DefaultStyleName_TableWeb2,
		DefaultStyleName_TableWeb3,
		DefaultStyleName_BalloonText,
		DefaultStyleName_TableGrid,
		DefaultStyleName_TableTheme,
		DefaultStyleName_toaheading,
		LocalizedStyleName_Normal,
		LocalizedStyleName_Hyperlink,
		LinkedCharacterStyleFormatString,
		ClearFormatting,
		FontStyle_Bold,
		FontStyle_Italic,
		FontStyle_BoldItalic,
		FontStyle_Regular,
		FontStyle_Strikeout,
		FontStyle_Underline,
		Caption_AlignmentCenter,
		Caption_AlignmentJustify,
		Caption_AlignmentLeft,
		Caption_AlignmentRight,
		Caption_LineSpacingSingle,
		Caption_LineSpacingSesquialteral,
		Caption_LineSpacingDouble,
		Caption_LineSpacingMultiple,
		Caption_LineSpacingExactly,
		Caption_LineSpacingAtLeast,
		Caption_FirstLineIndentNone,
		Caption_FirstLineIndentIndented,
		Caption_FirstLineIndentHanging,
		Caption_OutlineLevelBody,
		Caption_OutlineLevel1,
		Caption_OutlineLevel2,
		Caption_OutlineLevel3,
		Caption_OutlineLevel4,
		Caption_OutlineLevel5,
		Caption_OutlineLevel6,
		Caption_OutlineLevel7,
		Caption_OutlineLevel8,
		Caption_OutlineLevel9,
		Caption_Heading,
		HyperlinkForm_SelectionInDocument,
		HyperlinkForm_InsertHyperlinkTitle,
		HyperlinkForm_EditHyperlinkTitle,
		HyperlinkForm_SelectedBookmarkNone,
		DialogCaption_InsertSymbol,
		TabForm_All,
		UnderlineNone,
		BorderLineStyleNone,
		ColorAuto,
		Comment,
		Page,
		CommentToolTipHeader,
		CommentToolTipHeaderWithDate,
		FindAndReplaceForm_AnySingleCharacter,
		FindAndReplaceForm_ZeroOrMore,
		FindAndReplaceForm_OneOrMore,
		FindAndReplaceForm_BeginningOfLine,
		FindAndReplaceForm_EndOfLine,
		FindAndReplaceForm_BeginningOfWord,
		FindAndReplaceForm_EndOfWord,
		FindAndReplaceForm_AnyOneCharacterInTheSet,
		FindAndReplaceForm_AnyOneCharacterNotInTheSet,
		FindAndReplaceForm_Or,
		FindAndReplaceForm_EscapeSpecialCharacter,
		FindAndReplaceForm_TagExpression,
		FindAndReplaceForm_WordCharacter,
		FindAndReplaceForm_SpaceOrTab,
		FindAndReplaceForm_Integer,
		FindAndReplaceForm_TaggedExpression,
		FieldMapUniqueId,
		FieldMapTitle,
		FieldMapFirstName,
		FieldMapMiddleName,
		FieldLastName,
		FieldMapSuffix,
		FieldMapNickName,
		FieldMapJobTitle,
		FieldMapCompany,
		FieldMapAddress1,
		FieldMapAddress2,
		FieldMapCity,
		FieldMapState,
		FieldMapPostalCode,
		FieldMapCountry,
		FieldMapBusinessPhone,
		FieldMapBusinessFax,
		FieldMapHomePhone,
		FieldMapHomeFax,
		FieldMapEMailAddress,
		FieldMapWebPage,
		FieldMapPartnerTitle,
		FieldMapPartnerFirstName,
		FieldMapPartnerMiddleName,
		FieldMapPartnerLastName,
		FieldMapPartnerNickName,
		FieldMapPhoneticGuideFirstName,
		FieldMapPhoneticGuideLastName,
		FieldMapAddress3,
		FieldMapDepartment,
		TargetFrameDescription_Self,
		TargetFrameDescription_Blank,
		TargetFrameDescription_Top,
		TargetFrameDescription_Parent,
		KeyName_Control,
		KeyName_Shift,
		KeyName_Alt,
		Caption_NumberingListBoxNone,
		Caption_PageHeader,
		Caption_FirstPageHeader,
		Caption_OddPageHeader,
		Caption_EvenPageHeader,
		Caption_PageFooter,
		Caption_FirstPageFooter,
		Caption_OddPageFooter,
		Caption_EvenPageFooter,
		Caption_SameAsPrevious,
		Caption_SectionPropertiesApplyToWholeDocument,
		Caption_SectionPropertiesApplyToCurrentSection,
		Caption_SectionPropertiesApplyToSelectedSections,
		Caption_SectionPropertiesApplyThisPointForward,
		Caption_PageSetupSectionStartContinuous,
		Caption_PageSetupSectionStartColumn,
		Caption_PageSetupSectionStartNextPage,
		Caption_PageSetupSectionStartOddPage,
		Caption_PageSetupSectionStartEvenPage,
		Caption_HeightTypeExact,
		Caption_HeightTypeMinimum,
		Caption_CaptionPrefixEquation,
		Caption_CaptionPrefixFigure,
		Caption_CaptionPrefixTable,
		Caption_CurrentDocumentHyperlinkTooltip,
		Msg_InvalidNumberingListStartAtValue,
		Msg_Loading,
		Msg_Saving,
		Msg_DuplicateBookmark,
		Msg_NoDefaultTabs,
		Msg_CantResetDefaultProperties,
		Msg_CantDeleteDefaultStyle,
		Msg_NoTocEntriesFound,
		Msg_NumberingListNotInListCollection,
		Msg_ParagraphStyleNameAlreadyExists,
		Msg_DeleteTableStyleQuestion,
		FloatingObjectLayoutOptionsForm_HorizontalPositionTypeMargin,
		FloatingObjectLayoutOptionsForm_HorizontalPositionTypeCharacter,
		FloatingObjectLayoutOptionsForm_HorizontalPositionTypeColumn,
		FloatingObjectLayoutOptionsForm_HorizontalPositionTypeInsideMargin,
		FloatingObjectLayoutOptionsForm_HorizontalPositionTypeLeftMargin,
		FloatingObjectLayoutOptionsForm_HorizontalPositionTypeOutsideMargin,
		FloatingObjectLayoutOptionsForm_HorizontalPositionTypePage,
		FloatingObjectLayoutOptionsForm_HorizontalPositionTypeRightMargin,
		FloatingObjectLayoutOptionsForm_HorizontalPositionAlignmentCenter,
		FloatingObjectLayoutOptionsForm_HorizontalPositionAlignmentLeft,
		FloatingObjectLayoutOptionsForm_HorizontalPositionAlignmentRight,
		FloatingObjectLayoutOptionsForm_VerticalPositionTypeMargin,
		FloatingObjectLayoutOptionsForm_VerticalPositionTypePage,
		FloatingObjectLayoutOptionsForm_VerticalPositionTypeLine,
		FloatingObjectLayoutOptionsForm_VerticalPositionTypeTopMargin,
		FloatingObjectLayoutOptionsForm_VerticalPositionTypeBottomMargin,
		FloatingObjectLayoutOptionsForm_VerticalPositionTypeInsideMargin,
		FloatingObjectLayoutOptionsForm_VerticalPositionTypeOutsideMargin,
		FloatingObjectLayoutOptionsForm_VerticalPositionTypeParagraph,
		FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentTop,
		FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentCenter,
		FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentBottom,
		FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentInside,
		FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentOutside,
		Caption_PreviousParagraphText,
		Caption_CurrentParagraphText,
		Caption_FollowingParagraphText,
		Caption_EmptyParentStyle,
		Caption_ParagraphAlignment_Left,
		Caption_ParagraphAlignment_Right,
		Caption_ParagraphAlignment_Center,
		Caption_ParagraphAlignment_Justify,
		Caption_ParagraphFirstLineIndent_None,
		Caption_ParagraphFirstLineIndent_Indented,
		Caption_ParagraphFirstLineIndent_Hanging,
		Caption_ParagraphLineSpacing_Single,
		Caption_ParagraphLineSpacing_Sesquialteral,
		Caption_ParagraphLineSpacing_Double,
		Caption_ParagraphLineSpacing_Multiple,
		Caption_ParagraphLineSpacing_Exactly,
		Caption_ParagraphLineSpacing_AtLeast,
		Caption_ColorAutomatic,
		Caption_NoColor,
		Caption_PageCategoryTableTools,
		Caption_PageTableDesign,
		Caption_PageTableLayout,
		Caption_GroupTableStyleOptions,
		Caption_GroupTableDrawBorders,
		Caption_GroupTableRowsAndColumns,
		Caption_GroupTableStyles,
		Caption_GroupTableTable,
		Caption_GroupTableMerge,
		Caption_GroupTableCellSize,
		Caption_GroupTableAlignment,
		Caption_ConditionalTableStyleFormattingTypes_WholeTable,
		Caption_ConditionalTableStyleFormattingTypes_FirstRow,
		Caption_ConditionalTableStyleFormattingTypes_LastRow,
		Caption_ConditionalTableStyleFormattingTypes_FirstColumn,
		Caption_ConditionalTableStyleFormattingTypes_LastColumn,
		Caption_ConditionalTableStyleFormattingTypes_OddRowBanding,
		Caption_ConditionalTableStyleFormattingTypes_EvenRowBanding,
		Caption_ConditionalTableStyleFormattingTypes_OddColumnBanding,
		Caption_ConditionalTableStyleFormattingTypes_EvenColumnBanding,
		Caption_ConditionalTableStyleFormattingTypes_TopLeftCell,
		Caption_ConditionalTableStyleFormattingTypes_TopRightCell,
		Caption_ConditionalTableStyleFormattingTypes_BottomLeftCell,
		Caption_ConditionalTableStyleFormattingTypes_BottomRightCell,
		Caption_ClipboardSubItem,
		Msg_ClickToComment,
		SelectionCollection_EmptyCollectionException,
		SelectionCollection_SpecifiedSelectionsIntersectException,
		SelectionCollection_CurrentSelectionAndSpecifiedSelectionIntersectException,
		SelectionCollection_SelectionShouldContainAtLeastOneCharacterException,
		SelectionCollection_SelectionExtendsOutsideTableException,
		SelectionCollection_FirstCellContinuesVerticalMergeException,
		SelectionCollection_LastCellContinuesVerticalMergeException,
		SelectionCollection_SelecitonShouldIncludeNotMoreThanOneRowException,
		SelectionCollection_PartiallySelectedCellsException,
		SelectionCollection_SelectionCollectionEmptyException,
		SelectionCollection_AtLeastOneSelectionIsRequiredException,
		SelectionCollection_CannotRemoveCaretException,
		SelectionCollection_RangeCannotBeEmptyException,
		SelectionCollection_OutOfRangeException,
		Caption_MainDocumentComments,
	}
	#endregion
	#region XtraRichEditLocalizer
	public class XtraRichEditLocalizer : RichEditLocalizerBase<XtraRichEditStringId> {
		static XtraRichEditLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraRichEditStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(XtraRichEditStringId.Msg_IsNotValid, "'{0}' is not a valid value for '{1}'");
			AddString(XtraRichEditStringId.Msg_UnsupportedDocVersion, "Only MS Word 97 and later versions are supported");
			AddString(XtraRichEditStringId.Msg_EncryptedFile, "Encrypted files are not currently supported");
			AddString(XtraRichEditStringId.Msg_MagicNumberNotFound, "The file you are trying to open is in different format than specified by the file extension.");
			AddString(XtraRichEditStringId.Msg_InternalError, "An internal error occurred");
			AddString(XtraRichEditStringId.Msg_UseDeletedStyleError, "Error: using deleted style");
			AddString(XtraRichEditStringId.Msg_UseDeletedParagraphError, "Error: using deleted paragraph");
			AddString(XtraRichEditStringId.Msg_UseDeletedFieldError, "Error: using field paragraph");
			AddString(XtraRichEditStringId.Msg_UseDeletedSectionError, "Error: using deleted section");
			AddString(XtraRichEditStringId.Msg_UseDeletedBookmarkError, "Error: using deleted bookmark");
			AddString(XtraRichEditStringId.Msg_UseDeletedCommentError, "Error: using deleted comment");
			AddString(XtraRichEditStringId.Msg_UseDeletedCustomMarkError, "Error: using deleted customMark");
			AddString(XtraRichEditStringId.Msg_UseDeletedHyperlinkError, "Error: using deleted hyperlink");
			AddString(XtraRichEditStringId.Msg_UseDeletedTableError, "Error: using deleted table");
			AddString(XtraRichEditStringId.Msg_UseDeletedTableRowError, "Error: using deleted table row");
			AddString(XtraRichEditStringId.Msg_UseDeletedTableCellError, "Error: using deleted table cell");
			AddString(XtraRichEditStringId.Msg_UseInvalidParagraphProperties, "Error: paragraph properties are no longer valid");
			AddString(XtraRichEditStringId.Msg_UseInvalidCharacterProperties, "Error: character properties are no longer valid");
			AddString(XtraRichEditStringId.Msg_DocumentPositionDoesntMatchDocument, "Error: specified document position or range belongs to other document or subdocument");
			AddString(XtraRichEditStringId.Msg_UseInvalidDocument, "Error: this document is no longer valid");
			AddString(XtraRichEditStringId.Msg_UnsupportedFormatException, "File format is not supported");
			AddString(XtraRichEditStringId.Msg_InvalidBeginInit, "Error: call to BeginInit inside BeginUpdate");
			AddString(XtraRichEditStringId.Msg_InvalidEndInit, "Error: call to EndInit or CancelInit without BeginInit or inside BeginUpdate");
			AddString(XtraRichEditStringId.Msg_InvalidBeginUpdate, "Error: call to BeginUpdate inside BeginInit");
			AddString(XtraRichEditStringId.Msg_InvalidEndUpdate, "Error: call to EndUpdate or CancelUpate without BeginUpdate or inside BeginInit");
			AddString(XtraRichEditStringId.Msg_InvalidSetCharacterProperties, "Error: cannot set properties without BeginInit or without adding object to document");
			AddString(XtraRichEditStringId.Msg_InvalidSetParagraphProperties, "Error: cannot set properties without BeginInit or without adding object to document");
			AddString(XtraRichEditStringId.Msg_InvalidParentStyle, "Error: invalid parent style assignment caused circular reference");
			AddString(XtraRichEditStringId.Msg_InvalidDocumentModel, "Error: document models are different");
			AddString(XtraRichEditStringId.Msg_InvalidParagraphContainNumbering, "Error: paragraph already contains numbering");
			AddString(XtraRichEditStringId.Msg_TableIntegrityError, "Error: broken table integrity");
			AddString(XtraRichEditStringId.Msg_InvalidCopyFromDocumentModel, "Error: source and destination document models are different");
			AddString(XtraRichEditStringId.Msg_InvalidNumber, "This is not a valid number.");
			AddString(XtraRichEditStringId.Msg_InvalidFontSize, "The number must be between {0} and {1}.");
			AddString(XtraRichEditStringId.Msg_InvalidValueRange, "The value must be between {0} and {1}.");
			AddString(XtraRichEditStringId.Msg_InvalidDivisor, "The number must be a divisor of {0}.");
			AddString(XtraRichEditStringId.Msg_UsedWrongUnit, "The unit of measurement is incorrectly specified.");
			AddString(XtraRichEditStringId.Msg_InvalidTabStop, "This is not a valid tab stop.");
			AddString(XtraRichEditStringId.Msg_VariableDeletedOrMissed, "Error: document variable is either missing or deleted (from Variables collection)");
			AddString(XtraRichEditStringId.Msg_TopBottomSectionMarginsTooLarge, "The top/bottom margins are too large for the page height in some sections.");
			AddString(XtraRichEditStringId.Msg_LeftRightSectionMarginsTooLarge, "The left/right margins are too large for the page height in some sections.");
			AddString(XtraRichEditStringId.Msg_InvalidNumberingListIndex, "NumberingListIndex refers to a list that does not exist.");
			AddString(XtraRichEditStringId.Msg_InvalidImageFile, "The specified image is not valid.");
			AddString(XtraRichEditStringId.Msg_SearchComplete, "The search is complete.");
			AddString(XtraRichEditStringId.Msg_SearchInForwardDirectionComplete, "The end of the document has been reached.");
			AddString(XtraRichEditStringId.Msg_SearchInBackwardDirectionComplete, "The beginning of the document has been reached.");
			AddString(XtraRichEditStringId.Msg_SearchInSelectionComplete, "The search in the selection is finished.");
			AddString(XtraRichEditStringId.Msg_ContinueSearchFromBeginningQuestion, "Do you want to start the search from the beginning of the document?");
			AddString(XtraRichEditStringId.Msg_ContinueSearchFromEndQuestion, "Do you want to start the search from the end of the document?");
			AddString(XtraRichEditStringId.Msg_ContinueSearchInRemainderQuestion, "Do you want to search the remainder of the document?");
			AddString(XtraRichEditStringId.Msg_SearchItemNotFound, "The search item was not found.");
			AddString(XtraRichEditStringId.Msg_ReplacementsCount, "{0} replacements were made.");
			AddString(XtraRichEditStringId.Msg_StyleAlreadyLinked, "Error: style already contains a linked style");
			AddString(XtraRichEditStringId.Msg_ErrorLinkDeletedStyle, "Error: cannot link deleted style");
			AddString(XtraRichEditStringId.Msg_InvalidStyleName, "Invalid style name");
			AddString(XtraRichEditStringId.Msg_IncorrectNumericFieldFormat, "Error: number cannot be represented in the specified format.");
			AddString(XtraRichEditStringId.Msg_SyntaxErrorInFieldPattern, "Syntax Error, {0}.");
			AddString(XtraRichEditStringId.Msg_UnmatchedQuotesInFieldPattern, "Error: pattern string contains unmatched quotes.");
			AddString(XtraRichEditStringId.Msg_UnknownSwitchArgument, "Error! Unknown switch argument.");
			AddString(XtraRichEditStringId.Msg_UnexpectedEndOfFormula, "!Unexpected end of formula.");
			AddString(XtraRichEditStringId.Msg_MissingOperator, "!Missing operator.");
			AddString(XtraRichEditStringId.Msg_ZeroDivide, "!Zero divide.");
			AddString(XtraRichEditStringId.Msg_CreateHyperlinkError, "Cannot create a hyperlink. The hyperlink in the specified range already exists.");
			AddString(XtraRichEditStringId.Msg_ClickToFollowHyperlink, "Click to follow link");
			AddString(XtraRichEditStringId.Msg_InvalidNumberingListStartAtValue, "'Start At' must be between {0} and {1} for this format");
			AddString(XtraRichEditStringId.Msg_BookmarkCreationFailing, "Bookmark with the same name already exists. Replace?");
			AddString(XtraRichEditStringId.Msg_IncorrectBookmarkName, "Bookmark name should start with a letter and contain only alphanumeric characters and underscore.");
			AddString(XtraRichEditStringId.Msg_DuplicateBookmark, "Bookmark with that name already exists in the document");
			AddString(XtraRichEditStringId.Msg_InvalidNavigateUri, "The address of this site is not valid. Check the address and try again.");
			AddString(XtraRichEditStringId.Msg_IncorrectPattern, "Incorrect pattern.");
			AddString(XtraRichEditStringId.Msg_DocumentProtectionInvalidPassword, "The password is incorrect!");
			AddString(XtraRichEditStringId.Msg_DocumentProtectionInvalidPasswordConfirmation, "The password confirmation doesn't match.");
			AddString(XtraRichEditStringId.Msg_SelectBookmarkError, "Cannot select a bookmark of inactive SubDocument.");
			AddString(XtraRichEditStringId.Msg_SelectCommentError, "Cannot select a comment of inactive SubDocument.");
			AddString(XtraRichEditStringId.Msg_NoDefaultTabs, "Default tab stops cannot be set.");
			AddString(XtraRichEditStringId.Msg_CantResetDefaultProperties, "Can not reset default style settings.");
			AddString(XtraRichEditStringId.Msg_CantDeleteDefaultStyle, "Cannot delete default style.");
			AddString(XtraRichEditStringId.Msg_NoTocEntriesFound, "No table of contents entries found.");
			AddString(XtraRichEditStringId.Msg_NumberingListNotInListCollection, "Cannot use a numbering List. The numbering list must be added to Document.NumberingLists collection");
			AddString(XtraRichEditStringId.Msg_ParagraphStyleNameAlreadyExists, "This style name already exists.");
			AddString(XtraRichEditStringId.Msg_DeleteTableStyleQuestion, "Do you want to delete style {0} from the document?");
			AddString(XtraRichEditStringId.Msg_ClickToComment, " commented:");
			AddString(XtraRichEditStringId.MenuCmd_SimpleView, "Simple View");
			AddString(XtraRichEditStringId.MenuCmd_DraftView, "Draft View");
			AddString(XtraRichEditStringId.MenuCmd_ReadingView, "Reading View");
			AddString(XtraRichEditStringId.MenuCmd_PrintView, "Print View");
			AddString(XtraRichEditStringId.MenuCmd_SimpleViewDescription, "Simple View");
			AddString(XtraRichEditStringId.MenuCmd_DraftViewDescription, "Draft View");
			AddString(XtraRichEditStringId.MenuCmd_ReadingViewDescription, "Reading View");
			AddString(XtraRichEditStringId.MenuCmd_PrintViewDescription, "Print View");
			AddString(XtraRichEditStringId.MenuCmd_FitToPage, "Fit To Page");
			AddString(XtraRichEditStringId.MenuCmd_FitToPageDescription, "Adjusts the document zoom factor to fit the entire page to the editing surface dimensions.");
			AddString(XtraRichEditStringId.MenuCmd_FitHeight, "Fit Height");
			AddString(XtraRichEditStringId.MenuCmd_FitHeightDescription, "Adjusts the document zoom factor to fit the entire page height to the editing surface dimensions.");
			AddString(XtraRichEditStringId.MenuCmd_FitWidth, "Fit Width");
			AddString(XtraRichEditStringId.MenuCmd_FitWidthDescription, "Adjusts the document zoom factor to fit the entire page width to the editing surface dimensions.");
			AddString(XtraRichEditStringId.MenuCmd_Zoom, "Zoom");
			AddString(XtraRichEditStringId.MenuCmd_ZoomDescription, "Zoom");
			AddString(XtraRichEditStringId.MenuCmd_ZoomIn, "Zoom In");
			AddString(XtraRichEditStringId.MenuCmd_ZoomInDescription, "Zoom in to get a close-up view of the document.");
			AddString(XtraRichEditStringId.MenuCmd_ZoomOut, "Zoom Out");
			AddString(XtraRichEditStringId.MenuCmd_ZoomOutDescription, "Zoom out to see more of the page at a reduced size.");
			AddString(XtraRichEditStringId.MenuCmd_Undo, "Undo");
			AddString(XtraRichEditStringId.MenuCmd_UndoDescription, "Undo the last operation.");
			AddString(XtraRichEditStringId.MenuCmd_Redo, "Redo");
			AddString(XtraRichEditStringId.MenuCmd_RedoDescription, "Redo the last operation");
			AddString(XtraRichEditStringId.MenuCmd_ClearUndo, "ClearUndo");
			AddString(XtraRichEditStringId.MenuCmd_ClearUndoDescription, "Clear Undo Buffer");
			AddString(XtraRichEditStringId.MenuCmd_ScrollDown, "Scroll Down");
			AddString(XtraRichEditStringId.MenuCmd_ScrollDownDescription, "Scroll Down");
			AddString(XtraRichEditStringId.MenuCmd_EnsureCaretVisibleVertically, "EnsureCaretVisibleVertically");
			AddString(XtraRichEditStringId.MenuCmd_EnsureCaretVisibleVerticallyDescription, "EnsureCaretVisibleVertically");
			AddString(XtraRichEditStringId.MenuCmd_EnsureCaretVisibleHorizontally, "EnsureCaretVisibleHorizontally");
			AddString(XtraRichEditStringId.MenuCmd_EnsureCaretVisibleHorizontallyDescription, "EnsureCaretVisibleHorizontally");
			AddString(XtraRichEditStringId.MenuCmd_MoveForward, "MoveForward");
			AddString(XtraRichEditStringId.MenuCmd_MoveForwardDescription, "MoveForward");
			AddString(XtraRichEditStringId.MenuCmd_MoveBackward, "MoveBackward");
			AddString(XtraRichEditStringId.MenuCmd_MoveBackwardDescription, "MoveBackward");
			AddString(XtraRichEditStringId.MenuCmd_MoveToStartOfLine, "MoveToBeginOfLine");
			AddString(XtraRichEditStringId.MenuCmd_MoveToStartOfLineDescription, "MoveToBeginOfLine");
			AddString(XtraRichEditStringId.MenuCmd_MoveToEndOfLine, "MoveToEndOfLine");
			AddString(XtraRichEditStringId.MenuCmd_MoveToEndOfLineDescription, "MoveToEndOfLine");
			AddString(XtraRichEditStringId.MenuCmd_MoveLineUp, "MoveLineUp");
			AddString(XtraRichEditStringId.MenuCmd_MoveLineUpDescription, "MoveLineUp");
			AddString(XtraRichEditStringId.MenuCmd_MoveLineDown, "MoveLineDown");
			AddString(XtraRichEditStringId.MenuCmd_MoveLineDownDescription, "MoveLineDown");
			AddString(XtraRichEditStringId.MenuCmd_MovePreviousParagraph, "MovePreviousParagraph");
			AddString(XtraRichEditStringId.MenuCmd_MovePreviousParagraphDescription, "MovePreviousParagraph");
			AddString(XtraRichEditStringId.MenuCmd_MoveNextParagraph, "MoveNextParagraph");
			AddString(XtraRichEditStringId.MenuCmd_MoveNextParagraphDescription, "MoveNextParagraph");
			AddString(XtraRichEditStringId.MenuCmd_MovePreviousWord, "MovePreviousWord");
			AddString(XtraRichEditStringId.MenuCmd_MovePreviousWordDescription, "MovePreviousWord");
			AddString(XtraRichEditStringId.MenuCmd_MoveNextWord, "MoveNextWord");
			AddString(XtraRichEditStringId.MenuCmd_MoveNextWordDescription, "MoveNextWord");
			AddString(XtraRichEditStringId.MenuCmd_MovePreviousPage, "MovePreviousPage");
			AddString(XtraRichEditStringId.MenuCmd_MovePreviousPageDescription, "MovePreviousPage");
			AddString(XtraRichEditStringId.MenuCmd_MoveNextPage, "MoveNextPage");
			AddString(XtraRichEditStringId.MenuCmd_MoveNextPageDescription, "MoveNextPage");
			AddString(XtraRichEditStringId.MenuCmd_MoveToBeginOfDocument, "MoveToBeginBeginOfDocument");
			AddString(XtraRichEditStringId.MenuCmd_MoveToBeginOfDocumentDescription, "MoveToBeginBeginOfDocument");
			AddString(XtraRichEditStringId.MenuCmd_MoveToEndOfDocument, "MoveToEndOfDocument");
			AddString(XtraRichEditStringId.MenuCmd_MoveToEndOfDocumentDescription, "MoveToEndOfDocument");
			AddString(XtraRichEditStringId.MenuCmd_MoveScreenUp, "MoveScreenUp");
			AddString(XtraRichEditStringId.MenuCmd_MoveScreenUpDescription, "MoveScreenUp");
			AddString(XtraRichEditStringId.MenuCmd_MoveScreenDown, "MoveScreenDown");
			AddString(XtraRichEditStringId.MenuCmd_MoveScreenDownDescription, "MoveScreenDown");
			AddString(XtraRichEditStringId.MenuCmd_SelectAll, "Select All");
			AddString(XtraRichEditStringId.MenuCmd_SelectAllDescription, "Select entire document content.");
			AddString(XtraRichEditStringId.MenuCmd_DeselectAll, "Deselect All");
			AddString(XtraRichEditStringId.MenuCmd_DeselectAllDescription, "Reset document selection.");
			AddString(XtraRichEditStringId.MenuCmd_SymbolFormInsertButton, "Insert");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableElement, "Insert");
			AddString(XtraRichEditStringId.MenuCmd_InsertParagraph, "InsertParagraph");
			AddString(XtraRichEditStringId.MenuCmd_InsertParagraphDescription, "InsertParagraph");
			AddString(XtraRichEditStringId.MenuCmd_InsertLineBreak, "InsertLineBreak");
			AddString(XtraRichEditStringId.MenuCmd_InsertLineBreakDescription, "InsertLineBreak");
			AddString(XtraRichEditStringId.MenuCmd_InsertText, "InsertText");
			AddString(XtraRichEditStringId.MenuCmd_InsertTextDescription, "InsertText");
			AddString(XtraRichEditStringId.MenuCmd_OvertypeText, "OvertypeText");
			AddString(XtraRichEditStringId.MenuCmd_OvertypeTextDescription, "OvertypeText");
			AddString(XtraRichEditStringId.MenuCmd_InsertMultilevelList, "Multilevel list");
			AddString(XtraRichEditStringId.MenuCmd_InsertMultilevelListDescription, "Start a multilevel list.");
			AddString(XtraRichEditStringId.MenuCmd_InsertSimpleList, "Numbering");
			AddString(XtraRichEditStringId.MenuCmd_InsertSimpleListDescription, "Start a numbered list.");
			AddString(XtraRichEditStringId.MenuCmd_InsertBulletList, "Bullets");
			AddString(XtraRichEditStringId.MenuCmd_InsertBulletListDescription, "Start a bulleted list.");
			AddString(XtraRichEditStringId.MenuCmd_InsertField, "InsertField");
			AddString(XtraRichEditStringId.MenuCmd_InsertFieldDescription, "InsertField");
			AddString(XtraRichEditStringId.MenuCmd_InsertMergeField, "Insert Merge Field");
			AddString(XtraRichEditStringId.MenuCmd_InsertMergeFieldDescription, "Insert Merge Field.");
			AddString(XtraRichEditStringId.MenuCmd_InsertPageNumberField, "Page Number");
			AddString(XtraRichEditStringId.MenuCmd_InsertPageNumberFieldDescription, "Insert page numbers into the document.");
			AddString(XtraRichEditStringId.MenuCmd_InsertPageCountField, "Page Count");
			AddString(XtraRichEditStringId.MenuCmd_InsertPageCountFieldDescription, "Insert total page count into the document.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTabToParagraph, "InsertTabToParagraph");
			AddString(XtraRichEditStringId.MenuCmd_InsertTabToParagraphDescription, "InsertTabToParagraphDescription");
			AddString(XtraRichEditStringId.MenuCmd_InsertTable, "Table");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableDescription, "Insert a table into the document.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableRowAbove, "Insert Above");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableRowAboveDescription, "Add a new row directly above the selected row.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableRowBelow, "Insert Below");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableRowBelowDescription, "Add a new row directly below the selected row.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableCells, "Insert Cells");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableCellsDescription, "Insert Cells");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableRows, "Delete Rows");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableRowsDescription, "Delete Rows");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableCells, "Delete Cells...");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableCellsDescription, "Delete rows, columns, or cells.");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableCellsMenuItem, "Delete Cells...");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTable, "Delete Table");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableDescription, "Delete Entire Table.");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableElements, "Delete");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableElementsDescription, "Delete rows, columns, cells, or the entire Table.");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableColumns, "Delete Columns");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableColumnsDescription, "Delete Columns");
			AddString(XtraRichEditStringId.MenuCmd_ShowInsertMergeFieldForm, "Insert Merge Field");
			AddString(XtraRichEditStringId.MenuCmd_ShowInsertMergeFieldFormDescription, "Add a field from a list of recipients or from a data table to the document.");
			AddString(XtraRichEditStringId.MenuCmd_DeleteNumerationFromParagraph, "DeleteNumerationFromParagraph");
			AddString(XtraRichEditStringId.MenuCmd_DeleteNumerationFromParagraphDescription, "DeleteNumerationFromParagraph");
			AddString(XtraRichEditStringId.MenuCmd_IncrementNumerationFromParagraph, "IncrementNumerationFromParagraph");
			AddString(XtraRichEditStringId.MenuCmd_IncrementNumerationFromParagraphDescription, "IncrementNumerationFromParagraph");
			AddString(XtraRichEditStringId.MenuCmd_DecrementNumerationFromParagraph, "DecrementNumerationFromParagraph");
			AddString(XtraRichEditStringId.MenuCmd_DecrementNumerationFromParagraphDescription, "DecrementNumerationFromParagraph");
			AddString(XtraRichEditStringId.MenuCmd_TabKey, "TabKey");
			AddString(XtraRichEditStringId.MenuCmd_TabKeyDescription, "TabKey");
			AddString(XtraRichEditStringId.MenuCmd_ShiftTabKey, "ShiftTabKey");
			AddString(XtraRichEditStringId.MenuCmd_ShiftTabKeyDescription, "ShiftTabKey");
			AddString(XtraRichEditStringId.MenuCmd_BackSpaceKey, "BackSpaceKey");
			AddString(XtraRichEditStringId.MenuCmd_BackSpaceKeyDescription, "BackSpaceKey");
			AddString(XtraRichEditStringId.MenuCmd_InsertTab, "InsertTab");
			AddString(XtraRichEditStringId.MenuCmd_InsertTabDescription, "InsertTab");
			AddString(XtraRichEditStringId.MenuCmd_InsertPageBreak, "Page");
			AddString(XtraRichEditStringId.MenuCmd_InsertPageBreakDescription, "Start the next page at the current position.");
			AddString(XtraRichEditStringId.MenuCmd_InsertPageBreak2, "Page Break");
			AddString(XtraRichEditStringId.MenuCmd_InsertPageBreak2Description, "Start the next page at the current position.");
			AddString(XtraRichEditStringId.MenuCmd_InsertNonBreakingSpace, "InsertNonBreakingSpace");
			AddString(XtraRichEditStringId.MenuCmd_InsertNonBreakingSpaceDescription, "InsertNonBreakingSpace");
			AddString(XtraRichEditStringId.MenuCmd_InsertColumnBreak, "Column");
			AddString(XtraRichEditStringId.MenuCmd_InsertColumnBreakDescription, "Indicate that the text following the column break will begin in the next column.");
			AddString(XtraRichEditStringId.MenuCmd_InsertEmDash, "InsertEmDash");
			AddString(XtraRichEditStringId.MenuCmd_InsertEmDashDescription, "InsertEmDash");
			AddString(XtraRichEditStringId.MenuCmd_InsertEnDash, "InsertEnDash");
			AddString(XtraRichEditStringId.MenuCmd_InsertEnDashDescription, "InsertEnDash");
			AddString(XtraRichEditStringId.MenuCmd_InsertCopyrightSymbol, "InsertCopyrightSymbol");
			AddString(XtraRichEditStringId.MenuCmd_InsertCopyrightSymbolDescription, "InsertCopyrightSymbol");
			AddString(XtraRichEditStringId.MenuCmd_InsertRegisteredTrademarkSymbol, "InsertRegisteredTrademark Symbol");
			AddString(XtraRichEditStringId.MenuCmd_InsertRegisteredTrademarkSymbolDescription, "InsertRegisteredTrademark Symbol");
			AddString(XtraRichEditStringId.MenuCmd_InsertTrademarkSymbol, "InsertTrademarkSymbol");
			AddString(XtraRichEditStringId.MenuCmd_InsertTrademarkSymbolDescription, "InsertTrademarkSymbol");
			AddString(XtraRichEditStringId.MenuCmd_InsertEllipsis, "InsertEllipsis");
			AddString(XtraRichEditStringId.MenuCmd_InsertEllipsisDescription, "InsertEllipsis");
			AddString(XtraRichEditStringId.MenuCmd_InsertClosingSingleQuotationMark, "InsertClosingSingleQuotationMark");
			AddString(XtraRichEditStringId.MenuCmd_InsertClosingSingleQuotationMarkDescription, "InsertClosingSingleQuotationMark");
			AddString(XtraRichEditStringId.MenuCmd_InsertOpeningSingleQuotationMark, "InsertOpeningSingleQuotationMark");
			AddString(XtraRichEditStringId.MenuCmd_InsertOpeningSingleQuotationMarkDescription, "InsertOpeningSingleQuotationMark");
			AddString(XtraRichEditStringId.MenuCmd_InsertClosingDoubleQuotationMark, "InsertClosingDoubleQuotationMark");
			AddString(XtraRichEditStringId.MenuCmd_InsertClosingDoubleQuotationMarkDescription, "InsertClosingDoubleQuotationMark");
			AddString(XtraRichEditStringId.MenuCmd_InsertOpeningDoubleQuotationMark, "InsertOpeningDoubleQuotationMark");
			AddString(XtraRichEditStringId.MenuCmd_InsertOpeningDoubleQuotationMarkDescription, "InsertOpeningDoubleQuotationMark");
			AddString(XtraRichEditStringId.MenuCmd_InsertSymbol, "Symbol");
			AddString(XtraRichEditStringId.MenuCmd_InsertSymbolDescription, "Insert symbols that are not on your keyboard, such as copyright symbols, trademark symbols, paragraph marks and Unicode characters.");
			AddString(XtraRichEditStringId.MenuCmd_InsertPicture, "Inline Picture");
			AddString(XtraRichEditStringId.MenuCmd_InsertPictureDescription, "Insert inline picture from a file.");
			AddString(XtraRichEditStringId.MenuCmd_InsertBreak, "Breaks");
			AddString(XtraRichEditStringId.MenuCmd_InsertBreakDescription, "Add page, section, or column breaks to the document.");
			AddString(XtraRichEditStringId.MenuCmd_InsertSectionBreakNextPage, "Section (Next Page)");
			AddString(XtraRichEditStringId.MenuCmd_InsertSectionBreakNextPageDescription, "Insert a section break and start the new section on the next page.");
			AddString(XtraRichEditStringId.MenuCmd_InsertSectionBreakOddPage, "Section (Odd Page)");
			AddString(XtraRichEditStringId.MenuCmd_InsertSectionBreakOddPageDescription, "Insert a section break and start the new section on the next odd-numbered page.");
			AddString(XtraRichEditStringId.MenuCmd_InsertSectionBreakEvenPage, "Section (Even Page)");
			AddString(XtraRichEditStringId.MenuCmd_InsertSectionBreakEvenPageDescription, "Insert a section break and start the new section on the next even-numbered page.");
			AddString(XtraRichEditStringId.MenuCmd_InsertSectionBreakContinuous, "Section (Continuous)");
			AddString(XtraRichEditStringId.MenuCmd_InsertSectionBreakContinuousDescription, "Insert a section break and start the new section on the same page.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontBold, "Bold");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontBoldDescription, "Make the selected text bold.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontItalic, "Italic");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontItalicDescription, "Italicize the selected text.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleHiddenText, "Hidden");
			AddString(XtraRichEditStringId.MenuCmd_ToggleHiddenTextDescription, "Hidden.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontUnderline, "Underline");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontUnderlineDescription, "Underline the selected text.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontDoubleUnderline, "Double Underline");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontDoubleUnderlineDescription, "Double underline");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontStrikeout, "Strikethrough");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontStrikeoutDescription, "Draw a line through the middle of the selected text.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontDoubleStrikeout, "Double Strikethrough");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFontDoubleStrikeoutDescription, "Double strikethrough");
			AddString(XtraRichEditStringId.MenuCmd_IncrementFontSize, "IncrementFontSize");
			AddString(XtraRichEditStringId.MenuCmd_IncrementFontSizeDescription, "IncrementFontSize");
			AddString(XtraRichEditStringId.MenuCmd_DecrementFontSize, "DecrementFontSize");
			AddString(XtraRichEditStringId.MenuCmd_DecrementFontSizeDescription, "DecrementFontSize");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFontColor, "Font Color");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFontColorDescription, "Change the font color.");
			AddString(XtraRichEditStringId.MenuCmd_HighlightText, "Text Highlight Color");
			AddString(XtraRichEditStringId.MenuCmd_HighlightTextDescription, "Make text look like it was marked with a highlighter pen.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFontName, "Font");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFontNameDescription, "Change the font face.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFontSize, "Font Size");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFontSizeDescription, "Change the font size.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeStyle, "Quick Styles");
			AddString(XtraRichEditStringId.MenuCmd_ChangeStyleDescription, "Format titles, quotes, and other text using this gallery of styles.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeColumnCount, "Columns");
			AddString(XtraRichEditStringId.MenuCmd_ChangeColumnCountDescription, "Columns");
			AddString(XtraRichEditStringId.MenuCmd_IncreaseFontSize, "Grow Font");
			AddString(XtraRichEditStringId.MenuCmd_IncreaseFontSizeDescription, "Increase the font size.");
			AddString(XtraRichEditStringId.MenuCmd_DecreaseFontSize, "Shrink Font");
			AddString(XtraRichEditStringId.MenuCmd_DecreaseFontSizeDescription, "Decrease the font size.");
			AddString(XtraRichEditStringId.MenuCmd_FontSuperscript, "Superscript");
			AddString(XtraRichEditStringId.MenuCmd_FontSuperscriptDescription, "Create small letters above the line of text.");
			AddString(XtraRichEditStringId.MenuCmd_FontSubscript, "Subscript");
			AddString(XtraRichEditStringId.MenuCmd_FontSubscriptDescription, "Create small letters below the text baseline.");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentLeft, "Align Text Left");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentLeftDescription, "Align text to the left.");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentCenter, "Center");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentCenterDescription, "Center text.");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentRight, "Align Text Right");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentRightDescription, "Align text to the right.");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentJustify, "Justify");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentJustifyDescription, "Align text to both left and right margins, adding extra space between words as necessary.\r\n\r\nThis creates a clean look along the left and right side of the page.");
			AddString(XtraRichEditStringId.MenuCmd_SetSingleParagraphSpacing, "1.0");
			AddString(XtraRichEditStringId.MenuCmd_SetSingleParagraphSpacingDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetSesquialteralParagraphSpacing, "1.5");
			AddString(XtraRichEditStringId.MenuCmd_SetSesquialteralParagraphSpacingDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetDoubleParagraphSpacing, "2.0");
			AddString(XtraRichEditStringId.MenuCmd_SetDoubleParagraphSpacingDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_AddSpacingBeforeParagraph, "Add Space &Before Paragraph");
			AddString(XtraRichEditStringId.MenuCmd_AddSpacingBeforeParagraphDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_AddSpacingAfterParagraph, "Add Space &After Paragraph");
			AddString(XtraRichEditStringId.MenuCmd_AddSpacingAfterParagraphDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_RemoveSpacingBeforeParagraph, "Remove Space &Before Paragraph");
			AddString(XtraRichEditStringId.MenuCmd_RemoveSpacingBeforeParagraphDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_RemoveSpacingAfterParagraph, "Remove Space &After Paragraph");
			AddString(XtraRichEditStringId.MenuCmd_RemoveSpacingAfterParagraphDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphBackColor, "Shading");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphBackColorDescription, "Change the background behind the selected paragraph.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeTableCellShading, "Shading");
			AddString(XtraRichEditStringId.MenuCmd_ChangeTableCellShadingDescription, "Color the background behind the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsAllBorders, "&All Borders");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsAllBordersDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsOutsideBorder, "Out&side Borders");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsOutsideBorderDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideBorder, "&Inside Borders");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideBorderDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopBorder, "To&p Border");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopBorderDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomBorder, "&Bottom Border");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomBorderDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsLeftBorder, "&Left Border");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsLeftBorderDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsRightBorder, "&Right Border");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsRightBorderDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideHorizontalBorder, "Inside &Horizontal Border");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideHorizontalBorderDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideVerticalBorder, "Inside &Vertical Border");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideVerticalBorderDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ResetTableCellsBorders, "&No Border");
			AddString(XtraRichEditStringId.MenuCmd_ResetTableCellsBordersDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemLineStyle, "Line Style");
			AddString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemLineStyleDescription, "Change the style of the line used to draw borders.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemLineThickness, "Line Weight");
			AddString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemLineThicknessDescription, "Change the width of the line used to draw borders.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemColor, "Pen Color");
			AddString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemColorDescription, "Change the pen color.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeTableBorders, "Borders");
			AddString(XtraRichEditStringId.MenuCmd_ChangeTableBordersDescription, "Customize the borders of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeTableCellAlignmentPlaceholder, "Alignment");
			AddString(XtraRichEditStringId.MenuCmd_ChangeTableCellAlignmentPlaceholderDescription, "Customize the alignment of the selected cells.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeTableCellsContentAlignment, "Cell Alignment");
			AddString(XtraRichEditStringId.MenuCmd_ChangeTableCellsContentAlignmentDescription, "Cell Alignment");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopLeftAlignment, "Align Top Left");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopLeftAlignmentDescription, "Align text to the top left corner of the cell.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopCenterAlignment, "Align Top Center");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopCenterAlignmentDescription, "Center text and align it to the top of the cell.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopRightAlignment, "Align Top Right");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopRightAlignmentDescription, "Align text to the top right corner of the cell.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleLeftAlignment, "Align Center Left");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleLeftAlignmentDescription, "Center text vertically and align it to the left side of the cell.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleCenterAlignment, "Align Center");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleCenterAlignmentDescription, "Center text horizontally and vertically within the cell.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleRightAlignment, "Align Center Right");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleRightAlignmentDescription, "Center text vertically and align it to the right side of the cell.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomLeftAlignment, "Align Bottom Left");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomLeftAlignmentDescription, "Align text to the bottom left corner of the cell.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomCenterAlignment, "Align Bottom Center");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomCenterAlignmentDescription, "Center text and align it to the bottom of the cell.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomRightAlignment, "Align Bottom Right");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomRightAlignmentDescription, "Align text to the bottom right corner of the cell.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableAutoFitPlaceholder, "AutoFit");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableAutoFitPlaceholderDescription, "Automatically resize the column widths based on the text in them.\r\n\r\nYou can set the table width based on the window size or convert it back to use fixed column widths.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableAutoFitContents, "AutoFit Contents");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableAutoFitContentsDescription, "Auto-Fit Table to the contents.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableAutoFitWindow, "AutoFit Window");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableAutoFitWindowDescription, "Auto-Fit Table to the window.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableFixedColumnWidth, "Fixed Column Width");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTableFixedColumnWidthDescription, "Set table size to a fixed width.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeLanguage, "Language");
			AddString(XtraRichEditStringId.MenuCmd_ChangeLanguageDescription, "Set the language used to check the spelling and grammar of the selected text.");
			AddString(XtraRichEditStringId.MenuCmd_Language, "Language");
			AddString(XtraRichEditStringId.MenuCmd_LanguageDescription, "Set the language used to check the spelling and grammar of the selected text.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeNoProof, "NoProof");
			AddString(XtraRichEditStringId.MenuCmd_ChangeNoProofDescription, "Change NoProof");
			AddString(XtraRichEditStringId.MenuCmd_Comment, "View Comments");
			AddString(XtraRichEditStringId.MenuCmd_CommentDescription, "Choose whether or not to highlight comments in the document.");
			AddString(XtraRichEditStringId.MenuCmd_NewComment, "New Comment");
			AddString(XtraRichEditStringId.MenuCmd_NewCommentDescription, "Add a comment about the selection.");
			AddString(XtraRichEditStringId.MenuCmd_SelectComment, "Select Comment");
			AddString(XtraRichEditStringId.MenuCmd_SelectCommentDescription, "Select Comment");
			AddString(XtraRichEditStringId.MenuCmd_DeleteComment, "Delete");
			AddString(XtraRichEditStringId.MenuCmd_DeleteCommentDescription, "Delete the selected comment.");
			AddString(XtraRichEditStringId.MenuCmd_DeleteOneComment, "Delete Comment");
			AddString(XtraRichEditStringId.MenuCmd_DeleteOneCommentDescription, "Delete the selected comment.");
			AddString(XtraRichEditStringId.MenuCmd_DeleteAllCommentsShown, "Delete All Comments Shown");
			AddString(XtraRichEditStringId.MenuCmd_DeleteAllCommentsShownDescription, "Delete All Comments Shown.");
			AddString(XtraRichEditStringId.MenuCmd_DeleteAllComments, "Delete All Comments");
			AddString(XtraRichEditStringId.MenuCmd_DeleteAllCommentsDescription, "Delete All Comments.");
			AddString(XtraRichEditStringId.MenuCmd_PreviousComment, "Previous");
			AddString(XtraRichEditStringId.MenuCmd_PreviousCommentDescription, "Navigate to the previous comment in the document.");
			AddString(XtraRichEditStringId.MenuCmd_NextComment, "Next");
			AddString(XtraRichEditStringId.MenuCmd_NextCommentDescription, "Navigate to the next comment in the document.");
			AddString(XtraRichEditStringId.MenuCmd_ShowCommentForm, "Comment");
			AddString(XtraRichEditStringId.MenuCmd_ShowCommentFormDescription, "Show the Comment dialog box.");
			AddString(XtraRichEditStringId.MenuCmd_ReviewingPane, "Reviewing Pane");
			AddString(XtraRichEditStringId.MenuCmd_ReviewingPaneDescription, "Show or hide the document comments in a separate window.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleAuthorVisibility, "Toggle Authors Visibility");
			AddString(XtraRichEditStringId.MenuCmd_ToggleAuthorVisibilityDescription, "Toggle Authors Visibility");
			AddString(XtraRichEditStringId.MenuCmd_Reviewers, "Reviewers");
			AddString(XtraRichEditStringId.MenuCmd_ReviewersDescription, "Select which reviewer comments to highlight in the document.");
			AddString(XtraRichEditStringId.MenuCmd_Delete, "Delete");
			AddString(XtraRichEditStringId.MenuCmd_DeleteDescription, "Delete");
			AddString(XtraRichEditStringId.MenuCmd_DeleteCore, "DeleteCore");
			AddString(XtraRichEditStringId.MenuCmd_DeleteCoreDescription, "DeleteCoreDescription");
			AddString(XtraRichEditStringId.MenuCmd_DeleteBack, "DeleteBack");
			AddString(XtraRichEditStringId.MenuCmd_DeleteBackDescription, "DeleteBack");
			AddString(XtraRichEditStringId.MenuCmd_DeleteBackCore, "DeleteBackCore");
			AddString(XtraRichEditStringId.MenuCmd_DeleteBackCoreDescription, "DeleteBackCoreDescription");
			AddString(XtraRichEditStringId.MenuCmd_DeleteWord, "DeleteWord");
			AddString(XtraRichEditStringId.MenuCmd_DeleteWordDescription, "DeleteWord");
			AddString(XtraRichEditStringId.MenuCmd_DeleteWordCore, "DeleteWordCore");
			AddString(XtraRichEditStringId.MenuCmd_DeleteWordCoreDescription, "DeleteWordCoreDescription");
			AddString(XtraRichEditStringId.MenuCmd_DeleteWordBack, "DeleteWordBack");
			AddString(XtraRichEditStringId.MenuCmd_DeleteWordBackDescription, "DeleteWordBack");
			AddString(XtraRichEditStringId.MenuCmd_DeleteWordBackCore, "DeleteWordBack");
			AddString(XtraRichEditStringId.MenuCmd_DeleteWordBackCoreDescription, "DeleteWordBack");
			AddString(XtraRichEditStringId.MenuCmd_CopySelection, "Copy");
			AddString(XtraRichEditStringId.MenuCmd_CopySelectionDescription, "Copy the selection and put it on the Clipboard.");
			AddString(XtraRichEditStringId.MenuCmd_Paste, "Paste");
			AddString(XtraRichEditStringId.MenuCmd_PasteDescription, "Paste the contents of the Clipboard.");
			AddString(XtraRichEditStringId.MenuCmd_PastePlainText, "Unformatted text");
			AddString(XtraRichEditStringId.MenuCmd_PastePlainTextDescription, "Inserts the contents of the Clipboard as text without any formatting.");
			AddString(XtraRichEditStringId.MenuCmd_PasteRtfText, "Formatted text (RTF)");
			AddString(XtraRichEditStringId.MenuCmd_PasteRtfTextDescription, "Inserts the contents of the Clipboard as text with font and table formatting.");
			AddString(XtraRichEditStringId.MenuCmd_PasteSilverlightXamlText, "Formatted text (XAML)");
			AddString(XtraRichEditStringId.MenuCmd_PasteSilverlightXamlTextDescription, "Inserts the contents of the Clipboard as text with font formatting.");
			AddString(XtraRichEditStringId.MenuCmd_PasteHtmlText, "HTML Format");
			AddString(XtraRichEditStringId.MenuCmd_PasteHtmlTextDescription, "Inserts the contents of the Clipboard as HTML format.");
			AddString(XtraRichEditStringId.MenuCmd_PasteImage, "Picture");
			AddString(XtraRichEditStringId.MenuCmd_PasteImageDescription, "Inserts the contents of the Clipboard as picture.");
			AddString(XtraRichEditStringId.MenuCmd_PasteMetafileImage, "Metafile");
			AddString(XtraRichEditStringId.MenuCmd_PasteMetafileImageDescription, "Inserts the contents of the Clipboard as metafile.");
			AddString(XtraRichEditStringId.MenuCmd_PasteFiles, "Files");
			AddString(XtraRichEditStringId.MenuCmd_PasteFilesDescription, "Inserts the contents of the Clipboard as an embedded file.");
			AddString(XtraRichEditStringId.MenuCmd_ShowPasteSpecialForm, "Paste Special");
			AddString(XtraRichEditStringId.MenuCmd_ShowPasteSpecialFormDescription, "Paste Special");
			AddString(XtraRichEditStringId.MenuCmd_CutSelection, "Cut");
			AddString(XtraRichEditStringId.MenuCmd_CutSelectionDescription, "Cut the selection from the document and put it on the Clipboard.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleWhitespace, "Show/Hide ¶");			
			AddString(XtraRichEditStringId.MenuCmd_ToggleWhitespaceDescription, "Show paragraph marks and other hidden formatting symbols.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleOvertype, "Overtype");
			AddString(XtraRichEditStringId.MenuCmd_ToggleOvertypeDescription, "Overtype");
			AddString(XtraRichEditStringId.MenuCmd_ToggleShowTableGridLines, "View &Gridlines");
			AddString(XtraRichEditStringId.MenuCmd_ToggleShowTableGridLinesDescription, "Show or hide the gridlines within the table.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleShowHorizontalRuler, "Horizontal Ruler");
			AddString(XtraRichEditStringId.MenuCmd_ToggleShowHorizontalRulerDescription, "View the horizontal ruler, used to measure and line up objects in the document.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleShowVerticalRuler, "Vertical Ruler");
			AddString(XtraRichEditStringId.MenuCmd_ToggleShowVerticalRulerDescription, "View the vertical ruler, used to measure and line up objects in the document.");
			AddString(XtraRichEditStringId.MenuCmd_FindAndSelectForward, "FindAndSelectForward");
			AddString(XtraRichEditStringId.MenuCmd_FindAndSelectForwardDescription, "FindAndSelectForward");
			AddString(XtraRichEditStringId.MenuCmd_FindAndSelectBackward, "FindAndSelectBackward");
			AddString(XtraRichEditStringId.MenuCmd_FindAndSelectBackwardDescription, "FindAndSelectBackward");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceForward, "ReplaceForward");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceForwardDescription, "ReplaceForward");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceBackward, "ReplaceBackward");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceBackwardDescription, "ReplaceBackward");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceAllForward, "ReplaceAllForward");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceAllForwardDescription, "ReplaceAllForwardDescription");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceAllBackward, "ReplaceAllBackward");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceAllBackwardDescription, "ReplaceAllBackwardDescription");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceText, "ReplaceText");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceTextDescription, "ReplaceTextDescription");
			AddString(XtraRichEditStringId.MenuCmd_FindNext, "Find Next");
			AddString(XtraRichEditStringId.MenuCmd_FindNextDescription, "Repeats the last search forward.");
			AddString(XtraRichEditStringId.MenuCmd_FindPrev, "Find Prev");
			AddString(XtraRichEditStringId.MenuCmd_FindPrevDescription, "Repeats the last search backward.");
			AddString(XtraRichEditStringId.MenuCmd_NewEmptyDocument, "New");
			AddString(XtraRichEditStringId.MenuCmd_NewEmptyDocumentDescription, "Create a new document.");
			AddString(XtraRichEditStringId.MenuCmd_LoadDocument, "Open");
			AddString(XtraRichEditStringId.MenuCmd_LoadDocumentDescription, "Open a document.");
			AddString(XtraRichEditStringId.MenuCmd_SaveDocument, "Save");
			AddString(XtraRichEditStringId.MenuCmd_SaveDocumentDescription, "Save the document.");
			AddString(XtraRichEditStringId.MenuCmd_ShowFontForm, "Font...");
			AddString(XtraRichEditStringId.MenuCmd_ShowFontFormDescription, "Show the Font dialog box.");
			AddString(XtraRichEditStringId.MenuCmd_ShowParagraphForm, "Paragraph...");
			AddString(XtraRichEditStringId.MenuCmd_ShowParagraphFormDescription, "Show the Paragraph dialog box.");
			AddString(XtraRichEditStringId.MenuCmd_ShowEditStyleForm, "Modify Style...");
			AddString(XtraRichEditStringId.MenuCmd_ShowEditStyleFormDescription, "Show the Edit Style dialog box.");
			AddString(XtraRichEditStringId.MenuCmd_ShowSymbol, "Symbol");
			AddString(XtraRichEditStringId.MenuCmd_ShowSymbolDescription, "Show the Symbol dialog box.");
			AddString(XtraRichEditStringId.MenuCmd_ShowBookmarkForm, "Bookmark");
			AddString(XtraRichEditStringId.MenuCmd_ShowBookmarkFormDescription, "Create a bookmark to assign a name to a specific point in a document\r\n\r\nYou can make hyperlinks that jump directly to a bookmarked location.");
			AddString(XtraRichEditStringId.MenuCmd_ShowHyperlinkForm, "Hyperlink");
			AddString(XtraRichEditStringId.MenuCmd_ShowHyperlinkFormDescription, "Create a link to a Web page, a picture, an e-mail address, or a program.");
			AddString(XtraRichEditStringId.MenuCmd_ShowRangeEditingPermissionsForm, "Range Editing Permissions");
			AddString(XtraRichEditStringId.MenuCmd_ShowRangeEditingPermissionsFormDescription, "Grant user permissions to edit the selected part of the document.");
			AddString(XtraRichEditStringId.MenuCmd_ShowNumberingList, "Bullets and Numbering...");
			AddString(XtraRichEditStringId.MenuCmd_ShowNumberingListDescription, "Show the Numbered List dialog box.");
			AddString(XtraRichEditStringId.MenuCmd_ShowFloatingObjectLayoutOptionsForm, "More Layout Options...");
			AddString(XtraRichEditStringId.MenuCmd_ShowFloatingObjectLayoutOptionsFormDescription, "Show the Layout dialog box.");
			AddString(XtraRichEditStringId.MenuCmd_ShowTabsForm, "Tabs...");
			AddString(XtraRichEditStringId.MenuCmd_ShowTabsFormDescription, "Tabs");
			AddString(XtraRichEditStringId.MenuCmd_ShowLineSpacingForm, "Line Spacing Options...");
			AddString(XtraRichEditStringId.MenuCmd_ShowLineSpacingFormDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SaveDocumentAs, "Save As");
			AddString(XtraRichEditStringId.MenuCmd_SaveDocumentAsDescription, "Open the Save As dialog box to select a file format and save the document to a new location.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphStyle, "ChangeParagraphStyle");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphStyleDescription, "ChangeParagraphStyle");
			AddString(XtraRichEditStringId.MenuCmd_PlaceCaretToPhysicalPoint, "PlaceCaretToPhysicalPoint");
			AddString(XtraRichEditStringId.MenuCmd_PlaceCaretToPhysicalPointDescription, "PlaceCaretToPhysicalPoint");
			AddString(XtraRichEditStringId.MenuCmd_ChangeCharacterStyle, "ChangeCharacterStyle");
			AddString(XtraRichEditStringId.MenuCmd_ChangeCharacterStyleDescription, "ChangeCharacterStyle");
			AddString(XtraRichEditStringId.MenuCmd_SelectFieldNextToCaret, "SelectFieldNextToCaret");
			AddString(XtraRichEditStringId.MenuCmd_SelectFieldNextToCaretDescription, "SelectFieldNextToCaretDescription");
			AddString(XtraRichEditStringId.MenuCmd_SelectFieldPrevToCaret, "SelectFieldPrevToCaret");
			AddString(XtraRichEditStringId.MenuCmd_SelectFieldPrevToCaretDescription, "SelectFieldPrevToCaretDescription");
			AddString(XtraRichEditStringId.MenuCmd_EnterKey, "EnterKey");
			AddString(XtraRichEditStringId.MenuCmd_EnterKeyDescription, "EnterKeyDescription");
			AddString(XtraRichEditStringId.MenuCmd_CheckSpelling, "Spelling");
			AddString(XtraRichEditStringId.MenuCmd_CheckSpellingDescription, "Check the spelling of text in the document.");
			AddString(XtraRichEditStringId.MenuCmd_CheckSyntax, "CheckSyntax");
			AddString(XtraRichEditStringId.MenuCmd_CheckSyntaxDescription, "CheckSyntax");
			AddString(XtraRichEditStringId.MenuCmd_IgnoreMistakenWord, "Ignore");
			AddString(XtraRichEditStringId.MenuCmd_IgnoreMistakenWordDescription, "Ignore");
			AddString(XtraRichEditStringId.MenuCmd_IgnoreAllMistakenWords, "Ignore All");
			AddString(XtraRichEditStringId.MenuCmd_IgnoreAllMistakenWordsDescription, "Ignore All");
			AddString(XtraRichEditStringId.MenuCmd_AddWordToDictionary, "Add to Dictionary");
			AddString(XtraRichEditStringId.MenuCmd_AddWordToDictionaryDescription, "Add to Dictionary");
			AddString(XtraRichEditStringId.MenuCmd_DeleteRepeatedWord, "Delete Repeated Word");
			AddString(XtraRichEditStringId.MenuCmd_DeleteRepeatedWordDescription, "Delete Repeated Word");
			AddString(XtraRichEditStringId.MenuCmd_ToggleSpellCheckAsYouType, "Check Spelling As You Type");
			AddString(XtraRichEditStringId.MenuCmd_ToggleSpellCheckAsYouTypeDescription, "Check Spelling As You Type");
			AddString(XtraRichEditStringId.MenuCmd_ChangeColumnSize, "ChangeColumnSize");
			AddString(XtraRichEditStringId.MenuCmd_ChangeColumnSizeDescription, "ChangeColumnSizeDescription");
			AddString(XtraRichEditStringId.MenuCmd_ChangeIndent, "ChangeIndent");
			AddString(XtraRichEditStringId.MenuCmd_ChangeIndentDescription, "ChangeIndentDescription");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphLeftIndent, "ChangeParagraphLeftIndent");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphLeftIndentDescription, "ChangeParagraphLeftIndentDescription");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphRightIndent, "ChangeParagraphRightIndent");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphRightIndentDescription, "ChangeParagraphRightIndentDescription");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphFirstLineIndent, "IncrementParagraphFirstLineIndent");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphFirstLineIndentDescription, "IncrementParagraphFirstLineIndentDescription");
			AddString(XtraRichEditStringId.MenuCmd_IncrementIndent, "Increase Indent");
			AddString(XtraRichEditStringId.MenuCmd_IncrementIndentDescription, "Increase the indent level of the paragraph.");
			AddString(XtraRichEditStringId.MenuCmd_DecrementParagraphLeftIndent, "DecrementParagraphLeftIndent");
			AddString(XtraRichEditStringId.MenuCmd_DecrementParagraphLeftIndentDescription, "DecrementParagraphLeftIndentDescription");
			AddString(XtraRichEditStringId.MenuCmd_DecrementIndent, "Decrease Indent");
			AddString(XtraRichEditStringId.MenuCmd_DecrementIndentDescription, "Decrease the indent level of the paragraph.");
			AddString(XtraRichEditStringId.MenuCmd_ScrollToPage, "ScrollToPage");
			AddString(XtraRichEditStringId.MenuCmd_ScrollToPageDescription, "ScrollToPage");
			AddString(XtraRichEditStringId.MenuCmd_CreateField, "Create Field");
			AddString(XtraRichEditStringId.MenuCmd_CreateFieldDescription, "Create Field");
			AddString(XtraRichEditStringId.MenuCmd_CreateBookmark, "Create Bookmark");
			AddString(XtraRichEditStringId.MenuCmd_CreateBookmarkDescription, "Create Bookmark");
			AddString(XtraRichEditStringId.MenuCmd_SelectBookmark, "Select Bookmark");
			AddString(XtraRichEditStringId.MenuCmd_SelectBookmarkDescription, "Select Bookmark");
			AddString(XtraRichEditStringId.MenuCmd_DeleteBookmark, "Delete Bookmark");
			AddString(XtraRichEditStringId.MenuCmd_DeleteBookmarkDescription, "Delete Bookmark");
			AddString(XtraRichEditStringId.MenuCmd_UpdateField, "Update Field");
			AddString(XtraRichEditStringId.MenuCmd_UpdateFieldDescription, "Update Field");
			AddString(XtraRichEditStringId.MenuCmd_UpdateFields, "Update Fields");
			AddString(XtraRichEditStringId.MenuCmd_UpdateFieldsDescription, "Update Fields");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFieldCodes, "Toggle Field Codes");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFieldCodesDescription, "Toggle Field Codes");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFieldLocked, "Toggle Field Locked");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFieldLockedDescription, "Toggle Field Locked");
			AddString(XtraRichEditStringId.MenuCmd_LockField, "Lock Field");
			AddString(XtraRichEditStringId.MenuCmd_LockFieldDescription, "Lock Field");
			AddString(XtraRichEditStringId.MenuCmd_UnlockField, "Unlock Field");
			AddString(XtraRichEditStringId.MenuCmd_UnlockFieldDescription, "Unlock Field");
			AddString(XtraRichEditStringId.MenuCmd_ShowAllFieldCodes, "Show All Field Codes");
			AddString(XtraRichEditStringId.MenuCmd_ShowAllFieldCodesDescription, "View the document markup with dynamic elements displaying their rich-text codes.");
			AddString(XtraRichEditStringId.MenuCmd_ShowAllFieldResults, "Show All Field Results");
			AddString(XtraRichEditStringId.MenuCmd_ShowAllFieldResultsDescription, "View the document content with dynamic elements displaying real data.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleViewMergedData, "View Merged Data");
			AddString(XtraRichEditStringId.MenuCmd_ToggleViewMergedDataDescription, "Replaces the merge fields in your document with actual data from your recipient list so you can see what it looks like.");
			AddString(XtraRichEditStringId.MenuCmd_OpenHyperlink, "Open hyperlink");
			AddString(XtraRichEditStringId.MenuCmd_OpenHyperlinkDescription, "Open hyperlink");
			AddString(XtraRichEditStringId.MenuCmd_RemoveHyperlink, "Remove hyperlink");
			AddString(XtraRichEditStringId.MenuCmd_RemoveHyperlinkDescription, "Remove hyperlink");
			AddString(XtraRichEditStringId.MenuCmd_EditHyperlink, "Edit hyperlink...");
			AddString(XtraRichEditStringId.MenuCmd_EditHyperlinkDescription, "Edit hyperlink...");
			AddString(XtraRichEditStringId.MenuCmd_Find, "Find");
			AddString(XtraRichEditStringId.MenuCmd_FindDescription, "Find text in the document.");
			AddString(XtraRichEditStringId.MenuCmd_Replace, "Replace");
			AddString(XtraRichEditStringId.MenuCmd_ReplaceDescription, "Replace text in the document.");
			AddString(XtraRichEditStringId.MenuCmd_Print, "&Print");
			AddString(XtraRichEditStringId.MenuCmd_PrintDescription, "Select a printer, number of copies, and other printing options before printing.");
			AddString(XtraRichEditStringId.MenuCmd_QuickPrint, "&Quick Print");
			AddString(XtraRichEditStringId.MenuCmd_QuickPrintDescription, "Send the document directly to the default printer without making changes.");
			AddString(XtraRichEditStringId.MenuCmd_PrintPreview, "Print Pre&view");
			AddString(XtraRichEditStringId.MenuCmd_PrintPreviewDescription, "Preview pages before printing.");
			AddString(XtraRichEditStringId.MenuCmd_BrowserPrint, "Browser Print");
			AddString(XtraRichEditStringId.MenuCmd_BrowserPrintDescription, "Print document with browser print capabilities.");
			AddString(XtraRichEditStringId.MenuCmd_BrowserPrintPreview, "Browser Print Preview");
			AddString(XtraRichEditStringId.MenuCmd_BrowserPrintPreviewDescription, "Preview pages in browser before printing.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeMistakenWord, "(no spelling suggestions)");
			AddString(XtraRichEditStringId.MenuCmd_ChangeMistakenWordDescription, "(no spelling suggestions)");
			AddString(XtraRichEditStringId.MenuCmd_ClearFormatting, "Clear Formatting");
			AddString(XtraRichEditStringId.MenuCmd_ClearFormattingDescription, "Clear all the formatting from the selection, leaving only plain text.");
			AddString(XtraRichEditStringId.MenuCmd_CreateHyperlink, "CreateHyperlink");
			AddString(XtraRichEditStringId.MenuCmd_CreateHyperlinkDescription, "CreateHyperlinkDescription");
			AddString(XtraRichEditStringId.MenuCmd_InsertHyperlink, "InsertHyperlink");
			AddString(XtraRichEditStringId.MenuCmd_InsertHyperlinkDescription, "InsertHyperlinkDescription");
			AddString(XtraRichEditStringId.MenuCmd_OpenHyperlinkAtCaretPosition, "OpenHyperlinkAtCaretPosition");
			AddString(XtraRichEditStringId.MenuCmd_OpenHyperlinkAtCaretPositionDescription, "OpenHyperlinkAtCaretPositionDescription");
			AddString(XtraRichEditStringId.MenuCmd_Hyperlink, "Hyperlink...");
			AddString(XtraRichEditStringId.MenuCmd_HyperlinkDescription, "Hyperlink...");
			AddString(XtraRichEditStringId.MenuCmd_Bookmark, "Bookmark...");
			AddString(XtraRichEditStringId.MenuCmd_BookmarkDescription, "Bookmark...");
			AddString(XtraRichEditStringId.MenuCmd_CollapseOrExpandFormulaBar, "Collapse or expand Formula Bar");
			AddString(XtraRichEditStringId.MenuCmd_CollapseOrExpandFormulaBarDescription, "Collapse or expand Formula Bar (Ctrl+Shift+U)");
			AddString(XtraRichEditStringId.MenuCmd_SwitchToDraftView, "Draft View");
			AddString(XtraRichEditStringId.MenuCmd_SwitchToDraftViewDescription, "View the document as a draft to quickly edit the text.\r\n\r\nCertain elements of the document such as headers and footers will not be visible in this view.");
			AddString(XtraRichEditStringId.MenuCmd_SwitchToPrintLayoutView, "Print Layout");
			AddString(XtraRichEditStringId.MenuCmd_SwitchToPrintLayoutViewDescription, "View the document as it will appear on the printed page.");
			AddString(XtraRichEditStringId.MenuCmd_SwitchToSimpleView, "Simple View");
			AddString(XtraRichEditStringId.MenuCmd_SwitchToSimpleViewDescription, "View the document as a simple memo.\r\n\r\nThis view ignores the page layout to draw attention to text editing.");
			AddString(XtraRichEditStringId.MenuCmd_EditPageHeader, "Header");
			AddString(XtraRichEditStringId.MenuCmd_EditPageHeaderDescription, "Edit the header of the document.\r\n\r\nThe content in the header will appear at the top of each printed page.");
			AddString(XtraRichEditStringId.MenuCmd_EditPageFooter, "Footer");
			AddString(XtraRichEditStringId.MenuCmd_EditPageFooterDescription, "Edit the footer of the document.\r\n\r\nThe content in the footer will appear at the bottom of each printed page.");
			AddString(XtraRichEditStringId.MenuCmd_ClosePageHeaderFooter, "Close Header and Footer");
			AddString(XtraRichEditStringId.MenuCmd_ClosePageHeaderFooterDescription, "Close the Header and Footer Tools.\r\n\r\nYou can also double click the document area to return to editing it.");
			AddString(XtraRichEditStringId.MenuCmd_GoToPage, "Go To Page");
			AddString(XtraRichEditStringId.MenuCmd_GoToPageDescription, "Go To Page");
			AddString(XtraRichEditStringId.MenuCmd_GoToPageHeader, "Go to Header");
			AddString(XtraRichEditStringId.MenuCmd_GoToPageHeaderDescription, "Activate the header on this page so that you can edit it.");
			AddString(XtraRichEditStringId.MenuCmd_GoToPageFooter, "Go to Footer");
			AddString(XtraRichEditStringId.MenuCmd_GoToPageFooterDescription, "Activate the footer on this page so that you can edit it.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleHeaderFooterLinkToPrevious, "Link to Previous");
			AddString(XtraRichEditStringId.MenuCmd_ToggleHeaderFooterLinkToPreviousDescription, "Link to the previous section so that the header and footer in the current section contain the same content as in the previous section.");
			AddString(XtraRichEditStringId.MenuCmd_GoToPreviousHeaderFooter, "Show Previous");
			AddString(XtraRichEditStringId.MenuCmd_GoToPreviousHeaderFooterDescription, "Navigate to the previous section's header or footer.");
			AddString(XtraRichEditStringId.MenuCmd_GoToNextHeaderFooter, "Show Next");
			AddString(XtraRichEditStringId.MenuCmd_GoToNextHeaderFooterDescription, "Navigate to the next section's header or footer.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleDifferentFirstPage, "Different First Page");
			AddString(XtraRichEditStringId.MenuCmd_ToggleDifferentFirstPageDescription, "Specify a unique header and footer for the first page of the document.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleDifferentOddAndEvenPages, "Different Odd && Even Pages");
			AddString(XtraRichEditStringId.MenuCmd_ToggleDifferentOddAndEvenPagesDescription, "Specify that odd-numbered pages should have a different header and footer from even-numbered pages.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphLineSpacing, "Line Spacing");
			AddString(XtraRichEditStringId.MenuCmd_ChangeParagraphLineSpacingDescription, "Change the spacing between lines of text.\r\n\r\nYou can also customize the amount of space added before and after paragraphs.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeSectionPageOrientation, "Orientation");
			AddString(XtraRichEditStringId.MenuCmd_ChangeSectionPageOrientationDescription, "Switch the pages between portrait and landscape layouts.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeSectionPagePaperKind, "Size");
			AddString(XtraRichEditStringId.MenuCmd_ChangeSectionPagePaperKindDescription, "Choose a paper size for the current section.");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionOneColumn, "One");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionOneColumnDescription, "One column.");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionTwoColumns, "Two");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionTwoColumnsDescription, "Two columns.");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionThreeColumns, "Three");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionThreeColumnsDescription, "Three columns.");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionColumns, "Columns");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionColumnsDescription, "Split text into two or more columns.");
			AddString(XtraRichEditStringId.MenuCmd_SetLandscapePageOrientation, "Landscape");
			AddString(XtraRichEditStringId.MenuCmd_SetLandscapePageOrientationDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetPortraitPageOrientation, "Portrait");
			AddString(XtraRichEditStringId.MenuCmd_SetPortraitPageOrientationDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_ChangeSectionPageMargins, "Margins");
			AddString(XtraRichEditStringId.MenuCmd_ChangeSectionPageMarginsDescription, "Select the margin sizes for the entire document or the current section.");
			AddString(XtraRichEditStringId.MenuCmd_SetNormalSectionPageMargins, "Normal\r\nTop:\t{1,10}\tBottom:\t{3,10}\r\nLeft:\t{0,10}\tRight:\t\t{2,10}");
			AddString(XtraRichEditStringId.MenuCmd_SetNormalSectionPageMarginsDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetNarrowSectionPageMargins, "Narrow\r\nTop:\t{1,10}\tBottom:\t{3,10}\r\nLeft:\t{0,10}\tRight:\t\t{2,10}");
			AddString(XtraRichEditStringId.MenuCmd_SetNarrowSectionPageMarginsDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetModerateSectionPageMargins, "Moderate\r\nTop:\t{1,10}\tBottom:\t{3,10}\r\nLeft:\t{0,10}\tRight:\t\t{2,10}");
			AddString(XtraRichEditStringId.MenuCmd_SetModerateSectionPageMarginsDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetWideSectionPageMargins, "Wide\r\nTop:\t{1,10}\tBottom:\t{3,10}\r\nLeft:\t{0,10}\tRight:\t\t{2,10}");
			AddString(XtraRichEditStringId.MenuCmd_SetWideSectionPageMarginsDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_NextDataRecord, "Next Record");
			AddString(XtraRichEditStringId.MenuCmd_NextDataRecordDescription, "Next Record");
			AddString(XtraRichEditStringId.MenuCmd_PreviousDataRecord, "Previous Record");
			AddString(XtraRichEditStringId.MenuCmd_PreviousDataRecordDescription, "Previous Record");
			AddString(XtraRichEditStringId.MenuCmd_LastDataRecord, "Last Record");
			AddString(XtraRichEditStringId.MenuCmd_LastDataRecordDescription, "Last Record");
			AddString(XtraRichEditStringId.MenuCmd_FirstDataRecord, "First Record");
			AddString(XtraRichEditStringId.MenuCmd_FirstDataRecordDescription, "First Record");
			AddString(XtraRichEditStringId.MenuCmd_MailMergeSaveDocumentAsCommand, "Mail Merge");
			AddString(XtraRichEditStringId.MenuCmd_MailMergeSaveDocumentAsCommandDescription, "Mail Merge");
			AddString(XtraRichEditStringId.MenuCmd_EditTOC, "Edit Table of Contents...");
			AddString(XtraRichEditStringId.MenuCmd_None, " ");
			AddString(XtraRichEditStringId.FileFilterDescription_AllFiles, "All Files");
			AddString(XtraRichEditStringId.FileFilterDescription_AllSupportedFiles, "All Supported Files");
			AddString(XtraRichEditStringId.FileFilterDescription_RtfFiles, "Rich Text Format");
			AddString(XtraRichEditStringId.FileFilterDescription_HtmlFiles, "HyperText Markup Language Format");
			AddString(XtraRichEditStringId.FileFilterDescription_MhtFiles, "Web Archive, single file");
			AddString(XtraRichEditStringId.FileFilterDescription_TextFiles, "Text Files");
			AddString(XtraRichEditStringId.FileFilterDescription_OpenXmlFiles, "Word 2007 Document");
			AddString(XtraRichEditStringId.FileFilterDescription_OpenDocumentFiles, "OpenDocument Text Document");
			AddString(XtraRichEditStringId.FileFilterDescription_WordMLFiles, "Word XML Document");
			AddString(XtraRichEditStringId.FileFilterDescription_XamlFiles, "Xaml Document");
			AddString(XtraRichEditStringId.FileFilterDescription_ePubFiles, "Electronic Publication");
			AddString(XtraRichEditStringId.FileFilterDescription_PDFFiles, "Portable Document Format");
			AddString(XtraRichEditStringId.FileFilterDescription_BitmapFiles, "Bitmap");
			AddString(XtraRichEditStringId.FileFilterDescription_JPEGFiles, "JPEG File Interchange Format");
			AddString(XtraRichEditStringId.FileFilterDescription_PNGFiles, "Portable Network Graphics");
			AddString(XtraRichEditStringId.FileFilterDescription_GifFiles, "Graphics Interchage Graphics");
			AddString(XtraRichEditStringId.FileFilterDescription_TiffFiles, "Tagged Image Format");
			AddString(XtraRichEditStringId.FileFilterDescription_EmfFiles, "Microsoft Enhanced Metafile");
			AddString(XtraRichEditStringId.FileFilterDescription_WmfFiles, "Windows Metafile");
			AddString(XtraRichEditStringId.FileFilterDescription_DocFiles, "Microsoft Word Document");
			AddString(XtraRichEditStringId.DefaultStyleName_ArticleSection, KnownStyleNames.ArticleSection);
			AddString(XtraRichEditStringId.DefaultStyleName_CommentReference, KnownStyleNames.CommentReference);
			AddString(XtraRichEditStringId.DefaultStyleName_CommentSubject, KnownStyleNames.CommentSubject);
			AddString(XtraRichEditStringId.DefaultStyleName_CommentText, KnownStyleNames.CommentText);
			AddString(XtraRichEditStringId.DefaultStyleName_Normal, ParagraphStyleCollection.DefaultParagraphStyleName);
			AddString(XtraRichEditStringId.DefaultStyleName_heading1, KnownStyleNames.Heading1);
			AddString(XtraRichEditStringId.DefaultStyleName_heading2, KnownStyleNames.Heading2);
			AddString(XtraRichEditStringId.DefaultStyleName_heading3, KnownStyleNames.Heading3);
			AddString(XtraRichEditStringId.DefaultStyleName_heading4, KnownStyleNames.Heading4);
			AddString(XtraRichEditStringId.DefaultStyleName_heading5, KnownStyleNames.Heading5);
			AddString(XtraRichEditStringId.DefaultStyleName_heading6, KnownStyleNames.Heading6);
			AddString(XtraRichEditStringId.DefaultStyleName_heading7, KnownStyleNames.Heading7);
			AddString(XtraRichEditStringId.DefaultStyleName_heading8, KnownStyleNames.Heading8);
			AddString(XtraRichEditStringId.DefaultStyleName_heading9, KnownStyleNames.Heading9);
			AddString(XtraRichEditStringId.DefaultStyleName_index1, KnownStyleNames.Index1);
			AddString(XtraRichEditStringId.DefaultStyleName_index2, KnownStyleNames.Index2);
			AddString(XtraRichEditStringId.DefaultStyleName_index3, KnownStyleNames.Index3);
			AddString(XtraRichEditStringId.DefaultStyleName_index4, KnownStyleNames.Index4);
			AddString(XtraRichEditStringId.DefaultStyleName_index5, KnownStyleNames.Index5);
			AddString(XtraRichEditStringId.DefaultStyleName_index6, KnownStyleNames.Index6);
			AddString(XtraRichEditStringId.DefaultStyleName_index7, KnownStyleNames.Index7);
			AddString(XtraRichEditStringId.DefaultStyleName_index8, KnownStyleNames.Index8);
			AddString(XtraRichEditStringId.DefaultStyleName_index9, KnownStyleNames.Index9);
			AddString(XtraRichEditStringId.DefaultStyleName_toc1, KnownStyleNames.TOC1);
			AddString(XtraRichEditStringId.DefaultStyleName_toc2, KnownStyleNames.TOC2);
			AddString(XtraRichEditStringId.DefaultStyleName_toc3, KnownStyleNames.TOC3);
			AddString(XtraRichEditStringId.DefaultStyleName_toc4, KnownStyleNames.TOC4);
			AddString(XtraRichEditStringId.DefaultStyleName_toc5, KnownStyleNames.TOC5);
			AddString(XtraRichEditStringId.DefaultStyleName_toc6, KnownStyleNames.TOC6);
			AddString(XtraRichEditStringId.DefaultStyleName_toc7, KnownStyleNames.TOC7);
			AddString(XtraRichEditStringId.DefaultStyleName_toc8, KnownStyleNames.TOC8);
			AddString(XtraRichEditStringId.DefaultStyleName_toc9, KnownStyleNames.TOC9);
			AddString(XtraRichEditStringId.DefaultStyleName_NormalIndent, KnownStyleNames.NormalIndent);
			AddString(XtraRichEditStringId.DefaultStyleName_footnotetext, KnownStyleNames.FootnoteText);
			AddString(XtraRichEditStringId.DefaultStyleName_annotationtext, KnownStyleNames.AnnotationText);
			AddString(XtraRichEditStringId.DefaultStyleName_header, KnownStyleNames.Header);
			AddString(XtraRichEditStringId.DefaultStyleName_footer, KnownStyleNames.Footer);
			AddString(XtraRichEditStringId.DefaultStyleName_indexheading, KnownStyleNames.IndexHeading);
			AddString(XtraRichEditStringId.DefaultStyleName_caption, KnownStyleNames.Caption);
			AddString(XtraRichEditStringId.DefaultStyleName_tableoffigures, KnownStyleNames.TableOfFigures);
			AddString(XtraRichEditStringId.DefaultStyleName_envelopeaddress, KnownStyleNames.EnvelopeAddress);
			AddString(XtraRichEditStringId.DefaultStyleName_envelopereturn, KnownStyleNames.EnvelopeReturn);
			AddString(XtraRichEditStringId.DefaultStyleName_footnotereference, KnownStyleNames.FootnoteReference);
			AddString(XtraRichEditStringId.DefaultStyleName_annotationreference, KnownStyleNames.AnnotationReference);
			AddString(XtraRichEditStringId.DefaultStyleName_linenumber, CharacterStyleCollection.LineNumberingStyleName);
			AddString(XtraRichEditStringId.DefaultStyleName_pagenumber, KnownStyleNames.PageNumber);
			AddString(XtraRichEditStringId.DefaultStyleName_endnotereference, KnownStyleNames.EndnoteReference);
			AddString(XtraRichEditStringId.DefaultStyleName_endnotetext, KnownStyleNames.EndnoteText);
			AddString(XtraRichEditStringId.DefaultStyleName_tableofauthorities, KnownStyleNames.TableOfAuthorities);
			AddString(XtraRichEditStringId.DefaultStyleName_macrotoaheading, KnownStyleNames.MacroToAHeading);
			AddString(XtraRichEditStringId.DefaultStyleName_List, KnownStyleNames.List);
			AddString(XtraRichEditStringId.DefaultStyleName_List2, KnownStyleNames.List2);
			AddString(XtraRichEditStringId.DefaultStyleName_List3, KnownStyleNames.List3);
			AddString(XtraRichEditStringId.DefaultStyleName_List4, KnownStyleNames.List4);
			AddString(XtraRichEditStringId.DefaultStyleName_List5, KnownStyleNames.List5);
			AddString(XtraRichEditStringId.DefaultStyleName_ListBullet, KnownStyleNames.ListBullet);
			AddString(XtraRichEditStringId.DefaultStyleName_ListBullet2, KnownStyleNames.ListBullet2);
			AddString(XtraRichEditStringId.DefaultStyleName_ListBullet3, KnownStyleNames.ListBullet3);
			AddString(XtraRichEditStringId.DefaultStyleName_ListBullet4, KnownStyleNames.ListBullet4);
			AddString(XtraRichEditStringId.DefaultStyleName_ListBullet5, KnownStyleNames.ListBullet5);
			AddString(XtraRichEditStringId.DefaultStyleName_ListNumber, KnownStyleNames.ListNumber);
			AddString(XtraRichEditStringId.DefaultStyleName_ListNumber2, KnownStyleNames.ListNumber2);
			AddString(XtraRichEditStringId.DefaultStyleName_ListNumber3, KnownStyleNames.ListNumber3);
			AddString(XtraRichEditStringId.DefaultStyleName_ListNumber4, KnownStyleNames.ListNumber4);
			AddString(XtraRichEditStringId.DefaultStyleName_ListNumber5, KnownStyleNames.ListNumber5);
			AddString(XtraRichEditStringId.DefaultStyleName_Title, KnownStyleNames.Title);
			AddString(XtraRichEditStringId.DefaultStyleName_Closing, KnownStyleNames.Closing);
			AddString(XtraRichEditStringId.DefaultStyleName_Signature, KnownStyleNames.Signature);
			AddString(XtraRichEditStringId.DefaultStyleName_DefaultParagraphFont, CharacterStyleCollection.DefaultCharacterStyleName);
			AddString(XtraRichEditStringId.DefaultStyleName_ListContinue, KnownStyleNames.ListContinue);
			AddString(XtraRichEditStringId.DefaultStyleName_ListContinue2, KnownStyleNames.ListContinue2);
			AddString(XtraRichEditStringId.DefaultStyleName_ListContinue3, KnownStyleNames.ListContinue3);
			AddString(XtraRichEditStringId.DefaultStyleName_ListContinue4, KnownStyleNames.ListContinue4);
			AddString(XtraRichEditStringId.DefaultStyleName_ListContinue5, KnownStyleNames.ListContinue5);
			AddString(XtraRichEditStringId.DefaultStyleName_MessageHeader, KnownStyleNames.MessageHeader);
			AddString(XtraRichEditStringId.DefaultStyleName_Salutation, KnownStyleNames.Salutation);
			AddString(XtraRichEditStringId.DefaultStyleName_Date, KnownStyleNames.Date);
			AddString(XtraRichEditStringId.DefaultStyleName_NoteHeading, KnownStyleNames.NoteHeading);
			AddString(XtraRichEditStringId.DefaultStyleName_MacroText, KnownStyleNames.MacroText);
			AddString(XtraRichEditStringId.DefaultStyleName_Strong, KnownStyleNames.Strong);
			AddString(XtraRichEditStringId.DefaultStyleName_Subtitle, KnownStyleNames.Subtitle);
			AddString(XtraRichEditStringId.DefaultStyleName_BodyText, KnownStyleNames.BodyText);
			AddString(XtraRichEditStringId.DefaultStyleName_BodyText2, KnownStyleNames.BodyText2);
			AddString(XtraRichEditStringId.DefaultStyleName_BodyText3, KnownStyleNames.BodyText3);
			AddString(XtraRichEditStringId.DefaultStyleName_BodyTextFirstIndent, KnownStyleNames.BodyTextFirstIndent);
			AddString(XtraRichEditStringId.DefaultStyleName_BodyTextFirstIndent2, KnownStyleNames.BodyTextFirstIndent2);
			AddString(XtraRichEditStringId.DefaultStyleName_BodyTextIndent, KnownStyleNames.BodyTextIndent);
			AddString(XtraRichEditStringId.DefaultStyleName_BodyTextIndent2, KnownStyleNames.BodyTextIndent2);
			AddString(XtraRichEditStringId.DefaultStyleName_BodyTextIndent3, KnownStyleNames.BodyTextIndent3);
			AddString(XtraRichEditStringId.DefaultStyleName_BlockText, KnownStyleNames.BlockText);
			AddString(XtraRichEditStringId.DefaultStyleName_HyperlinkFollowed, KnownStyleNames.HyperlinkFollowed);
			AddString(XtraRichEditStringId.DefaultStyleName_HyperlinkStrongEmphasis, KnownStyleNames.HyperlinkStrongEmphasis);
			AddString(XtraRichEditStringId.DefaultStyleName_Emphasis, KnownStyleNames.Emphasis);
			AddString(XtraRichEditStringId.DefaultStyleName_FollowedHyperlink, KnownStyleNames.FollowedHyperlink);
			AddString(XtraRichEditStringId.DefaultStyleName_DocumentMap, KnownStyleNames.DocumentMap);
			AddString(XtraRichEditStringId.DefaultStyleName_PlainText, KnownStyleNames.PlainText);
			AddString(XtraRichEditStringId.DefaultStyleName_EmailSignature, KnownStyleNames.EmailSignature);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLTopofForm, KnownStyleNames.HTMLTopOfForm);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLBottomofForm, KnownStyleNames.HTMLBottomOfForm);
			AddString(XtraRichEditStringId.DefaultStyleName_NormalWeb, KnownStyleNames.NormalWeb);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLAcronym, KnownStyleNames.HTMLAcronym);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLAddress, KnownStyleNames.HTMLAddress);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLCite, KnownStyleNames.HTMLCite);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLCode, KnownStyleNames.HTMLCode);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLDefinition, KnownStyleNames.HTMLDefinition);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLKeyboard, KnownStyleNames.HTMLKeyboard);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLPreformatted, KnownStyleNames.HTMLPreformatted);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLSample, KnownStyleNames.HTMLSample);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLTypewriter, KnownStyleNames.HTMLTypewriter);
			AddString(XtraRichEditStringId.DefaultStyleName_HTMLVariable, KnownStyleNames.HTMLVariable);
			AddString(XtraRichEditStringId.DefaultStyleName_NormalTable, KnownStyleNames.NormalTable);
			AddString(XtraRichEditStringId.DefaultStyleName_annotationsubject, KnownStyleNames.AnnotationSubject);
			AddString(XtraRichEditStringId.DefaultStyleName_NoList, KnownStyleNames.NoList);
			AddString(XtraRichEditStringId.DefaultStyleName_OutlineList1, KnownStyleNames.OutlineList1);
			AddString(XtraRichEditStringId.DefaultStyleName_OutlineList2, KnownStyleNames.OutlineList2);
			AddString(XtraRichEditStringId.DefaultStyleName_OutlineList3, KnownStyleNames.OutlineList3);
			AddString(XtraRichEditStringId.DefaultStyleName_TableSimple1, KnownStyleNames.TableSimple1);
			AddString(XtraRichEditStringId.DefaultStyleName_TableSimple2, KnownStyleNames.TableSimple2);
			AddString(XtraRichEditStringId.DefaultStyleName_TableSimple3, KnownStyleNames.TableSimple3);
			AddString(XtraRichEditStringId.DefaultStyleName_TableClassic1, KnownStyleNames.TableClassic1);
			AddString(XtraRichEditStringId.DefaultStyleName_TableClassic2, KnownStyleNames.TableClassic2);
			AddString(XtraRichEditStringId.DefaultStyleName_TableClassic3, KnownStyleNames.TableClassic3);
			AddString(XtraRichEditStringId.DefaultStyleName_TableClassic4, KnownStyleNames.TableClassic4);
			AddString(XtraRichEditStringId.DefaultStyleName_TableColorful1, KnownStyleNames.TableColorful1);
			AddString(XtraRichEditStringId.DefaultStyleName_TableColorful2, KnownStyleNames.TableColorful2);
			AddString(XtraRichEditStringId.DefaultStyleName_TableColorful3, KnownStyleNames.TableColorful3);
			AddString(XtraRichEditStringId.DefaultStyleName_TableColumns1, KnownStyleNames.TableColumns1);
			AddString(XtraRichEditStringId.DefaultStyleName_TableColumns2, KnownStyleNames.TableColumns2);
			AddString(XtraRichEditStringId.DefaultStyleName_TableColumns3, KnownStyleNames.TableColumns3);
			AddString(XtraRichEditStringId.DefaultStyleName_TableColumns4, KnownStyleNames.TableColumns4);
			AddString(XtraRichEditStringId.DefaultStyleName_TableColumns5, KnownStyleNames.TableColumns5);
			AddString(XtraRichEditStringId.DefaultStyleName_TableGrid, KnownStyleNames.TableGrid);
			AddString(XtraRichEditStringId.DefaultStyleName_TableGrid1, KnownStyleNames.TableGrid1);
			AddString(XtraRichEditStringId.DefaultStyleName_TableGrid2, KnownStyleNames.TableGrid2);
			AddString(XtraRichEditStringId.DefaultStyleName_TableGrid3, KnownStyleNames.TableGrid3);
			AddString(XtraRichEditStringId.DefaultStyleName_TableGrid4, KnownStyleNames.TableGrid4);
			AddString(XtraRichEditStringId.DefaultStyleName_TableGrid5, KnownStyleNames.TableGrid5);
			AddString(XtraRichEditStringId.DefaultStyleName_TableGrid6, KnownStyleNames.TableGrid6);
			AddString(XtraRichEditStringId.DefaultStyleName_TableGrid7, KnownStyleNames.TableGrid7);
			AddString(XtraRichEditStringId.DefaultStyleName_TableGrid8, KnownStyleNames.TableGrid8);
			AddString(XtraRichEditStringId.DefaultStyleName_TableNormal, KnownStyleNames.TableNormal);
			AddString(XtraRichEditStringId.DefaultStyleName_TableList1, KnownStyleNames.TableList1);
			AddString(XtraRichEditStringId.DefaultStyleName_TableList2, KnownStyleNames.TableList2);
			AddString(XtraRichEditStringId.DefaultStyleName_TableList3, KnownStyleNames.TableList3);
			AddString(XtraRichEditStringId.DefaultStyleName_TableList4, KnownStyleNames.TableList4);
			AddString(XtraRichEditStringId.DefaultStyleName_TableList5, KnownStyleNames.TableList5);
			AddString(XtraRichEditStringId.DefaultStyleName_TableList6, KnownStyleNames.TableList6);
			AddString(XtraRichEditStringId.DefaultStyleName_TableList7, KnownStyleNames.TableList7);
			AddString(XtraRichEditStringId.DefaultStyleName_TableList8, KnownStyleNames.TableList8);
			AddString(XtraRichEditStringId.DefaultStyleName_Table3Deffects1, KnownStyleNames.Table3DEffects1);
			AddString(XtraRichEditStringId.DefaultStyleName_Table3Deffects2, KnownStyleNames.Table3DEffects2);
			AddString(XtraRichEditStringId.DefaultStyleName_Table3Deffects3, KnownStyleNames.Table3DEffects3);
			AddString(XtraRichEditStringId.DefaultStyleName_TableContemporary, KnownStyleNames.TableContemporary);
			AddString(XtraRichEditStringId.DefaultStyleName_TableElegant, KnownStyleNames.TableElegant);
			AddString(XtraRichEditStringId.DefaultStyleName_TableProfessional, KnownStyleNames.TableProfessional);
			AddString(XtraRichEditStringId.DefaultStyleName_TableSubtle1, KnownStyleNames.TableSubtle1);
			AddString(XtraRichEditStringId.DefaultStyleName_TableSubtle2, KnownStyleNames.TableSubtle2);
			AddString(XtraRichEditStringId.DefaultStyleName_TableTheme, KnownStyleNames.TableTheme);
			AddString(XtraRichEditStringId.DefaultStyleName_TableWeb1, KnownStyleNames.TableWeb1);
			AddString(XtraRichEditStringId.DefaultStyleName_TableWeb2, KnownStyleNames.TableWeb2);
			AddString(XtraRichEditStringId.DefaultStyleName_TableWeb3, KnownStyleNames.TableWeb3);
			AddString(XtraRichEditStringId.DefaultStyleName_toaheading, KnownStyleNames.TOAHeading);
			AddString(XtraRichEditStringId.DefaultStyleName_BalloonText, KnownStyleNames.BalloonText);
			AddString(XtraRichEditStringId.LocalizedStyleName_Normal, ParagraphStyleCollection.DefaultParagraphStyleName);
			AddString(XtraRichEditStringId.LocalizedStyleName_Hyperlink, CharacterStyleCollection.HyperlinkStyleName);
			AddString(XtraRichEditStringId.LinkedCharacterStyleFormatString, "{0} Char");
			AddString(XtraRichEditStringId.ClearFormatting, "Clear Formatting");
			AddString(XtraRichEditStringId.FontStyle_Bold, "Bold");
			AddString(XtraRichEditStringId.FontStyle_Italic, "Italic");
			AddString(XtraRichEditStringId.FontStyle_BoldItalic, "Bold Italic");
			AddString(XtraRichEditStringId.FontStyle_Regular, "Regular");
			AddString(XtraRichEditStringId.FontStyle_Strikeout, "Strikeout");
			AddString(XtraRichEditStringId.FontStyle_Underline, "Underline");
			AddString(XtraRichEditStringId.Caption_AlignmentCenter, "Centered");
			AddString(XtraRichEditStringId.Caption_AlignmentJustify, "Justified");
			AddString(XtraRichEditStringId.Caption_AlignmentLeft, "Left");
			AddString(XtraRichEditStringId.Caption_AlignmentRight, "Right");
			AddString(XtraRichEditStringId.Caption_LineSpacingSingle, "Single");
			AddString(XtraRichEditStringId.Caption_LineSpacingSesquialteral, "1.5 lines");
			AddString(XtraRichEditStringId.Caption_LineSpacingDouble, "Double");
			AddString(XtraRichEditStringId.Caption_LineSpacingMultiple, "Multiple");
			AddString(XtraRichEditStringId.Caption_LineSpacingExactly, "Exactly");
			AddString(XtraRichEditStringId.Caption_LineSpacingAtLeast, "At least");
			AddString(XtraRichEditStringId.Caption_FirstLineIndentNone, "(none)");
			AddString(XtraRichEditStringId.Caption_FirstLineIndentIndented, "First line");
			AddString(XtraRichEditStringId.Caption_FirstLineIndentHanging, "Hanging");
			AddString(XtraRichEditStringId.Caption_OutlineLevelBody, "Body Text");
			AddString(XtraRichEditStringId.Caption_OutlineLevel1, "Level 1");
			AddString(XtraRichEditStringId.Caption_OutlineLevel2, "Level 2");
			AddString(XtraRichEditStringId.Caption_OutlineLevel3, "Level 3");
			AddString(XtraRichEditStringId.Caption_OutlineLevel4, "Level 4");
			AddString(XtraRichEditStringId.Caption_OutlineLevel5, "Level 5");
			AddString(XtraRichEditStringId.Caption_OutlineLevel6, "Level 6");
			AddString(XtraRichEditStringId.Caption_OutlineLevel7, "Level 7");
			AddString(XtraRichEditStringId.Caption_OutlineLevel8, "Level 8");
			AddString(XtraRichEditStringId.Caption_OutlineLevel9, "Level 9");
			AddString(XtraRichEditStringId.Caption_Heading, "Heading");
			AddString(XtraRichEditStringId.DialogCaption_InsertSymbol, "Symbol");
			AddString(XtraRichEditStringId.TabForm_All, "All");
			AddString(XtraRichEditStringId.UnderlineNone, "(none)");
			AddString(XtraRichEditStringId.BorderLineStyleNone, "None");
			AddString(XtraRichEditStringId.ColorAuto, "Auto");
			AddString(XtraRichEditStringId.Comment, "Comment [{0} {1}]");
			AddString(XtraRichEditStringId.Page, "Page {2}: [{3}] ");
			AddString(XtraRichEditStringId.FieldMapUniqueId, "Unique Identifier");
			AddString(XtraRichEditStringId.FieldMapTitle, "Courtesy Title");
			AddString(XtraRichEditStringId.FieldMapFirstName, "First Name");
			AddString(XtraRichEditStringId.FieldMapMiddleName, "Middle Name");
			AddString(XtraRichEditStringId.FieldLastName, "Last Name");
			AddString(XtraRichEditStringId.FieldMapSuffix, "Suffix");
			AddString(XtraRichEditStringId.FieldMapNickName, "Nickname");
			AddString(XtraRichEditStringId.FieldMapJobTitle, "Job Title");
			AddString(XtraRichEditStringId.FieldMapCompany, "Company");
			AddString(XtraRichEditStringId.FieldMapAddress1, "Address 1");
			AddString(XtraRichEditStringId.FieldMapAddress2, "Address 2");
			AddString(XtraRichEditStringId.FieldMapCity, "City");
			AddString(XtraRichEditStringId.FieldMapState, "State");
			AddString(XtraRichEditStringId.FieldMapPostalCode, "Postal Code");
			AddString(XtraRichEditStringId.FieldMapCountry, "Country or Region");
			AddString(XtraRichEditStringId.FieldMapBusinessPhone, "Business Phone");
			AddString(XtraRichEditStringId.FieldMapBusinessFax, "Business Fax");
			AddString(XtraRichEditStringId.FieldMapHomePhone, "Home Phone");
			AddString(XtraRichEditStringId.FieldMapHomeFax, "Home Fax");
			AddString(XtraRichEditStringId.FieldMapEMailAddress, "E-mail Address");
			AddString(XtraRichEditStringId.FieldMapWebPage, "Web Page");
			AddString(XtraRichEditStringId.FieldMapPartnerTitle, "Spouse/Partner Courtesy Title");
			AddString(XtraRichEditStringId.FieldMapPartnerFirstName, "Spouse/Partner First Name");
			AddString(XtraRichEditStringId.FieldMapPartnerMiddleName, "Spouse/Partner Middle Name");
			AddString(XtraRichEditStringId.FieldMapPartnerLastName, "Spouse/Partner Last Name");
			AddString(XtraRichEditStringId.FieldMapPartnerNickName, "Spouse/Partner Nickname");
			AddString(XtraRichEditStringId.FieldMapPhoneticGuideFirstName, "Phonetic Guide for First Name");
			AddString(XtraRichEditStringId.FieldMapPhoneticGuideLastName, "Phonetic Guide for Last Name");
			AddString(XtraRichEditStringId.FieldMapAddress3, "Address 3");
			AddString(XtraRichEditStringId.FieldMapDepartment, "Department");
			AddString(XtraRichEditStringId.Msg_PrintingUnavailable, "Printing is not available. Check if the following assemblies are installed:\r\n{0}");
			AddString(XtraRichEditStringId.TargetFrameDescription_Blank, "New window");
			AddString(XtraRichEditStringId.TargetFrameDescription_Parent, "Parent frame");
			AddString(XtraRichEditStringId.TargetFrameDescription_Self, "Same frame");
			AddString(XtraRichEditStringId.TargetFrameDescription_Top, "Whole page");
			AddString(XtraRichEditStringId.HyperlinkForm_SelectionInDocument, "Selection in document");
			AddString(XtraRichEditStringId.HyperlinkForm_InsertHyperlinkTitle, "Insert Hyperlink");
			AddString(XtraRichEditStringId.HyperlinkForm_EditHyperlinkTitle, "Edit Hyperlink");
			AddString(XtraRichEditStringId.HyperlinkForm_SelectedBookmarkNone, "<None>");
			AddString(XtraRichEditStringId.MenuCmd_ModifyHyperlink, "ModifyHyperlink");
			AddString(XtraRichEditStringId.MenuCmd_ModifyHyperlinkDescription, "ModifyHyperlinkDescription");
			AddString(XtraRichEditStringId.MenuCmd_MergeTableCells, "Merge Cells");
			AddString(XtraRichEditStringId.MenuCmd_MergeTableCellsDescription, "Merge the selected cells into one cell.");
			AddString(XtraRichEditStringId.MenuCmd_SplitTable, "Split Table");
			AddString(XtraRichEditStringId.MenuCmd_SplitTableDescription, "Split the table into two tables.\r\n\r\nThe selected row will become the first row of the new table.");
			AddString(XtraRichEditStringId.MenuCmd_SplitTableCells, "Split Cells");
			AddString(XtraRichEditStringId.MenuCmd_SplitTableCellsMenuItem, "Split Cells...");
			AddString(XtraRichEditStringId.MenuCmd_SplitTableCellsDescription, "Split the selected cells into multiple new cells.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableColumnToTheLeft, "Insert Left");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableColumnToTheLeftDescription, "Add a new column directly to the left of the selected column.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableColumnToTheRight, "Insert Right");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableColumnToTheRightDescription, "Add a new column directly to the right of the selected column.");
			AddString(XtraRichEditStringId.MenuCmd_ResetCharacterFormatting, "Reset Character Formatting");
			AddString(XtraRichEditStringId.MenuCmd_ResetCharacterFormattingDescription, "Makes the selection the default character format of the applied style.");
			AddString(XtraRichEditStringId.MenuCmd_SelectTableColumns, "Select Column");
			AddString(XtraRichEditStringId.MenuCmd_SelectTableColumnsDescription, "Select Column");
			AddString(XtraRichEditStringId.MenuCmd_SelectTableCell, "Select Cell");
			AddString(XtraRichEditStringId.MenuCmd_SelectTableCellDescription, "Select Cell");
			AddString(XtraRichEditStringId.MenuCmd_SelectTableRow, "Select Row");
			AddString(XtraRichEditStringId.MenuCmd_SelectTableRowDescription, "Select Row");
			AddString(XtraRichEditStringId.MenuCmd_SelectTable, "Select Table");
			AddString(XtraRichEditStringId.MenuCmd_SelectTableDescription, "Select Table");
			AddString(XtraRichEditStringId.MenuCmd_SelectTableElements, "Select");
			AddString(XtraRichEditStringId.MenuCmd_SelectTableElementsDescription, "Select the current cell, row, column, or entire table.");
			AddString(XtraRichEditStringId.MenuCmd_ProtectDocument, "Protect Document");
			AddString(XtraRichEditStringId.MenuCmd_ProtectDocumentDescription, "Help restrict people from editing the document by specifying a password.");
			AddString(XtraRichEditStringId.MenuCmd_UnprotectDocument, "Unprotect Document");
			AddString(XtraRichEditStringId.MenuCmd_UnprotectDocumentDescription, "Enable users to edit the document.");
			AddString(XtraRichEditStringId.MenuCmd_MakeTextUpperCase, "UPPERCASE");
			AddString(XtraRichEditStringId.MenuCmd_MakeTextUpperCaseDescription, "Change all the selected text to UPPERCASE.");
			AddString(XtraRichEditStringId.MenuCmd_MakeTextLowerCase, "lowercase");
			AddString(XtraRichEditStringId.MenuCmd_MakeTextLowerCaseDescription, "Change all the selected text to lowercase.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTextCase, "tOGGLE cASE");
			AddString(XtraRichEditStringId.MenuCmd_ToggleTextCaseDescription, "tOGGLE cASE.");
			AddString(XtraRichEditStringId.MenuCmd_CapitalizeEachWordTextCase, "Capitalize Each Word");
			AddString(XtraRichEditStringId.MenuCmd_CapitalizeEachWordTextCaseDescription, "Capitalize each word.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeTextCase, "Change Case");
			AddString(XtraRichEditStringId.MenuCmd_ChangeTextCaseDescription, "Change all the selected text to UPPERCASE, lowercase, or other common capitalizations.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeSectionLineNumbering, "Line Numbers");
			AddString(XtraRichEditStringId.MenuCmd_ChangeSectionLineNumberingDescription, "Add line numbers in the margin alongside of each line of the document.");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionLineNumberingNone, "None");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionLineNumberingNoneDescription, "No line numbers.");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionLineNumberingContinuous, "Continuous");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionLineNumberingContinuousDescription, "Continuous");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionLineNumberingRestartNewPage, "Restart Each Page");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionLineNumberingRestartNewPageDescription, "Restart Each Page");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionLineNumberingRestartNewSection, "Restart Each Section");
			AddString(XtraRichEditStringId.MenuCmd_SetSectionLineNumberingRestartNewSectionDescription, "Restart Each Section");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphSuppressLineNumbers, "Suppress for Current Paragraph");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphSuppressLineNumbersDescription, "Suppress for Current Paragraph");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphSuppressHyphenation, "Suppress Hyphenation for Current Paragraph");
			AddString(XtraRichEditStringId.MenuCmd_ParagraphSuppressHyphenationDescription, "Suppress Hyphenation for Current Paragraph");
			AddString(XtraRichEditStringId.MenuCmd_ShowLineNumberingForm, "&Line Numbering Options...");
			AddString(XtraRichEditStringId.MenuCmd_ShowLineNumberingFormDescription, "Line Numbering Options...");
			AddString(XtraRichEditStringId.MenuCmd_ShowPageSetupForm, "Page Setup");
			AddString(XtraRichEditStringId.MenuCmd_ShowPageSetupFormDescription, "Show the Page Setup dialog box.");
			AddString(XtraRichEditStringId.MenuCmd_ShowColumnsSetupForm, "More &Columns...");
			AddString(XtraRichEditStringId.MenuCmd_ShowColumnsSetupFormDescription, "Show the Column dialog box to customize column widths.");
			AddString(XtraRichEditStringId.MenuCmd_ShowTablePropertiesForm, "Properties");
			AddString(XtraRichEditStringId.MenuCmd_ShowTablePropertiesFormDescription, "Show the Table Properties dialog box to change advanced table properties, such as indentation and text wrapping options.");
			AddString(XtraRichEditStringId.MenuCmd_ShowTablePropertiesFormMenuItem, "Table Properties...");
			AddString(XtraRichEditStringId.MenuCmd_ShowTablePropertiesFormDescriptionMenuItem, "Show the Table Properties dialog box.");
			AddString(XtraRichEditStringId.MenuCmd_ShowTableOptionsForm, "Cell Margins");
			AddString(XtraRichEditStringId.MenuCmd_ShowTableOptionsFormDescription, "Customize cell margins and the spacing between cells.");
			AddString(XtraRichEditStringId.MenuCmd_IncrementParagraphOutlineLevel, "Increment outline level");
			AddString(XtraRichEditStringId.MenuCmd_IncrementParagraphOutlineLevelDescription, "Increment outline level");
			AddString(XtraRichEditStringId.MenuCmd_DecrementParagraphOutlineLevel, "Decrement outline level");
			AddString(XtraRichEditStringId.MenuCmd_DecrementParagraphOutlineLevelDescription, "Decrement outline level");
			AddString(XtraRichEditStringId.MenuCmd_SetParagraphBodyTextLevel, "Do Not Show in Table of Contents");
			AddString(XtraRichEditStringId.MenuCmd_SetParagraphBodyTextLevelDescription, "Do Not Show in Table of Contents");
			AddString(XtraRichEditStringId.MenuCmd_SetParagraphHeadingLevel, "Level {0}");
			AddString(XtraRichEditStringId.MenuCmd_SetParagraphHeadingLevelDescription, "Level {0}");
			AddString(XtraRichEditStringId.MenuCmd_AddParagraphsToTableOfContents, "Add Text");
			AddString(XtraRichEditStringId.MenuCmd_AddParagraphsToTableOfContentsDescription, "Add the current paragraph as an entry in the Table of Contents.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableOfContents, "Table of Contents");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableOfContentsDescription, "Add the Table of Contents to the document.\n\nOnce you have added a Table of Contents, click the Add Text button to add entries the the table.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableOfEquations, "Table of Equations");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableOfEquationsDescription, "Insert a Table of Equations into the document.\n\nA Table of Equations includes a list of all the equations in the document.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableOfFigures, "Table of Figures");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableOfFiguresDescription, "Insert a Table of Figures into the document.\n\nA Table of Figures includes a list of all the figures in the document.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableOfTables, "Table of Tables");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableOfTablesDescription, "Insert a Table of Tables into the document.\n\nA Table of Tables includes a list of all the tables in the document.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableOfFiguresPlaceholder, "Insert Table of Figures");
			AddString(XtraRichEditStringId.MenuCmd_InsertTableOfFiguresPlaceholderDescription, "Insert a Table of Figures into the document.\n\nA Table of Figures includes a list of all of the figures, tables or equations in the document.");
			AddString(XtraRichEditStringId.MenuCmd_InsertEquationsCaption, "Equations Caption");
			AddString(XtraRichEditStringId.MenuCmd_InsertEquationsCaptionDescription, "Add a equation caption.");
			AddString(XtraRichEditStringId.MenuCmd_InsertFiguresCaption, "Figures Caption");
			AddString(XtraRichEditStringId.MenuCmd_InsertFiguresCaptionDescription, "Add a figure caption.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTablesCaption, "Tables Caption");
			AddString(XtraRichEditStringId.MenuCmd_InsertTablesCaptionDescription, "Add a table caption.");
			AddString(XtraRichEditStringId.MenuCmd_InsertCaptionPlaceholder, "Insert Caption");
			AddString(XtraRichEditStringId.MenuCmd_InsertCaptionPlaceholderDescription, "Add a caption to a picture or other image.\n\nA caption is a line of text that appears below an object to describe it.");
			AddString(XtraRichEditStringId.MenuCmd_UpdateTableOfContents, "Update Table");
			AddString(XtraRichEditStringId.MenuCmd_UpdateTableOfContentsDescription, "Update the Table of Contents so that all the entries refer to the correct page number.");
			AddString(XtraRichEditStringId.MenuCmd_UpdateTableOfFigures, "Update Table");
			AddString(XtraRichEditStringId.MenuCmd_UpdateTableOfFiguresDescription, "Update the Table of Figures to include all of the entries in the document.");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectSquareTextWrapType, "Square");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectSquareTextWrapTypeDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectBehindTextWrapType, "Behind Text");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectBehindTextWrapTypeDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectInFrontOfTextWrapType, "In Front of Text");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectInFrontOfTextWrapTypeDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectThroughTextWrapType, "Through");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectThroughTextWrapTypeDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectTightTextWrapType, "Tight");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectTightTextWrapTypeDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectTopAndBottomTextWrapType, "Top and Bottom");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectTopAndBottomTextWrapTypeDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectBringForward, "Bring Forward");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectBringForwardDescription, "Bring the selected object forward so that it is hidden by fewer object that are in front of it.");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectBringToFront, "Bring to Front");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectBringToFrontDescription, "Bring the selected object in front of all other objects so that no part of it is hidden behind another object.");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectBringInFrontOfText, "Bring in Front of Text");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectBringInFrontOfTextDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectSendBackward, "Send Backward");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectSendBackwardDescription, "Send the selected object backward so that it is hidden by the object that are in front of it.");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectSendToBack, "Send to Back");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectSendToBackDescription, "Send the selected object behind all other objects.");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectSendBehindText, "Send Behind Text");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectSendBehindTextDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectTopLeftAlignment, "Top Left");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectTopLeftAlignmentDescription, "Position in Top Left with Square Text Wrapping");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectTopCenterAlignment, "Top Center");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectTopCenterAlignmentDescription, "Position in Top Center with Square Text Wrapping");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectTopRightAlignment, "Top Right");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectTopRightAlignmentDescription, "Position in Top Right with Square Text Wrapping");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleLeftAlignment, "Middle Left");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleLeftAlignmentDescription, "Position in Middle Left with Square Text Wrapping");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleCenterAlignment, "Middle Center");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleCenterAlignmentDescription, "Position in Middle Center with Square Text Wrapping");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleRightAlignment, "Middle Right");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleRightAlignmentDescription, "Position in Middle Right with Square Text Wrapping");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomLeftAlignment, "Bottom Left");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomLeftAlignmentDescription, "Position in Bottom Left with Square Text Wrapping");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomCenterAlignment, "Bottom Center");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomCenterAlignmentDescription, "Position in Bottom Center with Square Text Wrapping");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomRightAlignment, "Bottom Right");
			AddString(XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomRightAlignmentDescription, "Position in Bottom Right with Square Text Wrapping");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFloatingObjectTextWrapType, "Wrap Text");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFloatingObjectTextWrapTypeDescription, "Change the way text wraps around the selected object. To configure the object so that it moves along with text the text around it, select \"In Line With Text\".");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFloatingObjectAlignment, "Position");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFloatingObjectAlignmentDescription, "Position the selected object on the page. Text is automatically set to wrap around the object.");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectBringForwardPlaceholder, "Bring to Front");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectBringForwardPlaceholderDescription, "Bring the selected object forward so that it is hidden by fewer object that are in front of it.");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectSendBackwardPlaceholder, "Send to Back");
			AddString(XtraRichEditStringId.MenuCmd_FloatingObjectSendBackwardPlaceholderDescription, "Send the selected object backward so that it is hidden by the object that are in front of it.");
			AddString(XtraRichEditStringId.MenuCmd_ShowPageMarginsSetupForm, "Custom M&argins...");
			AddString(XtraRichEditStringId.MenuCmd_ShowPageMarginsSetupFormDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_ShowPagePaperSetupForm, "More P&aper Sizes...");
			AddString(XtraRichEditStringId.MenuCmd_ShowPagePaperSetupFormDescription, " ");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFloatingObjectFillColor, "Shape Fill");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFloatingObjectFillColorDescription, "Fill the selected shape with a solid color.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFloatingObjectOutlineColor, "Shape Outline");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFloatingObjectOutlineColorDescription, "Specify color for the outline of the selected shape.");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFloatingObjectOutlineWidth, "Shape Outline Weight");
			AddString(XtraRichEditStringId.MenuCmd_ChangeFloatingObjectOutlineWidthDescription, "Specify width for the outline of the selected shape.");
			AddString(XtraRichEditStringId.MenuCmd_InsertTextBox, "Text Box");
			AddString(XtraRichEditStringId.MenuCmd_InsertTextBoxDescription, "Insert a text box into the document.");
			AddString(XtraRichEditStringId.MenuCmd_InsertFloatingObjectPicture, "Picture");
			AddString(XtraRichEditStringId.MenuCmd_InsertFloatingObjectPictureDescription, "Insert a picture from a file.");
			AddString(XtraRichEditStringId.MenuCmd_ChangePageColor, "Page Color");
			AddString(XtraRichEditStringId.MenuCmd_ChangePageColorDescription, "Choose a color for the background of the page.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFirstRow, "Header Row");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFirstRowDescription, "Display special formatting for the first row of the table.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleLastRow, "Total Row");
			AddString(XtraRichEditStringId.MenuCmd_ToggleLastRowDescription, "Display special formatting for the last row of the table.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFirstColumn, "First Column");
			AddString(XtraRichEditStringId.MenuCmd_ToggleFirstColumnDescription, "Display special formatting for the first column of the table.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleLastColumn, "Last Column");
			AddString(XtraRichEditStringId.MenuCmd_ToggleLastColumnDescription, "Display special formatting for the last column of the table.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleBandedRows, "Banded Rows");
			AddString(XtraRichEditStringId.MenuCmd_ToggleBandedRowsDescription, "Display banded rows, in which even rows are formatted differently from odd rows.\r\n\r\nThis banding can make tables easier to read.");
			AddString(XtraRichEditStringId.MenuCmd_ToggleBandedColumn, "Banded Columns");
			AddString(XtraRichEditStringId.MenuCmd_ToggleBandedColumnDescription, "Display banded columns, in which even columns are formatted differently from odd columns.\r\n\r\nThis banding can make tables easier to read.");
			AddString(XtraRichEditStringId.MenuCmd_NewTableStyle, "New Table Style...");
			AddString(XtraRichEditStringId.MenuCmd_ModifyTableStyle, "Modify Table Style...");
			AddString(XtraRichEditStringId.MenuCmd_DeleteTableStyle, "Delete Table Style...");
			AddString(XtraRichEditStringId.KeyName_Control, "Ctrl");
			AddString(XtraRichEditStringId.KeyName_Shift, "Shift");
			AddString(XtraRichEditStringId.KeyName_Alt, "Alt");
			AddString(XtraRichEditStringId.Caption_NumberingListBoxNone, "None");
			AddString(XtraRichEditStringId.Caption_PageHeader, "Header");
			AddString(XtraRichEditStringId.Caption_FirstPageHeader, "First Page Header");
			AddString(XtraRichEditStringId.Caption_OddPageHeader, "Odd Page Header");
			AddString(XtraRichEditStringId.Caption_EvenPageHeader, "Even Page Header");
			AddString(XtraRichEditStringId.Caption_PageFooter, "Footer");
			AddString(XtraRichEditStringId.Caption_FirstPageFooter, "First Page Footer");
			AddString(XtraRichEditStringId.Caption_OddPageFooter, "Odd Page Footer");
			AddString(XtraRichEditStringId.Caption_EvenPageFooter, "Even Page Footer");
			AddString(XtraRichEditStringId.Caption_SameAsPrevious, "Same as Previous");
			AddString(XtraRichEditStringId.Msg_Loading, "Loading...");
			AddString(XtraRichEditStringId.Msg_Saving, "Saving...");
			AddString(XtraRichEditStringId.FindAndReplaceForm_AnySingleCharacter, "Any single character");
			AddString(XtraRichEditStringId.FindAndReplaceForm_ZeroOrMore, "Zero or more");
			AddString(XtraRichEditStringId.FindAndReplaceForm_OneOrMore, "One or more");
			AddString(XtraRichEditStringId.FindAndReplaceForm_BeginningOfLine, "Beginning of paragraph");
			AddString(XtraRichEditStringId.FindAndReplaceForm_EndOfLine, "End of paragraph");
			AddString(XtraRichEditStringId.FindAndReplaceForm_BeginningOfWord, "Beginning of word");
			AddString(XtraRichEditStringId.FindAndReplaceForm_EndOfWord, "End of word");
			AddString(XtraRichEditStringId.FindAndReplaceForm_AnyOneCharacterInTheSet, "Any one character in the set");
			AddString(XtraRichEditStringId.FindAndReplaceForm_AnyOneCharacterNotInTheSet, "Any one character not in the set");
			AddString(XtraRichEditStringId.FindAndReplaceForm_Or, "Or");
			AddString(XtraRichEditStringId.FindAndReplaceForm_EscapeSpecialCharacter, "Escape special character");
			AddString(XtraRichEditStringId.FindAndReplaceForm_TagExpression, "Tag expression");
			AddString(XtraRichEditStringId.FindAndReplaceForm_WordCharacter, "Word character");
			AddString(XtraRichEditStringId.FindAndReplaceForm_SpaceOrTab, "Space or tab");
			AddString(XtraRichEditStringId.FindAndReplaceForm_Integer, "Integer");
			AddString(XtraRichEditStringId.FindAndReplaceForm_TaggedExpression, "Tagged expression {0}");
			AddString(XtraRichEditStringId.MenuCmd_IncrementParagraphLeftIndent, "IncrementParagraphLeftIndent");
			AddString(XtraRichEditStringId.MenuCmd_IncrementParagraphLeftIndentDescription, "IncrementParagraphLeftIndentDescription");
			AddString(XtraRichEditStringId.Caption_SectionPropertiesApplyToWholeDocument, "Whole document");
			AddString(XtraRichEditStringId.Caption_SectionPropertiesApplyToCurrentSection, "Current section");
			AddString(XtraRichEditStringId.Caption_SectionPropertiesApplyToSelectedSections, "Selected sections");
			AddString(XtraRichEditStringId.Caption_SectionPropertiesApplyThisPointForward, "This point forward");
			AddString(XtraRichEditStringId.Caption_PageSetupSectionStartContinuous, "Continuous");
			AddString(XtraRichEditStringId.Caption_PageSetupSectionStartColumn, "New column");
			AddString(XtraRichEditStringId.Caption_PageSetupSectionStartNextPage, "New page");
			AddString(XtraRichEditStringId.Caption_PageSetupSectionStartOddPage, "Odd page");
			AddString(XtraRichEditStringId.Caption_PageSetupSectionStartEvenPage, "Even page");
			AddString(XtraRichEditStringId.Caption_HeightTypeExact, "Exactly");
			AddString(XtraRichEditStringId.Caption_HeightTypeMinimum, "At least");
			AddString(XtraRichEditStringId.Caption_CaptionPrefixEquation, "Equation");
			AddString(XtraRichEditStringId.Caption_CaptionPrefixFigure, "Figure");
			AddString(XtraRichEditStringId.Caption_CaptionPrefixTable, "Table");
			AddString(XtraRichEditStringId.Caption_CurrentDocumentHyperlinkTooltip, "Current Document");
			AddString(XtraRichEditStringId.Caption_PreviousParagraphText, "Previous Paragraph ");
			AddString(XtraRichEditStringId.Caption_CurrentParagraphText, "Sample Text ");
			AddString(XtraRichEditStringId.Caption_FollowingParagraphText, "Following Paragraph ");
			AddString(XtraRichEditStringId.Caption_EmptyParentStyle, "(underlying properties)");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeMargin, "Margin");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeCharacter, "Character");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeColumn, "Column");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeInsideMargin, "Inside Margin");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeLeftMargin, "Left Margin");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeOutsideMargin, "Outside Margin");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypePage, "Page");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeRightMargin, "Right Margin");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionAlignmentCenter, "Centered");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionAlignmentLeft, "Left");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionAlignmentRight, "Right");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeMargin, "Margin");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypePage, "Page");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeLine, "Line");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeTopMargin, "Top Margin");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeBottomMargin, "Bottom Margin");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeInsideMargin, "Inside Margin");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeOutsideMargin, "Outside Margin");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeParagraph, "Paragraph");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentTop, "Top");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentCenter, "Center");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentBottom, "Bottom");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentInside, "Inside");
			AddString(XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentOutside, "Outside");
			AddString(XtraRichEditStringId.Caption_ParagraphAlignment_Left, "Left");
			AddString(XtraRichEditStringId.Caption_ParagraphAlignment_Right, "Right");
			AddString(XtraRichEditStringId.Caption_ParagraphAlignment_Center, "Center");
			AddString(XtraRichEditStringId.Caption_ParagraphAlignment_Justify, "Justify");
			AddString(XtraRichEditStringId.Caption_ParagraphFirstLineIndent_None, "None");
			AddString(XtraRichEditStringId.Caption_ParagraphFirstLineIndent_Indented, "Indented");
			AddString(XtraRichEditStringId.Caption_ParagraphFirstLineIndent_Hanging, "Hanging");
			AddString(XtraRichEditStringId.Caption_ParagraphLineSpacing_Single, "Single");
			AddString(XtraRichEditStringId.Caption_ParagraphLineSpacing_Sesquialteral, "Sesquialteral");
			AddString(XtraRichEditStringId.Caption_ParagraphLineSpacing_Double, "Double");
			AddString(XtraRichEditStringId.Caption_ParagraphLineSpacing_Multiple, "Multiple");
			AddString(XtraRichEditStringId.Caption_ParagraphLineSpacing_Exactly, "Exactly");
			AddString(XtraRichEditStringId.Caption_ParagraphLineSpacing_AtLeast, "At Least");
			AddString(XtraRichEditStringId.Caption_ColorAutomatic, "Automatic");
			AddString(XtraRichEditStringId.Caption_NoColor, "No Color");
			AddString(XtraRichEditStringId.Caption_PageCategoryTableTools, "Table Tools");
			AddString(XtraRichEditStringId.Caption_PageTableDesign, "Design");
			AddString(XtraRichEditStringId.Caption_PageTableLayout, "Layout");
			AddString(XtraRichEditStringId.Caption_GroupTableStyleOptions, "Table Style Options");
			AddString(XtraRichEditStringId.Caption_GroupTableDrawBorders, "Borders && Shadings");
			AddString(XtraRichEditStringId.Caption_GroupTableRowsAndColumns, "Rows && Columns");
			AddString(XtraRichEditStringId.Caption_GroupTableStyles, "Table Styles");
			AddString(XtraRichEditStringId.Caption_GroupTableTable, "Table");
			AddString(XtraRichEditStringId.Caption_GroupTableMerge, "Merge");
			AddString(XtraRichEditStringId.Caption_GroupTableCellSize, "Cell Size");
			AddString(XtraRichEditStringId.Caption_GroupTableAlignment, "Alignment");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_WholeTable, "Whole table");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_FirstRow, "Header row");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_LastRow, "Total row");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_FirstColumn, "First column");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_LastColumn, "Last column");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_OddRowBanding, "Odd banded rows");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_EvenRowBanding, "Even banded rows");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_OddColumnBanding, "Odd banded columns");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_EvenColumnBanding, "Even banded columns");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_TopLeftCell, "Top left cell");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_TopRightCell, "Top right cell");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_BottomLeftCell, "Bottom left cell");
			AddString(XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_BottomRightCell, "Bottom right cell");
			AddString(XtraRichEditStringId.Caption_ClipboardSubItem, "Clipboard");
			AddString(XtraRichEditStringId.SelectionCollection_EmptyCollectionException, "Cannot add an empty collection.");
			AddString(XtraRichEditStringId.SelectionCollection_SpecifiedSelectionsIntersectException, "Specified selections intersect.");
			AddString(XtraRichEditStringId.SelectionCollection_CurrentSelectionAndSpecifiedSelectionIntersectException, "Current selection and the specified selection intersect.");
			AddString(XtraRichEditStringId.SelectionCollection_SelectionShouldContainAtLeastOneCharacterException, "The selection should contain at least one character.");
			AddString(XtraRichEditStringId.SelectionCollection_SelectionExtendsOutsideTableException, "The selection extends outside the table, so the entire row must be selected.");
			AddString(XtraRichEditStringId.SelectionCollection_FirstCellContinuesVerticalMergeException, "The first cell in the selected range continues the vertical merge, which is not allowed in a selection collection.");
			AddString(XtraRichEditStringId.SelectionCollection_LastCellContinuesVerticalMergeException, "The last cell in the selected range continues the vertical merge, which is not allowed in a selection collection.");
			AddString(XtraRichEditStringId.SelectionCollection_SelecitonShouldIncludeNotMoreThanOneRowException, "A selection range should include not more than one row");
			AddString(XtraRichEditStringId.SelectionCollection_PartiallySelectedCellsException, "Partially selected cells are not allowed.");
			AddString(XtraRichEditStringId.SelectionCollection_SelectionCollectionEmptyException, "The selection collection is empty.");
			AddString(XtraRichEditStringId.SelectionCollection_AtLeastOneSelectionIsRequiredException, "Cannot remove a selection. At least one selection is required.");
			AddString(XtraRichEditStringId.SelectionCollection_CannotRemoveCaretException, "Cannot remove caret.");
			AddString(XtraRichEditStringId.SelectionCollection_RangeCannotBeEmptyException, "Range cannot be empty.");
			AddString(XtraRichEditStringId.SelectionCollection_OutOfRangeException, "Index was out of range. Must be non-negative and less than the size of the selection collection.");
			AddString(XtraRichEditStringId.Caption_MainDocumentComments, "Main document comments");
			AddString(XtraRichEditStringId.CommentToolTipHeader, "by {0}");
			AddString(XtraRichEditStringId.CommentToolTipHeaderWithDate, "{0} from {1}");
		}
		#endregion
		public static XtraLocalizer<XtraRichEditStringId> CreateDefaultLocalizer() {
			return new XtraRichEditResLocalizer();
		}
		public static string GetString(XtraRichEditStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<XtraRichEditStringId> CreateResXLocalizer() {
			return new XtraRichEditResLocalizer();
		}
	}
	#endregion
	#region XtraRichEditResLocalizer
	public class XtraRichEditResLocalizer : XtraResXLocalizer<XtraRichEditStringId> {
		public XtraRichEditResLocalizer()
			: base(new XtraRichEditLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
#if DXPORTABLE
			return new ResourceManager("DevExpress.RichEdit.Core.LocalizationRes", typeof(XtraRichEditResLocalizer).GetAssembly());
#else
			return new ResourceManager("DevExpress.XtraRichEdit.LocalizationRes", typeof(XtraRichEditResLocalizer).Assembly);
#endif
		}
	}
#endregion
}
namespace DevExpress.XtraRichEdit.Localization.Internal {
	public abstract class RichEditLocalizerBase<T> : XtraLocalizer<T> where T : struct {
		protected override void AddString(T id, string str) {
			Dictionary<T, string> table = XtraLocalizierHelper<T>.GetStringTable(this);
			table[id] = str;
		}
	}
}
