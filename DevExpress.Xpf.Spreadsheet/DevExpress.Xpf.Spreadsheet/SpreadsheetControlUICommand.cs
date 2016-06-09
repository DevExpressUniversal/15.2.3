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
using System.Windows.Input;
using DevExpress.Xpf.Bars.Native;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Xpf.Office.UI;
namespace DevExpress.Xpf.Spreadsheet {
	public class SpreadsheetUICommand : ICommand, ICommandWithInvoker {
		static readonly SpreadsheetUICommand fileNew = new SpreadsheetUICommand(SpreadsheetCommandId.FileNew);
		static readonly SpreadsheetUICommand fileOpen = new SpreadsheetUICommand(SpreadsheetCommandId.FileOpen);
		static readonly SpreadsheetUICommand fileSave = new SpreadsheetUICommand(SpreadsheetCommandId.FileSave);
		static readonly SpreadsheetUICommand fileSaveAs = new SpreadsheetUICommand(SpreadsheetCommandId.FileSaveAs);
		static readonly SpreadsheetUICommand fileQuickPrint = new SpreadsheetUICommand(SpreadsheetCommandId.FileQuickPrint);
		static readonly SpreadsheetUICommand filePrint = new SpreadsheetUICommand(SpreadsheetCommandId.FilePrint);
		static readonly SpreadsheetUICommand filePrintPreview = new SpreadsheetUICommand(SpreadsheetCommandId.FilePrintPreview);
		static readonly SpreadsheetUICommand fileUndo = new SpreadsheetUICommand(SpreadsheetCommandId.FileUndo);
		static readonly SpreadsheetUICommand fileRedo = new SpreadsheetUICommand(SpreadsheetCommandId.FileRedo);
		static readonly SpreadsheetUICommand fileShowDocumentProperties = new SpreadsheetUICommand(SpreadsheetCommandId.FileShowDocumentProperties);
		static readonly SpreadsheetUICommand editCut = new SpreadsheetUICommand(SpreadsheetCommandId.CutSelection);
		static readonly SpreadsheetUICommand editCopy = new SpreadsheetUICommand(SpreadsheetCommandId.CopySelection);
		static readonly SpreadsheetUICommand editPaste = new SpreadsheetUICommand(SpreadsheetCommandId.PasteSelection);
		static readonly SpreadsheetUICommand editPasteSpecial = new SpreadsheetUICommand(SpreadsheetCommandId.ShowPasteSpecialForm);
		static readonly SpreadsheetUICommand formatCells = new SpreadsheetUICommand(SpreadsheetCommandId.FormatCellsContextMenuItem);
		static readonly SpreadsheetUICommand formatCellsFont = new SpreadsheetUICommand(SpreadsheetCommandId.FormatCellsFont);
		static readonly SpreadsheetUICommand formatCellsAlignment = new SpreadsheetUICommand(SpreadsheetCommandId.FormatCellsAlignment);
		static readonly SpreadsheetUICommand formatCellsNumber = new SpreadsheetUICommand(SpreadsheetCommandId.FormatCellsNumber);
		static readonly SpreadsheetUICommand formatFontName = new SpreadsheetUICommand(SpreadsheetCommandId.FormatFontName);
		static readonly SpreadsheetUICommand formatFontSize = new SpreadsheetUICommand(SpreadsheetCommandId.FormatFontSize);
		static readonly SpreadsheetUICommand formatFontColor = new SpreadsheetUICommand(SpreadsheetCommandId.FormatFontColor);
		static readonly SpreadsheetUICommand formatFillColor = new SpreadsheetUICommand(SpreadsheetCommandId.FormatFillColor);
		static readonly SpreadsheetUICommand formatFontBold = new SpreadsheetUICommand(SpreadsheetCommandId.FormatFontBold);
		static readonly SpreadsheetUICommand formatFontItalic = new SpreadsheetUICommand(SpreadsheetCommandId.FormatFontItalic);
		static readonly SpreadsheetUICommand formatFontUnderline = new SpreadsheetUICommand(SpreadsheetCommandId.FormatFontUnderline);
		static readonly SpreadsheetUICommand formatFontStrikeout = new SpreadsheetUICommand(SpreadsheetCommandId.FormatFontStrikeout);
		static readonly SpreadsheetUICommand formatIncreaseFontSize = new SpreadsheetUICommand(SpreadsheetCommandId.FormatIncreaseFontSize);
		static readonly SpreadsheetUICommand formatDecreaseFontSize = new SpreadsheetUICommand(SpreadsheetCommandId.FormatDecreaseFontSize);
		static readonly SpreadsheetUICommand formatBordersCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FormatBordersCommandGroup);
		static readonly SpreadsheetUICommand formatBottomBorder = new SpreadsheetUICommand(SpreadsheetCommandId.FormatBottomBorder);
		static readonly SpreadsheetUICommand formatTopBorder = new SpreadsheetUICommand(SpreadsheetCommandId.FormatTopBorder);
		static readonly SpreadsheetUICommand formatLeftBorder = new SpreadsheetUICommand(SpreadsheetCommandId.FormatLeftBorder);
		static readonly SpreadsheetUICommand formatRightBorder = new SpreadsheetUICommand(SpreadsheetCommandId.FormatRightBorder);
		static readonly SpreadsheetUICommand formatNoBorders = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNoBorders);
		static readonly SpreadsheetUICommand formatAllBorders = new SpreadsheetUICommand(SpreadsheetCommandId.FormatAllBorders);
		static readonly SpreadsheetUICommand formatOutsideBorders = new SpreadsheetUICommand(SpreadsheetCommandId.FormatOutsideBorders);
		static readonly SpreadsheetUICommand formatThickBorder = new SpreadsheetUICommand(SpreadsheetCommandId.FormatThickBorder);
		static readonly SpreadsheetUICommand formatBottomDoubleBorder = new SpreadsheetUICommand(SpreadsheetCommandId.FormatBottomDoubleBorder);
		static readonly SpreadsheetUICommand formatBottomThickBorder = new SpreadsheetUICommand(SpreadsheetCommandId.FormatBottomThickBorder);
		static readonly SpreadsheetUICommand formatTopAndBottomBorder = new SpreadsheetUICommand(SpreadsheetCommandId.FormatTopAndBottomBorder);
		static readonly SpreadsheetUICommand formatTopAndThickBottomBorder = new SpreadsheetUICommand(SpreadsheetCommandId.FormatTopAndThickBottomBorder);
		static readonly SpreadsheetUICommand formatTopAndDoubleBottomBorder = new SpreadsheetUICommand(SpreadsheetCommandId.FormatTopAndDoubleBottomBorder);
		static readonly SpreadsheetUICommand formatCellLocked = new SpreadsheetUICommand(SpreadsheetCommandId.FormatCellLocked);
		static readonly SpreadsheetUICommand formatAlignmentTop = new SpreadsheetUICommand(SpreadsheetCommandId.FormatAlignmentTop);
		static readonly SpreadsheetUICommand formatAlignmentMiddle = new SpreadsheetUICommand(SpreadsheetCommandId.FormatAlignmentMiddle);
		static readonly SpreadsheetUICommand formatAlignmentBottom = new SpreadsheetUICommand(SpreadsheetCommandId.FormatAlignmentBottom);
		static readonly SpreadsheetUICommand formatAlignmentLeft = new SpreadsheetUICommand(SpreadsheetCommandId.FormatAlignmentLeft);
		static readonly SpreadsheetUICommand formatAlignmentCenter = new SpreadsheetUICommand(SpreadsheetCommandId.FormatAlignmentCenter);
		static readonly SpreadsheetUICommand formatAlignmentRight = new SpreadsheetUICommand(SpreadsheetCommandId.FormatAlignmentRight);
		static readonly SpreadsheetUICommand formatDecreaseIndent = new SpreadsheetUICommand(SpreadsheetCommandId.FormatDecreaseIndent);
		static readonly SpreadsheetUICommand formatIncreaseIndent = new SpreadsheetUICommand(SpreadsheetCommandId.FormatIncreaseIndent);
		static readonly SpreadsheetUICommand formatWrapText = new SpreadsheetUICommand(SpreadsheetCommandId.FormatWrapText);
		static readonly SpreadsheetUICommand formatNumberAccountingCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNumberAccountingCommandGroup);
		static readonly SpreadsheetUICommand formatNumberAccountingUS = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNumberAccountingUS);
		static readonly SpreadsheetUICommand formatNumberAccountingUK = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNumberAccountingUK);
		static readonly SpreadsheetUICommand formatNumberAccountingEuro = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNumberAccountingEuro);
		static readonly SpreadsheetUICommand formatNumberAccountingPRC = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNumberAccountingPRC);
		static readonly SpreadsheetUICommand formatNumberAccountingSwiss = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNumberAccountingSwiss);
		static readonly SpreadsheetUICommand formatNumberPercent = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNumberPercent);
		static readonly SpreadsheetUICommand formatNumberAccounting = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNumberAccounting);
		static readonly SpreadsheetUICommand formatNumberIncreaseDecimal = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNumberIncreaseDecimal);
		static readonly SpreadsheetUICommand formatNumberDecreaseDecimal = new SpreadsheetUICommand(SpreadsheetCommandId.FormatNumberDecreaseDecimal);
		static readonly SpreadsheetUICommand insertCellsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertCellsCommandGroup);
		static readonly SpreadsheetUICommand insertSheetRows = new SpreadsheetUICommand(SpreadsheetCommandId.InsertSheetRows);
		static readonly SpreadsheetUICommand insertSheetColumns = new SpreadsheetUICommand(SpreadsheetCommandId.InsertSheetColumns);
		static readonly SpreadsheetUICommand insertSheet = new SpreadsheetUICommand(SpreadsheetCommandId.InsertSheet);
		static readonly SpreadsheetUICommand removeCellsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.RemoveCellsCommandGroup);
		static readonly SpreadsheetUICommand removeSheetRows = new SpreadsheetUICommand(SpreadsheetCommandId.RemoveSheetRows);
		static readonly SpreadsheetUICommand removeSheetColumns = new SpreadsheetUICommand(SpreadsheetCommandId.RemoveSheetColumns);
		static readonly SpreadsheetUICommand removeSheet = new SpreadsheetUICommand(SpreadsheetCommandId.RemoveSheet);
		static readonly SpreadsheetUICommand formatCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FormatCommandGroup);
		static readonly SpreadsheetUICommand formatRowHeight = new SpreadsheetUICommand(SpreadsheetCommandId.FormatRowHeight);
		static readonly SpreadsheetUICommand formatAutoFitRowHeight = new SpreadsheetUICommand(SpreadsheetCommandId.FormatAutoFitRowHeight);
		static readonly SpreadsheetUICommand formatColumnWidth = new SpreadsheetUICommand(SpreadsheetCommandId.FormatColumnWidth);
		static readonly SpreadsheetUICommand formatAutoFitColumnWidth = new SpreadsheetUICommand(SpreadsheetCommandId.FormatAutoFitColumnWidth);
		static readonly SpreadsheetUICommand formatDefaultColumnWidth = new SpreadsheetUICommand(SpreadsheetCommandId.FormatDefaultColumnWidth);
		static readonly SpreadsheetUICommand formatHideAndUnhideCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.HideAndUnhideCommandGroup);
		static readonly SpreadsheetUICommand formatHideRows = new SpreadsheetUICommand(SpreadsheetCommandId.HideRows);
		static readonly SpreadsheetUICommand formatHideColumns = new SpreadsheetUICommand(SpreadsheetCommandId.HideColumns);
		static readonly SpreadsheetUICommand formatHideSheet = new SpreadsheetUICommand(SpreadsheetCommandId.HideSheet);
		static readonly SpreadsheetUICommand formatUnhideRows = new SpreadsheetUICommand(SpreadsheetCommandId.UnhideRows);
		static readonly SpreadsheetUICommand formatUnhideColumns = new SpreadsheetUICommand(SpreadsheetCommandId.UnhideColumns);
		static readonly SpreadsheetUICommand formatUnhideSheet = new SpreadsheetUICommand(SpreadsheetCommandId.UnhideSheet);
		static readonly SpreadsheetUICommand moveOrCopySheet = new SpreadsheetUICommand(SpreadsheetCommandId.MoveOrCopySheet);
		static readonly SpreadsheetUICommand renameSheet = new SpreadsheetUICommand(SpreadsheetCommandId.RenameSheet);
		static readonly SpreadsheetUICommand formatTabColor = new SpreadsheetUICommand(SpreadsheetCommandId.FormatTabColor);
		static readonly SpreadsheetUICommand editingMergeCellsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.EditingMergeCellsCommandGroup);
		static readonly SpreadsheetUICommand editingMergeAndCenterCells = new SpreadsheetUICommand(SpreadsheetCommandId.EditingMergeAndCenterCells);
		static readonly SpreadsheetUICommand editingMergeCellsAcross = new SpreadsheetUICommand(SpreadsheetCommandId.EditingMergeCellsAcross);
		static readonly SpreadsheetUICommand editingMergeCells = new SpreadsheetUICommand(SpreadsheetCommandId.EditingMergeCells);
		static readonly SpreadsheetUICommand editingUnmergeCells = new SpreadsheetUICommand(SpreadsheetCommandId.EditingUnmergeCells);
		static readonly SpreadsheetUICommand editingAutoSumCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.EditingAutoSumCommandGroup);
		static readonly SpreadsheetUICommand editingFillCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.EditingFillCommandGroup);
		static readonly SpreadsheetUICommand editingFillDown = new SpreadsheetUICommand(SpreadsheetCommandId.EditingFillDown);
		static readonly SpreadsheetUICommand editingFillRight = new SpreadsheetUICommand(SpreadsheetCommandId.EditingFillRight);
		static readonly SpreadsheetUICommand editingFillUp = new SpreadsheetUICommand(SpreadsheetCommandId.EditingFillUp);
		static readonly SpreadsheetUICommand editingFillLeft = new SpreadsheetUICommand(SpreadsheetCommandId.EditingFillLeft);
		static readonly SpreadsheetUICommand formatClearCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FormatClearCommandGroup);
		static readonly SpreadsheetUICommand formatClearAll = new SpreadsheetUICommand(SpreadsheetCommandId.FormatClearAll);
		static readonly SpreadsheetUICommand formatClearFormats = new SpreadsheetUICommand(SpreadsheetCommandId.FormatClearFormats);
		static readonly SpreadsheetUICommand formatClearContents = new SpreadsheetUICommand(SpreadsheetCommandId.FormatClearContents);
		static readonly SpreadsheetUICommand formatClearComments = new SpreadsheetUICommand(SpreadsheetCommandId.FormatClearComments);
		static readonly SpreadsheetUICommand formatClearHyperlinks = new SpreadsheetUICommand(SpreadsheetCommandId.FormatClearHyperlinks);
		static readonly SpreadsheetUICommand formatRemoveHyperlinks = new SpreadsheetUICommand(SpreadsheetCommandId.FormatRemoveHyperlinks);
		static readonly SpreadsheetUICommand editingSortAndFilterCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.EditingSortAndFilterCommandGroup);
		static readonly SpreadsheetUICommand editingFindAndSelectCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.EditingFindAndSelectCommandGroup);
		static readonly SpreadsheetUICommand editingFind = new SpreadsheetUICommand(SpreadsheetCommandId.EditingFind);
		static readonly SpreadsheetUICommand editingReplace = new SpreadsheetUICommand(SpreadsheetCommandId.EditingReplace);
		static readonly SpreadsheetUICommand editingSelectFormulas = new SpreadsheetUICommand(SpreadsheetCommandId.EditingSelectFormulas);
		static readonly SpreadsheetUICommand editingSelectComments = new SpreadsheetUICommand(SpreadsheetCommandId.EditingSelectComments);
		static readonly SpreadsheetUICommand editingSelectConditionalFormatting = new SpreadsheetUICommand(SpreadsheetCommandId.EditingSelectConditionalFormatting);
		static readonly SpreadsheetUICommand editingSelectConstants = new SpreadsheetUICommand(SpreadsheetCommandId.EditingSelectConstants);
		static readonly SpreadsheetUICommand editingSelectDataValidation = new SpreadsheetUICommand(SpreadsheetCommandId.EditingSelectDataValidation);
		static readonly SpreadsheetUICommand functionsAutoSumCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsAutoSumCommandGroup);
		static readonly SpreadsheetUICommand functionsInsertSum = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsInsertSum);
		static readonly SpreadsheetUICommand functionsInsertAverage = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsInsertAverage);
		static readonly SpreadsheetUICommand functionsInsertCountNumbers = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsInsertCountNumbers);
		static readonly SpreadsheetUICommand functionsInsertMax = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsInsertMax);
		static readonly SpreadsheetUICommand functionsInsertMin = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsInsertMin);
		static readonly SpreadsheetUICommand functionsFinancialCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsFinancialCommandGroup);
		static readonly SpreadsheetUICommand functionsLogicalCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsLogicalCommandGroup);
		static readonly SpreadsheetUICommand functionsTextCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsTextCommandGroup);
		static readonly SpreadsheetUICommand functionsDateAndTimeCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsDateAndTimeCommandGroup);
		static readonly SpreadsheetUICommand functionsLookupAndReferenceCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsLookupAndReferenceCommandGroup);
		static readonly SpreadsheetUICommand functionsMathAndTrigonometryCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsMathAndTrigonometryCommandGroup);
		static readonly SpreadsheetUICommand functionsStatisticalCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsStatisticalCommandGroup);
		static readonly SpreadsheetUICommand functionsEngineeringCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsEngineeringCommandGroup);
		static readonly SpreadsheetUICommand functionsCubeCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsCubeCommandGroup);
		static readonly SpreadsheetUICommand functionsInformationCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsInformationCommandGroup);
		static readonly SpreadsheetUICommand functionsCompatibilityCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FunctionsCompatibilityCommandGroup);
		static readonly SpreadsheetUICommand formulasCalculateNow = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasCalculateNow);
		static readonly SpreadsheetUICommand formulasCalculateSheet = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasCalculateSheet);
		static readonly SpreadsheetUICommand formulasCalculationOptionsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasCalculationOptionsCommandGroup);
		static readonly SpreadsheetUICommand formulasCalculationModeAutomatic = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasCalculationModeAutomatic);
		static readonly SpreadsheetUICommand formulasCalculationModeManual = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasCalculationModeManual);
		static readonly SpreadsheetUICommand formulasDefineNameCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasDefineNameCommandGroup);
		static readonly SpreadsheetUICommand formulasDefineNameCommand = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasDefineNameCommand);
		static readonly SpreadsheetUICommand formulasShowNameManager = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasShowNameManager);
		static readonly SpreadsheetUICommand formulasCreateDefinedNamesFromSelection = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasCreateDefinedNamesFromSelection);
		static readonly SpreadsheetUICommand formulasInsertDefinedNameCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasInsertDefinedNameCommandGroup);
		static readonly SpreadsheetUICommand formulasInsertDefinedName = new SpreadsheetUICommand(SpreadsheetCommandId.FormulasInsertDefinedName);
		static readonly SpreadsheetUICommand conditionalFormattingCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarsCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarsGradientFillCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarsGradientFillCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarGradientBlue = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarGradientBlue);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarGradientGreen = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarGradientGreen);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarGradientRed = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarGradientRed);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarGradientOrange = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarGradientOrange);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarGradientLightBlue = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarGradientLightBlue);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarGradientPurple = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarGradientPurple);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarsSolidFillCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarsSolidFillCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarSolidBlue = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarSolidBlue);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarSolidGreen = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarSolidGreen);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarSolidRed = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarSolidRed);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarSolidOrange = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarSolidOrange);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarSolidLightBlue = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarSolidLightBlue);
		static readonly SpreadsheetUICommand conditionalFormattingDataBarSolidPurple = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDataBarSolidPurple);
		static readonly SpreadsheetUICommand conditionalFormattingColorScalesCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScalesCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleGreenYellowRed = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleGreenYellowRed);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleRedYellowGreen = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleRedYellowGreen);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleGreenWhiteRed = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleGreenWhiteRed);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleRedWhiteGreen = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhiteGreen);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleBlueWhiteRed = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleBlueWhiteRed);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleRedWhiteBlue = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhiteBlue);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleWhiteRed = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleWhiteRed);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleRedWhite = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhite);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleGreenWhite = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleGreenWhite);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleWhiteGreen = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleWhiteGreen);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleGreenYellow = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleGreenYellow);
		static readonly SpreadsheetUICommand conditionalFormattingColorScaleYellowGreen = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingColorScaleYellowGreen);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetsCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetsDirectionalCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetsDirectionalCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetArrows3Colored = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetArrows3Colored);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetArrows3Grayed = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetArrows3Grayed);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetArrows4Colored = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetArrows4Colored);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetArrows4Grayed = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetArrows4Grayed);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetArrows5Colored = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetArrows5Colored);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetArrows5Grayed = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetArrows5Grayed);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetTriangles3 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetTriangles3);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetsShapesCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetsShapesCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetTrafficLights3 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights3);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetTrafficLights3Rimmed = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights3Rimmed);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetTrafficLights4 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights4);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetSigns3 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetSigns3);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetRedToBlack = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetRedToBlack);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetsIndicatorsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetsIndicatorsCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetSymbols3Circled = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetSymbols3Circled);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetSymbols3 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetSymbols3);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetFlags3 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetFlags3);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetsRatingsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetsRatingsCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetStars3 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetStars3);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetRatings4 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetRatings4);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetRatings5 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetRatings5);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetQuarters5 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetQuarters5);
		static readonly SpreadsheetUICommand conditionalFormattingIconSetBoxes5 = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingIconSetBoxes5);
		static readonly SpreadsheetUICommand conditionalFormattingHighlightCellsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingHighlightCellsRuleCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingGreaterThan = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingGreaterThanRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingLessThan = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingLessThanRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingBetween = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingBetweenRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingEqual = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingEqualToRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingTextContains = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingTextContainsRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingDateOccurring = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDateOccurringRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingDuplicateValues = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingDuplicateValuesRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingTopBottomCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingTopBottomRuleCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingTop10Items = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingTop10RuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingTop10Percent = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingTop10PercentRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingBottom10Items = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingBottom10RuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingBottom10Percent = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingBottom10PercentRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingAboveAverage = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingAboveAverageRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingBelowAverage = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingBelowAverageRuleCommand);
		static readonly SpreadsheetUICommand conditionalFormattingRemoveCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingRemoveCommandGroup);
		static readonly SpreadsheetUICommand conditionalFormattingRemoveFromSheet = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingRemoveFromSheet);
		static readonly SpreadsheetUICommand conditionalFormattingRemove = new SpreadsheetUICommand(SpreadsheetCommandId.ConditionalFormattingRemove);
		static readonly SpreadsheetUICommand insertPivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.InsertPivotTable);
		static readonly SpreadsheetUICommand insertTable = new SpreadsheetUICommand(SpreadsheetCommandId.InsertTable);
		static readonly SpreadsheetUICommand insertPicture = new SpreadsheetUICommand(SpreadsheetCommandId.InsertPicture);
		static readonly SpreadsheetUICommand insertSymbol = new SpreadsheetUICommand(SpreadsheetCommandId.InsertSymbol);
		static readonly SpreadsheetUICommand insertHyperlink = new SpreadsheetUICommand(SpreadsheetCommandId.InsertHyperlink);
		static readonly SpreadsheetUICommand viewZoomIn = new SpreadsheetUICommand(SpreadsheetCommandId.ViewZoomIn);
		static readonly SpreadsheetUICommand viewZoomOut = new SpreadsheetUICommand(SpreadsheetCommandId.ViewZoomOut);
		static readonly SpreadsheetUICommand viewZoom100Percent = new SpreadsheetUICommand(SpreadsheetCommandId.ViewZoom100Percent);
		static readonly SpreadsheetUICommand viewShowGridlines = new SpreadsheetUICommand(SpreadsheetCommandId.ViewShowGridlines);
		static readonly SpreadsheetUICommand viewShowHeadings = new SpreadsheetUICommand(SpreadsheetCommandId.ViewShowHeadings);
		static readonly SpreadsheetUICommand viewFreezePanesCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ViewFreezePanesCommandGroup);
		static readonly SpreadsheetUICommand viewFreezePanes = new SpreadsheetUICommand(SpreadsheetCommandId.ViewFreezePanes);
		static readonly SpreadsheetUICommand viewUnfreezePanes = new SpreadsheetUICommand(SpreadsheetCommandId.ViewUnfreezePanes);
		static readonly SpreadsheetUICommand viewFreezeTopRow = new SpreadsheetUICommand(SpreadsheetCommandId.ViewFreezeTopRow);
		static readonly SpreadsheetUICommand viewFreezeFirstColumn = new SpreadsheetUICommand(SpreadsheetCommandId.ViewFreezeFirstColumn);
		static readonly SpreadsheetUICommand viewShowFormulas = new SpreadsheetUICommand(SpreadsheetCommandId.ViewShowFormulas);
		static readonly SpreadsheetUICommand pageSetupMarginsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupMarginsCommandGroup);
		static readonly SpreadsheetUICommand pageSetupMarginsNormal = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupMarginsNormal);
		static readonly SpreadsheetUICommand pageSetupMarginsWide = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupMarginsWide);
		static readonly SpreadsheetUICommand pageSetupMarginsNarrow = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupMarginsNarrow);
		static readonly SpreadsheetUICommand pageSetupOrientationCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupOrientationCommandGroup);
		static readonly SpreadsheetUICommand pageSetupOrientationPortrait = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupOrientationPortrait);
		static readonly SpreadsheetUICommand pageSetupOrientationLandscape = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupOrientationLandscape);
		static readonly SpreadsheetUICommand pageSetupPaperKindCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupPaperKindCommandGroup);
		static readonly SpreadsheetUICommand pageSetupSetPrintArea = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupSetPrintArea);
		static readonly SpreadsheetUICommand pageSetupClearPrintArea = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupClearPrintArea);
		static readonly SpreadsheetUICommand pageSetupAddPrintArea = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupAddPrintArea);
		static readonly SpreadsheetUICommand pageSetupPrintAreaCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupPrintAreaCommandGroup);
		static readonly SpreadsheetUICommand pageSetupPrintGridlines = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupPrintGridlines);
		static readonly SpreadsheetUICommand pageSetupPrintHeadings = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupPrintHeadings);
		static readonly SpreadsheetUICommand arrangeBringForwardCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ArrangeBringForwardCommandGroup);
		static readonly SpreadsheetUICommand arrangeBringForward = new SpreadsheetUICommand(SpreadsheetCommandId.ArrangeBringForward);
		static readonly SpreadsheetUICommand arrangeBringToFront = new SpreadsheetUICommand(SpreadsheetCommandId.ArrangeBringToFront);
		static readonly SpreadsheetUICommand arrangeSendBackwardCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ArrangeSendBackwardCommandGroup);
		static readonly SpreadsheetUICommand arrangeSendBackward = new SpreadsheetUICommand(SpreadsheetCommandId.ArrangeSendBackward);
		static readonly SpreadsheetUICommand arrangeSendToBack = new SpreadsheetUICommand(SpreadsheetCommandId.ArrangeSendToBack);
		static readonly SpreadsheetUICommand dataSortAscending = new SpreadsheetUICommand(SpreadsheetCommandId.DataSortAscending);
		static readonly SpreadsheetUICommand dataSortDescending = new SpreadsheetUICommand(SpreadsheetCommandId.DataSortDescending);
		static readonly SpreadsheetUICommand dataFilterToggle = new SpreadsheetUICommand(SpreadsheetCommandId.DataFilterToggle);
		static readonly SpreadsheetUICommand dataFilterClear = new SpreadsheetUICommand(SpreadsheetCommandId.DataFilterClear);
		static readonly SpreadsheetUICommand dataFilterReApply = new SpreadsheetUICommand(SpreadsheetCommandId.DataFilterReApply);
		static readonly SpreadsheetUICommand dataValidationCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.DataToolsDataValidationCommandGroup);
		static readonly SpreadsheetUICommand dataValidation = new SpreadsheetUICommand(SpreadsheetCommandId.DataToolsDataValidation);
		static readonly SpreadsheetUICommand dataCircleValidationInvalidData = new SpreadsheetUICommand(SpreadsheetCommandId.DataToolsCircleInvalidData);
		static readonly SpreadsheetUICommand dataClearValidationCircles = new SpreadsheetUICommand(SpreadsheetCommandId.DataToolsClearValidationCircles);
		static readonly SpreadsheetUICommand dataOutlineGroupCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.OutlineGroupCommandGroup);
		static readonly SpreadsheetUICommand dataOutilneUngroupCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.OutlineUngroupCommandGroup);
		static readonly SpreadsheetUICommand dataOutlineSetting = new SpreadsheetUICommand(SpreadsheetCommandId.OutlineSettingsCommand);
		static readonly SpreadsheetUICommand dataGroupOutline = new SpreadsheetUICommand(SpreadsheetCommandId.GroupOutline);
		static readonly SpreadsheetUICommand dataAutoOutline = new SpreadsheetUICommand(SpreadsheetCommandId.AutoOutline);
		static readonly SpreadsheetUICommand dataUngroupOutline = new SpreadsheetUICommand(SpreadsheetCommandId.UngroupOutline);
		static readonly SpreadsheetUICommand dataClearOutline = new SpreadsheetUICommand(SpreadsheetCommandId.ClearOutline);
		static readonly SpreadsheetUICommand dataSubtotal = new SpreadsheetUICommand(SpreadsheetCommandId.Subtotal);
		static readonly SpreadsheetUICommand dataShowDetail = new SpreadsheetUICommand(SpreadsheetCommandId.ShowDetail);
		static readonly SpreadsheetUICommand dataHideDetail = new SpreadsheetUICommand(SpreadsheetCommandId.HideDetail);
		static readonly SpreadsheetUICommand insertChartColumnCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartColumnCommandGroup);
		static readonly SpreadsheetUICommand insertChartBarCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBarCommandGroup);
		static readonly SpreadsheetUICommand insertChartColumn2DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartColumn2DCommandGroup);
		static readonly SpreadsheetUICommand insertChartColumn3DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartColumn3DCommandGroup);
		static readonly SpreadsheetUICommand insertChartCylinderCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartCylinderCommandGroup);
		static readonly SpreadsheetUICommand insertChartConeCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartConeCommandGroup);
		static readonly SpreadsheetUICommand insertChartPyramidCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPyramidCommandGroup);
		static readonly SpreadsheetUICommand insertChartBar2DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBar2DCommandGroup);
		static readonly SpreadsheetUICommand insertChartBar3DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBar3DCommandGroup);
		static readonly SpreadsheetUICommand insertChartHorizontalCylinderCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalCylinderCommandGroup);
		static readonly SpreadsheetUICommand insertChartHorizontalConeCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalConeCommandGroup);
		static readonly SpreadsheetUICommand insertChartHorizontalPyramidCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalPyramidCommandGroup);
		static readonly SpreadsheetUICommand insertChartPieCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPieCommandGroup);
		static readonly SpreadsheetUICommand insertChartPie2DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPie2DCommandGroup);
		static readonly SpreadsheetUICommand insertChartPie3DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPie3DCommandGroup);
		static readonly SpreadsheetUICommand insertChartLineCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartLineCommandGroup);
		static readonly SpreadsheetUICommand insertChartLine2DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartLine2DCommandGroup);
		static readonly SpreadsheetUICommand insertChartLine3DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartLine3DCommandGroup);
		static readonly SpreadsheetUICommand insertChartAreaCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartAreaCommandGroup);
		static readonly SpreadsheetUICommand insertChartArea2DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartArea2DCommandGroup);
		static readonly SpreadsheetUICommand insertChartArea3DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartArea3DCommandGroup);
		static readonly SpreadsheetUICommand insertChartScatterCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartScatterCommandGroup);
		static readonly SpreadsheetUICommand insertChartBubbleCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBubbleCommandGroup);
		static readonly SpreadsheetUICommand insertChartDoughnut2DCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartDoughnut2DCommandGroup);
		static readonly SpreadsheetUICommand insertChartOtherCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartOtherCommandGroup);
		static readonly SpreadsheetUICommand insertChartStockCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartStockCommandGroup);
		static readonly SpreadsheetUICommand insertChartRadarCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartRadarCommandGroup);
		static readonly SpreadsheetUICommand insertChartColumnClustered2D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartColumnClustered2D);
		static readonly SpreadsheetUICommand insertChartColumnStacked2D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartColumnStacked2D);
		static readonly SpreadsheetUICommand insertChartColumnPercentStacked2D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartColumnPercentStacked2D);
		static readonly SpreadsheetUICommand insertChartColumnClustered3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartColumnClustered3D);
		static readonly SpreadsheetUICommand insertChartColumnStacked3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartColumnStacked3D);
		static readonly SpreadsheetUICommand insertChartColumnPercentStacked3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartColumnPercentStacked3D);
		static readonly SpreadsheetUICommand insertChartCylinderClustered = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartCylinderClustered);
		static readonly SpreadsheetUICommand insertChartCylinderStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartCylinderStacked);
		static readonly SpreadsheetUICommand insertChartCylinderPercentStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartCylinderPercentStacked);
		static readonly SpreadsheetUICommand insertChartConeClustered = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartConeClustered);
		static readonly SpreadsheetUICommand insertChartConeStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartConeStacked);
		static readonly SpreadsheetUICommand insertChartConePercentStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartConePercentStacked);
		static readonly SpreadsheetUICommand insertChartPyramidClustered = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPyramidClustered);
		static readonly SpreadsheetUICommand insertChartPyramidStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPyramidStacked);
		static readonly SpreadsheetUICommand insertChartPyramidPercentStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPyramidPercentStacked);
		static readonly SpreadsheetUICommand insertChartBarClustered2D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBarClustered2D);
		static readonly SpreadsheetUICommand insertChartBarStacked2D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBarStacked2D);
		static readonly SpreadsheetUICommand insertChartBarPercentStacked2D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBarPercentStacked2D);
		static readonly SpreadsheetUICommand insertChartBarClustered3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBarClustered3D);
		static readonly SpreadsheetUICommand insertChartBarStacked3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBarStacked3D);
		static readonly SpreadsheetUICommand insertChartBarPercentStacked3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBarPercentStacked3D);
		static readonly SpreadsheetUICommand insertChartHorizontalCylinderClustered = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalCylinderClustered);
		static readonly SpreadsheetUICommand insertChartHorizontalCylinderStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalCylinderStacked);
		static readonly SpreadsheetUICommand insertChartHorizontalCylinderPercentStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalCylinderPercentStacked);
		static readonly SpreadsheetUICommand insertChartHorizontalConeClustered = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalConeClustered);
		static readonly SpreadsheetUICommand insertChartHorizontalConeStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalConeStacked);
		static readonly SpreadsheetUICommand insertChartHorizontalConePercentStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalConePercentStacked);
		static readonly SpreadsheetUICommand insertChartHorizontalPyramidClustered = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalPyramidClustered);
		static readonly SpreadsheetUICommand insertChartHorizontalPyramidStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalPyramidStacked);
		static readonly SpreadsheetUICommand insertChartHorizontalPyramidPercentStacked = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartHorizontalPyramidPercentStacked);
		static readonly SpreadsheetUICommand insertChartColumn3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartColumn3D);
		static readonly SpreadsheetUICommand insertChartCylinder = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartCylinder);
		static readonly SpreadsheetUICommand insertChartCone = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartCone);
		static readonly SpreadsheetUICommand insertChartPyramid = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPyramid);
		static readonly SpreadsheetUICommand insertChartPie2D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPie2D);
		static readonly SpreadsheetUICommand insertChartPie3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPie3D);
		static readonly SpreadsheetUICommand insertChartPieExploded2D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPieExploded2D);
		static readonly SpreadsheetUICommand insertChartPieExploded3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPieExploded3D);
		static readonly SpreadsheetUICommand insertChartLine = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartLine);
		static readonly SpreadsheetUICommand insertChartStackedLine = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartStackedLine);
		static readonly SpreadsheetUICommand insertChartPercentStackedLine = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPercentStackedLine);
		static readonly SpreadsheetUICommand insertChartLineWithMarkers = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartLineWithMarkers);
		static readonly SpreadsheetUICommand insertChartStackedLineWithMarkers = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartStackedLineWithMarkers);
		static readonly SpreadsheetUICommand insertChartPercentStackedLineWithMarkers = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPercentStackedLineWithMarkers);
		static readonly SpreadsheetUICommand insertChartLine3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartLine3D);
		static readonly SpreadsheetUICommand insertChartArea = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartArea);
		static readonly SpreadsheetUICommand insertChartStackedArea = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartStackedArea);
		static readonly SpreadsheetUICommand insertChartPercentStackedArea = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPercentStackedArea);
		static readonly SpreadsheetUICommand insertChartArea3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartArea3D);
		static readonly SpreadsheetUICommand insertChartStackedArea3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartStackedArea3D);
		static readonly SpreadsheetUICommand insertChartPercentStackedArea3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartPercentStackedArea3D);
		static readonly SpreadsheetUICommand insertChartScatterMarkers = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartScatterMarkers);
		static readonly SpreadsheetUICommand insertChartScatterLines = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartScatterLines);
		static readonly SpreadsheetUICommand insertChartScatterSmoothLines = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartScatterSmoothLines);
		static readonly SpreadsheetUICommand insertChartScatterLinesAndMarkers = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartScatterLinesAndMarkers);
		static readonly SpreadsheetUICommand insertChartScatterSmoothLinesAndMarkers = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartScatterSmoothLinesAndMarkers);
		static readonly SpreadsheetUICommand insertChartBubble = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBubble);
		static readonly SpreadsheetUICommand insertChartBubble3D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartBubble3D);
		static readonly SpreadsheetUICommand insertChartDoughnut2D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartDoughnut2D);
		static readonly SpreadsheetUICommand insertChartDoughnutExploded2D = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartDoughnutExploded2D);
		static readonly SpreadsheetUICommand insertChartRadar = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartRadar);
		static readonly SpreadsheetUICommand insertChartRadarWithMarkers = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartRadarWithMarkers);
		static readonly SpreadsheetUICommand insertChartRadarFilled = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartRadarFilled);
		static readonly SpreadsheetUICommand insertChartStockHighLowClose = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartStockHighLowClose);
		static readonly SpreadsheetUICommand insertChartStockOpenHighLowClose = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartStockOpenHighLowClose);
		static readonly SpreadsheetUICommand insertChartStockVolumeHighLowClose = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartStockVolumeHighLowClose);
		static readonly SpreadsheetUICommand insertChartStockVolumeOpenHighLowClose = new SpreadsheetUICommand(SpreadsheetCommandId.InsertChartStockVolumeOpenHighLowClose);
		static readonly SpreadsheetUICommand chartSwitchRowColumn = new SpreadsheetUICommand(SpreadsheetCommandId.ChartSwitchRowColumn);
		static readonly SpreadsheetUICommand chartSelectData = new SpreadsheetUICommand(SpreadsheetCommandId.ChartSelectData);
		static readonly SpreadsheetUICommand chartTitleCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartTitleCommandGroup);
		static readonly SpreadsheetUICommand chartTitleNone = new SpreadsheetUICommand(SpreadsheetCommandId.ChartTitleNone);
		static readonly SpreadsheetUICommand chartTitleCenteredOverlay = new SpreadsheetUICommand(SpreadsheetCommandId.ChartTitleCenteredOverlay);
		static readonly SpreadsheetUICommand chartTitleAbove = new SpreadsheetUICommand(SpreadsheetCommandId.ChartTitleAbove);
		static readonly SpreadsheetUICommand chartAxisTitlesCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartAxisTitlesCommandGroup);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisTitleCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleCommandGroup);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisTitleCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleCommandGroup);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisTitleNone = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleNone);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisTitleBelow = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleBelow);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisTitleNone = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleNone);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisTitleRotated = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleRotated);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisTitleVertical = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleVertical);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisTitleHorizontal = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleHorizontal);
		static readonly SpreadsheetUICommand chartLegendCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartLegendCommandGroup);
		static readonly SpreadsheetUICommand chartLegendNone = new SpreadsheetUICommand(SpreadsheetCommandId.ChartLegendNone);
		static readonly SpreadsheetUICommand chartLegendAtRight = new SpreadsheetUICommand(SpreadsheetCommandId.ChartLegendAtRight);
		static readonly SpreadsheetUICommand chartLegendAtTop = new SpreadsheetUICommand(SpreadsheetCommandId.ChartLegendAtTop);
		static readonly SpreadsheetUICommand chartLegendAtLeft = new SpreadsheetUICommand(SpreadsheetCommandId.ChartLegendAtLeft);
		static readonly SpreadsheetUICommand chartLegendAtBottom = new SpreadsheetUICommand(SpreadsheetCommandId.ChartLegendAtBottom);
		static readonly SpreadsheetUICommand chartLegendOverlayAtRight = new SpreadsheetUICommand(SpreadsheetCommandId.ChartLegendOverlayAtRight);
		static readonly SpreadsheetUICommand chartLegendOverlayAtLeft = new SpreadsheetUICommand(SpreadsheetCommandId.ChartLegendOverlayAtLeft);
		static readonly SpreadsheetUICommand chartDataLabelsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsCommandGroup);
		static readonly SpreadsheetUICommand chartDataLabelsNone = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsNone);
		static readonly SpreadsheetUICommand chartDataLabelsDefault = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsDefault);
		static readonly SpreadsheetUICommand chartDataLabelsCenter = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsCenter);
		static readonly SpreadsheetUICommand chartDataLabelsInsideEnd = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsInsideEnd);
		static readonly SpreadsheetUICommand chartDataLabelsInsideBase = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsInsideBase);
		static readonly SpreadsheetUICommand chartDataLabelsOutsideEnd = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsOutsideEnd);
		static readonly SpreadsheetUICommand chartDataLabelsBestFit = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsBestFit);
		static readonly SpreadsheetUICommand chartDataLabelsLeft = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsLeft);
		static readonly SpreadsheetUICommand chartDataLabelsRight = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsRight);
		static readonly SpreadsheetUICommand chartDataLabelsAbove = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsAbove);
		static readonly SpreadsheetUICommand chartDataLabelsBelow = new SpreadsheetUICommand(SpreadsheetCommandId.ChartDataLabelsBelow);
		static readonly SpreadsheetUICommand chartAxesCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartAxesCommandGroup);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisCommandGroup);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisCommandGroup);
		static readonly SpreadsheetUICommand chartHidePrimaryHorizontalAxis = new SpreadsheetUICommand(SpreadsheetCommandId.ChartHidePrimaryHorizontalAxis);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisLeftToRight = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisLeftToRight);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisRightToLeft = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisRightToLeft);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisHideLabels = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisHideLabels);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisDefault = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisDefault);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisScaleLogarithm = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleLogarithm);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisScaleThousands = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleThousands);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisScaleMillions = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleMillions);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalAxisScaleBillions = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleBillions);
		static readonly SpreadsheetUICommand chartHidePrimaryVerticalAxis = new SpreadsheetUICommand(SpreadsheetCommandId.ChartHidePrimaryVerticalAxis);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisLeftToRight = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisLeftToRight);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisRightToLeft = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisRightToLeft);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisHideLabels = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisHideLabels);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisDefault = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisDefault);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisScaleLogarithm = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleLogarithm);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisScaleThousands = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleThousands);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisScaleMillions = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleMillions);
		static readonly SpreadsheetUICommand chartPrimaryVerticalAxisScaleBillions = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleBillions);
		static readonly SpreadsheetUICommand chartGridlinesCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartGridlinesCommandGroup);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalGridlinesCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesCommandGroup);
		static readonly SpreadsheetUICommand chartPrimaryVerticalGridlinesCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesCommandGroup);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalGridlinesNone = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesNone);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalGridlinesMajor = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMajor);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalGridlinesMinor = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMinor);
		static readonly SpreadsheetUICommand chartPrimaryHorizontalGridlinesMajorAndMinor = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMajorAndMinor);
		static readonly SpreadsheetUICommand chartPrimaryVerticalGridlinesNone = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesNone);
		static readonly SpreadsheetUICommand chartPrimaryVerticalGridlinesMajor = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMajor);
		static readonly SpreadsheetUICommand chartPrimaryVerticalGridlinesMinor = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMinor);
		static readonly SpreadsheetUICommand chartPrimaryVerticalGridlinesMajorAndMinor = new SpreadsheetUICommand(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMajorAndMinor);
		static readonly SpreadsheetUICommand chartLinesCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartLinesCommandGroup);
		static readonly SpreadsheetUICommand chartLinesNone = new SpreadsheetUICommand(SpreadsheetCommandId.ChartLinesNone);
		static readonly SpreadsheetUICommand chartShowDropLines = new SpreadsheetUICommand(SpreadsheetCommandId.ChartShowDropLines);
		static readonly SpreadsheetUICommand chartShowHighLowLines = new SpreadsheetUICommand(SpreadsheetCommandId.ChartShowHighLowLines);
		static readonly SpreadsheetUICommand chartShowDropLinesAndHighLowLines = new SpreadsheetUICommand(SpreadsheetCommandId.ChartShowDropLinesAndHighLowLines);
		static readonly SpreadsheetUICommand chartShowSeriesLines = new SpreadsheetUICommand(SpreadsheetCommandId.ChartShowSeriesLines);
		static readonly SpreadsheetUICommand chartUpDownBarsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartUpDownBarsCommandGroup);
		static readonly SpreadsheetUICommand chartHideUpDownBars = new SpreadsheetUICommand(SpreadsheetCommandId.ChartHideUpDownBars);
		static readonly SpreadsheetUICommand chartShowUpDownBars = new SpreadsheetUICommand(SpreadsheetCommandId.ChartShowUpDownBars);
		static readonly SpreadsheetUICommand chartErrorBarsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ChartErrorBarsCommandGroup);
		static readonly SpreadsheetUICommand chartErrorBarsNone = new SpreadsheetUICommand(SpreadsheetCommandId.ChartErrorBarsNone);
		static readonly SpreadsheetUICommand chartErrorBarsPercentage = new SpreadsheetUICommand(SpreadsheetCommandId.ChartErrorBarsPercentage);
		static readonly SpreadsheetUICommand chartErrorBarsStandardError = new SpreadsheetUICommand(SpreadsheetCommandId.ChartErrorBarsStandardError);
		static readonly SpreadsheetUICommand chartErrorBarsStandardDeviation = new SpreadsheetUICommand(SpreadsheetCommandId.ChartErrorBarsStandardDeviation);
		static readonly SpreadsheetUICommand toolsPictureCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ToolsPictureCommandGroup);
		static readonly SpreadsheetUICommand toolsDrawingCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ToolsDrawingCommandGroup);
		static readonly SpreadsheetUICommand toolsChartCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ToolsChartCommandGroup);
		static readonly SpreadsheetUICommand toolsPivotTableCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.ToolsPivotTableCommandGroup);
		static readonly SpreadsheetUICommand reviewProtectSheet = new SpreadsheetUICommand(SpreadsheetCommandId.ReviewProtectSheet);
		static readonly SpreadsheetUICommand reviewUnprotectSheet = new SpreadsheetUICommand(SpreadsheetCommandId.ReviewUnprotectSheet);
		static readonly SpreadsheetUICommand reviewProtectWorkbook = new SpreadsheetUICommand(SpreadsheetCommandId.ReviewProtectWorkbook);
		static readonly SpreadsheetUICommand reviewUnprotectWorkbook = new SpreadsheetUICommand(SpreadsheetCommandId.ReviewUnprotectWorkbook);
		static readonly SpreadsheetUICommand reviewShowProtectedRangeManager = new SpreadsheetUICommand(SpreadsheetCommandId.ReviewShowProtectedRangeManager);
		static readonly SpreadsheetUICommand reviewInsertComment = new SpreadsheetUICommand(SpreadsheetCommandId.ReviewInsertComment);
		static readonly SpreadsheetUICommand reviewEditComment = new SpreadsheetUICommand(SpreadsheetCommandId.ReviewEditComment);
		static readonly SpreadsheetUICommand reviewDeleteComment = new SpreadsheetUICommand(SpreadsheetCommandId.ReviewDeleteComment);
		static readonly SpreadsheetUICommand reviewShowHideComment = new SpreadsheetUICommand(SpreadsheetCommandId.ReviewShowHideComment);
		static readonly SpreadsheetUICommand tableToolsCommandGroup = new SpreadsheetUICommand(SpreadsheetCommandId.TableToolsCommandGroup);
		static readonly SpreadsheetUICommand tableConvertToRange = new SpreadsheetUICommand(SpreadsheetCommandId.TableToolsConvertToRange);
		static readonly SpreadsheetUICommand tableToggleHeaderRow = new SpreadsheetUICommand(SpreadsheetCommandId.TableToolsToggleHeaderRow);
		static readonly SpreadsheetUICommand tableToggleTotalRow = new SpreadsheetUICommand(SpreadsheetCommandId.TableToolsToggleTotalRow);
		static readonly SpreadsheetUICommand tableToggleBandedColumns = new SpreadsheetUICommand(SpreadsheetCommandId.TableToolsToggleBandedColumns);
		static readonly SpreadsheetUICommand tableToggleFirstColumn = new SpreadsheetUICommand(SpreadsheetCommandId.TableToolsToggleFirstColumn);
		static readonly SpreadsheetUICommand tableToggleLastColumn = new SpreadsheetUICommand(SpreadsheetCommandId.TableToolsToggleLastColumn);
		static readonly SpreadsheetUICommand tableToggleBandedRows = new SpreadsheetUICommand(SpreadsheetCommandId.TableToolsToggleBandedRows);
		static readonly SpreadsheetUICommand pageSetupPage = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetup);
		static readonly SpreadsheetUICommand pageSetupSheet = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupSheet);
		static readonly SpreadsheetUICommand pageSetupCustomMargins = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupCustomMargins);
		static readonly SpreadsheetUICommand pageSetupMorePaperSizes = new SpreadsheetUICommand(SpreadsheetCommandId.PageSetupMorePaperSizes);
		static readonly SpreadsheetUICommand optionsPivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.OptionsPivotTable);
		static readonly SpreadsheetUICommand optionsPivotTableContextMenuItem = new SpreadsheetUICommand(SpreadsheetCommandId.OptionsPivotTableContextMenuItem);
		static readonly SpreadsheetUICommand selectFieldTypePivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.SelectFieldTypePivotTable);
		static readonly SpreadsheetUICommand pivotTableExpandField = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableExpandField);
		static readonly SpreadsheetUICommand pivotTableCollapseField = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableCollapseField);
		static readonly SpreadsheetUICommand pivotTableDataRefreshGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableDataRefreshGroup);
		static readonly SpreadsheetUICommand refreshPivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.RefreshPivotTable);
		static readonly SpreadsheetUICommand refreshAllPivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.RefreshAllPivotTable);
		static readonly SpreadsheetUICommand changeDataSourcePivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.ChangeDataSourcePivotTable);
		static readonly SpreadsheetUICommand pivotTableActionsClearGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableActionsClearGroup);
		static readonly SpreadsheetUICommand clearAllPivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.ClearAllPivotTable);
		static readonly SpreadsheetUICommand clearFiltersPivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.ClearFiltersPivotTable);
		static readonly SpreadsheetUICommand pivotTableActionsSelectGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableActionsSelectGroup);
		static readonly SpreadsheetUICommand selectValuesPivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.SelectValuesPivotTable);
		static readonly SpreadsheetUICommand selectLabelsPivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.SelectLabelsPivotTable);
		static readonly SpreadsheetUICommand selectEntirePivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.SelectEntirePivotTable);
		static readonly SpreadsheetUICommand movePivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.MovePivotTable);
		static readonly SpreadsheetUICommand fieldListPanelPivotTable = new SpreadsheetUICommand(SpreadsheetCommandId.FieldListPanelPivotTable);
		static readonly SpreadsheetUICommand showPivotTableExpandCollapseButtons = new SpreadsheetUICommand(SpreadsheetCommandId.ShowPivotTableExpandCollapseButtons);
		static readonly SpreadsheetUICommand showPivotTableFieldHeaders = new SpreadsheetUICommand(SpreadsheetCommandId.ShowPivotTableFieldHeaders);
		static readonly SpreadsheetUICommand pivotTableLayoutSubtotalsGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableLayoutSubtotalsGroup);
		static readonly SpreadsheetUICommand pivotTableDoNotShowSubtotals = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableDoNotShowSubtotals);
		static readonly SpreadsheetUICommand pivotTableShowAllSubtotalsAtBottom = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableShowAllSubtotalsAtBottom);
		static readonly SpreadsheetUICommand pivotTableShowAllSubtotalsAtTop = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableShowAllSubtotalsAtTop);
		static readonly SpreadsheetUICommand pivotTableLayoutGrandTotalsGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableLayoutGrandTotalsGroup);
		static readonly SpreadsheetUICommand pivotTableGrandTotalsOffRowsColumns = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableGrandTotalsOffRowsColumns);
		static readonly SpreadsheetUICommand pivotTableGrandTotalsOnRowsColumns = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableGrandTotalsOnRowsColumns);
		static readonly SpreadsheetUICommand pivotTableGrandTotalsOnRowsOnly = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableGrandTotalsOnRowsOnly);
		static readonly SpreadsheetUICommand pivotTableGrandTotalsOnColumnsOnly = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableGrandTotalsOnColumnsOnly);
		static readonly SpreadsheetUICommand pivotTableLayoutReportLayoutGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableLayoutReportLayoutGroup);
		static readonly SpreadsheetUICommand pivotTableShowCompactForm = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableShowCompactForm);
		static readonly SpreadsheetUICommand pivotTableShowOutlineForm = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableShowOutlineForm);
		static readonly SpreadsheetUICommand pivotTableShowTabularForm = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableShowTabularForm);
		static readonly SpreadsheetUICommand pivotTableRepeatAllItemLabels = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableRepeatAllItemLabels);
		static readonly SpreadsheetUICommand pivotTableDoNotRepeatItemLabels = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableDoNotRepeatItemLabels);
		static readonly SpreadsheetUICommand pivotTableLayoutBlankRowsGroup = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableLayoutBlankRowsGroup);
		static readonly SpreadsheetUICommand pivotTableInsertBlankLineEachItem = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableInsertBlankLineEachItem);
		static readonly SpreadsheetUICommand pivotTableRemoveBlankLineEachItem = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableRemoveBlankLineEachItem);
		static readonly SpreadsheetUICommand pivotTableToggleRowHeaders = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableToggleRowHeaders);
		static readonly SpreadsheetUICommand pivotTableToggleColumnHeaders = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableToggleColumnHeaders);
		static readonly SpreadsheetUICommand pivotTableToggleBandedRows = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableToggleBandedRows);
		static readonly SpreadsheetUICommand pivotTableToggleBandedColumns = new SpreadsheetUICommand(SpreadsheetCommandId.PivotTableToggleBandedColumns);
		public static SpreadsheetUICommand FileNew { get { return fileNew; } }
		public static SpreadsheetUICommand FileOpen { get { return fileOpen; } }
		public static SpreadsheetUICommand FileSave { get { return fileSave; } }
		public static SpreadsheetUICommand FileSaveAs { get { return fileSaveAs; } }
		public static SpreadsheetUICommand FileQuickPrint { get { return fileQuickPrint; } }
		public static SpreadsheetUICommand FilePrint { get { return filePrint; } }
		public static SpreadsheetUICommand FilePrintPreview { get { return filePrintPreview; } }
		public static SpreadsheetUICommand FileUndo { get { return fileUndo; } }
		public static SpreadsheetUICommand FileRedo { get { return fileRedo; } }
		public static SpreadsheetUICommand FileShowDocumentProperties { get { return fileShowDocumentProperties; } }
		public static SpreadsheetUICommand EditCut { get { return editCut; } }
		public static SpreadsheetUICommand EditCopy { get { return editCopy; } }
		public static SpreadsheetUICommand EditPaste { get { return editPaste; } }
		public static SpreadsheetUICommand EditPasteSpecial { get { return editPasteSpecial; } }
		public static SpreadsheetUICommand FormatCells { get { return formatCells; } }
		public static SpreadsheetUICommand FormatCellsFont { get { return formatCellsFont; } }
		public static SpreadsheetUICommand FormatCellsAlignment { get { return formatCellsAlignment; } }
		public static SpreadsheetUICommand FormatCellsNumber { get { return formatCellsNumber; } }
		public static SpreadsheetUICommand FormatFontName { get { return formatFontName; } }
		public static SpreadsheetUICommand FormatFontSize { get { return formatFontSize; } }
		public static SpreadsheetUICommand FormatFontColor { get { return formatFontColor; } }
		public static SpreadsheetUICommand FormatFillColor { get { return formatFillColor; } }
		public static SpreadsheetUICommand FormatFontBold { get { return formatFontBold; } }
		public static SpreadsheetUICommand FormatFontItalic { get { return formatFontItalic; } }
		public static SpreadsheetUICommand FormatFontUnderline { get { return formatFontUnderline; } }
		public static SpreadsheetUICommand FormatFontStrikeout { get { return formatFontStrikeout; } }
		public static SpreadsheetUICommand FormatIncreaseFontSize { get { return formatIncreaseFontSize; } }
		public static SpreadsheetUICommand FormatDecreaseFontSize { get { return formatDecreaseFontSize; } }
		public static SpreadsheetUICommand FormatBordersCommandGroup { get { return formatBordersCommandGroup; } }
		public static SpreadsheetUICommand FormatBottomBorder { get { return formatBottomBorder; } }
		public static SpreadsheetUICommand FormatTopBorder { get { return formatTopBorder; } }
		public static SpreadsheetUICommand FormatLeftBorder { get { return formatLeftBorder; } }
		public static SpreadsheetUICommand FormatRightBorder { get { return formatRightBorder; } }
		public static SpreadsheetUICommand FormatNoBorders { get { return formatNoBorders; } }
		public static SpreadsheetUICommand FormatAllBorders { get { return formatAllBorders; } }
		public static SpreadsheetUICommand FormatOutsideBorders { get { return formatOutsideBorders; } }
		public static SpreadsheetUICommand FormatThickBorder { get { return formatThickBorder; } }
		public static SpreadsheetUICommand FormatBottomDoubleBorder { get { return formatBottomDoubleBorder; } }
		public static SpreadsheetUICommand FormatBottomThickBorder { get { return formatBottomThickBorder; } }
		public static SpreadsheetUICommand FormatTopAndBottomBorder { get { return formatTopAndBottomBorder; } }
		public static SpreadsheetUICommand FormatTopAndThickBottomBorder { get { return formatTopAndThickBottomBorder; } }
		public static SpreadsheetUICommand FormatTopAndDoubleBottomBorder { get { return formatTopAndDoubleBottomBorder; } }
		public static SpreadsheetUICommand FormatCellLocked { get { return formatCellLocked; } }
		public static SpreadsheetUICommand FormatAlignmentTop { get { return formatAlignmentTop; } }
		public static SpreadsheetUICommand FormatAlignmentMiddle { get { return formatAlignmentMiddle; } }
		public static SpreadsheetUICommand FormatAlignmentBottom { get { return formatAlignmentBottom; } }
		public static SpreadsheetUICommand FormatAlignmentLeft { get { return formatAlignmentLeft; } }
		public static SpreadsheetUICommand FormatAlignmentCenter { get { return formatAlignmentCenter; } }
		public static SpreadsheetUICommand FormatAlignmentRight { get { return formatAlignmentRight; } }
		public static SpreadsheetUICommand FormatDecreaseIndent { get { return formatDecreaseIndent; } }
		public static SpreadsheetUICommand FormatIncreaseIndent { get { return formatIncreaseIndent; } }
		public static SpreadsheetUICommand FormatWrapText { get { return formatWrapText; } }
		public static SpreadsheetUICommand FormatNumberAccountingCommandGroup { get { return formatNumberAccountingCommandGroup; } }
		public static SpreadsheetUICommand FormatNumberAccountingUS { get { return formatNumberAccountingUS; } }
		public static SpreadsheetUICommand FormatNumberAccountingUK { get { return formatNumberAccountingUK; } }
		public static SpreadsheetUICommand FormatNumberAccountingEuro { get { return formatNumberAccountingEuro; } }
		public static SpreadsheetUICommand FormatNumberAccountingPRC { get { return formatNumberAccountingPRC; } }
		public static SpreadsheetUICommand FormatNumberAccountingSwiss { get { return formatNumberAccountingSwiss; } }
		public static SpreadsheetUICommand FormatNumberPercent { get { return formatNumberPercent; } }
		public static SpreadsheetUICommand FormatNumberAccounting { get { return formatNumberAccounting; } }
		public static SpreadsheetUICommand FormatNumberIncreaseDecimal { get { return formatNumberIncreaseDecimal; } }
		public static SpreadsheetUICommand FormatNumberDecreaseDecimal { get { return formatNumberDecreaseDecimal; } }
		public static SpreadsheetUICommand InsertCellsCommandGroup { get { return insertCellsCommandGroup; } }
		public static SpreadsheetUICommand InsertSheetRows { get { return insertSheetRows; } }
		public static SpreadsheetUICommand InsertSheetColumns { get { return insertSheetColumns; } }
		public static SpreadsheetUICommand InsertSheet { get { return insertSheet; } }
		public static SpreadsheetUICommand RemoveCellsCommandGroup { get { return removeCellsCommandGroup; } }
		public static SpreadsheetUICommand RemoveSheetRows { get { return removeSheetRows; } }
		public static SpreadsheetUICommand RemoveSheetColumns { get { return removeSheetColumns; } }
		public static SpreadsheetUICommand RemoveSheet { get { return removeSheet; } }
		public static SpreadsheetUICommand FormatCommandGroup { get { return formatCommandGroup; } }
		public static SpreadsheetUICommand FormatRowHeight { get { return formatRowHeight; } }
		public static SpreadsheetUICommand FormatAutoFitRowHeight { get { return formatAutoFitRowHeight; } }
		public static SpreadsheetUICommand FormatColumnWidth { get { return formatColumnWidth; } }
		public static SpreadsheetUICommand FormatAutoFitColumnWidth { get { return formatAutoFitColumnWidth; } }
		public static SpreadsheetUICommand FormatDefaultColumnWidth { get { return formatDefaultColumnWidth; } }
		public static SpreadsheetUICommand FormatHideAndUnhideCommandGroup { get { return formatHideAndUnhideCommandGroup; } }
		public static SpreadsheetUICommand FormatHideRows { get { return formatHideRows; } }
		public static SpreadsheetUICommand FormatHideColumns { get { return formatHideColumns; } }
		public static SpreadsheetUICommand FormatHideSheet { get { return formatHideSheet; } }
		public static SpreadsheetUICommand FormatUnhideRows { get { return formatUnhideRows; } }
		public static SpreadsheetUICommand FormatUnhideColumns { get { return formatUnhideColumns; } }
		public static SpreadsheetUICommand FormatUnhideSheet { get { return formatUnhideSheet; } }
		public static SpreadsheetUICommand MoveOrCopySheet { get { return moveOrCopySheet; } }
		public static SpreadsheetUICommand RenameSheet { get { return renameSheet; } }
		public static SpreadsheetUICommand FormatTabColor { get { return formatTabColor; } }
		public static SpreadsheetUICommand EditingMergeCellsCommandGroup { get { return editingMergeCellsCommandGroup; } }
		public static SpreadsheetUICommand EditingMergeAndCenterCells { get { return editingMergeAndCenterCells; } }
		public static SpreadsheetUICommand EditingMergeCellsAcross { get { return editingMergeCellsAcross; } }
		public static SpreadsheetUICommand EditingMergeCells { get { return editingMergeCells; } }
		public static SpreadsheetUICommand EditingUnmergeCells { get { return editingUnmergeCells; } }
		public static SpreadsheetUICommand EditingAutoSumCommandGroup { get { return editingAutoSumCommandGroup; } }
		public static SpreadsheetUICommand EditingFillCommandGroup { get { return editingFillCommandGroup; } }
		public static SpreadsheetUICommand EditingFillDown { get { return editingFillDown; } }
		public static SpreadsheetUICommand EditingFillRight { get { return editingFillRight; } }
		public static SpreadsheetUICommand EditingFillUp { get { return editingFillUp; } }
		public static SpreadsheetUICommand EditingFillLeft { get { return editingFillLeft; } }
		public static SpreadsheetUICommand FormatClearCommandGroup { get { return formatClearCommandGroup; } }
		public static SpreadsheetUICommand FormatClearAll { get { return formatClearAll; } }
		public static SpreadsheetUICommand FormatClearFormats { get { return formatClearFormats; } }
		public static SpreadsheetUICommand FormatClearContents { get { return formatClearContents; } }
		public static SpreadsheetUICommand FormatClearComments { get { return formatClearComments; } }
		public static SpreadsheetUICommand FormatClearHyperlinks { get { return formatClearHyperlinks; } }
		public static SpreadsheetUICommand FormatRemoveHyperlinks { get { return formatRemoveHyperlinks; } }
		public static SpreadsheetUICommand EditingSortAndFilterCommandGroup { get { return editingSortAndFilterCommandGroup; } }
		public static SpreadsheetUICommand EditingFindAndSelectCommandGroup { get { return editingFindAndSelectCommandGroup; } }
		public static SpreadsheetUICommand EditingFind { get { return editingFind; } }
		public static SpreadsheetUICommand EditingReplace { get { return editingReplace; } }
		public static SpreadsheetUICommand EditingSelectFormulas { get { return editingSelectFormulas; } }
		public static SpreadsheetUICommand EditingSelectComments { get { return editingSelectComments; } }
		public static SpreadsheetUICommand EditingSelectConditionalFormatting { get { return editingSelectConditionalFormatting; } }
		public static SpreadsheetUICommand EditingSelectConstants { get { return editingSelectConstants; } }
		public static SpreadsheetUICommand EditingSelectDataValidation { get { return editingSelectDataValidation; } }
		public static SpreadsheetUICommand FunctionsAutoSumCommandGroup { get { return functionsAutoSumCommandGroup; } }
		public static SpreadsheetUICommand FunctionsInsertSum { get { return functionsInsertSum; } }
		public static SpreadsheetUICommand FunctionsInsertAverage { get { return functionsInsertAverage; } }
		public static SpreadsheetUICommand FunctionsInsertCountNumbers { get { return functionsInsertCountNumbers; } }
		public static SpreadsheetUICommand FunctionsInsertMax { get { return functionsInsertMax; } }
		public static SpreadsheetUICommand FunctionsInsertMin { get { return functionsInsertMin; } }
		public static SpreadsheetUICommand FunctionsFinancialCommandGroup { get { return functionsFinancialCommandGroup; } }
		public static SpreadsheetUICommand FunctionsLogicalCommandGroup { get { return functionsLogicalCommandGroup; } }
		public static SpreadsheetUICommand FunctionsTextCommandGroup { get { return functionsTextCommandGroup; } }
		public static SpreadsheetUICommand FunctionsDateAndTimeCommandGroup { get { return functionsDateAndTimeCommandGroup; } }
		public static SpreadsheetUICommand FunctionsLookupAndReferenceCommandGroup { get { return functionsLookupAndReferenceCommandGroup; } }
		public static SpreadsheetUICommand FunctionsMathAndTrigonometryCommandGroup { get { return functionsMathAndTrigonometryCommandGroup; } }
		public static SpreadsheetUICommand FunctionsStatisticalCommandGroup { get { return functionsStatisticalCommandGroup; } }
		public static SpreadsheetUICommand FunctionsEngineeringCommandGroup { get { return functionsEngineeringCommandGroup; } }
		public static SpreadsheetUICommand FunctionsCubeCommandGroup { get { return functionsCubeCommandGroup; } }
		public static SpreadsheetUICommand FunctionsInformationCommandGroup { get { return functionsInformationCommandGroup; } }
		public static SpreadsheetUICommand FunctionsCompatibilityCommandGroup { get { return functionsCompatibilityCommandGroup; } }
		public static SpreadsheetUICommand FormulasCalculateNow { get { return formulasCalculateNow; } }
		public static SpreadsheetUICommand FormulasCalculateSheet { get { return formulasCalculateSheet; } }
		public static SpreadsheetUICommand FormulasCalculationOptionsCommandGroup { get { return formulasCalculationOptionsCommandGroup; } }
		public static SpreadsheetUICommand FormulasCalculationModeAutomatic { get { return formulasCalculationModeAutomatic; } }
		public static SpreadsheetUICommand FormulasCalculationModeManual { get { return formulasCalculationModeManual; } }
		public static SpreadsheetUICommand FormulasDefineNameCommandGroup { get { return formulasDefineNameCommandGroup; } }
		public static SpreadsheetUICommand FormulasDefineNameCommand { get { return formulasDefineNameCommand; } }
		public static SpreadsheetUICommand FormulasShowNameManager { get { return formulasShowNameManager; } }
		public static SpreadsheetUICommand FormulasCreateDefinedNamesFromSelection { get { return formulasCreateDefinedNamesFromSelection; } }
		public static SpreadsheetUICommand FormulasInsertDefinedNameCommandGroup { get { return formulasInsertDefinedNameCommandGroup; } }
		public static SpreadsheetUICommand FormulasInsertDefinedName { get { return formulasInsertDefinedName; } }
		public static SpreadsheetUICommand ConditionalFormattingCommandGroup { get { return conditionalFormattingCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarsCommandGroup { get { return conditionalFormattingDataBarsCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarsGradientFillCommandGroup { get { return conditionalFormattingDataBarsGradientFillCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarGradientBlue { get { return conditionalFormattingDataBarGradientBlue; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarGradientGreen { get { return conditionalFormattingDataBarGradientGreen; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarGradientRed { get { return conditionalFormattingDataBarGradientRed; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarGradientOrange { get { return conditionalFormattingDataBarGradientOrange; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarGradientLightBlue { get { return conditionalFormattingDataBarGradientLightBlue; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarGradientPurple { get { return conditionalFormattingDataBarGradientPurple; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarsSolidFillCommandGroup { get { return conditionalFormattingDataBarsSolidFillCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarSolidBlue { get { return conditionalFormattingDataBarSolidBlue; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarSolidGreen { get { return conditionalFormattingDataBarSolidGreen; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarSolidRed { get { return conditionalFormattingDataBarSolidRed; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarSolidOrange { get { return conditionalFormattingDataBarSolidOrange; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarSolidLightBlue { get { return conditionalFormattingDataBarSolidLightBlue; } }
		public static SpreadsheetUICommand ConditionalFormattingDataBarSolidPurple { get { return conditionalFormattingDataBarSolidPurple; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScalesCommandGroup { get { return conditionalFormattingColorScalesCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleGreenYellowRed { get { return conditionalFormattingColorScaleGreenYellowRed; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleRedYellowGreen { get { return conditionalFormattingColorScaleRedYellowGreen; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleGreenWhiteRed { get { return conditionalFormattingColorScaleGreenWhiteRed; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleRedWhiteGreen { get { return conditionalFormattingColorScaleRedWhiteGreen; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleBlueWhiteRed { get { return conditionalFormattingColorScaleBlueWhiteRed; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleRedWhiteBlue { get { return conditionalFormattingColorScaleRedWhiteBlue; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleWhiteRed { get { return conditionalFormattingColorScaleWhiteRed; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleRedWhite { get { return conditionalFormattingColorScaleRedWhite; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleGreenWhite { get { return conditionalFormattingColorScaleGreenWhite; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleWhiteGreen { get { return conditionalFormattingColorScaleWhiteGreen; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleGreenYellow { get { return conditionalFormattingColorScaleGreenYellow; } }
		public static SpreadsheetUICommand ConditionalFormattingColorScaleYellowGreen { get { return conditionalFormattingColorScaleYellowGreen; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetsCommandGroup { get { return conditionalFormattingIconSetsCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetsDirectionalCommandGroup { get { return conditionalFormattingIconSetsDirectionalCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetArrows3Colored { get { return conditionalFormattingIconSetArrows3Colored; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetArrows3Grayed { get { return conditionalFormattingIconSetArrows3Grayed; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetArrows4Colored { get { return conditionalFormattingIconSetArrows4Colored; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetArrows4Grayed { get { return conditionalFormattingIconSetArrows4Grayed; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetArrows5Colored { get { return conditionalFormattingIconSetArrows5Colored; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetArrows5Grayed { get { return conditionalFormattingIconSetArrows5Grayed; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetTriangles3 { get { return conditionalFormattingIconSetTriangles3; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetsShapesCommandGroup { get { return conditionalFormattingIconSetsShapesCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetTrafficLights3 { get { return conditionalFormattingIconSetTrafficLights3; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetTrafficLights3Rimmed { get { return conditionalFormattingIconSetTrafficLights3Rimmed; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetTrafficLights4 { get { return conditionalFormattingIconSetTrafficLights4; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetSigns3 { get { return conditionalFormattingIconSetSigns3; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetRedToBlack { get { return conditionalFormattingIconSetRedToBlack; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetsIndicatorsCommandGroup { get { return conditionalFormattingIconSetsIndicatorsCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetSymbols3Circled { get { return conditionalFormattingIconSetSymbols3Circled; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetSymbols3 { get { return conditionalFormattingIconSetSymbols3; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetFlags3 { get { return conditionalFormattingIconSetFlags3; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetsRatingsCommandGroup { get { return conditionalFormattingIconSetsRatingsCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetStars3 { get { return conditionalFormattingIconSetStars3; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetRatings4 { get { return conditionalFormattingIconSetRatings4; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetRatings5 { get { return conditionalFormattingIconSetRatings5; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetQuarters5 { get { return conditionalFormattingIconSetQuarters5; } }
		public static SpreadsheetUICommand ConditionalFormattingIconSetBoxes5 { get { return conditionalFormattingIconSetBoxes5; } }
		public static SpreadsheetUICommand ConditionalFormattingHighlightCellsCommandGroup { get { return conditionalFormattingHighlightCellsCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingGreaterThan { get { return conditionalFormattingGreaterThan; } }
		public static SpreadsheetUICommand ConditionalFormattingLessThan { get { return conditionalFormattingLessThan; } }
		public static SpreadsheetUICommand ConditionalFormattingBetween { get { return conditionalFormattingBetween; } }
		public static SpreadsheetUICommand ConditionalFormattingEqual { get { return conditionalFormattingEqual; } }
		public static SpreadsheetUICommand ConditionalFormattingTextContains { get { return conditionalFormattingTextContains; } }
		public static SpreadsheetUICommand ConditionalFormattingDateOccurring { get { return conditionalFormattingDateOccurring; } }
		public static SpreadsheetUICommand ConditionalFormattingDuplicateValues { get { return conditionalFormattingDuplicateValues; } }
		public static SpreadsheetUICommand ConditionalFormattingTopBottomCommandGroup { get { return conditionalFormattingTopBottomCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingTop10Items { get { return conditionalFormattingTop10Items; } }
		public static SpreadsheetUICommand ConditionalFormattingTop10Percent { get { return conditionalFormattingTop10Percent; } }
		public static SpreadsheetUICommand ConditionalFormattingBottom10Items { get { return conditionalFormattingBottom10Items; } }
		public static SpreadsheetUICommand ConditionalFormattingBottom10Percent { get { return conditionalFormattingBottom10Percent; } }
		public static SpreadsheetUICommand ConditionalFormattingAboveAverage { get { return conditionalFormattingAboveAverage; } }
		public static SpreadsheetUICommand ConditionalFormattingBelowAverage { get { return conditionalFormattingBelowAverage; } }
		public static SpreadsheetUICommand ConditionalFormattingRemoveCommandGroup { get { return conditionalFormattingRemoveCommandGroup; } }
		public static SpreadsheetUICommand ConditionalFormattingRemoveFromSheet { get { return conditionalFormattingRemoveFromSheet; } }
		public static SpreadsheetUICommand ConditionalFormattingRemove { get { return conditionalFormattingRemove; } }
		public static SpreadsheetUICommand InsertPivotTable { get { return insertPivotTable; } }
		public static SpreadsheetUICommand InsertTable { get { return insertTable; } }
		public static SpreadsheetUICommand InsertPicture { get { return insertPicture; } }
		public static SpreadsheetUICommand InsertSymbol { get { return insertSymbol; } }
		public static SpreadsheetUICommand InsertHyperlink { get { return insertHyperlink; } }
		public static SpreadsheetUICommand ViewZoomIn { get { return viewZoomIn; } }
		public static SpreadsheetUICommand ViewZoomOut { get { return viewZoomOut; } }
		public static SpreadsheetUICommand ViewZoom100Percent { get { return viewZoom100Percent; } }
		public static SpreadsheetUICommand ViewShowGridlines { get { return viewShowGridlines; } }
		public static SpreadsheetUICommand ViewShowHeadings { get { return viewShowHeadings; } }
		public static SpreadsheetUICommand ViewFreezePanesCommandGroup { get { return viewFreezePanesCommandGroup; } }
		public static SpreadsheetUICommand ViewFreezePanes { get { return viewFreezePanes; } }
		public static SpreadsheetUICommand ViewUnfreezePanes { get { return viewUnfreezePanes; } }
		public static SpreadsheetUICommand ViewFreezeTopRow { get { return viewFreezeTopRow; } }
		public static SpreadsheetUICommand ViewFreezeFirstColumn { get { return viewFreezeFirstColumn; } }
		public static SpreadsheetUICommand ViewShowFormulas { get { return viewShowFormulas; } }
		public static SpreadsheetUICommand PageSetupMarginsCommandGroup { get { return pageSetupMarginsCommandGroup; } }
		public static SpreadsheetUICommand PageSetupMarginsNormal { get { return pageSetupMarginsNormal; } }
		public static SpreadsheetUICommand PageSetupMarginsWide { get { return pageSetupMarginsWide; } }
		public static SpreadsheetUICommand PageSetupMarginsNarrow { get { return pageSetupMarginsNarrow; } }
		public static SpreadsheetUICommand PageSetupOrientationCommandGroup { get { return pageSetupOrientationCommandGroup; } }
		public static SpreadsheetUICommand PageSetupOrientationPortrait { get { return pageSetupOrientationPortrait; } }
		public static SpreadsheetUICommand PageSetupOrientationLandscape { get { return pageSetupOrientationLandscape; } }
		public static SpreadsheetUICommand PageSetupPaperKindCommandGroup { get { return pageSetupPaperKindCommandGroup; } }
		public static SpreadsheetUICommand PageSetupSetPrintArea { get { return pageSetupSetPrintArea; } }
		public static SpreadsheetUICommand PageSetupClearPrintArea { get { return pageSetupClearPrintArea; } }
		public static SpreadsheetUICommand PageSetupAddPrintArea { get { return pageSetupAddPrintArea; } }
		public static SpreadsheetUICommand PageSetupPrintAreaCommandGroup { get { return pageSetupPrintAreaCommandGroup; } }
		public static SpreadsheetUICommand PageSetupPrintGridlines { get { return pageSetupPrintGridlines; } }
		public static SpreadsheetUICommand PageSetupPrintHeadings { get { return pageSetupPrintHeadings; } }
		public static SpreadsheetUICommand ArrangeBringForwardCommandGroup { get { return arrangeBringForwardCommandGroup; } }
		public static SpreadsheetUICommand ArrangeBringForward { get { return arrangeBringForward; } }
		public static SpreadsheetUICommand ArrangeBringToFront { get { return arrangeBringToFront; } }
		public static SpreadsheetUICommand ArrangeSendBackwardCommandGroup { get { return arrangeSendBackwardCommandGroup; } }
		public static SpreadsheetUICommand ArrangeSendBackward { get { return arrangeSendBackward; } }
		public static SpreadsheetUICommand ArrangeSendToBack { get { return arrangeSendToBack; } }
		public static SpreadsheetUICommand DataSortAscending { get { return dataSortAscending; } }
		public static SpreadsheetUICommand DataSortDescending { get { return dataSortDescending; } }
		public static SpreadsheetUICommand DataFilterToggle { get { return dataFilterToggle; } }
		public static SpreadsheetUICommand DataFilterClear { get { return dataFilterClear; } }
		public static SpreadsheetUICommand DataFilterReApply { get { return dataFilterReApply; } }
		public static SpreadsheetUICommand DataValidationCommandGroup { get { return dataValidationCommandGroup; } }
		public static SpreadsheetUICommand DataValidation { get { return dataValidation; } }
		public static SpreadsheetUICommand DataCircleValidationInvalidData { get { return dataCircleValidationInvalidData; } }
		public static SpreadsheetUICommand DataClearValidationCircles { get { return dataClearValidationCircles; } }
		public static SpreadsheetUICommand DataOutlineGroupCommandGroup { get { return dataOutlineGroupCommandGroup; } }
		public static SpreadsheetUICommand DataOutlineUngroupCommandGroup { get { return dataOutilneUngroupCommandGroup; } }
		public static SpreadsheetUICommand DataOutlineSetting { get { return dataOutlineSetting; } }
		public static SpreadsheetUICommand DataGroupOutline { get { return dataGroupOutline; } }
		public static SpreadsheetUICommand DataAutoOutline { get { return dataAutoOutline; } }
		public static SpreadsheetUICommand DataUngroupOutline { get { return dataUngroupOutline; } }
		public static SpreadsheetUICommand DataClearOutline { get { return dataClearOutline; } }
		public static SpreadsheetUICommand DataSubtotal { get { return dataSubtotal; } }
		public static SpreadsheetUICommand DataShowDetail { get { return dataShowDetail; } }
		public static SpreadsheetUICommand DataHideDetail { get { return dataHideDetail; } }
		public static SpreadsheetUICommand InsertChartColumnCommandGroup { get { return insertChartColumnCommandGroup; } }
		public static SpreadsheetUICommand InsertChartBarCommandGroup { get { return insertChartBarCommandGroup; } }
		public static SpreadsheetUICommand InsertChartColumn2DCommandGroup { get { return insertChartColumn2DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartColumn3DCommandGroup { get { return insertChartColumn3DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartCylinderCommandGroup { get { return insertChartCylinderCommandGroup; } }
		public static SpreadsheetUICommand InsertChartConeCommandGroup { get { return insertChartConeCommandGroup; } }
		public static SpreadsheetUICommand InsertChartPyramidCommandGroup { get { return insertChartPyramidCommandGroup; } }
		public static SpreadsheetUICommand InsertChartBar2DCommandGroup { get { return insertChartBar2DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartBar3DCommandGroup { get { return insertChartBar3DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartHorizontalCylinderCommandGroup { get { return insertChartHorizontalCylinderCommandGroup; } }
		public static SpreadsheetUICommand InsertChartHorizontalConeCommandGroup { get { return insertChartHorizontalConeCommandGroup; } }
		public static SpreadsheetUICommand InsertChartHorizontalPyramidCommandGroup { get { return insertChartHorizontalPyramidCommandGroup; } }
		public static SpreadsheetUICommand InsertChartPieCommandGroup { get { return insertChartPieCommandGroup; } }
		public static SpreadsheetUICommand InsertChartPie2DCommandGroup { get { return insertChartPie2DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartPie3DCommandGroup { get { return insertChartPie3DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartLineCommandGroup { get { return insertChartLineCommandGroup; } }
		public static SpreadsheetUICommand InsertChartLine2DCommandGroup { get { return insertChartLine2DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartLine3DCommandGroup { get { return insertChartLine3DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartAreaCommandGroup { get { return insertChartAreaCommandGroup; } }
		public static SpreadsheetUICommand InsertChartArea2DCommandGroup { get { return insertChartArea2DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartArea3DCommandGroup { get { return insertChartArea3DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartScatterCommandGroup { get { return insertChartScatterCommandGroup; } }
		public static SpreadsheetUICommand InsertChartBubbleCommandGroup { get { return insertChartBubbleCommandGroup; } }
		public static SpreadsheetUICommand InsertChartDoughnut2DCommandGroup { get { return insertChartDoughnut2DCommandGroup; } }
		public static SpreadsheetUICommand InsertChartOtherCommandGroup { get { return insertChartOtherCommandGroup; } }
		public static SpreadsheetUICommand InsertChartStockCommandGroup { get { return insertChartStockCommandGroup; } }
		public static SpreadsheetUICommand InsertChartRadarCommandGroup { get { return insertChartRadarCommandGroup; } }
		public static SpreadsheetUICommand InsertChartColumnClustered2D { get { return insertChartColumnClustered2D; } }
		public static SpreadsheetUICommand InsertChartColumnStacked2D { get { return insertChartColumnStacked2D; } }
		public static SpreadsheetUICommand InsertChartColumnPercentStacked2D { get { return insertChartColumnPercentStacked2D; } }
		public static SpreadsheetUICommand InsertChartColumnClustered3D { get { return insertChartColumnClustered3D; } }
		public static SpreadsheetUICommand InsertChartColumnStacked3D { get { return insertChartColumnStacked3D; } }
		public static SpreadsheetUICommand InsertChartColumnPercentStacked3D { get { return insertChartColumnPercentStacked3D; } }
		public static SpreadsheetUICommand InsertChartCylinderClustered { get { return insertChartCylinderClustered; } }
		public static SpreadsheetUICommand InsertChartCylinderStacked { get { return insertChartCylinderStacked; } }
		public static SpreadsheetUICommand InsertChartCylinderPercentStacked { get { return insertChartCylinderPercentStacked; } }
		public static SpreadsheetUICommand InsertChartConeClustered { get { return insertChartConeClustered; } }
		public static SpreadsheetUICommand InsertChartConeStacked { get { return insertChartConeStacked; } }
		public static SpreadsheetUICommand InsertChartConePercentStacked { get { return insertChartConePercentStacked; } }
		public static SpreadsheetUICommand InsertChartPyramidClustered { get { return insertChartPyramidClustered; } }
		public static SpreadsheetUICommand InsertChartPyramidStacked { get { return insertChartPyramidStacked; } }
		public static SpreadsheetUICommand InsertChartPyramidPercentStacked { get { return insertChartPyramidPercentStacked; } }
		public static SpreadsheetUICommand InsertChartBarClustered2D { get { return insertChartBarClustered2D; } }
		public static SpreadsheetUICommand InsertChartBarStacked2D { get { return insertChartBarStacked2D; } }
		public static SpreadsheetUICommand InsertChartBarPercentStacked2D { get { return insertChartBarPercentStacked2D; } }
		public static SpreadsheetUICommand InsertChartBarClustered3D { get { return insertChartBarClustered3D; } }
		public static SpreadsheetUICommand InsertChartBarStacked3D { get { return insertChartBarStacked3D; } }
		public static SpreadsheetUICommand InsertChartBarPercentStacked3D { get { return insertChartBarPercentStacked3D; } }
		public static SpreadsheetUICommand InsertChartHorizontalCylinderClustered { get { return insertChartHorizontalCylinderClustered; } }
		public static SpreadsheetUICommand InsertChartHorizontalCylinderStacked { get { return insertChartHorizontalCylinderStacked; } }
		public static SpreadsheetUICommand InsertChartHorizontalCylinderPercentStacked { get { return insertChartHorizontalCylinderPercentStacked; } }
		public static SpreadsheetUICommand InsertChartHorizontalConeClustered { get { return insertChartHorizontalConeClustered; } }
		public static SpreadsheetUICommand InsertChartHorizontalConeStacked { get { return insertChartHorizontalConeStacked; } }
		public static SpreadsheetUICommand InsertChartHorizontalConePercentStacked { get { return insertChartHorizontalConePercentStacked; } }
		public static SpreadsheetUICommand InsertChartHorizontalPyramidClustered { get { return insertChartHorizontalPyramidClustered; } }
		public static SpreadsheetUICommand InsertChartHorizontalPyramidStacked { get { return insertChartHorizontalPyramidStacked; } }
		public static SpreadsheetUICommand InsertChartHorizontalPyramidPercentStacked { get { return insertChartHorizontalPyramidPercentStacked; } }
		public static SpreadsheetUICommand InsertChartColumn3D { get { return insertChartColumn3D; } }
		public static SpreadsheetUICommand InsertChartCylinder { get { return insertChartCylinder; } }
		public static SpreadsheetUICommand InsertChartCone { get { return insertChartCone; } }
		public static SpreadsheetUICommand InsertChartPyramid { get { return insertChartPyramid; } }
		public static SpreadsheetUICommand InsertChartPie2D { get { return insertChartPie2D; } }
		public static SpreadsheetUICommand InsertChartPie3D { get { return insertChartPie3D; } }
		public static SpreadsheetUICommand InsertChartPieExploded2D { get { return insertChartPieExploded2D; } }
		public static SpreadsheetUICommand InsertChartPieExploded3D { get { return insertChartPieExploded3D; } }
		public static SpreadsheetUICommand InsertChartLine { get { return insertChartLine; } }
		public static SpreadsheetUICommand InsertChartStackedLine { get { return insertChartStackedLine; } }
		public static SpreadsheetUICommand InsertChartPercentStackedLine { get { return insertChartPercentStackedLine; } }
		public static SpreadsheetUICommand InsertChartLineWithMarkers { get { return insertChartLineWithMarkers; } }
		public static SpreadsheetUICommand InsertChartStackedLineWithMarkers { get { return insertChartStackedLineWithMarkers; } }
		public static SpreadsheetUICommand InsertChartPercentStackedLineWithMarkers { get { return insertChartPercentStackedLineWithMarkers; } }
		public static SpreadsheetUICommand InsertChartLine3D { get { return insertChartLine3D; } }
		public static SpreadsheetUICommand InsertChartArea { get { return insertChartArea; } }
		public static SpreadsheetUICommand InsertChartStackedArea { get { return insertChartStackedArea; } }
		public static SpreadsheetUICommand InsertChartPercentStackedArea { get { return insertChartPercentStackedArea; } }
		public static SpreadsheetUICommand InsertChartArea3D { get { return insertChartArea3D; } }
		public static SpreadsheetUICommand InsertChartStackedArea3D { get { return insertChartStackedArea3D; } }
		public static SpreadsheetUICommand InsertChartPercentStackedArea3D { get { return insertChartPercentStackedArea3D; } }
		public static SpreadsheetUICommand InsertChartScatterMarkers { get { return insertChartScatterMarkers; } }
		public static SpreadsheetUICommand InsertChartScatterLines { get { return insertChartScatterLines; } }
		public static SpreadsheetUICommand InsertChartScatterSmoothLines { get { return insertChartScatterSmoothLines; } }
		public static SpreadsheetUICommand InsertChartScatterLinesAndMarkers { get { return insertChartScatterLinesAndMarkers; } }
		public static SpreadsheetUICommand InsertChartScatterSmoothLinesAndMarkers { get { return insertChartScatterSmoothLinesAndMarkers; } }
		public static SpreadsheetUICommand InsertChartBubble { get { return insertChartBubble; } }
		public static SpreadsheetUICommand InsertChartBubble3D { get { return insertChartBubble3D; } }
		public static SpreadsheetUICommand InsertChartDoughnut2D { get { return insertChartDoughnut2D; } }
		public static SpreadsheetUICommand InsertChartDoughnutExploded2D { get { return insertChartDoughnutExploded2D; } }
		public static SpreadsheetUICommand InsertChartRadar { get { return insertChartRadar; } }
		public static SpreadsheetUICommand InsertChartRadarWithMarkers { get { return insertChartRadarWithMarkers; } }
		public static SpreadsheetUICommand InsertChartRadarFilled { get { return insertChartRadarFilled; } }
		public static SpreadsheetUICommand InsertChartStockHighLowClose { get { return insertChartStockHighLowClose; } }
		public static SpreadsheetUICommand InsertChartStockOpenHighLowClose { get { return insertChartStockOpenHighLowClose; } }
		public static SpreadsheetUICommand InsertChartStockVolumeHighLowClose { get { return insertChartStockVolumeHighLowClose; } }
		public static SpreadsheetUICommand InsertChartStockVolumeOpenHighLowClose { get { return insertChartStockVolumeOpenHighLowClose; } }
		public static SpreadsheetUICommand ChartSwitchRowColumn { get { return chartSwitchRowColumn; } }
		public static SpreadsheetUICommand ChartSelectData { get { return chartSelectData; } }
		public static SpreadsheetUICommand ChartTitleCommandGroup { get { return chartTitleCommandGroup; } }
		public static SpreadsheetUICommand ChartTitleNone { get { return chartTitleNone; } }
		public static SpreadsheetUICommand ChartTitleCenteredOverlay { get { return chartTitleCenteredOverlay; } }
		public static SpreadsheetUICommand ChartTitleAbove { get { return chartTitleAbove; } }
		public static SpreadsheetUICommand ChartAxisTitlesCommandGroup { get { return chartAxisTitlesCommandGroup; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisTitleCommandGroup { get { return chartPrimaryHorizontalAxisTitleCommandGroup; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisTitleCommandGroup { get { return chartPrimaryVerticalAxisTitleCommandGroup; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisTitleNone { get { return chartPrimaryHorizontalAxisTitleNone; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisTitleBelow { get { return chartPrimaryHorizontalAxisTitleBelow; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisTitleNone { get { return chartPrimaryVerticalAxisTitleNone; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisTitleRotated { get { return chartPrimaryVerticalAxisTitleRotated; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisTitleVertical { get { return chartPrimaryVerticalAxisTitleVertical; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisTitleHorizontal { get { return chartPrimaryVerticalAxisTitleHorizontal; } }
		public static SpreadsheetUICommand ChartLegendCommandGroup { get { return chartLegendCommandGroup; } }
		public static SpreadsheetUICommand ChartLegendNone { get { return chartLegendNone; } }
		public static SpreadsheetUICommand ChartLegendAtRight { get { return chartLegendAtRight; } }
		public static SpreadsheetUICommand ChartLegendAtTop { get { return chartLegendAtTop; } }
		public static SpreadsheetUICommand ChartLegendAtLeft { get { return chartLegendAtLeft; } }
		public static SpreadsheetUICommand ChartLegendAtBottom { get { return chartLegendAtBottom; } }
		public static SpreadsheetUICommand ChartLegendOverlayAtRight { get { return chartLegendOverlayAtRight; } }
		public static SpreadsheetUICommand ChartLegendOverlayAtLeft { get { return chartLegendOverlayAtLeft; } }
		public static SpreadsheetUICommand ChartDataLabelsCommandGroup { get { return chartDataLabelsCommandGroup; } }
		public static SpreadsheetUICommand ChartDataLabelsNone { get { return chartDataLabelsNone; } }
		public static SpreadsheetUICommand ChartDataLabelsDefault { get { return chartDataLabelsDefault; } }
		public static SpreadsheetUICommand ChartDataLabelsCenter { get { return chartDataLabelsCenter; } }
		public static SpreadsheetUICommand ChartDataLabelsInsideEnd { get { return chartDataLabelsInsideEnd; } }
		public static SpreadsheetUICommand ChartDataLabelsInsideBase { get { return chartDataLabelsInsideBase; } }
		public static SpreadsheetUICommand ChartDataLabelsOutsideEnd { get { return chartDataLabelsOutsideEnd; } }
		public static SpreadsheetUICommand ChartDataLabelsBestFit { get { return chartDataLabelsBestFit; } }
		public static SpreadsheetUICommand ChartDataLabelsLeft { get { return chartDataLabelsLeft; } }
		public static SpreadsheetUICommand ChartDataLabelsRight { get { return chartDataLabelsRight; } }
		public static SpreadsheetUICommand ChartDataLabelsAbove { get { return chartDataLabelsAbove; } }
		public static SpreadsheetUICommand ChartDataLabelsBelow { get { return chartDataLabelsBelow; } }
		public static SpreadsheetUICommand ChartAxesCommandGroup { get { return chartAxesCommandGroup; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisCommandGroup { get { return chartPrimaryHorizontalAxisCommandGroup; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisCommandGroup { get { return chartPrimaryVerticalAxisCommandGroup; } }
		public static SpreadsheetUICommand ChartHidePrimaryHorizontalAxis { get { return chartHidePrimaryHorizontalAxis; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisLeftToRight { get { return chartPrimaryHorizontalAxisLeftToRight; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisRightToLeft { get { return chartPrimaryHorizontalAxisRightToLeft; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisHideLabels { get { return chartPrimaryHorizontalAxisHideLabels; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisDefault { get { return chartPrimaryHorizontalAxisDefault; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisScaleLogarithm { get { return chartPrimaryHorizontalAxisScaleLogarithm; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisScaleThousands { get { return chartPrimaryHorizontalAxisScaleThousands; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisScaleMillions { get { return chartPrimaryHorizontalAxisScaleMillions; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalAxisScaleBillions { get { return chartPrimaryHorizontalAxisScaleBillions; } }
		public static SpreadsheetUICommand ChartHidePrimaryVerticalAxis { get { return chartHidePrimaryVerticalAxis; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisLeftToRight { get { return chartPrimaryVerticalAxisLeftToRight; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisRightToLeft { get { return chartPrimaryVerticalAxisRightToLeft; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisHideLabels { get { return chartPrimaryVerticalAxisHideLabels; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisDefault { get { return chartPrimaryVerticalAxisDefault; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisScaleLogarithm { get { return chartPrimaryVerticalAxisScaleLogarithm; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisScaleThousands { get { return chartPrimaryVerticalAxisScaleThousands; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisScaleMillions { get { return chartPrimaryVerticalAxisScaleMillions; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalAxisScaleBillions { get { return chartPrimaryVerticalAxisScaleBillions; } }
		public static SpreadsheetUICommand ChartGridlinesCommandGroup { get { return chartGridlinesCommandGroup; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalGridlinesCommandGroup { get { return chartPrimaryHorizontalGridlinesCommandGroup; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalGridlinesCommandGroup { get { return chartPrimaryVerticalGridlinesCommandGroup; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalGridlinesNone { get { return chartPrimaryHorizontalGridlinesNone; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalGridlinesMajor { get { return chartPrimaryHorizontalGridlinesMajor; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalGridlinesMinor { get { return chartPrimaryHorizontalGridlinesMinor; } }
		public static SpreadsheetUICommand ChartPrimaryHorizontalGridlinesMajorAndMinor { get { return chartPrimaryHorizontalGridlinesMajorAndMinor; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalGridlinesNone { get { return chartPrimaryVerticalGridlinesNone; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalGridlinesMajor { get { return chartPrimaryVerticalGridlinesMajor; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalGridlinesMinor { get { return chartPrimaryVerticalGridlinesMinor; } }
		public static SpreadsheetUICommand ChartPrimaryVerticalGridlinesMajorAndMinor { get { return chartPrimaryVerticalGridlinesMajorAndMinor; } }
		public static SpreadsheetUICommand ChartLinesCommandGroup { get { return chartLinesCommandGroup; } }
		public static SpreadsheetUICommand ChartLinesNone { get { return chartLinesNone; } }
		public static SpreadsheetUICommand ChartShowDropLines { get { return chartShowDropLines; } }
		public static SpreadsheetUICommand ChartShowHighLowLines { get { return chartShowHighLowLines; } }
		public static SpreadsheetUICommand ChartShowDropLinesAndHighLowLines { get { return chartShowDropLinesAndHighLowLines; } }
		public static SpreadsheetUICommand ChartShowSeriesLines { get { return chartShowSeriesLines; } }
		public static SpreadsheetUICommand ChartUpDownBarsCommandGroup { get { return chartUpDownBarsCommandGroup; } }
		public static SpreadsheetUICommand ChartHideUpDownBars { get { return chartHideUpDownBars; } }
		public static SpreadsheetUICommand ChartShowUpDownBars { get { return chartShowUpDownBars; } }
		public static SpreadsheetUICommand ChartErrorBarsCommandGroup { get { return chartErrorBarsCommandGroup; } }
		public static SpreadsheetUICommand ChartErrorBarsNone { get { return chartErrorBarsNone; } }
		public static SpreadsheetUICommand ChartErrorBarsPercentage { get { return chartErrorBarsPercentage; } }
		public static SpreadsheetUICommand ChartErrorBarsStandardError { get { return chartErrorBarsStandardError; } }
		public static SpreadsheetUICommand ChartErrorBarsStandardDeviation { get { return chartErrorBarsStandardDeviation; } }
		public static SpreadsheetUICommand ToolsPictureCommandGroup { get { return toolsPictureCommandGroup; } }
		public static SpreadsheetUICommand ToolsDrawingCommandGroup { get { return toolsDrawingCommandGroup; } }
		public static SpreadsheetUICommand ToolsChartCommandGroup { get { return toolsChartCommandGroup; } }
		public static SpreadsheetUICommand ToolsPivotTableCommandGroup { get { return toolsPivotTableCommandGroup; } }
		public static SpreadsheetUICommand ReviewProtectSheet { get { return reviewProtectSheet; } }
		public static SpreadsheetUICommand ReviewUnprotectSheet { get { return reviewUnprotectSheet; } }
		public static SpreadsheetUICommand ReviewProtectWorkbook { get { return reviewProtectWorkbook; } }
		public static SpreadsheetUICommand ReviewUnprotectWorkbook { get { return reviewUnprotectWorkbook; } }
		public static SpreadsheetUICommand ReviewShowProtectedRangeManager { get { return reviewShowProtectedRangeManager; } }
		public static SpreadsheetUICommand ReviewInsertComment { get { return reviewInsertComment; } }
		public static SpreadsheetUICommand ReviewEditComment { get { return reviewEditComment; } }
		public static SpreadsheetUICommand ReviewDeleteComment { get { return reviewDeleteComment; } }
		public static SpreadsheetUICommand ReviewShowHideComment { get { return reviewShowHideComment; } }
		public static SpreadsheetUICommand TableToolsCommandGroup { get { return tableToolsCommandGroup; } }
		public static SpreadsheetUICommand TableConvertToRange { get { return tableConvertToRange; } }
		public static SpreadsheetUICommand TableToggleHeaderRow { get { return tableToggleHeaderRow; } }
		public static SpreadsheetUICommand TableToggleTotalRow { get { return tableToggleTotalRow; } }
		public static SpreadsheetUICommand TableToggleBandedColumns { get { return tableToggleBandedColumns; } }
		public static SpreadsheetUICommand TableToggleFirstColumn { get { return tableToggleFirstColumn; } }
		public static SpreadsheetUICommand TableToggleLastColumn { get { return tableToggleLastColumn; } }
		public static SpreadsheetUICommand TableToggleBandedRows { get { return tableToggleBandedRows; } }
		public static SpreadsheetUICommand PageSetupPage { get { return pageSetupPage; } }
		public static SpreadsheetUICommand PageSetupSheet { get { return pageSetupSheet; } }
		public static SpreadsheetUICommand PageSetupCustomMargins { get { return pageSetupCustomMargins; } }
		public static SpreadsheetUICommand PageSetupMorePaperSizes { get { return pageSetupMorePaperSizes; } }
		public static SpreadsheetUICommand OptionsPivotTable { get { return optionsPivotTable; } }
		public static SpreadsheetUICommand OptionsPivotTableContextMenuItem { get { return optionsPivotTableContextMenuItem; } }
		public static SpreadsheetUICommand SelectFieldTypePivotTable { get { return selectFieldTypePivotTable; } }
		public static SpreadsheetUICommand PivotTableExpandField { get { return pivotTableExpandField; } }
		public static SpreadsheetUICommand PivotTableCollapseField { get { return pivotTableCollapseField; } }
		public static SpreadsheetUICommand PivotTableDataRefreshGroup { get { return pivotTableDataRefreshGroup; } }
		public static SpreadsheetUICommand RefreshPivotTable { get { return refreshPivotTable; } }
		public static SpreadsheetUICommand RefreshAllPivotTable { get { return refreshAllPivotTable; } }
		public static SpreadsheetUICommand ChangeDataSourcePivotTable { get { return changeDataSourcePivotTable; } }
		public static SpreadsheetUICommand PivotTableActionsClearGroup { get { return pivotTableActionsClearGroup; } }
		public static SpreadsheetUICommand ClearAllPivotTable { get { return clearAllPivotTable; } }
		public static SpreadsheetUICommand ClearFiltersPivotTable { get { return clearFiltersPivotTable; } }
		public static SpreadsheetUICommand PivotTableActionsSelectGroup { get { return pivotTableActionsSelectGroup; } }
		public static SpreadsheetUICommand SelectValuesPivotTable { get { return selectValuesPivotTable; } }
		public static SpreadsheetUICommand SelectLabelsPivotTable { get { return selectLabelsPivotTable; } }
		public static SpreadsheetUICommand SelectEntirePivotTable { get { return selectEntirePivotTable; } }
		public static SpreadsheetUICommand MovePivotTable { get { return movePivotTable; } }
		public static SpreadsheetUICommand FieldListPanelPivotTable { get { return fieldListPanelPivotTable; } }
		public static SpreadsheetUICommand ShowPivotTableExpandCollapseButtons { get { return showPivotTableExpandCollapseButtons; } }
		public static SpreadsheetUICommand ShowPivotTableFieldHeaders { get { return showPivotTableFieldHeaders; } }
		public static SpreadsheetUICommand PivotTableLayoutSubtotalsGroup { get { return pivotTableLayoutSubtotalsGroup; } }
		public static SpreadsheetUICommand PivotTableDoNotShowSubtotals { get { return pivotTableDoNotShowSubtotals; } }
		public static SpreadsheetUICommand PivotTableShowAllSubtotalsAtBottom { get { return pivotTableShowAllSubtotalsAtBottom; } }
		public static SpreadsheetUICommand PivotTableShowAllSubtotalsAtTop { get { return pivotTableShowAllSubtotalsAtTop; } }
		public static SpreadsheetUICommand PivotTableLayoutGrandTotalsGroup { get { return pivotTableLayoutGrandTotalsGroup; } }
		public static SpreadsheetUICommand PivotTableGrandTotalsOffRowsColumns { get { return pivotTableGrandTotalsOffRowsColumns; } }
		public static SpreadsheetUICommand PivotTableGrandTotalsOnRowsColumns { get { return pivotTableGrandTotalsOnRowsColumns; } }
		public static SpreadsheetUICommand PivotTableGrandTotalsOnRowsOnly { get { return pivotTableGrandTotalsOnRowsOnly; } }
		public static SpreadsheetUICommand PivotTableGrandTotalsOnColumnsOnly { get { return pivotTableGrandTotalsOnColumnsOnly; } }
		public static SpreadsheetUICommand PivotTableLayoutReportLayoutGroup { get { return pivotTableLayoutReportLayoutGroup; } }
		public static SpreadsheetUICommand PivotTableShowCompactForm { get { return pivotTableShowCompactForm; } }
		public static SpreadsheetUICommand PivotTableShowOutlineForm { get { return pivotTableShowOutlineForm; } }
		public static SpreadsheetUICommand PivotTableShowTabularForm { get { return pivotTableShowTabularForm; } }
		public static SpreadsheetUICommand PivotTableRepeatAllItemLabels { get { return pivotTableRepeatAllItemLabels; } }
		public static SpreadsheetUICommand PivotTableDoNotRepeatItemLabels { get { return pivotTableDoNotRepeatItemLabels; } }
		public static SpreadsheetUICommand PivotTableLayoutBlankRowsGroup { get { return pivotTableLayoutBlankRowsGroup; } }
		public static SpreadsheetUICommand PivotTableInsertBlankLineEachItem { get { return pivotTableInsertBlankLineEachItem; } }
		public static SpreadsheetUICommand PivotTableRemoveBlankLineEachItem { get { return pivotTableRemoveBlankLineEachItem; } }
		public static SpreadsheetUICommand PivotTableToggleRowHeaders { get { return pivotTableToggleRowHeaders; } }
		public static SpreadsheetUICommand PivotTableToggleColumnHeaders { get { return pivotTableToggleColumnHeaders; } }
		public static SpreadsheetUICommand PivotTableToggleBandedRows { get { return pivotTableToggleBandedRows; } }
		public static SpreadsheetUICommand PivotTableToggleBandedColumns { get { return pivotTableToggleBandedColumns; } }
		readonly SpreadsheetCommandId id;
		public SpreadsheetUICommand() {
		}
		protected internal SpreadsheetUICommand(SpreadsheetCommandId id) {
			this.id = id;
		}
		public SpreadsheetCommandId CommandId { get { return id; } }
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
			SpreadsheetControl control = ObtainSpreadsheetControl(parameter);
			if (control == null)
				return;
			BarSplitButtonEditItem splitEditItem = invoker as BarSplitButtonEditItem;
			if (splitEditItem != null)
				ExecuteCommand(control, CommandId, splitEditItem.EditValue);
			else
				ExecuteCommand(control, CommandId, null);
		}
		protected internal virtual void ExecuteCommand(SpreadsheetControl control, SpreadsheetCommandId commandId, object parameter) {
			try {
				if (control.ViewControl != null)
					control.ViewControl.ClearMeasureCache();
				control.CommandManager.ExecuteParametrizedCommand(CommandId, parameter);
			}
			catch (Exception e) {
				if (!control.HandleException(e))
					throw;
			}
		}
		#endregion
		protected internal virtual SpreadsheetControl ObtainSpreadsheetControl(object parameter) {
			SpreadsheetControl control = parameter as SpreadsheetControl;
			if (control != null)
				return control;
			SpreadsheetControlAccessor accessor = parameter as SpreadsheetControlAccessor;
			if (accessor != null)
				return accessor.SpreadsheetControl;
			SpreadsheetControlProvider provider = parameter as SpreadsheetControlProvider;
			if (provider == null)
				return null;
			return provider.SpreadsheetControl;
		}
	}
}
