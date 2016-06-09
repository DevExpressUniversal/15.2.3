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

using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
using System;
#else
using System.Windows.Input;
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.XtraSpreadsheet {
	#region UIErrorHandler
	public class UIErrorHandler : IErrorHandler, IErrorInfoVisitor {
		#region static
		static Dictionary<ModelErrorType, XtraSpreadsheetStringId> errorDescriptions = CreateErrorDescriptionsTable();
		static Dictionary<DialogResult, ErrorHandlingResult> dialogResultToErrorHandlingTable = CreateDialogResultToErrorHandlingTable();
		static Dictionary<ModelErrorType, XtraSpreadsheetStringId> CreateErrorDescriptionsTable() {
			Dictionary<ModelErrorType, XtraSpreadsheetStringId> result = new Dictionary<ModelErrorType, XtraSpreadsheetStringId>();
			result.Add(ModelErrorType.DiffRangeTypesCanNotBeChanged, XtraSpreadsheetStringId.Msg_DiffRangeTypesCanNotBeChanged);
			result.Add(ModelErrorType.IntersectedRangesCanNotBeChanged, XtraSpreadsheetStringId.Msg_IntersectedRangesCanNotBeChanged);
			result.Add(ModelErrorType.AutoFilterCannotBeChanged, XtraSpreadsheetStringId.Msg_ChangingRangeOfAutoFilterNotAllowed);
			result.Add(ModelErrorType.TableCanNotBeChanged, XtraSpreadsheetStringId.Msg_ShiftCellsInATableIsNotAllowed);
			result.Add(ModelErrorType.MergedCellCanNotBeChanged, XtraSpreadsheetStringId.Msg_ErrorCannotChangingMergedCell);
			result.Add(ModelErrorType.CanNotShiftNonBlankCellsOffOfTheSheet, XtraSpreadsheetStringId.Msg_CanNotShiftNonBlankCellsOffOfTheSheet);
			result.Add(ModelErrorType.UnmergeMergedCellsClarification, XtraSpreadsheetStringId.Msg_UnmergeMergedCellsClarification);
			result.Add(ModelErrorType.TableOverlap, XtraSpreadsheetStringId.Msg_TableOverlap);
			result.Add(ModelErrorType.CondFmtInvalidExpression, XtraSpreadsheetStringId.Msg_ErrorConditionalFormattingFormula);
			result.Add(ModelErrorType.CondFmtExpressionCantBeRange, XtraSpreadsheetStringId.Msg_ErrorConditionalFormattingRange);
			result.Add(ModelErrorType.CondFmtExpressionCantBeArray, XtraSpreadsheetStringId.Msg_CondFmtExpressionCantBeAnArray);
			result.Add(ModelErrorType.CondFmtExpressionCantContainErrorValues, XtraSpreadsheetStringId.Msg_CondFmtExpressionCantContainErrorValues);
			result.Add(ModelErrorType.CondFmtExpressionCantUseRelativeRefs, XtraSpreadsheetStringId.Msg_CondFmtExpressionCantContainRelativeRefs);
			result.Add(ModelErrorType.CondFmtRank, XtraSpreadsheetStringId.Msg_ErrorConditionalFormattingRank);
			result.Add(ModelErrorType.MuiltiCellArrayFormulaInTable, XtraSpreadsheetStringId.Msg_ErrorMuiltiCellArrayFormulaInTable);
			result.Add(ModelErrorType.InvalidReference, XtraSpreadsheetStringId.Msg_ErrorInvalidReference);
			result.Add(ModelErrorType.InvalidNumberFormat, XtraSpreadsheetStringId.Msg_ErrorInvalidNumberFormat);
			result.Add(ModelErrorType.UnionRangeNotAllowed, XtraSpreadsheetStringId.Msg_ErrorUnionRange);
			result.Add(ModelErrorType.TableCanNotOverlapTable, XtraSpreadsheetStringId.Msg_ErrorOverlapRange);
			result.Add(ModelErrorType.TableNameIsNotValid, XtraSpreadsheetStringId.Msg_ErrorTableNameIsNotValid);
			result.Add(ModelErrorType.TableAlreadyExists, XtraSpreadsheetStringId.Msg_ErrorTableAlreadyExists);
			result.Add(ModelErrorType.TableCannotBeCreatedInTheLastRow, XtraSpreadsheetStringId.Msg_ErrorTableCannotBeCreatedInTheLastRow);
			result.Add(ModelErrorType.ErrorCommandCannotPerformedWithMultipleSelections, XtraSpreadsheetStringId.Msg_ErrorCommandCannotPerformedWithMultipleSelections);
			result.Add(ModelErrorType.InternalError, XtraSpreadsheetStringId.Msg_ErrorInternalError);
			result.Add(ModelErrorType.UsingInvalidObject, XtraSpreadsheetStringId.Msg_ErrorInvalidObjectUsage);
			result.Add(ModelErrorType.MergedCellMustBeIdenticallySized, XtraSpreadsheetStringId.Msg_ErrorResizeRangeToMergedCell);
			result.Add(ModelErrorType.CopyAreaCannotBeFitIntoThePasteArea, XtraSpreadsheetStringId.Msg_ErrorCopyAreaCannotBeFitIntoThePasteArea);
			result.Add(ModelErrorType.PasteAreaNotSameSizeAsSelectionClarification, XtraSpreadsheetStringId.Msg_PasteAreaNotSameSizeAsSelectionClarification);
			result.Add(ModelErrorType.CopyAreaCannotOverlapUnlessSameSizeAndShape, XtraSpreadsheetStringId.Msg_ErrorCopyAreaCannotOverlapUnlessSameSizeAndShape);
			result.Add(ModelErrorType.AttemptToRemoveRangeWithLockedCells, XtraSpreadsheetStringId.Msg_ErrorAttemptToRemoveRangeWithLockedCells);
			result.Add(ModelErrorType.IncorrectPassword, XtraSpreadsheetStringId.Msg_IncorrectPassword);
			result.Add(ModelErrorType.CellOrChartIsReadonly, XtraSpreadsheetStringId.Msg_CellOrChartIsReadonly);
			result.Add(ModelErrorType.CellOrChartIsReadonlyShort, XtraSpreadsheetStringId.Msg_CellOrChartIsReadonlyShort);
			result.Add(ModelErrorType.ErrorChangingPartOfAnArray, XtraSpreadsheetStringId.Msg_ErrorChangingPartOfAnArray);
			result.Add(ModelErrorType.ErrorRangeContainsTable, XtraSpreadsheetStringId.Msg_ErrorRangeContainsTable);
			result.Add(ModelErrorType.ErrorAttemptToHideAllColumns, XtraSpreadsheetStringId.Msg_ErrorAttemptToHideAllColumns);
			result.Add(ModelErrorType.ErrorAttemptToHideAllRows, XtraSpreadsheetStringId.Msg_ErrorAttemptToHideAllRows);
			result.Add(ModelErrorType.ErrorUseRangeFromAnotherWorksheet, XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet);
			result.Add(ModelErrorType.ErrorCommandRequiresAtLeastTwoRows, XtraSpreadsheetStringId.Msg_CommandRequiresAtLeastTwoRows);
			result.Add(ModelErrorType.ReplaceTheContentsOfTheDestinationCells, XtraSpreadsheetStringId.Msg_CanReplaceTheContentsOfTheDestinationCells);
			result.Add(ModelErrorType.PivotTableWillNotFitOnTheSheet, XtraSpreadsheetStringId.Msg_PivotTableWillNotFitOnTheSheet);
			result.Add(ModelErrorType.PivotTableWillNotFitOnTheSheetSelectNewLocation, XtraSpreadsheetStringId.Msg_PivotTableWillNotFitOnTheSheetSelectNewLocation);
			result.Add(ModelErrorType.PivotTableWillOverrideSheetCells, XtraSpreadsheetStringId.Msg_PivotTableWillOverrideSheetCells);
			result.Add(ModelErrorType.PivotTableIndentOverflow, XtraSpreadsheetStringId.Msg_PivotTableIndentOverflow);
			result.Add(ModelErrorType.PivotTableFieldNameIsInvalid, XtraSpreadsheetStringId.Msg_PivotTableFieldNameIsInvalid);
			result.Add(ModelErrorType.PivotTableCanNotBeChanged, XtraSpreadsheetStringId.Msg_PivotTableCanNotBeChanged);
			result.Add(ModelErrorType.PivotTablePartCanNotBeChanged, XtraSpreadsheetStringId.Msg_PivotTablePartCanNotBeChanged);
			result.Add(ModelErrorType.PivotTableCanNotBeBuiltFromEmptyCache, XtraSpreadsheetStringId.Msg_PivotTableCanNotBeBuiltFromEmptyCache);
			result.Add(ModelErrorType.PivotTableGrouppedAndCalculatedFieldsAreNotSupportedNow, XtraSpreadsheetStringId.Msg_PivotTableGrouppedAndCalculatedFieldsAreNotSupportedNow);
			result.Add(ModelErrorType.PivotTableReportCanNotOverlapTable, XtraSpreadsheetStringId.Msg_PivotTableReportCanNotOverlapTable);
			result.Add(ModelErrorType.PivotTableDataSourceAndDestinationReferencesAreBothInvalid, XtraSpreadsheetStringId.Msg_PivotTableDataSourceAndDestinationReferencesAreBothInvalid);
			result.Add(ModelErrorType.PivotTableDataSourceReferenceNotValid, XtraSpreadsheetStringId.Msg_PivotTableDataSourceReferenceNotValid);
			result.Add(ModelErrorType.PivotTableDestinationReferenceNotValid, XtraSpreadsheetStringId.Msg_PivotTableDestinationReferenceNotValid);
			result.Add(ModelErrorType.PivotTableNameAlreadyExists, XtraSpreadsheetStringId.Msg_PivotTableNameAlreadyExists);
			result.Add(ModelErrorType.PivotTableNameIsInvalid, XtraSpreadsheetStringId.Msg_PivotTableNameIsInvalid);
			result.Add(ModelErrorType.PivotTableCanNotOverlapPivotTable, XtraSpreadsheetStringId.Msg_PivotTableCanNotOverlapPivotTable);
			result.Add(ModelErrorType.PivotTableTooMuchDataFields, XtraSpreadsheetStringId.Msg_PivotTableTooMuchDataFields);
			result.Add(ModelErrorType.PivotTableTooMuchPageFields, XtraSpreadsheetStringId.Msg_PivotTableTooMuchPageFields);
			result.Add(ModelErrorType.PivotTableNotEnoughDataFields, XtraSpreadsheetStringId.Msg_PivotTableNotEnoughDataFields);
			result.Add(ModelErrorType.PivotCacheStringVeryLong, XtraSpreadsheetStringId.Msg_PivotCacheStringVeryLong);
			result.Add(ModelErrorType.PivotFieldNameAlreadyExists, XtraSpreadsheetStringId.Msg_PivotFieldNameAlreadyExists);
			result.Add(ModelErrorType.PivotFieldNameIsInvalid, XtraSpreadsheetStringId.Msg_PivotFieldNameIsInvalid);
			result.Add(ModelErrorType.PivotFieldCannotBePlacedOnThatAxis, XtraSpreadsheetStringId.Msg_PivotFieldCannotBePlacedOnThatAxis);
			result.Add(ModelErrorType.PivotFieldHasTooMuchItems, XtraSpreadsheetStringId.Msg_PivotFieldHasTooMuchItems);
			result.Add(ModelErrorType.PivotFieldHasTooMuchItems_ColumnField, XtraSpreadsheetStringId.Msg_PivotFieldHasTooMuchItems_ColumnField);
			result.Add(ModelErrorType.PivotCacheSourceTypeIsInvalid, XtraSpreadsheetStringId.Msg_PivotCacheSourceTypeIsInvalid);
			result.Add(ModelErrorType.PivotCalculationRequiresField, XtraSpreadsheetStringId.Msg_PivotCalculationRequiresField);
			result.Add(ModelErrorType.PivotCalculationRequiresItem, XtraSpreadsheetStringId.Msg_PivotCalculationRequiresItem);
			result.Add(ModelErrorType.PivotCannotHideLastVisibleItem, XtraSpreadsheetStringId.Msg_PivotCannotHideLastVisibleItem);
			result.Add(ModelErrorType.PivotFilterRequiresValue, XtraSpreadsheetStringId.Msg_PivotFilterRequiresValue);
			result.Add(ModelErrorType.PivotFilterRequiresSecondValue, XtraSpreadsheetStringId.Msg_PivotFilterRequiresSecondValue);
			result.Add(ModelErrorType.PivotFilterRequiresMeasureField, XtraSpreadsheetStringId.Msg_PivotFilterRequiresMeasureField);
			result.Add(ModelErrorType.PivotFilterTop10CountMustBeInteger, XtraSpreadsheetStringId.Msg_PivotFilterTop10CountMustBeInteger);
			result.Add(ModelErrorType.PivotFilterCannotAddFilterToPageField, XtraSpreadsheetStringId.Msg_PivotFilterCannotAddFilterToPageField);
			result.Add(ModelErrorType.PivotFilterCannotChangeTop10TypeProperty, XtraSpreadsheetStringId.Msg_PivotFilterCannotChangeTop10TypeProperty);
			result.Add(ModelErrorType.PivotCacheFieldContainsNonDateItems, XtraSpreadsheetStringId.Msg_PivotCacheFieldContainsNonDateItems);
			result.Add(ModelErrorType.PivotTableSavedWithoutUnderlyingData, XtraSpreadsheetStringId.Msg_PivotTableSavedWithoutUnderlyingData);
			result.Add(ModelErrorType.PivotCanNotDetermineField, XtraSpreadsheetStringId.Msg_PivotCanNotDetermineField);
			result.Add(ModelErrorType.PivotFilterValueMustBeBetween, XtraSpreadsheetStringId.Msg_PivotFilterValueMustBeBetween);
			result.Add(ModelErrorType.PivotFilterEndNumberMustBeGreaterThanStartNumber, XtraSpreadsheetStringId.Msg_PivotFilterEndNumberMustBeGreaterThanStartNumber);
			result.Add(ModelErrorType.PivotTableCanNotApplyDataValidation, XtraSpreadsheetStringId.Msg_PivotTableCanNotApplyDataValidation);
			result.Add(ModelErrorType.PivotFilterInvalidDate, XtraSpreadsheetStringId.Msg_PivotFilterInvalidDate);
			result.Add(ModelErrorType.PivotTableSubtotalListPlace, XtraSpreadsheetStringId.Msg_PivotTableSubtotalListPlace);
			result.Add(ModelErrorType.InvalidFormula, XtraSpreadsheetStringId.Msg_InvalidFormula);
			result.Add(ModelErrorType.DataValidationFormulaIsEmpty, XtraSpreadsheetStringId.Msg_DataValidationFormulaIsEmpty);
			result.Add(ModelErrorType.DataValidationBothFormulasAreEmpty, XtraSpreadsheetStringId.Msg_DataValidationBothFormulasAreEmpty);
			result.Add(ModelErrorType.DataValidationMinGreaterThanMax, XtraSpreadsheetStringId.Msg_DataValidationMinGreaterThanMax);
			result.Add(ModelErrorType.DataValidationDefinedNameNotFound, XtraSpreadsheetStringId.Msg_DataValidationDefinedNameNotFound);
			result.Add(ModelErrorType.DataValidationInvalidNonnumericValue, XtraSpreadsheetStringId.Msg_DataValidationInvalidNonnumericValue);
			result.Add(ModelErrorType.DataValidationInvalidDecimalValue, XtraSpreadsheetStringId.Msg_DataValidationInvalidDecimalValue);
			result.Add(ModelErrorType.DataValidationInvalidNegativeValue, XtraSpreadsheetStringId.Msg_DataValidationInvalidNegativeValue);
			result.Add(ModelErrorType.DataValidationUnionRangeNotAllowed, XtraSpreadsheetStringId.Msg_DataValidationUnionRangeNotAllowed);
			result.Add(ModelErrorType.DataValidationInvalidReference, XtraSpreadsheetStringId.Msg_DataValidationInvalidReference);
			result.Add(ModelErrorType.DataValidationMoreThanOneCellInRange, XtraSpreadsheetStringId.Msg_DataValidationMoreThanOneCellInRange);
			result.Add(ModelErrorType.DataValidationMustBeRowOrColumnRange, XtraSpreadsheetStringId.Msg_DataValidationMustBeRowOrColumnRange);
			result.Add(ModelErrorType.DataValidationInvalidDate, XtraSpreadsheetStringId.Msg_DataValidationInvalidDate);
			result.Add(ModelErrorType.DataValidationInvalidTime, XtraSpreadsheetStringId.Msg_DataValidationInvalidTime);
			result.Add(ModelErrorType.IncorrectNumberRange, XtraSpreadsheetStringId.Msg_IncorrectNumberRange);
			result.Add(ModelErrorType.InvalidNumber, XtraSpreadsheetStringId.Msg_InvalidNumber);
			result.Add(ModelErrorType.PageSetupMarginsNotFitPageSize, XtraSpreadsheetStringId.Msg_PageSetupMarginsNotFitPageSize);
			result.Add(ModelErrorType.PageSetupProblemFormula, XtraSpreadsheetStringId.Msg_PageSetupProblemFormula);
			result.Add(ModelErrorType.HeaderFooterTooLongTextString, XtraSpreadsheetStringId.Msg_HeaderFooterTooLongTextString);
			result.Add(ModelErrorType.CantDoThatToAMergedCell, XtraSpreadsheetStringId.Msg_CantDoThatToAMergedCell);
			result.Add(ModelErrorType.SelectedCellsAffectsPivotTableAndCanNotBeChanged, XtraSpreadsheetStringId.Msg_SelectedCellsAffectsPivotTableAndCanNotBeChanged);
			result.Add(ModelErrorType.ChartDataRangeIntersectPivotTable, XtraSpreadsheetStringId.Msg_ChartDataRangeIntersectPivotTable);
			return result;
		}
		static Dictionary<DialogResult, ErrorHandlingResult> CreateDialogResultToErrorHandlingTable() {
			Dictionary<DialogResult, ErrorHandlingResult> result = new Dictionary<DialogResult, ErrorHandlingResult>();
			result.Add(DialogResult.Abort, ErrorHandlingResult.Abort);
			result.Add(DialogResult.Cancel, ErrorHandlingResult.Abort);
			result.Add(DialogResult.Ignore, ErrorHandlingResult.Ignore);
			result.Add(DialogResult.No, ErrorHandlingResult.Abort);
			result.Add(DialogResult.None, ErrorHandlingResult.Ignore);
			result.Add(DialogResult.OK, ErrorHandlingResult.Abort);
			result.Add(DialogResult.Retry, ErrorHandlingResult.Ignore);
			result.Add(DialogResult.Yes, ErrorHandlingResult.Ignore);
			return result;
		}
		#endregion
		readonly ISpreadsheetControl control;
		public UIErrorHandler(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region IErrorHandler Members
		public ErrorHandlingResult HandleError(IModelErrorInfo errorInfo) {
			if (errorInfo == null)
				return ErrorHandlingResult.Ignore;
			return errorInfo.Visit(this);
		}
		#endregion
		#region IErrorInfoVisitor Members
		public ErrorHandlingResult Visit(ModelErrorInfo info) {
			if (info.ErrorType == ModelErrorType.CellOrChartIsReadonly) {
				if (control.InnerControl.RaiseProtectionWarning())
					return ErrorHandlingResult.Abort;
			}
			XtraSpreadsheetStringId textDescriptionId = GetDescriptionStringId(info.ErrorType);
			DialogResult result = control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(textDescriptionId));
			return dialogResultToErrorHandlingTable[result];
		}
		public ErrorHandlingResult Visit(ClarificationErrorInfo info) {
			XtraSpreadsheetStringId textDescriptionId = GetDescriptionStringId(info.ErrorType);
			bool result = control.ShowOkCancelMessage(XtraSpreadsheetLocalizer.GetString(textDescriptionId));
			return result ? ErrorHandlingResult.Ignore : ErrorHandlingResult.Abort;
		}
		public ErrorHandlingResult Visit(ModelErrorInfoWithArgs info) {
			if (info.ErrorType == ModelErrorType.CellOrChartIsReadonly) {
				if (control.InnerControl.RaiseProtectionWarning())
					return ErrorHandlingResult.Abort;
			}
			XtraSpreadsheetStringId textDescriptionId = GetDescriptionStringId(info.ErrorType);
			string message = String.Format(XtraSpreadsheetLocalizer.GetString(textDescriptionId), info.Arguments);
			DialogResult result = control.ShowWarningMessage(message);
			return dialogResultToErrorHandlingTable[result];
		}
		#endregion
		XtraSpreadsheetStringId GetDescriptionStringId(ModelErrorType errorType) {
			return errorDescriptions[errorType];
		}
	}
	#endregion
}
