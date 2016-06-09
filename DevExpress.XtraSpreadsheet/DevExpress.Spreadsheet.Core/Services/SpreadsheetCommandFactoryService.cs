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
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Services {
	#region ISpreadsheetCommandFactoryService
	[ComVisible(true)]
	public interface ISpreadsheetCommandFactoryService {
		SpreadsheetCommand CreateCommand(SpreadsheetCommandId id);
	}
	#endregion
	#region SpreadsheetCommandFactoryServiceWrapper
	public class SpreadsheetCommandFactoryServiceWrapper : ISpreadsheetCommandFactoryService {
		readonly ISpreadsheetCommandFactoryService service;
		public SpreadsheetCommandFactoryServiceWrapper(ISpreadsheetCommandFactoryService service) {
			Guard.ArgumentNotNull(service, "service");
			this.service = service;
		}
		public ISpreadsheetCommandFactoryService Service { get { return service; } }
		#region ISpreadsheetCommandFactoryService Members
		public virtual SpreadsheetCommand CreateCommand(SpreadsheetCommandId id) {
			return Service.CreateCommand(id);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	#region SpreadsheetCommandConstructorTable
	public class SpreadsheetCommandConstructorTable : Dictionary<SpreadsheetCommandId, ConstructorInfo> {
	}
	#endregion
	#region SpreadsheetCommandFactoryService (abstract class)
	[ComVisible(true)]
	public abstract class SpreadsheetCommandFactoryService : ISpreadsheetCommandFactoryService {
		#region Fields
		readonly ISpreadsheetControl control;
		static readonly Type[] constructorParametersInterface = new Type[] { typeof(ISpreadsheetControl) };
		readonly SpreadsheetCommandConstructorTable commandConstructorTable;
		#endregion
		protected SpreadsheetCommandFactoryService(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.commandConstructorTable = CreateCommandConstructorTable();
		}
		public ISpreadsheetControl Control { get { return control; } }
		protected internal SpreadsheetCommandConstructorTable CreateCommandConstructorTable() {
			SpreadsheetCommandConstructorTable result = new SpreadsheetCommandConstructorTable();
			PopulateConstructorTable(result);
			return result;
		}
		protected void AddCommandConstructor(SpreadsheetCommandConstructorTable table, SpreadsheetCommandId commandId, Type commandType) {
			ConstructorInfo ci = GetConstructorInfo(commandType);
			if (ci == null)
				Exceptions.ThrowArgumentException("commandType", commandType);
			table.Add(commandId, ci);
		}
		protected internal virtual ConstructorInfo GetConstructorInfo(Type commandType) {
			return commandType.GetConstructor(constructorParametersInterface);
		}
		protected internal virtual void PopulateConstructorTable(SpreadsheetCommandConstructorTable table) {
			PopulateFileCommandsTable(table);
			PopulateInplaceEditCommandsTable(table);
			PopulateFormatCommandsTable(table);
			PopulateSearchReplaceCommandsTable(table);
			PopulateSelectionCommandsTable(table);
			PopulatePageLayoutCommandsTable(table);
			PopulateFormulasCommandsTable(table);
			PopulateViewCommandsTable(table);
			PopulateInsertCommandsTable(table);
			PopulateClipboardCommandsTable(table);
			PopulatePictureCommandTable(table);
			PopulateHyperlinkCommandTable(table);
			PopulateRemoveCommandsTable(table);
			PopulateHideAndUnhideCommandsTable(table);
			PopulateConditionalFormattingCommandsTable(table);
			PopulateDataCommandsTable(table);
			PopulateReviewCommandsTable(table);
			PopulateAdditionalCommandsTable(table);
			PopulateCollapseOrExpandFormulaBarCommandTable(table);
			PopulateChartDesignCommandTable(table);
			PopulateChartLayoutCommandTable(table);
			PopulateChartItemsTitleCommandsTable(table);
			PopulateTableToolsDesignCommandTable(table);
			PopulateMailMergeDesignCommandTable(table);
			PopulateArrangeCommandTable(table);
			PopulateToolsCommandTable(table);
			PopulateGroupDesignCommandTable(table);
			PopulateCommentEditorCommandsTable(table);
			PopulateDataValidationEditorCommandsTable(table);
			PopulatePivotTableAnalyzeCommandTable(table);
		}
		#region PopulateFileCommandsTable
		protected internal virtual void PopulateFileCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.FileOpen, typeof(OpenDocumentCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FileOpenSilently, typeof(OpenDocumentSilentlyCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FileSave, typeof(SaveDocumentCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FileSaveAs, typeof(SaveDocumentAsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FileSaveAsSilently, typeof(SaveDocumentAsSilentlyCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FileNew, typeof(CreateEmptyDocumentCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FilePrint, typeof(PrintCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FileQuickPrint, typeof(QuickPrintCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FilePrintPreview, typeof(PrintPreviewCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FileUndo, typeof(UndoCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FileRedo, typeof(RedoCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FileShowDocumentProperties, typeof(ShowDocumentPropertiesCommand));
		}
		#endregion
		#region PopulateEditCommandsTable
		protected internal virtual void PopulateInplaceEditCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.InplaceBeginEdit, typeof(InplaceBeginEditCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InplaceEndEdit, typeof(InplaceEndEditCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InplaceEndEditEnterToMultipleCells, typeof(InplaceEndEditEnterToMultipleCellsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InplaceEndEditEnterArrayFormula, typeof(InplaceEndEditEnterArrayFormulaCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InplaceCancelEdit, typeof(InplaceCancelEditCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InplaceToggleEditMode, typeof(InplaceToggleEditModeCommand));
		}
		#endregion
		#region PopulateFormatCommandsTable
		protected internal virtual void PopulateFormatCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.FormatFontBold, typeof(FormatFontBoldCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatFontItalic, typeof(FormatFontItalicCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatFontUnderline, typeof(FormatFontUnderlineCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatFontStrikeout, typeof(FormatFontStrikeoutCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatFontName, typeof(FormatFontNameCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatFontSize, typeof(FormatFontSizeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatIncreaseFontSize, typeof(FormatIncreaseFontSizeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatDecreaseFontSize, typeof(FormatDecreaseFontSizeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatFontColor, typeof(FormatFontColorCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatFillColor, typeof(FormatFillColorCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAlignmentLeft, typeof(FormatAlignmentLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAlignmentCenter, typeof(FormatAlignmentCenterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAlignmentRight, typeof(FormatAlignmentRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAlignmentTop, typeof(FormatAlignmentTopCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAlignmentMiddle, typeof(FormatAlignmentMiddleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAlignmentBottom, typeof(FormatAlignmentBottomCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatWrapText, typeof(FormatWrapTextCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatIncreaseIndent, typeof(FormatIncreaseIndentCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatDecreaseIndent, typeof(FormatDecreaseIndentCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatBorderLineColor, typeof(FormatBorderLineColorCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatBorderLineStyle, typeof(FormatBorderLineStyleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatThickBorder, typeof(FormatThickBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatOutsideBorders, typeof(FormatOutsideBordersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatLeftBorder, typeof(FormatLeftBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatRightBorder, typeof(FormatRightBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatTopBorder, typeof(FormatTopBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatBottomBorder, typeof(FormatBottomBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatBottomDoubleBorder, typeof(FormatBottomDoubleBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatBottomThickBorder, typeof(FormatBottomThickBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatTopAndBottomBorder, typeof(FormatTopAndBottomBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatTopAndThickBottomBorder, typeof(FormatTopAndThickBottomBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatTopAndDoubleBottomBorder, typeof(FormatTopAndDoubleBottomBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAllBorders, typeof(FormatAllBordersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNoBorders, typeof(FormatNoBordersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatBordersCommandGroup, typeof(FormatBordersCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatCellLocked, typeof(FormatCellLockedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumber, typeof(FormatNumberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberGeneral, typeof(FormatNumberGeneralCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberDecimal, typeof(FormatNumberDecimalCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberPercent, typeof(FormatNumberPercentCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberPercentage, typeof(FormatNumberPercentageCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberScientific, typeof(FormatNumberScientificCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberFraction, typeof(FormatNumberFractionCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberAccounting, typeof(FormatNumberCommaStyleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberAccountingCurrency, typeof(FormatNumberAccountingCurrencyCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberShortDate, typeof(FormatNumberShortDateCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberLongDate, typeof(FormatNumberLongDateCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberText, typeof(FormatNumberTextCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberTime, typeof(FormatNumberTimeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberAccountingRegular, typeof(FormatNumberAccountingCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberPredefined4, typeof(FormatNumberPredefined4));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberPredefined8, typeof(FormatNumberPredefined8));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberPredefined15, typeof(FormatNumberPredefined15));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberPredefined18, typeof(FormatNumberPredefined18));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberAccountingUS, typeof(FormatNumberAccountingUSCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberAccountingUK, typeof(FormatNumberAccountingUKCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberAccountingPRC, typeof(FormatNumberAccountingPRCCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberAccountingEuro, typeof(FormatNumberAccountingEuroCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberAccountingSwiss, typeof(FormatNumberAccountingSwissCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberAccountingCommandGroup, typeof(FormatNumberAccountingCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberIncreaseDecimal, typeof(FormatNumberIncreaseDecimalCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatNumberDecreaseDecimal, typeof(FormatNumberDecreaseDecimalCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatCellStyle, typeof(FormatCellStyleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAsTable, typeof(FormatAsTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingMergeCellsCommandGroup, typeof(EditingMergeCellsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingMergeAndCenterCells, typeof(EditingMergeAndCenterCellsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingMergeCellsAcross, typeof(EditingMergeCellsAcrossCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingMergeCells, typeof(EditingMergeCellsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingUnmergeCells, typeof(EditingUnmergeCellsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingAutoSumCommandGroup, typeof(EditingAutoSumCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingFillCommandGroup, typeof(EditingFillCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingFillDown, typeof(EditingFillDownCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingFillUp, typeof(EditingFillUpCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingFillRight, typeof(EditingFillRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingFillLeft, typeof(EditingFillLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatClearCommandGroup, typeof(FormatClearCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatClearAll, typeof(FormatClearAllCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatClearFormats, typeof(FormatClearFormatsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatClearContentsContextMenuItem, typeof(FormatClearContentsContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatClearContents, typeof(FormatClearContentsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatClearComments, typeof(FormatClearCommentsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatClearHyperlinks, typeof(FormatClearHyperlinksCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatRemoveHyperlinks, typeof(FormatRemoveHyperlinksCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatCommandGroup, typeof(FormatCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAutoFitRowHeight, typeof(FormatAutoFitRowHeightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAutoFitRowHeightUsingMouse, typeof(FormatAutoFitRowHeightUsingMouseCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAutoFitColumnWidth, typeof(FormatAutoFitColumnWidthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatAutoFitColumnWidthUsingMouse, typeof(FormatAutoFitColumnWidthUsingMouseCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatTabColor, typeof(FormatTabColorCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RenameSheet, typeof(RenameSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RenameSheetContextMenuItem, typeof(RenameSheetContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatRowHeight, typeof(ShowRowHeightFormCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatColumnWidth, typeof(ShowColumnWidthFormCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatDefaultColumnWidth, typeof(ShowDefaultColumnWidthFormCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatRowHeightContextMenuItem, typeof(ShowRowHeightFormContextMenuCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatColumnWidthContextMenuItem, typeof(ShowColumnWidthFormContextMenuCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MoveOrCopySheet, typeof(MoveOrCopySheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MoveOrCopySheetContextMenuItem, typeof(MoveOrCopySheetContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatCellsNumber, typeof(FormatCellsNumberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatCellsAlignment, typeof(FormatCellsAlignmentCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatCellsFont, typeof(FormatCellsFontCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatCellsBorder, typeof(FormatCellsBorderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatCellsFill, typeof(FormatCellsFillCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatCellsProtection, typeof(FormatCellsProtectionCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormatCellsContextMenuItem, typeof(FormatCellsContextMenuItemCommand));
		}
		#endregion
		#region PopulateSearchReplaceCommandsTable
		protected internal virtual void PopulateSearchReplaceCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.EditingSortAndFilterCommandGroup, typeof(EditingSortAndFilterCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingFindAndSelectCommandGroup, typeof(EditingFindAndSelectCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingFind, typeof(FindCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingReplace, typeof(ReplaceCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingSelectFormulas, typeof(EditingSelectFormulasCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingSelectComments, typeof(EditingSelectCommentsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingSelectConditionalFormatting, typeof(EditingSelectConditionalFormattingCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingSelectConstants, typeof(EditingSelectConstantsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingSelectDataValidation, typeof(EditingSelectDataValidationCommand));
		}
		#endregion
		#region PopulateSelectionCommandsTable
		protected internal virtual void PopulateSelectionCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveLeft, typeof(SelectionMoveLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveRight, typeof(SelectionMoveRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveUp, typeof(SelectionMoveUpCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveDown, typeof(SelectionMoveDownCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveToLeftColumn, typeof(SelectionMoveToLeftColumnCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveLeftToDataEdge, typeof(SelectionMoveLeftToDataEdgeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveRightToDataEdge, typeof(SelectionMoveRightToDataEdgeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveUpToDataEdge, typeof(SelectionMoveUpToDataEdgeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveDownToDataEdge, typeof(SelectionMoveDownToDataEdgeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveToTopLeftCell, typeof(SelectionMoveToTopLeftCellCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveToLastUsedCell, typeof(SelectionMoveToLastUsedCellCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveActiveCellOnEnterPress, typeof(SelectionMoveActiveCellOnEnterPressCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveActiveCellOnEnterPressReverse, typeof(SelectionMoveActiveCellOnEnterPressReverseCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveActiveCellRight, typeof(SelectionMoveActiveCellRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveActiveCellLeft, typeof(SelectionMoveActiveCellLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveActiveCellDown, typeof(SelectionMoveActiveCellDownCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveActiveCellUp, typeof(SelectionMoveActiveCellUpCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMoveActiveCellToNextCorner, typeof(SelectionMoveActiveCellToNextCornerCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionNextRange, typeof(SelectionNextRangeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionPreviousRange, typeof(SelectionPreviousRangeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectActiveCell, typeof(SelectActiveCellCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectActiveColumn, typeof(SelectActiveColumnCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectActiveRow, typeof(SelectActiveRowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectAll, typeof(SelectAllCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandLeft, typeof(SelectionExpandLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandRight, typeof(SelectionExpandRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandUp, typeof(SelectionExpandUpCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandDown, typeof(SelectionExpandDownCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandToLeftColumn, typeof(SelectionExpandToLeftColumnCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandLeftToDataEdge, typeof(SelectionExpandLeftToDataEdgeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandRightToDataEdge, typeof(SelectionExpandRightToDataEdgeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandUpToDataEdge, typeof(SelectionExpandUpToDataEdgeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandDownToDataEdge, typeof(SelectionExpandDownToDataEdgeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandToTopLeftCell, typeof(SelectionExpandToTopLeftCellCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandToLastUsedCell, typeof(SelectionExpandToLastUsedCellCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMovePageDown, typeof(SelectionMovePageDownCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionMovePageUp, typeof(SelectionMovePageUpCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandPageDown, typeof(SelectionExpandPageDownCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectionExpandPageUp, typeof(SelectionExpandPageUpCommand));
		}
		#endregion
		#region PopulatePageLayoutCommandsTable
		protected internal virtual void PopulatePageLayoutCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupMarginsNormal, typeof(PageSetupMarginsNormalCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupMarginsNarrow, typeof(PageSetupMarginsNarrowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupMarginsWide, typeof(PageSetupMarginsWideCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupMarginsCommandGroup, typeof(PageSetupMarginsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupOrientationPortrait, typeof(PageSetupOrientationPortraitCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupOrientationLandscape, typeof(PageSetupOrientationLandscapeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupOrientationCommandGroup, typeof(PageSetupOrientationCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupSetPaperKind, typeof(PageSetupSetPaperKindCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupPaperKindCommandGroup, typeof(PageSetupPaperKindCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupSetPrintArea, typeof(PageSetupSetPrintAreaCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupClearPrintArea, typeof(PageSetupClearPrintAreaCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupAddPrintArea, typeof(PageSetupAddPrintAreaCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupPrintAreaCommandGroup, typeof(PageSetupPrintAreaCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupPrintGridlines, typeof(PageSetupPrintGridlinesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupPrintHeadings, typeof(PageSetupPrintHeadingsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetup, typeof(ShowPageSetupCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupMargins, typeof(ShowPageSetupMarginsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupHeaderFooter, typeof(ShowPageSetupHeaderFooterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupSheet, typeof(ShowPageSetupSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupCustomMargins, typeof(ShowPageSetupCustomMarginsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PageSetupMorePaperSizes, typeof(ShowPageSetupMorePaperSizesCommand));
		}
		#endregion
		#region PopulateFormulasCommandsTable
		protected internal virtual void PopulateFormulasCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsAutoSumCommandGroup, typeof(FunctionsAutoSumCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsInsertSpecificFunction, typeof(FunctionsInsertSpecificFunctionCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsInsertSum, typeof(FunctionsInsertSumCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsInsertAverage, typeof(FunctionsInsertAverageCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsInsertCountNumbers, typeof(FunctionsInsertCountNumbersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsInsertMax, typeof(FunctionsInsertMaxCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsInsertMin, typeof(FunctionsInsertMinCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsFinancialCommandGroup, typeof(FunctionsFinancialCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsLogicalCommandGroup, typeof(FunctionsLogicalCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsTextCommandGroup, typeof(FunctionsTextCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsDateAndTimeCommandGroup, typeof(FunctionsDateAndTimeCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsLookupAndReferenceCommandGroup, typeof(FunctionsLookupAndReferenceCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsMathAndTrigonometryCommandGroup, typeof(FunctionsMathAndTrigonometryCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsMoreCommandGroup, typeof(FunctionsMoreCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsStatisticalCommandGroup, typeof(FunctionsStatisticalCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsEngineeringCommandGroup, typeof(FunctionsEngineeringCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsCubeCommandGroup, typeof(FunctionsCubeCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsInformationCommandGroup, typeof(FunctionsInformationCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsCompatibilityCommandGroup, typeof(FunctionsCompatibilityCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FunctionsWebCommandGroup, typeof(FunctionsWebCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasDefineNameCommandGroup, typeof(DefineNameCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasDefineNameCommand, typeof(DefineNameCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasShowNameManager, typeof(ShowNameManagerCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasCreateDefinedNamesFromSelection, typeof(CreateDefinedNamesFromSelectionCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasCalculateFull, typeof(CalculateFullCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasCalculateFullRebuild, typeof(CalculateFullRebuildCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasCalculateNow, typeof(CalculateNowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasCalculateSheet, typeof(CalculateSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasCalculationOptionsCommandGroup, typeof(CalculationOptionsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasCalculationModeAutomatic, typeof(CalculationModeAutomaticCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasCalculationModeManual, typeof(CalculationModeManualCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasInsertDefinedName, typeof(InsertDefinedNameCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FormulasInsertDefinedNameCommandGroup, typeof(InsertDefinedNameCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertFunction, typeof(InsertFunctionCommand));
		}
		#endregion
		#region PopulateViewCommandsTable
		protected internal virtual void PopulateViewCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ViewZoomIn, typeof(ViewZoomInCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ViewZoomOut, typeof(ViewZoomOutCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ViewZoom100Percent, typeof(ViewZoom100PercentCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ViewShowGridlines, typeof(ViewShowGridlinesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ViewShowHeadings, typeof(ViewShowHeadingsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ViewShowFormulas, typeof(ViewShowFormulasCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ViewFreezePanesCommandGroup, typeof(ViewFreezePanesCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ViewFreezePanes, typeof(ViewFreezePanesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ViewUnfreezePanes, typeof(ViewUnfreezePanesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ViewFreezeTopRow, typeof(ViewFreezeTopRowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ViewFreezeFirstColumn, typeof(ViewFreezeFirstColumnCommand));
		}
		#endregion
		#region PopulateInsertCommandsTable
		protected internal virtual void PopulateInsertCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.InsertCellsCommandGroup, typeof(InsertCellsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertSheet, typeof(InsertSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertSheetContextMenuItem, typeof(InsertSheetContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertSheetRows, typeof(InsertSheetRowsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertSheetRowsContextMenuItem, typeof(InsertSheetRowsContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertSheetColumns, typeof(InsertSheetColumnsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertSheetColumnsContextMenuItem, typeof(InsertSheetColumnsContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertHyperlink, typeof(InsertHyperlinkUICommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertTable, typeof(InsertTableUICommand));
			AddCommandConstructor(table, SpreadsheetCommandId.AddNewWorksheet, typeof(AddNewWorksheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartColumnCommandGroup, typeof(InsertChartColumnCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBarCommandGroup, typeof(InsertChartBarCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPieCommandGroup, typeof(InsertChartPieCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartLineCommandGroup, typeof(InsertChartLineCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartAreaCommandGroup, typeof(InsertChartAreaCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartScatterCommandGroup, typeof(InsertChartScatterCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartOtherCommandGroup, typeof(InsertChartOtherCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartColumn2DCommandGroup, typeof(InsertChartColumn2DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartColumn3DCommandGroup, typeof(InsertChartColumn3DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartCylinderCommandGroup, typeof(InsertChartCylinderCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartConeCommandGroup, typeof(InsertChartConeCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPyramidCommandGroup, typeof(InsertChartPyramidCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBar2DCommandGroup, typeof(InsertChartBar2DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBar3DCommandGroup, typeof(InsertChartBar3DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalCylinderCommandGroup, typeof(InsertChartHorizontalCylinderCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalConeCommandGroup, typeof(InsertChartHorizontalConeCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalPyramidCommandGroup, typeof(InsertChartHorizontalPyramidCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPie2DCommandGroup, typeof(InsertChartPie2DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPie3DCommandGroup, typeof(InsertChartPie3DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartDoughnut2DCommandGroup, typeof(InsertChartDoughnut2DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartLine2DCommandGroup, typeof(InsertChartLine2DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartLine3DCommandGroup, typeof(InsertChartLine3DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartArea2DCommandGroup, typeof(InsertChartArea2DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartArea3DCommandGroup, typeof(InsertChartArea3DCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBubbleCommandGroup, typeof(InsertChartBubbleCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartStockCommandGroup, typeof(InsertChartStockCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartRadarCommandGroup, typeof(InsertChartRadarCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartColumnClustered2D, typeof(InsertChartColumnClustered2DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartColumnStacked2D, typeof(InsertChartColumnStacked2DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartColumnPercentStacked2D, typeof(InsertChartColumnPercentStacked2DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartColumnClustered3D, typeof(InsertChartColumnClustered3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartColumnStacked3D, typeof(InsertChartColumnStacked3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartColumnPercentStacked3D, typeof(InsertChartColumnPercentStacked3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartCylinderClustered, typeof(InsertChartCylinderClusteredCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartCylinderStacked, typeof(InsertChartCylinderStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartCylinderPercentStacked, typeof(InsertChartCylinderPercentStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartConeClustered, typeof(InsertChartConeClusteredCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartConeStacked, typeof(InsertChartConeStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartConePercentStacked, typeof(InsertChartConePercentStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPyramidClustered, typeof(InsertChartPyramidClusteredCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPyramidStacked, typeof(InsertChartPyramidStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPyramidPercentStacked, typeof(InsertChartPyramidPercentStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBarClustered2D, typeof(InsertChartBarClustered2DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBarStacked2D, typeof(InsertChartBarStacked2DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBarPercentStacked2D, typeof(InsertChartBarPercentStacked2DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBarClustered3D, typeof(InsertChartBarClustered3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBarStacked3D, typeof(InsertChartBarStacked3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBarPercentStacked3D, typeof(InsertChartBarPercentStacked3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalCylinderClustered, typeof(InsertChartHorizontalCylinderClusteredCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalCylinderStacked, typeof(InsertChartHorizontalCylinderStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalCylinderPercentStacked, typeof(InsertChartHorizontalCylinderPercentStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalConeClustered, typeof(InsertChartHorizontalConeClusteredCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalConeStacked, typeof(InsertChartHorizontalConeStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalConePercentStacked, typeof(InsertChartHorizontalConePercentStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalPyramidClustered, typeof(InsertChartHorizontalPyramidClusteredCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalPyramidStacked, typeof(InsertChartHorizontalPyramidStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartHorizontalPyramidPercentStacked, typeof(InsertChartHorizontalPyramidPercentStackedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartColumn3D, typeof(InsertChartColumn3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartCylinder, typeof(InsertChartCylinderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartCone, typeof(InsertChartConeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPyramid, typeof(InsertChartPyramidCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPie2D, typeof(InsertChartPie2DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPie3D, typeof(InsertChartPie3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPieExploded2D, typeof(InsertChartPieExploded2DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPieExploded3D, typeof(InsertChartPieExploded3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartDoughnut2D, typeof(InsertChartDoughnut2DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartDoughnutExploded2D, typeof(InsertChartDoughnutExploded2DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartLine, typeof(InsertChartLineCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartStackedLine, typeof(InsertChartStackedLineCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPercentStackedLine, typeof(InsertChartPercentStackedLineCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartLineWithMarkers, typeof(InsertChartLineWithMarkersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartStackedLineWithMarkers, typeof(InsertChartStackedLineWithMarkersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPercentStackedLineWithMarkers, typeof(InsertChartPercentStackedLineWithMarkersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartLine3D, typeof(InsertChartLine3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartArea, typeof(InsertChartAreaCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartStackedArea, typeof(InsertChartStackedAreaCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPercentStackedArea, typeof(InsertChartPercentStackedAreaCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartArea3D, typeof(InsertChartArea3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartStackedArea3D, typeof(InsertChartStackedArea3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartPercentStackedArea3D, typeof(InsertChartPercentStackedArea3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartScatterMarkers, typeof(InsertChartScatterMarkersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartScatterLines, typeof(InsertChartScatterLinesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartScatterSmoothLines, typeof(InsertChartScatterSmoothLinesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartScatterLinesAndMarkers, typeof(InsertChartScatterLinesAndMarkersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartScatterSmoothLinesAndMarkers, typeof(InsertChartScatterSmoothLinesAndMarkersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBubble, typeof(InsertChartBubbleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartBubble3D, typeof(InsertChartBubble3DCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartStockHighLowClose, typeof(InsertChartStockHighLowCloseCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartStockOpenHighLowClose, typeof(InsertChartStockOpenHighLowCloseCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartStockVolumeHighLowClose, typeof(InsertChartStockVolumeHighLowCloseCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartStockVolumeOpenHighLowClose, typeof(InsertChartStockVolumeOpenHighLowCloseCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartRadar, typeof(InsertChartRadarCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartRadarWithMarkers, typeof(InsertChartRadarWithMarkersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertChartRadarFilled, typeof(InsertChartRadarFilledCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertPivotTable, typeof(InsertPivotTableCommand));
		}
		#endregion
		#region PopulateClipboardCommandsTable
		protected internal virtual void PopulateClipboardCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.CopySelection, typeof(CopySelectionCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PasteSelection, typeof(PasteSelectionCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.CutSelection, typeof(CutSelectionCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShowPasteSpecialForm, typeof(PasteSpecialCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ClearCopiedRange, typeof(ClearCopiedRangeCommand));
		}
		#endregion
		#region PopulateHyperlinkCommandTable
		protected internal virtual void PopulateHyperlinkCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.InsertHyperlinkContextMenuItem, typeof(InsertHyperlinkContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditHyperlinkContextMenuItem, typeof(EditHyperlinkContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.OpenHyperlinkContextMenuItem, typeof(OpenHyperlinkContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.OpenPictureHyperlinkContextMenuItem, typeof(OpenPictureHyperlinkContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RemoveHyperlinkContextMenuItem, typeof(RemoveHyperlinkContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RemoveHyperlinksContextMenuItem, typeof(RemoveHyperlinksContextMenuItemCommand));
		}
		#endregion
		#region PopulatePictureCommandTable
		protected internal void PopulatePictureCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeMoveLeft, typeof(ShapeMoveLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeMoveRight, typeof(ShapeMoveRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeMoveUp, typeof(ShapeMoveUpCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeMoveDown, typeof(ShapeMoveDownCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeRotateLeft, typeof(ShapeRotateLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeRotateLeftByDegree, typeof(ShapeRotateLeftByDegreeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeRotateRight, typeof(ShapeRotateRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeRotateRightByDegree, typeof(ShapeRotateRightByDegreeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeEnlargeWidth, typeof(ShapeEnlargeWidthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeEnlargeHeight, typeof(ShapeEnlargeHeightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeBitEnlargeWidth, typeof(ShapeBitEnlargeWidthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeBitEnlargeHeight, typeof(ShapeBitEnlargeHeightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeReduceWidth, typeof(ShapeReduceWidthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeReduceHeight, typeof(ShapeReduceHeightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeBitReduceWidth, typeof(ShapeBitReduceWidthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeBitReduceHeight, typeof(ShapeBitReduceHeightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeSelectAll, typeof(ShapeSelectAllCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeSelectNext, typeof(ShapeSelectNextCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeSelectPrevious, typeof(ShapeSelectPreviousCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeChangeBounds, typeof(ShapeChangeBoundsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShapeMoveAndResize, typeof(ShapeMoveAndResizeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertPicture, typeof(InsertPictureCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.InsertSymbol, typeof(InsertSymbolCommand));
		}
		#endregion
		#region PopulateRemoveCommandsTable
		protected internal virtual void PopulateRemoveCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.RemoveSheet, typeof(RemoveSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RemoveSheetContextMenuItem, typeof(RemoveSheetContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RemoveSheetRows, typeof(RemoveSheetRowsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RemoveSheetRowsContextMenuItem, typeof(RemoveSheetRowsContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RemoveSheetColumns, typeof(RemoveSheetColumnsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RemoveSheetColumnsContextMenuItem, typeof(RemoveSheetColumnsContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RemoveCellsCommandGroup, typeof(RemoveCellsCommandGroup));
		}
		#endregion
		#region PopulateHideAndUnhideCommandsTable
		protected internal virtual void PopulateHideAndUnhideCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.HideRows, typeof(HideRowsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.HideRowsContextMenuItem, typeof(HideRowsContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.HideColumns, typeof(HideColumnsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.HideColumnsContextMenuItem, typeof(HideColumnsContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.HideSheet, typeof(HideSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.HideSheetContextMenuItem, typeof(HideSheetContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.UnhideRows, typeof(UnhideRowsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.UnhideRowsContextMenuItem, typeof(UnhideRowsContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.UnhideColumns, typeof(UnhideColumnsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.UnhideColumnsContextMenuItem, typeof(UnhideColumnsContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.UnhideSheet, typeof(UnhideSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.UnhideSheetContextMenuItem, typeof(UnhideSheetContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.HideAndUnhideCommandGroup, typeof(HideAndUnhideCommandGroup));
		}
		#endregion
		#region PopulateConditionalFormattingCommandsTable
		protected internal virtual void PopulateConditionalFormattingCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingRemoveFromSheet, typeof(ConditionalFormattingRemoveFromSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingRemove, typeof(ConditionalFormattingRemoveCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingCommandGroup, typeof(ConditionalFormattingCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingRemoveCommandGroup, typeof(ConditionalFormattingRemoveCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScalesCommandGroup, typeof(ConditionalFormattingColorScalesCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleGreenYellowRed, typeof(ConditionalFormattingColorScaleGreenYellowRedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleRedYellowGreen, typeof(ConditionalFormattingColorScaleRedYellowGreenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleGreenWhiteRed, typeof(ConditionalFormattingColorScaleGreenWhiteRedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhiteGreen, typeof(ConditionalFormattingColorScaleRedWhiteGreenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleBlueWhiteRed, typeof(ConditionalFormattingColorScaleBlueWhiteRedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhiteBlue, typeof(ConditionalFormattingColorScaleRedWhiteBlueCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleWhiteRed, typeof(ConditionalFormattingColorScaleWhiteRedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhite, typeof(ConditionalFormattingColorScaleRedWhiteCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleWhiteGreen, typeof(ConditionalFormattingColorScaleWhiteGreenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleGreenWhite, typeof(ConditionalFormattingColorScaleGreenWhiteCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleYellowGreen, typeof(ConditionalFormattingColorScaleYellowGreenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingColorScaleGreenYellow, typeof(ConditionalFormattingColorScaleGreenYellowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarsCommandGroup, typeof(ConditionalFormattingDataBarsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarsGradientFillCommandGroup, typeof(ConditionalFormattingDataBarsGradientFillCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarsSolidFillCommandGroup, typeof(ConditionalFormattingDataBarsSolidFillCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetsCommandGroup, typeof(ConditionalFormattingIconSetsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetsDirectionalCommandGroup, typeof(ConditionalFormattingIconSetsDirectionalCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetsShapesCommandGroup, typeof(ConditionalFormattingIconSetsShapesCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetsIndicatorsCommandGroup, typeof(ConditionalFormattingIconSetsIndicatorsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetsRatingsCommandGroup, typeof(ConditionalFormattingIconSetsRatingsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetArrows3Colored, typeof(ConditionalFormattingIconSetArrows3ColoredCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetArrows3Grayed, typeof(ConditionalFormattingIconSetArrows3GrayedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetArrows4Colored, typeof(ConditionalFormattingIconSetArrows4ColoredCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetArrows4Grayed, typeof(ConditionalFormattingIconSetArrows4GrayedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetArrows5Colored, typeof(ConditionalFormattingIconSetArrows5ColoredCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetArrows5Grayed, typeof(ConditionalFormattingIconSetArrows5GrayedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetTriangles3, typeof(ConditionalFormattingIconSetTriangles3Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights3, typeof(ConditionalFormattingIconSetTrafficLights3Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights3Rimmed, typeof(ConditionalFormattingIconSetTrafficLights3RimmedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights4, typeof(ConditionalFormattingIconSetTrafficLights4Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetSigns3, typeof(ConditionalFormattingIconSetSigns3Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetRedToBlack, typeof(ConditionalFormattingIconSetRedToBlackCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetSymbols3Circled, typeof(ConditionalFormattingIconSetSymbols3CircledCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetSymbols3, typeof(ConditionalFormattingIconSetSymbols3Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetFlags3, typeof(ConditionalFormattingIconSetFlags3Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetStars3, typeof(ConditionalFormattingIconSetStars3Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetRatings4, typeof(ConditionalFormattingIconSetRatings4Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetRatings5, typeof(ConditionalFormattingIconSetRatings5Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetQuarters5, typeof(ConditionalFormattingIconSetQuarters5Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingIconSetBoxes5, typeof(ConditionalFormattingIconSetBoxes5Command));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarGradientBlue, typeof(ConditionalFormattingDataBarGradientBlue));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarSolidBlue, typeof(ConditionalFormattingDataBarSolidBlue));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarGradientGreen, typeof(ConditionalFormattingDataBarGradientGreen));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarSolidGreen, typeof(ConditionalFormattingDataBarSolidGreen));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarGradientRed, typeof(ConditionalFormattingDataBarGradientRed));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarSolidRed, typeof(ConditionalFormattingDataBarSolidRed));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarGradientOrange, typeof(ConditionalFormattingDataBarGradientOrange));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarSolidOrange, typeof(ConditionalFormattingDataBarSolidOrange));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarGradientLightBlue, typeof(ConditionalFormattingDataBarGradientLightBlue));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarSolidLightBlue, typeof(ConditionalFormattingDataBarSolidLightBlue));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarGradientPurple, typeof(ConditionalFormattingDataBarGradientPurple));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDataBarSolidPurple, typeof(ConditionalFormattingDataBarSolidPurple));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingTopBottomRuleCommandGroup, typeof(ConditionalFormattingTopBottomRuleCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingTop10RuleCommand, typeof(ConditionalFormattingTop10RuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingBottom10RuleCommand, typeof(ConditionalFormattingBottom10RuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingTop10PercentRuleCommand, typeof(ConditionalFormattingTop10PercentRuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingBottom10PercentRuleCommand, typeof(ConditionalFormattingBottom10PercentRuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingAboveAverageRuleCommand, typeof(ConditionalFormattingAboveAverageRuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingBelowAverageRuleCommand, typeof(ConditionalFormattingBelowAverageRuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingHighlightCellsRuleCommandGroup, typeof(ConditionalFormattingHighlightCellsRuleCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingGreaterThanRuleCommand, typeof(ConditionalFormattingGreaterThanRuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingLessThanRuleCommand, typeof(ConditionalFormattingLessThanRuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingBetweenRuleCommand, typeof(ConditionalFormattingBetweenRuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingEqualToRuleCommand, typeof(ConditionalFormattingEqualToRuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingTextContainsRuleCommand, typeof(ConditionalFormattingTextContainsRuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDateOccurringRuleCommand, typeof(ConditionalFormattingDateOccurringRuleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ConditionalFormattingDuplicateValuesRuleCommand, typeof(ConditionalFormattingDuplicateValuesRuleCommand));
		}
		#endregion
		#region PopulateDataCommandsTable
		protected internal virtual void PopulateDataCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.DataSortAscending, typeof(DataSortAscendingCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataSortDescending, typeof(DataSortDescendingCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterDateFiltersCommandGroup, typeof(FilterDateCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterNumberFiltersCommandGroup, typeof(FilterNumberCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterTextFiltersCommandGroup, typeof(FilterTextCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterAllDatesInPeriodCommandGroup, typeof(FilterAllDatesInPeriodCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterClear, typeof(FilterClearCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterReApply, typeof(FilterReApplyCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterToggle, typeof(FilterToggleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterColumnClear, typeof(FilterColumnClearCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterEquals, typeof(FilterEqualsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterDoesNotEqual, typeof(FilterDoesNotEqualCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterCustom, typeof(FilterCustomCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterBeginsWith, typeof(FilterBeginsWithCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterEndsWith, typeof(FilterEndsWithCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterContains, typeof(FilterContainsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterDoesNotContain, typeof(FilterDoesNotContainCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterGreaterThan, typeof(FilterGreaterThanCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterGreaterThanOrEqualTo, typeof(FilterGreaterThanOrEqualToCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterLessThan, typeof(FilterLessThanCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterLessThanOrEqualTo, typeof(FilterLessThanOrEqualToCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterBetween, typeof(FilterBetweenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterTop10, typeof(FilterTop10Command));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterAboveAverage, typeof(FilterAboveAverageCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterBelowAverage, typeof(FilterBelowAverageCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterToday, typeof(FilterDateTodayCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterYesterday, typeof(FilterDateYesterdayCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterTomorrow, typeof(FilterDateTomorrowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterThisWeek, typeof(FilterDateThisWeekCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterNextWeek, typeof(FilterDateNextWeekCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterLastWeek, typeof(FilterDateLastWeekCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterThisMonth, typeof(FilterDateThisMonthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterNextMonth, typeof(FilterDateNextMonthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterLastMonth, typeof(FilterDateLastMonthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterThisQuarter, typeof(FilterDateThisQuarterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterNextQuarter, typeof(FilterDateNextQuarterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterLastQuarter, typeof(FilterDateLastQuarterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterThisYear, typeof(FilterDateThisYearCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterNextYear, typeof(FilterDateNextYearCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterLastYear, typeof(FilterDateLastYearCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterYearToDate, typeof(FilterDateYearToDateCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterQuarter1, typeof(FilterDateQuarter1Command));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterQuarter2, typeof(FilterDateQuarter2Command));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterQuarter3, typeof(FilterDateQuarter3Command));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterQuarter4, typeof(FilterDateQuarter4Command));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthJanuary, typeof(FilterDateMonthJanuaryCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthFebruary, typeof(FilterDateMonthFebruaryCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthMarch, typeof(FilterDateMonthMarchCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthApril, typeof(FilterDateMonthAprilCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthMay, typeof(FilterDateMonthMayCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthJune, typeof(FilterDateMonthJuneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthJuly, typeof(FilterDateMonthJulyCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthAugust, typeof(FilterDateMonthAugustCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthSeptember, typeof(FilterDateMonthSeptemberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthOctober, typeof(FilterDateMonthOctoberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthNovember, typeof(FilterDateMonthNovemberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterMonthDecember, typeof(FilterDateMonthDecemberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterDateEquals, typeof(FilterDateEqualsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterDateAfter, typeof(FilterDateAfterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterDateBefore, typeof(FilterDateBeforeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterDateBetween, typeof(FilterDateBetweenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterDateCustom, typeof(FilterDateCustomCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFilterSimple, typeof(FilterSimpleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataToolsDataValidationCommandGroup, typeof(DataValidationCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.DataToolsDataValidation, typeof(DataValidationCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataToolsCircleInvalidData, typeof(CircleInvalidDataCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataToolsClearValidationCircles, typeof(ClearValidationCirclesCommand));
		}
		#endregion
		#region PopulateReviewCommandsTable
		protected internal virtual void PopulateReviewCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewInsertComment, typeof(CommentInsertCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewInsertCommentContextMenuItem, typeof(CommentInsertContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewEditComment, typeof(CommentEditCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewEditCommentContextMenuItem, typeof(CommentEditContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewDeleteComment, typeof(CommentDeleteCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewDeleteCommentContextMenuItem, typeof(CommentDeleteContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewShowHideComment, typeof(CommentShowHideCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewShowHideCommentContextMenuItem, typeof(CommentShowHideContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewShowAllComment, typeof(CommentShowAllCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewProtectSheet, typeof(ProtectSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewProtectSheetContextMenuItem, typeof(ProtectSheetContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewUnprotectSheet, typeof(UnprotectSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewUnprotectSheetContextMenuItem, typeof(UnprotectSheetContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewProtectWorkbook, typeof(ProtectWorkbookCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewUnprotectWorkbook, typeof(UnprotectWorkbookCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ReviewShowProtectedRangeManager, typeof(ShowProtectedRangeManagerCommand));
		}
		#endregion
		#region PopulateAdditionalCommandsTable
		protected internal virtual void PopulateAdditionalCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.MoveToPreviousSheet, typeof(MoveToPreviousSheetCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MoveToNextSheet, typeof(MoveToNextSheetCommand));
		}
		#endregion
		#region PopulateCollapseOrExpandFormulaBarCommandTable
		protected internal virtual void PopulateCollapseOrExpandFormulaBarCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.CollapseOrExpandFormulaBar, typeof(CollapseOrExpandFormulaBarCommand));
		}
		#endregion
		#region PopulateChartDesignCommandTable
		void PopulateChartDesignCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartChangeType, typeof(ChangeChartTypeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartChangeTypeContextMenuItem, typeof(ChangeChartTypeContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartSwitchRowColumn, typeof(ChartSwitchRowColumnCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ModifyChartLayout, typeof(ModifyChartLayoutCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ModifyChartStyle, typeof(ModifyChartStyleCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartSelectData, typeof(ChartSelectDataCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartSelectDataContextMenuItem, typeof(ChartSelectDataContextMenuItemCommand));
		}
		#endregion
		#region PopulateChartLayoutCommandTable
		void PopulateChartLayoutCommandTable(SpreadsheetCommandConstructorTable table) {
			PopulateChartLayoutTitleCommandTable(table);
			PopulateChartLayoutAxisTitlesCommandTable(table);
			PopulateChartLayoutLegendCommandTable(table);
			PopulateChartLayoutDataLabelsCommandTable(table);
			PopulateChartLayoutAxesCommandTable(table);
			PopulateChartLayoutGridlinesCommandTable(table);
			PopulateChartLayoutAnalysisLinesCommandTable(table);
			PopulateChartLayoutAnalysisUpDownBarsCommandTable(table);
			PopulateChartLayoutAnalysisErrorBarsCommandTable(table);
		}
		void PopulateChartLayoutAxesCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartAxesCommandGroup, typeof(ChartAxesCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisCommandGroup, typeof(ChartPrimaryHorizontalAxisCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisCommandGroup, typeof(ChartPrimaryVerticalAxisCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartHidePrimaryHorizontalAxis, typeof(ChartHidePrimaryHorizontalAxisCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisLeftToRight, typeof(ChartPrimaryHorizontalAxisLeftToRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisRightToLeft, typeof(ChartPrimaryHorizontalAxisRightToLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisHideLabels, typeof(ChartPrimaryHorizontalAxisHideLabelsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisDefault, typeof(ChartPrimaryHorizontalAxisDefaultCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleLogarithm, typeof(ChartPrimaryHorizontalAxisScaleLogarithmCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleThousands, typeof(ChartPrimaryHorizontalAxisScaleThousandsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleMillions, typeof(ChartPrimaryHorizontalAxisScaleMillionsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleBillions, typeof(ChartPrimaryHorizontalAxisScaleBillionsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartHidePrimaryVerticalAxis, typeof(ChartHidePrimaryVerticalAxisCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisLeftToRight, typeof(ChartPrimaryVerticalAxisLeftToRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisRightToLeft, typeof(ChartPrimaryVerticalAxisRightToLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisHideLabels, typeof(ChartPrimaryVerticalAxisHideLabelsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisDefault, typeof(ChartPrimaryVerticalAxisDefaultCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleLogarithm, typeof(ChartPrimaryVerticalAxisScaleLogarithmCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleThousands, typeof(ChartPrimaryVerticalAxisScaleThousandsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleMillions, typeof(ChartPrimaryVerticalAxisScaleMillionsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleBillions, typeof(ChartPrimaryVerticalAxisScaleBillionsCommand));
		}
		void PopulateChartLayoutGridlinesCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartGridlinesCommandGroup, typeof(ChartGridlinesCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesCommandGroup, typeof(ChartPrimaryHorizontalGridlinesCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalGridlinesCommandGroup, typeof(ChartPrimaryVerticalGridlinesCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesNone, typeof(ChartPrimaryHorizontalGridlinesNoneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMajor, typeof(ChartPrimaryHorizontalGridlinesMajorCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMinor, typeof(ChartPrimaryHorizontalGridlinesMinorCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMajorAndMinor, typeof(ChartPrimaryHorizontalGridlinesMajorAndMinorCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalGridlinesNone, typeof(ChartPrimaryVerticalGridlinesNoneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMajor, typeof(ChartPrimaryVerticalGridlinesMajorCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMinor, typeof(ChartPrimaryVerticalGridlinesMinorCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMajorAndMinor, typeof(ChartPrimaryVerticalGridlinesMajorAndMinorCommand));
		}
		void PopulateChartLayoutTitleCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartTitleCommandGroup, typeof(ChartTitleCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartTitleNone, typeof(ChartTitleNoneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartTitleCenteredOverlay, typeof(ChartTitleCenteredOverlayCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartTitleAbove, typeof(ChartTitleAboveCommand));
		}
		void PopulateChartLayoutAxisTitlesCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartAxisTitlesCommandGroup, typeof(ChartAxisTitlesCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleCommandGroup, typeof(ChartPrimaryHorizontalAxisTitleCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleCommandGroup, typeof(ChartPrimaryVerticalAxisTitleCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleNone, typeof(ChartPrimaryHorizontalAxisTitleNoneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleBelow, typeof(ChartPrimaryHorizontalAxisTitleBelowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleNone, typeof(ChartPrimaryVerticalAxisTitleNoneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleRotated, typeof(ChartPrimaryVerticalAxisTitleRotatedCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleVertical, typeof(ChartPrimaryVerticalAxisTitleVerticalCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleHorizontal, typeof(ChartPrimaryVerticalAxisTitleHorizontalCommand));
		}
		void PopulateChartLayoutLegendCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartLegendCommandGroup, typeof(ChartLegendCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartLegendNone, typeof(ChartLegendNoneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartLegendAtRight, typeof(ChartLegendAtRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartLegendAtTop, typeof(ChartLegendAtTopCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartLegendAtLeft, typeof(ChartLegendAtLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartLegendAtBottom, typeof(ChartLegendAtBottomCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartLegendOverlayAtRight, typeof(ChartLegendOverlayAtRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartLegendOverlayAtLeft, typeof(ChartLegendOverlayAtLeftCommand));
		}
		void PopulateChartLayoutDataLabelsCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsCommandGroup, typeof(ChartDataLabelsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsNone, typeof(ChartDataLabelsNoneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsDefault, typeof(ChartDataLabelsDefaultCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsCenter, typeof(ChartDataLabelsCenterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsInsideEnd, typeof(ChartDataLabelsInsideEndCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsInsideBase, typeof(ChartDataLabelsInsideBaseCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsOutsideEnd, typeof(ChartDataLabelsOutsideEndCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsBestFit, typeof(ChartDataLabelsBestFitCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsLeft, typeof(ChartDataLabelsLeftCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsRight, typeof(ChartDataLabelsRightCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsAbove, typeof(ChartDataLabelsAboveCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartDataLabelsBelow, typeof(ChartDataLabelsBelowCommand));
		}
		void PopulateChartLayoutAnalysisLinesCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartLinesCommandGroup, typeof(ChartLinesCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartLinesNone, typeof(ChartLinesNoneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartShowDropLines, typeof(ChartShowDropLinesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartShowHighLowLines, typeof(ChartShowHighLowLinesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartShowDropLinesAndHighLowLines, typeof(ChartShowDropLinesAndHighLowLinesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartShowSeriesLines, typeof(ChartShowSeriesLinesCommand));
		}
		void PopulateChartLayoutAnalysisUpDownBarsCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartUpDownBarsCommandGroup, typeof(ChartUpDownBarsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartHideUpDownBars, typeof(ChartHideUpDownBarsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartShowUpDownBars, typeof(ChartShowUpDownBarsCommand));
		}
		void PopulateChartLayoutAnalysisErrorBarsCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartErrorBarsCommandGroup, typeof(ChartErrorBarsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartErrorBarsNone, typeof(ChartErrorBarsNoneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartErrorBarsPercentage, typeof(ChartErrorBarsPercentageCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartErrorBarsStandardError, typeof(ChartErrorBarsStandardErrorCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartErrorBarsStandardDeviation, typeof(ChartErrorBarsStandardDeviationCommand));
		}
		#endregion
		#region PopulateChartItemsTitleCommandsTable
		void PopulateChartItemsTitleCommandsTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ChartChangeTitleContextMenuItem, typeof(ChartChangeTitleContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartChangeHorizontalAxisTitleContextMenuItem, typeof(ChartChangeHorizontalAxisTitleContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChartChangeVerticalAxisTitleContextMenuItem, typeof(ChartChangeVerticalAxisTitleContextMenuItemCommand));
		}
		#endregion
		#region PopulateTableToolsDesign
		void PopulateTableToolsDesignCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.TableToolsRenameTable, typeof(TableToolsRenameTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.TableToolsConvertToRange, typeof(TableToolsConvertToRangeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ModifyTableStyles, typeof(ModifyTableStylesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.TableToolsToggleHeaderRow, typeof(TableToolsToggleHeaderRowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.TableToolsToggleTotalRow, typeof(TableToolsToggleTotalRowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.TableToolsToggleBandedColumns, typeof(TableToolsToggleBandedColumnsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.TableToolsToggleBandedRows, typeof(TableToolsToggleBandedRowsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.TableToolsToggleFirstColumn, typeof(TableToolsToggleFirstColumnCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.TableToolsToggleLastColumn, typeof(TableToolsToggleLastColumnCommand));
		}
		#endregion
		#region PopulateMailMergeDesign
		void PopulateMailMergeDesignCommandTable(SpreadsheetCommandConstructorTable table) {
#if !DXPORTABLE
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSelectDataMember, typeof(MailMergeSelectDataMemberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeAddDataSource, typeof(MailMergeAddDataSourceCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSelectDataSource, typeof(MailMergeSelectDataSourceCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeDocumentsMode, typeof(MailMergeDocumentsModeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeOneDocumentMode, typeof(MailMergeOneDocumentModeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeOneSheetMode, typeof(MailMergeOneSheetModeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeOrientationCommandGroup, typeof(MailMergeOrientationCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeHorizontalMode, typeof(MailMergeHorizontalModeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeVerticalMode, typeof(MailMergeVerticalModeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeResetRange, typeof(ResetRangeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSetDetailRange, typeof(SetDetailRangeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSetFooterRange, typeof(SetFooterRangeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSetHeaderRange, typeof(SetHeaderRangeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.EditingMailMergeMasterDetailCommandGroup, typeof(EditingMailMergeMasterDetailCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSetDetailLevel, typeof(SetDetailLevelCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSetDetailDataMember, typeof(SetDetailDataMemberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSetGroup, typeof(SetGroupCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSetGroupHeader, typeof(SetGroupHeaderCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSetGroupFooter, typeof(SetGroupFooterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeSetFilter, typeof(SetFilterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeResetFilter, typeof(ResetFilterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeShowRanges, typeof(ShowRangesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergePreview, typeof(MailMergePreviewCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeManageRelationsCommandGroup, typeof(MailMergeManageRelationsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeManageRelationsCommand, typeof(MailMergeManageRelationsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeManageQueriesCommand, typeof(MailMergeManageQueriesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeManageDataSourceCommandGroup, typeof(MailMergeManageDataSourceCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.MailMergeManageDataSourcesCommand, typeof(MailMergeManageDataSourcesCommand));
#endif
		}
		#endregion
		#region PopulateArrangeCommandTable
		void PopulateArrangeCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ArrangeBringForwardCommandGroup, typeof(ArrangeBringForwardCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ArrangeBringForwardCommandGroupContextMenuItem, typeof(ArrangeBringForwardCommandGroupContextMenuItem));
			AddCommandConstructor(table, SpreadsheetCommandId.ArrangeBringForward, typeof(ArrangeBringForwardCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ArrangeBringToFront, typeof(ArrangeBringToFrontCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ArrangeSendBackwardCommandGroup, typeof(ArrangeSendBackwardCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ArrangeSendBackwardCommandGroupContextMenuItem, typeof(ArrangeSendBackwardCommandGroupContextMenuItem));
			AddCommandConstructor(table, SpreadsheetCommandId.ArrangeSendBackward, typeof(ArrangeSendBackwardCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ArrangeSendToBack, typeof(ArrangeSendToBackCommand));
		}
#endregion
		#region PopulateToolsCommandTable
		void PopulateToolsCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.ToolsPictureCommandGroup, typeof(ToolsPictureCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ToolsDrawingCommandGroup, typeof(ToolsDrawingCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ToolsChartCommandGroup, typeof(ToolsChartCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.TableToolsCommandGroup, typeof(TableToolsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ToolsPivotTableCommandGroup, typeof(ToolsPivotTableCommandGroup));
		}
		#endregion
		#region PopulateGroupDesign
				void PopulateGroupDesignCommandTable(SpreadsheetCommandConstructorTable table) {
					AddCommandConstructor(table, SpreadsheetCommandId.AutoOutline, typeof(AutoOutlineCommand));
					AddCommandConstructor(table, SpreadsheetCommandId.ClearOutline, typeof(ClearOutlineCommand));
					AddCommandConstructor(table, SpreadsheetCommandId.GroupOutline, typeof(GroupCommand));
					AddCommandConstructor(table, SpreadsheetCommandId.HideDetail, typeof(HideDetailCommand));
					AddCommandConstructor(table, SpreadsheetCommandId.ShowDetail, typeof(ShowDetailCommand));
					AddCommandConstructor(table, SpreadsheetCommandId.Subtotal, typeof(SubtotalCommand));
					AddCommandConstructor(table, SpreadsheetCommandId.UngroupOutline, typeof(UngroupCommand));
					AddCommandConstructor(table, SpreadsheetCommandId.OutlineGroupCommandGroup, typeof(OutlineGroupCommandGroup));
					AddCommandConstructor(table, SpreadsheetCommandId.OutlineUngroupCommandGroup, typeof(OutlineUngroupCommandGroup));
					AddCommandConstructor(table, SpreadsheetCommandId.OutlineSettingsCommand, typeof(OutlineSettingsCommand));
				}
		#endregion
		#region PopulateCommentEditorCommandsTable
				protected internal virtual void PopulateCommentEditorCommandsTable(SpreadsheetCommandConstructorTable table) {
					AddCommandConstructor(table, SpreadsheetCommandId.CommentCloseEditor, typeof(CommentCloseEditorCommand));
				}
		#endregion
		#region PopulateDataValidationEditorCommandsTable
				protected internal virtual void PopulateDataValidationEditorCommandsTable(SpreadsheetCommandConstructorTable table) {
					AddCommandConstructor(table, SpreadsheetCommandId.DataValidationCloseEditor, typeof(DataValidationCloseEditorCommand));
					AddCommandConstructor(table, SpreadsheetCommandId.DataValidationInplaceEndEdit, typeof(DataValidationInplaceEndEditCommand));
				}
		#endregion
		#region PopulatePivotTableAnalyzeCommandTable
		void PopulatePivotTableAnalyzeCommandTable(SpreadsheetCommandConstructorTable table) {
			AddCommandConstructor(table, SpreadsheetCommandId.OptionsPivotTable, typeof(OptionsPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.OptionsPivotTableContextMenuItem, typeof(OptionsPivotTableContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotTable, typeof(MovePivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ChangeDataSourcePivotTable, typeof(ChangeDataSourcePivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectFieldTypePivotTable, typeof(SelectFieldTypePivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FieldSettingsPivotTable, typeof(FieldSettingsPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FieldSettingsPivotTableContextMenuItem, typeof(FieldSettingsPivotTableContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFieldSettingsPivotTable, typeof(DataFieldSettingsPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.DataFieldSettingsPivotTableContextMenuItem, typeof(DataFieldSettingsPivotTableContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FieldListPanelPivotTable, typeof(FieldListPanelPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableActionsClearGroup, typeof(PivotTableActionsClearCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.ClearAllPivotTable, typeof(ClearAllPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ClearFiltersPivotTable, typeof(ClearFiltersPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableActionsSelectGroup, typeof(PivotTableActionsSelectCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectValuesPivotTable, typeof(SelectValuesPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectLabelsPivotTable, typeof(SelectLabelsPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SelectEntirePivotTable, typeof(SelectEntirePivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDataRefreshGroup, typeof(PivotTableDataRefreshCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.RefreshPivotTable, typeof(RefreshPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RefreshAllPivotTable, typeof(RefreshAllPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShowPivotTableFieldHeaders, typeof(ShowPivotTableFieldHeadersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ShowPivotTableExpandCollapseButtons, typeof(ShowPivotTableExpandCollapseButtonsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLayoutSubtotalsGroup, typeof(PivotTableLayoutSubtotalsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLayoutGrandTotalsGroup, typeof(PivotTableLayoutGrandTotalsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLayoutReportLayoutGroup, typeof(PivotTableLayoutReportLayoutCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLayoutBlankRowsGroup, typeof(PivotTableLayoutBlankRowsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDoNotShowSubtotals, typeof(PivotTableDoNotShowSubtotalsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowAllSubtotalsAtBottom, typeof(PivotTableShowAllSubtotalsAtBottomCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowAllSubtotalsAtTop, typeof(PivotTableShowAllSubtotalsAtTopCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableGrandTotalsOffRowsColumns, typeof(PivotTableGrandTotalsOffRowsColumnsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableGrandTotalsOnRowsColumns, typeof(PivotTableGrandTotalsOnRowsColumnsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableGrandTotalsOnRowsOnly, typeof(PivotTableGrandTotalsOnRowsOnlyCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableGrandTotalsOnColumnsOnly, typeof(PivotTableGrandTotalsOnColumnsOnlyCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowCompactForm, typeof(PivotTableShowCompactFormCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowOutlineForm, typeof(PivotTableShowOutlineFormCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowTabularForm, typeof(PivotTableShowTabularFormCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableRepeatAllItemLabels, typeof(PivotTableRepeatAllItemLabelsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDoNotRepeatItemLabels, typeof(PivotTableDoNotRepeatItemLabelsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableInsertBlankLineEachItem, typeof(PivotTableInsertBlankLineCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableRemoveBlankLineEachItem, typeof(PivotTableRemoveBlankLineCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableToggleRowHeaders, typeof(PivotTableToggleRowHeadersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableToggleColumnHeaders, typeof(PivotTableToggleColumnHeadersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableToggleBandedRows, typeof(PivotTableToggleBandedRowsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableToggleBandedColumns, typeof(PivotTableToggleBandedColumnsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.ModifyPivotTableStyles, typeof(ModifyPivotTableStylesCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableExpandField, typeof(PivotTableExpandFieldCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableCollapseField, typeof(PivotTableCollapseFieldCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTablePageFieldsFilterItems, typeof(PivotTablePageFieldsFilterItemsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableRowFieldsFilterItems, typeof(PivotTableRowFieldsFilterItemsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableColumnFieldsFilterItems, typeof(PivotTableColumnFieldsFilterItemsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.FieldListPanelPivotTableContextMenuItem, typeof(ShowHideFieldListPanelPivotTableContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableExpandCollapseFieldCommandGroup, typeof(PivotTableExpandCollapseFieldCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableExpandFieldContextMenuItem, typeof(PivotTableExpandFieldContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableCollapseFieldContextMenuItem, typeof(PivotTableCollapseFieldContextMenuItemCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableFieldSortCommandGroup, typeof(PivotTableFieldSortCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableFieldsFiltersCommandGroup, typeof(PivotTableFieldsFiltersCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFiltersCommandGroup, typeof(PivotTableLabelFiltersCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFiltersCommandGroup, typeof(PivotTableDateFiltersCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableValueFiltersCommandGroup, typeof(PivotTableValueFiltersCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableFilterAllDatesInPeriodCommandGroup, typeof(PivotTableFilterAllDatesInPeriodCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterToday, typeof(PivotTableDateFilterTodayCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterYesterday, typeof(PivotTableDateFilterYesterdayCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterTomorrow, typeof(PivotTableDateFilterTomorrowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterThisWeek, typeof(PivotTableDateFilterThisWeekCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterNextWeek, typeof(PivotTableDateFilterNextWeekCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterLastWeek, typeof(PivotTableDateFilterLastWeekCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterThisMonth, typeof(PivotTableDateFilterThisMonthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterNextMonth, typeof(PivotTableDateFilterNextMonthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterLastMonth, typeof(PivotTableDateFilterLastMonthCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterThisQuarter, typeof(PivotTableDateFilterThisQuarterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterNextQuarter, typeof(PivotTableDateFilterNextQuarterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterLastQuarter, typeof(PivotTableDateFilterLastQuarterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterThisYear, typeof(PivotTableDateFilterThisYearCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterNextYear, typeof(PivotTableDateFilterNextYearCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterLastYear, typeof(PivotTableDateFilterLastYearCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterYearToDate, typeof(PivotTableDateFilterYearToDateCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterFirstQuarter, typeof(PivotTableDateFilterFirstQuarterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterSecondQuarter, typeof(PivotTableDateFilterSecondQuarterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterThirdQuarter, typeof(PivotTableDateFilterThirdQuarterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterFourthQuarter, typeof(PivotTableDateFilterFourthQuarterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterJanuary, typeof(PivotTableDateFilterJanuaryCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterFebruary, typeof(PivotTableDateFilterFebruaryCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterMarch, typeof(PivotTableDateFilterMarchCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterApril, typeof(PivotTableDateFilterAprilCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterMay, typeof(PivotTableDateFilterMayCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterJune, typeof(PivotTableDateFilterJuneCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterJuly, typeof(PivotTableDateFilterJulyCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterAugust, typeof(PivotTableDateFilterAugustCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterSeptember, typeof(PivotTableDateFilterSeptemberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterOctober, typeof(PivotTableDateFilterOctoberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterNovember, typeof(PivotTableDateFilterNovemberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterDecember, typeof(PivotTableDateFilterDecemberCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterEquals, typeof(PivotTableDateFilterEqualsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterBefore, typeof(PivotTableDateFilterBeforeCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterAfter, typeof(PivotTableDateFilterAfterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterBetween, typeof(PivotTableDateFilterBetweenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableDateFilterCustom, typeof(PivotTableCustomDateFilterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterEquals, typeof(PivotTableLabelFilterEqualsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterDoesNotEqual, typeof(PivotTableLabelFilterDoesNotEqualCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterBeginsWith, typeof(PivotTableLabelFilterBeginsWithCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterDoesNotBeginWith, typeof(PivotTableLabelFilterDoesNotBeginWithCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterEndsWith, typeof(PivotTableLabelFilterEndsWithCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterDoesNotEndWith, typeof(PivotTableLabelFilterDoesNotEndWithCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterContains, typeof(PivotTableLabelFilterContainsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterDoesNotContain, typeof(PivotTableLabelFilterDoesNotContainCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterGreaterThan, typeof(PivotTableLabelFilterGreaterThanCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterGreaterThanOrEqual, typeof(PivotTableLabelFilterGreaterThanOrEqualCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterLessThan, typeof(PivotTableLabelFilterLessThanCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterLessThanOrEqual, typeof(PivotTableLabelFilterLessThanOrEqualCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterBetween, typeof(PivotTableLabelFilterBetweenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableLabelFilterNotBetween, typeof(PivotTableLabelFilterNotBetweenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableValueFilterEquals, typeof(PivotTableValueFilterEqualsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableValueFilterDoesNotEqual, typeof(PivotTableValueFilterDoesNotEqualCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableValueFilterGreaterThan, typeof(PivotTableValueFilterGreaterThanCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableValueFilterGreaterThanOrEqual, typeof(PivotTableValueFilterGreaterThanOrEqualCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableValueFilterLessThan, typeof(PivotTableValueFilterLessThanCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableValueFilterLessThanOrEqual, typeof(PivotTableValueFilterLessThanOrEqualCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableValueFilterBetween, typeof(PivotTableValueFilterBetweenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableValueFilterNotBetween, typeof(PivotTableValueFilterNotBetweenCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableValueFilterTop10, typeof(PivotTableValueFilterTop10Command));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableClearFieldFilters, typeof(PivotTableClearFieldFiltersCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableClearFieldLabelFilter, typeof(PivotTableClearFieldLabelFilterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableClearFieldValueFilter, typeof(PivotTableClearFieldValueFilterCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableFieldSortAscending, typeof(PivotTableFieldSortAscendingCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableFieldSortDescending, typeof(PivotTableFieldSortDescendingCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RemovePivotFieldCommand, typeof(RemovePivotFieldCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.RemoveGrandTotalPivotTable, typeof(RemoveGrandTotalPivotTableCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableSummarizeValuesByCommandGroup, typeof(PivotTableSummarizeValuesByCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableSummarizeValuesBySum, typeof(PivotTableSummarizeValuesBySumCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableSummarizeValuesByCount, typeof(PivotTableSummarizeValuesByCountCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableSummarizeValuesByMin, typeof(PivotTableSummarizeValuesByMinCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableSummarizeValuesByMax, typeof(PivotTableSummarizeValuesByMaxCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableSummarizeValuesByAverage, typeof(PivotTableSummarizeValuesByAverageCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableSummarizeValuesByProduct, typeof(PivotTableSummarizeValuesByProductCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableSummarizeValuesByMoreOptions, typeof(PivotTableSummarizeValuesByMoreOptionsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.SubtotalPivotField, typeof(SubtotalPivotFieldCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotFieldReferenceCommandGroup, typeof(MovePivotFieldReferenceCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotFieldReferenceUp, typeof(MovePivotFieldReferenceUpCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotFieldReferenceDown, typeof(MovePivotFieldReferenceDownCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotFieldReferenceToBeginning, typeof(MovePivotFieldReferenceToBeginningCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotFieldReferenceToEnd, typeof(MovePivotFieldReferenceToEndCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotFieldReferenceToDifferentAxis, typeof(MovePivotFieldReferenceToDifferentAxisCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotFieldItemUp, typeof(MovePivotFieldItemUpCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotFieldItemDown, typeof(MovePivotFieldItemDownCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotFieldItemToBeginning, typeof(MovePivotFieldItemToBeginningCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.MovePivotFieldItemToEnd, typeof(MovePivotFieldItemToEndCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsCommandGroup, typeof(PivotTableShowValuesAsCommandGroup));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsNormal, typeof(PivotTableShowValuesAsNormalCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsPercentOfTotal, typeof(PivotTableShowValuesAsPercentOfTotalCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsPercentOfColumn, typeof(PivotTableShowValuesAsPercentOfColumnCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsPercentOfParentColumn, typeof(PivotTableShowValuesAsPercentOfParentColumnCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsPercentOfRow, typeof(PivotTableShowValuesAsPercentOfRowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsPercentOfParentRow, typeof(PivotTableShowValuesAsPercentOfParentRowCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsIndex, typeof(PivotTableShowValuesAsIndexCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsMoreOptions, typeof(PivotTableShowValuesAsMoreOptionsCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsPercentOfParent, typeof(PivotTableShowValuesAsPercentOfParentCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsRunningTotal, typeof(PivotTableShowValuesAsRunningTotalCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsPercentOfRunningTotal, typeof(PivotTableShowValuesAsPercentOfRunningTotalCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsRankAscending, typeof(PivotTableShowValuesAsRankAscendingCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsRankDescending, typeof(PivotTableShowValuesAsRankDescendingCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsPercent, typeof(PivotTableShowValuesAsPercentCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsDifference, typeof(PivotTableShowValuesAsDifferenceCommand));
			AddCommandConstructor(table, SpreadsheetCommandId.PivotTableShowValuesAsPercentDifference, typeof(PivotTableShowValuesAsPercentDifferenceCommand));
		}
		#endregion
		#region ICommandFactoryService Members
				public virtual SpreadsheetCommand CreateCommand(SpreadsheetCommandId commandId) {
			SpreadsheetCommand command = CreateCommandCore(commandId);
			if (command == null)
				Exceptions.ThrowArgumentException("commandId", commandId);
			return command;
		}
		protected internal virtual SpreadsheetCommand CreateCommandCore(SpreadsheetCommandId commandId) {
			ConstructorInfo ci;
			if (commandConstructorTable.TryGetValue(commandId, out ci))
				return (SpreadsheetCommand)ci.Invoke(new object[] { Control });
			else
				return null;
		}
#endregion
	}
#endregion
}
