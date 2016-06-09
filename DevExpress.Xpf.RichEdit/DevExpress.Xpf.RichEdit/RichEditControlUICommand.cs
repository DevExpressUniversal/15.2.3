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
using System.Windows;
using System.Windows.Input;
using DevExpress.Office.Internal;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Office.UI;
using DevExpress.Xpf.RichEdit.UI;
using DevExpress.Xpf.RichEdit;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using FrameworkContentElement = System.Windows.FrameworkElement;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.RichEdit {
	public class RichEditUICommand : ICommand, ICommandWithInvoker {
		static readonly RichEditUICommand fileNew = new RichEditUICommand(RichEditCommandId.FileNew);
		static readonly RichEditUICommand fileOpen = new RichEditUICommand(RichEditCommandId.FileOpen);
#if !SL
		static readonly RichEditUICommand fileSave = new RichEditUICommand(RichEditCommandId.FileSave);
#endif
		static readonly RichEditUICommand fileSaveAs = new RichEditUICommand(RichEditCommandId.FileSaveAs);
		static readonly RichEditUICommand filePrint = new RichEditUICommand(RichEditCommandId.Print);
		static readonly RichEditUICommand filePrintPreview = new RichEditUICommand(RichEditCommandId.PrintPreview);
#if SL
		static readonly RichEditUICommand fileBrowserPrint = new RichEditUICommand(RichEditCommandId.BrowserPrint);
		static readonly RichEditUICommand fileBrowserPrintPreview = new RichEditUICommand(RichEditCommandId.BrowserPrintPreview);
#else
		static readonly RichEditUICommand fileQuickPrint = new RichEditUICommand(RichEditCommandId.QuickPrint);
#endif
		static readonly RichEditUICommand editUndo = new RichEditUICommand(RichEditCommandId.Undo);
		static readonly RichEditUICommand editRedo = new RichEditUICommand(RichEditCommandId.Redo);
		static readonly RichEditUICommand editCut = new RichEditUICommand(RichEditCommandId.CutSelection);
		static readonly RichEditUICommand editCopy = new RichEditUICommand(RichEditCommandId.CopySelection);
		static readonly RichEditUICommand editPaste = new RichEditUICommand(RichEditCommandId.PasteSelection);
		static readonly RichEditUICommand editPasteSpecial = new RichEditUICommand(RichEditCommandId.ShowPasteSpecialForm);
		static readonly RichEditUICommand editDelete = new RichEditUICommand(RichEditCommandId.Delete);
		static readonly RichEditUICommand editSelectAll = new RichEditUICommand(RichEditCommandId.SelectAll);
		static readonly RichEditUICommand editFind = new RichEditUICommand(RichEditCommandId.Find);
		static readonly RichEditUICommand editReplace = new RichEditUICommand(RichEditCommandId.Replace);
		static readonly RichEditUICommand editFindForward = new RichEditUICommand(RichEditCommandId.FindForward);
		static readonly RichEditUICommand editFindBackward = new RichEditUICommand(RichEditCommandId.FindBackward);
		static readonly RichEditUICommand editReplaceForward = new RichEditUICommand(RichEditCommandId.ReplaceForward);
		static readonly RichEditUICommand editReplaceAllForward = new RichEditUICommand(RichEditCommandId.ReplaceAllForward);
		static readonly RichEditUICommand viewDraft = new RichEditUICommand(RichEditCommandId.SwitchToDraftView);
		static readonly RichEditUICommand viewPrintLayout = new RichEditUICommand(RichEditCommandId.SwitchToPrintLayoutView);
		static readonly RichEditUICommand viewSimple = new RichEditUICommand(RichEditCommandId.SwitchToSimpleView);
		static readonly RichEditUICommand viewHorizontalRuler = new RichEditUICommand(RichEditCommandId.ToggleShowHorizontalRuler);
		static readonly RichEditUICommand viewVerticalRuler = new RichEditUICommand(RichEditCommandId.ToggleShowVerticalRuler);
		static readonly RichEditUICommand viewZoomIn = new RichEditUICommand(RichEditCommandId.ZoomIn);
		static readonly RichEditUICommand viewZoomOut = new RichEditUICommand(RichEditCommandId.ZoomOut);
		static readonly RichEditUICommand viewZoom = new RichEditUICommand(RichEditCommandId.Zoom);
		static readonly RichEditUICommand viewZoomPercent = new RichEditUICommand(RichEditCommandId.ZoomPercent);
		static readonly RichEditUICommand insertSymbol = new RichEditUICommand(RichEditCommandId.ShowSymbolForm);
		static readonly RichEditUICommand insertPicture = new RichEditUICommand(RichEditCommandId.InsertPicture);
		static readonly RichEditUICommand insertFloatingPicture = new RichEditUICommand(RichEditCommandId.InsertFloatingPicture);
		static readonly RichEditUICommand insertTextBox = new RichEditUICommand(RichEditCommandId.InsertTextBox);
		static readonly RichEditUICommand insertHeader = new RichEditUICommand(RichEditCommandId.EditPageHeader);
		static readonly RichEditUICommand insertFooter = new RichEditUICommand(RichEditCommandId.EditPageFooter);
		static readonly RichEditUICommand insertPageNumber = new RichEditUICommand(RichEditCommandId.InsertPageNumberField);
		static readonly RichEditUICommand insertPageCount = new RichEditUICommand(RichEditCommandId.InsertPageCountField);
		static readonly RichEditUICommand insertTable = new RichEditUICommand(RichEditCommandId.InsertTable);
		static readonly RichEditUICommand insertBookmark = new RichEditUICommand(RichEditCommandId.ShowBookmarkForm);
		static readonly RichEditUICommand insertHyperlink = new RichEditUICommand(RichEditCommandId.ShowHyperlinkForm);
		static readonly RichEditUICommand insertBreak = new RichEditUICommand(RichEditCommandId.InsertBreak);
		static readonly RichEditUICommand insertPageBreak = new RichEditUICommand(RichEditCommandId.InsertPageBreak);
		static readonly RichEditUICommand insertColumnBreak = new RichEditUICommand(RichEditCommandId.InsertColumnBreak);
		static readonly RichEditUICommand insertSectionBreakNextPage = new RichEditUICommand(RichEditCommandId.InsertSectionBreakNextPage);
		static readonly RichEditUICommand insertSectionBreakContinuous = new RichEditUICommand(RichEditCommandId.InsertSectionBreakContinuous);
		static readonly RichEditUICommand insertSectionBreakEvenPage = new RichEditUICommand(RichEditCommandId.InsertSectionBreakEvenPage);
		static readonly RichEditUICommand insertSectionBreakOddPage = new RichEditUICommand(RichEditCommandId.InsertSectionBreakOddPage);
		static readonly RichEditUICommand formatFont = new RichEditUICommand(RichEditCommandId.ShowFontForm);
		static readonly RichEditUICommand formatFontName = new RichEditUICommand(RichEditCommandId.ChangeFontName);
		static readonly RichEditUICommand formatFontSize = new RichEditUICommand(RichEditCommandId.ChangeFontSize);
		static readonly RichEditUICommand formatFontStyle = new RichEditUICommand(RichEditCommandId.ChangeFontStyle);
		static readonly RichEditUICommand formatEditFontStyle = new RichEditUICommand(RichEditCommandId.ShowEditStyleForm);
		static readonly RichEditUICommand formatFontForeColor = new RichEditUICommand(RichEditCommandId.ChangeFontForeColor);
		static readonly RichEditUICommand formatFontBackColor = new RichEditUICommand(RichEditCommandId.ChangeFontBackColor);
		static readonly RichEditUICommand formatFontBold = new RichEditUICommand(RichEditCommandId.ToggleFontBold);
		static readonly RichEditUICommand formatFontItalic = new RichEditUICommand(RichEditCommandId.ToggleFontItalic);
		static readonly RichEditUICommand formatFontUnderline = new RichEditUICommand(RichEditCommandId.ToggleFontUnderline);
		static readonly RichEditUICommand formatFontDoubleUnderline = new RichEditUICommand(RichEditCommandId.ToggleFontDoubleUnderline);
		static readonly RichEditUICommand formatFontStrikeout = new RichEditUICommand(RichEditCommandId.ToggleFontStrikeout);
		static readonly RichEditUICommand formatFontDoubleStrikeout = new RichEditUICommand(RichEditCommandId.ToggleFontDoubleStrikeout);
		static readonly RichEditUICommand formatFontSuperscript = new RichEditUICommand(RichEditCommandId.ToggleFontSuperscript);
		static readonly RichEditUICommand formatFontSubscript = new RichEditUICommand(RichEditCommandId.ToggleFontSubscript);
		static readonly RichEditUICommand formatIncreaseFontSize = new RichEditUICommand(RichEditCommandId.IncreaseFontSize);
		static readonly RichEditUICommand formatDecreaseFontSize = new RichEditUICommand(RichEditCommandId.DecreaseFontSize);
		static readonly RichEditUICommand formatClearFormatting = new RichEditUICommand(RichEditCommandId.ClearFormatting);
		static readonly RichEditUICommand editChangeCase = new RichEditUICommand(RichEditCommandId.ChangeTextCasePlaceholder);
		static readonly RichEditUICommand editMakeUpperCase = new RichEditUICommand(RichEditCommandId.MakeTextUpperCase);
		static readonly RichEditUICommand editMakeLowerCase = new RichEditUICommand(RichEditCommandId.MakeTextLowerCase);
		static readonly RichEditUICommand editToggleCase = new RichEditUICommand(RichEditCommandId.ToggleTextCase);
		static readonly RichEditUICommand formatParagraph = new RichEditUICommand(RichEditCommandId.ShowParagraphForm);
		static readonly RichEditUICommand formatParagraphAlignLeft = new RichEditUICommand(RichEditCommandId.ToggleParagraphAlignmentLeft);
		static readonly RichEditUICommand formatParagraphAlignCenter = new RichEditUICommand(RichEditCommandId.ToggleParagraphAlignmentCenter);
		static readonly RichEditUICommand formatParagraphAlignRight = new RichEditUICommand(RichEditCommandId.ToggleParagraphAlignmentRight);
		static readonly RichEditUICommand formatParagraphAlignJustify = new RichEditUICommand(RichEditCommandId.ToggleParagraphAlignmentJustify);
		static readonly RichEditUICommand formatParagraphLineSpacing = new RichEditUICommand(RichEditCommandId.ChangeParagraphLineSpacing);
		static readonly RichEditUICommand formatParagraphLineSpacingSingle = new RichEditUICommand(RichEditCommandId.SetSingleParagraphSpacing);
		static readonly RichEditUICommand formatParagraphLineSpacingSesquialteral = new RichEditUICommand(RichEditCommandId.SetSesquialteralParagraphSpacing);
		static readonly RichEditUICommand formatParagraphLineSpacingDouble = new RichEditUICommand(RichEditCommandId.SetDoubleParagraphSpacing);
		static readonly RichEditUICommand formatParagraphLineSpacingCustomize = new RichEditUICommand(RichEditCommandId.ShowLineSpacingForm);
		static readonly RichEditUICommand formatParagraphAddSpacingBefore = new RichEditUICommand(RichEditCommandId.AddSpacingBeforeParagraph);
		static readonly RichEditUICommand formatParagraphRemoveSpacingBefore = new RichEditUICommand(RichEditCommandId.RemoveSpacingBeforeParagraph);
		static readonly RichEditUICommand formatParagraphAddSpacingAfter = new RichEditUICommand(RichEditCommandId.AddSpacingAfterParagraph);
		static readonly RichEditUICommand formatParagraphRemoveSpacingAfter = new RichEditUICommand(RichEditCommandId.RemoveSpacingAfterParagraph);
		static readonly RichEditUICommand formatParagraphSuppressLineNumbers = new RichEditUICommand(RichEditCommandId.ToggleParagraphSuppressLineNumbers);
		static readonly RichEditUICommand formatParagraphBackColor = new RichEditUICommand(RichEditCommandId.ChangeParagraphBackColor);
		static readonly RichEditUICommand formatIncreaseIndent = new RichEditUICommand(RichEditCommandId.IncreaseIndent);
		static readonly RichEditUICommand formatDecreaseIndent = new RichEditUICommand(RichEditCommandId.DecreaseIndent);
		static readonly RichEditUICommand viewShowWhitespace = new RichEditUICommand(RichEditCommandId.ToggleShowWhitespace);
		static readonly RichEditUICommand formatBulletedList = new RichEditUICommand(RichEditCommandId.ToggleBulletedListItem);
		static readonly RichEditUICommand formatNumberingList = new RichEditUICommand(RichEditCommandId.ToggleNumberingListItem);
		static readonly RichEditUICommand formatMultilevelList = new RichEditUICommand(RichEditCommandId.ToggleMultilevelListItem);
		static readonly RichEditUICommand pageLayoutMargins = new RichEditUICommand(RichEditCommandId.ChangeSectionPageMargins);
		static readonly RichEditUICommand pageLayoutNormalMargins = new RichEditUICommand(RichEditCommandId.SetNormalSectionPageMargins);
		static readonly RichEditUICommand pageLayoutNarrowMargins = new RichEditUICommand(RichEditCommandId.SetNarrowSectionPageMargins);
		static readonly RichEditUICommand pageLayoutModerateMargins = new RichEditUICommand(RichEditCommandId.SetModerateSectionPageMargins);
		static readonly RichEditUICommand pageLayoutWideMargins = new RichEditUICommand(RichEditCommandId.SetWideSectionPageMargins);
		static readonly RichEditUICommand pageLayoutOrientation = new RichEditUICommand(RichEditCommandId.ChangeSectionPageOrientation);
		static readonly RichEditUICommand pageLayoutPortraitOrientation = new RichEditUICommand(RichEditCommandId.SetPortraitPageOrientation);
		static readonly RichEditUICommand pageLayoutLandscapeOrientation = new RichEditUICommand(RichEditCommandId.SetLandscapePageOrientation);
		static readonly RichEditUICommand pageLayoutOneColumn = new RichEditUICommand(RichEditCommandId.SetSectionOneColumn);
		static readonly RichEditUICommand pageLayoutTwoColumns = new RichEditUICommand(RichEditCommandId.SetSectionTwoColumns);
		static readonly RichEditUICommand pageLayoutThreeColumns = new RichEditUICommand(RichEditCommandId.SetSectionThreeColumns);
		static readonly RichEditUICommand pageLayoutColumnsOptions = new RichEditUICommand(RichEditCommandId.ShowColumnsSetupForm);
		static readonly RichEditUICommand pageLayoutColumns = new RichEditUICommand(RichEditCommandId.SetSectionColumnsPlaceholder);
		static readonly RichEditUICommand pageLayoutPageOptions = new RichEditUICommand(RichEditCommandId.ShowPageSetupForm);
		static readonly RichEditUICommand pageLayoutPageMarginsOptions = new RichEditUICommand(RichEditCommandId.ShowPageMarginsSetupForm);
		static readonly RichEditUICommand pageLayoutPagePaperOptions = new RichEditUICommand(RichEditCommandId.ShowPagePaperSetupForm);
		static readonly RichEditUICommand pageLayoutSize = new RichEditUICommand(RichEditCommandId.ChangeSectionPaperKindPlaceholder);
		static readonly RichEditUICommand pageLayoutLineNumbering = new RichEditUICommand(RichEditCommandId.ChangeSectionLineNumbering);
		static readonly RichEditUICommand pageLayoutLineNumberingNone = new RichEditUICommand(RichEditCommandId.SetSectionLineNumberingNone);
		static readonly RichEditUICommand pageLayoutLineNumberingContinuous = new RichEditUICommand(RichEditCommandId.SetSectionLineNumberingContinuous);
		static readonly RichEditUICommand pageLayoutLineNumberingRestartNewPage = new RichEditUICommand(RichEditCommandId.SetSectionLineNumberingRestartNewPage);
		static readonly RichEditUICommand pageLayoutLineNumberingRestartNewSection = new RichEditUICommand(RichEditCommandId.SetSectionLineNumberingRestartNewSection);
		static readonly RichEditUICommand pageLayoutLineNumberingOptions = new RichEditUICommand(RichEditCommandId.ShowLineNumberingForm);
		static readonly RichEditUICommand pageLayoutPageColor = new RichEditUICommand(RichEditCommandId.ChangePageColor);
		static readonly RichEditUICommand goToPage = new RichEditUICommand(RichEditCommandId.GoToPage);
		static readonly RichEditUICommand goToHeader = new RichEditUICommand(RichEditCommandId.GoToPageHeader);
		static readonly RichEditUICommand goToFooter = new RichEditUICommand(RichEditCommandId.GoToPageFooter);
		static readonly RichEditUICommand headerFooterGoToPrevious = new RichEditUICommand(RichEditCommandId.GoToPreviousHeaderFooter);
		static readonly RichEditUICommand headerFooterGoToNext = new RichEditUICommand(RichEditCommandId.GoToNextHeaderFooter);
		static readonly RichEditUICommand headerFooterLinkToPrevious = new RichEditUICommand(RichEditCommandId.ToggleHeaderFooterLinkToPrevious);
		static readonly RichEditUICommand headerFooterDifferentFirstPage = new RichEditUICommand(RichEditCommandId.ToggleDifferentFirstPage);
		static readonly RichEditUICommand headerFooterDifferentOddEvenPages = new RichEditUICommand(RichEditCommandId.ToggleDifferentOddAndEvenPages);
		static readonly RichEditUICommand headerFooterClose = new RichEditUICommand(RichEditCommandId.ClosePageHeaderFooter);
		static readonly RichEditUICommand tableSelectElement = new RichEditUICommand(RichEditCommandId.SelectTablePlaceholder);
		static readonly RichEditUICommand tableSelectCell = new RichEditUICommand(RichEditCommandId.SelectTableCell);
		static readonly RichEditUICommand tableSelectColumn = new RichEditUICommand(RichEditCommandId.SelectTableColumns);
		static readonly RichEditUICommand tableSelectRow = new RichEditUICommand(RichEditCommandId.SelectTableRow);
		static readonly RichEditUICommand tableSelect = new RichEditUICommand(RichEditCommandId.SelectTable);
		static readonly RichEditUICommand tableToggleShowGridlines = new RichEditUICommand(RichEditCommandId.ToggleShowTableGridLines);
		static readonly RichEditUICommand tableProperties = new RichEditUICommand(RichEditCommandId.ShowTablePropertiesForm);
		static readonly RichEditUICommand tableOptions = new RichEditUICommand(RichEditCommandId.ShowTableOptionsForm);
		static readonly RichEditUICommand tableDeleteElement = new RichEditUICommand(RichEditCommandId.DeleteTablePlaceholder);
		static readonly RichEditUICommand tableDeleteCell = new RichEditUICommand(RichEditCommandId.ShowDeleteTableCellsForm);
		static readonly RichEditUICommand tableDeleteColumn = new RichEditUICommand(RichEditCommandId.DeleteTableColumns);
		static readonly RichEditUICommand tableDeleteRow = new RichEditUICommand(RichEditCommandId.DeleteTableRows);
		static readonly RichEditUICommand tableDelete = new RichEditUICommand(RichEditCommandId.DeleteTable);
		static readonly RichEditUICommand tableInsertRowAbove = new RichEditUICommand(RichEditCommandId.InsertTableRowAbove);
		static readonly RichEditUICommand tableInsertRowBelow = new RichEditUICommand(RichEditCommandId.InsertTableRowBelow);
		static readonly RichEditUICommand tableInsertColumnToLeft = new RichEditUICommand(RichEditCommandId.InsertTableColumnToTheLeft);
		static readonly RichEditUICommand tableInsertColumnToRight = new RichEditUICommand(RichEditCommandId.InsertTableColumnToTheRight);
		static readonly RichEditUICommand tableChangeBorders = new RichEditUICommand(RichEditCommandId.ChangeTableBordersPlaceholder);
		static readonly RichEditUICommand tableToggleBottomBorder = new RichEditUICommand(RichEditCommandId.ToggleTableCellsBottomBorder);
		static readonly RichEditUICommand tableToggleTopBorder = new RichEditUICommand(RichEditCommandId.ToggleTableCellsTopBorder);
		static readonly RichEditUICommand tableToggleLeftBorder = new RichEditUICommand(RichEditCommandId.ToggleTableCellsLeftBorder);
		static readonly RichEditUICommand tableToggleRightBorder = new RichEditUICommand(RichEditCommandId.ToggleTableCellsRightBorder);
		static readonly RichEditUICommand tableResetAllBorders = new RichEditUICommand(RichEditCommandId.ResetTableCellsAllBorders);
		static readonly RichEditUICommand tableToggleAllBorders = new RichEditUICommand(RichEditCommandId.ToggleTableCellsAllBorders);
		static readonly RichEditUICommand tableToggleOutsideBorder = new RichEditUICommand(RichEditCommandId.ToggleTableCellsOutsideBorder);
		static readonly RichEditUICommand tableToggleInsideBorder = new RichEditUICommand(RichEditCommandId.ToggleTableCellsInsideBorder);
		static readonly RichEditUICommand tableToggleInsideHorizontalBorder = new RichEditUICommand(RichEditCommandId.ToggleTableCellsInsideHorizontalBorder);
		static readonly RichEditUICommand tableToggleInsideVerticalBorder = new RichEditUICommand(RichEditCommandId.ToggleTableCellsInsideVerticalBorder);
		static readonly RichEditUICommand tableInsertCells = new RichEditUICommand(RichEditCommandId.ShowInsertTableCellsForm);
		static readonly RichEditUICommand toggleTableAutoFit = new RichEditUICommand(RichEditCommandId.ToggleTableAutoFitPlaceholder);
		static readonly RichEditUICommand toggleTableAutoFitContents = new RichEditUICommand(RichEditCommandId.ToggleTableAutoFitContents);
		static readonly RichEditUICommand toggleTableAutoFitWindow = new RichEditUICommand(RichEditCommandId.ToggleTableAutoFitWindow);
		static readonly RichEditUICommand toggleTableFixedColumnWidth = new RichEditUICommand(RichEditCommandId.ToggleTableFixedColumnWidth);
		static readonly RichEditUICommand tableMergeCells = new RichEditUICommand(RichEditCommandId.MergeTableCells);
		static readonly RichEditUICommand tableSplitCells = new RichEditUICommand(RichEditCommandId.ShowSplitTableCellsForm);
		static readonly RichEditUICommand tableSplit = new RichEditUICommand(RichEditCommandId.SplitTable);
		static readonly RichEditUICommand tableChangeCellsShading = new RichEditUICommand(RichEditCommandId.ChangeTableCellsShading);
		static readonly RichEditUICommand tableChangeCellsBorderColor = new RichEditUICommand(RichEditCommandId.ChangeTableCellsBorderColor);
		static readonly RichEditUICommand tableChangeCellsBorderLineStyle = new RichEditUICommand(RichEditCommandId.ChangeTableCellsBorderLineStyle);
		static readonly RichEditUICommand tableChangeCellsContentAlignment = new RichEditUICommand(RichEditCommandId.ChangeTableCellsContentAlignmentPlaceholder);
		static readonly RichEditUICommand tableToggleCellsTopLeftAlignment = new RichEditUICommand(RichEditCommandId.ToggleTableCellsTopLeftAlignment);
		static readonly RichEditUICommand tableToggleCellsTopCenterAlignment = new RichEditUICommand(RichEditCommandId.ToggleTableCellsTopCenterAlignment);
		static readonly RichEditUICommand tableToggleCellsTopRightAlignment = new RichEditUICommand(RichEditCommandId.ToggleTableCellsTopRightAlignment);
		static readonly RichEditUICommand tableToggleCellsMiddleLeftAlignment = new RichEditUICommand(RichEditCommandId.ToggleTableCellsMiddleLeftAlignment);
		static readonly RichEditUICommand tableToggleCellsMiddleCenterAlignment = new RichEditUICommand(RichEditCommandId.ToggleTableCellsMiddleCenterAlignment);
		static readonly RichEditUICommand tableToggleCellsMiddleRightAlignment = new RichEditUICommand(RichEditCommandId.ToggleTableCellsMiddleRightAlignment);
		static readonly RichEditUICommand tableToggleCellsBottomLeftAlignment = new RichEditUICommand(RichEditCommandId.ToggleTableCellsBottomLeftAlignment);
		static readonly RichEditUICommand tableToggleCellsBottomCenterAlignment = new RichEditUICommand(RichEditCommandId.ToggleTableCellsBottomCenterAlignment);
		static readonly RichEditUICommand tableToggleCellsBottomRightAlignment = new RichEditUICommand(RichEditCommandId.ToggleTableCellsBottomRightAlignment);
		static readonly RichEditUICommand toolsCheckSpelling = new RichEditUICommand(RichEditCommandId.CheckSpelling);
		static readonly RichEditUICommand toolsSpellCheckAsYouType = new RichEditUICommand(RichEditCommandId.ToggleSpellCheckAsYouType);
		static readonly RichEditUICommand mailMergeViewMergedData = new RichEditUICommand(RichEditCommandId.ToggleViewMergedData);
		static readonly RichEditUICommand mailMergeShowAllFieldCodes = new RichEditUICommand(RichEditCommandId.ShowAllFieldCodes);
		static readonly RichEditUICommand mailMergeShowAllFieldResults = new RichEditUICommand(RichEditCommandId.ShowAllFieldResults);
		static readonly RichEditUICommand mailMergeLastDataRecord = new RichEditUICommand(RichEditCommandId.LastDataRecord);
		static readonly RichEditUICommand mailMergeFirstDataRecord = new RichEditUICommand(RichEditCommandId.FirstDataRecord);
		static readonly RichEditUICommand mailMergeNextDataRecord = new RichEditUICommand(RichEditCommandId.NextDataRecord);
		static readonly RichEditUICommand mailMergePreviousDataRecord = new RichEditUICommand(RichEditCommandId.PreviousDataRecord);
		static readonly RichEditUICommand mailMergeSaveDocumentAs = new RichEditUICommand(RichEditCommandId.MailMergeSaveDocumentAs);
		static readonly RichEditUICommand mailMergeInsertFieldPlaceholder = new RichEditUICommand(RichEditCommandId.InsertMailMergeFieldPlaceholder);
		static readonly RichEditUICommand reviewProtectDocument = new RichEditUICommand(RichEditCommandId.ProtectDocument);
		static readonly RichEditUICommand reviewUnprotectDocument = new RichEditUICommand(RichEditCommandId.UnprotectDocument);
		static readonly RichEditUICommand reviewEditPermissionRange = new RichEditUICommand(RichEditCommandId.ShowRangeEditingPermissionsForm);
		static readonly RichEditUICommand reviewLanguage = new RichEditUICommand(RichEditCommandId.ChangeLanguage);
		static readonly RichEditUICommand reviewNewComment = new RichEditUICommand(RichEditCommandId.NewComment);
		static readonly RichEditUICommand reviewDeleteCommentsPlaceholder = new RichEditUICommand(RichEditCommandId.DeleteCommentsPlaceholder);
		static readonly RichEditUICommand reviewDeleteAllComments = new RichEditUICommand(RichEditCommandId.DeleteAllComments);
		static readonly RichEditUICommand reviewDeleteAllCommentsShown = new RichEditUICommand(RichEditCommandId.DeleteAllCommentsShown);
		static readonly RichEditUICommand reviewDeleteOneComment = new RichEditUICommand(RichEditCommandId.DeleteOneComment);
		static readonly RichEditUICommand reviewNextComment = new RichEditUICommand(RichEditCommandId.NextComment);
		static readonly RichEditUICommand reviewPreviousComment = new RichEditUICommand(RichEditCommandId.PreviousComment);
		static readonly RichEditUICommand reviewViewComment = new RichEditUICommand(RichEditCommandId.ViewComments);
		static readonly RichEditUICommand reviewReviewers = new RichEditUICommand(RichEditCommandId.Reviewers);
		static readonly RichEditUICommand reviewReviewingPane = new RichEditUICommand(RichEditCommandId.ShowReviewingPane);
		static readonly RichEditUICommand referencesInsertTableOfContents = new RichEditUICommand(RichEditCommandId.InsertTableOfContents);
		static readonly RichEditUICommand referencesUpdateTableOfContents = new RichEditUICommand(RichEditCommandId.UpdateTableOfContents);
		static readonly RichEditUICommand referencesAddParagraphsToTableOfContents = new RichEditUICommand(RichEditCommandId.AddParagraphsToTableOfContents);
		static readonly RichEditUICommand referencesInsertTableOfEquations = new RichEditUICommand(RichEditCommandId.InsertTableOfEquations);
		static readonly RichEditUICommand referencesInsertTableOfFigures = new RichEditUICommand(RichEditCommandId.InsertTableOfFigures);
		static readonly RichEditUICommand referencesInsertTableOfTables = new RichEditUICommand(RichEditCommandId.InsertTableOfTables);
		static readonly RichEditUICommand referencesInsertTableOfFiguresPlaceholder = new RichEditUICommand(RichEditCommandId.InsertTableOfFiguresPlaceholder);
		static readonly RichEditUICommand referencesInsertEquationsCaption = new RichEditUICommand(RichEditCommandId.InsertEquationsCaption);
		static readonly RichEditUICommand referencesInsertFiguresCaption = new RichEditUICommand(RichEditCommandId.InsertFiguresCaption);
		static readonly RichEditUICommand referencesInsertTablesCaption = new RichEditUICommand(RichEditCommandId.InsertTablesCaption);
		static readonly RichEditUICommand referencesInsertCaptionPlaceholder = new RichEditUICommand(RichEditCommandId.InsertCaptionPlaceholder);
		static readonly RichEditUICommand referencesUpdateTableOfCaptions = new RichEditUICommand(RichEditCommandId.UpdateTableOfContents);
		static readonly RichEditUICommand formatParagraphSetBodyTextLevel = new RichEditUICommand(RichEditCommandId.SetParagraphBodyTextLevel);
		static readonly RichEditUICommand formatParagraphSetHeading1Level = new RichEditUICommand(RichEditCommandId.SetParagraphHeading1Level);
		static readonly RichEditUICommand formatParagraphSetHeading2Level = new RichEditUICommand(RichEditCommandId.SetParagraphHeading2Level);
		static readonly RichEditUICommand formatParagraphSetHeading3Level = new RichEditUICommand(RichEditCommandId.SetParagraphHeading3Level);
		static readonly RichEditUICommand formatParagraphSetHeading4Level = new RichEditUICommand(RichEditCommandId.SetParagraphHeading4Level);
		static readonly RichEditUICommand formatParagraphSetHeading5Level = new RichEditUICommand(RichEditCommandId.SetParagraphHeading5Level);
		static readonly RichEditUICommand formatParagraphSetHeading6Level = new RichEditUICommand(RichEditCommandId.SetParagraphHeading6Level);
		static readonly RichEditUICommand formatParagraphSetHeading7Level = new RichEditUICommand(RichEditCommandId.SetParagraphHeading7Level);
		static readonly RichEditUICommand formatParagraphSetHeading8Level = new RichEditUICommand(RichEditCommandId.SetParagraphHeading8Level);
		static readonly RichEditUICommand formatParagraphSetHeading9Level = new RichEditUICommand(RichEditCommandId.SetParagraphHeading9Level);
		static readonly RichEditUICommand pictureWrapText = new RichEditUICommand(RichEditCommandId.ChangeFloatingObjectTextWrapType);
		static readonly RichEditUICommand pictureWrapTextSquare = new RichEditUICommand(RichEditCommandId.SetFloatingObjectSquareTextWrapType);
		static readonly RichEditUICommand pictureWrapTextThrough = new RichEditUICommand(RichEditCommandId.SetFloatingObjectThroughTextWrapType);
		static readonly RichEditUICommand pictureWrapTextTight = new RichEditUICommand(RichEditCommandId.SetFloatingObjectTightTextWrapType);
		static readonly RichEditUICommand pictureWrapTextTopAndBottom = new RichEditUICommand(RichEditCommandId.SetFloatingObjectTopAndBottomTextWrapType);
		static readonly RichEditUICommand pictureWrapTextInFrontOf = new RichEditUICommand(RichEditCommandId.SetFloatingObjectInFrontOfTextWrapType);
		static readonly RichEditUICommand pictureWrapTextBehind = new RichEditUICommand(RichEditCommandId.SetFloatingObjectBehindTextWrapType);
		static readonly RichEditUICommand picturePosition = new RichEditUICommand(RichEditCommandId.ChangeFloatingObjectAlignment);
		static readonly RichEditUICommand pictureTopLeftAlignment = new RichEditUICommand(RichEditCommandId.SetFloatingObjectTopLeftAlignment);
		static readonly RichEditUICommand pictureTopCenterAlignment = new RichEditUICommand(RichEditCommandId.SetFloatingObjectTopCenterAlignment);
		static readonly RichEditUICommand pictureTopRightAlignment = new RichEditUICommand(RichEditCommandId.SetFloatingObjectTopRightAlignment);
		static readonly RichEditUICommand pictureMiddleLeftAlignment = new RichEditUICommand(RichEditCommandId.SetFloatingObjectMiddleLeftAlignment);
		static readonly RichEditUICommand pictureMiddleCenterAlignment = new RichEditUICommand(RichEditCommandId.SetFloatingObjectMiddleCenterAlignment);
		static readonly RichEditUICommand pictureMiddleRightAlignment = new RichEditUICommand(RichEditCommandId.SetFloatingObjectMiddleRightAlignment);
		static readonly RichEditUICommand pictureBottomLeftAlignment = new RichEditUICommand(RichEditCommandId.SetFloatingObjectBottomLeftAlignment);
		static readonly RichEditUICommand pictureBottomCenterAlignment = new RichEditUICommand(RichEditCommandId.SetFloatingObjectBottomCenterAlignment);
		static readonly RichEditUICommand pictureBottomRightAlignment = new RichEditUICommand(RichEditCommandId.SetFloatingObjectBottomRightAlignment);
		static readonly RichEditUICommand pictureBringForwardPlaceholder = new RichEditUICommand(RichEditCommandId.FloatingObjectBringForwardPlaceholder);
		static readonly RichEditUICommand pictureBringForward = new RichEditUICommand(RichEditCommandId.FloatingObjectBringForward);
		static readonly RichEditUICommand pictureBringToFront = new RichEditUICommand(RichEditCommandId.FloatingObjectBringToFront);
		static readonly RichEditUICommand pictureBringInFrontOfText = new RichEditUICommand(RichEditCommandId.FloatingObjectBringInFrontOfText);
		static readonly RichEditUICommand pictureSendBackwardPlaceholder = new RichEditUICommand(RichEditCommandId.FloatingObjectSendBackwardPlaceholder);
		static readonly RichEditUICommand pictureSendBackward = new RichEditUICommand(RichEditCommandId.FloatingObjectSendBackward);
		static readonly RichEditUICommand pictureSendToBack = new RichEditUICommand(RichEditCommandId.FloatingObjectSendToBack);
		static readonly RichEditUICommand pictureSendBehindText = new RichEditUICommand(RichEditCommandId.FloatingObjectSendBehindText);
		static readonly RichEditUICommand pictureShapeFillColor = new RichEditUICommand(RichEditCommandId.ChangeFloatingObjectFillColor);
		static readonly RichEditUICommand pictureShapeOutlineColor = new RichEditUICommand(RichEditCommandId.ChangeFloatingObjectOutlineColor);
		static readonly RichEditUICommand pictureShapeOutlineWeight = new RichEditUICommand(RichEditCommandId.ChangeFloatingObjectOutlineWeight);
		static readonly RichEditUICommand deleteRepeatedWord = new RichEditUICommand(RichEditCommandId.DeleteRepeatedWord);
		static readonly RichEditUICommand replaceMisspelling = new RichEditUICommand(RichEditCommandId.ReplaceMisspelling);
		static readonly RichEditUICommand ignoreMisspelling = new RichEditUICommand(RichEditCommandId.IgnoreMisspelling);
		static readonly RichEditUICommand ignoreAllMisspellings = new RichEditUICommand(RichEditCommandId.IgnoreAllMisspellings);
		static readonly RichEditUICommand addWordToDictionary = new RichEditUICommand(RichEditCommandId.AddWordToDictionary);
		static readonly RichEditUICommand toolsFloatingPictureCommandGroup = new RichEditUICommand(RichEditCommandId.ToolsFloatingPictureCommandGroup);
		static readonly RichEditUICommand toolsTableCommandGroup = new RichEditUICommand(RichEditCommandId.ToolsTableCommandGroup);
		static readonly RichEditUICommand toolsHeaderFooterCommandGroup = new RichEditUICommand(RichEditCommandId.ToolsHeaderFooterCommandGroup);
		public static RichEditUICommand FileOpen { get { return fileOpen; } }
		public static RichEditUICommand FileNew { get { return fileNew; } }
#if !SL
		public static RichEditUICommand FileSave { get { return fileSave; } }
#endif
		public static RichEditUICommand FileSaveAs { get { return fileSaveAs; } }
		public static RichEditUICommand FilePrint { get { return filePrint; } }
		public static RichEditUICommand FilePrintPreview { get { return filePrintPreview; } }
#if SL
		public static RichEditUICommand FileBrowserPrint { get { return fileBrowserPrint; } }
		public static RichEditUICommand FileBrowserPrintPreview { get { return fileBrowserPrintPreview; } }
#else
		public static RichEditUICommand FileQuickPrint { get { return fileQuickPrint; } }
#endif
		public static RichEditUICommand EditUndo { get { return editUndo; } }
		public static RichEditUICommand EditRedo { get { return editRedo; } }
		public static RichEditUICommand EditCut { get { return editCut; } }
		public static RichEditUICommand EditCopy { get { return editCopy; } }
		public static RichEditUICommand EditPaste { get { return editPaste; } }
		public static RichEditUICommand EditPasteSpecial { get { return editPasteSpecial; } }
		public static RichEditUICommand EditDelete { get { return editDelete; } }
		public static RichEditUICommand EditSelectAll { get { return editSelectAll; } }
		public static RichEditUICommand EditFind { get { return editFind; } }
		public static RichEditUICommand EditReplace { get { return editReplace; } }
		public static RichEditUICommand EditFindForward { get { return editFindForward; } }
		public static RichEditUICommand EditFindBackward { get { return editFindBackward; } }
		public static RichEditUICommand EditReplaceForward { get { return editReplaceForward; } }
		public static RichEditUICommand EditReplaceAllForward { get { return editReplaceAllForward; } }
		public static RichEditUICommand ViewDraft { get { return viewDraft; } }
		public static RichEditUICommand ViewPrintLayout { get { return viewPrintLayout; } }
		public static RichEditUICommand ViewSimple { get { return viewSimple; } }
		public static RichEditUICommand ViewShowWhitespace { get { return viewShowWhitespace; } }
		public static RichEditUICommand ViewHorizontalRuler { get { return viewHorizontalRuler; } }
		public static RichEditUICommand ViewVerticalRuler { get { return viewVerticalRuler; } }
		public static RichEditUICommand ViewZoomIn { get { return viewZoomIn; } }
		public static RichEditUICommand ViewZoomOut { get { return viewZoomOut; } }
		public static RichEditUICommand ViewZoom { get { return viewZoom; } }
		public static RichEditUICommand ViewZoomPercent { get { return viewZoomPercent; } }
		public static RichEditUICommand InsertSymbol { get { return insertSymbol; } }
		public static RichEditUICommand InsertPicture { get { return insertPicture; } }
		public static RichEditUICommand InsertFloatingPicture { get { return insertFloatingPicture; } }
		public static RichEditUICommand InsertTextBox { get { return insertTextBox; } }
		public static RichEditUICommand InsertHeader { get { return insertHeader; } }
		public static RichEditUICommand InsertFooter { get { return insertFooter; } }
		public static RichEditUICommand InsertPageNumber { get { return insertPageNumber; } }
		public static RichEditUICommand InsertPageCount { get { return insertPageCount; } }
		public static RichEditUICommand InsertTable { get { return insertTable; } }
		public static RichEditUICommand InsertBookmark { get { return insertBookmark; } }
		public static RichEditUICommand InsertHyperlink { get { return insertHyperlink; } }
		public static RichEditUICommand InsertBreak { get { return insertBreak; } }
		public static RichEditUICommand InsertPageBreak { get { return insertPageBreak; } }
		public static RichEditUICommand InsertColumnBreak { get { return insertColumnBreak; } }
		public static RichEditUICommand InsertSectionBreakNextPage { get { return insertSectionBreakNextPage; } }
		public static RichEditUICommand InsertSectionBreakContinuous { get { return insertSectionBreakContinuous; } }
		public static RichEditUICommand InsertSectionBreakEvenPage { get { return insertSectionBreakEvenPage; } }
		public static RichEditUICommand InsertSectionBreakOddPage { get { return insertSectionBreakOddPage; } }
		public static RichEditUICommand FormatFont { get { return formatFont; } }
		public static RichEditUICommand FormatFontName { get { return formatFontName; } }
		public static RichEditUICommand FormatFontSize { get { return formatFontSize; } }
		public static RichEditUICommand FormatFontStyle { get { return formatFontStyle; } }
		public static RichEditUICommand FormatEditFontStyle { get { return formatEditFontStyle; } }
		public static RichEditUICommand FormatFontForeColor { get { return formatFontForeColor; } }
		public static RichEditUICommand FormatFontBackColor { get { return formatFontBackColor; } }
		public static RichEditUICommand FormatFontBold { get { return formatFontBold; } }
		public static RichEditUICommand FormatFontItalic { get { return formatFontItalic; } }
		public static RichEditUICommand FormatFontUnderline { get { return formatFontUnderline; } }
		public static RichEditUICommand FormatFontDoubleUnderline { get { return formatFontDoubleUnderline; } }
		public static RichEditUICommand FormatFontStrikeout { get { return formatFontStrikeout; } }
		public static RichEditUICommand FormatFontDoubleStrikeout { get { return formatFontDoubleStrikeout; } }
		public static RichEditUICommand FormatFontSuperscript { get { return formatFontSuperscript; } }
		public static RichEditUICommand FormatFontSubscript { get { return formatFontSubscript; } }
		public static RichEditUICommand FormatIncreaseFontSize { get { return formatIncreaseFontSize; } }
		public static RichEditUICommand FormatDecreaseFontSize { get { return formatDecreaseFontSize; } }
		public static RichEditUICommand FormatClearFormatting { get { return formatClearFormatting; } }
		public static RichEditUICommand EditChangeCase { get { return editChangeCase; } }
		public static RichEditUICommand EditMakeUpperCase { get { return editMakeUpperCase; } }
		public static RichEditUICommand EditMakeLowerCase { get { return editMakeLowerCase; } }
		public static RichEditUICommand EditToggleCase { get { return editToggleCase; } }
		public static RichEditUICommand FormatParagraph { get { return formatParagraph; } }
		public static RichEditUICommand FormatParagraphAlignLeft { get { return formatParagraphAlignLeft; } }
		public static RichEditUICommand FormatParagraphAlignCenter { get { return formatParagraphAlignCenter; } }
		public static RichEditUICommand FormatParagraphAlignRight { get { return formatParagraphAlignRight; } }
		public static RichEditUICommand FormatParagraphAlignJustify { get { return formatParagraphAlignJustify; } }
		public static RichEditUICommand FormatParagraphLineSpacing { get { return formatParagraphLineSpacing; } }
		public static RichEditUICommand FormatParagraphLineSpacingSingle { get { return formatParagraphLineSpacingSingle; } }
		public static RichEditUICommand FormatParagraphLineSpacingSesquialteral { get { return formatParagraphLineSpacingSesquialteral; } }
		public static RichEditUICommand FormatParagraphLineSpacingDouble { get { return formatParagraphLineSpacingDouble; } }
		public static RichEditUICommand FormatParagraphLineSpacingCustomize { get { return formatParagraphLineSpacingCustomize; } }
		public static RichEditUICommand FormatParagraphAddSpacingBefore { get { return formatParagraphAddSpacingBefore; } }
		public static RichEditUICommand FormatParagraphRemoveSpacingBefore { get { return formatParagraphRemoveSpacingBefore; } }
		public static RichEditUICommand FormatParagraphAddSpacingAfter { get { return formatParagraphAddSpacingAfter; } }
		public static RichEditUICommand FormatParagraphRemoveSpacingAfter { get { return formatParagraphRemoveSpacingAfter; } }
		public static RichEditUICommand FormatParagraphSuppressLineNumbers { get { return formatParagraphSuppressLineNumbers; } }
		public static RichEditUICommand FormatParagraphBackColor { get { return formatParagraphBackColor; } }
		public static RichEditUICommand FormatIncreaseIndent { get { return formatIncreaseIndent; } }
		public static RichEditUICommand FormatDecreaseIndent { get { return formatDecreaseIndent; } }
		public static RichEditUICommand FormatBulletedList { get { return formatBulletedList; } }
		public static RichEditUICommand FormatNumberingList { get { return formatNumberingList; } }
		public static RichEditUICommand FormatMultilevelList { get { return formatMultilevelList; } }
		public static RichEditUICommand PageLayoutMargins { get { return pageLayoutMargins; } }
		public static RichEditUICommand PageLayoutNormalMargins { get { return pageLayoutNormalMargins; } }
		public static RichEditUICommand PageLayoutNarrowMargins { get { return pageLayoutNarrowMargins; } }
		public static RichEditUICommand PageLayoutModerateMargins { get { return pageLayoutModerateMargins; } }
		public static RichEditUICommand PageLayoutWideMargins { get { return pageLayoutWideMargins; } }
		public static RichEditUICommand PageLayoutOrientation { get { return pageLayoutOrientation; } }
		public static RichEditUICommand PageLayoutPortraitOrientation { get { return pageLayoutPortraitOrientation; } }
		public static RichEditUICommand PageLayoutLandscapeOrientation { get { return pageLayoutLandscapeOrientation; } }
		public static RichEditUICommand PageLayoutOneColumn { get { return pageLayoutOneColumn; } }
		public static RichEditUICommand PageLayoutTwoColumns { get { return pageLayoutTwoColumns; } }
		public static RichEditUICommand PageLayoutThreeColumns { get { return pageLayoutThreeColumns; } }
		public static RichEditUICommand PageLayoutColumnsOptions { get { return pageLayoutColumnsOptions; } }
		public static RichEditUICommand PageLayoutColumns { get { return pageLayoutColumns; } }
		public static RichEditUICommand PageLayoutPageOptions { get { return pageLayoutPageOptions; } }
		public static RichEditUICommand PageLayoutPageMarginsOptions { get { return pageLayoutPageMarginsOptions; } }
		public static RichEditUICommand PageLayoutPagePaperOptions { get { return pageLayoutPagePaperOptions; } }
		public static RichEditUICommand PageLayoutSize { get { return pageLayoutSize; } }
		public static RichEditUICommand PageLayoutLineNumbering { get { return pageLayoutLineNumbering; } }
		public static RichEditUICommand PageLayoutLineNumberingNone { get { return pageLayoutLineNumberingNone; } }
		public static RichEditUICommand PageLayoutLineNumberingContinuous { get { return pageLayoutLineNumberingContinuous; } }
		public static RichEditUICommand PageLayoutLineNumberingRestartNewPage { get { return pageLayoutLineNumberingRestartNewPage; } }
		public static RichEditUICommand PageLayoutLineNumberingRestartNewSection { get { return pageLayoutLineNumberingRestartNewSection; } }
		public static RichEditUICommand PageLayoutLineNumberingOptions { get { return pageLayoutLineNumberingOptions; } }
		public static RichEditUICommand PageLayoutPageColor { get { return pageLayoutPageColor; } }
		public static RichEditUICommand GoToPage { get { return goToPage; } }
		public static RichEditUICommand GoToHeader { get { return goToHeader; } }
		public static RichEditUICommand GoToFooter { get { return goToFooter; } }
		public static RichEditUICommand HeaderFooterGoToPrevious { get { return headerFooterGoToPrevious; } }
		public static RichEditUICommand HeaderFooterGoToNext { get { return headerFooterGoToNext; } }
		public static RichEditUICommand HeaderFooterLinkToPrevious { get { return headerFooterLinkToPrevious; } }
		public static RichEditUICommand HeaderFooterDifferentFirstPage { get { return headerFooterDifferentFirstPage; } }
		public static RichEditUICommand HeaderFooterDifferentOddEvenPages { get { return headerFooterDifferentOddEvenPages; } }
		public static RichEditUICommand HeaderFooterClose { get { return headerFooterClose; } }
		public static RichEditUICommand TableSelectElement { get { return tableSelectElement; } }
		public static RichEditUICommand TableSelectCell { get { return tableSelectCell; } }
		public static RichEditUICommand TableSelectColumn { get { return tableSelectColumn; } }
		public static RichEditUICommand TableSelectRow { get { return tableSelectRow; } }
		public static RichEditUICommand TableSelect { get { return tableSelect; } }
		public static RichEditUICommand TableToggleShowGridlines { get { return tableToggleShowGridlines; } }
		public static RichEditUICommand TableProperties { get { return tableProperties; } }
		public static RichEditUICommand TableOptions { get { return tableOptions; } }
		public static RichEditUICommand TableDeleteElement { get { return tableDeleteElement; } }
		public static RichEditUICommand TableDeleteCell { get { return tableDeleteCell; } }
		public static RichEditUICommand TableDeleteColumn { get { return tableDeleteColumn; } }
		public static RichEditUICommand TableDeleteRow { get { return tableDeleteRow; } }
		public static RichEditUICommand TableDelete { get { return tableDelete; } }
		public static RichEditUICommand TableInsertRowAbove { get { return tableInsertRowAbove; } }
		public static RichEditUICommand TableInsertRowBelow { get { return tableInsertRowBelow; } }
		public static RichEditUICommand TableInsertColumnToLeft { get { return tableInsertColumnToLeft; } }
		public static RichEditUICommand TableInsertColumnToRight { get { return tableInsertColumnToRight; } }
		public static RichEditUICommand TableInsertCells { get { return tableInsertCells; } }
		public static RichEditUICommand TableMergeCells { get { return tableMergeCells; } }
		public static RichEditUICommand TableSplitCells { get { return tableSplitCells; } }
		public static RichEditUICommand TableSplit { get { return tableSplit; } }
		public static RichEditUICommand TableChangeCellsShading { get { return tableChangeCellsShading; } }
		public static RichEditUICommand TableChangeCellsBorderColor { get { return tableChangeCellsBorderColor; } }
		public static RichEditUICommand TableChangeCellsBorderLineStyle { get { return tableChangeCellsBorderLineStyle; } }
		public static RichEditUICommand ToggleTableAutoFit { get { return toggleTableAutoFit; } }
		public static RichEditUICommand ToggleTableAutoFitContents { get { return toggleTableAutoFitContents; } }
		public static RichEditUICommand ToggleTableAutoFitWindow { get { return toggleTableAutoFitWindow; } }
		public static RichEditUICommand ToggleTableFixedColumnWidth { get { return toggleTableFixedColumnWidth; } }
		public static RichEditUICommand TableChangeBorders { get { return tableChangeBorders; } }
		public static RichEditUICommand TableToggleBottomBorder { get { return tableToggleBottomBorder; } }
		public static RichEditUICommand TableToggleTopBorder { get { return tableToggleTopBorder; } }
		public static RichEditUICommand TableToggleLeftBorder { get { return tableToggleLeftBorder; } }
		public static RichEditUICommand TableToggleRightBorder { get { return tableToggleRightBorder; } }
		public static RichEditUICommand TableResetAllBorders { get { return tableResetAllBorders; } }
		public static RichEditUICommand TableToggleAllBorders { get { return tableToggleAllBorders; } }
		public static RichEditUICommand TableToggleOutsideBorder { get { return tableToggleOutsideBorder; } }
		public static RichEditUICommand TableToggleInsideBorder { get { return tableToggleInsideBorder; } }
		public static RichEditUICommand TableToggleInsideHorizontalBorder { get { return tableToggleInsideHorizontalBorder; } }
		public static RichEditUICommand TableToggleInsideVerticalBorder { get { return tableToggleInsideVerticalBorder; } }
		public static RichEditUICommand TableChangeCellsContentAlignment { get { return tableChangeCellsContentAlignment; } }
		public static RichEditUICommand TableToggleCellsTopLeftAlignment { get { return tableToggleCellsTopLeftAlignment; } }
		public static RichEditUICommand TableToggleCellsTopCenterAlignment { get { return tableToggleCellsTopCenterAlignment; } }
		public static RichEditUICommand TableToggleCellsTopRightAlignment { get { return tableToggleCellsTopRightAlignment; } }
		public static RichEditUICommand TableToggleCellsMiddleLeftAlignment { get { return tableToggleCellsMiddleLeftAlignment; } }
		public static RichEditUICommand TableToggleCellsMiddleCenterAlignment { get { return tableToggleCellsMiddleCenterAlignment; } }
		public static RichEditUICommand TableToggleCellsMiddleRightAlignment { get { return tableToggleCellsMiddleRightAlignment; } }
		public static RichEditUICommand TableToggleCellsBottomLeftAlignment { get { return tableToggleCellsBottomLeftAlignment; } }
		public static RichEditUICommand TableToggleCellsBottomCenterAlignment { get { return tableToggleCellsBottomCenterAlignment; } }
		public static RichEditUICommand TableToggleCellsBottomRightAlignment { get { return tableToggleCellsBottomRightAlignment; } }
		public static RichEditUICommand CheckSpelling { get { return toolsCheckSpelling; } }
		public static RichEditUICommand SpellCheckAsYouType { get { return toolsSpellCheckAsYouType; } }
		public static RichEditUICommand MailMergeViewMergedData { get { return mailMergeViewMergedData; } }
		public static RichEditUICommand MailMergeShowAllFieldCodes { get { return mailMergeShowAllFieldCodes; } }
		public static RichEditUICommand MailMergeShowAllFieldResults { get { return mailMergeShowAllFieldResults; } }
		public static RichEditUICommand MailMergeLastDataRecord { get { return mailMergeLastDataRecord; } }
		public static RichEditUICommand MailMergeFirstDataRecord { get { return mailMergeFirstDataRecord; } }
		public static RichEditUICommand MailMergeNextDataRecord { get { return mailMergeNextDataRecord; } }
		public static RichEditUICommand MailMergePreviousDataRecord { get { return mailMergePreviousDataRecord; } }
		public static RichEditUICommand MailMergeSaveDocumentAs { get { return mailMergeSaveDocumentAs; } }
		public static RichEditUICommand MailMergeInsertFieldPlaceholder { get { return mailMergeInsertFieldPlaceholder; } }
		public static RichEditUICommand ReviewProtectDocument { get { return reviewProtectDocument; } }
		public static RichEditUICommand ReviewUnprotectDocument { get { return reviewUnprotectDocument; } }
		public static RichEditUICommand ReviewEditPermissionRange { get { return reviewEditPermissionRange; } }
		public static RichEditUICommand ReviewLanguage { get { return reviewLanguage; } }
		public static RichEditUICommand ReviewNewComment { get { return reviewNewComment; } }
		public static RichEditUICommand ReviewDeleteCommentsPlaceholder { get { return reviewDeleteCommentsPlaceholder; } }
		public static RichEditUICommand ReviewDeleteAllComments { get { return reviewDeleteAllComments; } }
		public static RichEditUICommand ReviewDeleteAllCommentsShown { get { return reviewDeleteAllCommentsShown; } }
		public static RichEditUICommand ReviewDeleteOneComment { get { return reviewDeleteOneComment; } }
		public static RichEditUICommand ReviewNextComment { get { return reviewNextComment; } }
		public static RichEditUICommand ReviewPreviousComment { get { return reviewPreviousComment; } }
		public static RichEditUICommand ReviewViewComment { get { return reviewViewComment; } }
		public static RichEditUICommand ReviewReviewingPane { get { return reviewReviewingPane; } }
		public static RichEditUICommand ReviewReviewers { get { return reviewReviewers; } }
		public static RichEditUICommand ReferencesInsertTableOfContents { get { return referencesInsertTableOfContents; } }
		public static RichEditUICommand ReferencesUpdateTableOfContents { get { return referencesUpdateTableOfContents; } }
		public static RichEditUICommand ReferencesAddParagraphsToTableOfContents { get { return referencesAddParagraphsToTableOfContents; } }
		public static RichEditUICommand ReferencesInsertTableOfEquations { get { return referencesInsertTableOfEquations; } }
		public static RichEditUICommand ReferencesInsertTableOfFigures { get { return referencesInsertTableOfFigures; } }
		public static RichEditUICommand ReferencesInsertTableOfTables { get { return referencesInsertTableOfTables; } }
		public static RichEditUICommand ReferencesInsertTableOfFiguresPlaceholder { get { return referencesInsertTableOfFiguresPlaceholder; } }
		public static RichEditUICommand ReferencesInsertEquationsCaption { get { return referencesInsertEquationsCaption; } }
		public static RichEditUICommand ReferencesInsertFiguresCaption { get { return referencesInsertFiguresCaption; } }
		public static RichEditUICommand ReferencesInsertTablesCaption { get { return referencesInsertTablesCaption; } }
		public static RichEditUICommand ReferencesInsertCaptionPlaceholder { get { return referencesInsertCaptionPlaceholder; } }
		public static RichEditUICommand ReferencesUpdateTableOfCaptions { get { return referencesUpdateTableOfCaptions; } }
		public static RichEditUICommand FormatParagraphSetBodyTextLevel { get { return formatParagraphSetBodyTextLevel; } }
		public static RichEditUICommand FormatParagraphSetHeading1Level { get { return formatParagraphSetHeading1Level; } }
		public static RichEditUICommand FormatParagraphSetHeading2Level { get { return formatParagraphSetHeading2Level; } }
		public static RichEditUICommand FormatParagraphSetHeading3Level { get { return formatParagraphSetHeading3Level; } }
		public static RichEditUICommand FormatParagraphSetHeading4Level { get { return formatParagraphSetHeading4Level; } }
		public static RichEditUICommand FormatParagraphSetHeading5Level { get { return formatParagraphSetHeading5Level; } }
		public static RichEditUICommand FormatParagraphSetHeading6Level { get { return formatParagraphSetHeading6Level; } }
		public static RichEditUICommand FormatParagraphSetHeading7Level { get { return formatParagraphSetHeading7Level; } }
		public static RichEditUICommand FormatParagraphSetHeading8Level { get { return formatParagraphSetHeading8Level; } }
		public static RichEditUICommand FormatParagraphSetHeading9Level { get { return formatParagraphSetHeading9Level; } }
		public static RichEditUICommand PictureWrapText { get { return pictureWrapText; } }
		public static RichEditUICommand PictureWrapTextSquare { get { return pictureWrapTextSquare; } }
		public static RichEditUICommand PictureWrapTextTight { get { return pictureWrapTextTight; } }
		public static RichEditUICommand PictureWrapTextThrough { get { return pictureWrapTextThrough; } }
		public static RichEditUICommand PictureWrapTextTopAndBottom { get { return pictureWrapTextTopAndBottom; } }
		public static RichEditUICommand PictureWrapTextBehind { get { return pictureWrapTextBehind; } }
		public static RichEditUICommand PictureWrapTextInFrontOf { get { return pictureWrapTextInFrontOf; } }
		public static RichEditUICommand PicturePosition { get { return picturePosition; } }
		public static RichEditUICommand PictureTopLeftAlignment { get { return pictureTopLeftAlignment; } }
		public static RichEditUICommand PictureTopCenterAlignment { get { return pictureTopCenterAlignment; } }
		public static RichEditUICommand PictureTopRightAlignment { get { return pictureTopRightAlignment; } }
		public static RichEditUICommand PictureMiddleLeftAlignment { get { return pictureMiddleLeftAlignment; } }
		public static RichEditUICommand PictureMiddleCenterAlignment { get { return pictureMiddleCenterAlignment; } }
		public static RichEditUICommand PictureMiddleRightAlignment { get { return pictureMiddleRightAlignment; } }
		public static RichEditUICommand PictureBottomLeftAlignment { get { return pictureBottomLeftAlignment; } }
		public static RichEditUICommand PictureBottomCenterAlignment { get { return pictureBottomCenterAlignment; } }
		public static RichEditUICommand PictureBottomRightAlignment { get { return pictureBottomRightAlignment; } }
		public static RichEditUICommand PictureBringForwardPlaceholder { get { return pictureBringForwardPlaceholder; } }
		public static RichEditUICommand PictureBringForward { get { return pictureBringForward; } }
		public static RichEditUICommand PictureBringToFront { get { return pictureBringToFront; } }
		public static RichEditUICommand PictureBringInFrontOfText { get { return pictureBringInFrontOfText; } }
		public static RichEditUICommand PictureSendBackwardPlaceholder { get { return pictureSendBackwardPlaceholder; } }
		public static RichEditUICommand PictureSendBackward { get { return pictureSendBackward; } }
		public static RichEditUICommand PictureSendToBack { get { return pictureSendToBack; } }
		public static RichEditUICommand PictureSendBehindText { get { return pictureSendBehindText; } }
		public static RichEditUICommand PictureShapeFillColor { get { return pictureShapeFillColor; } }
		public static RichEditUICommand PictureShapeOutlineColor { get { return pictureShapeOutlineColor; } }
		public static RichEditUICommand PictureShapeOutlineWeight { get { return pictureShapeOutlineWeight; } }
		public static RichEditUICommand DeleteRepeatedWord { get { return deleteRepeatedWord; } }
		public static RichEditUICommand ReplaceMisspelling { get { return replaceMisspelling; } }
		public static RichEditUICommand IgnoreMisspelling { get { return ignoreMisspelling; } }
		public static RichEditUICommand IgnoreAllMisspellings { get { return ignoreAllMisspellings; } }
		public static RichEditUICommand AddWordToDictionary { get { return addWordToDictionary; } }
		public static RichEditUICommand ToolsFloatingPictureCommandGroup { get { return toolsFloatingPictureCommandGroup; } }
		public static RichEditUICommand ToolsHeaderFooterCommandGroup { get { return toolsHeaderFooterCommandGroup; } }
		public static RichEditUICommand ToolsTableCommandGroup { get { return toolsTableCommandGroup; } }
		readonly RichEditCommandId id;
		readonly object parameter;
		public RichEditUICommand() {
		}
		protected internal RichEditUICommand(RichEditCommandId id) {
			this.id = id;
		}
		protected internal RichEditUICommand(RichEditCommandId id, object parameter) {
			this.id = id;
			this.parameter = parameter;
		}
		public RichEditCommandId CommandId { get { return id; } }
		#region ICommand Members
		public virtual bool CanExecute(object parameter) {
			return true;
		}
		public event EventHandler CanExecuteChanged {
			add {
			}
			remove {
			}
		}
		public virtual void Execute(object parameter) {
			Execute(null, parameter);
		}
		public virtual void Execute(object invoker, object parameter) {
			RichEditControl control = ObtainRichEditControl(parameter);
			if (control == null)
				return;
			if (this.parameter != null)
				ExecuteCommand(control, CommandId, this.parameter);
			else {
				BarSplitButtonEditItem splitEditItem = invoker as BarSplitButtonEditItem;
				if (splitEditItem != null)
					ExecuteCommand(control, CommandId, splitEditItem.EditValue);
				else
					ExecuteCommand(control, CommandId, null);
			}
		}
		protected internal virtual void ExecuteCommand(RichEditControl control, RichEditCommandId commandId, object parameter) {
			try {
				control.CommandManager.ExecuteParametrizedCommand(CommandId, parameter);
			}
			catch (Exception e) {
				if (!control.HandleException(e))
					throw;
			}
		}
		#endregion
		protected internal virtual RichEditControl ObtainRichEditControl(object parameter) {
			RichEditControl control = parameter as RichEditControl;
			if (control != null)
				return control;
			RichEditControlAccessor accessor = parameter as RichEditControlAccessor;
			if (accessor != null)
				return accessor.RichEditControl;
			RichEditControlProvider provider = parameter as RichEditControlProvider;
			if (provider == null)
				return null;
			return provider.RichEditControl;
		}
	}
	#region RichEditControlProvider
	public class RichEditControlProvider : FrameworkElement {
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(RichEditControlProvider), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditControlProvider instance = d as RichEditControlProvider;
			if (instance != null)
				instance.OnRichEditControlChanged((RichEditControl)e.OldValue, (RichEditControl)e.NewValue);
		}
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		protected internal virtual void OnRichEditControlChanged(RichEditControl oldValue, RichEditControl newValue) {
			if (newValue != null)
				newValue.InnerControl.RaiseUpdateUI();
		}
		public RichEditControlProvider() {
			Visibility = Visibility.Collapsed;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
	#region RichEditControlAccessor
	public class RichEditControlAccessor {
		readonly RichEditControl control;
		public RichEditControlAccessor(RichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public RichEditControl RichEditControl { get { return control; } }
	}
	#endregion    
	#region RichEditControlBarCommandManager
	public class RichEditControlBarCommandManager : ControlBarCommandManager<RichEditControl, RichEditCommand, RichEditCommandId> {
		public RichEditControlBarCommandManager(RichEditControl control)
			: base(control) {
		}
		protected override object ControlAccessor { get { return Control.Accessor; } }
		protected override BarManager BarManager { get { return Control.BarManager; } }
		protected override RibbonControl Ribbon { get { return Control.Ribbon; } }
		protected override RichEditCommandId EmptyCommandId { get { return RichEditCommandId.None; } }
		protected override RichEditCommandId GetCommandId(ICommand command) {
			RichEditUICommand commandUi = command as RichEditUICommand;
			if (commandUi == null)
				return RichEditCommandId.None;
			return commandUi.CommandId;
		}
		protected override bool ShouldSetFocus(Command command) {
			return base.ShouldSetFocus(command) && !(command is IPlaceholderCommand);
		}
		protected override void SetFocus() {
			Control.Dispatcher.BeginInvoke(new Action(() => Control.SetFocus()));
		}
		protected override bool IsControlProvider(object value) {
			return value is RichEditControlProvider;
		}
		protected override BarItemCommandUIState CreateBarItemUIState(BarItem item) {
			return new RichEditBarItemCommandUIState(item);
		}
	}
	#endregion
}
