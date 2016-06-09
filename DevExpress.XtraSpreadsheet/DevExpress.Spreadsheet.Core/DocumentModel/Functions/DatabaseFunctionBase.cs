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
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region WorksheetDatabaseFunctionBase (abstract class)
	public abstract class WorksheetDatabaseFunctionBase : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		#region Properties
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue database = arguments[0];
			if (database.IsError)
				return database;
			if (database.CellRangeValue.RangeType == CellRangeType.UnionRange || database.CellRangeValue.Height < 2 || database.CellRangeValue.Worksheet == null)
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue field = arguments[1];
			if (field.IsCellRange) {
				CellRangeBase cellRangeFieldValue = field.CellRangeValue;
				if (cellRangeFieldValue.RangeType == CellRangeType.UnionRange || cellRangeFieldValue.CellCount > 1)
					return VariantValue.ErrorInvalidValueInFunction;
				field = cellRangeFieldValue.GetFirstCellValue();
			}
			if (field.IsError)
				return field;
			VariantValue criteriaRange = arguments[2];
			if (criteriaRange.IsError)
				return criteriaRange;
			if (!criteriaRange.IsCellRange || criteriaRange.CellRangeValue.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			if (criteriaRange.CellRangeValue.Height < 2)
				return VariantValue.ErrorInvalidValueInFunction;
			CellRange range = (CellRange)database.CellRangeValue;
			DatabaseFunctionCondition condition = CreateCondition((CellRange)criteriaRange.CellRangeValue, range, context);
			return EvaluateCoreCore(range, field, condition, context);
		}
		protected virtual VariantValue EvaluateCoreCore(CellRange range, VariantValue field, DatabaseFunctionCondition condition, WorkbookDataContext context) {
			VariantValue fieldIndex = CalculateFieldIndex(range, field, context);
			if (fieldIndex.IsError)
				return fieldIndex;
			FunctionResult result = CreateInitialFunctionResult(context);
			return EvaluateCellRange(context, range, (int)fieldIndex.NumericValue, result, condition);
		}
		DatabaseFunctionCondition CreateCondition(CellRange conditionRange, CellRange range, WorkbookDataContext context) {
			DatabaseFunctionCalculationOrCondition result = new DatabaseFunctionCalculationOrCondition();
			RangeHorizontalVector conditionFields = new RangeHorizontalVector(conditionRange, 0);
			RangeHorizontalVector dataFields = new RangeHorizontalVector(range, 0);
			CellRange dataRange = new CellRange(range.Worksheet, new CellPosition(range.TopLeft.Column, range.TopLeft.Row + 1), range.BottomRight);
			int count = conditionRange.Height;
			for (int i = 1; i < count; i++) { 
				DatabaseFunctionCondition condition = CreateCondition(conditionFields, conditionRange.GetSubRowRange(i, i), dataFields, dataRange, context);
				if (condition != null)
					result.AddCondition(condition);
			}
			if (result.ConditionsCount <= 0)
				return null;
			else
				return result;
		}
		DatabaseFunctionCondition CreateCondition(RangeHorizontalVector conditionFields, CellRange conditionsRange, RangeHorizontalVector dataFields, CellRange dataRange, WorkbookDataContext context) {
			DatabaseFunctionCalculationAndCondition result = new DatabaseFunctionCalculationAndCondition();
			int count = conditionsRange.Width;
			for (int i = 0; i < count; i++) { 
				ICell cell = conditionsRange.TryGetCellRelative(i, 0) as ICell;
				if (cell == null)
					continue;
				string fieldName = conditionFields[i].ToText(context).InlineTextValue;
				VariantValue fieldIndex = CalculateFieldIndex(dataFields, fieldName, context);
				DatabaseFunctionCondition condition = null;
				if (!fieldIndex.IsError) {
					condition = TryCreateSimpleCondition((int)fieldIndex.NumericValue, cell.Value, dataFields, dataRange, context);
				}
				else {
					condition = TryCreateComplexCondition(conditionFields[i], cell, dataFields, dataRange, context);
				}
				if (condition != null)
					result.AddCondition(condition);
			}
			if (result.ConditionsCount <= 0)
				return null;
			else
				return result;
		}
		DatabaseFunctionCondition TryCreateSimpleCondition(int fieldIndex, VariantValue conditionText, RangeHorizontalVector dataFields, CellRange dataRange, WorkbookDataContext context) {
			if (conditionText.IsEmpty)
				return null;
			return new DatabaseFunctionSimpleCondition(dataRange, conditionText, context, fieldIndex);
		}
		DatabaseFunctionCondition TryCreateComplexCondition(VariantValue field, ICell conditionCell, RangeHorizontalVector dataFields, CellRange dataRange, WorkbookDataContext context) {
			if (conditionCell.HasFormula) {
				FormulaBase conditionFormula = conditionCell.GetFormula();
				ParsedExpression conditionFormulaExpression = conditionFormula.Expression.Clone();
				ConditionFormulaPrepareWalker walker = new ConditionFormulaPrepareWalker(dataRange, dataFields, context);
				context.PushCurrentCell(conditionCell);
				try {
					conditionFormulaExpression = walker.Process(conditionFormulaExpression);
				}
				finally {
					context.PopCurrentCell();
				}
				if (!walker.IsValidExpression)
					return new DatabaseFunctionAlwaysFalseCondition();
				return new DatabaseFunctionFormulaCondition(conditionFormulaExpression, walker.ValidRecordStart, walker.ValidRecordEnd, dataRange);
			}
			else {
				VariantValue value = conditionCell.Value;
				if (!value.ToBoolean(context).BooleanValue)
					return new DatabaseFunctionAlwaysFalseCondition();
				return null;
			}
		}
		protected VariantValue EvaluateCellRange(WorkbookDataContext context, CellRange range, int fieldIndex, FunctionResult result, DatabaseFunctionCondition condition) {
			int count = GetRangeEffectiveCount(range);
			result.BeginArrayProcessing(count - 1);
			for (int i = 1; i < count; i++) { 
				RangeHorizontalVector vector = new RangeHorizontalVector(range, i);
				if (ShouldProcessRecord(context, vector, i - 1, fieldIndex, result, condition))
					ProcessValue(vector[fieldIndex], result);
				if (result.Error.IsError)
					return result.Error;
			}
			result.EndArrayProcessing();
			if (result.Error.IsError)
				return result.Error;
			return result.GetFinalValue();
		}
		int GetRangeEffectiveCount(CellRange range) {
			IRowCollection rows = range.Worksheet.Rows as IRowCollection;
			if (rows == null || rows.Count <= 0)
				return range.Height;
			return Math.Min(Math.Max(0, rows.Last.Index - range.TopLeft.Row + 1), range.Height);
		}
		bool ProcessValue(VariantValue value, FunctionResult result) {
			if (value.IsError)
				return result.ProcessErrorValue(value);
			else
				return result.ProcessSingleValue(value);
		}
		bool ShouldProcessRecord(WorkbookDataContext context, RangeHorizontalVector vector, int relativeRowIndex, int relativeColumnIndex, FunctionResult result, DatabaseFunctionCondition condition) {
			VariantValue value = VariantValue.Empty;
			if(relativeColumnIndex>=0) {
				ConditionCalculationResult calculateCondtionsResult = result.CalculateConditions(relativeRowIndex, relativeColumnIndex);
				if (calculateCondtionsResult == ConditionCalculationResult.ErrorGettingData) {
					result.Error = VariantValue.ErrorGettingData;
					return false;
				}
				if (calculateCondtionsResult == ConditionCalculationResult.False)
					return false;
				value = vector[relativeColumnIndex];
				if (value == VariantValue.ErrorGettingData) {
					result.Error = VariantValue.ErrorGettingData;
					return false;
				}
				if (!result.ShouldProcessValueCore(value))
					return false;
			}
			if (condition != null) {
				ConditionCalculationResult calculatedCondition = condition.Calculate(context, vector, relativeRowIndex);
				if (calculatedCondition == ConditionCalculationResult.ErrorGettingData) {
					result.Error = VariantValue.ErrorGettingData;
					return false;
				}
				return calculatedCondition == ConditionCalculationResult.True;
			}
			else
				if (value.IsError && !result.ProcessErrorValues)
					result.Error = value;
			return true;
		}
		protected virtual VariantValue CalculateFieldIndex(CellRange range, VariantValue field, WorkbookDataContext context) {
			if (field.IsText) {
				RangeHorizontalVector fields = new RangeHorizontalVector(range, 0);
				string fieldName = field.ToText(context).InlineTextValue;
				return CalculateFieldIndex(fields, fieldName, context);
			}
			field = field.ToNumeric(context);
			if (!field.IsNumeric)
				return VariantValue.ErrorInvalidValueInFunction;
			int index = (int)field.NumericValue - 1;
			if (index < 0 || index >= range.Width)
				return VariantValue.ErrorInvalidValueInFunction;
			return index;
		}
		public static VariantValue CalculateFieldIndex(RangeHorizontalVector fields, string fieldName, WorkbookDataContext context) {
			int count = fields.Count;
			for (int i = 0; i < count; i++) {
				VariantValue value = fields[i].ToText(context);
				if (value.IsError)
					return value;
				if (StringExtensions.CompareInvariantCultureIgnoreCase(fieldName, value.InlineTextValue) == 0)
					return i;
			}
			return VariantValue.ErrorInvalidValueInFunction;
		}
		protected abstract FunctionResult CreateInitialFunctionResult(WorkbookDataContext context);
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value));
			return collection;
		}
	}
	#endregion
	#region DatabaseFunctionCondition
	public abstract class DatabaseFunctionCondition {
		public abstract ConditionCalculationResult Calculate(WorkbookDataContext context, RangeHorizontalVector vector, int relativeRowIndex);
		public abstract int ValidRecordStart { get; }
		public abstract int ValidRecordEnd { get; }
	}
	#endregion
	#region DatabaseFunctionSimpleCondition
	public class DatabaseFunctionSimpleCondition : DatabaseFunctionCondition {
		readonly int fieldIndex;
		readonly DatabaseFunctionCalculationCondition condition;
		public DatabaseFunctionSimpleCondition(CellRange dataRange, VariantValue conditionText, WorkbookDataContext context, int fieldIndex) {
			Guard.ArgumentNotNull(dataRange, "dataRange");
			this.fieldIndex = fieldIndex;
			this.condition = new DatabaseFunctionCalculationCondition(dataRange, conditionText, context);
		}
		public override int ValidRecordStart { get { return 0; } }
		public override int ValidRecordEnd { get { return int.MaxValue; } }
		public override ConditionCalculationResult Calculate(WorkbookDataContext context, RangeHorizontalVector vector, int relativeRowIndex) {
			return condition.Calculate(context, relativeRowIndex, fieldIndex);
		}
	}
	#endregion
	#region DatabaseFunctionCalculationCondition
	public class DatabaseFunctionCalculationCondition : FunctionCalculationCondition {
		public DatabaseFunctionCalculationCondition(CellRangeBase conditionRange, VariantValue condition, WorkbookDataContext context)
			: base(conditionRange, condition, context) {
		}
		protected internal override void CreateEqualsCondition(VariantValue rightValue, string condition, WorkbookDataContext context) {
			if (rightValue.IsText)
				rightValue = condition + "*";
			base.CreateEqualsCondition(rightValue, condition, context);
		}
	}
	#endregion
	#region DatabaseFunctionAlwaysFalseCondition
	public class DatabaseFunctionAlwaysFalseCondition : DatabaseFunctionCondition {
		public override int ValidRecordStart { get { return int.MaxValue; } }
		public override int ValidRecordEnd { get { return -1; } }
		public override ConditionCalculationResult Calculate(WorkbookDataContext context, RangeHorizontalVector vector, int relativeRowIndex) {
			return ConditionCalculationResult.False;
		}
	}
	#endregion
	#region DatabaseFunctionAlwaysTrueCondition
	public class DatabaseFunctionAlwaysTrueCondition : DatabaseFunctionCondition {
		public override int ValidRecordStart { get { return 0; } }
		public override int ValidRecordEnd { get { return int.MaxValue; } }
		public override ConditionCalculationResult Calculate(WorkbookDataContext context, RangeHorizontalVector vector, int relativeRowIndex) {
			return ConditionCalculationResult.True;
		}
	}
	#endregion
	#region DatabaseFunctionConditionContainer(abstract class)
	public abstract class DatabaseFunctionConditionContainer : DatabaseFunctionCondition {
		readonly List<DatabaseFunctionCondition> conditions = new List<DatabaseFunctionCondition>();
		int validRecordStart;
		int validRecordEnd;
		protected DatabaseFunctionConditionContainer() {
			this.validRecordStart = 0;
			this.validRecordEnd = Int32.MaxValue;
		}
		protected IList<DatabaseFunctionCondition> Conditions { get { return conditions; } }
		public int ConditionsCount { get { return conditions.Count; } }
		public override int ValidRecordStart { get { return validRecordStart; } }
		public override int ValidRecordEnd { get { return validRecordEnd; } }
		public virtual void AddCondition(DatabaseFunctionCondition condition) {
			this.conditions.Add(condition);
		}
		protected void SetValidRecordStart(int value) {
			this.validRecordStart = value;
		}
		protected void SetValidRecordEnd(int value) {
			this.validRecordEnd = value;
		}
	}
	#endregion
	#region DatabaseFunctionCalculationOrCondition
	public class DatabaseFunctionCalculationOrCondition : DatabaseFunctionConditionContainer {
		public DatabaseFunctionCalculationOrCondition()
			: base() {
		}
		public override ConditionCalculationResult Calculate(WorkbookDataContext context, RangeHorizontalVector vector, int relativeRowIndex) {
			if (relativeRowIndex > ValidRecordEnd || relativeRowIndex < ValidRecordStart)
				return ConditionCalculationResult.False;
			int count = ConditionsCount;
			for (int i = 0; i < count; i++) {
				ConditionCalculationResult calculationResult = Conditions[i].Calculate(context, vector, relativeRowIndex);
				if (calculationResult != ConditionCalculationResult.False)
					return calculationResult;
			}
			return ConditionCalculationResult.False;
		}
		public override void AddCondition(DatabaseFunctionCondition condition) {
			SetValidRecordStart(Math.Min(ValidRecordStart, condition.ValidRecordStart));
			SetValidRecordEnd(Math.Max(ValidRecordEnd, condition.ValidRecordEnd));
			base.AddCondition(condition);
		}
	}
	#endregion
	#region DatabaseFunctionCalculationAndCondition
	public class DatabaseFunctionCalculationAndCondition : DatabaseFunctionConditionContainer {
		public DatabaseFunctionCalculationAndCondition()
			: base() {
		}
		public override ConditionCalculationResult Calculate(WorkbookDataContext context, RangeHorizontalVector vector, int relativeRowIndex) {
			if (relativeRowIndex > ValidRecordEnd || relativeRowIndex < ValidRecordStart)
				return ConditionCalculationResult.False;
			int count = ConditionsCount;
			for (int i = 0; i < count; i++) {
				ConditionCalculationResult calculationResult = Conditions[i].Calculate(context, vector, relativeRowIndex);
				if (calculationResult != ConditionCalculationResult.True)
					return calculationResult;
			}
			return ConditionCalculationResult.True;
		}
		public override void AddCondition(DatabaseFunctionCondition condition) {
			SetValidRecordStart(Math.Max(ValidRecordStart, condition.ValidRecordStart));
			SetValidRecordEnd(Math.Min(ValidRecordEnd, condition.ValidRecordEnd));
			base.AddCondition(condition);
		}
	}
	#endregion
	#region DatabaseFunctionFormulaCondition
	public class DatabaseFunctionFormulaCondition : DatabaseFunctionCondition {
		readonly ParsedExpression expression;
		readonly int validRecordStart;
		readonly int validRecordEnd;
		readonly CellPosition dataRangeTopLeft;
		public DatabaseFunctionFormulaCondition(ParsedExpression expression, int validRecordStart, int validRecordEnd, CellRange dataRange) {
			Guard.ArgumentNotNull(expression, "expression");
			this.expression = expression;
			this.validRecordStart = validRecordStart;
			this.validRecordEnd = validRecordEnd;
			this.dataRangeTopLeft = dataRange.TopLeft;
		}
		public override int ValidRecordStart { get { return validRecordStart; } }
		public override int ValidRecordEnd { get { return validRecordEnd; } }
		public override ConditionCalculationResult Calculate(WorkbookDataContext context, RangeHorizontalVector vector, int relativeRowIndex) {
			if (relativeRowIndex < validRecordStart || relativeRowIndex > validRecordEnd)
				return ConditionCalculationResult.False;
			context.PushCurrentCell(dataRangeTopLeft.Column, dataRangeTopLeft.Row + relativeRowIndex);
			try {
				VariantValue result = expression.Evaluate(context);
				return result.ToBoolean(context).BooleanValue ? ConditionCalculationResult.True : ConditionCalculationResult.False;
			}
			finally {
				context.PopCurrentCell();
			}
		}
	}
	#endregion
	#region ConditionFormulaPrepareWalker
	public class ConditionFormulaPrepareWalker : ReplaceThingsPRNVisitor {
		#region Fields
		bool correctExpression = true;
		readonly CellRange dataRange;
		readonly WorkbookDataContext context;
		readonly RangeHorizontalVector dataFields;
		int validRecordStart;
		int validRecordEnd;
		#endregion
		public ConditionFormulaPrepareWalker(CellRange dataRange, RangeHorizontalVector dataFields, WorkbookDataContext context) {
			this.dataRange = dataRange;
			this.context = context;
			this.dataFields = dataFields;
			this.validRecordStart = 0;
			this.validRecordEnd = dataRange.Height - 1;
		}
		#region Properties
		public int ValidRecordStart { get { return validRecordStart; } }
		public int ValidRecordEnd { get { return validRecordEnd; } }
		public bool IsValidExpression { get { return correctExpression && validRecordStart <= dataRange.Height && validRecordEnd >= validRecordStart; } }
		#endregion
		bool CheckSheetDefinition(int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			if (sheetDefinition.IsExternalReference || sheetDefinition.Is3DReference)
				return false;
			if (sheetDefinition.GetSheetStart(context) != dataRange.Worksheet)
				return false;
			return true;
		}
		CellOffset GetShiftedCellOffset(CellOffset offset) {
			int column = offset.Column;
			int row = offset.Row;
			if (offset.ColumnType == CellOffsetType.Offset)
				column += context.CurrentColumnIndex - dataRange.TopLeft.Column;
			if (offset.RowType == CellOffsetType.Offset)
				row += context.CurrentRowIndex - dataRange.TopLeft.Row;
			return new CellOffset(column, row, offset.ColumnType, offset.RowType);
		}
		#region Ref
		bool ProcessCellPosition(CellPosition position) {
			bool isRelativeReference = position.RowType != PositionType.Absolute; 
			if (!isRelativeReference)
				return false;
			bool inDataRangeColumn = position.Column >= dataRange.TopLeft.Column && position.Column <= dataRange.BottomRight.Column;
			if (!inDataRangeColumn) {
				correctExpression = false;
				return false;
			}
			int dRow = dataRange.TopLeft.Row - position.Row;
			if (dRow > 0)
				validRecordStart = Math.Max(validRecordStart, dRow);
			else
				validRecordEnd = Math.Min(validRecordEnd, dataRange.Height + dRow - 1);
			return IsValidExpression;
		}
		void ProcessRefRel(ParsedThingRefRel thing) {
			CellPosition position = thing.Location.ToCellPosition(context);
			if (ProcessCellPosition(position))
				thing.Location = GetShiftedCellOffset(thing.Location);
		}
		public override void Visit(ParsedThingRef thing) {
			if (ProcessCellPosition(thing.Position)) {
				CellOffset offset = thing.Position.ToCellOffset(dataRange.TopLeft.Column, dataRange.TopLeft.Row);
				ParsedThingRefRel newPtg = new ParsedThingRefRel(offset);
				newPtg.DataType = thing.DataType;
				ReplaceCurrentExpression(newPtg);
			}
			base.Visit(thing);
		}
		public override void Visit(ParsedThingRef3d thing) {
			if (!CheckSheetDefinition(thing.SheetDefinitionIndex))
				return;
			if (ProcessCellPosition(thing.Position)) {
				CellOffset offset = thing.Position.ToCellOffset(dataRange.TopLeft.Column, dataRange.TopLeft.Row);
				ParsedThingRef3dRel newPtg = new ParsedThingRef3dRel(offset, thing.SheetDefinitionIndex);
				newPtg.DataType = thing.DataType;
				ReplaceCurrentExpression(newPtg);
			}
			base.Visit(thing);
		}
		public override void Visit(ParsedThingRefRel thing) {
			ProcessRefRel(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			if (!CheckSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ProcessRefRel(thing);
			base.Visit(thing);
		}
		#endregion
		#region Area
		bool ProcessCellArea(CellRange range) {
			bool isRelativeReference = range.TopLeft.RowType == PositionType.Relative || range.BottomRight.RowType == PositionType.Relative;
			if (!isRelativeReference)
				return false;
			bool inDataRangeColumn = range.TopLeft.Column <= dataRange.BottomRight.Column && range.BottomRight.Column >= dataRange.TopLeft.Column;
			if (!inDataRangeColumn) {
				correctExpression = false;
				return false;
			}
			if ((range.BottomRight.RowType == PositionType.Absolute && range.BottomRight.Row >= dataRange.TopLeft.Row && range.BottomRight.Row <= dataRange.BottomRight.Row) ||
				(range.TopLeft.RowType == PositionType.Absolute && range.TopLeft.Row >= dataRange.TopLeft.Row && range.TopLeft.Row <= dataRange.BottomRight.Row))
				return true;
			int height = dataRange.Height;
			int minRow = range.TopLeft.Row;
			int maxRow = range.BottomRight.Row;
			if (range.BottomRight.RowType == PositionType.Relative)
				maxRow += height - 1;
			else
				if (range.TopLeft.RowType == PositionType.Relative)
					maxRow = Math.Max(maxRow, minRow + height - 1);
			validRecordStart = Math.Max(validRecordStart, dataRange.BottomRight.Row - maxRow);
			validRecordEnd = Math.Min(validRecordEnd, dataRange.BottomRight.Row - minRow);
			return IsValidExpression;
		}
		bool ProcessAreaRel(ParsedThingAreaN thing) {
			CellPosition topLeft = thing.First.ToCellPosition(context);
			CellPosition bottomRight = thing.Last.ToCellPosition(context);
			return ProcessCellArea(new CellRange(dataRange.Worksheet, topLeft, bottomRight));
		}
		public override void Visit(ParsedThingArea thing) {
			if (ProcessCellArea(thing.CellRange)) {
				CellOffset topLeftOffset = thing.TopLeft.ToCellOffset(dataRange.TopLeft.Column, dataRange.TopLeft.Row);
				CellOffset bottomRightOffset = thing.BottomRight.ToCellOffset(dataRange.TopLeft.Column, dataRange.TopLeft.Row);
				ParsedThingAreaNRestrictedRange newPtg = new ParsedThingAreaNRestrictedRange(topLeftOffset, bottomRightOffset, dataRange);
				newPtg.DataType = thing.DataType;
				ReplaceCurrentExpression(newPtg);
			}
			base.Visit(thing);
		}
		public override void Visit(ParsedThingArea3d thing) {
			if (!CheckSheetDefinition(thing.SheetDefinitionIndex))
				return;
			if (ProcessCellArea(thing.CellRange)) {
				CellOffset topLeftOffset = thing.TopLeft.ToCellOffset(dataRange.TopLeft.Column, dataRange.TopLeft.Row);
				CellOffset bottomRightOffset = thing.BottomRight.ToCellOffset(dataRange.TopLeft.Column, dataRange.TopLeft.Row);
				ParsedThingArea3dRelRestrictedRange newPtg = new ParsedThingArea3dRelRestrictedRange(topLeftOffset, bottomRightOffset, thing.SheetDefinitionIndex, dataRange);
				newPtg.DataType = thing.DataType;
				ReplaceCurrentExpression(newPtg);
			}
			base.Visit(thing);
		}
		public override void Visit(ParsedThingAreaN thing) {
			if (ProcessAreaRel(thing)) {
				CellOffset first = GetShiftedCellOffset(thing.First);
				CellOffset last = GetShiftedCellOffset(thing.Last);
				ParsedThingAreaNRestrictedRange newPtg = new ParsedThingAreaNRestrictedRange(first, last, dataRange);
				newPtg.DataType = thing.DataType;
				ReplaceCurrentExpression(newPtg);
			}
			base.Visit(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			if (!CheckSheetDefinition(thing.SheetDefinitionIndex))
				return;
			if (ProcessAreaRel(thing)) {
				CellOffset first = GetShiftedCellOffset(thing.First);
				CellOffset last = GetShiftedCellOffset(thing.Last);
				ParsedThingArea3dRelRestrictedRange newPtg = new ParsedThingArea3dRelRestrictedRange(first, last, thing.SheetDefinitionIndex, dataRange);
				newPtg.DataType = thing.DataType;
				ReplaceCurrentExpression(newPtg);
			}
			base.Visit(thing);
		}
		#endregion
		#region Defined Name
		public override void Visit(ParsedThingName thing) {
			VariantValue fieldIndex = WorksheetDatabaseFunctionBase.CalculateFieldIndex(dataFields, thing.DefinedName, context);
			if (fieldIndex.IsNumeric) {
				CellOffset offset = new CellOffset((int)fieldIndex.NumericValue + dataRange.TopLeft.Column, 0, CellOffsetType.Position, CellOffsetType.Offset);
				ParsedThingRefRel newPtg = new ParsedThingRefRel(offset);
				newPtg.DataType = thing.DataType;
				ReplaceCurrentExpression(newPtg);
			}
			base.Visit(thing);
		}
		public override void Visit(ParsedThingNameX thing) {
			if (!CheckSheetDefinition(thing.SheetDefinitionIndex))
				return;
			VariantValue fieldIndex = WorksheetDatabaseFunctionBase.CalculateFieldIndex(dataFields, thing.DefinedName, context);
			if (fieldIndex.IsNumeric) {
				CellOffset offset = new CellOffset((int)fieldIndex.NumericValue + dataRange.TopLeft.Column, 0, CellOffsetType.Position, CellOffsetType.Offset);
				ParsedThingRef3dRel newPtg = new ParsedThingRef3dRel(offset, thing.SheetDefinitionIndex);
				newPtg.DataType = thing.DataType;
				ReplaceCurrentExpression(newPtg);
			}
			base.Visit(thing);
		}
		#endregion
	}
	#endregion
	#region Helper ptg
	class ParsedThingAreaNRestrictedRange : ParsedThingAreaN, IHeplerParsedThing {
		readonly CellRange restrictiveRange;
		public ParsedThingAreaNRestrictedRange(CellOffset first, CellOffset last, CellRange restrictiveRange)
			: base(first, last) {
			this.restrictiveRange = restrictiveRange;
		}
		protected internal override CellRange PreEvaluateReference(WorkbookDataContext context) {
			CellRange result = base.PreEvaluateReference(context);
			VariantValue restrictedResult = result.IntersectionWith(restrictiveRange);
			return (CellRange)restrictedResult.CellRangeValue;
		}
	}
	class ParsedThingArea3dRelRestrictedRange : ParsedThingArea3dRel, IHeplerParsedThing {
		readonly CellRange restrictiveRange;
		public ParsedThingArea3dRelRestrictedRange(CellOffset first, CellOffset last, int sheetDefinitionIndex, CellRange restrictiveRange)
			: base(first, last, sheetDefinitionIndex) {
			this.restrictiveRange = restrictiveRange;
		}
		protected internal override CellRange PreEvaluateReference(WorkbookDataContext context) {
			CellRange result = base.PreEvaluateReference(context);
			VariantValue restrictedResult = result.IntersectionWith(restrictiveRange);
			return (CellRange)restrictedResult.CellRangeValue;
		}
	}
	#endregion
}
