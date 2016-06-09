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

using DevExpress.Utils;
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ModelErrorType(enum)
	public enum ModelErrorType {
		None, 
		DiffRangeTypesCanNotBeChanged,
		IntersectedRangesCanNotBeChanged,
		AutoFilterCannotBeChanged,
		TableCanNotBeChanged,
		MergedCellCanNotBeChanged,
		CanNotShiftNonBlankCellsOffOfTheSheet,
		UnmergeMergedCellsClarification,
		TableOverlap,
		TableCannotBeCreatedInTheLastRow,
		CondFmtInvalidExpression,
		CondFmtExpressionCantBeRange,
		CondFmtExpressionCantBeArray,
		CondFmtExpressionCantContainErrorValues,
		CondFmtExpressionCantUseRelativeRefs,
		CondFmtRank,
		MuiltiCellArrayFormulaInTable,
		InvalidReference,
		InvalidNumberFormat,
		UnionRangeNotAllowed,
		TableCanNotOverlapTable,
		TableNameIsNotValid, 
		TableAlreadyExists, 
		ErrorCommandCannotPerformedWithMultipleSelections, 
		InternalError,
		UsingInvalidObject,
		MergedCellMustBeIdenticallySized,
		CopyAreaCannotBeFitIntoThePasteArea,
		CopyAreaCannotOverlapUnlessSameSizeAndShape,
		AttemptToRemoveRangeWithLockedCells,
		IncorrectPassword,
		LastPrimaryAxisCannotBeChanged,
		AxisGroupCannotBeChanged,
		SomeChartTypesCannotBeCombinedWithOtherChartTypes,
		SurfaceChartMustContainAtLeastTwoSeries,
		IncorrectCreateStockHighLowCloseChart,
		IncorrectCreateStockOpenHighLowCloseChart,
		IncorrectCreateStockVolumeHighLowCloseChart,
		IncorrectCreateStockVolumeOpenHighLowCloseChart,
		CellOrChartIsReadonly,
		CellOrChartIsReadonlyShort,
		UnableToSortMergedCells,
		InvalidSortColumnIndex,
		ErrorChangingPartOfAnArray,
		ErrorRangeContainsTable,
		ErrorRangeConsistsOfEmptyCells,
		ErrorInvalidFilterArgument,
		ErrorInvalidRange,
		ErrorTableRowIndexOutside,
		ErrorInsertAboveTableHeaderRow,
		ErrorInsertBelowTableTotalRow,
		ErrorDeleteTableHeaderRow,
		ErrorStayOnlyTableHeaders,
		ErrorAttemptToHideAllColumns,
		ErrorAttemptToHideAllRows,
		ErrorUseRangeFromAnotherWorksheet,
		ErrorCommandRequiresAtLeastTwoRows,
		ReplaceTheContentsOfTheDestinationCells,
		PivotTableWillNotFitOnTheSheet,
		PivotTableWillNotFitOnTheSheetSelectNewLocation,
		PivotTableWillOverrideSheetCells,
		PivotTableIndentOverflow,
		PivotTableFieldNameIsInvalid,
		PivotTableCanNotBeChanged,
		PivotTablePartCanNotBeChanged,
		PivotTableCanNotBeBuiltFromEmptyCache,
		PivotTableGrouppedAndCalculatedFieldsAreNotSupportedNow,
		PivotTableReportCanNotOverlapTable,
		PivotTableDataSourceAndDestinationReferencesAreBothInvalid,
		PivotTableDataSourceReferenceNotValid,
		PivotTableDestinationReferenceNotValid,
		PivotTableNameAlreadyExists,
		PivotTableNameIsInvalid,
		PivotTableCanNotOverlapPivotTable,
		PivotTableTooMuchDataFields,
		PivotTableTooMuchPageFields,
		PivotTableNotEnoughDataFields,
		PivotCacheStringVeryLong,
		PivotFieldNameAlreadyExists,
		PivotFieldNameIsInvalid,
		PivotFieldCannotBePlacedOnThatAxis,
		PivotFieldHasTooMuchItems,
		PivotFieldHasTooMuchItems_ColumnField,
		PivotCacheSourceTypeIsInvalid,
		PivotCalculationRequiresField,
		PivotCalculationRequiresItem,
		PivotCannotHideLastVisibleItem,
		PivotFilterRequiresValue,
		PivotFilterRequiresSecondValue,
		PivotFilterRequiresMeasureField,
		PivotFilterCannotAddFilterToPageField,
		PivotFilterTop10CountMustBeInteger,
		PivotFilterCannotChangeTop10TypeProperty,
		PivotCacheFieldContainsNonDateItems,
		PivotTableSavedWithoutUnderlyingData,
		PivotCanNotDetermineField,
		PivotFilterValueMustBeBetween,
		PivotFilterEndNumberMustBeGreaterThanStartNumber,
		PivotTableCanNotApplyDataValidation,
		PivotFilterInvalidDate,
		PivotTableSubtotalListPlace,
		RangeMustConsistOfASingleRowOrColumn,
		SparklinePositionsMustBeUnique,
		SparklinePositionOrDataRangeIsInvalid,
		InvalidFormula,
		DataValidationFormulaIsEmpty,
		DataValidationBothFormulasAreEmpty,
		DataValidationMinGreaterThanMax,
		DataValidationInvalidReference,
		DataValidationDefinedNameNotFound,
		DataValidationInvalidNonnumericValue,
		DataValidationInvalidDecimalValue,
		DataValidationInvalidNegativeValue,
		DataValidationUnionRangeNotAllowed,
		DataValidationMoreThanOneCellInRange,
		DataValidationMustBeRowOrColumnRange,
		DataValidationInvalidDate,
		DataValidationInvalidTime,
		IncorrectNumberRange,
		InvalidNumber,
		PageSetupMarginsNotFitPageSize,
		PageSetupProblemFormula,
		HeaderFooterTooLongTextString,
		CantDoThatToAMergedCell,
		SelectedCellsAffectsPivotTableAndCanNotBeChanged,
		PasteAreaNotSameSizeAsSelectionClarification,
		ChartDataRangeIntersectPivotTable
	}
	#endregion
	#region IErrorInfo
	public interface IModelErrorInfo {
		ModelErrorType ErrorType { get; }
		ErrorHandlingResult Visit(IErrorInfoVisitor visitor);
	}
	#endregion
	#region ErrorInfo
	public class ModelErrorInfo : IModelErrorInfo {
		ModelErrorType errorType;
		public ModelErrorInfo(ModelErrorType errorType) {
			this.errorType = errorType;
		}
		public ModelErrorType ErrorType { get { return errorType; } }
		#region IErrorInfo Members
		public ErrorHandlingResult Visit(IErrorInfoVisitor visitor) {
			return visitor.Visit(this);
		}
		#endregion
	}
	#endregion
	#region ClarificationErrorInfo
	public class ClarificationErrorInfo : IModelErrorInfo {
		ModelErrorType errorType;
		public ClarificationErrorInfo(ModelErrorType errorType) {
			this.errorType = errorType;
		}
		public ModelErrorType ErrorType { get { return errorType; } }
		#region IErrorInfo Members
		public ErrorHandlingResult Visit(IErrorInfoVisitor visitor) {
			return visitor.Visit(this);
		}
		#endregion
	}
	#endregion
	#region ModelErrorInfoWithArgs
	public class ModelErrorInfoWithArgs : IModelErrorInfo {
		ModelErrorType errorType;
		object[] arguments;
		public ModelErrorInfoWithArgs(ModelErrorType errorType, object argument) {
			Guard.ArgumentNotNull(argument, "argument");
			this.errorType = errorType;
			this.arguments = new object[] { argument };
		}
		public ModelErrorInfoWithArgs(ModelErrorType errorType, object[] arguments) {
			Guard.ArgumentNotNull(arguments, "arguments");
			this.errorType = errorType;
			this.arguments = arguments;
		}
		public ModelErrorType ErrorType { get { return errorType; } }
		public object[] Arguments { get { return arguments; } }
		#region IErrorInfo Members
		public ErrorHandlingResult Visit(IErrorInfoVisitor visitor) {
			return visitor.Visit(this);
		}
		#endregion
	}
	#endregion
	#region ErrorHandlingResult(enum)
	public enum ErrorHandlingResult { 
		Ignore,
		Abort,
	}
	#endregion
	#region IErrorInfoVisitor
	public interface IErrorInfoVisitor {
		ErrorHandlingResult Visit(ModelErrorInfo info);
		ErrorHandlingResult Visit(ClarificationErrorInfo info);
		ErrorHandlingResult Visit(ModelErrorInfoWithArgs info);
	}
	#endregion
	#region IErrorHandler
	public interface IErrorHandler {
		ErrorHandlingResult HandleError(IModelErrorInfo errorInfo);
	}
	#endregion
	public class AsserttingErrorHandler : IErrorHandler, IErrorInfoVisitor {
		#region IErrorHandler Members
		public ErrorHandlingResult HandleError(ModelErrorType errorType) {
			return HandleError(new ModelErrorInfo(errorType));
		}
		public ErrorHandlingResult HandleError(IModelErrorInfo errorInfo) {
			if (errorInfo == null)
				return ErrorHandlingResult.Ignore;
			return errorInfo.Visit(this);
		}
		#endregion
		#region IErrorInfoVisitor Members
		public ErrorHandlingResult Visit(ModelErrorInfo info) {
			throw new InvalidOperationException(info.ErrorType.ToString());
		}
		public ErrorHandlingResult Visit(ClarificationErrorInfo info) {
			return ErrorHandlingResult.Ignore;
		}
		public ErrorHandlingResult Visit(ModelErrorInfoWithArgs info) {
			throw new InvalidOperationException(info.ErrorType.ToString());
		}
		#endregion
	}
}
