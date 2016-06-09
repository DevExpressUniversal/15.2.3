﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.Utils.Internal;
using DevExpress.Web;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public enum RichEditClientCommand {
		None = 0,
		FileNew = 1,
		FileOpen = 2,
		FileSave = 3,
		FileSaveAs = 4,
		FilePrint = 5,
		Undo = 6,
		Redo = 7,
		PasteSelection = 8,
		CopySelection = 9,
		CutSelection = 10,
		ChangeFontName = 11,
		ChangeStyle = 12,
		ChangeFontSize = 13,
		IncreaseFontSize = 14,
		DecreaseFontSize = 15,
		MakeTextUpperCase = 16,
		MakeTextLowerCase = 17,
		CapitalizeEachWordTextCase = 18,
		ToggleTextCase = 19,
		ToggleFontBold = 20,
		ToggleFontItalic = 21,
		ToggleFontUnderline = 22,
		ToggleFontDoubleUnderline = 23,
		ToggleFontStrikeout = 24,
		ToggleFontSuperscript = 26,
		ToggleFontSubscript = 27,
		ChangeFontForeColor = 28,
		ChangeFontBackColor = 29,
		ClearFormatting = 30,
		ToggleBulletedListItem = 31,
		ToggleNumberingListItem = 32,
		ToggleMultilevelListItem = 33,
		DecreaseIndent = 34,
		IncreaseIndent = 35,
		ToggleShowWhitespace = 36,
		ToggleParagraphAlignmentLeft = 37,
		ToggleParagraphAlignmentCenter = 38,
		ToggleParagraphAlignmentRight = 39,
		ToggleParagraphAlignmentJustify = 40,
		SetSingleParagraphSpacing = 41,
		SetSesquialteralParagraphSpacing = 42,
		SetDoubleParagraphSpacing = 43,
		AddSpacingBeforeParagraph = 45,
		AddSpacingAfterParagraph = 46,
		RemoveSpacingBeforeParagraph = 47,
		RemoveSpacingAfterParagraph = 48,
		ChangeParagraphBackColor = 49,
		ShowFontForm = 52,
		ShowParagraphForm = 53,
		InsertPageBreak = 54,
		ShowInsertTableForm = 55,
		InsertPicture = 56,
		ShowBookmarkForm = 58,
		ShowHyperlinkForm = 59,
		ShowSymbolForm = 65,
		SetNormalSectionPageMargins = 66,
		SetNarrowSectionPageMargins = 67,
		SetModerateSectionPageMargins = 68,
		SetWideSectionPageMargins = 69,
		ShowPageMarginsSetupForm = 70,
		SetPortraitPageOrientation = 71,
		SetLandscapePageOrientation = 72,
		ShowPagePaperSetupForm = 73,
		SetSectionOneColumn = 74,
		SetSectionTwoColumns = 75,
		SetSectionThreeColumns = 76,
		ShowColumnsSetupForm = 77,
		InsertColumnBreak = 79,
		InsertSectionBreakNextPage = 80,
		InsertSectionBreakEvenPage = 81,
		InsertSectionBreakOddPage = 82,
		ChangePageColor = 90,
		ToggleShowHorizontalRuler = 94,
		FullScreen = 98,
		SetSectionLegalPaperKind = 99,
		SetSectionFolioPaperKind = 100,
		SetSectionA4PaperKind = 101,
		SetSectionA5PaperKind = 102,
		SetSectionA6PaperKind = 103,
		SetSectionB5PaperKind = 104,
		SetSectionExecutivePaperKind = 105,
		SelectAll = 106,
		ShowPageSetupForm = 107,
		ShowNumberingListForm = 108,
		ExtendLineDown = 109,
		ExtendLineEnd = 110,
		ExtendLineStart = 111,
		ExtendLineUp = 112,
		ExtendNextCharacter = 113,
		ExtendPreviousCharacter = 114,
		ExtendSelectLine = 115,
		InsertParagraph = 116,
		InsertText = 117,
		LineDown = 118,
		LineEnd = 119,
		LineStart = 120,
		LineUp = 121,
		NextCharacter = 122,
		PreviousCharacter = 123,
		SelectLine = 124,
		ToggleBackspaceKey = 125,
		ToggleDeleteKey = 126,
		InsertLineBreak = 127,
		NextPage = 128,
		ExtendNextPage = 129,
		PreviousPage = 130,
		ExtendPreviousPage = 131,
		ChangeInlinePictureScale = 132,
		IncrementParagraphLeftIndent = 133,
		DecrementParagraphLeftIndent = 134,
		DragMoveContent = 135,
		DragCopyContent = 136,
		InsertSpace = 137,
		RulerSectionMarginLeft = 138,
		RulerSectionMarginRight = 139,
		RulerParagraphRightIndent = 140,
		RulerSectionColumnsSettings = 141,
		RulerParagraphLeftIndents = 142,
		InsertTabMark = 143,
		InsertShiftTabMark = 144,
		DocumentStart = 145,
		ExtendDocumentStart = 146,
		DocumentEnd = 147,
		ExtendDocumentEnd = 148,
		GoToNextWord = 149,
		ExtendGoToNextWord = 150,
		GoToPrevWord = 151,
		ExtendGoToPrevWord = 152,
		GoToStartParagraph = 153,
		ExtendGoToStartParagraph = 154,
		GoToEndParagraph = 155,
		ExtendGoToEndParagraph = 156,
		ReloadDocument = 157,
		ShowErrorModelIsChangedMessageCommand = 158,
		ShowErrorSessionHasExpiredMessageCommand = 159,
		SelectParagraph = 160,
		ShowErrorOpeningAndOverstoreImpossibleMessageCommand = 161,
		SetSectionLetterPaperKind = 162,
		ShowErrorClipboardAccessDeniedMessageCommand = 163,
		SelectLineNoUpdateControlState = 164,
		ExtendSelectLineNoUpdateControlState = 165,
		ShowTabsForm = 166,
		ShowCustomNumberingListForm = 167,
		ShowServiceFontForm = 168,
		ShowServiceSymbolsForm = 169,
		RestartNumberingList = 170,
		DeleteTabRuler = 171,
		InsertTabRuler = 172,
		MoveTabRuler = 173,
		IncrementNumberingIndent = 174,
		DecrementNumberingIndent = 175,
		IncrementParagraphIndentFromFirstRow = 176,
		DecrementParagraphIndentFromFirstRow = 177,
		CreateField = 178,  
		UpdateField = 179,
		ToggleFieldCodes = 180,
		ShowAllFieldCodes = 186,
		ShowAllFieldResults = 187,
		ToggleAllFields = 188, 
		ContinueNumberingList = 189,
		InsertNumerationToParagraphs = 190,
		DeleteNumerationFromParagraphs = 191,
		ShowErrorInnerExceptionMessageCommand = 192,
		ShowErrorAuthExceptionMessageCommand = 193,
		ShowEditHyperlinkForm = 194,
		OpenHyperlink = 195,
		RemoveHyperlink = 196,
		ShowErrorSavingMessageCommand = 197,
		ShowErrorOpeningMessageCommand = 198,
		ShowErrorDocVariableErrorCommand = 199,
		UpdateAllFields = 200,
		InsertNonBreakingSpace = 201,
		RemoveHyperlinks = 202,
		CreateDateField = 203,
		CreateTimeField = 204,
		CreatePageField = 205,
		ShowCreateHyperlinkForm = 206,
		SentenceCase = 207,
		SwitchTextCase = 208,
		GoToFirstDataRecord = 209,
		GoToPreviousDataRecord = 210,
		GoToNextDataRecord = 211,
		GoToLastDataRecord = 212,
		ToggleViewMergedData = 213,
		ShowInsertMergeFieldForm = 214,
		CreateMergeField = 215,
		ShowFinishAndMergeForm = 216,
		ChangeActiveSubDocument = 217,
		ShowSaveMergedDocumentForm = 218,
		AddSelectedLineCommandNoUpdateControlState = 219,
		InsertHeader = 220,
		InsertFooter = 221,
		LinkHeaderFooterToPrevious = 222,
		LinkHeader = 223,
		LinkFooter = 224,
		CreateBookmark = 225,
		DeleteBookmarks = 226,
		GoToPageHeader = 227,
		GoToPageFooter = 228,
		GoToNextPageHeaderFooter = 229,
		GoToPreviousPageHeaderFooter = 230,
		ToggleDifferentFirstPage = 231,
		ToggleDifferentOddAndEvenPages = 232,
		ClosePageHeaderFooter = 233,
		ContextItem_HeadersFooters = 234,
		InsertPageNumberField = 235,
		InsertPageCountField = 236,
		GoToBookmark = 237,
		InsertTableCore = 238,
		ContextItem_Tables = 239,
		ShowTablePropertiesForm = 240,
		ShowCellOptionsForm = 242,
		InsertTableColumnToTheLeft = 243,
		InsertTableColumnToTheRight = 244,
		InsertTableRowBelow = 245,
		InsertTableRowAbove = 246,
		DeleteTableRows = 247,
		DeleteTableColumns = 248,
		InsertTableCellWithShiftToTheLeft = 249,
		DeleteTableCellsWithShiftToTheHorizontally = 250,
		DeleteTable = 251,
		ShowInsertTableCellsForm = 252,
		ShowDeleteTableCellsForm = 253,
		MergeTableCells = 254,
		ShowSplitTableCellsForm = 255,
		SplitTableCellsCommand = 256,
		InsertTableCellsWithShiftToTheVertically = 257,
		DeleteTableCellsWithShiftToTheVertically = 258,
		ShowBorderShadingForm = 259,
		TableCellAlignTopLeft = 260,
		TableCellAlignTopCenter = 261,
		TableCellAlignTopRight = 262,
		TableCellAlignMiddleLeft = 263,
		TableCellAlignMiddleCenter = 264,
		TableCellAlignMiddleRight = 265,
		TableCellAlignBottomLeft = 266,
		TableCellAlignBottomCenter = 267,
		TableCellAlignBottomRight = 268,
		ApplyTableStyle = 269,
		ToggleTableCellsTopBorder = 270,
		ToggleTableCellsRightBorder = 271,
		ToggleTableCellsBottomBorder = 272,
		ToggleTableCellsLeftBorder = 273,
		ToggleTableCellNoBorder = 274,
		ToggleTableCellAllBorders = 275,
		ToggleTableCellInsideBorders = 276,
		ToggleTableCellInsideHorizontalBorders = 277,
		ToggleTableCellInsideVerticalBorders = 278,
		ToggleTableCellOutsideBorders = 279,
		ToggleFirstRow = 280,
		ToggleLastRow = 281,
		ToggleFirstColumn = 282,
		ToggleLastColumn = 283,
		ToggleBandedRows = 284,
		ToggleBandedColumn = 285,
		SelectTableCell = 286,
		SelectTableColumn = 287,
		SelectTableRow = 288,
		SelectTable = 289,
		ChangeTableBorderColorRepositoryItem = 290,
		ChangeTableBorderWidthRepositoryItem = 291,
		ChangeTableBorderStyleRepositoryItem = 292,
		ShowErrorPathTooLongCommand = 293,
		ChangeTableCellShading = 294,
		ToggleShowTableGridLines = 295,
		ExtendSelectTableCell = 296,
		ExtendSelectTableColumn = 297,
		ExtendSelectTableRow = 298,
		ExtendSelectTable = 299
	}
}
