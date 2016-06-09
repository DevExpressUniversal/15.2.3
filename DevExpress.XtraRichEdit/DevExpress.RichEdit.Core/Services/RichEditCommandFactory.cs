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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Services;
namespace DevExpress.XtraRichEdit.Services {
	#region IRichEditCommandFactoryService
	[ComVisible(true)]
	public interface IRichEditCommandFactoryService {
		RichEditCommand CreateCommand(RichEditCommandId id);
	}
	#endregion
	#region RichEditCommandFactoryServiceWrapper
	public class RichEditCommandFactoryServiceWrapper : IRichEditCommandFactoryService {
		readonly IRichEditCommandFactoryService service;
		public RichEditCommandFactoryServiceWrapper(IRichEditCommandFactoryService service) {
			Guard.ArgumentNotNull(service, "service");
			this.service = service;
		}
		public IRichEditCommandFactoryService Service { get { return service; } }
		#region IRichEditCommandFactoryService Members
		public virtual RichEditCommand CreateCommand(RichEditCommandId id) {
			return Service.CreateCommand(id);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Services.Implementation {
	#region RichEditCommandConstructorTable
	public class RichEditCommandConstructorTable : Dictionary<RichEditCommandId, ConstructorInfo> {
	}
	#endregion
	#region RichEditCommandFactoryService (abstract class)
	[ComVisible(true)]
	public abstract class RichEditCommandFactoryService : IRichEditCommandFactoryService {
		#region Fields
		readonly IRichEditControl control;
		static readonly Type[] constructorParametersInterface = new Type[] { typeof(IRichEditControl) };
		readonly RichEditCommandConstructorTable commandConstructorTable;
		#endregion
		protected RichEditCommandFactoryService(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.commandConstructorTable = CreateCommandConstructorTable();
		}
		public IRichEditControl Control { get { return control; } }
		protected internal RichEditCommandConstructorTable CommandConstructorTable { get { return commandConstructorTable; } }
		protected internal RichEditCommandConstructorTable CreateCommandConstructorTable() {
			RichEditCommandConstructorTable result = new RichEditCommandConstructorTable();
			PopulateConstructorTable(result);
			return result;
		}
		protected void AddCommandConstructor(RichEditCommandConstructorTable table, RichEditCommandId commandId, Type commandType) {
			ConstructorInfo ci = GetConstructorInfo(commandType);
			if (ci == null)
				Exceptions.ThrowArgumentException("commandType", commandType);
			table.Add(commandId, ci);
		}
		protected internal virtual ConstructorInfo GetConstructorInfo(Type commandType) {
			return commandType.GetConstructor(constructorParametersInterface);
		}
		protected internal virtual void PopulateConstructorTable(RichEditCommandConstructorTable table) {
			AddCommandConstructor(table, RichEditCommandId.ToggleShowWhitespace, typeof(ToggleShowWhitespaceCommand));
			AddCommandConstructor(table, RichEditCommandId.PreviousCharacter, typeof(PreviousCharacterCommand));
			AddCommandConstructor(table, RichEditCommandId.NextCharacter, typeof(NextCharacterCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendPreviousCharacter, typeof(ExtendPreviousCharacterCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendNextCharacter, typeof(ExtendNextCharacterCommand));
			AddCommandConstructor(table, RichEditCommandId.PreviousWord, typeof(PreviousWordCommand));
			AddCommandConstructor(table, RichEditCommandId.NextWord, typeof(NextWordCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendPreviousWord, typeof(ExtendPreviousWordCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendNextWord, typeof(ExtendNextWordCommand));
			AddCommandConstructor(table, RichEditCommandId.PreviousLine, typeof(PreviousLineCommand));
			AddCommandConstructor(table, RichEditCommandId.NextLine, typeof(NextLineCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendPreviousLine, typeof(ExtendPreviousLineCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendNextLine, typeof(ExtendNextLineCommand));
			AddCommandConstructor(table, RichEditCommandId.PreviousParagraph, typeof(PreviousParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.NextParagraph, typeof(NextParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendPreviousParagraph, typeof(ExtendPreviousParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendNextParagraph, typeof(ExtendNextParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.PreviousPage, typeof(PreviousPageCommand));
			AddCommandConstructor(table, RichEditCommandId.NextPage, typeof(NextPageCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendPreviousPage, typeof(ExtendPreviousPageCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendNextPage, typeof(ExtendNextPageCommand));
			AddCommandConstructor(table, RichEditCommandId.PreviousScreen, typeof(PreviousScreenCommand));
			AddCommandConstructor(table, RichEditCommandId.NextScreen, typeof(NextScreenCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendPreviousScreen, typeof(ExtendPreviousScreenCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendNextScreen, typeof(ExtendNextScreenCommand));
			AddCommandConstructor(table, RichEditCommandId.StartOfLine, typeof(StartOfLineCommand));
			AddCommandConstructor(table, RichEditCommandId.EndOfLine, typeof(EndOfLineCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendStartOfLine, typeof(ExtendStartOfLineCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendEndOfLine, typeof(ExtendEndOfLineCommand));
			AddCommandConstructor(table, RichEditCommandId.StartOfDocument, typeof(StartOfDocumentCommand));
			AddCommandConstructor(table, RichEditCommandId.EndOfDocument, typeof(EndOfDocumentCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendStartOfDocument, typeof(ExtendStartOfDocumentCommand));
			AddCommandConstructor(table, RichEditCommandId.ExtendEndOfDocument, typeof(ExtendEndOfDocumentCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectAll, typeof(SelectAllCommand));
			AddCommandConstructor(table, RichEditCommandId.DeselectAll, typeof(DeselectAllCommand));
			AddCommandConstructor(table, RichEditCommandId.Undo, typeof(UndoCommand));
			AddCommandConstructor(table, RichEditCommandId.Redo, typeof(RedoCommand));
			AddCommandConstructor(table, RichEditCommandId.ClearUndo, typeof(ClearUndoCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleOvertype, typeof(ToggleOvertypeCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertParagraph, typeof(InsertParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertLineBreak, typeof(InsertLineBreakCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertPageBreak, typeof(InsertPageBreakCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertPageBreak2, typeof(InsertPageBreakCommand2));
			AddCommandConstructor(table, RichEditCommandId.InsertBreak, typeof(InsertBreakCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertSectionBreakNextPage, typeof(InsertSectionBreakNextPageCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertSectionBreakOddPage, typeof(InsertSectionBreakOddPageCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertSectionBreakEvenPage, typeof(InsertSectionBreakEvenPageCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertSectionBreakContinuous, typeof(InsertSectionBreakContinuousCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertNonBreakingSpace, typeof(InsertNonBreakingSpaceCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertColumnBreak, typeof(InsertColumnBreakCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertEnDash, typeof(InsertEnDashCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertEmDash, typeof(InsertEmDashCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertCopyrightSymbol, typeof(InsertCopyrightSymbolCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertRegisteredTrademarkSymbol, typeof(InsertRegisteredTrademarkSymbolCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTrademarkSymbol, typeof(InsertTrademarkSymbolCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertEllipsis, typeof(InsertEllipsisCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowSymbolForm, typeof(ShowSymbolFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFontBold, typeof(ToggleFontBoldCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFontItalic, typeof(ToggleFontItalicCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFontUnderline, typeof(ToggleFontUnderlineCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFontDoubleUnderline, typeof(ToggleFontDoubleUnderlineCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFontStrikeout, typeof(ToggleFontStrikeoutCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFontDoubleStrikeout, typeof(ToggleFontDoubleStrikeoutCommand));
			AddCommandConstructor(table, RichEditCommandId.IncreaseFontSize, typeof(IncreaseFontSizeCommand));
			AddCommandConstructor(table, RichEditCommandId.DecreaseFontSize, typeof(DecreaseFontSizeCommand));
			AddCommandConstructor(table, RichEditCommandId.IncrementFontSize, typeof(IncrementFontSizeCommand));
			AddCommandConstructor(table, RichEditCommandId.DecrementFontSize, typeof(DecrementFontSizeCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFontSuperscript, typeof(ToggleFontSuperscriptCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFontSubscript, typeof(ToggleFontSubscriptCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowFontForm, typeof(ShowFontFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowParagraphForm, typeof(ShowParagraphFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowBookmarkForm, typeof(ShowBookmarkFormCommand));
			AddCommandConstructor(table, RichEditCommandId.CreateBookmark, typeof(CreateBookmarkContextMenuItemCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowHyperlinkForm, typeof(ShowHyperlinkFormCommand));
			AddCommandConstructor(table, RichEditCommandId.CreateHyperlink, typeof(CreateHyperlinkContextMenuItemCommand));
			AddCommandConstructor(table, RichEditCommandId.EditHyperlink, typeof(EditHyperlinkCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowRangeEditingPermissionsForm, typeof(ShowRangeEditingPermissionsFormCommand));
			AddCommandConstructor(table, RichEditCommandId.CollapseOrExpandFormulaBar, typeof(CollapseOrExpandFormulaBarCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleParagraphAlignmentLeft, typeof(ToggleParagraphAlignmentLeftCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleParagraphAlignmentCenter, typeof(ToggleParagraphAlignmentCenterCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleParagraphAlignmentRight, typeof(ToggleParagraphAlignmentRightCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleParagraphAlignmentJustify, typeof(ToggleParagraphAlignmentJustifyCommand));
			AddCommandConstructor(table, RichEditCommandId.SetSingleParagraphSpacing, typeof(SetSingleParagraphSpacingCommand));
			AddCommandConstructor(table, RichEditCommandId.SetDoubleParagraphSpacing, typeof(SetDoubleParagraphSpacingCommand));
			AddCommandConstructor(table, RichEditCommandId.SetSesquialteralParagraphSpacing, typeof(SetSesquialteralParagraphSpacingCommand));
			AddCommandConstructor(table, RichEditCommandId.Delete, typeof(DeleteCommand));
			AddCommandConstructor(table, RichEditCommandId.BackSpaceKey, typeof(BackSpaceKeyCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteWord, typeof(DeleteWordCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteWordBack, typeof(DeleteWordBackCommand));
			AddCommandConstructor(table, RichEditCommandId.CopySelection, typeof(CopySelectionCommand));
			AddCommandConstructor(table, RichEditCommandId.PasteSelection, typeof(PasteSelectionCommand));
			AddCommandConstructor(table, RichEditCommandId.CutSelection, typeof(CutSelectionCommand));
			AddCommandConstructor(table, RichEditCommandId.IncrementNumerationFromParagraph, typeof(IncrementNumerationFromParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.DecrementNumerationFromParagraph, typeof(DecrementNumerationFromParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.Find, typeof(FindCommand));
			AddCommandConstructor(table, RichEditCommandId.Replace, typeof(ReplaceCommand));
			AddCommandConstructor(table, RichEditCommandId.FindForward, typeof(FindAndSelectForwardCommand));
			AddCommandConstructor(table, RichEditCommandId.FindBackward, typeof(FindAndSelectBackwardCommand));
			AddCommandConstructor(table, RichEditCommandId.ReplaceForward, typeof(ReplaceForwardCommand));
			AddCommandConstructor(table, RichEditCommandId.ReplaceAllForward, typeof(ReplaceAllForwardCommand));
			AddCommandConstructor(table, RichEditCommandId.TabKey, typeof(TabKeyCommand));
			AddCommandConstructor(table, RichEditCommandId.ShiftTabKey, typeof(ShiftTabKeyCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTab, typeof(InsertTabCommand));
			AddCommandConstructor(table, RichEditCommandId.FileNew, typeof(CreateEmptyDocumentCommand));
			AddCommandConstructor(table, RichEditCommandId.FileOpen, typeof(LoadDocumentCommand));
			AddCommandConstructor(table, RichEditCommandId.FileSaveAs, typeof(SaveDocumentAsCommand));
			AddCommandConstructor(table, RichEditCommandId.Print, typeof(PrintCommand));
			AddCommandConstructor(table, RichEditCommandId.PrintPreview, typeof(PrintPreviewCommand));
			AddCommandConstructor(table, RichEditCommandId.IncreaseIndent, typeof(IncrementIndentCommand));
			AddCommandConstructor(table, RichEditCommandId.DecreaseIndent, typeof(DecrementIndentCommand));
			AddCommandConstructor(table, RichEditCommandId.ZoomIn, typeof(ZoomInCommand));
			AddCommandConstructor(table, RichEditCommandId.ZoomOut, typeof(ZoomOutCommand));
			AddCommandConstructor(table, RichEditCommandId.FitToPage, typeof(FitToPageCommand));
			AddCommandConstructor(table, RichEditCommandId.FitWidth, typeof(FitWidthCommand));
			AddCommandConstructor(table, RichEditCommandId.FitHeight, typeof(FitHeightCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertPicture, typeof(InsertPictureCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertPictureInner, typeof(InsertPictureInnerCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleNumberingListItem, typeof(ToggleSimpleNumberingListCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleMultilevelListItem, typeof(ToggleMultiLevelListCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleBulletedListItem, typeof(ToggleBulletedListCommand));
			AddCommandConstructor(table, RichEditCommandId.ClearFormatting, typeof(ClearFormattingCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleHiddenText, typeof(ToggleHiddenTextCommand));
			AddCommandConstructor(table, RichEditCommandId.CreateField, typeof(CreateFieldCommand));
			AddCommandConstructor(table, RichEditCommandId.UpdateField, typeof(UpdateFieldCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFieldCodes, typeof(ToggleFieldCodesCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFieldLockedCommand, typeof(ToggleFieldLockedCommand));
			AddCommandConstructor(table, RichEditCommandId.LockFieldCommand, typeof(LockFieldCommand));
			AddCommandConstructor(table, RichEditCommandId.UnlockFieldCommand, typeof(UnlockFieldCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowInsertMergeFieldForm, typeof(ShowInsertMergeFieldFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleViewMergedData, typeof(ToggleViewMergedDataCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowAllFieldCodes, typeof(ShowAllFieldCodesCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowAllFieldResults, typeof(ShowAllFieldResultsCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeFontName, typeof(ChangeFontNameCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeDoubleFontSize, typeof(ChangeDoubleFontSizeCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeFontSize, typeof(ChangeFontSizeCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeFontStyle, typeof(ChangeFormattingCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowNumberingListForm, typeof(ShowNumberingListFormCommand));
			AddCommandConstructor(table, RichEditCommandId.SwitchToDraftView, typeof(SwitchToDraftViewCommand));
			AddCommandConstructor(table, RichEditCommandId.SwitchToPrintLayoutView, typeof(SwitchToPrintLayoutViewCommand));
			AddCommandConstructor(table, RichEditCommandId.SwitchToSimpleView, typeof(SwitchToSimpleViewCommand));
			AddCommandConstructor(table, RichEditCommandId.EnterKey, typeof(EnterKeyCommand));
			AddCommandConstructor(table, RichEditCommandId.EditPageHeader, typeof(EditPageHeaderCommand));
			AddCommandConstructor(table, RichEditCommandId.EditPageFooter, typeof(EditPageFooterCommand));
			AddCommandConstructor(table, RichEditCommandId.ClosePageHeaderFooter, typeof(ClosePageHeaderFooterCommand));
			AddCommandConstructor(table, RichEditCommandId.GoToPageHeader, typeof(GoToPageHeaderCommand));
			AddCommandConstructor(table, RichEditCommandId.GoToPageFooter, typeof(GoToPageFooterCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleHeaderFooterLinkToPrevious, typeof(ToggleHeaderFooterLinkToPreviousCommand));
			AddCommandConstructor(table, RichEditCommandId.GoToPreviousHeaderFooter, typeof(GoToPreviousPageHeaderFooterCommand));
			AddCommandConstructor(table, RichEditCommandId.GoToNextHeaderFooter, typeof(GoToNextPageHeaderFooterCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleDifferentFirstPage, typeof(ToggleDifferentFirstPageCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleDifferentOddAndEvenPages, typeof(ToggleDifferentOddAndEvenPagesCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeParagraphLineSpacing, typeof(ChangeParagraphLineSpacingCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowLineSpacingForm, typeof(ShowLineSpacingFormCommand));
			AddCommandConstructor(table, RichEditCommandId.AddSpacingBeforeParagraph, typeof(AddSpacingBeforeParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.AddSpacingAfterParagraph, typeof(AddSpacingAfterParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.RemoveSpacingBeforeParagraph, typeof(RemoveSpacingBeforeParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.RemoveSpacingAfterParagraph, typeof(RemoveSpacingAfterParagraphCommand));
			AddCommandConstructor(table, RichEditCommandId.SetPortraitPageOrientation, typeof(SetPortraitPageOrientationCommand));
			AddCommandConstructor(table, RichEditCommandId.SetLandscapePageOrientation, typeof(SetLandscapePageOrientationCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeSectionPageOrientation, typeof(ChangeSectionPageOrientationCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeSectionPageMargins, typeof(ChangeSectionPageMarginsCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeSectionPaperKind, typeof(ChangeSectionPaperKindCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeSectionPaperKindPlaceholder, typeof(ChangeSectionPaperKindPlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.SetNormalSectionPageMargins, typeof(SetNormalSectionPageMarginsCommand));
			AddCommandConstructor(table, RichEditCommandId.SetNarrowSectionPageMargins, typeof(SetNarrowSectionPageMarginsCommand));
			AddCommandConstructor(table, RichEditCommandId.SetModerateSectionPageMargins, typeof(SetModerateSectionPageMarginsCommand));
			AddCommandConstructor(table, RichEditCommandId.SetWideSectionPageMargins, typeof(SetWideSectionPageMarginsCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTable, typeof(InsertTableCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableInner, typeof(InsertTableInnerCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableRowBelow, typeof(InsertTableRowBelowCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableRowAbove, typeof(InsertTableRowAboveCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteTableRowsMenuItem, typeof(DeleteTableRowsMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.MergeTableElement, typeof(MergeTableElementMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.MergeTableCells, typeof(MergeTableCellsCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertPageNumberField, typeof(InsertPageNumberFieldCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertPageCountField, typeof(InsertPageCountFieldCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleSpellCheckAsYouType, typeof(ToggleSpellCheckAsYouTypeCommand));
			AddCommandConstructor(table, RichEditCommandId.CheckSpelling, typeof(CheckSpellingCommand));
			AddCommandConstructor(table, RichEditCommandId.NextDataRecord, typeof(NextDataRecordCommand));
			AddCommandConstructor(table, RichEditCommandId.PreviousDataRecord, typeof(PreviousDataRecordCommand));
			AddCommandConstructor(table, RichEditCommandId.LastDataRecord, typeof(LastDataRecordCommand));
			AddCommandConstructor(table, RichEditCommandId.FirstDataRecord, typeof(FirstDataRecordCommand));
			AddCommandConstructor(table, RichEditCommandId.MailMergeSaveDocumentAs, typeof(MailMergeSaveDocumentAsCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertMailMergeField, typeof(InsertMergeFieldCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertMailMergeFieldPlaceholder, typeof(InsertMergeFieldPlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.SplitTable, typeof(SplitTableCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableBordersPlaceholder, typeof(ChangeTableBordersPlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsAllBorders, typeof(ToggleTableCellsAllBordersCommand));
			AddCommandConstructor(table, RichEditCommandId.ResetTableCellsAllBorders, typeof(ResetTableCellsBordersCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsOutsideBorder, typeof(ToggleTableCellsOutsideBorderCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsInsideBorder, typeof(ToggleTableCellsInsideBorderCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsLeftBorder, typeof(ToggleTableCellsLeftBorderCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsRightBorder, typeof(ToggleTableCellsRightBorderCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsTopBorder, typeof(ToggleTableCellsTopBorderCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsBottomBorder, typeof(ToggleTableCellsBottomBorderCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsInsideHorizontalBorder, typeof(ToggleTableCellsInsideHorizontalBorderCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsInsideVerticalBorder, typeof(ToggleTableCellsInsideVerticalBorderCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableColumnToTheLeft, typeof(InsertTableColumnToTheLeftCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableColumnToTheRight, typeof(InsertTableColumnToTheRightCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeFontForeColor, typeof(ChangeFontColorCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeFontBackColor, typeof(ChangeFontBackColorCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableCellsBorderColor, typeof(ChangeCurrentBorderRepositoryItemColorCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableCellsBorderLineStyle, typeof(ChangeCurrentBorderRepositoryItemLineStyleCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableCellsBorderLineWeight, typeof(ChangeCurrentBorderRepositoryItemLineThicknessCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableCellsShading, typeof(ChangeTableCellsShadingCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteTable, typeof(DeleteTableCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowInsertTableCellsForm, typeof(ShowInsertTableCellsFormCommand));
			AddCommandConstructor(table, RichEditCommandId.Zoom, typeof(ZoomCommand));
			AddCommandConstructor(table, RichEditCommandId.GoToPage, typeof(GoToPageCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowDeleteTableCellsForm, typeof(ShowDeleteTableCellsFormCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteTableColumns, typeof(DeleteTableColumnsCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteTableRows, typeof(DeleteTableRowsCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteTablePlaceholder, typeof(DeleteTablePlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableCellsContentAlignmentPlaceholder, typeof(ChangeTableCellsContentAlignmentPlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsTopLeftAlignment, typeof(ToggleTableCellsTopLeftAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsTopCenterAlignment, typeof(ToggleTableCellsTopCenterAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsTopRightAlignment, typeof(ToggleTableCellsTopRightAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsMiddleLeftAlignment, typeof(ToggleTableCellsMiddleLeftAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsMiddleCenterAlignment, typeof(ToggleTableCellsMiddleCenterAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsMiddleRightAlignment, typeof(ToggleTableCellsMiddleRightAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsBottomLeftAlignment, typeof(ToggleTableCellsBottomLeftAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsBottomCenterAlignment, typeof(ToggleTableCellsBottomCenterAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableCellsBottomRightAlignment, typeof(ToggleTableCellsBottomRightAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.ZoomPercent, typeof(ZoomPercentCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowSplitTableCellsForm, typeof(ShowSplitTableCellsFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowSplitTableCellsFormMenuItem, typeof(ShowSplitTableCellsFormMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.SetSectionOneColumn, typeof(SetSectionOneColumnCommand));
			AddCommandConstructor(table, RichEditCommandId.SetSectionTwoColumns, typeof(SetSectionTwoColumnsCommand));
			AddCommandConstructor(table, RichEditCommandId.SetSectionThreeColumns, typeof(SetSectionThreeColumnsCommand));
			AddCommandConstructor(table, RichEditCommandId.SetSectionColumnsPlaceholder, typeof(SetSectionColumnsPlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectTableColumns, typeof(SelectTableColumnsCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectTableCell, typeof(SelectTableCellCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectTableRow, typeof(SelectTableRowCommand));
			AddCommandConstructor(table, RichEditCommandId.ProtectDocument, typeof(ProtectDocumentCommand));
			AddCommandConstructor(table, RichEditCommandId.UnprotectDocument, typeof(UnprotectDocumentCommand));
			AddCommandConstructor(table, RichEditCommandId.MakeTextUpperCase, typeof(MakeTextUpperCaseCommand));
			AddCommandConstructor(table, RichEditCommandId.MakeTextLowerCase, typeof(MakeTextLowerCaseCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTextCase, typeof(ToggleTextCaseCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTextCasePlaceholder, typeof(ChangeTextCasePlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectTable, typeof(SelectTableCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectTablePlaceholder, typeof(SelectTablePlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowDeleteTableCellsFormMenuItem, typeof(ShowDeleteTableCellsFormMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteTableColumnsMenuItem, typeof(DeleteTableColumnsMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleShowTableGridLines, typeof(ToggleShowTableGridLinesCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleShowHorizontalRuler, typeof(ToggleShowHorizontalRulerCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleShowVerticalRuler, typeof(ToggleShowVerticalRulerCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeSectionLineNumbering, typeof(ChangeSectionLineNumberingCommand));
			AddCommandConstructor(table, RichEditCommandId.SetSectionLineNumberingNone, typeof(SetSectionLineNumberingNoneCommand));
			AddCommandConstructor(table, RichEditCommandId.SetSectionLineNumberingContinuous, typeof(SetSectionLineNumberingContinuousCommand));
			AddCommandConstructor(table, RichEditCommandId.SetSectionLineNumberingRestartNewPage, typeof(SetSectionLineNumberingRestartNewPageCommand));
			AddCommandConstructor(table, RichEditCommandId.SetSectionLineNumberingRestartNewSection, typeof(SetSectionLineNumberingRestartNewSectionCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleParagraphSuppressLineNumbers, typeof(ToggleParagraphSuppressLineNumbersCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleParagraphSuppressHyphenation, typeof(ToggleParagraphSuppressHyphenationCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowLineNumberingForm, typeof(ShowLineNumberingFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowPageSetupForm, typeof(ShowPageSetupFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowColumnsSetupForm, typeof(ShowColumnsSetupFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowPasteSpecialForm, typeof(ShowPasteSpecialFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowTablePropertiesForm, typeof(ShowTablePropertiesFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowTablePropertiesFormMenuItem, typeof(ShowTablePropertiesFormMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.IncrementParagraphOutlineLevel, typeof(IncrementParagraphOutlineLevelCommand));
			AddCommandConstructor(table, RichEditCommandId.DecrementParagraphOutlineLevel, typeof(DecrementParagraphOutlineLevelCommand));
			AddCommandConstructor(table, RichEditCommandId.SetParagraphBodyTextLevel, typeof(SetParagraphBodyTextLevelCommand));
			AddCommandConstructor(table, RichEditCommandId.SetParagraphHeading1Level, typeof(SetParagraphHeading1LevelCommand));
			AddCommandConstructor(table, RichEditCommandId.SetParagraphHeading2Level, typeof(SetParagraphHeading2LevelCommand));
			AddCommandConstructor(table, RichEditCommandId.SetParagraphHeading3Level, typeof(SetParagraphHeading3LevelCommand));
			AddCommandConstructor(table, RichEditCommandId.SetParagraphHeading4Level, typeof(SetParagraphHeading4LevelCommand));
			AddCommandConstructor(table, RichEditCommandId.SetParagraphHeading5Level, typeof(SetParagraphHeading5LevelCommand));
			AddCommandConstructor(table, RichEditCommandId.SetParagraphHeading6Level, typeof(SetParagraphHeading6LevelCommand));
			AddCommandConstructor(table, RichEditCommandId.SetParagraphHeading7Level, typeof(SetParagraphHeading7LevelCommand));
			AddCommandConstructor(table, RichEditCommandId.SetParagraphHeading8Level, typeof(SetParagraphHeading8LevelCommand));
			AddCommandConstructor(table, RichEditCommandId.SetParagraphHeading9Level, typeof(SetParagraphHeading9LevelCommand));
			AddCommandConstructor(table, RichEditCommandId.AddParagraphsToTableOfContents, typeof(AddParagraphsToTableOfContentsCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableOfContents, typeof(InsertTableOfContentsCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableOfEquations, typeof(InsertTableOfEquationsCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableOfFigures, typeof(InsertTableOfFiguresCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableOfTables, typeof(InsertTableOfTablesCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableOfFiguresPlaceholder, typeof(InsertTableOfFiguresPlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertEquationsCaption, typeof(InsertEquationsCaptionCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertFiguresCaption, typeof(InsertFiguresCaptionCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTablesCaption, typeof(InsertTablesCaptionCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertCaptionPlaceholder, typeof(InsertCaptionPlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.UpdateTableOfContents, typeof(UpdateTableOfContentsCommand));
			AddCommandConstructor(table, RichEditCommandId.UpdateTableOfFigures, typeof(UpdateTableOfFiguresCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableAutoFitPlaceholder, typeof(ToggleTableAutoFitPlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableAutoFitContents, typeof(ToggleTableAutoFitContentsCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableAutoFitWindow, typeof(ToggleTableAutoFitWindowCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableFixedColumnWidth, typeof(ToggleTableFixedColumnWidthCommand));
			AddCommandConstructor(table, RichEditCommandId.UpdateFields, typeof(UpdateFieldsCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleTableAutoFitMenuPlaceholder, typeof(ToggleTableAutoFitPlaceholderMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectSquareTextWrapType, typeof(SetFloatingObjectSquareTextWrapTypeCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectBehindTextWrapType, typeof(SetFloatingObjectBehindTextWrapTypeCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectInFrontOfTextWrapType, typeof(SetFloatingObjectInFrontOfTextWrapTypeCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectThroughTextWrapType, typeof(SetFloatingObjectThroughTextWrapTypeCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectTightTextWrapType, typeof(SetFloatingObjectTightTextWrapTypeCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectTopAndBottomTextWrapType, typeof(SetFloatingObjectTopAndBottomTextWrapTypeCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectTopLeftAlignment, typeof(SetFloatingObjectTopLeftAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectTopCenterAlignment, typeof(SetFloatingObjectTopCenterAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectTopRightAlignment, typeof(SetFloatingObjectTopRightAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectMiddleLeftAlignment, typeof(SetFloatingObjectMiddleLeftAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectMiddleCenterAlignment, typeof(SetFloatingObjectMiddleCenterAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectMiddleRightAlignment, typeof(SetFloatingObjectMiddleRightAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectBottomLeftAlignment, typeof(SetFloatingObjectBottomLeftAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectBottomCenterAlignment, typeof(SetFloatingObjectBottomCenterAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.SetFloatingObjectBottomRightAlignment, typeof(SetFloatingObjectBottomRightAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeFloatingObjectTextWrapType, typeof(ChangeFloatingObjectTextWrapTypeCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeFloatingObjectAlignment, typeof(ChangeFloatingObjectAlignmentCommand));
			AddCommandConstructor(table, RichEditCommandId.FloatingObjectBringForwardPlaceholder, typeof(FloatingObjectBringForwardPlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.FloatingObjectBringForward, typeof(FloatingObjectBringForwardCommand));
			AddCommandConstructor(table, RichEditCommandId.FloatingObjectBringToFront, typeof(FloatingObjectBringToFrontCommand));
			AddCommandConstructor(table, RichEditCommandId.FloatingObjectBringInFrontOfText, typeof(FloatingObjectBringInFrontOfTextCommand));
			AddCommandConstructor(table, RichEditCommandId.FloatingObjectSendBackward, typeof(FloatingObjectSendBackwardCommand));
			AddCommandConstructor(table, RichEditCommandId.FloatingObjectSendToBack, typeof(FloatingObjectSendToBackCommand));
			AddCommandConstructor(table, RichEditCommandId.FloatingObjectSendBehindText, typeof(FloatingObjectSendBehindTextCommand));
			AddCommandConstructor(table, RichEditCommandId.FloatingObjectSendBackwardPlaceholder, typeof(FloatingObjectSendBackwardPlaceholderCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectUpperLevelObject, typeof(SelectUpperLevelObjectCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeFloatingObjectFillColor, typeof(ChangeFloatingObjectFillColorCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeFloatingObjectOutlineColor, typeof(ChangeFloatingObjectOutlineColorCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeFloatingObjectOutlineWeight, typeof(ChangeFloatingObjectOutlineWidthCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowPageMarginsSetupForm, typeof(ShowPageMarginsSetupFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowPagePaperSetupForm, typeof(ShowPagePaperSetupFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowEditStyleForm, typeof(ShowEditStyleFormCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTextBox, typeof(InsertTextBoxCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertFloatingPicture, typeof(InsertFloatingObjectPictureCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeParagraphBackColor, typeof(ChangeParagraphBackColorCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowFloatingObjectLayoutOptionsForm, typeof(ShowFloatingObjectLayoutOptionsFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowTableOptionsForm, typeof(ShowTableOptionsFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangePageColor, typeof(ChangePageColorCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableStyle, typeof(ChangeTableStyleCommand));
			AddCommandConstructor(table, RichEditCommandId.CapitalizeEachWordTextCase, typeof(CapitalizeEachWordCaseCommand));
			AddCommandConstructor(table, RichEditCommandId.InnerReplace, typeof(ReplaceInnerCommand));
			AddCommandConstructor(table, RichEditCommandId.ViewComments, typeof(ViewCommentsCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowReviewingPane, typeof(ShowReviewingPaneCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleReviewingPane, typeof(ToggleReviewingPaneCommand));
			AddCommandConstructor(table, RichEditCommandId.Reviewers, typeof(ReviewersCommand));
			AddCommandConstructor(table, RichEditCommandId.NewComment, typeof(NewCommentCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteOneComment, typeof(DeleteOneCommentCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteAllCommentsShown, typeof(DeleteAllCommentsShownCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteAllComments, typeof(DeleteAllCommentsCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteCommentsPlaceholder, typeof(DeleteCommentCommand));
			AddCommandConstructor(table, RichEditCommandId.PreviousComment, typeof(PreviousCommentCommand));
			AddCommandConstructor(table, RichEditCommandId.NextComment, typeof(NextCommentCommand));
			AddCommandConstructor(table, RichEditCommandId.OvertypeText, typeof(OvertypeTextCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertText, typeof(InsertTextCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertSymbol, typeof(InsertSymbolCommand));
			AddCommandConstructor(table, RichEditCommandId.OpenHyperlink, typeof(OpenHyperlinkCommand));
			AddCommandConstructor(table, RichEditCommandId.RemoveHyperlink, typeof(RemoveHyperlinkFieldCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFirstRow, typeof(ToggleFirstRowCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleLastRow, typeof(ToggleLastRowCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleBandedRows, typeof(ToggleBandedRowsCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFirstColumn, typeof(ToggleFirstColumnCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleLastColumn, typeof(ToggleLastColumnCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleBandedColumns, typeof(ToggleBandedColumnsCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteRepeatedWord, typeof(DeleteRepeatedWordCommand));
			AddCommandConstructor(table, RichEditCommandId.IgnoreMisspelling, typeof(IgnoreMisspellingCommand));
			AddCommandConstructor(table, RichEditCommandId.IgnoreAllMisspellings, typeof(IgnoreAllMisspellingsCommand));
			AddCommandConstructor(table, RichEditCommandId.AddWordToDictionary, typeof(AddWordToDictionaryCommand));
			AddCommandConstructor(table, RichEditCommandId.ReplaceMisspelling, typeof(ReplaceMisspellingCommand));
			AddCommandConstructor(table, RichEditCommandId.ToolsFloatingPictureCommandGroup, typeof(ToolsFloatingPictureCommandGroup));
			AddCommandConstructor(table, RichEditCommandId.ToolsTableCommandGroup, typeof(ToolsTableCommandGroup));
			AddCommandConstructor(table, RichEditCommandId.ToolsHeaderFooterCommandGroup, typeof(ToolsHeaderFooterCommandGroup));
			AddCommandConstructor(table, RichEditCommandId.ChangeCase, typeof(ChangeCaseCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteNonEmptySelection, typeof(DeleteNonEmptySelectionCommand));
			AddCommandConstructor(table, RichEditCommandId.ImeUndo, typeof(ImeUndoCommand));
			AddCommandConstructor(table, RichEditCommandId.ResetMerging, typeof(ResetMergingCommand));
			AddCommandConstructor(table, RichEditCommandId.ResizeInlinePicture, typeof(ResizeInlinePictureCommand));
			AddCommandConstructor(table, RichEditCommandId.RotateInlinePicture, typeof(RotateInlinePictureCommand));
#if!SL
			AddCommandConstructor(table, RichEditCommandId.ShowTOCForm , typeof(ShowTOCFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeLanguage, typeof(ChangeLanguageCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowLanguageForm, typeof(ShowLanguageFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeNoProof, typeof(ChangeNoProofCommand));
#endif
		}
		#region ICommandFactoryService Members
		public virtual RichEditCommand CreateCommand(RichEditCommandId commandId) {
			RichEditCommand command = CreateCommandCore(commandId);
			if (command == null)
				Exceptions.ThrowArgumentException("commandId", commandId);
			return command;
		}
		protected internal virtual RichEditCommand CreateCommandCore(RichEditCommandId commandId) {
			ConstructorInfo ci;
			if (commandConstructorTable.TryGetValue(commandId, out ci))
				return (RichEditCommand)ci.Invoke(new object[] { Control });
			else
				return null;
		}
		#endregion
	}
	#endregion
}
